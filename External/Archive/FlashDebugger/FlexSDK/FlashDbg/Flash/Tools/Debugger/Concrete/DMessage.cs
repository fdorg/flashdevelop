// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
using FieldFormat = Flash.Util.FieldFormat;
using Trace = Flash.Util.Trace;

namespace Flash.Tools.Debugger.Concrete
{
	
	/// <summary> DMessage.java
	/// 
	/// Wraps the contents of a specific message and provides a set of APIs that allow for
	/// typed extraction of fields within the message.
	/// 
	/// No interpretation of the messages contents is performed, this is left to the
	/// user of this class.  The code was constructed in this fashion, so that it more
	/// closely mimics the DataReader/DataWriter classes used in the Player code.
	/// 
	/// The type of the message should be one of the InXXX or OutXXX constant integers,
	/// but no checking of conformance is provided in this class.
	/// </summary>
	public class DMessage
	{
		virtual public int Type
		{
			/* getters/setters */
			
			get
			{
				return m_type;
			}
			
			set
			{
				m_type = value;
			}
			
		}
		virtual public String InTypeName
		{
			get
			{
				return inTypeName(Type);
			}
			
		}
		virtual public String OutTypeName
		{
			get
			{
				return outTypeName(Type);
			}
			
		}
		virtual public byte[] Data
		{
			get
			{
				return m_content;
			}
			
		}
		virtual public int Size
		{
			get
			{
				return (m_content == null) ? 0 : m_content.Length;
			}
			
		}
		virtual public int Remaining
		{
			get
			{
				return Size - m_index;
			}
			
		}
		virtual public int Position
		{
			get
			{
				return m_index;
			}

            set
            {
                if (value < 0)
                    throw new IndexOutOfRangeException(value + " < 0"); //$NON-NLS-1$

                if (value > m_content.Length)
                    throw new IndexOutOfRangeException(value + " > " + m_content.Length); //$NON-NLS-1$

                m_index = value;
            }
			
		}
		/// <summary> This set of constants defines the message types RECEIVED from the player
		/// through our debug socket
		/// </summary>
		public const int InUnknown = - 1;
		public const int InSetMenuState = 0;
		public const int InSetProperty = 1;
		public const int InExit = 2;
		public const int InNewObject = 3;
		public const int InRemoveObject = 4;
		public const int InTrace = 5;
		public const int InErrorTarget = 6;
		public const int InErrorExecLimit = 7;
		public const int InErrorWith = 8;
		public const int InErrorProtoLimit = 9;
		public const int InSetVariable = 10;
		public const int InDeleteVariable = 11;
		public const int InParam = 12;
		public const int InPlaceObject = 13;
		public const int InScript = 14;
		public const int InAskBreakpoints = 15;
		public const int InBreakAt = 16;
		public const int InContinue = 17;
		public const int InSetLocalVariables = 18;
		public const int InSetBreakpoint = 19;
		public const int InNumScript = 20;
		public const int InRemoveScript = 21;
		public const int InRemoveBreakpoint = 22;
		public const int InNotSynced = 23;
		public const int InErrorURLOpen = 24;
		public const int InProcessTag = 25;
		public const int InVersion = 26;
		public const int InBreakAtExt = 27;
		public const int InSetVariable2 = 28;
		public const int InSquelch = 29;
		public const int InGetVariable = 30;
		public const int InFrame = 31;
		public const int InOption = 32;
		public const int InWatch = 33;
		public const int InGetSwf = 34;
		public const int InGetSwd = 35;
		public const int InErrorException = 36;
		public const int InErrorStackUnderflow = 37;
		public const int InErrorZeroDivide = 38;
		public const int InErrorScriptStuck = 39;
		public const int InBreakReason = 40;
		public const int InGetActions = 41;
		public const int InSwfInfo = 42;
		public const int InConstantPool = 43;
		public const int InErrorConsole = 44;
		public const int InGetFncNames = 45;
		// 46 through 52 are for profiling
		public const int InWatch2 = 55;
		// If you add another message here, adjust the following line
		// and add a new case to the inTypeName() method below.
		public static readonly int InSIZE = InWatch2 + 1; /* last ID used +1 */
		
