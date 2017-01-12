using System.Collections.Generic;
using FlashDevelop;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using ScintillaNet;
using ScintillaNet.Enums;

namespace ProjectManager.Controls
{
    [TestFixture]
    public class OpenResourceFormTests
    {
        // TODO: Add more tests!
        public class SearchUtilTests : OpenResourceFormTests
        {
            [Test]
            public void SimpleTest()
            {
                List<string> files = new List<string>();
                files.AddRange(new string[] {
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
                var results = SearchUtil.getMatchedItems(files, "m", "\\", 1);
                
                Assert.AreEqual("src\\Main.hx", results[0]); //shortest and also starts with an "m", so should be first
            }

            [Test]
            public void LimitTest()
            {
                List<string> files = new List<string>();
                files.AddRange(new string[] {
                    "Main.hx",
                    "src\\com\\module\\example\\ExampleModule.hx",
                    "src\\com\\module\\example\\IExampleModule.hx",
                    "src\\com\\module\\test\\ITestModule.hx",
                    "src\\com\\module\\test\\TestModule.hx"
                });
                var results = SearchUtil.getMatchedItems(files, "m", "\\", 1);
                Assert.AreEqual(results.Count, 1);

                results = SearchUtil.getMatchedItems(files, "m", "\\", 1);
                Assert.Greater(results.Count, 0);
            }

            [Test]
            public void IClassTest()
            {
                List<string> files = new List<string>();
                files.AddRange(new string[] {
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
                var results = SearchUtil.getMatchedItems(files, "iclass", "\\", 2);

                //since the file name starts with iclass, it should be prefered over "src\\StaticClass.hx", which has 
                Assert.AreEqual("hexannotation\\hex\\annotation\\IClassAnnotationDataProvider.hx", results[0]);
                
                //however, "src\\StaticClass.hx" should be second, because it is only slightly off.
                Assert.AreEqual("src\\StaticClass.hx", results[1]);
            }

            [Test]
            public void TestTest()
            {
                List<string> files = new List<string>();
                files.AddRange(new string[] {
                    "src\\StaticClass.hx",
                    "src\\com\\module\\ITestModule.hx",
                    "src\\com\\module\\TestModule.hx",
                    "src\\com\\module\\example\\ExampleModule.hx",
                    "src\\com\\module\\example\\IExampleModule.hx",
                    "src\\com\\module\\test\\ITestModule.hx",
                    "src\\com\\module\\test\\TestModule.hx"
                });

                var results = SearchUtil.getMatchedItems(files, "test", "\\", 0);

                //"test\\TestModule.hx" should be first, because it has test in the path too
                Assert.AreEqual("src\\com\\module\\test\\TestModule.hx", results[0]);

                //then "test\\TestModule.hx" for the same reason, but only second, because it is longer.
                Assert.AreEqual("src\\com\\module\\test\\ITestModule.hx", results[1]);

                //then "TestModule.hx"
                Assert.AreEqual("src\\com\\module\\test\\ITestModule.hx", results[2]);

                //then "ITestModule.hx"
                Assert.AreEqual("src\\com\\module\\test\\ITestModule.hx", results[3]);

            }

            [Test]
            public void ExactTest()
            {
                List<string> files = new List<string>();
                files.AddRange(new string[] {
                    "src\\StaticClass.hx",
                    "src\\com\\module\\ITestModule.hx",
                    "src\\com\\module\\TestModule.hx",
                    "src\\com\\module\\example\\ExampleModule.hx",
                    "src\\com\\module\\example\\IExampleModule.hx",
                    "src\\com\\module\\test\\ITestModule.hx",
                    "src\\com\\module\\test\\TestModule.hx"
                });

                var excpected = new string[]
                {
                    "src\\com\\module\\TestModule.hx",
                    "src\\com\\module\\ITestModule.hx",
                    "src\\com\\module\\test\\TestModule.hx",
                    "src\\com\\module\\test\\ITestModule.hx",
                    "src\\com\\module\\example\\ExampleModule.hx",
                    "src\\com\\module\\example\\IExampleModule.hx",
                };
                var results = SearchUtil.getMatchedItems(files, "src\\com\\module", "\\", 0);

                CollectionAssert.AreEqual(excpected, results);

            }

        }
    }
}
