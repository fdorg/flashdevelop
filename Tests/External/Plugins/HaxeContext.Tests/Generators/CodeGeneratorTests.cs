using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ASCompletion;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.Settings;
using HaXeContext.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using PluginCore.Collections;
using PluginCore.Controls;
using PluginCore.Managers;
using ScintillaNet;

namespace HaXeContext.Generators
{
    [TestFixture]
    public class CodeGeneratorTests : ASCompletionTests
    {
        static readonly string testFilesAssemblyPath = $"\\FlashDevelop\\Bin\\Debug\\{nameof(HaXeContext)}\\Test_Files\\";
        static readonly string testFilesDirectory = $"\\Tests\\External\\Plugins\\{nameof(HaXeContext)}.Tests\\Test Files\\";

        internal static void SetCurrentFileName(string fileName)
        {
            fileName = GetFullPath(fileName);
            fileName = Path.GetFileNameWithoutExtension(fileName).Replace('.', Path.DirectorySeparatorChar) + Path.GetExtension(fileName);
            fileName = Path.GetFullPath(fileName);
            fileName = fileName.Replace(testFilesAssemblyPath, testFilesDirectory);
            ASContext.Context.CurrentModel.FileName = fileName;
            PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
        }

        static string GetFullPath(string fileName) => $"{nameof(HaXeContext)}.Test_Files.generators.code.{fileName}.hx";

