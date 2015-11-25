using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AS3Context;
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

//TODO: Sadly most of ASComplete is currently untestable in a proper way. Work on this branch solves it: https://github.com/Neverbirth/flashdevelop/tree/completionlist

namespace ASCompletion.Completion
{
    [TestFixture]
    public class ASCompleteTests
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

        // TODO: Add more tests!
        [Test]
        public void GetExpressionType_SimpleTest()
        {
            //TODO: Improve this test with more checks!
            var pluginMain = Substitute.For<PluginMain>();
            var pluginUiMock = new PluginUIMock(pluginMain);
            pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
            pluginMain.Settings.Returns(new GeneralSettings());
            pluginMain.Panel.Returns(pluginUiMock);
            ASContext.GlobalInit(pluginMain);
            ASContext.Context = Substitute.For<IASContext>();
            ASContext.Context.CurrentLine = 9;
            var asContext = new AS3Context.Context(new AS3Settings());
            ASContext.Context.Features.Returns(asContext.Features);
            ASContext.Context.GetCodeModel(null).ReturnsForAnyArgs(x => asContext.GetCodeModel(x.ArgAt<string>(0)));

            // Maybe we want to get the filemodel from ASFileParser even if we won't get a controlled environment?
            var member = new MemberModel("test1", "void", FlagType.Function, Visibility.Public)
            {
                LineFrom = 4,
                LineTo = 10,
                Parameters = new List<MemberModel>
                {
                    new MemberModel("arg1", "String", FlagType.ParameterVar, Visibility.Default),
                    new MemberModel("arg2", "Boolean", FlagType.ParameterVar, Visibility.Default) {Value = "false"}
                }
            };

            var classModel = new ClassModel();
            classModel.Name = "ASCompleteTest";
            classModel.Members.Add(member);

            var fileModel = new FileModel();
            fileModel.Classes.Add(classModel);

            classModel.InFile = fileModel;

            ASContext.Context.CurrentModel.Returns(fileModel);
            ASContext.Context.CurrentClass.Returns(classModel);
            ASContext.Context.CurrentMember.Returns(member);

            var sci = GetBaseScintillaControl();
            sci.Text = TestFile.ReadAllText("ASCompletion.Test_Files.completion.as3.SimpleTest.as");
            sci.ConfigurationLanguage = "as3";
            sci.Colourise(0, -1);

            var result = ASComplete.GetExpressionType(sci, 185);

            Assert.True(result.Context != null && result.Context.LocalVars != null);
            Assert.AreEqual(4, result.Context.LocalVars.Count);
        }
    }
}
