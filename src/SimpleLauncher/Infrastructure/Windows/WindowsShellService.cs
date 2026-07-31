using System.Runtime.InteropServices;
using SimpleLauncher.Logging;

namespace SimpleLauncher.Infrastructure.Windows;

/// <summary>
/// Applies reversible Windows shell presentation settings for kiosk-style sessions.
/// </summary>
public sealed class WindowsShellService(FileLogger logger)
{
    private const int SW_HIDE = 0;
    private const int SW_SHOW = 5;

    public void HideTaskbar()
    {
        var handle = FindWindow("Shell_TrayWnd", null);
        if (handle != IntPtr.Zero)
        {
            ShowWindow(handle, SW_HIDE);
            logger.Info("Taskbar hidden.");
        }
    }

    public void RestoreDesktop()
    {
        var handle = FindWindow("Shell_TrayWnd", null);
        if (handle != IntPtr.Zero)
        {
            ShowWindow(handle, SW_SHOW);
            logger.Info("Taskbar restored.");
        }
    }

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr FindWindow(string className, string? windowName);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool ShowWindow(IntPtr hWnd, int command);
}
