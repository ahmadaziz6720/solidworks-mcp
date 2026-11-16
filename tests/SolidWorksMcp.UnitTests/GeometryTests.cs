using System.Globalization;
using SolidWorksMcp.Core;
using SolidWorksMcp.Core.Geometry;
using Xunit;

namespace SolidWorksMcp.UnitTests;

public class UnitsTests
{
    [Fact]
    public void Converts_mm_m_and_rounds()
    {
        Assert.Equal(0.025, Units.M(25));
        Assert.Equal(25, Units.Mm(0.025));
        Assert.Equal(120000, Units.Mm3(1.2e-4));
        Assert.Equal(6000, Units.Mm2(0.006));
        Assert.Equal(90, Units.Deg(Math.PI / 2));
        Assert.Equal(Math.PI, Units.Rad(180), 12);
        Assert.Equal(1.0, Units.Mm(0.0010000000004)); // float noise rounded away
        // Noise below the rounding comes out as +0, never -0 (which JSON and messages would print as "-0").
        Assert.False(double.IsNegative(Units.Mm(-1e-15)));
        Assert.False(double.IsNegative(Units.R(-1e-12)));
        Assert.False(double.IsNegative(Units.Deg(-1e-12)));
        Assert.False(double.IsNegative(Units.Mm3(-1e-16)));
        Assert.True(double.IsNegative(Units.Mm(-0.001)));
    }

    [Fact]
    public void Point_validation_names_the_argument()
    {
        var ex = Assert.Throws<SwException>(() => Units.PointM([1, 2], "at"));
        Assert.Equal(ErrorCodes.InvalidArgument, ex.Code);
        Assert.Contains("'at'", ex.Message);
        Assert.Throws<SwException>(() => Units.PointM([1, double.NaN, 3], "at"));
        Assert.Throws<SwException>(() => Units.PositiveM(0, "depthMm"));
        Assert.Equal(new Vec3(0.001, 0.002, 0.003), Units.PointM([1, 2, 3], "at"));
    }

    [Fact]
    public void Formatting_is_culture_invariant()
    {
        var old = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var json = Core.Model.ResultJson.Serialize(new Core.Model.ModelState { VolumeMm3 = 1.5, Bodies = 1 });
            Assert.Contains("1.5", json);
            Assert.DoesNotContain("1,5", json);
        }
        finally { CultureInfo.CurrentCulture = old; }
    }

    [Fact]
    public void A_feature_result_lists_new_faces_by_id_and_surface_unless_asked_in_full()
    {
        var r = new Core.Model.FeatureResult
        {
            Feature = new Core.Model.FeatureRef { Name = "Boss-Extrude1", Type = "Extrusion" },
            Verified = new Core.Model.Verified { Material = "added" },
            Model = new Core.Model.ModelState { VolumeMm3 = 1000, Bodies = 1 },
            NewFaces =
            [
                new Core.Model.FaceInfo { Id = "F1", Surface = "plane", AreaMm2 = 100, PickMm = [1, 2, 3], Normal = [0, 1, 0], Feature = "Boss-Extrude1" },
                new Core.Model.FaceInfo { Id = "F2", Surface = "cylinder", AreaMm2 = 50, PickMm = [4, 5, 6], RadiusMm = 3 },
            ],
            NewFaceCount = 2,
        };
        string brief = Core.Model.ResultJson.Serialize(r);
        Assert.Contains("\"newFaces\":[{\"id\":\"F1\",\"surface\":\"plane\"},{\"id\":\"F2\",\"surface\":\"cylinder\"}],\"newFaceCount\":2", brief);
        Assert.DoesNotContain("pickMm", brief);
        string full = Core.Model.ResultJson.Serialize(r, detail: true);
        Assert.Contains("\"pickMm\":[1,2,3]", full);
        Assert.Contains("\"radiusMm\":3", full);
        // Everything but the faces is the same text.
        Assert.Equal(full[..full.IndexOf("\"newFaces\"", StringComparison.Ordinal)], brief[..brief.IndexOf("\"newFaces\"", StringComparison.Ordinal)]);
        Assert.Equal(full[full.IndexOf("\"newFaceCount\"", StringComparison.Ordinal)..], brief[brief.IndexOf("\"newFaceCount\"", StringComparison.Ordinal)..]);
    }

    [Fact]
    public void A_short_new_face_of_another_feature_than_the_first_keeps_its_feature()
    {
        // S75: a chamfer an edge moved to lists its faces after the named chamfer's.
        var r = new Core.Model.FeatureResult
        {
            Feature = new Core.Model.FeatureRef { Name = "C1", Type = "Chamfer" },
            Verified = new Core.Model.Verified { Material = "removed" },
            Model = new Core.Model.ModelState { VolumeMm3 = 1000, Bodies = 1 },
            NewFaces =
            [
                new Core.Model.FaceInfo { Id = "F1", Surface = "plane", PickMm = [0, 0, 0], Feature = "C1" },
                new Core.Model.FaceInfo { Id = "F2", Surface = "plane", PickMm = [0, 0, 0], Feature = "C1" },
                new Core.Model.FaceInfo { Id = "F3", Surface = "plane", PickMm = [0, 0, 0], Feature = "Chamfer2" },
            ],
            NewFaceCount = 3,
        };
        Assert.Contains("\"newFaces\":[{\"id\":\"F1\",\"surface\":\"plane\"},{\"id\":\"F2\",\"surface\":\"plane\"}," +
                        "{\"id\":\"F3\",\"surface\":\"plane\",\"feature\":\"Chamfer2\"}]", Core.Model.ResultJson.Serialize(r));
    }

    private sealed class RowsResult : Core.Model.ToolResult, Core.Model.IShortRows
    {
        public required string[] Rows { get; init; }
        public int Count { get; init; }
        public (string Property, System.Text.Json.Nodes.JsonArray Rows)? ShortRows() =>
            ("rows", new System.Text.Json.Nodes.JsonArray([.. Rows.Select(r => (System.Text.Json.Nodes.JsonNode)r[..1])]));
    }

    [Fact]
    public void A_result_with_short_rows_gives_them_in_place_unless_asked_in_full()
    {
        // S48 (d): equation_set's batch rows go through the same path.
        var r = new RowsResult { Doc = "Part1", Rows = ["abc", "def"], Count = 2, Warnings = ["w"] };
        Assert.Equal("{\"doc\":\"Part1\",\"rows\":[\"a\",\"d\"],\"count\":2,\"warnings\":[\"w\"]}", Core.Model.ResultJson.Serialize(r));
        Assert.Equal("{\"doc\":\"Part1\",\"rows\":[\"abc\",\"def\"],\"count\":2,\"warnings\":[\"w\"]}", Core.Model.ResultJson.Serialize(r, detail: true));
    }
}

