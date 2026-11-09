using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorksMcp.Core.Com;
using SolidWorksMcp.Core.Geometry;
using SolidWorksMcp.Core.Model;
using SolidWorksMcp.Core.Verify;

namespace SolidWorksMcp.Core.Services;

/// <summary>One annotation of a drawing as drw_annotation_list reads it.</summary>
public sealed class DrwAnnotationItem
{
    /// <summary>
    /// The name drw_annotation_edit takes: a dimension's "RD1@Drawing View1" (as drw_dimension_add returns it), a note's
    /// "DetailItem12" (as drw_note_add returns it), else the annotation's own name.
    /// </summary>
    public required string Name { get; init; }
    /// <summary>dimension | hole_callout | note | balloon | table | center_mark | center_line | datum | surface_finish | gtol | weld | other.</summary>
    public required string Kind { get; init; }
    /// <summary>The view it belongs to, or the sheet's name for an annotation on the sheet itself.</summary>
    public required string View { get; init; }
    public required string Sheet { get; init; }
    /// <summary>A note's text, a datum's letter, a hole callout.</summary>
    public string? Text { get; init; }
    /// <summary>A dimension's value (mm or deg).</summary>
    public double? Value { get; init; }
    public string? Unit { get; init; }
    /// <summary>[x, y] sheet mm: the text of a dimension or note, the symbol of the others.</summary>
    public double[]? AtMm { get; init; }
    /// <summary>How many model entities it is attached to (0: free-standing); absent when not known.</summary>
    public int? AttachedTo { get; init; }
}

public sealed class DrwAnnotationListResult : ToolResult
{
    /// <summary>The sheet listed, or the view's sheet.</summary>
    public required string Sheet { get; init; }
    public int Count { get; init; }
    public required IReadOnlyList<DrwAnnotationItem> Annotations { get; init; }
    public bool? Truncated { get; init; }
}

public sealed class DrwAnnotationEditResult : ToolResult
{
    public required string Action { get; init; }
    /// <summary>The annotation as it was before the call.</summary>
    public required DrwAnnotationItem Before { get; init; }
    /// <summary>The annotation read back after the call (absent after a delete).</summary>
    public DrwAnnotationItem? After { get; init; }
    public IReadOnlyList<string>? Checks { get; init; }
}

public sealed class DrwViewDeleteResult : ToolResult
{
    /// <summary>The view asked for.</summary>
    public required string View { get; init; }
    /// <summary>Every view that is gone after the call (the one asked for, and any SOLIDWORKS took with it).</summary>
    public required IReadOnlyList<string> Removed { get; init; }
    /// <summary>The annotations the view held, which went with it.</summary>
    public DrwCounts? Annotations { get; init; }
    /// <summary>Views whose parent was the deleted view and that are still there.</summary>
    public IReadOnlyList<string>? ChildrenKept { get; init; }
    public int ViewCount { get; init; }
    public IReadOnlyList<string>? Checks { get; init; }
}

public sealed partial class DrawingService
{
    private const int MaxAnnotations = 300; // drw_annotation_list

    private sealed record Ann(IAnnotation A, DrwAnnotationItem Item, ViewHit Owner);

    // ================================================================== drw_annotation_list

    public Task<DrwAnnotationListResult> AnnotationListAsync(string? view, string? kind, string? doc, CancellationToken ct = default)
    {
        string? k = DrawingLayout.Norm(kind);
        if (k is not null && !AnnotationKinds.Contains(k))
            throw SwException.InvalidArgument($"Unknown kind '{kind}'.", $"Use one of {string.Join(", ", AnnotationKinds)}.");
        return session.RunAsync("drw_annotation_list", ctx =>
        {
            var drw = ctx.Drawing();
            var (scope, sheet) = Scope(ctx, drw, view);
            var all = Annotations(scope, sheet).Where(a => k is null || a.Item.Kind == k).Select(a => a.Item).ToList();
            return new DrwAnnotationListResult
            {
                Doc = ctx.Title(ctx.Doc),
                Sheet = sheet,
                Count = all.Count,
                Annotations = all.Take(MaxAnnotations).ToList(),
                Truncated = all.Count > MaxAnnotations ? true : null,
            };
        }, RunOptions.Read(doc), ct);
    }

