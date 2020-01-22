using System.Collections.Generic;
using NUnit.Framework;

namespace ProjectManager.Controls
{
    [TestFixture]
    public class OpenResourceFormTests
    {
        public class SearchUtilTests : OpenResourceFormTests
        {
            [Test]
            public void SimpleTest()
            {
                List<string> files = new List<string>();
                files.AddRange(new[] {
                    "src\\Main.hx",
                    "src\\StaticClass.hx",
                    "src\\com\\Test.xml",
                    "Package.bat",
                    "Run.bat",
                    "Test.hxproj",
                    "bin\\.empty",
                    "bin\\Test.n",
                    "src\\com\\module\\ITestModule.hx",
                    "src\\com\\module\\TestModule.hx",
                    "src\\com\\module\\example\\ExampleModule.hx",
                    "src\\com\\module\\example\\IExampleModule.hx",
                    "src\\com\\module\\test\\ITestModule.hx",
                    "src\\com\\module\\test\\TestModule.hx"
                });
                var results = SearchUtil.getMatchedItems(files, "m", '\\', 1);
                
                Assert.AreEqual("src\\Main.hx", results[0]); //shortest and also starts with an "m", so should be first
            }

            [Test]
            public void LimitTest()
            {
                List<string> files = new List<string>();
                files.AddRange(new[] {
                    "Main.hx",
                    "src\\com\\module\\example\\ExampleModule.hx",
                    "src\\com\\module\\example\\IExampleModule.hx",
                    "src\\com\\module\\test\\ITestModule.hx",
                    "src\\com\\module\\test\\TestModule.hx"
                });
                var results = SearchUtil.getMatchedItems(files, "m", '\\', 1);
                Assert.AreEqual(results.Count, 1);

                results = SearchUtil.getMatchedItems(files, "m", '\\', 1);
                Assert.Greater(results.Count, 0);
            }

            [Test]
            public void IClassTest()
            {
                List<string> files = new List<string>();
                files.AddRange(new[] {
                    "hexannotation\\hex\\annotation\\AnnotationData.hx",
                    "hexannotation\\hex\\annotation\\AnnotationReader.hx",
                    "hexannotation\\hex\\annotation\\ArgumentData.hx",
                    "hexannotation\\hex\\annotation\\ClassAnnotationData.hx",
                    "hexannotation\\hex\\annotation\\ClassAnnotationDataProvider.hx",
                    "hexannotation\\hex\\annotation\\IClassAnnotationDataProvider.hx",
                    "hexannotation\\hex\\annotation\\MethodAnnotationData.hx",
                    "hexannotation\\hex\\annotation\\PropertyAnnotationData.hx",
                    "src\\Main.hx",
                    "src\\StaticClass.hx"
                });
                var results = SearchUtil.getMatchedItems(files, "iclass", '\\', 2);

                //since the file name starts with iclass, it should be prefered over "src\\StaticClass.hx", which has 
                Assert.AreEqual("hexannotation\\hex\\annotation\\IClassAnnotationDataProvider.hx", results[0]);
                
                //however, "src\\StaticClass.hx" should be second, because it is only slightly off.
                Assert.AreEqual("src\\StaticClass.hx", results[1]);
            }

            [Test]
            public void TestTest()
            {
                List<string> files = new List<string>();
                files.AddRange(new[] {
                    "src\\StaticClass.hx",
                    "src\\com\\module\\ITestModule.hx",
                    "src\\com\\module\\TestModule.hx",
                    "src\\com\\module\\example\\ExampleModule.hx",
                    "src\\com\\module\\example\\IExampleModule.hx",
                    "src\\com\\module\\test\\ITestModule.hx",
                    "src\\com\\module\\test\\TestModule.hx"
                });

                var results = SearchUtil.getMatchedItems(files, "test\\test", '\\', 0);

                //"test\\TestModule.hx" should be first, because it has test in the path and also in the beginning of the file name
                Assert.AreEqual("src\\com\\module\\test\\TestModule.hx", results[0]);
                Assert.AreEqual("src\\com\\module\\test\\ITestModule.hx", results[1]);
            }
        }
    }
}
