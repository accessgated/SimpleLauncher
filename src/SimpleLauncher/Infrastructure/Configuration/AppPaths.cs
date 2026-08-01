namespace SimpleLauncher.Infrastructure.Configuration;

/// <summary>
/// Centralizes filesystem locations used by the application.
/// </summary>
public sealed record AppPaths(
    string ConfigurationDirectory,
    string ConfigurationFilePath,
    string BackupDirectory,
    string LogDirectory,
    string LogFilePath,
    string LegacyIniPath,
    string AssetsDirectory,
    string TranslationCacheFilePath)
{
    public static AppPaths CreateDefault()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var baseDirectory = AppContext.BaseDirectory;
        var configurationDirectory = Path.Combine(appData, "SimpleLauncher");
        var logDirectory = Path.Combine(configurationDirectory, "Logs");
        var assetsDirectory = Path.Combine(configurationDirectory, "Assets");
        return new AppPaths(
            configurationDirectory,
            Path.Combine(configurationDirectory, "launcher.json"),
            Path.Combine(configurationDirectory, "Backups"),
            logDirectory,
            Path.Combine(logDirectory, "simplelauncher.log"),
            Path.Combine(baseDirectory, "Shortcuts.ini"),
            assetsDirectory,
            Path.Combine(assetsDirectory, "translations-cache.json"));
    }
}