		/// <summary> This set of constants defines the message types SENT to the player from our
		/// debug socket (WARNING: ID space overlaps with InXXX)
		/// </summary>
		public const int OutUnknown = - 2;
		public const int OutZoomIn = 0;
		public const int OutZoomOut = 1;
		public const int OutZoom100 = 2;
		public const int OutHome = 3;
		public const int OutSetQuality = 4;
		public const int OutPlay = 5;
		public const int OutLoop = 6;
		public const int OutRewind = 7;
		public const int OutForward = 8;
		public const int OutBack = 9;
		public const int OutPrint = 10;
		public const int OutSetVariable = 11;
		public const int OutSetProperty = 12;
		public const int OutExit = 13;
		public const int OutSetFocus = 14;
		public const int OutContinue = 15;
		public const int OutStopDebug = 16;
		public const int OutSetBreakpoints = 17;
		public const int OutRemoveBreakpoints = 18;
		public const int OutRemoveAllBreakpoints = 19;
		public const int OutStepOver = 20;
		public const int OutStepInto = 21;
		public const int OutStepOut = 22;
		public const int OutProcessedTag = 23;
		public const int OutSetSquelch = 24;
		public const int OutGetVariable = 25;
		public const int OutGetFrame = 26;
		public const int OutGetOption = 27;
		public const int OutSetOption = 28;
		public const int OutAddWatch = 29; // 16-bit id; used for as2
		public const int OutRemoveWatch = 30; // 16-bit id; used for as2
		public const int OutStepContinue = 31;
		public const int OutGetSwf = 32;
		public const int OutGetSwd = 33;
		public const int OutGetVariableWhichInvokesGetter = 34;
		public const int OutGetBreakReason = 35;
		public const int OutGetActions = 36;
		public const int OutSetActions = 37;
		public const int OutSwfInfo = 38;
		public const int OutConstantPool = 39;
		public const int OutGetFncNames = 40;
		// 41 through 47 are for profiling
		public const int OutAddWatch2 = 49; // 32-bit id; used for as3
		public const int OutRemoveWatch2 = 50; // 32-bit id; used for as3
		// If you add another message here, adjust the following line
		// and add a new case to the outTypeName() method below.
		public static readonly int OutSIZE = OutGetFncNames + 1; /* last ID used +1 */
		
		/// <summary> Enums originally extracted from shared_tcserver/tcparser.h; these correspond
		/// to Flash player values that are currently in playerdebugger.h, class DebugAtomType.
		/// </summary>
		public const int kNumberType = 0;
		public const int kBooleanType = 1;
		public const int kStringType = 2;
		public const int kObjectType = 3;
		public const int kMovieClipType = 4;
		public const int kNullType = 5;
		public const int kUndefinedType = 6;
		public const int kReferenceType = 7;
		public const int kArrayType = 8;
		public const int kObjectEndType = 9;
		public const int kStrictArrayType = 10;
		public const int kDateType = 11;
		public const int kLongStringType = 12;
		public const int kUnsupportedType = 13;
		public const int kRecordSetType = 14;
		public const int kXMLType = 15;
		public const int kTypedObjectType = 16;
		public const int kAvmPlusObjectType = 17;
		public const int kNamespaceType = 18;
		public const int kTraitsType = 19; // This one is special: When passed to the debugger, it indicates
		// that the "variable" is not a variable at all, but rather is a
		// class name.  For example, if class Y extends class X, then
		// we will send a kDTypeTraits for class Y; then we'll send all the
		// members of class Y; then we'll send a kDTypeTraits for class X;
		// and then we'll send all the members of class X.  This is only
		// used by the AVM+ debugger.
		
