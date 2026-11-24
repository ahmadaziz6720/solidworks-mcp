using SolidWorksMcp.Core.Services;
using Xunit;

namespace SolidWorksMcp.UnitTests;

/// <summary>S86 (a): the warning that sketch_delete took a segment that was not named (a polygon's construction circle).</summary>
public class SketchRemovedTests
{
    [Fact]
    public void A_segment_that_was_not_named_is_named_with_its_relations_and_dimensions()
    {
        string? w = SketchRemoved.Warning(
            ["Arc1 (construction circle, diameter 24 mm)"],
            ["equal: Arc1, Arc2", "coincident: Arc1 centre, origin"],
            ["D1@Sketch1 = 24 mm", "D2@Sketch1 = 5 mm"]);
        Assert.NotNull(w);
        Assert.StartsWith("sketch_delete also removed a segment not named in the call: Arc1 (construction circle, diameter 24 mm).", w);
        Assert.Contains("Relations that tied it went too: equal: Arc1, Arc2; coincident: Arc1 centre, origin.", w);
        Assert.Contains("The sketch lost dimension(s) in the call: D1@Sketch1 = 24 mm; D2@Sketch1 = 5 mm.", w);
        Assert.EndsWith("sketch_info shows what is left.", w);
    }

    [Fact]
    public void Nothing_unasked_is_no_warning_and_nothing_tied_is_left_out()
    {
        Assert.Null(SketchRemoved.Warning([], ["equal: Arc1, Arc2"], ["D1@Sketch1 = 24 mm"]));
        string w = SketchRemoved.Warning(["Arc1 (circle)", "Arc2 (circle)"], [], [])!;
        Assert.Contains("2 segments not named in the call", w, StringComparison.Ordinal);
        Assert.DoesNotContain("Relations", w, StringComparison.Ordinal);
        Assert.DoesNotContain("dimension(s)", w, StringComparison.Ordinal);
    }

    [Fact]
    public void A_long_list_is_cut_after_four()
    {
        string w = SketchRemoved.Warning(["Arc1 (circle)"], ["r1", "r2", "r3", "r4", "r5", "r6"], [])!;
        Assert.Contains("r4; and 2 more", w, StringComparison.Ordinal);
        Assert.DoesNotContain("r5;", w, StringComparison.Ordinal);
    }
}
