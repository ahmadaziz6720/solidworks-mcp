using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SolidWorksMcp.Core.Services;

// Interop-free half of material_add (linked into the offline unit tests): the custom material library this server writes.

/// <summary>A custom material in SI: kg/m³, Pa, W/(m·K), J/(kg·K), 1/K.</summary>
public sealed record MaterialDef(string Name, string Classification, double DensityKgM3, double ElasticModulusPa, double PoissonRatio,
    double ShearModulusPa, double? YieldPa = null, double? TensilePa = null, double? ThermalConductivity = null, double? SpecificHeat = null,
    double? ThermalExpansion = null);

/// <summary>
/// The .sldmat format (SOLIDWORKS' own files: UTF-16 XML, a mstns:materials root holding classification elements with
/// material elements and their physicalproperties: EX, NUXY, GXY, ALPX, DENS, KX, C, SIGXT, SIGYLD, values in SI). Each
/// material this server writes carries the marker in its description, which SOLIDWORKS keeps; a library this server
/// created also carries it as a comment, so its materials from before the per-material marker count as its own. Only
/// those materials are ever replaced: the rest of a library, SOLIDWORKS' own Custom Materials included, keeps its
/// materials and its layout.
/// </summary>
internal static class MaterialXml
{
    public const string Ns = "http://www.solidworks.com/sldmaterials";
    public const string Marker = "written by solidworks-mcp";

    /// <summary>The description of a material this server writes (shown in SOLIDWORKS' material dialog).</summary>
    public const string MaterialMarker = Marker + " (material_add)";

    private static readonly (string Tag, string Display)[] Props =
    [
        ("EX", "Elastic Modulus"), ("NUXY", "Poisson's Ratio"), ("GXY", "Shear Modulus"), ("ALPX", "Thermal Expansion Coefficient"),
        ("DENS", "Density"), ("KX", "Thermal Conductivity"), ("C", "Specific Heat"), ("SIGXT", "Tensile Strength"), ("SIGYLD", "Yield Strength"),
    ];

    public static XDocument NewLibrary() => new(new XDeclaration("1.0", "utf-16", null),
        new XComment($" {Marker}: materials added with material_add "),
        new XElement(XName.Get("materials", Ns), new XAttribute(XNamespace.Xmlns + "mstns", Ns), new XAttribute("version", "2008.03")));

    /// <summary>A library this server created (the comment marker).</summary>
    public static bool IsOurs(XDocument doc) => doc.Nodes().OfType<XComment>().Any(c => c.Value.Contains(Marker, StringComparison.Ordinal));

    /// <summary>A material this server wrote: its description carries the marker, or the whole library is this server's.</summary>
    public static bool IsOurs(XDocument doc, XElement material) =>
        ((string?)material.Attribute("description") ?? "").Contains(Marker, StringComparison.Ordinal) || IsOurs(doc);

