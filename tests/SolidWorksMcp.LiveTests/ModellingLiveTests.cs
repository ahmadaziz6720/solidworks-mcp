using SolidWorksMcp.Core;
using SolidWorksMcp.Core.Model;
using SolidWorksMcp.Core.Services;
using Xunit;

namespace SolidWorksMcp.LiveTests;

/// <summary>
/// Everyday modelling (M8 WP9) against a live SOLIDWORKS (skipped without SWMCP_LIVE_TESTS=1): engraved sketch text,
/// split lines, and variable, face and full-round fillets. Oracles: the volumes of the rounds (r²(1 − π/4) per length,
/// integrated for a linear radius), the area of a split-off face, face counts.
/// </summary>
public class ModellingLiveTests(LiveFixture fx) : LiveTest(fx)
{
    private const double V = 1e-2; // mm³
    private static readonly double Corner = 1 - Math.PI / 4; // a unit quarter-round's waste: r²(1 − π/4) per length

    private SketchService Sk => new(S);
    private FeatureService Fs => new(S);

    /// <summary>A 100 (x) × <paramref name="h"/> (y) × 60 (z) block on the Top plane: x ±50, y 0..h, z ±30.</summary>
    private async Task<string> BlockAsync(string name, double h = 20)
    {
        string doc = await NewPartAsync(name);
        await Sk.StartAsync(doc, new EntityRef { Name = "top" }, false);
        await Sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Center = [0, 0], WidthMm = 100, HeightMm = 60 }]);
        await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = h });
        return doc;
    }

    private static async Task<SwException> Fails(string code, Func<Task> call)
    {
        var ex = await Assert.ThrowsAsync<SwException>(call);
        Assert.Equal(code, ex.Code);
        return ex;
    }

    /// <summary>An R12 ASCII DXF in mm: lines through the points (closed), and circles (centre x, y, radius).</summary>
    private static string Dxf(double[][] polygon, params double[][] circles)
    {
        var sb = new System.Text.StringBuilder();
        void G(int code, object value) => sb.Append(code).Append('\n').Append(Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture)).Append('\n');
        G(0, "SECTION"); G(2, "HEADER"); G(9, "$ACADVER"); G(1, "AC1009"); G(9, "$INSUNITS"); G(70, 4); G(0, "ENDSEC");
        G(0, "SECTION"); G(2, "ENTITIES");
        for (int i = 0; i < polygon.Length; i++)
        {
            var a = polygon[i];
            var b = polygon[(i + 1) % polygon.Length];
            G(0, "LINE"); G(8, "0"); G(10, a[0]); G(20, a[1]); G(30, 0.0); G(11, b[0]); G(21, b[1]); G(31, 0.0);
        }
        foreach (var c in circles) { G(0, "CIRCLE"); G(8, "0"); G(10, c[0]); G(20, c[1]); G(30, 0.0); G(40, c[2]); }
        G(0, "ENDSEC"); G(0, "EOF");
        return sb.ToString();
    }

    [Fact]
    public async Task A_dxf_rectangle_with_a_circle_imports_at_its_coordinates_and_extrudes_to_a_plate_with_a_hole()
    {
        string dxf = Path.Combine(Path.GetTempPath(), $"swmcp_{Guid.NewGuid():N}.dxf");
        File.WriteAllText(dxf, Dxf([[0, 0], [40, 0], [40, 20], [0, 20]], [20, 10, 5]));
        try
        {
            string doc = await NewPartAsync("dxf");
            var r = await new SketchImportService(S).ImportAsync(new SketchImportRequest { Doc = doc, Path = dxf });
            Assert.Equal(5, r.Segments);
            Assert.Equal(4, r.ByType["line"]);
            Assert.Equal(1, r.ByType["arc"]);
            Assert.Equal([0.0, 0.0], r.ExtentsMm[0]);
            Assert.Equal([40.0, 20.0], r.ExtentsMm[1]);
            var ex = await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 10 });
            Assert.Equal((800 - Math.PI * 25) * 10, ex.Verified.VolumeDeltaMm3, V);
            await Fails(ErrorCodes.InvalidArgument, () => new SketchImportService(S).ImportAsync(new SketchImportRequest { Doc = doc, Path = dxf, Units = "furlong" }));
        }
        finally
        {
            try { File.Delete(dxf); }
            catch (IOException) { /* best effort */ }
        }
    }

    [Fact]
    public async Task A_curve_pattern_puts_five_holes_equally_along_a_sketch_line()
    {
        string doc = await BlockAsync("curve_pattern");
        await Sk.StartAsync(doc, new EntityRef { Type = "face", At = [30, 20, 10] }, false);
        await Sk.AddAsync(doc, [new SketchEntity { Kind = "circle", Center = [-40, 0], DiameterMm = 5 }]);
        var seed = await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, Cut = true, End = "through_all" });
        await Sk.StartAsync(doc, new EntityRef { Type = "face", At = [30, 20, 10] }, false);
        var line = await Sk.AddAsync(doc, [new SketchEntity { Kind = "line", From = [-40, 0], To = [40, 0] }]);
        await Sk.FinishAsync(doc);
        var r = await new PartFeatureService(S).CurvePatternAsync(new CurvePatternRequest
        {
            Doc = doc, Features = [seed.Feature.Name], Count = 5,
            Curve = new EntityRef { Type = "sketch_segment", Name = line.Entities[0].Segments[0], Sketch = line.Sketch },
        });
        Assert.Equal(4 * -Math.PI * 6.25 * 20, r.Verified.VolumeDeltaMm3, V);
        var count = Assert.Single(r.Verified.Checks!, c => c.StartsWith("4 instance(s): axes through", StringComparison.Ordinal));
        foreach (var x in new[] { "-20", "0", "20", "40" }) Assert.Contains($"[{x}, 0, 0]", count);
    }

    [Fact]
    public async Task A_perforation_fill_cuts_whole_holes_whose_count_accounts_for_the_removed_volume()
    {
        string doc = await BlockAsync("fill_pattern", h: 2);
        var r = await new PartFeatureService(S).FillPatternAsync(new FillPatternRequest
        {
            Doc = doc, Face = new EntityRef { At = [0, 2, 0] }, SpacingMm = 10, MarginMm = 5, HoleDiameterMm = 4,
        });
        var holes = Assert.Single(r.Verified.Checks!, c => c.Contains("hole(s) of Ø4 mm", StringComparison.Ordinal));
        int n = int.Parse(holes[..holes.IndexOf(' ')], System.Globalization.CultureInfo.InvariantCulture);
        Assert.InRange(n, 20, 80); // 90 × 50 mm inside the margin at 10 mm spacing
        Assert.Equal(-n * Math.PI * 4 * 2, r.Verified.VolumeDeltaMm3, V); // each Ø4 hole through the 2 mm plate
    }

    [Fact]
    public async Task A_modelled_m6_thread_on_a_shaft_and_in_a_tapped_hole_moves_material_within_its_annulus()
    {
        // A Ø6 shaft, 20 long (y 0..20): an external M6 × 12 mm from its top end.
        string shaft = await NewPartAsync("thread_shaft");
        await Sk.StartAsync(shaft, new EntityRef { Name = "top" }, false);
        await Sk.AddAsync(shaft, [new SketchEntity { Kind = "circle", Center = [0, 0], DiameterMm = 6 }]);
        await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = shaft, DepthMm = 20 });
        var ext = await Fs.ThreadAsync(new ThreadRequest { Doc = shaft, Edge = new EntityRef { Type = "edge", At = [3, 20, 0] }, Size = "M6", DepthMm = 12 });
        Assert.Contains(ext.Verified.Checks!, c => c.StartsWith("M6x1.0 external (Metric Die), 12 mm, right-handed", StringComparison.Ordinal));
        double annulus = Math.PI / 4 * (36 - Math.Pow(6 - 1.22687, 2)) * 12;
        Assert.InRange(-ext.Verified.VolumeDeltaMm3, 0.2 * annulus, annulus);

        // A Ø5 tap drill through the 20 mm block: an internal M6 × 10 mm from its top.
        string block = await BlockAsync("thread_hole");
        await Sk.StartAsync(block, new EntityRef { Type = "face", At = [30, 20, 10] }, false);
        await Sk.AddAsync(block, [new SketchEntity { Kind = "circle", Center = [0, 0], DiameterMm = 5 }]);
        await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = block, Cut = true, End = "through_all" });
        var tap = await Fs.ThreadAsync(new ThreadRequest { Doc = block, Edge = new EntityRef { Type = "edge", At = [2.5, 20, 0] }, Size = "M6", DepthMm = 10 });
        Assert.Contains(tap.Verified.Checks!, c => c.StartsWith("M6x1.0 internal (Metric Tap), 10 mm", StringComparison.Ordinal));
        await Fails(ErrorCodes.InvalidArgument, () => Fs.ThreadAsync(new ThreadRequest { Doc = block, Edge = new EntityRef { Type = "edge", At = [0, 20, 30] }, Size = "M6", DepthMm = 5 }));
        await Fails(ErrorCodes.InvalidArgument, () => Fs.ThreadAsync(new ThreadRequest { Doc = block, Edge = new EntityRef { Type = "edge", At = [2.5, 20, 0] }, Size = "UNC 1/4", DepthMm = 5 }));
    }

    [Fact]
    public async Task Sketch_text_at_its_height_engraves_the_top_face_with_a_cut()
    {
        string doc = await BlockAsync("text");
        await Sk.StartAsync(doc, new EntityRef { Type = "face", At = [30, 20, 10] }, false);
        await Fails(ErrorCodes.InvalidArgument, () => Sk.AddAsync(doc, [new SketchEntity { Kind = "text", Text = "MCP", At = [-30, -5] }])); // no height
        var add = await Sk.AddAsync(doc, [new SketchEntity { Kind = "text", Text = "MCP 42", At = [-30, -5], HeightMm = 10 }]);
        var e = Assert.Single(add.Entities);
        Assert.Contains("1 text created", e.Checks!);
        Assert.Contains(e.Checks!, c => c.StartsWith("text 'MCP 42', 10 mm high", StringComparison.Ordinal));
        Assert.Equal("closed", add.Contours.Status);
        var cut = await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, Cut = true, DepthMm = 1 });
        // Six characters, 10 mm high, 1 mm deep: some material, less than their 60 × 12 mm box.
        Assert.InRange(-cut.Verified.VolumeDeltaMm3, 20, 720);
    }

    [Fact]
    public async Task A_split_line_cuts_a_disc_out_of_the_top_face_without_changing_volume_or_area()
    {
        string doc = await BlockAsync("split");
        await Sk.StartAsync(doc, new EntityRef { Type = "face", At = [30, 20, 10] }, false);
        await Sk.AddAsync(doc, [new SketchEntity { Kind = "circle", Center = [0, 0], DiameterMm = 20 }]);
        var r = await Fs.SplitLineAsync(new SplitLineRequest { Doc = doc, Faces = [new EntityRef { At = [30, 20, 10] }] });
        Assert.Contains("faces 6 → 7", r.Verified.Checks!);
        Assert.Contains("volume and area unchanged", r.Verified.Checks!);
        var disc = await new MeasureService(S).MeasureAsync(doc, new EntityRef { Type = "face", At = [0, 20, 0] }, null);
        Assert.Equal(Math.PI * 100, disc.AreaMm2!.Value, 1e-3);
        await Fails(ErrorCodes.InvalidArgument, () => Fs.SplitLineAsync(new SplitLineRequest { Doc = doc, Faces = [] }));
    }

    [Fact]
    public async Task A_face_fillet_between_the_top_and_front_faces_removes_the_quarter_round_of_its_radius()
    {
        string doc = await BlockAsync("face_fillet");
        var r = await Fs.FilletAsync(new FilletRequest
        {
            Doc = doc, Kind = "face", Faces = [new EntityRef { At = [0, 20, 0] }], Faces2 = [new EntityRef { At = [0, 10, 30] }], RadiusMm = 5,
        });
        Assert.Equal(-Corner * 25 * 100, r.Verified.VolumeDeltaMm3, V);
        Assert.Contains("round face r 5 mm", r.Verified.Checks!);
        await Fails(ErrorCodes.InvalidArgument, () => Fs.FilletAsync(new FilletRequest { Doc = doc, Kind = "face", Faces = [new EntityRef { At = [0, 20, 0] }], RadiusMm = 5 }));
    }

    [Fact]
    public async Task A_full_round_turns_the_plates_end_into_a_half_cylinder()
    {
        string doc = await BlockAsync("full_round", h: 10);
        var r = await Fs.FilletAsync(new FilletRequest
        {
            Doc = doc, Kind = "full_round", Faces = [new EntityRef { At = [0, 10, 0] }], CenterFaces = [new EntityRef { At = [50, 5, 0] }],
            Faces2 = [new EntityRef { At = [0, 0, 0] }],
        });
        // The 10 mm end becomes a half round of r 5: two quarter-rounds of r 5 over the 60 mm width.
        Assert.Equal(-2 * Corner * 25 * 60, r.Verified.VolumeDeltaMm3, V);
        Assert.Contains("round face r 5 mm", r.Verified.Checks!);
    }

    [Fact]
    public async Task A_variable_fillet_grows_from_2_to_6_mm_along_the_edge_with_the_volume_of_a_linear_radius()
    {
        string doc = await BlockAsync("var_fillet");
        var r = await Fs.FilletAsync(new FilletRequest
        {
            Doc = doc, Kind = "variable", Edges = [new EntityRef { Type = "edge", At = [0, 20, 30] }], RadiiMm = [2, 6], StartAt = [-50, 20, 30],
        });
        Assert.Contains(r.Verified.Checks!, c => c.StartsWith("r 2 mm at [-50, 20, 30], r 6 mm at [50, 20, 30]", StringComparison.Ordinal)
                                                || c.StartsWith("r 6 mm at [50, 20, 30], r 2 mm at [-50, 20, 30]", StringComparison.Ordinal));
        // ∫ r(x)² (1 − π/4) dx over 100 mm with r from 2 to 6: (1 − π/4) · 100 · (4 + 12 + 36) / 3.
        Assert.Equal(-Corner * 100 * 52 / 3, r.Verified.VolumeDeltaMm3, 0.002 * Corner * 100 * 52 / 3); // 372.27 vs 371.98: 0.08 %
        await Fails(ErrorCodes.InvalidArgument, () => Fs.FilletAsync(new FilletRequest { Doc = doc, Kind = "variable", Edges = [new EntityRef { Type = "edge", At = [0, 20, -30] }], RadiiMm = [2] }));
    }

    [Fact]
    public async Task A_fillet_that_stops_where_its_edge_ends_tangent_to_one_not_in_the_call_names_that_joint()
    {
        // S64 (c): a fillet of a round alone, `propagate` off, failed at any radius with the hint "A radius too large …":
        // the round ends tangent to the top line, not in the call. A plate whose ends run in a quarter round R7 into a face
        // 30° off its tangent, 10 thick mid plane (z ±5); the rounds on the front face z = 5 end tangent to its top line.
        string doc = await NewPartAsync("tangent_joint");
        await Sk.StartAsync(doc, new EntityRef { Name = "front" }, false);
        double t = 56.5 - 3 * Math.Tan(Math.PI / 6);
        await Sk.AddAsync(doc, [
            new SketchEntity { Kind = "line", From = [-t, -5], To = [t, -5] },
            new SketchEntity { Kind = "line", From = [t, -5], To = [56.5, -2] },
            new SketchEntity { Kind = "arc", Center = [49.5, -2], RadiusMm = 7, StartDeg = 0, EndDeg = 90 },
            new SketchEntity { Kind = "line", From = [49.5, 5], To = [-49.5, 5] },
            new SketchEntity { Kind = "arc", Center = [-49.5, -2], RadiusMm = 7, StartDeg = 90, EndDeg = 180 },
            new SketchEntity { Kind = "line", From = [-56.5, -2], To = [-t, -5] },
        ]);
        await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, End = "mid_plane", DepthMm = 10 });
        double a = 7 * Math.Sqrt(0.5);
        double[] right = [49.5 + a, -2 + a, 5], left = [-49.5 - a, -2 + a, 5];
        var inspect = new InspectService(S);
        async Task<string> Id(double[] at) => Assert.Single((await inspect.EdgeListAsync(doc, new EntityFilter { Near = at, WithinMm = 0.001 }, 10, 0, false)).Edges).Id;
        string r = await Id(right), l = await Id(left), top = await Id([0, 5, 5]);

        var ex = await Fails(ErrorCodes.FeatureFailed, () => Fs.FilletAsync(doc, [new EntityRef { Id = r }], null, 1, false, null, false));
        Assert.Equal($"edge {r} ends tangent to {top}, not in the call: turn `propagate` on or add it.", ex.Hint);
        ex = await Fails(ErrorCodes.FeatureFailed, () => Fs.FilletAsync(doc, [new EntityRef { Id = r }, new EntityRef { Id = l }], null, 0.5, false, null, false));
        Assert.Equal($"edge {r} ends tangent to {top}; edge {l} ends tangent to {top}, not in the call: turn `propagate` on or add it.", ex.Hint);

        // The way out the hint names builds: the round with `propagate` on.
        var ok = await Fs.FilletAsync(doc, [new EntityRef { At = right }], null, 1, true, null, false);
        Assert.Contains("round face r 1 mm", ok.Verified.Checks!);
        Assert.Equal("removed", ok.Verified.Material);
    }

    [Fact]
    public async Task A_fillet_whose_round_reaches_past_the_faces_it_blends_warns_naming_them_and_an_ordinary_one_does_not()
    {
        // S124: R20 on the long bottom front edge of a bar of a 10 x 10 section, `propagate` off, was built overflowing the
        // faces it blends, a face consumed, reported `ok` with no warning. A bar x ±50, y 0..10, z ±5.
        string doc = await NewPartAsync("fillet_overflow");
        await Sk.StartAsync(doc, new EntityRef { Name = "top" }, false);
        await Sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Center = [0, 0], WidthMm = 100, HeightMm = 10 }]);
        await Fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 10 });
        var inspect = new InspectService(S);
        async Task<string> Face(double[] at) => Assert.Single((await inspect.FaceListAsync(doc, new EntityFilter { Near = at, WithinMm = 0.001 }, 10, 0)).Faces).Id;
        string front = await Face([0, 5, 5]), bottom = await Face([0, 0, 0]);

        // An ordinary fillet, R1 on the top back edge, well under the faces' 10 mm: no such warning.
        var ok = await Fs.FilletAsync(doc, [new EntityRef { At = [0, 10, -5] }], null, 1, false, null, false);
        Assert.Contains("round face r 1 mm", ok.Verified.Checks!);
        Assert.DoesNotContain(ok.Warnings ?? [], w => w.Contains("reaches past", StringComparison.Ordinal));

        var r = await Fs.FilletAsync(doc, [new EntityRef { At = [0, 0, 5] }], null, 20, false, null, false);
        string w = Assert.Single(r.Warnings ?? [], x => x.Contains("reaches past", StringComparison.Ordinal));
        // Both faces of the edge are gone (VERIFIED by a face listing: no face left at z = 5 or y = 0).
        Assert.StartsWith("The R20 mm fillet reaches past faces it blends: ", w);
        Assert.Contains($"{front} (plane, normal [0, 0, 1]) is 10 mm wide beside E", w);
        Assert.Contains($"{bottom} (plane, normal [0, -1, 0]) is 10 mm wide beside E", w);
        Assert.Equal(2, System.Text.RegularExpressions.Regex.Matches(w, "the round reaching 20 mm into it and is gone after the fillet").Count);
        var planes = (await inspect.FaceListAsync(doc, new EntityFilter { Surface = "plane" }, 50, 0)).Faces;
        Assert.DoesNotContain(planes, f => f.Normal is [0, 0, 1] or [0, -1, 0]);
    }
}
