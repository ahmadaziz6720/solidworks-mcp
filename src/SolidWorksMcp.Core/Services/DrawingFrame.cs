using System.Globalization;
using SolidWorksMcp.Core.Geometry;

namespace SolidWorksMcp.Core.Services;

/// <summary>A straight line on a drawing sheet (mm).</summary>
internal readonly record struct SheetLine(double X0, double Y0, double X1, double Y1)
{
    public SheetRect Box => new(Math.Min(X0, X1), Math.Min(Y0, Y1), Math.Max(X0, X1), Math.Max(Y0, Y1));

    public bool Horizontal(double tol) => Math.Abs(Y1 - Y0) <= tol;
    public bool Vertical(double tol) => Math.Abs(X1 - X0) <= tol;

    /// <summary>The shortest distance between two segments (0 when they cross or touch).</summary>
    public double DistanceTo(SheetLine o)
    {
        if (Crosses(o)) return 0;
        return Math.Min(Math.Min(PointDist(o.X0, o.Y0), PointDist(o.X1, o.Y1)), Math.Min(o.PointDist(X0, Y0), o.PointDist(X1, Y1)));
    }

    private double PointDist(double px, double py)
    {
        double dx = X1 - X0, dy = Y1 - Y0, l2 = dx * dx + dy * dy;
        double t = l2 == 0 ? 0 : Math.Clamp(((px - X0) * dx + (py - Y0) * dy) / l2, 0, 1);
        double cx = X0 + t * dx - px, cy = Y0 + t * dy - py;
        return Math.Sqrt(cx * cx + cy * cy);
    }

    private bool Crosses(SheetLine o)
    {
        static double Cross(double ax, double ay, double bx, double by, double cx, double cy) => (bx - ax) * (cy - ay) - (by - ay) * (cx - ax);
        double d1 = Cross(o.X0, o.Y0, o.X1, o.Y1, X0, Y0), d2 = Cross(o.X0, o.Y0, o.X1, o.Y1, X1, Y1);
        double d3 = Cross(X0, Y0, X1, Y1, o.X0, o.Y0), d4 = Cross(X0, Y0, X1, Y1, o.X1, o.Y1);
        return ((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) && ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0));
    }
}

/// <summary>
/// What a sheet's format leaves free: the sheet, the inside of the format's border (the drawable area) and the blocks the
/// format draws inside it, the title block first (S14). Null border: a sheet with no format.
/// </summary>
internal sealed record SheetFrame(SheetRect Sheet, SheetRect? Border, IReadOnlyList<SheetRect> Blocks, SheetRect? TitleBlock)
{
    private const double Tol = 0.05;

    /// <summary>
    /// The frame from the format's lines (sheet mm) and, for a format whose lines draw no border, its zone margins
    /// [top, bottom, right, left] (ISheet.GetZoneMargin, mm).
    /// </summary>
    public static SheetFrame From(double widthMm, double heightMm, bool hasFormat, IReadOnlyList<SheetLine> lines, double[]? zoneMargins)
    {
        var sheet = new SheetRect(0, 0, widthMm, heightMm);
        if (!hasFormat) return new SheetFrame(sheet, null, [], null);
        var border = InnermostBorder(lines, widthMm, heightMm);
        if (border is null && zoneMargins is { Length: 4 } m && m.All(v => double.IsFinite(v) && v >= 0) && m.Any(v => v > 0) &&
            m[3] + m[2] < widthMm && m[0] + m[1] < heightMm)
            border = new SheetRect(m[3], m[1], widthMm - m[2], heightMm - m[0]);
        if (border is not SheetRect b) return new SheetFrame(sheet, null, [], null);

        // The format's lines inside the border, not on it: grouped by touch, each group a block.
        var inside = lines.Where(l => l.Box.Inside(b, Tol) && !OnEdge(l, b)).ToList();
        var groups = Groups(inside);
        var blocks = groups.Select(g => Union(g.Select(l => l.Box))).Where(r => r.W > 1 && r.H > 1)
            .OrderByDescending(r => r.W * r.H).ToList();
        SheetRect? title = blocks.Cast<SheetRect?>().FirstOrDefault(r => Touches(r!.Value, b));
        if (title is SheetRect t) blocks = [t, .. blocks.Where(r => r != t)];
        return new SheetFrame(sheet, b, blocks, title);
    }

    /// <summary>
    /// Where a rectangle (sheet mm) lies against the frame: off the sheet, across the border, on the title block or another
    /// block of the format. Empty when it lies free.
    /// </summary>
    public List<string> Clashes(SheetRect r)
    {
        var found = new List<string>();
        if (!r.Inside(Sheet, Tol)) found.Add(Inv($"runs off the sheet ({Sheet.W:0.#} x {Sheet.H:0.#} mm)"));
        else if (Border is SheetRect b && !r.Inside(b, Tol)) found.Add($"crosses the format's border (drawable area {Fmt(b)})");
        for (int i = 0; i < Blocks.Count; i++)
            if (r.Overlaps(Blocks[i].Inflate(-Tol)))
                found.Add(TitleBlock is not null && i == 0 ? $"overlaps the title block {Fmt(Blocks[i])}" : $"overlaps a block of the format {Fmt(Blocks[i])}");
        return found;
    }

    public static string Fmt(SheetRect r) => "[" + string.Join(", ", r.ToArray().Select(v => v.ToString(CultureInfo.InvariantCulture))) + "]";

