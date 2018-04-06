using System.Collections.Generic;
using System.IO;
using ASCompletion;
using ASCompletion.Context;
using ASCompletion.Generators;
using HaXeContext.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using ScintillaNet;

namespace HaXeContext.Generators
{
    using DocumentationGeneratorItem = ASCompletion.Generators.DocumentationGenerator.GeneratorItem;

    public class DocumentationGeneratorTests : ASCompletionTests
    {
        protected static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

        protected static string GetFullPath(string fileName) => $"{nameof(HaXeContext)}.Test_Files.generators.documentation.{fileName}.hx";

        [TestFixtureSetUp]
        public void DocumentationGeneratorSetUp() => SetHaxeFeatures(sci);

        public class ContextualGeneratorTests : DocumentationGeneratorTests
        {
            internal static string Impl(ScintillaControl sci, string fileName, DocumentationGeneratorJobType job, bool enableLeadingAsterisks, bool hasGenerator)
            {
                SetHaxeFeatures(sci);
                var sourceText = ReadAllText(fileName);
                fileName = GetFullPath(fileName);
                fileName = Path.GetFileNameWithoutExtension(fileName).Replace('.', Path.DirectorySeparatorChar) + Path.GetExtension(fileName);
                fileName = Path.GetFullPath(fileName);
                fileName = fileName.Replace($"\\FlashDevelop\\Bin\\Debug\\{nameof(HaXeContext)}\\Test_Files\\", $"\\Tests\\External\\Plugins\\{nameof(HaXeContext)}.Tests\\Test Files\\");
                ASContext.Context.CurrentModel.FileName = fileName;
                PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
                return Common(sci, sourceText, job, enableLeadingAsterisks, hasGenerator);
            }

            internal static string Common(ScintillaControl sci, string sourceText, DocumentationGeneratorJobType job, bool enableLeadingAsterisks, bool hasGenerator)
            {
                SetSrc(sci, sourceText);
                var options = new List<ICompletionListItem>();
                ((HaXeSettings) ASContext.Context.Settings).EnableLeadingAsterisks = enableLeadingAsterisks;
                ASContext.Context.DocumentationGenerator.ContextualGenerator(sci, sci.CurrentPos, options);
                var item = options.Find(it => it is DocumentationGeneratorItem && ((DocumentationGeneratorItem)it).Job == job);
                if (hasGenerator)
                {
                    Assert.NotNull(item);
                    var value = item.Value;
                }
                else Assert.IsNull(item);
                return sci.Text;
            }

            public class ContextualGeneratorTestsEnableLeadingAsterisks : ContextualGeneratorTests
            {
                public IEnumerable<TestCaseData> TestCases
                {
                    get
                    {
                        yield return new TestCaseData("BeforeGenerateDocumentation_none", DocumentationGeneratorJobType.Empty, false)
                            .Returns(ReadAllText("AfterGenerateDocumentation_none"))
                            .SetName("None");
                        yield return new TestCaseData("BeforeGenerateDocumentation_empty", DocumentationGeneratorJobType.Empty, true)
                            .Returns(ReadAllText("AfterGenerateDocumentation_empty"))
                            .SetName("Empty");
                        yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails", DocumentationGeneratorJobType.MethodDetails, true)
                            .Returns(ReadAllText("AfterGenerateDocumentation_methodDetails"))
                            .SetName("MethodDetails");
                        yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_2", DocumentationGeneratorJobType.MethodDetails, true)
                            .Returns(ReadAllText("AfterGenerateDocumentation_methodDetails_2"))
                            .SetName("MethodDetails. Case 2");
                        yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_3", DocumentationGeneratorJobType.MethodDetails, true)
                            .Returns(ReadAllText("AfterGenerateDocumentation_methodDetails_3"))
                            .SetName("MethodDetails. Case 3");
                        yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_4", DocumentationGeneratorJobType.MethodDetails, true)
                            .Returns(ReadAllText("AfterGenerateDocumentation_methodDetails_4"))
                            .SetName("MethodDetails. Case 4");
                        yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_5", DocumentationGeneratorJobType.MethodDetails, true)
                            .Returns(ReadAllText("AfterGenerateDocumentation_methodDetails_5"))
                            .SetName("MethodDetails. Case 5");
                        yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_6", DocumentationGeneratorJobType.MethodDetails, true)
                            .Returns(ReadAllText("AfterGenerateDocumentation_methodDetails_6"))
                            .SetName("MethodDetails. Case 6");
                        yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_7", DocumentationGeneratorJobType.MethodDetails, true)
                            .Returns(ReadAllText("AfterGenerateDocumentation_methodDetails_7"))
                            .SetName("MethodDetails. Case 7");
                    }
                }

                [Test, TestCaseSource(nameof(TestCases))]
                public string Haxe(string fileName, DocumentationGeneratorJobType job, bool hasGenerator) => Impl(sci, fileName, job, true, hasGenerator);
            }

            public class ContextualGeneratorTestsDisableLeadingAsterisks : ContextualGeneratorTests
            {
                public IEnumerable<TestCaseData> TestCases
                {
                    get
                    {
                        yield return new TestCaseData("BeforeGenerateDocumentation_empty", DocumentationGeneratorJobType.Empty)
                            .Returns(ReadAllText("AfterGenerateDocumentation_disableLeadingAsterisks_empty"))
                            .SetName("Empty");
                        yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails", DocumentationGeneratorJobType.MethodDetails)
                            .Returns(ReadAllText("AfterGenerateDocumentation_disableLeadingAsterisks_methodDetails"))
                            .SetName("MethodDetails");
                        yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_2", DocumentationGeneratorJobType.MethodDetails)
                            .Returns(ReadAllText("AfterGenerateDocumentation_disableLeadingAsterisks_methodDetails_2"))
                            .SetName("MethodDetails. Case 2");
                        yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_3", DocumentationGeneratorJobType.MethodDetails)
                            .Returns(ReadAllText("AfterGenerateDocumentation_disableLeadingAsterisks_methodDetails_3"))
                            .SetName("MethodDetails. Case 3");
                        yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_4", DocumentationGeneratorJobType.MethodDetails)
                            .Returns(ReadAllText("AfterGenerateDocumentation_disableLeadingAsterisks_methodDetails_4"))
                            .SetName("MethodDetails. Case 4");
                        yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_5", DocumentationGeneratorJobType.MethodDetails)
                            .Returns(ReadAllText("AfterGenerateDocumentation_disableLeadingAsterisks_methodDetails_5"))
                            .SetName("MethodDetails. Case 5");
                        yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_6", DocumentationGeneratorJobType.MethodDetails)
                            .Returns(ReadAllText("AfterGenerateDocumentation_disableLeadingAsterisks_methodDetails_6"))
                            .SetName("MethodDetails. Case 6");
                    }
                }

                [Test, TestCaseSource(nameof(TestCases))]
                public string Haxe(string fileName, DocumentationGeneratorJobType job) => Impl(sci, fileName, job, false, true);
            }
        }
    }
}