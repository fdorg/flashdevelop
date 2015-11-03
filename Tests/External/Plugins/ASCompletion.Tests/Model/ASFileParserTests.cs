using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ASCompletion.TestUtils;
using NUnit.Framework;

namespace ASCompletion.Model
{
    [TestFixture]
    class ASFileParserTests
    {

        [Test]
        public void TestParseFile_SimpleClass()
        {
            using (var resourceFile = new TestFile("ASCompletion.Test_Files.Test.as"))
            {
                var srcModel = new FileModel(resourceFile.DestinationFile);
                var model = ASFileParser.ParseFile(srcModel);
                Assert.AreEqual(3, model.Version);
                Assert.IsTrue(model.HasPackage);
                Assert.AreEqual("test.test", model.FullPackage);
                Assert.AreEqual(1, model.Classes.Count);
                var classModel = model.Classes[0];
                Assert.AreEqual("Test", classModel.Name);
                Assert.AreEqual("test.test.Test", classModel.QualifiedName);
                Assert.AreEqual(FlagType.Class, classModel.Flags & FlagType.Class);
                Assert.AreEqual(Visibility.Public, classModel.Access & Visibility.Public);
                Assert.AreEqual(2, classModel.LineFrom);
                Assert.AreEqual(7, classModel.LineTo);
                Assert.AreEqual(1, classModel.Members.Count);
                var memberModel = classModel.Members[0];
                Assert.AreEqual("Test", memberModel.Name);
                Assert.AreEqual(FlagType.Function, memberModel.Flags & FlagType.Function);
                Assert.AreEqual(FlagType.Constructor, memberModel.Flags & FlagType.Constructor);
                Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
                Assert.AreEqual(4, memberModel.LineFrom);
                Assert.AreEqual(6, memberModel.LineTo);
            }
        }
    }
}
