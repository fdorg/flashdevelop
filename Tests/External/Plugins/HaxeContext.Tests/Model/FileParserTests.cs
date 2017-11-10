using System.Collections.Generic;
using ASCompletion.Model;
using HaXeContext.TestUtils;
using NUnit.Framework;
using PluginCore.Helpers;

namespace HaXeContext.Model
{
    [TestFixture]
    class FileParserTests
    {
        [Test(Description = "import static function")]
        public void ImportStaticMethod()
        {
            using (var file = new TestFile("HaXeContext.Test_Files.parser.ImportStaticMembers_Issue1150_1.hx"))
            {
                var ctx = new Context(new HaXeSettings()) {Classpath = new List<PathModel>(0)};
                var model = ctx.GetCodeModel(new FileModel {Context = ctx}, FileHelper.ReadFile(file.DestinationFile), false);
                Assert.IsTrue((model.Imports[0].Flags & FlagType.Function) > 0);
            }
        }

        [Test(Description = "import static variable")]
        public void ImportStaticVariable()
        {
            using (var file = new TestFile("HaXeContext.Test_Files.parser.ImportStaticMembers_Issue1150_2.hx"))
            {
                var ctx = new Context(new HaXeSettings()) {Classpath = new List<PathModel>(0)};
                var model = ctx.GetCodeModel(new FileModel {Context = ctx}, FileHelper.ReadFile(file.DestinationFile), false);
                Assert.IsTrue((model.Imports[0].Flags & FlagType.Variable) > 0);
            }
        }
    }
}
