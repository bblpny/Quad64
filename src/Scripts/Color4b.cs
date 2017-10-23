using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Quad64
{
	partial struct Color4b
	{
		public Color4b(ushort rgba5551)
		{
			TextureFormats.RGBA5551(out this, rgba5551);
		}
	}
	public static class ByteUtility
	{
		internal static readonly float[] b2f = new float[256];
		internal static readonly float[] b2n = new float[256];
		public static byte FloatToChannel(float value)
		{
			ushort v;
			// this odd looking statement makes NaN zero.
			return (!(value > 0f)) ? (byte)0 :
#if CHANNEL_FLOAT_ROUND
				// unsure if i like how this rounds to 1, makes zero's threshold less.
				// should probably just floor instead of rounding.
				(value >= (511 / 512f)||(v = (ushort)(((((((v= (ushort)(value * 512))) & 1))) >> 1) + (v512 & 1))) >= 255) ? 
#else
				(value >= 1f || (((v = (ushort)(value *= 256))>value)?--v:v)>=255) ?
#endif
				(byte)255 : (byte)v;
		}
		public static sbyte FloatToNormal(float value)
		{
			return (sbyte)(FloatToChannel((value + 1f) / 2f) - 128);
		}
		public static byte FloatToNormalChannel(float value)
		{
			return FloatToChannel((value + 1f) / 2f);
		}
		public static float ChannelToFloat(byte chn) { return b2f[chn]; }
		public static float NormalToFloat(sbyte axis) { return b2n[((short)axis) & 255]; }
		public static float NormalToFloat(byte signed_axis) { return b2n[signed_axis]; }
		public static float NormalChannelToFloat(byte chn) { return b2n[0 != (chn & 128) ? (byte)(chn & 127) : (byte)(256 - chn)]; }
		static ByteUtility()
		{
			for (int i = 255,j; i >= 0;--i)
			{
				b2f[i] = (float)(((double)i) / 255.0);
				if (i != (j=FloatToChannel(b2f[i])))
					throw new System.InvalidProgramException("whats wrong with your platform's float?");
			}
			b2f[255] = 1f; b2f[0] = 0f;
			for (int i = -128; i < 128; i++)
				b2n[i & 255] = i / 128f;
			//b2n[127] = 1f;
			b2n[0] = 0f;
			b2n[128] = -1f;
		}

	}
}
