using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorksMcp.Core.Com;
using SolidWorksMcp.Core.Geometry;

namespace SolidWorksMcp.Core.Services;

/// <summary>
/// Where things land on a sheet (S14): the sheet's frame (border, title block), the boxes of notes, dimension texts and
/// tables, and the warnings when a view, a note, a dimension, a label or a table lands off the sheet, across the border,
/// on the title block, on a view (S106) or on another annotation.
/// </summary>
public sealed partial class DrawingService
{
    /// <summary>One annotation's box on the sheet (mm), and what it is.</summary>
    private sealed record Placed(string Name, string Kind, string View, SheetRect Box, double? Value);

    /// <summary>
    /// The box an annotation covers on the sheet (mm): a note's (and a view label's) by INote.GetExtent, a dimension's
    /// text estimated from its display data (IDisplayData text position, reference corner, angle and height; 0.6 × the
    /// height a character, as SOLIDWORKS spaces its default font), a table's from its anchor corner and its visible
    /// columns and rows (S106: ITableAnnotation gives no extent). Null for other kinds or when it cannot be read.
    /// </summary>
    private static SheetRect? BoxOf(IAnnotation a)
    {
        try
        {
            object? spec = (object?)a.GetSpecificAnnotation();
            if (spec is ITableAnnotation ta)
            {
                var p = a.DrwPosition();
                if (p.Length < 2) return null;
                double w = 0, h = 0;
                for (int i = 0, cols = ta.TotalColumnCount; i < cols; i++)
                    if (!ta.ColumnHidden[i]) w += ta.GetColumnWidth(i);
                for (int i = 0, rows = ta.TotalRowCount; i < rows; i++)
                    if (!ta.RowHidden[i]) h += ta.GetRowHeight(i);
                return DrawingLayout.TableBox(ta.AnchorType, Units.Mm(p[0]), Units.Mm(p[1]), Units.Mm(w), Units.Mm(h));
            }
            if (spec is INote n)
            {
                var e = ComArgs.Doubles((object?)n.GetExtent());
                if (e.Length >= 6 && e.Take(6).All(double.IsFinite))
                    return new SheetRect(Units.Mm(Math.Min(e[0], e[3])), Units.Mm(Math.Min(e[1], e[4])), Units.Mm(Math.Max(e[0], e[3])), Units.Mm(Math.Max(e[1], e[4])));
                return null;
            }
            if (spec is not IDisplayDimension) return null;
            if ((object?)a.GetDisplayData() is not IDisplayData dd) return null;
            SheetRect? box = null;
            for (int i = 0, n2 = dd.GetTextCount(); i < n2; i++)
            {
                string t = dd.GetTextAtIndex(i) ?? "";
                int chars = Regex.Replace(t, "<[^>]*>", "#").TrimEnd().Length;
                if (chars == 0) continue;
                var p = ComArgs.Doubles((object?)dd.GetTextPositionAtIndex(i));
                double h = Units.Mm(dd.GetTextHeightAtIndex(i));
                if (p.Length < 2 || !(h > 0)) continue;
                var r = SheetFrame.TextBox(Units.Mm(p[0]), Units.Mm(p[1]), chars * 0.6 * h, h, dd.GetTextRefPositionAtIndex(i), dd.GetTextAngleAtIndex(i));
                box = box is SheetRect b ? new SheetRect(Math.Min(b.X0, r.X0), Math.Min(b.Y0, r.Y0), Math.Max(b.X1, r.X1), Math.Max(b.Y1, r.Y1)) : r;
            }
            return box;
        }
        catch (COMException) { return null; }
    }

    /// <summary>The boxes of the notes, labels, dimensions and tables on the current sheet, its own and its views', not the format's.</summary>
    private static List<Placed> PlacedOnSheet(SwContext ctx, IDrawingDoc drw)
    {
        var list = new List<Placed>();
        try
        {
            var (scope, sheet) = Scope(ctx, drw, null);
            foreach (var an in Annotations(scope, sheet))
            {
                if (an.Item.Format == true || an.Item.Kind is not ("note" or "view_label" or "dimension" or "hole_callout" or "balloon" or "table")) continue;
                if (BoxOf(an.A) is SheetRect r) list.Add(new Placed(an.Item.Name, an.Item.Kind, an.Item.View, r, an.Item.Value));
            }
        }
        catch (COMException e) when (!ComArgs.IsDisconnect(e)) { /* informational only */ }
        return list;
    }

