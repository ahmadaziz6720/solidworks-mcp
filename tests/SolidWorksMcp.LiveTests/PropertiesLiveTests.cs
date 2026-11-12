using System.Text;
using SolidWorksMcp.Core;
using SolidWorksMcp.Core.Model;
using SolidWorksMcp.Core.Services;
using Xunit;

namespace SolidWorksMcp.LiveTests;

/// <summary>The properties toolset against a live SOLIDWORKS: materials, custom properties, equations, configurations, appearance.</summary>
public class PropertiesLiveTests(LiveFixture fx) : LiveTest(fx)
{
    private const double BlockVolumeMm3 = 100 * 20 * 60; // 120 000 mm³ = 1.2e-4 m³

    private PropertiesService P => new(S);

    /// <summary>A 100 (x) × 20 (y) × 60 (z) block on the top plane; returns the document and its depth dimension (D1@Boss-Extrude1).</summary>
    private async Task<(string Doc, string Depth)> BlockAsync(string name)
    {
        string doc = await NewPartAsync(name);
        var sk = new SketchService(S);
        await sk.StartAsync(doc, new EntityRef { Name = "top" }, false);
        await sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Center = [0, 0], WidthMm = 100, HeightMm = 60 }]);
        var r = await new FeatureService(S).ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 20 });
        Assert.Equal(BlockVolumeMm3, r.Verified.VolumeDeltaMm3, 1e-3);
        // The depth is the 20 mm dimension of the extrude (D1@Boss-Extrude1).
        var depth = r.Dimensions!.Where(x => x.Unit == "mm" && Math.Abs(x.Value - 20) < 1e-9).OrderBy(x => x.Name, StringComparer.Ordinal).First();
        return (doc, depth.Name);
    }

    private static async Task<string> Code(Func<Task> call) => (await Assert.ThrowsAsync<SwException>(call)).Code;

    // ------------------------------------------------------------------ materials

    [Fact]
    public async Task A_custom_material_is_listed_assigned_and_used_by_simulation_with_its_own_stiffness()
    {
        // "Half steel": AISI 1020's density, yield and ν with E = 100 GPa. The 100 mm cantilever under 100 N bends 0.2007 mm in
        // AISI 1020, so twice that in it.
        // It goes into SOLIDWORKS' own "Custom Materials" (S28), whose own materials stay as they were.
        string name = $"MCP Test Half Steel {Guid.NewGuid().ToString("N")[..6]}";
        const string Db = "Custom Materials";
        var before = (await P.MaterialListAsync(Db, 1000, 0)).Databases.FirstOrDefault(d => d.Name == Db);
        Assert.SkipWhen(before is null, "SOLIDWORKS' Custom Materials library is not in the custom material folder on this machine.");
        string file = before!.Path;
        byte[] original = File.ReadAllBytes(file);
        string backup = MaterialXml.BackupPath(file);
        bool hadBackup = File.Exists(backup);
        var theirs = (await P.MaterialListAsync(null, 1000, 0)).Materials.Where(m => m.Database == Db).ToList();
        Assert.NotEmpty(theirs);
        try
        {
            // Into a classification the library already has, so the only new lines are the material's.
            var r = await P.MaterialAddAsync(new MaterialAddRequest
            {
                Name = name, Classification = theirs[0].Classification, DensityKgM3 = 7900, ElasticModulusGpa = 100, PoissonRatio = 0.29,
                YieldMpa = 351.571, TensileMpa = 420.507,
            });
            Assert.Equal(Db, r.Database);
            Assert.Equal(file, r.Path);
            Assert.False(r.CreatedDatabase);
            Assert.Equal(backup, r.Backup);
            if (!hadBackup) Assert.Equal(original, File.ReadAllBytes(backup)); // the library as it was, before the first write
            // The file is what it was, byte for byte, but for the new material's lines (scenario test of build 134233, point 2).
            string was = Encoding.Unicode.GetString(original).TrimStart('﻿'), now = File.ReadAllText(file);
            int at = now.IndexOf($"<material name=\"{name}\"", StringComparison.Ordinal);
            int from = now.LastIndexOf('\n', at) + 1, to = now.IndexOf('\n', now.IndexOf("</material>", at, StringComparison.Ordinal)) + 1;
            Assert.Equal(was, now[..from] + now[to..]);
            Assert.Equal(original[..2], File.ReadAllBytes(file)[..2]);
            Assert.Equal(100e9, r.Properties["EX"]);
            Assert.Equal(100e9 / 2.58, r.Properties["GXY"], 1);
            Assert.Contains(r.Checks, c => c.StartsWith("material_list lists it", StringComparison.Ordinal));
            Assert.Contains(r.Checks, c => c.StartsWith($"the library's other {theirs.Count} materials read back unchanged", StringComparison.Ordinal));
            var after = (await P.MaterialListAsync(null, 1000, 0)).Materials.Where(m => m.Database == Db).ToList();
            Assert.Contains(after, m => m.Name == name);
            static string Row(MaterialRow m) => $"{m.Classification}/{m.Name}/{m.DensityKgM3}";
            Assert.Equal(theirs.Select(Row), after.Where(m => m.Name != name).Select(Row));
            Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.MaterialAddAsync(new MaterialAddRequest
            {
                Name = name, DensityKgM3 = 7900, ElasticModulusGpa = 100, PoissonRatio = 0.29,
            })));
            // A material of SOLIDWORKS' own is never replaced, overwrite or not, and the file is not touched.
            byte[] mine = File.ReadAllBytes(file);
            Assert.Equal(ErrorCodes.NotOwned, await Code(() => P.MaterialAddAsync(new MaterialAddRequest
            {
                Name = theirs[0].Name.ToUpperInvariant(), DensityKgM3 = 1000, ElasticModulusGpa = 2, PoissonRatio = 0.4, Overwrite = true,
            })));
            Assert.Equal(mine, File.ReadAllBytes(file));

            string doc = await NewPartAsync("mat_custom");
            var sk = new SketchService(S);
            await sk.StartAsync(doc, new EntityRef { Name = "front" }, false);
            await sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Corners = [[0, 0], [100, 10]] }]);
            await new FeatureService(S).ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 10 });
            var set = await P.MaterialSetAsync(doc, name, Db);
            Assert.Equal(0.079, set.MassKg!.Value, 1e-6);
            var st = await new SimulationService(S).StaticAsync(new SimStaticRequest
            {
                Doc = doc, FixedFaces = [new EntityRef { At = [0, 5, 5] }],
                Loads = [new SimLoad { Kind = "force", Faces = [new EntityRef { At = [100, 5, 5] }], ValueN = 100,
                    Direction = System.Text.Json.JsonDocument.Parse("[0,-1,0]").RootElement.Clone() }],
            });
            Assert.Equal(2 * 0.200669, st.MaxDisplacementMm, 0.002);
            Assert.Equal(351.571, st.YieldStrengthMpa!.Value, 1e-3);
        }
        finally
        {
            // Leave SOLIDWORKS' library as it was, byte for byte, and no backup the test made.
            File.WriteAllBytes(file, original);
            if (!hadBackup) File.Delete(backup);
        }
    }

    [Fact]
    public async Task A_library_as_SOLIDWORKS_writes_it_keeps_its_bytes_but_for_the_added_material()
    {
        // Scenario test of build 134233, point 2: the declaration's "UTF-16" became "utf-16" and every "/>" of the user's
        // own materials " />". A library laid out as SOLIDWORKS writes its own, under a test name in the custom folder.
        string db = $"swmcp-test-layout-{Guid.NewGuid().ToString("N")[..6]}";
        var made = await P.MaterialAddAsync(new MaterialAddRequest { Name = "MCP Test Seed", Database = db, DensityKgM3 = 1000, ElasticModulusGpa = 1, PoissonRatio = 0.3 });
        string file = made.Path;
        const string Theirs =
            "<?xml version=\"1.0\" encoding=\"UTF-16\" standalone=\"no\"?>\r\n" +
            "<mstns:materials xmlns:mstns=\"http://www.solidworks.com/sldmaterials\" version=\"2008.03\">\r\n" +
            "\t<curves id=\"curve0\">\r\n\t\t<point x=\"1.0\" y=\"1.0\"/>\r\n\t</curves>\r\n" +
            "\t<classification name=\"Plastic\">\r\n" +
            "\t\t<material name=\"MCP Test Theirs\" description=\"-\">\r\n" +
            "\t\t\t<physicalproperties>\r\n" +
            "\t\t\t\t<EX displayname=\"Elastic modulus\" value=\"0.20E+10\"/>\r\n" +
            "\t\t\t\t<NUXY displayname=\"Poisson's ratio\" value=\"0.394\"/>\r\n" +
            "\t\t\t\t<DENS displayname=\"Density\" value=\"0.102E+04\"/>\r\n" +
            "\t\t\t</physicalproperties>\r\n\t\t</material>\r\n\t</classification>\r\n</mstns:materials>\r\n";
        try
        {
            File.WriteAllText(file, Theirs, Encoding.Unicode);
            var r = await P.MaterialAddAsync(new MaterialAddRequest
            {
                Name = "MCP Test Mine", Classification = "Plastic", Database = db, DensityKgM3 = 1270, ElasticModulusGpa = 2, PoissonRatio = 0.38,
            });
            Assert.Equal(MaterialXml.BackupPath(file), r.Backup);
            string now = File.ReadAllText(file);
            int at = now.IndexOf("<material name=\"MCP Test Mine\"", StringComparison.Ordinal);
            int from = now.LastIndexOf('\n', at) + 1, to = now.IndexOf('\n', now.IndexOf("</material>", at, StringComparison.Ordinal)) + 1;
            Assert.Equal(Theirs, now[..from] + now[to..]);
            Assert.Contains("\t\t\t\t<DENS displayname=\"Density\" value=\"1270\"/>\r\n", now); // the new one laid out alike
            Assert.Contains((await P.MaterialListAsync("MCP Test", 100, 0)).Materials, m => m.Name == "MCP Test Mine" && m.Database == db);
        }
        finally
        {
            File.Delete(file);
            File.Delete(MaterialXml.BackupPath(file));
        }
    }

    [Fact]
    public async Task A_material_library_that_does_not_parse_is_a_named_io_error_in_material_add_and_material_set()
    {
        // S28: SOLIDWORKS put its own header in front of a library; material_add then failed INTERNAL: XmlException and
        // material_set hinted at the name.
        string db = $"swmcp-test-broken-{Guid.NewGuid().ToString("N")[..6]}";
        var made = await P.MaterialAddAsync(new MaterialAddRequest { Name = "MCP Test Broken", Database = db, DensityKgM3 = 1270, ElasticModulusGpa = 2, PoissonRatio = 0.38 });
        string file = made.Path;
        try
        {
            string lib = Encoding.Unicode.GetString(File.ReadAllBytes(file)).TrimStart('﻿');
            File.WriteAllText(file, "<?xml version=\"1.0\"?>\r\n<mstns:materials xmlns:mstns=\"http://www.solidworks.com/sldmaterials\">\r\n" + lib, Encoding.Unicode);
            byte[] broken = File.ReadAllBytes(file);

            var add = await Assert.ThrowsAsync<SwException>(() => P.MaterialAddAsync(new MaterialAddRequest
            {
                Name = "MCP Test Broken", Database = db, DensityKgM3 = 1270, ElasticModulusGpa = 2, PoissonRatio = 0.38, Overwrite = true,
            }));
            Assert.Equal(ErrorCodes.IoError, add.Code);
            Assert.Contains($"'{file}' does not parse", add.Message);
            Assert.Contains("Line 3", add.Message);
            Assert.Contains("SOLIDWORKS can rewrite a library", add.Message);
            Assert.Contains("Move the file aside", add.Hint);
            Assert.Equal(broken, File.ReadAllBytes(file)); // nothing written

            Assert.Contains((await P.MaterialListAsync(null, 5, 0)).Warnings!, w => w.Contains(file));
            string doc = await NewPartAsync("mat_broken");
            var named = await Assert.ThrowsAsync<SwException>(() => P.MaterialSetAsync(doc, "MCP Test Broken", db));
            Assert.Equal(ErrorCodes.IoError, named.Code);
            Assert.Contains($"'{file}' does not parse", named.Message);
            var any = await Assert.ThrowsAsync<SwException>(() => P.MaterialSetAsync(doc, "MCP Test Broken"));
            Assert.Equal(ErrorCodes.IoError, any.Code);
            Assert.Contains($"'{db}' can't be read", any.Message);
            // A typo still gets the closest names from the libraries that parse (scenario test of build 134233, point 3).
            var typo = await Assert.ThrowsAsync<SwException>(() => P.MaterialSetAsync(doc, "AISI 102"));
            Assert.Equal(ErrorCodes.IoError, typo.Code);
            Assert.Contains("closest: ", typo.Message);
            Assert.Contains("'AISI 1020'", typo.Message);
            Assert.Null((await new InspectService(S).ModelInfoAsync(doc)).Material); // nothing was applied
            Assert.Equal(broken, File.ReadAllBytes(file));
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Fact]
    public async Task Material_list_has_AISI_1020_at_7900_kg_per_m3()
    {
        var r = await P.MaterialListAsync("aisi 1020", 100, 0);
        var m = Assert.Single(r.Materials, x => x.Name == "AISI 1020" && x.Database == "SOLIDWORKS Materials");
        Assert.Equal(7900, m.DensityKgM3!.Value, 1e-6);
        Assert.Equal("Steel", m.Classification);
        Assert.Equal("SOLIDWORKS Materials", r.Databases[0].Name); // the standard database is listed first
        Assert.True(r.Databases[0].Materials > 100);

        var page = await P.MaterialListAsync(null, 5, 0);
        Assert.Equal(5, page.Materials.Count);
        // S39 (b): a filter's databases are those it reaches, not the whole list repeated on every call.
        Assert.All(r.Databases, db => Assert.Contains(r.Materials, x => x.Database == db.Name));
        Assert.True(r.Databases.Count < page.Databases.Count, $"{r.Databases.Count} of {page.Databases.Count} databases");
        Assert.True(page.Truncated);
        Assert.True(page.Total > 100, $"total {page.Total}");
        var next = await P.MaterialListAsync(null, 5, 5);
        Assert.Empty(next.Materials.Select(m => m.Name).Intersect(page.Materials.Select(m => m.Name)));

        var none = await P.MaterialListAsync("PLA-does-not-exist", 10, 0);
        Assert.Empty(none.Materials);
        Assert.Equal(0, none.Total);
        Assert.Contains(none.Warnings!, w => w.Contains("No material matches"));
        Assert.Empty(none.Databases);
    }

    [Fact]
    public async Task Material_set_AISI_1020_gives_the_analytic_mass()
    {
        var (doc, _) = await BlockAsync("material");
        var r = await P.MaterialSetAsync(doc, "AISI 1020");
        Assert.Equal("AISI 1020", r.Material);
        Assert.Null(r.Previous);
        Assert.Equal(7900, r.DatabaseDensityKgM3!.Value, 1e-6);
        Assert.Equal(7900, r.DensityKgM3!.Value, 0.5);
        Assert.Equal(7900 * 1.2e-4, r.MassKg!.Value, 1e-6); // 0.948 kg
        Assert.Contains(r.Checks, c => c.StartsWith("density"));
        Assert.Equal("AISI 1020", (await new InspectService(S).ModelInfoAsync(doc)).Material);

        // A case-insensitive name is accepted and reported in its real spelling; the previous material comes back.
        var alu = await P.MaterialSetAsync(doc, "6061 alloy");
        Assert.Equal("6061 Alloy", alu.Material);
        Assert.Equal("AISI 1020", alu.Previous);
        Assert.Equal(2700 * 1.2e-4, alu.MassKg!.Value, 1e-6); // 0.324 kg
    }

    [Fact]
    public async Task Material_set_rejects_unknown_names_with_suggestions()
    {
        var (doc, _) = await BlockAsync("material_bad");
        var pla = await Assert.ThrowsAsync<SwException>(() => P.MaterialSetAsync(doc, "PLA"));
        Assert.Equal(ErrorCodes.EntityNotFound, pla.Code);
        Assert.Contains("Closest:", pla.Message);
        var alu = await Assert.ThrowsAsync<SwException>(() => P.MaterialSetAsync(doc, "Aluminum 6061-T6"));
        Assert.Contains("'6061-T6 (SS)'", alu.Message);
        Assert.Equal(ErrorCodes.EntityNotFound, await Code(() => P.MaterialSetAsync(doc, "AISI 1020", "No Such Database")));
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.MaterialSetAsync(doc, "  ")));
        Assert.Null((await new InspectService(S).ModelInfoAsync(doc)).Material); // nothing was applied
    }

    // ------------------------------------------------------------------ custom properties

    [Fact]
    public async Task Custom_properties_round_trip()
    {
        string doc = await NewPartAsync("props");
        var a = await P.PropSetAsync(doc, "SwmcpPartNo", "ACME-001");
        Assert.Null(a.Previous);
        Assert.Equal("ACME-001", a.Property.Value);
        Assert.Equal("text", a.Property.Type);
        var b = await P.PropSetAsync(doc, "SwmcpPartNo", "ACME-002");
        Assert.Equal("ACME-001", b.Previous!.Value);
        var n = await P.PropSetAsync(doc, "SwmcpCount", "12", null, "number");
        Assert.Contains(n.Property.Type, new[] { "number", "double" });
        var y = await P.PropSetAsync(doc, "SwmcpBought", "yes", null, "yesno");
        Assert.Equal("Yes", y.Property.Value);

        var list = await P.PropListAsync(doc);
        Assert.Equal("ACME-002", Assert.Single(list.Properties, p => p.Name == "SwmcpPartNo").Value);
        Assert.Contains(list.Properties, p => p.Name == "SwmcpCount");

        var del = await P.PropDeleteAsync(doc, "SwmcpPartNo");
        Assert.Equal("ACME-002", del.Deleted.Value);
        Assert.DoesNotContain((await P.PropListAsync(doc)).Properties, p => p.Name == "SwmcpPartNo");
        Assert.Equal(ErrorCodes.EntityNotFound, await Code(() => P.PropDeleteAsync(doc, "SwmcpPartNo")));
    }

    [Fact]
    public async Task Decimal_number_property_reads_back()
    {
        // Probes the locale: the value is passed with a dot whatever the Windows decimal separator is.
        string doc = await NewPartAsync("props_decimal");
        var r = await P.PropSetAsync(doc, "SwmcpMass", "12.5", null, "number");
        Assert.True(PropertiesService.SameValue(3, r.Property.Value, "12.5"), r.Property.Value);
    }

    [Fact]
    public async Task Configuration_properties_are_separate_and_links_resolve()
    {
        var (doc, depth) = await BlockAsync("props_cfg");
        string cfg = (await P.ConfigListAsync(doc)).Active;
        var c = await P.PropSetAsync(doc, "SwmcpFinish", "Anodized", cfg);
        Assert.Equal(cfg, c.Configuration);
        Assert.Contains((await P.PropListAsync(doc, cfg)).Properties, p => p.Name == "SwmcpFinish");
        Assert.DoesNotContain((await P.PropListAsync(doc)).Properties, p => p.Name == "SwmcpFinish");

        var link = await P.PropSetAsync(doc, "SwmcpThickness", $"\"{depth}\"");
        Assert.StartsWith($"\"{depth}", link.Property.Value); // possibly qualified with the document name
        Assert.StartsWith("20", link.Property.Resolved); // "20.00" or "20,00" (Windows locale)
    }

    [Fact]
    public async Task Custom_property_arguments_are_validated()
    {
        string doc = await NewPartAsync("props_bad");
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.PropSetAsync(doc, "SwmcpX", "1", null, "color")));
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.PropSetAsync(doc, "SwmcpX", "abc", null, "number")));
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.PropSetAsync(doc, "SwmcpX", "maybe", null, "yesno")));
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.PropSetAsync(doc, " ", "1")));
        Assert.Equal(ErrorCodes.EntityNotFound, await Code(() => P.PropSetAsync(doc, "SwmcpX", "1", "NoSuchConfiguration")));
        Assert.DoesNotContain((await P.PropListAsync(doc)).Properties, p => p.Name == "SwmcpX");
    }

    // ------------------------------------------------------------------ equations

    [Fact]
    public async Task A_global_variable_drives_the_extrude_depth()
    {
        var (doc, depth) = await BlockAsync("equations");
        var w = await P.EquationSetAsync(doc, "W", "60mm");
        Assert.True(w.Created);
        Assert.True(w.Equation.Global);
        Assert.Equal(60, w.Equation.Value!.Value, 1e-6); // document units (MMGS)
        Assert.Null(w.Warnings);

        var w2 = await P.EquationSetAsync(doc, "W", "80mm");
        Assert.False(w2.Created);
        Assert.Contains("60mm", w2.Previous!.Text);
        Assert.Equal(80, w2.Equation.Value!.Value, 1e-6);

        var d = await P.EquationSetAsync(doc, depth, "\"W\" / 2");
        Assert.Equal(40, d.Dimension!.Value, 1e-6);
        Assert.Equal(100 * 40 * 60, (await new InspectService(S).ModelInfoAsync(doc)).VolumeMm3!.Value, 1e-3);
        Assert.Equal(2, (await P.EquationListAsync(doc)).Equations.Count);

        // W is used by the depth equation, so it can't go first.
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.EquationDeleteAsync(doc, "W")));
        Assert.Equal(1, (await P.EquationDeleteAsync(doc, depth)).Remaining);
        Assert.Equal(0, (await P.EquationDeleteAsync(doc, "W")).Remaining);
        Assert.Empty((await P.EquationListAsync(doc)).Equations);
        // The dimension keeps its last value once its equation is gone.
        Assert.Equal(100 * 40 * 60, (await new InspectService(S).ModelInfoAsync(doc)).VolumeMm3!.Value, 1e-3);
    }

    [Fact]
    public async Task Equation_delete_takes_a_list_checked_before_anything_goes()
    {
        // S54 (b): one lhs per call took seven calls for one feature's equations.
        var (doc, depth) = await BlockAsync("equations_delete_list");
        await P.EquationSetManyAsync(doc, [new EquationInput { Lhs = "W", Expression = "60mm" }, new EquationInput { Lhs = depth, Expression = "\"W\" / 3" }]);
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.EquationDeleteManyAsync(doc, ["W"]))); // the depth uses W
        Assert.Equal(ErrorCodes.EntityNotFound, await Code(() => P.EquationDeleteManyAsync(doc, [depth, "Nope"])));
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.EquationDeleteManyAsync(doc, ["W", "w"])));
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.EquationDeleteManyAsync(doc, ["W"], "W")));
        Assert.Equal(2, (await P.EquationListAsync(doc)).Equations.Count); // nothing was deleted

        var r = await P.EquationDeleteManyAsync(doc, ["W", depth]); // W goes with the equation that uses it
        Assert.Equal(new[] { "W", depth }, r.Deleted.Select(x => x.Lhs), StringComparer.OrdinalIgnoreCase);
        Assert.Equal(0, r.Remaining);
        Assert.Null(r.Warnings);
        Assert.Empty((await P.EquationListAsync(doc)).Equations);
    }

    [Fact]
    public async Task Equation_errors_are_typed_and_bare_numbers_warn()
    {
        var (doc, _) = await BlockAsync("equations_bad");
        Assert.Equal(ErrorCodes.EntityNotFound, await Code(() => P.EquationSetAsync(doc, "D9@NoSuchSketch", "5mm")));
        Assert.Equal(ErrorCodes.EntityNotFound, await Code(() => P.EquationDeleteAsync(doc, "Nope")));
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.EquationSetAsync(doc, "W", "\"W\" = 5mm")));
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.EquationSetAsync(doc, "", "5mm")));

        var h = await P.EquationSetAsync(doc, "H", "25");
        Assert.Contains(h.Warnings!, x => x.Contains("Bare number"));
        Assert.Equal(25, h.Equation.Value!.Value, 1e-6);

        // An unknown name is refused before anything changes (SOLIDWORKS could keep an equation that reads 0).
        var unknown = await Assert.ThrowsAsync<SwException>(() => P.EquationSetAsync(doc, "Q", "\"NoSuchVariable\" * 2"));
        Assert.Equal(ErrorCodes.EntityNotFound, unknown.Code);
        Assert.Contains("Global variables: H", unknown.Hint!);

        // Circular references, directly and through another equation.
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.EquationSetAsync(doc, "H", "\"H\" + 1mm")));
        var g = await P.EquationSetAsync(doc, "G", "\"H\" * 2");
        Assert.Equal(50, g.Equation.Value!.Value, 1e-6);
        var loop = await Assert.ThrowsAsync<SwException>(() => P.EquationSetAsync(doc, "H", "\"G\" / 2"));
        Assert.Equal(ErrorCodes.InvalidArgument, loop.Code);
        Assert.Contains("circular", loop.Message);

        var list = await P.EquationListAsync(doc);
        Assert.DoesNotContain(list.Equations, e => e.Lhs == "Q");
        Assert.Equal(25, Assert.Single(list.Equations, e => e.Lhs == "H").Value!.Value, 1e-6); // unchanged
    }

    [Fact]
    public async Task An_equation_that_breaks_a_feature_names_it_and_its_error()
    {
        // S58 (a): a change that made features fail read as a success, "the rebuild reported problems" its only word.
        // Not a fillet radius: SOLIDWORKS lets a fillet overflow its faces (r100 on the block rebuilds). A shell thicker than
        // the 20 mm block fails.
        var (doc, _) = await BlockAsync("equations_breaks");
        var shell = await new PartFeatureService(S).ShellAsync(new ShellRequest { Doc = doc, RemoveFaces = [new EntityRef { At = [0, 20, 0] }], ThicknessMm = 2 });
        string wall = shell.Dimensions!.First(x => x.Unit == "mm" && Math.Abs(x.Value - 2) < 1e-9).Name;
        var r = await P.EquationSetAsync(doc, wall, "25mm");
        var e = Assert.Single(r.FeatureErrors!);
        Assert.Equal(shell.Feature.Name, e.Feature);
        Assert.Null(e.Warning);
        Assert.Contains(r.Warnings!, w => w.Contains("1 feature(s) now fail") && w.Contains($"'{shell.Feature.Name}'") && w.Contains(e.Error));
        Assert.DoesNotContain(r.Warnings!, w => w.Contains("The rebuild reported problems"));

        // A batch says it too, and one that breaks nothing new carries no list.
        var b = await P.EquationSetManyAsync(doc, [Eq("T", "3mm"), Eq(wall, "\"T\"")]);
        Assert.Null(b.FeatureErrors); // the shell failed before the call and rebuilds now
        var b2 = await P.EquationSetManyAsync(doc, [Eq("T", "30mm")]);
        Assert.Equal(shell.Feature.Name, Assert.Single(b2.FeatureErrors!).Feature);
    }

    [Fact]
    public async Task An_equation_on_a_plane_s_offset_gives_where_the_plane_went()
    {
        // S58 (c): the result gave the volume change, not where the plane went.
        string doc = await NewPartAsync("equations_plane");
        var plane = await new ReferenceService(S).PlaneAsync(new RefPlaneRequest { Doc = doc, Kind = "offset", Base = new EntityRef { Name = "top" }, OffsetMm = 30 });
        string offset = plane.Dimensions!.First(x => x.Unit == "mm" && Math.Abs(x.Value - 30) < 1e-9).Name;
        var r = await P.EquationSetAsync(doc, offset, "45mm");
        Assert.Equal(45, Math.Abs(r.Frame!.OffsetMm!.Value), 1e-6);
        Assert.Equal(1, Math.Abs(r.Frame.Normal![1]), 1e-9); // still parallel to the top plane
        Assert.Equal(45, r.Frame.OriginMm[1], 1e-6); // and 45 above it
        var many = await P.EquationSetManyAsync(doc, [Eq("H", "20mm"), Eq(offset, "\"H\"")]);
        Assert.Null(many.Equations[0].Frame); // a global variable
        Assert.Equal(20, many.Equations[1].Frame!.OriginMm[1], 1e-6);
        Assert.Contains("\"frame\":{", ResultJson.Serialize(many)); // the short rows keep it (S48 d)
    }

    [Fact]
    public async Task A_global_named_like_a_function_is_refused_for_its_name()
    {
        // S58 (b): SOLIDWORKS refuses the name of one of its functions as a variable; the message says so (probed 2026-11-13).
        string doc = await NewPartAsync("equations_refused");
        var ex = await Assert.ThrowsAsync<SwException>(() => P.EquationSetAsync(doc, "sin", "5mm"));
        Assert.Equal(ErrorCodes.FeatureFailed, ex.Code);
        Assert.Contains("name of a SOLIDWORKS function", ex.Message);
        Assert.Empty((await P.EquationListAsync(doc)).Equations);
    }

    [Fact]
    public void A_refused_plain_global_points_to_a_restart()
    {
        // S58 (b): every equation refused in one SOLIDWORKS session, "X" = 60mm too, the hint naming the syntax only.
        var (why, hint) = EquationText.Refused("X", "60mm", false);
        Assert.Contains("valid", why);
        Assert.Contains("restart SOLIDWORKS", hint);
        Assert.DoesNotContain("syntax", hint);
        Assert.Contains("SOLIDWORKS function", EquationText.Refused("Pi", "3", false).Why);
        var dim = EquationText.Refused("D1@Boss-Extrude1", "0mm", true); // a dimension can refuse a value; a plain one is no proof
        Assert.Equal(".", dim.Why);
        Assert.Contains("syntax", dim.Hint);
        Assert.Contains("restarted", dim.Hint);
        Assert.Equal(".", EquationText.Refused("W", "\"H\" / 2", false).Why);
    }

    [Fact]
    public async Task A_batch_result_gives_short_rows_unless_asked_in_full()
    {
        // S48 (d): each row gave the equation, the previous one or the driven dimension, and two checks: 3.1k for ten globals.
        var (doc, depth) = await BlockAsync("equations_short");
        await P.EquationSetManyAsync(doc, [.. Enumerable.Range(1, 10).Select(i => Eq($"G{i}", $"{i}mm")), Eq(depth, "\"G10\" * 3")]);
        var r = await P.EquationSetManyAsync(doc, [.. Enumerable.Range(1, 10).Select(i => Eq($"G{i}", $"{i + 1}mm")), Eq(depth, "\"G10\" * 4")]);
        string brief = ResultJson.Serialize(r);
        var rows = System.Text.Json.Nodes.JsonNode.Parse(brief)!["equations"]!.AsArray();
        Assert.Equal(11, rows.Count);
        Assert.Equal(new[] { "lhs", "value", "previousValue" }, rows[0]!.AsObject().Select(p => p.Key));
        Assert.Equal("G1", (string?)rows[0]!["lhs"]);
        Assert.Equal(2, (double)rows[0]!["value"]!, 1e-6);
        Assert.Equal(1, (double)rows[0]!["previousValue"]!, 1e-6);
        Assert.Equal(depth, (string?)rows[10]!["lhs"], StringComparer.OrdinalIgnoreCase);
        Assert.Equal(44, (double)rows[10]!["value"]!, 1e-6);
        Assert.Equal(30, (double)rows[10]!["previousValue"]!, 1e-6);
        Assert.True(brief.Length < 900, $"{brief.Length} characters: {brief}");
        string full = ResultJson.Serialize(r, detail: true);
        Assert.Contains("\"checks\":[", full);
        Assert.Contains("\"previous\":{", full);
        // Everything after the rows (volume, model, warnings) is the same text.
        Assert.Equal(full[full.IndexOf("\"volumeDeltaMm3\"", StringComparison.Ordinal)..],
            brief[brief.IndexOf("\"volumeDeltaMm3\"", StringComparison.Ordinal)..]);
    }

    private static EquationInput Eq(string lhs, string expression) => new() { Lhs = lhs, Expression = expression };

    /// <summary>
    /// The block with a 10 mm slot across its top face, cut 5 mm deep: a slot as deep as the block (20) splits it in two.
    /// Returns the block's depth dimension, the slot's, and the volume the 5 mm slot removed (negative).
    /// </summary>
    private async Task<(string Doc, string Depth, string SlotDepth, double SlotMm3)> SlotBlockAsync(string name)
    {
        var (doc, depth) = await BlockAsync(name);
        var sk = new SketchService(S);
        await sk.StartAsync(doc, new EntityRef { At = [0, 20, 0] }, false);
        // 120 long, so it crosses the block whichever way the face's sketch lies.
        await sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Center = [0, 0], WidthMm = 10, HeightMm = 120 }]);
        var cut = await new FeatureService(S).ExtrudeAsync(new ExtrudeRequest { Doc = doc, Cut = true, DepthMm = 5 });
        var slot = cut.Dimensions!.Where(x => x.Unit == "mm" && Math.Abs(x.Value - 5) < 1e-9).OrderBy(x => x.Name, StringComparer.Ordinal).First();
        return (doc, depth, slot.Name, cut.Verified.VolumeDeltaMm3);
    }

    [Fact]
    public async Task Several_equations_go_in_one_call_and_the_result_carries_the_volume_and_the_bodies()
    {
        var (doc, depth, slotDepth, slotMm3) = await SlotBlockAsync("equations_batch");
        double start = (await new InspectService(S).ModelInfoAsync(doc)).VolumeMm3!.Value;

        // The slot's row comes first and uses C, which the second row creates: written in an order that works (S37 a).
        var r = await P.EquationSetManyAsync(doc, [Eq(slotDepth, "\"C\" * 2"), Eq("C", "5mm"), Eq("U", "\"C\" + 1mm")]);
        Assert.Equal(3, r.Equations.Count);
        Assert.All(r.Equations, e => Assert.True(e.Created));
        Assert.Equal(10, r.Equations[0].Dimension!.Value, 1e-6);
        Assert.Equal("C", r.Equations[1].Equation.Lhs);
        Assert.Equal(5, r.Equations[1].Equation.Value!.Value, 1e-6);
        Assert.Equal(6, r.Equations[2].Equation.Value!.Value, 1e-6);
        Assert.Null(r.Warnings);
        // The slot went from 5 to 10 deep: as much again as it had removed (S37 b).
        Assert.Equal(slotMm3, r.VolumeDeltaMm3!.Value, 1e-3);
        Assert.Equal(start + slotMm3, r.Model!.VolumeMm3, 1e-3);
        Assert.Equal(1, r.Model.Bodies);
        Assert.Equal(3, (await P.EquationListAsync(doc)).Equations.Count);

        // A second batch changes two of them and gives the block's depth its first equation.
        var r2 = await P.EquationSetManyAsync(doc, [Eq("C", "2.5mm"), Eq("U", "\"C\" * 4"), Eq(depth, "\"U\" * 3")]);
        Assert.False(r2.Equations[0].Created);
        Assert.Contains("5mm", r2.Equations[0].Previous!.Text);
        Assert.Equal(10, r2.Equations[1].Equation.Value!.Value, 1e-6);
        Assert.True(r2.Equations[2].Created);
        Assert.Equal(30, r2.Equations[2].Dimension!.Value, 1e-6);
        // Back to a 5 mm slot, in a block of 30: 100 x 60 x 10 more.
        Assert.Equal(60_000 - slotMm3, r2.VolumeDeltaMm3!.Value, 1e-3);

        // A single call reports them too, and here the value splits the body: a slot deeper than the block (S7's usage).
        var one = await P.EquationSetAsync(doc, slotDepth, "\"U\" * 4");
        Assert.Equal(40, one.Dimension!.Value, 1e-6);
        Assert.Equal(5 * slotMm3, one.VolumeDeltaMm3!.Value, 1e-3); // 5 deep -> through the 30
        Assert.Equal(2, one.Model!.Bodies);
    }

    [Fact]
    public async Task A_batch_with_a_row_that_fails_leaves_the_model_as_it_was()
    {
        var (doc, depth, slotDepth, _) = await SlotBlockAsync("equations_batch_bad");
        double start = (await new InspectService(S).ModelInfoAsync(doc)).VolumeMm3!.Value;

        // Refused before anything is written: the arguments, a name twice, an unknown name, a loop inside the batch.
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.EquationSetManyAsync(doc, [])));
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.EquationSetManyAsync(doc, [Eq("T", "1mm")], "W", "5mm")));
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.EquationSetManyAsync(doc, [Eq("T", "1mm"), Eq("t", "2mm")])));
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.EquationSetManyAsync(doc, [Eq("T", "1mm"), Eq("U", "")])));
        Assert.Equal(ErrorCodes.EntityNotFound, await Code(() => P.EquationSetManyAsync(doc, [Eq("T", "15mm"), Eq("U", "\"Nope\" * 2")])));
        var loop = await Assert.ThrowsAsync<SwException>(() => P.EquationSetManyAsync(doc, [Eq("A", "\"B\" + 1mm"), Eq("B", "\"A\" * 2")]));
        Assert.Equal(ErrorCodes.InvalidArgument, loop.Code);
        Assert.Contains("circular", loop.Message);
        Assert.Empty((await P.EquationListAsync(doc)).Equations);

        // A new equation SOLIDWORKS won't take: the slot's depth can't be 0. The rows before it were written, the block's
        // depth among them; all of it is taken back, that depth, which had no equation, included.
        var bad = await Assert.ThrowsAsync<SwException>(() =>
            P.EquationSetManyAsync(doc, [Eq("T", "15mm"), Eq(depth, "\"T\" * 2"), Eq(slotDepth, "\"T\" - 15mm")]));
        Assert.True(bad.Code == ErrorCodes.FeatureFailed, $"{bad.Code}: {bad.Message}");
        Assert.Contains("Rolled back: every equation is as it was before the call", bad.Message);
        Assert.Empty((await P.EquationListAsync(doc)).Equations);
        Assert.Equal(start, (await new InspectService(S).ModelInfoAsync(doc)).VolumeMm3!.Value, 1e-3);

        // A row that fails after the rebuild, with equations already there: the slot's equation changed to one that gives 0,
        // which SOLIDWORKS drops without a word, keeping the old text (it used to come back ok with a warning).
        // Each is back at its text, the new one is gone, and the volume is the one from before.
        await P.EquationSetManyAsync(doc, [Eq("T", "15mm"), Eq(depth, "\"T\" * 2"), Eq(slotDepth, "\"T\" - 10mm")]);
        double grown = (await new InspectService(S).ModelInfoAsync(doc)).VolumeMm3!.Value;
        bad = await Assert.ThrowsAsync<SwException>(() =>
            P.EquationSetManyAsync(doc, [Eq("T", "20mm"), Eq("V", "\"T\" + 1mm"), Eq(slotDepth, "\"T\" - 20mm")]));
        Assert.True(bad.Code == ErrorCodes.VerifyFailed, $"{bad.Code}: {bad.Message}");
        Assert.Contains("kept the previous equation", bad.Message);
        Assert.Contains("Rolled back: every equation is as it was before the call", bad.Message);
        var list = (await P.EquationListAsync(doc)).Equations;
        Assert.Equal(3, list.Count);
        Assert.Contains("15mm", Assert.Single(list, e => e.Lhs == "T").Text);
        Assert.Contains("10mm", Assert.Single(list, e => e.Lhs.StartsWith(slotDepth, StringComparison.OrdinalIgnoreCase)).Text);
        Assert.DoesNotContain(list, e => e.Lhs == "V");
        Assert.Equal(grown, (await new InspectService(S).ModelInfoAsync(doc)).VolumeMm3!.Value, 1e-3);
    }

    // ------------------------------------------------------------------ configurations

    [Fact]
    public async Task Configurations_add_activate_and_delete()
    {
        string doc = await NewPartAsync("configs");
        var start = await P.ConfigListAsync(doc);
        string first = start.Active;
        int count = start.Configurations.Count;

        var a = await P.ConfigAddAsync(doc, "SwmcpLong");
        Assert.Equal("SwmcpLong", a.Active);
        Assert.Equal(first, a.Previous);
        var b = await P.ConfigAddAsync(doc, "SwmcpShort", activate: false);
        Assert.Equal("SwmcpLong", b.Active);
        Assert.Equal(count + 2, b.Configurations.Count);

        var c = await P.ConfigActivateAsync(doc, first);
        Assert.Equal(first, c.Active);
        Assert.Equal("SwmcpLong", c.Previous);
        Assert.Equal(first, (await P.ConfigListAsync(doc)).Active);

        var d = await P.ConfigDeleteAsync(doc, "swmcpshort"); // names match without case
        Assert.Equal("SwmcpShort", d.Configuration);
        Assert.DoesNotContain("SwmcpShort", d.Configurations);
        Assert.Equal(count + 1, d.Configurations.Count);
    }

    [Fact]
    public async Task Configuration_errors_are_typed()
    {
        string doc = await NewPartAsync("configs_bad");
        string active = (await P.ConfigListAsync(doc)).Active;
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.ConfigDeleteAsync(doc, active)));
        Assert.Equal(ErrorCodes.EntityNotFound, await Code(() => P.ConfigActivateAsync(doc, "NoSuchConfiguration")));
        Assert.Equal(ErrorCodes.EntityNotFound, await Code(() => P.ConfigDeleteAsync(doc, "NoSuchConfiguration")));
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.ConfigAddAsync(doc, active.ToUpperInvariant())));
        var again = await P.ConfigActivateAsync(doc, active); // already active: a no-op
        Assert.Equal(active, again.Active);
    }

    // ------------------------------------------------------------------ appearance

    [Fact]
    public async Task Appearance_colors_the_part_a_face_and_a_body()
    {
        var (doc, _) = await BlockAsync("appearance");
        var red = await P.AppearanceSetAsync(doc, [255, 0, 0]);
        Assert.Equal("part", red.Target);
        Assert.Equal([255, 0, 0], red.Appearance.ColorRgb);

        var glass = await P.AppearanceSetAsync(doc, [0, 0, 255], 0.5);
        Assert.Equal([255, 0, 0], glass.Previous!.ColorRgb);
        Assert.Equal(0.5, glass.Appearance.Transparency, 1e-3);

        var face = await P.AppearanceSetAsync(doc, [0, 255, 0], null, new EntityRef { At = [0, 20, 0] }); // the top face (y = 20)
        Assert.StartsWith("face", face.Target);
        Assert.Equal([0, 255, 0], face.Appearance.ColorRgb);

        var body = (await new InspectService(S).ModelInfoAsync(doc)).Bodies![0];
        var byId = await P.AppearanceSetAsync(doc, [255, 255, 0], null, new EntityRef { Id = body.Id });
        Assert.StartsWith("body", byId.Target);
        Assert.Equal([255, 255, 0], byId.Appearance.ColorRgb);
        var byName = await P.AppearanceSetAsync(doc, [0, 255, 255], null, new EntityRef { Name = body.Name }); // a body, not the feature
        Assert.StartsWith("body", byName.Target);
        Assert.Equal([255, 255, 0], byName.Previous!.ColorRgb);
    }

    [Fact]
    public async Task Appearance_arguments_are_validated()
    {
        var (doc, _) = await BlockAsync("appearance_bad");
        foreach (var bad in new[] { new[] { 300, 0, 0 }, new[] { 1, 2 }, new[] { -1, 0, 0 } })
            Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.AppearanceSetAsync(doc, bad)));
        Assert.Equal(ErrorCodes.InvalidArgument, await Code(() => P.AppearanceSetAsync(doc, [0, 0, 0], 1.5)));
        Assert.Equal(ErrorCodes.InvalidArgument,
            await Code(() => P.AppearanceSetAsync(doc, [0, 0, 0], null, new EntityRef { Type = "edge", At = [0, 20, 30] })));
        Assert.Equal(ErrorCodes.EntityNotFound,
            await Code(() => P.AppearanceSetAsync(doc, [0, 0, 0], null, new EntityRef { Name = "NoSuchBody" })));

        // 0-1 values are a likely mistake: applied as given (0-255), with a warning.
        var dark = await P.AppearanceSetAsync(doc, [1, 0, 0]);
        Assert.Equal([1, 0, 0], dark.Appearance.ColorRgb);
        Assert.Contains(dark.Warnings!, w => w.Contains("0-255"));
    }

    // ------------------------------------------------------------------ ownership

    [Fact]
    public async Task Foreign_documents_can_be_read_but_not_changed()
    {
        string title = await TestHooks.NewForeignPartAsync(S);
        try
        {
            Assert.Equal(ErrorCodes.NotOwned, await Code(() => P.PropSetAsync(title, "SwmcpX", "1")));
            Assert.Equal(ErrorCodes.NotOwned, await Code(() => P.MaterialSetAsync(title, "AISI 1020")));
            Assert.Equal(ErrorCodes.NotOwned, await Code(() => P.ConfigAddAsync(title, "SwmcpX")));
            Assert.Equal(ErrorCodes.NotOwned, await Code(() => P.AppearanceSetAsync(title, [255, 0, 0])));
            await P.PropListAsync(title);
            Assert.NotEmpty((await P.ConfigListAsync(title)).Configurations);
            Assert.Empty((await P.EquationListAsync(title)).Equations);
        }
        finally
        {
            await TestHooks.CloseRawAsync(S, title);
        }
    }
}

