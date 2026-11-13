using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorksMcp.Core.Com;
using SolidWorksMcp.Core.Geometry;
using SolidWorksMcp.Core.Model;
using SolidWorksMcp.Core.Verify;

namespace SolidWorksMcp.Core.Services;

/// <summary>Extrude options (validated by the tool layer's types; checked again here).</summary>
public sealed record ExtrudeRequest
{
    public string? Doc { get; init; }
    public string? Sketch { get; init; }
    public bool Cut { get; init; }
    /// <summary>blind | through_all | through_all_both | mid_plane.</summary>
    public string End { get; init; } = "blind";
    public double? DepthMm { get; init; }
    /// <summary>normal | reverse | auto | "+y" | "x,y,z".</summary>
    public string? Direction { get; init; }
    public bool Merge { get; init; } = true;
    public double? DraftDeg { get; init; }
    public bool DraftOutward { get; init; }
    public string? Name { get; init; }
    public bool KeepOnFailure { get; init; }
}

/// <summary>Part features: extrude (boss/cut) and fillet in M0. Every call is verified and rolled back on failure.</summary>
public sealed partial class FeatureService(SwSession session)
{
    private static readonly FeatureSpec BossSpec = new("feature_extrude", ["Extrusion", "ICE", "Boss", "BossThin"], VolumeRule.Increase);
    private static readonly FeatureSpec CutSpec = new("feature_extrude", ["Cut", "ICE", "CutThin"], VolumeRule.Decrease);
    private static readonly FeatureSpec FilletSpec = new("feature_fillet", ["Fillet"], VolumeRule.Change);

    // ------------------------------------------------------------------ extrude

    public Task<FeatureResult> ExtrudeAsync(ExtrudeRequest q, CancellationToken ct = default) =>
        session.RunAsync("feature_extrude", ctx =>
        {
            var d = (IModelDoc2)ctx.Part();
            var entry = ctx.DocEntry;
            string end = (q.End ?? "blind").Trim().ToLowerInvariant();
            if (end is not ("blind" or "through_all" or "through_all_both" or "mid_plane"))
                throw SwException.InvalidArgument($"end '{q.End}' is not supported.", "Use blind, through_all, through_all_both or mid_plane.");
            double depth = 0;
            if (end is "blind" or "mid_plane")
            {
                if (q.DepthMm is not double dm || !double.IsFinite(dm) || dm <= 0)
                    throw SwException.InvalidArgument($"end '{end}' needs depthMm > 0.");
                depth = Units.M(dm);
            }
            if (q.DraftDeg is double dd && (!double.IsFinite(dd) || dd < 0 || dd >= 89))
                throw SwException.InvalidArgument("draftDeg must be between 0 and 89.");

            // The profile: the server's open sketch (closed first), else the named one, else the last one finished.
            var (sketchFeat, sketch) = ProfileSketch(ctx, d, q.Sketch);
            var contours = SketchService.Contours(sketch);
            if (contours.ClosedContours == 0)
                throw SwException.InvalidArgument($"Sketch '{sketchFeat.Name}' has no closed contour ({contours.Status}{(contours.Problem is null ? "" : ", " + contours.Problem)}).",
                    "Close the profile (rectangle, circle, closed polyline) before extruding.");

            var s2m = SketchService.SketchToModel(sketch);
            var origin = s2m.ApplyPoint(Vec3.Zero);
            var normal = s2m.ApplyVector(Vec3.UnitX).Cross(s2m.ApplyVector(Vec3.UnitY)).Normalized();
            var before = Snapshot.Take(d);
            var (alongNormal, oblique) = ResolveDirection(q, normal, origin, before, end);
            var capNormal = alongNormal ? normal : -normal;
            var dirModel = oblique ?? capNormal;
            // API flag: bosses go along the sketch normal by default; cuts go against it by default.
            bool dirFlag = q.Cut ? alongNormal : !alongNormal;

            bool sd = end != "through_all_both";
            int t1 = end switch
            {
                "blind" => (int)swEndConditions_e.swEndCondBlind,
                "mid_plane" => (int)swEndConditions_e.swEndCondMidPlane,
                _ => (int)swEndConditions_e.swEndCondThroughAll,
            };
            int t2 = end == "through_all_both" ? (int)swEndConditions_e.swEndCondThroughAll : (int)swEndConditions_e.swEndCondBlind;
            bool draft = q.DraftDeg is > 0;
            double draftRad = draft ? Units.Rad(q.DraftDeg!.Value) : 0;

            // A direction off the normal: a 3D sketch line along it, made the extrude's direction reference (S53).
            ISketchSegment? dirLine = oblique is Vec3 ov ? DirectionLine(d, origin, ov) : null;

            d.ClearSelection2(true);
            if (!sketchFeat.Select2(false, 0))
                throw new SwException(ErrorCodes.FeatureFailed, $"Could not select sketch '{sketchFeat.Name}'.");
            var fm = d.FeatureManager;
            object? api = q.Cut
                ? fm.FeatureCut4(sd, false, dirFlag, t1, t2, depth, depth, draft, false, q.DraftOutward, false, draftRad, 0,
                    false, false, false, false, false, true, true, true, true, false,
                    (int)swStartConditions_e.swStartSketchPlane, 0, false, false)
                : fm.FeatureExtrusion3(sd, false, dirFlag, t1, t2, depth, depth, draft, false, q.DraftOutward, false, draftRad, 0,
                    false, false, false, false, q.Merge, true, true,
                    (int)swStartConditions_e.swStartSketchPlane, 0, false);
            d.ClearSelection2(true);
            if (dirLine is not null && api is IFeature made)
            {
                try { SetDirectionReference(d, made, dirLine, dirModel); }
                catch (SwException ex) { throw new SwException(ex.Code, ex.Message + " " + FeatureVerifier.Rollback(ctx, d, before), ex.Hint, ex); }
            }

            var profile = sketch.Points().Select(sp => s2m.ApplyPoint(new Vec3(sp.X, sp.Y, sp.Z))).ToList();
            var o = FeatureVerifier.Check(ctx, d, before, q.Cut ? CutSpec : BossSpec,
                (main, after, checks) => ExtrudeIntent(main, end, depth, origin, dirModel, capNormal, oblique is null ? null : profile, q.Cut, checks),
                q.KeepOnFailure, api, q.Name);
            entry.LastSketch = null; // consumed
            Rename(ctx, o.Main, q.Name);
            return FeatureVerifier.Result(ctx, d, o, sketchFeat.Name, dirModel);
        }, RunOptions.Write(q.Doc), ct);

