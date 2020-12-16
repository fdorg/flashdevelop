using System.Collections.Generic;
using NUnit.Framework;

namespace ASCompletion.Model
{
    [TestFixture]
    class CaseSensitiveImportComparerTests
    {
        static IEnumerable<TestCaseData> GetPackageTypeSeparationTestCases
        {
            get
            {
                yield return new TestCaseData("a.b.C").Returns(3);
                yield return new TestCaseData("a.b.c").Returns(3);
                yield return new TestCaseData("a.b.C.D").Returns(3);
                yield return new TestCaseData("a").Returns(-1);
                yield return new TestCaseData(".a").Returns(-1);
                yield return new TestCaseData("a.").Returns(-1);
                yield return new TestCaseData("a.b.c.").Returns(3);
            }
        }

        [
            Test,
            TestCaseSource(nameof(GetPackageTypeSeparationTestCases))
        ]
        public int GetPackageTypeSeparation(string import) => CaseSensitiveImportComparer.GetPackageTypeSeparation(import);

        static IEnumerable<TestCaseData> CompareImportsTests
        {
            get
            {
                yield return new TestCaseData("a", "A").Returns(32);
                yield return new TestCaseData("a", "b").Returns(-1);
                yield return new TestCaseData("b", "a").Returns(1);
                yield return new TestCaseData("a", "a").Returns(0);
                yield return new TestCaseData("a.A", "b").Returns(1);
                yield return new TestCaseData("a", "b.B").Returns(-1);
                yield return new TestCaseData("a.A", "b.A").Returns(-1);
                yield return new TestCaseData("b.A", "a.A").Returns(1);
                yield return new TestCaseData("a.A", "a.A").Returns(0);
                yield return new TestCaseData("a.A", "a.B").Returns(-1);
                yield return new TestCaseData("b.A", "a.A").Returns(1);
                yield return new TestCaseData("a.A", "a.a").Returns(-32);
                yield return new TestCaseData("a.MathReal", "a.Mathematics").Returns(-19);
            }
        }

        [
            Test,
            TestCaseSource(nameof(CompareImportsTests))
        ]
        public int CompareImports(string import, string import2) => CaseSensitiveImportComparer.CompareImports(import, import2);
    }

    [TestFixture]
    class ByKindMemberComparerTests
    {
        static IEnumerable<TestCaseData> GetPriorityTests
        {
            get
            {
                yield return new TestCaseData(FlagType.Constant).Returns(4);
                yield return new TestCaseData(FlagType.Variable).Returns(3);
                yield return new TestCaseData(FlagType.Final | FlagType.Variable).Returns(3);
                yield return new TestCaseData(FlagType.Getter | FlagType.Setter).Returns(2);
                yield return new TestCaseData(FlagType.Getter).Returns(2);
                yield return new TestCaseData(FlagType.Setter).Returns(2);
                yield return new TestCaseData(FlagType.Function).Returns(1);
                yield return new TestCaseData(FlagType.Constructor).Returns(1);
            }
        }

        [
            Test,
            TestCaseSource(nameof(GetPriorityTests))
        ]
        public uint GetPriority(FlagType flag) => ByKindMemberComparer.GetPriority(flag);
    }

