using System.Collections.Generic;
using System.IO;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using HaXeContext.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using PluginCore.Controls;

namespace HaXeContext.Completion
{
    class CodeCompleteTests : ASCompleteTests
    {
        internal static string GetFullPath(string fileName) => $"{nameof(HaXeContext)}.Test_Files.completion.{fileName}.hx";

        internal static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

        static readonly string testFilesAssemblyPath = $"\\FlashDevelop\\Bin\\Debug\\{nameof(HaXeContext)}\\Test_Files\\";
        static readonly string testFilesDirectory = $"\\Tests\\External\\Plugins\\{nameof(HaXeContext)}.Tests\\Test Files\\";

        internal static void SetCurrentFile(string fileName)
        {
            fileName = GetFullPath(fileName);
            fileName = Path.GetFileNameWithoutExtension(fileName).Replace('.', Path.DirectorySeparatorChar) + Path.GetExtension(fileName);
            fileName = Path.GetFullPath(fileName);
            fileName = fileName.Replace(testFilesAssemblyPath, testFilesDirectory);
            ASContext.Context.CurrentModel.FileName = fileName;
            PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
        }

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

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2358TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2358_1", '.', false, false)
                    .SetName("(v:Null<Vector<Int>>).| ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2358");
            }
        }

        [
            Test, 
            TestCaseSource(nameof(OnCharIssue2105TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2358TestCases)),
        ]
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

        static IEnumerable<TestCaseData> DeclarationLookupIssue2291TestCases
        {
            get
            {
                yield return new TestCaseData("DeclarationLookupIssue2291_1")
                    .Returns("args")
                    .SetName("function foo(args). Goto Declaration. Issue 2291. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2291");
                yield return new TestCaseData("DeclarationLookupIssue2291_2")
                    .Returns("args")
                    .SetName("function foo(?args). Goto Declaration. Issue 2291. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2291");
            }
        }

        static IEnumerable<TestCaseData> DeclarationLookupIssue366TestCases
        {
            get
            {
                yield return new TestCaseData("DeclarationLookupIssue366_1")
                    .Returns(string.Empty)
                    .SetName("Support for @:publicFields. Goto Declaration. Issue 366. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/366");
                yield return new TestCaseData("DeclarationLookupIssue366_2")
                    .Returns("foo")
                    .SetName("Support for @:publicFields. Goto Declaration. Issue 366. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/366");
            }
        }

        static IEnumerable<TestCaseData> DeclarationLookupIssue2366TestCases
        {
            get
            {
                yield return new TestCaseData("DeclarationLookupIssue2366_1")
                    .Returns(string.Empty)
                    .SetName("tra|ce(''). Issue 2366. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2366");
            }
        }

        [
            Test,
            TestCaseSource(nameof(DeclarationLookupIssue366TestCases)),
            TestCaseSource(nameof(DeclarationLookupIssue2291TestCases)),
            TestCaseSource(nameof(DeclarationLookupIssue2366TestCases)),
        ]
        public string DeclarationLookup(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFile(GetFullPath(fileName));
            ASComplete.DeclarationLookup(sci);
            return sci.SelText;
        }

        static IEnumerable<TestCaseData> CalltipDefIssue2364TestCases
        {
            get
            {
                yield return new TestCaseData("CalltipDef_issue2364_1")
                    .Returns("foo (parameter0:Int, parameter1:Int) : Bool")
                    .SetName("var foo:Int->Int->Bool;\nfoo(|). Case 1");
                yield return new TestCaseData("CalltipDef_issue2364_2")
                    .Returns("foo (parameter0:Int, parameter1:Int) : Bool")
                    .SetName("var foo:Int->Int->Bool;\nfoo(|). Case 2");
            }
        }

        static IEnumerable<TestCaseData> CalltipDefIssue2368TestCases
        {
            get
            {
                yield return new TestCaseData("CalltipDef_issue2368_1")
                    .Returns("foo (v1, v2)")
                    .SetName("foo(1|, 2). Calltip. Issue 2368. Case 1");
                yield return new TestCaseData("CalltipDef_issue2368_2")
                    .Returns("foo (v1, v2)")
                    .SetName("foo(1, 2|). Calltip. Issue 2368. Case 2");
                yield return new TestCaseData("CalltipDef_issue2368_3")
                    .Returns("foo (v1, v2)")
                    .SetName("foo((1 + 1)|, 2). Calltip. Issue 2368. Case 3");
                yield return new TestCaseData("CalltipDef_issue2368_4")
                    .Returns("foo (v1, v2)")
                    .SetName("foo((1 +| 1), 2). Calltip. Issue 2368. Case 4");
                yield return new TestCaseData("CalltipDef_issue2368_5")
                    .Returns("foo (v1, v2)")
                    .SetName("foo(1, 2 * (3 +| 1)). Calltip. Issue 2368. Case 5");
                yield return new TestCaseData("CalltipDef_issue2368_6")
                    .Returns("bar (v1, v2)")
                    .SetName("foo(1, bar(3|, 1)). Calltip. Issue 2368. Case 6");
                yield return new TestCaseData("CalltipDef_issue2368_7")
                    .Returns("bar (v1, v2)")
                    .SetName("foo(1, bar({x|:1}, 1)). Calltip. Issue 2368. Case 7");
                yield return new TestCaseData("CalltipDef_issue2368_8")
                    .Returns("bar_ (v1, v2)")
                    .SetName("foo(1, bar_({x|:1}, 1)). Calltip. Issue 2368. Case 8");
                yield return new TestCaseData("CalltipDef_issue2368_9")
                    .Returns("foo (v1, v2)")
                    .SetName("foo(1, (2 * (3 +| 1))). Calltip. Issue 2368. Case 9");
                yield return new TestCaseData("CalltipDef_issue2368_10")
                    .Returns("foo (v1, v2)")
                    .SetName("foo(1, (2 + ((3 +| 1) * 2))). Calltip. Issue 2368. Case 10");
            }
        }

        static IEnumerable<TestCaseData> CalltipDefIssue2356TestCases
        {
            get
            {
                yield return new TestCaseData("CalltipDef_issue2356_1")
                    .Returns("CalltipDef_issue2356_1_super (v1:Int)")
                    .SetName("super(|). Case 1");
                yield return new TestCaseData("CalltipDef_issue2356_2")
                    .Returns("CalltipDef_issue2356_2_super (v1:Int, v2:Int)")
                    .SetName("super(1, |). Case 2");
            }
        }

        [
            Test,
            TestCaseSource(nameof(CalltipDefIssue2356TestCases)),
            TestCaseSource(nameof(CalltipDefIssue2364TestCases)),
            TestCaseSource(nameof(CalltipDefIssue2368TestCases)),
        ]
        public string CalltipDef(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFile(GetFullPath(fileName));
            ASContext.Context
                .When(it => it.ResolveTopLevelElement(Arg.Any<string>(), Arg.Any<ASResult>()))
                .Do(it =>
                {
                    var topLevel = new FileModel();
                    topLevel.Members.Add(new MemberModel("this", ASContext.Context.CurrentClass.Name, FlagType.Variable, Visibility.Public) {InFile = ASContext.Context.CurrentModel});
                    topLevel.Members.Add(new MemberModel("super", ASContext.Context.CurrentClass.ExtendsType, FlagType.Variable, Visibility.Public) {InFile = ASContext.Context.CurrentModel});
                    var ctx = (ASContext) ASContext.GetLanguageContext("haxe");
                    ctx.TopLevel = topLevel;
                    ctx.ResolveTopLevelElement(it.ArgAt<string>(0), it.ArgAt<ASResult>(1));
                });
            var manager = UITools.Manager;
            ASComplete.HandleFunctionCompletion(sci, false);
            Assert.IsTrue(UITools.CallTip.CallTipActive);
            return ASComplete.calltipDef;
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

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2320TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2320_1", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceTextIssue2320_1"))
                    .SetName("override | ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2320");
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
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2320TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceText_enums_TestCases)),
            //TestCaseSource(nameof(OnCharAndReplaceText_enums2_TestCases)), // TODO: That tests pass without other tests.
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
