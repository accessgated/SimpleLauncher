using System.Windows;
using SimpleLauncher.Infrastructure.Security;
using SimpleLauncher.Infrastructure.Windows;
using SimpleLauncher.Logging;
using SimpleLauncher.Models;
using SimpleLauncher.Presentation.Dialogs;
using SimpleLauncher.Services;

namespace SimpleLauncher.Presentation.Windows;

public partial class RecoveryWindow : Window
{
    private readonly LauncherConfiguration configuration;
    private readonly IConfigurationStore configurationStore;
    private readonly Pbkdf2PinHasher pinHasher;
    private readonly FileLogger logger;
    private readonly DesktopSessionService desktopSessionService;
    private readonly RecoveryService recoveryService;
    private readonly LocalizationService localizationService;

    public RecoveryWindow(
        LauncherConfiguration configuration,
        IConfigurationStore configurationStore,
        Pbkdf2PinHasher pinHasher,
        FileLogger logger,
        DesktopSessionService desktopSessionService,
        RecoveryService recoveryService,
        LocalizationService localizationService)
    {
        this.configuration = configuration;
        this.configurationStore = configurationStore;
        this.pinHasher = pinHasher;
        this.logger = logger;
        this.desktopSessionService = desktopSessionService;
        this.recoveryService = recoveryService;
        this.localizationService = localizationService;

        InitializeComponent();
        ApplyLocalization();
    }

    private void OnReturnToLauncher(object sender, RoutedEventArgs e)
    {
        recoveryService.StartLauncherAndExitRecovery();
        Close();
    }

    private void OnOpenDesktop(object sender, RoutedEventArgs e)
    {
        if (!configuration.Security.IsPinEnabled || string.IsNullOrWhiteSpace(configuration.Security.PinHash))
        {
            desktopSessionService.StartExplorerIfNeeded();
            Close();
            return;
        }

        var dialog = new PinDialog(localizationService, configuration) { Owner = this };
        if (dialog.ShowDialog() != true)
        {
            return;
        }

        if (!pinHasher.Verify(dialog.Pin, configuration.Security.PinHash))
        {
            MessageBox.Show(this, localizationService.GetString("RecoveryPinError", configuration), "SimpleLauncher", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        desktopSessionService.StartExplorerIfNeeded();
        Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        recoveryService.ReportExit();
        base.OnClosed(e);
    }

    private void ApplyLocalization()
    {
        Title = localizationService.GetString("RecoveryTitle", configuration);
        TitleText.Text = localizationService.GetString("RecoveryTitle", configuration);
        ReturnButton.Content = localizationService.GetString("RecoveryReturn", configuration);
        DesktopButton.Content = localizationService.GetString("RecoveryDesktop", configuration);
    }
}
