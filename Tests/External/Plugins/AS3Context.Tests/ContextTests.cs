using System.Collections.Generic;
using ASCompletion.Completion;
using NUnit.Framework;

namespace AS3Context
{
    [TestFixture]
    class ContextTests : ASCompleteTests
    {
        Context context;

        [TestFixtureSetUp]
        public new void FixtureSetUp() => context = new Context(new AS3Settings());

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
        public IEnumerable<string> DecomposeTypes(IEnumerable<string> types) => context.DecomposeTypes(types);
    }
}
