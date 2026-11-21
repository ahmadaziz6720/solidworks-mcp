using SolidWorksMcp.Core;
using Xunit;

namespace SolidWorksMcp.UnitTests;

/// <summary>What sw_connect and NOT_CONNECTED say about the processes around the server (S60 b, c).</summary>
public class SessionNoteTests
{
    private static readonly DateTime T = new(2026, 11, 13, 12, 41, 7);

    [Fact]
    public void The_first_connection_of_a_server_says_so() =>
        Assert.Equal("first connection of this server", SessionNotes.ProcessNote(5148, T, null, null));

    [Fact]
    public void The_same_pid_and_start_time_is_the_same_process() =>
        Assert.Equal("same as before", SessionNotes.ProcessNote(5148, T, 5148, T));

    [Fact]
    public void Another_pid_is_a_new_process_naming_the_one_before() =>
        Assert.Equal("a new one (before: pid 5148, started 2026-11-13 12:41:07)", SessionNotes.ProcessNote(23516, T.AddHours(2), 5148, T));

    [Fact]
    public void A_reused_pid_with_another_start_time_is_a_new_process() =>
        Assert.StartsWith("a new one (before: pid 5148", SessionNotes.ProcessNote(5148, T.AddHours(2), 5148, T));

    [Fact]
    public void Unreachable_without_another_server_names_privileges_only()
    {
        var ex = SessionNotes.Unreachable([5148], []);
        Assert.Equal(ErrorCodes.NotConnected, ex.Code);
        Assert.Contains("different privileges", ex.Message);
        Assert.DoesNotContain("server process", ex.ToAgentText());
    }

    [Fact]
    public void Unreachable_names_the_other_server_process_and_says_not_to_end_it()
    {
        var ex = SessionNotes.Unreachable([5148], [(21332, new DateTime(2026, 11, 13, 7, 54, 3))]);
        Assert.Contains("another server process (pid 21332 since 2026-11-13 07:54", ex.Message);
        Assert.Contains("never end it yourself", ex.Hint);
        Assert.Contains("different privileges", ex.Message);
    }

    [Fact]
    public void Unreachable_started_through_COM_names_the_earlier_server_never_still_starting()
    {
        // S89: SOLIDWORKS started by an earlier server's sw_connect {launch} (-Embedding) was reported as "may still be starting".
        var ex = SessionNotes.Unreachable([5148], [], startedThroughCom: true);
        Assert.Equal(ErrorCodes.NotConnected, ex.Code);
        Assert.Contains("started through COM by an earlier server process: only it could reach it", ex.Message);
        Assert.Contains("start SOLIDWORKS from its exe", ex.Hint);
        Assert.DoesNotContain("still starting", ex.ToAgentText());
        Assert.DoesNotContain("Wait until", ex.ToAgentText());
    }

    [Fact]
    public void Unreachable_started_through_COM_with_another_server_running_names_it()
    {
        var ex = SessionNotes.Unreachable([5148], [(21332, new DateTime(2026, 11, 13, 7, 54, 3))], startedThroughCom: true);
        Assert.Contains("only the server process that started it can reach it (running: pid 21332 since 2026-11-13 07:54)", ex.Message);
        Assert.Contains("never end it yourself", ex.Hint);
        Assert.DoesNotContain("still starting", ex.ToAgentText());
    }

    [Theory]
    [InlineData(@"C:\PROGRA~1\SOLIDW~1\SOLIDW~1\SLDWORKS.exe -Embedding", true)]
    [InlineData(@"""C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\SLDWORKS.exe"" /Embedding", true)]
    [InlineData(@"""C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\SLDWORKS.exe""", false)]
    [InlineData(@"C:\Embedding\SLDWORKS.exe", false)]
    [InlineData(null, false)]
    public void A_COM_started_process_is_told_by_Embedding_on_its_command_line(string? commandLine, bool expected) =>
        Assert.Equal(expected, SessionNotes.StartedThroughCom(commandLine));

    [Fact]
    public void The_command_line_of_a_running_process_is_read() =>
        Assert.False(string.IsNullOrEmpty(SessionNotes.CommandLine(Environment.ProcessId)));

    [Theory]
    [InlineData(@"C:\PROGRA~1\SOLIDW~1\SOLIDW~1\SLDWORKS.exe", @"C:\PROGRA~1\SOLIDW~1\SOLIDW~1\SLDWORKS.exe")]
    [InlineData(@"""C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\SLDWORKS.exe"" -automation", @"C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\SLDWORKS.exe")]
    [InlineData(@"C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\SLDWORKS.exe /automation", @"C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\SLDWORKS.exe")]
    [InlineData("", null)]
    public void The_exe_is_read_from_a_LocalServer32_value(string value, string? expected) =>
        Assert.Equal(expected, SessionNotes.ExeFromLocalServer(value));
}
