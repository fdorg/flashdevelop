// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using NUnit.Framework;

namespace PluginCore.FRService
{
    [TestFixture]
    class FRSearchTests
    {
        [Test]
        public void Issue2432()
        {
            var search = new FRSearch("value");
            search.WholeWord = true;
            search.NoCase = true;
            search.Filter = SearchFilter.OutsideCodeComments | SearchFilter.OutsideStringLiterals;
            var input = "var value;//value\n" +
                        "value = 'value';\n" +
                        "value = \"\\\\\";\n" +
                        "trace(value);\n";
            var matches = search.Matches(input);
            Assert.AreEqual(4, matches.Count);
            input = "var value;//value\n" +
                        "value = 'value';\n" +
                        "value = '\\\\';\n" +
                        "trace(value);\n";
            matches = search.Matches(input);
            Assert.AreEqual(4, matches.Count);
        }
    }
}
