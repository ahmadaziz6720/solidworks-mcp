using SolidWorks.Interop.swconst;
using SolidWorksMcp.Core.Com;
using SolidWorksMcp.Core.Prefs;

namespace SolidWorksMcp.Core;

/// <summary>
/// Raw-API helpers for the live tests (which can't use COM types: they are embedded in this assembly only).
/// Internal; visible to SolidWorksMcp.LiveTests.
/// </summary>
internal static class TestHooks
{
    /// <summary>The allow-listed system preferences, read directly.</summary>
    public static Task<Dictionary<string, string>> PreferencesAsync(SwSession s) =>
        s.RunAsync("test_prefs", ctx =>
            SystemPrefIds.All.ToDictionary(p => $"{p.Kind}{p.Pref}", p => PreferenceScope.Read(ctx.App, p.Kind, p.Pref)));

    /// <summary>Creates a part through the raw API, bypassing the registry, so the session treats it as the user's.</summary>
    public static Task<string> NewForeignPartAsync(SwSession s) =>
        s.RunAsync("test_foreign_part", ctx =>
        {
            string tpl = ctx.App.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);
            var d = (object?)ctx.App.NewDocument(tpl, 0, 0, 0) as SolidWorks.Interop.sldworks.IModelDoc2
                    ?? throw new SwException(ErrorCodes.FeatureFailed, "NewDocument failed.");
            return ctx.Title(d);
        });

    /// <summary>
    /// Saves a copy of a part as a library feature part (.sldlfp) with <paramref name="sketch"/> selected, which makes
    /// that sketch the library feature, as a weldment profile is made by hand.
    /// </summary>
    public static Task SaveSketchAsProfileAsync(SwSession s, string doc, string sketch, string path) =>
        s.RunAsync("test_save_profile", ctx =>
        {
            var d = ctx.Doc;
            d.ClearSelection2(true);
            if (!d.Extension.SelectByID2(sketch, "SKETCH", 0, 0, 0, false, 0, null, 0))
                throw new SwException(ErrorCodes.EntityNotFound, $"Could not select '{sketch}'.");
            int e = 0, w = 0;
            bool ok = d.Extension.SaveAs3(path, (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                (int)(swSaveAsOptions_e.swSaveAsOptions_Silent | swSaveAsOptions_e.swSaveAsOptions_Copy), null, null, ref e, ref w);
            d.ClearSelection2(true);
            if (!ok || !File.Exists(path)) throw new SwException(ErrorCodes.IoError, $"SaveAs3 to '{path}' failed (error {e}, warning {w}).");
            return 0;
        }, RunOptions.Write(doc));

    /// <summary>The Weldment Profiles file location (folders separated by ';'): read, and set first when <paramref name="set"/> is given.</summary>
    public static Task<string> WeldmentProfileFoldersAsync(SwSession s, string? set = null) =>
        s.RunAsync("test_weldment_profiles", ctx =>
        {
            const int pref = (int)swUserPreferenceStringValue_e.swFileLocationsWeldmentProfiles;
            if (set is not null) ctx.App.SetUserPreferenceStringValue(pref, set);
            return ctx.App.GetUserPreferenceStringValue(pref) ?? "";
        });

    /// <summary>
    /// SOLIDWORKS' default drawing template (Tools &gt; Options &gt; Default Templates): read, and set first when
    /// <paramref name="set"/> is given. Returns the value read after.
    /// </summary>
    public static Task<string> DefaultDrawingTemplateAsync(SwSession s, string? set = null) =>
        s.RunAsync("test_default_drawing_template", ctx =>
        {
            const int pref = (int)swUserPreferenceStringValue_e.swDefaultTemplateDrawing;
            if (set is not null) ctx.App.SetUserPreferenceStringValue(pref, set);
            return ctx.App.GetUserPreferenceStringValue(pref) ?? "";
        });

    /// <summary>Saves a copy of a document to <paramref name="path"/> through the raw API (any extension, e.g. a .drwdot template).</summary>
    public static Task SaveCopyRawAsync(SwSession s, string doc, string path) =>
        s.RunAsync("test_save_copy_raw", ctx =>
        {
            int e = 0, w = 0;
            bool ok = ctx.Doc.Extension.SaveAs3(path, (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                (int)(swSaveAsOptions_e.swSaveAsOptions_Silent | swSaveAsOptions_e.swSaveAsOptions_Copy), null, null, ref e, ref w);
            if (!ok || !File.Exists(path)) throw new SwException(ErrorCodes.IoError, $"SaveAs3 to '{path}' failed (error {e}, warning {w}).");
            return 0;
        }, RunOptions.Read(doc));

    /// <summary>Closes a document by title through the raw API (test cleanup only).</summary>
    public static Task CloseRawAsync(SwSession s, string title) =>
        s.RunAsync("test_close_raw", ctx =>
        {
            ctx.App.CloseDoc(title);
            return 0;
        });

    /// <summary>
    /// Sets a length dimension (mm) and rebuilds, without dimension_set's checks: a broken model on purpose. Returns
    /// SetSystemValue3's status (swSetValueReturnStatus_e) and the value read back (mm). With <paramref name="asUser"/>
    /// the call is bound as a read, so the server keeps no record of it: a change by the user, as the next write's
    /// "modified outside this server" check sees it (S57).
    /// </summary>
    public static Task<(int Status, double Mm)> SetDimensionRawAsync(SwSession s, string doc, string name, double mm, bool asUser = false) =>
        s.RunAsync("test_set_dimension_raw", ctx =>
        {
            var dim = (object?)ctx.Doc.Parameter(name) as SolidWorks.Interop.sldworks.IDimension
                      ?? throw new SwException(ErrorCodes.EntityNotFound, $"No dimension '{name}'.");
            int status = dim.SetSystemValue3(mm / 1000, (int)swSetValueInConfiguration_e.swSetValue_InThisConfiguration, ComArgs.Empty);
            using (var prefs = ctx.SystemPrefs())
            {
                prefs.Toggle(SystemPrefIds.ShowErrorsEveryRebuild, false);
                ctx.Doc.EditRebuild3();
            }
            return (status, dim.SystemValue * 1000);
        }, asUser ? RunOptions.Read(doc) : RunOptions.Write(doc));

    /// <summary>
    /// A document toggle (swUserPreferenceToggle_e, e.g. 4 swDisplayAxes): read, and set first when <paramref name="set"/>
    /// is given. Returns the value read after.
    /// </summary>
    public static Task<bool> DocToggleAsync(SwSession s, string doc, int toggle, bool? set = null) =>
        s.RunAsync("test_doc_toggle", ctx =>
        {
            if (set is bool v) ctx.Doc.Extension.SetUserPreferenceToggle(toggle, 0, v);
            return ctx.Doc.Extension.GetUserPreferenceToggle(toggle, 0);
        }, RunOptions.Read(doc));

    /// <summary>
    /// The box (mm, assembly space) of the tessellation of a top-level component's faces, solid and surface bodies:
    /// an oracle for its row's box that does not use IBody2.GetExtremePoint (within the chord tolerance).
    /// </summary>
    public static Task<double[]> TessBoxAsync(SwSession s, string doc, string component) =>
        s.RunAsync("test_tess_box", ctx =>
        {
            var comps = (object?)((SolidWorks.Interop.sldworks.IAssemblyDoc)ctx.Doc).GetComponents(true) as object[] ?? [];
            var c = comps.OfType<SolidWorks.Interop.sldworks.IComponent2>().Single(k => k.Name2 == component);
            var xf = new Geometry.Xform(ComArgs.Doubles((object?)c.Transform2.ArrayData));
            double[] r = [double.MaxValue, double.MaxValue, double.MaxValue, double.MinValue, double.MinValue, double.MinValue];
            foreach (var b in c.Bodies(swBodyType_e.swSolidBody).Concat(c.Bodies(swBodyType_e.swSheetBody)))
                foreach (var f in b.Faces())
                {
                    var t = ComArgs.Doubles((object?)f.GetTessTriangles(true));
                    for (int i = 0; i + 2 < t.Length; i += 3)
                    {
                        var p = xf.ApplyPoint(new Geometry.Vec3(t[i], t[i + 1], t[i + 2]));
                        r[0] = Math.Min(r[0], p.X); r[1] = Math.Min(r[1], p.Y); r[2] = Math.Min(r[2], p.Z);
                        r[3] = Math.Max(r[3], p.X); r[4] = Math.Max(r[4], p.Y); r[5] = Math.Max(r[5], p.Z);
                    }
                }
            return r.Select(v => v * 1000).ToArray();
        }, RunOptions.Read(doc));

    /// <summary>The titles of all open documents.</summary>
    public static Task<List<string>> OpenTitlesAsync(SwSession s) =>
        s.RunAsync("test_titles", ctx => ctx.App.Documents().Select(ctx.Title).ToList());

    /// <summary>Opens a part or an assembly through the raw API, bypassing the registry, as the user would from File &gt; Open.</summary>
    public static Task<string> OpenRawAsync(SwSession s, string path) =>
        s.RunAsync("test_open_raw", ctx =>
        {
            int e = 0, w = 0;
            var type = path.EndsWith(".sldasm", StringComparison.OrdinalIgnoreCase) ? swDocumentTypes_e.swDocASSEMBLY : swDocumentTypes_e.swDocPART;
            var d = (object?)ctx.App.OpenDoc6(path, (int)type, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref e, ref w)
                        as SolidWorks.Interop.sldworks.IModelDoc2
                    ?? throw new SwException(ErrorCodes.IoError, $"OpenDoc6 failed ({e}).");
            return ctx.Title(d);
        });

    /// <summary>A document's update stamp now, and the one the server's registry holds as seen (S81 probe).</summary>
    public static Task<(int Now, int? Seen)> UpdateStampAsync(SwSession s, string doc) =>
        s.RunAsync("test_update_stamp", ctx => (ctx.Doc.GetUpdateStamp(), ctx.Documents.Find(ctx.Doc)?.StampSeen), RunOptions.Read(doc));

    /// <summary>
    /// SOLIDWORKS' own <c>IComponent2.Solving</c> of each top-level component, by component name
    /// (swComponentSolvingOption_e: 0 rigid, 1 flexible).
    /// </summary>
    public static Task<Dictionary<string, int>> ComponentSolvingAsync(SwSession s, string doc) =>
        s.RunAsync("test_solving", ctx =>
        {
            var solving = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var comps = (object?)((SolidWorks.Interop.sldworks.IAssemblyDoc)ctx.Doc).GetComponents(true) as object[] ?? [];
            foreach (var c in comps.OfType<SolidWorks.Interop.sldworks.IComponent2>())
                solving[c.Name2] = c.Solving;
            return solving;
        }, RunOptions.Read(doc));

    /// <summary>
    /// The FeatureManager tree's text of each top-level component (e.g. "(-) part&lt;1&gt; (Default)"), by component name:
    /// SOLIDWORKS' own constrained mark, "(-)" under-defined, "(f)" fixed, none fully defined.
    /// </summary>
    public static Task<Dictionary<string, string>> ComponentTreeTextAsync(SwSession s, string doc) =>
        s.RunAsync("test_tree_text", ctx =>
        {
            var texts = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var root = (object?)ctx.Doc.FeatureManager.GetFeatureTreeRootItem2((int)swFeatMgrPane_e.swFeatMgrPaneBottom)
                as SolidWorks.Interop.sldworks.ITreeControlItem;
            for (var it = root?.GetFirstChild(); it is not null; it = it.GetNext())
                if (it.ObjectType == (int)swTreeControlItemType_e.swFeatureManagerItem_Component &&
                    (object?)it.Object is SolidWorks.Interop.sldworks.IComponent2 c)
                    texts[c.Name2] = it.Text;
            return texts;
        }, RunOptions.Read(doc));
}
