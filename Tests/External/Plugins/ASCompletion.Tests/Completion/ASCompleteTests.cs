using System.Collections.Generic;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.TestUtils;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Constraints;
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
        protected static ASResult GetExpressionType(ScintillaControl sci, string sourceText)
        {
            SetSrc(sci, sourceText);
            //{ Update completion cache
            var ctx = ASContext.GetLanguageContext(sci.ConfigurationLanguage);
            ((ASContext) ctx).completionCache.IsDirty = true;
            var visibleExternalElements = ctx.GetVisibleExternalElements();
            ASContext.Context.GetVisibleExternalElements().Returns(visibleExternalElements);
            //}
            return ASComplete.GetExpressionType(sci, sci.WordEndPosition(sci.CurrentPos, true));
        }

        protected static string GetExpression(ScintillaControl sci, string sourceText)
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

        protected static ComaExpression DisambiguateComa(ScintillaControl sci, string sourceText)
        {
            sci.Text = sourceText;
            return ASComplete.DisambiguateComa(sci, sourceText.Length, 0);
        }

        protected static int FindParameterIndex(ScintillaControl sci, string sourceText)
        {
            sci.Text = sourceText;
            sci.Colourise(0, -1);
            SnippetHelper.PostProcessSnippets(sci, 0);
            var pos = sci.CurrentPos - 1;
            var result = ASComplete.FindParameterIndex(sci, ref pos);
            Assert.AreNotEqual(-1, pos);
            return result;
        }

        protected static int ExpressionEndPosition(ScintillaControl sci, string sourceText)
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

        protected static void OnChar(ScintillaControl sci, string sourceText, char addedChar, bool autoHide, EqualConstraint selectedLabelEqualConstraint)
        {
            SetSrc(sci, sourceText);
            ASContext.HasContext = true;
            ASComplete.OnChar(sci, addedChar, autoHide);
            Assert.That(CompletionList.SelectedLabel, selectedLabelEqualConstraint);
        }

        protected static string OnCharAndReplaceText(ScintillaControl sci, string sourceText, char addedChar, bool autoHide)
        {
            UITools.Init();
            CompletionList.CreateControl(PluginBase.MainForm);
            SetSrc(sci, sourceText);
            ASContext.Context.CurrentClass.InFile.Context = ASContext.Context;
            ASContext.HasContext = true;
            //{ Update completion cache
            var ctx = ASContext.GetLanguageContext(sci.ConfigurationLanguage);
            var visibleExternalElements = ctx.GetVisibleExternalElements();
            ASContext.Context.GetVisibleExternalElements().Returns(visibleExternalElements);
            //}
            ASComplete.OnChar(sci, addedChar, autoHide);
            Assert.That(CompletionList.SelectedLabel, Is.Not.Null.Or.Not.Empty);
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


            static IEnumerable<TestCaseData> GetExpressionTypeIssue2624TestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("GetExpressionType_issue2624_1"))
                        .Returns(null)
                        .SetName("this.<complete>. Issue 2624. Case 1");
                    yield return new TestCaseData(ReadAllText("GetExpressionType_issue2624_2"))
                        .Returns(null)
                        .SetName("super.<complete>. Issue 2624. Case 2");
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
                TestCaseSource(nameof(GetExpressionTypeIssue2624TestCases)),
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

            static IEnumerable<TestCaseData> OnCharIssue2955TestCases
            {
                get
                {
                    yield return new TestCaseData("OnCharIssue2955_1", '.', false, Is.Not.EqualTo("flash.display.IBitmapDrawable"))
                        .SetName("new IBitmapDrawable<complete>' Issue2105. Case 1.")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2955");
                }
            }

            [
                Test,
                TestCaseSource(nameof(OnCharIssue2955TestCases))
            ]
            public void OnChar(string fileName, char addedChar, bool autoHide, EqualConstraint selecvtedLabelEqualConstraint) => OnChar(sci, ReadAllText(fileName), addedChar, autoHide, selecvtedLabelEqualConstraint);

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

        [TestFixture]
        public class AddClosingBraces : ASCompleteTests
        {
            const string prefix = "AddClosingBraces: ";

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

            static string Common(string text, ScintillaControl sci)
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
