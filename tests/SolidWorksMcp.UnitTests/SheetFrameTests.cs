using SolidWorksMcp.Core.Geometry;
using SolidWorksMcp.Core.Services;
using Xunit;

namespace SolidWorksMcp.UnitTests;

/// <summary>A sheet's frame from its format's lines, text boxes, and a view's axes on the sheet (S14).</summary>
public class SheetFrameTests
{
    /// <summary>Lines of SOLIDWORKS 2023's ISO A3 landscape format, as IView.GetLines4 gives them on the sheet's view (mm).</summary>
    private static readonly SheetLine[] IsoA3 =
    [
        // the title block
        new(230, 14.25, 410, 14.25), new(410, 48.25, 230, 48.25), new(230, 65.25, 410, 65.25), new(256.99, 48.25, 256.99, 10),
        new(328.8, 31.25, 230, 31.25), new(284.6, 10, 284.6, 48.25), new(230, 65.25, 230, 10), new(328.8, 65.25, 328.8, 10),
        new(373, 10, 373, 14.25), new(410, 58.88, 328.8, 58.88), new(397, 14.25, 397, 27), new(410, 18.5, 397, 18.5),
        // the border (drawable area) and the zone strip's outer frame
        new(20, 10, 20, 287), new(20, 287, 410, 287), new(410, 287, 410, 10), new(410, 10, 20, 10),
        new(15, 5, 15, 292), new(15, 292, 415, 292), new(415, 292, 415, 5), new(415, 5, 15, 5),
        // centring marks reaching 5 mm into the border, zone ticks in the strip
        new(15, 148.5, 25, 148.5), new(215, 292, 215, 282), new(415, 148.5, 405, 148.5), new(215, 5, 215, 15),
        new(20, 48.5, 15, 48.5), new(65, 287, 65, 292), new(65, 10, 65, 5),
    ];

    private static readonly double[] IsoMargins = [10, 10, 10, 20]; // top, bottom, right, left

    [Fact]
    public void The_iso_a3_format_gives_its_inner_border_and_title_block()
    {
        var f = SheetFrame.From(420, 297, true, IsoA3, IsoMargins);
        Assert.Equal(new SheetRect(20, 10, 410, 287), f.Border);
        Assert.Equal(new SheetRect(230, 10, 410, 65.25), f.TitleBlock);
        Assert.Single(f.Blocks);
    }

    [Fact]
    public void A_format_whose_lines_close_no_border_takes_its_zone_margins()
    {
        var f = SheetFrame.From(420, 297, true, IsoA3.Take(12).ToList(), IsoMargins);
        Assert.Equal(new SheetRect(20, 10, 410, 287), f.Border);
        Assert.Equal(new SheetRect(230, 10, 410, 65.25), f.TitleBlock);
    }

    [Fact]
    public void A_sheet_without_a_format_has_no_border()
    {
        var f = SheetFrame.From(420, 297, false, [], IsoMargins);
        Assert.Null(f.Border);
        Assert.Null(f.TitleBlock);
        Assert.Empty(f.Clashes(new SheetRect(5, 5, 50, 50)));
        Assert.Contains(f.Clashes(new SheetRect(400, 5, 430, 50)), c => c.Contains("runs off the sheet"));
    }

    [Fact]
    public void Clashes_name_the_sheet_edge_the_border_and_the_title_block()
    {
        var f = SheetFrame.From(420, 297, true, IsoA3, IsoMargins);
        Assert.Empty(f.Clashes(new SheetRect(30, 80, 200, 200)));
        // a note's line run past the page (findings: a note 120 mm from the edge with a 180 mm line)
        Assert.Contains(f.Clashes(new SheetRect(300, 150, 480, 155)), c => c.Contains("runs off the sheet"));
        // a note over the left zone letters at x ≈ 19 (the border is at 20)
        Assert.Contains(f.Clashes(new SheetRect(18, 100, 60, 105)), c => c.Contains("crosses the format's border"));
        // a dimension's text over the top zone strip (y 292 on a 297 sheet)
        Assert.Contains(f.Clashes(new SheetRect(200, 289, 215, 293)), c => c.Contains("crosses the format's border"));
        Assert.Contains(f.Clashes(new SheetRect(300, 40, 340, 50)), c => c.Contains("overlaps the title block"));
        // touching the title block's top edge is not on it
        Assert.Empty(f.Clashes(new SheetRect(300, 65.25, 340, 80)));
    }

