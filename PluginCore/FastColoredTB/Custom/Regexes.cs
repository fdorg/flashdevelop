using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ScintillaNet.Lexers;
using ScintillaNet.Enums;

namespace FastColoredTextBoxNS
{
    public class Regexes
    {
        public static Regex[] CppRegexes = new Regex[32];
        public static Regex[] CssRegexes = new Regex[32];
        public static Regex[] XmlRegexes = new Regex[32];
        public static Regex[] HtmlRegexes = new Regex[32];
        public static Regex[] PropRegexes = new Regex[32];

        static Regexes()
        {
            // CPP
            CppRegexes[(int)CPP.COMMENT] = new Regex(@"//.*$", RegexOptions.Multiline | RegexOptions.Compiled);
            CppRegexes[(int)CPP.COMMENTLINE] = new Regex(@"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline | RegexOptions.Compiled);
            CppRegexes[(int)CPP.NUMBER] = new Regex(@"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b", RegexOptions.Compiled);
            CppRegexes[(int)CPP.WORD] = new Regex(@"\b(public|private|static|const|import|package|class|function|default|throw|new|switch|case|var|else|if|return|null|for|while)\b");
            CppRegexes[(int)CPP.STRING] = new Regex(@"""""|"".*?[^\\]""", RegexOptions.Compiled);
            CppRegexes[(int)CPP.CHARACTER] = new Regex(@"''|'.*?[^\\]'", RegexOptions.Compiled);
            CppRegexes[(int)CPP.PREPROCESSOR] = new Regex(@"#(if|elseif|else|end|error)\b");
            CppRegexes[(int)CPP.COMMENTDOCKEYWORD] = new Regex(@"\*?@\S+", RegexOptions.Singleline | RegexOptions.Compiled);
            // CSS
            CssRegexes[(int)CSS.COMMENT] = new Regex(@"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline | RegexOptions.Compiled);
            CssRegexes[(int)CSS.OPERATOR] = new Regex(@"[#:;{}.]", RegexOptions.Compiled);
            CssRegexes[(int)CSS.VALUE] = new Regex(@":.+\w+.+(?<!;)", RegexOptions.Compiled);
            CssRegexes[(int)CSS.TAG] = new Regex(@"^\w+[\n,]", RegexOptions.Compiled);
            CssRegexes[(int)CSS.CLASS] = new Regex(@":\w+[\n,]", RegexOptions.Compiled);
            CssRegexes[(int)CSS.PSEUDOCLASS] = new Regex(@":\w+[\n,]", RegexOptions.Compiled);
            CssRegexes[(int)CSS.DOUBLESTRING] = new Regex(@"""""|"".*?[^\\]""", RegexOptions.Singleline | RegexOptions.Compiled);
            CssRegexes[(int)CSS.SINGLESTRING] = new Regex(@"''|'.*?[^\\]''", RegexOptions.Singleline | RegexOptions.Compiled);
            CssRegexes[(int)CSS.ID] = new Regex(@"#\w+[., $]", RegexOptions.Compiled);
            // XML
            XmlRegexes[(int)XML.COMMENT] = new Regex(@"(<!--.*?-->)|(<!--.*)|(-->)", RegexOptions.Multiline | RegexOptions.Compiled);
            XmlRegexes[(int)XML.TAG] = new Regex(@"<\?|<|/>|</|>|\?>", RegexOptions.Compiled);
            XmlRegexes[(int)XML.TAGEND] = new Regex(@"</(?<range>[\w:]+)>", RegexOptions.Compiled);
            XmlRegexes[(int)XML.XMLSTART] = new Regex(@"<[?](?<range1>[x][m][l]{1})|<(?<range>[!\w:]+)", RegexOptions.Compiled);
            XmlRegexes[(int)XML.XMLEND] = new Regex(@"<[?](?<range1>[x][m][l]{1})|<(?<range>[!\w:]+)", RegexOptions.Compiled);
            XmlRegexes[(int)XML.ATTRIBUTE] = new Regex(@"(?<range>[\w\d\-\:]+)[ ]*=[ ]*'[^']*'|(?<range>[\w\d\-\:]+)[ ]*=[ ]*""[^""]*""|(?<range>[\w\d\-\:]+)[ ]*=[ ]*[\w\d\-\:]+", RegexOptions.Compiled);
            XmlRegexes[(int)XML.SINGLESTRING] = new Regex(@"[\w\d\-]+?=(?<range>'[^']*')|[\w\d\-]+[ ]*=[ ]*(?<range>""[^""]*"")|[\w\d\-]+[ ]*=[ ]*(?<range>[\w\d\-]+)", RegexOptions.Compiled);
            XmlRegexes[(int)XML.DOUBLESTRING] = new Regex(@"[\w\d\-]+?=(?<range>'[^']*')|[\w\d\-]+[ ]*=[ ]*(?<range>""[^""]*"")|[\w\d\-]+[ ]*=[ ]*(?<range>[\w\d\-]+)", RegexOptions.Compiled);
            XmlRegexes[(int)XML.ENTITY] = new Regex(@"\&(amp|gt|lt|nbsp|quot|apos|copy|reg|#[0-9]{1,8}|#x[0-9a-f]{1,8});", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            XmlRegexes[(int)XML.CDATA] = new Regex(@"<!\s*\[CDATA\s*\[(?<text>(?>[^]]+|](?!]>))*)]]>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            XmlRegexes[(int)XML.OTHER] = new Regex(@"[/=]", RegexOptions.Compiled);
            XmlRegexes[31] = new Regex(@"<(?<range>/?\w+)\s[^>]*?[^/]>|<(?<range>/?\w+)\s*>", RegexOptions.Singleline | RegexOptions.Compiled); // FOLDING
            // HTML
            HtmlRegexes[(int)HTML.COMMENT] = new Regex(@"(<!--.*?-->)|(<!--.*)|(-->)", RegexOptions.Multiline | RegexOptions.Compiled);
            HtmlRegexes[(int)HTML.TAG] = new Regex(@"<|/>|</|>", RegexOptions.Compiled);
            HtmlRegexes[(int)HTML.TAGEND] = new Regex(@"</(?<range>[\w:]+)>", RegexOptions.Compiled);
            HtmlRegexes[(int)HTML.XMLSTART] = new Regex(@"<[?](?<range1>[x][m][l]{1})|<(?<range>[!\w:]+)", RegexOptions.Compiled);
            HtmlRegexes[(int)HTML.XMLEND] = new Regex(@"<[?](?<range1>[x][m][l]{1})|<(?<range>[!\w:]+)", RegexOptions.Compiled);
            HtmlRegexes[(int)HTML.ATTRIBUTE] = new Regex(@"(?<range>[\w\d\-]{1,20}?)='[^']*'|(?<range>[\w\d\-]{1,20})=""[^""]*""|(?<range>[\w\d\-]{1,20})=[\w\d\-]{1,20}", RegexOptions.Compiled);
            HtmlRegexes[(int)HTML.VALUE] = new Regex(@"[\w\d\-]{1,20}?=(?<range>'[^']*')|[\w\d\-]{1,20}=(?<range>""[^""]*"")|[\w\d\-]{1,20}=(?<range>[\w\d\-]{1,20})", RegexOptions.Compiled);
            HtmlRegexes[(int)HTML.SINGLESTRING] = new Regex(@"[\w\d\-]+?=(?<range>'[^']*')|[\w\d\-]+[ ]*=[ ]*(?<range>""[^""]*"")|[\w\d\-]+[ ]*=[ ]*(?<range>[\w\d\-]+)", RegexOptions.Compiled);
            HtmlRegexes[(int)HTML.DOUBLESTRING] = new Regex(@"[\w\d\-]+?=(?<range>'[^']*')|[\w\d\-]+[ ]*=[ ]*(?<range>""[^""]*"")|[\w\d\-]+[ ]*=[ ]*(?<range>[\w\d\-]+)", RegexOptions.Compiled);
            HtmlRegexes[(int)HTML.ENTITY] = new Regex(@"\&(amp|gt|lt|nbsp|quot|apos|copy|reg|#[0-9]{1,8}|#x[0-9a-f]{1,8});", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            HtmlRegexes[(int)HTML.CDATA] = new Regex(@"<!\s*\[CDATA\s*\[(?<text>(?>[^]]+|](?!]>))*)]]>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            HtmlRegexes[(int)HTML.OTHER] = new Regex(@"[/=]", RegexOptions.Compiled);
            // PROPERTIES
            //PropRegexes[(int)PROPERTIES.ASSIGNMENT] = new Regex(@"[/=]", RegexOptions.Compiled);
        }

    }

}