    private static readonly string[] AnnotationKinds =
        ["dimension", "hole_callout", "note", "balloon", "table", "center_mark", "center_line", "datum", "surface_finish", "gtol", "weld", "other"];

    /// <summary>The views to read: one view (its sheet made current), or the current sheet's own view and its model views.</summary>
    private static (List<ViewHit> Views, string Sheet) Scope(SwContext ctx, IDrawingDoc drw, string? view)
    {
        if (!string.IsNullOrWhiteSpace(view))
        {
            var cur = drw.DrwCurrentSheet()?.GetName();
            // The sheet itself by name: its own annotations (notes placed on the sheet).
            if (cur is not null && string.Equals(view.Trim(), cur, StringComparison.OrdinalIgnoreCase) && drw.DrwSheetView() is IView sv)
                return ([new ViewHit(sv, cur, cur)], cur);
            var hit = FindView(ctx, drw, view, "view");
            return ([hit], hit.Sheet);
        }
        string sheet = drw.DrwCurrentSheet()?.GetName() ?? throw new SwException(ErrorCodes.Internal, "The drawing has no current sheet.");
        // AllViews walks the current sheet with GetFirstView / GetNextView: the sheet's own view first, then the model views.
        return (AllViews(drw).Where(h => string.Equals(h.Sheet, sheet, StringComparison.OrdinalIgnoreCase)).ToList(), sheet);
    }

    private static List<Ann> Annotations(IEnumerable<ViewHit> views, string sheet)
    {
        var list = new List<Ann>();
        foreach (var h in views)
        {
            List<IAnnotation> anns;
            try { anns = h.View.DrwAnnotations(); }
            catch (COMException e) when (!ComArgs.IsDisconnect(e)) { continue; }
            foreach (var a in anns) list.Add(new Ann(a, Item(a, h.Name, sheet), h));
        }
        return list;
    }

    /// <summary>Reads one annotation: its kind, the name drw_annotation_edit takes, what it says and where it is.</summary>
    private static DrwAnnotationItem Item(IAnnotation a, string view, string sheet)
    {
        int t = Safe(() => a.GetType(), -1);
        string name = Safe(() => a.GetName(), "") ?? "";
        object? spec = Safe(() => (object?)a.GetSpecificAnnotation(), null);
        string kind;
        string? text = null, unit = null;
        double? value = null;
        switch (t)
        {
            case (int)swAnnotationType_e.swDisplayDimension:
            {
                kind = "dimension";
                if (spec is IDisplayDimension dd)
                {
                    if (Safe(() => dd.IsHoleCallout(), false)) { kind = "hole_callout"; text = CalloutText(dd); }
                    if (Safe(() => dd.GetDimension2(0), null) is IDimension dim)
                    {
                        string full = Safe(() => dim.FullName, "") ?? "";
                        if (full.Length > 0) name = FeatureVerifier.ShortName(full);
                        bool angular = Safe(() => dim.GetType(), 0) == (int)swDimensionParamType_e.swDimensionParamTypeDoubleAngular;
                        double v = Safe(() => dim.SystemValue, double.NaN);
                        if (double.IsFinite(v)) { value = angular ? Units.Deg(v) : Units.Mm(v); unit = angular ? "deg" : "mm"; }
                    }
                }
                break;
            }
            case (int)swAnnotationType_e.swNote:
            {
                kind = "note";
                if (spec is INote n)
                {
                    if (Safe(() => n.IsBomBalloon(), false)) kind = "balloon";
                    text = Safe(() => n.GetText(), null)?.Replace("\r\n", "\n");
                    if (Safe(() => n.GetName(), null) is { Length: > 0 } nn) name = nn;
                }
                break;
            }
            case (int)swAnnotationType_e.swTableAnnotation: kind = "table"; break;
            case (int)swAnnotationType_e.swCenterMarkSym: kind = "center_mark"; break;
            case (int)swAnnotationType_e.swCenterLine: kind = "center_line"; break;
            case (int)swAnnotationType_e.swDatumTag:
                kind = "datum";
                text = spec is IDatumTag tag ? Safe(() => tag.GetLabel(), null) : null;
                break;
            case (int)swAnnotationType_e.swSFSymbol: kind = "surface_finish"; break;
            case (int)swAnnotationType_e.swGTol: kind = "gtol"; break;
            case (int)swAnnotationType_e.swWeldSymbol: kind = "weld"; break;
            default: kind = "other"; break;
        }
        var pos = Safe(() => a.DrwPosition(), []);
        int attached = Safe(() => a.GetAttachedEntityCount3(), -1);
        return new DrwAnnotationItem
        {
            Name = name,
            Kind = kind,
            View = view,
            Sheet = sheet,
            Text = text,
            Value = value,
            Unit = unit,
            AtMm = pos.Length >= 2 ? [Math.Round(pos[0] * 1000, 3), Math.Round(pos[1] * 1000, 3)] : null,
            AttachedTo = attached >= 0 ? attached : null,
        };
    }

