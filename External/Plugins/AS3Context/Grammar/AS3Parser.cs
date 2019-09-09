// $ANTLR 2.7.7 (20060930): "as3.g" -> "AS3Parser.cs"$


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
    // Generate the header common to all output files.
    using System;
    
    using TokenBuffer              = antlr.TokenBuffer;
    using TokenStreamException     = antlr.TokenStreamException;
    using TokenStreamIOException   = antlr.TokenStreamIOException;
    using ANTLRException           = antlr.ANTLRException;
    using LLkParser = antlr.LLkParser;
    using Token                    = antlr.Token;
    using IToken                   = antlr.IToken;
    using TokenStream              = antlr.TokenStream;
    using RecognitionException     = antlr.RecognitionException;
    using NoViableAltException     = antlr.NoViableAltException;
    using MismatchedTokenException = antlr.MismatchedTokenException;
    using SemanticException        = antlr.SemanticException;
    using ParserSharedInputState   = antlr.ParserSharedInputState;
    using BitSet                   = antlr.collections.impl.BitSet;
    using AST                      = antlr.collections.AST;
    using ASTPair                  = antlr.ASTPair;
    using ASTFactory               = antlr.ASTFactory;
    using ASTArray                 = antlr.collections.impl.ASTArray;
    
/**
 *  @author Martin Schnabel
 */
    public  class AS3Parser : antlr.LLkParser
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
        
        
        private void setRegionInfo(AST ast,AST startAST,AST endAST){
            // do something with the model
        }
        
        protected void initialize()
        {
            tokenNames = tokenNames_;
            initializeFactory();
        }
        
        
        protected AS3Parser(TokenBuffer tokenBuf, int k) : base(tokenBuf, k)
        {
            initialize();
        }
        
        public AS3Parser(TokenBuffer tokenBuf) : this(tokenBuf,2)
        {
        }
        
        protected AS3Parser(TokenStream lexer, int k) : base(lexer,k)
        {
            initialize();
        }
        
        public AS3Parser(TokenStream lexer) : this(lexer,2)
        {
        }
        
        public AS3Parser(ParserSharedInputState state) : base(state,2)
        {
            initialize();
        }
        
/**
 * this is the start rule for this parser
 */
    public void compilationUnit() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST compilationUnit_AST = null;
        AST mods_AST = null;
        
        AST tmp1_AST = null;
        tmp1_AST = astFactory.create(LT(1));
        astFactory.addASTChild(ref currentAST, tmp1_AST);
        match(LITERAL_package);
        {
            switch ( LA(1) )
            {
            case IDENT:
            {
                identifier();
                if (0 == inputState.guessing)
                {
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
                break;
            }
            case LCURLY:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        match(LCURLY);
        {    // ( ... )*
            for (;;)
            {
                switch ( LA(1) )
                {
                case LITERAL_import:
                {
                    importDefinition();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                    break;
                }
                case LBRACK:
                {
                    metadataDefinition();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                    break;
                }
                case LITERAL_class:
                case LITERAL_interface:
                case LITERAL_function:
                case LITERAL_namespace:
                case LITERAL_var:
                case LITERAL_const:
                case LITERAL_public:
                case LITERAL_private:
                case LITERAL_protected:
                case LITERAL_internal:
                case LITERAL_static:
                case LITERAL_final:
                case LITERAL_enumerable:
                case LITERAL_explicit:
                case LITERAL_override:
                case LITERAL_dynamic:
                {
                    modifiers();
                    if (0 == inputState.guessing)
                    {
                        mods_AST = (AST)returnAST;
                    }
                    {
                        switch ( LA(1) )
                        {
                        case LITERAL_class:
                        {
                            classDefinition(mods_AST);
                            if (0 == inputState.guessing)
                            {
                                astFactory.addASTChild(ref currentAST, returnAST);
                            }
                            break;
                        }
                        case LITERAL_interface:
                        {
                            interfaceDefinition(mods_AST);
                            if (0 == inputState.guessing)
                            {
                                astFactory.addASTChild(ref currentAST, returnAST);
                            }
                            break;
                        }
                        case LITERAL_var:
                        case LITERAL_const:
                        {
                            variableDefinition(mods_AST);
                            if (0 == inputState.guessing)
                            {
                                astFactory.addASTChild(ref currentAST, returnAST);
                            }
                            break;
                        }
                        case LITERAL_function:
                        {
                            methodDefinition(mods_AST);
                            if (0 == inputState.guessing)
                            {
                                astFactory.addASTChild(ref currentAST, returnAST);
                            }
                            break;
                        }
                        case LITERAL_namespace:
                        {
                            namespaceDefinition(mods_AST);
                            if (0 == inputState.guessing)
                            {
                                astFactory.addASTChild(ref currentAST, returnAST);
                            }
                            break;
                        }
                        default:
                        {
                            throw new NoViableAltException(LT(1), getFilename());
                        }
                         }
                    }
                    break;
                }
                case SEMI:
                {
                    match(SEMI);
                    break;
                }
                default:
                {
                    goto _loop5_breakloop;
                }
                 }
            }
_loop5_breakloop:           ;
        }    // ( ... )*
        match(RCURLY);
        match(Token.EOF_TYPE);
        if (0==inputState.guessing)
        {
            compilationUnit_AST = (AST)currentAST.root;
            compilationUnit_AST = (AST) astFactory.make(astFactory.create(COMPILATION_UNIT,"COMPILATION_UNIT"), compilationUnit_AST);
            currentAST.root = compilationUnit_AST;
            if ( (null != compilationUnit_AST) && (null != compilationUnit_AST.getFirstChild()) )
                currentAST.child = compilationUnit_AST.getFirstChild();
            else
                currentAST.child = compilationUnit_AST;
            currentAST.advanceChildToEnd();
        }
        compilationUnit_AST = currentAST.root;
        returnAST = compilationUnit_AST;
    }
    
    public void identifier() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST identifier_AST = null;
        IToken  s = null;
        AST s_AST = null;
        IToken  e = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        astFactory.makeASTRoot(ref currentAST, s_AST);
        match(IDENT);
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==DOT))
                {
                    match(DOT);
                    e = LT(1);
                    e_AST = astFactory.create(e);
                    astFactory.makeASTRoot(ref currentAST, e_AST);
                    match(IDENT);
                    if (0==inputState.guessing)
                    {
                        identifier_AST = (AST)currentAST.root;
                        setRegionInfo(identifier_AST,s_AST,e_AST);
                    }
                }
                else
                {
                    goto _loop112_breakloop;
                }
                
            }
