using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
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
        public bool hasImplements;
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
        public string arrayKey;
        public string importKey;
        public string importKeyAlt;
        public string[] typesPreKeys = new string[] { };
        public string[] codeKeywords = new string[] { };
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

        public Dictionary<string, string> metadata = new Dictionary<string,string>();

        public MemberModel functionArguments;

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
        internal List<string> GetDeclarationKeywords(string text)
        {
            List<string> access = new List<string>();
            if (staticKey != null) access.Add(staticKey);
            if (finalKey != null) access.Add(finalKey);
            if (overrideKey != null) access.Add(overrideKey);
            if (publicKey != null) access.Add(publicKey);
            if (internalKey != null) access.Add(internalKey);
            if (protectedKey != null) access.Add(protectedKey);
            if (privateKey != null) access.Add(privateKey);
            if (inlineKey != null) access.Add(inlineKey);
            List<string> members = new List<string>();
            if (varKey != null) members.Add(varKey);
            if (constKey != null) members.Add(constKey);
            if (functionKey != null) members.Add(functionKey);
            if (namespaceKey != null) members.Add(namespaceKey);

            bool foundMember = false;
            bool foundAccess = false;
            bool foundSpecial = false;

            if (text != null)
            {
                string[] tokens = System.Text.RegularExpressions.Regex.Split(text, "\\s+");
                foreach (string token in tokens)
                {
                    if (token.Length > 0 && members.Contains(token))
                    {
                        foundMember = true;
                        break;
                    }
                }
                foreach (string token in tokens)
                {
                    if (token.Length > 0 && access.Contains(token))
                    {
                        foundAccess = true;
                        if (token == staticKey || token == finalKey)
                        {
                            foundSpecial = true;
                            access.Remove(token);
                        }
                        else if (token == overrideKey)
                        {
                            foundSpecial = true;
                            if (varKey != null) members.Remove(varKey);
                            if (constKey != null) members.Remove(constKey);
                            if (namespaceKey != null) members.Remove(namespaceKey);
                            access.Add("flash_proxy");
                        }
                        else if (token == inlineKey)
                        {
                            foundSpecial = true;
                            access.Remove(token);
                            if (varKey != null) members.Remove(varKey);
                            if (constKey != null) members.Remove(constKey);
                            if (namespaceKey != null) members.Remove(namespaceKey);
                        }
                        else
                        {
                            bool keepStatic = staticKey != null && access.Contains(staticKey);
                            bool keepFinal = finalKey != null && access.Contains(finalKey);
                            bool keepOverride = overrideKey != null && access.Contains(overrideKey);
                            bool keepInline = inlineKey != null && access.Contains(inlineKey);
                            access.Clear();
                            if (keepStatic) access.Add(staticKey);
                            if (keepFinal) access.Add(finalKey);
                            if (keepOverride) access.Add(overrideKey);
                            if (keepInline) access.Add(inlineKey);
                        }
                    }
                }
            }

            if (!foundMember)
            {
                foreach (string token in members) access.Add(token);

                if (hasExtends && !foundSpecial) access.Add("extends");
                if (hasImplements && !foundSpecial) access.Add("implements");

                if (!foundAccess && importKey != null) access.Add(importKey);
                if (!foundAccess && importKeyAlt != null) access.Add(importKeyAlt);
            }
            access.Sort();
            return access;
        }
    }
}
