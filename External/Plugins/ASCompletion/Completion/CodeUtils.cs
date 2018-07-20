namespace ASCompletion.Completion
{
    static class CodeUtils
    {
        /// <summary>
        /// Lookup type declaration keywords anywhere in the provided text
        /// </summary>
        public static bool IsTypeDecl(string line, string[] typesKeywords)
        {
            var max = line.Length - 1;
            foreach (string keyword in typesKeywords)
            {
                var p = line.IndexOf(keyword);
                if (p >= 0) return IsSpaceAt(line, p - 1) && IsSpaceAt(line, p + keyword.Length);
            }
            return false;
        }

        /// <summary>
        /// Look if the provided text starts with any declaration keyword
        /// </summary>
        public static bool IsDeclaration(string line, ContextFeatures features)
        {
            foreach (string keyword in features.accessKeywords)
                if (line.StartsWith(keyword) && IsSpaceAt(line, keyword.Length)) return true;
            foreach (string keyword in features.declKeywords)
                if (line.StartsWith(keyword) && IsSpaceAt(line, keyword.Length)) return true;
            return false;
        }

        /// <summary>
        /// Look if character after first word of line is whitespace
        /// </summary>
        public static bool IsSpaceAt(string line, int index)
        {
            if (index < 0 || index >= line.Length) return true;
            else return line[index] <= 32;
        }
    }
}