    /// <summary>The annotation named <paramref name="name"/> (the listed name, or SOLIDWORKS' own) in the scope; one, or a typed error.</summary>
    private static Ann FindAnnotation(List<Ann> all, string name, string? view, string sheet)
    {
        string n = name.Trim();
        var hits = all.Where(a => string.Equals(a.Item.Name, n, StringComparison.OrdinalIgnoreCase) ||
                                  string.Equals(Safe(() => a.A.GetName(), ""), n, StringComparison.OrdinalIgnoreCase)).ToList();
        if (hits.Count == 1) return hits[0];
        string where = view is null ? $"sheet '{sheet}'" : $"'{view}'";
        if (hits.Count == 0)
            throw new SwException(ErrorCodes.EntityNotFound, $"No annotation named '{n}' in {where}.",
                all.Count == 0 ? "It has no annotations." :
                    $"Annotations: {string.Join(", ", all.Take(12).Select(a => $"{a.Item.Name} ({a.Item.Kind}, {a.Item.View})"))}{(all.Count > 12 ? ", …" : "")}; drw_annotation_list lists them all.");
        throw new SwException(ErrorCodes.InvalidArgument, $"{hits.Count} annotations are named '{n}' in {where} ({string.Join(", ", hits.Select(h => h.Item.View))}).",
            "Pass 'view' to say which.");
    }

    // ================================================================== drw_annotation_edit

