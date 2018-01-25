using System.Collections.Generic;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PluginCore.Helpers;
using ScintillaNet;

//TODO: Sadly most of ASComplete is currently untestable in a proper way. Work on this branch solves it: https://github.com/Neverbirth/flashdevelop/tree/completionlist

namespace ASCompletion.Completion
{
    [TestFixture]
    public class ASCompleteTests : ASCompletionTests
    {
        internal static ASResult GetExpressionType(ScintillaControl sci, string sourceText)
        {
            SetSrc(sci, sourceText);
            return ASComplete.GetExpressionType(sci, sci.WordEndPosition(sci.CurrentPos, true));
        }

        internal static string GetExpression(ScintillaControl sci, string sourceText)
        {
            SetSrc(sci, sourceText);
            var expr = ASComplete.GetExpression(sci, sci.CurrentPos);
            var value = expr.Value;
            if (!string.IsNullOrEmpty(value) && expr.SubExpressions != null)
            {
                for (var i = 0; i < expr.SubExpressions.Count; i++)
                {
                    var subExpr = expr.SubExpressions[i];
                    value = value.Replace($".#{i}~", subExpr).Replace($"#{i}~", subExpr);
                }
            }
            return $"{expr.WordBefore}{expr.Separator}{value}{expr.RightOperator}";
        }

        internal static ComaExpression DisambiguateComa(ScintillaControl sci, string sourceText)
        {
            sci.Text = sourceText;
            return ASComplete.DisambiguateComa(sci, sourceText.Length, 0);
        }

        internal static int FindParameterIndex(ScintillaControl sci, string sourceText)
        {
            sci.Text = sourceText;
            sci.Colourise(0, -1);
            SnippetHelper.PostProcessSnippets(sci, 0);
            var pos = sci.CurrentPos - 1;
            var result = ASComplete.FindParameterIndex(sci, ref pos);
            Assert.AreNotEqual(-1, pos);
            return result;
        }

        internal static int ExpressionEndPosition(ScintillaControl sci, string sourceText)
        {
            SetSrc(sci, sourceText);
            return ASComplete.ExpressionEndPosition(sci, sci.CurrentPos);
        }

        public class ActonScript3 : ASCompleteTests
        {
            protected static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

            protected static string GetFullPath(string fileName) => $"ASCompletion.Test_Files.completion.as3.{fileName}.as";

            [TestFixtureSetUp]
            public void Setup()
            {
                ASContext.Context.SetAs3Features();
                sci.ConfigurationLanguage = "as3";
            }

            [Test]
            public void GetExpressionTypeSimpleTest()
            {
                //TODO: Improve this test with more checks!
                ASContext.Context.CurrentLine = 9;

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

                sci.Text = ReadAllText("SimpleTest");
                sci.Colourise(0, -1);

                var result = ASComplete.GetExpressionType(sci, 185);

                Assert.NotNull(result.Context?.LocalVars);
                Assert.AreEqual(4, result.Context.LocalVars.Count);
            }

