using System.Globalization;

namespace SolidWorksMcp.Core.Services;

// Interop-free half of material_set's density check (linked into the offline unit tests).

public enum DensityVerdict { Matches, Warn, Refuse }

/// <summary>The verdict, its text (a check, a warning or the error's message) and, for a refusal, the hint.</summary>
public sealed record DensityCheck(DensityVerdict Verdict, string Text, string? Hint = null);

/// <summary>
/// What material_set makes of the density its mass properties read against the database's (S72). Within 0.5 % it
/// matches. Off by more than 1 %, with no body of another material and the mass not overridden, the material is not
/// applied as SOLIDWORKS says it is, and the call is refused (and rolled back by the caller): S30's part reads the
/// default 1000 kg/m³, and a SOLIDWORKS process in a bad state applies every database material at 1 kg/m³ until it is
/// restarted (the equations it refuses then are S58 b). Anything else off is a warning.
/// </summary>
internal static class MaterialDensity
{
    public const double MatchTolerance = 0.005;
    public const double RefuseTolerance = 0.01;

    public static DensityCheck Judge(string material, double measuredKgM3, double databaseKgM3, bool bodiesWithOwnMaterial, bool massOverridden)
    {
        double off = Math.Abs(measuredKgM3 - databaseKgM3);
        if (off <= MatchTolerance * databaseKgM3)
            return new(DensityVerdict.Matches, $"density {F(measuredKgM3)} kg/m³ matches the database");
        string read = $"the mass properties give {F(measuredKgM3)} kg/m³, the database {F(databaseKgM3)} kg/m³";
        if (bodiesWithOwnMaterial || massOverridden || off <= RefuseTolerance * databaseKgM3)
        {
            string why = bodiesWithOwnMaterial ? "bodies with their own material" + (massOverridden ? ", and the mass is overridden" : "")
                : massOverridden ? "the mass is overridden" : "within 1 %";
            return new(DensityVerdict.Warn, $"For '{material}' {read} ({why}).");
        }
        string lead = $"SOLIDWORKS names the material '{material}', but {read}, with no body of another material and the mass not " +
                      "overridden: the material is not applied at its density.";
        if (measuredKgM3 is >= 0.5 and <= 1.5)
            return new(DensityVerdict.Refuse,
                lead + " SOLIDWORKS can come to apply every database material at 1 kg/m³, in a new part too, until it is restarted.",
                "Save the open documents, restart SOLIDWORKS and repeat the call.");
        return new(DensityVerdict.Refuse, lead,
            "Check the part's mass properties in SOLIDWORKS against material_list's density for the material.");
    }

    private static string F(double v) => v.ToString("0.######", CultureInfo.InvariantCulture);
}
