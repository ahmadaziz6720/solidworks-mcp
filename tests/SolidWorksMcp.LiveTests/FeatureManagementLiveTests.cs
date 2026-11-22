using SolidWorksMcp.Core;
using SolidWorksMcp.Core.Model;
using SolidWorksMcp.Core.Services;
using Xunit;

namespace SolidWorksMcp.LiveTests;

/// <summary>Feature management, dimensions, rebuild, undo and units against a live SOLIDWORKS, with analytic oracles.</summary>
public class FeatureManagementLiveTests(LiveFixture fx) : LiveTest(fx)
{
    private const double V = 1e-2; // mm³: planar oracles, and the quarter-circle of a fillet

    /// <summary>An r5 fillet on a convex 100 mm edge removes 100·(r² − πr²/4) = 536.5 mm³ (DESIGN §6.3).</summary>
    private static readonly double FilletDv = (25 - 25 * Math.PI / 4) * 100;

    private FeatureManagementService M => new(S);

    private sealed record Block(string Doc, string Extrude, string Sketch, string Depth);

    /// <summary>A 100 (x) × 20 (y) × 60 (z) block on the top plane: V = 120 000 mm³, one depth dimension of 20 mm.</summary>
    private async Task<Block> BlockAsync(string name)
    {
        string doc = await NewPartAsync(name);
        var sk = new SketchService(S);
        var st = await sk.StartAsync(doc, new EntityRef { Name = "top" }, false);
        await sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Center = [0, 0], WidthMm = 100, HeightMm = 60 }]);
        var r = await new FeatureService(S).ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 20 });
        Assert.Equal(120000, r.Verified.VolumeDeltaMm3, 1e-3);
        var depth = r.Dimensions!.First(x => x.Unit == "mm" && Math.Abs(x.Value - 20) < 1e-6);
        return new Block(doc, r.Feature.Name, st.Sketch, depth.Name);
    }

    /// <summary>The r5 fillet on the block's convex top edge along x (y = 20, z = 30).</summary>
    private async Task<string> FilletAsync(string doc)
    {
        var r = await new FeatureService(S).FilletAsync(doc, [new EntityRef { At = [0, 20, 30] }], null, 5, true, null, false);
        Assert.Equal(-FilletDv, r.Verified.VolumeDeltaMm3, V);
        return r.Feature.Name;
    }

    private async Task<double> VolumeAsync(string doc) => (await new InspectService(S).ModelInfoAsync(doc)).VolumeMm3 ?? 0;

    // ------------------------------------------------------------------ feature_info

    [Fact]
    public async Task Feature_info_reads_the_extrude_and_fillet_definitions()
    {
        var b = await BlockAsync("fm_info");
        string fillet = await FilletAsync(b.Doc);

        var ex = await M.InfoAsync(b.Doc, b.Extrude);
        Assert.Contains(ex.Type, new[] { "Extrusion", "ICE", "Boss" });
        Assert.Null(ex.Error);
        Assert.Null(ex.Suppressed);
        Assert.Contains(b.Sketch, ex.Parents!);
        Assert.Contains(b.Sketch, ex.SubFeatures!);
        Assert.Contains(fillet, ex.Children!);
        Assert.Contains(ex.Dimensions!, x => x.Name == b.Depth && Math.Abs(x.Value - 20) < 1e-9 && x.Unit == "mm");
        var def = ex.Definition!;
        Assert.Equal("extrude", def.Kind);
        Assert.Equal("blind", def.EndCondition);
        Assert.Equal(20, def.DepthMm!.Value, 1e-6);
        Assert.Null(def.Cut);
        Assert.Null(def.DraftDeg);

        var fi = await M.InfoAsync(b.Doc, fillet);
        Assert.Equal("Fillet", fi.Type);
        Assert.Contains(b.Extrude, fi.Parents!);
        Assert.Equal("fillet", fi.Definition!.Kind);
        Assert.Equal(5, fi.Definition.RadiusMm!.Value, 1e-6);
        Assert.Equal(1, fi.Definition.EdgeCount); // read without AccessSelections (unverified that the count needs none)

        var top = await M.InfoAsync(b.Doc, "top"); // standard planes answer to front/top/right
        Assert.Equal("RefPlane", top.Type);
        Assert.Null(top.Definition);
    }

    [Fact]
    public async Task Feature_info_of_an_unknown_name_is_ENTITY_NOT_FOUND()
    {
        var b = await BlockAsync("fm_info_missing");
        var ex = await Assert.ThrowsAsync<SwException>(() => M.InfoAsync(b.Doc, "NoSuchFeature1"));
        Assert.Equal(ErrorCodes.EntityNotFound, ex.Code);
    }

    // ------------------------------------------------------------------ dimension_list

    [Fact]
    public async Task Dimension_list_returns_the_depth_in_mm_as_a_driving_dimension()
    {
        var b = await BlockAsync("fm_dimlist");
        var f = await M.DimensionListAsync(b.Doc, b.Extrude);
        var depth = Assert.Single(f.Dimensions, x => x.Name == b.Depth);
        Assert.Equal(20, depth.Value, 1e-9);
        Assert.Equal("mm", depth.Unit);
        Assert.Null(depth.Driven); // swDimensionDriving: checks the DrivenState reading that dimension_set relies on

        var all = await M.DimensionListAsync(b.Doc);
        Assert.Contains(all.Dimensions, x => x.Name == b.Depth);
        Assert.Equal(all.Dimensions.Count, all.Total);
        var sketch = await M.DimensionListAsync(b.Doc, sketch: b.Sketch);
        Assert.Equal(sketch.Dimensions.Count, sketch.Total);
    }

    [Fact]
    public async Task Dimension_list_gives_the_equation_of_a_dimension_it_drives_and_marks_only_a_reference_dimension_driven()
    {
        // S95: SOLIDWORKS reads a dimension an equation drives as driven, and the list said driven:true for it, the flag of a
        // reference dimension.
        var vs = Fx.VbaSession; // VBA makes the reference dimension: the API's DrivenState setter is the way a user does it
        string doc = await NewPartAsync("fm_dimlist_equation", vs);
        var sk = new SketchService(vs);
        var ed = new SketchEditService(vs);
        await sk.StartAsync(doc, new EntityRef { Name = "top" }, false);
        var add = await sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Center = [0, 0], WidthMm = 100, HeightMm = 60 }]);
        var sides = add.Entities[0].Sides!;
        string width = (await ed.DimensionAddAsync(doc, [new EntityRef { Name = sides["bottom"] }], "linear", valueMm: 100)).Dimension.Name;
        string height = (await ed.DimensionAddAsync(doc, [new EntityRef { Name = sides["left"] }], "vertical", valueMm: 60)).Dimension.Name;
        var vba = await new AutomationService(vs).VbaRunAsync(doc, $"Dim d As Object\nSet d = swDoc.Parameter(\"{height}\")\nEcho d.DrivenState\nd.DrivenState = 1\nEcho d.DrivenState", 20);
        Assert.Equal(["2", "1"], vba.Output); // driving (2), then driven (1)
        await sk.FinishAsync(doc);
        var p = new PropertiesService(vs);
        await p.EquationSetManyAsync(doc, [new EquationInput { Lhs = "W", Expression = "100mm" }, new EquationInput { Lhs = width, Expression = "\"W\"" }]);

        var list = (await new FeatureManagementService(vs).DimensionListAsync(doc)).Dimensions;
        var byEquation = Assert.Single(list, x => x.Name == width);
        Assert.Null(byEquation.Driven);
        Assert.Contains("\"W\"", byEquation.Equation);
        var reference = Assert.Single(list, x => x.Name == height);
        Assert.True(reference.Driven);
        Assert.Null(reference.Equation);
        Assert.Equal(60, reference.Value, 1e-9);
    }

    [Fact]
    public async Task Dimension_list_rejects_a_feature_passed_as_sketch_and_unknown_names()
    {
        var b = await BlockAsync("fm_dimlist_bad");
        var notSketch = await Assert.ThrowsAsync<SwException>(() => M.DimensionListAsync(b.Doc, sketch: b.Extrude));
        Assert.Equal(ErrorCodes.InvalidArgument, notSketch.Code);
        var missing = await Assert.ThrowsAsync<SwException>(() => M.DimensionListAsync(b.Doc, feature: "NoSuchFeature1"));
        Assert.Equal(ErrorCodes.EntityNotFound, missing.Code);
    }

    // ------------------------------------------------------------------ dimension_set

    [Fact]
    public async Task Dimension_set_changes_the_depth_and_the_volume()
    {
        var b = await BlockAsync("fm_dimset");
        var r = await M.DimensionSetAsync(b.Doc, [new DimensionValue { Name = b.Depth, Value = 30 }]);
        var c = Assert.Single(r.Changes);
        Assert.Equal(20, c.Previous, 1e-9);
        Assert.Equal(30, c.Value, 1e-9);
        Assert.Equal("mm", c.Unit);
        Assert.Equal("this", r.Configs);
        Assert.Equal(60000, r.VolumeDeltaMm3!.Value, 1e-3);
        Assert.Equal(180000, r.Model!.VolumeMm3, 1e-3);
        Assert.Equal([100, 30, 60], r.Model.Box!.Size);
        Assert.Equal(180000, await VolumeAsync(b.Doc), 1e-3);
    }

    [Fact]
    public async Task Dimension_set_refuses_bad_values_unknown_names_and_an_open_sketch()
    {
        var b = await BlockAsync("fm_dimset_bad");
        var missing = await Assert.ThrowsAsync<SwException>(() =>
            M.DimensionSetAsync(b.Doc, [new DimensionValue { Name = "D9@NoSuchFeature1", Value = 5 }]));
        Assert.Equal(ErrorCodes.EntityNotFound, missing.Code);
        var negative = await Assert.ThrowsAsync<SwException>(() => M.DimensionSetAsync(b.Doc, [new DimensionValue { Name = b.Depth, Value = -5 }]));
        Assert.Equal(ErrorCodes.InvalidArgument, negative.Code);

        // A dimension an equation drives: refused naming the equation and what it uses, with equation_set (S74).
        var p = new PropertiesService(S);
        await p.EquationSetManyAsync(b.Doc, [new EquationInput { Lhs = "DEPTH", Expression = "20mm" }, new EquationInput { Lhs = b.Depth, Expression = "\"DEPTH\"" }]);
        var byEquation = await Assert.ThrowsAsync<SwException>(() => M.DimensionSetAsync(b.Doc, [new DimensionValue { Name = b.Depth, Value = 30 }]));
        Assert.Equal(ErrorCodes.InvalidArgument, byEquation.Code);
        Assert.Contains("driven by the equation", byEquation.Message);
        Assert.Contains("\"DEPTH\"", byEquation.Message);
        Assert.Contains("equation_set: lhs 'DEPTH'", byEquation.Hint ?? "");
        await p.EquationDeleteManyAsync(b.Doc, [b.Depth, "DEPTH"]);

        var sk = new SketchService(S);
        await sk.StartAsync(b.Doc, new EntityRef { Name = "front" }, false);
        var open = await Assert.ThrowsAsync<SwException>(() => M.DimensionSetAsync(b.Doc, [new DimensionValue { Name = b.Depth, Value = 30 }]));
        Assert.Equal(ErrorCodes.SketchState, open.Code);
        await sk.AddAsync(b.Doc, [new SketchEntity { Kind = "circle", Center = [0, 0], RadiusMm = 5 }]);
        await sk.FinishAsync(b.Doc);
        Assert.Equal(120000, await VolumeAsync(b.Doc), 1e-3); // nothing changed
    }

    // ------------------------------------------------------------------ dimension_tolerance_set

    [Fact]
    public async Task Tolerances_read_back_and_fits_follow_the_ISO_tables()
    {
        var b = await BlockAsync("fm_tol");
        Task<DimensionToleranceResult> Set(ToleranceRequest q) => M.DimensionToleranceSetAsync(b.Doc, [q]);

        var bi = await Set(new ToleranceRequest { Name = b.Depth, Type = "bilateral", Upper = 0.1, Lower = -0.05 });
        var c = Assert.Single(bi.Changes);
        Assert.Equal("none", c.Previous.Type);
        Assert.Equal(("bilateral", 0.1, -0.05), (c.Tolerance.Type, c.Tolerance.Upper!.Value, c.Tolerance.Lower!.Value));
        Assert.Equal([19.95, 20.1], c.Limits!);
        var listed = Assert.Single((await M.DimensionListAsync(b.Doc, b.Extrude)).Dimensions, x => x.Name == b.Depth);
        Assert.Equal("bilateral", listed.Tolerance!.Type);

        var sym = Assert.Single((await Set(new ToleranceRequest { Name = b.Depth, Type = "symmetric", Upper = 0.2 })).Changes);
        Assert.Equal([19.8, 20.2], sym.Limits!);
        Assert.Equal("bilateral", sym.Previous.Type);
        var lim = Assert.Single((await Set(new ToleranceRequest { Name = b.Depth, Type = "limit", Upper = 0.3, Lower = 0.1 })).Changes);
        Assert.Equal([20.1, 20.3], lim.Limits!);
        Assert.Equal("basic", Assert.Single((await Set(new ToleranceRequest { Name = b.Depth, Type = "basic" })).Changes).Tolerance.Type);

        // ISO 286 for 18..30 mm: H7 = 0 / +0.021, g6 = -0.020 / -0.007 (SOLIDWORKS computes them from the class).
        var h7 = Assert.Single((await Set(new ToleranceRequest { Name = b.Depth, Type = "fit_with_tolerance", Hole = "H7" })).Changes);
        Assert.Equal("H7", h7.Tolerance.Hole);
        Assert.Equal(0.021, h7.Tolerance.Upper!.Value, 1e-9);
        Assert.Equal(0, h7.Tolerance.Lower!.Value, 1e-9);
        var g6 = Assert.Single((await Set(new ToleranceRequest { Name = b.Depth, Type = "fit_tolerance_only", Shaft = "g6" })).Changes);
        Assert.Equal(-0.007, g6.Tolerance.Upper!.Value, 1e-9);
        Assert.Equal(-0.020, g6.Tolerance.Lower!.Value, 1e-9);
        var fit = Assert.Single((await Set(new ToleranceRequest { Name = b.Depth, Type = "fit", Hole = "H7", Shaft = "g6" })).Changes);
        Assert.Equal(("H7", "g6"), (fit.Tolerance.Hole, fit.Tolerance.Shaft));

        // A rebuild keeps it; none clears it; the geometry never changes.
        await M.RebuildAsync(b.Doc, force: true);
        Assert.Equal("fit", Assert.Single((await M.DimensionListAsync(b.Doc, b.Extrude)).Dimensions, x => x.Name == b.Depth).Tolerance!.Type);
        Assert.Equal("none", Assert.Single((await Set(new ToleranceRequest { Name = b.Depth, Type = "none" })).Changes).Tolerance.Type);
        Assert.Null(Assert.Single((await M.DimensionListAsync(b.Doc, b.Extrude)).Dimensions, x => x.Name == b.Depth).Tolerance);
        Assert.Equal(120000, await VolumeAsync(b.Doc), 1e-3);
    }

    [Fact]
    public async Task Tolerances_refuse_unknown_dimensions_and_bad_entries_before_changing_anything()
    {
        var b = await BlockAsync("fm_tol_bad");
        var missing = await Assert.ThrowsAsync<SwException>(() =>
            M.DimensionToleranceSetAsync(b.Doc, [new ToleranceRequest { Name = b.Depth, Type = "basic" }, new ToleranceRequest { Name = "D9@NoSuchFeature1", Type = "basic" }]));
        Assert.Equal(ErrorCodes.EntityNotFound, missing.Code);
        var bad = await Assert.ThrowsAsync<SwException>(() =>
            M.DimensionToleranceSetAsync(b.Doc, [new ToleranceRequest { Name = b.Depth, Type = "bilateral", Upper = -0.1, Lower = 0.1 }]));
        Assert.Equal(ErrorCodes.InvalidArgument, bad.Code);
        Assert.Null(Assert.Single((await M.DimensionListAsync(b.Doc, b.Extrude)).Dimensions, x => x.Name == b.Depth).Tolerance);
    }

    [Fact]
    public async Task Dimension_set_restores_the_value_when_a_feature_breaks()
    {
        // Not with a fillet radius: SOLIDWORKS lets a fillet overflow its faces (r50 and even r1000 next to the 20 mm front
        // face rebuild without an error, VERIFIED live). A shell thicker than the part does fail.
        var b = await BlockAsync("fm_dimset_break");
        var shell = await new PartFeatureService(S).ShellAsync(new ShellRequest { Doc = b.Doc, RemoveFaces = [new EntityRef { At = [0, 20, 0] }], ThicknessMm = 2 });
        double v0 = await VolumeAsync(b.Doc);
        var t = (await M.DimensionListAsync(b.Doc, shell.Feature.Name)).Dimensions.Single(x => x.Unit == "mm");
        Assert.Equal(2, t.Value, 1e-9);
        var ex = await Assert.ThrowsAsync<SwException>(() => M.DimensionSetAsync(b.Doc, [new DimensionValue { Name = t.Name, Value = 40 }]));
        Assert.Contains(ex.Code, new[] { ErrorCodes.RebuildError, ErrorCodes.VerifyFailed, ErrorCodes.InvalidArgument });
        Assert.Equal(2, (await M.DimensionListAsync(b.Doc, shell.Feature.Name)).Dimensions.Single(x => x.Name == t.Name).Value, 1e-9);
        Assert.Equal(v0, await VolumeAsync(b.Doc), V);
        Assert.Empty((await M.RebuildAsync(b.Doc)).Errors);
    }

    [Fact]
    public async Task Sketch_finish_names_a_feature_the_redrawn_sketch_left_with_a_rebuild_warning()
    {
        // S93: the block's sketch redrawn so one of a fillet's two edges goes (its corner chamfered). The close rebuilds the
        // model and the fillet loses that edge; sketch_finish named nothing, only model_review showed it.
        var b = await BlockAsync("fm_redraw_warn");
        var fs = new FeatureService(S);
        var f = await fs.FilletAsync(b.Doc, [new EntityRef { At = [50, 10, 30] }, new EntityRef { At = [-50, 10, 30] }], null, 5, false, null, false);
        string fillet = f.Feature.Name;
        var sk = new SketchService(S);
        var ed = new SketchEditService(S);
        await ed.EditAsync(b.Doc, b.Sketch);
        await ed.ChamferAsync(b.Doc, null, [50, 0, 30], "equal", 10);
        var r = await sk.FinishAsync(b.Doc);
        var line = Assert.Single(r.Warnings ?? [], w => w.Contains("rebuild"));
        Assert.Equal($"'{fillet}' now has rebuild warning 13 (swFeatureErrorFilletNoEdge).", line);

        // A redraw that leaves every feature whole names nothing.
        await ed.EditAsync(b.Doc, b.Sketch);
        var clean = await sk.FinishAsync(b.Doc);
        Assert.DoesNotContain(clean.Warnings ?? [], w => w.Contains("rebuild"));
    }

    // ------------------------------------------------------------------ feature_delete

    [Fact]
    public async Task Feature_delete_of_a_fillet_restores_the_volume()
    {
        var b = await BlockAsync("fm_delete");
        string fillet = await FilletAsync(b.Doc);
        var r = await M.DeleteAsync(b.Doc, [fillet]);
        var row = Assert.Single(r.Removed);
        Assert.Equal(fillet, row.Name);
        Assert.Equal("requested", row.Why);
        Assert.Equal(FilletDv, r.VolumeDeltaMm3!.Value, V);
        Assert.Equal(120000, r.Model!.VolumeMm3, 1e-3);
        Assert.Null((await M.InfoAsync(b.Doc, b.Extrude)).Children);
    }

    [Fact]
    public async Task Feature_delete_refuses_a_parent_with_children_and_the_dry_run_matches_the_delete()
    {
        var b = await BlockAsync("fm_delete_parent");
        string fillet = await FilletAsync(b.Doc);
        var refused = await Assert.ThrowsAsync<SwException>(() => M.DeleteAsync(b.Doc, [b.Extrude]));
        Assert.Equal(ErrorCodes.InvalidArgument, refused.Code);
        Assert.Contains(fillet, refused.Message);

        var dry = await M.DeleteAsync(b.Doc, [b.Extrude], deleteChildren: true, deleteAbsorbed: true, dryRun: true);
        Assert.True(dry.DryRun);
        Assert.Equal(new[] { b.Extrude, fillet, b.Sketch }.Order(), dry.Removed.Select(x => x.Name).Order());
        Assert.Equal(120000 - FilletDv, await VolumeAsync(b.Doc), V); // the dry run changed nothing

        var r = await M.DeleteAsync(b.Doc, [b.Extrude], deleteChildren: true, deleteAbsorbed: true);
        Assert.Equal(dry.Removed.Select(x => (x.Name, x.Why)).Order(), r.Removed.Select(x => (x.Name, x.Why)).Order());
        Assert.Equal(0, r.Model!.Bodies);
        Assert.Equal(-(120000 - FilletDv), r.VolumeDeltaMm3!.Value, V);
    }

    [Fact]
    public async Task Feature_delete_with_children_lists_a_child_s_sketch_without_a_warning()
    {
        // A Hole Wizard hole in the block's top face: a child of the extrude, with sketches of its own that are not all
        // children of the extrude. Deleted with its parent, they go with the hole whatever deleteAbsorbed says: each is
        // listed with its why, and none is a warning (there was one per sketch, "which was not planned").
        var b = await BlockAsync("fm_delete_child_sketch");
        var hole = await new PartFeatureService(S).HoleAsync(new HoleRequest { Doc = b.Doc, Points = [[0, 20, 0]], Type = "drill", DiameterMm = 6, End = "blind", DepthMm = 5 });

        // S31: the dry run listed the hole's sketches under keptAbsorbed; the delete then removed them. They must agree.
        var dry = await M.DeleteAsync(b.Doc, [b.Extrude], deleteChildren: true, dryRun: true);
        var r = await M.DeleteAsync(b.Doc, [b.Extrude], deleteChildren: true);
        Assert.Equal(r.Removed.Select(x => (x.Name, x.Why)).Order(), dry.Removed.Select(x => (x.Name, x.Why)).Order());
        Assert.Equal(r.KeptAbsorbed, dry.KeptAbsorbed);
        Assert.Equal("requested", r.Removed.Single(x => x.Name == b.Extrude).Why);
        Assert.StartsWith("child of", r.Removed.Single(x => x.Name == hole.Feature.Name).Why);
        Assert.Contains(r.Removed, x => x.Why == $"absorbed by {hole.Feature.Name}");
        Assert.DoesNotContain(r.Removed, x => x.Why == "also removed");
        Assert.Null(r.Warnings);
        Assert.Equal(new[] { b.Sketch }, r.KeptAbsorbed!); // the requested feature's own sketch stays, as asked
    }

    [Fact]
    public async Task Feature_delete_of_a_plane_its_boss_and_the_boss_s_mirror_removes_all_three_or_nothing()
    {
        // S64 (d): a boss sketched on a plane of its own, mirrored about the right plane. Deleting the boss, its plane and
        // the mirror in one call failed VERIFY_FAILED, "'<boss>' still exist after the delete (removed: '<plane>',
        // '<mirror>')": a failure that had changed the model. And the dry run of the boss and the plane listed the mirror
        // under dependents without saying the real call would refuse.
        var b = await BlockAsync("fm_delete_plane_boss_mirror");
        var plane = await new ReferenceService(S).PlaneAsync(new RefPlaneRequest { Doc = b.Doc, Kind = "offset", Base = new EntityRef { Name = "front" }, OffsetMm = 30 });
        var sk = new SketchService(S);
        await sk.StartAsync(b.Doc, new EntityRef { Name = plane.Feature.Name }, false);
        await sk.AddAsync(b.Doc, [new SketchEntity { Kind = "rectangle", Corners = [[20, 0, 30], [40, 20, 30]] }]);
        var boss = await new FeatureService(S).ExtrudeAsync(new ExtrudeRequest { Doc = b.Doc, DepthMm = 10 });
        Assert.Equal(4000, boss.Verified.VolumeDeltaMm3, V);
        var mirror = await new PartFeatureService(S).MirrorAsync(new MirrorRequest { Doc = b.Doc, Features = [boss.Feature.Name], Plane = new EntityRef { Name = "right" } });
        Assert.Equal(4000, mirror.Verified.VolumeDeltaMm3, V);

        async Task Unchanged()
        {
            var names = (await new InspectService(S).FeatureListAsync(b.Doc, false, 500, 0)).Features.Select(f => f.Name).ToList();
            Assert.Contains(plane.Feature.Name, names);
            Assert.Contains(boss.Feature.Name, names);
            Assert.Contains(mirror.Feature.Name, names);
            Assert.Equal(128000, await VolumeAsync(b.Doc), 1e-3);
        }

        // Without the mirror: the dry run says the call would refuse, and the call refuses before removing anything.
        string[] two = [boss.Feature.Name, plane.Feature.Name];
        var dry = await M.DeleteAsync(b.Doc, two, deleteAbsorbed: true, dryRun: true);
        Assert.Equal(new[] { mirror.Feature.Name }, dry.Dependents!);
        Assert.Contains(dry.Warnings ?? [], w => w.Contains("would refuse") && w.Contains("deleteChildren") && w.Contains(mirror.Feature.Name));
        var refused = await Assert.ThrowsAsync<SwException>(() => M.DeleteAsync(b.Doc, two, deleteAbsorbed: true));
        Assert.Equal(ErrorCodes.InvalidArgument, refused.Code);
        await Unchanged();

        // All three but the boss's sketch kept: it can't stay on a plane that goes, so a dependent, refused the same way.
        string[] three = [boss.Feature.Name, plane.Feature.Name, mirror.Feature.Name];
        var keep = await M.DeleteAsync(b.Doc, three, dryRun: true);
        Assert.Contains(keep.Warnings ?? [], w => w.Contains("would refuse") && w.Contains("deleteAbsorbed"));
        var kept = await Assert.ThrowsAsync<SwException>(() => M.DeleteAsync(b.Doc, three));
        Assert.Equal(ErrorCodes.InvalidArgument, kept.Code);
        await Unchanged();

        // All three with their sketch: no deleteChildren needed, every child is in the list; the dry run matches.
        var dry3 = await M.DeleteAsync(b.Doc, three, deleteAbsorbed: true, dryRun: true);
        Assert.Null(dry3.Dependents);
        Assert.Null(dry3.Warnings);
        var r = await M.DeleteAsync(b.Doc, three, deleteAbsorbed: true);
        Assert.Equal(dry3.Removed.Select(x => (x.Name, x.Why)).Order(), r.Removed.Select(x => (x.Name, x.Why)).Order());
        Assert.Equal(4, r.Removed.Count); // the three and the boss's sketch
        Assert.Null(r.Warnings);
        Assert.Equal(-8000, r.VolumeDeltaMm3!.Value, V);
        Assert.Equal(120000, r.Model!.VolumeMm3, 1e-3);
    }

    [Fact]
    public async Task Feature_delete_of_a_feature_with_its_child_in_the_same_list_removes_both()
    {
        // S64 (d): a parent and its child named together need no deleteChildren; both go, as the dry run says.
        var b = await BlockAsync("fm_delete_parent_and_child");
        string fillet = await FilletAsync(b.Doc);
        var dry = await M.DeleteAsync(b.Doc, [b.Extrude, fillet], dryRun: true);
        Assert.Null(dry.Warnings);
        var r = await M.DeleteAsync(b.Doc, [b.Extrude, fillet]);
        Assert.Equal(dry.Removed.Select(x => (x.Name, x.Why)).Order(), r.Removed.Select(x => (x.Name, x.Why)).Order());
        Assert.Equal(new[] { b.Extrude, fillet }.Order(), r.Removed.Select(x => x.Name).Order());
        Assert.Equal(new[] { b.Sketch }, r.KeptAbsorbed!);
        Assert.Equal(0, r.Model!.Bodies);
    }

    [Fact]
    public async Task Feature_delete_of_a_sketch_another_feature_uses_refuses_before_removing_anything()
    {
        // S64 (d): with deleteAbsorbed the extrude's sketch goes too, and a second extrude made from that sketch, a body of
        // its own (not the extrude's child), is the sketch's: a dependent, refused (and said by the dry run) before
        // anything is removed.
        var b = await BlockAsync("fm_delete_shared_sketch");
        var second = await new FeatureService(S).ExtrudeAsync(new ExtrudeRequest { Doc = b.Doc, Sketch = b.Sketch, DepthMm = 5, Direction = "reverse", Merge = false });
        Assert.Equal(30000, second.Verified.VolumeDeltaMm3, V);
        var dry = await M.DeleteAsync(b.Doc, [b.Extrude], deleteAbsorbed: true, dryRun: true);
        Assert.Equal(new[] { second.Feature.Name }, dry.Dependents!);
        Assert.Contains(dry.Warnings ?? [], w => w.Contains("would refuse"));
        var ex = await Assert.ThrowsAsync<SwException>(() => M.DeleteAsync(b.Doc, [b.Extrude], deleteAbsorbed: true));
        Assert.Equal(ErrorCodes.InvalidArgument, ex.Code);
        Assert.Contains(second.Feature.Name, ex.Message);
        Assert.Equal(150000, await VolumeAsync(b.Doc), 1e-3);
        // Both extrudes and their sketch in one call: every child is in the list, so all three go.
        var both = await M.DeleteAsync(b.Doc, [b.Extrude, second.Feature.Name], deleteAbsorbed: true, dryRun: true);
        Assert.Null(both.Warnings);
        var r = await M.DeleteAsync(b.Doc, [b.Extrude, second.Feature.Name], deleteAbsorbed: true);
        Assert.Equal(both.Removed.Select(x => (x.Name, x.Why)).Order(), r.Removed.Select(x => (x.Name, x.Why)).Order());
        Assert.Contains(r.Removed, x => x.Name == b.Sketch); // and the sketch's copy the second extrude holds ("Sketch1<2>")
        Assert.Null(r.Warnings);
        Assert.Equal(0, r.Model!.Bodies);
    }

    [Fact]
    public async Task Feature_delete_keeps_the_absorbed_sketch_by_default()
    {
        var b = await BlockAsync("fm_delete_keep");
        var r = await M.DeleteAsync(b.Doc, [b.Extrude]);
        Assert.Equal(b.Extrude, Assert.Single(r.Removed).Name);
        Assert.Equal(new[] { b.Sketch }, r.KeptAbsorbed!);
        Assert.Equal("ProfileFeature", (await M.InfoAsync(b.Doc, b.Sketch)).Type);
        Assert.Equal(0, r.Model!.Bodies);
    }

    [Fact]
    public async Task Feature_delete_lists_the_equations_on_the_dimensions_that_go()
    {
        // S54 (a): the delete left the depth's equation behind, reported only as "'Equations' now has rebuild error 51",
        // and the dry run did not list it. Kept by default, listed and warned about.
        var b = await BlockAsync("fm_delete_equations");
        var p = new PropertiesService(S);
        await p.EquationSetAsync(b.Doc, b.Depth, "20mm");
        var dry = await M.DeleteAsync(b.Doc, [b.Extrude], dryRun: true);
        Assert.Contains(b.Depth, Assert.Single(dry.Equations!), StringComparison.OrdinalIgnoreCase);
        Assert.Null(dry.EquationsDeleted);
        Assert.Contains(dry.Warnings ?? [], w => w.Contains(b.Depth, StringComparison.OrdinalIgnoreCase) && w.Contains("deleteEquations"));
        var r = await M.DeleteAsync(b.Doc, [b.Extrude]);
        Assert.Equal(dry.Equations, r.Equations);
        Assert.Null(r.EquationsDeleted);
        Assert.Contains(r.Warnings ?? [], w => w.Contains(b.Depth, StringComparison.OrdinalIgnoreCase) && w.Contains("deleteEquations"));
        Assert.Single((await p.EquationListAsync(b.Doc)).Equations);
    }

    [Fact]
    public async Task Feature_delete_with_deleteEquations_removes_them_and_leaves_no_rebuild_error()
    {
        var b = await BlockAsync("fm_delete_equations_too");
        var p = new PropertiesService(S);
        await p.EquationSetAsync(b.Doc, b.Depth, "20mm");
        var dry = await M.DeleteAsync(b.Doc, [b.Extrude], dryRun: true, deleteEquations: true);
        Assert.True(dry.EquationsDeleted);
        Assert.Single((await p.EquationListAsync(b.Doc)).Equations); // the dry run changed nothing
        var r = await M.DeleteAsync(b.Doc, [b.Extrude], deleteEquations: true);
        Assert.True(r.EquationsDeleted);
        Assert.Contains(b.Depth, Assert.Single(r.Equations!), StringComparison.OrdinalIgnoreCase);
        Assert.Null(r.Warnings); // no "'Equations' now has rebuild error 51", nothing kept
        Assert.Empty((await p.EquationListAsync(b.Doc)).Equations);
    }

    [Fact]
    public async Task A_name_that_picks_up_a_deleted_feature_s_equations_warns()
    {
        // S54 (c): a new feature given a deleted one's name, or a feature renamed to it, takes over its equations unsaid.
        var b = await BlockAsync("fm_equation_pickup");
        var p = new PropertiesService(S);
        var fs = new FeatureService(S);
        await p.EquationSetAsync(b.Doc, b.Depth, "20mm");
        await M.DeleteAsync(b.Doc, [b.Extrude]);
        var named = await fs.ExtrudeAsync(new ExtrudeRequest { Doc = b.Doc, Sketch = b.Sketch, DepthMm = 10, Name = b.Extrude });
        Assert.Equal(b.Extrude, named.Feature.Name);
        Assert.Contains(named.Warnings ?? [], w => w.Contains(b.Depth, StringComparison.OrdinalIgnoreCase) && w.Contains("bind by name"));

        await M.DeleteAsync(b.Doc, [b.Extrude]);
        var other = await fs.ExtrudeAsync(new ExtrudeRequest { Doc = b.Doc, Sketch = b.Sketch, DepthMm = 10, Name = "Plate" });
        Assert.DoesNotContain(other.Warnings ?? [], w => w.Contains("bind by name")); // another name binds nothing
        var renamed = await M.RenameAsync(b.Doc, "Plate", b.Extrude);
        Assert.Contains(renamed.Warnings ?? [], w => w.Contains(b.Depth, StringComparison.OrdinalIgnoreCase) && w.Contains("bind by name"));
        Assert.Null((await M.RenameAsync(b.Doc, b.Extrude, "Plate")).Warnings); // renamed away: nothing new is bound
    }

    // ------------------------------------------------------------------ feature_suppress_set

    [Fact]
    public async Task Suppress_and_unsuppress_round_trip_with_previous_states()
    {
        var b = await BlockAsync("fm_suppress");
        string fillet = await FilletAsync(b.Doc);

        var s = await M.SuppressAsync(b.Doc, [fillet], true);
        var row = Assert.Single(s.Features);
        Assert.False(row.Previous);
        Assert.True(row.Suppressed);
        Assert.Equal("this", s.Configs);
        Assert.Equal(FilletDv, s.VolumeDeltaMm3!.Value, V);
        Assert.True((await M.InfoAsync(b.Doc, fillet)).Suppressed);

        var u = await M.SuppressAsync(b.Doc, [fillet], false);
        Assert.True(Assert.Single(u.Features).Previous);
        Assert.False(u.Features[0].Suppressed);
        Assert.Equal(-FilletDv, u.VolumeDeltaMm3!.Value, V);

        // A suppressed parent takes its dependent along; both come back in one call (parents first).
        var p = await M.SuppressAsync(b.Doc, [b.Extrude], true);
        Assert.Contains(fillet, p.AlsoChanged!);
        Assert.Equal(0, p.Model!.VolumeMm3, 1e-3);
        await M.SuppressAsync(b.Doc, [b.Extrude, fillet], false);
        Assert.Equal(120000 - FilletDv, await VolumeAsync(b.Doc), V);
    }

    [Fact]
    public async Task Suppress_refuses_system_features_and_unknown_configs()
    {
        var b = await BlockAsync("fm_suppress_bad");
        var origin = await Assert.ThrowsAsync<SwException>(() => M.SuppressAsync(b.Doc, ["Origin"], true));
        Assert.Equal(ErrorCodes.InvalidArgument, origin.Code);
        var cfg = await Assert.ThrowsAsync<SwException>(() => M.SuppressAsync(b.Doc, [b.Extrude], true, "Default"));
        Assert.Equal(ErrorCodes.InvalidArgument, cfg.Code);
        Assert.Equal(120000, await VolumeAsync(b.Doc), 1e-3);
    }

    // ------------------------------------------------------------------ feature_rename

    [Fact]
    public async Task Rename_reads_back_and_renames_the_dimensions()
    {
        var b = await BlockAsync("fm_rename");
        var r = await M.RenameAsync(b.Doc, b.Extrude, "Plate");
        Assert.Equal(b.Extrude, r.Previous);
        Assert.Equal("Plate", r.Name);
        var depth = Assert.Single(r.Dimensions!, x => x.Name.EndsWith("@Plate", StringComparison.Ordinal));
        Assert.Equal(20, depth.Value, 1e-9);
        var set = await M.DimensionSetAsync(b.Doc, [new DimensionValue { Name = depth.Name, Value = 25 }]);
        Assert.Equal(150000, set.Model!.VolumeMm3, 1e-3);
        Assert.Equal("Plate", (await M.InfoAsync(b.Doc, "plate")).Name); // names resolve ignoring case
    }

    [Fact]
    public async Task Rename_refuses_names_in_use_and_names_with_at()
    {
        var b = await BlockAsync("fm_rename_bad");
        var used = await Assert.ThrowsAsync<SwException>(() => M.RenameAsync(b.Doc, b.Extrude, b.Sketch));
        Assert.Equal(ErrorCodes.InvalidArgument, used.Code);
        var at = await Assert.ThrowsAsync<SwException>(() => M.RenameAsync(b.Doc, b.Extrude, "a@b"));
        Assert.Equal(ErrorCodes.InvalidArgument, at.Code);
        Assert.Equal(b.Extrude, (await M.InfoAsync(b.Doc, b.Extrude)).Name);
    }

    // ------------------------------------------------------------------ feature_rollback

    [Fact]
    public async Task Rollback_to_the_extrude_hides_the_fillet_and_end_restores_it()
    {
        var b = await BlockAsync("fm_rollback");
        string fillet = await FilletAsync(b.Doc);

        var r = await M.RollbackAsync(b.Doc, b.Extrude);
        Assert.Equal($"after {b.Extrude}", r.Bar);
        Assert.Equal(new[] { fillet }, r.RolledBack!);
        Assert.Equal(FilletDv, r.VolumeDeltaMm3!.Value, V);
        Assert.Equal(120000, r.Model!.VolumeMm3, 1e-3);
        Assert.Contains(r.Warnings!, w => w.Contains("rollback bar"));
        Assert.True((await M.InfoAsync(b.Doc, fillet)).RolledBack);

        var named = await M.RollbackAsync(b.Doc, fillet, before: true); // the same position, named from below
        Assert.Equal(new[] { fillet }, named.RolledBack!);

        var end = await M.RollbackAsync(b.Doc, "end");
        Assert.Equal("end", end.Bar);
        Assert.Null(end.RolledBack);
        Assert.Equal(120000 - FilletDv, end.Model!.VolumeMm3, V);
    }

    /// <summary>
    /// S67: the bar before a sketched feature goes before the sketch it absorbs too, which the history lists just
    /// before it; the check expected that sketch active. Renamed or not, the only feature or not.
    /// </summary>
    [Fact]
    public async Task Rollback_before_the_only_extrude_rolls_it_and_its_sketch_back()
    {
        var b = await BlockAsync("fm_rollback_only");
        var r = await M.RollbackAsync(b.Doc, b.Extrude, before: true);
        Assert.Equal($"before {b.Extrude}", r.Bar);
        Assert.Equal(new[] { b.Sketch, b.Extrude }, r.RolledBack!);
        Assert.Equal(-120000, r.VolumeDeltaMm3!.Value, 1e-3);
        Assert.True((await M.InfoAsync(b.Doc, b.Extrude)).RolledBack);
        var end = await M.RollbackAsync(b.Doc, "end");
        Assert.Null(end.RolledBack);
        Assert.Equal(120000, end.Model!.VolumeMm3, 1e-3);
    }

    [Fact]
    public async Task Rollback_before_the_only_extrude_renamed_rolls_it_back()
    {
        var b = await BlockAsync("fm_rollback_only_ren");
        await M.RenameAsync(b.Doc, b.Extrude, "Plate");
        var r = await M.RollbackAsync(b.Doc, "Plate", before: true);
        Assert.Equal("before Plate", r.Bar);
        Assert.Equal(new[] { b.Sketch, "Plate" }, r.RolledBack!);
        Assert.Equal(-120000, r.VolumeDeltaMm3!.Value, 1e-3);
        var end = await M.RollbackAsync(b.Doc, "end");
        Assert.Equal(120000, end.Model!.VolumeMm3, 1e-3);
    }

    [Fact]
    public async Task Rollback_before_a_second_extrude_keeps_the_first_and_its_sketch()
    {
        var b = await BlockAsync("fm_rollback_second");
        var sk = new SketchService(S);
        var st = await sk.StartAsync(b.Doc, new EntityRef { At = [0, 20, 0] }, false);
        await sk.AddAsync(b.Doc, [new SketchEntity { Kind = "rectangle", Center = [0, 0], WidthMm = 10, HeightMm = 10 }]);
        var boss = await new FeatureService(S).ExtrudeAsync(new ExtrudeRequest { Doc = b.Doc, DepthMm = 5 });
        Assert.Equal(500, boss.Verified.VolumeDeltaMm3, 1e-3);

        var r = await M.RollbackAsync(b.Doc, boss.Feature.Name, before: true);
        Assert.Equal(new[] { st.Sketch, boss.Feature.Name }, r.RolledBack!);
        Assert.Equal(-500, r.VolumeDeltaMm3!.Value, 1e-3);
        Assert.Null((await M.InfoAsync(b.Doc, b.Extrude)).RolledBack);
        await M.RollbackAsync(b.Doc, "end");
    }

    [Fact]
    public async Task Rollback_refuses_unknown_features_and_before_end()
    {
        var b = await BlockAsync("fm_rollback_bad");
        var missing = await Assert.ThrowsAsync<SwException>(() => M.RollbackAsync(b.Doc, "NoSuchFeature1"));
        Assert.Equal(ErrorCodes.EntityNotFound, missing.Code);
        var beforeEnd = await Assert.ThrowsAsync<SwException>(() => M.RollbackAsync(b.Doc, "end", before: true));
        Assert.Equal(ErrorCodes.InvalidArgument, beforeEnd.Code);
    }

    // ------------------------------------------------------------------ doc_rebuild

    [Fact]
    public async Task Rebuild_of_a_clean_part_reports_no_errors()
    {
        var b = await BlockAsync("fm_rebuild");
        var r = await M.RebuildAsync(b.Doc);
        Assert.True(r.Rebuilt);
        Assert.Empty(r.Errors);
        Assert.Null(r.Forced);
        Assert.Equal(120000, r.Model!.VolumeMm3, 1e-3);
        var forced = await M.RebuildAsync(b.Doc, force: true);
        Assert.True(forced.Forced);
        Assert.Empty(forced.Errors);
        Assert.Equal(120000, forced.Model!.VolumeMm3, 1e-3);
    }

    [Fact]
    public async Task Rebuild_refuses_while_a_sketch_is_open()
    {
        var b = await BlockAsync("fm_rebuild_sketch");
        var sk = new SketchService(S);
        await sk.StartAsync(b.Doc, new EntityRef { Name = "front" }, false);
        var ex = await Assert.ThrowsAsync<SwException>(() => M.RebuildAsync(b.Doc));
        Assert.Equal(ErrorCodes.SketchState, ex.Code);
        await sk.AddAsync(b.Doc, [new SketchEntity { Kind = "circle", Center = [0, 0], RadiusMm = 5 }]);
        await sk.FinishAsync(b.Doc);
    }

    [Fact]
    public async Task The_servers_own_failed_kept_and_rolled_back_changes_do_not_read_as_the_users()
    {
        // S57: the call after a rolled-back failure, a kept one, a failure that made no feature, or a rollback warned
        // "'…' was modified outside this server since its last change (by the user?)", though nobody else had touched the
        // part. A change made outside the server still warns.
        const string Outside = "modified outside this server";
        var b = await BlockAsync("fm_outside");
        var other = await BlockAsync("fm_outside_other");
        var sk = new SketchService(S);
        var fs = new FeatureService(S);
        async Task<IReadOnlyList<string>> Next(string step)
        {
            var w = (await M.RebuildAsync(b.Doc)).Warnings ?? [];
            Assert.False(w.Any(x => x.Contains(Outside)), $"after {step}: {string.Join(" | ", w)}");
            return w;
        }

        // A boss inside the block adds nothing: VERIFY_FAILED, rolled back.
        await sk.StartAsync(b.Doc, new EntityRef { Name = "top" }, false);
        await sk.AddAsync(b.Doc, [new SketchEntity { Kind = "circle", Center = [0, 0, 20], RadiusMm = 3 }]);
        var rolled = await Assert.ThrowsAsync<SwException>(() => fs.ExtrudeAsync(new ExtrudeRequest { Doc = b.Doc, DepthMm = 10 }));
        Assert.Equal(ErrorCodes.VerifyFailed, rolled.Code);
        await Next("a VERIFY_FAILED rollback");

        // The same boss kept with keepOnFailure.
        await sk.StartAsync(b.Doc, new EntityRef { Name = "top" }, false);
        await sk.AddAsync(b.Doc, [new SketchEntity { Kind = "circle", Center = [10, 0, 20], RadiusMm = 3 }]);
        var kept = await Assert.ThrowsAsync<SwException>(() =>
            fs.ExtrudeAsync(new ExtrudeRequest { Doc = b.Doc, DepthMm = 10, Name = "Kept", KeepOnFailure = true }));
        Assert.Contains("keepOnFailure", kept.Message);
        await Next("a feature kept with keepOnFailure");

        // A cut from a plane 100 mm above the block, away from it, makes no feature.
        var plane = await new ReferenceService(S).PlaneAsync(new RefPlaneRequest { Doc = b.Doc, Kind = "offset", Base = new EntityRef { Name = "top" }, OffsetMm = 100 });
        await sk.StartAsync(b.Doc, new EntityRef { Name = plane.Feature.Name }, false);
        await sk.AddAsync(b.Doc, [new SketchEntity { Kind = "circle", Center = [0, 100, 0], RadiusMm = 3 }]);
        var none = await Assert.ThrowsAsync<SwException>(() =>
            fs.ExtrudeAsync(new ExtrudeRequest { Doc = b.Doc, Cut = true, DepthMm = 10, Direction = "normal" }));
        Assert.True(none.Code is ErrorCodes.FeatureFailed or ErrorCodes.VerifyFailed, none.ToAgentText());
        await Next($"a failure that made no feature ({none.Code})");

        // feature_rollback, then a call on another part, so the next call re-activates this one.
        await M.RollbackAsync(b.Doc, b.Extrude);
        await M.RollbackAsync(b.Doc, "end");
        await M.RebuildAsync(other.Doc);
        var w = await Next("feature_rollback and a re-activation");
        Assert.Contains(w, x => x.StartsWith("Re-activated", StringComparison.Ordinal));

        // A change the server did not make still warns.
        Assert.Equal(25, (await TestHooks.SetDimensionRawAsync(S, b.Doc, b.Depth, 25, asUser: true)).Mm, 1e-6);
        var after = (await M.RebuildAsync(b.Doc)).Warnings ?? [];
        Assert.Contains(after, x => x.Contains(Outside));
    }

    // ------------------------------------------------------------------ doc_undo

    [Fact]
    public async Task Undo_after_a_fillet_removes_it()
    {
        var b = await BlockAsync("fm_undo");
        string fillet = await FilletAsync(b.Doc);
        var r = await M.UndoAsync(b.Doc, 1);
        Assert.Equal(1, r.Steps);
        Assert.Equal(new[] { fillet }, r.Removed!);
        Assert.Equal(FilletDv, r.VolumeDeltaMm3!.Value, V);
        Assert.Equal(120000, await VolumeAsync(b.Doc), 1e-3);
        Assert.Null(r.OpenSketch);
    }

    [Fact]
    public async Task Undo_never_goes_beyond_the_servers_own_steps()
    {
        var b = await BlockAsync("fm_undo_limit"); // sketch_start, sketch_add, feature_extrude: at most 3 server steps
        var tooMany = await Assert.ThrowsAsync<SwException>(() => M.UndoAsync(b.Doc, 20));
        Assert.Equal(ErrorCodes.InvalidArgument, tooMany.Code);
        Assert.Contains("steps", tooMany.Hint!);
        var range = await Assert.ThrowsAsync<SwException>(() => M.UndoAsync(b.Doc, 0));
        Assert.Equal(ErrorCodes.InvalidArgument, range.Code);
        Assert.Equal(120000, await VolumeAsync(b.Doc), 1e-3);
    }

    [Fact]
    public async Task Undo_refuses_after_a_change_that_left_no_undo_step()
    {
        // S79 (b): equations, custom properties and a rename leave no SOLIDWORKS undo step, so EditUndo2 took back the boss
        // before them (volumeDeltaMm3 -500, reported ok) and left them in place.
        string doc = await NewPartAsync("fm_undo_nostep");
        var sk = new SketchService(S);
        await sk.StartAsync(doc, new EntityRef { Name = "top" }, false);
        await sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Center = [0, 0], WidthMm = 10, HeightMm = 10 }]);
        var boss = await new FeatureService(S).ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 5 });
        string depth = boss.Dimensions!.First(x => x.Unit == "mm").Name;
        var p = new PropertiesService(S);
        await p.EquationSetManyAsync(doc, [new EquationInput { Lhs = "T", Expression = "5mm" }, new EquationInput { Lhs = depth, Expression = "\"T\"" }]);

        var ex = await Assert.ThrowsAsync<SwException>(() => M.UndoAsync(doc, 1));
        Assert.Equal(ErrorCodes.NotSupported, ex.Code);
        Assert.Contains("equation_set", ex.Message);
        Assert.Contains("feature_extrude", ex.Message); // the step SOLIDWORKS would undo instead
        Assert.Equal(500, await VolumeAsync(doc), 1e-3);
        Assert.Contains((await p.EquationListAsync(doc)).Equations, e => e.Lhs == "T");

        await p.PropSetAsync(doc, "PartNo", "X1");
        await M.RenameAsync(doc, boss.Feature.Name, "Blok");
        ex = await Assert.ThrowsAsync<SwException>(() => M.UndoAsync(doc, 1));
        Assert.Contains("prop_set", ex.Message);
        Assert.Contains("feature_rename", ex.Message);
        Assert.Equal(500, await VolumeAsync(doc), 1e-3);

        // A change that is an undo step of its own is undone again, and the result names what came back.
        await M.SuppressAsync(doc, ["Blok"], true);
        var r = await M.UndoAsync(doc, 1);
        Assert.Equal(500, r.VolumeDeltaMm3!.Value, 1e-3);
        Assert.Equal(500, await VolumeAsync(doc), 1e-3);
    }

    [Fact]
    public async Task Undo_names_what_it_took_back_and_after_an_undo_the_server_step_next()
    {
        // S114 (c): an undone suppress returned only {"steps":1}, naming no feature. (a): after an undo, the refusal after a
        // rename said the step it would take back "isn't the server's", though it was the server's feature_extrude.
        var b = await BlockAsync("fm_undo_names");
        await M.SuppressAsync(b.Doc, [b.Extrude], true);
        var r = await M.UndoAsync(b.Doc, 1);
        Assert.Equal([b.Extrude], r.Unsuppressed ?? []);
        Assert.Equal(120000, await VolumeAsync(b.Doc), 1e-3);

        await M.DimensionSetAsync(b.Doc, [new DimensionValue { Name = b.Depth, Value = 25 }]);
        r = await M.UndoAsync(b.Doc, 1);
        Assert.Equal([b.Depth], r.Dimensions ?? []);
        Assert.Equal(-30000, r.VolumeDeltaMm3!.Value, 1e-3);

        await M.RenameAsync(b.Doc, b.Extrude, "Blok");
        var ex = await Assert.ThrowsAsync<SwException>(() => M.UndoAsync(b.Doc, 1));
        Assert.Equal(ErrorCodes.NotSupported, ex.Code);
        Assert.Contains("feature_rename", ex.Message);
        Assert.Contains("the server's feature_extrude", ex.Message);
        Assert.Equal(120000, await VolumeAsync(b.Doc), 1e-3);
    }

    [Fact]
    public async Task Undo_units_rebuild_and_delete_refuse_the_users_documents()
    {
        string title = await TestHooks.NewForeignPartAsync(S);
        try
        {
            var undo = await Assert.ThrowsAsync<SwException>(() => M.UndoAsync(title, 1));
            Assert.Equal(ErrorCodes.NotOwned, undo.Code);
            var units = await Assert.ThrowsAsync<SwException>(() => M.UnitsSetAsync(title, "ips"));
            Assert.Equal(ErrorCodes.NotOwned, units.Code);
            var rebuild = await Assert.ThrowsAsync<SwException>(() => M.RebuildAsync(title));
            Assert.Equal(ErrorCodes.NotOwned, rebuild.Code);
            var delete = await Assert.ThrowsAsync<SwException>(() => M.DeleteAsync(title, ["Origin"]));
            Assert.Equal(ErrorCodes.NotOwned, delete.Code);
            var info = await M.DimensionListAsync(title); // reading is fine
            Assert.Equal(info.Dimensions.Count, info.Total);
        }
        finally
        {
            await TestHooks.CloseRawAsync(S, title);
        }
    }

    // ------------------------------------------------------------------ doc_units_set

    [Fact]
    public async Task Units_set_switches_the_unit_system_and_tool_values_stay_in_mm()
    {
        var b = await BlockAsync("fm_units");
        var r = await M.UnitsSetAsync(b.Doc, "ips");
        Assert.Equal("MMGS", r.Previous);
        Assert.Equal("IPS", r.Units);
        Assert.True(r.Changed);
        Assert.Equal("IPS", (await new InspectService(S).ModelInfoAsync(b.Doc)).Units);
        var depth = (await M.DimensionListAsync(b.Doc, b.Extrude)).Dimensions.Single(x => x.Name == b.Depth);
        Assert.Equal(20, depth.Value, 1e-9); // still mm
        Assert.Equal(120000, await VolumeAsync(b.Doc), 1e-3);

        Assert.False((await M.UnitsSetAsync(b.Doc, "ips")).Changed);
        var back = await M.UnitsSetAsync(b.Doc, "mmgs");
        Assert.Equal("IPS", back.Previous);
        Assert.Equal("MMGS", back.Units);
    }

    [Fact]
    public async Task Units_set_rejects_unknown_unit_systems()
    {
        var b = await BlockAsync("fm_units_bad");
        var ex = await Assert.ThrowsAsync<SwException>(() => M.UnitsSetAsync(b.Doc, "furlongs"));
        Assert.Equal(ErrorCodes.InvalidArgument, ex.Code);
        var keep = await Assert.ThrowsAsync<SwException>(() => M.UnitsSetAsync(b.Doc, "template"));
        Assert.Equal(ErrorCodes.InvalidArgument, keep.Code);
        Assert.Equal("MMGS", (await new InspectService(S).ModelInfoAsync(b.Doc)).Units);
    }
}

