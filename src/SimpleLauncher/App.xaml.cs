using System.Diagnostics;
using System.Windows;
using SimpleLauncher.Infrastructure.Configuration;
using SimpleLauncher.Infrastructure.Migration;
using SimpleLauncher.Infrastructure.Security;
using SimpleLauncher.Infrastructure.Windows;
using SimpleLauncher.Logging;
using SimpleLauncher.Presentation.Windows;
using SimpleLauncher.Services;

namespace SimpleLauncher;

/// <summary>
/// Application composition root.
/// </summary>
public partial class App : System.Windows.Application
{
    private FileLogger? logger;
    private LauncherConfigurationStore? configurationStore;
    private DesktopSessionService? desktopSessionService;
    private LocalizationService? localizationService;
    private RecoveryService? recoveryService;

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        var paths = AppPaths.CreateDefault();
        Directory.CreateDirectory(paths.ConfigurationDirectory);
        Directory.CreateDirectory(paths.LogDirectory);
        Directory.CreateDirectory(paths.AssetsDirectory);

        var packagedBackgroundPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Background.jpg");
        var managedBackgroundPath = Path.Combine(paths.AssetsDirectory, "Background.jpg");
        if (File.Exists(packagedBackgroundPath) && !File.Exists(managedBackgroundPath))
        {
            File.Copy(packagedBackgroundPath, managedBackgroundPath);
        }

        logger = new FileLogger(paths.LogFilePath);
        logger.Info("SimpleLauncher starting.");

        var passwordHasher = new Pbkdf2PinHasher();
        var validator = new ApplicationPathValidator();
        var migrator = new IniConfigurationMigrator(paths, logger);
        configurationStore = new LauncherConfigurationStore(paths, logger, migrator, validator, passwordHasher);
        desktopSessionService = new DesktopSessionService(logger);
        localizationService = new LocalizationService(paths, logger);
        localizationService.LoadCache();

        try
        {
            var configuration = await configurationStore.LoadAsync();
            var iconService = new WindowsIconService(logger);
            var processLauncher = new ProcessLauncher(logger, validator);
            var backupService = new ConfigurationBackupService(paths, logger);
            var closer = new DesktopApplicationCloser(logger);
            recoveryService = new RecoveryService(configuration, configurationStore, passwordHasher, logger, desktopSessionService, paths, localizationService);
            var mainWindow = new MainWindow(configuration, configurationStore, processLauncher, iconService, desktopSessionService, backupService, passwordHasher, logger, localizationService, closer, recoveryService, paths);
            MainWindow = mainWindow;
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            logger.Error("Startup failed.", ex);
            MessageBox.Show("SimpleLauncher could not start. A log file was written for troubleshooting.", "SimpleLauncher", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    public bool ShouldAutoRecover { get; private set; } = true;

    public void MarkExitToWindows()
    {
        ShouldAutoRecover = false;
    }

    private static void LaunchRecoveryExecutable()
    {
        var recoveryPath = Path.Combine(AppContext.BaseDirectory, "SimpleLauncherRecovery.exe");
        if (!File.Exists(recoveryPath))
        {
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = recoveryPath,
            UseShellExecute = true,
            WorkingDirectory = AppContext.BaseDirectory
        });
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (ShouldAutoRecover)
        {
            LaunchRecoveryExecutable();
        }

        desktopSessionService?.StartExplorerIfNeeded();
        desktopSessionService?.RestoreDesktop();
        logger?.Info("SimpleLauncher stopped.");
        logger?.Dispose();
        base.OnExit(e);
    }
}