/// <summary>The interop-free helpers of the properties toolset (no SOLIDWORKS needed).</summary>
public class PropertiesOfflineTests
{
    [Theory]
    [InlineData("\"W\" = 60mm", "W")]
    [InlineData("\"D1@Sketch1\" = \"W\" / 2", "D1@Sketch1")]
    [InlineData("  \"D1@Boss-Extrude1\"= 20", "D1@Boss-Extrude1")]
    public void The_left_hand_side_is_the_first_quoted_name(string text, string lhs) => Assert.Equal(lhs, EquationText.Lhs(text));

    [Theory]
    [InlineData("60", new[] { "60" })]
    [InlineData("60mm", new string[0])]
    [InlineData("\"W\" / 2", new string[0])]
    [InlineData("2 * \"W\" + 5", new[] { "5" })]
    [InlineData("\"D1@Sketch1\" + 2.5 in", new string[0])]
    [InlineData("sin(30) * \"W\"", new[] { "30" })]
    [InlineData("1e-3m + .5mm - 30deg", new string[0])]
    [InlineData("iif(\"W\" > 50mm, 3, 2)", new[] { "3", "2" })]
    public void Bare_numbers_are_found_outside_names(string expression, string[] bare) =>
        Assert.Equal(bare, EquationText.BareNumbers(expression));

