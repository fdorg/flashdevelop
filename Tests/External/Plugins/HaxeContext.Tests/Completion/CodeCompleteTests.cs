using System.Collections.Generic;
using System.IO;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Settings;
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

        static IEnumerable<TestCaseData> OnCharIssue2358TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2358_1", '.', false, true)
                    .SetName("(v:Null<Vector<Int>>).| Issue 2358. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2358");
                yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2358_2", '.', false, true)
                    .SetName("(v:Null<Vector<Int>>).| Issue 2358. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2358");
                yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2358_3", '.', false, true)
                    .SetName("(v:Null<Vector<Int>>).| Issue 2358. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2358");
                yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2358_4", '.', false, true)
                    .SetName("(v:Null<Vector<Int>>).| Issue 2358. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2358");
            }
        }

        static IEnumerable<TestCaseData> OnCharIssue2396TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2396_1", '.', false, true)
                    .SetName("localCallback().| Issue 2396. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2396");
                yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2396_2", '.', false, true)
                    .SetName("parameterCallback().| Issue 2396. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2396");
                yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2396_3", '.', false, true)
                    .SetName("localFunction().| Issue 2396. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2396");
                yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2396_4", '.', false, true)
                    .SetName("localCallback().| Issue 2396. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2396");
            }
        }

        static IEnumerable<TestCaseData> OnCharIssue589TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnChar_issue589_1", ' ', false, true)
                    .SetName("case <complete> Issue 589. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
                yield return new TestCaseData("BeforeOnChar_issue589_2", ' ', false, true)
                    .SetName("case <complete> Issue 589. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
                yield return new TestCaseData("BeforeOnChar_issue589_3", ' ', false, true)
                    .SetName("case <complete> Issue 589. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
                yield return new TestCaseData("BeforeOnChar_issue589_4", ' ', false, false)
                    .SetName("case <complete> Issue 589. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
                yield return new TestCaseData("BeforeOnChar_issue589_5", ' ', false, false)
                    .SetName("case <complete> Issue 589. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
                yield return new TestCaseData("BeforeOnChar_issue589_6", ' ', false, true)
                    .SetName("case EnumValue0 | <complete> Issue 589. Case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
                yield return new TestCaseData("BeforeOnChar_issue589_7", ' ', false, true)
                    .SetName("case EnumValue0, <complete> Issue 589. Case 7")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
                yield return new TestCaseData("BeforeOnChar_issue589_8", ' ', false, false)
                    .SetName("trace(1 | <complete>) Issue 589. Case 8")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
                yield return new TestCaseData("BeforeOnChar_issue589_9", ' ', false, false)
                    .SetName("trace(1 , <complete>) Issue 589. Case 9")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
                yield return new TestCaseData("BeforeOnChar_issue589_10", ' ', false, false)
                    .SetName("case Node(1 | <complete>) Issue 589. Case 10")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
                yield return new TestCaseData("BeforeOnChar_issue589_11", ' ', false, false)
                    .SetName("case Node(1 , <complete>) Issue 589. Case 11")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
                yield return new TestCaseData("BeforeOnChar_issue589_12", ' ', false, false)
                    .SetName("case [_, <complete>] Issue 589. Case 12")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
                yield return new TestCaseData("BeforeOnChar_issue589_13", ' ', false, true)
                    .SetName("case EnumValue0 | EnumValue1 | <complete> Issue 589. Case 13")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
                yield return new TestCaseData("BeforeOnChar_issue589_14", ' ', false, true)
                    .SetName("case EnumValue0, EnumValue1, <complete> Issue 589. Case 14")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
            }
        }

        static IEnumerable<TestCaseData> OnCharIssue2483TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnChar_issue2483_1", '>', false, true)
                    .SetName("var v:Any-><complete> Issue 2483. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2483");
                yield return new TestCaseData("BeforeOnChar_issue2483_2", '>', false, false)
                    .SetName("(a, b) -><complete> Issue 2483. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2483");
                yield return new TestCaseData("BeforeOnChar_issue2483_3", '>', false, false)
                    .SetName("a ><complete> Issue 2483. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2483");
                yield return new TestCaseData("BeforeOnChar_issue2483_4", '>', false, false)
                    .SetName("Array<Int><complete> Issue 2483. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2483");
                yield return new TestCaseData("BeforeOnChar_issue2483_5", '(', false, true)
                    .SetName("var a:Any->(<complete> Issue 2483. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2483");
            }
        }

        static IEnumerable<TestCaseData> OnCharIssue2497TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnChar_issue2497_1", ' ', false, false)
                    .SetName("(v:EnumValue) += <complete> Issue 2497. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2497");
            }
        }

        static IEnumerable<TestCaseData> OnCharIssue2518TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnChar_issue2518_1", ' ', false, false)
                    .SetName("(v:Void->Void) == <complete> Issue 2518. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2518");
                yield return new TestCaseData("BeforeOnChar_issue2518_2", ' ', false, false)
                    .SetName("(v:Void->Void) != <complete> Issue 2518. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2518");
                yield return new TestCaseData("BeforeOnChar_issue2518_3", ' ', false, false)
                    .SetName("(v:Void->Void) += <complete> Issue 2518. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2518");
            }
        }

        static IEnumerable<TestCaseData> OnCharIssue2522TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnChar_issue2522_1", ' ', false, false)
                    .SetName("function(v:Int == <complete> Issue 2522. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2522");
            }
        }

        static IEnumerable<TestCaseData> OnCharIssue2526TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnChar_issue2526_1", ' ', false, false)
                    .SetName("@:forward <complete> Issue 2526. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2526");
                yield return new TestCaseData("BeforeOnChar_issue2526_2", ' ', false, false)
                    .SetName("@:forward(<complete> . Before class. Issue 2526. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2526");
                yield return new TestCaseData("BeforeOnChar_issue2526_3", ' ', false, false)
                    .SetName("@:forward(<complete> . Before enum. Issue 2526. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2526");
                yield return new TestCaseData("BeforeOnChar_issue2526_4", ' ', false, false)
                    .SetName("@:forward(<complete> . Before interface. Issue 2526. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2526");
                yield return new TestCaseData("BeforeOnChar_issue2526_5", ' ', false, false)
                    .SetName("@:forward(<complete> . Before typedef. Issue 2526. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2526");
                yield return new TestCaseData("BeforeOnChar_issue2526_6", ' ', false, false)
                    .SetName("@:forward() <complete> Issue 2526. Case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2526");
                yield return new TestCaseData("BeforeOnChar_issue2526_7", ' ', false, false)
                    .SetName("@:forwardStatics <complete> Issue 2526. Case 7")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2526");
                yield return new TestCaseData("BeforeOnChar_issue2526_8", ' ', false, false)
                    .SetName("@:forwardStatics(<complete> . Before class. Issue 2526. Case 8")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2526");
                yield return new TestCaseData("BeforeOnChar_issue2526_9", ' ', false, false)
                    .SetName("@:forwardStatics(<complete> . Before enum. Issue 2526. Case 9")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2526");
                yield return new TestCaseData("BeforeOnChar_issue2526_10", ' ', false, false)
                    .SetName("@:forwardStatics(<complete> . Before interface. Issue 2526. Case 10")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2526");
                yield return new TestCaseData("BeforeOnChar_issue2526_11", ' ', false, false)
                    .SetName("@:forwardStatics(<complete> . Before typedef. Issue 2526. Case 11")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2526");
                yield return new TestCaseData("BeforeOnChar_issue2526_12", ' ', false, false)
                    .SetName("@:forwardStatics() <complete> Issue 2526. Case 12")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2526");
                yield return new TestCaseData("BeforeOnChar_issue2526_13", ' ', false, true)
                    .SetName("@:forwardStatics(fromCharCode, <complete>) Issue 2526. Case 13")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2526");
            }
        }

        [
            Test, 
            TestCaseSource(nameof(OnCharIssue2105TestCases)),
            TestCaseSource(nameof(OnCharIssue2358TestCases)),
            TestCaseSource(nameof(OnCharIssue2396TestCases)),
            TestCaseSource(nameof(OnCharIssue589TestCases)),
            TestCaseSource(nameof(OnCharIssue2483TestCases)),
            TestCaseSource(nameof(OnCharIssue2497TestCases)),
            TestCaseSource(nameof(OnCharIssue2518TestCases)),
            TestCaseSource(nameof(OnCharIssue2522TestCases)),
            TestCaseSource(nameof(OnCharIssue2526TestCases)),
        ]
        public void OnChar(string fileName, char addedChar, bool autoHide, bool hasCompletion)
        {
            ((HaXeSettings) ASContext.Context.Settings).CompletionMode = HaxeCompletionModeEnum.FlashDevelop;
            OnChar(sci, ReadAllText(fileName), addedChar, autoHide, hasCompletion);
        }

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

        static IEnumerable<TestCaseData> GetToolTipTextIssue2439TestCases
        {
            get
            {
                yield return new TestCaseData("GetToolTipText_issue2439_1")
                    .SetName("var type:Class|Issue2439_1. Issue 2439. Case 1")
                    .Returns("public class ClassIssue2439_1");
            }
        }

        [
            Test,
            TestCaseSource(nameof(GetToolTipTextTestCases)),
            TestCaseSource(nameof(GetToolTipTextIssue2439TestCases)),
        ]
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
                    .Returns("foo : Function<[BGCOLOR=#2F90:NORMAL](parameter0:Int, parameter1:Int):Bool[/BGCOLOR]>")
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

        static IEnumerable<TestCaseData> CalltipDefIssue2468TestCases
        {
            get
            {
                yield return new TestCaseData("CalltipDef_issue2468_1")
                    .Returns("CalltipDef_issue2468_1 (v1:Int)")
                    .SetName("new CalltipDef_issue2468_1<Foo>(|). Case 1");
            }
        }

        static IEnumerable<TestCaseData> CalltipDefIssue2475TestCases
        {
            get
            {
                yield return new TestCaseData("CalltipDef_issue2475_1")
                    .Returns("item : Function<[BGCOLOR=#2F90:NORMAL](parameter0:String):Void[/BGCOLOR]>")
                    .SetName("a[0](<complete>. Issue 2475. Case 1");
                yield return new TestCaseData("CalltipDef_issue2475_2")
                    .Returns("item : Function<[BGCOLOR=#2F90:NORMAL](parameter0:String):Void[/BGCOLOR]>")
                    .SetName("a[0](<complete>). Issue 2475. Case 2");
                yield return new TestCaseData("CalltipDef_issue2475_3")
                    .Returns("item : Function<[BGCOLOR=#2F90:NORMAL](parameter0:String):Void[/BGCOLOR]>")
                    .SetName("trace(a[0](<complete>)). Issue 2475. Case 3");
                yield return new TestCaseData("CalltipDef_issue2475_4")
                    .Returns("item : Function<[BGCOLOR=#2F90:NORMAL](parameter0:A):B[/BGCOLOR]>")
                    .SetName("trace(a[0](<complete>)). Issue 2475. Case 4");
                yield return new TestCaseData("CalltipDef_issue2475_5")
                    .Returns("callback : Function<[BGCOLOR=#2F90:NORMAL](parameter0:A):B[/BGCOLOR]>")
                    .SetName("trace(a[0]()(<complete>)). Issue 2475. Case 5");
                yield return new TestCaseData("CalltipDef_issue2475_6")
                    .Returns("callback : Function<[BGCOLOR=#2F90:NORMAL](parameter0:A):B[/BGCOLOR]>")
                    .SetName("trace(a[0]()()(<complete>)). Issue 2475. Case 6");
            }
        }

        static IEnumerable<TestCaseData> CalltipDefIssue2489TestCases
        {
            get
            {
                yield return new TestCaseData("CalltipDef_issue2489_1")
                    .Returns("test<String> (v:String) : String")
                    .SetName("(test<T>(v:T):T)(<complete>. Issue 2489. Case 1");
                yield return new TestCaseData("CalltipDef_issue2489_2")
                    .Returns("test<String, Int> (v:String, v2:Int) : Int")
                    .SetName("(test<T, K>(v:T, v1:K):K)(<complete>. Issue 2489. Case 2");
                yield return new TestCaseData("CalltipDef_issue2510_1")
                    .Returns("charAt (index:Int) : String")
                    .SetName("SomeType.(foo<T>('string'):T).charAt(<complete>. Issue 2510. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2510");
            }
        }

        static IEnumerable<TestCaseData> CalltipDefIssue589TestCases
        {
            get
            {
                yield return new TestCaseData("CalltipDef_issue589_1")
                    .Returns("EValue (v1:String, v2:Int) : EType")
                    .SetName("case EType(<complete>. Issue 589. Case 1");
            }
        }

        static IEnumerable<TestCaseData> CalltipDefTestCases
        {
            get
            {
                yield return new TestCaseData("CalltipDef_1")
                    .Returns("PCalltipDef_1 (a, b, c, d)")
                    .SetName("private var _value = new SomeType(<complete>. CalltipDef. Case 1");
            }
        }

        [
            Test,
            TestCaseSource(nameof(CalltipDefIssue2356TestCases)),
            TestCaseSource(nameof(CalltipDefIssue2364TestCases)),
            TestCaseSource(nameof(CalltipDefIssue2368TestCases)),
            TestCaseSource(nameof(CalltipDefIssue2468TestCases)),
            TestCaseSource(nameof(CalltipDefIssue2475TestCases)),
            TestCaseSource(nameof(CalltipDefIssue2489TestCases)),
            TestCaseSource(nameof(CalltipDefIssue589TestCases)),
            TestCaseSource(nameof(CalltipDefTestCases)),
        ]
        public string CalltipDef(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFile(GetFullPath(fileName));
            var manager = UITools.Manager;
            ASComplete.HandleFunctionCompletion(sci, false);
            Assert.IsTrue(UITools.CallTip.CallTipActive);
            return ASComplete.calltipDef;
        }
    }

    class CodeCompleteTests2 : ASCompleteTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            ASContext.CommonSettings.GeneratedMemberDefaultBodyStyle = GeneratedMemberBodyStyle.ReturnDefaultValue;
            SetHaxeFeatures(sci);
        }

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
                    .SetName("'${BeforeOnCharAndReplaceText_6.| }'");
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

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2415TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2415_1", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2415_1"))
                    .SetName("foo(function() {return null;}).| Issue 2415. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2415");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2415_2", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2415_2"))
                    .SetName("[function() {return null;}].| Issue 2415. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2415");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2415_3", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2415_3"))
                    .SetName("[function() return null].| Issue 2415. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2415");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2415_4", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2415_4"))
                    .SetName("[function():Void->Array<Void->Void> return null].| Issue 2415. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2415");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2415_5", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2415_5"))
                    .SetName("[function():{x:Int, y:Int} return null].| Issue 2415. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2415");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2415_6", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2415_6"))
                    .SetName("[function() return [], [function() return []].| Issue 2415. Case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2415");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue589TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue589_1", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue589_1"))
                    .SetName("case | Issue 589. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue589_2", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue589_2"))
                    .SetName("case | Issue 589. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue589_3", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue589_3"))
                    .SetName("case | Issue 589. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue589_4", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue589_4"))
                    .SetName("case | Issue 589. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue589_5", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue589_5"))
                    .SetName("case | Issue 589. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/589");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2497TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2497_1", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2497_1"))
                    .SetName("var v:EnumAbstract = <complete> Issue 2497. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2497");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2497_2", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2497_2"))
                    .SetName("if ((v:EnumAbstract) != <complete>) Issue 2497. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2497");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2497_3", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2497_3"))
                    .SetName("if ((v:EnumAbstract) == <complete>) Issue 2497. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2497");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2499TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2499_1", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2499_1"))
                    .SetName("(foo<T:{}>(String:Class<T>):T).<complete> Issue 2499. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2499");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2516TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2516_1", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2516_1"))
                    .SetName("var v:EnumType = <complete> Issue 2516. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2516");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2516_2", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2516_2"))
                    .SetName("if ((v:EnumType) == <complete> Issue 2516. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2516");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2516_3", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2516_3"))
                    .SetName("if ((v:EnumType) != <complete> Issue 2516. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2516");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2518TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2518_1", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2518_1"))
                    .SetName("var v:String->Float = <complete> Issue 2518. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2518");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2518_2", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2518_2"))
                    .SetName("(v:String->Float) = <complete> Issue 2518. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2518");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2518_3", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2518_3"))
                    .SetName("function(v:String->Float = <complete> Issue 2518. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2518");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2522TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2522_1", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2522_1"))
                    .SetName("function(v:Array<Int> = <complete> Issue 2522. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2522");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2522_2", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2522_2"))
                    .SetName("function(v:Bool = <complete> Issue 2522. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2522");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue1909TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue1909_1", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue1909_1"))
                    .SetName("(v:Abstract<Array>).<complete> Issue 1909. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1909");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue1909_2", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue1909_2"))
                    .SetName("(v:Abstract<Array>).<complete> Issue 1909. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1909");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue1909_3", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue1909_3"))
                    .SetName("(v:Abstract<Array>).<complete> Issue 1909. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1909");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue1909_4", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue1909_4"))
                    .SetName("(v:String).<complete> Issue 1909. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1909");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue1909_5", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue1909_5"))
                    .SetName("(v:String).<complete> Issue 1909. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1909");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue1909_6", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue1909_6"))
                    .SetName("(v:CustomType).<complete> Issue 1909. Case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1909");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue1909_7", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue1909_7"))
                    .SetName("(this:Abstract<Array>).<complete> Issue 1909. Case 7")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1909");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue1909_8", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue1909_8"))
                    .SetName("(v:Abstract<Array<String>>).<complete> Issue 1909. Case 8")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1909");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2526TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2526_1", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2526_1"))
                    .SetName("@:forward(<complete> Issue 2526. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2526");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2526_2", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2526_2"))
                    .SetName("@:forward(charAt, <complete> Issue 2526. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2526");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2526_3", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2526_3"))
                    .SetName("@:forwardStatics(<complete> Issue 2526. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2526");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2536TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2536_1", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2536_1"))
                    .SetName("class Foo<T:String>() { (v:T).<complete> Issue 2536. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2536");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2536_2", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2536_2"))
                    .SetName("class Foo<T:String>() { (v:T).<complete> Issue 2536. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2536");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2538TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnChar_issue2538_1", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2538_1"))
                    .SetName("new Foo().<complete> Issue 2538. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2538");
                yield return new TestCaseData("BeforeOnChar_issue2538_2", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2538_2"))
                    .SetName("@:privateAccess new Foo().<complete> Issue 2538. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2538");
                yield return new TestCaseData("BeforeOnChar_issue2538_3", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2538_3"))
                    .SetName("@:privateAccess new Foo().<complete> Issue 2538. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2538");
            }
        }

        [
            Test,
            TestCaseSource(nameof(OnCharAndReplaceTextTestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2134TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2320TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceText_enums_TestCases)),
            //TestCaseSource(nameof(OnCharAndReplaceText_enums2_TestCases)), // TODO: That tests pass without other tests.
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2415TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue589TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2497TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2499TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2516TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2518TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2522TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue1909TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2526TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2536TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2538TestCases)),
        ]
        public string OnCharAndReplaceText(string fileName, char addedChar, bool autoHide)
        {
            ASContext.Context.ResolveDotContext(null, null, false).ReturnsForAnyArgs(it => null);
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
                yield return new TestCaseData("BeforeOnCharAndReplaceText_16", ' ', true)
                    .SetName("public | ")
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

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2404TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2404_1", ';', true, true)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceTextIssue2404_1"))
                    .SetName("var v:Int=1;| Issue 2404. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2404");
                yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2404_1", ';', true, false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceTextIssue2404_1"))
                    .SetName("var v:Int=1;| Issue 2404. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2404");
            }
        }

        [Test, TestCaseSource(nameof(OnCharAndReplaceTextIssue2404TestCases))]
        public string OnCharAndReplaceTextIssue2404(string fileName, char addedChar, bool autoHide, bool disableCompletionOnDemand)
        {
            var settings = (HaXeSettings) ASContext.GetLanguageContext("haxe").Settings;
            var originDisableCompletionOnDemand = settings.DisableCompletionOnDemand;
            settings.DisableCompletionOnDemand = disableCompletionOnDemand;
            PluginBase.MainForm.CurrentDocument.IsEditable.Returns(true);
            var manager = UITools.Manager;
            SetSrc(sci, CodeCompleteTests.ReadAllText(fileName));
            ASContext.Context.CurrentClass.InFile.Context = ASContext.Context;
            ASContext.HasContext = true;
            ASComplete.OnChar(sci, addedChar, autoHide);
            settings.DisableCompletionOnDemand = originDisableCompletionOnDemand;
            return sci.Text;
        }
    }
}
