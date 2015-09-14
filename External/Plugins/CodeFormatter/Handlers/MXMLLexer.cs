// $ANTLR 3.1.1 MXMLLexer.g3 2015-02-17 22:19:22

using System.Collections.Generic;
using Antlr.Runtime;
using Stack         = Antlr.Runtime.Collections.StackList;


/** XML parser by Oliver Zeigermann October 10, 2005; posted to Antlr examples */
public class MXMLLexer : Lexer {
    public const int LETTER = 37;
    public const int SYSTEM_tag = 13;
    public const int DECL_STOP = 20;
    public const int PUBLIC_tag = 15;
    public const int VALUE_int = 35;
    public const int EMPTY_TAG_OPEN = 4;
    public const int VALUE = 14;
    public const int COMMENT = 8;
    public const int PCDATA = 25;
    public const int DOCTYPE_tag = 11;
    public const int EMPTY_ELEMENT = 28;
    public const int DOCUMENT = 10;
    public const int DOCTYPE = 6;
    public const int WS = 9;
    public const int INTERNAL_DTD = 16;
    public const int EOF = -1;
    public const int ATTRIBUTE = 19;
    public const int EOL = 24;
    public const int END_TAG = 27;
    public const int END_TAG_OPEN = 33;
    public const int COMMENT_int = 34;
    public const int GENERIC_ID = 12;
    public const int EOL_HELPER = 39;
    public const int EQ = 32;
    public const int GENERIC_ID_int = 36;
    public const int TAG_OPEN = 29;
    public const int ELEMENT = 7;
    public const int EMPTYTAG_CLOSE = 30;
    public const int XML = 22;
    public const int OTHERWS = 38;
    public const int TAG_CLOSE = 17;
    public const int XMLDECL = 5;
    public const int PI = 21;
    public const int DECL_START = 18;
    public const int START_TAG = 23;
    public const int EQ_int = 31;
    public const int CDATA = 26;

       private List<CommonToken> mRawTokens=new List<CommonToken>();
       int lastLine=1;
       int lastCharPos=0;
       public void AddToken( CommonToken t, int type, int channel)
       {
          ((CommonToken)t).Type = type;
          ((CommonToken)t).Channel = channel;
          t.Line = lastLine;
          lastLine=input.Line;
          t.CharPositionInLine = lastCharPos;
          lastCharPos=input.CharPositionInLine;
          mRawTokens.Add((CommonToken)t);
       }
       public List<CommonToken> GetTokens()
       {
          return mRawTokens;
       }
       
       override public void Reset()
       {
          base.Reset(); // reset all recognizer state variables
          if (input is ANTLRStringStream)
          {
         ((ANTLRStringStream)input).Reset();
          }
       }

       


    // delegates
    // delegators

    public MXMLLexer() 
    {
        InitializeCyclicDFAs();
    }
    public MXMLLexer(ICharStream input)
        : this(input, null) {
    }
    public MXMLLexer(ICharStream input, RecognizerSharedState state)
        : base(input, state) {
        InitializeCyclicDFAs(); 

    }
    
    override public string GrammarFileName
    {
        get { return "MXMLLexer.g3";} 
    }

