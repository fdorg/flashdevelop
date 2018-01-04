﻿using System.Collections.Generic;
using System.IO;
using ASCompletion;
using ASCompletion.Context;
using ASCompletion.Generators;
using ASCompletion.Model;
using ASCompletion.TestUtils;
using HaXeContext.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using PluginCore.Helpers;
using ScintillaNet;

namespace HaXeContext.Generators
{
    using GeneratorItem = ASCompletion.Generators.DocumentationGenerator.GeneratorItem;

    public class DocumentationGeneratorTests : ASCompletionTests
    {
        [TestFixture]
        public class ContextualGeneratorTests : DocumentationGeneratorTests
        {
            [TestFixtureSetUp]
            public void SetUp() => ASContext.Context.SetHaxeFeatures();

            public IEnumerable<TestCaseData> TestCases
            {
                get
                {
                    yield return new TestCaseData("BeforeGenerateDocumentation_empty", DocumentationGeneratorJobType.Empty)
                        .Returns(ReadAllText("AfterGenerateDocumentation_empty"))
                        .SetName("Empty");
                    yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails", DocumentationGeneratorJobType.MethodDetails)
                        .Returns(ReadAllText("AfterGenerateDocumentation_methodDetails"))
                        .SetName("MethodDetails");
                    yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_2", DocumentationGeneratorJobType.MethodDetails)
                        .Returns(ReadAllText("AfterGenerateDocumentation_methodDetails_2"))
                        .SetName("MethodDetails. Case 2");
                    yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_3", DocumentationGeneratorJobType.MethodDetails)
                        .Returns(ReadAllText("AfterGenerateDocumentation_methodDetails_3"))
                        .SetName("MethodDetails. Case 3");
                    yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_4", DocumentationGeneratorJobType.MethodDetails)
                        .Returns(ReadAllText("AfterGenerateDocumentation_methodDetails_4"))
                        .SetName("MethodDetails. Case 4");
                    yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_5", DocumentationGeneratorJobType.MethodDetails)
                        .Returns(ReadAllText("AfterGenerateDocumentation_methodDetails_5"))
                        .SetName("MethodDetails. Case 5");
                    yield return new TestCaseData("BeforeGenerateDocumentation_methodDetails_6", DocumentationGeneratorJobType.MethodDetails)
                        .Returns(ReadAllText("AfterGenerateDocumentation_methodDetails_6"))
                        .SetName("MethodDetails. Case 6");
                }
            }

            [Test, TestCaseSource(nameof(TestCases))]
            public string Haxe(string fileName, DocumentationGeneratorJobType job) => Impl(sci, fileName, job);

            internal static string Impl(ScintillaControl sci, string fileName, DocumentationGeneratorJobType job)
            {
                SetHaxeFeatures(sci);
                var sourceText = ReadAllText(fileName);
                fileName = GetFullPath(fileName);
                fileName = Path.GetFileNameWithoutExtension(fileName).Replace('.', Path.DirectorySeparatorChar) + Path.GetExtension(fileName);
                fileName = Path.GetFullPath(fileName);
                fileName = fileName.Replace($"\\FlashDevelop\\Bin\\Debug\\{nameof(HaXeContext)}\\Test_Files\\", $"\\Tests\\External\\Plugins\\{nameof(HaXeContext)}.Tests\\Test Files\\");
                ASContext.Context.CurrentModel.FileName = fileName;
                PluginBase.MainForm.CurrentDocument.FileName.Returns(fileName);
                return Common(sci, sourceText, job);
            }

            internal static string Common(ScintillaControl sci, string sourceText, DocumentationGeneratorJobType job)
            {
                SetSrc(sci, sourceText);
                var options = new List<ICompletionListItem>();
                ASContext.Context.DocumentationGenerator.ContextualGenerator(sci, sci.CurrentPos, options);
                var item = options.Find(it => it is GeneratorItem && ((GeneratorItem)it).Job == job);
                Assert.NotNull(item);
                var value = item.Value;
                return sci.Text;
            }
        }

        protected static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

        protected static string GetFullPath(string fileName) => $"{nameof(HaXeContext)}.Test_Files.generators.documentation.{fileName}.hx";

        protected new static void SetSrc(ScintillaControl sci, string sourceText)
        {
            sci.Text = sourceText;
            SnippetHelper.PostProcessSnippets(sci, 0);
            var currentModel = ASContext.Context.CurrentModel;
            new ASFileParser().ParseSrc(currentModel, sci.Text);
            var line = sci.CurrentLine;
            ASContext.Context.CurrentClass.Returns(currentModel.Classes.FirstOrDefault(line));
        }
    }
}