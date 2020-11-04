using System.Collections.Generic;
using NUnit.Framework;

namespace ASCompletion.Completion
{
    public class CodeCompleteTests : ASCompleteTests
    {
        [OneTimeSetUp]
        public void Setup() => SetAs3Features(sci);

        static IEnumerable<TestCaseData> Issue3085GetCharLeftTestCases
        {
            get
            {
                yield return new TestCaseData("c$(EntryPoint)", false)
                    .SetName("c|. Issue 3085. Case 1")
                    .Returns('c')
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3085");
                yield return new TestCaseData("c $(EntryPoint)", false)
                    .SetName("c |. Issue 3085. Case 2")
                    .Returns(' ')
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3085");
                yield return new TestCaseData("c$(EntryPoint)", true)
                    .SetName("c|. Issue 3085. Case 3")
                    .Returns('c')
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3085");
                yield return new TestCaseData("c $(EntryPoint)", true)
                    .SetName("c |. Issue 3085. Case 4")
                    .Returns('c')
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3085");
                yield return new TestCaseData("c    $(EntryPoint)", true)
                    .SetName("c     |. Issue 3085. Case 5")
                    .Returns('c')
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3085");
                yield return new TestCaseData("c    $(EntryPoint)a", true)
                    .SetName("c     |a. Issue 3085. Case 6")
                    .Returns('c')
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3085");
            }
        }

        [Test, TestCaseSource(nameof(Issue3085GetCharLeftTestCases))]
        public char GetCharLeft(string text, bool skipWhiteSpace)
        {
            SetSrc(sci, text);
            return ASComplete.GetCharLeft(sci, skipWhiteSpace, sci.CurrentPos);
        }

        static IEnumerable<TestCaseData> Issue3085GetCharRightTestCases
        {
            get
            {
                yield return new TestCaseData("$(EntryPoint)c", false)
                    .SetName("|c. Issue 3085. Case 1")
                    .Returns('c')
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3085");
                yield return new TestCaseData("$(EntryPoint) c", false)
                    .SetName("| c. Issue 3085. Case 2")
                    .Returns(' ')
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3085");
                yield return new TestCaseData("$(EntryPoint)c", true)
                    .SetName("|c. Issue 3085. Case 3")
                    .Returns('c')
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3085");
                yield return new TestCaseData("$(EntryPoint) c", true)
                    .SetName("| c. Issue 3085. Case 4")
                    .Returns('c')
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3085");
                yield return new TestCaseData("$(EntryPoint)    c", true)
                    .SetName("|     c. Issue 3085. Case 5")
                    .Returns('c')
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3085");
                yield return new TestCaseData("a$(EntryPoint)   c", true)
                    .SetName("a|    c. Issue 3085. Case 6")
                    .Returns('c')
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3085");
            }
        }

        [Test, TestCaseSource(nameof(Issue3085GetCharRightTestCases))]
        public char GetCharRight(string text, bool skipWhiteSpace)
        {
            SetSrc(sci, text);
            return ASComplete.GetCharRight(sci, skipWhiteSpace, sci.CurrentPos);
        }
    }
}