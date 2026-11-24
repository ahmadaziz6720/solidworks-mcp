using System.Globalization;

namespace SolidWorksMcp.Core.Services;

// Interop-free half of the check that a dimension did not turn a sketch inside out (linked into the offline unit tests).

/// <summary>A sketch line by name, its length (m) before and after a change.</summary>
internal sealed record LineLength(string Name, double Before, double After);

internal static class SketchShrink
{
    private const int ListMax = 3; // lines named in one warning

    /// <summary>A line this short before (m) is not measured: its half is the solver settling.</summary>
    private const double MinM = 1e-6;

    /// <summary>
    /// S82 (a): the warning for a dimension after which a line is less than half as long as before, or null. A value that
    /// reads back right can hold on a shape turned inside out: an angle measured across the vertex turns a line the other
    /// way, and the lines it ends collapse (a trapezoid's top 133.5 long went to 6.5). A length dimension explains a
    /// change up to twice its own (<paramref name="explainedM"/>, m; 0 for an angle). A warning, not a refusal (as S42):
    /// it can be what was meant.
    /// </summary>
    /// <param name="cause">"angular dimension", "dimension_set of 'D1@Sketch1'".</param>
    /// <param name="undo">How to set it back: "dimension_set 'D1@Sketch1' back to 60".</param>
    public static string? Warning(string cause, IEnumerable<LineLength> lines, double explainedM, string undo)
    {
        var shrunk = lines.Where(l => l.Before > MinM && l.After < l.Before / 2 && l.Before - l.After > 2 * Math.Abs(explainedM))
                          .OrderByDescending(l => l.Before - l.After).ToList();
        if (shrunk.Count == 0) return null;
        string list = string.Join(", ", shrunk.Take(ListMax).Select(l => $"'{l.Name}' {Mm(l.Before)} to {Mm(l.After)} mm")) +
                      (shrunk.Count > ListMax ? $" and {shrunk.Count - ListMax} more" : "");
        return $"After the {cause}, {(shrunk.Count == 1 ? "a line is" : "lines are")} less than half as long as before: {list}. " +
               $"The value reads back, but the sketch may have turned inside out: if that was not meant, {undo}.";
    }

    private static string Mm(double m) => (Math.Round(m * 1000, 3) + 0.0).ToString("0.###", CultureInfo.InvariantCulture);
}
