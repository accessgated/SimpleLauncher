using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLauncher.Infrastructure.Migration;

namespace SimpleLauncher.Tests;

[TestClass]
public sealed class IniConfigurationMigratorTests
{
    [TestMethod]
    public void ParseMigratesShortcutSections()
    {
        string[] lines =
        [
            "[Shortcut1]",
            "FilePath=\"C:\\Apps\\Game.exe\"",
            "X=100",
            "Y=200",
            "RequiresCode=true"
        ];

        var apps = IniConfigurationMigrator.Parse(lines);

        Assert.AreEqual(1, apps.Count);
        Assert.AreEqual("Game", apps[0].DisplayName);
        Assert.AreEqual(100, apps[0].PositionX);
        Assert.AreEqual(200, apps[0].PositionY);
        Assert.IsTrue(apps[0].RequiresPin);
    }
}