            static IEnumerable<TestCaseData> GetExpressionTypeTestCases
            {
                get
                {
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfConstructor"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Function | FlagType.Constructor,
                                Name = "Foo"
                            })
                            .SetName("Get Expression Type of constructor");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfConstructorParameter"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Variable | FlagType.ParameterVar,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of constructor parameter");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfFunction"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Dynamic | FlagType.Function,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of function");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfFunctionParameter"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Variable | FlagType.ParameterVar,
                                Name = "bar"
                            })
                            .SetName("Get Expression Type of function parameter");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfFunctionParameter2"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Variable | FlagType.ParameterVar,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of function parameter");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfFunctionWithParameter"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Dynamic | FlagType.Function,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of function with parameter");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfLocalObjectKey"))
                            .Returns(null)
                            .SetName("Get Expression Type of local dynamic object key");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfLocalObjectValue"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Dynamic | FlagType.Function,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of local dynamic object value");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfLocalVar"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Dynamic | FlagType.Variable | FlagType.LocalVar,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of local var");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfSetterParameter"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Variable | FlagType.ParameterVar,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of setter parameter");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfVariable"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Dynamic | FlagType.Variable,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of variable");
                }
            }

            [Test, TestCaseSource(nameof(GetExpressionTypeTestCases))]
            public MemberModel GetExpressionType_Member(string sourceText)
            {
                var expr = GetExpressionType(sci, sourceText);
                return expr.Member;
            }

            static IEnumerable<TestCaseData> GetExpressionType_TypeTestCases
            {
                get
                {
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionType_Type_as"))
                            .Returns(new ClassModel
                            {
                                Name = "String",
                                Flags = FlagType.Class,
                                Access = Visibility.Public
                            })
                            .SetName("('s' as String).");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionType_Type_as_2"))
                            .Returns(new ClassModel
                            {
                                Name = "String",
                                Flags = FlagType.Class,
                                Access = Visibility.Public
                            })
                            .SetName("return ('s' as String).");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionType_Type_is"))
                            .Returns(new ClassModel
                            {
                                Name = "Boolean",
                                Flags = FlagType.Class,
                                Access = Visibility.Public
                            })
                            .SetName("('s' is String).");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer"))
                            .Returns(new ClassModel
                            {
                                Name = "Array",
                                Flags = FlagType.Class,
                                Access = Visibility.Public
                            })
                            .SetName("[].");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer"))
                            .Returns(new ClassModel
                            {
                                Name = "String",
                                Flags = FlagType.Class,
                                Access = Visibility.Public
                            })
                            .SetName("\"\".");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer_2"))
                            .Returns(new ClassModel
                            {
                                Name = "String",
                                Flags = FlagType.Class,
                                Access = Visibility.Public
                            })
                            .SetName("''.");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionType_Type_objectInitializer"))
                            .Returns(new ClassModel
                            {
                                Name = "Object",
                                Flags = FlagType.Class,
                                Access = Visibility.Public
                            })
                            .SetName("{}.");
                }
            }

            [Test, TestCaseSource(nameof(GetExpressionType_TypeTestCases))]
            public ClassModel GetExpressionType_Type(string sourceText)
            {
                var expr = GetExpressionType(sci, sourceText);
                return expr.Type;
            }

            static IEnumerable<TestCaseData> GetExpressionTestCases
            {
                get
                {
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfString"))
                            .Returns(";String")
                            .SetName("From String|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfNewString"))
                            .Returns("new String")
                            .SetName("From new String|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfNewString.charCodeAt"))
                            .Returns("new String().charCodeAt")
                            .SetName("From new String().charCodeAt|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfNewString.charCodeAt0.toString"))
                            .Returns("new String().charCodeAt(0).toString")
                            .SetName("From new String().charCodeAt(0).toString|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfStringInitializer.charCodeAt0.toString"))
                            .Returns(";\"string\".charCodeAt(0).toString")
                            .SetName("From \"string\".charCodeAt(0).toString|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfStringInitializer2.charCodeAt0.toString"))
                            .Returns(";'string'.charCodeAt(0).toString")
                            .SetName("From 'string'.charCodeAt(0).toString|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfEmptyStringInitializer"))
                            .Returns(";\"\"")
                            .SetName("From \"\"|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfEmptyStringInitializerSingleQuotes"))
                            .Returns(";''")
                            .SetName("From ''|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfStringInitializer"))
                            .Returns(";\"string\"")
                            .SetName("From \"string\"|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfStringInitializer2"))
                            .Returns(";\"{[(<string\"")
                            .SetName("From \"{[(<string\"|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfStringInitializer3"))
                            .Returns(";\"string>}])\"")
                            .SetName("From \"string>}])\"|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfGlobalFunctionString"))
                            .Returns(";")
                            .SetName("From String(\"string\")|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfObjectInitializer"))
                            .Returns(";{}")
                            .SetName("From {}|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfArrayInitializer"))
                            .Returns(";[]")
                            .SetName("From []|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfArrayInitializer.push"))
                            .Returns(";[].push")
                            .SetName("From [].push|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfTwoDimensionalArrayInitializer"))
                            .Returns(";[[], []]")
                            .SetName("From [[], []]|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfVectorInitializer"))
                            .Returns("new <int>[]")
                            .SetName("From new <int>[]|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfTwoDimensionalVectorInitializer"))
                            .Returns("new <Vector.<int>>[new <int>[]]")
                            .SetName("From new <Vector.<int>>[new <int>[]]|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfArrayAccess"))
                            .Returns(",[4,5,6].[0].[2]")
                            .SetName("From [[1,2,3], [4,5,6][0][2]|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfNewVector"))
                            .Returns("new Vector.<String>")
                            .SetName("From new Vector.<String>|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfNewTwoDimensionalVector"))
                            .Returns("new Vector.<Vector.<String>>")
                            .SetName("From new Vector.<Vector.<String>>|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfRegex"))
                            .Returns(";g")
                            .SetName("From /regex/g|")
                            .Ignore("https://github.com/fdorg/flashdevelop/issues/1880");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfDigit"))
                            .Returns(";1")
                            .SetName("From 1|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfNumber"))
                            .Returns(";10.0")
                            .SetName("From 10.0|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfInt"))
                            .Returns("-1")
                            .SetName("From -1|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfBoolean"))
                            .Returns(";true")
                            .SetName("From true|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfXML"))
                            .Returns(";</>")
                            .SetName("<xml/>|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_plus"))
                            .Returns("+1")
                            .SetName("1 + 1");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_minus"))
                            .Returns("-1")
                            .SetName("1 - 1");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_mul"))
                            .Returns("*1")
                            .SetName("1 * 1");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_division"))
                            .Returns("/1")
                            .SetName("1 / 1");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_increment"))
                            .Returns("++1")
                            .SetName("++1");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_increment2"))
                            .Returns(";1++")
                            .SetName("1++. case 1");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_increment3"))
                            .Returns(";1++")
                            .SetName("1++. case 2");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_increment4"))
                            .Returns(";a++")
                            .SetName("a++");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_increment5"))
                            .Returns("=getId()++")
                            .SetName("var id = getId()++");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_decrement"))
                            .Returns("--1")
                            .SetName("--1");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_decrement2"))
                            .Returns(";1--")
                            .SetName("1--. case 1");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_decrement3"))
                            .Returns(";1--")
                            .SetName("1--. case 2");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_decrement4"))
                            .Returns(";a--")
                            .SetName("a--");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_decrement5"))
                            .Returns("=getId()--")
                            .SetName("var id = getId()--");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_decrement6"))
                            .Returns("* ++1")
                            .SetName("5 * ++1");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_decrement7"))
                            .Returns("*1++")
                            .SetName("5 * 1++");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1908_typeof"))
                            .Returns("typeof 1")
                            .SetName("typeof 1");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1908_delete"))
                            .Returns("delete o.[k]")
                            .SetName("delete o[k]");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_operator_is"))
                            .Returns(";(\"s\" is String).")
                            .SetName("(\"s\" is String)");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_operator_as"))
                            .Returns(";(\"s\" as String).")
                            .SetName("(\"s\" as String)");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_return_operator_as"))
                            .Returns("return;(\"s\" as String).")
                            .SetName("return (\"s\" as String)");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1954"))
                            .Returns(";re")
                            .SetName("function foo():Vector.<int> { re");
                    yield return
                        new TestCaseData("[].$(EntryPoint)")
                            .Returns(" [].")
                            .SetName("From [].|");
                    yield return
                        new TestCaseData("new <int>[].$(EntryPoint)")
                            .Returns("new <int>[].")
                            .SetName("From new <int>[].|");
                    yield return
                        new TestCaseData("new <int>[1,2,3,4].$(EntryPoint)")
                            .Returns("new <int>[1,2,3,4].")
                            .SetName("From new <int>[1,2,3,4].|");
                }
            }

            [Test, TestCaseSource(nameof(GetExpressionTestCases))]
            public string GetExpression(string sourceText) => GetExpression(sci, sourceText);

            static IEnumerable<TestCaseData> DisambiguateComaTestCases
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

            [Test, TestCaseSource(nameof(DisambiguateComaTestCases))]
            public ComaExpression DisambiguateComa(string sourceText) => DisambiguateComa(sci, sourceText);

            static IEnumerable<TestCaseData> ExpressionEndPositionTestCases
            {
                get
                {
                    yield return new TestCaseData("test()\ntest()")
                        .Returns("test()".Length);
                    yield return new TestCaseData("test()  /*some comment*/, 1)")
                        .Returns("test()".Length);
                    yield return new TestCaseData("test ( [1,2,3,4,5,6,7,8] )  /*some comment*/, 1)")
                        .Returns("test ( [1,2,3,4,5,6,7,8] )".Length);
                    yield return new TestCaseData("test.apply(null, [1,2,3,4,5,6,7,8] )  /*some comment*/, 1)")
                        .Returns("test".Length);
                    yield return new TestCaseData("test( <int>[1,2,3,4] ); //")
                        .Returns("test( <int>[1,2,3,4] )".Length);
                    yield return new TestCaseData("a > b")
                        .Returns("a".Length);
                    yield return new TestCaseData("a ? b : c")
                        .Returns("a".Length);
                    yield return new TestCaseData("a:Boolean")
                        .Returns("a".Length);
                    yield return new TestCaseData("a = b")
                        .Returns("a".Length);
                    yield return new TestCaseData("a += b")
                        .Returns("a".Length);
                    yield return new TestCaseData("a << b")
                        .Returns("a".Length);
                    yield return new TestCaseData("a+b")
                        .Returns("a".Length);
                    yield return new TestCaseData("test(1, 2, 3 ;")
                        .Returns("test".Length);
                    yield return new TestCaseData("test(1, 2, 3 \n}")
                        .Returns("test".Length);
                    yield return new TestCaseData("test(1, 2, 3 ]")
                        .Returns("test".Length);
                    yield return new TestCaseData("test }")
                        .Returns("test".Length);
                    yield return new TestCaseData(ReadAllText("ExpressionEndPosition_TypeOfVariable"))
                        .Returns(94);
                }
            }

            [Test, TestCaseSource(nameof(ExpressionEndPositionTestCases))]
            public int ExpressionEndPosition(string sourceText) => ExpressionEndPosition(sci, sourceText);
        }

        public class Haxe : ASCompleteTests
        {
            protected static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

            protected static string GetFullPath(string fileName) => $"ASCompletion.Test_Files.completion.haxe.{fileName}.hx";

            [TestFixtureSetUp]
            public void Setup()
            {
                ASContext.Context.SetHaxeFeatures();
                sci.ConfigurationLanguage = "haxe";
            }

            static IEnumerable<TestCaseData> GetExpressionTypeTestCases
            {
                get
                {
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfConstructor"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Function | FlagType.Constructor,
                                Name = "Foo"
                            })
                            .SetName("Get Expression Type of constructor");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfConstructorParameter"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Variable | FlagType.ParameterVar,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of constructor parameter");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfFunction"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Dynamic | FlagType.Function,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of function");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfFunctionParameter"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Variable | FlagType.ParameterVar,
                                Name = "bar"
                            })
                            .SetName("Get Expression Type of function parameter");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfFunctionParameter2"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Variable | FlagType.ParameterVar,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of function parameter");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfFunctionWithParameter"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Dynamic | FlagType.Function,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of function with parameter");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfLocalDynamicKey"))
                            .Returns(null)
                            .SetName("Get Expression Type of local dynamic object key");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfLocalDynamicValue"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Dynamic | FlagType.Function,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of local dynamic object value");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfLocalVar"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Dynamic | FlagType.Variable | FlagType.LocalVar,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of local var");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfPropertyGetter"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Dynamic | FlagType.Function,
                                Name = "get_foo"
                            })
                            .SetName("Get Expression Type of property getter");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfPropertySetter"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Dynamic | FlagType.Function,
                                Name = "set_foo"
                            })
                            .SetName("Get Expression Type of property setter");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionTypeOfVariable"))
                            .Returns(new MemberModel
                            {
                                Flags = FlagType.Access | FlagType.Dynamic | FlagType.Variable,
                                Name = "foo"
                            })
                            .SetName("Get Expression Type of variable");
                }
            }

            [Test, TestCaseSource(nameof(GetExpressionTypeTestCases))]
            public MemberModel GetExpressionType_Member(string sourceText)
            {
                var expr = GetExpressionType(sci, sourceText);
                return expr.Member;
            }

            static IEnumerable<TestCaseData> GetExpressionType_TypeTestCases
            {
                get
                {
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionType_Type_typecheck"))
                            .Returns(new ClassModel
                            {
                                Name = "String",
                                Flags = FlagType.Class
                            })
                            .SetName("('s':String).");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionType_Type_typecheck_2"))
                            .Returns(new ClassModel
                            {
                                Name = "String",
                                Flags = FlagType.Class
                            })
                            .SetName("return ('s':String).");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionType_Type_cast"))
                            .Returns(new ClassModel
                            {
                                Name = "String",
                                Flags = FlagType.Class
                            })
                            .SetName("cast('s', String).");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionType_Type_cast"))
                            .Returns(new ClassModel
                            {
                                Name = "String",
                                Flags = FlagType.Class
                            })
                            .SetName("return cast('s', String).");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionType_Type_is"))
                            .Returns(new ClassModel
                            {
                                Name = "Bool",
                                Flags = FlagType.Class
                            })
                            .SetName("('s' is String).");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer"))
                            .Returns(new ClassModel
                            {
                                Name = "Array<T>",
                                Flags = FlagType.Class
                            })
                            .SetName("[].");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionType_Type_mapInitializer"))
                            .Returns(new ClassModel
                            {
                                Name = "Map<K, V>",
                                Flags = FlagType.Class
                            })
                            .SetName("[1=>1].");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer"))
                            .Returns(new ClassModel
                            {
                                Name = "String",
                                Flags = FlagType.Class
                            })
                            .SetName("\"\".");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer_2"))
                            .Returns(new ClassModel
                            {
                                Name = "String",
                                Flags = FlagType.Class
                            })
                            .SetName("''.");
                }
            }

            [Test, TestCaseSource(nameof(GetExpressionType_TypeTestCases))]
            public ClassModel GetExpressionType_Type(string sourceText)
            {
                var expr = GetExpressionType(sci, sourceText);
                return expr.Type;
            }

            [Test]
            public void Issue1867()
            {
                SetSrc(sci, ReadAllText("GetExpressionTypeOfFunction_Issue1867_1"));
                var expr = ASComplete.GetExpressionType(sci, sci.WordEndPosition(sci.CurrentPos, true));
                Assert.AreEqual(new ClassModel {Name = "Function", InFile = new FileModel {Package = "haxe.Constraints"}}, expr.Type);
            }

            static IEnumerable<TestCaseData> GetExpressionTestCases
            {
                get
                {
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfNewMap"))
                            .Returns("new Map<String, Int>")
                            .SetName("From new Map<String, Int>|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfNewMap2"))
                            .Returns("new Map<Map<String, Int>, Int>")
                            .SetName("From new Map<Map<String, Int>, Int>|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfNewMap3"))
                            .Returns("new Map<String, Array<Map<String, Int>>>")
                            .SetName("From new Map<String, Array<Map<String, Int>>>|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfNewMap4"))
                            .Returns("new Map<String, Array<Int->Int->Int>>")
                            .SetName("From new Map<String, Array<Int->Int->Int>>|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfMapInitializer"))
                            .Returns(";[\"1\" => 1, \"2\" => 2]")
                            .SetName("From [\"1\" => 1, \"2\" => 2]|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfRegex"))
                            .Returns(";g")
                            .SetName("~/regex/g|")
                            .Ignore("https://github.com/fdorg/flashdevelop/issues/1880");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfNewArray"))
                            .Returns("new Array<Int>")
                            .SetName("From new Array<Int>|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfNewArray2"))
                            .Returns("new Array<{x:Int, y:Int}>")
                            .SetName("From new Array<{x:Int, y:Int}>|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfNewArray3"))
                            .Returns("new Array<{name:String, params:Array<Dynamic>}>")
                            .SetName("From new Array<{name:String, params:Array<Dynamic>}>|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpressionOfStringInterpolation.charAt"))
                            .Returns(";'result: ${1 + 2}'.charAt")
                            .SetName("'result: ${1 + 2}'.charAt");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_plus"))
                            .Returns("+1")
                            .SetName("1 + 1");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_minus"))
                            .Returns("-1")
                            .SetName("1 - 1");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_mul"))
                            .Returns("*1")
                            .SetName("1 * 1");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_division"))
                            .Returns("/1")
                            .SetName("1 / 1");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_increment"))
                            .Returns("++1")
                            .SetName("++1");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_increment2"))
                            .Returns(";1++")
                            .SetName("1++. case 1");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_increment3"))
                            .Returns(";1++")
                            .SetName("1++. case 2");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_increment4"))
                            .Returns(";a++")
                            .SetName("a++");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_increment5"))
                            .Returns("=getId()++")
                            .SetName("var id = getId()++");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_decrement"))
                            .Returns("--1")
                            .SetName("--1");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_decrement2"))
                            .Returns(";1--")
                            .SetName("1--. case 1");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_decrement3"))
                            .Returns(";1--")
                            .SetName("1--. case 2");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_decrement4"))
                            .Returns(";a--")
                            .SetName("a--");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1749_decrement5"))
                            .Returns("=getId()--")
                            .SetName("var id = getId()--");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1954"))
                            .Returns(";re")
                            .SetName("function foo():Array<Int> { re");
                }
            }

            [Test, TestCaseSource(nameof(GetExpressionTestCases))]
            public string GetExpression(string sourceText) => GetExpression(sci, sourceText);

            static IEnumerable<TestCaseData> DisambiguateComaHaxeTestCases
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

            [Test, TestCaseSource(nameof(DisambiguateComaHaxeTestCases))]
            public ComaExpression DisambiguateComa(string sourceText) => DisambiguateComa(sci, sourceText);

            static IEnumerable<TestCaseData> FindParameterIndexTestCases
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
                    yield return
                        new TestCaseData("foo([1 => 1], $(EntryPoint));")
                            .Returns(1)
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/764");
                    yield return
                        new TestCaseData("foo([for(i in 0...10) i], $(EntryPoint)")
                            .Returns(1)
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/764");
                }
            }

            [Test, TestCaseSource(nameof(FindParameterIndexTestCases))]
            public int FindParameterIndex(string sourceText) => FindParameterIndex(sci, sourceText);

            static IEnumerable<TestCaseData> ParseClass_Issue104TestCases
            {
                get
                {
                    yield return new TestCaseData("Iterable", FlagType.Class | FlagType.TypeDef)
                        .Returns("\n\tAn `Iterable` is a data structure which has an `iterator()` method.\n\tSee `Lambda` for generic functions on iterable structures.\n\n\t@see https://haxe.org/manual/lf-iterators.html\n")
                        .SetName("Issue 104. typedef");
                    yield return new TestCaseData("Any", FlagType.Class | FlagType.Abstract)
                        .Returns("\n\t`Any` is a type that is compatible with any other in both ways.\n\n\tThis means that a value of any type can be assigned to `Any`, and\n\tvice-versa, a value of `Any` type can be assigned to any other type.\n\n\tIt's a more type-safe alternative to `Dynamic`, because it doesn't\n\tsupport field access or operators and it's bound to monomorphs. So,\n\tto work with the actual value, it needs to be explicitly promoted\n\tto another type.\n")
                        .SetName("Issue 104. abstract");
                }
            }

            [Test, TestCaseSource(nameof(ParseClass_Issue104TestCases))]
            public string ParseFile_Issue104(string name, FlagType flags)
            {
                var visibleExternalElements = ASContext.Context.GetVisibleExternalElements();
                var result = visibleExternalElements.Search(name, flags, 0);
                return result.Comments;
            }

            static IEnumerable<TestCaseData> ExpressionEndPositionTestCases
            {
                get
                {
                    yield return new TestCaseData("test(); //")
                        .Returns("test()".Length);
                    yield return new TestCaseData("test[1]; //")
                        .Returns("test".Length);
                    yield return new TestCaseData("test['1']; //")
                        .Returns("test".Length);
                    yield return new TestCaseData("x:10, y:10}; //")
                        .Returns("x".Length);
                    yield return new TestCaseData("test()); //")
                        .Returns("test()".Length);
                    yield return new TestCaseData("test()]; //")
                        .Returns("test()".Length);
                    yield return new TestCaseData("test()}; //")
                        .Returns("test()".Length);
                    yield return new TestCaseData("test[1]); //")
                        .Returns("test".Length);
                    yield return new TestCaseData("test(), 1, 2); //")
                        .Returns("test()".Length);
                    yield return new TestCaseData("test().test().test().test; //")
                        .Returns("test()".Length);
                    yield return new TestCaseData("test(')))))').test(']]]]]]]]').test('}}}}}}}}}}').test; //")
                        .Returns("test(')))))')".Length);
                    yield return new TestCaseData("test.test(')))))')\n.test(']]]]]]]]')\n.test('}}}}}}}}}}')\n.test; //")
                        .Returns("test".Length);
                    yield return new TestCaseData("test(function() return 10); //")
                        .Returns("test(function() return 10)".Length);
                    yield return new TestCaseData("test(1, 2, 3); //")
                        .Returns("test(1, 2, 3)".Length);
                    yield return new TestCaseData("test( new Map<K,V> ); //")
                        .Returns("test( new Map<K,V> )".Length);
                    yield return new TestCaseData("Map<K,V> ; //")
                        .Returns("Map".Length);
                    yield return new TestCaseData("test; //")
                        .Returns("test".Length);
                    yield return new TestCaseData("test.a.b.c.d().e(1).f('${g}').h([1 => {x:1}]); //")
                        .Returns("test".Length);
                    yield return new TestCaseData("test(/*12345*/); //")
                        .Returns("test(/*12345*/)".Length);
                }
            }

            [Test, TestCaseSource(nameof(ExpressionEndPositionTestCases))]
            public int ExpressionEndPosition(string sourceText) => ExpressionEndPosition(sci, sourceText);
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
    }
}
