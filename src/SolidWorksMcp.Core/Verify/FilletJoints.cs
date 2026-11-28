using SolidWorksMcp.Core.Geometry;

namespace SolidWorksMcp.Core.Verify;

// Interop-free half of a fillet's tangent-joint hint (S64 c; linked into the offline unit tests).

/// <summary>
/// An edge of the call and another edge meeting it at one of its ends: their ids and directions at the shared vertex,
/// each pointing away from it along its edge.
/// </summary>
internal sealed record EdgeJoint(string Edge, string Other, Vec3 EdgeDir, Vec3 OtherDir);

internal static class FilletJoints
{
    /// <summary>
    /// Whether two edges leaving one vertex in the directions given continue each other smoothly: opposite directions,
    /// within about 1.4 mrad.
    /// </summary>
    public static bool Tangent(Vec3 a, Vec3 b) =>
        a.Length > 0 && b.Length > 0 && a.Normalized().Dot(b.Normalized()) < -(1 - 1e-6);

    /// <summary>The joints where an edge of the call ends tangent to an edge not in it, each pair once, in the order met.</summary>
    public static IReadOnlyList<(string Edge, string Other)> Open(IEnumerable<EdgeJoint> joints, IReadOnlySet<string> inCall) =>
        joints.Where(j => j.Edge != j.Other && !inCall.Contains(j.Other) && Tangent(j.EdgeDir, j.OtherDir))
            .Select(j => (j.Edge, j.Other)).Distinct().ToList();

    /// <summary>
    /// The hint of a fillet that failed with such joints (a fillet that stops at a tangent joint fails at any radius, so
    /// the radius hint misses there), or null when there are none.
    /// </summary>
    public static string? Hint(IReadOnlyList<(string Edge, string Other)> open) =>
        open.Count == 0 ? null
            : $"{string.Join("; ", open.Select(p => $"edge {p.Edge} ends tangent to {p.Other}"))}, not in the call: " +
              $"turn `propagate` on or add {(open.Select(p => p.Other).Distinct().Count() == 1 ? "it" : "them")}.";
}

// Interop-free half of a constant fillet's overflow warning (S124).

/// <summary>
/// A face beside an edge of a constant fillet, read before the fillet: its id and a short description, the edge's id, how
/// far the face reaches from the edge (m, its boundary's farthest point) and how far the round reaches into it (m), and
/// whether it is gone after the fillet.
/// </summary>
internal sealed record BlendedFace(string Face, string Description, string Edge, double WidthM, double ReachM, bool Gone);

internal static class FilletOverflow
{
    /// <summary>
    /// How far from the edge a round of radius <paramref name="r"/> meets each face: r·tan(α/2), α the angle between the
    /// faces' outward normals (r at a right angle, more at an acute edge, less at an obtuse one). A normal not read gives r.
    /// </summary>
    public static double Reach(double r, Vec3 n1, Vec3 n2)
    {
        if (n1.Length == 0 || n2.Length == 0) return r;
        double a = Math.Acos(Math.Clamp(n1.Normalized().Dot(n2.Normalized()), -1, 1));
        return r * Math.Tan(Math.Min(a, Math.PI - 1e-6) / 2);
    }

    /// <summary>Whether the round reaches past the face (beyond a relative 1e-6 of its width), or the face is gone.</summary>
    public static bool Overflows(BlendedFace f) => f.Gone || f.ReachM > f.WidthM * (1 + 1e-6) + 1e-9;

    /// <summary>The warning of a constant fillet of radius <paramref name="r"/> (m) that overflows faces it blends, or null.</summary>
    public static string? Warning(double r, IEnumerable<BlendedFace> faces)
    {
        var over = faces.Where(Overflows).ToList();
        if (over.Count == 0) return null;
        static string Item(BlendedFace f)
        {
            var parts = new List<string>();
            if (f.ReachM > f.WidthM * (1 + 1e-6) + 1e-9)
                parts.Add($"is {Units.Mm(f.WidthM)} mm wide beside {f.Edge}, the round reaching {Units.Mm(f.ReachM)} mm into it");
            if (f.Gone) parts.Add("is gone after the fillet");
            return $"{f.Face} ({f.Description}) {string.Join(" and ", parts)}";
        }
        return $"The R{Units.Mm(r)} mm fillet reaches past faces it blends: {string.Join("; ", over.Select(Item))}. " +
               "SOLIDWORKS may have run the round over them: check the shape (view_screenshot) or use a smaller radius.";
    }
}
