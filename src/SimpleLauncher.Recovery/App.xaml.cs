using System.IO;
using System.Windows;
using SimpleLauncher.Infrastructure.Configuration;
using SimpleLauncher.Infrastructure.Migration;
using SimpleLauncher.Infrastructure.Security;
using SimpleLauncher.Infrastructure.Windows;
using SimpleLauncher.Logging;
using SimpleLauncher.Presentation.Windows;
using SimpleLauncher.Services;

namespace SimpleLauncher.Recovery;

public partial class App : Application
{
    private async void OnStartup(object sender, StartupEventArgs e)
    {
        var paths = AppPaths.CreateDefault();
        Directory.CreateDirectory(paths.ConfigurationDirectory);
        Directory.CreateDirectory(paths.LogDirectory);
        Directory.CreateDirectory(paths.AssetsDirectory);

        var logger = new FileLogger(paths.LogFilePath);
        var passwordHasher = new Pbkdf2PinHasher();
        var configurationStore = new LauncherConfigurationStore(paths, logger, new IniConfigurationMigrator(paths, logger), new ApplicationPathValidator(), passwordHasher);
        var configuration = await configurationStore.LoadAsync();
        var desktopSessionService = new DesktopSessionService(logger);
        var localizationService = new LocalizationService(paths, logger);
        localizationService.LoadCache();
        var recoveryService = new RecoveryService(configuration, configurationStore, passwordHasher, logger, desktopSessionService, localizationService);
        var recoveryWindow = new RecoveryWindow(configuration, configurationStore, passwordHasher, logger, desktopSessionService, recoveryService, localizationService);
        MainWindow = recoveryWindow;
        recoveryWindow.Show();
    }
}
