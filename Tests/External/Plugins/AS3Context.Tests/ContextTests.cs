using System.Collections.Generic;
using AS3Context.TestUtils;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.TestUtils;
using NUnit.Framework;

namespace AS3Context
{
    [TestFixture]
    class ContextTests : ASCompleteTests
    {
        protected static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

        protected static string GetFullPath(string fileName) => $"{nameof(AS3Context)}.Test_Files.parser.{fileName}.as";

        [OneTimeSetUp]
        public new void FixtureSetUp()
        {
            ASContext.Context.SetAs3Features();
            sci.ConfigurationLanguage = "as3";
        }

        static IEnumerable<TestCaseData> DecomposeTypesTestCases
        {
            get
            {
                yield return new TestCaseData(new List<string> {null})
                    .SetName("null")
                    .Returns(new [] {"*"});
                yield return new TestCaseData(new List<string> {string.Empty})
                    .SetName("")
                    .Returns(new [] {"*"});
                yield return new TestCaseData(new List<string> {"int"})
                    .SetName("int")
                    .Returns(new [] {"int"});
                yield return new TestCaseData(new List<string> {"Vector.<int>"})
                    .SetName("Vector.<int>")
                    .Returns(new [] {"Vector", "int"});
                yield return new TestCaseData(new List<string> {"Vector.<Vector.<int>>"})
                    .SetName("Vector.<Vector.<int>>")
                    .Returns(new[] {"Vector", "int"});
            }
        }

        [Test, TestCaseSource(nameof(DecomposeTypesTestCases))]
        public IEnumerable<string> DecomposeTypes(IEnumerable<string> types) => ASContext.Context.DecomposeTypes(types);

        static IEnumerable<TestCaseData> IsImportedTestCases
        {
            get
            {
                yield return new TestCaseData("IsImported_case1")
                    .Returns(true);
                yield return new TestCaseData(null)
                    .Returns(false)
                    .SetName("ClassModel.VoidClass")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1930");
            }
        }

        [Test, TestCaseSource(nameof(IsImportedTestCases))]
        public bool IsImported(string fileName)
        {
            MemberModel member;
            if (fileName != null)
            {
                var sourceText = ReadAllText(fileName);
                SetSrc(sci, sourceText);
                var expr = ASComplete.GetExpressionType(sci, ASComplete.ExpressionEndPosition(sci, sci.CurrentPos), false, true);
                if (expr.Type != null) member = expr.Type;
                else
                {
                    var type = sci.GetWordFromPosition(sci.CurrentPos);
                    member = ASContext.Context.ResolveType(type, ASContext.Context.CurrentModel);
                }
            }
            else member = ClassModel.VoidClass;
            return ASContext.Context.IsImported(member, sci.CurrentLine);
        }