_loop112_breakloop:         ;
        }    // ( ... )*
        if (0==inputState.guessing)
        {
            identifier_AST = (AST)currentAST.root;
            setRegionInfo(identifier_AST,s_AST,e_AST);
        }
        identifier_AST = currentAST.root;
        returnAST = identifier_AST;
    }
    
    public void importDefinition() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST importDefinition_AST = null;
        IToken  s = null;
        AST s_AST = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        match(LITERAL_import);
        identifierStar();
        if (0 == inputState.guessing)
        {
            e_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        if (0==inputState.guessing)
        {
            importDefinition_AST = (AST)currentAST.root;
            importDefinition_AST = (AST) astFactory.make(astFactory.create(IMPORT,"IMPORT"), importDefinition_AST);
                    setRegionInfo(importDefinition_AST,s_AST,e_AST);
            currentAST.root = importDefinition_AST;
            if ( (null != importDefinition_AST) && (null != importDefinition_AST.getFirstChild()) )
                currentAST.child = importDefinition_AST.getFirstChild();
            else
                currentAST.child = importDefinition_AST;
            currentAST.advanceChildToEnd();
        }
        importDefinition_AST = currentAST.root;
        returnAST = importDefinition_AST;
    }
    
    public void metadataDefinition() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST metadataDefinition_AST = null;
        
        AST tmp7_AST = null;
        tmp7_AST = astFactory.create(LT(1));
        astFactory.addASTChild(ref currentAST, tmp7_AST);
        match(LBRACK);
        {
            metadataItem();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            {    // ( ... )*
                for (;;)
                {
                    if ((LA(1)==COMMA))
                    {
                        match(COMMA);
                        metadataItem();
                        if (0 == inputState.guessing)
                        {
                            astFactory.addASTChild(ref currentAST, returnAST);
                        }
                    }
                    else
                    {
                        goto _loop10_breakloop;
                    }
                    
                }
_loop10_breakloop:              ;
            }    // ( ... )*
        }
        AST tmp9_AST = null;
        tmp9_AST = astFactory.create(LT(1));
        astFactory.addASTChild(ref currentAST, tmp9_AST);
        match(RBRACK);
        metadataDefinition_AST = currentAST.root;
        returnAST = metadataDefinition_AST;
    }
    
    public void modifiers() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST modifiers_AST = null;
        AST s_AST = null;
        AST e_AST = null;
        
        {
            switch ( LA(1) )
            {
            case LITERAL_public:
            case LITERAL_private:
            case LITERAL_protected:
            case LITERAL_internal:
            case LITERAL_static:
            case LITERAL_final:
            case LITERAL_enumerable:
            case LITERAL_explicit:
            case LITERAL_override:
            case LITERAL_dynamic:
            {
                modifier();
                if (0 == inputState.guessing)
                {
                    s_AST = (AST)returnAST;
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
                {    // ( ... )*
                    for (;;)
                    {
                        if (((LA(1) >= LITERAL_public && LA(1) <= LITERAL_dynamic)))
                        {
                            modifier();
                            if (0 == inputState.guessing)
                            {
                                e_AST = (AST)returnAST;
                                astFactory.addASTChild(ref currentAST, returnAST);
                            }
                        }
                        else
                        {
                            goto _loop120_breakloop;
                        }
                        
                    }
_loop120_breakloop:                 ;
                }    // ( ... )*
                break;
            }
            case LITERAL_class:
            case LITERAL_interface:
            case LITERAL_function:
            case LITERAL_namespace:
            case LITERAL_var:
            case LITERAL_const:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        if (0==inputState.guessing)
        {
            modifiers_AST = (AST)currentAST.root;
            modifiers_AST = (AST) astFactory.make(astFactory.create(MODIFIERS,"MODIFIERS"), modifiers_AST);
                    setRegionInfo(modifiers_AST,s_AST,e_AST);
            currentAST.root = modifiers_AST;
            if ( (null != modifiers_AST) && (null != modifiers_AST.getFirstChild()) )
                currentAST.child = modifiers_AST.getFirstChild();
            else
                currentAST.child = modifiers_AST;
            currentAST.advanceChildToEnd();
        }
        modifiers_AST = currentAST.root;
        returnAST = modifiers_AST;
    }
    
    public void classDefinition(
        AST mods
    ) //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST classDefinition_AST = null;
        IToken  s = null;
        AST s_AST = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        match(LITERAL_class);
        identifier();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        extendsClause();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        implementsClause();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        typeBlock();
        if (0 == inputState.guessing)
        {
            e_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        if (0==inputState.guessing)
        {
            classDefinition_AST = (AST)currentAST.root;
            classDefinition_AST = (AST) astFactory.make(astFactory.create(CLASS_DEF,"CLASS_DEF"), classDefinition_AST, mods);
                    setRegionInfo(classDefinition_AST,s_AST,e_AST);
            currentAST.root = classDefinition_AST;
            if ( (null != classDefinition_AST) && (null != classDefinition_AST.getFirstChild()) )
                currentAST.child = classDefinition_AST.getFirstChild();
            else
                currentAST.child = classDefinition_AST;
            currentAST.advanceChildToEnd();
        }
        classDefinition_AST = currentAST.root;
        returnAST = classDefinition_AST;
    }
    
    public void interfaceDefinition(
        AST mods
    ) //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST interfaceDefinition_AST = null;
        IToken  s = null;
        AST s_AST = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        match(LITERAL_interface);
        identifier();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        extendsClause();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        typeBlock();
        if (0 == inputState.guessing)
        {
            e_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        if (0==inputState.guessing)
        {
            interfaceDefinition_AST = (AST)currentAST.root;
            interfaceDefinition_AST = (AST) astFactory.make(astFactory.create(INTERFACE_DEF,"INTERFACE_DEF"), interfaceDefinition_AST, mods);
                    setRegionInfo(interfaceDefinition_AST,s_AST,e_AST);
            currentAST.root = interfaceDefinition_AST;
            if ( (null != interfaceDefinition_AST) && (null != interfaceDefinition_AST.getFirstChild()) )
                currentAST.child = interfaceDefinition_AST.getFirstChild();
            else
                currentAST.child = interfaceDefinition_AST;
            currentAST.advanceChildToEnd();
        }
        interfaceDefinition_AST = currentAST.root;
        returnAST = interfaceDefinition_AST;
    }
    
    public void variableDefinition(
        AST mods
    ) //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST variableDefinition_AST = null;
        
        {
            switch ( LA(1) )
            {
            case LITERAL_var:
            {
                match(LITERAL_var);
                break;
            }
            case LITERAL_const:
            {
                match(LITERAL_const);
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        variableDeclarator(getASTFactory().dupTree(mods));
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==COMMA))
                {
                    match(COMMA);
                    variableDeclarator(getASTFactory().dupTree(mods));
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop38_breakloop;
                }
                
            }
_loop38_breakloop:          ;
        }    // ( ... )*
        variableDefinition_AST = currentAST.root;
        returnAST = variableDefinition_AST;
    }
    
    public void methodDefinition(
        AST mods
    ) //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST methodDefinition_AST = null;
        IToken  s = null;
        AST s_AST = null;
        AST e1_AST = null;
        AST e2_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        match(LITERAL_function);
        {
            switch ( LA(1) )
            {
            case LITERAL_get:
            case LITERAL_set:
            {
                {
                    switch ( LA(1) )
                    {
                    case LITERAL_get:
                    {
                        AST tmp13_AST = null;
                        tmp13_AST = astFactory.create(LT(1));
                        astFactory.addASTChild(ref currentAST, tmp13_AST);
                        match(LITERAL_get);
                        break;
                    }
                    case LITERAL_set:
                    {
                        AST tmp14_AST = null;
                        tmp14_AST = astFactory.create(LT(1));
                        astFactory.addASTChild(ref currentAST, tmp14_AST);
                        match(LITERAL_set);
                        break;
                    }
                    default:
                    {
                        throw new NoViableAltException(LT(1), getFilename());
                    }
                     }
                }
                {
                    AST tmp15_AST = null;
                    tmp15_AST = astFactory.create(LT(1));
                    astFactory.addASTChild(ref currentAST, tmp15_AST);
                    match(tokenSet_0_);
                }
                break;
            }
            case IDENT:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        AST tmp16_AST = null;
        tmp16_AST = astFactory.create(LT(1));
        astFactory.addASTChild(ref currentAST, tmp16_AST);
        match(IDENT);
        parameterDeclarationList();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        typeExpression();
        if (0 == inputState.guessing)
        {
            e1_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {
            switch ( LA(1) )
            {
            case LCURLY:
            {
                block();
                if (0 == inputState.guessing)
                {
                    e2_AST = (AST)returnAST;
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
                break;
            }
            case SEMI:
            case RCURLY:
            case LITERAL_import:
            case LBRACK:
            case LITERAL_class:
            case LITERAL_interface:
            case LITERAL_function:
            case LITERAL_namespace:
            case LITERAL_var:
            case LITERAL_const:
            case LITERAL_public:
            case LITERAL_private:
            case LITERAL_protected:
            case LITERAL_internal:
            case LITERAL_static:
            case LITERAL_final:
            case LITERAL_enumerable:
            case LITERAL_explicit:
            case LITERAL_override:
            case LITERAL_dynamic:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        if (0==inputState.guessing)
        {
            methodDefinition_AST = (AST)currentAST.root;
            methodDefinition_AST = (AST) astFactory.make(astFactory.create(METHOD_DEF,"METHOD_DEF"), methodDefinition_AST, mods);
                    setRegionInfo(methodDefinition_AST,s_AST,e2_AST==null?e1_AST:e2_AST);
            currentAST.root = methodDefinition_AST;
            if ( (null != methodDefinition_AST) && (null != methodDefinition_AST.getFirstChild()) )
                currentAST.child = methodDefinition_AST.getFirstChild();
            else
                currentAST.child = methodDefinition_AST;
            currentAST.advanceChildToEnd();
        }
        methodDefinition_AST = currentAST.root;
        returnAST = methodDefinition_AST;
    }
    
    public void namespaceDefinition(
        AST mods
    ) //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST namespaceDefinition_AST = null;
        IToken  s = null;
        AST s_AST = null;
        IToken  e = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        match(LITERAL_namespace);
        e = LT(1);
        e_AST = astFactory.create(e);
        astFactory.addASTChild(ref currentAST, e_AST);
        match(IDENT);
        if (0==inputState.guessing)
        {
            namespaceDefinition_AST = (AST)currentAST.root;
            namespaceDefinition_AST = (AST) astFactory.make(astFactory.create(NAMESPACE_DEF,"NAMESPACE_DEF"), namespaceDefinition_AST, mods);
                    setRegionInfo(namespaceDefinition_AST,s_AST,e_AST);
            currentAST.root = namespaceDefinition_AST;
            if ( (null != namespaceDefinition_AST) && (null != namespaceDefinition_AST.getFirstChild()) )
                currentAST.child = namespaceDefinition_AST.getFirstChild();
            else
                currentAST.child = namespaceDefinition_AST;
            currentAST.advanceChildToEnd();
        }
        namespaceDefinition_AST = currentAST.root;
        returnAST = namespaceDefinition_AST;
    }
    
    public void identifierStar() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST identifierStar_AST = null;
        IToken  s = null;
        AST s_AST = null;
        IToken  e1 = null;
        AST e1_AST = null;
        IToken  e2 = null;
        AST e2_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        astFactory.makeASTRoot(ref currentAST, s_AST);
        match(IDENT);
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==DOT) && (LA(2)==IDENT))
                {
                    match(DOT);
                    e1 = LT(1);
                    e1_AST = astFactory.create(e1);
                    astFactory.makeASTRoot(ref currentAST, e1_AST);
                    match(IDENT);
                    if (0==inputState.guessing)
                    {
                        identifierStar_AST = (AST)currentAST.root;
                        setRegionInfo(identifierStar_AST,s_AST,e1_AST);
                    }
                }
                else
                {
                    goto _loop115_breakloop;
                }
                
            }
_loop115_breakloop:         ;
        }    // ( ... )*
        {
            switch ( LA(1) )
            {
            case DOT:
            {
                match(DOT);
                e2 = LT(1);
                e2_AST = astFactory.create(e2);
                astFactory.makeASTRoot(ref currentAST, e2_AST);
                match(STAR);
                if (0==inputState.guessing)
                {
                    identifierStar_AST = (AST)currentAST.root;
                    setRegionInfo(identifierStar_AST,s_AST,e2_AST);
                }
                break;
            }
            case SEMI:
            case RCURLY:
            case LITERAL_import:
            case LBRACK:
            case LITERAL_class:
            case LITERAL_interface:
            case LITERAL_function:
            case LITERAL_namespace:
            case LITERAL_var:
            case LITERAL_const:
            case LITERAL_public:
            case LITERAL_private:
            case LITERAL_protected:
            case LITERAL_internal:
            case LITERAL_static:
            case LITERAL_final:
            case LITERAL_enumerable:
            case LITERAL_explicit:
            case LITERAL_override:
            case LITERAL_dynamic:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        if (0==inputState.guessing)
        {
            identifierStar_AST = (AST)currentAST.root;
            setRegionInfo(identifierStar_AST,s_AST,e2_AST==null?e1_AST:e2_AST);
        }
        identifierStar_AST = currentAST.root;
        returnAST = identifierStar_AST;
    }
    
    public void metadataItem() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST metadataItem_AST = null;
        
        identifier();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {
            switch ( LA(1) )
            {
            case LPAREN:
            {
                AST tmp19_AST = null;
                tmp19_AST = astFactory.create(LT(1));
                astFactory.addASTChild(ref currentAST, tmp19_AST);
                match(LPAREN);
                {
                    switch ( LA(1) )
                    {
                    case LCURLY:
                    case LBRACK:
                    case LPAREN:
                    case LITERAL_function:
                    case IDENT:
                    case NUMBER:
                    case PLUS:
                    case MINUS:
                    case INC:
                    case DEC:
                    case LITERAL_delete:
                    case LITERAL_typeof:
                    case LNOT:
                    case BNOT:
                    case LITERAL_null:
                    case LITERAL_true:
                    case LITERAL_false:
                    case LITERAL_undefined:
                    case STRING_LITERAL:
                    case REGEX_LITERAL:
                    case XML_LITERAL:
                    case LITERAL_new:
                    {
                        expression();
                        {    // ( ... )*
                            for (;;)
                            {
                                if ((LA(1)==COMMA))
                                {
                                    match(COMMA);
                                    expression();
                                    if (0 == inputState.guessing)
                                    {
                                        astFactory.addASTChild(ref currentAST, returnAST);
                                    }
                                }
                                else
                                {
                                    goto _loop15_breakloop;
                                }
                                
                            }
_loop15_breakloop:                          ;
                        }    // ( ... )*
                        break;
                    }
                    case RPAREN:
                    {
                        break;
                    }
                    default:
                    {
                        throw new NoViableAltException(LT(1), getFilename());
                    }
                     }
                }
                AST tmp21_AST = null;
                tmp21_AST = astFactory.create(LT(1));
                astFactory.addASTChild(ref currentAST, tmp21_AST);
                match(RPAREN);
                break;
            }
            case COMMA:
            case RBRACK:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        metadataItem_AST = currentAST.root;
        returnAST = metadataItem_AST;
    }
    
    public void expression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST expression_AST = null;
        AST s_AST = null;
        
        assignmentExpression();
        if (0 == inputState.guessing)
        {
            s_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        if (0==inputState.guessing)
        {
            expression_AST = (AST)currentAST.root;
            expression_AST = (AST) astFactory.make(astFactory.create(EXPR,"EXPR"), expression_AST);
                    setRegionInfo(expression_AST,s_AST,null);
            currentAST.root = expression_AST;
            if ( (null != expression_AST) && (null != expression_AST.getFirstChild()) )
                currentAST.child = expression_AST.getFirstChild();
            else
                currentAST.child = expression_AST;
            currentAST.advanceChildToEnd();
        }
        expression_AST = currentAST.root;
        returnAST = expression_AST;
    }
    
    public void extendsClause() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST extendsClause_AST = null;
        IToken  s = null;
        AST s_AST = null;
        AST e_AST = null;
        
        {
            switch ( LA(1) )
            {
            case LITERAL_extends:
            {
                s = LT(1);
                s_AST = astFactory.create(s);
                match(LITERAL_extends);
                identifier();
                if (0 == inputState.guessing)
                {
                    e_AST = (AST)returnAST;
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
                break;
            }
            case LCURLY:
            case LITERAL_implements:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        if (0==inputState.guessing)
        {
            extendsClause_AST = (AST)currentAST.root;
            extendsClause_AST = (AST) astFactory.make(astFactory.create(EXTENDS_CLAUSE,"EXTENDS_CLAUSE"), extendsClause_AST);
                    setRegionInfo(extendsClause_AST,s_AST,e_AST);
            currentAST.root = extendsClause_AST;
            if ( (null != extendsClause_AST) && (null != extendsClause_AST.getFirstChild()) )
                currentAST.child = extendsClause_AST.getFirstChild();
            else
                currentAST.child = extendsClause_AST;
            currentAST.advanceChildToEnd();
        }
        extendsClause_AST = currentAST.root;
        returnAST = extendsClause_AST;
    }
    
    public void implementsClause() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST implementsClause_AST = null;
        IToken  s = null;
        AST s_AST = null;
        AST e1_AST = null;
        AST e2_AST = null;
        
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==LITERAL_implements))
                {
                    s = LT(1);
                    s_AST = astFactory.create(s);
                    match(LITERAL_implements);
                    identifier();
                    if (0 == inputState.guessing)
                    {
                        e1_AST = (AST)returnAST;
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                    {    // ( ... )*
                        for (;;)
                        {
                            if ((LA(1)==COMMA))
                            {
                                match(COMMA);
                                identifier();
                                if (0 == inputState.guessing)
                                {
                                    e2_AST = (AST)returnAST;
                                    astFactory.addASTChild(ref currentAST, returnAST);
                                }
                            }
                            else
                            {
                                goto _loop23_breakloop;
                            }
                            
                        }
_loop23_breakloop:                      ;
                    }    // ( ... )*
                }
                else
                {
                    goto _loop24_breakloop;
                }
                
            }
_loop24_breakloop:          ;
        }    // ( ... )*
        if (0==inputState.guessing)
        {
            implementsClause_AST = (AST)currentAST.root;
            implementsClause_AST = (AST) astFactory.make(astFactory.create(IMPLEMENTS_CLAUSE,"IMPLEMENTS_CLAUSE"), implementsClause_AST);
                    setRegionInfo(implementsClause_AST,s_AST,e2_AST==null?e1_AST:e2_AST);
            currentAST.root = implementsClause_AST;
            if ( (null != implementsClause_AST) && (null != implementsClause_AST.getFirstChild()) )
                currentAST.child = implementsClause_AST.getFirstChild();
            else
                currentAST.child = implementsClause_AST;
            currentAST.advanceChildToEnd();
        }
        implementsClause_AST = currentAST.root;
        returnAST = implementsClause_AST;
    }
    
    public void typeBlock() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST typeBlock_AST = null;
        IToken  s = null;
        AST s_AST = null;
        AST m_AST = null;
        IToken  e = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        match(LCURLY);
        {    // ( ... )*
            for (;;)
            {
                switch ( LA(1) )
                {
                case LBRACK:
                {
                    metadataDefinition();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                    break;
                }
                case LITERAL_function:
                case LITERAL_var:
                case LITERAL_const:
                case LITERAL_public:
                case LITERAL_private:
                case LITERAL_protected:
                case LITERAL_internal:
                case LITERAL_static:
                case LITERAL_final:
                case LITERAL_enumerable:
                case LITERAL_explicit:
                case LITERAL_override:
                case LITERAL_dynamic:
                {
                    modifiers();
                    if (0 == inputState.guessing)
                    {
                        m_AST = (AST)returnAST;
                    }
                    {
                        switch ( LA(1) )
                        {
                        case LITERAL_var:
                        case LITERAL_const:
                        {
                            variableDefinition(m_AST);
                            if (0 == inputState.guessing)
                            {
                                astFactory.addASTChild(ref currentAST, returnAST);
                            }
                            break;
                        }
                        case LITERAL_function:
                        {
                            methodDefinition(m_AST);
                            if (0 == inputState.guessing)
                            {
                                astFactory.addASTChild(ref currentAST, returnAST);
                            }
                            break;
                        }
                        default:
                        {
                            throw new NoViableAltException(LT(1), getFilename());
                        }
                         }
                    }
                    break;
                }
                case SEMI:
                {
                    match(SEMI);
                    break;
                }
                default:
                {
                    goto _loop28_breakloop;
                }
                 }
            }
_loop28_breakloop:          ;
        }    // ( ... )*
        e = LT(1);
        e_AST = astFactory.create(e);
        match(RCURLY);
        if (0==inputState.guessing)
        {
            typeBlock_AST = (AST)currentAST.root;
            typeBlock_AST = (AST) astFactory.make(astFactory.create(TYPE_BLOCK,"TYPE_BLOCK"), typeBlock_AST);
                    setRegionInfo(typeBlock_AST,s_AST,e_AST);
            currentAST.root = typeBlock_AST;
            if ( (null != typeBlock_AST) && (null != typeBlock_AST.getFirstChild()) )
                currentAST.child = typeBlock_AST.getFirstChild();
            else
                currentAST.child = typeBlock_AST;
            currentAST.advanceChildToEnd();
        }
        typeBlock_AST = currentAST.root;
        returnAST = typeBlock_AST;
    }
    
    public void parameterDeclarationList() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST parameterDeclarationList_AST = null;
        IToken  s = null;
        AST s_AST = null;
        IToken  e = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        match(LPAREN);
        {
            switch ( LA(1) )
            {
            case IDENT:
            case LITERAL_const:
            case REST:
            {
                {
                    switch ( LA(1) )
                    {
                    case IDENT:
                    case LITERAL_const:
                    {
                        parameterDeclaration();
                        if (0 == inputState.guessing)
                        {
                            astFactory.addASTChild(ref currentAST, returnAST);
                        }
                        break;
                    }
                    case REST:
                    {
                        parameterRestDeclaration();
                        if (0 == inputState.guessing)
                        {
                            astFactory.addASTChild(ref currentAST, returnAST);
                        }
                        break;
                    }
                    default:
                    {
                        throw new NoViableAltException(LT(1), getFilename());
                    }
                     }
                }
                {    // ( ... )*
                    for (;;)
                    {
                        if ((LA(1)==COMMA))
                        {
                            match(COMMA);
                            {
                                switch ( LA(1) )
                                {
                                case IDENT:
                                case LITERAL_const:
                                {
                                    parameterDeclaration();
                                    if (0 == inputState.guessing)
                                    {
                                        astFactory.addASTChild(ref currentAST, returnAST);
                                    }
                                    break;
                                }
                                case REST:
                                {
                                    parameterRestDeclaration();
                                    if (0 == inputState.guessing)
                                    {
                                        astFactory.addASTChild(ref currentAST, returnAST);
                                    }
                                    break;
                                }
                                default:
                                {
                                    throw new NoViableAltException(LT(1), getFilename());
                                }
                                 }
                            }
                        }
                        else
                        {
                            goto _loop50_breakloop;
                        }
                        
                    }
_loop50_breakloop:                  ;
                }    // ( ... )*
                break;
            }
            case RPAREN:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        e = LT(1);
        e_AST = astFactory.create(e);
        match(RPAREN);
        if (0==inputState.guessing)
        {
            parameterDeclarationList_AST = (AST)currentAST.root;
            parameterDeclarationList_AST = (AST) astFactory.make(astFactory.create(PARAMS,"PARAMS"), parameterDeclarationList_AST);
                    setRegionInfo(parameterDeclarationList_AST,s_AST,e_AST);
            currentAST.root = parameterDeclarationList_AST;
            if ( (null != parameterDeclarationList_AST) && (null != parameterDeclarationList_AST.getFirstChild()) )
                currentAST.child = parameterDeclarationList_AST.getFirstChild();
            else
                currentAST.child = parameterDeclarationList_AST;
            currentAST.advanceChildToEnd();
        }
        parameterDeclarationList_AST = currentAST.root;
        returnAST = parameterDeclarationList_AST;
    }
    
    public void typeExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST typeExpression_AST = null;
        
        {
            switch ( LA(1) )
            {
            case COLON:
            {
                match(COLON);
                identifier();
                if (0 == inputState.guessing)
                {
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
                if (0==inputState.guessing)
                {
                    typeExpression_AST = (AST)currentAST.root;
                    typeExpression_AST = (AST) astFactory.make(astFactory.create(TYPE_SPEC,"TYPE_SPEC"), typeExpression_AST);
                    currentAST.root = typeExpression_AST;
                    if ( (null != typeExpression_AST) && (null != typeExpression_AST.getFirstChild()) )
                        currentAST.child = typeExpression_AST.getFirstChild();
                    else
                        currentAST.child = typeExpression_AST;
                    currentAST.advanceChildToEnd();
                }
                break;
            }
            case LCURLY:
            case SEMI:
            case RCURLY:
            case LITERAL_import:
            case LBRACK:
            case COMMA:
            case LPAREN:
            case RPAREN:
            case LITERAL_class:
            case LITERAL_interface:
            case LITERAL_function:
            case IDENT:
            case LITERAL_namespace:
            case LITERAL_var:
            case LITERAL_const:
            case ASSIGN:
            case LITERAL_while:
            case LITERAL_do:
            case LITERAL_with:
            case LITERAL_if:
            case LITERAL_else:
            case LITERAL_throw:
            case LITERAL_return:
            case LITERAL_continue:
            case LITERAL_break:
            case LITERAL_switch:
            case LITERAL_case:
            case LITERAL_default:
            case LITERAL_for:
            case LITERAL_in:
            case LITERAL_public:
            case LITERAL_private:
            case LITERAL_protected:
            case LITERAL_internal:
            case LITERAL_static:
            case LITERAL_final:
            case LITERAL_enumerable:
            case LITERAL_explicit:
            case LITERAL_override:
            case LITERAL_dynamic:
            case NUMBER:
            case PLUS:
            case MINUS:
            case INC:
            case DEC:
            case LITERAL_delete:
            case LITERAL_typeof:
            case LNOT:
            case BNOT:
            case LITERAL_null:
            case LITERAL_true:
            case LITERAL_false:
            case LITERAL_undefined:
            case STRING_LITERAL:
            case REGEX_LITERAL:
            case XML_LITERAL:
            case LITERAL_new:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        typeExpression_AST = currentAST.root;
        returnAST = typeExpression_AST;
    }
    
    public void block() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST block_AST = null;
        IToken  s = null;
        AST s_AST = null;
        IToken  e = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        match(LCURLY);
        {    // ( ... )*
            for (;;)
            {
                if ((tokenSet_1_.member(LA(1))))
                {
                    statement();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop58_breakloop;
                }
                
            }
_loop58_breakloop:          ;
        }    // ( ... )*
        e = LT(1);
        e_AST = astFactory.create(e);
        match(RCURLY);
        if (0==inputState.guessing)
        {
            block_AST = (AST)currentAST.root;
            block_AST = (AST) astFactory.make(astFactory.create(BLOCK,"BLOCK"), block_AST);
                    setRegionInfo(block_AST,s_AST,e_AST);
            currentAST.root = block_AST;
            if ( (null != block_AST) && (null != block_AST.getFirstChild()) )
                currentAST.child = block_AST.getFirstChild();
            else
                currentAST.child = block_AST;
            currentAST.advanceChildToEnd();
        }
        block_AST = currentAST.root;
        returnAST = block_AST;
    }
    
    public void variableDeclarator(
        AST mods
    ) //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST variableDeclarator_AST = null;
        IToken  s = null;
        AST s_AST = null;
        AST e1_AST = null;
        AST e2_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        astFactory.addASTChild(ref currentAST, s_AST);
        match(IDENT);
        typeExpression();
        if (0 == inputState.guessing)
        {
            e1_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {
            switch ( LA(1) )
            {
            case ASSIGN:
            {
                variableInitializer();
                if (0 == inputState.guessing)
                {
                    e2_AST = (AST)returnAST;
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
                break;
            }
            case LCURLY:
            case SEMI:
            case RCURLY:
            case LITERAL_import:
            case LBRACK:
            case COMMA:
            case LPAREN:
            case LITERAL_class:
            case LITERAL_interface:
            case LITERAL_function:
            case IDENT:
            case LITERAL_namespace:
            case LITERAL_var:
            case LITERAL_const:
            case LITERAL_while:
            case LITERAL_do:
            case LITERAL_with:
            case LITERAL_if:
            case LITERAL_else:
            case LITERAL_throw:
            case LITERAL_return:
            case LITERAL_continue:
            case LITERAL_break:
            case LITERAL_switch:
            case LITERAL_case:
            case LITERAL_default:
            case LITERAL_for:
            case LITERAL_in:
            case LITERAL_public:
            case LITERAL_private:
            case LITERAL_protected:
            case LITERAL_internal:
            case LITERAL_static:
            case LITERAL_final:
            case LITERAL_enumerable:
            case LITERAL_explicit:
            case LITERAL_override:
            case LITERAL_dynamic:
            case NUMBER:
            case PLUS:
            case MINUS:
            case INC:
            case DEC:
            case LITERAL_delete:
            case LITERAL_typeof:
            case LNOT:
            case BNOT:
            case LITERAL_null:
            case LITERAL_true:
            case LITERAL_false:
            case LITERAL_undefined:
            case STRING_LITERAL:
            case REGEX_LITERAL:
            case XML_LITERAL:
            case LITERAL_new:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        if (0==inputState.guessing)
        {
            variableDeclarator_AST = (AST)currentAST.root;
            variableDeclarator_AST = (AST) astFactory.make(astFactory.create(VARIABLE_DEF,"VARIABLE_DEF"), variableDeclarator_AST, mods);
                    setRegionInfo(variableDeclarator_AST,s_AST,e2_AST==null?e1_AST:e2_AST);
            currentAST.root = variableDeclarator_AST;
            if ( (null != variableDeclarator_AST) && (null != variableDeclarator_AST.getFirstChild()) )
                currentAST.child = variableDeclarator_AST.getFirstChild();
            else
                currentAST.child = variableDeclarator_AST;
            currentAST.advanceChildToEnd();
        }
        variableDeclarator_AST = currentAST.root;
        returnAST = variableDeclarator_AST;
    }
    
    public void variableInitializer() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST variableInitializer_AST = null;
        IToken  s = null;
        AST s_AST = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        astFactory.makeASTRoot(ref currentAST, s_AST);
        match(ASSIGN);
        expression();
        if (0 == inputState.guessing)
        {
            e_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        if (0==inputState.guessing)
        {
            variableInitializer_AST = (AST)currentAST.root;
            setRegionInfo(variableInitializer_AST,s_AST,e_AST);
        }
        variableInitializer_AST = currentAST.root;
        returnAST = variableInitializer_AST;
    }
    
    public void declaration() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST declaration_AST = null;
        
        match(LITERAL_var);
        variableDeclarator(null);
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==COMMA))
                {
                    match(COMMA);
                    variableDeclarator(null);
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop43_breakloop;
                }
                
            }
_loop43_breakloop:          ;
        }    // ( ... )*
        declaration_AST = currentAST.root;
        returnAST = declaration_AST;
    }
    
    public void parameterDeclaration() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST parameterDeclaration_AST = null;
        
        {
            switch ( LA(1) )
            {
            case LITERAL_const:
            {
                AST tmp28_AST = null;
                tmp28_AST = astFactory.create(LT(1));
                astFactory.addASTChild(ref currentAST, tmp28_AST);
                match(LITERAL_const);
                break;
            }
            case IDENT:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        AST tmp29_AST = null;
        tmp29_AST = astFactory.create(LT(1));
        astFactory.addASTChild(ref currentAST, tmp29_AST);
        match(IDENT);
        typeExpression();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {
            switch ( LA(1) )
            {
            case ASSIGN:
            {
                AST tmp30_AST = null;
                tmp30_AST = astFactory.create(LT(1));
                astFactory.addASTChild(ref currentAST, tmp30_AST);
                match(ASSIGN);
                assignmentExpression();
                if (0 == inputState.guessing)
                {
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
                break;
            }
            case COMMA:
            case RPAREN:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        if (0==inputState.guessing)
        {
            parameterDeclaration_AST = (AST)currentAST.root;
            parameterDeclaration_AST = (AST) astFactory.make(astFactory.create(PARAM,"PARAM"), parameterDeclaration_AST);
            currentAST.root = parameterDeclaration_AST;
            if ( (null != parameterDeclaration_AST) && (null != parameterDeclaration_AST.getFirstChild()) )
                currentAST.child = parameterDeclaration_AST.getFirstChild();
            else
                currentAST.child = parameterDeclaration_AST;
            currentAST.advanceChildToEnd();
        }
        parameterDeclaration_AST = currentAST.root;
        returnAST = parameterDeclaration_AST;
    }
    
    public void parameterRestDeclaration() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST parameterRestDeclaration_AST = null;
        
        AST tmp31_AST = null;
        tmp31_AST = astFactory.create(LT(1));
        astFactory.addASTChild(ref currentAST, tmp31_AST);
        match(REST);
        {
            switch ( LA(1) )
            {
            case LITERAL_const:
            {
                AST tmp32_AST = null;
                tmp32_AST = astFactory.create(LT(1));
                astFactory.addASTChild(ref currentAST, tmp32_AST);
                match(LITERAL_const);
                break;
            }
            case IDENT:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        AST tmp33_AST = null;
        tmp33_AST = astFactory.create(LT(1));
        astFactory.addASTChild(ref currentAST, tmp33_AST);
        match(IDENT);
        if (0==inputState.guessing)
        {
            parameterRestDeclaration_AST = (AST)currentAST.root;
            parameterRestDeclaration_AST = (AST) astFactory.make(astFactory.create(PARAM,"PARAM"), parameterRestDeclaration_AST);
            currentAST.root = parameterRestDeclaration_AST;
            if ( (null != parameterRestDeclaration_AST) && (null != parameterRestDeclaration_AST.getFirstChild()) )
                currentAST.child = parameterRestDeclaration_AST.getFirstChild();
            else
                currentAST.child = parameterRestDeclaration_AST;
            currentAST.advanceChildToEnd();
        }
        parameterRestDeclaration_AST = currentAST.root;
        returnAST = parameterRestDeclaration_AST;
    }
    
    public void assignmentExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST assignmentExpression_AST = null;
        AST s_AST = null;
        AST e_AST = null;
        
        conditionalExpression();
        if (0 == inputState.guessing)
        {
            s_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {
            bool synPredMatched147 = false;
            if (((tokenSet_2_.member(LA(1))) && (tokenSet_3_.member(LA(2)))))
            {
                int _m147 = mark();
                synPredMatched147 = true;
                inputState.guessing++;
                try {
                    {
                        {
                            switch ( LA(1) )
                            {
                            case ASSIGN:
                            {
                                match(ASSIGN);
                                break;
                            }
                            case STAR_ASSIGN:
                            {
                                match(STAR_ASSIGN);
                                break;
                            }
                            case DIV_ASSIGN:
                            {
                                match(DIV_ASSIGN);
                                break;
                            }
                            case MOD_ASSIGN:
                            {
                                match(MOD_ASSIGN);
                                break;
                            }
                            case PLUS_ASSIGN:
                            {
                                match(PLUS_ASSIGN);
                                break;
                            }
                            case MINUS_ASSIGN:
                            {
                                match(MINUS_ASSIGN);
                                break;
                            }
                            case SL_ASSIGN:
                            {
                                match(SL_ASSIGN);
                                break;
                            }
                            case SR_ASSIGN:
                            {
                                match(SR_ASSIGN);
                                break;
                            }
                            case BSR_ASSIGN:
                            {
                                match(BSR_ASSIGN);
                                break;
                            }
                            case BAND_ASSIGN:
                            {
                                match(BAND_ASSIGN);
                                break;
                            }
                            case BXOR_ASSIGN:
                            {
                                match(BXOR_ASSIGN);
                                break;
                            }
                            case BOR_ASSIGN:
                            {
                                match(BOR_ASSIGN);
                                break;
                            }
                            case LAND_ASSIGN:
                            {
                                match(LAND_ASSIGN);
                                break;
                            }
                            case LOR_ASSIGN:
                            {
                                match(LOR_ASSIGN);
                                break;
                            }
                            default:
                            {
                                throw new NoViableAltException(LT(1), getFilename());
                            }
                             }
                        }
                        matchNot(ASSIGN);
                    }
                }
                catch (RecognitionException)
                {
                    synPredMatched147 = false;
                }
                rewind(_m147);
                inputState.guessing--;
            }
            if ( synPredMatched147 )
            {
                {
                    switch ( LA(1) )
                    {
                    case ASSIGN:
                    {
                        AST tmp34_AST = null;
                        tmp34_AST = astFactory.create(LT(1));
                        astFactory.makeASTRoot(ref currentAST, tmp34_AST);
                        match(ASSIGN);
                        break;
                    }
                    case STAR_ASSIGN:
                    {
                        AST tmp35_AST = null;
                        tmp35_AST = astFactory.create(LT(1));
                        astFactory.makeASTRoot(ref currentAST, tmp35_AST);
                        match(STAR_ASSIGN);
                        break;
                    }
                    case DIV_ASSIGN:
                    {
                        AST tmp36_AST = null;
                        tmp36_AST = astFactory.create(LT(1));
                        astFactory.makeASTRoot(ref currentAST, tmp36_AST);
                        match(DIV_ASSIGN);
                        break;
                    }
                    case MOD_ASSIGN:
                    {
                        AST tmp37_AST = null;
                        tmp37_AST = astFactory.create(LT(1));
                        astFactory.makeASTRoot(ref currentAST, tmp37_AST);
                        match(MOD_ASSIGN);
                        break;
                    }
                    case PLUS_ASSIGN:
                    {
                        AST tmp38_AST = null;
                        tmp38_AST = astFactory.create(LT(1));
                        astFactory.makeASTRoot(ref currentAST, tmp38_AST);
                        match(PLUS_ASSIGN);
                        break;
                    }
                    case MINUS_ASSIGN:
                    {
                        AST tmp39_AST = null;
                        tmp39_AST = astFactory.create(LT(1));
                        astFactory.makeASTRoot(ref currentAST, tmp39_AST);
                        match(MINUS_ASSIGN);
                        break;
                    }
                    case SL_ASSIGN:
                    {
                        AST tmp40_AST = null;
                        tmp40_AST = astFactory.create(LT(1));
                        astFactory.makeASTRoot(ref currentAST, tmp40_AST);
                        match(SL_ASSIGN);
                        break;
                    }
                    case SR_ASSIGN:
                    {
                        AST tmp41_AST = null;
                        tmp41_AST = astFactory.create(LT(1));
                        astFactory.makeASTRoot(ref currentAST, tmp41_AST);
                        match(SR_ASSIGN);
                        break;
                    }
                    case BSR_ASSIGN:
                    {
                        AST tmp42_AST = null;
                        tmp42_AST = astFactory.create(LT(1));
                        astFactory.makeASTRoot(ref currentAST, tmp42_AST);
                        match(BSR_ASSIGN);
                        break;
                    }
                    case BAND_ASSIGN:
                    {
                        AST tmp43_AST = null;
                        tmp43_AST = astFactory.create(LT(1));
                        astFactory.makeASTRoot(ref currentAST, tmp43_AST);
                        match(BAND_ASSIGN);
                        break;
                    }
                    case BXOR_ASSIGN:
                    {
                        AST tmp44_AST = null;
                        tmp44_AST = astFactory.create(LT(1));
                        astFactory.makeASTRoot(ref currentAST, tmp44_AST);
                        match(BXOR_ASSIGN);
                        break;
                    }
                    case BOR_ASSIGN:
                    {
                        AST tmp45_AST = null;
                        tmp45_AST = astFactory.create(LT(1));
                        astFactory.makeASTRoot(ref currentAST, tmp45_AST);
                        match(BOR_ASSIGN);
                        break;
                    }
                    case LAND_ASSIGN:
                    {
                        AST tmp46_AST = null;
                        tmp46_AST = astFactory.create(LT(1));
                        astFactory.makeASTRoot(ref currentAST, tmp46_AST);
                        match(LAND_ASSIGN);
                        break;
                    }
                    case LOR_ASSIGN:
                    {
                        AST tmp47_AST = null;
                        tmp47_AST = astFactory.create(LT(1));
                        astFactory.makeASTRoot(ref currentAST, tmp47_AST);
                        match(LOR_ASSIGN);
                        break;
                    }
                    default:
                    {
                        throw new NoViableAltException(LT(1), getFilename());
                    }
                     }
                }
                assignmentExpression();
                if (0 == inputState.guessing)
                {
                    e_AST = (AST)returnAST;
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
                if (0==inputState.guessing)
                {
                    assignmentExpression_AST = (AST)currentAST.root;
                    setRegionInfo(assignmentExpression_AST,s_AST,e_AST);
                }
            }
            else if ((tokenSet_4_.member(LA(1))) && (tokenSet_5_.member(LA(2)))) {
            }
            else
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
            
        }
        assignmentExpression_AST = currentAST.root;
        returnAST = assignmentExpression_AST;
    }
    
    public void statement() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST statement_AST = null;
        AST s1_AST = null;
        IToken  e1 = null;
        AST e1_AST = null;
        AST s2_AST = null;
        IToken  e2 = null;
        AST e2_AST = null;
        
        switch ( LA(1) )
        {
        case LITERAL_var:
        {
            declaration();
            if (0 == inputState.guessing)
            {
                s1_AST = (AST)returnAST;
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            {
                if (((LA(1)==SEMI) && (tokenSet_6_.member(LA(2))))&&(LA(1)==SEMI))
                {
                    {
                        e1 = LT(1);
                        e1_AST = astFactory.create(e1);
                        match(SEMI);
                    }
                }
                else if ((tokenSet_6_.member(LA(1))) && (tokenSet_7_.member(LA(2)))) {
                }
                else
                {
                    throw new NoViableAltException(LT(1), getFilename());
                }
                
            }
            if (0==inputState.guessing)
            {
                statement_AST = (AST)currentAST.root;
                setRegionInfo(statement_AST,s1_AST,e1_AST);
            }
            statement_AST = currentAST.root;
            break;
        }
        case LITERAL_if:
        {
            ifStatement();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            statement_AST = currentAST.root;
            break;
        }
        case LITERAL_for:
        {
            forStatement();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            statement_AST = currentAST.root;
            break;
        }
        case LITERAL_while:
        {
            AST tmp48_AST = null;
            tmp48_AST = astFactory.create(LT(1));
            astFactory.makeASTRoot(ref currentAST, tmp48_AST);
            match(LITERAL_while);
            match(LPAREN);
            expression();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            match(RPAREN);
            statement();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            statement_AST = currentAST.root;
            break;
        }
        case LITERAL_do:
        {
            AST tmp51_AST = null;
            tmp51_AST = astFactory.create(LT(1));
            astFactory.makeASTRoot(ref currentAST, tmp51_AST);
            match(LITERAL_do);
            statement();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            match(LITERAL_while);
            match(LPAREN);
            expression();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            match(RPAREN);
            statement_AST = currentAST.root;
            break;
        }
        case LITERAL_with:
        {
            AST tmp55_AST = null;
            tmp55_AST = astFactory.create(LT(1));
            astFactory.makeASTRoot(ref currentAST, tmp55_AST);
            match(LITERAL_with);
            match(LPAREN);
            expression();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            match(RPAREN);
            statement();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            statement_AST = currentAST.root;
            break;
        }
        case LITERAL_switch:
        {
            switchStatement();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            statement_AST = currentAST.root;
            break;
        }
        case LITERAL_break:
        {
            breakStatement();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            statement_AST = currentAST.root;
            break;
        }
        case LITERAL_continue:
        {
            continueStatement();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            statement_AST = currentAST.root;
            break;
        }
        case LITERAL_return:
        {
            returnStatement();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            statement_AST = currentAST.root;
            break;
        }
        case LITERAL_throw:
        {
            throwStatement();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            statement_AST = currentAST.root;
            break;
        }
        case SEMI:
        {
            match(SEMI);
            statement_AST = currentAST.root;
            break;
        }
        default:
            if (((LA(1)==LCURLY) && (tokenSet_8_.member(LA(2))))&&(LA(1)==LCURLY))
            {
                {
                    block();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                statement_AST = currentAST.root;
            }
            else if ((tokenSet_3_.member(LA(1))) && (tokenSet_9_.member(LA(2)))) {
                assignmentExpression();
                if (0 == inputState.guessing)
                {
                    s2_AST = (AST)returnAST;
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
                {
                    if (((LA(1)==SEMI) && (tokenSet_6_.member(LA(2))))&&(LA(1)==SEMI))
                    {
                        {
                            e2 = LT(1);
                            e2_AST = astFactory.create(e2);
                            match(SEMI);
                        }
                    }
                    else if ((tokenSet_6_.member(LA(1))) && (tokenSet_7_.member(LA(2)))) {
                    }
                    else
                    {
                        throw new NoViableAltException(LT(1), getFilename());
                    }
                    
                }
                if (0==inputState.guessing)
                {
                    statement_AST = (AST)currentAST.root;
                    statement_AST = (AST) astFactory.make(astFactory.create(EXPR_STMNT,"EXPR_STMNT"), statement_AST);
                            setRegionInfo(statement_AST,s2_AST,e2_AST);
                    currentAST.root = statement_AST;
                    if ( (null != statement_AST) && (null != statement_AST.getFirstChild()) )
                        currentAST.child = statement_AST.getFirstChild();
                    else
                        currentAST.child = statement_AST;
                    currentAST.advanceChildToEnd();
                }
                statement_AST = currentAST.root;
            }
        else
        {
            throw new NoViableAltException(LT(1), getFilename());
        }
        break; }
        returnAST = statement_AST;
    }
    
    public void ifStatement() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST ifStatement_AST = null;
        IToken  s = null;
        AST s_AST = null;
        AST e1_AST = null;
        AST e2_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        astFactory.makeASTRoot(ref currentAST, s_AST);
        match(LITERAL_if);
        match(LPAREN);
        expression();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        match(RPAREN);
        statement();
        if (0 == inputState.guessing)
        {
            e1_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==LITERAL_else) && (tokenSet_1_.member(LA(2))))
                {
                    elseStatement();
                    if (0 == inputState.guessing)
                    {
                        e2_AST = (AST)returnAST;
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop67_breakloop;
                }
                
            }
_loop67_breakloop:          ;
        }    // ( ... )*
        if (0==inputState.guessing)
        {
            ifStatement_AST = (AST)currentAST.root;
            setRegionInfo(ifStatement_AST,s_AST,e2_AST==null?e1_AST:e2_AST);
        }
        ifStatement_AST = currentAST.root;
        returnAST = ifStatement_AST;
    }
    
    public void forStatement() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST forStatement_AST = null;
        IToken  s = null;
        AST s_AST = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        astFactory.makeASTRoot(ref currentAST, s_AST);
        match(LITERAL_for);
        {
            bool synPredMatched96 = false;
            if (((LA(1)==LPAREN) && (tokenSet_10_.member(LA(2)))))
            {
                int _m96 = mark();
                synPredMatched96 = true;
                inputState.guessing++;
                try {
                    {
                        match(LPAREN);
                        forInit();
                        match(SEMI);
                    }
                }
                catch (RecognitionException)
                {
                    synPredMatched96 = false;
                }
                rewind(_m96);
                inputState.guessing--;
            }
            if ( synPredMatched96 )
            {
                traditionalForClause();
                if (0 == inputState.guessing)
                {
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
            }
            else if ((LA(1)==LITERAL_each)) {
                {
                    AST tmp61_AST = null;
                    tmp61_AST = astFactory.create(LT(1));
                    astFactory.addASTChild(ref currentAST, tmp61_AST);
                    match(LITERAL_each);
                    forInClause();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
            }
            else if ((LA(1)==LPAREN) && (LA(2)==LITERAL_var)) {
                forInClause();
                if (0 == inputState.guessing)
                {
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
            }
            else
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
            
        }
        statement();
        if (0 == inputState.guessing)
        {
            e_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        if (0==inputState.guessing)
        {
            forStatement_AST = (AST)currentAST.root;
            setRegionInfo(forStatement_AST,s_AST,e_AST);
        }
        forStatement_AST = currentAST.root;
        returnAST = forStatement_AST;
    }
    
    public void switchStatement() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST switchStatement_AST = null;
        IToken  s = null;
        AST s_AST = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        astFactory.makeASTRoot(ref currentAST, s_AST);
        match(LITERAL_switch);
        match(LPAREN);
        expression();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        match(RPAREN);
        switchBlock();
        if (0 == inputState.guessing)
        {
            e_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        if (0==inputState.guessing)
        {
            switchStatement_AST = (AST)currentAST.root;
            setRegionInfo(switchStatement_AST,s_AST,e_AST);
        }
        switchStatement_AST = currentAST.root;
        returnAST = switchStatement_AST;
    }
    
    public void breakStatement() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST breakStatement_AST = null;
        IToken  s = null;
        AST s_AST = null;
        IToken  e = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        astFactory.makeASTRoot(ref currentAST, s_AST);
        match(LITERAL_break);
        {
            if (((LA(1)==SEMI) && (tokenSet_6_.member(LA(2))))&&(LA(1)==SEMI))
            {
                {
                    e = LT(1);
                    e_AST = astFactory.create(e);
                    match(SEMI);
                }
            }
            else if ((tokenSet_6_.member(LA(1))) && (tokenSet_7_.member(LA(2)))) {
            }
            else
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
            
        }
        if (0==inputState.guessing)
        {
            breakStatement_AST = (AST)currentAST.root;
            setRegionInfo(breakStatement_AST,s_AST,e_AST);
        }
        breakStatement_AST = currentAST.root;
        returnAST = breakStatement_AST;
    }
    
    public void continueStatement() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST continueStatement_AST = null;
        IToken  s = null;
        AST s_AST = null;
        IToken  e = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        astFactory.makeASTRoot(ref currentAST, s_AST);
        match(LITERAL_continue);
        {
            if (((LA(1)==SEMI) && (tokenSet_6_.member(LA(2))))&&(LA(1)==SEMI))
            {
                {
                    e = LT(1);
                    e_AST = astFactory.create(e);
                    match(SEMI);
                }
            }
            else if ((tokenSet_6_.member(LA(1))) && (tokenSet_7_.member(LA(2)))) {
            }
            else
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
            
        }
        if (0==inputState.guessing)
        {
            continueStatement_AST = (AST)currentAST.root;
            setRegionInfo(continueStatement_AST,s_AST,e_AST);
        }
        continueStatement_AST = currentAST.root;
        returnAST = continueStatement_AST;
    }
    
    public void returnStatement() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST returnStatement_AST = null;
        IToken  s = null;
        AST s_AST = null;
        AST e1_AST = null;
        IToken  e2 = null;
        AST e2_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        astFactory.makeASTRoot(ref currentAST, s_AST);
        match(LITERAL_return);
        {
            if ((tokenSet_3_.member(LA(1))) && (tokenSet_9_.member(LA(2))))
            {
                expression();
                if (0 == inputState.guessing)
                {
                    e1_AST = (AST)returnAST;
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
            }
            else if ((tokenSet_6_.member(LA(1))) && (tokenSet_7_.member(LA(2)))) {
            }
            else
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
            
        }
        {
            if (((LA(1)==SEMI) && (tokenSet_6_.member(LA(2))))&&(LA(1)==SEMI))
            {
                {
                    e2 = LT(1);
                    e2_AST = astFactory.create(e2);
                    match(SEMI);
                }
            }
            else if ((tokenSet_6_.member(LA(1))) && (tokenSet_7_.member(LA(2)))) {
            }
            else
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
            
        }
        if (0==inputState.guessing)
        {
            returnStatement_AST = (AST)currentAST.root;
            setRegionInfo(returnStatement_AST,s_AST,e2_AST==null?e1_AST:e2_AST);
        }
        returnStatement_AST = currentAST.root;
        returnAST = returnStatement_AST;
    }
    
    public void throwStatement() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST throwStatement_AST = null;
        IToken  s = null;
        AST s_AST = null;
        AST e1_AST = null;
        IToken  e2 = null;
        AST e2_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        astFactory.makeASTRoot(ref currentAST, s_AST);
        match(LITERAL_throw);
        expression();
        if (0 == inputState.guessing)
        {
            e1_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {
            if (((LA(1)==SEMI) && (tokenSet_6_.member(LA(2))))&&(LA(1)==SEMI))
            {
                {
                    e2 = LT(1);
                    e2_AST = astFactory.create(e2);
                    match(SEMI);
                }
            }
            else if ((tokenSet_6_.member(LA(1))) && (tokenSet_7_.member(LA(2)))) {
            }
            else
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
            
        }
        if (0==inputState.guessing)
        {
            throwStatement_AST = (AST)currentAST.root;
            setRegionInfo(throwStatement_AST,s_AST,e2_AST==null?e1_AST:e2_AST);
        }
        throwStatement_AST = currentAST.root;
        returnAST = throwStatement_AST;
    }
    
    public void elseStatement() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST elseStatement_AST = null;
        IToken  s = null;
        AST s_AST = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        astFactory.makeASTRoot(ref currentAST, s_AST);
        match(LITERAL_else);
        statement();
        if (0 == inputState.guessing)
        {
            e_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        if (0==inputState.guessing)
        {
            elseStatement_AST = (AST)currentAST.root;
            setRegionInfo(elseStatement_AST,s_AST,e_AST);
        }
        elseStatement_AST = currentAST.root;
        returnAST = elseStatement_AST;
    }
    
    public void switchBlock() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST switchBlock_AST = null;
        IToken  s = null;
        AST s_AST = null;
        IToken  e = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        match(LCURLY);
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==LITERAL_case))
                {
                    caseStatement();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop85_breakloop;
                }
                
            }
_loop85_breakloop:          ;
        }    // ( ... )*
        {
            switch ( LA(1) )
            {
            case LITERAL_default:
            {
                defaultStatement();
                if (0 == inputState.guessing)
                {
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
                break;
            }
            case RCURLY:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        e = LT(1);
        e_AST = astFactory.create(e);
        match(RCURLY);
        if (0==inputState.guessing)
        {
            switchBlock_AST = (AST)currentAST.root;
            switchBlock_AST = (AST) astFactory.make(astFactory.create(BLOCK,"BLOCK"), switchBlock_AST);
                    setRegionInfo(switchBlock_AST,s_AST,e_AST);
            currentAST.root = switchBlock_AST;
            if ( (null != switchBlock_AST) && (null != switchBlock_AST.getFirstChild()) )
                currentAST.child = switchBlock_AST.getFirstChild();
            else
                currentAST.child = switchBlock_AST;
            currentAST.advanceChildToEnd();
        }
        switchBlock_AST = currentAST.root;
        returnAST = switchBlock_AST;
    }
    
    public void caseStatement() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST caseStatement_AST = null;
        IToken  s = null;
        AST s_AST = null;
        IToken  e1 = null;
        AST e1_AST = null;
        AST e2_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        astFactory.makeASTRoot(ref currentAST, s_AST);
        match(LITERAL_case);
        expression();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        e1 = LT(1);
        e1_AST = astFactory.create(e1);
        match(COLON);
        {    // ( ... )*
            for (;;)
            {
                if ((tokenSet_1_.member(LA(1))))
                {
                    statement();
                    if (0 == inputState.guessing)
                    {
                        e2_AST = (AST)returnAST;
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop89_breakloop;
                }
                
            }
_loop89_breakloop:          ;
        }    // ( ... )*
        if (0==inputState.guessing)
        {
            caseStatement_AST = (AST)currentAST.root;
            setRegionInfo(caseStatement_AST,s_AST,e2_AST==null?e1_AST:e2_AST);
        }
        caseStatement_AST = currentAST.root;
        returnAST = caseStatement_AST;
    }
    
    public void defaultStatement() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST defaultStatement_AST = null;
        IToken  s = null;
        AST s_AST = null;
        IToken  e1 = null;
        AST e1_AST = null;
        AST e2_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        astFactory.makeASTRoot(ref currentAST, s_AST);
        match(LITERAL_default);
        e1 = LT(1);
        e1_AST = astFactory.create(e1);
        match(COLON);
        {    // ( ... )*
            for (;;)
            {
                if ((tokenSet_1_.member(LA(1))))
                {
                    statement();
                    if (0 == inputState.guessing)
                    {
                        e2_AST = (AST)returnAST;
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop92_breakloop;
                }
                
            }
_loop92_breakloop:          ;
        }    // ( ... )*
        if (0==inputState.guessing)
        {
            defaultStatement_AST = (AST)currentAST.root;
            setRegionInfo(defaultStatement_AST,s_AST,e2_AST==null?e1_AST:e2_AST);
        }
        defaultStatement_AST = currentAST.root;
        returnAST = defaultStatement_AST;
    }
    
    public void forInit() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST forInit_AST = null;
        AST s1_AST = null;
        AST s2_AST = null;
        
        {
            switch ( LA(1) )
            {
            case LITERAL_var:
            {
                declaration();
                if (0 == inputState.guessing)
                {
                    s1_AST = (AST)returnAST;
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
                break;
            }
            case LCURLY:
            case LBRACK:
            case LPAREN:
            case LITERAL_function:
            case IDENT:
            case NUMBER:
            case PLUS:
            case MINUS:
            case INC:
            case DEC:
            case LITERAL_delete:
            case LITERAL_typeof:
            case LNOT:
            case BNOT:
            case LITERAL_null:
            case LITERAL_true:
            case LITERAL_false:
            case LITERAL_undefined:
            case STRING_LITERAL:
            case REGEX_LITERAL:
            case XML_LITERAL:
            case LITERAL_new:
            {
                expressionList();
                if (0 == inputState.guessing)
                {
                    s2_AST = (AST)returnAST;
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
                break;
            }
            case SEMI:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        if (0==inputState.guessing)
        {
            forInit_AST = (AST)currentAST.root;
            forInit_AST = (AST) astFactory.make(astFactory.create(FOR_INIT,"FOR_INIT"), forInit_AST);
                    setRegionInfo(forInit_AST,s2_AST==null?s1_AST:s2_AST,null);
            currentAST.root = forInit_AST;
            if ( (null != forInit_AST) && (null != forInit_AST.getFirstChild()) )
                currentAST.child = forInit_AST.getFirstChild();
            else
                currentAST.child = forInit_AST;
            currentAST.advanceChildToEnd();
        }
        forInit_AST = currentAST.root;
        returnAST = forInit_AST;
    }
    
    public void traditionalForClause() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST traditionalForClause_AST = null;
        IToken  s = null;
        AST s_AST = null;
        IToken  e = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        match(LPAREN);
        forInit();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        match(SEMI);
        forCond();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        match(SEMI);
        forIter();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        e = LT(1);
        e_AST = astFactory.create(e);
        match(RPAREN);
        if (0==inputState.guessing)
        {
            traditionalForClause_AST = (AST)currentAST.root;
            setRegionInfo(traditionalForClause_AST,s_AST,e_AST);
        }
        traditionalForClause_AST = currentAST.root;
        returnAST = traditionalForClause_AST;
    }
    
    public void forInClause() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST forInClause_AST = null;
        IToken  s = null;
        AST s_AST = null;
        IToken  e = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        match(LPAREN);
        declaration();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        AST tmp66_AST = null;
        tmp66_AST = astFactory.create(LT(1));
        astFactory.addASTChild(ref currentAST, tmp66_AST);
        match(LITERAL_in);
        expression();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        e = LT(1);
        e_AST = astFactory.create(e);
        match(RPAREN);
        if (0==inputState.guessing)
        {
            forInClause_AST = (AST)currentAST.root;
            setRegionInfo(forInClause_AST,s_AST,e_AST);
        }
        forInClause_AST = currentAST.root;
        returnAST = forInClause_AST;
    }
    
    public void forCond() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST forCond_AST = null;
        AST s_AST = null;
        
        {
            switch ( LA(1) )
            {
            case LCURLY:
            case LBRACK:
            case LPAREN:
            case LITERAL_function:
            case IDENT:
            case NUMBER:
            case PLUS:
            case MINUS:
            case INC:
            case DEC:
            case LITERAL_delete:
            case LITERAL_typeof:
            case LNOT:
            case BNOT:
            case LITERAL_null:
            case LITERAL_true:
            case LITERAL_false:
            case LITERAL_undefined:
            case STRING_LITERAL:
            case REGEX_LITERAL:
            case XML_LITERAL:
            case LITERAL_new:
            {
                expression();
                if (0 == inputState.guessing)
                {
                    s_AST = (AST)returnAST;
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
                break;
            }
            case SEMI:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        if (0==inputState.guessing)
        {
            forCond_AST = (AST)currentAST.root;
            forCond_AST = (AST) astFactory.make(astFactory.create(FOR_CONDITION,"FOR_CONDITION"), forCond_AST);
                    setRegionInfo(forCond_AST,s_AST,null);
            currentAST.root = forCond_AST;
            if ( (null != forCond_AST) && (null != forCond_AST.getFirstChild()) )
                currentAST.child = forCond_AST.getFirstChild();
            else
                currentAST.child = forCond_AST;
            currentAST.advanceChildToEnd();
        }
        forCond_AST = currentAST.root;
        returnAST = forCond_AST;
    }
    
    public void forIter() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST forIter_AST = null;
        AST s_AST = null;
        
        {
            switch ( LA(1) )
            {
            case LCURLY:
            case LBRACK:
            case LPAREN:
            case LITERAL_function:
            case IDENT:
            case NUMBER:
            case PLUS:
            case MINUS:
            case INC:
            case DEC:
            case LITERAL_delete:
            case LITERAL_typeof:
            case LNOT:
            case BNOT:
            case LITERAL_null:
            case LITERAL_true:
            case LITERAL_false:
            case LITERAL_undefined:
            case STRING_LITERAL:
            case REGEX_LITERAL:
            case XML_LITERAL:
            case LITERAL_new:
            {
                expressionList();
                if (0 == inputState.guessing)
                {
                    s_AST = (AST)returnAST;
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
                break;
            }
            case RPAREN:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        if (0==inputState.guessing)
        {
            forIter_AST = (AST)currentAST.root;
            forIter_AST = (AST) astFactory.make(astFactory.create(FOR_ITERATOR,"FOR_ITERATOR"), forIter_AST);
                    setRegionInfo(forIter_AST,s_AST,null);
            currentAST.root = forIter_AST;
            if ( (null != forIter_AST) && (null != forIter_AST.getFirstChild()) )
                currentAST.child = forIter_AST.getFirstChild();
            else
                currentAST.child = forIter_AST;
            currentAST.advanceChildToEnd();
        }
        forIter_AST = currentAST.root;
        returnAST = forIter_AST;
    }
    
    public void expressionList() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST expressionList_AST = null;
        AST s_AST = null;
        AST e_AST = null;
        
        expression();
        if (0 == inputState.guessing)
        {
            s_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==COMMA))
                {
                    match(COMMA);
                    expression();
                    if (0 == inputState.guessing)
                    {
                        e_AST = (AST)returnAST;
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop142_breakloop;
                }
                
            }
_loop142_breakloop:         ;
        }    // ( ... )*
        if (0==inputState.guessing)
        {
            expressionList_AST = (AST)currentAST.root;
            expressionList_AST = (AST) astFactory.make(astFactory.create(ELIST,"ELIST"), expressionList_AST);
                    setRegionInfo(expressionList_AST,s_AST,e_AST);
            currentAST.root = expressionList_AST;
            if ( (null != expressionList_AST) && (null != expressionList_AST.getFirstChild()) )
                currentAST.child = expressionList_AST.getFirstChild();
            else
                currentAST.child = expressionList_AST;
            currentAST.advanceChildToEnd();
        }
        expressionList_AST = currentAST.root;
        returnAST = expressionList_AST;
    }
    
    public void modifier() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST modifier_AST = null;
        
        switch ( LA(1) )
        {
        case LITERAL_public:
        {
            AST tmp68_AST = null;
            tmp68_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp68_AST);
            match(LITERAL_public);
            modifier_AST = currentAST.root;
            break;
        }
        case LITERAL_private:
        {
            AST tmp69_AST = null;
            tmp69_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp69_AST);
            match(LITERAL_private);
            modifier_AST = currentAST.root;
            break;
        }
        case LITERAL_protected:
        {
            AST tmp70_AST = null;
            tmp70_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp70_AST);
            match(LITERAL_protected);
            modifier_AST = currentAST.root;
            break;
        }
        case LITERAL_internal:
        {
            AST tmp71_AST = null;
            tmp71_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp71_AST);
            match(LITERAL_internal);
            modifier_AST = currentAST.root;
            break;
        }
        case LITERAL_static:
        {
            AST tmp72_AST = null;
            tmp72_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp72_AST);
            match(LITERAL_static);
            modifier_AST = currentAST.root;
            break;
        }
        case LITERAL_final:
        {
            AST tmp73_AST = null;
            tmp73_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp73_AST);
            match(LITERAL_final);
            modifier_AST = currentAST.root;
            break;
        }
        case LITERAL_enumerable:
        {
            AST tmp74_AST = null;
            tmp74_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp74_AST);
            match(LITERAL_enumerable);
            modifier_AST = currentAST.root;
            break;
        }
        case LITERAL_explicit:
        {
            AST tmp75_AST = null;
            tmp75_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp75_AST);
            match(LITERAL_explicit);
            modifier_AST = currentAST.root;
            break;
        }
        case LITERAL_override:
        {
            AST tmp76_AST = null;
            tmp76_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp76_AST);
            match(LITERAL_override);
            modifier_AST = currentAST.root;
            break;
        }
        case LITERAL_dynamic:
        {
            AST tmp77_AST = null;
            tmp77_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp77_AST);
            match(LITERAL_dynamic);
            modifier_AST = currentAST.root;
            break;
        }
        default:
        {
            throw new NoViableAltException(LT(1), getFilename());
        }
         }
        returnAST = modifier_AST;
    }
    
    public void arguments() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST arguments_AST = null;
        
        {
            switch ( LA(1) )
            {
            case LCURLY:
            case LBRACK:
            case LPAREN:
            case LITERAL_function:
            case IDENT:
            case NUMBER:
            case PLUS:
            case MINUS:
            case INC:
            case DEC:
            case LITERAL_delete:
            case LITERAL_typeof:
            case LNOT:
            case BNOT:
            case LITERAL_null:
            case LITERAL_true:
            case LITERAL_false:
            case LITERAL_undefined:
            case STRING_LITERAL:
            case REGEX_LITERAL:
            case XML_LITERAL:
            case LITERAL_new:
            {
                expressionList();
                if (0 == inputState.guessing)
                {
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
                break;
            }
            case RPAREN:
            {
                if (0==inputState.guessing)
                {
                    arguments_AST = (AST)currentAST.root;
                    arguments_AST = astFactory.create(ELIST,"ELIST");
                    currentAST.root = arguments_AST;
                    if ( (null != arguments_AST) && (null != arguments_AST.getFirstChild()) )
                        currentAST.child = arguments_AST.getFirstChild();
                    else
                        currentAST.child = arguments_AST;
                    currentAST.advanceChildToEnd();
                }
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        arguments_AST = currentAST.root;
        returnAST = arguments_AST;
    }
    
    public void arrayLiteral() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST arrayLiteral_AST = null;
        
        AST tmp78_AST = null;
        tmp78_AST = astFactory.create(LT(1));
        astFactory.addASTChild(ref currentAST, tmp78_AST);
        match(LBRACK);
        {
            switch ( LA(1) )
            {
            case LCURLY:
            case LBRACK:
            case COMMA:
            case LPAREN:
            case LITERAL_function:
            case IDENT:
            case NUMBER:
            case PLUS:
            case MINUS:
            case INC:
            case DEC:
            case LITERAL_delete:
            case LITERAL_typeof:
            case LNOT:
            case BNOT:
            case LITERAL_null:
            case LITERAL_true:
            case LITERAL_false:
            case LITERAL_undefined:
            case STRING_LITERAL:
            case REGEX_LITERAL:
            case XML_LITERAL:
            case LITERAL_new:
            {
                elementList();
                if (0 == inputState.guessing)
                {
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
                break;
            }
            case RBRACK:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        AST tmp79_AST = null;
        tmp79_AST = astFactory.create(LT(1));
        astFactory.addASTChild(ref currentAST, tmp79_AST);
        match(RBRACK);
        arrayLiteral_AST = currentAST.root;
        returnAST = arrayLiteral_AST;
    }
    
    public void elementList() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST elementList_AST = null;
        
        switch ( LA(1) )
        {
        case COMMA:
        {
            AST tmp80_AST = null;
            tmp80_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp80_AST);
            match(COMMA);
            elementList_AST = currentAST.root;
            break;
        }
        case LCURLY:
        case LBRACK:
        case LPAREN:
        case LITERAL_function:
        case IDENT:
        case NUMBER:
        case PLUS:
        case MINUS:
        case INC:
        case DEC:
        case LITERAL_delete:
        case LITERAL_typeof:
        case LNOT:
        case BNOT:
        case LITERAL_null:
        case LITERAL_true:
        case LITERAL_false:
        case LITERAL_undefined:
        case STRING_LITERAL:
        case REGEX_LITERAL:
        case XML_LITERAL:
        case LITERAL_new:
        {
            nonemptyElementList();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            elementList_AST = currentAST.root;
            break;
        }
        default:
        {
            throw new NoViableAltException(LT(1), getFilename());
        }
         }
        returnAST = elementList_AST;
    }
    
    public void nonemptyElementList() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST nonemptyElementList_AST = null;
        
        assignmentExpression();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==COMMA))
                {
                    AST tmp81_AST = null;
                    tmp81_AST = astFactory.create(LT(1));
                    astFactory.addASTChild(ref currentAST, tmp81_AST);
                    match(COMMA);
                    assignmentExpression();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop129_breakloop;
                }
                
            }