/// <summary>The COM-free rules of the feature-management tools (FeatureManagementLogic). These run without SOLIDWORKS.</summary>
public class FeatureManagementLogicTests
{
    private const int Linear = 0, Angular = 1, Count = 2, Scalar = -1; // swDimensionParamType_e (Scalar: Unknown, no unit)

    private static string Code(Action a) => Assert.Throws<SwException>(a).Code;

    [Theory]
    [InlineData("this", 1)]
    [InlineData(null, 1)]
    [InlineData(" ALL ", 2)]
    public void Configs_map_to_the_api_options(string? configs, int expected) =>
        Assert.Equal(expected, FeatureManagementLogic.ParseConfigs(configs));

    [Fact]
    public void Configuration_names_are_not_supported() =>
        Assert.Equal(ErrorCodes.InvalidArgument, Code(() => FeatureManagementLogic.ParseConfigs("Default")));

    [Fact]
    public void Values_convert_by_dimension_type()
    {
        Assert.Equal(0.03, FeatureManagementLogic.ToApi(Linear, 30, "D1"), 12);
        Assert.Equal(Math.PI / 2, FeatureManagementLogic.ToApi(Angular, 90, "D2"), 12);
        Assert.Equal(4.0, FeatureManagementLogic.ToApi(Count, 4, "D3"));
        Assert.Equal(30.0, FeatureManagementLogic.FromApi(Linear, 0.03));
        Assert.Equal(90.0, FeatureManagementLogic.FromApi(Angular, Math.PI / 2));
        Assert.Equal(6.0, FeatureManagementLogic.FromApi(Count, 6.0000001));
        Assert.Equal("mm", FeatureManagementLogic.UnitOf(Linear));
        Assert.Equal("deg", FeatureManagementLogic.UnitOf(Angular));
        Assert.Equal("count", FeatureManagementLogic.UnitOf(Count));
        Assert.Equal("scalar", FeatureManagementLogic.UnitOf(Scalar)); // a helix's revolutions: not mm, not ×1000 (S63)
        Assert.Equal(5.527778, FeatureManagementLogic.FromApi(Scalar, 5.5277777777));
        Assert.Equal(6.5, FeatureManagementLogic.ToApi(Scalar, 6.5, "D5"));
    }

