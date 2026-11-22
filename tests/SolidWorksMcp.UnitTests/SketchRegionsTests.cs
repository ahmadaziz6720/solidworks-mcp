using SolidWorksMcp.Core.Services;
using Xunit;

namespace SolidWorksMcp.UnitTests;

/// <summary>
/// S64 (f), (l): which of the regions SOLIDWORKS counts on a face are the sketch's own. The loops are those read live for
/// a Ø6.5 circle around a Ø3.2 hole and a Ø8 circle beside it on a block's top face (4 regions, 2 the sketch's).
/// </summary>
public class SketchRegionsTests
{
    private static RegionLoop OwnOuter => new(Outer: true, Own: true);

    [Fact]
    public void A_region_inside_one_contour_is_the_sketchs()
    {
        Assert.True(SketchRegions.IsOwn([OwnOuter]));
    }

    [Fact]
    public void A_contour_around_a_hole_of_the_face_is_the_sketchs()
    {
        // The ring: outer loop the Ø6.5 circle, inner loop the hole's edge, an inner loop of the face.
        Assert.True(SketchRegions.IsOwn([new(Outer: false, Own: false, InnerLoopOfFace: true), OwnOuter]));
        // A contour inside another, both the sketch's, as on a plane.
        Assert.True(SketchRegions.IsOwn([new(Outer: false, Own: true), OwnOuter]));
    }

    [Fact]
    public void Regions_the_face_closes_are_not_the_sketchs()
    {
        // The face around the sketch: outer loop the face's edges, inner loops the two circles.
        Assert.False(SketchRegions.IsOwn([new(false, true), new(false, true), new(Outer: true, Own: false)]));
        // The hole's disc: its one loop is the hole's edge, outer here.
        Assert.False(SketchRegions.IsOwn([new(Outer: true, Own: false, InnerLoopOfFace: true)]));
        // An inner loop of model edges that is not an inner loop of the face.
        Assert.False(SketchRegions.IsOwn([new(Outer: false, Own: false), OwnOuter]));
        // No outer loop read.
        Assert.False(SketchRegions.IsOwn([]));
    }
}
