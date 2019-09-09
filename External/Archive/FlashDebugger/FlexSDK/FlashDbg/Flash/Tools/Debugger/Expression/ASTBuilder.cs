// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2003-2007 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using LocalizationManager = Flash.Localization.LocalizationManager;
using Trace = Flash.Util.Trace;

namespace Flash.Tools.Debugger.Expression
{
	
	/// <summary> ASTBuilder.java
	/// 
	/// This class creates an abstract syntax tree representation
	/// of an expression given a sequence of tokens.
	/// 
	/// The tree is built in a single pass in a fairly traditional
	/// manner using an expression stack and an operator stack to 
	/// convert to infix notation on the fly.
	/// 
	/// No compression is performed on the tree, thus expressions
	/// such as (3*4) will result in 3 nodes.
	/// 
	/// </summary>
    public class ParseException : Exception
    {
        public ParseException(String s, int pos)
            : base(s)
        {
            this.pos = pos;
        }

        int pos;
    }

	public class ASTBuilder : IASTBuilder
	{
		/// <returns> whether the fdb indirection operators are allowed, e.g. asterisk
		/// (*x) or trailing dot (x.)
		/// </returns>
		virtual public bool IndirectionOperatorAllowed
		{
			get
			{
				return m_isIndirectionOperatorAllowed;
			}
			
		}
		internal static LocalizationManager LocalizationManager
		{
			get
			{
				return m_localizationManager;
			}
			
		}
		private static LocalizationManager m_localizationManager;
		
		private System.Collections.Stack m_expStack;
        private System.Collections.Stack m_opStack;
		
		/* set when the reader has reached EOF (in parsing) */
		private bool m_readerEOF = false;
		private int m_parsePos = 0;
		
		/// <summary> whether the fdb indirection operators are allowed, e.g. asterisk (*x) or
		/// trailing dot (x.)
		/// </summary>
		private bool m_isIndirectionOperatorAllowed = true;
		
		/// <param name="isIndirectionOperatorAllowed">whether the fdb indirection operators are allowed, e.g.
		/// asterisk (*x) or trailing dot (x.)
		/// </param>
		public ASTBuilder(bool isIndirectionOperatorAllowed)
		{
			m_expStack = new System.Collections.Stack();
            m_opStack = new System.Collections.Stack();
			m_isIndirectionOperatorAllowed = isIndirectionOperatorAllowed;
		}
		
		/// <summary> The front-end has finished so we pop the 
		/// stack until it empty 
		/// </summary>
		private ValueExp done()
		{
            while (m_opStack.Count > 0)
                popOperation();
			
			/* should only have one entry on the expression stack */
			ValueExp tree = (ValueExp) m_expStack.Pop();
			
			if (m_expStack.Count != 0 || m_opStack.Count != 0)
			{
				/* clear the stacks */
				m_expStack.Clear();
				
				m_opStack.Clear();
				
				throw new IncompleteExpressionException();
			}
			
			return tree;
		}
		
		/// <summary> Here's where the good stuff happens...
		/// 
		/// 1. If we get an open parenthesis or open bracket, then we push it.
		/// 2. If we get a close parenthesis or close bracket, the we pop the stack
		/// growing our tree, until we find an open parenthesis or open bracket.
		/// The type must match, e.g. if we are processing a close parenthesis
		/// but encounter an open bracket, that indicates a syntax error in the
		/// expression, and we throw an exception.
		/// 3. If our op has lower precedence then the current TOS op
		/// we grow the tree and repeat (3).
		/// 
		/// Note we are using a trick, whereby the opType constants
		/// are number in order of precedence (that is lower precedence 
		/// = lower numeric value).  The exception to this rule is 
		/// the open parenthesis which is highest precedence but is
		/// lowest numerically.  This is to allow (3) to work correctly,
		/// since we want to make sure that a open parenthesis only gets
		/// popped by a close parenthesis.
		/// </summary>
		private void  addOp(Operator opType)
		{
			if (opType == Operator.OPEN_PAREN)
			{
				m_opStack.Push(opType);
			}
			else if (opType == Operator.OPEN_BRACKET)
			{
				// This is tricky: for an array index, e.g. "a[k]", we treat it
				// like "a SUBSCRIPT k".  So first we push SUBSCRIPT, and
				// then we push the open-bracket op.
				addOp(Operator.SUBSCRIPT); // recursive call
				m_opStack.Push(opType);
			}
			else if (opType == Operator.CLOSE_PAREN || opType == Operator.CLOSE_BRACKET)
			{
				Operator openingOpType;
				
				if (opType == Operator.CLOSE_PAREN)
					openingOpType = Operator.OPEN_PAREN;
				else
					openingOpType = Operator.OPEN_BRACKET;
				
				while (m_opStack.Peek() != Operator.OPEN_PAREN && m_opStack.Peek() != Operator.OPEN_BRACKET)
				{
					popOperation();
				}
				
				// If we saw "(x]" or "[x)", then indicate a syntax error
				if (m_opStack.Peek() != openingOpType)
					throw new ParseException(LocalizationManager.getLocalizedTextString("key1"), m_parsePos); //$NON-NLS-1$
				
				popOperation(); // pop the "(" or "["
			}
			else
			{
				while (m_opStack.Count != 0 && ((Operator)m_opStack.Peek()).precedence >= opType.precedence)
					popOperation();
				
				m_opStack.Push(opType);
			}
		}
		
