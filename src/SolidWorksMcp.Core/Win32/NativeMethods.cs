using System.Runtime.InteropServices;
using System.Text;

namespace SolidWorksMcp.Core.Win32;

/// <summary>user32/ole32/oleaut32 P/Invoke declarations used by the session, the dispatcher and the dialog watchdog.</summary>
internal static class NativeMethods
{
    public const uint WM_COMMAND = 0x0111;
    public const uint WM_CLOSE = 0x0010;
    public const int IDOK = 1;
    public const int IDCANCEL = 2;
    public const uint GW_OWNER = 4;

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern uint GetLongPathName(string shortPath, StringBuilder longPath, uint size);

    /// <summary>The path with its 8.3 components expanded (the input when it does not exist or has none).</summary>
    public static string LongPath(string path)
    {
        try
        {
            var sb = new StringBuilder(520);
            uint n = GetLongPathName(path, sb, (uint)sb.Capacity);
            if (n > sb.Capacity) { sb = new StringBuilder((int)n + 1); n = GetLongPathName(path, sb, (uint)sb.Capacity); }
            return n > 0 && n < sb.Capacity ? sb.ToString() : path;
        }
        catch (Exception ex) when (ex is DllNotFoundException or EntryPointNotFoundException) { return path; }
    }

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumWindows(EnumWindowsProc callback, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumChildWindows(IntPtr parent, EnumWindowsProc callback, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder className, int maxCount);

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowEnabled(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint msg, IntPtr wParam, StringBuilder lParam,
        uint flags, uint timeoutMs, out IntPtr result);

    [DllImport("user32.dll")]
    public static extern int GetDlgCtrlID(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr GetWindow(IntPtr hWnd, uint cmd);

    // --- Another process's command line (S89: a SOLIDWORKS started through COM runs with -Embedding) ---

    public const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;
    public const int ProcessCommandLineInformation = 60;

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr OpenProcess(uint access, [MarshalAs(UnmanagedType.Bool)] bool inherit, int processId);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(IntPtr handle);

    [DllImport("ntdll.dll")]
    public static extern int NtQueryInformationProcess(IntPtr process, int infoClass, IntPtr info, int length, out int returnLength);

    // --- COM activation (Marshal.GetActiveObject does not exist in modern .NET) ---

    [DllImport("ole32.dll", CharSet = CharSet.Unicode)]
    public static extern int CLSIDFromProgID(string progId, out Guid clsid);

    [DllImport("oleaut32.dll")]
    public static extern int GetActiveObject(ref Guid clsid, IntPtr reserved,
        [MarshalAs(UnmanagedType.IUnknown)] out object? obj);

    [DllImport("ole32.dll")]
    public static extern int CoRegisterMessageFilter(IOleMessageFilter? newFilter, out IOleMessageFilter? oldFilter);

    public static string GetText(IntPtr hWnd)
    {
        int len = GetWindowTextLength(hWnd);
        if (len <= 0) return "";
        var sb = new StringBuilder(len + 1);
        GetWindowText(hWnd, sb, sb.Capacity);
        return sb.ToString();
    }

    /// <summary>Reads a control's text with WM_GETTEXT and a timeout, so a hung target window cannot block the caller.</summary>
    public static string GetTextSafe(IntPtr hWnd)
    {
        const uint WM_GETTEXT = 0x000D, SMTO_ABORTIFHUNG = 0x0002;
        var sb = new StringBuilder(2048);
        IntPtr r = SendMessageTimeout(hWnd, WM_GETTEXT, (IntPtr)sb.Capacity, sb, SMTO_ABORTIFHUNG, 500, out _);
        return r == IntPtr.Zero ? GetText(hWnd) : sb.ToString();
    }

    public static string GetClass(IntPtr hWnd)
    {
        var sb = new StringBuilder(256);
        GetClassName(hWnd, sb, sb.Capacity);
        return sb.ToString();
    }
}

/// <summary>The classic OLE IMessageFilter interface (used to retry calls SolidWorks rejects while busy).</summary>
[ComImport, Guid("00000016-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IOleMessageFilter
{
    [PreserveSig]
    int HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo);

    [PreserveSig]
    int RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType);

    [PreserveSig]
    int MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType);
}