    // $ANTLR start "DOCUMENT"
    public void mDOCUMENT() // throws RecognitionException [2]
    {
            try
            {
            int _type = DOCUMENT;
        int _channel = DEFAULT_TOKEN_CHANNEL;
            // MXMLLexer.g3:54:7: ( ( XMLDECL | DOCTYPE | ELEMENT | COMMENT | WS )* )
            // MXMLLexer.g3:55:7: ( XMLDECL | DOCTYPE | ELEMENT | COMMENT | WS )*
            {
                // MXMLLexer.g3:55:7: ( XMLDECL | DOCTYPE | ELEMENT | COMMENT | WS )*
                do 
                {
                    int alt1 = 6;
                    int LA1_0 = input.LA(1);

                    if ( (LA1_0 == '<') )
                    {
                        switch ( input.LA(2) ) 
                        {
                        case '?':
                            {
                            alt1 = 1;
                            }
                            break;
                        case '!':
                            {
                            int LA1_5 = input.LA(3);

                            if ( (LA1_5 == 'D') )
                            {
                                alt1 = 2;
                            }
                            else if ( (LA1_5 == '-') )
                            {
                                alt1 = 4;
                            }


                            }
                            break;
                        case '\t':
                        case '\n':
                        case '\r':
                        case ' ':
                        case ':':
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
                            alt1 = 3;
                            }
                            break;

                        }

                    }
                    else if ( ((LA1_0 >= '\t' && LA1_0 <= '\n') || LA1_0 == '\r' || LA1_0 == ' ') )
                    {
                        alt1 = 5;
                    }


                    switch (alt1) 
                    {
                        case 1 :
                            // MXMLLexer.g3:55:9: XMLDECL
                            {
                                mXMLDECL(); 

                            }
                            break;
                        case 2 :
                            // MXMLLexer.g3:55:19: DOCTYPE
                            {
                                mDOCTYPE(); 

                            }
                            break;
                        case 3 :
                            // MXMLLexer.g3:55:29: ELEMENT
                            {
                                mELEMENT(); 

                            }
                            break;
                        case 4 :
                            // MXMLLexer.g3:55:39: COMMENT
                            {
                                mCOMMENT(); 

                            }
                            break;
                        case 5 :
                            // MXMLLexer.g3:55:49: WS
                            {
                                mWS(); 

                            }
                            break;

                        default:
                            goto loop1;
                    }
                } while (true);

                loop1:
                    ;   // Stops C# compiler whining that label 'loop1' has no statements


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
        {
        }
    }
    // $ANTLR end "DOCUMENT"

    // $ANTLR start "DOCTYPE"
    public void mDOCTYPE() // throws RecognitionException [2]
    {
            try
            {
            IToken dt = null;
            IToken rootElementName = null;
            IToken sys = null;
            IToken sys1 = null;
            IToken pub = null;
            IToken sys2 = null;
            IToken dtd = null;
            IToken close = null;

            // MXMLLexer.g3:60:5: (dt= DOCTYPE_tag WS rootElementName= GENERIC_ID WS ( (sys= SYSTEM_tag WS sys1= VALUE | pub= PUBLIC_tag WS pub= VALUE WS sys2= VALUE ) ( WS )? )? (dtd= INTERNAL_DTD )? close= TAG_CLOSE )
            // MXMLLexer.g3:61:9: dt= DOCTYPE_tag WS rootElementName= GENERIC_ID WS ( (sys= SYSTEM_tag WS sys1= VALUE | pub= PUBLIC_tag WS pub= VALUE WS sys2= VALUE ) ( WS )? )? (dtd= INTERNAL_DTD )? close= TAG_CLOSE
            {
                int dtStart120 = CharIndex;
                mDOCTYPE_tag(); 
                dt = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, dtStart120, CharIndex-1);
                AddToken( (CommonToken)dt, DOCTYPE_tag, 0);
                mWS(); 
                int rootElementNameStart128 = CharIndex;
                mGENERIC_ID(); 
                rootElementName = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, rootElementNameStart128, CharIndex-1);
                 
                mWS(); 
                // MXMLLexer.g3:64:9: ( (sys= SYSTEM_tag WS sys1= VALUE | pub= PUBLIC_tag WS pub= VALUE WS sys2= VALUE ) ( WS )? )?
                int alt4 = 2;
                int LA4_0 = input.LA(1);

                if ( (LA4_0 == 'P' || LA4_0 == 'S') )
                {
                    alt4 = 1;
                }
                switch (alt4) 
                {
                    case 1 :
                        // MXMLLexer.g3:65:13: (sys= SYSTEM_tag WS sys1= VALUE | pub= PUBLIC_tag WS pub= VALUE WS sys2= VALUE ) ( WS )?
                        {
                            // MXMLLexer.g3:65:13: (sys= SYSTEM_tag WS sys1= VALUE | pub= PUBLIC_tag WS pub= VALUE WS sys2= VALUE )
                            int alt2 = 2;
                            int LA2_0 = input.LA(1);

                            if ( (LA2_0 == 'S') )
                            {
                                alt2 = 1;
                            }
                            else if ( (LA2_0 == 'P') )
                            {
                                alt2 = 2;
                            }
                            else 
                            {
                                NoViableAltException nvae_d2s0 =
                                    new NoViableAltException("", 2, 0, input);

                                throw nvae_d2s0;
                            }
                            switch (alt2) 
                            {
                                case 1 :
                                    // MXMLLexer.g3:65:15: sys= SYSTEM_tag WS sys1= VALUE
                                    {
                                        int sysStart181 = CharIndex;
                                        mSYSTEM_tag(); 
                                        sys = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, sysStart181, CharIndex-1);
                                        AddToken( (CommonToken)sys, SYSTEM_tag, 0);
                                        mWS(); 
                                        int sys1Start189 = CharIndex;
                                        mVALUE(); 
                                        sys1 = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, sys1Start189, CharIndex-1);

                                    }
                                    break;
                                case 2 :
                                    // MXMLLexer.g3:66:15: pub= PUBLIC_tag WS pub= VALUE WS sys2= VALUE
                                    {
                                        int pubStart207 = CharIndex;
                                        mPUBLIC_tag(); 
                                        pub = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, pubStart207, CharIndex-1);
                                        AddToken( (CommonToken)pub, PUBLIC_tag, 0);
                                        mWS(); 
                                        int pubStart215 = CharIndex;
                                        mVALUE(); 
                                        pub = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, pubStart215, CharIndex-1);
                                        mWS(); 
                                        int sys2Start221 = CharIndex;
                                        mVALUE(); 
                                        sys2 = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, sys2Start221, CharIndex-1);

                                    }
                                    break;

                            }

                            // MXMLLexer.g3:68:13: ( WS )?
                            int alt3 = 2;
                            int LA3_0 = input.LA(1);

                            if ( ((LA3_0 >= '\t' && LA3_0 <= '\n') || LA3_0 == '\r' || LA3_0 == ' ') )
                            {
                                alt3 = 1;
                            }
                            switch (alt3) 
                            {
                                case 1 :
                                    // MXMLLexer.g3:68:15: WS
                                    {
                                        mWS(); 

                                    }
                                    break;

                            }


                        }
                        break;

                }

                // MXMLLexer.g3:70:9: (dtd= INTERNAL_DTD )?
                int alt5 = 2;
                int LA5_0 = input.LA(1);

                if ( (LA5_0 == '[') )
                {
                    alt5 = 1;
                }
                switch (alt5) 
                {
                    case 1 :
                        // MXMLLexer.g3:70:11: dtd= INTERNAL_DTD
                        {
                            int dtdStart279 = CharIndex;
                            mINTERNAL_DTD(); 
                            dtd = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, dtdStart279, CharIndex-1);

                        }
                        break;

                }

                int closeStart298 = CharIndex;
                mTAG_CLOSE(); 
                close = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, closeStart298, CharIndex-1);
                AddToken( (CommonToken)close, TAG_CLOSE, 0);

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "DOCTYPE"

    // $ANTLR start "SYSTEM_tag"
    public void mSYSTEM_tag() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:77:2: ( 'SYSTEM' )
            // MXMLLexer.g3:77:4: 'SYSTEM'
            {
                Match("SYSTEM"); 


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "SYSTEM_tag"

    // $ANTLR start "PUBLIC_tag"
    public void mPUBLIC_tag() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:80:2: ( 'PUBLIC' )
            // MXMLLexer.g3:80:4: 'PUBLIC'
            {
                Match("PUBLIC"); 


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "PUBLIC_tag"

    // $ANTLR start "DOCTYPE_tag"
    public void mDOCTYPE_tag() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:83:2: ( '<!DOCTYPE' )
            // MXMLLexer.g3:83:4: '<!DOCTYPE'
            {
                Match("<!DOCTYPE"); 


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "DOCTYPE_tag"

    // $ANTLR start "INTERNAL_DTD"
    public void mINTERNAL_DTD() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:85:23: ( '[' ( options {greedy=false; } : . )* ']' )
            // MXMLLexer.g3:85:25: '[' ( options {greedy=false; } : . )* ']'
            {
                Match('['); 
                // MXMLLexer.g3:85:29: ( options {greedy=false; } : . )*
                do 
                {
                    int alt6 = 2;
                    int LA6_0 = input.LA(1);

                    if ( (LA6_0 == ']') )
                    {
                        alt6 = 2;
                    }
                    else if ( ((LA6_0 >= '\u0000' && LA6_0 <= '\\') || (LA6_0 >= '^' && LA6_0 <= '\uFFFF')) )
                    {
                        alt6 = 1;
                    }


                    switch (alt6) 
                    {
                        case 1 :
                            // MXMLLexer.g3:85:56: .
                            {
                                MatchAny(); 

                            }
                            break;

                        default:
                            goto loop6;
                    }
                } while (true);

                loop6:
                    ;   // Stops C# compiler whining that label 'loop6' has no statements

                Match(']'); 

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "INTERNAL_DTD"

    // $ANTLR start "PI"
    public void mPI() // throws RecognitionException [2]
    {
            try
            {
            IToken ds = null;
            IToken target = null;
            IToken de = null;

            // MXMLLexer.g3:87:13: (ds= DECL_START target= GENERIC_ID ( WS )? ( ATTRIBUTE ( WS )? )* de= DECL_STOP )
            // MXMLLexer.g3:88:9: ds= DECL_START target= GENERIC_ID ( WS )? ( ATTRIBUTE ( WS )? )* de= DECL_STOP
            {
                int dsStart389 = CharIndex;
                mDECL_START(); 
                ds = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, dsStart389, CharIndex-1);
                int targetStart393 = CharIndex;
                mGENERIC_ID(); 
                target = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, targetStart393, CharIndex-1);
                // MXMLLexer.g3:88:41: ( WS )?
                int alt7 = 2;
                int LA7_0 = input.LA(1);

                if ( ((LA7_0 >= '\t' && LA7_0 <= '\n') || LA7_0 == '\r' || LA7_0 == ' ') )
                {
                    alt7 = 1;
                }
                switch (alt7) 
                {
                    case 1 :
                        // MXMLLexer.g3:88:41: WS
                        {
                            mWS(); 

                        }
                        break;

                }

                // MXMLLexer.g3:89:9: ( ATTRIBUTE ( WS )? )*
                do 
                {
                    int alt9 = 2;
                    int LA9_0 = input.LA(1);

                    if ( (LA9_0 == ':' || (LA9_0 >= 'A' && LA9_0 <= 'Z') || LA9_0 == '_' || (LA9_0 >= 'a' && LA9_0 <= 'z')) )
                    {
                        alt9 = 1;
                    }


                    switch (alt9) 
                    {
                        case 1 :
                            // MXMLLexer.g3:89:11: ATTRIBUTE ( WS )?
                            {
                                mATTRIBUTE(); 
                                // MXMLLexer.g3:89:21: ( WS )?
                                int alt8 = 2;
                                int LA8_0 = input.LA(1);

                                if ( ((LA8_0 >= '\t' && LA8_0 <= '\n') || LA8_0 == '\r' || LA8_0 == ' ') )
                                {
                                    alt8 = 1;
                                }
                                switch (alt8) 
                                {
                                    case 1 :
                                        // MXMLLexer.g3:89:21: WS
                                        {
                                            mWS(); 

                                        }
                                        break;

                                }


                            }
                            break;

                        default:
                            goto loop9;
                    }
                } while (true);

                loop9:
                    ;   // Stops C# compiler whining that label 'loop9' has no statements

                int deStart420 = CharIndex;
                mDECL_STOP(); 
                de = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, deStart420, CharIndex-1);
                AddToken( (CommonToken)de, DECL_STOP, 0);

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "PI"

    // $ANTLR start "XMLDECL"
    public void mXMLDECL() // throws RecognitionException [2]
    {
            try
            {
            IToken ds = null;
            IToken xml = null;
            IToken de = null;

            // MXMLLexer.g3:92:18: (ds= DECL_START xml= XML ( WS )? ( ATTRIBUTE ( WS )? )* de= DECL_STOP )
            // MXMLLexer.g3:93:9: ds= DECL_START xml= XML ( WS )? ( ATTRIBUTE ( WS )? )* de= DECL_STOP
            {
                int dsStart444 = CharIndex;
                mDECL_START(); 
                ds = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, dsStart444, CharIndex-1);
                AddToken( (CommonToken)ds, DECL_START, 0);
                int xmlStart450 = CharIndex;
                mXML(); 
                xml = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, xmlStart450, CharIndex-1);
                AddToken( (CommonToken)xml, XML, 0);
                // MXMLLexer.g3:93:117: ( WS )?
                int alt10 = 2;
                int LA10_0 = input.LA(1);

                if ( ((LA10_0 >= '\t' && LA10_0 <= '\n') || LA10_0 == '\r' || LA10_0 == ' ') )
                {
                    alt10 = 1;
                }
                switch (alt10) 
                {
                    case 1 :
                        // MXMLLexer.g3:93:117: WS
                        {
                            mWS(); 

                        }
                        break;

                }

                // MXMLLexer.g3:94:9: ( ATTRIBUTE ( WS )? )*
                do 
                {
                    int alt12 = 2;
                    int LA12_0 = input.LA(1);

                    if ( (LA12_0 == ':' || (LA12_0 >= 'A' && LA12_0 <= 'Z') || LA12_0 == '_' || (LA12_0 >= 'a' && LA12_0 <= 'z')) )
                    {
                        alt12 = 1;
                    }


                    switch (alt12) 
                    {
                        case 1 :
                            // MXMLLexer.g3:94:11: ATTRIBUTE ( WS )?
                            {
                                mATTRIBUTE(); 
                                // MXMLLexer.g3:94:21: ( WS )?
                                int alt11 = 2;
                                int LA11_0 = input.LA(1);

                                if ( ((LA11_0 >= '\t' && LA11_0 <= '\n') || LA11_0 == '\r' || LA11_0 == ' ') )
                                {
                                    alt11 = 1;
                                }
                                switch (alt11) 
                                {
                                    case 1 :
                                        // MXMLLexer.g3:94:21: WS
                                        {
                                            mWS(); 

                                        }
                                        break;

                                }


                            }
                            break;

                        default:
                            goto loop12;
                    }
                } while (true);

                loop12:
                    ;   // Stops C# compiler whining that label 'loop12' has no statements

                int deStart479 = CharIndex;
                mDECL_STOP(); 
                de = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, deStart479, CharIndex-1);
                AddToken( (CommonToken)de, DECL_STOP, 0);

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "XMLDECL"

    // $ANTLR start "XML"
    public void mXML() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:98:2: ( ( 'x' | 'X' ) ( 'm' | 'M' ) ( 'l' | 'L' ) )
            // MXMLLexer.g3:98:4: ( 'x' | 'X' ) ( 'm' | 'M' ) ( 'l' | 'L' )
            {
                if ( input.LA(1) == 'X' || input.LA(1) == 'x' ) 
                {
                    input.Consume();

                }
                else 
                {
                    MismatchedSetException mse = new MismatchedSetException(null,input);
                    Recover(mse);
                    throw mse;}

                if ( input.LA(1) == 'M' || input.LA(1) == 'm' ) 
                {
                    input.Consume();

                }
                else 
                {
                    MismatchedSetException mse = new MismatchedSetException(null,input);
                    Recover(mse);
                    throw mse;}

                if ( input.LA(1) == 'L' || input.LA(1) == 'l' ) 
                {
                    input.Consume();

                }
                else 
                {
                    MismatchedSetException mse = new MismatchedSetException(null,input);
                    Recover(mse);
                    throw mse;}


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "XML"

    // $ANTLR start "DECL_START"
    public void mDECL_START() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:100:3: ( '<?' )
            // MXMLLexer.g3:100:5: '<?'
            {
                Match("<?"); 


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "DECL_START"

    // $ANTLR start "DECL_STOP"
    public void mDECL_STOP() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:104:3: ( '?>' )
            // MXMLLexer.g3:104:5: '?>'
            {
                Match("?>"); 


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "DECL_STOP"

    // $ANTLR start "ELEMENT"
    public void mELEMENT() // throws RecognitionException [2]
    {
            try
            {
            IToken t = null;
            IToken pi = null;

            // MXMLLexer.g3:108:5: ( ( START_TAG ( ELEMENT | EOL | t= PCDATA | t= CDATA | t= COMMENT | pi= PI )* END_TAG | EMPTY_ELEMENT ) )
            // MXMLLexer.g3:108:7: ( START_TAG ( ELEMENT | EOL | t= PCDATA | t= CDATA | t= COMMENT | pi= PI )* END_TAG | EMPTY_ELEMENT )
            {
                // MXMLLexer.g3:108:7: ( START_TAG ( ELEMENT | EOL | t= PCDATA | t= CDATA | t= COMMENT | pi= PI )* END_TAG | EMPTY_ELEMENT )
                int alt14 = 2;
                alt14 = dfa14.Predict(input);
                switch (alt14) 
                {
                    case 1 :
                        // MXMLLexer.g3:108:9: START_TAG ( ELEMENT | EOL | t= PCDATA | t= CDATA | t= COMMENT | pi= PI )* END_TAG
                        {
                            mSTART_TAG(); 
                            // MXMLLexer.g3:109:13: ( ELEMENT | EOL | t= PCDATA | t= CDATA | t= COMMENT | pi= PI )*
                            do 
                            {
                                int alt13 = 7;
                                alt13 = dfa13.Predict(input);
                                switch (alt13) 
                                {
                                    case 1 :
                                        // MXMLLexer.g3:109:14: ELEMENT
                                        {
                                            mELEMENT(); 

                                        }
                                        break;
                                    case 2 :
                                        // MXMLLexer.g3:110:15: EOL
                                        {
                                            mEOL(); 

                                        }
                                        break;
                                    case 3 :
                                        // MXMLLexer.g3:111:15: t= PCDATA
                                        {
                                            int tStart607 = CharIndex;
                                            mPCDATA(); 
                                            t = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, tStart607, CharIndex-1);
                                             AddToken( (CommonToken)t, PCDATA, 0);

                                        }
                                        break;
                                    case 4 :
                                        // MXMLLexer.g3:113:15: t= CDATA
                                        {
                                            int tStart643 = CharIndex;
                                            mCDATA(); 
                                            t = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, tStart643, CharIndex-1);
                                             AddToken( (CommonToken)t, CDATA, 0);

                                        }
                                        break;
                                    case 5 :
                                        // MXMLLexer.g3:115:15: t= COMMENT
                                        {
                                            int tStart679 = CharIndex;
                                            mCOMMENT(); 
                                            t = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, tStart679, CharIndex-1);

                                        }
                                        break;
                                    case 6 :
                                        // MXMLLexer.g3:116:15: pi= PI
                                        {
                                            int piStart697 = CharIndex;
                                            mPI(); 
                                            pi = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, piStart697, CharIndex-1);

                                        }
                                        break;

                                    default:
                                        goto loop13;
                                }
                            } while (true);

                            loop13:
                                ;   // Stops C# compiler whining that label 'loop13' has no statements

                            mEND_TAG(); 

                        }
                        break;
                    case 2 :
                        // MXMLLexer.g3:119:11: EMPTY_ELEMENT
                        {
                            mEMPTY_ELEMENT(); 

                        }
                        break;

                }


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "ELEMENT"

    // $ANTLR start "START_TAG"
    public void mSTART_TAG() // throws RecognitionException [2]
    {
            try
            {
            IToken open = null;
            IToken name = null;
            IToken close = null;

            // MXMLLexer.g3:124:5: (open= TAG_OPEN ( WS )? name= GENERIC_ID ( WS )? ( ATTRIBUTE ( WS )? )* close= TAG_CLOSE )
            // MXMLLexer.g3:124:7: open= TAG_OPEN ( WS )? name= GENERIC_ID ( WS )? ( ATTRIBUTE ( WS )? )* close= TAG_CLOSE
            {
                int openStart770 = CharIndex;
                mTAG_OPEN(); 
                open = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, openStart770, CharIndex-1);
                AddToken( (CommonToken)open, TAG_OPEN, 0);
                // MXMLLexer.g3:124:67: ( WS )?
                int alt15 = 2;
                int LA15_0 = input.LA(1);

                if ( ((LA15_0 >= '\t' && LA15_0 <= '\n') || LA15_0 == '\r' || LA15_0 == ' ') )
                {
                    alt15 = 1;
                }
                switch (alt15) 
                {
                    case 1 :
                        // MXMLLexer.g3:124:67: WS
                        {
                            mWS(); 

                        }
                        break;

                }

                int nameStart779 = CharIndex;
                mGENERIC_ID(); 
                name = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, nameStart779, CharIndex-1);
                // MXMLLexer.g3:124:87: ( WS )?
                int alt16 = 2;
                int LA16_0 = input.LA(1);

                if ( ((LA16_0 >= '\t' && LA16_0 <= '\n') || LA16_0 == '\r' || LA16_0 == ' ') )
                {
                    alt16 = 1;
                }
                switch (alt16) 
                {
                    case 1 :
                        // MXMLLexer.g3:124:87: WS
                        {
                            mWS(); 

                        }
                        break;

                }

                // MXMLLexer.g3:125:9: ( ATTRIBUTE ( WS )? )*
                do 
                {
                    int alt18 = 2;
                    int LA18_0 = input.LA(1);

                    if ( (LA18_0 == ':' || (LA18_0 >= 'A' && LA18_0 <= 'Z') || LA18_0 == '_' || (LA18_0 >= 'a' && LA18_0 <= 'z')) )
                    {
                        alt18 = 1;
                    }


                    switch (alt18) 
                    {
                        case 1 :
                            // MXMLLexer.g3:125:11: ATTRIBUTE ( WS )?
                            {
                                mATTRIBUTE(); 
                                // MXMLLexer.g3:125:21: ( WS )?
                                int alt17 = 2;
                                int LA17_0 = input.LA(1);

                                if ( ((LA17_0 >= '\t' && LA17_0 <= '\n') || LA17_0 == '\r' || LA17_0 == ' ') )
                                {
                                    alt17 = 1;
                                }
                                switch (alt17) 
                                {
                                    case 1 :
                                        // MXMLLexer.g3:125:21: WS
                                        {
                                            mWS(); 

                                        }
                                        break;

                                }


                            }
                            break;

                        default:
                            goto loop18;
                    }
                } while (true);

                loop18:
                    ;   // Stops C# compiler whining that label 'loop18' has no statements

                int closeStart804 = CharIndex;
                mTAG_CLOSE(); 
                close = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, closeStart804, CharIndex-1);
                AddToken( (CommonToken)close, TAG_CLOSE, 0);

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "START_TAG"

    // $ANTLR start "EMPTY_ELEMENT"
    public void mEMPTY_ELEMENT() // throws RecognitionException [2]
    {
            try
            {
            IToken open = null;
            IToken name = null;
            IToken close = null;

            // MXMLLexer.g3:129:5: (open= TAG_OPEN ( WS )? name= GENERIC_ID ( WS )? ( ATTRIBUTE ( WS )? )* close= EMPTYTAG_CLOSE )
            // MXMLLexer.g3:129:7: open= TAG_OPEN ( WS )? name= GENERIC_ID ( WS )? ( ATTRIBUTE ( WS )? )* close= EMPTYTAG_CLOSE
            {
                int openStart828 = CharIndex;
                mTAG_OPEN(); 
                open = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, openStart828, CharIndex-1);
                AddToken( (CommonToken)open, EMPTY_TAG_OPEN, 0);
                // MXMLLexer.g3:129:73: ( WS )?
                int alt19 = 2;
                int LA19_0 = input.LA(1);

                if ( ((LA19_0 >= '\t' && LA19_0 <= '\n') || LA19_0 == '\r' || LA19_0 == ' ') )
                {
                    alt19 = 1;
                }
                switch (alt19) 
                {
                    case 1 :
                        // MXMLLexer.g3:129:73: WS
                        {
                            mWS(); 

                        }
                        break;

                }

                int nameStart837 = CharIndex;
                mGENERIC_ID(); 
                name = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, nameStart837, CharIndex-1);
                // MXMLLexer.g3:129:93: ( WS )?
                int alt20 = 2;
                int LA20_0 = input.LA(1);

                if ( ((LA20_0 >= '\t' && LA20_0 <= '\n') || LA20_0 == '\r' || LA20_0 == ' ') )
                {
                    alt20 = 1;
                }
                switch (alt20) 
                {
                    case 1 :
                        // MXMLLexer.g3:129:93: WS
                        {
                            mWS(); 

                        }
                        break;

                }

                // MXMLLexer.g3:130:9: ( ATTRIBUTE ( WS )? )*
                do 
                {
                    int alt22 = 2;
                    int LA22_0 = input.LA(1);

                    if ( (LA22_0 == ':' || (LA22_0 >= 'A' && LA22_0 <= 'Z') || LA22_0 == '_' || (LA22_0 >= 'a' && LA22_0 <= 'z')) )
                    {
                        alt22 = 1;
                    }


                    switch (alt22) 
                    {
                        case 1 :
                            // MXMLLexer.g3:130:11: ATTRIBUTE ( WS )?
                            {
                                mATTRIBUTE(); 
                                // MXMLLexer.g3:130:21: ( WS )?
                                int alt21 = 2;
                                int LA21_0 = input.LA(1);

                                if ( ((LA21_0 >= '\t' && LA21_0 <= '\n') || LA21_0 == '\r' || LA21_0 == ' ') )
                                {
                                    alt21 = 1;
                                }
                                switch (alt21) 
                                {
                                    case 1 :
                                        // MXMLLexer.g3:130:21: WS
                                        {
                                            mWS(); 

                                        }
                                        break;

                                }


                            }
                            break;

                        default:
                            goto loop22;
                    }
                } while (true);

                loop22:
                    ;   // Stops C# compiler whining that label 'loop22' has no statements

                int closeStart862 = CharIndex;
                mEMPTYTAG_CLOSE(); 
                close = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, closeStart862, CharIndex-1);
                AddToken( (CommonToken)close, EMPTYTAG_CLOSE, 0);

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "EMPTY_ELEMENT"

    // $ANTLR start "EMPTYTAG_CLOSE"
    public void mEMPTYTAG_CLOSE() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:134:2: ( '/>' )
            // MXMLLexer.g3:134:4: '/>'
            {
                Match("/>"); 


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "EMPTYTAG_CLOSE"

    // $ANTLR start "TAG_OPEN"
    public void mTAG_OPEN() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:138:2: ( '<' )
            // MXMLLexer.g3:138:4: '<'
            {
                Match('<'); 

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "TAG_OPEN"

    // $ANTLR start "EQ"
    public void mEQ() // throws RecognitionException [2]
    {
            try
            {
            IToken eq = null;

            // MXMLLexer.g3:142:2: (eq= EQ_int )
            // MXMLLexer.g3:143:2: eq= EQ_int
            {
                int eqStart919 = CharIndex;
                mEQ_int(); 
                eq = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, eqStart919, CharIndex-1);
                AddToken( (CommonToken)eq, EQ, 0);

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "EQ"

    // $ANTLR start "EQ_int"
    public void mEQ_int() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:147:2: ( '=' )
            // MXMLLexer.g3:148:2: '='
            {
                Match('='); 

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "EQ_int"

    // $ANTLR start "ATTRIBUTE"
    public void mATTRIBUTE() // throws RecognitionException [2]
    {
            try
            {
            IToken name = null;
            IToken value = null;

            // MXMLLexer.g3:152:5: (name= GENERIC_ID ( WS )? EQ ( WS )? value= VALUE )
            // MXMLLexer.g3:152:7: name= GENERIC_ID ( WS )? EQ ( WS )? value= VALUE
            {
                int nameStart957 = CharIndex;
                mGENERIC_ID(); 
                name = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, nameStart957, CharIndex-1);
                // MXMLLexer.g3:152:23: ( WS )?
                int alt23 = 2;
                int LA23_0 = input.LA(1);

                if ( ((LA23_0 >= '\t' && LA23_0 <= '\n') || LA23_0 == '\r' || LA23_0 == ' ') )
                {
                    alt23 = 1;
                }
                switch (alt23) 
                {
                    case 1 :
                        // MXMLLexer.g3:152:23: WS
                        {
                            mWS(); 

                        }
                        break;

                }

                mEQ(); 
                // MXMLLexer.g3:152:30: ( WS )?
                int alt24 = 2;
                int LA24_0 = input.LA(1);

                if ( ((LA24_0 >= '\t' && LA24_0 <= '\n') || LA24_0 == '\r' || LA24_0 == ' ') )
                {
                    alt24 = 1;
                }
                switch (alt24) 
                {
                    case 1 :
                        // MXMLLexer.g3:152:30: WS
                        {
                            mWS(); 

                        }
                        break;

                }

                int valueStart969 = CharIndex;
                mVALUE(); 
                value = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, valueStart969, CharIndex-1);

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "ATTRIBUTE"

    // $ANTLR start "END_TAG_OPEN"
    public void mEND_TAG_OPEN() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:155:22: ( '</' )
            // MXMLLexer.g3:156:2: '</'
            {
                Match("</"); 


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "END_TAG_OPEN"

    // $ANTLR start "TAG_CLOSE"
    public void mTAG_CLOSE() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:159:3: ( '>' )
            // MXMLLexer.g3:159:5: '>'
            {
                Match('>'); 

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "TAG_CLOSE"

    // $ANTLR start "END_TAG"
    public void mEND_TAG() // throws RecognitionException [2]
    {
            try
            {
            IToken open = null;
            IToken name = null;
            IToken close = null;

            // MXMLLexer.g3:162:5: (open= END_TAG_OPEN ( WS )? name= GENERIC_ID ( WS )? close= TAG_CLOSE )
            // MXMLLexer.g3:162:7: open= END_TAG_OPEN ( WS )? name= GENERIC_ID ( WS )? close= TAG_CLOSE
            {
                int openStart1017 = CharIndex;
                mEND_TAG_OPEN(); 
                open = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, openStart1017, CharIndex-1);
                AddToken( (CommonToken)open, END_TAG_OPEN, 0);
                // MXMLLexer.g3:162:75: ( WS )?
                int alt25 = 2;
                int LA25_0 = input.LA(1);

                if ( ((LA25_0 >= '\t' && LA25_0 <= '\n') || LA25_0 == '\r' || LA25_0 == ' ') )
                {
                    alt25 = 1;
                }
                switch (alt25) 
                {
                    case 1 :
                        // MXMLLexer.g3:162:75: WS
                        {
                            mWS(); 

                        }
                        break;

                }

                int nameStart1026 = CharIndex;
                mGENERIC_ID(); 
                name = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, nameStart1026, CharIndex-1);
                // MXMLLexer.g3:162:95: ( WS )?
                int alt26 = 2;
                int LA26_0 = input.LA(1);

                if ( ((LA26_0 >= '\t' && LA26_0 <= '\n') || LA26_0 == '\r' || LA26_0 == ' ') )
                {
                    alt26 = 1;
                }
                switch (alt26) 
                {
                    case 1 :
                        // MXMLLexer.g3:162:95: WS
                        {
                            mWS(); 

                        }
                        break;

                }

                int closeStart1033 = CharIndex;
                mTAG_CLOSE(); 
                close = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, closeStart1033, CharIndex-1);
                AddToken( (CommonToken)close, TAG_CLOSE, 0);

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "END_TAG"

    // $ANTLR start "COMMENT"
    public void mCOMMENT() // throws RecognitionException [2]
    {
            try
            {
            IToken c = null;

            // MXMLLexer.g3:165:17: (c= COMMENT_int )
            // MXMLLexer.g3:166:2: c= COMMENT_int
            {
                int cStart1052 = CharIndex;
                mCOMMENT_int(); 
                c = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, cStart1052, CharIndex-1);
                AddToken( (CommonToken)c, COMMENT, 0);

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "COMMENT"

    // $ANTLR start "COMMENT_int"
    public void mCOMMENT_int() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:169:2: ( '<!--' ( options {greedy=false; } : . )* '-->' )
            // MXMLLexer.g3:169:4: '<!--' ( options {greedy=false; } : . )* '-->'
            {
                Match("<!--"); 

                // MXMLLexer.g3:169:11: ( options {greedy=false; } : . )*
                do 
                {
                    int alt27 = 2;
                    int LA27_0 = input.LA(1);

                    if ( (LA27_0 == '-') )
                    {
                        int LA27_1 = input.LA(2);

                        if ( (LA27_1 == '-') )
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
                        else if ( ((LA27_1 >= '\u0000' && LA27_1 <= ',') || (LA27_1 >= '.' && LA27_1 <= '\uFFFF')) )
                        {
                            alt27 = 1;
                        }


                    }
                    else if ( ((LA27_0 >= '\u0000' && LA27_0 <= ',') || (LA27_0 >= '.' && LA27_0 <= '\uFFFF')) )
                    {
                        alt27 = 1;
                    }


                    switch (alt27) 
                    {
                        case 1 :
                            // MXMLLexer.g3:169:38: .
                            {
                                MatchAny(); 

                            }
                            break;

                        default:
                            goto loop27;
                    }
                } while (true);

                loop27:
                    ;   // Stops C# compiler whining that label 'loop27' has no statements

                Match("-->"); 


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "COMMENT_int"

    // $ANTLR start "CDATA"
    public void mCDATA() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:173:2: ( '<![CDATA[' ( options {greedy=false; } : . )* ']]>' )
            // MXMLLexer.g3:173:4: '<![CDATA[' ( options {greedy=false; } : . )* ']]>'
            {
                Match("<![CDATA["); 

                // MXMLLexer.g3:173:16: ( options {greedy=false; } : . )*
                do 
                {
                    int alt28 = 2;
                    int LA28_0 = input.LA(1);

                    if ( (LA28_0 == ']') )
                    {
                        int LA28_1 = input.LA(2);

                        if ( (LA28_1 == ']') )
                        {
                            int LA28_3 = input.LA(3);

                            if ( (LA28_3 == '>') )
                            {
                                alt28 = 2;
                            }
                            else if ( ((LA28_3 >= '\u0000' && LA28_3 <= '=') || (LA28_3 >= '?' && LA28_3 <= '\uFFFF')) )
                            {
                                alt28 = 1;
                            }


                        }
                        else if ( ((LA28_1 >= '\u0000' && LA28_1 <= '\\') || (LA28_1 >= '^' && LA28_1 <= '\uFFFF')) )
                        {
                            alt28 = 1;
                        }


                    }
                    else if ( ((LA28_0 >= '\u0000' && LA28_0 <= '\\') || (LA28_0 >= '^' && LA28_0 <= '\uFFFF')) )
                    {
                        alt28 = 1;
                    }


                    switch (alt28) 
                    {
                        case 1 :
                            // MXMLLexer.g3:173:43: .
                            {
                                MatchAny(); 

                            }
                            break;

                        default:
                            goto loop28;
                    }
                } while (true);

                loop28:
                    ;   // Stops C# compiler whining that label 'loop28' has no statements

                Match("]]>"); 


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "CDATA"

    // $ANTLR start "PCDATA"
    public void mPCDATA() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:177:3: ( (~ ( '<' | '\\n' | '\\r' ) )+ )
            // MXMLLexer.g3:178:4: (~ ( '<' | '\\n' | '\\r' ) )+
            {
                // MXMLLexer.g3:178:4: (~ ( '<' | '\\n' | '\\r' ) )+
                int cnt29 = 0;
                do 
                {
                    int alt29 = 2;
                    int LA29_0 = input.LA(1);

                    if ( ((LA29_0 >= '\u0000' && LA29_0 <= '\t') || (LA29_0 >= '\u000B' && LA29_0 <= '\f') || (LA29_0 >= '\u000E' && LA29_0 <= ';') || (LA29_0 >= '=' && LA29_0 <= '\uFFFF')) )
                    {
                        alt29 = 1;
                    }


                    switch (alt29) 
                    {
                        case 1 :
                            // MXMLLexer.g3:178:4: ~ ( '<' | '\\n' | '\\r' )
                            {
                                if ( (input.LA(1) >= '\u0000' && input.LA(1) <= '\t') || (input.LA(1) >= '\u000B' && input.LA(1) <= '\f') || (input.LA(1) >= '\u000E' && input.LA(1) <= ';') || (input.LA(1) >= '=' && input.LA(1) <= '\uFFFF') ) 
                                {
                                    input.Consume();

                                }
                                else 
                                {
                                    MismatchedSetException mse = new MismatchedSetException(null,input);
                                    Recover(mse);
                                    throw mse;}


                            }
                            break;

                        default:
                            if ( cnt29 >= 1 ) goto loop29;
                                EarlyExitException eee =
                                    new EarlyExitException(29, input);
                                throw eee;
                    }
                    cnt29++;
                } while (true);

                loop29:
                    ;   // Stops C# compiler whinging that label 'loop29' has no statements


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "PCDATA"

    // $ANTLR start "VALUE"
    public void mVALUE() // throws RecognitionException [2]
    {
            try
            {
            IToken v = null;

            // MXMLLexer.g3:181:16: (v= VALUE_int )
            // MXMLLexer.g3:182:3: v= VALUE_int
            {
                int vStart1160 = CharIndex;
                mVALUE_int(); 
                v = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, vStart1160, CharIndex-1);
                AddToken( (CommonToken)v, VALUE, 0);

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "VALUE"

    // $ANTLR start "VALUE_int"
    public void mVALUE_int() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:185:20: ( ( '\\\"' (~ '\\\"' )* '\\\"' | '\\'' (~ '\\'' )* '\\'' ) )
            // MXMLLexer.g3:186:9: ( '\\\"' (~ '\\\"' )* '\\\"' | '\\'' (~ '\\'' )* '\\'' )
            {
                // MXMLLexer.g3:186:9: ( '\\\"' (~ '\\\"' )* '\\\"' | '\\'' (~ '\\'' )* '\\'' )
                int alt32 = 2;
                int LA32_0 = input.LA(1);

                if ( (LA32_0 == '\"') )
                {
                    alt32 = 1;
                }
                else if ( (LA32_0 == '\'') )
                {
                    alt32 = 2;
                }
                else 
                {
                    NoViableAltException nvae_d32s0 =
                        new NoViableAltException("", 32, 0, input);

                    throw nvae_d32s0;
                }
                switch (alt32) 
                {
                    case 1 :
                        // MXMLLexer.g3:186:11: '\\\"' (~ '\\\"' )* '\\\"'
                        {
                            Match('\"'); 
                            // MXMLLexer.g3:186:16: (~ '\\\"' )*
                            do 
                            {
                                int alt30 = 2;
                                int LA30_0 = input.LA(1);

                                if ( ((LA30_0 >= '\u0000' && LA30_0 <= '!') || (LA30_0 >= '#' && LA30_0 <= '\uFFFF')) )
                                {
                                    alt30 = 1;
                                }


                                switch (alt30) 
                                {
                                    case 1 :
                                        // MXMLLexer.g3:186:17: ~ '\\\"'
                                        {
                                            if ( (input.LA(1) >= '\u0000' && input.LA(1) <= '!') || (input.LA(1) >= '#' && input.LA(1) <= '\uFFFF') ) 
                                            {
                                                input.Consume();

                                            }
                                            else 
                                            {
                                                MismatchedSetException mse = new MismatchedSetException(null,input);
                                                Recover(mse);
                                                throw mse;}


                                        }
                                        break;

                                    default:
                                        goto loop30;
                                }
                            } while (true);

                            loop30:
                                ;   // Stops C# compiler whining that label 'loop30' has no statements

                            Match('\"'); 

                        }
                        break;
                    case 2 :
                        // MXMLLexer.g3:187:11: '\\'' (~ '\\'' )* '\\''
                        {
                            Match('\''); 
                            // MXMLLexer.g3:187:16: (~ '\\'' )*
                            do 
                            {
                                int alt31 = 2;
                                int LA31_0 = input.LA(1);

                                if ( ((LA31_0 >= '\u0000' && LA31_0 <= '&') || (LA31_0 >= '(' && LA31_0 <= '\uFFFF')) )
                                {
                                    alt31 = 1;
                                }


                                switch (alt31) 
                                {
                                    case 1 :
                                        // MXMLLexer.g3:187:17: ~ '\\''
                                        {
                                            if ( (input.LA(1) >= '\u0000' && input.LA(1) <= '&') || (input.LA(1) >= '(' && input.LA(1) <= '\uFFFF') ) 
                                            {
                                                input.Consume();

                                            }
                                            else 
                                            {
                                                MismatchedSetException mse = new MismatchedSetException(null,input);
                                                Recover(mse);
                                                throw mse;}


                                        }
                                        break;

                                    default:
                                        goto loop31;
                                }
                            } while (true);

                            loop31:
                                ;   // Stops C# compiler whining that label 'loop31' has no statements

                            Match('\''); 

                        }
                        break;

                }


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "VALUE_int"

    // $ANTLR start "GENERIC_ID"
    public void mGENERIC_ID() // throws RecognitionException [2]
    {
            try
            {
            IToken id = null;

            // MXMLLexer.g3:192:2: (id= GENERIC_ID_int )
            // MXMLLexer.g3:193:2: id= GENERIC_ID_int
            {
                int idStart1243 = CharIndex;
                mGENERIC_ID_int(); 
                id = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, idStart1243, CharIndex-1);
                AddToken( (CommonToken)id, GENERIC_ID, 0);

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "GENERIC_ID"

    // $ANTLR start "GENERIC_ID_int"
    public void mGENERIC_ID_int() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:197:5: ( ( LETTER | '_' | ':' ) ( options {greedy=true; } : LETTER | '0' .. '9' | '.' | '-' | '_' | ':' )* )
            // MXMLLexer.g3:197:7: ( LETTER | '_' | ':' ) ( options {greedy=true; } : LETTER | '0' .. '9' | '.' | '-' | '_' | ':' )*
            {
                if ( input.LA(1) == ':' || (input.LA(1) >= 'A' && input.LA(1) <= 'Z') || input.LA(1) == '_' || (input.LA(1) >= 'a' && input.LA(1) <= 'z') ) 
                {
                    input.Consume();

                }
                else 
                {
                    MismatchedSetException mse = new MismatchedSetException(null,input);
                    Recover(mse);
                    throw mse;}

                // MXMLLexer.g3:198:9: ( options {greedy=true; } : LETTER | '0' .. '9' | '.' | '-' | '_' | ':' )*
                do 
                {
                    int alt33 = 7;
                    switch ( input.LA(1) ) 
                    {
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
                        alt33 = 1;
                        }
                        break;
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
                        {
                        alt33 = 2;
                        }
                        break;
                    case '.':
                        {
                        alt33 = 3;
                        }
                        break;
                    case '-':
                        {
                        alt33 = 4;
                        }
                        break;
                    case '_':
                        {
                        alt33 = 5;
                        }
                        break;
                    case ':':
                        {
                        alt33 = 6;
                        }
                        break;

                    }

                    switch (alt33) 
                    {
                        case 1 :
                            // MXMLLexer.g3:198:36: LETTER
                            {
                                mLETTER(); 

                            }
                            break;
                        case 2 :
                            // MXMLLexer.g3:198:45: '0' .. '9'
                            {
                                MatchRange('0','9'); 

                            }
                            break;
                        case 3 :
                            // MXMLLexer.g3:198:56: '.'
                            {
                                Match('.'); 

                            }
                            break;
                        case 4 :
                            // MXMLLexer.g3:198:62: '-'
                            {
                                Match('-'); 

                            }
                            break;
                        case 5 :
                            // MXMLLexer.g3:198:68: '_'
                            {
                                Match('_'); 

                            }
                            break;
                        case 6 :
                            // MXMLLexer.g3:198:74: ':'
                            {
                                Match(':'); 

                            }
                            break;

                        default:
                            goto loop33;
                    }
                } while (true);

                loop33:
                    ;   // Stops C# compiler whining that label 'loop33' has no statements


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "GENERIC_ID_int"

    // $ANTLR start "LETTER"
    public void mLETTER() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:202:2: ( 'a' .. 'z' | 'A' .. 'Z' )
            // MXMLLexer.g3:
            {
                if ( (input.LA(1) >= 'A' && input.LA(1) <= 'Z') || (input.LA(1) >= 'a' && input.LA(1) <= 'z') ) 
                {
                    input.Consume();

                }
                else 
                {
                    MismatchedSetException mse = new MismatchedSetException(null,input);
                    Recover(mse);
                    throw mse;}


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "LETTER"

    // $ANTLR start "WS"
    public void mWS() // throws RecognitionException [2]
    {
            try
            {
            IToken ws = null;

            // MXMLLexer.g3:206:14: ( (ws= OTHERWS | EOL )+ )
            // MXMLLexer.g3:207:9: (ws= OTHERWS | EOL )+
            {
                // MXMLLexer.g3:207:9: (ws= OTHERWS | EOL )+
                int cnt34 = 0;
                do 
                {
                    int alt34 = 3;
                    int LA34_0 = input.LA(1);

                    if ( (LA34_0 == '\t' || LA34_0 == ' ') )
                    {
                        alt34 = 1;
                    }
                    else if ( (LA34_0 == '\n' || LA34_0 == '\r') )
                    {
                        alt34 = 2;
                    }


                    switch (alt34) 
                    {
                        case 1 :
                            // MXMLLexer.g3:207:10: ws= OTHERWS
                            {
                                int wsStart1367 = CharIndex;
                                mOTHERWS(); 
                                ws = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, wsStart1367, CharIndex-1);
                                AddToken( (CommonToken)ws, WS, 0);

                            }
                            break;
                        case 2 :
                            // MXMLLexer.g3:208:11: EOL
                            {
                                mEOL(); 

                            }
                            break;

                        default:
                            if ( cnt34 >= 1 ) goto loop34;
                                EarlyExitException eee =
                                    new EarlyExitException(34, input);
                                throw eee;
                    }
                    cnt34++;
                } while (true);

                loop34:
                    ;   // Stops C# compiler whinging that label 'loop34' has no statements


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "WS"

    // $ANTLR start "OTHERWS"
    public void mOTHERWS() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:219:17: ( ( ' ' | '\\t' ) )
            // MXMLLexer.g3:220:11: ( ' ' | '\\t' )
            {
                if ( input.LA(1) == '\t' || input.LA(1) == ' ' ) 
                {
                    input.Consume();

                }
                else 
                {
                    MismatchedSetException mse = new MismatchedSetException(null,input);
                    Recover(mse);
                    throw mse;}


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "OTHERWS"

    // $ANTLR start "EOL"
    public void mEOL() // throws RecognitionException [2]
    {
            try
            {
            IToken ws = null;

            // MXMLLexer.g3:223:14: (ws= EOL_HELPER )
            // MXMLLexer.g3:224:10: ws= EOL_HELPER
            {
                int wsStart1472 = CharIndex;
                mEOL_HELPER(); 
                ws = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, wsStart1472, CharIndex-1);
                AddToken( (CommonToken)ws, EOL, 0);

            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "EOL"

    // $ANTLR start "EOL_HELPER"
    public void mEOL_HELPER() // throws RecognitionException [2]
    {
            try
            {
            // MXMLLexer.g3:228:3: ( ( '\\n' | '\\r\\n' | '\\r' ) )
            // MXMLLexer.g3:228:5: ( '\\n' | '\\r\\n' | '\\r' )
            {
                // MXMLLexer.g3:228:5: ( '\\n' | '\\r\\n' | '\\r' )
                int alt35 = 3;
                int LA35_0 = input.LA(1);

                if ( (LA35_0 == '\n') )
                {
                    alt35 = 1;
                }
                else if ( (LA35_0 == '\r') )
                {
                    int LA35_2 = input.LA(2);

                    if ( (LA35_2 == '\n') )
                    {
                        alt35 = 2;
                    }
                    else 
                    {
                        alt35 = 3;}
                }
                else 
                {
                    NoViableAltException nvae_d35s0 =
                        new NoViableAltException("", 35, 0, input);

                    throw nvae_d35s0;
                }
                switch (alt35) 
                {
                    case 1 :
                        // MXMLLexer.g3:228:6: '\\n'
                        {
                            Match('\n'); 

                        }
                        break;
                    case 2 :
                        // MXMLLexer.g3:228:13: '\\r\\n'
                        {
                            Match("\r\n"); 


                        }
                        break;
                    case 3 :
                        // MXMLLexer.g3:228:22: '\\r'
                        {
                            Match('\r'); 

                        }
                        break;

                }


            }

        }
        finally 
        {
        }
    }
    // $ANTLR end "EOL_HELPER"

    override public void mTokens() // throws RecognitionException 
    {
        // MXMLLexer.g3:1:8: ( DOCUMENT )
        // MXMLLexer.g3:1:10: DOCUMENT
        {
            mDOCUMENT(); 

        }


    }


    protected DFA14 dfa14;
    protected DFA13 dfa13;
    private void InitializeCyclicDFAs()
    {
        this.dfa14 = new DFA14(this);
        this.dfa13 = new DFA13(this);
        this.dfa14.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA14_SpecialStateTransition);
        this.dfa13.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA13_SpecialStateTransition);
    }

    const string DFA14_eotS =
        "\x37\uffff";
    const string DFA14_eofS =
        "\x37\uffff";
    const string DFA14_minS =
        "\x01\x3c\x0f\x09\x02\uffff\x10\x09\x02\x00\x0a\x09\x01\x00\x01"+
        "\x09\x01\x00\x06\x09";
    const string DFA14_maxS =
        "\x01\x3c\x0f\x7a\x02\uffff\x09\x7a\x01\x27\x03\x7a\x03\x27\x02"+
        "\uffff\x06\x7a\x03\x3d\x01\x27\x01\uffff\x01\x7a\x01\uffff\x01\x7a"+
        "\x01\x3d\x04\x7a";
    const string DFA14_acceptS =
        "\x10\uffff\x01\x01\x01\x02\x25\uffff";
    const string DFA14_specialS =
        "\x22\uffff\x01\x02\x01\x01\x0a\uffff\x01\x03\x01\uffff\x01\x00"+
        "\x06\uffff}>";
    static readonly string[] DFA14_transitionS = {
            "\x01\x01",
            "\x01\x02\x01\x03\x02\uffff\x01\x04\x12\uffff\x01\x02\x19\uffff"+
            "\x01\x05\x06\uffff\x1a\x05\x04\uffff\x01\x05\x01\uffff\x1a\x05",
            "\x01\x02\x01\x03\x02\uffff\x01\x04\x12\uffff\x01\x02\x19\uffff"+
            "\x01\x05\x06\uffff\x1a\x05\x04\uffff\x01\x05\x01\uffff\x1a\x05",
            "\x01\x02\x01\x03\x02\uffff\x01\x04\x12\uffff\x01\x02\x19\uffff"+
            "\x01\x05\x06\uffff\x1a\x05\x04\uffff\x01\x05\x01\uffff\x1a\x05",
            "\x01\x02\x01\x06\x02\uffff\x01\x04\x12\uffff\x01\x02\x19\uffff"+
            "\x01\x05\x06\uffff\x1a\x05\x04\uffff\x01\x05\x01\uffff\x1a\x05",
            "\x01\x0d\x01\x0e\x02\uffff\x01\x0f\x12\uffff\x01\x0d\x0c\uffff"+
            "\x01\x0a\x01\x09\x01\x11\x0a\x08\x01\x0c\x03\uffff\x01\x10\x02"+
            "\uffff\x1a\x07\x04\uffff\x01\x0b\x01\uffff\x1a\x07",
            "\x01\x02\x01\x03\x02\uffff\x01\x04\x12\uffff\x01\x02\x19\uffff"+
            "\x01\x05\x06\uffff\x1a\x05\x04\uffff\x01\x05\x01\uffff\x1a\x05",
            "\x01\x18\x01\x19\x02\uffff\x01\x1a\x12\uffff\x01\x18\x0c\uffff"+
            "\x01\x15\x01\x14\x01\x11\x0a\x13\x01\x17\x02\uffff\x01\x1b\x01"+
            "\x10\x02\uffff\x1a\x12\x04\uffff\x01\x16\x01\uffff\x1a\x12",
            "\x01\x0d\x01\x0e\x02\uffff\x01\x0f\x12\uffff\x01\x0d\x0c\uffff"+
            "\x01\x0a\x01\x09\x01\x11\x0a\x08\x01\x0c\x03\uffff\x01\x10\x02"+
            "\uffff\x1a\x07\x04\uffff\x01\x0b\x01\uffff\x1a\x07",
            "\x01\x0d\x01\x0e\x02\uffff\x01\x0f\x12\uffff\x01\x0d\x0c\uffff"+
            "\x01\x0a\x01\x09\x01\x11\x0a\x08\x01\x0c\x03\uffff\x01\x10\x02"+
            "\uffff\x1a\x07\x04\uffff\x01\x0b\x01\uffff\x1a\x07",
            "\x01\x0d\x01\x0e\x02\uffff\x01\x0f\x12\uffff\x01\x0d\x0c\uffff"+
            "\x01\x0a\x01\x09\x01\x11\x0a\x08\x01\x0c\x03\uffff\x01\x10\x02"+
            "\uffff\x1a\x07\x04\uffff\x01\x0b\x01\uffff\x1a\x07",
            "\x01\x18\x01\x19\x02\uffff\x01\x1a\x12\uffff\x01\x18\x0c\uffff"+
            "\x01\x15\x01\x14\x01\x11\x0a\x13\x01\x17\x02\uffff\x01\x1b\x01"+
            "\x10\x02\uffff\x1a\x12\x04\uffff\x01\x16\x01\uffff\x1a\x12",
            "\x01\x18\x01\x19\x02\uffff\x01\x1a\x12\uffff\x01\x18\x0c\uffff"+
            "\x01\x15\x01\x14\x01\x11\x0a\x13\x01\x17\x02\uffff\x01\x1b\x01"+
            "\x10\x02\uffff\x1a\x12\x04\uffff\x01\x16\x01\uffff\x1a\x12",
            "\x01\x0d\x01\x0e\x02\uffff\x01\x0f\x12\uffff\x01\x0d\x0e\uffff"+
            "\x01\x11\x0a\uffff\x01\x1c\x03\uffff\x01\x10\x02\uffff\x1a\x1c"+
            "\x04\uffff\x01\x1c\x01\uffff\x1a\x1c",
            "\x01\x0d\x01\x0e\x02\uffff\x01\x0f\x12\uffff\x01\x0d\x0e\uffff"+
            "\x01\x11\x0a\uffff\x01\x1c\x03\uffff\x01\x10\x02\uffff\x1a\x1c"+
            "\x04\uffff\x01\x1c\x01\uffff\x1a\x1c",
            "\x01\x0d\x01\x1d\x02\uffff\x01\x0f\x12\uffff\x01\x0d\x0e\uffff"+
            "\x01\x11\x0a\uffff\x01\x1c\x03\uffff\x01\x10\x02\uffff\x1a\x1c"+
            "\x04\uffff\x01\x1c\x01\uffff\x1a\x1c",
            "",
            "",
            "\x01\x18\x01\x19\x02\uffff\x01\x1a\x12\uffff\x01\x18\x0c\uffff"+
            "\x01\x15\x01\x14\x01\x11\x0a\x13\x01\x17\x02\uffff\x01\x1b\x01"+
            "\x10\x02\uffff\x1a\x12\x04\uffff\x01\x16\x01\uffff\x1a\x12",
            "\x01\x18\x01\x19\x02\uffff\x01\x1a\x12\uffff\x01\x18\x0c\uffff"+
            "\x01\x15\x01\x14\x01\x11\x0a\x13\x01\x17\x02\uffff\x01\x1b\x01"+
            "\x10\x02\uffff\x1a\x12\x04\uffff\x01\x16\x01\uffff\x1a\x12",
            "\x01\x18\x01\x19\x02\uffff\x01\x1a\x12\uffff\x01\x18\x0c\uffff"+
            "\x01\x15\x01\x14\x01\x11\x0a\x13\x01\x17\x02\uffff\x01\x1b\x01"+
            "\x10\x02\uffff\x1a\x12\x04\uffff\x01\x16\x01\uffff\x1a\x12",
            "\x01\x18\x01\x19\x02\uffff\x01\x1a\x12\uffff\x01\x18\x0c\uffff"+
            "\x01\x15\x01\x14\x01\x11\x0a\x13\x01\x17\x02\uffff\x01\x1b\x01"+
            "\x10\x02\uffff\x1a\x12\x04\uffff\x01\x16\x01\uffff\x1a\x12",
            "\x01\x18\x01\x19\x02\uffff\x01\x1a\x12\uffff\x01\x18\x0c\uffff"+
            "\x01\x15\x01\x14\x01\x11\x0a\x13\x01\x17\x02\uffff\x01\x1b\x01"+
            "\x10\x02\uffff\x1a\x12\x04\uffff\x01\x16\x01\uffff\x1a\x12",
            "\x01\x18\x01\x19\x02\uffff\x01\x1a\x12\uffff\x01\x18\x0c\uffff"+
            "\x01\x15\x01\x14\x01\x11\x0a\x13\x01\x17\x02\uffff\x01\x1b\x01"+
            "\x10\x02\uffff\x1a\x12\x04\uffff\x01\x16\x01\uffff\x1a\x12",
            "\x01\x18\x01\x19\x02\uffff\x01\x1a\x12\uffff\x01\x18\x0e\uffff"+
            "\x01\x11\x0a\uffff\x01\x1c\x02\uffff\x01\x1b\x01\x10\x02\uffff"+
            "\x1a\x1c\x04\uffff\x01\x1c\x01\uffff\x1a\x1c",
            "\x01\x18\x01\x19\x02\uffff\x01\x1a\x12\uffff\x01\x18\x0e\uffff"+
            "\x01\x11\x0a\uffff\x01\x1c\x02\uffff\x01\x1b\x01\x10\x02\uffff"+
            "\x1a\x1c\x04\uffff\x01\x1c\x01\uffff\x1a\x1c",
            "\x01\x18\x01\x1e\x02\uffff\x01\x1a\x12\uffff\x01\x18\x0e\uffff"+
            "\x01\x11\x0a\uffff\x01\x1c\x02\uffff\x01\x1b\x01\x10\x02\uffff"+
            "\x1a\x1c\x04\uffff\x01\x1c\x01\uffff\x1a\x1c",
            "\x01\x1f\x01\x20\x02\uffff\x01\x21\x12\uffff\x01\x1f\x01\uffff"+
            "\x01\x22\x04\uffff\x01\x23",
            "\x01\x2a\x01\x2b\x02\uffff\x01\x2c\x12\uffff\x01\x2a\x0c\uffff"+
            "\x01\x27\x01\x26\x01\uffff\x0a\x25\x01\x29\x02\uffff\x01\x1b"+
            "\x03\uffff\x1a\x24\x04\uffff\x01\x28\x01\uffff\x1a\x24",
            "\x01\x0d\x01\x0e\x02\uffff\x01\x0f\x12\uffff\x01\x0d\x0e\uffff"+
            "\x01\x11\x0a\uffff\x01\x1c\x03\uffff\x01\x10\x02\uffff\x1a\x1c"+
            "\x04\uffff\x01\x1c\x01\uffff\x1a\x1c",
            "\x01\x18\x01\x19\x02\uffff\x01\x1a\x12\uffff\x01\x18\x0e\uffff"+
            "\x01\x11\x0a\uffff\x01\x1c\x02\uffff\x01\x1b\x01\x10\x02\uffff"+
            "\x1a\x1c\x04\uffff\x01\x1c\x01\uffff\x1a\x1c",
            "\x01\x1f\x01\x20\x02\uffff\x01\x21\x12\uffff\x01\x1f\x01\uffff"+
            "\x01\x22\x04\uffff\x01\x23",
            "\x01\x1f\x01\x20\x02\uffff\x01\x21\x12\uffff\x01\x1f\x01\uffff"+
            "\x01\x22\x04\uffff\x01\x23",
            "\x01\x1f\x01\x2d\x02\uffff\x01\x21\x12\uffff\x01\x1f\x01\uffff"+
            "\x01\x22\x04\uffff\x01\x23",
            "\x22\x2e\x01\x2f\uffdd\x2e",
            "\x27\x30\x01\x31\uffd8\x30",
            "\x01\x2a\x01\x2b\x02\uffff\x01\x2c\x12\uffff\x01\x2a\x0c\uffff"+
            "\x01\x27\x01\x26\x01\uffff\x0a\x25\x01\x29\x02\uffff\x01\x1b"+
            "\x03\uffff\x1a\x24\x04\uffff\x01\x28\x01\uffff\x1a\x24",
            "\x01\x2a\x01\x2b\x02\uffff\x01\x2c\x12\uffff\x01\x2a\x0c\uffff"+
            "\x01\x27\x01\x26\x01\uffff\x0a\x25\x01\x29\x02\uffff\x01\x1b"+
            "\x03\uffff\x1a\x24\x04\uffff\x01\x28\x01\uffff\x1a\x24",
            "\x01\x2a\x01\x2b\x02\uffff\x01\x2c\x12\uffff\x01\x2a\x0c\uffff"+
            "\x01\x27\x01\x26\x01\uffff\x0a\x25\x01\x29\x02\uffff\x01\x1b"+
            "\x03\uffff\x1a\x24\x04\uffff\x01\x28\x01\uffff\x1a\x24",
            "\x01\x2a\x01\x2b\x02\uffff\x01\x2c\x12\uffff\x01\x2a\x0c\uffff"+
            "\x01\x27\x01\x26\x01\uffff\x0a\x25\x01\x29\x02\uffff\x01\x1b"+
            "\x03\uffff\x1a\x24\x04\uffff\x01\x28\x01\uffff\x1a\x24",
            "\x01\x2a\x01\x2b\x02\uffff\x01\x2c\x12\uffff\x01\x2a\x0c\uffff"+
            "\x01\x27\x01\x26\x01\uffff\x0a\x25\x01\x29\x02\uffff\x01\x1b"+
            "\x03\uffff\x1a\x24\x04\uffff\x01\x28\x01\uffff\x1a\x24",
            "\x01\x2a\x01\x2b\x02\uffff\x01\x2c\x12\uffff\x01\x2a\x0c\uffff"+
            "\x01\x27\x01\x26\x01\uffff\x0a\x25\x01\x29\x02\uffff\x01\x1b"+
            "\x03\uffff\x1a\x24\x04\uffff\x01\x28\x01\uffff\x1a\x24",
            "\x01\x2a\x01\x2b\x02\uffff\x01\x2c\x12\uffff\x01\x2a\x1c\uffff"+
            "\x01\x1b",
            "\x01\x2a\x01\x2b\x02\uffff\x01\x2c\x12\uffff\x01\x2a\x1c\uffff"+
            "\x01\x1b",
            "\x01\x2a\x01\x32\x02\uffff\x01\x2c\x12\uffff\x01\x2a\x1c\uffff"+
            "\x01\x1b",
            "\x01\x1f\x01\x20\x02\uffff\x01\x21\x12\uffff\x01\x1f\x01\uffff"+
            "\x01\x22\x04\uffff\x01\x23",
            "\x22\x2e\x01\x2f\uffdd\x2e",
            "\x01\x33\x01\x34\x02\uffff\x01\x35\x12\uffff\x01\x33\x0e\uffff"+
            "\x01\x11\x0a\uffff\x01\x1c\x03\uffff\x01\x10\x02\uffff\x1a\x1c"+
            "\x04\uffff\x01\x1c\x01\uffff\x1a\x1c",
            "\x27\x30\x01\x31\uffd8\x30",
            "\x01\x33\x01\x34\x02\uffff\x01\x35\x12\uffff\x01\x33\x0e\uffff"+
            "\x01\x11\x0a\uffff\x01\x1c\x03\uffff\x01\x10\x02\uffff\x1a\x1c"+
            "\x04\uffff\x01\x1c\x01\uffff\x1a\x1c",
            "\x01\x2a\x01\x2b\x02\uffff\x01\x2c\x12\uffff\x01\x2a\x1c\uffff"+
            "\x01\x1b",
            "\x01\x33\x01\x34\x02\uffff\x01\x35\x12\uffff\x01\x33\x0e\uffff"+
            "\x01\x11\x0a\uffff\x01\x1c\x03\uffff\x01\x10\x02\uffff\x1a\x1c"+
            "\x04\uffff\x01\x1c\x01\uffff\x1a\x1c",
            "\x01\x33\x01\x34\x02\uffff\x01\x35\x12\uffff\x01\x33\x0e\uffff"+
            "\x01\x11\x0a\uffff\x01\x1c\x03\uffff\x01\x10\x02\uffff\x1a\x1c"+
            "\x04\uffff\x01\x1c\x01\uffff\x1a\x1c",
            "\x01\x33\x01\x36\x02\uffff\x01\x35\x12\uffff\x01\x33\x0e\uffff"+
            "\x01\x11\x0a\uffff\x01\x1c\x03\uffff\x01\x10\x02\uffff\x1a\x1c"+
            "\x04\uffff\x01\x1c\x01\uffff\x1a\x1c",
            "\x01\x33\x01\x34\x02\uffff\x01\x35\x12\uffff\x01\x33\x0e\uffff"+
            "\x01\x11\x0a\uffff\x01\x1c\x03\uffff\x01\x10\x02\uffff\x1a\x1c"+
            "\x04\uffff\x01\x1c\x01\uffff\x1a\x1c"
    };

    static readonly short[] DFA14_eot = DFA.UnpackEncodedString(DFA14_eotS);
    static readonly short[] DFA14_eof = DFA.UnpackEncodedString(DFA14_eofS);
    static readonly char[] DFA14_min = DFA.UnpackEncodedStringToUnsignedChars(DFA14_minS);
    static readonly char[] DFA14_max = DFA.UnpackEncodedStringToUnsignedChars(DFA14_maxS);
    static readonly short[] DFA14_accept = DFA.UnpackEncodedString(DFA14_acceptS);
    static readonly short[] DFA14_special = DFA.UnpackEncodedString(DFA14_specialS);
    static readonly short[][] DFA14_transition = DFA.UnpackEncodedStringArray(DFA14_transitionS);

    protected class DFA14 : DFA
    {
        public DFA14(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 14;
            this.eot = DFA14_eot;
            this.eof = DFA14_eof;
            this.min = DFA14_min;
            this.max = DFA14_max;
            this.accept = DFA14_accept;
            this.special = DFA14_special;
            this.transition = DFA14_transition;

        }

        override public string Description
        {
            get { return "108:7: ( START_TAG ( ELEMENT | EOL | t= PCDATA | t= CDATA | t= COMMENT | pi= PI )* END_TAG | EMPTY_ELEMENT )"; }
        }

    }


    protected internal int DFA14_SpecialStateTransition(DFA dfa, int s, IIntStream _input) //throws NoViableAltException
    {
            IIntStream input = _input;
        int _s = s;
        switch ( s )
        {
                case 0 : 
                    int LA14_48 = input.LA(1);

                    s = -1;
                    if ( (LA14_48 == '\'') ) { s = 49; }

                    else if ( ((LA14_48 >= '\u0000' && LA14_48 <= '&') || (LA14_48 >= '(' && LA14_48 <= '\uFFFF')) ) { s = 48; }

                    if ( s >= 0 ) return s;
                    break;
                case 1 : 
                    int LA14_35 = input.LA(1);

                    s = -1;
                    if ( ((LA14_35 >= '\u0000' && LA14_35 <= '&') || (LA14_35 >= '(' && LA14_35 <= '\uFFFF')) ) { s = 48; }

                    else if ( (LA14_35 == '\'') ) { s = 49; }

                    if ( s >= 0 ) return s;
                    break;
                case 2 : 
                    int LA14_34 = input.LA(1);

                    s = -1;
                    if ( ((LA14_34 >= '\u0000' && LA14_34 <= '!') || (LA14_34 >= '#' && LA14_34 <= '\uFFFF')) ) { s = 46; }

                    else if ( (LA14_34 == '\"') ) { s = 47; }

                    if ( s >= 0 ) return s;
                    break;
                case 3 : 
                    int LA14_46 = input.LA(1);

                    s = -1;
                    if ( (LA14_46 == '\"') ) { s = 47; }

                    else if ( ((LA14_46 >= '\u0000' && LA14_46 <= '!') || (LA14_46 >= '#' && LA14_46 <= '\uFFFF')) ) { s = 46; }

                    if ( s >= 0 ) return s;
                    break;
        }
        NoViableAltException nvae =
            new NoViableAltException(dfa.Description, 14, _s, input);
        dfa.Error(nvae);
        throw nvae;
    }
    const string DFA13_eotS =
        "\x0a\uffff";
    const string DFA13_eofS =
        "\x0a\uffff";
    const string DFA13_minS =
        "\x01\x00\x01\x09\x03\uffff\x01\x2d\x04\uffff";
    const string DFA13_maxS =
        "\x01\uffff\x01\x7a\x03\uffff\x01\x5b\x04\uffff";
    const string DFA13_acceptS =
        "\x02\uffff\x01\x02\x01\x03\x01\x07\x01\uffff\x01\x06\x01\x01\x01"+
        "\x04\x01\x05";
    const string DFA13_specialS =
        "\x01\x00\x09\uffff}>";
    static readonly string[] DFA13_transitionS = {
            "\x0a\x03\x01\x02\x02\x03\x01\x02\x2e\x03\x01\x01\uffc3\x03",
            "\x02\x07\x02\uffff\x01\x07\x12\uffff\x01\x07\x01\x05\x0d\uffff"+
            "\x01\x04\x0a\uffff\x01\x07\x04\uffff\x01\x06\x01\uffff\x1a\x07"+
            "\x04\uffff\x01\x07\x01\uffff\x1a\x07",
            "",
            "",
            "",
            "\x01\x09\x2d\uffff\x01\x08",
            "",
            "",
            "",
            ""
    };

    static readonly short[] DFA13_eot = DFA.UnpackEncodedString(DFA13_eotS);
    static readonly short[] DFA13_eof = DFA.UnpackEncodedString(DFA13_eofS);
    static readonly char[] DFA13_min = DFA.UnpackEncodedStringToUnsignedChars(DFA13_minS);
    static readonly char[] DFA13_max = DFA.UnpackEncodedStringToUnsignedChars(DFA13_maxS);
    static readonly short[] DFA13_accept = DFA.UnpackEncodedString(DFA13_acceptS);
    static readonly short[] DFA13_special = DFA.UnpackEncodedString(DFA13_specialS);
    static readonly short[][] DFA13_transition = DFA.UnpackEncodedStringArray(DFA13_transitionS);

    protected class DFA13 : DFA
    {
        public DFA13(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 13;
            this.eot = DFA13_eot;
            this.eof = DFA13_eof;
            this.min = DFA13_min;
            this.max = DFA13_max;
            this.accept = DFA13_accept;
            this.special = DFA13_special;
            this.transition = DFA13_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 109:13: ( ELEMENT | EOL | t= PCDATA | t= CDATA | t= COMMENT | pi= PI )*"; }
        }

    }


    protected internal int DFA13_SpecialStateTransition(DFA dfa, int s, IIntStream _input) //throws NoViableAltException
    {
            IIntStream input = _input;
        int _s = s;
        switch ( s )
        {
                case 0 : 
                    int LA13_0 = input.LA(1);

                    s = -1;
                    if ( (LA13_0 == '<') ) { s = 1; }

                    else if ( (LA13_0 == '\n' || LA13_0 == '\r') ) { s = 2; }

                    else if ( ((LA13_0 >= '\u0000' && LA13_0 <= '\t') || (LA13_0 >= '\u000B' && LA13_0 <= '\f') || (LA13_0 >= '\u000E' && LA13_0 <= ';') || (LA13_0 >= '=' && LA13_0 <= '\uFFFF')) ) { s = 3; }

                    if ( s >= 0 ) return s;
                    break;
        }
        NoViableAltException nvae =
            new NoViableAltException(dfa.Description, 13, _s, input);
        dfa.Error(nvae);
        throw nvae;
    }
 
    
}
