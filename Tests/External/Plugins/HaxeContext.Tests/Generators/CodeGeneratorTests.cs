using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASCompletion.Completion;
using ASCompletion.Context;
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

        static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

        static readonly string testFilesAssemblyPath = $"\\FlashDevelop\\Bin\\Debug\\{nameof(HaXeContext)}\\Test_Files\\";
        static readonly string testFilesDirectory = $"\\Tests\\External\\Plugins\\{nameof(HaXeContext)}.Tests\\Test Files\\";

        static void SetCurrentFile(string fileName)
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
                    .SetName("fo|o((v is String)). Generate function. Case 1");
                yield return new TestCaseData("BeforeContextualGeneratorTests_GenerateFunction_2", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_GenerateFunction_2"))
                    .SetName("Generate function with many arguments. Case 2");
                yield return new TestCaseData("BeforeContextualGeneratorTests_GenerateFunction_3", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterContextualGeneratorTests_GenerateFunction_3"))
                    .SetName("fo|o(1 < 2 && 1 > 0 ? 1 : 0). Generate function. Case 3");
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
                yield return new TestCaseData("BeforeGenerateConstructor_issue1738_2", GeneratorJobType.Constructor, true)
                    .Returns(ReadAllText("AfterGenerateConstructor_issue1738_2"))
                    .SetName("Generate constructor with parameters")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1738");
                yield return new TestCaseData("BeforeGenerateConstructor_issue1738_3", GeneratorJobType.ChangeConstructorDecl, true)
                    .Returns(ReadAllText("AfterGenerateConstructor_issue1738_3"))
                    .SetName("Change constructor declaration")
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
                yield return new TestCaseData("BeforeGenerateConstructor_issue1738_8", GeneratorJobType.ChangeConstructorDecl, false)
                    .Returns(null)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1738");
                yield return new TestCaseData("BeforeGenerateConstructor_issue1738_9", GeneratorJobType.ChangeConstructorDecl, true)
                    .Returns(ReadAllText("AfterGenerateConstructor_issue1738_9"))
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1738");
                yield return new TestCaseData("BeforeImplementInterface_issue1982_1", GeneratorJobType.ImplementInterface, true)
                    .Returns(ReadAllText("AfterImplementInterface_issue1982_1"))
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
                    .SetName("foo(?ar|gs). Generate field.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2022");
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
                    .SetName("Infer var type. Case 18.")
                    .Ignore("Result should be `var v1:Class<Dynamic> = v;` instead of `var v1:Dynamic = v;`");
                yield return new TestCaseData("BeforeAssignStatementToVar_inferVar_19", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_inferVar_19"))
                    .SetName("Infer var type. Case 19.");
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

        static IEnumerable<TestCaseData> AssignStatementToVarTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_unsafe_cast_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_unsafe_cast_1"))
                    .SetName("cast v|");
                yield return new TestCaseData("BeforeAssignStatementToVar_unsafe_cast_2", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_unsafe_cast_2"))
                    .SetName("cast v.length|");
                yield return new TestCaseData("BeforeAssignStatementToVar_unsafe_cast_3", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_unsafe_cast_3"))
                    .SetName("cast []|");
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
                yield return
                    new TestCaseData("BeforeImplementInterface_issue1696_1", GeneratorJobType.ImplementInterface, true)
                        .Returns(ReadAllText("AfterImplementInterface_issue1696_1"))
                        .SetName("Implement interface methods. Issue 1696. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1696");
                yield return
                    new TestCaseData("BeforeImplementInterface_issue1696_2", GeneratorJobType.ImplementInterface, true)
                        .Returns(ReadAllText("AfterImplementInterface_issue1696_2"))
                        .SetName("Implement interface properties. Issue 1696. Case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1696");
                yield return
                    new TestCaseData("BeforeImplementInterface_issue1696_3", GeneratorJobType.ImplementInterface, true)
                        .Returns(ReadAllText("AfterImplementInterface_issue1696_3"))
                        .SetName("Implement interface properties. Issue 1696. Case 3")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1696");
            }
        }

        [
            Test,
            TestCaseSource(nameof(ContextualGeneratorTestCases)),
            TestCaseSource(nameof(Issue2017TestCases)),
            TestCaseSource(nameof(ContextualGeneratorForOptionParametersTestCases)),
            TestCaseSource(nameof(Issue1880TestCases)),
            TestCaseSource(nameof(Issue2060TestCases)),
            TestCaseSource(nameof(Issue2069TestCases)),
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
            TestCaseSource(nameof(AssignStatementToVarInferParameterVarTestCases)),
            TestCaseSource(nameof(AssignStatementToVarInferVarTestCases)),
            TestCaseSource(nameof(AssignStatementToVarTestCases)),
            TestCaseSource(nameof(AddToInterfaceTestCases)),
            TestCaseSource(nameof(GenerateFunctionTestCases)),
            TestCaseSource(nameof(GenerateFunctionIssue2200TestCases)),
            TestCaseSource(nameof(GenerateVariableIssue2201TestCases)),
            TestCaseSource(nameof(ImplementInterfaceTestCases)),
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
            var options = new List<ICompletionListItem>();
            ASGenerator.ContextualGenerator(sci, options);
            if (hasGenerator)
            {
                Assert.IsNotEmpty(options);
                var item = options.Find(it => ((GeneratorItem) it).job == job);
                Assert.IsNotNull(item);
                var value = item.Value;
                return sci.Text;
            }
            if (job == (GeneratorJobType) (-1)) Assert.IsEmpty(options);
            if (options.Count > 0) Assert.IsFalse(options.Any(it => ((GeneratorItem) it).job == job));
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
            }
        }

        [Test, TestCaseSource(nameof(HandleOverrideTestCases))]
        public bool HandleOverride(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFile(fileName);
            return ASGenerator.HandleGeneratorCompletion(sci, false, ASContext.Context.Features.overrideKey);
        }
    }
}
