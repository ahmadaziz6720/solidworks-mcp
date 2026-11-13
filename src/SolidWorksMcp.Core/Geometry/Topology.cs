using SolidWorks.Interop.sldworks;
using SolidWorksMcp.Core.Com;

namespace SolidWorksMcp.Core.Geometry;

/// <summary>Geometry facts about faces and edges (in the coordinates the entity reports: model space for parts).</summary>
internal sealed record FaceGeom(string Surface, Vec3? Normal, Vec3? AxisOrigin, Vec3? Axis, double? Radius, double? HalfAngle);

internal sealed record EdgeGeom(string Curve, Vec3 Start, Vec3 End, Vec3 Mid, double Length, Vec3? Center, Vec3? Axis,
    double? Radius, Vec3? Direction, bool Closed);

/// <summary>Reads surface/curve types and parameters, and computes unambiguous pick points.</summary>
internal static class Topology
{
    public static string SurfaceKind(ISurface? s)
    {
        if (s is null) return "unknown";
        if (s.IsPlane()) return "plane";
        if (s.IsCylinder()) return "cylinder";
        if (s.IsCone()) return "cone";
        if (s.IsSphere()) return "sphere";
        if (s.IsTorus()) return "torus";
        if (s.IsParametric()) return "bspline";
        if (s.IsSwept()) return "swept";
        if (s.IsRevolved()) return "revolved";
        if (s.IsOffset()) return "offset";
        if (s.IsBlending()) return "blend";
        return "other";
    }

    public static string CurveKind(ICurve? c)
    {
        if (c is null) return "unknown";
        if (c.IsLine()) return "line";
        if (c.IsCircle()) return "circle";
        if (c.IsEllipse()) return "ellipse";
        if (c.IsBcurve()) return "bspline";
        return "other";
    }

    public static FaceGeom Describe(IFace2 face)
    {
        var s = face.Surface();
        string kind = SurfaceKind(s);
        switch (kind)
        {
            case "plane":
            {
                var n = face.NormalVec();
                return new FaceGeom(kind, n.Length >= 3 ? new Vec3(n[0], n[1], n[2]).Normalized() : null, null, null, null, null);
            }
            case "cylinder":
            {
                var p = ComArgs.Doubles((object?)s!.CylinderParams);
                return new FaceGeom(kind, null, Vec3.From(p), Vec3.From(p, 3).Normalized(), p[6], null);
            }
            case "cone":
            {
                var p = ComArgs.Doubles((object?)s!.ConeParams);
                return new FaceGeom(kind, null, Vec3.From(p), Vec3.From(p, 3).Normalized(), p[6], p.Length > 7 ? p[7] : null);
            }
            case "sphere":
            {
                var p = ComArgs.Doubles((object?)s!.SphereParams);
                return new FaceGeom(kind, null, Vec3.From(p), null, p[3], null);
            }
            case "torus":
            {
                var p = ComArgs.Doubles((object?)s!.TorusParams);
                return new FaceGeom(kind, null, Vec3.From(p), Vec3.From(p, 3).Normalized(), p[6], null);
            }
            default:
                return new FaceGeom(kind, null, null, null, null, null);
        }
    }

    public static EdgeGeom Describe(IEdge edge)
    {
        var cp = edge.CurveParams(); // [start xyz, end xyz, t0, t1, ...]
        var start = Vec3.From(cp, 0);
        var end = Vec3.From(cp, 3);
        double t0 = cp[6], t1 = cp[7];
        var c = edge.Curve();
        var midArr = edge.EvalAt((t0 + t1) / 2);
        var mid = midArr.Length >= 3 ? Vec3.From(midArr) : (start + end) / 2;
        double length = c is null ? start.DistanceTo(end) : c.GetLength3(t0, t1);
        string kind = CurveKind(c);
        bool closed = start.DistanceTo(end) < 1e-9 && length > 1e-9;
        if (kind == "circle")
        {
            var p = ComArgs.Doubles((object?)c!.CircleParams);
            return new EdgeGeom(kind, start, end, mid, length, Vec3.From(p), Vec3.From(p, 3).Normalized(), p[6], null, closed);
        }
        if (kind == "line")
        {
            var dir = (end - start).Normalized();
            return new EdgeGeom(kind, start, end, mid, length, null, null, null, dir, false);
        }
        return new EdgeGeom(kind, start, end, mid, length, null, null, null, null, closed);
    }

    /// <summary>
    /// A point strictly inside the face (not on its boundary), so that resolving it back gives exactly this face:
    /// the centroid of one of the largest tessellation triangles, projected onto the face, choosing the candidate
    /// farthest from the face's edges. Falls back to the face box centre projected onto the face.
    /// </summary>
    public static Vec3 InteriorPoint(IFace2 face)
    {
        var edges = face.Edges();
        Vec3? best = null;
        double bestScore = -1;
        var tess = ComArgs.Doubles((object?)face.GetTessTriangles(true));
        if (tess.Length >= 9)
        {
            var tris = new List<(double Area, Vec3 C)>();
            for (int i = 0; i + 8 < tess.Length; i += 9)
            {
                var a = Vec3.From(tess, i);
                var b = Vec3.From(tess, i + 3);
                var c = Vec3.From(tess, i + 6);
                double area = (b - a).Cross(c - a).Length / 2;
                tris.Add((area, (a + b + c) / 3));
            }
            foreach (var (_, centroid) in tris.OrderByDescending(t => t.Area).Take(3))
            {
                var cp = face.ClosestPoint(centroid);
                if (cp.Length < 3) continue;
                var p = Vec3.From(cp);
                double score = edges.Count == 0 ? double.MaxValue : edges.Min(e => DistanceToEdge(e, p));
                if (score > bestScore) { bestScore = score; best = p; }
            }
        }
        if (best is null)
        {
            var box = face.Box();
            var center = box.Length == 6 ? new Vec3((box[0] + box[3]) / 2, (box[1] + box[4]) / 2, (box[2] + box[5]) / 2) : Vec3.Zero;
            var cp = face.ClosestPoint(center);
            best = cp.Length >= 3 ? Vec3.From(cp) : center;
        }
        return best.Value;
    }

