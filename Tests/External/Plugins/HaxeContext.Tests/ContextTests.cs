using System.Collections.Generic;
using System.Linq;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using HaXeContext.TestUtils;
using NUnit.Framework;
using PluginCore;

namespace HaXeContext
{
    [TestFixture]
    class ContextTests : ASCompleteTests
    {
        static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

        static string GetFullPath(string fileName) => $"{nameof(HaXeContext)}.Test_Files.parser.{fileName}.hx";

        [TestFixtureSetUp]
        public void ContextTestsSetUp() => SetHaxeFeatures(sci);

        static IEnumerable<TestCaseData> DecomposeTypesTestCases
        {
            get
            {
                yield return new TestCaseData(new List<string> {"haxe.Timer->Type.ValueType"})
                    .SetName("haxe.Timer->Type.ValueType")
                    .Returns(new[] {"haxe.Timer", "Type.ValueType"});
                yield return new TestCaseData(new List<string> {"{t:haxe.Timer, v:Type.ValueType}"})
                    .SetName("{t:haxe.Timer, v:Type.ValueType}")
                    .Returns(new[] {"haxe.Timer", "Type.ValueType"});
                yield return new TestCaseData(new List<string> {"{t:haxe.Timer}->Type.ValueType"})
                    .SetName("{t:haxe.Timer}->Type.ValueType")
                    .Returns(new[] {"haxe.Timer", "Type.ValueType"});
                yield return new TestCaseData(new List<string> {"{t:haxe.Timer->Type.ValueType}"})
                    .SetName("{t:haxe.Timer->Type.ValueType}")
                    .Returns(new[] {"haxe.Timer", "Type.ValueType"});
                yield return new TestCaseData(new List<string> {"{t:haxe.Timer->Type.ValueType->haxe.ds.Vector<Int>}"})
                    .SetName("{t:haxe.Timer->Type.ValueType->haxe.ds.Vector<Int>}")
                    .Returns(new[] {"haxe.Timer", "Type.ValueType", "haxe.ds.Vector", "Int"});
                yield return new TestCaseData(new List<string> {"{t:haxe.Timer->Type.ValueType}->haxe.ds.Vector<Int>"})
                    .SetName("{t:haxe.Timer->Type.ValueType}->haxe.ds.Vector<Int>")
                    .Returns(new[] {"haxe.Timer", "Type.ValueType", "haxe.ds.Vector", "Int"});
                yield return new TestCaseData(new List<string> {"haxe.Timer->Type.ValueType->{v:haxe.ds.Vector<Int>}"})
                    .SetName("haxe.Timer->Type.ValueType->{v:haxe.ds.Vector<Int>}")
                    .Returns(new[] {"haxe.Timer", "Type.ValueType", "haxe.ds.Vector", "Int"});
                yield return new TestCaseData(new List<string> {"haxe.Timer->haxe.Timer"})
                    .SetName("haxe.Timer->haxe.Timer")
                    .Returns(new[] {"haxe.Timer"});
                yield return new TestCaseData(new List<string> {"haxe.ds.Vector<{c:haxe.Timer->Type.ValueType}>"})
                    .SetName("haxe.ds.Vector<{c:haxe.Timer->Type.ValueType}>")
                    .Returns(new[] {"haxe.ds.Vector", "haxe.Timer", "Type.ValueType"});
                yield return new TestCaseData(new List<string> {"Array<Int>->Map<String, Int>->haxe.ds.Vector<{c:haxe.Timer->Type.ValueType}>"})
                    .SetName("Array<Int>->Map<String, Int>->haxe.ds.Vector<{c:haxe.Timer->Type.ValueType}>")
                    .Returns(new[] {"Array", "Int", "Map", "String", "haxe.ds.Vector", "haxe.Timer", "Type.ValueType"});
                yield return new TestCaseData(new List<string> {"Array<Array<Map<haxe.ds.Vector<haxe.Timer>, Type.ValueType>>>"})
                    .SetName("Array<Array<Map<haxe.ds.Vector<haxe.Timer>, Type.ValueType>>>")
                    .Returns(new[] {"Array", "Map", "haxe.ds.Vector", "haxe.Timer", "Type.ValueType"});
                yield return new TestCaseData(new List<string> {"Array<Array<Map<haxe.ds.Vector<haxe.Timer>,Type.ValueType>>>"})
                    .SetName("Array<Array<Map<haxe.ds.Vector<haxe.Timer>,Type.ValueType>>>")
                    .Returns(new[] {"Array", "Map", "haxe.ds.Vector", "haxe.Timer", "Type.ValueType"});
                yield return new TestCaseData(new List<string> {"Array<Array<Map<haxe.ds.Vector<{t:haxe.Timer}>, Type.ValueType>>>"})
                    .SetName("Array<Array<Map<haxe.ds.Vector<{t:haxe.Timer}>, Type.ValueType>>>")
                    .Returns(new[] {"Array", "Map", "haxe.ds.Vector", "haxe.Timer", "Type.ValueType"});
                yield return new TestCaseData(new List<string> {"Array<Array<Map<haxe.ds.Vector<{t:haxe.Timer}>->Type.ValueType, Int>>>"})
                    .SetName("Array<Array<Map<haxe.ds.Vector<{t:haxe.Timer}>->Type.ValueType, Int>>>")
                    .Returns(new[] {"Array", "Map", "haxe.ds.Vector", "haxe.Timer", "Type.ValueType", "Int"});
            }
        }

