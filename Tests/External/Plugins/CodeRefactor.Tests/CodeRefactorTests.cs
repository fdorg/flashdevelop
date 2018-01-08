using System.Collections.Generic;
using ASCompletion.Context;
using ASCompletion.Settings;
using FlashDevelop;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using ScintillaNet;
using ScintillaNet.Enums;

namespace CodeRefactor
{
    public class CodeRefactorTests
    {
#pragma warning disable CS0436 // Type conflicts with imported type
        protected MainForm MainForm;
#pragma warning restore CS0436 // Type conflicts with imported type
        protected ISettings Settings;
        protected ITabbedDocument Doc;
        protected ScintillaControl Sci;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
#pragma warning disable CS0436 // Type conflicts with imported type
            MainForm = new MainForm();
#pragma warning restore CS0436 // Type conflicts with imported type
            Settings = Substitute.For<ISettings>();
            Settings.UseTabs = true;
            Settings.IndentSize = 4;
            Settings.SmartIndentType = SmartIndent.CPP;
            Settings.TabIndents = true;
            Settings.TabWidth = 4;
            Doc = Substitute.For<ITabbedDocument>();
            MainForm.Settings = Settings;
            MainForm.CurrentDocument = Doc;
            MainForm.Documents = new[] {Doc};
            MainForm.StandaloneMode = true;
            PluginBase.Initialize(MainForm);
            FlashDevelop.Managers.ScintillaManager.LoadConfiguration();
            var pluginMain = Substitute.For<ASCompletion.PluginMain>();
            var pluginUiMock = new PluginUIMock(pluginMain);
            pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
            pluginMain.Settings.Returns(new GeneralSettings());
            pluginMain.Panel.Returns(pluginUiMock);
            ASContext.GlobalInit(pluginMain);
            ASContext.Context = Substitute.For<IASContext>();
            Sci = GetBaseScintillaControl();
            Doc.SciControl.Returns(Sci);
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            Settings = null;
            Doc = null;
            Sci = null;
            MainForm.Dispose();
            MainForm = null;
        }

        protected ScintillaControl GetBaseScintillaControl()
        {
            return new ScintillaControl
            {
                Encoding = System.Text.Encoding.UTF8,
                CodePage = 65001,
                Indent = Settings.IndentSize,
                Lexer = 3,
                StyleBits = 7,
                IsTabIndents = Settings.TabIndents,
                IsUseTabs = Settings.UseTabs,
                TabWidth = Settings.TabWidth
            };
        }
    }
}