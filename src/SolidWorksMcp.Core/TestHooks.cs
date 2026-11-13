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

    /// <summary>The titles of all open documents.</summary>
    public static Task<List<string>> OpenTitlesAsync(SwSession s) =>
        s.RunAsync("test_titles", ctx => ctx.App.Documents().Select(ctx.Title).ToList());

    /// <summary>Opens a file through the raw API, bypassing the registry, as the user would from File &gt; Open.</summary>
    public static Task<string> OpenRawAsync(SwSession s, string path) =>
        s.RunAsync("test_open_raw", ctx =>
        {
            int e = 0, w = 0;
            var d = (object?)ctx.App.OpenDoc6(path, (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref e, ref w)
                        as SolidWorks.Interop.sldworks.IModelDoc2
                    ?? throw new SwException(ErrorCodes.IoError, $"OpenDoc6 failed ({e}).");
            return ctx.Title(d);
        });

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
