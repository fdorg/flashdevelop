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

        private static void SetAs3Features(ScintillaControl sci)
        {
            if (sci.ConfigurationLanguage != "as3")
            {
                sci.ConfigurationLanguage = "as3";
                ASContext.Context.SetAs3Features();
            }
        }

        private static void SetHaxeFeatures(ScintillaControl sci)
        {
            if (sci.ConfigurationLanguage != "haxe")
            {
                sci.ConfigurationLanguage = "haxe";
                ASContext.Context.SetHaxeFeatures();
            }
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
                SetAs3Features(sci);
                return Common(sourceText, sci);
            }

            internal static MemberModel HaxeImpl(string sourceText, ScintillaControl sci)
            {
                SetHaxeFeatures(sci);
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
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfStringInitializer.charCodeAt0.toString"))
                            .Returns(";\"string\".#1~.charCodeAt.#0~.toString")
                            .SetName("From \"string\".charCodeAt(0).toString|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfStringInitializer2.charCodeAt0.toString"))
                            .Returns(";'string'.#1~.charCodeAt.#0~.toString")
                            .SetName("From 'string'.charCodeAt(0).toString|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfEmptyStringInitializer"))
                            .Returns(";\"\"")
                            .SetName("From \"\"|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfEmptyStringInitializerSingleQuotes"))
                            .Returns(";''")
                            .SetName("From ''|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfStringInitializer"))
                            .Returns(";\"string\"")
                            .SetName("From \"string\"|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfGlogalFunctionString"))
                            .Returns(";")
                            .SetName("From String(\"string\")|");
                    yield return
                        new TestCaseData(ReadAllTextAS3("GetExpressionOfObjectInitializer"))
                            .Returns(";{}")
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
                            .Returns(";.[[], []]")
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
                            .Returns(",.[4,5,6].[0].[2]")
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
                            .Returns(";</>")
                            .SetName("<xml/>|");
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
                    yield return
                        new TestCaseData(ReadAllTextHaxe("GetExpressionOfStringInterpolation.charAt"))
                            .Returns(";'result: ${1 + 2}'.#0~.charAt")
                            .SetName("'result: ${1 + 2}'.charAt");
                }
            }

            [Test, TestCaseSource(nameof(HaxeTestCases))]
            public string Haxe(string text) => HaxeImpl(text, sci);

            internal static string AS3Impl(string text, ScintillaControl sci)
            {
                SetAs3Features(sci);
                return Common(text, sci);
            }

            internal static string HaxeImpl(string text, ScintillaControl sci)
            {
                SetHaxeFeatures(sci);
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

        [TestFixture]
        public class AddClosingBraces : ASCompleteTests
        {
            private const string prefix = "AddClosingBraces: ";

            [TestFixtureSetUp]
            public void AddClosingBracesSetUp()
            {
                ASContext.CommonSettings.AddClosingBraces = true;
            }
            
            public IEnumerable<TestCaseData> OpenBraceTestCases
            {
                get
                {
                    yield return new TestCaseData("(+").       Returns("()").        SetName(prefix + "Open ( after 'Default' before 'Default'");
                    yield return new TestCaseData("(+/* */").  Returns("()/* */").   SetName(prefix + "Open ( after 'Default' before 'Comment'");
                    yield return new TestCaseData("(+//").     Returns("()//").      SetName(prefix + "Open ( after 'Default' before 'CommentLine'");
                    yield return new TestCaseData("(+/** */"). Returns("()/** */").  SetName(prefix + "Open ( after 'Default' before 'CommentDoc'");
                    yield return new TestCaseData("(+///").    Returns("()///").     SetName(prefix + "Open ( after 'Default' before 'CommentLineDoc'");
                    yield return new TestCaseData("(+)").      Returns("())").       SetName(prefix + "Open ( after 'Default' before )");
                    yield return new TestCaseData("(+}").      Returns("()}").       SetName(prefix + "Open ( after 'Default' before }");
                    yield return new TestCaseData("(+]").      Returns("()]").       SetName(prefix + "Open ( after 'Default' before ]");
                    yield return new TestCaseData("(+>").      Returns("()>").       SetName(prefix + "Open ( after 'Default' before >");
                    yield return new TestCaseData("(+a").      Returns("(a").        SetName(prefix + "Open ( after 'Default' before 'Identifier'");

                    yield return new TestCaseData("{+").       Returns("{ }").       SetName(prefix + "Open { after 'Default' before 'Default'");
                    yield return new TestCaseData("{+)").      Returns("{ })").      SetName(prefix + "Open { after 'Default' before )");
                    yield return new TestCaseData("{+}").      Returns("{ }}").      SetName(prefix + "Open { after 'Default' before }");
                    yield return new TestCaseData("{+]").      Returns("{ }]").      SetName(prefix + "Open { after 'Default' before ]");
                    yield return new TestCaseData("{+>").      Returns("{ }>").      SetName(prefix + "Open { after 'Default' before >");
                    yield return new TestCaseData("{+a").      Returns("{a").        SetName(prefix + "Open { after 'Default' before 'Identifier'");

                    yield return new TestCaseData("[+").       Returns("[]").        SetName(prefix + "Open [ after 'Default' before 'Default'");
                    yield return new TestCaseData("[+/* */").  Returns("[]/* */").   SetName(prefix + "Open [ after 'Default' before 'Comment'");
                    yield return new TestCaseData("[+//").     Returns("[]//").      SetName(prefix + "Open [ after 'Default' before 'CommentLine'");
                    yield return new TestCaseData("[+/** */"). Returns("[]/** */").  SetName(prefix + "Open [ after 'Default' before 'CommentDoc'");
                    yield return new TestCaseData("[+///").    Returns("[]///").     SetName(prefix + "Open [ after 'Default' before 'CommentLineDoc'");
                    yield return new TestCaseData("[+)").      Returns("[])").       SetName(prefix + "Open [ after 'Default' before )");
                    yield return new TestCaseData("[+}").      Returns("[]}").       SetName(prefix + "Open [ after 'Default' before }");
                    yield return new TestCaseData("[+]").      Returns("[]]").       SetName(prefix + "Open [ after 'Default' before ]");
                    yield return new TestCaseData("[+>").      Returns("[]>").       SetName(prefix + "Open [ after 'Default' before >");
                    yield return new TestCaseData("[+a").      Returns("[a").        SetName(prefix + "Open [ after 'Default' before 'Identifier'");

                    yield return new TestCaseData("\"+").      Returns("\"\"").      SetName(prefix + "Open \" after 'Default' before 'Default'");
                    yield return new TestCaseData("\"+/* */"). Returns("\"\"/* */"). SetName(prefix + "Open \" after 'Default' before 'Comment'");
                    yield return new TestCaseData("\"+//").    Returns("\"\"//").    SetName(prefix + "Open \" after 'Default' before 'CommentLine'");
                    yield return new TestCaseData("\"+/** */").Returns("\"\"/** */").SetName(prefix + "Open \" after 'Default' before 'CommentDoc'");
                    yield return new TestCaseData("\"+///").   Returns("\"\"///").   SetName(prefix + "Open \" after 'Default' before 'CommentLineDoc'");
                    yield return new TestCaseData("\"+\"\"").  Returns("\"\"\"\"").  SetName(prefix + "Open \" after 'Default' before 'String'");
                    yield return new TestCaseData("\"+''").    Returns("\"\"''").    SetName(prefix + "Open \" after 'Default' before 'Char'");
                    yield return new TestCaseData("\"+#a").    Returns("\"\"#a").    SetName(prefix + "Open \" after 'Default' before 'Preprocessor'");
                    yield return new TestCaseData("\"+.").     Returns("\"\".").     SetName(prefix + "Open \" after 'Default' before 'Operator'");
                    yield return new TestCaseData("\"+a").     Returns("\"a").       SetName(prefix + "Open \" after 'Default' before 'Identifier'");

                    yield return new TestCaseData("'+").       Returns("''").        SetName(prefix + "Open ' after 'Default' before 'Default'");
                    yield return new TestCaseData("'+/* */").  Returns("''/* */").   SetName(prefix + "Open ' after 'Default' before 'Comment'");
                    yield return new TestCaseData("'+//").     Returns("''//").      SetName(prefix + "Open ' after 'Default' before 'CommentLine'");
                    yield return new TestCaseData("'+/** */"). Returns("''/** */").  SetName(prefix + "Open ' after 'Default' before 'CommentDoc'");
                    yield return new TestCaseData("'+///").    Returns("''///").     SetName(prefix + "Open ' after 'Default' before 'CommentLineDoc'");
                    yield return new TestCaseData("'+\"\"").   Returns("''\"\"").    SetName(prefix + "Open ' after 'Default' before 'String'");
                    yield return new TestCaseData("'+''").     Returns("''''").      SetName(prefix + "Open ' after 'Default' before 'Char'");
                    yield return new TestCaseData("'+#a").     Returns("''#a").      SetName(prefix + "Open ' after 'Default' before 'Preprocessor'");
                    yield return new TestCaseData("'+.").      Returns("''.").       SetName(prefix + "Open ' after 'Default' before 'Operator'");
                    yield return new TestCaseData("'+a").      Returns("'a").        SetName(prefix + "Open ' after 'Default' before 'Identifier'");

                    yield return new TestCaseData("<+").       Returns("<").         SetName(prefix + "Open < after 'Default' before 'Default'");
                    yield return new TestCaseData(".<+").      Returns(".<>").       SetName(prefix + "Open < after 'Operator' before 'Default'");
                    yield return new TestCaseData("Void<+").   Returns("Void<>").    SetName(prefix + "Open < after 'Type' before 'Default'");
                    yield return new TestCaseData(".<+<").     Returns(".<<").       SetName(prefix + "Open < after 'Operator' before <");
                    yield return new TestCaseData(".<+a").     Returns(".<a").       SetName(prefix + "Open < after 'Operator' before 'Identifier'");
                    yield return new TestCaseData(".<+Void").  Returns(".<Void").    SetName(prefix + "Open < after 'Operator' before 'Type'");
                }
            }

            public IEnumerable<TestCaseData> CloseBraceTestCases
            {
                get
                {
                    yield return new TestCaseData("()+)").     Returns("()").      SetName(prefix + "Close ) to overwrite )");
                    yield return new TestCaseData("{}+}").     Returns("{}").      SetName(prefix + "Close } to overwrite }");
                    yield return new TestCaseData("[]+]").     Returns("[]").      SetName(prefix + "Close ] to overwrite ]");
                    yield return new TestCaseData("\"\"+\"").  Returns("\"\"").    SetName(prefix + "Close \" to overwrite \"");
                    yield return new TestCaseData("\"\\\"+\"").Returns("\"\\\"\"").SetName(prefix + "Close \" escaped should not overwrite \"");
                    yield return new TestCaseData("''+'").     Returns("''").      SetName(prefix + "Close ' to overwrite '");
                    yield return new TestCaseData("'\\'+'").   Returns("'\\''").   SetName(prefix + "Close ' escaped should not overwrite '");
                    yield return new TestCaseData("<>+>").     Returns("<>").      SetName(prefix + "Close > to overwrite >");
                }
            }

            public IEnumerable<TestCaseData> DeleteBraceTestCases
            {
                get
                {
                    yield return new TestCaseData("(-)").  Returns("").SetName(prefix + "Delete ( to delete )");
                    yield return new TestCaseData("{-}").  Returns("").SetName(prefix + "Delete { to delete }");
                    yield return new TestCaseData("[-]").  Returns("").SetName(prefix + "Delete [ to delete ]");
                    yield return new TestCaseData("\"-\"").Returns("").SetName(prefix + "Delete \" to delete \"");
                    yield return new TestCaseData("'-'").  Returns("").SetName(prefix + "Delete ' to delete '");
                    yield return new TestCaseData("<->").  Returns("").SetName(prefix + "Delete < to delete >");
                }
            }

            public IEnumerable<TestCaseData> AroundStringsTestCases
            {
                get
                {
                    yield return new TestCaseData("\"\"(+").     Returns("\"\"()").   SetName(prefix + "Open ( after 'String' before 'Default'");
                    yield return new TestCaseData("\"\"(+\"\""). Returns("\"\"(\"\"").SetName(prefix + "Open ( after 'String' before 'String'");
                    yield return new TestCaseData("\"\")+)").    Returns("\"\")").    SetName(prefix + "Close ) after 'String' to overwrite )");
                    yield return new TestCaseData("\"\")+)\"\"").Returns("\"\")\"\"").SetName(prefix + "Close ) after 'String' before 'String' to overwrite )");
                    yield return new TestCaseData("\"\"(-)").    Returns("\"\"").     SetName(prefix + "Delete ( after 'String' to delete )");
                    yield return new TestCaseData("\"\"(-)\"\"").Returns("\"\"\"\""). SetName(prefix + "Delete ( after 'String' before 'String' to delete )");

                    yield return new TestCaseData("\"\"\"+").    Returns("\"\"\"\""). SetName(prefix + "Open \" after 'String' before 'Default'");
                    yield return new TestCaseData("\"\"\"\"+\"").Returns("\"\"\"\""). SetName(prefix + "Close \" after 'String' to overwrite \"");
                    yield return new TestCaseData("\"\"\"-\"").  Returns("\"\"").     SetName(prefix + "Delete \" after 'String' to delete \"");

                    yield return new TestCaseData("\"(+\"").     Returns("\"(\"").    SetName(prefix + "Open ( inside a string");
                    yield return new TestCaseData("\")+)\"").    Returns("\"))\"").   SetName(prefix + "Close ) inside a string");
                    yield return new TestCaseData("\"(-)\"").    Returns("\")\"").    SetName(prefix + "Delete ( inside a string");
                }
            }

            public IEnumerable<TestCaseData> DeleteWhitespaceTestCases
            {
                get
                {
                    yield return new TestCaseData(")+ )").  Returns(" )"). SetName(prefix + "Close ) to overwrite ) after whitespace");
                    yield return new TestCaseData("}+\t}"). Returns("\t}").SetName(prefix + "Close } to overwrite } after whitespace");
                    yield return new TestCaseData("]+\t]"). Returns("\t]").SetName(prefix + "Close ] to overwrite ] after whitespace");

                    yield return new TestCaseData("(- )").  Returns("").   SetName(prefix + "Delete ( to delete ) and the whitespace in between");
                    yield return new TestCaseData("{-\n}"). Returns("").   SetName(prefix + "Delete { to delete } and the whitespace in between");
                    yield return new TestCaseData("[-\t]"). Returns("").   SetName(prefix + "Delete [ to delete ] and the whitespace in between");
                    yield return new TestCaseData("\"- \"").Returns(" \"").SetName(prefix + "Delete \" without deleting \" and the whitespace in between");
                    yield return new TestCaseData("'- '").  Returns(" '"). SetName(prefix + "Delete ' without deleting ' and the whitespace in between");
                    yield return new TestCaseData("<- >").  Returns(" >"). SetName(prefix + "Delete < without deleting > and the whitespace in between");
                }
            }

            public IEnumerable<TestCaseData> InsideInterpolationTestCases
            {
                get
                {
                    yield return new TestCaseData("'${+'").     Returns("'${}'").    SetName(prefix + "Open interpolation {");
                    yield return new TestCaseData("'${-}'").    Returns("'$'").      SetName(prefix + "Delete interpolation { to delete }");

                    yield return new TestCaseData("'${(+}'").   Returns("'${()}'").  SetName(prefix + "Open ( inside string interpolation");
                    yield return new TestCaseData("'${{+}'").   Returns("'${{ }}'"). SetName(prefix + "Open { inside string interpolation");
                    yield return new TestCaseData("'${[+}'").   Returns("'${[]}'").  SetName(prefix + "Open [ inside string interpolation");
                    yield return new TestCaseData("'${\"+}'").  Returns("'${\"\"}'").SetName(prefix + "Open \" inside string interpolation");
                    yield return new TestCaseData("'${'+}'").   Returns("'${''}'").  SetName(prefix + "Open ' inside string interpolation");

                    yield return new TestCaseData("'${(-)}'").  Returns("'${}'").    SetName(prefix + "Delete ( to delete ) inside string interpolation");
                    yield return new TestCaseData("'${{-}}'").  Returns("'${}'").    SetName(prefix + "Delete { to delete } inside string interpolation");
                    yield return new TestCaseData("'${[-]}'").  Returns("'${}'").    SetName(prefix + "Delete [ to delete ] inside string interpolation");
                    yield return new TestCaseData("'${\"-\"}'").Returns("'${}'").    SetName(prefix + "Delete \" to delete \" inside string interpolation");
                    yield return new TestCaseData("'${'-'}'").  Returns("'${}'").    SetName(prefix + "Delete ' to delete ' inside string interpolation");

                    yield return new TestCaseData("''(+''").    Returns("''(''").    SetName(prefix + "Open ( after 'Character' before 'Character'");
                }
            }

            [Test]
            [TestCaseSource(nameof(OpenBraceTestCases)), TestCaseSource(nameof(CloseBraceTestCases)),
                TestCaseSource(nameof(DeleteBraceTestCases)), TestCaseSource(nameof(AroundStringsTestCases)),
                TestCaseSource(nameof(DeleteWhitespaceTestCases))]
            public string AS3(string text)
            {
                SetAs3Features(sci);
                return Common(text.Replace('V', 'v'), sci).Replace('v', 'V'); //Replace "Void" with "void" for type checking
            }

            [Test]
            [TestCaseSource(nameof(OpenBraceTestCases)), TestCaseSource(nameof(CloseBraceTestCases)),
                TestCaseSource(nameof(DeleteBraceTestCases)), TestCaseSource(nameof(AroundStringsTestCases)),
                TestCaseSource(nameof(DeleteWhitespaceTestCases)), TestCaseSource(nameof(InsideInterpolationTestCases))]
            public string Haxe(string text)
            {
                SetHaxeFeatures(sci);
                return Common(text, sci);
            }

            private static string Common(string text, ScintillaControl sci)
            {
                text = "\n" + text + "\n"; // Surround with new line characters to enable colourisation
                int cursor = text.IndexOf('+'); // Char before is added
                bool addedChar = cursor >= 0;
                if (!addedChar)
                {
                    cursor = text.IndexOf('-'); // Char before is about to be deleted
                    Assert.GreaterOrEqual(cursor, 0, "Missing a cursor character: either + or -");
                }
                sci.SetText(text);
                sci.SetSel(cursor, cursor + 1);
                sci.ReplaceSel("");
                sci.Colourise(0, -1);
                int c = addedChar ? sci.CharAt(sci.CurrentPos - 1) : sci.CurrentChar;
                ASComplete.HandleAddClosingBraces(sci, (char) c, addedChar);
                if (!addedChar)
                {
                    sci.DeleteBack(); // Backspace is handled after HandleAddClosingBraces(), so mimic that behaviour
                }
                return sci.GetTextRange(1, sci.TextLength - 1); // Ignore the surrounding new line characters
            }
        }

        [TestFixture]
        public class FindParameterIndexTests : ASCompleteTests
        {
            public IEnumerable<TestCaseData> HaxeTestCases
            {
                get
                {
                    yield return
                        new TestCaseData("foo($(EntryPoint));function foo();")
                            .Returns(0);
                    yield return
                        new TestCaseData("foo(1, $(EntryPoint));function foo(x:Int, y:Int);")
                            .Returns(1);
                    yield return
                        new TestCaseData("foo([1,2,3,4,5], $(EntryPoint));function foo(x:Array<Int>, y:Int);")
                            .Returns(1);
                    yield return
                        new TestCaseData("foo(new Array<Int>(), $(EntryPoint));function foo(x:Array<Int>, y:Int);")
                            .Returns(1);
                    yield return
                        new TestCaseData("foo(new Map<Int, String>(), $(EntryPoint));function foo(x:Map<Int, String>, y:Int);")
                            .Returns(1);
                    yield return
                        new TestCaseData("foo({x:Int, y:Int}, $(EntryPoint));function foo(x:{x:Int, y:Int}, y:Int);")
                            .Returns(1);
                    yield return
                        new TestCaseData("foo(',,,,', $(EntryPoint));function foo(s:String, y:Int);")
                            .Returns(1);
                    yield return
                        new TestCaseData("foo(\",,,,\", $(EntryPoint));function foo(s:String, y:Int);")
                            .Returns(1);
                    yield return
                        new TestCaseData("foo(\"\\ \", $(EntryPoint));function foo(s:String, y:Int);")
                            .Returns(1);
                    yield return
                        new TestCaseData("foo(0, ';', $(EntryPoint));function foo(i:Int, s:String, y:Int);")
                            .Returns(2);
                    yield return
                        new TestCaseData("foo(0, '}}}', $(EntryPoint));function foo(i:Int, s:String, y:Int);")
                            .Returns(2);
                    yield return
                        new TestCaseData("foo(0, '<<>>><{}>}->({[]})>><<,,;>><<', $(EntryPoint));function foo(i:Int, s:String, y:Int);")
                            .Returns(2);
                    yield return
                        new TestCaseData("foo(0, '(', $(EntryPoint));function foo(i:Int, s:String, y:Int);")
                            .Returns(2);
                    yield return
                        new TestCaseData("foo(0, ')', $(EntryPoint));function foo(i:Int, s:String, y:Int);")
                            .Returns(2);
                    yield return
                        new TestCaseData("foo(0, {c:Array(<Int>->Map<String, Int>)->Void}, $(EntryPoint));function foo(i:Int, s:Dynamic, y:Int);")
                            .Returns(2);
                    yield return
                        new TestCaseData("foo(0, function(i:Int, s:String) {return Std.string(i) + ', ' + s;}, $(EntryPoint));function foo(i:Int, s:Dynamic, y:Int);")
                            .Returns(2);
                    yield return
                        new TestCaseData("foo(0, function() {var i = 1, j = 2; return i + j;}, $(EntryPoint));function foo(i:Int, s:Dynamic, y:Int);")
                            .Returns(2);
                }
            }

            [Test, TestCaseSource(nameof(HaxeTestCases))]
            public int Haxe(string text) => HaxeImpl(text, sci);

            internal static int HaxeImpl(string text, ScintillaControl sci)
            {
                SetHaxeFeatures(sci);
                return Common(text, sci);
            }

            internal static int Common(string text, ScintillaControl sci)
            {
                sci.Text = text;
                SnippetHelper.PostProcessSnippets(sci, 0);
                var pos = sci.CurrentPos - 1;
                return ASComplete.FindParameterIndex(sci, ref pos);
            }
        }

        [TestFixture]
        public class Issue104Tests : ASCompleteTests
        {
            static IEnumerable<TestCaseData> ParseClassTestCases_issue104
            {
                get
                {
                    yield return new TestCaseData("Iterable", FlagType.Class | FlagType.TypeDef)
                        .Returns("\n\tAn `Iterable` is a data structure which has an `iterator()` method.\n\tSee `Lambda` for generic functions on iterable structures.\n\n\t@see https://haxe.org/manual/lf-iterators.html\n")
                        .SetName("Issue 104. typdef");
                    yield return new TestCaseData("Any", FlagType.Class | FlagType.Abstract)
                        .Returns("\n\t`Any` is a type that is compatible with any other in both ways.\n\n\tThis means that a value of any type can be assigned to `Any`, and\n\tvice-versa, a value of `Any` type can be assigned to any other type.\n\n\tIt's a more type-safe alternative to `Dynamic`, because it doesn't\n\tsupport field access or operators and it's bound to monomorphs. So,\n\tto work with the actual value, it needs to be explicitly promoted\n\tto another type.\n")
                        .SetName("Issue 104. abstract");
                }
            }

            [Test, TestCaseSource(nameof(ParseClassTestCases_issue104))]
            public string ParseFile_Issue104(string name, FlagType flags)
            {
                ASContext.Context.SetHaxeFeatures();
                var visibleExternalElements = ASContext.Context.GetVisibleExternalElements();
                var result = visibleExternalElements.Search(name, flags, 0);
                return result.Comments;
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
