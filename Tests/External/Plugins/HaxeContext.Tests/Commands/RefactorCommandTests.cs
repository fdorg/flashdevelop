using System.Collections.Generic;
using CodeRefactor;
using CodeRefactor.Provider;
using HaXeContext.CodeRefactor.Provider;
using HaXeContext.TestUtils;
using NUnit.Framework;

namespace HaXeContext.Commands
{
    [TestFixture]
    class RefactorCommandTests : CodeRefactorTests
    {
        [TestFixture]
        public class OrganizeImports : RefactorCommandTests
        {
            protected static string ReadAllTextHaxe(string fileName) => TestFile.ReadAllText($"{nameof(HaXeContext)}.Test_Files.coderefactor.organizeimports.{fileName}.hx");

            [TestFixtureSetUp]
            public void OrganizeImportsFixtureSetUp()
            {
                // Needed for preprocessor directives...
                Sci.SetProperty("fold", "1");
                Sci.SetProperty("fold.preprocessor", "1");
                CommandFactoryProvider.Register("haxe", new HaxeCommandFactory());
            }

            public IEnumerable<TestCaseData> HaxeTestCases
            {
                get
                {
                    yield return
                        new TestCaseData(ReadAllTextHaxe("BeforeOrganizeImports_issue191_1"), "BeforeOrganizeImports_issue191_1.hx")
                            .Returns(ReadAllTextHaxe("AfterOrganizeImports_issue191_1"))
                            .SetName("Issue191. Case 1.")
                            .SetDescription("https://github.com/fdorg/flashdevelop/issues/191");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("BeforeOrganizeImports"), "BeforeOrganizeImports.hx")
                            .Returns(ReadAllTextHaxe("AfterOrganizeImports"))
                            .SetName("OrganizeImports");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("BeforeOrganizeImports_withImportsFromSameModule"), "Main.hx")
                            .Returns(ReadAllTextHaxe("AfterOrganizeImports_withImportsFromSameModule"))
                            .SetName("Issue782. Package is empty.");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("BeforeOrganizeImports_withImportsFromSameModule2"), "Main.hx")
                            .Returns(ReadAllTextHaxe("AfterOrganizeImports_withImportsFromSameModule2"))
                            .SetName("Issue782. Package is not empty.");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("BeforeOrganizeImports_withImportsFromSameModule2"), "Main.hx")
                            .Returns(ReadAllTextHaxe("AfterOrganizeImports_withImportsFromSameModule2"))
                            .SetName("Issue782. Package is not empty.");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("BeforeOrganizeImports_withElseIfDirective"), "Main.hx")
                            .Returns(ReadAllTextHaxe("AfterOrganizeImports_withElseIfDirective"))
                            .SetName("Issue783. Shouldn't touch #elseif blocks.");
                }
            }

            [Test, TestCaseSource(nameof(HaxeTestCases))]
            public string Haxe(string sourceText, string fileName) => global::CodeRefactor.Commands.RefactorCommandTests.OrganizeImports.HaxeImpl(Sci, sourceText, fileName);
        }
    }
}
