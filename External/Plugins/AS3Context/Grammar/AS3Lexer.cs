// $ANTLR 2.7.7 (20060930): "as3.g" -> "AS3Lexer.cs"$


/**********************************************************
 * ActionScript Development Tool
 * Copyright (C) 2005 asdt.org
 *
 * http://www.asdt.org
 * http://sourceforge.net/projects/aseclipseplugin/
 *
 * This program is free software;
 * you can redistribute it and/or modify it under the terms of
 * the GNU General Public License as published by the
 * Free Software Foundation; either version 2 of the License,
 * or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the
 * Free Software Foundation, Inc.,
 * 59 Temple Place, Suite 330, Boston, MA 02111-1307, USA.
 *
 * Created on 29 sept 2005
 *
 * Contribution:
 * - metadatas parsing, by Philippe Elsass, 9 may 2007
 *
 **********************************************************/


namespace AS3Context.Grammar
{
    // Generate header specific to lexer CSharp file
    using System;
    using Stream                          = System.IO.Stream;
    using TextReader                      = System.IO.TextReader;
    using Hashtable                       = System.Collections.Hashtable;
    using Comparer                        = System.Collections.Comparer;
    
    using TokenStreamException            = antlr.TokenStreamException;
    using TokenStreamIOException          = antlr.TokenStreamIOException;
    using TokenStreamRecognitionException = antlr.TokenStreamRecognitionException;
    using CharStreamException             = antlr.CharStreamException;
    using CharStreamIOException           = antlr.CharStreamIOException;
    using ANTLRException                  = antlr.ANTLRException;
    using CharScanner                     = antlr.CharScanner;
    using InputBuffer                     = antlr.InputBuffer;
    using ByteBuffer                      = antlr.ByteBuffer;
    using CharBuffer                      = antlr.CharBuffer;
    using Token                           = antlr.Token;
    using IToken                          = antlr.IToken;
    using CommonToken                     = antlr.CommonToken;
    using SemanticException               = antlr.SemanticException;
    using RecognitionException            = antlr.RecognitionException;
    using NoViableAltForCharException     = antlr.NoViableAltForCharException;
    using MismatchedCharException         = antlr.MismatchedCharException;
    using TokenStream                     = antlr.TokenStream;
    using LexerSharedInputState           = antlr.LexerSharedInputState;
    using BitSet                          = antlr.collections.impl.BitSet;
    
    public  class AS3Lexer : antlr.CharScanner  , TokenStream
     {
        public const int EOF = 1;
        public const int NULL_TREE_LOOKAHEAD = 3;
        public const int COMPILATION_UNIT = 4;
        public const int IMPORT = 5;
        public const int CLASS_DEF = 6;
        public const int INTERFACE_DEF = 7;
        public const int EXTENDS_CLAUSE = 8;
        public const int IMPLEMENTS_CLAUSE = 9;
        public const int TYPE_BLOCK = 10;
        public const int MODIFIERS = 11;
        public const int VARIABLE_DEF = 12;
        public const int METHOD_DEF = 13;
        public const int NAMESPACE_DEF = 14;
        public const int PARAMS = 15;
        public const int PARAM = 16;
        public const int TYPE_SPEC = 17;
        public const int BLOCK = 18;
        public const int EXPR = 19;
        public const int ELIST = 20;
        public const int EXPR_STMNT = 21;
        public const int NEW_EXPR = 22;
        public const int ENCPS_EXPR = 23;
        public const int VAR_INIT = 24;
        public const int METHOD_CALL = 25;
        public const int ARRAY_ACC = 26;
        public const int UNARY_PLUS = 27;
        public const int UNARY_MINUS = 28;
        public const int POST_INC = 29;
        public const int POST_DEC = 30;
        public const int ARRAY_LITERAL = 31;
        public const int ELEMENT = 32;
        public const int OBJECT_LITERAL = 33;
        public const int OBJECT_FIELD = 34;
        public const int FUNC_DEF = 35;
        public const int FOR_INIT = 36;
        public const int FOR_CONDITION = 37;
        public const int FOR_ITERATOR = 38;
        public const int LITERAL_package = 39;
        public const int LCURLY = 40;
        public const int SEMI = 41;
        public const int RCURLY = 42;
        public const int LITERAL_import = 43;
        public const int LBRACK = 44;
        public const int COMMA = 45;
        public const int RBRACK = 46;
        public const int LPAREN = 47;
        public const int RPAREN = 48;
        public const int LITERAL_class = 49;
        public const int LITERAL_interface = 50;
        public const int LITERAL_extends = 51;
        public const int LITERAL_implements = 52;
        public const int LITERAL_function = 53;
        public const int LITERAL_get = 54;
        public const int LITERAL_set = 55;
        // "/n" = 56
        // "/r" = 57
        public const int IDENT = 58;
        public const int LITERAL_namespace = 59;
        public const int LITERAL_var = 60;
        public const int LITERAL_const = 61;
        public const int ASSIGN = 62;
        public const int REST = 63;
        public const int LITERAL_while = 64;
        public const int LITERAL_do = 65;
        public const int LITERAL_with = 66;
        public const int LITERAL_if = 67;
        public const int LITERAL_else = 68;
        public const int LITERAL_throw = 69;
        public const int LITERAL_return = 70;
        public const int LITERAL_continue = 71;
        public const int LITERAL_break = 72;
        public const int LITERAL_switch = 73;
        public const int LITERAL_case = 74;
        public const int COLON = 75;
        public const int LITERAL_default = 76;
        public const int LITERAL_for = 77;
        public const int LITERAL_each = 78;
        public const int LITERAL_in = 79;
        public const int DOT = 80;
        public const int STAR = 81;
        public const int LITERAL_public = 82;
        public const int LITERAL_private = 83;
        public const int LITERAL_protected = 84;
        public const int LITERAL_internal = 85;
        public const int LITERAL_static = 86;
        public const int LITERAL_final = 87;
        public const int LITERAL_enumerable = 88;
        public const int LITERAL_explicit = 89;
        public const int LITERAL_override = 90;
        public const int LITERAL_dynamic = 91;
        public const int NUMBER = 92;
        public const int STAR_ASSIGN = 93;
        public const int DIV_ASSIGN = 94;
        public const int MOD_ASSIGN = 95;
        public const int PLUS_ASSIGN = 96;
        public const int MINUS_ASSIGN = 97;
        public const int SL_ASSIGN = 98;
        public const int SR_ASSIGN = 99;
        public const int BSR_ASSIGN = 100;
        public const int BAND_ASSIGN = 101;
        public const int BXOR_ASSIGN = 102;
        public const int BOR_ASSIGN = 103;
        public const int LAND_ASSIGN = 104;
        public const int LOR_ASSIGN = 105;
        public const int QUESTION = 106;
        public const int LOR = 107;
        public const int LAND = 108;
        public const int BOR = 109;
        public const int BXOR = 110;
        public const int BAND = 111;
        public const int STRICT_EQUAL = 112;
        public const int STRICT_NOT_EQUAL = 113;
        public const int NOT_EQUAL = 114;
        public const int EQUAL = 115;
        public const int LWT = 116;
        public const int GT = 117;
        public const int LE = 118;
        public const int GE = 119;
        public const int LITERAL_is = 120;
        public const int LITERAL_as = 121;
        public const int SL = 122;
        public const int SR = 123;
        public const int BSR = 124;
        public const int PLUS = 125;
        public const int MINUS = 126;
        public const int DIV = 127;
        public const int MOD = 128;
        public const int INC = 129;
        public const int DEC = 130;
        public const int LITERAL_delete = 131;
        public const int LITERAL_typeof = 132;
        public const int LNOT = 133;
        public const int BNOT = 134;
        public const int E4X_DESC = 135;
        public const int E4X_ATTRI = 136;
        public const int LITERAL_null = 137;
        public const int LITERAL_true = 138;
        public const int LITERAL_false = 139;
        public const int LITERAL_undefined = 140;
        public const int STRING_LITERAL = 141;
        public const int REGEX_LITERAL = 142;
        public const int XML_LITERAL = 143;
        public const int LITERAL_new = 144;
        public const int DBL_COLON = 145;
        public const int XML_ATTRIBUTE = 146;
        public const int XML_BINDING = 147;
        public const int XML_AS3_EXPRESSION = 148;
        public const int XML_TEXTNODE = 149;
        public const int XML_COMMENT = 150;
        public const int XML_CDATA = 151;
        public const int REGEX_BODY = 152;
        public const int WS = 153;
        public const int NL = 154;
        public const int BOM = 155;
        public const int SL_COMMENT = 156;
        public const int ML_COMMENT = 157;
        public const int EXPONENT = 158;
        public const int HEX_DIGIT = 159;
        public const int ESC = 160;
        
