// $ANTLR 3.1.1 AS3_ex.g3 2015-02-17 22:17:17

using System;
using Antlr.Runtime;
using Stack         = Antlr.Runtime.Collections.StackList;

public class AS3_exLexer : Lexer {
    public const int UNDERSCORE = 115;
    public const int REGULAR_EXPR_FIRST_CHAR = 133;
    public const int VAR = 45;
    public const int THROW = 39;
    public const int STATIC = 57;
    public const int INTERFACE = 25;
    public const int AND_ASSIGN = 104;
    public const int WHITESPACE = 125;
    public const int BREAK = 5;
    public const int INTERNAL = 26;
    public const int XML_LS_STD = 113;
    public const int DEC_NUMBER_LITERAL = 140;
    public const int ELSE = 14;
    public const int INCLUDE = 53;
    public const int IF = 20;
    public const int EACH = 49;
    public const int SUB = 76;
    public const int IN = 23;
    public const int LPAREN = 61;
    public const int DOT = 65;
    public const int IS = 27;
    public const int FUNCTION = 19;
    public const int LOR = 91;
    public const int CASE = 6;
    public const int AS = 4;
    public const int ELLIPSIS = 107;
    public const int LBRACK = 63;
    public const int GET = 50;
    public const int PUBLIC = 34;
    public const int UNICODE_ESCAPE = 122;
    public const int XOR = 87;
    public const int DOLLAR = 116;
    public const int SAME = 73;
    public const int SET = 51;
    public const int IDENT_NAME_ASCII_START = 141;
    public const int SUB_ASSIGN = 98;
    public const int SEMI = 58;
    public const int ASSIGN = 94;
    public const int IMPORT = 22;
    public const int DELETE = 12;
    public const int CATCH = 7;
    public const int PROTECTED = 33;
    public const int RCURLY = 60;
    public const int COMMA = 66;
    public const int USE = 44;
    public const int REGULAR_EXPR_BODY = 130;
    public const int LCURLY = 59;
    public const int SHL_ASSIGN = 99;
    public const int ALPHABET = 117;
    public const int PRIVATE = 32;
    public const int DYNAMIC = 54;
    public const int CR = 120;
    public const int BACKSLASH_SEQUENCE = 135;
    public const int CONTINUE = 10;
    public const int DIV = 78;
    public const int STAR = 77;
    public const int DEC_NUMBER = 138;
    public const int EXPONENT = 139;
    public const int LF = 121;
    public const int TO = 40;
    public const int NEQ = 72;
    public const int XML_NS_OP = 111;
    public const int NAMESPACE = 52;
    public const int EXTENDS = 15;
    public const int INSTANCEOF = 24;
    public const int NEW = 29;
    public const int COMMENT_SINGLELINE = 127;
    public const int LAND_ASSIGN = 102;
    public const int LT = 67;
    public const int SHU_ASSIGN = 101;
    public const int IDENT_ASCII_START = 143;
    public const int CLASS = 8;
    public const int DO = 13;
    public const int FINALLY = 17;
    public const int ESCAPE_SEQUENCE = 123;
    public const int COMMENT_MULTILINE = 126;
    public const int HEX_NUMBER_LITERAL = 137;
    public const int SHR_ASSIGN = 100;
    public const int REGULAR_EXPR_CHAR = 134;
    public const int CONST = 9;
    public const int PACKAGE = 31;
    public const int SHL = 82;
    public const int XML_TEND = 109;
    public const int OR_ASSIGN = 106;
    public const int TRY = 42;
    public const int SHR = 83;
    public const int QUE = 92;
    public const int SHU = 84;
    public const int NUMBER = 118;
    public const int NULL = 30;
    public const int REGULAR_EXPR_FLAG = 131;
    public const int XML_CDATA = 145;
    public const int FOR = 18;
    public const int TRUE = 41;
    public const int FINAL = 55;
    public const int RPAREN = 62;
    public const int EQ = 71;
    public const int IDENT_PART = 136;
    public const int RBRACK = 64;
    public const int NOT = 88;
    public const int AND = 85;
    public const int THIS = 38;
    public const int SWITCH = 37;
    public const int VOID = 46;
    public const int LTE = 69;
    public const int PLUS = 75;
    public const int INC = 80;
    public const int XML_LS_END = 114;
    public const int NATIVE = 28;
    public const int DIV_ASSIGN = 95;
    public const int NSAME = 74;
    public const int LAND = 90;
    public const int XML_PI = 146;
    public const int XML_TEXT = 147;
    public const int REGULAR_EXPR_LITERAL = 132;
    public const int INV = 89;
    public const int RETURN = 35;
    public const int SINGLE_QUOTE_LITERAL = 128;
    public const int XML_E_TEND = 110;
    public const int HEX_DIGIT = 119;
    public const int ADD_ASSIGN = 97;
    public const int IDENTIFIER = 142;
    public const int EOF = -1;
    public const int LOR_ASSIGN = 103;
    public const int SUPER = 36;
    public const int MOD = 79;
    public const int DEC = 81;
    public const int OR = 86;
    public const int XOR_ASSIGN = 105;
    public const int EOL = 124;
    public const int DOUBLE_QUOTE_LITERAL = 129;
    public const int IMPLEMENTS = 21;
    public const int COLON = 93;
    public const int XML_ELLIPSIS = 108;
    public const int GT = 68;
    public const int WITH = 48;
    public const int XML_AT = 112;
    public const int OVERRIDE = 56;
    public const int XML_COMMENT = 144;
    public const int TYPEOF = 43;
    public const int MOD_ASSIGN = 96;
    public const int GTE = 70;
    public const int FALSE = 16;
    public const int WHILE = 47;
    public const int DEFAULT = 11;

        /**  */
        private IToken lastDefaultCnlToken = null;
        
        // override
        override public IToken NextToken()
        {
            IToken result = base.NextToken();
            if (result!=null && result.Channel != AS3_exParser.CHANNEL_WHITESPACE )
            {
                lastDefaultCnlToken = result;
            }
            return result;      
        }
        
        override public void Reset()
        {
            base.Reset(); // reset all recognizer state variables
            if (input is ANTLRStringStream)
            {
                ((ANTLRStringStream)input).Reset();
            }
        }
        

        //TODO: fix this so that regular expression embedded within xml text will work
        private bool isRegularExpression(){
            if(lastDefaultCnlToken!=null){
                switch(lastDefaultCnlToken.Type){
                    case NULL :
                    case TRUE :
                    case FALSE:
                    case THIS :
                    case SUPER:
                    case IDENTIFIER:
                    case HEX_NUMBER_LITERAL:
                    case DEC_NUMBER_LITERAL:
                    case SINGLE_QUOTE_LITERAL:
                    case DOUBLE_QUOTE_LITERAL:
                    case RCURLY:
                    case RBRACK:
                    case RPAREN:
                        //this is an attempt to not think something is a regular expression if it happens
                        //to be part of a mathematical expression.
                        return false;
                    default:
                        break;
                }
            }

            //System.out.println("start to predict if is a RegularExpression");
            // start to predict if the next is a regular expression
            int next = -1;
            int index=1;
            bool success = false;
            if((next=input.LA(index)) != '/'){
                success = false;
                return success;
            }
            index++;
            // check the first regular character
            next=input.LA(index);
            if(next == '\r' || next == '\n' || 
                next == '*' || //starts a comment
                next == '/'  //if no regex content?
                //|| next == '>' //I think the idea of failing on /> is to prevent conflicts with other tokens, but I think that is irrelevant since I've made this context sensitive.
                ){
                success = false;
                return success;
            }else if(next == '\\'){
                next=input.LA(index+1);
                if(next == '\r' || next == '\n'){
                    success=false;
                    return success;
                }
                // we omit the escape sequence \ u XXXX or \ x XX
                index++;
            }
            index++;
            // check the body of regular character
            while((next=input.LA(index))!=-1){
                ////System.out.println("char["+index+"] = ("+(char)next+")");
                switch(next){
                    case '\r':
                    case '\n':
                        success = false;
                        return success;
                    case '\\':
                        next=input.LA(index+1);
                        if(next == '\r' || next == '\n'){
                            success=false;
                            return success;
                        }
                        // we omit the escape sequence \ u XXXX or \ x XX
                        index++;
                        break;
                    case '/':
                        success = true;
                        return success;
                }            
                index++;
            }
            return success;
        }
            
       /**
        * <pre> judge if is a XMLName </pre>
        * @param ch character
        * @return if is a XMLName return true
        */
        static bool isXMLText(int ch){
            //System.out.println("isXMLText start");
            return (ch!='{'&&ch!='<'&&!(isUnicodeIdentifierPart(ch)));
        }
            
        /*---------------------------UNICODE_INDENTIFER START------------------------------------------*/    
        
        private static bool isUnicodeIdentifierPart(int ch)
        {
            return ch=='$'||ch=='_'||isUnicodeLetter(ch)||isUnicodeDigit(ch)||isUnicodeCombiningMark(ch)||isUnicodeConnectorPunctuation(ch);
        }

        private void consumeIdentifierUnicodeStart() 
        {
            int ch = input.LA(1);
            if (isUnicodeLetter(ch) || ch=='$' || ch=='_')
            {
                MatchAny();
                do
                {
                    ch = input.LA(1);
                    if (isUnicodeIdentifierPart(ch))
                    {
                        mIDENT_PART();
                    }
                    else
                    {
                        return;
                    }
                }
                while (true);
            }
        }
        
        private static bool isUnicodeLetter(int ch) 
        {
            return (ch >= '\u0041' && ch <= '\u005A')
                    || (ch >= '\u0061' && ch <= '\u007A') || (ch == '\u00AA')
                    || (ch == '\u00B5') || (ch == '\u00BA')
                    || (ch >= '\u00C0' && ch <= '\u00D6')
                    || (ch >= '\u00D8' && ch <= '\u00F6')
                    || (ch >= '\u00F8' && ch <= '\u02C1')
                    || (ch >= '\u02C6' && ch <= '\u02D1')
                    || (ch >= '\u02E0' && ch <= '\u02E4') || (ch == '\u02EC')
                    || (ch == '\u02EE') || (ch >= '\u0370' && ch <= '\u0374')
                    || (ch >= '\u0376' && ch <= '\u037D') || (ch == '\u0386')
                    || (ch >= '\u0388' && ch <= '\u03F5')
                    || (ch >= '\u03F7' && ch <= '\u0481')
                    || (ch >= '\u048A' && ch <= '\u0559')
                    || (ch >= '\u0561' && ch <= '\u0587')
                    || (ch >= '\u05D0' && ch <= '\u05F2')
                    || (ch >= '\u0621' && ch <= '\u064A')
                    || (ch >= '\u066E' && ch <= '\u066F')
                    || (ch >= '\u0671' && ch <= '\u06D3') || (ch == '\u06D5')
                    || (ch >= '\u06E5' && ch <= '\u06E6')
                    || (ch >= '\u06EE' && ch <= '\u06EF')
                    || (ch >= '\u06FA' && ch <= '\u06FC') || (ch == '\u06FF')
                    || (ch == '\u0710') || (ch >= '\u0712' && ch <= '\u072F')
                    || (ch >= '\u074D' && ch <= '\u07A5') || (ch == '\u07B1')
                    || (ch >= '\u07CA' && ch <= '\u07EA')
                    || (ch >= '\u07F4' && ch <= '\u07F5') || (ch == '\u07FA')
                    || (ch >= '\u0904' && ch <= '\u0939') || (ch == '\u093D')
                    || (ch == '\u0950') || (ch >= '\u0958' && ch <= '\u0961')
                    || (ch >= '\u0971' && ch <= '\u097F')
                    || (ch >= '\u0985' && ch <= '\u09B9') || (ch == '\u09BD')
                    || (ch == '\u09CE') || (ch >= '\u09DC' && ch <= '\u09E1')
                    || (ch >= '\u09F0' && ch <= '\u09F1')
                    || (ch >= '\u0A05' && ch <= '\u0A39')
                    || (ch >= '\u0A59' && ch <= '\u0A5E')
                    || (ch >= '\u0A72' && ch <= '\u0A74')
                    || (ch >= '\u0A85' && ch <= '\u0AB9') || (ch == '\u0ABD')
                    || (ch >= '\u0AD0' && ch <= '\u0AE1')
                    || (ch >= '\u0B05' && ch <= '\u0B39') || (ch == '\u0B3D')
                    || (ch >= '\u0B5C' && ch <= '\u0B61') || (ch == '\u0B71')
                    || (ch >= '\u0B83' && ch <= '\u0BB9') || (ch == '\u0BD0')
                    || (ch >= '\u0C05' && ch <= '\u0C3D')
                    || (ch >= '\u0C58' && ch <= '\u0C61')
                    || (ch >= '\u0C85' && ch <= '\u0CB9') || (ch == '\u0CBD')
                    || (ch >= '\u0CDE' && ch <= '\u0CE1')
                    || (ch >= '\u0D05' && ch <= '\u0D3D')
                    || (ch >= '\u0D60' && ch <= '\u0D61')
                    || (ch >= '\u0D7A' && ch <= '\u0D7F')
                    || (ch >= '\u0D85' && ch <= '\u0DC6')
                    || (ch >= '\u0E01' && ch <= '\u0E30')
                    || (ch >= '\u0E32' && ch <= '\u0E33')
                    || (ch >= '\u0E40' && ch <= '\u0E46')
                    || (ch >= '\u0E81' && ch <= '\u0EB0')
                    || (ch >= '\u0EB2' && ch <= '\u0EB3')
                    || (ch >= '\u0EBD' && ch <= '\u0EC6')
                    || (ch >= '\u0EDC' && ch <= '\u0F00')
                    || (ch >= '\u0F40' && ch <= '\u0F6C')
                    || (ch >= '\u0F88' && ch <= '\u0F8B')
                    || (ch >= '\u1000' && ch <= '\u102A') || (ch == '\u103F')
                    || (ch >= '\u1050' && ch <= '\u1055')
                    || (ch >= '\u105A' && ch <= '\u105D') || (ch == '\u1061')
                    || (ch >= '\u1065' && ch <= '\u1066')
                    || (ch >= '\u106E' && ch <= '\u1070')
                    || (ch >= '\u1075' && ch <= '\u1081') || (ch == '\u108E')
                    || (ch >= '\u10A0' && ch <= '\u10FA')
                    || (ch >= '\u10FC' && ch <= '\u135A')
                    || (ch >= '\u1380' && ch <= '\u138F')
                    || (ch >= '\u13A0' && ch <= '\u166C')
                    || (ch >= '\u166F' && ch <= '\u1676')
                    || (ch >= '\u1681' && ch <= '\u169A')
                    || (ch >= '\u16A0' && ch <= '\u16EA')
                    || (ch >= '\u16EE' && ch <= '\u1711')
                    || (ch >= '\u1720' && ch <= '\u1731')
                    || (ch >= '\u1740' && ch <= '\u1751')
                    || (ch >= '\u1760' && ch <= '\u1770')
                    || (ch >= '\u1780' && ch <= '\u17B3') || (ch == '\u17D7')
                    || (ch == '\u17DC') || (ch >= '\u1820' && ch <= '\u18A8')
                    || (ch >= '\u18AA' && ch <= '\u191C')
                    || (ch >= '\u1950' && ch <= '\u19A9')
                    || (ch >= '\u19C1' && ch <= '\u19C7')
                    || (ch >= '\u1A00' && ch <= '\u1A16')
                    || (ch >= '\u1B05' && ch <= '\u1B33')
                    || (ch >= '\u1B45' && ch <= '\u1B4B')
                    || (ch >= '\u1B83' && ch <= '\u1BA0')
                    || (ch >= '\u1BAE' && ch <= '\u1BAF')
                    || (ch >= '\u1C00' && ch <= '\u1C23')
                    || (ch >= '\u1C4D' && ch <= '\u1C4F')
                    || (ch >= '\u1C5A' && ch <= '\u1C7D')
                    || (ch >= '\u1D00' && ch <= '\u1DBF')
                    || (ch >= '\u1E00' && ch <= '\u1FBC') || (ch == '\u1FBE')
                    || (ch >= '\u1FC2' && ch <= '\u1FCC')
                    || (ch >= '\u1FD0' && ch <= '\u1FDB')
                    || (ch >= '\u1FE0' && ch <= '\u1FEC')
                    || (ch >= '\u1FF2' && ch <= '\u1FFC') || (ch == '\u2071')
                    || (ch == '\u207F') || (ch >= '\u2090' && ch <= '\u2094')
                    || (ch == '\u2102') || (ch == '\u2107')
                    || (ch >= '\u210A' && ch <= '\u2113') || (ch == '\u2115')
                    || (ch >= '\u2119' && ch <= '\u211D') || (ch == '\u2124')
                    || (ch == '\u2126') || (ch == '\u2128')
                    || (ch >= '\u212A' && ch <= '\u212D')
                    || (ch >= '\u212F' && ch <= '\u2139')
                    || (ch >= '\u213C' && ch <= '\u213F')
                    || (ch >= '\u2145' && ch <= '\u2149') || (ch == '\u214E')
                    || (ch >= '\u2160' && ch <= '\u2188')
                    || (ch >= '\u2C00' && ch <= '\u2CE4')
                    || (ch >= '\u2D00' && ch <= '\u2DDE') || (ch == '\u2E2F')
                    || (ch >= '\u3005' && ch <= '\u3007')
                    || (ch >= '\u3021' && ch <= '\u3029')
                    || (ch >= '\u3031' && ch <= '\u3035')
                    || (ch >= '\u3038' && ch <= '\u303C')
                    || (ch >= '\u3041' && ch <= '\u3096')
                    || (ch >= '\u309D' && ch <= '\u309F')
                    || (ch >= '\u30A1' && ch <= '\u30FA')
                    || (ch >= '\u30FC' && ch <= '\u318E')
                    || (ch >= '\u31A0' && ch <= '\u31B7')
                    || (ch >= '\u31F0' && ch <= '\u31FF')
                    || (ch >= '\u3400' && ch <= '\u4DB5')
                    || (ch >= '\u4E00' && ch <= '\uA48C')
                    || (ch >= '\uA500' && ch <= '\uA60C')
                    || (ch >= '\uA610' && ch <= '\uA61F')
                    || (ch >= '\uA62A' && ch <= '\uA66E')
                    || (ch >= '\uA67F' && ch <= '\uA697')
                    || (ch >= '\uA717' && ch <= '\uA71F')
                    || (ch >= '\uA722' && ch <= '\uA788')
                    || (ch >= '\uA78B' && ch <= '\uA801')
                    || (ch >= '\uA803' && ch <= '\uA805')
                    || (ch >= '\uA807' && ch <= '\uA80A')
                    || (ch >= '\uA80C' && ch <= '\uA822')
                    || (ch >= '\uA840' && ch <= '\uA873')
                    || (ch >= '\uA882' && ch <= '\uA8B3')
                    || (ch >= '\uA90A' && ch <= '\uA925')
                    || (ch >= '\uA930' && ch <= '\uA946')
                    || (ch >= '\uAA00' && ch <= '\uAA28')
                    || (ch >= '\uAA40' && ch <= '\uAA42')
                    || (ch >= '\uAA44' && ch <= '\uAA4B')
                    || (ch >= '\uAC00' && ch <= '\uD7A3')
                    || (ch >= '\uF900' && ch <= '\uFB1D')
                    || (ch >= '\uFB1F' && ch <= '\uFB28')
                    || (ch >= '\uFB2A' && ch <= '\uFD3D')
                    || (ch >= '\uFD50' && ch <= '\uFDFB')
                    || (ch >= '\uFE70' && ch <= '\uFEFC')
                    || (ch >= '\uFF21' && ch <= '\uFF3A')
                    || (ch >= '\uFF41' && ch <= '\uFF5A')
                    || (ch >= '\uFF66' && ch <= '\uFFDC');
        }
        