        internal static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));
        
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
                    .SetName("Generate constructor with parameters. Issue 1738. Case 2")
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
                    .SetName("test(a[0]()).<generator> Generate function. Issue2478. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2478");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2493_1", GeneratorJobType.GetterSetter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2493_1"))
                    .SetName("var s<generator> = new Sprite(). Generate getter and setter. Issue2493. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2493");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2493_1", GeneratorJobType.Getter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2493_2"))
                    .SetName("var s<generator> = new Sprite(). Generate getter. Issue2493. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2493");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2493_1", GeneratorJobType.Setter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2493_3"))
                    .SetName("var s<generator> = new Sprite(). Generate setter. Issue2493. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2493");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2499_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2499_1"))
                    .SetName("(foo<T>(String:Class<T>):T).<generator> Assign statement to variable. Issue2499. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2499");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2499_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2499_2"))
                    .SetName("(foo<T:{}>(String:Class<T>):T).<generator> Assign statement to variable. Issue2499. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2499");
            }
        }

        static IEnumerable<TestCaseData> GenerateToStringTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateToString_1", GeneratorJobType.ToString, true)
                    .Returns(ReadAllText("AfterGenerateToString_1"))
                    .SetName("Generate toString(). Case 1");
                yield return new TestCaseData("BeforeGenerateToString_2", GeneratorJobType.ToString, true)
                    .Returns(ReadAllText("AfterGenerateToString_2"))
                    .SetName("Generate toString(). Case 2");
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

        static IEnumerable<TestCaseData> ContextualGeneratorIssue2779TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2779_1", GeneratorJobType.FunctionPublic, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2779_1"))
                    .SetName("Issue2779. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2779");
            }
        }

        static IEnumerable<TestCaseData> Issue1880TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1880_1", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue1880_1"))
                    .SetName("fo|o(~/regex/). Generate private function. Issue 1880. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1880");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1880_2", GeneratorJobType.FunctionPublic, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue1880_2"))
                    .SetName("fo|o(~/regex/). Generate public function. Issue 1880. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1880");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue1880_3", GeneratorJobType.ChangeMethodDecl, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue1880_3"))
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
                    .SetName("foo(?v = 1.0). Infer parameter var type. Issue 2371. Case 5.")
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

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2569TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2569_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2569_1"))
                    .SetName("trace<generator>. Issue 2569. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2569");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2569_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2569_2"))
                    .SetName("trace<generator>. Issue 2569. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2569");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2217TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2217_1", GeneratorJobType.AssignStatementToVar, false)
                    .Returns(null)
                    .SetName("Issue2217.new()<generator>. Issue 2217. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2217");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2217_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2217_2"))
                    .SetName("Issue2217.new<generator>. Issue 2217. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2217");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2574TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2574_1", GeneratorJobType.AssignStatementToVar, false)
                    .Returns(null)
                    .SetName("Issue2574.trace()<generator>. Issue 2574. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2574");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2574_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2574_2"))
                    .SetName("Issue2574.trace()<generator>. Issue 2574. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2574");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2594TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2594_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2594_1"))
                    .SetName("var v = true ? 1 : 0;\r\nv<generator>. Issue 2594. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2594");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2594_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2594_2"))
                    .SetName("var v = 1 + 2 > 0 || true ? 1 : 0;\r\nv<generator>. Issue 2594. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2594");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2725TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2725_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2725_1"))
                    .SetName("var v = value == v;\r\nv<generator>. Issue 2725. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2725");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2725_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2725_2"))
                    .SetName("var v = value + v;\r\nv<generator>. Issue 2725. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2725");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2725_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2725_3"))
                    .SetName("var v = value | v;\r\nv<generator>. Issue 2725. Case 3.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2725");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2725_4", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2725_4"))
                    .SetName("var v = true ? null : v;\r\nv<generator>. Issue 2725. Case 4.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2725");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2725_5", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2725_5"))
                    .SetName("var v = ++v;\r\nv<generator>. Issue 2725. Case 5.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2725");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2725_6", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2725_6"))
                    .SetName("var v = v + v;\r\nv<generator>. Issue 2725. Case 6.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2725");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2734TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2734_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2734_1"))
                    .SetName("var V = new V();\r\nv<generator>. Issue 2734. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2734");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2743TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2743_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2743_1"))
                    .SetName("var v = '\r\n';\r\nv<generator>. Issue 2743. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2743");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2743_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2743_2"))
                    .SetName("var v = \"\r\n\";\r\nv<generator>. Issue 2743. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2743");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2743_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2743_3"))
                    .SetName("var v = '\r\'\n';\r\nv<generator>. Issue 2743. Case 3.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2743");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2743_4", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2743_4"))
                    .SetName("var v = '\r\"\n';\r\nv<generator>. Issue 2743. Case 4.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2743");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2743_5", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2743_5"))
                    .SetName("var v = '\r[\n';\r\nv<generator>. Issue 2743. Case 5.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2743");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2830TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2830_1", GeneratorJobType.AssignStatementToVar, false)
                    .Returns(null)
                    .SetName("var v2 = $(EntryPoint)v1 + 1$(ExitPoint)<generator>. Issue 2830. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2830");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2825TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2825_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2825_1"))
                    .SetName("(function foo() {return foo();})()<generator>. Issue 2825. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2825");
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
                    .SetName("a[0].<generator> Assign statement to var. Issue 2471. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2471");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2471_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2471_2"))
                    .SetName("a[0].<generator> Assign statement to var. Issue 2471. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2471");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2475TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2475_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2475_1"))
                    .SetName("a[0]().<generator> Assign statement to var. Issue 2451. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2475");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2475_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2475_2"))
                    .SetName("a[0][0]().<generator> Assign statement to var. Issue 2451. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2475");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2203TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2203_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2203_1"))
                    .SetName("(foo<T>(a):T).<generator> Assign statement to var. Issue 2203. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2203");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2203_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2203_2"))
                    .SetName("(foo<T>(a):T).<generator> Assign statement to var. Issue 2203. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2203");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2203_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2203_3"))
                    .SetName("(foo<T>(a):T).<generator> Assign statement to var. Inference the type of variable. Issue 2203. Case 3.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2203");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue1756TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1756_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1756_1"))
                    .SetName("true ? 1 : 2;<generator> Assign statement to var. Issue 1756. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1756");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1756_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1756_2"))
                    .SetName("true ? '' : '';<generator> Assign statement to var. Issue 1756. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1756");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_unsafe_cast_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_unsafe_cast_1"))
                    .SetName("cast v<generator>");
                yield return new TestCaseData("BeforeAssignStatementToVar_unsafe_cast_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_unsafe_cast_2"))
                    .SetName("cast v.length<generator>");
                yield return new TestCaseData("BeforeAssignStatementToVar_unsafe_cast_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_unsafe_cast_3"))
                    .SetName("cast []<generator>");
                yield return new TestCaseData("BeforeAssignStatementToVar_ParameterizedFunction_case1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_ParameterizedFunction_case1"))
                    .SetName("(foo<T>(stringValue):Array<T>)<generator>");
                yield return new TestCaseData("BeforeAssignStatementToVar_ParameterizedFunction_issue2510_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_ParameterizedFunction_issue2510_1"))
                    .SetName("SomeType.(foo<T>(value):T).foo<generator>. Assign statement to variable. Issue 2510")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2510");
                yield return new TestCaseData("BeforeAssignStatementToVar_ParameterizedFunction_issue2510_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_ParameterizedFunction_issue2510_2"))
                    .SetName("SomeType.(foo<T>('string'):T).charAt<generator>. Assign statement to variable. Issue 2510")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2510");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue2628_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2628_1"))
                    .SetName("!true<generator>. Assign statement to variable. Issue 2628")
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

        static IEnumerable<TestCaseData> DeclareVariableIssue2558TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeDeclareVariable_issue2558_1", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterDeclareVariable_issue2558_1"))
                    .SetName("char<generator> = ''.charAt(0). Declare private variable. Issue 2558. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2558");
                yield return new TestCaseData("BeforeDeclareVariable_issue2558_2", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterDeclareVariable_issue2558_2"))
                    .SetName("char<generator> = ''.charAt. Declare private variable. Issue 2558. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2558");
                yield return new TestCaseData("BeforeDeclareVariable_issue2558_3", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterDeclareVariable_issue2558_3"))
                    .SetName("length<generator> = ''.length. Declare private variable. Issue 2558. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2558");
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
                yield return new TestCaseData("BeforeGenerateFunction", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGeneratePrivateFunction_generateExplicitScopeIsFalse"))
                    .SetName("Generate private function");
                yield return new TestCaseData("BeforeGenerateFunction", GeneratorJobType.FunctionPublic, true)
                    .Returns(ReadAllText("AfterGeneratePublicFunction_generateExplicitScopeIsFalse"))
                    .SetName("Generate public function");
                yield return new TestCaseData("BeforeGenerateFunction_issue103", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103"))
                    .SetName("Issue103. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_2", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_2"))
                    .SetName("Issue103. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_3", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_3"))
                    .SetName("Issue103. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_4", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_4"))
                    .SetName("Issue103. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_5", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_5"))
                    .SetName("Issue103. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_6", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_6"))
                    .SetName("Issue103. Case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_7", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_7"))
                    .SetName("Issue103. Case 7")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_8", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_8"))
                    .SetName("Issue103. Case 8")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_9", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_9"))
                    .SetName("Issue103. Case 9")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_10", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_10"))
                    .SetName("Issue103. Case 10")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_11", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_11"))
                    .SetName("Issue103. Case 11")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_12", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_12"))
                    .SetName("Issue103. Case 12")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_13", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_13"))
                    .SetName("Issue103. Case 13")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103")
                    .Ignore("ASContext.CommonSettings.GeneratedMemberDefaultBodyStyle = GeneratedMemberBodyStyle.UncompilableCode");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_14", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_14"))
                    .SetName("Issue103. Case 14")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103")
                    .Ignore("ASContext.CommonSettings.GeneratedMemberDefaultBodyStyle = GeneratedMemberBodyStyle.UncompilableCode");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_14_1", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_14_1"))
                    .SetName("Issue103. Case 14.1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103")
                    .Ignore("ASContext.CommonSettings.GeneratedMemberDefaultBodyStyle = GeneratedMemberBodyStyle.UncompilableCode");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_15", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_15"))
                    .SetName("Issue103. Case 15")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_16", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_16"))
                    .SetName("Issue103. Case 16")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_17", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_17"))
                    .SetName("Issue103. Case 17")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_18", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_18"))
                    .SetName("Issue103. Case 18")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_19", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_19"))
                    .SetName("Issue103. Case 19")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_20", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_20"))
                    .SetName("Issue103. Case 20")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_21", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_21"))
                    .SetName("Issue103. Case 21")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_22", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_22"))
                    .SetName("Issue103. Case 22")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue1645", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue1645"))
                    .SetName("Issue1645. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1645");
                yield return new TestCaseData("BeforeGenerateFunction_issue1645_2", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue1645_2"))
                    .SetName("Issue1645. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1645")
                    .Ignore("ASContext.CommonSettings.GeneratedMemberDefaultBodyStyle = GeneratedMemberBodyStyle.UncompilableCode");
                yield return new TestCaseData("BeforeGenerateFunction_issue1780_1", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue1780_1"))
                    .SetName("foo(Math.round(1.5))")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1780");
                yield return new TestCaseData("BeforeGenerateFunction_issue1780_2", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue1780_2"))
                    .SetName("foo(round(1.5))")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1780");
                yield return new TestCaseData("BeforeGenerateFunction_issue1836", GeneratorJobType.FunctionPublic, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue1836"))
                    .SetName("Issue 1836. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1836");
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
        
        static IEnumerable<TestCaseData> GenerateVariableTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateVariable", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterGeneratePrivateVariable_generateExplicitScopeIsFalse"))
                    .SetName("Generate private variable");
                yield return new TestCaseData("BeforeGenerateStaticVariable", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterGeneratePrivateStaticVariable"))
                    .SetName("Generate private static variable");
                yield return new TestCaseData("BeforeGenerateVariable", GeneratorJobType.VariablePublic, true)
                    .Returns(ReadAllText("AfterGeneratePublicVariable_generateExplicitScopeIsFalse"))
                    .SetName("Generate public variable");
                yield return new TestCaseData("BeforeGenerateStaticVariable", GeneratorJobType.VariablePublic, true)
                    .Returns(ReadAllText("AfterGeneratePublicStaticVariable_generateExplicitScopeIsFalse"))
                    .SetName("Generate public static variable");
                yield return new TestCaseData("BeforeGeneratePublicStaticVariable_forSomeType", GeneratorJobType.VariablePublic, true)
                    .Returns(ReadAllText("AfterGeneratePublicStaticVariable_forSomeType"))
                    .SetName("From SomeType.foo|");
                yield return new TestCaseData("BeforeGeneratePublicStaticVariable_forCurrentType", GeneratorJobType.VariablePublic, true)
                    .Returns(ReadAllText("AfterGeneratePublicStaticVariable_forCurrentType"))
                    .SetName("From CurrentType.foo|");
                yield return new TestCaseData("BeforeGenerateVariable_issue1460_1", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterGenerateVariable_issue1460_1"))
                    .SetName("Generate Variable. Issue 1460. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1460");
                yield return new TestCaseData("BeforeGenerateVariable_issue1460_2", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterGenerateVariable_issue1460_2"))
                    .SetName("Generate Variable. Issue 1460. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1460");
                yield return new TestCaseData("BeforeGenerateVariable_issue1460_3", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterGenerateVariable_issue1460_3"))
                    .SetName("Generate Variable. Issue 1460. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1460");
                yield return new TestCaseData("BeforeGenerateConstant", GeneratorJobType.Constant, true)
                    .Returns(ReadAllText("AfterGenerateConstant"))
                    .SetName("Generate constant");
                yield return new TestCaseData("BeforeGenerateConstant_issue1460", GeneratorJobType.Constant, true)
                    .Returns(ReadAllText("AfterGenerateConstant_issue1460"))
                    .SetName("Generate Constant. Issue 1460. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1460");
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

        static IEnumerable<TestCaseData> GenerateVariableIssue1734TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGeneratePublicVariable_issue1734_1", GeneratorJobType.VariablePublic, true)
                    .Returns(ReadAllText("AfterGeneratePublicVariable_issue1734_1"))
                    .SetName("Issue1734. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1734");
            }
        }

        static IEnumerable<TestCaseData> GenerateVariableIssue2477TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateVariable_issue2477_1", GeneratorJobType.VariablePublic, true)
                    .Returns(ReadAllText("AfterGenerateVariable_issue2477_1"))
                    .SetName("Issue2477. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2477");
                yield return new TestCaseData("BeforeGenerateVariable_issue2477_2", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterGenerateVariable_issue2477_2"))
                    .SetName("Issue2477. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2477");
            }
        }

        static IEnumerable<TestCaseData> GenerateVariableIssue2952TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateVariable_issue2952_1", GeneratorJobType.VariablePublic, true)
                    .Returns(ReadAllText("AfterGenerateVariable_issue2952_1"))
                    .SetName("v$(EntryPoint) = new Array<Type>. Declare variable. Issue2952. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2952");
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

        static IEnumerable<TestCaseData> ImplementInterfaceIssue2553TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeImplementInterface_issue2553_1", GeneratorJobType.ImplementInterface, true)
                    .Returns(ReadAllText("AfterImplementInterface_issue2553_1"))
                    .SetName("Implement interface methods. Issue 2553. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2553");
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

        static IEnumerable<TestCaseData> GenerateGetterSetterTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateGetterSetter", GeneratorJobType.GetterSetter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter"))
                    .SetName("Generate getter and setter");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue221", GeneratorJobType.GetterSetter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue221"))
                    .SetName("issue 221");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue_1", GeneratorJobType.Setter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue_1"))
                    .SetName("issue set");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue_2", GeneratorJobType.Getter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue_2"))
                    .SetName("issue get 1");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue_3", GeneratorJobType.Getter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue_3"))
                    .SetName("issue get 2");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue_4", GeneratorJobType.Getter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue_4"))
                    .SetName("issue get 3");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue_5", GeneratorJobType.Getter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue_5"))
                    .SetName("issue get 4");
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

        static IEnumerable<TestCaseData> GenerateGetterSetter2838TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2838_1", GeneratorJobType.GetterSetter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2838_1"))
                    .SetName("Generate getter and setter. Issue 2838. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2838");
            }
        }

        static IEnumerable<TestCaseData> GenerateGetterSetter2477TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2477_1", GeneratorJobType.Getter, false)
                    .Returns(null)
                    .SetName("Generate getter. Issue 2477. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2477");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2477_1", GeneratorJobType.Setter, false)
                    .Returns(null)
                    .SetName("Generate setter. Issue 2477. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2477");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2477_1", GeneratorJobType.GetterSetter, false)
                    .Returns(null)
                    .SetName("Generate getter and setter. Issue 2477. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2477");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2477_4", GeneratorJobType.GetterSetter, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2477_4"))
                    .SetName("Generate getter and setter. Issue 2477. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2477");
            }
        }

        static IEnumerable<TestCaseData> GenerateConstructorIssue2845TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateConstructor_issue2845_1", GeneratorJobType.Constructor, true)
                    .Returns(ReadAllText("AfterGenerateConstructor_issue2845_1"))
                    .SetName("Generate constructor. Issue 2845. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2845");
                yield return new TestCaseData("BeforeGenerateConstructor_issue2845_2", GeneratorJobType.Constructor, true)
                    .Returns(ReadAllText("AfterGenerateConstructor_issue2845_2"))
                    .SetName("Generate constructor. Issue 2845. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2845");
            }
        }

        static IEnumerable<TestCaseData> GenerateConstructorWithInitializerIssue2872TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateConstructor_issue2872_1", GeneratorJob.ConstructorWithInitializer, true)
                    .Returns(ReadAllText("AfterGenerateConstructor_issue2872_1"))
                    .SetName("Generate constructor with initializer. Issue 2872. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2872");
            }
        }

        static IEnumerable<TestCaseData> NewClassIssue2585TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateClass_issue2585_1", GeneratorJobType.Class, false)
                    .Returns(null)
                    .SetName("var v:type<generator>. Generate new Class. Issue 2585. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2585");
                yield return new TestCaseData("BeforeGenerateClass_issue2585_2", GeneratorJobType.Class, false)
                    .Returns(null)
                    .SetName("new type<generator>. Generate new Class. Issue 2585. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2585");
                yield return new TestCaseData("BeforeGenerateClass_issue2585_3", GeneratorJobType.Class, false)
                    .Returns(null)
                    .SetName("localVar = new type<generator>. Generate new Class. Issue 2585. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2585");
                yield return new TestCaseData("BeforeGenerateClass_issue2585_4", GeneratorJobType.Class, false)
                    .Returns(null)
                    .SetName("localVar = new Type().value<generator>. Generate new Class. Issue 2585. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2585");
            }
        }

        static IEnumerable<TestCaseData> NewInterfaceIssue2587TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeNewInterface_issue2587_1", GeneratorJobType.Interface, false)
                    .Returns(null)
                    .SetName("var v:type<generator>. Generate new Class. Issue 2587. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2587");
                yield return new TestCaseData("BeforeNewInterface_issue2587_2", GeneratorJobType.Interface, false)
                    .Returns(null)
                    .SetName("new Type<generator>. Generate new Class. Issue 2587. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2587");
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
                    .SetName("interface A extends b<generator>. Issue 2473. Case 7")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2473_8", GeneratorJobType.Class, false)
                    .Returns(null)
                    .SetName("interface A extends B<generator>. Issue 2473. Case 8")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeGenerateGetterSetter_issue2473_9", GeneratorJobType.Interface, true)
                    .Returns(ReadAllText("AfterGenerateGetterSetter_issue2473_9"))
                    .SetName("interface A extends B<generator>. Issue 2473. Case 8")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
            }
        }
        
        static IEnumerable<TestCaseData> ConvertStaticMethodCallToStaticExtensionCallIssue1565TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeConvertStaticMethodCallIntoStaticExtensionsCall", GeneratorJob.ConvertStaticMethodCallToStaticExtensionCall, true)
                    .Returns(ReadAllText("AfterConvertStaticMethodCallIntoStaticExtensionsCall"))
                    .SetName("var v = StringTools.trim(' string ') -> var v = ' string '.trim()")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1565");
                yield return new TestCaseData("BeforeConvertStaticMethodCallIntoStaticExtensionsCall2", GeneratorJob.ConvertStaticMethodCallToStaticExtensionCall, true)
                    .Returns(ReadAllText("AfterConvertStaticMethodCallIntoStaticExtensionsCall2"))
                    .SetName("var v = StringTools.lpad('10' , 8, 0) -> var v = '10'.lpad(8, 0)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1565");
                yield return new TestCaseData("BeforeConvertStaticMethodCallIntoStaticExtensionsCall3", GeneratorJob.ConvertStaticMethodCallToStaticExtensionCall, true)
                    .Returns(ReadAllText("AfterConvertStaticMethodCallIntoStaticExtensionsCall3"))
                    .SetName("var v = Lambda.count([1, 2, 3]) -> var s = [1, 2, 3].count()")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1565");
                yield return new TestCaseData("BeforeConvertStaticMethodCallIntoStaticExtensionsCall4", GeneratorJob.ConvertStaticMethodCallToStaticExtensionCall, true)
                    .Returns(ReadAllText("AfterConvertStaticMethodCallIntoStaticExtensionsCall4"))
                    .SetName("var v = Reflect.isObject({x:0, y:1}) -> var v = {x:0, y:1}.isObject()")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1565");
                yield return new TestCaseData("BeforeConvertStaticMethodCallIntoStaticExtensionsCall5", GeneratorJob.ConvertStaticMethodCallToStaticExtensionCall, true)
                    .Returns(ReadAllText("AfterConvertStaticMethodCallIntoStaticExtensionsCall5"))
                    .SetName("private var v = Reflect.isObject({x:0, y:1}) -> private var v = {x:0, y:1}.isObject()")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1565");
                yield return new TestCaseData("BeforeConvertStaticMethodCallIntoStaticExtensionsCall6", GeneratorJob.ConvertStaticMethodCallToStaticExtensionCall, true)
                    .Returns(ReadAllText("AfterConvertStaticMethodCallIntoStaticExtensionsCall6"))
                    .SetName("private function foo() return Reflect.isObject({x:0, y:1}) -> private function foo() return {x:0, y:1}.isObject()")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1565");
                yield return new TestCaseData("BeforeConvertStaticMethodCallIntoStaticExtensionsCall7", GeneratorJob.ConvertStaticMethodCallToStaticExtensionCall, true)
                    .Returns(ReadAllText("AfterConvertStaticMethodCallIntoStaticExtensionsCall7"))
                    .SetName("var v = StringTools.lpad(Std.string(1), '0', 2) -> var v = Std.string(1).lpad('0', 2)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1565");
                yield return new TestCaseData("BeforeConvertStaticMethodCallIntoStaticExtensionsCall8", GeneratorJob.ConvertStaticMethodCallToStaticExtensionCall, true)
                    .Returns(ReadAllText("AfterConvertStaticMethodCallIntoStaticExtensionsCall8"))
                    .SetName("var v = StringTools.lpad(someVar, '0', 2) -> var v = someVar.lpad('0', 2)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1565");
                yield return new TestCaseData("BeforeConvertStaticMethodCallIntoStaticExtensionsCall9", GeneratorJob.ConvertStaticMethodCallToStaticExtensionCall, true)
                    .Returns(ReadAllText("AfterConvertStaticMethodCallIntoStaticExtensionsCall9"))
                    .SetName("var v = StringTools.lpad('-${someVar}', '0', 20) -> var v = '-${someVar}'.lpad('0', 20)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1565");
                yield return new TestCaseData("BeforeConvertStaticMethodCallIntoStaticExtensionsCall10", GeneratorJob.ConvertStaticMethodCallToStaticExtensionCall, true)
                    .Returns(ReadAllText("AfterConvertStaticMethodCallIntoStaticExtensionsCall10"))
                    .SetName("var v = StringTools.lpad('12345'.split('')[0].charCodeAt(0), '0', 20) -> var v = '12345'.split('')[0].charCodeAt(0).lpad('0', 20)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1565");
                yield return new TestCaseData("BeforeConvertStaticMethodCallIntoStaticExtensionsCall11", GeneratorJob.ConvertStaticMethodCallToStaticExtensionCall, true)
                    .Returns(ReadAllText("AfterConvertStaticMethodCallIntoStaticExtensionsCall11"))
                    .SetName("var v = Lambda.count(new Array<Int>()) -> var v = new Array<Int>().count()")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1565");
            }
        }

        static IEnumerable<TestCaseData> ConvertStaticMethodCallToStaticExtensionCallIssue2939TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeConvertStaticMethodCallIntoStaticExtensionsCall_issue2939_1", GeneratorJob.ConvertStaticMethodCallToStaticExtensionCall, true)
                    .Returns(ReadAllText("AfterConvertStaticMethodCallIntoStaticExtensionsCall_issue2939_1"))
                    .SetName("var v = StringTools.trim(' string ', foo()) -> var v = ' string '.trim(foo())")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2939");
            }
        }

        static IEnumerable<TestCaseData> InitializeLocalVariable2762TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeInitializeLocalVariable_issue2762_1", GeneratorJob.InitializeLocalVariable, true)
                    .Returns(ReadAllText("AfterInitializeLocalVariable_issue2762_1"))
                    .SetName("var v<generator>:String; -> var v:String = null; Issue 2762. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2762");
                yield return new TestCaseData("BeforeInitializeLocalVariable_issue2762_2", GeneratorJob.InitializeLocalVariable, false)
                    .Returns(null)
                    .SetName("var v<generator>:String = null; Issue 2762. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2762");
                yield return new TestCaseData("BeforeInitializeLocalVariable_issue2762_3", GeneratorJob.InitializeLocalVariable, true)
                    .Returns(ReadAllText("AfterInitializeLocalVariable_issue2762_3"))
                    .SetName("if (v<generator>); Issue 2762. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2762");
                yield return new TestCaseData("BeforeInitializeLocalVariable_issue2762_4", GeneratorJob.InitializeLocalVariable, false)
                    .Returns(null)
                    .SetName("for (v<generator> in 0...1) Issue 2762. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2762");
                yield return new TestCaseData("BeforeInitializeLocalVariable_issue2762_5", GeneratorJob.InitializeLocalVariable, false)
                    .Returns(null)
                    .SetName("for (v<generator> in 0...1) Issue 2762. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2762");
                yield return new TestCaseData("BeforeInitializeLocalVariable_issue2762_6", GeneratorJob.InitializeLocalVariable, false)
                    .Returns(null)
                    .SetName("function(v<generator>) Issue 2762. Case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2762");
                yield return new TestCaseData("BeforeInitializeLocalVariable_issue2762_7", GeneratorJob.InitializeLocalVariable, false)
                    .Returns(null)
                    .SetName("case v<generator> Issue 2762. Case 7")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2762");
            }
        }

        [
            Test,
            TestCaseSource(nameof(ContextualGeneratorTestCases)),
            TestCaseSource(nameof(ContextualGeneratorForOptionParametersTestCases)),
            TestCaseSource(nameof(ContextualGeneratorIssue2779TestCases)),
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
            TestCaseSource(nameof(AssignStatementToVarIssue1756TestCases)),
            TestCaseSource(nameof(AssignStatementToVarInferParameterVarTestCases)),
            TestCaseSource(nameof(AssignStatementToVarInferParameterVarIssue2350TestCases)),
            TestCaseSource(nameof(AssignStatementToVarInferParameterVarIssue2371TestCases)),
            TestCaseSource(nameof(AssignStatementToVarInferVarTestCases)),
            TestCaseSource(nameof(AssignStatementToVarInferVarIssue2385TestCases)),
            TestCaseSource(nameof(AssignStatementToVarInferVarIssue2444TestCases)),
            TestCaseSource(nameof(AssignStatementToVarInferVarIssue2447TestCases)),
            TestCaseSource(nameof(AssignStatementToVarInferVarIssue2450TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2569TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2217TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2574TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2594TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2725TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2734TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2743TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2830TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2825TestCases)),
            TestCaseSource(nameof(AssignStatementToVarTestCases)),
            TestCaseSource(nameof(AddToInterfaceTestCases)),
            TestCaseSource(nameof(AddToInterfaceIssue1733TestCases)),
            TestCaseSource(nameof(DeclareVariableIssue2558TestCases)),
            TestCaseSource(nameof(GenerateFunctionTestCases)),
            TestCaseSource(nameof(GenerateFunctionIssue2200TestCases)),
            TestCaseSource(nameof(GenerateFunctionIssue394TestCases)),
            TestCaseSource(nameof(GenerateFunctionIssue2293TestCases)),
            TestCaseSource(nameof(GenerateVariableTestCases)),
            TestCaseSource(nameof(GenerateVariableIssue2201TestCases)),
            TestCaseSource(nameof(GenerateVariableIssue1734TestCases)),
            TestCaseSource(nameof(GenerateVariableIssue2477TestCases)),
            TestCaseSource(nameof(GenerateVariableIssue2952TestCases)),
            TestCaseSource(nameof(ImplementInterfaceTestCases)),
            TestCaseSource(nameof(ImplementInterfaceIssue2264TestCases)),
            TestCaseSource(nameof(ImplementInterfaceIssue2531TestCases)),
            TestCaseSource(nameof(ImplementInterfaceIssue2553TestCases)),
            TestCaseSource(nameof(GenerateEventHandlerIssue751TestCases)),
            TestCaseSource(nameof(CreateNewClassIssue2393TestCases)),
            TestCaseSource(nameof(GenerateGetterSetterTestCases)),
            TestCaseSource(nameof(GenerateGetterSetterInAbstractIssue2403TestCases)),
            TestCaseSource(nameof(GenerateGetterSetterInferVar2456TestCases)),
            TestCaseSource(nameof(GenerateGetterSetter2838TestCases)),
            TestCaseSource(nameof(GenerateGetterSetter2477TestCases)),
            TestCaseSource(nameof(GenerateConstructorIssue2845TestCases)),
            TestCaseSource(nameof(GenerateConstructorWithInitializerIssue2872TestCases)),
            TestCaseSource(nameof(InterfaceContextualGeneratorTestCases)),
            TestCaseSource(nameof(NewClassIssue2585TestCases)),
            TestCaseSource(nameof(NewInterfaceIssue2587TestCases)),
            TestCaseSource(nameof(ConvertStaticMethodCallToStaticExtensionCallIssue1565TestCases)),
            TestCaseSource(nameof(ConvertStaticMethodCallToStaticExtensionCallIssue2939TestCases)),
            TestCaseSource(nameof(InitializeLocalVariable2762TestCases)),
            TestCaseSource(nameof(GenerateToStringTestCases)),
        ]
        public string ContextualGenerator(string fileName, GeneratorJobType job, bool hasGenerator) => ContextualGenerator(sci, fileName, job, hasGenerator);

        static string ContextualGenerator(ScintillaControl sci, string fileName, GeneratorJobType job, bool hasGenerator)
        {
            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFileName(fileName);
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
                    var item = options.Find(it => it is GeneratorItem generatorItem && generatorItem.Job == job);
                    Assert.IsNotNull(item);
                    var value = item.Value;
                }
                return sci.Text;
            }
            if (job == (GeneratorJobType) (-1)) Assert.IsEmpty(options);
            if (options.Count > 0) Assert.IsFalse(options.Any(it => it is GeneratorItem item && item.Job == job));
            return null;
        }

        static IEnumerable<TestCaseData> Issue1984_1987_1995_TestCases
        {
            get
            {
                yield return new TestCaseData("ContextualGenerator_issue1984_1", false)
                    .SetName("Issue1984. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                yield return new TestCaseData("ContextualGenerator_issue1984_2", false)
                    .SetName("Issue1984. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                yield return new TestCaseData("ContextualGenerator_issue1984_3", false)
                    .SetName("Issue1984. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                yield return new TestCaseData("ContextualGenerator_issue1984_4", false)
                    .SetName("Issue1984. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                yield return new TestCaseData("ContextualGenerator_issue1984_5", false)
                    .SetName("Issue1984. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                yield return new TestCaseData("ContextualGenerator_issue1984_6", false)
                    .SetName("Issue1984. Case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                yield return new TestCaseData("ContextualGenerator_issue1984_7", false)
                    .SetName("Issue1984. Case 7")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                yield return new TestCaseData("ContextualGenerator_issue1984_8", false)
                    .SetName("Issue1984. Case 8")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                yield return new TestCaseData("ContextualGenerator_issue1984_9", false)
                    .SetName("Issue1984. Case 9")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                yield return new TestCaseData("ContextualGenerator_issue1984_10", false)
                    .SetName("Issue1984. Case 10")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                yield return new TestCaseData("ContextualGenerator_issue1987_1", false)
                    .SetName("Issue1987. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1987");
                yield return new TestCaseData("ContextualGenerator_issue1987_2", false)
                    .SetName("Issue1987. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1987");
                yield return new TestCaseData("ContextualGenerator_issue1987_3", false)
                    .SetName("Issue1987. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1987");
                yield return new TestCaseData("ContextualGenerator_issue1987_4", false)
                    .SetName("Issue1987. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1987");
                yield return new TestCaseData("ContextualGenerator_issue1995_1", false)
                    .SetName("Issue1995. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1995");
                yield return new TestCaseData("ContextualGenerator_issue1995_2", false)
                    .SetName("Issue1995. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1995");
                yield return new TestCaseData("ContextualGenerator_issue1995_3", false)
                    .SetName("Issue1995. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1995");
                yield return new TestCaseData("ContextualGenerator_issue1995_4", true)
                    .SetName("Issue1995. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1995");
                yield return new TestCaseData("ContextualGenerator_issue1995_5", true)
                    .SetName("Issue1995. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1995");
                yield return new TestCaseData("ContextualGenerator_issue1995_6", false)
                    .SetName("Issue1995. Case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1995");
                yield return new TestCaseData("ContextualGenerator_issue1995_7", false)
                    .SetName("Issue1995. Case 7")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1995");
                yield return new TestCaseData("ContextualGenerator_issue1995_8", false)
                    .SetName("Issue1995. Case 8")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2005");
            }
        }

        [Test, TestCaseSource(nameof(Issue1984_1987_1995_TestCases))]
        public void HasContextualGenerator(string fileName, bool hasGenerator)
        {
            SetCurrentFileName(GetFullPath(fileName));
            SetSrc(sci, ReadAllText(fileName));
            var options = new List<ICompletionListItem>();
            ASGenerator.ContextualGenerator(sci, options);
            if (hasGenerator) Assert.IsNotEmpty(options);
            else Assert.IsEmpty(options);
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
            SetCurrentFileName(fileName);
            UITools.Init();
            CompletionList.CreateControl(PluginBase.MainForm);
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
            SetCurrentFileName(fileName);
            return ASGenerator.ParseFunctionParameters(sci, sci.CurrentPos).Count;
        }

        static IEnumerable<TestCaseData> ParseFunctionParametersTestCases2
        {
            get
            {
                yield return new TestCaseData("ParseFunctionParameters_String")
                    .Returns(new List<MemberModel> { new ClassModel { Name = "String", InFile = FileModel.Ignore } })
                    .SetName("Parse function parameters of foo(\"string\")");
                yield return new TestCaseData("ParseFunctionParameters_String_2")
                    .Returns(new List<MemberModel> { new ClassModel { Name = "String", InFile = FileModel.Ignore } })
                    .SetName("Parse function parameters of foo('string')");
                yield return new TestCaseData("ParseFunctionParameters_Boolean")
                    .Returns(new List<MemberModel> { new ClassModel { Name = "Bool", InFile = FileModel.Ignore } })
                    .SetName("Parse function parameters of foo(true)");
                yield return new TestCaseData("ParseFunctionParameters_Boolean_false")
                    .Returns(new List<MemberModel> { new ClassModel { Name = "Bool", InFile = FileModel.Ignore } })
                    .SetName("Parse function parameters of foo(false)");
                yield return new TestCaseData("ParseFunctionParameters_Digit")
                    .Returns(new List<MemberModel> { new ClassModel { Name = "Int", InFile = FileModel.Ignore } })
                    .SetName("Parse function parameters of foo(1)");
                yield return new TestCaseData("ParseFunctionParameters_Digit_2")
                    .Returns(new List<MemberModel> { new ClassModel { Name = "Float", InFile = FileModel.Ignore } })
                    .SetName("Parse function parameters of foo(1)");
                yield return new TestCaseData("ParseFunctionParameters_Array")
                    .Returns(new List<MemberModel> { new ClassModel { Name = "Array", InFile = FileModel.Ignore } })
                    .SetName("Parse function parameters of foo(new Array())");
                yield return new TestCaseData("ParseFunctionParameters_TypedArray")
                    .Returns(new List<MemberModel> { new ClassModel { Name = "Array<Int>", InFile = FileModel.Ignore } })
                    .SetName("Parse function parameters of foo(new Array<Int>())");
                yield return new TestCaseData("ParseFunctionParameters_TypedArray_2")
                    .Returns(new List<MemberModel> { new ClassModel { Name = "Array<Int->{x:Int, y:Int}>", InFile = FileModel.Ignore } })
                    .SetName("Parse function parameters of foo(new Array<Int->{x:Int, y:Int}>())");
                yield return new TestCaseData("ParseFunctionParameters_ArrayInitializer")
                    .Returns(new List<MemberModel> { new ClassModel { Name = "Array<T>", InFile = FileModel.Ignore } })
                    .SetName("Parse function parameters of foo([])");
                yield return new TestCaseData("ParseFunctionParameters_ArrayInitializer_2")
                    .Returns(new List<MemberModel> { new ClassModel { Name = "Array<T>", InFile = FileModel.Ignore } })
                    .SetName("Parse function parameters of foo([{v:[1,2,3,4]}])");
                yield return new TestCaseData("ParseFunctionParameters_ObjectInitializer")
                    .Returns(new List<MemberModel> { new ClassModel { Name = "Dynamic", InFile = FileModel.Ignore } })
                    .SetName("Parse function parameters of foo({})");
                yield return new TestCaseData("ParseFunctionParameters_ObjectInitializer_2")
                    .Returns(new List<MemberModel> { new ClassModel { Name = "Dynamic", InFile = FileModel.Ignore } })
                    .SetName("Parse function parameters of foo({key:'value'})");
                yield return new TestCaseData("ParseFunctionParameters_ObjectInitializer_3")
                    .Returns(new List<MemberModel> { new ClassModel { Name = "Dynamic", InFile = FileModel.Ignore } })
                    .SetName("Parse function parameters of foo({v:[{key:'value'}]})");
                yield return new TestCaseData("ParseFunctionParameters_ArrayAccess")
                    .Returns(new List<MemberModel> { new ClassModel { Name = "Int", InFile = FileModel.Ignore } })
                    .SetName("Parse function parameters of foo(a[0][0].length)");
                yield return new TestCaseData("ParseFunctionParameters_Function")
                    .Returns(new List<MemberModel> { new ClassModel { Name = "Function", InFile = FileModel.Ignore } })
                    .SetName("Parse function parameters of foo(function() {})");
                yield return new TestCaseData("ParseFunctionParameters_Math.random.1.5")
                    .Returns(new List<MemberModel> { new ClassModel { Name = "Float", InFile = FileModel.Ignore } })
                    .SetName("Parse function parameters of foo(Math.random(1.5))");
                yield return new TestCaseData("ParseFunctionParameters_complexExpr")
                    .Returns(new List<MemberModel> { new ClassModel { Name = "DisplayObject", InFile = FileModel.Ignore } })
                    .SetName("Parse function parameters of foo(new Sprite().addChild(new Sprite()))");
            }
        }

        [Test, TestCaseSource(nameof(ParseFunctionParametersTestCases2))]
        public List<MemberModel> ParseFunctionParameters2(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFileName(fileName);
            var context = (Context)ASContext.GetLanguageContext("haxe");
            context.CurrentModel = ASContext.Context.CurrentModel;
            context.completionCache.IsDirty = true;
            context.GetTopLevelElements();
            var visibleExternalElements = context.GetVisibleExternalElements();
            ASContext.Context.GetVisibleExternalElements().Returns(visibleExternalElements);
            var result = ASGenerator.ParseFunctionParameters(sci, sci.CurrentPos).Select(it => it.result.Type ?? it.result.Member).ToList();
            return result;
        }

        static IEnumerable<TestCaseData> DisableVoidTypeDeclarationForFunctionsIssue2613TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2613_1", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2613_1"))
                    .SetName("fo|o(~/regex/). Generate private function")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2613");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2613_2", GeneratorJobType.FunctionPublic, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2613_2"))
                    .SetName("fo|o(~/regex/). Generate public function")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2613");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2613_3", GeneratorJobType.ImplementInterface, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_issue2613_3"))
                    .SetName("implements IFoo<generator>. Implement interface methods")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2613");
            }
        }

        [
            Test,
            TestCaseSource(nameof(DisableVoidTypeDeclarationForFunctionsIssue2613TestCases))
        ]
        public string DisableVoidTypeDeclarationForFunctionsIssue2613(string fileName, GeneratorJobType job, bool hasGenerator)
        {
            ((HaXeSettings) ASContext.Context.Settings).DisableVoidTypeDeclaration = true;
            var result = ContextualGenerator(sci, fileName, job, hasGenerator);
            ((HaXeSettings) ASContext.Context.Settings).DisableVoidTypeDeclaration = false;
            return result;
        }

        static IEnumerable<TestCaseData> GenerateFieldFromParameterTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateFieldFromParameter", GeneratorJobType.FieldFromParameter, Visibility.Private)
                    .Returns(ReadAllText("AfterGenerateFieldFromParameter"));
                yield return new TestCaseData("BeforeGenerateFieldFromOptionalParameter", GeneratorJobType.FieldFromParameter, Visibility.Private)
                    .Returns(ReadAllText("AfterGenerateFieldFromOptionalParameter"));
                yield return new TestCaseData("BeforeGenerateFieldFromOptionalUntypedParameter", GeneratorJobType.FieldFromParameter, Visibility.Private)
                    .Returns(ReadAllText("AfterGenerateFieldFromOptionalUntypedParameter"));
                yield return new TestCaseData("BeforeGenerateFieldFromOptionalParameter2", GeneratorJobType.FieldFromParameter, Visibility.Private)
                    .Returns(ReadAllText("AfterGenerateFieldFromOptionalParameter2"));
            }
        }

        [
            Test,
            TestCaseSource(nameof(GenerateFieldFromParameterTestCases))
        ]
        public string GenerateFieldFromParameter(string fileName, GeneratorJobType job, Visibility scope)
        {
            SetSrc(sci, ReadAllText(fileName));
            ASGenerator.SetJobContext(null, null, ASContext.Context.CurrentMember.Parameters.First(), null);
            ASGenerator.GenerateJob(job, ASContext.Context.CurrentMember, ASContext.Context.CurrentClass, null, new Hashtable {["scope"] = scope});
            return sci.Text;
        }

        static IEnumerable<TestCaseData> GenerateClassTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateClassTest_issue1762_1", "$(Boundary)dynamicValue:Dynamic$(Boundary)")
                    .SetName("Issue1762. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1762");
                yield return new TestCaseData("BeforeGenerateClassTest_issue2255_1", string.Empty)
                    .SetName("Issue2255. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2255");
                yield return new TestCaseData("BeforeGenerateClassTest_issue2255_2", string.Empty)
                    .SetName("Issue2255. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2255");
                yield return new TestCaseData("BeforeGenerateClassTest_issue2255_3", string.Empty)
                    .SetName("Issue2255. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2255");
            }
        }

        [Test, TestCaseSource(nameof(GenerateClassTestCases))]
        public void GenerateClass(string fileName, string constructorArgs)
        {
            var handler = Substitute.For<IEventHandler>();
            handler
                .When(it => it.HandleEvent(Arg.Any<object>(), Arg.Any<NotifyEvent>(), Arg.Any<HandlingPriority>()))
                .Do(it =>
                {
                    var e = it.ArgAt<NotifyEvent>(1);
                    switch (e.Type)
                    {
                        case EventType.Command:
                            EventManager.RemoveEventHandler(handler);
                            e.Handled = true;
                            var de = (DataEvent)e;
                            var info = (Hashtable)de.Data;
                            var actualArgs = (string)info[nameof(constructorArgs)];
                            Assert.AreEqual(constructorArgs, actualArgs);
                            break;
                    }
                });
            EventManager.AddEventHandler(handler, EventType.Command);
            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFileName(GetFullPath(fileName));
            var options = new List<ICompletionListItem>();
            ASGenerator.ContextualGenerator(sci, options);
            var item = options.Find(it => ((GeneratorItem)it).Job == GeneratorJobType.Class);
            var value = item.Value;
        }

        static IEnumerable<TestCaseData> GenerateClassIssue2477TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateClassTest_issue2477_1", GeneratorJobType.Class, "<T>")
                    .SetName("abstract A(NewClass$(EntryPoint)<String>). Issue2477. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2477");
                yield return new TestCaseData("BeforeGenerateClassTest_issue2477_2", GeneratorJobType.Class, "<T>")
                    .SetName("abstract A() from NewClass$(EntryPoint)<String>. Issue2477. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2477");
                yield return new TestCaseData("BeforeGenerateClassTest_issue2477_3", GeneratorJobType.Class, "<T>")
                    .SetName("abstract A() to NewClass$(EntryPoint)<String>. Issue2477. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2477");
            }
        }

        static IEnumerable<TestCaseData> GenerateClassIssue2589TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateClassTest_issue2589_1", GeneratorJobType.Class, "<T>")
                    .SetName("new NewClass$(EntryPoint)<String>. Issue2589. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2589");
                yield return new TestCaseData("BeforeGenerateClassTest_issue2589_2", GeneratorJobType.Class, "<T1, T2>")
                    .SetName("new NewClass$(EntryPoint)<Map<Int, String>, Int>. Issue2589. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2589");
                yield return new TestCaseData("BeforeGenerateClassTest_issue2589_3", GeneratorJobType.Class, "<T1, T2, T3>")
                    .SetName("new NewClass$(EntryPoint)<Map<Int, String>, Int, /*, Int>*/ Array<{x:Int, y:Int}->String>>. Issue2589. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2589");
                yield return new TestCaseData("BeforeGenerateClassTest_issue2589_4", GeneratorJobType.Class, "<T1, T2, T3>")
                    .SetName("new NewClass$(EntryPoint)/*<Int>*/<Map<Int, String>, Int, /*, Int>*/ Array<{x:Int, y:Int}->String>>. Issue2589. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2589");
            }
        }

        static IEnumerable<TestCaseData> GenerateInterfaceIssue2477TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateClassTest_issue2477_1", GeneratorJobType.Interface, "<T>")
                    .SetName("abstract A(NewInterface$(EntryPoint)<String>). Issue2477. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2477");
                yield return new TestCaseData("BeforeGenerateClassTest_issue2477_2", GeneratorJobType.Interface, "<T>")
                    .SetName("abstract A() from NewInterface$(EntryPoint)<String>. Issue2477. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2477");
                yield return new TestCaseData("BeforeGenerateClassTest_issue2477_3", GeneratorJobType.Interface, "<T>")
                    .SetName("abstract A() to NewInterface$(EntryPoint)<String>. Issue2477. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2477");
            }
        }

        static IEnumerable<TestCaseData> GenerateInterfaceIssue2870TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateInterfaceTest_issue2870_1", GeneratorJobType.Interface, "<T>")
                    .SetName("var v:INewInterface$(EntryPoint)<String>. Issue2870. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2870");
            }
        }

        [
            Test,
            TestCaseSource(nameof(GenerateClassIssue2477TestCases)),
            TestCaseSource(nameof(GenerateClassIssue2589TestCases)),
            TestCaseSource(nameof(GenerateInterfaceIssue2477TestCases)),
            TestCaseSource(nameof(GenerateInterfaceIssue2870TestCases)),
        ]
        public void GenerateNewType(string fileName, GeneratorJobType job, string classTemplate)
        {
            var handler = Substitute.For<IEventHandler>();
            handler
                .When(it => it.HandleEvent(Arg.Any<object>(), Arg.Any<NotifyEvent>(), Arg.Any<HandlingPriority>()))
                .Do(it =>
                {
                    var e = it.ArgAt<NotifyEvent>(1);
                    switch (e.Type)
                    {
                        case EventType.Command:
                            EventManager.RemoveEventHandler(handler);
                            e.Handled = true;
                            var de = (DataEvent)e;
                            var info = (Hashtable)de.Data;
                            var actualArgs = (string)info[nameof(classTemplate)];
                            Assert.AreEqual(classTemplate, actualArgs);
                            break;
                    }
                });
            EventManager.AddEventHandler(handler, EventType.Command);
            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFileName(GetFullPath(fileName));
            var options = new List<ICompletionListItem>();
            ASGenerator.ContextualGenerator(sci, options);
            var item = options.Find(it => ((GeneratorItem)it).Job == job);
            var value = item.Value;
        }

        static IEnumerable<TestCaseData> ChangeConstructorDeclarationTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_String")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_String"))
                    .SetName("new Foo(\"\") -> function new(string:String)");
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_String2")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_String2"))
                    .SetName("new Foo(\"\", \"\") -> function new(string:String, string1:String)");
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_Digit")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_Digit"))
                    .SetName("new Foo(1) -> function new(int:Int)");
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_Digit_2")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_Digit_2"))
                    .SetName("new Foo(1.0) -> function new(float:Float)");
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_Boolean")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_Boolean"))
                    .SetName("new Foo(true) -> function new(bool:Bool)");
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_ItemOfTwoDimensionalArrayInitializer")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_ItemOfTwoDimensionalArrayInitializer"))
                    .SetName("new Foo(strings[0][0]) -> function new(string:String)");
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_Dynamic")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_Dynamic"))
                    .SetName("new Foo({}) -> function new(dynamicValue:Dynamic)");
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_issue1712_1")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_issue1712_1"))
                    .SetName("new Foo(new Array<haxe.Timer->Type.ValueType>()) -> function Foo(array:haxe.Timer->Type.ValueType)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1712");
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_issue1712_2")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_issue1712_2"))
                    .SetName("new Foo(new haxe.ds.Vector<Int>(0)) -> function Foo(vector:haxe.ds.Vector<Int>)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1712");
            }
        }

        [Test, TestCaseSource(nameof(ChangeConstructorDeclarationTestCases))]
        public string ChangeConstructorDeclaration(string fileName)
        {
            SetCurrentFileName(GetFullPath(fileName));
            SetSrc(sci, ReadAllText(fileName));
            ASGenerator.GenerateJob(GeneratorJobType.ChangeConstructorDecl, ASContext.Context.CurrentMember, ASContext.Context.CurrentClass, null, null);
            return sci.Text;
        }

        static IEnumerable<TestCaseData> GenerateDelegateMethodsTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateDelegateMethod")
                    .Returns(ReadAllText("AfterGenerateDelegateMethod"));
            }
        }

        [Test, TestCaseSource(nameof(GenerateDelegateMethodsTestCases))]
        public string GenerateDelegateMethods(string fileName)
        {
            SetCurrentFileName(GetFullPath(fileName));
            SetSrc(sci, ReadAllText(fileName));
            var type = ASContext.Context.ResolveType(ASContext.Context.CurrentMember.Type, ASContext.Context.CurrentModel);
            var selectedMembers = type.Members.ToDictionary(it => it, it => ASContext.Context.ResolveType(it.Type, it.InFile));
            ASGenerator.GenerateDelegateMethods(sci, ASContext.Context.CurrentMember, selectedMembers, type, ASContext.Context.CurrentClass);
            return sci.Text;
        }

        static IEnumerable<TestCaseData> AvoidKeywordTestCases
        {
            get
            {
                yield return new TestCaseData("import").Returns("importValue");
                yield return new TestCaseData("new").Returns("newValue");
                yield return new TestCaseData("extends").Returns("extendsValue");
                yield return new TestCaseData("implements").Returns("implementsValue");
                yield return new TestCaseData("using").Returns("usingValue");
                yield return new TestCaseData("var").Returns("varValue");
                yield return new TestCaseData("function").Returns("functionValue");
                yield return new TestCaseData("cast").Returns("castValue");
                yield return new TestCaseData("return").Returns("returnValue");
                yield return new TestCaseData("break").Returns("breakValue");
                yield return new TestCaseData("continue").Returns("continueValue");
                yield return new TestCaseData("if").Returns("ifValue");
                yield return new TestCaseData("else").Returns("elseValue");
                yield return new TestCaseData("for").Returns("forValue");
                yield return new TestCaseData("in").Returns("inValue");
                yield return new TestCaseData("while").Returns("whileValue");
                yield return new TestCaseData("do").Returns("doValue");
                yield return new TestCaseData("switch").Returns("switchValue");
                yield return new TestCaseData("case").Returns("caseValue");
                yield return new TestCaseData("default").Returns("defaultValue");
                yield return new TestCaseData("untyped").Returns("untypedValue");
                yield return new TestCaseData("null").Returns("nullValue");
                yield return new TestCaseData("true").Returns("trueValue");
                yield return new TestCaseData("false").Returns("falseValue");
                yield return new TestCaseData("try").Returns("tryValue");
                yield return new TestCaseData("catch").Returns("catchValue");
                yield return new TestCaseData("throw").Returns("throwValue");
                yield return new TestCaseData("trace").Returns("traceValue");
                yield return new TestCaseData("macro").Returns("macroValue");
                yield return new TestCaseData("dynamic").Returns("dynamicValue");
                yield return new TestCaseData("private").Returns("privateValue");
                yield return new TestCaseData("public").Returns("publicValue");
                yield return new TestCaseData("inline").Returns("inlineValue");
                yield return new TestCaseData("extern").Returns("externValue");
                yield return new TestCaseData("static").Returns("staticValue");
                yield return new TestCaseData("override").Returns("overrideValue");
                yield return new TestCaseData("class").Returns("classValue");
                yield return new TestCaseData("interface").Returns("interfaceValue");
                yield return new TestCaseData("typedef").Returns("typedefValue");
                yield return new TestCaseData("enum").Returns("enumValue");
                yield return new TestCaseData("abstract").Returns("abstractValue");
            }
        }

        [Test, TestCaseSource(nameof(AvoidKeywordTestCases))]
        public string AvoidKeyword(string sourceText) => ASGenerator.AvoidKeyword(sourceText);

        static IEnumerable<TestCaseData> GetEndOfStatementTestCases
        {
            get
            {
                yield return new TestCaseData("foo(/*:)*/)\nbar()\n   ")
                    .Returns("foo(/*:)*/)\n".Length);
                yield return new TestCaseData("foo(\"(.)(.) <-- :)\")\nbar()\n   ")
                    .Returns("foo(\"(.)(.) <-- :)\")\n".Length);
                yield return new TestCaseData("foo('(.)(.) <-- :)')\nbar()\n   ")
                    .Returns("foo('(.)(.) <-- :)')\n".Length);
                yield return new TestCaseData("foo('\\'(.)(.) <-- :)\\'')\nbar()\n   ")
                    .Returns("foo('\\'(.)(.) <-- :)\\'')\n".Length);
            }
        }

        static IEnumerable<TestCaseData> GetStartOfStatementTestCases
        {
            get
            {
                yield return new TestCaseData(" new Array<Int>()$(EntryPoint)", new ASResult { Type = new ClassModel { Flags = FlagType.Class }, Context = new ASExpr { WordBefore = "new", WordBeforePosition = 1 } })
                    .Returns(1);
                yield return new TestCaseData(" new Map<String, Int>()$(EntryPoint)", new ASResult { Type = new ClassModel { Flags = FlagType.Class }, Context = new ASExpr { WordBefore = "new", WordBeforePosition = 1 } })
                    .Returns(1);
                yield return new TestCaseData(" new Map<String, Map<String, Int>>()$(EntryPoint)", new ASResult { Type = new ClassModel { Flags = FlagType.Class }, Context = new ASExpr { WordBefore = "new", WordBeforePosition = 1 } })
                    .Returns(1);
                yield return new TestCaseData(" new Map<String, Map<String, Void->Int>>()$(EntryPoint)", new ASResult { Type = new ClassModel { Flags = FlagType.Class }, Context = new ASExpr { WordBefore = "new", WordBeforePosition = 1 } })
                    .Returns(1);
                yield return new TestCaseData(" new Map<String, Map<String, String->Int->Void>>()$(EntryPoint)", new ASResult { Type = new ClassModel { Flags = FlagType.Class }, Context = new ASExpr { WordBefore = "new", WordBeforePosition = 1 } })
                    .Returns(1);
                yield return new TestCaseData(" new Array<Int->Int->String>()$(EntryPoint)", new ASResult { Type = new ClassModel { Flags = FlagType.Class }, Context = new ASExpr { WordBefore = "new", WordBeforePosition = 1 } })
                    .Returns(1);
                yield return new TestCaseData(" new Array<{x:Int, y:Int}>()$(EntryPoint)", new ASResult { Type = new ClassModel { Flags = FlagType.Class }, Context = new ASExpr { WordBefore = "new", WordBeforePosition = 1 } })
                    .Returns(1);
                yield return new TestCaseData(" new Array<{name:String, params:Array<Dynamic>}>()$(EntryPoint)", new ASResult { Type = new ClassModel { Flags = FlagType.Class }, Context = new ASExpr { WordBefore = "new", WordBeforePosition = 1 } })
                    .Returns(1);
                yield return new TestCaseData(" new Array<{name:String, factory:String->Dynamic}>()$(EntryPoint)", new ASResult { Type = new ClassModel { Flags = FlagType.Class }, Context = new ASExpr { WordBefore = "new", WordBeforePosition = 1 } })
                    .Returns(1);
                yield return new TestCaseData(" new Array<{name:String, factory:String->Array<String>}>()$(EntryPoint)", new ASResult { Type = new ClassModel { Flags = FlagType.Class }, Context = new ASExpr { WordBefore = "new", WordBeforePosition = 1 } })
                    .Returns(1);
                yield return new TestCaseData(" new Array<{name:String, factory:String->{x:Int, y:Int}}>()$(EntryPoint)", new ASResult { Type = new ClassModel { Flags = FlagType.Class }, Context = new ASExpr { WordBefore = "new", WordBeforePosition = 1 } })
                    .Returns(1);
                yield return new TestCaseData(" [1 => 1, 2 => 2]$(EntryPoint)", new ASResult { Type = ClassModel.VoidClass, Context = new ASExpr { PositionExpression = 1 } })
                    .Returns(1);
                yield return new TestCaseData(" (1 > 2 ? 1 : 2)$(EntryPoint)", new ASResult { Type = ClassModel.VoidClass, Context = new ASExpr { PositionExpression = 1 } })
                    .Returns(1);
                yield return new TestCaseData(" {v:1 > 2 ? 1 : 2}$(EntryPoint)", new ASResult { Type = ClassModel.VoidClass, Context = new ASExpr { PositionExpression = 1 } })
                    .Returns(1);
                yield return new TestCaseData(" [new Array<String>()]$(EntryPoint)", new ASResult { Type = ClassModel.VoidClass, Context = new ASExpr { PositionExpression = 1 } })
                    .Returns(1);
                yield return new TestCaseData(" test(type:Class<Dynamic>)$(EntryPoint)", new ASResult { Type = ClassModel.VoidClass, Context = new ASExpr { PositionExpression = 1 } })
                    .Returns(1);
            }
        }

        [Test, TestCaseSource(nameof(GetStartOfStatementTestCases))]
        public int GetStartOfStatement(string sourceText, ASResult expr)
        {
            SetSrc(sci, sourceText);
            return ASGenerator.GetStartOfStatement(expr);
        }

        [Test, TestCaseSource(nameof(GetEndOfStatementTestCases))]
        public int GetEndOfStatement(string sourceText)
        {
            SetSrc(sci, sourceText);
            return ASGenerator.GetEndOfStatement(0, sci.TextLength, sci);
        }
        
        static IEnumerable<TestCaseData> PromoteLocalTestCases
        {
            get
            {
                yield return new TestCaseData("BeforePromoteLocal")
                    .Returns(ReadAllText("AfterPromoteLocal_generateExplicitScopeIsFalse"))
                    .SetName("Promote to class member");
            }
        }

        static IEnumerable<TestCaseData> PromoteLocalWithExplicitScopeTestCases
        {
            get
            {
                yield return new TestCaseData("BeforePromoteLocal")
                    .Returns(ReadAllText("AfterPromoteLocal_generateExplicitScopeIsTrue"))
                    .SetName("Promote to class member");
            }
        }

        [Test, TestCaseSource(nameof(PromoteLocalWithExplicitScopeTestCases))]
        public string PromoteLocalWithExplicitScope(string fileName)
        {
            ASContext.CommonSettings.GenerateScope = true;
            var result = PromoteLocal(fileName);
            ASContext.CommonSettings.GenerateScope = false;
            return result;
        }

        static IEnumerable<TestCaseData> PromoteLocalWithDefaultModifierDeclarationTestCases
        {
            get
            {
                yield return new TestCaseData("BeforePromoteLocal")
                    .Returns(ReadAllText("AfterPromoteLocalWithDefaultModifier"))
                    .SetName("Promote to private class member with default modifier declaration");
            }
        }

        [Test, TestCaseSource(nameof(PromoteLocalWithDefaultModifierDeclarationTestCases))]
        public string PromoteLocalWithDefaultModifierDeclaration(string fileName)
        {
            ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
            var result = PromoteLocal(fileName);
            ASContext.CommonSettings.GenerateDefaultModifierDeclaration = false;
            return result;
        }

        [Test, TestCaseSource(nameof(PromoteLocalTestCases))]
        public string PromoteLocal(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            var expr = ASComplete.GetExpressionType(sci, sci.CurrentPos);
            ASGenerator.contextMember = expr.Context.LocalVars[0];
            var options = new List<ICompletionListItem>();
            ASGenerator.ContextualGenerator(sci, options);
            var item = options.Find(it => ((GeneratorItem)it).Job == GeneratorJobType.PromoteLocal);
            var value = item.Value;
            return sci.Text;
        }

        static IEnumerable<TestCaseData> AssignStatementToVariableTestCases
        {
            get
            {

                yield return
                    new TestCaseData("BeforeAssignStatementToVar_useSpaces", GeneratorJobType.AssignStatementToVar, false)
                        .Returns(ReadAllText("AfterAssignStatementToVar_useSpaces"))
                        .SetName("Assign statement to var. Use spaces instead of tabs.");
                yield return
                    new TestCaseData("BeforeAssignStatementToVar_useTabs", GeneratorJobType.AssignStatementToVar, true)
                        .Returns(ReadAllText("AfterAssignStatementToVar_useTabs"))
                        .SetName("Assign statement to var. Use tabs instead of spaces.");
                yield return
                    new TestCaseData("BeforeAssignStatementToVarFromFieldOfItemOfArray", GeneratorJobType.AssignStatementToVar, true)
                        .Returns(ReadAllText("AfterAssignStatementToVarFromFieldOfItemOfArray"))
                        .SetName("from a[0][0].length");
                yield return
                    new TestCaseData("BeforeAssignStatementToVarFromNewMap", GeneratorJobType.AssignStatementToVar, true)
                        .Returns(ReadAllText("AfterAssignStatementToVarFromNewMap"))
                        .SetName("from new Map<String, Int>()");
                yield return
                    new TestCaseData("BeforeAssignStatementToVarFromNewMap2", GeneratorJobType.AssignStatementToVar, true)
                        .Returns(ReadAllText("AfterAssignStatementToVarFromNewMap2"))
                        .SetName("from new Map<Map<String, Int>, Int>()");
                yield return
                    new TestCaseData("BeforeAssignStatementToVarFromNewMap3", GeneratorJobType.AssignStatementToVar, true)
                        .Returns(ReadAllText("AfterAssignStatementToVarFromNewMap3"))
                        .SetName("from new Map<String, Array<Map<String, Int>>>()");
                yield return
                    new TestCaseData("BeforeAssignStatementToVarFromNewMap4", GeneratorJobType.AssignStatementToVar, true)
                        .Returns(ReadAllText("AfterAssignStatementToVarFromNewMap4"))
                        .SetName("from new Map<String, Array<Map<String, Int->Int->Int>>>()");
                yield return
                    new TestCaseData("BeforeAssignStatementToVarFromCallback_useSpaces", GeneratorJobType.AssignStatementToVar, false)
                        .Returns(ReadAllText("AfterAssignStatementToVarFromCallback_useSpaces"))
                        .SetName("from callback");
                yield return
                    new TestCaseData("BeforeAssignStatementToVarFromCallback2_useSpaces", GeneratorJobType.AssignStatementToVar, false)
                        .Returns(ReadAllText("AfterAssignStatementToVarFromCallback2_useSpaces"))
                        .SetName("from callback 2");
                yield return
                    new TestCaseData("BeforeAssignStatementToVarFromCallback3_useSpaces", GeneratorJobType.AssignStatementToVar, false)
                        .Returns(ReadAllText("AfterAssignStatementToVarFromCallback3_useSpaces"))
                        .SetName("from callback 3");
                yield return
                    new TestCaseData("BeforeAssignStatementToVarFromClass_useSpaces", GeneratorJobType.AssignStatementToVar, false)
                        .Returns(ReadAllText("AfterAssignStatementToVarFromClass_useSpaces"))
                        .SetName("from Class");
                yield return
                    new TestCaseData("BeforeAssignStatementToVarFromArray_useSpaces", GeneratorJobType.AssignStatementToVar, false)
                        .Returns(ReadAllText("AfterAssignStatementToVarFromArray_useSpaces"))
                        .SetName("from new Array<Int>()");
                yield return
                    new TestCaseData("BeforeAssignStatementToVarFromArray2_useSpaces", GeneratorJobType.AssignStatementToVar, false)
                        .Returns(ReadAllText("AfterAssignStatementToVarFromArray2_useSpaces"))
                        .SetName("from new Array<Int->Int>()");
                yield return
                    new TestCaseData("BeforeAssignStatementToVarFromArray3_useSpaces", GeneratorJobType.AssignStatementToVar, false)
                        .Returns(ReadAllText("AfterAssignStatementToVarFromArray3_useSpaces"))
                        .SetName("from new Array<{name:String, factory:String->{x:Int, y:Int}}>()");
                yield return
                    new TestCaseData("BeforeAssignStatementToVarFromDynamic_useSpaces", GeneratorJobType.AssignStatementToVar, false)
                        .Returns(ReadAllText("AfterAssignStatementToVarFromDynamic_useSpaces"))
                        .SetName("from {}");
                yield return
                    new TestCaseData("BeforeAssignStatementToVarFromCastExp", GeneratorJobType.AssignStatementToVar, false)
                        .Returns(ReadAllText("AfterAssignStatementToVarFromCastExp"))
                        .SetName("cast(d, String)");
                yield return
                    new TestCaseData("BeforeAssignStatementToVarFromCastExp2", GeneratorJobType.AssignStatementToVar, false)
                        .Returns(ReadAllText("AfterAssignStatementToVarFromCastExp2"))
                        .SetName("cast (d, String)");
                yield return
                    new TestCaseData("BeforeAssignStatementToVarFromCastExp3", GeneratorJobType.AssignStatementToVar, false)
                        .Returns(ReadAllText("AfterAssignStatementToVarFromCastExp3"))
                        .SetName("cast ( d, String )");
                yield return
                    new TestCaseData("BeforeAssignStatementToVar_issue1696_1", GeneratorJobType.AssignStatementToVar, true)
                        .Returns(ReadAllText("AfterAssignStatementToVar_issue1696_1"))
                        .SetName("issue 1696")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1696");
                yield return
                    new TestCaseData("BeforeAssignStatementToVar_issue_1704_1", GeneratorJobType.AssignStatementToVar, false)
                        .Returns(ReadAllText("AfterAssignStatementToVar_issue_1704_1"))
                        .SetName("from (function foo():haxe.ds.Vector<haxe.Timer->Type.ValueType> ...)()")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1704");
                yield return
                    new TestCaseData("BeforeAssignStatementToVar_issue_1704_2", GeneratorJobType.AssignStatementToVar, false)
                        .Returns(ReadAllText("AfterAssignStatementToVar_issue_1704_2"))
                        .SetName("from (function foo():haxe.ds.Vector<haxe.Timer> ...)()")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1704");
                yield return
                    new TestCaseData("BeforeAssignStatementToVar_issue_1704_3", GeneratorJobType.AssignStatementToVar, false)
                        .Returns(ReadAllText("AfterAssignStatementToVar_issue_1704_3"))
                        .SetName("from (function foo():haxe.Timer ...)()")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1704");
                yield return
                    new TestCaseData("BeforeAssignStatementToVar_issue_1704_4", GeneratorJobType.AssignStatementToVar, false)
                        .Returns(ReadAllText("AfterAssignStatementToVar_issue_1704_4"))
                        .SetName("from (function foo():haxe.Timer->{v:haxe.ds.Vector<Int>->Type.ValueType} ...)()")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1704");
                yield return
                    new TestCaseData("BeforeAssignStatementToVar_issue1749_6", GeneratorJobType.AssignStatementToVar, true)
                        .Returns(ReadAllText("AfterAssignStatementToVar_issue1749_6"))
                        .SetName("Issue 1749. Modulo")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1749");
                yield return
                    new TestCaseData("BeforeAssignStatementToVar_issue_1766", GeneratorJobType.AssignStatementToVar, false)
                        .Returns(ReadAllText("AfterAssignStatementToVar_issue_1766"))
                        .SetName("from [1 => '1', 2 = '2']")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1704");
                yield return
                    new TestCaseData("BeforeAssignStatementToVar_operator_is", GeneratorJobType.AssignStatementToVar, true)
                        .Returns(ReadAllText("AfterAssignStatementToVar_operator_is"))
                        .SetName("Issue 1918. (v is String)")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1918");
                yield return
                    new TestCaseData("BeforeAssignStatementToVar_issue1908_unsafecast", GeneratorJobType.AssignStatementToVar, true)
                        .Returns(ReadAllText("AfterAssignStatementToVar_issue1908_unsafecast"))
                        .SetName("Issue 1908. Unsafe cast")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1908");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1908_untyped", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1908_untyped"))
                    .SetName("Issue 1908. untyped")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1908");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1880_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1880_1"))
                    .SetName("~/regex/|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1908");
            }
        }

        [Test, TestCaseSource(nameof(AssignStatementToVariableTestCases))]
        public string AssignStatementToVariable(string fileName, GeneratorJobType job, bool isUseTabs)
        {
            sci.IsUseTabs = isUseTabs;
            SetSrc(sci, ReadAllText(fileName));
            var list = new MemberList();
            list.Merge(ASContext.GetLanguageContext(sci.ConfigurationLanguage).GetVisibleExternalElements());
            list.Merge(ASContext.Context.CurrentModel.Imports);
            ASContext.Context.GetVisibleExternalElements().Returns(list);
            ASGenerator.GenerateJob(job, ASContext.Context.CurrentMember, ASContext.Context.CurrentClass, null, null);
            return sci.Text;
        }

        static IEnumerable<TestCaseData> AssignStatementToVariableSdk330TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1992_typecheck", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1992_typecheck"))
                    .SetName("Issue 1992. from (v:Iterable<Dynamic>)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1908");
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1992_typecheck_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1992_typecheck_2"))
                    .SetName("Issue 1992. from (v:Iterable<Iterable<Dynamic>>)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1908");
            }
        }

        [Test, TestCaseSource(nameof(AssignStatementToVariableSdk330TestCases))]
        public string AssignStatementToVariableSdk330(string fileName, GeneratorJobType job, bool isUseTabs)
        {
            var sdks = ASContext.Context.Settings.InstalledSDKs;
            ASContext.Context.Settings.InstalledSDKs = new[] { new InstalledSDK { Path = PluginBase.CurrentProject.CurrentSDK, Version = "3.3.0" } };
            var result = AssignStatementToVariable(fileName, job, isUseTabs);
            ASContext.Context.Settings.InstalledSDKs = sdks;
            return result;
        }
        static IEnumerable<TestCaseData> GenerateVariableWithExplicitScopeTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateVariable", GeneratorJobType.Variable)
                    .Returns(ReadAllText("AfterGeneratePrivateVariable_generateExplicitScopeIsTrue"))
                    .SetName("Generate private variable");
                yield return new TestCaseData("BeforeGenerateVariable", GeneratorJobType.VariablePublic)
                    .Returns(ReadAllText("AfterGeneratePublicVariable_generateExplicitScopeIsTrue"))
                    .SetName("Generate public variable");
                yield return new TestCaseData("BeforeGenerateStaticVariable", GeneratorJobType.VariablePublic)
                    .Returns(ReadAllText("AfterGeneratePublicStaticVariable_generateExplicitScopeIsTrue"))
                    .SetName("Generate public static variable");
                yield return new TestCaseData("BeforeGeneratePublicStaticVariable_forSomeType", GeneratorJobType.VariablePublic)
                    .Returns(ReadAllText("AfterGeneratePublicStaticVariable_forSomeType"))
                    .SetName("From SomeType.foo| variable");
                yield return new TestCaseData("BeforeGeneratePublicStaticVariable_forCurrentType", GeneratorJobType.VariablePublic)
                    .Returns(ReadAllText("AfterGeneratePublicStaticVariable_forCurrentType"))
                    .SetName("From CurrentType.foo| variable");
            }
        }

        [Test, TestCaseSource(nameof(GenerateVariableWithExplicitScopeTestCases))]
        public string GenerateVariableWithExplicitScope(string fileName, GeneratorJobType job)
        {
            ASContext.CommonSettings.GenerateScope = true;
            var result = ContextualGenerator(sci, fileName, job, true);
            ASContext.CommonSettings.GenerateScope = false;
            return result;
        }
        
        static IEnumerable<TestCaseData> GenerateVariableWithDefaultModifierDeclarationTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateVariable", GeneratorJobType.Variable)
                    .Returns(ReadAllText("AfterGeneratePrivateVariableWithDefaultModifier"))
                    .SetName("Generate private variable with default modifier declration");
                yield return new TestCaseData("BeforeGenerateStaticVariable", GeneratorJobType.Variable)
                    .Returns(ReadAllText("AfterGeneratePrivateStaticVariableWithDefaultModifier"))
                    .SetName("Generate private static variable with default modifier declration");
            }
        }

        [Test, TestCaseSource(nameof(GenerateVariableWithDefaultModifierDeclarationTestCases))]
        public string GenerateVariableWithDefaultModifierDeclaration(string fileName, GeneratorJobType job)
        {
            ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
            var result = ContextualGenerator(sci, fileName, job, true);
            ASContext.CommonSettings.GenerateDefaultModifierDeclaration = false;
            return result;
        }

        static IEnumerable<TestCaseData> GenerateEventHandlerTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateEventHandler", EmptyArray<string>.Instance)
                    .Returns(ReadAllText("AfterGenerateEventHandler_withoutAutoRemove"))
                    .SetName("Generate event handler without auto remove");
                yield return new TestCaseData("BeforeGenerateEventHandler", new[] { "Event.ADDED", "Event.REMOVED" })
                    .Returns(ReadAllText("AfterGenerateEventHandler_withAutoRemove"))
                    .SetName("Generate event handler with auto remove");
            }
        }

        [Test, TestCaseSource(nameof(GenerateEventHandlerTestCases))]
        public string GenerateEventHandler(string fileName, string[] autoRemove)
        {
            ASContext.CommonSettings.EventListenersAutoRemove = autoRemove;
            SetSrc(sci, ReadAllText(fileName));
            var re = string.Format(ASGenerator.patternEvent, ASGenerator.contextToken);
            var m = Regex.Match(sci.GetLine(sci.CurrentLine), re, RegexOptions.IgnoreCase);
            ASGenerator.contextMatch = m;
            ASGenerator.contextParam = ASGenerator.CheckEventType(m.Groups["event"].Value);
            ASGenerator.GenerateJob(GeneratorJobType.ComplexEvent, ASContext.Context.CurrentMember, ASContext.Context.CurrentClass, null, null);
            return sci.Text;
        }

        static IEnumerable<TestCaseData> GenerateEventHandlerWithExplicitScopeTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateEventHandler", new[] { "Event.ADDED", "Event.REMOVED" })
                    .Returns(ReadAllText("AfterGenerateEventHandler_withAutoRemove_generateExplicitScopeIsTrue"))
                    .SetName("Generate event handler with auto remove");
            }
        }

        [Test, TestCaseSource(nameof(GenerateEventHandlerWithExplicitScopeTestCases))]
        public string GenerateEventHandlerWithExplicitScope(string fileName, string[] autoRemove)
        {
            ASContext.CommonSettings.GenerateScope = true;
            var result = GenerateEventHandler(fileName, autoRemove);
            ASContext.CommonSettings.GenerateScope = false;
            return result;
        }

        static IEnumerable<TestCaseData> GenerateEventHandlerWithDefaultModifierDeclarationTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateEventHandler", new[] { "Event.ADDED", "Event.REMOVED" })
                    .Returns(ReadAllText("AfterGeneratePrivateEventHandlerWithDefaultModifier"))
                    .SetName("Generate private event handler with default modifier declaration");
                yield return new TestCaseData("BeforeGeneratePrivateStaticEventHandler", EmptyArray<string>.Instance)
                    .Returns(ReadAllText("AfterGeneratePrivateStaticEventHandlerWithDefaultModifier"))
                    .SetName("Generate private static event handler with default modifier declaration");
            }
        }

        [Test, TestCaseSource(nameof(GenerateEventHandlerWithDefaultModifierDeclarationTestCases))]
        public string GenerateEventHandlerWithDefaultModifierDeclaration(string fileName, string[] autoRemove)
        {
            ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
            var result = GenerateEventHandler(fileName, autoRemove);
            ASContext.CommonSettings.GenerateDefaultModifierDeclaration = false;
            return result;
        }

        static IEnumerable<TestCaseData> GenerateGetterSetterWithDefaultModifierDeclarationTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateGetterSetter", GeneratorJobType.GetterSetter)
                    .Returns(ReadAllText("AfterGeneratePrivateGetterSetterWithDefaultModifier"))
                    .SetName("Generate private getter and setter with default modifier declaration");
                yield return new TestCaseData("BeforeGeneratePrivateStaticGetterSetter", GeneratorJobType.GetterSetter)
                    .Returns(ReadAllText("AfterGeneratePrivateStaticGetterSetterWithDefaultModifier"))
                    .SetName("Generate private static getter and setter with default modifier declaration");
            }
        }

        [Test, TestCaseSource(nameof(GenerateGetterSetterWithDefaultModifierDeclarationTestCases))]
        public string GenerateGetterSetterWithDefaultModifierDeclaration(string fileName, GeneratorJobType job)
        {
            ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
            var result = ContextualGenerator(sci, fileName, job, true);
            ASContext.CommonSettings.GenerateDefaultModifierDeclaration = false;
            return result;
        }

        static IEnumerable<TestCaseData> GenerateOverrideTestCases
        {
            get
            {
                yield return
                    new TestCaseData("BeforeOverrideGetNull", "Foo", "foo", FlagType.Getter | FlagType.Setter)
                        .Returns(ReadAllText("AfterOverrideGetNull"))
                        .SetName("Override var foo(get, null)");
                yield return
                    new TestCaseData("BeforeOverrideNullSet", "Foo", "foo", FlagType.Getter | FlagType.Setter)
                        .Returns(ReadAllText("AfterOverrideNullSet"))
                        .SetName("Override var foo(null, set)");
                yield return
                    new TestCaseData("BeforeOverrideGetSet", "Foo", "foo", FlagType.Getter | FlagType.Setter)
                        .Returns(ReadAllText("AfterOverrideGetSet"))
                        .SetName("Override var foo(get, set)");
                yield return
                    new TestCaseData("BeforeOverrideGetSet_2", "Foo", "foo", FlagType.Getter | FlagType.Setter)
                        .Returns(ReadAllText("AfterOverrideGetSet_2"))
                        .SetName("Override var foo(get, set). If the getter is already overridden.");
                yield return
                    new TestCaseData("BeforeOverrideGetSet_3", "Foo", "foo", FlagType.Getter | FlagType.Setter)
                        .Returns(ReadAllText("AfterOverrideGetSet_3"))
                        .SetName("Override var foo(get, set). If the setter is already overridden.");
                yield return
                    new TestCaseData("BeforeOverrideIssue793", "Foo", "foo", FlagType.Getter | FlagType.Setter)
                        .Returns(ReadAllText("AfterOverrideIssue793"))
                        .SetName("issue #793")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/793");
                yield return
                    new TestCaseData("BeforeOverridePublicFunction", "Foo", "foo", FlagType.Function)
                        .Returns(ReadAllText("AfterOverridePublicFunction"))
                        .SetName("Override public function");
                yield return
                    new TestCaseData("BeforeOverridePrivateFunction", "Foo", "foo", FlagType.Function)
                        .Returns(ReadAllText("AfterOverridePrivateFunction"))
                        .SetName("Override private function");
                yield return
                    new TestCaseData("BeforeOverrideFunctionWithTypeParams", "Foo", "foo", FlagType.Function)
                        .Returns(ReadAllText("AfterOverrideFunctionWithTypeParams"))
                        .SetName("override function with type parameters");
                yield return
                    new TestCaseData("BeforeOverrideFunction_issue_1553_1", "Foo", "foo", FlagType.Function)
                        .Returns(ReadAllText("AfterOverrideFunction_issue_1553_1"))
                        .SetName("override function foo(c:haxe.Timer->Void)")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1553");
                yield return new TestCaseData("BeforeOverrideFunction_issue_1553_2", "Foo", "foo", FlagType.Function)
                        .Returns(ReadAllText("AfterOverrideFunction_issue_1553_2"))
                        .SetName("override function foo(c:haxe.Timer->(Type.ValueType->Void))")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1553");
                yield return new TestCaseData("BeforeOverrideFunction_issue_1553_3", "Foo", "foo", FlagType.Function)
                        .Returns(ReadAllText("AfterOverrideFunction_issue_1553_3"))
                        .SetName("override function foo(c:haxe.Timer->{v:Type.ValueType, s:String}->Void)")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1553");
                yield return new TestCaseData("BeforeOverrideFunction_issue_1553_4", "Foo", "foo", FlagType.Function)
                        .Returns(ReadAllText("AfterOverrideFunction_issue_1553_4"))
                        .SetName("override function foo(c:haxe.Timer->({v:Type.ValueType, s:String}->Void))")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1553");
                yield return new TestCaseData("BeforeOverrideFunction_issue_1553_5", "Foo", "foo", FlagType.Function)
                        .Returns(ReadAllText("AfterOverrideFunction_issue_1553_5"))
                        .SetName("override function foo(c:{v:Type.ValueType, t:haxe.Timer})")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1553");
                yield return new TestCaseData("BeforeOverrideFunction_issue_1553_6", "Foo", "foo", FlagType.Function)
                        .Returns(ReadAllText("AfterOverrideFunction_issue_1553_6"))
                        .SetName("override function foo(c:{v:Type.ValueType, t:{t:haxe.Timer}}})")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1553");
                yield return new TestCaseData("BeforeOverrideFunction_issue_1696_1", "Foo", "foo", FlagType.Function)
                        .Returns(ReadAllText("AfterOverrideFunction_issue_1696_1"))
                        .SetName("override function foo(v:Array<haxe.Timer->String>)")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1696");
                yield return new TestCaseData("BeforeOverrideFunction_issue_1696_2", "Foo", "foo", FlagType.Function)
                        .Returns(ReadAllText("AfterOverrideFunction_issue_1696_2"))
                        .SetName("override function foo(v:Array<haxe.Timer->Type.ValueType->String>)")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1696");
                yield return new TestCaseData("BeforeOverrideFunction_issue_1696_3", "Foo", "foo", FlagType.Function)
                        .Returns(ReadAllText("AfterOverrideFunction_issue_1696_3"))
                        .SetName("override function foo(v:{a:Array<haxe.Timer>}->{a:haxe.ds.Vector<Type.ValueType>}->String)")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1696");
                yield return new TestCaseData("BeforeOverrideFunction_issue_1696_4", "flash.display.DisplayObjectContainer", "addChild", FlagType.Function)
                    .Returns(ReadAllText("AfterOverrideFunction_issue_1696_4"))
                    .SetName("override function addChild(child:DisplayObject)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1696");
                yield return new TestCaseData("BeforeOverrideFunction_issue_1696_5", "Foo", "foo", FlagType.Getter | FlagType.Setter)
                    .Returns(ReadAllText("AfterOverrideFunction_issue_1696_5"))
                    .SetName("override var foo(get, set):haxe.ds.Vector<haxe.Timer->Type.ValueType>")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1696");
                yield return new TestCaseData("BeforeOverrideFunction_issue_2134_1", "flash.utils.Proxy", "callProperty", FlagType.Function)
                    .Returns(ReadAllText("AfterOverrideFunction_issue_2134_1"))
                    .SetName("override function callProperty()")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2134");
                yield return new TestCaseData("BeforeOverrideFunction_issue_2231_1", "Foo", "foo", FlagType.Getter)
                    .Returns(ReadAllText("AfterOverrideFunction_issue_2231_1"))
                    .SetName("override function foo(get):haxe.ds.Vector<haxe.Timer->Type.ValueType>")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2231");
                yield return new TestCaseData("BeforeOverrideFunction_issue_2231_2", "Foo", "foo", FlagType.Setter)
                    .Returns(ReadAllText("AfterOverrideFunction_issue_2231_2"))
                    .SetName("override function foo(set):haxe.ds.Vector<haxe.Timer->Type.ValueType>")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2231");
            }
        }

        [Test, TestCaseSource(nameof(GenerateOverrideTestCases))]
        public string GenerateOverride(string fileName, string ofClassName, string memberName, FlagType memberFlags)
        {
            SetSrc(sci, ReadAllText(fileName));
            var ofClass = ASContext.Context.CurrentModel.Classes.Find(model => model.Name == ofClassName);
            if (ofClass is null)
            {
                foreach (var classpath in ASContext.Context.Classpath)
                {
                    classpath.ForeachFile(model =>
                    {
                        foreach (var it in model.Classes)
                        {
                            if (it.QualifiedName != ofClassName) continue;
                            ofClass = it;
                            return false;
                        }
                        return true;
                    });
                    if (ofClass != null) break;
                }
            }
            var member = ofClass.Members.Search(memberName, memberFlags, 0);
            ASGenerator.GenerateOverride(sci, ofClass, member, sci.CurrentPos);
            return sci.Text;
        }

        static IEnumerable<TestCaseData> GenerateOverrideWithDefaultModifierDeclarationTestCases
        {
            get
            {
                yield return
                    new TestCaseData("BeforeOverrideGetNull", "Foo", "foo", FlagType.Getter | FlagType.Setter)
                        .Returns(ReadAllText("AfterOverrideGetNullWithDefaultModifier"))
                        .SetName("Override var foo(get, null) with default modifier declaration");
                yield return
                    new TestCaseData("BeforeOverrideNullSet", "Foo", "foo", FlagType.Getter | FlagType.Setter)
                        .Returns(ReadAllText("AfterOverrideNullSetWithDefaultModifier"))
                        .SetName("Override var foo(null, set) with default modifier declaration");
                yield return
                    new TestCaseData("BeforeOverrideGetSet", "Foo", "foo", FlagType.Getter | FlagType.Setter)
                        .Returns(ReadAllText("AfterOverrideGetSetWithDefaultModifier"))
                        .SetName("Override var foo(get, set) with default modifier declaration");
                yield return
                    new TestCaseData("BeforeOverridePrivateFunction", "Foo", "foo", FlagType.Function)
                        .Returns(ReadAllText("AfterOverridePrivateFunctionWithDefaultModifier"))
                        .SetName("Override private function with default modifier");
            }
        }

        [Test, TestCaseSource(nameof(GenerateOverrideWithDefaultModifierDeclarationTestCases))]
        public string GenerateOverrideWithDefaultModifierDeclaration(string fileName, string ofClassName, string memberName, FlagType memberFlags)
        {
            ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
            var result = GenerateOverride(fileName, ofClassName, memberName, memberFlags);
            ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
            return result;
        }

        static IEnumerable<TestCaseData> GenerateFunctionWithExplicitScopeTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateFunction", GeneratorJobType.Function)
                    .Returns(ReadAllText("AfterGeneratePrivateFunction_generateExplicitScopeIsTrue"))
                    .SetName("Generate private function");
                yield return new TestCaseData("BeforeGenerateFunction", GeneratorJobType.FunctionPublic)
                    .Returns(ReadAllText("AfterGeneratePublicFunction_generateExplicitScopeIsTrue"))
                    .SetName("Generate public function");
            }
        }

        [Test, TestCaseSource(nameof(GenerateFunctionWithExplicitScopeTestCases))]
        public string GenerateFunctionWithExplicitScope(string fileName, GeneratorJobType job)
        {
            ASContext.CommonSettings.GenerateScope = true;
            var result = ContextualGenerator(sci, fileName, job, true);
            ASContext.CommonSettings.GenerateScope = false;
            return result;
        }

        static IEnumerable<TestCaseData> GenerateFunctionWithDefaultModifierDeclarationTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateFunction", GeneratorJobType.Function)
                    .Returns(ReadAllText("AfterGeneratePrivateFunctionWithDefaultModifier"))
                    .SetName("Generate private function with default modifier declaration");
                yield return new TestCaseData("BeforeGenerateStaticFunction", GeneratorJobType.Function)
                    .Returns(ReadAllText("AfterGeneratePrivateStaticFunctionWithDefaultModifier"))
                    .SetName("Generate private static function with default modifier declaration");
            }
        }

        [Test, TestCaseSource(nameof(GenerateFunctionWithDefaultModifierDeclarationTestCases))]
        public string GenerateFunctionWithDefaultModifierDeclaration(string fileName, GeneratorJobType job)
        {
            ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
            var result = ContextualGenerator(sci, fileName, job, true);
            ASContext.CommonSettings.GenerateDefaultModifierDeclaration = false;
            return result;
        }

        static IEnumerable<TestCaseData> CanGenerateEnumTestCases
        {
            get
            {
                yield return new TestCaseData("CanGenerateEnum_1", GeneratorJob.Enum)
                    .Returns(true)
                    .SetName("Can Generate Enum. Case 1");
                yield return new TestCaseData("CanGenerateEnum_2", GeneratorJob.Enum)
                    .Returns(false)
                    .SetName("Can Generate Enum. Case 2");
                yield return new TestCaseData("CanGenerateEnum_3", GeneratorJob.Enum)
                    .Returns(false)
                    .SetName("Can Generate Enum. Case 3");
                yield return new TestCaseData("CanGenerateEnum_4", GeneratorJob.Enum)
                    .Returns(false)
                    .SetName("Can Generate Enum. Case 4");
            }
        }

        static IEnumerable<TestCaseData> CanGenerateClassTestCases_issue2891
        {
            get
            {
                yield return new TestCaseData("CanGenerateClass_issue2891_1", GeneratorJobType.Class)
                    .Returns(true)
                    .SetName("Can Generate Class. Issue 2891. Case 1");
            }
        }

        [
            Test,
            TestCaseSource(nameof(CanGenerateEnumTestCases)),
            TestCaseSource(nameof(CanGenerateClassTestCases_issue2891)),
        ]
        public bool HasGenerator(string fileName, GeneratorJobType job)
        {

            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFileName(GetFullPath(fileName));
            var options = new List<ICompletionListItem>();
            ASGenerator.ContextualGenerator(sci, options);
            return options.Any(it => ((GeneratorItem) it).Job == job);
        }
    }
}