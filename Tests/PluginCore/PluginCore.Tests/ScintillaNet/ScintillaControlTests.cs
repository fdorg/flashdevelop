using System.Collections.Generic;
using FlashDevelop;
using NSubstitute;
using NUnit.Framework;
using PluginCore.Helpers;
using ScintillaNet;
using ScintillaNet.Enums;

namespace PluginCore.ScintillaNet
{
    [TestFixture]
    internal class ScintillaControlTests
    {
#pragma warning disable CS0436 // Type conflicts with imported type
        MainForm mainForm;
#pragma warning restore CS0436 // Type conflicts with imported type
        ISettings settings;
        ITabbedDocument doc;
        ScintillaControl sci;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            settings = Substitute.For<ISettings>();
            settings.UseTabs = true;
            settings.IndentSize = 4;
            settings.SmartIndentType = SmartIndent.CPP;
            settings.TabIndents = true;
            settings.TabWidth = 4;
            doc = Substitute.For<ITabbedDocument>();
#pragma warning disable CS0436 // Type conflicts with imported type
            mainForm = new MainForm
#pragma warning restore CS0436 // Type conflicts with imported type
            {
                Settings = settings,
                CurrentDocument = doc,
                StandaloneMode = true
            };
            PluginBase.Initialize(mainForm);
            FlashDevelop.Managers.ScintillaManager.LoadConfiguration();
            sci = GetBaseScintillaControl();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            settings = null;
            doc = null;
            mainForm.Dispose();
            mainForm = null;
            sci = null;
        }

        ScintillaControl GetBaseScintillaControl()
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

        [TestFixture]
        class GetWordLeftTests : ScintillaControlTests
        {
            IEnumerable<TestCaseData> AS3TestCases
            {
                get
                {
                    yield return new TestCaseData(" word$(EntryPoint) ", false).Returns(string.Empty);
                    yield return new TestCaseData(" word $(EntryPoint) ", false).Returns(string.Empty);
                    yield return new TestCaseData(" wor$(EntryPoint)d ", false).Returns("word");
                    yield return new TestCaseData(" wo$(EntryPoint)rd ", false).Returns("wor");
                    yield return new TestCaseData(" $(EntryPoint)word ", false).Returns("w");
                    yield return new TestCaseData(" word$(EntryPoint) ", true).Returns("word");
                    yield return new TestCaseData(" word $(EntryPoint) ", true).Returns("word");
                    yield return new TestCaseData(" wor$(EntryPoint)d ", true).Returns("word");
                    yield return new TestCaseData(" wo$(EntryPoint)rd ", true).Returns("wor");
                    yield return new TestCaseData(" $(EntryPoint)word ", true).Returns("w");
                    yield return new TestCaseData(" word\n\n\r\t\t \t$(EntryPoint) ", true).Returns("word");
                }
            }

            [Test, TestCaseSource(nameof(AS3TestCases))]
            public string AS3(string sourceText, bool skipWS) => AS3Impl(sci, sourceText, skipWS);

            static string AS3Impl(ScintillaControl sci, string sourceText, bool skipWS)
            {
                sci.ConfigurationLanguage = "as3";
                return Common(sci, sourceText, skipWS);
            }

            static string Common(ScintillaControl sci, string sourceText, bool skipWS)
            {
                sci.Text = sourceText;
                SnippetHelper.PostProcessSnippets(sci, 0);
                return sci.GetWordLeft(sci.CurrentPos, skipWS);
            }
        }

        [TestFixture]
        class GetWordRightTests : ScintillaControlTests
        {
            IEnumerable<TestCaseData> AS3TestCases
            {
                get
                {
                    yield return new TestCaseData("$(EntryPoint) word ", false).Returns(string.Empty);
                    yield return new TestCaseData(" $(EntryPoint)word ", false).Returns("word");
                    yield return new TestCaseData(" wor$(EntryPoint)d ", false).Returns("d");
                    yield return new TestCaseData(" wo$(EntryPoint)rd ", false).Returns("rd");
                    yield return new TestCaseData(" word$(EntryPoint) ", false).Returns(string.Empty);
                    yield return new TestCaseData(" word $(EntryPoint)", false).Returns(string.Empty);
                    yield return new TestCaseData("$(EntryPoint) word ", true).Returns("word");
                    yield return new TestCaseData(" $(EntryPoint)word ", true).Returns("word");
                    yield return new TestCaseData(" wor$(EntryPoint)d ", true).Returns("d");
                    yield return new TestCaseData(" wo$(EntryPoint)rd ", true).Returns("rd");
                    yield return new TestCaseData(" word$(EntryPoint) ", true).Returns(string.Empty);
                    yield return new TestCaseData("$(EntryPoint)\n\n\r\t\t\tword", true).Returns("word");
                }
            }

            [Test, TestCaseSource(nameof(AS3TestCases))]
            public string AS3(string sourceText, bool skipWS) => AS3Impl(sci, sourceText, skipWS);

            static string AS3Impl(ScintillaControl sci, string sourceText, bool skipWS)
            {
                sci.ConfigurationLanguage = "as3";
                return Common(sci, sourceText, skipWS);
            }

            static string Common(ScintillaControl sci, string sourceText, bool skipWS)
            {
                sci.Text = sourceText;
                SnippetHelper.PostProcessSnippets(sci, 0);
                return sci.GetWordRight(sci.CurrentPos, skipWS);
            }
        }

        [TestFixture]
        class OnSmartIndent : ScintillaControlTests
        {
            IEnumerable<TestCaseData> CppTestCases
            {
                get
                {
                    yield return new TestCaseData("function test() {\n$(EntryPoint)\n}", '\n').Returns("function test() {\n\t\n}");
                    yield return new TestCaseData("function test() {\n\t\n$(EntryPoint)\n}", '\n').Returns("function test() {\n\t\n\t\n}");
                }
            }

            // Remark, OnSmartIndent happens after the char has been inserted
            [Test, TestCaseSource(nameof(CppTestCases))]
            public string Cpp(string sourceText, int ch)
            {
                sci.SmartIndentType = SmartIndent.CPP;
                sci.ConfigurationLanguage = "haxe";
                return Common(sci, sourceText, ch);
            }

            static string Common(ScintillaControl sci, string sourceText, int ch)
            {
                sci.Text = sourceText;
                sci.Colourise(0, -1);
                SnippetHelper.PostProcessSnippets(sci, 0);
                sci.OnSmartIndent(sci, ch);
                return sci.Text;
            }
        }
    }
}
