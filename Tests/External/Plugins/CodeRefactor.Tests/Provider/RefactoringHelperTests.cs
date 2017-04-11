﻿using System.Collections.Generic;
using System.Linq;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.TestUtils;
using CodeRefactor.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PluginCore.Helpers;

namespace CodeRefactor.Provider
{
    class GetDefaultRefactorTargetTests : CodeRefactorTests
    {
        internal static string GetFullPathHaxe(string fileName) => $"{nameof(CodeRefactor)}.Test_Files.coderefactor.findallreferences.haxe.{fileName}.hx";

        internal static string ReadAllTextHaxe(string fileName) => TestFile.ReadAllText(GetFullPathHaxe(fileName));

        IEnumerable<TestCaseData> HaxeTestCases
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
                            new ClassModel { Name = "Main", InFile = FileModel.Ignore })
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
        public Result Haxe(string sourceText)
        {
            ASContext.Context.SetHaxeFeatures();
            Sci.ConfigurationLanguage = "haxe";
            Sci.Text = sourceText;
            SnippetHelper.PostProcessSnippets(Sci, 0);
            var currentModel = ASContext.Context.CurrentModel;
            new ASFileParser().ParseSrc(currentModel, Sci.Text);
            var currentClass = currentModel.Classes.FirstOrDefault() ?? ClassModel.VoidClass;
            ASContext.Context.CurrentClass.Returns(currentClass);
            ASContext.Context.CurrentMember.Returns(currentClass.Members.Items.FirstOrDefault());
            var target = RefactoringHelper.GetDefaultRefactorTarget();
            return new Result(target.IsPackage, target.Member, target.Type);
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