_loop129_breakloop:         ;
        }    // ( ... )*
        nonemptyElementList_AST = currentAST.root;
        returnAST = nonemptyElementList_AST;
    }
    
    public void element() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST element_AST = null;
        AST s_AST = null;
        
        assignmentExpression();
        if (0 == inputState.guessing)
        {
            s_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        if (0==inputState.guessing)
        {
            element_AST = (AST)currentAST.root;
            element_AST = (AST) astFactory.make(astFactory.create(ELEMENT,"ELEMENT"), element_AST);
                    setRegionInfo(element_AST,s_AST,null);
            currentAST.root = element_AST;
            if ( (null != element_AST) && (null != element_AST.getFirstChild()) )
                currentAST.child = element_AST.getFirstChild();
            else
                currentAST.child = element_AST;
            currentAST.advanceChildToEnd();
        }
        element_AST = currentAST.root;
        returnAST = element_AST;
    }
    
    public void objectLiteral() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST objectLiteral_AST = null;
        IToken  s = null;
        AST s_AST = null;
        IToken  e = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        match(LCURLY);
        {
            switch ( LA(1) )
            {
            case IDENT:
            case NUMBER:
            {
                fieldList();
                if (0 == inputState.guessing)
                {
                    astFactory.addASTChild(ref currentAST, returnAST);
                }
                break;
            }
            case RCURLY:
            {
                break;
            }
            default:
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
             }
        }
        e = LT(1);
        e_AST = astFactory.create(e);
        match(RCURLY);
        if (0==inputState.guessing)
        {
            objectLiteral_AST = (AST)currentAST.root;
            objectLiteral_AST = (AST) astFactory.make(astFactory.create(OBJECT_LITERAL,"OBJECT_LITERAL"), objectLiteral_AST);
                    setRegionInfo(objectLiteral_AST,s_AST,e_AST);
            currentAST.root = objectLiteral_AST;
            if ( (null != objectLiteral_AST) && (null != objectLiteral_AST.getFirstChild()) )
                currentAST.child = objectLiteral_AST.getFirstChild();
            else
                currentAST.child = objectLiteral_AST;
            currentAST.advanceChildToEnd();
        }
        objectLiteral_AST = currentAST.root;
        returnAST = objectLiteral_AST;
    }
    
    public void fieldList() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST fieldList_AST = null;
        
        literalField();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==COMMA))
                {
                    match(COMMA);
                    {
                        switch ( LA(1) )
                        {
                        case IDENT:
                        case NUMBER:
                        {
                            literalField();
                            if (0 == inputState.guessing)
                            {
                                astFactory.addASTChild(ref currentAST, returnAST);
                            }
                            break;
                        }
                        case RCURLY:
                        case COMMA:
                        {
                            break;
                        }
                        default:
                        {
                            throw new NoViableAltException(LT(1), getFilename());
                        }
                         }
                    }
                }
                else
                {
                    goto _loop136_breakloop;
                }
                
            }