    [Theory]
    [InlineData(Linear, -1.0)]
    [InlineData(Linear, 0.0)]
    [InlineData(Linear, double.NaN)]
    [InlineData(Linear, double.PositiveInfinity)]
    [InlineData(Angular, 0.0)]
    [InlineData(Angular, 400.0)]
    [InlineData(Count, 0.0)]
    [InlineData(Count, 2.5)]
    public void Out_of_range_values_are_refused(int type, double value) =>
        Assert.Equal(ErrorCodes.InvalidArgument, Code(() => FeatureManagementLogic.ToApi(type, value, "D1@X")));

    [Fact]
    public void Read_back_compares_with_a_tight_tolerance()
    {
        Assert.True(FeatureManagementLogic.SameApi(Linear, 0.0300000000001, 0.03));
        Assert.False(FeatureManagementLogic.SameApi(Linear, 0.0301, 0.03));
        Assert.False(FeatureManagementLogic.SameApi(Linear, double.NaN, 0.03));
        Assert.True(FeatureManagementLogic.SameApi(Count, 4.0000001, 4));
        Assert.False(FeatureManagementLogic.SameApi(Count, 5, 4));
    }

    [Theory]
    [InlineData("\"D1@Boss-Extrude1\" = 20mm", "D1@Boss-Extrude1")]
    [InlineData("\"W\" = 60mm", "W")]
    [InlineData("  \"D2@Sketch1\"=\"W\"/2", "D2@Sketch1")]
    [InlineData("no assignment", null)]
    [InlineData("", null)]
    public void Equation_targets_are_the_left_hand_side(string equation, string? expected) =>
        Assert.Equal(expected, FeatureManagementLogic.EquationTarget(equation));