        public AS3Lexer(Stream ins) : this(new ByteBuffer(ins))
        {
        }
        
        public AS3Lexer(TextReader r) : this(new CharBuffer(r))
        {
        }
        
        public AS3Lexer(InputBuffer ib)      : this(new LexerSharedInputState(ib))
        {
        }
        
        public AS3Lexer(LexerSharedInputState state) : base(state)
        {
            initialize();
        }
        private void initialize()
        {
            caseSensitiveLiterals = true;
            setCaseSensitive(true);
            literals = new Hashtable(100, (float) 0.4, null, Comparer.Default);
            literals.Add("public", 82);
            literals.Add("namespace", 59);
            literals.Add("case", 74);
            literals.Add("break", 72);
            literals.Add("while", 64);
            literals.Add("delete", 131);
            literals.Add("new", 144);
            literals.Add("/r", 57);
            literals.Add("implements", 52);
            literals.Add("typeof", 132);
            literals.Add("const", 61);
            literals.Add("package", 39);
            literals.Add("return", 70);
            literals.Add("throw", 69);
            literals.Add("var", 60);
            literals.Add("null", 137);
            literals.Add("protected", 84);
            literals.Add("class", 49);
            literals.Add("do", 65);
            literals.Add("function", 53);
            literals.Add("each", 78);
            literals.Add("with", 66);
            literals.Add("set", 55);
            literals.Add("/n", 56);
            literals.Add("dynamic", 91);
            literals.Add("interface", 50);
            literals.Add("is", 120);
            literals.Add("internal", 85);
            literals.Add("final", 87);
            literals.Add("explicit", 89);
            literals.Add("if", 67);
            literals.Add("override", 90);
            literals.Add("as", 121);
            literals.Add("for", 77);
            literals.Add("extends", 51);
            literals.Add("private", 83);
            literals.Add("default", 76);
            literals.Add("false", 139);
            literals.Add("static", 86);
            literals.Add("undefined", 140);
            literals.Add("get", 54);
            literals.Add("continue", 71);
            literals.Add("enumerable", 88);
            literals.Add("else", 68);
            literals.Add("import", 43);
            literals.Add("in", 79);
            literals.Add("switch", 73);
            literals.Add("true", 138);
        }
        