_loop136_breakloop:         ;
        }    // ( ... )*
        fieldList_AST = currentAST.root;
        returnAST = fieldList_AST;
    }
    
    public void literalField() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST literalField_AST = null;
        AST s_AST = null;
        AST e_AST = null;
        
        fieldName();
        if (0 == inputState.guessing)
        {
            s_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        match(COLON);
        element();
        if (0 == inputState.guessing)
        {
            e_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        if (0==inputState.guessing)
        {
            literalField_AST = (AST)currentAST.root;
            literalField_AST = (AST) astFactory.make(astFactory.create(OBJECT_FIELD,"OBJECT_FIELD"), literalField_AST);
                    setRegionInfo(literalField_AST,s_AST,e_AST);
            currentAST.root = literalField_AST;
            if ( (null != literalField_AST) && (null != literalField_AST.getFirstChild()) )
                currentAST.child = literalField_AST.getFirstChild();
            else
                currentAST.child = literalField_AST;
            currentAST.advanceChildToEnd();
        }
        literalField_AST = currentAST.root;
        returnAST = literalField_AST;
    }
    
    public void fieldName() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST fieldName_AST = null;
        
        switch ( LA(1) )
        {
        case IDENT:
        {
            AST tmp84_AST = null;
            tmp84_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp84_AST);
            match(IDENT);
            fieldName_AST = currentAST.root;
            break;
        }
        case NUMBER:
        {
            AST tmp85_AST = null;
            tmp85_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp85_AST);
            match(NUMBER);
            fieldName_AST = currentAST.root;
            break;
        }
        default:
        {
            throw new NoViableAltException(LT(1), getFilename());
        }
         }
        returnAST = fieldName_AST;
    }
    
    public void conditionalExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST conditionalExpression_AST = null;
        AST s_AST = null;
        AST e_AST = null;
        
        logicalOrExpression();
        if (0 == inputState.guessing)
        {
            s_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==QUESTION) && (tokenSet_3_.member(LA(2))))
                {
                    AST tmp86_AST = null;
                    tmp86_AST = astFactory.create(LT(1));
                    astFactory.makeASTRoot(ref currentAST, tmp86_AST);
                    match(QUESTION);
                    conditionalSubExpression();
                    if (0 == inputState.guessing)
                    {
                        e_AST = (AST)returnAST;
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                    if (0==inputState.guessing)
                    {
                        conditionalExpression_AST = (AST)currentAST.root;
                        setRegionInfo(conditionalExpression_AST,s_AST,e_AST);
                    }
                }
                else
                {
                    goto _loop151_breakloop;
                }
                
            }
