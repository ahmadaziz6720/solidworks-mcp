using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorksMcp.Core.Com;
using SolidWorksMcp.Core.Geometry;
using SolidWorksMcp.Core.Model;
using SolidWorksMcp.Core.Prefs;
using SolidWorksMcp.Core.Verify;

namespace SolidWorksMcp.Core.Services;

// ====================================================================== DTOs

/// <summary>One material of an installed material database.</summary>
public sealed class MaterialRow
{
    public required string Name { get; init; }
    public required string Classification { get; init; }
    /// <summary>The database name as SOLIDWORKS takes it ("SOLIDWORKS Materials", else the .sldmat file name).</summary>
    public required string Database { get; init; }
    /// <summary>Mass density from the database's physical properties (kg/m³).</summary>
    public double? DensityKgM3 { get; init; }
}

public sealed class MaterialDatabaseRow
{
    public required string Name { get; init; }
    public required string Path { get; init; }
    public int Materials { get; init; }
}

public sealed class MaterialListResult : ToolResult
{
    public required IReadOnlyList<MaterialRow> Materials { get; init; }
    public int Total { get; init; }
    public bool? Truncated { get; init; }
    public required IReadOnlyList<MaterialDatabaseRow> Databases { get; init; }
}

public sealed class MaterialSetResult : ToolResult
{
    public required string Material { get; init; }
    public string? Database { get; init; }
    public string? Classification { get; init; }
    /// <summary>The configuration the material was set in.</summary>
    public required string Configuration { get; init; }
    /// <summary>The material before the call (null = none).</summary>
    public string? Previous { get; init; }
    public string? PreviousDatabase { get; init; }
    /// <summary>Density from the document's mass properties after the change (mass / volume, kg/m³).</summary>
    public double? DensityKgM3 { get; init; }
    public double? DatabaseDensityKgM3 { get; init; }
    public double? MassKg { get; init; }
    public required IReadOnlyList<string> Checks { get; init; }
}

/// <summary>One custom property.</summary>
public sealed class CustomPropRow
{
    public required string Name { get; init; }
    /// <summary>The raw value as stored (links stay links, e.g. "D1@Boss-Extrude1" in quotes).</summary>
    public required string Value { get; init; }
    /// <summary>The evaluated value when it differs from the raw one (numbers use the Windows locale).</summary>
    public string? Resolved { get; init; }
    /// <summary>text | number | double | yesno | date | equation | unknown.</summary>
    public required string Type { get; init; }
    /// <summary>True when the property is linked to its parent's property (derived part, cut list): it is changed there.</summary>
    public bool? Linked { get; init; }
}

public sealed class PropListResult : ToolResult
{
    /// <summary>null = file level (the Custom tab); otherwise the configuration whose properties are listed.</summary>
    public string? Configuration { get; init; }
    public required IReadOnlyList<CustomPropRow> Properties { get; init; }
}

public sealed class PropSetResult : ToolResult
{
    public string? Configuration { get; init; }
    public required CustomPropRow Property { get; init; }
    /// <summary>The property before the call (null = it is new).</summary>
    public CustomPropRow? Previous { get; init; }
}

public sealed class PropDeleteResult : ToolResult
{
    public string? Configuration { get; init; }
    public required CustomPropRow Deleted { get; init; }
    public int Remaining { get; init; }
}

/// <summary>One equation or global variable.</summary>
public sealed class EquationRow
{
    public int Index { get; init; }
    public required string Text { get; init; }
    /// <summary>The left-hand side without quotes: a global variable (W) or a dimension (D1@Sketch1).</summary>
    public required string Lhs { get; init; }
    /// <summary>The value SOLIDWORKS evaluates (IEquationMgr.Value; document units).</summary>
    public double? Value { get; init; }
    public bool? Global { get; init; }
}

public sealed class EquationListResult : ToolResult
{
    public required IReadOnlyList<EquationRow> Equations { get; init; }
}

public sealed class EquationSetResult : ToolResult
{
    public required EquationRow Equation { get; init; }
    public bool Created { get; init; }
    /// <summary>The equation with this left-hand side before the call (null = it is new).</summary>
    public EquationRow? Previous { get; init; }
    /// <summary>For a dimension left-hand side: the dimension after the rebuild (mm / deg / count).</summary>
    public DimensionInfo? Dimension { get; init; }
    /// <summary>For a dimension of a reference plane or axis: where that plane or axis lies after the rebuild.</summary>
    public RefFrame? Frame { get; init; }
    public required IReadOnlyList<string> Checks { get; init; }
    /// <summary>For a part: the change in solid volume over the call, as dimension_set reports it.</summary>
    public double? VolumeDeltaMm3 { get; init; }
    /// <summary>For a part: volume, body count and box after the rebuild.</summary>
    public ModelState? Model { get; init; }
    /// <summary>Features that fail to rebuild after the call and did not before (null = none).</summary>
    public IReadOnlyList<FeatureError>? FeatureErrors { get; init; }
}

/// <summary>One equation_set entry of a batch.</summary>
[System.ComponentModel.Description("{lhs:'W', expression:'60mm'}")]
public sealed class EquationInput
{
    [System.ComponentModel.Description("Left-hand side without quotes: a global variable (W) or a dimension (D1@Sketch1).")]
    public required string Lhs { get; set; }

    [System.ComponentModel.Description("Right-hand side, e.g. 60mm or \"W\" / 2 + 5mm; names in double quotes.")]
    public required string Expression { get; set; }
}

/// <summary>One equation of an equation_set batch, as a single call reports it.</summary>
public sealed class EquationChange
{
    public required EquationRow Equation { get; init; }
    public bool Created { get; init; }
    public EquationRow? Previous { get; init; }
    public DimensionInfo? Dimension { get; init; }
    /// <summary>For a dimension of a reference plane or axis: where that plane or axis lies after the rebuild.</summary>
    public RefFrame? Frame { get; init; }
    public required IReadOnlyList<string> Checks { get; init; }
}

public sealed class EquationBatchResult : ToolResult
{
    /// <summary>In the order given.</summary>
    public required IReadOnlyList<EquationChange> Equations { get; init; }
    public double? VolumeDeltaMm3 { get; init; }
    public ModelState? Model { get; init; }
    /// <summary>Features that fail to rebuild after the call and did not before (null = none).</summary>
    public IReadOnlyList<FeatureError>? FeatureErrors { get; init; }
}

public sealed class EquationDeleteResult : ToolResult
{
    public required EquationRow Deleted { get; init; }
    public int Remaining { get; init; }
}

public sealed class EquationDeleteBatchResult : ToolResult
{
    /// <summary>In the order given, as they were before the call.</summary>
    public required IReadOnlyList<EquationRow> Deleted { get; init; }
    public int Remaining { get; init; }
}

public sealed class ConfigListResult : ToolResult
{
    public required IReadOnlyList<string> Configurations { get; init; }
    public required string Active { get; init; }
    /// <summary>The active configuration's display states, the active one first.</summary>
    public IReadOnlyList<string>? DisplayStates { get; init; }
}

public sealed class ConfigChangeResult : ToolResult
{
    /// <summary>The configuration the call created, activated or deleted.</summary>
    public required string Configuration { get; init; }
    public required string Active { get; init; }
    /// <summary>The configuration that was active before the call.</summary>
    public string? Previous { get; init; }
    public required IReadOnlyList<string> Configurations { get; init; }
    /// <summary>For kind display_state: the display state the call created, activated or deleted.</summary>
    public string? DisplayState { get; init; }
    public string? ActiveDisplayState { get; init; }
    public string? PreviousDisplayState { get; init; }
    /// <summary>The active configuration's display states (kind display_state), the active one first.</summary>
    public IReadOnlyList<string>? DisplayStates { get; init; }
    public string? Message { get; init; }
}

public sealed class AppearanceRow
{
    /// <summary>[r, g, b], 0-255.</summary>
    public required int[] ColorRgb { get; init; }
    /// <summary>0 = opaque, 1 = fully transparent.</summary>
    public double Transparency { get; init; }
}

public sealed class AppearanceSetResult : ToolResult
{
    /// <summary>"part", or the body / face that was colored.</summary>
    public required string Target { get; init; }
    public required AppearanceRow Appearance { get; init; }
    /// <summary>The appearance before the call (null when the target had none of its own).</summary>
    public AppearanceRow? Previous { get; init; }
    public ResolvedInfo? Resolved { get; init; }
}

// ====================================================================== service

/// <summary>
/// Materials, custom properties, equations, configurations and appearance (DESIGN §4.12). Every set tool reads back
/// what it set and returns the previous state; tools that rebuild keep the "errors at every rebuild" dialog off.
/// </summary>
public sealed partial class PropertiesService(SwSession session)
{
    /// <summary>Rebuilds can take a while on large models (progress is reported by the tool layer).</summary>
    private static readonly TimeSpan RebuildTimeout = TimeSpan.FromSeconds(120);

    private const double DensityRelTol = 0.005;

    // ------------------------------------------------------------------ material_list

    public Task<MaterialListResult> MaterialListAsync(string? filter, int limit = 100, int offset = 0, CancellationToken ct = default) =>
        session.RunAsync("material_list", ctx =>
        {
            var cat = LoadCatalog(ctx);
            string f = filter?.Trim() ?? "";
            var rows = cat.Materials.Where(m => f.Length == 0 || Has(m.Name, f) || Has(m.Classification, f) || Has(m.Database, f)).ToList();
            int skip = Math.Max(0, offset);
            var page = rows.Skip(skip).Take(Math.Clamp(limit, 1, 1000)).Select(m => m.ToRow()).ToList();
            if (rows.Count == 0 && f.Length > 0 && cat.Materials.Count > 0)
            {
                var close = MaterialCatalog.Suggest(cat.Materials.Select(m => m.Name), f);
                ctx.Warn(close.Count > 0 ? $"No material matches '{f}'. Closest names: {string.Join(", ", close)}." : $"No material matches '{f}'.");
            }
            return new MaterialListResult
            {
                Materials = page,
                Total = rows.Count,
                Truncated = skip + page.Count < rows.Count ? true : null,
                // With a filter, only the databases it reaches (S39): the whole list came back on every call.
                Databases = f.Length == 0 ? cat.Databases
                    : cat.Databases.Where(db => Has(db.Name, f) || rows.Any(m => m.Database.Equals(db.Name, StringComparison.OrdinalIgnoreCase))).ToList(),
            };
        }, null, ct);

    private static bool Has(string s, string part) => s.Contains(part, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// The databases SOLIDWORKS itself uses (Tools &gt; Options &gt; File Locations &gt; Material Databases). The folders come
    /// from SOLIDWORKS' own setting, never from tool arguments ('database' is matched against the names found here).
    /// </summary>
    private static MaterialCatalog.Catalog LoadCatalog(SwContext ctx)
    {
        var cat = MaterialCatalog.Load(CatalogFolders(ctx), ctx.Paths.AllowNetwork);
        foreach (var w in cat.Warnings) ctx.Warn(w);
        return cat;
    }

    private static string CatalogFolders(SwContext ctx)
    {
        string folders = ctx.App.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swFileLocationsMaterialDatabases) ?? "";
        // The File Locations list usually names only the custom folders: SOLIDWORKS always loads its standard databases
        // ("SOLIDWORKS Materials", DIN, …) from <install>\lang\<language>\sldmaterials as well (verified live).
        if (StandardMaterialFolder(ctx.App) is { } std) folders = std + ";" + folders;
        return folders;
    }

