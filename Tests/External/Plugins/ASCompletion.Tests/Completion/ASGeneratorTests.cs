using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.Settings;
using ASCompletion.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using ScintillaNet;
using System.Text.RegularExpressions;
using PluginCore.Managers;

// TODO: Tests with different formatting options using parameterized tests

namespace ASCompletion.Completion
{
    [TestFixture]
    public class ASGeneratorTests : ASCompletionTests
    {
        public class GetBodyStart : ASGeneratorTests
        {
            static IEnumerable<TestCaseData> GetBodyStartTestCases
            {
                get
                {
                    yield return new TestCaseData("function test():void{\r\n\t\t\t\r\n}", 0, 1, "function test():void{\r\n\t\t\t\r\n}", 26).SetName("SimpleCase");
                    // Should we reindent the second line?
                    yield return new TestCaseData("function test():void{\r\n\t\t\t}", 0, 1, "function test():void{\r\n\t\t\t\r\n}", 26).SetName("EndOnSecondLine");
                    yield return new TestCaseData("function test():void{\r\n}", 0, 1, "function test():void{\r\n\t\r\n}", 24).SetName("EndOnSecondLineNoExtraIndent");
                    yield return new TestCaseData("function test():void{\r\n\t\t\t//comment}", 0, 1, "function test():void{\r\n\t\t\t//comment}", 26).SetName("CharOnSecondLine");
                    yield return new TestCaseData("function test():void{}", 0, 0, "function test():void{\r\n\t\r\n}", 24).SetName("EndOnSameDeclarationLine");
                    yield return new TestCaseData("function test():void\r\n\r\n{}\r\n", 0, 2, "function test():void\r\n\r\n{\r\n\t\r\n}\r\n", 28).SetName("EndOnSameLine");
                    yield return new TestCaseData("function test():void {trace(1);}", 0, 0, "function test():void {\r\n\ttrace(1);}", 25).SetName("TextOnStartLine");
                    yield return new TestCaseData("function test(arg:String='{', arg2:String=\"{\"):void/*{*/{\r\n}", 0, 1, "function test(arg:String='{', arg2:String=\"{\"):void/*{*/{\r\n\t\r\n}", 60)
                        .SetName("BracketInCommentsOrText");
                    yield return new TestCaseData("function test():void/*áéíóú*/\r\n{}", 0, 1, "function test():void/*áéíóú*/\r\n{\r\n\t\r\n}", 40).SetName("MultiByteCharacters");
                    yield return new TestCaseData("function tricky():void {} function test():void{\r\n\t\t\t}", 0, 1, "function tricky():void {} function test():void{\r\n\t\t\t}", 49)
                        .SetName("WithAnotherMemberInTheSameLine")
                        .Ignore("Having only LineFrom and LineTo for members is not enough to handle these cases. FlashDevelop in general is not too kind when it comes to several members in the same line, but we could change the method to use positions and try to get the proper position before.");
                    yield return new TestCaseData("function test<T:{}>(arg:T):void{\r\n\r\n}", 0, 1, "function test<T:{}>(arg:T):void{\r\n\r\n}", 34).SetName("BracketsInGenericConstraint");
                    yield return new TestCaseData("function test(arg:{x:Int}):void{\r\n\r\n}", 0, 1, "function test(arg:{x:Int}):void{\r\n\r\n}", 34).SetName("AnonymousStructures");
                }
            }

            [Test, TestCaseSource(nameof(GetBodyStartTestCases))]
            public void Common(string text, int lineStart, int lineEnd, string resultText, int bodyStart)
            {
                sci.Text = text;
                sci.ConfigurationLanguage = "haxe";
                sci.Colourise(0, -1);
                int funcBodyStart = ASGenerator.GetBodyStart(lineStart, lineEnd, sci);

                Assert.AreEqual(bodyStart, funcBodyStart);
                Assert.AreEqual(resultText, sci.Text);
            }
        }

        [TestFixture]
        public class ContextualActions : ASGeneratorTests
        {
            [TestFixture]
            class ShowEventsList : ContextualActions
            {
                ClassModel dataEventModel;
                FoundDeclaration found;

                [TestFixtureSetUp]
                public void ShowEventsListSetup()
                {
                    ASContext.Context.SetAs3Features();
                    dataEventModel = CreateDataEventModel();
                    found = new FoundDeclaration
                    {
                        InClass = new ClassModel(),
                        Member = new MemberModel()
                    };
                }

                [Test]
                public void ShowEventsList_EventWithDataEvent()
                {
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(dataEventModel);
                    ASGenerator.contextParam = "Event";
                    var options = new List<ICompletionListItem>();
                    ASGenerator.ShowEventList(found, options);
                    Assert.AreEqual(2, options.Count);
                    Assert.IsTrue(Regex.IsMatch(options[0].Label, "\\bEvent\\b"));
                    Assert.IsTrue(Regex.IsMatch(options[1].Label, "\\bDataEvent\\b"));
                }

                [Test]
                public void ShowEventsList_EventWithoutDataEvent()
                {
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(ClassModel.VoidClass);
                    var options = new List<ICompletionListItem>();
                    ASGenerator.contextParam = "Event";
                    ASGenerator.ShowEventList(found, options);
                    Assert.AreEqual(1, options.Count);
                    Assert.IsTrue(Regex.IsMatch(options[0].Label, "\\bEvent\\b"));
                }

                [Test]
                public void ShowEventsList_CustomEventWithDataEvent()
                {
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(dataEventModel);
                    var options = new List<ICompletionListItem>();
                    ASGenerator.contextParam = "CustomEvent";
                    ASGenerator.ShowEventList(found, options);
                    Assert.AreEqual(2, options.Count);
                    Assert.IsTrue(Regex.IsMatch(options[0].Label, "\\bCustomEvent\\b"));
                    Assert.IsTrue(Regex.IsMatch(options[1].Label, "\\bEvent\\b"));
                }

                private ClassModel CreateDataEventModel()
                {
                    var dataEventFile = new FileModel();
                    var dataEventModel = new ClassModel
                    {
                        Name = "DataEvent",
                        InFile = dataEventFile
                    };
                    dataEventFile.Classes.Add(dataEventModel);
                    return dataEventModel;
                }
            }
        }

        [TestFixture]
        public class GenerateJob : ASGeneratorTests
        {

            [TestFixture]
            public class FieldFromParameter : GenerateJob
            {
                static IEnumerable<TestCaseData> FieldFromParameterCommonTestCases
                {
                    get
                    {
                        yield return new TestCaseData(Visibility.Public,
                            "package generatortest {\r\n\tpublic class FieldFromParameterTest{\r\n\t\tpublic function FieldFromParameterTest(arg:String){}\r\n\t}\r\n}",
                            new ClassModel
                            {
                                LineFrom = 1,
                                LineTo = 3,
                                Members = new MemberList
                                {
                                    new MemberModel("FieldFromParameterTest", null, FlagType.Constructor,
                                        Visibility.Public)
                                    {
                                        LineFrom = 2,
                                        LineTo = 2,
                                        Parameters = new List<MemberModel>
                                        {
                                            new MemberModel {Name = "arg", LineFrom = 2, LineTo = 2}
                                        }
                                    }
                                },
                                InFile = FileModel.Ignore
                            }, 0, 0)
                            .Returns(ReadAllTextAS3("FieldFromParameterEmptyBody"))
                            .SetName("PublicScopeWithEmptyBody");

                        yield return new TestCaseData(Visibility.Public,
                            "package generatortest {\r\n\tpublic class FieldFromParameterTest{\r\n\t\tpublic function FieldFromParameterTest(arg:String){\r\n\t\t\tsuper(arg);}\r\n\t}\r\n}",
                            new ClassModel
                            {
                                LineFrom = 1,
                                LineTo = 4,
                                Members = new MemberList
                                {
                                    new MemberModel("FieldFromParameterTest", null, FlagType.Constructor,
                                        Visibility.Public)
                                    {
                                        LineFrom = 2,
                                        LineTo = 3,
                                        Parameters = new List<MemberModel>
                                        {
                                            new MemberModel {Name = "arg", LineFrom = 2, LineTo = 2}
                                        }
                                    }
                                },
                                InFile = FileModel.Ignore
                            }, 0, 0)
                            .Returns(ReadAllTextAS3("FieldFromParameterWithSuperConstructor"))
                            .SetName("PublicScopeWithSuperConstructor");

                        yield return new TestCaseData(Visibility.Public,
                            ReadAllTextAS3("BeforeFieldFromParameterWithSuperConstructorMultiLine"),
                            new ClassModel
                            {
                                LineFrom = 1,
                                LineTo = 6,
                                Members = new MemberList
                                {
                                    new MemberModel("FieldFromParameterTest", null, FlagType.Constructor,
                                        Visibility.Public)
                                    {
                                        LineFrom = 2,
                                        LineTo = 5,
                                        Parameters = new List<MemberModel>
                                        {
                                            new MemberModel {Name = "arg", LineFrom = 2, LineTo = 2}
                                        }
                                    }
                                },
                                InFile = FileModel.Ignore
                            }, 0, 0)
                            .Returns(ReadAllTextAS3("FieldFromParameterWithSuperConstructorMultiLine"))
                            .SetName("PublicScopeWithSuperConstructorMultiLine");

                        yield return new TestCaseData(Visibility.Public,
                            ReadAllTextAS3("BeforeFieldFromParameterWithWrongSuperConstructor"),
                            new ClassModel
                            {
                                LineFrom = 1,
                                LineTo = 6,
                                Members = new MemberList
                                {
                                    new MemberModel("FieldFromParameterTest", null, FlagType.Constructor,
                                        Visibility.Public)
                                    {
                                        LineFrom = 2,
                                        LineTo = 5,
                                        Parameters = new List<MemberModel>
                                        {
                                            new MemberModel {Name = "arg", LineFrom = 2, LineTo = 2}
                                        }
                                    }
                                },
                                InFile = FileModel.Ignore
                            }, 0, 0)
                            .Returns(ReadAllTextAS3("FieldFromParameterWithWrongSuperConstructor"))
                            .SetName("PublicScopeWithWrongSuperConstructor");
                    }
                }

                [Test, TestCaseSource(nameof(FieldFromParameterCommonTestCases))]
                public string Common(Visibility scope, string sourceText, ClassModel inClass, int memberPos, int parameterPos)
                {
                    SetAs3Features(sci);
                    var table = new Hashtable();
                    table["scope"] = scope;
                    sci.Text = sourceText;
                    var sourceMember = inClass.Members[memberPos];
                    ASGenerator.SetJobContext(null, null, sourceMember.Parameters[parameterPos], null);
                    ASGenerator.GenerateJob(GeneratorJobType.FieldFromParameter, sourceMember, inClass, null, table);
                    return sci.Text;
                }

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFieldFromParameter"), GeneratorJobType.FieldFromParameter, Visibility.Private)
                                .Returns(ReadAllTextHaxe("AfterGenerateFieldFromParameter"));
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFieldFromOptionalParameter"), GeneratorJobType.FieldFromParameter, Visibility.Private)
                                .Returns(ReadAllTextHaxe("AfterGenerateFieldFromOptionalParameter"));
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFieldFromOptionalUntypedParameter"), GeneratorJobType.FieldFromParameter, Visibility.Private)
                                .Returns(ReadAllTextHaxe("AfterGenerateFieldFromOptionalUntypedParameter"));
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFieldFromOptionalParameter2"), GeneratorJobType.FieldFromParameter, Visibility.Private)
                                .Returns(ReadAllTextHaxe("AfterGenerateFieldFromOptionalParameter2"));
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job, Visibility scope) => HaxeImpl(sourceText, job, scope, sci);

                internal static string HaxeImpl(string sourceText, GeneratorJobType job, Visibility scope, ScintillaControl sci)
                {
                    SetHaxeFeatures(sci);
                    return Common(sourceText, job, scope, sci);
                }

                internal static string Common(string sourceText, GeneratorJobType job, Visibility scope, ScintillaControl sci)
                {
                    SetSrc(sci, sourceText);
                    ASGenerator.SetJobContext(null, null, ASContext.Context.CurrentMember.Parameters.First(), null);
                    ASGenerator.GenerateJob(job, ASContext.Context.CurrentMember, ASContext.Context.CurrentClass, null, new Hashtable {["scope"] = scope});
                    return sci.Text;
                }
            }

            [TestFixture]
            public class ImplementInterface : GenerateJob
            {
                internal static string[] DeclarationModifierOrder = { "public", "protected", "internal", "private", "static", "override" };

                [TestFixtureSetUp]
                public void ImplementInterfaceSetup() => ASContext.CommonSettings.DeclarationModifierOrder = DeclarationModifierOrder;

                static ClassModel GetAs3ImplementInterfaceModel()
                {
                    var interfaceModel = new ClassModel { InFile = new FileModel(), Name = "ITest", Type = "ITest" };
                    interfaceModel.Members.Add(new MemberList
                    {
                        new MemberModel("getter", "String", FlagType.Getter, Visibility.Public),
                        new MemberModel("setter", "void", FlagType.Setter, Visibility.Public)
                        {
                            Parameters =
                                new List<MemberModel>
                                {
                                    new MemberModel("value", "String", FlagType.Variable, Visibility.Default)
                                }
                        },
                        new MemberModel("testMethod", "Number", FlagType.Function, Visibility.Public),
                        new MemberModel("testMethodArgs", "int", FlagType.Function, Visibility.Public)
                        {
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel("arg", "Number", FlagType.Variable, Visibility.Default),
                                new MemberModel("arg2", "Boolean", FlagType.Variable, Visibility.Default)
                            }
                        }
                    });

                    return interfaceModel;
                }

