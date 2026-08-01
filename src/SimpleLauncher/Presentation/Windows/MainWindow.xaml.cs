using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using SimpleLauncher.Infrastructure.Configuration;
using SimpleLauncher.Infrastructure.Security;
using SimpleLauncher.Infrastructure.Windows;
using SimpleLauncher.Logging;
using SimpleLauncher.Models;
using SimpleLauncher.Presentation.Dialogs;
using SimpleLauncher.Services;

namespace SimpleLauncher.Presentation.Windows;

public partial class MainWindow : Window
{
    private readonly LauncherConfiguration configuration;
    private readonly IConfigurationStore configurationStore;
    private readonly ProcessLauncher launcher;
    private readonly WindowsIconService iconService;
    private readonly DesktopSessionService desktopSessionService;
    private readonly ConfigurationBackupService backupService;
    private readonly Pbkdf2PinHasher pinHasher;
    private readonly FileLogger logger;
    private readonly LocalizationService localizationService;
    private readonly DesktopApplicationCloser applicationCloser;
    private readonly RecoveryService recoveryService;
    private readonly AppPaths paths;
    private readonly ObservableCollection<LauncherTile> tiles = [];

    public MainWindow(
        LauncherConfiguration configuration,
        IConfigurationStore configurationStore,
        ProcessLauncher launcher,
        WindowsIconService iconService,
        DesktopSessionService desktopSessionService,
        ConfigurationBackupService backupService,
        Pbkdf2PinHasher pinHasher,
        FileLogger logger,
        LocalizationService localizationService,
        DesktopApplicationCloser applicationCloser,
        RecoveryService recoveryService,
        AppPaths paths)
    {
        this.configuration = configuration;
        this.configurationStore = configurationStore;
        this.launcher = launcher;
        this.iconService = iconService;
        this.desktopSessionService = desktopSessionService;
        this.backupService = backupService;
        this.pinHasher = pinHasher;
        this.logger = logger;
        this.localizationService = localizationService;
        this.applicationCloser = applicationCloser;
        this.recoveryService = recoveryService;
        this.paths = paths;

        InitializeComponent();
        Title = localizationService.GetString("AppTitle", configuration);
        TitleText.Text = localizationService.GetString("AppTitle", configuration);
        SearchBox.ToolTip = localizationService.GetString("SearchPlaceholder", configuration);
        AddButton.Content = localizationService.GetString("Add", configuration);
        SettingsButton.Content = localizationService.GetString("Settings", configuration);
        ExitButton.Content = localizationService.GetString("Exit", configuration);
        Topmost = configuration.Kiosk.TopMost;
        AppsList.ItemsSource = tiles;
        ApplyBackground();
        RefreshTiles();

        if (configuration.Kiosk.HideTaskbar && desktopSessionService.IsExplorerRunning())
        {
            desktopSessionService.HideTaskbar();
        }

        desktopSessionService.KillExplorerIfRunning();
        applicationCloser.CloseVisibleApplications(configuration);
    }

    private void ApplyBackground()
    {
        var backgroundPath = configuration.Theme.BackgroundImagePath;
        if (!string.IsNullOrWhiteSpace(backgroundPath) && File.Exists(backgroundPath))
        {
            BackgroundImage.Source = new BitmapImage(new Uri(backgroundPath));
            return;
        }

        var fallbackPath = Path.Combine(paths.AssetsDirectory, "Background.jpg");
        if (File.Exists(fallbackPath))
        {
            BackgroundImage.Source = new BitmapImage(new Uri(fallbackPath));
        }
    }

    private void RefreshTiles()
    {
        var query = SearchBox.Text.Trim();
        var apps = configuration.Apps.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(query))
        {
            apps = apps.Where(app => app.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) || app.Category.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        apps = configuration.Layout.SortMode.Equals("Name", StringComparison.OrdinalIgnoreCase)
            ? apps.OrderBy(app => app.DisplayName)
            : apps.OrderByDescending(app => configuration.Layout.ShowFavoritesFirst && app.IsFavorite).ThenBy(app => app.PositionY).ThenBy(app => app.PositionX);

        tiles.Clear();
        foreach (var app in apps)
        {
            tiles.Add(new LauncherTile(app, iconService.GetIcon(app.Path), configuration.Layout.IconSize));
        }
    }

    private async void OnAddApplication(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = localizationService.GetString("AddApplicationTitle", configuration),
            Filter = localizationService.GetString("AddApplicationFilter", configuration)
        };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        configuration.Apps.Add(new LauncherApplication
        {
            DisplayName = GetDisplayName(dialog.FileName),
            Path = dialog.FileName,
            Category = "General"
        });

        await SaveAndRefreshAsync();
    }

    private void OnLaunchApplication(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: Guid id })
        {
            return;
        }

        var app = configuration.Apps.FirstOrDefault(item => item.Id == id);
        if (app is null)
        {
            return;
        }

        if (app.RequiresPin && !VerifyPin())
        {
            MessageBox.Show(this, "The PIN was not accepted.", "SimpleLauncher", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!launcher.TryLaunch(app, out var errorMessage))
        {
            MessageBox.Show(this, errorMessage, "SimpleLauncher", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void OnSettings(object sender, RoutedEventArgs e)
    {
        if (configuration.Security.LockSettings && !VerifyPin())
        {
            MessageBox.Show(this, localizationService.GetString("SettingsProtected", configuration), "SimpleLauncher", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var settings = new SettingsWindow(configuration, configurationStore, backupService, pinHasher, localizationService, paths) { Owner = this };
        if (settings.ShowDialog() == true)
        {
            await SaveAndRefreshAsync();
        }
    }

    private void OnExit(object sender, RoutedEventArgs e)
    {
        if (configuration.Security.IsPinEnabled && !VerifyPin())
        {
            MessageBox.Show(this, "The PIN was not accepted.", "SimpleLauncher", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (Application.Current is App app)
        {
            app.MarkExitToWindows();
        }

        desktopSessionService.StartExplorerIfNeeded();
        Close();
    }

    private void OnSearchChanged(object sender, TextChangedEventArgs e) => RefreshTiles();

    protected override void OnClosed(EventArgs e)
    {
        desktopSessionService.RestoreDesktop();
        base.OnClosed(e);
    }

    private bool VerifyPin()
    {
        if (!configuration.Security.IsPinEnabled || string.IsNullOrWhiteSpace(configuration.Security.PinHash))
        {
            return true;
        }

        var dialog = new PinDialog(localizationService, configuration) { Owner = this };
        return dialog.ShowDialog() == true && pinHasher.Verify(dialog.Pin, configuration.Security.PinHash);
    }

    private async Task SaveAndRefreshAsync()
    {
        await configurationStore.SaveAsync(configuration);
        ApplyBackground();
        RefreshTiles();
    }

    private static string GetDisplayName(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        return string.IsNullOrWhiteSpace(name) ? "Application" : name;
    }

    private sealed record LauncherTile(LauncherApplication App, ImageSource Icon, int IconSize)
    {
        public Guid Id => App.Id;

        public string DisplayName => App.DisplayName;

        public int TileSize => IconSize + 64;

        public int TileHeight => IconSize + 96;
    }
}
