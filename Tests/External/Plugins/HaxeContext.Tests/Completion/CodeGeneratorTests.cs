using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.TestUtils;
using HaXeContext.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using PluginCore.Helpers;
using ScintillaNet;

namespace HaXeContext.Completion
{
    [TestFixture]
    public class ContextualGeneratorTests : ASGeneratorTests.GenerateJob
    {
        internal new static string ReadAllTextHaxe(string fileName) => TestFile.ReadAllText($"{nameof(HaXeContext)}.Test_Files.completion.generated.{fileName}.hx");

        [TestFixtureSetUp]
        public void AddInterfaceDefTestsSetup()
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
                    new TestCaseData("BeforeContextualGeneratorTests_issue1833_1")
                        .Returns(ReadAllTextHaxe("AfterContextualGeneratorTests_issue1833_1"))
                        .SetName("Issue1833. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1833");
                yield return
                    new TestCaseData("BeforeContextualGeneratorTests_issue1833_2")
                        .Returns(ReadAllTextHaxe("AfterContextualGeneratorTests_issue1833_2"))
                        .SetName("Issue1833. Case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1833");
            }
        }

        [Test, TestCaseSource(nameof(HaxeTestCases))]
        public string Haxe(string fileName) => HaxeImpl(fileName, sci);

        internal static string HaxeImpl(string fileName, ScintillaControl sci)
        {
            var sourceText = ReadAllTextHaxe(fileName);
            fileName = GetFullPathHaxe(fileName);
            ASContext.Context.CurrentModel.FileName = fileName;
            PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
            return Common(sourceText, sci);
        }

        internal static string Common(string sourceText, ScintillaControl sci)
        {
            sci.Text = sourceText;
            SnippetHelper.PostProcessSnippets(sci, 0);
            var currentModel = ASContext.Context.CurrentModel;
            new ASFileParser().ParseSrc(currentModel, sci.Text);
            var line = sci.CurrentLine;
            var currentClass = currentModel.Classes.Find(it => it.LineFrom <= line && it.LineTo >= line);
            ASContext.Context.CurrentClass.Returns(currentClass);
            var currentMember = currentClass.Members.Items.Find(it => it.LineFrom <= line && it.LineTo >= line);
            ASContext.Context.CurrentMember.Returns(currentMember);
            ASGenerator.contextToken = sci.GetWordFromPosition(sci.CurrentPos);
            ASGenerator.ContextualGenerator(sci, new List<ICompletionListItem>());
            return sci.Text;
        }
    }
}
