using System.Diagnostics;
using System.Runtime.InteropServices;
using SimpleLauncher.Logging;

namespace SimpleLauncher.Infrastructure.Windows;

/// <summary>
/// Manages kiosk desktop session behavior such as explorer shutdown and recovery flow.
/// </summary>
public sealed class DesktopSessionService(FileLogger logger)
{
    private const int SW_HIDE = 0;
    private const int SW_SHOW = 5;

    public bool IsExplorerRunning()
    {
        return Process.GetProcessesByName("explorer").Any(process => !process.HasExited);
    }

    public void KillExplorerIfRunning()
    {
        foreach (var process in Process.GetProcessesByName("explorer"))
        {
            try
            {
                process.Kill(entireProcessTree: true);
                process.WaitForExit(2000);
                logger.Info("Explorer terminated for launcher mode.");
            }
            catch (Exception ex)
            {
                logger.Error("Failed to terminate explorer.", ex);
            }
        }
    }

    public void StartExplorerIfNeeded()
    {
        if (IsExplorerRunning())
        {
            return;
        }

        try
        {
            var explorerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");
            Process.Start(new ProcessStartInfo
            {
                FileName = explorerPath,
                UseShellExecute = true,
                WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Windows)
            });
            logger.Info("Explorer started.");
        }
        catch (Exception ex)
        {
            logger.Error("Failed to start explorer.", ex);
        }
    }

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
