using System.Collections.Generic;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Managers;
using ScintillaNet;

//TODO: Sadly most of ASComplete is currently untestable in a proper way. Work on this branch solves it: https://github.com/Neverbirth/flashdevelop/tree/completionlist

namespace ASCompletion.Completion
{
    [TestFixture]
    public class ASCompleteTests : ASCompletionTests
    {
        static ASResult GetExpressionType(ScintillaControl sci, string sourceText)
        {
            SetSrc(sci, sourceText);
            return ASComplete.GetExpressionType(sci, sci.WordEndPosition(sci.CurrentPos, true));
        }

        static string GetExpression(ScintillaControl sci, string sourceText)
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

        static ComaExpression DisambiguateComa(ScintillaControl sci, string sourceText)
        {
            sci.Text = sourceText;
            return ASComplete.DisambiguateComa(sci, sourceText.Length, 0);
        }

        static int FindParameterIndex(ScintillaControl sci, string sourceText)
        {
            sci.Text = sourceText;
            sci.Colourise(0, -1);
            SnippetHelper.PostProcessSnippets(sci, 0);
            var pos = sci.CurrentPos - 1;
            var result = ASComplete.FindParameterIndex(sci, ref pos);
            Assert.AreNotEqual(-1, pos);
            return result;
        }

        static int ExpressionEndPosition(ScintillaControl sci, string sourceText)
        {
            SetSrc(sci, sourceText);
            return ASComplete.ExpressionEndPosition(sci, sci.CurrentPos);
        }

        protected static void OnChar(ScintillaControl sci, string sourceText, char addedChar, bool autoHide, bool hasCompletion)
        {
            var passed = true;
            var handler = Substitute.For<IEventHandler>();
            handler
                .When(it => it.HandleEvent(Arg.Any<object>(), Arg.Any<NotifyEvent>(), Arg.Any<HandlingPriority>()))
                .Do(it =>
                {
                    var e = it.ArgAt<NotifyEvent>(1);
                    if (e.Type != EventType.Command) return;
                    var de = (DataEvent)e;
                    if (de.Action != "ASCompletion.DotCompletion") return;
                    if (hasCompletion) passed = ((IList<ICompletionListItem>)de.Data).Count > 0;
                    else passed = ((IList<ICompletionListItem>)de.Data).Count == 0;
                });
            EventManager.AddEventHandler(handler, EventType.Command);
            SetSrc(sci, sourceText);
            ASContext.HasContext = true;
            Assert.AreEqual(hasCompletion, ASComplete.OnChar(sci, addedChar, autoHide));
            Assert.IsTrue(passed);
            EventManager.RemoveEventHandler(handler);
        }

        protected static string OnCharAndReplaceText(ScintillaControl sci, string sourceText, char addedChar, bool autoHide)
        {
            PluginBase.MainForm.CurrentDocument.IsEditable.Returns(true);
            var manager = UITools.Manager;
            SetSrc(sci, sourceText);
            ASContext.Context.CurrentClass.InFile.Context = ASContext.Context;
            ASContext.HasContext = true;
            //{ Update completion cache
            var ctx = ASContext.GetLanguageContext(sci.ConfigurationLanguage);
            var visibleExternalElements = ctx.GetVisibleExternalElements();
            ASContext.Context.GetVisibleExternalElements().Returns(visibleExternalElements);
            //}
            ASComplete.OnChar(sci, addedChar, autoHide);
            Assert.IsNotNullOrEmpty(CompletionList.SelectedLabel);
            CompletionList.OnInsert += ASComplete.HandleCompletionInsert;
            CompletionList.ReplaceText(sci, '\0');
            CompletionList.OnInsert -= ASComplete.HandleCompletionInsert;
            return sci.Text;
        }

        public class ActonScript3 : ASCompleteTests
        {
            static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

            static string GetFullPath(string fileName) => $"{nameof(ASCompletion)}.Test_Files.completion.as3.{fileName}.as";

            [TestFixtureSetUp]
            public void Setup() => SetAs3Features(sci);

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
                    yield return new TestCaseData(ReadAllText("GetExpressionType_issue1976_1"))
                        .Returns(new MemberModel("foo", "void", FlagType.Access | FlagType.Dynamic | FlagType.Function, Visibility.Public))
                        .SetName("Issue 1976. case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1976");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_issue1976_2"))
                        .Returns(new MemberModel("foo", "void", FlagType.Access | FlagType.Dynamic | FlagType.Function, Visibility.Public))
                        .SetName("Issue 1976. case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1976");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_issue1976_3"))
                        .Returns(new MemberModel("foo", "void", FlagType.Access | FlagType.Dynamic | FlagType.Function, Visibility.Public))
                        .SetName("Issue 1976. case 3")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1976");
                }
            }

            [Test, TestCaseSource(nameof(GetExpressionTypeTestCases))]
            public MemberModel GetExpressionType_Member(string sourceText)
            {
                var expr = GetExpressionType(sci, sourceText);
                return expr.Member;
            }

