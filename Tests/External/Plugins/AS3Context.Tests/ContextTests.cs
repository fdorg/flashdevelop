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

        [TestFixtureSetUp]
        public new void FixtureSetUp()
        {
            ASContext.Context.SetAs3Features();
            sci.ConfigurationLanguage = "as3";
        }

        IEnumerable<TestCaseData> DecomposeTypesTestCases
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
                yield return new TestCaseData(ReadAllText("IsImported_case1"))
                    .Returns(true);
                yield return new TestCaseData(null)
                    .Returns(false)
                    .SetName("ClassModel.VoidClass")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/1930");
            }
        }

        [Test, TestCaseSource(nameof(IsImportedTestCases))]
        public bool IsImported(string sourceText)
        {
            MemberModel member;
            if (sourceText != null)
            {
                SetSrc(sci, sourceText);
                var type = sci.GetWordFromPosition(sci.CurrentPos);
                member = new MemberModel(type, type, FlagType.Class, Visibility.Public);
            }
            else member = ClassModel.VoidClass;
            return ASContext.Context.IsImported(member, sci.CurrentLine);
        }

        IEnumerable<TestCaseData> ResolveTokenTestCases
        {
            get
            {
                yield return new TestCaseData("true")
                    .Returns(new ClassModel {Name = "Boolean", Type = "Boolean", InFile = FileModel.Ignore})
                    .SetName("true");
                yield return new TestCaseData("false")
                    .Returns(new ClassModel {Name = "Boolean", Type = "Boolean", InFile = FileModel.Ignore})
                    .SetName("false");
                yield return new TestCaseData("[]")
                    .Returns(new ClassModel {Name = "Array", Type = "Array", InFile = FileModel.Ignore})
                    .SetName("[]");
                yield return new TestCaseData("{}")
                    .Returns(new ClassModel {Name = "Object", Type = "Object", InFile = FileModel.Ignore})
                    .SetName("{}");
                yield return new TestCaseData("10")
                    .Returns(new ClassModel {Name = "Number", Type = "Number", InFile = FileModel.Ignore})
                    .SetName("10");
                yield return new TestCaseData("-10")
                    .Returns(new ClassModel {Name = "Number", Type = "Number", InFile = FileModel.Ignore})
                    .SetName("-10");
                yield return new TestCaseData("\"\"")
                    .Returns(new ClassModel {Name = "String", Type = "String", InFile = FileModel.Ignore})
                    .SetName("\"\"");
                yield return new TestCaseData("''")
                    .Returns(new ClassModel {Name = "String", Type = "String", InFile = FileModel.Ignore})
                    .SetName("''");
                yield return new TestCaseData("</>")
                    .Returns(new ClassModel {Name = "XML", Type = "XML", InFile = FileModel.Ignore})
                    .SetName("</>");
                yield return new TestCaseData("0xFF0000")
                    .Returns(new ClassModel {Name = "uint", Type = "uint", InFile = FileModel.Ignore})
                    .SetName("0xFF0000");
            }
        }

        [Test, TestCaseSource(nameof(ResolveTokenTestCases))]
        public ClassModel ResolveToken(string token) => ASContext.Context.ResolveToken(token, null);
    }
}
