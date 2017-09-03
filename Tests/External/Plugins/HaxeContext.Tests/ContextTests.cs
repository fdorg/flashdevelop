using System.Collections.Generic;
using NUnit.Framework;

namespace HaXeContext
{
    [TestFixture]
    class ContextTests
    {
        Context context;

        [TestFixtureSetUp]
        public void FixtureSetUp() => context = new Context(new HaXeSettings());

        IEnumerable<TestCaseData> DecomposeTypesTestCases
        {
            get
            {
                yield return new TestCaseData(new List<string> {"haxe.Timer->Type.ValueType"})
                    .SetName("haxe.Timer->Type.ValueType")
                    .Returns(new[] {"haxe.Timer", "Type.ValueType"});
                yield return new TestCaseData(new List<string> {"{t:haxe.Timer, v:Type.ValueType}"})
                    .SetName("{t:haxe.Timer, v:Type.ValueType}")
                    .Returns(new[] {"haxe.Timer", "Type.ValueType"});
                yield return new TestCaseData(new List<string> {"{t:haxe.Timer}->Type.ValueType"})
                    .SetName("{t:haxe.Timer}->Type.ValueType")
                    .Returns(new[] {"haxe.Timer", "Type.ValueType"});
                yield return new TestCaseData(new List<string> {"{t:haxe.Timer->Type.ValueType}"})
                    .SetName("{t:haxe.Timer->Type.ValueType}")
                    .Returns(new[] {"haxe.Timer", "Type.ValueType"});
                yield return new TestCaseData(new List<string> {"{t:haxe.Timer->Type.ValueType->haxe.ds.Vector}"})
                    .SetName("{t:haxe.Timer->Type.ValueType->haxe.ds.Vector}")
                    .Returns(new[] {"haxe.Timer", "Type.ValueType", "haxe.ds.Vector"});
                yield return new TestCaseData(new List<string> {"{t:haxe.Timer->Type.ValueType}->haxe.ds.Vector"})
                    .SetName("{t:haxe.Timer->Type.ValueType}->haxe.ds.Vector")
                    .Returns(new[] {"haxe.Timer", "Type.ValueType", "haxe.ds.Vector"});
                yield return new TestCaseData(new List<string> {"haxe.Timer->Type.ValueType->{v:haxe.ds.Vector}"})
                    .SetName("haxe.Timer->Type.ValueType->{v:haxe.ds.Vector}")
                    .Returns(new[] {"haxe.Timer", "Type.ValueType", "haxe.ds.Vector"});
                yield return new TestCaseData(new List<string> {"haxe.Timer->haxe.Timer"})
                    .SetName("haxe.Timer->haxe.Timer")
                    .Returns(new[] {"haxe.Timer"});
            }
        }

        [Test, TestCaseSource(nameof(DecomposeTypesTestCases))]
        public IEnumerable<string> DecomposeTypes(IEnumerable<string> types) => context.DecomposeTypes(types);
    }
}
