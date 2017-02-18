// TODO: Tests with different formatting options using parameterized tests

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System;
using ProjectManager.Projects.Haxe;

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
                            .Returns(ReadAllTextAS3("FieldFromParameterEmptyBody"))
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
                            .Returns(ReadAllTextAS3("FieldFromParameterWithSuperConstructor"))
                            .SetName("PublicScopeWithSuperConstructor");

                        yield return new TestCaseData(Visibility.Public,
                            ReadAllTextAS3("BeforeFieldFromParameterWithSuperConstructorMultiLine"),
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
                            .Returns(ReadAllTextAS3("FieldFromParameterWithSuperConstructorMultiLine"))
                            .SetName("PublicScopeWithSuperConstructorMultiLine");

                        yield return new TestCaseData(Visibility.Public,
                            ReadAllTextAS3("BeforeFieldFromParameterWithWrongSuperConstructor"),
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
                            .Returns(ReadAllTextAS3("FieldFromParameterWithWrongSuperConstructor"))
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
                            .Returns(ReadAllTextAS3("ImplementInterfaceNoMembers"))
                            .SetName("Full");

                        yield return new TestCaseData(ReadAllTextAS3("BeforeImplementInterfacePublicMemberBehindPrivate"),
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
                            .Returns(ReadAllTextAS3("ImplementInterfacePublicMemberBehindPrivate"))
                            .SetName("FullWithPublicMemberBehindPrivate");

                        yield return new TestCaseData(ReadAllTextAS3("BeforeImplementInterfaceNoPublicMember"),
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
                            .Returns(ReadAllTextAS3("ImplementInterfaceNoPublicMember"))
                            .SetName("FullWithoutPublicMember");
                    }
                }

                public IEnumerable<TestCaseData> ImplementInterfaceHaxeTestCases
                {
                    get
                    {
                        yield return new TestCaseData("package generatortest;\r\n\r\nclass ImplementTest{}",
                            new ClassModel { InFile = new FileModel(), LineFrom = 2, LineTo = 2 }, GetHaxeImplementInterfaceModel())
                            .Returns(ReadAllTextHaxe("ImplementInterfaceNoMembers"))
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
                            .Returns(ReadAllTextHaxe("ImplementInterfaceNoMembersInsertSingleProperty"))
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

                [TestFixtureSetUp]
                public void PromoteLocalWithSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = DeclarationModifierOrder;
                }

                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforePromoteLocal"))
                                .Returns(ReadAllTextAS3("AfterPromoteLocal_generateExplicitScopeIsFalse"))
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
                            new TestCaseData(ReadAllTextHaxe("BeforePromoteLocal"))
                                .Returns(ReadAllTextHaxe("AfterPromoteLocal_generateExplicitScopeIsFalse"))
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
                            new TestCaseData(ReadAllTextAS3("BeforePromoteLocal"))
                                .Returns(ReadAllTextAS3("AfterPromoteLocal_generateExplicitScopeIsTrue"))
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
                            new TestCaseData(ReadAllTextHaxe("BeforePromoteLocal"))
                                .Returns(ReadAllTextHaxe("AfterPromoteLocal_generateExplicitScopeIsTrue"))
                                .SetName("Promote to class member");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText) => PromoteLocal.GenerateHaxe(sourceText, sci);
            }

            [TestFixture]
            public class PromoteLocalWithDefaultModifierDeclaration : GenerateJob
            {
                [TestFixtureSetUp]
                public void PromoteLocalWithSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = PromoteLocal.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
                }

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforePromoteLocal"))
                                .Returns(ReadAllTextHaxe("AfterPromoteLocalWithDefaultModifier"))
                                .SetName("Promote to private class member with default modifier declaration");
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
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateFunction_generateExplicitScopeIsFalse"))
                                .SetName("Generate private function");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicFunction_generateExplicitScopeIsFalse"))
                                .SetName("Generate public function");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_forSomeObj"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_forSomeObj"))
                                .SetName("From some.foo|();");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_forSomeObj2"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_forSomeObj2"))
                                .SetName("From new Some().foo|();");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_forSomeObj3"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_forSomeObj3"))
                                .SetName("From new Some()\n.foo|();");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateStaticFunction"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicStaticFunction_generateExplicitScopeIsFalse"))
                                .SetName("Generate public static function");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateStaticFunction_forCurrentType"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicStaticFunction_generateExplicitScopeIsTrue"))
                                .SetName("From CurrentType.foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateStaticFunction_forSomeType"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicStaticFunction_forSomeType"))
                                .SetName("From SomeType.foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue1436"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateFunction_issue1436"))
                                .SetName("From foo(vector[0])")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1436");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_TwoDimensionalVector_issue1436"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateFunction_TwoDimensionalVector_issue1436"))
                                .SetName("From foo(vector[0][0])")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1436");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_MultidimensionalVector_issue1436"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateFunction_MultidimensionalVector_issue1436"))
                                .SetName("From foo(vector[0][0][0][0])")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1436");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, GeneratorJobType job) => GenerateAS3(sourceText, job, sci);

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction"), GeneratorJobType.Function )
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateFunction_generateExplicitScopeIsFalse"))
                                .SetName("Generate private function");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicFunction_generateExplicitScopeIsFalse"))
                                .SetName("Generate public function");
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
                    BuildClassPath(context);
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
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateFunction_generateExplicitScopeIsTrue"))
                                .SetName("Generate private function");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicFunction_generateExplicitScopeIsTrue"))
                                .SetName("Generate public function");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_forSomeObj"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_forSomeObj"))
                                .SetName("From some.foo|();");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_forSomeObj2"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_forSomeObj2"))
                                .SetName("From new Some().foo|();");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_forSomeObj3"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_forSomeObj3"))
                                .SetName("From new Some()\n.foo|();");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateStaticFunction"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicStaticFunction_generateExplicitScopeIsTrue"))
                                .SetName("Generate public static function");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateStaticFunction_forCurrentType"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicStaticFunction_generateExplicitScopeIsTrue"))
                                .SetName("From CurrentType.foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateStaticFunction_forSomeType"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicStaticFunction_forSomeType"))
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
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateFunction_generateExplicitScopeIsTrue"))
                                .SetName("Generate private function");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicFunction_generateExplicitScopeIsTrue"))
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
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateFunctionWithDefaultModifier"))
                                .SetName("Generate private function with default modifier declaration");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateStaticFunction"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateStaticFunctionWithDefaultModifier"))
                                .SetName("Generate private static function with default modifier declaration");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job) => GenerateFunction.GenerateHaxe(sourceText, job, sci);
            }

            [TestFixture]
            public class GenerateFunctionWithProtectedDeclaration : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateFunctionSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateFunction.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateProtectedDeclarations = true;
                }

                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateProtectedFunction"))
                                .SetName("Generate private function with protected modifier declration");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, GeneratorJobType job) => GenerateVariable.GenerateAS3(sourceText, job, sci);
            }

            [TestFixture]
            public class AssignStatementToVar : GenerateJob
            {

                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromVectorInitializer"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromVectorInitializer"))
                                .SetName("from new <int>[]");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromTwoDimensionalVectorInitializer"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromTwoDimensionalVectorInitializer"))
                                .SetName("from new <Vector.<int>>[new <int>[]]");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromFIeldOfItemOfVector"), GeneratorJobType.AssignStatementToVar, true)
                                .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromFIeldOfItemOfVector"))
                                .SetName("from v[0][0].length");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromMultilineArrayInitializer_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromMultilineArrayInitializer_useSpaces"))
                                .SetName("From multiline array initializer");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromMultilineObjectInitializer_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromMultilineObjectInitializer_useSpaces"))
                                .SetName("From multiline object initializer");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromMultilineObjectInitializer2_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromMultilineObjectInitializer2_useSpaces"))
                                .SetName("From multiline object initializer 2");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromMethodChaining_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromMethodChaining_useSpaces"))
                                .SetName("From method chaining");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromMethodChaining2_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromMethodChaining2_useSpaces"))
                                .SetName("From method chaining 2");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromNewString_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromNewString_useSpaces"))
                                .SetName("From new String(\"\")");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromNewString2_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromNewString2_useSpaces"))
                                .SetName("From new String(\"\".charAt(0))");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromNewBitmapDataWithParams_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromNewBitmapDataWithParams_useSpaces"))
                                .SetName("From new BitmapData(rect.width, rect.height)");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromNewBitmapDataWithParams_multiline_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromNewBitmapDataWithParams_multiline_useSpaces"))
                                .SetName("From new BitmapData(rect.width, rect.height). Multiline constructor");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromArrayInitializer_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromArrayInitializer_useSpaces"))
                                .SetName("from []");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromArrayInitializer2_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromArrayInitializer2_useSpaces"))
                                .SetName("from [rect.width, rect.height]");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromFunctionResult_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromFunctionResult_useSpaces"))
                                .SetName("from foo()");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromCallback_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromCallback_useSpaces"))
                                .SetName("from callback");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, GeneratorJobType job, bool isUseTabs) => GenerateAS3(sourceText, job, isUseTabs, sci);

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVar_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVar_useSpaces"))
                                .SetName("Assign statement to var. Use spaces instead of tabs.");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVar_useTabs"), GeneratorJobType.AssignStatementToVar, true)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVar_useTabs"))
                                .SetName("Assign statement to var. Use tabs instead of spaces.");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVarFromFieldOfItemOfArray"), GeneratorJobType.AssignStatementToVar, true)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVarFromFieldOfItemOfArray"))
                                .SetName("from a[0][0].length");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job, bool isUseTabs) => GenerateHaxe(sourceText, job, isUseTabs, sci);

                internal static string GenerateAS3(string sourceText, GeneratorJobType job, bool isUseTabs, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "as3";
                    sci.IsUseTabs = isUseTabs;
                    ASContext.Context.SetAs3Features();
                    var currentModel = new FileModel {Context = ASContext.Context, Version = 3};
                    ASContext.Context.CurrentModel.Returns(currentModel);
                    var context = new AS3Context.Context(new AS3Settings());
                    BuildClassPath(context);
                    context.CurrentModel = currentModel;
                    return Generate(sourceText, job, context, sci);
                }

                internal static string GenerateHaxe(string sourceText, GeneratorJobType job, bool isUseTabs, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "haxe";
                    sci.IsUseTabs = isUseTabs;
                    ASContext.Context.SetHaxeFeatures();
                    var currentModel = new FileModel {haXe = true, Context = ASContext.Context, Version = 4};
                    ASContext.Context.CurrentModel.Returns(currentModel);
                    var context = new HaXeContext.Context(new HaXeSettings());
                    BuildClassPath(context);
                    context.CurrentModel = currentModel;
                    return Generate(sourceText, job, context, sci);
                }

                internal static string Generate(string sourceText, GeneratorJobType job, IASContext context, ScintillaControl sci)
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
                    var visibleExternalElements = context.GetVisibleExternalElements();
                    ASContext.Context.GetVisibleExternalElements().Returns(visibleExternalElements);
                    ASContext.Context.GetCodeModel(null).ReturnsForAnyArgs(x =>
                    {
                        var src = x[0] as string;
                        return string.IsNullOrEmpty(src) ? null : context.GetCodeModel(src);
                    });
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(it => context.ResolveType(it.ArgAt<string>(0), it.ArgAt<FileModel>(1)));
                    ASContext.Context.IsImported(null, Arg.Any<int>()).ReturnsForAnyArgs(it =>
                    {
                        var member = it.ArgAt<MemberModel>(0);
                        return member != null && context.IsImported(member, it.ArgAt<int>(1));
                    });
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
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateVariable_generateExplicitScopeIsFalse"))
                                .SetName("Generate private variable");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicVariable_generateExplicitScopeIsFalse"))
                                .SetName("Generate public variable");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable_forSomeObj"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGenerateVariable_forSomeObj"))
                                .SetName("From some.foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable_forSomeObj2"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGenerateVariable_forSomeObj2"))
                                .SetName("From new Some().foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable_forSomeObj3"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGenerateVariable_forSomeObj3"))
                                .SetName("From new Some()\n.foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateStaticVariable_forCurrentType"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicStaticVariable_forCurrentType"))
                                .SetName("From CurrentType.foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateStaticVariable_forSomeType"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicStaticVariable_forSomeType"))
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
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateVariable"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateVariable_generateExplicitScopeIsFalse"))
                                .SetName("Generate private variable");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateStaticVariable"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateStaticVariable"))
                                .SetName("Generate private static variable");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateVariable"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicVariable_generateExplicitScopeIsFalse"))
                                .SetName("Generate public variable");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateStaticVariable"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicStaticVariable_generateExplicitScopeIsFalse"))
                                .SetName("Generate public static variable");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGeneratePublicStaticVariable_forSomeType"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicStaticVariable_forSomeType"))
                                .SetName("From SomeType.foo|");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGeneratePublicStaticVariable_forCurrentType"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicStaticVariable_forCurrentType"))
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
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateVariable_generateExplicitScopeIsTrue"))
                                .SetName("Generate private variable");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicVariable_generateExplicitScopeIsTrue"))
                                .SetName("Generate public variable");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable_forSomeObj"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGenerateVariable_forSomeObj"))
                                .SetName("From some.foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable_forSomeObj2"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGenerateVariable_forSomeObj2"))
                                .SetName("From new Some().foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable_forSomeObj3"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGenerateVariable_forSomeObj3"))
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
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateVariable"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateVariable_generateExplicitScopeIsTrue"))
                                .SetName("Generate private variable");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateVariable"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicVariable_generateExplicitScopeIsTrue"))
                                .SetName("Generate public variable");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateStaticVariable"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicStaticVariable_generateExplicitScopeIsTrue"))
                                .SetName("Generate public static variable");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGeneratePublicStaticVariable_forSomeType"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicStaticVariable_forSomeType"))
                                .SetName("From SomeType.foo| variable");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGeneratePublicStaticVariable_forCurrentType"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicStaticVariable_forCurrentType"))
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
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateVariable_generateExplicitScopeIsFalse"))
                                .SetName("Generate private variable with default modifier declration");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateStaticVariable_forCurrentType"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateStaticVariabeWithDefaultModifier"))
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
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateVariable"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateVariableWithDefaultModifier"))
                                .SetName("Generate private variable with default modifier declration");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateStaticVariable"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateStaticVariableWithDefaultModifier"))
                                .SetName("Generate private static variable with default modifier declration");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job) => GenerateVariable.GenerateHaxe(sourceText, job, sci);
            }

            [TestFixture]
            public class GenerateVariableWithProtectedDeclaration : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateVariableSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateVariable.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateProtectedDeclarations = true;
                }

                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextAS3("AfterGenerateProtectedVariable"))
                                .SetName("Generate private variable with protected modifier declration");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, GeneratorJobType job) => GenerateVariable.GenerateAS3(sourceText, job, sci);
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
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateEventHandler"), new string[0])
                                .Returns(ReadAllTextAS3("AfterGenerateEventHandler_withoutAutoRemove"))
                                .SetName("Generate event handler without auto remove");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateEventHandler"), new[] {"Event.ADDED", "Event.REMOVED"})
                                .Returns(ReadAllTextAS3("AfterGenerateEventHandler_withAutoRemove"))
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
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateEventHandler"), new string[0])
                                .Returns(ReadAllTextHaxe("AfterGenerateEventHandler_withoutAutoRemove"))
                                .SetName("Generate event handler without auto remove");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateEventHandler"), new[] {"Event.ADDED", "Event.REMOVED"})
                                .Returns(ReadAllTextHaxe("AfterGenerateEventHandler_withAutoRemove"))
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
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateEventHandler"), new[] {"Event.ADDED", "Event.REMOVED"})
                                .Returns(ReadAllTextAS3("AfterGenerateEventHandler_withAutoRemove_generateExplicitScopeIsTrue"))
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
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateEventHandler"), new[] {"Event.ADDED", "Event.REMOVED"})
                                .Returns(ReadAllTextHaxe("AfterGenerateEventHandler_withAutoRemove_generateExplicitScopeIsTrue"))
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
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateEventHandler"), new[] {"Event.ADDED", "Event.REMOVED"})
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateEventHandlerWithDefaultModifier"))
                                .SetName("Generate private event handler with default modifier declaration");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGeneratePrivateStaticEventHandler"), new string[0])
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateStaticEventHandlerWithDefaultModifier"))
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
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateGetterSetter_fromPublicField"))
                                .Returns(ReadAllTextAS3("AfterGenerateGetterSetter_fromPublicField"))
                                .SetName("Generate getter and setter from public field");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateGetterSetter_fromPublicFieldIfNameStartWith_"))
                                .Returns(ReadAllTextAS3("AfterGenerateGetterSetter_fromPublicFieldIfNameStartWith_"))
                                .SetName("Generate getter and setter from public field if name start with \"_\"");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateGetterSetter_fromPrivateField"))
                                .Returns(ReadAllTextAS3("AfterGenerateGetterSetter_fromPrivateField"))
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
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateGetterSetter"))
                                .Returns(ReadAllTextHaxe("AfterGenerateGetterSetter"))
                                .SetName("Generate getter and setter");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateGetterSetter_issue221"))
                                .Returns(ReadAllTextHaxe("AfterGenerateGetterSetter_issue221"))
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
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateGetterSetter"))
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateGetterSetterWithDefaultModifier"))
                                .SetName("Generate private getter and setter with default modifier declaration");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGeneratePrivateStaticGetterSetter"))
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateStaticGetterSetterWithDefaultModifier"))
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
                            new TestCaseData(ReadAllTextAS3("BeforeOverridePublicFunction"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextAS3("AfterOverridePublicFunction"))
                                .SetName("Override public function");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeOverrideProtectedFunction"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextAS3("AfterOverrideProtectedFunction"))
                                .SetName("Override proteced function");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeOverrideInternalFunction"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextAS3("AfterOverrideInternalFunction"))
                                .SetName("Override internal function");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeOverrideHasOwnProperty"), "Object", "hasOwnProperty", FlagType.Function)
                                .Returns(ReadAllTextAS3("AfterOverrideHasOwnProperty"))
                                .SetName("Override hasOwnProperty");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeOverridePublicGetSet"), "Foo", "foo", FlagType.Getter)
                                .Returns(ReadAllTextAS3("AfterOverridePublicGetSet"))
                                .SetName("Override public getter and setter");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeOverrideInternalGetSet"), "Foo", "foo", FlagType.Getter)
                                .Returns(ReadAllTextAS3("AfterOverrideInternalGetSet"))
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
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideGetNull"), "Foo", "foo", FlagType.Getter | FlagType.Setter)
                                .Returns(ReadAllTextHaxe("AfterOverrideGetNull"))
                                .SetName("Override var foo(get, null)");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideNullSet"), "Foo", "foo", FlagType.Getter | FlagType.Setter)
                                .Returns(ReadAllTextHaxe("AfterOverrideNullSet"))
                                .SetName("Override var foo(null, set)");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideGetSet"), "Foo", "foo", FlagType.Getter | FlagType.Setter)
                                .Returns(ReadAllTextHaxe("AfterOverrideGetSet"))
                                .SetName("Override var foo(get, set)");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideIssue793"), "Foo", "foo", FlagType.Getter | FlagType.Setter)
                                .Returns(ReadAllTextHaxe("AfterOverrideIssue793"))
                                .SetName("issue #793");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverridePublicFunction"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextHaxe("AfterOverridePublicFunction"))
                                .SetName("Override public function");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverridePrivateFunction"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextHaxe("AfterOverridePrivateFunction"))
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
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideGetNull"), "Foo", "foo", FlagType.Getter | FlagType.Setter)
                                .Returns(ReadAllTextHaxe("AfterOverrideGetNullWithDefaultModifier"))
                                .SetName("Override var foo(get, null) with default modifier declaration");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideNullSet"), "Foo", "foo", FlagType.Getter | FlagType.Setter)
                                .Returns(ReadAllTextHaxe("AfterOverrideNullSetWithDefaultModifier"))
                                .SetName("Override var foo(null, set) with default modifier declaration");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideGetSet"), "Foo", "foo", FlagType.Getter | FlagType.Setter)
                                .Returns(ReadAllTextHaxe("AfterOverrideGetSetWithDefaultModifier"))
                                .SetName("Override var foo(get, set) with default modifier declaration");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverridePrivateFunction"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextHaxe("AfterOverridePrivateFunctionWithDefaultModifier"))
                                .SetName("Override private function with default modifier");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, string ofClassName, string memberName, FlagType memberFlags)
                {
                    return GenerateOverride.GenerateHaxe(sourceText, ofClassName, memberName, memberFlags, sci);
                }
            }

            [TestFixture]
            public class GetStatementReturnType : GenerateJob
            {
                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfStringInitializer"))
                                .Returns(new ClassModel {Name = "String", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of \"\"");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfDigit"))
                                .Returns(new ClassModel {Name = "Number", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of 1");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfObjectInitializer"))
                                .Returns(new ClassModel {Name = "Object", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of {}");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfNewArray"))
                                .Returns(new ClassModel {Name = "Array", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of new Array()");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfArrayInitializer"))
                                .Returns(new ClassModel {Name = "Array", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of []");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfNewVector"))
                                .Returns(new ClassModel {Name = "Vector.<int>", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of new Vector.<int>()");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfVectorInitializer"))
                                .Returns(new ClassModel {Name = "Vector.<int>", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of new <int>[]");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfTwoDimensionalVectorInitializer"))
                                .Returns(new ClassModel {Name = "Vector.<Vector.<int>>", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of new new <Vector.<int>>[new <int>[0]]");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfItemOfVector"))
                                .Returns(new ClassModel {Name = "int", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of vector[0]");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfItemOfTwoDimensionalVector"))
                                .Returns(new ClassModel {Name = "int", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of vector[0][0]");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfItemOfMultidimensionalVector"))
                                .Returns(new ClassModel {Name = "int", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of vector[0][0][0][0]");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfArrayAccess"))
                                .Returns(new ClassModel {Name = "int", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of v[0][0].length");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public ClassModel AS3(string sourceText) => GetStatementReturnTypeAS3(sourceText, sci);

                public static ClassModel GetStatementReturnTypeAS3(string sourceText, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "as3";
                    ASContext.Context.SetAs3Features();
                    var currentModel = new FileModel {Context = ASContext.Context, Version = 3};
                    ASContext.Context.CurrentModel.Returns(currentModel);
                    var context = new AS3Context.Context(new AS3Settings());
                    BuildClassPath(context);
                    context.CurrentModel = currentModel;
                    return DoGetStatementReturnType(sourceText, context, sci);
                }

                public static ClassModel DoGetStatementReturnType(string sourceText, IASContext context, ScintillaControl sci)
                {
                    sci.Text = sourceText;
                    SnippetHelper.PostProcessSnippets(sci, 0);
                    var currentModel = ASContext.Context.CurrentModel;
                    new ASFileParser().ParseSrc(currentModel, sci.Text);
                    var currentClass = currentModel.Classes[0];
                    ASContext.Context.CurrentClass.Returns(currentClass);
                    ASContext.Context.CurrentMember.Returns(currentClass.Members[0]);
                    ASContext.Context.GetCodeModel(null).ReturnsForAnyArgs(it =>
                    {
                        var src = it[0] as string;
                        return string.IsNullOrEmpty(src) ? null : context.GetCodeModel(src);
                    });
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(it => context.ResolveType(it.ArgAt<string>(0), it.ArgAt<FileModel>(1)));
                    var visibleExternalElements = context.GetVisibleExternalElements();
                    ASContext.Context.GetVisibleExternalElements().Returns(visibleExternalElements);
                    var currentLine = sci.CurrentLine;
                    var returnType = ASGenerator.GetStatementReturnType(sci, ASContext.Context.CurrentClass, sci.GetLine(currentLine), sci.PositionFromLine(currentLine));
                    var result = returnType.resolve.Type;
                    return result;
                }
            }

            [TestFixture]
            public class ParseFunctionParameters : GenerateJob
            {
                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_String"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "String", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(\"string\")");
                        yield return
                            new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_Boolean"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Boolean", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(true)");
                        yield return
                            new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_Digit"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Number", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(1)");
                        yield return
                            new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_Array"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Array", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(new Array())");
                        yield return
                            new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_ArrayInitializer"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Array", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo([])");
                        yield return
                            new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_Object"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Object", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(new Object())");
                        yield return
                            new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_ObjectInitializer"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Object", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo({})");
                        yield return
                            new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_Vector"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Vector.<int>", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(new Vector.<int>())");
                        yield return
                            new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_VectorInitializer"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Vector.<int>", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(new <int>[])");
                        yield return
                            new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_TwoDimensionalVectorInitializer"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Vector.<Vector.<int>>", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(new <Vector.<int>>[new <int>[]])");
                        yield return
                            new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_MultidimensionalVectorInitializer"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Vector.<Vector.<Vector.<int>>>", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(new <Vector.<Vector.<int>>>[new <Vector.<int>[new <int>[]]])");
                        yield return
                            new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_ArrayAccess"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "int", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(v[0][0].length)");
                    }
                }

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_String"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "String", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(\"string\")");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_Boolean"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Bool", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(true)");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_Digit"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Float", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(1)");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_Array"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Array", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(new Array())");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_TypedArray"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Array<Int>", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(new Array<Int>())");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_ArrayInitializer"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Array<T>", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo([])");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_ObjectInitializer"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Dynamic", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo({})");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_ArrayAccess"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Int", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(a[0][0].length)");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_Function"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Function", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(function() {})");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public List<MemberModel> AS3(string sourceText) => ParseFunctionParametersAS3(sourceText, sci);

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public List<MemberModel> Haxe(string sourceText) => ParseFunctionParametersHaxe(sourceText, sci);

                internal static List<MemberModel> ParseFunctionParametersAS3(string sourceText, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "as3";
                    ASContext.Context.SetAs3Features();
                    var currentModel = new FileModel {Context = ASContext.Context, Version = 3};
                    ASContext.Context.CurrentModel.Returns(currentModel);
                    var context = new AS3Context.Context(new AS3Settings());
                    BuildClassPath(context);
                    context.CurrentModel = currentModel;
                    return DoParseFunctionParameters(sourceText, context, sci);
                }

                internal static List<MemberModel> ParseFunctionParametersHaxe(string sourceText, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "haxe";
                    ASContext.Context.SetHaxeFeatures();
                    var currentModel = new FileModel {Context = ASContext.Context, haXe = true, Version = 4};
                    ASContext.Context.CurrentModel.Returns(currentModel);
                    var context = new HaXeContext.Context(new HaXeSettings());
                    BuildClassPath(context);
                    context.CurrentModel = currentModel;
                    return DoParseFunctionParameters(sourceText, context, sci);
                }

                internal static List<MemberModel> DoParseFunctionParameters(string sourceText, IASContext context, ScintillaControl sci)
                {
                    sci.Text = sourceText;
                    SnippetHelper.PostProcessSnippets(sci, 0);
                    var currentModel = ASContext.Context.CurrentModel;
                    new ASFileParser().ParseSrc(currentModel, sci.Text);
                    var currentClass = currentModel.Classes[0];
                    ASContext.Context.CurrentClass.Returns(currentClass);
                    var currentMember = currentClass.Members[0];
                    ASContext.Context.CurrentMember.Returns(currentMember);
                    ASContext.Context.GetCodeModel(null).ReturnsForAnyArgs(it =>
                    {
                        var src = it[0] as string;
                        return string.IsNullOrEmpty(src) ? null : context.GetCodeModel(src);
                    });
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(it => context.ResolveType(it.ArgAt<string>(0), it.ArgAt<FileModel>(1)));
                    var visibleExternalElements = context.GetVisibleExternalElements();
                    ASContext.Context.GetVisibleExternalElements().Returns(visibleExternalElements);
                    var result = ASGenerator.ParseFunctionParameters(sci, sci.CurrentPos).Select(it => it.result.Type ?? it.result.Member).ToList();
                    return result;
                }
            }

            [TestFixture]
            public class ChangeConstructorDeclaration : GenerateJob
            {
                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeChangeConstructorDeclaration_String"))
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_String"))
                                .SetName("new Foo(\"\") -> function Foo(string:String)");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeChangeConstructorDeclaration_String2"))
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_String2"))
                                .SetName("new Foo(\"\", \"\") -> function Foo(string:String, string1:String)");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeChangeConstructorDeclaration_Digit"))
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_Digit"))
                                .SetName("new Foo(1) -> function Foo(number:Number)");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeChangeConstructorDeclaration_Boolean"))
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_Boolean"))
                                .SetName("new Foo(true) -> function Foo(boolean:Boolean)");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeChangeConstructorDeclaration_ObjectInitializer"))
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_ObjectInitializer"))
                                .SetName("new Foo({}) -> function Foo(object:Object)");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeChangeConstructorDeclaration_ArrayInitializer"))
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_ArrayInitializer"))
                                .SetName("new Foo([]) -> function Foo(array:Array)");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeChangeConstructorDeclaration_VectorInitializer"))
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_VectorInitializer"))
                                .SetName("new Foo(new <int>[]) -> function Foo(vector:Vector.<int>)");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeChangeConstructorDeclaration_TwoDimensionalVectorInitializer"))
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_TwoDimensionalVectorInitializer"))
                                .SetName("new Foo(new <Vector.<Vector.<int>>[new <int>[]]) -> function Foo(vector:Vector.<Vector.<int>>)");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeChangeConstructorDeclaration_ItemOfTwoDimensionalVectorInitializer"))
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_ItemOfTwoDimensionalVectorInitializer"))
                                .SetName("new Foo(strings[0][0]) -> function Foo(string:String)");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeChangeConstructorDeclaration_Function"))
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_Function"))
                                .SetName("new Foo(function():void {}) -> function Foo(function:Function)");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText) => GenerateAS3(sourceText, sci);

                public IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_String")
                                .Returns(ReadAllTextHaxe("AfterChangeConstructorDeclaration_String"))
                                .SetName("new Foo(\"\") -> function new(string:String)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_String2")
                                .Returns(ReadAllTextHaxe("AfterChangeConstructorDeclaration_String2"))
                                .SetName("new Foo(\"\", \"\") -> function new(string:String, string1:String)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_Digit")
                                .Returns(ReadAllTextHaxe("AfterChangeConstructorDeclaration_Digit"))
                                .SetName("new Foo(1) -> function new(float:Float)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_Boolean")
                                .Returns(ReadAllTextHaxe("AfterChangeConstructorDeclaration_Boolean"))
                                .SetName("new Foo(true) -> function new(bool:Bool)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_ItemOfTwoDimensionalArrayInitializer")
                                .Returns(ReadAllTextHaxe("AfterChangeConstructorDeclaration_ItemOfTwoDimensionalArrayInitializer"))
                                .SetName("new Foo(strings[0][0]) -> function new(string:String)");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string fileName) => GenerateHaxe(fileName, sci);

                internal string GenerateAS3(string sourceText, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "as3";
                    ASContext.Context.SetAs3Features();
                    var currentModel = new FileModel {Context = ASContext.Context, Version = 3};
                    ASContext.Context.CurrentModel.Returns(currentModel);
                    var context = new AS3Context.Context(new AS3Settings());
                    BuildClassPath(context);
                    context.CurrentModel = currentModel;
                    return Generate(sourceText, context, sci);
                }

                internal string GenerateHaxe(string fileName, ScintillaControl sci)
                {
                    var sourceText = ReadAllTextHaxe(fileName);
                    sci.ConfigurationLanguage = "haxe";
                    ASContext.Context.SetHaxeFeatures();
                    fileName = GetFullPathHaxe(fileName);
                    var currentModel = new FileModel
                    {
                        Context = ASContext.Context,
                        Version = 4,
                        haXe = true,
                        FileName = fileName
                    };
                    ASContext.Context.CurrentModel.Returns(currentModel);
                    PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
                    var context = new HaXeContext.Context(new HaXeSettings());
                    BuildClassPath(context);
                    context.CurrentModel = currentModel;
                    return Generate(sourceText, context, sci);
                }

                internal string Generate(string sourceText, IASContext context, ScintillaControl sci)
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
                    var visibleExternalElements = context.GetVisibleExternalElements();
                    ASContext.Context.GetVisibleExternalElements().Returns(visibleExternalElements);
                    ASContext.Context.GetCodeModel(null).ReturnsForAnyArgs(x =>
                    {
                        var src = x[0] as string;
                        return string.IsNullOrEmpty(src) ? null : context.GetCodeModel(src);
                    });
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(x => context.ResolveType(x.ArgAt<string>(0), x.ArgAt<FileModel>(1)));
                    ASGenerator.contextToken = sci.GetWordFromPosition(sci.CurrentPos);
                    ASGenerator.GenerateJob(GeneratorJobType.ChangeConstructorDecl, currentMember, ASContext.Context.CurrentClass, null, null);
                    return sci.Text;
                }
            }

            [TestFixture]
            public class GetStartOfStatement : GenerateJob
            {
                public IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(" new Vector.<int>()$(EntryPoint)")
                                .Returns(1);
                        yield return
                            new TestCaseData(" new <int>[]$(EntryPoint)")
                                .Returns(1);
                        yield return
                            new TestCaseData(" new <Object>[{}]$(EntryPoint)")
                                .Returns(1);
                        yield return
                            new TestCaseData(" new <Vector.<Object>>[new <Object>[{}]]$(EntryPoint)")
                                .Returns(1);
                        yield return
                            new TestCaseData(" new <Object>[{a:[new Number('10.0')]}]$(EntryPoint)")
                                .Returns(1);
                        yield return
                            new TestCaseData(" new Object()$(EntryPoint)")
                                .Returns(1);
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public int AS3(string sourceText) => GetStartOfStatementAS3(sourceText, sci);

                internal static int GetStartOfStatementAS3(string sourceText, ScintillaControl sci)
                {
                    sci.ConfigurationLanguage = "as3";
                    ASContext.Context.SetAs3Features();
                    return Common(sourceText, sci);
                }

                internal static int Common(string sourceText, ScintillaControl sci)
                {
                    var expr = new ASResult
                    {
                        Type = new ClassModel {Flags = FlagType.Class},
                        Context = new ASExpr {WordBefore = "new"}
                    };
                    sci.Text = sourceText;
                    SnippetHelper.PostProcessSnippets(sci, 0);
                    return ASGenerator.GetStartOfStatement(sci, sci.CurrentPos, expr);
                }
            }

            protected static void BuildClassPath(AS3Context.Context context)
            {
                context.BuildClassPath();
                var intrinsicPath = $"{PathHelper.LibraryDir}{Path.DirectorySeparatorChar}AS3{Path.DirectorySeparatorChar}intrinsic";
                context.Classpath.AddRange(Directory.GetDirectories(intrinsicPath).Select(it => new PathModel(it, context)));
                foreach (var it in context.Classpath)
                {
                    if (it.IsVirtual) context.ExploreVirtualPath(it);
                    else
                    {
                        var path = it.Path;
                        foreach (var searchPattern in context.GetExplorerMask())
                        {
                            foreach (var fileName in Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories))
                            {
                                it.AddFile(ASFileParser.ParseFile(new FileModel(fileName) {Context = context, Version = 3}));
                            }
                        }
                        context.RefreshContextCache(path);
                    }
                }
            }

            protected static void BuildClassPath(HaXeContext.Context context)
            {
                var platformsFile = Path.Combine("Settings", "Platforms");
                PlatformData.Load(Path.Combine(PathHelper.AppDir, platformsFile));
                PluginBase.CurrentProject = new HaxeProject("haxe") {CurrentSDK = Environment.GetEnvironmentVariable("HAXEPATH")};
                context.BuildClassPath();
                foreach (var it in context.Classpath)
                {
                    var path = it.Path;
                    foreach (var searchPattern in context.GetExplorerMask())
                    {
                        foreach (var fileName in Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories))
                        {
                            it.AddFile(ASFileParser.ParseFile(new FileModel(fileName) {Context = context, haXe = true, Version = 4}));
                        }
                    }
                    context.RefreshContextCache(path);
                }
            }
        }

        protected static string ReadAllTextAS3(string fileName)
        {
            return TestFile.ReadAllText($"ASCompletion.Test_Files.generated.as3.{fileName}.as");
        }

        protected static string ReadAllTextHaxe(string fileName) => TestFile.ReadAllText(GetFullPathHaxe(fileName));

        protected static string GetFullPathHaxe(string fileName) => $"ASCompletion.Test_Files.generated.haxe.{fileName}.hx";
    }
}