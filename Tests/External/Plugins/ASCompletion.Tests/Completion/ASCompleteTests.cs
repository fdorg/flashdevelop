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

        [TestFixture]
        public class DisambiguateComa : ASCompleteTests
        {
            public IEnumerable<TestCaseData> DisambiguateComaAS3TestCases
            {
                get
                {
                    yield return new TestCaseData("function test(arg:String, arg2").SetName("FunctionArgument").Returns(ComaExpression.FunctionDeclaration);
                    yield return new TestCaseData("function test(arg:").SetName("FunctionArgumentType").Returns(ComaExpression.FunctionDeclaration);
                    yield return new TestCaseData("var arr:Array = [1, 2").SetName("ArrayValue").Returns(ComaExpression.ArrayValue);
                    yield return new TestCaseData("var obj:Object = {test: 10").SetName("ObjectParameter").Returns(ComaExpression.AnonymousObjectParam);
                    yield return new TestCaseData("var obj:Object = {test").SetName("ObjectProperty").Returns(ComaExpression.AnonymousObjectParam);
                    yield return new TestCaseData("var obj:Obj").SetName("VariableType").Returns(ComaExpression.VarDeclaration);
                    yield return new TestCaseData("var obj:Obj, ").SetName("VariableMultiple").Returns(ComaExpression.VarDeclaration);
                    yield return new TestCaseData("this.call(true").SetName("FunctionCallSimple").Returns(ComaExpression.FunctionParameter);
                }
            }

            public IEnumerable<TestCaseData> DisambiguateComaHaxeTestCases
            {
                get
                {
                    yield return new TestCaseData("function test<K:(ArrayAccess, {})>(arg:").SetName("GenericFunctionArgumentType").SetDescription("This includes PR #963").Returns(ComaExpression.FunctionDeclaration);
                    yield return new TestCaseData("function test<K:(ArrayAccess, {})>(arg:String, arg2").SetName("GenericFunctionArgument").SetDescription("This includes PR #963").Returns(ComaExpression.FunctionDeclaration);
                    yield return new TestCaseData("class Generic<K,").SetName("GenericTypeParameterInClass").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                    yield return new TestCaseData("function generic<K,").SetName("GenericTypeParameterInFunction").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                    yield return new TestCaseData("class Generic<K:").SetName("GenericTypeParameterConstraintInClass").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                    yield return new TestCaseData("class Generic<K:(").SetName("GenericTypeParameterConstraintMultipleInClass").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                    yield return new TestCaseData("class Generic<K:({},").SetName("GenericTypeParameterConstraintMultipleInClassAfterFirst").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                    yield return new TestCaseData("function generic<K:").SetName("GenericTypeParameterConstraintInFunction").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                    yield return new TestCaseData("function generic<K:(").SetName("GenericTypeParameterConstraintMultipleInFunction").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                    yield return new TestCaseData("function generic<K:({},").SetName("GenericTypeParameterConstraintMultipleInFunctionAfterFirst").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                    yield return new TestCaseData("new Generic<Array<Int>,").SetName("GenericTypeParameterInDeclaration").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                    yield return new TestCaseData("com.test.Generic<Array<Int>,").SetName("GenericTypeParameterInDeclarationWithFullyQualifiedClass").Returns(ComaExpression.None).Ignore("Not supported at the moment");
                    yield return new TestCaseData("var p:{x:").SetName("HaxeAnonymousStructureParameterType").Returns(ComaExpression.VarDeclaration);
                    yield return new TestCaseData("var p:{?x:").SetName("HaxeAnonymousStructureOptionalParameterType").Returns(ComaExpression.VarDeclaration);
                    yield return new TestCaseData("function p(arg:{x:").SetName("HaxeAnonymousStructureParameterTypeAsFunctionArg").Returns(ComaExpression.VarDeclaration);
                    yield return new TestCaseData("function p(arg:{?x:").SetName("HaxeAnonymousStructureOptionalParameterTypeAsFunctionArg").Returns(ComaExpression.VarDeclaration);
                    yield return new TestCaseData("function p(?arg:{?x:").SetName("HaxeAnonymousStructureOptionalParameterTypeAsFunctionOptionalArg").Returns(ComaExpression.VarDeclaration);
                    yield return new TestCaseData("function test(arg:String, ?arg2").SetName("HaxeFunctionOptionalArgument").Returns(ComaExpression.FunctionDeclaration);
                    yield return new TestCaseData("function test(?arg:").SetName("HaxeFunctionOptionalArgumentType").Returns(ComaExpression.FunctionDeclaration);
                    yield return new TestCaseData("var a:String = (2 > 3) ?").SetName("HaxeTernaryOperatorTruePart").Returns(ComaExpression.AnonymousObject);
                    yield return new TestCaseData("var a:String = (2 > 3) ? 'Hah' :").SetName("HaxeTernaryOperatorFalsePart").Returns(ComaExpression.AnonymousObject);
                }
            }

            [TestFixtureSetUp]
            public void DisambiguateComaSetUp()
            {
                var pluginMain = Substitute.For<PluginMain>();
                var pluginUiMock = new PluginUIMock(pluginMain);
                pluginMain.MenuItems.Returns(new List<System.Windows.Forms.ToolStripItem>());
                pluginMain.Settings.Returns(new GeneralSettings());
                pluginMain.Panel.Returns(pluginUiMock);
                ASContext.GlobalInit(pluginMain);
            }

            [Test, TestCaseSource("DisambiguateComaAS3TestCases")]
            public ComaExpression AS3(string text)
            {
                ASContext.Context = new AS3Context.Context(new AS3Settings());

                var sci = GetBaseScintillaControl();
                sci.Text = text;
                sci.ConfigurationLanguage = "as3";

                var coma = ASComplete.DisambiguateComa(sci, text.Length, 0);

                return coma;
            }

            [Test, TestCaseSource("DisambiguateComaHaxeTestCases")]
            public ComaExpression Haxe(string text)
            {
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                var sci = GetBaseScintillaControl();
                sci.Text = text;
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, text.Length, 0);

                return coma;
            }
        }
    }
}
