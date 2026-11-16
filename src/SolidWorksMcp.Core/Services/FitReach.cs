namespace SolidWorksMcp.Core.Services;

/// <summary>
/// Whether a shown sketch reaches far enough past a part's bodies to leave them small in a fitted image (S88; pure,
/// offline-tested). Boxes are [xmin, ymin, zmin, xmax, ymax, zmax].
/// </summary>
internal static class FitReach
{
    /// <summary>A sketch past the bodies by more than this share of their largest side is worth a warning.</summary>
    public const double Share = 0.1;

    /// <summary>How far <paramref name="box"/> reaches past <paramref name="bodies"/> on any side; 0 when inside.</summary>
    public static double Past(double[] bodies, double[] box)
    {
        double past = 0;
        for (int i = 0; i < 3; i++)
            past = Math.Max(past, Math.Max(bodies[i] - box[i], box[i + 3] - bodies[i + 3]));
        return past;
    }

    /// <summary>True when <paramref name="box"/> reaches past <paramref name="bodies"/> by more than <see cref="Share"/> of their largest side.</summary>
    public static bool Warns(double[] bodies, double[] box)
    {
        double side = Enumerable.Range(0, 3).Max(i => bodies[i + 3] - bodies[i]);
        return Past(bodies, box) > Share * Math.Max(side, 1e-9);
    }

    /// <summary>The box of <paramref name="points"/> (x, y, z); empty for none.</summary>
    public static double[] Box(IEnumerable<(double X, double Y, double Z)> points)
    {
        var p = points.ToList();
        return p.Count == 0 ? [] : [p.Min(q => q.X), p.Min(q => q.Y), p.Min(q => q.Z), p.Max(q => q.X), p.Max(q => q.Y), p.Max(q => q.Z)];
    }

    /// <summary>
    /// The points of a 2D arc's circle at its axis extremes (0°, 90°, 180°, 270° about the centre) that lie on its sweep
    /// from (<paramref name="sx"/>, <paramref name="sy"/>) to (<paramref name="ex"/>, <paramref name="ey"/>); all four for
    /// a full circle. With the ends, they make the arc's box.
    /// </summary>
    public static IEnumerable<(double X, double Y)> ArcExtremes(double cx, double cy, double r, double sx, double sy, double ex, double ey,
        bool ccw, bool circle)
    {
        double a0 = Math.Atan2(sy - cy, sx - cx), a1 = Math.Atan2(ey - cy, ex - cx);
        double sweep = Turn(ccw ? a1 - a0 : a0 - a1);
        for (int k = 0; k < 4; k++)
        {
            double t = k * Math.PI / 2;
            if (circle || Turn(ccw ? t - a0 : a0 - t) <= sweep + 1e-12)
                yield return (cx + r * Math.Cos(t), cy + r * Math.Sin(t));
        }
    }

    private static double Turn(double a)
    {
        a %= 2 * Math.PI;
        return a < 0 ? a + 2 * Math.PI : a;
    }
}
