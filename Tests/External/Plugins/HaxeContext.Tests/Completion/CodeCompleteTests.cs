using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using HaXeContext.TestUtils;
using NUnit.Framework;

namespace HaXeContext.Completion
{
    class CodeCompleteTests : ASCompleteTests
    {
        internal static string GetFullPath(string fileName) => $"{nameof(HaXeContext)}.Test_Files.completion.{fileName}.hx";

        internal static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

        [TestFixtureSetUp]
        public void Setup() => SetHaxeFeatures(sci);

        static IEnumerable<TestCaseData> IsInterpolationStringStyleTestCases
        {
            get
            {
                yield return new TestCaseData("IsInterpolationStringStyle_1")
                    .Returns(false)
                    .SetName("\"${exp|r}\"");
                yield return new TestCaseData("IsInterpolationStringStyle_2")
                    .Returns(true)
                    .SetName("'${exp|r}'");
                yield return new TestCaseData("IsInterpolationStringStyle_3")
                    .Returns(false)
                    .SetName("'$${exp|r}'");
            }
        }

        [Test, TestCaseSource(nameof(IsInterpolationStringStyleTestCases))]
        public bool IsInterpolationStringStyle(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            sci.Colourise(0, -1);
            return ASContext.Context.CodeComplete.IsStringInterpolationStyle(sci, sci.CurrentPos);
        }

        static IEnumerable<TestCaseData> IsRegexStyleTestCases
        {
            get
            {
                yield return new TestCaseData("IsRegexStyle_1")
                    .Returns(false)
                    .SetName("//|");
                yield return new TestCaseData("IsRegexStyle_2")
                    .Returns(false)
                    .SetName("~/regex/|");
                yield return new TestCaseData("IsRegexStyle_3")
                    .Returns(true)
                    .SetName("~/reg|ex/");
                yield return new TestCaseData("IsRegexStyle_4")
                    .Returns(true)
                    .SetName("~|/regex/");
                yield return new TestCaseData("IsRegexStyle_6")
                    .Returns(true)
                    .SetName("|~/regex/");
                yield return new TestCaseData("IsRegexStyle_5")
                    .Returns(false)
                    .SetName("//~|/regex/");
                yield return new TestCaseData("IsRegexStyle_7")
                    .Returns(false)
                    .SetName("//|~/regex/");
                yield return new TestCaseData("IsRegexStyle_8")
                    .Returns(false)
                    .SetName("/**\n|~/regex/\n**/");
            }
        }

        [Test, TestCaseSource(nameof(IsRegexStyleTestCases))]
        public bool IsRegexStyle(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            sci.Colourise(0, -1);
            return ASContext.Context.CodeComplete.IsRegexStyle(sci, sci.CurrentPos);
        }
    }
}