    [TestFixture]
    class SmartMemberComparerTests
    {
        static IEnumerable<TestCaseData> GetPriorityTests
        {
            get
            {
                yield return new TestCaseData(new MemberModel{Access = Visibility.Public})
                    .SetName(Visibility.Public.ToString())
                    .Returns(11);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Protected})
                    .SetName(Visibility.Protected.ToString())
                    .Returns(12);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Private})
                    .SetName(Visibility.Private.ToString())
                    .Returns(13);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Internal})
                    .SetName(Visibility.Internal.ToString())
                    .Returns(14);
                yield return new TestCaseData(new MemberModel{Flags = FlagType.Constant})
                    .SetName(FlagType.Constant.ToString())
                    .Returns(54);
                yield return new TestCaseData(new MemberModel{Flags = FlagType.Variable})
                    .SetName(FlagType.Variable.ToString())
                    .Returns(34);
                yield return new TestCaseData(new MemberModel{Flags = FlagType.Getter})
                    .SetName(FlagType.Getter.ToString())
                    .Returns(34);
                yield return new TestCaseData(new MemberModel{Flags = FlagType.Setter})
                    .SetName(FlagType.Setter.ToString())
                    .Returns(34);
                yield return new TestCaseData(new MemberModel{Flags = FlagType.Getter | FlagType.Setter})
                    .SetName((FlagType.Getter | FlagType.Setter).ToString())
                    .Returns(34);
                yield return new TestCaseData(new MemberModel{Flags = FlagType.Constructor})
                    .SetName(FlagType.Constructor.ToString())
                    .Returns(24);
                yield return new TestCaseData(new MemberModel()).Returns(14);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Public, Flags = FlagType.Constant})
                    .SetName($"{Visibility.Public}, {FlagType.Constant}")
                    .Returns(51);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Public, Flags = FlagType.Variable})
                    .SetName($"{Visibility.Public}, {FlagType.Variable}")
                    .Returns(31);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Public, Flags = FlagType.Getter})
                    .SetName($"{Visibility.Public}, {FlagType.Getter}")
                    .Returns(31);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Public, Flags = FlagType.Setter})
                    .SetName($"{Visibility.Public}, {FlagType.Setter}")
                    .Returns(31);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Public, Flags = FlagType.Getter | FlagType.Setter})
                    .SetName($"{Visibility.Public}, {FlagType.Getter | FlagType.Setter}")
                    .Returns(31);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Public, Flags = FlagType.Constructor})
                    .SetName($"{Visibility.Public}, {FlagType.Constructor}")
                    .Returns(21);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Protected, Flags = FlagType.Constant})
                    .SetName($"{Visibility.Protected}, {FlagType.Constant}")
                    .Returns(52);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Protected, Flags = FlagType.Variable})
                    .SetName($"{Visibility.Protected}, {FlagType.Variable}")
                    .Returns(32);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Protected, Flags = FlagType.Getter})
                    .SetName($"{Visibility.Protected}, {FlagType.Getter}")
                    .Returns(32);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Protected, Flags = FlagType.Setter})
                    .SetName($"{Visibility.Protected}, {FlagType.Setter}")
                    .Returns(32);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Protected, Flags = FlagType.Getter | FlagType.Setter})
                    .SetName($"{Visibility.Protected}, {FlagType.Getter | FlagType.Setter}")
                    .Returns(32);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Protected, Flags = FlagType.Constructor})
                    .SetName($"{Visibility.Protected}, {FlagType.Constructor}")
                    .Returns(22);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Private, Flags = FlagType.Constant})
                    .SetName($"{Visibility.Private}, {FlagType.Constant}")
                    .Returns(53);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Private, Flags = FlagType.Variable})
                    .SetName($"{Visibility.Private}, {FlagType.Variable}")
                    .Returns(33);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Private, Flags = FlagType.Getter})
                    .SetName($"{Visibility.Private}, {FlagType.Getter}")
                    .Returns(33);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Private, Flags = FlagType.Setter})
                    .SetName($"{Visibility.Private}, {FlagType.Setter}")
                    .Returns(33);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Private, Flags = FlagType.Getter | FlagType.Setter})
                    .SetName($"{Visibility.Private}, {FlagType.Getter | FlagType.Setter}")
                    .Returns(33);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Private, Flags = FlagType.Constructor})
                    .SetName($"{Visibility.Private}, {FlagType.Constructor}")
                    .Returns(23);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Internal, Flags = FlagType.Constant})
                    .SetName($"{Visibility.Internal}, {FlagType.Constant}")
                    .Returns(54);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Internal, Flags = FlagType.Variable})
                    .SetName($"{Visibility.Internal}, {FlagType.Variable}")
                    .Returns(34);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Internal, Flags = FlagType.Getter})
                    .SetName($"{Visibility.Internal}, {FlagType.Getter}")
                    .Returns(34);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Internal, Flags = FlagType.Setter})
                    .SetName($"{Visibility.Internal}, {FlagType.Setter}")
                    .Returns(34);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Internal, Flags = FlagType.Getter | FlagType.Setter})
                    .SetName($"{Visibility.Internal}, {FlagType.Getter | FlagType.Setter}")
                    .Returns(34);
                yield return new TestCaseData(new MemberModel{Access = Visibility.Internal, Flags = FlagType.Constructor})
                    .SetName($"{Visibility.Internal}, {FlagType.Constructor}")
                    .Returns(24);
            }
        }

        [
            Test,
            TestCaseSource(nameof(GetPriorityTests))
        ]
        public uint GetPriority(MemberModel model) => SmartMemberComparer.GetPriority(model);
    }
}