using SimpleLauncher.Models;

namespace SimpleLauncher.Services;

public interface IConfigurationStore
{
    Task<LauncherConfiguration> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(LauncherConfiguration configuration, CancellationToken cancellationToken = default);

    Task<string> ExportAsync(LauncherConfiguration configuration, string destinationPath, CancellationToken cancellationToken = default);

    Task<LauncherConfiguration> ImportAsync(string sourcePath, CancellationToken cancellationToken = default);
}
