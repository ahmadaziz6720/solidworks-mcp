using SolidWorksMcp.Core.Geometry;
using SolidWorksMcp.Core.Services;
using Xunit;

namespace SolidWorksMcp.UnitTests;

/// <summary>
/// S80 (a), (c): sketch_add's warnings when SOLIDWORKS' solve moved what it drew, and when a rectangle drawn square was made
/// equal-sided. The points are those read live for a rectangle drawn 3 tall on a face, solved 2.5 tall.
/// </summary>
public class SketchDriftTests
{
    private const double Tol = 1e-7; // m, sketch_add's MergeTol

    private static Vec3 P(double x, double y) => new(x / 1000, y / 1000, 0);

    private static readonly string[] Segs = ["Line1", "Line2", "Line3", "Line4"];

    private static readonly DrawnPoint[] Drawn =
    [
        new("corners[0]", P(100, 40)), new("corners[1]", P(-95, 43)), new("a corner", P(100, 43)), new("a corner", P(-95, 40)),
    ];

    private static readonly string[] Relations =
    [
        "horizontal: Line1", "vertical: Line2", "horizontal: Line3", "vertical: Line4",
        "coincident: a model entity, Line1 start", "midpoint: Line3 end, a model edge",
    ];

    private static SolvedPoint[] Solved(double top) =>
    [
        new(["Line1 start", "Line2 start"], P(100, 40)), new(["Line2 end", "Line3 start"], P(100, top)),
        new(["Line3 end", "Line4 end"], P(-95, top)), new(["Line1 end", "Line4 start"], P(-95, 40)),
    ];

    [Fact]
    public void A_rectangle_solved_shorter_names_its_moved_corners_and_the_relation_to_the_model_edge()
    {
        string? w = SketchDrift.Moved("entities[0] (rectangle)", Drawn, Solved(42.5), Relations, Segs, false, Tol);
        Assert.NotNull(w);
        Assert.StartsWith("entities[0] (rectangle) is not where it was drawn: SOLIDWORKS moved " +
                          "corners[1] [-95, 43] to [-95, 42.5] (0.5 mm), a corner [100, 43] to [100, 42.5] (0.5 mm), " +
                          "by its relations midpoint: Line3 end, a model edge. ", w);
        Assert.DoesNotContain("coincident", w); // its corner did not move
        Assert.DoesNotContain("vertical", w);   // the rectangle's own relations
    }

    [Fact]
    public void Points_where_they_were_drawn_give_no_warning()
    {
        Assert.Null(SketchDrift.Moved("entities[0] (rectangle)", Drawn, Solved(43), Relations, Segs, false, Tol));
        // A merge within the tolerance (S65 e) is no move.
        Assert.Null(SketchDrift.Moved("entities[0] (rectangle)", Drawn, Solved(43.00009), Relations, Segs, false, Tol));
    }

    [Fact]
    public void A_move_with_no_relation_naming_its_point_names_every_outside_relation_or_none()
    {
        // A whole line pulled onto a model edge: the relation names the segment, not a point.
        DrawnPoint[] line = [new("from", P(0, 0)), new("to", P(10, 0))];
        SolvedPoint[] solved = [new(["Line5 start"], P(0, 1)), new(["Line5 end"], P(10, 1))];
        string? w = SketchDrift.Moved("entities[1] (line)", line, solved, ["collinear: Line5, a model edge"], ["Line5"], false, Tol);
        Assert.Contains("by its relations collinear: Line5, a model edge.", w);
        Assert.Contains("from [0, 0] to [0, 1] (1 mm), to [10, 0] to [10, 1] (1 mm)", w);

        string? none = SketchDrift.Moved("entities[1] (line)", line, solved, null, ["Line5"], false, Tol);
        Assert.Contains("with no relation to other geometry: snapped to it as SOLIDWORKS drew", none);
    }

    [Fact]
    public void A_centre_rectangle_s_own_centre_point_is_not_named_as_the_cause()
    {
        // Read live: a centre rectangle's diagonals are coincident with its free centre point; snapped taller, it has no
        // relation to other geometry.
        DrawnPoint[] drawn = [new("a corner", P(163, -3)), new("a corner", P(177, 3))];
        SolvedPoint[] solved = [new(["Line11 start", "Line12 start"], P(163, -3.5)), new(["Line13 end", "Line14 end"], P(177, 3.5))];
        string[] rels = ["horizontal: Line11", "coincident: point [170, 0], Line15", "coincident: point [170, 0], Line16"];
        string? w = SketchDrift.Moved("entities[4] (rectangle)", drawn, solved, rels,
            ["Line11", "Line12", "Line13", "Line14", "Line15", "Line16"], false, Tol);
        Assert.Contains("with no relation to other geometry", w);
        Assert.Equal(["point [170, 0]", "Line15"], SketchDrift.Tokens("coincident: point [170, 0], Line15"));
    }

    [Fact]
    public void Only_a_rectangle_with_an_equal_is_warned_of_as_square()
    {
        string? w = SketchDrift.SquareEqual("entities[0] (rectangle)", "rectangle", ["horizontal: Line1", "equal: Line2, Line1"]);
        Assert.StartsWith("entities[0] (rectangle) was drawn square, and SOLIDWORKS made its sides equal (equal: Line2, Line1)", w);
        Assert.Null(SketchDrift.SquareEqual("entities[0] (rectangle)", "rectangle", ["horizontal: Line1", "vertical: Line2"]));
        Assert.Null(SketchDrift.SquareEqual("entities[0] (rectangle)", "rectangle", null));
        Assert.Null(SketchDrift.SquareEqual("entities[0] (polygon)", "polygon", ["equal: Line1, Line2"]));
    }
}
