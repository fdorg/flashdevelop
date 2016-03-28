using System.Collections.Generic;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.Settings;
using CodeRefactor.TestUtils;
using FlashDevelop;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using PluginCore.Helpers;
using ScintillaNet;
using ScintillaNet.Enums;

namespace CodeRefactor.Commands
{
    [TestFixture]
    class CodeRefactorTests
    {
        private MainForm mainForm;
        private ISettings settings;
        private ITabbedDocument doc;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            mainForm = new MainForm();
            settings = Substitute.For<ISettings>();
            settings.UseTabs = true;
            settings.IndentSize = 4;
            settings.SmartIndentType = SmartIndent.CPP;
            settings.TabIndents = true;
            settings.TabWidth = 4;
            doc = Substitute.For<ITabbedDocument>();
            mainForm.Settings = settings;
            mainForm.CurrentDocument = doc;
            mainForm.Documents = new[] {doc};
            mainForm.StandaloneMode = false;
            PluginBase.Initialize(mainForm);
            FlashDevelop.Managers.ScintillaManager.LoadConfiguration();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            settings = null;
            doc = null;
            mainForm.Dispose();
            mainForm = null;
        }

        private ScintillaControl GetBaseScintillaControl()
        {
            return new ScintillaControl
            {
                Encoding = System.Text.Encoding.UTF8,
                CodePage = 65001,
                Indent = settings.IndentSize,
                Lexer = 3,
                StyleBits = 7,
                IsTabIndents = settings.TabIndents,
                IsUseTabs = settings.UseTabs,
                TabWidth = settings.TabWidth
            };
        }

        [TestFixture]
        public class RefactorCommand : CodeRefactorTests
        {
            protected ScintillaControl Sci;

            [TestFixtureSetUp]
            public void Setup()
            {
                var pluginMain = Substitute.For<ASCompletion.PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = Substitute.For<IASContext>();
                Sci = GetBaseScintillaControl();
                doc.SciControl.Returns(Sci);
            }

            [TestFixture]
            public class ExtractLocalVariable : RefactorCommand
            {
                public IEnumerable<TestCaseData> GetAS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.as3.BeforeExtractLocalVariable.as"),
                                    new MemberModel("ExtractLocalVariable", null, FlagType.Constructor | FlagType.Function, 0)
                                    {
                                        LineFrom = 4,
                                        LineTo = 7
                                    },
                                    "newVar"
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.as3.AfterExtractLocalVariable.as"))
                                .SetName("ExtractLocaleVariable");

                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.as3.BeforeExtractLocalVariable_fromString.as"),
                                    new MemberModel("ExtractLocalVariable", null, FlagType.Constructor | FlagType.Function, 0)
                                    {
                                        LineFrom = 4,
                                        LineTo = 7
                                    },
                                    "newVar"
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.as3.AfterExtractLocalVariable_fromString.as"))
                                .SetName("ExtractLocaleVariable from String");

                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.as3.BeforeExtractLocalVariable_fromNumber.as"),
                                    new MemberModel("ExtractLocalVariable", null, FlagType.Constructor | FlagType.Function, 0)
                                    {
                                        LineFrom = 4,
                                        LineTo = 7
                                    },
                                    "newVar"
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.as3.AfterExtractLocalVariable_fromNumber.as"))
                                .SetName("ExtractLocaleVariable from Number");

                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.as3.BeforeExtractLocalVariable_forCheckingThePositionOfNewVar.as"),
                                    new MemberModel("extractLocalVariable", null, FlagType.Function, Visibility.Public)
                                    {
                                        LineFrom = 4,
                                        LineTo = 10
                                    },
                                    "newVar"
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.as3.AfterExtractLocalVariable_forCheckingThePositionOfNewVar.as"))
                                .SetName("ExtractLocaleVariable with checking the position of a new variable");
                    }
                }

                [Test, TestCaseSource("GetAS3TestCases")]
                public string AS3(string sourceText, MemberModel currentMember, string newName)
                {
                    ASContext.Context.SetAs3Features();
                    ASContext.Context.CurrentModel.Returns(new FileModel {Context = ASContext.Context});
                    ASContext.Context.CurrentMember.Returns(currentMember);
                    Sci.Text = sourceText;
                    Sci.ConfigurationLanguage = "as3";
                    SnippetHelper.PostProcessSnippets(Sci, 0);
                    new ExtractLocalVariableCommand(false, newName).Execute();
                    return Sci.Text;
                }
            }
        }
    }
}