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
using System.IO.Compression;

namespace Flash.Swf
{
	public class SwfEncoder
	{
        private BinaryWriter binStream;

		private int bitPos = 8; //Must start as a full byte with value of 8
		private byte currentByte = (byte) (0x00);
		private int compressPos = - 1;
		
		internal int swfVersion;
		
		public SwfEncoder(int version)
		{
            MemoryStream memStream = new MemoryStream(1024);

            binStream = new BinaryWriter(memStream);
			swfVersion = version;
		}
		
		public virtual void  writeUI8(int c)
		{
			if (bitPos != 8 || c < 0 || c > 255)
			{
                System.Diagnostics.Debug.Assert(bitPos == 8);
				System.Diagnostics.Debug.Assert(c >= 0 && c <= 255, "UI8 overflow 0x" + c.ToString("X8"));
			}

            binStream.Write((byte)c);
		}
		
		public virtual void  writeFixed8(float v)
		{
            binStream.Write((UInt16)(int)(v * 256));
		}
		
		public virtual void  writeUI16(int c)
		{
			// FIXME - we should really deal with this upstream.
			// The standard case here is an unused fillstyle
			// when importing swfs with bitmaps from Matador.
			if (c == - 1)
				c = 65535;
			System.Diagnostics.Debug.Assert(bitPos == 8);
			System.Diagnostics.Debug.Assert(c >= 0 && c <= 65535, "UI16 overflow");
			binStream.Write((ushort)c);
		}
		
		public virtual void  writeSI16(int c)
		{
			System.Diagnostics.Debug.Assert(bitPos == 8);
			System.Diagnostics.Debug.Assert(c >= -32768 && c <= 32767, "SI16 overflow");
            binStream.Write((ushort)c);
		}
		
		public virtual void  write32(int c)
		{
			System.Diagnostics.Debug.Assert(bitPos == 8);
            binStream.Write(c);
		}
		
		public virtual void  write64(long c)
		{
            binStream.Write(c);
		}
		
		public virtual void  writeFloat(float f)
		{
			binStream.Write(f);
		}
		
		public virtual void  markComp()
		{
			compressPos = (int)binStream.BaseStream.Position;
		}
		
		/// <summary> compress the marked section of our buffer, in place.</summary>
		/// <throws>  IOException </throws>
		public virtual void  compress()
		{
			if (compressPos != - 1)
			{
				// compress in place from compressPos to pos
                binStream.BaseStream.Position = compressPos;

                byte[] bytes = new byte[binStream.BaseStream.Length - compressPos];

                binStream.BaseStream.Read(bytes, 0, bytes.Length);
                binStream.BaseStream.Position = compressPos;
				
				DeflateStream deflater = new DeflateStream(binStream.BaseStream, CompressionMode.Compress, true);
				
				deflater.Write(bytes, 0, bytes.Length);
				deflater.Close();

				compressPos = - 1;
			}
		}
		
		/// <summary> send buffer to the given stream.  If markComp was called, bytes after that mark
		/// will be compressed.
		/// </summary>
		/// <param name="output">
		/// </param>
		/// <throws>  IOException </throws>
		public void  WriteTo(Stream output)
		{
			if (compressPos == - 1)
			{
                byte[] bytes = new byte[binStream.BaseStream.Length];
                binStream.BaseStream.Position = 0;
                binStream.BaseStream.Read(bytes, 0, bytes.Length);
                output.Write(bytes, 0, (int)bytes.Length);
			}
			else
			{
				long Length = binStream.BaseStream.Length;
                byte[] bytes = new byte[compressPos];
                binStream.BaseStream.Read(bytes, 0, compressPos);

				output.Write(bytes, 0, compressPos);
				
                bytes = new byte[Length - compressPos];
                binStream.BaseStream.Read(bytes, 0, bytes.Length);

                binStream.BaseStream.Position = compressPos;

                DeflateStream deflater = new DeflateStream(output, CompressionMode.Compress, true);

                deflater.Write(bytes, 0, (int)bytes.Length);
				deflater.Close();
			}
		}
		
		public virtual void  writeBit(bool data)
		{
			writeBits(data?1:0, 1);
		}
		
		private void  writeBits(int data, int size)
		{
			//        if (print&&size>0) System.out.println("  write"+size+" "+data);
			while (size > 0)
			{
				if (size > bitPos)
				{
					//if more bits left to write than shift out what will fit
					currentByte |= (byte) ((uint)(data << (32 - size)) >> (32 - bitPos));
					
					// shift all the way left, then right to right
					// justify the data to be or'ed in
					binStream.Write((byte) currentByte);
					size -= bitPos;
					currentByte = 0;
					bitPos = 8;
				}
				// if (size <= bytePos)
				else
				{
					currentByte |= (byte) (((uint)data << (32 - size)) >> (32 - bitPos));
					bitPos -= size;
					size = 0;
					
					if (bitPos == 0)
					{
						//if current byte is filled
                        binStream.Write((byte)currentByte);
						currentByte = 0;
						bitPos = 8;
					}
				}
			}
		}
		
