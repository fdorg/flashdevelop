////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2003-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Diagnostics;
using System.Reflection;

using Trace = Flash.Util.Trace;

namespace Flash.Tools.Debugger.Expression
{
	/// <summary> Provides access to Operator related actions. </summary>
	public class Operator
	{
		public int precedence;
		public String name;
		public String token; // never null, but sometimes the empty string
		private System.Type expressionNodeClass; // a subclass of NonTerminalExp corresponding to this Operator
		
		private static System.Collections.Hashtable mapTokenToOperator = new System.Collections.Hashtable(); // map of String to Operator
		
		/// <param name="precedence">the operator precedence relative to other operators
		/// </param>
		/// <param name="name">the name of the operator, used by toString() and thus by UnknownOperationException
		/// </param>
		/// <param name="token">the actual characters of the operator, used to find this operator in the expression text
		/// </param>
		/// <param name="expressionNodeClass">the subclass of NonTerminalExp which should be created to represent this operator
		/// </param>
		private Operator(int precedence, String name, String token, System.Type expressionNodeClass)
		{
			Debug.Assert(name != null || token != null, "Either the name or the token must be non-null; //$NON-NLS-1$");
			//Debug.Assert(expressionNodeClass == null || NonTerminalExp..class.isAssignableFrom(expressionNodeClass), "expressionNodeClass must be a subclass of NonTerminalExp; //$NON-NLS-1$");
		
		    // 'token' can be null, but 'this.token' never is
		    if(token == null) 
    		    token = ""; //$NON-NLS-1$
		
		    // if the name wasn't specified, use the token as the name
		    if(name == null) 
	    	    name = token;
		
		    this.precedence = precedence;
		    this.name = name;
		    this.token = token;
		    this.expressionNodeClass = expressionNodeClass;
    		
		    if(token.Length > 0) 
	    	    mapTokenToOperator.Add(token, this);
	    }
	
	    public String toString()
	    { 
		    return name;
	    }
	
	    /// <summary> These constants represent the various operators that are supported as 
	    /// subclasses of this node.
	    /// 
	    /// They are numerically in order of precedence, with numbers meaning higher
	    /// precedence.  The only exceptions are open-paren and open-bracket, which
	    /// we've placed at an artifically low precedence in order allow the infix
	    /// converter logic to work more easily.
	    /// 
	    /// The numbering is derived from Harbison's "C: A Reference Manual," Table 7-3.
	    /// Gaps are left for operators we don't support.
	    /// </summary>
	    public static Operator CLOSE_BRACKET = new Operator(-2, null, "]", null); //$NON-NLS-1$
	    public static Operator CLOSE_PAREN = new Operator(-2, null, ")", null); //$NON-NLS-1$
	    public static Operator OPEN_BRACKET = new Operator(-1, null, "[", null); //$NON-NLS-1$
	    public static Operator OPEN_PAREN = new Operator(-1, null, "(", null); //$NON-NLS-1$
	    public static Operator UNKNOWN = new Operator(0, "Operator.UNKNOWN", null, null); //$NON-NLS-1$

        public static Operator ASSIGNMENT = new Operator(2, null, "=", Type.GetType("Flash.Tools.Debugger.Expression.AssignmentExp")); //$NON-NLS-1$
        public static Operator LOGICAL_OR = new Operator(4, null, "||", Type.GetType("Flash.Tools.Debugger.Expression.LogicOrExp")); //$NON-NLS-1$
        public static Operator LOGICAL_AND = new Operator(5, null, "&&", Type.GetType("Flash.Tools.Debugger.Expression.LogicAndExp")); //$NON-NLS-1$
        public static Operator BITWISE_OR = new Operator(6, null, "|", Type.GetType("Flash.Tools.Debugger.Expression.OrExp")); //$NON-NLS-1$
        public static Operator BITWISE_XOR = new Operator(7, null, "^", Type.GetType("Flash.Tools.Debugger.Expression.XorExp")); //$NON-NLS-1$
        public static Operator BITWISE_AND = new Operator(8, null, "&", Type.GetType("Flash.Tools.Debugger.Expression.AndExp")); //$NON-NLS-1$
        public static Operator RELATION_EQ = new Operator(9, null, "==", Type.GetType("Flash.Tools.Debugger.Expression.EqExp")); //$NON-NLS-1$
        public static Operator RELATION_NEQ = new Operator(9, null, "!=", Type.GetType("Flash.Tools.Debugger.Expression.NeqExp")); //$NON-NLS-1$
        public static Operator RELATION_LT = new Operator(10, null, "<", Type.GetType("Flash.Tools.Debugger.Expression.LTExp")); //$NON-NLS-1$
        public static Operator RELATION_GT = new Operator(10, null, ">", Type.GetType("Flash.Tools.Debugger.Expression.GTExp")); //$NON-NLS-1$
        public static Operator RELATION_LTEQ = new Operator(10, null, "<=", Type.GetType("Flash.Tools.Debugger.Expression.LTEqExp")); //$NON-NLS-1$
        public static Operator RELATION_GTEQ = new Operator(10, null, ">=", Type.GetType("Flash.Tools.Debugger.Expression.GTEqExp")); //$NON-NLS-1$
        public static Operator BITWISE_LSHIFT = new Operator(11, null, "<<", Type.GetType("Flash.Tools.Debugger.Expression.LShiftExp")); //$NON-NLS-1$
        public static Operator BITWISE_RSHIFT = new Operator(11, null, ">>", Type.GetType("Flash.Tools.Debugger.Expression.RShiftExp")); //$NON-NLS-1$
        public static Operator ARITH_ADD = new Operator(12, null, "+", Type.GetType("Flash.Tools.Debugger.Expression.AddExp")); //$NON-NLS-1$
        public static Operator ARITH_SUB = new Operator(12, null, "-", Type.GetType("Flash.Tools.Debugger.Expression.SubExp")); //$NON-NLS-1$
        public static Operator ARITH_MULT = new Operator(13, null, "*", Type.GetType("Flash.Tools.Debugger.Expression.MultExp")); //$NON-NLS-1$
        public static Operator ARITH_DIV = new Operator(13, null, "/", Type.GetType("Flash.Tools.Debugger.Expression.DivExp")); //$NON-NLS-1$
        public static Operator ARITH_MOD = new Operator(13, null, "%", Type.GetType("Flash.Tools.Debugger.Expression.ModExp")); //$NON-NLS-1$

