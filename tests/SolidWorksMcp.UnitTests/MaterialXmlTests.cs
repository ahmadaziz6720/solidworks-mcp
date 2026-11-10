using System.Text;
using SolidWorksMcp.Core;
using SolidWorksMcp.Core.Services;
using Xunit;

namespace SolidWorksMcp.UnitTests;

/// <summary>material_add's library (MaterialXml): SOLIDWORKS' .sldmat layout in SI, marked as this server's, checked inputs.</summary>
public class MaterialXmlTests
{
    private static MaterialDef Steel(string name = "Test Steel") =>
        MaterialXml.Check(name, "Steel", 7850, 210, 0.3, null, 355, 510, 50, 460, 1.2e-5);

    [Fact]
    public void A_material_is_written_in_si_and_reads_back_with_the_shear_modulus_from_e_and_nu()
    {
        var m = Steel();
        Assert.Equal(210e9 / (2 * 1.3), m.ShearModulusPa, 1e-3);
        var doc = MaterialXml.NewLibrary();
        Assert.True(MaterialXml.IsOurs(doc));
        Assert.False(MaterialXml.Upsert(doc, m, overwrite: false));
        // Through bytes and back, as the file will be.
        var again = MaterialXml.Parse(Encoding.Unicode.GetString(MaterialXml.Bytes(doc)).TrimStart('﻿'));
        var props = MaterialXml.Read(again, "Test Steel")!;
        Assert.Equal(MaterialXml.Expected(m), props);
        Assert.Equal(210e9, props["EX"]);
        Assert.Equal(7850, props["DENS"]);
        Assert.Equal(355e6, props["SIGYLD"]);
        Assert.Contains("<classification name=\"Steel\">", again.ToString());
        Assert.StartsWith("<?xml version=\"1.0\" encoding=\"utf-16\"?>", Encoding.Unicode.GetString(MaterialXml.Bytes(doc)).TrimStart('﻿'));
    }

    [Fact]
    public void A_name_is_replaced_only_with_overwrite_and_an_emptied_classification_goes()
    {
        var doc = MaterialXml.NewLibrary();
        MaterialXml.Upsert(doc, Steel(), false);
        Assert.Equal(ErrorCodes.InvalidArgument, Assert.Throws<SwException>(() => MaterialXml.Upsert(doc, Steel(), false)).Code);
        var softer = MaterialXml.Check("Test Steel", "Plastics", 1200, 2.5, 0.38, null, null, null, null, null, null);
        Assert.True(MaterialXml.Upsert(doc, softer, overwrite: true));
        Assert.Equal(2.5e9, MaterialXml.Read(doc, "Test Steel")!["EX"]);
        Assert.Single(doc.Root!.Elements("classification")); // the empty Steel classification was removed
        Assert.False(MaterialXml.Read(doc, "Test Steel")!.ContainsKey("SIGYLD"));
    }

    [Theory]
    [InlineData(null, 7850.0, 210.0, 0.3)]      // no name
    [InlineData("A<b", 7850.0, 210.0, 0.3)]     // XML markup in the name
    [InlineData("Steel", 0.0, 210.0, 0.3)]      // no density
    [InlineData("Steel", 7850.0, -1.0, 0.3)]    // negative modulus
    [InlineData("Steel", 7850.0, 210.0, 0.5)]   // incompressible
    public void Bad_materials_are_refused(string? name, double density, double e, double nu) =>
        Assert.Equal(ErrorCodes.InvalidArgument,
            Assert.Throws<SwException>(() => MaterialXml.Check(name, null, density, e, nu, null, null, null, null, null, null)).Code);

    [Fact]
    public void Yield_above_tensile_and_libraries_without_the_marker_are_told_apart()
    {
        Assert.Equal(ErrorCodes.InvalidArgument, Assert.Throws<SwException>(() => MaterialXml.Check("x", null, 1, 1, 0.3, null, 500, 400, null, null, null)).Code);
        Assert.False(MaterialXml.IsOurs(MaterialXml.Parse("<?xml version=\"1.0\"?><materials><classification name=\"Steel\"/></materials>")));
    }

    // SOLIDWORKS' own Custom Materials.sldmat as installed (2023): tab-indented, CRLF, one material without the marker.
    private const string SwCustom =
        "<?xml version=\"1.0\" encoding=\"UTF-16\" standalone=\"no\"?>\r\n" +
        "<mstns:materials xmlns:mstns=\"http://www.solidworks.com/sldmaterials\" version=\"2008.03\">\r\n" +
        "\t<curves id=\"curve0\">\r\n\t\t<point x=\"1.0\" y=\"1.0\"/>\r\n\t</curves>\r\n" +
        "\t<classification name=\"Plastic\">\r\n" +
        "\t\t<material name=\"Custom Plastic\" description=\"-\">\r\n" +
        "\t\t\t<physicalproperties>\r\n\t\t\t\t<DENS displayname=\"Density\" value=\"0.102E+04\"/>\r\n\t\t\t</physicalproperties>\r\n" +
        "\t\t</material>\r\n\t</classification>\r\n</mstns:materials>\r\n";

