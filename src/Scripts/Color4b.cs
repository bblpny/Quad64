using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Quad64
{
	[StructLayout(LayoutKind.Explicit, Size = sizeof(uint))]
	public struct Color4b : IEquatable<Color4b>, IEquatable<uint>
	{
		[FieldOffset(3)]
		public byte A;
		[FieldOffset(2)]
		public byte R;
		[FieldOffset(1)]
		public byte G;
		[FieldOffset(0)]
		public byte B;
		[FieldOffset(0)]
		public uint Value;
		
		public static Color4b Black => new Color4b { A = 255, };
		public static Color4b White => new Color4b { Value = uint.MaxValue, };
		public static Color4b Red => new Color4b { A = 255, R = 255, };
		public static Color4b Green => new Color4b { A = 255, G = 255, };
		public static Color4b Blue => new Color4b { A = 255, B = 255, };
		public static Color4b Magenta => new Color4b { A = 255, B = 255, R=255, };
		public static Color4b Yellow => new Color4b { A = 255, R = 255, G = 255, };
		public static Color4b Cyan => new Color4b { A = 255, G = 255, B = 255, };
		public static Color4b ClearBlack => default(Color4b);
		public static Color4b ClearWhite => new Color4b { R = 255, G = 255, B = 255, };
		public static Color4b ClearRed => new Color4b { R = 255, };
		public static Color4b ClearGreen => new Color4b {  G = 255, };
		public static Color4b ClearBlue => new Color4b { B = 255, };
		public static Color4b ClearMagenta => new Color4b { B = 255, R = 255, };
		public static Color4b ClearYellow => new Color4b {  R = 255, G = 255, };
		public static Color4b ClearCyan => new Color4b { G = 255, B = 255, };


		// these are GL defaults.. https://www.khronos.org/registry/OpenGL-Refpages/gl2.1/xhtml/glMaterial.xml
		public static Color4b Default_Ambient => new Color4b { R = 51, G = 51, B = 51, A = 255, };
		// 0.8 * 255 = 204
		public static Color4b Default_Diffuse => new Color4b { R = 204, G = 204, B = 204, A = 255, };
		public static Color4b Default_Specular => new Color4b { A = 255, };
		public static Color4b Default_Emission => new Color4b { A = 255, };

		public float a { get => ByteUtility.b2f[A]; set => A = ByteUtility.FloatToChannel(value); }
		public float r { get => ByteUtility.b2f[R]; set => R = ByteUtility.FloatToChannel(value); }
		public float g { get => ByteUtility.b2f[G]; set => G = ByteUtility.FloatToChannel(value); }
		public float b { get => ByteUtility.b2f[B]; set => B = ByteUtility.FloatToChannel(value); }
		public float F(int channel) { return ByteUtility.b2f[this[channel]]; }
		public void F(int channel, float value) { this[channel] = ByteUtility.FloatToChannel(value); }

		public void GL_Load() { OpenTK.Graphics.OpenGL.GL.Color4(R, G, B, A); }
		public void GL_Load3() { OpenTK.Graphics.OpenGL.GL.Color3(R, G, B); }

		public static explicit operator OpenTK.Graphics.Color4(Color4b criteria)
		{
			return new OpenTK.Graphics.Color4(criteria.R, criteria.G, criteria.B, criteria.A);
		}

		public unsafe void GL_LoadMaterial(
			OpenTK.Graphics.OpenGL.MaterialFace MaterialFace = OpenTK.Graphics.OpenGL.MaterialFace.FrontAndBack,
			OpenTK.Graphics.OpenGL.MaterialParameter MaterialParameter = OpenTK.Graphics.OpenGL.MaterialParameter.Diffuse)
		{
			float* f = stackalloc float[4];
			f[0] = ByteUtility.b2f[R];
			f[1] = ByteUtility.b2f[G];
			f[2] = ByteUtility.b2f[B];
			f[3] = ByteUtility.b2f[A];
			OpenTK.Graphics.OpenGL.GL.Material(MaterialFace, MaterialParameter, f);
		}

		unsafe public void GL_LoadFogColor()
		{
			float* f = stackalloc float[4];
			f[0] = ByteUtility.b2f[R];
			f[1] = ByteUtility.b2f[G];
			f[2] = ByteUtility.b2f[B];
			f[3] = ByteUtility.b2f[A];
			OpenTK.Graphics.OpenGL.GL.Fog(
				OpenTK.Graphics.OpenGL.FogParameter.FogColor,
				f);
		}
		/// <summary>
		/// this is a bit standard but nonstandard too.
		/// 0 = R, 1 = G, 2 = B, 3 = A.
		/// </summary>
		/// <param name="channel"> 0 = R, 1 = G, 2 = B, 3 = A.</param>
		public byte this[int channel] {
			get => 0 == (channel & 2) ? 0 == (channel & 1) ? R : G : 0 == (channel & 1) ? B : A;
			set { if (0 == (channel & 2)) if (0 == (channel & 1)) R = value; else G = value; else if (0 == (channel & 1)) B = value; else A = value; }
		}
		public bool Equals(Color4b other) { return Value == other.Value; }
		public bool Equals(uint other) { return Value == other; }
		public Color4b(ushort rgba5551)
		{
			TextureFormats.RGBA5551(out this, rgba5551);
		}
		public Color4b(byte R, byte G, byte B, byte A=255)
		{
			Value = 0;
			this.A = A;
			this.R = R;
			this.G = G;
			this.B = B;
		}
		/// <summary>
		/// hue is in degrees.
		/// </summary>
		public static Color4b HSV(double Hue, double Saturation, double Value, byte Alpha=255)
		{
			if (Value > 1.0) Value = 1.0;
			else if (!(Value > 0.0)) Value = 0.0;

			if (Saturation > 1.0) Saturation = 1.0;
			else if (!(Saturation > 0.0)) Saturation = 0.0;

			double Chroma = Value * Saturation;

			if (!(Hue <= double.MaxValue && Hue >= -double.MaxValue))
				return new Color4b { A = Alpha, };
			else
			{
				if (Hue >= 360.0 || Hue < 0.0) Hue = Hue % 360.0;
				if (Hue < 0.0) Hue += 360.0;
				if (Hue >= 360.0) Hue -= 360.0;
			}
			Hue = Hue / 60;
			if (Hue >= 6.0) Hue -= 6;
			double X = ((Hue % 2d) - 1);
			if (X < 0.0) X = -X;
			X=Chroma * (1-X);
			double m = Value - Chroma;
			Chroma += m;
			X += m;

			if(Hue <= 3.0)
			{
				if(1.0<= Hue)
				{
					if (2.0 <= Hue)
					{
						return new Color4b { A = Alpha, r = (float)m, g = (float)Chroma, b = (float)X, };
					}
					else
					{
						return new Color4b { A = Alpha, r = (float)X, g = (float)Chroma, b = (float)m, };
					}
				}
				else
				{
					return new Color4b { A = Alpha, r = (float)Chroma, g = (float)X, b = (float)m, };
				}
			}
			else
			{
				if (4.0 <= Hue)
				{
					if (5.0 <= Hue)
					{
						return new Color4b { A = Alpha, r = (float)Chroma, g = (float)m, b = (float)X, };
					}
					else
					{
						return new Color4b { A = Alpha, r = (float)X, g = (float)m, b = (float)Chroma, };
					}
				}
				else
				{
					return new Color4b { A = Alpha, r = (float)m, g = (float)X, b = (float)Chroma, };
				}
			}
		}
		public ushort Index => (ushort)((ushort)(G << 8) | R);
		public byte List => B;
		public Color4b(ushort Index, byte List) : this((byte)(Index & 255), (byte)(Index >> 8), List) { }
		public Color4b(uint ARGB)
		{
			A = R = B = G = 0;
			Value = ARGB;
		}
		public static explicit operator Color4b(uint ARGB) { return new Color4b { Value = ARGB, }; }
		public static explicit operator uint(Color4b criteria) { return criteria.Value; }
		public static explicit operator int(Color4b criteria) { return unchecked((int)criteria.Value); }
		public static bool operator ==(Color4b L, Color4b R) { return L.Value == R.Value; }
		public static bool operator !=(Color4b L, Color4b R) { return L.Value != R.Value; }
		public static bool operator ==(Color4b L, uint R) { return L.Value == R; }
		public static bool operator !=(Color4b L, uint R) { return L.Value != R; }
		public static bool operator ==(uint L, Color4b R) { return L == R.Value; }
		public static bool operator !=(uint L, Color4b R) { return L != R.Value; }

		public static Color4b operator |(Color4b L, Color4b R) { return new Color4b { Value = L.Value | R.Value, }; }
		public static Color4b operator &(Color4b L, Color4b R) { return new Color4b { Value = L.Value & R.Value, }; }
		public static Color4b operator ^(Color4b L, Color4b R) { return new Color4b { Value = L.Value ^ R.Value, }; }
		public static Color4b operator |(Color4b L, uint R) { return new Color4b { Value = L.Value | R, }; }
		public static Color4b operator &(Color4b L, uint R) { return new Color4b { Value = L.Value & R, }; }
		public static Color4b operator ^(Color4b L, uint R) { return new Color4b { Value = L.Value ^ R, }; }
		public static Color4b operator |(uint L, Color4b R) { return new Color4b { Value = L | R.Value, }; }
		public static Color4b operator &(uint L, Color4b R) { return new Color4b { Value = L & R.Value, }; }
		public static Color4b operator ^(uint L, Color4b R) { return new Color4b { Value = L ^ R.Value, }; }
		public static Color4b operator ~(Color4b criteria) { return new Color4b { Value = ~criteria.Value, }; }
		public static Color4b operator -(Color4b criteria) { return new Color4b { R = (byte)(255 - criteria.R), G = (byte)(255 - criteria.G), B = (byte)(255 - criteria.B), A = (byte)(255 - criteria.A), }; }
		public static Color4b operator +(Color4b criteria) { return new Color4b { R = criteria.R, G = criteria.G, B = criteria.B, A = 255, }; }
		public static bool operator true(Color4b L) { return L.A != 0; }
		public static bool operator false(Color4b L) { return L.A == 0; }

		public static byte ByteClampSum(byte L, byte R)
		{
			ushort V = (ushort)(L + R);
			return (V == (V & 255)) ? (byte)V : (byte)255;
		}
		public static byte ByteClampSub(byte L, byte R)
		{
			return R >= L ? (byte)0 : (byte)(L - R);
		}
		public static Color4b operator +(Color4b L, Color4b R)
		{
			unchecked
			{
				return new Color4b
				{
					A = ByteClampSum(L.A , R.A),
					R = ByteClampSum(L.R , R.R),
					G = ByteClampSum(L.G , R.G),
					B = ByteClampSum(L.B , R.B),
				};
			}
		}
		public static Color4b operator -(Color4b L, Color4b R)
		{
			unchecked
			{
				return new Color4b
				{
					A = ByteClampSub(L.A, R.A),
					R = ByteClampSub(L.R, R.R),
					G = ByteClampSub(L.G, R.G),
					B = ByteClampSub(L.B, R.B),
				};
			}
		}
		public static byte ByteMul(byte L, byte R)
		{
			ushort V = (ushort)((ushort)L * (ushort)R);
			return (byte)((V >> 8) + (0 == (V & 255) ? 0 : 1));
		}
		public static Color4b operator *(Color4b L, Color4b R)
		{
			unchecked
			{
				return new Color4b
				{
					A = ByteMul(L.A , R.A),
					R = ByteMul(L.R , R.R),
					G = ByteMul(L.G , R.G),
					B = ByteMul(L.B , R.B),
				};
			}
		}
		public static byte ByteDiv(byte L, byte R)
		{
			ushort v;
			return R == 0 ? (byte)255 : ((v = (ushort)((ushort)L << 8 / R)) & 255) == v ? (byte)v : (byte)255;
		}
		public static Color4b operator /(Color4b L, Color4b R)
		{
			unchecked
			{
				return new Color4b
				{
					A = ByteDiv(L.A, R.A),
					R = ByteDiv(L.R, R.R),
					G = ByteDiv(L.G, R.G),
					B = ByteDiv(L.B, R.B),
				};
			}
		}
		public static byte ByteBlend(byte L, byte R, byte A)
		{
			return (L < R)?(byte)(L + ByteMul((byte)(R - L), A)):(L > R)?(byte)(R + ByteMul((byte)(L - R), (byte)(255 - A))):L;
		}
		public static Color4b AlphaBlendSrc(Color4b Src, Color4b Dst)
		{
			return new Color4b
			{
				A = ByteBlend(Src.A, Dst.A, Src.A),
				R = ByteBlend(Src.R, Dst.R, Src.A),
				G = ByteBlend(Src.G, Dst.G, Src.A),
				B = ByteBlend(Src.B, Dst.B, Src.A),
			};
		}
		public static Color4b AlphaBlendDst(Color4b Src, Color4b Dst)
		{
			return new Color4b
			{
				A = ByteBlend(Src.A, Dst.A, Dst.A),
				R = ByteBlend(Src.R, Dst.R, Dst.A),
				G = ByteBlend(Src.G, Dst.G, Dst.A),
				B = ByteBlend(Src.B, Dst.B, Dst.A),
			};
		}
		public static Color4b AlphaBlend(Color4b Src, Color4b Dst, byte Alpha)
		{
			return new Color4b
			{
				A = ByteBlend(Src.A, Dst.A, Alpha),
				R = ByteBlend(Src.R, Dst.R, Alpha),
				G = ByteBlend(Src.G, Dst.G, Alpha),
				B = ByteBlend(Src.B, Dst.B, Alpha),
			};
		}


		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
		unsafe public override string ToString()
		{
			byte* buf = stackalloc byte[12];
			buf[0] = (byte)'0';
			buf[1] = (byte)'x';
			buf[2] = (byte)(A >> 4);
			buf[3] = (byte)(A & 15);
			buf[4] = (byte)(R >> 4);
			buf[5] = (byte)(R & 15);
			buf[6] = (byte)(G >> 4);
			buf[7] = (byte)(G & 15);
			buf[8] = (byte)(B >> 4);
			buf[9] = (byte)(B & 15);
			buf[10] = 0;
			for (int i = 9; i > 1; i--) if (buf[i] < 10) buf[i] += (byte)'0'; else buf[i] += (byte)('A' - 10);
			return new string((sbyte*)buf, 0, 10);
		}
		public override bool Equals(object obj)
		{
			return obj is Color4b ? Value == ((Color4b)obj).Value : Value.Equals(obj);
		}
		public static explicit operator Color4b(OpenTK.Vector3 f)
		{
			return new Color4b
			{
				R = ByteUtility.FloatToChannel(f.X),
				G = ByteUtility.FloatToChannel(f.Y),
				B = ByteUtility.FloatToChannel(f.Z),
				A = 255,
			};
		}
		public static explicit operator Color4b(OpenTK.Vector4 f)
		{
			return new Color4b
			{
				R = ByteUtility.FloatToChannel(f.X),
				G = ByteUtility.FloatToChannel(f.Y),
				B = ByteUtility.FloatToChannel(f.Z),
				A = ByteUtility.FloatToChannel(f.W),
			};
		}
		public static explicit operator OpenTK.Vector3(Color4b criteria) { return new OpenTK.Vector3 { X = criteria.r, Y = criteria.g, Z = criteria.b, }; }
		public static explicit operator OpenTK.Vector4(Color4b criteria) { return new OpenTK.Vector4 { X = criteria.r, Y = criteria.g, Z = criteria.b, W = criteria.a, }; }
		public static implicit operator Color4b(System.Drawing.Color color)
		{
			var o = new Color4b { R = color.R, G = color.G, B = color.B, A = color.A, };
			//if (o.Value != unchecked((uint)color.ToArgb())) throw new InvalidOperationException();
			return o;
		}
		public static explicit operator System.Drawing.Color(Color4b color)
		{
			return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
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
				b2n[i & 255] = i < 0 ? i / 128f : i / 127f;
			b2n[127] = 1f;
			b2n[0] = 0f;
			b2n[128] = -1f;
		}

	}
}
