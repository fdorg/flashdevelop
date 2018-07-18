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

        static string ReadAllText(string fileName) => TestFile.ReadAllText($"{nameof(HaXeContext)}.Test_Files.coderefactor.organizeimports.{fileName}.hx");

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
        public string OrganizeImports(string sourceText, string fileName) => global::CodeRefactor.Commands.RefactorCommandTests.OrganizeImports.HaxeImpl(sci, sourceText, fileName, false);
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

        [
            Test,
            TestCaseSource(nameof(TestCases)),
            TestCaseSource(nameof(RenameEnumTestCases)),
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
            fileName = Path.GetFileNameWithoutExtension(fileName).Replace('.', Path.DirectorySeparatorChar) +
                       Path.GetExtension(fileName);
            fileName = Path.GetFullPath(fileName);
            fileName = fileName.Replace($"\\FlashDevelop\\Bin\\Debug\\{nameof(HaXeContext)}\\Test_Files\\", ProjectPath);
            fileName = fileName.Replace(".hx", "_withoutEntryPoint.hx");
            ASContext.Context.CurrentModel.FileName = fileName;
            PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
            //{TODO slavara: quick hack
            ASContext.Context.When(it => it.ResolveTopLevelElement(Arg.Any<string>(), Arg.Any<ASResult>()))
                .Do(it =>
                {
                    var ctx = (Context) ASContext.GetLanguageContext("haxe");
                    ctx.CurrentModel.FileName = fileName;
                    ctx.GetCodeModel(ctx.CurrentModel, sci.Text);
                    ctx.completionCache.IsDirty = true;
                    ctx.ResolveTopLevelElement(it.ArgAt<string>(0), it.ArgAt<ASResult>(1));
                });
            //}
            return global::CodeRefactor.Commands.RefactorCommandTests.RenameTests.Rename(sci, sourceText, newName);
        }
    }
}
