using SolidWorks.Interop.sldworks;
using SolidWorksMcp.Core.Com;
using SolidWorksMcp.Core.Geometry;
using SolidWorksMcp.Core.Model;
using SolidWorksMcp.Core.Prefs;
using SolidWorksMcp.Core.Verify;

namespace SolidWorksMcp.Core.Services;

public sealed partial class ReferenceService
{
    private const double FlipVolEps = 1e-13; // m³, as FeatureVerifier

    /// <summary>
    /// ref_plane edit: flips an existing offset or angle plane (IRefPlaneFeatureData.ReversedReferenceDirection of its
    /// distance or angle reference, the PropertyManager's "Flip"). Checked: an offset plane lies as far on the other side
    /// of its base, an angle plane still holds its axis at the same angle the other way, and no feature (its children
    /// included) gained a rebuild error; otherwise it is flipped back.
    /// </summary>
    private static RefResult FlipPlane(SwContext ctx, IModelDoc2 d, RefPlaneRequest q)
    {
        string? extra = new (string Name, bool Given)[]
        {
            ("kind", !string.IsNullOrWhiteSpace(q.Kind)), ("base", q.Base is not null), ("offsetMm", q.OffsetMm is not null),
            ("angleDeg", q.AngleDeg is not null), ("axis", q.Axis is not null), ("points", q.Points is not null), ("curve", q.Curve is not null),
            ("point", q.Point is not null), ("second", q.Second is not null), ("name", q.Name is not null),
        }.Where(a => a.Given).Select(a => a.Name).FirstOrDefault();
        if (!q.Flip || extra is not null)
            throw SwException.InvalidArgument(
                extra is null ? "edit needs flip:true (it flips the plane, nothing else)." : $"With edit only flip applies; '{extra}' was given.",
                "Change an offset or angle with dimension_set, a name with feature_rename; other changes: make the plane again.");
        var f = EntityResolver.FeatureByName(d, q.Edit!)
                ?? throw new SwException(ErrorCodes.EntityNotFound, $"No feature named '{q.Edit}'.", "feature_list shows the planes (type RefPlane).");
        string name = f.Name;
        if (f.Specific() is not IRefPlane rp0) throw SwException.InvalidArgument($"'{name}' is a {f.GetTypeName2()}, not a reference plane.");
        var data = f.Definition() as IRefPlaneFeatureData;
        int count = data is null ? 0 : Math.Min(3, data.GetSelectionsCount());
        int bi = -1;
        for (int i = 0; i < count && bi < 0; i++)
            if ((data!.Constraint[i] & (Distance | Angle)) != 0) bi = i;
        if (data is null || bi < 0)
            throw SwException.InvalidArgument($"'{name}' is not an offset or angle plane, so it has no other side to flip to.",
                "Make it again with ref_plane (three_points, mid_plane …: give the points or references in the other order).");
        bool isAngle = (data.Constraint[bi] & Angle) != 0;
        var g0 = ReadPlane(ctx, rp0, $"'{name}'");

        var before = Snapshot.Take(d);
        var errorsBefore = FeatureManagementService.ErrorMap(d);
        using var prefs = ctx.SystemPrefs();
        prefs.Toggle(SystemPrefIds.ShowErrorsEveryRebuild, false);

        if (!data.AccessSelections((ModelDoc2)d, null))
            throw new SwException(ErrorCodes.FeatureFailed, $"SOLIDWORKS refused to open '{name}' for editing (AccessSelections).");
        PlaneGeom bp;
        LineGeom? axis = null;
        string baseLabel, axisLabel = "";
        bool done = false;
        try
        {
            (bp, baseLabel) = FlipBase((object?)data.Reference[bi], ctx);
            if (isAngle)
                for (int i = 0; i < count && axis is null; i++)
                {
                    if (i == bi || (object?)data.Reference[i] is not { } r) continue;
                    var res = new Resolved { Obj = r, Kind = EntityKind.Edge, Label = r is IFeature af ? $"'{af.Name}'" : r is ISketchSegment s ? $"sketch line {s.GetName()}" : "its axis edge" };
                    axis = LineOf(res, "axis");
                    axisLabel = res.Label;
                }
            if (isAngle && axis is null) throw new SwException(ErrorCodes.Internal, $"'{name}' names no axis to turn about.");
            data.ReversedReferenceDirection[bi] = !data.ReversedReferenceDirection[bi];
            done = f.ModifyDefinition(data, d, null);
        }
        finally
        {
            if (!done) data.ReleaseSelectionAccess();
        }
        if (!done) throw new SwException(ErrorCodes.FeatureFailed, $"SOLIDWORKS refused the flip of '{name}' (ModifyDefinition). Nothing was changed.");
        d.EditRebuild3();

        string FlipBack()
        {
            try
            {
                var f2 = EntityResolver.FeatureByName(d, name);
                if (f2?.Definition() is not IRefPlaneFeatureData d2 || !d2.AccessSelections((ModelDoc2)d, null))
                    return "Flipping it back failed (its definition could not be opened); check feature_info.";
                d2.ReversedReferenceDirection[bi] = !d2.ReversedReferenceDirection[bi];
                if (!f2.ModifyDefinition(d2, d, null))
                {
                    d2.ReleaseSelectionAccess();
                    return "Flipping it back failed (ModifyDefinition); check feature_info.";
                }
                d.EditRebuild3();
                var g = ReadPlane(ctx, (IRefPlane)f2.Specific()!, $"'{name}'");
                return g.Normal.IsParallelTo(g0.Normal, AngTol) && Math.Abs(g0.SignedDistance(g.Origin)) <= LinTol
                    ? "It was flipped back." : "Flipping it back did not put it where it was; check feature_info.";
            }
            catch (Exception e) when (!ComArgs.IsDisconnect(e))
            {
                return $"Flipping it back failed ({e.Message}); check feature_info.";
            }
        }

        f = EntityResolver.FeatureByName(d, name) ?? throw new SwException(ErrorCodes.VerifyFailed, $"'{name}' is gone after the flip.");
        var g1 = ReadPlane(ctx, (IRefPlane)f.Specific()!, $"'{name}'");
        var checks = new List<string>();
        string? problem;
        if (!isAngle)
        {
            double s0 = bp.SignedDistance(g0.Origin), s1 = bp.SignedDistance(g1.Origin);
            problem = !g1.Normal.IsParallelTo(bp.Normal, AngTol) ? $"it is no longer parallel to {baseLabel}"
                : Math.Abs(Math.Abs(s1) - Math.Abs(s0)) > LinTol || s0 * s1 > 0 ? $"it lies {Mm(s1)} mm along the normal of {baseLabel}, not {Mm(-s0)} mm"
                : null;
            if (problem is null) checks.Add($"flipped: {Mm(s1)} mm along the normal [{FmtDir(bp.Normal)}] of {baseLabel} (was {Mm(s0)} mm)");
        }
        else
        {
            var l = axis!.Value;
            var mirrored = g0.Normal - bp.Normal * (2 * g0.Normal.Dot(bp.Normal));
            double a0 = Math.Acos(Math.Min(1, Math.Abs(g0.Normal.Dot(bp.Normal))));
            problem = Math.Abs(g1.Normal.Dot(l.Dir)) > AngTol || Math.Abs(g1.SignedDistance(l.Point)) > LinTol ? $"it no longer contains {axisLabel}"
                : !g1.Normal.IsParallelTo(mirrored.Normalized(), AngTol) ? $"its normal [{FmtDir(g1.Normal)}] is not [{FmtDir(g0.Normal)}] turned the other way about {axisLabel}"
                : null;
            if (problem is null)
            {
                checks.Add($"flipped: contains {axisLabel}, {Units.Deg(a0)}° to {baseLabel} the other way (normal [{FmtDir(g1.Normal)}], was [{FmtDir(g0.Normal)}])");
                if (g0.Normal.IsParallelTo(g1.Normal, AngTol))
                    ctx.Warn($"At {Units.Deg(a0)}° to {baseLabel} the other way is the same plane: '{name}' did not move.");
            }
        }
        if (problem is not null)
            throw new SwException(ErrorCodes.VerifyFailed, $"After the flip of '{name}' {problem}. {FlipBack()}");

        var newErrors = FeatureManagementService.NewErrors(errorsBefore, FeatureManagementService.ErrorMap(d));
        var broken = newErrors.Where(e => e.Warning != true).ToList();
        if (broken.Count > 0)
            throw new SwException(ErrorCodes.RebuildError,
                $"Flipping '{name}' breaks features: {string.Join("; ", broken.Take(6).Select(e => $"{e.Feature}: {e.Error}"))}. {FlipBack()}",
                "Fix or delete the features built on it first, or make the plane again on the side wanted.");
        foreach (var e in newErrors) ctx.Warn($"'{e.Feature}' now has rebuild warning {e.Code} ({e.Error}).");
        var children = f.ChildFeatures().Select(c => c.Name).ToList();
        checks.Add(children.Count == 0 ? "no features built on it" : $"rebuilt with no new errors, its children too ({string.Join(", ", children.Take(8))})");

        var after = Snapshot.Take(d);
        double dv = after.Volume - before.Volume;
        int err = f.GetErrorCode2(out bool warn);
        return new RefResult
        {
            Doc = ctx.Title(d),
            Feature = new FeatureRef { Name = name, Type = f.GetTypeName2(), Error = err == 0 ? null : err, Warning = warn ? true : null },
            Frame = PlaneFrame(g1),
            Verified = new Verified
            {
                VolumeDeltaMm3 = Units.Mm3(dv),
                AreaDeltaMm2 = Units.Mm2(after.Area - before.Area),
                BodyCountDelta = after.Bodies - before.Bodies,
                FaceCountDelta = after.Faces - before.Faces,
                Material = dv > FlipVolEps ? "added" : dv < -FlipVolEps ? "removed" : "none",
                Checks = checks,
            },
            Dimensions = FeatureVerifier.Dimensions(f) is { Count: > 0 } dims ? dims : null,
            Model = after.ToState(),
        };
    }

    /// <summary>The plane of a flipped plane's distance or angle reference: a planar face or a plane, with a label.</summary>
    private static (PlaneGeom Plane, string Label) FlipBase(object? r, SwContext ctx) => r switch
    {
        IFace2 face => (PlaneOf(ctx, new Resolved { Obj = face, Kind = EntityKind.Face, Label = "its base face" }, "base"), "its base face"),
        IFeature pf when pf.Specific() is IRefPlane brp => (ReadPlane(ctx, brp, $"'{pf.Name}'"), $"'{pf.Name}'"),
        IRefPlane brp2 => (ReadPlane(ctx, brp2, "its base plane"), "its base plane"),
        _ => throw new SwException(ErrorCodes.Internal, "The plane's base is neither a planar face nor a plane."),
    };
}