		/* byte array of our message and current index into it */
		internal byte[] m_content; /* the data bytes of the message */
		internal int m_index; /* current position within the content array */
		internal int m_type; /* one of OutXXX or InXXX integer constants */
		
		/* Debugging only: The contents of this message, formatted as a string for display */
		private System.Text.StringBuilder m_debugFormatted;
		/* Debugging only: The number of bytes from the input that we have formatted into m_debugFormatted */
		private int m_debugFormattedThroughIndex;
		
		/* used by our cache to create empty DMessages */
		public DMessage(int size)
		{
			m_content = new byte[size];
			m_debugFormatted = new System.Text.StringBuilder();
			m_debugFormattedThroughIndex = 0;
			clear();
		}
		
		/// <summary> Allow the message to be 're-parsed' by someone else</summary>
		public virtual void  reset()
		{
			m_index = 0;
		}
		
		/// <summary> Allow the message to be reused later</summary>
		public virtual void  clear()
		{
			Type = - 1;
			m_debugFormatted.Length = 0;
			m_debugFormattedThroughIndex = 0;
			reset();
		}

        /// <summary> Extract the next byte</summary>
        virtual public int getByte()
        {
            if (m_index + 1 > m_content.Length)
                throw new IndexOutOfRangeException(m_content.Length - m_index + " < 1"); //$NON-NLS-1$

            int value = m_content[m_index++];
            debugAppendNumber(value, 1);
            return value;
        }

        /// <summary> Extract the next 2 bytes, which form a 16b integer, from the message</summary>
        virtual public int getWord()
        {
            if (m_index + 2 > m_content.Length)
                throw new IndexOutOfRangeException(m_content.Length - m_index + " < 2"); //$NON-NLS-1$

            int b0 = m_content[m_index++];
            int b1 = m_content[m_index++];

            int value = ((b1 << 8) & 0xff00) | (b0 & 0xff);
            debugAppendNumber(value, 2);
            return value;
        }

        /// <summary> Extract the next 4 bytes, which form a 32b integer, from the message</summary>
        virtual public long getDWord()
        {
            if (m_index + 4 > m_content.Length)
                throw new IndexOutOfRangeException(m_content.Length - m_index + " < 4"); //$NON-NLS-1$

            int b0 = m_content[m_index++];
            int b1 = m_content[m_index++];
            int b2 = m_content[m_index++];
            int b3 = m_content[m_index++];

            long value = ((b3 << 24) & unchecked((int)0xff000000)) | ((b2 << 16) & 0xff0000) | ((b1 << 8) & 0xff00) | (b0 & 0xff);
            debugAppendNumber(value, 4);
            return value;
        }
        /// <summary> Heart wrenchingly slow but since we don't have a length so we can't
        /// do much better
        /// </summary>
        virtual public String getString()
        {
            int startAt = m_index;

            /* scan looking for a terminating null */
            while (true)
            {
                int ch = m_content[m_index++];
                if (ch == 0)
                    break;
                else if (m_index > m_content.Length)
                    throw new IndexOutOfRangeException("no string terminator found @" + m_index); //$NON-NLS-1$
            }

            /* build a new string and return it */
            String s;
            try
            {
                // The player I believe uses UTF-8?
                s = System.Text.Encoding.UTF8.GetString(m_content, startAt, m_index - startAt - 1); //$NON-NLS-1$
            }
            catch (IOException)
            {
                // couldn't convert so let's try the default
                s = System.Text.Encoding.Default.GetString(m_content, startAt, m_index - startAt - 1);
            }
            debugAppendString(s);
            return s;
        }

        /// <summary> Append a byte to the end of the message</summary>
		public virtual void  putByte(byte b0)
		{
			if (m_index + 1 > m_content.Length)
				throw new IndexOutOfRangeException(m_content.Length - m_index + " < 1"); //$NON-NLS-1$
			
			m_content[m_index++] = b0;
			debugAppendNumber(b0, 1);
		}
		
