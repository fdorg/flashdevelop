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

namespace HaXeContext.CodeRefactor.Commands
{
    [TestFixture]
    class OrganizeImportsTests : ASCompleteTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            CommandFactoryProvider.Register("haxe", new HaxeCommandFactory());
            SetHaxeFeatures(sci);
            // Needed for preprocessor directives...
            sci.SetProperty("fold", "1");
            sci.SetProperty("fold.preprocessor", "1");
        }
        
        static string GetFullPath(string fileName) => $"{nameof(HaXeContext)}.Test_Files.coderefactor.organizeimports.{fileName}.hx";

        static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

        static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("BeforeOrganizeImports"), "BeforeOrganizeImports.hx", false)
                    .Returns(ReadAllText("AfterOrganizeImports"))
                    .SetName("OrganizeImports. Case 1");
                yield return new TestCaseData(ReadAllText("BeforeOrganizeImports_2"), "BeforeOrganizeImports_2.hx", false)
                    .Returns(ReadAllText("AfterOrganizeImports_2"))
                    .SetName("OrganizeImports. Case 2");
                yield return new TestCaseData(ReadAllText("BeforeOrganizeImports_issue191_1"), "BeforeOrganizeImports_issue191_1.hx", false)
                    .Returns(ReadAllText("AfterOrganizeImports_issue191_1"))
                    .SetName("Issue191. Case 1.")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/191");
                yield return new TestCaseData(ReadAllText("BeforeOrganizeImports_withImportsFromSameModule"), "Main.hx", false)
                    .Returns(ReadAllText("AfterOrganizeImports_withImportsFromSameModule"))
                    .SetName("Issue782. Package is empty.");
                yield return new TestCaseData(ReadAllText("BeforeOrganizeImports_withImportsFromSameModule2"), "Main.hx", false)
                    .Returns(ReadAllText("AfterOrganizeImports_withImportsFromSameModule2"))
                    .SetName("Issue782. Package is not empty.");
                yield return new TestCaseData(ReadAllText("BeforeOrganizeImports_withImportsFromSameModule2"), "Main.hx", false)
                    .Returns(ReadAllText("AfterOrganizeImports_withImportsFromSameModule2"))
                    .SetName("Issue782. Package is not empty.");
                yield return new TestCaseData(ReadAllText("BeforeOrganizeImports_withElseIfDirective"), "Main.hx", false)
                    .Returns(ReadAllText("AfterOrganizeImports_withElseIfDirective"))
                    .SetName("Issue783. Shouldn't touch #elseif blocks.");
                yield return new TestCaseData(ReadAllText("BeforeOrganizeImports_importStaticMember"), "BeforeOrganizeImports_importStaticMember.hx", false)
                    .Returns(ReadAllText("BeforeOrganizeImports_importStaticMember"))
                    .SetName("Issue783. Shouldn't touch #elseif blocks.");
            }
        }

        static IEnumerable<TestCaseData> Issue1342TestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("BeforeOrganizeImports_Issue1342"), "BeforeOrganizeImports.hx", true)
                    .Returns(ReadAllText("AfterOrganizeImports_Issue1342"))
                    .SetName("Issue1342. Separate Packages = True");
            }
        }

        static IEnumerable<TestCaseData> Issue2512TestCases
        {
            get
            {
                yield return new TestCaseData(ReadAllText("BeforeOrganizeImports_issue2512_1"), "BeforeOrganizeImports_issue2512_1.hx", false)
                    .Returns(ReadAllText("BeforeOrganizeImports_issue2512_1"))
                    .SetName("'${Json.stringify({x:1})}'. Issue2512. String interpolation");
                yield return new TestCaseData(ReadAllText("BeforeOrganizeImports_issue2512_2"), "BeforeOrganizeImports_issue2512_2.hx", false)
                    .Returns(ReadAllText("AfterOrganizeImports_issue2512_2"))
                    .SetName("\"${Json.stringify({x:1})}\". Issue2512. String interpolation");
            }
        }

        [
            Test, 
            TestCaseSource(nameof(TestCases)),
            TestCaseSource(nameof(Issue1342TestCases)),
            TestCaseSource(nameof(Issue2512TestCases)),
        ]
        public string OrganizeImports(string sourceText, string fileName, bool separatePackages) => global::CodeRefactor.Commands.RefactorCommandTests.OrganizeImportsTests.OrganizeImports(sci, sourceText, fileName, separatePackages);
    }

    [TestFixture]
    public class RenameCommandTests : ASCompleteTests
    {
        static readonly string ProjectPath = $"\\Tests\\External\\Plugins\\{nameof(HaXeContext)}.Tests\\Test Files\\";

        [TestFixtureSetUp]
        public void Setup()
        {
            SetHaxeFeatures(sci);
            CommandFactoryProvider.Register("haxe", new HaxeCommandFactory());
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
                yield return new TestCaseData("BeforeRename_enum_3", "NewName")
                    .Returns(ReadAllText("AfterRename_enum_3"))
                    .SetName("Enu|mAbstractValue");
                yield return new TestCaseData("BeforeRename_enum_4", "NewName")
                    .Returns(ReadAllText("AfterRename_enum_4"))
                    .SetName("AType.Enu|mAbstractValue. without access modifiers");
                yield return new TestCaseData("BeforeRename_enum_5", "NewName")
                    .Returns(ReadAllText("AfterRename_enum_5"))
                    .SetName("AType.Enu|mAbstractValue. with access modifiers");
            }
        }

        static IEnumerable<TestCaseData> RenameEnumAbstractValueTestIssue2373Cases
        {
            get
            {
                yield return new TestCaseData("BeforeRename_issue2373_1", "NewName")
                    .Returns(ReadAllText("AfterRename_issue2373_1"))
                    .SetName("AType.Enu|mAbstractValue. Issue 2373. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2373");
            }
        }

        [
            Test,
            TestCaseSource(nameof(TestCases)),
            TestCaseSource(nameof(RenameEnumTestCases)),
            TestCaseSource(nameof(RenameEnumAbstractValueTestIssue2373Cases)),
        ]
        public string Rename(string fileName, string newName)
        {
            ((HaXeSettings) ASContext.GetLanguageContext("haxe").Settings).CompletionMode = HaxeCompletionModeEnum.FlashDevelop;
            return Common(fileName, newName);
        }

        static IEnumerable<TestCaseData> TestCases2
        {
            get
            {
                yield return new TestCaseData("BeforeRenameOptionalParameterVar_issue2167_case_1", "newName", "3.4.7")
                    .Returns(ReadAllText("AfterRenameOptionalParameterVar_issue2167_case_1"))
                    .SetName("Rename optional parameter. local function. case 3")
                    .Ignore("")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2167");
            }
        }

        [
            Test,
            TestCaseSource(nameof(TestCases2)),
        ]
        public string Rename2(string fileName, string newName, string sdkVersion)
        {
            ASContext.Context.Settings.InstalledSDKs = new[] {new InstalledSDK {Path = PluginBase.CurrentProject.CurrentSDK, Version = sdkVersion}};
            ((HaXeSettings) ASContext.GetLanguageContext("haxe").Settings).CompletionMode = HaxeCompletionModeEnum.Compiler;
            return Common(fileName, newName);
        }

        string Common(string fileName, string newName)
        {
            var sourceText = ReadAllText(fileName);
            fileName = GetFullPath(fileName);
            fileName = Path.GetFileNameWithoutExtension(fileName).Replace('.', Path.DirectorySeparatorChar)
                       + Path.GetExtension(fileName);
            fileName = Path.GetFullPath(fileName);
            fileName = fileName.Replace($"\\FlashDevelop\\Bin\\Debug\\{nameof(HaXeContext)}\\Test_Files\\", ProjectPath);
            fileName = fileName.Replace(".hx", "_withoutEntryPoint.hx");
            ASContext.Context.CurrentModel.FileName = fileName;
            PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
            return global::CodeRefactor.Commands.RefactorCommandTests.RenameTests.Rename(sci, sourceText, newName);
        }
    }
}
