using SolidWorksMcp.Core.Documents;
using Xunit;

namespace SolidWorksMcp.UnitTests;

/// <summary>
/// doc_undo's record of the server's steps (S79 b, S114 a, S117): a change that left no SOLIDWORKS undo step is refused
/// while it is the server's last, and again once an undo has taken back the step after it.
/// </summary>
public class UndoLedgerTests
{
    private static UndoLedger Ledger(params string[] calls)
    {
        var l = new UndoLedger();
        foreach (var c in calls)
            if (c.StartsWith('~')) l.NoUndo(c[1..]);
            else l.Counted(c);
        return l;
    }

    private static void Passes(UndoLedger l, int steps, string[] changes, int before)
    {
        var p = l.PassedOver(steps);
        Assert.NotNull(p);
        Assert.Equal(changes, p.Value.Changes);
        Assert.Equal(before, p.Value.Before);
    }

    [Fact]
    public void A_change_with_no_undo_step_is_refused_while_it_is_the_last()
    {
        var l = Ledger("feature_extrude", "~equation_set", "~prop_set");
        Passes(l, 1, ["equation_set", "prop_set"], 0);
        Assert.Equal("feature_extrude", l.StepAfter(0));
    }

    [Fact]
    public void A_counted_step_after_it_lifts_the_refusal()
    {
        var l = Ledger("feature_extrude", "~feature_rename", "feature_suppress_set");
        Assert.Null(l.PassedOver(1));
        Assert.Empty(l.NoUndoChanges);
    }

    [Fact]
    public void An_undo_past_the_step_after_it_puts_it_on_record_again()
    {
        // S117: the suppress undone, the next undo would take back the boss and leave the rename.
        var l = Ledger("feature_extrude", "feature_extrude", "~feature_rename", "feature_suppress_set");
        l.Undone(1);
        Passes(l, 1, ["feature_rename"], 0);
        Assert.Equal("feature_extrude", l.StepAfter(0));
    }

    [Fact]
    public void An_undo_of_several_steps_is_refused_where_it_would_pass_over_one()
    {
        var l = Ledger("feature_extrude", "~feature_rename", "dimension_set", "feature_suppress_set");
        Assert.Null(l.PassedOver(2));
        Passes(l, 3, ["feature_rename"], 2);
        Assert.Equal("feature_extrude", l.StepAfter(2));
        l.Undone(2);
        Assert.Equal(["feature_rename"], l.NoUndoChanges);
        Assert.Equal(["feature_extrude"], l.Steps.Select(s => s.Tool));
    }

    [Fact]
    public void Steps_past_the_kept_ones_and_a_clear_leave_nothing_on_record()
    {
        var l = Ledger("~prop_set", "feature_extrude");
        Assert.Null(l.StepAfter(1));
        l.Undone(2); // more than are kept: nothing is known before them
        Assert.Empty(l.NoUndoChanges);
        Assert.Empty(l.Steps);

        l = Ledger("feature_extrude", "~prop_set");
        l.Clear();
        Assert.Null(l.PassedOver(1));
    }

    [Fact]
    public void Only_the_last_steps_are_kept()
    {
        var l = new UndoLedger();
        for (int i = 0; i < UndoLedger.MaxSteps + 3; i++) l.Counted($"t{i}");
        Assert.Equal(UndoLedger.MaxSteps, l.Steps.Count);
        Assert.Equal("t3", l.Steps[0].Tool);
    }
}
