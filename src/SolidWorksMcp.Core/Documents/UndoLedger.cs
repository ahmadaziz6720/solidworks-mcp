namespace SolidWorksMcp.Core.Documents;

/// <summary>
/// doc_undo's record of the server's writes in one document, interop-free. A counted write is a SOLIDWORKS undo step; a
/// change that left none (equation_set, prop_set, feature_rename, ...) is kept with the step that follows it, so an undo
/// that takes that step back puts the change on record again (S79 b, S114 a, S117): EditUndo2 would pass over it next.
/// </summary>
internal sealed class UndoLedger
{
    /// <summary>A counted write: its tool, and the server's changes before it that left no undo step.</summary>
    internal sealed record Step(string Tool, IReadOnlyList<string> NoUndoBefore);

    /// <summary>The server's changes since its last counted write (or its last step undone) that left no undo step.</summary>
    public List<string> NoUndoChanges { get; } = [];

    /// <summary>The server's counted writes, oldest first, the last one what EditUndo2 takes back next. Kept to <see cref="MaxSteps"/>.</summary>
    public List<Step> Steps { get; } = [];

    public const int MaxSteps = 64;

    /// <summary>Someone else changed the document: nothing before now is the server's to undo.</summary>
    public void Clear()
    {
        NoUndoChanges.Clear();
        Steps.Clear();
    }

    /// <summary>A change by <paramref name="tool"/> that left no SOLIDWORKS undo step.</summary>
    public void NoUndo(string tool)
    {
        if (!NoUndoChanges.Contains(tool)) NoUndoChanges.Add(tool);
    }

    /// <summary>A write by <paramref name="tool"/> that is an undo step; the changes on record go with it.</summary>
    public void Counted(string tool)
    {
        Steps.Add(new Step(tool, [.. NoUndoChanges]));
        NoUndoChanges.Clear();
        if (Steps.Count > MaxSteps) Steps.RemoveAt(0);
    }

    /// <summary>
    /// The changes with no undo step an undo of <paramref name="steps"/> would pass over, and how many steps it could take
    /// back before them (0: they are the server's last change); null when it passes over none.
    /// </summary>
    public (IReadOnlyList<string> Changes, int Before)? PassedOver(int steps)
    {
        if (NoUndoChanges.Count > 0) return (NoUndoChanges.ToList(), 0);
        for (int i = 1; i < steps && i <= Steps.Count; i++)
            if (Steps[^i].NoUndoBefore.Count > 0) return (Steps[^i].NoUndoBefore, i);
        return null;
    }

    /// <summary>The server's step EditUndo2 would take back after <paramref name="before"/> steps; null when none is kept.</summary>
    public string? StepAfter(int before) => before < Steps.Count ? Steps[^(before + 1)].Tool : null;

    /// <summary>doc_undo took back <paramref name="steps"/>: the changes before the last one taken back are on record again.</summary>
    public void Undone(int steps)
    {
        NoUndoChanges.Clear();
        if (steps <= Steps.Count) NoUndoChanges.AddRange(Steps[^steps].NoUndoBefore);
        Steps.RemoveRange(Math.Max(0, Steps.Count - steps), Math.Min(steps, Steps.Count));
    }
}
