using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Settings;
using HaXeContext.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using ScintillaNet;

namespace HaXeContext.Generators
{
    [TestFixture]
    public class CodeGeneratorTests : ASGeneratorTests.GenerateJob
    {
        static string GetFullPath(string fileName) => $"{nameof(HaXeContext)}.Test_Files.generators.code.{fileName}.hx";

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
        public void Setup()
        {
            ASContext.CommonSettings.DeclarationModifierOrder = new[] {"public", "protected", "internal", "private", "static", "inline", "override"};
            ASContext.CommonSettings.GeneratedMemberDefaultBodyStyle = GeneratedMemberBodyStyle.ReturnDefaultValue;
            ASContext.Context.Settings.GenerateImports.Returns(true);
            SetHaxeFeatures(sci);
        }

        static IEnumerable<TestCaseData> ContextualGeneratorTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1833_1", -1, false)
                    .Returns(null)
                    .SetName("Issue1833. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1833");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1833_2", -1, false)
                    .Returns(null)
                    .SetName("Issue1833. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1833");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1743_1", -1, false)
                    .Returns(null)
                    .SetName("Issue1743. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1743");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1743_2", -1, false)
                    .Returns(null)
                    .SetName("Issue1743. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1743");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1743_3", -1, false)
                    .Returns(null)
                    .SetName("Issue1743. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1743");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1927_1", -1, false)
                    .Returns(null)
                    .SetName("Issue1927. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1927");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1964_1", -1, false)
                    .Returns(null)
                    .SetName("Issue1964. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1964");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2009_1", GeneratorJobType.ConvertToConst, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2009_1"))
                    .SetName("Convert to const. Issue2009. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2009");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2009_2", GeneratorJobType.ConvertToConst, false)
                    .Returns(null)
                    .SetName("Convert to const. Issue2009. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2009");
                yield return new TestCaseData("BeforeContextualGeneratorTests_GenerateFunction_1", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_GenerateFunction_1"))
                    .SetName("fo|o(1, 1.0, (v is String)). Generate function. Case 1");
                yield return new TestCaseData("BeforeContextualGeneratorTests_GenerateFunction_2", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_GenerateFunction_2"))
                    .SetName("Generate function with many arguments. Case 2");
                yield return new TestCaseData("BeforeContextualGeneratorTests_GenerateFunction_3", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_GenerateFunction_3"))
                    .SetName("fo|o(1 < 2 && 1 > 0 ? 1.0 : 0.0). Generate function. Case 3");
                yield return new TestCaseData("BeforeContextualGeneratorTests_GenerateFunction_3_1", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_GenerateFunction_3_1"))
                    .SetName("fo|o(1 < 2 && 1 > 0 ? 1 : 0). Generate function. Case 3.1");
                yield return new TestCaseData("BeforeContextualGeneratorTests_GenerateFunction_4", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_GenerateFunction_4"))
                    .SetName("fo|o(this.bar().x). Generate function . Case 4");
                yield return new TestCaseData("BeforeContextualGeneratorTests_GenerateFunction_5", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_GenerateFunction_5"))
                    .SetName("fo|o((this.bar().x:Int)). Generate function . Case 5")
                    .Ignore("Setup sdk");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1747_1", -1, false)
                    .Returns(null)
                    .SetName("Issue1747. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1747");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1747_2", -1, false)
                    .Returns(null)
                    .SetName("Issue1747. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1747");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1747_3", 0, false)
                    .Returns(null)
                    .SetName("Issue1747. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1747");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1747_4", GeneratorJobType.GetterSetter, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue1747_4"))
                    .SetName("Issue1747. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1747");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1767_1", -1, false)
                    .Returns(null)
                    .SetName("Issue1767. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1767");
                yield return new TestCaseData("BeforeGenerateConstructor_issue1738_1", GeneratorJobType.Constructor, true)
                    .Returns(ReadAllText("AfterGenerateConstructor_issue1738_1"))
                    .SetName("Generate constructor")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1738");
                yield return new TestCaseData("BeforeGenerateConstructor_issue2305_1", GeneratorJobType.Constructor, true)
                    .Returns(ReadAllText("AfterGenerateConstructor_issue2305_1"))
                    .SetName("Generate constructor. Issue 2305. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2305");
                yield return new TestCaseData("BeforeGenerateConstructor_issue2305_2", GeneratorJobType.Constructor, true)
                    .Returns(ReadAllText("AfterGenerateConstructor_issue2305_1"))
                    .SetName("Generate constructor. Issue 2305. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2305");
                yield return new TestCaseData("BeforeGenerateConstructor_issue2305_3", GeneratorJobType.Constructor, true)
                    .Returns(ReadAllText("AfterGenerateConstructor_issue2305_1"))
                    .SetName("Generate constructor. Issue 2305. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2305");
                yield return new TestCaseData("BeforeGenerateConstructor_issue1738_2", GeneratorJobType.Constructor, true)
                    .Returns(ReadAllText("AfterGenerateConstructor_issue1738_2"))
                    .SetName("Generate constructor with parameters")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1738");
                yield return new TestCaseData("BeforeGenerateConstructor_issue1738_3", GeneratorJobType.ChangeConstructorDecl, true)
                    .Returns(ReadAllText("AfterGenerateConstructor_issue1738_3"))
                    .SetName("Change constructor declaration. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1738");
                yield return new TestCaseData("BeforeGenerateConstructor_issue1738_3_1", GeneratorJobType.ChangeConstructorDecl, true)
                    .Returns(ReadAllText("AfterGenerateConstructor_issue1738_3_1"))
                    .SetName("Change constructor declaration. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1738");
                yield return new TestCaseData("BeforeGenerateConstructor_issue1738_4", GeneratorJobType.Constructor, false)
                    .Returns(null)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1738");
                yield return new TestCaseData("BeforeGenerateConstructor_issue1738_5", GeneratorJobType.ChangeConstructorDecl, false)
                    .Returns(null)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1738");
                yield return new TestCaseData("BeforeGenerateConstructor_issue1738_6", GeneratorJobType.ChangeConstructorDecl, false)
                    .Returns(null)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1738");
                yield return new TestCaseData("BeforeGenerateConstructor_issue1738_7", GeneratorJobType.ChangeConstructorDecl, true)
                    .Returns(ReadAllText("AfterGenerateConstructor_issue1738_7"))
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1738");
                yield return new TestCaseData("BeforeGenerateConstructor_issue1738_7_1", GeneratorJobType.ChangeConstructorDecl, true)
                    .Returns(ReadAllText("AfterGenerateConstructor_issue1738_7_1"))
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1738");
                yield return new TestCaseData("BeforeGenerateConstructor_issue1738_8", GeneratorJobType.ChangeConstructorDecl, false)
                    .Returns(null)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1738");
                yield return new TestCaseData("BeforeGenerateConstructor_issue1738_9", GeneratorJobType.ChangeConstructorDecl, true)
                    .Returns(ReadAllText("AfterGenerateConstructor_issue1738_9"))
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1738");
                yield return new TestCaseData("BeforeImplementInterface_issue1982_1", GeneratorJobType.ImplementInterface, false)
                    .Returns(null)
                    .SetName("Issue1982. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1982");
                yield return new TestCaseData("BeforeImplementInterface_issue1982_2", GeneratorJobType.ImplementInterface, false)
                    .Returns(null)
                    .SetName("Issue1982. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1982");
                yield return new TestCaseData("BeforeImplementInterface_issue1982_3", GeneratorJobType.ImplementInterface, false)
                    .Returns(null)
                    .SetName("Issue1982. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1982");
                yield return new TestCaseData("BeforeImplementInterface_issue1982_4", GeneratorJobType.ImplementInterface, false)
                    .Returns(null)
                    .SetName("Issue1982. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1982");
                yield return new TestCaseData("BeforeImplementInterface_issue1982_5", GeneratorJobType.ImplementInterface, false)
                    .Returns(null)
                    .SetName("Issue1982. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1982");
                yield return new TestCaseData("BeforeImplementInterface_issue1982_6", GeneratorJobType.ImplementInterface, false)
                    .Returns(null)
                    .SetName("Issue1982. Case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1982");
                yield return new TestCaseData("BeforeGenerateFunction_issue2478_1", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue2478_1"))
                    .SetName("test(a[0]()).<generate> Generate function. Issue2478. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2478");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2493_1", GeneratorJobType.GetterSetter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2493_1"))
                    .SetName("var s<generate> = new Sprite(). Generate getter and setter. Issue2493. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2493");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2493_1", GeneratorJobType.Getter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2493_2"))
                    .SetName("var s<generate> = new Sprite(). Generate getter. Issue2493. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2493");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2493_1", GeneratorJobType.Setter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2493_3"))
                    .SetName("var s<generate> = new Sprite(). Generate setter. Issue2493. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2493");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2499_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2499_1"))
                    .SetName("(foo<T>(String:Class<T>):T).<generate> Assign statement to variable. Issue2499. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2499");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2499_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2499_2"))
                    .SetName("(foo<T:{}>(String:Class<T>):T).<generate> Assign statement to variable. Issue2499. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2499");
            }
        }

        static IEnumerable<TestCaseData> ContextualGeneratorForOptionParametersTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2022_1", GeneratorJobType.Function, false)
                    .Returns(null)
                    .SetName("Generate private function shouldn't work for optional parameter. private function.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2022");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2022_1", GeneratorJobType.FunctionPublic, false)
                    .Returns(null)
                    .SetName("Generate public function shouldn't work for optional parameter. private function.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2022");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2022_2", GeneratorJobType.Function, false)
                    .Returns(null)
                    .SetName("Generate private function shouldn't work for optional parameter. local function.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2022");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2022_2", GeneratorJobType.FunctionPublic, false)
                    .Returns(null)
                    .SetName("Generate public function shouldn't work for optional parameter. local function.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2022");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2022_3", GeneratorJobType.FieldFromParameter, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2022_3"))
                    .SetName("foo(?ar|gs). Field from parameter. Issue 2022. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2022");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2022_4", GeneratorJobType.FieldFromParameter, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2022_4"))
                    .SetName("foo(?ar|gs). Field From parameter. Issue 2022. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2022");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2312_1", GeneratorJobType.FieldFromParameter, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2312_1"))
                    .SetName("foo(?ar|gs = ''). Field From parameter. Issue 2312. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2312");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2312_2", GeneratorJobType.FieldFromParameter, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2312_2"))
                    .SetName("foo(?ar|gs = true). Field From parameter. Issue 2312. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2312");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2312_3", GeneratorJobType.FieldFromParameter, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2312_3"))
                    .SetName("foo(ar|gs = AbstractEnum.Value). Field From parameter. Issue 2312. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2312");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2312_4", GeneratorJobType.FieldFromParameter, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2312_4"))
                    .SetName("foo(ar|gs = /*AbstractEnum.Value*/). Field From parameter. Issue 2312. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2312");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2312_5", GeneratorJobType.FieldFromParameter, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2312_5"))
                    .SetName("foo(ar|gs = ). Field From parameter. Issue 2312. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2312");
            }
        }

        static IEnumerable<TestCaseData> Issue1880TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1880_1", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue1880_1"))
                    .SetName("fo|o(~/regex/). Generate private function")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1880");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1880_2", GeneratorJobType.FunctionPublic, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue1880_2"))
                    .SetName("fo|o(~/regex/). Generate public function")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1880");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1880_3", GeneratorJobType.ChangeMethodDecl, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue1880_1"))
                    .SetName("fo|o(~/regex/). Change method declaration.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1880");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1880_4", GeneratorJobType.ChangeConstructorDecl, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue1880_4"))
                    .SetName("new Fo|o(~/regex/). Change constructor declaration. ")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1880");
            }
        }
        
        static IEnumerable<TestCaseData> Issue2017TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2017_1", -1, false)
                    .Returns(null)
                    .SetName("Contextual generator shouldn't work. `var foo(|null, null)`")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2017");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2017_2", -1, false)
                    .Returns(null)
                    .SetName("Contextual generator shouldn't work. `var foo(null, |null)`")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2017");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2017_3", -1, false)
                    .Returns(null)
                    .SetName("Contextual generator shouldn't work. `function foo(v = |null)`")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2017");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2017_4", -1, false)
                    .Returns(null)
                    .SetName("Contextual generator shouldn't work. `foo(|null)`")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2017");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2017_5", -1, false)
                    .Returns(null)
                    .SetName("Contextual generator shouldn't work. `[|null]`")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2017");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2017_6", -1, false)
                    .Returns(null)
                    .SetName("Contextual generator shouldn't work. `{v:|null}`")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2017");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2017_7", -1, false)
                    .Returns(null)
                    .SetName("Contextual generator shouldn't work. `return |null;`")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2017");
            }
        }

        static IEnumerable<TestCaseData> Issue2060TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2060_1", GeneratorJobType.FunctionPublic, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2060_1"))
                    .SetName("Foo.f|oo([new Array<Int>()]). Generate public function")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2060");
            }
        }

        static IEnumerable<TestCaseData> Issue2069TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2069_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2069_1"))
                    .SetName("Issue2069. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2069");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2069_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2069_2"))
                    .SetName("Issue2069. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2069");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2069_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2069_3"))
                    .SetName("Issue2069. Case 3.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2069");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2069_4", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2069_4"))
                    .SetName("Issue2069. Case 4.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2069");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2069_5", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2069_5"))
                    .SetName("Issue2069. Case 5.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2069");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2069_6", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2069_6"))
                    .SetName("Issue2069. Case 6.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2069");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2069_7", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2069_7"))
                    .SetName("Issue2069. Case 7.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2069");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2069_7_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2069_7_1"))
                    .SetName("Issue2069. Case 7.1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2069");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2069_8", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2069_8"))
                    .SetName("Issue2069. Case 8.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2069");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2069_9", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2069_9"))
                    .SetName("Issue2069. Case 9.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2069");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2069_10", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2069_10"))
                    .SetName("Issue2069. Case 10.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2069");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2069_11", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2069_11"))
                    .SetName("Issue2069. Case 11.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2069");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2069_12", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2069_12"))
                    .SetName("Issue2069. Case 12.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2069");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2069_13", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2069_13"))
                    .SetName("Issue2069. Case 13.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2069");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2069_14", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2069_14"))
                    .SetName("Issue2069. Case 14.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2069");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2069_15", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2069_15"))
                    .SetName("Issue2069. Case 15.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2069");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2069_16", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2069_16"))
                    .SetName("Issue2069. Case 16.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2069");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2069_17", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2069_17"))
                    .SetName("Issue2069. Case 17.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2069");
            }
        }

        static IEnumerable<TestCaseData> Issue2210TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2210_1", GeneratorJobType.FieldFromParameter, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2210_1"))
                    .SetName("foo(arg|s) -> foo(args) this.args = args;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2326");
            }
        }

        static IEnumerable<TestCaseData> Issue2220TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2220_1", GeneratorJobType.Getter, false)
                    .Returns(null)
                    .SetName("ge|t(). Issue2220. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2220");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2220_2", GeneratorJobType.Getter, false)
                    .Returns(null)
                    .SetName("se|t(). Issue2220. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2220");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2220_3", GeneratorJobType.FunctionPublic, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2220_3"))
                    .SetName("ge|t() -> public function get(){}. Issue2220. Case 3.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2220");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2220_4", GeneratorJobType.FunctionPublic, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2220_4"))
                    .SetName("se|t() -> public function set(){}. Issue2220. Case 4.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2220");
            }
        }

        static IEnumerable<TestCaseData> Issue2295TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2295_1", GeneratorJobType.FunctionPublic, false)
                    .Returns(null)
                    .SetName("Typedef.fo|o(). Issue 2295. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2295");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2295_1", GeneratorJobType.VariablePublic, false)
                    .Returns(null)
                    .SetName("Typedef.fo|o(). Issue 2295. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2295");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2295_2", GeneratorJobType.FunctionPublic, false)
                    .Returns(null)
                    .SetName("Typedef.fo|o. Issue 2295. Case 3.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2295");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2295_2", GeneratorJobType.VariablePublic, false)
                    .Returns(null)
                    .SetName("Typedef.fo|o. Issue 2295. Case 4.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2295");
            }
        }

        static IEnumerable<TestCaseData> Issue2297TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2297_1", GeneratorJobType.FunctionPublic, false)
                    .Returns(null)
                    .SetName("Interface.fo|o(). Issue 2297. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2297");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2297_1", GeneratorJobType.VariablePublic, false)
                    .Returns(null)
                    .SetName("Interface.fo|o(). Issue 2297. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2297");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2297_2", GeneratorJobType.FunctionPublic, false)
                    .Returns(null)
                    .SetName("Interface.fo|o. Issue 2297. Case 3.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2297");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2297_2", GeneratorJobType.VariablePublic, false)
                    .Returns(null)
                    .SetName("Interface.fo|o. Issue 2297. Case 4.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2297");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2297_5", GeneratorJobType.FunctionPublic, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2297_5"))
                    .SetName("interface.fo|o(). Issue 2297. Case 5.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2297");
            }
        }

        static IEnumerable<TestCaseData> Issue2299TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2299_1", GeneratorJobType.FunctionPublic, false)
                    .Returns(null)
                    .SetName("Enum.fo|o(). Issue 2299. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2299");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2299_1", GeneratorJobType.VariablePublic, false)
                    .Returns(null)
                    .SetName("Enum.fo|o(). Issue 2299. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2299");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2299_2", GeneratorJobType.FunctionPublic, false)
                    .Returns(null)
                    .SetName("Enum.fo|o. Issue 2299. Case 3.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2299");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2299_2", GeneratorJobType.VariablePublic, false)
                    .Returns(null)
                    .SetName("Enum.fo|o. Issue 2299. Case 4.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2299");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2299_5", GeneratorJobType.VariablePublic, false)
                    .Returns(null)
                    .SetName("enumValue.fo|o. Issue 2299. Case 5.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2299");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2299_6", GeneratorJobType.VariablePublic, false)
                    .Returns(null)
                    .SetName("enumValue.fo|o(). Issue 2299. Case 6.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2299");
            }
        }

        static IEnumerable<TestCaseData> Issue2303TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2303_1", -1, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2303_1"))
                    .SetName("Auto import. Issue 2303. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2303");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2303_2", -1, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2303_2"))
                    .SetName("Auto import. Issue 2303. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2303");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2303_3", -1, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2303_3"))
                    .SetName("Auto import. Issue 2303. Case 3.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2303");
            }
        }

        static IEnumerable<TestCaseData> Issue2407TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2407_1", GeneratorJobType.Variable , false)
                    .Returns(null)
                    .SetName("@:me$(EntryPoint)ta Issue 2407. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2407");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2407_1", GeneratorJobType.VariablePublic, false)
                    .Returns(null)
                    .SetName("@:me$(EntryPoint)ta Issue 2407. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2407");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2407_1", GeneratorJobType.Function, false)
                    .Returns(null)
                    .SetName("@:me$(EntryPoint)ta Issue 2407. Case 3.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2407");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2407_1", GeneratorJobType.FunctionPublic, false)
                    .Returns(null)
                    .SetName("@:me$(EntryPoint)ta Issue 2407. Case 4.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2407");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2407_1", GeneratorJobType.Class, false)
                    .Returns(null)
                    .SetName("@:me$(EntryPoint)ta Issue 2407. Case 5.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2407");
            }
        }

        static IEnumerable<TestCaseData> Issue2411TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2411_1", GeneratorJobType.Variable , false)
                    .Returns(null)
                    .SetName("abstract AFoo(Int) fr$(EntryPoint)om. Issue 2411. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2407");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2411_1", GeneratorJobType.VariablePublic, false)
                    .Returns(null)
                    .SetName("abstract AFoo(Int) fr$(EntryPoint)om. Issue 2411. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2407");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2411_1", GeneratorJobType.Function, false)
                    .Returns(null)
                    .SetName("abstract AFoo(Int) fr$(EntryPoint)om. Issue 2411. Case 3.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2407");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2411_1", GeneratorJobType.FunctionPublic, false)
                    .Returns(null)
                    .SetName("abstract AFoo(Int) fr$(EntryPoint)om. Issue 2411. Case 4.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2407");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2411_1", GeneratorJobType.Class, false)
                    .Returns(null)
                    .SetName("abstract AFoo(Int) fr$(EntryPoint)om. Issue 2411. Case 5.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2407");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2411_2", GeneratorJobType.Variable , false)
                    .Returns(null)
                    .SetName("abstract AFoo(Int) t$(EntryPoint)o. Issue 2411. Case 6.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2407");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2411_2", GeneratorJobType.VariablePublic, false)
                    .Returns(null)
                    .SetName("abstract AFoo(Int) t$(EntryPoint)o. Issue 2411. Case 7.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2407");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2411_2", GeneratorJobType.Function, false)
                    .Returns(null)
                    .SetName("abstract AFoo(Int) t$(EntryPoint)o. Issue 2411. Case 8.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2407");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2411_2", GeneratorJobType.FunctionPublic, false)
                    .Returns(null)
                    .SetName("abstract AFoo(Int) t$(EntryPoint)o. Issue 2411. Case 9.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2407");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2411_2", GeneratorJobType.Class, false)
                    .Returns(null)
                    .SetName("abstract AFoo(Int) t$(EntryPoint)o. Issue 2411. Case 10.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2407");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue1999TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1999_1", GeneratorJobType.AssignStatementToVar, false)
                    .Returns(null)
                    .SetName("Contextual generator shouldn't work. if(expr) {|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1999");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1999_2", GeneratorJobType.AssignStatementToVar, false)
                    .Returns(null)
                    .SetName("Contextual generator shouldn't work. if(expr) {}|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1999");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1999_3", GeneratorJobType.AssignStatementToVar, false)
                    .Returns(null)
                    .SetName("Contextual generator shouldn't work. if(expr) {}\nelse|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1999");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1999_4", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1999_4"))
                    .SetName("if(true){}\n[]|. Assign statement to var")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1999");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1999_5", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1999_5"))
                    .SetName("if(true){}\n(v:String)|. Assign statement to var")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1999")
                    .Ignore("Setup sdk");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1999_6", GeneratorJobType.AssignStatementToVar, false)
                    .Returns(null)
                    .SetName("Contextual generator shouldn't work. /*some comment*/|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1999");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1999_7", GeneratorJobType.AssignStatementToVar, false)
                    .Returns(null)
                    .SetName("Contextual generator shouldn't work. case v:|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1999");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2086TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2086_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2086_1"))
                    .SetName("Issue 2086. Case 1. Infer local variable type")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2086");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2086_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2086_2"))
                    .SetName("Issue 2086. Case 2. Infer local variable type")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2086");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2086_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2086_3"))
                    .SetName("Issue 2086. Case 3. Infer local variable type. typedef Ints = Array<Int>")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2086");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2086_4", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2086_4"))
                    .SetName("Issue 2086. Case 4. Infer local variable type. typedef Ints = Array<Int>")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2086");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2086_5", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2086_5"))
                    .SetName("Issue 2086. Case 5. Infer local variable type. abstract Ints(Array<int>)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2086");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2086_6", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2086_6"))
                    .SetName("Issue 2086. Case 6. Infer local variable type. var v = new flash.display.BitmapData(2, 2)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2086");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2086_7", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2086_7"))
                    .SetName("Issue 2086. Case 7. Infer local variable type. var v = new flash.display.BitmapData(Std.int(2), Std.int(2))")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2086");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2086_8", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2086_8"))
                    .SetName("Issue 2086. Case 8. Infer local variable type. var v = new haxe.ds.Vector<flash.display.BitmapData>(function() {return 10;}())")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2086");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue1764TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1764_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1764_1"))
                    .SetName("1 < 2|. Assign statement to local variable")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1764_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1764_2"))
                    .SetName("1 > 2|. Assign statement to local variable")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1764_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1764_3"))
                    .SetName("1 && 2|. Assign statement to local variable")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1764_4", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1764_4"))
                    .SetName("1 || 2|. Assign statement to local variable")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1764_5", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1764_5"))
                    .SetName("1 != 2|. Assign statement to local variable")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1764_6", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1764_6"))
                    .SetName("1 == 2|. Assign statement to local variable")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarInferParameterVarTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_inferParameterVar_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferParameterVar_1"))
                    .SetName("Infer parameter var type. Case 1.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferParameterVar_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferParameterVar_2"))
                    .SetName("Infer parameter var type. Case 2.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferParameterVar_2_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferParameterVar_2_1"))
                    .SetName("Infer parameter var type. Case 2.1.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferParameterVar_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferParameterVar_3"))
                    .SetName("Infer parameter var type. Case 3.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferParameterVar_4", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferParameterVar_4"))
                    .SetName("Infer parameter var type. Case 4.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferParameterVar_5", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferParameterVar_5"))
                    .SetName("Infer parameter var type. Case 5.");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarInferParameterVarIssue2350TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_inferParameterVar_issue2350_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferParameterVar_issue2350_1"))
                    .SetName("foo(?v = ). Infer parameter var type. Issue 2350. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2350");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarInferParameterVarIssue2371TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_inferParameterVar_issue2371_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferParameterVar_issue2371_1"))
                    .SetName("foo(?v = [). Infer parameter var type. Issue 2371. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2371");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferParameterVar_issue2371_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferParameterVar_issue2371_2"))
                    .SetName("foo(?v = {). Infer parameter var type. Issue 2371. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2371");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferParameterVar_issue2371_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferParameterVar_issue2371_3"))
                    .SetName("foo(?v = (). Infer parameter var type. Issue 2371. Case 3.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2371");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferParameterVar_issue2371_4", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferParameterVar_issue2371_4"))
                    .SetName("foo(?v = /*123*/). Infer parameter var type. Issue 2371. Case 4.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2371");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferParameterVar_issue2371_5", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferParameterVar_issue2371_5"))
                    .SetName("foo(?v = 1.0f). Infer parameter var type. Issue 2371. Case 5.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2371");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferParameterVar_issue2371_6", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferParameterVar_issue2371_6"))
                    .SetName("foo(?v = v). Infer parameter var type. Issue 2371. Case 6.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2371");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferParameterVar_issue2371_7", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferParameterVar_issue2371_7"))
                    .SetName("foo(?v = someText). Infer parameter var type. Issue 2371. Case 7.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2371");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarInferVarTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_1"))
                    .SetName("Infer var type. Case 1.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_2"))
                    .SetName("Infer var type. Case 2.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_3"))
                    .SetName("Infer var type. Case 3.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_4", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_4"))
                    .SetName("Infer var type. Case 4.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_5", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_5"))
                    .SetName("Infer var type. Case 5.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_6", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_6"))
                    .SetName("Infer var type. Case 6.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_7", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_7"))
                    .SetName("Infer var type. Case 7.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_8", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_8"))
                    .SetName("Infer var type. Case 8.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_9", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_9"))
                    .SetName("Infer var type. Case 9.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_9_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_9_1"))
                    .SetName("Infer var type. Case 9.1.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_10", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_10"))
                    .SetName("Infer var type. Case 10.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_11", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_11"))
                    .SetName("Infer var type. Case 11.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_12", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_12"))
                    .SetName("Infer var type. Case 12.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_13", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_13"))
                    .SetName("Infer var type. Case 13.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_14", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_14"))
                    .SetName("Infer var type. Case 14.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_15", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_15"))
                    .SetName("Infer var type. Case 15.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_16", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_16"))
                    .SetName("Infer var type. Case 16.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_17", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_17"))
                    .SetName("Infer var type. Case 17.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_18", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_18"))
                    .SetName("Infer var type. Case 18.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_19", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_19"))
                    .SetName("Infer var type. Case 19.");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarInferVarIssue2385TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_issue2385_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_issue2385_1"))
                    .SetName("Infer var type. Issue 2385. Case 1.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_issue2385_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_issue2385_2"))
                    .SetName("Infer var type. Issue 2385. Case 2.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_issue2385_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_issue2385_3"))
                    .SetName("Infer var type. Issue 2385. Case 3.");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarInferVarIssue2444TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_issue2444_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_issue2444_1"))
                    .SetName("Infer var type. Issue 2444. Case 1.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_issue2444_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_issue2444_2"))
                    .SetName("Infer var type. Issue 2444. Case 2.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_issue2444_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_issue2444_3"))
                    .SetName("Infer var type. Issue 2444. Case 3.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_issue2444_4", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_issue2444_4"))
                    .SetName("Infer var type. Issue 2444. Case 4.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_issue2444_5", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_issue2444_5"))
                    .SetName("Infer var type. Issue 2444. Case 5.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_issue2444_6", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_issue2444_6"))
                    .SetName("Infer var type. Issue 2444. Case 6.");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarInferVarIssue2447TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_issue2447_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_issue2447_1"))
                    .SetName("Infer var type. Issue 2447. Case 1.");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarInferVarIssue2450TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_issue2450_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_issue2450_1"))
                    .SetName("Infer var type. Issue 2450. Case 1.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_issue2450_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_issue2450_2"))
                    .SetName("Infer var type. Issue 2450. Case 2.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_issue2450_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_issue2450_3"))
                    .SetName("Infer var type. Issue 2450. Case 3.");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_issue2450_4", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_issue2450_4"))
                    .SetName("Infer var type. Issue 2450. Case 4.");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue220TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue220_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue220_1"))
                    .SetName("EnumInstance(1)|. Assign statement to var");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue220_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue220_2"))
                    .SetName("EnumAbstractValue|. Assign statement to var");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue220_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue220_3"))
                    .SetName("EnumAbstract.Value|. Assign statement to var");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2117TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2117_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2117_1"))
                    .SetName("Issue 2117. Case 1. Infer local variable type.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2117");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2117__1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2117__1"))
                    .SetName("Issue 2117. Case 1.1. Infer local variable type.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2117");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2117_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2117_2"))
                    .SetName("Issue 2117. Case 2. Infer local variable type.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2117");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2117_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2117_3"))
                    .SetName("Issue 2117. Case 3. Infer local variable type.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2117");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2117_4", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2117_4"))
                    .SetName("Issue 2117. Case 4. Infer local variable type.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2117");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2117_5", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2117_5"))
                    .SetName("Issue 2117. Case 5. Infer local variable type.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2117");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2151TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2151_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2151_1"))
                    .SetName("_;| Assign statement to var. Issue 2151.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2151");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2153TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2153_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2153_1"))
                    .SetName("get3D;| Assign statement to var. Issue 2153. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2153");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2153_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2153_2"))
                    .SetName("getThis;| Assign statement to var. Issue 2153. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2153");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2153_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2153_3"))
                    .SetName("getSuper;| Assign statement to var. Issue 2153. Case 3.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2153");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2154TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2154_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2154_1"))
                    .SetName("Assign statement to var. Issue 2154. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2154");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2161TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2161_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2161_1"))
                    .SetName("Assign statement to var. Issue 2161. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2161");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2198TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2198_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2198_1"))
                    .SetName("Assign statement to var. Issue 2198. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2198");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2198_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2198_2"))
                    .SetName("Assign statement to var. Issue 2198. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2198");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2198_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2198_3"))
                    .SetName("Assign statement to var. Issue 2198. Case 3.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2198");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2198_4", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2198_4"))
                    .SetName("Assign statement to var. Issue 2198. Case 4.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2198");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2198_5", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2198_5"))
                    .SetName("Assign statement to var. Issue 2198. Case 5.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2198");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2198_6", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2198_6"))
                    .SetName("Assign statement to var. Issue 2198. Case 6.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2198");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2198_7", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2198_7"))
                    .SetName("Assign statement to var. Issue 2198. Case 7.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2198");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2198_8", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2198_8"))
                    .SetName("Assign statement to var. Issue 2198. Case 8.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2198");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2198_9", GeneratorJobType.AssignStatementToVar, true)
                    .Ignore("")
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2198_9"))
                    .SetName("Assign statement to var. Issue 2198. Case 9.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2198");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2198_10", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2198_10"))
                    .SetName("Assign statement to var. Issue 2198. Case 10.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2198");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2306TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2306_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2306_1"))
                    .SetName("Assign statement to var. Issue 2306. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2306");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2455TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2455_1", GeneratorJobType.AssignStatementToVar, false)
                    .Returns(null)
                    .SetName("return 10;|. Assign statement to var. Issue 2455. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2455");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2455_2", GeneratorJobType.AssignStatementToVar, false)
                    .Returns(null)
                    .SetName("return cast 10;|. Assign statement to var. Issue 2455. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2455");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2455_3", GeneratorJobType.AssignStatementToVar, false)
                    .Returns(null)
                    .SetName("return untyped 10;|. Assign statement to var. Issue 2455. Case 3.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2455");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2457TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2457_1", GeneratorJobType.AssignStatementToVar, false)
                    .Returns(null)
                    .SetName("Assign statement to var. Issue 2457. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2457");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2457_2", GeneratorJobType.AssignStatementToVar, false)
                    .Returns(null)
                    .SetName("Assign statement to var. Issue 2457. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2457");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2471TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2471_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2471_1"))
                    .SetName("a[0].<generate> Assign statement to var. Issue 2471. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2471");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2471_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2471_2"))
                    .SetName("a[0].<generate> Assign statement to var. Issue 2471. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2471");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2475TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2475_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2475_1"))
                    .SetName("a[0]().<generate> Assign statement to var. Issue 2451. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2475");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2475_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2475_2"))
                    .SetName("a[0][0]().<generate> Assign statement to var. Issue 2451. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2475");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2203TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2203_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2203_1"))
                    .SetName("(foo<T>(a):T).<generate> Assign statement to var. Issue 2203. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2203");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2203_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2203_2"))
                    .SetName("(foo<T>(a):T).<generate> Assign statement to var. Issue 2203. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2203");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2203_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2203_3"))
                    .SetName("(foo<T>(a):T).<generate> Assign statement to var. Inference the type of variable. Issue 2203. Case 3.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2203");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_unsafe_cast_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_unsafe_cast_1"))
                    .SetName("cast v<generate>");
                yield return new TestCaseData("BeforeAssignStatementToVar_unsafe_cast_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_unsafe_cast_2"))
                    .SetName("cast v.length<generate>");
                yield return new TestCaseData("BeforeAssignStatementToVar_unsafe_cast_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_unsafe_cast_3"))
                    .SetName("cast []<generate>");
                yield return new TestCaseData("BeforeAssignStatementToVar_ParameterizedFunction_case1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_ParameterizedFunction_case1"))
                    .SetName("(foo<T>(stringValue):Array<T>)<generate>");
                yield return new TestCaseData("BeforeAssignStatementToVar_ParameterizedFunction_issue2510_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_ParameterizedFunction_issue2510_1"))
                    .SetName("SomeType.(foo<T>(value):T).foo<generate>. Assign statement to variable. Issue 2510")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2510");
                yield return new TestCaseData("BeforeAssignStatementToVar_ParameterizedFunction_issue2510_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_ParameterizedFunction_issue2510_2"))
                    .SetName("SomeType.(foo<T>('string'):T).charAt<generate>. Assign statement to variable. Issue 2510")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2510");
            }
        }

        static IEnumerable<TestCaseData> AddToInterfaceTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAddInterfaceDefTests_issue1731_1", GeneratorJobType.AddInterfaceDef, true)
                    .Returns(ReadAllText("AfterAddInterfaceDefTests_issue1731_1"))
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1731");
                yield return new TestCaseData("BeforeAddInterfaceDefTests_issue1989_1", GeneratorJobType.AddInterfaceDef, false)
                    .SetName("Issue 1989. Case 1")
                    .Returns(null)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1989");
                yield return new TestCaseData("BeforeAddInterfaceDefTests_issue1989_2", GeneratorJobType.AddInterfaceDef, false)
                    .SetName("Issue 1989. Case 2")
                    .Returns(null)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1989");
                yield return new TestCaseData("BeforeAddInterfaceDefTests_issue1989_3", GeneratorJobType.AddInterfaceDef, false)
                    .SetName("Issue 1989. Case 3")
                    .Returns(null)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1989");
                yield return new TestCaseData("BeforeAddInterfaceDefTests_issue1989_4", GeneratorJobType.AddInterfaceDef, false)
                    .SetName("Issue 1989. Case 4")
                    .Returns(null)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1989");
            }
        }

        static IEnumerable<TestCaseData> AddToInterfaceIssue1733TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAddInterfaceDefTests_issue1733_1", GeneratorJobType.AddInterfaceDef, true)
                    .Returns(ReadAllText("AfterAddInterfaceDefTests_issue1733_1"))
                    .SetName("var v(get, set). Add to interface. Issue 1733. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1733");
                yield return new TestCaseData("BeforeAddInterfaceDefTests_issue1733_2", GeneratorJobType.AddInterfaceDef, true)
                    .Returns(ReadAllText("AfterAddInterfaceDefTests_issue1733_2"))
                    .SetName("var v(null, never). Add to interface. Issue 1733. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1733");
                yield return new TestCaseData("BeforeAddInterfaceDefTests_issue1733_3", GeneratorJobType.AddInterfaceDef, true)
                    .Returns(ReadAllText("AfterAddInterfaceDefTests_issue1733_3"))
                    .SetName("var v(never, null). Add to interface. Issue 1733. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1733");
                yield return new TestCaseData("BeforeAddInterfaceDefTests_issue1733_4", GeneratorJobType.AddInterfaceDef, true)
                    .Returns(ReadAllText("AfterAddInterfaceDefTests_issue1733_4"))
                    .SetName("var v(default, default). Add to interface. Issue 1733. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1733");
            }
        }

        static IEnumerable<TestCaseData> GenerateFunctionTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGeneratePublicFunction_issue1735_1", GeneratorJobType.FunctionPublic, true)
                    .Returns(ReadAllText("AfterGeneratePublicFunction_issue1735_1"))
                    .SetName("Issue1725. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1735");
            }
        }

        static IEnumerable<TestCaseData> GenerateFunctionIssue2200TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGeneratePublicFunction_issue2200_1", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGeneratePublicFunction_issue2200_1"))
                    .SetName("Issue2200. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2200");
                yield return new TestCaseData("BeforeGeneratePublicFunction_issue2200_2", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGeneratePublicFunction_issue2200_2"))
                    .SetName("Issue2200. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2200");
                yield return new TestCaseData("BeforeGeneratePublicFunction_issue2200_3", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGeneratePublicFunction_issue2200_3"))
                    .SetName("Issue2200. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2200");
                yield return new TestCaseData("BeforeGeneratePublicFunction_issue2200_4", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGeneratePublicFunction_issue2200_4"))
                    .SetName("Issue2200. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2200");
                yield return new TestCaseData("BeforeGeneratePublicFunction_issue2200_5", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGeneratePublicFunction_issue2200_5"))
                    .SetName("Issue2200. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2200");
            }
        }

        static IEnumerable<TestCaseData> GenerateFunctionIssue394TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGeneratePublicFunction_issue394_1", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGeneratePublicFunction_issue394_1"))
                    .SetName("Issue 394. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/394");
            }
        }

        static IEnumerable<TestCaseData> GenerateFunctionIssue2293TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGeneratePublicFunction_issue2293_1", GeneratorJobType.FunctionPublic, true)
                    .Returns(ReadAllText("AfterGeneratePublicFunction_issue2293_1"))
                    .SetName("Issue 2293. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2293");
            }
        }

        static IEnumerable<TestCaseData> GenerateVariableIssue2201TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGeneratePublicFunction_issue2201_1", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterGeneratePublicFunction_issue2201_1"))
                    .SetName("Issue2201. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2201");
                yield return new TestCaseData("BeforeGeneratePublicFunction_issue2201_2", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterGeneratePublicFunction_issue2201_2"))
                    .SetName("Issue2201. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2201");
                yield return new TestCaseData("BeforeGeneratePublicFunction_issue2201_3", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterGeneratePublicFunction_issue2201_3"))
                    .SetName("Issue2201. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2201");
                yield return new TestCaseData("BeforeGeneratePublicFunction_issue2201_4", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterGeneratePublicFunction_issue2201_4"))
                    .SetName("Issue2201. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2201");
                yield return new TestCaseData("BeforeGeneratePublicFunction_issue2201_5", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterGeneratePublicFunction_issue2201_5"))
                    .SetName("Issue2201. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2201");
            }
        }

        static IEnumerable<TestCaseData> ImplementInterfaceTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeImplementInterface_issue1696_1", GeneratorJobType.ImplementInterface, true)
                    .Returns(ReadAllText("AfterImplementInterface_issue1696_1"))
                    .SetName("Implement interface methods. Issue 1696. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1696");
                yield return new TestCaseData("BeforeImplementInterface_issue1696_2", GeneratorJobType.ImplementInterface, true)
                    .Returns(ReadAllText("AfterImplementInterface_issue1696_2"))
                    .SetName("Implement interface properties. Issue 1696. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1696");
                yield return new TestCaseData("BeforeImplementInterface_issue1696_3", GeneratorJobType.ImplementInterface, true)
                    .Returns(ReadAllText("AfterImplementInterface_issue1696_3"))
                    .SetName("Implement interface properties. Issue 1696. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1696");
            }
        }

        static IEnumerable<TestCaseData> ImplementInterfaceIssue2264TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeImplementInterface_issue2264_1", GeneratorJobType.ImplementInterface, false)
                    .Returns(null)
                    .SetName("Implement interface methods. Issue 2264. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2264");
                yield return new TestCaseData("BeforeImplementInterface_issue2264_2", GeneratorJobType.ImplementInterface, false)
                    .Returns(null)
                    .SetName("Implement interface methods. Issue 2264. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2264");
                yield return new TestCaseData("BeforeImplementInterface_issue2264_3", GeneratorJobType.ImplementInterface, true)
                    .Returns(ReadAllText("AfterImplementInterface_issue2264_3"))
                    .SetName("Implement interface methods. Issue 2264. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2264");
            }
        }

        static IEnumerable<TestCaseData> ImplementInterfaceIssue2531TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeImplementInterface_issue2531_1", GeneratorJobType.ImplementInterface, true)
                    .Returns(ReadAllText("AfterImplementInterface_issue2531_1"))
                    .SetName("Implement interface methods. Issue 2531. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2531");
            }
        }

        static IEnumerable<TestCaseData> GenerateEventHandlerIssue751TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateEventHandler_issue751_1", GeneratorJobType.ComplexEvent, true)
                    .Returns(ReadAllText("AfterGenerateEventHandler_issue751_1"))
                    .SetName("Generate event handler. Issue 751. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/751");
                yield return new TestCaseData("BeforeGenerateEventHandler_issue751_2", GeneratorJobType.ComplexEvent, true)
                    .Returns(ReadAllText("AfterGenerateEventHandler_issue751_2"))
                    .SetName("Generate event handler. Issue 751. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/751");
            }
        }

        static IEnumerable<TestCaseData> CreateNewClassIssue2393TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeCreateNewClass_issue2393_1", GeneratorJobType.Class, false)
                    .Returns(null)
                    .SetName("@:meta|Tag. Generate new class. Issue 2393. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2393");
            }
        }

        static IEnumerable<TestCaseData> GenerateGetterSetterInAbstractIssue2403TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateGetterSetter_in_abstract_issue2403_1", GeneratorJobType.GetterSetter, false)
                    .Returns(null)
                    .SetName("Generate getter and setter. Issue 2403. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2403");
                yield return new TestCaseData("BeforeGenerateGetterSetter_in_abstract_issue2403_1", GeneratorJobType.Getter, false)
                    .Returns(null)
                    .SetName("Generate getter. Issue 2403. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2403");
                yield return new TestCaseData("BeforeGenerateGetterSetter_in_abstract_issue2403_1", GeneratorJobType.Setter, false)
                    .Returns(null)
                    .SetName("Generate setter. Issue 2403. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2403");
            }
        }

        static IEnumerable<TestCaseData> GenerateGetterSetterInferVar2456TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2456_1", GeneratorJobType.GetterSetter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2456_1"))
                    .SetName("Generate getter and setter. Issue 2456. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2456");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2456_2", GeneratorJobType.GetterSetter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2456_1"))
                    .SetName("Generate getter and setter. Issue 2456. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2456");
            }
        }

        static IEnumerable<TestCaseData> InterfaceContextualGeneratorTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2473_1", GeneratorJobType.GetterSetter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2473_1"))
                    .SetName("Generate Getter and Setter. Issue 2473. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2473_2", GeneratorJobType.Getter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2473_2"))
                    .SetName("Generate Getter. Issue 2473. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2473_3", GeneratorJobType.Setter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2473_3"))
                    .SetName("Generate Setter. Issue 2473. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2473_4", GeneratorJobType.FunctionPublic, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2473_4"))
                    .SetName("Generate Function. Issue 2473. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2473_5", -1, false)
                    .Returns(null)
                    .SetName("Issue 2473. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2473_7", GeneratorJobType.Interface, false)
                    .Returns(null)
                    .SetName("interface A extends b<generate>. Issue 2473. Case 7")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2473_8", GeneratorJobType.Class, false)
                    .Returns(null)
                    .SetName("interface A extends B<generate>. Issue 2473. Case 8")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2473_9", GeneratorJobType.Interface, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2473_9"))
                    .SetName("interface A extends B<generate>. Issue 2473. Case 8")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
            }
        }

        [
            Test,
            TestCaseSource(nameof(ContextualGeneratorTestCases)),
            TestCaseSource(nameof(ContextualGeneratorForOptionParametersTestCases)),
            TestCaseSource(nameof(Issue1880TestCases)),
            TestCaseSource(nameof(Issue2017TestCases)),
            TestCaseSource(nameof(Issue2060TestCases)),
            TestCaseSource(nameof(Issue2069TestCases)),
            //TestCaseSource(nameof(Issue2210TestCases)),
            TestCaseSource(nameof(Issue2220TestCases)),
            TestCaseSource(nameof(Issue2295TestCases)),
            TestCaseSource(nameof(Issue2297TestCases)),
            TestCaseSource(nameof(Issue2299TestCases)),
            TestCaseSource(nameof(Issue2303TestCases)),
            TestCaseSource(nameof(Issue2407TestCases)),
            TestCaseSource(nameof(Issue2411TestCases)),
            //TestCaseSource(nameof(AssignStatementToVarIssue220TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue1764TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue1999TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2086TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2117TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2151TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2153TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2154TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2161TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2198TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2306TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2455TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2457TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2471TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2475TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2203TestCases)),
            TestCaseSource(nameof(AssignStatementToVarInferParameterVarTestCases)),
            TestCaseSource(nameof(AssignStatementToVarInferParameterVarIssue2350TestCases)),
            TestCaseSource(nameof(AssignStatementToVarInferParameterVarIssue2371TestCases)),
            TestCaseSource(nameof(AssignStatementToVarInferVarTestCases)),
            TestCaseSource(nameof(AssignStatementToVarInferVarIssue2385TestCases)),
            TestCaseSource(nameof(AssignStatementToVarInferVarIssue2444TestCases)),
            TestCaseSource(nameof(AssignStatementToVarInferVarIssue2447TestCases)),
            TestCaseSource(nameof(AssignStatementToVarInferVarIssue2450TestCases)),
            TestCaseSource(nameof(AssignStatementToVarTestCases)),
            TestCaseSource(nameof(AddToInterfaceTestCases)),
            TestCaseSource(nameof(AddToInterfaceIssue1733TestCases)),
            TestCaseSource(nameof(GenerateFunctionTestCases)),
            TestCaseSource(nameof(GenerateFunctionIssue2200TestCases)),
            TestCaseSource(nameof(GenerateFunctionIssue394TestCases)),
            TestCaseSource(nameof(GenerateFunctionIssue2293TestCases)),
            TestCaseSource(nameof(GenerateVariableIssue2201TestCases)),
            TestCaseSource(nameof(ImplementInterfaceTestCases)),
            TestCaseSource(nameof(ImplementInterfaceIssue2264TestCases)),
            TestCaseSource(nameof(ImplementInterfaceIssue2531TestCases)),
            TestCaseSource(nameof(GenerateEventHandlerIssue751TestCases)),
            TestCaseSource(nameof(CreateNewClassIssue2393TestCases)),
            TestCaseSource(nameof(GenerateGetterSetterInAbstractIssue2403TestCases)),
            TestCaseSource(nameof(GenerateGetterSetterInferVar2456TestCases)),
            TestCaseSource(nameof(InterfaceContextualGeneratorTestCases)),
        ]
        public string ContextualGenerator(string fileName, GeneratorJobType job, bool hasGenerator) => ContextualGenerator(sci, fileName, job, hasGenerator);

        static string ContextualGenerator(ScintillaControl sci, string fileName, GeneratorJobType job, bool hasGenerator)
        {
            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFile(fileName);
            var context = (Context)ASContext.GetLanguageContext("haxe");
            context.CurrentModel = ASContext.Context.CurrentModel;
            context.completionCache.IsDirty = true;
            context.GetTopLevelElements();
            var visibleExternalElements = context.GetVisibleExternalElements();
            ASContext.Context.GetVisibleExternalElements().Returns(visibleExternalElements);
            var options = new List<ICompletionListItem>();
            ASGenerator.ContextualGenerator(sci, options);
            if (hasGenerator)
            {
                if (job != (GeneratorJobType) (-1))
                {
                    Assert.IsNotEmpty(options);
                    var item = options.Find(it => it is ASCompletion.Completion.GeneratorItem generatorItem && generatorItem.Job == job);
                    Assert.IsNotNull(item);
                    var value = item.Value;
                }
                return sci.Text;
            }
            if (job == (GeneratorJobType) (-1)) Assert.IsEmpty(options);
            if (options.Count > 0) Assert.IsFalse(options.Any(it => it is ASCompletion.Completion.GeneratorItem item && item.Job == job));
            return null;
        }

        static IEnumerable<TestCaseData> HandleOverrideTestCases
        {
            get
            {
                yield return new TestCaseData("HandleOverrideCompletion_class_1")
                    .Returns(true)
                    .SetName("Handle override completion. Class. Case 1.");
                yield return new TestCaseData("HandleOverrideCompletion_class_2")
                    .Returns(true)
                    .SetName("Handle override completion. Class. Case 2.");
                yield return new TestCaseData("HandleOverrideCompletion_abstract")
                    .Returns(false)
                    .SetName("Handle override completion. Abstract.");
                yield return new TestCaseData("HandleOverrideCompletion_interface")
                    .Returns(false)
                    .SetName("Handle override completion. Interface.");
                yield return new TestCaseData("HandleOverrideCompletion_typedef")
                    .Returns(false)
                    .SetName("Handle override completion. Typedef.");
                yield return new TestCaseData("HandleOverrideCompletion_issue2222_1")
                    .Returns(true)
                    .SetName("Issue2222. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2222");
            }
        }

        [Test, TestCaseSource(nameof(HandleOverrideTestCases))]
        public bool HandleOverride(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFile(fileName);
            return ASGenerator.HandleGeneratorCompletion(sci, false, ASContext.Context.Features.overrideKey);
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2230TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2117_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2230_1"))
                    .SetName("Issue 2230. Case 1. Infer local variable type.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2230");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2117__1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2230__1"))
                    .SetName("Issue 2230. Case 1.1. Infer local variable type.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2230");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2117_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2230_2"))
                    .SetName("Issue 2230. Case 2. Infer local variable type.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2230");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2117_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2230_3"))
                    .SetName("Issue 2230. Case 3. Infer local variable type.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2230");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2117_4", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2230_4"))
                    .SetName("Issue 2230. Case 4. Infer local variable type.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2230");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2117_5", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2230_5"))
                    .SetName("Issue 2230. Case 5. Infer local variable type.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2230");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2352TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2352_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2352_1"))
                    .SetName("Issue 2352. Case 1. Disable type declaration.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2352");
            }
        }

        [
            Test,
            TestCaseSource(nameof(AssignStatementToVarIssue2230TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2352TestCases)),
        ]
        public string AssignStatementToVarIssue2230(string fileName, GeneratorJobType job, bool hasGenerator)
        {
            ((HaXeSettings) ASContext.Context.Settings).DisableTypeDeclaration = true;
            var result = ContextualGenerator(sci, fileName, job, hasGenerator);
            ((HaXeSettings) ASContext.Context.Settings).DisableTypeDeclaration = false;
            return result;
        }

        static IEnumerable<TestCaseData> ParseFunctionParametersTestCases
        {
            get
            {
                yield return new TestCaseData("ParseFunctionParameters_issue2478_1")
                    .Returns(1)
                    .SetName("test(a[0]()). Parse function parameters. Issue2478. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2478");
                yield return new TestCaseData("ParseFunctionParameters_issue2478_2")
                    .Returns(2)
                    .SetName("test(a[0](), a[1]()). Parse function parameters. Issue2478. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2478");
            }
        }
        
        [Test, TestCaseSource(nameof(ParseFunctionParametersTestCases))]
        public int ParseFunctionParameters(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFile(fileName);
            return ASGenerator.ParseFunctionParameters(sci, sci.CurrentPos).Count;
        }
    }
}
