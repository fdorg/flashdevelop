header{

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

}
options {
	language  = "CSharp";
	namespace = "AS3Context.Grammar";
}

/**
 *	@author Martin Schnabel
 */
class AS3Parser extends Parser;

options {
   classHeaderPrefix = "public";
	k = 2;
	exportVocab = AS3;
	buildAST = true;
	defaultErrorHandler=false;
}

tokens {
	COMPILATION_UNIT;
	IMPORT;
	CLASS_DEF; INTERFACE_DEF;
	EXTENDS_CLAUSE; IMPLEMENTS_CLAUSE; TYPE_BLOCK;
	MODIFIERS; VARIABLE_DEF; METHOD_DEF; NAMESPACE_DEF; PARAMS; PARAM; TYPE_SPEC;
	BLOCK; EXPR; ELIST; EXPR_STMNT;
	NEW_EXPR; ENCPS_EXPR;
	VAR_INIT;
	METHOD_CALL; ARRAY_ACC;
	UNARY_PLUS; UNARY_MINUS; POST_INC; POST_DEC;
	ARRAY_LITERAL; ELEMENT; OBJECT_LITERAL; OBJECT_FIELD; FUNC_DEF;
	FOR_INIT; FOR_CONDITION; FOR_ITERATOR;
}
{
		private void setRegionInfo(AST ast,AST startAST,AST endAST){
			// do something with the model
		}
}
/**
 * this is the start rule for this parser
 */
