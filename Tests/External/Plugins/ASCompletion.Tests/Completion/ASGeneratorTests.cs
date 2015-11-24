using System.Collections;
using System.Collections.Generic;
using ASCompletion.Context;
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

        [Test]
        public void GetBodyStart_SimpleCase()
        {
            var sci = GetBaseScintillaControl();
            sci.Text = "function test():void{\r\n\t\t\t\r\n}";
            sci.ConfigurationLanguage = "haxe";
            sci.Colourise(0, -1);
            int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

            Assert.AreEqual(26, funcBodyStart);
        }

        [Test]
        public void GetBodyStart_EndOnSecondLine()
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
        public void GetBodyStart_CharOnSecondLine()
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
        public void GetBodyStart_EndOnSameDeclarationLine()
        {
            var sci = GetBaseScintillaControl();
            sci.Text = "function test():void{}";
            sci.ConfigurationLanguage = "haxe";
            sci.Colourise(0, -1);
            int funcBodyStart = ASGenerator.GetBodyStart(0, 0, sci);

            Assert.AreEqual(27, funcBodyStart);
            Assert.AreEqual("function test():void{\r\n    \r\n}", sci.Text);
        }

        [Test]
        public void GetBodyStart_EndOnSameLine()
        {
            var sci = GetBaseScintillaControl();
            sci.Text = "function test():void\r\n\r\n{}\r\n";
            sci.ConfigurationLanguage = "as3";
            sci.Colourise(0, -1);
            int funcBodyStart = ASGenerator.GetBodyStart(0, 2, sci);

            Assert.AreEqual(31, funcBodyStart);
            Assert.AreEqual("function test():void\r\n\r\n{\r\n    \r\n}\r\n", sci.Text);
        }

        [Test]
        public void GetBodyStart_TextOnStartLine()
        {
            var sci = GetBaseScintillaControl();
            sci.Text = "function test():void {trace(1);}";
            sci.ConfigurationLanguage = "as3";
            sci.Colourise(0, -1);
            int funcBodyStart = ASGenerator.GetBodyStart(0, 2, sci);

            Assert.AreEqual(28, funcBodyStart);
            Assert.AreEqual("function test():void {\r\n    trace(1);}", sci.Text);
        }

        [Test]
        public void GetBodyStart_BracketInCommentsOrText()
        {
            var sci = GetBaseScintillaControl();
            sci.ConfigurationLanguage = "haxe";
            sci.Text = "function test(arg:String='{', arg2:String=\"{\"):void/*{*/{\r\n}";
            sci.Colourise(0, -1);
            int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

            Assert.AreEqual(59, funcBodyStart);
        }

        [Test]
        public void GetBodyStart_MultiByteCharacters()
        {
            var sci = GetBaseScintillaControl();
            sci.Text = "function test():void/*áéíóú*/\r\n{}";
            sci.ConfigurationLanguage = "haxe";
            sci.Colourise(0, -1);

            int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

            Assert.AreEqual(43, funcBodyStart);
            Assert.AreEqual("function test():void/*áéíóú*/\r\n{\r\n    \r\n}", sci.Text);
        }

        [Test]
        [Ignore("Having only LineFrom and LineTo for members is not enough to handle these cases. FlashDevelop in general is not too kind when it comes to several members in the same line, but we could change the method to use positions and try to get the proper position before.")]
        public void GetBodyWithAnotherMemberInTheSameLine()
        {
            var sci = GetBaseScintillaControl();
            sci.Text = "function tricky():void {} function test():void{\r\n\t\t\t}";
            int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

            Assert.AreEqual(49, funcBodyStart);
        }


        [Test]
        public void GetBodyStart_BracketsInGenericConstraint()
        {
            var sci = GetBaseScintillaControl();
            sci.ConfigurationLanguage = "haxe";
            sci.Text = "function test<T:{}>(arg:T):void{\r\n\r\n}";
            sci.Colourise(0, -1);
            int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

            Assert.AreEqual(34, funcBodyStart);
            Assert.AreEqual("function test<T:{}>(arg:T):void{\r\n\r\n}", sci.Text);
        }

        [Test]
        public void GenerateJob_FieldFromParameterPublicScope()
        {
            var table = new Hashtable();
            table["scope"] = Model.Visibility.Public;

            var sci = GetBaseScintillaControl();
            sci.Text = "\tfunction test():void{\r\n\t\t\t}";
            doc.SciControl.Returns(sci);
            ASGenerator.GenerateJob(GeneratorJobType.FieldFromPatameter, null, null, null, table);
        }

        // TODO: Tests with different formatting options
        [Test]
        public void GenerateJob_ImplementFromInterface_FullAs3()
        {
            var interfaceModel = new Model.ClassModel { InFile = new Model.FileModel(), Name = "ITest", Type = "ITest"};
            var classModel = new Model.ClassModel {InFile = new Model.FileModel(), LineFrom = 1, LineTo = 1};
            var pluginMain = Substitute.For<PluginMain>();
            var pluginUiMock = new PluginUIMock(pluginMain);
            pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
            pluginMain.Settings.Returns(new GeneralSettings());
            pluginMain.Panel.Returns(pluginUiMock);
            ASContext.GlobalInit(pluginMain);
            ASContext.Context = Substitute.For<IASContext>();
            ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(interfaceModel);
            ASContext.Context.Features.voidKey = "void";

            var sci = GetBaseScintillaControl();
            sci.Text = "package generatortest {\r\n\tpublic class ImplementTest{}\r\n}";
            sci.ConfigurationLanguage = "as3";
            doc.SciControl.Returns(sci);

            interfaceModel.Members.Add(new Model.MemberList
                                           {
                                               new Model.MemberModel("getter", "String", Model.FlagType.Getter, Model.Visibility.Public),
                                               new Model.MemberModel("setter", "void", Model.FlagType.Setter, Model.Visibility.Public)
                                                   {
                                                        Parameters = new List<Model.MemberModel> { new Model.MemberModel("value", "String", Model.FlagType.Variable, Model.Visibility.Default) }
                                                   },
                                               new Model.MemberModel("testMethod", "Number", Model.FlagType.Function, Model.Visibility.Public),
                                               new Model.MemberModel("testMethodArgs", "int", Model.FlagType.Function, Model.Visibility.Public)
                                                   {
                                                        Parameters = new List<Model.MemberModel>
                                                        {
                                                            new Model.MemberModel("arg", "Number", Model.FlagType.Variable, Model.Visibility.Default),
                                                            new Model.MemberModel("arg2", "Boolean", Model.FlagType.Variable, Model.Visibility.Default)
                                                        }
                                                   }
                                           });

            ASGenerator.GenerateJob(GeneratorJobType.ImplementInterface, null, classModel, null, null);
            Assert.AreEqual(TestFile.ReadAllText("ASCompletion.Test_Files.generated.as3.ImplementInterfaceNoMembers.as"), sci.Text);
        }

        [Test]
        public void GenerateJob_ImplementFromInterface_FullAs3WithPublicMemberBehindPrivate()
        {
            var interfaceModel = new Model.ClassModel { InFile = new Model.FileModel(), Name = "ITest", Type = "ITest" };
            var classModel = new Model.ClassModel { InFile = new Model.FileModel(), LineFrom = 1, LineTo = 10 };
            var pluginMain = Substitute.For<PluginMain>();
            var pluginUiMock = new PluginUIMock(pluginMain);
            pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
            pluginMain.Settings.Returns(new GeneralSettings());
            pluginMain.Panel.Returns(pluginUiMock);
            ASContext.GlobalInit(pluginMain);
            ASContext.Context = Substitute.For<IASContext>();
            ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(interfaceModel);
            ASContext.Context.Features.voidKey = "void";

            var sci = GetBaseScintillaControl();
            sci.Text = TestFile.ReadAllText("ASCompletion.Test_Files.generated.as3.BeforeImplementInterfacePublicMemberBehindPrivate.as");
            sci.ConfigurationLanguage = "as3";
            doc.SciControl.Returns(sci);

            classModel.Members.Add(new Model.MemberList
                                           {
                                               new Model.MemberModel("publicMember", "void", Model.FlagType.Function, Model.Visibility.Public)
                                                   {LineFrom = 3, LineTo = 5},
                                               new Model.MemberModel("privateMember", "String", Model.FlagType.Function, Model.Visibility.Private)
                                                   {LineFrom = 7, LineTo = 9}
                                           });

            interfaceModel.Members.Add(new Model.MemberList
                                           {
                                               new Model.MemberModel("getter", "String", Model.FlagType.Getter, Model.Visibility.Public),
                                               new Model.MemberModel("setter", "void", Model.FlagType.Setter, Model.Visibility.Public)
                                                   {
                                                        Parameters = new List<Model.MemberModel> { new Model.MemberModel("value", "String", Model.FlagType.Variable, Model.Visibility.Default) }
                                                   },
                                               new Model.MemberModel("testMethod", "Number", Model.FlagType.Function, Model.Visibility.Public),
                                               new Model.MemberModel("testMethodArgs", "int", Model.FlagType.Function, Model.Visibility.Public)
                                                   {
                                                        Parameters = new List<Model.MemberModel>
                                                        {
                                                            new Model.MemberModel("arg", "Number", Model.FlagType.Variable, Model.Visibility.Default),
                                                            new Model.MemberModel("arg2", "Boolean", Model.FlagType.Variable, Model.Visibility.Default)
                                                        }
                                                   }
                                           });

            ASGenerator.GenerateJob(GeneratorJobType.ImplementInterface, null, classModel, null, null);
            Assert.AreEqual(TestFile.ReadAllText("ASCompletion.Test_Files.generated.as3.ImplementInterfacePublicMemberBehindPrivate.as"), sci.Text);
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

    }
}
