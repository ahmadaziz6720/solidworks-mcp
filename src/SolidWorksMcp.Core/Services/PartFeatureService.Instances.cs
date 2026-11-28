using SolidWorks.Interop.sldworks;
using SolidWorksMcp.Core.Com;
using SolidWorksMcp.Core.Geometry;
using SolidWorksMcp.Core.Verify;

namespace SolidWorksMcp.Core.Services;

/// <summary>
/// What a linear pattern's instances left in the model, read from an existing pattern (S156): the grid, the instances
/// that left none of the seed's faces where the pattern puts them, and the directions none of whose instances did.
/// </summary>
/// <param name="Empty">Instances that formed nothing, 1-based (place along direction 1, place along direction 2).</param>
/// <param name="Lost1">Direction 1 has more than one place and no instance beyond the seed's row formed anything.</param>
public sealed record LinearInstances(string Pattern, int Count1, int Count2, int Checked, IReadOnlyList<(int I, int J)> Empty,
    bool Lost1, bool Lost2);

public sealed partial class PartFeatureService
{
    /// <summary>
    /// The instances (0-based grid places, the seed's (0, 0) left out) whose seed faces are all missing from
    /// <paramref name="result"/> where hypothesis <paramref name="h"/> puts them: the instances that formed nothing.
    /// Shared by feature_pattern_linear and dimension_set's check of an existing pattern (S156).
    /// </summary>
    private static List<(int I, int J)> EmptyInstances(List<FaceSig> seed, List<FaceSig> result, Hypothesis h, Vec3 u1, double s1, int n1,
        Vec3? u2, double s2, int n2) =>
        GridSample(n1, n2)
            .Where(ij => Score(seed, result, [Rigid.Translate(u1 * (ij.I * s1 * h.S1) + (u2 ?? Vec3.Zero) * (ij.J * s2 * h.S2))]).Matched == 0)
            .ToList();

    /// <summary>
    /// S156: checks an existing linear pattern's instances as feature_pattern_linear checks a new one, both senses of each
    /// direction tried (the pattern's own sense is SOLIDWORKS'). Null when the pattern can't be read or checked (no
    /// definition, no direction, a seed without planar, cylindrical or spherical faces, too many faces): a check, not
    /// the caller's work. Reading the definition's seeds and directions may need AccessSelections, released at once.
    /// </summary>
    internal static LinearInstances? CheckLinearInstances(IModelDoc2 d, IFeature pattern)
    {
        try
        {
            if ((object?)pattern.GetDefinition() is not ILinearPatternFeatureData data) return null;
            int n1 = data.D1TotalInstances;
            bool two = data.IsDirection2Specified();
            int n2 = two ? data.D2TotalInstances : 1;
            double s1 = data.D1Spacing, s2 = two ? data.D2Spacing : 0;
            if (n1 < 1 || n2 < 1 || n1 * n2 < 2) return null;

            var (seeds, u1, u2) = ReadPattern(data, two);
            if ((seeds.Count == 0 || u1 is null || (two && u2 is null)) && data.AccessSelections((ModelDoc2)d, null))
            {
                try { (seeds, u1, u2) = ReadPattern(data, two); }
                finally { data.ReleaseSelectionAccess(); }
            }
            if (seeds.Count == 0 || u1 is not Vec3 dir1 || (two && u2 is null)) return null;

            var seedSigs = Signatures(seeds.SelectMany(FeatureFaces), out _);
            var faces = FeatureFaces(pattern);
            if (seedSigs.Count == 0 || faces.Count > MaxSignatureFaces) return null;
            var res = Signatures(faces, out _);
            var h = BestOf(LinearHypotheses(seedSigs, res, dir1, s1, n1, u2, s2, n2));
            var empty = EmptyInstances(seedSigs, res, h, dir1, s1, n1, u2, s2, n2);
            int sampled = GridSample(n1, n2).Count();
            // A direction is lost when every sampled instance off the seed's own row along it formed nothing.
            bool Lost(bool first) => (first ? n1 : n2) > 1 &&
                GridSample(n1, n2).Where(ij => first ? ij.I > 0 : ij.J > 0).All(ij => empty.Contains(ij));
            return new LinearInstances(pattern.Name, n1, n2, sampled, empty.Select(ij => (ij.I + 1, ij.J + 1)).ToList(), Lost(true), Lost(false));
        }
        catch (Exception ex) when (!ComArgs.IsDisconnect(ex)) { return null; }
    }

    /// <summary>The pattern's seed features and its directions' unit vectors (unsigned), as far as the definition gives them.</summary>
    private static (List<IFeature> Seeds, Vec3? U1, Vec3? U2) ReadPattern(ILinearPatternFeatureData data, bool two)
    {
        var seeds = ComArgs.Objects<IFeature>(Read(() => (object?)data.PatternFeatureArray));
        return (seeds, DirectionOf(Read(() => (object?)data.D1Axis)), two ? DirectionOf(Read(() => (object?)data.D2Axis)) : null);
    }

    private static object? Read(Func<object?> read)
    {
        try { return read(); }
        catch (Exception ex) when (!ComArgs.IsDisconnect(ex)) { return null; }
    }

    /// <summary>A pattern direction reference's unit vector: a plane's normal, a straight edge's or an axis's direction.</summary>
    private static Vec3? DirectionOf(object? reference)
    {
        object? o = reference is IFeature f ? f.Specific() ?? f : reference;
        switch (o)
        {
            case IRefPlane rp:
            {
                var t = rp.RefPlaneXform();
                if (t.Length < 12) return null;
                var n = new Vec3(t[6], t[7], t[8]);
                return n.Length < 1e-12 ? null : n.Normalized();
            }
            case IRefAxis ax:
            {
                var p = ax.RefAxisEnds();
                if (p.Length < 6) return null;
                var v = Vec3.From(p, 3) - Vec3.From(p, 0);
                return v.Length < 1e-12 ? null : v.Normalized();
            }
            case IEdge e when Topology.Describe(e) is { Curve: "line", Direction: Vec3 u }:
                return u.Normalized();
            case IFace2 face when Topology.Describe(face) is { Surface: "plane", Normal: Vec3 fn }:
                return fn.Normalized();
        }
        return null;
    }
}
