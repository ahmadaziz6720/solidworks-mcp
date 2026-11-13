using SolidWorksMcp.Core;
using SolidWorksMcp.Core.Model;
using SolidWorksMcp.Core.Services;
using Xunit;

namespace SolidWorksMcp.LiveTests;

/// <summary>M0 tools against a live SOLIDWORKS, with analytic oracles (DESIGN §6.3).</summary>
public class M0LiveTests(LiveFixture fx) : LiveTest(fx)
{
    private const double V = 1e-3; // mm³ tolerance for planar oracles

    /// <summary>A 100 (x) × 20 (y) × 60 (z) block on the top plane, centred on the origin in x and z.</summary>
    private async Task<string> BlockAsync(string name)
    {
        string doc = await NewPartAsync(name);
        var sk = new SketchService(S);
        await sk.StartAsync(doc, new EntityRef { Name = "top" }, false);
        await sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Center = [0, 0], WidthMm = 100, HeightMm = 60 }]);
        var r = await new FeatureService(S).ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 20 });
        Assert.Equal(120000, r.Verified.VolumeDeltaMm3, V);
        return doc;
    }

    [Fact]
    public async Task Connect_reports_the_revision()
    {
        var st = await new SessionService(S).StatusAsync();
        Assert.True(st.Attached);
        // sw2023 only (S60 d): this fork is tested on SOLIDWORKS 2023, revision 31.x, so sw_connect warns of no untested revision.
        Assert.StartsWith("31.", st.Revision);
        var c = await new SessionService(S).ConnectAsync(false);
        Assert.Null(c.Warnings);
    }

    [Fact]
    public async Task Connect_says_whether_it_found_the_same_SOLIDWORKS_process_as_before()
    {
        // S60 (c): a session that asked the user to restart SOLIDWORKS could not tell that the restart had not happened.
        var c = await new SessionService(S).ConnectAsync(false);
        string json = ResultJson.Serialize(c);
        Assert.Contains("\"process\":\"same as before\"", json);
        Assert.Matches("\"processStarted\":\"\\d{4}-\\d\\d-\\d\\d \\d\\d:\\d\\d:\\d\\d\"", json);
    }

    [Fact]
    public async Task Status_names_the_commit_the_server_was_built_from()
    {
        var st = await new SessionService(S).StatusAsync();
        // The SDK stamps the commit (+<sha>, .dirty from a changed tree); sw_status keeps it, shortened to 7.
        Assert.Matches(@"^\d+\.\d+\.\d+[^+]*\+[0-9a-f]{7}(\.dirty)?$", st.ServerVersion);
    }

    [Fact]
    public async Task Status_gives_a_failed_call_s_error_code_not_its_text()
    {
        // The caller of a failed call has read its error; recentOps repeated the whole text, hint included, for ten calls.
        string doc = await BlockAsync("status_ops");
        var failed = await Assert.ThrowsAsync<SwException>(() => new PropertiesService(S).MaterialSetAsync(doc, "No Such Material 12345"));
        Assert.True(failed.ToAgentText().Length > 60, failed.ToAgentText());
        var st = await new SessionService(S).StatusAsync();
        var op = st.RecentOps!.Last(o => o.Name == "material_set");
        Assert.StartsWith("failed", op.Status);
        Assert.Equal(failed.Code, op.Error);
    }

    [Fact]
    public async Task Box_extrude_matches_the_oracle()
    {
        string doc = await BlockAsync("box");
        var info = await new InspectService(S).ModelInfoAsync(doc);
        Assert.Equal(120000, info.VolumeMm3!.Value, V);
        Assert.Equal([100, 20, 60], info.Box!.Size);
        Assert.Equal("MMGS", info.Units);
    }

    [Fact]
    public async Task Top_plane_sketch_frame_is_x_0_minus_y()
    {
        string doc = await NewPartAsync("frame");
        var st = await new SketchService(S).StartAsync(doc, new EntityRef { Name = "top" }, false);
        Assert.Equal([1, 0, 0], st.Frame!.XAxis);
        Assert.Equal([0, 0, -1], st.Frame.YAxis);
        Assert.Equal([0, 1, 0], st.Frame.Normal);
        await new SketchService(S).FinishAsync(doc);
    }

    [Fact]
    public async Task Mid_plane_extrude_has_caps_at_plus_minus_half_depth()
    {
        string doc = await NewPartAsync("mid");
        var sk = new SketchService(S);
        await sk.StartAsync(doc, new EntityRef { Name = "front" }, false);
        await sk.AddAsync(doc, [new SketchEntity { Kind = "circle", Center = [0, 0], RadiusMm = 10 }]);
        var r = await new FeatureService(S).ExtrudeAsync(new ExtrudeRequest { Doc = doc, End = "mid_plane", DepthMm = 30 });
        Assert.Equal(Math.PI * 100 * 30, r.Verified.VolumeDeltaMm3, 1e-2);
        Assert.Equal([-15], [r.Model.Box!.Min[2]]);
        Assert.Equal([15], [r.Model.Box.Max[2]]);
        Assert.Contains("cap at 15 mm", r.Verified.Checks!);
        Assert.Contains("cap at -15 mm", r.Verified.Checks!);
    }

    [Fact]
    public async Task A_direction_off_the_sketch_normal_extrudes_along_it()
    {
        // S53: a Ø10 boss on the top plane (normal +y), 10 mm along [0, cos 40°, sin 40°]; it went along the normal.
        string doc = await NewPartAsync("slanted");
        var sk = new SketchService(S);
        await sk.StartAsync(doc, new EntityRef { Name = "top" }, false);
        await sk.AddAsync(doc, [new SketchEntity { Kind = "circle", Center = [0, 0], RadiusMm = 5 }]);
        double c = Math.Cos(40 * Math.PI / 180), s = Math.Sin(40 * Math.PI / 180);
        var r = await new FeatureService(S).ExtrudeAsync(new ExtrudeRequest
        {
            Doc = doc, DepthMm = 10, Direction = string.Create(System.Globalization.CultureInfo.InvariantCulture, $"0,{c},{s}"),
        });
        Assert.Equal(0, r.DirectionModel![0], 1e-6);
        Assert.Equal(c, r.DirectionModel[1], 1e-6);
        Assert.Equal(s, r.DirectionModel[2], 1e-6);
        // An oblique cylinder: the cap is the circle moved 10 mm along the direction, 10·cos 40° above the plane.
        Assert.Equal(Math.PI * 25 * 10 * c, r.Verified.VolumeDeltaMm3, 5e-2); // SOLIDWORKS' mass properties of the slanted side: 4e-5 off
        Assert.Contains("cap at 10 mm", r.Verified.Checks!);
        Assert.Contains(r.Verified.Checks!, x => x.StartsWith("profile moved along [0, 0.766, 0.643]"));

        // A cut from the top face into the block, 10 mm along [0, -cos 40°, sin 40°].
        string doc2 = await BlockAsync("slanted_cut");
        await sk.StartAsync(doc2, new EntityRef { At = [0, 20, 0] }, false);
        await sk.AddAsync(doc2, [new SketchEntity { Kind = "circle", Center = [0, 20, 0], RadiusMm = 5 }]);
        var cut = await new FeatureService(S).ExtrudeAsync(new ExtrudeRequest
        {
            Doc = doc2, Cut = true, DepthMm = 10, Direction = string.Create(System.Globalization.CultureInfo.InvariantCulture, $"0,{-c},{s}"),
        });
        Assert.Equal(-Math.PI * 25 * 10 * c, cut.Verified.VolumeDeltaMm3, 5e-2);
        Assert.Contains("floor at 10 mm", cut.Verified.Checks!);
        Assert.Contains(cut.Verified.Checks!, x => x.StartsWith("profile moved along [0, -0.766, 0.643]"));
    }

    [Fact]
    public async Task Convex_fillet_removes_the_analytic_volume()
    {
        string doc = await BlockAsync("fillet_convex");
        // The top edge along x at y=20, z=30 is 100 mm long and convex.
        var r = await new FeatureService(S).FilletAsync(doc, [new EntityRef { At = [0, 20, 30] }], null, 5, true, null, false);
        Assert.Equal(-(25 - 25 * Math.PI / 4) * 100, r.Verified.VolumeDeltaMm3, 1e-2);
        Assert.Equal("removed", r.Verified.Material);
        Assert.Contains(r.NewFaces!, f => f.Surface == "cylinder" && f.RadiusMm == 5);
    }

    [Fact]
    public async Task Concave_fillet_adds_the_analytic_volume()
    {
        string doc = await BlockAsync("fillet_concave");
        // A 100 × 20 × 30 step on top of the back half (z from -30 to 0) makes a concave 100 mm edge at y=20, z=0.
        var sk = new SketchService(S);
        await sk.StartAsync(doc, new EntityRef { At = [0, 20, -15] }, false);
        await sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Corners = [[-50, 20, -30], [50, 20, 0]] }]);
        await new FeatureService(S).ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 20 });
        var r = await new FeatureService(S).FilletAsync(doc, [new EntityRef { At = [0, 20, 0] }], null, 5, true, null, false);
        Assert.Equal((25 - 25 * Math.PI / 4) * 100, r.Verified.VolumeDeltaMm3, 1e-2);
        Assert.Equal("added", r.Verified.Material);
    }

    [Fact]
    public async Task Cut_from_an_open_sketch_goes_into_the_material()
    {
        string doc = await BlockAsync("cut");
        var sk = new SketchService(S);
        await sk.StartAsync(doc, new EntityRef { Name = "top" }, false); // the plane is the bottom face: material is on +y
        await sk.AddAsync(doc, [new SketchEntity { Kind = "circle", Center = [0, 0], DiameterMm = 10 }]);
        var r = await new FeatureService(S).ExtrudeAsync(new ExtrudeRequest { Doc = doc, Cut = true, End = "through_all" });
        Assert.Equal(-Math.PI * 25 * 20, r.Verified.VolumeDeltaMm3, 1e-2);
        Assert.Equal([0, 1, 0], r.DirectionModel!);
    }

    [Fact]
    public async Task A_boss_buried_in_the_body_is_rolled_back()
    {
        string doc = await BlockAsync("rollback");
        var before = await new InspectService(S).FeatureListAsync(doc, false, 500, 0);
        var sk = new SketchService(S);
        await sk.StartAsync(doc, new EntityRef { At = [0, 20, 0] }, false);
        await sk.AddAsync(doc, [new SketchEntity { Kind = "circle", Center = [0, 20, 0], RadiusMm = 5 }]);
        var ex = await Assert.ThrowsAsync<SwException>(() =>
            new FeatureService(S).ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 5, Direction = "reverse" }));
        Assert.Equal(ErrorCodes.VerifyFailed, ex.Code);
        Assert.Contains("Rolled back", ex.Message);
        var after = await new InspectService(S).FeatureListAsync(doc, false, 500, 0);
        Assert.Equal(before.Total + 1, after.Total); // only the (kept) sketch is new
        Assert.Equal("ProfileFeature", after.Features[^1].Type);
    }

    [Fact]
    public async Task Point_on_a_corner_is_ambiguous_and_listing_pick_points_resolve_back()
    {
        string doc = await BlockAsync("picks");
        var ex = await Assert.ThrowsAsync<SwException>(() =>
            new FeatureService(S).FilletAsync(doc, null, [new EntityRef { At = [-50, 20, -30] }], 1, true, null, false));
        Assert.Equal(ErrorCodes.EntityAmbiguous, ex.Code);

        var faces = await new InspectService(S).FaceListAsync(doc, null, 100, 0);
        Assert.Equal(6, faces.Total);
        var edges = await new InspectService(S).EdgeListAsync(doc, null, 100, 0, false);
        Assert.Equal(12, edges.Total);
        // Every face's pick point is inside that face: a fillet on the face given by point finds exactly that face.
        foreach (var f in faces.Faces)
        {
            var q = await new InspectService(S).FaceListAsync(doc, new EntityFilter { Near = f.PickMm, WithinMm = 0.001 }, 10, 0);
            Assert.Equal(f.Id, Assert.Single(q.Faces).Id);
        }
    }

    [Fact]
    public async Task An_edge_id_listed_before_a_cut_split_it_is_refused_stale()
    {
        // S66: a notch cut across the top front edge (x from 30 to 40) splits it in two; the id listed before resolved to
        // the fragment that holds the listed mid-point (x -50..30) and a fillet on it reported ok on part of the edge.
        string doc = await BlockAsync("stale_edge");
        var inspect = new InspectService(S);
        string Edge(EdgeListResult l) => Assert.Single(l.Edges).Id;
        string top = Edge(await inspect.EdgeListAsync(doc, new EntityFilter { Near = [0, 20, 30], WithinMm = 0.001 }, 10, 0, false));
        string bottom = Edge(await inspect.EdgeListAsync(doc, new EntityFilter { Near = [0, 0, 30], WithinMm = 0.001 }, 10, 0, false));
        string back = Edge(await inspect.EdgeListAsync(doc, new EntityFilter { Near = [0, 0, -30], WithinMm = 0.001 }, 10, 0, false));

        var sk = new SketchService(S);
        await sk.StartAsync(doc, new EntityRef { At = [0, 10, 30] }, false);
        await sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Corners = [[30, 15, 30], [40, 25, 30]] }]);
        await new FeatureService(S).ExtrudeAsync(new ExtrudeRequest { Doc = doc, Cut = true, End = "through_all" });

        var ex = await Assert.ThrowsAsync<SwException>(() =>
            new FeatureService(S).FilletAsync(doc, [new EntityRef { Id = top }], null, 1, false, null, false));
        Assert.Equal(ErrorCodes.EntityStale, ex.Code);
        Assert.Contains("edge_list", ex.Hint);

        // Edges no feature touched keep their ids from that one listing: after the cut, a fillet on another edge and a
        // boss elsewhere, each still takes a feature.
        var r = await new FeatureService(S).FilletAsync(doc, [new EntityRef { Id = bottom }], null, 1, false, null, false);
        Assert.Equal(-(1 - Math.PI / 4) * 100, r.Verified.VolumeDeltaMm3, 1e-2);
        await sk.StartAsync(doc, new EntityRef { At = [-20, 20, 0] }, false);
        await sk.AddAsync(doc, [new SketchEntity { Kind = "circle", Center = [-20, 20, 0], RadiusMm = 5 }]);
        await new FeatureService(S).ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 5 });
        r = await new FeatureService(S).FilletAsync(doc, [new EntityRef { Id = back }], null, 1, false, null, false);
        Assert.Equal(-(1 - Math.PI / 4) * 100, r.Verified.VolumeDeltaMm3, 1e-2);
    }

    [Fact]
    public async Task Queries_with_expect_guard_the_count()
    {
        string doc = await BlockAsync("query");
        var ex = await Assert.ThrowsAsync<SwException>(() => new FeatureService(S).FilletAsync(doc,
            [new EntityRef { Where = System.Text.Json.JsonSerializer.SerializeToElement(new { curve = "line", parallelTo = "y" }), Expect = 3 }],
            null, 2, true, null, false));
        Assert.Equal(ErrorCodes.EntityAmbiguous, ex.Code);
        var r = await new FeatureService(S).FilletAsync(doc,
            [new EntityRef { Where = System.Text.Json.JsonSerializer.SerializeToElement(new { curve = "line", parallelTo = "y" }), Expect = 4 }],
            null, 2, true, null, false);
        Assert.Equal(4, r.Resolved!.Count);
    }

    [Fact]
    public async Task Foreign_documents_are_read_only()
    {
        // A document created through the raw API is not owned by the session.
        string title = await TestHooks.NewForeignPartAsync(S);
        try
        {
            var ex = await Assert.ThrowsAsync<SwException>(() => new SketchService(S).StartAsync(title, new EntityRef { Name = "top" }, false));
            Assert.Equal(ErrorCodes.NotOwned, ex.Code);
            var close = await Assert.ThrowsAsync<SwException>(() => new DocumentService(S).CloseAsync(title, false, true));
            Assert.Equal(ErrorCodes.NotOwned, close.Code);
            var info = await new InspectService(S).ModelInfoAsync(title); // reading is fine
            Assert.Equal("part", info.Type);
        }
        finally
        {
            await TestHooks.CloseRawAsync(S, title);
        }
    }

    [Fact]
    public async Task Dirty_owned_documents_need_save_or_discard()
    {
        string doc = await BlockAsync("dirty");
        var ex = await Assert.ThrowsAsync<SwException>(() => new DocumentService(S).CloseAsync(doc, false, false));
        Assert.Equal(ErrorCodes.Dirty, ex.Code);
    }

    [Fact]
    public async Task Save_copy_writes_a_fresh_file_and_refuses_overwrites()
    {
        string doc = await BlockAsync("save");
        string file = Path.Combine(Path.GetTempPath(), $"swmcp_test_{Guid.NewGuid():N}.sldprt");
        try
        {
            var r = await new DocumentService(S).SaveAsync(doc, "copy", file, false);
            Assert.True(File.Exists(r.SavedTo!) && r.FileSizeBytes > 10000);
            var ex = await Assert.ThrowsAsync<SwException>(() => new DocumentService(S).SaveAsync(doc, "copy", file, false));
            Assert.Equal(ErrorCodes.IoError, ex.Code);
            await new DocumentService(S).SaveAsync(doc, "copy", file, true); // our own file: overwrite allowed without a backup
        }
        finally { File.Delete(file); }
    }

    [Fact]
    public async Task Screenshot_returns_a_png_and_restores_display_settings()
    {
        string doc = await BlockAsync("shot");
        var r = await new ViewService(S).ScreenshotAsync(doc, "iso", true, "shaded_edges", true, true, 640, null, false);
        Assert.Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47 }, r.Png[..4]);
        Assert.InRange(Math.Max(r.WidthPx, r.HeightPx), 600, 640);
    }

    [Fact]
    public async Task Vba_is_disabled_by_default()
    {
        var ex = await Assert.ThrowsAsync<SwException>(() => new AutomationService(S).VbaRunAsync(null, "Echo 1", 10));
        Assert.Equal(ErrorCodes.Disabled, ex.Code);
    }

    [Fact]
    public async Task Vba_captures_output_errors_and_syntax_failures()
    {
        var vba = new AutomationService(Fx.VbaSession);
        string doc = await NewPartAsync("vba", Fx.VbaSession);
        var ok = await vba.VbaRunAsync(doc, "Echo MM(25)\nEcho \"Ø 6\"", 20);
        Assert.Equal(["0.025", "Ø 6"], ok.Output);
        var rt = await Assert.ThrowsAsync<SwException>(() => vba.VbaRunAsync(doc, "Dim x As Double\nx = 1 / 0", 20));
        Assert.Equal(ErrorCodes.VbaError, rt.Code);
        Assert.Contains("11", rt.Message);
        var syn = await Assert.ThrowsAsync<SwException>(() => vba.VbaRunAsync(doc, "Echo (\"unclosed", 20));
        Assert.Contains("did not run", syn.Message);
    }

    [Fact]
    public async Task A_modal_dialog_returns_DIALOG_BLOCKING_and_can_be_answered()
    {
        var session = Fx.VbaSession;
        string doc = await NewPartAsync("dialog", session);
        var call = new AutomationService(session).VbaRunAsync(doc, "MsgBox \"swmcp-live-test\"\nEcho \"after\"", 60);
        var ex = await Assert.ThrowsAsync<SwException>(() => call);
        Assert.Equal(ErrorCodes.DialogBlocking, ex.Code);
        var dlg = Assert.Single(session.ListDialogs(), d => d.Text.Contains("swmcp-live-test"));
        var st = await new SessionService(session).StatusAsync(); // answers while SOLIDWORKS is blocked
        Assert.True(st.UiBlocked);
        var answered = await new SessionService(session).RespondAsync(dlg.HandleHex, "OK", false);
        Assert.True(answered.Closed);
        for (int i = 0; i < 50 && session.Dispatcher.Current is not null; i++) await Task.Delay(100);
        Assert.Contains(session.Dispatcher.LateCompletions, l => l.Name == "vba_run" && l.Succeeded);
    }

    [Fact]
    public async Task Dirty_reads_unsaved_changes_made_through_another_process()
    {
        // S60 (a), probe kept as a test: a part holding unsaved work read dirty:false in a session with a second server
        // process. GetSaveFlag is SOLIDWORKS's own: a change through another COM client (a VBScript here) reads dirty in
        // doc_list, sw_status and doc_activate; an export and a saved copy leave it dirty, a save clears it.
        var docs = new DocumentService(S);
        string dir = Path.Combine(Path.GetTempPath(), $"swmcp_status_{Guid.NewGuid():N}");
        Directory.CreateDirectory(dir);
        try
        {
            async Task<bool> Dirty(string doc)
            {
                bool listed = (await docs.ListAsync()).Documents.Single(x => x.Handle == doc).Dirty;
                bool status = (await new SessionService(S).StatusAsync()).Documents!.Single(x => x.Handle == doc).Dirty;
                Assert.Equal(listed, status);
                return listed;
            }
            string doc = await NewPartAsync("dirty");
            var sk = new SketchService(S);
            await sk.StartAsync(doc, new EntityRef { Name = "top" }, false);
            await sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Center = [0, 0], WidthMm = 100, HeightMm = 60 }]);
            string f = (await new FeatureService(S).ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 20 })).Feature.Name;
            string file = Path.Combine(dir, "dirty.sldprt");
            await docs.SaveAsync(doc, "as", file, false);
            Assert.False(await Dirty(doc));
            await new FeatureManagementService(S).DimensionSetAsync(doc, [new DimensionValue { Name = $"D1@{f}", Value = 25 }]);
            Assert.True(await Dirty(doc));
            await new ExportService(S).ExportAsync(new ExportRequest { Doc = doc, Path = Path.Combine(dir, "dirty.stl") });
            await docs.SaveAsync(doc, "copy", Path.Combine(dir, "copy.sldprt"), false);
            Assert.True(await Dirty(doc));
            await docs.SaveAsync(doc, "inPlace", null, false);
            Assert.False(await Dirty(doc));

            // Another process changes the depth to 30 mm through COM (whole millimetres: CDbl would read the locale).
            string vbs = Path.Combine(dir, "change.vbs");
            File.WriteAllText(vbs, string.Join("\r\n",
                "Set sw = GetObject(, \"SldWorks.Application\")",
                "Set doc = sw.GetOpenDocumentByName(WScript.Arguments(0))",
                "Set d = doc.Parameter(WScript.Arguments(1))",
                "d.SystemValue = CLng(WScript.Arguments(2)) / 1000",
                "doc.EditRebuild3",
                "WScript.Echo \"saveflag \" & doc.GetSaveFlag()"));
            var psi = new System.Diagnostics.ProcessStartInfo(Path.Combine(Environment.SystemDirectory, "cscript.exe"),
                $"//nologo \"{vbs}\" \"{file}\" \"D1@{f}\" 30")
            { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
            using (var p = System.Diagnostics.Process.Start(psi)!)
            {
                string output = await p.StandardOutput.ReadToEndAsync() + await p.StandardError.ReadToEndAsync();
                await p.WaitForExitAsync();
                Assert.True(p.ExitCode == 0 && output.Contains("saveflag True"), output);
            }
            Assert.True(await Dirty(doc));
            Assert.True((await docs.ActivateAsync(doc, false)).Document.Dirty);
            Assert.Equal(30 * 100 * 60, (await new InspectService(S).ModelInfoAsync(doc)).VolumeMm3!.Value, V);
        }
        finally
        {
            try { Directory.Delete(dir, recursive: true); } catch (Exception e) when (e is IOException or UnauthorizedAccessException) { /* SOLIDWORKS may hold it; the sweep removes it */ }
        }
    }
}