_loop151_breakloop:         ;
        }    // ( ... )*
        conditionalExpression_AST = currentAST.root;
        returnAST = conditionalExpression_AST;
    }
    
    public void logicalOrExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST logicalOrExpression_AST = null;
        
        logicalAndExpression();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==LOR))
                {
                    AST tmp87_AST = null;
                    tmp87_AST = astFactory.create(LT(1));
                    astFactory.makeASTRoot(ref currentAST, tmp87_AST);
                    match(LOR);
                    logicalAndExpression();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop155_breakloop;
                }
                
            }
_loop155_breakloop:         ;
        }    // ( ... )*
        logicalOrExpression_AST = currentAST.root;
        returnAST = logicalOrExpression_AST;
    }
    
    public void conditionalSubExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST conditionalSubExpression_AST = null;
        AST s_AST = null;
        AST e_AST = null;
        
        assignmentExpression();
        if (0 == inputState.guessing)
        {
            s_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        AST tmp88_AST = null;
        tmp88_AST = astFactory.create(LT(1));
        astFactory.makeASTRoot(ref currentAST, tmp88_AST);
        match(COLON);
        assignmentExpression();
        if (0 == inputState.guessing)
        {
            e_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        if (0==inputState.guessing)
        {
            conditionalSubExpression_AST = (AST)currentAST.root;
            setRegionInfo(conditionalSubExpression_AST,s_AST,e_AST);
        }
        conditionalSubExpression_AST = currentAST.root;
        returnAST = conditionalSubExpression_AST;
    }
    
    public void logicalAndExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST logicalAndExpression_AST = null;
        
        bitwiseOrExpression();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==LAND))
                {
                    AST tmp89_AST = null;
                    tmp89_AST = astFactory.create(LT(1));
                    astFactory.makeASTRoot(ref currentAST, tmp89_AST);
                    match(LAND);
                    bitwiseOrExpression();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop158_breakloop;
                }
                
            }
