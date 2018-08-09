using System.Collections.Generic;
using System.Linq;
using ASCompletion;
using ASCompletion.Context;
using HaXeContext.TestUtils;
using NUnit.Framework;

namespace HaXeContext.Model
{
    [TestFixture]
    class FileParserTests : ASCompletionTests
    {
        [TestFixtureSetUp]
        public void Setup() => SetHaxeFeatures(sci);

        static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

        static string GetFullPath(string fileName) => $"{nameof(HaXeContext)}.Test_Files.parser.{fileName}.hx";

        static IEnumerable<TestCaseData> Issue2302TestCases
        {
            get
            {
                yield return new TestCaseData("Issue2320_1", 1)
                    .SetName("foo(? v)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2320");
                yield return new TestCaseData("Issue2320_2", 1)
                    .SetName("foo(?     v)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2320");
                yield return new TestCaseData("Issue2320_3", 2)
                    .SetName("foo(?\nv1\n,\nv2)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2320");
            }
        }

        [Test, TestCaseSource(nameof(Issue2302TestCases))]
        public int ParseFile_Issue2320(string fileName)
        {
            var model = ASContext.Context.GetCodeModel(ReadAllText(fileName));
            return model.Classes.First().Members.Items.First().Parameters.Count;
        }
    }
}
