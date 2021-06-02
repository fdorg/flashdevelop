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
                        new MemberModel("a", "int", FlagType.Access | FlagType.Dynamic | FlagType.Variable, Visibility.Public),
                    });
                yield return new TestCaseData("Issue3179_2", FlagType.Variable, true)
                    .Returns(new MemberList
                    {
                        new MemberModel("x", "Number", FlagType.Dynamic | FlagType.Variable, Visibility.Public),
                        new MemberModel("y", "Number", FlagType.Dynamic | FlagType.Variable, Visibility.Public),
                        new MemberModel("width", "Number", FlagType.Dynamic | FlagType.Variable, Visibility.Public),
                        new MemberModel("height", "Number", FlagType.Dynamic | FlagType.Variable, Visibility.Public),
                        new MemberModel("length", "int", FlagType.Static | FlagType.Constant | FlagType.Variable, Visibility.Public),
                        new MemberModel("prototype", "Object", FlagType.Dynamic | FlagType.Variable, Visibility.Public),
                    });
            }
        }

        [
            Test,
            TestCaseSource(nameof(SearchMemberTestCases))
        ]
        public MemberList SearchMember(string fileName, FlagType flags, bool recursive)
        {
            SetSrc(sci, ReadAllText(fileName));
            return ASContext.Context.CurrentClass.SearchMembers(flags, recursive);
        }
    }
}