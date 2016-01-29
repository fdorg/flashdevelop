namespace ASCompletion.TestUtils.File
{
    public class TestFilePathInfo
    {
        internal readonly string rootNamespace;
        internal readonly string testName;
        
        public TestFilePathInfo(string rootNamespace, string testName)
        {
            this.rootNamespace = rootNamespace;
            this.testName = testName;
        }

        public QualifiedFilePathInfo AS3()
        {
            return new QualifiedFilePathInfo(this, Language.AS3);
        }

        public QualifiedFilePathInfo Haxe()
        {
            return new QualifiedFilePathInfo(this, Language.Haxe);
        }
    }

    public class QualifiedFilePathInfo
    {
        private const string TEST_FILE_DIR = "Test_Files";

        private readonly Language language;
        private readonly TestFilePathInfo pathInfo;

        internal QualifiedFilePathInfo(TestFilePathInfo pathInfo, Language language)
        {
            this.pathInfo = pathInfo;
            this.language = language;
        }

        /// <summary>
        /// Builds a path like
        /// <code>"ASCompletion.Test_Files.generated.as3.FieldFromParameterEmptyBody.as"</code>
        /// </summary>
        public string GetPath(string resourceFile)
        {
            return string.Join(".", new[] {
                pathInfo.rootNamespace, TEST_FILE_DIR, pathInfo.testName,
                language.Directory, resourceFile, language.Extension });
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