    private static string Inv(FormattableString f) => f.ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// The box of a text <paramref name="w"/> × <paramref name="h"/> anchored at (x, y) by its reference corner
    /// (swTextPosition_e: 0 upper left, 1 lower left, 2 centre, 3 upper right, 4 lower right, 5 upper centre), turned by
    /// <paramref name="angle"/> (rad) about that point.
    /// </summary>
    public static SheetRect TextBox(double x, double y, double w, double h, int refPos, double angle)
    {
        double dx = refPos switch { 2 or 5 => -w / 2, 3 or 4 => -w, _ => 0 };
        double dy = refPos switch { 0 or 3 or 5 => -h, 2 => -h / 2, _ => 0 };
        double c = Math.Cos(angle), s = Math.Sin(angle);
        (double u, double v)[] corners = [(dx, dy), (dx + w, dy), (dx + w, dy + h), (dx, dy + h)];
        var xs = corners.Select(p => x + p.u * c - p.v * s).ToArray();
        var ys = corners.Select(p => y + p.u * s + p.v * c).ToArray();
        return new SheetRect(xs.Min(), ys.Min(), xs.Max(), ys.Max());
    }

    private static SheetRect? InnermostBorder(IReadOnlyList<SheetLine> lines, double w, double h)
    {
        // Axis-aligned rectangles the lines close, at least half the sheet each way; the innermost is the border.
        var hs = lines.Where(l => l.Horizontal(Tol)).Select(l => l.Box).ToList();
        var vs = lines.Where(l => l.Vertical(Tol)).Select(l => l.Box).ToList();
        SheetRect? best = null;
        for (int i = 0; i < hs.Count; i++)
            for (int j = 0; j < hs.Count; j++)
            {
                var lo = hs[i]; var hi = hs[j];
                if (hi.Y0 - lo.Y0 < h / 2 || Math.Abs(lo.X0 - hi.X0) > Tol || Math.Abs(lo.X1 - hi.X1) > Tol || lo.W < w / 2) continue;
                var r = new SheetRect(lo.X0, lo.Y0, lo.X1, hi.Y0);
                bool left = vs.Any(v => Math.Abs(v.X0 - r.X0) <= Tol && v.Y0 <= r.Y0 + Tol && v.Y1 >= r.Y1 - Tol);
                bool right = vs.Any(v => Math.Abs(v.X0 - r.X1) <= Tol && v.Y0 <= r.Y0 + Tol && v.Y1 >= r.Y1 - Tol);
                if (left && right && (best is null || r.W * r.H < best.Value.W * best.Value.H)) best = r;
            }
        return best;
    }

    private static bool OnEdge(SheetLine l, SheetRect b) =>
        (l.Horizontal(Tol) && (Math.Abs(l.Y0 - b.Y0) <= Tol || Math.Abs(l.Y0 - b.Y1) <= Tol)) ||
        (l.Vertical(Tol) && (Math.Abs(l.X0 - b.X0) <= Tol || Math.Abs(l.X0 - b.X1) <= Tol));

    private static bool Touches(SheetRect r, SheetRect b) =>
        Math.Abs(r.X0 - b.X0) <= Tol || Math.Abs(r.X1 - b.X1) <= Tol || Math.Abs(r.Y0 - b.Y0) <= Tol || Math.Abs(r.Y1 - b.Y1) <= Tol;

    private static List<List<SheetLine>> Groups(List<SheetLine> lines)
    {
        int n = lines.Count;
        var parent = Enumerable.Range(0, n).ToArray();
        int Find(int i) { while (parent[i] != i) i = parent[i] = parent[parent[i]]; return i; }
        for (int i = 0; i < n; i++)
            for (int j = i + 1; j < n; j++)
                if (lines[i].DistanceTo(lines[j]) <= Tol) parent[Find(i)] = Find(j);
        return Enumerable.Range(0, n).GroupBy(Find).Select(g => g.Select(i => lines[i]).ToList()).ToList();
    }

    private static SheetRect Union(IEnumerable<SheetRect> rs)
    {
        var l = rs.ToList();
        return new SheetRect(l.Min(r => r.X0), l.Min(r => r.Y0), l.Max(r => r.X1), l.Max(r => r.Y1));
    }
}

/// <summary>Which model direction points along the sheet's +x (right), +y (up) and out of the sheet, for a view (S14).</summary>
public sealed record DrwAxes
{
    /// <summary>The model direction toward the sheet's right, e.g. "+X"; a vector when it is not along an axis.</summary>
    public required string Right { get; init; }
    /// <summary>The model direction toward the sheet's top.</summary>
    public required string Up { get; init; }
    /// <summary>The model direction out of the sheet, toward the reader.</summary>
    public required string Out { get; init; }

    internal static DrwAxes Of(ViewAxes a) => new() { Right = Name(a.SheetX), Up = Name(a.SheetY), Out = Name(a.Toward) };

    internal static string Name(Vec3 v)
    {
        var u = v.Normalized();
        (double c, string n)[] axes = [(u.X, "X"), (u.Y, "Y"), (u.Z, "Z")];
        foreach (var (c, n) in axes)
            if (Math.Abs(Math.Abs(c) - 1) < 1e-6) return (c > 0 ? "+" : "-") + n;
        return FormattableString.Invariant($"[{Math.Round(u.X, 4) + 0.0}, {Math.Round(u.Y, 4) + 0.0}, {Math.Round(u.Z, 4) + 0.0}]");
    }
}
