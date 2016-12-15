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
using AS3Context;
using HaXeContext;
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
                    yield return new TestCaseData("function test(arg:{x:Int}):void{\r\n\r\n}", 0, 1, "function test(arg:{x:Int}):void{\r\n\r\n}", 34).SetName("AnonymousStructures");
                }
            }

            [Test, TestCaseSource(nameof(GetBodyStartTestCases))]
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

                [Test, TestCaseSource(nameof(FieldFromParameterCommonTestCases))]
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
                    ASGenerator.GenerateJob(GeneratorJobType.FieldFromParameter, sourceMember, inClass, null, table);

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

                [Test, TestCaseSource(nameof(ImplementInterfaceAs3TestCases))]
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

                [Test, TestCaseSource(nameof(ImplementInterfaceHaxeTestCases))]
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
            public class PromoteLocal : GenerateJob
            {
                internal static string[] DeclarationModifierOrder = { "public", "protected", "internal", "private", "static", "override" };

                internal static string ReadAllTextAS3(string fileName)
                {
                    return TestFile.ReadAllText($"ASCompletion.Test_Files.generated.as3.{fileName}");
                }

                internal static string ReadAllTextHaxe(string fileName)
                {
                    return TestFile.ReadAllText($"ASCompletion.Test_Files.generated.haxe.{fileName}");
                }

                [TestFixtureSetUp]
                public void PromoteLocalWithSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = PromoteLocal.DeclarationModifierOrder;
                }

                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforePromoteLocal.as"))
                                .Returns(ReadAllTextAS3("AfterPromoteLocal_generateExplicitScopeIsFalse.as"))
                                .SetName("Promote to class member");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText) => GenerateAS3(sourceText, sci);

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforePromoteLocal.hx"))
                                .Returns(ReadAllTextHaxe("AfterPromoteLocal_generateExplicitScopeIsFalse.hx"))
                                .SetName("Promote to class member");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText) => GenerateHaxe(sourceText, sci);

                internal static string GenerateAS3(string sourceText, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "as3";
                    ASContext.Context.SetAs3Features();
                    ASContext.Context.CurrentModel.Returns(new FileModel {Context = ASContext.Context});
                    var context = new AS3Context.Context(new AS3Settings());
                    return Generate(sourceText, context, sci);
                }

                internal static string GenerateHaxe(string sourceText, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "haxe";
                    ASContext.Context.SetHaxeFeatures();
                    ASContext.Context.CurrentModel.Returns(new FileModel {haXe = true, Context = ASContext.Context});
                    return Generate(sourceText, new HaXeContext.Context(new HaXeSettings()), sci);
                }

                static string Generate(string sourceText, IASContext context, ScintillaControl sci)
                {
                    sci.Text = sourceText;
                    SnippetHelper.PostProcessSnippets(sci, 0);
                    var currentModel = ASContext.Context.CurrentModel;
                    new ASFileParser().ParseSrc(currentModel, sci.Text);
                    var currentClass = currentModel.Classes[0];
                    ASContext.Context.CurrentClass.Returns(currentClass);
                    ASContext.Context.CurrentMember.Returns(currentClass.Members[0]);
                    ASContext.Context.GetVisibleExternalElements().Returns(x => context.GetVisibleExternalElements());
                    ASContext.Context.GetCodeModel(null).ReturnsForAnyArgs(x =>
                    {
                        var src = x[0] as string;
                        return string.IsNullOrEmpty(src) ? null : context.GetCodeModel(src);
                    });
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(x => context.ResolveType(x.ArgAt<string>(0), x.ArgAt<FileModel>(1)));
                    var expr = ASComplete.GetExpressionType(sci, sci.CurrentPos);
                    var currentMember = expr.Context.LocalVars[0];
                    ASGenerator.contextMember = currentMember;
                    ASGenerator.GenerateJob(GeneratorJobType.PromoteLocal, currentMember, ASContext.Context.CurrentClass, null, null);
                    return sci.Text;
                }
            }

            [TestFixture]
            public class PromoteLocalWithExplicitScope : GenerateJob
            {
                [TestFixtureSetUp]
                public void PromoteLocalWithSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = PromoteLocal.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateScope = true;
                }

                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(PromoteLocal.ReadAllTextAS3("BeforePromoteLocal.as"))
                                .Returns(PromoteLocal.ReadAllTextAS3("AfterPromoteLocal_generateExplicitScopeIsTrue.as"))
                                .SetName("Promote to class member");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText) => PromoteLocal.GenerateAS3(sourceText, sci);

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(PromoteLocal.ReadAllTextHaxe("BeforePromoteLocal.hx"))
                                .Returns(PromoteLocal.ReadAllTextHaxe("AfterPromoteLocal_generateExplicitScopeIsTrue.hx"))
                                .SetName("Promote to class member");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText) => PromoteLocal.GenerateHaxe(sourceText, sci);
            }

            [TestFixture]
            public class GenerateFunction : GenerateJob
            {
                internal static string[] DeclarationModifierOrder = { "public", "protected", "internal", "private", "static", "override" };

                [TestFixtureSetUp]
                public void GenerateFunctionSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = DeclarationModifierOrder;
                }

                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateFunction.as"),
                                GeneratorJobType.Function
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGeneratePrivateFunction_generateExplicitScopeIsFalse.as"))
                                .SetName("Generate private function");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateFunction.as"),
                                GeneratorJobType.FunctionPublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGeneratePublicFunction_generateExplicitScopeIsFalse.as"))
                                .SetName("Generate public function");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateFunction_forSomeObj.as"),
                                GeneratorJobType.FunctionPublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGenerateFunction_forSomeObj.as"))
                                .SetName("From some.foo|();");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateFunction_forSomeObj2.as"),
                                GeneratorJobType.FunctionPublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGenerateFunction_forSomeObj2.as"))
                                .SetName("From new Some().foo|();");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateFunction_forSomeObj3.as"),
                                GeneratorJobType.FunctionPublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGenerateFunction_forSomeObj3.as"))
                                .SetName("From new Some()\n.foo|();");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateStaticFunction.as"),
                                GeneratorJobType.FunctionPublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGeneratePublicStaticFunction_generateExplicitScopeIsFalse.as"))
                                .SetName("Generate public static function");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateStaticFunction_forCurrentType.as"),
                                GeneratorJobType.FunctionPublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGeneratePublicStaticFunction_generateExplicitScopeIsTrue.as"))
                                .SetName("From CurrentType.foo|");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateStaticFunction_forSomeType.as"),
                                GeneratorJobType.FunctionPublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGeneratePublicStaticFunction_forSomeType.as"))
                                .SetName("From SomeType.foo|");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, GeneratorJobType job) => GenerateAS3(sourceText, job, sci);

                internal static string GenerateAS3(string sourceText, GeneratorJobType job, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "as3";
                    ASContext.Context.SetAs3Features();
                    ASContext.Context.CurrentModel.Returns(new FileModel {Context = ASContext.Context});
                    var context = new AS3Context.Context(new AS3Settings());
                    context.BuildClassPath();
                    return Generate(sourceText, job, context, sci);
                }

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGenerateFunction.hx"),
                                GeneratorJobType.Function
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePrivateFunction_generateExplicitScopeIsFalse.hx"))
                                .SetName("Generate private function");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGenerateFunction.hx"),
                                GeneratorJobType.FunctionPublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePublicFunction_generateExplicitScopeIsFalse.hx"))
                                .SetName("Generate public function");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job) => GenerateHaxe(sourceText, job, sci);

                internal static string GenerateHaxe(string sourceText, GeneratorJobType job, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "haxe";
                    ASContext.Context.SetHaxeFeatures();
                    ASContext.Context.CurrentModel.Returns(new FileModel {haXe = true, Context = ASContext.Context});
                    return Generate(sourceText, job, new HaXeContext.Context(new HaXeSettings()), sci);
                }

                static string Generate(string sourceText, GeneratorJobType job, IASContext context, ScintillaControl sci)
                {
                    sci.Text = sourceText;
                    SnippetHelper.PostProcessSnippets(sci, 0);
                    var currentModel = ASContext.Context.CurrentModel;
                    new ASFileParser().ParseSrc(currentModel, sci.Text);
                    var currentClass = currentModel.Classes[0];
                    ASContext.Context.CurrentClass.Returns(currentClass);
                    var currentMember = currentClass.Members[0];
                    ASContext.Context.CurrentMember.Returns(currentMember);
                    ASContext.Context.GetVisibleExternalElements().Returns(x => context.GetVisibleExternalElements());
                    ASContext.Context.GetCodeModel(null).ReturnsForAnyArgs(x =>
                    {
                        var src = x[0] as string;
                        return string.IsNullOrEmpty(src) ? null : context.GetCodeModel(src);
                    });
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(x => context.ResolveType(x.ArgAt<string>(0), x.ArgAt<FileModel>(1)));
                    ASGenerator.contextToken = sci.GetWordFromPosition(sci.CurrentPos);
                    ASGenerator.GenerateJob(job, currentMember, ASContext.Context.CurrentClass, null, null);
                    return sci.Text;
                }
            }

            [TestFixture]
            public class GenerateFunctionWithExplicitScope : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateFunctionSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateFunction.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateScope = true;
                }

                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateFunction.as"),
                                GeneratorJobType.Function
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGeneratePrivateFunction_generateExplicitScopeIsTrue.as"))
                                .SetName("Generate private function");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateFunction.as"),
                                GeneratorJobType.FunctionPublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGeneratePublicFunction_generateExplicitScopeIsTrue.as"))
                                .SetName("Generate public function");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateFunction_forSomeObj.as"),
                                GeneratorJobType.FunctionPublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGenerateFunction_forSomeObj.as"))
                                .SetName("From some.foo|();");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateFunction_forSomeObj2.as"),
                                GeneratorJobType.FunctionPublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGenerateFunction_forSomeObj2.as"))
                                .SetName("From new Some().foo|();");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateFunction_forSomeObj3.as"),
                                GeneratorJobType.FunctionPublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGenerateFunction_forSomeObj3.as"))
                                .SetName("From new Some()\n.foo|();");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateStaticFunction.as"),
                                GeneratorJobType.FunctionPublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGeneratePublicStaticFunction_generateExplicitScopeIsTrue.as"))
                                .SetName("Generate public static function");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateStaticFunction_forCurrentType.as"),
                                GeneratorJobType.FunctionPublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGeneratePublicStaticFunction_generateExplicitScopeIsTrue.as"))
                                .SetName("From CurrentType.foo|");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateStaticFunction_forSomeType.as"),
                                GeneratorJobType.FunctionPublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGeneratePublicStaticFunction_forSomeType.as"))
                                .SetName("From SomeType.foo|");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, GeneratorJobType job) => GenerateFunction.GenerateAS3(sourceText, job, sci);

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGenerateFunction.hx"),
                                GeneratorJobType.Function
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePrivateFunction_generateExplicitScopeIsTrue.hx"))
                                .SetName("Generate private function");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGenerateFunction.hx"),
                                GeneratorJobType.FunctionPublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePublicFunction_generateExplicitScopeIsTrue.hx"))
                                .SetName("Generate public function");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job) => GenerateFunction.GenerateHaxe(sourceText, job, sci);
            }

            [TestFixture]
            public class GenerateFunctionWithDefaultModifierDeclaration : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateFunctionSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateFunction.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
                }

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.BeforeGenerateFunction.hx"),
                                    GeneratorJobType.Function
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePrivateFunctionWithDefaultModifier.hx"))
                                .SetName("Generate private function with default modifier declaration");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.BeforeGenerateStaticFunction.hx"),
                                    GeneratorJobType.Function
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePrivateStaticFunctionWithDefaultModifier.hx"))
                                .SetName("Generate private static function with default modifier declaration");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job) => GenerateFunction.GenerateHaxe(sourceText, job, sci);
            }

            [TestFixture]
            public class AssignStatementToVar : GenerateJob
            {
                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText("ASCompletion.Test_Files.generated.haxe.BeforeAssignStatementToVar_useSpaces.hx"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(TestFile.ReadAllText("ASCompletion.Test_Files.generated.haxe.AfterAssignStatementToVar_useSpaces.hx"))
                                .SetName("Assign statement to var. Use spaces instead of tabs.");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText("ASCompletion.Test_Files.generated.haxe.BeforeAssignStatementToVar_useTabs.hx"), GeneratorJobType.AssignStatementToVar, true)
                                .Returns(TestFile.ReadAllText("ASCompletion.Test_Files.generated.haxe.AfterAssignStatementToVar_useTabs.hx"))
                                .SetName("Assign statement to var. Use tabs instead of spaces.");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job, bool isUseTabs)
                {
                    sci.ConfigurationLanguage = "haxe";
                    sci.IsUseTabs = isUseTabs;
                    ASContext.Context.SetHaxeFeatures();
                    ASContext.Context.CurrentModel.Returns(new FileModel {haXe = true, Context = ASContext.Context});
                    return Generate(sourceText, job, new HaXeContext.Context(new HaXeSettings()));
                }

                string Generate(string sourceText, GeneratorJobType job, IASContext context)
                {
                    sci.Text = sourceText;
                    SnippetHelper.PostProcessSnippets(sci, 0);
                    var currentModel = ASContext.Context.CurrentModel;
                    new ASFileParser().ParseSrc(currentModel, sci.Text);
                    var currentClass = currentModel.Classes[0];
                    ASContext.Context.CurrentClass.Returns(currentClass);
                    ASContext.Context.CurrentModel.Returns(currentModel);
                    var currentMember = currentClass.Members[0];
                    ASContext.Context.CurrentMember.Returns(currentMember);
                    ASContext.Context.GetVisibleExternalElements().Returns(x => context.GetVisibleExternalElements());
                    ASContext.Context.GetCodeModel(null).ReturnsForAnyArgs(x =>
                    {
                        var src = x[0] as string;
                        return string.IsNullOrEmpty(src) ? null : context.GetCodeModel(src);
                    });
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(_ => new ClassModel {Name = "String", Type = "String", InFile = currentModel});
                    ASGenerator.contextToken = sci.GetWordFromPosition(sci.CurrentPos);
                    ASGenerator.GenerateJob(job, currentMember, ASContext.Context.CurrentClass, null, null);
                    return sci.Text;
                }
            }

            [TestFixture]
            public class GenerateVariable : GenerateJob
            {
                internal static string[] DeclarationModifierOrder = { "public", "protected", "internal", "private", "static", "override" };

                [TestFixtureSetUp]
                public void GenerateVariableSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = DeclarationModifierOrder;
                }

                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateVariable.as"),
                                GeneratorJobType.Variable
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGeneratePrivateVariable_generateExplicitScopeIsFalse.as"))
                                .SetName("Generate private variable");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateVariable.as"),
                                GeneratorJobType.VariablePublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGeneratePublicVariable_generateExplicitScopeIsFalse.as"))
                                .SetName("Generate public variable");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateVariable_forSomeObj.as"),
                                GeneratorJobType.VariablePublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGenerateVariable_forSomeObj.as"))
                                .SetName("From some.foo|");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateVariable_forSomeObj2.as"),
                                GeneratorJobType.VariablePublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGenerateVariable_forSomeObj2.as"))
                                .SetName("From new Some().foo|");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateVariable_forSomeObj3.as"),
                                GeneratorJobType.VariablePublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGenerateVariable_forSomeObj3.as"))
                                .SetName("From new Some()\n.foo|");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateStaticVariable_forCurrentType.as"),
                                GeneratorJobType.VariablePublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGeneratePublicStaticVariable_forCurrentType.as"))
                                .SetName("From CurrentType.foo|");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateStaticVariable_forSomeType.as"),
                                GeneratorJobType.VariablePublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGeneratePublicStaticVariable_forSomeType.as"))
                                .SetName("From SomeType.foo|");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, GeneratorJobType job) => GenerateAS3(sourceText, job, sci);

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGenerateVariable.hx"),
                                GeneratorJobType.Variable
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePrivateVariable_generateExplicitScopeIsFalse.hx"))
                                .SetName("Generate private variable");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGenerateStaticVariable.hx"),
                                GeneratorJobType.Variable
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePrivateStaticVariable.hx"))
                                .SetName("Generate private static variable");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGenerateVariable.hx"),
                                GeneratorJobType.VariablePublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePublicVariable_generateExplicitScopeIsFalse.hx"))
                                .SetName("Generate public variable");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGenerateStaticVariable.hx"),
                                GeneratorJobType.VariablePublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePublicStaticVariable_generateExplicitScopeIsFalse.hx"))
                                .SetName("Generate public static variable");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGeneratePublicStaticVariable_forSomeType.hx"),
                                GeneratorJobType.VariablePublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePublicStaticVariable_forSomeType.hx"))
                                .SetName("From SomeType.foo|");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGeneratePublicStaticVariable_forCurrentType.hx"),
                                GeneratorJobType.VariablePublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePublicStaticVariable_forCurrentType.hx"))
                                .SetName("From CurrentType.foo|");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job) => GenerateHaxe(sourceText, job, sci);

                internal static string GenerateAS3(string sourceText, GeneratorJobType job, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "as3";
                    ASContext.Context.SetAs3Features();
                    ASContext.Context.CurrentModel.Returns(new FileModel {Context = ASContext.Context});
                    var context = new AS3Context.Context(new AS3Settings());
                    context.BuildClassPath();
                    return Generate(sourceText, job, context, sci);
                }
                
                internal static string GenerateHaxe(string sourceText, GeneratorJobType job, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "haxe";
                    ASContext.Context.SetHaxeFeatures();
                    ASContext.Context.CurrentModel.Returns(new FileModel {haXe = true, Context = ASContext.Context});
                    return Generate(sourceText, job, new HaXeContext.Context(new HaXeSettings()), sci);
                }

                static string Generate(string sourceText, GeneratorJobType job, IASContext context, ScintillaControl sci)
                {
                    sci.Text = sourceText;
                    SnippetHelper.PostProcessSnippets(sci, 0);
                    var currentModel = ASContext.Context.CurrentModel;
                    new ASFileParser().ParseSrc(currentModel, sci.Text);
                    var currentClass = currentModel.Classes[0];
                    ASContext.Context.CurrentClass.Returns(currentClass);
                    ASContext.Context.CurrentModel.Returns(currentModel);
                    var currentMember = currentClass.Members[0];
                    ASContext.Context.CurrentMember.Returns(currentMember);
                    ASContext.Context.GetVisibleExternalElements().Returns(x => context.GetVisibleExternalElements());
                    ASContext.Context.GetCodeModel(null).ReturnsForAnyArgs(x =>
                    {
                        var src = x[0] as string;
                        return string.IsNullOrEmpty(src) ? null : context.GetCodeModel(src);
                    });
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(x => context.ResolveType(x.ArgAt<string>(0), x.ArgAt<FileModel>(1)));
                    ASGenerator.contextToken = sci.GetWordFromPosition(sci.CurrentPos);
                    ASGenerator.GenerateJob(job, currentMember, ASContext.Context.CurrentClass, null, null);
                    return sci.Text;
                }
            }

            [TestFixture]
            public class GenerateVariableWithExplicitScope : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateVariableSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateVariable.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateScope = true;
                }

                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateVariable.as"),
                                GeneratorJobType.Variable
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGeneratePrivateVariable_generateExplicitScopeIsTrue.as"))
                                .SetName("Generate private variable");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateVariable.as"),
                                GeneratorJobType.VariablePublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGeneratePublicVariable_generateExplicitScopeIsTrue.as"))
                                .SetName("Generate public variable");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateVariable_forSomeObj.as"),
                                GeneratorJobType.VariablePublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGenerateVariable_forSomeObj.as"))
                                .SetName("From some.foo|");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateVariable_forSomeObj2.as"),
                                GeneratorJobType.VariablePublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGenerateVariable_forSomeObj2.as"))
                                .SetName("From new Some().foo|");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateVariable_forSomeObj3.as"),
                                GeneratorJobType.VariablePublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGenerateVariable_forSomeObj3.as"))
                                .SetName("From new Some()\n.foo|");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, GeneratorJobType job) => GenerateVariable.GenerateAS3(sourceText, job, sci);

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGenerateVariable.hx"),
                                GeneratorJobType.Variable
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePrivateVariable_generateExplicitScopeIsTrue.hx"))
                                .SetName("Generate private variable");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGenerateVariable.hx"),
                                GeneratorJobType.VariablePublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePublicVariable_generateExplicitScopeIsTrue.hx"))
                                .SetName("Generate public variable");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGenerateStaticVariable.hx"),
                                GeneratorJobType.VariablePublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePublicStaticVariable_generateExplicitScopeIsTrue.hx"))
                                .SetName("Generate public static variable");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGeneratePublicStaticVariable_forSomeType.hx"),
                                GeneratorJobType.VariablePublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePublicStaticVariable_forSomeType.hx"))
                                .SetName("From SomeType.foo| variable");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGeneratePublicStaticVariable_forCurrentType.hx"),
                                GeneratorJobType.VariablePublic
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePublicStaticVariable_forCurrentType.hx"))
                                .SetName("From CurrentType.foo| variable");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job) => GenerateVariable.GenerateHaxe(sourceText, job, sci);
            }

            [TestFixture]
            public class GenerateVariableWithDefaultModifierDeclaration : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateVariableSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateVariable.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
                }

                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.BeforeGenerateVariable.as"),
                                    GeneratorJobType.Variable
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGeneratePrivateVariable_generateExplicitScopeIsFalse.as"))
                                .SetName("Generate private variable with default modifier declration");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.BeforeGenerateStaticVariable_forCurrentType.as"),
                                    GeneratorJobType.Variable
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGeneratePrivateStaticVariabeWithDefaultModifier.as"))
                                .SetName("Generate private static variable with default modifier declration");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, GeneratorJobType job) => GenerateVariable.GenerateAS3(sourceText, job, sci);

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.BeforeGenerateVariable.hx"),
                                    GeneratorJobType.Variable
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePrivateVariableWithDefaultModifier.hx"))
                                .SetName("Generate private variable with default modifier declration");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.BeforeGenerateStaticVariable.hx"),
                                    GeneratorJobType.Variable
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePrivateStaticVariableWithDefaultModifier.hx"))
                                .SetName("Generate private static variable with default modifier declration");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job) => GenerateVariable.GenerateHaxe(sourceText, job, sci);
            }

            [TestFixture]
            public class GenerateEventHandler : GenerateJob
            {
                internal static string[] DeclarationModifierOrder = { "public", "protected", "internal", "private", "static", "override" };

                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateEventHandler.as"),
                                new string[0]
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGenerateEventHandler_withoutAutoRemove.as"))
                                .SetName("Generate event handler without auto remove");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateEventHandler.as"),
                                new[] {"Event.ADDED", "Event.REMOVED"}
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGenerateEventHandler_withAutoRemove.as"))
                                .SetName("Generate event handler with auto remove");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, string[] autoRemove) => GenerateAS3(sourceText, autoRemove, sci);

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGenerateEventHandler.hx"),
                                new string[0]
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGenerateEventHandler_withoutAutoRemove.hx"))
                                .SetName("Generate event handler without auto remove");
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.haxe.BeforeGenerateEventHandler.hx"),
                                new[] {"Event.ADDED", "Event.REMOVED"}
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGenerateEventHandler_withAutoRemove.hx"))
                                .SetName("Generate event handler with auto remove");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, string[] autoRemove) => GenerateHaxe(sourceText, autoRemove, sci);

                internal static string GenerateAS3(string sourceText, string[] autoRemove, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "as3";
                    ASContext.Context.SetAs3Features();
                    ASContext.Context.CurrentModel.Returns(new FileModel {Context = ASContext.Context});
                    return Generate(sourceText, autoRemove, new AS3Context.Context(new AS3Settings()), sci);
                }

                internal static string GenerateHaxe(string sourceText, string[] autoRemove, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "haxe";
                    ASContext.Context.SetHaxeFeatures();
                    ASContext.Context.CurrentModel.Returns(new FileModel {haXe = true, Context = ASContext.Context});
                    return Generate(sourceText, autoRemove, new HaXeContext.Context(new HaXeSettings()), sci);
                }

                static string Generate(string sourceText, string[] autoRemove, IASContext context, ScintillaControl sci)
                {
                    sci.Text = sourceText;
                    SnippetHelper.PostProcessSnippets(sci, 0);
                    ASContext.CommonSettings.EventListenersAutoRemove = autoRemove;
                    var currentModel = ASContext.Context.CurrentModel;
                    new ASFileParser().ParseSrc(currentModel, sci.Text);
                    var currentClass = currentModel.Classes[0];
                    ASContext.Context.CurrentClass.Returns(currentClass);
                    var currentMember = currentClass.Members[0];
                    ASContext.Context.CurrentMember.Returns(currentMember);
                    ASContext.Context.GetVisibleExternalElements().Returns(x => context.GetVisibleExternalElements());
                    ASContext.Context.GetCodeModel(null).ReturnsForAnyArgs(x =>
                    {
                        var src = x[0] as string;
                        return string.IsNullOrEmpty(src) ? null : context.GetCodeModel(src);
                    });
                    var eventModel = new ClassModel {Name = "Event", Type = "flash.events.Event"};
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(x => eventModel);
                    ASGenerator.contextToken = sci.GetWordFromPosition(sci.CurrentPos);
                    var re = string.Format(ASGenerator.patternEvent, ASGenerator.contextToken);
                    var m = Regex.Match(sci.GetLine(sci.CurrentLine), re, RegexOptions.IgnoreCase);
                    ASGenerator.contextMatch = m;
                    ASGenerator.contextParam = ASGenerator.CheckEventType(m.Groups["event"].Value);
                    ASGenerator.GenerateJob(GeneratorJobType.ComplexEvent, currentMember, ASContext.Context.CurrentClass, null, null);
                    return sci.Text;
                }
            }

            [TestFixture]
            public class GenerateEventHandlerWithExplicitScope : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateEventHandlerSetup()
                {
                    ASContext.CommonSettings.GenerateScope = true;
                }

                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                TestFile.ReadAllText(
                                    "ASCompletion.Test_Files.generated.as3.BeforeGenerateEventHandler.as"),
                                new[] {"Event.ADDED", "Event.REMOVED"}
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterGenerateEventHandler_withAutoRemove_generateExplicitScopeIsTrue.as"))
                                .SetName("Generate event handler with auto remove");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, string[] autoRemove) => GenerateEventHandler.GenerateAS3(sourceText, autoRemove, sci);

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.BeforeGenerateEventHandler.hx"),
                                    new[] {"Event.ADDED", "Event.REMOVED"}
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGenerateEventHandler_withAutoRemove_generateExplicitScopeIsTrue.hx"))
                                .SetName("Generate event handler with auto remove");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, string[] autoRemove) => GenerateEventHandler.GenerateHaxe(sourceText, autoRemove, sci);
            }

            [TestFixture]
            public class GenerateEventHandlerWithDefaultModifierDeclaration : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateEventHandlerSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateEventHandler.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
                }

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.BeforeGenerateEventHandler.hx"),
                                    new[] {"Event.ADDED", "Event.REMOVED"}
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePrivateEventHandlerWithDefaultModifier.hx"))
                                .SetName("Generate private event handler with default modifier declaration");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.BeforeGeneratePrivateStaticEventHandler.hx"),
                                    new string[0]
                                )
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePrivateStaticEventHandlerWithDefaultModifier.hx"))
                                .SetName("Generate private static event handler with default modifier declaration");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, string[] autoRemove) => GenerateEventHandler.GenerateHaxe(sourceText, autoRemove, sci);
            }

            [TestFixture]
            public class GenerateGetterSetter : GenerateJob
            {
                internal static string[] DeclarationModifierOrder = { "public", "protected", "internal", "private", "static", "override" };

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

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText) => GenerateAS3(sourceText, sci);

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

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText) => GenerateHaxe(sourceText, sci);

                internal static string GenerateAS3(string sourceText, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "as3";
                    ASContext.Context.SetAs3Features();
                    ASContext.Context.CurrentModel.Returns(new FileModel {Context = ASContext.Context});
                    return Generate(sourceText, sci);
                }

                internal static string GenerateHaxe(string sourceText, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "haxe";
                    ASContext.Context.SetHaxeFeatures();
                    ASContext.Context.CurrentModel.Returns(new FileModel {haXe = true, Context = ASContext.Context});
                    return Generate(sourceText, sci);
                }

                static string Generate(string sourceText, ScintillaControl sci)
                {
                    sci.Text = sourceText;
                    SnippetHelper.PostProcessSnippets(sci, 0);
                    var currentModel = ASContext.Context.CurrentModel;
                    new ASFileParser().ParseSrc(currentModel, sci.Text);
                    var currentClass = currentModel.Classes[0];
                    var currentMember = currentClass.Members[0];
                    ASContext.Context.CurrentClass.Returns(currentClass);
                    ASContext.Context.CurrentMember.Returns(currentMember);
                    ASGenerator.GenerateJob(GeneratorJobType.GetterSetter, currentMember, ASContext.Context.CurrentClass, null, null);
                    return sci.Text;
                }
            }

            [TestFixture]
            public class GenerateGetterSetterWithDefaultModifierDeclaration : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateEventHandlerSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateGetterSetter.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
                }

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
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePrivateGetterSetterWithDefaultModifier.hx"))
                                .SetName("Generate private getter and setter with default modifier declaration");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.BeforeGeneratePrivateStaticGetterSetter.hx"))
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterGeneratePrivateStaticGetterSetterWithDefaultModifier.hx"))
                                .SetName("Generate private static getter and setter with default modifier declaration");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText) => GenerateGetterSetter.GenerateHaxe(sourceText, sci);
            }

            [TestFixture]
            public class GenerateOverride : GenerateJob
            {
                internal static string[] DeclarationModifierOrder = { "public", "protected", "internal", "private", "static", "override" };

                [TestFixtureSetUp]
                public void GenerateOverrideSetUp()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = DeclarationModifierOrder;
                    ASContext.Context.Settings.GenerateImports = true;
                }

                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.BeforeOverridePublicFunction.as"),
                                    "Foo",
                                    "foo",
                                    FlagType.Function)
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterOverridePublicFunction.as"))
                                .SetName("Override public function");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.BeforeOverrideProtectedFunction.as"),
                                    "Foo",
                                    "foo",
                                    FlagType.Function)
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterOverrideProtectedFunction.as"))
                                .SetName("Override proteced function");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.BeforeOverrideInternalFunction.as"),
                                    "Foo",
                                    "foo",
                                    FlagType.Function)
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterOverrideInternalFunction.as"))
                                .SetName("Override internal function");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.BeforeOverrideHasOwnProperty.as"),
                                    "Object",
                                    "hasOwnProperty",
                                    FlagType.Function)
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterOverrideHasOwnProperty.as"))
                                .SetName("Override hasOwnProperty");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.BeforeOverridePublicGetSet.as"),
                                    "Foo",
                                    "foo",
                                    FlagType.Getter)
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterOverridePublicGetSet.as"))
                                .SetName("Override public getter and setter");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.BeforeOverrideInternalGetSet.as"),
                                    "Foo",
                                    "foo",
                                    FlagType.Getter)
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.as3.AfterOverrideInternalGetSet.as"))
                                .SetName("Override internal getter and setter");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string source, string ofClassName, string memberName, FlagType memberFlags)
                {
                    return GenerateAS3(source, ofClassName, memberName, memberFlags, sci);
                }

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.BeforeOverrideGetNull.hx"),
                                    "Foo",
                                    "foo",
                                    FlagType.Getter | FlagType.Setter)
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterOverrideGetNull.hx"))
                                .SetName("Override var foo(get, null)");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.BeforeOverrideNullSet.hx"),
                                    "Foo",
                                    "foo",
                                    FlagType.Getter | FlagType.Setter)
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterOverrideNullSet.hx"))
                                .SetName("Override var foo(null, set)");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.BeforeOverrideGetSet.hx"),
                                    "Foo",
                                    "foo",
                                    FlagType.Getter | FlagType.Setter)
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterOverrideGetSet.hx"))
                                .SetName("Override var foo(get, set)");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.BeforeOverrideIssue793.hx"),
                                    "Foo",
                                    "foo",
                                    FlagType.Getter | FlagType.Setter)
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterOverrideIssue793.hx"))
                                .SetName("issue #793");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.BeforeOverridePublicFunction.hx"),
                                    "Foo",
                                    "foo",
                                    FlagType.Function)
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterOverridePublicFunction.hx"))
                                .SetName("Override public function");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.BeforeOverridePrivateFunction.hx"),
                                    "Foo",
                                    "foo",
                                    FlagType.Function)
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterOverridePrivateFunction.hx"))
                                .SetName("Override private function");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string source, string ofClassName, string memberName, FlagType memberFlags)
                {
                    return GenerateHaxe(source, ofClassName, memberName, memberFlags, sci);
                }

                internal static string GenerateAS3(string source, string ofClassName, string memberName, FlagType memberFlags, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "as3";
                    ASContext.Context.SetAs3Features();
                    ASContext.Context.CurrentModel.Returns(new FileModel {Context = ASContext.Context});
                    var context = new AS3Context.Context(new AS3Settings());
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(it => context.ResolveType(it.ArgAt<string>(0), it.ArgAt<FileModel>(1)));
                    return Generate(source, ofClassName, memberName, memberFlags, sci);
                }

                internal static string GenerateHaxe(string source, string ofClassName, string memberName, FlagType memberFlags, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "haxe";
                    ASContext.Context.SetHaxeFeatures();
                    ASContext.Context.CurrentModel.Returns(new FileModel {haXe = true, Context = ASContext.Context});
                    var context = new HaXeContext.Context(new HaXeSettings());
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(it => context.ResolveType(it.ArgAt<string>(0), it.ArgAt<FileModel>(1)));
                    return Generate(source, ofClassName, memberName, memberFlags, sci);
                }

                static string Generate(string source, string ofClassName, string memberName, FlagType memberFlags, ScintillaControl sci)
                {
                    sci.Text = source;
                    SnippetHelper.PostProcessSnippets(sci, 0);
                    var currentModel = ASContext.Context.CurrentModel;
                    new ASFileParser().ParseSrc(currentModel, sci.Text);
                    var ofClass = currentModel.Classes.Find(model => model.Name == ofClassName);
                    var member = ofClass.Members.Search(memberName, memberFlags, 0);
                    ASGenerator.GenerateOverride(sci, ofClass, member, sci.CurrentPos);
                    return sci.Text;
                }
            }

            [TestFixture]
            public class GenerateOverrideWithDefaultModifierDeclaration : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateOverrideSetUp()
                {
                    ASContext.Context.Settings.GenerateImports = true;
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateOverride.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
                }

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.BeforeOverrideGetNull.hx"),
                                    "Foo",
                                    "foo",
                                    FlagType.Getter | FlagType.Setter)
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterOverrideGetNullWithDefaultModifier.hx"))
                                .SetName("Override var foo(get, null) with default modifier declaration");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.BeforeOverrideNullSet.hx"),
                                    "Foo",
                                    "foo",
                                    FlagType.Getter | FlagType.Setter)
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterOverrideNullSetWithDefaultModifier.hx"))
                                .SetName("Override var foo(null, set) with default modifier declaration");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.BeforeOverrideGetSet.hx"),
                                    "Foo",
                                    "foo",
                                    FlagType.Getter | FlagType.Setter)
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterOverrideGetSetWithDefaultModifier.hx"))
                                .SetName("Override var foo(get, set) with default modifier declaration");
                        yield return
                            new TestCaseData(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.BeforeOverridePrivateFunction.hx"),
                                    "Foo",
                                    "foo",
                                    FlagType.Function)
                                .Returns(
                                    TestFile.ReadAllText(
                                        "ASCompletion.Test_Files.generated.haxe.AfterOverridePrivateFunctionWithDefaultModifier.hx"))
                                .SetName("Override private function with default modifier");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, string ofClassName, string memberName, FlagType memberFlags)
                {
                    return GenerateOverride.GenerateHaxe(sourceText, ofClassName, memberName, memberFlags, sci);
                }
            }
        }
    }
}