    [Fact]
    public void Left_hand_sides_are_normalized_and_checked()
    {
        Assert.Equal("W", EquationText.NormalizeLhs("\"W\""));
        Assert.Equal("D1@Sketch1", EquationText.NormalizeLhs("D1@Sketch1@Part1.Part"));
        Assert.True(EquationText.SameLhs("D1@Sketch1@Part1.Part", "d1@sketch1"));
        Assert.False(EquationText.SameLhs("W", "W2"));
        Assert.Equal(ErrorCodes.InvalidArgument, Assert.Throws<SwException>(() => EquationText.NormalizeLhs("W = 5")).Code);
        Assert.Equal(ErrorCodes.InvalidArgument, Assert.Throws<SwException>(() => EquationText.Expression("\"W\" = 5mm")).Code);
        Assert.Equal(ErrorCodes.InvalidArgument, Assert.Throws<SwException>(() => EquationText.Expression("\"W / 2")).Code);
        Assert.Equal("\"W\" = 60mm", EquationText.Build("W", "60mm"));
        Assert.True(EquationText.SameText("\"W\"=60MM", "\"W\" = 60mm"));
    }

    [Fact]
    public void Equations_name_their_features_and_dimensions()
    {
        // S54: what binds an equation to a feature is the name after '@' (or a suppression equation's left-hand side).
        Assert.Equal("Boss-Extrude1", EquationText.FeatureOf("D1@Boss-Extrude1"));
        Assert.Equal("Fillet1", EquationText.FeatureOf("Fillet1"));
        Assert.Null(EquationText.FeatureOf("W", global: true));
        Assert.Null(EquationText.FeatureOf("D1@Sketch1@bracket-1.Part"));
        Assert.Equal(["D1@Sketch1", "D2@Boss-Extrude1"], EquationText.DimensionNames("\"D1@Sketch1\" = \"W\" / 2 + \"D2@Boss-Extrude1\""));
        Assert.Equal(["D3@Cut1"], EquationText.DimensionNames("\"W\" = \"D3@Cut1\" * 2 + \"SW-Mass@Part1.SLDPRT\""));
        Assert.Empty(EquationText.DimensionNames("\"Fillet1\" = \"suppressed\""));
    }

