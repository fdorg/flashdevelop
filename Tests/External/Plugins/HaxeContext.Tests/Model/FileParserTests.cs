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
        [Test]
        public void ImportStaticMethod_Issue1150_1()
        {
            using (var file = new TestFile("HaXeContext.Test_Files.parser.ImportStaticMembers_Issue1150_1.hx"))
            {
                var ctx = new Context(new HaXeSettings());
                ctx.Classpath = new List<PathModel>(0);
                var model = ctx.GetCodeModel(new FileModel { Context = ctx }, FileHelper.ReadFile(file.DestinationFile));
                Assert.AreEqual(1, model.Imports.Count);
            }
        }
    }
}
