using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLauncher.Infrastructure.Security;

namespace SimpleLauncher.Tests;

[TestClass]
public sealed class Pbkdf2PinHasherTests
{
    [TestMethod]
    public void VerifyReturnsTrueForOriginalPin()
    {
        var hasher = new Pbkdf2PinHasher();
        var hash = hasher.Hash("2468");

        Assert.IsTrue(hasher.Verify("2468", hash));
    }

    [TestMethod]
    public void VerifyReturnsFalseForDifferentPin()
    {
        var hasher = new Pbkdf2PinHasher();
        var hash = hasher.Hash("2468");

        Assert.IsFalse(hasher.Verify("1357", hash));
    }
}