    /// <summary>
    /// The face's unit normal at <paramref name="p"/> pointing out of the material: the plane's normal, else the surface
    /// normal there (ISurface.EvaluateAtPoint) reversed when IFace2.FaceInSurfaceSense is true, which means the face and
    /// the surface point in opposite directions despite its name (api help, VERIFIED: a hole's face points to its axis).
    /// Zero when it can't be read.
    /// </summary>
    public static Vec3 OutwardNormal(IFace2 f, FaceGeom g, Vec3 p)
    {
        if (g.Surface == "plane" && g.Normal is Vec3 n) return n;
        var ev = ComArgs.Doubles((object?)f.Surface()?.EvaluateAtPoint(p.X, p.Y, p.Z));
        if (ev.Length < 3) return Vec3.Zero;
        var sn = new Vec3(ev[0], ev[1], ev[2]).Normalized();
        return f.FaceInSurfaceSense() ? -sn : sn;
    }

    /// <summary>True when a cylindrical face is a hole (material outside it), false for a boss or shaft.</summary>
    public static bool IsHole(IFace2 cylinder, FaceGeom g)
    {
        if (g.Surface != "cylinder" || g.Axis is not Vec3 ax || g.AxisOrigin is not Vec3 o) return false;
        var p = InteriorPoint(cylinder);
        var toAxis = (o - p) - ax * (o - p).Dot(ax);
        return OutwardNormal(cylinder, g, p).Dot(toAxis) > 0;
    }

    public static double DistanceToEdge(IEdge e, Vec3 p)
    {
        var cp = e.ClosestPoint(p);
        return cp.Length >= 3 ? Vec3.From(cp).DistanceTo(p) : double.MaxValue;
    }

    public static double DistanceToFace(IFace2 f, Vec3 p)
    {
        var cp = f.ClosestPoint(p);
        return cp.Length >= 3 ? Vec3.From(cp).DistanceTo(p) : double.MaxValue;
    }

    /// <summary>
    /// The hits of rays on bodies, one RayIntersections call (docs/solidworks-api-notes.md, Ray casts): per ray, each hit's
    /// body index, distance along the ray (m, ahead of its start only) and hit type.
    /// </summary>
    public static List<(int Body, double Dist, int Type)>[] RayHits(IModelDoc2 d, IReadOnlyList<IBody2> bodies, IReadOnlyList<(Vec3 From, Vec3 Dir)> rays)
    {
        var result = rays.Select(_ => new List<(int, double, int)>()).ToArray();
        if (rays.Count == 0 || bodies.Count == 0) return result;
        var pts = rays.SelectMany(r => new[] { r.From.X, r.From.Y, r.From.Z }).ToArray();
        var dirs = rays.SelectMany(r => new[] { r.Dir.X, r.Dir.Y, r.Dir.Z }).ToArray();
        int n = d.Extension.RayIntersections(ComArgs.DispatchArray(bodies.ToList()), pts, dirs, 0, 1e-7, 1e-7, true);
        if (n <= 0) return result;
        var h = ComArgs.Doubles((object?)d.GetRayIntersectionsPoints());
        for (int k = 0; k + 5 < h.Length; k += 6)
        {
            int ray = (int)h[k + 1];
            if (ray < 0 || ray >= rays.Count) continue;
            double dist = (new Vec3(h[k + 3], h[k + 4], h[k + 5]) - rays[ray].From).Dot(rays[ray].Dir);
            if (dist > 0) result[ray].Add(((int)h[k], dist, (int)h[k + 2]));
        }
        return result;
    }

    /// <summary>
    /// Whether each point lies strictly inside one of the part's solid bodies: a ray each way along
    /// <paramref name="dir"/> (pick one oblique to the faces near the points) crosses that body's faces an odd number of
    /// times. Hits within 1 µm of the point are left out, so a point on a face counts as not inside (live: the ray
    /// leaving the body then has none).
    /// </summary>
    public static bool[] InsideSolid(SwContext ctx, IReadOnlyList<Vec3> points, Vec3 dir)
    {
        var part = ctx.Part();
        var bodies = part.Bodies(SolidWorks.Interop.swconst.swBodyType_e.swSolidBody, visibleOnly: false);
        var rays = points.SelectMany(p => new[] { (p, dir), (p, -dir) }).ToList();
        var hits = RayHits((IModelDoc2)part, bodies, rays);
        bool Odd(int ray, int body) => hits[ray].Count(h => h.Body == body && h.Dist > 1e-6) % 2 == 1;
        return points.Select((_, i) => Enumerable.Range(0, bodies.Count).Any(b => Odd(2 * i, b) && Odd(2 * i + 1, b))).ToArray();
    }
}
