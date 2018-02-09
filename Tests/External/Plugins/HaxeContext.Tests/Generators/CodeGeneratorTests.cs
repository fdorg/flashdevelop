using System.Collections.Generic;
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

        internal static string ReadAll(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

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
                yield return
                    new TestCaseData("BeforeContextualGeneratorTests_issue1833_1", false)
                        .Returns(ReadAll("AfterContextualGeneratorTests_issue1833_1"))
                        .SetName("Issue1833. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1833");
                yield return
                    new TestCaseData("BeforeContextualGeneratorTests_issue1833_2", false)
                        .Returns(ReadAll("AfterContextualGeneratorTests_issue1833_2"))
                        .SetName("Issue1833. Case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1833");
                yield return
                    new TestCaseData("BeforeContextualGeneratorTests_issue1743_1", false)
                        .Returns(ReadAll("AfterContextualGeneratorTests_issue1743_1"))
                        .SetName("Issue1743. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1743");
                yield return
                    new TestCaseData("BeforeContextualGeneratorTests_issue1743_2", false)
                        .Returns(ReadAll("AfterContextualGeneratorTests_issue1743_2"))
                        .SetName("Issue1743. Case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1743");
                yield return
                    new TestCaseData("BeforeContextualGeneratorTests_issue1743_3", false)
                        .Returns(ReadAll("AfterContextualGeneratorTests_issue1743_3"))
                        .SetName("Issue1743. Case 3")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1743");
                yield return
                    new TestCaseData("BeforeContextualGeneratorTests_issue1927_1", true)
                        .Returns(ReadAll("AfterContextualGeneratorTests_issue1927_1"))
                        .SetName("Issue1927. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1927");
                yield return
                    new TestCaseData("BeforeContextualGeneratorTests_issue1964_1", false)
                        .Returns(ReadAll("AfterContextualGeneratorTests_issue1964_1"))
                        .SetName("Issue1964. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1964");
            }
        }

        [Test, TestCaseSource(nameof(ContextualGeneratorTestCases))]
        public string ContextualGenerator(string fileName, bool hasGenerator) => ContextualGenerator(sci, fileName, hasGenerator);

        internal static string ContextualGenerator(ScintillaControl sci, string fileName, bool hasGenerator)
        {
            SetSrc(sci, ReadAll(fileName));
            fileName = GetFullPath(fileName);
            ASContext.Context.CurrentModel.FileName = fileName;
            PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
            var options = new List<ICompletionListItem>();
            ASGenerator.ContextualGenerator(sci, options);
            if (!hasGenerator) Assert.AreEqual(0, options.Count);
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
            SetSrc(sci, ReadAll(fileName));
            fileName = GetFullPath(fileName);
            ASContext.Context.CurrentModel.FileName = fileName;
            PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
            return ASGenerator.HandleGeneratorCompletion(sci, false, ASContext.Context.Features.overrideKey);
        }
    }
}
