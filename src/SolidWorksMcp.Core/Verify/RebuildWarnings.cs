namespace SolidWorksMcp.Core.Verify;

// Interop-free half of the rebuild warnings a feature tool reports (linked into the offline unit tests).

/// <summary>A feature's rebuild state as read from the tree: its name now, the code, warning or error, the code's name.</summary>
internal sealed record RebuildState(string Name, int Code, bool Warning, string ErrorName);

/// <summary>
/// S84 (b): a feature tool reads its new feature's rebuild warning at creation, but reports it only once the call is
/// done with it: renamed, and rebuilt. A pattern made in the wrong sense is taken back and made again, so the warning
/// read on the first one belongs to a feature that is gone; and a call's <c>name</c> renames the feature after the
/// check. The lines name each feature as it is called now, and leave out a feature that is no longer in the tree or
/// whose warning the rebuild cleared.
/// </summary>
internal static class RebuildWarnings
{
    /// <summary>
    /// The warning lines for the features deferred by id, in their order, each once. <paramref name="now"/>: the features
    /// with a rebuild error or warning after the rebuild, by id (a feature deleted or cleared is not in it).
    /// </summary>
    public static List<string> Lines(IEnumerable<int> deferred, IReadOnlyDictionary<int, RebuildState> now)
    {
        var lines = new List<string>();
        foreach (int id in deferred.Distinct())
        {
            if (!now.TryGetValue(id, out var s) || s.Code == 0) continue;
            lines.Add(s.Warning
                ? $"'{s.Name}' has rebuild warning {s.Code} ({s.ErrorName})."
                : $"'{s.Name}' has rebuild error {s.Code} ({s.ErrorName}) after the rebuild; it had a warning when made.");
        }
        return lines;
    }
}