        private static bool isUnicodeCombiningMark(int ch) 
        {
                return (ch >= '\u0300' && ch <= '\u036F')
                        || (ch >= '\u0483' && ch <= '\u0487')
                        || (ch >= '\u0591' && ch <= '\u05BD') || (ch == '\u05BF')
                        || (ch >= '\u05C1' && ch <= '\u05C2')
                        || (ch >= '\u05C4' && ch <= '\u05C5') || (ch == '\u05C7')
                        || (ch >= '\u0610' && ch <= '\u061A')
                        || (ch >= '\u064B' && ch <= '\u065E') || (ch == '\u0670')
                        || (ch >= '\u06D6' && ch <= '\u06DC')
                        || (ch >= '\u06DF' && ch <= '\u06E4')
                        || (ch >= '\u06E7' && ch <= '\u06E8')
                        || (ch >= '\u06EA' && ch <= '\u06ED') || (ch == '\u0711')
                        || (ch >= '\u0730' && ch <= '\u074A')
                        || (ch >= '\u07A6' && ch <= '\u07B0')
                        || (ch >= '\u07EB' && ch <= '\u07F3')
                        || (ch >= '\u0901' && ch <= '\u0903') || (ch == '\u093C')
                        || (ch >= '\u093E' && ch <= '\u094D')
                        || (ch >= '\u0951' && ch <= '\u0954')
                        || (ch >= '\u0962' && ch <= '\u0963')
                        || (ch >= '\u0981' && ch <= '\u0983') || (ch == '\u09BC')
                        || (ch >= '\u09BE' && ch <= '\u09CD') || (ch == '\u09D7')
                        || (ch >= '\u09E2' && ch <= '\u09E3')
                        || (ch >= '\u0A01' && ch <= '\u0A03')
                        || (ch >= '\u0A3C' && ch <= '\u0A51')
                        || (ch >= '\u0A70' && ch <= '\u0A71')
                        || (ch >= '\u0A75' && ch <= '\u0A83') || (ch == '\u0ABC')
                        || (ch >= '\u0ABE' && ch <= '\u0ACD')
                        || (ch >= '\u0AE2' && ch <= '\u0AE3')
                        || (ch >= '\u0B01' && ch <= '\u0B03') || (ch == '\u0B3C')
                        || (ch >= '\u0B3E' && ch <= '\u0B57')
                        || (ch >= '\u0B62' && ch <= '\u0B63') || (ch == '\u0B82')
                        || (ch >= '\u0BBE' && ch <= '\u0BCD') || (ch == '\u0BD7')
                        || (ch >= '\u0C01' && ch <= '\u0C03')
                        || (ch >= '\u0C3E' && ch <= '\u0C56')
                        || (ch >= '\u0C62' && ch <= '\u0C63')
                        || (ch >= '\u0C82' && ch <= '\u0C83') || (ch == '\u0CBC')
                        || (ch >= '\u0CBE' && ch <= '\u0CD6')
                        || (ch >= '\u0CE2' && ch <= '\u0CE3')
                        || (ch >= '\u0D02' && ch <= '\u0D03')
                        || (ch >= '\u0D3E' && ch <= '\u0D57')
                        || (ch >= '\u0D62' && ch <= '\u0D63')
                        || (ch >= '\u0D82' && ch <= '\u0D83')
                        || (ch >= '\u0DCA' && ch <= '\u0DF3') || (ch == '\u0E31')
                        || (ch >= '\u0E34' && ch <= '\u0E3A')
                        || (ch >= '\u0E47' && ch <= '\u0E4E') || (ch == '\u0EB1')
                        || (ch >= '\u0EB4' && ch <= '\u0EBC')
                        || (ch >= '\u0EC8' && ch <= '\u0ECD')
                        || (ch >= '\u0F18' && ch <= '\u0F19') || (ch == '\u0F35')
                        || (ch == '\u0F37') || (ch == '\u0F39')
                        || (ch >= '\u0F3E' && ch <= '\u0F3F')
                        || (ch >= '\u0F71' && ch <= '\u0F84')
                        || (ch >= '\u0F86' && ch <= '\u0F87')
                        || (ch >= '\u0F90' && ch <= '\u0FBC') || (ch == '\u0FC6')
                        || (ch >= '\u102B' && ch <= '\u103E')
                        || (ch >= '\u1056' && ch <= '\u1059')
                        || (ch >= '\u105E' && ch <= '\u1060')
                        || (ch >= '\u1062' && ch <= '\u1064')
                        || (ch >= '\u1067' && ch <= '\u106D')
                        || (ch >= '\u1071' && ch <= '\u1074')
                        || (ch >= '\u1082' && ch <= '\u108D') || (ch == '\u108F')
                        || (ch == '\u135F') || (ch >= '\u1712' && ch <= '\u1714')
                        || (ch >= '\u1732' && ch <= '\u1734')
                        || (ch >= '\u1752' && ch <= '\u1753')
                        || (ch >= '\u1772' && ch <= '\u1773')
                        || (ch >= '\u17B6' && ch <= '\u17D3') || (ch == '\u17DD')
                        || (ch >= '\u180B' && ch <= '\u180D') || (ch == '\u18A9')
                        || (ch >= '\u1920' && ch <= '\u193B')
                        || (ch >= '\u19B0' && ch <= '\u19C0')
                        || (ch >= '\u19C8' && ch <= '\u19C9')
                        || (ch >= '\u1A17' && ch <= '\u1A1B')
                        || (ch >= '\u1B00' && ch <= '\u1B04')
                        || (ch >= '\u1B34' && ch <= '\u1B44')
                        || (ch >= '\u1B6B' && ch <= '\u1B73')
                        || (ch >= '\u1B80' && ch <= '\u1B82')
                        || (ch >= '\u1BA1' && ch <= '\u1BAA')
                        || (ch >= '\u1C24' && ch <= '\u1C37')
                        || (ch >= '\u1DC0' && ch <= '\u1DFF')
                        || (ch >= '\u20D0' && ch <= '\u20DC') || (ch == '\u20E1')
                        || (ch >= '\u20E5' && ch <= '\u20F0')
                        || (ch >= '\u2DE0' && ch <= '\u2DFF')
                        || (ch >= '\u302A' && ch <= '\u302F')
                        || (ch >= '\u3099' && ch <= '\u309A') || (ch == '\uA66F')
                        || (ch >= '\uA67C' && ch <= '\uA67D') || (ch == '\uA802')
                        || (ch == '\uA806') || (ch == '\uA80B')
                        || (ch >= '\uA823' && ch <= '\uA827')
                        || (ch >= '\uA880' && ch <= '\uA881')
                        || (ch >= '\uA8B4' && ch <= '\uA8C4')
                        || (ch >= '\uA926' && ch <= '\uA92D')
                        || (ch >= '\uA947' && ch <= '\uA953')
                        || (ch >= '\uAA29' && ch <= '\uAA36') || (ch == '\uAA43')
                        || (ch >= '\uAA4C' && ch <= '\uAA4D') || (ch == '\uFB1E')
                        || (ch >= '\uFE00' && ch <= '\uFE0F')
                        || (ch >= '\uFE20' && ch <= '\uFE26');
            }

            private static bool isUnicodeDigit(int ch) 
            {
                return (ch >= '\u0030' && ch <= '\u0039')
                        || (ch >= '\u0660' && ch <= '\u0669')
                        || (ch >= '\u06F0' && ch <= '\u06F9')
                        || (ch >= '\u07C0' && ch <= '\u07C9')
                        || (ch >= '\u0966' && ch <= '\u096F')
                        || (ch >= '\u09E6' && ch <= '\u09EF')
                        || (ch >= '\u0A66' && ch <= '\u0A6F')
                        || (ch >= '\u0AE6' && ch <= '\u0AEF')
                        || (ch >= '\u0B66' && ch <= '\u0B6F')
                        || (ch >= '\u0BE6' && ch <= '\u0BEF')
                        || (ch >= '\u0C66' && ch <= '\u0C6F')
                        || (ch >= '\u0CE6' && ch <= '\u0CEF')
                        || (ch >= '\u0D66' && ch <= '\u0D6F')
                        || (ch >= '\u0E50' && ch <= '\u0E59')
                        || (ch >= '\u0ED0' && ch <= '\u0ED9')
                        || (ch >= '\u0F20' && ch <= '\u0F29')
                        || (ch >= '\u1040' && ch <= '\u1049')
                        || (ch >= '\u1090' && ch <= '\u1099')
                        || (ch >= '\u17E0' && ch <= '\u17E9')
                        || (ch >= '\u1810' && ch <= '\u1819')
                        || (ch >= '\u1946' && ch <= '\u194F')
                        || (ch >= '\u19D0' && ch <= '\u19D9')
                        || (ch >= '\u1B50' && ch <= '\u1B59')
                        || (ch >= '\u1BB0' && ch <= '\u1BB9')
                        || (ch >= '\u1C40' && ch <= '\u1C49')
                        || (ch >= '\u1C50' && ch <= '\u1C59')
                        || (ch >= '\uA620' && ch <= '\uA629')
                        || (ch >= '\uA8D0' && ch <= '\uA909')
                        || (ch >= '\uAA50' && ch <= '\uAA59')
                        || (ch >= '\uFF10' && ch <= '\uFF19');
            }

            private static bool isUnicodeConnectorPunctuation(int ch) 
            {
                return (ch == '\u005F') || (ch >= '\u203F' && ch <= '\u2040')
                        || (ch == '\u2054') || (ch >= '\uFE33' && ch <= '\uFE34')
                        || (ch >= '\uFE4D' && ch <= '\uFE4F') || (ch == '\uFF3F');
            }

        /*---------------------------UNICODE_INDENTIFER END------------------------------------------*/
        
        private void debugMethod(String methodName,String text){
            //System.out.println("recognized as <<"+methodName+">> text=("+text+")");
        }    


    // delegates
    // delegators

    public AS3_exLexer() 
    {
        InitializeCyclicDFAs();
    }
    public AS3_exLexer(ICharStream input)
        : this(input, null) {
    }
    public AS3_exLexer(ICharStream input, RecognizerSharedState state)
        : base(input, state) {
        InitializeCyclicDFAs(); 

    }
    
    override public string GrammarFileName
    {
        get { return "AS3_ex.g3";} 
    }

