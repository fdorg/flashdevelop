using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ASCompletion.Completion;
using ASCompletion.Context;
using CodeRefactor.Provider;
using CodeRefactor.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using ScintillaNet;

namespace CodeRefactor.Commands
{
    [TestFixture]
    public class RefactorCommandTests : ASCompleteTests
    {
        [TestFixture]
        public class ExtractLocalVariable : RefactorCommandTests
        {
            internal static string ReadAllTextAS3(string fileName) => TestFile.ReadAllText($"{nameof(CodeRefactor)}.Test_Files.coderefactor.extractlocalvariable.as3.{fileName}.as");

            internal static string ReadAllTextHaxe(string fileName) => TestFile.ReadAllText($"{nameof(CodeRefactor)}.Test_Files.coderefactor.extractlocalvariable.haxe.{fileName}.hx");

            static IEnumerable<TestCaseData> HaxeTestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllTextHaxe("BeforeExtractLocalVariable_fromGeneric"), "newVar")
                        .Returns(ReadAllTextHaxe("AfterExtractLocalVariable_fromGeneric"))
                        .SetName("ExtractLocaleVariable from Generic");
                    yield return new TestCaseData(ReadAllTextHaxe("BeforeExtractLocalVariable_fromGeneric_2"), "newVar")
                        .Returns(ReadAllTextHaxe("AfterExtractLocalVariable_fromGeneric_2"))
                        .SetName("new Test<String>('test') -> var newVar:Test<String> = new Test<String>('test')");
                    yield return new TestCaseData(ReadAllTextHaxe("BeforeExtractLocalVariable_fromString"), "newVar")
                        .Returns(ReadAllTextHaxe("AfterExtractLocalVariable_fromString"))
                        .SetName("ExtractLocaleVariable from String");
                    yield return new TestCaseData(ReadAllTextHaxe("BeforeExtractLocalVariable_fromNumber"), "newVar" )
                        .Returns(ReadAllTextHaxe("AfterExtractLocalVariable_fromNumber"))
                        .SetName("ExtractLocaleVariable from Int");
                    yield return new TestCaseData(ReadAllTextHaxe("BeforeExtractLocalVariable_fromNumber_2"), "newVar" )
                        .Returns(ReadAllTextHaxe("AfterExtractLocalVariable_fromNumber_2"))
                        .SetName("ExtractLocaleVariable from Float");
                    yield return new TestCaseData(ReadAllTextHaxe("BeforeExtractLocalVariable_inSinglelineMethod"), "newVar")
                        .Ignore("Not supported at the moment")
                        .Returns(ReadAllTextHaxe("AfterExtractLocalVariable_inSinglelineMethod"))
                        .SetName("ExtractLocaleVariable in single line method");
                    yield return new TestCaseData(ReadAllTextHaxe("BeforeExtractLocalVariable_arrayInitializer_1"), "newVar")
                        .Returns(ReadAllTextHaxe("AfterExtractLocalVariable_arrayInitializer_1"))
                        .SetName("[1,2,3] -> var newArray:Array<T> = [1,2,3]");
                    yield return new TestCaseData(ReadAllTextHaxe("BeforeExtractLocalVariable_arrayInitializer_2"), "newVar")
                        .Returns(ReadAllTextHaxe("AfterExtractLocalVariable_arrayInitializer_2"))
                        .SetName("[1,2,3].push(4) -> var newArray:Int = [1,2,3].push(4)");
                }
            }

            [Test, TestCaseSource(nameof(HaxeTestCases))]
            public string Haxe(string sourceText, string newName)
            {
                SetHaxeFeatures(sci);
                return Common(sourceText, newName, sci);
            }

            static IEnumerable<TestCaseData> AS3TestCases
            {
                get
                {
                    yield return new TestCaseData(ReadAllTextAS3("BeforeExtractLocalVariable"), "newVar")
                        .Returns(ReadAllTextAS3("AfterExtractLocalVariable"))
                        .SetName("getChildByName('child') -> var newVar:DisplayObject = getChildByName('child')");
                    yield return new TestCaseData(ReadAllTextAS3("BeforeExtractLocalVariable_fromString"), "newVar")
                        .Returns(ReadAllTextAS3("AfterExtractLocalVariable_fromString"))
                        .SetName("ExtractLocaleVariable from String");
                    yield return new TestCaseData(ReadAllTextAS3("BeforeExtractLocalVariable_fromNumber"), "newVar")
                        .Returns(ReadAllTextAS3("AfterExtractLocalVariable_fromNumber"))
                        .SetName("ExtractLocaleVariable from Number");
                }
            }
                
            [Test, TestCaseSource(nameof(AS3TestCases))]
            public string AS3(string sourceText, string newName)
            {
                SetAs3Features(sci);
                return Common(sourceText, newName, sci);
            }

            static string Common(string sourceText, string newName, ScintillaControl sci)
            {
                SetSrc(sci, sourceText);
                CommandFactoryProvider.GetFactory(sci)
                    .CreateExtractLocalVariableCommand(false, newName)
                    .Execute();
                return sci.Text;
            }
        }

        [TestFixture]
        public class ExtractMethodTests : RefactorCommandTests
        {
            static IEnumerable<TestCaseData> HaxeTestCases
            {
                get
                {
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case1"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case1"))
                            .SetName("Issue 1617. Case 1.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case2"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case2"))
                            .SetName("Issue 1617. Case 2.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case3"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case3"))
                            .SetName("Issue 1617. Case 3.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case4"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case4"))
                            .SetName("Issue 1617. Case 4.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case5"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case5"))
                            .SetName("Issue 1617. Case 5.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case6"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case6"))
                            .SetName("Issue 1617. Case 6.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case7"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case7"))
                            .SetName("Issue 1617. Case 7.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case8"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case8"))
                            .SetName("Issue 1617. Case 8.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case9"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case9"))
                            .SetName("Issue 1617. Case 9.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case10"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case10"))
                            .SetName("Issue 1617. Case 10.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                    yield return
                        new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractMethod_issue1617_case11"), "newVar")
                            .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractMethod_issue1617_case11"))
                            .SetName("Issue 1617. Case 11.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/1617");
                }
            }

            [Test, TestCaseSource(nameof(HaxeTestCases))]
            public string Haxe(string sourceText, string newName)
            {
                SetHaxeFeatures(sci);
                return Common(sourceText, newName, sci);
            }

            static string Common(string sourceText, string newName, ScintillaControl sci)
            {
                SetSrc(sci, sourceText);
                CommandFactoryProvider.GetFactory(sci)
                    .CreateExtractMethodCommand(newName)
                    .Execute();
                return sci.Text;
            }
        }

        [TestFixture]
        public class ExtractLocalVariableWithContextualGenerator : RefactorCommandTests
        {
            static IEnumerable<TestCaseData> HaxeTestCases
            {
                get
                {
                    yield return new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractLocalVariable_withContextualGenerator"), "newVar", 0)
                        .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractLocalVariable_ReplaceAllOccurrences"))
                        .SetName("ExtractLocaleVariable replace all occurrences");
                    yield return new TestCaseData(ExtractLocalVariable.ReadAllTextHaxe("BeforeExtractLocalVariable_withContextualGenerator"), "newVar", 1)
                        .Returns(ExtractLocalVariable.ReadAllTextHaxe("AfterExtractLocalVariable_ReplaceInitialOccurrence"))
                        .SetName("ExtractLocaleVariable replace initial occurrence");
                }
            }

            [Test, TestCaseSource(nameof(HaxeTestCases))]
            public string Haxe(string sourceText, string newName, int contextualGeneratorItem)
            {
                SetHaxeFeatures(sci);
                SetSrc(sci, sourceText);
                var command = (ExtractLocalVariableCommand)CommandFactoryProvider.GetFactory(sci).CreateExtractLocalVariableCommand(false, newName);
                command.Execute();
                ((CompletionListItem)command.CompletionList[contextualGeneratorItem]).PerformClick();
                return sci.Text;
            }
        }

        [TestFixture]
        public class OrganizeImportsTests : RefactorCommandTests
        {
            static string GetFullPath(string fileName) => $"{nameof(CodeRefactor)}.Test_Files.coderefactor.organizeimports.{fileName}.as";

            static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

            [TestFixtureSetUp]
            public void OrganizeImportsFixtureSetUp()
            {
                SetAs3Features(sci);
                // Needed for preprocessor directives...
                sci.SetProperty("fold", "1");
                sci.SetProperty("fold.preprocessor", "1");
            }

            static IEnumerable<TestCaseData> TestCases
            {
                get
                {
                    yield return new TestCaseData("BeforeOrganizeImports", false)
                        .Returns(ReadAllText("AfterOrganizeImports"))
                        .SetName("OrganizeImports. Case 1");
                    yield return new TestCaseData("BeforeOrganizeImports_2", false)
                        .Returns(ReadAllText("AfterOrganizeImports_2"))
                        .SetName("OrganizeImports. Case 2");
                    yield return new TestCaseData("BeforeOrganizeImports_3", false)
                        .Returns(ReadAllText("AfterOrganizeImports_3"))
                        .SetName("OrganizeImports. Case 3");
                    yield return new TestCaseData("BeforeOrganizeImports_4", true)
                        .Returns(ReadAllText("AfterOrganizeImports_4"))
                        .SetName("OrganizeImports. Case 4");
                    yield return new TestCaseData("BeforeOrganizeImports_issue592_1", false)
                        .Returns(ReadAllText("BeforeOrganizeImports_issue592_1"))
                        .SetName("Issue 592. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/592");
                    yield return new TestCaseData("BeforeOrganizeImports_issue592_2", false)
                        .Returns(ReadAllText("BeforeOrganizeImports_issue592_2"))
                        .SetName("Issue 592. Case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/592");
                }
            }

            [
                Test,
                TestCaseSource(nameof(TestCases)),
            ]
            public string OrganizeImports(string fileName, bool separatePackages) => OrganizeImports(sci, ReadAllText(fileName), fileName, separatePackages);

            public static string OrganizeImports(ScintillaControl sci, string sourceText, string fileName, bool separatePackages)
            {
                ASContext.Context.CurrentModel.FileName = fileName;
                SetSrc(sci, sourceText);
                var command = CommandFactoryProvider.GetFactory(sci)
                    .CreateOrganizeImportsCommand();
                ((OrganizeImports) command).SeparatePackages = separatePackages;
                command.Execute();
                return sci.Text;
            }

            static IEnumerable<TestCaseData> Issue781TestCases
            {
                get
                {
                    yield return new TestCaseData("BeforeOrganizeImports", false)
                        .Returns(true)
                        .SetName("Issue 781. Case 1");
                    yield return new TestCaseData("BeforeOrganizeImports_issue781_1", false)
                        .Returns(false)
                        .SetName("Issue 781. Case 2");
                }
            }

            [
                Test,
                TestCaseSource(nameof(Issue781TestCases)),
            ]
            public bool OrganizeImportsIssue781(string fileName, bool separatePackages)
            {
                var sourceText = ReadAllText(fileName);
                OrganizeImports(sci, sourceText, fileName, separatePackages);
                return sci.Length != sourceText.Length;
            }
        }

        [TestFixture]
        public class RenameTests : RefactorCommandTests
        {
            static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

            static string GetFullPath(string fileName) => $"{nameof(CodeRefactor)}.Test_Files.coderefactor.rename.as3.{fileName}.as";

            static IEnumerable<TestCaseData> TestCases
            {
                get
                {
                    yield return new TestCaseData("BeforeRenameLocalVariable", "newName")
                        .Returns(ReadAllText("AfterRenameLocalVariable"))
                        .SetName("Rename local variable");
                    yield return new TestCaseData("BeforeRename_issue1852", "b")
                        .Returns(ReadAllText("AfterRename_issue1852"))
                        .SetName("Issue 1852");
                    yield return new TestCaseData("BeforeRenameParameterVar", "newName")
                        .Returns(ReadAllText("AfterRenameParameterVar"))
                        .SetName("Rename parameter of function");
                    yield return new TestCaseData("BeforeRenameClass", "Bar")
                        .Returns(ReadAllText("AfterRenameClass"))
                        .SetName("Rename class");
                }
            }

            static readonly string testFilesAssemblyPath = $"\\FlashDevelop\\Bin\\Debug\\{nameof(CodeRefactor)}\\Test_Files\\";
            static readonly string testFilesDirectory = $"\\Tests\\External\\Plugins\\{nameof(CodeRefactor)}.Tests\\Test Files\\";

            [Test, TestCaseSource(nameof(TestCases))]
            public string Rename(string fileName, string newName)
            {
                SetAs3Features(sci);
                var sourceText = ReadAllText(fileName);
                fileName = GetFullPath(fileName);
                fileName = Path.GetFileNameWithoutExtension(fileName).Replace('.', Path.DirectorySeparatorChar) + Path.GetExtension(fileName);
                fileName = Path.GetFullPath(fileName);
                fileName = fileName.Replace(testFilesAssemblyPath, testFilesDirectory);
                fileName = fileName.Replace(".as", "_withoutEntryPoint.as");
                ASContext.Context.CurrentModel.FileName = fileName;
                PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
                return Rename(sci, sourceText, newName);
            }

            public static string Rename(ScintillaControl sci, string sourceText, string newName)
            {
                var context = SynchronizationContext.Current;
                if (context == null) Assert.Ignore("SynchronizationContext.Current is null");
                SetSrc(sci, sourceText);
                var waitHandle = new AutoResetEvent(false);
                CommandFactoryProvider.GetFactory(sci)
                        .CreateRenameCommandAndExecute(RefactoringHelper.GetDefaultRefactorTarget(), false, newName)
                        .OnRefactorComplete += (sender, args) => waitHandle.Set();
                var end = DateTime.Now.AddSeconds(2);
                var result = false;
                while ((!result) && (DateTime.Now < end))
                {
                    context.Send(state => {}, new {});
                    result = waitHandle.WaitOne(0);
                }
                return sci.Text;
            }
        }
    }
}