                static ClassModel GetHaxeImplementInterfaceModel()
                {
                    var interfaceModel = new ClassModel { InFile = new FileModel(), Name = "ITest", Type = "ITest" };
                    interfaceModel.Members.Add(new MemberList
                    {
                        new MemberModel("normalVariable", "Int", FlagType.Variable, Visibility.Public),
                        new MemberModel("ro", "Int", FlagType.Getter, Visibility.Public)
                        {
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel {Name = "default"},
                                new MemberModel {Name = "null"}
                            }
                        },
                        new MemberModel("wo", "Int", FlagType.Getter, Visibility.Public)
                        {
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel {Name = "null"},
                                new MemberModel {Name = "default"}
                            }
                        },
                        new MemberModel("x", "Int", FlagType.Getter, Visibility.Public)
                        {
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel {Name = "get"},
                                new MemberModel {Name = "set"}
                            }
                        },
                        new MemberModel("y", "Int", FlagType.Getter, Visibility.Public)
                        {
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel {Name = "get"},
                                new MemberModel {Name = "never"}
                            }
                        },
                        new MemberModel("testMethod", "Float", FlagType.Function, Visibility.Public),
                        new MemberModel("testMethodArgs", "Int", FlagType.Function, Visibility.Public)
                        {
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel("arg", "Float", FlagType.Variable, Visibility.Default),
                                new MemberModel("arg2", "Bool", FlagType.Variable, Visibility.Default)
                            }
                        },
                        new MemberModel("testPrivateMethod", "Float", FlagType.Function, Visibility.Private)
                        {
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel("?arg", "String", FlagType.Variable, Visibility.Default),
                                new MemberModel("?arg2", "Int", FlagType.Variable, Visibility.Default)
                                {
                                    Value = "1"
                                }
                            }
                        },
                        new MemberModel("testMethodWithTypeParams", "Float", FlagType.Function, Visibility.Public)
                        {
                            Template = "<K:IOtherInterface>",
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel("arg", "K", FlagType.Variable, Visibility.Default)
                            }
                        }
                    });

                    return interfaceModel;
                }

                static IEnumerable<TestCaseData> ImplementInterfaceAs3TestCases
                {
                    get
                    {
                        yield return new TestCaseData("package generatortest {\r\n\tpublic class ImplementTest{}\r\n}",
                            new ClassModel { InFile = new FileModel(), LineFrom = 1, LineTo = 1 }, GetAs3ImplementInterfaceModel())
                            .Returns(ReadAllTextAS3("ImplementInterfaceNoMembers"))
                            .SetName("Full");

                        yield return new TestCaseData(ReadAllTextAS3("BeforeImplementInterfacePublicMemberBehindPrivate"),
                            new ClassModel
                            {
                                InFile = new FileModel(),
                                LineFrom = 1,
                                LineTo = 10,
                                Members = new MemberList
                                {
                                    new MemberModel("publicMember", "void", FlagType.Function, Visibility.Public)
                                    {LineFrom = 3, LineTo = 5},
                                    new MemberModel("privateMember", "String", FlagType.Function, Visibility.Private)
                                    {LineFrom = 7, LineTo = 9}
                                }
                            },
                            GetAs3ImplementInterfaceModel())
                            .Returns(ReadAllTextAS3("ImplementInterfacePublicMemberBehindPrivate"))
                            .SetName("FullWithPublicMemberBehindPrivate");

                        yield return new TestCaseData(ReadAllTextAS3("BeforeImplementInterfaceNoPublicMember"),
                            new ClassModel
                            {
                                InFile = new FileModel(),
                                LineFrom = 1,
                                LineTo = 10,
                                Members = new MemberList
                                {
                                    new MemberModel("privateMember", "String", FlagType.Function, Visibility.Private)
                                    {
                                        LineFrom = 3,
                                        LineTo = 5
                                    }
                                }
                            },
                            GetAs3ImplementInterfaceModel())
                            .Returns(ReadAllTextAS3("ImplementInterfaceNoPublicMember"))
                            .SetName("FullWithoutPublicMember");
                    }
                }

                static IEnumerable<TestCaseData> ImplementInterfaceHaxeTestCases
                {
                    get
                    {
                        yield return new TestCaseData("package generatortest;\r\n\r\nclass ImplementTest{}",
                            new ClassModel { InFile = new FileModel(), LineFrom = 2, LineTo = 2 }, GetHaxeImplementInterfaceModel())
                            .Returns(ReadAllTextHaxe("ImplementInterfaceNoMembers"))
                            .SetName("Full");

                        yield return new TestCaseData("package generatortest;\r\n\r\nclass ImplementTest{}",
                            new ClassModel { InFile = new FileModel(), LineFrom = 2, LineTo = 2 },
                            new ClassModel
                            {
                                InFile = new FileModel(), Name = "ITest", Type = "ITest",
                                Members = new MemberList
                                {
                                    new MemberModel("x", "Int", FlagType.Getter, Visibility.Public)
                                    {
                                        Parameters = new List<MemberModel>
                                        {
                                            new MemberModel {Name = "get"},
                                            new MemberModel {Name = "set"}
                                        }
                                    }
                                }
                            })
                            .Returns(ReadAllTextHaxe("ImplementInterfaceNoMembersInsertSingleProperty"))
                            .SetName("SingleProperty");
                    }
                }

                [Test, TestCaseSource(nameof(ImplementInterfaceAs3TestCases))]
                public string AS3(string sourceText, ClassModel sourceModel, ClassModel interfaceToImplement)
                {
                    SetAs3Features(sci);
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(interfaceToImplement);
                    sci.Text = sourceText;
                    ASGenerator.GenerateJob(GeneratorJobType.ImplementInterface, null, sourceModel, null, null);
                    return sci.Text;
                }

                [Test, TestCaseSource(nameof(ImplementInterfaceHaxeTestCases))]
                public string Haxe(string sourceText, ClassModel sourceModel, ClassModel interfaceToImplement)
                {
                    SetHaxeFeatures(sci);
                    ASContext.Context.ResolveType(null, null).ReturnsForAnyArgs(interfaceToImplement);
                    sci.Text = sourceText;
                    ASGenerator.GenerateJob(GeneratorJobType.ImplementInterface, null, sourceModel, null, null);
                    return sci.Text;
                }
            }

            [TestFixture]
            public class PromoteLocal : GenerateJob
            {
                internal static string[] DeclarationModifierOrder = { "public", "protected", "internal", "private", "static", "override" };

                [TestFixtureSetUp]
                public void PromoteLocalWithSetup() => ASContext.CommonSettings.DeclarationModifierOrder = DeclarationModifierOrder;

                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforePromoteLocal"))
                                .Returns(ReadAllTextAS3("AfterPromoteLocal_generateExplicitScopeIsFalse"))
                                .SetName("Promote to class member");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText) => AS3Impl(sourceText, sci);

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforePromoteLocal"))
                                .Returns(ReadAllTextHaxe("AfterPromoteLocal_generateExplicitScopeIsFalse"))
                                .SetName("Promote to class member");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText) => HaxeImpl(sourceText, sci);

                internal static string AS3Impl(string sourceText, ScintillaControl sci)
                {
                    SetAs3Features(sci);
                    return Common(sourceText, sci);
                }

                internal static string HaxeImpl(string sourceText, ScintillaControl sci)
                {
                    SetHaxeFeatures(sci);
                    return Common(sourceText, sci);
                }

                static string Common(string sourceText, ScintillaControl sci)
                {
                    SetSrc(sci, sourceText);
                    var expr = ASComplete.GetExpressionType(sci, sci.CurrentPos);
                    ASGenerator.contextMember = expr.Context.LocalVars[0];
                    var options = new List<ICompletionListItem>();
                    ASGenerator.ContextualGenerator(sci, options);
                    var item = options.Find(it => ((GeneratorItem)it).job == GeneratorJobType.PromoteLocal);
                    var value = item.Value;
                    return sci.Text;
                }
            }

            [TestFixture]
            public class PromoteLocalWithExplicitScope : GenerateJob
            {
                [TestFixtureSetUp]
                public void PromoteLocalWithSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = PromoteLocal.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateScope = true;
                }

                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforePromoteLocal"))
                                .Returns(ReadAllTextAS3("AfterPromoteLocal_generateExplicitScopeIsTrue"))
                                .SetName("Promote to class member");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText) => PromoteLocal.AS3Impl(sourceText, sci);

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforePromoteLocal"))
                                .Returns(ReadAllTextHaxe("AfterPromoteLocal_generateExplicitScopeIsTrue"))
                                .SetName("Promote to class member");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText) => PromoteLocal.HaxeImpl(sourceText, sci);
            }

            [TestFixture]
            public class PromoteLocalWithDefaultModifierDeclaration : GenerateJob
            {
                [TestFixtureSetUp]
                public void PromoteLocalWithSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = PromoteLocal.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
                }

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforePromoteLocal"))
                                .Returns(ReadAllTextHaxe("AfterPromoteLocalWithDefaultModifier"))
                                .SetName("Promote to private class member with default modifier declaration");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText) => PromoteLocal.HaxeImpl(sourceText, sci);
            }

            [TestFixture]
            public class GenerateFunction : GenerateJob
            {
                internal static string[] DeclarationModifierOrder = { "public", "protected", "internal", "private", "static", "override" };

                [TestFixtureSetUp]
                public void GenerateFunctionSetup() => ASContext.CommonSettings.DeclarationModifierOrder = DeclarationModifierOrder;

                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateFunction_generateExplicitScopeIsFalse"))
                                .SetName("Generate private function");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicFunction_generateExplicitScopeIsFalse"))
                                .SetName("Generate public function");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_forSomeObj"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_forSomeObj"))
                                .SetName("From some.foo|();");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_forSomeObj2"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_forSomeObj2"))
                                .SetName("From new Some().foo|();");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_forSomeObj3"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_forSomeObj3"))
                                .SetName("From new Some()\n.foo|();");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateStaticFunction"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicStaticFunction_generateExplicitScopeIsFalse"))
                                .SetName("Generate public static function");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateStaticFunction_forCurrentType"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicStaticFunction_generateExplicitScopeIsTrue"))
                                .SetName("From CurrentType.foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateStaticFunction_forSomeType"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicStaticFunction_forSomeType"))
                                .SetName("From SomeType.foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue1436"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateFunction_issue1436"))
                                .SetName("From foo(vector[0])")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1436");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_TwoDimensionalVector_issue1436"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateFunction_TwoDimensionalVector_issue1436"))
                                .SetName("From foo(vector[0][0])")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1436");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_MultidimensionalVector_issue1436"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateFunction_MultidimensionalVector_issue1436"))
                                .SetName("From foo(vector[0][0][0][0])")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1436");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue103"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103"))
                                .SetName("Issue 103. Case 1")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue103_2"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_2"))
                                .SetName("Issue 103. Case 2")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue103_3"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_3"))
                                .SetName("Issue 103. Case 3")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue103_4"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_4"))
                                .SetName("Issue 103. Case 4")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue103_5"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_5"))
                                .SetName("Issue 103. Case 5")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue103_6"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_6"))
                                .SetName("Issue 103. Case 6")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue1645"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_issue1645"))
                                .SetName("Issue 1645. Case 1")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1645");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue1645_2"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue1645_2"))
                                .SetName("Issue 1645. Case 2")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1645");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue1780_1"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_issue1780_1"))
                                .SetName("foo(Math.round(1.5))")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1780");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue1780_2"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_issue1780_2"))
                                .SetName("foo(round(1.5))")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1780");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, GeneratorJobType job) => AS3Impl(sourceText, job, sci);

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateFunction_generateExplicitScopeIsFalse"))
                                .SetName("Generate private function");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicFunction_generateExplicitScopeIsFalse"))
                                .SetName("Generate public function");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103"))
                                .SetName("Issue103. Case 1")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_2"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_2"))
                                .SetName("Issue103. Case 2")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_3"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_3"))
                                .SetName("Issue103. Case 3")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_4"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_4"))
                                .SetName("Issue103. Case 4")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_5"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_5"))
                                .SetName("Issue103. Case 5")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_6"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_6"))
                                .SetName("Issue103. Case 6")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_7"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_7"))
                                .SetName("Issue103. Case 7")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_8"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_8"))
                                .SetName("Issue103. Case 8")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_9"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_9"))
                                .SetName("Issue103. Case 9")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_10"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_10"))
                                .SetName("Issue103. Case 10")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_11"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_11"))
                                .SetName("Issue103. Case 11")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_12"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_12"))
                                .SetName("Issue103. Case 12")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_13"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_13"))
                                .SetName("Issue103. Case 13")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_14"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_14"))
                                .SetName("Issue103. Case 14")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_15"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_15"))
                                .SetName("Issue103. Case 15")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_16"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_16"))
                                .SetName("Issue103. Case 16")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_17"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_17"))
                                .SetName("Issue103. Case 17")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_18"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_18"))
                                .SetName("Issue103. Case 18")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_19"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_19"))
                                .SetName("Issue103. Case 19")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_20"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_20"))
                                .SetName("Issue103. Case 20")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_21"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_21"))
                                .SetName("Issue103. Case 21")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_22"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_22"))
                                .SetName("Issue103. Case 22")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue1645"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue1645"))
                                .SetName("Issue1645. Case 1")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1645");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue1645_2"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue1645_2"))
                                .SetName("Issue1645. Case 2")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1645");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue1780_1"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue1780_1"))
                                .SetName("foo(Math.round(1.5))")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1780");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue1780_2"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue1780_2"))
                                .SetName("foo(round(1.5))")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1780");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue1836"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue1836"))
                                .SetName("Issue 1836. Case 1")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1836");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job) => HaxeImpl(sourceText, job, sci);

                internal static string AS3Impl(string sourceText, GeneratorJobType job, ScintillaControl sci)
                {
                    SetAs3Features(sci);
                    return Common(sourceText, job, sci);
                }

                internal static string HaxeImpl(string sourceText, GeneratorJobType job, ScintillaControl sci)
                {
                    SetHaxeFeatures(sci);
                    return Common(sourceText, job, sci);
                }

                internal static string Common(string sourceText, GeneratorJobType job, ScintillaControl sci)
                {
                    SetSrc(sci, sourceText);
                    var options = new List<ICompletionListItem>();
                    ASGenerator.ContextualGenerator(sci, options);
                    var item = options.Find(it => ((GeneratorItem) it).job == job);
                    var value = item.Value;
                    return sci.Text;
                }
            }

            [TestFixture]
            public class GenerateFunctionWithReturnDefaultValue : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateFunctionSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateFunction.DeclarationModifierOrder;
                    ASContext.CommonSettings.GeneratedMemberDefaultBodyStyle = GeneratedMemberBodyStyle.ReturnDefaultValue;
                }

                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue103"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_issue103"))
                                .SetName("Issue 103. Case 1")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue103_2"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_issue103_2"))
                                .SetName("Issue 103. Case 2")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue103_3"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_issue103_3"))
                                .SetName("Issue 103. Case 3")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue103_4"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_issue103_4"))
                                .SetName("Issue 103. Case 4")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue103_5"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_issue103_5"))
                                .SetName("Issue 103. Case 5")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue103_6"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_issue103_6"))
                                .SetName("Issue 103. Case 6")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_issue1645_2"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_issue1645_2"))
                                .SetName("Issue 1645. Case 2")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1645");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, GeneratorJobType job) => GenerateFunction.AS3Impl(sourceText, job, sci);

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_13"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_13"))
                                .SetName("Issue103. Case 13")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_14"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_14"))
                                .SetName("Issue103. Case 14")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_18"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_18"))
                                .SetName("Issue103. Case 18")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_19"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_19"))
                                .SetName("Issue103. Case 19")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_20"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_20"))
                                .SetName("Issue103. Case 20")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue103_21"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue103_21"))
                                .SetName("Issue103. Case 21")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction_issue1645_2"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGenerateFunction_issue1645_2"))
                                .SetName("Issue1645. Case 2")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1645");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job) => GenerateFunction.HaxeImpl(sourceText, job, sci);
            }

            [TestFixture]
            public class GenerateFunctionWithExplicitScope : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateFunctionSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateFunction.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateScope = true;
                }

                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateFunction_generateExplicitScopeIsTrue"))
                                .SetName("Generate private function from member scope");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicFunction_generateExplicitScopeIsTrue"))
                                .SetName("Generate public function from member scope");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_forSomeObj"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_forSomeObj"))
                                .SetName("From some.foo|();");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_forSomeObj2"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_forSomeObj2"))
                                .SetName("From new Some().foo|();");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction_forSomeObj3"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGenerateFunction_forSomeObj3"))
                                .SetName("From new Some()\n.foo|();");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateStaticFunction"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicStaticFunction_generateExplicitScopeIsTrue"))
                                .SetName("Generate public static function");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateStaticFunction_forCurrentType"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicStaticFunction_generateExplicitScopeIsTrue"))
                                .SetName("From CurrentType.foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateStaticFunction_forSomeType"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicStaticFunction_forSomeType"))
                                .SetName("From SomeType.foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction2"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateFunction2_generateExplicitScopeIsTrue"))
                                .SetName("Generate private function from class scope");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, GeneratorJobType job) => GenerateFunction.AS3Impl(sourceText, job, sci);

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateFunction_generateExplicitScopeIsTrue"))
                                .SetName("Generate private function");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction"), GeneratorJobType.FunctionPublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicFunction_generateExplicitScopeIsTrue"))
                                .SetName("Generate public function");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job) => GenerateFunction.HaxeImpl(sourceText, job, sci);
            }

            [TestFixture]
            public class GenerateFunctionWithDefaultModifierDeclaration : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateFunctionSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateFunction.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
                }

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateFunction"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateFunctionWithDefaultModifier"))
                                .SetName("Generate private function with default modifier declaration");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateStaticFunction"), GeneratorJobType.Function)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateStaticFunctionWithDefaultModifier"))
                                .SetName("Generate private static function with default modifier declaration");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job) => GenerateFunction.HaxeImpl(sourceText, job, sci);
            }

            [TestFixture]
            public class GenerateFunctionWithProtectedDeclaration : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateFunctionSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateFunction.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateProtectedDeclarations = true;
                }

                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateFunction"), GeneratorJobType.Function)
                                .Returns(ReadAllTextAS3("AfterGenerateProtectedFunction"))
                                .SetName("Generate private function with protected modifier declaration");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, GeneratorJobType job) => GenerateFunction.AS3Impl(sourceText, job, sci);
            }

            [TestFixture]
            public class AssignStatementToVar : GenerateJob
            {
                [TestFixtureSetUp]
                public void AssignStatementToVarSetUp() => ASContext.Context.Settings.GenerateImports = true;

                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromVectorInitializer"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromVectorInitializer"))
                            .SetName("new <int>[]|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromTwoDimensionalVectorInitializer"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromTwoDimensionalVectorInitializer"))
                            .SetName("new <Vector.<int>>[new <int>[]]|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromNewVector"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromNewVector"))
                            .SetName("new Vector.<Vector.<int>>()|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromFIeldOfItemOfVector"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromFIeldOfItemOfVector"))
                            .SetName("v[0][0].length|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromMultilineArrayInitializer_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromMultilineArrayInitializer_useSpaces"))
                            .SetName("From multiline array initializer");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromMultilineObjectInitializer_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromMultilineObjectInitializer_useSpaces"))
                            .SetName("From multiline object initializer");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromMultilineObjectInitializer2_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromMultilineObjectInitializer2_useSpaces"))
                            .SetName("From multiline object initializer 2");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromMethodChaining_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromMethodChaining_useSpaces"))
                            .SetName("From method chaining");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromMethodChaining2_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromMethodChaining2_useSpaces"))
                            .SetName("From method chaining 2");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromNewString_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromNewString_useSpaces"))
                            .SetName("new String(\"\")|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromStringInitializer_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromStringInitializer_useSpaces"))
                            .SetName("\"\"|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromStringInitializer2_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromStringInitializer2_useSpaces"))
                            .SetName("''|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromNewString2_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromNewString2_useSpaces"))
                            .SetName("new String(\"\".charAt(0))|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromNewBitmapDataWithParams_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromNewBitmapDataWithParams_useSpaces"))
                            .SetName("new BitmapData(rect.width, rect.height)| . Case 1");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromNewBitmapDataWithParams_multiline_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromNewBitmapDataWithParams_multiline_useSpaces"))
                            .SetName("new BitmapData(rect.width,\n rect.height)| . Case 2");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromArrayInitializer_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromArrayInitializer_useSpaces"))
                            .SetName("[]|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromArrayInitializer2_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromArrayInitializer2_useSpaces"))
                            .SetName("[rect.width, rect.height]|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromFunctionResult_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromFunctionResult_useSpaces"))
                            .SetName("foo()|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromCallback_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromCallback_useSpaces"))
                            .SetName("from callback");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromClass_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromClass_useSpaces"))
                            .SetName("Class|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromPrivateField"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromPrivateField"))
                            .SetName("from private field");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromNewVar"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromNewVar"))
                            .SetName("new Var()|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromUnsafeCastExpr"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromUnsafeCastExpr"))
                            .SetName("(new type() as String)|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromTrue"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromTrue"))
                            .SetName("true|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromXML"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromXML"))
                            .SetName("<xml/>|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromArrayAccess"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromArrayAccess"))
                            .SetName("array[0]|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVarFromArrayAccess2"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVarFromArrayAccess2"))
                            .SetName("vector[0]|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1704_1"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1704_1"))
                            .SetName("from function():Vector.<Sprite>")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1704");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1704_2"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1704_2"))
                            .SetName("from function():Vector.<flash.display.Sprite>")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1704");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1704_3"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1704_3"))
                            .SetName("from function():Array/*flash.display.Sprite*/")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1704");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1704_4"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1704_4"))
                            .SetName("from function():flash.display.Sprite")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1704");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1749_1"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1749_1"))
                            .SetName("1 + 1|")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1749");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1749_2"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1749_2"))
                            .SetName("1 +\n1|")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1749");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1749_3"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1749_3"))
                            .SetName("Issue 1749. Case 3")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1749");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1749_4"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1749_4"))
                            .SetName("Issue 1749. Case 4")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1749");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1749_5"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1749_5"))
                            .SetName("Issue 1749. Case 5")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1749");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1749_6"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1749_6"))
                            .SetName("Issue 1749. Modulo")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1749");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_increment"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_increment"))
                            .SetName("++1|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_increment2"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_increment2"))
                            .SetName("1++|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_increment3"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_increment3"))
                            .SetName("++1 * 1++|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_increment4"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_increment4"))
                            .SetName("++1 * ++1|");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_typeof1"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_typeof1"))
                            .SetName("typeof value. Issue 1908.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1908");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_delete"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_delete"))
                            .SetName("delete o[key]. Issue 1908.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1908");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1383_1"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1383_1"))
                            .SetName("new <*>[]|")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1383");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1383_2"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1383_2"))
                            .SetName("new <Vector.<*>>[new <*>[]]|")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1383");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1383_3"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1383_3"))
                            .SetName("new Vector.<Vector.<*>>()|")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1383");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1880_1"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1880_1"))
                            .SetName("/regex/|")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1880");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1880_2"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1880_2"))
                            .SetName("[/regex/]|")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1880");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1880_3"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1880_3"))
                            .SetName("{v:/regex/}|")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1880");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1880_4"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1880_4"))
                            .SetName("(/regex/ as String)|")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1880");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1880_5"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1880_5"))
                            .SetName("/regex/gm|")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1880");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1765_1"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1765_1"))
                            .SetName("1 << 1|")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1765");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1765_2"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1765_2"))
                            .SetName("1 >> 1|")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1765");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1765_3"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1765_3"))
                            .SetName("1 ^ 1|")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1765");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1765_4"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1765_4"))
                            .SetName("1 >>> 1|")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1765");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1765_5"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1765_5"))
                            .SetName("1 | 1|")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1765");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1765_6"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1765_6"))
                            .SetName("1 & 1|")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1765");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1765_7"), GeneratorJobType.AssignStatementToVar, false)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1765_7"))
                            .SetName("~1|")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1765");
                    }
                }

                static IEnumerable<TestCaseData> AS3Issue1764TestCases
                {
                    get
                    {
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1764_1"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1764_1"))
                            .SetName("1 < 2|. Assign statement to local variable")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1764_2"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1764_2"))
                            .SetName("1 > 2|. Assign statement to local variable")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1764_3"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1764_3"))
                            .SetName("1 && 2|. Assign statement to local variable")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1764_4"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1764_4"))
                            .SetName("1 || 2|. Assign statement to local variable")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1764_5"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1764_5"))
                            .SetName("1 != 2|. Assign statement to local variable")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1764_6"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1764_6"))
                            .SetName("1 == 2|. Assign statement to local variable")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1764_5"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1764_5"))
                            .SetName("1 !== 2|. Assign statement to local variable")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue1764_6"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue1764_6"))
                            .SetName("1 === 2|. Assign statement to local variable")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                    }
                }

                static IEnumerable<TestCaseData> AS3Issue2151TestCases
                {
                    get
                    {
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue2151_1"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue2151_1"))
                            .SetName("_;| Assign statement to var. Issue 2151.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/2151");
                    }
                }

                static IEnumerable<TestCaseData> AS3Issue2153TestCases
                {
                    get
                    {
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue2153_1"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue2153_1"))
                            .SetName("get3D;| Assign statement to var. Issue 2153.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/2153");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue2153_2"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue2153_2"))
                            .SetName("getThis;| Assign statement to var. Issue 2153.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/2153");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeAssignStatementToVar_issue2153_2"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextAS3("AfterAssignStatementToVar_issue2153_2"))
                            .SetName("getSuper;| Assign statement to var. Issue 2153.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/2153");
                    }
                }

                [
                    Test,
                    TestCaseSource(nameof(AS3TestCases)),
                    TestCaseSource(nameof(AS3Issue1764TestCases)),
                    TestCaseSource(nameof(AS3Issue2151TestCases)),
                    TestCaseSource(nameof(AS3Issue2153TestCases)),
                ]
                public string AS3(string sourceText, GeneratorJobType job, bool isUseTabs) => AS3Impl(sourceText, job, isUseTabs, sci);

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVar_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVar_useSpaces"))
                                .SetName("Assign statement to var. Use spaces instead of tabs.");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVar_useTabs"), GeneratorJobType.AssignStatementToVar, true)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVar_useTabs"))
                                .SetName("Assign statement to var. Use tabs instead of spaces.");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVarFromFieldOfItemOfArray"), GeneratorJobType.AssignStatementToVar, true)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVarFromFieldOfItemOfArray"))
                                .SetName("from a[0][0].length");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVarFromNewMap"), GeneratorJobType.AssignStatementToVar, true)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVarFromNewMap"))
                                .SetName("from new Map<String, Int>()");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVarFromNewMap2"), GeneratorJobType.AssignStatementToVar, true)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVarFromNewMap2"))
                                .SetName("from new Map<Map<String, Int>, Int>()");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVarFromNewMap3"), GeneratorJobType.AssignStatementToVar, true)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVarFromNewMap3"))
                                .SetName("from new Map<String, Array<Map<String, Int>>>()");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVarFromNewMap4"), GeneratorJobType.AssignStatementToVar, true)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVarFromNewMap4"))
                                .SetName("from new Map<String, Array<Map<String, Int->Int->Int>>>()");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVarFromCallback_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVarFromCallback_useSpaces"))
                                .SetName("from callback");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVarFromCallback2_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVarFromCallback2_useSpaces"))
                                .SetName("from callback 2");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVarFromCallback3_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVarFromCallback3_useSpaces"))
                                .SetName("from callback 3");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVarFromClass_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVarFromClass_useSpaces"))
                                .SetName("from Class");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVarFromArray_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVarFromArray_useSpaces"))
                                .SetName("from new Array<Int>()");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVarFromArray2_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVarFromArray2_useSpaces"))
                                .SetName("from new Array<Int->Int>()");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVarFromArray3_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVarFromArray3_useSpaces"))
                                .SetName("from new Array<{name:String, factory:String->{x:Int, y:Int}}>()");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVarFromDynamic_useSpaces"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVarFromDynamic_useSpaces"))
                                .SetName("from {}");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVarFromCastExp"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVarFromCastExp"))
                                .SetName("cast(d, String)");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVarFromCastExp2"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVarFromCastExp2"))
                                .SetName("cast (d, String)");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVarFromCastExp3"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVarFromCastExp3"))
                                .SetName("cast ( d, String )");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVar_issue1696_1"), GeneratorJobType.AssignStatementToVar, true)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVar_issue1696_1"))
                                .SetName("issue 1696")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1696");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVar_issue_1704_1"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVar_issue_1704_1"))
                                .SetName("from (function foo():haxe.ds.Vector<haxe.Timer->Type.ValueType> ...)()")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1704");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVar_issue_1704_2"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVar_issue_1704_2"))
                                .SetName("from (function foo():haxe.ds.Vector<haxe.Timer> ...)()")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1704");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVar_issue_1704_3"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVar_issue_1704_3"))
                                .SetName("from (function foo():haxe.Timer ...)()")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1704");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVar_issue_1704_4"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVar_issue_1704_4"))
                                .SetName("from (function foo():haxe.Timer->{v:haxe.ds.Vector<Int>->Type.ValueType} ...)()")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1704");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVar_issue1749_6"), GeneratorJobType.AssignStatementToVar, true)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVar_issue1749_6"))
                                .SetName("Issue 1749. Modulo")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1749");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVar_issue_1766"), GeneratorJobType.AssignStatementToVar, false)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVar_issue_1766"))
                                .SetName("from [1 => '1', 2 = '2']")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1704");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVar_operator_is"), GeneratorJobType.AssignStatementToVar, true)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVar_operator_is"))
                                .SetName("Issue 1918. (v is String)")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1918");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVar_issue1908_unsafecast"), GeneratorJobType.AssignStatementToVar, true)
                                .Returns(ReadAllTextHaxe("AfterAssignStatementToVar_issue1908_unsafecast"))
                                .SetName("Issue 1908. Unsafe cast")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1908");
                        yield return new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVar_issue1908_untyped"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextHaxe("AfterAssignStatementToVar_issue1908_untyped"))
                            .SetName("Issue 1908. untyped")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1908");
                        yield return new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVar_issue1880_1"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextHaxe("AfterAssignStatementToVar_issue1880_1"))
                            .SetName("~/regex/|")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1908");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job, bool isUseTabs)
                {
                    SetHaxeFeatures(sci);
                    return HaxeImpl(sourceText, job, isUseTabs, sci);
                }

                static IEnumerable<TestCaseData> HaxeTestCasesSdk330
                {
                    get
                    {
                        yield return new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVar_issue1992_typecheck"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextHaxe("AfterAssignStatementToVar_issue1992_typecheck"))
                            .SetName("Issue 1992. from (v:Iterable<Dynamic>)")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1908");
                        yield return new TestCaseData(ReadAllTextHaxe("BeforeAssignStatementToVar_issue1992_typecheck_2"), GeneratorJobType.AssignStatementToVar, true)
                            .Returns(ReadAllTextHaxe("AfterAssignStatementToVar_issue1992_typecheck_2"))
                            .SetName("Issue 1992. from (v:Iterable<Iterable<Dynamic>>)")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1908");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCasesSdk330))]
                public string HaxeSdk330(string sourceText, GeneratorJobType job, bool isUseTabs)
                {
                    SetHaxeFeatures(sci);
                    ASContext.Context.Settings.InstalledSDKs = new[] {new InstalledSDK {Path = PluginBase.CurrentProject.CurrentSDK, Version = "3.3.0"}};
                    return HaxeImpl(sourceText, job, isUseTabs, sci);
                }

                internal static string AS3Impl(string sourceText, GeneratorJobType job, bool isUseTabs, ScintillaControl sci)
                {
                    sci.IsUseTabs = isUseTabs;
                    SetAs3Features(sci);
                    return Common(sourceText, job, sci);
                }

                internal static string HaxeImpl(string sourceText, GeneratorJobType job, bool isUseTabs, ScintillaControl sci)
                {
                    sci.IsUseTabs = isUseTabs;
                    SetSrc(sci, sourceText);
                    return Common(sourceText, job, sci);
                }

                internal static string Common(string sourceText, GeneratorJobType job, ScintillaControl sci)
                {
                    SetSrc(sci, sourceText);
                    var list = new MemberList();
                    list.Merge(ASContext.GetLanguageContext(sci.ConfigurationLanguage).GetVisibleExternalElements());
                    list.Merge(ASContext.Context.CurrentModel.Imports);
                    ASContext.Context.GetVisibleExternalElements().Returns(list);
                    ASGenerator.GenerateJob(job, ASContext.Context.CurrentMember, ASContext.Context.CurrentClass, null, null);
                    return sci.Text;
                }
            }

            [TestFixture]
            public class GenerateVariable : GenerateJob
            {
                internal static string[] DeclarationModifierOrder = { "public", "protected", "internal", "private", "static", "inline", "override" };

                [TestFixtureSetUp]
                public void GenerateVariableSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = DeclarationModifierOrder;
                }

                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateVariable_generateExplicitScopeIsFalse"))
                                .SetName("Generate private variable");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicVariable_generateExplicitScopeIsFalse"))
                                .SetName("Generate public variable");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable_forSomeObj"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGenerateVariable_forSomeObj"))
                                .SetName("From some.foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable_forSomeObj2"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGenerateVariable_forSomeObj2"))
                                .SetName("From new Some().foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable_forSomeObj3"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGenerateVariable_forSomeObj3"))
                                .SetName("From new Some()\n.foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateStaticVariable_forCurrentType"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicStaticVariable_forCurrentType"))
                                .SetName("From CurrentType.foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateStaticVariable_forSomeType"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicStaticVariable_forSomeType"))
                                .SetName("From SomeType.foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable_issue1460_1"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateVariable_issue1460_1"))
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1460");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable_issue1460_2"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateVariable_issue1460_2"))
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1460");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateConstant"), GeneratorJobType.Constant)
                                .Returns(ReadAllTextAS3("AfterGenerateConstant"))
                                .SetName("Generate constant");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateConstant_issue1460"), GeneratorJobType.Constant)
                                .Returns(ReadAllTextAS3("AfterGenerateConstant_issue1460"))
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1460");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, GeneratorJobType job) => AS3Impl(sourceText, job, sci);

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateVariable"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateVariable_generateExplicitScopeIsFalse"))
                                .SetName("Generate private variable");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateStaticVariable"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateStaticVariable"))
                                .SetName("Generate private static variable");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateVariable"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicVariable_generateExplicitScopeIsFalse"))
                                .SetName("Generate public variable");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateStaticVariable"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicStaticVariable_generateExplicitScopeIsFalse"))
                                .SetName("Generate public static variable");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGeneratePublicStaticVariable_forSomeType"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicStaticVariable_forSomeType"))
                                .SetName("From SomeType.foo|");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGeneratePublicStaticVariable_forCurrentType"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicStaticVariable_forCurrentType"))
                                .SetName("From CurrentType.foo|");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateVariable_issue1460_1"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextHaxe("AfterGenerateVariable_issue1460_1"))
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1460");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateVariable_issue1460_2"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextHaxe("AfterGenerateVariable_issue1460_2"))
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1460");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateVariable_issue1460_3"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextHaxe("AfterGenerateVariable_issue1460_3"))
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1460");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateConstant"), GeneratorJobType.Constant)
                                .Returns(ReadAllTextHaxe("AfterGenerateConstant"))
                                .SetName("Generate constant");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateConstant_issue1460"), GeneratorJobType.Constant)
                                .Returns(ReadAllTextHaxe("AfterGenerateConstant_issue1460"))
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1460");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job) => HaxeImpl(sourceText, job, sci);

                internal static string AS3Impl(string sourceText, GeneratorJobType job, ScintillaControl sci)
                {
                    SetAs3Features(sci);
                    return Common(sourceText, job, sci);
                }
                
                internal static string HaxeImpl(string sourceText, GeneratorJobType job, ScintillaControl sci)
                {
                    SetHaxeFeatures(sci);
                    return Common(sourceText, job, sci);
                }

                internal static string Common(string sourceText, GeneratorJobType job, ScintillaControl sci)
                {
                    SetSrc(sci, sourceText);
                    ASGenerator.GenerateJob(job, ASContext.Context.CurrentMember, ASContext.Context.CurrentClass, null, null);
                    return sci.Text;
                }
            }

            [TestFixture]
            public class GenerateVariableWithExplicitScope : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateVariableSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateVariable.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateScope = true;
                }

                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateVariable_generateExplicitScopeIsTrue"))
                                .SetName("Generate private variable from member scope");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicVariable_generateExplicitScopeIsTrue"))
                                .SetName("Generate public variable from member scope");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable_forSomeObj"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGenerateVariable_forSomeObj"))
                                .SetName("From some.foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable_forSomeObj2"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGenerateVariable_forSomeObj2"))
                                .SetName("From new Some().foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable_forSomeObj3"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGenerateVariable_forSomeObj3"))
                                .SetName("From new Some()\n.foo|");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable2"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateVariable2_generateExplicitScopeIsTrue"))
                                .SetName("Generate private variable from class scope");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, GeneratorJobType job) => GenerateVariable.AS3Impl(sourceText, job, sci);

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateVariable"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateVariable_generateExplicitScopeIsTrue"))
                                .SetName("Generate private variable");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateVariable"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicVariable_generateExplicitScopeIsTrue"))
                                .SetName("Generate public variable");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateStaticVariable"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicStaticVariable_generateExplicitScopeIsTrue"))
                                .SetName("Generate public static variable");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGeneratePublicStaticVariable_forSomeType"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicStaticVariable_forSomeType"))
                                .SetName("From SomeType.foo| variable");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGeneratePublicStaticVariable_forCurrentType"), GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicStaticVariable_forCurrentType"))
                                .SetName("From CurrentType.foo| variable");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job) => GenerateVariable.HaxeImpl(sourceText, job, sci);
            }

            [TestFixture]
            public class GenerateVariableWithDefaultModifierDeclaration : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateVariableSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateVariable.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
                }

                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateVariable_generateExplicitScopeIsFalse"))
                                .SetName("Generate private variable with default modifier declration");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateStaticVariable_forCurrentType"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextAS3("AfterGeneratePrivateStaticVariabeWithDefaultModifier"))
                                .SetName("Generate private static variable with default modifier declration");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, GeneratorJobType job) => GenerateVariable.AS3Impl(sourceText, job, sci);

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateVariable"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateVariableWithDefaultModifier"))
                                .SetName("Generate private variable with default modifier declration");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateStaticVariable"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateStaticVariableWithDefaultModifier"))
                                .SetName("Generate private static variable with default modifier declration");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, GeneratorJobType job) => GenerateVariable.HaxeImpl(sourceText, job, sci);
            }

            [TestFixture]
            public class GenerateVariableWithProtectedDeclaration : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateVariableSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateVariable.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateProtectedDeclarations = true;
                }

                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateVariable"), GeneratorJobType.Variable)
                                .Returns(ReadAllTextAS3("AfterGenerateProtectedVariable"))
                                .SetName("Generate private variable with protected modifier declration");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, GeneratorJobType job) => GenerateVariable.AS3Impl(sourceText, job, sci);
            }

            [TestFixture]
            public class GenerateVariableWithGenerateImports : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateVariableSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateVariable.DeclarationModifierOrder;
                    ASContext.Context.Settings.GenerateImports.Returns(true);
                }

                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData("BeforeGeneratePublicVariable_issue1734_1", GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicVariable_issue1734_1"))
                                .SetName("Issue1734. Case 1")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1734");
                        yield return
                            new TestCaseData("BeforeGeneratePublicVariable_issue1734_2", GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextAS3("AfterGeneratePublicVariable_issue1734_2"))
                                .SetName("Issue1734. Case 2")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1734");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string fileName, GeneratorJobType job)
                {
                    SetAs3Features(sci);
                    SetCurrentFileName(GetFullPathAS3(fileName));
                    return GenerateVariable.Common(ReadAllTextAS3(fileName), job, sci);
                }
                
                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData("BeforeGeneratePublicVariable_issue1734_1", GeneratorJobType.VariablePublic)
                                .Returns(ReadAllTextHaxe("AfterGeneratePublicVariable_issue1734_1"))
                                .SetName("Issue1734. Case 1")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1734");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string fileName, GeneratorJobType job)
                {
                    SetHaxeFeatures(sci);
                    SetCurrentFileName(GetFullPathHaxe(fileName));
                    return GenerateVariable.Common(ReadAllTextHaxe(fileName), job, sci);
                }
            }

            [TestFixture]
            public class GenerateEventHandler : GenerateJob
            {
                internal static readonly string[] DeclarationModifierOrder = { "public", "protected", "internal", "private", "static", "override" };

                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return new TestCaseData(ReadAllTextAS3("BeforeGenerateEventHandler"), new string[0])
                            .Returns(ReadAllTextAS3("AfterGenerateEventHandler_withoutAutoRemove"))
                            .SetName("Generate event handler without auto remove");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeGenerateEventHandler"), new[] {"Event.ADDED", "Event.REMOVED"})
                            .Returns(ReadAllTextAS3("AfterGenerateEventHandler_withAutoRemove"))
                            .SetName("Generate event handler with auto remove");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeGenerateEventHandler_issue164_1"), new[] {"Event.ADDED", "Event.REMOVED"})
                            .Returns(ReadAllTextAS3("AfterGenerateEventHandler_withAutoRemove_issue164_1"))
                            .SetName("Generate event handler with auto remove. Issue 164. Case 1")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/164");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeGenerateEventHandler_issue164_2"), new[] {"Event.ADDED", "Event.REMOVED"})
                            .Returns(ReadAllTextAS3("AfterGenerateEventHandler_withAutoRemove_issue164_2"))
                            .SetName("Generate event handler with auto remove. Issue 164. Case 2")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/164");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, string[] autoRemove) => AS3Impl(sourceText, autoRemove, sci);

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return new TestCaseData(ReadAllTextHaxe("BeforeGenerateEventHandler"), new string[0])
                            .Returns(ReadAllTextHaxe("AfterGenerateEventHandler_withoutAutoRemove"))
                            .SetName("Generate event handler without auto remove");
                        yield return new TestCaseData(ReadAllTextHaxe("BeforeGenerateEventHandler"), new[] {"Event.ADDED", "Event.REMOVED"})
                            .Returns(ReadAllTextHaxe("AfterGenerateEventHandler_withAutoRemove"))
                            .SetName("Generate event handler with auto remove");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, string[] autoRemove) => HaxeImpl(sourceText, autoRemove, sci);

                internal static string AS3Impl(string sourceText, string[] autoRemove, ScintillaControl sci)
                {
                    SetAs3Features(sci);
                    return Common(sourceText, autoRemove, sci);
                }

                internal static string HaxeImpl(string sourceText, string[] autoRemove, ScintillaControl sci)
                {
                    SetHaxeFeatures(sci);
                    return Common(sourceText, autoRemove, sci);
                }

                static string Common(string sourceText, string[] autoRemove, ScintillaControl sci)
                {
                    ASContext.CommonSettings.EventListenersAutoRemove = autoRemove;
                    SetSrc(sci, sourceText);
                    var re = string.Format(ASGenerator.patternEvent, ASGenerator.contextToken);
                    var m = Regex.Match(sci.GetLine(sci.CurrentLine), re, RegexOptions.IgnoreCase);
                    ASGenerator.contextMatch = m;
                    ASGenerator.contextParam = ASGenerator.CheckEventType(m.Groups["event"].Value);
                    ASGenerator.GenerateJob(GeneratorJobType.ComplexEvent, ASContext.Context.CurrentMember, ASContext.Context.CurrentClass, null, null);
                    return sci.Text;
                }
            }

            [TestFixture]
            public class GenerateEventHandlerWithExplicitScope : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateEventHandlerSetup() => ASContext.CommonSettings.GenerateScope = true;

                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeGenerateEventHandler"), new[] {"Event.ADDED", "Event.REMOVED"})
                                .Returns(ReadAllTextAS3("AfterGenerateEventHandler_withAutoRemove_generateExplicitScopeIsTrue"))
                                .SetName("Generate event handler with auto remove");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, string[] autoRemove) => GenerateEventHandler.AS3Impl(sourceText, autoRemove, sci);

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateEventHandler"), new[] {"Event.ADDED", "Event.REMOVED"})
                                .Returns(ReadAllTextHaxe("AfterGenerateEventHandler_withAutoRemove_generateExplicitScopeIsTrue"))
                                .SetName("Generate event handler with auto remove");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, string[] autoRemove) => GenerateEventHandler.HaxeImpl(sourceText, autoRemove, sci);
            }

            [TestFixture]
            public class GenerateEventHandlerWithDefaultModifierDeclaration : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateEventHandlerSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateEventHandler.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
                }

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGenerateEventHandler"), new[] {"Event.ADDED", "Event.REMOVED"})
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateEventHandlerWithDefaultModifier"))
                                .SetName("Generate private event handler with default modifier declaration");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeGeneratePrivateStaticEventHandler"), new string[0])
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateStaticEventHandlerWithDefaultModifier"))
                                .SetName("Generate private static event handler with default modifier declaration");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, string[] autoRemove) => GenerateEventHandler.HaxeImpl(sourceText, autoRemove, sci);
            }

            [TestFixture]
            public class GenerateGetterSetter : GenerateJob
            {
                internal static string[] DeclarationModifierOrder = { "public", "protected", "internal", "private", "static", "override" };

                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData("BeforeGenerateGetterSetter_fromPublicField", GeneratorJobType.GetterSetter)
                                .Returns(ReadAllTextAS3("AfterGenerateGetterSetter_fromPublicField"))
                                .SetName("Generate getter and setter from public field");
                        yield return
                            new TestCaseData("BeforeGenerateGetterSetter_fromPublicFieldIfNameStartWith_", GeneratorJobType.GetterSetter)
                                .Returns(ReadAllTextAS3("AfterGenerateGetterSetter_fromPublicFieldIfNameStartWith_"))
                                .SetName("Generate getter and setter from public field if name start with \"_\"");
                        yield return
                            new TestCaseData("BeforeGenerateGetterSetter_fromPrivateField", GeneratorJobType.GetterSetter)
                                .Returns(ReadAllTextAS3("AfterGenerateGetterSetter_fromPrivateField"))
                                .SetName("Generate getter and setter from private field");
                        yield return
                            new TestCaseData("BeforeGenerateGetterSetter_issue_1", GeneratorJobType.Setter)
                                .Returns(ReadAllTextAS3("AfterGenerateGetterSetter_issue_1"))
                                .SetName("issue set");
                        yield return
                            new TestCaseData("BeforeGenerateGetterSetter_issue_2", GeneratorJobType.Getter)
                                .Returns(ReadAllTextAS3("AfterGenerateGetterSetter_issue_2"))
                                .SetName("issue get 1");
                        yield return
                            new TestCaseData("BeforeGenerateGetterSetter_issue_3", GeneratorJobType.Getter)
                                .Returns(ReadAllTextAS3("AfterGenerateGetterSetter_issue_3"))
                                .SetName("issue get 2");
                        yield return
                            new TestCaseData("BeforeGenerateGetterSetter_issue_4", GeneratorJobType.Getter)
                                .Returns(ReadAllTextAS3("AfterGenerateGetterSetter_issue_4"))
                                .SetName("issue get 3");
                        yield return
                            new TestCaseData("BeforeGenerateGetterSetter_issue_5", GeneratorJobType.Getter)
                                .Returns(ReadAllTextAS3("AfterGenerateGetterSetter_issue_5"))
                                .SetName("issue get 4");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string fileName, GeneratorJobType job) => AS3Impl(fileName, sci, job);

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData("BeforeGenerateGetterSetter", GeneratorJobType.GetterSetter)
                                .Returns(ReadAllTextHaxe("AfterGenerateGetterSetter"))
                                .SetName("Generate getter and setter");
                        yield return
                            new TestCaseData("BeforeGenerateGetterSetter_issue221", GeneratorJobType.GetterSetter)
                                .Returns(ReadAllTextHaxe("AfterGenerateGetterSetter_issue221"))
                                .SetName("issue 221");
                        yield return
                            new TestCaseData("BeforeGenerateGetterSetter_issue_1", GeneratorJobType.Setter)
                                .Returns(ReadAllTextHaxe("AfterGenerateGetterSetter_issue_1"))
                                .SetName("issue set");
                        yield return
                            new TestCaseData("BeforeGenerateGetterSetter_issue_2", GeneratorJobType.Getter)
                                .Returns(ReadAllTextHaxe("AfterGenerateGetterSetter_issue_2"))
                                .SetName("issue get 1");
                        yield return
                            new TestCaseData("BeforeGenerateGetterSetter_issue_3", GeneratorJobType.Getter)
                                .Returns(ReadAllTextHaxe("AfterGenerateGetterSetter_issue_3"))
                                .SetName("issue get 2");
                        yield return
                            new TestCaseData("BeforeGenerateGetterSetter_issue_4", GeneratorJobType.Getter)
                                .Returns(ReadAllTextHaxe("AfterGenerateGetterSetter_issue_4"))
                                .SetName("issue get 3");
                        yield return
                            new TestCaseData("BeforeGenerateGetterSetter_issue_5", GeneratorJobType.Getter)
                                .Returns(ReadAllTextHaxe("AfterGenerateGetterSetter_issue_5"))
                                .SetName("issue get 4");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string fileName, GeneratorJobType job) => HaxeImpl(fileName, sci, job);

                internal static string AS3Impl(string fileName, ScintillaControl sci, GeneratorJobType job)
                {
                    SetAs3Features(sci);
                    SetCurrentFileName(GetFullPathAS3(fileName));
                    return Common(ReadAllTextAS3(fileName), sci, job);
                }

                internal static string HaxeImpl(string fileName, ScintillaControl sci, GeneratorJobType job)
                {
                    SetHaxeFeatures(sci);
                    SetCurrentFileName(GetFullPathHaxe(fileName));
                    return Common(ReadAllTextHaxe(fileName), sci, job);
                }

                static string Common(string sourceText, ScintillaControl sci, GeneratorJobType job)
                {
                    SetSrc(sci, sourceText);
                    var options = new List<ICompletionListItem>();
                    ASGenerator.ContextualGenerator(sci, options);
                    var item = options.Find(it => it is GeneratorItem && ((GeneratorItem)it).job == job);
                    Assert.NotNull(item);
                    var value = item.Value;
                    return sci.Text;
                }
            }

            [TestFixture]
            public class GenerateGetterSetterWithDefaultModifierDeclaration : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateEventHandlerSetup()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateGetterSetter.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
                }

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData("BeforeGenerateGetterSetter", GeneratorJobType.GetterSetter)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateGetterSetterWithDefaultModifier"))
                                .SetName("Generate private getter and setter with default modifier declaration");
                        yield return
                            new TestCaseData("BeforeGeneratePrivateStaticGetterSetter", GeneratorJobType.GetterSetter)
                                .Returns(ReadAllTextHaxe("AfterGeneratePrivateStaticGetterSetterWithDefaultModifier"))
                                .SetName("Generate private static getter and setter with default modifier declaration");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string fileName, GeneratorJobType job) => GenerateGetterSetter.HaxeImpl(fileName, sci, job);
            }

            [TestFixture]
            public class GenerateOverride : GenerateJob
            {
                internal static string[] DeclarationModifierOrder = { "public", "protected", "internal", "private", "static", "override" };

                [TestFixtureSetUp]
                public void GenerateOverrideSetUp()
                {
                    ASContext.CommonSettings.DeclarationModifierOrder = DeclarationModifierOrder;
                    ASContext.Context.Settings.GenerateImports = true;
                }

                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeOverridePublicFunction"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextAS3("AfterOverridePublicFunction"))
                                .SetName("override public function");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeOverrideProtectedFunction"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextAS3("AfterOverrideProtectedFunction"))
                                .SetName("override protected function");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeOverrideInternalFunction"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextAS3("AfterOverrideInternalFunction"))
                                .SetName("override internal function");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeOverrideHasOwnProperty"), "Object", "hasOwnProperty", FlagType.Function)
                                .Returns(ReadAllTextAS3("AfterOverrideHasOwnProperty"))
                                .SetName("override hasOwnProperty");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeOverridePublicGetSet"), "Foo", "foo", FlagType.Getter)
                                .Returns(ReadAllTextAS3("AfterOverridePublicGetSet"))
                                .SetName("override public getter and setter");
                        yield return
                            new TestCaseData(ReadAllTextAS3("BeforeOverrideInternalGetSet"), "Foo", "foo", FlagType.Getter)
                                .Returns(ReadAllTextAS3("AfterOverrideInternalGetSet"))
                                .SetName("override internal getter and setter");
                        yield return new TestCaseData(ReadAllTextAS3("BeforeOverrideProtectedFunction_issue1383_1"), "Foo", "foo", FlagType.Function)
                            .Returns(ReadAllTextAS3("AfterOverrideProtectedFunction_issue1383_1"))
                            .SetName("override protected function foo(v:Vector.<*>):Vector.<*>");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText, string ofClassName, string memberName, FlagType memberFlags)
                {
                    return AS3Impl(sourceText, ofClassName, memberName, memberFlags, sci);
                }

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideGetNull"), "Foo", "foo", FlagType.Getter | FlagType.Setter)
                                .Returns(ReadAllTextHaxe("AfterOverrideGetNull"))
                                .SetName("Override var foo(get, null)");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideNullSet"), "Foo", "foo", FlagType.Getter | FlagType.Setter)
                                .Returns(ReadAllTextHaxe("AfterOverrideNullSet"))
                                .SetName("Override var foo(null, set)");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideGetSet"), "Foo", "foo", FlagType.Getter | FlagType.Setter)
                                .Returns(ReadAllTextHaxe("AfterOverrideGetSet"))
                                .SetName("Override var foo(get, set)");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideGetSet_2"), "Foo", "foo", FlagType.Getter | FlagType.Setter)
                                .Returns(ReadAllTextHaxe("AfterOverrideGetSet_2"))
                                .SetName("Override var foo(get, set). If the getter is already overridden.");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideGetSet_3"), "Foo", "foo", FlagType.Getter | FlagType.Setter)
                                .Returns(ReadAllTextHaxe("AfterOverrideGetSet_3"))
                                .SetName("Override var foo(get, set). If the setter is already overridden.");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideIssue793"), "Foo", "foo", FlagType.Getter | FlagType.Setter)
                                .Returns(ReadAllTextHaxe("AfterOverrideIssue793"))
                                .SetName("issue #793")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/793");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverridePublicFunction"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextHaxe("AfterOverridePublicFunction"))
                                .SetName("Override public function");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverridePrivateFunction"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextHaxe("AfterOverridePrivateFunction"))
                                .SetName("Override private function");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideFunctionWithTypeParams"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextHaxe("AfterOverrideFunctionWithTypeParams"))
                                .SetName("override function with type parameters");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideFunction_issue_1553_1"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextHaxe("AfterOverrideFunction_issue_1553_1"))
                                .SetName("override function foo(c:haxe.Timer->Void)")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1553");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideFunction_issue_1553_2"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextHaxe("AfterOverrideFunction_issue_1553_2"))
                                .SetName("override function foo(c:haxe.Timer->(Type.ValueType->Void))")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1553");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideFunction_issue_1553_3"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextHaxe("AfterOverrideFunction_issue_1553_3"))
                                .SetName("override function foo(c:haxe.Timer->{v:Type.ValueType, s:String}->Void)")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1553");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideFunction_issue_1553_4"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextHaxe("AfterOverrideFunction_issue_1553_4"))
                                .SetName("override function foo(c:haxe.Timer->({v:Type.ValueType, s:String}->Void))")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1553");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideFunction_issue_1553_5"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextHaxe("AfterOverrideFunction_issue_1553_5"))
                                .SetName("override function foo(c:{v:Type.ValueType, t:haxe.Timer})")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1553");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideFunction_issue_1553_6"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextHaxe("AfterOverrideFunction_issue_1553_6"))
                                .SetName("override function foo(c:{v:Type.ValueType, t:{t:haxe.Timer}}})")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1553");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideFunction_issue_1696_1"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextHaxe("AfterOverrideFunction_issue_1696_1"))
                                .SetName("override function foo(v:Array<haxe.Timer->String>)")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1696");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideFunction_issue_1696_2"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextHaxe("AfterOverrideFunction_issue_1696_2"))
                                .SetName("override function foo(v:Array<haxe.Timer->Type.ValueType->String>)")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1696");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideFunction_issue_1696_3"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextHaxe("AfterOverrideFunction_issue_1696_3"))
                                .SetName("override function foo(v:{a:Array<haxe.Timer>}->{a:haxe.ds.Vector<Type.ValueType>}->String)")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1696");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideFunction_issue_1696_4"), "flash.display.DisplayObjectContainer", "addChild", FlagType.Function)
                                .Returns(ReadAllTextHaxe("AfterOverrideFunction_issue_1696_4"))
                                .SetName("override function addChild(child:DisplayObject)")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1696");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideFunction_issue_1696_5"), "Foo", "foo", FlagType.Getter | FlagType.Setter)
                                .Returns(ReadAllTextHaxe("AfterOverrideFunction_issue_1696_5"))
                                .SetName("override function foo(get, set):haxe.ds.Vector<haxe.Timer->Type.ValueType>")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1696");
                        yield return new TestCaseData(ReadAllTextHaxe("BeforeOverrideFunction_issue_2134_1"), "flash.utils.Proxy", "callProperty", FlagType.Function)
                            .Returns(ReadAllTextHaxe("AfterOverrideFunction_issue_2134_1"))
                            .SetName("override function callProperty()")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/2134");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, string ofClassName, string memberName, FlagType memberFlags)
                {
                    return HaxeImpl(sourceText, ofClassName, memberName, memberFlags, sci);
                }

                internal static string AS3Impl(string sourceText, string ofClassName, string memberName, FlagType memberFlags, ScintillaControl sci)
                {
                    SetAs3Features(sci);
                    return Common(sourceText, ofClassName, memberName, memberFlags, sci);
                }

                internal static string HaxeImpl(string sourceText, string ofClassName, string memberName, FlagType memberFlags, ScintillaControl sci)
                {
                    SetHaxeFeatures(sci);
                    return Common(sourceText, ofClassName, memberName, memberFlags, sci);
                }

                static string Common(string sourceText, string ofClassName, string memberName, FlagType memberFlags, ScintillaControl sci)
                {
                    SetSrc(sci, sourceText);
                    var ofClass = ASContext.Context.CurrentModel.Classes.Find(model => model.Name == ofClassName);
                    if (ofClass == null)
                    {
                        foreach (var classpath in ASContext.Context.Classpath)
                        {
                            classpath.ForeachFile(model =>
                            {
                                foreach (var it in model.Classes)
                                {
                                    if (it.QualifiedName != ofClassName) continue;
                                    ofClass = it;
                                    return false;
                                }
                                return true;
                            });
                            if (ofClass != null) break;
                        }
                    }
                    var member = ofClass.Members.Search(memberName, memberFlags, 0);
                    ASGenerator.GenerateOverride(sci, ofClass, member, sci.CurrentPos);
                    return sci.Text;
                }
            }

            [TestFixture]
            public class GenerateOverrideWithDefaultModifierDeclaration : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateOverrideSetUp()
                {
                    ASContext.Context.Settings.GenerateImports = true;
                    ASContext.CommonSettings.DeclarationModifierOrder = GenerateOverride.DeclarationModifierOrder;
                    ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
                }

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideGetNull"), "Foo", "foo", FlagType.Getter | FlagType.Setter)
                                .Returns(ReadAllTextHaxe("AfterOverrideGetNullWithDefaultModifier"))
                                .SetName("Override var foo(get, null) with default modifier declaration");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideNullSet"), "Foo", "foo", FlagType.Getter | FlagType.Setter)
                                .Returns(ReadAllTextHaxe("AfterOverrideNullSetWithDefaultModifier"))
                                .SetName("Override var foo(null, set) with default modifier declaration");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverrideGetSet"), "Foo", "foo", FlagType.Getter | FlagType.Setter)
                                .Returns(ReadAllTextHaxe("AfterOverrideGetSetWithDefaultModifier"))
                                .SetName("Override var foo(get, set) with default modifier declaration");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("BeforeOverridePrivateFunction"), "Foo", "foo", FlagType.Function)
                                .Returns(ReadAllTextHaxe("AfterOverridePrivateFunctionWithDefaultModifier"))
                                .SetName("Override private function with default modifier");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText, string ofClassName, string memberName, FlagType memberFlags)
                {
                    return GenerateOverride.HaxeImpl(sourceText, ofClassName, memberName, memberFlags, sci);
                }
            }

            [TestFixture]
            public class GetStatementReturnType : GenerateJob
            {
                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfStringInitializer"))
                                .Returns(new ClassModel {Name = "String", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of \"\"");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfDigit"))
                                .Returns(new ClassModel {Name = "Number", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of 1");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfObjectInitializer"))
                                .Returns(new ClassModel {Name = "Object", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of {}");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfNewArray"))
                                .Returns(new ClassModel {Name = "Array", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of new Array()");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfArrayInitializer"))
                                .Returns(new ClassModel {Name = "Array", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of []");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfNewVector"))
                                .Returns(new ClassModel {Name = "Vector.<int>", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of new Vector.<int>()");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfVectorInitializer"))
                                .Returns(new ClassModel {Name = "Vector.<int>", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of new <int>[]");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfTwoDimensionalVectorInitializer"))
                                .Returns(new ClassModel {Name = "Vector.<Vector.<int>>", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of new new <Vector.<int>>[new <int>[0]]");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfItemOfVector"))
                                .Returns(new ClassModel {Name = "int", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of vector[0]");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfItemOfTwoDimensionalVector"))
                                .Returns(new ClassModel {Name = "int", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of vector[0][0]");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfItemOfMultidimensionalVector"))
                                .Returns(new ClassModel {Name = "int", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of vector[0][0][0][0]");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfArrayAccess"))
                                .Returns(new ClassModel {Name = "int", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of v[0][0].length");
                        yield return
                            new TestCaseData(ReadAllTextAS3("GetStatementReturnTypeOfNewObject"))
                                .Returns(new ClassModel {Name = "Object", InFile = FileModel.Ignore})
                                .SetName("Get statement return type of new Object");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public ClassModel AS3(string sourceText) => AS3Impl(sourceText, sci);

                public static ClassModel AS3Impl(string sourceText, ScintillaControl sci)
                {
                    SetAs3Features(sci);
                    return Common(sourceText, sci);
                }

                public static ClassModel Common(string sourceText, ScintillaControl sci)
                {
                    SetSrc(sci, sourceText);
                    var currentLine = sci.CurrentLine;
                    var returnType = ASGenerator.GetStatementReturnType(sci, ASContext.Context.CurrentClass, sci.GetLine(currentLine), sci.PositionFromLine(currentLine));
                    var result = returnType.resolve.Type;
                    return result;
                }
            }

            [TestFixture]
            public class ParseFunctionParameters : GenerateJob
            {
                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_String"))
                            .Returns(new List<MemberModel> {new ClassModel {Name = "String", InFile = FileModel.Ignore}})
                            .SetName("Parse function parameters of foo(\"string\")");
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_Boolean"))
                            .Returns(new List<MemberModel> {new ClassModel {Name = "Boolean", InFile = FileModel.Ignore}})
                            .SetName("Parse function parameters of foo(true)");
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_Boolean_false"))
                            .Returns(new List<MemberModel> {new ClassModel {Name = "Boolean", InFile = FileModel.Ignore}})
                            .SetName("Parse function parameters of foo(false)");
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_Digit"))
                            .Returns(new List<MemberModel> {new ClassModel {Name = "Number", InFile = FileModel.Ignore}})
                            .SetName("Parse function parameters of foo(1)");
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_Array"))
                            .Returns(new List<MemberModel> {new ClassModel {Name = "Array", InFile = FileModel.Ignore}})
                            .SetName("Parse function parameters of foo(new Array())");
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_ArrayInitializer"))
                            .Returns(new List<MemberModel> {new ClassModel {Name = "Array", InFile = FileModel.Ignore}})
                            .SetName("Parse function parameters of foo([])");
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_Object"))
                            .Returns(new List<MemberModel> {new ClassModel {Name = "Object", InFile = FileModel.Ignore}})
                            .SetName("Parse function parameters of foo(new Object())");
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_ObjectInitializer"))
                            .Returns(new List<MemberModel> {new ClassModel {Name = "Object", InFile = FileModel.Ignore}})
                            .SetName("Parse function parameters of foo({})");
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_Vector"))
                            .Returns(new List<MemberModel> {new ClassModel {Name = "Vector.<int>", InFile = FileModel.Ignore}})
                            .SetName("Parse function parameters of foo(new Vector.<int>())");
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_VectorInitializer"))
                            .Returns(new List<MemberModel> {new ClassModel {Name = "Vector.<int>", InFile = FileModel.Ignore}})
                            .SetName("Parse function parameters of foo(new <int>[])");
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_TwoDimensionalVectorInitializer"))
                            .Returns(new List<MemberModel> {new ClassModel {Name = "Vector.<Vector.<int>>", InFile = FileModel.Ignore}})
                            .SetName("Parse function parameters of foo(new <Vector.<int>>[new <int>[]])");
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_MultidimensionalVectorInitializer"))
                            .Returns(new List<MemberModel> {new ClassModel {Name = "Vector.<Vector.<Vector.<int>>>", InFile = FileModel.Ignore}})
                            .SetName("Parse function parameters of foo(new <Vector.<Vector.<int>>>[new <Vector.<int>>[new <int>[]]])");
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_ArrayAccess"))
                            .Returns(new List<MemberModel> {new ClassModel {Name = "int", InFile = FileModel.Ignore}})
                            .SetName("Parse function parameters of foo(v[0][0].length)");
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_uint"))
                            .Returns(new List<MemberModel> {new ClassModel {Name = "uint", InFile = FileModel.Ignore}})
                            .SetName("Parse function parameters of foo(0xFF0000)");
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_Sprite"))
                            .Returns(new List<MemberModel> {new ClassModel {Name = "Sprite", InFile = FileModel.Ignore}})
                            .SetName("Parse function parameters of foo(new Sprite())");
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_Sprite2"))
                            .Returns(new List<MemberModel>
                            {
                                new ClassModel {Name = "Sprite", InFile = FileModel.Ignore},
                                new ClassModel {Name = "Sprite", InFile = FileModel.Ignore}
                            })
                            .SetName("Parse function parameters of foo(new Sprite(), new Sprite())");
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_Sprite2_withComments"))
                            .Returns(new List<MemberModel>
                            {
                                new ClassModel {Name = "Sprite", InFile = FileModel.Ignore},
                                new ClassModel {Name = "Sprite", InFile = FileModel.Ignore}
                            })
                            .SetName("Parse function parameters of foo(/*new MovieClip()*/new Sprite(), /*new MovieClip()*/new Sprite())");
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_Sprite2_withComments2"))
                            .Returns(new List<MemberModel>
                            {
                                new ClassModel {Name = "Sprite", InFile = FileModel.Ignore},
                                new ClassModel {Name = "Sprite", InFile = FileModel.Ignore}
                            })
                            .SetName("Parse function parameters of foo(/*)*/new Sprite(), /*(((((*/new Sprite())");
                        yield return new TestCaseData(ReadAllTextAS3("ParseFunctionParameters_XML"))
                            .Returns(new List<MemberModel> {new ClassModel {Name = "XML", InFile = FileModel.Ignore}})
                            .SetName("Parse function parameters of foo(<xml param = '10' />)");
                    }
                }

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_String"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "String", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(\"string\")");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_String_2"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "String", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo('string')");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_Boolean"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Bool", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(true)");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_Boolean_false"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Bool", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(false)");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_Digit"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Float", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(1)");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_Array"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Array", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(new Array())");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_TypedArray"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Array<Int>", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(new Array<Int>())");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_TypedArray_2"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Array<Int->{x:Int, y:Int}>", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(new Array<Int->{x:Int, y:Int}>())");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_ArrayInitializer"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Array<T>", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo([])");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_ArrayInitializer_2"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Array<T>", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo([{v:[1,2,3,4]}])");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_ObjectInitializer"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Dynamic", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo({})");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_ObjectInitializer_2"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Dynamic", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo({key:'value'})");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_ObjectInitializer_3"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Dynamic", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo({v:[{key:'value'}]})");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_ArrayAccess"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Int", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(a[0][0].length)");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_Function"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Function", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(function() {})");
                        yield return
                            new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_Math.random.1.5"))
                                .Returns(new List<MemberModel> {new ClassModel {Name = "Float", InFile = FileModel.Ignore}})
                                .SetName("Parse function parameters of foo(Math.random(1.5))");
                        yield return new TestCaseData(ReadAllTextHaxe("ParseFunctionParameters_complexExpr"))
                            .Returns(new List<MemberModel> {new ClassModel {Name = "DisplayObject", InFile = FileModel.Ignore}})
                            .SetName("Parse function parameters of foo(new Sprite().addChild(new Sprite()))");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public List<MemberModel> AS3(string sourceText) => AS3Impl(sourceText, sci);

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public List<MemberModel> Haxe(string sourceText) => HaxeImpl(sourceText, sci);

                internal static List<MemberModel> AS3Impl(string sourceText, ScintillaControl sci)
                {
                    SetAs3Features(sci);
                    return Common(sourceText, sci);
                }

                internal static List<MemberModel> HaxeImpl(string sourceText, ScintillaControl sci)
                {
                    SetHaxeFeatures(sci);
                    return Common(sourceText, sci);
                }

                internal static List<MemberModel> Common(string sourceText, ScintillaControl sci)
                {
                    SetSrc(sci, sourceText);
                    var list = new MemberList();
                    list.Merge(ASContext.GetLanguageContext(sci.ConfigurationLanguage).GetVisibleExternalElements());
                    list.Merge(ASContext.Context.CurrentModel.Imports);
                    ASContext.Context.GetVisibleExternalElements().Returns(list);
                    var result = ASGenerator.ParseFunctionParameters(sci, sci.CurrentPos).Select(it => it.result.Type ?? it.result.Member).ToList();
                    return result;
                }
            }

            [TestFixture]
            public class ChangeConstructorDeclaration : GenerateJob
            {
                [TestFixtureSetUp]
                public void ChangeConstructorDeclarationSetup() => ASContext.Context.Settings.GenerateImports.Returns(true);

                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_String")
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_String"))
                                .SetName("new Foo(\"\") -> function Foo(string:String)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_String2")
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_String2"))
                                .SetName("new Foo(\"\", \"\") -> function Foo(string:String, string1:String)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_Digit")
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_Digit"))
                                .SetName("new Foo(1) -> function Foo(number:Number)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_Boolean")
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_Boolean"))
                                .SetName("new Foo(true) -> function Foo(boolean:Boolean)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_ObjectInitializer")
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_ObjectInitializer"))
                                .SetName("new Foo({}) -> function Foo(object:Object)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_ArrayInitializer")
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_ArrayInitializer"))
                                .SetName("new Foo([]) -> function Foo(array:Array)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_VectorInitializer")
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_VectorInitializer"))
                                .SetName("new Foo(new <int>[]) -> function Foo(vector:Vector.<int>)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_TwoDimensionalVectorInitializer")
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_TwoDimensionalVectorInitializer"))
                                .SetName("new Foo(new <Vector.<Vector.<int>>[new <int>[]]) -> function Foo(vector:Vector.<Vector.<int>>)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_ItemOfTwoDimensionalVectorInitializer")
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_ItemOfTwoDimensionalVectorInitializer"))
                                .SetName("new Foo(strings[0][0]) -> function Foo(string:String)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_Function")
                                .Returns(ReadAllTextAS3("AfterChangeConstructorDeclaration_Function"))
                                .SetName("new Foo(function():void {}) -> function Foo(functionValue:Function)");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string fileName) => AS3Impl(fileName, sci);

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_String")
                                .Returns(ReadAllTextHaxe("AfterChangeConstructorDeclaration_String"))
                                .SetName("new Foo(\"\") -> function new(string:String)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_String2")
                                .Returns(ReadAllTextHaxe("AfterChangeConstructorDeclaration_String2"))
                                .SetName("new Foo(\"\", \"\") -> function new(string:String, string1:String)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_Digit")
                                .Returns(ReadAllTextHaxe("AfterChangeConstructorDeclaration_Digit"))
                                .SetName("new Foo(1) -> function new(float:Float)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_Boolean")
                                .Returns(ReadAllTextHaxe("AfterChangeConstructorDeclaration_Boolean"))
                                .SetName("new Foo(true) -> function new(bool:Bool)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_ItemOfTwoDimensionalArrayInitializer")
                                .Returns(ReadAllTextHaxe("AfterChangeConstructorDeclaration_ItemOfTwoDimensionalArrayInitializer"))
                                .SetName("new Foo(strings[0][0]) -> function new(string:String)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_Dynamic")
                                .Returns(ReadAllTextHaxe("AfterChangeConstructorDeclaration_Dynamic"))
                                .SetName("new Foo({}) -> function new(dynamicValue:Dynamic)");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_issue1712_1")
                                .Returns(ReadAllTextHaxe("AfterChangeConstructorDeclaration_issue1712_1"))
                                .SetName("new Foo(new Array<haxe.Timer->Type.ValueType>()) -> function Foo(array:haxe.Timer->Type.ValueType)")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1712");
                        yield return
                            new TestCaseData("BeforeChangeConstructorDeclaration_issue1712_2")
                                .Returns(ReadAllTextHaxe("AfterChangeConstructorDeclaration_issue1712_2"))
                                .SetName("new Foo(new haxe.ds.Vector<Int>(0)) -> function Foo(vector:haxe.ds.Vector<Int>)")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1712");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string fileName) => HaxeImpl(fileName, sci);

                internal string AS3Impl(string fileName, ScintillaControl sci)
                {
                    SetAs3Features(sci);
                    SetCurrentFileName(GetFullPathAS3(fileName));
                    return Common(ReadAllTextAS3(fileName), sci);
                }

                internal string HaxeImpl(string fileName, ScintillaControl sci)
                {
                    SetHaxeFeatures(sci);
                    SetCurrentFileName(GetFullPathHaxe(fileName));
                    return Common(ReadAllTextHaxe(fileName), sci);
                }

                internal string Common(string sourceText, ScintillaControl sci)
                {
                    SetSrc(sci, sourceText);
                    ASGenerator.GenerateJob(GeneratorJobType.ChangeConstructorDecl, ASContext.Context.CurrentMember, ASContext.Context.CurrentClass, null, null);
                    return sci.Text;
                }
            }

            [TestFixture]
            public class GetStartOfStatement : GenerateJob
            {
                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(" new Vector.<int>()$(EntryPoint)", new ASResult {Type = new ClassModel {Flags = FlagType.Class }, Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" new <int>[]$(EntryPoint)", new ASResult {Type = new ClassModel {Flags = FlagType.Class }, Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" new <Object>[{}]$(EntryPoint)", new ASResult {Type = new ClassModel {Flags = FlagType.Class }, Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" new <Vector.<Object>>[new <Object>[{}]]$(EntryPoint)", new ASResult {Type = new ClassModel {Flags = FlagType.Class }, Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" new <Object>[{a:[new Number('10.0')]}]$(EntryPoint)", new ASResult {Type = new ClassModel {Flags = FlagType.Class }, Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" new Object()$(EntryPoint)", new ASResult {Type = new ClassModel {Flags = FlagType.Class }, Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" new Object(/*:)*/)$(EntryPoint)", new ASResult {Type = new ClassModel {Flags = FlagType.Class }, Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}})
                                .Returns(1);
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public int AS3(string sourceText, ASResult expr) => AS3Impl(sci, sourceText, expr);

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData(" new Array<Int>()$(EntryPoint)", new ASResult {Type = new ClassModel {Flags = FlagType.Class }, Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" new Map<String, Int>()$(EntryPoint)", new ASResult {Type = new ClassModel {Flags = FlagType.Class }, Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" new Map<String, Map<String, Int>>()$(EntryPoint)", new ASResult {Type = new ClassModel {Flags = FlagType.Class }, Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" new Map<String, Map<String, Void->Int>>()$(EntryPoint)", new ASResult {Type = new ClassModel {Flags = FlagType.Class }, Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" new Map<String, Map<String, String->Int->Void>>()$(EntryPoint)", new ASResult {Type = new ClassModel {Flags = FlagType.Class }, Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" new Array<Int->Int->String>()$(EntryPoint)", new ASResult {Type = new ClassModel {Flags = FlagType.Class }, Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" new Array<{x:Int, y:Int}>()$(EntryPoint)", new ASResult {Type = new ClassModel {Flags = FlagType.Class }, Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" new Array<{name:String, params:Array<Dynamic>}>()$(EntryPoint)", new ASResult {Type = new ClassModel {Flags = FlagType.Class }, Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" new Array<{name:String, factory:String->Dynamic}>()$(EntryPoint)", new ASResult {Type = new ClassModel {Flags = FlagType.Class }, Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" new Array<{name:String, factory:String->Array<String>}>()$(EntryPoint)", new ASResult {Type = new ClassModel {Flags = FlagType.Class }, Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" new Array<{name:String, factory:String->{x:Int, y:Int}}>()$(EntryPoint)", new ASResult {Type = new ClassModel {Flags = FlagType.Class }, Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" [1 => 1, 2 => 2]$(EntryPoint)", new ASResult {Type = ClassModel.VoidClass, Context = new ASExpr{PositionExpression = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" (1 > 2 ? 1 : 2)$(EntryPoint)", new ASResult {Type = ClassModel.VoidClass, Context = new ASExpr {PositionExpression = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" {v:1 > 2 ? 1 : 2}$(EntryPoint)", new ASResult {Type = ClassModel.VoidClass, Context = new ASExpr {PositionExpression = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" [new Array<String>()]$(EntryPoint)", new ASResult {Type = ClassModel.VoidClass, Context = new ASExpr {PositionExpression = 1}})
                                .Returns(1);
                        yield return
                            new TestCaseData(" test(type:Class<Dynamic>)$(EntryPoint)", new ASResult {Type = ClassModel.VoidClass, Context = new ASExpr {PositionExpression = 1}})
                                .Returns(1);
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public int Haxe(string sourceText, ASResult expr) => HaxeImpl(sci, sourceText, expr);

                internal static int AS3Impl(ScintillaControl sci, string sourceText, ASResult expr)
                {
                    SetAs3Features(sci);
                    return Common(sci, sourceText, expr);
                }

                internal static int HaxeImpl(ScintillaControl sci, string sourceText, ASResult expr)
                {
                    SetHaxeFeatures(sci);
                    return Common(sci, sourceText, expr);
                }

                internal static int Common(ScintillaControl sci, string sourceText, ASResult expr)
                {
                    SetSrc(sci, sourceText);
                    return ASGenerator.GetStartOfStatement(expr);
                }
            }

            [TestFixture]
            public class GetEndOfStatement : GenerateJob
            {
                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData("foo(/*:)*/)\nbar()\n   ")
                                .Returns("foo(/*:)*/)\n".Length);
                        yield return
                            new TestCaseData("foo(\"(.)(.) <-- :)\")\nbar()\n   ")
                                .Returns("foo(\"(.)(.) <-- :)\")\n".Length);
                        yield return
                            new TestCaseData("foo('(.)(.) <-- :)')\nbar()\n   ")
                                .Returns("foo('(.)(.) <-- :)')\n".Length);
                        yield return
                            new TestCaseData("foo('\\'(.)(.) <-- :)\\'')\nbar()\n   ")
                                .Returns("foo('\\'(.)(.) <-- :)\\'')\n".Length);
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public int Haxe(string sourceText) => HaxeImpl(sourceText, sci);

                internal static int HaxeImpl(string sourceText, ScintillaControl sci)
                {
                    SetHaxeFeatures(sci);
                    return Common(sci, sourceText);
                }

                internal static int Common(ScintillaControl sci, string sourceText)
                {
                    SetSrc(sci, sourceText);
                    return ASGenerator.GetEndOfStatement(0, sci.TextLength, sci);
                }
            }

            [TestFixture]
            public class AvoidKeywordTests : GenerateJob
            {
                static IEnumerable<TestCaseData> AS3TestCases
                {
                    get
                    {
                        yield return new TestCaseData("import").Returns("importValue");
                        yield return new TestCaseData("new").Returns("newValue");
                        yield return new TestCaseData("typeof").Returns("typeofValue");
                        yield return new TestCaseData("is").Returns("isValue");
                        yield return new TestCaseData("as").Returns("asValue");
                        yield return new TestCaseData("extends").Returns("extendsValue");
                        yield return new TestCaseData("implements").Returns("implementsValue");
                        yield return new TestCaseData("var").Returns("varValue");
                        yield return new TestCaseData("function").Returns("functionValue");
                        yield return new TestCaseData("const").Returns("constValue");
                        yield return new TestCaseData("delete").Returns("deleteValue");
                        yield return new TestCaseData("return").Returns("returnValue");
                        yield return new TestCaseData("break").Returns("breakValue");
                        yield return new TestCaseData("continue").Returns("continueValue");
                        yield return new TestCaseData("if").Returns("ifValue");
                        yield return new TestCaseData("else").Returns("elseValue");
                        yield return new TestCaseData("for").Returns("forValue");
                        yield return new TestCaseData("each").Returns("eachValue");
                        yield return new TestCaseData("in").Returns("inValue");
                        yield return new TestCaseData("while").Returns("whileValue");
                        yield return new TestCaseData("do").Returns("doValue");
                        yield return new TestCaseData("switch").Returns("switchValue");
                        yield return new TestCaseData("case").Returns("caseValue");
                        yield return new TestCaseData("default").Returns("defaultValue");
                        yield return new TestCaseData("with").Returns("withValue");
                        yield return new TestCaseData("null").Returns("nullValue");
                        yield return new TestCaseData("true").Returns("trueValue");
                        yield return new TestCaseData("false").Returns("falseValue");
                        yield return new TestCaseData("try").Returns("tryValue");
                        yield return new TestCaseData("catch").Returns("catchValue");
                        yield return new TestCaseData("finally").Returns("finallyValue");
                        yield return new TestCaseData("throw").Returns("throwValue");
                        yield return new TestCaseData("use").Returns("useValue");
                        yield return new TestCaseData("namespace").Returns("namespaceValue");
                        yield return new TestCaseData("native").Returns("nativeValue");
                        yield return new TestCaseData("dynamic").Returns("dynamicValue");
                        yield return new TestCaseData("final").Returns("finalValue");
                        yield return new TestCaseData("private").Returns("privateValue");
                        yield return new TestCaseData("public").Returns("publicValue");
                        yield return new TestCaseData("protected").Returns("protectedValue");
                        yield return new TestCaseData("internal").Returns("internalValue");
                        yield return new TestCaseData("static").Returns("staticValue");
                        yield return new TestCaseData("override").Returns("overrideValue");
                        yield return new TestCaseData("get").Returns("getValue");
                        yield return new TestCaseData("set").Returns("setValue");
                        yield return new TestCaseData("class").Returns("classValue");
                        yield return new TestCaseData("interface").Returns("interfaceValue");
                    }
                }

                [Test, TestCaseSource(nameof(AS3TestCases))]
                public string AS3(string sourceText) => AS3Impl(sourceText, sci);

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return new TestCaseData("import").Returns("importValue");
                        yield return new TestCaseData("new").Returns("newValue");
                        yield return new TestCaseData("extends").Returns("extendsValue");
                        yield return new TestCaseData("implements").Returns("implementsValue");
                        yield return new TestCaseData("using").Returns("usingValue");
                        yield return new TestCaseData("var").Returns("varValue");
                        yield return new TestCaseData("function").Returns("functionValue");
                        yield return new TestCaseData("cast").Returns("castValue");
                        yield return new TestCaseData("return").Returns("returnValue");
                        yield return new TestCaseData("break").Returns("breakValue");
                        yield return new TestCaseData("continue").Returns("continueValue");
                        yield return new TestCaseData("if").Returns("ifValue");
                        yield return new TestCaseData("else").Returns("elseValue");
                        yield return new TestCaseData("for").Returns("forValue");
                        yield return new TestCaseData("in").Returns("inValue");
                        yield return new TestCaseData("while").Returns("whileValue");
                        yield return new TestCaseData("do").Returns("doValue");
                        yield return new TestCaseData("switch").Returns("switchValue");
                        yield return new TestCaseData("case").Returns("caseValue");
                        yield return new TestCaseData("default").Returns("defaultValue");
                        yield return new TestCaseData("untyped").Returns("untypedValue");
                        yield return new TestCaseData("null").Returns("nullValue");
                        yield return new TestCaseData("true").Returns("trueValue");
                        yield return new TestCaseData("false").Returns("falseValue");
                        yield return new TestCaseData("try").Returns("tryValue");
                        yield return new TestCaseData("catch").Returns("catchValue");
                        yield return new TestCaseData("throw").Returns("throwValue");
                        yield return new TestCaseData("trace").Returns("traceValue");
                        yield return new TestCaseData("macro").Returns("macroValue");
                        yield return new TestCaseData("dynamic").Returns("dynamicValue");
                        yield return new TestCaseData("private").Returns("privateValue");
                        yield return new TestCaseData("public").Returns("publicValue");
                        yield return new TestCaseData("inline").Returns("inlineValue");
                        yield return new TestCaseData("extern").Returns("externValue");
                        yield return new TestCaseData("static").Returns("staticValue");
                        yield return new TestCaseData("override").Returns("overrideValue");
                        yield return new TestCaseData("class").Returns("classValue");
                        yield return new TestCaseData("interface").Returns("interfaceValue");
                        yield return new TestCaseData("typedef").Returns("typedefValue");
                        yield return new TestCaseData("enum").Returns("enumValue");
                        yield return new TestCaseData("abstract").Returns("abstractValue");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string sourceText) => HaxeImpl(sourceText, sci);

                internal static string AS3Impl(string sourceText, ScintillaControl sci)
                {
                    SetAs3Features(sci);
                    return Common(sourceText);
                }

                internal static string HaxeImpl(string sourceText, ScintillaControl sci)
                {
                    SetHaxeFeatures(sci);
                    return Common(sourceText);
                }

                internal static string Common(string sourceText) => ASGenerator.AvoidKeyword(sourceText);
            }

            [TestFixture]
            public class GenerateDelegateMethods : GenerateJob
            {
                [TestFixtureSetUp]
                public void GenerateDelegateMethodsSetup() => ASContext.Context.Settings.GenerateImports.Returns(true);

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData("BeforeGenerateDelegateMethod")
                                .Returns(ReadAllTextHaxe("AfterGenerateDelegateMethod"));
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public string Haxe(string fileName) => HaxeImpl(fileName, sci);

                internal string HaxeImpl(string fileName, ScintillaControl sci)
                {
                    SetHaxeFeatures(sci);
                    SetCurrentFileName(GetFullPathHaxe(fileName));
                    return Common(ReadAllTextHaxe(fileName), sci);
                }

                internal string Common(string sourceText, ScintillaControl sci)
                {
                    SetSrc(sci, sourceText);
                    var type = ASContext.Context.ResolveType(ASContext.Context.CurrentMember.Type, ASContext.Context.CurrentModel);
                    var selectedMembers = new Dictionary<MemberModel, ClassModel>();
                    foreach (var it in type.Members.Items)
                    {
                        selectedMembers[it] = ASContext.Context.ResolveType(it.Type, it.InFile);
                    }
                    ASGenerator.GenerateDelegateMethods(sci, ASContext.Context.CurrentMember, selectedMembers, type, ASContext.Context.CurrentClass);
                    return sci.Text;
                }
            }

            [TestFixture]
            public class GenerateClassTests : GenerateJob
            {
                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return
                            new TestCaseData("BeforeGenerateClassTest_issue1762_1", "$(Boundary)dynamicValue:Dynamic$(Boundary)")
                                .SetName("Issue1762. Case 1")
                                .SetDescription("https://github.com/fdorg/flashdevelop/issues/1762");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public void Haxe(string fileName, string constructorArgs) => HaxeImpl(fileName, constructorArgs, sci);

                static void HaxeImpl(string fileName, string constructorArgs, ScintillaControl sci)
                {
                    SetHaxeFeatures(sci);
                    SetCurrentFileName(GetFullPathHaxe(fileName));
                    Common(ReadAllTextHaxe(fileName), constructorArgs, sci);
                }

                static void Common(string sourceText, string constructorArgs, ScintillaControl sci)
                {
                    var handler = Substitute.For<IEventHandler>();
                    handler
                        .When(it => it.HandleEvent(Arg.Any<object>(), Arg.Any<NotifyEvent>(), Arg.Any<HandlingPriority>()))
                        .Do(it =>
                        {
                            var e = it.ArgAt<NotifyEvent>(1);
                            switch (e.Type)
                            {
                                case EventType.Command:
                                    EventManager.RemoveEventHandler(handler);
                                    e.Handled = true;
                                    var de = (DataEvent) e;
                                    var info = (Hashtable) de.Data;
                                    var actualArgs = (string) info[nameof(constructorArgs)];
                                    Assert.AreEqual(constructorArgs, actualArgs);
                                    break;
                            }
                        });
                    EventManager.AddEventHandler(handler, EventType.Command);
                    SetSrc(sci, sourceText);
                    var options = new List<ICompletionListItem>();
                    ASGenerator.ContextualGenerator(sci, options);
                    var item = options.Find(it => ((GeneratorItem)it).job == GeneratorJobType.Class);
                    var value = item.Value;
                }
            }

            [TestFixture]
            public class ContextualGeneratorTests : GenerateJob
            {
                [TestFixtureSetUp]
                public void Setup()
                {
                    ASContext.Context.Settings.GenerateImports.Returns(true);
                    SetAs3Features(sci);
                }

                static IEnumerable<TestCaseData> ContextualGeneratorTestCases
                {
                    get
                    {
                        yield return new TestCaseData("BeforeGenerateFunction_1", GeneratorJobType.Function, true)
                            .Returns(ReadAllTextAS3("AfterGenerateFunction_1"))
                            .SetName("Generate function. case 1");
                        yield return new TestCaseData("BeforeImplementInterfaceMethods", GeneratorJobType.ImplementInterface, true)
                            .Returns(ReadAllTextAS3("AfterImplementInterfaceMethods"))
                            .SetName("Implement interface methods. Issue 1684")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1684");
                        yield return new TestCaseData("BeforeGenerateVariable_1", GeneratorJobType.VariablePublic, true)
                            .Returns(ReadAllTextAS3("AfterGenerateVariable_1"))
                            .SetName("Generate variable. case 1");
                    }
                }

                [Test, TestCaseSource(nameof(ContextualGeneratorTestCases))]
                public string ContextualGenerator(string fileName, GeneratorJobType job, bool hasGenerator) => Common(sci, fileName, job, hasGenerator);

                internal static string Common(ScintillaControl sci, string fileName, GeneratorJobType job, bool hasGenerator)
                {
                    SetCurrentFileName(GetFullPathAS3(fileName));
                    SetSrc(sci, ReadAllTextAS3(fileName));
                    var options = new List<ICompletionListItem>();
                    ASGenerator.ContextualGenerator(sci, options);
                    if (hasGenerator)
                    {
                        Assert.IsNotEmpty(options);
                        var item = options.Find(it => ((ASCompletion.Completion.GeneratorItem)it).job == job);
                        Assert.IsNotNull(item);
                        var value = item.Value;
                        return sci.Text;
                    }
                    if (job == (GeneratorJobType)(-1)) Assert.IsEmpty(options);
                    if (options.Count > 0) Assert.IsFalse(options.Any(it => ((ASCompletion.Completion.GeneratorItem)it).job == job));
                    return null;
                }
            }

            [TestFixture]
            public class ContextualGeneratorTests2 : GenerateJob
            {
                [TestFixtureSetUp]
                public void Setup() => ASContext.Context.Settings.GenerateImports.Returns(true);

                static IEnumerable<TestCaseData> HaxeTestCases
                {
                    get
                    {
                        yield return new TestCaseData("ContextualGenerator_issue1984_1", false)
                            .SetName("Issue1984. Case 1")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                        yield return new TestCaseData("ContextualGenerator_issue1984_2", false)
                            .SetName("Issue1984. Case 2")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                        yield return new TestCaseData("ContextualGenerator_issue1984_3", false)
                            .SetName("Issue1984. Case 3")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                        yield return new TestCaseData("ContextualGenerator_issue1984_4", false)
                            .SetName("Issue1984. Case 4")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                        yield return new TestCaseData("ContextualGenerator_issue1984_5", false)
                            .SetName("Issue1984. Case 5")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                        yield return new TestCaseData("ContextualGenerator_issue1984_6", false)
                            .SetName("Issue1984. Case 6")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                        yield return new TestCaseData("ContextualGenerator_issue1984_7", false)
                            .SetName("Issue1984. Case 7")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                        yield return new TestCaseData("ContextualGenerator_issue1984_8", false)
                            .SetName("Issue1984. Case 8")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                        yield return new TestCaseData("ContextualGenerator_issue1984_9", false)
                            .SetName("Issue1984. Case 9")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                        yield return new TestCaseData("ContextualGenerator_issue1984_10", false)
                            .SetName("Issue1984. Case 10")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1984");
                        yield return new TestCaseData("ContextualGenerator_issue1987_1", false)
                            .SetName("Issue1987. Case 1")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1987");
                        yield return new TestCaseData("ContextualGenerator_issue1987_2", false)
                            .SetName("Issue1987. Case 2")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1987");
                        yield return new TestCaseData("ContextualGenerator_issue1987_3", false)
                            .SetName("Issue1987. Case 3")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1987");
                        yield return new TestCaseData("ContextualGenerator_issue1987_4", false)
                            .SetName("Issue1987. Case 4")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1987");
                        yield return new TestCaseData("ContextualGenerator_issue1995_1", false)
                            .SetName("Issue1995. Case 1")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1995");
                        yield return new TestCaseData("ContextualGenerator_issue1995_2", false)
                            .SetName("Issue1995. Case 2")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1995");
                        yield return new TestCaseData("ContextualGenerator_issue1995_3", false)
                            .SetName("Issue1995. Case 3")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1995");
                        yield return new TestCaseData("ContextualGenerator_issue1995_4", true)
                            .SetName("Issue1995. Case 4")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1995");
                        yield return new TestCaseData("ContextualGenerator_issue1995_5", true)
                            .SetName("Issue1995. Case 5")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1995");
                        yield return new TestCaseData("ContextualGenerator_issue1995_6", false)
                            .SetName("Issue1995. Case 6")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1995");
                        yield return new TestCaseData("ContextualGenerator_issue1995_7", false)
                            .SetName("Issue1995. Case 7")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1995");
                        yield return new TestCaseData("ContextualGenerator_issue1995_8", false)
                            .SetName("Issue1995. Case 8")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/2005");
                    }
                }

                [Test, TestCaseSource(nameof(HaxeTestCases))]
                public void Haxe(string fileName, bool hasGenerator) => Common(sci, fileName, hasGenerator);

                internal static void Common(ScintillaControl sci, string fileName, bool hasGenerator)
                {
                    SetHaxeFeatures(sci);
                    SetCurrentFileName(GetFullPathHaxe(fileName));
                    SetSrc(sci, ReadAllTextHaxe(fileName));
                    var options = new List<ICompletionListItem>();
                    ASGenerator.ContextualGenerator(sci, options);
                    if (hasGenerator) Assert.IsNotEmpty(options);
                    else Assert.IsEmpty(options);
                }
            }
        }

        static readonly string testFilesAssemblyPath = $"\\FlashDevelop\\Bin\\Debug\\{nameof(ASCompletion)}\\Test_Files\\";
        static readonly string testFilesDirectory = $"\\Tests\\External\\Plugins\\{nameof(ASCompletion)}.Tests\\Test Files\\";

        protected static void SetCurrentFileName(string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName).Replace('.', Path.DirectorySeparatorChar) + Path.GetExtension(fileName);
            fileName = Path.GetFullPath(fileName);
            fileName = fileName.Replace(testFilesAssemblyPath, testFilesDirectory);
            ASContext.Context.CurrentModel.FileName = fileName;
            PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
        }

        protected static string ReadAllTextAS3(string fileName) => TestFile.ReadAllText(GetFullPathAS3(fileName));

        protected static string GetFullPathAS3(string fileName) => $"ASCompletion.Test_Files.generated.as3.{fileName}.as";

        protected static string ReadAllTextHaxe(string fileName) => TestFile.ReadAllText(GetFullPathHaxe(fileName));

        protected static string GetFullPathHaxe(string fileName) => $"ASCompletion.Test_Files.generated.haxe.{fileName}.hx";
    }
}