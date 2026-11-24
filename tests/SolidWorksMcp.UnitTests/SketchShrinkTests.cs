using SolidWorksMcp.Core.Services;
using Xunit;

namespace SolidWorksMcp.UnitTests;

/// <summary>
/// S82 (a): the warning for a dimension after which a sketch line is less than half as long as before. The lengths are those
/// read live for a trapezoid whose sides an angle taken across the vertex turned to lean in: its top 133.508 to 6.492 mm.
/// </summary>
public class SketchShrinkTests
{
    private static LineLength Mm(string name, double before, double after) => new(name, before / 1000, after / 1000);

    private static readonly LineLength[] Trapezoid =
    [
        Mm("Line2", 3, 3.0000001), Mm("Line3", 60.508397, 60.508397), Mm("Line4", 133.508397, 6.491603), Mm("Line5", 63.508397, 63.508397),
    ];

    [Fact]
    public void An_angle_that_collapses_a_line_names_it_with_both_lengths_and_the_way_back()
    {
        string? w = SketchShrink.Warning("angular dimension", Trapezoid, 0, "dimension_set 'D1@Sketch1' back to 119.999793");
        Assert.NotNull(w);
        Assert.StartsWith("After the angular dimension, a line is less than half as long as before: 'Line4' 133.508 to 6.492 mm.", w);
        Assert.EndsWith("if that was not meant, dimension_set 'D1@Sketch1' back to 119.999793.", w);
        Assert.DoesNotContain("Line3", w);
    }

    [Fact]
    public void A_line_that_keeps_half_its_length_or_grows_is_not_named()
    {
        Assert.Null(SketchShrink.Warning("angular dimension", [Mm("Line1", 100, 50), Mm("Line2", 6.5, 133.5)], 0, "x"));
        Assert.NotNull(SketchShrink.Warning("angular dimension", [Mm("Line1", 100, 49.9)], 0, "x"));
    }

    [Fact]
    public void A_length_dimension_explains_a_drop_up_to_twice_its_change()
    {
        // A rectangle 100 wide set to 40: its top and bottom go 100 to 40, as asked.
        Assert.Null(SketchShrink.Warning("linear dimension", [Mm("Line1", 100, 40), Mm("Line3", 100, 40)], 0.060, "x"));
        // A 5 mm change that took a line from 100 to 10: more than the value explains.
        string? w = SketchShrink.Warning("linear dimension", [Mm("Line1", 100, 10)], 0.005, "x");
        Assert.Contains("'Line1' 100 to 10 mm", w);
    }

    [Fact]
    public void A_line_too_short_to_measure_is_left_out_and_many_are_counted()
    {
        Assert.Null(SketchShrink.Warning("angular dimension", [Mm("Line1", 0.0005, 0.0001)], 0, "x"));
        var many = Enumerable.Range(1, 5).Select(i => Mm($"Line{i}", 100 + i, 1)).ToList();
        string? w = SketchShrink.Warning("dimension_set of 'D1@Sketch1'", many, 0, "x");
        Assert.NotNull(w);
        Assert.StartsWith("After the dimension_set of 'D1@Sketch1', lines are less than half as long as before: 'Line5' 105 to 1 mm, ", w);
        Assert.Contains(" and 2 more.", w);
    }
}
