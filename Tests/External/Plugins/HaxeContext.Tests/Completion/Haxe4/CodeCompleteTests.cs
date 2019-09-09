// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using NUnit.Framework;
using PluginCore;

namespace HaXeContext.Completion.Haxe4
{
    class CodeCompleteTests : ASCompleteTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            SetHaxeFeatures(sci);
            ASContext.Context.Settings.InstalledSDKs = new[] { new InstalledSDK { Path = PluginBase.CurrentProject.CurrentSDK, Version = "4.0.0" } };
        }

        static IEnumerable<TestCaseData> GetToolTipTextIssue2804TestCases
        {
            get
            {
                yield return new TestCaseData("GetToolTipText_issue2804_1")
                    .SetName("case First<complete>. Issue 2804. Case 1")
                    .Returns("static public inline var First : AInt = 1\n[COLOR=Black]in Issue2804_1[/COLOR]");
                yield return new TestCaseData("GetToolTipText_issue2804_2")
                    .SetName("case First<complete>. Issue 2804. Case 2")
                    .Returns("static public inline var First : AInt = 1\n[COLOR=Black]in Issue2804_2[/COLOR]");
                yield return new TestCaseData("GetToolTipText_issue2804_3")
                    .SetName("case Six<complete>. Issue 2804. Case 3")
                    .Returns("static public inline var Six : AInt = 6\n[COLOR=Black]in Issue2804_3[/COLOR]");
                yield return new TestCaseData("GetToolTipText_issue2804_4")
                    .SetName("first<complete>. Issue 2804. Case 4")
                    .Returns("static public inline var first : Int = 1\n[COLOR=Black]in Issue2804_4[/COLOR]");
            }
        }

        [Test, TestCaseSource(nameof(GetToolTipTextIssue2804TestCases))]
        public string GetToolTipText(string fileName)
        {
            SetSrc(sci, Haxe3.CodeCompleteTests.ReadAllText(fileName));
            var expr = ASComplete.GetExpressionType(sci, sci.CurrentPos, false, true);
            return ASComplete.GetToolTipText(expr);
        }
    }
}
