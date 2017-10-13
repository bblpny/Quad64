using System;
using BubblePony.Alloc;
using System.Runtime.InteropServices;
using BubblePony.GLHandle;

namespace Quad64
{
	public enum WrapModes : byte
	{
		RepeatRepeat = 0,
		RepeatMirror = 1,
		RepeatClamp = 2,

		MirrorRepeat = 4 | 0,
		MirrorMirror = 4 | 1,
		MirrorClamp = 4 | 2,

		ClampRepeat = 8 | 0,
		ClampMirror = 8 | 1,
		ClampClamp = 8 | 2,
	};
	[StructLayout(LayoutKind.Explicit)]
	public struct TextureIdentifier : IEquatable<TextureIdentifier>, IEquatable<ulong>
	{
		[FieldOffset(0)]
		public SegmentOffset SegmentOffset;

		[FieldOffset(4)]
		public byte Width;

		[FieldOffset(5)]
		public byte Height;

		[FieldOffset(6)]
		public byte Format;

		[FieldOffset(7)]
		public WrapModes Wrap;

		[FieldOffset(0)]
		public ulong Value;

		/// <summary>
		/// the size in bytes of the image in its native format.
		/// </summary>
		public uint ByteSize => TextureFormats.ByteSize(Format, Width, Height);

