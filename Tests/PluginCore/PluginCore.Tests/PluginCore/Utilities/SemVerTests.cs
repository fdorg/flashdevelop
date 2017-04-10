// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using NUnit.Framework;
using PluginCore.Utilities;

namespace PluginCore.PluginCore.Utilities
{
    [TestFixture]
    class SemVerTests
    {
        [Test]
        public void IsOlderThan()
        {
            var ver = new SemVer("3.2.1");
            Assert.IsTrue(ver.IsOlderThan(new SemVer("3.3.0")));
            Assert.IsFalse(ver.IsOlderThan(new SemVer("3.0.0")));
        }

        [Test]
        public void Equals()
        {
            var ver = new SemVer("3.3.0");
            Assert.IsTrue(ver.Equals(new SemVer("3.3.0")));
            Assert.IsFalse(ver.Equals(new SemVer("3.2.1")));
        }

        [Test]
        public void IsGreaterThan()
        {
            var ver = new SemVer("3.3.0");
            Assert.IsTrue(ver.IsGreaterThan(new SemVer("3.2.1")));
            Assert.IsFalse(ver.IsGreaterThan(new SemVer("3.3.0")));
        }

        [Test]
        public void IsGreaterThanOrEquals()
        {
            var ver = new SemVer("3.3.0");
            Assert.IsTrue(ver.IsGreaterThanOrEquals(new SemVer("3.2.1")));
            Assert.IsTrue(ver.IsGreaterThanOrEquals(new SemVer("3.3.0")));
            Assert.IsFalse(ver.IsGreaterThanOrEquals(new SemVer("3.4.0")));
        }
    }
}