        override public IToken nextToken()          //throws TokenStreamException
        {
            IToken theRetToken = null;
tryAgain:
            for (;;)
            {
                IToken _token = null;
                int _ttype = Token.INVALID_TYPE;
                resetText();
                try     // for char stream error handling
                {
                    try     // for lexical error handling
                    {
                        switch ( cached_LA1 )
                        {
                        case '?':
                        {
                            mQUESTION(true);
                            theRetToken = returnToken_;
                            break;
                        }
                        case '(':
                        {
                            mLPAREN(true);
                            theRetToken = returnToken_;
                            break;
                        }
                        case ')':
                        {
                            mRPAREN(true);
                            theRetToken = returnToken_;
                            break;
                        }
                        case '[':
                        {
                            mLBRACK(true);
                            theRetToken = returnToken_;
                            break;
                        }
                        case ']':
                        {
                            mRBRACK(true);
                            theRetToken = returnToken_;
                            break;
                        }
                        case '{':
                        {
                            mLCURLY(true);
                            theRetToken = returnToken_;
                            break;
                        }
                        case '}':
                        {
                            mRCURLY(true);
                            theRetToken = returnToken_;
                            break;
                        }
                        case ',':
                        {
                            mCOMMA(true);
                            theRetToken = returnToken_;
                            break;
                        }
                        case '~':
                        {
                            mBNOT(true);
                            theRetToken = returnToken_;
                            break;
                        }
                        case '@':
                        {
                            mE4X_ATTRI(true);
                            theRetToken = returnToken_;
                            break;
                        }
                        case ';':
                        {
                            mSEMI(true);
                            theRetToken = returnToken_;
                            break;
                        }
                        case '$':  case 'A':  case 'B':  case 'C':
                        case 'D':  case 'E':  case 'F':  case 'G':
                        case 'H':  case 'I':  case 'J':  case 'K':
                        case 'L':  case 'M':  case 'N':  case 'O':
                        case 'P':  case 'Q':  case 'R':  case 'S':
                        case 'T':  case 'U':  case 'V':  case 'W':
                        case 'X':  case 'Y':  case 'Z':  case '_':
                        case 'a':  case 'b':  case 'c':  case 'd':
                        case 'e':  case 'f':  case 'g':  case 'h':
                        case 'i':  case 'j':  case 'k':  case 'l':
                        case 'm':  case 'n':  case 'o':  case 'p':
                        case 'q':  case 'r':  case 's':  case 't':
                        case 'u':  case 'v':  case 'w':  case 'x':
                        case 'y':  case 'z':
                        {
                            mIDENT(true);
                            theRetToken = returnToken_;
                            break;
                        }
                        case '"':  case '\'':
                        {
                            mSTRING_LITERAL(true);
                            theRetToken = returnToken_;
                            break;
                        }
                        case '\t':  case '\n':  case '\u000c':  case '\r':
                        case ' ':
                        {
                            mWS(true);
                            theRetToken = returnToken_;
                            break;
                        }
                        case '.':  case '0':  case '1':  case '2':
                        case '3':  case '4':  case '5':  case '6':
                        case '7':  case '8':  case '9':
                        {
                            mNUMBER(true);
                            theRetToken = returnToken_;
                            break;
                        }
                        case '\u00bb':  case '\u00bf':  case '\u00ef':
                        {
                            mBOM(true);
                            theRetToken = returnToken_;
                            break;
                        }
                        default:
                            if ((cached_LA1=='>') && (cached_LA2=='>') && (LA(3)=='>') && (LA(4)=='='))
                            {
                                mBSR_ASSIGN(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='=') && (cached_LA2=='=') && (LA(3)=='=')) {
                                mSTRICT_EQUAL(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='!') && (cached_LA2=='=') && (LA(3)=='=')) {
                                mSTRICT_NOT_EQUAL(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='>') && (cached_LA2=='>') && (LA(3)=='=')) {
                                mSR_ASSIGN(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='>') && (cached_LA2=='>') && (LA(3)=='>') && (true)) {
                                mBSR(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='<') && (cached_LA2=='<') && (LA(3)=='=')) {
                                mSL_ASSIGN(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='&') && (cached_LA2=='&') && (LA(3)=='=')) {
                                mLAND_ASSIGN(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='|') && (cached_LA2=='|') && (LA(3)=='=')) {
                                mLOR_ASSIGN(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='/') && (tokenSet_0_.member(cached_LA2)) && (tokenSet_1_.member(LA(3)))) {
                                mREGEX_LITERAL(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1==':') && (cached_LA2==':')) {
                                mDBL_COLON(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='=') && (cached_LA2=='=') && (true)) {
                                mEQUAL(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='!') && (cached_LA2=='=') && (true)) {
                                mNOT_EQUAL(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='/') && (cached_LA2=='=') && (true)) {
                                mDIV_ASSIGN(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='+') && (cached_LA2=='=')) {
                                mPLUS_ASSIGN(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='+') && (cached_LA2=='+')) {
                                mINC(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='-') && (cached_LA2=='=')) {
                                mMINUS_ASSIGN(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='-') && (cached_LA2=='-')) {
                                mDEC(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='*') && (cached_LA2=='=')) {
                                mSTAR_ASSIGN(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='%') && (cached_LA2=='=')) {
                                mMOD_ASSIGN(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='>') && (cached_LA2=='>') && (true)) {
                                mSR(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='>') && (cached_LA2=='=')) {
                                mGE(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='<') && (cached_LA2=='<') && (true)) {
                                mSL(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='<') && (cached_LA2=='=')) {
                                mLE(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='^') && (cached_LA2=='=')) {
                                mBXOR_ASSIGN(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='|') && (cached_LA2=='=')) {
                                mBOR_ASSIGN(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='|') && (cached_LA2=='|') && (true)) {
                                mLOR(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='&') && (cached_LA2=='=')) {
                                mBAND_ASSIGN(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='&') && (cached_LA2=='&') && (true)) {
                                mLAND(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='<') && (tokenSet_2_.member(cached_LA2))) {
                                mXML_LITERAL(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='/') && (cached_LA2=='/')) {
                                mSL_COMMENT(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='/') && (cached_LA2=='*')) {
                                mML_COMMENT(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1==':') && (true)) {
                                mCOLON(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='=') && (true)) {
                                mASSIGN(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='!') && (true)) {
                                mLNOT(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='/') && (true)) {
                                mDIV(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='+') && (true)) {
                                mPLUS(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='-') && (true)) {
                                mMINUS(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='*') && (true)) {
                                mSTAR(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='%') && (true)) {
                                mMOD(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='>') && (true)) {
                                mGT(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='<') && (true)) {
                                mLWT(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='^') && (true)) {
                                mBXOR(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='|') && (true)) {
                                mBOR(true);
                                theRetToken = returnToken_;
                            }
                            else if ((cached_LA1=='&') && (true)) {
                                mBAND(true);
                                theRetToken = returnToken_;
                            }
                        else
                        {
                            if (cached_LA1==EOF_CHAR) { uponEOF(); returnToken_ = makeToken(Token.EOF_TYPE); }
                else {throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());}
                        }
                        break; }
                        if ( null==returnToken_ ) goto tryAgain; // found SKIP token
                        _ttype = returnToken_.Type;
                        returnToken_.Type = _ttype;
                        return returnToken_;
                    }
                    catch (RecognitionException e) {
                            throw new TokenStreamRecognitionException(e);
                    }
                }
                catch (CharStreamException cse) {
                    if ( cse is CharStreamIOException ) {
                        throw new TokenStreamIOException(((CharStreamIOException)cse).io);
                    }
                    else {
                        throw new TokenStreamException(cse.Message);
                    }
                }
            }
        }
        
