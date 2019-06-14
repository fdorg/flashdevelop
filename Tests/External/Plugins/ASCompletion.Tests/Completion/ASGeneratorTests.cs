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
        static readonly string testFilesAssemblyPath = $"\\FlashDevelop\\Bin\\Debug\\{nameof(ASCompletion)}\\Test_Files\\";

        static readonly string testFilesDirectory = $"\\Tests\\External\\Plugins\\{nameof(ASCompletion)}.Tests\\Test Files\\";

        static void SetCurrentFileName(string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName).Replace('.', Path.DirectorySeparatorChar) + Path.GetExtension(fileName);
            fileName = Path.GetFullPath(fileName);
            fileName = fileName.Replace(testFilesAssemblyPath, testFilesDirectory);
            ASContext.Context.CurrentModel.FileName = fileName;
            PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
        }

        static string GetFullPath(string fileName) => $"{nameof(ASCompletion)}.Test_Files.generated.as3.{fileName}.as";

        static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

        [TestFixtureSetUp]
        public void Setup()
        {
            ASContext.CommonSettings.DeclarationModifierOrder = new[] {"public", "protected", "internal", "private", "static", "inline", "override"};
            SetAs3Features(sci);
        }

        static IEnumerable<TestCaseData> GetBodyStartTestCases
        {
            get
            {
                yield return new TestCaseData("function test():void{\r\n\t\t\t\r\n}", 0, 1,
                    "function test():void{\r\n\t\t\t\r\n}", 26).SetName("SimpleCase");
                // Should we reindent the second line?
                yield return new TestCaseData("function test():void{\r\n\t\t\t}", 0, 1,
                    "function test():void{\r\n\t\t\t\r\n}", 26).SetName("EndOnSecondLine");
                yield return new TestCaseData("function test():void{\r\n}", 0, 1, "function test():void{\r\n\t\r\n}",
                    24).SetName("EndOnSecondLineNoExtraIndent");
                yield return new TestCaseData("function test():void{\r\n\t\t\t//comment}", 0, 1,
                    "function test():void{\r\n\t\t\t//comment}", 26).SetName("CharOnSecondLine");
                yield return new TestCaseData("function test():void{}", 0, 0, "function test():void{\r\n\t\r\n}", 24)
                    .SetName("EndOnSameDeclarationLine");
                yield return new TestCaseData("function test():void\r\n\r\n{}\r\n", 0, 2,
                    "function test():void\r\n\r\n{\r\n\t\r\n}\r\n", 28).SetName("EndOnSameLine");
                yield return new TestCaseData("function test():void {trace(1);}", 0, 0,
                    "function test():void {\r\n\ttrace(1);}", 25).SetName("TextOnStartLine");
                yield return new TestCaseData("function test(arg:String='{', arg2:String=\"{\"):void/*{*/{\r\n}", 0, 1,
                        "function test(arg:String='{', arg2:String=\"{\"):void/*{*/{\r\n\t\r\n}", 60)
                    .SetName("BracketInCommentsOrText");
                yield return new TestCaseData("function test():void/*áéíóú*/\r\n{}", 0, 1,
                    "function test():void/*áéíóú*/\r\n{\r\n\t\r\n}", 40).SetName("MultiByteCharacters");
                yield return new TestCaseData("function tricky():void {} function test():void{\r\n\t\t\t}", 0, 1,
                        "function tricky():void {} function test():void{\r\n\t\t\t}", 49)
                    .SetName("WithAnotherMemberInTheSameLine")
                    .Ignore(
                        "Having only LineFrom and LineTo for members is not enough to handle these cases. FlashDevelop in general is not too kind when it comes to several members in the same line, but we could change the method to use positions and try to get the proper position before.");
                yield return new TestCaseData("function test<T:{}>(arg:T):void{\r\n\r\n}", 0, 1,
                    "function test<T:{}>(arg:T):void{\r\n\r\n}", 34).SetName("BracketsInGenericConstraint");
                yield return new TestCaseData("function test(arg:{x:Int}):void{\r\n\r\n}", 0, 1,
                    "function test(arg:{x:Int}):void{\r\n\r\n}", 34).SetName("AnonymousStructures");
            }
        }

        [Test, TestCaseSource(nameof(GetBodyStartTestCases))]
        public void GetBodyStart(string text, int lineStart, int lineEnd, string resultText, int bodyStart)
        {
            sci.Text = text;
            sci.ConfigurationLanguage = "haxe";
            sci.Colourise(0, -1);
            int funcBodyStart = ASGenerator.GetBodyStart(lineStart, lineEnd, sci);

            Assert.AreEqual(bodyStart, funcBodyStart);
            Assert.AreEqual(resultText, sci.Text);
        }

        static IEnumerable<TestCaseData> GenerateFieldFromParameterTestCases
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
                    .Returns(ReadAllText("FieldFromParameterEmptyBody"))
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
                    .Returns(ReadAllText("FieldFromParameterWithSuperConstructor"))
                    .SetName("PublicScopeWithSuperConstructor");

                yield return new TestCaseData(Visibility.Public,
                        ReadAllText("BeforeFieldFromParameterWithSuperConstructorMultiLine"),
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
                    .Returns(ReadAllText("FieldFromParameterWithSuperConstructorMultiLine"))
                    .SetName("PublicScopeWithSuperConstructorMultiLine");

                yield return new TestCaseData(Visibility.Public,
                        ReadAllText("BeforeFieldFromParameterWithWrongSuperConstructor"),
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
                    .Returns(ReadAllText("FieldFromParameterWithWrongSuperConstructor"))
                    .SetName("PublicScopeWithWrongSuperConstructor");
            }
        }

        [Test, TestCaseSource(nameof(GenerateFieldFromParameterTestCases))]
        public string GenerateFieldFromParameter(Visibility scope, string sourceText, ClassModel inClass, int memberPos, int parameterPos)
        {
            sci.Text = sourceText;
            var sourceMember = inClass.Members[memberPos];
            ASGenerator.SetJobContext(null, null, sourceMember.Parameters[parameterPos], null);
            ASGenerator.GenerateJob(GeneratorJobType.FieldFromParameter, sourceMember, inClass, null, new Hashtable {["scope"] = scope});
            return sci.Text;
        }

        static IEnumerable<TestCaseData> PromoteLocalTestCases
        {
            get
            {
                yield return new TestCaseData("BeforePromoteLocal")
                    .Returns(ReadAllText("AfterPromoteLocal_generateExplicitScopeIsFalse"))
                    .SetName("Promote to class member");
            }
        }

        static IEnumerable<TestCaseData> PromoteLocalWithExplicitScopeTestCases
        {
            get
            {
                yield return new TestCaseData("BeforePromoteLocal")
                    .Returns(ReadAllText("AfterPromoteLocal_generateExplicitScopeIsTrue"))
                    .SetName("Promote to class member");
            }
        }

        [Test, TestCaseSource(nameof(PromoteLocalWithExplicitScopeTestCases))]
        public string PromoteLocalWithExplicitScope(string fileName)
        {
            ASContext.CommonSettings.GenerateScope = true;
            var result = PromoteLocalImpl(fileName);
            ASContext.CommonSettings.GenerateScope = false;
            return result;
        }

        [Test, TestCaseSource(nameof(PromoteLocalTestCases))]
        public string PromoteLocalImpl(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            var expr = ASComplete.GetExpressionType(sci, sci.CurrentPos);
            ASGenerator.contextMember = expr.Context.LocalVars[0];
            var options = new List<ICompletionListItem>();
            ASGenerator.ContextualGenerator(sci, options);
            var item = options.Find(it => ((GeneratorItem) it).Job == GeneratorJobType.PromoteLocal);
            var value = item.Value;
            return sci.Text;
        }

        static IEnumerable<TestCaseData> GenerateFunctionTestCases
        {
            get
            {
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateFunction"), GeneratorJobType.Function)
                        .Returns(ReadAllText("AfterGeneratePrivateFunction_generateExplicitScopeIsFalse"))
                        .SetName("Generate private function");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateFunction"), GeneratorJobType.FunctionPublic)
                        .Returns(ReadAllText("AfterGeneratePublicFunction_generateExplicitScopeIsFalse"))
                        .SetName("Generate public function");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateFunction_forSomeObj"), GeneratorJobType.FunctionPublic)
                        .Returns(ReadAllText("AfterGenerateFunction_forSomeObj"))
                        .SetName("From some.foo|();");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateFunction_forSomeObj2"), GeneratorJobType.FunctionPublic)
                        .Returns(ReadAllText("AfterGenerateFunction_forSomeObj2"))
                        .SetName("From new Some().foo|();");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateFunction_forSomeObj3"), GeneratorJobType.FunctionPublic)
                        .Returns(ReadAllText("AfterGenerateFunction_forSomeObj3"))
                        .SetName("From new Some()\n.foo|();");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateStaticFunction"), GeneratorJobType.FunctionPublic)
                        .Returns(ReadAllText("AfterGeneratePublicStaticFunction_generateExplicitScopeIsFalse"))
                        .SetName("Generate public static function");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateStaticFunction_forCurrentType"),
                            GeneratorJobType.FunctionPublic)
                        .Returns(ReadAllText("AfterGeneratePublicStaticFunction_generateExplicitScopeIsTrue"))
                        .SetName("From CurrentType.foo|");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateStaticFunction_forSomeType"),
                            GeneratorJobType.FunctionPublic)
                        .Returns(ReadAllText("AfterGeneratePublicStaticFunction_forSomeType"))
                        .SetName("From SomeType.foo|");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateFunction_issue1436"), GeneratorJobType.Function)
                        .Returns(ReadAllText("AfterGeneratePrivateFunction_issue1436"))
                        .SetName("From foo(vector[0])")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1436");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateFunction_TwoDimensionalVector_issue1436"),
                            GeneratorJobType.Function)
                        .Returns(ReadAllText("AfterGeneratePrivateFunction_TwoDimensionalVector_issue1436"))
                        .SetName("From foo(vector[0][0])")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1436");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateFunction_MultidimensionalVector_issue1436"),
                            GeneratorJobType.Function)
                        .Returns(ReadAllText("AfterGeneratePrivateFunction_MultidimensionalVector_issue1436"))
                        .SetName("From foo(vector[0][0][0][0])")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1436");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateFunction_issue103"), GeneratorJobType.Function)
                        .Returns(ReadAllText("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103"))
                        .SetName("Issue 103. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateFunction_issue103_2"), GeneratorJobType.Function)
                        .Returns(
                            ReadAllText("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_2"))
                        .SetName("Issue 103. Case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateFunction_issue103_3"), GeneratorJobType.Function)
                        .Returns(
                            ReadAllText("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_3"))
                        .SetName("Issue 103. Case 3")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateFunction_issue103_4"), GeneratorJobType.Function)
                        .Returns(
                            ReadAllText("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_4"))
                        .SetName("Issue 103. Case 4")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateFunction_issue103_5"), GeneratorJobType.Function)
                        .Returns(
                            ReadAllText("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_5"))
                        .SetName("Issue 103. Case 5")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateFunction_issue103_6"), GeneratorJobType.Function)
                        .Returns(
                            ReadAllText("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_6"))
                        .SetName("Issue 103. Case 6")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateFunction_issue1645"), GeneratorJobType.Function)
                        .Returns(ReadAllText("AfterGenerateFunction_issue1645"))
                        .SetName("Issue 1645. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1645");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateFunction_issue1645_2"), GeneratorJobType.Function)
                        .Returns(ReadAllText(
                            "AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue1645_2"))
                        .SetName("Issue 1645. Case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1645");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateFunction_issue1780_1"), GeneratorJobType.Function)
                        .Returns(ReadAllText("AfterGenerateFunction_issue1780_1"))
                        .SetName("foo(Math.round(1.5))")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1780");
                yield return
                    new TestCaseData(ReadAllText("BeforeGenerateFunction_issue1780_2"), GeneratorJobType.Function)
                        .Returns(ReadAllText("AfterGenerateFunction_issue1780_2"))
                        .SetName("foo(round(1.5))")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1780");
            }
        }

        [Test, TestCaseSource(nameof(GenerateFunctionTestCases))]
        public string GenerateFunction(string sourceText, GeneratorJobType job) => GenerateFunctionImpl(sourceText, job, sci);

        static string GenerateFunctionImpl(string sourceText, GeneratorJobType job, ScintillaControl sci)
        {
            SetSrc(sci, sourceText);
            var options = new List<ICompletionListItem>();
            ASGenerator.ContextualGenerator(sci, options);
            var item = options.Find(it => ((GeneratorItem) it).Job == job);
            var value = item.Value;
            return sci.Text;
        }

        static IEnumerable<TestCaseData> GenerateFunctionWithReturnDefaultValueTestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("BeforeGenerateFunction_issue103"), GeneratorJobType.Function)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103"))
                    .SetName("Issue 103. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData(ReadAllText("BeforeGenerateFunction_issue103_2"), GeneratorJobType.Function)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_2"))
                    .SetName("Issue 103. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData(ReadAllText("BeforeGenerateFunction_issue103_3"), GeneratorJobType.Function)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_3"))
                    .SetName("Issue 103. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData(ReadAllText("BeforeGenerateFunction_issue103_4"), GeneratorJobType.Function)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_4"))
                    .SetName("Issue 103. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData(ReadAllText("BeforeGenerateFunction_issue103_5"), GeneratorJobType.Function)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_5"))
                    .SetName("Issue 103. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData(ReadAllText("BeforeGenerateFunction_issue103_6"), GeneratorJobType.Function)
                    .Returns(ReadAllText("AfterGenerateFunction_issue103_6"))
                    .SetName("Issue 103. Case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData(ReadAllText("BeforeGenerateFunction_issue1645_2"), GeneratorJobType.Function)
                    .Returns(ReadAllText("AfterGenerateFunction_issue1645_2"))
                    .SetName("Issue 1645. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1645");
            }
        }

        [Test, TestCaseSource(nameof(GenerateFunctionWithReturnDefaultValueTestCases))]
        public string GenerateFunctionWithReturnDefaultValue(string sourceText, GeneratorJobType job)
        {
            ASContext.CommonSettings.GeneratedMemberDefaultBodyStyle = GeneratedMemberBodyStyle.ReturnDefaultValue;
            var result = GenerateFunctionImpl(sourceText, job, sci);
            ASContext.CommonSettings.GeneratedMemberDefaultBodyStyle = GeneratedMemberBodyStyle.UncompilableCode;
            return result;
        }

        static IEnumerable<TestCaseData> GenerateFunctionWithExplicitScopeTestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("BeforeGenerateFunction"), GeneratorJobType.Function)
                    .Returns(ReadAllText("AfterGeneratePrivateFunction_generateExplicitScopeIsTrue"))
                    .SetName("Generate private function from member scope");
                yield return new TestCaseData(ReadAllText("BeforeGenerateFunction"), GeneratorJobType.FunctionPublic)
                    .Returns(ReadAllText("AfterGeneratePublicFunction_generateExplicitScopeIsTrue"))
                    .SetName("Generate public function from member scope");
                yield return new TestCaseData(ReadAllText("BeforeGenerateFunction_forSomeObj"), GeneratorJobType.FunctionPublic)
                    .Returns(ReadAllText("AfterGenerateFunction_forSomeObj"))
                    .SetName("From some.foo|();");
                yield return new TestCaseData(ReadAllText("BeforeGenerateFunction_forSomeObj2"), GeneratorJobType.FunctionPublic)
                    .Returns(ReadAllText("AfterGenerateFunction_forSomeObj2"))
                    .SetName("From new Some().foo|();");
                yield return new TestCaseData(ReadAllText("BeforeGenerateFunction_forSomeObj3"), GeneratorJobType.FunctionPublic)
                    .Returns(ReadAllText("AfterGenerateFunction_forSomeObj3"))
                    .SetName("From new Some()\n.foo|();");
                yield return new TestCaseData(ReadAllText("BeforeGenerateStaticFunction"), GeneratorJobType.FunctionPublic)
                    .Returns(ReadAllText("AfterGeneratePublicStaticFunction_generateExplicitScopeIsTrue"))
                    .SetName("Generate public static function");
                yield return new TestCaseData(ReadAllText("BeforeGenerateStaticFunction_forCurrentType"), GeneratorJobType.FunctionPublic)
                    .Returns(ReadAllText("AfterGeneratePublicStaticFunction_generateExplicitScopeIsTrue"))
                    .SetName("From CurrentType.foo|");
                yield return new TestCaseData(ReadAllText("BeforeGenerateStaticFunction_forSomeType"), GeneratorJobType.FunctionPublic)
                    .Returns(ReadAllText("AfterGeneratePublicStaticFunction_forSomeType"))
                    .SetName("From SomeType.foo|");
                yield return new TestCaseData(ReadAllText("BeforeGenerateFunction2"), GeneratorJobType.Function)
                    .Returns(ReadAllText("AfterGeneratePrivateFunction2_generateExplicitScopeIsTrue"))
                    .SetName("Generate private function from class scope");
            }
        }

        [Test, TestCaseSource(nameof(GenerateFunctionWithExplicitScopeTestCases))]
        public string GenerateFunctionWithExplicitScope(string sourceText, GeneratorJobType job)
        {
            ASContext.CommonSettings.GenerateScope = true;
            var result = GenerateFunctionImpl(sourceText, job, sci);
            ASContext.CommonSettings.GenerateScope = false;
            return result;
        }

        static IEnumerable<TestCaseData> GenerateFunctionWithProtectedDeclarationTestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("BeforeGenerateFunction"), GeneratorJobType.Function)
                    .Returns(ReadAllText("AfterGenerateProtectedFunction"))
                    .SetName("Generate private function with protected modifier declaration");
            }
        }

        [Test, TestCaseSource(nameof(GenerateFunctionWithProtectedDeclarationTestCases))]
        public string GenerateFunctionWithProtectedDeclaration(string sourceText, GeneratorJobType job)
        {
            ASContext.CommonSettings.GenerateProtectedDeclarations = true;
            var result = GenerateFunctionImpl(sourceText, job, sci);
            ASContext.CommonSettings.GenerateProtectedDeclarations = false;
            return result;
        }

        static IEnumerable<TestCaseData> AssignStatementToVarTestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromVectorInitializer"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromVectorInitializer"))
                    .SetName("new <int>[]|");
                yield return new TestCaseData(
                        ReadAllText("BeforeAssignStatementToVarFromTwoDimensionalVectorInitializer"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromTwoDimensionalVectorInitializer"))
                    .SetName("new <Vector.<int>>[new <int>[]]|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromNewVector"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromNewVector"))
                    .SetName("new Vector.<Vector.<int>>()|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromFieldOfItemOfVector"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromFieldOfItemOfVector"))
                    .SetName("v[0][0].length|");
                yield return new TestCaseData(
                        ReadAllText("BeforeAssignStatementToVarFromMultilineArrayInitializer_useSpaces"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromMultilineArrayInitializer_useSpaces"))
                    .SetName("From multiline array initializer");
                yield return new TestCaseData(
                        ReadAllText("BeforeAssignStatementToVarFromMultilineObjectInitializer_useSpaces"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromMultilineObjectInitializer_useSpaces"))
                    .SetName("From multiline object initializer");
                yield return new TestCaseData(
                        ReadAllText("BeforeAssignStatementToVarFromMultilineObjectInitializer2_useSpaces"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromMultilineObjectInitializer2_useSpaces"))
                    .SetName("From multiline object initializer 2");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromMethodChaining_useSpaces"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromMethodChaining_useSpaces"))
                    .SetName("From method chaining");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromMethodChaining2_useSpaces"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromMethodChaining2_useSpaces"))
                    .SetName("From method chaining 2");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromNewString_useSpaces"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromNewString_useSpaces"))
                    .SetName("new String(\"\")|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromStringInitializer_useSpaces"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromStringInitializer_useSpaces"))
                    .SetName("\"\"|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromStringInitializer2_useSpaces"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromStringInitializer2_useSpaces"))
                    .SetName("''|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromNewString2_useSpaces"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromNewString2_useSpaces"))
                    .SetName("new String(\"\".charAt(0))|");
                yield return new TestCaseData(
                        ReadAllText("BeforeAssignStatementToVarFromNewBitmapDataWithParams_useSpaces"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromNewBitmapDataWithParams_useSpaces"))
                    .SetName("new BitmapData(rect.width, rect.height)| . Case 1");
                yield return new TestCaseData(
                        ReadAllText("BeforeAssignStatementToVarFromNewBitmapDataWithParams_multiline_useSpaces"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromNewBitmapDataWithParams_multiline_useSpaces"))
                    .SetName("new BitmapData(rect.width,\n rect.height)| . Case 2");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromArrayInitializer_useSpaces"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromArrayInitializer_useSpaces"))
                    .SetName("[]|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromArrayInitializer2_useSpaces"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromArrayInitializer2_useSpaces"))
                    .SetName("[rect.width, rect.height]|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromFunctionResult_useSpaces"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromFunctionResult_useSpaces"))
                    .SetName("foo()|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromCallback_useSpaces"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromCallback_useSpaces"))
                    .SetName("from callback");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromClass_useSpaces"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromClass_useSpaces"))
                    .SetName("Class|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromPrivateField"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromPrivateField"))
                    .SetName("from private field");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromNewVar"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromNewVar"))
                    .SetName("new Var()|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromUnsafeCastExpr"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromUnsafeCastExpr"))
                    .SetName("(new type() as String)|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromTrue"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromTrue"))
                    .SetName("true|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromXML"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromXML"))
                    .SetName("<xml/>|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromArrayAccess"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromArrayAccess"))
                    .SetName("array[0]|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVarFromArrayAccess2"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVarFromArrayAccess2"))
                    .SetName("vector[0]|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1704_1"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1704_1"))
                    .SetName("from function():Vector.<Sprite>")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1704");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1704_2"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1704_2"))
                    .SetName("from function():Vector.<flash.display.Sprite>")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1704");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1704_3"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1704_3"))
                    .SetName("from function():Array/*flash.display.Sprite*/")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1704");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1704_4"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1704_4"))
                    .SetName("from function():flash.display.Sprite")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1704");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1749_1"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1749_1"))
                    .SetName("1 + 1|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1749");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1749_2"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1749_2"))
                    .SetName("1 +\n1|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1749");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1749_3"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1749_3"))
                    .SetName("Issue 1749. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1749");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1749_4"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1749_4"))
                    .SetName("Issue 1749. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1749");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1749_5"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1749_5"))
                    .SetName("Issue 1749. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1749");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1749_6"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1749_6"))
                    .SetName("Issue 1749. Modulo")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1749");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_increment"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_increment"))
                    .SetName("++1|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_increment2"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_increment2"))
                    .SetName("1++|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_increment3"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_increment3"))
                    .SetName("++1 * 1++|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_increment4"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_increment4"))
                    .SetName("++1 * ++1|");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_typeof1"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_typeof1"))
                    .SetName("typeof value. Issue 1908.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1908");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_delete"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_delete"))
                    .SetName("delete o[key]. Issue 1908.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1908");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1383_1"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1383_1"))
                    .SetName("new <*>[]|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1383");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1383_2"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1383_2"))
                    .SetName("new <Vector.<*>>[new <*>[]]|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1383");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1383_3"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1383_3"))
                    .SetName("new Vector.<Vector.<*>>()|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1383");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1880_1"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1880_1"))
                    .SetName("/regex/|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1880");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1880_2"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1880_2"))
                    .SetName("[/regex/]|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1880");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1880_3"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1880_3"))
                    .SetName("{v:/regex/}|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1880");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1880_4"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1880_4"))
                    .SetName("(/regex/ as String)|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1880");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1880_5"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1880_5"))
                    .SetName("/regex/gm|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1880");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1765_1"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1765_1"))
                    .SetName("1 << 1|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1765");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1765_2"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1765_2"))
                    .SetName("1 >> 1|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1765");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1765_3"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1765_3"))
                    .SetName("1 ^ 1|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1765");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1765_4"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1765_4"))
                    .SetName("1 >>> 1|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1765");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1765_5"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1765_5"))
                    .SetName("1 | 1|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1765");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1765_6"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1765_6"))
                    .SetName("1 & 1|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1765");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1765_7"),
                        GeneratorJobType.AssignStatementToVar, false)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1765_7"))
                    .SetName("~1|")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1765");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue1764TestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1764_1"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1764_1"))
                    .SetName("1 < 2|. Assign statement to local variable")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1764_2"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1764_2"))
                    .SetName("1 > 2|. Assign statement to local variable")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1764_3"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1764_3"))
                    .SetName("1 && 2|. Assign statement to local variable")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1764_4"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1764_4"))
                    .SetName("1 || 2|. Assign statement to local variable")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1764_5"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1764_5"))
                    .SetName("1 != 2|. Assign statement to local variable")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1764_6"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1764_6"))
                    .SetName("1 == 2|. Assign statement to local variable")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1764_5"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1764_5"))
                    .SetName("1 !== 2|. Assign statement to local variable")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue1764_6"),
                        GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue1764_6"))
                    .SetName("1 === 2|. Assign statement to local variable")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1764");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2151TestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue2151_1"), GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2151_1"))
                    .SetName("_;| Assign statement to var. Issue 2151.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2151");
            }
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue2153TestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue2153_1"), GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2153_1"))
                    .SetName("get3D;| Assign statement to var. Issue 2153.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2153");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue2153_2"), GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2153_2"))
                    .SetName("getThis;| Assign statement to var. Issue 2153.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2153");
                yield return new TestCaseData(ReadAllText("BeforeAssignStatementToVar_issue2153_2"), GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterAssignStatementToVar_issue2153_2"))
                    .SetName("getSuper;| Assign statement to var. Issue 2153.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2153");
            }
        }

        [
            Test,
            TestCaseSource(nameof(AssignStatementToVarTestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue1764TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2151TestCases)),
            TestCaseSource(nameof(AssignStatementToVarIssue2153TestCases)),
        ]
        public string AssignStatementToVar(string sourceText, GeneratorJobType job, bool isUseTabs)
        {
            sci.IsUseTabs = isUseTabs;
            SetSrc(sci, sourceText);
            var list = new MemberList();
            list.Merge(ASContext.GetLanguageContext(sci.ConfigurationLanguage).GetVisibleExternalElements());
            list.Merge(ASContext.Context.CurrentModel.Imports);
            ASContext.Context.GetVisibleExternalElements().Returns(list);
            ASContext.Context.Settings.GenerateImports = true;
            ASGenerator.GenerateJob(job, ASContext.Context.CurrentMember, ASContext.Context.CurrentClass, null, null);
            ASContext.Context.Settings.GenerateImports = false;
            return sci.Text;
        }

        static IEnumerable<TestCaseData> GenerateVariableTestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("BeforeGenerateVariable"), GeneratorJobType.Variable)
                    .Returns(ReadAllText("AfterGeneratePrivateVariable_generateExplicitScopeIsFalse"))
                    .SetName("Generate private variable");
                yield return new TestCaseData(ReadAllText("BeforeGenerateVariable"), GeneratorJobType.VariablePublic)
                    .Returns(ReadAllText("AfterGeneratePublicVariable_generateExplicitScopeIsFalse"))
                    .SetName("Generate public variable");
                yield return new TestCaseData(ReadAllText("BeforeGenerateVariable_forSomeObj"), GeneratorJobType.VariablePublic)
                    .Returns(ReadAllText("AfterGenerateVariable_forSomeObj"))
                    .SetName("From some.foo|");
                yield return new TestCaseData(ReadAllText("BeforeGenerateVariable_forSomeObj2"), GeneratorJobType.VariablePublic)
                    .Returns(ReadAllText("AfterGenerateVariable_forSomeObj2"))
                    .SetName("From new Some().foo|");
                yield return new TestCaseData(ReadAllText("BeforeGenerateVariable_forSomeObj3"), GeneratorJobType.VariablePublic)
                    .Returns(ReadAllText("AfterGenerateVariable_forSomeObj3"))
                    .SetName("From new Some()\n.foo|");
                yield return new TestCaseData(ReadAllText("BeforeGenerateStaticVariable_forCurrentType"), GeneratorJobType.VariablePublic)
                    .Returns(ReadAllText("AfterGeneratePublicStaticVariable_forCurrentType"))
                    .SetName("From CurrentType.foo|");
                yield return new TestCaseData(ReadAllText("BeforeGenerateStaticVariable_forSomeType"), GeneratorJobType.VariablePublic)
                    .Returns(ReadAllText("AfterGeneratePublicStaticVariable_forSomeType"))
                    .SetName("From SomeType.foo|");
                yield return new TestCaseData(ReadAllText("BeforeGenerateVariable_issue1460_1"), GeneratorJobType.Variable)
                    .Returns(ReadAllText("AfterGeneratePrivateVariable_issue1460_1"))
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1460");
                yield return new TestCaseData(ReadAllText("BeforeGenerateVariable_issue1460_2"), GeneratorJobType.Variable)
                    .Returns(ReadAllText("AfterGeneratePrivateVariable_issue1460_2"))
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1460");
                yield return new TestCaseData(ReadAllText("BeforeGenerateConstant"), GeneratorJobType.Constant)
                    .Returns(ReadAllText("AfterGenerateConstant"))
                    .SetName("Generate constant");
                yield return new TestCaseData(ReadAllText("BeforeGenerateConstant_issue1460"), GeneratorJobType.Constant)
                    .Returns(ReadAllText("AfterGenerateConstant_issue1460"))
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1460");
            }
        }

        [Test, TestCaseSource(nameof(GenerateVariableTestCases))]
        public string GenerateVariable(string sourceText, GeneratorJobType job) => GenerateVariableImpl(sourceText, job, sci);

        static string GenerateVariableImpl(string sourceText, GeneratorJobType job, ScintillaControl sci)
        {
            SetSrc(sci, sourceText);
            ASGenerator.GenerateJob(job, ASContext.Context.CurrentMember, ASContext.Context.CurrentClass, null, null);
            return sci.Text;
        }

        static IEnumerable<TestCaseData> GenerateVariableWithExplicitScopeTestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("BeforeGenerateVariable"), GeneratorJobType.Variable)
                    .Returns(ReadAllText("AfterGeneratePrivateVariable_generateExplicitScopeIsTrue"))
                    .SetName("Generate private variable from member scope");
                yield return new TestCaseData(ReadAllText("BeforeGenerateVariable"), GeneratorJobType.VariablePublic)
                    .Returns(ReadAllText("AfterGeneratePublicVariable_generateExplicitScopeIsTrue"))
                    .SetName("Generate public variable from member scope");
                yield return new TestCaseData(ReadAllText("BeforeGenerateVariable_forSomeObj"), GeneratorJobType.VariablePublic)
                    .Returns(ReadAllText("AfterGenerateVariable_forSomeObj"))
                    .SetName("From some.foo|");
                yield return new TestCaseData(ReadAllText("BeforeGenerateVariable_forSomeObj2"), GeneratorJobType.VariablePublic)
                    .Returns(ReadAllText("AfterGenerateVariable_forSomeObj2"))
                    .SetName("From new Some().foo|");
                yield return new TestCaseData(ReadAllText("BeforeGenerateVariable_forSomeObj3"), GeneratorJobType.VariablePublic)
                    .Returns(ReadAllText("AfterGenerateVariable_forSomeObj3"))
                    .SetName("From new Some()\n.foo|");
                yield return new TestCaseData(ReadAllText("BeforeGenerateVariable2"), GeneratorJobType.Variable)
                    .Returns(ReadAllText("AfterGeneratePrivateVariable2_generateExplicitScopeIsTrue"))
                    .SetName("Generate private variable from class scope");
            }
        }

        [Test, TestCaseSource(nameof(GenerateVariableWithExplicitScopeTestCases))]
        public string GenerateVariableWithExplicitScope(string sourceText, GeneratorJobType job)
        {
            ASContext.CommonSettings.GenerateScope = true;
            var result = GenerateVariableImpl(sourceText, job, sci);
            ASContext.CommonSettings.GenerateScope = false;
            return result;
        }

        static IEnumerable<TestCaseData> GenerateVariableWithDefaultModifierDeclarationTestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("BeforeGenerateVariable"), GeneratorJobType.Variable)
                    .Returns(ReadAllText("AfterGeneratePrivateVariable_generateExplicitScopeIsFalse"))
                    .SetName("Generate private variable with default modifier declaration");
                yield return new TestCaseData(ReadAllText("BeforeGenerateStaticVariable_forCurrentType"), GeneratorJobType.Variable)
                    .Returns(ReadAllText("AfterGeneratePrivateStaticVariabeWithDefaultModifier"))
                    .SetName("Generate private static variable with default modifier declaration");
            }
        }

        [Test, TestCaseSource(nameof(GenerateVariableWithDefaultModifierDeclarationTestCases))]
        public string GenerateVariableWithDefaultModifierDeclaration(string sourceText, GeneratorJobType job)
        {
            ASContext.CommonSettings.GenerateDefaultModifierDeclaration = true;
            var result = GenerateVariableImpl(sourceText, job, sci);
            ASContext.CommonSettings.GenerateDefaultModifierDeclaration = false;
            return result;
        }

        static IEnumerable<TestCaseData> GenerateVariableWithProtectedDeclarationTestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("BeforeGenerateVariable"), GeneratorJobType.Variable)
                    .Returns(ReadAllText("AfterGenerateProtectedVariable"))
                    .SetName("Generate private variable with protected modifier declaration");
            }
        }

        [Test, TestCaseSource(nameof(GenerateVariableWithProtectedDeclarationTestCases))]
        public string GenerateVariableWithProtectedDeclaration(string sourceText, GeneratorJobType job)
        {
            ASContext.CommonSettings.GenerateProtectedDeclarations = true;
            var result = GenerateVariableImpl(sourceText, job, sci);
            ASContext.CommonSettings.GenerateProtectedDeclarations = false;
            return result;
        }

        static IEnumerable<TestCaseData> GenerateVariableWithGenerateTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGeneratePublicVariable_issue1734_1", GeneratorJobType.VariablePublic)
                    .Returns(ReadAllText("AfterGeneratePublicVariable_issue1734_1"))
                    .SetName("Issue1734. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1734");
                yield return new TestCaseData("BeforeGeneratePublicVariable_issue1734_2", GeneratorJobType.VariablePublic)
                    .Returns(ReadAllText("AfterGeneratePublicVariable_issue1734_2"))
                    .SetName("Issue1734. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1734");
            }
        }

        [Test, TestCaseSource(nameof(GenerateVariableWithGenerateTestCases))]
        public string GenerateVariableWithGenerate(string fileName, GeneratorJobType job)
        {
            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFileName(GetFullPath(fileName));
            ASContext.Context.Settings.GenerateImports = true;
            ASGenerator.GenerateJob(job, ASContext.Context.CurrentMember, ASContext.Context.CurrentClass, null, null);
            ASContext.Context.Settings.GenerateImports = false;
            return sci.Text;
        }

        static IEnumerable<TestCaseData> GenerateEventHandlerTestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("BeforeGenerateEventHandler"), new string[0])
                    .Returns(ReadAllText("AfterGenerateEventHandler_withoutAutoRemove"))
                    .SetName("Generate event handler without auto remove");
                yield return new TestCaseData(ReadAllText("BeforeGenerateEventHandler"), new[] {"Event.ADDED", "Event.REMOVED"})
                    .Returns(ReadAllText("AfterGenerateEventHandler_withAutoRemove"))
                    .SetName("Generate event handler with auto remove");
                yield return new TestCaseData(ReadAllText("BeforeGenerateEventHandler_issue164_1"), new[] {"Event.ADDED", "Event.REMOVED"})
                    .Returns(ReadAllText("AfterGenerateEventHandler_withAutoRemove_issue164_1"))
                    .SetName("Generate event handler with auto remove. Issue 164. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/164");
                yield return new TestCaseData(ReadAllText("BeforeGenerateEventHandler_issue164_2"), new[] {"Event.ADDED", "Event.REMOVED"})
                    .Returns(ReadAllText("AfterGenerateEventHandler_withAutoRemove_issue164_2"))
                    .SetName("Generate event handler with auto remove. Issue 164. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/164");
            }
        }

        [Test, TestCaseSource(nameof(GenerateEventHandlerTestCases))]
        public string GenerateEventHandler(string sourceText, string[] autoRemove) =>
            GenerateEventHandlerImpl(sourceText, autoRemove, sci);

        static string GenerateEventHandlerImpl(string sourceText, string[] autoRemove, ScintillaControl sci)
        {
            ASContext.CommonSettings.EventListenersAutoRemove = autoRemove;
            SetSrc(sci, sourceText);
            var re = string.Format(ASGenerator.patternEvent, ASGenerator.contextToken);
            var m = Regex.Match(sci.GetLine(sci.CurrentLine), re, RegexOptions.IgnoreCase);
            ASGenerator.contextMatch = m;
            ASGenerator.contextParam = ASGenerator.CheckEventType(m.Groups["event"].Value);
            ASGenerator.GenerateJob(GeneratorJobType.ComplexEvent, ASContext.Context.CurrentMember,
                ASContext.Context.CurrentClass, null, null);
            return sci.Text;
        }

        static IEnumerable<TestCaseData> GenerateEventHandlerWithExplicitScopeTestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("BeforeGenerateEventHandler"),
                        new[] {"Event.ADDED", "Event.REMOVED"})
                    .Returns(ReadAllText("AfterGenerateEventHandler_withAutoRemove_generateExplicitScopeIsTrue"))
                    .SetName("Generate event handler with auto remove");
            }
        }

        [Test, TestCaseSource(nameof(GenerateEventHandlerWithExplicitScopeTestCases))]
        public string GenerateEventHandlerWithExplicitScope(string sourceText, string[] autoRemove)
        {
            ASContext.CommonSettings.GenerateScope = true;
            var result = GenerateEventHandlerImpl(sourceText, autoRemove, sci);
            ASContext.CommonSettings.GenerateScope = false;
            return result;
        }

        static IEnumerable<TestCaseData> GenerateGetterSetterTestCases
        {
            get
            {
                yield return
                    new TestCaseData("BeforeGenerateGetterSetter_fromPublicField", GeneratorJobType.GetterSetter)
                        .Returns(ReadAllText("AfterGenerateGetterSetter_fromPublicField"))
                        .SetName("Generate getter and setter from public field");
                yield return
                    new TestCaseData("BeforeGenerateGetterSetter_fromPublicFieldIfNameStartWith_",
                            GeneratorJobType.GetterSetter)
                        .Returns(ReadAllText("AfterGenerateGetterSetter_fromPublicFieldIfNameStartWith_"))
                        .SetName("Generate getter and setter from public field if name start with \"_\"");
                yield return
                    new TestCaseData("BeforeGenerateGetterSetter_fromPrivateField", GeneratorJobType.GetterSetter)
                        .Returns(ReadAllText("AfterGenerateGetterSetter_fromPrivateField"))
                        .SetName("Generate getter and setter from private field");
                yield return
                    new TestCaseData("BeforeGenerateGetterSetter_issue_1", GeneratorJobType.Setter)
                        .Returns(ReadAllText("AfterGenerateGetterSetter_issue_1"))
                        .SetName("issue set");
                yield return
                    new TestCaseData("BeforeGenerateGetterSetter_issue_2", GeneratorJobType.Getter)
                        .Returns(ReadAllText("AfterGenerateGetterSetter_issue_2"))
                        .SetName("issue get 1");
                yield return
                    new TestCaseData("BeforeGenerateGetterSetter_issue_3", GeneratorJobType.Getter)
                        .Returns(ReadAllText("AfterGenerateGetterSetter_issue_3"))
                        .SetName("issue get 2");
                yield return
                    new TestCaseData("BeforeGenerateGetterSetter_issue_4", GeneratorJobType.Getter)
                        .Returns(ReadAllText("AfterGenerateGetterSetter_issue_4"))
                        .SetName("issue get 3");
                yield return
                    new TestCaseData("BeforeGenerateGetterSetter_issue_5", GeneratorJobType.Getter)
                        .Returns(ReadAllText("AfterGenerateGetterSetter_issue_5"))
                        .SetName("issue get 4");
            }
        }

        [Test, TestCaseSource(nameof(GenerateGetterSetterTestCases))]
        public string GenerateGetterSetter(string fileName, GeneratorJobType job)
        {
            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFileName(GetFullPath(fileName));
            var options = new List<ICompletionListItem>();
            ASGenerator.ContextualGenerator(sci, options);
            var item = options.Find(it => it is GeneratorItem generatorItem && generatorItem.Job == job);
            Assert.NotNull(item);
            var value = item.Value;
            return sci.Text;
        }

        static IEnumerable<TestCaseData> GenerateOverrideTestCases
        {
            get
            {
                yield return new TestCaseData(("BeforeOverridePublicFunction"), "Foo", "foo", FlagType.Function)
                    .Returns(ReadAllText("AfterOverridePublicFunction"))
                    .SetName("override public function");
                yield return new TestCaseData(("BeforeOverrideProtectedFunction"), "Foo", "foo", FlagType.Function)
                    .Returns(ReadAllText("AfterOverrideProtectedFunction"))
                    .SetName("override protected function");
                yield return new TestCaseData(("BeforeOverrideInternalFunction"), "Foo", "foo", FlagType.Function)
                    .Returns(ReadAllText("AfterOverrideInternalFunction"))
                    .SetName("override internal function");
                yield return new TestCaseData(("BeforeOverrideHasOwnProperty"), "Object", "hasOwnProperty",
                        FlagType.Function)
                    .Returns(ReadAllText("AfterOverrideHasOwnProperty"))
                    .SetName("override hasOwnProperty");
                yield return new TestCaseData(("BeforeOverridePublicGetSet"), "Foo", "foo", FlagType.Getter)
                    .Returns(ReadAllText("AfterOverridePublicGetSet"))
                    .SetName("override public getter and setter");
                yield return new TestCaseData(("BeforeOverrideInternalGetSet"), "Foo", "foo", FlagType.Getter)
                    .Returns(ReadAllText("AfterOverrideInternalGetSet"))
                    .SetName("override internal getter and setter");
                yield return new TestCaseData(("BeforeOverrideProtectedFunction_issue1383_1"), "Foo", "foo",
                        FlagType.Function)
                    .Returns(ReadAllText("AfterOverrideProtectedFunction_issue1383_1"))
                    .SetName("override protected function foo(v:Vector.<*>):Vector.<*>");
            }
        }

        [Test, TestCaseSource(nameof(GenerateOverrideTestCases))]
        public string GenerateOverride(string fileName, string ofClassName, string memberName, FlagType memberFlags)
        {
            SetSrc(sci, ReadAllText(fileName));
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
            ASContext.Context.Settings.GenerateImports = true;
            ASGenerator.GenerateOverride(sci, ofClass, member, sci.CurrentPos);
            ASContext.Context.Settings.GenerateImports = false;
            return sci.Text;
        }

        static IEnumerable<TestCaseData> GetStatementReturnTypeTestCases
        {
            get
            {
                yield return new TestCaseData("GetStatementReturnTypeOfStringInitializer")
                    .Returns(new ClassModel {Name = "String", InFile = FileModel.Ignore})
                    .SetName("Get statement return type of \"\"");
                yield return new TestCaseData("GetStatementReturnTypeOfDigit")
                    .Returns(new ClassModel {Name = "int", InFile = FileModel.Ignore})
                    .SetName("Get statement return type of 1");
                yield return new TestCaseData("GetStatementReturnTypeOfDigit_2")
                    .Returns(new ClassModel {Name = "Number", InFile = FileModel.Ignore})
                    .SetName("Get statement return type of 1.0");
                yield return new TestCaseData("GetStatementReturnTypeOfObjectInitializer")
                    .Returns(new ClassModel {Name = "Object", InFile = FileModel.Ignore})
                    .SetName("Get statement return type of {}");
                yield return new TestCaseData("GetStatementReturnTypeOfNewArray")
                    .Returns(new ClassModel {Name = "Array", InFile = FileModel.Ignore})
                    .SetName("Get statement return type of new Array()");
                yield return new TestCaseData("GetStatementReturnTypeOfArrayInitializer")
                    .Returns(new ClassModel {Name = "Array", InFile = FileModel.Ignore})
                    .SetName("Get statement return type of []");
                yield return new TestCaseData("GetStatementReturnTypeOfNewVector")
                    .Returns(new ClassModel {Name = "Vector.<int>", InFile = FileModel.Ignore})
                    .SetName("Get statement return type of new Vector.<int>()");
                yield return new TestCaseData("GetStatementReturnTypeOfVectorInitializer")
                    .Returns(new ClassModel {Name = "Vector.<int>", InFile = FileModel.Ignore})
                    .SetName("Get statement return type of new <int>[]");
                yield return new TestCaseData("GetStatementReturnTypeOfTwoDimensionalVectorInitializer")
                    .Returns(new ClassModel {Name = "Vector.<Vector.<int>>", InFile = FileModel.Ignore})
                    .SetName("Get statement return type of new new <Vector.<int>>[new <int>[0]]");
                yield return new TestCaseData("GetStatementReturnTypeOfItemOfVector")
                    .Returns(new ClassModel {Name = "int", InFile = FileModel.Ignore})
                    .SetName("Get statement return type of vector[0]");
                yield return new TestCaseData("GetStatementReturnTypeOfItemOfTwoDimensionalVector")
                    .Returns(new ClassModel {Name = "int", InFile = FileModel.Ignore})
                    .SetName("Get statement return type of vector[0][0]");
                yield return new TestCaseData("GetStatementReturnTypeOfItemOfMultidimensionalVector")
                    .Returns(new ClassModel {Name = "int", InFile = FileModel.Ignore})
                    .SetName("Get statement return type of vector[0][0][0][0]");
                yield return new TestCaseData("GetStatementReturnTypeOfArrayAccess")
                    .Returns(new ClassModel {Name = "int", InFile = FileModel.Ignore})
                    .SetName("Get statement return type of v[0][0].length");
                yield return new TestCaseData("GetStatementReturnTypeOfNewObject")
                    .Returns(new ClassModel {Name = "Object", InFile = FileModel.Ignore})
                    .SetName("Get statement return type of new Object");
            }
        }

        [Test, TestCaseSource(nameof(GetStatementReturnTypeTestCases))]
        public ClassModel GetStatementReturnType(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            var currentLine = sci.CurrentLine;
            var returnType = ASGenerator.GetStatementReturnType(sci, ASContext.Context.CurrentClass, sci.GetLine(currentLine), sci.PositionFromLine(currentLine));
            var result = returnType.Resolve.Type;
            return result;
        }

        static IEnumerable<TestCaseData> ParseFunctionParametersTestCases
        {
            get
            {
                yield return new TestCaseData("ParseFunctionParameters_String")
                    .Returns(new List<MemberModel> {new ClassModel {Name = "String", InFile = FileModel.Ignore}})
                    .SetName("Parse function parameters of foo(\"string\")");
                yield return new TestCaseData("ParseFunctionParameters_Boolean")
                    .Returns(new List<MemberModel> {new ClassModel {Name = "Boolean", InFile = FileModel.Ignore}})
                    .SetName("Parse function parameters of foo(true)");
                yield return new TestCaseData("ParseFunctionParameters_Boolean_false")
                    .Returns(new List<MemberModel> {new ClassModel {Name = "Boolean", InFile = FileModel.Ignore}})
                    .SetName("Parse function parameters of foo(false)");
                yield return new TestCaseData("ParseFunctionParameters_Digit")
                    .Returns(new List<MemberModel> {new ClassModel {Name = "int", InFile = FileModel.Ignore}})
                    .SetName("Parse function parameters of foo(1)");
                yield return new TestCaseData("ParseFunctionParameters_Digit_2")
                    .Returns(new List<MemberModel> {new ClassModel {Name = "Number", InFile = FileModel.Ignore}})
                    .SetName("Parse function parameters of foo(1.0)");
                yield return new TestCaseData("ParseFunctionParameters_Array")
                    .Returns(new List<MemberModel> {new ClassModel {Name = "Array", InFile = FileModel.Ignore}})
                    .SetName("Parse function parameters of foo(new Array())");
                yield return new TestCaseData("ParseFunctionParameters_ArrayInitializer")
                    .Returns(new List<MemberModel> {new ClassModel {Name = "Array", InFile = FileModel.Ignore}})
                    .SetName("Parse function parameters of foo([])");
                yield return new TestCaseData("ParseFunctionParameters_Object")
                    .Returns(new List<MemberModel> {new ClassModel {Name = "Object", InFile = FileModel.Ignore}})
                    .SetName("Parse function parameters of foo(new Object())");
                yield return new TestCaseData("ParseFunctionParameters_ObjectInitializer")
                    .Returns(new List<MemberModel> {new ClassModel {Name = "Object", InFile = FileModel.Ignore}})
                    .SetName("Parse function parameters of foo({})");
                yield return new TestCaseData("ParseFunctionParameters_Vector")
                    .Returns(new List<MemberModel> {new ClassModel {Name = "Vector.<int>", InFile = FileModel.Ignore}})
                    .SetName("Parse function parameters of foo(new Vector.<int>())");
                yield return new TestCaseData("ParseFunctionParameters_VectorInitializer")
                    .Returns(new List<MemberModel> {new ClassModel {Name = "Vector.<int>", InFile = FileModel.Ignore}})
                    .SetName("Parse function parameters of foo(new <int>[])");
                yield return new TestCaseData("ParseFunctionParameters_TwoDimensionalVectorInitializer")
                    .Returns(new List<MemberModel> {new ClassModel {Name = "Vector.<Vector.<int>>", InFile = FileModel.Ignore}})
                    .SetName("Parse function parameters of foo(new <Vector.<int>>[new <int>[]])");
                yield return new TestCaseData("ParseFunctionParameters_MultidimensionalVectorInitializer")
                    .Returns(new List<MemberModel> {new ClassModel {Name = "Vector.<Vector.<Vector.<int>>>", InFile = FileModel.Ignore}})
                    .SetName("Parse function parameters of foo(new <Vector.<Vector.<int>>>[new <Vector.<int>>[new <int>[]]])");
                yield return new TestCaseData("ParseFunctionParameters_ArrayAccess")
                    .Returns(new List<MemberModel> {new ClassModel {Name = "int", InFile = FileModel.Ignore}})
                    .SetName("Parse function parameters of foo(v[0][0].length)")
                    .Ignore("");
                yield return new TestCaseData("ParseFunctionParameters_uint")
                    .Returns(new List<MemberModel> {new ClassModel {Name = "uint", InFile = FileModel.Ignore}})
                    .SetName("Parse function parameters of foo(0xFF0000)");
                yield return new TestCaseData("ParseFunctionParameters_Sprite")
                    .Returns(new List<MemberModel> {new ClassModel {Name = "Sprite", InFile = FileModel.Ignore}})
                    .SetName("Parse function parameters of foo(new Sprite())");
                yield return new TestCaseData("ParseFunctionParameters_Sprite2")
                    .Returns(new List<MemberModel>
                    {
                        new ClassModel {Name = "Sprite", InFile = FileModel.Ignore},
                        new ClassModel {Name = "Sprite", InFile = FileModel.Ignore}
                    })
                    .SetName("Parse function parameters of foo(new Sprite(), new Sprite())");
                yield return new TestCaseData("ParseFunctionParameters_Sprite2_withComments")
                    .Returns(new List<MemberModel>
                    {
                        new ClassModel {Name = "Sprite", InFile = FileModel.Ignore},
                        new ClassModel {Name = "Sprite", InFile = FileModel.Ignore}
                    })
                    .SetName("Parse function parameters of foo(/*new MovieClip()*/new Sprite(), /*new MovieClip()*/new Sprite())");
                yield return new TestCaseData("ParseFunctionParameters_Sprite2_withComments2")
                    .Returns(new List<MemberModel>
                    {
                        new ClassModel {Name = "Sprite", InFile = FileModel.Ignore},
                        new ClassModel {Name = "Sprite", InFile = FileModel.Ignore}
                    })
                    .SetName("Parse function parameters of foo(/*)*/new Sprite(), /*(((((*/new Sprite())");
                yield return new TestCaseData("ParseFunctionParameters_XML")
                    .Returns(new List<MemberModel> {new ClassModel {Name = "XML", InFile = FileModel.Ignore}})
                    .SetName("Parse function parameters of foo(<xml param = '10' />)");
            }
        }

        [Test, TestCaseSource(nameof(ParseFunctionParametersTestCases))]
        public List<MemberModel> ParseFunctionParameters(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            var list = new MemberList();
            list.Merge(ASContext.GetLanguageContext(sci.ConfigurationLanguage).GetVisibleExternalElements());
            list.Merge(ASContext.Context.CurrentModel.Imports);
            ASContext.Context.GetVisibleExternalElements().Returns(list);
            var result = ASGenerator.ParseFunctionParameters(sci, sci.CurrentPos) .Select(it => it.result.Type ?? it.result.Member).ToList();
            return result;
        }

        static IEnumerable<TestCaseData> ChangeConstructorDeclarationTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_String")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_String"))
                    .SetName("new Foo(\"\") -> function Foo(string:String)");
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_String2")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_String2"))
                    .SetName("new Foo(\"\", \"\") -> function Foo(string:String, string1:String)");
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_Digit")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_Digit"))
                    .SetName("new Foo(1) -> function Foo(intValue:int)");
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_Digit_2")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_Digit_2"))
                    .SetName("new Foo(1.0) -> function Foo(number:Number)");
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_Boolean")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_Boolean"))
                    .SetName("new Foo(true) -> function Foo(boolean:Boolean)");
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_ObjectInitializer")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_ObjectInitializer"))
                    .SetName("new Foo({}) -> function Foo(object:Object)");
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_ArrayInitializer")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_ArrayInitializer"))
                    .SetName("new Foo([]) -> function Foo(array:Array)");
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_VectorInitializer")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_VectorInitializer"))
                    .SetName("new Foo(new <int>[]) -> function Foo(vector:Vector.<int>)");
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_TwoDimensionalVectorInitializer")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_TwoDimensionalVectorInitializer"))
                    .SetName(
                        "new Foo(new <Vector.<Vector.<int>>[new <int>[]]) -> function Foo(vector:Vector.<Vector.<int>>)");
                yield return new TestCaseData(
                        "BeforeChangeConstructorDeclaration_ItemOfTwoDimensionalVectorInitializer")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_ItemOfTwoDimensionalVectorInitializer"))
                    .SetName("new Foo(strings[0][0]) -> function Foo(string:String)");
                yield return new TestCaseData("BeforeChangeConstructorDeclaration_Function")
                    .Returns(ReadAllText("AfterChangeConstructorDeclaration_Function"))
                    .SetName("new Foo(function():void {}) -> function Foo(functionValue:Function)");
            }
        }

        [Test, TestCaseSource(nameof(ChangeConstructorDeclarationTestCases))]
        public string ChangeConstructorDeclaration(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFileName(GetFullPath(fileName));
            ASContext.Context.Settings.GenerateImports = true;
            ASGenerator.GenerateJob(GeneratorJobType.ChangeConstructorDecl, ASContext.Context.CurrentMember,
                ASContext.Context.CurrentClass, null, null);
            ASContext.Context.Settings.GenerateImports = false;
            return sci.Text;
        }

        static IEnumerable<TestCaseData> GetStartOfStatementTestCases
        {
            get
            {
                yield return new TestCaseData(" new Vector.<int>()$(EntryPoint)",
                        new ASResult
                        {
                            Type = new ClassModel {Flags = FlagType.Class},
                            Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}
                        })
                    .Returns(1);
                yield return new TestCaseData(" new <int>[]$(EntryPoint)",
                        new ASResult
                        {
                            Type = new ClassModel {Flags = FlagType.Class},
                            Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}
                        })
                    .Returns(1);
                yield return new TestCaseData(" new <Object>[{}]$(EntryPoint)",
                        new ASResult
                        {
                            Type = new ClassModel {Flags = FlagType.Class},
                            Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}
                        })
                    .Returns(1);
                yield return new TestCaseData(" new <Vector.<Object>>[new <Object>[{}]]$(EntryPoint)",
                        new ASResult
                        {
                            Type = new ClassModel {Flags = FlagType.Class},
                            Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}
                        })
                    .Returns(1);
                yield return new TestCaseData(" new <Object>[{a:[new Number('10.0')]}]$(EntryPoint)",
                        new ASResult
                        {
                            Type = new ClassModel {Flags = FlagType.Class},
                            Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}
                        })
                    .Returns(1);
                yield return new TestCaseData(" new Object()$(EntryPoint)",
                        new ASResult
                        {
                            Type = new ClassModel {Flags = FlagType.Class},
                            Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}
                        })
                    .Returns(1);
                yield return new TestCaseData(" new Object(/*:)*/)$(EntryPoint)",
                        new ASResult
                        {
                            Type = new ClassModel {Flags = FlagType.Class},
                            Context = new ASExpr {WordBefore = "new", WordBeforePosition = 1}
                        })
                    .Returns(1);
            }
        }

        [Test, TestCaseSource(nameof(GetStartOfStatementTestCases))]
        public int GetStartOfStatement(string sourceText, ASResult expr)
        {
            SetSrc(sci, sourceText);
            return ASGenerator.GetStartOfStatement(expr);
        }

        static IEnumerable<TestCaseData> AvoidKeywordTestCases
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
                yield return new TestCaseData("int").Returns("intValue");
                yield return new TestCaseData("uint").Returns("uintValue");
            }
        }

        [Test, TestCaseSource(nameof(AvoidKeywordTestCases))]
        public string AvoidKeyword(string sourceText) => ASGenerator.AvoidKeyword(sourceText);

        static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateInterface_issue2481_1", true)
                    .SetName("implements IFoo<generator>. Generate interface. case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2481");
                yield return new TestCaseData("BeforeGenerateInterface_issue2481_2", false)
                    .SetName("extends IFoo<generator>, Generate interface. case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2481");
                yield return new TestCaseData("BeforeGenerateInterface_issue2481_3", true)
                    .SetName("foo(v:IFoo<generator>), Generate interface. case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2481");
            }
        }

        [Test, TestCaseSource(nameof(TestCases))]
        public void Common(string fileName, bool hasGenerator)
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
                            //var de = (DataEvent) e;
                            //var info = (Hashtable) de.Data;
                            //var actualArgs = (string) info[nameof(constructorArgs)];
                            //Assert.AreEqual(constructorArgs, actualArgs);
                            break;
                    }
                });
            EventManager.AddEventHandler(handler, EventType.Command);
            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFileName(GetFullPath(fileName));
            var options = new List<ICompletionListItem>();
            ASGenerator.ContextualGenerator(sci, options);
            var item = options.Find(it => ((GeneratorItem) it).Job == GeneratorJobType.Interface);
            if (hasGenerator)
            {
                Assert.IsNotNull(item);
                var value = item.Value;
                return;
            }

            Assert.IsNull(item);
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue1756TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGenerator_issue1756_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGenerator_issue1756_1"))
                    .SetName("true ? 1 : 2;<generator> Issue 1756. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1756");
            }
        }

        static IEnumerable<TestCaseData> ContextualGeneratorTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateFunction_1", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_1"))
                    .SetName("Generate function. case 1");
                yield return new TestCaseData("BeforeGenerateFunction_2", GeneratorJobType.Function, true)
                    .Ignore("")
                    .Returns(ReadAllText("AfterGenerateFunction_2"))
                    .SetName("Generate function. case 2");
                yield return new TestCaseData("BeforeImplementInterfaceMethods", GeneratorJobType.ImplementInterface, true)
                    .Returns(ReadAllText("AfterImplementInterfaceMethods"))
                    .SetName("Implement interface methods. Issue 1684")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1684");
                yield return new TestCaseData("BeforeGenerateVariable_1", GeneratorJobType.VariablePublic, true)
                    .Returns(ReadAllText("AfterGenerateVariable_1"))
                    .SetName("Generate variable. case 1");
                yield return new TestCaseData("BeforeAddToInterface_issue2257_1", GeneratorJobType.AddInterfaceDef, true)
                    .Returns(ReadAllText("AfterAddToInterface_issue2257_1"))
                    .SetName("Add to interface. case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2257");
                yield return new TestCaseData("BeforeAddToInterface_issue2257_2", GeneratorJobType.AddInterfaceDef, true)
                    .Returns(ReadAllText("AfterAddToInterface_issue2257_2"))
                    .SetName("Add to interface. case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2257");
                yield return new TestCaseData("BeforeAddToInterface_issue2257_3", GeneratorJobType.AddInterfaceDef, true)
                    .Returns(ReadAllText("AfterAddToInterface_issue2257_3"))
                    .SetName("Add to interface. case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2257");
                yield return new TestCaseData("BeforeAddToInterface_issue2257_4", GeneratorJobType.AddInterfaceDef, true)
                    .Returns(ReadAllText("AfterAddToInterface_issue2257_4"))
                    .SetName("Add to interface. case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2257");
                yield return new TestCaseData("BeforeGenerateFunction_issue394_1", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue394_1"))
                    .SetName("Generate function. Issue 394. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/394");
                yield return new TestCaseData("BeforeGenerateFunction_issue394_2", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue394_2"))
                    .SetName("Generate function. Issue 394. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/394");
                yield return new TestCaseData("BeforeGenerateFunction_issue394_3", GeneratorJobType.FieldFromParameter, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue394_3"))
                    .SetName("Generate function. Issue 394. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/394");
                yield return new TestCaseData("BeforeGenerateFunction_issue394_4", GeneratorJobType.FieldFromParameter, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue394_4"))
                    .SetName("Generate function. Issue 394. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/394");
                yield return new TestCaseData("BeforeGenerateFunction_issue394_5", GeneratorJobType.PromoteLocal, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue394_5"))
                    .SetName("Generate function. Issue 394. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/394");
                yield return new TestCaseData("BeforeGenerateFunction_issue394_6", GeneratorJobType.FieldFromParameter, true)
                    .Returns(ReadAllText("AfterGenerateFunction_issue394_6"))
                    .SetName("Generate function. Issue 394. Case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/394");
                yield return new TestCaseData("BeforeFieldFromParameter_varargs", GeneratorJobType.FieldFromParameter, true)
                    .Returns(ReadAllText("AfterFieldFromParameter_varargs"))
                    .SetName("foo(...args). Field from parameter. Case 1");
                yield return new TestCaseData("BeforeFieldFromParameter_varargs_2", GeneratorJobType.FieldFromParameter, true)
                    .Returns(ReadAllText("AfterFieldFromParameter_varargs_2"))
                    .SetName("foo(v1:(, ...args). Field from parameter. Case 2");
                yield return new TestCaseData("BeforeDeclareVariable_issue2271_1", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterDeclareVariable_issue2271_1"))
                    .SetName("this.|v = v:Function/*(v1:*):void*/. Declare variable. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2271");
                yield return new TestCaseData("BeforeDeclareVariable_issue2271_1", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterDeclareVariable_issue2271_1"))
                    .SetName("this.|v = v:Function/*(v1:*):void*/. Declare variable. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2271");
                yield return new TestCaseData("BeforeDeclareVariable_issue2271_2", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterDeclareVariable_issue2271_2"))
                    .SetName("this.|v = v:Function/*(v1:*):int*/. Declare variable. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2271");
                yield return new TestCaseData("BeforeDeclareVariable_issue2273_1", GeneratorJobType.GetterSetter, true)
                    .Returns(ReadAllText("AfterDeclareVariable_issue2273_1"))
                    .SetName("private var |v:Function/*(v1:*):void*/. Generate Getter and Setter. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2273");
                yield return new TestCaseData("BeforeDeclareVariable_issue2273_2", GeneratorJobType.GetterSetter, true)
                    .Returns(ReadAllText("AfterDeclareVariable_issue2273_2"))
                    .SetName("private var |v:Function/*(v1:*):int*/. Generate Getter and Setter. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2273");
                yield return new TestCaseData("BeforeAddToInterface_issue2275_1", GeneratorJobType.AddInterfaceDef, true)
                    .Returns(ReadAllText("AfterAddToInterface_issue2275_1"))
                    .SetName("Add to interface. Issue 2275. case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2275");
                yield return new TestCaseData("BeforeImplementInterfaceMethods_issue2278_1", GeneratorJobType.ImplementInterface, true)
                    .Returns(ReadAllText("AfterImplementInterfaceMethods_issue2278_1"))
                    .SetName("Implement interface methods. Issue 2278. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2278");
                yield return new TestCaseData("BeforeImplementInterfaceMethods_issue2278_2", GeneratorJobType.ImplementInterface, true)
                    .Returns(ReadAllText("AfterImplementInterfaceMethods_issue2278_2"))
                    .SetName("Implement interface methods. Issue 2278. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2278");
                yield return new TestCaseData("BeforeImplementInterfaceMethods_issue2278_3", GeneratorJobType.ImplementInterface, true)
                    .Returns(ReadAllText("AfterImplementInterfaceMethods_issue2278_3"))
                    .SetName("Implement interface methods. Issue 2278. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2278");
                yield return new TestCaseData("BeforeImplementInterfaceMethods_issue2280_1", GeneratorJobType.AddAsParameter, true)
                    .Returns(ReadAllText("AfterImplementInterfaceMethods_issue2280_1"))
                    .SetName("Implement interface methods. Issue 2280. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2280");
                yield return new TestCaseData("BeforeImplementInterfaceMethods_issue2280_2", GeneratorJobType.AddAsParameter, true)
                    .Returns(ReadAllText("AfterImplementInterfaceMethods_issue2280_2"))
                    .SetName("Implement interface methods. Issue 2280. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2280");
                yield return new TestCaseData("BeforeContextualGenerator_issue2628_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGenerator_issue2628_1"))
                    .SetName("!true;<generator> Issue 2628. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2628");
            }
        }

        static IEnumerable<TestCaseData> Issue2297TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGenerator_issue2297_1", GeneratorJobType.FunctionPublic, false)
                    .Returns(null)
                    .SetName("Interface.fo|o(). Issue 2297. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2297");
                yield return new TestCaseData("BeforeContextualGenerator_issue2297_1", GeneratorJobType.VariablePublic, false)
                    .Returns(null)
                    .SetName("Interface.fo|o(). Issue 2297. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2297");
                yield return new TestCaseData("BeforeContextualGenerator_issue2297_2", GeneratorJobType.FunctionPublic, false)
                    .Returns(null)
                    .SetName("Interface.fo|o. Issue 2297. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2297");
                yield return new TestCaseData("BeforeContextualGenerator_issue2297_2", GeneratorJobType.VariablePublic, false)
                    .Returns(null)
                    .SetName("Interface.fo|o. Issue 2297. Case 2.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2297");
            }
        }

        static IEnumerable<TestCaseData> Issue2346TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeContextualGenerator_issue2346_1", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(ReadAllText("AfterContextualGenerator_issue2346_1"))
                    .SetName("Issue 2346. Case 1. Assign statement to var")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2346");
                yield return new TestCaseData("BeforeContextualGenerator_issue2346_2", GeneratorJobType.Function, true)
                    .Returns(ReadAllText("AfterContextualGenerator_issue2346_2"))
                    .SetName("Issue 2346. Case 2. Generate function")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2346");
                yield return new TestCaseData("BeforeContextualGenerator_issue2346_3", GeneratorJobType.FieldFromParameter, true)
                    .Returns(ReadAllText("AfterContextualGenerator_issue2346_3"))
                    .SetName("Issue 2346. Case 3. Generate field from parameter")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2346");
                yield return new TestCaseData("BeforeContextualGenerator_issue2346_4", GeneratorJobType.PromoteLocal, true)
                    .Returns(ReadAllText("AfterContextualGenerator_issue2346_4"))
                    .SetName("Issue 2346. Case 4. Promote local")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2346");
                yield return new TestCaseData("BeforeContextualGenerator_issue2346_5", GeneratorJobType.ChangeMethodDecl, true)
                    .Returns(ReadAllText("AfterContextualGenerator_issue2346_5"))
                    .SetName("Issue 2346. Case 5. Change method declaration")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2346");
                yield return new TestCaseData("BeforeContextualGenerator_issue2346_6", GeneratorJobType.Variable, true)
                    .Returns(ReadAllText("AfterContextualGenerator_issue2346_6"))
                    .SetName("Issue 2346. Case 6. Generate variable")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2346");
                yield return new TestCaseData("BeforeContextualGenerator_issue2346_7", GeneratorJobType.AddAsParameter, true)
                    .Returns(ReadAllText("AfterContextualGenerator_issue2346_7"))
                    .SetName("Issue 2346. Case 7. Add as parameter")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2346");
            }
        }

        static IEnumerable<TestCaseData> GenerateEventHandlerIssue2421TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateEventHandler_issue2421_1", GeneratorJobType.ComplexEvent, true)
                    .Returns(ReadAllText("AfterGenerateEventHandler_issue2421_1"))
                    .SetName("Generate event handler. Issue 2421. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2421");
            }
        }

        static IEnumerable<TestCaseData> ConvertToConstIssue2406TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeConvertToConst_issue2406_1", GeneratorJobType.ConvertToConst, true)
                    .Returns(ReadAllText("AfterConvertToConst_issue2406_1"))
                    .SetName("Convert to const. Issue 2406. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2406");
                yield return new TestCaseData("BeforeConvertToConst_issue2406_2", GeneratorJobType.ConvertToConst, true)
                    .Returns(ReadAllText("AfterConvertToConst_issue2406_2"))
                    .SetName("Convert to const. Issue 2406. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2406");
                yield return new TestCaseData("BeforeConvertToConst_issue2406_3", GeneratorJobType.ConvertToConst, true)
                    .Returns(ReadAllText("AfterConvertToConst_issue2406_3"))
                    .SetName("Convert to const. Issue 2406. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2406");
                yield return new TestCaseData("BeforeConvertToConst_issue2406_4", GeneratorJobType.ConvertToConst, true)
                    .Returns(ReadAllText("AfterConvertToConst_issue2406_4"))
                    .SetName("Convert to const. Issue 2406. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2406");
                yield return new TestCaseData("BeforeConvertToConst_issue2406_5", GeneratorJobType.ConvertToConst, true)
                    .Returns(ReadAllText("AfterConvertToConst_issue2406_5"))
                    .SetName("Convert to const. Issue 2406. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2406");
                yield return new TestCaseData("BeforeConvertToConst_issue2406_6", GeneratorJobType.ConvertToConst, true)
                    .Returns(ReadAllText("AfterConvertToConst_issue2406_6"))
                    .SetName("Convert to const. Issue 2406. Case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2406");
                yield return new TestCaseData("BeforeConvertToConst_issue2406_7", GeneratorJobType.ConvertToConst, true)
                    .Returns(ReadAllText("AfterConvertToConst_issue2406_7"))
                    .SetName("Convert to const. Issue 2406. Case 7")
                    .Ignore("")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2406");
            }
        }

        static IEnumerable<TestCaseData> InterfaceContextualGeneratorTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeInterfaceContextualGeneratorGetterSetter_issue2473_1", GeneratorJobType.GetterSetter, true)
                    .Returns(ReadAllText("AfterInterfaceContextualGeneratorGetterSetter_issue2473_1"))
                    .SetName("Generate Getter and Setter. Issue 2473. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeInterfaceContextualGeneratorGetter_issue2473_1", GeneratorJobType.Getter, true)
                    .Returns(ReadAllText("AfterInterfaceContextualGeneratorGetter_issue2473_1"))
                    .SetName("Generate Getter. Issue 2473. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeInterfaceContextualGeneratorSetter_issue2473_1", GeneratorJobType.Setter, true)
                    .Returns(ReadAllText("AfterInterfaceContextualGeneratorSetter_issue2473_1"))
                    .SetName("Generate Setter. Issue 2473. Case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeInterfaceContextualGeneratorFunction_issue2473_1", GeneratorJobType.FunctionPublic, true)
                    .Returns(ReadAllText("AfterInterfaceContextualGeneratorFunction_issue2473_1"))
                    .SetName("Generate Function. Issue 2473. Case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeInterfaceContextualGeneratorAutoImport_issue2473_1", -1, false)
                    .Returns(null)
                    .SetName("Auto import. Issue 2473. Case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeInterfaceContextualGenerator_issue2473_6", -1, false)
                    .Returns(null)
                    .SetName("Issue 2473. Case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeInterfaceContextualGenerator_issue2473_7", -1, false)
                    .Returns(null)
                    .SetName("Issue 2473. Case 7")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeInterfaceContextualGenerator_issue2473_8", -1, false)
                    .Returns(null)
                    .SetName("Issue 2473. Case 8")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeInterfaceContextualGenerator_issue2473_9", GeneratorJobType.FunctionPublic, false)
                    .Returns(null)
                    .SetName("Generate new class. Issue 2473. Case 9")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeInterfaceContextualGenerator_issue2473_10", GeneratorJobType.Class, false)
                    .Returns(null)
                    .SetName("interface A extends B<generate>. Issue 2473. Case 10")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
                yield return new TestCaseData("BeforeInterfaceContextualGenerator_issue2473_11", GeneratorJobType.Interface, true)
                    .Returns(ReadAllText("AfterInterfaceContextualGenerator_issue2473_11"))
                    .SetName("interface A extends B<generate>. Issue 2473. Case 11")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2473");
            }
        }

        [
            Test,
            TestCaseSource(nameof(AssignStatementToVarIssue1756TestCases)),
            TestCaseSource(nameof(ContextualGeneratorTestCases)),
            TestCaseSource(nameof(Issue2297TestCases)),
            TestCaseSource(nameof(Issue2346TestCases)),
            TestCaseSource(nameof(GenerateEventHandlerIssue2421TestCases)),
            TestCaseSource(nameof(ConvertToConstIssue2406TestCases)),
            TestCaseSource(nameof(InterfaceContextualGeneratorTestCases)),
        ]
        public string ContextualGenerator(string fileName, GeneratorJobType job, bool hasGenerator)
        {
            SetSrc(sci, ReadAllText(fileName));
            SetCurrentFileName(GetFullPath(fileName));
            var handler = Substitute.For<IEventHandler>();
            handler
                .When(it => it.HandleEvent(Arg.Any<object>(), Arg.Any<NotifyEvent>(), Arg.Any<HandlingPriority>()))
                .Do(it =>
                {
                    var e = it.ArgAt<NotifyEvent>(1);
                    switch (e.Type)
                    {
                        case EventType.Command:
                            var de = (DataEvent) e;
                            if (de.Action == "ProjectManager.LineEntryDialog")
                            {
                                EventManager.RemoveEventHandler(handler);
                                e.Handled = true;
                            }
                            else if (de.Action == "ProjectManager.CreateNewFile")
                            {
                                EventManager.RemoveEventHandler(handler);
                                e.Handled = true;
                                var info = (Hashtable) de.Data;
                                Assert.IsNotNullOrEmpty((string) info["interfaceName"]);
                            }

                            break;
                    }
                });
            EventManager.AddEventHandler(handler, EventType.Command);
            var options = new List<ICompletionListItem>();
            ASGenerator.ContextualGenerator(sci, options);
            if (hasGenerator)
            {
                Assert.IsNotEmpty(options);
                var item = options.Find(it => ((GeneratorItem) it).Job == job);
                Assert.IsNotNull(item);
                ASContext.Context.ResolveImports(null).ReturnsForAnyArgs(it =>
                {
                    var ctx = (ASContext) ASContext.GetLanguageContext(sci.ConfigurationLanguage);
                    ctx.completionCache.IsDirty = true;
                    ctx.completionCache.Imports = null;
                    return ctx.ResolveImports(it.ArgAt<FileModel>(0));
                });
                ASContext.Context.Settings.GenerateImports = true;
                var value = item.Value;
                ASContext.Context.Settings.GenerateImports = false;
                EventManager.RemoveEventHandler(handler);
                return sci.Text;
            }

            EventManager.RemoveEventHandler(handler);
            if (job == (GeneratorJobType) (-1)) Assert.IsEmpty(options);
            if (options.Count > 0) Assert.IsFalse(options.Any(it => ((GeneratorItem) it).Job == job));
            return null;
        }
    }
}