using System.Collections.Generic;
using System.Text.RegularExpressions;
using ASCompletion.Model;

namespace ASCompletion.Completion
{
    /// <summary>
    /// Describes the language features
    /// </summary>
    public class ContextFeatures
    {
        // code completion provided by a 3rd party compiler (ie. Haxe)
        public bool externalCompletion;

        // language constructs
        public bool hasPackages;
        public bool hasFriendlyParentPackages;
        public bool hasModules;
        public bool hasNamespaces;
        public bool hasImports;
        public bool hasImportsWildcard;
        public bool hasClasses;
        public bool hasMultipleDefs;
        public bool hasExtends;
        public string ExtendsKey;
        public bool hasImplements;
        public string ImplementsKey;
        public bool hasInterfaces;
        public bool hasEnums;
        public bool hasTypeDefs;
        public bool hasStructs;
        public bool hasDelegates;
        public bool hasGenerics;
        public bool hasEcmaTyping;
        public bool hasVars;
        public bool hasConsts;
        public bool hasMethods;
        public bool hasStatics;
        public bool hasOverride;
        public bool hasTryCatch;
        public bool hasE4X;
        public bool hasStaticInheritance;
        public bool hasInference;
        public bool hasStringInterpolation;
        public bool checkFileName;
        public char hiddenPackagePrefix;

        // C-style array type (ie. string[])
        public bool hasCArrays;
        public string CArrayTemplate;

        // support for directives
        public bool hasDirectives;
        public List<string> Directives;

        // allowed declarations access modifiers
        public Visibility classModifiers;
        public Visibility enumModifiers;
        public Visibility varModifiers;
        public Visibility constModifiers;
        public Visibility methodModifiers;

        // default declarations access modifiers
        public Visibility classModifierDefault;
        public Visibility enumModifierDefault;
        public Visibility typedefModifierDefault;
        public Visibility varModifierDefault;
        public Visibility constModifierDefault;
        public Visibility methodModifierDefault;

        // keywords
        public string dot;
        public string voidKey;
        public string objectKey;
        public string booleanKey;
        public string numberKey;
        public string stringKey;
        public string arrayKey;
        public string dynamicKey;
        public string importKey;
        public string importKeyAlt;
        public string[] typesPreKeys = { };
        public string[] accessKeywords = { };
        public string[] codeKeywords = { };
        public string[] declKeywords = { };
        public string[] typesKeywords = { };
        public string varKey;
        public string constKey;
        public string functionKey;
        public string getKey;
        public string setKey;
        public string staticKey;
        public string finalKey;
        public string overrideKey;
        public string publicKey;
        public string internalKey;
        public string protectedKey;
        public string privateKey;
        public string intrinsicKey;
        public string inlineKey;
        public string namespaceKey;
        public string stringInterpolationQuotes = "";
        public string ThisKey;
        public string BaseKey;

        public Dictionary<string, string> metadata = new Dictionary<string,string>();

        public MemberModel functionArguments;
        public char[] SpecialPostfixOperators = {};
        public string ConstructorKey;
        public bool HasGenericsShortNotation;
        public HashSet<char> ArithmeticOperators = new HashSet<char>();
        public string[] IncrementDecrementOperators = {};
        public string[] BitwiseOperators = { };
        public string[] BooleanOperators = { };

        /// <summary>
        /// Tells if a word is a keyword which precedes a type (like 'new')
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        internal bool HasTypePreKey(string word)
        {
            if (typesPreKeys != null)
            foreach (string key in typesPreKeys)
                if (key == word) return true;
            return false;
        }

        /// <summary>
        /// Get a selected list of possible completion keywords
        /// </summary>
        /// <param name="text">Context</param>
        /// <returns>Keywords list</returns>
        internal List<string> GetDeclarationKeywords(string text, bool insideClass)
        {
            List<string> access = new List<string>(accessKeywords);
            List<string> members = new List<string>(declKeywords);
            if (!insideClass) members.AddRange(typesKeywords);

            string foundMember = null;

            if (text != null)
            {
                string[] tokens = Regex.Split(text, "\\s+");
                foreach (string token in tokens)
                {
                    if (token.Length > 0 && members.Contains(token))
                    {
                        foundMember = token;
                        break;
                    }
                }
                foreach (string token in tokens)
                {
                    if (token.Length > 0 && access.Contains(token))
                    {
                        access.Remove(token);
                        if (token == overrideKey)
                        {
                            members.Clear();
                            members.Add(functionKey);
                        }
                        else if (token == privateKey || token == internalKey || token == publicKey)
                        {
                            if (privateKey != null) access.Remove(privateKey);
                            if (internalKey != null) access.Remove(internalKey);
                            if (publicKey != null) access.Remove(publicKey);
                        }
                        else 
                        {
                            if (importKey != null) members.Remove(importKey);
                            if (importKeyAlt != null) members.Remove(importKeyAlt);
                        }
                    }
                }
            }

            if (foundMember == null)
            {
                access.AddRange(members);
            }
            else if (foundMember == "class" || foundMember == "interface")
            {
                if (hasExtends) access.Add("extends");
                if (hasImplements && foundMember != "interface") access.Add("implements");
            }
            else if (foundMember == "abstract")
            {
                access.Add("to");
                access.Add("from");
            }

            access.Sort();
            return access;
        }
    }
}
