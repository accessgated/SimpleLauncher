using SimpleLauncher.Infrastructure.Configuration;
using SimpleLauncher.Logging;
using SimpleLauncher.Models;

namespace SimpleLauncher.Infrastructure.Migration;

/// <summary>
/// Migrates the prototype Shortcuts.ini format to versioned JSON.
/// </summary>
public sealed class IniConfigurationMigrator(AppPaths paths, FileLogger logger)
{
    public Task<LauncherConfiguration?> TryMigrateAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!File.Exists(paths.LegacyIniPath))
        {
            return Task.FromResult<LauncherConfiguration?>(null);
        }

        try
        {
            var apps = Parse(File.ReadAllLines(paths.LegacyIniPath));
            if (apps.Count == 0)
            {
                return Task.FromResult<LauncherConfiguration?>(null);
            }

            logger.Info($"Migrated {apps.Count} application(s) from legacy INI configuration.");
            return Task.FromResult<LauncherConfiguration?>(new LauncherConfiguration { Apps = apps });
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or FormatException)
        {
            logger.Error("Legacy INI migration failed.", ex);
            return Task.FromResult<LauncherConfiguration?>(null);
        }
    }

    public static List<LauncherApplication> Parse(IEnumerable<string> lines)
    {
        var apps = new List<LauncherApplication>();
        Dictionary<string, string>? section = null;

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith(';') || line.StartsWith('#'))
            {
                continue;
            }

            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                AddSection(section, apps);
                section = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                continue;
            }

            var separator = line.IndexOf('=');
            if (separator <= 0 || section is null)
            {
                continue;
            }

            section[line[..separator].Trim()] = line[(separator + 1)..].Trim().Trim('"');
        }

        AddSection(section, apps);
        return apps;
    }

    private static void AddSection(Dictionary<string, string>? section, List<LauncherApplication> apps)
    {
        if (section is null || !section.TryGetValue("FilePath", out var filePath) || string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }

        apps.Add(new LauncherApplication
        {
            DisplayName = GetDisplayName(filePath),
            Path = filePath,
            PositionX = ReadInt(section, "X"),
            PositionY = ReadInt(section, "Y"),
            RequiresPin = ReadBool(section, "RequiresCode")
        });
    }

    private static int ReadInt(IReadOnlyDictionary<string, string> section, string key)
        => section.TryGetValue(key, out var value) && int.TryParse(value, out var parsed) ? parsed : 0;

    private static bool ReadBool(IReadOnlyDictionary<string, string> section, string key)
        => section.TryGetValue(key, out var value) && bool.TryParse(value, out var parsed) && parsed;

    private static string GetDisplayName(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        return string.IsNullOrWhiteSpace(name) ? "Application" : name;
    }
}
