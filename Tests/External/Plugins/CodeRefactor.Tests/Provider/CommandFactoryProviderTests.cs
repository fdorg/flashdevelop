using System.Collections.Generic;
using System.IO;
using ASCompletion.Completion;
using ASCompletion.Context;
using CodeRefactor.Commands;
using CodeRefactor.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PluginCore;

namespace CodeRefactor.Provider
{
    class CommandFactoryProviderTests : ASCompleteTests
    {
        [TestFixtureSetUp]
        public void Setup() => SetAs3Features(sci);

        static IEnumerable<TestCaseData> RenameValidatorTestCases
        {
            get
            {
                yield return new TestCaseData("RenameValidator_1")
                    .Returns(true)
                    .SetName("Issue 2013. case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2013");
                yield return new TestCaseData("RenameValidator_2")
                    .Returns(true)
                    .SetName("Issue 2013. case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2013");
                yield return new TestCaseData("RenameValidator_3")
                    .Returns(true)
                    .SetName("Issue 2013. case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2013");
                yield return new TestCaseData("RenameValidator_4")
                    .Returns(false)
                    .SetName("Issue 2013. case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2013");
                yield return new TestCaseData("RenameValidator_5")
                    .Returns(false)
                    .SetName("Issue 2013. case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2013");
                yield return new TestCaseData("RenameValidator_6")
                    .Returns(false)
                    .SetName("Issue 2013. case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2013");
                yield return new TestCaseData("RenameValidator_7")
                    .Returns(true)
                    .SetName("Issue 2013. case 7")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2013");
                yield return new TestCaseData("RenameValidator_8")
                    .Returns(true)
                    .SetName("Issue 2013. case 8")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2013");
            }
        }

        [Test, TestCaseSource(nameof(RenameValidatorTestCases))]
        public bool RenameValidator(string fileName)
        {
            SetSrc(sci, ReadAllText(fileName));
            fileName = GetFullPath(fileName);
            fileName = Path.GetFileNameWithoutExtension(fileName).Replace('.', Path.DirectorySeparatorChar) + Path.GetExtension(fileName);
            fileName = Path.GetFullPath(fileName);
            fileName = fileName.Replace($"\\FlashDevelop\\Bin\\Debug\\{nameof(CodeRefactor)}\\Test_Files\\", $"\\Tests\\External\\Plugins\\{nameof(CodeRefactor)}.Tests\\Test Files\\");
            ASContext.Context.CurrentModel.FileName = fileName;
            PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
            var validator = CommandFactoryProvider.DefaultFactory.GetValidator(typeof(Rename));
            var target = RefactoringHelper.GetDefaultRefactorTarget();
            if (target.Member != null) target.Member.InFile = ASContext.Context.CurrentModel;
            return validator(target);
            // utils
            string ReadAllText(string path) => TestFile.ReadAllText(GetFullPath(path));
            string GetFullPath(string path) => $"{nameof(CodeRefactor)}.Test_Files.{nameof(CodeRefactor).ToLower()}.{nameof(Rename).ToLower()}.as3.{path}.as";
        }

        static IEnumerable<TestCaseData> DelegateMethodsValidatorTestCases
        {
            get
            {
                yield return new TestCaseData("DelegateMethodsValidator_1")
                    .Returns(true)
                    .SetName("Issue 2412. case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2412");
                yield return new TestCaseData("DelegateMethodsValidator_2")
                    .Returns(false)
                    .SetName("Issue 2412. case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2412");
                yield return new TestCaseData("DelegateMethodsValidator_3")
                    .Returns(false)
                    .SetName("Issue 2412. case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2412");
                yield return new TestCaseData("DelegateMethodsValidator_4")
                    .Returns(false)
                    .SetName("Issue 2412. case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2412");
                yield return new TestCaseData("DelegateMethodsValidator_5")
                    .Returns(false)
                    .SetName("Issue 2412. case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2412");
                yield return new TestCaseData("DelegateMethodsValidator_6")
                    .Returns(false)
                    .SetName("Issue 2412. case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2412");
                yield return new TestCaseData("DelegateMethodsValidator_7")
                    .Returns(false)
                    .SetName("Issue 2412. case 7")
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
            fileName = fileName.Replace($"\\FlashDevelop\\Bin\\Debug\\{nameof(CodeRefactor)}\\Test_Files\\", $"\\Tests\\External\\Plugins\\{nameof(CodeRefactor)}.Tests\\Test Files\\");
            ASContext.Context.CurrentModel.FileName = fileName;
            PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
            var validator = CommandFactoryProvider.DefaultFactory.GetValidator(typeof(DelegateMethods));
            var target = RefactoringHelper.GetDefaultRefactorTarget();
            if (target.Member != null) target.Member.InFile = ASContext.Context.CurrentModel;
            return validator(target);
            // utils
            string ReadAllText(string path) => TestFile.ReadAllText(GetFullPath(path));
            string GetFullPath(string path) => $"{nameof(CodeRefactor)}.Test_Files.{nameof(CodeRefactor).ToLower()}.{nameof(DelegateMethods).ToLower()}.as3.{path}.as";
        }
    }
}