		private void  addVariable(String name)
		{
			/* create a variable node push */
			VariableExp node = VariableExp.create(name);
			m_expStack.Push(node);
		}
		
		private void  addInternalVariable(String name)
		{
			/* create a variable node push */
			VariableExp node = InternalVariableExp.create(name);
            m_expStack.Push(node);
		}
		
		private void  addLong(long value)
		{
			/* create a constant node and push */
			ConstantExp node = ConstantExp.create(value);
            m_expStack.Push(node);
		}
		
		private void  addBoolean(bool value)
		{
			/* create a constant node and push */
			ConstantBooleanExp node = ConstantBooleanExp.create(value);
            m_expStack.Push(node);
		}
		
		private void  addString(String text)
		{
			StringExp node = StringExp.create(text);
            m_expStack.Push(node);
		}
		
		/// <summary> Pop the next operation off the stack and build a non-terminal node
		/// around it, poping the next two items from the expression stack 
		/// as its children 
		/// </summary>
		private void  popOperation()
		{
			Operator op = (Operator)m_opStack.Pop();
			
			/* dispose of stack place holder ops (e.g.. OPEN_PAREN and OPEN_BRACKET) */
			if (op.precedence < 0)
				return ;
			
			if (IndirectionOperatorAllowed)
			{
				/*
				* Special case to support trailing dot syntax (e.g. a.b. )
				* If DotOp and nothing on stack then we convert to
				* an indirection op
				*/
				if (op == Operator.DIRECT_SELECT && m_expStack.Count < 2)
					op = Operator.INDIRECTION;
			}
			
			// create operation and set its nodes
			NonTerminalExp node = op.createExpNode();

            node.RightChild = (ValueExp)m_expStack.Pop();
			
			if (!(node is SingleArgumentExp))
                node.LeftChild = (ValueExp)m_expStack.Pop();

            m_expStack.Push(node);
		}

		/*
		* @see Flash.Tools.Debugger.Expression.IASTBuilder#parse(java.io.Reader)
		*/
		public virtual ValueExp parse(TextReader inStream)
		{
            return parse(inStream, true);
		}
		
		/*
		* @see Flash.Tools.Debugger.Expression.IASTBuilder#parse(java.io.Reader, boolean)
		*/
		public virtual ValueExp parse(TextReader inStream, bool ignoreUnknownCharacters)
		{
			try
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				bool skipRead = false;
				bool inDot = false;
				char ch = ' ';
				
				m_readerEOF = false;
				m_parsePos = 0;
				
				while (!m_readerEOF)
				{
					if (!skipRead)
						ch = readChar(inStream);
					
					/* whitespace? => ignore */
					if (m_readerEOF || Char.IsWhiteSpace(ch))
					{
						skipRead = false;
						inDot = false;
					}
					/* A number? => parse constant */
					else if (!inDot && Char.IsDigit(ch))
					{
						/* build up our value */
						int numberBase = 10;
						long n = ch - '0';
						
						ch = readChar(inStream);
						
						/* support for hex values */
						if (ch == 'x' || ch == 'X')
						{
							numberBase = 16;
							ch = readChar(inStream);
						}
						
						while (Char.IsLetterOrDigit(ch))
						{
                            n = (n * numberBase) + charToLong(ch, numberBase);
							ch = readChar(inStream);
						}
						
						/* now add the constant */
						addLong(n);
						skipRead = true;
					}
					/* special demarcation for internal variables */
					else if (ch == '$')
					{
						sb.Length = 0;
						do 
						{
							sb.Append(ch);
							ch = readChar(inStream);
						}
						while (Util.isJavaIdentifierPart(ch));
						
						/* now add it */
						addInternalVariable(sb.ToString());
						sb.Length = 0;
						skipRead = true;
					}
					/* letter? => parse variable, accept #N, where N is entity id */
					else if ((Char.IsLetter(ch) || ch.CompareTo('$') == 0 || ch.CompareTo('_') == 0) || ch == '#' || (Char.IsDigit(ch) && inDot))
					{
						sb.Length = 0;
						do 
						{
							sb.Append(ch);
							ch = readChar(inStream);
						}
						while (Util.isJavaIdentifierPart(ch));
						
						/* now add it ; true/false look like variables but are not */
						String s = sb.ToString();
						if (s.Equals("true"))
						//$NON-NLS-1$
							addBoolean(true);
						else if (s.Equals("false"))
						//$NON-NLS-1$
							addBoolean(false);
						else
							addVariable(s);
						
						sb.Length = 0;
						skipRead = true;
					}
					/* quote? => parse string */
					else if (ch == '\'' || ch == '\"')
					{
						/* go until we find matching */
						char matching = ch;
						
						do 
						{
							ch = readChar(inStream);
							sb.Append(ch);
						}
						while (!m_readerEOF && ch != matching);
						
						/* add it */
						int to = sb.Length - 1;
						addString(sb.ToString().Substring(0, (to) - (0)));
						sb.Length = 0;
						skipRead = false;
					}
					else if (inDot && ch == '*')
					{
						// This is for the XML syntax, myXmlObject.*
						// to refer to all children of an XML object
						addVariable("*"); //$NON-NLS-1$
						inDot = false;
						skipRead = false;
					}
					/* could be an operator? */
					else
					{
						/* do a lookup */
						char lookaheadCh = readChar(inStream);
						Operator op = Operator.opFor(ch, lookaheadCh, IndirectionOperatorAllowed);
						
						if (op == Operator.UNKNOWN && !ignoreUnknownCharacters)
						{
							System.Collections.IDictionary args = new System.Collections.Hashtable();
							args["arg1"] = "" + ch; //$NON-NLS-1$ //$NON-NLS-2$
							throw new ParseException(LocalizationManager.getLocalizedTextString("key2", args), m_parsePos); //$NON-NLS-1$
						}
						else
						{
							addOp(op);
							skipRead = (op.token.Length == 1 || op == Operator.INDIRECTION)?true:false; /* did we consume the character? */
							if (skipRead)
								ch = lookaheadCh;
							inDot = (op == Operator.DIRECT_SELECT)?true:false;
						}
					}
				}
				
				/* now return the root node of the tree */
				return done();
			}
			finally
			{
				// We need to do this in case any exceptions were thrown, so that the
				// next time we're called to evaluate an expression, the stacks are
				// empty.
				m_expStack.Clear();
				m_opStack.Clear();
			}
		}
		
