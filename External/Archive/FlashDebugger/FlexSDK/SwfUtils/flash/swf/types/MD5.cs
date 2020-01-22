// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
// Md5.java
// $Id: Md5.java,v 1.5 2000/08/16 21:37:48 ylafon Exp $
// (c) COPYRIGHT MIT and INRIA, 1996.
// Please first read the full copyright statement in file COPYRIGHT.html
using System;
namespace Flash.Swf.Types
{
	
	public class MD5
	{
		
		private const int S11 = 7;
		private const int S12 = 12;
		private const int S13 = 17;
		private const int S14 = 22;
		private const int S21 = 5;
		private const int S22 = 9;
		private const int S23 = 14;
		private const int S24 = 20;
		private const int S31 = 4;
		private const int S32 = 11;
		private const int S33 = 16;
		private const int S34 = 23;
		private const int S41 = 6;
		private const int S42 = 10;
		private const int S43 = 15;
		private const int S44 = 21;
		
		private static byte[] padding = new byte[]{(byte) 0x80, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0};
		
		private int[] state = null;
		private long count = 0;
		private byte[] buffer = null;
		private byte[] digest = null;
		
		public static String stringify(byte[] buf)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(2 * buf.Length);
			for (int i = 0; i < buf.Length; i++)
			{
				int h = (buf[i] & 0xf0) >> 4;
				int l = (buf[i] & 0x0f);
				sb.Append((char) ((h > 9)?'A' + h - 10:'0' + h));
				sb.Append((char) ((l > 9)?'A' + l - 10:'0' + l));
			}
			return sb.ToString();
		}
		
		private int rotate_left(int x, int n)
		{
			return (int)(((uint)x << n) | ((uint)x >> (32 - n)));
		}
		
		private int FF(int a, int b, int c, int d, int x, int s, int ac)
		{
			a += (((b & c) | ((~ b) & d)) + x + ac);
			a = rotate_left(a, s);
			a += b;
			return a;
		}
		
		private int GG(int a, int b, int c, int d, int x, int s, int ac)
		{
			a += (((b & d) | (c & (~ d))) + x + ac);
			a = rotate_left(a, s);
			a += b;
			return a;
		}
		
		private int HH(int a, int b, int c, int d, int x, int s, int ac)
		{
			a += ((b ^ c ^ d) + x + ac);
			a = rotate_left(a, s);
			a += b;
			return a;
		}
		
		private int II(int a, int b, int c, int d, int x, int s, int ac)
		{
			a += ((c ^ (b | (~ d))) + x + ac);
			a = rotate_left(a, s);
			a += b;
			return a;
		}
		
