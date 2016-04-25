// TODO: Tests with different formatting options using parameterized tests
using System.Collections;
using System.Collections.Generic;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.Settings;
using ASCompletion.TestUtils;
using FlashDevelop;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using ScintillaNet;
using ScintillaNet.Enums;
using System.Text.RegularExpressions;
using PluginCore.Helpers;

namespace ASCompletion.Completion
{
    [TestFixture]
    public class ASGeneratorTests
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

        public class GetBodyStart : ASGeneratorTests
        {
            public IEnumerable<TestCaseData> GetBodyStartTestCases
            {
                get
                {
                    yield return new TestCaseData("function test():void{\r\n\t\t\t\r\n}", 0, 1, "function test():void{\r\n\t\t\t\r\n}", 26).SetName("SimpleCase");
                    // Should we reindent the second line?
                    yield return new TestCaseData("function test():void{\r\n\t\t\t}", 0, 1, "function test():void{\r\n\t\t\t\r\n}", 26).SetName("EndOnSecondLine");
                    yield return new TestCaseData("function test():void{\r\n}", 0, 1, "function test():void{\r\n\t\r\n}", 24).SetName("EndOnSecondLineNoExtraIndent");
                    yield return new TestCaseData("function test():void{\r\n\t\t\t//comment}", 0, 1, "function test():void{\r\n\t\t\t//comment}", 26).SetName("CharOnSecondLine");
                    yield return new TestCaseData("function test():void{}", 0, 0, "function test():void{\r\n\t\r\n}", 24).SetName("EndOnSameDeclarationLine");
                    yield return new TestCaseData("function test():void\r\n\r\n{}\r\n", 0, 2, "function test():void\r\n\r\n{\r\n\t\r\n}\r\n", 28).SetName("EndOnSameLine");
                    yield return new TestCaseData("function test():void {trace(1);}", 0, 0, "function test():void {\r\n\ttrace(1);}", 25).SetName("TextOnStartLine");
                    yield return new TestCaseData("function test(arg:String='{', arg2:String=\"{\"):void/*{*/{\r\n}", 0, 1, "function test(arg:String='{', arg2:String=\"{\"):void/*{*/{\r\n\t\r\n}", 60)
                        .SetName("BracketInCommentsOrText");
                    yield return new TestCaseData("function test():void/*áéíóú*/\r\n{}", 0, 1, "function test():void/*áéíóú*/\r\n{\r\n\t\r\n}", 40).SetName("MultiByteCharacters");
                    yield return new TestCaseData("function tricky():void {} function test():void{\r\n\t\t\t}", 0, 1, "function tricky():void {} function test():void{\r\n\t\t\t}", 49)
                        .SetName("WithAnotherMemberInTheSameLine")
                        .Ignore("Having only LineFrom and LineTo for members is not enough to handle these cases. FlashDevelop in general is not too kind when it comes to several members in the same line, but we could change the method to use positions and try to get the proper position before.");
                    yield return new TestCaseData("function test<T:{}>(arg:T):void{\r\n\r\n}", 0, 1, "function test<T:{}>(arg:T):void{\r\n\r\n}", 34).SetName("BracketsInGenericConstraint");
                }
            }

            [Test, TestCaseSource("GetBodyStartTestCases")]
            public void Common(string text, int lineStart, int lineEnd, string resultText, int bodyStart)
            {
                var sci = GetBaseScintillaControl();
                sci.Text = text;
                sci.ConfigurationLanguage = "haxe";
                sci.Colourise(0, -1);
                int funcBodyStart = ASGenerator.GetBodyStart(lineStart, lineEnd, sci);

                Assert.AreEqual(bodyStart, funcBodyStart);
                Assert.AreEqual(resultText, sci.Text);
            }
        }

        [TestFixture]
        public class ContextualActions : ASGeneratorTests
        {
            [TestFixtureSetUp]
            public void ContextualActionsSetup()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = Substitute.For<IASContext>();
            }

            [TestFixture]
            class ShowEventsList : ContextualActions
            {
                ClassModel dataEventModel;
                FoundDeclaration found;

                [TestFixtureSetUp]
                public void ShowEventsListSetup()
                {
                    ASContext.Context.SetAs3Features();
                    ASContext.Context.CurrentModel.Returns(new FileModel());
                    dataEventModel = CreateDataEventModel();
                    found = new FoundDeclaration
                    {
                        inClass = new ClassModel(),
                        member = new MemberModel()
                    };
                }