        static IEnumerable<TestCaseData> ResolveTokenTestCases
        {
            get
            {
                yield return new TestCaseData("true")
                    .Returns(new ClassModel {Name = "Boolean", Type = "Boolean", InFile = FileModel.Ignore});
                yield return new TestCaseData("false")
                    .Returns(new ClassModel {Name = "Boolean", Type = "Boolean", InFile = FileModel.Ignore});
                yield return new TestCaseData("[]")
                    .Returns(new ClassModel {Name = "Array", Type = "Array", InFile = FileModel.Ignore});
                yield return new TestCaseData("{}")
                    .Returns(new ClassModel {Name = "Object", Type = "Object", InFile = FileModel.Ignore});
                yield return new TestCaseData("10")
                    .Returns(new ClassModel {Name = "int", Type = "int", InFile = FileModel.Ignore});
                yield return new TestCaseData("-10")
                    .Returns(new ClassModel {Name = "int", Type = "int", InFile = FileModel.Ignore});
                yield return new TestCaseData("10.0")
                    .Returns(new ClassModel {Name = "Number", Type = "Number", InFile = FileModel.Ignore});
                yield return new TestCaseData("-10.0")
                    .Returns(new ClassModel {Name = "Number", Type = "Number", InFile = FileModel.Ignore});
                yield return new TestCaseData("5e-324")
                    .Returns(new ClassModel {Name = "Number", Type = "Number", InFile = FileModel.Ignore});
                yield return new TestCaseData("\"\"")
                    .Returns(new ClassModel {Name = "String", Type = "String", InFile = FileModel.Ignore});
                yield return new TestCaseData("\"")
                    .Returns(ClassModel.VoidClass);
                yield return new TestCaseData("''")
                    .Returns(new ClassModel {Name = "String", Type = "String", InFile = FileModel.Ignore});
                yield return new TestCaseData("'")
                    .Returns(ClassModel.VoidClass);
                yield return new TestCaseData("</>")
                    .Returns(new ClassModel {Name = "XML", Type = "XML", InFile = FileModel.Ignore});
                yield return new TestCaseData("0xFF0000")
                    .Returns(new ClassModel {Name = "uint", Type = "uint", InFile = FileModel.Ignore});
                yield return new TestCaseData("new Sprite().addChild(new Sprite())")
                    .Returns(ClassModel.VoidClass);
                yield return new TestCaseData("new String")
                    .Returns(new ClassModel {Name = "String", Type = "String", InFile = FileModel.Ignore});
                yield return new TestCaseData("(' 1 '   is String)")
                    .Returns(new ClassModel {Name = "Boolean", Type = "Boolean", InFile = FileModel.Ignore});
                yield return new TestCaseData("(' 1 '   as String)")
                    .Returns(new ClassModel {Name = "String", Type = "String", InFile = FileModel.Ignore});
            }
        }

        [Test, TestCaseSource(nameof(ResolveTokenTestCases))]
        public ClassModel ResolveToken(string token) => ASContext.Context.ResolveToken(token, null);

        static IEnumerable<TestCaseData> BraceMatchIssue2855
        {
            get
            {
                yield return new TestCaseData("a >>$(EntryPoint) b. Issue 2855. Case 1")
                    .Returns(-1)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2855");
                yield return new TestCaseData("a >$(EntryPoint)> b. Issue 2855. Case 2")
                    .Returns(-1)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2855");
                yield return new TestCaseData("a <<$(EntryPoint) b. Issue 2855. Case 3")
                    .Returns(-1)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2855");
                yield return new TestCaseData("a <$(EntryPoint)< b. Issue 2855. Case 4")
                    .Returns(-1)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2855");
                yield return new TestCaseData("new Vector.<int$(EntryPoint)>. Issue 2855. Case 5")
                    .Returns("new Vector.".Length)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2855");
                yield return new TestCaseData("new Vector.<int>$(EntryPoint). Issue 2855. Case 6")
                    .Returns(-1)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2855");
                yield return new TestCaseData("new Vector.<Vector.<int>$(EntryPoint)>. Issue 2855. Case 7")
                    .Returns("new Vector.".Length)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2855");
                yield return new TestCaseData("new Vector.<Vector.<int$(EntryPoint)>>. Issue 2855. Case 8")
                    .Returns("new Vector.<Vector.".Length)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2855");
                yield return new TestCaseData("\"new Vector.<Vector.<int$(EntryPoint)>>\". Issue 2855. Case 9")
                    .Returns(-1)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2855");
                yield return new TestCaseData("new Vector.<Vector.</*<uint*/int$(EntryPoint)>>. Issue 2855. Case 10")
                    .Returns("new Vector.<Vector.".Length)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2855");
                yield return new TestCaseData("/*<$(EntryPoint)>*/. Issue 2855. Case 11")
                    .Returns(-1)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2855");
                yield return new TestCaseData("a < b\na $(EntryPoint)> b. Issue 2855. Case 12")
                    .Returns(-1)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2855");
                yield return new TestCaseData("new <int$(EntryPoint)>[1,2,3]. Issue 2855. Case 13")
                    .Returns("new ".Length)
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2855");
            }
        }

        [Test, TestCaseSource(nameof(BraceMatchIssue2855))]
        public int BraceMatch(string sourceText)
        {
            SetSrc(sci, sourceText);
            var result = ((Context)ASContext.GetLanguageContext("as3")).BraceMatch(sci, sci.CurrentPos);
            return result;
        }
    }
}