		private void  transform(byte[] block, int offset)
		{
			int a = state[0];
			int b = state[1];
			int c = state[2];
			int d = state[3];
			int[] x = new int[16];
			
			int i = 0;
			int j = 0;
			for (; j < 64; i++, j += 4)
			{
				x[i] = ((block[offset + j] & 0xff) | ((block[offset + j + 1] & 0xff) << 8) | ((block[offset + j + 2] & 0xff) << 16) | ((block[offset + j + 3] & 0xff) << 24));
			}
			
			/* Round 1 */
			a = FF(a, b, c, d, x[0], S11, unchecked((int) 0xd76aa478)); /* 1 */
			d = FF(d, a, b, c, x[1], S12, unchecked((int) 0xe8c7b756)); /* 2 */
			c = FF(c, d, a, b, x[2], S13, 0x242070db); /* 3 */
			b = FF(b, c, d, a, x[3], S14, unchecked((int) 0xc1bdceee)); /* 4 */
			a = FF(a, b, c, d, x[4], S11, unchecked((int) 0xf57c0faf)); /* 5 */
			d = FF(d, a, b, c, x[5], S12, 0x4787c62a); /* 6 */
			c = FF(c, d, a, b, x[6], S13, unchecked((int) 0xa8304613)); /* 7 */
			b = FF(b, c, d, a, x[7], S14, unchecked((int) 0xfd469501)); /* 8 */
			a = FF(a, b, c, d, x[8], S11, 0x698098d8); /* 9 */
			d = FF(d, a, b, c, x[9], S12, unchecked((int) 0x8b44f7af)); /* 10 */
			c = FF(c, d, a, b, x[10], S13, unchecked((int) 0xffff5bb1)); /* 11 */
			b = FF(b, c, d, a, x[11], S14, unchecked((int) 0x895cd7be)); /* 12 */
			a = FF(a, b, c, d, x[12], S11, 0x6b901122); /* 13 */
			d = FF(d, a, b, c, x[13], S12, unchecked((int) 0xfd987193)); /* 14 */
			c = FF(c, d, a, b, x[14], S13, unchecked((int) 0xa679438e)); /* 15 */
			b = FF(b, c, d, a, x[15], S14, 0x49b40821); /* 16 */
			/* Round 2 */
			a = GG(a, b, c, d, x[1], S21, unchecked((int) 0xf61e2562)); /* 17 */
			d = GG(d, a, b, c, x[6], S22, unchecked((int) 0xc040b340)); /* 18 */
			c = GG(c, d, a, b, x[11], S23, 0x265e5a51); /* 19 */
			b = GG(b, c, d, a, x[0], S24, unchecked((int) 0xe9b6c7aa)); /* 20 */
			a = GG(a, b, c, d, x[5], S21, unchecked((int) 0xd62f105d)); /* 21 */
			d = GG(d, a, b, c, x[10], S22, 0x2441453); /* 22 */
			c = GG(c, d, a, b, x[15], S23, unchecked((int) 0xd8a1e681)); /* 23 */
			b = GG(b, c, d, a, x[4], S24, unchecked((int) 0xe7d3fbc8)); /* 24 */
			a = GG(a, b, c, d, x[9], S21, 0x21e1cde6); /* 25 */
			d = GG(d, a, b, c, x[14], S22, unchecked((int) 0xc33707d6)); /* 26 */
			c = GG(c, d, a, b, x[3], S23, unchecked((int) 0xf4d50d87)); /* 27 */
			b = GG(b, c, d, a, x[8], S24, 0x455a14ed); /* 28 */
			a = GG(a, b, c, d, x[13], S21, unchecked((int) 0xa9e3e905)); /* 29 */
			d = GG(d, a, b, c, x[2], S22, unchecked((int) 0xfcefa3f8)); /* 30 */
			c = GG(c, d, a, b, x[7], S23, 0x676f02d9); /* 31 */
			b = GG(b, c, d, a, x[12], S24, unchecked((int) 0x8d2a4c8a)); /* 32 */
			
			/* Round 3 */
			a = HH(a, b, c, d, x[5], S31, unchecked((int) 0xfffa3942)); /* 33 */
			d = HH(d, a, b, c, x[8], S32, unchecked((int) 0x8771f681)); /* 34 */
			c = HH(c, d, a, b, x[11], S33, 0x6d9d6122); /* 35 */
			b = HH(b, c, d, a, x[14], S34, unchecked((int) 0xfde5380c)); /* 36 */
			a = HH(a, b, c, d, x[1], S31, unchecked((int) 0xa4beea44)); /* 37 */
			d = HH(d, a, b, c, x[4], S32, 0x4bdecfa9); /* 38 */
			c = HH(c, d, a, b, x[7], S33, unchecked((int) 0xf6bb4b60)); /* 39 */
			b = HH(b, c, d, a, x[10], S34, unchecked((int) 0xbebfbc70)); /* 40 */
			a = HH(a, b, c, d, x[13], S31, 0x289b7ec6); /* 41 */
			d = HH(d, a, b, c, x[0], S32, unchecked((int) 0xeaa127fa)); /* 42 */
			c = HH(c, d, a, b, x[3], S33, unchecked((int) 0xd4ef3085)); /* 43 */
			b = HH(b, c, d, a, x[6], S34, 0x4881d05); /* 44 */
			a = HH(a, b, c, d, x[9], S31, unchecked((int) 0xd9d4d039)); /* 45 */
			d = HH(d, a, b, c, x[12], S32, unchecked((int) 0xe6db99e5)); /* 46 */
			c = HH(c, d, a, b, x[15], S33, 0x1fa27cf8); /* 47 */
			b = HH(b, c, d, a, x[2], S34, unchecked((int) 0xc4ac5665)); /* 48 */
			
			/* Round 4 */
			a = II(a, b, c, d, x[0], S41, unchecked((int) 0xf4292244)); /* 49 */
			d = II(d, a, b, c, x[7], S42, 0x432aff97); /* 50 */
			c = II(c, d, a, b, x[14], S43, unchecked((int) 0xab9423a7)); /* 51 */
			b = II(b, c, d, a, x[5], S44, unchecked((int) 0xfc93a039)); /* 52 */
			a = II(a, b, c, d, x[12], S41, 0x655b59c3); /* 53 */
			d = II(d, a, b, c, x[3], S42, unchecked((int) 0x8f0ccc92)); /* 54 */
			c = II(c, d, a, b, x[10], S43, unchecked((int) 0xffeff47d)); /* 55 */
			b = II(b, c, d, a, x[1], S44, unchecked((int) 0x85845dd1)); /* 56 */
			a = II(a, b, c, d, x[8], S41, 0x6fa87e4f); /* 57 */
			d = II(d, a, b, c, x[15], S42, unchecked((int) 0xfe2ce6e0)); /* 58 */
			c = II(c, d, a, b, x[6], S43, unchecked((int) 0xa3014314)); /* 59 */
			b = II(b, c, d, a, x[13], S44, 0x4e0811a1); /* 60 */
			a = II(a, b, c, d, x[4], S41, unchecked((int) 0xf7537e82)); /* 61 */
			d = II(d, a, b, c, x[11], S42, unchecked((int) 0xbd3af235)); /* 62 */
			c = II(c, d, a, b, x[2], S43, 0x2ad7d2bb); /* 63 */
			b = II(b, c, d, a, x[9], S44, unchecked((int) 0xeb86d391)); /* 64 */
			
			state[0] += a;
			state[1] += b;
			state[2] += c;
			state[3] += d;
		}
		