		/// <summary> Append 2 bytes, which form a 16b integer, into the message</summary>
		public virtual void  putWord(int val)
		{
			if (m_index + 2 > m_content.Length)
				throw new IndexOutOfRangeException(m_content.Length - m_index + " < 2"); //$NON-NLS-1$
			
			m_content[m_index++] = (byte) (val);
			m_content[m_index++] = (byte) (val >> 8);
			
			debugAppendNumber(val, 2);
		}
		
		/// <summary> Append 4 bytes, which form a 32b integer, into the message</summary>
		public virtual void  putDWord(long val)
		{
			if (m_index + 4 > m_content.Length)
				throw new IndexOutOfRangeException(m_content.Length - m_index + " < 4"); //$NON-NLS-1$
			
			m_content[m_index++] = (byte) (val);
			m_content[m_index++] = (byte) (val >> 8);
			m_content[m_index++] = (byte) (val >> 16);
			m_content[m_index++] = (byte) (val >> 24);
			
			debugAppendNumber(val, 4);
		}

        /// <summary> Helper to get the number of bytes that a string will need when it is sent
		/// across the socket to the Flash player.  Do *not* use string.getLength(),
		/// because that will return an incorrect result for strings that have non-
		/// ASCII characters.
		/// </summary>
		public static int getStringLength(String s)
		{
			try
			{
				return System.Text.Encoding.UTF8.GetByteCount(s); //$NON-NLS-1$
			}
			catch (IOException e)
			{
				if (Trace.error)
				{
					Trace.trace(e.ToString());
				}
				return 0;
			}
		}
		
		/// <summary> Place a string into the message (using UTF-8 encoding)</summary>
		public virtual void  putString(String s)
		{
			/* convert the string into a byte array */
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s); //$NON-NLS-1$
			int length = bytes.Length;
			int endAt = m_index + length + 1;
			
			if (endAt > m_content.Length)
				throw new IndexOutOfRangeException(endAt + " > " + m_content.Length); //$NON-NLS-1$
			
			/* copy the string as a byte array */
			Array.Copy(bytes, 0, m_content, m_index, length);
			m_index += length;
			
			/* now the null terminator */
			m_content[m_index++] = (byte) '\x0000';
			
			debugAppendString(s);
		}
		