    /// <summary>
    /// Tier B: a cap face of the new feature lies at the requested depth along the direction. The caps are parallel to
    /// the sketch plane (normal <paramref name="capNormal"/>); along an oblique direction a depth d puts them d·cos θ off
    /// it, and the profile's points moved d along the direction lie on a cap (<paramref name="profile"/>, oblique only).
    /// </summary>
    private static IEnumerable<string> ExtrudeIntent(IFeature main, string end, double depth, Vec3 origin, Vec3 dir, Vec3 capNormal,
        List<Vec3>? profile, bool cut, List<string> checks)
    {
        if (end is not ("blind" or "mid_plane")) yield break;
        var expect = end == "mid_plane" ? new[] { depth / 2, -depth / 2 } : new[] { depth };
        double cos = dir.Dot(capNormal);
        var offsets = new List<double>();
        var caps = new List<IFace2>();
        foreach (var f in ComArgs.Objects<IFace2>((object?)main.GetFaces()))
        {
            if (Topology.SurfaceKind(f.Surface()) != "plane") continue;
            var n = f.NormalVec();
            if (n.Length < 3 || !new Vec3(n[0], n[1], n[2]).IsParallelTo(capNormal, 1e-6)) continue;
            var p = Topology.InteriorPoint(f);
            offsets.Add((p - origin).Dot(capNormal) / cos); // the depth along the direction that puts a cap there
            caps.Add(f);
        }
        if (profile is not null)
        {
            // The caps' offset alone fits the normal too (a shorter extrude); the profile moved along the direction tells.
            var moved = expect.SelectMany(e => profile.Select(p => p + dir * e)).ToList();
            if (moved.Count > 0 && !moved.Any(m => caps.Any(f => Topology.DistanceToFace(f, m) < 1e-6)))
                yield return $"no cap where the profile lands moved along [{TopoQuery.Fmt(Units.Unit(dir))}]";
            else if (moved.Count > 0)
                checks.Add($"profile moved along [{TopoQuery.Fmt(Units.Unit(dir))}]");
        }
        // A cut whose far end leaves the body has no floor there (a hole through a plate, a pocket that opens through a
        // slanted underside). With no floor anywhere but where one was asked, that is the cut made as asked: it passes on
        // the volume it removed, and a check line says it broke out (S40). A floor at another depth still fails. A blind cut
        // from a plane inside the body leaves a face on that plane, where it starts (S64 a): that one counts as no floor,
        // once every face of the cut lies on the asked side of the plane (a cut gone the other way has its faces behind it).
        bool AtStart(double x) => cut && end == "blind" && Math.Abs(x) < 1e-6;
        bool brokeOutOnly = cut && offsets.All(x => AtStart(x) || expect.Any(e => Math.Abs(x - e) < 1e-6));
        if (brokeOutOnly && offsets.Any(AtStart) && !expect.All(e => offsets.Any(x => Math.Abs(x - e) < 1e-6)))
            brokeOutOnly = ComArgs.Objects<IFace2>((object?)main.GetFaces())
                .All(f => (Topology.InteriorPoint(f) - origin).Dot(capNormal) / cos > -1e-6);
        foreach (var e in expect)
        {
            if (offsets.Any(x => Math.Abs(x - e) < 1e-6)) checks.Add($"{(cut ? "floor" : "cap")} at {Units.Mm(e)} mm");
            else if (brokeOutOnly)
            {
                string? depthDim = end == "blind"
                    ? FeatureVerifier.Dimensions(main).FirstOrDefault(x => x.Unit == "mm" && Math.Abs(x.Value - Units.Mm(depth)) < 1e-6)?.Name
                    : null;
                checks.Add($"no floor at {Units.Mm(e)} mm: the cut broke out of the body before its end" +
                           (depthDim is null ? "" : $" ({depthDim} reads {Units.Mm(depth)} mm)"));
            }
            else
                yield return $"no {(cut ? "floor" : "cap")} face at {Units.Mm(e)} mm along [{TopoQuery.Fmt(Units.Unit(dir))}]" +
                             (offsets.Count == 0 ? "" : $" (planar faces at {string.Join(", ", offsets.Select(x => Units.Mm(x)))} mm)");
        }
    }

