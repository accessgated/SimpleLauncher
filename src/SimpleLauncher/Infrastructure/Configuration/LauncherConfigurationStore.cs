using System.Text.Json;
using SimpleLauncher.Infrastructure.Migration;
using SimpleLauncher.Infrastructure.Security;
using SimpleLauncher.Logging;
using SimpleLauncher.Models;
using SimpleLauncher.Services;

namespace SimpleLauncher.Infrastructure.Configuration;

/// <summary>
/// Reads and writes versioned launcher configuration.
/// </summary>
public sealed class LauncherConfigurationStore(
    AppPaths paths,
    FileLogger logger,
    IniConfigurationMigrator migrator,
    ApplicationPathValidator validator,
    Pbkdf2PinHasher pinHasher) : IConfigurationStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    public async Task<LauncherConfiguration> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (File.Exists(paths.ConfigurationFilePath))
        {
            try
            {
                await using var stream = File.OpenRead(paths.ConfigurationFilePath);
                var configuration = await JsonSerializer.DeserializeAsync<LauncherConfiguration>(stream, SerializerOptions, cancellationToken);
                return Normalize(configuration ?? CreateDefaultConfiguration());
            }
            catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException)
            {
                logger.Error("Configuration was invalid. A safe default configuration will be used.", ex);
                await BackupCorruptedConfigurationAsync(cancellationToken);
                return CreateDefaultConfiguration();
            }
        }

        var migrated = await migrator.TryMigrateAsync(cancellationToken);
        var initial = Normalize(migrated ?? CreateDefaultConfiguration());
        await SaveAsync(initial, cancellationToken);
        return initial;
    }

    public async Task SaveAsync(LauncherConfiguration configuration, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(paths.ConfigurationDirectory);
        var normalized = Normalize(configuration);
        var tempFile = paths.ConfigurationFilePath + ".tmp";
        await using (var stream = File.Create(tempFile))
        {
            await JsonSerializer.SerializeAsync(stream, normalized, SerializerOptions, cancellationToken);
        }

        File.Move(tempFile, paths.ConfigurationFilePath, true);
    }

    public async Task<string> ExportAsync(LauncherConfiguration configuration, string destinationPath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.GetFullPath(destinationPath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? paths.ConfigurationDirectory);
        await using var stream = File.Create(fullPath);
        await JsonSerializer.SerializeAsync(stream, Normalize(configuration), SerializerOptions, cancellationToken);
        return fullPath;
    }

    public async Task<LauncherConfiguration> ImportAsync(string sourcePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.GetFullPath(sourcePath);
        validator.EnsureReadableFile(fullPath);
        await using var stream = File.OpenRead(fullPath);
        var configuration = await JsonSerializer.DeserializeAsync<LauncherConfiguration>(stream, SerializerOptions, cancellationToken);
        return Normalize(configuration ?? throw new InvalidOperationException("The selected configuration file is empty."));
    }

    private LauncherConfiguration Normalize(LauncherConfiguration configuration)
    {
        configuration.Version = Math.Max(configuration.Version, 1);
        configuration.Theme ??= new ThemeSettings();
        configuration.Layout ??= new LayoutSettings();
        configuration.Security ??= new SecuritySettings();
        configuration.Kiosk ??= new KioskSettings();
        configuration.Startup ??= new StartupSettings();
        configuration.Apps ??= [];

        configuration.Layout.IconSize = Math.Clamp(configuration.Layout.IconSize, 48, 192);

        foreach (var app in configuration.Apps)
        {
            app.Id = app.Id == Guid.Empty ? Guid.NewGuid() : app.Id;
            app.DisplayName = string.IsNullOrWhiteSpace(app.DisplayName) ? GetDisplayName(app.Path) : app.DisplayName.Trim();
            app.Category = string.IsNullOrWhiteSpace(app.Category) ? "General" : app.Category.Trim();
            app.Path = app.Path.Trim().Trim('"');
        }

        return configuration;
    }

    private LauncherConfiguration CreateDefaultConfiguration()
    {
        return new LauncherConfiguration
        {
            Theme = new ThemeSettings
            {
                Mode = "Dark",
                BackgroundImagePath = Path.Combine(AppContext.BaseDirectory, "Assets", "Background.jpg")
            },
            Security = new SecuritySettings
            {
                IsPinEnabled = true,
                PinHash = pinHasher.Hash("1234"),
                LockSettings = true
            }
        };
    }

    private static string GetDisplayName(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        return string.IsNullOrWhiteSpace(name) ? "Application" : name;
    }

    private async Task BackupCorruptedConfigurationAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(paths.ConfigurationFilePath))
        {
            return;
        }

        Directory.CreateDirectory(paths.BackupDirectory);
        var backupPath = Path.Combine(paths.BackupDirectory, $"launcher.corrupt.{DateTimeOffset.UtcNow:yyyyMMddHHmmss}.json");
        await using var source = File.OpenRead(paths.ConfigurationFilePath);
        await using var destination = File.Create(backupPath);
        await source.CopyToAsync(destination, cancellationToken);
    }
}
