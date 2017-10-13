using System;
using System.Runtime.InteropServices;
using BubblePony.Alloc;

namespace LIBMIO0
{
	[StructLayout(LayoutKind.Sequential,Size =16)]
    public struct Header : IEquatable<Header>
    {
		public uint MagicNumber;
        public uint UncompressedSize;
        public uint CompressedOffset;
        public uint UncompressedOffset;
		public static bool operator ==(Header L, Header R)
		{
			return L.UncompressedSize == R.UncompressedSize &&
				L.CompressedOffset == R.CompressedOffset &&
				L.UncompressedOffset == R.UncompressedOffset &&
				L.MagicNumber == R.MagicNumber;
		}
		public static bool operator !=(Header L, Header R)
		{
			return L.UncompressedSize != R.UncompressedSize ||
				L.CompressedOffset != R.CompressedOffset ||
				L.UncompressedOffset != R.UncompressedOffset ||
				L.MagicNumber != R.MagicNumber;
		}
		public override string ToString()
		{
			return string.Format("{{\"uncompressed\":{1},\"offsets\":[{2},{3}],\"magic\":{0}}}", MagicNumber, UncompressedSize, CompressedOffset, UncompressedOffset);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				return (int)(((UncompressedSize + CompressedOffset) - UncompressedSize) ^ MagicNumber);
			}
		}
		public override bool Equals(object obj)
		{
			return obj is Header && Equals((Header)obj);
		}
		public bool Equals(Header other) { return MagicNumber == other.MagicNumber && UncompressedOffset == other.UncompressedOffset && UncompressedSize == other.UncompressedSize && CompressedOffset == other.CompressedOffset; }

		public const byte HeaderSizeInBytes = 16;
		public const uint BigEndian = 0x4D494F30;
		public uint SwitchOffset => HeaderSizeInBytes;
		internal static uint AtLeast16(uint Value) { return Value < 16u ? 16u : Value; }
		public bool IsBigEndian => MagicNumber == BigEndian;
		public uint MostOffset => AtLeast16((CompressedOffset > UncompressedOffset ? CompressedOffset : UncompressedOffset));

		private static unsafe uint read_u32_be(byte* buf, int off)
		{
			return (uint)(((buf)[off + 0] << 24) + ((buf)[off + 1] << 16) +
				((buf)[off + 2] << 8) + ((buf)[off + 3]));
		}
		private static uint read_u32_be(byte[] buf, int off)
		{
			return (uint)(((buf)[off + 0] << 24) + ((buf)[off + 1] << 16) +
				((buf)[off + 2] << 8) + ((buf)[off + 3]));
		}


		private static unsafe uint read_u32_le(byte* buf, int off)
		{
			return (uint)(((buf)[off + 1] << 24) + ((buf)[off + 0] << 16) +
				((buf)[off + 3] << 8) + ((buf)[off + 2]));
		}
		private static uint read_u32_le(byte[] buf, int off)
		{
			return (uint)(((buf)[off + 1] << 24) + ((buf)[off + 0] << 16) +
				((buf)[off + 3] << 8) + ((buf)[off + 2]));
		}

		private unsafe static void write_u32_be(byte* buf, uint val, int off)
        {
            buf[(int)off + 0] = (byte)((val >> 24) & 0xFF);
            buf[(int)off + 1] = (byte)((val >> 16) & 0xFF);
            buf[(int)off + 2] = (byte)((val >> 8) & 0xFF);
            buf[(int)off + 3] = (byte)(val & 0xFF);
        }
		///<summary>
		/// decode MIO0 header<para/>
		/// returns true if valid header, false otherwise
		///</summary>
		public static unsafe bool Read(ByteSegment buf, out Header head)
		{
			if (buf.Length >= Header.HeaderSizeInBytes)
				return Read(buf.Byte, out head);
			else
			{
				head = default(Header);
				return false;
			}
		}
		///<summary>
		/// decode MIO0 header<para/>
		/// returns true if valid header, false otherwise
		///</summary>
		public static bool Read(byte[] buf, out Header head)
		{
			if (null == (object)buf || buf.Length < Header.HeaderSizeInBytes)
			{
				head = default(Header);
				return false;
			}
			else
			{
				head.MagicNumber = read_u32_be(buf, 0);
				head.UncompressedSize = read_u32_be(buf, 4);
				head.CompressedOffset = read_u32_be(buf, 8);
				head.UncompressedOffset = read_u32_be(buf, 12);
				return head.MagicNumber == Header.BigEndian;
			}
		}
		///<summary>
		/// decode MIO0 header<para/>
		/// returns true if valid header, false otherwise
		///</summary>
		public static bool Read(byte[] buf, out Header head, uint offset)
		{
			if (null == (object)buf || offset >= buf.Length || (buf.Length-offset) < Header.HeaderSizeInBytes)
			{
				head = default(Header);
				return false;
			}
			else
			{
				head.MagicNumber = read_u32_be(buf, (int)offset);
				head.UncompressedSize = read_u32_be(buf, (int)offset +4);
				head.CompressedOffset = read_u32_be(buf, (int)offset +8);
				head.UncompressedOffset = read_u32_be(buf, (int)offset +12);
				return head.MagicNumber == Header.BigEndian;
			}
		}
		///<summary>
		/// decode MIO0 header<para/>
		/// returns true if valid header, false otherwise
		///</summary>
		public static unsafe bool Read(byte *buf, out Header head)
		{
			if (null == buf)
			{
				head = default(Header);
				return false;
			}
			else
			{
				head.MagicNumber = read_u32_be(buf, 0);
				head.UncompressedSize = read_u32_be(buf, 4);
				head.CompressedOffset = read_u32_be(buf, 8);
				head.UncompressedOffset = read_u32_be(buf, 12);
				return head.MagicNumber == Header.BigEndian;
			}
		}
		