    [Fact]
    public void Literals_and_references_parse()
    {
        Assert.Equal((60.0, "mm"), EquationText.Literal("60mm"));
        Assert.Equal((2.5, "in"), EquationText.Literal(" 2.5 in "));
        Assert.Equal((12.0, ""), EquationText.Literal("12"));
        Assert.Equal((-30.0, "deg"), EquationText.Literal("-30deg"));
        Assert.Null(EquationText.Literal("\"W\" / 2"));
        Assert.Equal(["W", "D2@Sketch1"], EquationText.References("\"D1@Sketch1\" = \"W\" / 2 + \"D2@Sketch1\""));
        Assert.Equal((0.0254, false), EquationText.UnitFactor("in"));
        Assert.Null(EquationText.UnitFactor("furlong"));
    }

    [Fact]
    public void Circular_references_are_found_through_other_equations()
    {
        (string, string)[] eqs = [("H", "\"H\" = \"W\" * 2"), ("D1@Sketch1", "\"D1@Sketch1\" = \"H\" + 5mm"), ("X", "\"X\" = 3mm")];
        Assert.Equal(["W", "W"], EquationText.Cycle("W", ["W"], eqs));
        Assert.Equal(["W", "H", "W"], EquationText.Cycle("W", ["h"], eqs));
        Assert.Equal(["W", "D1@Sketch1", "H", "W"], EquationText.Cycle("W", ["X", "D1@Sketch1"], eqs));
        Assert.Null(EquationText.Cycle("W", ["X"], eqs));
        Assert.Null(EquationText.Cycle("Y", ["H", "D1@Sketch1"], eqs)); // H uses W, which has no equation
        Assert.Null(EquationText.Cycle("W", [], eqs));
    }

