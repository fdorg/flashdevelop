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
using System.IO;

namespace Flash.Swf
{
	
	/// <author>  Clement Wong
	/// </author>
	public sealed class SwfDecoder
	{
		private int bitBuf;
		private int bitPos;
		internal int swfVersion;
        BinaryReader binStream;
		
		/// <summary> create a decoder that reads directly from this byte array</summary>
		/// <param name="b">
		/// </param>
		/// <param name="swfVersion">
		/// </param>
		public SwfDecoder(byte[] b, int swfVersion)
		{
            MemoryStream memStream = new MemoryStream(b, false);

            binStream = new BinaryReader(memStream);

            this.swfVersion = swfVersion;
		}
		
		/// <summary> create a buffering decoder that reads from this unbuffered
		/// input stream.  Since SwfDecoder is a BufferedInputStream,
		/// it is not necessary to provide a BufferedInputStream for good
		/// performance.
		/// </summary>
		/// <param name="inStream">
		/// </param>
		/// <param name="swfVersion">
		/// </param>
		public SwfDecoder(Stream inStream, int swfVersion)
		{
            binStream = new BinaryReader(inStream);
			this.swfVersion = swfVersion;
		}
		
		public SwfDecoder(Stream inStream, int swfVersion, int offset)
		{
            binStream = new BinaryReader(inStream);
            binStream.BaseStream.Seek(offset, SeekOrigin.Begin);
		}

        public int Offset
        {
            get
            {
                return (int)binStream.BaseStream.Position;
            }
        }

		public void readFully(byte[] b)
		{
			int remain = b.Length;
			int off = 0;
			int count;
			while (remain > 0)
			{
				count = read(b, off, remain);
				if (count > 0)
				{
					off += count;
					remain -= count;
				}
				else
				{
					throw new SwfFormatException("couldn't read " + remain);
				}
			}
		}
		
		public byte read()
		{
			return binStream.ReadByte();
		}
		
		public int read(byte[] b, int off, int len)
		{
            return binStream.Read(b, off, len);
		}
		
		public long skip(long len)
		{
            return binStream.BaseStream.Seek(len, SeekOrigin.Current);
		}
		
		public float readFixed8()
		{
            return ((float)binStream.ReadUInt16()) / 256;
		}
		
		public int readUI8()
		{
            int result = -1;

            try
            {
                result = binStream.ReadByte();
            }
            catch (EndOfStreamException)
            {
            }

            return result;
		}
		
		public ushort readUI16()
		{
			syncBits();
            return binStream.ReadUInt16();
		}
		
		public uint readUI32()
		{
            return binStream.ReadUInt32();
		}
		
		public int readSI32()
		{
			syncBits();
            return binStream.ReadInt32();
		}
		
		public long read64()
		{
            return binStream.ReadInt64();
		}
		
		public bool readBit()
		{
			return readUBits(1) != 0;
		}
		
		public int readUBits(int numBits)
		{
			if (numBits == 0)
			{
				return 0;
			}
			
			int bitsLeft = numBits;
			int result = 0;
			
			if (bitPos == 0)
			//no value in the buffer - read a byte
			{
				bitBuf = readUI8();
				bitPos = 8;
			}
			
			while (true)
			{
				int shift = bitsLeft - bitPos;
				if (shift > 0)
				{
					// Consume the entire buffer
					result |= bitBuf << shift;
					bitsLeft -= bitPos;
					
					// Get the next byte from the input stream
					bitBuf = readUI8();
					bitPos = 8;
				}
				else
				{
					// Consume a portion of the buffer
					result |= bitBuf >> - shift;
					bitPos -= bitsLeft;
					bitBuf &= 0xff >> (8 - bitPos); // mask off the consumed bits
					
					//                if (print) System.out.println("  read"+numBits+" " + result);
					return result;
				}
			}
		}
		
		public int readSBits(int numBits)
		{
			if (numBits > 32)
			{
				throw new SwfFormatException("Number of bits > 32");
			}
			
			int num = readUBits(numBits);
			int shift = 32 - numBits;
			// sign extension
			num = (num << shift) >> shift;
			return num;
		}
		
		public short readSI16()
		{
			return binStream.ReadInt16();
		}
		
		public float readFloat()
		{
            return binStream.ReadSingle();
		}
		
		//private MemoryStream out_Renamed;
		
		public String readLengthString()
		{
			int length = readUI8();
			byte[] b = new byte[length];
			readFully(b);
			
			// [paul] Flash Authoring and the player null terminate the
			// string, so ignore the last byte when constructing the String.
			if (swfVersion >= 6)
			{
				return System.Text.Encoding.UTF8.GetString(b, 0, length - 1);
			}
			else
			{
				// use platform encoding
				return  System.Text.Encoding.Default.GetString(b, 0, length - 1);
			}
		}

		
		public String readString()
		{
			if (swfVersion >= 6)
			{
				return readUTF();
			}
			else
			{
				int ch;
                MemoryStream memStream = new MemoryStream();
 
				while ((ch = readUI8()) > 0)
				{
                    memStream.WriteByte((byte)ch);
				}

                // use platform encoding
				return System.Text.Encoding.Default.GetString(memStream.ToArray(), 0, (int)memStream.Length);
			}
		}
		
		private String readUTF()
		{
			System.Text.StringBuilder b = new System.Text.StringBuilder();
			int c, c2, c3;
			
			while ((c = readUI8()) > 0)
			{
				switch (c >> 4)
				{
					
					case 0: 
					case 1: 
					case 2: 
					case 3: 
					case 4: 
					case 5: 
					case 6: 
					case 7: 
						/* 0xxxxxxx*/
						b.Append((char) c);
						break;
					
					
					case 12: 
					case 13: 
						/* 110x xxxx   10xx xxxx*/
						c2 = readUI8();
						if (c2 <= 0 || (c2 & 0xC0) != 0x80)
							throw new IOException();
						b.Append((char) ((c & 0x1F) << 6 | c2 & 0x3F));
						break;
					
					
					case 14: 
						/* 1110 xxxx  10xx xxxx  10xx xxxx */
						c2 = readUI8();
						c3 = readUI8();
						if (c2 <= 0 || c3 <= 0 || ((c2 & 0xC0) != 0x80) || ((c3 & 0xC0) != 0x80))
							throw new IOException();
						b.Append((char) ((c & 0x0F) << 12 | (c2 & 0x3F) << 6 | c3 & 0x3F));
						break;
					
					
					default: 
						/* 10xx xxxx,  1111 xxxx */
						throw new IOException();
					
				}
			}
			return b.ToString();
		}
		
		
		public void syncBits()
		{
			bitPos = 0;
		}
		
		private long markOffset = 0;
		
		public void  mark(int readlimit)
		{
			markOffset = binStream.BaseStream.Position;
		}
		
		public void  reset()
		{
            binStream.BaseStream.Seek(markOffset, SeekOrigin.Begin);
		}
	}
}
