using System.Collections.Generic;
using System.IO;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.Settings;
using HaXeContext.TestUtils;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using PluginCore;
using PluginCore.Controls;

namespace HaXeContext.Completion.Haxe3
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
                yield return new TestCaseData("IsInterpolationStringStyle_issue2684_1")
                    .Returns(true)
                    .SetName("'$value<cursor>'. Issue 2684. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2684");
                yield return new TestCaseData("IsInterpolationStringStyle_issue2684_2")
                    .Returns(false)
                    .SetName("'$value.charAt<cursor>'. Issue 2684. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2684");
                yield return new TestCaseData("IsInterpolationStringStyle_issue2684_3")
                    .Returns(false)
                    .SetName("'$array[index<cursor>]'. Issue 2684. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2684");
                yield return new TestCaseData("IsInterpolationStringStyle_issue2684_4")
                    .Returns(false)
                    .SetName("'$func(param<cursor>)'. Issue 2684. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2684");
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
                yield return new TestCaseData("BeforeOnChar_issue589_8", ' ', false, true)
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
                yield return new TestCaseData("BeforeOnChar_issue2518_1", ' ', false, true)
                    .SetName("if((v:Void->Void) == <complete> Issue 2518. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2518");
                yield return new TestCaseData("BeforeOnChar_issue2518_2", ' ', false, true)
                    .SetName("if((v:Void->Void) != <complete> Issue 2518. Case 2")
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

        static IEnumerable<TestCaseData> OnCharIssue2598TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnChar_issue2598_1", '.', false, true)
                    .SetName("(v:Null<T>).<complete> Issue 2598. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2598");
                yield return new TestCaseData("BeforeOnChar_issue2598_2", '.', false, true)
                    .SetName("(v:Null<js.html.VideoElement>).preload.<complete> Issue 2598. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2598");
            }
        }

        static IEnumerable<TestCaseData> OnCharIssue170TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnChar_issue170_1", '.', false, true)
                    .SetName("foo().<complete> Issue 170. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/170");
                yield return new TestCaseData("BeforeOnChar_issue170_2", '.', false, true)
                    .SetName("foo().<complete> Issue 170. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/170");
                yield return new TestCaseData("BeforeOnChar_issue170_3", '.', false, true)
                    .SetName("foo().<complete> Issue 170. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/170");
            }
        }

        static IEnumerable<TestCaseData> OnCharIssue2630TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnChar_issue2630_1", ' ', false, true)
                    .SetName("var v:Bool = <complete> Issue 2630. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2630");
                yield return new TestCaseData("BeforeOnChar_issue2630_2", ' ', false, false)
                    .SetName("var v:Bool |= <complete> Issue 2630. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2630");
                yield return new TestCaseData("BeforeOnChar_issue2630_3", ' ', false, true)
                    .SetName("var v:Null<Int> = <complete> Issue 2630. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2630");
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
            TestCaseSource(nameof(OnCharIssue2598TestCases)),
            TestCaseSource(nameof(OnCharIssue170TestCases)),
            TestCaseSource(nameof(OnCharIssue2630TestCases)),
        ]
        public void OnChar(string fileName, char addedChar, bool autoHide, bool hasCompletion)
        {
            ((HaXeSettings) ASContext.Context.Settings).CompletionMode = HaxeCompletionModeEnum.FlashDevelop;
            OnChar(sci, ReadAllText(fileName), addedChar, autoHide, hasCompletion);
        }


        static IEnumerable<TestCaseData> OnCharIssue2955TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnChar_issue2955_1", '.', false, Is.Not.EqualTo("IInterface"))
                    .SetName("new IInterface<complete> Issue 2955. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2955");
                yield return new TestCaseData("BeforeOnChar_issue2955_2", '.', false, Is.Not.EqualTo("Enum"))
                    .SetName("new Enum<complete> Issue 2955. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2955");
                yield return new TestCaseData("BeforeOnChar_issue2955_3", '.', false, Is.Not.EqualTo("Typedef"))
                    .SetName("new Typedef<complete> Issue 2955. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2955");
                yield return new TestCaseData("BeforeOnChar_issue2955_4", '.', false, Is.Not.EqualTo("EnumAbstract"))
                    .SetName("new EnumAbstract<complete> Issue 2955. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2955");
                yield return new TestCaseData("BeforeOnChar_issue2955_5", '.', false, Is.Not.EqualTo("ClassWithoutConstructor"))
                    .SetName("new ClassWithoutConstructor<complete> Issue 2955. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2955");
                yield return new TestCaseData("BeforeOnChar_issue2955_6", '.', false, Is.EqualTo("String"))
                    .SetName("new ClassWithConstructor<complete> Issue 2955. Case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2955");
                yield return new TestCaseData("BeforeOnChar_issue2955_7", '.', false, Is.Not.EqualTo("Float"))
                    .SetName("new Float<complete> Issue 2955. Case 7")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2955");
                yield return new TestCaseData("BeforeOnChar_issue2955_8", '.', false, Is.Not.EqualTo("Iterator"))
                    .SetName("new Iterator<complete> Issue 2955. Case 8")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2955");
                yield return new TestCaseData("BeforeOnChar_issue2955_9", '.', false, Is.Not.EqualTo("ValueType"))
                    .SetName("new ValueType<complete> Issue 2955. Case 9")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2955");
                yield return new TestCaseData("BeforeOnChar_issue2955_10", '.', false, Is.EqualTo("Map"))
                    .SetName("new Map<complete> Issue 2955. Case 10")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2955");
                yield return new TestCaseData("BeforeOnChar_issue2955_11", '.', false, Is.EqualTo("Array"))
                    .SetName("new Array<complete> Issue 2955. Case 11")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2955");
            }
        }

        [
            Test,
            TestCaseSource(nameof(OnCharIssue2955TestCases)),
        ]
        public void OnChar(string fileName, char addedChar, bool autoHide, EqualConstraint selectedLabelEqualConstraint)
        {
            ((HaXeSettings) ASContext.Context.Settings).CompletionMode = HaxeCompletionModeEnum.FlashDevelop;
            OnChar(sci, ReadAllText(fileName), addedChar, autoHide, selectedLabelEqualConstraint);
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
                    .SetName("var type:Class<complete>. Issue 2439. Case 1")
                    .Returns("public class ClassIssue2439_1");
            }
        }

        static IEnumerable<TestCaseData> GetToolTipTextIssue2804TestCases
        {
            get
            {
                yield return new TestCaseData("GetToolTipText_issue2804_1")
                    .SetName("case First<complete>. Issue 2804. Case 1")
                    .Returns("static public inline var First : AInt = 1\n[COLOR=Black]in Issue2804_1[/COLOR]");
            }
        }

        static IEnumerable<TestCaseData> GetToolTipTextIssue2818TestCases
        {
            get
            {
                yield return new TestCaseData("GetToolTipText_issue2818_1")
                    .SetName("a +<complete> b. Issue 2818. Case 1")
                    .Returns("static public inline plus (a:AType, b:Int) : AType\n[COLOR=Black]in Issue2818_1[/COLOR]");
            }
        }

        [
            Test,
            TestCaseSource(nameof(GetToolTipTextTestCases)),
            TestCaseSource(nameof(GetToolTipTextIssue2439TestCases)),
            TestCaseSource(nameof(GetToolTipTextIssue2804TestCases)),
            TestCaseSource(nameof(GetToolTipTextIssue2818TestCases)),
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
                    .Returns("foo (v1, v2) : Void")
                    .SetName("foo(1|, 2). Calltip. Issue 2368. Case 1");
                yield return new TestCaseData("CalltipDef_issue2368_2")
                    .Returns("foo (v1, v2) : Void")
                    .SetName("foo(1, 2|). Calltip. Issue 2368. Case 2");
                yield return new TestCaseData("CalltipDef_issue2368_3")
                    .Returns("foo (v1, v2) : Void")
                    .SetName("foo((1 + 1)|, 2). Calltip. Issue 2368. Case 3");
                yield return new TestCaseData("CalltipDef_issue2368_4")
                    .Returns("foo (v1, v2) : Void")
                    .SetName("foo((1 +| 1), 2). Calltip. Issue 2368. Case 4");
                yield return new TestCaseData("CalltipDef_issue2368_5")
                    .Returns("foo (v1, v2) : Void")
                    .SetName("foo(1, 2 * (3 +| 1)). Calltip. Issue 2368. Case 5");
                yield return new TestCaseData("CalltipDef_issue2368_6")
                    .Returns("bar (v1, v2) : Void")
                    .SetName("foo(1, bar(3|, 1)). Calltip. Issue 2368. Case 6");
                yield return new TestCaseData("CalltipDef_issue2368_7")
                    .Returns("bar (v1, v2) : Void")
                    .SetName("foo(1, bar({x|:1}, 1)). Calltip. Issue 2368. Case 7");
                yield return new TestCaseData("CalltipDef_issue2368_8")
                    .Returns("bar_ (v1, v2) : Void")
                    .SetName("foo(1, bar_({x|:1}, 1)). Calltip. Issue 2368. Case 8");
                yield return new TestCaseData("CalltipDef_issue2368_9")
                    .Returns("foo (v1, v2) : Void")
                    .SetName("foo(1, (2 * (3 +| 1))). Calltip. Issue 2368. Case 9");
                yield return new TestCaseData("CalltipDef_issue2368_10")
                    .Returns("foo (v1, v2) : Void")
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

        static IEnumerable<TestCaseData> CalltipDefIssue2559TestCases
        {
            get
            {
                yield return new TestCaseData("CalltipDef_issue2559_1")
                    .Returns("cast<TResult> (expression:Dynamic, type:Class<TResult>) : TResult")
                    .SetName("cast(<complete>. Issue 2559. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2559");
            }
        }

        static IEnumerable<TestCaseData> CalltipDefIssue2569TestCases
        {
            get
            {
                yield return new TestCaseData("CalltipDef_issue2569_1")
                    .Returns("trace (v:Dynamic, ?infos:PosInfos) : Void")
                    .SetName("trace(<complete>. Issue 2569. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2569");
                yield return new TestCaseData("CalltipDef_issue2569_2")
                    .Returns("trace (args:Dynamic) : Void")
                    .SetName("trace(<complete>. Issue 2569. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2569");
            }
        }

        static IEnumerable<TestCaseData> CalltipDefIssue2217TestCases
        {
            get
            {
                yield return new TestCaseData("CalltipDef_issue2217_1")
                    .Returns("newCallback : Function<[BGCOLOR=#2F90:NORMAL](parameter0:Dynamic):CalltipDef_issue2217_1[/BGCOLOR]>")
                    .SetName("newCallback(<complete>. Issue 2217. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2217");
            }
        }

        static IEnumerable<TestCaseData> CalltipDefIssue170TestCases
        {
            get
            {
                yield return new TestCaseData("CalltipDef_issue170_1")
                    .Returns("f () : String")
                    .SetName("f(<complete>). Issue 170. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/170");
                yield return new TestCaseData("CalltipDef_issue170_2")
                    .Returns("f170_2 () : Void")
                    .SetName("f(<complete>). Issue 170. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/170");
                yield return new TestCaseData("CalltipDef_issue170_3")
                    .Returns("f170_3 () : Dynamic")
                    .SetName("f(<complete>). Issue 170. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/170");
            }
        }

        static IEnumerable<TestCaseData> CalltipDefTestCases
        {
            get
            {
                yield return new TestCaseData("CalltipDef_1")
                    .Returns("PCalltipDef_1 (a, b, c, d)")
                    .SetName("private var _value = new SomeType(<complete>. CalltipDef. Case 1");
                yield return new TestCaseData("CalltipDef_issue1900_1")
                    .Returns("urlEncode () : String")
                    .SetName("''.urlEncode(<complete>. CalltipDef. Issue 1900. Case 1");
                yield return new TestCaseData("CalltipDef_issue2682_1")
                    .Returns("f (e:Dynamic) : String")
                    .SetName("f(<complete>. CalltipDef. Issue 2682. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2682");
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
            TestCaseSource(nameof(CalltipDefIssue2559TestCases)),
            TestCaseSource(nameof(CalltipDefIssue2569TestCases)),
            TestCaseSource(nameof(CalltipDefIssue2217TestCases)),
            TestCaseSource(nameof(CalltipDefIssue170TestCases)),
            TestCaseSource(nameof(CalltipDefTestCases)),
        ]
        public string CalltipDef(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFile(GetFullPath(fileName));
            UITools.Init();
            CompletionList.CreateControl(PluginBase.MainForm);
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
                    .SetName("override <complete>. Issue 2134. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2134");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2320TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2320_1", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceTextIssue2320_1"))
                    .SetName("override <complete>. Issue 2320. Case 1 ")
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

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2217TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnChar_issue2217_1", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2217_1"))
                    .SetName("Foo.<complete> Issue 2217. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2217");
                yield return new TestCaseData("BeforeOnChar_issue2217_2", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2217_2"))
                    .SetName("Foo.<complete> Issue 2217. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2217");
                yield return new TestCaseData("BeforeOnChar_issue2217_3", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2217_3"))
                    .SetName("Foo.<complete> Issue 2217. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2217");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue170TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnChar_issue170_4", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue170_4"))
                    .SetName("foo<T>('').<complete> Issue 170. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/170");
                yield return new TestCaseData("BeforeOnChar_issue170_5", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue170_5"))
                    .SetName("foo<T>([]).<complete> Issue 170. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/170");
                yield return new TestCaseData("BeforeOnChar_issue170_6", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue170_6"))
                    .SetName("foo<T>([1,2,3,4]).<complete> Issue 170. Case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/170");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2632TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnChar_issue2632_1", '(', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2632_1"))
                    .SetName("someMethod(<complete> Issue 2632. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2632");
                yield return new TestCaseData("BeforeOnChar_issue2632_2", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2632_2"))
                    .SetName("someMethod(<complete> Issue 2632. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2632");
                yield return new TestCaseData("BeforeOnChar_issue2632_3", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2632_3"))
                    .SetName("someMethod(<complete> Issue 2632. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2632");
                yield return new TestCaseData("BeforeOnChar_issue2632_4", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2632_4"))
                    .SetName("someMethod(<complete> Issue 2632. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2632");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2636TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2636_1", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2636_1"))
                    .SetName("[].<complete> Issue 2636. Case 1")
                    .Ignore("")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2636");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2577TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2577_1", '$', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2577_1"))
                    .SetName("$<complete> Issue 2577. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2577");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2726TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2726_1", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2726_1"))
                    .SetName("extensionOwner.<complete> Issue 2726. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2726");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2750TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2750_1", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2750_1"))
                    .SetName("'\n'.<complete> Issue 2750. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2750");
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2750_2", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2750_2"))
                    .SetName("\"\n\".<complete> Issue 2750. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2750");
            }
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2757TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2757_1", '.', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2757_1"))
                    .SetName("enumValue.<complete> Issue 2757. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2757");

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
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2217TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue170TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2632TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2636TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2577TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2726TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2750TestCases)),
            TestCaseSource(nameof(OnCharAndReplaceTextIssue2757TestCases)),
        ]
        public string OnCharAndReplaceText(string fileName, char addedChar, bool autoHide)
        {
            ASContext.Context.ResolveDotContext(null, null, false).ReturnsForAnyArgs(it => null);
            return OnCharAndReplaceText(sci, CodeCompleteTests.ReadAllText(fileName), addedChar, autoHide);
        }

        static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2657TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeOnCharAndReplaceText_issue2657_1", ' ', false)
                    .Returns(CodeCompleteTests.ReadAllText("AfterOnCharAndReplaceText_issue2657_1"))
                    .SetName("var v:Void->Void = <complete>. Disable void type declaration for function. Issue 2657. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2657");
            }
        }

        [Test, TestCaseSource(nameof(OnCharAndReplaceTextIssue2657TestCases))]
        public string OnCharAndReplaceTextDisableVoidTypeDeclarationForFunction(string fileName, char addedChar, bool autoHide)
        {
            ((HaXeSettings) ASContext.Context.Settings).DisableVoidTypeDeclaration = true;
            var result = OnCharAndReplaceText(fileName, addedChar, autoHide);
            ((HaXeSettings) ASContext.Context.Settings).DisableVoidTypeDeclaration = false;
            return result;
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
            UITools.Init();
            CompletionList.CreateControl(PluginBase.MainForm);
            SetSrc(sci, CodeCompleteTests.ReadAllText(fileName));
            ASContext.Context.CurrentClass.InFile.Context = ASContext.Context;
            ASContext.HasContext = true;
            ASComplete.OnChar(sci, addedChar, autoHide);
            settings.DisableCompletionOnDemand = originDisableCompletionOnDemand;
            return sci.Text;
        }
    }

    class CodeCompleteTests4 : ASCompleteTests
    {
        [TestFixtureSetUp]
        public void Setup() => SetHaxeFeatures(sci);

        static IEnumerable<TestCaseData> GetExpressionTypeTestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionTypeOfConstructor"))
                    .Returns(new MemberModel
                    {
                        Flags = FlagType.Access | FlagType.Function | FlagType.Constructor,
                        Name = "Foo"
                    })
                    .SetName("Get Expression Type of constructor");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionTypeOfConstructorParameter"))
                    .Returns(new MemberModel
                    {
                        Flags = FlagType.Variable | FlagType.ParameterVar,
                        Name = "foo"
                    })
                    .SetName("Get Expression Type of constructor parameter");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionTypeOfFunction"))
                    .Returns(new MemberModel
                    {
                        Flags = FlagType.Access | FlagType.Dynamic | FlagType.Function | FlagType.Inferred,
                        Name = "foo",
                        Type = "Void"
                    })
                    .SetName("Get Expression Type of function");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionTypeOfFunctionParameter"))
                    .Returns(new MemberModel
                    {
                        Flags = FlagType.Variable | FlagType.ParameterVar,
                        Name = "bar"
                    })
                    .SetName("Get Expression Type of function parameter. Case 1");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionTypeOfFunctionParameter2"))
                    .Returns(new MemberModel
                    {
                        Flags = FlagType.Variable | FlagType.ParameterVar,
                        Name = "foo"
                    })
                    .SetName("Get Expression Type of function parameter. Case 2");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionTypeOfFunctionWithParameter"))
                    .Returns(new MemberModel
                    {
                        Flags = FlagType.Access | FlagType.Dynamic | FlagType.Function | FlagType.Inferred,
                        Name = "foo",
                        Type = "Void",
                        Parameters = new List<MemberModel>
                        {
                                new MemberModel("foo", "Int", FlagType.Variable | FlagType.ParameterVar, 0)
                        }
                    })
                    .SetName("Get Expression Type of function with parameter");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionTypeOfLocalDynamicKey"))
                    .Returns(null)
                    .SetName("Get Expression Type of local dynamic object key");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionTypeOfLocalDynamicValue"))
                    .Returns(new MemberModel
                    {
                        Flags = FlagType.Access | FlagType.Dynamic | FlagType.Function | FlagType.Inferred,
                        Name = "foo",
                        Type = "Void"
                    })
                    .SetName("Get Expression Type of local dynamic object value");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionTypeOfLocalVar"))
                    .Returns(new MemberModel
                    {
                        Flags = FlagType.Dynamic | FlagType.Variable | FlagType.LocalVar,
                        Name = "foo"
                    })
                    .SetName("Get Expression Type of local var");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionTypeOfPropertyGetter"))
                    .Returns(new MemberModel
                    {
                        Flags = FlagType.Dynamic | FlagType.Function | FlagType.Inferred,
                        Name = "get_foo",
                        Type = "Int"
                    })
                    .SetName("Get Expression Type of property getter");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionTypeOfPropertySetter"))
                    .Returns(new MemberModel
                    {
                        Flags = FlagType.Dynamic | FlagType.Function | FlagType.Inferred,
                        Name = "set_foo",
                        Type = "Int",
                        Parameters = new List<MemberModel>
                        {
                                new MemberModel("v", "Dynamic", FlagType.Variable | FlagType.ParameterVar, 0)
                        }
                    })
                    .SetName("Get Expression Type of property setter");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionTypeOfVariable"))
                    .Returns(new MemberModel
                    {
                        Flags = FlagType.Access | FlagType.Dynamic | FlagType.Variable,
                        Name = "foo"
                    })
                    .SetName("Get Expression Type of variable");
            }
        }

        [Test, TestCaseSource(nameof(GetExpressionTypeTestCases))]
        public MemberModel GetExpressionType_Member(string sourceText)
        {
            var expr = GetExpressionType(sci, sourceText);
            return expr.Member;
        }

        static IEnumerable<TestCaseData> GetExpressionType_untyped_TypeTestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_untyped_1"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class })
                    .SetName("untyped 's'.");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_untyped_2"))
                    .Returns(new ClassModel { Name = "Array<T>", Flags = FlagType.Class })
                    .SetName("untyped [].");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_cast_TypeTestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_cast"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class })
                    .SetName("cast('s', String).");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_cast_2"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class })
                    .SetName("return cast('s', String).");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_cast_3"))
                    .Returns(new ClassModel { Name = "Int", Flags = FlagType.Class })
                    .SetName("cast('s', String).length");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_cast_4"))
                    .Returns(new ClassModel { Name = "Int", Flags = FlagType.Class })
                    .SetName("cast('s', String).charAt(0).length");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_cast_5"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class })
                    .SetName("cast('...', String).");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_cast_6"))
                    .Returns(new ClassModel { Name = "Int", Flags = FlagType.Class })
                    .SetName("cast('...', String).charAt(0).length.");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_cast_7"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class })
                    .SetName("cast(', Int', String).");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_cast_8"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class })
                    .SetName("cast ( 's' , String ).");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_cast_9"))
                    .Returns(new ClassModel { Name = "Array<String>", Flags = FlagType.Class })
                    .SetName("cast('s', String).split('').");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_cast_10"))
                    .Returns(new ClassModel { Name = "Function", Flags = FlagType.Class })
                    .SetName("cast('s', String).charAt.");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_cast_11"))
                    .Returns(new ClassModel { Name = "Array<T>", Flags = FlagType.Class })
                    .SetName("cast [].");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_cast_12"))
                    .Returns(new ClassModel { Name = "Function", Flags = FlagType.Class })
                    .SetName("cast [].concat.");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_cast_13"))
                    .Returns(new ClassModel { Name = "Int", Flags = FlagType.Class })
                    .SetName("cast [].concat([1]).length.");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_is_TypeTestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_is_1"))
                    .Returns(new ClassModel { Name = "Bool", Flags = FlagType.Class })
                    .SetName("('s' is String).");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_is_2"))
                    .Returns(new ClassModel { Name = "Bool", Flags = FlagType.Class })
                    .SetName("(' is ' is String).");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_is_3"))
                    .Returns(new ClassModel { Name = "Bool", Flags = FlagType.Class })
                    .SetName("('v is Int' is String).");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_is_4"))
                    .Returns(new ClassModel { Name = "Bool", Flags = FlagType.Class })
                    .SetName("('(v is Int)' is String).");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_is_5"))
                    .Returns(new ClassModel { Name = "Bool", Flags = FlagType.Class })
                    .SetName("( 's' is String ).");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_is_6"))
                    .Returns(new ClassModel { Name = "Bool", Flags = FlagType.Class })
                    .SetName("switch ( 's' is String ).");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_is_7"))
                    .Returns(new ClassModel { Name = "Bool", Flags = FlagType.Class })
                    .SetName("return ( 's' is String ).");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_ArrayInitializer_TypeTestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_arrayInitializer_1"))
                    .Returns(new ClassModel { Name = "Array<T>", Flags = FlagType.Class })
                    .SetName("[].");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_arrayInitializer_2"))
                    .Returns(new ClassModel { Name = "Array<T>", Flags = FlagType.Class })
                    .SetName("['...'].");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_arrayInitializer_3"))
                    .Returns(new ClassModel { Name = "Array<T>", Flags = FlagType.Class })
                    .SetName("['=>'].");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_arrayInitializer_4"))
                    .Returns(new ClassModel { Name = "Array<T>", Flags = FlagType.Class })
                    .SetName("[[1 => 2], [2 => 3]].");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_arrayInitializer_5"))
                    .Returns(new ClassModel { Name = "Int", Flags = FlagType.Class })
                    .SetName("[].length.");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_arrayInitializer_6"))
                    .Returns(new ClassModel { Name = "Function", Flags = FlagType.Class })
                    .SetName("[].concat.");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_arrayInitializer_7"))
                    .Returns(new ClassModel { Name = "Array<T>", Flags = FlagType.Class })
                    .SetName("[].concat([]).");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_arrayInitializer_8"))
                    .Returns(new ClassModel { Name = "Int", Flags = FlagType.Class })
                    .SetName("[].concat([]).length.");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_arrayInitializer_9"))
                    .Returns(new ClassModel { Name = "Int", Flags = FlagType.Class })
                    .SetName("[].concat([])[0].");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_arrayInitializer_10"))
                    .Returns(new ClassModel { Name = "Array<T>", Flags = FlagType.Class })
                    .SetName("return [].");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_arrayInitializer_11"))
                    .Returns(new ClassModel { Name = "Array<T>", Flags = FlagType.Class })
                    .SetName("switch [].");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_arrayInitializer_12"))
                    .Returns(new ClassModel { Name = "Array<T>", Flags = FlagType.Class })
                    .SetName("for(it in [].");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_arrayInitializer_13"))
                    .Returns(new ClassModel { Name = "Array<T>", Flags = FlagType.Class })
                    .SetName("var l = try [1]. catch(e:Dynamic) 1;");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_arrayInitializer_14"))
                    .Returns(new ClassModel { Name = "Function", Flags = FlagType.Class })
                    .SetName("var l = try [1].concat. catch(e:Dynamic) 1;");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_arrayInitializer_15"))
                    .Returns(new ClassModel { Name = "Array<T>", Flags = FlagType.Class })
                    .SetName("case [1, 2].");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_arrayInitializer_16"))
                    .Returns(new ClassModel { Name = "Array<T>", Flags = FlagType.Class })
                    .SetName("var a = [1, 2].");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_arrayInitializer_17"))
                    .Returns(new ClassModel { Name = "Function", Flags = FlagType.Class })
                    .SetName("[1 => [1].concat.]");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_arrayInitializer_18"))
                    .Returns(new ClassModel { Name = "Function", Flags = FlagType.Class })
                    .SetName("{c:[1].concat.}");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_MapInitializer_TypeTestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_mapInitializer_1"))
                    .Returns(new ClassModel { Name = "Map<K, V>", Flags = FlagType.Class })
                    .SetName("[1=>1].");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_mapInitializer_2"))
                    .Returns(new ClassModel { Name = "Map<K, V>", Flags = FlagType.Class })
                    .SetName("['...' => 1, '1' => '...'].");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_StringInitializer_TypeTestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_stringInitializer"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class })
                    .SetName("\"\".");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_stringInitializer_2"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class })
                    .SetName("''.");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_stringInitializer_3"))
                    .Returns(new ClassModel { Name = "Int", Flags = FlagType.Class })
                    .SetName("''.length.");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_stringInitializer_4"))
                    .Returns(new ClassModel { Name = "Function", Flags = FlagType.Class })
                    .SetName("''.charAt.");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_stringInitializer_5"))
                    .Returns(new ClassModel { Name = "Array<String>", Flags = FlagType.Class })
                    .SetName("''.split('').");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_stringInitializer_6"))
                    .Returns(new ClassModel { Name = "Int", Flags = FlagType.Class })
                    .SetName("''.split('').length.");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_stringInitializer_7"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class })
                    .SetName("''.split('')[0].");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_stringInitializer_8"))
                    .Returns(new ClassModel { Name = "Int", Flags = FlagType.Class })
                    .SetName("'...'.length.");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_stringInitializer_9"))
                    .Returns(new ClassModel { Name = "Int", Flags = FlagType.Class })
                    .SetName("\"...\".length.");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_new_TypeTestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_new_1"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class })
                    .SetName("new String('').");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_new_2"))
                    .Returns(new ClassModel { Name = "Function", Flags = FlagType.Class })
                    .SetName("new String('').charAt.");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_new_3"))
                    .Returns(new ClassModel { Name = "Array<String>", Flags = FlagType.Class })
                    .SetName("new String('').split('').");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_InferVariableTypeIssue2362TestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_issue2362_1"))
                    .Returns(new ClassModel { Name = "Dynamic", Flags = FlagType.Class })
                    .SetName("var фывValue. = ''");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_issue2362_2"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class })
                    .SetName("function foo(?фывValue. = '')");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_issue2362_3"))
                    .Returns(new ClassModel { Name = "VariableType", Flags = FlagType.Class | FlagType.TypeDef })
                    .SetName("typedef фывVariableType. = String");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_InferVariableTypeIssue2373TestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_issue2373_1"))
                    .Returns(new ClassModel { Name = "Issue2373Foo", Flags = FlagType.Class | FlagType.Abstract })
                    .SetName("var One = 1. Issue 2373. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2373");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_issue2373_2"))
                    .Returns(new ClassModel { Name = "Issue2373Bar", Flags = FlagType.Class | FlagType.Abstract })
                    .SetName("var One = 1. Issue 2373. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2373");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_InferVariableTypeIssue2401TestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_issue2401_1"))
                    .Returns(new ClassModel { Name = "Null<Dynamic>", Flags = FlagType.Class | FlagType.Abstract })
                    .SetName("function foo(?v| = null). Issue 2401. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2401");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_InferVariableTypeIssue2771TestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_issue2771_1"))
                    .Returns(new ClassModel { Name = "Dynamic", Flags = FlagType.Class | FlagType.Abstract })
                    .SetName("v<complete> = switch... Issue 2771. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2771");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_InferVariableTypeIssue2788TestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_issue2788_1"))
                    .Returns(new ClassModel { Name = "Dynamic", Flags = FlagType.Class | FlagType.Abstract })
                    .SetName("v<complete> - foo(this.x = 0). Issue 2788. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2788");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_InferVariableTypeIssue2796TestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_issue2796_1"))
                    .Returns(new ClassModel { Name = "Dynamic", Flags = FlagType.Class | FlagType.Abstract })
                    .SetName("for(v<complete> ... Issue 2796. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2796");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_ArrayAccess_issue2471_TypeTestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ArrayAccess_issue2471_1"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class })
                    .SetName("a[0].<complete> Issue 2471. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2471");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ArrayAccess_issue2471_2"))
                    .Returns(new ClassModel { Name = "Function", Flags = FlagType.Class })
                    .SetName("a[0].<complete> Issue 2471. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2471");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ArrayAccess_issue2471_3"))
                    .Returns(new ClassModel { Name = "Array<String->String>", Flags = FlagType.Class })
                    .SetName("a[0].<complete> Issue 2471. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2471");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ArrayAccess_issue2471_4"))
                    .Returns(new ClassModel { Name = "Array<Array<String->String>->String>", Flags = FlagType.Class })
                    .SetName("a[0].<complete> Issue 2471. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2471");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_ParameterizedFunction_issue2203_TypeTestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ParameterizedFunction_issue2203_1"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, InFile = FileModel.Ignore })
                    .SetName("foo<T>('string').<complete> Issue 2203. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2203");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ParameterizedFunction_issue2203_2"))
                    .Returns(new ClassModel { Name = "Bool", Flags = FlagType.Class, InFile = FileModel.Ignore })
                    .SetName("foo<T>(_, bool, ?_, ?_).<complete> Issue 2203. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2203");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ParameterizedFunction_issue2203_3"))
                    .Returns(new ClassModel { Name = "Float", Flags = FlagType.Class, InFile = FileModel.Ignore })
                    .SetName("foo<K, T, R1, R2>(_, 1.0, ?_, ?_).<complete> Issue 2203. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2203");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ParameterizedFunction_issue2203_4"))
                    .Returns(new ClassModel { Name = "Float", Flags = FlagType.Class, InFile = FileModel.Ignore })
                    .SetName("foo<K, T, R1, R2>(_, localVar, ?_, ?_).<complete> Issue 2203. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2203");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_ParameterizedFunction_issue2487_TypeTestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ParameterizedFunction_issue2487_1"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, InFile = FileModel.Ignore })
                    .SetName("(foo<T>(''):Array<T>)[0].<complete> Issue 2487. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2487");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_ParameterizedFunction_issue2499_TypeTestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ParameterizedFunction_issue2499_1"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, InFile = FileModel.Ignore })
                    .SetName("(foo<T>((String:Null<T>)):<T>).<complete> Issue 2499. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2499");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ParameterizedFunction_issue2499_2"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, InFile = FileModel.Ignore })
                    .SetName("(foo<T>((String:Dynamic<T>)):<T>).<complete> Issue 2499. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2499");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ParameterizedFunction_issue2499_3"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, InFile = FileModel.Ignore })
                    .SetName("(foo<T>((String:Class<T>)):<T>).<complete> Issue 2499. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2499");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ParameterizedFunction_issue2499_4"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, InFile = FileModel.Ignore })
                    .SetName("(foo<T:{}>((String:Class<T>)):<T>).<complete> Issue 2499. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2499");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ParameterizedFunction_issue2499_5"))
                    .Returns(new ClassModel { Name = "Class<String>", Flags = FlagType.Class | FlagType.Abstract, InFile = FileModel.Ignore })
                    .SetName("Type.getClass(String).<complete> Issue 2499. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2499");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_ParameterizedFunction_issue2503_TypeTestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ParameterizedFunction_issue2503_1"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, InFile = FileModel.Ignore })
                    .SetName("a[Some.Value].<complete> Issue 2503. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2503");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_ParameterizedFunction_issue2505_TypeTestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ParameterizedFunction_issue2505_1"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, InFile = FileModel.Ignore })
                    .SetName("(v:T).<complete> Issue 2505. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2505");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ParameterizedFunction_issue2505_2"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, InFile = FileModel.Ignore })
                    .SetName("(v:T).<complete> Issue 2505. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2505");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionType_ParameterizedClass_issue2536_TypeTestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ParameterizedClass_issue2536_1"))
                    .Returns(new ClassModel { Name = "A2536_1", Flags = FlagType.Class, InFile = FileModel.Ignore })
                    .SetName("(v:T).<complete> Issue 2536. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2536");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ParameterizedClass_issue2536_2"))
                    .Returns(new ClassModel { Name = "B2536_2", Flags = FlagType.Class, InFile = FileModel.Ignore })
                    .SetName("(v:T).<complete> Issue 2536. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2536");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_ParameterizedClass_issue2536_3"))
                    .Returns(new ClassModel { Name = "A2536_3", Flags = FlagType.Class, InFile = FileModel.Ignore })
                    .SetName("(v:T).<complete> Issue 2536. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2536");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionTypeIssue2624TestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_issue2624_1"))
                    .Returns(null)
                    .SetName("this.<complete>. Issue 2624. Case 1");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_issue2624_2"))
                    .Returns(null)
                    .SetName("super.<complete>. Issue 2624. Case 2");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionTypeIssue2710TestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_issue2710_1"))
                    .Returns(new ClassModel { Name = "Null<BinaryType>", Flags = FlagType.Abstract | FlagType.Class, InFile = FileModel.Ignore })
                    .SetName("v<complete>. Issue 2710. Case 1");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_issue2710_2"))
                    .Returns(new ClassModel { Name = "Null<AlignSetting>", Flags = FlagType.Abstract | FlagType.Class, InFile = FileModel.Ignore })
                    .SetName("v<complete>. Issue 2710. Case 2");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_issue2710_3"))
                    .Returns(new ClassModel { Name = "Null<BinaryType>", Flags = FlagType.Abstract | FlagType.Class, InFile = FileModel.Ignore })
                    .SetName("v<complete>. Issue 2710. Case 3");
            }
        }

        [
            Test,
            TestCaseSource(nameof(GetExpressionType_untyped_TypeTestCases)),
            TestCaseSource(nameof(GetExpressionType_cast_TypeTestCases)),
            TestCaseSource(nameof(GetExpressionType_is_TypeTestCases)),
            TestCaseSource(nameof(GetExpressionType_ArrayInitializer_TypeTestCases)),
            TestCaseSource(nameof(GetExpressionType_MapInitializer_TypeTestCases)),
            TestCaseSource(nameof(GetExpressionType_StringInitializer_TypeTestCases)),
            TestCaseSource(nameof(GetExpressionType_new_TypeTestCases)),
            TestCaseSource(nameof(GetExpressionType_InferVariableTypeIssue2362TestCases)),
            TestCaseSource(nameof(GetExpressionType_InferVariableTypeIssue2373TestCases)),
            TestCaseSource(nameof(GetExpressionType_InferVariableTypeIssue2401TestCases)),
            TestCaseSource(nameof(GetExpressionType_InferVariableTypeIssue2771TestCases)),
            TestCaseSource(nameof(GetExpressionType_InferVariableTypeIssue2788TestCases)),
            TestCaseSource(nameof(GetExpressionType_InferVariableTypeIssue2796TestCases)),
            TestCaseSource(nameof(GetExpressionType_ArrayAccess_issue2471_TypeTestCases)),
            TestCaseSource(nameof(GetExpressionType_ParameterizedFunction_issue2203_TypeTestCases)),
            TestCaseSource(nameof(GetExpressionType_ParameterizedFunction_issue2487_TypeTestCases)),
            TestCaseSource(nameof(GetExpressionType_ParameterizedFunction_issue2499_TypeTestCases)),
            TestCaseSource(nameof(GetExpressionType_ParameterizedFunction_issue2503_TypeTestCases)),
            TestCaseSource(nameof(GetExpressionType_ParameterizedFunction_issue2505_TypeTestCases)),
            TestCaseSource(nameof(GetExpressionType_ParameterizedClass_issue2536_TypeTestCases)),
            TestCaseSource(nameof(GetExpressionTypeIssue2624TestCases)),
            TestCaseSource(nameof(GetExpressionTypeIssue2710TestCases)),
        ]
        public ClassModel GetExpressionType_Type(string sourceText)
        {
            var expr = GetExpressionType(sci, sourceText);
            return expr.Type;
        }

        static IEnumerable<TestCaseData> GetExpressionTypeSDK_340_TypeTestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_typecheck"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class })
                    .SetName("('s':String).");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_typecheck_2"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class })
                    .SetName("return ('s':String).");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_typecheck_3"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class })
                    .SetName("return ('...':String).");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_typecheck_4"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class })
                    .SetName("('...' : String).");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_typecheck_5"))
                    .Returns(new ClassModel { Name = "String", Flags = FlagType.Class })
                    .SetName("('v:Int' : String).");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_typecheck_6"))
                    .Returns(new ClassModel { Name = "Int", Flags = FlagType.Class })
                    .SetName("('s':String).charAt(0).length.");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_typecheck_7"))
                    .Returns(new ClassModel { Name = "Array<String>", Flags = FlagType.Class })
                    .SetName("('s':String).split('').");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionType_Type_typecheck_8"))
                    .Returns(new ClassModel { Name = "Function", Flags = FlagType.Class })
                    .SetName("('s':String).split.");
            }
        }

        [Test, TestCaseSource(nameof(GetExpressionTypeSDK_340_TypeTestCases))]
        public ClassModel GetExpressionType_Type_SDK_340(string sourceText)
        {
            ASContext.Context.Settings.InstalledSDKs = new[] { new InstalledSDK { Path = PluginBase.CurrentProject.CurrentSDK, Version = "3.4.0" } };
            var expr = GetExpressionType(sci, sourceText);
            return expr.Type;
        }

        [Test]
        public void Issue1867()
        {
            SetSrc(sci, CodeCompleteTests.ReadAllText("GetExpressionTypeOfFunction_Issue1867_1"));
            var expr = ASComplete.GetExpressionType(sci, sci.WordEndPosition(sci.CurrentPos, true));
            Assert.AreEqual(new ClassModel { Name = "Function", InFile = new FileModel { Package = "haxe.Constraints" } }, expr.Type);
        }

        static IEnumerable<TestCaseData> GetExpressionTestCases
        {
            get
            {
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionOfNewMap"))
                    .Returns("new Map<String, Int>")
                    .SetName("From new Map<String, Int>|");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionOfNewMap2"))
                    .Returns("new Map<Map<String, Int>, Int>")
                    .SetName("From new Map<Map<String, Int>, Int>|");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionOfNewMap3"))
                    .Returns("new Map<String, Array<Map<String, Int>>>")
                    .SetName("From new Map<String, Array<Map<String, Int>>>|");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionOfNewMap4"))
                    .Returns("new Map<String, Array<Int->Int->Int>>")
                    .SetName("From new Map<String, Array<Int->Int->Int>>|");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionOfMapInitializer"))
                    .Returns(";[\"1\" => 1, \"2\" => 2]")
                    .SetName("From [\"1\" => 1, \"2\" => 2]|");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionOfRegex"))
                    .Returns(";g")
                    .SetName("~/regex/g|")
                    .Ignore("https://github.com/fdorg/flashdevelop/issues/1880");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionOfNewArray"))
                    .Returns("new Array<Int>")
                    .SetName("From new Array<Int>|");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionOfNewArray2"))
                    .Returns("new Array<{x:Int, y:Int}>")
                    .SetName("From new Array<{x:Int, y:Int}>|");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionOfNewArray3"))
                    .Returns("new Array<{name:String, params:Array<Dynamic>}>")
                    .SetName("From new Array<{name:String, params:Array<Dynamic>}>|");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpressionOfStringInterpolation.charAt"))
                    .Returns(";'result: ${1 + 2}'.charAt")
                    .SetName("'result: ${1 + 2}'.charAt");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpression_issue1749_plus"))
                    .Returns("+1")
                    .SetName("1 + 1");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpression_issue1749_minus"))
                    .Returns("-1")
                    .SetName("1 - 1");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpression_issue1749_mul"))
                    .Returns("*1")
                    .SetName("1 * 1");
                yield return new TestCaseData(CodeCompleteTests.ReadAllText("GetExpression_issue1749_division"))
                    .Returns("/1")
                    .SetName("1 / 1");
                yield return
                    new TestCaseData(CodeCompleteTests.ReadAllText("GetExpression_issue1749_increment"))
                        .Returns("++1")
                        .SetName("++1");
                yield return
                    new TestCaseData(CodeCompleteTests.ReadAllText("GetExpression_issue1749_increment2"))
                        .Returns(";1++")
                        .SetName("1++. case 1");
                yield return
                    new TestCaseData(CodeCompleteTests.ReadAllText("GetExpression_issue1749_increment3"))
                        .Returns(";1++")
                        .SetName("1++. case 2");
                yield return
                    new TestCaseData(CodeCompleteTests.ReadAllText("GetExpression_issue1749_increment4"))
                        .Returns(";a++")
                        .SetName("a++");
                yield return
                    new TestCaseData(CodeCompleteTests.ReadAllText("GetExpression_issue1749_increment5"))
                        .Returns("=getId()++")
                        .SetName("var id = getId()++|");
                yield return
                    new TestCaseData(CodeCompleteTests.ReadAllText("GetExpression_issue1749_decrement"))
                        .Returns("--1")
                        .SetName("--1");
                yield return
                    new TestCaseData(CodeCompleteTests.ReadAllText("GetExpression_issue1749_decrement2"))
                        .Returns(";1--")
                        .SetName("1--. case 1");
                yield return
                    new TestCaseData(CodeCompleteTests.ReadAllText("GetExpression_issue1749_decrement3"))
                        .Returns(";1--")
                        .SetName("1--. case 2");
                yield return
                    new TestCaseData(CodeCompleteTests.ReadAllText("GetExpression_issue1749_decrement4"))
                        .Returns(";a--")
                        .SetName("a--");
                yield return
                    new TestCaseData(CodeCompleteTests.ReadAllText("GetExpression_issue1749_decrement5"))
                        .Returns("=getId()--")
                        .SetName("var id = getId()--|");
                yield return
                    new TestCaseData(CodeCompleteTests.ReadAllText("GetExpression_issue1954"))
                        .Returns(";re")
                        .SetName("function foo():Array<Int> { re|");
                yield return
                    new TestCaseData("(v:String).$(EntryPoint)")
                        .Returns(" (v:String).")
                        .SetName("(v:String).|");
                yield return
                    new TestCaseData("(v:Iterable<Dynamic>).$(EntryPoint)")
                        .Returns(" (v:Iterable<Dynamic>).")
                        .SetName("(v:Iterable<Dynamic>).|");
                yield return
                    new TestCaseData("(v:{x:Int, y:Int}).$(EntryPoint)")
                        .Returns(" (v:{x:Int, y:Int}).")
                        .SetName("(v:{x:Int, y:Int}).|");
                yield return
                    new TestCaseData("(v:{x:Int, y:Int->Array<Int>}).$(EntryPoint)")
                        .Returns(" (v:{x:Int, y:Int->Array<Int>}).")
                        .SetName("(v:{x:Int, y:Int->Array<Int>}).|");
                yield return
                    new TestCaseData("return (v:{x:Int, y:Int->Array<Int>}).$(EntryPoint)")
                        .Returns("return;(v:{x:Int, y:Int->Array<Int>}).")
                        .SetName("return (v:{x:Int, y:Int->Array<Int>}).|");
                yield return
                    new TestCaseData("[(v:{x:Int, y:Int->Array<Int>}).$(EntryPoint)")
                        .Returns(";(v:{x:Int, y:Int->Array<Int>}).")
                        .SetName("[(v:{x:Int, y:Int->Array<Int>}).|");
                yield return
                    new TestCaseData("${(v:{x:Int, y:Int->Array<Int>}).$(EntryPoint)")
                        .Returns(";(v:{x:Int, y:Int->Array<Int>}).")
                        .SetName("${(v:{x:Int, y:Int->Array<Int>}).|");
                yield return new TestCaseData("case _: (v:{x:Int, y:Int->Array<Int>}).$(EntryPoint)")
                    .Returns(":(v:{x:Int, y:Int->Array<Int>}).")
                    .SetName("case _: (v:{x:Int, y:Int->Array<Int>}).|");
                yield return new TestCaseData("function foo(Math.random() > .5 || Math.random() < 0.5 ? {x:10, y:10} : null).$(EntryPoint)")
                    .Returns("function foo(Math.random() > .5 || Math.random() < 0.5 ? {x:10, y:10} : null).")
                    .SetName("function foo(Math.random() > .5 || Math.random() < 0.5 ? {x:10, y:10} : null).|");
                yield return new TestCaseData("function foo(1 << 2).$(EntryPoint)")
                    .Returns("function foo(1 << 2).")
                    .SetName("function foo(1 << 2).|");
                yield return new TestCaseData("function foo(1 >> 2).$(EntryPoint)")
                    .Returns("function foo(1 >> 2).")
                    .SetName("function foo(1 >> 2).|");
                yield return new TestCaseData("function foo(1 >>> 3).$(EntryPoint)")
                    .Returns("function foo(1 >>> 3).")
                    .SetName("function foo(1 >>> 3).|");
                yield return new TestCaseData("cast(v, String).charAt(0).charAt(1).charAt(2).charAt(3).charAt(4).charAt(5).charAt(6).charAt(7).charAt(8).charAt(9).charAt(10).charAt(11).$(EntryPoint)")
                    .Returns("cast;(v, String).charAt(0).charAt(1).charAt(2).charAt(3).charAt(4).charAt(5).charAt(6).charAt(7).charAt(8).charAt(9).charAt(10).charAt(11).")
                    .SetName("cast(v, String).charAt(0).charAt(1).charAt(2).charAt(3).charAt(4).charAt(5).charAt(6).charAt(7).charAt(8).charAt(9).charAt(10).charAt(11).|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2108");
                yield return new TestCaseData("cast(v, String).charAt.$(EntryPoint)")
                    .Returns("cast;(v, String).charAt.")
                    .SetName("cast(v, String).charAt.|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2108");
            }
        }

        static IEnumerable<TestCaseData> GetExpressionIssue2386TestCases
        {
            get
            {
                yield return new TestCaseData("cast(v, String).charAt(1), '$(EntryPoint)")
                    .Returns(" ")
                    .SetName("cast(v, String).charAt(1), '|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2386");
                yield return new TestCaseData("cast(v, String).charAt(1), \"$(EntryPoint)")
                    .Returns(" ")
                    .SetName("cast(v, String).charAt(1), \"|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2386");
            }
        }

        [
            Test,
            TestCaseSource(nameof(GetExpressionTestCases)),
            TestCaseSource(nameof(GetExpressionIssue2386TestCases)),
        ]
        public string GetExpression(string sourceText) => GetExpression(sci, sourceText);

        static IEnumerable<TestCaseData> DisambiguateComaHaxeTestCases
        {
            get
            {
                yield return new TestCaseData("function test<K:(ArrayAccess, {})>(arg:").SetName("GenericFunctionArgumentType").SetDescription("This includes PR #963").Returns(ComaExpression.FunctionDeclaration);
                yield return new TestCaseData("function test<K:(ArrayAccess, {})>(arg:String, arg2").SetName("GenericFunctionArgument").SetDescription("This includes PR #963").Returns(ComaExpression.FunctionDeclaration);
                yield return new TestCaseData("class Generic<K,").SetName("GenericTypeParameterInClass").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                yield return new TestCaseData("function generic<K,").SetName("GenericTypeParameterInFunction").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                yield return new TestCaseData("class Generic<K:").SetName("GenericTypeParameterConstraintInClass").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                yield return new TestCaseData("class Generic<K:(").SetName("GenericTypeParameterConstraintMultipleInClass").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                yield return new TestCaseData("class Generic<K:({},").SetName("GenericTypeParameterConstraintMultipleInClassAfterFirst").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                yield return new TestCaseData("function generic<K:").SetName("GenericTypeParameterConstraintInFunction").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                yield return new TestCaseData("function generic<K:(").SetName("GenericTypeParameterConstraintMultipleInFunction").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                yield return new TestCaseData("function generic<K:({},").SetName("GenericTypeParameterConstraintMultipleInFunctionAfterFirst").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                yield return new TestCaseData("new Generic<Array<Int>,").SetName("GenericTypeParameterInDeclaration").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                yield return new TestCaseData("com.test.Generic<Array<Int>,").SetName("GenericTypeParameterInDeclarationWithFullyQualifiedClass").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                yield return new TestCaseData("var p:{x:").SetName("HaxeAnonymousStructureParameterType").Returns(ComaExpression.VarDeclaration);
                yield return new TestCaseData("var p:{?x:").SetName("HaxeAnonymousStructureOptionalParameterType").Returns(ComaExpression.VarDeclaration);
                yield return new TestCaseData("function p(arg:{x:").SetName("HaxeAnonymousStructureParameterTypeAsFunctionArg").Returns(ComaExpression.VarDeclaration);
                yield return new TestCaseData("function p(arg:{?x:").SetName("HaxeAnonymousStructureOptionalParameterTypeAsFunctionArg").Returns(ComaExpression.VarDeclaration);
                yield return new TestCaseData("function p(?arg:{?x:").SetName("HaxeAnonymousStructureOptionalParameterTypeAsFunctionOptionalArg").Returns(ComaExpression.VarDeclaration);
                yield return new TestCaseData("function test(arg:String, ?arg2").SetName("HaxeFunctionOptionalArgument").Returns(ComaExpression.FunctionDeclaration);
                yield return new TestCaseData("function test(?arg:").SetName("HaxeFunctionOptionalArgumentType").Returns(ComaExpression.FunctionDeclaration);
                yield return new TestCaseData("var a:String = (2 > 3) ?").SetName("HaxeTernaryOperatorTruePart").Returns(ComaExpression.AnonymousObject);
                yield return new TestCaseData("var a:String = (2 > 3) ? 'Hah' :").SetName("HaxeTernaryOperatorFalsePart").Returns(ComaExpression.AnonymousObject);
            }
        }

        [Test, TestCaseSource(nameof(DisambiguateComaHaxeTestCases))]
        public ComaExpression DisambiguateComa(string sourceText) => DisambiguateComa(sci, sourceText);

        static IEnumerable<TestCaseData> FindParameterIndexTestCases
        {
            get
            {
                yield return new TestCaseData("foo($(EntryPoint));function foo();")
                    .Returns(0);
                yield return new TestCaseData("foo(1, $(EntryPoint));function foo(x:Int, y:Int);")
                    .Returns(1);
                yield return new TestCaseData("foo([1,2,3,4,5], $(EntryPoint));function foo(x:Array<Int>, y:Int);")
                    .Returns(1);
                yield return new TestCaseData("foo(new Array<Int>(), $(EntryPoint));function foo(x:Array<Int>, y:Int);")
                    .Returns(1);
                yield return new TestCaseData("foo(new Map<Int, String>(), $(EntryPoint));function foo(x:Map<Int, String>, y:Int);")
                    .Returns(1);
                yield return new TestCaseData("foo({x:Int, y:Int}, $(EntryPoint));function foo(x:{x:Int, y:Int}, y:Int);")
                    .Returns(1);
                yield return new TestCaseData("foo(',,,,', $(EntryPoint));function foo(s:String, y:Int);")
                    .Returns(1);
                yield return new TestCaseData("foo(\",,,,\", $(EntryPoint));function foo(s:String, y:Int);")
                    .Returns(1);
                yield return new TestCaseData("foo(\"\\ \", $(EntryPoint));function foo(s:String, y:Int);")
                    .Returns(1);
                yield return new TestCaseData("foo(0, ';', $(EntryPoint));function foo(i:Int, s:String, y:Int);")
                    .Returns(2);
                yield return new TestCaseData("foo(0, '}}}', $(EntryPoint));function foo(i:Int, s:String, y:Int);")
                    .Returns(2);
                yield return new TestCaseData("foo(0, '<<>>><{}>}->({[]})>><<,,;>><<', $(EntryPoint));function foo(i:Int, s:String, y:Int);")
                    .Returns(2);
                yield return new TestCaseData("foo(0, '(', $(EntryPoint));function foo(i:Int, s:String, y:Int);")
                    .Returns(2);
                yield return new TestCaseData("foo(0, ')', $(EntryPoint));function foo(i:Int, s:String, y:Int);")
                    .Returns(2);
                yield return new TestCaseData("foo(0, {c:Array(<Int>->Map<String, Int>)->Void}, $(EntryPoint));function foo(i:Int, s:Dynamic, y:Int);")
                    .Returns(2);
                yield return new TestCaseData("foo(0, function(i:Int, s:String) {return Std.string(i) + ', ' + s;}, $(EntryPoint));function foo(i:Int, s:Dynamic, y:Int);")
                    .Returns(2);
                yield return new TestCaseData("foo(0, function() {var i = 1, j = 2; return i + j;}, $(EntryPoint));function foo(i:Int, s:Dynamic, y:Int);")
                    .Returns(2);
                yield return new TestCaseData("foo([1 => 1], [1, $(EntryPoint));")
                    .Returns(1);
                yield return new TestCaseData("foo([1 => 1], {x:1, y:$(EntryPoint));")
                    .Returns(1);
                yield return new TestCaseData("foo([1 => 1], bar({x:1, y:$(EntryPoint)));")
                    .Returns(0);
                yield return new TestCaseData("foo([1 => 1], $(EntryPoint));")
                    .Returns(1)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/764");
                yield return new TestCaseData("foo([for(i in 0...10) i], $(EntryPoint)")
                    .Returns(1)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/764");
                yield return new TestCaseData("foo(1, 1 > 2$(EntryPoint)")
                    .Returns(1)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2324");
                yield return new TestCaseData("foo(1, 1 < 2$(EntryPoint)")
                    .Returns(1)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2324");
                yield return new TestCaseData("foo(1, 1 << 2$(EntryPoint)")
                    .Returns(1)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2324");
                yield return new TestCaseData("foo(1, 1 >> 2$(EntryPoint)")
                    .Returns(1)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2324");
                yield return new TestCaseData("foo(1, 1 > 2, 1 < 2 ? 2 > 3 : $(EntryPoint)")
                    .Returns(2)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2324");
                yield return new TestCaseData("foo(1, 1 > 2, 1 < 2 ? 2 >>> 3 : $(EntryPoint)")
                    .Returns(2)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2324");
                yield return new TestCaseData("foo(1, bar(1, 2), 1 < 2$(EntryPoint)")
                    .Returns(2)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2324");
            }
        }

        static IEnumerable<TestCaseData> FindParameterIndexIssue2468TestCases
        {
            get
            {
                yield return new TestCaseData("new Vector<Foo>(1, bar(1, 2), 1 < 2$(EntryPoint)")
                    .Returns(2)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2468");
                yield return new TestCaseData("new Vector<Foo>(1, bar(1, 2), 1 > 2$(EntryPoint)")
                    .Returns(2)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2468");
            }
        }

        [
            Test,
            TestCaseSource(nameof(FindParameterIndexTestCases)),
            TestCaseSource(nameof(FindParameterIndexIssue2468TestCases)),
        ]
        public int FindParameterIndex(string sourceText) => FindParameterIndex(sci, sourceText);

        static IEnumerable<TestCaseData> ParseClass_Issue104TestCases
        {
            get
            {
                yield return new TestCaseData("Iterable", FlagType.Class | FlagType.TypeDef)
                    .Returns("\n\tAn `Iterable` is a data structure which has an `iterator()` method.\n\tSee `Lambda` for generic functions on iterable structures.\n\n\t@see https://haxe.org/manual/lf-iterators.html\n")
                    .SetName("Issue 104. typedef");
                yield return new TestCaseData("Any", FlagType.Class | FlagType.Abstract)
                    .Returns("\n\t`Any` is a type that is compatible with any other in both ways.\n\n\tThis means that a value of any type can be assigned to `Any`, and\n\tvice-versa, a value of `Any` type can be assigned to any other type.\n\n\tIt's a more type-safe alternative to `Dynamic`, because it doesn't\n\tsupport field access or operators and it's bound to monomorphs. So,\n\tto work with the actual value, it needs to be explicitly promoted\n\tto another type.\n")
                    .SetName("Issue 104. abstract");
            }
        }

        [Test, TestCaseSource(nameof(ParseClass_Issue104TestCases))]
        public string ParseFile_Issue104(string name, FlagType flags)
        {
            var list = ASContext.Context.GetVisibleExternalElements();
            var result = list.Search(name, flags, 0);
            return result.Comments;
        }

        static IEnumerable<TestCaseData> ExpressionEndPositionTestCases
        {
            get
            {
                yield return new TestCaseData("test(); //")
                    .Returns("test()".Length);
                yield return new TestCaseData("test[1]; //")
                    .Returns("test[1]".Length);
                yield return new TestCaseData("test['1']; //")
                    .Returns("test['1']".Length);
                yield return new TestCaseData("x:10, y:10}; //")
                    .Returns("x".Length);
                yield return new TestCaseData("test()); //")
                    .Returns("test()".Length);
                yield return new TestCaseData("test()]; //")
                    .Returns("test()".Length);
                yield return new TestCaseData("test()}; //")
                    .Returns("test()".Length);
                yield return new TestCaseData("test[1]); //")
                    .Returns("test[1]".Length);
                yield return new TestCaseData("test(), 1, 2); //")
                    .Returns("test()".Length);
                yield return new TestCaseData("test().test().test().test; //")
                    .Returns("test()".Length);
                yield return new TestCaseData("test(')))))').test(']]]]]]]]').test('}}}}}}}}}}').test; //")
                    .Returns("test(')))))')".Length);
                yield return new TestCaseData("test.test(')))))')\n.test(']]]]]]]]')\n.test('}}}}}}}}}}')\n.test; //")
                    .Returns("test".Length);
                yield return new TestCaseData("test(function() return 10); //")
                    .Returns("test(function() return 10)".Length);
                yield return new TestCaseData("test(1, 2, 3); //")
                    .Returns("test(1, 2, 3)".Length);
                yield return new TestCaseData("test( new Map<K,V> ); //")
                    .Returns("test( new Map<K,V> )".Length);
                yield return new TestCaseData("Map<K,V> ; //")
                    .Returns("Map".Length);
                yield return new TestCaseData("test; //")
                    .Returns("test".Length);
                yield return new TestCaseData("test.a.b.c.d().e(1).f('${g}').h([1 => {x:1}]); //")
                    .Returns("test".Length);
                yield return new TestCaseData("test(/*12345*/); //")
                    .Returns("test(/*12345*/)".Length);
                yield return new TestCaseData("test(Math.random() > 0.5 ? 1 : 1); //")
                    .Returns("test(Math.random() > 0.5 ? 1 : 1)".Length);
                yield return new TestCaseData("[1,2,3,4]; //")
                    .Returns("[1,2,3,4]".Length);
                yield return new TestCaseData("{v:1}; //")
                    .Returns("{v:1}".Length);
                yield return new TestCaseData("{v:[1]}; //")
                    .Returns("{v:[1]}".Length);
                yield return new TestCaseData("[{v:[1]}]; //")
                    .Returns("[{v:[1]}]".Length);
                yield return new TestCaseData("(v:{v:Int}); //")
                    .Returns("(v:{v:Int})".Length);
                yield return new TestCaseData("[(v:{v:Int})]; //")
                    .Returns("[(v:{v:Int})]".Length);
                yield return new TestCaseData("[function() {return 1;}]; //")
                    .Returns("[function() {return 1;}]".Length);
                yield return new TestCaseData("'12345'; //")
                    .Returns("'12345'".Length);
                yield return new TestCaseData("'${1$(EntryPoint)2345}'; //")
                    .Returns("'${12345".Length);
                yield return new TestCaseData("-1; //")
                    .Returns("-1".Length);
                yield return new TestCaseData("--1; //")
                    .Returns("--1".Length);
                yield return new TestCaseData("+1; //")
                    .Returns("+1".Length);
                yield return new TestCaseData("++1; //")
                    .Returns("++1".Length);
                yield return new TestCaseData("5e-324; //")
                    .Returns("5e-324".Length);
                yield return new TestCaseData("2.225e-308; //")
                    .Returns("2.225e-308".Length);
                yield return new TestCaseData("~/}])\"\'/; //")
                    .Returns("~/}])\"\'/".Length);
                yield return new TestCaseData("function foo$(EntryPoint)(v:String) { return null;} //")
                    .Returns("function foo".Length);
                yield return new TestCaseData("var foo$(EntryPoint)(null, set) //")
                    .Returns("var foo".Length);
                yield return new TestCaseData("abstract AFoo$(EntryPoint)(Int) //")
                    .Returns("abstract AFoo".Length);
                yield return new TestCaseData("'${$(EntryPoint)array[index]}'; //")
                    .Returns("'${array[index]".Length);
                yield return new TestCaseData("'$$(EntryPoint)array[index]'; //")
                    .Returns("'$array".Length);
                yield return new TestCaseData("'$$(EntryPoint)array.length'; //")
                    .Returns("'$array".Length);
                yield return new TestCaseData("'$$(EntryPoint)array(list)'; //")
                    .Returns("'$array".Length);
                yield return new TestCaseData("'\n\n\n'; //")
                    .Returns("'\n\n\n'".Length);
                yield return new TestCaseData("'\n\n[{(\n'; //")
                    .Returns("'\n\n[{(\n'".Length);
                yield return new TestCaseData("\"\n\n\n\"; //")
                    .Returns("\"\n\n\n\"".Length);
            }
        }

        [Test, TestCaseSource(nameof(ExpressionEndPositionTestCases))]
        public int ExpressionEndPosition(string sourceText) => ExpressionEndPosition(sci, sourceText);
    }
}