                [Test]
                public void ShowEventsList_EventWithDataEvent()
                {
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(dataEventModel);
                    ASGenerator.contextParam = "Event";
                    var options = new List<ICompletionListItem>();
                    ASGenerator.ShowEventList(found, options);
                    Assert.AreEqual(2, options.Count);
                    Assert.IsTrue(Regex.IsMatch(options[0].Label, "\\bEvent\\b"));
                    Assert.IsTrue(Regex.IsMatch(options[1].Label, "\\bDataEvent\\b"));
                }

                [Test]
                public void ShowEventsList_EventWithoutDataEvent()
                {
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(ClassModel.VoidClass);
                    var options = new List<ICompletionListItem>();
                    ASGenerator.contextParam = "Event";
                    ASGenerator.ShowEventList(found, options);
                    Assert.AreEqual(1, options.Count);
                    Assert.IsTrue(Regex.IsMatch(options[0].Label, "\\bEvent\\b"));
                }

                [Test]
                public void ShowEventsList_CustomEventWithDataEvent()
                {
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(dataEventModel);
                    var options = new List<ICompletionListItem>();
                    ASGenerator.contextParam = "CustomEvent";
                    ASGenerator.ShowEventList(found, options);
                    Assert.AreEqual(2, options.Count);
                    Assert.IsTrue(Regex.IsMatch(options[0].Label, "\\bCustomEvent\\b"));
                    Assert.IsTrue(Regex.IsMatch(options[1].Label, "\\bEvent\\b"));
                }