    /// <summary>&lt;SOLIDWORKS folder&gt;\lang\&lt;UI language&gt;\sldmaterials (english as the fallback), when it exists.</summary>
    private static string? StandardMaterialFolder(ISldWorks app)
    {
        try
        {
            string? exe = app.GetExecutablePath();
            if (string.IsNullOrWhiteSpace(exe)) return null;
            string dir = exe.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ? Path.GetDirectoryName(exe) ?? exe : exe;
            foreach (var lang in new[] { app.GetCurrentLanguage() ?? "", "english" }.Where(l => l.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                string folder = Path.Combine(dir, "lang", lang, "sldmaterials");
                if (Directory.Exists(folder)) return folder;
            }
        }
        catch (COMException ex) when (!ComArgs.IsDisconnect(ex)) { /* only the configured folders then */ }
        return null;
    }

    // ------------------------------------------------------------------ material_set

    public Task<MaterialSetResult> MaterialSetAsync(string? doc, string name, string? database = null, string? configuration = null,
        CancellationToken ct = default) =>
        session.RunAsync("material_set", ctx =>
        {
            string want = (name ?? "").Trim();
            if (want.Length == 0)
                throw SwException.InvalidArgument("'name' is required, e.g. \"AISI 1020\".", "material_list shows the installed materials.");
            var d = ctx.Doc;
            var part = ctx.Part();
            string cfg = ConfigArg(d, configuration, "configuration");
            string active = ActiveConfig(d);
            string cfgName = cfg.Length == 0 ? active : cfg;

            // Validate first: SetMaterialPropertyName2 silently ignores names it doesn't know.
            var cat = LoadCatalog(ctx);
            MaterialCatalog.Entry? entry = null;
            if (cat.Materials.Count > 0) entry = PickMaterial(cat, want, database);
            else ctx.Warn("No material database could be read, so the name was not checked in advance (the read-back still is).");
            string material = entry?.Name ?? want;
            string db = entry?.Database ?? MaterialCatalog.ApiName(string.IsNullOrWhiteSpace(database) ? MaterialCatalog.DefaultDatabase : database!.Trim());

            string prev = part.GetMaterialPropertyName2(cfg, out string prevDb) ?? "";
            // A name SOLIDWORKS keeps and does not apply (S30) was no material: say so, and the set below applies one.
            bool prevApplied = true;
            if (prev.Length > 0 && string.Equals(cfgName, active, StringComparison.OrdinalIgnoreCase) && d.SketchManager?.ActiveSketch is null &&
                MassProps(d) is { } before && before.Volume > 1e-15 &&
                MaterialNotApplied(ctx, part, cfgName, prev, prevDb, before.Mass / before.Volume) is { } notApplied)
            {
                prevApplied = false;
                ctx.Warn(notApplied); // whether this call applied its material is the density check below

            }
            if (cfg.Length > 0) SetMaterialIn(ctx, d, cfg, db, material); // a named configuration: that one only
            else part.SetMaterialPropertyName2(cfg, db, material);
            string now = part.GetMaterialPropertyName2(cfg, out string nowDb) ?? "";
            var checks = new List<string>();
            if (!string.Equals(now, material, StringComparison.Ordinal))
            {
                string restored = "";
                if (!string.Equals(now, prev, StringComparison.Ordinal) && prev.Length > 0)
                {
                    part.SetMaterialPropertyName2(cfg, prevDb ?? "", prev);
                    restored = part.GetMaterialPropertyName2(cfg, out _) == prev ? $" Restored '{prev}'." : $" Could not restore '{prev}'.";
                }
                // SOLIDWORKS has rewritten a custom library while failing to apply a material from it, leaving it unreadable.
                if (cat.Databases.FirstOrDefault(x => x.Name.Equals(db, StringComparison.OrdinalIgnoreCase)) is { } row &&
                    MaterialCatalog.ParseError(row.Path) is { } broke)
                    throw MaterialXml.Unreadable(row.Path, broke,
                        $"SOLIDWORKS did not apply '{material}' from '{db}' (the material of '{cfgName}' reads '{(now.Length == 0 ? "(none)" : now)}').{restored} " +
                        "During the call SOLIDWORKS rewrote the library. ");
                throw new SwException(ErrorCodes.VerifyFailed,
                    $"SOLIDWORKS did not apply '{material}' from '{db}': the material of '{cfgName}' reads '{(now.Length == 0 ? "(none)" : now)}'.{restored}",
                    "Check the name and database with material_list (SOLIDWORKS ignores unknown names).");
            }
            checks.Add($"material reads back '{now}'");
            if (!MaterialCatalog.SameDatabase(nowDb, db))
                ctx.Warn($"SOLIDWORKS reports the database as '{nowDb}' (requested '{db}').");

            double? density = null, mass = null;
            if (!string.Equals(cfgName, active, StringComparison.OrdinalIgnoreCase))
                checks.Add($"density not checked ('{cfgName}' is not the active configuration)");
            else if (d.SketchManager?.ActiveSketch is not null)
                ctx.Warn("A sketch is open, so mass properties read zero: the density was not checked (sketch_finish, then model_info).");
            else if (MassProps(d) is { } mp && mp.Volume > 1e-15)
            {
                mass = mp.Mass;
                density = mp.Mass / mp.Volume;
                if (entry?.DensityKgM3 is double expect)
                {
                    if (Math.Abs(density.Value - expect) <= DensityRelTol * expect)
                        checks.Add($"density {F(density.Value)} kg/m³ matches the database");
                    else
                        ctx.Warn($"The mass properties give {F(density.Value)} kg/m³, the database {F(expect)} kg/m³ " +
                                 "(bodies with their own material, or an overridden mass?).");
                }
            }
            else checks.Add("density not checked (no solid volume)");

            return new MaterialSetResult
            {
                Doc = ctx.Title(d),
                Material = now,
                Database = string.IsNullOrEmpty(nowDb) ? db : nowDb,
                Classification = entry?.Classification is { Length: > 0 } c ? c : null,
                Configuration = cfgName,
                Previous = prev.Length == 0 || !prevApplied ? null : prev,
                PreviousDatabase = prev.Length == 0 || !prevApplied || string.IsNullOrEmpty(prevDb) ? null : prevDb,
                DensityKgM3 = density is double x ? Math.Round(x, 3) : null,
                DatabaseDensityKgM3 = entry?.DensityKgM3,
                MassKg = mass is double m ? Units.R(m) : null,
                Checks = checks,
            };
        }, RunOptions.Write(doc), ct);

    /// <summary>Exact name, then case-insensitive; a miss lists the closest names (ENTITY_NOT_FOUND).</summary>
    private static MaterialCatalog.Entry PickMaterial(MaterialCatalog.Catalog cat, string name, string? database)
    {
        IEnumerable<MaterialCatalog.Entry> pool = cat.Materials;
        string where = "the installed databases";
        if (!string.IsNullOrWhiteSpace(database))
        {
            string dbWant = database.Trim();
            var dbHit = cat.Databases.FirstOrDefault(x => x.Name.Equals(dbWant, StringComparison.OrdinalIgnoreCase));
            if (dbHit is null && cat.Broken.FirstOrDefault(x => x.Database.Equals(dbWant, StringComparison.OrdinalIgnoreCase)) is { } bad)
                throw MaterialXml.Unreadable(bad.File, bad.Error, $"'{name}' can't be looked up in '{bad.Database}'. ");
            if (dbHit is null)
                throw new SwException(ErrorCodes.EntityNotFound, $"No material database named '{dbWant}'.",
                    $"Databases: {string.Join(", ", cat.Databases.Select(x => x.Name).Distinct())}.");
            pool = pool.Where(m => m.Database.Equals(dbHit.Name, StringComparison.OrdinalIgnoreCase));
            where = $"'{dbHit.Name}'";
        }
        var list = pool.ToList();
        var hits = list.Where(m => m.Name == name).ToList();
        if (hits.Count == 0) hits = list.Where(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();
        if (hits.Count == 0 && string.IsNullOrWhiteSpace(database) && cat.Broken.Count > 0)
        {
            // The material may well be in the library that no longer parses: say that, and the closest names for a typo.
            var bad = cat.Broken[0];
            var near = MaterialCatalog.Suggest(list.Select(m => m.Name), name);
            throw MaterialXml.Unreadable(bad.File, bad.Error,
                $"No material named '{name}' in the databases that parse" +
                (near.Count > 0 ? $" (closest: {string.Join(", ", near.Select(s => $"'{s}'"))})" : "") +
                $", and '{bad.Database}' can't be read" +
                (cat.Broken.Count > 1 ? $" (nor {string.Join(", ", cat.Broken.Skip(1).Select(b => $"'{b.Database}'"))})" : "") + ". ");
        }
        if (hits.Count == 0)
        {
            var near = MaterialCatalog.Suggest(list.Select(m => m.Name), name);
            throw new SwException(ErrorCodes.EntityNotFound,
                $"No material named '{name}' in {where} (SOLIDWORKS would silently ignore it)." +
                (near.Count > 0 ? $" Closest: {string.Join(", ", near.Select(s => $"'{s}'"))}." : ""),
                "Pass an exact name from material_list (its filter searches names and classifications).");
        }
        if (hits.Count > 1)
        {
            var std = hits.FirstOrDefault(h => MaterialCatalog.IsDefault(h.Database));
            if (std is not null) return std;
            throw new SwException(ErrorCodes.EntityAmbiguous,
                $"'{name}' is in several databases: {string.Join(", ", hits.Select(h => h.Database).Distinct())}.", "Pass 'database'.");
        }
        return hits[0];
    }

    /// <summary>
    /// S30: SOLIDWORKS 2023 can name a material for the active configuration that it does not apply (a part with a
    /// configuration table, after its configurations were activated in turn): the name reads back by '""', by the
    /// configuration's name and as MaterialIdName, while the mass has the document's default density. Reading by name
    /// does not tell, so the density is the check: the warning when the name's database density and the measured one
    /// disagree, null when they agree or it can't be judged (no catalog entry, bodies with their own material).
    /// </summary>
    internal static string? MaterialNotApplied(SwContext ctx, IPartDoc part, string config, string name, string? database,
        double densityKgM3)
    {
        if (name.Length == 0 || !(densityKgM3 > 0)) return null;
        try
        {
            if (part.Bodies(swBodyType_e.swSolidBody, visibleOnly: false)
                .Any(b => (b.GetMaterialPropertyName("", out _) ?? "") is { Length: > 0 } own && own != name)) return null;
        }
        catch (COMException e) when (!ComArgs.IsDisconnect(e)) { return null; }
        var cat = MaterialCatalog.Load(CatalogFolders(ctx), ctx.Paths.AllowNetwork); // its warnings are material_list's
        var hits = cat.Materials.Where(m => m.Name == name && m.DensityKgM3 is > 0).ToList();
        var entry = hits.FirstOrDefault(m => !string.IsNullOrEmpty(database) && MaterialCatalog.SameDatabase(database, m.Database))
                    ?? (hits.Count == 1 ? hits[0] : null);
        if (entry?.DensityKgM3 is not double expect || Math.Abs(densityKgM3 - expect) <= DensityRelTol * expect) return null;
        return $"SOLIDWORKS names the material '{name}' for '{config}', but its mass has {F(densityKgM3)} kg/m³, not the " +
               $"{F(expect)} kg/m³ of '{name}': the material is not applied, so it is reported as none.";
    }

    private static IMassProperty2? MassProps(IModelDoc2 d)
    {
        if ((object?)d.Extension.CreateMassProperty2() is not IMassProperty2 mp) return null;
        mp.UseSystemUnits = true; // kg, m³ whatever the document units
        return mp;
    }

    // ------------------------------------------------------------------ custom properties

    public Task<PropListResult> PropListAsync(string? doc, string? configuration = null, CancellationToken ct = default) =>
        session.RunAsync("prop_list", ctx =>
        {
            var d = ctx.Doc;
            string cfg = ConfigArg(d, configuration, "configuration");
            var mgr = PropManager(d, cfg);
            var rows = new List<CustomPropRow>();
            foreach (var n in PropNames(mgr))
                if (ReadProp(mgr, n) is { } row) rows.Add(row);
            return new PropListResult { Doc = ctx.Title(d), Configuration = cfg.Length == 0 ? null : cfg, Properties = rows };
        }, RunOptions.Read(doc), ct);

    public Task<PropSetResult> PropSetAsync(string? doc, string name, string value, string? configuration = null, string? type = "text",
        CancellationToken ct = default) =>
        session.RunAsync("prop_set", ctx =>
        {
            string n = PropName(name);
            var (code, v) = PropValue(type, value);
            var d = ctx.Doc;
            string cfg = ConfigArg(d, configuration, "configuration");
            var mgr = PropManager(d, cfg);
            string? existing = FindProp(PropNames(mgr), n);
            var previous = existing is null ? null : ReadProp(mgr, existing);
            int previousCode = existing is null ? 0 : mgr.GetType2(existing);
            string target = existing ?? n;

            int r = mgr.Add3(target, code, v, (int)swCustomPropertyAddOption_e.swCustomPropertyReplaceValue);
            if (r == (int)swCustomInfoAddResult_e.swCustomInfoAddResult_MismatchAgainstExistingType)
            {
                // The property exists with another type: replace it with one of the requested type.
                r = mgr.Add3(target, code, v, (int)swCustomPropertyAddOption_e.swCustomPropertyDeleteAndAdd);
                if (r == 0) ctx.Warn($"'{target}' was a {previous?.Type ?? "different"} property; it is now a {TypeName(code)} property.");
            }
            if (r != 0)
            {
                // A failed delete-and-add may already have removed the old property: put it back.
                string restored = previous is not null && ReadProp(mgr, target) is null ? " " + RestoreProp(mgr, target, previous, previousCode) : "";
                throw AddError(r, target, v, code, restored);
            }

            var now = ReadProp(mgr, target);
            bool dateReformatted = now is not null && code == (int)swCustomInfoType_e.swCustomInfoDate && !SameValue(code, now.Value, v);
            if (now is null || (!dateReformatted && !SameValue(code, now.Value, v)))
                throw new SwException(ErrorCodes.VerifyFailed,
                    (now is null ? $"'{target}' is missing after SOLIDWORKS reported it as added." : $"'{target}' reads '{now.Value}' after setting '{v}'.") +
                    " " + RestoreProp(mgr, target, previous, previousCode),
                    "SOLIDWORKS may have rejected the value for this type; try type 'text'.");
            if (dateReformatted) ctx.Warn($"SOLIDWORKS stored the date as '{now.Value}' (requested '{v}').");
            if (!SameType(code, now.Type))
                ctx.Warn($"'{target}' reads back as type {now.Type} (requested {TypeName(code)}).");
            return new PropSetResult { Doc = ctx.Title(d), Configuration = cfg.Length == 0 ? null : cfg, Property = now, Previous = previous };
        }, RunOptions.Write(doc), ct);

    public Task<PropDeleteResult> PropDeleteAsync(string? doc, string name, string? configuration = null, CancellationToken ct = default) =>
        session.RunAsync("prop_delete", ctx =>
        {
            string n = PropName(name);
            var d = ctx.Doc;
            string cfg = ConfigArg(d, configuration, "configuration");
            var mgr = PropManager(d, cfg);
            var names = PropNames(mgr);
            string where = cfg.Length == 0 ? "at file level" : $"in configuration '{cfg}'";
            string existing = FindProp(names, n)
                              ?? throw new SwException(ErrorCodes.EntityNotFound, $"No custom property '{n}' {where}.",
                                  names.Length == 0 ? "There are no custom properties there (configuration-specific ones need 'configuration')."
                                      : $"Properties: {string.Join(", ", names.Take(20))}{(names.Length > 20 ? ", …" : "")}.");
            var before = ReadProp(mgr, existing) ?? new CustomPropRow { Name = existing, Value = "", Type = "unknown" };
            int r = mgr.Delete2(existing);
            if (r == (int)swCustomInfoDeleteResult_e.swCustomInfoDeleteResult_LinkedProp)
                throw new SwException(ErrorCodes.NotSupported, $"'{existing}' is linked to its parent (a derived or cut-list property) and can't be deleted here.",
                    "Change it where it is defined.");
            if (r == (int)swCustomInfoDeleteResult_e.swCustomInfoDeleteResult_NotPresent)
                throw new SwException(ErrorCodes.EntityNotFound, $"SOLIDWORKS reports no property '{existing}' {where}.");
            if (r != 0) throw new SwException(ErrorCodes.FeatureFailed, $"Deleting '{existing}' failed (result {r}).");
            var after = PropNames(mgr);
            if (FindProp(after, existing) is not null)
                throw new SwException(ErrorCodes.VerifyFailed, $"'{existing}' is still present after SOLIDWORKS reported it deleted.");
            return new PropDeleteResult { Doc = ctx.Title(d), Configuration = cfg.Length == 0 ? null : cfg, Deleted = before, Remaining = after.Length };
        }, RunOptions.Write(doc), ct);

    private static ICustomPropertyManager PropManager(IModelDoc2 d, string cfg) =>
        d.Extension.CustomPropertyManager[cfg]
        ?? throw new SwException(ErrorCodes.NotSupported, $"SOLIDWORKS returned no custom property manager for '{(cfg.Length == 0 ? "the file" : cfg)}'.");

    private static string[] PropNames(ICustomPropertyManager mgr) => ComArgs.Strings((object?)mgr.GetNames());

    private static string? FindProp(string[] names, string name) =>
        names.FirstOrDefault(x => x == name) ?? names.FirstOrDefault(x => x.Equals(name, StringComparison.OrdinalIgnoreCase));

    private static CustomPropRow? ReadProp(ICustomPropertyManager mgr, string name)
    {
        int r = mgr.Get6(name, false, out string raw, out string resolved, out _, out bool linked);
        if (r == (int)swCustomInfoGetResult_e.swCustomInfoGetResult_NotPresent) return null;
        raw ??= "";
        return new CustomPropRow
        {
            Name = name,
            Value = raw,
            Resolved = resolved is not null && resolved != raw ? resolved : null,
            Type = TypeName(mgr.GetType2(name)),
            Linked = linked ? true : null,
        };
    }

    private static string PropName(string? name)
    {
        string n = (name ?? "").Trim();
        if (n.Length == 0) throw SwException.InvalidArgument("'name' is required (the custom property's name).");
        if (n.Any(char.IsControl) || n.Length > 255) throw SwException.InvalidArgument("'name' must be at most 255 printable characters.");
        return n;
    }

    /// <summary>The swCustomInfoType_e code and the value to store, validated per type.</summary>
    internal static (int Code, string Value) PropValue(string? type, string? value)
    {
        string v = value ?? "";
        switch ((type ?? "text").Trim().ToLowerInvariant())
        {
            case "" or "text":
                return ((int)swCustomInfoType_e.swCustomInfoText, v);
            case "number":
            {
                string s = v.Trim();
                if (!double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double x) || !double.IsFinite(x))
                    throw SwException.InvalidArgument($"type 'number' needs a numeric value (got '{v}').",
                        "Use a dot for decimals (12.5); links like \"D1@Boss-Extrude1\" need type 'text'.");
                // VERIFIED live: Add3 with swCustomInfoNumber (3) only takes whole numbers ("12.5" fails, result 1);
                // swCustomInfoDouble (5) takes both, with a dot whatever the Windows separator, and reads back as type 3.
                return ((int)swCustomInfoType_e.swCustomInfoDouble, s);
            }
            case "yesno" or "yes_no" or "boolean" or "bool":
                return ((int)swCustomInfoType_e.swCustomInfoYesOrNo, v.Trim().ToLowerInvariant() switch
                {
                    "yes" or "y" or "true" or "1" => "Yes",
                    "no" or "n" or "false" or "0" => "No",
                    _ => throw SwException.InvalidArgument($"type 'yesno' needs Yes or No (got '{v}')."),
                });
            case "date":
                if (v.Trim().Length == 0) throw SwException.InvalidArgument("type 'date' needs a date, e.g. 2026-10-22.");
                return ((int)swCustomInfoType_e.swCustomInfoDate, v.Trim());
            default:
                throw SwException.InvalidArgument($"Unknown property type '{type}'.", "Use text, number, date or yesno.");
        }
    }

    internal static string TypeName(int code) => code switch
    {
        (int)swCustomInfoType_e.swCustomInfoText => "text",
        (int)swCustomInfoType_e.swCustomInfoNumber => "number",
        (int)swCustomInfoType_e.swCustomInfoDouble => "double",
        (int)swCustomInfoType_e.swCustomInfoYesOrNo => "yesno",
        (int)swCustomInfoType_e.swCustomInfoDate => "date",
        // sw2023: swCustomInfoEquation does not exist in the SW 2023 interop (the type came later).
        _ => "unknown",
    };

    private static bool SameType(int code, string readBack) =>
        readBack == TypeName(code) || (IsNumber(code) && readBack is "number" or "double");

    private static bool IsNumber(int code) => code is (int)swCustomInfoType_e.swCustomInfoNumber or (int)swCustomInfoType_e.swCustomInfoDouble;

    /// <summary>
    /// The stored raw value matches the request: numbers compared numerically, Yes/No without case, and a link
    /// ("\"D1@Boss-Extrude1\"") also in the document-qualified form SOLIDWORKS may store ("\"D1@Boss-Extrude1@Part1.SLDPRT\"").
    /// </summary>
    internal static bool SameValue(int code, string stored, string requested)
    {
        string a = stored.Trim(), b = requested.Trim();
        if (IsNumber(code))
        {
            bool pa = double.TryParse(a, NumberStyles.Float, CultureInfo.InvariantCulture, out double x) ||
                      double.TryParse(a, NumberStyles.Float, CultureInfo.CurrentCulture, out x);
            return pa && double.TryParse(b, NumberStyles.Float, CultureInfo.InvariantCulture, out double y) &&
                   Math.Abs(x - y) <= 1e-9 * Math.Max(1, Math.Abs(y));
        }
        if (code == (int)swCustomInfoType_e.swCustomInfoYesOrNo) return a.Equals(b, StringComparison.OrdinalIgnoreCase);
        if (string.Equals(a, b, StringComparison.Ordinal)) return true;
        bool link = b.Length > 2 && b[0] == '"' && b[^1] == '"' && b.IndexOf('"', 1) == b.Length - 1;
        return link && a.Length > b.Length && a[^1] == '"' && a.StartsWith(b[..^1] + "@", StringComparison.OrdinalIgnoreCase);
    }

    private static SwException AddError(int result, string name, string value, int code, string suffix = "") => result switch
    {
        (int)swCustomInfoAddResult_e.swCustomInfoAddResult_MismatchAgainstSpecifiedType => SwException.InvalidArgument(
            $"SOLIDWORKS rejected '{value}' as a {TypeName(code)} value for '{name}'.{suffix}",
            code == (int)swCustomInfoType_e.swCustomInfoNumber || code == (int)swCustomInfoType_e.swCustomInfoDate
                ? "It may expect the Windows number/date format; type 'text' stores the value as given."
                : "Check the value, or use type 'text'."),
        (int)swCustomInfoAddResult_e.swCustomInfoAddResult_MismatchAgainstExistingType or
        (int)swCustomInfoAddResult_e.swCustomInfoAddResult_MismatchAgainstLegacyTypes => SwException.InvalidArgument(
            $"'{name}' already exists with another type and SOLIDWORKS would not change it.{suffix}", "Delete it with prop_delete first, or use its current type."),
        _ => new SwException(ErrorCodes.FeatureFailed, $"SOLIDWORKS could not set '{name}' (result {result}).{suffix}",
            "The document may be read-only, or the property linked to a parent."),
    };

    /// <summary>After a failed prop_set: puts the previous property back, or removes the one the call added. Returns a report.</summary>
    private static string RestoreProp(ICustomPropertyManager mgr, string name, CustomPropRow? previous, int previousCode)
    {
        try
        {
            if (previous is null)
            {
                if (ReadProp(mgr, name) is null) return "Nothing was left behind.";
                mgr.Delete2(name);
                return ReadProp(mgr, name) is null ? "The new property was removed again." : "Could not remove it again; prop_list shows it.";
            }
            int code = previousCode > 0 ? previousCode : (int)swCustomInfoType_e.swCustomInfoText;
            mgr.Add3(name, code, previous.Value, (int)swCustomPropertyAddOption_e.swCustomPropertyDeleteAndAdd);
            return ReadProp(mgr, name) is { } back && back.Value == previous.Value
                ? $"Restored the previous value '{previous.Value}'."
                : "Could not restore the previous value; prop_list shows the current one.";
        }
        catch (COMException ex) when (!ComArgs.IsDisconnect(ex))
        {
            return $"Restoring the previous state failed ({ex.Message}).";
        }
    }

    // ------------------------------------------------------------------ equations

    public Task<EquationListResult> EquationListAsync(string? doc, CancellationToken ct = default) =>
        session.RunAsync("equation_list", ctx =>
        {
            var d = ctx.DocOfType(swDocumentTypes_e.swDocPART, swDocumentTypes_e.swDocASSEMBLY);
            return new EquationListResult { Doc = ctx.Title(d), Equations = ReadEquations(EquationMgr(d)) };
        }, RunOptions.Read(doc), ct);

    public Task<EquationSetResult> EquationSetAsync(string? doc, string? lhs, string? expression, CancellationToken ct = default) =>
        session.RunAsync("equation_set", ctx =>
        {
            var run = SetEquations(ctx, [new EquationPlan(EquationText.NormalizeLhs(lhs), EquationText.Expression(expression))]);
            var one = run.Changes[0];
            return new EquationSetResult
            {
                Doc = run.Doc, Equation = one.Equation, Created = one.Created, Previous = one.Previous, Dimension = one.Dimension,
                Frame = one.Frame, FeatureErrors = run.FeatureErrors,
                Checks = one.Checks, VolumeDeltaMm3 = run.VolumeDeltaMm3, Model = run.Model,
            };
        }, RunOptions.Write(doc) with { Timeout = RebuildTimeout }, ct);

    /// <summary>
    /// Several equations in one call: all checked before the first change, one rebuild, each verified as a single call
    /// verifies it, and every one put back when one fails. <paramref name="lhs"/> and <paramref name="expression"/> are
    /// the single call's arguments, refused here.
    /// </summary>
    public Task<EquationBatchResult> EquationSetManyAsync(string? doc, IReadOnlyList<EquationInput>? equations, string? lhs = null,
        string? expression = null, CancellationToken ct = default) =>
        session.RunAsync("equation_set", ctx =>
        {
            if (!string.IsNullOrWhiteSpace(lhs) || !string.IsNullOrWhiteSpace(expression))
                throw SwException.InvalidArgument("Give either 'equations' or 'lhs' with 'expression', not both.");
            var run = SetEquations(ctx, EquationText.RequireEquations(equations).Select(e => new EquationPlan(e.Lhs, e.Expression)).ToList());
            return new EquationBatchResult
            {
                Doc = run.Doc, Equations = run.Changes, VolumeDeltaMm3 = run.VolumeDeltaMm3, Model = run.Model, FeatureErrors = run.FeatureErrors,
            };
        }, RunOptions.Write(doc) with { Timeout = RebuildTimeout }, ct);

    private sealed record EquationPlan(string Key, string Expr)
    {
        public string Text => EquationText.Build(Key, Expr);
        public bool IsDim => Key.Contains('@');
    }

    private sealed record EquationRun(string Doc, List<EquationChange> Changes, double? VolumeDeltaMm3, ModelState? Model,
        List<FeatureError>? FeatureErrors);

    private sealed record EquationCheck(EquationRow? Row, DimensionInfo? Dimension, List<string> Checks, List<string> Problems);

    /// <summary>
    /// equation_set's work, for one equation or a batch: every row checked before the first change, the rows written so that
    /// a variable comes before the equations that use it, one rebuild, each row verified, and all of them put back when one
    /// fails. For a part the volume and the bodies are read before and after, as dimension_set does.
    /// </summary>
    private static EquationRun SetEquations(SwContext ctx, IReadOnlyList<EquationPlan> plan)
    {
        var d = ctx.DocOfType(swDocumentTypes_e.swDocPART, swDocumentTypes_e.swDocASSEMBLY);
        NoOpenSketch(ctx, d, "equation_set");
        foreach (var p in plan)
            if (p.IsDim && Dim(d, p.Key) is null)
                throw new SwException(ErrorCodes.EntityNotFound, $"No dimension named '{p.Key}'.",
                    "Dimension names look like D1@Sketch1 or D1@Boss-Extrude1; feature and sketch results list them.");
        var em = EquationMgr(d);
        var before = ReadEquations(em);
        var prevs = plan.Select(p => before.FirstOrDefault(e => EquationText.SameLhs(e.Lhs, p.Key))).ToList();
        // The equations as they will stand after the call: a name another row of the batch defines is a known one.
        var planned = before.Where(e => !plan.Any(p => EquationText.SameLhs(e.Lhs, p.Key)))
            .Concat(plan.Select(p => new EquationRow { Index = -1, Text = p.Text, Lhs = p.Key, Global = p.IsDim ? null : true })).ToList();
        foreach (var p in plan)
        {
            CheckReferences(d, p.Key, p.Expr, planned);
            var bare = EquationText.BareNumbers(p.Expr);
            if (bare.Count > 0)
                ctx.Warn($"Bare number(s) {string.Join(", ", bare)} in '{p.Expr}' are in the document's units; write units (60mm, 30deg) " +
                         "unless they are counts or factors.");
        }
        // A dimension that gets its first equation keeps the value that equation gave it once the equation is deleted
        // again, so a rollback sets its value from before back too.
        var dimBefore = plan.Select(p => p.IsDim && Dim(d, p.Key) is { } dim ? dim.SystemValue : (double?)null).ToList();
        bool part = d.GetType() == (int)swDocumentTypes_e.swDocPART;
        var snapBefore = part ? Snapshot.Take(d) : null;
        var errorsBefore = FeatureManagementService.ErrorMap(d);

        using var prefs = ctx.SystemPrefs();
        prefs.Toggle(SystemPrefIds.ShowErrorsEveryRebuild, false); // no modal What's Wrong box on a bad expression
        var written = new List<int>();
        foreach (int i in EquationText.WriteOrder(plan.Select(p => (p.Key, p.Expr)).ToList()))
        {
            var p = plan[i];
            // Looked up now, not taken from the list read before: an equation added by this call may have moved it.
            int at = prevs[i] is null ? -1 : IndexOfEquation(em, p.Key);
            if (at >= 0) em.Equation[at] = p.Text; // assigning Equation(i) updates in place (VERIFIED)
            else if (em.Add2(-1, p.Text, true) < 0)
            {
                var (why, hint) = EquationText.Refused(p.Key, p.Expr, p.IsDim);
                throw new SwException(ErrorCodes.FeatureFailed,
                    $"SOLIDWORKS rejected the equation {p.Text}{why}{(written.Count == 0 ? "" : " " + Restore())}", hint);
            }
            written.Add(i);
        }
        bool rebuilt = d.EditRebuild3(); // one rebuild for the whole batch

        var (lin, ang) = DocUnitFactors(d);
        var found = Verify();
        if (found.Any(c => c.Problems.Count > 0 && c.Row is not null))
        {
            // Equations are solved during the rebuild; give dependent features one full rebuild before calling it a failure.
            var unsettled = found.Select(c => c.Problems.Count > 0).ToList();
            rebuilt = d.ForceRebuild3(false);
            found = Verify();
            for (int i = 0; i < found.Count; i++)
                if (unsettled[i] && found[i].Problems.Count == 0) found[i].Checks.Add("values settled after a forced rebuild");
        }
        var failed = Enumerable.Range(0, plan.Count).Where(i => found[i].Problems.Count > 0).ToList();
        if (failed.Count > 0)
        {
            string rb = Restore();
            string what = plan.Count == 1
                ? $"equation_set {plan[0].Text}: {string.Join("; ", found[0].Problems)}"
                : $"equation_set, {failed.Count} of {plan.Count} equations: " +
                  string.Join(" | ", failed.Take(4).Select(i => $"{plan[i].Text}: {string.Join("; ", found[i].Problems)}")) +
                  (failed.Count > 4 ? $" | and {failed.Count - 4} more" : "");
            throw new SwException(ErrorCodes.VerifyFailed, $"{what}. {rb}",
                "Check the expression (units, quoted names, existing variables) and the dimension name.");
        }
        for (int i = 0; i < plan.Count; i++)
        {
            var now = found[i].Row!;
            if (!EquationText.SameText(now.Text, plan[i].Text)) ctx.Warn($"SOLIDWORKS stored the equation as {now.Text}.");
            if (now.Value == 0 && EquationText.Literal(plan[i].Expr) is null)
                ctx.Warn($"{now.Text} evaluates to 0; check the expression (SOLIDWORKS may keep an equation it can't evaluate).");
        }
        // Kept, not rolled back: the equations are what was asked for, and a session may mean to fix the features next. But
        // a change that breaks features must not read as a success (S58 a): each one is named, with its error.
        var newErrors = FeatureManagementService.NewErrors(errorsBefore, FeatureManagementService.ErrorMap(d));
        var broken = newErrors.Where(e => e.Warning != true).ToList();
        if (broken.Count > 0)
            ctx.Warn($"The equations went in, but {broken.Count} feature(s) now fail to rebuild: " +
                     string.Join("; ", broken.Take(8).Select(e => $"'{e.Feature}' ({e.Type}) error {e.Code} {e.Error}")) +
                     (broken.Count > 8 ? $"; and {broken.Count - 8} more" : "") +
                     ". The volume and bodies are the model without them; fix the values or undo.");
        foreach (var e in newErrors.Where(e => e.Warning == true))
            ctx.Warn($"'{e.Feature}' now has rebuild warning {e.Code} ({e.Error}).");
        if (!rebuilt && newErrors.Count == 0) ctx.Warn("The rebuild reported problems; feature_list shows features with errors.");
        var snapAfter = part ? Snapshot.Take(d) : null;
        return new EquationRun(ctx.Title(d),
            plan.Select((p, i) => new EquationChange
            {
                Equation = found[i].Row!, Created = prevs[i] is null, Previous = prevs[i], Dimension = found[i].Dimension,
                Frame = p.IsDim ? RefFrameOf(ctx, d, p.Key) : null, Checks = found[i].Checks,
            }).ToList(),
            part ? Units.Mm3(snapAfter!.Volume - snapBefore!.Volume) : null, snapAfter?.ToState(), newErrors.Count > 0 ? newErrors : null);

        List<EquationCheck> Verify()
        {
            var now = ReadEquations(em);
            return plan.Select((p, i) => VerifyEquation(d, now, p, prevs[i], lin, ang)).ToList();
        }

        // The last written first: a new variable goes after the equations that use it.
        string Restore() =>
            RestoreEquations(d, em, Enumerable.Reverse(written).Select(i => (plan[i].Key, prevs[i], dimBefore[i])).ToList(), snapBefore);
    }

    /// <summary>
    /// For a dimension of a reference plane or axis (an offset, an angle): where it lies now, so that a plane driven to the
    /// wrong side of its base shows (S58 c). Null for a dimension of any other feature.
    /// </summary>
    private static RefFrame? RefFrameOf(SwContext ctx, IModelDoc2 d, string key) =>
        EquationText.FeatureOf(key) is { } name && EntityResolver.FeatureByName(d, name) is { } f ? ReferenceService.FrameOf(ctx, f) : null;

    /// <summary>Tier B for one equation: it is there, evaluates to the literal it was given, and drives its dimension.</summary>
    private static EquationCheck VerifyEquation(IModelDoc2 d, IReadOnlyList<EquationRow> now, EquationPlan p, EquationRow? prev,
        double lin, double ang)
    {
        var checks = new List<string>();
        var row = now.FirstOrDefault(e => EquationText.SameLhs(e.Lhs, p.Key));
        if (row is null) return new(null, null, checks, [$"no equation for \"{p.Key}\" after the rebuild"]);
        // An equation changed in place to one SOLIDWORKS won't take (a depth of 0) is dropped without an error: the old
        // text stays, and the value and the dimension then agree with each other, on the old equation.
        if (prev is not null && !EquationText.SameText(row.Text, p.Text) && EquationText.SameText(row.Text, prev.Text))
            return new(row, null, checks,
                [$"SOLIDWORKS kept the previous equation {prev.Text} and did not take {p.Expr} (a value the dimension can't have, such as 0?)"]);
        checks.Add($"equation for \"{p.Key}\" present");
        if (row.Value is not double value) return new(row, null, checks, [$"{row.Text} does not evaluate to a number"]);
        var problems = new List<string>();
        DimensionInfo? dimInfo = null;
        if (EquationText.Literal(p.Expr) is { } lit)
        {
            if (LiteralAgrees(value, lit, lin, ang)) checks.Add($"value {F(value)} matches {p.Expr}");
            else problems.Add($"it evaluates to {F(value)}, which is not {p.Expr}");
        }
        if (p.IsDim && Dim(d, p.Key) is { } dim)
        {
            dimInfo = FeatureVerifier.Dimension(dim);
            double factor = dimInfo.Unit switch { "deg" => ang, "count" => 1, _ => lin };
            if (Agrees(value, dim.SystemValue, factor)) checks.Add($"{p.Key} = {F(dimInfo.Value)} {dimInfo.Unit}");
            else problems.Add($"{p.Key} is {F(dimInfo.Value)} {dimInfo.Unit} after the rebuild, but the equation evaluates to {F(value)}");
        }
        return new(row, dimInfo, checks, problems);
    }

    /// <summary>
    /// Before anything changes: every name the expression uses exists (SOLIDWORKS can keep an equation it can't evaluate,
    /// which then reads 0), and the new equation doesn't depend on its own left-hand side.
    /// </summary>
    private static void CheckReferences(IModelDoc2 d, string key, string expr, IReadOnlyList<EquationRow> equations)
    {
        var refs = EquationText.ExpressionReferences(expr);
        var others = equations.Where(e => !EquationText.SameLhs(e.Lhs, key)).ToList();
        if (EquationText.Cycle(key, refs, others.Select(e => (e.Lhs, e.Text))) is { } cycle)
            throw SwException.InvalidArgument($"\"{key}\" = {expr} would be circular: {string.Join(" -> ", cycle.Select(c => $"\"{c}\""))}.",
                "An equation can't depend on its own left-hand side, directly or through other equations.");
        var unknown = refs.Where(r => !EquationText.IsExternal(r) && !others.Any(e => EquationText.SameLhs(e.Lhs, r)) &&
                                      Dim(d, r) is null && EntityResolver.FeatureByName(d, r) is null)
            .Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (unknown.Count == 0) return;
        var globals = others.Where(e => e.Global == true).Select(e => e.Lhs).Take(12).ToList();
        throw new SwException(ErrorCodes.EntityNotFound,
            $"The expression uses {string.Join(", ", unknown.Take(6).Select(u => $"\"{u}\""))}, which is not a global variable, dimension or feature.",
            (globals.Count > 0 ? $"Global variables: {string.Join(", ", globals)}. " : "") +
            "Create the variable first (equation_set with lhs W), or use a dimension name such as D1@Sketch1.");
    }

    public Task<EquationDeleteResult> EquationDeleteAsync(string? doc, string? lhs, CancellationToken ct = default) =>
        session.RunAsync("equation_delete", ctx =>
        {
            if (string.IsNullOrWhiteSpace(lhs))
                throw SwException.InvalidArgument("Give 'lhs' (W or D1@Sketch1), or 'equations' for several: ['W', 'D1@Sketch1'].");
            var (d, deleted, remaining) = DeleteEquations(ctx, [EquationText.NormalizeLhs(lhs)]);
            return new EquationDeleteResult { Doc = ctx.Title(d), Deleted = deleted[0], Remaining = remaining };
        }, RunOptions.Write(doc) with { Timeout = RebuildTimeout }, ct);

    /// <summary>
    /// Several equations in one call (S54): all checked before the first is deleted, one rebuild. <paramref name="lhs"/> is the
    /// single call's argument, refused here.
    /// </summary>
    public Task<EquationDeleteBatchResult> EquationDeleteManyAsync(string? doc, IReadOnlyList<string>? equations, string? lhs = null,
        CancellationToken ct = default) =>
        session.RunAsync("equation_delete", ctx =>
        {
            if (!string.IsNullOrWhiteSpace(lhs)) throw SwException.InvalidArgument("Give either 'equations' or 'lhs', not both.");
            if (equations is null || equations.Count == 0)
                throw SwException.InvalidArgument("'equations' must name at least one left-hand side, e.g. ['W', 'D1@Sketch1'].");
            if (equations.Count > 100) throw SwException.InvalidArgument($"'equations' has {equations.Count} entries (at most 100 per call).");
            var keys = new List<string>();
            for (int i = 0; i < equations.Count; i++)
            {
                string key;
                try { key = EquationText.NormalizeLhs(equations[i]); }
                catch (SwException ex) when (ex.Code == ErrorCodes.InvalidArgument)
                {
                    throw SwException.InvalidArgument($"equations[{i}]: {ex.Message}", ex.Hint);
                }
                if (keys.Any(k => EquationText.SameLhs(k, key))) throw SwException.InvalidArgument($"\"{key}\" is listed twice in 'equations'.");
                keys.Add(key);
            }
            var (d, deleted, remaining) = DeleteEquations(ctx, keys);
            return new EquationDeleteBatchResult { Doc = ctx.Title(d), Deleted = deleted, Remaining = remaining };
        }, RunOptions.Write(doc) with { Timeout = RebuildTimeout }, ct);

    /// <summary>
    /// equation_delete's work for one left-hand side or several: every one found, and no global variable deleted that an
    /// equation staying uses, before anything changes; then deleted, one rebuild, and checked gone.
    /// </summary>
    private static (IModelDoc2 Doc, List<EquationRow> Deleted, int Remaining) DeleteEquations(SwContext ctx, IReadOnlyList<string> keys)
    {
        var d = ctx.DocOfType(swDocumentTypes_e.swDocPART, swDocumentTypes_e.swDocASSEMBLY);
        NoOpenSketch(ctx, d, "equation_delete");
        var em = EquationMgr(d);
        var all = ReadEquations(em);
        var missing = keys.Where(k => !all.Any(e => EquationText.SameLhs(e.Lhs, k))).ToList();
        if (missing.Count > 0)
            throw new SwException(ErrorCodes.EntityNotFound,
                $"No equation for {string.Join(", ", missing.Take(6).Select(k => $"\"{k}\""))}{(keys.Count > 1 ? "; nothing was deleted" : "")}.",
                all.Count == 0 ? "The document has no equations." : $"Left-hand sides: {string.Join(", ", all.Select(e => e.Lhs).Take(20))}.");
        var targets = keys.Select(k => all.First(e => EquationText.SameLhs(e.Lhs, k))).ToList();
        foreach (var target in targets.Where(t => !t.Lhs.Contains('@')))
        {
            // Deleting a global variable breaks every equation that uses it, unless those go in the same call.
            var users = all.Where(e => !targets.Any(t => t.Index == e.Index) &&
                                       EquationText.References(e.Text).Any(r => EquationText.SameLhs(r, target.Lhs))).ToList();
            if (users.Count > 0)
                throw SwException.InvalidArgument($"\"{target.Lhs}\" is used by {string.Join(", ", users.Select(u => u.Text).Take(6))}.",
                    "Delete or change those equations first, or in the same call (equation_set / equation_delete).");
        }
        using var prefs = ctx.SystemPrefs();
        prefs.Toggle(SystemPrefIds.ShowErrorsEveryRebuild, false);
        var left = RemoveEquations(d, em, targets, out var after);
        if (left.Count > 0)
            throw new SwException(ErrorCodes.VerifyFailed, targets.Count == 1
                ? $"The equation for \"{targets[0].Lhs}\" is still there after deleting it."
                : $"{string.Join(", ", left.Take(6).Select(t => $"\"{t.Lhs}\""))} still there after deleting {targets.Count} equations; " +
                  $"the other {targets.Count - left.Count} are gone.");
        if (after.Count != all.Count - targets.Count)
            ctx.Warn($"The document had {all.Count} equations and now has {after.Count}.");
        return (d, targets, after.Count);
    }

    /// <summary>
    /// Deletes <paramref name="rows"/> (as just read: their indices are current), the last first so that the indices hold, and
    /// rebuilds once. Returns the rows still there afterwards. The caller turns off the What's Wrong box.
    /// </summary>
    internal static List<EquationRow> RemoveEquations(IModelDoc2 d, IEquationMgr em, IReadOnlyList<EquationRow> rows, out List<EquationRow> after)
    {
        foreach (var r in rows.OrderByDescending(r => r.Index)) em.Delete(r.Index);
        d.EditRebuild3();
        var now = ReadEquations(em);
        after = now;
        return rows.Where(r => now.Any(e => EquationText.SameLhs(e.Lhs, r.Lhs))).ToList();
    }

    internal static IEquationMgr EquationMgr(IModelDoc2 d) =>
        d.GetEquationMgr() ?? throw new SwException(ErrorCodes.NotSupported, "SOLIDWORKS returned no equation manager for this document.");

    internal static List<EquationRow> ReadEquations(IEquationMgr em)
    {
        int n = em.GetCount();
        var list = new List<EquationRow>(Math.Max(0, n));
        for (int i = 0; i < n; i++)
        {
            string text = em.Equation[i] ?? "";
            double? value = null;
            try
            {
                double v = em.Value[i];
                if (double.IsFinite(v)) value = Units.R(v);
            }
            catch (COMException ex) when (!ComArgs.IsDisconnect(ex)) { /* an equation with an error has no value */ }
            bool global = false;
            try { global = em.GlobalVariable[i]; }
            catch (COMException ex) when (!ComArgs.IsDisconnect(ex)) { /* older documents */ }
            list.Add(new EquationRow { Index = i, Text = text, Lhs = EquationText.Lhs(text) ?? "", Value = value, Global = global ? true : null });
        }
        return list;
    }

    /// <summary>The index of the equation with that left-hand side, or -1 (the texts only, no values read).</summary>
    private static int IndexOfEquation(IEquationMgr em, string key)
    {
        int n = em.GetCount();
        for (int i = 0; i < n; i++)
            if (EquationText.Lhs(em.Equation[i] ?? "") is { } lhs && EquationText.SameLhs(lhs, key)) return i;
        return -1;
    }

    /// <summary>
    /// After a failed equation_set: puts every equation the call wrote back as it was (or removes the new ones), in the order
    /// given, and sets a dimension whose first equation was removed back to its value from before. Returns a sentence for
    /// the error message; for a part the volume is compared with the one before the call.
    /// </summary>
    private static string RestoreEquations(IModelDoc2 d, IEquationMgr em,
        IReadOnlyList<(string Key, EquationRow? Prev, double? DimBefore)> items, Snapshot? before)
    {
        if (items.Count == 0) return "Nothing was changed.";
        const string look = "equation_list shows the current state.";
        try
        {
            foreach (var (key, prev, _) in items)
            {
                int at = IndexOfEquation(em, key);
                if (prev is null)
                {
                    if (at >= 0) em.Delete(at);
                }
                else if (at >= 0) em.Equation[at] = prev.Text;
                else em.Add2(-1, prev.Text, true);
            }
            d.EditRebuild3();
            // Only once the rebuild has let go of them: a dimension still read as equation-driven refuses a value.
            bool reset = false;
            foreach (var (key, prev, dimBefore) in items)
            {
                if (prev is not null || dimBefore is not double v || Dim(d, key) is not { } dim || Close(dim.SystemValue, v)) continue;
                dim.SetSystemValue3(v, (int)swSetValueInConfiguration_e.swSetValue_InThisConfiguration, ComArgs.Empty);
                reset = true;
            }
            if (reset) d.EditRebuild3();

            var check = ReadEquations(em);
            var bad = items.Where(i =>
            {
                var c = check.FirstOrDefault(e => EquationText.SameLhs(e.Lhs, i.Key));
                return i.Prev is null ? c is not null : c is null || !EquationText.SameText(c.Text, i.Prev.Text);
            }).Select(i => i.Key).ToList();
            if (bad.Count > 0)
                return items.Count == 1 ? $"Rollback incomplete: {look}"
                    : $"Rollback incomplete ({string.Join(", ", bad.Take(6))}): {look}";
            if (before is not null && !SameVolume(Snapshot.Take(d), before))
            {
                d.ForceRebuild3(false);
                if (!SameVolume(Snapshot.Take(d), before))
                    return "The equations were put back, but the model's volume is not what it was before the call; model_info shows it.";
            }
            if (items.Count > 1) return "Rolled back: every equation is as it was before the call.";
            return items[0].Prev is null ? "Rolled back: the new equation was removed." : $"Rolled back to {items[0].Prev!.Text}.";
        }
        catch (COMException ex) when (!ComArgs.IsDisconnect(ex))
        {
            return $"Rollback failed ({ex.Message}); {look}";
        }

        static bool SameVolume(Snapshot a, Snapshot b) => Math.Abs(a.Volume - b.Volume) <= 1e-12 + 1e-9 * Math.Abs(b.Volume);
    }

    /// <summary>The dimension of that name (D1@Sketch1), or null; IModelDoc2.Parameter may fault on names it can't parse.</summary>
    internal static IDimension? Dim(IModelDoc2 d, string name)
    {
        try { return (object?)d.Parameter(name) as IDimension; }
        catch (COMException ex) when (!ComArgs.IsDisconnect(ex)) { return null; }
    }

    /// <summary>Meters per document length unit and radians per document angle unit.</summary>
    private static (double Linear, double Angular) DocUnitFactors(IModelDoc2 d)
    {
        int lu = d.Extension.GetUserPreferenceInteger((int)swUserPreferenceIntegerValue_e.swUnitsLinear, 0);
        int au = d.Extension.GetUserPreferenceInteger((int)swUserPreferenceIntegerValue_e.swUnitsAngular, 0);
        double lin = lu switch
        {
            (int)swLengthUnit_e.swMM => 1e-3,
            (int)swLengthUnit_e.swCM => 1e-2,
            (int)swLengthUnit_e.swMETER => 1,
            (int)swLengthUnit_e.swINCHES or (int)swLengthUnit_e.swFEETINCHES => 0.0254,
            (int)swLengthUnit_e.swFEET => 0.3048,
            (int)swLengthUnit_e.swANGSTROM => 1e-10,
            (int)swLengthUnit_e.swNANOMETER => 1e-9,
            (int)swLengthUnit_e.swMICRON => 1e-6,
            (int)swLengthUnit_e.swMIL => 2.54e-5,
            (int)swLengthUnit_e.swUIN => 2.54e-8,
            _ => 1e-3,
        };
        double ang = au == (int)swAngleUnit_e.swRADIANS ? 1 : Math.PI / 180;
        return (lin, ang);
    }

    /// <summary>
    /// A value SOLIDWORKS reports agrees with an SI quantity, read either in SI or in document units (which one
    /// IEquationMgr.Value uses is not pinned down, so both are accepted; the check still catches wrong values).
    /// </summary>
    private static bool Agrees(double reported, double si, double docFactor) =>
        Close(reported, si) || (docFactor > 0 && Close(reported, si / docFactor));

    private static bool LiteralAgrees(double reported, (double Value, string Unit) lit, double lin, double ang)
    {
        if (lit.Unit.Length == 0) return Close(reported, lit.Value) || Close(reported, lit.Value * lin) || Close(reported, lit.Value * ang);
        if (EquationText.UnitFactor(lit.Unit) is not { } u) return true; // a unit this check doesn't know: not judged
        return Agrees(reported, lit.Value * u.Factor, u.Angle ? ang : lin);
    }

    private static bool Close(double a, double b) => Math.Abs(a - b) <= 1e-6 * Math.Max(1, Math.Abs(b));

    // ------------------------------------------------------------------ configurations

    public Task<ConfigListResult> ConfigListAsync(string? doc, CancellationToken ct = default) =>
        session.RunAsync("config_list", ctx =>
        {
            var d = ctx.DocOfType(swDocumentTypes_e.swDocPART, swDocumentTypes_e.swDocASSEMBLY);
            return new ConfigListResult
            {
                Doc = ctx.Title(d), Configurations = ConfigNames(d), Active = ActiveConfig(d),
                DisplayStates = DisplayStates(d) is { Length: > 0 } ds ? ds : null,
            };
        }, RunOptions.Read(doc), ct);

    public Task<ConfigChangeResult> ConfigAddAsync(string? doc, string name, bool activate = true, CancellationToken ct = default) =>
        session.RunAsync("config_add", ctx =>
        {
            string n = (name ?? "").Trim();
            if (n.Length == 0) throw SwException.InvalidArgument("'name' is required (the new configuration's name).");
            if (n.Any(char.IsControl)) throw SwException.InvalidArgument("'name' must not contain control characters.");
            var d = ctx.DocOfType(swDocumentTypes_e.swDocPART, swDocumentTypes_e.swDocASSEMBLY);
            NoOpenSketch(ctx, d, "config_add");
            var names = ConfigNames(d);
            if (names.FirstOrDefault(x => x.Equals(n, StringComparison.OrdinalIgnoreCase)) is { } clash)
                throw SwException.InvalidArgument($"A configuration named '{clash}' already exists.", "Activate it with config_activate, or choose another name.");
            string previous = ActiveConfig(d);

            using var prefs = ctx.SystemPrefs();
            prefs.Toggle(SystemPrefIds.ShowErrorsEveryRebuild, false);
            var made = (object?)d.AddConfiguration3(n, "", "", 0) as IConfiguration; // creates AND activates it (VERIFIED)
            var after = ConfigNames(d);
            if (!after.Contains(n, StringComparer.Ordinal))
            {
                var added = after.Where(x => !names.Contains(x, StringComparer.Ordinal)).ToList();
                if (added.Count != 1)
                    throw new SwException(ErrorCodes.FeatureFailed,
                        $"SOLIDWORKS did not create configuration '{n}'{(made is null ? " (AddConfiguration3 returned nothing)" : "")}.",
                        "Try a simpler name (letters, digits, spaces, '-' and '_').");
                ctx.Warn($"SOLIDWORKS named the new configuration '{added[0]}' (requested '{n}').");
                n = added[0];
            }
            string active = ActiveConfig(d);
            string? message = null;
            string want = activate ? n : previous;
            if (active != want)
            {
                d.ShowConfiguration2(want);
                active = ActiveConfig(d);
                if (active != want)
                    ctx.Warn($"'{n}' was created, but '{want}' could not be (re)activated; '{active}' is active.");
            }
            if (!activate) message = $"Created '{n}'; '{active}' stays active.";
            return new ConfigChangeResult
            {
                Doc = ctx.Title(d), Configuration = n, Active = active, Previous = previous, Configurations = after, Message = message,
            };
        }, RunOptions.Write(doc) with { Timeout = RebuildTimeout }, ct);

    public Task<ConfigChangeResult> ConfigActivateAsync(string? doc, string name, CancellationToken ct = default) =>
        session.RunAsync("config_activate", ctx =>
        {
            var d = ctx.DocOfType(swDocumentTypes_e.swDocPART, swDocumentTypes_e.swDocASSEMBLY);
            string target = ExistingConfig(d, name);
            string previous = ActiveConfig(d);
            if (target == previous)
                return new ConfigChangeResult
                {
                    Doc = ctx.Title(d), Configuration = target, Active = previous, Previous = previous, Configurations = ConfigNames(d),
                    Message = $"'{target}' was already active.",
                };
            NoOpenSketch(ctx, d, "config_activate");
            using var prefs = ctx.SystemPrefs();
            prefs.Toggle(SystemPrefIds.ShowErrorsEveryRebuild, false);
            bool ok = d.ShowConfiguration2(target);
            string active = ActiveConfig(d);
            if (active != target)
                throw new SwException(ErrorCodes.VerifyFailed,
                    $"'{target}' did not become active (ShowConfiguration2 returned {ok}; '{active}' is active).",
                    "A modal dialog may have interrupted it (dialog_list), or the configuration is a SpeedPak/derived one SOLIDWORKS can't show.");
            return new ConfigChangeResult { Doc = ctx.Title(d), Configuration = target, Active = active, Previous = previous, Configurations = ConfigNames(d) };
        }, RunOptions.Write(doc) with { Timeout = RebuildTimeout }, ct);

    public Task<ConfigChangeResult> ConfigDeleteAsync(string? doc, string name, CancellationToken ct = default) =>
        session.RunAsync("config_delete", ctx =>
        {
            var d = ctx.DocOfType(swDocumentTypes_e.swDocPART, swDocumentTypes_e.swDocASSEMBLY);
            string target = ExistingConfig(d, name);
            string active = ActiveConfig(d);
            if (target == active)
                throw SwException.InvalidArgument($"'{target}' is the active configuration, which SOLIDWORKS can't delete.",
                    "Activate another one with config_activate first.");
            var before = ConfigNames(d);
            bool ok = d.DeleteConfiguration2(target);
            var after = ConfigNames(d);
            if (after.Contains(target, StringComparer.Ordinal))
                throw new SwException(ok ? ErrorCodes.VerifyFailed : ErrorCodes.FeatureFailed,
                    $"Configuration '{target}' still exists (DeleteConfiguration2 returned {ok}).",
                    "It may be referenced (an assembly component or a drawing view uses it) or locked.");
            var lost = before.Where(x => x != target && !after.Contains(x, StringComparer.Ordinal)).ToList();
            if (lost.Count > 0) ctx.Warn($"SOLIDWORKS also deleted: {string.Join(", ", lost)} (derived configurations).");
            if (ActiveConfig(d) != active) ctx.Warn($"The active configuration changed to '{ActiveConfig(d)}'.");
            return new ConfigChangeResult
            {
                Doc = ctx.Title(d), Configuration = target, Active = ActiveConfig(d), Previous = active, Configurations = after,
                Message = $"Deleted '{target}'.",
            };
        }, RunOptions.Write(doc), ct);

    private static string[] ConfigNames(IModelDoc2 d) => ComArgs.Strings((object?)d.GetConfigurationNames());

    private static string ActiveConfig(IModelDoc2 d) => d.ConfigurationManager?.ActiveConfiguration?.Name ?? "";

    /// <summary>The canonical name of an existing configuration (exact, then case-insensitive).</summary>
    private static string ExistingConfig(IModelDoc2 d, string? name, string argName = "name")
    {
        string want = (name ?? "").Trim();
        if (want.Length == 0) throw SwException.InvalidArgument($"'{argName}' is required (a configuration name; config_list shows them).");
        var names = ConfigNames(d);
        return names.FirstOrDefault(x => x == want) ?? names.FirstOrDefault(x => x.Equals(want, StringComparison.OrdinalIgnoreCase))
               ?? throw new SwException(ErrorCodes.EntityNotFound, $"No configuration named '{want}'.",
                   $"Configurations: {string.Join(", ", names.Take(20))}{(names.Length > 20 ? ", …" : "")}.");
    }

    /// <summary>"" for none (file level / the active configuration), else an existing configuration's name.</summary>
    private static string ConfigArg(IModelDoc2 d, string? configuration, string argName)
    {
        if (string.IsNullOrWhiteSpace(configuration)) return "";
        if (d.GetType() == (int)swDocumentTypes_e.swDocDRAWING)
            throw SwException.InvalidArgument($"Drawings have no configurations; omit '{argName}'.");
        return ExistingConfig(d, configuration, argName);
    }

    private static void NoOpenSketch(SwContext ctx, IModelDoc2 d, string tool)
    {
        if (d.SketchManager?.ActiveSketch is not ISketch open) return;
        string name = ((IFeature)open).Name;
        throw new SwException(ErrorCodes.SketchState, $"Sketch '{name}' is open; {tool} rebuilds the model, which would close it.",
            ctx.DocEntry.ServerSketch == name ? "Finish it with sketch_finish first." : "The user is editing it; ask them to exit the sketch.");
    }

    // ------------------------------------------------------------------ appearance

    public Task<AppearanceSetResult> AppearanceSetAsync(string? doc, int[]? colorRgb, double? transparency = null, EntityRef? target = null,
        CancellationToken ct = default) =>
        session.RunAsync("appearance_set", ctx =>
        {
            var rgb = Appearance.Rgb(colorRgb);
            if (transparency is double t && (!double.IsFinite(t) || t < 0 || t > 1))
                throw SwException.InvalidArgument($"'transparency' must be between 0 (opaque) and 1 (clear) (got {F(t)}).");
            var d = ctx.Doc;
            ctx.Part();
            Resolved? r = null;
            if (target is not null)
            {
                // Faces have no names, so a bare name means a body (otherwise it would match the feature of the same name).
                var tr = target is { Name: not null, Type: null, Id: null, At: null, Where: null }
                    ? new EntityRef { Name = target.Name, Type = "body" }
                    : target;
                r = EntityResolver.Resolve(ctx, d, tr, "target", EntityKind.Body, EntityKind.Face);
            }
            if (rgb.All(c => c <= 1) && rgb.Any(c => c == 1))
                ctx.Warn($"colorRgb is 0-255, so [{string.Join(", ", rgb)}] is almost black; multiply 0-1 values by 255 (red is [255, 0, 0]).");
            string label = r?.Label ?? "part";
            double[] prev = Appearance.Read(d, r?.Obj);
            // A body or face without an appearance of its own starts from the part's lighting values.
            double[] basis = r is null || Appearance.Valid(prev) ? prev : Appearance.Read(d, null);
            double[] values = Appearance.Compose(basis, rgb, transparency);
            Appearance.Write(d, r?.Obj, values);
            d.GraphicsRedraw2();
            double[] now = Appearance.Read(d, r?.Obj);
            if (!Appearance.Matches(now, values))
            {
                string restored = "";
                if (Appearance.Valid(prev))
                {
                    Appearance.Write(d, r?.Obj, prev);
                    d.GraphicsRedraw2();
                    restored = Appearance.Matches(Appearance.Read(d, r?.Obj), prev)
                        ? " The previous appearance was restored." : " The previous appearance could not be restored.";
                }
                throw new SwException(ErrorCodes.VerifyFailed,
                    $"The appearance of {label} reads [{string.Join(", ", now.Select(F))}] after setting [{string.Join(", ", values.Select(F))}].{restored}",
                    r is null ? "A body or face appearance may override the part color; target it directly." : "Try the whole part (omit 'target').");
            }
            return new AppearanceSetResult
            {
                Doc = ctx.Title(d),
                Target = label,
                Appearance = Appearance.Row(now)!,
                Previous = Appearance.Row(prev),
                Resolved = r?.Info("target"),
            };
        }, RunOptions.Write(doc), ct);

    /// <summary>The legacy "material property values": r, g, b (0-1), ambient, diffuse, specular, shininess, transparency, emission.</summary>
    private static class Appearance
    {
        public static double[] Read(IModelDoc2 d, object? target) => target switch
        {
            IBody2 b => ComArgs.Doubles((object?)b.MaterialPropertyValues2),
            IFace2 f => ComArgs.Doubles((object?)f.MaterialPropertyValues),
            _ => ComArgs.Doubles((object?)d.MaterialPropertyValues),
        };

        public static void Write(IModelDoc2 d, object? target, double[] v)
        {
            switch (target)
            {
                case IBody2 b: b.MaterialPropertyValues2 = v; break;
                case IFace2 f: f.MaterialPropertyValues = v; break;
                default: d.MaterialPropertyValues = v; break; // VERIFIED (red part)
            }
        }

        public static int[] Rgb(int[]? rgb)
        {
            if (rgb is null || rgb.Length != 3)
                throw SwException.InvalidArgument("'colorRgb' must be [r, g, b] with 0-255 each, e.g. [255, 0, 0].");
            if (rgb.Any(c => c is < 0 or > 255))
                throw SwException.InvalidArgument($"'colorRgb' values must be 0-255 (got [{string.Join(", ", rgb)}]).");
            return rgb;
        }

        /// <summary>Nine finite values in 0-1: an appearance of the entity's own.</summary>
        public static bool Valid(double[] v) => v.Length >= 9 && v.Take(9).All(x => double.IsFinite(x) && x >= 0 && x <= 1);

        /// <summary>New values: the requested color and transparency, the other lighting values kept when sane.</summary>
        public static double[] Compose(double[] prev, int[] rgb, double? transparency)
        {
            bool keep = Valid(prev);
            return
            [
                rgb[0] / 255.0, rgb[1] / 255.0, rgb[2] / 255.0,
                keep ? prev[3] : 1.0,   // ambient
                keep ? prev[4] : 1.0,   // diffuse
                keep ? prev[5] : 0.5,   // specular
                keep ? prev[6] : 0.4,   // shininess
                transparency ?? (keep ? prev[7] : 0.0),
                keep ? prev[8] : 0.0,   // emission
            ];
        }

        public static bool Matches(double[] now, double[] want) =>
            now.Length >= 9 && new[] { 0, 1, 2, 7 }.All(i => Math.Abs(now[i] - want[i]) <= 2e-3);

        public static AppearanceRow? Row(double[] v)
        {
            if (v.Length < 9 || v.Take(3).Any(x => !double.IsFinite(x) || x < 0 || x > 1)) return null;
            return new AppearanceRow
            {
                ColorRgb = [To255(v[0]), To255(v[1]), To255(v[2])],
                Transparency = double.IsFinite(v[7]) ? Math.Round(Math.Clamp(v[7], 0, 1), 4) : 0,
            };
        }

        private static int To255(double x) => (int)Math.Round(Math.Clamp(x, 0, 1) * 255);
    }

    private static string F(double v) => v.ToString("0.######", CultureInfo.InvariantCulture);
}

