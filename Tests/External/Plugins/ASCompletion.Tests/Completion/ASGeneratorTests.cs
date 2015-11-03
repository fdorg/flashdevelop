using System.Collections;
using System.Collections.Generic;
using ASCompletion.Context;
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
            int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

            Assert.AreEqual(22, funcBodyStart);
        }

        [Test]
        public void GetBodyStart_EndOnSameDeclarationLine()
        {
            var sci = new ScintillaControl();
            sci.Encoding = System.Text.Encoding.UTF8;
            sci.CodePage = 65001;
            sci.Text = "function test():void{}";
            int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

            Assert.AreEqual(22, funcBodyStart);
        }

        [Test]
        public void GetBodyStart_EndOnSameLine()
        {
            var sci = new ScintillaControl();
            sci.Encoding = System.Text.Encoding.UTF8;
            sci.CodePage = 65001;
            sci.Text = "function test():void\r\n{}";
            int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

            Assert.AreEqual(22, funcBodyStart);
        }

        [Test]
        public void GetBodyStart_BracketInCommentsOrText()
        {
            var sci = new ScintillaControl();
            sci.Encoding = System.Text.Encoding.UTF8;
            sci.CodePage = 65001;
            sci.Lexer = 3;
            sci.StyleBits = 7;
            FlashDevelop.Managers.ScintillaManager.LoadConfiguration();
            sci.ConfigurationLanguage = "as3";
            sci.Colourise(0, -1);
            sci.Text = "function test(arg:String='{', arg2:String=\"{\"):void/*{*/\r\n{}";
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
            int funcBodyStart = ASGenerator.GetBodyStart(0, 1, sci);

            Assert.AreEqual(37, funcBodyStart);
        }

        [Test]
        [Ignore("Having only LineFrom and LineTo for members is not enough to handle these cases")]
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
        public void GenerateJob_ImplementFromInterface()
        {
            var interfaceModel = new Model.ClassModel();
            var classModel = new Model.ClassModel();
            var pluginMain = Substitute.For<PluginMain>();
            pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
            ASContext.GlobalInit(pluginMain);
            ASContext.Context = Substitute.For<IASContext>();
            ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(interfaceModel); 
            var table = new Hashtable();
            table["scope"] = Model.Visibility.Public;

            var sci = new ScintillaControl();
            sci.Encoding = System.Text.Encoding.UTF8;
            sci.CodePage = 65001;
            sci.Text = "\tfunction test():void{\r\n\t\t\t}";
            doc.SciControl.Returns(sci);
            ASGenerator.GenerateJob(GeneratorJobType.ImplementInterface, null, classModel, null, table);
        }
    }
}
