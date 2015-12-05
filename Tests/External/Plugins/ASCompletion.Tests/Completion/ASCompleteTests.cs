using System.Collections.Generic;
using AS3Context;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.Settings;
using ASCompletion.TestUtils;
using FlashDevelop;
using HaXeContext;
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
        public class GetExpressionType : ASCompleteTests
        {
            [Test]
            public void SimpleTest()
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

        public class DisambiguateComa : ASCompleteTests
        {
            [Test]
            public void FunctionArgument()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new AS3Context.Context(new AS3Settings());

                var sci = GetBaseScintillaControl();
                sci.Text = "function test(arg:String, arg2";
                sci.ConfigurationLanguage = "as3";

                var coma = ASComplete.DisambiguateComa(sci, 30, 0);

                Assert.AreEqual(ComaExpression.FunctionDeclaration, coma);
            }

            [Test]
            public void FunctionArgumentType()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new AS3Context.Context(new AS3Settings());

                var sci = GetBaseScintillaControl();
                sci.Text = "function test(arg:";
                sci.ConfigurationLanguage = "as3";

                var coma = ASComplete.DisambiguateComa(sci, 18, 0);

                Assert.AreEqual(ComaExpression.FunctionDeclaration, coma);
            }

            [Test(Description = "This includes PR #963")]
            public void GenericFunctionArgumentType()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new AS3Context.Context(new AS3Settings());

                var sci = GetBaseScintillaControl();
                sci.Text = "function test<K:(ArrayAccess, {})>(arg:";
                sci.ConfigurationLanguage = "as3";

                var coma = ASComplete.DisambiguateComa(sci, 39, 0);
                Assert.AreEqual(ComaExpression.FunctionDeclaration, coma);
            }

            [Test(Description = "This includes PR #963")]
            public void GenericFunctionArgument()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new AS3Context.Context(new AS3Settings());

                var sci = GetBaseScintillaControl();
                sci.Text = "function test<K:(ArrayAccess, {})>(arg:String, arg2";
                sci.ConfigurationLanguage = "as3";

                var coma = ASComplete.DisambiguateComa(sci, 51, 0);
                Assert.AreEqual(ComaExpression.FunctionDeclaration, coma);
            }

            [Test]
            public void ArrayValue()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new AS3Context.Context(new AS3Settings());

                var sci = GetBaseScintillaControl();
                sci.Text = "var arr:Array = [1, 2";
                sci.ConfigurationLanguage = "as3";

                var coma = ASComplete.DisambiguateComa(sci, 21, 0);

                Assert.AreEqual(ComaExpression.ArrayValue, coma);
            }

            [Test]
            public void ObjectParameter()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new AS3Context.Context(new AS3Settings());

                var sci = GetBaseScintillaControl();
                sci.Text = "var obj:Object = {test: 10";
                sci.ConfigurationLanguage = "as3";

                var coma = ASComplete.DisambiguateComa(sci, 26, 0);

                Assert.AreEqual(ComaExpression.AnonymousObjectParam, coma);
            }

            [Test]
            public void ObjectProperty()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new AS3Context.Context(new AS3Settings());

                var sci = GetBaseScintillaControl();
                sci.Text = "var obj:Object = {test";
                sci.ConfigurationLanguage = "as3";

                var coma = ASComplete.DisambiguateComa(sci, 22, 0);
                // Should this be AnonymousObject?
                Assert.AreEqual(ComaExpression.AnonymousObjectParam, coma);
            }

            [Test]
            public void VariableType()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new AS3Context.Context(new AS3Settings());

                var sci = GetBaseScintillaControl();
                sci.Text = "var obj:Obj";
                sci.ConfigurationLanguage = "as3";

                var coma = ASComplete.DisambiguateComa(sci, 11, 0);
                Assert.AreEqual(ComaExpression.VarDeclaration, coma);
            }

            [Test]
            public void VariableMultiple()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new AS3Context.Context(new AS3Settings());

                var sci = GetBaseScintillaControl();
                sci.Text = "var obj:Object, ";
                sci.ConfigurationLanguage = "as3";

                var coma = ASComplete.DisambiguateComa(sci, 20, 0);
                Assert.AreEqual(ComaExpression.VarDeclaration, coma);
            }

            [Test]
            public void FunctionCallSimple()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new AS3Context.Context(new AS3Settings());

                var sci = GetBaseScintillaControl();
                sci.Text = "this.call(true";
                sci.ConfigurationLanguage = "as3";

                var coma = ASComplete.DisambiguateComa(sci, 14, 0);
                Assert.AreEqual(ComaExpression.FunctionParameter, coma);
            }

            [Test(Description = "Not supported now")]
            public void GenericTypeParameterInClass()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "class Generic<K,";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 16, 0);
                Assert.AreNotEqual(ComaExpression.None, coma);
            }

            [Test(Description = "Not supported now")]
            public void GenericTypeParameterInFunction()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "function generic<K,";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 19, 0);
                Assert.AreNotEqual(ComaExpression.None, coma);
            }

            [Test(Description = "Not supported now")]
            public void GenericTypeParameterConstraintInClass()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "class Generic<K:";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 16, 0);
                Assert.AreNotEqual(ComaExpression.None, coma);
            }

            [Test(Description = "Not supported now")]
            public void GenericTypeParameterConstraintMultipleInClass()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "class Generic<K:(";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 17, 0);
                Assert.AreNotEqual(ComaExpression.None, coma);
            }

            [Test(Description = "Not supported now")]
            public void GenericTypeParameterConstraintMultipleInClassAfterFirst()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "class Generic<K:({},";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 20, 0);
                Assert.AreNotEqual(ComaExpression.None, coma);
            }

            [Test(Description = "Not supported now")]
            public void GenericTypeParameterConstraintInFunction()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "function generic<K:";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 19, 0);
                Assert.AreNotEqual(ComaExpression.None, coma);
            }

            [Test(Description = "Not supported now")]
            public void GenericTypeParameterConstraintMultipleInFunction()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "function generic<K:(";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 20, 0);
                Assert.AreNotEqual(ComaExpression.None, coma);
            }

            [Test(Description = "Not supported now")]
            public void GenericTypeParameterConstraintMultipleInFunctionAfterFirst()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "function generic<K:({},";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 23, 0);
                Assert.AreNotEqual(ComaExpression.None, coma);
            }

            [Test(Description = "Not supported now")]
            public void GenericTypeParameterInDeclaration()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "new Generic<Array<Int>,";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 23, 0);
                Assert.AreNotEqual(ComaExpression.None, coma);
            }

            [Test(Description = "Not supported now")]
            public void GenericTypeParameterInDeclarationWithFullyQualifiedClass()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "com.test.Generic<Array<Int>,";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 28, 0);
                Assert.AreNotEqual(ComaExpression.None, coma);
            }

            [Test]
            public void HaxeAnonymousStructureParameterType()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "var p:{x:";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 9, 0);

                Assert.AreEqual(ComaExpression.VarDeclaration, coma);
            }

            [Test]
            public void HaxeAnonymousStructureOptionalParameterType()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "var p:{?x:";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 10, 0);

                Assert.AreEqual(ComaExpression.VarDeclaration, coma);
            }

            [Test]
            public void HaxeAnonymousStructureParameterTypeAsFunctionArg()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "function p(arg:{x:";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 18, 0);

                Assert.AreEqual(ComaExpression.VarDeclaration, coma);
            }

            [Test]
            public void HaxeAnonymousStructureOptionalParameterTypeAsFunctionArg()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "function p(arg:{?x:";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 19, 0);

                Assert.AreEqual(ComaExpression.VarDeclaration, coma);
            }

            [Test]
            public void HaxeAnonymousStructureOptionalParameterTypeAsFunctionOptionalArg()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "function p(?arg:{?x:";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 20, 0);

                Assert.AreEqual(ComaExpression.VarDeclaration, coma);
            }

            [Test]
            public void HaxeFunctionOptionalArgument()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "function test(arg:String, ?arg2";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 31, 0);

                Assert.AreEqual(ComaExpression.FunctionDeclaration, coma);
            }

            [Test]
            public void HaxeFunctionOptionalArgumentType()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "function test(?arg:";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 19, 0);

                Assert.AreEqual(ComaExpression.FunctionDeclaration, coma);
            }

            [Test]
            public void HaxeTernaryOperatorTruePart()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "var a:String = (2 > 3) ?";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 28, 0);

                Assert.AreEqual(ComaExpression.AnonymousObject, coma);
            }

            [Test]
            public void HaxeTernaryOperatorFalsePart()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = "var a:String = (2 > 3) ? 'Hah' :";
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, 36, 0);

                Assert.AreEqual(ComaExpression.AnonymousObject, coma);
            }
        }
    }
}