        public static Operator INDIRECTION = new Operator(15, "Operator.INDIRECTION", null, Type.GetType("Flash.Tools.Debugger.Expression.IndirectionExp")); // *a or a. //$NON-NLS-1$

        public static Operator LOGICAL_NOT = new Operator(15, null, "!", Type.GetType("Flash.Tools.Debugger.Expression.LogicNotExp")); //$NON-NLS-1$
        public static Operator BITWISE_NOT = new Operator(15, null, "~", Type.GetType("Flash.Tools.Debugger.Expression.NotExp")); //$NON-NLS-1$

        public static Operator DIRECT_SELECT = new Operator(17, null, ".", Type.GetType("Flash.Tools.Debugger.Expression.DotExp")); // a.b //$NON-NLS-1$
        public static Operator SUBSCRIPT = new Operator(17, "Operator.SUBSCRIPT", null, Type.GetType("Flash.Tools.Debugger.Expression.SubscriptExp")); // a[k]; see ASTBuilder.addOp() //$NON-NLS-1$
    	
	    /// <summary> We create an empty non-terminal node of the given type based on the
	    /// operator.
	    /// </summary>
	    public NonTerminalExp createExpNode() // throws UnknownOperationException
	    { 
		    NonTerminalExp node = null;
    	
	        if (expressionNodeClass == null)
	        { 
		        throw new UnknownOperationException(this);
	        } 
	        else
	        {
	            try
	            { 
		            node = (NonTerminalExp) expressionNodeClass.GetConstructor(Type.EmptyTypes).Invoke(null);
	            }
                catch(OutOfMemoryException e)
                {
                    // should never happen
                    if(Trace.error) 
                        Trace.trace(e.Message);
                }
                catch(AccessViolationException e)
                {
                    // should never happen
                    if(Trace.error)
                        Trace.trace(e.Message);
                }
            } 

            return node;
        }
    	
        /// <summary> Given two characters, find out which operator they refer to. We may only
        /// use the first character. The caller can figure out how many characters we
        /// used by looking at the length of Operator.token for the returned
        /// Operator.
        /// 
        /// </summary>
        /// <param name="firstCh">the first character
        /// </param>
        /// <param name="secondCh">the second character; may or may not be used
        /// </param>
        /// <param name="isIndirectionOperatorAllowed">whether the fdb indirection operators are allowed: asterisk
        /// (*x) or trailing dot (x.)
        /// </param>
        public static Operator opFor(char firstCh, char secondCh, Boolean isIndirectionOperatorAllowed)
        { 
	        Operator op;
	
            // See if there is a two-character operator which matches these two characters
            String s = "" + firstCh + secondCh; //$NON-NLS-1$
            op = (Operator) mapTokenToOperator[s];
            if (op == null)
            {
                // No two-character operator, so see if there is a one-character operator
                // which matches the first character passed in
                s = "" + firstCh; //$NON-NLS-1$
                op =(Operator) mapTokenToOperator[s];
                if(op == null) 
            	    op = UNKNOWN;
            } 
            	
            if (isIndirectionOperatorAllowed)
            { 
	            if(op == ARITH_MULT && (secondCh == '#' || Util.isJavaIdentifierStart(secondCh))) 
		            op = INDIRECTION;
            } 
    	
    	    return op;
	    }
	}
}
