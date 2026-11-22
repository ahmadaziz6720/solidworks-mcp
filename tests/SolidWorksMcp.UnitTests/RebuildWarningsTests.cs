using SolidWorksMcp.Core.Verify;
using Xunit;

namespace SolidWorksMcp.UnitTests;

/// <summary>
/// S84 (b): the rebuild warnings a feature tool reports are read after the call's rename and a rebuild, named as the
/// features are called now; a feature taken back since, or whose warning the rebuild cleared, is not reported.
/// </summary>
public class RebuildWarningsTests
{
    private static RebuildState Warn(string name) => new(name, 51, true, "swSketchErrorExtRefFail");

    [Fact]
    public void A_warning_is_named_by_the_name_the_feature_has_now()
    {
        // Made as LPattern2, renamed to Row by the call: the tree now reads Row.
        var lines = RebuildWarnings.Lines([7], new Dictionary<int, RebuildState> { [7] = Warn("Row") });
        Assert.Equal(["'Row' has rebuild warning 51 (swSketchErrorExtRefFail)."], lines);
    }

    [Fact]
    public void A_feature_taken_back_or_cleared_by_the_rebuild_is_left_out()
    {
        // 5: the pattern made in SOLIDWORKS' own sense, deleted and made again flipped (no longer in the tree);
        // 6: the one kept, its warning gone after the rebuild (not in the map, or read as 0).
        Assert.Empty(RebuildWarnings.Lines([5, 6], new Dictionary<int, RebuildState>()));
        Assert.Empty(RebuildWarnings.Lines([6], new Dictionary<int, RebuildState> { [6] = new("Row", 0, false, "0") }));
        // Only the one still in the tree with its warning is said, once.
        var lines = RebuildWarnings.Lines([5, 6, 6], new Dictionary<int, RebuildState> { [6] = Warn("Row"), [9] = Warn("Other") });
        Assert.Equal(["'Row' has rebuild warning 51 (swSketchErrorExtRefFail)."], lines);
    }

    [Fact]
    public void A_warning_that_became_an_error_in_the_rebuild_says_so()
    {
        var lines = RebuildWarnings.Lines([3], new Dictionary<int, RebuildState> { [3] = new("Row", 51, false, "swSketchErrorExtRefFail") });
        Assert.Equal(["'Row' has rebuild error 51 (swSketchErrorExtRefFail) after the rebuild; it had a warning when made."], lines);
    }
}
