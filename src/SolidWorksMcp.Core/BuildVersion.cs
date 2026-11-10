using System.Reflection;

namespace SolidWorksMcp.Core;

/// <summary>
/// The version a build reports (sw_status serverVersion, the MCP server info): the assembly's informational version
/// with the commit it was built from, so a test result can be pinned to the code that ran. Interop-free.
/// </summary>
public static class BuildVersion
{
    /// <summary>This build's version, read from the assembly of <paramref name="anchor"/> (default: Core).</summary>
    public static string Of(Type? anchor = null) =>
        Format((anchor ?? typeof(BuildVersion)).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);

    /// <summary>
    /// "0.1.0-x+&lt;40-hex commit&gt;[.dirty]" → "0.1.0-x+&lt;7-hex commit&gt;[.dirty]". The SDK appends the commit after '+'
    /// (SourceRevisionId); a full hash is shortened to git's usual 7 characters, anything else after it is kept.
    /// </summary>
    public static string Format(string? informational)
    {
        if (string.IsNullOrWhiteSpace(informational)) return "0.0.0";
        int plus = informational.IndexOf('+');
        if (plus < 0) return informational;
        string version = informational[..plus];
        string[] meta = informational[(plus + 1)..].Split('.');
        if (meta[0].Length > 7 && meta[0].All(Uri.IsHexDigit)) meta[0] = meta[0][..7];
        string tail = string.Join('.', meta.Where(m => m.Length > 0));
        return tail.Length == 0 ? version : $"{version}+{tail}";
    }
}
