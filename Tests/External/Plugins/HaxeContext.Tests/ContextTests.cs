using System.Collections.Generic;
using System.Linq;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using HaXeContext.TestUtils;
using NSubstitute;
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

        static IEnumerable<TestCaseData> ParseFileIssue1849TestCases
        {
            get
            {
                yield return new TestCaseData("Issue1849_1")
                    .Returns("Dynamic<T>")
                    .SetName("implements Dynamic<T>")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1849");
                yield return new TestCaseData("Issue1849_2")
                    .Returns("IStruct<T>")
                    .SetName("implements IStruct<T>")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1849");
                yield return new TestCaseData("Issue1849_3")
                    .Returns("IStruct<K,V>")
                    .SetName("implements IStruct<K,V>")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1849");
            }
        }

        [Test, TestCaseSource(nameof(ParseFileIssue1849TestCases))]
        public string ParseFileIssue1849(string fileName)
        {
            var sourceText = ReadAllText(fileName);
            var model = ASContext.Context.GetCodeModel(sourceText);
            var interfaceType = ASContext.Context.ResolveType(model.Classes.First().Implements.First(), model);
            return interfaceType.Type;
        }

        static IEnumerable<TestCaseData> ResolveDotContextIssue750TestCases
        {
            get
            {
                yield return new TestCaseData("ResolveDotContext_Issue1926_1", null)
                    .SetName("case 1");
                yield return new TestCaseData("ResolveDotContext_Issue1926_2", new MemberModel("code", "Int", FlagType.Getter, Visibility.Public))
                    .SetName("case 2");
                yield return new TestCaseData("ResolveDotContext_Issue1926_3", new MemberModel("code", "Int", FlagType.Getter, Visibility.Public))
                    .SetName("case 3");
                yield return new TestCaseData("ResolveDotContext_Issue1926_4", null)
                    .SetName("case 4");
                yield return new TestCaseData("ResolveDotContext_Issue1926_5", new MemberModel("code", "Int", FlagType.Getter, Visibility.Public))
                    .SetName("case 5");
                yield return new TestCaseData("ResolveDotContext_Issue1926_6", new MemberModel("code", "Int", FlagType.Getter, Visibility.Public))
                    .SetName("case 6");
            }
        }

        [Test, TestCaseSource(nameof(ResolveDotContextIssue750TestCases))]
        public void ResolveDotContextIssue750(string fileName, MemberModel code)
        {
            ((HaXeSettings)ASContext.Context.Settings).CompletionMode = HaxeCompletionModeEnum.FlashDevelop;
            SetSrc(sci, ReadAllText(fileName));
            var mix = new MemberList();
            var expr = ASComplete.GetExpressionType(sci, sci.CurrentPos);
            ASContext.Context.ResolveDotContext(sci, expr, mix);
            Assert.AreEqual(code, mix.Items.FirstOrDefault());
        }

        static IEnumerable<TestCaseData> ResolveDotContextIssue2467TestCases
        {
            get
            {
                yield return new TestCaseData("ResolveDotContext_issue2467_1", true)
                    .SetName("haxe.macro.Expr.<complete> ResolveDotContext. Issue 2467. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2467");
                yield return new TestCaseData("ResolveDotContext_issue2467_2", false)
                    .SetName("haxe.macro.<complete> ResolveDotContext. Issue 2467. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2467");
                yield return new TestCaseData("ResolveDotContext_issue2467_3", false)
                    .SetName("staticVar.<complete> ResolveDotContext. Issue 2467. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2467");
                yield return new TestCaseData("ResolveDotContext_issue2467_4", true)
                    .SetName("CurrentClass.<complete> ResolveDotContext. Issue 2467. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2467");
                yield return new TestCaseData("ResolveDotContext_issue2467_5", false)
                    .SetName("SubType.<complete> ResolveDotContext. Issue 2467. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2467");
                yield return new TestCaseData("ResolveDotContext_issue2467_6", true)
                    .SetName("CurrentAbstract.<complete> ResolveDotContext. Issue 2467. Case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2467");
                yield return new TestCaseData("ResolveDotContext_issue2467_7", true)
                    .SetName("CurrentInterface.<complete> ResolveDotContext. Issue 2467. Case 7")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2467");
                yield return new TestCaseData("ResolveDotContext_issue2467_8", true)
                    .SetName("CurrentEnum.<complete> ResolveDotContext. Issue 2467. Case 8")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2467");
                yield return new TestCaseData("ResolveDotContext_issue2467_9", true)
                    .SetName("CurrentTypedef.<complete> ResolveDotContext. Issue 2467. Case 9")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2467");
            }
        }

        [Test, TestCaseSource(nameof(ResolveDotContextIssue2467TestCases))]
        public void ResolveDotContextIssue2467(string fileName, bool resultIsNotEmpty)
        {
            ((HaXeSettings)ASContext.Context.Settings).CompletionMode = HaxeCompletionModeEnum.FlashDevelop;
            ASContext.Context.CurrentModel.FileName = fileName;
            SetSrc(sci, ReadAllText(fileName));
            var result = new MemberList();
            var expr = ASComplete.GetExpressionType(sci, sci.CurrentPos);
            ASContext.Context.ResolveDotContext(sci, expr, result);
            if (resultIsNotEmpty) Assert.IsNotEmpty(result);
            else Assert.IsEmpty(result);
        }

        static IEnumerable<TestCaseData> IsImportedTestCases
        {
            get
            {
                yield return new TestCaseData("IsImported_case1")
                    .Returns(true)
                    .SetName("Case 1");
                yield return new TestCaseData("IsImported_case2")
                    .Returns(false)
                    .SetName("Case 2");
                yield return new TestCaseData(null)
                    .Returns(false)
                    .SetName("ClassModel.VoidClass")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1930");
                yield return new TestCaseData("IsImported_issue1969_1")
                    .Returns(true)
                    .SetName("Issue 1969. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1969");
                yield return new TestCaseData("IsImported_issue1969_2")
                    .Returns(true)
                    .SetName("Issue 1969. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1969");
                yield return new TestCaseData("IsImported_Issue2339_1")
                    .Returns(false)
                    .SetName("Issue 2339. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2339");
                yield return new TestCaseData("IsImported_Issue2339_2")
                    .Returns(true)
                    .SetName("Issue 2339. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2339");
            }
        }

        [Test, TestCaseSource(nameof(IsImportedTestCases))]
        public bool IsImported(string fileName)
        {
            MemberModel member = ClassModel.VoidClass;
            if (fileName != null)
            {
                var sourceText = ReadAllText(fileName);
                SetSrc(sci, sourceText);
                var expr = ASComplete.GetExpressionType(sci, ASComplete.ExpressionEndPosition(sci, sci.CurrentPos), false, true);
                if (expr.Type != null) member = expr.Type;
                else
                {
                    var type = sci.GetWordFromPosition(sci.CurrentPos);
                    member = ASContext.Context.ResolveType(type, ASContext.Context.CurrentModel);
                }
            }
            return ASContext.Context.IsImported(member, sci.CurrentLine);
        }

        static IEnumerable<TestCaseData> ResolveTokenTestCases
        {
            get
            {
                yield return new TestCaseData("true", "3.4.0")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore});
                yield return new TestCaseData("false", "3.4.0")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore});
                yield return new TestCaseData("{}", "3.4.0")
                    .Returns(new ClassModel {Name = "Dynamic", Type = "Dynamic", InFile = FileModel.Ignore});
                yield return new TestCaseData("10.0", "3.4.0")
                    .Returns(new ClassModel {Name = "Float", Type = "Float", InFile = FileModel.Ignore});
                yield return new TestCaseData("-10.0", "3.4.0")
                    .Returns(new ClassModel {Name = "Float", Type = "Float", InFile = FileModel.Ignore});
                yield return new TestCaseData("10", "3.4.0")
                    .Returns(new ClassModel {Name = "Int", Type = "Int", InFile = FileModel.Ignore});
                yield return new TestCaseData("-10", "3.4.0")
                    .Returns(new ClassModel {Name = "Int", Type = "Int", InFile = FileModel.Ignore});
                yield return new TestCaseData("\"\"", "3.4.0")
                    .Returns(new ClassModel {Name = "String", Type = "String", InFile = FileModel.Ignore});
                yield return new TestCaseData("\"", "3.4.0")
                    .Returns(ClassModel.VoidClass);
                yield return new TestCaseData("''", "3.4.0")
                    .Returns(new ClassModel {Name = "String", Type = "String", InFile = FileModel.Ignore});
                yield return new TestCaseData("'", "3.4.0")
                    .Returns(ClassModel.VoidClass);
                yield return new TestCaseData("0xFF0000", "3.4.0")
                    .Returns(new ClassModel {Name = "Int", Type = "Int", InFile = FileModel.Ignore});
                yield return new TestCaseData("[]", "3.4.0")
                    .Returns(new ClassModel {Name = "Array<T>", Type = "Array<T>", InFile = FileModel.Ignore});
                yield return new TestCaseData("[1 => 1]", "3.4.0")
                    .Returns(new ClassModel {Name = "Map<K, V>", Type = "Map<K, V>", InFile = FileModel.Ignore});
                yield return new TestCaseData("(v:String)", "3.4.0")
                    .Returns(new ClassModel {Name = "String", Type = "String", InFile = FileModel.Ignore});
                yield return new TestCaseData("(v:Map<Dynamic, Dynamic>)", "3.4.0")
                    .Returns(new ClassModel {Name = "Map<Dynamic,Dynamic>", Type = "Map<Dynamic,Dynamic>", InFile = FileModel.Ignore});
                yield return new TestCaseData("(v:Map<Dynamic, {x:Int}>)", "3.4.0")
                    .Returns(new ClassModel {Name = "Map<Dynamic,{x:Int}>", Type = "Map<Dynamic,{x:Int}>", InFile = FileModel.Ignore});
                yield return new TestCaseData("(v:String)", "3.0.0")
                    .Returns(ClassModel.VoidClass);
                yield return new TestCaseData("new Sprite().addChild(new Sprite())", "3.0.0")
                    .Returns(ClassModel.VoidClass);
                yield return new TestCaseData("new String('1')", "3.0.0")
                    .Returns(new ClassModel {Name = "String", Type = "String", InFile = FileModel.Ignore});
                yield return new TestCaseData("(v is String)", "3.4.0")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore});
                yield return new TestCaseData("(['is'] is Array)", "3.4.0")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore});
                yield return new TestCaseData("(' is string' is String)", "3.4.0")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore});
                yield return new TestCaseData("({x:Int, y:Int} is Point)", "3.4.0")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore});
                yield return new TestCaseData("('   is  ' is Array)", "3.4.0")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore});
                yield return new TestCaseData("('   is  '   is  Array)", "3.4.0")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore});
                yield return new TestCaseData("cast('s', String)", "3.4.0")
                    .Returns(new ClassModel {Name = "String", Type = "String", InFile = FileModel.Ignore});
                yield return new TestCaseData("cast(v, Array<Dynamic>)", "3.4.0")
                    .Returns(new ClassModel {Name = "Array<Dynamic>", Type = "Array<Dynamic>", InFile = FileModel.Ignore});
                yield return new TestCaseData("cast(v, Map<Dynamic, Dynamic>)", "3.4.0")
                    .Returns(new ClassModel {Name = "Map<Dynamic,Dynamic>", Type = "Map<Dynamic,Dynamic>", InFile = FileModel.Ignore});
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

        static IEnumerable<TestCaseData> ParseFileIssue1150_1_TestCases
        {
            get
            {
                yield return new TestCaseData("Issue1150_1")
                    .Returns(new List<MemberModel> {new MemberModel("lpad", "String", FlagType.Access | FlagType.Static | FlagType.Function, Visibility.Public)})
                    .SetName("Import static member. Issue 1150. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1150");
                yield return new TestCaseData("Issue1150_2")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("rpad", "String", FlagType.Access | FlagType.Static | FlagType.Function, Visibility.Public),
                        new MemberModel("lpad", "String", FlagType.Access | FlagType.Static | FlagType.Function, Visibility.Public),
                    })
                    .SetName("Import static member. Issue 1150. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1150");
                yield return new TestCaseData("Issue1150_3")
                    .Returns(new List<MemberModel> {new MemberModel("PI", "Float", FlagType.Static | FlagType.Getter | FlagType.Setter, Visibility.Public)})
                    .SetName("Import static member. Issue 1150. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1150");
            }
        }

        [Test, TestCaseSource(nameof(ParseFileIssue1150_1_TestCases))]
        public List<MemberModel> ParseFileIssue1150_1(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            var context = (Context)ASContext.GetLanguageContext("haxe");
            context.CurrentModel = ASContext.Context.CurrentModel;
            ASContext.Context.ResolveImports(null).ReturnsForAnyArgs(it =>
            {
                context.completionCache.Imports = null;
                return context.ResolveImports(it.ArgAt<FileModel>(0));
            });
            var imports = ASContext.Context.ResolveImports(context.CurrentModel);
            return imports.Items;
        }
        
        static IEnumerable<TestCaseData> ParseFileIssue1150_2_TestCases
        {
            get
            {
                yield return new TestCaseData("Issue1150_4", "Math")
                    .SetName("Import static member. Issue 1150. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1150");
            }
        }

        [Test, TestCaseSource(nameof(ParseFileIssue1150_2_TestCases))]
        public void ParseFileIssue1150_2(string fileName, string fromClass)
        {
            SetSrc(sci, ReadAllText(fileName));
            var context = (Context)ASContext.GetLanguageContext("haxe");
            context.CurrentModel = ASContext.Context.CurrentModel;
            ASContext.Context.ResolveImports(null).ReturnsForAnyArgs(it =>
            {
                context.completionCache.Imports = null;
                return context.ResolveImports(it.ArgAt<FileModel>(0));
            });
            var type = ASContext.Context.ResolveType(fromClass, ASContext.Context.CurrentModel);
            var expectedImports = type.Members.Items.Where(it => (it.Flags & FlagType.Static) != 0 && (it.Access & Visibility.Public) != 0).ToList();
            var actualImports = ASContext.Context.ResolveImports(context.CurrentModel);
            expectedImports.Sort();
            actualImports.Sort();
            Assert.AreEqual(expectedImports, actualImports.Items);
        }
    }
}