    [Fact]
    public void Expression_names_skip_keywords_and_other_documents()
    {
        Assert.Equal(["W", "D2@Sketch1"],
            EquationText.ExpressionReferences("iif(\"W\" > 5mm, \"suppressed\", \"Unsuppressed\") + \"D2@Sketch1\""));
        Assert.Empty(EquationText.ExpressionReferences("60mm"));
        Assert.True(EquationText.IsExternal("D1@Sketch1@bracket-1.Part"));
        Assert.True(EquationText.IsExternal("W@Part1<1>.Part"));
        Assert.True(EquationText.IsExternal("SW-Mass@Part1.SLDPRT"));
        Assert.False(EquationText.IsExternal("D1@Sketch1"));
        Assert.False(EquationText.IsExternal("W"));
    }

    [Fact]
    public void Property_values_are_validated_per_type()
    {
        Assert.Equal((30, "x"), PropertiesService.PropValue("text", "x"));
        Assert.Equal((5, "12.5"), PropertiesService.PropValue("number", " 12.5 ")); // swCustomInfoDouble: swCustomInfoNumber (3) takes whole numbers only
        Assert.Equal((11, "Yes"), PropertiesService.PropValue("yesno", "true"));
        Assert.Equal((64, "2026-10-22"), PropertiesService.PropValue("date", "2026-10-22"));
        Assert.Throws<SwException>(() => PropertiesService.PropValue("number", "12,5"));
        Assert.Throws<SwException>(() => PropertiesService.PropValue("color", "red"));
        Assert.True(PropertiesService.SameValue(3, "12.50", "12.5"));
        Assert.True(PropertiesService.SameValue(11, "YES", "Yes"));
        Assert.False(PropertiesService.SameValue(30, "a", "b"));
        Assert.True(PropertiesService.SameValue(30, "\"D1@Boss-Extrude1@Part1.SLDPRT\"", "\"D1@Boss-Extrude1\""));
        Assert.False(PropertiesService.SameValue(30, "\"D1@Boss-Extrude2@Part1.SLDPRT\"", "\"D1@Boss-Extrude1\""));
        Assert.Equal("yesno", PropertiesService.TypeName(11));
    }