		private void  update(byte[] input, int len)
		{
			int index = ((int) (count >> 3)) & 0x3f;
			count += (len << 3);
			int partLen = 64 - index;
			int i = 0;
			if (len >= partLen)
			{
				Array.Copy(input, 0, buffer, index, partLen);
				transform(buffer, 0);
				for (i = partLen; i + 63 < len; i += 64)
					transform(input, i);
				index = 0;
			}
			else
			{
				i = 0;
			}
			Array.Copy(input, i, buffer, index, len - i);
		}
		
		private byte[] end()
		{
			byte[] bits = new byte[8];
			for (int i = 0; i < 8; i++)
				bits[i] = (byte) (((uint)count >> (i * 8)) & 0xff);
			int index = ((int) (count >> 3)) & 0x3f;
			int padlen = (index < 56)?(56 - index):(120 - index);
			update(padding, padlen);
			update(bits, 8);
			return encode(state, 16);
		}
		
		// Encode the content.state array into 16 bytes array
		private byte[] encode(int[] input, int len)
		{
			byte[] output = new byte[len];
			int i = 0;
			int j = 0;
			for (; j < len; i++, j += 4)
			{
				output[j] = (byte) ((input[i]) & 0xff);
				output[j + 1] = (byte) ((input[i] >> 8) & 0xff);
				output[j + 2] = (byte) ((input[i] >> 16) & 0xff);
				output[j + 3] = (byte) ((input[i] >> 24) & 0xff);
			}
			return output;
		}
		
		/// <summary> Construct a digestifier for the given data</summary>
		public MD5(byte[] bytes, int len)
		{
			this.state = new int[4];
			this.buffer = new byte[64];
			this.count = 0;
			state[0] = 0x67452301;
			state[1] = unchecked((int) 0xefcdab89);
			state[2] = unchecked((int) 0x98badcfe);
			state[3] = 0x10325476;
			update(bytes, len);
			this.digest = end();
		}
		
		public static byte[] getDigest(byte[] b, int len)
		{
			MD5 md5 = new MD5(b, len);
			return md5.digest;
		}
	}
}
