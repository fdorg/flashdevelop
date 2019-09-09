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
namespace flash.util
{
	
	public sealed class URLEncoder
	{
		public const System.String charset = "UTF8"; //$NON-NLS-1$
		
		private URLEncoder()
		{
		}
		
		public static System.String encode(System.String s)
		{
			try
			{
				return encode(s, charset);
			}
			catch (System.IO.IOException ex)
			{
				throw new System.ArgumentException(charset);
			}
		}
		
		public static System.String encode(System.String s, System.String enc)
		{
			if (!needsEncoding(s))
			{
				return s;
			}
			
			int length = s.Length;
			
			System.Text.StringBuilder out_Renamed = new System.Text.StringBuilder(length);
			
			System.IO.MemoryStream buf = new System.IO.MemoryStream(10); // why 10? w3c says so.
			
			//UPGRADE_WARNING: At least one expression was used more than once in the target code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1181'"
			//UPGRADE_TODO: Constructor 'java.io.OutputStreamWriter.OutputStreamWriter' was converted to 'System.IO.StreamWriter.StreamWriter' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioOutputStreamWriterOutputStreamWriter_javaioOutputStream_javalangString'"
			System.IO.StreamWriter writer = new System.IO.StreamWriter(new System.IO.StreamWriter(buf, System.Text.Encoding.GetEncoding(enc)).BaseStream, new System.IO.StreamWriter(buf, System.Text.Encoding.GetEncoding(enc)).Encoding);
			
			for (int i = 0; i < length; i++)
			{
				int c = (int) s[i];
				if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == ' ')
				{
					if (c == ' ')
					{
						c = '+';
					}
					
					toHex(out_Renamed, SupportClass.ToSByteArray(buf.ToArray()));
					//UPGRADE_ISSUE: Method 'java.io.ByteArrayOutputStream.reset' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaioByteArrayOutputStreamreset'"
					buf.reset();
					
					out_Renamed.Append((char) c);
				}
				else
				{
					try
					{
						writer.Write((System.Char) c);
						
						if (c >= 0xD800 && c <= 0xDBFF && i < length - 1)
						{
							int d = (int) s[i + 1];
							if (d >= 0xDC00 && d <= 0xDFFF)
							{
								writer.Write((System.Char) d);
								i++;
							}
						}
						
						writer.Flush();
					}
					catch (System.IO.IOException ex)
					{
						throw new System.ArgumentException(s);
					}
				}
			}
			
			toHex(out_Renamed, SupportClass.ToSByteArray(buf.ToArray()));
			
			return out_Renamed.ToString();
		}
		
		private static void  toHex(System.Text.StringBuilder buffer, sbyte[] b)
		{
			for (int i = 0; i < b.Length; i++)
			{
				buffer.Append('%');
				
				//UPGRADE_ISSUE: Method 'java.lang.Character.forDigit' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangCharacterforDigit_int_int'"
				char ch = Character.forDigit((b[i] >> 4) & 0xF, 16);
				if (System.Char.IsLetter(ch))
				{
					ch -= (char) (32);
				}
				buffer.Append(ch);
				
				//UPGRADE_ISSUE: Method 'java.lang.Character.forDigit' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangCharacterforDigit_int_int'"
				ch = Character.forDigit(b[i] & 0xF, 16);
				if (System.Char.IsLetter(ch))
				{
					ch -= (char) (32);
				}
				buffer.Append(ch);
			}
		}
		
		private static bool needsEncoding(System.String s)
		{
			if (s == null)
			{
				return false;
			}
			
			int length = s.Length;
			
			for (int i = 0; i < length; i++)
			{
				int c = (int) s[i];
				if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9')
				{
					// keep going
				}
				else
				{
					return true;
				}
			}
			
			return false;
		}
	}
}