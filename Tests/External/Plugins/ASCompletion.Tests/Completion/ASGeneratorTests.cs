using System.Collections;
using System.Collections.Generic;
using ASCompletion.Context;
using ASCompletion.Settings;
using FlashDevelop;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using ScintillaNet;

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
            var sci = new ScintillaControl();
            sci.Encoding = System.Text.Encoding.UTF8;
            sci.CodePage = 65001;
            sci.Text = "function test():void{\r\n\t\t\t}";
            sci.ConfigurationLanguage = "haxe";
            sci.Colourise(0, -1);
            int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

            Assert.AreEqual(21, funcBodyStart);
        }

        [Test]
        public void GetBodyStart_EndOnSameDeclarationLine()
        {
            var sci = new ScintillaControl();
            sci.Encoding = System.Text.Encoding.UTF8;
            sci.CodePage = 65001;
            sci.Text = "function test():void{}";
            sci.ConfigurationLanguage = "haxe";
            sci.Colourise(0, -1);
            int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

            Assert.AreEqual(22, funcBodyStart);
        }

        [Test]
        public void GetBodyStart_EndOnSameLine()
        {
            var sci = new ScintillaControl();
            sci.Encoding = System.Text.Encoding.UTF8;
            sci.CodePage = 65001;
            sci.Text = "function test():void\r\n\r\n{}";
            sci.ConfigurationLanguage = "haxe";
            sci.Colourise(0, -1);
            int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

            Assert.AreEqual(22, funcBodyStart);
        }

        [Test]
        public void GetBodyStart_BracketInCommentsOrText()
        {
            var sci = new ScintillaControl();
            sci.Encoding = System.Text.Encoding.UTF8;
            sci.CodePage = 65001;
            sci.ConfigurationLanguage = "haxe";
            sci.Text = "function test(arg:String='{', arg2:String=\"{\"):void/*{*/\r\n{}";
            sci.Colourise(0, -1);
            int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

            Assert.AreEqual(59, funcBodyStart);
        }

        [Test]
        public void GetBodyStart_MultiByteCharacters()
        {
            var sci = new ScintillaControl();
            sci.Encoding = System.Text.Encoding.UTF8;
            sci.CodePage = 65001;
            sci.Text = "function test():void/*áéíóú*/\r\n{}";
            sci.ConfigurationLanguage = "haxe";
            sci.Colourise(0, -1);

            int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

            Assert.AreEqual(37, funcBodyStart);
        }

        [Test]
        [Ignore("Having only LineFrom and LineTo for members is not enough to handle these cases. FlashDevelop in general is not too kind when it comes to several members in the same line...")]
        public void GetBodyWithAnotherMemberInTheSameLine()
        {
            var sci = new ScintillaControl();
            sci.Encoding = System.Text.Encoding.UTF8;
            sci.CodePage = 65001;
            sci.Text = "function tricky():void {} function test():void{\r\n\t\t\t}";
            int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

            Assert.AreEqual(49, funcBodyStart);
        }


        [Test]
        public void GetBodyStart_BracketInGenericConstraint()
        {
            var sci = new ScintillaControl();
            sci.Encoding = System.Text.Encoding.UTF8;
            sci.CodePage = 65001;
            sci.Lexer = 3;
            sci.StyleBits = 7;
            sci.ConfigurationLanguage = "haxe";
            sci.Text = "function test<T:{}>(arg:T):void{\r\n}";
            sci.Colourise(0, -1);
            int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

            Assert.AreEqual(32, funcBodyStart);
        }

        [Test]
        public void GenerateJob_FieldFromParameterPublicScope()
        {
            var table = new Hashtable();
            table["scope"] = Model.Visibility.Public;

            var sci = new ScintillaControl();
            sci.Encoding = System.Text.Encoding.UTF8;
            sci.CodePage = 65001;
            sci.Text = "\tfunction test():void{\r\n\t\t\t}";
            doc.SciControl.Returns(sci);
            ASGenerator.GenerateJob(GeneratorJobType.FieldFromPatameter, null, null, null, table);
        }

        [Test]
        public void GenerateJob_ImplementFromInterface_FullAs3()
        {
            var interfaceModel = new Model.ClassModel { InFile = new Model.FileModel(), Name = "ITest" };
            var classModel = new Model.ClassModel {InFile = new Model.FileModel()};
            var pluginMain = Substitute.For<PluginMain>();
            pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
            pluginMain.Settings.Returns(new GeneralSettings());
            ASContext.GlobalInit(pluginMain);
            ASContext.Context = Substitute.For<IASContext>();
            ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(interfaceModel);
            ASContext.Context.Features.voidKey = "void";

            var sci = new ScintillaControl();
            sci.Encoding = System.Text.Encoding.UTF8;
            sci.CodePage = 65001;
            sci.Text = "package  test():void{\r\n\t\t\t}";
            sci.ConfigurationLanguage = "as3";
            doc.SciControl.Returns(sci);

            interfaceModel.Members.Add(new Model.MemberList
                                           {
                                               new Model.MemberModel("getter", "String", Model.FlagType.Getter, Model.Visibility.Default),
                                               new Model.MemberModel("setter", "void", Model.FlagType.Setter, Model.Visibility.Default),
                                               new Model.MemberModel("testMethod", "Number", Model.FlagType.Getter, Model.Visibility.Default)
                                           });

            ASGenerator.GenerateJob(GeneratorJobType.ImplementInterface, null, classModel, null, null);
        }
    }
}
