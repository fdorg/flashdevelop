using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ASCompletion.Model;
using PluginCore.Collections;

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
        public bool HasMultilineString;
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
        public string IntegerKey;
        public string stringKey;
        public string arrayKey;
        public string dynamicKey;
        public string importKey;
        public string importKeyAlt;
        public string[] typesPreKeys = Array.Empty<string>();
        public string[] accessKeywords = Array.Empty<string>();
        public string[] codeKeywords = Array.Empty<string>();
        public string[] declKeywords = Array.Empty<string>();
        public string[] typesKeywords = Array.Empty<string>();
        public HashSet<string> Literals = new HashSet<string>();
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
        public char[] SpecialPostfixOperators = Array.Empty<char>();
        public string ConstructorKey;
        public bool HasGenericsShortNotation;
        public HashSet<char> ArithmeticOperators = new HashSet<char>();
        public string[] IncrementDecrementOperators = Array.Empty<string>();
        public string[] BitwiseOperators = Array.Empty<string>();
        public string[] BooleanOperators = Array.Empty<string>();
        public string[] TernaryOperators = Array.Empty<string>();

        /// <summary>
        /// Tells if a word is a keyword which precedes a type (like 'new')
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        internal bool HasTypePreKey(string word)
        {
            return typesPreKeys != null && typesPreKeys.Any(it => it == word);
        }

        /// <summary>
        /// Get a selected list of possible completion keywords
        /// </summary>
        /// <param name="text">Context</param>
        /// <returns>Keywords list</returns>
        internal List<string> GetDeclarationKeywords(string text, bool insideClass)
        {
            var result = new List<string>(accessKeywords);
            var members = new List<string>(declKeywords);
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
                    if (token.Length > 0 && result.Contains(token))
                    {
                        result.Remove(token);
                        if (token == overrideKey)
                        {
                            members.Clear();
                            members.Add(functionKey);
                        }
                        else if (token == privateKey || token == internalKey || token == publicKey)
                        {
                            if (privateKey != null) result.Remove(privateKey);
                            if (internalKey != null) result.Remove(internalKey);
                            if (publicKey != null) result.Remove(publicKey);
                        }
                        else 
                        {
                            if (importKey != null) members.Remove(importKey);
                            if (importKeyAlt != null) members.Remove(importKeyAlt);
                        }
                    }
                }
            }

            if (foundMember is null)
            {
                result.AddRange(members);
            }
            else if (foundMember == "class" || foundMember == "interface")
            {
                if (hasExtends) result.Add("extends");
                if (hasImplements && foundMember != "interface") result.Add("implements");
            }
            else if (foundMember == "abstract")
            {
                result.Add("to");
                result.Add("from");
            }

            result.Sort();
            return result;
        }
    }
}