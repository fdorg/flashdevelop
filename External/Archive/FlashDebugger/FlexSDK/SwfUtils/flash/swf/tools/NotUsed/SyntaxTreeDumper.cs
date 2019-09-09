// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2005-2007 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using macromedia.asc.parser;
using Value = macromedia.asc.semantics.Value;
using QName = macromedia.asc.semantics.QName;
using Context = macromedia.asc.util.Context;

namespace flash.swf.tools
{
	
	public class SyntaxTreeDumper : Evaluator
	{
		private int indent_Renamed_Field;
		private System.IO.StreamWriter out_Renamed;
		
		public SyntaxTreeDumper(System.IO.StreamWriter out_Renamed):this(out_Renamed, 0)
		{
		}
		
		public SyntaxTreeDumper(System.IO.StreamWriter out_Renamed, int indent)
		{
			this.out_Renamed = out_Renamed;
			this.indent_Renamed_Field = indent;
		}
		
		public virtual bool checkFeature(Context cx, Node node)
		{
			return true;
		}
		
		public virtual Value evaluate(Context cx, Node node)
		{
			output("<Node position=\"" + node.pos() + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ApplyTypeExprNode node)
		{
			output("<ApplyTypeExprNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			
			output("<expr>");
			if (node.expr != null)
			{
				indent_Renamed_Field++;
				node.expr.evaluate(cx, this);
				indent_Renamed_Field--;
			}
			output("</expr>");
			
			output("<typeArgs>");
			if (node.typeArgs != null)
			{
				indent_Renamed_Field++;
				node.typeArgs.evaluate(cx, this);
				indent_Renamed_Field--;
			}
			output("</typeArgs>");
			
			indent_Renamed_Field--;
			output("</ApplyTypeExprNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, IdentifierNode node)
		{
			output("<IdentifierNode name=\"" + node.name + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, IncrementNode node)
		{
			output("<IncrementNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</IncrementNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ThisExpressionNode node)
		{
			output("<ThisExpressionNode position=\"" + node.pos() + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, QualifiedIdentifierNode node)
		{
			if (node.qualifier != null)
			{
				output("<QualifiedIdentifierNode name=\"" + node.name + "\">");
				indent_Renamed_Field++;
				node.qualifier.evaluate(cx, this);
				indent_Renamed_Field--;
				output("</QualifiedIdentifierNode>");
			}
			else
			{
				output("<QualifiedIdentifierNode name=\"" + node.name + "\"/>");
			}
			return null;
		}
		
		public virtual Value evaluate(Context cx, QualifiedExpressionNode node)
		{
			if (node.ref_Renamed == null)
			{
				evaluate(cx, (QualifiedIdentifierNode) node);
				node.expr.evaluate(cx, this);
			}
			return node.ref_Renamed;
		}
		
		public virtual Value evaluate(Context cx, LiteralBooleanNode node)
		{
			output("<LiteralBooleanNode value=\"" + node.value_Renamed + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, LiteralNumberNode node)
		{
			output("<LiteralNumberNode value=\"" + node.value_Renamed + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, LiteralStringNode node)
		{
			if (node.value_Renamed.length() > 0)
			{
				output("<LiteralStringNode value=\"" + node.value_Renamed + "\"/>");
			}
			return null;
		}
		
		public virtual Value evaluate(Context cx, LiteralNullNode node)
		{
			output("<LiteralNullNode position=\"" + node.pos() + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, LiteralRegExpNode node)
		{
			output("<LiteralRegExpNode value=\"" + node.value_Renamed + "\" position=\"" + node.pos() + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, LiteralXMLNode node)
		{
			output("<LiteralXMLNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.list != null)
			{
				node.list.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</LiteralXMLNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, FunctionCommonNode node)
		{
			output("<FunctionCommonNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.signature != null)
			{
				node.signature.evaluate(cx, this);
			}
			if (node.body != null)
			{
				node.body.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</FunctionCommonNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ParenExpressionNode node)
		{
			output("<ParenExpressionNode position=\"" + node.pos() + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ParenListExpressionNode node)
		{
			output("<ParenListExpressionNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</ParenListExpressionNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, LiteralObjectNode node)
		{
			output("<LiteralObjectNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.fieldlist != null)
			{
				node.fieldlist.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</LiteralObjectNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, LiteralFieldNode node)
		{
			output("<LiteralFieldNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.name != null)
			{
				node.name.evaluate(cx, this);
			}
			if (node.value_Renamed != null)
			{
				node.value_Renamed.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</LiteralFieldNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, LiteralArrayNode node)
		{
			output("<LiteralArrayNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.elementlist != null)
			{
				node.elementlist.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</LiteralArrayNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, SuperExpressionNode node)
		{
			output("<SuperExpressionNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</SuperExpressionNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, MemberExpressionNode node)
		{
			output("<MemberExpressionNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.base_Renamed != null)
			{
				node.base_Renamed.evaluate(cx, this);
			}
			if (node.selector != null)
			{
				node.selector.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</MemberExpressionNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, InvokeNode node)
		{
			output("<InvokeNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.args != null)
			{
				node.args.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</InvokeNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, CallExpressionNode node)
		{
			output("<CallExpressionNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			if (node.args != null)
			{
				node.args.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</CallExpressionNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, DeleteExpressionNode node)
		{
			output("<DeleteExpressionNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</DeleteExpressionNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, GetExpressionNode node)
		{
			output("<GetExpressionNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</GetExpressionNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, SetExpressionNode node)
		{
			output("<SetExpressionNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			if (node.args != null)
			{
				node.args.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</SetExpressionNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, UnaryExpressionNode node)
		{
			output("<UnaryExpressionNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</UnaryExpressionNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, BinaryExpressionNode node)
		{
			output("<BinaryExpressionNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.lhs != null)
			{
				node.lhs.evaluate(cx, this);
			}
			if (node.rhs != null)
			{
				node.rhs.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</BinaryExpressionNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ConditionalExpressionNode node)
		{
			output("<ConditionalExpressionNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.condition != null)
			{
				node.condition.evaluate(cx, this);
			}
			if (node.thenexpr != null)
			{
				node.thenexpr.evaluate(cx, this);
			}
			if (node.elseexpr != null)
			{
				node.elseexpr.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</ConditionalExpressionNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ArgumentListNode node)
		{
			output("<ArgumentListNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			// for (Node n : node.items)
			for (int i = 0, size = node.items.size(); i < size; i++)
			{
				Node n = (Node) node.items.get_Renamed(i);
				n.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</ArgumentListNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ListNode node)
		{
			output("<ListNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			// for (Node n : node.items)
			for (int i = 0, size = node.items.size(); i < size; i++)
			{
				Node n = (Node) node.items.get_Renamed(i);
				n.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</ListNode>");
			return null;
		}
		
		// Statements
		
		public virtual Value evaluate(Context cx, StatementListNode node)
		{
			output("<StatementListNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			for (int i = 0, size = node.items.size(); i < size; i++)
			{
				Node n = (Node) node.items.get_Renamed(i);
				if (n != null)
				{
					n.evaluate(cx, this);
				}
			}
			indent_Renamed_Field--;
			output("</StatementListNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, EmptyElementNode node)
		{
			//output("<EmptyElementNode position=\"" + node.pos() + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, EmptyStatementNode node)
		{
			//output("<EmptyStatementNode position=\"" + node.pos() + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ExpressionStatementNode node)
		{
			output("<ExpressionStatementNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</ExpressionStatementNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, SuperStatementNode node)
		{
			output("<SuperStatementNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.call.args != null)
			{
				node.call.args.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</SuperStatementNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, LabeledStatementNode node)
		{
			output("<LabeledStatementNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.label != null)
			{
				node.label.evaluate(cx, this);
			}
			if (node.statement != null)
			{
				node.statement.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</LabeledStatementNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, IfStatementNode node)
		{
			output("<IfStatementNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.condition != null)
			{
				node.condition.evaluate(cx, this);
			}
			if (node.thenactions != null)
			{
				node.thenactions.evaluate(cx, this);
			}
			if (node.elseactions != null)
			{
				node.elseactions.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</IfStatementNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, SwitchStatementNode node)
		{
			output("<SwitchStatementNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			if (node.statements != null)
			{
				node.statements.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</SwitchStatementNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, CaseLabelNode node)
		{
			output("<CaseLabelNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.label != null)
			{
				node.label.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</CaseLabelNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, DoStatementNode node)
		{
			output("<DoStatementNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			if (node.statements != null)
			{
				node.statements.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</DoStatementNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, WhileStatementNode node)
		{
			output("<WhileStatementNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			if (node.statement != null)
			{
				node.statement.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</WhileStatementNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ForStatementNode node)
		{
			output("<ForStatementNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.initialize != null)
			{
				node.initialize.evaluate(cx, this);
			}
			if (node.test != null)
			{
				node.test.evaluate(cx, this);
			}
			if (node.increment != null)
			{
				node.increment.evaluate(cx, this);
			}
			if (node.statement != null)
			{
				node.statement.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</ForStatementNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, WithStatementNode node)
		{
			output("<WithStatementNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			if (node.statement != null)
			{
				node.statement.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</WithStatementNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ContinueStatementNode node)
		{
			output("<ContinueStatementNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.id != null)
			{
				node.id.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</ContinueStatementNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, BreakStatementNode node)
		{
			output("<BreakStatementNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.id != null)
			{
				node.id.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</BreakStatementNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ReturnStatementNode node)
		{
			output("<ReturnStatementNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</ReturnStatementNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ThrowStatementNode node)
		{
			output("<ThrowStatementNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</ThrowStatementNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, TryStatementNode node)
		{
			output("<TryStatementNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.tryblock != null)
			{
				node.tryblock.evaluate(cx, this);
			}
			if (node.catchlist != null)
			{
				node.catchlist.evaluate(cx, this);
			}
			if (node.finallyblock != null)
			{
				node.finallyblock.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</TryStatementNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, CatchClauseNode node)
		{
			output("<CatchClauseNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.parameter != null)
			{
				node.parameter.evaluate(cx, this);
			}
			if (node.statements != null)
			{
				node.statements.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</CatchClauseNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, FinallyClauseNode node)
		{
			output("<FinallyClauseNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.statements != null)
			{
				node.statements.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</FinallyClauseNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, UseDirectiveNode node)
		{
			output("<UseDirectiveNode position=\"" + node.pos() + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, IncludeDirectiveNode node)
		{
			output("<IncludeDirectiveNode position=\"" + node.pos() + "\"/>");
			return null;
		}
		
		// Definitions
		
		public virtual Value evaluate(Context cx, ImportDirectiveNode node)
		{
			output("<ImportDirectiveNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.attrs != null)
			{
				node.attrs.evaluate(cx, this);
			}
			if (node.name != null)
			{
				node.name.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</ImportDirectiveNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, AttributeListNode node)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder("<AttributeListNode");
			if (node.hasIntrinsic)
			{
				buffer.Append(" intrinsic='true'");
			}
			if (node.hasStatic)
			{
				buffer.Append(" static='true'");
			}
			if (node.hasFinal)
			{
				buffer.Append(" final='true'");
			}
			if (node.hasVirtual)
			{
				buffer.Append(" virtual='true'");
			}
			if (node.hasOverride)
			{
				buffer.Append(" override='true'");
			}
			if (node.hasDynamic)
			{
				buffer.Append(" dynamic='true'");
			}
			if (node.hasNative)
			{
				buffer.Append(" native='true'");
			}
			if (node.hasPrivate)
			{
				buffer.Append(" private='true'");
			}
			if (node.hasProtected)
			{
				buffer.Append(" protected='true'");
			}
			if (node.hasPublic)
			{
				buffer.Append(" public='true'");
			}
			if (node.hasInternal)
			{
				buffer.Append(" internal='true'");
			}
			if (node.hasConst)
			{
				buffer.Append(" const='true'");
			}
			if (node.hasFalse)
			{
				buffer.Append(" false='true'");
			}
			if (node.hasPrototype)
			{
				buffer.Append(" prototype='true'");
			}
			buffer.Append(" position=\"" + node.pos() + "\">");
			output(buffer.ToString());
			indent_Renamed_Field++;
			// for (Node n : node.items)
			for (int i = 0, size = node.items.size(); i < size; i++)
			{
				Node n = (Node) node.items.get_Renamed(i);
				n.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</AttributeListNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, VariableDefinitionNode node)
		{
			output("<VariableDefinitionNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.attrs != null)
			{
				node.attrs.evaluate(cx, this);
			}
			if (node.list != null)
			{
				node.list.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</VariableDefinitionNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, VariableBindingNode node)
		{
			output("<VariableBindingNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.variable != null)
			{
				node.variable.evaluate(cx, this);
			}
			if (node.initializer != null)
			{
				node.initializer.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</VariableBindingNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, UntypedVariableBindingNode node)
		{
			output("<UntypedVariableBindingNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.identifier != null)
			{
				node.identifier.evaluate(cx, this);
			}
			if (node.initializer != null)
			{
				node.initializer.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</UntypedVariableBindingNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, TypedIdentifierNode node)
		{
			output("<TypedIdentifierNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.identifier != null)
			{
				node.identifier.evaluate(cx, this);
			}
			if (node.type != null)
			{
				node.type.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</TypedIdentifierNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, BinaryFunctionDefinitionNode node)
		{
			return evaluate(node, cx, "BinaryFunctionDefinitionNode");
		}
		
		public virtual Value evaluate(Context cx, FunctionDefinitionNode node)
		{
			return evaluate(node, cx, "FunctionDefinitionNode");
		}
		
		private Value evaluate(FunctionDefinitionNode node, Context cx, System.String name)
		{
			if ((node.name != null) && (node.name.identifier.name != null))
			{
				output("<" + name + " name=\"" + node.name.identifier.name + "\">");
			}
			else
			{
				output("<" + name + ">");
			}
			indent_Renamed_Field++;
			if (node.attrs != null)
			{
				node.attrs.evaluate(cx, this);
			}
			if (node.fexpr != null)
			{
				node.fexpr.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</" + name + ">");
			return null;
		}
		
		public virtual Value evaluate(Context cx, FunctionNameNode node)
		{
			output("<FunctionNameNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.identifier != null)
			{
				node.identifier.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</FunctionNameNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, FunctionSignatureNode node)
		{
			output("<FunctionSignatureNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.parameter != null)
			{
				node.parameter.evaluate(cx, this);
			}
			if (node.result != null)
			{
				node.result.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</FunctionSignatureNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ParameterNode node)
		{
			if ((0 <= node.kind) && (node.kind < Tokens.tokenClassNames.length))
			{
				output("<ParameterNode kind=\"" + Tokens.tokenClassNames[node.kind] + "\">");
			}
			else
			{
				output("<ParameterNode kind=\"" + node.kind + "\">");
			}
			indent_Renamed_Field++;
			if (node.identifier != null)
			{
				node.identifier.evaluate(cx, this);
			}
			if (node.type != null)
			{
				node.type.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</ParameterNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, RestExpressionNode node)
		{
			output("<RestExpressionNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</RestExpressionNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, RestParameterNode node)
		{
			output("<RestParameterNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.parameter != null)
			{
				node.parameter.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</RestParameterNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, BinaryClassDefNode node)
		{
			return evaluate(node, cx, "BinaryClassDefNode");
		}
		
		public virtual Value evaluate(Context cx, BinaryInterfaceDefinitionNode node)
		{
			return evaluate(node, cx, "BinaryInterfaceDefinitionNode");
		}
		
		public virtual Value evaluate(Context cx, ClassDefinitionNode node)
		{
			return evaluate(node, cx, "ClassDefinitionNode");
		}
		
		private Value evaluate(ClassDefinitionNode node, Context cx, System.String name)
		{
			if ((node.name != null) && (node.name.name != null))
			{
				output("<" + name + " name=\"" + node.name.name + "\">");
			}
			else if ((node.cframe != null) && (node.cframe.builder != null))
			{
				output("<" + name + " name=\"" + node.cframe.builder.classname + "\">");
			}
			indent_Renamed_Field++;
			if (node.attrs != null)
			{
				node.attrs.evaluate(cx, this);
			}
			if (node.name != null)
			{
				node.name.evaluate(cx, this);
			}
			if (node.baseclass != null)
			{
				node.baseclass.evaluate(cx, this);
			}
			if (node.interfaces != null)
			{
				node.interfaces.evaluate(cx, this);
			}
			
			if (node.fexprs != null)
			{
				for (int i = 0, size = node.fexprs.size(); i < size; i++)
				{
					Node fexpr = (Node) node.fexprs.get_Renamed(i);
					fexpr.evaluate(cx, this);
				}
			}
			
			if (node.staticfexprs != null)
			{
				for (int i = 0, size = node.staticfexprs.size(); i < size; i++)
				{
					Node staticfexpr = (Node) node.staticfexprs.get_Renamed(i);
					staticfexpr.evaluate(cx, this);
				}
			}
			if (node.instanceinits != null)
			{
				for (int i = 0, size = node.instanceinits.size(); i < size; i++)
				{
					Node instanceinit = (Node) node.instanceinits.get_Renamed(i);
					instanceinit.evaluate(cx, this);
				}
			}
			
			if (node.statements != null)
			{
				node.statements.evaluate(cx, this);
			}
			
			indent_Renamed_Field--;
			output("</" + name + ">");
			return null;
		}
		
		public virtual Value evaluate(Context cx, InterfaceDefinitionNode node)
		{
			if ((node.name != null) && (node.name.name != null))
			{
				output("<InterfaceDefinitionNode name=\"" + node.name.name + "\">");
			}
			else if ((node.cframe != null) && (node.cframe.builder != null))
			{
				output("<InterfaceDefinitionNode name=\"" + node.cframe.builder.classname + "\">");
			}
			indent_Renamed_Field++;
			if (node.attrs != null)
			{
				node.attrs.evaluate(cx, this);
			}
			if (node.name != null)
			{
				node.name.evaluate(cx, this);
			}
			if (node.interfaces != null)
			{
				node.interfaces.evaluate(cx, this);
			}
			if (node.statements != null)
			{
				node.statements.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</InterfaceDefinitionNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ClassNameNode node)
		{
			output("<ClassNameNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.pkgname != null)
			{
				node.pkgname.evaluate(cx, this);
			}
			if (node.ident != null)
			{
				node.ident.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</ClassNameNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, InheritanceNode node)
		{
			output("<InheritanceNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.baseclass != null)
			{
				node.baseclass.evaluate(cx, this);
			}
			if (node.interfaces != null)
			{
				node.interfaces.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</InheritanceNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, NamespaceDefinitionNode node)
		{
			output("<NamespaceDefinitionNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.attrs != null)
			{
				node.attrs.evaluate(cx, this);
			}
			if (node.name != null)
			{
				node.name.evaluate(cx, this);
			}
			if (node.value_Renamed != null)
			{
				node.value_Renamed.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</NamespaceDefinitionNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ConfigNamespaceDefinitionNode node)
		{
			output("<ConfigNamespaceDefinitionNode />");
			return null;
		}
		
		public virtual Value evaluate(Context cx, PackageDefinitionNode node)
		{
			output("<PackageDefinitionNode />");
			return null;
		}
		
		public virtual Value evaluate(Context cx, PackageIdentifiersNode node)
		{
			output("<PackageIdentifiersNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			for (int i = 0, size = node.list.size(); i < size; i++)
			{
				IdentifierNode n = (IdentifierNode) node.list.get_Renamed(i);
				n.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</PackageIdentifiersNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, PackageNameNode node)
		{
			output("<PackageNameNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.id != null)
			{
				node.id.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</PackageNameNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ProgramNode node)
		{
			output("<ProgramNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.pkgdefs != null)
			{
				// for (PackageDefinitionNode n : node.pkgdefs)
				for (int i = 0, size = node.pkgdefs.size(); i < size; i++)
				{
					PackageDefinitionNode n = (PackageDefinitionNode) node.pkgdefs.get_Renamed(i);
					n.evaluate(cx, this);
				}
			}
			
			if (node.statements != null)
			{
				node.statements.evaluate(cx, this);
			}
			
			if (node.fexprs != null)
			{
				// for (FunctionCommonNode n : node.fexprs)
				for (int i = 0, size = node.fexprs.size(); i < size; i++)
				{
					FunctionCommonNode n = (FunctionCommonNode) node.fexprs.get_Renamed(i);
					n.evaluate(cx, this);
				}
			}
			
			if (node.clsdefs != null)
			{
				// for (FunctionCommonNode n : node.clsdefs)
				for (int i = 0, size = node.clsdefs.size(); i < size; i++)
				{
					ClassDefinitionNode n = (ClassDefinitionNode) node.clsdefs.get_Renamed(i);
					n.evaluate(cx, this);
				}
			}
			
			indent_Renamed_Field--;
			output("</ProgramNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ErrorNode node)
		{
			output("<ErrorNode position=\"" + node.pos() + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ToObjectNode node)
		{
			output("<ToObjectNode position=\"" + node.pos() + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, LoadRegisterNode node)
		{
			output("<LoadRegisterNode position=\"" + node.pos() + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, StoreRegisterNode node)
		{
			output("<StoreRegisterNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</StoreRegisterNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, BoxNode node)
		{
			output("<BoxNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</BoxNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, CoerceNode node)
		{
			output("<CoerceNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.expr != null)
			{
				node.expr.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</CoerceNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, PragmaNode node)
		{
			output("<PragmaNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.list != null)
			{
				node.list.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</PragmaNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, PragmaExpressionNode node)
		{
			output("<PragmaExpressionNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.identifier != null)
			{
				node.identifier.evaluate(cx, this);
			}
			indent_Renamed_Field--;
			output("</PragmaExpressionNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ParameterListNode node)
		{
			output("<ParameterListNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			for (int i = 0, size = node.items.size(); i < size; i++)
			{
				// ParameterNode param = node.items.get(i);
				ParameterNode param = (ParameterNode) node.items.get_Renamed(i);
				if (param != null)
				{
					param.evaluate(cx, this);
				}
			}
			indent_Renamed_Field--;
			output("</ParameterListNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, MetaDataNode node)
		{
			output("<MetaDataNode id=\"" + node.id + "\">");
			indent_Renamed_Field++;
			if (node.data != null)
			{
				MetaDataEvaluator mde = new MetaDataEvaluator();
				node.evaluate(cx, mde);
			}
			indent_Renamed_Field--;
			output("</MetaDataNode>");
			return null;
		}
		
		public virtual Value evaluate(Context context, DefaultXMLNamespaceNode node)
		{
			output("<DefaultXMLNamespaceNode position=\"" + node.pos() + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, DocCommentNode node)
		{
			output("<DocCommentNode position=\"" + node.pos() + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, ImportNode node)
		{
			System.String id = node.filespec.value_Renamed;
			QName qname = new QName(cx.publicNamespace(), id);
			output("<ImportNode value=" + qname + "/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, BinaryProgramNode node)
		{
			output("<BinaryProgramNode position=\"" + node.pos() + "\">");
			indent_Renamed_Field++;
			if (node.pkgdefs != null)
			{
				// for (PackageDefinitionNode n : node.pkgdefs)
				for (int i = 0, size = node.pkgdefs.size(); i < size; i++)
				{
					PackageDefinitionNode n = (PackageDefinitionNode) node.pkgdefs.get_Renamed(i);
					n.evaluate(cx, this);
				}
			}
			
			if (node.statements != null)
			{
				node.statements.evaluate(cx, this);
			}
			
			if (node.fexprs != null)
			{
				// for (FunctionCommonNode n : node.fexprs)
				for (int i = 0, size = node.fexprs.size(); i < size; i++)
				{
					FunctionCommonNode n = (FunctionCommonNode) node.fexprs.get_Renamed(i);
					n.evaluate(cx, this);
				}
			}
			
			if (node.clsdefs != null)
			{
				// for (FunctionCommonNode n : node.clsdefs)
				for (int i = 0, size = node.clsdefs.size(); i < size; i++)
				{
					ClassDefinitionNode n = (ClassDefinitionNode) node.clsdefs.get_Renamed(i);
					n.evaluate(cx, this);
				}
			}
			
			indent_Renamed_Field--;
			output("</BinaryProgramNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, RegisterNode node)
		{
			output("<RegisterNode position=\"" + node.pos() + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, HasNextNode node)
		{
			return null;
		}
		
		public virtual Value evaluate(Context cx, TypeExpressionNode node)
		{
			output("<TypeExpressionNode position=\"" + node.pos() + "\">");
			node.expr.evaluate(cx, this);
			output("</TypeExpressionNode>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, UseNumericNode node)
		{
			output("<UseNumericNode position=\"" + node.pos() + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, UsePrecisionNode node)
		{
			output("<UsePrecisionNode position=\"" + node.pos() + "\"/>");
			return null;
		}
		
		public virtual Value evaluate(Context cx, UseRoundingNode node)
		{
			output("<UseRoundingNode position=\"" + node.pos() + "\"/>");
			return null;
		}
		
		private System.String indent()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();
			
			for (int i = 0; i < indent_Renamed_Field; i++)
			{
				buffer.Append("  ");
			}
			
			return buffer.ToString();
		}
		
		private void  output(System.String tag)
		{
			try
			{
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine(indent() + tag);
			}
			catch (System.Exception exception)
			{
				SupportClass.WriteStackTrace(exception, Console.Error);
			}
		}
	}
}