    public void mQUESTION(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = QUESTION;
        
        match('?');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mLPAREN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = LPAREN;
        
        match('(');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mRPAREN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = RPAREN;
        
        match(')');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mLBRACK(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = LBRACK;
        
        match('[');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mRBRACK(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = RBRACK;
        
        match(']');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mLCURLY(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = LCURLY;
        
        match('{');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mRCURLY(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = RCURLY;
        
        match('}');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mCOLON(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = COLON;
        
        match(':');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mDBL_COLON(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = DBL_COLON;
        
        match("::");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mCOMMA(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = COMMA;
        
        match(',');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = ASSIGN;
        
        match('=');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mEQUAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = EQUAL;
        
        match("==");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mSTRICT_EQUAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = STRICT_EQUAL;
        
        match("===");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mLNOT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = LNOT;
        
        match('!');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mBNOT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = BNOT;
        
        match('~');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mNOT_EQUAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = NOT_EQUAL;
        
        match("!=");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mSTRICT_NOT_EQUAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = STRICT_NOT_EQUAL;
        
        match("!==");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mDIV(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = DIV;
        
        match('/');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mDIV_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = DIV_ASSIGN;
        
        match("/=");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mPLUS(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = PLUS;
        
        match('+');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mPLUS_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = PLUS_ASSIGN;
        
        match("+=");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mINC(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = INC;
        
        match("++");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mMINUS(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = MINUS;
        
        match('-');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mMINUS_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = MINUS_ASSIGN;
        
        match("-=");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mDEC(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = DEC;
        
        match("--");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mSTAR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = STAR;
        
        match('*');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mSTAR_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = STAR_ASSIGN;
        
        match("*=");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mMOD(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = MOD;
        
        match('%');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mMOD_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = MOD_ASSIGN;
        
        match("%=");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mSR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = SR;
        
        match(">>");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mSR_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = SR_ASSIGN;
        
        match(">>=");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mBSR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = BSR;
        
        match(">>>");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mBSR_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = BSR_ASSIGN;
        
        match(">>>=");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mGE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = GE;
        
        match(">=");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mGT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = GT;
        
        match('>');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mSL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = SL;
        
        match("<<");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mSL_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = SL_ASSIGN;
        
        match("<<=");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mLE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = LE;
        
        match("<=");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mLWT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = LWT;
        
        match('<');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mBXOR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = BXOR;
        
        match('^');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mBXOR_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = BXOR_ASSIGN;
        
        match("^=");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mBOR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = BOR;
        
        match('|');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mBOR_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = BOR_ASSIGN;
        
        match("|=");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mLOR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = LOR;
        
        match("||");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mBAND(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = BAND;
        
        match('&');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mBAND_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = BAND_ASSIGN;
        
        match("&=");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mLAND(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = LAND;
        
        match("&&");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mLAND_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = LAND_ASSIGN;
        
        match("&&=");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mLOR_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = LOR_ASSIGN;
        
        match("||=");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mE4X_ATTRI(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = E4X_ATTRI;
        
        match('@');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mSEMI(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = SEMI;
        
        match(';');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    private void mDOT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = DOT;
        
        match('.');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    private void mE4X_DESC(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = E4X_DESC;
        
        match("..");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    private void mREST(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = REST;
        
        match("...");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mIDENT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = IDENT;
        
        {
            switch ( cached_LA1 )
            {
            case 'a':  case 'b':  case 'c':  case 'd':
            case 'e':  case 'f':  case 'g':  case 'h':
            case 'i':  case 'j':  case 'k':  case 'l':
            case 'm':  case 'n':  case 'o':  case 'p':
            case 'q':  case 'r':  case 's':  case 't':
            case 'u':  case 'v':  case 'w':  case 'x':
            case 'y':  case 'z':
            {
                matchRange('a','z');
                break;
            }
            case 'A':  case 'B':  case 'C':  case 'D':
            case 'E':  case 'F':  case 'G':  case 'H':
            case 'I':  case 'J':  case 'K':  case 'L':
            case 'M':  case 'N':  case 'O':  case 'P':
            case 'Q':  case 'R':  case 'S':  case 'T':
            case 'U':  case 'V':  case 'W':  case 'X':
            case 'Y':  case 'Z':
            {
                matchRange('A','Z');
                break;
            }
            case '_':
            {
                match('_');
                break;
            }
            case '$':
            {
                match('$');
                break;
            }
            default:
            {
                throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
            }
             }
        }
        {    // ( ... )*
            for (;;)
            {
                if (((cached_LA1 >= 'a' && cached_LA1 <= 'z')) && (true) && (true) && (true))
                {
                    matchRange('a','z');
                }
                else if (((cached_LA1 >= 'A' && cached_LA1 <= 'Z')) && (true) && (true) && (true)) {
                    matchRange('A','Z');
                }
                else if ((cached_LA1=='_') && (true) && (true) && (true)) {
                    match('_');
                }
                else if (((cached_LA1 >= '0' && cached_LA1 <= '9'))) {
                    matchRange('0','9');
                }
                else if ((cached_LA1=='$') && (true) && (true) && (true)) {
                    match('$');
                }
                else
                {
                    goto _loop271_breakloop;
                }
                
            }
_loop271_breakloop:         ;
        }    // ( ... )*
        _ttype = testLiteralsTable(_ttype);
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mSTRING_LITERAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = STRING_LITERAL;
        
        switch ( cached_LA1 )
        {
        case '"':
        {
            match('"');
            {    // ( ... )*
                for (;;)
                {
                    if ((cached_LA1=='\\'))
                    {
                        mESC(false);
                    }
                    else if ((tokenSet_3_.member(cached_LA1))) {
                        {
                            match(tokenSet_3_);
                        }
                    }
                    else
                    {
                        goto _loop275_breakloop;
                    }
                    
                }
_loop275_breakloop:             ;
            }    // ( ... )*
            match('"');
            break;
        }
        case '\'':
        {
            match('\'');
            {    // ( ... )*
                for (;;)
                {
                    if ((cached_LA1=='\\'))
                    {
                        mESC(false);
                    }
                    else if ((tokenSet_4_.member(cached_LA1))) {
                        {
                            match(tokenSet_4_);
                        }
                    }
                    else
                    {
                        goto _loop278_breakloop;
                    }
                    
                }
_loop278_breakloop:             ;
            }    // ( ... )*
            match('\'');
            break;
        }
        default:
        {
            throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
        }
         }
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    protected void mESC(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = ESC;
        
        match('\\');
        {
            switch ( cached_LA1 )
            {
            case 'n':
            {
                match('n');
                break;
            }
            case 'r':
            {
                match('r');
                break;
            }
            case 't':
            {
                match('t');
                break;
            }
            case 'b':
            {
                match('b');
                break;
            }
            case 'f':
            {
                match('f');
                break;
            }
            case '"':
            {
                match('"');
                break;
            }
            case '\'':
            {
                match('\'');
                break;
            }
            case '\\':
            {
                match('\\');
                break;
            }
            case 'u':
            {
                { // ( ... )+
                    int _cnt378=0;
                    for (;;)
                    {
                        if ((cached_LA1=='u'))
                        {
                            match('u');
                        }
                        else
                        {
                            if (_cnt378 >= 1) { goto _loop378_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
                        }
                        
                        _cnt378++;
                    }
_loop378_breakloop:                 ;
                }    // ( ... )+
                mHEX_DIGIT(false);
                mHEX_DIGIT(false);
                mHEX_DIGIT(false);
                mHEX_DIGIT(false);
                break;
            }
            case '0':  case '1':  case '2':  case '3':
            {
                matchRange('0','3');
                {
                    if (((cached_LA1 >= '0' && cached_LA1 <= '7')) && (tokenSet_1_.member(cached_LA2)) && (true) && (true))
                    {
                        matchRange('0','7');
                        {
                            if (((cached_LA1 >= '0' && cached_LA1 <= '7')) && (tokenSet_1_.member(cached_LA2)) && (true) && (true))
                            {
                                matchRange('0','7');
                            }
                            else if ((tokenSet_1_.member(cached_LA1)) && (true) && (true) && (true)) {
                            }
                            else
                            {
                                throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                            }
                            
                        }
                    }
                    else if ((tokenSet_1_.member(cached_LA1)) && (true) && (true) && (true)) {
                    }
                    else
                    {
                        throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                    }
                    
                }
                break;
            }
            case '4':  case '5':  case '6':  case '7':
            {
                matchRange('4','7');
                {
                    if (((cached_LA1 >= '0' && cached_LA1 <= '7')) && (tokenSet_1_.member(cached_LA2)) && (true) && (true))
                    {
                        matchRange('0','7');
                    }
                    else if ((tokenSet_1_.member(cached_LA1)) && (true) && (true) && (true)) {
                    }
                    else
                    {
                        throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                    }
                    
                }
                break;
            }
            default:
            {
                throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
            }
             }
        }
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mXML_LITERAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = XML_LITERAL;
        
        match('<');
        mIDENT(false);
        {    // ( ... )*
            for (;;)
            {
                switch ( cached_LA1 )
                {
                case '\t':  case '\n':  case '\u000c':  case '\r':
                case ' ':
                {
                    mWS(false);
                    break;
                }
                case '$':  case 'A':  case 'B':  case 'C':
                case 'D':  case 'E':  case 'F':  case 'G':
                case 'H':  case 'I':  case 'J':  case 'K':
                case 'L':  case 'M':  case 'N':  case 'O':
                case 'P':  case 'Q':  case 'R':  case 'S':
                case 'T':  case 'U':  case 'V':  case 'W':
                case 'X':  case 'Y':  case 'Z':  case '_':
                case 'a':  case 'b':  case 'c':  case 'd':
                case 'e':  case 'f':  case 'g':  case 'h':
                case 'i':  case 'j':  case 'k':  case 'l':
                case 'm':  case 'n':  case 'o':  case 'p':
                case 'q':  case 'r':  case 's':  case 't':
                case 'u':  case 'v':  case 'w':  case 'x':
                case 'y':  case 'z':
                {
                    mXML_ATTRIBUTE(false);
                    break;
                }
                default:
                {
                    goto _loop281_breakloop;
                }
                 }
            }
_loop281_breakloop:         ;
        }    // ( ... )*
        {
            switch ( cached_LA1 )
            {
            case '>':
            {
                match('>');
                {    // ( ... )*
                    for (;;)
                    {
                        if ((cached_LA1=='\t'||cached_LA1=='\n'||cached_LA1=='\u000c'||cached_LA1=='\r'||cached_LA1==' ') && (tokenSet_5_.member(cached_LA2)) && (tokenSet_5_.member(LA(3))) && (tokenSet_5_.member(LA(4))))
                        {
                            mWS(false);
                        }
                        else if ((tokenSet_6_.member(cached_LA1)) && (tokenSet_5_.member(cached_LA2)) && (tokenSet_5_.member(LA(3))) && (tokenSet_5_.member(LA(4)))) {
                            mXML_TEXTNODE(false);
                        }
                        else if ((cached_LA1=='<') && (cached_LA2=='!') && (LA(3)=='-')) {
                            mXML_COMMENT(false);
                        }
                        else if ((cached_LA1=='<') && (cached_LA2=='!') && (LA(3)=='[')) {
                            mXML_CDATA(false);
                        }
                        else if ((cached_LA1=='<') && (tokenSet_2_.member(cached_LA2))) {
                            mXML_LITERAL(false);
                        }
                        else
                        {
                            goto _loop284_breakloop;
                        }
                        
                    }
_loop284_breakloop:                 ;
                }    // ( ... )*
                match("</");
                mIDENT(false);
                match('>');
                break;
            }
            case '/':
            {
                match("/>");
                break;
            }
            default:
            {
                throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
            }
             }
        }
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mWS(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = WS;
        
        { // ( ... )+
            int _cnt354=0;
            for (;;)
            {
                if ((cached_LA1==' ') && (true) && (true) && (true))
                {
                    match(' ');
                }
                else if ((cached_LA1=='\t') && (true) && (true) && (true)) {
                    match('\t');
                }
                else if ((cached_LA1=='\u000c') && (true) && (true) && (true)) {
                    match('\f');
                }
                else if ((cached_LA1=='\n'||cached_LA1=='\r') && (true) && (true) && (true)) {
                    mNL(false);
                }
                else
                {
                    if (_cnt354 >= 1) { goto _loop354_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
                }
                
                _cnt354++;
            }
_loop354_breakloop:         ;
        }    // ( ... )+
        if (0==inputState.guessing)
        {
            _ttype = Token.SKIP;
        }
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    protected void mXML_ATTRIBUTE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = XML_ATTRIBUTE;
        
        mIDENT(false);
        {    // ( ... )*
            for (;;)
            {
                if ((cached_LA1=='\t'||cached_LA1=='\n'||cached_LA1=='\u000c'||cached_LA1=='\r'||cached_LA1==' '))
                {
                    mWS(false);
                }
                else
                {
                    goto _loop287_breakloop;
                }
                
            }
_loop287_breakloop:         ;
        }    // ( ... )*
        mASSIGN(false);
        {    // ( ... )*
            for (;;)
            {
                if ((cached_LA1=='\t'||cached_LA1=='\n'||cached_LA1=='\u000c'||cached_LA1=='\r'||cached_LA1==' '))
                {
                    mWS(false);
                }
                else
                {
                    goto _loop289_breakloop;
                }
                
            }
_loop289_breakloop:         ;
        }    // ( ... )*
        {
            switch ( cached_LA1 )
            {
            case '"':  case '\'':
            {
                mSTRING_LITERAL(false);
                break;
            }
            case '{':
            {
                mXML_BINDING(false);
                break;
            }
            default:
            {
                throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
            }
             }
        }
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    protected void mXML_TEXTNODE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = XML_TEXTNODE;
        
        {
            if ((cached_LA1=='\n'||cached_LA1=='\r'))
            {
                mNL(false);
            }
            else if (((cached_LA1=='/'))&&( LA(2)!='>' )) {
                match('/');
            }
            else if ((tokenSet_7_.member(cached_LA1))) {
                {
                    match(tokenSet_7_);
                }
            }
            else
            {
                throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
            }
            
        }
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    protected void mXML_COMMENT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = XML_COMMENT;
        
        match("<!--");
        {    // ( ... )*
            for (;;)
            {
                if (((cached_LA1=='-') && ((cached_LA2 >= '\u0003' && cached_LA2 <= '\u7ffe')) && ((LA(3) >= '\u0003' && LA(3) <= '\u7ffe')) && ((LA(4) >= '\u0003' && LA(4) <= '\u7ffe')))&&( LA(2)!='-' ))
                {
                    match('-');
                }
                else if ((cached_LA1=='\n'||cached_LA1=='\r')) {
                    mNL(false);
                }
                else if ((tokenSet_8_.member(cached_LA1))) {
                    {
                        match(tokenSet_8_);
                    }
                }
                else
                {
                    goto _loop303_breakloop;
                }
                
            }
_loop303_breakloop:         ;
        }    // ( ... )*
        match("-->");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    protected void mXML_CDATA(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = XML_CDATA;
        
        match("<![CDATA[");
        {    // ( ... )*
            for (;;)
            {
                if (((cached_LA1==']') && ((cached_LA2 >= '\u0003' && cached_LA2 <= '\u7ffe')) && ((LA(3) >= '\u0003' && LA(3) <= '\u7ffe')) && ((LA(4) >= '\u0003' && LA(4) <= '\u7ffe')))&&( LA(2)!=']'))
                {
                    match(']');
                }
                else if ((cached_LA1=='\n'||cached_LA1=='\r')) {
                    mNL(false);
                }
                else if ((tokenSet_9_.member(cached_LA1))) {
                    {
                        match(tokenSet_9_);
                    }
                }
                else
                {
                    goto _loop307_breakloop;
                }
                
            }
_loop307_breakloop:         ;
        }    // ( ... )*
        match("]]>");
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    protected void mXML_BINDING(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = XML_BINDING;
        
        match('{');
        mXML_AS3_EXPRESSION(false);
        match('}');
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    protected void mXML_AS3_EXPRESSION(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = XML_AS3_EXPRESSION;
        
        {
            {    // ( ... )*
                for (;;)
                {
                    if ((tokenSet_10_.member(cached_LA1)))
                    {
                        {
                            match(tokenSet_10_);
                        }
                    }
                    else
                    {
                        goto _loop296_breakloop;
                    }
                    
                }
_loop296_breakloop:             ;
            }    // ( ... )*
        }
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    protected void mNL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = NL;
        
        {
            if ((cached_LA1=='\r') && (cached_LA2=='\n') && (true) && (true))
            {
                match('\r');
                match('\n');
            }
            else if ((cached_LA1=='\r') && (true) && (true) && (true)) {
                match('\r');
            }
            else if ((cached_LA1=='\n')) {
                match('\n');
            }
            else
            {
                throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
            }
            
        }
        if (0==inputState.guessing)
        {
            newline(); _ttype = Token.SKIP;
        }
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mNUMBER(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = NUMBER;
        Token t=null;
        
        if ((cached_LA1=='.') && (cached_LA2=='.') && (LA(3)=='.'))
        {
            match("...");
            if (0==inputState.guessing)
            {
                _ttype = REST;
            }
        }
        else if ((cached_LA1=='.') && (cached_LA2=='.') && (true)) {
            match("..");
            if (0==inputState.guessing)
            {
                _ttype = E4X_DESC;
            }
        }
        else if ((cached_LA1=='.') && (true)) {
            match('.');
            if (0==inputState.guessing)
            {
                _ttype = DOT;
            }
            {
                if (((cached_LA1 >= '0' && cached_LA1 <= '9')))
                {
                    { // ( ... )+
                        int _cnt311=0;
                        for (;;)
                        {
                            if (((cached_LA1 >= '0' && cached_LA1 <= '9')))
                            {
                                matchRange('0','9');
                            }
                            else
                            {
                                if (_cnt311 >= 1) { goto _loop311_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
                            }
                            
                            _cnt311++;
                        }
_loop311_breakloop:                     ;
                    }    // ( ... )+
                    {
                        if ((cached_LA1=='E'||cached_LA1=='e'))
                        {
                            mEXPONENT(false);
                        }
                        else {
                        }
                        
                    }
                    if (0==inputState.guessing)
                    {
                        _ttype = NUMBER;
                    }
                }
                else {
                }
                
            }
        }
        else if (((cached_LA1 >= '0' && cached_LA1 <= '9'))) {
            {
                switch ( cached_LA1 )
                {
                case '0':
                {
                    match('0');
                    {
                        if ((cached_LA1=='X'||cached_LA1=='x'))
                        {
                            {
                                switch ( cached_LA1 )
                                {
                                case 'X':
                                {
                                    match('X');
                                    break;
                                }
                                case 'x':
                                {
                                    match('x');
                                    break;
                                }
                                default:
                                {
                                    throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                                }
                                 }
                            }
                            { // ( ... )+
                                int _cnt317=0;
                                for (;;)
                                {
                                    if ((tokenSet_11_.member(cached_LA1)))
                                    {
                                        mHEX_DIGIT(false);
                                    }
                                    else
                                    {
                                        if (_cnt317 >= 1) { goto _loop317_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
                                    }
                                    
                                    _cnt317++;
                                }
_loop317_breakloop:                             ;
                            }    // ( ... )+
                        }
                        else if (((cached_LA1 >= '0' && cached_LA1 <= '7')) && (true) && (true) && (true)) {
                            { // ( ... )+
                                int _cnt333=0;
                                for (;;)
                                {
                                    if (((cached_LA1 >= '0' && cached_LA1 <= '7')))
                                    {
                                        matchRange('0','7');
                                    }
                                    else
                                    {
                                        if (_cnt333 >= 1) { goto _loop333_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
                                    }
                                    
                                    _cnt333++;
                                }
_loop333_breakloop:                             ;
                            }    // ( ... )+
                        }
                        else {
                            bool synPredMatched325 = false;
                            if (( true ))
                            {
                                int _m325 = mark();
                                synPredMatched325 = true;
                                inputState.guessing++;
                                try {
                                    {
                                        {    // ( ... )*
                                            for (;;)
                                            {
                                                if (((cached_LA1 >= '0' && cached_LA1 <= '9')))
                                                {
                                                    matchRange('0','9');
                                                }
                                                else
                                                {
                                                    goto _loop320_breakloop;
                                                }
                                                
                                            }
_loop320_breakloop:                                         ;
                                        }    // ( ... )*
                                        {
                                            if ((cached_LA1=='.'))
                                            {
                                                match('.');
                                                { // ( ... )+
                                                    int _cnt323=0;
                                                    for (;;)
                                                    {
                                                        if (((cached_LA1 >= '0' && cached_LA1 <= '9')))
                                                        {
                                                            matchRange('0','9');
                                                        }
                                                        else
                                                        {
                                                            if (_cnt323 >= 1) { goto _loop323_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
                                                        }
                                                        
                                                        _cnt323++;
                                                    }
_loop323_breakloop:                                                 ;
                                                }    // ( ... )+
                                            }
                                            else {
                                            }
                                            
                                        }
                                        {
                                            if ((cached_LA1=='E'||cached_LA1=='e'))
                                            {
                                                mEXPONENT(false);
                                            }
                                            else {
                                            }
                                            
                                        }
                                    }
                                }
                                catch (RecognitionException)
                                {
                                    synPredMatched325 = false;
                                }
                                rewind(_m325);
                                inputState.guessing--;
                            }
                            if ( synPredMatched325 )
                            {
                                {    // ( ... )*
                                    for (;;)
                                    {
                                        if (((cached_LA1 >= '0' && cached_LA1 <= '9')))
                                        {
                                            matchRange('0','9');
                                        }
                                        else
                                        {
                                            goto _loop327_breakloop;
                                        }
                                        
                                    }
_loop327_breakloop:                                 ;
                                }    // ( ... )*
                                {
                                    if ((cached_LA1=='.'))
                                    {
                                        match('.');
                                        { // ( ... )+
                                            int _cnt330=0;
                                            for (;;)
                                            {
                                                if (((cached_LA1 >= '0' && cached_LA1 <= '9')))
                                                {
                                                    matchRange('0','9');
                                                }
                                                else
                                                {
                                                    if (_cnt330 >= 1) { goto _loop330_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
                                                }
                                                
                                                _cnt330++;
                                            }
_loop330_breakloop:                                         ;
                                        }    // ( ... )+
                                    }
                                    else {
                                    }
                                    
                                }
                                {
                                    if ((cached_LA1=='E'||cached_LA1=='e'))
                                    {
                                        mEXPONENT(false);
                                    }
                                    else {
                                    }
                                    
                                }
                            }
                            else {
                            }
                            }
                        }
                        break;
                    }
                    case '1':  case '2':  case '3':  case '4':
                    case '5':  case '6':  case '7':  case '8':
                    case '9':
                    {
                        {
                            matchRange('1','9');
                        }
                        {    // ( ... )*
                            for (;;)
                            {
                                if (((cached_LA1 >= '0' && cached_LA1 <= '9')))
                                {
                                    matchRange('0','9');
                                }
                                else
                                {
                                    goto _loop336_breakloop;
                                }
                                
                            }
_loop336_breakloop:                         ;
                        }    // ( ... )*
                        if (0==inputState.guessing)
                        {
                            _ttype = NUMBER;
                        }
                        break;
                    }
                    default:
                    {
                        throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                    }
                     }
                }
            }
            else
            {
                throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
            }
            
            if (_createToken && (null == _token) && (_ttype != Token.SKIP))
            {
                _token = makeToken(_ttype);
                _token.setText(text.ToString(_begin, text.Length-_begin));
            }
            returnToken_ = _token;
        }
        
    protected void mEXPONENT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = EXPONENT;
        
        {
            switch ( cached_LA1 )
            {
            case 'e':
            {
                match('e');
                break;
            }
            case 'E':
            {
                match('E');
                break;
            }
            default:
            {
                throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
            }
             }
        }
        {
            switch ( cached_LA1 )
            {
            case '+':
            {
                match('+');
                break;
            }
            case '-':
            {
                match('-');
                break;
            }
            case '0':  case '1':  case '2':  case '3':
            case '4':  case '5':  case '6':  case '7':
            case '8':  case '9':
            {
                break;
            }
            default:
            {
                throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
            }
             }
        }
        { // ( ... )+
            int _cnt372=0;
            for (;;)
            {
                if (((cached_LA1 >= '0' && cached_LA1 <= '9')))
                {
                    matchRange('0','9');
                }
                else
                {
                    if (_cnt372 >= 1) { goto _loop372_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
                }
                
                _cnt372++;
            }
_loop372_breakloop:         ;
        }    // ( ... )+
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    protected void mHEX_DIGIT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = HEX_DIGIT;
        
        {
            switch ( cached_LA1 )
            {
            case '0':  case '1':  case '2':  case '3':
            case '4':  case '5':  case '6':  case '7':
            case '8':  case '9':
            {
                matchRange('0','9');
                break;
            }
            case 'A':  case 'B':  case 'C':  case 'D':
            case 'E':  case 'F':
            {
                matchRange('A','F');
                break;
            }
            case 'a':  case 'b':  case 'c':  case 'd':
            case 'e':  case 'f':
            {
                matchRange('a','f');
                break;
            }
            default:
            {
                throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
            }
             }
        }
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mREGEX_LITERAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = REGEX_LITERAL;
        
        match('/');
        mREGEX_BODY(false);
        match('/');
        {    // ( ... )*
            for (;;)
            {
                switch ( cached_LA1 )
                {
                case 'a':  case 'b':  case 'c':  case 'd':
                case 'e':  case 'f':  case 'g':  case 'h':
                case 'i':  case 'j':  case 'k':  case 'l':
                case 'm':  case 'n':  case 'o':  case 'p':
                case 'q':  case 'r':  case 's':  case 't':
                case 'u':  case 'v':  case 'w':  case 'x':
                case 'y':  case 'z':
                {
                    matchRange('a','z');
                    break;
                }
                case 'A':  case 'B':  case 'C':  case 'D':
                case 'E':  case 'F':  case 'G':  case 'H':
                case 'I':  case 'J':  case 'K':  case 'L':
                case 'M':  case 'N':  case 'O':  case 'P':
                case 'Q':  case 'R':  case 'S':  case 'T':
                case 'U':  case 'V':  case 'W':  case 'X':
                case 'Y':  case 'Z':
                {
                    matchRange('A','Z');
                    break;
                }
                case '_':
                {
                    match('_');
                    break;
                }
                case '0':  case '1':  case '2':  case '3':
                case '4':  case '5':  case '6':  case '7':
                case '8':  case '9':
                {
                    matchRange('0','9');
                    break;
                }
                case '$':
                {
                    match('$');
                    break;
                }
                default:
                {
                    goto _loop339_breakloop;
                }
                 }
            }
_loop339_breakloop:         ;
        }    // ( ... )*
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    protected void mREGEX_BODY(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = REGEX_BODY;
        
        {
            if ((tokenSet_12_.member(cached_LA1)))
            {
                {
                    {
                        match(tokenSet_12_);
                    }
                }
            }
            else if ((cached_LA1=='\\')) {
                match('\\');
                {
                    {
                        match(tokenSet_1_);
                    }
                }
            }
            else
            {
                throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
            }
            
        }
        {    // ( ... )*
            for (;;)
            {
                if ((tokenSet_13_.member(cached_LA1)))
                {
                    {
                        {
                            match(tokenSet_13_);
                        }
                    }
                }
                else if ((cached_LA1=='\\')) {
                    match('\\');
                    {
                        {
                            match(tokenSet_1_);
                        }
                    }
                }
                else
                {
                    goto _loop351_breakloop;
                }
                
            }
_loop351_breakloop:         ;
        }    // ( ... )*
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mBOM(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = BOM;
        
        switch ( cached_LA1 )
        {
        case '\u00ef':
        {
            match('\u00EF');
            break;
        }
        case '\u00bb':
        {
            match('\u00BB');
            break;
        }
        case '\u00bf':
        {
            match('\u00BF');
            if (0==inputState.guessing)
            {
                _ttype = Token.SKIP;
            }
            break;
        }
        default:
        {
            throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
        }
         }
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mSL_COMMENT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = SL_COMMENT;
        
        match("//");
        {    // ( ... )*
            for (;;)
            {
                if ((tokenSet_1_.member(cached_LA1)))
                {
                    {
                        match(tokenSet_1_);
                    }
                }
                else
                {
                    goto _loop361_breakloop;
                }
                
            }
_loop361_breakloop:         ;
        }    // ( ... )*
        {
            switch ( cached_LA1 )
            {
            case '\n':
            {
                match('\n');
                break;
            }
            case '\r':
            {
                match('\r');
                {
                    if ((cached_LA1=='\n'))
                    {
                        match('\n');
                    }
                    else {
                    }
                    
                }
                break;
            }
            default:
                {
                }
            break; }
        }
        if (0==inputState.guessing)
        {
            _ttype = Token.SKIP; newline();
        }
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    public void mML_COMMENT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
        int _ttype; IToken _token=null; int _begin=text.Length;
        _ttype = ML_COMMENT;
        
        match("/*");
        {    // ( ... )*
            for (;;)
            {
                if (((cached_LA1=='*') && ((cached_LA2 >= '\u0003' && cached_LA2 <= '\u7ffe')) && ((LA(3) >= '\u0003' && LA(3) <= '\u7ffe')))&&( LA(2)!='/' ))
                {
                    match('*');
                }
                else if ((cached_LA1=='\n'||cached_LA1=='\r')) {
                    mNL(false);
                }
                else if ((tokenSet_14_.member(cached_LA1))) {
                    {
                        match(tokenSet_14_);
                    }
                }
                else
                {
                    goto _loop367_breakloop;
                }
                
            }
_loop367_breakloop:         ;
        }    // ( ... )*
        match("*/");
        if (0==inputState.guessing)
        {
            _ttype = Token.SKIP;
        }
        if (_createToken && (null == _token) && (_ttype != Token.SKIP))
        {
            _token = makeToken(_ttype);
            _token.setText(text.ToString(_begin, text.Length-_begin));
        }
        returnToken_ = _token;
    }
    
    
    private static long[] mk_tokenSet_0_()
    {
        long[] data = new long[1024];
        data[0]=-145135534875656L;
        for (int i = 1; i<=510; i++) { data[i]=-1L; }
        data[511]=9223372036854775807L;
        for (int i = 512; i<=1023; i++) { data[i]=0L; }
        return data;
    }
    public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
    private static long[] mk_tokenSet_1_()
    {
        long[] data = new long[1024];
        data[0]=-9224L;
        for (int i = 1; i<=510; i++) { data[i]=-1L; }
        data[511]=9223372036854775807L;
        for (int i = 512; i<=1023; i++) { data[i]=0L; }
        return data;
    }
    public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
    private static long[] mk_tokenSet_2_()
    {
        long[] data = new long[513];
        data[0]=68719476736L;
        data[1]=576460745995190270L;
        for (int i = 2; i<=512; i++) { data[i]=0L; }
        return data;
    }
    public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
    private static long[] mk_tokenSet_3_()
    {
        long[] data = new long[1024];
        data[0]=-17179878408L;
        data[1]=-268435457L;
        for (int i = 2; i<=510; i++) { data[i]=-1L; }
        data[511]=9223372036854775807L;
        for (int i = 512; i<=1023; i++) { data[i]=0L; }
        return data;
    }
    public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
    private static long[] mk_tokenSet_4_()
    {
        long[] data = new long[1024];
        data[0]=-549755823112L;
        data[1]=-268435457L;
        for (int i = 2; i<=510; i++) { data[i]=-1L; }
        data[511]=9223372036854775807L;
        for (int i = 512; i<=1023; i++) { data[i]=0L; }
        return data;
    }
    public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
    private static long[] mk_tokenSet_5_()
    {
        long[] data = new long[1024];
        data[0]=-8L;
        data[1]=-576460752303423489L;
        for (int i = 2; i<=510; i++) { data[i]=-1L; }
        data[511]=9223372036854775807L;
        for (int i = 512; i<=1023; i++) { data[i]=0L; }
        return data;
    }
    public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
    private static long[] mk_tokenSet_6_()
    {
        long[] data = new long[1024];
        data[0]=-1152921504606846984L;
        data[1]=-576460752303423489L;
        for (int i = 2; i<=510; i++) { data[i]=-1L; }
        data[511]=9223372036854775807L;
        for (int i = 512; i<=1023; i++) { data[i]=0L; }
        return data;
    }
    public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
    private static long[] mk_tokenSet_7_()
    {
        long[] data = new long[1024];
        data[0]=-1153062242095211528L;
        data[1]=-576460752303423489L;
        for (int i = 2; i<=510; i++) { data[i]=-1L; }
        data[511]=9223372036854775807L;
        for (int i = 512; i<=1023; i++) { data[i]=0L; }
        return data;
    }
    public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
    private static long[] mk_tokenSet_8_()
    {
        long[] data = new long[1024];
        data[0]=-35184372098056L;
        for (int i = 1; i<=510; i++) { data[i]=-1L; }
        data[511]=9223372036854775807L;
        for (int i = 512; i<=1023; i++) { data[i]=0L; }
        return data;
    }
    public static readonly BitSet tokenSet_8_ = new BitSet(mk_tokenSet_8_());
    private static long[] mk_tokenSet_9_()
    {
        long[] data = new long[1024];
        data[0]=-9224L;
        data[1]=-536870913L;
        for (int i = 2; i<=510; i++) { data[i]=-1L; }
        data[511]=9223372036854775807L;
        for (int i = 512; i<=1023; i++) { data[i]=0L; }
        return data;
    }
    public static readonly BitSet tokenSet_9_ = new BitSet(mk_tokenSet_9_());
    private static long[] mk_tokenSet_10_()
    {
        long[] data = new long[1024];
        data[0]=-8L;
        data[1]=-2882303761517117441L;
        for (int i = 2; i<=510; i++) { data[i]=-1L; }
        data[511]=9223372036854775807L;
        for (int i = 512; i<=1023; i++) { data[i]=0L; }
        return data;
    }
    public static readonly BitSet tokenSet_10_ = new BitSet(mk_tokenSet_10_());
    private static long[] mk_tokenSet_11_()
    {
        long[] data = new long[513];
        data[0]=287948901175001088L;
        data[1]=541165879422L;
        for (int i = 2; i<=512; i++) { data[i]=0L; }
        return data;
    }
    public static readonly BitSet tokenSet_11_ = new BitSet(mk_tokenSet_11_());
    private static long[] mk_tokenSet_12_()
    {
        long[] data = new long[1024];
        data[0]=-145135534875656L;
        data[1]=-268435457L;
        for (int i = 2; i<=510; i++) { data[i]=-1L; }
        data[511]=9223372036854775807L;
        for (int i = 512; i<=1023; i++) { data[i]=0L; }
        return data;
    }
    public static readonly BitSet tokenSet_12_ = new BitSet(mk_tokenSet_12_());
    private static long[] mk_tokenSet_13_()
    {
        long[] data = new long[1024];
        data[0]=-140737488364552L;
        data[1]=-268435457L;
        for (int i = 2; i<=510; i++) { data[i]=-1L; }
        data[511]=9223372036854775807L;
        for (int i = 512; i<=1023; i++) { data[i]=0L; }
        return data;
    }
    public static readonly BitSet tokenSet_13_ = new BitSet(mk_tokenSet_13_());
    private static long[] mk_tokenSet_14_()
    {
        long[] data = new long[1024];
        data[0]=-4398046520328L;
        for (int i = 1; i<=510; i++) { data[i]=-1L; }
        data[511]=9223372036854775807L;
        for (int i = 512; i<=1023; i++) { data[i]=0L; }
        return data;
    }
    public static readonly BitSet tokenSet_14_ = new BitSet(mk_tokenSet_14_());
    
}
}
