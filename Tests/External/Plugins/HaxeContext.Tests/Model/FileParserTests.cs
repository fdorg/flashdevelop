using System.Collections.Generic;
using System.Linq;
using ASCompletion;
using ASCompletion.Context;
using ASCompletion.Model;
using HaXeContext.TestUtils;
using NUnit.Framework;

namespace HaXeContext.Model
{
    [TestFixture]
    class FileParserTests : ASCompletionTests
    {
        [TestFixtureSetUp]
        public void Setup() => SetHaxeFeatures(sci);

        static string ReadAllText(string fileName) => TestFile.ReadAllText(GetFullPath(fileName));

        static string GetFullPath(string fileName) => $"{nameof(HaXeContext)}.Test_Files.parser.{fileName}.hx";

        static IEnumerable<TestCaseData> Issue2302TestCases
        {
            get
            {
                yield return new TestCaseData("Issue2320_1")
                    .Returns(1)
                    .SetName("foo(? v)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2320");
                yield return new TestCaseData("Issue2320_2")
                    .Returns(1)
                    .SetName("foo(?     v)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2320");
                yield return new TestCaseData("Issue2320_3")
                    .Returns(2)
                    .SetName("foo(?\nv1\n,\nv2)")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2320");
            }
        }

        [Test, TestCaseSource(nameof(Issue2302TestCases))]
        public int ParseFile_Issue2320(string fileName)
        {
            var model = ASContext.Context.GetCodeModel(ReadAllText(fileName));
            return model.Classes.First().Members.Items.First().Parameters.Count;
        }

        static IEnumerable<TestCaseData> Issue2210TestCases
        {
            get
            {
                yield return new TestCaseData("Issue2210_1")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                        {
                            LineFrom = 2,
                            LineTo = 2,
                        }
                    })
                    .SetName("function foo() return 1;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_2")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                        {
                            LineFrom = 2,
                            LineTo = 2,
                        },
                        new MemberModel("bar", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                        {
                            LineFrom = 4,
                            LineTo = 4,
                        }
                    })
                    .SetName("function foo() return 1;\nfunction bar() return 1;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_3")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                        {
                            LineFrom = 2,
                            LineTo = 5,
                        }
                    })
                    .SetName("function foo(i:Int):Int\nreturn i % 2 == 0\n? 0\n: 1;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_4")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("v", "Int", FlagType.Dynamic | FlagType.Variable, Visibility.Private)
                        {
                            LineFrom = 2,
                            LineTo = 2,
                        },
                        new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                        {
                            LineFrom = 3,
                            LineTo = 3,
                        }
                    })
                    .SetName("function foo(i:Int):Int this.v = i;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_5")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                        {
                            LineFrom = 2,
                            LineTo = 2,
                        }
                    })
                    .SetName("function foo(i:Int):Int var v = i;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_6")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                        {
                            LineFrom = 2,
                            LineTo = 2,
                        },
                        new MemberModel("v", "Int", FlagType.Dynamic | FlagType.Variable, Visibility.Private)
                        {
                            LineFrom = 3,
                            LineTo = 3,
                        }
                    })
                    .SetName("function foo(i:Int):Int trace(i);\vvar v:Int;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_7")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                        {
                            LineFrom = 2,
                            LineTo = 2,
                        },
                        new MemberModel("v", "Int", FlagType.Static | FlagType.Variable, Visibility.Private)
                        {
                            LineFrom = 3,
                            LineTo = 3,
                        }
                    })
                    .SetName("function foo(i:Int):Int trace(i)\vstatic var v:Int;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_8")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                        {
                            LineFrom = 2,
                            LineTo = 2,
                        },
                        new MemberModel("v", "Int", FlagType.Access | FlagType.Dynamic | FlagType.Variable, Visibility.Public)
                        {
                            LineFrom = 3,
                            LineTo = 3,
                        }
                    })
                    .SetName("function foo(i:Int):Int trace(i)\vpublic var v:Int;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_9")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                        {
                            LineFrom = 2,
                            LineTo = 2,
                        },
                        new MemberModel("v", "Int", FlagType.Access | FlagType.Static | FlagType.Variable, Visibility.Public)
                        {
                            LineFrom = 3,
                            LineTo = 3,
                        }
                    })
                    .SetName("function foo(i:Int):Int trace(i)\vinline public static var v:Int;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_10")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                        {
                            LineFrom = 2,
                            LineTo = 2,
                        },
                        new MemberModel("v", "Int", FlagType.Intrinsic | FlagType.Extern | FlagType.Dynamic | FlagType.Variable, Visibility.Public)
                        {
                            LineFrom = 3,
                            LineTo = 3,
                        }
                    })
                    .SetName("function foo(i:Int):Int trace(i)\vextern var v:Int;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_11")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                        {
                            LineFrom = 2,
                            LineTo = 2,
                        },
                        new MemberModel("v", "Int", FlagType.Access | FlagType.Dynamic | FlagType.Variable, Visibility.Private)
                        {
                            LineFrom = 3,
                            LineTo = 3,
                        }
                    })
                    .SetName("function foo(i:Int):Int trace(i)\vprivate var v:Int;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_12")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                        {
                            LineFrom = 2,
                            LineTo = 2,
                        },
                        new MemberModel("v", "Int", FlagType.Access | FlagType.Dynamic | FlagType.Variable, Visibility.Private)
                        {
                            LineFrom = 3,
                            LineTo = 3,
                        }
                    })
                    .SetName("function foo(i:Int):Int trace(i)\vprivate var v:Int;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_13")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                        {
                            LineFrom = 2,
                            LineTo = 2,
                        },
                        new MemberModel("max", "ExprOf<T>", FlagType.Access | FlagType.Static | FlagType.Function, Visibility.Public)
                        {
                            LineFrom = 3,
                            LineTo = 3,
                        }
                    })
                    .SetName("function foo(i:Int):Int trace(i)\nmacro public static inline max(v1, v2) return v1;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
            }
        }

        [Test, TestCaseSource(nameof(Issue2210TestCases))]
        public List<MemberModel> ParseFile_Issue2210(string fileName)
        {
            var model = ASContext.Context.GetCodeModel(ReadAllText(fileName));
            return model.Classes.First().Members.Items;
        }
    }
}
