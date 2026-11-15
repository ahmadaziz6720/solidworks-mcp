using System.Globalization;
using SolidWorksMcp.Core.Geometry;

namespace SolidWorksMcp.Core.Services;

/// <summary>
/// How a circular pattern turned, in words (pure; offline-tested): the sense and the instances' angles from the seed about
/// the axis as its reference reads, and, when that direction points the negative way (along -x, -y or -z, or with its
/// largest component negative), the same about the axis the other way along it, so a caller who thinks of +z reads it
/// without turning the sense round in their head.
/// </summary>
internal static class CircularPatternSense
{
    /// <param name="axis">The axis direction as the reference reads it.</param>
    /// <param name="count">Instances, the seed included.</param>
    /// <param name="stepDeg">Degrees between instances.</param>
    /// <param name="sense">+1 counter-clockwise about <paramref name="axis"/> (right hand), -1 clockwise, 0 not told apart.</param>
    public static string Describe(Vec3 axis, int count, double stepDeg, int sense)
    {
        var u = axis.Normalized();
        string head = $"{count} x {Num(stepDeg)}° about [{Dir(u)}]";
        if (sense == 0) return head;
        string text = $"{head} (the axis as read), {Turn(sense)}: the instances at {Angles(count, stepDeg, sense)} from the seed";
        if (!PointsNegative(u)) return text;
        string other = Name(-u);
        return $"{text}; the axis as read points the other way from {other}: about {other}, {Turn(-sense)}, at {Angles(count, stepDeg, -sense)}";
    }

    private static string Turn(int sense) => sense > 0 ? "counter-clockwise" : "clockwise";

    /// <summary>The largest component (the first of equals) is negative.</summary>
    private static bool PointsNegative(Vec3 u)
    {
        double[] c = [u.X, u.Y, u.Z];
        int k = 0;
        for (int i = 1; i < 3; i++)
            if (Math.Abs(c[i]) > Math.Abs(c[k]) + 1e-9) k = i;
        return c[k] < 0;
    }

    /// <summary>"+x" … "-z" for a model axis, else the vector.</summary>
    private static string Name(Vec3 u)
    {
        double[] c = [u.X, u.Y, u.Z];
        for (int i = 0; i < 3; i++)
            if (Math.Abs(c[i]) > 1 - 1e-9) return (c[i] > 0 ? "+" : "-") + "xyz"[i];
        return $"[{Dir(u)}]";
    }

    /// <summary>"+10°, +20°"; more than four instances: the first two, "…" and the last.</summary>
    private static string Angles(int count, double stepDeg, int sense)
    {
        int n = count - 1;
        string A(int k) => (sense > 0 ? "+" : "-") + Num(k * stepDeg) + "°";
        return n <= 4 ? string.Join(", ", Enumerable.Range(1, n).Select(A)) : $"{A(1)}, {A(2)}, …, {A(n)}";
    }

    private static string Num(double v) => (Math.Round(v, 6) + 0.0).ToString("0.######", CultureInfo.InvariantCulture);

    private static string Dir(Vec3 v) =>
        string.Join(", ", new[] { v.X, v.Y, v.Z }.Select(x => (Math.Round(x, 3) + 0.0).ToString("0.###", CultureInfo.InvariantCulture)));
}
