using System.Diagnostics;
using System.Runtime.InteropServices;
using SimpleLauncher.Logging;
using SimpleLauncher.Models;

namespace SimpleLauncher.Services;

/// <summary>
/// Gracefully closes visible desktop applications before entering launcher mode.
/// </summary>
public sealed class DesktopApplicationCloser(FileLogger logger)
{
    private const int WM_CLOSE = 0x0010;

    public void CloseVisibleApplications(LauncherConfiguration configuration)
    {
        foreach (var process in Process.GetProcesses())
        {
            if (!ShouldClose(process, configuration))
            {
                continue;
            }

            try
            {
                if (process.MainWindowHandle != IntPtr.Zero)
                {
                    SendMessage(process.MainWindowHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                }

                if (!process.WaitForExit(3000))
                {
                    process.Kill(entireProcessTree: true);
                }

                logger.Info($"Closed application '{process.ProcessName}'.");
            }
            catch (Exception ex)
            {
                logger.Error($"Failed to close app '{process.ProcessName}'.", ex);
            }
        }
    }

    private static bool ShouldClose(Process process, LauncherConfiguration configuration)
    {
        if (process.ProcessName.Equals("explorer", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (process.ProcessName.Equals("SimpleLauncher", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (process.ProcessName.Equals("SimpleLauncherRecovery", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (process.MainWindowHandle == IntPtr.Zero)
        {
            return false;
        }

        if (process.ProcessName is "csrss" or "wininit" or "winlogon" or "services" or "lsass" or "svchost")
        {
            return false;
        }

        if (process.ProcessName is "Idle")
        {
            return false;
        }

        if (configuration.Kiosk.DesktopCloseExclusions.Contains(process.ProcessName, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
}