    [Fact]
    public void An_equation_driven_dimension_is_refused_naming_the_equation_and_what_it_uses()
    {
        // S74: the equation found by the name read back or the one given, letter case ignored; a component's equation is not
        // this document's dimension of the same short name.
        string[] equations = ["\"TILT\" = 40", "\"D1@Angle1\" = \"TILT\"", "\"D1@Boss-Extrude1\" = 20mm", "\"D1@Cut1@cube-1.Part\" = 5mm"];
        Assert.Equal(equations[1], FeatureManagementLogic.EquationOn(equations, "d1@angle1"));
        Assert.Equal(equations[2], FeatureManagementLogic.EquationOn(equations, "x", " D1@Boss-Extrude1"));
        Assert.Null(FeatureManagementLogic.EquationOn(equations, "D2@Angle1"));
        Assert.Null(FeatureManagementLogic.EquationOn(equations, "D1@Cut1"));

        var (message, hint) = FeatureManagementLogic.EquationDrivenRefusal("D1@Angle1", equations[1]);
        Assert.Equal("'D1@Angle1' is driven by the equation \"D1@Angle1\" = \"TILT\", which uses \"TILT\"; the next rebuild would " +
                     "overwrite a direct change.", message);
        Assert.Equal("Change it with equation_set: lhs 'TILT' for the value the equation uses, or lhs 'D1@Angle1' for a new expression.", hint);
        // A variable is named before a dimension the expression also uses; a constant names the equation's own left-hand side.
        Assert.Contains("lhs 'W'", FeatureManagementLogic.EquationDrivenRefusal("D2@Sketch1", "\"D2@Sketch1\" = \"D1@Sketch1\" + \"W\"").Hint);
        var constant = FeatureManagementLogic.EquationDrivenRefusal("D1@Boss-Extrude1", equations[2]);
        Assert.DoesNotContain("which uses", constant.Message);
        Assert.Contains("equation_set lhs 'D1@Boss-Extrude1' with", constant.Hint);
    }

