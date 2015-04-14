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
    public class AS3TokenTypes
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
        
    }
}
