using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using HaXeContext.TestUtils;
using NUnit.Framework;

namespace HaXeContext.Completion
{
    class CodeCompleteTests : ASCompleteTests
    {
        static string GetFullPath(string fileName) => $"{nameof(HaXeContext)}.Test_Files.completion.{fileName}.hx";

        static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

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
            return ASContext.Context.CodeComplete.IsRegexStyle(sci, sci.CurrentPos);
        }

        static IEnumerable<TestCaseData> PositionIsBeforeBodyTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeBody_1")
                    .SetName("|class Some {}")
                    .Returns(true);
                yield return new TestCaseData("BeforeBody_2")
                    .SetName("class Some {|}")
                    .Returns(false);
                yield return new TestCaseData("BeforeBody_3")
                    .SetName("class Some | extends Foo<{v:Int}> {}")
                    .Returns(true);
                yield return new TestCaseData("BeforeBody_4")
                    .SetName("class Some {}|")
                    .Returns(false);
            }
        }

        [Test, TestCaseSource(nameof(PositionIsBeforeBodyTestCases))]
        public bool PositionIsBeforeBody(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            return ASContext.Context.CodeComplete.PositionIsBeforeBody(sci, sci.CurrentPos, ASContext.Context.CurrentClass);
        }

        static IEnumerable<TestCaseData> Issue2053TestCases
        {
            get
            {
                yield return new TestCaseData("Issue2053_1")
                    .Returns(true)
                    .SetName("new Foo(|. class with constructor")
                    .SetDescription("https://github.com/fdorg/flashdevelop/pull/2055");
                yield return new TestCaseData("Issue2053_2")
                    .Returns(false)
                    .SetName("new Foo(|. class without constructor")
                    .SetDescription("https://github.com/fdorg/flashdevelop/pull/2055");
                yield return new TestCaseData("Issue2053_3")
                    .Returns(true)
                    .SetName("new Foo(|. class with superconstructor")
                    .SetDescription("https://github.com/fdorg/flashdevelop/pull/2055");
                yield return new TestCaseData("Issue2053_4")
                    .Returns(true)
                    .SetName("new Foo(|. typedef = class with constructor")
                    .SetDescription("https://github.com/fdorg/flashdevelop/pull/2055");
                yield return new TestCaseData("Issue2053_5")
                    .Returns(true)
                    .SetName("new Foo(|. abstract with constructor")
                    .SetDescription("https://github.com/fdorg/flashdevelop/pull/2055");
                yield return new TestCaseData("Issue2053_6")
                    .Returns(false)
                    .SetName("new Foo(|. abstract without constructor")
                    .SetDescription("https://github.com/fdorg/flashdevelop/pull/2055");
                yield return new TestCaseData("Issue2053_7")
                    .Returns(true)
                    .SetName("new Foo(|. typedef = abstract with constructor")
                    .SetDescription("https://github.com/fdorg/flashdevelop/pull/2055");
            }
        }

        [Test, TestCaseSource(nameof(Issue2053TestCases))]
        public bool ResolveFunction(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            return ASContext.Context.CodeComplete.ResolveFunction(sci, sci.CurrentPos - 1, true);
        }

        static IEnumerable<TestCaseData> OnCharIssue2105TestCases
        {
            get
            {
                yield return new TestCaseData("OnCharIssue2105_1", '.', false, false)
                    .SetName("'.|' Issue2105. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2105");
                yield return new TestCaseData("OnCharIssue2105_2", '.', false, false)
                    .SetName("\".|\" Issue2105. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2105");
            }
        }

        [Test, TestCaseSource(nameof(OnCharIssue2105TestCases))]
        public void OnChar(string fileName, char addedChar, bool autoHide, bool hasCompletion) => OnChar(sci, ReadAllText(fileName), addedChar, autoHide, hasCompletion);

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2134TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2134_1", ' ', false)
                    .Returns(ReadAllText("AfterOnCharAndReplaceTextIssue2134_1"))
                    .SetName("override | ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2134");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_1", '.', false)
                    .Returns(ReadAllText("AfterOnCharAndReplaceText_1"))
                    .SetName("[].| ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2134");
            }
        }

        [
            Test,
            //TestCaseSource(nameof(OnCharAndReplaceTextIssue2134TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextTestCases)),
        ]
        public string OnCharAndReplaceText(string fileName, char addedChar, bool autoHide)
        {
            var ctx = ((Context) ASContext.GetLanguageContext("haxe"));
            ((HaXeSettings) ctx.Settings).CompletionMode = HaxeCompletionModeEnum.FlashDevelop;
            ctx.completionCache.IsDirty = true;
            return OnCharAndReplaceText(sci, ReadAllText(fileName), addedChar, autoHide);
        }
    }
}
