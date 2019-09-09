////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
namespace flash.swf.tools
{
	
	public class AbcPrinter
	{
		private sbyte[] abc;
		private System.IO.StreamWriter out_Renamed;
		private bool showOffset;
		private int indent;
		private int offset = 0;
		
		private int[] intConstants;
		private long[] uintConstants;
		private double[] floatConstants;
		private System.String[] stringConstants;
		private System.String[] namespaceConstants;
		private System.String[][] namespaceSetConstants;
		private MultiName[] multiNameConstants;
		
		private MethodInfo[] methods;
		private System.String[] instanceNames;
		private System.String indentString;
		
		public AbcPrinter(sbyte[] abc, System.IO.StreamWriter out_Renamed, bool showOffset, int indent)
		{
			this.abc = abc;
			this.out_Renamed = out_Renamed;
			this.showOffset = showOffset;
			this.indent = indent;
			char[] spaces = new char[indent * 2 + 1];
			for (int i = 0; i < indent * 2; i++)
				spaces[i] = ' ';
			this.indentString = new System.String(spaces, 0, indent * 2);
		}
		
		public virtual void  print()
		{
			printOffset();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(abc[offset++] + " " + abc[offset++] + " minor version");
			printOffset();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(abc[offset++] + " " + abc[offset++] + " major version");
			printIntConstantPool();
			printUintConstantPool();
			printDoubleConstantPool();
			printStringConstantPool();
			printNamespaceConstantPool();
			printNamespaceSetsConstantPool();
			printMultiNameConstantPool();
			printMethods();
			printMetaData();
			printClasses();
			printScripts();
			printBodies();
		}
		
		//UPGRADE_NOTE: Final was removed from the declaration of 'TRAIT_Slot '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int TRAIT_Slot = 0x00;
		//UPGRADE_NOTE: Final was removed from the declaration of 'TRAIT_Method '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int TRAIT_Method = 0x01;
		//UPGRADE_NOTE: Final was removed from the declaration of 'TRAIT_Getter '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int TRAIT_Getter = 0x02;
		//UPGRADE_NOTE: Final was removed from the declaration of 'TRAIT_Setter '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int TRAIT_Setter = 0x03;
		//UPGRADE_NOTE: Final was removed from the declaration of 'TRAIT_Class '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int TRAIT_Class = 0x04;
		//UPGRADE_NOTE: Final was removed from the declaration of 'TRAIT_Function '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int TRAIT_Function = 0x05;
		//UPGRADE_NOTE: Final was removed from the declaration of 'TRAIT_Const '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int TRAIT_Const = 0x06;
		
		//UPGRADE_NOTE: Final was removed from the declaration of 'traitKinds '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal System.String[] traitKinds = new System.String[]{"var", "function", "function get", "function set", "class", "function", "const"};
		