    /// <summary>Parses a library with its whitespace, so a write keeps the layout of what was there.</summary>
    public static XDocument Parse(string xml) =>
        XDocument.Load(XmlReader.Create(new StringReader(xml), new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null }),
            LoadOptions.PreserveWhitespace);

    /// <summary>Reads a library file; one that does not parse is <see cref="Unreadable"/>, never an XmlException.</summary>
    public static XDocument Load(string file)
    {
        try { return Parse(File.ReadAllText(file)); }
        catch (XmlException ex) { throw Unreadable(file, ex.Message); }
    }

    /// <summary>Where a library is copied before this server first writes into one it didn't create.</summary>
    public static string BackupPath(string file) => file + ".before-solidworks-mcp";

    /// <summary>The error for a library that does not parse: which file, where, why it may have happened, what to do.</summary>
    public static SwException Unreadable(string file, string error, string? lead = null)
    {
        string backup = BackupPath(file);
        return new SwException(ErrorCodes.IoError,
            $"{lead}The material library '{file}' does not parse: {error.Trim()} SOLIDWORKS can rewrite a library it has loaded (it has " +
            "put its own XML header in front of one), and then neither it nor this server can read the file.",
            (File.Exists(backup) ? $"Move the file aside or restore it from '{backup}'" : "Move the file aside or restore it from a copy") +
            ", then call again (after restarting SOLIDWORKS if it rewrote the file); this server doesn't repair it.");
    }

    /// <summary>The material elements, as text, other than those named <paramref name="except"/>: what a write must leave unchanged.</summary>
    public static List<string> Others(XDocument doc, string except) =>
        [.. (doc.Root?.Descendants("material") ?? [])
            .Where(e => !string.Equals((string?)e.Attribute("name"), except, StringComparison.OrdinalIgnoreCase))
            .Select(e => e.ToString(SaveOptions.DisableFormatting)).Order(StringComparer.Ordinal)];

    /// <summary>
    /// Adds the material under its classification (created when missing). One of that name (in any case) is replaced only
    /// when this server wrote it and <paramref name="overwrite"/> is set; one it didn't write is refused (NOT_OWNED).
    /// </summary>
    public static bool Upsert(XDocument doc, MaterialDef m, bool overwrite, string library = "The library")
    {
        var root = doc.Root ?? throw new SwException(ErrorCodes.Internal, "The material library has no root element.");
        string? unit = IndentUnit(doc);
        var existing = root.Descendants("material").FirstOrDefault(e => string.Equals((string?)e.Attribute("name"), m.Name, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            string had = (string?)existing.Attribute("name") ?? m.Name;
            if (!IsOurs(doc, existing))
                throw new SwException(ErrorCodes.NotOwned, $"{library} already has a material '{had}' that this server didn't write, so it is left alone.",
                    "Pick another name, or another database.");
            if (!overwrite)
                throw SwException.InvalidArgument($"{library} already has a material named '{had}'.", "Pass overwrite:true to replace it, or pick another name.");
            var parent = existing.Parent;
            Remove(existing);
            if (parent is not null && parent != root && !parent.Elements().Any()) Remove(parent);
        }
        var cls = root.Elements("classification").FirstOrDefault(e => string.Equals((string?)e.Attribute("name"), m.Classification, StringComparison.Ordinal));
        if (cls is null)
        {
            cls = new XElement("classification", new XAttribute("name", m.Classification));
            Append(root, cls, unit);
        }
        Append(cls, Element(m), unit);
        return existing is not null;
    }

    /// <summary>The indent of a library read with its whitespace (SOLIDWORKS' own: a tab), or null for one built here.</summary>
    private static string? IndentUnit(XDocument doc)
    {
        var ws = doc.Root?.Nodes().OfType<XText>().FirstOrDefault(t => t.Value.Contains('\n') && string.IsNullOrWhiteSpace(t.Value));
        if (ws is null) return null;
        string u = ws.Value[(ws.Value.LastIndexOf('\n') + 1)..];
        return u.Length > 0 ? u : null;
    }

    private static string Line(string unit, int depth) => "\n" + string.Concat(Enumerable.Repeat(unit, depth));

    /// <summary>Adds <paramref name="child"/> last in <paramref name="parent"/>, laid out as the rest of the file when it has a layout.</summary>
    private static void Append(XElement parent, XElement child, string? unit)
    {
        if (unit is null) { parent.Add(child); return; }
        int depth = parent.Ancestors().Count();
        Layout(child, depth + 1, unit);
        if (parent.LastNode is XText closing && string.IsNullOrWhiteSpace(closing.Value))
            closing.AddBeforeSelf(new XText(Line(unit, depth + 1)), child);
        else
            parent.Add(new XText(Line(unit, depth + 1)), child, new XText(Line(unit, depth)));
    }

    private static void Layout(XElement e, int depth, string unit)
    {
        var kids = e.Elements().ToList();
        if (kids.Count == 0) return;
        foreach (var k in kids)
        {
            k.AddBeforeSelf(new XText(Line(unit, depth + 1)));
            Layout(k, depth + 1, unit);
        }
        e.Add(new XText(Line(unit, depth)));
    }

    /// <summary>Removes an element with the whitespace that put it on its own line.</summary>
    private static void Remove(XElement e)
    {
        if (e.PreviousNode is XText ws && string.IsNullOrWhiteSpace(ws.Value)) ws.Remove();
        e.Remove();
    }

    private static XElement Element(MaterialDef m)
    {
        double?[] values = [m.ElasticModulusPa, m.PoissonRatio, m.ShearModulusPa, m.ThermalExpansion, m.DensityKgM3, m.ThermalConductivity,
            m.SpecificHeat, m.TensilePa, m.YieldPa];
        var phys = new XElement("physicalproperties");
        for (int i = 0; i < Props.Length; i++)
            if (values[i] is double v)
                phys.Add(new XElement(Props[i].Tag, new XAttribute("displayname", Props[i].Display), new XAttribute("value", v.ToString("R", CultureInfo.InvariantCulture))));
        return new XElement("material", new XAttribute("name", m.Name), new XAttribute("description", MaterialMarker), phys);
    }

    /// <summary>The physical properties of the material <paramref name="name"/> as written (tag → SI value), or null.</summary>
    public static IReadOnlyDictionary<string, double>? Read(XDocument doc, string name)
    {
        var mat = doc.Root?.Descendants("material").FirstOrDefault(e => string.Equals((string?)e.Attribute("name"), name, StringComparison.Ordinal));
        if (mat is null) return null;
        var map = new Dictionary<string, double>(StringComparer.Ordinal);
        foreach (var p in mat.Element("physicalproperties")?.Elements() ?? [])
            if (double.TryParse((string?)p.Attribute("value"), NumberStyles.Float, CultureInfo.InvariantCulture, out double v)) map[p.Name.LocalName] = v;
        return map;
    }

    /// <summary>The tags and SI values a material should read back with.</summary>
    public static IReadOnlyDictionary<string, double> Expected(MaterialDef m)
    {
        double?[] values = [m.ElasticModulusPa, m.PoissonRatio, m.ShearModulusPa, m.ThermalExpansion, m.DensityKgM3, m.ThermalConductivity,
            m.SpecificHeat, m.TensilePa, m.YieldPa];
        var map = new Dictionary<string, double>(StringComparer.Ordinal);
        for (int i = 0; i < Props.Length; i++)
            if (values[i] is double v) map[Props[i].Tag] = v;
        return map;
    }

    /// <summary>The library as SOLIDWORKS writes its own: UTF-16 with a byte-order mark.</summary>
    public static byte[] Bytes(XDocument doc)
    {
        using var ms = new MemoryStream();
        using (var w = XmlWriter.Create(ms, new XmlWriterSettings { Encoding = new UnicodeEncoding(false, true), Indent = true }))
            doc.Save(w);
        return ms.ToArray();
    }

    /// <summary>
    /// Checks a material before it is written: a name and classification SOLIDWORKS can show, and properties in physical
    /// ranges (ν between -1 and 0.5, positive moduli and density). The shear modulus defaults to E / (2(1+ν)).
    /// </summary>
    public static MaterialDef Check(string? name, string? classification, double? densityKgM3, double? elasticModulusGpa, double? poissonRatio,
        double? shearModulusGpa, double? yieldMpa, double? tensileMpa, double? conductivity, double? specificHeat, double? expansion)
    {
        static double Need(double? v, string arg, double min, double max)
        {
            if (v is not double x) throw SwException.InvalidArgument($"'{arg}' is required.");
            if (!double.IsFinite(x) || x <= min || x > max)
                throw SwException.InvalidArgument($"{arg} must be greater than {min.ToString(CultureInfo.InvariantCulture)} and at most {max.ToString(CultureInfo.InvariantCulture)} (got {x.ToString(CultureInfo.InvariantCulture)}).");
            return x;
        }
        static double? Opt(double? v, string arg, double min, double max) => v is null ? null : Need(v, arg, min, max);
        static string Text(string? s, string arg, string? fallback)
        {
            string t = (s ?? fallback ?? "").Trim();
            if (t.Length == 0) throw SwException.InvalidArgument($"'{arg}' is required.");
            if (t.Length > 80 || t.Any(char.IsControl) || t.IndexOfAny(['<', '>', '&', '"']) >= 0)
                throw SwException.InvalidArgument($"'{arg}' must be at most 80 plain characters (got '{t}').");
            return t;
        }
        double nu = poissonRatio is double p && double.IsFinite(p) && p > -1 && p < 0.5
            ? p
            : throw SwException.InvalidArgument($"poissonRatio must be between -1 and 0.5 (got {poissonRatio?.ToString(CultureInfo.InvariantCulture) ?? "nothing"}).");
        double e = Need(elasticModulusGpa, "elasticModulusGpa", 0, 2000) * 1e9;
        double g = Opt(shearModulusGpa, "shearModulusGpa", 0, 2000) is double gg ? gg * 1e9 : e / (2 * (1 + nu));
        double? ys = Opt(yieldMpa, "yieldMpa", 0, 100_000) * 1e6, ts = Opt(tensileMpa, "tensileMpa", 0, 100_000) * 1e6;
        if (ys is double y0 && ts is double t0 && y0 > t0) throw SwException.InvalidArgument($"yieldMpa ({yieldMpa}) is above tensileMpa ({tensileMpa}).");
        return new MaterialDef(Text(name, "name", null), Text(classification, "classification", "solidworks-mcp"),
            Need(densityKgM3, "densityKgM3", 0, 30_000), e, nu, g, ys, ts,
            Opt(conductivity, "thermalConductivityWmK", 0, 10_000), Opt(specificHeat, "specificHeatJkgK", 0, 100_000), Opt(expansion, "thermalExpansionPerK", -1e-3, 1e-3));
    }
}