		// Debugging helper function: we've parsed a number out of the stream of input bytes,
		// so record that as a hex number of the appropriate length, e.g. "0x12" or "0x1234"
		// or "0x12345678", depending on numBytes.
		private void  debugAppendNumber(long value_Renamed, int numBytes)
		{
			if (PlayerSession.m_debugMsgOn || PlayerSession.m_debugMsgFileOn)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.Append("0x"); //$NON-NLS-1$
				FieldFormat.formatLongToHex(sb, value_Renamed, numBytes * 2, true);
				debugAppend(sb.ToString());
			}
		}
		
		// Debugging helper function: we've parsed a string out of the stream of input bytes,
		// so record it as a quoted string in the formatted debugging output.
		private void  debugAppendString(String s)
		{
			if (PlayerSession.m_debugMsgOn || PlayerSession.m_debugMsgFileOn)
				debugAppend('"' + s + '"');
		}
		
		// Debugging helper function: append a string to the formatted debugging output.
		private void  debugAppend(String s)
		{
			if (PlayerSession.m_debugMsgOn || PlayerSession.m_debugMsgFileOn)
			{
				if (m_index > m_debugFormattedThroughIndex)
				{
					m_debugFormattedThroughIndex = m_index;
					if (m_debugFormatted.Length > 0)
						m_debugFormatted.Append(' ');
					m_debugFormatted.Append(s);
				}
			}
		}
		
		public virtual String inToString()
		{
			return inToString(16);
		}
		
		public virtual String inToString(int maxContentBytes)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(InTypeName);
			sb.Append('[');
			sb.Append(Size);
			sb.Append("] "); //$NON-NLS-1$
			if (Size > 0)
				appendContent(sb, maxContentBytes);
			
			return sb.ToString();
		}
		
		public virtual String outToString()
		{
			return outToString(16);
		}
		
		public virtual String outToString(int maxContentBytes)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(OutTypeName);
			sb.Append('[');
			sb.Append(Size);
			sb.Append("] "); //$NON-NLS-1$
			if (Size > 0)
				appendContent(sb, maxContentBytes);
			
			return sb.ToString();
		}
		
		public virtual System.Text.StringBuilder appendContent(System.Text.StringBuilder sb, int max)
		{
			int size = Size;
			byte[] data = Data;
			int i = 0;
			
			// First, output formatted content -- content for which some of the other functions
			// in this class, such as getDWord and getString, did formatting.
			sb.Append(m_debugFormatted);
			
			// Now, for any left-over bytes which no one bothered to parse, output them as hex. 
			for (i = 0; i < max && i + m_debugFormattedThroughIndex < size; i++)
			{
				int v = data[i + m_debugFormattedThroughIndex] & 0xff;
				sb.Append(" 0x"); //$NON-NLS-1$
				FieldFormat.formatLongToHex(sb, v, 2, true);
			}
			
			if (i + m_debugFormattedThroughIndex < size)
				sb.Append(" ..."); //$NON-NLS-1$
			
			return sb;
		}
		
		/// <summary> Convenience function for converting a type into a name used mainly for debugging
		/// but can also be used during trace facility of command line session
		/// </summary>
		public static String inTypeName(int type)
		{
			String s = "InUnknown(" + type + ")"; //$NON-NLS-1$ //$NON-NLS-2$
			
			switch (type)
			{
				
				case InSetMenuState: 
					s = "InSetMenuState"; //$NON-NLS-1$
					break;
				
				
				case InSetProperty: 
					s = "InSetProperty"; //$NON-NLS-1$
					break;
				
				
				case InExit: 
					s = "InExit"; //$NON-NLS-1$
					break;
				
				
				case InNewObject: 
					s = "InNewObject"; //$NON-NLS-1$
					break;
				
				
				case InRemoveObject: 
					s = "InRemoveObject"; //$NON-NLS-1$
					break;
				
				
				case InTrace: 
					s = "InTrace"; //$NON-NLS-1$
					break;
				
				
				case InErrorTarget: 
					s = "InErrorTarget"; //$NON-NLS-1$
					break;
				
				
				case InErrorExecLimit: 
					s = "InErrorExecLimit"; //$NON-NLS-1$
					break;
				
				
				case InErrorWith: 
					s = "InErrorWith"; //$NON-NLS-1$
					break;
				
				
				case InErrorProtoLimit: 
					s = "InErrorProtoLimit"; //$NON-NLS-1$
					break;
				
				
				case InSetVariable: 
					s = "InSetVariable"; //$NON-NLS-1$
					break;
				
				
				case InDeleteVariable: 
					s = "InDeleteVariable"; //$NON-NLS-1$
					break;
				
				
				case InParam: 
					s = "InParam"; //$NON-NLS-1$
					break;
				
				
				case InPlaceObject: 
					s = "InPlaceObject"; //$NON-NLS-1$
					break;
				
				
				case InScript: 
					s = "InScript"; //$NON-NLS-1$
					break;
				
				
				case InAskBreakpoints: 
					s = "InAskBreakpoints"; //$NON-NLS-1$
					break;
				
				
				case InBreakAt: 
					s = "InBreakAt"; //$NON-NLS-1$
					break;
				
				
				case InContinue: 
					s = "InContinue"; //$NON-NLS-1$
					break;
				
				
				case InSetLocalVariables: 
					s = "InSetLocalVariables"; //$NON-NLS-1$
					break;
				
				
				case InSetBreakpoint: 
					s = "InSetBreakpoint"; //$NON-NLS-1$
					break;
				
				
				case InNumScript: 
					s = "InNumScript"; //$NON-NLS-1$
					break;
				
				
				case InRemoveScript: 
					s = "InRemoveScript"; //$NON-NLS-1$
					break;
				
				
				case InRemoveBreakpoint: 
					s = "InRemoveBreakpoint"; //$NON-NLS-1$
					break;
				
				
				case InNotSynced: 
					s = "InNotSynced"; //$NON-NLS-1$
					break;
				
				
				case InErrorURLOpen: 
					s = "InErrorURLOpen"; //$NON-NLS-1$
					break;
				
				
				case InProcessTag: 
					s = "InProcessTag"; //$NON-NLS-1$
					break;
				
				
				case InVersion: 
					s = "InVersion"; //$NON-NLS-1$
					break;
				
				
				case InBreakAtExt: 
					s = "InBreakAtExt"; //$NON-NLS-1$
					break;
				
				
				case InSetVariable2: 
					s = "InSetVariable2"; //$NON-NLS-1$
					break;
				
				
				case InSquelch: 
					s = "InSquelch"; //$NON-NLS-1$
					break;
				
				
				case InGetVariable: 
					s = "InGetVariable"; //$NON-NLS-1$
					break;
				
				
				case InFrame: 
					s = "InFrame"; //$NON-NLS-1$
					break;
				
				
				case InOption: 
					s = "InOption"; //$NON-NLS-1$
					break;
				
				
				case InWatch: 
					s = "InWatch"; //$NON-NLS-1$
					break;
				
				
				case InGetSwf: 
					s = "InGetSwf"; //$NON-NLS-1$
					break;
				
				
				case InGetSwd: 
					s = "InGetSwd"; //$NON-NLS-1$
					break;
				
				
				case InErrorException: 
					s = "InErrorException"; //$NON-NLS-1$
					break;
				
				
				case InErrorStackUnderflow: 
					s = "InErrorStackUnderflow"; //$NON-NLS-1$
					break;
				
				
				case InErrorZeroDivide: 
					s = "InErrorZeroDivide"; //$NON-NLS-1$
					break;
				
				
				case InErrorScriptStuck: 
					s = "InErrorScriptStuck"; //$NON-NLS-1$
					break;
				
				
				case InBreakReason: 
					s = "InBreakReason"; //$NON-NLS-1$
					break;
				
				
				case InGetActions: 
					s = "InGetActions"; //$NON-NLS-1$
					break;
				
				
				case InSwfInfo: 
					s = "InSwfInfo"; //$NON-NLS-1$
					break;
				
				
				case InConstantPool: 
					s = "InConstantPool"; //$NON-NLS-1$
					break;
				
				
				case InErrorConsole: 
					s = "InErrorConsole"; //$NON-NLS-1$
					break;
				
				
				case InGetFncNames: 
					s = "InGetFncNames"; //$NON-NLS-1$
					break;
				}
			return s;
		}
		
		/// <summary> Convenience function for converting a type into a name used mainly for debugging
		/// but can also be used during trace facility of command line session
		/// </summary>
		public static String outTypeName(int type)
		{
			String s = "OutUnknown(" + type + ")"; //$NON-NLS-1$ //$NON-NLS-2$
			
			switch (type)
			{
				
				case OutZoomIn: 
					s = "OutZoomIn"; //$NON-NLS-1$
					break;
				
				
				case OutZoomOut: 
					s = "OutZoomOut"; //$NON-NLS-1$
					break;
				
				
				case OutZoom100: 
					s = "OutZoom100"; //$NON-NLS-1$
					break;
				
				
				case OutHome: 
					s = "OutHome"; //$NON-NLS-1$
					break;
				
				
				case OutSetQuality: 
					s = "OutSetQuality"; //$NON-NLS-1$
					break;
				
				
				case OutPlay: 
					s = "OutPlay"; //$NON-NLS-1$
					break;
				
				
				case OutLoop: 
					s = "OutLoop"; //$NON-NLS-1$
					break;
				
				
				case OutRewind: 
					s = "OutRewind"; //$NON-NLS-1$
					break;
				
				
				case OutForward: 
					s = "OutForward"; //$NON-NLS-1$
					break;
				
				
				case OutBack: 
					s = "OutBack"; //$NON-NLS-1$
					break;
				
				
				case OutPrint: 
					s = "OutPrint"; //$NON-NLS-1$
					break;
				
				
				case OutSetVariable: 
					s = "OutSetVariable"; //$NON-NLS-1$
					break;
				
				
				case OutSetProperty: 
					s = "OutSetProperty"; //$NON-NLS-1$
					break;
				
				
				case OutExit: 
					s = "OutExit"; //$NON-NLS-1$
					break;
				
				
				case OutSetFocus: 
					s = "OutSetFocus"; //$NON-NLS-1$
					break;
				
				
				case OutContinue: 
					s = "OutContinue"; //$NON-NLS-1$
					break;
				
				
				case OutStopDebug: 
					s = "OutStopDebug"; //$NON-NLS-1$
					break;
				
				
				case OutSetBreakpoints: 
					s = "OutSetBreakpoints"; //$NON-NLS-1$
					break;
				
				
				case OutRemoveBreakpoints: 
					s = "OutRemoveBreakpoints"; //$NON-NLS-1$
					break;
				
				
				case OutRemoveAllBreakpoints: 
					s = "OutRemoveAllBreakpoints"; //$NON-NLS-1$
					break;
				
				
				case OutStepOver: 
					s = "OutStepOver"; //$NON-NLS-1$
					break;
				
				
				case OutStepInto: 
					s = "OutStepInto"; //$NON-NLS-1$
					break;
				
				
				case OutStepOut: 
					s = "OutStepOut"; //$NON-NLS-1$
					break;
				
				
				case OutProcessedTag: 
					s = "OutProcessedTag"; //$NON-NLS-1$
					break;
				
				
				case OutSetSquelch: 
					s = "OutSetSquelch"; //$NON-NLS-1$
					break;
				
				
				case OutGetVariable: 
					s = "OutGetVariable"; //$NON-NLS-1$
					break;
				
				
				case OutGetFrame: 
					s = "OutGetFrame"; //$NON-NLS-1$
					break;
				
				
				case OutGetOption: 
					s = "OutGetOption"; //$NON-NLS-1$
					break;
				
				
				case OutSetOption: 
					s = "OutSetOption"; //$NON-NLS-1$
					break;
				
				
				case OutAddWatch: 
					s = "OutAddWatch"; //$NON-NLS-1$
					break;
				
				
				case OutRemoveWatch: 
					s = "OutRemoveWatch"; //$NON-NLS-1$
					break;
				
				
				case OutStepContinue: 
					s = "OutStepContinue"; //$NON-NLS-1$
					break;
				
				
				case OutGetSwf: 
					s = "OutGetSwf"; //$NON-NLS-1$
					break;
				
				
				case OutGetSwd: 
					s = "OutGetSwd"; //$NON-NLS-1$
					break;
				
				
				case OutGetVariableWhichInvokesGetter: 
					s = "OutGetVariableWhichInvokesGetter"; //$NON-NLS-1$
					break;
				
				
				case OutGetBreakReason: 
					s = "OutGetBreakReason"; //$NON-NLS-1$
					break;
				
				
				case OutGetActions: 
					s = "OutGetActions"; //$NON-NLS-1$
					break;
				
				
				case OutSetActions: 
					s = "OutSetActions"; //$NON-NLS-1$
					break;
				
				
				case OutSwfInfo: 
					s = "OutSwfInfo"; //$NON-NLS-1$
					break;
				
				
				case OutConstantPool: 
					s = "OutConstantPool"; //$NON-NLS-1$
					break;
				
				
				case OutGetFncNames: 
					s = "OutGetFncNames"; //$NON-NLS-1$
					break;
				}
			return s;
		}
	}
}