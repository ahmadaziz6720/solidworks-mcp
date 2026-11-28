using SolidWorksMcp.Core.Geometry;
using SolidWorksMcp.Core.Verify;
using Xunit;

namespace SolidWorksMcp.UnitTests;

/// <summary>
/// S64 (c): a fillet with `propagate` off that stops where its edge ends tangent to an edge not in the call fails at any
/// radius; its hint names that joint. The directions are those of a plate's round (R7) on its front face: at its top end
/// it runs on into the top line, at its other end it meets a face 30° off its tangent.
/// </summary>
public class FilletJointsTests
{
    private static readonly Vec3 X = new(1, 0, 0), MinusX = new(-1, 0, 0), Down = new(0, -1, 0), Z = new(0, 0, 1);
    private static readonly Vec3 ThirtyOff = new(-Math.Sin(Math.PI / 6), -Math.Cos(Math.PI / 6), 0);

    [Fact]
    public void Edges_that_run_on_into_each_other_are_tangent()
    {
        Assert.True(FilletJoints.Tangent(MinusX, X));
        Assert.True(FilletJoints.Tangent(new Vec3(-2, 0, 0), new Vec3(1, 1e-4, 0))); // 0.1 mrad off, lengths not unit
        Assert.False(FilletJoints.Tangent(X, X)); // the same way: a cusp, not a joint
        Assert.False(FilletJoints.Tangent(new Vec3(0, 1, 0), ThirtyOff)); // the round's lower end, 30° off
        Assert.False(FilletJoints.Tangent(MinusX, Z)); // the edge along the plate's thickness
        Assert.False(FilletJoints.Tangent(new Vec3(0, 0, 0), X));
    }

    [Fact]
    public void A_round_alone_is_told_it_ends_tangent_to_the_line()
    {
        EdgeJoint[] joints =
        [
            new("E1", "E3", MinusX, X), // the round's top end runs on into the top line E3
            new("E1", "E9", Down, ThirtyOff), // its lower end meets the 30° line: no joint
        ];
        var open = FilletJoints.Open(joints, new HashSet<string> { "E1" });
        Assert.Equal(new[] { ("E1", "E3") }, open);
        Assert.Equal("edge E1 ends tangent to E3, not in the call: turn `propagate` on or add it.", FilletJoints.Hint(open));
    }

    [Fact]
    public void Two_rounds_name_each_joint_and_one_line_is_it()
    {
        EdgeJoint[] joints = [new("E1", "E3", MinusX, X), new("E2", "E3", X, MinusX), new("E1", "E3", MinusX, X)];
        var open = FilletJoints.Open(joints, new HashSet<string> { "E1", "E2" });
        Assert.Equal("edge E1 ends tangent to E3; edge E2 ends tangent to E3, not in the call: turn `propagate` on or add it.",
            FilletJoints.Hint(open));
        Assert.EndsWith("add them.", FilletJoints.Hint([("E1", "E3"), ("E1", "E4")]));
    }

    [Fact]
    public void A_joint_with_an_edge_in_the_call_is_no_hint()
    {
        // The round and the line it runs on into: SOLIDWORKS builds it (probed), and no joint is left open.
        var open = FilletJoints.Open([new EdgeJoint("E1", "E3", MinusX, X)], new HashSet<string> { "E1", "E3" });
        Assert.Empty(open);
        Assert.Null(FilletJoints.Hint(open));
        Assert.Empty(FilletJoints.Open([new EdgeJoint("E1", "E1", MinusX, X)], new HashSet<string> { "E1" }));
    }

    // S124: a constant fillet whose round reaches past a face it blends, or after which such a face is gone, warns naming it.

    [Fact]
    public void The_round_reaches_r_into_the_faces_of_a_right_angle_more_at_an_acute_edge_less_at_an_obtuse_one()
    {
        var front = new Vec3(0, 0, 1);
        var bottom = new Vec3(0, -1, 0);
        Assert.Equal(20, FilletOverflow.Reach(20, front, bottom), 9);
        // An edge of 60° between the faces: the normals 120° apart, r·tan 60°.
        Assert.Equal(20 * Math.Sqrt(3), FilletOverflow.Reach(20, front, new Vec3(0, -Math.Cos(Math.PI / 6), -Math.Sin(Math.PI / 6))), 9);
        // An edge of 120°: the normals 60° apart, r·tan 30°.
        Assert.Equal(20 / Math.Sqrt(3), FilletOverflow.Reach(20, front, new Vec3(0, -Math.Cos(Math.PI / 6), Math.Sin(Math.PI / 6))), 9);
        Assert.Equal(20, FilletOverflow.Reach(20, Vec3.Zero, bottom)); // a normal not read: r
    }

    [Fact]
    public void A_round_past_its_faces_is_warned_of_naming_each_and_an_ordinary_one_is_not()
    {
        BlendedFace F(string id, double widthMm, double reachMm, bool gone) => new(id, "plane, normal [0, 0, 1]", "E2", widthMm / 1000, reachMm / 1000, gone);
        Assert.Null(FilletOverflow.Warning(0.001, [F("F3", 10, 1, false), F("F1", 10, 1, false)]));
        Assert.Null(FilletOverflow.Warning(0.01, [F("F3", 10, 10, false)])); // reaching exactly to the far edge
        Assert.Equal("The R20 mm fillet reaches past faces it blends: F3 (plane, normal [0, 0, 1]) is 10 mm wide beside E2, the round " +
                     "reaching 20 mm into it and is gone after the fillet; F1 (plane, normal [0, 0, 1]) is 10 mm wide beside E2, the round " +
                     "reaching 20 mm into it. SOLIDWORKS may have run the round over them: check the shape (view_screenshot) or use a smaller radius.",
            FilletOverflow.Warning(0.02, [F("F3", 10, 20, true), F("F1", 10, 20, false), F("F5", 30, 20, false)]));
        Assert.Contains("F4 (plane, normal [0, 0, 1]) is gone after the fillet.", FilletOverflow.Warning(0.002, [F("F4", 10, 2, true)]));
    }
}
