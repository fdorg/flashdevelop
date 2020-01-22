using NUnit.Framework;

namespace ASCompletion.Model
{
    class ClassModelTests
    {
        [Test]
        public void ToMemberModelTest()
        {
            var type = new ClassModel
            {
                Name = "Test",
                Type = "Test",
                Comments = "Test Comments",
                Flags = FlagType.Class,
                InFile =  FileModel.Ignore,
                LineFrom = 1,
                LineTo = 2,
            };
            var member = type.ToMemberModel();
            Assert.AreEqual(type.Name, member.Name);
            Assert.AreEqual(type.Type, member.Type);
            Assert.AreEqual(type.Flags, member.Flags);
            Assert.AreEqual(type.Comments, member.Comments);
            Assert.AreEqual(type.LineFrom, member.LineFrom);
            Assert.AreEqual(type.LineTo, member.LineTo);
        }
    }
}