    // $ANTLR start "AS"
    public void mAS() // throws RecognitionException [2]
    {
            try
            {
            int _type = AS;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:449:4: ( 'as' )
            // AS3_ex.g3:449:6: 'as'
            {
                Match("as"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "AS"

    // $ANTLR start "BREAK"
    public void mBREAK() // throws RecognitionException [2]
    {
            try
            {
            int _type = BREAK;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:450:7: ( 'break' )
            // AS3_ex.g3:450:9: 'break'
            {
                Match("break"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "BREAK"

    // $ANTLR start "CASE"
    public void mCASE() // throws RecognitionException [2]
    {
            try
            {
            int _type = CASE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:451:6: ( 'case' )
            // AS3_ex.g3:451:8: 'case'
            {
                Match("case"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "CASE"

    // $ANTLR start "CATCH"
    public void mCATCH() // throws RecognitionException [2]
    {
            try
            {
            int _type = CATCH;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:452:7: ( 'catch' )
            // AS3_ex.g3:452:9: 'catch'
            {
                Match("catch"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "CATCH"

    // $ANTLR start "CLASS"
    public void mCLASS() // throws RecognitionException [2]
    {
            try
            {
            int _type = CLASS;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:453:7: ( 'class' )
            // AS3_ex.g3:453:9: 'class'
            {
                Match("class"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "CLASS"

    // $ANTLR start "CONST"
    public void mCONST() // throws RecognitionException [2]
    {
            try
            {
            int _type = CONST;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:454:7: ( 'const' )
            // AS3_ex.g3:454:9: 'const'
            {
                Match("const"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "CONST"

    // $ANTLR start "CONTINUE"
    public void mCONTINUE() // throws RecognitionException [2]
    {
            try
            {
            int _type = CONTINUE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:455:10: ( 'continue' )
            // AS3_ex.g3:455:12: 'continue'
            {
                Match("continue"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "CONTINUE"

    // $ANTLR start "DEFAULT"
    public void mDEFAULT() // throws RecognitionException [2]
    {
            try
            {
            int _type = DEFAULT;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:456:9: ( 'default' )
            // AS3_ex.g3:456:11: 'default'
            {
                Match("default"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "DEFAULT"

    // $ANTLR start "DELETE"
    public void mDELETE() // throws RecognitionException [2]
    {
            try
            {
            int _type = DELETE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:457:8: ( 'delete' )
            // AS3_ex.g3:457:10: 'delete'
            {
                Match("delete"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "DELETE"

    // $ANTLR start "DO"
    public void mDO() // throws RecognitionException [2]
    {
            try
            {
            int _type = DO;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:458:4: ( 'do' )
            // AS3_ex.g3:458:6: 'do'
            {
                Match("do"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "DO"

    // $ANTLR start "ELSE"
    public void mELSE() // throws RecognitionException [2]
    {
            try
            {
            int _type = ELSE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:459:6: ( 'else' )
            // AS3_ex.g3:459:8: 'else'
            {
                Match("else"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "ELSE"

    // $ANTLR start "EXTENDS"
    public void mEXTENDS() // throws RecognitionException [2]
    {
            try
            {
            int _type = EXTENDS;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:460:9: ( 'extends' )
            // AS3_ex.g3:460:11: 'extends'
            {
                Match("extends"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "EXTENDS"

    // $ANTLR start "FALSE"
    public void mFALSE() // throws RecognitionException [2]
    {
            try
            {
            int _type = FALSE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:461:7: ( 'false' )
            // AS3_ex.g3:461:9: 'false'
            {
                Match("false"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "FALSE"

    // $ANTLR start "FINALLY"
    public void mFINALLY() // throws RecognitionException [2]
    {
            try
            {
            int _type = FINALLY;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:462:9: ( 'finally' )
            // AS3_ex.g3:462:11: 'finally'
            {
                Match("finally"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "FINALLY"

    // $ANTLR start "FOR"
    public void mFOR() // throws RecognitionException [2]
    {
            try
            {
            int _type = FOR;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:463:5: ( 'for' )
            // AS3_ex.g3:463:7: 'for'
            {
                Match("for"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "FOR"

    // $ANTLR start "FUNCTION"
    public void mFUNCTION() // throws RecognitionException [2]
    {
            try
            {
            int _type = FUNCTION;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:464:10: ( 'function' )
            // AS3_ex.g3:464:12: 'function'
            {
                Match("function"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "FUNCTION"

    // $ANTLR start "IF"
    public void mIF() // throws RecognitionException [2]
    {
            try
            {
            int _type = IF;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:465:4: ( 'if' )
            // AS3_ex.g3:465:6: 'if'
            {
                Match("if"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "IF"

    // $ANTLR start "IMPLEMENTS"
    public void mIMPLEMENTS() // throws RecognitionException [2]
    {
            try
            {
            int _type = IMPLEMENTS;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:466:12: ( 'implements' )
            // AS3_ex.g3:466:14: 'implements'
            {
                Match("implements"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "IMPLEMENTS"

    // $ANTLR start "IMPORT"
    public void mIMPORT() // throws RecognitionException [2]
    {
            try
            {
            int _type = IMPORT;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:467:8: ( 'import' )
            // AS3_ex.g3:467:10: 'import'
            {
                Match("import"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "IMPORT"

    // $ANTLR start "IN"
    public void mIN() // throws RecognitionException [2]
    {
            try
            {
            int _type = IN;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:468:4: ( 'in' )
            // AS3_ex.g3:468:6: 'in'
            {
                Match("in"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "IN"

    // $ANTLR start "INSTANCEOF"
    public void mINSTANCEOF() // throws RecognitionException [2]
    {
            try
            {
            int _type = INSTANCEOF;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:469:12: ( 'instanceof' )
            // AS3_ex.g3:469:14: 'instanceof'
            {
                Match("instanceof"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "INSTANCEOF"

    // $ANTLR start "INTERFACE"
    public void mINTERFACE() // throws RecognitionException [2]
    {
            try
            {
            int _type = INTERFACE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:470:11: ( 'interface' )
            // AS3_ex.g3:470:13: 'interface'
            {
                Match("interface"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "INTERFACE"

    // $ANTLR start "INTERNAL"
    public void mINTERNAL() // throws RecognitionException [2]
    {
            try
            {
            int _type = INTERNAL;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:471:10: ( 'internal' )
            // AS3_ex.g3:471:12: 'internal'
            {
                Match("internal"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "INTERNAL"

    // $ANTLR start "IS"
    public void mIS() // throws RecognitionException [2]
    {
            try
            {
            int _type = IS;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:472:4: ( 'is' )
            // AS3_ex.g3:472:6: 'is'
            {
                Match("is"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "IS"

    // $ANTLR start "NATIVE"
    public void mNATIVE() // throws RecognitionException [2]
    {
            try
            {
            int _type = NATIVE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:473:8: ( 'native' )
            // AS3_ex.g3:473:10: 'native'
            {
                Match("native"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "NATIVE"

    // $ANTLR start "NEW"
    public void mNEW() // throws RecognitionException [2]
    {
            try
            {
            int _type = NEW;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:474:5: ( 'new' )
            // AS3_ex.g3:474:7: 'new'
            {
                Match("new"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "NEW"

    // $ANTLR start "NULL"
    public void mNULL() // throws RecognitionException [2]
    {
            try
            {
            int _type = NULL;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:475:6: ( 'null' )
            // AS3_ex.g3:475:8: 'null'
            {
                Match("null"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "NULL"

    // $ANTLR start "PACKAGE"
    public void mPACKAGE() // throws RecognitionException [2]
    {
            try
            {
            int _type = PACKAGE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:476:9: ( 'package' )
            // AS3_ex.g3:476:11: 'package'
            {
                Match("package"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "PACKAGE"

    // $ANTLR start "PRIVATE"
    public void mPRIVATE() // throws RecognitionException [2]
    {
            try
            {
            int _type = PRIVATE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:477:9: ( 'private' )
            // AS3_ex.g3:477:11: 'private'
            {
                Match("private"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "PRIVATE"

    // $ANTLR start "PROTECTED"
    public void mPROTECTED() // throws RecognitionException [2]
    {
            try
            {
            int _type = PROTECTED;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:478:11: ( 'protected' )
            // AS3_ex.g3:478:13: 'protected'
            {
                Match("protected"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "PROTECTED"

    // $ANTLR start "PUBLIC"
    public void mPUBLIC() // throws RecognitionException [2]
    {
            try
            {
            int _type = PUBLIC;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:479:8: ( 'public' )
            // AS3_ex.g3:479:10: 'public'
            {
                Match("public"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "PUBLIC"

    // $ANTLR start "RETURN"
    public void mRETURN() // throws RecognitionException [2]
    {
            try
            {
            int _type = RETURN;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:480:8: ( 'return' )
            // AS3_ex.g3:480:10: 'return'
            {
                Match("return"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "RETURN"

    // $ANTLR start "SUPER"
    public void mSUPER() // throws RecognitionException [2]
    {
            try
            {
            int _type = SUPER;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:481:7: ( 'super' )
            // AS3_ex.g3:481:9: 'super'
            {
                Match("super"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "SUPER"

    // $ANTLR start "SWITCH"
    public void mSWITCH() // throws RecognitionException [2]
    {
            try
            {
            int _type = SWITCH;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:482:8: ( 'switch' )
            // AS3_ex.g3:482:10: 'switch'
            {
                Match("switch"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "SWITCH"

    // $ANTLR start "THIS"
    public void mTHIS() // throws RecognitionException [2]
    {
            try
            {
            int _type = THIS;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:483:6: ( 'this' )
            // AS3_ex.g3:483:8: 'this'
            {
                Match("this"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "THIS"

    // $ANTLR start "THROW"
    public void mTHROW() // throws RecognitionException [2]
    {
            try
            {
            int _type = THROW;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:484:7: ( 'throw' )
            // AS3_ex.g3:484:9: 'throw'
            {
                Match("throw"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "THROW"

    // $ANTLR start "TO"
    public void mTO() // throws RecognitionException [2]
    {
            try
            {
            int _type = TO;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:485:4: ( 'to' )
            // AS3_ex.g3:485:6: 'to'
            {
                Match("to"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "TO"

    // $ANTLR start "TRUE"
    public void mTRUE() // throws RecognitionException [2]
    {
            try
            {
            int _type = TRUE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:486:6: ( 'true' )
            // AS3_ex.g3:486:8: 'true'
            {
                Match("true"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "TRUE"

    // $ANTLR start "TRY"
    public void mTRY() // throws RecognitionException [2]
    {
            try
            {
            int _type = TRY;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:487:5: ( 'try' )
            // AS3_ex.g3:487:7: 'try'
            {
                Match("try"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "TRY"

    // $ANTLR start "TYPEOF"
    public void mTYPEOF() // throws RecognitionException [2]
    {
            try
            {
            int _type = TYPEOF;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:488:8: ( 'typeof' )
            // AS3_ex.g3:488:10: 'typeof'
            {
                Match("typeof"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "TYPEOF"

    // $ANTLR start "USE"
    public void mUSE() // throws RecognitionException [2]
    {
            try
            {
            int _type = USE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:489:5: ( 'use' )
            // AS3_ex.g3:489:7: 'use'
            {
                Match("use"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "USE"

    // $ANTLR start "VAR"
    public void mVAR() // throws RecognitionException [2]
    {
            try
            {
            int _type = VAR;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:490:5: ( 'var' )
            // AS3_ex.g3:490:7: 'var'
            {
                Match("var"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "VAR"

    // $ANTLR start "VOID"
    public void mVOID() // throws RecognitionException [2]
    {
            try
            {
            int _type = VOID;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:491:6: ( 'void' )
            // AS3_ex.g3:491:8: 'void'
            {
                Match("void"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "VOID"

    // $ANTLR start "WHILE"
    public void mWHILE() // throws RecognitionException [2]
    {
            try
            {
            int _type = WHILE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:492:7: ( 'while' )
            // AS3_ex.g3:492:9: 'while'
            {
                Match("while"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "WHILE"

    // $ANTLR start "WITH"
    public void mWITH() // throws RecognitionException [2]
    {
            try
            {
            int _type = WITH;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:493:6: ( 'with' )
            // AS3_ex.g3:493:8: 'with'
            {
                Match("with"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "WITH"

    // $ANTLR start "EACH"
    public void mEACH() // throws RecognitionException [2]
    {
            try
            {
            int _type = EACH;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:494:6: ( 'each' )
            // AS3_ex.g3:494:8: 'each'
            {
                Match("each"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "EACH"

    // $ANTLR start "GET"
    public void mGET() // throws RecognitionException [2]
    {
            try
            {
            int _type = GET;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:495:5: ( 'get' )
            // AS3_ex.g3:495:7: 'get'
            {
                Match("get"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "GET"

    // $ANTLR start "SET"
    public void mSET() // throws RecognitionException [2]
    {
            try
            {
            int _type = SET;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:496:5: ( 'set' )
            // AS3_ex.g3:496:7: 'set'
            {
                Match("set"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "SET"

    // $ANTLR start "NAMESPACE"
    public void mNAMESPACE() // throws RecognitionException [2]
    {
            try
            {
            int _type = NAMESPACE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:497:11: ( 'namespace' )
            // AS3_ex.g3:497:13: 'namespace'
            {
                Match("namespace"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "NAMESPACE"

    // $ANTLR start "INCLUDE"
    public void mINCLUDE() // throws RecognitionException [2]
    {
            try
            {
            int _type = INCLUDE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:498:9: ( 'include' )
            // AS3_ex.g3:498:11: 'include'
            {
                Match("include"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "INCLUDE"

    // $ANTLR start "DYNAMIC"
    public void mDYNAMIC() // throws RecognitionException [2]
    {
            try
            {
            int _type = DYNAMIC;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:499:9: ( 'dynamic' )
            // AS3_ex.g3:499:11: 'dynamic'
            {
                Match("dynamic"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "DYNAMIC"

    // $ANTLR start "FINAL"
    public void mFINAL() // throws RecognitionException [2]
    {
            try
            {
            int _type = FINAL;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:500:7: ( 'final' )
            // AS3_ex.g3:500:9: 'final'
            {
                Match("final"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "FINAL"

    // $ANTLR start "OVERRIDE"
    public void mOVERRIDE() // throws RecognitionException [2]
    {
            try
            {
            int _type = OVERRIDE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:501:10: ( 'override' )
            // AS3_ex.g3:501:12: 'override'
            {
                Match("override"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "OVERRIDE"

    // $ANTLR start "STATIC"
    public void mSTATIC() // throws RecognitionException [2]
    {
            try
            {
            int _type = STATIC;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:502:8: ( 'static' )
            // AS3_ex.g3:502:10: 'static'
            {
                Match("static"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "STATIC"

    // $ANTLR start "SEMI"
    public void mSEMI() // throws RecognitionException [2]
    {
            try
            {
            int _type = SEMI;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:503:6: ( ';' )
            // AS3_ex.g3:503:8: ';'
            {
                Match(';'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "SEMI"

    // $ANTLR start "LCURLY"
    public void mLCURLY() // throws RecognitionException [2]
    {
            try
            {
            int _type = LCURLY;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:504:8: ( '{' )
            // AS3_ex.g3:504:10: '{'
            {
                Match('{'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "LCURLY"

    // $ANTLR start "RCURLY"
    public void mRCURLY() // throws RecognitionException [2]
    {
            try
            {
            int _type = RCURLY;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:505:8: ( '}' )
            // AS3_ex.g3:505:10: '}'
            {
                Match('}'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "RCURLY"

    // $ANTLR start "LPAREN"
    public void mLPAREN() // throws RecognitionException [2]
    {
            try
            {
            int _type = LPAREN;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:506:8: ( '(' )
            // AS3_ex.g3:506:10: '('
            {
                Match('('); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "LPAREN"

    // $ANTLR start "RPAREN"
    public void mRPAREN() // throws RecognitionException [2]
    {
            try
            {
            int _type = RPAREN;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:507:8: ( ')' )
            // AS3_ex.g3:507:10: ')'
            {
                Match(')'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "RPAREN"

    // $ANTLR start "LBRACK"
    public void mLBRACK() // throws RecognitionException [2]
    {
            try
            {
            int _type = LBRACK;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:508:8: ( '[' )
            // AS3_ex.g3:508:10: '['
            {
                Match('['); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "LBRACK"

    // $ANTLR start "RBRACK"
    public void mRBRACK() // throws RecognitionException [2]
    {
            try
            {
            int _type = RBRACK;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:509:8: ( ']' )
            // AS3_ex.g3:509:10: ']'
            {
                Match(']'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "RBRACK"

    // $ANTLR start "DOT"
    public void mDOT() // throws RecognitionException [2]
    {
            try
            {
            int _type = DOT;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:510:5: ( '.' )
            // AS3_ex.g3:510:7: '.'
            {
                Match('.'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "DOT"

    // $ANTLR start "COMMA"
    public void mCOMMA() // throws RecognitionException [2]
    {
            try
            {
            int _type = COMMA;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:511:7: ( ',' )
            // AS3_ex.g3:511:9: ','
            {
                Match(','); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "COMMA"

    // $ANTLR start "LT"
    public void mLT() // throws RecognitionException [2]
    {
            try
            {
            int _type = LT;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:512:4: ( '<' )
            // AS3_ex.g3:512:6: '<'
            {
                Match('<'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "LT"

    // $ANTLR start "GT"
    public void mGT() // throws RecognitionException [2]
    {
            try
            {
            int _type = GT;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:513:4: ( '>' )
            // AS3_ex.g3:513:6: '>'
            {
                Match('>'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "GT"

    // $ANTLR start "LTE"
    public void mLTE() // throws RecognitionException [2]
    {
            try
            {
            int _type = LTE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:514:5: ( '<=' )
            // AS3_ex.g3:514:7: '<='
            {
                Match("<="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "LTE"

    // $ANTLR start "EQ"
    public void mEQ() // throws RecognitionException [2]
    {
            try
            {
            int _type = EQ;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:515:4: ( '==' )
            // AS3_ex.g3:515:6: '=='
            {
                Match("=="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "EQ"

    // $ANTLR start "NEQ"
    public void mNEQ() // throws RecognitionException [2]
    {
            try
            {
            int _type = NEQ;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:516:5: ( '!=' )
            // AS3_ex.g3:516:7: '!='
            {
                Match("!="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "NEQ"

    // $ANTLR start "SAME"
    public void mSAME() // throws RecognitionException [2]
    {
            try
            {
            int _type = SAME;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:517:6: ( '===' )
            // AS3_ex.g3:517:8: '==='
            {
                Match("==="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "SAME"

    // $ANTLR start "NSAME"
    public void mNSAME() // throws RecognitionException [2]
    {
            try
            {
            int _type = NSAME;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:518:7: ( '!==' )
            // AS3_ex.g3:518:9: '!=='
            {
                Match("!=="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "NSAME"

    // $ANTLR start "PLUS"
    public void mPLUS() // throws RecognitionException [2]
    {
            try
            {
            int _type = PLUS;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:519:6: ( '+' )
            // AS3_ex.g3:519:8: '+'
            {
                Match('+'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "PLUS"

    // $ANTLR start "SUB"
    public void mSUB() // throws RecognitionException [2]
    {
            try
            {
            int _type = SUB;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:520:5: ( '-' )
            // AS3_ex.g3:520:7: '-'
            {
                Match('-'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "SUB"

    // $ANTLR start "STAR"
    public void mSTAR() // throws RecognitionException [2]
    {
            try
            {
            int _type = STAR;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:521:6: ( '*' )
            // AS3_ex.g3:521:8: '*'
            {
                Match('*'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "STAR"

    // $ANTLR start "DIV"
    public void mDIV() // throws RecognitionException [2]
    {
            try
            {
            int _type = DIV;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:522:5: ( '/' )
            // AS3_ex.g3:522:7: '/'
            {
                Match('/'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "DIV"

    // $ANTLR start "MOD"
    public void mMOD() // throws RecognitionException [2]
    {
            try
            {
            int _type = MOD;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:523:5: ( '%' )
            // AS3_ex.g3:523:7: '%'
            {
                Match('%'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "MOD"

    // $ANTLR start "INC"
    public void mINC() // throws RecognitionException [2]
    {
            try
            {
            int _type = INC;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:524:5: ( '++' )
            // AS3_ex.g3:524:7: '++'
            {
                Match("++"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "INC"

    // $ANTLR start "DEC"
    public void mDEC() // throws RecognitionException [2]
    {
            try
            {
            int _type = DEC;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:525:5: ( '--' )
            // AS3_ex.g3:525:7: '--'
            {
                Match("--"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "DEC"

    // $ANTLR start "SHL"
    public void mSHL() // throws RecognitionException [2]
    {
            try
            {
            int _type = SHL;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:526:5: ( '<<' )
            // AS3_ex.g3:526:7: '<<'
            {
                Match("<<"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "SHL"

    // $ANTLR start "AND"
    public void mAND() // throws RecognitionException [2]
    {
            try
            {
            int _type = AND;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:527:5: ( '&' )
            // AS3_ex.g3:527:7: '&'
            {
                Match('&'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "AND"

    // $ANTLR start "OR"
    public void mOR() // throws RecognitionException [2]
    {
            try
            {
            int _type = OR;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:528:4: ( '|' )
            // AS3_ex.g3:528:6: '|'
            {
                Match('|'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "OR"

    // $ANTLR start "XOR"
    public void mXOR() // throws RecognitionException [2]
    {
            try
            {
            int _type = XOR;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:529:5: ( '^' )
            // AS3_ex.g3:529:7: '^'
            {
                Match('^'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "XOR"

    // $ANTLR start "NOT"
    public void mNOT() // throws RecognitionException [2]
    {
            try
            {
            int _type = NOT;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:530:5: ( '!' )
            // AS3_ex.g3:530:7: '!'
            {
                Match('!'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "NOT"

    // $ANTLR start "INV"
    public void mINV() // throws RecognitionException [2]
    {
            try
            {
            int _type = INV;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:531:5: ( '~' )
            // AS3_ex.g3:531:7: '~'
            {
                Match('~'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "INV"

    // $ANTLR start "LAND"
    public void mLAND() // throws RecognitionException [2]
    {
            try
            {
            int _type = LAND;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:532:6: ( '&&' )
            // AS3_ex.g3:532:8: '&&'
            {
                Match("&&"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "LAND"

    // $ANTLR start "LOR"
    public void mLOR() // throws RecognitionException [2]
    {
            try
            {
            int _type = LOR;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:533:5: ( '||' )
            // AS3_ex.g3:533:7: '||'
            {
                Match("||"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "LOR"

    // $ANTLR start "QUE"
    public void mQUE() // throws RecognitionException [2]
    {
            try
            {
            int _type = QUE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:534:5: ( '?' )
            // AS3_ex.g3:534:7: '?'
            {
                Match('?'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "QUE"

    // $ANTLR start "COLON"
    public void mCOLON() // throws RecognitionException [2]
    {
            try
            {
            int _type = COLON;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:535:7: ( ':' )
            // AS3_ex.g3:535:9: ':'
            {
                Match(':'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "COLON"

    // $ANTLR start "ASSIGN"
    public void mASSIGN() // throws RecognitionException [2]
    {
            try
            {
            int _type = ASSIGN;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:536:8: ( '=' )
            // AS3_ex.g3:536:10: '='
            {
                Match('='); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "ASSIGN"

    // $ANTLR start "DIV_ASSIGN"
    public void mDIV_ASSIGN() // throws RecognitionException [2]
    {
            try
            {
            int _type = DIV_ASSIGN;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:537:12: ( '/=' )
            // AS3_ex.g3:537:14: '/='
            {
                Match("/="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "DIV_ASSIGN"

    // $ANTLR start "MOD_ASSIGN"
    public void mMOD_ASSIGN() // throws RecognitionException [2]
    {
            try
            {
            int _type = MOD_ASSIGN;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:538:12: ( '%=' )
            // AS3_ex.g3:538:14: '%='
            {
                Match("%="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "MOD_ASSIGN"

    // $ANTLR start "ADD_ASSIGN"
    public void mADD_ASSIGN() // throws RecognitionException [2]
    {
            try
            {
            int _type = ADD_ASSIGN;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:539:12: ( '+=' )
            // AS3_ex.g3:539:14: '+='
            {
                Match("+="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "ADD_ASSIGN"

    // $ANTLR start "SUB_ASSIGN"
    public void mSUB_ASSIGN() // throws RecognitionException [2]
    {
            try
            {
            int _type = SUB_ASSIGN;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:540:12: ( '-=' )
            // AS3_ex.g3:540:14: '-='
            {
                Match("-="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "SUB_ASSIGN"

    // $ANTLR start "SHL_ASSIGN"
    public void mSHL_ASSIGN() // throws RecognitionException [2]
    {
            try
            {
            int _type = SHL_ASSIGN;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:541:12: ( '<<=' )
            // AS3_ex.g3:541:14: '<<='
            {
                Match("<<="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "SHL_ASSIGN"

    // $ANTLR start "LAND_ASSIGN"
    public void mLAND_ASSIGN() // throws RecognitionException [2]
    {
            try
            {
            int _type = LAND_ASSIGN;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:542:13: ( '&&=' )
            // AS3_ex.g3:542:15: '&&='
            {
                Match("&&="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "LAND_ASSIGN"

    // $ANTLR start "LOR_ASSIGN"
    public void mLOR_ASSIGN() // throws RecognitionException [2]
    {
            try
            {
            int _type = LOR_ASSIGN;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:543:12: ( '||=' )
            // AS3_ex.g3:543:14: '||='
            {
                Match("||="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "LOR_ASSIGN"

    // $ANTLR start "AND_ASSIGN"
    public void mAND_ASSIGN() // throws RecognitionException [2]
    {
            try
            {
            int _type = AND_ASSIGN;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:544:12: ( '&=' )
            // AS3_ex.g3:544:14: '&='
            {
                Match("&="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "AND_ASSIGN"

    // $ANTLR start "XOR_ASSIGN"
    public void mXOR_ASSIGN() // throws RecognitionException [2]
    {
            try
            {
            int _type = XOR_ASSIGN;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:545:12: ( '^=' )
            // AS3_ex.g3:545:14: '^='
            {
                Match("^="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "XOR_ASSIGN"

    // $ANTLR start "OR_ASSIGN"
    public void mOR_ASSIGN() // throws RecognitionException [2]
    {
            try
            {
            int _type = OR_ASSIGN;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:546:11: ( '|=' )
            // AS3_ex.g3:546:13: '|='
            {
                Match("|="); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "OR_ASSIGN"

    // $ANTLR start "ELLIPSIS"
    public void mELLIPSIS() // throws RecognitionException [2]
    {
            try
            {
            int _type = ELLIPSIS;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:547:10: ( '...' )
            // AS3_ex.g3:547:12: '...'
            {
                Match("..."); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "ELLIPSIS"

    // $ANTLR start "XML_ELLIPSIS"
    public void mXML_ELLIPSIS() // throws RecognitionException [2]
    {
            try
            {
            int _type = XML_ELLIPSIS;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:548:14: ( '..' )
            // AS3_ex.g3:548:16: '..'
            {
                Match(".."); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "XML_ELLIPSIS"

    // $ANTLR start "XML_TEND"
    public void mXML_TEND() // throws RecognitionException [2]
    {
            try
            {
            int _type = XML_TEND;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:549:10: ( '/>' )
            // AS3_ex.g3:549:12: '/>'
            {
                Match("/>"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "XML_TEND"

    // $ANTLR start "XML_E_TEND"
    public void mXML_E_TEND() // throws RecognitionException [2]
    {
            try
            {
            int _type = XML_E_TEND;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:550:12: ( '</' )
            // AS3_ex.g3:550:14: '</'
            {
                Match("</"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "XML_E_TEND"

    // $ANTLR start "XML_NS_OP"
    public void mXML_NS_OP() // throws RecognitionException [2]
    {
            try
            {
            int _type = XML_NS_OP;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:551:11: ( '::' )
            // AS3_ex.g3:551:13: '::'
            {
                Match("::"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "XML_NS_OP"

    // $ANTLR start "XML_AT"
    public void mXML_AT() // throws RecognitionException [2]
    {
            try
            {
            int _type = XML_AT;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:552:8: ( '@' )
            // AS3_ex.g3:552:10: '@'
            {
                Match('@'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "XML_AT"

    // $ANTLR start "XML_LS_STD"
    public void mXML_LS_STD() // throws RecognitionException [2]
    {
            try
            {
            int _type = XML_LS_STD;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:553:12: ( '<>' )
            // AS3_ex.g3:553:14: '<>'
            {
                Match("<>"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "XML_LS_STD"

    // $ANTLR start "XML_LS_END"
    public void mXML_LS_END() // throws RecognitionException [2]
    {
            try
            {
            int _type = XML_LS_END;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:554:12: ( '</>' )
            // AS3_ex.g3:554:14: '</>'
            {
                Match("</>"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "XML_LS_END"

    // $ANTLR start "UNDERSCORE"
    public void mUNDERSCORE() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1370:22: ( '_' )
            // AS3_ex.g3:1370:24: '_'
            {
                Match('_'); if (state.failed) return ;

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "UNDERSCORE"

    // $ANTLR start "DOLLAR"
    public void mDOLLAR() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1371:22: ( '$' )
            // AS3_ex.g3:1371:24: '$'
            {
                Match('$'); if (state.failed) return ;

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "DOLLAR"

    // $ANTLR start "ALPHABET"
    public void mALPHABET() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1373:30: ( 'a' .. 'z' | 'A' .. 'Z' )
            // AS3_ex.g3:
            {
                if ( (input.LA(1) >= 'A' && input.LA(1) <= 'Z') || (input.LA(1) >= 'a' && input.LA(1) <= 'z') ) 
                {
                    input.Consume();
                state.failed = false;
                }
                else 
                {
                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    MismatchedSetException mse = new MismatchedSetException(null,input);
                    Recover(mse);
                    throw mse;}


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "ALPHABET"

    // $ANTLR start "NUMBER"
    public void mNUMBER() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1375:30: ( '0' .. '9' )
            // AS3_ex.g3:1375:35: '0' .. '9'
            {
                MatchRange('0','9'); if (state.failed) return ;

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "NUMBER"

    // $ANTLR start "HEX_DIGIT"
    public void mHEX_DIGIT() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1377:30: ( ( '0' .. '9' | 'a' .. 'f' | 'A' .. 'F' ) )
            // AS3_ex.g3:1377:35: ( '0' .. '9' | 'a' .. 'f' | 'A' .. 'F' )
            {
                if ( (input.LA(1) >= '0' && input.LA(1) <= '9') || (input.LA(1) >= 'A' && input.LA(1) <= 'F') || (input.LA(1) >= 'a' && input.LA(1) <= 'f') ) 
                {
                    input.Consume();
                state.failed = false;
                }
                else 
                {
                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    MismatchedSetException mse = new MismatchedSetException(null,input);
                    Recover(mse);
                    throw mse;}


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "HEX_DIGIT"

    // $ANTLR start "CR"
    public void mCR() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1379:30: ( '\\r' )
            // AS3_ex.g3:1379:35: '\\r'
            {
                Match('\r'); if (state.failed) return ;

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "CR"

    // $ANTLR start "LF"
    public void mLF() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1381:30: ( '\\n' )
            // AS3_ex.g3:1381:35: '\\n'
            {
                Match('\n'); if (state.failed) return ;

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "LF"

    // $ANTLR start "UNICODE_ESCAPE"
    public void mUNICODE_ESCAPE() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1383:30: ( '\\\\' 'u' HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT )
            // AS3_ex.g3:1383:35: '\\\\' 'u' HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT
            {
                Match('\\'); if (state.failed) return ;
                Match('u'); if (state.failed) return ;
                mHEX_DIGIT(); if (state.failed) return ;
                mHEX_DIGIT(); if (state.failed) return ;
                mHEX_DIGIT(); if (state.failed) return ;
                mHEX_DIGIT(); if (state.failed) return ;

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "UNICODE_ESCAPE"

    // $ANTLR start "ESCAPE_SEQUENCE"
    public void mESCAPE_SEQUENCE() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1386:30: ( '\\\\' '\\\\' | '\\\\' ~ ( '\\\\' ) )
            int alt1 = 2;
            int LA1_0 = input.LA(1);

            if ( (LA1_0 == '\\') )
            {
                int LA1_1 = input.LA(2);

                if ( (LA1_1 == '\\') )
                {
                    alt1 = 1;
                }
                else if ( ((LA1_1 >= '\u0000' && LA1_1 <= '[') || (LA1_1 >= ']' && LA1_1 <= '\uFFFF')) )
                {
                    alt1 = 2;
                }
                else 
                {
                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    NoViableAltException nvae_d1s1 =
                        new NoViableAltException("", 1, 1, input);

                    throw nvae_d1s1;
                }
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d1s0 =
                    new NoViableAltException("", 1, 0, input);

                throw nvae_d1s0;
            }
            switch (alt1) 
            {
                case 1 :
                    // AS3_ex.g3:1389:31: '\\\\' '\\\\'
                    {
                        Match('\\'); if (state.failed) return ;
                        Match('\\'); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // AS3_ex.g3:1390:32: '\\\\' ~ ( '\\\\' )
                    {
                        Match('\\'); if (state.failed) return ;
                        if ( (input.LA(1) >= '\u0000' && input.LA(1) <= '[') || (input.LA(1) >= ']' && input.LA(1) <= '\uFFFF') ) 
                        {
                            input.Consume();
                        state.failed = false;
                        }
                        else 
                        {
                            if ( state.backtracking > 0 ) {state.failed = true; return ;}
                            MismatchedSetException mse = new MismatchedSetException(null,input);
                            Recover(mse);
                            throw mse;}


                    }
                    break;

            }
        }
        finally 
        {
        }
    }
    // $ANTLR end "ESCAPE_SEQUENCE"

    // $ANTLR start "EOL"
    public void mEOL() // throws RecognitionException [2]
    {
            try
            {
            int _type = EOL;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:1397:5: ( ( CR LF | CR | LF ) )
            // AS3_ex.g3:1397:10: ( CR LF | CR | LF )
            {
                // AS3_ex.g3:1397:10: ( CR LF | CR | LF )
                int alt2 = 3;
                int LA2_0 = input.LA(1);

                if ( (LA2_0 == '\r') )
                {
                    int LA2_1 = input.LA(2);

                    if ( (LA2_1 == '\n') )
                    {
                        alt2 = 1;
                    }
                    else 
                    {
                        alt2 = 2;}
                }
                else if ( (LA2_0 == '\n') )
                {
                    alt2 = 3;
                }
                else 
                {
                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    NoViableAltException nvae_d2s0 =
                        new NoViableAltException("", 2, 0, input);

                    throw nvae_d2s0;
                }
                switch (alt2) 
                {
                    case 1 :
                        // AS3_ex.g3:1397:11: CR LF
                        {
                            mCR(); if (state.failed) return ;
                            mLF(); if (state.failed) return ;

                        }
                        break;
                    case 2 :
                        // AS3_ex.g3:1397:19: CR
                        {
                            mCR(); if (state.failed) return ;

                        }
                        break;
                    case 3 :
                        // AS3_ex.g3:1397:24: LF
                        {
                            mLF(); if (state.failed) return ;

                        }
                        break;

                }

                if ( state.backtracking == 0 ) 
                {
                   _channel = AS3_exParser.CHANNEL_EOL; 
                }

            }

            state.type = _type;
            state.channel = _channel;
            if ( state.backtracking == 0 ) 
            {

                  debugMethod("EOL",Text);

            }    }
        finally 
        {
        }
    }
    // $ANTLR end "EOL"

    // $ANTLR start "WHITESPACE"
    public void mWHITESPACE() // throws RecognitionException [2]
    {
            try
            {
            int _type = WHITESPACE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:1404:5: ( ( ( '\\u0020' | '\\u0009' | '\\u000B' | '\\u00A0' | '\\u000C' ) | ( '\\u001C' .. '\\u001F' ) )+ )
            // AS3_ex.g3:1404:9: ( ( '\\u0020' | '\\u0009' | '\\u000B' | '\\u00A0' | '\\u000C' ) | ( '\\u001C' .. '\\u001F' ) )+
            {
                // AS3_ex.g3:1404:9: ( ( '\\u0020' | '\\u0009' | '\\u000B' | '\\u00A0' | '\\u000C' ) | ( '\\u001C' .. '\\u001F' ) )+
                int cnt3 = 0;
                do 
                {
                    int alt3 = 3;
                    int LA3_0 = input.LA(1);

                    if ( (LA3_0 == '\t' || (LA3_0 >= '\u000B' && LA3_0 <= '\f') || LA3_0 == ' ' || LA3_0 == '\u00A0') )
                    {
                        alt3 = 1;
                    }
                    else if ( ((LA3_0 >= '\u001C' && LA3_0 <= '\u001F')) )
                    {
                        alt3 = 2;
                    }


                    switch (alt3) 
                    {
                        case 1 :
                            // AS3_ex.g3:1404:10: ( '\\u0020' | '\\u0009' | '\\u000B' | '\\u00A0' | '\\u000C' )
                            {
                                if ( input.LA(1) == '\t' || (input.LA(1) >= '\u000B' && input.LA(1) <= '\f') || input.LA(1) == ' ' || input.LA(1) == '\u00A0' ) 
                                {
                                    input.Consume();
                                state.failed = false;
                                }
                                else 
                                {
                                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                                    MismatchedSetException mse = new MismatchedSetException(null,input);
                                    Recover(mse);
                                    throw mse;}


                            }
                            break;
                        case 2 :
                            // AS3_ex.g3:1404:57: ( '\\u001C' .. '\\u001F' )
                            {
                                // AS3_ex.g3:1404:57: ( '\\u001C' .. '\\u001F' )
                                // AS3_ex.g3:1404:58: '\\u001C' .. '\\u001F'
                                {
                                    MatchRange('\u001C','\u001F'); if (state.failed) return ;

                                }


                            }
                            break;

                        default:
                            if ( cnt3 >= 1 ) goto loop3;
                            if ( state.backtracking > 0 ) {state.failed = true; return ;}
                                EarlyExitException eee =
                                    new EarlyExitException(3, input);
                                throw eee;
                    }
                    cnt3++;
                } while (true);

                loop3:
                    ;   // Stops C# compiler whinging that label 'loop3' has no statements

                if ( state.backtracking == 0 ) 
                {
                   _channel = AS3_exParser.CHANNEL_WHITESPACE; 
                }

            }

            state.type = _type;
            state.channel = _channel;
            if ( state.backtracking == 0 ) 
            {

                  debugMethod("WHITESPACE",Text);

            }    }
        finally 
        {
        }
    }
    // $ANTLR end "WHITESPACE"

    // $ANTLR start "COMMENT_MULTILINE"
    public void mCOMMENT_MULTILINE() // throws RecognitionException [2]
    {
            try
            {
            int _type = COMMENT_MULTILINE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:1411:5: ( '/*' ( options {greedy=false; } : . )* '*/' )
            // AS3_ex.g3:1411:9: '/*' ( options {greedy=false; } : . )* '*/'
            {
                Match("/*"); if (state.failed) return ;

                // AS3_ex.g3:1411:14: ( options {greedy=false; } : . )*
                do 
                {
                    int alt4 = 2;
                    int LA4_0 = input.LA(1);

                    if ( (LA4_0 == '*') )
                    {
                        int LA4_1 = input.LA(2);

                        if ( (LA4_1 == '/') )
                        {
                            alt4 = 2;
                        }
                        else if ( ((LA4_1 >= '\u0000' && LA4_1 <= '.') || (LA4_1 >= '0' && LA4_1 <= '\uFFFF')) )
                        {
                            alt4 = 1;
                        }


                    }
                    else if ( ((LA4_0 >= '\u0000' && LA4_0 <= ')') || (LA4_0 >= '+' && LA4_0 <= '\uFFFF')) )
                    {
                        alt4 = 1;
                    }


                    switch (alt4) 
                    {
                        case 1 :
                            // AS3_ex.g3:1411:42: .
                            {
                                MatchAny(); if (state.failed) return ;

                            }
                            break;

                        default:
                            goto loop4;
                    }
                } while (true);

                loop4:
                    ;   // Stops C# compiler whining that label 'loop4' has no statements

                Match("*/"); if (state.failed) return ;

                if ( state.backtracking == 0 ) 
                {
                   _channel = AS3_exParser.CHANNEL_MLCOMMENT; 
                }

            }

            state.type = _type;
            state.channel = _channel;
            if ( state.backtracking == 0 ) 
            {

                  debugMethod("COMMENT_MULTILINE",Text);

            }    }
        finally 
        {
        }
    }
    // $ANTLR end "COMMENT_MULTILINE"

    // $ANTLR start "COMMENT_SINGLELINE"
    public void mCOMMENT_SINGLELINE() // throws RecognitionException [2]
    {
            try
            {
            int _type = COMMENT_SINGLELINE;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:1417:5: ( '//' (~ ( CR | LF ) )* ( CR LF | CR | LF ) )
            // AS3_ex.g3:1417:9: '//' (~ ( CR | LF ) )* ( CR LF | CR | LF )
            {
                Match("//"); if (state.failed) return ;

                // AS3_ex.g3:1417:14: (~ ( CR | LF ) )*
                do 
                {
                    int alt5 = 2;
                    int LA5_0 = input.LA(1);

                    if ( ((LA5_0 >= '\u0000' && LA5_0 <= '\t') || (LA5_0 >= '\u000B' && LA5_0 <= '\f') || (LA5_0 >= '\u000E' && LA5_0 <= '\uFFFF')) )
                    {
                        alt5 = 1;
                    }


                    switch (alt5) 
                    {
                        case 1 :
                            // AS3_ex.g3:1417:14: ~ ( CR | LF )
                            {
                                if ( (input.LA(1) >= '\u0000' && input.LA(1) <= '\t') || (input.LA(1) >= '\u000B' && input.LA(1) <= '\f') || (input.LA(1) >= '\u000E' && input.LA(1) <= '\uFFFF') ) 
                                {
                                    input.Consume();
                                state.failed = false;
                                }
                                else 
                                {
                                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                                    MismatchedSetException mse = new MismatchedSetException(null,input);
                                    Recover(mse);
                                    throw mse;}


                            }
                            break;

                        default:
                            goto loop5;
                    }
                } while (true);

                loop5:
                    ;   // Stops C# compiler whining that label 'loop5' has no statements

                // AS3_ex.g3:1417:28: ( CR LF | CR | LF )
                int alt6 = 3;
                int LA6_0 = input.LA(1);

                if ( (LA6_0 == '\r') )
                {
                    int LA6_1 = input.LA(2);

                    if ( (LA6_1 == '\n') )
                    {
                        alt6 = 1;
                    }
                    else 
                    {
                        alt6 = 2;}
                }
                else if ( (LA6_0 == '\n') )
                {
                    alt6 = 3;
                }
                else 
                {
                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    NoViableAltException nvae_d6s0 =
                        new NoViableAltException("", 6, 0, input);

                    throw nvae_d6s0;
                }
                switch (alt6) 
                {
                    case 1 :
                        // AS3_ex.g3:1417:29: CR LF
                        {
                            mCR(); if (state.failed) return ;
                            mLF(); if (state.failed) return ;

                        }
                        break;
                    case 2 :
                        // AS3_ex.g3:1417:37: CR
                        {
                            mCR(); if (state.failed) return ;

                        }
                        break;
                    case 3 :
                        // AS3_ex.g3:1417:42: LF
                        {
                            mLF(); if (state.failed) return ;

                        }
                        break;

                }

                if ( state.backtracking == 0 ) 
                {
                   _channel = AS3_exParser.CHANNEL_SLCOMMENT; 
                }

            }

            state.type = _type;
            state.channel = _channel;
            if ( state.backtracking == 0 ) 
            {

                  debugMethod("COMMENT_SINGLELINE",Text);

            }    }
        finally 
        {
        }
    }
    // $ANTLR end "COMMENT_SINGLELINE"

    // $ANTLR start "SINGLE_QUOTE_LITERAL"
    public void mSINGLE_QUOTE_LITERAL() // throws RecognitionException [2]
    {
            try
            {
            int _type = SINGLE_QUOTE_LITERAL;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:1425:5: ( '\\'' ( ESCAPE_SEQUENCE | ~ ( '\\\\' | '\\'' ) )* '\\'' )
            // AS3_ex.g3:1425:9: '\\'' ( ESCAPE_SEQUENCE | ~ ( '\\\\' | '\\'' ) )* '\\''
            {
                Match('\''); if (state.failed) return ;
                // AS3_ex.g3:1425:14: ( ESCAPE_SEQUENCE | ~ ( '\\\\' | '\\'' ) )*
                do 
                {
                    int alt7 = 3;
                    int LA7_0 = input.LA(1);

                    if ( (LA7_0 == '\\') )
                    {
                        alt7 = 1;
                    }
                    else if ( ((LA7_0 >= '\u0000' && LA7_0 <= '&') || (LA7_0 >= '(' && LA7_0 <= '[') || (LA7_0 >= ']' && LA7_0 <= '\uFFFF')) )
                    {
                        alt7 = 2;
                    }


                    switch (alt7) 
                    {
                        case 1 :
                            // AS3_ex.g3:1425:16: ESCAPE_SEQUENCE
                            {
                                mESCAPE_SEQUENCE(); if (state.failed) return ;

                            }
                            break;
                        case 2 :
                            // AS3_ex.g3:1425:34: ~ ( '\\\\' | '\\'' )
                            {
                                if ( (input.LA(1) >= '\u0000' && input.LA(1) <= '&') || (input.LA(1) >= '(' && input.LA(1) <= '[') || (input.LA(1) >= ']' && input.LA(1) <= '\uFFFF') ) 
                                {
                                    input.Consume();
                                state.failed = false;
                                }
                                else 
                                {
                                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                                    MismatchedSetException mse = new MismatchedSetException(null,input);
                                    Recover(mse);
                                    throw mse;}


                            }
                            break;

                        default:
                            goto loop7;
                    }
                } while (true);

                loop7:
                    ;   // Stops C# compiler whining that label 'loop7' has no statements

                Match('\''); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
            if ( state.backtracking == 0 ) 
            {

                  debugMethod("SINGLE_QUOTE_LITERAL",Text);

            }    }
        finally 
        {
        }
    }
    // $ANTLR end "SINGLE_QUOTE_LITERAL"

    // $ANTLR start "DOUBLE_QUOTE_LITERAL"
    public void mDOUBLE_QUOTE_LITERAL() // throws RecognitionException [2]
    {
            try
            {
            int _type = DOUBLE_QUOTE_LITERAL;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:1430:5: ( '\"' ( ESCAPE_SEQUENCE | ~ ( '\\\\' | '\"' ) )* '\"' )
            // AS3_ex.g3:1430:9: '\"' ( ESCAPE_SEQUENCE | ~ ( '\\\\' | '\"' ) )* '\"'
            {
                Match('\"'); if (state.failed) return ;
                // AS3_ex.g3:1430:14: ( ESCAPE_SEQUENCE | ~ ( '\\\\' | '\"' ) )*
                do 
                {
                    int alt8 = 3;
                    int LA8_0 = input.LA(1);

                    if ( (LA8_0 == '\\') )
                    {
                        alt8 = 1;
                    }
                    else if ( ((LA8_0 >= '\u0000' && LA8_0 <= '!') || (LA8_0 >= '#' && LA8_0 <= '[') || (LA8_0 >= ']' && LA8_0 <= '\uFFFF')) )
                    {
                        alt8 = 2;
                    }


                    switch (alt8) 
                    {
                        case 1 :
                            // AS3_ex.g3:1430:16: ESCAPE_SEQUENCE
                            {
                                mESCAPE_SEQUENCE(); if (state.failed) return ;

                            }
                            break;
                        case 2 :
                            // AS3_ex.g3:1430:34: ~ ( '\\\\' | '\"' )
                            {
                                if ( (input.LA(1) >= '\u0000' && input.LA(1) <= '!') || (input.LA(1) >= '#' && input.LA(1) <= '[') || (input.LA(1) >= ']' && input.LA(1) <= '\uFFFF') ) 
                                {
                                    input.Consume();
                                state.failed = false;
                                }
                                else 
                                {
                                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                                    MismatchedSetException mse = new MismatchedSetException(null,input);
                                    Recover(mse);
                                    throw mse;}


                            }
                            break;

                        default:
                            goto loop8;
                    }
                } while (true);

                loop8:
                    ;   // Stops C# compiler whining that label 'loop8' has no statements

                Match('\"'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
            if ( state.backtracking == 0 ) 
            {

                  debugMethod("DOUBLE_QUOTE_LITERAL",Text);

            }    }
        finally 
        {
        }
    }
    // $ANTLR end "DOUBLE_QUOTE_LITERAL"

    // $ANTLR start "REGULAR_EXPR_LITERAL"
    public void mREGULAR_EXPR_LITERAL() // throws RecognitionException [2]
    {
            try
            {
            int _type = REGULAR_EXPR_LITERAL;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:1440:5: ({...}? => DIV REGULAR_EXPR_BODY DIV ( REGULAR_EXPR_FLAG )* )
            // AS3_ex.g3:1440:9: {...}? => DIV REGULAR_EXPR_BODY DIV ( REGULAR_EXPR_FLAG )*
            {
                if ( !((isRegularExpression())) ) 
                {
                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    throw new FailedPredicateException(input, "REGULAR_EXPR_LITERAL", "isRegularExpression()");
                }
                mDIV(); if (state.failed) return ;
                mREGULAR_EXPR_BODY(); if (state.failed) return ;
                mDIV(); if (state.failed) return ;
                // AS3_ex.g3:1440:63: ( REGULAR_EXPR_FLAG )*
                do 
                {
                    int alt9 = 2;
                    int LA9_0 = input.LA(1);

                    if ( (LA9_0 == '$' || (LA9_0 >= '0' && LA9_0 <= '9') || (LA9_0 >= 'A' && LA9_0 <= 'Z') || LA9_0 == '_' || (LA9_0 >= 'a' && LA9_0 <= 'z')) )
                    {
                        alt9 = 1;
                    }
                    else if ( ((isUnicodeIdentifierPart(input.LA(1)))) )
                    {
                        alt9 = 1;
                    }


                    switch (alt9) 
                    {
                        case 1 :
                            // AS3_ex.g3:1440:63: REGULAR_EXPR_FLAG
                            {
                                mREGULAR_EXPR_FLAG(); if (state.failed) return ;

                            }
                            break;

                        default:
                            goto loop9;
                    }
                } while (true);

                loop9:
                    ;   // Stops C# compiler whining that label 'loop9' has no statements


            }

            state.type = _type;
            state.channel = _channel;
            if ( state.backtracking == 0 ) 
            {

                  debugMethod("REGULAR_EXPR_LITERAL",Text);

            }    }
        finally 
        {
        }
    }
    // $ANTLR end "REGULAR_EXPR_LITERAL"

    // $ANTLR start "REGULAR_EXPR_BODY"
    public void mREGULAR_EXPR_BODY() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1444:5: ( REGULAR_EXPR_FIRST_CHAR ( REGULAR_EXPR_CHAR )* )
            // AS3_ex.g3:1444:9: REGULAR_EXPR_FIRST_CHAR ( REGULAR_EXPR_CHAR )*
            {
                mREGULAR_EXPR_FIRST_CHAR(); if (state.failed) return ;
                // AS3_ex.g3:1444:33: ( REGULAR_EXPR_CHAR )*
                do 
                {
                    int alt10 = 2;
                    int LA10_0 = input.LA(1);

                    if ( ((LA10_0 >= '\u0000' && LA10_0 <= '\t') || (LA10_0 >= '\u000B' && LA10_0 <= '\f') || (LA10_0 >= '\u000E' && LA10_0 <= '.') || (LA10_0 >= '0' && LA10_0 <= '\uFFFF')) )
                    {
                        alt10 = 1;
                    }


                    switch (alt10) 
                    {
                        case 1 :
                            // AS3_ex.g3:1444:33: REGULAR_EXPR_CHAR
                            {
                                mREGULAR_EXPR_CHAR(); if (state.failed) return ;

                            }
                            break;

                        default:
                            goto loop10;
                    }
                } while (true);

                loop10:
                    ;   // Stops C# compiler whining that label 'loop10' has no statements


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "REGULAR_EXPR_BODY"

    // $ANTLR start "REGULAR_EXPR_FIRST_CHAR"
    public void mREGULAR_EXPR_FIRST_CHAR() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1449:5: (~ ( CR | LF | '*' | '\\\\' | '/' ) | BACKSLASH_SEQUENCE )
            int alt11 = 2;
            int LA11_0 = input.LA(1);

            if ( ((LA11_0 >= '\u0000' && LA11_0 <= '\t') || (LA11_0 >= '\u000B' && LA11_0 <= '\f') || (LA11_0 >= '\u000E' && LA11_0 <= ')') || (LA11_0 >= '+' && LA11_0 <= '.') || (LA11_0 >= '0' && LA11_0 <= '[') || (LA11_0 >= ']' && LA11_0 <= '\uFFFF')) )
            {
                alt11 = 1;
            }
            else if ( (LA11_0 == '\\') )
            {
                alt11 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d11s0 =
                    new NoViableAltException("", 11, 0, input);

                throw nvae_d11s0;
            }
            switch (alt11) 
            {
                case 1 :
                    // AS3_ex.g3:1449:9: ~ ( CR | LF | '*' | '\\\\' | '/' )
                    {
                        if ( (input.LA(1) >= '\u0000' && input.LA(1) <= '\t') || (input.LA(1) >= '\u000B' && input.LA(1) <= '\f') || (input.LA(1) >= '\u000E' && input.LA(1) <= ')') || (input.LA(1) >= '+' && input.LA(1) <= '.') || (input.LA(1) >= '0' && input.LA(1) <= '[') || (input.LA(1) >= ']' && input.LA(1) <= '\uFFFF') ) 
                        {
                            input.Consume();
                        state.failed = false;
                        }
                        else 
                        {
                            if ( state.backtracking > 0 ) {state.failed = true; return ;}
                            MismatchedSetException mse = new MismatchedSetException(null,input);
                            Recover(mse);
                            throw mse;}


                    }
                    break;
                case 2 :
                    // AS3_ex.g3:1450:9: BACKSLASH_SEQUENCE
                    {
                        mBACKSLASH_SEQUENCE(); if (state.failed) return ;

                    }
                    break;

            }
        }
        finally 
        {
        }
    }
    // $ANTLR end "REGULAR_EXPR_FIRST_CHAR"

    // $ANTLR start "REGULAR_EXPR_CHAR"
    public void mREGULAR_EXPR_CHAR() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1454:5: (~ ( CR | LF | '\\\\' | '/' ) | BACKSLASH_SEQUENCE )
            int alt12 = 2;
            int LA12_0 = input.LA(1);

            if ( ((LA12_0 >= '\u0000' && LA12_0 <= '\t') || (LA12_0 >= '\u000B' && LA12_0 <= '\f') || (LA12_0 >= '\u000E' && LA12_0 <= '.') || (LA12_0 >= '0' && LA12_0 <= '[') || (LA12_0 >= ']' && LA12_0 <= '\uFFFF')) )
            {
                alt12 = 1;
            }
            else if ( (LA12_0 == '\\') )
            {
                alt12 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d12s0 =
                    new NoViableAltException("", 12, 0, input);

                throw nvae_d12s0;
            }
            switch (alt12) 
            {
                case 1 :
                    // AS3_ex.g3:1454:9: ~ ( CR | LF | '\\\\' | '/' )
                    {
                        if ( (input.LA(1) >= '\u0000' && input.LA(1) <= '\t') || (input.LA(1) >= '\u000B' && input.LA(1) <= '\f') || (input.LA(1) >= '\u000E' && input.LA(1) <= '.') || (input.LA(1) >= '0' && input.LA(1) <= '[') || (input.LA(1) >= ']' && input.LA(1) <= '\uFFFF') ) 
                        {
                            input.Consume();
                        state.failed = false;
                        }
                        else 
                        {
                            if ( state.backtracking > 0 ) {state.failed = true; return ;}
                            MismatchedSetException mse = new MismatchedSetException(null,input);
                            Recover(mse);
                            throw mse;}


                    }
                    break;
                case 2 :
                    // AS3_ex.g3:1455:9: BACKSLASH_SEQUENCE
                    {
                        mBACKSLASH_SEQUENCE(); if (state.failed) return ;

                    }
                    break;

            }
        }
        finally 
        {
        }
    }
    // $ANTLR end "REGULAR_EXPR_CHAR"

    // $ANTLR start "BACKSLASH_SEQUENCE"
    public void mBACKSLASH_SEQUENCE() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1458:28: ( '\\\\' ~ ( CR | LF ) )
            // AS3_ex.g3:1458:33: '\\\\' ~ ( CR | LF )
            {
                Match('\\'); if (state.failed) return ;
                if ( (input.LA(1) >= '\u0000' && input.LA(1) <= '\t') || (input.LA(1) >= '\u000B' && input.LA(1) <= '\f') || (input.LA(1) >= '\u000E' && input.LA(1) <= '\uFFFF') ) 
                {
                    input.Consume();
                state.failed = false;
                }
                else 
                {
                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    MismatchedSetException mse = new MismatchedSetException(null,input);
                    Recover(mse);
                    throw mse;}


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "BACKSLASH_SEQUENCE"

    // $ANTLR start "REGULAR_EXPR_FLAG"
    public void mREGULAR_EXPR_FLAG() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1460:28: ( IDENT_PART )
            // AS3_ex.g3:1460:33: IDENT_PART
            {
                mIDENT_PART(); if (state.failed) return ;

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "REGULAR_EXPR_FLAG"

    // $ANTLR start "HEX_NUMBER_LITERAL"
    public void mHEX_NUMBER_LITERAL() // throws RecognitionException [2]
    {
            try
            {
            int _type = HEX_NUMBER_LITERAL;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:1470:5: ( '0' ( 'X' | 'x' ) ( HEX_DIGIT )+ )
            // AS3_ex.g3:1470:7: '0' ( 'X' | 'x' ) ( HEX_DIGIT )+
            {
                Match('0'); if (state.failed) return ;
                if ( input.LA(1) == 'X' || input.LA(1) == 'x' ) 
                {
                    input.Consume();
                state.failed = false;
                }
                else 
                {
                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    MismatchedSetException mse = new MismatchedSetException(null,input);
                    Recover(mse);
                    throw mse;}

                // AS3_ex.g3:1470:21: ( HEX_DIGIT )+
                int cnt13 = 0;
                do 
                {
                    int alt13 = 2;
                    int LA13_0 = input.LA(1);

                    if ( ((LA13_0 >= '0' && LA13_0 <= '9') || (LA13_0 >= 'A' && LA13_0 <= 'F') || (LA13_0 >= 'a' && LA13_0 <= 'f')) )
                    {
                        alt13 = 1;
                    }


                    switch (alt13) 
                    {
                        case 1 :
                            // AS3_ex.g3:1470:21: HEX_DIGIT
                            {
                                mHEX_DIGIT(); if (state.failed) return ;

                            }
                            break;

                        default:
                            if ( cnt13 >= 1 ) goto loop13;
                            if ( state.backtracking > 0 ) {state.failed = true; return ;}
                                EarlyExitException eee =
                                    new EarlyExitException(13, input);
                                throw eee;
                    }
                    cnt13++;
                } while (true);

                loop13:
                    ;   // Stops C# compiler whinging that label 'loop13' has no statements


            }

            state.type = _type;
            state.channel = _channel;
            if ( state.backtracking == 0 ) 
            {

                  debugMethod("HEX_NUMBER_LITERAL",Text);

            }    }
        finally 
        {
        }
    }
    // $ANTLR end "HEX_NUMBER_LITERAL"

    // $ANTLR start "DEC_NUMBER"
    public void mDEC_NUMBER() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1472:30: ( ( NUMBER )+ '.' ( NUMBER )* | '.' ( NUMBER )+ | ( NUMBER )+ )
            int alt18 = 3;
            alt18 = dfa18.Predict(input);
            switch (alt18) 
            {
                case 1 :
                    // AS3_ex.g3:1472:33: ( NUMBER )+ '.' ( NUMBER )*
                    {
                        // AS3_ex.g3:1472:33: ( NUMBER )+
                        int cnt14 = 0;
                        do 
                        {
                            int alt14 = 2;
                            int LA14_0 = input.LA(1);

                            if ( ((LA14_0 >= '0' && LA14_0 <= '9')) )
                            {
                                alt14 = 1;
                            }


                            switch (alt14) 
                            {
                                case 1 :
                                    // AS3_ex.g3:1472:33: NUMBER
                                    {
                                        mNUMBER(); if (state.failed) return ;

                                    }
                                    break;

                                default:
                                    if ( cnt14 >= 1 ) goto loop14;
                                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                                        EarlyExitException eee =
                                            new EarlyExitException(14, input);
                                        throw eee;
                            }
                            cnt14++;
                        } while (true);

                        loop14:
                            ;   // Stops C# compiler whinging that label 'loop14' has no statements

                        Match('.'); if (state.failed) return ;
                        // AS3_ex.g3:1472:45: ( NUMBER )*
                        do 
                        {
                            int alt15 = 2;
                            int LA15_0 = input.LA(1);

                            if ( ((LA15_0 >= '0' && LA15_0 <= '9')) )
                            {
                                alt15 = 1;
                            }


                            switch (alt15) 
                            {
                                case 1 :
                                    // AS3_ex.g3:1472:45: NUMBER
                                    {
                                        mNUMBER(); if (state.failed) return ;

                                    }
                                    break;

                                default:
                                    goto loop15;
                            }
                        } while (true);

                        loop15:
                            ;   // Stops C# compiler whining that label 'loop15' has no statements


                    }
                    break;
                case 2 :
                    // AS3_ex.g3:1472:55: '.' ( NUMBER )+
                    {
                        Match('.'); if (state.failed) return ;
                        // AS3_ex.g3:1472:59: ( NUMBER )+
                        int cnt16 = 0;
                        do 
                        {
                            int alt16 = 2;
                            int LA16_0 = input.LA(1);

                            if ( ((LA16_0 >= '0' && LA16_0 <= '9')) )
                            {
                                alt16 = 1;
                            }


                            switch (alt16) 
                            {
                                case 1 :
                                    // AS3_ex.g3:1472:59: NUMBER
                                    {
                                        mNUMBER(); if (state.failed) return ;

                                    }
                                    break;

                                default:
                                    if ( cnt16 >= 1 ) goto loop16;
                                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                                        EarlyExitException eee =
                                            new EarlyExitException(16, input);
                                        throw eee;
                            }
                            cnt16++;
                        } while (true);

                        loop16:
                            ;   // Stops C# compiler whinging that label 'loop16' has no statements


                    }
                    break;
                case 3 :
                    // AS3_ex.g3:1472:69: ( NUMBER )+
                    {
                        // AS3_ex.g3:1472:69: ( NUMBER )+
                        int cnt17 = 0;
                        do 
                        {
                            int alt17 = 2;
                            int LA17_0 = input.LA(1);

                            if ( ((LA17_0 >= '0' && LA17_0 <= '9')) )
                            {
                                alt17 = 1;
                            }


                            switch (alt17) 
                            {
                                case 1 :
                                    // AS3_ex.g3:1472:69: NUMBER
                                    {
                                        mNUMBER(); if (state.failed) return ;

                                    }
                                    break;

                                default:
                                    if ( cnt17 >= 1 ) goto loop17;
                                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                                        EarlyExitException eee =
                                            new EarlyExitException(17, input);
                                        throw eee;
                            }
                            cnt17++;
                        } while (true);

                        loop17:
                            ;   // Stops C# compiler whinging that label 'loop17' has no statements


                    }
                    break;

            }
        }
        finally 
        {
        }
    }
    // $ANTLR end "DEC_NUMBER"

    // $ANTLR start "DEC_NUMBER_LITERAL"
    public void mDEC_NUMBER_LITERAL() // throws RecognitionException [2]
    {
            try
            {
            int _type = DEC_NUMBER_LITERAL;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:1478:5: ( DEC_NUMBER ( EXPONENT )? )
            // AS3_ex.g3:1478:8: DEC_NUMBER ( EXPONENT )?
            {
                mDEC_NUMBER(); if (state.failed) return ;
                // AS3_ex.g3:1478:19: ( EXPONENT )?
                int alt19 = 2;
                int LA19_0 = input.LA(1);

                if ( (LA19_0 == 'E' || LA19_0 == 'e') )
                {
                    alt19 = 1;
                }
                switch (alt19) 
                {
                    case 1 :
                        // AS3_ex.g3:1478:19: EXPONENT
                        {
                            mEXPONENT(); if (state.failed) return ;

                        }
                        break;

                }


            }

            state.type = _type;
            state.channel = _channel;
            if ( state.backtracking == 0 ) 
            {

                  debugMethod("DEC_NUMBER_LITERAL",Text);

            }    }
        finally 
        {
        }
    }
    // $ANTLR end "DEC_NUMBER_LITERAL"

    // $ANTLR start "EXPONENT"
    public void mEXPONENT() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1480:30: ( ( 'e' | 'E' ) ( '+' | '-' )? ( NUMBER )+ )
            // AS3_ex.g3:1480:32: ( 'e' | 'E' ) ( '+' | '-' )? ( NUMBER )+
            {
                if ( input.LA(1) == 'E' || input.LA(1) == 'e' ) 
                {
                    input.Consume();
                state.failed = false;
                }
                else 
                {
                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    MismatchedSetException mse = new MismatchedSetException(null,input);
                    Recover(mse);
                    throw mse;}

                // AS3_ex.g3:1480:42: ( '+' | '-' )?
                int alt20 = 2;
                int LA20_0 = input.LA(1);

                if ( (LA20_0 == '+' || LA20_0 == '-') )
                {
                    alt20 = 1;
                }
                switch (alt20) 
                {
                    case 1 :
                        // AS3_ex.g3:
                        {
                            if ( input.LA(1) == '+' || input.LA(1) == '-' ) 
                            {
                                input.Consume();
                            state.failed = false;
                            }
                            else 
                            {
                                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                                MismatchedSetException mse = new MismatchedSetException(null,input);
                                Recover(mse);
                                throw mse;}


                        }
                        break;

                }

                // AS3_ex.g3:1480:53: ( NUMBER )+
                int cnt21 = 0;
                do 
                {
                    int alt21 = 2;
                    int LA21_0 = input.LA(1);

                    if ( ((LA21_0 >= '0' && LA21_0 <= '9')) )
                    {
                        alt21 = 1;
                    }


                    switch (alt21) 
                    {
                        case 1 :
                            // AS3_ex.g3:1480:53: NUMBER
                            {
                                mNUMBER(); if (state.failed) return ;

                            }
                            break;

                        default:
                            if ( cnt21 >= 1 ) goto loop21;
                            if ( state.backtracking > 0 ) {state.failed = true; return ;}
                                EarlyExitException eee =
                                    new EarlyExitException(21, input);
                                throw eee;
                    }
                    cnt21++;
                } while (true);

                loop21:
                    ;   // Stops C# compiler whinging that label 'loop21' has no statements


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "EXPONENT"

    // $ANTLR start "IDENTIFIER"
    public void mIDENTIFIER() // throws RecognitionException [2]
    {
            try
            {
            int _type = IDENTIFIER;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:1488:5: ( IDENT_NAME_ASCII_START | ( UNICODE_ESCAPE )+ | )
            int alt23 = 3;
            switch ( input.LA(1) ) 
            {
            case '$':
            case 'A':
            case 'B':
            case 'C':
            case 'D':
            case 'E':
            case 'F':
            case 'G':
            case 'H':
            case 'I':
            case 'J':
            case 'K':
            case 'L':
            case 'M':
            case 'N':
            case 'O':
            case 'P':
            case 'Q':
            case 'R':
            case 'S':
            case 'T':
            case 'U':
            case 'V':
            case 'W':
            case 'X':
            case 'Y':
            case 'Z':
            case '_':
            case 'a':
            case 'b':
            case 'c':
            case 'd':
            case 'e':
            case 'f':
            case 'g':
            case 'h':
            case 'i':
            case 'j':
            case 'k':
            case 'l':
            case 'm':
            case 'n':
            case 'o':
            case 'p':
            case 'q':
            case 'r':
            case 's':
            case 't':
            case 'u':
            case 'v':
            case 'w':
            case 'x':
            case 'y':
            case 'z':
                {
                alt23 = 1;
                }
                break;
            case '\\':
                {
                alt23 = 2;
                }
                break;
                default:
                    alt23 = 3;
                    break;}

            switch (alt23) 
            {
                case 1 :
                    // AS3_ex.g3:1488:9: IDENT_NAME_ASCII_START
                    {
                        mIDENT_NAME_ASCII_START(); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // AS3_ex.g3:1489:9: ( UNICODE_ESCAPE )+
                    {
                        // AS3_ex.g3:1489:9: ( UNICODE_ESCAPE )+
                        int cnt22 = 0;
                        do 
                        {
                            int alt22 = 2;
                            int LA22_0 = input.LA(1);

                            if ( (LA22_0 == '\\') )
                            {
                                alt22 = 1;
                            }


                            switch (alt22) 
                            {
                                case 1 :
                                    // AS3_ex.g3:1489:9: UNICODE_ESCAPE
                                    {
                                        mUNICODE_ESCAPE(); if (state.failed) return ;

                                    }
                                    break;

                                default:
                                    if ( cnt22 >= 1 ) goto loop22;
                                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                                        EarlyExitException eee =
                                            new EarlyExitException(22, input);
                                        throw eee;
                            }
                            cnt22++;
                        } while (true);

                        loop22:
                            ;   // Stops C# compiler whinging that label 'loop22' has no statements


                    }
                    break;
                case 3 :
                    // AS3_ex.g3:1490:9: 
                    {
                        if ( state.backtracking == 0 ) 
                        {
                          consumeIdentifierUnicodeStart();
                        }

                    }
                    break;

            }
            state.type = _type;
            state.channel = _channel;
            if ( state.backtracking == 0 ) 
            {

                  debugMethod("Identifier",Text);

            }    }
        finally 
        {
        }
    }
    // $ANTLR end "IDENTIFIER"

    // $ANTLR start "IDENT_NAME_ASCII_START"
    public void mIDENT_NAME_ASCII_START() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1493:35: ( IDENT_ASCII_START ( IDENT_PART )* )
            // AS3_ex.g3:1493:37: IDENT_ASCII_START ( IDENT_PART )*
            {
                mIDENT_ASCII_START(); if (state.failed) return ;
                // AS3_ex.g3:1493:55: ( IDENT_PART )*
                do 
                {
                    int alt24 = 2;
                    int LA24_0 = input.LA(1);

                    if ( (LA24_0 == '$' || (LA24_0 >= '0' && LA24_0 <= '9') || (LA24_0 >= 'A' && LA24_0 <= 'Z') || LA24_0 == '_' || (LA24_0 >= 'a' && LA24_0 <= 'z')) )
                    {
                        alt24 = 1;
                    }
                    else if ( ((isUnicodeIdentifierPart(input.LA(1)))) )
                    {
                        alt24 = 1;
                    }


                    switch (alt24) 
                    {
                        case 1 :
                            // AS3_ex.g3:1493:55: IDENT_PART
                            {
                                mIDENT_PART(); if (state.failed) return ;

                            }
                            break;

                        default:
                            goto loop24;
                    }
                } while (true);

                loop24:
                    ;   // Stops C# compiler whining that label 'loop24' has no statements


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "IDENT_NAME_ASCII_START"

    // $ANTLR start "IDENT_ASCII_START"
    public void mIDENT_ASCII_START() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1495:35: ( ALPHABET | DOLLAR | UNDERSCORE )
            // AS3_ex.g3:
            {
                if ( input.LA(1) == '$' || (input.LA(1) >= 'A' && input.LA(1) <= 'Z') || input.LA(1) == '_' || (input.LA(1) >= 'a' && input.LA(1) <= 'z') ) 
                {
                    input.Consume();
                state.failed = false;
                }
                else 
                {
                    if ( state.backtracking > 0 ) {state.failed = true; return ;}
                    MismatchedSetException mse = new MismatchedSetException(null,input);
                    Recover(mse);
                    throw mse;}


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "IDENT_ASCII_START"

    // $ANTLR start "IDENT_PART"
    public void mIDENT_PART() // throws RecognitionException [2]
    {
            try
            {
            // AS3_ex.g3:1501:5: ( ( IDENT_ASCII_START )=> IDENT_ASCII_START | NUMBER | {...}?)
            int alt25 = 3;
            int LA25_0 = input.LA(1);

            if ( (LA25_0 == '$' || (LA25_0 >= 'A' && LA25_0 <= 'Z') || LA25_0 == '_' || (LA25_0 >= 'a' && LA25_0 <= 'z')) && (synpred1_AS3_ex()) )
            {
                alt25 = 1;
            }
            else if ( ((LA25_0 >= '0' && LA25_0 <= '9')) )
            {
                alt25 = 2;
            }
            else 
            {
                alt25 = 3;}
            switch (alt25) 
            {
                case 1 :
                    // AS3_ex.g3:1501:9: ( IDENT_ASCII_START )=> IDENT_ASCII_START
                    {
                        mIDENT_ASCII_START(); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // AS3_ex.g3:1502:9: NUMBER
                    {
                        mNUMBER(); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // AS3_ex.g3:1503:9: {...}?
                    {
                        if ( !((isUnicodeIdentifierPart(input.LA(1)))) ) 
                        {
                            if ( state.backtracking > 0 ) {state.failed = true; return ;}
                            throw new FailedPredicateException(input, "IDENT_PART", "isUnicodeIdentifierPart(input.LA(1))");
                        }
                        if ( state.backtracking == 0 ) 
                        {
                          /*matchAny();*/
                        }

                    }
                    break;

            }
        }
        finally 
        {
        }
    }
    // $ANTLR end "IDENT_PART"

    // $ANTLR start "XML_COMMENT"
    public void mXML_COMMENT() // throws RecognitionException [2]
    {
            try
            {
            int _type = XML_COMMENT;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:1510:5: ( '<!--' ( options {greedy=false; } : . )* '-->' )
            // AS3_ex.g3:1510:9: '<!--' ( options {greedy=false; } : . )* '-->'
            {
                Match("<!--"); if (state.failed) return ;

                // AS3_ex.g3:1510:16: ( options {greedy=false; } : . )*
                do 
                {
                    int alt26 = 2;
                    int LA26_0 = input.LA(1);

                    if ( (LA26_0 == '-') )
                    {
                        int LA26_1 = input.LA(2);

                        if ( (LA26_1 == '-') )
                        {
                            int LA26_3 = input.LA(3);

                            if ( (LA26_3 == '>') )
                            {
                                alt26 = 2;
                            }
                            else if ( ((LA26_3 >= '\u0000' && LA26_3 <= '=') || (LA26_3 >= '?' && LA26_3 <= '\uFFFF')) )
                            {
                                alt26 = 1;
                            }


                        }
                        else if ( ((LA26_1 >= '\u0000' && LA26_1 <= ',') || (LA26_1 >= '.' && LA26_1 <= '\uFFFF')) )
                        {
                            alt26 = 1;
                        }


                    }
                    else if ( ((LA26_0 >= '\u0000' && LA26_0 <= ',') || (LA26_0 >= '.' && LA26_0 <= '\uFFFF')) )
                    {
                        alt26 = 1;
                    }


                    switch (alt26) 
                    {
                        case 1 :
                            // AS3_ex.g3:1510:44: .
                            {
                                MatchAny(); if (state.failed) return ;

                            }
                            break;

                        default:
                            goto loop26;
                    }
                } while (true);

                loop26:
                    ;   // Stops C# compiler whining that label 'loop26' has no statements

                Match("-->"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
            if ( state.backtracking == 0 ) 
            {

                  debugMethod("XML_COMMENT",Text);

            }    }
        finally 
        {
        }
    }
    // $ANTLR end "XML_COMMENT"

    // $ANTLR start "XML_CDATA"
    public void mXML_CDATA() // throws RecognitionException [2]
    {
            try
            {
            int _type = XML_CDATA;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:1516:5: ( '<![CDATA' ( options {greedy=false; } : . )* ']]>' )
            // AS3_ex.g3:1516:9: '<![CDATA' ( options {greedy=false; } : . )* ']]>'
            {
                Match("<![CDATA"); if (state.failed) return ;

                // AS3_ex.g3:1516:20: ( options {greedy=false; } : . )*
                do 
                {
                    int alt27 = 2;
                    int LA27_0 = input.LA(1);

                    if ( (LA27_0 == ']') )
                    {
                        int LA27_1 = input.LA(2);

                        if ( (LA27_1 == ']') )
                        {
                            int LA27_3 = input.LA(3);

                            if ( (LA27_3 == '>') )
                            {
                                alt27 = 2;
                            }
                            else if ( ((LA27_3 >= '\u0000' && LA27_3 <= '=') || (LA27_3 >= '?' && LA27_3 <= '\uFFFF')) )
                            {
                                alt27 = 1;
                            }


                        }
                        else if ( ((LA27_1 >= '\u0000' && LA27_1 <= '\\') || (LA27_1 >= '^' && LA27_1 <= '\uFFFF')) )
                        {
                            alt27 = 1;
                        }


                    }
                    else if ( ((LA27_0 >= '\u0000' && LA27_0 <= '\\') || (LA27_0 >= '^' && LA27_0 <= '\uFFFF')) )
                    {
                        alt27 = 1;
                    }


                    switch (alt27) 
                    {
                        case 1 :
                            // AS3_ex.g3:1516:48: .
                            {
                                MatchAny(); if (state.failed) return ;

                            }
                            break;

                        default:
                            goto loop27;
                    }
                } while (true);

                loop27:
                    ;   // Stops C# compiler whining that label 'loop27' has no statements

                Match("]]>"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
            if ( state.backtracking == 0 ) 
            {

                  debugMethod("XML_CDATA",Text);

            }    }
        finally 
        {
        }
    }
    // $ANTLR end "XML_CDATA"

    // $ANTLR start "XML_PI"
    public void mXML_PI() // throws RecognitionException [2]
    {
            try
            {
            int _type = XML_PI;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:1522:5: ( '<?' ( options {greedy=false; } : . )* '?>' )
            // AS3_ex.g3:1522:9: '<?' ( options {greedy=false; } : . )* '?>'
            {
                Match("<?"); if (state.failed) return ;

                // AS3_ex.g3:1522:14: ( options {greedy=false; } : . )*
                do 
                {
                    int alt28 = 2;
                    int LA28_0 = input.LA(1);

                    if ( (LA28_0 == '?') )
                    {
                        int LA28_1 = input.LA(2);

                        if ( (LA28_1 == '>') )
                        {
                            alt28 = 2;
                        }
                        else if ( ((LA28_1 >= '\u0000' && LA28_1 <= '=') || (LA28_1 >= '?' && LA28_1 <= '\uFFFF')) )
                        {
                            alt28 = 1;
                        }


                    }
                    else if ( ((LA28_0 >= '\u0000' && LA28_0 <= '>') || (LA28_0 >= '@' && LA28_0 <= '\uFFFF')) )
                    {
                        alt28 = 1;
                    }


                    switch (alt28) 
                    {
                        case 1 :
                            // AS3_ex.g3:1522:42: .
                            {
                                MatchAny(); if (state.failed) return ;

                            }
                            break;

                        default:
                            goto loop28;
                    }
                } while (true);

                loop28:
                    ;   // Stops C# compiler whining that label 'loop28' has no statements

                Match("?>"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
            if ( state.backtracking == 0 ) 
            {

                  debugMethod("XML_PI",Text);

            }    }
        finally 
        {
        }
    }
    // $ANTLR end "XML_PI"

    // $ANTLR start "XML_TEXT"
    public void mXML_TEXT() // throws RecognitionException [2]
    {
            try
            {
            int _type = XML_TEXT;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // AS3_ex.g3:1529:5: ( '\\u0020' .. '\\u003b' | '\\u003d' .. '\\u007a' | '\\u007c' .. '\\u007e' | {...}?)
            int alt29 = 4;
            switch ( input.LA(1) ) 
            {
            case ' ':
            case '!':
            case '\"':
            case '#':
            case '$':
            case '%':
            case '&':
            case '\'':
            case '(':
            case ')':
            case '*':
            case '+':
            case ',':
            case '-':
            case '.':
            case '/':
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
            case ':':
            case ';':
                {
                alt29 = 1;
                }
                break;
            case '=':
            case '>':
            case '?':
            case '@':
            case 'A':
            case 'B':
            case 'C':
            case 'D':
            case 'E':
            case 'F':
            case 'G':
            case 'H':
            case 'I':
            case 'J':
            case 'K':
            case 'L':
            case 'M':
            case 'N':
            case 'O':
            case 'P':
            case 'Q':
            case 'R':
            case 'S':
            case 'T':
            case 'U':
            case 'V':
            case 'W':
            case 'X':
            case 'Y':
            case 'Z':
            case '[':
            case '\\':
            case ']':
            case '^':
            case '_':
            case '`':
            case 'a':
            case 'b':
            case 'c':
            case 'd':
            case 'e':
            case 'f':
            case 'g':
            case 'h':
            case 'i':
            case 'j':
            case 'k':
            case 'l':
            case 'm':
            case 'n':
            case 'o':
            case 'p':
            case 'q':
            case 'r':
            case 's':
            case 't':
            case 'u':
            case 'v':
            case 'w':
            case 'x':
            case 'y':
            case 'z':
                {
                alt29 = 2;
                }
                break;
            case '|':
            case '}':
            case '~':
                {
                alt29 = 3;
                }
                break;
                default:
                    alt29 = 4;
                    break;}

            switch (alt29) 
            {
                case 1 :
                    // AS3_ex.g3:1529:7: '\\u0020' .. '\\u003b'
                    {
                        MatchRange(' ',';'); if (state.failed) return ;

                    }
                    break;
                case 2 :
                    // AS3_ex.g3:1530:7: '\\u003d' .. '\\u007a'
                    {
                        MatchRange('=','z'); if (state.failed) return ;

                    }
                    break;
                case 3 :
                    // AS3_ex.g3:1531:7: '\\u007c' .. '\\u007e'
                    {
                        MatchRange('|','~'); if (state.failed) return ;

                    }
                    break;
                case 4 :
                    // AS3_ex.g3:1532:7: {...}?
                    {
                        if ( !((isXMLText(input.LA(1)))) ) 
                        {
                            if ( state.backtracking > 0 ) {state.failed = true; return ;}
                            throw new FailedPredicateException(input, "XML_TEXT", "isXMLText(input.LA(1))");
                        }
                        if ( state.backtracking == 0 ) 
                        {
                          /*matchAny();*/
                        }

                    }
                    break;

            }
            state.type = _type;
            state.channel = _channel;
            if ( state.backtracking == 0 ) 
            {

                  debugMethod("XMLText",Text);

            }    }
        finally 
        {
        }
    }
    // $ANTLR end "XML_TEXT"

    override public void mTokens() // throws RecognitionException 
    {
        // AS3_ex.g3:1:8: ( AS | BREAK | CASE | CATCH | CLASS | CONST | CONTINUE | DEFAULT | DELETE | DO | ELSE | EXTENDS | FALSE | FINALLY | FOR | FUNCTION | IF | IMPLEMENTS | IMPORT | IN | INSTANCEOF | INTERFACE | INTERNAL | IS | NATIVE | NEW | NULL | PACKAGE | PRIVATE | PROTECTED | PUBLIC | RETURN | SUPER | SWITCH | THIS | THROW | TO | TRUE | TRY | TYPEOF | USE | VAR | VOID | WHILE | WITH | EACH | GET | SET | NAMESPACE | INCLUDE | DYNAMIC | FINAL | OVERRIDE | STATIC | SEMI | LCURLY | RCURLY | LPAREN | RPAREN | LBRACK | RBRACK | DOT | COMMA | LT | GT | LTE | EQ | NEQ | SAME | NSAME | PLUS | SUB | STAR | DIV | MOD | INC | DEC | SHL | AND | OR | XOR | NOT | INV | LAND | LOR | QUE | COLON | ASSIGN | DIV_ASSIGN | MOD_ASSIGN | ADD_ASSIGN | SUB_ASSIGN | SHL_ASSIGN | LAND_ASSIGN | LOR_ASSIGN | AND_ASSIGN | XOR_ASSIGN | OR_ASSIGN | ELLIPSIS | XML_ELLIPSIS | XML_TEND | XML_E_TEND | XML_NS_OP | XML_AT | XML_LS_STD | XML_LS_END | EOL | WHITESPACE | COMMENT_MULTILINE | COMMENT_SINGLELINE | SINGLE_QUOTE_LITERAL | DOUBLE_QUOTE_LITERAL | REGULAR_EXPR_LITERAL | HEX_NUMBER_LITERAL | DEC_NUMBER_LITERAL | IDENTIFIER | XML_COMMENT | XML_CDATA | XML_PI | XML_TEXT )
        int alt30 = 120;
        alt30 = dfa30.Predict(input);
        switch (alt30) 
        {
            case 1 :
                // AS3_ex.g3:1:10: AS
                {
                    mAS(); if (state.failed) return ;

                }
                break;
            case 2 :
                // AS3_ex.g3:1:13: BREAK
                {
                    mBREAK(); if (state.failed) return ;

                }
                break;
            case 3 :
                // AS3_ex.g3:1:19: CASE
                {
                    mCASE(); if (state.failed) return ;

                }
                break;
            case 4 :
                // AS3_ex.g3:1:24: CATCH
                {
                    mCATCH(); if (state.failed) return ;

                }
                break;
            case 5 :
                // AS3_ex.g3:1:30: CLASS
                {
                    mCLASS(); if (state.failed) return ;

                }
                break;
            case 6 :
                // AS3_ex.g3:1:36: CONST
                {
                    mCONST(); if (state.failed) return ;

                }
                break;
            case 7 :
                // AS3_ex.g3:1:42: CONTINUE
                {
                    mCONTINUE(); if (state.failed) return ;

                }
                break;
            case 8 :
                // AS3_ex.g3:1:51: DEFAULT
                {
                    mDEFAULT(); if (state.failed) return ;

                }
                break;
            case 9 :
                // AS3_ex.g3:1:59: DELETE
                {
                    mDELETE(); if (state.failed) return ;

                }
                break;
            case 10 :
                // AS3_ex.g3:1:66: DO
                {
                    mDO(); if (state.failed) return ;

                }
                break;
            case 11 :
                // AS3_ex.g3:1:69: ELSE
                {
                    mELSE(); if (state.failed) return ;

                }
                break;
            case 12 :
                // AS3_ex.g3:1:74: EXTENDS
                {
                    mEXTENDS(); if (state.failed) return ;

                }
                break;
            case 13 :
                // AS3_ex.g3:1:82: FALSE
                {
                    mFALSE(); if (state.failed) return ;

                }
                break;
            case 14 :
                // AS3_ex.g3:1:88: FINALLY
                {
                    mFINALLY(); if (state.failed) return ;

                }
                break;
            case 15 :
                // AS3_ex.g3:1:96: FOR
                {
                    mFOR(); if (state.failed) return ;

                }
                break;
            case 16 :
                // AS3_ex.g3:1:100: FUNCTION
                {
                    mFUNCTION(); if (state.failed) return ;

                }
                break;
            case 17 :
                // AS3_ex.g3:1:109: IF
                {
                    mIF(); if (state.failed) return ;

                }
                break;
            case 18 :
                // AS3_ex.g3:1:112: IMPLEMENTS
                {
                    mIMPLEMENTS(); if (state.failed) return ;

                }
                break;
            case 19 :
                // AS3_ex.g3:1:123: IMPORT
                {
                    mIMPORT(); if (state.failed) return ;

                }
                break;
            case 20 :
                // AS3_ex.g3:1:130: IN
                {
                    mIN(); if (state.failed) return ;

                }
                break;
            case 21 :
                // AS3_ex.g3:1:133: INSTANCEOF
                {
                    mINSTANCEOF(); if (state.failed) return ;

                }
                break;
            case 22 :
                // AS3_ex.g3:1:144: INTERFACE
                {
                    mINTERFACE(); if (state.failed) return ;

                }
                break;
            case 23 :
                // AS3_ex.g3:1:154: INTERNAL
                {
                    mINTERNAL(); if (state.failed) return ;

                }
                break;
            case 24 :
                // AS3_ex.g3:1:163: IS
                {
                    mIS(); if (state.failed) return ;

                }
                break;
            case 25 :
                // AS3_ex.g3:1:166: NATIVE
                {
                    mNATIVE(); if (state.failed) return ;

                }
                break;
            case 26 :
                // AS3_ex.g3:1:173: NEW
                {
                    mNEW(); if (state.failed) return ;

                }
                break;
            case 27 :
                // AS3_ex.g3:1:177: NULL
                {
                    mNULL(); if (state.failed) return ;

                }
                break;
            case 28 :
                // AS3_ex.g3:1:182: PACKAGE
                {
                    mPACKAGE(); if (state.failed) return ;

                }
                break;
            case 29 :
                // AS3_ex.g3:1:190: PRIVATE
                {
                    mPRIVATE(); if (state.failed) return ;

                }
                break;
            case 30 :
                // AS3_ex.g3:1:198: PROTECTED
                {
                    mPROTECTED(); if (state.failed) return ;

                }
                break;
            case 31 :
                // AS3_ex.g3:1:208: PUBLIC
                {
                    mPUBLIC(); if (state.failed) return ;

                }
                break;
            case 32 :
                // AS3_ex.g3:1:215: RETURN
                {
                    mRETURN(); if (state.failed) return ;

                }
                break;
            case 33 :
                // AS3_ex.g3:1:222: SUPER
                {
                    mSUPER(); if (state.failed) return ;

                }
                break;
            case 34 :
                // AS3_ex.g3:1:228: SWITCH
                {
                    mSWITCH(); if (state.failed) return ;

                }
                break;
            case 35 :
                // AS3_ex.g3:1:235: THIS
                {
                    mTHIS(); if (state.failed) return ;

                }
                break;
            case 36 :
                // AS3_ex.g3:1:240: THROW
                {
                    mTHROW(); if (state.failed) return ;

                }
                break;
            case 37 :
                // AS3_ex.g3:1:246: TO
                {
                    mTO(); if (state.failed) return ;

                }
                break;
            case 38 :
                // AS3_ex.g3:1:249: TRUE
                {
                    mTRUE(); if (state.failed) return ;

                }
                break;
            case 39 :
                // AS3_ex.g3:1:254: TRY
                {
                    mTRY(); if (state.failed) return ;

                }
                break;
            case 40 :
                // AS3_ex.g3:1:258: TYPEOF
                {
                    mTYPEOF(); if (state.failed) return ;

                }
                break;
            case 41 :
                // AS3_ex.g3:1:265: USE
                {
                    mUSE(); if (state.failed) return ;

                }
                break;
            case 42 :
                // AS3_ex.g3:1:269: VAR
                {
                    mVAR(); if (state.failed) return ;

                }
                break;
            case 43 :
                // AS3_ex.g3:1:273: VOID
                {
                    mVOID(); if (state.failed) return ;

                }
                break;
            case 44 :
                // AS3_ex.g3:1:278: WHILE
                {
                    mWHILE(); if (state.failed) return ;

                }
                break;
            case 45 :
                // AS3_ex.g3:1:284: WITH
                {
                    mWITH(); if (state.failed) return ;

                }
                break;
            case 46 :
                // AS3_ex.g3:1:289: EACH
                {
                    mEACH(); if (state.failed) return ;

                }
                break;
            case 47 :
                // AS3_ex.g3:1:294: GET
                {
                    mGET(); if (state.failed) return ;

                }
                break;
            case 48 :
                // AS3_ex.g3:1:298: SET
                {
                    mSET(); if (state.failed) return ;

                }
                break;
            case 49 :
                // AS3_ex.g3:1:302: NAMESPACE
                {
                    mNAMESPACE(); if (state.failed) return ;

                }
                break;
            case 50 :
                // AS3_ex.g3:1:312: INCLUDE
                {
                    mINCLUDE(); if (state.failed) return ;

                }
                break;
            case 51 :
                // AS3_ex.g3:1:320: DYNAMIC
                {
                    mDYNAMIC(); if (state.failed) return ;

                }
                break;
            case 52 :
                // AS3_ex.g3:1:328: FINAL
                {
                    mFINAL(); if (state.failed) return ;

                }
                break;
            case 53 :
                // AS3_ex.g3:1:334: OVERRIDE
                {
                    mOVERRIDE(); if (state.failed) return ;

                }
                break;
            case 54 :
                // AS3_ex.g3:1:343: STATIC
                {
                    mSTATIC(); if (state.failed) return ;

                }
                break;
            case 55 :
                // AS3_ex.g3:1:350: SEMI
                {
                    mSEMI(); if (state.failed) return ;

                }
                break;
            case 56 :
                // AS3_ex.g3:1:355: LCURLY
                {
                    mLCURLY(); if (state.failed) return ;

                }
                break;
            case 57 :
                // AS3_ex.g3:1:362: RCURLY
                {
                    mRCURLY(); if (state.failed) return ;

                }
                break;
            case 58 :
                // AS3_ex.g3:1:369: LPAREN
                {
                    mLPAREN(); if (state.failed) return ;

                }
                break;
            case 59 :
                // AS3_ex.g3:1:376: RPAREN
                {
                    mRPAREN(); if (state.failed) return ;

                }
                break;
            case 60 :
                // AS3_ex.g3:1:383: LBRACK
                {
                    mLBRACK(); if (state.failed) return ;

                }
                break;
            case 61 :
                // AS3_ex.g3:1:390: RBRACK
                {
                    mRBRACK(); if (state.failed) return ;

                }
                break;
            case 62 :
                // AS3_ex.g3:1:397: DOT
                {
                    mDOT(); if (state.failed) return ;

                }
                break;
            case 63 :
                // AS3_ex.g3:1:401: COMMA
                {
                    mCOMMA(); if (state.failed) return ;

                }
                break;
            case 64 :
                // AS3_ex.g3:1:407: LT
                {
                    mLT(); if (state.failed) return ;

                }
                break;
            case 65 :
                // AS3_ex.g3:1:410: GT
                {
                    mGT(); if (state.failed) return ;

                }
                break;
            case 66 :
                // AS3_ex.g3:1:413: LTE
                {
                    mLTE(); if (state.failed) return ;

                }
                break;
            case 67 :
                // AS3_ex.g3:1:417: EQ
                {
                    mEQ(); if (state.failed) return ;

                }
                break;
            case 68 :
                // AS3_ex.g3:1:420: NEQ
                {
                    mNEQ(); if (state.failed) return ;

                }
                break;
            case 69 :
                // AS3_ex.g3:1:424: SAME
                {
                    mSAME(); if (state.failed) return ;

                }
                break;
            case 70 :
                // AS3_ex.g3:1:429: NSAME
                {
                    mNSAME(); if (state.failed) return ;

                }
                break;
            case 71 :
                // AS3_ex.g3:1:435: PLUS
                {
                    mPLUS(); if (state.failed) return ;

                }
                break;
            case 72 :
                // AS3_ex.g3:1:440: SUB
                {
                    mSUB(); if (state.failed) return ;

                }
                break;
            case 73 :
                // AS3_ex.g3:1:444: STAR
                {
                    mSTAR(); if (state.failed) return ;

                }
                break;
            case 74 :
                // AS3_ex.g3:1:449: DIV
                {
                    mDIV(); if (state.failed) return ;

                }
                break;
            case 75 :
                // AS3_ex.g3:1:453: MOD
                {
                    mMOD(); if (state.failed) return ;

                }
                break;
            case 76 :
                // AS3_ex.g3:1:457: INC
                {
                    mINC(); if (state.failed) return ;

                }
                break;
            case 77 :
                // AS3_ex.g3:1:461: DEC
                {
                    mDEC(); if (state.failed) return ;

                }
                break;
            case 78 :
                // AS3_ex.g3:1:465: SHL
                {
                    mSHL(); if (state.failed) return ;

                }
                break;
            case 79 :
                // AS3_ex.g3:1:469: AND
                {
                    mAND(); if (state.failed) return ;

                }
                break;
            case 80 :
                // AS3_ex.g3:1:473: OR
                {
                    mOR(); if (state.failed) return ;

                }
                break;
            case 81 :
                // AS3_ex.g3:1:476: XOR
                {
                    mXOR(); if (state.failed) return ;

                }
                break;
            case 82 :
                // AS3_ex.g3:1:480: NOT
                {
                    mNOT(); if (state.failed) return ;

                }
                break;
            case 83 :
                // AS3_ex.g3:1:484: INV
                {
                    mINV(); if (state.failed) return ;

                }
                break;
            case 84 :
                // AS3_ex.g3:1:488: LAND
                {
                    mLAND(); if (state.failed) return ;

                }
                break;
            case 85 :
                // AS3_ex.g3:1:493: LOR
                {
                    mLOR(); if (state.failed) return ;

                }
                break;
            case 86 :
                // AS3_ex.g3:1:497: QUE
                {
                    mQUE(); if (state.failed) return ;

                }
                break;
            case 87 :
                // AS3_ex.g3:1:501: COLON
                {
                    mCOLON(); if (state.failed) return ;

                }
                break;
            case 88 :
                // AS3_ex.g3:1:507: ASSIGN
                {
                    mASSIGN(); if (state.failed) return ;

                }
                break;
            case 89 :
                // AS3_ex.g3:1:514: DIV_ASSIGN
                {
                    mDIV_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 90 :
                // AS3_ex.g3:1:525: MOD_ASSIGN
                {
                    mMOD_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 91 :
                // AS3_ex.g3:1:536: ADD_ASSIGN
                {
                    mADD_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 92 :
                // AS3_ex.g3:1:547: SUB_ASSIGN
                {
                    mSUB_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 93 :
                // AS3_ex.g3:1:558: SHL_ASSIGN
                {
                    mSHL_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 94 :
                // AS3_ex.g3:1:569: LAND_ASSIGN
                {
                    mLAND_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 95 :
                // AS3_ex.g3:1:581: LOR_ASSIGN
                {
                    mLOR_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 96 :
                // AS3_ex.g3:1:592: AND_ASSIGN
                {
                    mAND_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 97 :
                // AS3_ex.g3:1:603: XOR_ASSIGN
                {
                    mXOR_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 98 :
                // AS3_ex.g3:1:614: OR_ASSIGN
                {
                    mOR_ASSIGN(); if (state.failed) return ;

                }
                break;
            case 99 :
                // AS3_ex.g3:1:624: ELLIPSIS
                {
                    mELLIPSIS(); if (state.failed) return ;

                }
                break;
            case 100 :
                // AS3_ex.g3:1:633: XML_ELLIPSIS
                {
                    mXML_ELLIPSIS(); if (state.failed) return ;

                }
                break;
            case 101 :
                // AS3_ex.g3:1:646: XML_TEND
                {
                    mXML_TEND(); if (state.failed) return ;

                }
                break;
            case 102 :
                // AS3_ex.g3:1:655: XML_E_TEND
                {
                    mXML_E_TEND(); if (state.failed) return ;

                }
                break;
            case 103 :
                // AS3_ex.g3:1:666: XML_NS_OP
                {
                    mXML_NS_OP(); if (state.failed) return ;

                }
                break;
            case 104 :
                // AS3_ex.g3:1:676: XML_AT
                {
                    mXML_AT(); if (state.failed) return ;

                }
                break;
            case 105 :
                // AS3_ex.g3:1:683: XML_LS_STD
                {
                    mXML_LS_STD(); if (state.failed) return ;

                }
                break;
            case 106 :
                // AS3_ex.g3:1:694: XML_LS_END
                {
                    mXML_LS_END(); if (state.failed) return ;

                }
                break;
            case 107 :
                // AS3_ex.g3:1:705: EOL
                {
                    mEOL(); if (state.failed) return ;

                }
                break;
            case 108 :
                // AS3_ex.g3:1:709: WHITESPACE
                {
                    mWHITESPACE(); if (state.failed) return ;

                }
                break;
            case 109 :
                // AS3_ex.g3:1:720: COMMENT_MULTILINE
                {
                    mCOMMENT_MULTILINE(); if (state.failed) return ;

                }
                break;
            case 110 :
                // AS3_ex.g3:1:738: COMMENT_SINGLELINE
                {
                    mCOMMENT_SINGLELINE(); if (state.failed) return ;

                }
                break;
            case 111 :
                // AS3_ex.g3:1:757: SINGLE_QUOTE_LITERAL
                {
                    mSINGLE_QUOTE_LITERAL(); if (state.failed) return ;

                }
                break;
            case 112 :
                // AS3_ex.g3:1:778: DOUBLE_QUOTE_LITERAL
                {
                    mDOUBLE_QUOTE_LITERAL(); if (state.failed) return ;

                }
                break;
            case 113 :
                // AS3_ex.g3:1:799: REGULAR_EXPR_LITERAL
                {
                    mREGULAR_EXPR_LITERAL(); if (state.failed) return ;

                }
                break;
            case 114 :
                // AS3_ex.g3:1:820: HEX_NUMBER_LITERAL
                {
                    mHEX_NUMBER_LITERAL(); if (state.failed) return ;

                }
                break;
            case 115 :
                // AS3_ex.g3:1:839: DEC_NUMBER_LITERAL
                {
                    mDEC_NUMBER_LITERAL(); if (state.failed) return ;

                }
                break;
            case 116 :
                // AS3_ex.g3:1:858: IDENTIFIER
                {
                    mIDENTIFIER(); if (state.failed) return ;

                }
                break;
            case 117 :
                // AS3_ex.g3:1:869: XML_COMMENT
                {
                    mXML_COMMENT(); if (state.failed) return ;

                }
                break;
            case 118 :
                // AS3_ex.g3:1:881: XML_CDATA
                {
                    mXML_CDATA(); if (state.failed) return ;

                }
                break;
            case 119 :
                // AS3_ex.g3:1:891: XML_PI
                {
                    mXML_PI(); if (state.failed) return ;

                }
                break;
            case 120 :
                // AS3_ex.g3:1:898: XML_TEXT
                {
                    mXML_TEXT(); if (state.failed) return ;

                }
                break;

        }

    }

    // $ANTLR start "synpred1_AS3_ex"
    public void synpred1_AS3_ex_fragment() {
        // AS3_ex.g3:1501:9: ( IDENT_ASCII_START )
        // AS3_ex.g3:1501:10: IDENT_ASCII_START
        {
            mIDENT_ASCII_START(); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred1_AS3_ex"

    public bool synpred1_AS3_ex() 
    {
        state.backtracking++;
        int start = input.Mark();
        try 
        {
            synpred1_AS3_ex_fragment(); // can never throw exception
        }
        catch (RecognitionException re) 
        {
            Console.Error.WriteLine("impossible: "+re);
        }
        bool success = !state.failed;
        input.Rewind(start);
        state.backtracking--;
        state.failed = false;
        return success;
    }


    protected DFA18 dfa18;
    protected DFA30 dfa30;
    private void InitializeCyclicDFAs()
    {
        this.dfa18 = new DFA18(this);
        this.dfa30 = new DFA30(this);

        this.dfa30.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA30_SpecialStateTransition);
    }

    const string DFA18_eotS =
        "\x01\uffff\x01\x03\x03\uffff";
    const string DFA18_eofS =
        "\x05\uffff";
    const string DFA18_minS =
        "\x02\x2e\x03\uffff";
    const string DFA18_maxS =
        "\x02\x39\x03\uffff";
    const string DFA18_acceptS =
        "\x02\uffff\x01\x02\x01\x03\x01\x01";
    const string DFA18_specialS =
        "\x05\uffff}>";
    static readonly string[] DFA18_transitionS = {
            "\x01\x02\x01\uffff\x0a\x01",
            "\x01\x04\x01\uffff\x0a\x01",
            "",
            "",
            ""
    };

    static readonly short[] DFA18_eot = DFA.UnpackEncodedString(DFA18_eotS);
    static readonly short[] DFA18_eof = DFA.UnpackEncodedString(DFA18_eofS);
    static readonly char[] DFA18_min = DFA.UnpackEncodedStringToUnsignedChars(DFA18_minS);
    static readonly char[] DFA18_max = DFA.UnpackEncodedStringToUnsignedChars(DFA18_maxS);
    static readonly short[] DFA18_accept = DFA.UnpackEncodedString(DFA18_acceptS);
    static readonly short[] DFA18_special = DFA.UnpackEncodedString(DFA18_specialS);
    static readonly short[][] DFA18_transition = DFA.UnpackEncodedStringArray(DFA18_transitionS);

    protected class DFA18 : DFA
    {
        public DFA18(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 18;
            this.eot = DFA18_eot;
            this.eof = DFA18_eof;
            this.min = DFA18_min;
            this.max = DFA18_max;
            this.accept = DFA18_accept;
            this.special = DFA18_special;
            this.transition = DFA18_transition;

        }

        override public string Description
        {
            get { return "1472:10: fragment DEC_NUMBER : ( ( NUMBER )+ '.' ( NUMBER )* | '.' ( NUMBER )+ | ( NUMBER )+ );"; }
        }

    }

    const string DFA30_eotS =
        "\x01\x34\x11\x38\x07\uffff\x01\x68\x01\uffff\x01\x71\x01\uffff"+
        "\x01\x74\x01\x76\x01\x79\x01\x7c\x01\uffff\x01\u0082\x01\u0085\x01"+
        "\u0088\x01\u008b\x01\u008d\x02\uffff\x01\u0091\x04\uffff\x02\x36"+
        "\x01\x69\x02\uffff\x01\x36\x03\uffff\x01\u0096\x01\uffff\x05\x38"+
        "\x01\u009e\x08\x38\x01\u00a7\x01\x38\x01\u00ac\x01\u00ad\x0c\x38"+
        "\x01\u00bd\x09\x38\x06\uffff\x01\u00c9\x04\uffff\x01\u00cb\x01\u00cd"+
        "\x05\uffff\x01\u00d1\x01\uffff\x01\u00d3\x08\uffff\x01\u00d4\x01"+
        "\u00d5\x06\uffff\x01\u00d7\x02\uffff\x01\u00d9\x0d\uffff\x07\x38"+
        "\x01\uffff\x06\x38\x01\u00e8\x01\x38\x01\uffff\x04\x38\x02\uffff"+
        "\x02\x38\x01\u00f1\x08\x38\x01\u00fa\x03\x38\x01\uffff\x01\x38\x01"+
        "\u00ff\x01\x38\x01\u0101\x01\u0102\x03\x38\x01\u0106\x01\x38\x12"+
        "\uffff\x01\x38\x01\u0109\x07\x38\x01\u0111\x01\x38\x01\u0113\x02"+
        "\x38\x01\uffff\x08\x38\x01\uffff\x01\u011e\x07\x38\x01\uffff\x01"+
        "\x38\x01\u0127\x01\x38\x01\u0129\x01\uffff\x01\x38\x02\uffff\x01"+
        "\u012b\x01\x38\x01\u012d\x01\uffff\x01\x38\x01\u012f\x01\uffff\x01"+
        "\u0130\x01\u0131\x01\u0132\x04\x38\x01\uffff\x01\x38\x01\uffff\x01"+
        "\u0138\x01\u013a\x08\x38\x01\uffff\x05\x38\x01\u0149\x02\x38\x01"+
        "\uffff\x01\u014c\x01\uffff\x01\x38\x01\uffff\x01\u014e\x01\uffff"+
        "\x01\x38\x04\uffff\x02\x38\x01\u0152\x02\x38\x01\uffff\x01\x38\x01"+
        "\uffff\x02\x38\x01\u0158\x04\x38\x01\u015d\x04\x38\x01\u0162\x01"+
        "\u0163\x01\uffff\x01\u0164\x01\u0165\x01\uffff\x01\u0166\x01\uffff"+
        "\x02\x38\x01\u0169\x01\uffff\x01\u016a\x01\u016b\x01\u016c\x02\x38"+
        "\x01\uffff\x03\x38\x01\u0172\x01\uffff\x01\x38\x01\u0174\x01\u0175"+
        "\x01\x38\x05\uffff\x01\x38\x01\u0178\x04\uffff\x01\u0179\x03\x38"+
        "\x01\u017d\x01\uffff\x01\x38\x02\uffff\x01\x38\x01\u0180\x02\uffff"+
        "\x02\x38\x01\u0183\x01\uffff\x01\u0184\x01\u0185\x01\uffff\x01\u0186"+
        "\x01\u0187\x05\uffff";
    const string DFA30_eofS =
        "\u0188\uffff";
    const string DFA30_minS =
        "\x01\x09\x01\x73\x01\x72\x01\x61\x01\x65\x02\x61\x01\x66\x02\x61"+
        "\x02\x65\x01\x68\x01\x73\x01\x61\x01\x68\x01\x65\x01\x76\x07\uffff"+
        "\x01\x2e\x01\uffff\x01\x21\x01\uffff\x02\x3d\x01\x2b\x01\x2d\x01"+
        "\uffff\x01\x00\x01\x3d\x01\x26\x02\x3d\x02\uffff\x01\x3a\x04\uffff"+
        "\x02\x00\x01\x58\x02\uffff\x01\x75\x01\x00\x02\uffff\x01\x24\x01"+
        "\uffff\x01\x65\x01\x73\x01\x61\x01\x6e\x01\x66\x01\x24\x01\x6e\x01"+
        "\x73\x01\x74\x01\x63\x01\x6c\x01\x6e\x01\x72\x01\x6e\x01\x24\x01"+
        "\x70\x02\x24\x01\x6d\x01\x77\x01\x6c\x01\x63\x01\x69\x01\x62\x01"+
        "\x74\x01\x70\x01\x69\x01\x74\x01\x61\x01\x69\x01\x24\x01\x75\x01"+
        "\x70\x01\x65\x01\x72\x02\x69\x02\x74\x01\x65\x06\uffff\x01\x2e\x04"+
        "\uffff\x01\x3d\x01\x3e\x01\uffff\x01\x2d\x03\uffff\x01\x3d\x01\uffff"+
        "\x01\x3d\x08\uffff\x02\x00\x06\uffff\x01\x3d\x02\uffff\x01\x3d\x0d"+
        "\uffff\x01\x61\x01\x65\x01\x63\x02\x73\x01\x61\x01\x65\x01\uffff"+
        "\x01\x61\x02\x65\x01\x68\x01\x73\x01\x61\x01\x24\x01\x63\x01\uffff"+
        "\x01\x6c\x01\x74\x01\x65\x01\x6c\x02\uffff\x01\x69\x01\x65\x01\x24"+
        "\x01\x6c\x01\x6b\x01\x76\x01\x74\x01\x6c\x01\x75\x01\x65\x01\x74"+
        "\x01\x24\x01\x74\x01\x73\x01\x6f\x01\uffff\x01\x65\x01\x24\x01\x65"+
        "\x02\x24\x01\x64\x01\x6c\x01\x68\x01\x24\x01\x72\x12\uffff\x01\x6b"+
        "\x01\x24\x01\x68\x01\x73\x01\x74\x01\x69\x01\x75\x01\x74\x01\x6d"+
        "\x01\x24\x01\x6e\x01\x24\x01\x65\x01\x6c\x01\uffff\x01\x74\x01\x65"+
        "\x01\x72\x01\x61\x01\x72\x01\x75\x01\x76\x01\x73\x01\uffff\x01\x24"+
        "\x02\x61\x01\x65\x01\x69\x02\x72\x01\x63\x01\uffff\x01\x69\x01\x24"+
        "\x01\x77\x01\x24\x01\uffff\x01\x6f\x02\uffff\x01\x24\x01\x65\x01"+
        "\x24\x01\uffff\x01\x72\x01\x24\x01\uffff\x03\x24\x01\x6e\x01\x6c"+
        "\x01\x65\x01\x69\x01\uffff\x01\x64\x01\uffff\x02\x24\x01\x69\x01"+
        "\x6d\x01\x74\x01\x6e\x01\x66\x01\x64\x01\x65\x01\x70\x01\uffff\x01"+
        "\x67\x01\x74\x02\x63\x01\x6e\x01\x24\x01\x68\x01\x63\x01\uffff\x01"+
        "\x24\x01\uffff\x01\x66\x01\uffff\x01\x24\x01\uffff\x01\x69\x04\uffff"+
        "\x01\x75\x01\x74\x01\x24\x01\x63\x01\x73\x01\uffff\x01\x79\x01\uffff"+
        "\x01\x6f\x01\x65\x01\x24\x01\x63\x02\x61\x01\x65\x01\x24\x01\x61"+
        "\x02\x65\x01\x74\x02\x24\x01\uffff\x02\x24\x01\uffff\x01\x24\x01"+
        "\uffff\x01\x64\x01\x65\x01\x24\x01\uffff\x03\x24\x02\x6e\x01\uffff"+
        "\x01\x65\x01\x63\x01\x6c\x01\x24\x01\uffff\x01\x63\x02\x24\x01\x65"+
        "\x05\uffff\x01\x65\x01\x24\x04\uffff\x01\x24\x01\x74\x01\x6f\x01"+
        "\x65\x01\x24\x01\uffff\x01\x65\x02\uffff\x01\x64\x01\x24\x02\uffff"+
        "\x01\x73\x01\x66\x01\x24\x01\uffff\x02\x24\x01\uffff\x02\x24\x05"+
        "\uffff";
    const string DFA30_maxS =
        "\x01\u00a0\x01\x73\x01\x72\x01\x6f\x01\x79\x01\x78\x01\x75\x01"+
        "\x73\x02\x75\x01\x65\x01\x77\x01\x79\x01\x73\x01\x6f\x01\x69\x01"+
        "\x65\x01\x76\x07\uffff\x01\x39\x01\uffff\x01\x3f\x01\uffff\x04\x3d"+
        "\x01\uffff\x01\uffff\x02\x3d\x01\x7c\x01\x3d\x02\uffff\x01\x3a\x04"+
        "\uffff\x02\uffff\x01\x78\x02\uffff\x01\x75\x01\x00\x02\uffff\x01"+
        "\x7a\x01\uffff\x01\x65\x01\x74\x01\x61\x01\x6e\x01\x6c\x01\x7a\x01"+
        "\x6e\x01\x73\x01\x74\x01\x63\x01\x6c\x01\x6e\x01\x72\x01\x6e\x01"+
        "\x7a\x01\x70\x02\x7a\x01\x74\x01\x77\x01\x6c\x01\x63\x01\x6f\x01"+
        "\x62\x01\x74\x01\x70\x01\x69\x01\x74\x01\x61\x01\x72\x01\x7a\x01"+
        "\x79\x01\x70\x01\x65\x01\x72\x02\x69\x02\x74\x01\x65\x06\uffff\x01"+
        "\x2e\x04\uffff\x01\x3d\x01\x3e\x01\uffff\x01\x5b\x03\uffff\x01\x3d"+
        "\x01\uffff\x01\x3d\x08\uffff\x02\uffff\x06\uffff\x01\x3d\x02\uffff"+
        "\x01\x3d\x0d\uffff\x01\x61\x01\x65\x01\x63\x01\x73\x01\x74\x01\x61"+
        "\x01\x65\x01\uffff\x01\x61\x02\x65\x01\x68\x01\x73\x01\x61\x01\x7a"+
        "\x01\x63\x01\uffff\x01\x6f\x01\x74\x01\x65\x01\x6c\x02\uffff\x01"+
        "\x69\x01\x65\x01\x7a\x01\x6c\x01\x6b\x01\x76\x01\x74\x01\x6c\x01"+
        "\x75\x01\x65\x01\x74\x01\x7a\x01\x74\x01\x73\x01\x6f\x01\uffff\x01"+
        "\x65\x01\x7a\x01\x65\x02\x7a\x01\x64\x01\x6c\x01\x68\x01\x7a\x01"+
        "\x72\x12\uffff\x01\x6b\x01\x7a\x01\x68\x01\x73\x01\x74\x01\x69\x01"+
        "\x75\x01\x74\x01\x6d\x01\x7a\x01\x6e\x01\x7a\x01\x65\x01\x6c\x01"+
        "\uffff\x01\x74\x01\x65\x01\x72\x01\x61\x01\x72\x01\x75\x01\x76\x01"+
        "\x73\x01\uffff\x01\x7a\x02\x61\x01\x65\x01\x69\x02\x72\x01\x63\x01"+
        "\uffff\x01\x69\x01\x7a\x01\x77\x01\x7a\x01\uffff\x01\x6f\x02\uffff"+
        "\x01\x7a\x01\x65\x01\x7a\x01\uffff\x01\x72\x01\x7a\x01\uffff\x03"+
        "\x7a\x01\x6e\x01\x6c\x01\x65\x01\x69\x01\uffff\x01\x64\x01\uffff"+
        "\x02\x7a\x01\x69\x01\x6d\x01\x74\x02\x6e\x01\x64\x01\x65\x01\x70"+
        "\x01\uffff\x01\x67\x01\x74\x02\x63\x01\x6e\x01\x7a\x01\x68\x01\x63"+
        "\x01\uffff\x01\x7a\x01\uffff\x01\x66\x01\uffff\x01\x7a\x01\uffff"+
        "\x01\x69\x04\uffff\x01\x75\x01\x74\x01\x7a\x01\x63\x01\x73\x01\uffff"+
        "\x01\x79\x01\uffff\x01\x6f\x01\x65\x01\x7a\x01\x63\x02\x61\x01\x65"+
        "\x01\x7a\x01\x61\x02\x65\x01\x74\x02\x7a\x01\uffff\x02\x7a\x01\uffff"+
        "\x01\x7a\x01\uffff\x01\x64\x01\x65\x01\x7a\x01\uffff\x03\x7a\x02"+
        "\x6e\x01\uffff\x01\x65\x01\x63\x01\x6c\x01\x7a\x01\uffff\x01\x63"+
        "\x02\x7a\x01\x65\x05\uffff\x01\x65\x01\x7a\x04\uffff\x01\x7a\x01"+
        "\x74\x01\x6f\x01\x65\x01\x7a\x01\uffff\x01\x65\x02\uffff\x01\x64"+
        "\x01\x7a\x02\uffff\x01\x73\x01\x66\x01\x7a\x01\uffff\x02\x7a\x01"+
        "\uffff\x02\x7a\x05\uffff";
    const string DFA30_acceptS =
        "\x12\uffff\x01\x37\x01\x38\x01\x39\x01\x3a\x01\x3b\x01\x3c\x01"+
        "\x3d\x01\uffff\x01\x3f\x01\uffff\x01\x41\x04\uffff\x01\x49\x05\uffff"+
        "\x01\x53\x01\x56\x01\uffff\x01\x68\x01\x6b\x02\x6c\x03\uffff\x01"+
        "\x73\x01\x74\x02\uffff\x01\x74\x01\x78\x01\uffff\x01\x74\x28\uffff"+
        "\x01\x37\x01\x39\x01\x3a\x01\x3b\x01\x3c\x01\x3d\x01\uffff\x01\x3e"+
        "\x01\x73\x01\x3f\x01\x42\x02\uffff\x01\x69\x01\uffff\x01\x77\x01"+
        "\x40\x01\x41\x01\uffff\x01\x58\x01\uffff\x01\x52\x01\x4c\x01\x5b"+
        "\x01\x47\x01\x4d\x01\x5c\x01\x48\x01\x49\x02\uffff\x01\x6d\x01\x6e"+
        "\x01\x4a\x01\x71\x01\x5a\x01\x4b\x01\uffff\x01\x60\x01\x4f\x01\uffff"+
        "\x01\x62\x01\x50\x01\x61\x01\x51\x01\x53\x01\x56\x01\x67\x01\x57"+
        "\x01\x68\x01\x6f\x01\x70\x01\x72\x01\x01\x07\uffff\x01\x0a\x08\uffff"+
        "\x01\x11\x04\uffff\x01\x14\x01\x18\x0f\uffff\x01\x25\x0a\uffff\x01"+
        "\x63\x01\x64\x01\x5d\x01\x4e\x01\x6a\x01\x66\x01\x75\x01\x76\x01"+
        "\x45\x01\x43\x01\x46\x01\x44\x01\x59\x01\x65\x01\x5e\x01\x54\x01"+
        "\x5f\x01\x55\x0e\uffff\x01\x0f\x08\uffff\x01\x1a\x08\uffff\x01\x30"+
        "\x04\uffff\x01\x27\x01\uffff\x01\x29\x01\x2a\x03\uffff\x01\x2f\x02"+
        "\uffff\x01\x03\x07\uffff\x01\x0b\x01\uffff\x01\x2e\x0a\uffff\x01"+
        "\x1b\x08\uffff\x01\x23\x01\uffff\x01\x26\x01\uffff\x01\x2b\x01\uffff"+
        "\x01\x2d\x01\uffff\x01\x02\x01\x04\x01\x05\x01\x06\x05\uffff\x01"+
        "\x0d\x01\uffff\x01\x34\x0e\uffff\x01\x21\x02\uffff\x01\x24\x01\uffff"+
        "\x01\x2c\x03\uffff\x01\x09\x05\uffff\x01\x13\x04\uffff\x01\x19\x04"+
        "\uffff\x01\x1f\x01\x20\x01\x22\x01\x36\x01\x28\x02\uffff\x01\x08"+
        "\x01\x33\x01\x0c\x01\x0e\x05\uffff\x01\x32\x01\uffff\x01\x1c\x01"+
        "\x1d\x02\uffff\x01\x07\x01\x10\x03\uffff\x01\x17\x02\uffff\x01\x35"+
        "\x02\uffff\x01\x16\x01\x31\x01\x1e\x01\x12\x01\x15";
    const string DFA30_specialS =
        "\x22\uffff\x01\x05\x0b\uffff\x01\x00\x01\x01\x04\uffff\x01\x02"+
        "\x49\uffff\x01\x03\x01\x04\u0108\uffff}>";
    static readonly string[] DFA30_transitionS = {
            "\x01\x2d\x01\x2b\x02\x2d\x01\x2b\x0e\uffff\x04\x2d\x01\x2c"+
            "\x01\x1e\x01\x2f\x01\x36\x01\x32\x01\x23\x01\x24\x01\x2e\x01"+
            "\x15\x01\x16\x01\x21\x01\x1f\x01\x1a\x01\x20\x01\x19\x01\x22"+
            "\x01\x30\x09\x31\x01\x29\x01\x12\x01\x1b\x01\x1d\x01\x1c\x01"+
            "\x28\x01\x2a\x1a\x35\x01\x17\x01\x33\x01\x18\x01\x26\x01\x35"+
            "\x01\x36\x01\x01\x01\x02\x01\x03\x01\x04\x01\x05\x01\x06\x01"+
            "\x10\x01\x35\x01\x07\x04\x35\x01\x08\x01\x11\x01\x09\x01\x35"+
            "\x01\x0a\x01\x0b\x01\x0c\x01\x0d\x01\x0e\x01\x0f\x03\x35\x01"+
            "\x13\x01\x25\x01\x14\x01\x27\x21\uffff\x01\x2d",
            "\x01\x37",
            "\x01\x39",
            "\x01\x3a\x0a\uffff\x01\x3b\x02\uffff\x01\x3c",
            "\x01\x3d\x09\uffff\x01\x3e\x09\uffff\x01\x3f",
            "\x01\x42\x0a\uffff\x01\x40\x0b\uffff\x01\x41",
            "\x01\x43\x07\uffff\x01\x44\x05\uffff\x01\x45\x05\uffff\x01"+
            "\x46",
            "\x01\x47\x06\uffff\x01\x48\x01\x49\x04\uffff\x01\x4a",
            "\x01\x4b\x03\uffff\x01\x4c\x0f\uffff\x01\x4d",
            "\x01\x4e\x10\uffff\x01\x4f\x02\uffff\x01\x50",
            "\x01\x51",
            "\x01\x54\x0e\uffff\x01\x55\x01\x52\x01\uffff\x01\x53",
            "\x01\x56\x06\uffff\x01\x57\x02\uffff\x01\x58\x06\uffff\x01"+
            "\x59",
            "\x01\x5a",
            "\x01\x5b\x0d\uffff\x01\x5c",
            "\x01\x5d\x01\x5e",
            "\x01\x5f",
            "\x01\x60",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "\x01\x67\x01\uffff\x0a\x69",
            "",
            "\x01\x6f\x0d\uffff\x01\x6d\x0c\uffff\x01\x6c\x01\x6b\x01\x6e"+
            "\x01\x70",
            "",
            "\x01\x73",
            "\x01\x75",
            "\x01\x77\x11\uffff\x01\x78",
            "\x01\x7a\x0f\uffff\x01\x7b",
            "",
            "\x0a\u0083\x01\uffff\x02\u0083\x01\uffff\x1c\u0083\x01\u0080"+
            "\x04\u0083\x01\u0081\x0d\u0083\x01\x7e\x01\x7f\uffc1\u0083",
            "\x01\u0084",
            "\x01\u0086\x16\uffff\x01\u0087",
            "\x01\u008a\x3e\uffff\x01\u0089",
            "\x01\u008c",
            "",
            "",
            "\x01\u0090",
            "",
            "",
            "",
            "",
            "\x00\u0093",
            "\x00\u0094",
            "\x01\u0095\x1f\uffff\x01\u0095",
            "",
            "",
            "\x01\x38",
            "\x01\uffff",
            "",
            "",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "",
            "\x01\u0097",
            "\x01\u0098\x01\u0099",
            "\x01\u009a",
            "\x01\u009b",
            "\x01\u009c\x05\uffff\x01\u009d",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u009f",
            "\x01\u00a0",
            "\x01\u00a1",
            "\x01\u00a2",
            "\x01\u00a3",
            "\x01\u00a4",
            "\x01\u00a5",
            "\x01\u00a6",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u00a8",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x02\x38\x01\u00ab\x0f\x38\x01\u00a9\x01\u00aa"+
            "\x06\x38",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u00af\x06\uffff\x01\u00ae",
            "\x01\u00b0",
            "\x01\u00b1",
            "\x01\u00b2",
            "\x01\u00b3\x05\uffff\x01\u00b4",
            "\x01\u00b5",
            "\x01\u00b6",
            "\x01\u00b7",
            "\x01\u00b8",
            "\x01\u00b9",
            "\x01\u00ba",
            "\x01\u00bb\x08\uffff\x01\u00bc",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u00be\x03\uffff\x01\u00bf",
            "\x01\u00c0",
            "\x01\u00c1",
            "\x01\u00c2",
            "\x01\u00c3",
            "\x01\u00c4",
            "\x01\u00c5",
            "\x01\u00c6",
            "\x01\u00c7",
            "",
            "",
            "",
            "",
            "",
            "",
            "\x01\u00c8",
            "",
            "",
            "",
            "",
            "\x01\u00ca",
            "\x01\u00cc",
            "",
            "\x01\u00ce\x2d\uffff\x01\u00cf",
            "",
            "",
            "",
            "\x01\u00d0",
            "",
            "\x01\u00d2",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "\x0a\u0083\x01\uffff\x02\u0083\x01\uffff\ufff2\u0083",
            "\x0a\u0083\x01\uffff\x02\u0083\x01\uffff\ufff2\u0083",
            "",
            "",
            "",
            "",
            "",
            "",
            "\x01\u00d6",
            "",
            "",
            "\x01\u00d8",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "\x01\u00da",
            "\x01\u00db",
            "\x01\u00dc",
            "\x01\u00dd",
            "\x01\u00de\x01\u00df",
            "\x01\u00e0",
            "\x01\u00e1",
            "",
            "\x01\u00e2",
            "\x01\u00e3",
            "\x01\u00e4",
            "\x01\u00e5",
            "\x01\u00e6",
            "\x01\u00e7",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u00e9",
            "",
            "\x01\u00ea\x02\uffff\x01\u00eb",
            "\x01\u00ec",
            "\x01\u00ed",
            "\x01\u00ee",
            "",
            "",
            "\x01\u00ef",
            "\x01\u00f0",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u00f2",
            "\x01\u00f3",
            "\x01\u00f4",
            "\x01\u00f5",
            "\x01\u00f6",
            "\x01\u00f7",
            "\x01\u00f8",
            "\x01\u00f9",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u00fb",
            "\x01\u00fc",
            "\x01\u00fd",
            "",
            "\x01\u00fe",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u0100",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u0103",
            "\x01\u0104",
            "\x01\u0105",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u0107",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "\x01\u0108",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u010a",
            "\x01\u010b",
            "\x01\u010c",
            "\x01\u010d",
            "\x01\u010e",
            "\x01\u010f",
            "\x01\u0110",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u0112",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u0114",
            "\x01\u0115",
            "",
            "\x01\u0116",
            "\x01\u0117",
            "\x01\u0118",
            "\x01\u0119",
            "\x01\u011a",
            "\x01\u011b",
            "\x01\u011c",
            "\x01\u011d",
            "",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u011f",
            "\x01\u0120",
            "\x01\u0121",
            "\x01\u0122",
            "\x01\u0123",
            "\x01\u0124",
            "\x01\u0125",
            "",
            "\x01\u0126",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u0128",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "",
            "\x01\u012a",
            "",
            "",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u012c",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "",
            "\x01\u012e",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u0133",
            "\x01\u0134",
            "\x01\u0135",
            "\x01\u0136",
            "",
            "\x01\u0137",
            "",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x0b\x38\x01\u0139\x0e\x38",
            "\x01\u013b",
            "\x01\u013c",
            "\x01\u013d",
            "\x01\u013e",
            "\x01\u013f\x07\uffff\x01\u0140",
            "\x01\u0141",
            "\x01\u0142",
            "\x01\u0143",
            "",
            "\x01\u0144",
            "\x01\u0145",
            "\x01\u0146",
            "\x01\u0147",
            "\x01\u0148",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u014a",
            "\x01\u014b",
            "",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "",
            "\x01\u014d",
            "",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "",
            "\x01\u014f",
            "",
            "",
            "",
            "",
            "\x01\u0150",
            "\x01\u0151",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u0153",
            "\x01\u0154",
            "",
            "\x01\u0155",
            "",
            "\x01\u0156",
            "\x01\u0157",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u0159",
            "\x01\u015a",
            "\x01\u015b",
            "\x01\u015c",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u015e",
            "\x01\u015f",
            "\x01\u0160",
            "\x01\u0161",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "",
            "\x01\u0167",
            "\x01\u0168",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u016d",
            "\x01\u016e",
            "",
            "\x01\u016f",
            "\x01\u0170",
            "\x01\u0171",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "",
            "\x01\u0173",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u0176",
            "",
            "",
            "",
            "",
            "",
            "\x01\u0177",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "",
            "",
            "",
            "",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\u017a",
            "\x01\u017b",
            "\x01\u017c",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "",
            "\x01\u017e",
            "",
            "",
            "\x01\u017f",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "",
            "",
            "\x01\u0181",
            "\x01\u0182",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "\x01\x38\x0b\uffff\x0a\x38\x07\uffff\x1a\x38\x04\uffff\x01"+
            "\x38\x01\uffff\x1a\x38",
            "",
            "",
            "",
            "",
            ""
    };

    static readonly short[] DFA30_eot = DFA.UnpackEncodedString(DFA30_eotS);
    static readonly short[] DFA30_eof = DFA.UnpackEncodedString(DFA30_eofS);
    static readonly char[] DFA30_min = DFA.UnpackEncodedStringToUnsignedChars(DFA30_minS);
    static readonly char[] DFA30_max = DFA.UnpackEncodedStringToUnsignedChars(DFA30_maxS);
    static readonly short[] DFA30_accept = DFA.UnpackEncodedString(DFA30_acceptS);
    static readonly short[] DFA30_special = DFA.UnpackEncodedString(DFA30_specialS);
    static readonly short[][] DFA30_transition = DFA.UnpackEncodedStringArray(DFA30_transitionS);

    protected class DFA30 : DFA
    {
        public DFA30(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 30;
            this.eot = DFA30_eot;
            this.eof = DFA30_eof;
            this.min = DFA30_min;
            this.max = DFA30_max;
            this.accept = DFA30_accept;
            this.special = DFA30_special;
            this.transition = DFA30_transition;

        }

        override public string Description
        {
            get { return "1:1: Tokens : ( AS | BREAK | CASE | CATCH | CLASS | CONST | CONTINUE | DEFAULT | DELETE | DO | ELSE | EXTENDS | FALSE | FINALLY | FOR | FUNCTION | IF | IMPLEMENTS | IMPORT | IN | INSTANCEOF | INTERFACE | INTERNAL | IS | NATIVE | NEW | NULL | PACKAGE | PRIVATE | PROTECTED | PUBLIC | RETURN | SUPER | SWITCH | THIS | THROW | TO | TRUE | TRY | TYPEOF | USE | VAR | VOID | WHILE | WITH | EACH | GET | SET | NAMESPACE | INCLUDE | DYNAMIC | FINAL | OVERRIDE | STATIC | SEMI | LCURLY | RCURLY | LPAREN | RPAREN | LBRACK | RBRACK | DOT | COMMA | LT | GT | LTE | EQ | NEQ | SAME | NSAME | PLUS | SUB | STAR | DIV | MOD | INC | DEC | SHL | AND | OR | XOR | NOT | INV | LAND | LOR | QUE | COLON | ASSIGN | DIV_ASSIGN | MOD_ASSIGN | ADD_ASSIGN | SUB_ASSIGN | SHL_ASSIGN | LAND_ASSIGN | LOR_ASSIGN | AND_ASSIGN | XOR_ASSIGN | OR_ASSIGN | ELLIPSIS | XML_ELLIPSIS | XML_TEND | XML_E_TEND | XML_NS_OP | XML_AT | XML_LS_STD | XML_LS_END | EOL | WHITESPACE | COMMENT_MULTILINE | COMMENT_SINGLELINE | SINGLE_QUOTE_LITERAL | DOUBLE_QUOTE_LITERAL | REGULAR_EXPR_LITERAL | HEX_NUMBER_LITERAL | DEC_NUMBER_LITERAL | IDENTIFIER | XML_COMMENT | XML_CDATA | XML_PI | XML_TEXT );"; }
        }

    }


    protected internal int DFA30_SpecialStateTransition(DFA dfa, int s, IIntStream _input) //throws NoViableAltException
    {
            IIntStream input = _input;
        int _s = s;
        switch ( s )
        {
                case 0 : 
                    int LA30_46 = input.LA(1);

                    s = -1;
                    if ( ((LA30_46 >= '\u0000' && LA30_46 <= '\uFFFF')) ) { s = 147; }

                    else s = 54;

                    if ( s >= 0 ) return s;
                    break;
                case 1 : 
                    int LA30_47 = input.LA(1);

                    s = -1;
                    if ( ((LA30_47 >= '\u0000' && LA30_47 <= '\uFFFF')) ) { s = 148; }

                    else s = 54;

                    if ( s >= 0 ) return s;
                    break;
                case 2 : 
                    int LA30_52 = input.LA(1);

                     
                    int index30_52 = input.Index();
                    input.Rewind();
                    s = -1;
                    if ( (!(((isXMLText(input.LA(1)))))) ) { s = 56; }

                    else if ( ((isXMLText(input.LA(1)))) ) { s = 54; }

                     
                    input.Seek(index30_52);
                    if ( s >= 0 ) return s;
                    break;
                case 3 : 
                    int LA30_126 = input.LA(1);

                     
                    int index30_126 = input.Index();
                    input.Rewind();
                    s = -1;
                    if ( ((LA30_126 >= '\u0000' && LA30_126 <= '\t') || (LA30_126 >= '\u000B' && LA30_126 <= '\f') || (LA30_126 >= '\u000E' && LA30_126 <= '\uFFFF')) && ((isRegularExpression())) ) { s = 131; }

                    else s = 212;

                     
                    input.Seek(index30_126);
                    if ( s >= 0 ) return s;
                    break;
                case 4 : 
                    int LA30_127 = input.LA(1);

                     
                    int index30_127 = input.Index();
                    input.Rewind();
                    s = -1;
                    if ( ((LA30_127 >= '\u0000' && LA30_127 <= '\t') || (LA30_127 >= '\u000B' && LA30_127 <= '\f') || (LA30_127 >= '\u000E' && LA30_127 <= '\uFFFF')) && ((isRegularExpression())) ) { s = 131; }

                    else s = 213;

                     
                    input.Seek(index30_127);
                    if ( s >= 0 ) return s;
                    break;
                case 5 : 
                    int LA30_34 = input.LA(1);

                     
                    int index30_34 = input.Index();
                    input.Rewind();
                    s = -1;
                    if ( (LA30_34 == '=') ) { s = 126; }

                    else if ( (LA30_34 == '>') ) { s = 127; }

                    else if ( (LA30_34 == '*') ) { s = 128; }

                    else if ( (LA30_34 == '/') ) { s = 129; }

                    else if ( ((LA30_34 >= '\u0000' && LA30_34 <= '\t') || (LA30_34 >= '\u000B' && LA30_34 <= '\f') || (LA30_34 >= '\u000E' && LA30_34 <= ')') || (LA30_34 >= '+' && LA30_34 <= '.') || (LA30_34 >= '0' && LA30_34 <= '<') || (LA30_34 >= '?' && LA30_34 <= '\uFFFF')) && ((isRegularExpression())) ) { s = 131; }

                    else s = 130;

                     
                    input.Seek(index30_34);
                    if ( s >= 0 ) return s;
                    break;
        }
        if (state.backtracking > 0) {state.failed = true; return -1;}
        NoViableAltException nvae =
            new NoViableAltException(dfa.Description, 30, _s, input);
        dfa.Error(nvae);
        throw nvae;
    }
 
    
}
