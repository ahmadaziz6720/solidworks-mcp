using System.Runtime.InteropServices;
using SolidWorks.Interop.swconst;
using SolidWorksMcp.Core.Com;
using SolidWorksMcp.Core.Model;

namespace SolidWorksMcp.Core.Services;

/// <summary>Input of material_add: a custom material in a custom library (units as the tool takes them).</summary>
public sealed record MaterialAddRequest
{
    public string? Name { get; init; }
    public string? Classification { get; init; }
    /// <summary>The library (a .sldmat file name); default "Custom Materials", SOLIDWORKS' own custom library.</summary>
    public string? Database { get; init; }
    public double? DensityKgM3 { get; init; }
    public double? ElasticModulusGpa { get; init; }
    public double? PoissonRatio { get; init; }
    public double? ShearModulusGpa { get; init; }
    public double? YieldMpa { get; init; }
    public double? TensileMpa { get; init; }
    public double? ThermalConductivityWmK { get; init; }
    public double? SpecificHeatJkgK { get; init; }
    public double? ThermalExpansionPerK { get; init; }
    public bool Overwrite { get; init; }
}

public sealed class MaterialAddResult : ToolResult
{
    public required string Material { get; init; }
    public required string Classification { get; init; }
    /// <summary>The database name for material_set.</summary>
    public required string Database { get; init; }
    public required string Path { get; init; }
    public bool CreatedDatabase { get; init; }
    public bool Replaced { get; init; }
    /// <summary>The copy of the library as it was before this server first wrote into it (a library it didn't create).</summary>
    public string? Backup { get; init; }
    /// <summary>The physical properties as written (SOLIDWORKS' tags, SI: EX and GXY Pa, DENS kg/m³, SIGYLD Pa, …).</summary>
    public required IReadOnlyDictionary<string, double> Properties { get; init; }
    public required IReadOnlyList<string> Checks { get; init; }
}

public sealed partial class PropertiesService
{
    /// <summary>SOLIDWORKS' own custom library (made at install in the custom material folder), material_add's default.</summary>
    private const string CustomDatabase = "Custom Materials";

    /// <summary>
    /// material_add: the material is written into a library in SOLIDWORKS' first custom material folder (Tools &gt; Options &gt;
    /// File Locations &gt; Material Databases, outside the installation), by default SOLIDWORKS' own "Custom Materials". A
    /// material this server didn't write is never replaced or removed, and a library it didn't create is copied aside before
    /// its first write. Tier B: the file reads back with every value and with its other materials unchanged, and the
    /// material catalog lists it.
    /// </summary>
    public Task<MaterialAddResult> MaterialAddAsync(MaterialAddRequest q, CancellationToken ct = default)
    {
        var m = MaterialXml.Check(q.Name, q.Classification, q.DensityKgM3, q.ElasticModulusGpa, q.PoissonRatio, q.ShearModulusGpa, q.YieldMpa,
            q.TensileMpa, q.ThermalConductivityWmK, q.SpecificHeatJkgK, q.ThermalExpansionPerK);
        string db = (q.Database ?? CustomDatabase).Trim();
        if (db.Length == 0 || db.Length > 60 || db.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0 || MaterialCatalog.IsDefault(db))
            throw SwException.InvalidArgument($"'database' must be a plain custom library name, not '{db}'.", $"Leave it out for '{CustomDatabase}'.");
        return session.RunAsync("material_add", ctx =>
        {
            string folder = CustomMaterialFolder(ctx);
            string file = System.IO.Path.Combine(folder, db + ".sldmat");
            bool created = !File.Exists(file);
            var doc = created ? MaterialXml.NewLibrary() : MaterialXml.Load(file);
            var others = MaterialXml.Others(doc, m.Name);
            bool replaced = MaterialXml.Upsert(doc, m, q.Overwrite, $"'{db}'");

            // A library this server didn't create (SOLIDWORKS' own, or the user's) is copied once, before the first write.
            string? backup = created || MaterialXml.IsOurs(doc) ? null : MaterialXml.BackupPath(file);
            string tmp = file + ".tmp";
            try
            {
                if (backup is not null && !File.Exists(backup)) File.Copy(file, backup);
                File.WriteAllBytes(tmp, MaterialXml.Bytes(doc));
                File.Move(tmp, file, overwrite: true);
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                try { File.Delete(tmp); } catch (Exception e2) when (e2 is IOException or UnauthorizedAccessException) { /* best effort */ }
                throw new SwException(ErrorCodes.IoError, $"Could not write '{file}': {e.Message}", "Check that the custom material folder can be written.");
            }

            var checks = new List<string>();
            var written = MaterialXml.Load(file);
            var back = MaterialXml.Read(written, m.Name);
            var want = MaterialXml.Expected(m);
            if (back is null || want.Any(kv => !back.TryGetValue(kv.Key, out double v) || Math.Abs(v - kv.Value) > 1e-12 * Math.Abs(kv.Value)))
                throw new SwException(ErrorCodes.VerifyFailed, $"'{file}' doesn't read back with '{m.Name}' as written.");
            checks.Add($"'{m.Name}' reads back from {System.IO.Path.GetFileName(file)} with all {want.Count} properties");
            if (!MaterialXml.Others(written, m.Name).SequenceEqual(others, StringComparer.Ordinal))
                throw new SwException(ErrorCodes.VerifyFailed, $"'{file}' doesn't read back with its other materials as they were.",
                    backup is not null ? $"The library as it was is in '{backup}'." : null);
            if (others.Count > 0) checks.Add($"the library's other {others.Count} materials read back unchanged");
            var entry = LoadCatalog(ctx).Materials.FirstOrDefault(e => e.Name == m.Name && MaterialCatalog.SameDatabase(e.Database, db));
            if (entry is null) throw new SwException(ErrorCodes.VerifyFailed, $"The material list doesn't show '{m.Name}' in '{db}'.");
            checks.Add($"material_list lists it in '{db}' ({entry.DensityKgM3} kg/m³)");
            return new MaterialAddResult
            {
                Material = m.Name, Classification = m.Classification, Database = db, Path = file, CreatedDatabase = created, Replaced = replaced,
                Backup = backup, Properties = back, Checks = checks,
            };
        }, null, ct);
    }

    /// <summary>The first configured material folder outside the installation that exists (SOLIDWORKS reads custom libraries there).</summary>
    private static string CustomMaterialFolder(SwContext ctx)
    {
        string folders = ctx.App.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swFileLocationsMaterialDatabases) ?? "";
        string? install = StandardMaterialFolder(ctx.App);
        foreach (var raw in folders.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            string f = Environment.ExpandEnvironmentVariables(raw.Trim('"'));
            if (f.Length == 0 || f.StartsWith(@"\\", StringComparison.Ordinal) || !Directory.Exists(f)) continue;
            if (install is not null && System.IO.Path.GetFullPath(f).TrimEnd('\\').Equals(System.IO.Path.GetFullPath(install).TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
                continue;
            return f;
        }
        throw new SwException(ErrorCodes.NotSupported, "SOLIDWORKS has no custom material folder (Tools > Options > File Locations > Material Databases).",
            "Ask the user to add one there; this server doesn't change SOLIDWORKS' file locations.");
    }
}