        [Test, TestCaseSource(nameof(DecomposeTypesTestCases))]
        public IEnumerable<string> DecomposeTypes(IEnumerable<string> types) => ASContext.Context.DecomposeTypes(types);

        static IEnumerable<TestCaseData> ParseFile_Issue1849TestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("Issue1849_1"))
                    .Returns("Dynamic<T>")
                    .SetName("implements Dynamic<T>")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1849");
                yield return new TestCaseData(ReadAllText("Issue1849_2"))
                    .Returns("IStruct<T>")
                    .SetName("implements IStruct<T>")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1849");
                yield return new TestCaseData(ReadAllText("Issue1849_3"))
                    .Returns("IStruct<K,V>")
                    .SetName("implements IStruct<K,V>")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1849");
            }
        }

        [Test, TestCaseSource(nameof(ParseFile_Issue1849TestCases))]
        public string ParseFile_Issue1849(string sourceText)
        {
            var model = ASContext.Context.GetCodeModel(sourceText);
            var interfaceType = ASContext.Context.ResolveType(model.Classes.First().Implements.First(), model);
            return interfaceType.Type;
        }

        static IEnumerable<TestCaseData> ResolveDotContext_Issue750TestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("ResolveDotContext_Issue1926_1"), null)
                    .SetName("case 1");
                yield return new TestCaseData(ReadAllText("ResolveDotContext_Issue1926_2"), new MemberModel("code", "Int", FlagType.Getter, Visibility.Public))
                    .SetName("case 2");
                yield return new TestCaseData(ReadAllText("ResolveDotContext_Issue1926_3"), new MemberModel("code", "Int", FlagType.Getter, Visibility.Public))
                    .SetName("case 3");
                yield return new TestCaseData(ReadAllText("ResolveDotContext_Issue1926_4"), null)
                    .SetName("case 4");
                yield return new TestCaseData(ReadAllText("ResolveDotContext_Issue1926_5"), new MemberModel("code", "Int", FlagType.Getter, Visibility.Public))
                    .SetName("case 5");
                yield return new TestCaseData(ReadAllText("ResolveDotContext_Issue1926_6"), new MemberModel("code", "Int", FlagType.Getter, Visibility.Public))
                    .SetName("case 6");
            }
        }

        [Test, TestCaseSource(nameof(ResolveDotContext_Issue750TestCases))]
        public void ResolveDotContext_issue750(string sourceText, MemberModel code)
        {
            ((HaXeSettings)ASContext.Context.Settings).CompletionMode = HaxeCompletionModeEnum.FlashDevelop;
            SetSrc(sci, sourceText);
            var mix = new MemberList();
            var expr = ASComplete.GetExpression(sci, sci.CurrentPos);
            ASContext.Context.ResolveDotContext(sci, expr, mix);
            Assert.AreEqual(code, mix.Items.FirstOrDefault());
        }

        static IEnumerable<TestCaseData> IsImportedTestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("IsImported_case1"))
                    .Returns(true)
                    .SetName("Case 1");
                yield return new TestCaseData(ReadAllText("IsImported_case2"))
                    .Returns(false)
                    .SetName("Case 2");
                yield return new TestCaseData(null)
                    .Returns(false)
                    .SetName("ClassModel.VoidClass")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1930");
                yield return new TestCaseData(ReadAllText("IsImported_issue1969_1"))
                    .Returns(true)
                    .SetName("Issue 1969. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1969");
                yield return new TestCaseData(ReadAllText("IsImported_issue1969_2"))
                    .Returns(true)
                    .SetName("Issue 1969. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1969");
            }
        }

        [Test, TestCaseSource(nameof(IsImportedTestCases))]
        public bool IsImported(string sourceText)
        {
            MemberModel member;
            if (sourceText != null)
            {
                SetSrc(sci, sourceText);
                var expr = ASComplete.GetExpressionType(sci, ASComplete.ExpressionEndPosition(sci, sci.CurrentPos), false, true);
                if (expr.Type != null) member = expr.Type;
                else
                {
                    var type = sci.GetWordFromPosition(sci.CurrentPos);
                    member = new MemberModel(type, type, FlagType.Class, Visibility.Public);
                }
            }
            else member = ClassModel.VoidClass;
            return ASContext.Context.IsImported(member, sci.CurrentLine);
        }

        static IEnumerable<TestCaseData> ResolveTokenTestCases
        {
            get
            {
                yield return new TestCaseData("true", "3.4.0")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore})
                    .SetName("true");
                yield return new TestCaseData("false", "3.4.0")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore})
                    .SetName("false");
                yield return new TestCaseData("{}", "3.4.0")
                    .Returns(new ClassModel {Name = "Dynamic", Type = "Dynamic", InFile = FileModel.Ignore})
                    .SetName("{}");
                yield return new TestCaseData("10", "3.4.0")
                    .Returns(new ClassModel {Name = "Float", Type = "Float", InFile = FileModel.Ignore})
                    .SetName("10");
                yield return new TestCaseData("-10", "3.4.0")
                    .Returns(new ClassModel {Name = "Float", Type = "Float", InFile = FileModel.Ignore})
                    .SetName("-10");
                yield return new TestCaseData("\"\"", "3.4.0")
                    .Returns(new ClassModel {Name = "String", Type = "String", InFile = FileModel.Ignore})
                    .SetName("\"\"");
                yield return new TestCaseData("''", "3.4.0")
                    .Returns(new ClassModel {Name = "String", Type = "String", InFile = FileModel.Ignore})
                    .SetName("''");
                yield return new TestCaseData("0xFF0000", "3.4.0")
                    .Returns(new ClassModel {Name = "Int", Type = "Int", InFile = FileModel.Ignore})
                    .SetName("0xFF0000");
                yield return new TestCaseData("[]", "3.4.0")
                    .Returns(new ClassModel {Name = "Array<T>", Type = "Array<T>", InFile = FileModel.Ignore})
                    .SetName("[]");
                yield return new TestCaseData("[1 => 1]", "3.4.0")
                    .Returns(new ClassModel {Name = "Map<K, V>", Type = "Map<K, V>", InFile = FileModel.Ignore})
                    .SetName("[1 => 1]");
                yield return new TestCaseData("(v:String)", "3.4.0")
                    .Returns(new ClassModel {Name = "String", Type = "String", InFile = FileModel.Ignore})
                    .SetName("(v:String). Haxe 3.4.0");
                yield return new TestCaseData("(v:Map<Dynamic, Dynamic>)", "3.4.0")
                    .Returns(new ClassModel {Name = "Map<Dynamic,Dynamic>", Type = "Map<Dynamic,Dynamic>", InFile = FileModel.Ignore})
                    .SetName("(v:Map<Dynamic, Dynamic>). Haxe 3.4.0");
                yield return new TestCaseData("(v:Map<Dynamic, {x:Int}>)", "3.4.0")
                    .Returns(new ClassModel {Name = "Map<Dynamic,{x:Int}>", Type = "Map<Dynamic,{x:Int}>", InFile = FileModel.Ignore})
                    .SetName("(v:Map<Dynamic, {x:Int}>). Haxe 3.4.0");
                yield return new TestCaseData("(v:String)", "3.0.0")
                    .Returns(ClassModel.VoidClass)
                    .SetName("(v:String). Haxe 3.0.0");
                yield return new TestCaseData("new Sprite().addChild(new Sprite())", "3.0.0")
                    .Returns(ClassModel.VoidClass);
                yield return new TestCaseData("new String('1')", "3.0.0")
                    .Returns(new ClassModel {Name = "String", Type = "String", InFile = FileModel.Ignore})
                    .SetName("new String('1')");
                yield return new TestCaseData("(v is String)", "3.4.0")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore})
                    .SetName("(v is String)");
                yield return new TestCaseData("(['is'] is Array)", "3.4.0")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore})
                    .SetName("(['is'] is Array)");
                yield return new TestCaseData("(' is string' is String)", "3.4.0")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore})
                    .SetName("(' is string' is String)");
                yield return new TestCaseData("({x:Int, y:Int} is Point)", "3.4.0")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore})
                    .SetName("({x:Int, y:Int} is Point)");
                yield return new TestCaseData("('   is  ' is Array)", "3.4.0")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore})
                    .SetName("('   is  ' is Array)");
                yield return new TestCaseData("('   is  '   is  Array)", "3.4.0")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore})
                    .SetName("('   is  '   is  Array)");
                yield return new TestCaseData("cast('s', String)", "3.4.0")
                    .Returns(new ClassModel {Name = "String", Type = "String", InFile = FileModel.Ignore})
                    .SetName("cast('s', String)");
                yield return new TestCaseData("cast(v, Array<Dynamic>)", "3.4.0")
                    .Returns(new ClassModel {Name = "Array<Dynamic>", Type = "Array<Dynamic>", InFile = FileModel.Ignore})
                    .SetName("cast(v, Array<Dynamic>)");
                yield return new TestCaseData("cast(v, Map<Dynamic, Dynamic>)", "3.4.0")
                    .Returns(new ClassModel {Name = "Map<Dynamic,Dynamic>", Type = "Map<Dynamic,Dynamic>", InFile = FileModel.Ignore})
                    .SetName("cast(v, Map<Dynamic, Dynamic>)");
            }
        }

        [Test, TestCaseSource(nameof(ResolveTokenTestCases))]
        public ClassModel ResolveToken(string token, string sdkVersion)
        {
            ASContext.Context.Settings.InstalledSDKs = new[] {new InstalledSDK {Path = PluginBase.CurrentProject.CurrentSDK, Version = sdkVersion}};
            return ASContext.Context.ResolveToken(token, null);
        }

        static IEnumerable<TestCaseData> GetTopLevelElementsTestCases
        {
            get
            {
                yield return new TestCaseData("GetTopLevelElements_1", new MemberModel("Foo", string.Empty, FlagType.Enum | FlagType.Static | FlagType.Variable, Visibility.Public))
                    .Returns(true)
                    .SetName("Case 1. enum");
                yield return new TestCaseData("GetTopLevelElements_2", new MemberModel("Foo", string.Empty, FlagType.Enum | FlagType.Static | FlagType.Variable, Visibility.Public))
                    .Returns(true)
                    .SetName("Case 2. @:enum abstract");
                yield return new TestCaseData("GetTopLevelElements_3", new MemberModel("toString", string.Empty, FlagType.Function, Visibility.Public))
                    .Returns(false)
                    .SetName("Case 3. @:enum abstract without variables");
            }
        }

        [Test, TestCaseSource(nameof(GetTopLevelElementsTestCases))]
        public bool GetTopLevelElements(string fileName, MemberModel member)
        {
            SetSrc(sci, ReadAllText(fileName));
            var context = ((ASContext) ASContext.GetLanguageContext("haxe"));
            context.CurrentModel = ASContext.Context.CurrentModel;
            context.completionCache.IsDirty = true;
            var topLevelElements = context.GetTopLevelElements();
            return topLevelElements.Items.Contains(member);
        }

        static IEnumerable<TestCaseData> ResolveTopLevelElementTestCases
        {
            get
            {
                yield return new TestCaseData("ResolveTopLevelElement_enum")
                    .Returns(new MemberModel("EFoo", "Foo", FlagType.Enum | FlagType.Static | FlagType.Variable, Visibility.Public));
                yield return new TestCaseData("ResolveTopLevelElement_abstract")
                    .Returns(new MemberModel("EFoo", "Foo", FlagType.Enum | FlagType.Static | FlagType.Variable, Visibility.Public));
            }
        }

        [Test, TestCaseSource(nameof(ResolveTopLevelElementTestCases))]
        public MemberModel ResolveTopLevelElement(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            var context = (Context)ASContext.GetLanguageContext("haxe");
            context.CurrentModel = ASContext.Context.CurrentModel;
            context.completionCache.IsDirty = true;
            context.GetVisibleExternalElements();
            var result = new ASResult();
            context.ResolveTopLevelElement("EFoo", result);
            return result.Member;
        }
    }
}
