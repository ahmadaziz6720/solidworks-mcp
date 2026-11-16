using SolidWorksMcp.Core.Services;
using Xunit;

namespace SolidWorksMcp.UnitTests;

/// <summary>material_set's density check (S72): what the mass properties read against the database's density.</summary>
public class MaterialDensityTests
{
    [Fact]
    public void A_density_within_half_a_percent_matches()
    {
        var r = MaterialDensity.Judge("AISI 1020", 7930, 7900, bodiesWithOwnMaterial: false, massOverridden: false);
        Assert.Equal(DensityVerdict.Matches, r.Verdict);
        Assert.Equal("density 7930 kg/m³ matches the database", r.Text);
        Assert.Null(r.Hint);
    }

    [Fact]
    public void A_database_material_at_1_kg_m3_is_refused_naming_a_restart()
    {
        // The scenario's process: "AISI 1020" named and read back, its mass at 1 kg/m³, no body of its own, no override.
        var r = MaterialDensity.Judge("AISI 1020", 1, 7900, false, false);
        Assert.Equal(DensityVerdict.Refuse, r.Verdict);
        Assert.Contains("1 kg/m³", r.Text);
        Assert.Contains("7900 kg/m³", r.Text);
        Assert.Contains("until it is restarted", r.Text);
        Assert.Contains("restart SOLIDWORKS", r.Hint);
    }

    [Fact]
    public void Another_density_more_than_1_percent_off_is_refused_without_naming_a_restart()
    {
        // S30's state: the name read back, the mass at the document's default density.
        var r = MaterialDensity.Judge("AISI 1020", 1000, 7900, false, false);
        Assert.Equal(DensityVerdict.Refuse, r.Verdict);
        Assert.Contains("1000 kg/m³", r.Text);
        Assert.DoesNotContain("restart", r.Text + r.Hint);
        Assert.NotNull(r.Hint);
    }

    [Theory]
    [InlineData(true, false, "own material")]
    [InlineData(false, true, "overridden")]
    [InlineData(true, true, "own material")]
    public void A_body_with_its_own_material_or_an_overridden_mass_is_a_warning_only(bool own, bool overridden, string reason)
    {
        var r = MaterialDensity.Judge("AISI 1020", 1, 7900, own, overridden);
        Assert.Equal(DensityVerdict.Warn, r.Verdict);
        Assert.Contains(reason, r.Text);
        Assert.DoesNotContain("restart", r.Text);
    }

    [Fact]
    public void Between_half_a_percent_and_1_percent_is_a_warning_and_past_1_percent_a_refusal()
    {
        Assert.Equal(DensityVerdict.Warn, MaterialDensity.Judge("AISI 1020", 7960, 7900, false, false).Verdict);  // 0.76 %
        Assert.Equal(DensityVerdict.Refuse, MaterialDensity.Judge("AISI 1020", 7990, 7900, false, false).Verdict); // 1.14 %
        Assert.Equal(DensityVerdict.Refuse, MaterialDensity.Judge("AISI 1020", 7810, 7900, false, false).Verdict);
    }
}
