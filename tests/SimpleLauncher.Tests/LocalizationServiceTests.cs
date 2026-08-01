using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLauncher.Infrastructure.Configuration;
using SimpleLauncher.Logging;
using SimpleLauncher.Models;
using SimpleLauncher.Services;

namespace SimpleLauncher.Tests;

[TestClass]
public class LocalizationServiceTests
{
    [TestMethod]
    public void GetString_FallsBackToEnglishWhenLanguageIsNotConfigured()
    {
        var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDirectory);
        var paths = new AppPaths(tempDirectory, Path.Combine(tempDirectory, "launcher.json"), Path.Combine(tempDirectory, "Backups"), Path.Combine(tempDirectory, "Logs"), Path.Combine(tempDirectory, "Logs", "simplelauncher.log"), string.Empty, Path.Combine(tempDirectory, "Assets"), Path.Combine(tempDirectory, "Assets", "translations-cache.json"));
        using var logger = new FileLogger(Path.Combine(tempDirectory, "Logs", "test.log"));
        var service = new LocalizationService(paths, logger);
        var configuration = new LauncherConfiguration();

        var result = service.GetString("Settings", configuration);

        Assert.AreEqual("Settings", result);

        logger.Dispose();
        if (Directory.Exists(tempDirectory))
        {
            Directory.Delete(tempDirectory, true);
        }
    }
}
