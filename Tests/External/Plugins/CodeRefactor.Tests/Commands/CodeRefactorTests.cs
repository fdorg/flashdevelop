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
            mainForm.StandaloneMode = true;
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
                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.haxe.BeforeExtractLocalVariable_fromGeneric.hx"),
                                    new MemberModel("main", null, FlagType.Static | FlagType.Function, 0)
                                    {
                                        LineFrom = 2,
                                        LineTo = 5
                                    },
                                    "newVar"
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.haxe.AfterExtractLocalVariable_fromGeneric.hx"))
                                .SetName("ExtractLocaleVariable from Generic");

                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.haxe.BeforeExtractLocalVariable_fromString.hx"),
                                    new MemberModel("extractLocalVariable", null, FlagType.Function, Visibility.Public)
                                    {
                                        LineFrom = 4,
                                        LineTo = 7
                                    },
                                    "newVar"
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.haxe.AfterExtractLocalVariable_fromString.hx"))
                                .SetName("ExtractLocaleVariable from String");

                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.haxe.BeforeExtractLocalVariable_fromNumber.hx"),
                                    new MemberModel("extractLocalVariable", null, FlagType.Function, Visibility.Public)
                                    {
                                        LineFrom = 4,
                                        LineTo = 7
                                    },
                                    "newVar"
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.haxe.AfterExtractLocalVariable_fromNumber.hx"))
                                .SetName("ExtractLocaleVariable from Number");

                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.haxe.BeforeExtractLocalVariable_inSinglelineMethod.hx"),
                                    new MemberModel("extractLocalVariable", null, FlagType.Function, Visibility.Public)
                                    {
                                        LineFrom = 4,
                                        LineTo = 5
                                    },
                                    "newVar"
                                )
                                .Ignore("Not supported at the moment")
                                .Returns(
                                    TestFile.ReadAllText(
                                        "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.haxe.AfterExtractLocalVariable_inSinglelineMethod.hx"))
                                .SetName("ExtractLocaleVariable in single line method");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, MemberModel currentMember, string newName)
                {
                    ASContext.Context.SetHaxeFeatures();
                    ASContext.Context.CurrentModel.Returns(new FileModel {haXe = true, Context = ASContext.Context});
                    ASContext.Context.CurrentMember.Returns(currentMember);
                    Sci.Text = sourceText;
                    Sci.ConfigurationLanguage = "haxe";
                    SnippetHelper.PostProcessSnippets(Sci, 0);
                    new ExtractLocalVariableCommand(false, newName).Execute();
                    return Sci.Text;
                }

                public IEnumerable<TestCaseData> HaxeTestCases_withContextualGenerator
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.haxe.BeforeExtractLocalVariable_withContextualGenerator.hx"),
                                    new MemberModel("extractLocalVariable", null, FlagType.Function, Visibility.Public)
                                    {
                                        LineFrom = 4,
                                        LineTo = 10
                                    },
                                    "newVar",
                                    0
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.haxe.AfterExtractLocalVariable_ReplaceAllOccurrences.hx"))
                                .SetName("ExtractLocaleVariable replace all occurrences");

                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.haxe.BeforeExtractLocalVariable_withContextualGenerator.hx"),
                                    new MemberModel("extractLocalVariable", null, FlagType.Function, Visibility.Public)
                                    {
                                        LineFrom = 4,
                                        LineTo = 10
                                    },
                                    "newVar",
                                    1
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.haxe.AfterExtractLocalVariable_ReplaceInitialOccurrence.hx"))
                                .SetName("ExtractLocaleVariable replace initial occurrence");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases_withContextualGenerator))]
                public string Haxe_withContextualGenerator(string sourceText, MemberModel currentMember, string newName, int contextualGeneratorItem)
                {
                    ASContext.Context.SetHaxeFeatures();
                    ASContext.Context.CurrentModel.Returns(new FileModel {haXe = true, Context = ASContext.Context});
                    ASContext.Context.CurrentMember.Returns(currentMember);
                    Sci.Text = sourceText;
                    Sci.ConfigurationLanguage = "haxe";
                    SnippetHelper.PostProcessSnippets(Sci, 0);
                    var command = new ExtractLocalVariableCommand(false, newName);
                    command.Execute();
                    ((CompletionListItem) command.CompletionList[contextualGeneratorItem]).PerformClick();
                    return Sci.Text;
                }

                public IEnumerable<TestCaseData> AS3TestCases
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
                                .Ignore("Not supported at the moment")
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
                                .Ignore("Not supported at the moment")
                                .Returns(
                                    TestFile.ReadAllText(
                                        "CodeRefactor.Test_Files.coderefactor.extractlocalvariable.as3.AfterExtractLocalVariable_fromNumber.as"))
                                .SetName("ExtractLocaleVariable from Number");
                    }
                }
                
                [Test, TestCaseSource(nameof(AS3TestCases))]
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

            [TestFixture]
            public class OrganizeImports : RefactorCommand
            {
                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "CodeRefactor.Test_Files.coderefactor.organizeimports.haxe.BeforeOrganizeImports.hx"),
                                    "BeforeOrganizeImports.hx"
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "CodeRefactor.Test_Files.coderefactor.organizeimports.haxe.AfterOrganizeImports.hx"))
                                .SetName("OrganizeImports");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "CodeRefactor.Test_Files.coderefactor.organizeimports.haxe.BeforeOrganizeImports_withImportsFromSameModule.hx"),
                                    "BeforeOrganizeImports_withImportsFromSameModule.hx"
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "CodeRefactor.Test_Files.coderefactor.organizeimports.haxe.AfterOrganizeImports_withImportsFromSameModule.hx"))
                                .SetName("Issue782");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, string fileName)
                {
                    Sci.ConfigurationLanguage = "haxe";
                    ASContext.Context.SetHaxeFeatures();
                    ASContext.Context.CurrentModel.Returns(new FileModel
                    {
                        haXe = true,
                        Context = ASContext.Context,
                        FileName = fileName
                    });
                    Sci.Text = sourceText;
                    SnippetHelper.PostProcessSnippets(Sci, 0);
                    var currentModel = ASContext.Context.CurrentModel;
                    new ASFileParser().ParseSrc(currentModel, Sci.Text);
                    new Commands.OrganizeImports().Execute();
                    return Sci.Text;
                }
            }
        }
    }
}