compilationUnit
	:	"package" (identifier)?
		LCURLY!
		(	importDefinition
		|	metadataDefinition
		|	mods:modifiers!
			(	classDefinition[#mods]
			|	interfaceDefinition[#mods]
			|	variableDefinition[#mods]
			|	methodDefinition[#mods]
			|	namespaceDefinition[#mods]
			)
		|	SEMI!
		)*
		RCURLY!
		EOF!
		{## = #(#[COMPILATION_UNIT,"COMPILATION_UNIT"],##);}
	;

importDefinition
	:	s:"import"! e:identifierStar
		{## = #(#[IMPORT,"IMPORT"],##);
		setRegionInfo(##,#s,#e);}
	;

metadataDefinition
	:	LBRACK
		(	metadataItem
			(COMMA! metadataItem)*
		)
		RBRACK
	;

metadataItem
	:	identifier
		(	LPAREN
			(	expression!
				(COMMA! expression)*
			)?
			RPAREN
		)?
	;

classDefinition[AST mods]
	:	s:"class"! identifier
		extendsClause
		implementsClause
		e:typeBlock
		{## = #(#[CLASS_DEF,"CLASS_DEF"],##,mods);
		setRegionInfo(##,#s,#e);}
	;

interfaceDefinition[AST mods]
	:	s:"interface"! identifier
		extendsClause
		e:typeBlock
		{## = #(#[INTERFACE_DEF,"INTERFACE_DEF"],##,mods);
		setRegionInfo(##,#s,#e);}
	;
extendsClause
	:	(s:"extends"! e:identifier)?
		{## = #(#[EXTENDS_CLAUSE,"EXTENDS_CLAUSE"],##);
		setRegionInfo(##,#s,#e);}
	;
implementsClause
	:	(s:"implements"! e1:identifier ( COMMA! e2:identifier)*)*
		{## = #(#[IMPLEMENTS_CLAUSE,"IMPLEMENTS_CLAUSE"],##);
		setRegionInfo(##,#s,#e2==null?#e1:#e2);}
	;
typeBlock
	:	s:LCURLY!
		(	metadataDefinition
		|	m:modifiers!
			(	variableDefinition[#m]
			|	methodDefinition[#m]
			)
			|SEMI!
		)*
		e:RCURLY!
		{## = #([TYPE_BLOCK, "TYPE_BLOCK"], ##);
		setRegionInfo(##,#s,#e);}
	;
methodDefinition[AST mods]
	:	s:"function"! (("get"|"set") ~("/n"|"/r"))? IDENT
		parameterDeclarationList
		e1:typeExpression
		(e2:block)?
		{## = #([METHOD_DEF, "METHOD_DEF"],##,mods);
		setRegionInfo(##,#s,#e2==null?#e1:#e2);}
	;
namespaceDefinition[AST mods]
	:	s:"namespace"! e:IDENT
		{## = #([NAMESPACE_DEF, "NAMESPACE_DEF"],##,mods);
		setRegionInfo(##,#s,#e);}
	;
variableDefinition[AST mods]
	:	("var"!|"const"!) variableDeclarator[getASTFactory().dupTree(mods)]
		(COMMA!	variableDeclarator[getASTFactory().dupTree(mods)])*
	;

variableDeclarator[AST mods]
    :   s:IDENT e1:typeExpression (e2:variableInitializer)?
        {## = #(#[VARIABLE_DEF,"VARIABLE_DEF"],##,mods);
		setRegionInfo(##,#s,#e2==null?#e1:#e2);}
    ;
// A declaration is the creation of a reference or primitive-type variable
// Create a separate Type/Var tree for each var in the var list.
declaration
	:	"var"! variableDeclarator[null]
		(COMMA!	variableDeclarator[null])*
	;
variableInitializer
	:	s:ASSIGN^ e:expression
		{setRegionInfo(##,#s,#e);}
	;
// A list of formal parameters
parameterDeclarationList
	:	s:LPAREN!
		(	( parameterDeclaration | parameterRestDeclaration)
			(	COMMA! ( parameterDeclaration | parameterRestDeclaration))*
		)?
		e:RPAREN!
		{## = #([PARAMS, "PARAMS"], ##);
		setRegionInfo(##,#s,#e);}
	;

parameterDeclaration
	:	("const")? IDENT typeExpression (ASSIGN assignmentExpression)?
		{## = #([PARAM,"PARAM"],##);}
	;
parameterRestDeclaration
	:	REST ("const")? IDENT
		{## = #([PARAM,"PARAM"],##);}
	;
block
	:	s:LCURLY! (statement)* e:RCURLY!
		{## = #([BLOCK, "BLOCK"], ##);
		setRegionInfo(##,#s,#e);}
	;

statement
	:	{LA(1)==LCURLY}?(block)
	|	s1:declaration ({LA(1)==SEMI}?(e1:SEMI!))?
		{setRegionInfo(##,#s1,#e1);}
	|	s2:assignmentExpression ({LA(1)==SEMI}?(e2:SEMI!))?
		{## = #([EXPR_STMNT, "EXPR_STMNT"], ##);
		setRegionInfo(##,#s2,#e2);}
	|	ifStatement
	// For statement
	|	forStatement

	// While statement
	|	"while"^ LPAREN! expression RPAREN! statement

	// do-while statement
	|	"do"^ statement "while"! LPAREN! expression RPAREN!

	// with statement
	|	"with"^ LPAREN! expression RPAREN! statement

	// switch statement
	|	switchStatement

	// get out of a loop (or switch)
	|	breakStatement

	// do next iteration of a loop
	|	continueStatement

	// Return an expression
	|	returnStatement

	// throw an exception
	|	throwStatement

	// empty statement
	|	SEMI!
	;

ifStatement
	:	s:"if"^ LPAREN! expression RPAREN! e1:statement
		(	options {greedy=true;}: e2:elseStatement)*
		{setRegionInfo(##,#s,#e2==null?#e1:#e2);}
	;

elseStatement
	:	s:"else"^ e:statement
		{setRegionInfo(##,#s,#e);}
	;
throwStatement
	:	s:"throw"^ e1:expression ({LA(1)==SEMI}?(e2:SEMI!))?
		{setRegionInfo(##,#s,#e2==null?#e1:#e2);}
	;

returnStatement
	:	s:"return"^ (options {greedy=true;} :e1:expression)? ({LA(1)==SEMI}?(e2:SEMI!))?
		{setRegionInfo(##,#s,#e2==null?#e1:#e2);}
	;

continueStatement
	:	s:"continue"^ ({LA(1)==SEMI}?(e:SEMI!))?
		{setRegionInfo(##,#s,#e);}
	;

breakStatement
	:	s:"break"^( {LA(1)==SEMI}?(e:SEMI!))?
		{setRegionInfo(##,#s,#e);}
	;

switchStatement
	:	s:"switch"^ LPAREN! expression RPAREN!
		e:switchBlock
		{setRegionInfo(##,#s,#e);}
	;

switchBlock
	:	s:LCURLY!
		(caseStatement)*
		(defaultStatement)?
		e:RCURLY!
		{## = #(#[BLOCK,"BLOCK"], ##);
		setRegionInfo(##,#s,#e);}
	;

caseStatement
	:	s:"case"^ expression e1:COLON! (e2:statement)*
		{setRegionInfo(##,#s,#e2==null?#e1:#e2);}
	;

defaultStatement
	:	s:"default"^ e1:COLON! (e2:statement)*
		{setRegionInfo(##,#s,#e2==null?#e1:#e2);}
	;
forStatement
	:	s:"for"^
		(	(LPAREN forInit SEMI)=>traditionalForClause
		|	("each" forInClause)
		|	forInClause
		)
		e:statement					 // statement to loop over
		{setRegionInfo(##,#s,#e);}
	;
traditionalForClause
	:	s:LPAREN!
		forInit SEMI!	// initializer
		forCond SEMI!	// condition test
		forIter			// updater
		e:RPAREN!
		{setRegionInfo(##,#s,#e);}
	;

forInClause
	:	s:LPAREN!
		declaration "in" expression
		e:RPAREN!
		{setRegionInfo(##,#s,#e);}
	;
// The initializer for a for loop
forInit
	:	((declaration)=> s1:declaration | s2:expressionList )?
		{## = #(#[FOR_INIT,"FOR_INIT"],##);
		setRegionInfo(##,#s2==null?#s1:#s2,null);}
	;

forCond
	:	(s:expression)?
		{## = #(#[FOR_CONDITION,"FOR_CONDITION"],##);
		setRegionInfo(##,#s,null);}
	;

forIter
	:	(s:expressionList)?
		{## = #(#[FOR_ITERATOR,"FOR_ITERATOR"],##);
		setRegionInfo(##,#s,null);}
	;

typeExpression
	:	(	COLON!
			identifier
			{## = #([TYPE_SPEC,"TYPE_SPEC"],##);}
		)?
	;

identifier
	:	s:IDENT^
		(	options{greedy=true;}
		: 	DOT! e:IDENT^
			{setRegionInfo(##,#s,#e);}
		)*
		{setRegionInfo(##,#s,#e);}
	;

identifierStar
	:	s:IDENT^
		(	options{greedy=true;}
		:	DOT! e1:IDENT^
			{setRegionInfo(##,#s,#e1);}
		)*
		(	DOT! e2:STAR^
			{setRegionInfo(##,#s,#e2);}
		)?
		{setRegionInfo(##,#s,#e2==null?#e1:#e2);}
	;
modifiers
	:	( s:modifier (e:modifier)* )?
		{## = #([MODIFIERS, "MODIFIERS"],##);
		setRegionInfo(##,#s,#e);}
	;
modifier
	:	"public"
	|	"private"
	|	"protected"
	|	"internal"
	|	"static"
	|	"final"
	|	"enumerable"
	|	"explicit"
	|	"override"
	|	"dynamic"
	;
arguments
	:	(	expressionList
		|	/*nothing*/
			{#arguments = #[ELIST,"ELIST"];}
		)
	;
// This is an initializer used to set up an array.
arrayLiteral
	:	LBRACK (elementList)? RBRACK
	;

elementList
	:	COMMA
	|	nonemptyElementList
	;
nonemptyElementList
	:	assignmentExpression (COMMA assignmentExpression)*
	;

element
	:	s:assignmentExpression
		{## = #([ELEMENT,"ELEMENT"],##);
		setRegionInfo(##,#s,null);}
	;

// This is an initializer used to set up an object.
objectLiteral
	:	s:LCURLY! (fieldList)? e:RCURLY!
		{## = #([OBJECT_LITERAL,"OBJECT_LITERAL"],##);
		setRegionInfo(##,#s,#e);}
	;

fieldList
	:	literalField (COMMA! (literalField)?)*
	;

literalField
	: 	s:fieldName COLON! e:element
		{## = #([OBJECT_FIELD,"OBJECT_FIELD"],##);
		setRegionInfo(##,#s,#e);}
	;

fieldName
	:	IDENT
	|	NUMBER
	;

// the mother of all expressions
expression
	:	s:assignmentExpression
		{## = #(#[EXPR,"EXPR"],##);
		setRegionInfo(##,#s,null);}
	;

// This is a list of expressions.
expressionList
	:	s:expression (COMMA! e:expression)*
		{## = #(#[ELIST,"ELIST"], ##);
		setRegionInfo(##,#s,#e);}
	;

// assignment expression (level 13)
assignmentExpression
	:	s:conditionalExpression
	(	(	(	ASSIGN
			| 	STAR_ASSIGN
			|	DIV_ASSIGN
			|	MOD_ASSIGN
			|	PLUS_ASSIGN
			|	MINUS_ASSIGN
			|	SL_ASSIGN
			|	SR_ASSIGN
			|	BSR_ASSIGN
			|	BAND_ASSIGN
			|	BXOR_ASSIGN
			|	BOR_ASSIGN
			|	LAND_ASSIGN
			|	LOR_ASSIGN
			)	~ASSIGN
		)=>
			(	ASSIGN^
			| 	STAR_ASSIGN^
			|	DIV_ASSIGN^
			|	MOD_ASSIGN^
			|	PLUS_ASSIGN^
			|	MINUS_ASSIGN^
			|	SL_ASSIGN^
			|	SR_ASSIGN^
			|	BSR_ASSIGN^
			|	BAND_ASSIGN^
			|	BXOR_ASSIGN^
			|	BOR_ASSIGN^
			|	LAND_ASSIGN^
			|	LOR_ASSIGN^
			)
		e:assignmentExpression
		{setRegionInfo(##,#s,#e);}
	)?
	;
// conditional test (level 12)
conditionalExpression
	:	s:logicalOrExpression
		(	options {greedy=true;}
		:	QUESTION^
			e:conditionalSubExpression
			{setRegionInfo(##,#s,#e);}
		)*
	;
conditionalSubExpression
	:	s:assignmentExpression COLON^ e:assignmentExpression
		{setRegionInfo(##,#s,#e);}
	;
// logical or (||)  (level 11)
logicalOrExpression
	:	logicalAndExpression (LOR^ logicalAndExpression)*
	;
// logical and (&&)  (level 10)
logicalAndExpression
	:	bitwiseOrExpression (LAND^ bitwiseOrExpression)*
	;
// bitwise or non-short-circuiting or (|)  (level 9)
bitwiseOrExpression
	:	bitwiseXorExpression (BOR^ bitwiseXorExpression)*
	;
// exclusive or (^)  (level 8)
bitwiseXorExpression
	:	bitwiseAndExpression (BXOR^ bitwiseAndExpression)*
	;
// bitwise or non-short-circuiting and (&)  (level 7)
bitwiseAndExpression
	:	equalityExpression (BAND^ equalityExpression)*
	;
// equality/inequality (==/!=) (level 6)
equalityExpression
	:	relationalExpression
	(	( STRICT_EQUAL^ | STRICT_NOT_EQUAL^ | NOT_EQUAL^ | EQUAL^ )
		relationalExpression
	)*
	;

// boolean relational expressions (level 5)
relationalExpression
	:	shiftExpression ((LWT^ | GT^ | LE^ | GE^ | "is"^ | "as"^) shiftExpression)*
	;

// bit shift expressions (level 4)
shiftExpression
	:	additiveExpression ((SL^ | SR^ | BSR^) additiveExpression)*
	;

// binary addition/subtraction (level 3)
additiveExpression
	:	multiplicativeExpression (options {greedy=true;} :(PLUS^ | MINUS^) multiplicativeExpression)*
	;
// multiplication/division/modulo (level 2)
multiplicativeExpression
	:	unaryExpression ((STAR^ | DIV^ | MOD^) unaryExpression)*
	;
//	(level 1)
unaryExpression
	:	INC^ unaryExpression
	|	DEC^ unaryExpression
	|	MINUS^ {#MINUS.setType(UNARY_MINUS);} unaryExpression
	|	PLUS^ {#PLUS.setType(UNARY_PLUS);} unaryExpression
	|	unaryExpressionNotPlusMinus
	;
unaryExpressionNotPlusMinus
	:	"delete" postfixExpression
	//|	"void" unaryExpression
	|	"typeof" unaryExpression
	|	LNOT^ unaryExpression
	|	BNOT^ unaryExpression
	|	postfixExpression
	;

// qualified names, array expressions, method invocation, post inc/dec
postfixExpression
	:	s:primaryExpression
		(	options{greedy=true;} :
			(	DOT!
				(	options {generateAmbigWarnings=false;}
				:	e11:IDENT^
					{setRegionInfo(#e11,#s,#e11);}
				|	e12:e4xExpression
					{setRegionInfo(#e12,#s,#e12);}
				)
			|	(	t1:LBRACK^ expression e2:RBRACK!
					{#t1.setType(ARRAY_ACC);
					#t1.setText("ARRAY_ACC");
					setRegionInfo(#t1,#s,#e2);}
				)
			|	E4X_DESC^ e4xExpression
			)
			(	options {greedy=true;}
			:	t2:LPAREN^ arguments e3:RPAREN!
				{#t2.setType(METHOD_CALL);
				#t2.setText("METHOD_CALL");
				setRegionInfo(#t2,#s,#e3);}
				(	options {greedy=true;}
				:	t3:LPAREN^ arguments e4:RPAREN!
					{#t3.setType(METHOD_CALL);
					#t3.setText("METHOD_CALL");
					setRegionInfo(#t3,#s,#e4);}
				)*
			)?
		)*
		(	options {greedy=true;}
		: 	oin:INC^ {#oin.setType(POST_INC);}
	 	|	ode:DEC^ {#ode.setType(POST_DEC);}
		)?
 	;
e4xExpression
	:	IDENT
	|	e12:STAR^
	|	e13:E4X_ATTRI^
		(	IDENT
		|	(STAR)? t1:LBRACK^ expression e2:RBRACK!
		)
	;
primaryExpression
	:	identPrimary
	|	"null"
	|	"true"
	|	"false"
	|	"undefined"
    |   constant
	|	arrayLiteral
	|	objectLiteral
    |	functionDefinition
    |	newExpression
    |	encapsulatedExpression
	;
identPrimary
	:	s:IDENT
		(	options{greedy=true;}
		:	DOT! e1:IDENT^
			{setRegionInfo(#e1,#s,#e1);}
		|	(	t1:LBRACK^ expression e2:RBRACK!
				{#t1.setType(ARRAY_ACC);
				#t1.setText("ARRAY_ACC");
				setRegionInfo(#t1,#s,#e2);}
			)
		)*
		(	options {greedy=true;}
		:	t2:LPAREN^ arguments e3:RPAREN!
			{#t2.setType(METHOD_CALL);
			#t2.setText("METHOD_CALL");
			setRegionInfo(#t2,#s,#e3);}
		)*
    ;

constant
	:	NUMBER
	|	STRING_LITERAL
	|	REGEX_LITERAL
	|	XML_LITERAL
	;
newExpression
	:	s:"new" e:postfixExpression
		{## = #([NEW_EXPR,"NEW_EXPR"],##);
		setRegionInfo(##,#s,#e);}
	;
encapsulatedExpression
	:	s:LPAREN! assignmentExpression e:RPAREN!
		{## = #([ENCPS_EXPR,"ENCPS_EXPR"],##);
		setRegionInfo(##,#s,#e);}
	;
functionDefinition
	:	s:"function" parameterDeclarationList typeExpression e:block
		{## = #([FUNC_DEF,"FUNC_DEF"],##);
		setRegionInfo(##,#s,#e);}
	;
class AS3Lexer extends Lexer;

options {
	exportVocab=AS3;      	// call the vocabulary "AS3"
	testLiterals=false;    	// don't automatically test for literals
	k=4;                   	// four characters of lookahead
	charVocabulary='\u0003'..'\u7FFE';
	codeGenBitsetTestThreshold=20;
	defaultErrorHandler=false;
}

// OPERATORS
QUESTION		:	'?'		;
LPAREN			:	'('		;
RPAREN			:	')'		;
LBRACK			:	'['		;
RBRACK			:	']'		;
LCURLY			:	'{'		;
RCURLY			:	'}'		;
COLON			:	':'		;
DBL_COLON		:	"::"	;
COMMA			:	','		;
ASSIGN			:	'='		;
EQUAL			:	"=="	;
STRICT_EQUAL	:	"==="	;
LNOT			:	'!'		;
BNOT			:	'~'		;
NOT_EQUAL		:	"!="	;
STRICT_NOT_EQUAL:	"!=="	;
DIV				:	'/'		;
DIV_ASSIGN		:	"/="	;
PLUS			:	'+'		;
PLUS_ASSIGN		:	"+="	;
INC				:	"++"	;
MINUS			:	'-'		;
MINUS_ASSIGN	:	"-="	;
DEC				:	"--"	;
STAR			:	'*'		;
STAR_ASSIGN		:	"*="	;
MOD				:	'%'		;
MOD_ASSIGN		:	"%="	;
SR				:	">>"	;
SR_ASSIGN		:	">>="	;
BSR				:	">>>"	;
BSR_ASSIGN		:	">>>="	;
GE				:	">="	;
GT				:	'>'		;
SL				:	"<<"	;
SL_ASSIGN		:	"<<="	;
LE				:	"<="	;
LWT				:	'<'		;
BXOR			:	'^'		;
BXOR_ASSIGN		:	"^="	;
BOR				:	'|'		;
BOR_ASSIGN		:	"|="	;
LOR				:	"||"	;
BAND			:	'&'		;
BAND_ASSIGN		:	"&="	;
LAND			:	"&&"	;
LAND_ASSIGN		:	"&&="	;
LOR_ASSIGN		:	"||="	;
E4X_ATTRI		:	'@'		;
SEMI			:	';'		;


private DOT			:	'.'		; //declared in NUMBER
private E4X_DESC	:	".."	; //declared in NUMBER
private REST		:	"..."	; //declared in NUMBER
// an identifier.  Note that testLiterals is set to true!  This means
// that after we match the rule, we look in the literals table to see
// if it's a literal or really an identifer
IDENT options {testLiterals=true;}
	:	('a'..'z'|'A'..'Z'|'_'|'$')
	(	options {generateAmbigWarnings=false;}
	:	'a'..'z'|'A'..'Z'|'_'|'0'..'9'|'$')*
	;

STRING_LITERAL
	:	'"' (ESC|~('"'|'\\'|'\n'|'\r'))* '"'
	|	'\'' (ESC|~('\''|'\\'|'\n'|'\r'))* '\''
	;
XML_LITERAL
	:	'<' IDENT (WS|XML_ATTRIBUTE)*
		(	'>'
			(options {generateAmbigWarnings=false;}
			:WS|XML_LITERAL|XML_TEXTNODE|XML_COMMENT|XML_CDATA)*
			"</" IDENT '>'
		|	"/>"
		)
	;

protected XML_ATTRIBUTE
	:	IDENT (WS)* ASSIGN (WS)* (STRING_LITERAL | XML_BINDING)
	;

protected XML_BINDING
	:	'{' XML_AS3_EXPRESSION '}'
	;

// it should be parsed as an AS3 expression...
protected XML_AS3_EXPRESSION
	:	(options {generateAmbigWarnings=false;}
		: (~('{'|'}'))*
		)
	;

protected XML_TEXTNODE
	:	(	options {generateAmbigWarnings=false;}
		:	NL
		|	{ LA(2)!='>' }? '/'
		|	~('<'|'{'|'/'|'\n'|'\r')
		)
	;
protected XML_COMMENT
	:	"<!--"
		(options {generateAmbigWarnings=false;}
		:	NL
		|	~('-'|'\n'|'\r')
		|	{ LA(2)!='-' }? '-'
		)*
		"-->"
	;
protected XML_CDATA
	:	"<![CDATA["
		(options {generateAmbigWarnings=false;}
		:	NL
		|	{ LA(2)!=']'}? ']'
		|	~(']'|'\n'|'\r')
		)*
		"]]>"
	;

NUMBER {Token t=null;} // dot could start a number
	:	"..." {_ttype = REST;}
	|	".." {_ttype = E4X_DESC;}
	|	'.' {_ttype = DOT;} (('0'..'9')+ (EXPONENT)? {_ttype = NUMBER;})?
	|	(	'0'
			(	('X'|'x') (options {warnWhenFollowAmbig = false;}:HEX_DIGIT)+
			|	(('0'..'9')* ('.' ('0'..'9')+)?  (EXPONENT)?)
			=>   ('0'..'9')* ('.' ('0'..'9')+)?  (EXPONENT)?
			|	('0'..'7')+
			)?
		|	('1'..'9') ('0'..'9')*
		{_ttype = NUMBER;}
		)
	;

REGEX_LITERAL
	: '/'REGEX_BODY'/' ('a'..'z'|'A'..'Z'|'_'|'0'..'9'|'$')*
	;
protected REGEX_BODY
	:	(	(~('\n'|'\r'|'*'|'/'|'\\'))
		|	'\\'(~('\n'|'\r'))
		)
		(	(~('\n'|'\r'|'/'|'\\'))
		|	'\\'(~('\n'|'\r'))
		)*
	;
// whitespace -- ignored
WS	:	(	options {generateAmbigWarnings=false;}
		:	' '
		|	'\t'
		|	'\f'
			// handle newlines
		|	NL
		)+
		{ _ttype = Token.SKIP; }
	;
protected NL
	:	(	options {generateAmbigWarnings=false;}
		:	'\r' '\n'  	// DOS
		|	'\r'    	// Mac
		|	'\n'    	// Unix
		)
		{ newline(); _ttype = Token.SKIP;}
	;

// skip BOM bytes
BOM	:	'\u00EF' | '\u00BB'|'\u00BF' { _ttype = Token.SKIP; };
// single-line comments
SL_COMMENT
	:	"//" (~('\n'|'\r'))* ('\n'|'\r'('\n')?)?
		{$setType(Token.SKIP); newline();}
	;
// multiple-line comments
ML_COMMENT
	:	"/*"
		(	options {generateAmbigWarnings=false;}
		:
			{ LA(2)!='/' }? '*'
		|	NL
		|	~('*'|'\n'|'\r')
		)*
		"*/"
		{$setType(Token.SKIP);}
	;
/* protected will not directly be
 * returned to the parser
 */
protected EXPONENT
	:	('e'|'E') ('+'|'-')? ('0'..'9')+
	;
protected HEX_DIGIT
	:	('0'..'9'|'A'..'F'|'a'..'f')
	;

protected ESC
	:	'\\'
		('n'|'r'|'t'|'b'|'f'|'"'|'\''|'\\'
		|	('u')+ HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT
		|	'0'..'3'
			( options{warnWhenFollowAmbig = false;}
			:	'0'..'7'( options{warnWhenFollowAmbig = false;}:'0'..'7')?
			)?
		|	'4'..'7'
			( options{warnWhenFollowAmbig = false;}
			:	'0'..'7'
			)?
		)
	;