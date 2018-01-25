using NUnit.Framework;

namespace ASCompletion.Model
{
    class ClassModelTests
    {
        [Test]
        public void ToMemeberModelTest()
        {
            var type = new ClassModel
            {
                Name = "Test",
                Type = "Test",
                Comments = "Test Comments",
                Flags = FlagType.Class,
                InFile =  FileModel.Ignore
            };
            var member = type.ToMemberModel();
            Assert.AreEqual(type.Name, member.Name);
            Assert.AreEqual(type.Type, member.Type);
            Assert.AreEqual(type.Flags, member.Flags);
            Assert.AreEqual(type.Comments, member.Comments);
        }
    }
}
