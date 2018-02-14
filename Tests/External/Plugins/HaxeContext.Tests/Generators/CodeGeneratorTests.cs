using System.Collections.Generic;
using System.Linq;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.TestUtils;
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

        static void SetCurrentFile(string fileName)
        {
            fileName = GetFullPath(fileName);
            ASContext.Context.CurrentModel.FileName = fileName;
            PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
        }

        [TestFixtureSetUp]
        public void Setup()
        {
            ASContext.Context.Settings.GenerateImports.Returns(true);
            ASContext.Context.SetHaxeFeatures();
            sci.ConfigurationLanguage = "haxe";
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
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2022")
                    .Ignore();
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2022_1", GeneratorJobType.FunctionPublic, false)
                    .Returns(null)
                    .SetName("`Generate public function` shouldn't work for optional parameter. private function.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2022")
                    .Ignore("");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2022_2", GeneratorJobType.Function, false)
                    .Returns(null)
                    .SetName("`Generate private function` shouldn't work for optional parameter. local function.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2022");
                yield return new TestCaseData("BeforeContextualGeneratorTests_issue2022_2", GeneratorJobType.FunctionPublic, false)
                    .Returns(null)
                    .SetName("`Generate public function` shouldn't work for optional parameter. local function.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2022");
            }
        }

        [
            Test,
            TestCaseSource(nameof(ContextualGeneratorTestCases)),
            TestCaseSource(nameof(Issue2017TestCases)),
            TestCaseSource(nameof(ContextualGeneratorForOptionParametersTestCases))
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
            }
            else if (job == (GeneratorJobType) (-1))
            {
                Assert.IsEmpty(options);
                return null;
            }
            else if (options.Count > 0)
            {
                Assert.IsFalse(options.Any(it => ((ASCompletion.Completion.GeneratorItem) it).job == job));
                return null;
            }
            return sci.Text;
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
