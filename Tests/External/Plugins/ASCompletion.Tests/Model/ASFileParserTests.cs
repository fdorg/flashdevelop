using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ASCompletion.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace ASCompletion.Model
{
    [TestFixture]
    class ASFileParserTests
    {
        public class As3
        {
            [Test]
            public void ParseFile_SimpleClass()
            {
                using (var resourceFile = new TestFile("ASCompletion.Test_Files.as3.SimpleClassTest.as"))
                {
                    var srcModel = new FileModel(resourceFile.DestinationFile);
                    srcModel.Context = new AS3Context.Context();
                    var model = ASFileParser.ParseFile(srcModel);
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
            }

            [Test(Description = "Commit 7c8718c")]
            public void ParseFile_OverrideFunction()
            {
                using (var resourceFile = new TestFile("ASCompletion.Test_Files.as3.OverrideFunctionTest.as"))
                {
                    var srcModel = new FileModel(resourceFile.DestinationFile);
                    srcModel.Context = new AS3Context.Context();
                    var model = ASFileParser.ParseFile(srcModel);
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
            }

        }

        public class Haxe
        {
            [Test]
            public void ParseFile_SimpleClass()
            {
                using (var resourceFile = new TestFile("ASCompletion.Test_Files.haxe.SimpleClassTest.hx"))
                {
                    var srcModel = new FileModel(resourceFile.DestinationFile);
                    srcModel.Context = new HaXeContext.Context(new HaXeContext.HaXeSettings());
                    var model = ASFileParser.ParseFile(srcModel);
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
            }

            [Test]
            public void ParseFile_PrivateClass()
            {
                using (var resourceFile = new TestFile("ASCompletion.Test_Files.haxe.PrivateClassTest.hx"))
                {
                    var srcModel = new FileModel(resourceFile.DestinationFile);
                    srcModel.Context = new HaXeContext.Context(new HaXeContext.HaXeSettings());
                    var model = ASFileParser.ParseFile(srcModel);
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
            }

            [Test]
            public void ParseFile_Interface()
            {
                using (var resourceFile = new TestFile("ASCompletion.Test_Files.haxe.InterfaceTest.hx"))
                {
                    var srcModel = new FileModel(resourceFile.DestinationFile);
                    srcModel.Context = new HaXeContext.Context(new HaXeContext.HaXeSettings());
                    var model = ASFileParser.ParseFile(srcModel);

                    Assert.AreEqual(1, model.Classes.Count);
                    var classModel = model.Classes[0];
                    Assert.AreEqual("Test", classModel.Name);
                    Assert.AreEqual(Visibility.Public, classModel.Access);
                    Assert.AreEqual(FlagType.Interface, classModel.Flags & FlagType.Interface);
                    Assert.AreEqual(4, classModel.Members.Count);

                    var member = classModel.Members[0];
                    Assert.AreEqual("test", member.Name);
                    Assert.AreEqual("Int", member.Type);
                    Assert.IsNull(member.Parameters);
                    Assert.AreEqual(Visibility.Public, member.Access);
                    Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);

                    member = classModel.Members[1];
                    Assert.AreEqual("testVar", member.Name);
                    Assert.AreEqual("String", member.Type);
                    Assert.IsNull(member.Parameters);
                    Assert.AreEqual(Visibility.Public, member.Access);
                    Assert.AreEqual(FlagType.Variable, member.Flags & FlagType.Variable);

                    member = classModel.Members[2];
                    Assert.AreEqual("testPrivate", member.Name);
                    Assert.AreEqual("Int", member.Type);
                    Assert.IsNull(member.Parameters);
                    Assert.AreEqual(Visibility.Private, member.Access);
                    Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);

                    member = classModel.Members[3];
                    Assert.AreEqual("testProperty", member.Name);
                    Assert.AreEqual("Float", member.Type);
                    Assert.AreEqual(2, member.Parameters.Count);
                    Assert.AreEqual(Visibility.Public, member.Access);
                    Assert.AreEqual(FlagType.Getter, member.Flags & FlagType.Getter);
                    Assert.AreEqual("get", member.Parameters[0].Name);
                    Assert.AreEqual("set", member.Parameters[1].Name);
                }
            }

            [Test(Description = "Commit 7c8718c")]
            public void ParseFile_OverrideFunction()
            {
                using (var resourceFile = new TestFile("ASCompletion.Test_Files.haxe.OverrideFunctionTest.hx"))
                {
                    var srcModel = new FileModel(resourceFile.DestinationFile);
                    srcModel.Context = new HaXeContext.Context(new HaXeContext.HaXeSettings());
                    var model = ASFileParser.ParseFile(srcModel);
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
            }

            [Test(Description = "Includes Commit 304ca93")]
            public void ParseFile_TypeDefs()
            {
                using (var resourceFile = new TestFile("ASCompletion.Test_Files.haxe.TypeDefsTest.hx"))
                {
                    var srcModel = new FileModel(resourceFile.DestinationFile);
                    srcModel.Context = new HaXeContext.Context(new HaXeContext.HaXeSettings());
                    var model = ASFileParser.ParseFile(srcModel);
                    Assert.AreEqual(7, model.Classes.Count);

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
                }
            }

            [Test]
            public void ParseFile_Enums()
            {
                using (var resourceFile = new TestFile("ASCompletion.Test_Files.haxe.EnumsTest.hx"))
                {
                    var srcModel = new FileModel(resourceFile.DestinationFile);
                    srcModel.Context = new HaXeContext.Context(new HaXeContext.HaXeSettings());
                    var model = ASFileParser.ParseFile(srcModel);
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
            }

            [Test(Description = "Includes Commit 51938e0")]
            public void ParseFile_Generics()
            {
                using (var resourceFile = new TestFile("ASCompletion.Test_Files.haxe.GenericsTest.hx"))
                {
                    var srcModel = new FileModel(resourceFile.DestinationFile);
                    srcModel.Context = new HaXeContext.Context(new HaXeContext.HaXeSettings());
                    var model = ASFileParser.ParseFile(srcModel);
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
            }
            [Ignore("Easy fix, to add")]
            public void ParseFile_GenericsWithObjectConstraints()
            {
                using (var resourceFile = new TestFile("ASCompletion.Test_Files.haxe.GenericsObjectConstraintTest.hx"))
                {
                    var srcModel = new FileModel(resourceFile.DestinationFile);
                    srcModel.Context = new HaXeContext.Context(new HaXeContext.HaXeSettings());
                    var model = ASFileParser.ParseFile(srcModel);
                    Assert.AreEqual(2, model.Classes.Count);

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
                    var simpleGeneric = model.Classes[0];
                    Assert.AreEqual(2, simpleGeneric.LineFrom);
                    Assert.AreEqual(11, simpleGeneric.LineTo);
                    Assert.AreEqual(FlagType.Class, simpleGeneric.Flags & FlagType.Class);
                    Assert.AreEqual("Test<T>", simpleGeneric.FullName);
                    Assert.AreEqual("Test", simpleGeneric.Name);
                    Assert.AreEqual("<T>", simpleGeneric.Template);
                    Assert.AreEqual(2, simpleGeneric.Members.Count);

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
                }
            }

            [Test(Description = "PR 680")]
            public void ParseFile_RegExLiterals()
            {
                using (var resourceFile = new TestFile("ASCompletion.Test_Files.haxe.RegExTest.hx"))
                {
                    var plugin = Substitute.For<PluginMain>();
                    plugin.MenuItems.Returns(new List<ToolStripItem>());
                    var context = new HaXeContext.Context(new HaXeContext.HaXeSettings());
                    Context.ASContext.GlobalInit(plugin);
                    Context.ASContext.Context = context;
                    var model = context.GetCodeModel(File.ReadAllText(resourceFile.DestinationFile));

                    Assert.AreEqual(4, model.Members.Count);  // First member = function itself

                    var regExMember = model.Members[1];
                    Assert.AreEqual("regExInArray", regExMember.Name);
                    Assert.AreEqual("Array<EReg>", regExMember.Type);
                    regExMember = model.Members[2];
                    Assert.AreEqual("regExInObject", regExMember.Name);
                    regExMember = model.Members[3];
                    Assert.AreEqual("regEx", regExMember.Name);
                    Assert.AreEqual("EReg", regExMember.Type);
                }
            }

            [Test(Description = "PR 680")]
            public void ParseFile_MultiLineStrings()
            {
                using (var resourceFile = new TestFile("ASCompletion.Test_Files.haxe.MultiLineStringsTest.hx"))
                {
                    var srcModel = new FileModel(resourceFile.DestinationFile);
                    srcModel.Context = new HaXeContext.Context(new HaXeContext.HaXeSettings());
                    var model = ASFileParser.ParseFile(srcModel);

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
            }

            [Test(Description = "PR 680")]
            public void ParseFile_StringWithEscapedChars()
            {
                using (var resourceFile = new TestFile("ASCompletion.Test_Files.haxe.EscapedStringsTest.hx"))
                {
                    var srcModel = new FileModel(resourceFile.DestinationFile);
                    srcModel.Context = new HaXeContext.Context(new HaXeContext.HaXeSettings());
                    var model = ASFileParser.ParseFile(srcModel);

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
            }

            [Test]
            public void ParseFile_Imports()
            {
                using (var resourceFile = new TestFile("ASCompletion.Test_Files.haxe.ImportTest.hx"))
                {
                    var srcModel = new FileModel(resourceFile.DestinationFile);
                    srcModel.Context = new HaXeContext.Context(new HaXeContext.HaXeSettings());
                    var model = ASFileParser.ParseFile(srcModel);

                    Assert.AreEqual(4, model.Imports.Count);  // import * ignored

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
            }

            [Ignore("Not supported, for now we hope!")]
            public void ParseFile_ImportAliases()
            {
                using (var resourceFile = new TestFile("ASCompletion.Test_Files.haxe.ImportAliasTest.hx"))
                {
                    var srcModel = new FileModel(resourceFile.DestinationFile);
                    srcModel.Context = new HaXeContext.Context(new HaXeContext.HaXeSettings());
                    var model = ASFileParser.ParseFile(srcModel);

                    Assert.AreEqual(2, model.Imports.Count);
                }
            }

            [Test]
            public void ParseFile_ComplexClass()
            {
                using (var resourceFile = new TestFile("ASCompletion.Test_Files.haxe.ComplexClassTest.hx"))
                {
                    var srcModel = new FileModel(resourceFile.DestinationFile);
                    srcModel.Context = new HaXeContext.Context(new HaXeContext.HaXeSettings());
                    var model = ASFileParser.ParseFile(srcModel);
                    var classModel = model.Classes[0];
                    Assert.AreEqual("Test", classModel.Name);
                    Assert.AreEqual(FlagType.Class, classModel.Flags & FlagType.Class);
                    Assert.AreEqual(2, classModel.LineFrom);
                    Assert.AreEqual(45, classModel.LineTo);
                    Assert.AreEqual(14, classModel.Members.Count);

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
                }
            }
        }

    }
}
