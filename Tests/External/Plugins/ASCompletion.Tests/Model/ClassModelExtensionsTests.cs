using System.Collections.Generic;
using ASCompletion.Context;
using ASCompletion.TestUtils;
using NUnit.Framework;

namespace ASCompletion.Model
{
    [TestFixture]
    public class ClassModelExtensionsTests : ASCompletionTests
    {
        [OneTimeSetUp]
        public void Setup() => SetAs3Features(sci);

        static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

        static string GetFullPath(string fileName) => $"ASCompletion.Test_Files.model.{fileName}.as";

        static IEnumerable<TestCaseData> SearchMemberTestCases
        {
            get
            {
                yield return new TestCaseData("Issue3179_1", FlagType.Variable, false)
                    .Returns(new MemberList
                    {
                        new MemberModel("a", "int", FlagType.Access | FlagType.Dynamic | FlagType.Variable, Visibility.Private),
                    });
                yield return new TestCaseData("Issue3179_2", FlagType.Variable, true)
                    .Returns(new MemberList
                    {
                        new MemberModel("x", "int", FlagType.Variable, Visibility.Private),
                    });
            }
        }

        [
            Test,
            TestCaseSource(nameof(SearchMemberTestCases))
        ]
        public MemberList SearchMember(string fileName, FlagType flags, bool recursive)
        {
            var model = ASContext.Context.GetCodeModel(ReadAllText(fileName));
            return model.Classes[0].SearchMembers(flags, recursive);
        }
    }
}