    [Fact]
    public void A_listed_dimension_an_equation_drives_gives_the_equation_and_no_driven_flag()
    {
        // S95: SOLIDWORKS reads both an equation-driven and a reference dimension as driven; only the second keeps the flag.
        string[] equations = ["\"D1@Sketch1\" = \"W\" "];
        var tol = new ToleranceInfo { Type = "symmetric", Upper = 0.1 };
        var driven = new DimensionInfo { Name = "D1@Sketch1", Value = 30, Unit = "mm", Driven = true, Tolerance = tol };
        var row = FeatureManagementLogic.WithEquation(driven, equations);
        Assert.Null(row.Driven);
        Assert.Equal("\"D1@Sketch1\" = \"W\"", row.Equation);
        Assert.Equal((30d, "mm", tol), (row.Value, row.Unit, row.Tolerance));
        var reference = new DimensionInfo { Name = "D2@Sketch1", Value = 5, Unit = "mm", Driven = true };
        Assert.Same(reference, FeatureManagementLogic.WithEquation(reference, equations));
    }

    [Fact]
    public void New_names_are_trimmed_and_checked()
    {
        Assert.Equal("Plate", FeatureManagementLogic.ValidateNewName("  Plate "));
        Assert.Equal(ErrorCodes.InvalidArgument, Code(() => FeatureManagementLogic.ValidateNewName(" ")));
        Assert.Equal(ErrorCodes.InvalidArgument, Code(() => FeatureManagementLogic.ValidateNewName("D1@Plate")));
        Assert.Equal(ErrorCodes.InvalidArgument, Code(() => FeatureManagementLogic.ValidateNewName("a\"b")));
        Assert.Equal(ErrorCodes.InvalidArgument, Code(() => FeatureManagementLogic.ValidateNewName(new string('x', 201))));
    }