    [Fact]
    public void A_second_block_inside_the_border_is_kept_out_too()
    {
        var revision = new SheetLine[] { new(330, 270, 410, 270), new(330, 270, 330, 287), new(370, 270, 370, 287) };
        var f = SheetFrame.From(420, 297, true, [.. IsoA3, .. revision], IsoMargins);
        Assert.Equal(new SheetRect(230, 10, 410, 65.25), f.TitleBlock);
        Assert.Equal(2, f.Blocks.Count);
        Assert.Equal(new SheetRect(330, 270, 410, 287), f.Blocks[1]);
        Assert.Contains(f.Clashes(new SheetRect(340, 275, 360, 280)), c => c.Contains("overlaps a block of the format"));
    }

    [Theory]
    [InlineData(1, 0.0, 206.6, 235.7, 223.4, 239.2)]   // lower left: the text runs right and up from the point
    [InlineData(2, 0.0, 198.2, 233.95, 215.0, 237.45)] // centre
    [InlineData(0, 0.0, 206.6, 232.2, 223.4, 235.7)]   // upper left
    public void A_text_box_hangs_from_its_reference_corner(int refPos, double angle, double x0, double y0, double x1, double y1)
    {
        var r = SheetFrame.TextBox(206.6, 235.7, 16.8, 3.5, refPos, angle);
        Assert.Equal(x0, r.X0, 1e-9);
        Assert.Equal(y0, r.Y0, 1e-9);
        Assert.Equal(x1, r.X1, 1e-9);
        Assert.Equal(y1, r.Y1, 1e-9);
    }

    [Fact]
    public void A_vertical_dimension_s_text_turns_with_it()
    {
        // turned 90° about its lower-left point: the text runs up, its height to the left
        var r = SheetFrame.TextBox(100, 50, 16.8, 3.5, 1, Math.PI / 2);
        Assert.Equal(96.5, r.X0, 1e-9);
        Assert.Equal(100, r.X1, 1e-9);
        Assert.Equal(50, r.Y0, 1e-9);
        Assert.Equal(66.8, r.Y1, 1e-9);
    }

    [Theory]
    [InlineData("front", "+X", "+Y", "+Z")]
    [InlineData("top", "+X", "-Z", "+Y")]
    [InlineData("right", "-Z", "+Y", "+X")]
    [InlineData("left", "+Z", "+Y", "-X")]
    [InlineData("bottom", "+X", "+Z", "-Y")]
    public void A_view_s_axes_name_the_model_direction_on_the_sheet(string view, string right, string up, string @out)
    {
        var a = DrwAxes.Of(DrawingLayout.NamedViews[view].Axes);
        Assert.Equal(right, a.Right);
        Assert.Equal(up, a.Up);
        Assert.Equal(@out, a.Out);
    }

    [Fact]
    public void An_iso_view_s_axes_are_vectors()
    {
        var a = DrwAxes.Of(DrawingLayout.NamedViews["iso"].Axes);
        Assert.Equal("[0.7071, 0, -0.7071]", a.Right);
        Assert.StartsWith("[", a.Out);
    }

    [Fact]
    public void Touching_segments_join_one_block_and_apart_ones_do_not()
    {
        Assert.Equal(0, new SheetLine(0, 0, 10, 0).DistanceTo(new SheetLine(5, -5, 5, 5)), 1e-12);
        Assert.Equal(0, new SheetLine(0, 0, 10, 0).DistanceTo(new SheetLine(10, 0, 10, 5)), 1e-12);
        Assert.Equal(2, new SheetLine(0, 0, 10, 0).DistanceTo(new SheetLine(12, 0, 20, 0)), 1e-12);
    }
}
