namespace SimpleLauncher.Models;

/// <summary>
/// Versioned JSON configuration for SimpleLauncher.
/// </summary>
public sealed class LauncherConfiguration
{
    public int Version { get; set; } = 1;

    public ThemeSettings Theme { get; set; } = new();

    public LayoutSettings Layout { get; set; } = new();

    public SecuritySettings Security { get; set; } = new();

    public KioskSettings Kiosk { get; set; } = new();

    public StartupSettings Startup { get; set; } = new();

    public List<LauncherApplication> Apps { get; set; } = [];
}

public sealed class ThemeSettings
{
    public string Mode { get; set; } = "Dark";

    public string? BackgroundImagePath { get; set; }

    public string Language { get; set; } = "en-US";
}

public sealed class LayoutSettings
{
    public int IconSize { get; set; } = 96;

    public string SortMode { get; set; } = "Manual";

    public bool ShowFavoritesFirst { get; set; } = true;
}

public sealed class SecuritySettings
{
    public bool IsPinEnabled { get; set; }

    public string? PinHash { get; set; }

    public bool LockSettings { get; set; } = true;
}

public sealed class KioskSettings
{
    public bool HideTaskbar { get; set; } = true;

    public bool BlockWindowsKeys { get; set; }

    public bool TopMost { get; set; } = true;
}

public sealed class StartupSettings
{
    public bool StartWithWindows { get; set; }

    public bool ReplaceShell { get; set; }
}