            static IEnumerable<TestCaseData> GetExpressionType_as_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_as_1"))
                        .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("('s' as String).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_as_2"))
                        .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("return ('s' as String).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_as_3"))
                        .Returns(new ClassModel { Name = "int", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("('s' as String).charAt(0).length.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_as_4"))
                        .Returns(new ClassModel { Name = "int", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("('...' as String).charAt(0).length.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_as_5"))
                        .Returns(new ClassModel { Name = "Array", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("('s' as String).split().");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_as_6"))
                        .Returns(new ClassModel { Name = "Function", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("('s' as String).split.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_as_7"))
                        .Returns(new ClassModel { Name = "Array", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("for each(var it:* in (a as Array).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_as_8"))
                        .Returns(new ClassModel { Name = "Array", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("for each(var it:* in (a as Array).concat([1]).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_as_9"))
                        .Returns(new ClassModel { Name = "Array", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("for each(var it:* in [1,2,3,4].");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_is_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_is_1"))
                        .Returns(new ClassModel { Name = "Boolean", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("('s' is String).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_is_2"))
                        .Returns(new ClassModel { Name = "Function", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("('s' is String).toString.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_is_3"))
                        .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("('s' is String).toString().");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_is_4"))
                        .Returns(new ClassModel { Name = "int", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("('s' is String).toString().length.");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_StringInitializer_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer_1"))
                        .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("\"\".");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer_2"))
                        .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("''.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_issue2029_1"))
                        .Returns(new ClassModel { Name = "int", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("'123'.length.")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2029");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_issue2029_2"))
                        .Returns(new ClassModel { Name = "Function", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("'123'.toString.")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2029");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_issue2029_3"))
                        .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("'123'.toString(10).")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2029");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_issue2029_4"))
                        .Returns(new ClassModel { Name = "int", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("'123'.toString(10).length.")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2029");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer_3"))
                        .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("\"...\".");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer_4"))
                        .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("'...'.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer_5"))
                        .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("'>.['.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer_6"))
                        .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("'.<'.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer_7"))
                        .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("'#RegExp'.");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_Vector_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_issue1383_1"))
                        .Returns(new ClassModel { Name = "Vector.<int>", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("new Vector.<int>()")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1383");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_issue1383_2"))
                        .Returns(new ClassModel { Name = "Vector", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("new Vector.<*>()")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1383");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_VectorInitializer_1"))
                        .Returns(new ClassModel { Name = "Vector.<int>", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("new <int>[].");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_VectorInitializer_2"))
                        .Returns(new ClassModel { Name = "int", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("new <int>[].length.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_VectorInitializer_3"))
                        .Returns(new ClassModel { Name = "Vector.<int>", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("new <int>[].concat(new <int>[1,2,3]).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_VectorInitializer_4"))
                        .Returns(new ClassModel { Name = "Function", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("new <int>[].concat(new <int>[1,2,3]).push.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_VectorInitializer_5"))
                        .Returns(new ClassModel { Name = "int", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("new Vector.<int>().length.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_VectorInitializer_6"))
                        .Returns(new ClassModel { Name = "Function", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("new Vector.<int>().push.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_VectorInitializer_7"))
                        .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("new Vector.<int>().join(',').");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_XML_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_XMLInitializer_1"))
                        .Returns(new ClassModel { Name = "XML", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("<xml/>.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_XMLInitializer_2"))
                        .Returns(new ClassModel { Name = "Boolean", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("<xml/>.contains(<xml/>).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_XMLInitializer_3"))
                        .Returns(new ClassModel { Name = "Function", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("<xml/>.contains.");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer"))
                        .Returns(new ClassModel { Name = "Array", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("[].");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_2"))
                        .Returns(new ClassModel { Name = "Object", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("[].concat([])[0].");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_3"))
                        .Returns(new ClassModel { Name = "uint", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("[].length.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_objectInitializer"))
                        .Returns(new ClassModel { Name = "Object", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("{}.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_SafeCast_1"))
                        .Returns(new ClassModel { Name = "String", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("String(v).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_SafeCast_2"))
                        .Returns(new ClassModel { Name = "Function", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("String(v).charAt.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_SafeCast_3"))
                        .Returns(new ClassModel { Name = "Array", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("String(v).split().");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_SafeCast_3"))
                        .Returns(new ClassModel { Name = "Array", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("String(v).split().length.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_typeof_1"))
                        .Returns(new ClassModel { Name = "Boolean", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("typeof Boolean(v).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_typeof_2"))
                        .Returns(new ClassModel { Name = "int", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("typeof Boolean(v).toString().length.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_typeof_3"))
                        .Returns(new ClassModel { Name = "int", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("typeof v.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_typeof_4"))
                        .Returns(new ClassModel { Name = "int", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("typeof v.length.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_typeof_5"))
                        .Returns(new ClassModel { Name = "int", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("typeof new Vector.<int>(10, true).length.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_typeof_6"))
                        .Returns(new ClassModel { Name = "uint", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("typeof [].length.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_typeof_7"))
                        .Returns(new ClassModel { Name = "Array", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("typeof [].");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_return_1"))
                        .Returns(new ClassModel { Name = "Array", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("return [].");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_return_2"))
                        .Returns(new ClassModel { Name = "uint", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore })
                        .SetName("return [].length.");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionTypeIssue2429TestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_issue2429_1"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore})
                        .SetName("0.1.toExponential(2).<complete>");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_issue2429_2"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore})
                        .SetName("5e-324.toExponential(2).<complete>");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_issue2429_3"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore})
                        .SetName("1.79e+308.toExponential(2).<complete>");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_issue2429_4"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore})
                        .SetName("2.225e-308.toExponential(2).<complete>");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_issue2429_5"))
                        .Returns(new ClassModel {Name = "Number", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore})
                        .SetName("2.225e-308.<complete>");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_issue2429_6"))
                        .Returns(new ClassModel {Name = "Number", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore})
                        .SetName("1.79e+308.<complete>");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_issue2429_7"))
                        .Returns(new ClassModel {Name = "Number", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore})
                        .SetName("5e-324.<complete>");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_issue2429_8"))
                        .Returns(new ClassModel {Name = "int", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore})
                        .SetName("-1.valueOf().<complete>");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_issue2429_9"))
                        .Returns(new ClassModel {Name = "uint", Flags = FlagType.Class, Access = Visibility.Public, InFile = FileModel.Ignore})
                        .SetName("0xFF0000.valueOf().<complete>");
                }
            }

            [
                Test,
                TestCaseSource(nameof(GetExpressionType_as_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_is_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_StringInitializer_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_Vector_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_XML_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionTypeIssue2429TestCases)),
            ]
            public ClassModel GetExpressionType_Type(string sourceText)
            {
                var expr = GetExpressionType(sci, sourceText);
                return expr.Type;
            }

            static IEnumerable<TestCaseData> GetExpressionTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionOfString"))
                            .Returns(";String")
                            .SetName("From String|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfNewString"))
                            .Returns("new String")
                            .SetName("From new String|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfNewString.charCodeAt"))
                            .Returns("new String().charCodeAt")
                            .SetName("From new String().charCodeAt|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfNewString.charCodeAt0.toString"))
                            .Returns("new String().charCodeAt(0).toString")
                            .SetName("From new String().charCodeAt(0).toString|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfStringInitializer.charCodeAt0.toString"))
                            .Returns(";\"string\".charCodeAt(0).toString")
                            .SetName("From \"string\".charCodeAt(0).toString|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfStringInitializer2.charCodeAt0.toString"))
                            .Returns(";'string'.charCodeAt(0).toString")
                            .SetName("From 'string'.charCodeAt(0).toString|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfEmptyStringInitializer"))
                            .Returns(";\"\"")
                            .SetName("From \"\"|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfEmptyStringInitializerSingleQuotes"))
                            .Returns(";''")
                            .SetName("From ''|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfStringInitializer"))
                            .Returns(";\"string\"")
                            .SetName("From \"string\"|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfStringInitializer2"))
                            .Returns(";\"{[(<string\"")
                            .SetName("From \"{[(<string\"|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfStringInitializer3"))
                            .Returns(";\"string>}])\"")
                            .SetName("From \"string>}])\"|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfGlobalFunctionString"))
                            .Returns(";")
                            .SetName("From String(\"string\")|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfObjectInitializer"))
                            .Returns(";{}")
                            .SetName("From {}|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfArrayInitializer"))
                            .Returns(";[]")
                            .SetName("From []|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfArrayInitializer.push"))
                            .Returns(";[].push")
                            .SetName("From [].push|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfTwoDimensionalArrayInitializer"))
                            .Returns(";[[], []]")
                            .SetName("From [[], []]|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfVectorInitializer"))
                            .Returns("new <int>[]")
                            .SetName("From new <int>[]|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfTwoDimensionalVectorInitializer"))
                            .Returns("new <Vector.<int>>[new <int>[]]")
                            .SetName("From new <Vector.<int>>[new <int>[]]|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfArrayAccess"))
                        .Returns(",[4,5,6][0][2]")
                        .SetName("From [[1,2,3], [4,5,6][0][2]|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfNewVector"))
                        .Returns("new Vector.<String>")
                        .SetName("From new Vector.<String>|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfNewTwoDimensionalVector"))
                        .Returns("new Vector.<Vector.<String>>")
                        .SetName("From new Vector.<Vector.<String>>|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfRegex"))
                        .Returns(";#RegExp")
                        .SetName("From /regex/g|")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1880");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfDigit"))
                        .Returns(";1")
                        .SetName("From 1|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfNumber"))
                        .Returns(";10.0")
                        .SetName("From 10.0|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfInt"))
                        .Returns("-1")
                        .SetName("From -1|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfBoolean"))
                        .Returns(";true")
                        .SetName("From true|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfXML"))
                        .Returns(";</>")
                        .SetName("<xml/>|");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_plus"))
                        .Returns("+1")
                        .SetName("1 + 1");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_minus"))
                        .Returns("-1")
                        .SetName("1 - 1");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_mul"))
                        .Returns("*1")
                        .SetName("1 * 1");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_division"))
                        .Returns("/1")
                        .SetName("1 / 1");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_increment"))
                        .Returns("++1")
                        .SetName("++1");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_increment2"))
                        .Returns(";1++")
                        .SetName("1++. case 1");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_increment3"))
                        .Returns(";1++")
                        .SetName("1++. case 2");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_increment4"))
                        .Returns(";a++")
                        .SetName("a++");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_increment5"))
                        .Returns("=getId()++")
                        .SetName("var id = getId()++");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_decrement"))
                        .Returns("--1")
                        .SetName("--1");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_decrement2"))
                        .Returns(";1--")
                        .SetName("1--. case 1");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_decrement3"))
                        .Returns(";1--")
                        .SetName("1--. case 2");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_decrement4"))
                        .Returns(";a--")
                        .SetName("a--");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_decrement5"))
                        .Returns("=getId()--")
                        .SetName("var id = getId()--");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_decrement6"))
                        .Returns("* ++1")
                        .SetName("5 * ++1");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_decrement7"))
                        .Returns("*1++")
                        .SetName("5 * 1++");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1908_typeof"))
                        .Returns("typeof 1")
                        .SetName("typeof 1");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1908_delete"))
                        .Returns("delete o[k]")
                        .SetName("delete o[k]");
                    yield return new TestCaseData(ReadAllText("GetExpression_operator_is"))
                        .Returns(";(\"s\" is String).")
                        .SetName("(\"s\" is String).|");
                    yield return new TestCaseData(ReadAllText("GetExpression_operator_as"))
                        .Returns(";(\"s\" as String).")
                        .SetName("(\"s\" as String).|");
                    yield return new TestCaseData(ReadAllText("GetExpression_return_operator_as"))
                        .Returns("return;(\"s\" as String).")
                        .SetName("return (\"s\" as String).|");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1954"))
                        .Returns(";re")
                        .SetName("function foo():Vector.<int> { re|");
                    yield return new TestCaseData("[].$(EntryPoint)")
                        .Returns(" [].")
                        .SetName("[].|");
                    yield return new TestCaseData("new <int>[].$(EntryPoint)")
                        .Returns("new <int>[].")
                        .SetName("new <int>[].|");
                    yield return new TestCaseData("new <int>[1,2,3,4].$(EntryPoint)")
                        .Returns("new <int>[1,2,3,4].")
                        .SetName("new <int>[1,2,3,4].|");
                    yield return new TestCaseData("new <*>[1,2,3,4].$(EntryPoint)")
                        .Returns("new <*>[1,2,3,4].")
                        .SetName("new <*>[1,2,3,4].|");
                    yield return new TestCaseData("new Vector.<*>().$(EntryPoint)")
                        .Returns("new Vector.<*>().")
                        .SetName("new Vector.<*>().|");
                    yield return new TestCaseData(">> 1$(EntryPoint)")
                        .Returns(">>1")
                        .SetName(">>1");
                    yield return new TestCaseData(">>> 1$(EntryPoint)")
                        .Returns(">>>1")
                        .SetName(">>>1");
                    yield return new TestCaseData("<< 1$(EntryPoint)")
                        .Returns("<<1")
                        .SetName("<<1");
                    yield return new TestCaseData("1 | 2$(EntryPoint)")
                        .Returns("|2")
                        .SetName("|2");
                    yield return new TestCaseData(" ~2$(EntryPoint)")
                        .Returns("~2")
                        .SetName("~2");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionIssue2429TestCases
            {
                get
                {
                    yield return new TestCaseData("1.79e+308$(EntryPoint)")
                        .Returns(" 1.79e+308")
                        .SetName("1.79e+308|");
                    yield return new TestCaseData("2.225e-308$(EntryPoint)")
                        .Returns(" 2.225e-308")
                        .SetName("2.225e-308|");
                    yield return new TestCaseData("5e-324$(EntryPoint)")
                        .Returns(" 5e-324")
                        .SetName("5e-324|");
                }
            }

            [
                Test, 
                TestCaseSource(nameof(GetExpressionTestCases)),
                TestCaseSource(nameof(GetExpressionIssue2429TestCases)),
            ]
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
                    yield return new TestCaseData("Test2 {\n}")
                        .Returns("Test2".Length);
                    yield return new TestCaseData("Test2{\n}")
                        .Returns("Test2".Length);
                    yield return new TestCaseData("[1] }\n  private function foo() {\n}")
                        .Returns("[1]".Length);
                    yield return new TestCaseData("'' }\n  private function foo() {\n}")
                        .Returns("''".Length);
                    yield return new TestCaseData("[123]\n trace(1)")
                        .Returns("[123]".Length);
                }
            }

            [Test, TestCaseSource(nameof(ExpressionEndPositionTestCases))]
            public int ExpressionEndPosition(string sourceText) => ExpressionEndPosition(sci, sourceText);

            static IEnumerable<TestCaseData> OnCharTestCases
            {
                get
                {
                    yield return new TestCaseData("OnChar_1", '.', false, true)
                        .SetName("this.|");
                    yield return new TestCaseData("OnChar_3", '.', false, true)
                        .SetName("''.|");
                    yield return new TestCaseData("OnChar_4", '.', false, true)
                        .SetName("[].|");
                    yield return new TestCaseData("OnChar_7", '.', false, false)
                        .SetName("1.|");
                    yield return new TestCaseData("OnChar_8", '.', false, true)
                        .SetName("true.|")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2105");
                    yield return new TestCaseData("OnChar_9", '.', false, true)
                        .SetName("0xFF0000.|");
                    yield return new TestCaseData("OnChar_10", '.', false, true)
                        .SetName("{}.|");
                    yield return new TestCaseData("OnChar_2", '.', false, false)
                        .Ignore("Completion shouldn't work for this case.")
                        .SetName("this.|. inside static function");
                }
            }

            static IEnumerable<TestCaseData> OnCharIssue2105TestCases
            {
                get
                {
                    yield return new TestCaseData("OnCharIssue2105_5", '.', false, false)
                        .SetName("'.|' Issue2105. Case 1.")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2105");
                    yield return new TestCaseData("OnCharIssue2105_6", '.', false, false)
                        .SetName("\".|\" Issue2105. Case 2.")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2105");
                }
            }

            [
                Test,
                TestCaseSource(nameof(OnCharTestCases)),
                TestCaseSource(nameof(OnCharIssue2105TestCases)),
            ]
            public void OnChar(string fileName, char addedChar, bool autoHide, bool hasCompletion) => OnChar(sci, ReadAllText(fileName), addedChar, autoHide, hasCompletion);

            static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2076TestCases
            {
                get
                {
                    yield return new TestCaseData("BeforeOnCharIssue2076_1", ' ', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_1"))
                        .SetName("var v:Sprite = new | ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_2", ' ', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_2"))
                        .SetName("override | ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_3", ' ', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_3"))
                        .SetName("extends | ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_4", ' ', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_4"))
                        .SetName("implements | ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_5", ':', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_5"))
                        .SetName("function foo(v:| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_6", '.', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_6"))
                        .SetName("function foo(v:flash.display.| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_7", '.', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_7"))
                        .SetName("this.| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_8", '.', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_8"))
                        .SetName("[].| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_9", '.', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_9"))
                        .SetName("''.| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_10", '.', true)
                        .Returns(ReadAllText("AfterOnCharIssue2076_10"))
                        .SetName("0x1.| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_11", '.', true)
                        .Returns(ReadAllText("AfterOnCharIssue2076_11"))
                        .SetName("true.| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_12", '.', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_12"))
                        .SetName("(v as String).| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_13", '.', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_13"))
                        .SetName("(v is String).| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_14", '.', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_14"))
                        .SetName("(v is String).toString().| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_15", '.', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_15"))
                        .SetName("new Vector.<int>(10, true).| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_16", '.', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_16"))
                        .SetName("new Vector.<int>(10, true).join(',').| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_17", '.', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_17"))
                        .SetName("new <int>[1,2,3].| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_18", '.', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_18"))
                        .SetName("new <int>[1,2,3].join(',').| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_19", '.', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_19"))
                        .SetName("String.| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_20", '.', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_20"))
                        .SetName("typeof String.| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_21", '.', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_21"))
                        .SetName("String.fromCharCode(0).| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_22", '.', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_22"))
                        .SetName("String.fromCharCode.| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_23", '.', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_23"))
                        .SetName("super.| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_24", '.', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_24"))
                        .SetName("[super.| ]")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                    yield return new TestCaseData("BeforeOnCharIssue2076_25", '.', false)
                        .Returns(ReadAllText("AfterOnCharIssue2076_25"))
                        .SetName("new Date().| ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2076");
                }
            }

            static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2134TestCases
            {
                get
                {
                    yield return new TestCaseData("BeforeOnCharIssue2134_1", ' ', false)
                        .Returns(ReadAllText("AfterOnCharIssue2134_1"))
                        .SetName("override | ")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2134");
                }
            }

            static IEnumerable<TestCaseData> OnCharAndReplaceTextIssue2282TestCases
            {
                get
                {
                    yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2282_1", ' ', false)
                        .Returns(ReadAllText("AfterOnCharAndReplaceTextIssue2282_1"))
                        .SetName("override |. function(v:Function/*(v:int):int*/). Issue 2282. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2282");
                    yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2282_2", ' ', false)
                        .Returns(ReadAllText("AfterOnCharAndReplaceTextIssue2282_2"))
                        .SetName("override |. function set foo(v:Function/*(v:int):int*/). Issue 2282. Case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2282");
                    yield return new TestCaseData("BeforeOnCharAndReplaceTextIssue2282_3", ' ', false)
                        .Returns(ReadAllText("AfterOnCharAndReplaceTextIssue2282_3"))
                        .SetName("override |. function get foo():Function/*(v:int):int*/. Issue 2282. Case 3")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2282");
                }
            }

            [
                Test,
                TestCaseSource(nameof(OnCharAndReplaceTextIssue2076TestCases)),
                TestCaseSource(nameof(OnCharAndReplaceTextIssue2134TestCases)),
                TestCaseSource(nameof(OnCharAndReplaceTextIssue2282TestCases)),
            ]
            public string OnCharAndReplaceText(string fileName, char addedChar, bool autoHide) => OnCharAndReplaceText(sci, ReadAllText(fileName), addedChar, autoHide);

            static IEnumerable<TestCaseData> GetToolTipTextTestCases
            {
                get
                {
                    yield return new TestCaseData("GetToolTipText_1")
                        .SetName("new B|(). Case 1. Class without constructor")
                        .Returns("public Bar ()\n[COLOR=Black]in Bar[/COLOR]");
                    yield return new TestCaseData("GetToolTipText_2")
                        .SetName("new B|(). Case 2. Class with explicit constructor")
                        .Returns("public Bar (v:int)\n[COLOR=Black]in Bar[/COLOR]");
                    yield return new TestCaseData("GetToolTipText_4")
                        .SetName("new B|(). Case 4. Class with explicit constructor")
                        .Returns("public Bar ()\n[COLOR=Black]in Bar[/COLOR]");
                    yield return new TestCaseData("GetToolTipText_3")
                        .SetName("new B|(). Case 3. Class with implicit constructor")
                        .Returns("public Bar ()\n[COLOR=Black]in Bar[/COLOR]");
                }
            }

            [Test, TestCaseSource(nameof(GetToolTipTextTestCases))]
            public string GetToolTipText(string fileName)
            {
                SetSrc(sci, ReadAllText(fileName));
                var expr = ASComplete.GetExpressionType(sci, sci.CurrentPos, false, true);
                return ASComplete.GetToolTipText(expr);
            }
        }

        public class Haxe : ASCompleteTests
        {
            static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

            static string GetFullPath(string fileName) => $"{nameof(ASCompletion)}.Test_Files.completion.haxe.{fileName}.hx";

            [TestFixtureSetUp]
            public void Setup() => SetHaxeFeatures(sci);

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

            static IEnumerable<TestCaseData> GetExpressionType_untyped_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_untyped_1"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class})
                        .SetName("untyped 's'.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_untyped_2"))
                        .Returns(new ClassModel {Name = "Array<T>", Flags = FlagType.Class})
                        .SetName("untyped [].");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_cast_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_cast"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class})
                        .SetName("cast('s', String).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_cast_2"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class})
                        .SetName("return cast('s', String).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_cast_3"))
                        .Returns(new ClassModel {Name = "Int", Flags = FlagType.Class})
                        .SetName("cast('s', String).length");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_cast_4"))
                        .Returns(new ClassModel {Name = "Int", Flags = FlagType.Class})
                        .SetName("cast('s', String).charAt(0).length");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_cast_5"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class})
                        .SetName("cast('...', String).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_cast_6"))
                        .Returns(new ClassModel {Name = "Int", Flags = FlagType.Class})
                        .SetName("cast('...', String).charAt(0).length.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_cast_7"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class})
                        .SetName("cast(', Int', String).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_cast_8"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class})
                        .SetName("cast ( 's' , String ).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_cast_9"))
                        .Returns(new ClassModel {Name = "Array<String>", Flags = FlagType.Class})
                        .SetName("cast('s', String).split('').");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_cast_10"))
                        .Returns(new ClassModel {Name = "Function", Flags = FlagType.Class})
                        .SetName("cast('s', String).charAt.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_cast_11"))
                        .Returns(new ClassModel {Name = "Array<T>", Flags = FlagType.Class})
                        .SetName("cast [].");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_cast_12"))
                        .Returns(new ClassModel {Name = "Function", Flags = FlagType.Class})
                        .SetName("cast [].concat.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_cast_13"))
                        .Returns(new ClassModel {Name = "Int", Flags = FlagType.Class})
                        .SetName("cast [].concat([1]).length.");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_is_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_is_1"))
                        .Returns(new ClassModel {Name = "Bool", Flags = FlagType.Class})
                        .SetName("('s' is String).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_is_2"))
                        .Returns(new ClassModel {Name = "Bool", Flags = FlagType.Class})
                        .SetName("(' is ' is String).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_is_3"))
                        .Returns(new ClassModel {Name = "Bool", Flags = FlagType.Class})
                        .SetName("('v is Int' is String).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_is_4"))
                        .Returns(new ClassModel {Name = "Bool", Flags = FlagType.Class})
                        .SetName("('(v is Int)' is String).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_is_5"))
                        .Returns(new ClassModel {Name = "Bool", Flags = FlagType.Class})
                        .SetName("( 's' is String ).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_is_6"))
                        .Returns(new ClassModel {Name = "Bool", Flags = FlagType.Class})
                        .SetName("switch ( 's' is String ).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_is_7"))
                        .Returns(new ClassModel {Name = "Bool", Flags = FlagType.Class})
                        .SetName("return ( 's' is String ).");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_ArrayInitializer_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_1"))
                        .Returns(new ClassModel {Name = "Array<T>", Flags = FlagType.Class})
                        .SetName("[].");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_2"))
                        .Returns(new ClassModel {Name = "Array<T>", Flags = FlagType.Class})
                        .SetName("['...'].");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_3"))
                        .Returns(new ClassModel {Name = "Array<T>", Flags = FlagType.Class})
                        .SetName("['=>'].");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_4"))
                        .Returns(new ClassModel {Name = "Array<T>", Flags = FlagType.Class})
                        .SetName("[[1 => 2], [2 => 3]].");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_5"))
                        .Returns(new ClassModel {Name = "Int", Flags = FlagType.Class})
                        .SetName("[].length.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_6"))
                        .Returns(new ClassModel {Name = "Function", Flags = FlagType.Class})
                        .SetName("[].concat.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_7"))
                        .Returns(new ClassModel {Name = "Array<T>", Flags = FlagType.Class})
                        .SetName("[].concat([]).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_8"))
                        .Returns(new ClassModel {Name = "Int", Flags = FlagType.Class})
                        .SetName("[].concat([]).length.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_9"))
                        .Returns(new ClassModel {Name = "Int", Flags = FlagType.Class})
                        .SetName("[].concat([])[0].");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_10"))
                        .Returns(new ClassModel {Name = "Array<T>", Flags = FlagType.Class})
                        .SetName("return [].");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_11"))
                        .Returns(new ClassModel {Name = "Array<T>", Flags = FlagType.Class})
                        .SetName("switch [].");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_12"))
                        .Returns(new ClassModel {Name = "Array<T>", Flags = FlagType.Class})
                        .SetName("for(it in [].");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_13"))
                        .Returns(new ClassModel {Name = "Array<T>", Flags = FlagType.Class})
                        .SetName("var l = try [1]. catch(e:Dynamic) 1;");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_14"))
                        .Returns(new ClassModel {Name = "Function", Flags = FlagType.Class})
                        .SetName("var l = try [1].concat. catch(e:Dynamic) 1;");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_15"))
                        .Returns(new ClassModel {Name = "Array<T>", Flags = FlagType.Class})
                        .SetName("case [1, 2].");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_16"))
                        .Returns(new ClassModel {Name = "Array<T>", Flags = FlagType.Class})
                        .SetName("var a = [1, 2].");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_17"))
                        .Returns(new ClassModel {Name = "Function", Flags = FlagType.Class})
                        .SetName("[1 => [1].concat.]");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_arrayInitializer_18"))
                        .Returns(new ClassModel {Name = "Function", Flags = FlagType.Class})
                        .SetName("{c:[1].concat.}");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_MapInitializer_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_mapInitializer_1"))
                        .Returns(new ClassModel {Name = "Map<K, V>", Flags = FlagType.Class})
                        .SetName("[1=>1].");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_mapInitializer_2"))
                        .Returns(new ClassModel {Name = "Map<K, V>", Flags = FlagType.Class})
                        .SetName("['...' => 1, '1' => '...'].");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_StringInitializer_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class})
                        .SetName("\"\".");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer_2"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class})
                        .SetName("''.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer_3"))
                        .Returns(new ClassModel {Name = "Int", Flags = FlagType.Class})
                        .SetName("''.length.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer_4"))
                        .Returns(new ClassModel {Name = "Function", Flags = FlagType.Class})
                        .SetName("''.charAt.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer_5"))
                        .Returns(new ClassModel {Name = "Array<String>", Flags = FlagType.Class})
                        .SetName("''.split('').");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer_6"))
                        .Returns(new ClassModel {Name = "Int", Flags = FlagType.Class})
                        .SetName("''.split('').length.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer_7"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class})
                        .SetName("''.split('')[0].");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer_8"))
                        .Returns(new ClassModel {Name = "Int", Flags = FlagType.Class})
                        .SetName("'...'.length.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_stringInitializer_9"))
                        .Returns(new ClassModel {Name = "Int", Flags = FlagType.Class})
                        .SetName("\"...\".length.");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_new_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_new_1"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class})
                        .SetName("new String('').");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_new_2"))
                        .Returns(new ClassModel {Name = "Function", Flags = FlagType.Class})
                        .SetName("new String('').charAt.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_new_3"))
                        .Returns(new ClassModel {Name = "Array<String>", Flags = FlagType.Class})
                        .SetName("new String('').split('').");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_InferVariableTypeIssue2362TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_issue2362_1"))
                        .Returns(new ClassModel {Name = "Dynamic", Flags = FlagType.Class})
                        .SetName("var фывValue. = ''");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_issue2362_2"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class})
                        .SetName("function foo(?фывValue. = '')");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_issue2362_3"))
                        .Returns(new ClassModel {Name = "VariableType", Flags = FlagType.Class | FlagType.TypeDef})
                        .SetName("typedef фывVariableType. = String");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_InferVariableTypeIssue2373TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_issue2373_1"))
                        .Returns(new ClassModel {Name = "Issue2373Foo", Flags = FlagType.Class | FlagType.Abstract})
                        .SetName("var One = 1. Issue 2373. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2373");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_issue2373_2"))
                        .Returns(new ClassModel {Name = "Issue2373Bar", Flags = FlagType.Class | FlagType.Abstract})
                        .SetName("var One = 1. Issue 2373. Case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2373");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_InferVariableTypeIssue2401TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_issue2401_1"))
                        .Returns(new ClassModel {Name = "Null<Dynamic>", Flags = FlagType.Class | FlagType.Abstract})
                        .SetName("function foo(?v| = null). Issue 2401. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2401");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_ArrayAccess_issue2471_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ArrayAccess_issue2471_1"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class})
                        .SetName("a[0].<complete> Issue 2471. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2471");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ArrayAccess_issue2471_2"))
                        .Returns(new ClassModel {Name = "Function", Flags = FlagType.Class})
                        .SetName("a[0].<complete> Issue 2471. Case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2471");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ArrayAccess_issue2471_3"))
                        .Returns(new ClassModel {Name = "Array<String->String>", Flags = FlagType.Class})
                        .SetName("a[0].<complete> Issue 2471. Case 3")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2471");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ArrayAccess_issue2471_4"))
                        .Returns(new ClassModel {Name = "Array<Array<String->String>->String>", Flags = FlagType.Class})
                        .SetName("a[0].<complete> Issue 2471. Case 4")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2471");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_ParameterizedFunction_issue2203_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ParameterizedFunction_issue2203_1"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class, InFile = FileModel.Ignore})
                        .SetName("foo<T>('string').<complete> Issue 2203. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2203");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ParameterizedFunction_issue2203_2"))
                        .Returns(new ClassModel {Name = "Bool", Flags = FlagType.Class, InFile = FileModel.Ignore})
                        .SetName("foo<T>(_, bool, ?_, ?_).<complete> Issue 2203. Case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2203");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ParameterizedFunction_issue2203_3"))
                        .Returns(new ClassModel {Name = "Float", Flags = FlagType.Class, InFile = FileModel.Ignore})
                        .SetName("foo<K, T, R1, R2>(_, 1.0, ?_, ?_).<complete> Issue 2203. Case 3")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2203");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ParameterizedFunction_issue2203_4"))
                        .Returns(new ClassModel {Name = "Float", Flags = FlagType.Class, InFile = FileModel.Ignore})
                        .SetName("foo<K, T, R1, R2>(_, localVar, ?_, ?_).<complete> Issue 2203. Case 4")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2203");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_ParameterizedFunction_issue2487_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ParameterizedFunction_issue2487_1"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class, InFile = FileModel.Ignore})
                        .SetName("(foo<T>(''):Array<T>)[0].<complete> Issue 2487. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2487");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_ParameterizedFunction_issue2499_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ParameterizedFunction_issue2499_1"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class, InFile = FileModel.Ignore})
                        .SetName("(foo<T>((String:Null<T>)):<T>).<complete> Issue 2499. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2499");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ParameterizedFunction_issue2499_2"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class, InFile = FileModel.Ignore})
                        .SetName("(foo<T>((String:Dynamic<T>)):<T>).<complete> Issue 2499. Case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2499");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ParameterizedFunction_issue2499_3"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class, InFile = FileModel.Ignore})
                        .SetName("(foo<T>((String:Class<T>)):<T>).<complete> Issue 2499. Case 3")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2499");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ParameterizedFunction_issue2499_4"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class, InFile = FileModel.Ignore})
                        .SetName("(foo<T:{}>((String:Class<T>)):<T>).<complete> Issue 2499. Case 4")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2499");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ParameterizedFunction_issue2499_5"))
                        .Returns(new ClassModel {Name = "Class<String>", Flags = FlagType.Class | FlagType.Abstract, InFile = FileModel.Ignore})
                        .SetName("Type.getClass(String).<complete> Issue 2499. Case 5")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2499");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_ParameterizedFunction_issue2503_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ParameterizedFunction_issue2503_1"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class, InFile = FileModel.Ignore})
                        .SetName("a[Some.Value].<complete> Issue 2503. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2503");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionType_ParameterizedFunction_issue2505_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ParameterizedFunction_issue2505_1"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class, InFile = FileModel.Ignore})
                        .SetName("(v:T).<complete> Issue 2505. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2505");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ParameterizedFunction_issue2505_2"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class, InFile = FileModel.Ignore})
                        .SetName("(v:T).<complete> Issue 2505. Case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2505");
                }
            }
            static IEnumerable<TestCaseData> GetExpressionType_ParameterizedClass_issue2536_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ParameterizedClass_issue2536_1"))
                        .Returns(new ClassModel {Name = "A2536_1", Flags = FlagType.Class, InFile = FileModel.Ignore})
                        .SetName("(v:T).<complete> Issue 2536. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2536");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ParameterizedClass_issue2536_2"))
                        .Returns(new ClassModel {Name = "B2536_2", Flags = FlagType.Class, InFile = FileModel.Ignore})
                        .SetName("(v:T).<complete> Issue 2536. Case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2536");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_ParameterizedClass_issue2536_3"))
                        .Returns(new ClassModel {Name = "A2536_3", Flags = FlagType.Class, InFile = FileModel.Ignore})
                        .SetName("(v:T).<complete> Issue 2536. Case 3")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2536");
                }
            }

            [
                Test,
                TestCaseSource(nameof(GetExpressionType_untyped_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_cast_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_is_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_ArrayInitializer_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_MapInitializer_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_StringInitializer_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_new_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_InferVariableTypeIssue2362TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_InferVariableTypeIssue2373TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_InferVariableTypeIssue2401TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_ArrayAccess_issue2471_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_ParameterizedFunction_issue2203_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_ParameterizedFunction_issue2487_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_ParameterizedFunction_issue2499_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_ParameterizedFunction_issue2503_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_ParameterizedFunction_issue2505_TypeTestCases)),
                TestCaseSource(nameof(GetExpressionType_ParameterizedClass_issue2536_TypeTestCases)),
            ]
            public ClassModel GetExpressionType_Type(string sourceText)
            {
                var expr = GetExpressionType(sci, sourceText);
                return expr.Type;
            }

            static IEnumerable<TestCaseData> GetExpressionTypeSDK_340_TypeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_typecheck"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class})
                        .SetName("('s':String).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_typecheck_2"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class})
                        .SetName("return ('s':String).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_typecheck_3"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class})
                        .SetName("return ('...':String).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_typecheck_4"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class})
                        .SetName("('...' : String).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_typecheck_5"))
                        .Returns(new ClassModel {Name = "String", Flags = FlagType.Class})
                        .SetName("('v:Int' : String).");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_typecheck_6"))
                        .Returns(new ClassModel {Name = "Int", Flags = FlagType.Class})
                        .SetName("('s':String).charAt(0).length.");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_typecheck_7"))
                        .Returns(new ClassModel {Name = "Array<String>", Flags = FlagType.Class})
                        .SetName("('s':String).split('').");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_Type_typecheck_8"))
                        .Returns(new ClassModel {Name = "Function", Flags = FlagType.Class})
                        .SetName("('s':String).split.");
                }
            }

            [Test, TestCaseSource(nameof(GetExpressionTypeSDK_340_TypeTestCases))]
            public ClassModel GetExpressionType_Type_SDK_340(string sourceText)
            {
                ASContext.Context.Settings.InstalledSDKs = new[] {new InstalledSDK {Path = PluginBase.CurrentProject.CurrentSDK, Version = "3.4.0"}};
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
                    yield return new TestCaseData(ReadAllText("GetExpressionOfNewMap"))
                        .Returns("new Map<String, Int>")
                        .SetName("From new Map<String, Int>|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfNewMap2"))
                        .Returns("new Map<Map<String, Int>, Int>")
                        .SetName("From new Map<Map<String, Int>, Int>|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfNewMap3"))
                        .Returns("new Map<String, Array<Map<String, Int>>>")
                        .SetName("From new Map<String, Array<Map<String, Int>>>|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfNewMap4"))
                        .Returns("new Map<String, Array<Int->Int->Int>>")
                        .SetName("From new Map<String, Array<Int->Int->Int>>|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfMapInitializer"))
                        .Returns(";[\"1\" => 1, \"2\" => 2]")
                        .SetName("From [\"1\" => 1, \"2\" => 2]|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfRegex"))
                        .Returns(";g")
                        .SetName("~/regex/g|")
                        .Ignore("https://github.com/fdorg/flashdevelop/issues/1880");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfNewArray"))
                        .Returns("new Array<Int>")
                        .SetName("From new Array<Int>|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfNewArray2"))
                        .Returns("new Array<{x:Int, y:Int}>")
                        .SetName("From new Array<{x:Int, y:Int}>|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfNewArray3"))
                        .Returns("new Array<{name:String, params:Array<Dynamic>}>")
                        .SetName("From new Array<{name:String, params:Array<Dynamic>}>|");
                    yield return new TestCaseData(ReadAllText("GetExpressionOfStringInterpolation.charAt"))
                        .Returns(";'result: ${1 + 2}'.charAt")
                        .SetName("'result: ${1 + 2}'.charAt");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_plus"))
                        .Returns("+1")
                        .SetName("1 + 1");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_minus"))
                        .Returns("-1")
                        .SetName("1 - 1");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_mul"))
                        .Returns("*1")
                        .SetName("1 * 1");
                    yield return new TestCaseData(ReadAllText("GetExpression_issue1749_division"))
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
                            .SetName("var id = getId()++|");
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
                            .SetName("var id = getId()--|");
                    yield return
                        new TestCaseData(ReadAllText("GetExpression_issue1954"))
                            .Returns(";re")
                            .SetName("function foo():Array<Int> { re|");
                    yield return
                        new TestCaseData("(v:String).$(EntryPoint)")
                            .Returns(" (v:String).")
                            .SetName("(v:String).|");
                    yield return
                        new TestCaseData("(v:Iterable<Dynamic>).$(EntryPoint)")
                            .Returns(" (v:Iterable<Dynamic>).")
                            .SetName("(v:Iterable<Dynamic>).|");
                    yield return
                        new TestCaseData("(v:{x:Int, y:Int}).$(EntryPoint)")
                            .Returns(" (v:{x:Int, y:Int}).")
                            .SetName("(v:{x:Int, y:Int}).|");
                    yield return
                        new TestCaseData("(v:{x:Int, y:Int->Array<Int>}).$(EntryPoint)")
                            .Returns(" (v:{x:Int, y:Int->Array<Int>}).")
                            .SetName("(v:{x:Int, y:Int->Array<Int>}).|");
                    yield return
                        new TestCaseData("return (v:{x:Int, y:Int->Array<Int>}).$(EntryPoint)")
                            .Returns("return;(v:{x:Int, y:Int->Array<Int>}).")
                            .SetName("return (v:{x:Int, y:Int->Array<Int>}).|");
                    yield return
                        new TestCaseData("[(v:{x:Int, y:Int->Array<Int>}).$(EntryPoint)")
                            .Returns(";(v:{x:Int, y:Int->Array<Int>}).")
                            .SetName("[(v:{x:Int, y:Int->Array<Int>}).|");
                    yield return
                        new TestCaseData("${(v:{x:Int, y:Int->Array<Int>}).$(EntryPoint)")
                            .Returns(";(v:{x:Int, y:Int->Array<Int>}).")
                            .SetName("${(v:{x:Int, y:Int->Array<Int>}).|");
                    yield return new TestCaseData("case _: (v:{x:Int, y:Int->Array<Int>}).$(EntryPoint)")
                        .Returns(":(v:{x:Int, y:Int->Array<Int>}).")
                        .SetName("case _: (v:{x:Int, y:Int->Array<Int>}).|");
                    yield return new TestCaseData("function foo(Math.random() > .5 || Math.random() < 0.5 ? {x:10, y:10} : null).$(EntryPoint)")
                        .Returns("function foo(Math.random() > .5 || Math.random() < 0.5 ? {x:10, y:10} : null).")
                        .SetName("function foo(Math.random() > .5 || Math.random() < 0.5 ? {x:10, y:10} : null).|");
                    yield return new TestCaseData("function foo(1 << 2).$(EntryPoint)")
                        .Returns("function foo(1 << 2).")
                        .SetName("function foo(1 << 2).|");
                    yield return new TestCaseData("function foo(1 >> 2).$(EntryPoint)")
                        .Returns("function foo(1 >> 2).")
                        .SetName("function foo(1 >> 2).|");
                    yield return new TestCaseData("function foo(1 >>> 3).$(EntryPoint)")
                        .Returns("function foo(1 >>> 3).")
                        .SetName("function foo(1 >>> 3).|");
                    yield return new TestCaseData("cast(v, String).charAt(0).charAt(1).charAt(2).charAt(3).charAt(4).charAt(5).charAt(6).charAt(7).charAt(8).charAt(9).charAt(10).charAt(11).$(EntryPoint)")
                        .Returns("cast;(v, String).charAt(0).charAt(1).charAt(2).charAt(3).charAt(4).charAt(5).charAt(6).charAt(7).charAt(8).charAt(9).charAt(10).charAt(11).")
                        .SetName("cast(v, String).charAt(0).charAt(1).charAt(2).charAt(3).charAt(4).charAt(5).charAt(6).charAt(7).charAt(8).charAt(9).charAt(10).charAt(11).|")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2108");
                    yield return new TestCaseData("cast(v, String).charAt.$(EntryPoint)")
                        .Returns("cast;(v, String).charAt.")
                        .SetName("cast(v, String).charAt.|")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2108");
                }
            }

            static IEnumerable<TestCaseData> GetExpressionIssue2386TestCases
            {
                get
                {
                    yield return new TestCaseData("cast(v, String).charAt(1), '$(EntryPoint)")
                        .Returns(" ")
                        .SetName("cast(v, String).charAt(1), '|")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2386");
                    yield return new TestCaseData("cast(v, String).charAt(1), \"$(EntryPoint)")
                        .Returns(" ")
                        .SetName("cast(v, String).charAt(1), \"|")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2386");
                }
            }

            [
                Test,
                TestCaseSource(nameof(GetExpressionTestCases)),
                TestCaseSource(nameof(GetExpressionIssue2386TestCases)),
            ]
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
                    yield return new TestCaseData("foo($(EntryPoint));function foo();")
                        .Returns(0);
                    yield return new TestCaseData("foo(1, $(EntryPoint));function foo(x:Int, y:Int);")
                        .Returns(1);
                    yield return new TestCaseData("foo([1,2,3,4,5], $(EntryPoint));function foo(x:Array<Int>, y:Int);")
                        .Returns(1);
                    yield return new TestCaseData("foo(new Array<Int>(), $(EntryPoint));function foo(x:Array<Int>, y:Int);")
                        .Returns(1);
                    yield return new TestCaseData("foo(new Map<Int, String>(), $(EntryPoint));function foo(x:Map<Int, String>, y:Int);")
                        .Returns(1);
                    yield return new TestCaseData("foo({x:Int, y:Int}, $(EntryPoint));function foo(x:{x:Int, y:Int}, y:Int);")
                        .Returns(1);
                    yield return new TestCaseData("foo(',,,,', $(EntryPoint));function foo(s:String, y:Int);")
                        .Returns(1);
                    yield return new TestCaseData("foo(\",,,,\", $(EntryPoint));function foo(s:String, y:Int);")
                        .Returns(1);
                    yield return new TestCaseData("foo(\"\\ \", $(EntryPoint));function foo(s:String, y:Int);")
                        .Returns(1);
                    yield return new TestCaseData("foo(0, ';', $(EntryPoint));function foo(i:Int, s:String, y:Int);")
                        .Returns(2);
                    yield return new TestCaseData("foo(0, '}}}', $(EntryPoint));function foo(i:Int, s:String, y:Int);")
                        .Returns(2);
                    yield return new TestCaseData("foo(0, '<<>>><{}>}->({[]})>><<,,;>><<', $(EntryPoint));function foo(i:Int, s:String, y:Int);")
                        .Returns(2);
                    yield return new TestCaseData("foo(0, '(', $(EntryPoint));function foo(i:Int, s:String, y:Int);")
                        .Returns(2);
                    yield return new TestCaseData("foo(0, ')', $(EntryPoint));function foo(i:Int, s:String, y:Int);")
                        .Returns(2);
                    yield return new TestCaseData("foo(0, {c:Array(<Int>->Map<String, Int>)->Void}, $(EntryPoint));function foo(i:Int, s:Dynamic, y:Int);")
                        .Returns(2);
                    yield return new TestCaseData("foo(0, function(i:Int, s:String) {return Std.string(i) + ', ' + s;}, $(EntryPoint));function foo(i:Int, s:Dynamic, y:Int);")
                        .Returns(2);
                    yield return new TestCaseData("foo(0, function() {var i = 1, j = 2; return i + j;}, $(EntryPoint));function foo(i:Int, s:Dynamic, y:Int);")
                        .Returns(2);
                    yield return new TestCaseData("foo([1 => 1], [1, $(EntryPoint));")
                        .Returns(1);
                    yield return new TestCaseData("foo([1 => 1], {x:1, y:$(EntryPoint));")
                        .Returns(1);
                    yield return new TestCaseData("foo([1 => 1], bar({x:1, y:$(EntryPoint)));")
                        .Returns(0);
                    yield return new TestCaseData("foo([1 => 1], $(EntryPoint));")
                        .Returns(1)
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/764");
                    yield return new TestCaseData("foo([for(i in 0...10) i], $(EntryPoint)")
                        .Returns(1)
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/764");
                    yield return new TestCaseData("foo(1, 1 > 2$(EntryPoint)")
                        .Returns(1)
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2324");
                    yield return new TestCaseData("foo(1, 1 < 2$(EntryPoint)")
                        .Returns(1)
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2324");
                    yield return new TestCaseData("foo(1, 1 << 2$(EntryPoint)")
                        .Returns(1)
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2324");
                    yield return new TestCaseData("foo(1, 1 >> 2$(EntryPoint)")
                        .Returns(1)
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2324");
                    yield return new TestCaseData("foo(1, 1 > 2, 1 < 2 ? 2 > 3 : $(EntryPoint)")
                        .Returns(2)
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2324");
                    yield return new TestCaseData("foo(1, 1 > 2, 1 < 2 ? 2 >>> 3 : $(EntryPoint)")
                        .Returns(2)
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2324");
                    yield return new TestCaseData("foo(1, bar(1, 2), 1 < 2$(EntryPoint)")
                        .Returns(2)
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2324");
                }
            }

            static IEnumerable<TestCaseData> FindParameterIndexIssue2468TestCases
            {
                get
                {
                    yield return new TestCaseData("new Vector<Foo>(1, bar(1, 2), 1 < 2$(EntryPoint)")
                        .Returns(2)
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2468");
                    yield return new TestCaseData("new Vector<Foo>(1, bar(1, 2), 1 > 2$(EntryPoint)")
                        .Returns(2)
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2468");
                }
            }

            [
                Test, 
                TestCaseSource(nameof(FindParameterIndexTestCases)),
                TestCaseSource(nameof(FindParameterIndexIssue2468TestCases)),
            ]
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
                        .Returns("test[1]".Length);
                    yield return new TestCaseData("test['1']; //")
                        .Returns("test['1']".Length);
                    yield return new TestCaseData("x:10, y:10}; //")
                        .Returns("x".Length);
                    yield return new TestCaseData("test()); //")
                        .Returns("test()".Length);
                    yield return new TestCaseData("test()]; //")
                        .Returns("test()".Length);
                    yield return new TestCaseData("test()}; //")
                        .Returns("test()".Length);
                    yield return new TestCaseData("test[1]); //")
                        .Returns("test[1]".Length);
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
                    yield return new TestCaseData("test(Math.random() > 0.5 ? 1 : 1); //")
                        .Returns("test(Math.random() > 0.5 ? 1 : 1)".Length);
                    yield return new TestCaseData("[1,2,3,4]; //")
                        .Returns("[1,2,3,4]".Length);
                    yield return new TestCaseData("{v:1}; //")
                        .Returns("{v:1}".Length);
                    yield return new TestCaseData("{v:[1]}; //")
                        .Returns("{v:[1]}".Length);
                    yield return new TestCaseData("[{v:[1]}]; //")
                        .Returns("[{v:[1]}]".Length);
                    yield return new TestCaseData("(v:{v:Int}); //")
                        .Returns("(v:{v:Int})".Length);
                    yield return new TestCaseData("[(v:{v:Int})]; //")
                        .Returns("[(v:{v:Int})]".Length);
                    yield return new TestCaseData("[function() {return 1;}]; //")
                        .Returns("[function() {return 1;}]".Length);
                    yield return new TestCaseData("'12345'; //")
                        .Returns("'12345'".Length);
                    yield return new TestCaseData("'${1$(EntryPoint)2345}'; //")
                        .Ignore("")
                        .Returns("'${12345".Length);
                    yield return new TestCaseData("-1; //")
                        .Returns("-1".Length);
                    yield return new TestCaseData("--1; //")
                        .Returns("--1".Length);
                    yield return new TestCaseData("+1; //")
                        .Returns("+1".Length);
                    yield return new TestCaseData("++1; //")
                        .Returns("++1".Length);
                    yield return new TestCaseData("5e-324; //")
                        .Returns("5e-324".Length);
                    yield return new TestCaseData("2.225e-308; //")
                        .Returns("2.225e-308".Length);
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
            public void AddClosingBracesSetUp() => ASContext.CommonSettings.AddClosingBraces = true;

            static IEnumerable<TestCaseData> OpenBraceTestCases
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

            static IEnumerable<TestCaseData> CloseBraceTestCases
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

            static IEnumerable<TestCaseData> DeleteBraceTestCases
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

            static IEnumerable<TestCaseData> AroundStringsTestCases
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

            static IEnumerable<TestCaseData> DeleteWhitespaceTestCases
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

            static IEnumerable<TestCaseData> InsideInterpolationTestCases
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
