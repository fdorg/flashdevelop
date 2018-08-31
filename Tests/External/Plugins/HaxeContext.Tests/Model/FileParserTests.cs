﻿using System.Collections.Generic;
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

        static IEnumerable<TestCaseData> Issue2359TestCases
        {
            get
            {
                yield return new TestCaseData("Issue2359_1")
                    .Returns("Null<String>->EitherType<Int,String>->EitherType<Int,String>->Void")
                    .SetName("typedef TDef = Null<String> -> EitherType<Int,String> -> EitherType<Int,String> -> Void;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2359");
            }
        }

        static IEnumerable<TestCaseData> Issue2377TestCases
        {
            get
            {
                yield return new TestCaseData("Issue2377_1")
                    .Returns("Vector<T>")
                    .SetName("typedef UnsafeArray<T> = Vector<T>;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2377");
                yield return new TestCaseData("Issue2377_2")
                    .Returns("Vector<T>")
                    .SetName("private typedef UnsafeArray<T> = Vector<T>;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2377");
            }
        }

        [
            Test,
            TestCaseSource(nameof(Issue2359TestCases)),
            TestCaseSource(nameof(Issue2377TestCases)),
        ]
        public string ParseFileExtendsType(string fileName)
        {
            var model = ASContext.Context.GetCodeModel(ReadAllText(fileName));
            return model.Classes.First().ExtendsType;
        }

        static IEnumerable<TestCaseData> Issue163TestCases
        {
            get
            {
                yield return new TestCaseData("Issue163_1")
                    .Returns(new List<MemberModel> {new MemberModel("foo", "Int", FlagType.Access | FlagType.Dynamic | FlagType.Variable, Visibility.Public)})
                    .SetName("Support for @:publicFields. case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/163");
                yield return new TestCaseData("Issue163_2") 
                    .Returns(new List<MemberModel> {new MemberModel("foo", "Int", FlagType.Access | FlagType.Dynamic | FlagType.Variable, Visibility.Public)})
                    .SetName("Support for @:publicFields. case 1.1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/163");
                yield return new TestCaseData("Issue163_3")
                    .Returns(new List<MemberModel> {new MemberModel("foo", "Int", FlagType.Access | FlagType.Static | FlagType.Variable, Visibility.Public)})
                    .SetName("Support for @:publicFields. case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/163");
                yield return new TestCaseData("Issue163_3_1")
                    .Returns(new List<MemberModel> {new MemberModel("foo", "Int", FlagType.Access | FlagType.Static | FlagType.Variable, Visibility.Public)})
                    .SetName("Support for @:publicFields. case 2.1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/163");
                yield return new TestCaseData("Issue163_4")
                    .Returns(new List<MemberModel> {new MemberModel("foo", "Int", FlagType.Access | FlagType.Dynamic | FlagType.Getter | FlagType.Setter, Visibility.Public)})
                    .SetName("Support for @:publicFields. case 3")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/163");
                yield return new TestCaseData("Issue163_4_1")
                    .Returns(new List<MemberModel> {new MemberModel("foo", "Int", FlagType.Access | FlagType.Dynamic | FlagType.Getter | FlagType.Setter, Visibility.Public)})
                    .SetName("Support for @:publicFields. case 3.1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/163");
                yield return new TestCaseData("Issue163_5")
                    .Returns(new List<MemberModel> {new MemberModel("foo", "Int", FlagType.Access | FlagType.Dynamic | FlagType.Function, Visibility.Public)})
                    .SetName("Support for @:publicFields. case 4")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/163");
                yield return new TestCaseData("Issue163_5_1")
                    .Returns(new List<MemberModel> {new MemberModel("foo", "Int", FlagType.Access | FlagType.Dynamic | FlagType.Function, Visibility.Public)})
                    .SetName("Support for @:publicFields. case 4.1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/163");
                yield return new TestCaseData("Issue163_6")
                    .Returns(new List<MemberModel> {new MemberModel("Foo", null, FlagType.Access | FlagType.Function | FlagType.Constructor, Visibility.Public)})
                    .SetName("Support for @:publicFields. case 5")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/163");
                yield return new TestCaseData("Issue163_6_1")
                    .Returns(new List<MemberModel> {new MemberModel("Foo", null, FlagType.Access | FlagType.Function | FlagType.Constructor, Visibility.Public)})
                    .SetName("Support for @:publicFields. case 5.1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/163");
                yield return new TestCaseData("Issue163_7")
                    .Returns(new List<MemberModel> {new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Override | FlagType.Function, Visibility.Public)})
                    .SetName("Support for @:publicFields. case 6")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/163");
                yield return new TestCaseData("Issue163_7_1")
                    .Returns(new List<MemberModel> {new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Override | FlagType.Function, Visibility.Public)})
                    .SetName("Support for @:publicFields. case 6.1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/163");
                yield return new TestCaseData("Issue163_8")
                    .Returns(new List<MemberModel> {new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Override | FlagType.Function, Visibility.Public)})
                    .SetName("Support for @:publicFields. case 8")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/163");
                yield return new TestCaseData("Issue163_8_1")
                    .Returns(new List<MemberModel> {new MemberModel("foo", "Int", FlagType.Access | FlagType.Dynamic | FlagType.Override | FlagType.Function, Visibility.Public)})
                    .SetName("Support for @:publicFields. case 7.1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/163");
            }
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
                    .SetName("function foo(i:Int):Int\nreturn i % 2 == 0\n\t? 0\n\t: 1;")
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
                yield return new TestCaseData("Issue2210_14")
                    .Ignore("")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                        {
                            LineFrom = 2,
                            LineTo = 2,
                        },
                        new MemberModel("v", "Int", FlagType.Getter | FlagType.Setter, Visibility.Private)
                        {
                            LineFrom = 3,
                            LineTo = 3,
                        }
                    })
                    .SetName("function foo(i:Int):Int trace(i)\vvar v(get, set):Int;")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_15")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("foo", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private)
                        {
                            LineFrom = 2,
                            LineTo = 2,
                        },
                        new MemberModel("bar", null, FlagType.Dynamic | FlagType.Override | FlagType.Function, Visibility.Private)
                        {
                            LineFrom = 3,
                            LineTo = 3,
                        }
                    })
                    .SetName("function foo(i:Int):Int trace(i)\voverride function bar() {}")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_16")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("foo1", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private),
                        new MemberModel("foo2", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private),
                    })
                    .SetName("function foo() return switch(true) {\n\tcase: 1;\n\tcase _: 2;\n}")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_17")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("foo1", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private),
                        new MemberModel("foo2", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private),
                    })
                    .SetName("function foo() return if(true) {\n\t1;\n} else {\n\t2;\n}. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_18")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("foo1", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private),
                        new MemberModel("foo2", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private),
                    })
                    .SetName("function foo() return if(true) {\n\t(function bar() {return 1;})();\n} else {\n\t2;\n}")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
                yield return new TestCaseData("Issue2210_19")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("foo1", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private),
                        new MemberModel("foo2", "Int", FlagType.Dynamic | FlagType.Function, Visibility.Private),
                    })
                    .SetName("function foo() return if(true) {\n\t1;\n} else {\n\t2;\n}. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2210");
            }
        }

        static IEnumerable<TestCaseData> Issue2342TestCases
        {
            get
            {
                yield return new TestCaseData("Issue2342_1")
                    .Returns(new List<MemberModel>
                    {
                        new MemberModel("listen", "Void", FlagType.Dynamic | FlagType.Function, Visibility.Public)
                        {
                            LineFrom = 2,
                            LineTo = 2,
                        },
                        new MemberModel("close", "Void", FlagType.Dynamic | FlagType.Function, Visibility.Public)
                        {
                            LineFrom = 2,
                            LineTo = 2,
                        },
                    })
                    .SetName("Issue 2342. Case 1")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2342");
                yield return new TestCaseData("Issue2342_2")
                    .Returns(new List<MemberModel>())
                    .SetName("Issue 2342. Case 2")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/2342");
            }
        }

        [
            Test,
            TestCaseSource(nameof(Issue163TestCases)),
            TestCaseSource(nameof(Issue2210TestCases)),
            TestCaseSource(nameof(Issue2342TestCases)),
        ]
        public List<MemberModel> ParseFile(string fileName)
        {
            var model = ASContext.Context.GetCodeModel(ReadAllText(fileName));
            return model.Classes.First().Members.Items;
        }
    }
}