		private static unsafe void Inflate(
			byte * bit_buf,
			byte * comp_buf,
			byte * uncomp_buf,
			byte * dst,
			byte * dst_end)
		{
			ushort idx;
			byte len;
			for(sbyte bit_pos = 8; dst < dst_end;
				bit_pos = bit_pos == 0 ? (sbyte)8 : bit_pos,
				bit_buf += (bit_pos>>3))
				if (0 == ((*bit_buf) & (byte)(1u << (--bit_pos))))
				{
					len = *comp_buf++;
					idx = (ushort)(((len & 15u) << 8) + (*comp_buf++) + 1u);
					for (len = (byte)((len >> 4) + 3u); 0 != len; ++dst, --len)
						*dst = *(dst - idx);
				}
				else
					*dst++ = *uncomp_buf++;
		}

		private unsafe static void Simulate(
			ref byte* bit_buf,
			ref byte* comp_buf,
			ref byte* uncomp_buf,
			uint dst_size)
		{
			for (sbyte bit_pos = 8; 0 != dst_size;
				bit_pos = bit_pos == 0 ? (sbyte)8 : bit_pos,
				bit_buf += (bit_pos >> 3))
				if (0 == ((*bit_buf) & (byte)(1u << (--bit_pos))))
				{
					dst_size -= (((uint)(*comp_buf) >> 4) + 3u);
					comp_buf += 2;
				}
				else
				{
					uncomp_buf++;
					--dst_size;
				}
		}

		private static unsafe byte* End(
			byte* bit_buf,
			byte* comp_buf,
			byte* uncomp_buf,
			uint dest_size)
		{
			Simulate(ref bit_buf, ref comp_buf, ref uncomp_buf, dest_size);
			if (comp_buf < bit_buf)
				comp_buf = bit_buf;
			return comp_buf > uncomp_buf ? comp_buf : uncomp_buf;
		}
		