    public Task<DrwAnnotationEditResult> AnnotationEditAsync(string? name, string? action, double[]? atMm, string? text, string? view, string? doc,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name)) throw SwException.InvalidArgument("'name' is required: the annotation's name from drw_annotation_list (or from drw_dimension_add / drw_note_add).");
        string act = DrawingLayout.Norm(action) switch
        {
            "move" => "move",
            "delete" or "remove" => "delete",
            "text" or "set_text" => "text",
            null => throw SwException.InvalidArgument("'action' is required: move, delete or text."),
            _ => throw SwException.InvalidArgument($"Unknown action '{action}'.", "Use move (with atMm), delete, or text (a note's new text)."),
        };
        (double X, double Y) to = default;
        if (act == "move") to = Sheet2(atMm, "atMm");
        else if (atMm is not null) throw SwException.InvalidArgument("'atMm' only applies to action 'move'.");
        if (act == "text")
        {
            if (string.IsNullOrWhiteSpace(text)) throw SwException.InvalidArgument("Action 'text' needs the new 'text'.");
            if (text.Length > 4000) throw SwException.InvalidArgument($"'text' is {text.Length} characters long (at most 4000).");
        }
        else if (text is not null) throw SwException.InvalidArgument("'text' only applies to action 'text'.");

        return session.RunAsync("drw_annotation_edit", ctx =>
        {
            var d = ctx.Doc;
            var drw = ctx.Drawing();
            var (scope, sheet) = Scope(ctx, drw, view);
            var all = Annotations(scope, sheet);
            var hit = FindAnnotation(all, name, view, sheet);
            var checks = new List<string>();
            return act switch
            {
                "move" => MoveAnnotation(ctx, d, hit, to, checks),
                "delete" => DeleteOne(ctx, d, drw, hit, view, checks),
                _ => SetNoteText(ctx, d, hit, text!, checks),
            };
        }, RunOptions.Write(doc), ct);
    }

    /// <summary>IAnnotation.SetPosition2 (sheet m); verified by reading the position back, else put back where it was.</summary>
    private static DrwAnnotationEditResult MoveAnnotation(SwContext ctx, IModelDoc2 d, Ann hit, (double X, double Y) to, List<string> checks)
    {
        var old = hit.A.DrwPosition();
        bool set = hit.A.SetPosition2(Units.M(to.X), Units.M(to.Y), 0);
        d.GraphicsRedraw2();
        var pos = hit.A.DrwPosition();
        bool ok = pos.Length >= 2 && Math.Abs(pos[0] - Units.M(to.X)) <= PosTol && Math.Abs(pos[1] - Units.M(to.Y)) <= PosTol;
        if (!ok)
        {
            string back = old.Length >= 2 && hit.A.SetPosition2(old[0], old[1], old.Length > 2 ? old[2] : 0) ? "it was put back" : "it could not be put back";
            throw new SwException(ErrorCodes.VerifyFailed,
                $"'{hit.Item.Name}' reads [{(pos.Length < 2 ? "?" : Fmt([pos[0] * 1000, pos[1] * 1000]))}] mm after the move, not [{Fmt([to.X, to.Y])}] mm" +
                $"{(set ? "" : " (SetPosition2 returned false)")}; {back}.");
        }
        checks.Add("position read back");
        return new DrwAnnotationEditResult
        {
            Doc = ctx.Title(d),
            Action = "move",
            Before = hit.Item,
            After = Item(hit.A, hit.Item.View, hit.Item.Sheet),
            Checks = checks,
        };
    }

    /// <summary>Selects the annotation and deletes it; verified: it is no longer listed and the view holds one fewer.</summary>
    private static DrwAnnotationEditResult DeleteOne(SwContext ctx, IModelDoc2 d, IDrawingDoc drw, Ann hit, string? view, List<string> checks)
    {
        int countBefore = Annotations([hit.Owner], hit.Item.Sheet).Count;
        if (!DeleteAnnotation(d, hit.A))
            throw new SwException(ErrorCodes.FeatureFailed, $"SOLIDWORKS did not delete '{hit.Item.Name}'.",
                "Check that it is not part of the sheet format (title block), which is edited in SOLIDWORKS.");
        d.GraphicsRedraw2();
        var (scope, _) = Scope(ctx, drw, view);
        var after = Annotations(scope, hit.Item.Sheet);
        if (after.Any(a => string.Equals(a.Item.Name, hit.Item.Name, StringComparison.OrdinalIgnoreCase) && a.Item.View == hit.Item.View))
            throw new SwException(ErrorCodes.VerifyFailed, $"'{hit.Item.Name}' is still listed in '{hit.Item.View}' after the delete.");
        int countAfter = after.Count(a => a.Item.View == hit.Item.View);
        if (countAfter == countBefore - 1) checks.Add($"'{hit.Item.View}' went from {countBefore} to {countAfter} annotation(s)");
        else ctx.Warn($"'{hit.Item.View}' went from {countBefore} to {countAfter} annotation(s), not one fewer.");
        checks.Add("no longer listed");
        return new DrwAnnotationEditResult { Doc = ctx.Title(d), Action = "delete", Before = hit.Item, Checks = checks };
    }

    /// <summary>INote.SetText on a note; verified by reading the text back, else the old text is put back.</summary>
    private static DrwAnnotationEditResult SetNoteText(SwContext ctx, IModelDoc2 d, Ann hit, string text, List<string> checks)
    {
        if (hit.Item.Kind != "note" || Safe(() => (object?)hit.A.GetSpecificAnnotation(), null) is not INote note)
            throw SwException.InvalidArgument($"'{hit.Item.Name}' is a {hit.Item.Kind.Replace('_', ' ')}; action 'text' changes notes only.",
                hit.Item.Kind is "dimension" or "hole_callout" ? "A drawing dimension's value follows the model; tolerances: dimension_tolerance_set." : null);
        string old = note.GetText() ?? "";
        string want = text.Replace("\r\n", "\n");
        bool set = note.SetText(want.Replace("\n", "\r\n"));
        d.GraphicsRedraw2();
        string got = (note.GetText() ?? "").Replace("\r\n", "\n");
        if (got != want)
        {
            if (want.Contains('$') || want.Contains('<'))
                ctx.Warn($"The note reads '{Clip(got, 80)}' (links and formatting are evaluated).");
            else
            {
                string back = note.SetText(old) ? "the old text was put back" : "the old text could not be put back";
                throw new SwException(ErrorCodes.VerifyFailed, $"The note reads '{Clip(got, 80)}' after the change{(set ? "" : " (SetText returned false)")}; {back}.");
            }
        }
        else checks.Add("text read back");
        return new DrwAnnotationEditResult
        {
            Doc = ctx.Title(d),
            Action = "text",
            Before = hit.Item,
            After = Item(hit.A, hit.Item.View, hit.Item.Sheet),
            Checks = checks,
        };
    }

    // ================================================================== drw_view_edit kind=move

    /// <summary>
    /// Sets IView.Position (the view's centre, sheet m). Verified: the position read back; a view that SOLIDWORKS keeps
    /// elsewhere (aligned with its parent) is put back and refused. Views that moved with it (aligned children) are named.
    /// </summary>
    private static DrwViewEditResult MoveView(SwContext ctx, DrwViewEditRequest q)
    {
        var d = ctx.Doc;
        var drw = ctx.Drawing();
        var hit = FindView(ctx, drw, q.View, "view");
        if (hit.View.Type == (int)swDrawingViewTypes_e.swDrawingSheet)
            throw SwException.InvalidArgument($"'{hit.Name}' is a sheet, not a view.");
        var to = Sheet2(q.AtMm, "atMm");
        var sheet = SheetOf(drw, hit);
        var others = ModelViews(drw, currentSheetOnly: true).Where(h => h.Name != hit.Name)
            .Select(h => (h.Name, h.View, Pos: h.View.DrwPosition())).ToList();
        var before = Outline(hit);
        var old = hit.View.DrwPosition();
        hit.View.DrwMoveTo(Units.M(to.X), Units.M(to.Y));
        d.GraphicsRedraw2();
        var pos = hit.View.DrwPosition();
        bool ok = pos.Length >= 2 && Math.Abs(pos[0] - Units.M(to.X)) <= PosTol && Math.Abs(pos[1] - Units.M(to.Y)) <= PosTol;
        if (!ok)
        {
            string parent = hit.View.DrwBaseView()?.GetName2() is { Length: > 0 } p ? $" It may be aligned with '{p}': move it along that alignment, or move '{p}'." : "";
            if (old.Length >= 2) hit.View.DrwMoveTo(old[0], old[1]);
            throw new SwException(ErrorCodes.VerifyFailed,
                $"'{hit.Name}' sits at [{(pos.Length < 2 ? "?" : Fmt([pos[0] * 1000, pos[1] * 1000]))}] mm after the move, not [{Fmt([to.X, to.Y])}] mm; it was put back.",
                parent.Length > 0 ? parent.Trim() : null);
        }
        var checks = new List<string> { "position read back" };
        var moved = new List<string>();
        foreach (var o in others)
        {
            var now = Safe(() => o.View.DrwPosition(), []);
            if (o.Pos.Length >= 2 && now.Length >= 2 && (Math.Abs(now[0] - o.Pos[0]) > PosTol || Math.Abs(now[1] - o.Pos[1]) > PosTol))
                moved.Add(o.Name);
        }
        if (moved.Count > 0) checks.Add($"moved with it (aligned): {string.Join(", ", moved)}");
        OverlapCheck(ctx, sheet, [hit.View, .. others.Where(o => moved.Contains(o.Name)).Select(o => o.View)], checks);
        return new DrwViewEditResult
        {
            Doc = ctx.Title(d),
            Kind = "move",
            View = ViewInfo(ctx, hit.View, false),
            BoundsBeforeMm = before.ToArray(),
            AlsoMoved = moved.Count > 0 ? moved : null,
            Checks = checks,
        };
    }

    // ================================================================== drw_view_delete

    public Task<DrwViewDeleteResult> ViewDeleteAsync(string? view, string? doc, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(view)) throw SwException.InvalidArgument("'view' is required (drw_info lists the views).");
        return session.RunAsync("drw_view_delete", ctx =>
        {
            var d = ctx.Doc;
            var drw = ctx.Drawing();
            var hit = FindView(ctx, drw, view, "view");
            if (hit.View.Type == (int)swDrawingViewTypes_e.swDrawingSheet)
                throw SwException.InvalidArgument($"'{hit.Name}' is a sheet, not a view; sheets are not deleted by this tool.");
            var counts = Count(hit.View);
            var beforeNames = ModelViews(drw).Select(h => h.Name).ToList();
            var children = ModelViews(drw).Where(h => h.Name != hit.Name && string.Equals(h.View.DrwBaseView()?.GetName2(), hit.Name, StringComparison.OrdinalIgnoreCase))
                .Select(h => h.Name).ToList();
            d.ClearSelection2(true);
            if (!d.Extension.SelectByID2(hit.Name, "DRAWINGVIEW", 0, 0, 0, false, 0, null, 0))
                throw new SwException(ErrorCodes.FeatureFailed, $"Could not select view '{hit.Name}'.");
            bool deleted = d.Extension.DeleteSelection2(0);
            d.ClearSelection2(true);
            d.GraphicsRedraw2();
            var afterNames = ModelViews(drw).Select(h => h.Name).ToList();
            if (afterNames.Contains(hit.Name, StringComparer.OrdinalIgnoreCase))
                throw new SwException(ErrorCodes.FeatureFailed, $"View '{hit.Name}' is still there{(deleted ? "" : " (DeleteSelection2 returned false)")}.");
            var removed = beforeNames.Where(n => !afterNames.Contains(n, StringComparer.OrdinalIgnoreCase)).ToList();
            var checks = new List<string> { $"'{hit.Name}' no longer listed", $"{afterNames.Count} view(s) left" };
            var others = removed.Where(n => !string.Equals(n, hit.Name, StringComparison.OrdinalIgnoreCase)).ToList();
            if (others.Count > 0) ctx.Warn($"SOLIDWORKS also removed {string.Join(", ", others)}.");
            var kept = children.Where(c => afterNames.Contains(c, StringComparer.OrdinalIgnoreCase)).ToList();
            if (kept.Count > 0) ctx.Warn($"{string.Join(", ", kept)} had '{hit.Name}' as parent and stay{(kept.Count == 1 ? "s" : "")}.");
            return new DrwViewDeleteResult
            {
                Doc = ctx.Title(d),
                View = hit.Name,
                Removed = removed,
                Annotations = counts,
                ChildrenKept = kept.Count > 0 ? kept : null,
                ViewCount = afterNames.Count,
                Checks = checks,
            };
        }, RunOptions.Write(doc), ct);
    }
}
