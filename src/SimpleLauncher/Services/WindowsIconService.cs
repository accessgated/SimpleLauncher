using System.Collections.Concurrent;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using SimpleLauncher.Logging;

namespace SimpleLauncher.Services;

public sealed class WindowsIconService(FileLogger logger)
{
    private readonly ConcurrentDictionary<string, ImageSource> cache = new(StringComparer.OrdinalIgnoreCase);

    public ImageSource GetIcon(string path)
    {
        return cache.GetOrAdd(path, LoadIcon);
    }

    private ImageSource LoadIcon(string path)
    {
        try
        {
            using var icon = Icon.ExtractAssociatedIcon(path);
            if (icon is null)
            {
                return Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Application.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }

            return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
        catch (Exception ex) when (ex is IOException or ArgumentException or UnauthorizedAccessException)
        {
            logger.Error($"Icon loading failed for '{path}'.", ex);
            return Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Application.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
