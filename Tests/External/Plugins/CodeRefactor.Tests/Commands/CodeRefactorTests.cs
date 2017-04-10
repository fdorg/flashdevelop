// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System.Collections.Generic;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.Settings;
using CodeRefactor.Provider;
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
        MainForm mainForm;
        ISettings settings;
        ITabbedDocument doc;

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

        ScintillaControl GetBaseScintillaControl()
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
        public class RefactorCommandTests : CodeRefactorTests
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
            public class ExtractLocalVariable : RefactorCommandTests
            {
                internal static string ReadAllTextAS3(string fileName) => TestFile.ReadAllText($"{nameof(CodeRefactor)}.Test_Files.coderefactor.extractlocalvariable.as3.{fileName}.as");

                internal static string ReadAllTextHaxe(string fileName) => TestFile.ReadAllText($"{nameof(CodeRefactor)}.Test_Files.coderefactor.extractlocalvariable.haxe.{fileName}.hx");

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeExtractLocalVariable_fromGeneric"),
                                    new MemberModel("main", null, FlagType.Static | FlagType.Function, 0)
                                    {
                                        LineFrom = 2,
                                        LineTo = 5
                                    },
                                    "newVar"
                                )
                                .Returns(ReadAllTextHaxe("AfterExtractLocalVariable_fromGeneric"))
                                .SetName("ExtractLocaleVariable from Generic");

                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeExtractLocalVariable_fromString"),
                                    new MemberModel("extractLocalVariable", null, FlagType.Function, Visibility.Public)
                                    {
                                        LineFrom = 4,
                                        LineTo = 7
                                    },
                                    "newVar"
                                )
                                .Returns(ReadAllTextHaxe("AfterExtractLocalVariable_fromString"))
                                .SetName("ExtractLocaleVariable from String");

                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeExtractLocalVariable_fromNumber"),
                                    new MemberModel("extractLocalVariable", null, FlagType.Function, Visibility.Public)
                                    {
                                        LineFrom = 4,
                                        LineTo = 7
                                    },
                                    "newVar"
                                )
                                .Returns(ReadAllTextHaxe("AfterExtractLocalVariable_fromNumber"))
                                .SetName("ExtractLocaleVariable from Number");

                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeExtractLocalVariable_inSinglelineMethod"),
                                    new MemberModel("extractLocalVariable", null, FlagType.Function, Visibility.Public)
                                    {
                                        LineFrom = 4,
                                        LineTo = 5
                                    },
                                    "newVar"
                                )
                                .Ignore("Not supported at the moment")
                                .Returns(ReadAllTextHaxe("AfterExtractLocalVariable_inSinglelineMethod"))
                                .SetName("ExtractLocaleVariable in single line method");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, MemberModel currentMember, string newName)
                {
                    ASContext.Context.SetHaxeFeatures();
                    ASContext.Context.CurrentModel.Returns(new FileModel {haXe = true, Context = ASContext.Context});
                    Sci.ConfigurationLanguage = "haxe";
                    return Common(sourceText, currentMember, newName, Sci);
                }

                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeExtractLocalVariable"),
                                    new MemberModel("ExtractLocalVariable", null, FlagType.Constructor | FlagType.Function, 0)
                                    {
                                        LineFrom = 4,
                                        LineTo = 7
                                    },
                                    "newVar"
                                )
                                .Returns(ReadAllTextAS3("AfterExtractLocalVariable"))
                                .SetName("ExtractLocaleVariable");

                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeExtractLocalVariable_fromString"),
                                    new MemberModel("ExtractLocalVariable", null, FlagType.Constructor | FlagType.Function, 0)
                                    {
                                        LineFrom = 4,
                                        LineTo = 7
                                    },
                                    "newVar"
                                )
                                .Ignore("Not supported at the moment")
                                .Returns(ReadAllTextAS3("AfterExtractLocalVariable_fromString"))
                                .SetName("ExtractLocaleVariable from String");

                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeExtractLocalVariable_fromNumber"),
                                    new MemberModel("ExtractLocalVariable", null, FlagType.Constructor | FlagType.Function, 0)
                                    {
                                        LineFrom = 4,
                                        LineTo = 7
                                    },
                                    "newVar"
                                )
                                .Ignore("Not supported at the moment")
                                .Returns(ReadAllTextAS3("AfterExtractLocalVariable_fromNumber"))
                                .SetName("ExtractLocaleVariable from Number");
                    }
                }
                
                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, MemberModel currentMember, string newName)
                {
                    ASContext.Context.SetAs3Features();
                    ASContext.Context.CurrentModel.Returns(new FileModel {Context = ASContext.Context});
                    Sci.ConfigurationLanguage = "as3";
                    return Common(sourceText, currentMember, newName, Sci);
                }

                static string Common(string sourceText, MemberModel currentMember, string newName, ScintillaControl sci)
                {
                    sci.Text = sourceText;
                    SnippetHelper.PostProcessSnippets(sci, 0);
                    ASContext.Context.CurrentMember.Returns(currentMember);
                    CommandFactoryProvider.GetFactory(sci.ConfigurationLanguage)
                        .CreateExtractLocalVariableCommand(false, newName)
                        .Execute();
                    return sci.Text;
                }
            }

            [TestFixture]
            public class ExtractLocalVariableWithContextualGenertator : RefactorCommandTests
            {
                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractLocalVariable_withContextualGenerator"),
                                    new MemberModel("extractLocalVariable", null, FlagType.Function, Visibility.Public)
                                    {
                                        LineFrom = 4,
                                        LineTo = 10
                                    },
                                    "newVar",
                                    0
                                )
                                .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractLocalVariable_ReplaceAllOccurrences"))
                                .SetName("ExtractLocaleVariable replace all occurrences");

                        yield return
                            new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractLocalVariable_withContextualGenerator"),
                                    new MemberModel("extractLocalVariable", null, FlagType.Function, Visibility.Public)
                                    {
                                        LineFrom = 4,
                                        LineTo = 10
                                    },
                                    "newVar",
                                    1
                                )
                                .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractLocalVariable_ReplaceInitialOccurrence"))
                                .SetName("ExtractLocaleVariable replace initial occurrence");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, MemberModel currentMember, string newName, int contextualGeneratorItem)
                {
                    ASContext.Context.SetHaxeFeatures();
                    ASContext.Context.CurrentModel.Returns(new FileModel { haXe = true, Context = ASContext.Context });
                    ASContext.Context.CurrentMember.Returns(currentMember);
                    Sci.Text = sourceText;
                    Sci.ConfigurationLanguage = "haxe";
                    SnippetHelper.PostProcessSnippets(Sci, 0);
                    var command = (ExtractLocalVariableCommand)CommandFactoryProvider.GetFactory("haxe").CreateExtractLocalVariableCommand(false, newName);
                    command.Execute();
                    ((CompletionListItem)command.CompletionList[contextualGeneratorItem]).PerformClick();
                    return Sci.Text;
                }
            }

            [TestFixture]
            public class OrganizeImports : RefactorCommandTests
            {
                protected static string ReadAllTextHaxe(string fileName) => TestFile.ReadAllText($"{nameof(CodeRefactor)}.Test_Files.coderefactor.organizeimports.haxe.{fileName}.hx");

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOrganizeImports"), "BeforeOrganizeImports.hx")
                                .Returns(ReadAllTextHaxe("AfterOrganizeImports"))
                                .SetName("OrganizeImports");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOrganizeImports_withImportsFromSameModule"), "Main.hx")
                                .Returns(ReadAllTextHaxe("AfterOrganizeImports_withImportsFromSameModule"))
                                .SetName("Issue782. Package is empty.");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOrganizeImports_withImportsFromSameModule2"), "Main.hx")
                                .Returns(ReadAllTextHaxe("AfterOrganizeImports_withImportsFromSameModule2"))
                                .SetName("Issue782. Package is not empty.");
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