    [Fact]
    public void Feature_names_are_trimmed_and_deduplicated_ignoring_case()
    {
        Assert.Equal(["Fillet1", "Cut-Extrude1"], FeatureManagementLogic.RequireNames([" Fillet1", "fillet1", "Cut-Extrude1"], "features"));
        Assert.Equal(ErrorCodes.InvalidArgument, Code(() => FeatureManagementLogic.RequireNames([], "features")));
        Assert.Equal(ErrorCodes.InvalidArgument, Code(() => FeatureManagementLogic.RequireNames(["Fillet1", " "], "features")));
        Assert.Equal(ErrorCodes.InvalidArgument, Code(() => FeatureManagementLogic.RequireNames(null, "features")));
    }

    [Fact]
    public void Dimension_values_need_unique_names_and_finite_numbers()
    {
        var ok = FeatureManagementLogic.RequireValues([new DimensionValue { Name = " D1@Boss-Extrude1 ", Value = 30 }]);
        Assert.Equal(("D1@Boss-Extrude1", 30.0), Assert.Single(ok));
        Assert.Equal(ErrorCodes.InvalidArgument, Code(() => FeatureManagementLogic.RequireValues([])));
        Assert.Equal(ErrorCodes.InvalidArgument, Code(() => FeatureManagementLogic.RequireValues(
            [new DimensionValue { Name = "D1@A", Value = 1 }, new DimensionValue { Name = "d1@a", Value = 2 }])));
        Assert.Equal(ErrorCodes.InvalidArgument, Code(() => FeatureManagementLogic.RequireValues([new DimensionValue { Name = "D1@A", Value = double.NaN }])));
        Assert.Equal(ErrorCodes.InvalidArgument, Code(() => FeatureManagementLogic.RequireValues([new DimensionValue { Name = "", Value = 1 }])));
    }

