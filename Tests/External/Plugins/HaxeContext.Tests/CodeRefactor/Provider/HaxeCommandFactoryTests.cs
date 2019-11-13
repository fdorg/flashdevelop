using System.Collections.Generic;
using System.IO;
using ASCompletion.Completion;
using ASCompletion.Context;
using CodeRefactor.Commands;
using CodeRefactor.Provider;
using HaXeContext.TestUtils;
using NUnit.Framework;

namespace HaXeContext.CodeRefactor.Provider
{
    class HaxeCommandFactoryTests : ASCompleteTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            CommandFactoryProvider.Register("haxe", new HaxeCommandFactory());
            SetHaxeFeatures(sci);
        }

        static IEnumerable<TestCaseData> OrganizeImportsValidatorTestCases
        {
            get
            {
                yield return new TestCaseData("OrganizeImportsValidator_1")
                    .Returns(true)
                    .SetName("Issue 2284. case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2284");
                yield return new TestCaseData("OrganizeImportsValidator_2")
                    .Returns(false)
                    .SetName("Issue 2284. case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2284");
                yield return new TestCaseData("OrganizeImportsValidator_3")
                    .Returns(true)
                    .SetName("Issue 2284. case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2284");
                yield return new TestCaseData("import")
                    .Returns(false)
                    .SetName("Issue 2284. case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2284");
            }
        }

        [Test, TestCaseSource(nameof(OrganizeImportsValidatorTestCases))]
        public bool OrganizeImportsValidator(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            fileName = GetFullPath(fileName);
            fileName = Path.GetFileNameWithoutExtension(fileName).Replace('.', Path.DirectorySeparatorChar) + Path.GetExtension(fileName);
            fileName = Path.GetFullPath(fileName);
            fileName = fileName.Replace($"\\FlashDevelop\\Bin\\Debug\\{nameof(HaXeContext)}\\Test_Files\\", $"\\Tests\\External\\Plugins\\{nameof(HaXeContext)}.Tests\\Test Files\\");
            ASContext.Context.CurrentModel.FileName = fileName;
            var validator = CommandFactoryProvider.GetFactory(ASContext.Context.CurrentModel).GetValidator(typeof(OrganizeImports));
            return validator(new ASResult {InFile = ASContext.Context.CurrentModel});
            // Utils
            string GetFullPath(string path) => $"{nameof(HaXeContext)}.Test_Files.{nameof(CodeRefactor).ToLower()}.{nameof(OrganizeImports).ToLower()}.{path}.hx";
            string ReadAllText(string path) => TestFile.ReadAllText(GetFullPath(path));
        }

        static IEnumerable<TestCaseData> DelegateMethodsValidatorTestCases
        {
            get
            {
                yield return new TestCaseData("DelegateMethodsValidator_1")
                    .Returns(true)
                    .SetName("Delegate methods. Issue 2412. case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2412");
                yield return new TestCaseData("DelegateMethodsValidator_2")
                    .Returns(false)
                    .SetName("Delegate methods. Issue 2412. case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2412");
                yield return new TestCaseData("DelegateMethodsValidator_3")
                    .Returns(false)
                    .SetName("Delegate methods. Issue 2412. case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2412");
                yield return new TestCaseData("DelegateMethodsValidator_4")
                    .Returns(false)
                    .SetName("Delegate methods. Issue 2412. case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2412");
                yield return new TestCaseData("DelegateMethodsValidator_5")
                    .Returns(true)
                    .SetName("var value<generate> = 1. Delegate methods. Issue 2412. case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2412");
            }
        }

        [Test, TestCaseSource(nameof(DelegateMethodsValidatorTestCases))]
        public bool DelegateMethodsValidator(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            fileName = GetFullPath(fileName);
            fileName = Path.GetFileNameWithoutExtension(fileName).Replace('.', Path.DirectorySeparatorChar) + Path.GetExtension(fileName);
            fileName = Path.GetFullPath(fileName);
            fileName = fileName.Replace($"\\FlashDevelop\\Bin\\Debug\\{nameof(HaXeContext)}\\Test_Files\\", $"\\Tests\\External\\Plugins\\{nameof(HaXeContext)}.Tests\\Test Files\\");
            ASContext.Context.CurrentModel.FileName = fileName;
            var validator = CommandFactoryProvider.GetFactory(ASContext.Context.CurrentModel).GetValidator(typeof(DelegateMethods));
            var target = RefactoringHelper.GetDefaultRefactorTarget();
            if (target.Member != null) target.Member.InFile = ASContext.Context.CurrentModel;
            return validator(target);
            // Utils
            string GetFullPath(string path) => $"{nameof(HaXeContext)}.Test_Files.{nameof(CodeRefactor).ToLower()}.{nameof(DelegateMethods).ToLower()}.{path}.hx";
            string ReadAllText(string path) => TestFile.ReadAllText(GetFullPath(path));
        }
    }
}
