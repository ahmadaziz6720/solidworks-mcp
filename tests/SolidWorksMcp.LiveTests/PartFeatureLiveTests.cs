using System.Text.RegularExpressions;
using SolidWorksMcp.Core;
using SolidWorksMcp.Core.Model;
using SolidWorksMcp.Core.Services;
using Xunit;

namespace SolidWorksMcp.LiveTests;

/// <summary>
/// Revolve, chamfer, shell, Hole Wizard holes, linear and circular patterns and mirror against a live SOLIDWORKS, with
/// analytic oracles (DESIGN §6.3). Every tool has positive tests and negative ones (typed errors, rollback).
/// </summary>
public class PartFeatureLiveTests(LiveFixture fx) : LiveTest(fx)
{
    private const double V = 1e-3;  // mm³: planar oracles
    private const double Vq = 2e-2; // mm³: oracles with cylinders and cones
    private const double HoleMm3 = Math.PI * 9 * 20; // a Ø6 hole through the 20 mm block: 565.49 mm³

    private PartFeatureService Pf => new(S);
    private SketchService Sk => new(S);
    private FeatureService Fs => new(S);
    private InspectService Ins => new(S);

    // ------------------------------------------------------------------ fixtures

    /// <summary>A 100 (x) × 20 (y) × 60 (z) block on the top plane, centred on the origin in x and z (top face at y = 20).</summary>
    private async Task<string> BlockAsync(string name)
    {
        string doc = await NewPartAsync(name);
        await Sk.StartAsync(doc, new EntityRef { Name = "top" }, false);
        await Sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Center = [0, 0], WidthMm = 100, HeightMm = 60 }]);
        var r = await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 20 });
        Assert.Equal(120000, r.Verified.VolumeDeltaMm3, V);
        return doc;
    }

    /// <summary>A disc r 40, 10 thick (y from 0 to 10), centred on the y axis.</summary>
    private async Task<string> DiscAsync(string name)
    {
        string doc = await NewPartAsync(name);
        await Sk.StartAsync(doc, new EntityRef { Name = "top" }, false);
        await Sk.AddAsync(doc, [new SketchEntity { Kind = "circle", Center = [0, 0], RadiusMm = 40 }]);
        var r = await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 10 });
        Assert.Equal(Math.PI * 1600 * 10, r.Verified.VolumeDeltaMm3, Vq);
        return doc;
    }

    /// <summary>A vertical hole through the part at model (x, z), cut from the top plane (the bottom face).</summary>
    private async Task<FeatureResult> HoleCutAsync(string doc, double x, double z, double diameterMm = 6)
    {
        await Sk.StartAsync(doc, new EntityRef { Name = "top" }, false);
        await Sk.AddAsync(doc, [new SketchEntity { Kind = "circle", Center = [x, 0, z], DiameterMm = diameterMm }]);
        var r = await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, Cut = true, End = "through_all" });
        Assert.Equal("removed", r.Verified.Material);
        return r;
    }

    /// <summary>A cylindrical boss r × h standing on the block's top face at model (x, z).</summary>
    private async Task<FeatureResult> BossAsync(string doc, double x, double z, double radiusMm = 5, double heightMm = 10)
    {
        await Sk.StartAsync(doc, new EntityRef { At = [x, 20, z] }, false);
        await Sk.AddAsync(doc, [new SketchEntity { Kind = "circle", Center = [x, 20, z], RadiusMm = radiusMm }]);
        var r = await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = heightMm });
        Assert.Equal(Math.PI * radiusMm * radiusMm * heightMm, r.Verified.VolumeDeltaMm3, Vq);
        return r;
    }

    private static void AssertBox(BoxMm? box, double[] min, double[] max, double tol = 0.01)
    {
        Assert.NotNull(box);
        for (int i = 0; i < 3; i++)
        {
            Assert.Equal(min[i], box.Min[i], tol);
            Assert.Equal(max[i], box.Max[i], tol);
        }
    }

    private static void AssertFailedAndRolledBack(SwException ex) =>
        Assert.True(ex.Code is ErrorCodes.FeatureFailed or ErrorCodes.RebuildError or ErrorCodes.VerifyFailed,
            $"expected a feature failure, got {ex.ToAgentText()}");

    private async Task<int> FeatureCountAsync(string doc) => (await Ins.FeatureListAsync(doc, false, 500, 0)).Total;

    /// <summary>The x (axisIndex 0) or z (2) positions of the axes of the cylindrical faces of radius r, sorted.</summary>
    private async Task<double[]> HoleAxesAsync(string doc, int axisIndex, double radiusMm = 3)
    {
        var faces = await Ins.FaceListAsync(doc, new EntityFilter { Surface = "cylinder", RadiusMm = radiusMm }, 200, 0);
        return faces.Faces.Select(f => Math.Round(f.AxisOriginMm![axisIndex], 3)).OrderBy(v => v).ToArray();
    }

    private async Task AssertHoleAxesAsync(double[] expected, string doc, int axisIndex, double radiusMm = 3)
    {
        double[] got = await HoleAxesAsync(doc, axisIndex, radiusMm);
        Assert.Equal(expected, got);
    }

    /// <summary>Drill-tip cone (118°) volume for a hole of radius r.</summary>
    private static double TipMm3(double r) => Math.PI * r * r * (r / Math.Tan(59 * Math.PI / 180)) / 3;

    // ------------------------------------------------------------------ revolve

    [Fact]
    public async Task Revolve_tube_around_the_sketch_centerline_matches_the_oracle()
    {
        string doc = await NewPartAsync("revolve");
        await Sk.StartAsync(doc, new EntityRef { Name = "front" }, false);
        await Sk.AddAsync(doc,
        [
            new SketchEntity { Kind = "rectangle", Corners = [[10, 0], [20, 40]] },
            new SketchEntity { Kind = "centerline", From = [0, -10], To = [0, 50] },
        ]);
        var r = await Pf.RevolveAsync(new RevolveRequest { Doc = doc });
        Assert.Equal(Math.PI * (400 - 100) * 40, r.Verified.VolumeDeltaMm3, Vq); // 37 699.11 mm³
        Assert.Equal("added", r.Verified.Material);
        Assert.NotNull(r.ConsumedSketch);
        Assert.Contains(r.Verified.Checks!, c => c.Contains("coaxial"));
        Assert.Contains(r.NewFaces!, f => f.Surface == "cylinder" && f.RadiusMm == 10);
        Assert.Contains(r.NewFaces!, f => f.Surface == "cylinder" && f.RadiusMm == 20);
        AssertBox(r.Model.Box, [-20, 0, -20], [20, 40, 20]);
    }

    [Fact]
    public async Task Revolve_partial_and_two_direction_angles_scale_the_volume()
    {
        string doc = await NewPartAsync("revolve_90");
        await Sk.StartAsync(doc, new EntityRef { Name = "front" }, false);
        await Sk.AddAsync(doc,
        [
            new SketchEntity { Kind = "rectangle", Corners = [[10, 0], [20, 40]] },
            new SketchEntity { Kind = "centerline", From = [0, -10], To = [0, 50] },
        ]);
        var r = await Pf.RevolveAsync(new RevolveRequest { Doc = doc, AngleDeg = 90 });
        Assert.Equal(Math.PI * 300 * 40 / 4, r.Verified.VolumeDeltaMm3, Vq);
        Assert.Contains(r.Verified.Checks!, c => c.Contains("end caps 90° apart"));

        string doc2 = await NewPartAsync("revolve_2dir");
        await Sk.StartAsync(doc2, new EntityRef { Name = "front" }, false);
        await Sk.AddAsync(doc2,
        [
            new SketchEntity { Kind = "rectangle", Corners = [[10, 0], [20, 40]] },
            new SketchEntity { Kind = "centerline", From = [0, -10], To = [0, 50] },
        ]);
        var r2 = await Pf.RevolveAsync(new RevolveRequest { Doc = doc2, AngleDeg = 90, Angle2Deg = 90 });
        Assert.Equal(Math.PI * 300 * 40 / 2, r2.Verified.VolumeDeltaMm3, Vq);
    }

    [Fact]
    public async Task Revolve_around_a_named_centerline_when_the_sketch_has_two()
    {
        // Best-guess path (the axis is selected with mark 16): the second centerline at x = -30 must be used, not the first.
        string doc = await NewPartAsync("revolve_axis");
        await Sk.StartAsync(doc, new EntityRef { Name = "front" }, false);
        var add = await Sk.AddAsync(doc,
        [
            new SketchEntity { Kind = "rectangle", Corners = [[10, 0], [20, 40]] },
            new SketchEntity { Kind = "centerline", From = [0, -10], To = [0, 50] },
            new SketchEntity { Kind = "centerline", From = [-30, -10], To = [-30, 50] },
        ]);
        string axis = add.Entities[2].Segments[0];
        var r = await Pf.RevolveAsync(new RevolveRequest { Doc = doc, Axis = new EntityRef { Name = axis } });
        Assert.Equal(Math.PI * (50 * 50 - 40 * 40) * 40, r.Verified.VolumeDeltaMm3, Vq); // r 40..50 about x = -30
        AssertBox(r.Model.Box, [-80, 0, -50], [20, 40, 50]);
        Assert.Equal("sketch_segment", Assert.Single(r.Resolved!).Type);
    }

    [Fact]
    public async Task Revolve_cut_turns_a_groove_into_the_block()
    {
        string doc = await BlockAsync("revolve_cut");
        await Sk.StartAsync(doc, new EntityRef { Name = "front" }, false); // z = 0, through the middle of the block
        await Sk.AddAsync(doc,
        [
            new SketchEntity { Kind = "rectangle", Corners = [[5, 15], [10, 25]] },
            new SketchEntity { Kind = "centerline", From = [0, 0], To = [0, 30] },
        ]);
        var r = await Pf.RevolveAsync(new RevolveRequest { Doc = doc, Cut = true });
        // The ring r 5..10 between y = 15 and the top face at y = 20.
        Assert.Equal(-Math.PI * (100 - 25) * 5, r.Verified.VolumeDeltaMm3, Vq); // -1 178.10 mm³
        Assert.Equal("removed", r.Verified.Material);
    }

    [Fact]
    public async Task Revolve_without_a_centerline_or_angle_is_rejected()
    {
        string doc = await NewPartAsync("revolve_noaxis");
        await Sk.StartAsync(doc, new EntityRef { Name = "front" }, false);
        await Sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Corners = [[10, 0], [20, 40]] }]);
        int before = await FeatureCountAsync(doc);
        var bad = await Assert.ThrowsAsync<SwException>(() => Pf.RevolveAsync(new RevolveRequest { Doc = doc, AngleDeg = 0 }));
        Assert.Equal(ErrorCodes.InvalidArgument, bad.Code);
        var ex = await Assert.ThrowsAsync<SwException>(() => Pf.RevolveAsync(new RevolveRequest { Doc = doc }));
        Assert.Equal(ErrorCodes.InvalidArgument, ex.Code);
        Assert.Contains("centerline", ex.Message);
        Assert.Equal(before, await FeatureCountAsync(doc)); // only the (now closed) sketch
    }

    [Fact]
    public async Task Revolve_of_a_profile_crossing_the_axis_is_rolled_back()
    {
        string doc = await NewPartAsync("revolve_cross");
        await Sk.StartAsync(doc, new EntityRef { Name = "front" }, false);
        await Sk.AddAsync(doc,
        [
            new SketchEntity { Kind = "rectangle", Corners = [[-10, 0], [20, 40]] },
            new SketchEntity { Kind = "centerline", From = [0, -10], To = [0, 50] },
        ]);
        int before = await FeatureCountAsync(doc);
        var ex = await Assert.ThrowsAsync<SwException>(() => Pf.RevolveAsync(new RevolveRequest { Doc = doc }));
        AssertFailedAndRolledBack(ex);
        Assert.Equal(before, await FeatureCountAsync(doc));
        Assert.Equal(0, (await Ins.ModelInfoAsync(doc)).VolumeMm3 ?? 0, V);
    }

    // ------------------------------------------------------------------ boxes

    [Fact]
    public async Task The_model_box_after_round_faces_is_the_body_s_extremes()
    {
        // S52 (a): the box in a result read the bodies' loose bounds over tori and spheres.
        string disc = await DiscAsync("box_torus");
        var r = await Fs.FilletAsync(disc, [new EntityRef { At = [40, 10, 0] }], null, 3, true, null, false);
        AssertBox(r.Model.Box!, [-40, 0, -40], [40, 10, 40]);
        Assert.Null(r.Model.Box!.Approximate);

        // A plate whose ends run in a quarter round R7 into a face 30° off its tangent, its top chain rounded R0.5: tori.
        string plate = await NewPartAsync("box_plate");
        await Sk.StartAsync(plate, new EntityRef { Name = "front" }, false);
        double t = 56.5 - 3 * Math.Tan(Math.PI / 6);
        await Sk.AddAsync(plate, [
            new SketchEntity { Kind = "line", From = [-t, -5], To = [t, -5] },
            new SketchEntity { Kind = "line", From = [t, -5], To = [56.5, -2] },
            new SketchEntity { Kind = "arc", Center = [49.5, -2], RadiusMm = 7, StartDeg = 0, EndDeg = 90 },
            new SketchEntity { Kind = "line", From = [49.5, 5], To = [-49.5, 5] },
            new SketchEntity { Kind = "arc", Center = [-49.5, -2], RadiusMm = 7, StartDeg = 90, EndDeg = 180 },
            new SketchEntity { Kind = "line", From = [-56.5, -2], To = [-t, -5] },
        ]);
        await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = plate, End = "mid_plane", DepthMm = 10 });
        var round = await Fs.FilletAsync(plate, [new EntityRef { At = [0, 5, 5] }], null, 0.5, true, null, false);
        AssertBox(round.Model.Box!, [-56.5, -5, -5], [56.5, 5, 5]);

        // An elliptic cylinder, 30 × 10 radii, 10 high: a spline-like face.
        string ell = await NewPartAsync("box_ellipse");
        await Sk.StartAsync(ell, new EntityRef { Name = "top" }, false);
        await Sk.AddAsync(ell, [new SketchEntity { Kind = "ellipse", Center = [0, 0], MajorRadiusMm = 30, MinorRadiusMm = 10 }]);
        var e = await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = ell, DepthMm = 10 });
        AssertBox(e.Model.Box!, [-30, 0, -10], [30, 10, 10]);
        AssertBox((await Ins.ModelInfoAsync(ell)).Box!, [-30, 0, -10], [30, 10, 10]);
    }

    private static void AssertBox(BoxMm box, double[] min, double[] max)
    {
        for (int i = 0; i < 3; i++)
            Assert.True(Math.Abs(min[i] - box.Min[i]) <= 1e-4 && Math.Abs(max[i] - box.Max[i]) <= 1e-4,
                $"box [{string.Join(", ", box.Min)}] .. [{string.Join(", ", box.Max)}], expected [{string.Join(", ", min)}] .. [{string.Join(", ", max)}]");
    }

    // ------------------------------------------------------------------ chamfer

    [Fact]
    public async Task Chamfer_equal_3mm_on_a_convex_edge_removes_450()
    {
        string doc = await BlockAsync("chamfer_equal");
        var r = await Pf.ChamferAsync(new ChamferRequest
        {
            Doc = doc, Edges = [new EntityRef { At = [0, 20, 30] }], DistanceMm = 3, Name = "EdgeBreak",
        });
        Assert.Equal(-450, r.Verified.VolumeDeltaMm3, V); // ½ · 3 · 3 · 100
        Assert.Equal("Chamfer", r.Feature.Type);
        Assert.Equal("EdgeBreak", r.Feature.Name);
        Assert.Contains(r.Verified.Checks!, c => c.Contains("legs measured on 1 edge"));
        Assert.Contains(r.NewFaces!, f => f.Surface == "plane" && Math.Abs(f.AreaMm2 - 100 * 3 * Math.Sqrt(2)) < 1e-3);
        // S44: on 2023 SP3 the equal-distance type made a chamfer with no faces on every edge tried, and each call warned
        // that it fell back to distance-distance. The same geometry, now without the failed attempt or the warning.
        Assert.Null(r.Warnings);

        // A concave circular edge, the base of a boss: an equal chamfer adds material there.
        string doc2 = await BlockAsync("chamfer_equal_concave");
        await BossAsync(doc2, 0, 0);
        var c = await Pf.ChamferAsync(new ChamferRequest { Doc = doc2, Edges = [new EntityRef { At = [5, 20, 0] }], DistanceMm = 1 });
        Assert.Equal("added", c.Verified.Material);
        Assert.Null(c.Warnings);
    }

    [Fact]
    public async Task Chamfer_two_distances_and_distance_angle_match_their_triangles()
    {
        string doc = await BlockAsync("chamfer_dd");
        var dd = await Pf.ChamferAsync(new ChamferRequest
        {
            Doc = doc, Edges = [new EntityRef { At = [0, 20, 30] }], Type = "distance_distance", DistanceMm = 2, Distance2Mm = 4,
        });
        Assert.Equal(-400, dd.Verified.VolumeDeltaMm3, V); // ½ · 2 · 4 · 100

        string doc2 = await BlockAsync("chamfer_da");
        var da = await Pf.ChamferAsync(new ChamferRequest
        {
            Doc = doc2, Edges = [new EntityRef { At = [0, 20, -30] }], Type = "angle_distance", DistanceMm = 3, AngleDeg = 30,
        });
        // Leg 3 and the other leg 3·tan 30° (or 3·tan 60°, depending on which face the angle is measured from).
        Assert.Contains(Math.Round(-da.Verified.VolumeDeltaMm3, 1), new[] { 259.8, 779.4 });
        Assert.Contains(da.Verified.Checks!, c => c.Contains("legs measured"));
    }

    [Fact]
    public async Task Chamfer_says_per_edge_which_face_took_each_distance()
    {
        // S51 (a): two top edges, 1 and 2 mm; which face takes the 1 is SOLIDWORKS' choice per edge, so the result names it.
        string doc = await BlockAsync("chamfer_sides");
        var r = await Pf.ChamferAsync(new ChamferRequest
        {
            Doc = doc, Edges = [new EntityRef { At = [0, 20, 30] }, new EntityRef { At = [0, 20, -30] }],
            Type = "distance_distance", DistanceMm = 1, Distance2Mm = 2,
        });
        Assert.Equal(-200, r.Verified.VolumeDeltaMm3, V);
        var lines = r.Verified.Checks!.Where(c => c.Contains(": 1 mm along F")).ToList();
        Assert.Equal(2, lines.Count);
        Assert.All(lines, l => Assert.Matches(@"2 mm along F\d+ \(plane \[[-0-9., ]+\]\); the chamfer face meets the first at 63[.,]43\d+° and the second at 26[.,]56\d+°", l));

        // flipEdges swaps the faces on that edge only; the top face then took the 1 on one edge and the 2 on the other: warned.
        string doc2 = await BlockAsync("chamfer_flip_one");
        var back = new EntityRef { At = [0, 20, -30] };
        var f = await Pf.ChamferAsync(new ChamferRequest
        {
            Doc = doc2, Edges = [new EntityRef { At = [0, 20, 30] }, back], FlipEdges = [1],
            Type = "distance_distance", DistanceMm = 1, Distance2Mm = 2,
        });
        Assert.Equal(-200, f.Verified.VolumeDeltaMm3, V);
        var top = f.Verified.Checks!.Where(c => Regex.IsMatch(c, @": 1 mm along F\d+ \(plane \[0, 1, 0\]\)")).ToList();
        var side = f.Verified.Checks!.Where(c => Regex.IsMatch(c, @": 1 mm along F\d+ \(plane \[0, 0, -?1\]\)")).ToList();
        Assert.Single(top);
        Assert.Single(side);
        Assert.Contains(f.Warnings!, w => w.Contains("first distance"));
        var notOne = await Assert.ThrowsAsync<SwException>(() => Pf.ChamferAsync(new ChamferRequest
        {
            Doc = doc2, Edges = [new EntityRef { At = [0, 0, 30] }], FlipEdges = [1], DistanceMm = 1,
        }));
        Assert.Equal(ErrorCodes.InvalidArgument, notOne.Code);
    }

    [Fact]
    public async Task Chamfer_edit_changes_edges_flips_and_distances_and_splits_edges_with_their_own()
    {
        // S51 (d): a chamfer with a hole after it; edges added and flipped, distances changed, one edge given its own.
        string doc = await BlockAsync("chamfer_edit");
        var front = new EntityRef { At = [0, 20, 30] };
        var back = new EntityRef { At = [0, 20, -30] };
        await Pf.ChamferAsync(new ChamferRequest { Doc = doc, Edges = [front], Type = "distance_distance", DistanceMm = 1, Distance2Mm = 2, Name = "C1" });
        await HoleCutAsync(doc, 30, 0);

        var a = await Pf.ChamferEditAsync(new ChamferEditRequest { Doc = doc, Feature = "C1", Edges = [front, back], FlipEdges = [1] });
        Assert.Equal(-100, a.Verified.VolumeDeltaMm3, V);
        Assert.Contains(a.Verified.Checks!, c => c.Contains("legs measured on 2 edge(s)"));
        Assert.Single(a.Verified.Checks!, c => Regex.IsMatch(c, @": 1 mm along F\d+ \(plane \[0, 1, 0\]\)"));

        var b = await Pf.ChamferEditAsync(new ChamferEditRequest { Doc = doc, Feature = "C1", DistanceMm = 1.5, Distance2Mm = 2.5 });
        Assert.Equal(-2 * (1.875 - 1) * 100, b.Verified.VolumeDeltaMm3, V); // each edge ½·1.5·2.5 instead of ½·1·2

        // A distance the faces can't take fails, and C1 is put back.
        var bad = await Assert.ThrowsAsync<SwException>(() => Pf.ChamferEditAsync(new ChamferEditRequest { Doc = doc, Feature = "C1", DistanceMm = 30 }));
        Assert.Contains("put back", bad.Message);
        Assert.Equal(120000 - 2 * 187.5 - HoleMm3, (await Ins.ModelInfoAsync(doc)).VolumeMm3!.Value, Vq);

        // The back edge at 3/3 leaves C1 for a new chamfer, placed right after C1, before the hole.
        // Through the tool: feature_chamfer {edit, edges, distancesMm: [[first, second] per edge]}.
        var tool = new SolidWorksMcp.Server.Tools.PartFeatureTools(S);
        await Assert.ThrowsAsync<SwException>(() => tool.FeatureChamfer(null!, edges: [front], distanceMm: 1, distancesMm: [[1, 1]], doc: doc));
        var json = System.Text.Json.JsonDocument.Parse(await tool.FeatureChamfer(null!, edges: [front, back], edit: "C1",
            distancesMm: [[1.5, 2.5], [3, 3]], doc: doc)).RootElement;
        Assert.Equal(-(450 - 187.5), json.GetProperty("verified").GetProperty("volumeDeltaMm3").GetDouble(), V);
        var made = Assert.Single(json.GetProperty("alsoCreated").EnumerateArray().ToList());
        Assert.Equal("Chamfer", made.GetProperty("type").GetString());
        string madeName = made.GetProperty("name").GetString()!;
        var names = (await Ins.FeatureListAsync(doc, false, 500, 0)).Features.Select(x => x.Name).ToList();
        Assert.True(names.IndexOf("C1") < names.IndexOf(madeName) && names.IndexOf(madeName) < names.FindIndex(n => n.StartsWith("Cut-Extrude")),
            string.Join(", ", names));
        Assert.Equal(120000 - 187.5 - 450 - HoleMm3, (await Ins.ModelInfoAsync(doc)).VolumeMm3!.Value, Vq);
    }

    [Fact]
    public async Task Chamfer_measures_the_legs_on_circular_edges()
    {
        // S51 (b): a rod's two end edges, 1 and 2 mm; the legs are read from the cone, as on straight edges from the plane.
        string doc = await NewPartAsync("chamfer_rod");
        await Sk.StartAsync(doc, new EntityRef { Name = "top" }, false);
        await Sk.AddAsync(doc, [new SketchEntity { Kind = "circle", Center = [0, 0], RadiusMm = 10 }]);
        await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 30 });
        var r = await Pf.ChamferAsync(new ChamferRequest
        {
            Doc = doc, Edges = [new EntityRef { At = [10, 0, 0] }, new EntityRef { At = [10, 30, 0] }],
            Type = "distance_distance", DistanceMm = 1, Distance2Mm = 2,
        });
        Assert.Contains(r.Verified.Checks!, c => c.Contains("legs measured on 2 edge(s)"));
        Assert.DoesNotContain(r.Verified.Checks!, c => c.Contains("not leg-checked"));
        // Per end: radial leg a, axial leg b removes ½·a·b·2π·(10 − a/3).
        double Ring(double a, double b) => 0.5 * a * b * 2 * Math.PI * (10 - a / 3);
        var ends = r.Verified.Checks!.Where(c => c.Contains(": 1 mm along F")).ToList();
        Assert.Equal(2, ends.Count);
        double want = -ends.Sum(c => Regex.IsMatch(c, @": 1 mm along F\d+ \(plane") ? Ring(1, 2) : Ring(2, 1));
        Assert.Equal(want, r.Verified.VolumeDeltaMm3, Vq);
        // One end with the 1 on its end face and the other with it on the cylinder is the swap a session met: warned.
        bool mixed = ends.Count(c => Regex.IsMatch(c, @": 1 mm along F\d+ \(plane")) == 1;
        Assert.Equal(mixed, r.Warnings?.Any(w => w.Contains("first distance")) == true);
    }

    [Fact]
    public async Task Chamfer_warns_of_a_sliver_left_by_a_distance_just_under_a_face_s_width()
    {
        // S51 (c): the front face is 20 high; 19.99995 leaves a strip 0.00005 × 100 = 0.005 mm².
        string doc = await BlockAsync("chamfer_sliver");
        var r = await Pf.ChamferAsync(new ChamferRequest { Doc = doc, Edges = [new EntityRef { At = [0, 20, 30] }], DistanceMm = 19.99995 });
        Assert.Contains(r.Warnings!, w => w.Contains("sliver") && w.Contains("mm²"));
    }

    [Fact]
    public async Task Chamfer_rejects_bad_input_and_rolls_back_an_oversized_chamfer()
    {
        string doc = await BlockAsync("chamfer_bad");
        var missing = await Assert.ThrowsAsync<SwException>(() => Pf.ChamferAsync(new ChamferRequest
        {
            Doc = doc, Edges = [new EntityRef { At = [0, 20, 30] }], Type = "distance_distance", DistanceMm = 2,
        }));
        Assert.Equal(ErrorCodes.InvalidArgument, missing.Code);
        var unknown = await Assert.ThrowsAsync<SwException>(() => Pf.ChamferAsync(new ChamferRequest
        {
            Doc = doc, Edges = [new EntityRef { At = [0, 20, 30] }], Type = "round", DistanceMm = 2,
        }));
        Assert.Equal(ErrorCodes.InvalidArgument, unknown.Code);

        int before = await FeatureCountAsync(doc);
        var big = await Assert.ThrowsAsync<SwException>(() => Pf.ChamferAsync(new ChamferRequest
        {
            Doc = doc, Edges = [new EntityRef { At = [0, 20, 30] }], DistanceMm = 30, // the front face is only 20 high
        }));
        AssertFailedAndRolledBack(big);
        Assert.Equal(before, await FeatureCountAsync(doc));
        Assert.Equal(120000, (await Ins.ModelInfoAsync(doc)).VolumeMm3!.Value, V);
    }

    // ------------------------------------------------------------------ shell

    [Fact]
    public async Task Shell_inward_2mm_removing_the_top_face_matches_the_oracle()
    {
        string doc = await BlockAsync("shell_in");
        var r = await Pf.ShellAsync(new ShellRequest { Doc = doc, RemoveFaces = [new EntityRef { At = [0, 20, 0] }], ThicknessMm = 2 });
        Assert.Equal(120000 - 96 * 56 * 18, r.Model.VolumeMm3, V); // 23 232 mm³
        Assert.Equal("Shell", r.Feature.Type);
        Assert.Contains(r.Verified.Checks!, c => c.Contains("5/5 walls"));
        AssertBox(r.Model.Box, [-50, 0, -30], [50, 20, 30]);
        Assert.Equal("face", Assert.Single(r.Resolved!).Type);
    }

    [Fact]
    public async Task Shell_outward_puts_the_walls_outside()
    {
        string doc = await BlockAsync("shell_out");
        var r = await Pf.ShellAsync(new ShellRequest
        {
            Doc = doc, RemoveFaces = [new EntityRef { At = [0, 20, 0] }], ThicknessMm = 2, Outward = true,
        });
        Assert.Equal(104 * 22 * 64 - 120000, r.Model.VolumeMm3, V); // 26 432 mm³: the old block is the cavity
        AssertBox(r.Model.Box, [-52, -2, -32], [52, 20, 32]);
    }

    [Fact]
    public async Task Shell_rejects_a_zero_wall_and_rolls_back_one_thicker_than_the_part()
    {
        string doc = await BlockAsync("shell_bad");
        var zero = await Assert.ThrowsAsync<SwException>(() =>
            Pf.ShellAsync(new ShellRequest { Doc = doc, RemoveFaces = [new EntityRef { At = [0, 20, 0] }], ThicknessMm = 0 }));
        Assert.Equal(ErrorCodes.InvalidArgument, zero.Code);

        int before = await FeatureCountAsync(doc);
        var thick = await Assert.ThrowsAsync<SwException>(() =>
            Pf.ShellAsync(new ShellRequest { Doc = doc, RemoveFaces = [new EntityRef { At = [0, 20, 0] }], ThicknessMm = 25 }));
        AssertFailedAndRolledBack(thick);
        Assert.Equal(before, await FeatureCountAsync(doc));
        Assert.Equal(120000, (await Ins.ModelInfoAsync(doc)).VolumeMm3!.Value, V);
    }

    // ------------------------------------------------------------------ holes

    [Fact]
    public async Task Hole_drill_6x10_matches_the_cylinder_and_tip_oracle()
    {
        string doc = await BlockAsync("hole_drill");
        var r = await Pf.HoleAsync(new HoleRequest
        {
            Doc = doc, Points = [[20, 20, 10]], Type = "drill", DiameterMm = 6, End = "blind", DepthMm = 10,
        });
        Assert.Equal(-(Math.PI * 9 * 10 + TipMm3(3)), r.Verified.VolumeDeltaMm3, 0.05); // -299.73 mm³
        Assert.Equal("HoleWzd", r.Feature.Type);
        Assert.Contains("Ø", r.Feature.Name);
        var cyl = Assert.Single(r.NewFaces!, f => f.Surface == "cylinder");
        Assert.Equal(3, cyl.RadiusMm!.Value, 1e-6);
        double[] axisAt = cyl.AxisOriginMm!;
        Assert.Equal(20, axisAt[0], 1e-6);
        Assert.Equal(10, axisAt[2], 1e-6);
        Assert.Contains(r.NewFaces!, f => f.Surface == "cone");
        Assert.Contains(r.Verified.Checks!, c => c.Contains("10 mm deep"));
    }

    [Fact]
    public async Task Hole_names_each_placement_sketch_and_warns_while_it_is_under_defined()
    {
        // S38: the result named the hole's own sketch dimensions but not the sketch that places it, which SOLIDWORKS leaves
        // under defined; only model_review showed it (info, sketch_under_defined).
        string doc = await BlockAsync("hole_place");
        var r = await Pf.HoleAsync(new HoleRequest { Doc = doc, Points = [[-20, 20, 0], [20, 20, 0]], DiameterMm = 5, End = "through_all" });
        Assert.Equal(2, r.PlacementSketches!.Count);
        Assert.Equal([r.Feature.Name, Assert.Single(r.AlsoCreated!).Name], r.PlacementSketches.Select(p => p.Feature));
        Assert.All(r.PlacementSketches, p => Assert.Equal("under", p.Constrained));
        Assert.Equal(2, r.PlacementSketches.Select(p => p.Sketch).Distinct().Count());
        // Not the hole's profile sketch, which carries its diameter and depth.
        Assert.Contains(r.Dimensions!, x => x.Name.StartsWith("Hole Dia.@", StringComparison.Ordinal));
        Assert.DoesNotContain(r.Dimensions!, x => x.Name.EndsWith("@" + r.PlacementSketches[0].Sketch, StringComparison.Ordinal));

        // The oracle: model_review lists exactly these as under defined among the holes' sketches, not the hole profiles.
        var review = await new ReviewService(S).ReviewAsync(doc);
        var under = review.Issues.Where(i => i.Kind == "sketch_under_defined").Select(i => i.Where).ToHashSet();
        Assert.All(r.PlacementSketches, p => Assert.Contains(p.Sketch, under));
        Assert.All(r.PlacementSketches, p => Assert.Contains(r.Warnings!, w => w.Contains($"placement sketch '{p.Sketch}' is under defined")));
    }

    [Fact]
    public async Task Hole_through_all_at_two_points_makes_two_named_features()
    {
        string doc = await BlockAsync("hole_two");
        var r = await Pf.HoleAsync(new HoleRequest
        {
            Doc = doc, Points = [[-20, 20, 0], [20, 20, 0]], DiameterMm = 5, End = "through_all", Name = "Pin",
        });
        Assert.Equal(-2 * Math.PI * 6.25 * 20, r.Verified.VolumeDeltaMm3, Vq); // two Ø5 through 20: -785.40 mm³
        Assert.Equal("Pin1", r.Feature.Name);
        Assert.Equal("Pin2", Assert.Single(r.AlsoCreated!).Name);
        await AssertHoleAxesAsync([-20, 20], doc, 0, 2.5);
    }

    [Fact]
    public async Task Hole_on_an_explicit_face_projects_the_points_onto_it()
    {
        string doc = await BlockAsync("hole_face");
        var r = await Pf.HoleAsync(new HoleRequest
        {
            Doc = doc, Points = [[-30, 25, 0], [30, 25, 0]], Face = new EntityRef { At = [0, 20, 0] }, DiameterMm = 6, End = "through_all",
        });
        Assert.Equal(-2 * HoleMm3, r.Verified.VolumeDeltaMm3, Vq);
        Assert.Contains(r.Warnings!, w => w.Contains("mm off"));
        await AssertHoleAxesAsync([-30, 30], doc, 0);
    }

    [Fact]
    public async Task Hole_clearance_M5_normal_fit_is_within_the_clearance_range()
    {
        // Best-guess recipe (ANSI-metric screw clearances, fastener type 40): verify live.
        string doc = await BlockAsync("hole_clear");
        var r = await Pf.HoleAsync(new HoleRequest
        {
            Doc = doc, Points = [[0, 20, 0]], Type = "clearance", Size = "M5", Fit = "normal", End = "through_all",
        });
        var cyl = Assert.Single(r.NewFaces!, f => f.Surface == "cylinder");
        double rr = cyl.RadiusMm!.Value;
        Assert.InRange(rr, 2.65, 2.9); // Ø5.3 (close) .. Ø5.8 (loose); normal is Ø5.5
        Assert.Equal(-Math.PI * rr * rr * 20, r.Verified.VolumeDeltaMm3, Vq);
    }

    [Fact]
    public async Task Hole_counterbore_M5_has_a_wider_coaxial_bore_at_the_face()
    {
        // Best-guess recipe (counterbore Ø10 x 5.4 through Value1/Value2): verify live.
        string doc = await BlockAsync("hole_cbore");
        var r = await Pf.HoleAsync(new HoleRequest { Doc = doc, Points = [[0, 20, 0]], Type = "counterbore", Size = "M5", End = "through_all" });
        Assert.Contains(r.Verified.Checks!, c => c.Contains("c'bore"));
        var radii = r.NewFaces!.Where(f => f.Surface == "cylinder").Select(f => f.RadiusMm!.Value).OrderBy(v => v).ToList();
        Assert.True(radii.Count >= 2 && radii[^1] > radii[0], $"cylinders: {string.Join(", ", radii)}");
        Assert.Equal("removed", r.Verified.Material);
    }

    [Fact]
    public async Task Hole_countersink_M5_has_a_cone_at_the_face()
    {
        // Best-guess recipe (countersink Ø11.4 x 90° through Value1/Value2): verify live.
        string doc = await BlockAsync("hole_csink");
        var r = await Pf.HoleAsync(new HoleRequest { Doc = doc, Points = [[0, 20, 0]], Type = "countersink", Size = "M5", End = "through_all" });
        Assert.Contains(r.Verified.Checks!, c => c.Contains("c'sink"));
        Assert.Contains(r.NewFaces!, f => f.Surface == "cone");
        Assert.Equal("removed", r.Verified.Material);
    }

    [Fact]
    public async Task Hole_tapped_M6_is_made_as_its_tap_drill_without_the_experimental_flag()
    {
        Assert.SkipWhen(PartFeatureService.Experimental, "SWMCP_EXPERIMENTAL is set: the experimental tapped-hole path runs instead.");
        string doc = await BlockAsync("hole_tap");
        var r = await Pf.HoleAsync(new HoleRequest { Doc = doc, Points = [[0, 20, 0]], Type = "tapped", Size = "M6", DepthMm = 12 });
        var cyl = Assert.Single(r.NewFaces!, f => f.Surface == "cylinder");
        Assert.Equal(2.5, cyl.RadiusMm!.Value, 1e-6); // tap drill M6 x 1.0 = Ø5
        Assert.Equal(-(Math.PI * 6.25 * 12 + TipMm3(2.5)), r.Verified.VolumeDeltaMm3, 0.05);
        Assert.Contains(r.Warnings!, w => w.Contains("tap-drill"));
    }

    [Fact]
    public async Task Hole_rejects_missing_depth_points_off_the_model_curved_faces_and_overlaps()
    {
        string doc = await BlockAsync("hole_bad");
        await HoleCutAsync(doc, 0, 0, 10); // a Ø10 hole: its wall is a cylinder at r 5
        double vol = (await Ins.ModelInfoAsync(doc)).VolumeMm3!.Value;
        int before = await FeatureCountAsync(doc);

        var depth = await Assert.ThrowsAsync<SwException>(() =>
            Pf.HoleAsync(new HoleRequest { Doc = doc, Points = [[20, 20, 10]], DiameterMm = 6 }));
        Assert.Equal(ErrorCodes.InvalidArgument, depth.Code);
        var off = await Assert.ThrowsAsync<SwException>(() =>
            Pf.HoleAsync(new HoleRequest { Doc = doc, Points = [[20, 50, 10]], DiameterMm = 6, End = "through_all" }));
        Assert.Equal(ErrorCodes.EntityNotFound, off.Code);
        var curved = await Assert.ThrowsAsync<SwException>(() =>
            Pf.HoleAsync(new HoleRequest { Doc = doc, Points = [[5, 10, 0]], DiameterMm = 2, End = "through_all" }));
        Assert.Equal(ErrorCodes.InvalidArgument, curved.Code);
        var overlap = await Assert.ThrowsAsync<SwException>(() =>
            Pf.HoleAsync(new HoleRequest { Doc = doc, Points = [[20, 20, 10], [22, 20, 10]], DiameterMm = 6, End = "through_all" }));
        Assert.Equal(ErrorCodes.InvalidArgument, overlap.Code);

        Assert.Equal(before, await FeatureCountAsync(doc));
        Assert.Equal(vol, (await Ins.ModelInfoAsync(doc)).VolumeMm3!.Value, V);
    }

    [Fact]
    public async Task Hole_failing_at_the_second_point_removes_the_first_one_too()
    {
        string doc = await BlockAsync("hole_partial");
        int before = await FeatureCountAsync(doc);
        // The second point is 30 mm above the model: no face there.
        var ex = await Assert.ThrowsAsync<SwException>(() =>
            Pf.HoleAsync(new HoleRequest { Doc = doc, Points = [[-20, 20, 0], [20, 50, 0]], DiameterMm = 6, End = "through_all" }));
        Assert.Equal(ErrorCodes.EntityNotFound, ex.Code);
        Assert.Contains("points[1]", ex.Message);
        Assert.Equal(before, await FeatureCountAsync(doc));
        Assert.Equal(120000, (await Ins.ModelInfoAsync(doc)).VolumeMm3!.Value, V);
    }

    // ------------------------------------------------------------------ results

    [Fact]
    public async Task A_feature_result_lists_its_new_faces_short_and_the_ids_still_resolve()
    {
        // S45: a boss on the block gives three faces; as a tool returns them they are id and surface only, and in full
        // with faceDetail. The short list is enough to go on: the cap's id takes the next sketch.
        string doc = await BlockAsync("faces_short");
        var boss = await BossAsync(doc, 0, 0);
        string brief = ResultJson.Serialize(boss);
        Assert.DoesNotContain("pickMm", brief);
        Assert.DoesNotContain("areaMm2", brief);
        var faces = System.Text.Json.JsonDocument.Parse(brief).RootElement.GetProperty("newFaces").EnumerateArray()
            .Select(f => (Id: f.GetProperty("id").GetString()!, Surface: f.GetProperty("surface").GetString()!, Fields: f.EnumerateObject().Count()))
            .ToList();
        Assert.Equal(boss.NewFaces!.Count, faces.Count);
        Assert.All(faces, f => Assert.Equal(2, f.Fields));
        Assert.Contains(faces, f => f.Surface == "cylinder");
        Assert.True(brief.Length < ResultJson.Serialize(boss, faceDetail: true).Length - 200, "the short list saves little");
        Assert.Contains("pickMm", ResultJson.Serialize(boss, faceDetail: true));

        // The boss's cap is its plane face at y = 30: found by its id alone.
        string cap = boss.NewFaces!.Single(f => f.Surface == "plane" && Math.Abs(f.PickMm[1] - 30) < 1e-6).Id;
        Assert.Contains(faces, f => f.Id == cap && f.Surface == "plane");
        await Sk.StartAsync(doc, new EntityRef { Id = cap }, false);
        await Sk.AddAsync(doc, [new SketchEntity { Kind = "circle", Center = [0, 30, 0], RadiusMm = 2 }]);
        var pin = await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 5 });
        Assert.Equal(Math.PI * 4 * 5, pin.Verified.VolumeDeltaMm3, Vq);
    }

    [Fact]
    public async Task A_blind_cut_that_leaves_the_body_passes_on_its_volume_and_a_kept_failure_takes_its_name()
    {
        // S40: a blind cut 30 deep from the top of the 20 mm block has no floor: it used to be refused ("no floor face at
        // 30 mm") and rolled back, though the cut was the one asked for.
        string doc = await BlockAsync("cut_breaks_out");
        await Sk.StartAsync(doc, new EntityRef { At = [-20, 20, 0] }, false);
        await Sk.AddAsync(doc, [new SketchEntity { Kind = "circle", Center = [-20, 20, 0], RadiusMm = 3 }]);
        var through = await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, Cut = true, DepthMm = 30, Name = "ThroughHole" });
        Assert.Equal(-Math.PI * 9 * 20, through.Verified.VolumeDeltaMm3, Vq);
        Assert.Equal("ThroughHole", through.Feature.Name);
        Assert.Contains(through.Verified.Checks!, c => c.Contains("broke out of the body"));

        // A cut that ends inside the body still shows its floor, and no such line.
        await Sk.StartAsync(doc, new EntityRef { At = [20, 20, 0] }, false);
        await Sk.AddAsync(doc, [new SketchEntity { Kind = "circle", Center = [20, 20, 0], RadiusMm = 3 }]);
        var pocket = await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, Cut = true, DepthMm = 5 });
        Assert.Equal(-Math.PI * 9 * 5, pocket.Verified.VolumeDeltaMm3, Vq);
        Assert.Contains(pocket.Verified.Checks!, c => c.Contains("floor at 5 mm"));
        Assert.DoesNotContain(pocket.Verified.Checks!, c => c.Contains("broke out"));

        // A failure kept with keepOnFailure takes the name the call gave: a boss that lies inside the block adds nothing.
        await Sk.StartAsync(doc, new EntityRef { Name = "top" }, false);
        await Sk.AddAsync(doc, [new SketchEntity { Kind = "circle", Center = [0, 0, 20], RadiusMm = 3 }]);
        var kept = await Assert.ThrowsAsync<SwException>(() =>
            Fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 10, Name = "Inner", KeepOnFailure = true }));
        Assert.Contains("kept as 'Inner' (keepOnFailure)", kept.Message);
        Assert.Contains((await Ins.FeatureListAsync(doc, false, 500, 0)).Features, f => f.Name == "Inner");
    }

    // ------------------------------------------------------------------ linear pattern

    [Fact]
    public async Task Linear_pattern_of_a_hole_along_plus_x_matches_the_oracle()
    {
        string doc = await BlockAsync("lpat_x");
        var seed = await HoleCutAsync(doc, -40, -20);
        Assert.Equal(-HoleMm3, seed.Verified.VolumeDeltaMm3, Vq);
        var r = await Pf.LinearPatternAsync(new LinearPatternRequest
        {
            Doc = doc, Features = [seed.Feature.Name], Direction = "+x", Count = 4, SpacingMm = 20,
        });
        Assert.Equal(3 * seed.Verified.VolumeDeltaMm3, r.Verified.VolumeDeltaMm3, Vq); // -1 696.46 mm³
        Assert.Equal("LPattern", r.Feature.Type);
        Assert.Equal([1, 0, 0], r.DirectionModel!);
        Assert.Contains(r.Verified.Checks!, c => c.Contains("expected positions: 3/3"));
        await AssertHoleAxesAsync([-40, -20, 0, 20], doc, 0);
    }

    [Fact]
    public async Task Linear_pattern_toward_minus_x_runs_the_requested_way()
    {
        string doc = await BlockAsync("lpat_minus_x");
        var seed = await HoleCutAsync(doc, 40, -20);
        var r = await Pf.LinearPatternAsync(new LinearPatternRequest
        {
            Doc = doc, Features = [seed.Feature.Name], Direction = "-x", Count = 3, SpacingMm = 30,
        });
        Assert.Equal(2 * seed.Verified.VolumeDeltaMm3, r.Verified.VolumeDeltaMm3, Vq);
        Assert.Equal([-1, 0, 0], r.DirectionModel!);
        await AssertHoleAxesAsync([-20, 10, 40], doc, 0);
    }

    [Fact]
    public async Task Linear_pattern_in_two_directions_fills_the_grid()
    {
        string doc = await BlockAsync("lpat_grid");
        var seed = await HoleCutAsync(doc, -40, -20);
        var r = await Pf.LinearPatternAsync(new LinearPatternRequest
        {
            Doc = doc, Features = [seed.Feature.Name], Direction = "+x", Count = 3, SpacingMm = 20,
            Direction2 = "+z", Count2 = 2, Spacing2Mm = 30,
        });
        Assert.Equal(5 * seed.Verified.VolumeDeltaMm3, r.Verified.VolumeDeltaMm3, Vq);
        Assert.Contains(r.Verified.Checks!, c => c.Contains("expected positions: 5/5"));
        await AssertHoleAxesAsync([-40, -40, -20, -20, 0, 0], doc, 0);
        await AssertHoleAxesAsync([-20, -20, -20, 10, 10, 10], doc, 2);
    }

    [Fact]
    public async Task Linear_pattern_runs_direction_2_the_requested_way_when_its_own_way_leaves_the_part()
    {
        // S41: from z = 20 the second row belongs at z = -10. SOLIDWORKS' own sense puts it at z = 50, off the block, where
        // the cuts leave no faces: it used to come back ok with two rows' worth of holes missing and two warnings.
        string doc = await BlockAsync("lpat_grid_minus_z");
        var seed = await HoleCutAsync(doc, -40, 20);
        var r = await Pf.LinearPatternAsync(new LinearPatternRequest
        {
            Doc = doc, Features = [seed.Feature.Name], Direction = "+x", Count = 3, SpacingMm = 20,
            Direction2 = "-z", Count2 = 2, Spacing2Mm = 30,
        });
        Assert.Equal(5 * seed.Verified.VolumeDeltaMm3, r.Verified.VolumeDeltaMm3, Vq);
        Assert.Contains(r.Verified.Checks!, c => c.Contains("expected positions: 5/5"));
        Assert.Null(r.Warnings);
        await AssertHoleAxesAsync([-40, -40, -20, -20, 0, 0], doc, 0);
        await AssertHoleAxesAsync([-10, -10, -10, 20, 20, 20], doc, 2);

        // The requested sense is the one that leaves the part: toward +z from z = 20. The other way is tried, fits, and is
        // not taken; the result is the pattern as asked, with its warning.
        string doc2 = await BlockAsync("lpat_grid_off");
        var seed2 = await HoleCutAsync(doc2, -40, 20);
        var off = await Pf.LinearPatternAsync(new LinearPatternRequest
        {
            Doc = doc2, Features = [seed2.Feature.Name], Direction = "+x", Count = 3, SpacingMm = 20,
            Direction2 = "+z", Count2 = 2, Spacing2Mm = 30,
        });
        Assert.Equal(2 * seed2.Verified.VolumeDeltaMm3, off.Verified.VolumeDeltaMm3, Vq);
        Assert.Contains(off.Warnings!, w => w.Contains("instance faces were not found"));
        Assert.Contains(off.Verified.Checks!, c => c.Contains("it is the requested one that leaves instances out"));
        await AssertHoleAxesAsync([20, 20, 20], doc2, 2);
    }

    [Fact]
    public async Task Linear_pattern_along_a_slanted_edge_takes_the_side_the_axis_names()
    {
        // S41 (b): a 10 mm chamfer on the block's upright corner edge leaves a top edge from (40, 20, 30) to (50, 20, 20).
        // 'direction' with a reference says which way along it; "-x" was refused as not parallel, with a hint to pass "+".
        string doc = await BlockAsync("lpat_slanted");
        await Pf.ChamferAsync(new ChamferRequest { Doc = doc, Edges = [new EntityRef { At = [50, 10, 30] }], DistanceMm = 10 });
        var seed = await HoleCutAsync(doc, 0, 0);
        var slanted = new EntityRef { Type = "edge", At = [45, 20, 25] };
        double step = 20 / Math.Sqrt(2);
        var r = await Pf.LinearPatternAsync(new LinearPatternRequest
        {
            Doc = doc, Features = [seed.Feature.Name], DirectionRef = slanted, Direction = "-x", Count = 2, SpacingMm = 20,
        });
        Assert.Equal(seed.Verified.VolumeDeltaMm3, r.Verified.VolumeDeltaMm3, Vq);
        Assert.Equal(-Math.Sqrt(0.5), r.DirectionModel![0], 1e-6);
        Assert.Equal(Math.Sqrt(0.5), r.DirectionModel![2], 1e-6);
        await AssertHoleAxesAsync([Math.Round(-step, 3), 0], doc, 0);
        await AssertHoleAxesAsync([0, Math.Round(step, 3)], doc, 2);

        // Square to the reference, an axis can't say which way.
        var square = await Assert.ThrowsAsync<SwException>(() => Pf.LinearPatternAsync(new LinearPatternRequest
        {
            Doc = doc, Features = [seed.Feature.Name], DirectionRef = slanted, Direction = "+y", Count = 2, SpacingMm = 20,
        }));
        Assert.Equal(ErrorCodes.InvalidArgument, square.Code);
        Assert.DoesNotContain("\"+\"/\"-\"", square.Hint!);
    }

    [Fact]
    public async Task Linear_pattern_along_an_edge_reference()
    {
        string doc = await BlockAsync("lpat_edge");
        var seed = await HoleCutAsync(doc, -40, 0);
        var r = await Pf.LinearPatternAsync(new LinearPatternRequest
        {
            Doc = doc, Features = [seed.Feature.Name], DirectionRef = new EntityRef { Type = "edge", At = [0, 20, 30] }, Direction = "+x",
            Count = 3, SpacingMm = 25,
        });
        Assert.Equal(2 * seed.Verified.VolumeDeltaMm3, r.Verified.VolumeDeltaMm3, Vq);
        Assert.Equal([1, 0, 0], r.DirectionModel!);
        await AssertHoleAxesAsync([-40, -15, 10], doc, 0);
        Assert.Equal("edge", Assert.Single(r.Resolved!).Type);
    }

    [Fact]
    public async Task Linear_pattern_of_a_boss_adds_its_volume_per_instance()
    {
        string doc = await BlockAsync("lpat_boss");
        var seed = await BossAsync(doc, -40, 0);
        var r = await Pf.LinearPatternAsync(new LinearPatternRequest
        {
            Doc = doc, Features = [seed.Feature.Name], Direction = "+x", Count = 3, SpacingMm = 25,
        });
        Assert.Equal(2 * Math.PI * 25 * 10, r.Verified.VolumeDeltaMm3, Vq); // 1 570.80 mm³
        Assert.Equal("added", r.Verified.Material);
        await AssertHoleAxesAsync([-40, -15, 10], doc, 0, 5);
    }

    [Fact]
    public async Task Linear_pattern_rejects_bad_input_and_rolls_back_instances_off_the_part()
    {
        string doc = await BlockAsync("lpat_bad");
        var seed = await HoleCutAsync(doc, -40, -20);
        var none = await Assert.ThrowsAsync<SwException>(() => Pf.LinearPatternAsync(new LinearPatternRequest
        {
            Doc = doc, Features = ["NoSuchFeature"], Direction = "+x", Count = 3, SpacingMm = 20,
        }));
        Assert.Equal(ErrorCodes.EntityNotFound, none.Code);
        var one = await Assert.ThrowsAsync<SwException>(() => Pf.LinearPatternAsync(new LinearPatternRequest
        {
            Doc = doc, Features = [seed.Feature.Name], Direction = "+x", Count = 1, SpacingMm = 20,
        }));
        Assert.Equal(ErrorCodes.InvalidArgument, one.Code);
        var diagonal = await Assert.ThrowsAsync<SwException>(() => Pf.LinearPatternAsync(new LinearPatternRequest
        {
            Doc = doc, Features = [seed.Feature.Name], Direction = "1,0,1", Count = 3, SpacingMm = 20,
        }));
        Assert.Equal(ErrorCodes.InvalidArgument, diagonal.Code);

        int before = await FeatureCountAsync(doc);
        double vol = (await Ins.ModelInfoAsync(doc)).VolumeMm3!.Value;
        var away = await Assert.ThrowsAsync<SwException>(() => Pf.LinearPatternAsync(new LinearPatternRequest
        {
            Doc = doc, Features = [seed.Feature.Name], Direction = "+z", Count = 2, SpacingMm = 200, // lands outside the block
        }));
        AssertFailedAndRolledBack(away);
        Assert.Equal(before, await FeatureCountAsync(doc));
        Assert.Equal(vol, (await Ins.ModelInfoAsync(doc)).VolumeMm3!.Value, V);
    }

    // ------------------------------------------------------------------ circular pattern

    [Fact]
    public async Task Circular_pattern_of_a_hole_around_the_disc_face_matches_the_oracle()
    {
        string doc = await DiscAsync("cpat");
        var seed = await HoleCutAsync(doc, 30, 0);
        Assert.Equal(-Math.PI * 9 * 10, seed.Verified.VolumeDeltaMm3, Vq);
        var r = await Pf.CircularPatternAsync(new CircularPatternRequest
        {
            Doc = doc, Features = [seed.Feature.Name], Axis = new EntityRef { Type = "face", At = [40, 5, 0] }, Count = 6,
        });
        Assert.Equal(5 * seed.Verified.VolumeDeltaMm3, r.Verified.VolumeDeltaMm3, Vq); // -1 413.72 mm³
        Assert.Equal("CirPattern", r.Feature.Type);
        Assert.Contains(r.Verified.Checks!, c => c.Contains("expected positions: 5/5"));
        // Six holes 60° apart on r 30: x = 30 cos(k·60°).
        await AssertHoleAxesAsync([-30, -15, -15, 15, 15, 30], doc, 0);
    }

    [Fact]
    public async Task Circular_pattern_over_90_degrees_around_a_circular_edge()
    {
        string doc = await DiscAsync("cpat_edge");
        var seed = await HoleCutAsync(doc, 30, 0);
        var r = await Pf.CircularPatternAsync(new CircularPatternRequest
        {
            Doc = doc, Features = [seed.Feature.Name], Axis = new EntityRef { Type = "edge", At = [40, 10, 0] }, Count = 3,
            TotalAngleDeg = 90,
        });
        Assert.Equal(2 * seed.Verified.VolumeDeltaMm3, r.Verified.VolumeDeltaMm3, Vq);
        Assert.Contains(r.Verified.Checks!, c => c.Contains("expected positions: 2/2"));
        // Instances at 45° and 90° from the seed (either sense): |x| = 30 cos 45° and 0.
        var xs = (await HoleAxesAsync(doc, 0)).Select(Math.Abs).OrderBy(v => v).ToArray();
        Assert.Equal([0, Math.Round(30 * Math.Cos(Math.PI / 4), 3), 30], xs);
    }

    [Fact]
    public async Task Circular_pattern_rejects_a_planar_axis_one_instance_and_overlapping_angles()
    {
        string doc = await DiscAsync("cpat_bad");
        var seed = await HoleCutAsync(doc, 30, 0);
        int before = await FeatureCountAsync(doc);
        var planar = await Assert.ThrowsAsync<SwException>(() => Pf.CircularPatternAsync(new CircularPatternRequest
        {
            Doc = doc, Features = [seed.Feature.Name], Axis = new EntityRef { Type = "face", At = [0, 10, 0] }, Count = 4,
        }));
        Assert.Equal(ErrorCodes.InvalidArgument, planar.Code);
        var one = await Assert.ThrowsAsync<SwException>(() => Pf.CircularPatternAsync(new CircularPatternRequest
        {
            Doc = doc, Features = [seed.Feature.Name], Axis = new EntityRef { Type = "face", At = [40, 5, 0] }, Count = 1,
        }));
        Assert.Equal(ErrorCodes.InvalidArgument, one.Code);
        var wrap = await Assert.ThrowsAsync<SwException>(() => Pf.CircularPatternAsync(new CircularPatternRequest
        {
            Doc = doc, Features = [seed.Feature.Name], Axis = new EntityRef { Type = "face", At = [40, 5, 0] }, Count = 10,
            TotalAngleDeg = 60, EqualSpacing = false,
        }));
        Assert.Equal(ErrorCodes.InvalidArgument, wrap.Code);
        Assert.Equal(before, await FeatureCountAsync(doc));
    }

    // ------------------------------------------------------------------ mirror

    [Fact]
    public async Task Mirror_of_a_hole_about_the_right_plane_is_symmetric()
    {
        string doc = await BlockAsync("mirror_hole");
        var seed = await HoleCutAsync(doc, -30, -10);
        Assert.True(Math.Abs((await Ins.ModelInfoAsync(doc)).CenterOfMassMm![0]) > 1e-3); // off-centre before the mirror
        var r = await Pf.MirrorAsync(new MirrorRequest { Doc = doc, Features = [seed.Feature.Name], Plane = new EntityRef { Name = "right" } });
        Assert.Equal(seed.Verified.VolumeDeltaMm3, r.Verified.VolumeDeltaMm3, Vq);
        Assert.Equal("MirrorPattern", r.Feature.Type);
        Assert.Contains(r.Verified.Checks!, c => c.Contains("reflected"));
        await AssertHoleAxesAsync([-30, 30], doc, 0);
        Assert.Equal(0, (await Ins.ModelInfoAsync(doc)).CenterOfMassMm![0], 1e-4);
    }

    [Fact]
    public async Task Mirror_of_a_boss_about_a_planar_face_extends_the_part()
    {
        string doc = await BlockAsync("mirror_face");
        // A 20 x 10 x 20 boss flush with the +x side face (x = 50).
        await Sk.StartAsync(doc, new EntityRef { At = [40, 20, 0] }, false);
        await Sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Corners = [[30, 20, -10], [50, 20, 10]] }]);
        var boss = await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 10 });
        Assert.Equal(4000, boss.Verified.VolumeDeltaMm3, V);
        var r = await Pf.MirrorAsync(new MirrorRequest
        {
            Doc = doc, Features = [boss.Feature.Name], Plane = new EntityRef { Type = "face", At = [50, 10, 0] },
        });
        Assert.Equal(4000, r.Verified.VolumeDeltaMm3, V);
        AssertBox(r.Model.Box, [-50, 0, -30], [70, 30, 30]);
        Assert.Equal(1, r.Model.Bodies);
    }

    [Fact]
    public async Task Mirror_of_a_boss_with_a_face_merged_into_the_body_does_not_warn()
    {
        string doc = await BlockAsync("mirror_merged");
        // A rib along the −x edge of the top face, and a 20 x 10 x 20 boss against it: the boss's top merges with the
        // rib's into one face of the boss (600 + 400 mm²) and its −x face lies inside the rib, while its mirrored copy
        // stands free on the +x side.
        await Sk.StartAsync(doc, new EntityRef { At = [-45, 20, 0] }, false);
        await Sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Corners = [[-50, 20, -30], [-40, 20, 30]] }]);
        await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 10 });
        await Sk.StartAsync(doc, new EntityRef { At = [-30, 20, 0] }, false);
        await Sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Corners = [[-40, 20, -10], [-20, 20, 10]] }]);
        var boss = await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 10 });
        Assert.Equal(4000, boss.Verified.VolumeDeltaMm3, V);

        var r = await Pf.MirrorAsync(new MirrorRequest { Doc = doc, Features = [boss.Feature.Name], Plane = new EntityRef { Name = "right" } });
        Assert.Equal(4000, r.Verified.VolumeDeltaMm3, V);
        Assert.Equal(1, r.Model.Bodies);
        Assert.DoesNotContain(r.Warnings ?? [], w => w.Contains("not found where expected"));
        // Seed faces 1600 mm², the copy's 1200: every seed face found reflected, so a check, not a warning.
        Assert.DoesNotContain(r.Warnings ?? [], w => w.Contains("overlap, merge or are clipped"));
        Assert.Contains(r.Verified.Checks!, c => c.Contains("x the seed's area") && (c.Contains("0.75") || c.Contains("0,75"))); // the test host keeps the machine's culture
    }

    [Fact]
    public async Task Mirror_into_the_body_or_about_a_curved_face_is_rejected()
    {
        string doc = await BlockAsync("mirror_bad");
        var boss = await BossAsync(doc, -30, 0);
        var curved = await Assert.ThrowsAsync<SwException>(() => Pf.MirrorAsync(new MirrorRequest
        {
            Doc = doc, Features = [boss.Feature.Name], Plane = new EntityRef { Type = "face", At = [-25, 25, 0] },
        }));
        Assert.Equal(ErrorCodes.InvalidArgument, curved.Code);
        var empty = await Assert.ThrowsAsync<SwException>(() => Pf.MirrorAsync(new MirrorRequest
        {
            Doc = doc, Features = [], Plane = new EntityRef { Name = "right" },
        }));
        Assert.Equal(ErrorCodes.InvalidArgument, empty.Code);

        int before = await FeatureCountAsync(doc);
        double vol = (await Ins.ModelInfoAsync(doc)).VolumeMm3!.Value;
        // Mirrored about the top face, the boss lands inside the block: no new material.
        var inside = await Assert.ThrowsAsync<SwException>(() => Pf.MirrorAsync(new MirrorRequest
        {
            Doc = doc, Features = [boss.Feature.Name], Plane = new EntityRef { Type = "face", At = [0, 20, 20] },
        }));
        AssertFailedAndRolledBack(inside);
        Assert.Equal(before, await FeatureCountAsync(doc));
        Assert.Equal(vol, (await Ins.ModelInfoAsync(doc)).VolumeMm3!.Value, V);
    }
}
