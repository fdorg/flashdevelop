using System.Collections.Generic;
using System.Linq;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.TestUtils;
using CodeRefactor.Provider;
using CodeRefactor.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PluginCore.Helpers;
using ScintillaNet;

namespace CodeRefactor.Commands
{
    [TestFixture]
    public class RefactorCommandTests : CodeRefactorTests
    {
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
                Sci.ConfigurationLanguage = "as3";
                return Common(sourceText, currentMember, newName, Sci);
            }

            static string Common(string sourceText, MemberModel currentMember, string newName, ScintillaControl sci)
            {
                sci.Text = sourceText;
                SnippetHelper.PostProcessSnippets(sci, 0);
                ASContext.Context.CurrentMember.Returns(currentMember);
                CommandFactoryProvider.GetFactory(sci)
                    .CreateExtractLocalVariableCommand(false, newName)
                    .Execute();
                return sci.Text;
            }
        }

        [TestFixture]
        public class ExtractMethodTests : RefactorCommandTests
        {
            public IEnumerable<TestCaseData> HaxeTestCases
            {
                get
                {
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case1"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case1"))
                            .SetName("Issue 1617. Case 1.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case2"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case2"))
                            .SetName("Issue 1617. Case 2.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case3"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case3"))
                            .SetName("Issue 1617. Case 3.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case4"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case4"))
                            .SetName("Issue 1617. Case 4.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case5"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case5"))
                            .SetName("Issue 1617. Case 5.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case6"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case6"))
                            .SetName("Issue 1617. Case 6.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case7"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case7"))
                            .SetName("Issue 1617. Case 7.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case8"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case8"))
                            .SetName("Issue 1617. Case 8.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case9"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case9"))
                            .SetName("Issue 1617. Case 9.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case10"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case10"))
                            .SetName("Issue 1617. Case 10.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case11"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case11"))
                            .SetName("Issue 1617. Case 11.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                }
            }

            [Test, TestCaseSource(nameof(HaxeTestCases))]
            public string Haxe(string sourceText, string newName)
            {
                ASContext.Context.SetHaxeFeatures();
                Sci.ConfigurationLanguage = "haxe";
                return Common(sourceText, newName, Sci);
            }

            static string Common(string sourceText, string newName, ScintillaControl sci)
            {
                sci.Text = sourceText;
                SnippetHelper.PostProcessSnippets(sci, 0);
                var model = ASContext.Context.GetCodeModel(sourceText);
                ASContext.Context.CurrentMember.Returns(model.Classes.First().Members.Items.First());
                CommandFactoryProvider.GetFactory(sci)
                    .CreateExtractMethodCommand(newName)
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
                var command = (ExtractLocalVariableCommand)CommandFactoryProvider.GetFactory(Sci).CreateExtractLocalVariableCommand(false, newName);
                command.Execute();
                ((CompletionListItem)command.CompletionList[contextualGeneratorItem]).PerformClick();
                return Sci.Text;
            }
        }

        [TestFixture]
        public class OrganizeImports : RefactorCommandTests
        {
            protected static string ReadAllTextHaxe(string fileName) => TestFile.ReadAllText($"{nameof(CodeRefactor)}.Test_Files.coderefactor.organizeimports.haxe.{fileName}.hx");

            [TestFixtureSetUp]
            public void OrganizeImportsFixtureSetUp()
            {
                // Needed for preprocessor directives...
                Sci.SetProperty("fold", "1");
                Sci.SetProperty("fold.preprocessor", "1");
            }

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
                    yield return
                        new TestCaseData(ReadAllTextHaxe("BeforeOrganizeImports_withImportsFromSameModule2"), "Main.hx")
                            .Returns(ReadAllTextHaxe("AfterOrganizeImports_withImportsFromSameModule2"))
                            .SetName("Issue782. Package is not empty.");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("BeforeOrganizeImports_withElseIfDirective"), "Main.hx")
                            .Returns(ReadAllTextHaxe("AfterOrganizeImports_withElseIfDirective"))
                            .SetName("Issue783. Shouldn't touch #elseif blocks.");
                }
            }

            [Test, TestCaseSource(nameof(HaxeTestCases))]
            public string Haxe(string sourceText, string fileName) => HaxeImpl(Sci, sourceText, fileName);

            public static string HaxeImpl(ScintillaControl sci, string sourceText, string fileName)
            {
                sci.ConfigurationLanguage = "haxe";
                ASContext.Context.SetHaxeFeatures();
                ASContext.Context.CurrentModel.Returns(new FileModel
                {
                    haXe = true,
                    Context = ASContext.Context,
                    FileName = fileName
                });
                return Common(sci, sourceText, fileName);
            }

            internal static string Common(ScintillaControl sci, string sourceText, string fileName)
            {
                sci.Text = sourceText;
                sci.Colourise(0, -1); // Needed for preprocessor directives...
                SnippetHelper.PostProcessSnippets(sci, 0);
                var currentModel = ASContext.Context.CurrentModel;
                new ASFileParser().ParseSrc(currentModel, sci.Text);
                CommandFactoryProvider.GetFactory(sci)
                    .CreateOrganizeImportsCommand()
                    .Execute();
                return sci.Text;
            }
        }
    }
}