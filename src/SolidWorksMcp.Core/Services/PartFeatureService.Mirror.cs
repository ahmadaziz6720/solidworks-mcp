using System.Text.RegularExpressions;
using SolidWorks.Interop.sldworks;
using SolidWorksMcp.Core.Com;
using SolidWorksMcp.Core.Geometry;
using SolidWorksMcp.Core.Model;
using SolidWorksMcp.Core.Prefs;
using SolidWorksMcp.Core.Verify;

namespace SolidWorksMcp.Core.Services;

/// <summary>feature_mirror of bodies (Mirror body): each solid body reflected as a body of its own, or merged.</summary>
public sealed partial class PartFeatureService
{
    private const int MirrorBodyMark = 256; // bodies to mirror; the plane on mark 2
    private static readonly FeatureSpec MirrorBodySpec = new("feature_mirror", ["MirrorSolid", "MirrorPattern"], VolumeRule.Increase, RequireFaces: false);

    private static FeatureResult MirrorBodies(SwContext ctx, IModelDoc2 d, MirrorRequest q, Resolved planeRef, (Vec3 Q, Vec3 M) plane)
    {
        var part = (IPartDoc)d;
        if (q.Bodies!.Count > 50) throw SwException.InvalidArgument($"At most 50 bodies per call (got {q.Bodies.Count}).");
        var refs = q.Bodies.Select((s, i) => BodyRef(s, i)).ToList();
        var targets = BodyKit.ResolveBodies(ctx, d, refs, "bodies", BodyKind.Solid);
        var seeds = targets.Select(t => BodyKit.Geom(ctx, d, (IBody2)t.Obj, withVertices: true)).ToList();
        var sigs = seeds.Select(s => Signatures(s.Body.Faces(), out _)).ToList();
        var x = Rigid.Reflect(plane.Q, plane.M);
        string about = $"about [{Dir(plane.M)}] through [{Pt(plane.Q)}]";

        using var prefs = ctx.SystemPrefs();
        prefs.Toggle(SystemPrefIds.ShowErrorsEveryRebuild, false);
        var before = Snapshot.Take(d);
        d.ClearSelection2(true);
        BodyKit.SelectAll(d, targets, MirrorBodyMark);
        if (!planeRef.Select(d, true, 2)) throw new SwException(ErrorCodes.FeatureFailed, $"Could not select {planeRef.Label} as the mirror plane.");
        object? api = d.FeatureManager.InsertMirrorFeature2(true, false, q.Merge, false, 0);
        d.ClearSelection2(true);

        var o = Verify(ctx, d, before, MirrorBodySpec,
            (_, _, checks) => BodyMirrorIntent(ctx, part, d, seeds, sigs, x, q.Merge, before, about, checks),
            q.KeepOnFailure, api, q.Merge
                ? "With merge, every copy must touch a body (SOLIDWORKS makes nothing otherwise); mirror the others with merge:false."
                : "Check the bodies (body_list) and the plane.");
        Rename(ctx, o.Main, q.Name);
        return FeatureVerifier.Result(ctx, d, o, resolved: [.. targets.Select(t => t.Info("bodies")), planeRef.Info("plane")]);
    }

    /// <summary>A body by its id ("B2") or its name.</summary>
    private static EntityRef BodyRef(string? s, int i)
    {
        string v = s?.Trim() ?? "";
        if (v.Length == 0) throw SwException.InvalidArgument($"bodies[{i}] is empty.", "Pass a body's id or name from body_list.");
        return Regex.IsMatch(v, "^B[0-9]+$") ? new EntityRef { Id = v } : new EntityRef { Name = v };
    }

