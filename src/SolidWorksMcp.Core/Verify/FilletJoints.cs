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