    /// <summary>The views' outlines on the current sheet (their boundsMm), as boxes a note, a dimension or a table lands on (S106).</summary>
    private static List<Placed> ViewBoxes(IDrawingDoc drw)
    {
        try
        {
            return (drw.DrwCurrentSheet()?.DrwViews() ?? [])
                .Select(v => (Name: v.GetName2() ?? "", R: SheetRect.FromMeters(v.DrwOutline())))
                .Where(x => x.R is not null).Select(x => new Placed(x.Name, "view", x.Name, x.R!.Value, null)).ToList();
        }
        catch (COMException e) when (!ComArgs.IsDisconnect(e)) { return []; }
    }

    /// <summary>
    /// Where a new view may go on a sheet (mm): the sheet inside its 5 mm edge and the format's border, and what is taken
    /// there: the views' outlines (but <paramref name="except"/>), the format's blocks (the title block, S90) and the
    /// tables (S128; read on the current sheet, as the overlap check reads them).
    /// </summary>
    private static (SheetRect Area, List<SheetRect> Occupied) FreeSpace(SwContext ctx, ISheet sheet, string? except = null)
    {
        var area = SheetArea(sheet);
        var occupied = sheet.DrwViews().Where(v => except is null || !string.Equals(v.GetName2(), except, StringComparison.OrdinalIgnoreCase))
            .Select(v => SheetRect.FromMeters(v.DrwOutline())).OfType<SheetRect>().ToList();
        var drw = ctx.Drawing();
        if (FrameOfSheet(drw, sheet) is { } frame)
        {
            occupied.AddRange(frame.Blocks);
            if (frame.Border is SheetRect border) area = new SheetRect(Math.Max(area.X0, border.X0), Math.Max(area.Y0, border.Y0), Math.Min(area.X1, border.X1), Math.Min(area.Y1, border.Y1));
        }
        if (string.Equals(drw.DrwCurrentSheet()?.GetName(), sheet.GetName(), StringComparison.OrdinalIgnoreCase))
            occupied.AddRange(PlacedOnSheet(ctx, drw).Where(p => p.Kind == "table").Select(p => p.Box));
        return (area, occupied);
    }

    /// <summary>
    /// S128: an auxiliary view made with no atMm goes to the first free spot along its alignment, on the side of its
    /// parent it was made on (across the parent it would lie opposite its arrow): what it covers, its outline and its own
    /// label, clear of the other views, the tables, the title block and the border. Where there is none it stays, and
    /// the result says so with the hint, rather than claim a free spot.
    /// </summary>
    private static void PlaceAlongAlignment(SwContext ctx, ISheet sheet, ViewHit parent, IView v, (double X, double Y) dir, List<string> checks)
    {
        string name = v.GetName2() ?? "";
        if (SheetRect.FromMeters(v.DrwOutline()) is not SheetRect now) return;
        var foot = PlacedOnSheet(ctx, ctx.Drawing()).Where(p => p.Kind == "view_label" && string.Equals(p.View, name, StringComparison.OrdinalIgnoreCase))
            .Select(p => p.Box).Aggregate(now, (a, b) => new SheetRect(Math.Min(a.X0, b.X0), Math.Min(a.Y0, b.Y0), Math.Max(a.X1, b.X1), Math.Max(a.Y1, b.Y1)));
        var (area, occupied) = FreeSpace(ctx, sheet, name);
        var p0 = Outline(parent);
        double back = (now.Cx - p0.Cx) * dir.X + (now.Cy - p0.Cy) * dir.Y; // the view's centre along the line, from the parent's
        double? found = DrawingLayout.FirstFreeAlong(foot, (dir.X, dir.Y), -back, 3000, area, occupied);
        if (found is not double t)
        {
            ctx.Warn($"No free spot of {Math.Round(foot.W)} x {Math.Round(foot.H)} mm for '{name}' along its alignment with '{parent.Name}', on the side it was made on: it was left " +
                     $"at [{Fmt([now.Cx, now.Cy])}] mm. Pass atMm, or give '{parent.Name}' a smaller scale.");
            return;
        }
        if (Math.Abs(t) < 0.5) { checks.Add($"a free spot along its alignment with '{parent.Name}'"); return; }
        var pos = v.DrwPosition();
        if (pos.Length < 2) return;
        v.DrwMoveTo(pos[0] + Units.M(dir.X * t), pos[1] + Units.M(dir.Y * t));
        ctx.Doc.GraphicsRedraw2();
        var got = SheetRect.FromMeters(v.DrwOutline());
        double wx = now.Cx + dir.X * t, wy = now.Cy + dir.Y * t;
        if (got is SheetRect g && Math.Abs(g.Cx - wx) <= 0.5 && Math.Abs(g.Cy - wy) <= 0.5)
            checks.Add($"moved {Math.Round(Math.Abs(t), 1)} mm along its alignment with '{parent.Name}' to the first free spot");
        else
        {
            v.DrwMoveTo(pos[0], pos[1]);
            ctx.Doc.GraphicsRedraw2();
            ctx.Warn($"'{name}' could not be moved along its alignment to the free spot at [{Fmt([wx, wy])}] mm (it read " +
                     $"[{(got is SheetRect r ? Fmt([r.Cx, r.Cy]) : "?")}]); it was left where it was made. Pass atMm.");
        }
    }

