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
    private WindowsShellService? shellService;

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        var paths = AppPaths.CreateDefault();
        Directory.CreateDirectory(paths.ConfigurationDirectory);
        Directory.CreateDirectory(paths.LogDirectory);

        logger = new FileLogger(paths.LogFilePath);
        logger.Info("SimpleLauncher starting.");

        var passwordHasher = new Pbkdf2PinHasher();
        var validator = new ApplicationPathValidator();
        var migrator = new IniConfigurationMigrator(paths, logger);
        configurationStore = new LauncherConfigurationStore(paths, logger, migrator, validator, passwordHasher);
        shellService = new WindowsShellService(logger);

        try
        {
            var configuration = await configurationStore.LoadAsync();
            var iconService = new WindowsIconService(logger);
            var processLauncher = new ProcessLauncher(logger, validator);
            var backupService = new ConfigurationBackupService(paths, logger);
            var mainWindow = new MainWindow(configuration, configurationStore, processLauncher, iconService, shellService, backupService, passwordHasher, logger);
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

    protected override void OnExit(ExitEventArgs e)
    {
        shellService?.RestoreDesktop();
        logger?.Info("SimpleLauncher stopped.");
        logger?.Dispose();
        base.OnExit(e);
    }
}
