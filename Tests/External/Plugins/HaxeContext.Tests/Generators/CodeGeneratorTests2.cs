using System.Collections.Generic;
using System.Linq;
using ASCompletion.Completion;
using ASCompletion.Context;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using ScintillaNet;

namespace HaXeContext.Generators
{
    using GeneratorJobType = HaXeContext.Generators.GeneratorJob;

    [TestFixture]
    public class CodeGeneratorTests2 : ASGeneratorTests.GenerateJob
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            ASContext.CommonSettings.DeclarationModifierOrder = new[] {"public", "protected", "internal", "private", "static", "inline", "override"};
            ASContext.Context.Settings.GenerateImports.Returns(true);
            SetHaxeFeatures(sci);
        }

        static IEnumerable<TestCaseData> GenerateSwitchLabelsIssue1759TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateSwitchLabels_issue1759_1", GeneratorJobType.Switch, true)
                    .Returns(CodeGeneratorTests.ReadAllText("AfterGenerateSwitchLabels_issue1759_1"))
                    .SetName("Generate switch labels. case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1759");
                yield return new TestCaseData("BeforeGenerateSwitchLabels_issue1759_2", GeneratorJobType.Switch, false)
                    .Returns(null)
                    .SetName("Generate switch labels. case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1759");
                yield return new TestCaseData("BeforeGenerateSwitchLabels_issue1759_3", GeneratorJobType.Switch, true)
                    .Returns(CodeGeneratorTests.ReadAllText("AfterGenerateSwitchLabels_issue1759_3"))
                    .SetName("Generate switch labels. case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1759");
                yield return new TestCaseData("BeforeGenerateSwitchLabels_issue1759_4", GeneratorJobType.Switch, false)
                    .Returns(null)
                    .SetName("Generate switch labels. case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1759");
                yield return new TestCaseData("BeforeGenerateSwitchLabels_issue1759_5", GeneratorJobType.Switch, true)
                    .Returns(CodeGeneratorTests.ReadAllText("AfterGenerateSwitchLabels_issue1759_5"))
                    .SetName("Generate switch labels. case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1759");
                yield return new TestCaseData("BeforeGenerateSwitchLabels_issue1759_6", GeneratorJobType.Switch, true)
                    .Returns(CodeGeneratorTests.ReadAllText("AfterGenerateSwitchLabels_issue1759_6"))
                    .SetName("Generate switch labels. case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1759");
                yield return new TestCaseData("BeforeGenerateSwitchLabels_issue1759_7", GeneratorJobType.Switch, true)
                    .Returns(CodeGeneratorTests.ReadAllText("AfterGenerateSwitchLabels_issue1759_7"))
                    .SetName("Generate switch labels. case 7")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1759");
                yield return new TestCaseData("BeforeGenerateSwitchLabels_issue1759_8", GeneratorJobType.Switch, true)
                    .Returns(CodeGeneratorTests.ReadAllText("AfterGenerateSwitchLabels_issue1759_8"))
                    .SetName("Generate switch labels. case 8")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1759");
                yield return new TestCaseData("BeforeGenerateSwitchLabels_issue1759_9", GeneratorJobType.Switch, true)
                    .Returns(CodeGeneratorTests.ReadAllText("AfterGenerateSwitchLabels_issue1759_9"))
                    .SetName("Generate switch labels. case 9")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1759");
            }
        }

        [
            Test,
            TestCaseSource(nameof(GenerateSwitchLabelsIssue1759TestCases)),
        ]
        public string ContextualGenerator(string fileName, GeneratorJobType job, bool hasGenerator) => ContextualGenerator(sci, fileName, job, hasGenerator);

        static string ContextualGenerator(ScintillaControl sci, string fileName, GeneratorJobType job, bool hasGenerator)
        {
            SetSrc(sci, CodeGeneratorTests.ReadAllText(fileName));
            CodeGeneratorTests.SetCurrentFile(fileName);
            var context = (Context) ASContext.GetLanguageContext("haxe");
            context.CurrentModel = ASContext.Context.CurrentModel;
            context.completionCache.IsDirty = true;
            context.GetTopLevelElements();
            var options = new List<ICompletionListItem>();
            ASGenerator.ContextualGenerator(sci, options);
            if (hasGenerator)
            {
                Assert.IsNotEmpty(options);
                var item = options.Find(it => ((GeneratorItem) it).Job == job);
                Assert.IsNotNull(item);
                var value = item.Value;
                return sci.Text;
            }
            if (job == (GeneratorJobType) (-1)) Assert.IsEmpty(options);
            if (options.Count > 0) Assert.IsFalse(options.Any(it => it is GeneratorItem item && item.Job == job));
            return null;
        }
    }
}