    /// <summary>The frame of a sheet of the drawing (the current one through its own view; another through GetViews).</summary>
    private static SheetFrame? FrameOfSheet(IDrawingDoc drw, ISheet sheet)
    {
        try
        {
            string name = sheet.GetName();
            if (string.Equals(drw.DrwCurrentSheet()?.GetName(), name, StringComparison.OrdinalIgnoreCase)) return CurrentFrame(drw);
            var sv = drw.DrwViewsBySheet().FirstOrDefault(a => a.Count > 0 && string.Equals(a[0].GetName2(), name, StringComparison.OrdinalIgnoreCase))?[0];
            return FrameOf(sheet, sv);
        }
        catch (COMException) { return null; }
    }

    /// <summary>
    /// What a box (an annotation's, sheet mm) lands on: the frame (off the sheet, across the border, on the title block)
    /// and the other annotations' boxes it overlaps, a few named. Empty when it lies free.
    /// </summary>
    private static List<string> LandsOn(SheetFrame? frame, IEnumerable<Placed> others, SheetRect box, double? value = null)
    {
        var found = frame?.Clashes(box) ?? [];
        var hits = others.Where(o => o.Box.Overlaps(box.Inflate(-0.05))).ToList();
        if (hits.Count > 0)
            found.Add("overlaps " + string.Join(", ", hits.Take(4).Select(o =>
                          $"{o.Kind.Replace('_', ' ')} '{o.Name}'{(o.Kind is "view_label" or "view" ? "" : $" of '{o.View}'")} {SheetFrame.Fmt(o.Box)}" +
                          (value is double v && o.Value is double ov && Math.Abs(v - ov) < 1e-6 ? " (the same value: a duplicate?)" : ""))) +
                      (hits.Count > 4 ? $" and {hits.Count - 4} more" : ""));
        return found;
    }

    /// <summary>
    /// After a note, a dimension or a move: a check when the annotation lies free, else a warning that names what it
    /// lands on (S14), the tables and the other views' outlines included (S106; its own view's is where it belongs).
    /// <paramref name="what"/> starts the sentence ("The note"), <paramref name="self"/> is left out.
    /// </summary>
    private static void PlacementCheck(SwContext ctx, IDrawingDoc drw, IAnnotation a, string self, string what, List<string> checks, out SheetRect? box,
        double? value = null)
    {
        box = BoxOf(a);
        if (box is not SheetRect b) return;
        var placed = PlacedOnSheet(ctx, drw);
        string? own = placed.FirstOrDefault(o => string.Equals(o.Name, self, StringComparison.OrdinalIgnoreCase))?.View;
        var others = placed.Where(o => !string.Equals(o.Name, self, StringComparison.OrdinalIgnoreCase))
            .Concat(ViewBoxes(drw).Where(v => !string.Equals(v.Name, own, StringComparison.OrdinalIgnoreCase))).ToList();
        var found = LandsOn(CurrentFrame(drw), others, b, value);
        if (found.Count == 0) checks.Add("clear of the border, the title block, the other views and the other annotations");
        else ctx.Warn($"{what} {SheetFrame.Fmt(b)} {string.Join("; ", found)}.");
    }

    /// <summary>
    /// After drw_table_add (S106): the table's box on the current sheet, and a check when it lies free, else a warning
    /// naming the views, the frame and the other annotations it lands on. Null, with a warning, when it cannot be read.
    /// </summary>
    private static SheetRect? TableCheck(SwContext ctx, IDrawingDoc drw, ITableAnnotation table, List<string> checks)
    {
        var an = Safe(() => (object?)table.GetAnnotation() as IAnnotation, null);
        if (an is null || BoxOf(an) is not SheetRect box)
        {
            ctx.Warn("The table's extent could not be read, so what it lands on was not checked.");
            return null;
        }
        string self = Safe(() => an.GetName(), "") ?? "";
        var others = PlacedOnSheet(ctx, drw).Where(o => !(o.Kind == "table" && string.Equals(o.Name, self, StringComparison.OrdinalIgnoreCase)))
            .Concat(ViewBoxes(drw)).ToList();
        var found = LandsOn(CurrentFrame(drw), others, box);
        if (found.Count == 0) checks.Add("clear of the views, the border, the title block and the other annotations");
        else ctx.Warn($"The table {SheetFrame.Fmt(box)} {string.Join("; ", found)}.");
        return box;
    }
}