		public override string ToString()
		{
			return string.Format("{{\"adr\":\"{0}\",\"dim\":[{1},{2}],\"fmt\":{3},\"wrap\":{4}}}", SegmentOffset, Width, Height, Format, Wrap);
		}
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			return obj is TextureIdentifier ? Value == ((TextureIdentifier)obj).Value : Value.Equals(obj);
		}
		public bool Equals(TextureIdentifier other) { return Value == other.Value; }
		public bool Equals(ulong other) { return Value == other; }
		public static bool operator ==(TextureIdentifier L, TextureIdentifier R) { return L.Value == R.Value; }
		public static bool operator !=(TextureIdentifier L, TextureIdentifier R) { return L.Value != R.Value; }
		public static bool operator ==(TextureIdentifier L, ulong R) { return L.Value == R; }
		public static bool operator !=(TextureIdentifier L, ulong R) { return L.Value != R; }
		public static bool operator ==(ulong L, TextureIdentifier R) { return L == R.Value; }
		public static bool operator !=(ulong L, TextureIdentifier R) { return L != R.Value; }

	}
	public sealed class TextureFormats
	{
		public static uint ByteSize(byte Format, byte Width, byte Height)
		{
			return Once.Formats[Format].ByteSize(Width, Height);
		}
		public sealed class Raw
		{
			public readonly byte[] Data;
			public readonly byte[] ARGB;
			public readonly ushort Width, Height;
			public readonly Format Format;
			public readonly object Tag;
			public bool IsColorTexture => Format == ColorFormat;
			public Color4b Color => IsColorTexture ? new Color4b { R = ARGB[R], G = ARGB[G], B = ARGB[B], A = ARGB[A], } : Color4b.White;
			private GraphicsHandle.Texture Handle;

			/// <summary>
			/// gets the handle without automatically binding it to GL. it may or may not be loaded.
			/// </summary>
			public GraphicsHandle.Texture GetHandle()
			{
				lock (this)
					return Handle;
			}
			/// <summary>
			/// gets the handle, if the handle was not loaded and buffered to GL, it will be done by the time this returns.
			/// </summary>
			public GraphicsHandle.Texture GetLoadedHandle()
			{
				if(CreateHandle(out GraphicsHandle.Texture Ret))
					ContentPipe.UpdateHandle(this, ref Ret);
				return Ret;
			}
			internal bool CreateHandle(out GraphicsHandle.Texture Handle)
			{
				bool ret = false;
				lock (this) if (this.Handle.Alive == GraphicsHandle.Null) { this.Handle.Gen(); this.Handle.Tag = this; ret = true; }
				Handle = this.Handle;
				return ret;
			}
			public Raw(
				byte[] Data,
				ushort Width,
				ushort Height,
				Format Format,
				object Tag)
			{
				byte[] ARGB = null;
				Format.Driver.Load(ref Width, ref Height, ref Data, ref ARGB, Format.Code);
				this.Data = Data;
				this.ARGB = ARGB;
				this.Width = Width;
				this.Height = Height;
				this.Format = Format;
				this.Tag = Tag;
			}
		}

		private struct Color : FormatDescription
		{
			byte FormatDescription.AlphaDepth => 8;
			byte FormatDescription.ColorChannels => 3;
			byte FormatDescription.ColorDepth => 8;
			uint FormatDriver.ByteSize(byte w, byte h) => 4;

			void FormatDriver.Load(
				ref ushort width,
				ref ushort height,
				ref byte[] data,
				ref byte[] ARGB,
				byte Code)
			{
				Resize(ref ARGB, 1);

				ARGB[R] = data[0];
				ARGB[G] = data[1];
				ARGB[B] = data[2];
				ARGB[A] = data[3];
			}

		}
		internal interface FormatDriver
		{
			uint ByteSize(byte Width, byte Height);

			void Load(
				ref ushort Width,
				ref ushort Height,
				ref byte[] Data,
				ref byte[] ARGB,
				byte Code);
		}
		private interface FormatDescription : FormatDriver
		{
			byte AlphaDepth { get; }
			byte ColorDepth { get; }
			byte ColorChannels { get; }

		}
		public sealed class Format
		{
			internal readonly FormatDriver Driver;
			public readonly byte Code;
			public readonly byte AlphaDepth;
			public readonly byte ColorDepth;
			public readonly byte ColorChannels;
			public uint ByteSize(byte Width, byte Height) { return Driver.ByteSize(Width, Height); }

			internal Format(object desc, byte Code)
			{
				var Desc = desc as FormatDescription;
				this.Code = Code;
				this.Driver = Desc;
				this.AlphaDepth = Desc.AlphaDepth;
				this.ColorDepth = Desc.ColorDepth;
				this.ColorChannels = Desc.ColorChannels;
			}
		}
		public static readonly Format ColorFormat = new Format(default(Color),0);

		const int R = 2;
		const int G = 1;
		const int B = 0;
		const int A = 3;
		private static Format MakeFormat<T>(Format[] registry, byte code) where T : struct, FormatDriver
		{
			return registry[code] = new Format(default(T), code);
		}
		private static Format MakeFormat(Format[] registry, byte code, byte from)
		{
			return registry[code] = new Format(registry[from].Driver, code);
		}
		static class Once
		{
			public static readonly Format[] Formats = new Format[256];

			static Once()
			{
				MakeFormat<fmtRGBA16>(Formats, 0x10);
				MakeFormat<fmtRGBA32>(Formats, 0x18);
				MakeFormat<fmtIA4>(Formats, 0x60);
				MakeFormat<fmtIA8>(Formats, 0x68);
				MakeFormat<fmtIA16>(Formats, 0x70);
				// Interpret CI textures as grayscale (for now)
				MakeFormat(Formats, 0x40, 0x60);
				MakeFormat(Formats, 0x80, 0x60);
				MakeFormat(Formats, 0x90, 0x60);

				MakeFormat(Formats, 0x48, 0x68);
				MakeFormat(Formats, 0x88, 0x68);
				MakeFormat(Formats, 0x90, 0x68);

				for(int i = 255; i >=0; i--)
					if (null == (object)Formats[i])
						MakeFormat(Formats, (byte)i, 0x10);
				/*
				default:
				case 0x10:
					return decodeRGBA16(data, width, height);
				case 0x18:
					return decodeRGBA32(data, width, height);
				case 0x60:
					return decodeIA4(data, width, height);
				case 0x68:
					return decodeIA8(data, width, height);
				case 0x70:
					return decodeIA16(data, width, height);
				case 0x40: 
				case 0x80:
				case 0x90:
					return decodeI4(data, width, height);
				case 0x48: // Interpret CI textures as grayscale (for now)
				case 0x88:
					return decodeI8(data, width, height);
					*/
			}
		}
		public static Format GetFormat(byte Code)
		{
			return Once.Formats[Code];
		}

		public static Raw ColorTexture(Color4b color)
		{
			return (color.R==255&&color.G==255&&color.B==255&&(color.A==255||color.A==0)) ? 
				color.A==0?ClearWhite:White:new Raw(ColorToPixel(color), ColorFormatWidth, ColorFormatHeight, ColorFormat, color);
		}
		const byte ColorFormatWidth = 1, ColorFormatHeight = 1;

		private static byte[] ColorToPixel(Color4b color)
		{
			byte[] o = new byte[4];
			o[A] = color.A;
			o[R] = color.R;
			o[G] = color.G;
			o[B] = color.B;
			return o;
		}
		private static class ColorConst
		{
			public static readonly object White = Color4b.White;
			public static readonly object ClearWhite = Color4b.ClearWhite;
		}
		private static class Constants
		{
			public static readonly Raw White = new Raw(
				ColorToPixel((Color4b)ColorConst.White),
				ColorFormatWidth, ColorFormatHeight, ColorFormat, ColorConst.White);

			public static readonly Raw ClearWhite = new Raw(
				ColorToPixel((Color4b)ColorConst.ClearWhite),
				ColorFormatWidth, ColorFormatHeight, ColorFormat, ColorConst.ClearWhite);

			public static readonly Raw NullTexture = new Raw(
				ColorToPixel((Color4b)ColorConst.White),
				ColorFormatWidth, ColorFormatHeight, ColorFormat, ColorConst.White);

			public static readonly Raw Empty = new Raw(
				ColorToPixel((Color4b)ColorConst.ClearWhite),
				ColorFormatWidth, ColorFormatHeight, ColorFormat, ColorConst.ClearWhite);
		}
		public static Raw White => Constants.White;
		public static Raw ClearWhite => Constants.ClearWhite;
		public static Raw NullTexture => Constants.NullTexture;
		public static Raw Empty => Constants.Empty;

		/// <summary>
		/// creates a raw texture from data. setting its tag to a weak reference to data.
		/// data must not be null, width and or height must not be zero.
		/// </summary>
		/// <returns>never null</returns>
		public static Raw decodeTexture(byte format, byte[] data, byte width, byte height)
		{
			if (null == (object)data) throw new ArgumentNullException("data");
			if (0 == width) throw new ArgumentException("0", "width");
			if (0 == height) throw new ArgumentException("0", "height");
			return width ==0&&height==0?Empty : new Raw(data, width, height, Once.Formats[format], new WeakReference(data));
		}
		/// <summary>
		/// creates a raw texture from data. setting its tag to the segment.
		/// if data is empty and or both width and height are zero, other arguments are ignored and NullTexture is returned.
		/// otherwise width or height cannot be zero.
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="data"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns>never null</returns>
		public static Raw decodeTexture(byte format, ByteSegment data, byte width, byte height)
		{
			if ((width == 0) ^ (height == 0)) throw new ArgumentException("0", width == 0 ? "width" : "height");

			return data.Length == 0 || (width == 0 && height == 0) ? NullTexture : new Raw(data.ToArray(), width, height, Once.Formats[format], data);
		}
		static byte[] b8 =
		{
			0,
			(byte)(1*256.0/7),
			(byte)(2*256.0/7),
			(byte)(3*256.0/7),
			(byte)(4*256.0/7),
			(byte)(5*256.0/7),
			(byte)(6*256.0/7),
			255,
		};
		static byte[] b16 =
		{
			0,
			(byte)(1*256.0/15),
			(byte)(2*256.0/15),
			(byte)(3*256.0/15),
			(byte)(4*256.0/15),
			(byte)(5*256.0/15),
			(byte)(6*256.0/15),
			(byte)(7*256.0/15),
			(byte)(8*256.0/15),
			(byte)(9*256.0/15),
			(byte)(10*256.0/15),
			(byte)(11*256.0/15),
			(byte)(12*256.0/15),
			(byte)(13*256.0/15),
			(byte)(14*256.0/15),
			255,
		};
		static byte[] b32 =
		{
			0,
			(byte)(1*256.0/31),
			(byte)(2*256.0/31),
			(byte)(3*256.0/31),
			(byte)(4*256.0/31),
			(byte)(5*256.0/31),
			(byte)(6*256.0/31),
			(byte)(7*256.0/31),
			(byte)(8*256.0/31),
			(byte)(9*256.0/31),
			(byte)(10*256.0/31),
			(byte)(11*256.0/31),
			(byte)(12*256.0/31),
			(byte)(13*256.0/31),
			(byte)(14*256.0/31),
			(byte)(15*256.0/31),
			(byte)(16*256.0/31),
			(byte)(17*256.0/31),
			(byte)(18*256.0/31),
			(byte)(19*256.0/31),
			(byte)(20*256.0/31),
			(byte)(21*256.0/31),
			(byte)(22*256.0/31),
			(byte)(23*256.0/31),
			(byte)(24*256.0/31),
			(byte)(25*256.0/31),
			(byte)(26*256.0/31),
			(byte)(27*256.0/31),
			(byte)(28*256.0/31),
			(byte)(29*256.0/31),
			(byte)(30*256.0/31),
			255,
		};
		static void Resize(ref byte[] ARGB, int pixels)
		{
			pixels <<= 2;
			if (null == (object)ARGB || ARGB.Length != pixels)
				ARGB = new byte[pixels];
		}

		struct fmtRGBA32 : FormatDescription
		{
			byte FormatDescription.ColorDepth => 8;
			byte FormatDescription.AlphaDepth => 8;
			byte FormatDescription.ColorChannels => 3;
			uint FormatDriver.ByteSize(byte Width, byte Height) => ((uint)Width * Height) << 2;

			void FormatDriver.Load(
				ref ushort width,
				ref ushort height,
				ref byte[] data,
				ref byte[] ARGB,
				byte Code)
			{
				Resize(ref ARGB, width * (int)height);
				byte red, green, blue, alpha;

				for (int i = (ARGB.Length>>2) - 1; i >= 0; --i)
				{
					red = data[(i<<2)|0]; // Red
					green = data[(i<<2)| 1]; // Green
					blue = data[(i<<2)| 2]; // Blue
					alpha = data[(i<<2)| 3]; // Alpha (Transparency)

					ARGB[(i << 2) | R] = red;
					ARGB[(i << 2) | G] = green;
					ARGB[(i << 2) | B] = blue;
					ARGB[(i << 2) | A] = alpha;
				}
			}
		}

		struct fmtRGBA16 : FormatDescription
		{
			byte FormatDescription.ColorDepth => 5;
			byte FormatDescription.AlphaDepth => 1;
			byte FormatDescription.ColorChannels => 3;
			uint FormatDriver.ByteSize(byte Width, byte Height) => ((uint)Width * Height) << 1;

			void FormatDriver.Load(
				ref ushort width,
				ref ushort height,
				ref byte[] data,
				ref byte[] ARGB,
				byte Code)
			{
				Resize(ref ARGB, width * (int)height);
				byte red, green, blue, alpha;
				ushort pixel;

				for (int i = (ARGB.Length >> 2) - 1; i >= 0; --i)
				{
					pixel = (ushort)((data[i << 1] << 8) | data[(i << 1) | 1]);
					red = b32[(pixel >> 11) & 0x1F]; // Red
					green = b32[(pixel >> 6) & 0x1F]; // Green
					blue = b32[(pixel >> 1) & 0x1F]; // Blue
					alpha = 0 == (pixel & 1) ? (byte)0 : (byte)255; // Alpha (Transparency)
					ARGB[(i << 2) | R] = red;
					ARGB[(i << 2) | G] = green;
					ARGB[(i << 2) | B] = blue;
					ARGB[(i << 2) | A] = alpha;
				}
			}
		}

		struct fmtIA16 : FormatDescription
		{
			byte FormatDescription.ColorDepth => 8;
			byte FormatDescription.AlphaDepth => 8;
			byte FormatDescription.ColorChannels => 1;
			uint FormatDriver.ByteSize(byte Width, byte Height) => ((uint)Width * Height) << 1;
			void FormatDriver.Load(
				ref ushort width,
				ref ushort height,
				ref byte[] data,
				ref byte[] ARGB,
				byte Code)
			{
				Resize(ref ARGB, width * (int)height);
				byte intensity, alpha;
				for (int i = (ARGB.Length >> 2) - 1; i >= 0; --i)
				{
					intensity = data[i <<1];
					alpha = data[(i<<1)| 1];
					ARGB[(i << 2) | R] = intensity;
					ARGB[(i << 2) | G] = intensity;
					ARGB[(i << 2) | B] = intensity;
					ARGB[(i << 2) | A] = alpha;
				}
			}
		}

		struct fmtIA8 : FormatDescription
		{
			byte FormatDescription.ColorDepth => 4;
			byte FormatDescription.AlphaDepth => 4;
			byte FormatDescription.ColorChannels => 1;
			uint FormatDriver.ByteSize(byte Width, byte Height) => ((uint)Width * Height);
			void FormatDriver.Load(
				ref ushort width,
				ref ushort height,
				ref byte[] data,
				ref byte[] ARGB,
				byte Code)
			{
				Resize(ref ARGB, width * (int)height);
				byte intensity, alpha;
				for (int i = (ARGB.Length >> 2) - 1; i >= 0; --i)
				{
					intensity = b16[data[i] >> 4];
					alpha = b16[data[i]&15];
					ARGB[(i << 2) | R] = intensity;
					ARGB[(i << 2) | G] = intensity;
					ARGB[(i << 2) | B] = intensity;
					ARGB[(i << 2) | A] = alpha;
				}
			}
		}
		const byte Ialpha = 255;
		struct fmtIA4 : FormatDescription
		{
			byte FormatDescription.ColorDepth => 3;
			byte FormatDescription.AlphaDepth => 1;
			byte FormatDescription.ColorChannels => 1;
			uint FormatDriver.ByteSize(byte Width, byte Height) => ((uint)Width * Height)>>1;
			void FormatDriver.Load(
				ref ushort width,
				ref ushort height,
				ref byte[] data,
				ref byte[] ARGB,
				byte Code)
			{
				Resize(ref ARGB, width * (int)height);
				int len = (width * height) / 2;
				for (int i = (ARGB.Length>>3) - 1; i >= 0; --i)
				{
					byte twoPixels = data[i];
					byte intensity = b8[twoPixels >> 5];
					ARGB[(i << 3) | R] = intensity;
					ARGB[(i << 3) | G] = intensity;
					ARGB[(i << 3) | B] = intensity;
					ARGB[(i << 3) | A] = 0 == (twoPixels & 16) ? (byte)0 : (byte)255; 
					intensity = b8[(twoPixels >> 1) & 0x7];
					ARGB[((i << 3) | 4) | R] = intensity;
					ARGB[((i << 3) | 4) | G] = intensity;
					ARGB[((i << 3) | 4) | B] = intensity;
					ARGB[((i << 3) | 4) | A] = 0 == (twoPixels & 1) ? (byte)0 : (byte)255;
				}
			}
		}
		struct fmtI8 : FormatDescription
		{
			byte FormatDescription.ColorDepth => 8;
			byte FormatDescription.AlphaDepth => 0;
			byte FormatDescription.ColorChannels => 1;
			uint FormatDriver.ByteSize(byte Width, byte Height) => ((uint)Width * Height);
			void FormatDriver.Load(
				ref ushort width,
				ref ushort height,
				ref byte[] data,
				ref byte[] ARGB,
				byte Code)
			{
				Resize(ref ARGB, width * (int)height);

				byte intensity;
				for (int i = (ARGB.Length >> 2) - 1; i >= 0; --i)
				{
					intensity = data[i];
					ARGB[(i << 2) | R] = intensity;
					ARGB[(i << 2) | G] = intensity;
					ARGB[(i << 2) | B] = intensity;
					ARGB[(i << 2) | A] = Ialpha;
				}
			}
		}
		struct fmtI4 : FormatDescription
		{
			byte FormatDescription.ColorDepth => 4;
			byte FormatDescription.AlphaDepth => 0;
			byte FormatDescription.ColorChannels => 1;
			uint FormatDriver.ByteSize(byte Width, byte Height) => (uint)(Width * Height)>>1;
			void FormatDriver.Load(
				ref ushort width,
				ref ushort height,
				ref byte[] data,
				ref byte[] ARGB,
				byte Code)
			{
				Resize(ref ARGB, width * (int)height);
				byte twoPixels, intensity;
				for (int i = (ARGB.Length >> 3) - 1; i >= 0; --i)
				{
					twoPixels = data[i];
					intensity = b16[twoPixels >> 4];

					ARGB[(i << 3) | R] = intensity;
					ARGB[(i << 3) | G] = intensity;
					ARGB[(i << 3) | B] = intensity;
					ARGB[(i << 3) | A] = Ialpha;
					intensity = b16[twoPixels & 255];

					ARGB[((i << 3)|4) | R] = intensity;
					ARGB[((i << 3)|4) | G] = intensity;
					ARGB[((i << 3)|4) | B] = intensity;
					ARGB[((i << 3)|4) | A] = Ialpha;
				}
			}
		}
    }
}
