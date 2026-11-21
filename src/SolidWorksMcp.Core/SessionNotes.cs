using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using SolidWorksMcp.Core.Win32;

namespace SolidWorksMcp.Core;

/// <summary>
/// What sw_connect and NOT_CONNECTED say about the processes around the server (S60 b, c). Free of the SolidWorks
/// interop, so the unit tests compile it in.
/// </summary>
public static class SessionNotes
{
    /// <summary>A process's start time (local), or null when it is gone or can't be read.</summary>
    public static DateTime? StartTime(int pid)
    {
        try
        {
            using var p = Process.GetProcessById(pid);
            return p.StartTime;
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or System.ComponentModel.Win32Exception) { return null; }
    }

    /// <summary>The other solidworks-mcp processes on this machine (an older build, another session), with their start time.</summary>
    public static IReadOnlyList<(int Pid, DateTime? Started)> OtherServerProcesses()
    {
        try
        {
            var me = Environment.ProcessId;
            var procs = Process.GetProcessesByName("solidworks-mcp");
            try { return procs.Where(p => p.Id != me).Select(p => (p.Id, StartTime(p.Id))).ToList(); }
            finally { foreach (var p in procs) p.Dispose(); }
        }
        catch (InvalidOperationException) { return []; }
    }

    /// <summary>A process's command line, or null when it is gone or can't be read (another user, an elevated process).</summary>
    public static string? CommandLine(int pid)
    {
        IntPtr h = NativeMethods.OpenProcess(NativeMethods.PROCESS_QUERY_LIMITED_INFORMATION, false, pid);
        if (h == IntPtr.Zero) return null;
        IntPtr buf = IntPtr.Zero;
        try
        {
            NativeMethods.NtQueryInformationProcess(h, NativeMethods.ProcessCommandLineInformation, IntPtr.Zero, 0, out int size);
            if (size <= 0) return null;
            buf = Marshal.AllocHGlobal(size);
            if (NativeMethods.NtQueryInformationProcess(h, NativeMethods.ProcessCommandLineInformation, buf, size, out _) != 0) return null;
            // A UNICODE_STRING (Length in bytes, MaximumLength, Buffer) with its text after it in the same block.
            int length = (ushort)Marshal.ReadInt16(buf);
            IntPtr text = Marshal.ReadIntPtr(buf, IntPtr.Size);
            return length == 0 || text == IntPtr.Zero ? "" : Marshal.PtrToStringUni(text, length / 2);
        }
        finally
        {
            if (buf != IntPtr.Zero) Marshal.FreeHGlobal(buf);
            NativeMethods.CloseHandle(h);
        }
    }

    /// <summary>
    /// Whether a command line is that of a COM server started through COM (the DCOM launcher adds -Embedding): such a
    /// SOLIDWORKS stays out of the running object table, so only the process that created it holds it (S89).
    /// </summary>
    public static bool StartedThroughCom(string? commandLine) =>
        commandLine is not null && Regex.IsMatch(commandLine, @"(^|\s)[-/]Embedding(\s|$)", RegexOptions.IgnoreCase);

    /// <summary>The executable in a COM server's LocalServer32 value (quoted or not, its arguments dropped).</summary>
    public static string? ExeFromLocalServer(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        value = value.Trim();
        if (value.StartsWith('"')) { int q = value.IndexOf('"', 1); return q > 1 ? value[1..q] : null; }
        int i = value.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
        return i > 0 ? value[..(i + 4)] : value.Split(' ')[0];
    }

    /// <summary>
    /// NOT_CONNECTED for a SOLIDWORKS that runs but is not in the COM running object table. With another server process
    /// running, the error names it (S60 b): it may have started SOLIDWORKS at its own privilege level, or hold it in a call.
    /// A SOLIDWORKS started through COM (<paramref name="startedThroughCom"/>) is never "still starting" (S89).
    /// </summary>
    public static SwException Unreachable(IReadOnlyList<int> solidWorksPids, IReadOnlyList<(int Pid, DateTime? Started)> otherServers,
        bool startedThroughCom = false)
    {
        string others = string.Join(", ", otherServers.Select(o => o.Started is { } t
            ? $"pid {o.Pid} since {t.ToString("yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture)}"
            : $"pid {o.Pid}"));
        string head = $"SOLIDWORKS is running (pid {string.Join(", ", solidWorksPids)}) but can't be reached over COM: ";
        const string dontEnd = "never end it yourself: ask the user to close that session or restart SOLIDWORKS. ";
        if (startedThroughCom)
            return new SwException(ErrorCodes.NotConnected,
                head + (otherServers.Count == 0
                    ? "it was started through COM by an earlier server process: only it could reach it."
                    : $"it was started through COM, so only the server process that started it can reach it (running: {others})."),
                (otherServers.Count == 0 ? "" : "If that is another session's process, " + dontEnd) +
                "Ask the user to close that SOLIDWORKS (saving their work; with no window shown, end SLDWORKS.exe) and start " +
                "SOLIDWORKS from its exe, then call sw_connect.");
        return new SwException(ErrorCodes.NotConnected,
            head + "it may still be starting, " +
            (otherServers.Count == 0
                ? "or it runs with different privileges than this server."
                : $"run with different privileges than this server, or be held by another server process ({others}: an older build or another session)."),
            (otherServers.Count == 0 ? "" : "That process may have started SOLIDWORKS at its own level, or be stuck in a call; " + dontEnd) +
            "Wait until SOLIDWORKS has finished starting and retry. If it was started 'as administrator' (or the MCP client was), " +
            "restart one of them so both run at the same level. A second SOLIDWORKS is never launched.");
    }

    /// <summary>
    /// Whether sw_connect found the SOLIDWORKS process this server was attached to before (S60 c): the same pid and start
    /// time, so a restart the user was asked for did not happen, or a new one.
    /// </summary>
    public static string ProcessNote(int pid, DateTime? started, int? previousPid, DateTime? previousStarted)
    {
        if (previousPid is not int prev) return "first connection of this server";
        if (prev == pid && (started is null || previousStarted is null || started == previousStarted)) return "same as before";
        return $"a new one (before: pid {prev}" +
               (previousStarted is { } t ? $", started {t.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)})" : ")");
    }
}