		///<summary>
		/// decode MIO0 data<para/>
		/// mio0_buf: buffer containing MIO0 data<para/>
		/// returns the raw data as a byte array or null.
		///</summary>
		public unsafe static byte[] Inflate(ByteSegment mio0_buf)
		{
			byte[] dst=null;
			if (Read(mio0_buf, out Header head))
				if(mio0_buf.Length <= head.MostOffset) {
					Console.Error.WriteLine("mio0_buf does not contain the count of bytes required by its own header");
				}
				else
				{
					dst = new byte[(int)head.UncompressedSize];
					fixed (byte* dst_buf = dst)
						Inflate(
							mio0_buf.Byte + Header.HeaderSizeInBytes,
							mio0_buf.Byte + head.CompressedOffset,
							mio0_buf.Byte + head.UncompressedOffset,
							dst_buf,
							dst_buf + head.UncompressedSize);
				}
			return dst;
		}
		///<summary>
		/// decode MIO0 data<para/>
		/// mio0_buf: buffer containing MIO0 data<para/>
		/// returns the raw data as a byte array or null.
		///</summary>
		public unsafe static byte[] Inflate(byte[] mio0_buf, uint offset = 0)
		{
			byte[] dst = null;
			if (Read(mio0_buf, out Header head))
				if (mio0_buf.Length - (int)offset <= head.MostOffset)
				{
					Console.Error.WriteLine("mio0_buf does not contain the count of bytes required by its own header");
				}
				else
				{
					dst = new byte[(int)head.UncompressedSize];
					fixed (byte* buf = &mio0_buf[(int)offset])
					fixed (byte* dst_buf = dst)
						Inflate(
							buf + Header.HeaderSizeInBytes,
							buf + head.CompressedOffset,
							buf + head.UncompressedOffset,
							dst_buf,
							dst_buf + head.UncompressedSize);
				}
			return dst;
		}
		///<summary>
		/// determines the actual size of the compressed mio0 data without inflating.
		/// returns the length from the start of mio0_buf to the end (past last) or 0 when arguments are bad.
		///</summary>
		public unsafe static uint Measure(ByteSegment mio0_buf, out Header head)
		{
			uint length = 0;
			if (Read(mio0_buf, out head))
				if (mio0_buf.Length <= head.MostOffset)
				{
					Console.Error.WriteLine("mio0_buf does not contain the count of bytes required by its own header");
				}
				else
				{
					length = (uint)(End(
						mio0_buf.Byte + Header.HeaderSizeInBytes,
						mio0_buf.Byte + head.CompressedOffset,
						mio0_buf.Byte + head.UncompressedOffset,
						head.UncompressedSize)-mio0_buf.Byte);
				}
			return length;
		}
		///<summary>
		/// determines the actual size of the compressed mio0 data without inflating.
		/// returns the length from offset (start=0) of mio0_buf to the end (past last) or 0 when arguments are bad.
		///</summary>
		public unsafe static uint Measure(byte[] mio0_buf, out Header head, uint offset = 0)
		{
			uint length = 0;
			if (Read(mio0_buf, out head, offset))
				if (mio0_buf.Length - (int)offset <= head.MostOffset)
				{
					Console.Error.WriteLine("mio0_buf does not contain the count of bytes required by its own header");
				}
				else
				{
					fixed (byte* buf = &mio0_buf[(int)offset])
						length = (uint)(End(
							buf + Header.HeaderSizeInBytes,
							buf + head.CompressedOffset,
							buf + head.UncompressedOffset,
							head.UncompressedSize) - buf);
				}
			return length;
		}
		///<summary>
		/// determines the actual size of the compressed mio0 data without inflating.
		/// returns the length from the start of mio0_buf to the end (past last) or 0 when arguments are bad.
		///</summary>
		public unsafe static uint Measure(byte* mio0_buf, out Header head)
		{
			uint length = 0;
			if (Read(mio0_buf, out head))
				length = (uint)(End(
					mio0_buf + Header.HeaderSizeInBytes,
					mio0_buf + head.CompressedOffset,
					mio0_buf + head.UncompressedOffset,
					head.UncompressedSize) - mio0_buf);

			return length;
		}
		///<summary>
		/// decode MIO0 data<para/>
		/// mio0_buf: buffer containing MIO0 data<para/>
		/// returns the raw data as an Allocation or null.
		///</summary>
		public unsafe static Allocation InflateToNewAllocation(ByteSegment mio0_buf)
		{
			Allocation dst = null;
			if (Read(mio0_buf, out Header head))
				if (mio0_buf.Length <= head.MostOffset)
				{
					Console.Error.WriteLine("mio0_buf does not contain the count of bytes required by its own header");
				}
				else
				{
					dst = new Allocation(head.UncompressedSize);
					Inflate(
						mio0_buf.Byte + Header.HeaderSizeInBytes,
						mio0_buf.Byte + head.CompressedOffset,
						mio0_buf.Byte + head.UncompressedOffset,
						dst.Byte,
						dst.Byte + head.UncompressedSize);
				}
			return dst;
		}
		///<summary>
		/// decode MIO0 data<para/>
		/// mio0_buf: buffer containing MIO0 data<para/>
		/// returns the raw data as an Allocation or null.
		///</summary>
		public unsafe static Allocation InflateToNewAllocation(byte[] mio0_buf, uint offset = 0)
		{
			Allocation dst = null;
			if (Read(mio0_buf, out Header head))
				if (mio0_buf.Length - (int)offset <= head.MostOffset)
				{
					Console.Error.WriteLine("mio0_buf does not contain the count of bytes required by its own header");
				}
				else
				{
					dst = new Allocation(head.UncompressedSize);
					fixed (byte* buf = &mio0_buf[(int)offset])
						Inflate(
							buf + Header.HeaderSizeInBytes,
							buf + head.CompressedOffset,
							buf + head.UncompressedOffset,
							dst.Byte,
							dst.Byte + head.UncompressedSize);
				}
			return dst;
		}
		const uint K = 4096u;
		const uint Z = 0;
		unsafe static void FL0(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				0u <= (s = (p - o)))for (s = 
				0u; c < 
				0u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				0u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		unsafe static void FL1(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				1u <= (s = (p - o)))for (s = 
				1u; c < 
				1u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				1u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		unsafe static void FL2(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				2u <= (s = (p - o)))for (s = 
				2u; c < 
				2u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				2u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		unsafe static void FL3(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				3u <= (s = (p - o)))for (s = 
				3u; c < 
				3u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				3u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		unsafe static void FL4(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				4u <= (s = (p - o)))for (s = 
				4u; c < 
				4u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				4u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		unsafe static void FL5(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				5u <= (s = (p - o)))for (s = 
				5u; c < 
				5u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				5u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		unsafe static void FL6(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				6u <= (s = (p - o)))for (s = 
				6u; c < 
				6u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				6u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		unsafe static void FL7(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				7u <= (s = (p - o)))for (s = 
				7u; c < 
				7u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				7u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		unsafe static void FL8(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				8u <= (s = (p - o)))for (s = 
				8u; c < 
				8u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				8u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		unsafe static void FL9(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				9u <= (s = (p - o)))for (s = 
				9u; c < 
				9u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				9u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		unsafe static void FL10(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				10u <= (s = (p - o)))for (s = 
				10u; c < 
				10u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				10u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		unsafe static void FL11(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				11u <= (s = (p - o)))for (s = 
				11u; c < 
				11u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				11u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		unsafe static void FL12(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				12u <= (s = (p - o)))for (s = 
				12u; c < 
				12u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				12u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		unsafe static void FL13(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				13u <= (s = (p - o)))for (s = 
				13u; c < 
				13u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				13u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		unsafe static void FL14(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				14u <= (s = (p - o)))for (s = 
				14u; c < 
				14u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				14u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		unsafe static void FL15(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				15u <= (s = (p - o)))for (s = 
				15u; c < 
				15u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				15u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		unsafe static void FL16(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				16u <= (s = (p - o)))for (s = 
				16u; c < 
				16u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				16u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		unsafe static void FL17(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				17u <= (s = (p - o)))for (s = 
				17u; c < 
				17u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				17u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		unsafe static void FL18(
			byte* X,
			out uint L,
			out uint F,
			uint p){uint B = Z, b = Z, c,s,o,i,j;for (o = (p > K ? (p - K) : Z), c = Z; o < p;++o, c = Z){if (
				18u <= (s = (p - o)))for (s = 
				18u; c < 
				18u && X[p + c] == X[o + c]; ++c) ;else{for (; c < s && X[p + c] == X[o + c]; ++c) ;if (s == c) { for (s = 
				18u - c, j = p + c, i = Z; i < s && X[j + i] == X[o + i]; ++i) ; c += i; }}if (c <= B) continue;b = p - o; B = c;}F = b;L = B;
		}
		private unsafe static void FindLongest(
			byte* buf,
			out uint length,
			out uint found,
			uint buf_offset, 
			uint max_search)
		{
			if (max_search >= 18u)
				FL18(buf, out length, out found, buf_offset);
			else if (0u == (max_search & 16u))
				if (0u == (max_search & 8u))
					if (0u == (max_search & 4u))
						if (0u == (max_search & 2u))
							if (0u == (max_search & 1u))
								FL0(buf, out length, out found, buf_offset);
							else
								FL1(buf, out length, out found, buf_offset);
						else if (0u == (max_search & 1u))
							FL2(buf, out length, out found, buf_offset);
						else
							FL3(buf, out length, out found, buf_offset);
					else if (0u == (max_search & 2u))
						if (0u == (max_search & 1u))
							FL4(buf, out length, out found, buf_offset);
						else
							FL5(buf, out length, out found, buf_offset);
					else if (0u == (max_search & 1u))
						FL6(buf, out length, out found, buf_offset);
					else
						FL7(buf, out length, out found, buf_offset);
				else if (0u == (max_search & 4u))
					if (0u == (max_search & 2u))
						if (0u == (max_search & 1u))
							FL8(buf, out length, out found, buf_offset);
						else
							FL9(buf, out length, out found, buf_offset);
					else if (0 == (max_search & 1u))
						FL10(buf, out length, out found, buf_offset);
					else
						FL11(buf, out length, out found, buf_offset);
				else if (0u == (max_search & 2u))
					if (0u == (max_search & 1u))
						FL12(buf, out length, out found, buf_offset);
					else
						FL13(buf, out length, out found, buf_offset);
				else if (0u == (max_search & 1u))
					FL14(buf, out length, out found, buf_offset);
				else
					FL15(buf, out length, out found, buf_offset);
			else if (16u == max_search)
				FL16(buf, out length, out found, buf_offset);
			else
				FL17(buf, out length, out found, buf_offset);
		}

		private struct Sequence
		{
			unsafe public byte* pos, start, end;
			unsafe public void Offset(byte* ptrv)
			{
				start = ptrv + ((uint)(start - (byte*)null));
				end = ptrv+ ((uint)(end - (byte*)null));
				pos = ptrv+ ((uint)(pos - (byte*)null));
			}
		}

		private struct Deflator
		{
			public Sequence buf;
			public Sequence comp;
			public Sequence uncomp;
			public Sequence bit;

			unsafe public Header header
			{
				get
				{
					Header head;
					head.MagicNumber = Header.BigEndian;
					head.UncompressedSize = (uint)(buf.end - buf.start);
					head.CompressedOffset = (((uint)(bit.pos - bit.start) + 3u) & ~3u);
					head.UncompressedOffset = head.CompressedOffset + ((uint)(comp.pos - comp.start));
					return head;
				}
			}

			public unsafe uint alloc_size => 16u + (uint)(uncomp.end - bit.start);

			unsafe public void Offset(byte* ptrv)
			{
				comp.Offset(ptrv);
				uncomp.Offset(ptrv);
				bit.Offset(ptrv);
			}
			unsafe public void Offset(IntPtr ptr)
			{
				var ptrv = (byte*)ptr.ToPointer();
				comp.Offset(ptrv);
				uncomp.Offset(ptrv);
				bit.Offset(ptrv);
			}

			unsafe public void Deflate()
			{
				uint longest_match,
					offset,
					lookahead_match,
					lookahead_offset;

				sbyte bit_counter;
				// encode data
				// special case for first byte
				for (*uncomp.pos++ = *buf.pos++,
					*bit.pos = 128,
					bit_counter = 7;
					buf.pos < buf.end;)
				{
					FindLongest(
						buf.start,
						out longest_match,
						out offset,
						(uint)(buf.pos - buf.start),
						(uint)(buf.end - buf.pos));

					if (longest_match > 2u)
					{
						FindLongest(
							buf.start,
							out lookahead_match,
							out lookahead_offset,
							(uint)(buf.pos - buf.start) + 1u,
							(uint)(buf.end - buf.pos) - 1u);

						// better match found, use uncompressed + lookahead compressed
						if ((longest_match + 1u) < lookahead_match)
						{
							// uncompressed byte
							*uncomp.pos++ = *buf.pos++;
							*bit.pos |= (byte)(1u << (--bit_counter));
							*comp.pos++ = (byte)((((lookahead_match - 3u) & 0x0Fu) << 4) |
											  (((--lookahead_offset) >> 8) & 0x0Fu));
							*comp.pos++ = (byte)((lookahead_offset) & 0xFFu);
							if (0 == bit_counter) { bit_counter = 8; bit.pos++; }
							buf.pos += (uint)lookahead_match;
						}
						else
						{
							*comp.pos++ = (byte)((((longest_match - 3u) & 0x0Fu) << 4) |
											  (((--offset) >> 8) & 0x0Fu));
							*comp.pos++ = (byte)((offset) & 0xFFu);
							buf.pos += (uint)longest_match;
						}

						*bit.pos &= (byte)((~(1u << (--bit_counter))) & 0xFFu);
					}
					else
					{
						// uncompressed byte
						*uncomp.pos++ = *buf.pos++;
						*bit.pos |= (byte)(1u << (--bit_counter));
					}
					if (0 == bit_counter) {
						bit_counter = 8;
						bit.pos++;
					}
				}
				// zero out any remaining bits.
				if (bit_counter != 8) *bit.pos++ &= (byte)((~((1u << bit_counter) - 1u)) & 0xFFu);
			}

			unsafe public void Simulate()
			{
				uint longest_match,
					offset,
					lookahead_match,
					lookahead_offset;

				sbyte bit_counter;
				// encode data
				// special case for first byte
				for (*uncomp.pos++ = *buf.pos++,
					*bit.pos = 128,
					bit_counter = 7;
					buf.pos < buf.end;)
				{
					FindLongest(
						buf.start,
						out longest_match,
						out offset,
						(uint)(buf.pos - buf.start),
						(uint)(buf.end - buf.pos));

					if (longest_match > 2u)
					{
						FindLongest(
							buf.start,
							out lookahead_match,
							out lookahead_offset,
							(uint)(buf.pos - buf.start) + 1u,
							(uint)(buf.end - buf.pos) - 1u);
						comp.pos += 2;
						// better match found, use uncompressed + lookahead compressed
						if ((longest_match + 1u) < lookahead_match)
						{
							// uncompressed byte
							uncomp.pos++;
							if (0 == --bit_counter) {
								bit_counter = 8;
								bit.pos++;
							}
							buf.pos += 1u+(uint)lookahead_match;
						}
						else
						{
							buf.pos += (uint)longest_match;
						}
					}
					else
					{
						// uncompressed byte
						uncomp.pos++;
						buf.pos++;
					}
					if (0 == --bit_counter)
					{
						bit_counter = 8;
						bit.pos++;
					}
				}
				// zero out any remaining bits.
				if (bit_counter != 8) bit.pos++;
			}

			unsafe public Deflator(
				byte* buf_start,
				byte* buf_end)
			{
				this.buf.pos = buf_start;
				this.buf.start = buf_start;
				this.buf.end = buf_end;

				var buf_length = (uint)(buf_end-buf_start);
				var comp_uncomp_seg_len = ((buf_length + 7u)) & ~7u;
				bit.pos=bit.start = ((byte*)null+16);
				bit.end = bit.start + ((buf_length + 7u) >> 3);
				comp.pos = comp.start = bit.start + (((uint)(bit.end - bit.start) + 7u) & ~7u);
				uncomp.pos=uncomp.start=comp.end = comp.start+comp_uncomp_seg_len;
				uncomp.end = uncomp.start + comp_uncomp_seg_len;
			}

			unsafe public uint write_size => 16u +
				(uint)(comp.pos - comp.start) +
				(uint)(uncomp.pos - uncomp.start) +
				(((uint)(bit.pos - bit.start) + 3u) & ~3u);

			public unsafe void WriteTo(System.IO.Stream stream)
			{
				int temp,i;
				stream.WriteByte((byte)((Header.BigEndian >> 24) & 0xFF));
				stream.WriteByte((byte)((Header.BigEndian >> 16) & 0xFF));
				stream.WriteByte((byte)((Header.BigEndian >> 8) & 0xFF));
				stream.WriteByte((byte)(Header.BigEndian & 0xFF));

				temp = (int)(buf.end - buf.start);
				stream.WriteByte((byte)((temp >> 24) & 0xFF));
				stream.WriteByte((byte)((temp >> 16) & 0xFF));
				stream.WriteByte((byte)((temp >> 8) & 0xFF));
				stream.WriteByte((byte)(temp & 0xFF));

				temp = ((((int)(bit.pos - bit.start)) + 3) & ~3) + 16;

				stream.WriteByte((byte)((temp >> 24) & 0xFF));
				stream.WriteByte((byte)((temp >> 16) & 0xFF));
				stream.WriteByte((byte)((temp >> 8) & 0xFF));
				stream.WriteByte((byte)(temp & 0xFF));

				temp += (int)(comp.pos - comp.start);

				stream.WriteByte((byte)((temp >> 24) & 0xFF));
				stream.WriteByte((byte)((temp >> 16) & 0xFF));
				stream.WriteByte((byte)((temp >> 8) & 0xFF));
				stream.WriteByte((byte)(temp & 0xFF));

				for (temp = (int)(bit.pos - bit.start), i = 0; i < temp; ++i)
					stream.WriteByte(bit.start[i]);
				//zero any padding.
				temp =(4 - ((int)(bit.pos - bit.start) & 3)) & 3;
				while (temp-- != 0) stream.WriteByte(0);


				for (temp = (int)(comp.pos - comp.start),i=0; i<temp; ++i)
					stream.WriteByte(comp.start[i]);

				for (temp = (int)(uncomp.pos - uncomp.start), i = 0; i < temp; ++i)
					stream.WriteByte(uncomp.start[i]);
			}
			public unsafe byte* WriteTo(byte* bytes)
			{
				byte* bits_out = bytes + 16,
					comp_out, uncomp_out;
				uint temp; int i;
				bytes[0] = (byte)((Header.BigEndian >> 24) & 0xFF);
				bytes[1] = (byte)((Header.BigEndian >> 16) & 0xFF);
				bytes[2] = (byte)((Header.BigEndian >> 8) & 0xFF);
				bytes[3] = (byte)(Header.BigEndian & 0xFF);

				temp = (uint)(buf.end - buf.start);
				bytes[4] = (byte)((temp >> 24) & 0xFF);
				bytes[5] = (byte)((temp >> 16) & 0xFF);
				bytes[6] = (byte)((temp >> 8) & 0xFF);
				bytes[7] = (byte)(temp & 0xFF);

				temp = ((((uint)(bit.pos - bit.start)) + 3u) & ~3u) + 16u;
				comp_out = bytes + temp;


				bytes[8] = (byte)((temp >> 24) & 0xFF);
				bytes[9] = (byte)((temp >> 16) & 0xFF);
				bytes[10] = (byte)((temp >> 8) & 0xFF);
				bytes[11] = (byte)(temp & 0xFF);

				temp = (uint)(4u - ((uint)(bit.pos - bit.start) & 3u)) & 3u;
				//zero any padding.
				while (temp != 0) *(comp_out - (temp--)) = 0;

				temp = (uint)(comp_out-bytes)+(uint)(comp.pos - comp.start);
				uncomp_out = bytes + temp;
				bytes[12] = (byte)((temp >> 24) & 0xFF);
				bytes[13] = (byte)((temp >> 16) & 0xFF);
				bytes[14] = (byte)((temp >> 8) & 0xFF);
				bytes[15] = (byte)(temp & 0xFF);

				for (i = (int)(uncomp.pos - uncomp.start)-1; 0 <= i; --i)
					uncomp_out[i] = uncomp.start[i];

				for (i = (int)(comp.pos - comp.start)-1; 0 <= i; --i)
					comp_out[i] = comp.start[i];

				for (i = (int)(bit.pos - bit.start)-1; 0 <= i; --i)
					bits_out[i] = bit.start[i];

				return bytes + (uint)(uncomp.pos - uncomp.start);
			}
			public unsafe uint WriteTo(byte[] array) { fixed (byte* arr = array) return (uint)(WriteTo(arr) - arr); }
			public unsafe uint WriteTo(byte[] array, uint offset) { fixed (byte* arr = &array[(int)offset]) return (uint)(WriteTo(arr) - arr); }
		}

		private unsafe static byte[] DeflateVerif(
			byte* buf_start,
			byte * buf_end)
		{
			byte[] o;
			var deflator = new Deflator(buf_start, buf_end);
			var heap_alloc = Marshal.AllocHGlobal((int)deflator.alloc_size+7);
			try
			{
				// try to keep 8 byte alignment, which always has benefits.
				deflator.Offset((IntPtr)(((byte*)heap_alloc.ToPointer())+((8u-(uint)(heap_alloc.ToInt64()&7))&7u)));
				deflator.Deflate();
				o = new byte[(int)deflator.write_size];
				fixed (byte* put = o)
					deflator.WriteTo(put);
			}
			finally
			{
				Marshal.FreeHGlobal(heap_alloc);
			}
			return o;
		}
		
		private unsafe static uint MeasureDeflate(
			byte* buf_start,
			byte* buf_end,
			out Header head)
		{
			var deflator = new Deflator(buf_start, buf_end);
			deflator.Simulate();
			head = deflator.header;
			return deflator.write_size;
		}
		private unsafe static Allocation DeflateToNewAllocation(
			byte* buf_start,
			byte* buf_end)
		{
			Allocation o;
			var deflator = new Deflator(buf_start, buf_end);
			IntPtr heap_alloc = IntPtr.Zero;
			try
			{
				heap_alloc = Marshal.AllocHGlobal((int)deflator.alloc_size + 7);
				if (IntPtr.Zero == heap_alloc) o = null;
				else
				{
					// try to keep 8 byte alignment, which always has benefits.
					deflator.Offset((IntPtr)(((byte*)heap_alloc.ToPointer()) + ((8u - (uint)(heap_alloc.ToInt64() & 7)) & 7u)));
					deflator.Deflate();
					o = new Allocation(deflator.write_size);
					deflator.WriteTo(o.Byte);
				}
			}
			finally
			{
				if (IntPtr.Zero != heap_alloc)
					Marshal.FreeHGlobal(heap_alloc);
			}
			if (IntPtr.Zero == heap_alloc) throw new System.InvalidProgramException("Could not get heap that size:" + (uint)(buf_end - buf_start));
			return o;
		}
		private unsafe static uint Deflate(
			System.IO.Stream stream,
			byte* buf_start,
			byte* buf_end)
		{
			var deflator = new Deflator(buf_start, buf_end);
			var heap_alloc = IntPtr.Zero;
			try
			{
				heap_alloc = Marshal.AllocHGlobal((int)deflator.alloc_size + 7);
				if (IntPtr.Zero != heap_alloc)
				{
					// try to keep 8 byte alignment, which always has benefits.
					deflator.Offset((IntPtr)(((byte*)heap_alloc.ToPointer()) + ((8u - (uint)(heap_alloc.ToInt64() & 7)) & 7u)));
					deflator.Deflate();
					deflator.WriteTo(stream);
				}
			}
			finally
			{
				if(IntPtr.Zero != heap_alloc)
					Marshal.FreeHGlobal(heap_alloc);
			}
			if (IntPtr.Zero == heap_alloc) throw new System.InvalidProgramException("Could not get heap that size:" + (uint)(buf_end - buf_start));
			return deflator.write_size;
		}
		public unsafe static uint MeasureDeflate(ByteSegment segment, out Header head)
		{
			if (null == segment.Byte) throw new ArgumentException("has a null underlying buffer", "segment");
			if (segment.Length < 1) throw new ArgumentException("cannot be zero length", "segment");

			return MeasureDeflate(segment.Byte, segment.Byte + segment.Length, out head);
		}
		public unsafe static uint MeasureDeflate(byte* buf, uint length, out Header head)
		{
			if (null == buf) throw new ArgumentNullException("buf");
			if (length < 1) throw new ArgumentException("cannot be zero length", "array");
			return MeasureDeflate(buf, buf + length, out head);
		}
		public unsafe static uint MeasureDeflate(byte[] array, out Header head)
		{
			uint ret;
			if (null == (object)array) throw new ArgumentNullException("array");
			if (array.Length < 1) throw new ArgumentException("cannot be zero length", "array");
			fixed (byte* buf = array)
				ret = MeasureDeflate(buf, buf + array.Length, out head);
			return ret;
		}
		public unsafe static uint MeasureDeflate(byte[] array, uint offset, uint length, out Header head)
		{
			if (null == (object)array) throw new ArgumentNullException("array");
			if (0 == length) throw new ArgumentException("cannot be zero length", "length");
			if (offset >= array.Length) throw new ArgumentOutOfRangeException("offset");
			if (offset + length > array.Length) throw new ArgumentException("when added with offset, exeeds length of array", "length");

			fixed (byte* buf = &array[offset])
				length = MeasureDeflate(buf, buf + length, out head);
			return length;
		}
		public unsafe static byte[] Deflate(ByteSegment segment)
		{
			if (null==segment.Byte) throw new ArgumentException("has a null underlying buffer", "segment");
			if (segment.Length < 1) throw new ArgumentException("cannot be zero length", "segment");

			return DeflateVerif(segment.Byte, segment.Byte+segment.Length);
		}
		public unsafe static byte[] Deflate(byte* buf, uint length)
		{
			if (null == buf) throw new ArgumentNullException("buf");
			if (length < 1) throw new ArgumentException("cannot be zero length", "array");
			return DeflateVerif(buf, buf+length);
		}
		public unsafe static byte[] Deflate(byte[] array)
		{
			if (null == (object)array) throw new ArgumentNullException("array");
			if (array.Length < 1) throw new ArgumentException("cannot be zero length", "array");
			fixed (byte* buf = array)
				array = DeflateVerif(buf, buf+array.Length);
			return array;
		}
		public unsafe static byte[] Deflate(byte[] array, uint offset, uint length)
		{
			if (null == (object)array) throw new ArgumentNullException("array");
			if (0==length) throw new ArgumentException("cannot be zero length", "length");
			if (offset >= array.Length) throw new ArgumentOutOfRangeException("offset");
			if (offset + length > array.Length) throw new ArgumentException("when added with offset, exeeds length of array", "length");

			fixed (byte* buf = &array[offset])
				array = DeflateVerif(buf, buf+length);
			return array;
		}
		public unsafe static uint Deflate(ByteSegment segment, System.IO.Stream stream)
		{
			if (null == segment.Byte) throw new ArgumentException("has a null underlying buffer", "segment");
			if (segment.Length < 1) throw new ArgumentException("cannot be zero length", "segment");

			return Deflate(stream, segment.Byte, segment.Byte + segment.Length);
		}
		public unsafe static uint Deflate(byte* buf, System.IO.Stream stream, uint length)
		{
			if (null == buf) throw new ArgumentNullException("buf");
			if (length < 1) throw new ArgumentException("cannot be zero length", "array");
			return Deflate(stream, buf, buf + length);
		}
		public unsafe static uint Deflate(byte[] array, System.IO.Stream stream)
		{
			uint length;
			if (null == (object)array) throw new ArgumentNullException("array");
			if (array.Length < 1) throw new ArgumentException("cannot be zero length", "array");
			fixed (byte* buf = array)
				length = Deflate(stream, buf, buf + array.Length);
			return length;
		}
		public unsafe static uint Deflate(byte[] array, System.IO.Stream stream, uint offset, uint length)
		{
			if (null == (object)array) throw new ArgumentNullException("array");
			if (0 == length) throw new ArgumentException("cannot be zero length", "length");
			if (offset >= array.Length) throw new ArgumentOutOfRangeException("offset");
			if (offset + length > array.Length) throw new ArgumentException("when added with offset, exeeds length of array", "length");

			fixed (byte* buf = &array[offset])
				length = Deflate(stream, buf, buf + length);
			return length;
		}
		public unsafe static Allocation DeflateToNewAllocation(ByteSegment segment)
		{
			if (null == segment.Byte) throw new ArgumentException("has a null underlying buffer", "segment");
			if (segment.Length < 1) throw new ArgumentException("cannot be zero length", "segment");

			return DeflateToNewAllocation(segment.Byte, segment.Byte + segment.Length);
		}
		public unsafe static Allocation DeflateToNewAllocation(byte* buf, uint length)
		{
			if (null == buf) throw new ArgumentNullException("buf");
			if (length < 1) throw new ArgumentException("cannot be zero length", "array");
			return DeflateToNewAllocation(buf, buf + length);
		}
		public unsafe static Allocation DeflateToNewAllocation(byte[] array)
		{
			Allocation o;
			if (null == (object)array) throw new ArgumentNullException("array");
			if (array.Length < 1) throw new ArgumentException("cannot be zero length", "array");
			fixed (byte* buf = array)
				o = DeflateToNewAllocation(buf, buf + array.Length);
			return o;
		}
		public unsafe static Allocation DeflateToNewAllocation(byte[] array, uint offset, uint length)
		{
			Allocation o;
			if (null == (object)array) throw new ArgumentNullException("array");
			if (0 == length) throw new ArgumentException("cannot be zero length", "length");
			if (offset >= array.Length) throw new ArgumentOutOfRangeException("offset");
			if (offset + length > array.Length) throw new ArgumentException("when added with offset, exeeds length of array", "length");

			fixed (byte* buf = &array[offset])
				o = DeflateToNewAllocation(buf, buf + length);
			return o;
		}
#if DEBUG
		private struct BinaryHelper
		{
			public byte Value;
			public unsafe override string ToString()
			{
				sbyte* v = stackalloc sbyte[8];
				for (int i = 0; i < 8; i++) v[i] = (Value & (1u << i)) == 0 ? (sbyte)'_' : (sbyte)('0' + i);
				return new string(v, 0, 8);
			}
		}
		private static BinaryHelper Binary(byte v) { return new BinaryHelper { Value = v, }; }
#endif
		/// <summary>
		/// attempts to compress Data which originally contains data decompressed from Input which may have since been modified (in Data) then store the new data back into the input.
		/// returns false when the newly compressed data is not large enough to fit within the Input.
		/// 
		/// The uncompressed size of input always needs to match the size of Data.
		/// if any errors arise (such as input not compressed MIO0) exceptions are thrown.
		/// </summary>
		public static unsafe bool Update(ByteSegment Input, ByteSegment Data)
		{
			if (Input.Allocation == Data.Allocation &&
				Input.Offset < Data.Offset ? (Input.Offset + Input.Length) > Data.Offset :
				(Input.Offset == Data.Offset || (Data.Offset + Data.Length) > Input.Offset))
				throw new ArgumentException("Input and Data arguments overlap in memory");
			// the header should be at dst still!
			Header head, head2;

			uint OldSize = Measure(Input, out head), NewSize;
			bool Updated;

			if (0 == OldSize)
				throw new ArgumentException("the destination did not contains a valid big endian mio0 header", "Input");

			if (head.UncompressedSize != Data.Length)
				throw new ArgumentException("cannot change the size of uncompressed payload", "Data");
			/*
			var mioSrc = Inflate(Input);
			var mioSrcD = Deflate(mioSrc);

			Input.ToArray(out byte[] mioSrcI, 0, (int)OldSize);
			for(int i = 0; i < mioSrcI.Length; i++)
			{
				if (mioSrcI[i] != mioSrcD[i])
					Console.WriteLine("{0,-16}:\n\t{1}\n\t{2}", i, Binary(mioSrcI[i]), Binary(mioSrcD[i]));
			}
			*/

			var deflator = new Deflator(Data.Byte, Data.Byte + Data.Length);
			IntPtr heap_alloc = IntPtr.Zero;
			try
			{
				heap_alloc = Marshal.AllocHGlobal((int)deflator.alloc_size + 7);
				if (IntPtr.Zero == heap_alloc)
				{
					Updated = false;
					NewSize = 0;
					head2 = default(Header);
				}
				else
				{
					// try to keep 8 byte alignment, which always has benefits.
					deflator.Offset((IntPtr)(((byte*)heap_alloc.ToPointer()) + ((8u - (uint)(heap_alloc.ToInt64() & 7)) & 7u)));
					deflator.Deflate();
					head2 = deflator.header;
					NewSize = deflator.write_size;

					if (Updated = (NewSize <= Input.Length))
						deflator.WriteTo(Input.Byte);
				}
			}
			finally
			{
				if (heap_alloc != IntPtr.Zero)
					Marshal.FreeHGlobal(heap_alloc);
			}

			if (Updated)
				//zero any bytes we may have lost.
				while (OldSize-- > NewSize) Input.Byte[OldSize] = 0;
			else if (heap_alloc == IntPtr.Zero)
				Console.WriteLine("Cannot fit because Marshal.AllocHGlobal returned IntPtr.Zero");
			else
				Console.WriteLine("Cannot fit because it reaches beyond the input scratch");

			return Updated;
		}
    }
}
