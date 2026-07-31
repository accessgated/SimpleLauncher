namespace SimpleLauncher.Infrastructure.Security;

/// <summary>
/// Validates user-selected filesystem paths before launching or importing them.
/// </summary>
public sealed class ApplicationPathValidator
{
    private static readonly HashSet<string> AllowedExecutableExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".exe",
        ".lnk",
        ".bat",
        ".cmd",
        ".ps1",
        ".msc"
    };

    public bool IsLaunchablePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        var fullPath = Path.GetFullPath(path.Trim().Trim('"'));
        if (Directory.Exists(fullPath))
        {
            return true;
        }

        return File.Exists(fullPath) && AllowedExecutableExtensions.Contains(Path.GetExtension(fullPath));
    }

    public void EnsureReadableFile(string path)
    {
        var fullPath = Path.GetFullPath(path);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("The file does not exist.", fullPath);
        }

        using var _ = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }
}
