namespace SolidWorksMcp.Core.Services;

// Interop-free half of the sketch's region count (linked into the offline unit tests).

/// <summary>
/// One loop of a sketch region as SOLIDWORKS gives it: whether it is the region's outer loop, whether every edge of it
/// lies on the sketch's own contours, and whether every edge of it lies on an inner loop of the face the sketch is on.
/// </summary>
internal sealed record RegionLoop(bool Outer, bool Own, bool InnerLoopOfFace = false);

internal static class SketchRegions
{
    /// <summary>
    /// Whether a region is the sketch's own (S64 f, l). On a face SOLIDWORKS also counts the regions the face's edges
    /// close: the face around the sketch, a hole's disc. A region is the sketch's own when its one outer loop lies on the
    /// sketch's contours and each inner loop does too or is an inner loop of the face: a hole the contour encloses, so
    /// wholly inside it.
    /// </summary>
    public static bool IsOwn(IReadOnlyList<RegionLoop> loops) =>
        loops.Count(l => l.Outer) == 1 && loops.All(l => l.Own || (!l.Outer && l.InnerLoopOfFace));
}
