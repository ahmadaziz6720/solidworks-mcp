using SolidWorks.Interop.sldworks;
using SolidWorksMcp.Core.Com;

namespace SolidWorksMcp.Core.Geometry;

/// <summary>
/// The box of bodies from their extreme points (<c>IBody2.GetExtremePoint</c> along ±x, ±y, ±z of the target frame), not
/// <c>GetBodyBox</c>, which is a loose bound over B-spline and torus faces (S52: 0.09 mm outside a filleted plate), nor a
/// component's part box turned and re-boxed (2 mm off for a tilted shaft). A body whose extremes can't be read adds its
/// own box (turned into the frame) and the result is marked approximate.
/// </summary>
internal static class TightBox
{
    /// <summary>[xmin, ymin, zmin, xmax, ymax, zmax] in m, in the frame <paramref name="toWorld"/> maps the bodies into.</summary>
    public static (double[]? Box, bool Approximate) Of(IEnumerable<IBody2> bodies, Xform? toWorld = null)
    {
        double[]? box = null;
        bool approx = false;
        var toLocal = toWorld?.Inverse();
        foreach (var b in bodies)
        {
            var bb = Extremes(b, toWorld, toLocal);
            if (bb is null)
            {
                approx = true;
                bb = Loose(b, toWorld);
                if (bb is null) continue;
            }
            box = box is null ? bb : [Math.Min(box[0], bb[0]), Math.Min(box[1], bb[1]), Math.Min(box[2], bb[2]),
                Math.Max(box[3], bb[3]), Math.Max(box[4], bb[4]), Math.Max(box[5], bb[5])];
        }
        return (box, approx);
    }

    private static readonly Vec3[] Axes = [Vec3.UnitX, Vec3.UnitY, Vec3.UnitZ];

    private static double[]? Extremes(IBody2 b, Xform? toWorld, Xform? toLocal)
    {
        var res = new double[6];
        for (int i = 0; i < 3; i++)
            for (int sign = -1; sign <= 1; sign += 2)
            {
                var dw = Axes[i] * sign;
                var dl = toLocal is null ? dw : toLocal.ApplyVector(dw).Normalized();
                bool ok;
                double x, y, z;
                try { ok = b.GetExtremePoint(dl.X, dl.Y, dl.Z, out x, out y, out z); }
                catch (Exception ex) when (!ComArgs.IsDisconnect(ex)) { return null; }
                if (!ok) return null;
                var p = new Vec3(x, y, z);
                if (toWorld is not null) p = toWorld.ApplyPoint(p);
                res[sign < 0 ? i : i + 3] = p.Dot(Axes[i]);
            }
        return res;
    }

    private static double[]? Loose(IBody2 b, Xform? toWorld)
    {
        var bb = b.Box();
        if (bb.Length != 6) return null;
        if (toWorld is null) return bb;
        double[] r = [double.MaxValue, double.MaxValue, double.MaxValue, double.MinValue, double.MinValue, double.MinValue];
        foreach (var x in new[] { bb[0], bb[3] })
            foreach (var y in new[] { bb[1], bb[4] })
                foreach (var z in new[] { bb[2], bb[5] })
                {
                    var p = toWorld.ApplyPoint(new Vec3(x, y, z));
                    r[0] = Math.Min(r[0], p.X); r[1] = Math.Min(r[1], p.Y); r[2] = Math.Min(r[2], p.Z);
                    r[3] = Math.Max(r[3], p.X); r[4] = Math.Max(r[4], p.Y); r[5] = Math.Max(r[5], p.Z);
                }
        return r;
    }
}
