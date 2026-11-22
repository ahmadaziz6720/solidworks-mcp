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
}
