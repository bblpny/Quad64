using BubblePony.Alloc;
using BubblePony.Integers;

namespace Quad64.Scripts
{
	public abstract class Script
	{
		public static int bytesToInt32(ByteSegment b, int offset=0)
		{
			return
				((int)b[offset + 3]) |
				((int)b[offset + 2] << 8) |
				((int)b[offset + 1] << 16) | 
				((int)b[offset] << 24);
		}
		public static long bytesToInt64(ByteSegment b, int offset=0)
		{
			return
				((long)b[offset + 7]) |
				((long)b[offset] << 56) | ((long)b[offset + 1] << 48) | ((long)b[offset + 2] << 40) | ((long)b[offset + 3] << 32) |
				((long)b[offset + 4] << 24) | ((long)b[offset + 5] << 16) | ((long)b[offset + 6] << 8);
		}
		public static short bytesToInt16(ByteSegment b, int offset=0)
		{
			return (short)((((int)b[offset] << 24) | ((int)b[offset + 1]<<16))>>16);
		}
		public static sbyte bytesToSByte(ByteSegment b, int offset=0)
		{
			return unchecked((sbyte)b[offset]);
		}
		
		public static uint bytesToUInt32(ByteSegment b, int offset=0)
		{
			return
				((uint)b[offset + 3]) |
				((uint)b[offset + 2] << 8) |
				((uint)b[offset + 1] << 16) |
				((uint)b[offset] << 24);
		}
		public static ulong bytesToUInt64(ByteSegment b, int offset=0)
		{
			return
				((ulong)b[offset + 7]) |
				((ulong)b[offset] << 56) | ((ulong)b[offset + 1] << 48) | ((ulong)b[offset + 2] << 40) | ((ulong)b[offset + 3] << 32) |
				((ulong)b[offset + 4] << 24) | ((ulong)b[offset + 5] << 16) | ((ulong)b[offset + 6] << 8);
		}
		public static ushort bytesToUInt16(ByteSegment b, int offset = 0)
		{
			return (ushort)(((ushort)b[offset] << 8) | b[offset + 1]);
		}
		public static byte bytesToByte(ByteSegment b, int offset = 0)
		{
			return b[offset];
		}
		public static UInt24 bytesToUInt24(ByteSegment b, int offset = 0)
		{
			return new UInt24 { B0 = b[offset + 2], B1 = b[offset + 1], B2 = b[offset] };
		}
		public static Vector3s bytesToVector3s(ByteSegment b, int offset = 0)
		{
			unchecked
			{
				return new Vector3s
				{
					Z = bytesToInt16(b, offset + 4),
					Y = bytesToInt16(b, offset + 2),
					X = bytesToInt16(b, offset),
				};
			}
		}
		public static Vector3c bytesToVector3c(ByteSegment b, int offset = 0)
		{
			unchecked
			{
				return new Vector3c
				{
					Z = (sbyte)b[offset + 2],
					Y = (sbyte)b[offset + 1],
					X = (sbyte)b[offset],
				};
			}
		}
		public static SegmentOffset bytesToSegmentOffset(ByteSegment b, int offset = 0)
		{
			return new SegmentOffset { Value = bytesToUInt32(b, offset), };
		}
		public static short intToShort(uint value)
		{
			unchecked
			{
				return (short)(((int)value << 16) >> 16);
			}
		}
		public static short intToShort(int value)
		{
			return (short)(((int)value << 16) >> 16);
		}
		public static sbyte intToSByte(uint value)
		{
			unchecked
			{
				return (sbyte)(((int)value << 24) >> 24);
			}
		}
		public static sbyte intToSByte(int value)
		{
			return (sbyte)(((int)value << 24) >> 24);
		}
	}

}
