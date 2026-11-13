using System.Diagnostics;

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

    /// <summary>
    /// NOT_CONNECTED for a SOLIDWORKS that runs but is not in the COM running object table. With another server process
    /// running, the error names it (S60 b): it may have started SOLIDWORKS at its own privilege level, or hold it in a call.
    /// </summary>
    public static SwException Unreachable(IReadOnlyList<int> solidWorksPids, IReadOnlyList<(int Pid, DateTime? Started)> otherServers)
    {
        string others = string.Join(", ", otherServers.Select(o => o.Started is { } t
            ? $"pid {o.Pid} since {t.ToString("yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture)}"
            : $"pid {o.Pid}"));
        return new SwException(ErrorCodes.NotConnected,
            $"SOLIDWORKS is running (pid {string.Join(", ", solidWorksPids)}) but can't be reached over COM: it may still be starting, " +
            (otherServers.Count == 0
                ? "or it runs with different privileges than this server."
                : $"run with different privileges than this server, or be held by another server process ({others}: an older build or another session)."),
            (otherServers.Count == 0 ? "" :
                "That process may have started SOLIDWORKS at its own level, or be stuck in a call; never end it yourself: ask the user to " +
                "close that session or restart SOLIDWORKS. ") +
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
