using System.Collections.Generic;
using System.IO;
using ASCompletion.Context;
using ASCompletion.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using ScintillaNet;

namespace ASCompletion.Generators
{
    [TestFixture]
    public class DocumentationGeneratorTests : ASCompletionTests
    {
        protected static string ReadAllTextAS3(string fileName) => TestFile.ReadAllText(GetFullPathAS3(fileName));

        protected static string GetFullPathAS3(string fileName) => $"{nameof(ASCompletion)}.Test_Files.generated.as3.documentation.{fileName}.as";

        public class ContextualGeneratorTests : DocumentationGeneratorTests
        {
            [TestFixtureSetUp]
            public void SetUp() => ASContext.Context.SetAs3Features();

            public IEnumerable<TestCaseData> TestCases
            {
                get
                {
                    yield return new TestCaseData("BeforeGenerateDocumentation_none", DocumentationGeneratorJobType.Empty, false)
                        .Returns(ReadAllTextAS3("AfterGenerateDocumentation_none"))
                        .SetName("None");
                    yield return new TestCaseData("BeforeGenerateDocumentation_empty", DocumentationGeneratorJobType.Empty, true)
                        .Returns(ReadAllTextAS3("AfterGenerateDocumentation_empty"))
                        .SetName("Empty");
                    yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails", DocumentationGeneratorJobType.MethodDetails, true)
                        .Returns(ReadAllTextAS3("AfterGenerateDocumentation_methodDetails"))
                        .SetName("MethodDetails");
                    yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_2", DocumentationGeneratorJobType.MethodDetails, true)
                        .Returns(ReadAllTextAS3("AfterGenerateDocumentation_methodDetails_2"))
                        .SetName("MethodDetails. Case 2");
                    yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_3", DocumentationGeneratorJobType.MethodDetails, true)
                        .Returns(ReadAllTextAS3("AfterGenerateDocumentation_methodDetails_3"))
                        .SetName("MethodDetails. Case 3");
                    yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_4", DocumentationGeneratorJobType.MethodDetails, true)
                        .Returns(ReadAllTextAS3("AfterGenerateDocumentation_methodDetails_4"))
                        .SetName("MethodDetails. Case 4");
                }
            }

            [Test, TestCaseSource(nameof(TestCases))]
            public string AS3(string fileName, DocumentationGeneratorJobType job, bool hasGenerator) => Impl(sci, fileName, job, hasGenerator);

            internal static string Impl(ScintillaControl sci, string fileName, DocumentationGeneratorJobType job, bool hasGenerator)
            {
                SetHaxeFeatures(sci);
                var sourceText = ReadAllTextAS3(fileName);
                fileName = GetFullPathAS3(fileName);
                fileName = Path.GetFileNameWithoutExtension(fileName).Replace('.', Path.DirectorySeparatorChar) + Path.GetExtension(fileName);
                fileName = Path.GetFullPath(fileName);
                fileName = fileName.Replace($"\\FlashDevelop\\Bin\\Debug\\{nameof(ASCompletion)}\\Test_Files\\", $"\\Tests\\External\\Plugins\\{nameof(ASCompletion)}.Tests\\Test Files\\");
                ASContext.Context.CurrentModel.FileName = fileName;
                PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
                return Common(sci, sourceText, job, hasGenerator);
            }

            internal static string Common(ScintillaControl sci, string sourceText, DocumentationGeneratorJobType job, bool hasGenerator)
            {
                SetSrc(sci, sourceText);
                var options = new List<ICompletionListItem>();
                ASContext.Context.DocumentationGenerator.ContextualGenerator(sci, sci.CurrentPos, options);
                var item = options.Find(it => it is DocumentationGenerator.GeneratorItem && ((DocumentationGenerator.GeneratorItem)it).Job == job);
                if (hasGenerator)
                {
                    Assert.NotNull(item);
                    var value = item.Value;
                }
                else Assert.IsNull(item);
                return sci.Text;
            }
        }
    }
}