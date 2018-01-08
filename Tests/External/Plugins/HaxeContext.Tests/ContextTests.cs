using System.Collections.Generic;
using System.Linq;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.TestUtils;
using HaXeContext.TestUtils;
using NUnit.Framework;

namespace HaXeContext
{
    [TestFixture]
    class ContextTests : ASCompleteTests
    {
        protected static string ReadAllTextHaxe(string fileName) => TestFile.ReadAllText(GetFullPathHaxe(fileName));

        protected static string GetFullPathHaxe(string fileName) => $"{nameof(HaXeContext)}.Test_Files.parser.{fileName}.hx";

        [TestFixtureSetUp]
        public void ContextTestsSetUp()
        {
            ASContext.Context.SetHaxeFeatures();
            sci.ConfigurationLanguage = "haxe";
        }

        IEnumerable<TestCaseData> DecomposeTypesTestCases
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

        IEnumerable<TestCaseData> ParseFile_Issue1849TestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllTextHaxe("Issue1849_1"))
                    .Returns("Dynamic<T>")
                    .SetName("implements Dynamic<T>")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1849");
                yield return new TestCaseData(ReadAllTextHaxe("Issue1849_2"))
                    .Returns("IStruct<T>")
                    .SetName("implements IStruct<T>")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1849");
                yield return new TestCaseData(ReadAllTextHaxe("Issue1849_3"))
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

        IEnumerable<TestCaseData> ResolveDotContext_issue750TestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllTextHaxe("ResolveDotContext_Issue1916_1"))
                    .Returns(null)
                    .SetName("case 1");
                yield return new TestCaseData(ReadAllTextHaxe("ResolveDotContext_Issue1916_2"))
                    .Returns(new MemberModel("code", "Int", FlagType.Getter, Visibility.Public))
                    .SetName("case 2");
                yield return new TestCaseData(ReadAllTextHaxe("ResolveDotContext_Issue1916_3"))
                    .Returns(new MemberModel("code", "Int", FlagType.Getter, Visibility.Public))
                    .SetName("case 3");
                yield return new TestCaseData(ReadAllTextHaxe("ResolveDotContext_Issue1916_4"))
                    .Returns(null)
                    .SetName("case 4");
                yield return new TestCaseData(ReadAllTextHaxe("ResolveDotContext_Issue1916_5"))
                    .Returns(new MemberModel("code", "Int", FlagType.Getter, Visibility.Public))
                    .SetName("case 5");
                yield return new TestCaseData(ReadAllTextHaxe("ResolveDotContext_Issue1916_6"))
                    .Returns(new MemberModel("code", "Int", FlagType.Getter, Visibility.Public))
                    .SetName("case 6");
            }
        }

        [Test, TestCaseSource(nameof(ResolveDotContext_issue750TestCases))]
        public MemberModel ResolveDotContext_issue750(string sourceText)
        {
            ((HaXeSettings)ASContext.Context.Settings).CompletionMode = HaxeCompletionModeEnum.FlashDevelop;
            SetSrc(sci, sourceText);
            var expr = ASComplete.GetExpression(sci, sci.CurrentPos);
            var list = ASContext.Context.ResolveDotContext(sci, expr, false);
            return list?.Search("code", FlagType.Getter, Visibility.Public);
        }

        IEnumerable<TestCaseData> ResolveTokenTestCases
        {
            get
            {
                yield return new TestCaseData("true")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore})
                    .SetName("true");
                yield return new TestCaseData("false")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore})
                    .SetName("false");
                yield return new TestCaseData("{}")
                    .Returns(new ClassModel {Name = "Dynamic", Type = "Dynamic", InFile = FileModel.Ignore})
                    .SetName("{}");
                yield return new TestCaseData("10")
                    .Returns(new ClassModel {Name = "Float", Type = "Float", InFile = FileModel.Ignore})
                    .SetName("10");
                yield return new TestCaseData("-10")
                    .Returns(new ClassModel {Name = "Float", Type = "Float", InFile = FileModel.Ignore})
                    .SetName("-10");
                yield return new TestCaseData("\"\"")
                    .Returns(new ClassModel {Name = "String", Type = "String", InFile = FileModel.Ignore})
                    .SetName("\"\"");
                yield return new TestCaseData("''")
                    .Returns(new ClassModel {Name = "String", Type = "String", InFile = FileModel.Ignore})
                    .SetName("''");
                yield return new TestCaseData("0xFF0000")
                    .Returns(new ClassModel {Name = "Int", Type = "Int", InFile = FileModel.Ignore})
                    .SetName("0xFF0000");
                yield return new TestCaseData("[]")
                    .Returns(new ClassModel {Name = "Array<T>", Type = "Array<T>", InFile = FileModel.Ignore})
                    .SetName("[]");
                yield return new TestCaseData("[1 => 1]")
                    .Returns(new ClassModel {Name = "Map<K, V>", Type = "Map<K, V>", InFile = FileModel.Ignore})
                    .SetName("[1 => 1]");
                yield return new TestCaseData("(v is String)")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore});
                yield return new TestCaseData("(['is'] is Array)")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore});
                yield return new TestCaseData("(' is string' is String)")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore});
                yield return new TestCaseData("({x:Int, y:Int} is Point)")
                    .Returns(new ClassModel {Name = "Bool", Type = "Bool", InFile = FileModel.Ignore});
            }
        }

        [Test, TestCaseSource(nameof(ResolveTokenTestCases))]
        public ClassModel ResolveToken(string token) => ASContext.Context.ResolveToken(token, null);
    }
}