// ====================================================================== interop-free helpers

/// <summary>
/// The installed material databases: SOLIDWORKS .sldmat files (UTF-16 XML with classification / material elements and
/// the mass density in physicalproperties/DENS, kg/m³). Each file is parsed once per version and cached for the process.
/// </summary>
internal static class MaterialCatalog
{
    internal sealed record Entry(string Name, string Classification, string Database, double? DensityKgM3)
    {
        public MaterialRow ToRow() => new() { Name = Name, Classification = Classification, Database = Database, DensityKgM3 = DensityKgM3 };
    }

    /// <summary>A database file that was found and could not be read or parsed.</summary>
    internal sealed record Unreadable(string Database, string File, string Error);

    internal sealed record Catalog(IReadOnlyList<Entry> Materials, IReadOnlyList<MaterialDatabaseRow> Databases, IReadOnlyList<string> Warnings,
        IReadOnlyList<Unreadable> Broken);

    /// <summary>The standard database as the API names it (its file is "solidworks materials.sldmat").</summary>
    public const string DefaultDatabase = "SOLIDWORKS Materials";

    private static readonly object Gate = new();
    private static readonly Dictionary<string, (DateTime Stamp, long Length, List<Entry> Rows)> Cache = new(StringComparer.OrdinalIgnoreCase);