    [Fact]
    public void Enum_names_are_snake_case()
    {
        Assert.Equal("blind", FeatureManagementLogic.EndConditionName(0));
        Assert.Equal("mid_plane", FeatureManagementLogic.EndConditionName(6));
        Assert.Equal("through_all_both", FeatureManagementLogic.EndConditionName(9));
        Assert.Equal("42", FeatureManagementLogic.EndConditionName(42));
        Assert.True(FeatureManagementLogic.HasDepth(0));
        Assert.False(FeatureManagementLogic.HasDepth(1));
        Assert.Equal("hole_blind", FeatureManagementLogic.HoleTypeName(22));
        Assert.Null(FeatureManagementLogic.HoleTypeName(-1));
        Assert.Equal("AnsiMetric", FeatureManagementLogic.HoleStandardName(1));
        Assert.Equal("equal_distance", FeatureManagementLogic.ChamferTypeName(16));
        Assert.Equal("constant_radius", FeatureManagementLogic.FilletTypeName(0));
        Assert.Equal("two_direction", FeatureManagementLogic.RevolveTypeName(2));
        Assert.Equal("counter_bore_blind", FeatureManagementLogic.Snake("CounterBoreBlind"));
    }

    [Fact]
    public void Set_value_failures_map_to_typed_errors()
    {
        Assert.Equal(ErrorCodes.InvalidArgument, FeatureManagementLogic.SetValueError(2, "D1@A", 5, "mm").Code);
        Assert.Equal(ErrorCodes.InvalidArgument, FeatureManagementLogic.SetValueError(3, "D1@A", 5, "mm").Code);
        Assert.Equal(ErrorCodes.NotSupported, FeatureManagementLogic.SetValueError(4, "D1@A", 5, "mm").Code);
        Assert.Equal(ErrorCodes.NotSupported, FeatureManagementLogic.SetValueError(5, "D1@A", 5, "mm").Code);
        Assert.Equal(ErrorCodes.FeatureFailed, FeatureManagementLogic.SetValueError(1, "D1@A", 5, "mm").Code);
    }
}
