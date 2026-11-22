using SolidWorksMcp.Core.Services;
using Xunit;

namespace SolidWorksMcp.UnitTests;

/// <summary>
/// equation_list's and equation_set's read-back check (S92 a, b): a global variable set to a plain number that SOLIDWORKS
/// reads back as another value is the sign of the state in which it refuses equations until it is restarted.
/// </summary>
public class EquationReadbackTests
{
    private const double Lin = 1e-3, Ang = Math.PI / 180; // MMGS

    private static (string Lhs, string Text, bool Global, double? Value) Row(string lhs, string rhs, double? value, bool global = true) =>
        (lhs, $"\"{lhs}\" = {rhs}", global, value);

    [Fact]
    public void Globals_read_back_as_written_show_no_sign()
    {
        // The probe's good state: 60.4, 2.5, a row from another, a dimension's equation.
        var rows = new[]
        {
            Row("g", "60.4mm", 60.4), Row("k", "2.5mm", 2.5), Row("h", "\"g\" / 2", 30.2), Row("n", "12", 12),
            Row("a", "30deg", 30), Row("t", "1in", 25.4), Row("D1@Boss-Extrude1", "\"k\" * 8", 20, global: false),
        };
        Assert.Empty(EquationReadback.Misreads(rows, Lin, Ang));
    }

    [Fact]
    public void A_value_read_in_SI_agrees_too()
    {
        Assert.Empty(EquationReadback.Misreads([Row("g", "60.4mm", 0.0604), Row("a", "30deg", Math.PI / 6)], Lin, Ang));
    }

    [Fact]
    public void Numbers_cut_at_their_decimal_point_are_the_sign_and_name_each_global()
    {
        // The sessions' state: 60.4mm read as 60, 2.5mm as 2, 0.5mm as 0, and every row from others 0.
        var rows = new[]
        {
            Row("g", "60.4mm", 60), Row("k", "2.5mm", 2), Row("s", "0.5mm", 0), Row("h", "\"g\" / 2", 0),
            Row("W", "100mm", 100), Row("D1@Sketch1", "59.614mm", 59, global: false),
        };
        var m = EquationReadback.Misreads(rows, Lin, Ang);
        Assert.Equal(["g", "k", "s"], m.Select(x => x.Lhs));
        Assert.Equal(60, m[0].Read);
        string sign = EquationReadback.Sign(m);
        Assert.Contains("\"g\" = 60.4mm as 60", sign);
        Assert.Contains("\"s\" = 0.5mm as 0", sign);
        Assert.Contains("until it is restarted", sign);
    }

    [Fact]
    public void A_global_that_is_not_a_plain_number_or_reads_no_value_is_not_judged()
    {
        var rows = new[] { Row("h", "\"g\" / 2", 0), Row("q", "60.4mm", null), Row("f", "3furlong", 1), Row("c", "60.4mm 'note", 60) };
        Assert.Empty(EquationReadback.Misreads(rows, Lin, Ang));
    }

    [Fact]
    public void The_list_warning_says_its_values_are_not_to_be_trusted_and_names_the_restart()
    {
        var m = EquationReadback.Misreads([Row("g", "60.4mm", 60)], Lin, Ang);
        string w = EquationReadback.ListWarning(m);
        Assert.Contains("\"g\" = 60.4mm as 60", w);
        Assert.Contains("not to be trusted", w);
        Assert.Contains("restart SOLIDWORKS", w);
        Assert.Contains("restart SOLIDWORKS", EquationReadback.SetWarning(m));
    }

    [Fact]
    public void A_verify_failure_names_the_state_in_place_of_the_expression_while_the_sign_shows()
    {
        const string what = "equation_set \"W\" = 73.5mm: SOLIDWORKS kept the previous equation \"W\" = 100mm and did not take 73.5mm";
        var plain = EquationReadback.VerifyFailed(what, "Nothing was changed.", []);
        Assert.Equal(what + ". Nothing was changed.", plain.Message);
        Assert.Contains("Check the expression", plain.Hint);

        var m = EquationReadback.Misreads([Row("g", "60.4mm", 60)], Lin, Ang);
        var state = EquationReadback.VerifyFailed(what, "Nothing was changed.", m);
        Assert.StartsWith(what + ". ", state.Message);
        Assert.Contains("\"g\" = 60.4mm as 60", state.Message);
        Assert.EndsWith("Nothing was changed.", state.Message);
        Assert.DoesNotContain("Check the expression", state.Hint);
        Assert.Contains("restart SOLIDWORKS", state.Hint);
    }

