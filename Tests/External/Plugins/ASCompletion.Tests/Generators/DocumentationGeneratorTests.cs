using System.Collections.Generic;
using System.IO;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using ScintillaNet;

namespace ASCompletion.Generators
{
    public class DocumentationGeneratorTests : ASCompletionTests
    {
        [TestFixture]
        public class ContextualGeneratorTests : DocumentationGeneratorTests
        {
            [TestFixtureSetUp]
            public void SetUp()
            {
                var generator = new DocumentationGenerator();
                ASContext.Context.DocumentationGenerator.Returns(generator);
            }

            public IEnumerable<TestCaseData> AS3TestCases
            {
                get { yield return null; }
            }

            [Test, TestCaseSource(nameof(AS3TestCases))]
            public string AS3(string fileName, GeneratorJobType job, bool hasGenerator) => AS3Impl(sci, fileName, job, hasGenerator);

            internal static string AS3Impl(ScintillaControl sci, string fileName, GeneratorJobType job, bool hasGenerator)
            {
                SetHaxeFeatures(sci);
                var sourceText = ReadAllTextHaxe(fileName);
                fileName = GetFullPathHaxe(fileName);
                fileName = Path.GetFileNameWithoutExtension(fileName).Replace('.', Path.DirectorySeparatorChar) + Path.GetExtension(fileName);
                fileName = Path.GetFullPath(fileName);
                fileName = fileName.Replace($"\\FlashDevelop\\Bin\\Debug\\{nameof(ASCompletion)}\\Test_Files\\", $"\\Tests\\External\\Plugins\\{nameof(ASCompletion)}.Tests\\Test Files\\");
                ASContext.Context.CurrentModel.FileName = fileName;
                PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
                return Common(sci, sourceText, job, hasGenerator);
            }

            internal static string Common(ScintillaControl sci, string sourceText, GeneratorJobType job, bool hasGenerator)
            {
                SetSrc(sci, sourceText);
                var options = new List<ICompletionListItem>();
                ASContext.Context.DocumentationGenerator.ContextualGenerator(sci, sci.CurrentPos, options);
                var item = options.Find(it => it is GeneratorItem && ((GeneratorItem)it).job == job);
                if (hasGenerator)
                {
                    Assert.NotNull(item);
                    var value = item.Value;
                }
                else Assert.IsNull(item);
                return sci.Text;
            }
        }

        protected static string ReadAllTextAS3(string fileName) => TestFile.ReadAllText(GetFullPathAS3(fileName));

        protected static string GetFullPathAS3(string fileName) => $"ASCompletion.Test_Files.generated.as3.{fileName}.as";

        protected static string ReadAllTextHaxe(string fileName) => TestFile.ReadAllText(GetFullPathHaxe(fileName));

        protected static string GetFullPathHaxe(string fileName) => $"ASCompletion.Test_Files.generated.haxe.{fileName}.hx";
    }
}