public class Vec3Tests
{
    [Fact]
    public void Vector_algebra()
    {
        var a = new Vec3(1, 0, 0);
        var b = new Vec3(0, 1, 0);
        Assert.Equal(Vec3.UnitZ, a.Cross(b));
        Assert.Equal(0, a.Dot(b));
        Assert.True(new Vec3(0, 0, 2).IsParallelTo(-Vec3.UnitZ));
        Assert.False(new Vec3(0, 1, 2).IsParallelTo(Vec3.UnitZ, 0.01));
        Assert.Equal(5, new Vec3(3, 4, 0).Length, 12);
    }
}

public class XformTests
{
    // SOLIDWORKS ArrayData: rotation 0..8 row-major, translation 9..11, scale 12; row vectors: p' = p·R + t.
    private static Xform RotZ90(double tx = 0, double ty = 0, double tz = 0) =>
        new([0, 1, 0, -1, 0, 0, 0, 0, 1, tx, ty, tz, 1, 0, 0, 0]);

    [Fact]
    public void Applies_row_vector_convention()
    {
        var x = RotZ90();
        var p = x.ApplyPoint(new Vec3(1, 0, 0));
        Assert.Equal(0, p.X, 12);
        Assert.Equal(1, p.Y, 12);
        Assert.Equal(new Vec3(-1, 0, 0), Round(x.ApplyVector(Vec3.UnitY)));
    }

    [Fact]
    public void Inverse_round_trips()
    {
        var x = RotZ90(0.01, -0.02, 0.03);
        var inv = x.Inverse();
        var p = new Vec3(0.1, 0.2, 0.3);
        var back = inv.ApplyPoint(x.ApplyPoint(p));
        Assert.True(back.DistanceTo(p) < 1e-12);
        Assert.True(Xform.Identity.IsIdentity());
    }

    [Fact]
    public void Top_plane_sketch_maps_x_y_to_x_0_minus_y()
    {
        // Model→sketch for Top Plane: sketch x = model x, sketch y = −model z. Its inverse maps (x, y) to (x, 0, −y).
        var sketchToModel = new Xform([1, 0, 0, 0, 0, -1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0]);
        var m = sketchToModel.ApplyPoint(new Vec3(0.02, 0.03, 0));
        Assert.Equal(new Vec3(0.02, 0, -0.03), Round(m));
    }

    private static Vec3 Round(Vec3 v) => new(Math.Round(v.X, 12), Math.Round(v.Y, 12), Math.Round(v.Z, 12));
}

public class AxisSpecTests
{
    [Theory]
    [InlineData("+y", 0, 1, 0)]
    [InlineData("y", 0, 1, 0)]
    [InlineData("-x", -1, 0, 0)]
    [InlineData("z-", 0, 0, -1)]
    [InlineData("0,0,2", 0, 0, 1)]
    [InlineData("[1, 1, 0]", 0.7071067811865475, 0.7071067811865475, 0)]
    public void Parses_axes(string spec, double x, double y, double z)
    {
        var v = AxisSpec.Parse(spec, "a");
        Assert.Equal(x, v.X, 9);
        Assert.Equal(y, v.Y, 9);
        Assert.Equal(z, v.Z, 9);
    }

    [Theory]
    [InlineData("up")]
    [InlineData("0,0,0")]
    [InlineData("1,2")]
    public void Rejects_bad_axes(string spec) =>
        Assert.Equal(ErrorCodes.InvalidArgument, Assert.Throws<SwException>(() => AxisSpec.Parse(spec, "a")).Code);

