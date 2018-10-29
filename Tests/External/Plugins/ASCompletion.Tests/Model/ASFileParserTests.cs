using System.Collections.Generic;
using System.Linq;
using ASCompletion.Context;
using ASCompletion.TestUtils;
using NUnit.Framework;

namespace ASCompletion.Model
{
    using MemberWithType = KeyValuePair<MemberModel, string>;

    class ASFileParserTests
    {
        [TestFixture]
        public class As3 : ASCompletionTests
        {
            [TestFixtureSetUp]
            public void Setup() => SetAs3Features(sci);

            protected static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

            protected static string GetFullPath(string fileName) => $"ASCompletion.Test_Files.parser.as3.{fileName}.as";

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

        [TestFixture]
        public class Haxe : ASCompletionTests
        {
            [TestFixtureSetUp]
            public void Setup() => SetHaxeFeatures(sci);

            protected static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

            protected static string GetFullPath(string fileName) => $"ASCompletion.Test_Files.parser.haxe.{fileName}.hx";

            [Test]
            public void ParseFile_SimpleClass()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("SimpleClassTest"));
                Assert.AreEqual(4, model.Version);
                Assert.IsTrue(model.HasPackage);
                Assert.AreEqual("test.test", model.Package);
                Assert.AreEqual(1, model.Classes.Count);
                var classModel = model.Classes[0];
                Assert.AreEqual("Test", classModel.Name);
                Assert.AreEqual(FlagType.Class, classModel.Flags & FlagType.Class);
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

            [Test]
            public void ParseFile_PrivateClass()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("PrivateClassTest"));
                Assert.AreEqual(4, model.Version);
                Assert.IsTrue(model.HasPackage);
                Assert.AreEqual("test.test", model.Package);
                Assert.AreEqual(2, model.Classes.Count);

                var classModel = model.Classes[0];
                Assert.AreEqual("Test", classModel.Name);
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

                classModel = model.Classes[1];
                Assert.AreEqual("TestPrivate", classModel.Name);
                Assert.AreEqual(FlagType.Class, classModel.Flags & FlagType.Class);
                Assert.AreEqual(Visibility.Private, classModel.Access & Visibility.Private);
                Assert.AreEqual(9, classModel.LineFrom);
                Assert.AreEqual(14, classModel.LineTo);
                Assert.AreEqual(1, classModel.Members.Count);
                memberModel = classModel.Members[0];
                Assert.AreEqual("TestPrivate", memberModel.Name);
                Assert.AreEqual(FlagType.Function, memberModel.Flags & FlagType.Function);
                Assert.AreEqual(FlagType.Constructor, memberModel.Flags & FlagType.Constructor);
                Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
                Assert.AreEqual(11, memberModel.LineFrom);
                Assert.AreEqual(13, memberModel.LineTo);
            }

