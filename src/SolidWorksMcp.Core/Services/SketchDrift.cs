using System.Globalization;
using SolidWorksMcp.Core.Geometry;

namespace SolidWorksMcp.Core.Services;

// Interop-free half of sketch_add's check that what it drew stayed where it was drawn (linked into the offline unit tests).

/// <summary>A point as sketch_add was asked to draw it: the field it came from ("corners[1]", "a corner") and where (m).</summary>
internal sealed record DrawnPoint(string Label, Vec3 At);

/// <summary>A point of the drawn entity in the solved sketch: who has it ("Line3 end", "Line4 end") and where (m).</summary>
internal sealed record SolvedPoint(IReadOnlyList<string> Owners, Vec3 At);

internal static class SketchDrift
{
    private const int ListMax = 4; // moved points named per entity

    /// <summary>
    /// S80 (a): the warning for an entity whose points the solve moved off where they were drawn, or null. Each drawn
    /// point is matched to the nearest point of the entity in the solved sketch; one farther than <paramref name="tolM"/>
    /// moved. The warning names each moved point, where it is now and how far, and the relations SOLIDWORKS made between
    /// the entity and other geometry that name a moved point or its segment (all of them when none does). With none, the
    /// point was snapped as SOLIDWORKS drew it (seen live: a polygon's vertex onto the height of another entity's end, no
    /// relation added).
    /// </summary>
    /// <param name="segments">The entity's segment names (its own relations name only these: "vertical: Line2").</param>
    public static string? Moved(string entity, IReadOnlyList<DrawnPoint> drawn, IReadOnlyList<SolvedPoint> solved,
        IReadOnlyList<string>? relations, IReadOnlyCollection<string> segments, bool is3D, double tolM)
    {
        if (drawn.Count == 0 || solved.Count == 0) return null;
        var moved = new List<(DrawnPoint D, SolvedPoint S, double Gap)>();
        foreach (var d in drawn)
        {
            var near = solved.MinBy(s => s.At.DistanceTo(d.At))!;
            double gap = near.At.DistanceTo(d.At);
            if (gap > tolM) moved.Add((d, near, gap));
        }
        if (moved.Count == 0) return null;

        var names = segments.ToHashSet(StringComparer.Ordinal);
        // A free point ("point [170, 0]", a centre rectangle's centre) is no one's segment: neither its own nor another's.
        bool Own(string token) => names.Contains(token) || token.StartsWith("point [", StringComparison.Ordinal) ||
                                  (token.IndexOf(' ') is > 0 and var sp && names.Contains(token[..sp]));
        var external = (relations ?? []).Where(r => Tokens(r) is { Count: > 0 } t && !t.All(Own)).ToList();
        var touched = moved.SelectMany(m => m.S.Owners.Concat(m.S.Owners.Select(o => o.Split(' ')[0]))).ToHashSet(StringComparer.Ordinal);
        var cause = external.Where(r => Tokens(r).Any(touched.Contains)).ToList();
        if (cause.Count == 0) cause = external;

        string Pt(Vec3 v) => "[" + Fmt(is3D ? [v.X, v.Y, v.Z] : [v.X, v.Y]) + "]";
        var listed = moved.Take(ListMax).Select(m => $"{m.D.Label} {Pt(m.D.At)} to {Pt(m.S.At)} ({Mm(m.Gap)} mm)").ToList();
        if (moved.Count > ListMax) listed.Add($"{moved.Count - ListMax} more");
        string by = cause.Count == 0 ? "with no relation to other geometry: snapped to it as SOLIDWORKS drew"
            : "by its relations " + string.Join("; ", cause);
        return $"{entity} is not where it was drawn: SOLIDWORKS moved {string.Join(", ", listed)}, {by}. " +
               "Draw it clear of other geometry and model edges and place it by dimensions, or sketch_delete it.";
    }

    /// <summary>
    /// S80 (c): the warning for a rectangle drawn square that SOLIDWORKS made equal-sided, or null: its width and height
    /// can then not be driven apart.
    /// </summary>
    public static string? SquareEqual(string entity, string kind, IReadOnlyList<string>? relations)
    {
        if (kind != "rectangle") return null;
        var eq = (relations ?? []).Where(r => r.StartsWith("equal: ", StringComparison.Ordinal)).ToList();
        if (eq.Count == 0) return null;
        return $"{entity} was drawn square, and SOLIDWORKS made its sides equal ({string.Join("; ", eq)}): a width and a " +
               "height driven apart later conflict. For a rectangle that may change shape, draw it a closed polyline.";
    }

    /// <summary>What a listed relation relates: "midpoint: Line3 end, a model edge" → ["Line3 end", "a model edge"].</summary>
    internal static List<string> Tokens(string relation)
    {
        int c = relation.IndexOf(": ", StringComparison.Ordinal);
        if (c < 0) return [];
        var list = new List<string>();
        foreach (var part in relation[(c + 2)..].Split(", "))
        {
            // "point [170, 0]" holds the separator: joined again while a bracket is open.
            if (list.Count > 0 && list[^1].Count(ch => ch == '[') > list[^1].Count(ch => ch == ']')) list[^1] += ", " + part;
            else list.Add(part.Trim());
        }
        return list.Where(t => t.Length > 0).ToList();
    }

    private static string Mm(double m) => (Units.Mm(m) + 0.0).ToString("0.######", CultureInfo.InvariantCulture);

    private static string Fmt(double[] m) =>
        string.Join(", ", m.Select(v => (Math.Round(Units.Mm(v), 3) + 0.0).ToString("0.###", CultureInfo.InvariantCulture)));
}