    public static bool IsDefault(string database) => database.Equals(DefaultDatabase, StringComparison.OrdinalIgnoreCase);

    /// <summary>The name SetMaterialPropertyName2 takes: the file name without extension ("SOLIDWORKS Materials" for the standard one).</summary>
    public static string ApiName(string fileStem) => IsDefault(fileStem) ? DefaultDatabase : fileStem;

    /// <summary>The database SOLIDWORKS reports is the one requested (by name, or by a path to it).</summary>
    public static bool SameDatabase(string? reported, string requested)
    {
        if (string.IsNullOrWhiteSpace(reported)) return true;
        string r = reported.Trim();
        if (r.Equals(requested, StringComparison.OrdinalIgnoreCase)) return true;
        try { return Path.GetFileNameWithoutExtension(r).Equals(requested, StringComparison.OrdinalIgnoreCase); }
        catch (ArgumentException) { return false; }
    }

    /// <summary>All materials of the *.sldmat files in the ';'-separated folders (top level of each folder).</summary>
    public static Catalog Load(string folderList, bool allowNetwork)
    {
        var warnings = new List<string>();
        var files = new List<(int Folder, string File, string Db)>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        int folderIndex = 0;
        foreach (var raw in (folderList ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            string folder = Environment.ExpandEnvironmentVariables(raw.Trim('"'));
            if (folder.Length == 0 || !seen.Add(folder.TrimEnd('\\', '/'))) continue;
            if (!allowNetwork && (folder.StartsWith(@"\\", StringComparison.Ordinal) || folder.StartsWith("//", StringComparison.Ordinal)))
            {
                warnings.Add($"Skipped the network folder '{folder}' (SWMCP_ALLOW_NETWORK_PATHS=1 allows it).");
                continue;
            }
            try
            {
                if (!Directory.Exists(folder)) continue;
                foreach (var f in Directory.GetFiles(folder, "*.sldmat", SearchOption.TopDirectoryOnly))
                    files.Add((folderIndex, f, ApiName(Path.GetFileNameWithoutExtension(f))));
                folderIndex++;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
            {
                warnings.Add($"Could not read '{folder}': {ex.Message}");
            }
        }

        // The standard database first, then the others in folder and name order.
        var materials = new List<Entry>();
        var dbs = new List<MaterialDatabaseRow>();
        var broken = new List<Unreadable>();
        foreach (var (_, file, db) in files.OrderBy(x => IsDefault(x.Db) ? 0 : 1).ThenBy(x => x.Folder)
                     .ThenBy(x => x.File, StringComparer.OrdinalIgnoreCase))
        {
            List<Entry> rows;
            try { rows = Cached(file, db); }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or XmlException)
            {
                warnings.Add($"Could not parse '{file}': {ex.Message}");
                broken.Add(new Unreadable(db, file, ex.Message));
                continue;
            }
            materials.AddRange(rows);
            dbs.Add(new MaterialDatabaseRow { Name = db, Path = file, Materials = rows.Count });
        }
        if (dbs.Count == 0 && broken.Count == 0)
            warnings.Add($"No material database (*.sldmat) found in '{folderList}' (Tools > Options > File Locations > Material Databases).");
        return new Catalog(materials, dbs, warnings, broken);
    }

    /// <summary>The database file no longer parses (null when it does, or can't be read at all): SOLIDWORKS rewrote it.</summary>
    public static string? ParseError(string file)
    {
        try { Parse(file, ""); return null; }
        catch (XmlException ex) { return ex.Message; }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { return null; }
    }

    private static List<Entry> Cached(string file, string database)
    {
        var fi = new FileInfo(file);
        lock (Gate)
        {
            if (Cache.TryGetValue(file, out var c) && c.Stamp == fi.LastWriteTimeUtc && c.Length == fi.Length) return c.Rows;
        }
        var rows = Parse(file, database);
        lock (Gate) Cache[file] = (fi.LastWriteTimeUtc, fi.Length, rows);
        return rows;
    }

    /// <summary>Parses one database file (any encoding with a BOM or declaration; no DTDs, no external resources).</summary>
    public static List<Entry> Parse(string file, string database)
    {
        using var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return Parse(fs, database);
    }

    public static List<Entry> Parse(Stream stream, string database)
    {
        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            IgnoreComments = true,
            IgnoreWhitespace = true,
            IgnoreProcessingInstructions = true,
        };
        using var r = XmlReader.Create(stream, settings);
        var rows = new List<Entry>();
        var classes = new Stack<(string Name, int Depth)>();
        string? material = null;
        int materialDepth = -1, physDepth = -1;
        double? density = null;
        while (r.Read())
        {
            if (r.NodeType == XmlNodeType.Element)
            {
                string ln = r.LocalName;
                int depth = r.Depth;
                bool empty = r.IsEmptyElement;
                if (material is null)
                {
                    if (ln == "classification")
                    {
                        if (!empty) classes.Push((r.GetAttribute("name") ?? "", depth));
                    }
                    else if (ln == "material" && r.GetAttribute("name") is { Length: > 0 } name)
                    {
                        if (empty) rows.Add(new Entry(name, Class(classes), database, null));
                        else { material = name; materialDepth = depth; density = null; physDepth = -1; }
                    }
                }
                else if (ln == "physicalproperties" && depth == materialDepth + 1 && !empty) physDepth = depth;
                else if (physDepth >= 0 && depth == physDepth + 1 && ln == "DENS") density = Number(r.GetAttribute("value"));
            }
            else if (r.NodeType == XmlNodeType.EndElement)
            {
                string ln = r.LocalName;
                int depth = r.Depth;
                if (material is not null)
                {
                    if (ln == "physicalproperties" && depth == physDepth) physDepth = -1;
                    else if (ln == "material" && depth == materialDepth)
                    {
                        rows.Add(new Entry(material, Class(classes), database, density));
                        material = null;
                    }
                }
                else if (ln == "classification" && classes.Count > 0 && classes.Peek().Depth == depth) classes.Pop();
            }
        }
        return rows;
    }