            [Test]
            public void ParseFile_Interface()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("InterfaceTest"));

                Assert.AreEqual(1, model.Classes.Count);
                var classModel = model.Classes[0];
                Assert.AreEqual("Test", classModel.Name);
                Assert.AreEqual(Visibility.Public, classModel.Access);
                Assert.AreEqual(FlagType.Interface, classModel.Flags & FlagType.Interface);
                Assert.AreEqual(5, classModel.Members.Count);

                var member = classModel.Members[0];
                Assert.AreEqual("testVar", member.Name);
                Assert.AreEqual("String", member.Type);
                Assert.IsNull(member.Parameters);
                Assert.AreEqual(Visibility.Public, member.Access);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);

                member = classModel.Members[1];
                Assert.AreEqual("test", member.Name);
                Assert.AreEqual("Int", member.Type);
                Assert.AreEqual(1, member.Parameters.Count);
                Assert.AreEqual("?arg", member.Parameters[0].Name);
                Assert.AreEqual("Array<Dynamic>", member.Parameters[0].Type);
                Assert.AreEqual(Visibility.Public, member.Access);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);

                member = classModel.Members[2];
                Assert.AreEqual("test2", member.Name);
                Assert.AreEqual("Void", member.Type);
                Assert.AreEqual(1, member.Parameters.Count);
                Assert.AreEqual("arg", member.Parameters[0].Name);
                Assert.AreEqual("Bool", member.Parameters[0].Type);
                Assert.AreEqual(1, member.Parameters.Count);
                Assert.AreEqual(Visibility.Public, member.Access);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);

                member = classModel.Members[3];
                Assert.AreEqual("testPrivate", member.Name);
                Assert.AreEqual("Int", member.Type);
                Assert.IsNull(member.Parameters);
                Assert.AreEqual(Visibility.Private, member.Access);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);

                member = classModel.Members[4];
                Assert.AreEqual("testProperty", member.Name);
                Assert.AreEqual("Float", member.Type);
                Assert.AreEqual(2, member.Parameters.Count);
                Assert.AreEqual(Visibility.Public, member.Access);
                Assert.AreEqual(FlagType.Getter, member.Flags & FlagType.Getter);
                Assert.AreEqual("get", member.Parameters[0].Name);
                Assert.AreEqual("set", member.Parameters[1].Name);
            }

            [Test]
            public void ParseFile_ClassImplements()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("ImplementClassTest"));

                Assert.AreEqual(2, model.Classes.Count);
                var classModel = model.Classes[0];
                Assert.AreEqual("Test", classModel.Name);
                Assert.AreEqual(Visibility.Public, classModel.Access);
                Assert.AreEqual(FlagType.Class, classModel.Flags & FlagType.Class);
                Assert.That(classModel.Implements, Is.EquivalentTo(new[] {"ITest"}));

                classModel = model.Classes[1];
                Assert.AreEqual("MultipleTest", classModel.Name);
                Assert.AreEqual(Visibility.Public, classModel.Access);
                Assert.AreEqual(FlagType.Class, classModel.Flags & FlagType.Class);
                Assert.That(classModel.Implements, Is.EquivalentTo(new[] {"ITest", "ITest2", "ITest3"}));
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

            [Test(Description = "Includes Commit 304ca93")]
            public void ParseFile_TypeDefs()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("TypeDefsTest"));
                Assert.AreEqual(8, model.Classes.Count);

                var aliasTypeDef = model.Classes[0];
                Assert.AreEqual(2, aliasTypeDef.LineFrom);
                Assert.AreEqual(2, aliasTypeDef.LineTo);
                Assert.AreEqual(FlagType.TypeDef, aliasTypeDef.Flags & FlagType.TypeDef);
                Assert.AreEqual("Array<Int>", aliasTypeDef.ExtendsType);
                Assert.AreEqual("Alias", aliasTypeDef.FullName);

                var iterableTypeDef = model.Classes[1];
                Assert.AreEqual(4, iterableTypeDef.LineFrom);
                Assert.AreEqual(6, iterableTypeDef.LineTo);
                Assert.AreEqual(FlagType.TypeDef, iterableTypeDef.Flags & FlagType.TypeDef);
                Assert.AreEqual("Iterable<T>", iterableTypeDef.FullName);
                Assert.AreEqual("Iterable", iterableTypeDef.Name);
                Assert.AreEqual("<T>", iterableTypeDef.Template);
                Assert.AreEqual(1, iterableTypeDef.Members.Count);
                var member = iterableTypeDef.Members[0];
                Assert.AreEqual("iterator", member.Name);
                Assert.AreEqual(5, member.LineFrom);
                Assert.AreEqual(5, member.LineTo);
                Assert.AreEqual("Iterator<T>", member.Type);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);

                var typeWithOptionalATypeDef = model.Classes[2];
                Assert.AreEqual(8, typeWithOptionalATypeDef.LineFrom);
                Assert.AreEqual(12, typeWithOptionalATypeDef.LineTo);
                Assert.AreEqual(FlagType.TypeDef, typeWithOptionalATypeDef.Flags & FlagType.TypeDef);
                Assert.AreEqual("TypeWithOptionalA", typeWithOptionalATypeDef.FullName);
                Assert.AreEqual(3, typeWithOptionalATypeDef.Members.Count);
                member = typeWithOptionalATypeDef.Members[0];
                Assert.AreEqual("age", member.Name);
                Assert.AreEqual(9, member.LineFrom);
                Assert.AreEqual(9, member.LineTo);
                Assert.AreEqual("Int", member.Type);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                member = typeWithOptionalATypeDef.Members[1];
                Assert.AreEqual("name", member.Name);
                Assert.AreEqual(10, member.LineFrom);
                Assert.AreEqual(10, member.LineTo);
                Assert.AreEqual("String", member.Type);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                member = typeWithOptionalATypeDef.Members[2];
                Assert.AreEqual("phoneNumber", member.Name);
                Assert.AreEqual(11, member.LineFrom);
                Assert.AreEqual(11, member.LineTo);
                Assert.AreEqual("String", member.Type);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);

                var typeWithOptionalBTypeDef = model.Classes[3];
                Assert.AreEqual(14, typeWithOptionalBTypeDef.LineFrom);
                Assert.AreEqual(17, typeWithOptionalBTypeDef.LineTo);
                Assert.AreEqual(FlagType.TypeDef, typeWithOptionalBTypeDef.Flags & FlagType.TypeDef);
                Assert.AreEqual("TypeWithOptionalB", typeWithOptionalBTypeDef.FullName);
                Assert.AreEqual(2, typeWithOptionalBTypeDef.Members.Count);
                member = typeWithOptionalBTypeDef.Members[0];
                Assert.AreEqual("optionalString", member.Name);
                Assert.AreEqual(15, member.LineFrom);
                Assert.AreEqual(15, member.LineTo);
                Assert.AreEqual("String", member.Type);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                member = typeWithOptionalBTypeDef.Members[1];
                Assert.AreEqual("requiredInt", member.Name);
                Assert.AreEqual(16, member.LineFrom);
                Assert.AreEqual(16, member.LineTo);
                Assert.AreEqual("Int", member.Type);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);

                var singleLineTypeDef = model.Classes[4];
                Assert.AreEqual(19, singleLineTypeDef.LineFrom);
                Assert.AreEqual(19, singleLineTypeDef.LineTo);
                Assert.AreEqual(FlagType.TypeDef, singleLineTypeDef.Flags & FlagType.TypeDef);
                Assert.AreEqual("SingleLine", singleLineTypeDef.FullName);
                Assert.AreEqual(2, singleLineTypeDef.Members.Count);
                member = singleLineTypeDef.Members[0];
                Assert.AreEqual("x", member.Name);
                Assert.AreEqual(19, member.LineFrom);
                Assert.AreEqual(19, member.LineTo);
                Assert.AreEqual("Int", member.Type);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                member = singleLineTypeDef.Members[1];
                Assert.AreEqual("y", member.Name);
                Assert.AreEqual(19, member.LineFrom);
                Assert.AreEqual(19, member.LineTo);
                Assert.AreEqual("Int", member.Type);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);

                var normalDefTypeDef = model.Classes[5];
                Assert.AreEqual(21, normalDefTypeDef.LineFrom);
                Assert.AreEqual(24, normalDefTypeDef.LineTo);
                Assert.AreEqual(FlagType.TypeDef, normalDefTypeDef.Flags & FlagType.TypeDef);
                Assert.AreEqual("NormalDef", normalDefTypeDef.FullName);
                Assert.AreEqual(2, normalDefTypeDef.Members.Count);
                member = normalDefTypeDef.Members[0];
                Assert.AreEqual("aliases", member.Name);
                Assert.AreEqual(22, member.LineFrom);
                Assert.AreEqual(22, member.LineTo);
                Assert.AreEqual("Array<String>", member.Type);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                member = normalDefTypeDef.Members[1];
                Assert.AreEqual("processFunction", member.Name);
                Assert.AreEqual(23, member.LineFrom);
                Assert.AreEqual(23, member.LineTo);
                Assert.AreEqual("Dynamic", member.Type);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);

                var shortDefTypeDef = model.Classes[6];
                Assert.AreEqual(26, shortDefTypeDef.LineFrom);
                Assert.AreEqual(29, shortDefTypeDef.LineTo);
                Assert.AreEqual(FlagType.TypeDef, shortDefTypeDef.Flags & FlagType.TypeDef);
                Assert.AreEqual("ShortDef", shortDefTypeDef.FullName);
                Assert.AreEqual(2, shortDefTypeDef.Members.Count);
                member = shortDefTypeDef.Members[0];
                Assert.AreEqual("aliases", member.Name);
                Assert.AreEqual(27, member.LineFrom);
                Assert.AreEqual(27, member.LineTo);
                Assert.AreEqual("Array<String>", member.Type);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                member = shortDefTypeDef.Members[1];
                Assert.AreEqual("processFunction", member.Name);
                Assert.AreEqual(28, member.LineFrom);
                Assert.AreEqual(28, member.LineTo);
                Assert.AreEqual("Dynamic", member.Type);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);

                //issue: https://github.com/fdorg/flashdevelop/issues/2209
                var typedef = model.Classes[7];
                Assert.AreEqual("ShortDef", typedef.ExtendsType);
                Assert.AreEqual(1, typedef.Members.Count);
            }

            [Test]
            public void ParseFile_Enums()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("EnumsTest"));
                Assert.AreEqual(2, model.Classes.Count);

                var simpleEnum = model.Classes[0];
                Assert.AreEqual("SimpleEnum", simpleEnum.Name);
                Assert.AreEqual(2, simpleEnum.LineFrom);
                Assert.AreEqual(6, simpleEnum.LineTo);
                Assert.AreEqual(FlagType.Enum, simpleEnum.Flags & FlagType.Enum);
                Assert.AreEqual(3, simpleEnum.Members.Count);
                var member = simpleEnum.Members[0];
                Assert.AreEqual("Foo", member.Name);
                Assert.AreEqual(3, member.LineFrom);
                Assert.AreEqual(3, member.LineTo);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                member = simpleEnum.Members[1];
                Assert.AreEqual("Bar", member.Name);
                Assert.AreEqual(4, member.LineFrom);
                Assert.AreEqual(4, member.LineTo);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                member = simpleEnum.Members[2];
                Assert.AreEqual("Baz", member.Name);
                Assert.AreEqual(5, member.LineFrom);
                Assert.AreEqual(5, member.LineTo);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);

                var complexEnum = model.Classes[1];
                Assert.AreEqual("ComplexEnum", complexEnum.Name);
                Assert.AreEqual(8, complexEnum.LineFrom);
                Assert.AreEqual(11, complexEnum.LineTo);
                Assert.AreEqual(FlagType.Enum, complexEnum.Flags & FlagType.Enum);
                Assert.AreEqual(2, complexEnum.Members.Count);
                member = complexEnum.Members[0];
                Assert.AreEqual("IntEnum", member.Name);
                Assert.AreEqual(9, member.LineFrom);
                Assert.AreEqual(9, member.LineTo);
                Assert.AreEqual("i", member.Parameters[0].Name);
                Assert.AreEqual("Int", member.Parameters[0].Type);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                member = complexEnum.Members[1];
                Assert.AreEqual("MultiEnum", member.Name);
                Assert.AreEqual(10, member.LineFrom);
                Assert.AreEqual(10, member.LineTo);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                Assert.AreEqual("i", member.Parameters[0].Name);
                Assert.AreEqual("Int", member.Parameters[0].Type);
                Assert.AreEqual("j", member.Parameters[1].Name);
                Assert.AreEqual("String", member.Parameters[1].Type);
                Assert.AreEqual("k", member.Parameters[2].Name);
                Assert.AreEqual("Float", member.Parameters[2].Type);
            }

            [Test]
            public void ParseFile_Abstracts()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("AbstractsTest"));
                Assert.AreEqual(2, model.Classes.Count);

                var plainAbstract = model.Classes[0];
                Assert.AreEqual("AbstractInt", plainAbstract.Name);
                Assert.AreEqual("Int", plainAbstract.ExtendsType);
                Assert.AreEqual(2, plainAbstract.LineFrom);
                Assert.AreEqual(6, plainAbstract.LineTo);
                Assert.AreEqual(FlagType.Abstract, plainAbstract.Flags & FlagType.Abstract);
                Assert.AreEqual(1, plainAbstract.Members.Count);
                var member = plainAbstract.Members[0];
                Assert.AreEqual(3, member.LineFrom);
                Assert.AreEqual(5, member.LineTo);
                Assert.AreEqual("AbstractInt", member.Name);
                Assert.AreEqual(FlagType.Constructor, member.Flags & FlagType.Constructor);
                Assert.AreEqual(1, member.Parameters.Count);
                Assert.AreEqual("i", member.Parameters[0].Name);
                Assert.AreEqual("Int", member.Parameters[0].Type);

                var implicitCastAbstract = model.Classes[1];
                Assert.AreEqual("MyAbstract", implicitCastAbstract.Name);
                Assert.AreEqual("Int", implicitCastAbstract.ExtendsType);
                Assert.AreEqual(8, implicitCastAbstract.LineFrom);
                Assert.AreEqual(12, implicitCastAbstract.LineTo);
                Assert.AreEqual(FlagType.Abstract, implicitCastAbstract.Flags & FlagType.Abstract);
                Assert.AreEqual(1, implicitCastAbstract.Members.Count);
                member = implicitCastAbstract.Members[0];
                Assert.AreEqual(9, member.LineFrom);
                Assert.AreEqual(11, member.LineTo);
                Assert.AreEqual("MyAbstract", member.Name);
                Assert.AreEqual(FlagType.Constructor, member.Flags & FlagType.Constructor);
                Assert.AreEqual(1, member.Parameters.Count);
                Assert.AreEqual("i", member.Parameters[0].Name);
                Assert.AreEqual("Int", member.Parameters[0].Type);
                Assert.IsNotNull(implicitCastAbstract.MetaDatas);
                Assert.IsNotEmpty(implicitCastAbstract.MetaDatas);
                Assert.AreEqual("Int", implicitCastAbstract.MetaDatas.Find(it => it.Name == "from").RawParams);
                Assert.AreEqual("Int", implicitCastAbstract.MetaDatas.Find(it => it.Name == "to").RawParams);
            }

            [Test]
            public void ParseFile_AbstractsWithMeta()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("AbstractsTest2"));
                var classModel = model.Classes.First();
                Assert.IsNotNull(classModel.MetaDatas);
                Assert.AreEqual(2, classModel.MetaDatas.Count);
                Assert.IsTrue(classModel.MetaDatas.Any(it => it.Name == ":forwardStatic"));
                Assert.AreEqual("length, get, set", classModel.MetaDatas.Find(it => it.Name == ":forward").Params["Default"]);
            }

            [Test(Description = "Includes Commit 51938e0")]
            public void ParseFile_Generics()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("GenericsTest"));
                Assert.AreEqual(4, model.Classes.Count);

                var simpleGeneric = model.Classes[0];
                Assert.AreEqual(2, simpleGeneric.LineFrom);
                Assert.AreEqual(11, simpleGeneric.LineTo);
                Assert.AreEqual(FlagType.Class, simpleGeneric.Flags & FlagType.Class);
                Assert.AreEqual("Test<T>", simpleGeneric.FullName);
                Assert.AreEqual("Test", simpleGeneric.Name);
                Assert.AreEqual("<T>", simpleGeneric.Template);
                Assert.AreEqual(2, simpleGeneric.Members.Count);
                var member = simpleGeneric.Members[0];
                Assert.AreEqual("test1", member.Name);
                Assert.AreEqual(4, member.LineFrom);
                Assert.AreEqual(6, member.LineTo);
                Assert.AreEqual("T", member.Type);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                Assert.AreEqual(2, member.Parameters.Count);
                var arg = member.Parameters[0];
                Assert.AreEqual("expected", arg.Name);
                Assert.AreEqual("T", arg.Type);
                arg = member.Parameters[1];
                Assert.AreEqual("actual", arg.Name);
                Assert.AreEqual("T", arg.Type);
                member = simpleGeneric.Members[1];
                Assert.AreEqual("test2<K>", member.FullName);
                Assert.AreEqual("test2", member.Name);
                Assert.AreEqual("<K>", member.Template);
                Assert.AreEqual(8, member.LineFrom);
                Assert.AreEqual(10, member.LineTo);
                Assert.AreEqual("K", member.Type);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                arg = member.Parameters[0];
                Assert.AreEqual("expected", arg.Name);
                Assert.AreEqual("K", arg.Type);
                arg = member.Parameters[1];
                Assert.AreEqual("actual", arg.Name);
                Assert.AreEqual("K", arg.Type);

                var constraintGeneric = model.Classes[1];
                Assert.AreEqual(13, constraintGeneric.LineFrom);
                Assert.AreEqual(22, constraintGeneric.LineTo);
                Assert.AreEqual(FlagType.Class, constraintGeneric.Flags & FlagType.Class);
                Assert.AreEqual("TestConstraint<T:Iterable<String>>", constraintGeneric.FullName);
                Assert.AreEqual("TestConstraint", constraintGeneric.Name);
                Assert.AreEqual("<T:Iterable<String>>", constraintGeneric.Template);
                Assert.AreEqual(2, constraintGeneric.Members.Count);
                member = constraintGeneric.Members[0];
                Assert.AreEqual("test1", member.Name);
                Assert.AreEqual(15, member.LineFrom);
                Assert.AreEqual(17, member.LineTo);
                Assert.AreEqual("T", member.Type);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                Assert.AreEqual(2, member.Parameters.Count);
                arg = member.Parameters[0];
                Assert.AreEqual("expected", arg.Name);
                Assert.AreEqual("T", arg.Type);
                arg = member.Parameters[1];
                Assert.AreEqual("actual", arg.Name);
                Assert.AreEqual("T", arg.Type);
                member = constraintGeneric.Members[1];
                Assert.AreEqual("test2<K:Iterable<String>>", member.FullName);
                Assert.AreEqual("test2", member.Name);
                Assert.AreEqual("<K:Iterable<String>>", member.Template);
                Assert.AreEqual(19, member.LineFrom);
                Assert.AreEqual(21, member.LineTo);
                Assert.AreEqual("K", member.Type);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                arg = member.Parameters[0];
                Assert.AreEqual("expected", arg.Name);
                Assert.AreEqual("K", arg.Type);
                arg = member.Parameters[1];
                Assert.AreEqual("actual", arg.Name);
                Assert.AreEqual("K", arg.Type);

                var multipleConstraintGeneric = model.Classes[2];
                Assert.AreEqual(24, multipleConstraintGeneric.LineFrom);
                Assert.AreEqual(33, multipleConstraintGeneric.LineTo);
                Assert.AreEqual(FlagType.Class, multipleConstraintGeneric.Flags & FlagType.Class);
                Assert.AreEqual("TestMultiple<T:(Iterable<String>,Measurable)>", multipleConstraintGeneric.FullName);
                Assert.AreEqual("TestMultiple", multipleConstraintGeneric.Name);
                Assert.AreEqual("<T:(Iterable<String>,Measurable)>", multipleConstraintGeneric.Template);
                Assert.AreEqual(2, multipleConstraintGeneric.Members.Count);
                member = multipleConstraintGeneric.Members[0];
                Assert.AreEqual("test1", member.Name);
                Assert.AreEqual(26, member.LineFrom);
                Assert.AreEqual(28, member.LineTo);
                Assert.AreEqual("T", member.Type);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                Assert.AreEqual(2, member.Parameters.Count);
                arg = member.Parameters[0];
                Assert.AreEqual("expected", arg.Name);
                Assert.AreEqual("T", arg.Type);
                arg = member.Parameters[1];
                Assert.AreEqual("actual", arg.Name);
                Assert.AreEqual("T", arg.Type);
                member = multipleConstraintGeneric.Members[1];
                Assert.AreEqual("test2<K:(Iterable<String>,Measurable)>", member.FullName);
                Assert.AreEqual("test2", member.Name);
                Assert.AreEqual("<K:(Iterable<String>,Measurable)>", member.Template);
                Assert.AreEqual(30, member.LineFrom);
                Assert.AreEqual(32, member.LineTo);
                Assert.AreEqual("K", member.Type);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                arg = member.Parameters[0];
                Assert.AreEqual("expected", arg.Name);
                Assert.AreEqual("K", arg.Type);
                arg = member.Parameters[1];
                Assert.AreEqual("actual", arg.Name);
                Assert.AreEqual("K", arg.Type);

                var complexConstraintGeneric = model.Classes[3];
                Assert.AreEqual(35, complexConstraintGeneric.LineFrom);
                Assert.AreEqual(44, complexConstraintGeneric.LineTo);
                Assert.AreEqual(FlagType.Class, complexConstraintGeneric.Flags & FlagType.Class);
                Assert.AreEqual("TestFullConstraint<T:Measurable,Z:(Iterable<String>,Measurable)>", complexConstraintGeneric.FullName);
                Assert.AreEqual("TestFullConstraint", complexConstraintGeneric.Name);
                Assert.AreEqual("<T:Measurable,Z:(Iterable<String>,Measurable)>", complexConstraintGeneric.Template);
                Assert.AreEqual(2, complexConstraintGeneric.Members.Count);
                member = complexConstraintGeneric.Members[0];
                Assert.AreEqual("test1", member.Name);
                Assert.AreEqual(37, member.LineFrom);
                Assert.AreEqual(39, member.LineTo);
                Assert.AreEqual("T", member.Type);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                Assert.AreEqual(2, member.Parameters.Count);
                arg = member.Parameters[0];
                Assert.AreEqual("expected", arg.Name);
                Assert.AreEqual("T", arg.Type);
                arg = member.Parameters[1];
                Assert.AreEqual("actual", arg.Name);
                Assert.AreEqual("Z", arg.Type);
                member = complexConstraintGeneric.Members[1];
                Assert.AreEqual("test2<K:Measurable,V:(Iterable<String>,Measurable)>", member.FullName);
                Assert.AreEqual("test2", member.Name);
                Assert.AreEqual("<K:Measurable,V:(Iterable<String>,Measurable)>", member.Template);
                Assert.AreEqual(41, member.LineFrom);
                Assert.AreEqual(43, member.LineTo);
                Assert.AreEqual("K", member.Type);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                arg = member.Parameters[0];
                Assert.AreEqual("expected", arg.Name);
                Assert.AreEqual("K", arg.Type);
                arg = member.Parameters[1];
                Assert.AreEqual("actual", arg.Name);
                Assert.AreEqual("V", arg.Type);
            }

            [Test]
            public void ParseFile_GenericsWithObjectConstraints()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("GenericsObjectConstraintTest"));
                Assert.AreEqual(3, model.Classes.Count);

                var objectConstraintGeneric = model.Classes[0];
                Assert.AreEqual(2, objectConstraintGeneric.LineFrom);
                Assert.AreEqual(11, objectConstraintGeneric.LineTo);
                Assert.AreEqual(FlagType.Class, objectConstraintGeneric.Flags & FlagType.Class);
                Assert.AreEqual("TestObjectConstraint<T:({},Measurable)>", objectConstraintGeneric.FullName);
                Assert.AreEqual("TestObjectConstraint", objectConstraintGeneric.Name);
                Assert.AreEqual("<T:({},Measurable)>", objectConstraintGeneric.Template);
                Assert.AreEqual(2, objectConstraintGeneric.Members.Count);
                var member = objectConstraintGeneric.Members[0];
                Assert.AreEqual("test1", member.Name);
                Assert.AreEqual(4, member.LineFrom);
                Assert.AreEqual(6, member.LineTo);
                Assert.AreEqual("T", member.Type);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                Assert.AreEqual(2, member.Parameters.Count);
                var arg = member.Parameters[0];
                Assert.AreEqual("expected", arg.Name);
                Assert.AreEqual("T", arg.Type);
                arg = member.Parameters[1];
                Assert.AreEqual("actual", arg.Name);
                Assert.AreEqual("T", arg.Type);
                member = objectConstraintGeneric.Members[1];
                Assert.AreEqual("test2<K:({},Measurable)>", member.FullName);
                Assert.AreEqual("test2", member.Name);
                Assert.AreEqual("<K:({},Measurable)>", member.Template);
                Assert.AreEqual(8, member.LineFrom);
                Assert.AreEqual(10, member.LineTo);
                Assert.AreEqual("K", member.Type);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                arg = member.Parameters[0];
                Assert.AreEqual("expected", arg.Name);
                Assert.AreEqual("K", arg.Type);
                arg = member.Parameters[1];
                Assert.AreEqual("actual", arg.Name);
                Assert.AreEqual("K", arg.Type);

                var fullConstraintGeneric = model.Classes[1];
                Assert.AreEqual(13, fullConstraintGeneric.LineFrom);
                Assert.AreEqual(22, fullConstraintGeneric.LineTo);
                Assert.AreEqual(FlagType.Class, fullConstraintGeneric.Flags & FlagType.Class);
                Assert.AreEqual("TestFullConstraint<T:({},Measurable),Z:(Iterable<String>,Measurable)>", fullConstraintGeneric.FullName);
                Assert.AreEqual("TestFullConstraint", fullConstraintGeneric.Name);
                Assert.AreEqual("<T:({},Measurable),Z:(Iterable<String>,Measurable)>", fullConstraintGeneric.Template);
                Assert.AreEqual(2, fullConstraintGeneric.Members.Count);
                member = fullConstraintGeneric.Members[0];
                Assert.AreEqual("test1", member.Name);
                Assert.AreEqual(15, member.LineFrom);
                Assert.AreEqual(17, member.LineTo);
                Assert.AreEqual("T", member.Type);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                Assert.AreEqual(2, member.Parameters.Count);
                arg = member.Parameters[0];
                Assert.AreEqual("expected", arg.Name);
                Assert.AreEqual("T", arg.Type);
                arg = member.Parameters[1];
                Assert.AreEqual("actual", arg.Name);
                Assert.AreEqual("Z", arg.Type);
                member = fullConstraintGeneric.Members[1];
                Assert.AreEqual("test2<K:({},Measurable),V:(Iterable<String>,Measurable)>", member.FullName);
                Assert.AreEqual("test2", member.Name);
                Assert.AreEqual("<K:({},Measurable),V:(Iterable<String>,Measurable)>", member.Template);
                Assert.AreEqual(19, member.LineFrom);
                Assert.AreEqual(21, member.LineTo);
                Assert.AreEqual("K", member.Type);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                arg = member.Parameters[0];
                Assert.AreEqual("expected", arg.Name);
                Assert.AreEqual("K", arg.Type);
                arg = member.Parameters[1];
                Assert.AreEqual("actual", arg.Name);
                Assert.AreEqual("V", arg.Type);

                // TODO: There should be a space between 'function' and 'new'! There should be a separate test covering this error
                var typeDefConstraintGeneric = model.Classes[2];
                Assert.AreEqual(24, typeDefConstraintGeneric.LineFrom);
                Assert.AreEqual(33, typeDefConstraintGeneric.LineTo);
                Assert.AreEqual(FlagType.Class, typeDefConstraintGeneric.Flags & FlagType.Class);
                Assert.AreEqual("TestTypeDefConstraint<T:({functionnew():Void;},Measurable)>", typeDefConstraintGeneric.FullName);
                Assert.AreEqual("TestTypeDefConstraint", typeDefConstraintGeneric.Name);
                Assert.AreEqual("<T:({functionnew():Void;},Measurable)>", typeDefConstraintGeneric.Template);
                Assert.AreEqual(2, typeDefConstraintGeneric.Members.Count);
                member = typeDefConstraintGeneric.Members[0];
                Assert.AreEqual("test1", member.Name);
                Assert.AreEqual(26, member.LineFrom);
                Assert.AreEqual(28, member.LineTo);
                Assert.AreEqual("T", member.Type);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                Assert.AreEqual(2, member.Parameters.Count);
                arg = member.Parameters[0];
                Assert.AreEqual("expected", arg.Name);
                Assert.AreEqual("T", arg.Type);
                arg = member.Parameters[1];
                Assert.AreEqual("actual", arg.Name);
                Assert.AreEqual("T", arg.Type);
                member = typeDefConstraintGeneric.Members[1];
                Assert.AreEqual("test2<K:({functionnew():Void;},Measurable)>", member.FullName);
                Assert.AreEqual("test2", member.Name);
                Assert.AreEqual("<K:({functionnew():Void;},Measurable)>", member.Template);
                Assert.AreEqual(30, member.LineFrom);
                Assert.AreEqual(32, member.LineTo);
                Assert.AreEqual("K", member.Type);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                arg = member.Parameters[0];
                Assert.AreEqual("expected", arg.Name);
                Assert.AreEqual("K", arg.Type);
                arg = member.Parameters[1];
                Assert.AreEqual("actual", arg.Name);
                Assert.AreEqual("K", arg.Type);
            }

            [Test(Description = "Includes Commit a2b92a6")]
            public void ParseFile_Regions()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("RegionsTest"));
                Assert.AreEqual(1, model.Classes.Count);
                Assert.AreEqual(2, model.Regions.Count);

                var region = model.Regions[0];
                Assert.AreEqual("Fields", region.Name);
                Assert.AreEqual(4, region.LineFrom);
                Assert.AreEqual(6, region.LineTo);
                region = model.Regions[1];
                Assert.AreEqual("Complex stuff", region.Name);
                Assert.AreEqual(14, region.LineFrom);
                Assert.AreEqual(16, region.LineTo);

                var classModel = model.Classes[0];
                Assert.AreEqual("Test", classModel.Name);
                Assert.AreEqual(2, classModel.LineFrom);
                Assert.AreEqual(18, classModel.LineTo);
                Assert.AreEqual(FlagType.Class, classModel.Flags & FlagType.Class);
                Assert.AreEqual(3, classModel.Members.Count);
                var member = classModel.Members[0];
                Assert.AreEqual(5, member.LineFrom);
                Assert.AreEqual(5, member.LineTo);
                Assert.AreEqual("_test", member.Name);
                Assert.AreEqual("String", member.Type);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                Assert.AreEqual(Visibility.Private, member.Access & Visibility.Private);
                member = classModel.Members[1];
                Assert.AreEqual(9, member.LineFrom);
                Assert.AreEqual(9, member.LineTo);
                Assert.AreEqual("_test2", member.Name);
                Assert.AreEqual("String", member.Type);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                Assert.AreEqual(Visibility.Private, member.Access & Visibility.Private);
                member = classModel.Members[2];
                Assert.AreEqual(12, member.LineFrom);
                Assert.AreEqual(17, member.LineTo);
                Assert.AreEqual("regionInside", member.Name);
                Assert.AreEqual("String", member.Type);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                Assert.AreEqual(Visibility.Private, member.Access & Visibility.Private);
            }

            [Test]
            public void ParseFile_Comments()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("CommentsTest"));
                Assert.AreEqual(1, model.Classes.Count);

                var classModel = model.Classes[0];
                Assert.AreEqual("Test", classModel.Name);
                Assert.AreEqual("\r * Some custom comments\r ", classModel.Comments);
                Assert.AreEqual(5, classModel.LineFrom);
                Assert.AreEqual(19, classModel.LineTo);
                Assert.AreEqual(FlagType.Class, classModel.Flags & FlagType.Class);
                Assert.AreEqual(2, classModel.Members.Count);
                var member = classModel.Members[0];
                Assert.AreEqual(10, member.LineFrom);
                Assert.AreEqual(10, member.LineTo);
                Assert.AreEqual("Test", member.Name);
                Assert.AreEqual("Java Style comments", member.Comments);
                Assert.AreEqual(FlagType.Constructor, member.Flags & FlagType.Constructor);
                Assert.AreEqual(Visibility.Public, member.Access & Visibility.Public);
                member = classModel.Members[1];
                Assert.AreEqual(15, member.LineFrom);
                Assert.AreEqual(18, member.LineTo);
                Assert.AreEqual("testAdd", member.Name);
                Assert.AreEqual("Int", member.Type);
                Assert.AreEqual("\r\t * Some method documentation\r\t ", member.Comments);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                Assert.AreEqual(Visibility.Public, member.Access & Visibility.Public);
                Assert.AreEqual(2, member.Parameters.Count);
            }

            [Test(Description = "Shows that enum elements are not getting comments currently")]
            public void ParseFile_SpecialClassesComments()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("SpecialClassesCommentsTest"));
                Assert.AreEqual(3, model.Classes.Count);

                var classModel = model.Classes[0];
                Assert.AreEqual("TypedefTest", classModel.Name);
                Assert.AreEqual("\r * Some typedef custom comments\r ", classModel.Comments);
                Assert.AreEqual(5, classModel.LineFrom);
                Assert.AreEqual(11, classModel.LineTo);
                Assert.AreEqual(FlagType.TypeDef, classModel.Flags & FlagType.TypeDef);
                Assert.AreEqual(1, classModel.Members.Count);
                var member = classModel.Members[0];
                Assert.AreEqual(10, member.LineFrom);
                Assert.AreEqual(10, member.LineTo);
                Assert.AreEqual("age", member.Name);
                Assert.AreEqual("Int", member.Type);
                Assert.AreEqual("Java Style comments", member.Comments);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);

                classModel = model.Classes[1];
                Assert.AreEqual("EnumTest", classModel.Name);
                Assert.AreEqual("\r * Some enum custom comments\r ", classModel.Comments);
                Assert.AreEqual(16, classModel.LineFrom);
                Assert.AreEqual(22, classModel.LineTo);
                Assert.AreEqual(FlagType.Enum, classModel.Flags & FlagType.Enum);
                Assert.AreEqual(1, classModel.Members.Count);
                member = classModel.Members[0];
                Assert.AreEqual(21, member.LineFrom);
                Assert.AreEqual(21, member.LineTo);
                Assert.AreEqual("Foo", member.Name);
                //TODO: Add support for this!
                //Assert.AreEqual("\r\t * Enum element comments\r\t ", member.Comments);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);

                classModel = model.Classes[2];
                Assert.AreEqual("AbstractInt", classModel.Name);
                Assert.AreEqual("\r * Some abstract custom comments\r ", classModel.Comments);
                Assert.AreEqual("Int", classModel.ExtendsType);
                Assert.AreEqual(27, classModel.LineFrom);
                Assert.AreEqual(34, classModel.LineTo);
                Assert.AreEqual(FlagType.Abstract, classModel.Flags & FlagType.Abstract);
                Assert.AreEqual(1, classModel.Members.Count);
                member = classModel.Members[0];
                Assert.AreEqual(31, member.LineFrom);
                Assert.AreEqual(33, member.LineTo);
                Assert.AreEqual("AbstractInt", member.Name);
                Assert.AreEqual("Java Style comments", member.Comments);
            }

            [Test(Description = "PR 680")]
            public void ParseFile_RegExLiterals()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("RegExTest"), true);

                Assert.AreEqual(4, model.Members.Count); // First member = function itself

                var regExMember = model.Members[1];
                Assert.AreEqual("regExInArray", regExMember.Name);
                Assert.AreEqual("Array<EReg>", regExMember.Type);
                regExMember = model.Members[2];
                Assert.AreEqual("regExInObject", regExMember.Name);
                regExMember = model.Members[3];
                Assert.AreEqual("regEx", regExMember.Name);
                Assert.AreEqual("EReg", regExMember.Type);
            }

            [Test(Description = "PR 680")]
            public void ParseFile_MultiLineStrings()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("MultiLineStringsTest"));

                Assert.AreEqual(1, model.Classes.Count);
                var classModel = model.Classes[0];

                Assert.AreEqual(2, classModel.Members.Count);
                var member = classModel.Members[0];
                Assert.AreEqual("test", member.Name);
                Assert.AreEqual("Int", member.Type);
                Assert.AreEqual(4, member.LineFrom);
                Assert.AreEqual(7, member.LineTo);
                Assert.AreEqual(1, member.Parameters.Count);
                var param = member.Parameters[0];
                Assert.AreEqual("arg", param.Name);
                Assert.AreEqual("String", param.Type);
                Assert.AreEqual("\"hello \r    world\"", param.Value);
                Assert.AreEqual(4, param.LineFrom);
                Assert.AreEqual(5, param.LineTo);

                member = classModel.Members[1];
                Assert.AreEqual("test2", member.Name);
                Assert.AreEqual("Float", member.Type);
                Assert.AreEqual(9, member.LineFrom);
                Assert.AreEqual(12, member.LineTo);
                Assert.AreEqual(1, member.Parameters.Count);
                param = member.Parameters[0];
                Assert.AreEqual("arg", param.Name);
                Assert.AreEqual("String", param.Type);
                Assert.AreEqual("'hello \r    world'", param.Value);
                Assert.AreEqual(9, param.LineFrom);
                Assert.AreEqual(10, param.LineTo);
            }

            [Test(Description = "PR 680")]
            public void ParseFile_StringWithEscapedChars()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("EscapedStringsTest"));

                Assert.AreEqual(1, model.Classes.Count);
                var classModel = model.Classes[0];

                Assert.AreEqual(1, classModel.Members.Count);
                var member = classModel.Members[0];
                Assert.AreEqual("test", member.Name);
                Assert.AreEqual("String", member.Type);
                Assert.AreEqual(4, member.LineFrom);
                Assert.AreEqual(6, member.LineTo);
                Assert.AreEqual(2, member.Parameters.Count);
                var param = member.Parameters[0];
                Assert.AreEqual("arg", param.Name);
                Assert.AreEqual("String", param.Type);
                Assert.AreEqual("\"hello \\t\\r\\n\\\\\\\"\\\\\"", param.Value);
                Assert.AreEqual(4, param.LineFrom);
                Assert.AreEqual(4, param.LineTo);
                param = member.Parameters[1];
                Assert.AreEqual("arg2", param.Name);
                Assert.AreEqual("String", param.Type);
                Assert.AreEqual(@"'hello \t\r\n\\\\'", param.Value);
                Assert.AreEqual(4, param.LineFrom);
                Assert.AreEqual(4, param.LineTo);
            }

            [Test]
            public void ParseFile_Imports()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("ImportTest"));

                Assert.AreEqual(4, model.Imports.Count); // import * ignored

                var import = model.Imports[0];
                Assert.AreEqual(FlagType.Class, import.Flags & FlagType.Class);
                Assert.AreEqual("Test", import.Name);
                Assert.AreEqual("package1.Test", import.Type);

                import = model.Imports[1];
                Assert.AreEqual(FlagType.Package, import.Flags & FlagType.Package);
                Assert.AreEqual("*", import.Name);
                Assert.AreEqual("package1.subpackage1.*", import.Type);

                import = model.Imports[2];
                Assert.AreEqual(FlagType.Package, import.Flags & FlagType.Package);
                Assert.AreEqual("*", import.Name);
                Assert.AreEqual("package2.*", import.Type);

                import = model.Imports[3];
                Assert.AreEqual(FlagType.Class, import.Flags & FlagType.Class);
                Assert.AreEqual("Test", import.Name);
                Assert.AreEqual("package2.subpackage1.Test", import.Type);
            }

            [Ignore("Not supported, for now we hope!")]
            public void ParseFile_ImportAliases()
            {
                Assert.AreEqual(2, ASContext.Context.GetCodeModel(ReadAllText("ImportAliasTest")).Imports.Count);
            }

            [Test]
            public void ParseFile_AnonymousStructures()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("AnonymousStructuresTest"));

                Assert.AreEqual(1, model.Classes.Count);

                var classModel = model.Classes[0];

                Assert.AreEqual(2, classModel.Members.Count);

                var member = classModel.Members[0];
                Assert.AreEqual("start", member.Name);
                Assert.AreEqual(1, member.LineFrom);
                Assert.AreEqual(1, member.LineTo);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                Assert.AreEqual("{x:Int, y:Int}", member.Type);

                member = classModel.Members[1];
                Assert.AreEqual("target", member.Name);
                Assert.AreEqual(2, member.LineFrom);
                Assert.AreEqual(2, member.LineTo);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                Assert.AreEqual("{x:Int, y:Int}", member.Type);
            }

            [Test]
            public void ParseFile_FunctionTypes()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("FunctionTypesTest"), true);

                Assert.AreEqual(3, model.Members.Count);

                var member = model.Members[1];
                Assert.AreEqual("functionType", member.Name);
                Assert.AreEqual(2, member.LineFrom);
                Assert.AreEqual(2, member.LineTo);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                Assert.AreEqual("Dynamic", member.Parameters[0].Type);
                Assert.AreEqual("Dynamic", member.Type);

                member = model.Members[2];
                Assert.AreEqual("functionType2", member.Name);
                Assert.AreEqual(3, member.LineFrom);
                Assert.AreEqual(3, member.LineTo);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                Assert.AreEqual("Int", member.Parameters[0].Type);
                Assert.AreEqual("Int", member.Parameters[1].Type);
                Assert.AreEqual("Int", member.Type);
            }

            static IEnumerable<TestCaseData> FunctionTypesTestCases
            {
                get
                {
                    yield return new TestCaseData("var functionType:String->Void;")
                        .Returns(new MemberWithType(new MemberModel
                        {
                            Name = "functionType", Flags = FlagType.Dynamic | FlagType.Variable | FlagType.Function,
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel("parameter0", "String", FlagType.ParameterVar, Visibility.Private)
                            }
                        }, "Void"));
                    yield return new TestCaseData("var functionType:(Int->String)->Void;")
                        .Returns(new MemberWithType(new MemberModel
                        {
                            Name = "functionType", Flags = FlagType.Dynamic | FlagType.Variable | FlagType.Function,
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel("parameter0", "Int->String", FlagType.ParameterVar, Visibility.Private)
                            }
                        }, "Void"));
                    yield return new TestCaseData("var functionType:String->(Int->String);")
                        .Returns(new MemberWithType(new MemberModel
                        {
                            Name = "functionType", Flags = FlagType.Dynamic | FlagType.Variable | FlagType.Function,
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel("parameter0", "String", FlagType.ParameterVar, Visibility.Private)
                            }
                        }, "Int->String"));
                    yield return new TestCaseData("var functionType:String->(Int->String)->Void;")
                        .Returns(new MemberWithType(new MemberModel
                        {
                            Name = "functionType", Flags = FlagType.Dynamic | FlagType.Variable | FlagType.Function,
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel("parameter0", "String", FlagType.ParameterVar, Visibility.Private),
                                new MemberModel("parameter1", "Int->String", FlagType.ParameterVar, Visibility.Private),
                            }
                        }, "Void"));
                    yield return new TestCaseData("var functionType:String->{c:Int->String};")
                        .Returns(new MemberWithType(new MemberModel
                        {
                            Name = "functionType", Flags = FlagType.Dynamic | FlagType.Variable | FlagType.Function,
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel("parameter0", "String", FlagType.ParameterVar, Visibility.Private),
                            }
                        }, "{c:Int->String}"));
                    yield return new TestCaseData("var functionType:{c:Int->String}->Void;")
                        .Returns(new MemberWithType(new MemberModel
                        {
                            Name = "functionType", Flags = FlagType.Dynamic | FlagType.Variable | FlagType.Function,
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel("parameter0", "{c:Int->String}", FlagType.ParameterVar, Visibility.Private),
                            }
                        }, "Void"));
                    yield return new TestCaseData("var functionType:{c:(Int->String)->String}->Void;")
                        .Returns(new MemberWithType(new MemberModel
                        {
                            Name = "functionType", Flags = FlagType.Dynamic | FlagType.Variable | FlagType.Function,
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel("parameter0", "{c:(Int->String)->String}", FlagType.ParameterVar, Visibility.Private),
                            }
                        }, "Void"));
                    yield return new TestCaseData("var functionType:String->{c:Int->Array<String>};")
                        .Returns(new MemberWithType(new MemberModel
                        {
                            Name = "functionType", Flags = FlagType.Dynamic | FlagType.Variable | FlagType.Function,
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel("parameter0", "String", FlagType.ParameterVar, Visibility.Private),
                            }
                        }, "{c:Int->Array<String>}"));
                    yield return new TestCaseData("var functionType:String->{c:Int->Array<{x:Int, y:Int}>};")
                        .Returns(new MemberWithType(new MemberModel
                        {
                            Name = "functionType", Flags = FlagType.Dynamic | FlagType.Variable | FlagType.Function,
                            Parameters = new List<MemberModel>
                            {
                                new MemberModel("parameter0", "String", FlagType.ParameterVar, Visibility.Private),
                            }
                        }, "{c:Int->Array<{x:Int, y:Int}>}"));
                }
            }

            [Test, TestCaseSource(nameof(FunctionTypesTestCases))]
            public MemberWithType ParseFunctionTypes(string sourceText)
            {
                var model = ASContext.Context.GetCodeModel(sourceText);
                var member = model.Members.Items.First();
                return new MemberWithType(member, member.Type);
            }

            static IEnumerable<TestCaseData> ParseFunctionParametersTestCases
            {
                get
                {
                    yield return new TestCaseData("function foo(p:String->Int) {}")
                        .Returns(new[] {"String->Int"});
                    yield return new TestCaseData("function foo(p:String->Int, p1:Int->String->Void) {}")
                        .Returns(new[] {"String->Int", "Int->String->Void"});
                    yield return new TestCaseData("function foo(p:String->Int->Void, p2:Int, p3:Int->Array<String>) {}")
                        .Returns(new[] {"String->Int->Void", "Int", "Int->Array<String>"});
                    yield return new TestCaseData("function foo(p:String->{x:Int, y:Int}->Void) {}")
                        .Returns(new[] {"String->{x:Int, y:Int}->Void"});
                    yield return new TestCaseData("function foo ( p : String -> { x : Int, y : Int } -> Void ) {}")
                        .Returns(new[] {"String->{x:Int, y:Int}->Void"});
                    yield return new TestCaseData("function foo(p:String->{p:{x:Int, y:Int}}->Void) {}")
                        .Returns(new[] {"String->{p:{x:Int, y:Int}}->Void"});
                    yield return new TestCaseData("function foo(p:String->{p1:{x:Int, y:Int}, p2:{x:Int, y:Int}}->Void) {}")
                        .Returns(new[] {"String->{p1:{x:Int, y:Int}, p2:{x:Int, y:Int}}->Void"});
                    yield return new TestCaseData("function foo(p:String->{a:Array<{x:Int, y:Int}>}->Void) {}")
                        .Returns(new[] {"String->{a:Array<{x:Int, y:Int}>}->Void"});
                    yield return new TestCaseData("function foo(p:(String->{a:Array<{x:Int, y:Int}>})->Void) {}")
                        .Returns(new[] {"(String->{a:Array<{x:Int, y:Int}>})->Void"});
                    yield return new TestCaseData("function foo(p:String->({a:Array<{x:Int, y:Int}>}->Void)) {}")
                        .Returns(new[] {"String->({a:Array<{x:Int, y:Int}>}->Void)"});
                    yield return new TestCaseData("function foo(p:Array<(Int->Void)->Int->Void>) {}")
                        .Returns(new[] {"Array<(Int->Void)->Int->Void>"});
                    yield return new TestCaseData("function foo(p : Array < ( Int -> Void ) -> Int -> Void > ) {}")
                        .Returns(new[] {"Array<(Int->Void)->Int->Void>"});
                    yield return new TestCaseData("function foo(p:Map<String, {x:Int, y:Int}>) {}")
                        .Returns(new[] {"Map<String, {x:Int, y:Int}>"});
                    yield return new TestCaseData("function foo(p:Map<{x:Int, y:Int}, String>) {}")
                        .Returns(new[] {"Map<{x:Int, y:Int}, String>"});
                    yield return new TestCaseData("function foo ( p : Map <{ x : Int , y : Int } , String> ) {}")
                        .Returns(new[] {"Map<{x:Int, y:Int}, String>"});
                    yield return new TestCaseData("function foo(p:Map< {x:Int, y:Int}, String > ) {}")
                        .Returns(new[] {"Map<{x:Int, y:Int}, String>"});
                    yield return new TestCaseData("function foo ( p : Map < { x : Int , y : Int } , String > ) {}")
                        .Returns(new[] {"Map<{x:Int, y:Int}, String>"});
                    yield return new TestCaseData("function foo ( p : Map < { c : Int -> { x : Int , y : Int } } , String > ) {}")
                        .Returns(new[] {"Map<{c:Int->{x:Int, y:Int}}, String>"});
                    yield return new TestCaseData("function foo(p:Map<{c:Int->{x:Int,y:Int}},String>) {}")
                        .Returns(new[] {"Map<{c:Int->{x:Int, y:Int}}, String>"});
                    yield return new TestCaseData("function foo(p:Map<{c:Int->Point/*{x:Int,y:Int}*/},String>) {}")
                        .Returns(new[] {"Map<{c:Int->Point}, String>"});
                    yield return new TestCaseData("function foo(p:Map<{c:Int->/*{x:Int,y:Int}*/Point},String>) {}")
                        .Returns(new[] {"Map<{c:Int->Point}, String>"});
                    yield return new TestCaseData("function foo(p:Map<Int, Array<Map<Int, String>>>) {}")
                        .Returns(new[] {"Map<Int, Array<Map<Int, String>>>"});
                    yield return new TestCaseData("function foo(p:{}) {}")
                        .Returns(new[] {"{}"});
                    yield return new TestCaseData("function foo(p:String->?{x:Int, y:Int}->Void) {}")
                        .Returns(new[] {"String->?{x:Int, y:Int}->Void"});
                    yield return new TestCaseData("function foo(p:?String->?{x:Int, y:Int}->Void) {}")
                        .Returns(new[] {"?String->?{x:Int, y:Int}->Void"});
                    yield return new TestCaseData("function foo(p:?String->?{?x:Int, ?y:Int}->Void) {}")
                        .Returns(new[] {"?String->?{?x:Int, ?y:Int}->Void"});
                    yield return new TestCaseData("function foo ( p : ?String -> ?{ ?x : Int , ?y : Int} -> Void ) {}")
                        .Returns(new[] {"?String->?{?x:Int, ?y:Int}->Void"});
                    yield return new TestCaseData("function foo(p:Array<?(Int->Void)->Int->Void>) {}")
                        .Returns(new[] {"Array<?(Int->Void)->Int->Void>"});
                    yield return new TestCaseData("function foo(p:Array<?(?Int->Void)->?Int->Void>) {}")
                        .Returns(new[] {"Array<?(?Int->Void)->?Int->Void>"});
                    yield return new TestCaseData("function foo ( p : Array < ? ( ?Int -> Void ) -> ?Int -> Void > ) {}")
                        .Returns(new[] {"Array<?(?Int->Void)->?Int->Void>"});
                    yield return new TestCaseData("function foo(v:{a:Array<haxe.Timer>}->{a:haxe.ds.Vector<Type.ValueType>}->String) {}")
                        .Returns(new[] {"{a:Array<haxe.Timer>}->{a:haxe.ds.Vector<Type.ValueType>}->String"})
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1699");
                    yield return new TestCaseData("function foo(v:{a:Array<haxe.Timer>->haxe.ds.Vector<Type.ValueType>}->String) {}")
                        .Returns(new[] {"{a:Array<haxe.Timer>->haxe.ds.Vector<Type.ValueType>}->String"})
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1699");
                    yield return new TestCaseData("function foo(v:{a:Array<haxe.Timer>->Int->{x:Int, y:Int}}->String) {}")
                        .Returns(new[] {"{a:Array<haxe.Timer>->Int->{x:Int, y:Int}}->String"})
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1699");
                    yield return new TestCaseData("function foo(v:Array<{a:Array<haxe.Timer>->Int->{x:Int, y:Int}}>->String) {}")
                        .Returns(new[] {"Array<{a:Array<haxe.Timer>->Int->{x:Int, y:Int}}>->String"})
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1699");
                }
            }

            [Test, TestCaseSource(nameof(ParseFunctionParametersTestCases))]
            public IEnumerable<string> ParseFunctionParameters(string sourceText)
            {
                var model = ASContext.Context.GetCodeModel(sourceText);
                var member = model.Members.Items.First();
                return member.Parameters.Select(it => it.Type);
            }

            [Test]
            public void ParseFile_FunctionTypesWithSubTypes()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("FunctionTypesWithSubTypesTest"), true);

                Assert.AreEqual(5, model.Members.Count);

                var member = model.Members[0];
                Assert.AreEqual("functionTypesWithSubTypes", member.Name);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                Assert.AreEqual(3, member.Parameters.Count);
                Assert.AreEqual("((Dynamic->Dynamic)->Int)->Int", member.Type);
                var arg = member.Parameters[0];
                Assert.AreEqual("functionTypeArg", arg.Name);
                Assert.AreEqual("(Dynamic->Dynamic)->Dynamic", arg.Type);
                arg = member.Parameters[1];
                Assert.AreEqual("functionTypeArg2", arg.Name);
                Assert.AreEqual("(Dynamic->Dynamic)->(Int->Int)", arg.Type);
                Assert.AreEqual("null", arg.Value);
                arg = member.Parameters[2];
                Assert.AreEqual("test2", arg.Name);
                Assert.AreEqual("String", arg.Type);

                member = model.Members[1];
                Assert.AreEqual("functionType", member.Name);
                Assert.AreEqual(2, member.LineFrom);
                Assert.AreEqual(2, member.LineTo);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                Assert.AreEqual("Dynamic->Dynamic", member.Parameters[0].Type);
                Assert.AreEqual("Dynamic", member.Type);

                member = model.Members[2];
                Assert.AreEqual("functionType2", member.Name);
                Assert.AreEqual(3, member.LineFrom);
                Assert.AreEqual(3, member.LineTo);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                Assert.AreEqual("(Dynamic->Dynamic)->Int", member.Parameters[0].Type);
                Assert.AreEqual("Int", member.Type);

                member = model.Members[3];
                Assert.AreEqual("functionType3", member.Name);
                Assert.AreEqual(4, member.LineFrom);
                Assert.AreEqual(4, member.LineTo);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                Assert.AreEqual("Dynamic->Dynamic", member.Parameters[0].Type);
                Assert.AreEqual("Int->Int", member.Type);

                member = model.Members[4];
                Assert.AreEqual("functionType4", member.Name);
                Assert.AreEqual(5, member.LineFrom);
                Assert.AreEqual(5, member.LineTo);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                Assert.IsEmpty(member.Parameters);
                Assert.AreEqual("Void->Array<Int>", member.Type);
            }

            [Test]
            public void ParseFile_MultipleVarsAtOnce()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("MultipleVarsAtOnceTest"), true);

                Assert.AreEqual(7, model.Members.Count);

                var member = model.Members[1];
                Assert.AreEqual("var1", member.Name);
                Assert.AreEqual(2, member.LineFrom);
                Assert.AreEqual(2, member.LineTo);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                Assert.AreEqual("Int", member.Type);

                member = model.Members[2];
                Assert.AreEqual("var2", member.Name);
                Assert.AreEqual(2, member.LineFrom);
                Assert.AreEqual(2, member.LineTo);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                Assert.AreEqual("Dynamic->Dynamic", member.Parameters[0].Type);
                Assert.AreEqual("Int->Int", member.Type);

                member = model.Members[3];
                Assert.AreEqual("var3", member.Name);
                Assert.AreEqual(3, member.LineFrom);
                Assert.AreEqual(3, member.LineTo);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                Assert.AreEqual("Float", member.Type);

                member = model.Members[4];
                Assert.AreEqual("var4", member.Name);
                Assert.AreEqual(3, member.LineFrom);
                Assert.AreEqual(3, member.LineTo);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                Assert.AreEqual("String", member.Type);

                member = model.Members[5];
                Assert.AreEqual("var5", member.Name);
                Assert.AreEqual(4, member.LineFrom);
                Assert.AreEqual(4, member.LineTo);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                Assert.AreEqual("Bool", member.Type);

                member = model.Members[6];
                Assert.AreEqual("var6", member.Name);
                Assert.AreEqual(5, member.LineFrom);
                Assert.AreEqual(5, member.LineTo);
                Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);
                Assert.AreEqual("Dynamic", member.Type);
            }

            [Test]
            public void ParseFile_ComplexClass()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("ComplexClassTest"));
                var classModel = model.Classes[0];
                Assert.AreEqual("Test", classModel.Name);
                Assert.AreEqual(FlagType.Class, classModel.Flags & FlagType.Class);
                Assert.AreEqual(2, classModel.LineFrom);
                Assert.AreEqual(49, classModel.LineTo);
                Assert.AreEqual(15, classModel.Members.Count);

                var memberModel = classModel.Members[0];
                Assert.AreEqual("CONSTANT", memberModel.Name);
                var flags = FlagType.Static | FlagType.Variable;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
                Assert.AreEqual(4, memberModel.LineFrom);
                Assert.AreEqual(4, memberModel.LineTo);

                memberModel = classModel.Members[1];
                Assert.AreEqual("id", memberModel.Name);
                Assert.AreEqual("Int", memberModel.Type);
                flags = FlagType.Variable;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
                Assert.AreEqual(6, memberModel.LineFrom);
                Assert.AreEqual(6, memberModel.LineTo);

                memberModel = classModel.Members[2];
                Assert.AreEqual("_name", memberModel.Name);
                Assert.AreEqual("String", memberModel.Type);
                flags = FlagType.Variable;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Private, memberModel.Access & Visibility.Private);
                Assert.AreEqual(8, memberModel.LineFrom);
                Assert.AreEqual(8, memberModel.LineTo);

                memberModel = classModel.Members[3];
                Assert.AreEqual("ro", memberModel.Name);
                Assert.AreEqual("Int", memberModel.Type);
                flags = FlagType.Getter;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
                Assert.AreEqual(10, memberModel.LineFrom);
                Assert.AreEqual(10, memberModel.LineTo);
                Assert.AreEqual(2, memberModel.Parameters.Count);
                Assert.AreEqual("default", memberModel.Parameters[0].Name);
                Assert.AreEqual("null", memberModel.Parameters[1].Name);

                memberModel = classModel.Members[4];
                Assert.AreEqual("wo", memberModel.Name);
                Assert.AreEqual("Int", memberModel.Type);
                flags = FlagType.Getter;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
                Assert.AreEqual(12, memberModel.LineFrom);
                Assert.AreEqual(12, memberModel.LineTo);
                Assert.AreEqual(2, memberModel.Parameters.Count);
                Assert.AreEqual("null", memberModel.Parameters[0].Name);
                Assert.AreEqual("default", memberModel.Parameters[1].Name);

                memberModel = classModel.Members[5];
                Assert.AreEqual("x", memberModel.Name);
                Assert.AreEqual("Int", memberModel.Type);
                flags = FlagType.Getter;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
                Assert.AreEqual(14, memberModel.LineFrom);
                Assert.AreEqual(14, memberModel.LineTo);
                Assert.AreEqual(2, memberModel.Parameters.Count);
                Assert.AreEqual("get", memberModel.Parameters[0].Name);
                Assert.AreEqual("set", memberModel.Parameters[1].Name);

                memberModel = classModel.Members[6];
                Assert.AreEqual("get_x", memberModel.Name);
                Assert.AreEqual(null, memberModel.Type);
                flags = FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Private, memberModel.Access & Visibility.Private);
                Assert.AreEqual(15, memberModel.LineFrom);
                Assert.AreEqual(18, memberModel.LineTo);
                Assert.IsNull(memberModel.Parameters);

                memberModel = classModel.Members[7];
                Assert.AreEqual("set_x", memberModel.Name);
                Assert.AreEqual(null, memberModel.Type);
                flags = FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Private, memberModel.Access & Visibility.Private);
                Assert.AreEqual(19, memberModel.LineFrom);
                Assert.AreEqual(22, memberModel.LineTo);
                Assert.AreEqual(1, memberModel.Parameters.Count);
                Assert.AreEqual("val", memberModel.Parameters[0].Name);
                flags = FlagType.ParameterVar;
                Assert.AreEqual(flags, memberModel.Parameters[0].Flags & flags);

                memberModel = classModel.Members[8];
                Assert.AreEqual("y", memberModel.Name);
                Assert.AreEqual("Int", memberModel.Type);
                flags = FlagType.Getter;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
                Assert.AreEqual(24, memberModel.LineFrom);
                Assert.AreEqual(24, memberModel.LineTo);
                Assert.AreEqual(2, memberModel.Parameters.Count);
                Assert.AreEqual("get", memberModel.Parameters[0].Name);
                Assert.AreEqual("never", memberModel.Parameters[1].Name);

                memberModel = classModel.Members[9];
                Assert.AreEqual("get_y", memberModel.Name);
                Assert.AreEqual("Int", memberModel.Type);
                flags = FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Private, memberModel.Access & Visibility.Private);
                Assert.AreEqual(25, memberModel.LineFrom);
                Assert.AreEqual(25, memberModel.LineTo);
                Assert.IsNull(memberModel.Parameters);

                memberModel = classModel.Members[10];
                Assert.AreEqual("Test", memberModel.Name);
                flags = FlagType.Function | FlagType.Constructor;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
                Assert.AreEqual(27, memberModel.LineFrom);
                Assert.AreEqual(29, memberModel.LineTo);
                Assert.AreEqual(1, memberModel.Parameters.Count);
                var param = memberModel.Parameters[0];
                Assert.AreEqual("?ds", param.Name);
                Assert.AreEqual("Iterable<String>", param.Type);
                Assert.AreEqual(FlagType.ParameterVar, param.Flags & FlagType.ParameterVar);

                memberModel = classModel.Members[11];
                Assert.AreEqual("bar", memberModel.Name);
                Assert.AreEqual("Void", memberModel.Type);
                flags = FlagType.Static | FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Private, memberModel.Access & Visibility.Private);
                Assert.AreEqual(31, memberModel.LineFrom);
                Assert.AreEqual(33, memberModel.LineTo);
                Assert.AreEqual(2, memberModel.Parameters.Count);
                param = memberModel.Parameters[0];
                Assert.AreEqual("s", param.Name);
                Assert.AreEqual("String", param.Type);
                Assert.AreEqual(FlagType.ParameterVar, param.Flags & FlagType.ParameterVar);
                Assert.AreEqual(31, param.LineFrom);
                Assert.AreEqual(31, param.LineTo);
                param = memberModel.Parameters[1];
                Assert.AreEqual("v", param.Name);
                Assert.AreEqual("Bool", param.Type);
                Assert.AreEqual(FlagType.ParameterVar, param.Flags & FlagType.ParameterVar);
                Assert.AreEqual(31, param.LineFrom);
                Assert.AreEqual(31, param.LineTo);

                memberModel = classModel.Members[12];
                Assert.AreEqual("foo", memberModel.Name);
                Assert.AreEqual("Bool", memberModel.Type);
                flags = FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
                Assert.AreEqual(35, memberModel.LineFrom);
                Assert.AreEqual(38, memberModel.LineTo);
                Assert.AreEqual(2, memberModel.Parameters.Count);
                param = memberModel.Parameters[0];
                Assert.AreEqual("?s", param.Name);
                Assert.AreEqual("String", param.Type);
                Assert.AreEqual(FlagType.ParameterVar, param.Flags & FlagType.ParameterVar);
                Assert.AreEqual(35, param.LineFrom);
                Assert.AreEqual(35, param.LineTo);
                param = memberModel.Parameters[1];
                Assert.AreEqual("?v", param.Name);
                Assert.AreEqual("Bool", param.Type);
                Assert.AreEqual("true", param.Value);
                Assert.AreEqual(FlagType.ParameterVar, param.Flags & FlagType.ParameterVar);
                Assert.AreEqual(35, param.LineFrom);
                Assert.AreEqual(35, param.LineTo);

                memberModel = classModel.Members[13];
                Assert.AreEqual("boz", memberModel.Name);
                flags = FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
                Assert.AreEqual(40, memberModel.LineFrom);
                Assert.AreEqual(44, memberModel.LineTo);
                Assert.AreEqual(2, memberModel.Parameters.Count);
                param = memberModel.Parameters[0];
                Assert.AreEqual("?s", param.Name);
                Assert.AreEqual("String", param.Type);
                Assert.AreEqual(FlagType.ParameterVar, param.Flags & FlagType.ParameterVar);
                Assert.AreEqual(40, param.LineFrom);
                Assert.AreEqual(40, param.LineTo);
                param = memberModel.Parameters[1];
                Assert.AreEqual("?v", param.Name);
                Assert.AreEqual("Bool", param.Type);
                Assert.AreEqual("true", param.Value);
                Assert.AreEqual(FlagType.ParameterVar, param.Flags & FlagType.ParameterVar);
                Assert.AreEqual(41, param.LineFrom);
                Assert.AreEqual(41, param.LineTo);

                memberModel = classModel.Members[14];
                Assert.AreEqual("nestedGenerics", memberModel.Name);
                flags = FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
                Assert.AreEqual(46, memberModel.LineFrom);
                Assert.AreEqual(48, memberModel.LineTo);
                Assert.AreEqual(1, memberModel.Parameters.Count);
                param = memberModel.Parameters[0];
                Assert.AreEqual("s", param.Name);
                Assert.AreEqual("Array<Array<Int>>", param.Type);
                Assert.AreEqual(FlagType.ParameterVar, param.Flags & FlagType.ParameterVar);
                Assert.AreEqual(46, param.LineFrom);
                Assert.AreEqual(46, param.LineTo);
            }

            [Test(Description = "Issue 1075")]
            public void ParseFile_MethodAfterGenericReturn()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("MethodAfterGenericReturnTest"));
                var classModel = model.Classes[0];
                Assert.AreEqual("Test", classModel.Name);
                Assert.AreEqual(FlagType.Class, classModel.Flags & FlagType.Class);
                Assert.AreEqual(2, classModel.LineFrom);
                Assert.AreEqual(19, classModel.LineTo);
                Assert.AreEqual(4, classModel.Members.Count);

                var memberModel = classModel.Members[0];
                Assert.AreEqual("retMethod", memberModel.Name);
                Assert.AreEqual("Array<Dynamic>", memberModel.Type);
                var flags = FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(4, memberModel.LineFrom);
                Assert.AreEqual(6, memberModel.LineTo);

                memberModel = classModel.Members[1];
                Assert.AreEqual("func", memberModel.Name);
                Assert.AreEqual("Void", memberModel.Type);
                flags = FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(8, memberModel.LineFrom);
                Assert.AreEqual(10, memberModel.LineTo);
                Assert.AreEqual(1, memberModel.Parameters.Count);
                Assert.AreEqual("arg", memberModel.Parameters[0].Name);
                Assert.AreEqual("String", memberModel.Parameters[0].Type);

                memberModel = classModel.Members[2];
                Assert.AreEqual("retMethod2", memberModel.Name);
                Assert.AreEqual("{x:Int, y:Int}", memberModel.Type);
                flags = FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(12, memberModel.LineFrom);
                Assert.AreEqual(14, memberModel.LineTo);
                Assert.IsNull(memberModel.Parameters);

                memberModel = classModel.Members[3];
                Assert.AreEqual("func2", memberModel.Name);
                Assert.AreEqual("Void", memberModel.Type);
                flags = FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(16, memberModel.LineFrom);
                Assert.AreEqual(18, memberModel.LineTo);
                Assert.AreEqual(3, memberModel.Parameters.Count);
                Assert.AreEqual("arg", memberModel.Parameters[0].Name);
                Assert.AreEqual("Array<Dynamic>", memberModel.Parameters[0].Type);
                Assert.AreEqual("arg2", memberModel.Parameters[1].Name);
                Assert.AreEqual("Array<String>", memberModel.Parameters[1].Type);
                Assert.AreEqual("null", memberModel.Parameters[1].Value);
                Assert.AreEqual("arg3", memberModel.Parameters[2].Name);
                Assert.AreEqual("String", memberModel.Parameters[2].Type);
            }

            [Test(Description = "Issue 1125")]
            public void ParseFile_FunctionTypesAsArguments()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("FunctionTypesAsArgumentsTest"));
                var classModel = model.Classes[0];
                Assert.AreEqual("Test", classModel.Name);
                Assert.AreEqual(FlagType.Class, classModel.Flags & FlagType.Class);
                Assert.AreEqual(2, classModel.LineFrom);
                Assert.AreEqual(15, classModel.LineTo);
                Assert.AreEqual(3, classModel.Members.Count);

                var memberModel = classModel.Members[0];
                Assert.AreEqual("func1", memberModel.Name);
                Assert.AreEqual("Array<Dynamic>", memberModel.Type);
                var flags = FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Private, memberModel.Access);
                Assert.AreEqual(4, memberModel.LineFrom);
                Assert.AreEqual(6, memberModel.LineTo);
                Assert.AreEqual(1, memberModel.Parameters.Count);
                Assert.AreEqual("arg", memberModel.Parameters[0].Name);
                Assert.AreEqual("(Float->Int)->(Int->Array<Dynamic>)", memberModel.Parameters[0].Type);

                memberModel = classModel.Members[1];
                Assert.AreEqual("func2", memberModel.Name);
                Assert.AreEqual(null, memberModel.Type);
                flags = FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Public, memberModel.Access);
                Assert.AreEqual(8, memberModel.LineFrom);
                Assert.AreEqual(10, memberModel.LineTo);
                Assert.AreEqual(2, memberModel.Parameters.Count);
                Assert.AreEqual("arg1", memberModel.Parameters[0].Name);
                Assert.AreEqual("Int->Void", memberModel.Parameters[0].Type);
                Assert.AreEqual("arg2", memberModel.Parameters[1].Name);
                Assert.AreEqual("Dynamic", memberModel.Parameters[1].Type);

                memberModel = classModel.Members[2];
                Assert.AreEqual("func3", memberModel.Name);
                Assert.AreEqual("Int", memberModel.Type);
                flags = FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Private, memberModel.Access);
                Assert.AreEqual(12, memberModel.LineFrom);
                Assert.AreEqual(14, memberModel.LineTo);
                Assert.AreEqual(1, memberModel.Parameters.Count);
                Assert.AreEqual("arg", memberModel.Parameters[0].Name);
                Assert.AreEqual("Float", memberModel.Parameters[0].Type);
            }

            [Test(Description = "Issue 1141")]
            public void ParseFile_KeywordAndUnderscoreInName()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("KeywordAndUnderscoreInNameTest"));
                var classModel = model.Classes[0];
                Assert.AreEqual(1, model.Classes.Count);
                Assert.AreEqual("Test", classModel.Name);
                Assert.AreEqual(FlagType.Class, classModel.Flags & FlagType.Class);
                Assert.AreEqual(2, classModel.LineFrom);
                Assert.AreEqual(6, classModel.LineTo);
                Assert.AreEqual(2, classModel.Members.Count);

                var memberModel = classModel.Members[0];
                Assert.AreEqual("var_1244", memberModel.Name);
                Assert.AreEqual("Int", memberModel.Type);
                var flags = FlagType.Variable | FlagType.Static;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
                Assert.AreEqual(4, memberModel.LineFrom);
                Assert.AreEqual(4, memberModel.LineTo);

                memberModel = classModel.Members[1];
                Assert.AreEqual("ERR_LOGINFAILED", memberModel.Name);
                Assert.AreEqual("Int", memberModel.Type);
                flags = FlagType.Variable | FlagType.Static;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
                Assert.AreEqual(5, memberModel.LineFrom);
                Assert.AreEqual(5, memberModel.LineTo);
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
                Assert.AreNotEqual("thísIsVälid", memberModel.Name);
                Assert.AreEqual(4, memberModel.LineFrom);
                Assert.AreEqual(6, memberModel.LineTo);

                memberModel = classModel.Members[1];
                Assert.AreNotEqual("日本語文字ヴァリアブル", memberModel.Name);
                Assert.AreEqual(8, memberModel.LineFrom);
                Assert.AreEqual(8, memberModel.LineTo);
            }

            [Test]
            public void ParseFile_NotGeneric()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("NotGenericTest"), true);

                Assert.AreEqual(3, model.Members.Count); // First member = function itself

                var funcMember = model.Members[0];
                Assert.AreEqual("init", funcMember.Name);
                Assert.AreEqual(FlagType.Function, funcMember.Flags & FlagType.Function);
                var member1 = model.Members[1];
                Assert.AreEqual("testA", member1.Name);
                Assert.AreEqual("Int", member1.Type);
                var member2 = model.Members[2];
                Assert.AreEqual("i1", member2.Name);
            }

            [Test]
            public void ParseFile_MetadataClass()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("MetadataTest"));
                var classModel = model.Classes[0];
                Assert.AreEqual("MetadataTest", classModel.Name);
                Assert.AreEqual(FlagType.Class, classModel.Flags & FlagType.Class);
                Assert.AreEqual(2, classModel.LineFrom);
                Assert.AreEqual(28, classModel.LineTo);
                Assert.AreEqual(6, classModel.Members.Count);

                var memberModel = classModel.Members[0];
                Assert.AreEqual("func", memberModel.Name);
                Assert.AreEqual("Void", memberModel.Type);
                var flags = FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Private, memberModel.Access & Visibility.Private);
                Assert.AreEqual(4, memberModel.LineFrom);
                Assert.AreEqual(6, memberModel.LineTo);
                Assert.IsNull(memberModel.Parameters);
                Assert.IsNull(memberModel.MetaDatas);

                memberModel = classModel.Members[1];
                Assert.AreEqual("test", memberModel.Name);
                Assert.AreEqual("Int", memberModel.Type);
                flags = FlagType.Variable;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Private, memberModel.Access & Visibility.Private);
                Assert.AreEqual(9, memberModel.LineFrom);
                Assert.AreEqual(9, memberModel.LineTo);
                Assert.AreEqual(1, memberModel.MetaDatas.Count);
                Assert.AreEqual(":allow", memberModel.MetaDatas[0].Name);
                Assert.AreEqual("flixel", memberModel.MetaDatas[0].Params["Default"]);

                memberModel = classModel.Members[2];
                Assert.AreEqual("func2", memberModel.Name);
                Assert.AreEqual(null, memberModel.Type);
                flags = FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Private, memberModel.Access & Visibility.Private);
                Assert.AreEqual(11, memberModel.LineFrom);
                Assert.AreEqual(13, memberModel.LineTo);
                Assert.IsNull(memberModel.Parameters);
                Assert.IsNull(memberModel.MetaDatas);

                memberModel = classModel.Members[3];
                Assert.AreEqual("test2", memberModel.Name);
                Assert.AreEqual("Int", memberModel.Type);
                flags = FlagType.Variable;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Private, memberModel.Access & Visibility.Private);
                Assert.AreEqual(16, memberModel.LineFrom);
                Assert.AreEqual(16, memberModel.LineTo);
                Assert.AreEqual(1, memberModel.MetaDatas.Count);
                Assert.AreEqual(":allow", memberModel.MetaDatas[0].Name);
                Assert.AreEqual("flixel", memberModel.MetaDatas[0].Params["Default"]);

                memberModel = classModel.Members[4];
                Assert.AreEqual("func3", memberModel.Name);
                Assert.AreEqual("Void", memberModel.Type);
                flags = FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Private, memberModel.Access & Visibility.Private);
                Assert.AreEqual(18, memberModel.LineFrom);
                Assert.AreEqual(20, memberModel.LineTo);
                Assert.IsNull(memberModel.Parameters);
                Assert.IsNull(memberModel.MetaDatas);

                memberModel = classModel.Members[5];
                Assert.AreEqual("test3", memberModel.Name);
                Assert.AreEqual("Bool", memberModel.Type);
                flags = FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Public, memberModel.Access & Visibility.Public);
                Assert.AreEqual(24, memberModel.LineFrom);
                Assert.AreEqual(27, memberModel.LineTo);
                Assert.AreEqual(1, memberModel.Parameters.Count);
                Assert.AreEqual("arg", memberModel.Parameters[0].Name);
                Assert.AreEqual("Int", memberModel.Parameters[0].Type);
                Assert.AreEqual(2, memberModel.MetaDatas.Count);
                Assert.AreEqual("author", memberModel.MetaDatas[0].Name);
                Assert.AreEqual("\"FlashDevelop\"", memberModel.MetaDatas[0].Params["Default"]);
                Assert.AreEqual("test", memberModel.MetaDatas[1].Name);
                Assert.IsNull(memberModel.MetaDatas[1].Params);

                classModel = model.Classes[1];
                Assert.AreEqual("MetaClass", classModel.Name);
                Assert.AreEqual(FlagType.Class, classModel.Flags & FlagType.Class);
                Assert.AreEqual(33, classModel.LineFrom);
                Assert.AreEqual(36, classModel.LineTo);
                Assert.AreEqual(0, classModel.Members.Count);
                Assert.AreEqual(2, classModel.MetaDatas.Count);
                Assert.AreEqual(":build", classModel.MetaDatas[0].Name);
                Assert.AreEqual("ResourceGenerator.build(\"resource/strings.json\")", classModel.MetaDatas[0].Params["Default"]);
                Assert.AreEqual(":build", classModel.MetaDatas[1].Name);
                Assert.AreEqual("TemplateBuilder.build('\r\n    <div class=\"mycomponent\"></div>')", classModel.MetaDatas[1].Params["Default"]);
            }

            [Test]
            public void ParseFile_WrongSyntaxCompilerMetaAfterMethodWithNoType()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("WrongSyntaxCompilerMetaAfterMethodWithNoType"));
                var classModel = model.Classes[0];
                Assert.AreEqual("WrongSyntaxMetadataTest", classModel.Name);
                Assert.AreEqual(FlagType.Class, classModel.Flags & FlagType.Class);
                Assert.AreEqual(2, classModel.LineFrom);
                //I'd say this should be possible
                //Assert.AreEqual(9, classModel.LineTo);
                Assert.AreEqual(2, classModel.Members.Count);

                var memberModel = classModel.Members[0];
                Assert.AreEqual("func", memberModel.Name);
                Assert.AreEqual(null, memberModel.Type);
                var flags = FlagType.Function;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Private, memberModel.Access & Visibility.Private);
                Assert.AreEqual(6, memberModel.LineFrom);
                Assert.AreEqual(8, memberModel.LineTo);
                Assert.AreEqual(" Dummy data to make sure this method keeps values at the end of the parsing ", memberModel.Comments);
                Assert.AreEqual("dummy", memberModel.MetaDatas[0].Name);
            }

            [Ignore("Not working for now")]
            public void ParseFile_WrongSyntaxCompilerMetaAfterVarWithNoType()
            {
                var model = ASContext.Context.GetCodeModel(ReadAllText("WrongSyntaxCompilerMetaAfterVarWithNoType"));
                var classModel = model.Classes[0];
                Assert.AreEqual("WrongSyntaxMetadataTest", classModel.Name);
                Assert.AreEqual(FlagType.Class, classModel.Flags & FlagType.Class);
                Assert.AreEqual(2, classModel.LineFrom);
                //I'd say this should be possible
                //Assert.AreEqual(9, classModel.LineTo);
                Assert.AreEqual(2, classModel.Members.Count);

                var memberModel = classModel.Members[0];
                Assert.AreEqual("func", memberModel.Name);
                Assert.AreEqual(null, memberModel.Type);
                var flags = FlagType.Variable;
                Assert.AreEqual(flags, memberModel.Flags & flags);
                Assert.AreEqual(Visibility.Private, memberModel.Access & Visibility.Private);
                Assert.AreEqual(6, memberModel.LineFrom);
                Assert.AreEqual(6, memberModel.LineTo);
                Assert.AreEqual(" Dummy data to make sure this method keeps values at the end of the parsing ", memberModel.Comments);
                Assert.AreEqual("dummy", memberModel.MetaDatas[0].Name);
            }

            static IEnumerable<TestCaseData> ParseClassTestCases
            {
                get
                {
                    yield return new TestCaseData("class Foo {}").Returns(new ClassModel {Name = "Foo", InFile = FileModel.Ignore });
                    yield return new TestCaseData("class Foo<T> {}").Returns(new ClassModel {Name = "Foo", InFile = FileModel.Ignore });
                    yield return new TestCaseData("private class Database_r<T> {}").Returns(new ClassModel {Name = "Database_r", InFile = FileModel.Ignore});
                    yield return new TestCaseData("private class Database_<T> {}").Returns(new ClassModel {Name = "Database_", InFile = FileModel.Ignore});
                }
            }

            [Test, TestCaseSource(nameof(ParseClassTestCases))]
            public ClassModel ParseClass(string sourceText)
            {
                var model = ASContext.Context.GetCodeModel(sourceText);
                var result = model.Classes.First();
                return result;
            }

            static IEnumerable<TestCaseData> ParseClassTestCases_issue1814
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("Issue1814"))
                        .Returns(9)
                        .SetName("Issue 1814. Case 0. ?");
                    yield return new TestCaseData(ReadAllText("Issue1814_case1"))
                        .Returns(9)
                        .SetName("Issue 1814. Case 1. return");
                    yield return new TestCaseData(ReadAllText("Issue1814_case2"))
                        .Returns(9)
                        .SetName("Issue 1814. Case 2. [");
                    yield return new TestCaseData(ReadAllText("Issue1814_case3"))
                        .Returns(9)
                        .SetName("Issue 1814. Case 3. :");
                    yield return new TestCaseData(ReadAllText("Issue1814_case4"))
                        .Returns(9)
                        .SetName("Issue 1814. Case 4. =");
                    yield return new TestCaseData(ReadAllText("Issue1814_case5"))
                        .Returns(9)
                        .SetName("Issue 1814. Case 5. in");
                    yield return new TestCaseData(ReadAllText("Issue1814_case6"))
                        .Returns(8)
                        .SetName("Issue 1814. Case 6. +");
                    yield return new TestCaseData(ReadAllText("Issue1814_case7"))
                        .Returns(8)
                        .SetName("Issue 1814. Case 7. =>");
                    yield return new TestCaseData(ReadAllText("Issue1814_case8"))
                        .Returns(8)
                        .SetName("Issue 1814. Case 8. -");
                    yield return new TestCaseData(ReadAllText("Issue1814_case9"))
                        .Returns(8)
                        .SetName("Issue 1814. Case 9. *");
                    yield return new TestCaseData(ReadAllText("Issue1814_case10"))
                        .Returns(8)
                        .SetName("Issue 1814. Case 10. /");
                    yield return new TestCaseData(ReadAllText("Issue1814_case11"))
                        .Returns(8)
                        .SetName("Issue 1814. Case 11. |");
                    yield return new TestCaseData(ReadAllText("Issue1814_case12"))
                        .Returns(8)
                        .SetName("Issue 1814. Case 12. &");
                    yield return new TestCaseData(ReadAllText("Issue1814_case13"))
                        .Returns(8)
                        .SetName("Issue 1814. Case 13. <");
                    yield return new TestCaseData(ReadAllText("Issue1814_case14"))
                        .Returns(8)
                        .SetName("Issue 1814. Case 14. ~");
                    yield return new TestCaseData(ReadAllText("Issue1814_case15"))
                        .Returns(8)
                        .SetName("Issue 1814. Case 15. ^");
                    yield return new TestCaseData(ReadAllText("Issue1814_case16"))
                        .Returns(8)
                        .SetName("Issue 1814. Case 16. !");
                    yield return new TestCaseData(ReadAllText("Issue1814_case17"))
                        .Returns(9)
                        .SetName("Issue 1814. Case 17. switch");
                    yield return new TestCaseData(ReadAllText("Issue1814_case18"))
                        .Returns(7)
                        .SetName("Issue 1814. Case 18. try");
                    yield return new TestCaseData(ReadAllText("Issue1814_case19"))
                        .Returns(7)
                        .SetName("Issue 1814. Case 19. function():Bool ~/regex/ ? true : false");
                    yield return new TestCaseData(ReadAllText("Issue1814_case20"))
                        .Returns(7)
                        .SetName("Issue 1814. Case 20. function():{v:Bool} ~/regex/ ? {v:true} : {v:false}");
                }
            }

            [Test, TestCaseSource(nameof(ParseClassTestCases_issue1814))]
            public int ParseFile_Issue1814(string sourceText)
            {
                return ASContext.Context.GetCodeModel(sourceText).Classes[0].LineTo;
            }

            static IEnumerable<TestCaseData> ParseClassTestCases_issue1921
            {
                get
                {
                    yield return new TestCaseData(ReadAllText("Issue1921_case1"))
                        .Returns(11)
                        .SetName("Issue 1921. Case 1")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1921");
                    yield return new TestCaseData(ReadAllText("Issue1921_case2"))
                        .Returns(11)
                        .SetName("Issue 1921. Case 2")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1921");
                    yield return new TestCaseData(ReadAllText("Issue1921_case3"))
                        .Returns(11)
                        .SetName("Issue 1921. Case 3")
                        .SetDescription("https://github.com/fdorg/flashdevelop/issues/1921");
                }
            }

            [Test, TestCaseSource(nameof(ParseClassTestCases_issue1921))]
            public int ParseFile_Issue1921(string sourceText)
            {
                return ASContext.Context.GetCodeModel(sourceText).Classes.First().LineTo;
            }

            [Test]
            public void ParseFile_Issue1964()
            {
                Assert.AreEqual("test.extern", ASContext.Context.GetCodeModel(ReadAllText("Issue1964_package_test.extern")).Package);
            }
        }
    }
}