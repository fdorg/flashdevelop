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
        internal static string GetFullPath(string fileName) => $"{nameof(HaXeContext)}.Test_Files.generators.code.{fileName}.hx";

        internal static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

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
                    .SetName("fo|o((this.bar().x:Int)). Generate function . Case 5");
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
                    .SetName("`Generate private function` shouldn't work for optional parameter. private function.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2022");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2022_1", GeneratorJobType.FunctionPublic, false)
                    .Returns(null)
                    .SetName("`Generate public function` shouldn't work for optional parameter. private function.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2022");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2022_2", GeneratorJobType.Function, false)
                    .Returns(null)
                    .SetName("`Generate private function` shouldn't work for optional parameter. local function.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2022");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2022_2", GeneratorJobType.FunctionPublic, false)
                    .Returns(null)
                    .SetName("`Generate public function` shouldn't work for optional parameter. local function.")
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

        static IEnumerable<TestCaseData> AssignStatementToVarIssue1999
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
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1999");
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

        [
            Test,
            TestCaseSource(nameof(ContextualGeneratorTestCases)),
            TestCaseSource(nameof(Issue2017TestCases)),
            TestCaseSource(nameof(ContextualGeneratorForOptionParametersTestCases)),
            TestCaseSource(nameof(Issue1880TestCases)),
            TestCaseSource(nameof(Issue2060TestCases)),
            TestCaseSource(nameof(Issue2069TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue1999)),
        ]
        public string ContextualGenerator(string fileName, GeneratorJobType job, bool hasGenerator) => ContextualGenerator(sci, fileName, job, hasGenerator);

        internal static string ContextualGenerator(ScintillaControl sci, string fileName, GeneratorJobType job, bool hasGenerator)
        {
            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFile(fileName);
            sci.Colourise(0, -1);
            var options = new List<ICompletionListItem>();
            ASGenerator.ContextualGenerator(sci, options);
            if (hasGenerator)
            {
                Assert.IsNotEmpty(options);
                var item = options.Find(it => ((ASCompletion.Completion.GeneratorItem) it).job == job);
                Assert.IsNotNull(item);
                var value = item.Value;
                return sci.Text;
            }
            if (job == (GeneratorJobType) (-1)) Assert.IsEmpty(options);
            if (options.Count > 0) Assert.IsFalse(options.Any(it => ((ASCompletion.Completion.GeneratorItem) it).job == job));
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