    [Fact]
    public void Databases_load_from_folders_with_the_standard_one_first()
    {
        string dir = Path.Combine(Path.GetTempPath(), $"swmcp_mat_{Guid.NewGuid():N}");
        Directory.CreateDirectory(dir);
        try
        {
            static string Db(string cls, string name, string dens) =>
                "<?xml version=\"1.0\" encoding=\"utf-16\"?><mstns:materials xmlns:mstns=\"http://www.solidworks.com/sldmaterials\">" +
                $"<classification name=\"{cls}\"><material name=\"{name}\"><physicalproperties><DENS value=\"{dens}\" />" +
                "</physicalproperties></material></classification></mstns:materials>";
            File.WriteAllText(Path.Combine(dir, "a custom.sldmat"), Db("Custom", "My Alloy", "2800"), Encoding.Unicode);
            File.WriteAllText(Path.Combine(dir, "solidworks materials.sldmat"), Db("Steel", "Test Steel", "7850"), Encoding.Unicode);
            File.WriteAllText(Path.Combine(dir, "broken.sldmat"), "<not xml", Encoding.Unicode);
            var cat = MaterialCatalog.Load($"{dir};{dir}\\", allowNetwork: false); // the same folder twice is read once
            Assert.Equal(["SOLIDWORKS Materials", "a custom"], cat.Databases.Select(d => d.Name));
            Assert.Equal(["Test Steel", "My Alloy"], cat.Materials.Select(m => m.Name));
            Assert.Contains(cat.Warnings, w => w.Contains("broken.sldmat"));
            Assert.Equal(2800, cat.Materials[1].DensityKgM3);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public void Material_databases_parse_classification_name_and_density()
    {
        const string xml = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<mstns:materials xmlns:mstns=\"http://www.solidworks.com/sldmaterials\" version=\"2008.03\">" +
            "<curves id=\"curve0\"><point x=\"1.0\" y=\"1.0\" /></curves>" +
            "<classification name=\"Metals\"><material name=\"Test Steel\" matid=\"1\"><shaders /><physicalproperties>" +
            "<EX displayname=\"Elastic modulus\" value=\"2.1E+11\" /><DENS displayname=\"Mass density\" value=\"0.79E+04\" />" +
            "</physicalproperties><custom><DENS value=\"1\" /></custom></material>" +
            "<material name=\"No Density\" /></classification>" +
            "<classification name=\"Plastics\"><material name=\"Test Plastic\"><physicalproperties><DENS value=\"1020\" />" +
            "</physicalproperties></material></classification></mstns:materials>";
        using var ms = new MemoryStream([.. Encoding.Unicode.GetPreamble(), .. Encoding.Unicode.GetBytes(xml)]);
        var rows = MaterialCatalog.Parse(ms, "Test DB");
        Assert.Equal(3, rows.Count);
        Assert.Equal(new MaterialCatalog.Entry("Test Steel", "Metals", "Test DB", 7900), rows[0]);
        Assert.Equal(new MaterialCatalog.Entry("No Density", "Metals", "Test DB", null), rows[1]);
        Assert.Equal(new MaterialCatalog.Entry("Test Plastic", "Plastics", "Test DB", 1020), rows[2]);
    }

    [Fact]
    public void Missing_or_network_folders_are_reported_not_thrown()
    {
        var cat = MaterialCatalog.Load(@"\\server\share\materials;C:\swmcp-no-such-folder", allowNetwork: false);
        Assert.Empty(cat.Materials);
        Assert.Contains(cat.Warnings, w => w.Contains("network folder"));
        Assert.Contains(cat.Warnings, w => w.Contains("No material database"));
        Assert.Equal("SOLIDWORKS Materials", MaterialCatalog.ApiName("solidworks materials"));
        Assert.True(MaterialCatalog.SameDatabase(@"C:\x\solidworks materials.sldmat", "SOLIDWORKS Materials"));
    }

    [Fact]
    public void Suggestions_rank_shared_words_first()
    {
        string[] names = ["6061 Alloy", "6061-T6 (SS)", "7075-T6, Plate (SS)", "AISI 1020", "AISI 1020 Steel, Cold Rolled", "ABS", "ABS PC",
            "Plain Carbon Steel"];
        Assert.Equal("6061-T6 (SS)", MaterialCatalog.Suggest(names, "Aluminum 6061-T6")[0]);
        Assert.Equal("AISI 1020", MaterialCatalog.Suggest(names, "AISI1020")[0]);
        Assert.Equal("ABS", MaterialCatalog.Suggest(names, "abs plastic")[0]);
        Assert.InRange(MaterialCatalog.Suggest(names, "PLA").Count, 1, 5);
        Assert.Empty(MaterialCatalog.Suggest([], "PLA"));
    }

    [Fact]
    public void The_installed_SOLIDWORKS_database_parses()
    {
        string? dir = InstalledMaterialFolder();
        Assert.SkipWhen(dir is null, "No SOLIDWORKS material folder (lang\\english\\sldmaterials) on this machine.");
        var cat = MaterialCatalog.Load(dir!, allowNetwork: false);
        var aisi = Assert.Single(cat.Materials, m => m.Name == "AISI 1020" && m.Database == MaterialCatalog.DefaultDatabase);
        Assert.Equal(7900, aisi.DensityKgM3);
        Assert.Equal("Steel", aisi.Classification);
        Assert.DoesNotContain(cat.Materials, m => m.Name == "PLA");
        Assert.Contains("6061-T6 (SS)", MaterialCatalog.Suggest(cat.Materials.Select(m => m.Name), "Aluminum 6061-T6"));
        Assert.All(cat.Materials.Where(m => m.Database == MaterialCatalog.DefaultDatabase), m => Assert.NotNull(m.DensityKgM3));
    }

    private static string? InstalledMaterialFolder()
    {
        foreach (var version in new[] { "2026", "2025", "2024", "2023" })
        {
            using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey($@"SOFTWARE\SolidWorks\SOLIDWORKS {version}\Setup");
            if (key?.GetValue("SolidWorks Folder") is string root && Path.Combine(root, "lang", "english", "sldmaterials") is var dir &&
                Directory.Exists(dir))
                return dir;
        }
        return null;
    }
}
