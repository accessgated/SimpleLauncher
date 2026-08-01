using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
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
    private readonly WindowsShellService shellService;
    private readonly ConfigurationBackupService backupService;
    private readonly Pbkdf2PinHasher pinHasher;
    private readonly FileLogger logger;
    private readonly ObservableCollection<LauncherTile> tiles = [];

    public MainWindow(
        LauncherConfiguration configuration,
        IConfigurationStore configurationStore,
        ProcessLauncher launcher,
        WindowsIconService iconService,
        WindowsShellService shellService,
        ConfigurationBackupService backupService,
        Pbkdf2PinHasher pinHasher,
        FileLogger logger)
    {
        this.configuration = configuration;
        this.configurationStore = configurationStore;
        this.launcher = launcher;
        this.iconService = iconService;
        this.shellService = shellService;
        this.backupService = backupService;
        this.pinHasher = pinHasher;
        this.logger = logger;

        InitializeComponent();
        Topmost = configuration.Kiosk.TopMost;
        AppsList.ItemsSource = tiles;
        ApplyBackground();
        RefreshTiles();

        if (configuration.Kiosk.HideTaskbar)
        {
            shellService.HideTaskbar();
        }
    }

    private void ApplyBackground()
    {
        var backgroundPath = configuration.Theme.BackgroundImagePath;
        if (!string.IsNullOrWhiteSpace(backgroundPath) && File.Exists(backgroundPath))
        {
            BackgroundImage.Source = new BitmapImage(new Uri(backgroundPath));
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
            Title = "Add application",
            Filter = "Applications and shortcuts|*.exe;*.lnk;*.bat;*.cmd;*.ps1;*.msc|All files|*.*"
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
            MessageBox.Show(this, "Settings are protected.", "SimpleLauncher", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var settings = new SettingsWindow(configuration, configurationStore, backupService, pinHasher) { Owner = this };
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

        Close();
    }

    private void OnSearchChanged(object sender, TextChangedEventArgs e) => RefreshTiles();

    protected override void OnClosed(EventArgs e)
    {
        shellService.RestoreDesktop();
        base.OnClosed(e);
    }

    private bool VerifyPin()
    {
        if (!configuration.Security.IsPinEnabled || string.IsNullOrWhiteSpace(configuration.Security.PinHash))
        {
            return true;
        }

        var dialog = new PinDialog { Owner = this };
        return dialog.ShowDialog() == true && pinHasher.Verify(dialog.Pin, configuration.Security.PinHash);
    }

    private async Task SaveAndRefreshAsync()
    {
        await configurationStore.SaveAsync(configuration);
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