    /// <summary>
    /// Whether the feature goes to the sketch normal's side, and a direction off the normal when one was given (a vector
    /// not parallel to it: the extrude goes along the vector, S53).
    /// </summary>
    private static (bool AlongNormal, Vec3? Oblique) ResolveDirection(ExtrudeRequest q, Vec3 normal, Vec3 origin, Snapshot before, string end) =>
        Side(q, normal, origin, before) switch
        {
            bool b => (b, null),
            null => Oblique(q.Direction!, normal),
        };

    private static (bool, Vec3?) Oblique(string direction, Vec3 normal)
    {
        var axis = TopoQuery.Axis(direction, "direction").Normalized();
        double dot = axis.Dot(normal);
        if (Math.Abs(dot) < 1e-6)
            throw SwException.InvalidArgument($"direction [{TopoQuery.Fmt(Units.Unit(axis))}] lies in the sketch plane; an extrude leaves the plane along the normal [{TopoQuery.Fmt(Units.Unit(normal))}] or at an angle to it.");
        if (Math.Abs(Math.Abs(dot) - 1) < 1e-9) return (dot > 0, null);
        if (Math.Abs(dot) < Math.Cos(Units.Rad(89)))
            throw SwException.InvalidArgument($"direction [{TopoQuery.Fmt(Units.Unit(axis))}] is {Units.Deg(Math.Acos(Math.Abs(dot)))}° off the sketch normal; at most 89°.");
        return (dot > 0, axis);
    }

    /// <summary>The side for the named directions (normal, reverse, auto); null for an axis or vector.</summary>
    private static bool? Side(ExtrudeRequest q, Vec3 normal, Vec3 origin, Snapshot before)
    {
        switch (q.Direction?.Trim().ToLowerInvariant())
        {
            case null or "auto" when q.Cut:
            {
                // Into the material: compare the body box against the sketch plane.
                if (before.Box is not double[] b) return false;
                double max = double.MinValue, min = double.MaxValue;
                foreach (var x in new[] { b[0], b[3] })
                    foreach (var y in new[] { b[1], b[4] })
                        foreach (var z in new[] { b[2], b[5] })
                        {
                            double s = (new Vec3(x, y, z) - origin).Dot(normal);
                            max = Math.Max(max, s);
                            min = Math.Min(min, s);
                        }
                bool plus = max > 1e-9, minus = min < -1e-9;
                if (plus && !minus) return true;
                return false; // material on the −normal side, or on both (SOLIDWORKS' own default)
            }
            case null or "auto" or "normal":
                return true;
            case "reverse":
                return false;
        }
        return null;
    }

