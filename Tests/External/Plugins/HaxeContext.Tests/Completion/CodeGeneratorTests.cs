﻿using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.TestUtils;
using HaXeContext.TestUtils;
using NSubstitute;
using NUnit.Framework;
using PluginCore.Helpers;

namespace HaXeContext.Completion
{
    [TestFixture]
    class ConvertStaticMethodCallToStaticExtensionCall : ASGeneratorTests.GenerateJob
    {
        internal new static string ReadAllTextHaxe(string fileName) => TestFile.ReadAllText($"{nameof(HaXeContext)}.Test_Files.completion.generated.{fileName}.hx");

        [TestFixtureSetUp]
        public void Setup()
        {
            sci.ConfigurationLanguage = "haxe";
            ASContext.Context.SetHaxeFeatures();
        }

        public IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return
                    new TestCaseData(ReadAllTextHaxe("BeforeConvertStaticMethodCallIntoStaticExtensionsCall"))
                        .Returns(ReadAllTextHaxe("AfterConvertStaticMethodCallIntoStaticExtensionsCall"))
                        .SetName("var v = StringTools.trim(' string ') -> var v = ' string '.trim()");
                yield return
                    new TestCaseData(ReadAllTextHaxe("BeforeConvertStaticMethodCallIntoStaticExtensionsCall2"))
                        .Returns(ReadAllTextHaxe("AfterConvertStaticMethodCallIntoStaticExtensionsCall2"))
                        .SetName("var v = StringTools.lpad('10' , 8, 0) -> var v = '10'.lpad(8, 0)");
                yield return
                    new TestCaseData(ReadAllTextHaxe("BeforeConvertStaticMethodCallIntoStaticExtensionsCall3"))
                        .Returns(ReadAllTextHaxe("AfterConvertStaticMethodCallIntoStaticExtensionsCall3"))
                        .SetName("var v = Lambda.count([1, 2, 3]) -> var s = [1, 2, 3].count()");
                yield return
                    new TestCaseData(ReadAllTextHaxe("BeforeConvertStaticMethodCallIntoStaticExtensionsCall4"))
                        .Returns(ReadAllTextHaxe("AfterConvertStaticMethodCallIntoStaticExtensionsCall4"))
                        .SetName("var v = Reflect.isObject({x:0, y:1}) -> var v = {x:0, y:1}.isObject()");
                yield return
                    new TestCaseData(ReadAllTextHaxe("BeforeConvertStaticMethodCallIntoStaticExtensionsCall5"))
                        .Returns(ReadAllTextHaxe("AfterConvertStaticMethodCallIntoStaticExtensionsCall5"))
                        .SetName("private var v = Reflect.isObject({x:0, y:1}) -> private var v = {x:0, y:1}.isObject()");
                yield return
                    new TestCaseData(ReadAllTextHaxe("BeforeConvertStaticMethodCallIntoStaticExtensionsCall6"))
                        .Returns(ReadAllTextHaxe("AfterConvertStaticMethodCallIntoStaticExtensionsCall6"))
                        .SetName("private function foo() return Reflect.isObject({x:0, y:1}) -> private function foo() return {x:0, y:1}.isObject()");
                yield return
                    new TestCaseData(ReadAllTextHaxe("BeforeConvertStaticMethodCallIntoStaticExtensionsCall7"))
                        .Returns(ReadAllTextHaxe("AfterConvertStaticMethodCallIntoStaticExtensionsCall7"))
                        .SetName("var v = StringTools.lpad(Std.string(1), '0', 2) -> var v = Std.string(1).lpad('0', 2)");
                yield return
                    new TestCaseData(ReadAllTextHaxe("BeforeConvertStaticMethodCallIntoStaticExtensionsCall8"))
                        .Returns(ReadAllTextHaxe("AfterConvertStaticMethodCallIntoStaticExtensionsCall8"))
                        .SetName("var v = StringTools.lpad(someVar, '0', 2) -> var v = someVar.lpad('0', 2)");
                yield return
                    new TestCaseData(ReadAllTextHaxe("BeforeConvertStaticMethodCallIntoStaticExtensionsCall9"))
                        .Returns(ReadAllTextHaxe("AfterConvertStaticMethodCallIntoStaticExtensionsCall9"))
                        .SetName("var v = StringTools.lpad('-${someVar}', '0', 20) -> var v = '-${someVar}'.lpad('0', 20)");
                yield return
                    new TestCaseData(ReadAllTextHaxe("BeforeConvertStaticMethodCallIntoStaticExtensionsCall10"))
                        .Returns(ReadAllTextHaxe("AfterConvertStaticMethodCallIntoStaticExtensionsCall10"))
                        .SetName("var v = StringTools.lpad('12345'.split('')[0].charCodeAt(0), '0', 20) -> var v = '12345'.split('')[0].charCodeAt(0).lpad('0', 20)");
                yield return
                    new TestCaseData(ReadAllTextHaxe("BeforeConvertStaticMethodCallIntoStaticExtensionsCall11"))
                        .Returns(ReadAllTextHaxe("AfterConvertStaticMethodCallIntoStaticExtensionsCall11"))
                        .SetName("var v = Lambda.count(new Array<Int>()) -> var v = new Array<Int>().count()");
            }
        }

        [Test, TestCaseSource(nameof(TestCases))]
        public string Common(string sourceText)
        {
            sci.Text = sourceText;
            SnippetHelper.PostProcessSnippets(sci, 0);
            var currentModel = ASContext.Context.CurrentModel;
            new ASFileParser().ParseSrc(currentModel, sci.Text);
            var currentClass = currentModel.Classes[0];
            ASContext.Context.CurrentClass.Returns(currentClass);
            ASContext.Context.CurrentMember.Returns(currentClass.Members[0]);
            var expr = ASComplete.GetExpressionType(sci, sci.WordEndPosition(sci.CurrentPos, true));
            CodeGenerator.ConvertStaticMethodCallIntoStaticExtensionCall(sci, expr);
            return sci.Text;
        }
    }
}
