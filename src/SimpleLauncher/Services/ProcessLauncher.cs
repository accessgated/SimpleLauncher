using System.Diagnostics;
using SimpleLauncher.Infrastructure.Security;
using SimpleLauncher.Logging;
using SimpleLauncher.Models;

namespace SimpleLauncher.Services;

/// <summary>
/// Starts approved applications through Windows shell execution.
/// </summary>
public sealed class ProcessLauncher(FileLogger logger, ApplicationPathValidator validator)
{
    public bool TryLaunch(LauncherApplication app, out string errorMessage)
    {
        if (!validator.IsLaunchablePath(app.Path))
        {
            errorMessage = "The selected application path is missing or not allowed.";
            return false;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Path.GetFullPath(app.Path),
                Arguments = app.Arguments ?? string.Empty,
                UseShellExecute = true,
                WorkingDirectory = Directory.Exists(app.Path) ? app.Path : Path.GetDirectoryName(app.Path) ?? AppContext.BaseDirectory
            });

            logger.Info($"Launched '{app.DisplayName}'.");
            errorMessage = string.Empty;
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or System.ComponentModel.Win32Exception)
        {
            logger.Error($"Failed to launch '{app.DisplayName}'.", ex);
            errorMessage = "Windows could not start that application. Check the path and permissions.";
            return false;
        }
    }
}
