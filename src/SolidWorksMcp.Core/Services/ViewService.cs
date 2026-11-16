using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorksMcp.Core.Com;
using SolidWorksMcp.Core.Documents;
using SolidWorksMcp.Core.Geometry;
using SolidWorksMcp.Core.Imaging;
using SolidWorksMcp.Core.Model;

namespace SolidWorksMcp.Core.Services;

public sealed class ScreenshotResult : ToolResult
{
    public required string View { get; init; }
    public required string DisplayMode { get; init; }
    public int WidthPx { get; init; }
    public int HeightPx { get; init; }
    public string? SavedTo { get; init; }
    public string? BackupPath { get; init; }
    /// <summary>The section plane the image was cut with (removed again afterwards).</summary>
    public string? Section { get; init; }
    [System.Text.Json.Serialization.JsonIgnore] public byte[] Png { get; init; } = [];
}

/// <summary>
/// Screenshots that leave no trace: the view orientation, zoom, display mode, plane/axis/sketch visibility, selection and
/// active document are restored after the capture.
/// </summary>
public sealed class ViewService(SwSession session)
{
    private static readonly Dictionary<string, int> Views = new(StringComparer.OrdinalIgnoreCase)
    {
        ["front"] = (int)swStandardViews_e.swFrontView,
        ["back"] = (int)swStandardViews_e.swBackView,
        ["left"] = (int)swStandardViews_e.swLeftView,
        ["right"] = (int)swStandardViews_e.swRightView,
        ["top"] = (int)swStandardViews_e.swTopView,
        ["bottom"] = (int)swStandardViews_e.swBottomView,
        ["iso"] = (int)swStandardViews_e.swIsometricView,
        ["isometric"] = (int)swStandardViews_e.swIsometricView,
        ["trimetric"] = (int)swStandardViews_e.swTrimetricView,
        ["dimetric"] = (int)swStandardViews_e.swDimetricView,
    };

