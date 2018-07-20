using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Model;
using CodeRefactor.TestUtils;
using NUnit.Framework;
using PluginCore;

namespace CodeRefactor.Provider
{
    [TestFixture]
    class RefactoringHelperTests : ASCompleteTests
    {
        internal static string GetFullPathHaxe(string fileName) => $"{nameof(CodeRefactor)}.Test_Files.coderefactor.findallreferences.haxe.{fileName}.hx";

        internal static string ReadAllTextHaxe(string fileName) => TestFile.ReadAllText(GetFullPathHaxe(fileName));

        [TestFixture]
        class GetDefaultRefactorTargetTests : RefactoringHelperTests
        {
            static IEnumerable<TestCaseData> HaxeTestCases
            {
                get
                {
                    yield return
                        new TestCaseData(ReadAllTextHaxe("Constructor"))
                            .Returns(new Result(
                                new MemberModel
                                {
                                    Name = "Main",
                                    Flags = FlagType.Access | FlagType.Function | FlagType.Constructor
                                },
                                new ClassModel {Name = "Main", InFile = FileModel.Ignore})
                            );
                    yield return
                        new TestCaseData(ReadAllTextHaxe("ParameterVar"))
                            .Returns(new Result(
                                new MemberModel
                                {
                                    Name = "args",
                                    Type = "Array<Dynamic>",
                                    Flags = FlagType.Variable | FlagType.ParameterVar
                                },
                                new ClassModel {Name = "Array<Dynamic>", InFile = FileModel.Ignore})
                            );
                    yield return
                        new TestCaseData(ReadAllTextHaxe("LocalVar"))
                            .Returns(new Result(
                                new MemberModel
                                {
                                    Name = "a",
                                    Type = "Array<Dynamic>",
                                    Flags = FlagType.Inferred | FlagType.Dynamic | FlagType.Variable | FlagType.LocalVar
                                },
                                new ClassModel {Name = "Array<Dynamic>", InFile = FileModel.Ignore})
                            );
                }
            }

            [Test, TestCaseSource(nameof(HaxeTestCases))]
            public Result Common(string sourceText)
            {
                SetHaxeFeatures(sci);
                SetSrc(sci, sourceText);
                var target = RefactoringHelper.GetDefaultRefactorTarget();
                return new Result(target.IsPackage, target.Member, target.Type);
            }
        }

        [TestFixture]
        class GetDefaultRefactorTargetNameTests : RefactoringHelperTests
        {
            static IEnumerable<TestCaseData> HaxeTestCases
            {
                get
                {
                    yield return
                        new TestCaseData(ReadAllTextHaxe("LocalVar_enum"))
                            .Returns("v");
                }
            }

            [Test, TestCaseSource(nameof(HaxeTestCases))]
            public string Common(string sourceText)
            {
                SetHaxeFeatures(sci);
                SetSrc(sci, sourceText);
                var target = RefactoringHelper.GetDefaultRefactorTarget();
                return RefactoringHelper.GetRefactorTargetName(target);
            }
        }

        [TestFixture]
        class IsPrivateTargetTests : RefactoringHelperTests
        {
            static IEnumerable<TestCaseData> HaxeTestCases
            {
                get
                {
                    yield return
                        new TestCaseData(ReadAllTextHaxe("Constructor"), "3.4.2")
                            .Returns(false)
                            .SetName("Public constructor");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("PrivateVar"), "3.4.2")
                            .Returns(false)
                            .SetName("Private var");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("PublicVar"), "3.4.2")
                            .Returns(false)
                            .SetName("Public var");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("Type"), "3.4.2")
                            .Returns(false)
                            .SetName("Type");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("ParameterVar"), "3.4.2")
                            .Returns(true)
                            .SetName("Parameter of function");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("LocalVar"), "3.4.2")
                            .Returns(true)
                            .SetName("Local variable");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("LocalFunction"), "3.4.2")
                            .Returns(true)
                            .SetName("Local function");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("PrivateTypedef"), "3.4.2")
                            .Returns(true)
                            .SetName("Private typedef. SDK 3.4.2");
                    yield return
                        new TestCaseData(ReadAllTextHaxe("PrivateTypedef"), "4.0.0")
                            .Returns(false)
                            .SetName("Private typedef. SDK 4.0.0");
                }
            }

            [Test, TestCaseSource(nameof(HaxeTestCases))]
            public bool Haxe(string sourceText, string sdkVersion)
            {
                PluginBase.CurrentSDK = new InstalledSDK {Version = sdkVersion};
                SetHaxeFeatures(sci);
                return Common(sourceText);
            }

            bool Common(string sourceText)
            {
                SetSrc(sci, sourceText);
                var target = RefactoringHelper.GetDefaultRefactorTarget();
                return RefactoringHelper.IsPrivateTarget(target);
            }
        }
    }

    struct Result
    {
        public readonly bool IsPackage;
        public readonly MemberModel Member;
        public readonly ClassModel Type;

        public Result(ClassModel type) : this(false, null, type)
        {
        }

        public Result(MemberModel member, ClassModel type) : this(false, member, type)
        {
        }

        public Result(bool isPackage, MemberModel member, ClassModel type)
        {
            IsPackage = isPackage;
            Member = member;
            Type = type;
        }

        public bool Equals(Result other)
        {
            return IsPackage == other.IsPackage && Equals(Member, other.Member) && Equals(Type, other.Type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Result && Equals((Result) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = IsPackage.GetHashCode();
                hashCode = (hashCode * 397) ^ (Member != null ? Member.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Type != null ? Type.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString() => $"{nameof(IsPackage)}: {IsPackage}, {nameof(Member)}: {Member}, {nameof(Type)}: {Type}";
    } 
}