                private ClassModel CreateDataEventModel()
                {
                    var dataEventFile = new FileModel();
                    var dataEventModel = new ClassModel
                    {
                        Name = "DataEvent",
                        InFile = dataEventFile
                    };
                    dataEventFile.Classes.Add(dataEventModel);
                    return dataEventModel;
                }
            }

        }

        [TestFixture]
        public class GenerateJob : ASGeneratorTests
        {
            protected ScintillaControl sci;

            [TestFixtureSetUp]
            public void GenerateJobSetup()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = Substitute.For<IASContext>();

                sci = GetBaseScintillaControl();
                doc.SciControl.Returns(sci);
            }

            [TestFixture]
            public class FieldFromParameter : GenerateJob
            {
                public IEnumerable<TestCaseData> FieldFromParameterCommonTestCases
                {
                    get
                    {
                        yield return new TestCaseData(Visibility.Public,
                            "package generatortest {\r\n\tpublic class FieldFromParameterTest{\r\n\t\tpublic function FieldFromParameterTest(arg:String){}\r\n\t}\r\n}",
                            new ClassModel
                            {
                                LineFrom = 1,
                                LineTo = 3,
                                Members = new MemberList
                                {
                                    new MemberModel("FieldFromParameterTest", null, FlagType.Constructor,
                                        Visibility.Public)
                                    {
                                        LineFrom = 2,
                                        LineTo = 2,
                                        Parameters = new List<MemberModel>
                                        {
                                            new MemberModel {Name = "arg", LineFrom = 2, LineTo = 2}
                                        }
                                    }
                                }
                            }, 0, 0)
                            .Returns(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.FieldFromParameterEmptyBody.as"))
                            .SetName("PublicScopeWithEmptyBody");

                        yield return new TestCaseData(Visibility.Public,
                            "package generatortest {\r\n\tpublic class FieldFromParameterTest{\r\n\t\tpublic function FieldFromParameterTest(arg:String){\r\n\t\t\tsuper(arg);}\r\n\t}\r\n}",
                            new ClassModel
                            {
                                LineFrom = 1,
                                LineTo = 4,
                                Members = new MemberList
                                {
                                    new MemberModel("FieldFromParameterTest", null, FlagType.Constructor,
                                        Visibility.Public)
                                    {
                                        LineFrom = 2,
                                        LineTo = 3,
                                        Parameters = new List<MemberModel>
                                        {
                                            new MemberModel {Name = "arg", LineFrom = 2, LineTo = 2}
                                        }
                                    }
                                }
                            }, 0, 0)
                            .Returns(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.FieldFromParameterWithSuperConstructor.as"))
                            .SetName("PublicScopeWithSuperConstructor");

                        yield return new TestCaseData(Visibility.Public,
                            TestFile.ReadAllText(
                                "ASCompletion.Test_Files.generated.as3.BeforeFieldFromParameterWithSuperConstructorMultiLine.as"),
                            new ClassModel
                            {
                                LineFrom = 1,
                                LineTo = 6,
                                Members = new MemberList
                                {
                                    new MemberModel("FieldFromParameterTest", null, FlagType.Constructor,
                                        Visibility.Public)
                                    {
                                        LineFrom = 2,
                                        LineTo = 5,
                                        Parameters = new List<MemberModel>
                                        {
                                            new MemberModel {Name = "arg", LineFrom = 2, LineTo = 2}
                                        }
                                    }
                                }
                            }, 0, 0)
                            .Returns(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.FieldFromParameterWithSuperConstructorMultiLine.as"))
                            .SetName("PublicScopeWithSuperConstructorMultiLine");

                        yield return new TestCaseData(Visibility.Public,
                            TestFile.ReadAllText(
                                "ASCompletion.Test_Files.generated.as3.BeforeFieldFromParameterWithWrongSuperConstructor.as"),
                            new ClassModel
                            {
                                LineFrom = 1,
                                LineTo = 6,
                                Members = new MemberList
                                {
                                    new MemberModel("FieldFromParameterTest", null, FlagType.Constructor,
                                        Visibility.Public)
                                    {
                                        LineFrom = 2,
                                        LineTo = 5,
                                        Parameters = new List<MemberModel>
                                        {
                                            new MemberModel {Name = "arg", LineFrom = 2, LineTo = 2}
                                        }
                                    }
                                }
                            }, 0, 0)
                            .Returns(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.FieldFromParameterWithWrongSuperConstructor.as"))
                            .SetName("PublicScopeWithWrongSuperConstructor");
                    }
                }

                [Test, TestCaseSource("FieldFromParameterCommonTestCases")]
                public string Common(Visibility scope, string sourceText, ClassModel sourceClassModel,
                    int memberPos, int parameterPos)
                {
                    ASContext.Context.SetAs3Features();

                    var table = new Hashtable();
                    table["scope"] = scope;

                    sci.Text = sourceText;
                    sci.ConfigurationLanguage = "as3";

                    var inClass = sourceClassModel;
                    var sourceMember = sourceClassModel.Members[memberPos];

                    ASGenerator.SetJobContext(null, null, sourceMember.Parameters[parameterPos], null);
                    ASGenerator.GenerateJob(GeneratorJobType.FieldFromPatameter, sourceMember, inClass, null, table);

                    return sci.Text;
                }
            }

            [TestFixture]
            public class ImplementInterface : GenerateJob
            {
                private ClassModel GetAs3ImplementInterfaceModel()
                {
                    var interfaceModel = new ClassModel { InFile = new FileModel(), Name = "ITest", Type = "ITest" };
                    interfaceModel.Members.Add(new MemberList
                    {
                        new MemberModel("getter", "String", FlagType.Getter, Visibility.Public),
                        new MemberModel("setter", "void", FlagType.Setter, Visibility.Public)
                        {
                            Parameters =
                                new List<MemberModel>
                                {
                                    new MemberModel("value", "String", FlagType.Variable, Visibility.Default)
                                }
                        },
                        new MemberModel("testMethod", "Number", FlagType.Function, Visibility.Public),
                        new MemberModel("testMethodArgs", "int", FlagType.Function, Visibility.Public)
                        {
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel("arg", "Number", FlagType.Variable, Visibility.Default),
                                new MemberModel("arg2", "Boolean", FlagType.Variable, Visibility.Default)
                            }
                        }
                    });

                    return interfaceModel;
                }

                private ClassModel GetHaxeImplementInterfaceModel()
                {
                    var interfaceModel = new ClassModel { InFile = new FileModel(), Name = "ITest", Type = "ITest" };
                    interfaceModel.Members.Add(new MemberList
                    {
                        new MemberModel("normalVariable", "Int", FlagType.Variable, Visibility.Public),
                        new MemberModel("ro", "Int", FlagType.Getter, Visibility.Public)
                        {
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel {Name = "default"},
                                new MemberModel {Name = "null"}
                            }
                        },
                        new MemberModel("wo", "Int", FlagType.Getter, Visibility.Public)
                        {
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel {Name = "null"},
                                new MemberModel {Name = "default"}
                            }
                        },
                        new MemberModel("x", "Int", FlagType.Getter, Visibility.Public)
                        {
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel {Name = "get"},
                                new MemberModel {Name = "set"}
                            }
                        },
                        new MemberModel("y", "Int", FlagType.Getter, Visibility.Public)
                        {
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel {Name = "get"},
                                new MemberModel {Name = "never"}
                            }
                        },
                        new MemberModel("testMethod", "Float", FlagType.Function, Visibility.Public),
                        new MemberModel("testMethodArgs", "Int", FlagType.Function, Visibility.Public)
                        {
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel("arg", "Float", FlagType.Variable, Visibility.Default),
                                new MemberModel("arg2", "Bool", FlagType.Variable, Visibility.Default)
                            }
                        },
                        new MemberModel("testPrivateMethod", "Float", FlagType.Function, Visibility.Private)
                        {
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel("?arg", "String", FlagType.Variable, Visibility.Default),
                                new MemberModel("?arg2", "Int", FlagType.Variable, Visibility.Default)
                                {
                                    Value = "1"
                                }
                            }
                        }
                    });

                    return interfaceModel;
                }

                public IEnumerable<TestCaseData> ImplementInterfaceAs3TestCases
                {
                    get
                    {
                        yield return new TestCaseData("package generatortest {\r\n\tpublic class ImplementTest{}\r\n}",
                            new ClassModel { InFile = new FileModel(), LineFrom = 1, LineTo = 1 }, GetAs3ImplementInterfaceModel())
                            .Returns(TestFile.ReadAllText("ASCompletion.Test_Files.generated.as3.ImplementInterfaceNoMembers.as"))
                            .SetName("Full");

                        yield return new TestCaseData(TestFile.ReadAllText("ASCompletion.Test_Files.generated.as3.BeforeImplementInterfacePublicMemberBehindPrivate.as"),
                            new ClassModel
                            {
                                InFile = new FileModel(),
                                LineFrom = 1,
                                LineTo = 10,
                                Members = new MemberList
                                {
                                    new MemberModel("publicMember", "void", FlagType.Function, Visibility.Public)
                                    {LineFrom = 3, LineTo = 5},
                                    new MemberModel("privateMember", "String", FlagType.Function, Visibility.Private)
                                    {LineFrom = 7, LineTo = 9}
                                }
                            },
                            GetAs3ImplementInterfaceModel())
                            .Returns(TestFile.ReadAllText("ASCompletion.Test_Files.generated.as3.ImplementInterfacePublicMemberBehindPrivate.as"))
                            .SetName("FullWithPublicMemberBehindPrivate");

                        yield return new TestCaseData(TestFile.ReadAllText("ASCompletion.Test_Files.generated.as3.BeforeImplementInterfaceNoPublicMember.as"),
                            new ClassModel
                            {
                                InFile = new FileModel(),
                                LineFrom = 1,
                                LineTo = 10,
                                Members = new MemberList
                                {
                                    new MemberModel("privateMember", "String", FlagType.Function, Visibility.Private)
                                    {
                                        LineFrom = 3,
                                        LineTo = 5
                                    }
                                }
                            },
                            GetAs3ImplementInterfaceModel())
                            .Returns(TestFile.ReadAllText("ASCompletion.Test_Files.generated.as3.ImplementInterfaceNoPublicMember.as"))
                            .SetName("FullWithoutPublicMember");
                    }
                }

                public IEnumerable<TestCaseData> ImplementInterfaceHaxeTestCases
                {
                    get
                    {
                        yield return new TestCaseData("package generatortest;\r\n\r\nclass ImplementTest{}",
                            new ClassModel { InFile = new FileModel(), LineFrom = 2, LineTo = 2 }, GetHaxeImplementInterfaceModel())
                            .Returns(TestFile.ReadAllText("ASCompletion.Test_Files.generated.haxe.ImplementInterfaceNoMembers.hx"))
                            .SetName("Full");

                        yield return new TestCaseData("package generatortest;\r\n\r\nclass ImplementTest{}",
                            new ClassModel { InFile = new FileModel(), LineFrom = 2, LineTo = 2 },
                            new ClassModel
                            {
                                InFile = new FileModel(), Name = "ITest", Type = "ITest",
                                Members = new MemberList
                                {
                                    new MemberModel("x", "Int", FlagType.Getter, Visibility.Public)
                                    {
                                        Parameters = new List<MemberModel>
                                        {
                                            new MemberModel {Name = "get"},
                                            new MemberModel {Name = "set"}
                                        }
                                    }
                                }
                            })
                            .Returns(TestFile.ReadAllText("ASCompletion.Test_Files.generated.haxe.ImplementInterfaceNoMembersInsertSingleProperty.hx"))
                            .SetName("SingleProperty");
                    }
                }

                [Test, TestCaseSource("ImplementInterfaceAs3TestCases")]
                public string As3(string sourceText, ClassModel sourceModel, ClassModel interfaceToImplement)
                {
                    ASContext.Context.SetAs3Features();
                    ASContext.Context.CurrentModel.Returns(new FileModel());
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(interfaceToImplement);

                    sci.Text = sourceText;
                    sci.ConfigurationLanguage = "as3";

                    ASGenerator.GenerateJob(GeneratorJobType.ImplementInterface, null, sourceModel, null, null);

                    return sci.Text;
                }

                [Test, TestCaseSource("ImplementInterfaceHaxeTestCases")]
                public string Haxe(string sourceText, ClassModel sourceModel, ClassModel interfaceToImplement)
                {
                    ASContext.Context.SetHaxeFeatures();
                    ASContext.Context.CurrentModel.Returns(new FileModel { haXe = true });
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(interfaceToImplement);

                    sci.Text = sourceText;
                    sci.ConfigurationLanguage = "haxe";

                    ASGenerator.GenerateJob(GeneratorJobType.ImplementInterface, null, sourceModel, null, null);

                    return sci.Text;
                }
            }

            [TestFixture]
            public class GenerateExtractVariable : GenerateJob
            {
                public IEnumerable<TestCaseData> GenerateExtractVariableHaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGenerateExtractVariableGeneric.hx"),
                                    new MemberModel("main", null, FlagType.Static | FlagType.Function, 0)
                                    {
                                        LineFrom = 2,
                                        LineTo = 4
                                    },
                                    "newVar"
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGenerateExtractVariableGeneric.hx"))
                                .SetName("GenerateExtractVariable");

                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeExtractLocalVariable_fromString.hx"),
                                    new MemberModel("extractLocalVariable", null, FlagType.Function, Visibility.Public)
                                    {
                                        LineFrom = 4,
                                        LineTo = 7
                                    },
                                    "newVar"
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterExtractLocalVariable_fromString.hx"))
                                .SetName("ExtractLocaleVariable from String");

                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeExtractLocalVariable_fromNumber.hx"),
                                    new MemberModel("extractLocalVariable", null, FlagType.Function, Visibility.Public)
                                    {
                                        LineFrom = 4,
                                        LineTo = 7
                                    },
                                    "newVar"
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterExtractLocalVariable_fromNumber.hx"))
                                .SetName("ExtractLocaleVariable from Number");

                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeExtractLocalVariable_inSinglelineMethod.hx"),
                                    new MemberModel("extractLocalVariable", null, FlagType.Function, Visibility.Public)
                                    {
                                        LineFrom = 4,
                                        LineTo = 5
                                    },
                                    "newVar"
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterExtractLocalVariable_inSinglelineMethod.hx"))
                                .SetName("ExtractLocaleVariable in single line method");
                    }
                }

                [Test, TestCaseSource("GenerateExtractVariableHaxeTestCases")]
                public string Haxe(string sourceText, MemberModel currentMember, string newName)
                {
                    ASContext.Context.SetHaxeFeatures();
                    ASContext.Context.CurrentModel.Returns(new FileModel {haXe = true, Context = ASContext.Context});
                    ASContext.Context.CurrentMember.Returns(currentMember);
                    sci.Text = sourceText;
                    sci.ConfigurationLanguage = "haxe";
                    SnippetHelper.PostProcessSnippets(sci, 0);
                    ASGenerator.GenerateExtractVariable(sci, newName);
                    return sci.Text;
                }

                public IEnumerable<TestCaseData> GenerateExtractVariableAS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeExtractLocalVariable.as"),
                                    new MemberModel("ExtractLocalVariable", null, FlagType.Constructor | FlagType.Function, 0)
                                    {
                                        LineFrom = 4,
                                        LineTo = 7
                                    },
                                    "newVar"
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterExtractLocalVariable.as"))
                                .SetName("ExtractLocaleVariable");

                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeExtractLocalVariable_fromString.as"),
                                    new MemberModel("ExtractLocalVariable", null, FlagType.Constructor | FlagType.Function, 0)
                                    {
                                        LineFrom = 4,
                                        LineTo = 7
                                    },
                                    "newVar"
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterExtractLocalVariable_fromString.as"))
                                .SetName("ExtractLocaleVariable from String");

                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeExtractLocalVariable_fromNumber.as"),
                                    new MemberModel("ExtractLocalVariable", null, FlagType.Constructor | FlagType.Function, 0)
                                    {
                                        LineFrom = 4,
                                        LineTo = 7
                                    },
                                    "newVar"
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterExtractLocalVariable_fromNumber.as"))
                                .SetName("ExtractLocaleVariable from Number");

                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeExtractLocalVariable_forCheckingThePositionOfNewVar.as"),
                                    new MemberModel("extractLocalVariable", null, FlagType.Function, Visibility.Public)
                                    {
                                        LineFrom = 4,
                                        LineTo = 10
                                    },
                                    "newVar"
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterExtractLocalVariable_forCheckingThePositionOfNewVar.as"))
                                .SetName("ExtractLocaleVariable with checking the position of a new variable");
                    }
                }

                [Test, TestCaseSource("GenerateExtractVariableAS3TestCases")]
                public string AS3(string sourceText, MemberModel currentMember, string newName)
                {
                    ASContext.Context.SetHaxeFeatures();
                    ASContext.Context.CurrentModel.Returns(new FileModel { Context = ASContext.Context });
                    ASContext.Context.CurrentMember.Returns(currentMember);
                    sci.Text = sourceText;
                    sci.ConfigurationLanguage = "as3";
                    SnippetHelper.PostProcessSnippets(sci, 0);
                    ASGenerator.GenerateExtractVariable(sci, newName);
                    return sci.Text;
                }
            }

            [TestFixture]
            public class GenerateGetterSetter : GenerateJob
            {
                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGenerateGetterSetter.hx"))
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGenerateGetterSetter.hx"))
                                .SetName("Generate getter and setter");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGenerateGetterSetter_issue221.hx"))
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGenerateGetterSetter_issue221.hx"))
                                .SetName("issue 221");
                    }
                }

                [Test, TestCaseSource("HaxeTestCases")]
                public string Haxe(string sourceText)
                {
                    sci.Text = sourceText;
                    sci.ConfigurationLanguage = "haxe";
                    SnippetHelper.PostProcessSnippets(sci, 0);
                    ASContext.Context.SetHaxeFeatures();
                    var currentModel = new FileModel {haXe = true, Context = ASContext.Context};
                    new ASFileParser().ParseSrc(currentModel, sci.Text);
                    var currentClass = currentModel.Classes[0];
                    var currentMember = currentClass.Members[0];
                    ASContext.Context.CurrentModel.Returns(currentModel);
                    ASContext.Context.CurrentClass.Returns(currentClass);
                    ASContext.Context.CurrentMember.Returns(currentMember);
                    ASGenerator.GenerateJob(GeneratorJobType.GetterSetter, currentMember, ASContext.Context.CurrentClass, null, null);
                    return sci.Text;
                }

                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateGetterSetter_fromPublicField.as"))
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGenerateGetterSetter_fromPublicField.as"))
                                .SetName("Generate getter and setter from public field");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateGetterSetter_fromPublicFieldIfNameStartWith_.as"))
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGenerateGetterSetter_fromPublicFieldIfNameStartWith_.as"))
                                .SetName("Generate getter and setter from public field if name start with \"_\"");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateGetterSetter_fromPrivateField.as"))
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGenerateGetterSetter_fromPrivateField.as"))
                                .SetName("Generate getter and setter from private field");
                    }
                }

                [Test, TestCaseSource("AS3TestCases")]
                public string AS3(string sourceText)
                {
                    sci.Text = sourceText;
                    sci.ConfigurationLanguage = "as3";
                    SnippetHelper.PostProcessSnippets(sci, 0);
                    ASContext.Context.SetAs3Features();
                    var currentModel = new FileModel {Context = ASContext.Context};
                    new ASFileParser().ParseSrc(currentModel, sci.Text);
                    ASContext.Context.CurrentModel.Returns(currentModel);
                    var currentClass = currentModel.Classes[0];
                    var currentMember = currentClass.Members[0];
                    ASContext.Context.CurrentClass.Returns(currentClass);
                    ASContext.Context.CurrentMember.Returns(currentMember);
                    ASGenerator.GenerateJob(GeneratorJobType.GetterSetter, currentMember, ASContext.Context.CurrentClass, null, null);
                    return sci.Text;
                }
            }
        }
    }
}
