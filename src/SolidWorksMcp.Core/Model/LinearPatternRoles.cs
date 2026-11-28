namespace SolidWorksMcp.Core.Model;

/// <summary>
/// S146: which direction each dimension of a linear pattern belongs to. SOLIDWORKS names the counts D1 (direction 1) and
/// D2 (direction 2) and the spacings D3 and D4 in the same order, and says nothing else of them. The name order is checked
/// against the pattern's definition; a dimension whose value the definition does not bear out gets no role rather than a
/// wrong one.
/// </summary>
public static class LinearPatternRoles
{
    private const double Eps = 1e-6;

    /// <summary>
    /// <paramref name="dims"/> with <see cref="DimensionInfo.Role"/> set (count1, spacing1, count2, spacing2).
    /// <paramref name="count2"/> and <paramref name="spacing2Mm"/> are null for a pattern of one direction.
    /// </summary>
    public static List<DimensionInfo> Apply(IReadOnlyList<DimensionInfo> dims, int count1, double spacing1Mm, int? count2, double? spacing2Mm)
    {
        var roles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Assign(dims, "count", [("count1", count1), ("count2", count2)], roles);
        Assign(dims, "mm", [("spacing1", spacing1Mm), ("spacing2", spacing2Mm)], roles);
        return dims.Select(x => roles.TryGetValue(x.Name, out var role) ? With(x, role) : x).ToList();
    }

    private static void Assign(IReadOnlyList<DimensionInfo> dims, string unit, (string Role, double? Value)[] wanted,
        Dictionary<string, string> roles)
    {
        var rows = dims.Where(x => x.Unit == unit).OrderBy(x => Number(x.Name)).ThenBy(x => x.Name, StringComparer.Ordinal).ToList();
        int n = wanted.Count(w => w.Value is not null);
        if (rows.Count != n) return;
        // The name order first; the other order only when the definition bears that one out and not the first.
        foreach (var order in new[] { rows, [.. Enumerable.Reverse(rows)] })
        {
            if (!order.Select((r, i) => Math.Abs(r.Value - wanted[i].Value!.Value) < Eps).All(ok => ok)) continue;
            for (int i = 0; i < n; i++) roles[order[i].Name] = wanted[i].Role;
            return;
        }
    }

    /// <summary>The 1 of "D1@LPattern1".</summary>
    private static int Number(string name)
    {
        string head = name.Split('@')[0];
        return int.TryParse(head.TrimStart('D', 'd'), out int n) ? n : int.MaxValue;
    }

    private static DimensionInfo With(DimensionInfo x, string role) => new()
    {
        Name = x.Name, Value = x.Value, Unit = x.Unit, Driven = x.Driven, Equation = x.Equation, Tolerance = x.Tolerance, Role = role,
    };
}