		public virtual void  writeUBits(int data, int size)
		{
			System.Diagnostics.Debug.Assert(data >= 0 && data <= (1 << size) - 1);
			writeBits(data, size);
		}
		
		public virtual void  writeSBits(int data, int size)
		{
			System.Diagnostics.Debug.Assert(data >= -(1 << (size - 1)) && data <= (1 << (size - 1)) - 1);
			writeBits(data, size);
		}
		
		public virtual void  flushBits()
		{
			if (bitPos != 8)
			{
                binStream.Write((byte)currentByte);
				currentByte = 0;
				bitPos = 8;
			}
		}
		
		public void reset()
		{
            binStream.BaseStream.Position = 0;
			compressPos = - 1;
		}
		
		public virtual void  writeUI8at(int pos, int value)
		{
			long oldPosition = binStream.BaseStream.Position;
            binStream.BaseStream.Position = pos;
			writeUI8(value);
            binStream.BaseStream.Position = oldPosition;
		}
		
		public virtual void  writeUI16at(int pos, int value)
		{
            long oldPosition = binStream.BaseStream.Position;
            binStream.BaseStream.Position = pos;
            writeUI16(value);
            binStream.BaseStream.Position = oldPosition;
        }
		
		public virtual void  writeSI16at(int pos, int value)
		{
            long oldPosition = binStream.BaseStream.Position;
            binStream.BaseStream.Position = pos;
            writeSI16(value);
            binStream.BaseStream.Position = oldPosition;
        }
		
		public virtual void  write32at(int pos, int value)
		{
            long oldPosition = binStream.BaseStream.Position;
            binStream.BaseStream.Position = pos;
            write32(value);
            binStream.BaseStream.Position = oldPosition;
        }
		
		public virtual void  writeString(String s)
		{
			try
			{
				System.Diagnostics.Debug.Assert(bitPos == 8);
				binStream.Write(swfVersion >= 6 ? System.Text.Encoding.UTF8.GetBytes(s) : System.Text.Encoding.Default.GetBytes(s));
				binStream.Write((byte)0);
			}
			catch (IOException)
			{
				System.Diagnostics.Debug.Assert(false);
			}
		}
		
		public virtual void  writeLengthString(String name)
		{
			try
			{
				System.Diagnostics.Debug.Assert(bitPos == 8);

                byte[] b = swfVersion >= 6 ? System.Text.Encoding.UTF8.GetBytes(name) : System.Text.Encoding.Default.GetBytes(name);
				
				// [paul] Flash Authoring and the player expect the String
				// to be null terminated.
				binStream.Write((byte)(b.Length + 1));
				binStream.Write(b);
				binStream.Write((byte)0);
			}
			catch (IOException)
			{
				System.Diagnostics.Debug.Assert(false);
			}
		}
		
		
		/// <summary>  Compares the absolute values of 4 signed integers and returns the unsigned magnitude of
		/// the number with the greatest absolute value.
		/// </summary>
		public static int maxNum(int a, int b, int c, int d)
		{
			//take the absolute values of the given numbers
			a = System.Math.Abs(a);
			b = System.Math.Abs(b);
			c = System.Math.Abs(c);
			d = System.Math.Abs(d);
			
			//compare the numbers and return the unsigned value of the one with the greatest magnitude
			return a > b?(a > c?(a > d?a:d):(c > d?c:d)):(b > c?(b > d?b:d):(c > d?c:d));
		}
		
		/// <summary>  Calculates the minimum number of bits necessary to represent the given number.  The
		/// number should be given in its unsigned form with the starting bits equal to 1 if it is
		/// signed.  Repeatedly compares number to another unsigned int called x.
		/// x is initialized to 1.  The value of x is shifted left i times until x is greater
		/// than number.  Now i is equal to the number of bits the UNSIGNED value of number needs.
		/// The signed value will need one more bit for the sign so i+1 is returned if the number
		/// is signed, and i is returned if the number is unsigned.
		/// </summary>
		/// <param name="number">the number to compute the size of
		/// </param>
		/// <param name="bits">1 if number is signed, 0 if unsigned
		/// </param>
		public static int minBits(int number, int bits)
		{
			int val = 1;
			for (int x = 1; val <= number; x <<= 1)
			{
				val = val | x;
				bits++;
			}
			
			if (bits > 32)
			{
				System.Diagnostics.Debug.Assert(false, "(minBits " + bits + " must not exceed 32)");
			}
			return bits;
		}
		
		public virtual void  writeAt(int offset, byte[] b)
		{
            long oldPosition = binStream.BaseStream.Position;
            binStream.BaseStream.Position = offset;
            binStream.Write(b);
            binStream.BaseStream.Position = oldPosition;
		}
	}
}
