// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
using PluginCore.Helpers;
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
        protected ScintillaControl sci;

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

        // TODO: Add more tests!
        public class GetExpressionType : ASCompleteTests
        {
            [Test]
            public void SimpleTest()
            {
                //TODO: Improve this test with more checks!
                ASContext.Context.CurrentLine = 9;
                ASContext.Context.SetAs3Features();

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

                sci.Text = ReadAllTextAS3("SimpleTest");
                sci.ConfigurationLanguage = "as3";
                sci.Colourise(0, -1);

                var result = ASComplete.GetExpressionType(sci, 185);

                Assert.True(result.Context != null && result.Context.LocalVars != null);
                Assert.AreEqual(4, result.Context.LocalVars.Count);
            }

            public IEnumerable<TestCaseData> AS3TestCases
            {
                get
                {
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionTypeOfConstructor"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Function | FlagType.Constructor,
                                Name = "Foo"
                            })
                            .SetName("Get Expression Type of constructor");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionTypeOfConstructorParameter"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Variable | FlagType.ParameterVar,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of constructor parameter");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionTypeOfFunction"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Dynamic | FlagType.Function,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of function");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionTypeOfFunctionParameter"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Variable | FlagType.ParameterVar,
                                Name = "bar"
                            })
                            .SetName("Get Expression Type of function parameter");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionTypeOfFunctionParameter2"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Variable | FlagType.ParameterVar,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of function parameter");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionTypeOfFunctionWithParameter"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Dynamic | FlagType.Function,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of function with parameter");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionTypeOfLocalObjectKey"))
                            .Returns(null)
                            .SetName("Get Expression Type of local dynamic object key");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionTypeOfLocalObjectValue"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Dynamic | FlagType.Function,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of local dynamic object value");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionTypeOfLocalVar"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Dynamic | FlagType.Variable | FlagType.LocalVar,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of local var");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionTypeOfSetterParameter"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Variable | FlagType.ParameterVar,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of setter parameter");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionTypeOfVariable"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Dynamic | FlagType.Variable,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of variable");
                }
            }

            [Test, TestCaseSource(nameof(AS3TestCases))]
            public MemberModel AS3(string sourceText) => AS3Impl(sourceText, sci);

            public IEnumerable<TestCaseData> HaxeTestCases
            {
                get {
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionTypeOfConstructor"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Function | FlagType.Constructor,
                                Name = "Foo"
                            })
                            .SetName("Get Expression Type of constructor");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionTypeOfConstructorParameter"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Variable | FlagType.ParameterVar,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of constructor parameter");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionTypeOfFunction"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Dynamic | FlagType.Function,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of function");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionTypeOfFunctionParameter"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Variable | FlagType.ParameterVar,
                                Name = "bar"
                            })
                            .SetName("Get Expression Type of function parameter");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionTypeOfFunctionParameter2"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Variable | FlagType.ParameterVar,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of function parameter");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionTypeOfFunctionWithParameter"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Dynamic | FlagType.Function,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of function with parameter");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionTypeOfLocalDynamicKey"))
                            .Returns(null)
                            .SetName("Get Expression Type of local dynamic object key");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionTypeOfLocalDynamicValue"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Dynamic | FlagType.Function,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of local dynamic object value");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionTypeOfLocalVar"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Dynamic | FlagType.Variable | FlagType.LocalVar,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of local var");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionTypeOfPropertyGetter"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Dynamic | FlagType.Function,
                                Name = "get_foo"
                            })
                            .SetName("Get Expression Type of property getter");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionTypeOfPropertySetter"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Dynamic | FlagType.Function,
                                Name = "set_foo"
                            })
                            .SetName("Get Expression Type of property setter");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionTypeOfVariable"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Dynamic | FlagType.Variable,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of variable");
                }
            }

            [Test, TestCaseSource(nameof(HaxeTestCases))]
            public MemberModel Haxe(string sourceText) => HaxeImpl(sourceText, sci);

            internal static MemberModel AS3Impl(string sourceText, ScintillaControl sci)
            {
                sci.ConfigurationLanguage = "as3";
                ASContext.Context.SetAs3Features();
                return Common(sourceText, sci);
            }

            internal static MemberModel HaxeImpl(string sourceText, ScintillaControl sci)
            {
                sci.ConfigurationLanguage = "haxe";
                ASContext.Context.SetHaxeFeatures();
                return Common(sourceText, sci);
            }

            internal static MemberModel Common(string sourceText, ScintillaControl sci)
            {
                sci.Text = sourceText;
                SnippetHelper.PostProcessSnippets(sci, 0);
                var currentModel = ASContext.Context.CurrentModel;
                new ASFileParser().ParseSrc(currentModel, sci.Text);
                var currentClass = currentModel.Classes[0];
                ASContext.Context.CurrentClass.Returns(currentClass);
                var currentMember = currentClass.Members[0];
                ASContext.Context.CurrentMember.Returns(currentMember);
                var position = sci.WordEndPosition(sci.CurrentPos, true);
                var result = ASComplete.GetExpressionType(sci, position).Member;
                return result;
            }
        }

        [TestFixture]
        public class GetExpression : ASCompleteTests
        {
            public IEnumerable<TestCaseData> AS3TestCases
            {
                get
                {
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfString"))
                            .Returns(";String")
                            .SetName("From String|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfNewString"))
                            .Returns("new String")
                            .SetName("From new String|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfNewString.charCodeAt"))
                            .Returns("new String.#0~.charCodeAt")
                            .SetName("From new String().charCodeAt|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfNewString.charCodeAt0.toString"))
                            .Returns("new String.#1~.charCodeAt.#0~.toString")
                            .SetName("From new String().charCodeAt(0).toString|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfEmptyStringInitializer"))
                            .Returns("\"")
                            .SetName("From \"\"|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfEmptyStringInitializerSingleQuotes"))
                            .Returns("\"")
                            .SetName("From ''|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfStringInitializer"))
                            .Returns("\"")
                            .SetName("From \"string\"|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfGlogalFunctionString"))
                            .Returns(";")
                            .SetName("From String(\"string\")|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfObjectInitializer"))
                            .Returns(";")
                            .SetName("From {}|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfArrayInitializer"))
                            .Returns(";.[]")
                            .SetName("From []|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfArrayInitializer.push"))
                            .Returns(";.[].push")
                            .SetName("From [].push|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfTwoDimensionalArrayInitializer"))
                            .Returns(";.[]")
                            .SetName("From [[], []]|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfVectorInitializer"))
                            .Returns("new <int>")
                            .SetName("From new <int>[]|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfTwoDimensionalVectorInitializer"))
                            .Returns("new <Vector.<int>>")
                            .SetName("From new <Vector.<int>>[new <int>[]]|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfArrayAccess"))
                            .Returns(",.[].[].[]")
                            .SetName("From [[1,2,3], [4,5,6][0][2]|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfNewVector"))
                            .Returns("new Vector.<String>")
                            .SetName("From new Vector.<String>|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfNewTwoDimensionalVector"))
                            .Returns("new Vector.<Vector.<String>>")
                            .SetName("From new Vector.<Vector.<String>>|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfRegex"))
                            .Returns(";g")
                            .SetName("From /regex/g|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfDigit"))
                            .Returns(";1")
                            .SetName("From 1|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfNumber"))
                            .Returns(";10.0")
                            .SetName("From 10.0|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfInt"))
                            .Returns(";1")
                            .SetName("From -1|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfBoolean"))
                            .Returns(";true")
                            .SetName("From true|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfXML"))
                            .Returns(" ")
                            .SetName("<xml><![CDATA[string]]></xml>|");
                }
            }

            [Test, TestCaseSource(nameof(AS3TestCases))]
            public string AS3(string text) => AS3Impl(text, sci);

            public IEnumerable<TestCaseData> HaxeTestCases
            {
                get
                {
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionOfNewMap"))
                            .Returns("new Map<String, Int>")
                            .SetName("From new Map<String, Int>|");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionOfNewMap2"))
                            .Returns("new Map<Map<String, Int>, Int>")
                            .SetName("From new Map<Map<String, Int>, Int>|");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionOfNewMap3"))
                            .Returns("new Map<String, Array<Map<String, Int>>>")
                            .SetName("From new Map<String, Array<Map<String, Int>>>|");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionOfNewMap4"))
                            .Returns("new Map<String, Array<Int->Int->Int>>")
                            .SetName("From new Map<String, Array<Int->Int->Int>>|");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionOfMapInitializer"))
                            .Returns(" ")
                            .SetName("From ['1' => 1, '2' => 2]|");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionOfRegex"))
                            .Returns(";g")
                            .SetName("~/regex/g|");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionOfNewArray"))
                            .Returns("new Array<Int>")
                            .SetName("From new Array<Int>|");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionOfNewArray2"))
                            .Returns("new Array<{x:Int, y:Int}>")
                            .SetName("From new Array<{x:Int, y:Int}>|");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionOfNewArray3"))
                            .Returns("new Array<{name:String, params:Array<Dynamic>}>")
                            .SetName("From new Array<{name:String, params:Array<Dynamic>}>|");
                }
            }

            [Test, TestCaseSource(nameof(HaxeTestCases))]
            public string Haxe(string text) => HaxeImpl(text, sci);

            internal static string AS3Impl(string text, ScintillaControl sci)
            {
                sci.ConfigurationLanguage = "as3";
                ASContext.Context.SetAs3Features();
                return Common(text, sci);
            }

            internal static string HaxeImpl(string text, ScintillaControl sci)
            {
                sci.ConfigurationLanguage = "haxe";
                ASContext.Context.SetHaxeFeatures();
                return Common(text, sci);
            }

            internal static string Common(string text, ScintillaControl sci)
            {
                sci.Text = text;
                SnippetHelper.PostProcessSnippets(sci, 0);
                var expr = ASComplete.GetExpression(sci, sci.CurrentPos);
                return $"{expr.WordBefore}{expr.Separator}{expr.Value}";
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

            [Test, TestCaseSource(nameof(DisambiguateComaAS3TestCases))]
            public ComaExpression AS3(string text)
            {
                ASContext.Context = new AS3Context.Context(new AS3Settings());

                sci.Text = text;
                sci.ConfigurationLanguage = "as3";

                var coma = ASComplete.DisambiguateComa(sci, text.Length, 0);

                return coma;
            }

            [Test, TestCaseSource(nameof(DisambiguateComaHaxeTestCases))]
            public ComaExpression Haxe(string text)
            {
                ASContext.Context = new HaXeContext.Context(new HaXeSettings());

                sci.Text = text;
                sci.ConfigurationLanguage = "haxe";

                var coma = ASComplete.DisambiguateComa(sci, text.Length, 0);

                return coma;
            }
        }

        internal static string ReadAllTextAS3(string fileName)
        {
            return TestFile.ReadAllText($"ASCompletion.Test_Files.completion.as3.{fileName}.as");
        }

        internal static string ReadAllTextHaxe(string fileName)
        {
            return TestFile.ReadAllText($"ASCompletion.Test_Files.completion.haxe.{fileName}.hx");
        }

    }
}