_loop158_breakloop:         ;
        }    // ( ... )*
        logicalAndExpression_AST = currentAST.root;
        returnAST = logicalAndExpression_AST;
    }
    
    public void bitwiseOrExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST bitwiseOrExpression_AST = null;
        
        bitwiseXorExpression();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==BOR))
                {
                    AST tmp90_AST = null;
                    tmp90_AST = astFactory.create(LT(1));
                    astFactory.makeASTRoot(ref currentAST, tmp90_AST);
                    match(BOR);
                    bitwiseXorExpression();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop161_breakloop;
                }
                
            }
_loop161_breakloop:         ;
        }    // ( ... )*
        bitwiseOrExpression_AST = currentAST.root;
        returnAST = bitwiseOrExpression_AST;
    }
    
    public void bitwiseXorExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST bitwiseXorExpression_AST = null;
        
        bitwiseAndExpression();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==BXOR))
                {
                    AST tmp91_AST = null;
                    tmp91_AST = astFactory.create(LT(1));
                    astFactory.makeASTRoot(ref currentAST, tmp91_AST);
                    match(BXOR);
                    bitwiseAndExpression();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop164_breakloop;
                }
                
            }
_loop164_breakloop:         ;
        }    // ( ... )*
        bitwiseXorExpression_AST = currentAST.root;
        returnAST = bitwiseXorExpression_AST;
    }
    
    public void bitwiseAndExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST bitwiseAndExpression_AST = null;
        
        equalityExpression();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==BAND))
                {
                    AST tmp92_AST = null;
                    tmp92_AST = astFactory.create(LT(1));
                    astFactory.makeASTRoot(ref currentAST, tmp92_AST);
                    match(BAND);
                    equalityExpression();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop167_breakloop;
                }
                
            }
_loop167_breakloop:         ;
        }    // ( ... )*
        bitwiseAndExpression_AST = currentAST.root;
        returnAST = bitwiseAndExpression_AST;
    }
    
    public void equalityExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST equalityExpression_AST = null;
        
        relationalExpression();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {    // ( ... )*
            for (;;)
            {
                if (((LA(1) >= STRICT_EQUAL && LA(1) <= EQUAL)))
                {
                    {
                        switch ( LA(1) )
                        {
                        case STRICT_EQUAL:
                        {
                            AST tmp93_AST = null;
                            tmp93_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp93_AST);
                            match(STRICT_EQUAL);
                            break;
                        }
                        case STRICT_NOT_EQUAL:
                        {
                            AST tmp94_AST = null;
                            tmp94_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp94_AST);
                            match(STRICT_NOT_EQUAL);
                            break;
                        }
                        case NOT_EQUAL:
                        {
                            AST tmp95_AST = null;
                            tmp95_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp95_AST);
                            match(NOT_EQUAL);
                            break;
                        }
                        case EQUAL:
                        {
                            AST tmp96_AST = null;
                            tmp96_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp96_AST);
                            match(EQUAL);
                            break;
                        }
                        default:
                        {
                            throw new NoViableAltException(LT(1), getFilename());
                        }
                         }
                    }
                    relationalExpression();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop171_breakloop;
                }
                
            }
_loop171_breakloop:         ;
        }    // ( ... )*
        equalityExpression_AST = currentAST.root;
        returnAST = equalityExpression_AST;
    }
    
    public void relationalExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST relationalExpression_AST = null;
        
        shiftExpression();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {    // ( ... )*
            for (;;)
            {
                if (((LA(1) >= LWT && LA(1) <= LITERAL_as)))
                {
                    {
                        switch ( LA(1) )
                        {
                        case LWT:
                        {
                            AST tmp97_AST = null;
                            tmp97_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp97_AST);
                            match(LWT);
                            break;
                        }
                        case GT:
                        {
                            AST tmp98_AST = null;
                            tmp98_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp98_AST);
                            match(GT);
                            break;
                        }
                        case LE:
                        {
                            AST tmp99_AST = null;
                            tmp99_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp99_AST);
                            match(LE);
                            break;
                        }
                        case GE:
                        {
                            AST tmp100_AST = null;
                            tmp100_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp100_AST);
                            match(GE);
                            break;
                        }
                        case LITERAL_is:
                        {
                            AST tmp101_AST = null;
                            tmp101_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp101_AST);
                            match(LITERAL_is);
                            break;
                        }
                        case LITERAL_as:
                        {
                            AST tmp102_AST = null;
                            tmp102_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp102_AST);
                            match(LITERAL_as);
                            break;
                        }
                        default:
                        {
                            throw new NoViableAltException(LT(1), getFilename());
                        }
                         }
                    }
                    shiftExpression();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop175_breakloop;
                }
                
            }
_loop175_breakloop:         ;
        }    // ( ... )*
        relationalExpression_AST = currentAST.root;
        returnAST = relationalExpression_AST;
    }
    
    public void shiftExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST shiftExpression_AST = null;
        
        additiveExpression();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {    // ( ... )*
            for (;;)
            {
                if (((LA(1) >= SL && LA(1) <= BSR)))
                {
                    {
                        switch ( LA(1) )
                        {
                        case SL:
                        {
                            AST tmp103_AST = null;
                            tmp103_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp103_AST);
                            match(SL);
                            break;
                        }
                        case SR:
                        {
                            AST tmp104_AST = null;
                            tmp104_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp104_AST);
                            match(SR);
                            break;
                        }
                        case BSR:
                        {
                            AST tmp105_AST = null;
                            tmp105_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp105_AST);
                            match(BSR);
                            break;
                        }
                        default:
                        {
                            throw new NoViableAltException(LT(1), getFilename());
                        }
                         }
                    }
                    additiveExpression();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop179_breakloop;
                }
                
            }
_loop179_breakloop:         ;
        }    // ( ... )*
        shiftExpression_AST = currentAST.root;
        returnAST = shiftExpression_AST;
    }
    
    public void additiveExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST additiveExpression_AST = null;
        
        multiplicativeExpression();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==PLUS||LA(1)==MINUS) && (tokenSet_3_.member(LA(2))))
                {
                    {
                        switch ( LA(1) )
                        {
                        case PLUS:
                        {
                            AST tmp106_AST = null;
                            tmp106_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp106_AST);
                            match(PLUS);
                            break;
                        }
                        case MINUS:
                        {
                            AST tmp107_AST = null;
                            tmp107_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp107_AST);
                            match(MINUS);
                            break;
                        }
                        default:
                        {
                            throw new NoViableAltException(LT(1), getFilename());
                        }
                         }
                    }
                    multiplicativeExpression();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop183_breakloop;
                }
                
            }
_loop183_breakloop:         ;
        }    // ( ... )*
        additiveExpression_AST = currentAST.root;
        returnAST = additiveExpression_AST;
    }
    
    public void multiplicativeExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST multiplicativeExpression_AST = null;
        
        unaryExpression();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==STAR||LA(1)==DIV||LA(1)==MOD))
                {
                    {
                        switch ( LA(1) )
                        {
                        case STAR:
                        {
                            AST tmp108_AST = null;
                            tmp108_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp108_AST);
                            match(STAR);
                            break;
                        }
                        case DIV:
                        {
                            AST tmp109_AST = null;
                            tmp109_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp109_AST);
                            match(DIV);
                            break;
                        }
                        case MOD:
                        {
                            AST tmp110_AST = null;
                            tmp110_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp110_AST);
                            match(MOD);
                            break;
                        }
                        default:
                        {
                            throw new NoViableAltException(LT(1), getFilename());
                        }
                         }
                    }
                    unaryExpression();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                }
                else
                {
                    goto _loop187_breakloop;
                }
                
            }