    private static readonly Dictionary<string, int> Modes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["shaded_edges"] = (int)swViewDisplayMode_e.swViewDisplayMode_ShadedWithEdges,
        ["shaded"] = (int)swViewDisplayMode_e.swViewDisplayMode_Shaded,
        ["wireframe"] = (int)swViewDisplayMode_e.swViewDisplayMode_Wireframe,
        ["hidden_removed"] = (int)swViewDisplayMode_e.swViewDisplayMode_HiddenLinesRemoved,
        ["hidden_grayed"] = (int)swViewDisplayMode_e.swViewDisplayMode_HiddenLinesGrayed,
    };

    public Task<ScreenshotResult> ScreenshotAsync(string? doc, string view, bool zoomToFit, string displayMode, bool hidePlanes,
        bool hideSketches, int maxEdgePx, string? file, bool overwrite, bool hideAxes = true, CancellationToken ct = default) =>
        ScreenshotAsync(doc, view, zoomToFit, displayMode, hidePlanes, hideSketches, maxEdgePx, file, overwrite, null, 0, false, hideAxes, ct);

    /// <summary>
    /// With <paramref name="section"/>: a 3D section view (IModelViewManager.CreateSectionViewData, FirstPlane
    /// = the plane or planar face, FirstOffset, capped) for the image, removed again afterwards (RemoveSectionView).
    /// <paramref name="hideAxes"/> hides the reference axes (swDisplayAxes) as <paramref name="hidePlanes"/> the planes (S65 (d));
    /// zoom to fit takes in neither. In a part the fit is to what the image shows (<see cref="FitToWhatIsShown"/>).
    /// </summary>
    public Task<ScreenshotResult> ScreenshotAsync(string? doc, string view, bool zoomToFit, string displayMode, bool hidePlanes,
        bool hideSketches, int maxEdgePx, string? file, bool overwrite, EntityRef? section, double sectionOffsetMm, bool sectionReverse,
        bool hideAxes = true, CancellationToken ct = default) =>
        session.RunAsync("view_screenshot", ctx =>
        {
            string v = (view ?? "iso").Trim().ToLowerInvariant();
            if (v != "current" && !Views.ContainsKey(v))
                throw SwException.InvalidArgument($"Unknown view '{view}'.", "Use current, iso, front, back, left, right, top, bottom, trimetric or dimetric.");
            string mode = (displayMode ?? "shaded_edges").Trim().ToLowerInvariant();
            if (mode != "current" && !Modes.ContainsKey(mode))
                throw SwException.InvalidArgument($"Unknown displayMode '{displayMode}'.", "Use current, shaded_edges, shaded, wireframe, hidden_removed or hidden_grayed.");
            int edge = Math.Clamp(maxEdgePx, 64, 2048);
            string? saveTo = null, backup = null;
            if (!string.IsNullOrWhiteSpace(file))
            {
                string f = file!.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? file : file + ".png";
                saveTo = ctx.Paths.ForWrite(f, overwrite, out backup, "file");
            }

            var d = ctx.Doc;
            var previous = ctx.ActiveDocOrNull;
            bool switched = false;
            if (previous is null || !DocLocator.SameDoc(previous, d))
            {
                ctx.ActivateDoc(d, ctx.Title(d));
                switched = true;
            }
            var mv = d.View() ?? throw new SwException(ErrorCodes.NotSupported, $"'{ctx.Title(d)}' has no graphics view.");
            var sel = d.Sel();
            Resolved? cut = null;
            if (section is not null)
            {
                if (d.GetType() == (int)swDocumentTypes_e.swDocDRAWING) throw SwException.InvalidArgument("Section views are for parts and assemblies.");
                cut = EntityResolver.Resolve(ctx, d, section, "section", EntityKind.Plane, EntityKind.Face);
                if (cut.Obj is IFace2 cf && Topology.Describe(cf).Surface != "plane")
                    throw SwException.InvalidArgument($"{cut.Label} is not planar; a section needs a plane or a planar face.");
            }
            bool sectioned = false;

            // Save everything we may change.
            var orientation = mv.Orientation3;
            double scale = mv.Scale2;
            var translation = mv.Translation3;
            int oldMode = mv.DisplayMode;
            bool planes = d.Extension.GetUserPreferenceToggle((int)swUserPreferenceToggle_e.swDisplayPlanes, 0);
            bool sketches = d.Extension.GetUserPreferenceToggle((int)swUserPreferenceToggle_e.swDisplaySketches, 0);
            bool axes = d.Extension.GetUserPreferenceToggle((int)swUserPreferenceToggle_e.swDisplayAxes, 0);
            string tmp = Path.Combine(Path.GetTempPath(), $"swmcp_{Guid.NewGuid():N}.bmp");
            bool suspended = false;
            try
            {
                try { sel.SuspendSelectionList(); suspended = true; } catch { /* nothing selected */ }
                if (hidePlanes && planes) d.Extension.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swDisplayPlanes, 0, false);
                if (hideSketches && sketches) d.Extension.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swDisplaySketches, 0, false);
                if (hideAxes && axes) d.Extension.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swDisplayAxes, 0, false);
                if (mode != "current") mv.DisplayMode = Modes[mode];
                if (v != "current") d.ShowNamedView2("", Views[v]);
                List<string> far = [];
                if (zoomToFit && !FitToWhatIsShown(d, sketches && !hideSketches, planes && !hidePlanes, far)) d.ViewZoomtofit2();
                if (far.Count > 0)
                    ctx.Warn($"The fit takes in {string.Join(", ", far)}, shown past the bodies, which leaves them small in the image; " +
                             "hideSketches:true leaves sketches out of the image and the fit.");
                if (cut is not null)
                {
                    var vm = d.ModelViewManager;
                    var data = (object?)vm.CreateSectionViewData() as SectionViewData
                               ?? throw new SwException(ErrorCodes.NotSupported, "SOLIDWORKS returned no section view data.");
                    data.FirstPlane = cut.Obj;
                    data.FirstOffset = Units.M(sectionOffsetMm);
                    data.FirstReverseDirection = sectionReverse;
                    data.ShowSectionCap = true;
                    data.GraphicsOnlySection = false; // a graphics-only section stays: RemoveSectionView fails on it (VERIFIED)
                    if (!vm.CreateSectionView(data))
                        throw new SwException(ErrorCodes.FeatureFailed, $"SOLIDWORKS made no section view on {cut.Label}.");
                    sectioned = true;
                }
                d.GraphicsRedraw2();
                if (!d.SaveBMP(tmp, 0, 0) || !File.Exists(tmp))
                    throw new SwException(ErrorCodes.IoError, "SOLIDWORKS could not render the view (SaveBMP failed).",
                        "The window may be minimized or the session locked; ask the user to restore the SOLIDWORKS window.");
                var png = Png.FromBmp(File.ReadAllBytes(tmp), edge, out int w, out int h);
                if (saveTo is not null)
                {
                    File.WriteAllBytes(saveTo, png);
                    ctx.Paths.RecordWritten(saveTo);
                }
                return new ScreenshotResult
                {
                    Doc = ctx.Title(d), View = v, DisplayMode = mode, WidthPx = w, HeightPx = h, Png = png,
                    SavedTo = saveTo, BackupPath = backup,
                    Section = cut is null ? null : $"{cut.Label}{(sectionOffsetMm != 0 ? $" offset {sectionOffsetMm} mm" : "")}{(sectionReverse ? ", reversed" : "")}",
                };
            }
            finally
            {
                try { File.Delete(tmp); } catch { /* temp */ }
                try
                {
                    if (sectioned && !d.ModelViewManager.RemoveSectionView()) ctx.Warn("The section view could not be removed again.");
                    if (hidePlanes && planes) d.Extension.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swDisplayPlanes, 0, true);
                    if (hideSketches && sketches) d.Extension.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swDisplaySketches, 0, true);
                    if (hideAxes && axes) d.Extension.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swDisplayAxes, 0, true);
                    if (mode != "current") mv.DisplayMode = oldMode;
                    if (v != "current" || zoomToFit)
                    {
                        if (orientation is not null) mv.Orientation3 = orientation;
                        if (translation is not null) mv.Translation3 = translation;
                        mv.Scale2 = scale;
                    }
                    if (suspended) sel.ResumeSelectionList();
                    d.GraphicsRedraw2();
                    if (switched && previous is not null)
                    {
                        int err = 0;
                        ctx.App.ActivateDoc3(ctx.Title(previous), false, (int)swRebuildOnActivation_e.swDontRebuildActiveDoc, ref err);
                    }
                }
                catch (Exception ex) { ctx.Warn($"Could not fully restore the view: {ex.Message}"); }
            }
        }, RunOptions.Read(doc), ct);

    /// <summary>
    /// Zoom to fit what the image shows (S65 (d)). <c>ViewZoomtofit2</c> fits the part's box (<c>GetPartBox</c>), which
    /// takes in every sketch, shown or hidden (VERIFIED 2023 SP3: a sketch circle 300 mm out, or the 3D sketch holding an
    /// axis's points, kept the block at under 2 % of the image with the sketches toggle off, with the axes toggle off and with
    /// the axis blanked), and reference points; not reference planes or axes (an axis of two planes 300 mm out, shown, left
    /// the fit as without it). So when the part's box reaches past its visible bodies, the view is fitted to a selection of
    /// what is shown instead (<c>ViewZoomToSelection</c>, the selection cleared after): per body the faces that reach its
    /// box's six extremes (a body selected zooms to more than itself, probed), the sketches shown when
    /// <paramref name="sketchesShown"/>, the planes shown when <paramref name="planesShown"/>, and the reference points and
    /// coordinate systems shown. False, leaving the fit to <c>ViewZoomtofit2</c>, for an assembly or drawing, a part with no
    /// visible body, or one whose box is its bodies' box. <paramref name="far"/> gets the shown sketches in the fit that
    /// reach past the bodies' box by more than <see cref="FitReach.Share"/> of its largest side (S88).
    /// </summary>
    private static bool FitToWhatIsShown(IModelDoc2 d, bool sketchesShown, bool planesShown, List<string> far)
    {
        if (d is not IPartDoc part) return false;
        var bodies = ((object?)part.GetBodies2((int)swBodyType_e.swAllBodies, true) as object[] ?? []).OfType<IBody2>().ToList();
        if (bodies.Count == 0) return false;
        const double Tol = 1e-6; // m
        var whole = ComArgs.Doubles((object?)part.GetPartBox(true));
        var boxes = bodies.Select(b => ComArgs.Doubles((object?)b.GetBodyBox())).Where(x => x.Length == 6).ToList();
        if (whole.Length != 6 || boxes.Count == 0) return false;
        bool reachesPast = Enumerable.Range(0, 3).Any(i => whole[i] < boxes.Min(x => x[i]) - Tol || whole[i + 3] > boxes.Max(x => x[i + 3]) + Tol);
        if (!reachesPast) return false;

        var shown = new List<IFeature>();
        void Look(IFeature f)
        {
            if (f.Visible != (int)swVisibilityState_e.swVisibilityStateShown) return;
            string t = f.GetTypeName2();
            if (t is "RefPoint" or "CoordSys" || sketchesShown && t is "ProfileFeature" or "3DProfileFeature" || planesShown && t == "RefPlane")
                shown.Add(f);
        }
        for (var f = (object?)d.FirstFeature() as IFeature; f is not null; f = (object?)f.GetNextFeature() as IFeature)
        {
            Look(f);
            for (var s = (object?)f.GetFirstSubFeature() as IFeature; s is not null; s = (object?)s.GetNextSubFeature() as IFeature)
                Look(s);
        }
        var faces = new List<IFace2>();
        foreach (var b in bodies)
        {
            var fs = ((object?)b.GetFaces() as object[] ?? []).OfType<IFace2>().Select(f => (Face: f, Box: f.Box())).Where(x => x.Box.Length == 6).ToList();
            if (fs.Count == 0) continue;
            var picked = new HashSet<int>();
            for (int i = 0; i < 6; i++)
            {
                int k = i;
                var ext = Enumerable.Range(0, fs.Count);
                picked.Add(k < 3 ? ext.MinBy(j => fs[j].Box[k]) : ext.MaxBy(j => fs[j].Box[k]));
            }
            faces.AddRange(picked.Select(j => fs[j].Face));
        }
        if (faces.Count == 0) return false;
        double[] all = [boxes.Min(x => x[0]), boxes.Min(x => x[1]), boxes.Min(x => x[2]), boxes.Max(x => x[3]), boxes.Max(x => x[4]), boxes.Max(x => x[5])];
        foreach (var f in shown.Where(f => f.GetTypeName2() is "ProfileFeature" or "3DProfileFeature"))
        {
            try
            {
                var box = SketchModelBox((ISketch)(object)f.GetSpecificFeature2());
                if (box.Length == 6 && FitReach.Warns(all, box)) far.Add(f.Name);
            }
            catch { /* a sketch that cannot be read is fitted all the same, unwarned */ }
        }
        d.ClearSelection2(true);
        try
        {
            foreach (var f in faces) ((IEntity)f).Select4(true, null);
            foreach (var f in shown) f.Select2(true, 0);
            d.ViewZoomToSelection();
        }
        finally { d.ClearSelection2(true); }
        return true;
    }

    /// <summary>
    /// A sketch's box in model space (m), as the fit takes it in: its points but the centres of arcs and ellipses (a shallow
    /// arc's centre 180 mm out left the fit as without it, VERIFIED 2023 SP3), the arcs' ends and, in a 2D sketch, the points
    /// of each arc's circle at its axis extremes that lie on the arc. A 3D sketch's arcs and an ellipse's or spline's bulge
    /// give their points only, so the box may fall short of them, never past.
    /// </summary>
    private static double[] SketchModelBox(ISketch sk)
    {
        bool is3D = sk.Is3D();
        Xform? s2m = is3D ? null : SketchService.SketchToModel(sk);
        static Vec3 P(ISketchPoint p) => new(p.X, p.Y, p.Z);
        var segs = ComArgs.Objects<ISketchSegment>((object?)sk.GetSketchSegments()).ToList();
        var centres = segs.Select(s => s switch { ISketchArc a => a.SkCenter(), ISketchEllipse e => e.SkCenter(), _ => null })
            .OfType<ISketchPoint>().Select(P).ToList();
        var pts = ComArgs.Objects<ISketchPoint>((object?)sk.GetSketchPoints2()).Select(P)
            .Where(p => !centres.Any(c => (p - c).Length < 1e-9)).ToList();
        foreach (var arc in segs.OfType<ISketchArc>())
        {
            if (arc.SkCenter() is not { } c || arc.SkStart() is not { } a || arc.SkEnd() is not { } b) continue;
            pts.Add(P(a));
            pts.Add(P(b));
            if (is3D) continue;
            double r = arc.GetRadius();
            bool circle = arc.IsCircle() != 0;
            bool ccw = circle || AdvancedFeatureLogic.ArcCcw(new Vec3(c.X, c.Y, 0), r, new Vec3(a.X, a.Y, 0), new Vec3(b.X, b.Y, 0),
                ((ISketchSegment)arc).GetLength(), arc.GetRotationDir());
            pts.AddRange(FitReach.ArcExtremes(c.X, c.Y, r, a.X, a.Y, b.X, b.Y, ccw, circle).Select(q => new Vec3(q.X, q.Y, 0)));
        }
        foreach (var line in segs.OfType<ISketchLine>())
            if (line.SkStart() is { } a && line.SkEnd() is { } b) { pts.Add(P(a)); pts.Add(P(b)); }
        return FitReach.Box(pts.Select(p => s2m is null ? p : s2m.ApplyPoint(p)).Select(p => (p.X, p.Y, p.Z)));
    }
}
