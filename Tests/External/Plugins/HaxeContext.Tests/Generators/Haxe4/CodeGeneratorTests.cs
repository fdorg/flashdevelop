using System.Collections.Generic;
using ASCompletion;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Settings;
using NSubstitute;
using NUnit.Framework;
using PluginCore;

namespace HaXeContext.Generators.Haxe4
{
    [TestFixture]
    public class CodeGeneratorTests : ASCompletionTests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            ASContext.CommonSettings.DeclarationModifierOrder = new[] {"public", "protected", "internal", "private", "static", "inline", "override"};
            ASContext.CommonSettings.GeneratedMemberDefaultBodyStyle = GeneratedMemberBodyStyle.ReturnDefaultValue;
            ASContext.Context.Settings.GenerateImports.Returns(true);
            SetHaxeFeatures(sci);
            ASContext.Context.Settings.InstalledSDKs = new[] {new InstalledSDK {Path = PluginBase.CurrentProject.CurrentSDK, Version = "4.0.0"}};
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue1999TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1999_5", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(Haxe3.CodeGeneratorTests.ReadAllText("AfterAssignStatementToVar_issue1999_5"))
                    .SetName("if(true){}\n(v:String)|. Assign statement to var")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1999");
            }
        }

        [
            Test,
            TestCaseSource(nameof(AssignStatementToVarIssue1999TestCases)),
        ]
        public string ContextualGenerator(string fileName, GeneratorJobType job, bool hasGenerator) => Haxe3.CodeGeneratorTests.ContextualGenerator(sci, fileName, job, hasGenerator);
    }
}