        /* convert text digit to numeric value */
        private long charToLong(char ch, int numberBase)
        {
            if (Char.IsDigit(ch))
            {
                return (long)(ch - '0');
            }
            else if (numberBase == 16)
            {
                ch = Char.ToLower(ch);

                if ('a' <= ch && ch <= 'f')
                {
                    return (long)(ch - 'a' + 10);
                }
            }

            throw new FormatException();
        }

		/* read a character from a reader, throw end of stream exception if done */
		private char readChar(TextReader inStream)
		{
			int c = (char) ' ';
			
			if (!m_readerEOF)
			{
				c = inStream.Read();
				m_parsePos++;
				if (c < 0)
					m_readerEOF = true;
			}
			return (char) c;
		}
		
		[STAThread]
		public static void  Main(String[] args)
		{
			ASTBuilder ab = new ASTBuilder(true);
			
			try
			{
				ab.addLong(5);
				ab.addOp(Operator.ARITH_SUB);
				ab.addLong(6);
				
				ValueExp exp1 = ab.done();
				
				ab.addLong(5);
				ab.addOp(Operator.ARITH_ADD);
				ab.addOp(Operator.OPEN_PAREN);
				ab.addLong(6);
				ab.addOp(Operator.ARITH_DIV);
				ab.addLong(4);
				ab.addOp(Operator.ARITH_MULT);
				ab.addLong(7);
				ab.addOp(Operator.CLOSE_PAREN);
				ab.addOp(Operator.BITWISE_RSHIFT);
				ab.addLong(2);
				
				ValueExp exp2 = ab.done();
				
				ValueExp exp3 = ab.parse(new StringReader("5-6")); //$NON-NLS-1$
				ValueExp exp4 = ab.parse(new StringReader("5 +(6/4*7 )>>2")); //$NON-NLS-1$
				
				ValueExp exp5 = ab.parse(new StringReader(" 4 == 2")); //$NON-NLS-1$
				
				Object o1 = exp1.evaluate(null);
				Object o2 = exp2.evaluate(null);
				Object o3 = exp3.evaluate(null);
				Object o4 = exp4.evaluate(null);
				Object o5 = exp5.evaluate(null);
				
				Console.Out.WriteLine("=" + o1 + "," + o2); //$NON-NLS-1$ //$NON-NLS-2$
				Console.Out.WriteLine("=" + o3 + "," + o4); //$NON-NLS-1$ //$NON-NLS-2$
				Console.Out.WriteLine("=" + o5); //$NON-NLS-1$
			}
			catch (Exception e)
			{
                if (Trace.error)
                {
                    Console.Error.Write(e.StackTrace);
                    Console.Error.Flush();
                }
			}
		}
		static ASTBuilder()
		{
			{
				// set up for localizing messages
				m_localizationManager = new LocalizationManager();
				m_localizationManager.addLocalizer(new DebuggerLocalizer("Flash.Tools.Debugger.Expression.expression.")); //$NON-NLS-1$
			}
		}
	}
}
