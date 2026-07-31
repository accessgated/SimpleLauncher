namespace SimpleLauncher.Models;

/// <summary>
/// A launchable Windows application, folder, or shortcut.
/// </summary>
public sealed class LauncherApplication
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string DisplayName { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public string? Arguments { get; set; }

    public string Category { get; set; } = "General";

    public bool IsFavorite { get; set; }

    public bool RequiresPin { get; set; }

    public int PositionX { get; set; }

    public int PositionY { get; set; }
}