    /// <summary>
    /// Tier B of a body mirror. merge:false: one new solid body per seed, each the seed reflected (volume, area, center of
    /// mass, sampled vertices) and the volume grown by the seeds' volume. merge:true: the volume grew by at most the seeds'
    /// volume (less by what the copies overlap), and each copy that is not a body of its own has its faces found reflected
    /// on the bodies.
    /// </summary>
    private static List<string> BodyMirrorIntent(SwContext ctx, IPartDoc part, IModelDoc2 d, List<BodyGeom> seeds, List<List<FaceSig>> sigs,
        Rigid x, bool merge, Snapshot before, string about, List<string> checks)
    {
        var after = BodyKit.SolidGeoms(ctx, part, d);
        var problems = new List<string>();
        int n = seeds.Count;
        double sumV = seeds.Sum(s => s.Volume), dV = after.Sum(a => a.Volume) - before.Volume, tol = 1e-12 + 1e-6 * sumV;
        var apart = Enumerable.Range(0, n).Where(i => CopyOf(seeds[i], after, x) is not null).ToList();
        string bodies = n == 1 ? "1 body" : $"{n} bodies";
        if (!merge)
        {
            if (after.Count != before.SolidBodies + n) problems.Add($"the part has {after.Count} solid bodies, expected {before.SolidBodies + n}");
            foreach (var s in seeds.Where((_, i) => !apart.Contains(i)).Take(3))
                problems.Add($"no body is the reflection of {s.Label} (center of mass expected at [{Pt(x.Pt(s.Center))}] mm)");
            if (Math.Abs(dV - sumV) > tol) problems.Add($"the volume grew by {Units.Mm3(dV)} mm³, not the seeds' {Units.Mm3(sumV)} mm³");
            if (problems.Count == 0)
                checks.Add($"{bodies} copied as bodies of their own {about}: each copy's volume, area, center of mass and sampled vertices are the seed's reflected");
            return problems;
        }

        if (dV > sumV + tol) problems.Add($"the volume grew by {Units.Mm3(dV)} mm³, more than the seeds' {Units.Mm3(sumV)} mm³");
        var joined = Enumerable.Range(0, n).Where(i => !apart.Contains(i)).ToList();
        if (joined.Count > 0)
        {
            var faces = after.SelectMany(a => a.Body.Faces()).ToList();
            if (faces.Count > MaxSignatureFaces) checks.Add($"the merged copies' faces not checked ({faces.Count} faces)");
            else
            {
                var res = Signatures(faces, out _);
                int matched = 0, total = 0;
                foreach (int i in joined)
                {
                    var (m, t) = Score(sigs[i], res, [x]);
                    if (t > 0 && m == 0) problems.Add($"no face of {seeds[i].Label} is found reflected {about}");
                    matched += m;
                    total += t;
                }
                if (problems.Count == 0 && total > 0)
                    checks.Add($"merged copies' faces at their reflections {about}: {matched}/{total} (faces on the plane join)");
            }
        }
        if (problems.Count > 0) return problems;
        checks.Add($"volume +{Units.Mm3(dV)} mm³ = the seeds' {Units.Mm3(sumV)} − {Units.Mm3(Math.Max(0, sumV - dV))} where the copies overlap");
        if (apart.Count > 0)
            ctx.Warn($"{apart.Count} of {n} copies touch no body and stay bodies of their own: {string.Join(", ", apart.Take(4).Select(i => seeds[i].Label))}.");
        else checks.Add($"{bodies} mirrored and merged {about}: the part has {after.Count} solid bodies");
        return problems;
    }

    /// <summary>The body that is <paramref name="s"/> reflected, if it is one of its own.</summary>
    private static BodyGeom? CopyOf(BodyGeom s, List<BodyGeom> after, Rigid x)
    {
        var c = x.Pt(s.Center);
        var sample = MoveMath.Sample(s.Vertices, 24);
        return after.FirstOrDefault(a => BodyKit.SameSize(a, s) && a.Center.DistanceTo(c) <= BodyKit.CenterTol &&
                                         (sample.Count == 0 || MoveMath.AllNear(sample.Select(x.Pt), a.Vertices, BodyKit.VertexTol)));
    }
}