    /// <summary>A 3D sketch with one line from <paramref name="from"/> along <paramref name="dir"/> (10 mm), drawn without inference.</summary>
    private static ISketchSegment DirectionLine(IModelDoc2 d, Vec3 from, Vec3 dir)
    {
        var sm = d.SketchManager;
        d.ClearSelection2(true);
        sm.Insert3DSketch(true);
        if (sm.ActiveSketch is not ISketch sketch)
            throw new SwException(ErrorCodes.FeatureFailed, "SOLIDWORKS did not open a 3D sketch for the direction.");
        bool addToDb = sm.AddToDB;
        ISketchSegment? line;
        try
        {
            sm.AddToDB = true;
            var to = from + dir * 0.01;
            line = sm.CreateLine(from.X, from.Y, from.Z, to.X, to.Y, to.Z);
        }
        finally
        {
            sm.AddToDB = addToDb;
            sm.Insert3DSketch(true);
        }
        if (sm.ActiveSketch is not null) throw new SwException(ErrorCodes.SketchState, $"The direction sketch '{((IFeature)sketch).Name}' did not close.");
        return line ?? throw new SwException(ErrorCodes.FeatureFailed, "Could not draw the direction line.");
    }

    /// <summary>Makes <paramref name="line"/> the extrude's direction reference, the way along <paramref name="dir"/>.</summary>
    private static void SetDirectionReference(IModelDoc2 d, IFeature f, ISketchSegment line, Vec3 dir)
    {
        var data = (object?)f.GetDefinition() as IExtrudeFeatureData2
                   ?? throw new SwException(ErrorCodes.Internal, $"'{f.Name}' has no extrude definition.");
        if (!data.AccessSelections((ModelDoc2)d, null))
            throw new SwException(ErrorCodes.FeatureFailed, $"SOLIDWORKS refused to open '{f.Name}' for its direction (AccessSelections).");
        bool done = false;
        try
        {
            data.SetDirectionReference(line, null);
            done = f.ModifyDefinition(data, d, null);
        }
        finally
        {
            if (!done) data.ReleaseSelectionAccess();
        }
        if (!done) throw new SwException(ErrorCodes.FeatureFailed, $"SOLIDWORKS refused the direction reference on '{f.Name}' (ModifyDefinition).");
    }

    /// <summary>The profile sketch: the server's open sketch (closed now), the named sketch, or the last finished one.</summary>
    private static (IFeature Feature, ISketch Sketch) ProfileSketch(SwContext ctx, IModelDoc2 d, string? name)
    {
        var entry = ctx.DocEntry;
        var sm = d.SketchManager;
        if (sm.ActiveSketch is ISketch open)
        {
            string openName = ((IFeature)open).Name;
            if (entry.ServerSketch != openName)
                throw new SwException(ErrorCodes.SketchState, $"Sketch '{openName}' is open, but this server didn't open it.",
                    "Ask the user to exit the sketch first.");
            if (name is not null && !string.Equals(name, openName, StringComparison.OrdinalIgnoreCase))
                throw new SwException(ErrorCodes.SketchState, $"Sketch '{openName}' is open; finish it before extruding '{name}'.");
            if (open.Is3D()) throw SwException.InvalidArgument("A 3D sketch can't be extruded.");
            sm.InsertSketch(true); // close it: features and mass properties need it closed
            if (sm.ActiveSketch is not null) throw new SwException(ErrorCodes.SketchState, $"Sketch '{openName}' did not close.");
            entry.ServerSketch = null;
            entry.LastSketch = openName;
            name = openName;
        }
        name ??= entry.LastSketch ?? throw SwException.InvalidArgument("No sketch to extrude.",
            "Pass 'sketch' (e.g. \"Sketch1\"), or draw one with sketch_start / sketch_add first.");
        var feat = EntityResolver.FeatureByName(d, name)
                   ?? throw new SwException(ErrorCodes.EntityNotFound, $"No sketch named '{name}'.", "feature_list shows the sketches.");
        if (feat.Specific() is not ISketch sk || EntityResolver.FeatureKind(feat) != EntityKind.Sketch)
            throw SwException.InvalidArgument($"'{name}' is a {feat.GetTypeName2()}, not a sketch.");
        return (feat, sk);
    }

