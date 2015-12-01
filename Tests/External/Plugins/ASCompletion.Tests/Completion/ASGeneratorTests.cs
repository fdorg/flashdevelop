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
            [Test]
            public void SimpleCase()
            {
                var sci = GetBaseScintillaControl();
                sci.Text = "function test():void{\r\n\t\t\t\r\n}";
                sci.ConfigurationLanguage = "haxe";
                sci.Colourise(0, -1);
                int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

                Assert.AreEqual(26, funcBodyStart);
            }

            [Test]
            public void EndOnSecondLine()
            {
                var sci = GetBaseScintillaControl();
                // TODO: Should we reindent second line?
                sci.Text = "function test():void{\r\n\t\t\t}";
                sci.ConfigurationLanguage = "haxe";
                sci.Colourise(0, -1);
                int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

                Assert.AreEqual(26, funcBodyStart);
                Assert.AreEqual("function test():void{\r\n\t\t\t\r\n}", sci.Text);
            }

            [Test]
            public void CharOnSecondLine()
            {
                var sci = GetBaseScintillaControl();
                // TODO: Should we reindent second line?
                sci.Text = "function test():void{\r\n\t\t\t//comment}";
                sci.ConfigurationLanguage = "haxe";
                sci.Colourise(0, -1);
                int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

                Assert.AreEqual(26, funcBodyStart);
                Assert.AreEqual("function test():void{\r\n\t\t\t//comment}", sci.Text);
            }

            [Test]
            public void EndOnSameDeclarationLine()
            {
                var sci = GetBaseScintillaControl();
                sci.Text = "function test():void{}";
                sci.ConfigurationLanguage = "haxe";
                sci.Colourise(0, -1);
                int funcBodyStart = ASGenerator.GetBodyStart(0, 0, sci);

                Assert.AreEqual(24, funcBodyStart);
                Assert.AreEqual("function test():void{\r\n\t\r\n}", sci.Text);
            }

            [Test]
            public void EndOnSameLine()
            {
                var sci = GetBaseScintillaControl();
                sci.Text = "function test():void\r\n\r\n{}\r\n";
                sci.ConfigurationLanguage = "as3";
                sci.Colourise(0, -1);
                int funcBodyStart = ASGenerator.GetBodyStart(0, 2, sci);

                Assert.AreEqual(28, funcBodyStart);
                Assert.AreEqual("function test():void\r\n\r\n{\r\n\t\r\n}\r\n", sci.Text);
            }

            [Test]
            public void TextOnStartLine()
            {
                var sci = GetBaseScintillaControl();
                sci.Text = "function test():void {trace(1);}";
                sci.ConfigurationLanguage = "as3";
                sci.Colourise(0, -1);
                int funcBodyStart = ASGenerator.GetBodyStart(0, 2, sci);

                Assert.AreEqual(25, funcBodyStart);
                Assert.AreEqual("function test():void {\r\n\ttrace(1);}", sci.Text);
            }

            [Test]
            public void BracketInCommentsOrText()
            {
                var sci = GetBaseScintillaControl();
                sci.ConfigurationLanguage = "haxe";
                sci.Text = "function test(arg:String='{', arg2:String=\"{\"):void/*{*/{\r\n}";
                sci.Colourise(0, -1);
                int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

                Assert.AreEqual(59, funcBodyStart);
            }

            [Test]
            public void MultiByteCharacters()
            {
                var sci = GetBaseScintillaControl();
                sci.Text = "function test():void/*áéíóú*/\r\n{}";
                sci.ConfigurationLanguage = "haxe";
                sci.Colourise(0, -1);

                int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

                Assert.AreEqual(40, funcBodyStart);
                Assert.AreEqual("function test():void/*áéíóú*/\r\n{\r\n\t\r\n}", sci.Text);
            }

            [Test]
            [Ignore("Having only LineFrom and LineTo for members is not enough to handle these cases. FlashDevelop in general is not too kind when it comes to several members in the same line, but we could change the method to use positions and try to get the proper position before.")]
            public void WithAnotherMemberInTheSameLine()
            {
                var sci = GetBaseScintillaControl();
                sci.Text = "function tricky():void {} function test():void{\r\n\t\t\t}";
                int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

                Assert.AreEqual(49, funcBodyStart);
            }


            [Test]
            public void BracketsInGenericConstraint()
            {
                var sci = GetBaseScintillaControl();
                sci.ConfigurationLanguage = "haxe";
                sci.Text = "function test<T:{}>(arg:T):void{\r\n\r\n}";
                sci.Colourise(0, -1);
                int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

                Assert.AreEqual(34, funcBodyStart);
                Assert.AreEqual("function test<T:{}>(arg:T):void{\r\n\r\n}", sci.Text);
            }
        }

        public class GenerateJob : ASGeneratorTests
        {
            private ClassModel GetAs3ImplementInterfaceModel()
            {
                var interfaceModel = new ClassModel { InFile = new FileModel(), Name = "ITest", Type = "ITest" };
                interfaceModel.Members.Add(new MemberList
                                           {
                                               new MemberModel("getter", "String", FlagType.Getter, Visibility.Public),
                                               new MemberModel("setter", "void", FlagType.Setter, Visibility.Public)
                                                   {
                                                        Parameters = new List<MemberModel> { new MemberModel("value", "String", FlagType.Variable, Visibility.Default) }
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

            [Test]
            [Ignore]
            public void FieldFromParameterPublicScope()
            {
                var table = new Hashtable();
                table["scope"] = Visibility.Public;

                var sci = GetBaseScintillaControl();
                sci.Text = "\tfunction test():void{\r\n\t\t\t}";
                doc.SciControl.Returns(sci);
                ASGenerator.GenerateJob(GeneratorJobType.FieldFromPatameter, null, null, null, table);
            }

            [Test]
            public void ImplementFromInterface_FullAs3()
            {
                var interfaceModel = GetAs3ImplementInterfaceModel();
                var classModel = new ClassModel { InFile = new FileModel(), LineFrom = 1, LineTo = 1 };
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = Substitute.For<IASContext>();
                ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(interfaceModel);
                ASContext.Context.Features.voidKey = "void";
                ASContext.Context.CurrentModel.Returns(new FileModel());

                var sci = GetBaseScintillaControl();
                sci.Text = "package generatortest {\r\n\tpublic class ImplementTest{}\r\n}";
                sci.ConfigurationLanguage = "as3";
                doc.SciControl.Returns(sci);

                ASGenerator.GenerateJob(GeneratorJobType.ImplementInterface, null, classModel, null, null);
                Assert.AreEqual(TestFile.ReadAllText("ASCompletion.Test_Files.generated.as3.ImplementInterfaceNoMembers.as"), sci.Text);
            }

            [Test]
            public void ImplementFromInterface_FullAs3WithPublicMemberBehindPrivate()
            {
                var interfaceModel = GetAs3ImplementInterfaceModel();
                var classModel = new ClassModel { InFile = new FileModel(), LineFrom = 1, LineTo = 10 };
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = Substitute.For<IASContext>();
                ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(interfaceModel);
                ASContext.Context.Features.voidKey = "void";
                ASContext.Context.CurrentModel.Returns(new FileModel());

                var sci = GetBaseScintillaControl();
                sci.Text = TestFile.ReadAllText("ASCompletion.Test_Files.generated.as3.BeforeImplementInterfacePublicMemberBehindPrivate.as");
                sci.ConfigurationLanguage = "as3";
                doc.SciControl.Returns(sci);

                classModel.Members.Add(new MemberList
                                           {
                                               new MemberModel("publicMember", "void", FlagType.Function, Visibility.Public)
                                                   {LineFrom = 3, LineTo = 5},
                                               new MemberModel("privateMember", "String", FlagType.Function, Visibility.Private)
                                                   {LineFrom = 7, LineTo = 9}
                                           });

                ASGenerator.GenerateJob(GeneratorJobType.ImplementInterface, null, classModel, null, null);
                Assert.AreEqual(TestFile.ReadAllText("ASCompletion.Test_Files.generated.as3.ImplementInterfacePublicMemberBehindPrivate.as"), sci.Text);
            }

            [Test]
            public void ImplementFromInterface_FullAs3WithoutPublicMember()
            {
                var interfaceModel = GetAs3ImplementInterfaceModel();
                var classModel = new ClassModel { InFile = new FileModel(), LineFrom = 1, LineTo = 10 };
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = Substitute.For<IASContext>();
                ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(interfaceModel);
                ASContext.Context.Features.voidKey = "void";
                ASContext.Context.CurrentModel.Returns(new FileModel());

                var sci = GetBaseScintillaControl();
                sci.Text = TestFile.ReadAllText("ASCompletion.Test_Files.generated.as3.BeforeImplementInterfaceNoPublicMember.as");
                sci.ConfigurationLanguage = "as3";
                doc.SciControl.Returns(sci);

                classModel.Members.Add(new MemberModel("privateMember", "String", FlagType.Function, Visibility.Private)
                {
                    LineFrom = 3, LineTo = 5
                });

                ASGenerator.GenerateJob(GeneratorJobType.ImplementInterface, null, classModel, null, null);
                Assert.AreEqual(TestFile.ReadAllText("ASCompletion.Test_Files.generated.as3.ImplementInterfaceNoPublicMember.as"), sci.Text);
            }

            [Test]
            public void ImplementFromInterface_FullHaxe()
            {
                var interfaceModel = GetHaxeImplementInterfaceModel();
                var classModel = new ClassModel { InFile = new FileModel(), LineFrom = 2, LineTo = 2 };
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = Substitute.For<IASContext>();
                ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(interfaceModel);
                ASContext.Context.Features.voidKey = "Void";
                ASContext.Context.CurrentModel.Returns(new FileModel {haXe = true});

                var sci = GetBaseScintillaControl();
                sci.Text = "package generatortest;\r\n\r\nclass ImplementTest{}";
                sci.ConfigurationLanguage = "haxe";
                doc.SciControl.Returns(sci);

                ASGenerator.GenerateJob(GeneratorJobType.ImplementInterface, null, classModel, null, null);
                Assert.AreEqual(TestFile.ReadAllText("ASCompletion.Test_Files.generated.haxe.ImplementInterfaceNoMembers.hx"), sci.Text);
            }

            [Test]
            public void ImplementFromInterface_SinglePropertyHaxe()
            {
                var interfaceModel = new ClassModel { InFile = new FileModel(), Name = "ITest", Type = "ITest" };
                interfaceModel.Members.Add(new MemberList
                                           {
                                               new MemberModel("x", "Int", FlagType.Getter, Visibility.Public)
                                               {
                                                   Parameters = new List<MemberModel>
                                                   {
                                                       new MemberModel {Name = "get"},
                                                       new MemberModel {Name = "set"}
                                                   }
                                               }
                                           });

                var classModel = new ClassModel { InFile = new FileModel(), LineFrom = 2, LineTo = 2 };
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = Substitute.For<IASContext>();
                ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(interfaceModel);
                ASContext.Context.Features.voidKey = "Void";
                ASContext.Context.CurrentModel.Returns(new FileModel { haXe = true });

                var sci = GetBaseScintillaControl();
                sci.Text = "package generatortest;\r\n\r\nclass ImplementTest{}";
                sci.ConfigurationLanguage = "haxe";
                doc.SciControl.Returns(sci);

                ASGenerator.GenerateJob(GeneratorJobType.ImplementInterface, null, classModel, null, null);
                Assert.AreEqual(TestFile.ReadAllText("ASCompletion.Test_Files.generated.haxe.ImplementInterfaceNoMembersInsertSingleProperty.hx"), sci.Text);
            }
        }
    }
}
