using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using HaXeContext.TestUtils;
using NSubstitute;
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
                    .SetName("new Foo(|. class with super constructor")
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

        static IEnumerable<TestCaseData> GetToolTipTextTestCases
        {
            get
            {
                yield return new TestCaseData("GetToolTipText_1")
                    .SetName("new B|(). Case 1. Class without constructor")
                    .Returns(null);
                yield return new TestCaseData("GetToolTipText_2")
                    .SetName("new B|(). Case 2. Class with explicit constructor")
                    .Returns("public Bar (v:Int)\n[COLOR=Black]in Bar[/COLOR]");
                yield return new TestCaseData("GetToolTipText_3")
                    .SetName("new B|(). Case 3. Class with implicit constructor")
                    .Returns("public Bar (v:Int)\n[COLOR=Black]in Foo[/COLOR]");
            }
        }

        [Test, TestCaseSource(nameof(GetToolTipTextTestCases))]
        public string GetToolTipText(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            var expr = ASComplete.GetExpressionType(sci, sci.CurrentPos, false, true);
            return ASComplete.GetToolTipText(expr);
        }
    }

    class CodeCompleteTests2 : ASCompleteTests
    {
        [TestFixtureSetUp]
        public void Setup() => SetHaxeFeatures(sci);

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2134TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2134_1", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceTextIssue2134_1"))
                    .SetName("override | ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2134");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_1", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_1"))
                    .SetName("[].| ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2134");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_2", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_2"))
                    .SetName("'${[].| }'")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2134");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_3", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_3"))
                    .SetName("[[].| ]")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2134");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_4", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_4"))
                    .SetName("''.| ");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_5", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_5"))
                    .Ignore("")
                    .SetName("'${\"123\".| }'");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_6", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_6"))
                    .SetName("'${String.| }'");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_7", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_7"))
                    .SetName("'${String.fromCharCode(1).| }'");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_8", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_8"))
                    .SetName("'${[1 => 1].| }'");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_9", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_9"))
                    .SetName("cast(v, String).| ");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_10", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_10"))
                    .SetName("import | ");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceText_enums_TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_enums_1", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_enums_1"))
                    .SetName("EnumType.| ");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_enums_2", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_enums_2"))
                    .Ignore("Need support for `haxe.EnumValueTools`")
                    .SetName("EnumType.EnumInstance.| ");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_enums_3", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_enums_3"))
                    .Ignore("Need support for `haxe.EnumValueTools`")
                    .SetName("EnumInstance.| ");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_enums_4", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_enums_4"))
                    .SetName("EnumAbstractType.| ");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_enums_5", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_enums_5"))
                    .SetName("EnumAbstractType.EnumAbstractInstance.| ");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_enums_6", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_enums_6"))
                    .SetName("EnumAbstractInstance.| ");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_enums_7", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_enums_7"))
                    .SetName("EnumAbstractVariable.| ");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceText_enums2_TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_enums_8", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_enums_8"))
                    .SetName("EnumAbstractType.| . case 2");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_enums_9", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_enums_9"))
                    .SetName("EnumAbstractType.EnumAbstractInstance.| . case 2");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_enums_10", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_enums_10"))
                    .SetName("EnumAbstractInstance.| . case 3");
            }
        }

        [
            Test,
            TestCaseSource(nameof(OnCharAndReplaceTextTestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2134TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceText_enums_TestCases)),
            // TODO: That tests pass without other tests.
            //TestCaseSource(nameof(OnCharAndReplaceText_enums2_TestCases)),
        ]
        public string OnCharAndReplaceText(string fileName, char addedChar, bool autoHide)
        {
            ASContext.Context.ResolveDotContext(null, null, false).ReturnsForAnyArgs(it => null);
            //{TODO slavara: quick hack
            ASContext.Context.When(it => it.ResolveTopLevelElement(Arg.Any<string>(), Arg.Any<ASResult>()))
                .Do(it =>
                {
                    var ctx = (Context) ASContext.GetLanguageContext("haxe");
                    ctx.GetCodeModel(ctx.CurrentModel, sci.Text);
                    ctx.completionCache.IsDirty = true;
                    ctx.ResolveTopLevelElement(it.ArgAt<string>(0), it.ArgAt<ASResult>(1));
                });
            //}
            ((Context) ASContext.GetLanguageContext("haxe")).completionCache.IsDirty = true;
            return OnCharAndReplaceText(sci, CodeCompleteTests.ReadAllText(fileName), addedChar, autoHide);
        }
    }

    class CodeCompleteTests3 : ASCompleteTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            SetHaxeFeatures(sci);
            ((HaXeSettings) ASContext.GetLanguageContext("haxe").Settings).DisableCompletionOnDemand = false;
        }

        static IEnumerable<TestCaseData> OnCharIssue825TestCases
        {
            get
            {
                yield return new TestCaseData("OnCharIssue2105_1", '.', true)
                    .SetName("'.|' Issue825. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("OnCharIssue2105_2", '.', true)
                    .SetName("\".|\" Issue825. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_1", '.', true)
                    .SetName("[].| ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_2", '.', true)
                    .SetName("'${[].| }'")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_3", '.', true)
                    .SetName("[[].| ]")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_4", '.', true)
                    .SetName("''.| ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_5", '.', true)
                    .SetName("'${\"123\".| }'")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_6", '.', true)
                    .SetName("'${String.| }'")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_7", '.', true)
                    .SetName("'${String.fromCharCode(1).| }'")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_8", '.', true)
                    .SetName("'${[1 => 1].| }'")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_9", '.', true)
                    .SetName("cast(v, String).| ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_14", ' ', true)
                    .SetName("from | ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_15", ' ', true)
                    .SetName("to | ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_16", ' ', true)
                    .SetName("public | ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_enums_1", '.', true)
                    .SetName("EnumType.| ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_enums_4", '.', true)
                    .SetName("EnumAbstractType.| ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_enums_5", '.', true)
                    .SetName("EnumAbstractType.EnumAbstractInstance.| ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_enums_6", '.', true)
                    .SetName("EnumAbstractInstance.| ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_enums_7", '.', true)
                    .SetName("EnumAbstractVariable.| ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
            }
        }

        static IEnumerable<TestCaseData> OnCharIssue825TestCases2
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_10", ' ', true)
                    .SetName("import | ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_11", ' ', true)
                    .SetName("new | ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_12", ' ', true)
                    .SetName("extends | ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_13", ' ', true)
                    .SetName("implements | ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
                yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2134_1", ' ', true)
                    .SetName("override | ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/825");
            }
        }

        [
            Test,
            TestCaseSource(nameof(OnCharIssue825TestCases)),
            // TODO: That tests pass without other tests
            //TestCaseSource(nameof(OnCharIssue825TestCases2)),
        ]
        public void OnChar(string fileName, char addedChar, bool autoHide) => OnChar(sci, CodeCompleteTests.ReadAllText(fileName), addedChar, autoHide, false);
    }
}