    // ------------------------------------------------------------------ fillet

    public Task<FeatureResult> FilletAsync(string? doc, IReadOnlyList<EntityRef>? edges, IReadOnlyList<EntityRef>? faces,
        double radiusMm, bool propagate, string? name, bool keepOnFailure, CancellationToken ct = default) =>
        session.RunAsync("feature_fillet", ctx =>
        {
            var d = (IModelDoc2)ctx.Part();
            double r = Units.PositiveM(radiusMm, "radiusMm");
            if ((edges is null || edges.Count == 0) && (faces is null || faces.Count == 0))
                throw SwException.InvalidArgument("Pass 'edges' (and/or 'faces') to fillet.",
                    "E.g. edges: [{where:{curve:'line', parallelTo:'y'}, expect:4}] or [{id:'E3'}].");
            if (d.SketchManager.ActiveSketch is ISketch open)
                throw new SwException(ErrorCodes.SketchState, $"Sketch '{((IFeature)open).Name}' is open.", "Finish it with sketch_finish first.");
            var targets = new List<Resolved>();
            var resolved = new List<ResolvedInfo>();
            if (edges is { Count: > 0 })
            {
                var es = EntityResolver.ResolveAll(ctx, d, edges, "edges", EntityKind.Edge);
                targets.AddRange(es);
                resolved.AddRange(es.Select(x => x.Info("edges")));
            }
            if (faces is { Count: > 0 })
            {
                var fs = EntityResolver.ResolveAll(ctx, d, faces, "faces", EntityKind.Face);
                targets.AddRange(fs);
                resolved.AddRange(fs.Select(x => x.Info("faces")));
            }
            var before = Snapshot.Take(d);
            d.ClearSelection2(true);
            foreach (var t in targets)
                if (!t.Select(d, true, 0))
                    throw new SwException(ErrorCodes.FeatureFailed, $"Could not select {t.Label}.");
            int options = (int)(swFeatureFilletOptions_e.swFeatureFilletUniformRadius | swFeatureFilletOptions_e.swFeatureFilletAttachEdges |
                                swFeatureFilletOptions_e.swFeatureFilletKeepFeatures);
            if (propagate) options |= (int)swFeatureFilletOptions_e.swFeatureFilletPropagate;
            object? api = d.FeatureManager.FeatureFillet3(options, r, 0, 0, (int)swFeatureFilletType_e.swFeatureFilletType_Simple, 0, 0,
                ComArgs.Empty, ComArgs.Empty, ComArgs.Empty, ComArgs.Empty, ComArgs.Empty, ComArgs.Empty, ComArgs.Empty);
            d.ClearSelection2(true);
            var o = FeatureVerifier.Check(ctx, d, before, FilletSpec, (main, after, checks) => FilletIntent(main, r, checks), keepOnFailure, api);
            Rename(ctx, o.Main, name);
            return FeatureVerifier.Result(ctx, d, o, resolved: resolved);
        }, RunOptions.Write(doc), ct);

    /// <summary>Tier B: the fillet has a round face (cylinder, or torus tube) of the requested radius.</summary>
    private static IEnumerable<string> FilletIntent(IFeature main, double r, List<string> checks)
    {
        var radii = new List<double>();
        foreach (var f in ComArgs.Objects<IFace2>((object?)main.GetFaces()))
        {
            var s = f.Surface();
            if (s is null) continue;
            if (s.IsCylinder()) radii.Add(ComArgs.Doubles((object?)s.CylinderParams)[6]);
            else if (s.IsTorus()) { var p = ComArgs.Doubles((object?)s.TorusParams); if (p.Length > 7) radii.Add(p[7]); }
            else if (s.IsSphere()) radii.Add(ComArgs.Doubles((object?)s.SphereParams)[3]);
        }
        if (radii.Any(x => Math.Abs(x - r) < 1e-6)) { checks.Add($"round face r {Units.Mm(r)} mm"); yield break; }
        if (radii.Count == 0) { checks.Add("fillet faces are freeform (radius not checked)"); yield break; }
        yield return $"no fillet face of radius {Units.Mm(r)} mm (found {string.Join(", ", radii.Select(x => Units.Mm(x)).Distinct())} mm)";
    }

    private static void Rename(SwContext ctx, IFeature f, string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        f.Name = name;
        if (f.Name != name) ctx.Warn($"Could not rename '{f.Name}' to '{name}' (the name may be in use).");
    }
}
