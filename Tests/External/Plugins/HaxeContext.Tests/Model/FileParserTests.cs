using System.Collections.Generic;
using System.Linq;
using ASCompletion;
using ASCompletion.Context;
using ASCompletion.Model;
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
                yield return new TestCaseData("Issue2320_1")
                    .Returns(1)
                    .SetName("foo(? v)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2320");
                yield return new TestCaseData("Issue2320_2")
                    .Returns(1)
                    .SetName("foo(?     v)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2320");
                yield return new TestCaseData("Issue2320_3")
                    .Returns(2)
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

        static IEnumerable<TestCaseData> Issue2210TestCases
        {
            get
            {
                yield return new TestCaseData("Issue2210_1")
                    .Returns(new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                    {
                        LineFrom = 2,
                        LineTo = 2,
                    })
                    .SetName("function foo() return 1;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_2")
                    .Returns(new MemberModel("bar", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                    {
                        LineFrom = 4,
                        LineTo = 4,
                    })
                    .SetName("function foo() return 1;\nfunction bar() return 1;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_3")
                    .Returns(new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                    {
                        LineFrom = 2,
                        LineTo = 5,
                    })
                    .SetName("function foo(i:Int):Int\nreturn i % 2 == 0\n? 0\n: 1;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
            }
        }

        [Test, TestCaseSource(nameof(Issue2210TestCases))]
        public MemberModel ParseFile_Issue2210(string fileName)
        {
            var model = ASContext.Context.GetCodeModel(ReadAllText(fileName));
            return model.Classes.First().Members.Items.Last();
        }
    }
}