    [Fact]
    public void Extreme_gives_axis_and_sign()
    {
        var (axis, sign) = AxisSpec.Extreme("-z");
        Assert.Equal(Vec3.UnitZ, axis);
        Assert.Equal(-1, sign);
    }
}

/// <summary>feature_pattern_circular's words for its sense (S73): about the axis as read, and about it the other way along.</summary>
public class CircularPatternSenseTests
{
    [Fact]
    public void An_axis_along_minus_z_names_the_sense_about_plus_z_too()
    {
        string s = SolidWorksMcp.Core.Services.CircularPatternSense.Describe(-Vec3.UnitZ, 3, 10, 1);
        Assert.Equal("3 x 10° about [0, 0, -1] (the axis as read), counter-clockwise: the instances at +10°, +20° from the seed; " +
                     "the axis as read points the other way from +z: about +z, clockwise, at -10°, -20°", s);
    }

    [Fact]
    public void A_positive_axis_says_its_sense_once_and_a_tie_says_none()
    {
        Assert.Equal("4 x 30° about [0, 1, 0] (the axis as read), clockwise: the instances at -30°, -60°, -90° from the seed",
            SolidWorksMcp.Core.Services.CircularPatternSense.Describe(Vec3.UnitY, 4, 30, -1));
        Assert.Equal("6 x 60° about [0, 0, -1]", SolidWorksMcp.Core.Services.CircularPatternSense.Describe(-Vec3.UnitZ, 6, 60, 0));
    }

    [Fact]
    public void A_slanted_negative_axis_names_the_vector_and_many_instances_are_cut_short()
    {
        string s = SolidWorksMcp.Core.Services.CircularPatternSense.Describe(new Vec3(-0.6, 0.8, 0) * -1, 8, 12.5, 1);
        Assert.Equal("8 x 12.5° about [0.6, -0.8, 0] (the axis as read), counter-clockwise: the instances at +12.5°, +25°, …, +87.5° " +
                     "from the seed; the axis as read points the other way from [-0.6, 0.8, 0]: about [-0.6, 0.8, 0], clockwise, at -12.5°, -25°, …, -87.5°", s);
    }
}

/// <summary>view_screenshot's warning of a shown sketch far past the bodies in the fit (S88).</summary>
public class FitReachTests
{
    private static readonly double[] Block = [-0.02, 0, -0.02, 0.02, 0.04, 0.02]; // 40 mm

    [Fact]
    public void A_sketch_far_past_the_bodies_warns_one_near_them_does_not()
    {
        Assert.True(SolidWorksMcp.Core.Services.FitReach.Warns(Block, [0, -0.02, 0.3, 0, 0.02, 0.3])); // an axis's points at z 300
        Assert.Equal(0.28, SolidWorksMcp.Core.Services.FitReach.Past(Block, [0, -0.02, 0.3, 0, 0.02, 0.3]), 12);
        Assert.False(SolidWorksMcp.Core.Services.FitReach.Warns(Block, [-0.02, 0, -0.02, 0.02, 0.02, 0.02])); // inside
        Assert.False(SolidWorksMcp.Core.Services.FitReach.Warns(Block, [-0.023, 0, -0.02, 0.02, 0.02, 0.02])); // 3 mm past
        Assert.True(SolidWorksMcp.Core.Services.FitReach.Warns(Block, [-0.025, 0, -0.02, 0.02, 0.02, 0.02])); // 5 mm past
        Assert.Empty(SolidWorksMcp.Core.Services.FitReach.Box([]));
        Assert.Equal([-1, 0, 2, 3, 5, 2], SolidWorksMcp.Core.Services.FitReach.Box([(3, 0, 2), (-1, 5, 2)]));
    }

    [Fact]
    public void An_arc_adds_the_extremes_on_its_sweep_only()
    {
        // A quarter from (1, 0) to (0, 1) about the origin: counter-clockwise it holds 0° and 90°; clockwise the other three quarters.
        var ccw = SolidWorksMcp.Core.Services.FitReach.ArcExtremes(0, 0, 1, 1, 0, 0, 1, true, false).ToList();
        Assert.Equal(2, ccw.Count);
        Assert.DoesNotContain(ccw, p => p.X < -0.5 || p.Y < -0.5);
        var cw = SolidWorksMcp.Core.Services.FitReach.ArcExtremes(0, 0, 1, 1, 0, 0, 1, false, false).ToList();
        Assert.Equal(4, cw.Count);
        Assert.Contains(cw, p => Math.Abs(p.X + 1) < 1e-12);
        Assert.Contains(cw, p => Math.Abs(p.Y + 1) < 1e-12);
        // An arc between the extremes holds none of them; a circle all four.
        Assert.Empty(SolidWorksMcp.Core.Services.FitReach.ArcExtremes(0, 0, 1, Math.Cos(0.2), Math.Sin(0.2), Math.Cos(0.4), Math.Sin(0.4), true, false));
        Assert.Equal(4, SolidWorksMcp.Core.Services.FitReach.ArcExtremes(5, 5, 2, 7, 5, 7, 5, true, true).Count());
    }
}
