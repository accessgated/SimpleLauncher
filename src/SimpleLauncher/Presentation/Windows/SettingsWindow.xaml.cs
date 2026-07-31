using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Win32;
using SimpleLauncher.Models;
using SimpleLauncher.Services;

namespace SimpleLauncher.Presentation.Windows;

public partial class SettingsWindow : Window
{
    private readonly LauncherConfiguration configuration;
    private readonly IConfigurationStore configurationStore;
    private readonly ConfigurationBackupService backupService;
    private readonly ObservableCollection<LauncherApplication> apps;

    public SettingsWindow(LauncherConfiguration configuration, IConfigurationStore configurationStore, ConfigurationBackupService backupService)
    {
        this.configuration = configuration;
        this.configurationStore = configurationStore;
        this.backupService = backupService;
        apps = new ObservableCollection<LauncherApplication>(configuration.Apps);

        InitializeComponent();
        AppsList.ItemsSource = apps;
        IconSizeSlider.Value = configuration.Layout.IconSize;
        LockSettingsBox.IsChecked = configuration.Security.LockSettings;
        HideTaskbarBox.IsChecked = configuration.Kiosk.HideTaskbar;
    }

    private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (AppsList.SelectedItem is not LauncherApplication app)
        {
            return;
        }

        NameBox.Text = app.DisplayName;
        CategoryBox.Text = app.Category;
        FavoriteBox.IsChecked = app.IsFavorite;
        RequiresPinBox.IsChecked = app.RequiresPin;
    }

    private void OnApplyApplication(object sender, RoutedEventArgs e)
    {
        if (AppsList.SelectedItem is not LauncherApplication app)
        {
            return;
        }

        app.DisplayName = string.IsNullOrWhiteSpace(NameBox.Text) ? GetDisplayName(app.Path) : NameBox.Text.Trim();
        app.Category = string.IsNullOrWhiteSpace(CategoryBox.Text) ? "General" : CategoryBox.Text.Trim();
        app.IsFavorite = FavoriteBox.IsChecked == true;
        app.RequiresPin = RequiresPinBox.IsChecked == true;
        AppsList.Items.Refresh();
    }

    private void OnRemoveApplication(object sender, RoutedEventArgs e)
    {
        if (AppsList.SelectedItem is LauncherApplication app)
        {
            apps.Remove(app);
        }
    }

    private void OnBackup(object sender, RoutedEventArgs e)
    {
        var backupPath = backupService.Backup();
        MessageBox.Show(this, $"Backup created:\n{backupPath}", "SimpleLauncher", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private async void OnImport(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Import configuration",
            Filter = "JSON configuration|*.json|All files|*.*"
        };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        var imported = await configurationStore.ImportAsync(dialog.FileName);
        apps.Clear();
        foreach (var app in imported.Apps)
        {
            apps.Add(app);
        }

        IconSizeSlider.Value = imported.Layout.IconSize;
        LockSettingsBox.IsChecked = imported.Security.LockSettings;
        HideTaskbarBox.IsChecked = imported.Kiosk.HideTaskbar;
    }

    private async void OnExport(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Title = "Export configuration",
            Filter = "JSON configuration|*.json",
            FileName = "simplelauncher-config.json"
        };

        if (dialog.ShowDialog(this) == true)
        {
            ApplyToConfiguration();
            await configurationStore.ExportAsync(configuration, dialog.FileName);
        }
    }

    private async void OnSave(object sender, RoutedEventArgs e)
    {
        ApplyToConfiguration();
        await configurationStore.SaveAsync(configuration);
        DialogResult = true;
    }

    private void ApplyToConfiguration()
    {
        configuration.Apps = apps.ToList();
        configuration.Layout.IconSize = (int)IconSizeSlider.Value;
        configuration.Security.LockSettings = LockSettingsBox.IsChecked == true;
        configuration.Kiosk.HideTaskbar = HideTaskbarBox.IsChecked == true;
    }

    private static string GetDisplayName(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        return string.IsNullOrWhiteSpace(name) ? "Application" : name;
    }
}