_loop187_breakloop:         ;
        }    // ( ... )*
        multiplicativeExpression_AST = currentAST.root;
        returnAST = multiplicativeExpression_AST;
    }
    
    public void unaryExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST unaryExpression_AST = null;
        
        switch ( LA(1) )
        {
        case INC:
        {
            AST tmp111_AST = null;
            tmp111_AST = astFactory.create(LT(1));
            astFactory.makeASTRoot(ref currentAST, tmp111_AST);
            match(INC);
            unaryExpression();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            unaryExpression_AST = currentAST.root;
            break;
        }
        case DEC:
        {
            AST tmp112_AST = null;
            tmp112_AST = astFactory.create(LT(1));
            astFactory.makeASTRoot(ref currentAST, tmp112_AST);
            match(DEC);
            unaryExpression();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            unaryExpression_AST = currentAST.root;
            break;
        }
        case MINUS:
        {
            AST tmp113_AST = null;
            tmp113_AST = astFactory.create(LT(1));
            astFactory.makeASTRoot(ref currentAST, tmp113_AST);
            match(MINUS);
            if (0==inputState.guessing)
            {
                tmp113_AST.setType(UNARY_MINUS);
            }
            unaryExpression();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            unaryExpression_AST = currentAST.root;
            break;
        }
        case PLUS:
        {
            AST tmp114_AST = null;
            tmp114_AST = astFactory.create(LT(1));
            astFactory.makeASTRoot(ref currentAST, tmp114_AST);
            match(PLUS);
            if (0==inputState.guessing)
            {
                tmp114_AST.setType(UNARY_PLUS);
            }
            unaryExpression();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            unaryExpression_AST = currentAST.root;
            break;
        }
        case LCURLY:
        case LBRACK:
        case LPAREN:
        case LITERAL_function:
        case IDENT:
        case NUMBER:
        case LITERAL_delete:
        case LITERAL_typeof:
        case LNOT:
        case BNOT:
        case LITERAL_null:
        case LITERAL_true:
        case LITERAL_false:
        case LITERAL_undefined:
        case STRING_LITERAL:
        case REGEX_LITERAL:
        case XML_LITERAL:
        case LITERAL_new:
        {
            unaryExpressionNotPlusMinus();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            unaryExpression_AST = currentAST.root;
            break;
        }
        default:
        {
            throw new NoViableAltException(LT(1), getFilename());
        }
         }
        returnAST = unaryExpression_AST;
    }
    
    public void unaryExpressionNotPlusMinus() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST unaryExpressionNotPlusMinus_AST = null;
        
        switch ( LA(1) )
        {
        case LITERAL_delete:
        {
            AST tmp115_AST = null;
            tmp115_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp115_AST);
            match(LITERAL_delete);
            postfixExpression();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            unaryExpressionNotPlusMinus_AST = currentAST.root;
            break;
        }
        case LITERAL_typeof:
        {
            AST tmp116_AST = null;
            tmp116_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp116_AST);
            match(LITERAL_typeof);
            unaryExpression();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            unaryExpressionNotPlusMinus_AST = currentAST.root;
            break;
        }
        case LNOT:
        {
            AST tmp117_AST = null;
            tmp117_AST = astFactory.create(LT(1));
            astFactory.makeASTRoot(ref currentAST, tmp117_AST);
            match(LNOT);
            unaryExpression();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            unaryExpressionNotPlusMinus_AST = currentAST.root;
            break;
        }
        case BNOT:
        {
            AST tmp118_AST = null;
            tmp118_AST = astFactory.create(LT(1));
            astFactory.makeASTRoot(ref currentAST, tmp118_AST);
            match(BNOT);
            unaryExpression();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            unaryExpressionNotPlusMinus_AST = currentAST.root;
            break;
        }
        case LCURLY:
        case LBRACK:
        case LPAREN:
        case LITERAL_function:
        case IDENT:
        case NUMBER:
        case LITERAL_null:
        case LITERAL_true:
        case LITERAL_false:
        case LITERAL_undefined:
        case STRING_LITERAL:
        case REGEX_LITERAL:
        case XML_LITERAL:
        case LITERAL_new:
        {
            postfixExpression();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            unaryExpressionNotPlusMinus_AST = currentAST.root;
            break;
        }
        default:
        {
            throw new NoViableAltException(LT(1), getFilename());
        }
         }
        returnAST = unaryExpressionNotPlusMinus_AST;
    }
    
    public void postfixExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST postfixExpression_AST = null;
        AST s_AST = null;
        IToken  e11 = null;
        AST e11_AST = null;
        AST e12_AST = null;
        IToken  t1 = null;
        AST t1_AST = null;
        IToken  e2 = null;
        AST e2_AST = null;
        IToken  t2 = null;
        AST t2_AST = null;
        IToken  e3 = null;
        AST e3_AST = null;
        IToken  t3 = null;
        AST t3_AST = null;
        IToken  e4 = null;
        AST e4_AST = null;
        IToken  oin = null;
        AST oin_AST = null;
        IToken  ode = null;
        AST ode_AST = null;
        
        primaryExpression();
        if (0 == inputState.guessing)
        {
            s_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==LBRACK||LA(1)==DOT||LA(1)==E4X_DESC) && (tokenSet_11_.member(LA(2))))
                {
                    {
                        switch ( LA(1) )
                        {
                        case DOT:
                        {
                            match(DOT);
                            {
                                if ((LA(1)==IDENT) && (tokenSet_12_.member(LA(2))))
                                {
                                    e11 = LT(1);
                                    e11_AST = astFactory.create(e11);
                                    astFactory.makeASTRoot(ref currentAST, e11_AST);
                                    match(IDENT);
                                    if (0==inputState.guessing)
                                    {
                                        setRegionInfo(e11_AST,s_AST,e11_AST);
                                    }
                                }
                                else if ((LA(1)==IDENT||LA(1)==STAR||LA(1)==E4X_ATTRI) && (tokenSet_12_.member(LA(2)))) {
                                    e4xExpression();
                                    if (0 == inputState.guessing)
                                    {
                                        e12_AST = (AST)returnAST;
                                        astFactory.addASTChild(ref currentAST, returnAST);
                                    }
                                    if (0==inputState.guessing)
                                    {
                                        setRegionInfo(e12_AST,s_AST,e12_AST);
                                    }
                                }
                                else
                                {
                                    throw new NoViableAltException(LT(1), getFilename());
                                }
                                
                            }
                            break;
                        }
                        case LBRACK:
                        {
                            {
                                t1 = LT(1);
                                t1_AST = astFactory.create(t1);
                                astFactory.makeASTRoot(ref currentAST, t1_AST);
                                match(LBRACK);
                                expression();
                                if (0 == inputState.guessing)
                                {
                                    astFactory.addASTChild(ref currentAST, returnAST);
                                }
                                e2 = LT(1);
                                e2_AST = astFactory.create(e2);
                                match(RBRACK);
                                if (0==inputState.guessing)
                                {
                                    t1_AST.setType(ARRAY_ACC);
                                                        t1_AST.setText("ARRAY_ACC");
                                                        setRegionInfo(t1_AST,s_AST,e2_AST);
                                }
                            }
                            break;
                        }
                        case E4X_DESC:
                        {
                            AST tmp120_AST = null;
                            tmp120_AST = astFactory.create(LT(1));
                            astFactory.makeASTRoot(ref currentAST, tmp120_AST);
                            match(E4X_DESC);
                            e4xExpression();
                            if (0 == inputState.guessing)
                            {
                                astFactory.addASTChild(ref currentAST, returnAST);
                            }
                            break;
                        }
                        default:
                        {
                            throw new NoViableAltException(LT(1), getFilename());
                        }
                         }
                    }
                    {
                        if ((LA(1)==LPAREN) && (tokenSet_13_.member(LA(2))))
                        {
                            t2 = LT(1);
                            t2_AST = astFactory.create(t2);
                            astFactory.makeASTRoot(ref currentAST, t2_AST);
                            match(LPAREN);
                            arguments();
                            if (0 == inputState.guessing)
                            {
                                astFactory.addASTChild(ref currentAST, returnAST);
                            }
                            e3 = LT(1);
                            e3_AST = astFactory.create(e3);
                            match(RPAREN);
                            if (0==inputState.guessing)
                            {
                                t2_AST.setType(METHOD_CALL);
                                                t2_AST.setText("METHOD_CALL");
                                                setRegionInfo(t2_AST,s_AST,e3_AST);
                            }
                            {    // ( ... )*
                                for (;;)
                                {
                                    if ((LA(1)==LPAREN) && (tokenSet_13_.member(LA(2))))
                                    {
                                        t3 = LT(1);
                                        t3_AST = astFactory.create(t3);
                                        astFactory.makeASTRoot(ref currentAST, t3_AST);
                                        match(LPAREN);
                                        arguments();
                                        if (0 == inputState.guessing)
                                        {
                                            astFactory.addASTChild(ref currentAST, returnAST);
                                        }
                                        e4 = LT(1);
                                        e4_AST = astFactory.create(e4);
                                        match(RPAREN);
                                        if (0==inputState.guessing)
                                        {
                                            t3_AST.setType(METHOD_CALL);
                                                                t3_AST.setText("METHOD_CALL");
                                                                setRegionInfo(t3_AST,s_AST,e4_AST);
                                        }
                                    }
                                    else
                                    {
                                        goto _loop197_breakloop;
                                    }
                                    
                                }
_loop197_breakloop:                             ;
                            }    // ( ... )*
                        }
                        else if ((tokenSet_12_.member(LA(1))) && (tokenSet_14_.member(LA(2)))) {
                        }
                        else
                        {
                            throw new NoViableAltException(LT(1), getFilename());
                        }
                        
                    }
                }
                else
                {
                    goto _loop198_breakloop;
                }
                
            }
_loop198_breakloop:         ;
        }    // ( ... )*
        {
            if ((LA(1)==INC) && (tokenSet_12_.member(LA(2))))
            {
                oin = LT(1);
                oin_AST = astFactory.create(oin);
                astFactory.makeASTRoot(ref currentAST, oin_AST);
                match(INC);
                if (0==inputState.guessing)
                {
                    oin_AST.setType(POST_INC);
                }
            }
            else if ((LA(1)==DEC) && (tokenSet_12_.member(LA(2)))) {
                ode = LT(1);
                ode_AST = astFactory.create(ode);
                astFactory.makeASTRoot(ref currentAST, ode_AST);
                match(DEC);
                if (0==inputState.guessing)
                {
                    ode_AST.setType(POST_DEC);
                }
            }
            else if ((tokenSet_12_.member(LA(1))) && (tokenSet_14_.member(LA(2)))) {
            }
            else
            {
                throw new NoViableAltException(LT(1), getFilename());
            }
            
        }
        postfixExpression_AST = currentAST.root;
        returnAST = postfixExpression_AST;
    }
    
    public void primaryExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST primaryExpression_AST = null;
        
        switch ( LA(1) )
        {
        case IDENT:
        {
            identPrimary();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            primaryExpression_AST = currentAST.root;
            break;
        }
        case LITERAL_null:
        {
            AST tmp121_AST = null;
            tmp121_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp121_AST);
            match(LITERAL_null);
            primaryExpression_AST = currentAST.root;
            break;
        }
        case LITERAL_true:
        {
            AST tmp122_AST = null;
            tmp122_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp122_AST);
            match(LITERAL_true);
            primaryExpression_AST = currentAST.root;
            break;
        }
        case LITERAL_false:
        {
            AST tmp123_AST = null;
            tmp123_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp123_AST);
            match(LITERAL_false);
            primaryExpression_AST = currentAST.root;
            break;
        }
        case LITERAL_undefined:
        {
            AST tmp124_AST = null;
            tmp124_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp124_AST);
            match(LITERAL_undefined);
            primaryExpression_AST = currentAST.root;
            break;
        }
        case NUMBER:
        case STRING_LITERAL:
        case REGEX_LITERAL:
        case XML_LITERAL:
        {
            constant();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            primaryExpression_AST = currentAST.root;
            break;
        }
        case LBRACK:
        {
            arrayLiteral();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            primaryExpression_AST = currentAST.root;
            break;
        }
        case LCURLY:
        {
            objectLiteral();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            primaryExpression_AST = currentAST.root;
            break;
        }
        case LITERAL_function:
        {
            functionDefinition();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            primaryExpression_AST = currentAST.root;
            break;
        }
        case LITERAL_new:
        {
            newExpression();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            primaryExpression_AST = currentAST.root;
            break;
        }
        case LPAREN:
        {
            encapsulatedExpression();
            if (0 == inputState.guessing)
            {
                astFactory.addASTChild(ref currentAST, returnAST);
            }
            primaryExpression_AST = currentAST.root;
            break;
        }
        default:
        {
            throw new NoViableAltException(LT(1), getFilename());
        }
         }
        returnAST = primaryExpression_AST;
    }
    
    public void e4xExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST e4xExpression_AST = null;
        IToken  e12 = null;
        AST e12_AST = null;
        IToken  e13 = null;
        AST e13_AST = null;
        IToken  t1 = null;
        AST t1_AST = null;
        IToken  e2 = null;
        AST e2_AST = null;
        
        switch ( LA(1) )
        {
        case IDENT:
        {
            AST tmp125_AST = null;
            tmp125_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp125_AST);
            match(IDENT);
            e4xExpression_AST = currentAST.root;
            break;
        }
        case STAR:
        {
            e12 = LT(1);
            e12_AST = astFactory.create(e12);
            astFactory.makeASTRoot(ref currentAST, e12_AST);
            match(STAR);
            e4xExpression_AST = currentAST.root;
            break;
        }
        case E4X_ATTRI:
        {
            e13 = LT(1);
            e13_AST = astFactory.create(e13);
            astFactory.makeASTRoot(ref currentAST, e13_AST);
            match(E4X_ATTRI);
            {
                switch ( LA(1) )
                {
                case IDENT:
                {
                    AST tmp126_AST = null;
                    tmp126_AST = astFactory.create(LT(1));
                    astFactory.addASTChild(ref currentAST, tmp126_AST);
                    match(IDENT);
                    break;
                }
                case LBRACK:
                case STAR:
                {
                    {
                        switch ( LA(1) )
                        {
                        case STAR:
                        {
                            AST tmp127_AST = null;
                            tmp127_AST = astFactory.create(LT(1));
                            astFactory.addASTChild(ref currentAST, tmp127_AST);
                            match(STAR);
                            break;
                        }
                        case LBRACK:
                        {
                            break;
                        }
                        default:
                        {
                            throw new NoViableAltException(LT(1), getFilename());
                        }
                         }
                    }
                    t1 = LT(1);
                    t1_AST = astFactory.create(t1);
                    astFactory.makeASTRoot(ref currentAST, t1_AST);
                    match(LBRACK);
                    expression();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                    e2 = LT(1);
                    e2_AST = astFactory.create(e2);
                    match(RBRACK);
                    break;
                }
                default:
                {
                    throw new NoViableAltException(LT(1), getFilename());
                }
                 }
            }
            e4xExpression_AST = currentAST.root;
            break;
        }
        default:
        {
            throw new NoViableAltException(LT(1), getFilename());
        }
         }
        returnAST = e4xExpression_AST;
    }
    
    public void identPrimary() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST identPrimary_AST = null;
        IToken  s = null;
        AST s_AST = null;
        IToken  e1 = null;
        AST e1_AST = null;
        IToken  t1 = null;
        AST t1_AST = null;
        IToken  e2 = null;
        AST e2_AST = null;
        IToken  t2 = null;
        AST t2_AST = null;
        IToken  e3 = null;
        AST e3_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        astFactory.addASTChild(ref currentAST, s_AST);
        match(IDENT);
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==DOT) && (LA(2)==IDENT))
                {
                    match(DOT);
                    e1 = LT(1);
                    e1_AST = astFactory.create(e1);
                    astFactory.makeASTRoot(ref currentAST, e1_AST);
                    match(IDENT);
                    if (0==inputState.guessing)
                    {
                        setRegionInfo(e1_AST,s_AST,e1_AST);
                    }
                }
                else if ((LA(1)==LBRACK) && (tokenSet_3_.member(LA(2)))) {
                    {
                        t1 = LT(1);
                        t1_AST = astFactory.create(t1);
                        astFactory.makeASTRoot(ref currentAST, t1_AST);
                        match(LBRACK);
                        expression();
                        if (0 == inputState.guessing)
                        {
                            astFactory.addASTChild(ref currentAST, returnAST);
                        }
                        e2 = LT(1);
                        e2_AST = astFactory.create(e2);
                        match(RBRACK);
                        if (0==inputState.guessing)
                        {
                            t1_AST.setType(ARRAY_ACC);
                                            t1_AST.setText("ARRAY_ACC");
                                            setRegionInfo(t1_AST,s_AST,e2_AST);
                        }
                    }
                }
                else
                {
                    goto _loop207_breakloop;
                }
                
            }