    private static string Class(Stack<(string Name, int Depth)> classes) => classes.Count > 0 ? classes.Peek().Name : "";

    private static double? Number(string? s) =>
        double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double v) && double.IsFinite(v) && v > 0 ? v : null;

    /// <summary>Up to <paramref name="max"/> names closest to <paramref name="query"/>: shared words first, then spelling.</summary>
    public static List<string> Suggest(IEnumerable<string> names, string query, int max = 5)
    {
        string q = Squash(query);
        var qt = Words(query);
        var scored = new List<(string Name, double Score, int Dist)>();
        foreach (var n in names.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            string s = Squash(n);
            var nt = Words(n);
            double score = q.Length > 0 && (s.Contains(q, StringComparison.Ordinal) || (s.Length >= 3 && q.Contains(s, StringComparison.Ordinal))) ? 2 : 0;
            foreach (var a in qt)
            {
                double best = 0;
                foreach (var b in nt)
                {
                    double sim = a == b ? 1
                        : (a.Length >= 2 && b.StartsWith(a, StringComparison.Ordinal)) || (b.Length >= 2 && a.StartsWith(b, StringComparison.Ordinal)) ? 0.7
                        : Similarity(a, b);
                    best = Math.Max(best, sim);
                }
                score += best;
            }
            scored.Add((n, score, Levenshtein(q, s)));
        }
        var ranked = scored.Where(x => x.Score > 0).OrderByDescending(x => x.Score).ThenBy(x => x.Dist)
            .ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase).Select(x => x.Name).Take(max).ToList();
        if (ranked.Count < max)
            ranked.AddRange(scored.Where(x => x.Score == 0 && x.Dist <= Math.Max(2, q.Length / 3)).OrderBy(x => x.Dist)
                .ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase).Select(x => x.Name).Take(max - ranked.Count));
        return ranked;
    }

    private static string Squash(string s) => new(s.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());

    private static List<string> Words(string s) =>
        Regex.Split(s.ToLowerInvariant(), @"[^\p{L}\p{N}]+").Where(w => w.Length > 0).ToList();

    private static double Similarity(string a, string b)
    {
        if (Math.Min(a.Length, b.Length) < 2) return 0;
        double sim = 1 - (double)Levenshtein(a, b) / Math.Max(a.Length, b.Length);
        return sim >= 0.6 ? sim : 0;
    }

    internal static int Levenshtein(string a, string b)
    {
        if (a.Length == 0) return b.Length;
        if (b.Length == 0) return a.Length;
        var prev = new int[b.Length + 1];
        var cur = new int[b.Length + 1];
        for (int j = 0; j <= b.Length; j++) prev[j] = j;
        for (int i = 1; i <= a.Length; i++)
        {
            cur[0] = i;
            for (int j = 1; j <= b.Length; j++)
                cur[j] = Math.Min(Math.Min(cur[j - 1] + 1, prev[j] + 1), prev[j - 1] + (a[i - 1] == b[j - 1] ? 0 : 1));
            (prev, cur) = (cur, prev);
        }
        return prev[b.Length];
    }
}

