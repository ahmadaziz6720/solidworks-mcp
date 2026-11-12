using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidWorksMcp.Core.Services;

/// <summary>
/// Equations bind to dimensions by name ("D1@Sketch1"), not to the feature (S54): one left by a deleted feature stays, and
/// a feature that later gets that name, new or renamed, takes it over. These helpers find the equations on features that
/// go, and notice when a call gives such an equation a dimension again.
/// </summary>
internal static class EquationBindings
{
    /// <summary>The equations whose left-hand side is a dimension, or the suppression state, of one of <paramref name="features"/>.</summary>
    public static List<EquationRow> OnFeatures(IReadOnlyList<EquationRow> all, IEnumerable<string> features)
    {
        var names = new HashSet<string>(features, StringComparer.OrdinalIgnoreCase);
        return all.Where(e => EquationText.FeatureOf(e.Lhs, e.Global == true) is { } f && names.Contains(f)).ToList();
    }

    /// <summary>The other equations whose right-hand side uses a dimension of one of <paramref name="features"/>.</summary>
    public static List<EquationRow> UsersOf(IReadOnlyList<EquationRow> all, IEnumerable<string> features, IReadOnlyList<EquationRow> except)
    {
        var names = new HashSet<string>(features, StringComparer.OrdinalIgnoreCase);
        return all.Where(e => !except.Any(x => x.Index == e.Index) &&
                              EquationText.DimensionNames(e.Text).Any(n => !EquationText.SameLhs(n, e.Lhs) &&
                                                                           EquationText.FeatureOf(n) is { } f && names.Contains(f)))
            .ToList();
    }

    /// <summary>
    /// The equations (by text) that name a dimension this document does not have, with those names; null when there are
    /// none or the document has no equations. Read when a write starts, to tell afterwards which of them the call bound.
    /// </summary>
    public static Dictionary<string, List<string>>? Unbound(IModelDoc2 d)
    {
        int type = d.GetType();
        if (type != (int)swDocumentTypes_e.swDocPART && type != (int)swDocumentTypes_e.swDocASSEMBLY) return null;
        var em = d.GetEquationMgr();
        int n = em is null ? 0 : em.GetCount();
        if (n <= 0) return null;
        var known = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, List<string>>? map = null;
        for (int i = 0; i < n; i++)
        {
            string text = em!.Equation[i] ?? "";
            var missing = EquationText.DimensionNames(text).Where(x => !Resolves(d, x, known)).ToList();
            if (missing.Count > 0) (map ??= new(StringComparer.Ordinal))[text] = missing;
        }
        return map;
    }

    /// <summary>
    /// After a write: warns, naming them, about the equations of <paramref name="before"/> (from <see cref="Unbound"/>) that
    /// now find their dimensions, because a feature or sketch got the name they use (a new one, a rename, an undo).
    /// </summary>
    public static void WarnBound(SwContext ctx, IModelDoc2 d, Dictionary<string, List<string>> before)
    {
        var em = d.GetEquationMgr();
        if (em is null) return;
        int n = em.GetCount();
        var now = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < n; i++) now.Add(em.Equation[i] ?? "");
        var known = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        var bound = before.Where(kv => now.Contains(kv.Key))
            .Select(kv => (Text: kv.Key, Names: kv.Value.Where(x => Resolves(d, x, known)).ToList()))
            .Where(x => x.Names.Count > 0).ToList();
        if (bound.Count == 0) return;
        var features = bound.SelectMany(x => x.Names).Select(x => EquationText.FeatureOf(x)).OfType<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase).Select(f => $"'{f}'");
        ctx.Warn($"{bound.Count} equation(s) that named no existing dimension now bind by name to {string.Join(", ", features)}: " +
                 string.Join("; ", bound.Take(6).Select(x => x.Text)) + (bound.Count > 6 ? $"; and {bound.Count - 6} more" : "") +
                 ". Left by a deleted feature of that name? Check they are meant for this one; equation_delete removes them.");
    }

    private static bool Resolves(IModelDoc2 d, string name, Dictionary<string, bool> known)
    {
        if (!known.TryGetValue(name, out bool ok)) known[name] = ok = PropertiesService.Dim(d, name) is not null;
        return ok;
    }
}
