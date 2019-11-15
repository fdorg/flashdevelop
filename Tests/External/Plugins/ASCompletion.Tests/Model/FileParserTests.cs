// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using ASCompletion.Context;
using ASCompletion.TestUtils;
using NUnit.Framework;

namespace ASCompletion.Model
{
    [TestFixture]
    class FileParserTests : ASCompletionTests
    {
        [TestFixtureSetUp]
        public void Setup() => SetAs3Features(sci);

        static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

        static string GetFullPath(string fileName) => $"ASCompletion.Test_Files.parser.{fileName}.as";

        [Test]
        public void ParseFile_SimpleClass()
        {
            var model = ASContext.Context.GetCodeModel(ReadAllText("SimpleClassTest"));
            Assert.AreEqual(3, model.Version);
            Assert.IsTrue(model.HasPackage);
            Assert.AreEqual("test.test", model.Package);
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

        [Test(Description = "Commit 7c8718c")]
        public void ParseFile_OverrideFunction()
        {
            var model = ASContext.Context.GetCodeModel(ReadAllText("OverrideFunctionTest"));
            var classModel = model.Classes[0];
            Assert.AreEqual(2, classModel.Members.Count);
            var memberModel = classModel.Members[0];
            Assert.AreEqual("test1", memberModel.Name);
            var expectedFlags = FlagType.Function | FlagType.Override;
            Assert.AreEqual(expectedFlags, memberModel.Flags & expectedFlags);
            Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
            Assert.AreEqual(4, memberModel.LineFrom);
            Assert.AreEqual(7, memberModel.LineTo);
            memberModel = classModel.Members[1];
            Assert.AreEqual("test2", memberModel.Name);
            Assert.AreEqual(expectedFlags, memberModel.Flags & expectedFlags);
            Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
            Assert.AreEqual(9, memberModel.LineFrom);
            Assert.AreEqual(12, memberModel.LineTo);
        }

        [Test(Description = "Error #617")]
        public void ParseFile_CompletionError()
        {
            var model = ASContext.Context.GetCodeModel(ReadAllText("CompletionErrorTest"), true);

            Assert.AreEqual(2, model.Members.Count); // First member = function itself

            var funcMember = model.Members[0];
            Assert.AreEqual("init", funcMember.Name);
            Assert.AreEqual("void", funcMember.Type);
            Assert.AreEqual(FlagType.Function, funcMember.Flags & FlagType.Function);
            Assert.AreEqual("args", funcMember.Parameters[0].Name);
            Assert.AreEqual("String", funcMember.Parameters[0].Type);

            var infoMember = model.Members[1];
            Assert.AreEqual("info", infoMember.Name);
            Assert.AreEqual("NativeProcessStartupInfo", infoMember.Type);
            Assert.AreEqual(FlagType.Variable, infoMember.Flags & FlagType.Variable);
        }

        [Test]
        public void ParseFile_IdentifiersWithUnicodeChars()
        {
            var model = ASContext.Context.GetCodeModel(ReadAllText("IdentifiersWithUnicodeCharsTest"));
            var classModel = model.Classes[0];
            Assert.AreEqual("Test", classModel.Name);
            Assert.AreEqual(FlagType.Class, classModel.Flags & FlagType.Class);
            Assert.AreEqual(2, classModel.LineFrom);
            Assert.AreEqual(9, classModel.LineTo);
            Assert.AreEqual(2, classModel.Members.Count);

            var memberModel = classModel.Members[0];
            Assert.AreEqual("thísIsVälid", memberModel.Name);
            Assert.AreEqual("String", memberModel.Type);
            Assert.AreEqual(FlagType.Function, memberModel.Flags & FlagType.Function);
            Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
            Assert.AreEqual(4, memberModel.LineFrom);
            Assert.AreEqual(6, memberModel.LineTo);

            memberModel = classModel.Members[1];
            Assert.AreEqual("日本語文字ヴァリアブル", memberModel.Name);
            Assert.AreEqual("Dynamic", memberModel.Type);
            Assert.AreEqual(FlagType.Variable, memberModel.Flags & FlagType.Variable);
            Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
            Assert.AreEqual(8, memberModel.LineFrom);
            Assert.AreEqual(8, memberModel.LineTo);
        }

        [Test(Description = "Parse Vector.<*>. https://github.com/fdorg/flashdevelop/issues/1383")]
        public void ParseFile_Issue1383()
        {
            Assert.AreEqual("Vector.<*>", ASContext.Context.GetCodeModel("var v:Vector.<*>;").Members[0].Type);
            Assert.AreEqual("Vector.<*>", ASContext.Context.GetCodeModel("public var v:Vector.<*> = new Vector.<*>();").Members[0].Type);
        }
    }
}