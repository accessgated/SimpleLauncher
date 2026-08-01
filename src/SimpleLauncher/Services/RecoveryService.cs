using System.Diagnostics;
using System.Windows;
using SimpleLauncher.Infrastructure.Configuration;
using SimpleLauncher.Infrastructure.Security;
using SimpleLauncher.Infrastructure.Windows;
using SimpleLauncher.Logging;
using SimpleLauncher.Models;
using SimpleLauncher.Presentation.Windows;

namespace SimpleLauncher.Services;

/// <summary>
/// Coordinates the recovery experience and its watchdog lifecycle.
/// </summary>
public sealed class RecoveryService(
    LauncherConfiguration configuration,
    IConfigurationStore configurationStore,
    Pbkdf2PinHasher pinHasher,
    FileLogger logger,
    DesktopSessionService desktopSessionService,
    AppPaths paths,
    LocalizationService localizationService)
{
    private readonly object syncRoot = new();
    private bool recoveryActive;

    public void StartRecoveryIfNeeded()
    {
        lock (syncRoot)
        {
            if (recoveryActive)
            {
                return;
            }

            recoveryActive = true;
        }

        logger.Info("Recovery launched.");
        var recoveryWindow = new RecoveryWindow(configuration, configurationStore, pinHasher, logger, desktopSessionService, this, localizationService);
        recoveryWindow.Show();
    }

    public void ReportExit()
    {
        lock (syncRoot)
        {
            recoveryActive = false;
        }
    }

    public void StartLauncherAndExitRecovery()
    {
        var launcherPath = Path.Combine(AppContext.BaseDirectory, "SimpleLauncher.exe");
        if (!File.Exists(launcherPath))
        {
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = launcherPath,
            UseShellExecute = true,
            WorkingDirectory = AppContext.BaseDirectory
        });
    }
}