		internal char[] hexChars = new char[]{'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_bkpt '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_bkpt = 0x01;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_nop '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_nop = 0x02;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_throw '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_throw = 0x03;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_getsuper '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_getsuper = 0x04;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_setsuper '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_setsuper = 0x05;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_dxns '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_dxns = 0x06;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_dxnslate '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_dxnslate = 0x07;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_kill '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_kill = 0x08;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_label '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_label = 0x09;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_ifnlt '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_ifnlt = 0x0C;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_ifnle '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_ifnle = 0x0D;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_ifngt '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_ifngt = 0x0E;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_ifnge '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_ifnge = 0x0F;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_jump '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_jump = 0x10;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_iftrue '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_iftrue = 0x11;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_iffalse '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_iffalse = 0x12;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_ifeq '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_ifeq = 0x13;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_ifne '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_ifne = 0x14;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_iflt '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_iflt = 0x15;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_ifle '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_ifle = 0x16;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_ifgt '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_ifgt = 0x17;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_ifge '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_ifge = 0x18;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_ifstricteq '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_ifstricteq = 0x19;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_ifstrictne '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_ifstrictne = 0x1A;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_lookupswitch '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_lookupswitch = 0x1B;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_pushwith '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_pushwith = 0x1C;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_popscope '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_popscope = 0x1D;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_nextname '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_nextname = 0x1E;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_hasnext '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_hasnext = 0x1F;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_pushnull '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_pushnull = 0x20;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_pushundefined '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_pushundefined = 0x21;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_pushintant '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_pushintant = 0x22;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_nextvalue '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_nextvalue = 0x23;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_pushbyte '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_pushbyte = 0x24;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_pushshort '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_pushshort = 0x25;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_pushtrue '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_pushtrue = 0x26;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_pushfalse '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_pushfalse = 0x27;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_pushnan '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_pushnan = 0x28;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_pop '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_pop = 0x29;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_dup '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_dup = 0x2A;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_swap '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_swap = 0x2B;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_pushstring '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_pushstring = 0x2C;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_pushint '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_pushint = 0x2D;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_pushuint '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_pushuint = 0x2E;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_pushdouble '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_pushdouble = 0x2F;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_pushscope '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_pushscope = 0x30;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_pushnamespace '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_pushnamespace = 0x31;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_hasnext2 '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_hasnext2 = 0x32;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_newfunction '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_newfunction = 0x40;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_call '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_call = 0x41;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_construct '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_construct = 0x42;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_callmethod '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_callmethod = 0x43;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_callstatic '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_callstatic = 0x44;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_callsuper '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_callsuper = 0x45;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_callproperty '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_callproperty = 0x46;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_returnvoid '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_returnvoid = 0x47;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_returnvalue '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_returnvalue = 0x48;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_constructsuper '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_constructsuper = 0x49;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_constructprop '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_constructprop = 0x4A;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_callsuperid '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_callsuperid = 0x4B;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_callproplex '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_callproplex = 0x4C;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_callinterface '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_callinterface = 0x4D;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_callsupervoid '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_callsupervoid = 0x4E;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_callpropvoid '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_callpropvoid = 0x4F;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_newobject '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_newobject = 0x55;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_newarray '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_newarray = 0x56;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_newactivation '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_newactivation = 0x57;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_newclass '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_newclass = 0x58;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_getdescendants '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_getdescendants = 0x59;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_newcatch '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_newcatch = 0x5A;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_findpropstrict '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_findpropstrict = 0x5D;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_findproperty '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_findproperty = 0x5E;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_finddef '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_finddef = 0x5F;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_getlex '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_getlex = 0x60;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_setproperty '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_setproperty = 0x61;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_getlocal '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_getlocal = 0x62;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_setlocal '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_setlocal = 0x63;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_getglobalscope '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_getglobalscope = 0x64;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_getscopeobject '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_getscopeobject = 0x65;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_getproperty '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_getproperty = 0x66;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_getpropertylate '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_getpropertylate = 0x67;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_initproperty '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_initproperty = 0x68;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_setpropertylate '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_setpropertylate = 0x69;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_deleteproperty '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_deleteproperty = 0x6A;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_deletepropertylate '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_deletepropertylate = 0x6B;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_getslot '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_getslot = 0x6C;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_setslot '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_setslot = 0x6D;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_getglobalslot '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_getglobalslot = 0x6E;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_setglobalslot '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_setglobalslot = 0x6F;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_convert_s '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_convert_s = 0x70;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_esc_xelem '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_esc_xelem = 0x71;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_esc_xattr '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_esc_xattr = 0x72;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_convert_i '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_convert_i = 0x73;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_convert_u '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_convert_u = 0x74;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_convert_d '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_convert_d = 0x75;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_convert_b '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_convert_b = 0x76;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_convert_o '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_convert_o = 0x77;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_coerce '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_coerce = 0x80;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_coerce_b '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_coerce_b = 0x81;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_coerce_a '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_coerce_a = 0x82;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_coerce_i '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_coerce_i = 0x83;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_coerce_d '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_coerce_d = 0x84;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_coerce_s '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_coerce_s = 0x85;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_astype '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_astype = 0x86;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_astypelate '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_astypelate = 0x87;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_coerce_u '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_coerce_u = 0x88;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_coerce_o '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_coerce_o = 0x89;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_negate '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_negate = 0x90;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_increment '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_increment = 0x91;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_inclocal '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_inclocal = 0x92;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_decrement '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_decrement = 0x93;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_declocal '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_declocal = 0x94;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_typeof '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_typeof = 0x95;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_not '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_not = 0x96;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_bitnot '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_bitnot = 0x97;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_concat '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_concat = 0x9A;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_add_d '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_add_d = 0x9B;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_add '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_add = 0xA0;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_subtract '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_subtract = 0xA1;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_multiply '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_multiply = 0xA2;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_divide '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_divide = 0xA3;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_modulo '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_modulo = 0xA4;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_lshift '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_lshift = 0xA5;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_rshift '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_rshift = 0xA6;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_urshift '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_urshift = 0xA7;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_bitand '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_bitand = 0xA8;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_bitor '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_bitor = 0xA9;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_bitxor '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_bitxor = 0xAA;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_equals '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_equals = 0xAB;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_strictequals '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_strictequals = 0xAC;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_lessthan '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_lessthan = 0xAD;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_lessequals '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_lessequals = 0xAE;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_greaterthan '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_greaterthan = 0xAF;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_greaterequals '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_greaterequals = 0xB0;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_instanceof '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_instanceof = 0xB1;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_istype '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_istype = 0xB2;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_istypelate '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_istypelate = 0xB3;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_in '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_in = 0xB4;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_increment_i '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_increment_i = 0xC0;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_decrement_i '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_decrement_i = 0xC1;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_inclocal_i '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_inclocal_i = 0xC2;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_declocal_i '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_declocal_i = 0xC3;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_negate_i '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_negate_i = 0xC4;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_add_i '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_add_i = 0xC5;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_subtract_i '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_subtract_i = 0xC6;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_multiply_i '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_multiply_i = 0xC7;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_getlocal0 '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_getlocal0 = 0xD0;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_getlocal1 '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_getlocal1 = 0xD1;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_getlocal2 '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_getlocal2 = 0xD2;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_getlocal3 '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_getlocal3 = 0xD3;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_setlocal0 '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_setlocal0 = 0xD4;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_setlocal1 '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_setlocal1 = 0xD5;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_setlocal2 '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_setlocal2 = 0xD6;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_setlocal3 '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_setlocal3 = 0xD7;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_debug '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_debug = 0xEF;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_debugline '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_debugline = 0xF0;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_debugfile '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_debugfile = 0xF1;
		//UPGRADE_NOTE: Final was removed from the declaration of 'OP_bkptline '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		internal int OP_bkptline = 0xF2;
		
		internal System.String[] opNames = new System.String[]{"OP_0x00       ", "bkpt          ", "nop           ", "throw         ", "getsuper      ", "setsuper      ", "dxns          ", "dxnslate      ", "kill          ", "label         ", "OP_0x0A       ", "OP_0x0B       ", "ifnlt         ", "ifnle         ", "ifngt         ", "ifnge         ", "jump          ", "iftrue        ", "iffalse       ", "ifeq          ", "ifne          ", "iflt          ", "ifle          ", "ifgt          ", "ifge          ", "ifstricteq    ", "ifstrictne    ", "lookupswitch  ", "pushwith      ", "popscope      ", "nextname      ", "hasnext       ", "pushnull      ", "pushundefined ", "pushconstant  ", "nextvalue     ", "pushbyte      ", "pushshort     ", "pushtrue      ", "pushfalse     ", "pushnan       ", "pop           ", "dup           ", "swap          ", "pushstring    ", "pushint       ", "pushuint      ", "pushdouble    ", "pushscope     ", "pushnamespace ", "hasnext2      ", "OP_0x33       ", "OP_0x34       ", "OP_0x35       ", "OP_0x36       ", "OP_0x37       ", "OP_0x38       ", "OP_0x39       ", "OP_0x3A       ", "OP_0x3B       ", "OP_0x3C       ", "OP_0x3D       ", "OP_0x3E       ", "OP_0x3F       ", "newfunction   ", "call          ", "construct     ", "callmethod    ", "callstatic    ", "callsuper     ", "callproperty  ", "returnvoid    ", "returnvalue   ", "constructsuper", "constructprop ", "callsuperid   ", "callproplex   ", "callinterface ", "callsupervoid ", "callpropvoid  ", "OP_0x50       ", "OP_0x51       ", "OP_0x52       ", "OP_0x53       ", "OP_0x54       ", "newobject     ", "newarray      ", "newactivation ", "newclass      ", "getdescendants", "newcatch      ", "OP_0x5B       ", "OP_0x5C       ", "findpropstrict", "findproperty  ", "finddef       ", "getlex        ", "setproperty   ", "getlocal      ", "setlocal      ", "getglobalscope", "getscopeobject", "getproperty   ", "OP_0x67       ", "initproperty  ", "OP_0x69       ", "deleteproperty", "OP_0x6A       ", "getslot       ", "setslot       ", 
			"getglobalslot ", "setglobalslot ", "convert_s     ", "esc_xelem     ", "esc_xattr     ", "convert_i     ", "convert_u     ", "convert_d     ", "convert_b     ", "convert_o     ", "checkfilter   ", "OP_0x79       ", "OP_0x7A       ", "OP_0x7B       ", "OP_0x7C       ", "OP_0x7D       ", "OP_0x7E       ", "OP_0x7F       ", "coerce        ", "coerce_b      ", "coerce_a      ", "coerce_i      ", "coerce_d      ", "coerce_s      ", "astype        ", "astypelate    ", "coerce_u      ", "coerce_o      ", "OP_0x8A       ", "OP_0x8B       ", "OP_0x8C       ", "OP_0x8D       ", "OP_0x8E       ", "OP_0x8F       ", "negate        ", "increment     ", "inclocal      ", "decrement     ", "declocal      ", "typeof        ", "not           ", "bitnot        ", "OP_0x98       ", "OP_0x99       ", "concat        ", "add_d         ", "OP_0x9C       ", "OP_0x9D       ", "OP_0x9E       ", "OP_0x9F       ", "add           ", "subtract      ", "multiply      ", "divide        ", "modulo        ", "lshift        ", "rshift        ", "urshift       ", "bitand        ", "bitor         ", "bitxor        ", "equals        ", "strictequals  ", "lessthan      ", "lessequals    ", "greaterthan   ", "greaterequals ", "instanceof    ", "istype        ", "istypelate    ", "in            ", "OP_0xB5       ", "OP_0xB6       ", "OP_0xB7       ", "OP_0xB8       ", "OP_0xB9       ", "OP_0xBA       ", "OP_0xBB       ", "OP_0xBC       ", "OP_0xBD       ", "OP_0xBE       ", "OP_0xBF       ", "increment_i   ", "decrement_i   ", "inclocal_i    ", "declocal_i    ", "negate_i      ", "add_i         ", "subtract_i    ", "multiply_i    ", "OP_0xC8       ", "OP_0xC9       ", "OP_0xCA       ", "OP_0xCB       ", "OP_0xCC       ", "OP_0xCD       ", "OP_0xCE       ", "OP_0xCF       ", "getlocal0     ", "getlocal1     ", "getlocal2     ", "getlocal3     ", "setlocal0     ", "setlocal1     ", "setlocal2     ", "setlocal3     ", "OP_0xD8       ", "OP_0xD9       ", "OP_0xDA       ", "OP_0xDB       ", "OP_0xDC       ", "OP_0xDD       ", 
			"OP_0xDE       ", "OP_0xDF       ", "OP_0xE0       ", "OP_0xE1       ", "OP_0xE2       ", "OP_0xE3       ", "OP_0xE4       ", "OP_0xE5       ", "OP_0xE6       ", "OP_0xE7       ", "OP_0xE8       ", "OP_0xE9       ", "OP_0xEA       ", "OP_0xEB       ", "OP_0xEC       ", "OP_0xED       ", "OP_0xEE       ", "debug         ", "debugline     ", "debugfile     ", "bkptline      ", "timestamp     ", "OP_0xF4       ", "verifypass    ", "alloc         ", "mark          ", "wb            ", "prologue      ", "sendenter     ", "doubletoatom  ", "sweep         ", "codegenop     ", "verifyop      ", "decode        "};
		
		internal virtual System.String hex(sbyte b)
		{
			return new System.Text.StringBuilder().Append(hexChars[(b >> 4) & 0xF]).Append(hexChars[b & 0xF]).ToString();
		}
		
		internal virtual void  printOffset()
		{
			if (showOffset)
				out_Renamed.Write(indentString + "offset " + offset + ": ");
			else
				out_Renamed.Write(indentString);
		}
		
		internal virtual int readS24()
		{
			int b = abc[offset++];
			b &= 0xFF;
			b |= abc[offset++] << 8;
			b &= 0xFFFF;
			b |= abc[offset++] << 16;
			return b;
		}
		
		internal virtual long readU32()
		{
			long b = abc[offset++];
			b &= 0xFF;
			long u32 = b;
			if (!((u32 & 0x00000080) == 0x00000080))
				return u32;
			b = abc[offset++];
			b &= 0xFF;
			u32 = u32 & 0x0000007f | b << 7;
			if (!((u32 & 0x00004000) == 0x00004000))
				return u32;
			b = abc[offset++];
			b &= 0xFF;
			u32 = u32 & 0x00003fff | b << 14;
			if (!((u32 & 0x00200000) == 0x00200000))
				return u32;
			b = abc[offset++];
			b &= 0xFF;
			u32 = u32 & 0x001fffff | b << 21;
			if (!((u32 & 0x10000000) == 0x10000000))
				return u32;
			b = abc[offset++];
			b &= 0xFF;
			u32 = u32 & 0x0fffffff | b << 28;
			return u32;
		}
		
		internal virtual System.String readUTFBytes(long n)
		{
			System.IO.StringWriter sw = new System.IO.StringWriter();
			for (int i = 0; i < n; i++)
			{
				sw.Write((byte) abc[offset++]);
			}
			return sw.ToString();
		}
		
		internal virtual void  printIntConstantPool()
		{
			long n = readU32();
			printOffset();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(n + " Integer Constant Pool Entries");
			intConstants = new int[(n > 0)?(int) n:1];
			intConstants[0] = 0;
			for (int i = 1; i < n; i++)
			{
				long val = readU32();
				intConstants[i] = (int) val;
				printOffset();
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_long'"
				out_Renamed.WriteLine(val);
			}
		}
		
		internal virtual void  printUintConstantPool()
		{
			long n = readU32();
			printOffset();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(n + " Unsigned Integer Constant Pool Entries");
			uintConstants = new long[(n > 0)?(int) n:1];
			uintConstants[0] = 0;
			for (int i = 1; i < n; i++)
			{
				long val = readU32();
				uintConstants[i] = (int) val;
				printOffset();
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_long'"
				out_Renamed.WriteLine(val);
			}
		}
		
		internal virtual void  printDoubleConstantPool()
		{
			long n = readU32();
			printOffset();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(n + " Floating Point Constant Pool Entries");
			if (n > 0)
				offset = (int) (offset + (n - 1) * 8);
		}
		
		internal virtual void  printStringConstantPool()
		{
			long n = readU32();
			printOffset();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(n + " String Constant Pool Entries");
			stringConstants = new System.String[(n > 0)?(int) n:1];
			stringConstants[0] = "";
			for (int i = 1; i < n; i++)
			{
				printOffset();
				System.String s = readUTFBytes(readU32());
				stringConstants[i] = s;
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine(" " + s);
			}
		}
		
		internal virtual void  printNamespaceConstantPool()
		{
			long n = readU32();
			printOffset();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(n + " Namespace Constant Pool Entries");
			namespaceConstants = new System.String[(n > 0)?(int) n:1];
			namespaceConstants[0] = "public";
			for (int i = 1; i < n; i++)
			{
				printOffset();
				sbyte b = abc[offset++];
				System.String s;
				if (b == 5)
				{
					readU32();
					s = "private";
				}
				else
				{
					s = stringConstants[(int) readU32()];
				}
				namespaceConstants[i] = s;
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine(" " + s);
			}
		}
		
		internal virtual void  printNamespaceSetsConstantPool()
		{
			long n = readU32();
			printOffset();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(n + " Namespace Set Constant Pool Entries");
			namespaceSetConstants = new System.String[(n > 0)?(int) n:1][];
			namespaceSetConstants[0] = new System.String[0];
			for (int i = 1; i < n; i++)
			{
				long val = readU32();
				System.String[] nsset = new System.String[(int) val];
				namespaceSetConstants[i] = nsset;
				for (int j = 0; j < val; j++)
				{
					nsset[j] = namespaceConstants[(int) readU32()];
				}
			}
		}
		
		internal virtual void  printMultiNameConstantPool()
		{
			long n = readU32();
			printOffset();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(n + " MultiName Constant Pool Entries");
			multiNameConstants = new MultiName[(n > 0)?(int) n:1];
			multiNameConstants[0] = new MultiName(this);
			for (int i = 1; i < n; i++)
			{
				printOffset();
				sbyte b = abc[offset++];
				multiNameConstants[i] = new MultiName(this);
				multiNameConstants[i].kind = b;
				switch (b)
				{
					
					case (sbyte) (0x07): 
					// QName
					case (sbyte) (0x0D): 
						multiNameConstants[i].long1 = (int) readU32();
						multiNameConstants[i].long2 = (int) readU32();
						break;
					
					case (sbyte) (0x0F): 
					// RTQName
					case (sbyte) (0x10): 
						multiNameConstants[i].long1 = (int) readU32();
						break;
					
					case (sbyte) (0x11): 
					// RTQNameL
					case (sbyte) (0x12): 
						break;
					
					case (sbyte) (0x13): 
					// NameL
					case (sbyte) (0x14): 
						break;
					
					case (sbyte) (0x09): 
					case (sbyte) (0x0E): 
						multiNameConstants[i].long1 = (int) readU32();
						multiNameConstants[i].long2 = (int) readU32();
						break;
					
					case (sbyte) (0x1B): 
					case (sbyte) (0x1C): 
						multiNameConstants[i].long1 = (int) readU32();
						break;
					}
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangObject'"
				out_Renamed.WriteLine(multiNameConstants[i]);
			}
		}
		
		internal virtual void  printMethods()
		{
			long n = readU32();
			printOffset();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(n + " Method Entries");
			methods = new MethodInfo[(int) n];
			for (int i = 0; i < n; i++)
			{
				int start = offset;
				printOffset();
				MethodInfo m = methods[i] = new MethodInfo(this);
				m.paramCount = (int) readU32();
				m.returnType = (int) readU32();
				m.params_Renamed = new int[m.paramCount];
				for (int j = 0; j < m.paramCount; j++)
				{
					m.params_Renamed[j] = (int) readU32();
				}
				int nameIndex = (int) readU32();
				if (nameIndex > 0)
					m.name = stringConstants[nameIndex];
				else
					m.name = "no name";
				
				m.flags = abc[offset++];
				if ((m.flags & 0x8) == 0x8)
				{
					// read in optional parameter info
					m.optionCount = (int) readU32();
					m.optionIndex = new int[m.optionCount];
					m.optionKinds = new int[m.optionCount];
					for (int k = 0; k < m.optionCount; k++)
					{
						m.optionIndex[k] = (int) readU32();
						m.optionKinds[k] = abc[offset++];
					}
				}
				if ((m.flags & 0x80) == 0x80)
				{
					// read in parameter names info
					m.paramNames = new int[m.paramCount];
					for (int k = 0; k < m.paramCount; k++)
					{
						m.paramNames[k] = (int) readU32();
					}
				}
				for (int x = start; x < offset; x++)
				{
					out_Renamed.Write(hex(abc[(int) x]) + " ");
				}
				out_Renamed.Write(m.name + "(");
				for (int x = 0; x < m.paramCount; x++)
				{
					out_Renamed.Write(multiNameConstants[m.params_Renamed[x]]);
					if (x < m.paramCount - 1)
						out_Renamed.Write(",");
				}
				out_Renamed.Write("):");
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine(multiNameConstants[m.returnType] + " ");
			}
		}
		
		internal virtual void  printMetaData()
		{
			long n = readU32();
			printOffset();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(n + " Metadata Entries");
			for (int i = 0; i < n; i++)
			{
				int start = offset;
				printOffset();
				System.String s = stringConstants[(int) readU32()];
				long val = readU32();
				for (int j = 0; j < val; j++)
				{
					s += (" " + stringConstants[(int) readU32()]);
				}
				for (int j = 0; j < val; j++)
				{
					s += (" " + stringConstants[(int) readU32()]);
				}
				for (int x = start; x < offset; x++)
				{
					out_Renamed.Write(hex(abc[(int) x]) + " ");
				}
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine(s);
			}
		}
		
		internal virtual void  printClasses()
		{
			long n = readU32();
			printOffset();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(n + " Instance Entries");
			instanceNames = new System.String[(int) n];
			for (int i = 0; i < n; i++)
			{
				int start = offset;
				printOffset();
				System.String name = multiNameConstants[(int) readU32()].ToString();
				instanceNames[i] = name;
				System.String base_Renamed = multiNameConstants[(int) readU32()].ToString();
				int b = abc[offset++];
				if ((b & 0x8) == 0x8)
					readU32(); // eat protected namespace
				long val = readU32();
				System.String s = "";
				for (int j = 0; j < val; j++)
				{
					s += (" " + multiNameConstants[(int) readU32()].ToString());
				}
				int init = (int) readU32(); // eat init method
				MethodInfo mi = methods[init];
				mi.name = name;
				mi.className = name;
				mi.kind = TRAIT_Method;
				for (int x = start; x < offset; x++)
				{
					out_Renamed.Write(hex(abc[(int) x]) + " ");
				}
				out_Renamed.Write(name + " ");
				if (base_Renamed.Length > 0)
					out_Renamed.Write("extends " + base_Renamed + " ");
				if (s.Length > 0)
					out_Renamed.Write("implements " + s);
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("");
				
				int numTraits = (int) readU32(); // number of traits
				printOffset();
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine(numTraits + " Traits Entries");
				for (int j = 0; j < numTraits; j++)
				{
					printOffset();
					start = offset;
					s = multiNameConstants[(int) readU32()].ToString(); // eat trait name;
					b = abc[offset++];
					int kind = b & 0xf;
					switch (kind)
					{
						
						case 0x00: 
						// slot
						case 0x06:  // const
							readU32(); // id
							readU32(); // type
							int index = (int) readU32(); // index;
							if (index != 0)
								offset++; // kind
							break;
						
						case 0x04:  // class
							readU32(); // id
							readU32(); // value;
							break;
						
						default: 
							readU32(); // id
							mi = methods[(int) readU32()]; // method
							mi.name = s;
							mi.className = name;
							mi.kind = kind;
							break;
						
					}
					if ((b >> 4 & 0x4) == 0x4)
					{
						val = readU32(); // metadata count
						for (int k = 0; k < val; k++)
						{
							readU32(); // metadata
						}
					}
					for (int x = start; x < offset; x++)
					{
						out_Renamed.Write(hex(abc[(int) x]) + " ");
					}
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine(s);
				}
			}
			printOffset();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(n + " Class Entries");
			for (int i = 0; i < n; i++)
			{
				int start = offset;
				printOffset();
				MethodInfo mi = methods[(int) readU32()];
				System.String name = instanceNames[i];
				mi.name = name + "$cinit";
				mi.className = name;
				mi.kind = TRAIT_Method;
				System.String base_Renamed = "Class";
				for (int x = start; x < offset; x++)
				{
					out_Renamed.Write(hex(abc[(int) x]) + " ");
				}
				out_Renamed.Write(name + " ");
				if (base_Renamed.Length > 0)
					out_Renamed.Write("extends " + base_Renamed + " ");
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("");
				
				int numTraits = (int) readU32(); // number of traits
				printOffset();
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine(numTraits + " Traits Entries");
				for (int j = 0; j < numTraits; j++)
				{
					printOffset();
					start = offset;
					System.String s = multiNameConstants[(int) readU32()].ToString(); // eat trait name;
					int b = abc[offset++];
					int kind = b & 0xf;
					switch (kind)
					{
						
						case 0x00: 
						// slot
						case 0x06:  // const
							readU32(); // id
							readU32(); // type
							int index = (int) readU32(); // index;
							if (index != 0)
								offset++; // kind
							break;
						
						case 0x04:  // class
							readU32(); // id
							readU32(); // value;
							break;
						
						default: 
							readU32(); // id
							mi = methods[(int) readU32()]; // method
							mi.name = s;
							mi.className = name;
							mi.kind = kind;
							break;
						
					}
					if ((b >> 4 & 0x4) == 0x4)
					{
						int val = (int) readU32(); // metadata count
						for (int k = 0; k < val; k++)
						{
							readU32(); // metadata
						}
					}
					for (int x = start; x < offset; x++)
					{
						out_Renamed.Write(hex(abc[(int) x]) + " ");
					}
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine(s);
				}
			}
		}
		
		internal virtual void  printScripts()
		{
			long n = readU32();
			printOffset();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(n + " Script Entries");
			for (int i = 0; i < n; i++)
			{
				int start = offset;
				printOffset();
				System.String name = "script" + System.Convert.ToString(i);
				int init = (int) readU32(); // eat init method
				MethodInfo mi = methods[init];
				mi.name = name + "$init";
				mi.className = name;
				mi.kind = TRAIT_Method;
				for (int x = start; x < offset; x++)
				{
					out_Renamed.Write(hex(abc[(int) x]) + " ");
				}
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine(name + " ");
				
				int numTraits = (int) readU32(); // number of traits
				printOffset();
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine(numTraits + " Traits Entries");
				for (int j = 0; j < numTraits; j++)
				{
					printOffset();
					start = offset;
					System.String s = multiNameConstants[(int) readU32()].ToString(); // eat trait name;
					int b = abc[offset++];
					int kind = b & 0xf;
					switch (kind)
					{
						
						case 0x00: 
						// slot
						case 0x06:  // const
							readU32(); // id
							readU32(); // type
							int index = (int) readU32(); // index;
							if (index != 0)
								offset++; // kind
							break;
						
						case 0x04:  // class
							readU32(); // id
							readU32(); // value;
							break;
						
						default: 
							readU32(); // id
							mi = methods[(int) readU32()]; // method
							mi.name = s;
							mi.className = name;
							mi.kind = kind;
							break;
						
					}
					if ((b >> 4 & 0x4) == 0x4)
					{
						int val = (int) readU32(); // metadata count
						for (int k = 0; k < val; k++)
						{
							readU32(); // metadata
						}
					}
					for (int x = start; x < offset; x++)
					{
						out_Renamed.Write(hex(abc[(int) x]) + " ");
					}
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine(s);
				}
			}
		}
		
		internal virtual void  printBodies()
		{
			long n = readU32();
			printOffset();
			//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
			out_Renamed.WriteLine(n + " Method Bodies");
			for (int i = 0; i < n; i++)
			{
				printOffset();
				int start = offset;
				int methodIndex = (int) readU32();
				int maxStack = (int) readU32();
				int localCount = (int) readU32();
				int initScopeDepth = (int) readU32();
				int maxScopeDepth = (int) readU32();
				int codeLength = (int) readU32();
				for (int x = start; x < offset; x++)
				{
					out_Renamed.Write(hex(abc[(int) x]) + " ");
				}
				for (int x = offset - start; x < 7; x++)
				{
					out_Renamed.Write("   ");
				}
				MethodInfo mi = methods[methodIndex];
				out_Renamed.Write(traitKinds[mi.kind] + " ");
				out_Renamed.Write(mi.className + "::" + mi.name + "(");
				for (int x = 0; x < mi.paramCount - 1; x++)
				{
					out_Renamed.Write(multiNameConstants[mi.params_Renamed[x]].ToString() + ", ");
				}
				if (mi.paramCount > 0)
				{
					out_Renamed.Write(multiNameConstants[mi.params_Renamed[mi.paramCount - 1]].ToString());
				}
				out_Renamed.Write("):");
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine(multiNameConstants[mi.returnType].ToString());
				printOffset();
				out_Renamed.Write("maxStack:" + maxStack + " localCount:" + localCount + " ");
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("initScopeDepth:" + initScopeDepth + " maxScopeDepth:" + maxScopeDepth);
				
				LabelMgr labels = new LabelMgr(this);
				int stopAt = codeLength + offset;
				while (offset < stopAt)
				{
					System.String s = "";
					start = offset;
					printOffset();
					int opcode = abc[offset++] & 0xFF;
					
					if (opcode == OP_label || labels.hasLabelAt(offset - 1))
					{
						s = labels.getLabelAt(offset - 1) + ":";
						while (s.Length < 4)
							s += " ";
					}
					else
						s = "    ";
					
					
					s += opNames[opcode];
					s += (opNames[opcode].Length < 8?"\t\t":"\t");
					
					switch (opcode)
					{
						
						case OP_debugfile: 
						case OP_pushstring: 
							s += ('"' + stringConstants[(int) readU32()].replaceAll("\n", "\\n").replaceAll("\t", "\\t") + '"');
							break;
						
						case OP_pushnamespace: 
							s += namespaceConstants[(int) readU32()];
							break;
						
						case OP_pushint: 
							int k = intConstants[(int) readU32()];
							s += (k + "\t// 0x" + System.Convert.ToString(k, 16));
							break;
						
						case OP_pushuint: 
							long u = uintConstants[(int) readU32()];
							s += (u + "\t// 0x" + System.Convert.ToString(u, 16));
							break;
						
						case OP_pushdouble: 
							int f = (int) readU32();
							s += ("floatConstant" + f);
							break;
						
						case OP_getsuper: 
						case OP_setsuper: 
						case OP_getproperty: 
						case OP_initproperty: 
						case OP_setproperty: 
						case OP_getlex: 
						case OP_findpropstrict: 
						case OP_findproperty: 
						case OP_finddef: 
						case OP_deleteproperty: 
						case OP_istype: 
						case OP_coerce: 
						case OP_astype: 
						case OP_getdescendants: 
							s += multiNameConstants[(int) readU32()];
							break;
						
						case OP_constructprop: 
						case OP_callproperty: 
						case OP_callproplex: 
						case OP_callsuper: 
						case OP_callsupervoid: 
						case OP_callpropvoid: 
							s += multiNameConstants[(int) readU32()];
							s += (" (" + readU32() + ")");
							break;
						
						case OP_newfunction: 
							int method_id = (int) readU32();
							s += methods[method_id].name;
							// abc.methods[method_id].anon = true  (do later?)
							break;
						
						case OP_callstatic: 
							s += methods[(int) readU32()].name;
							s += (" (" + readU32() + ")");
							break;
						
						case OP_newclass: 
							s += instanceNames[(int) readU32()];
							break;
						
						case OP_lookupswitch: 
							int pos = offset - 1;
							int target = pos + readS24();
							int maxindex = (int) readU32();
							s += ("default:" + labels.getLabelAt(target)); // target + "("+(target-pos)+")"
							s += (" maxcase:" + System.Convert.ToString(maxindex));
							for (int m = 0; m <= maxindex; m++)
							{
								target = pos + readS24();
								s += (" " + labels.getLabelAt(target)); // target + "("+(target-pos)+")"
							}
							break;
						
						case OP_jump: 
						case OP_iftrue: 
						case OP_iffalse: 
						case OP_ifeq: 
						case OP_ifne: 
						case OP_ifge: 
						case OP_ifnge: 
						case OP_ifgt: 
						case OP_ifngt: 
						case OP_ifle: 
						case OP_ifnle: 
						case OP_iflt: 
						case OP_ifnlt: 
						case OP_ifstricteq: 
						case OP_ifstrictne: 
							int delta = (int) readS24();
							int targ = offset + delta;
							//s += target + " ("+offset+")"
							s += labels.getLabelAt(targ);
							if (!(labels.hasLabelAt(offset)))
								s += "\n";
							break;
						
						case OP_inclocal: 
						case OP_declocal: 
						case OP_inclocal_i: 
						case OP_declocal_i: 
						case OP_getlocal: 
						case OP_kill: 
						case OP_setlocal: 
						case OP_debugline: 
						case OP_getglobalslot: 
						case OP_getslot: 
						case OP_setglobalslot: 
						case OP_setslot: 
						case OP_pushshort: 
						case OP_newcatch: 
							s += readU32();
							break;
						
						case OP_debug: 
							s += System.Convert.ToString(abc[offset++] & 0xFF);
							s += (" " + readU32());
							s += (" " + System.Convert.ToString(abc[offset++] & 0xFF));
							s += (" " + readU32());
							break;
						
						case OP_newobject: 
							s += ("{" + readU32() + "}");
							break;
						
						case OP_newarray: 
							s += ("[" + readU32() + "]");
							break;
						
						case OP_call: 
						case OP_construct: 
						case OP_constructsuper: 
							s += ("(" + readU32() + ")");
							break;
						
						case OP_pushbyte: 
						case OP_getscopeobject: 
							s += abc[offset++];
							break;
						
						case OP_hasnext2: 
							s += (readU32() + " " + readU32());
							goto default;
						
						default: 
							/*if (opNames[opcode] == ("0x"+opcode.toString(16).toUpperCase()))
							s += " UNKNOWN OPCODE"*/
							break;
						
					}
					for (int x = start; x < offset; x++)
					{
						out_Renamed.Write(hex(abc[(int) x]) + " ");
					}
					for (int x = offset - start; x < 7; x++)
					{
						out_Renamed.Write("   ");
					}
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine(s);
				}
				int exCount = (int) readU32();
				printOffset();
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine(exCount + " Extras");
				for (int j = 0; j < exCount; j++)
				{
					start = offset;
					printOffset();
					int from = (int) readU32();
					int to = (int) readU32();
					int target = (int) readU32();
					int typeIndex = (int) readU32();
					int nameIndex = (int) readU32();
					for (int x = start; x < offset; x++)
					{
						out_Renamed.Write(hex(abc[(int) x]) + " ");
					}
					out_Renamed.Write(multiNameConstants[nameIndex] + " ");
					out_Renamed.Write("type:" + multiNameConstants[typeIndex] + " from:" + from + " ");
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine("to:" + to + " target:" + target);
				}
				int numTraits = (int) readU32(); // number of traits
				printOffset();
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine(numTraits + " Traits Entries");
				for (int j = 0; j < numTraits; j++)
				{
					printOffset();
					start = offset;
					System.String s = multiNameConstants[(int) readU32()].ToString(); // eat trait name;
					int b = abc[offset++];
					int kind = b & 0xf;
					switch (kind)
					{
						
						case 0x00: 
						// slot
						case 0x06:  // const
							readU32(); // id
							readU32(); // type
							int index = (int) readU32(); // index;
							if (index != 0)
								offset++; // kind
							break;
						
						case 0x04:  // class
							readU32(); // id
							readU32(); // value;
							break;
						
						default: 
							readU32(); // id
							readU32(); // method
							break;
						
					}
					if ((b >> 4 & 0x4) == 0x4)
					{
						int val = (int) readU32(); // metadata count
						for (int k = 0; k < val; k++)
						{
							readU32(); // metadata
						}
					}
					for (int x = start; x < offset; x++)
					{
						out_Renamed.Write(hex(abc[(int) x]) + " ");
					}
					//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
					out_Renamed.WriteLine(s);
				}
				//UPGRADE_TODO: Method 'java.io.PrintWriter.println' was converted to 'System.IO.TextWriter.WriteLine' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioPrintWriterprintln_javalangString'"
				out_Renamed.WriteLine("");
			}
		}
		
		//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'MultiName' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
		internal class MultiName
		{
			private void  InitBlock(AbcPrinter enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private AbcPrinter enclosingInstance;
			public AbcPrinter Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			public MultiName(AbcPrinter enclosingInstance)
			{
				InitBlock(enclosingInstance);
			}
			
			public int kind;
			public int long1;
			public int long2;
			
			public override System.String ToString()
			{
				System.String s = "";
				
				System.String[] nsSet;
				int len;
				int j;
				
				switch (kind)
				{
					
					case 0x07: 
					// QName
					case 0x0D: 
						s = Enclosing_Instance.namespaceConstants[(int) long1] + ":";
						s += Enclosing_Instance.stringConstants[(int) long2];
						break;
					
					case 0x0F: 
					// RTQName
					case 0x10: 
						s = Enclosing_Instance.stringConstants[(int) long1];
						break;
					
					case 0x11: 
					// RTQNameL
					case 0x12: 
						s = "RTQNameL";
						break;
					
					case 0x13: 
					// NameL
					case 0x14: 
						s = "NameL";
						break;
					
					case 0x09: 
					case 0x0E: 
						nsSet = Enclosing_Instance.namespaceSetConstants[(int) long2];
						len = nsSet.Length;
						for (j = 0; j < len - 1; j++)
						{
							s += (nsSet[j] + ",");
						}
						if (len > 0)
							s += (nsSet[len - 1] + ":");
						s += Enclosing_Instance.stringConstants[(int) long1];
						break;
					
					case 0x1B: 
					case 0x1C: 
						nsSet = Enclosing_Instance.namespaceSetConstants[(int) long1];
						len = nsSet.Length;
						for (j = 0; j < len - 1; j++)
						{
							s += (nsSet[j] + ",");
						}
						if (len > 0)
							s += (nsSet[len - 1] + ":");
						s += "null";
						break;
					}
				return s;
			}
		}
		
		//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'MethodInfo' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
		internal class MethodInfo
		{
			public MethodInfo(AbcPrinter enclosingInstance)
			{
				InitBlock(enclosingInstance);
			}
			private void  InitBlock(AbcPrinter enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private AbcPrinter enclosingInstance;
			public AbcPrinter Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			internal int paramCount;
			internal int returnType;
			internal int[] params_Renamed;
			internal System.String name;
			internal int kind;
			internal int flags;
			internal int optionCount;
			internal int[] optionKinds;
			internal int[] optionIndex;
			internal int[] paramNames;
			internal System.String className;
		}
		
		//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'LabelMgr' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
		internal class LabelMgr
		{
			private void  InitBlock(AbcPrinter enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}
			private AbcPrinter enclosingInstance;
			public AbcPrinter Enclosing_Instance
			{
				get
				{
					return enclosingInstance;
				}
				
			}
			internal int index = 0;
			
			//UPGRADE_TODO: Class 'java.util.HashMap' was converted to 'System.Collections.Hashtable' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMap'"
			internal System.Collections.Hashtable labels;
			
			public LabelMgr(AbcPrinter enclosingInstance)
			{
				InitBlock(enclosingInstance);
				//UPGRADE_TODO: Class 'java.util.HashMap' was converted to 'System.Collections.Hashtable' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMap'"
				labels = new System.Collections.Hashtable();
			}
			
			public virtual System.String getLabelAt(int offset)
			{
				System.String key = System.Convert.ToString(offset);
				if (!labels.ContainsKey(key))
					labels[key] = (System.Int32) index++;
				//UPGRADE_TODO: Method 'java.util.HashMap.get' was converted to 'System.Collections.Hashtable.Item' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMapget_javalangObject'"
				return "L" + ((System.Int32) labels[key]).ToString();
			}
			
			public virtual bool hasLabelAt(int offset)
			{
				System.String key = System.Convert.ToString(offset);
				return labels.ContainsKey(key);
			}
		}
	}
}