// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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

        static IEnumerable<TestCaseData> Issue3087GetWordLeftTestCases
        {
            get
            {
                yield return new TestCaseData("word$(EntryPoint)")
                    .SetName("word|. Issue 3085. Case 1")
                    .Returns("word")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3087");
                yield return new TestCaseData("word $(EntryPoint)")
                    .SetName("word |. Issue 3085. Case 2")
                    .Returns("word")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3087");
                yield return new TestCaseData("word     $(EntryPoint)")
                    .SetName("word      |. Issue 3085. Case 3")
                    .Returns("word")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3087");
                yield return new TestCaseData("word     $(EntryPoint)word2")
                    .SetName("word      |word2. Issue 3085. Case 4")
                    .Returns("word")
                    .Ignore("")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3087");
            }
        }

        [Test, TestCaseSource(nameof(Issue3087GetWordLeftTestCases))]
        public string GetWordLeft(string text)
        {
            SetSrc(sci, text);
            return ASComplete.GetWordLeft(sci, sci.CurrentPos);
        }

        static IEnumerable<TestCaseData> Issue3087GetWordRightTestCases
        {
            get
            {
                yield return new TestCaseData("$(EntryPoint)word")
                    .SetName("|word. Issue 3085. Case 1")
                    .Returns("word")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3087");
                yield return new TestCaseData("$(EntryPoint) word")
                    .SetName("| word. Issue 3085. Case 2")
                    .Returns("word")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3087");
                yield return new TestCaseData("$(EntryPoint)     word")
                    .SetName("|     word. Issue 3085. Case 3")
                    .Returns("word")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3087");
                yield return new TestCaseData("word2$(EntryPoint) word")
                    .SetName("word2| word. Issue 3085. Case 4")
                    .Returns("word")
                    .SetDescription("https://github.com/fdorg/flashdevelop/issues/3087");
            }
        }

        [Test, TestCaseSource(nameof(Issue3087GetWordRightTestCases))]
        public string GetWordRight(string text)
        {
            SetSrc(sci, text);
            return ASComplete.GetWordRight(sci, sci.CurrentPos);
        }
    }
}