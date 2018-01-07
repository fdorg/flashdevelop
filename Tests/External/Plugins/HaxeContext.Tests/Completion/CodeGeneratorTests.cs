using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.TestUtils;
using HaXeContext.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using ScintillaNet;

namespace HaXeContext.Completion
{
    [TestFixture]
    public class ContextualGeneratorTests : ASGeneratorTests.GenerateJob
    {
        internal new static string ReadAllTextHaxe(string fileName) => TestFile.ReadAllText($"{nameof(HaXeContext)}.Test_Files.completion.generated.{fileName}.hx");

        [TestFixtureSetUp]
        public void Setup()
        {
            ASContext.Context.Settings.GenerateImports.Returns(true);
            ASContext.Context.SetHaxeFeatures();
            sci.ConfigurationLanguage = "haxe";
        }

        public IEnumerable<TestCaseData> HaxeTestCases
        {
            get
            {
                yield return
                    new TestCaseData("BeforeContextualGeneratorTests_issue1833_1", false)
                        .Returns(ReadAllTextHaxe("AfterContextualGeneratorTests_issue1833_1"))
                        .SetName("Issue1833. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1833");
                yield return
                    new TestCaseData("BeforeContextualGeneratorTests_issue1833_2", false)
                        .Returns(ReadAllTextHaxe("AfterContextualGeneratorTests_issue1833_2"))
                        .SetName("Issue1833. Case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1833");
                yield return
                    new TestCaseData("BeforeContextualGeneratorTests_issue1743_1", false)
                        .Returns(ReadAllTextHaxe("AfterContextualGeneratorTests_issue1743_1"))
                        .SetName("Issue1743. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1743");
                yield return
                    new TestCaseData("BeforeContextualGeneratorTests_issue1743_2", false)
                        .Returns(ReadAllTextHaxe("AfterContextualGeneratorTests_issue1743_2"))
                        .SetName("Issue1743. Case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1743");
                yield return
                    new TestCaseData("BeforeContextualGeneratorTests_issue1743_3", false)
                        .Returns(ReadAllTextHaxe("AfterContextualGeneratorTests_issue1743_3"))
                        .SetName("Issue1743. Case 3")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1743");
            }
        }

        [Test, TestCaseSource(nameof(HaxeTestCases))]
        public string Haxe(string fileName, bool hasGenerator) => Impl(sci, fileName, hasGenerator);

        internal static string Impl(ScintillaControl sci, string fileName, bool hasGenerator)
        {
            var sourceText = ReadAllTextHaxe(fileName);
            fileName = GetFullPathHaxe(fileName);
            ASContext.Context.CurrentModel.FileName = fileName;
            PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
            return Common(sci, sourceText, hasGenerator);
        }

        internal static string Common(ScintillaControl sci, string sourceText, bool hasGenerator)
        {
            SetSrc(sci, sourceText);
            var options = new List<ICompletionListItem>();
            ASGenerator.ContextualGenerator(sci, options);
            if (!hasGenerator) Assert.AreEqual(0, options.Count);
            return sci.Text;
        }
    }
}
