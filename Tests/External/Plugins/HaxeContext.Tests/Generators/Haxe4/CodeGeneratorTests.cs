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
            ASContext.Context.Settings.InstalledSDKs = new[] {new InstalledSDK {Path = PluginBase.CurrentProject.CurrentSDK, Version = "4.0.0"}};
            SetHaxeFeatures(sci);
        }

        static IEnumerable<TestCaseData> AssignStatementToVarIssue1999TestCases
        {
            get
            {
                yield return new TestCaseData("BeforeAssignStatementToVar_issue1999_5", GeneratorJobType.AssignStatementToVar, true)
                    .Returns(Haxe3.CodeGeneratorTests.ReadAllText("AfterAssignStatementToVar_issue1999_5"))
                    .SetName("if(true){}\n(v:String)|. Assign statement to var")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1999");
                yield return new TestCaseData("BeforeContextualGeneratorTests_GenerateFunction_5", GeneratorJobType.Function, true)
                    .Returns(Haxe3.CodeGeneratorTests.ReadAllText("AfterContextualGeneratorTests_GenerateFunction_5"))
                    .SetName("fo|o((this.bar().x:Int)). Generate function . Case 5");
            }
        }

        [
            Test,
            TestCaseSource(nameof(AssignStatementToVarIssue1999TestCases)),
        ]
        public string ContextualGenerator(string fileName, GeneratorJobType job, bool hasGenerator) => Haxe3.CodeGeneratorTests.ContextualGenerator(sci, fileName, job, hasGenerator);
    }

    [TestFixture]
    public class CodeGeneratorTests2 : ASCompletionTests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            ASContext.CommonSettings.DeclarationModifierOrder = new[] {"public", "protected", "internal", "private", "static", "inline", "override"};
            ASContext.CommonSettings.GeneratedMemberDefaultBodyStyle = GeneratedMemberBodyStyle.UncompilableCode;
            ASContext.Context.Settings.GenerateImports.Returns(true);
            ASContext.Context.Settings.InstalledSDKs = new[] {new InstalledSDK {Path = PluginBase.CurrentProject.CurrentSDK, Version = "4.0.0"}};
            SetHaxeFeatures(sci);
        }

        static IEnumerable<TestCaseData> GenerateFunctionTestCases
        {
            get
            {
                yield return new TestCaseData("BeforeGenerateFunction_issue103_13", GeneratorJobType.Function, true)
                    .Returns(Haxe3.CodeGeneratorTests.ReadAllText("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_13"))
                    .SetName("Issue103. Case 13")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_14", GeneratorJobType.Function, true)
                    .Returns(Haxe3.CodeGeneratorTests.ReadAllText("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_14"))
                    .SetName("Issue103. Case 14")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue103_14_1", GeneratorJobType.Function, true)
                    .Returns(Haxe3.CodeGeneratorTests.ReadAllText("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue103_14_1"))
                    .SetName("Issue103. Case 14.1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/103");
                yield return new TestCaseData("BeforeGenerateFunction_issue1645_2", GeneratorJobType.Function, true)
                    .Returns(Haxe3.CodeGeneratorTests.ReadAllText("AfterGenerateFunction_MemberDefaultBodyStyle_UncompilableCode_issue1645_2"))
                    .SetName("Issue1645. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1645");
            }
        }

        [
            Test,
            TestCaseSource(nameof(GenerateFunctionTestCases)),
        ]
        public string ContextualGenerator(string fileName, GeneratorJobType job, bool hasGenerator) => Haxe3.CodeGeneratorTests.ContextualGenerator(sci, fileName, job, hasGenerator);
    }
}