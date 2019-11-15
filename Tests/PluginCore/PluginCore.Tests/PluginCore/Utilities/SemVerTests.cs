// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using NUnit.Framework;
using PluginCore.Utilities;

namespace PluginCore.PluginCore.Utilities
{
    [TestFixture]
    class SemVerTests
    {
        [Test]
        public void IsEqualTo()
        {
            var ver = new SemVer("3.3.0");
            Assert.IsFalse(ver == "3.2.1");
            Assert.IsTrue(ver == "3.3.0");
        }

        [Test]
        public void IsNotEqualTo()
        {
            var ver = new SemVer("3.3.0");
            Assert.IsTrue(ver != "3.2.1");
            Assert.IsFalse(ver != "3.3.0");
        }

        [Test]
        public void IsLessThan()
        {
            var ver = new SemVer("3.2.1");
            Assert.IsFalse(ver < "3.0.0");
            Assert.IsTrue(ver < "3.3.0");
        }

        [Test]
        public void IsLessThanOrEqualTo()
        {
            var ver = new SemVer("3.3.0");
            Assert.IsFalse(ver <= "3.2.1");
            Assert.IsTrue(ver <= "3.3.0");
            Assert.IsTrue(ver <= "3.4.0");
        }

        [Test]
        public void IsGreaterThan()
        {
            var ver = new SemVer("3.3.0");
            Assert.IsTrue(ver > "3.2.1");
            Assert.IsFalse(ver > "3.3.0");
        }

        [Test]
        public void IsGreaterThanOrEquals()
        {
            var ver = new SemVer("3.3.0");
            Assert.IsTrue(ver >= "3.2.1");
            Assert.IsTrue(ver >= "3.3.0");
            Assert.IsFalse(ver >= "3.4.0");
        }
    }
}