/// <summary>Equation text handling: "\"lhs\" = expression", left-hand sides, references, literals and unit-less numbers.</summary>
internal static class EquationText
{
    private static readonly Regex LhsRx = new(@"^\s*""([^""]+)""\s*=", RegexOptions.CultureInvariant);
    private static readonly Regex QuotedRx = new(@"""([^""]+)""", RegexOptions.CultureInvariant);
    private static readonly Regex LiteralRx = new(@"^\s*([+-]?(?:\d+\.?\d*|\.\d+)(?:[eE][+-]?\d+)?)\s*([A-Za-zµ°'""]*)\s*$", RegexOptions.CultureInvariant);
    private static readonly Regex DocSuffixRx = new(@"\.(part|assembly|sldprt|sldasm)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>The left-hand side of an equation (without quotes), or null.</summary>
    public static string? Lhs(string text)
    {
        var m = LhsRx.Match(text);
        if (m.Success) return m.Groups[1].Value.Trim();
        int eq = text.IndexOf('=');
        return eq <= 0 ? null : text[..eq].Trim().Trim('"').Trim();
    }

    /// <summary>A user left-hand side: quotes stripped, "D1@Sketch1@Part1.Part" shortened to "D1@Sketch1".</summary>
    public static string NormalizeLhs(string? lhs)
    {
        string s = (lhs ?? "").Trim();
        if (s.Length >= 2 && s[0] == '"' && s[^1] == '"') s = s[1..^1].Trim();
        if (s.Length == 0)
            throw SwException.InvalidArgument("'lhs' is required: a global variable name (W) or a dimension name (D1@Sketch1).",
                "For several equations in one call, give 'equations' instead: [{lhs:'W', expression:'60mm'}].");
        if (s.Contains('"') || s.Contains('=') || s.Any(char.IsControl))
            throw SwException.InvalidArgument($"'lhs' must be a bare name without quotes or '=' (got {lhs}).", "E.g. lhs W with expression 60mm.");
        var parts = s.Split('@');
        if (parts.Length >= 3 && DocSuffixRx.IsMatch(parts[^1])) s = string.Join('@', parts[..^1]);
        return s;
    }

    /// <summary>A user right-hand side, checked for the obvious mistakes.</summary>
    public static string Expression(string? expression)
    {
        string e = (expression ?? "").Trim();
        if (e.Length == 0) throw SwException.InvalidArgument("'expression' is required, e.g. 60mm or \"W\" / 2.");
        if (e.Contains('\n') || e.Contains('\r')) throw SwException.InvalidArgument("'expression' must be a single line.");
        if (LhsRx.IsMatch(e))
            throw SwException.InvalidArgument("'expression' is only the right-hand side (e.g. 60mm); the name goes in 'lhs'.");
        if (e.Count(c => c == '"') % 2 != 0)
            throw SwException.InvalidArgument($"'expression' has an unmatched double quote: {e}", "Names go in double quotes (\"W\"); write inches as 'in'.");
        return e;
    }

    public static string Build(string lhs, string expression) => $"\"{lhs}\" = {expression}";

    /// <summary>Checked (lhs, expression) pairs for an equation_set batch: at least one, each as a single call takes it, no left-hand side twice.</summary>
    public static List<(string Lhs, string Expression)> RequireEquations(IReadOnlyList<EquationInput>? equations)
    {
        if (equations is null || equations.Count == 0)
            throw SwException.InvalidArgument("'equations' must contain at least one {lhs, expression}.",
                "E.g. equations:[{lhs:'W', expression:'60mm'}, {lhs:'D1@Sketch1', expression:'\"W\" / 2'}].");
        if (equations.Count > 100) throw SwException.InvalidArgument($"'equations' has {equations.Count} entries (at most 100 per call).");
        var list = new List<(string Lhs, string Expression)>();
        for (int i = 0; i < equations.Count; i++)
        {
            var e = equations[i] ?? throw SwException.InvalidArgument($"equations[{i}] is null.");
            string key, expr;
            try
            {
                key = NormalizeLhs(e.Lhs);
                expr = Expression(e.Expression);
            }
            catch (SwException ex) when (ex.Code == ErrorCodes.InvalidArgument)
            {
                throw SwException.InvalidArgument($"equations[{i}]: {ex.Message}", ex.Hint);
            }
            if (list.Any(x => SameLhs(x.Lhs, key))) throw SwException.InvalidArgument($"\"{key}\" is listed twice in 'equations'.");
            list.Add((key, expr));
        }
        return list;
    }

    /// <summary>
    /// The order to write a batch in: a row after the rows of the batch its expression uses, otherwise as given (indices
    /// into <paramref name="rows"/>). A cycle is refused before this is called; one left over is written as given.
    /// </summary>
    public static List<int> WriteOrder(IReadOnlyList<(string Lhs, string Expression)> rows)
    {
        var refs = rows.Select(r => ExpressionReferences(r.Expression)).ToList();
        var left = Enumerable.Range(0, rows.Count).ToList();
        var order = new List<int>(rows.Count);
        while (left.Count > 0)
        {
            int pick = left.FindIndex(i => !refs[i].Any(r => left.Any(j => j != i && SameLhs(rows[j].Lhs, r))));
            if (pick < 0) pick = 0;
            order.Add(left[pick]);
            left.RemoveAt(pick);
        }
        return order;
    }

    /// <summary>Same left-hand side: letter case and a trailing document qualifier ("@Part1.Part") are ignored.</summary>
    public static bool SameLhs(string a, string b) => string.Equals(Key(a), Key(b), StringComparison.OrdinalIgnoreCase);

    private static string Key(string lhs)
    {
        string s = lhs.Trim().Trim('"').Trim();
        var parts = s.Split('@');
        return parts.Length >= 3 && DocSuffixRx.IsMatch(parts[^1]) ? string.Join('@', parts[..^1]) : s;
    }

    /// <summary>Equal apart from white space and letter case.</summary>
    public static bool SameText(string a, string b) => Squash(a) == Squash(b);

    private static string Squash(string s) => new(s.Where(c => !char.IsWhiteSpace(c)).Select(char.ToLowerInvariant).ToArray());

    /// <summary>The quoted names used on the right-hand side.</summary>
    public static List<string> References(string text)
    {
        var m = LhsRx.Match(text);
        int start = m.Success ? m.Length : Math.Max(0, text.IndexOf('=') + 1);
        return QuotedRx.Matches(text[start..]).Select(x => x.Groups[1].Value.Trim()).ToList();
    }

    /// <summary>
    /// The feature an equation's left-hand side belongs to: "D1@Sketch1" → Sketch1, a suppression equation's "Fillet1" →
    /// Fillet1 (when it is not a global variable); null for a global variable or a name in another document.
    /// </summary>
    public static string? FeatureOf(string lhs, bool global = false)
    {
        string s = lhs.Trim().Trim('"').Trim();
        if (s.Length == 0 || IsExternal(s)) return null;
        int at = s.IndexOf('@');
        return at < 0 ? (global ? null : s) : at + 1 < s.Length ? s[(at + 1)..] : null;
    }

    /// <summary>The dimension names of this document an equation uses, its left-hand side included (D1@Sketch1; no global variables, no external names).</summary>
    public static List<string> DimensionNames(string text)
    {
        var names = new List<string>();
        if (Lhs(text) is { } lhs) names.Add(lhs);
        names.AddRange(ExpressionReferences(text[Math.Max(0, text.IndexOf('=') + 1)..]));
        return names.Where(n => n.IndexOf('@') > 0 && !IsExternal(n)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    /// <summary>Quoted words that are values, not names (feature suppression equations).</summary>
    private static readonly HashSet<string> Keywords = new(StringComparer.OrdinalIgnoreCase) { "suppressed", "unsuppressed" };

    /// <summary>The names a right-hand side uses (quoted, without the suppression keywords).</summary>
    public static List<string> ExpressionReferences(string expression) =>
        QuotedRx.Matches(expression).Select(x => x.Groups[1].Value.Trim()).Where(n => n.Length > 0 && !Keywords.Contains(n)).ToList();

    /// <summary>
    /// A name SOLIDWORKS resolves in another document or component ("D1@Sketch1@bracket-1.Part", "W@Part1&lt;1&gt;.Part"),
    /// or a file property ("SW-Mass@Part1.SLDPRT"): not checked against this document.
    /// </summary>
    public static bool IsExternal(string name)
    {
        if (name.StartsWith("SW-", StringComparison.OrdinalIgnoreCase)) return true;
        var parts = name.Split('@');
        return parts.Length >= 2 && (DocSuffixRx.IsMatch(parts[^1]) || parts[^1].Contains('<'));
    }

    /// <summary>
    /// The chain from <paramref name="lhs"/> through the equations its <paramref name="references"/> use back to itself
    /// (e.g. [W, H, W]), or null when the new equation creates no cycle. <paramref name="others"/> must not contain the
    /// equation being replaced.
    /// </summary>
    public static List<string>? Cycle(string lhs, IEnumerable<string> references, IEnumerable<(string Lhs, string Text)> others)
    {
        var uses = new Dictionary<string, (string Name, List<string> Refs)>(StringComparer.OrdinalIgnoreCase);
        foreach (var (l, t) in others)
            uses.TryAdd(Key(l), (Key(l), References(t).Select(Key).ToList()));
        string start = Key(lhs);
        var path = new List<string> { start };
        var explored = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var r in references.Select(Key))
            if (Walk(r)) return path;
        return null;

        bool Walk(string node)
        {
            if (string.Equals(node, start, StringComparison.OrdinalIgnoreCase))
            {
                path.Add(start);
                return true;
            }
            if (!explored.Add(node) || !uses.TryGetValue(node, out var next)) return false;
            path.Add(next.Name); // the spelling of the equation, not of the reference
            foreach (var n in next.Refs)
                if (Walk(n)) return true;
            path.RemoveAt(path.Count - 1);
            return false;
        }
    }

    /// <summary>Names SOLIDWORKS 2023 refuses as a global variable, in any case: its functions and pi (probed, S58).</summary>
    private static readonly HashSet<string> RefusedNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "sin", "cos", "tan", "sec", "cosec", "cotan", "arcsin", "arccos", "atn", "arcsec", "exp", "log", "sqr", "int", "sgn", "abs", "pi",
    };

    /// <summary>
    /// Why IEquationMgr.Add2 refused an equation, as the rest of the message after its text, and the hint (S58 b). A global
    /// set to a plain number is valid unless its name is a function's; SOLIDWORKS can come to refuse every equation, in a
    /// new part too, until it is restarted, and that is the likelier cause then.
    /// </summary>
    public static (string Why, string Hint) Refused(string lhs, string expression, bool dimension)
    {
        if (!dimension && RefusedNames.Contains(lhs))
            return ($": \"{lhs}\" is the name of a SOLIDWORKS function, which it refuses as a variable name.",
                $"Give the variable another name, e.g. {lhs}1.");
        if (!dimension && Literal(expression) is not null)
            return (", though the equation is valid: a global variable set to a number. SOLIDWORKS can come to refuse every " +
                    "equation, in a new part too, until it is restarted.",
                "Save the open documents, restart SOLIDWORKS and repeat the call; the equation needs no change.");
        return (".", "Check the syntax: names in double quotes, units on numbers (60mm), operators and function names; and that the " +
                     "value is one the dimension can take (a depth of 0 is refused). If a plain \"W\" = 1mm is refused too, SOLIDWORKS " +
                     "refuses every equation until it is restarted.");
    }

    /// <summary>A single literal such as "60mm", "2.5in", "30deg" or "12": its value and lower-case unit ("" for none).</summary>
    public static (double Value, string Unit)? Literal(string expression)
    {
        var m = LiteralRx.Match(expression);
        if (!m.Success || !double.TryParse(m.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double v)) return null;
        return (v, m.Groups[2].Value.ToLowerInvariant());
    }

    /// <summary>SI factor of an equation unit (meters or radians per unit), or null for a unit not known here.</summary>
    public static (double Factor, bool Angle)? UnitFactor(string unit) => unit switch
    {
        "mm" => (1e-3, false),
        "cm" => (1e-2, false),
        "m" => (1, false),
        "um" or "µm" => (1e-6, false),
        "nm" => (1e-9, false),
        "in" or "\"" => (0.0254, false),
        "ft" or "'" => (0.3048, false),
        "mil" => (2.54e-5, false),
        "deg" or "°" => (Math.PI / 180, true),
        "rad" => (1, true),
        _ => null,
    };

    /// <summary>
    /// Numbers written without a unit, outside quoted names, that are not plain factors (operands of *, / or ^):
    /// SOLIDWORKS reads them in the document's units.
    /// </summary>
    public static List<string> BareNumbers(string expression)
    {
        var found = new List<string>();
        string s = expression;
        int i = 0;
        while (i < s.Length)
        {
            char c = s[i];
            if (c == '"')
            {
                int close = s.IndexOf('"', i + 1);
                i = close < 0 ? s.Length : close + 1;
                continue;
            }
            bool number = char.IsDigit(c) || (c == '.' && i + 1 < s.Length && char.IsDigit(s[i + 1]));
            if (!number) { i++; continue; }
            if (i > 0 && (char.IsLetter(s[i - 1]) || s[i - 1] == '_'))
            {
                while (i < s.Length && (char.IsLetterOrDigit(s[i]) || s[i] == '_')) i++; // part of a name
                continue;
            }
            int start = i;
            while (i < s.Length && char.IsDigit(s[i])) i++;
            if (i < s.Length && s[i] == '.') { i++; while (i < s.Length && char.IsDigit(s[i])) i++; }
            if (i + 1 < s.Length && (s[i] is 'e' or 'E') &&
                (char.IsDigit(s[i + 1]) || (s[i + 1] is '+' or '-' && i + 2 < s.Length && char.IsDigit(s[i + 2]))))
            {
                i += 2;
                while (i < s.Length && char.IsDigit(s[i])) i++;
            }
            string token = s[start..i];
            int k = i;
            while (k < s.Length && s[k] == ' ') k++;
            if (k < s.Length && (char.IsLetter(s[k]) || s[k] is '°' or '\'' || (s[k] == '"' && k == i)))
            {
                i = k; // a unit follows
                while (i < s.Length && (char.IsLetter(s[i]) || s[i] is '°' or '\'' or '"')) i++;
                continue;
            }
            int p = start - 1;
            while (p >= 0 && s[p] == ' ') p--;
            int q = i;
            while (q < s.Length && s[q] == ' ') q++;
            char before = p >= 0 ? s[p] : '\0', after = q < s.Length ? s[q] : '\0';
            if (before is '*' or '/' or '^' || after is '*' or '/' or '^') continue; // a factor or divisor
            if (!found.Contains(token)) found.Add(token);
        }
        return found;
    }
}
