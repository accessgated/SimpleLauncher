using SimpleLauncher.Infrastructure.Configuration;
using SimpleLauncher.Logging;

namespace SimpleLauncher.Services;

public sealed class ConfigurationBackupService(AppPaths paths, FileLogger logger)
{
    public string Backup()
    {
        Directory.CreateDirectory(paths.BackupDirectory);
        var backupPath = Path.Combine(paths.BackupDirectory, $"launcher.{DateTimeOffset.UtcNow:yyyyMMddHHmmss}.json");

        if (File.Exists(paths.ConfigurationFilePath))
        {
            File.Copy(paths.ConfigurationFilePath, backupPath, overwrite: false);
            logger.Info($"Configuration backup created: {backupPath}");
        }

        return backupPath;
    }
}
