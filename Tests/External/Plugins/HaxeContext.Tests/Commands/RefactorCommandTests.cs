using System;
using System.Collections.Generic;
using System.IO;
using ASCompletion.Completion;
using ASCompletion.Context;
using CodeRefactor.Provider;
using HaXeContext.CodeRefactor.Provider;
using HaXeContext.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using ProjectManager;
using ProjectManager.Projects.Haxe;

namespace HaXeContext.Commands
{
    [TestFixture]
    class RefactorCommandTests : ASCompleteTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            CommandFactoryProvider.Register("haxe", new HaxeCommandFactory());
            SetHaxeFeatures(sci);
        }

        [TestFixture]
        public class OrganizeImportsTests : RefactorCommandTests
        {
            protected static string ReadAllText(string fileName) => TestFile.ReadAllText($"{nameof(HaXeContext)}.Test_Files.coderefactor.organizeimports.{fileName}.hx");

            [TestFixtureSetUp]
            public void OrganizeImportsFixtureSetUp()
            {
                // Needed for preprocessor directives...
                sci.SetProperty("fold", "1");
                sci.SetProperty("fold.preprocessor", "1");
            }

            static IEnumerable<TestCaseData> TestCases
            {
                get
                {
                    yield return
                        new TestCaseData(ReadAllText("BeforeOrganizeImports_issue191_1"), "BeforeOrganizeImports_issue191_1.hx")
                            .Returns(ReadAllText("AfterOrganizeImports_issue191_1"))
                            .SetName("Issue191. Case 1.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/191");
                    yield return
                        new TestCaseData(ReadAllText("BeforeOrganizeImports"), "BeforeOrganizeImports.hx")
                            .Returns(ReadAllText("AfterOrganizeImports"))
                            .SetName("OrganizeImports");
                    yield return
                        new TestCaseData(ReadAllText("BeforeOrganizeImports_withImportsFromSameModule"), "Main.hx")
                            .Returns(ReadAllText("AfterOrganizeImports_withImportsFromSameModule"))
                            .SetName("Issue782. Package is empty.");
                    yield return
                        new TestCaseData(ReadAllText("BeforeOrganizeImports_withImportsFromSameModule2"), "Main.hx")
                            .Returns(ReadAllText("AfterOrganizeImports_withImportsFromSameModule2"))
                            .SetName("Issue782. Package is not empty.");
                    yield return
                        new TestCaseData(ReadAllText("BeforeOrganizeImports_withImportsFromSameModule2"), "Main.hx")
                            .Returns(ReadAllText("AfterOrganizeImports_withImportsFromSameModule2"))
                            .SetName("Issue782. Package is not empty.");
                    yield return
                        new TestCaseData(ReadAllText("BeforeOrganizeImports_withElseIfDirective"), "Main.hx")
                            .Returns(ReadAllText("AfterOrganizeImports_withElseIfDirective"))
                            .SetName("Issue783. Shouldn't touch #elseif blocks.");
                }
            }

            [Test, TestCaseSource(nameof(TestCases))]
            public string OrganizeImports(string sourceText, string fileName) => global::CodeRefactor.Commands.RefactorCommandTests.OrganizeImports.HaxeImpl(sci, sourceText, fileName);
        }

        [TestFixture]
        public class RenameCommandTests : RefactorCommandTests
        {
            static readonly string ProjectPath = $"\\Tests\\External\\Plugins\\{nameof(HaXeContext)}.Tests\\Test Files\\";

            [TestFixtureSetUp]
            public new void Setup()
            {
                ProjectManager.PluginMain.Settings = new ProjectManagerSettings();
                PluginBase.CurrentProject = new HaxeProject(ProjectPath)
                {
                    CurrentSDK = Environment.GetEnvironmentVariable("HAXEPATH")
                };
            }

            static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

            static string GetFullPath(string fileName) => $"{nameof(HaXeContext)}.Test_Files.coderefactor.rename.{fileName}.hx";

            static IEnumerable<TestCaseData> TestCases
            {
                get
                {
                    yield return new TestCaseData("BeforeRenameOptionalParameterVar_issue2022_case_1", "newName")
                        .Returns(ReadAllText("AfterRenameOptionalParameterVar_issue2022_case_1"))
                        .SetName("Rename optional parameter. local function. case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2022");
                    yield return new TestCaseData("BeforeRenameOptionalParameterVar_issue2022_case_2", "newName")
                        .Returns(ReadAllText("AfterRenameOptionalParameterVar_issue2022_case_2"))
                        .SetName("Rename optional parameter. local function. case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/2022");
                    yield return new TestCaseData("BeforeRename_stringInterpolations_1", "newName")
                        .Returns(ReadAllText("AfterRename_stringInterpolations_1"))
                        .Ignore("")
                        .SetName("'${val|ue}'");
                }
            }

            static IEnumerable<TestCaseData> RenameEnumTestCases
            {
                get
                {
                    yield return new TestCaseData("BeforeRename_enum_1", "NewName")
                        .Returns(ReadAllText("AfterRename_enum_1"))
                        .SetName("Enu|mInstance");
                    yield return new TestCaseData("BeforeRename_enum_2", "NewName")
                        .Returns(ReadAllText("AfterRename_enum_2"))
                        .SetName("EType.Enu|mInstance");
                }
            }

            [
                Test,
                TestCaseSource(nameof(TestCases)),
                TestCaseSource(nameof(RenameEnumTestCases)),
            ]
            public string Rename(string fileName, string newName)
            {
                var sourceText = ReadAllText(fileName);
                fileName = GetFullPath(fileName);
                fileName = Path.GetFileNameWithoutExtension(fileName).Replace('.', Path.DirectorySeparatorChar) + Path.GetExtension(fileName);
                fileName = Path.GetFullPath(fileName);
                fileName = fileName.Replace($"\\FlashDevelop\\Bin\\Debug\\{nameof(HaXeContext)}\\Test_Files\\", ProjectPath);
                fileName = fileName.Replace(".hx", "_withoutEntryPoint.hx");
                ASContext.Context.CurrentModel.FileName = fileName;
                PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
                ((Context) ASContext.GetLanguageContext("haxe")).completionCache.IsDirty = true;
                return global::CodeRefactor.Commands.RefactorCommandTests.RenameTests.Rename(sci, sourceText, newName);
            }
        }
    }
}
