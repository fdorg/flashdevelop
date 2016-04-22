namespace ASCompletion.TestUtils.File
{
    public class TestFilePathInfo
    {
        private const string TEST_FILE_DIR = "Test_Files";

        internal readonly string rootNamespace;
        internal readonly string testName;
        
        public TestFilePathInfo(string rootNamespace, string testName)
        {
            this.rootNamespace = rootNamespace;
            this.testName = testName;
        }

        public string AS3(string resourceFile)
        {
            return GetPath(resourceFile, Language.AS3);
        }

        public string Haxe(string resourceFile)
        {
            return GetPath(resourceFile, Language.Haxe);
        }

        /// <summary>
        /// Builds a path like
        /// <code>"ASCompletion.Test_Files.generated.as3.FieldFromParameterEmptyBody.as"</code>
        /// </summary>
        private string GetPath(string fileName, Language language)
        {
            return string.Join(".", new[] {
                rootNamespace, TEST_FILE_DIR, testName,
                language.Directory, fileName, language.Extension });
        }
    }

    internal class Language
    {
        internal static readonly Language AS3 =
            new Language { Directory = "as3", Extension = "as" };
        internal static readonly Language Haxe =
            new Language { Directory = "haxe", Extension = "hx" };

        public string Directory;
        public string Extension;
    }
}