    [Fact]
    public void The_restart_message_names_the_process_start_time_and_the_hint_the_window_that_leaves_it_alive()
    {
        // S92 (d): "restarted?" is read from sw_status solidWorksStarted; closing the window left the process alive.
        var started = new DateTime(2026, 11, 22, 9, 5, 3);
        var m = EquationReadback.Misreads([Row("g", "60.4mm", 60)], Lin, Ang);
        var state = EquationReadback.VerifyFailed("equation_set \"W\" = 73.5mm: SOLIDWORKS kept it", "Nothing was changed.", m, started, [4120, 5588]);
        Assert.Contains("started 2026-11-22 09:05:03", state.Message);
        Assert.Contains("solidWorksStarted", state.Message);
        Assert.EndsWith("Nothing was changed.", state.Message);
        Assert.Contains("closing the window can leave the process alive", state.Hint, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("otherServerPids", state.Hint);
        Assert.Contains("(4120, 5588)", state.Hint);
        Assert.Contains("restart SOLIDWORKS", state.Hint);

        Assert.Contains("started 2026-11-22 09:05:03", EquationReadback.ListWarning(m, started));
        Assert.Contains("started 2026-11-22 09:05:03", EquationReadback.SetWarning(m, started));

        // No start time to read, no other server process: the clause is left out, the hint still says it.
        Assert.DoesNotContain("started 20", EquationReadback.ListWarning(m));
        Assert.DoesNotContain("(", EquationReadback.RestartHintFor(null).Replace("(sw_status", ""));
    }

    [Fact]
    public void The_kept_previous_text_guesses_at_the_value_only_without_the_sign()
    {
        Assert.Contains("such as 0?", EquationReadback.KeptPrevious("\"W\" = 100mm", "73.5mm", stateShown: false));
        string inState = EquationReadback.KeptPrevious("\"W\" = 100mm", "73.5mm", stateShown: true);
        Assert.Equal("SOLIDWORKS kept the previous equation \"W\" = 100mm and did not take 73.5mm", inState);
    }

    // S118: in the state a whole number reads right, so a document whose globals are all whole numbers shows S112's sign
    // nowhere; every row defined from them reads 0 (ten globals, thirteen dimension rows, in the scenario test).
    private static (string Lhs, string Text, bool Global, double? Value)[] WholeNumberGlobals(double derived) =>
    [
        Row("W", "120mm", 120), Row("H", "80mm", 80), Row("T", "6mm", 6),
        Row("D1@Sketch1", "\"W\"", derived == 0 ? 0 : 120, global: false),
        Row("D2@Sketch1", "\"H\" - 2 * \"T\"", derived == 0 ? 0 : 68, global: false),
        Row("D1@Boss-Extrude1", "\"T\"", derived == 0 ? 0 : 6, global: false),
        Row("half", "\"W\" / 2", derived == 0 ? 0 : 60),
    ];

    [Fact]
    public void Every_row_from_non_zero_globals_read_0_is_the_sign_when_the_globals_are_whole_numbers()
    {
        var m = EquationReadback.Misreads(WholeNumberGlobals(0), Lin, Ang);
        Assert.Equal(["D1@Sketch1", "D2@Sketch1", "D1@Boss-Extrude1", "half"], m.Select(x => x.Lhs));
        Assert.All(m, x => Assert.True(x.FromOthers));
        string sign = EquationReadback.Sign(m);
        Assert.Contains("all 4 rows defined from globals that read non-zero as 0", sign);
        Assert.Contains("D1@Sketch1", sign);
        Assert.Contains("until it is restarted", sign);

        // The batch of the scenario test, every global refused: the state and the restart, not the expression.
        const string what = "equation_set, 3 of 3 equations: \"W\" = 140mm: SOLIDWORKS kept the previous equation \"W\" = 120mm";
        var state = EquationReadback.VerifyFailed(what, "Nothing was changed.", m);
        Assert.Contains("the expression is likely not at fault", state.Message);
        Assert.DoesNotContain("Check the expression", state.Hint);
        Assert.Contains("restart SOLIDWORKS", state.Hint);
        Assert.Contains("all 4 rows defined from globals", EquationReadback.ListWarning(m));
    }

    [Fact]
    public void The_same_rows_read_right_show_no_sign()
    {
        Assert.Empty(EquationReadback.Misreads(WholeNumberGlobals(1), Lin, Ang));
    }

    [Fact]
    public void A_row_that_is_0_beside_rows_that_are_not_shows_no_sign()
    {
        // A healthy part: "g" - "h" is 0 when the two are equal; the other row from them reads its value.
        var rows = new[]
        {
            Row("g", "60mm", 60), Row("h", "60mm", 60), Row("gap", "\"g\" - \"h\"", 0),
            Row("D1@Sketch1", "\"g\" + \"h\"", 120, global: false),
        };
        Assert.Empty(EquationReadback.Misreads(rows, Lin, Ang));
    }

    [Fact]
    public void A_single_row_read_0_or_rows_over_a_global_of_0_are_not_judged()
    {
        // One row alone can be 0 on a healthy part; a row over a global of 0 (or over a name with no row) can be 0 too.
        Assert.Empty(EquationReadback.Misreads([Row("g", "60mm", 60), Row("h", "60mm", 60), Row("gap", "\"g\" - \"h\"", 0)], Lin, Ang));
        var rows = new[]
        {
            Row("n", "0", 0), Row("W", "100mm", 100),
            Row("D1@Sketch1", "\"W\" * \"n\"", 0, global: false), Row("D2@Sketch1", "\"n\"", 0, global: false),
            Row("D3@Sketch1", "\"X\" * 2", 0, global: false), Row("D4@Sketch1", "\"D1@Sketch2\" * 2", 0, global: false),
        };
        Assert.Empty(EquationReadback.Misreads(rows, Lin, Ang));
    }

    [Fact]
    public void Literals_parse_as_equation_set_checks_them()
    {
        Assert.Equal((60.4, "mm"), EquationReadback.Literal("60.4mm"));
        Assert.Equal((12.0, ""), EquationReadback.Literal("12"));
        Assert.Null(EquationReadback.Literal("\"W\" / 2"));
        Assert.Null(EquationReadback.Literal("60,4mm"));
        Assert.Equal((0.0254, false), EquationReadback.UnitFactor("in"));
        Assert.Equal("60.4mm", EquationReadback.Rhs("\"g\" = 60.4mm"));
        Assert.Equal("60.4mm", EquationReadback.Rhs("\"g\"=60.4mm"));
        Assert.Null(EquationReadback.Rhs("no equation"));
    }
}