    [Fact]
    public void A_material_goes_into_solidworks_own_library_in_its_layout_and_its_own_materials_stay()
    {
        var doc = MaterialXml.Parse(SwCustom);
        Assert.False(MaterialXml.IsOurs(doc));
        var before = MaterialXml.Others(doc, "Test Steel");
        Assert.False(MaterialXml.Upsert(doc, Steel(), false, "'Custom Materials'"));
        string text = Encoding.Unicode.GetString(MaterialXml.Bytes(doc)).TrimStart('﻿');
        var again = MaterialXml.Parse(text);
        Assert.Equal(before, MaterialXml.Others(again, "Test Steel"));
        Assert.Equal(210e9, MaterialXml.Read(again, "Test Steel")!["EX"]);
        // The new classification and material are laid out as SOLIDWORKS lays out its own, and nothing else moved.
        Assert.Contains("\r\n\t<classification name=\"Steel\">\r\n\t\t<material name=\"Test Steel\" description=\"written by solidworks-mcp (material_add)\">" +
            "\r\n\t\t\t<physicalproperties>\r\n\t\t\t\t<EX ", text);
        Assert.Contains("\t\t</material>\r\n\t</classification>\r\n\t<classification name=\"Steel\">", text);
        Assert.EndsWith("\t\t</material>\r\n\t</classification>\r\n</mstns:materials>", text.TrimEnd());
        Assert.Contains("\t\t<material name=\"Custom Plastic\" description=\"-\">\r\n\t\t\t<physicalproperties>", text);
        Assert.StartsWith("<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"no\"?>", text);

        // Its own material is replaced only with overwrite; SOLIDWORKS' is never replaced, whatever the case of the name.
        Assert.Equal(ErrorCodes.InvalidArgument, Assert.Throws<SwException>(() => MaterialXml.Upsert(again, Steel(), false)).Code);
        Assert.True(MaterialXml.Upsert(again, Steel(), true));
        Assert.Equal(before, MaterialXml.Others(again, "Test Steel"));
        var mine = MaterialXml.Check("custom plastic", "Plastic", 1000, 2, 0.4, null, null, null, null, null, null);
        var refused = Assert.Throws<SwException>(() => MaterialXml.Upsert(again, mine, true, "'Custom Materials'"));
        Assert.Equal(ErrorCodes.NotOwned, refused.Code);
        Assert.Contains("'Custom Plastic'", refused.Message);
        Assert.Equal(before, MaterialXml.Others(again, "Test Steel"));
    }

    [Fact]
    public void A_library_this_server_created_owns_its_older_materials_without_the_marker()
    {
        var doc = MaterialXml.NewLibrary();
        doc.Root!.Add(new System.Xml.Linq.XElement("classification", new System.Xml.Linq.XAttribute("name", "Plastics"),
            new System.Xml.Linq.XElement("material", new System.Xml.Linq.XAttribute("name", "PETG"))));
        var petg = MaterialXml.Check("PETG", "Plastics", 1270, 2, 0.38, null, null, null, null, null, null);
        Assert.True(MaterialXml.Upsert(doc, petg, true));
    }

    [Fact]
    public void A_library_that_does_not_parse_is_a_named_io_error()
    {
        string file = Path.Combine(Path.GetTempPath(), $"swmcp_broken_{Guid.NewGuid():N}.sldmat");
        // As SOLIDWORKS left one: its own header in front of the library, a second declaration on line 3.
        File.WriteAllText(file, "<?xml version=\"1.0\"?>\r\n<mstns:materials xmlns:mstns=\"http://www.solidworks.com/sldmaterials\">\r\n" +
            Encoding.Unicode.GetString(MaterialXml.Bytes(MaterialXml.NewLibrary())).TrimStart('﻿'), Encoding.Unicode);
        try
        {
            var e = Assert.Throws<SwException>(() => MaterialXml.Load(file));
            Assert.Equal(ErrorCodes.IoError, e.Code);
            Assert.Contains(file, e.Message);
            Assert.Contains("does not parse", e.Message);
            Assert.Contains("Line 3", e.Message);
            Assert.Contains("restore it from a copy", e.Hint);
            File.WriteAllText(MaterialXml.BackupPath(file), "");
            Assert.Contains(MaterialXml.BackupPath(file), MaterialXml.Unreadable(file, "x").Hint);
        }
        finally
        {
            File.Delete(file);
            File.Delete(MaterialXml.BackupPath(file));
        }
    }
}
