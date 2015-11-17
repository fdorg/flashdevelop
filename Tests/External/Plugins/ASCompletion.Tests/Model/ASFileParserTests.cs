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
        public void ParseFile_As3SimpleClass()
        {
            using (var resourceFile = new TestFile("ASCompletion.Test_Files.as3.SimpleClassTest.as"))
            {
                var srcModel = new FileModel(resourceFile.DestinationFile);
                srcModel.Context = new AS3Context.Context();
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

        [Test]
        public void ParseFile_HaxeSimpleClass()
        {
            using (var resourceFile = new TestFile("ASCompletion.Test_Files.haxe.SimpleClassTest.hx"))
            {
                var srcModel = new FileModel(resourceFile.DestinationFile);
                srcModel.Context = new HaXeContext.Context(new HaXeContext.HaXeSettings());
                var model = ASFileParser.ParseFile(srcModel);
                Assert.AreEqual(4, model.Version);
                Assert.IsTrue(model.HasPackage);
                Assert.AreEqual("test.test", model.FullPackage);
                Assert.AreEqual(1, model.Classes.Count);
                var classModel = model.Classes[0];
                Assert.AreEqual("Test", classModel.Name);
                Assert.AreEqual("test.test.Test", classModel.QualifiedName);
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

        [Test(Description = "Commit 7c8718c")]
        public void ParseFile_HaxeOverrideFunction()
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

        [Test(Description = "Commit 7c8718c")]
        public void ParseFile_As3OverrideFunction()
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

        [Test(Description = "Includes Commit 304ca93")]
        public void ParseFile_HaxeTypeDefs()
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

        [Test(Description = "Includes Commit 51938e0")]
        public void ParseFile_HaxeGenerics()
        {
            using (var resourceFile = new TestFile("ASCompletion.Test_Files.haxe.GenericsTest.hx"))
            {
                var srcModel = new FileModel(resourceFile.DestinationFile);
                srcModel.Context = new HaXeContext.Context(new HaXeContext.HaXeSettings());
                var model = ASFileParser.ParseFile(srcModel);
                Assert.AreEqual(5, model.Classes.Count);

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

                var objectConstraintGeneric = model.Classes[3];
                Assert.AreEqual(35, objectConstraintGeneric.LineFrom);
                Assert.AreEqual(44, objectConstraintGeneric.LineTo);
                Assert.AreEqual(FlagType.Class, objectConstraintGeneric.Flags & FlagType.Class);
                Assert.AreEqual("TestObjectConstraint<T:({},Measurable)>", objectConstraintGeneric.FullName);
                Assert.AreEqual("TestObjectConstraint", objectConstraintGeneric.Name);
                Assert.AreEqual("<T:({},Measurable)>", objectConstraintGeneric.Template);
                Assert.AreEqual(2, objectConstraintGeneric.Members.Count);
                member = objectConstraintGeneric.Members[0];
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
                Assert.AreEqual("T", arg.Type);
                member = objectConstraintGeneric.Members[1];
                Assert.AreEqual("test2<K:({},Measurable)>", member.FullName);
                Assert.AreEqual("test2", member.Name);
                Assert.AreEqual("<K:({},Measurable)>", member.Template);
                Assert.AreEqual(41, member.LineFrom);
                Assert.AreEqual(43, member.LineTo);
                Assert.AreEqual("K", member.Type);
                Assert.AreEqual(FlagType.Function, member.Flags & FlagType.Function);
                arg = member.Parameters[0];
                Assert.AreEqual("expected", arg.Name);
                Assert.AreEqual("K", arg.Type);
                arg = member.Parameters[1];
                Assert.AreEqual("actual", arg.Name);
                Assert.AreEqual("K", arg.Type);

                var fullConstraintGeneric = model.Classes[4];
                Assert.AreEqual(47, fullConstraintGeneric.LineFrom);
                Assert.AreEqual(56, fullConstraintGeneric.LineTo);
                Assert.AreEqual(FlagType.Class, fullConstraintGeneric.Flags & FlagType.Class);
                Assert.AreEqual("TestFullConstraint<T:({},Measurable),Z:(Iterable<String>,Measurable)>", fullConstraintGeneric.FullName);
                Assert.AreEqual("TestFullConstraint", fullConstraintGeneric.Name);
                Assert.AreEqual("<T:({},Measurable),Z:(Iterable<String>,Measurable)>", fullConstraintGeneric.Template);
                Assert.AreEqual(2, fullConstraintGeneric.Members.Count);
                member = fullConstraintGeneric.Members[0];
                Assert.AreEqual("test1", member.Name);
                Assert.AreEqual(49, member.LineFrom);
                Assert.AreEqual(51, member.LineTo);
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
                Assert.AreEqual(53, member.LineFrom);
                Assert.AreEqual(55, member.LineTo);
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
    }
}
