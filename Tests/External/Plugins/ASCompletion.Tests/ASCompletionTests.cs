using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.Settings;
using ASCompletion.TestUtils;
using FlashDevelop;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using PluginCore.Helpers;
using ScintillaNet;
using ScintillaNet.Enums;

namespace ASCompletion
{
    public class ASCompletionTests
    {
#pragma warning disable CS0436 // Type conflicts with imported type
        private MainForm mainForm;
#pragma warning restore CS0436 // Type conflicts with imported type
        private ISettings settings;
        private ITabbedDocument doc;
        protected ScintillaControl sci;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
#pragma warning disable CS0436 // Type conflicts with imported type
            mainForm = new MainForm();
#pragma warning restore CS0436 // Type conflicts with imported type
            settings = Substitute.For<ISettings>();
            settings.UseTabs = true;
            settings.IndentSize = 4;
            settings.SmartIndentType = SmartIndent.CPP;
            settings.TabIndents = true;
            settings.TabWidth = 4;
            settings.DefaultFont.Returns(SystemFonts.DefaultFont);
            doc = Substitute.For<ITabbedDocument>();
            mainForm.Settings = settings;
            mainForm.CurrentDocument = doc;
            mainForm.Documents = new[] {doc};
            mainForm.StandaloneMode = true;
            PluginBase.Initialize(mainForm);
            FlashDevelop.Managers.ScintillaManager.LoadConfiguration();

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

        protected static void SetAs3Features(ScintillaControl sci)
        {
            if (sci.ConfigurationLanguage != "as3")
            {
                sci.ConfigurationLanguage = "as3";
                ASContext.Context.SetAs3Features();
            }
        }

        protected static void SetHaxeFeatures(ScintillaControl sci)
        {
            if (sci.ConfigurationLanguage != "haxe")
            {
                sci.ConfigurationLanguage = "haxe";
                ASContext.Context.SetHaxeFeatures();
            }
        }

        protected static void SetSrc(ScintillaControl sci, string sourceText)
        {
            sci.Text = sourceText;
            SnippetHelper.PostProcessSnippets(sci, 0);
            sci.Colourise(0, -1);
            var currentModel = ASContext.Context.CurrentModel;
            ASContext.Context.GetCodeModel(currentModel, sci.Text);
            var line = sci.CurrentLine;
            var currentClass = currentModel.Classes.FirstOrDefault(line);
            if (currentClass == null && currentModel.Classes.Count > 0) currentClass = currentModel.Classes[0];
            ASContext.Context.CurrentClass.Returns(currentClass);
            var currentMember = currentClass?.Members.FirstOrDefault(line);
            ASContext.Context.CurrentMember.Returns(currentMember);
            ASGenerator.contextToken = sci.GetWordFromPosition(sci.CurrentPos);
        }
    }
}

public static class CollectionExtensions
{
    public static MemberModel FirstOrDefault(this MemberList list, int line) => list.Items.FirstOrDefault(line);

    public static TSource FirstOrDefault<TSource>(this ICollection<TSource> items, int line) where TSource : MemberModel
    {
        return items.FirstOrDefault(it => it.LineFrom <= line && it.LineTo >= line);
    }
}
