namespace SolidWorksMcp.Core.Services;

// Interop-free half of the warning that sketch_delete took a segment that was not named (linked into the offline unit tests).

internal static class SketchRemoved
{
    private const int ListMax = 4; // dimensions and relations named in one warning

    /// <summary>
    /// S86 (a): the warning for a sketch_delete after which a segment that was not named is gone, or null. SOLIDWORKS removes
    /// a polygon's construction circle with its last line, and what was dimensioned or related to the circle goes with it
    /// (the other polygon was left with no size). <paramref name="unasked"/> are "Arc1 (construction circle, diameter 20 mm)";
    /// <paramref name="relations"/> are the relations that tied them ("equal: Arc1, Arc2"); <paramref name="dimensions"/>
    /// are the dimensions the sketch lost in the call ("D1@Sketch1 = 20 mm"), named or not: a dimension does not say what
    /// it measured.
    /// </summary>
    public static string? Warning(IReadOnlyList<string> unasked, IReadOnlyList<string> relations, IReadOnlyList<string> dimensions)
    {
        if (unasked.Count == 0) return null;
        string went = unasked.Count == 1 ? "a segment" : $"{unasked.Count} segments";
        string text = $"sketch_delete also removed {went} not named in the call: {string.Join(", ", unasked)}.";
        if (relations.Count > 0) text += $" Relations that tied {(unasked.Count == 1 ? "it" : "them")} went too: {List(relations)}.";
        if (dimensions.Count > 0) text += $" The sketch lost dimension(s) in the call: {List(dimensions)}.";
        return text + " What they sized or held may now be under-defined: sketch_info shows what is left.";
    }

    private static string List(IReadOnlyList<string> items) =>
        string.Join("; ", items.Take(ListMax)) + (items.Count > ListMax ? $"; and {items.Count - ListMax} more" : "");
}
