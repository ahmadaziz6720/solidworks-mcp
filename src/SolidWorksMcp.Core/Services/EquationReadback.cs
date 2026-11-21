using System.Globalization;
using System.Text.RegularExpressions;

namespace SolidWorksMcp.Core.Services;

// Interop-free half of the equation tools' value checks (linked into the offline unit tests).

/// <summary>A global variable set to a plain number that SOLIDWORKS reads back as another value.</summary>
public sealed record GlobalMisread(string Lhs, string Text, double Read);

/// <summary>
/// What a value SOLIDWORKS reads for an equation is checked against (equation_set's tier B), and the sign of the state in
/// which SOLIDWORKS refuses every equation until it is restarted (S58 b, S92): in it, a global set to a plain number reads
/// back as another value (60.4mm as 60, 0.5mm as 0) and every row defined from others as 0, before any write. A restart
/// clears it. equation_list and equation_set read the sign from the document's own globals and name the restart.
/// </summary>
internal static class EquationReadback
{
    private static readonly Regex LiteralRx = new(@"^\s*([+-]?(?:\d+\.?\d*|\.\d+)(?:[eE][+-]?\d+)?)\s*([A-Za-zµ°'""]*)\s*$", RegexOptions.CultureInvariant);
    private static readonly Regex RhsRx = new(@"^\s*""[^""]+""\s*=(.*)$", RegexOptions.CultureInvariant);

    public const string RestartHint = "Save the open documents, restart SOLIDWORKS and repeat the call; the equation needs no change.";

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

    /// <summary>The right-hand side of a stored equation ("g" = 60.4mm gives 60.4mm), or null.</summary>
    public static string? Rhs(string text)
    {
        var m = RhsRx.Match(text);
        return m.Success ? m.Groups[1].Value.Trim() : null;
    }

    /// <summary>
    /// A value SOLIDWORKS reports agrees with an SI quantity, read either in SI or in document units (which one
    /// IEquationMgr.Value uses is not pinned down, so both are accepted; the check still catches wrong values).
    /// </summary>
    public static bool Agrees(double reported, double si, double docFactor) =>
        Close(reported, si) || (docFactor > 0 && Close(reported, si / docFactor));

    /// <summary>A value read agrees with a literal; a unit not known here is not judged (true).</summary>
    public static bool LiteralAgrees(double reported, (double Value, string Unit) lit, double lin, double ang)
    {
        if (lit.Unit.Length == 0) return Close(reported, lit.Value) || Close(reported, lit.Value * lin) || Close(reported, lit.Value * ang);
        if (UnitFactor(lit.Unit) is not { } u) return true;
        return Agrees(reported, lit.Value * u.Factor, u.Angle ? ang : lin);
    }

    public static bool Close(double a, double b) => Math.Abs(a - b) <= 1e-6 * Math.Max(1, Math.Abs(b));

    /// <summary>
    /// Every global variable defined by a plain number whose value read back is another (rows as equation_list reads them;
    /// lin and ang are the document's meters and radians per unit). A row with no value, or not a plain number, or in a
    /// unit not known here, is not judged.
    /// </summary>
    public static List<GlobalMisread> Misreads(IEnumerable<(string Lhs, string Text, bool Global, double? Value)> rows, double lin, double ang)
    {
        var found = new List<GlobalMisread>();
        foreach (var r in rows)
        {
            if (!r.Global || r.Value is not double read || Rhs(r.Text) is not { } rhs || Literal(rhs) is not { } lit) continue;
            if (lit.Unit.Length > 0 && UnitFactor(lit.Unit) is null) continue;
            if (!LiteralAgrees(read, lit, lin, ang) && !found.Any(f => string.Equals(f.Lhs, r.Lhs, StringComparison.OrdinalIgnoreCase)))
                found.Add(new GlobalMisread(r.Lhs, r.Text, read));
        }
        return found;
    }

    /// <summary>The sign, as a sentence without its full stop: which globals read as what, and the state it shows.</summary>
    public static string Sign(IReadOnlyList<GlobalMisread> misreads) =>
        "SOLIDWORKS reads " + string.Join(", ", misreads.Take(4).Select(m => $"{m.Text.Trim()} as {F(m.Read)}")) +
        (misreads.Count > 4 ? $" and {misreads.Count - 4} more" : "") +
        ": a global set to a number read back as another, the sign of the state in which SOLIDWORKS refuses or misreads " +
        "every equation, in any document, until it is restarted";

    /// <summary>equation_list's warning: its values are not to be trusted.</summary>
    public static string ListWarning(IReadOnlyList<GlobalMisread> misreads) =>
        Sign(misreads) + ". The values listed are not to be trusted (rows defined from others read wrong too). Save the open " +
        "documents and restart SOLIDWORKS before any equation_set, then read again.";

    /// <summary>equation_set's warning after a call whose own checks passed.</summary>
    public static string SetWarning(IReadOnlyList<GlobalMisread> misreads) =>
        Sign(misreads) + ". Values read in this state are not to be trusted; save the open documents and restart SOLIDWORKS.";

    /// <summary>
    /// equation_set's VERIFY_FAILED message and hint: <paramref name="what"/> is what failed, <paramref name="restored"/> the
    /// rollback's sentence. While the sign shows, the state and the restart are named in place of the expression.
    /// </summary>
    public static (string Message, string Hint) VerifyFailed(string what, string restored, IReadOnlyList<GlobalMisread> misreads) =>
        misreads.Count == 0
            ? ($"{what}. {restored}", "Check the expression (units, quoted names, existing variables) and the dimension name.")
            : ($"{what}. {Sign(misreads)}; the expression is likely not at fault. {restored}", RestartHint);

    /// <summary>An equation changed in place that SOLIDWORKS dropped, keeping the old text; the guess at the value only without the sign.</summary>
    public static string KeptPrevious(string previous, string expression, bool stateShown) =>
        $"SOLIDWORKS kept the previous equation {previous} and did not take {expression}" +
        (stateShown ? "" : " (a value the dimension can't have, such as 0?)");

    private static string F(double v) => v.ToString("0.######", CultureInfo.InvariantCulture);
}
