using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorksMcp.Core.Com;
using SolidWorksMcp.Core.Geometry;

namespace SolidWorksMcp.Core.Services;

/// <summary>
/// Where things land on a sheet (S14): the sheet's frame (border, title block), the boxes of notes and dimension texts,
/// and the warnings when a view, a note, a dimension or a label lands off the sheet, across the border, on the title
/// block or on another annotation.
/// </summary>
public sealed partial class DrawingService
{
    /// <summary>One annotation's box on the sheet (mm), and what it is.</summary>
    private sealed record Placed(string Name, string Kind, string View, SheetRect Box, double? Value);

    /// <summary>
    /// The box an annotation covers on the sheet (mm): a note's (and a view label's) by INote.GetExtent, a dimension's
    /// text estimated from its display data (IDisplayData text position, reference corner, angle and height; 0.6 × the
    /// height a character, as SOLIDWORKS spaces its default font). Null for other kinds or when it cannot be read.
    /// </summary>
    private static SheetRect? BoxOf(IAnnotation a)
    {
        try
        {
            object? spec = (object?)a.GetSpecificAnnotation();
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

    /// <summary>The boxes of the notes, labels and dimensions on the current sheet, its own and its views', not the format's.</summary>
    private static List<Placed> PlacedOnSheet(SwContext ctx, IDrawingDoc drw)
    {
        var list = new List<Placed>();
        try
        {
            var (scope, sheet) = Scope(ctx, drw, null);
            foreach (var an in Annotations(scope, sheet))
            {
                if (an.Item.Format == true || an.Item.Kind is not ("note" or "view_label" or "dimension" or "hole_callout" or "balloon")) continue;
                if (BoxOf(an.A) is SheetRect r) list.Add(new Placed(an.Item.Name, an.Item.Kind, an.Item.View, r, an.Item.Value));
            }
        }
        catch (COMException e) when (!ComArgs.IsDisconnect(e)) { /* informational only */ }
        return list;
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
                          $"{o.Kind.Replace('_', ' ')} '{o.Name}'{(o.Kind == "view_label" ? "" : $" of '{o.View}'")} {SheetFrame.Fmt(o.Box)}" +
                          (value is double v && o.Value is double ov && Math.Abs(v - ov) < 1e-6 ? " (the same value: a duplicate?)" : ""))) +
                      (hits.Count > 4 ? $" and {hits.Count - 4} more" : ""));
        return found;
    }

    /// <summary>
    /// After a note, a dimension or a move: a check when the annotation lies free, else a warning that names what it
    /// lands on (S14). <paramref name="what"/> starts the sentence ("The note"), <paramref name="self"/> is left out.
    /// </summary>
    private static void PlacementCheck(SwContext ctx, IDrawingDoc drw, IAnnotation a, string self, string what, List<string> checks, out SheetRect? box,
        double? value = null)
    {
        box = BoxOf(a);
        if (box is not SheetRect b) return;
        var others = PlacedOnSheet(ctx, drw).Where(o => !string.Equals(o.Name, self, StringComparison.OrdinalIgnoreCase)).ToList();
        var found = LandsOn(CurrentFrame(drw), others, b, value);
        if (found.Count == 0) checks.Add("clear of the border, the title block and the other annotations");
        else ctx.Warn($"{what} {SheetFrame.Fmt(b)} {string.Join("; ", found)}.");
    }
}