_loop207_breakloop:         ;
        }    // ( ... )*
        {    // ( ... )*
            for (;;)
            {
                if ((LA(1)==LPAREN) && (tokenSet_13_.member(LA(2))))
                {
                    t2 = LT(1);
                    t2_AST = astFactory.create(t2);
                    astFactory.makeASTRoot(ref currentAST, t2_AST);
                    match(LPAREN);
                    arguments();
                    if (0 == inputState.guessing)
                    {
                        astFactory.addASTChild(ref currentAST, returnAST);
                    }
                    e3 = LT(1);
                    e3_AST = astFactory.create(e3);
                    match(RPAREN);
                    if (0==inputState.guessing)
                    {
                        t2_AST.setType(METHOD_CALL);
                                    t2_AST.setText("METHOD_CALL");
                                    setRegionInfo(t2_AST,s_AST,e3_AST);
                    }
                }
                else
                {
                    goto _loop209_breakloop;
                }
                
            }
_loop209_breakloop:         ;
        }    // ( ... )*
        identPrimary_AST = currentAST.root;
        returnAST = identPrimary_AST;
    }
    
    public void constant() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST constant_AST = null;
        
        switch ( LA(1) )
        {
        case NUMBER:
        {
            AST tmp129_AST = null;
            tmp129_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp129_AST);
            match(NUMBER);
            constant_AST = currentAST.root;
            break;
        }
        case STRING_LITERAL:
        {
            AST tmp130_AST = null;
            tmp130_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp130_AST);
            match(STRING_LITERAL);
            constant_AST = currentAST.root;
            break;
        }
        case REGEX_LITERAL:
        {
            AST tmp131_AST = null;
            tmp131_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp131_AST);
            match(REGEX_LITERAL);
            constant_AST = currentAST.root;
            break;
        }
        case XML_LITERAL:
        {
            AST tmp132_AST = null;
            tmp132_AST = astFactory.create(LT(1));
            astFactory.addASTChild(ref currentAST, tmp132_AST);
            match(XML_LITERAL);
            constant_AST = currentAST.root;
            break;
        }
        default:
        {
            throw new NoViableAltException(LT(1), getFilename());
        }
         }
        returnAST = constant_AST;
    }
    
    public void functionDefinition() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST functionDefinition_AST = null;
        IToken  s = null;
        AST s_AST = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        astFactory.addASTChild(ref currentAST, s_AST);
        match(LITERAL_function);
        parameterDeclarationList();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        typeExpression();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        block();
        if (0 == inputState.guessing)
        {
            e_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        if (0==inputState.guessing)
        {
            functionDefinition_AST = (AST)currentAST.root;
            functionDefinition_AST = (AST) astFactory.make(astFactory.create(FUNC_DEF,"FUNC_DEF"), functionDefinition_AST);
                    setRegionInfo(functionDefinition_AST,s_AST,e_AST);
            currentAST.root = functionDefinition_AST;
            if ( (null != functionDefinition_AST) && (null != functionDefinition_AST.getFirstChild()) )
                currentAST.child = functionDefinition_AST.getFirstChild();
            else
                currentAST.child = functionDefinition_AST;
            currentAST.advanceChildToEnd();
        }
        functionDefinition_AST = currentAST.root;
        returnAST = functionDefinition_AST;
    }
    
    public void newExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST newExpression_AST = null;
        IToken  s = null;
        AST s_AST = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        astFactory.addASTChild(ref currentAST, s_AST);
        match(LITERAL_new);
        postfixExpression();
        if (0 == inputState.guessing)
        {
            e_AST = (AST)returnAST;
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        if (0==inputState.guessing)
        {
            newExpression_AST = (AST)currentAST.root;
            newExpression_AST = (AST) astFactory.make(astFactory.create(NEW_EXPR,"NEW_EXPR"), newExpression_AST);
                    setRegionInfo(newExpression_AST,s_AST,e_AST);
            currentAST.root = newExpression_AST;
            if ( (null != newExpression_AST) && (null != newExpression_AST.getFirstChild()) )
                currentAST.child = newExpression_AST.getFirstChild();
            else
                currentAST.child = newExpression_AST;
            currentAST.advanceChildToEnd();
        }
        newExpression_AST = currentAST.root;
        returnAST = newExpression_AST;
    }
    
    public void encapsulatedExpression() //throws RecognitionException, TokenStreamException
{
        
        returnAST = null;
        ASTPair currentAST = new ASTPair();
        AST encapsulatedExpression_AST = null;
        IToken  s = null;
        AST s_AST = null;
        IToken  e = null;
        AST e_AST = null;
        
        s = LT(1);
        s_AST = astFactory.create(s);
        match(LPAREN);
        assignmentExpression();
        if (0 == inputState.guessing)
        {
            astFactory.addASTChild(ref currentAST, returnAST);
        }
        e = LT(1);
        e_AST = astFactory.create(e);
        match(RPAREN);
        if (0==inputState.guessing)
        {
            encapsulatedExpression_AST = (AST)currentAST.root;
            encapsulatedExpression_AST = (AST) astFactory.make(astFactory.create(ENCPS_EXPR,"ENCPS_EXPR"), encapsulatedExpression_AST);
                    setRegionInfo(encapsulatedExpression_AST,s_AST,e_AST);
            currentAST.root = encapsulatedExpression_AST;
            if ( (null != encapsulatedExpression_AST) && (null != encapsulatedExpression_AST.getFirstChild()) )
                currentAST.child = encapsulatedExpression_AST.getFirstChild();
            else
                currentAST.child = encapsulatedExpression_AST;
            currentAST.advanceChildToEnd();
        }
        encapsulatedExpression_AST = currentAST.root;
        returnAST = encapsulatedExpression_AST;
    }
    
    private void initializeFactory()
    {
        if (astFactory == null)
        {
            astFactory = new ASTFactory();
        }
        initializeASTFactory( astFactory );
    }
    static public void initializeASTFactory( ASTFactory factory )
    {
        factory.setMaxNodeType(160);
    }
    
    public static readonly string[] tokenNames_ = new string[] {
        @"""<0>""",
        @"""EOF""",
        @"""<2>""",
        @"""NULL_TREE_LOOKAHEAD""",
        @"""COMPILATION_UNIT""",
        @"""IMPORT""",
        @"""CLASS_DEF""",
        @"""INTERFACE_DEF""",
        @"""EXTENDS_CLAUSE""",
        @"""IMPLEMENTS_CLAUSE""",
        @"""TYPE_BLOCK""",
        @"""MODIFIERS""",
        @"""VARIABLE_DEF""",
        @"""METHOD_DEF""",
        @"""NAMESPACE_DEF""",
        @"""PARAMS""",
        @"""PARAM""",
        @"""TYPE_SPEC""",
        @"""BLOCK""",
        @"""EXPR""",
        @"""ELIST""",
        @"""EXPR_STMNT""",
        @"""NEW_EXPR""",
        @"""ENCPS_EXPR""",
        @"""VAR_INIT""",
        @"""METHOD_CALL""",
        @"""ARRAY_ACC""",
        @"""UNARY_PLUS""",
        @"""UNARY_MINUS""",
        @"""POST_INC""",
        @"""POST_DEC""",
        @"""ARRAY_LITERAL""",
        @"""ELEMENT""",
        @"""OBJECT_LITERAL""",
        @"""OBJECT_FIELD""",
        @"""FUNC_DEF""",
        @"""FOR_INIT""",
        @"""FOR_CONDITION""",
        @"""FOR_ITERATOR""",
        @"""package""",
        @"""LCURLY""",
        @"""SEMI""",
        @"""RCURLY""",
        @"""import""",
        @"""LBRACK""",
        @"""COMMA""",
        @"""RBRACK""",
        @"""LPAREN""",
        @"""RPAREN""",
        @"""class""",
        @"""interface""",
        @"""extends""",
        @"""implements""",
        @"""function""",
        @"""get""",
        @"""set""",
        @"""/n""",
        @"""/r""",
        @"""IDENT""",
        @"""namespace""",
        @"""var""",
        @"""const""",
        @"""ASSIGN""",
        @"""REST""",
        @"""while""",
        @"""do""",
        @"""with""",
        @"""if""",
        @"""else""",
        @"""throw""",
        @"""return""",
        @"""continue""",
        @"""break""",
        @"""switch""",
        @"""case""",
        @"""COLON""",
        @"""default""",
        @"""for""",
        @"""each""",
        @"""in""",
        @"""DOT""",
        @"""STAR""",
        @"""public""",
        @"""private""",
        @"""protected""",
        @"""internal""",
        @"""static""",
        @"""final""",
        @"""enumerable""",
        @"""explicit""",
        @"""override""",
        @"""dynamic""",
        @"""NUMBER""",
        @"""STAR_ASSIGN""",
        @"""DIV_ASSIGN""",
        @"""MOD_ASSIGN""",
        @"""PLUS_ASSIGN""",
        @"""MINUS_ASSIGN""",
        @"""SL_ASSIGN""",
        @"""SR_ASSIGN""",
        @"""BSR_ASSIGN""",
        @"""BAND_ASSIGN""",
        @"""BXOR_ASSIGN""",
        @"""BOR_ASSIGN""",
        @"""LAND_ASSIGN""",
        @"""LOR_ASSIGN""",
        @"""QUESTION""",
        @"""LOR""",
        @"""LAND""",
        @"""BOR""",
        @"""BXOR""",
        @"""BAND""",
        @"""STRICT_EQUAL""",
        @"""STRICT_NOT_EQUAL""",
        @"""NOT_EQUAL""",
        @"""EQUAL""",
        @"""LWT""",
        @"""GT""",
        @"""LE""",
        @"""GE""",
        @"""is""",
        @"""as""",
        @"""SL""",
        @"""SR""",
        @"""BSR""",
        @"""PLUS""",
        @"""MINUS""",
        @"""DIV""",
        @"""MOD""",
        @"""INC""",
        @"""DEC""",
        @"""delete""",
        @"""typeof""",
        @"""LNOT""",
        @"""BNOT""",
        @"""E4X_DESC""",
        @"""E4X_ATTRI""",
        @"""null""",
        @"""true""",
        @"""false""",
        @"""undefined""",
        @"""STRING_LITERAL""",
        @"""REGEX_LITERAL""",
        @"""XML_LITERAL""",
        @"""new""",
        @"""DBL_COLON""",
        @"""XML_ATTRIBUTE""",
        @"""XML_BINDING""",
        @"""XML_AS3_EXPRESSION""",
        @"""XML_TEXTNODE""",
        @"""XML_COMMENT""",
        @"""XML_CDATA""",
        @"""REGEX_BODY""",
        @"""WS""",
        @"""NL""",
        @"""BOM""",
        @"""SL_COMMENT""",
        @"""ML_COMMENT""",
        @"""EXPONENT""",
        @"""HEX_DIGIT""",
        @"""ESC"""
    };
    
    private static long[] mk_tokenSet_0_()
    {
        long[] data = { -216172782113783824L, -1L, 8589934591L, 0L, 0L, 0L};
        return data;
    }
    public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
    private static long[] mk_tokenSet_1_()
    {
        long[] data = { 1450320708222582784L, 6917529027909526511L, 130686L, 0L, 0L, 0L};
        return data;
    }
    public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
    private static long[] mk_tokenSet_2_()
    {
        long[] data = { 4611686018427387904L, 4397509640192L, 0L, 0L};
        return data;
    }
    public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
    private static long[] mk_tokenSet_3_()
    {
        long[] data = { 297397004592480256L, 6917529027909517312L, 130686L, 0L, 0L, 0L};
        return data;
    }
    public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
    private static long[] mk_tokenSet_4_()
    {
        long[] data = { 8946399560259862528L, 6917537823733891071L, 130686L, 0L, 0L, 0L};
        return data;
    }
    public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
    private static long[] mk_tokenSet_5_()
    {
        long[] data = { -222929281066467326L, -1L, 130815L, 0L, 0L, 0L};
        return data;
    }
    public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
    private static long[] mk_tokenSet_6_()
    {
        long[] data = { 1450325106269093888L, 6917529027909531647L, 130686L, 0L, 0L, 0L};
        return data;
    }
    public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
    private static long[] mk_tokenSet_7_()
    {
        long[] data = { 8946399560259862528L, -1L, 130815L, 0L, 0L, 0L};
        return data;
    }
    public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
    private static long[] mk_tokenSet_8_()
    {
        long[] data = { 1450325106269093888L, 6917529027909526511L, 130686L, 0L, 0L, 0L};
        return data;
    }
    public static readonly BitSet tokenSet_8_ = new BitSet(mk_tokenSet_8_());
    private static long[] mk_tokenSet_9_()
    {
        long[] data = { 6062116677812748288L, -268224513L, 130815L, 0L, 0L, 0L};
        return data;
    }
    public static readonly BitSet tokenSet_9_ = new BitSet(mk_tokenSet_9_());
    private static long[] mk_tokenSet_10_()
    {
        long[] data = { 1450320708222582784L, 6917529027909517312L, 130686L, 0L, 0L, 0L};
        return data;
    }
    public static readonly BitSet tokenSet_10_ = new BitSet(mk_tokenSet_10_());
    private static long[] mk_tokenSet_11_()
    {
        long[] data = { 297397004592480256L, 6917529027909648384L, 130942L, 0L, 0L, 0L};
        return data;
    }
    public static readonly BitSet tokenSet_11_ = new BitSet(mk_tokenSet_11_());
    private static long[] mk_tokenSet_12_()
    {
        long[] data = { 8946399560259862528L, -16385L, 130815L, 0L, 0L, 0L};
        return data;
    }
    public static readonly BitSet tokenSet_12_ = new BitSet(mk_tokenSet_12_());
    private static long[] mk_tokenSet_13_()
    {
        long[] data = { 297678479569190912L, 6917529027909517312L, 130686L, 0L, 0L, 0L};
        return data;
    }
    public static readonly BitSet tokenSet_13_ = new BitSet(mk_tokenSet_13_());
    private static long[] mk_tokenSet_14_()
    {
        long[] data = { -222929281066467326L, -1L, 131071L, 0L, 0L, 0L};
        return data;
    }
    public static readonly BitSet tokenSet_14_ = new BitSet(mk_tokenSet_14_());
    
}
}
