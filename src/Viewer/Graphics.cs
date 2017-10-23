using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using BubblePony.GLHandle;
using BubblePony.Integers;
using System.Runtime.InteropServices;
namespace Quad64
{
	[Flags]
	public enum DrawOptions : byte
	{
		NoMaterial = 1,
		NoBuffers = 2,
		NoTriangles = 4,
		NoCullingChanges = 8,
		NoBind = 16,
		NoGetState = 32,
		ForceInvalidate = 64,
	}
	public static class DrawOptionsUtility
	{
		public const int BitSize = 6;
		public const DrawOptions Mask = (DrawOptions)127;

		public static DrawOptions Sanitize(this DrawOptions Value) {
			if ((DrawOptions.NoBind | DrawOptions.ForceInvalidate) ==
				(Value & (DrawOptions.ForceInvalidate | DrawOptions.NoBind)))
				Value ^= DrawOptions.NoBind;// <- force invalidate forces no bind off.

			return Value & Mask;
		}
		public static void Sanitize(ref DrawOptions Value) {
			Value = Sanitize(Value);
		}
	}
	public enum Culling : byte
	{
		Off,
		Front,
		Back,
		Both,
	}
	public static class CullingUtility
	{
		public const int Count = 4;
		public const int BitSize = 2;
		public const Culling Mask = (Culling)3;
		public static Culling Sanitize(this Culling Value) { return Value & Mask; }
		public static void Sanitize(ref Culling Value) { Value &= Mask; }
	}
	public enum LightMode : byte
	{
		Hard0,
		Smooth0,
		Hard1,
		Smooth1,
		Hard2,
		Smooth2,
		Hard3,
		Smooth3,
		Hard4,
		Smooth4,
		Hard5,
		Smooth5,
		Hard6,
		Smooth6,
		Hard7,
		Smooth7,
	}
	public static class LightModeUtility
	{
		public const int Count = 16;
		public const int BitSize = 4;
		public const LightMode Mask = (LightMode)15;
		public const byte MaxLights = 7;
		public static LightMode Describe(bool Smooth, byte LightCount)
		{
			if (LightCount > MaxLights) LightCount = MaxLights;

			return (LightMode)((byte)(((LightCount<<1)|(Smooth?1:0))&(byte)Mask));
		}
		public static byte LightCount(this LightMode Value) { return (byte)((byte)(Value & Mask) >> 1); }
		public static LightMode Sanitize(this LightMode Value) { return Value & Mask; }
		public static void Sanitize(ref LightMode Value) { Value &= Mask; }
	}
	[Flags]
	public enum ElementMask : byte
	{
		Position = 1,
		Texcoord = 2,
		Normal = 4,
		Color = 8,
		Index = 16,
		Texture = 32,
		// Note that if you add more, you must update ElementMaskUtility.
	}
	public static class ElementMaskUtility
	{
		public const int BitSize = 6;

		public const ElementMask Vertex = ElementMask.Color | ElementMask.Position | ElementMask.Texcoord | ElementMask.Normal;
		public const ElementMask NormalColor = ElementMask.Normal | ElementMask.Color;
		public const ElementMask All = Vertex | ElementMask.Index | ElementMask.Texture;

		public const ElementMask NonVertex = (ElementMask)((~((byte)Vertex)) & (byte)All);
		public const ElementMask NonIndex = (ElementMask)((~((byte)ElementMask.Index)) & (byte)All);
		public const ElementMask NonTexture = (ElementMask)((~((byte)ElementMask.Texture)) & (byte)All);

		public static ElementMask Sanitize(this ElementMask Mask)
		{
			return Mask & All;
		}
		public static void Sanitize(ref ElementMask Mask)
		{
			Mask &= All;
		}

	}
	[Flags]
	public enum FeatureMask : byte
	{
		ZWrite = 1,
		PolygonOffset = 2,
		AlphaBlend = 4,
		ZTest = 8,
		Lit = 16,
		Fog = 32,
		// Note that if you add more, you must update All in FeatureMaskUtility.
	}
	public static class FeatureMaskUtility
	{
		public const int BitSize = 5;
		public const FeatureMask All = (FeatureMask)63;
		public static FeatureMask Sanitize(this FeatureMask Value) { return (Value & All); }
		public static void Sanitize(ref FeatureMask Value) { Value &= All; }
	}
	public enum VertexColor : byte
	{
		// smoothly interpolate color between each triangle vertex.
		Smooth=0,
		// use the vertex color on the first vertex of triangle.(unimplemented)
		A = 1,
		// use the vertex color on the second vertex of triangle.(unimplemented)
		B = 2,
		// use the vertex color on the third vertex of triangle.(unimplemented)
		C = 3,
	}
	public static class VertexColorUtility
	{
		public const int Count = 4;
		public const int BitSize = 2;
		public const VertexColor Mask = (VertexColor)3;
		public static void Sanitize(ref VertexColor Value)
		{
			Value &= Mask;
		}
		public static VertexColor Sanitize(this VertexColor Value)
		{
			return Value & Mask;
		}
	}

	[Flags]
	public enum TexturePresentation : byte
	{
		MirrorS = 1,
		ClampS = 2,
		MirrorT = 4,
		ClampT = 8,
	}
	public static class TexturePresentationUtility
	{
		public const TexturePresentation MaskS = TexturePresentation.MirrorS | TexturePresentation.ClampS;
		public const TexturePresentation MaskT = TexturePresentation.MirrorT | TexturePresentation.ClampT;
		public const TexturePresentation Mask = MaskS | MaskT;
		public const int BitSize = 4;
		public const sbyte ShiftS = 0, ShiftT = 2;

		public static TextureAxisWrap GetWrapS(this TexturePresentation TexturePresentation)
		{
			return 0 == (TexturePresentation & TexturePresentation.MirrorS) ?
				0 == (TexturePresentation & TexturePresentation.ClampS) ? TextureAxisWrap.Repeat : TextureAxisWrap.Clamp :
				0 == (TexturePresentation & TexturePresentation.ClampS) ? TextureAxisWrap.Mirror : TextureAxisWrap.Repeat;
		}
		public static TextureAxisWrap GetWrapT(this TexturePresentation TexturePresentation)
		{
			return 0 == (TexturePresentation & TexturePresentation.MirrorT) ?
				0 == (TexturePresentation & TexturePresentation.ClampT) ? TextureAxisWrap.Repeat : TextureAxisWrap.Clamp :
				0 == (TexturePresentation & TexturePresentation.ClampT) ? TextureAxisWrap.Mirror : TextureAxisWrap.Repeat;
		}
		public static TexturePresentation WithWrapS(this TexturePresentation TexturePresentation, TextureAxisWrap Wrap)
		{
			return (TexturePresentation & ~MaskS) | (
				TextureAxisWrap.Mirror == Wrap ? TexturePresentation.MirrorS :
				TextureAxisWrap.Clamp == Wrap ? TexturePresentation.ClampS : (TexturePresentation)0
				);
		}
		public static TexturePresentation WithWrapT(this TexturePresentation TexturePresentation, TextureAxisWrap Wrap)
		{
			return (TexturePresentation & ~MaskT) | (
				TextureAxisWrap.Mirror == Wrap ? TexturePresentation.MirrorT :
				TextureAxisWrap.Clamp == Wrap ? TexturePresentation.ClampT : (TexturePresentation)0
				);
		}
		public static TexturePresentation Wrap(TextureAxisWrap S, TextureAxisWrap T)
		{
			return (
				TextureAxisWrap.Mirror == S ? TexturePresentation.MirrorS :
				TextureAxisWrap.Clamp == S ? TexturePresentation.ClampS 
				: (TexturePresentation)0
				) | (
				TextureAxisWrap.Mirror == T ? TexturePresentation.MirrorT :
				TextureAxisWrap.Clamp == T ? TexturePresentation.ClampT
				: (TexturePresentation)0
				);
		}
		public static void Sanitize(
			ref TexturePresentation TexturePresentation)
		{
			TexturePresentation = Wrap(GetWrapS(TexturePresentation), GetWrapT(TexturePresentation));
		}
		public static TexturePresentation Sanitize(this TexturePresentation TexturePresentation)
		{
			return Wrap(GetWrapS(TexturePresentation), GetWrapT(TexturePresentation));
		}
	}
	public enum TextureAxisWrap : byte
	{
		Repeat, Mirror, Clamp
	}
	public static class TextureAxisWrapUtility
	{
		public const int Count = 3;
		public const int BitSize = 2;
		public static void Sanitize(ref TextureAxisWrap Wrap)
		{
			if (Wrap >= (TextureAxisWrap)Count) Wrap = 0;
		}
		public static TextureAxisWrap Sanitize(this TextureAxisWrap Wrap)
		{
			return Wrap >= (TextureAxisWrap)Count ? (TextureAxisWrap)0 : Wrap;
		}
		public static T Select<T>(this TextureAxisWrap Value, T Repeat, T Mirror, T Clamp)
		{
			return Value == TextureAxisWrap.Clamp ? Clamp : Value == TextureAxisWrap.Mirror ? Mirror : Repeat;
		}
	}
	// names must match opentk's PrimitiveType in order to convert
	// (index doesn't matter, but there should be no more than 16 entries)
	public enum IndexPrimitive : byte
	{
		Triangles=0,
		TriangleStrip = 1,
		Lines = 2,
		LineStrip = 3,

		TrianglesAdjacency = 4,
		TriangleStripAdjacency = 5,
		LinesAdjacency = 6,
		LineStripAdjacency = 7,

		Quads = 8,
		QuadStrip = 9,

		Points = 10,
		LineLoop = 11,

		Polygon = 12,
		TriangleFan = 13,

		Patches = 14,
	}
	public static class IndexPrimitiveUtility
	{
		public const int Count = 15;
		public const int BitSize = 4;
		public static void Sanitize(ref IndexPrimitive Primitive)
		{
			if (Primitive >= (IndexPrimitive)Count)
				Primitive = IndexPrimitive.Triangles;
		}

		public static IndexPrimitive Sanitize(this IndexPrimitive Primitive)
		{
			Sanitize(ref Primitive);
			return Primitive;
		}

		static readonly PrimitiveType[] GLEquiv = new PrimitiveType[Count];

		static IndexPrimitiveUtility()
		{
			for (int i = Count - 1; i >= 0; --i)
			{
				var P = (IndexPrimitive)((byte)i);
				var Str = P.ToString();
				try
				{
					GLEquiv[i] = (PrimitiveType)Enum.Parse(typeof(PrimitiveType), Str);
					continue;
				}
				catch (ArgumentException) { }
				catch (OverflowException) { }
				GLEquiv[i] = (PrimitiveType)254;
			}
		}

		public static PrimitiveType ToGL(this IndexPrimitive Value, PrimitiveType Fallback = PrimitiveType.Triangles)
		{
			return ((byte)Value >= Count || GLEquiv[(byte)Value] == (PrimitiveType)254) ? Fallback : GLEquiv[(byte)Value];
		}
	}
	public enum IndexInteger : byte
	{
		Byte,
		Short,
		Int,
	}
	public static class IndexIntegerUtility
	{
		public const int Count = 3;
		public const int BitSize = 2;
		public static void Sanitize(ref IndexInteger Value)
		{
			if ((byte)Value >= Count) Value = (IndexInteger)(Count - 1);
		}
		public static IndexInteger Sanitize(this IndexInteger Value)
		{
			Sanitize(ref Value);
			return Value;
		}
	}
	[StructLayout(LayoutKind.Explicit)]
	public struct GraphicsLight : IEquatable<GraphicsLight>
	{
		[FieldOffset(0)]
		public Color4b Color;
		[FieldOffset(4)]
		public Vector4c Normal;
		[FieldOffset(0)]
		public long Value;
		public static void Sanitize(ref GraphicsLight Light)
		{
			Light.Color.A = 255;
			Light.Normal.W = 0;
		}
		public GraphicsLight Sanitize()
		{
			var Copy = this;
			Copy.Color.A = 255;
			Copy.Normal.W = 0;
			return Copy;
		}
		public static int GetHashCode(ref GraphicsLight L)
		{
			return ((((int)L.Color.R << 16) | ((int)L.Color.G << 8) | ((int)L.Color.B))*5) ^
				((((L.Normal.Y & 127) << 25) | ((((int)L.Normal.Y << (24 - 7)) & (1 << 24)))) |
				(((L.Normal.X & 127) << 17) | (((int)L.Normal.X << (16 - 7)) & (1 << 16))) |
				(((L.Normal.Z & 127) << 7) | (((int)L.Normal.Z << (8 - 7)) & (1 << 8))));
		}
		public static int GetHashCode(ref GraphicsLight L, sbyte ROT)
		{
			unchecked
			{
				uint HashCode = (uint)GetHashCode(ref L);
				return (int)((HashCode << ROT) | (HashCode >> (32 - ROT)));
			}
		}
		public override int GetHashCode()
		{
			return GetHashCode(ref this);
		}
		public bool Equals(ref GraphicsLight other)
		{
			return Color.R == other.Color.R &&
				Color.G == other.Color.G &&
				Color.B == other.Color.B &&
				Normal.X == other.Normal.X &&
				Normal.Y == other.Normal.Y &&
				Normal.Z == other.Normal.Z;
		}
		public bool Equals(GraphicsLight other)
		{
			return Color.R == other.Color.R &&
				Color.G == other.Color.G &&
				Color.B == other.Color.B &&
				Normal.X == other.Normal.X &&
				Normal.Y == other.Normal.Y &&
				Normal.Z == other.Normal.Z;
		}
		public override bool Equals(object obj)
		{
			return obj is GraphicsLight && ((GraphicsLight)obj).Equals(ref this);
		}
		public static bool Equals(ref GraphicsLight L, ref GraphicsLight R)
		{
			return L.Color.R == R.Color.R &&
					L.Color.G == R.Color.G &&
					L.Color.B == R.Color.B &&
					L.Normal.X == R.Normal.X &&
					L.Normal.Y == R.Normal.Y &&
					L.Normal.Z == R.Normal.Z;
		}
		public static bool Inequals(ref GraphicsLight L, ref GraphicsLight R)
		{
			return L.Color.R != R.Color.R ||
					L.Color.G != R.Color.G ||
					L.Color.B != R.Color.B ||
					L.Normal.X != R.Normal.X ||
					L.Normal.Y != R.Normal.Y ||
					L.Normal.Z != R.Normal.Z;
		}
		public static bool operator ==(GraphicsLight L, GraphicsLight R)
		{
			return L.Color.R == R.Color.R &&
					L.Color.G == R.Color.G &&
					L.Color.B == R.Color.B &&
					L.Normal.X == R.Normal.X &&
					L.Normal.Y == R.Normal.Y &&
					L.Normal.Z == R.Normal.Z;

		}
		public static bool operator !=(GraphicsLight L, GraphicsLight R)
		{
			return L.Color.R != R.Color.R ||
					L.Color.G != R.Color.G ||
					L.Color.B != R.Color.B ||
					L.Normal.X != R.Normal.X ||
					L.Normal.Y != R.Normal.Y ||
					L.Normal.Z != R.Normal.Z;

		}
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct GraphicsState : IEquatable<GraphicsState>
	{
		/// <summary>
		/// the vertex buffer reference.
		/// its data shall be in the format of:
		/// XXXX YYYY ZZZZ FFFF SSSS TTTT RR GG BB AA
		/// where X indicates a signed short (16 bits) for position
		/// where Y indicates a signed short (16 bits) for position
		/// where Z indicates a signed short (16 bits) for position
		/// where F indicates 16 bits of unused data (flags)
		/// where S indicates the first texture coordinate as a signed short (16 bits)
		/// where T indicates the second texture coordinate as a signed short (16 bits)
		/// where R indicates either the red color (unsigned byte) or the X normal (signed byte)
		/// where G indicates either the green color (unsigned byte) or the Y normal (signed byte)
		/// where B indicates either the blue color (unsigned byte) or the Z normal (signed byte)
		/// where A indicates the alpha color.
		/// 
		/// this value is ignored when each Position, Texcoord, Normal and Color are all not present in the ElementMask.
		/// </summary>
		public GraphicsHandle.Buffer Vertex;

		/// <summary>
		/// the index buffer.
		/// value is ignored when Index is not present in ElementMask.
		/// </summary>
		public GraphicsHandle.Buffer Index;

		/// <summary>
		/// a handle to the texture buffer.
		/// value is ignored when Texture is not present in ElementMask
		/// </summary>
		public GraphicsHandle.Texture Texture;

		/// <summary>
		/// the color to use for non-lit and non-vertex color drawing.
		/// this is ignored when vertex color is enabled in ElementMask
		/// this is ignored when lighting is enabled in FeatureMask
		/// 
		/// when ignored, it is treated as White.
		/// </summary>
		public Color4b Color;
		/// <summary>
		/// game does not set this.
		/// this is ignored when Lit is not present in FeatureMask (treated as Color4b.Default_Diffuse when so)
		/// </summary>
		public Color4b Diffuse;

		/// <summary>
		/// the ambient LIGHTING color. A is ignored.
		/// </summary>
		public Color4b Ambient;

		public GraphicsLight Light1;
		public GraphicsLight Light2;
		public GraphicsLight Light3;
		public GraphicsLight Light4;
		public GraphicsLight Light5;
		public GraphicsLight Light6;
		public GraphicsLight Light7;

		/// <summary>
		/// the fog color.
		/// this is ignored when Fog is not present in FeatureMask.
		/// </summary>
		public Color4b Fog;
		public ushort FogMultiplier;
		public ushort FogOffset;

		/// <summary>
		/// the count of indices in the index buffer.
		/// (ignored when Index is not present in ElementMask)
		/// </summary>
		public ushort IndexCount;
		/// <summary>
		/// the primitive type to draw from the indices.
		/// (ignored when Index is not present in ElementMask)
		/// </summary>
		public IndexPrimitive IndexPrimitive;
		/// <summary>
		/// the integer (byte,ushort or uint) type of indices in the index buffer.
		/// (ignored when Index is not present in ElementMask)
		/// </summary>
		public IndexInteger IndexInteger;

		/// <summary>
		/// the width of the texture used to scale texture coordinates.
		/// (ignored when Texcoord is not present in ElementMask)
		/// </summary>
		public ushort TextureWidth;

		/// <summary>
		/// the height of the texture used to scale texture coordinates.
		/// (ignored when Texcoord is not present in ElementMask)
		/// </summary>
		public ushort TextureHeight;

		/// <summary>
		/// the S (U/X) texture scale for texture coordinates.
		/// (ignored when Texcoord is not present in ElementMask)
		/// </summary>
		public ushort TextureScaleX;

		/// <summary>
		/// the T (V/Y) texture scale for texture coordinates.
		/// (ignored when Texcoord is not present in ElementMask)
		/// </summary>
		public ushort TextureScaleY;

		/// <summary>
		/// the two texture wrap modes (ignored when Texture is not present in the ElementMask)
		/// </summary>
		public TexturePresentation TexturePresentation;

		/// <summary>
		/// how lighting works. 
		/// (Ignored when Lit is not present in FeatureMask)
		/// </summary>
		public LightMode LightMode;

		/// <summary>
		/// how vertex colors are blended. 
		/// (ignored when Color is not present in ElementMask)
		/// </summary>
		public VertexColor VertexColor;
		/// <summary>
		/// determines what elements from the Vertex, Index and Texture handles are used to draw.
		/// </summary>
		public ElementMask ElementMask;
		/// <summary>
		/// determines how culling works.
		/// </summary>
		public Culling Culling;
		/// <summary>
		/// features that are present in this mask determine how things draw.
		/// </summary>
		public FeatureMask FeatureMask;

		public static GraphicsState Default => new GraphicsState
		{
			Color = { Value = uint.MaxValue, },
			Diffuse = Color4b.Default_Diffuse,
			Ambient = Color4b.White,
			Fog = Color4b.ClearWhite,
		};

		private const int SumBits = 
			IndexIntegerUtility.BitSize +
			IndexPrimitiveUtility.BitSize +
			VertexColorUtility.BitSize + 
			TexturePresentationUtility.BitSize +
			CullingUtility.BitSize + 
			ElementMaskUtility.BitSize + 
			LightModeUtility.BitSize +
			ElementMaskUtility.BitSize;

		//if this don't compile bits don't fit.
		private const uint CompileTestSumBitsGreaterEqual32 = 32 - SumBits;

		private const int FeatureMaskShift = (32 - FeatureMaskUtility.BitSize);
		private const int VertexColorShift = FeatureMaskShift - 1;

		private const int ElementMaskShift = VertexColorShift - ElementMaskUtility.BitSize;
		private const int CullingShift = ElementMaskShift - CullingUtility.BitSize;

		private const int TexturePresentationShift = CullingShift - TexturePresentationUtility.BitSize;
		private const int LightModeShift = TexturePresentationShift - VertexColorUtility.BitSize;

		private const int IndexPrimitiveShift = LightModeShift - IndexPrimitiveUtility.BitSize;
		private const int IndexIntegerShift = IndexPrimitiveShift - IndexIntegerUtility.BitSize;

		public int HashComponentRoot => unchecked( 5 * 
			(((int)((byte)this.ElementMask.Sanitize()) << ElementMaskShift) |
			((int)((byte)this.FeatureMask.Sanitize()) << FeatureMaskShift) |
			((int)((byte)this.Culling.Sanitize()) << CullingShift) |
			(0 == (this.FeatureMask & FeatureMask.Lit) ? 0 
				: ((int)((byte)this.LightMode.Sanitize()) << LightModeShift)) |
			(0 == (this.ElementMask & ElementMask.Color) ? 0 
				: ((int)((byte)this.VertexColor.Sanitize()) << VertexColorShift)) |
			(0 == (this.ElementMask & ElementMask.Texture) ? 0 
				: ((int)((byte)this.TexturePresentation.Sanitize()) << TexturePresentationShift)) |
			(0 == (this.ElementMask & ElementMask.Index)?0
				: (((int)((byte)this.IndexPrimitive.Sanitize()) <<IndexPrimitiveShift)|
					((int)((byte)this.IndexInteger.Sanitize()) <<IndexIntegerShift)))));

		private static int ROT3(int value) { return (value << 3) | ((value >> (32 - 3)) & ((1 << 3) - 1)); }
		private static int ROT6(int value) { return (value << 6) | ((value >> (32 - 6)) & ((1 << 6) - 1)); }
		private static int ROT9(int value) { return (value << 9) | ((value >> (32 - 9)) & ((1 << 9) - 1)); }
		private static int ROT15(int value) { return (value << 15) | ((value >> (32 - 15)) & ((1 << 15) - 1)); }
		private static int ROT17(int value) { return (value << 17) | ((value >> (32 - 17)) & ((1 << 17) - 1)); }
		private static int ROT24(int value) { return (value << 24) | ((value >> (32 - 24)) & ((1 << 24) - 1)); }


		public int HashComponentTexture => 
			(0 == (this.ElementMask & ElementMask.Texcoord) ? 0 : ROT3(
			((TextureWidth < TextureHeight) ? -1 : 0) ^
			(((int)TextureWidth << 16) | TextureHeight) ^
			ROT24(((int)TextureScaleX << 16)| TextureScaleY))) ^
			(0 == (this.ElementMask & ElementMask.Texture) ? 0 : this.Texture.GetHashCode());

		public int HashComponentVertex => 0 == (this.ElementMask & ElementMaskUtility.Vertex) ? 0 : 
			ROT6(this.Vertex.GetHashCode());

		public int HashComponentIndex => 0 == (this.ElementMask & ElementMask.Index) ? 0 :
			ROT9(this.Index.GetHashCode()) - this.IndexCount;

		public int HashComponentMaterial =>
				0 == (this.FeatureMask & FeatureMask.Lit) ?
				ROT15(
					0 == (this.FeatureMask & FeatureMask.Fog) ?
					unchecked((int)Color.Value) :
					unchecked((int)(Color.Value + Fog.Value)^((int)FogOffset << 16|(int)FogMultiplier))) :
				ROT17(0 == (this.FeatureMask & FeatureMask.Fog) ?
				unchecked((int)(Diffuse.Value)) :
				unchecked((int)(Diffuse.Value + Fog.Value) ^ ((int)FogOffset << 16 | (int)FogMultiplier)));
		public int HashComponentLights =>
			(0 == (this.FeatureMask & FeatureMask.Lit) || 0 == ((byte)
			(LightModeUtility.Mask & this.LightMode) >> 1)) ? 0 :
			(
				(((int)Ambient.R << 24) | ((int)Ambient.G << 16) | ((int)Ambient.B << 8) / 5) ^
				GraphicsLight.GetHashCode(ref Light1) ^
				(1 == ((byte)(LightModeUtility.Mask & this.LightMode) >> 1) ? 0 :
				(GraphicsLight.GetHashCode(ref Light2, 3) ^
				(2 == ((byte)(LightModeUtility.Mask & this.LightMode) >> 1) ? 0 :
				(GraphicsLight.GetHashCode(ref Light3, 6) ^
				(3 == ((byte)(LightModeUtility.Mask & this.LightMode) >> 1) ? 0 :
				(GraphicsLight.GetHashCode(ref Light4, 9) ^
				(4 == ((byte)(LightModeUtility.Mask & this.LightMode) >> 1) ? 0 :
				(GraphicsLight.GetHashCode(ref Light5, 12) ^
				(5 == ((byte)(LightModeUtility.Mask & this.LightMode) >> 1) ? 0 :
				(GraphicsLight.GetHashCode(ref Light6, 15) ^
				(6 == ((byte)(LightModeUtility.Mask & this.LightMode) >> 1) ? 0 :
				(GraphicsLight.GetHashCode(ref Light7, 18))))))))))))));

		public static int GetHashCode(ref GraphicsState A)
		{
			return A.HashComponentRoot ^
				A.HashComponentTexture ^
				A.HashComponentVertex ^
				A.HashComponentIndex ^
				A.HashComponentMaterial ^
				A.HashComponentLights;
		}
		public override int GetHashCode()
		{
			return HashComponentRoot ^
				HashComponentTexture ^
				HashComponentVertex ^
				HashComponentIndex ^
				HashComponentMaterial ^
				HashComponentLights;
		}
		public static bool Equals(ref GraphicsState A, ref GraphicsState B)
		{
			return (A.ElementMask == B.ElementMask && A.FeatureMask == B.FeatureMask && A.Culling == B.Culling) &&
				(0 == (A.ElementMask & ElementMask.Color) ? 
					(0 != (A.FeatureMask & FeatureMask.Lit) || A.Color == B.Color)
					: (A.VertexColor == B.VertexColor)) &&
				(0 == (A.FeatureMask & FeatureMask.Fog) || (A.Fog.Value == B.Fog.Value && A.FogMultiplier == B.FogMultiplier && A.FogOffset == B.FogOffset)) &&
				(0 == (A.ElementMask & ElementMask.Texcoord) ||
				(A.TexturePresentation == B.TexturePresentation &&
				A.TextureWidth == B.TextureWidth &&
				A.TextureHeight == B.TextureHeight)) &&
				(0 == (A.ElementMask & ElementMask.Texture) || A.Texture == B.Texture) &&
				(0 == (A.FeatureMask & FeatureMask.Lit) || 
				(A.LightMode == B.LightMode &&
					A.Diffuse.Value == B.Diffuse.Value &&
					A.Ambient.R == B.Ambient.R &&
					A.Ambient.G == B.Ambient.G &&
					A.Ambient.B == B.Ambient.B &&
					(((byte)(A.LightMode&LightModeUtility.Mask)>>1)==0|| (
						GraphicsLight.Equals(ref A.Light1, ref B.Light1) &&
						(((byte)(A.LightMode & LightModeUtility.Mask) >> 1) == 1 ||(
							GraphicsLight.Equals(ref A.Light2, ref B.Light2) &&
							(((byte)(A.LightMode & LightModeUtility.Mask) >> 1)==2||(
								GraphicsLight.Equals(ref A.Light3, ref B.Light3) &&
								(((byte)(A.LightMode & LightModeUtility.Mask) >> 1) == 3 || (
									GraphicsLight.Equals(ref A.Light4, ref B.Light4) &&
								(((byte)(A.LightMode & LightModeUtility.Mask) >> 1) == 4 || (
									GraphicsLight.Equals(ref A.Light5, ref B.Light5) &&
								(((byte)(A.LightMode & LightModeUtility.Mask) >> 1) == 5 || (
									GraphicsLight.Equals(ref A.Light6, ref B.Light6) &&
								(((byte)(A.LightMode & LightModeUtility.Mask) >> 1) == 6 ||
									GraphicsLight.Equals(ref A.Light7, ref B.Light7) ))))))))))))
							)
						)) &&
				(0 == (A.ElementMask & (ElementMask.Position | ElementMask.Normal | ElementMask.Color | ElementMask.Texcoord)) ||
					A.Vertex == B.Vertex) &&
				(0 == (A.ElementMask & ElementMask.Index) ||
				(A.IndexInteger == B.IndexInteger && 
				A.IndexCount == B.IndexCount &&
				A.IndexPrimitive == B.IndexPrimitive &&
				A.Index == B.Index));
		}
		public static bool operator ==(GraphicsState L, GraphicsState R)
		{
			return Equals(ref L, ref R);
		}
		public static bool operator !=(GraphicsState L, GraphicsState R)
		{
			return !Equals(ref L, ref R);
		}
		public bool Equals(ref GraphicsState other) { return Equals(ref this, ref other); }
		public bool Equals(GraphicsState other) { return Equals(ref this, ref other); }
		public override bool Equals(object obj)
		{
			return obj is GraphicsState && ((GraphicsState)obj).Equals(ref this);
		}

		/// <summary>
		/// if no vertex related flags are present in the element mask, the Vertex handle is dereferenced.
		/// if no index related flags are present in the element mask, the Index handle is dereferenced.
		/// if no texture related flags are present in the element mask, the Texture handle is dereferenced.
		/// 
		/// if Vertex is either null or had been deleted, vertex related flags that are set are removed from this instance and returned.
		/// if Texture is either null or had been deleted, texture flag is removed from this instance and returned if it was set.
		/// if Index is either null or had been deleted, index flag is removed from this instance and returned if it was set.
		/// </summary>
		public static ElementMask SanitizeHandles(ref GraphicsState GraphicsState)
		{
			ElementMask Dropped = 0;
			if (0 == (GraphicsState.ElementMask & ElementMaskUtility.Vertex) ||
				GraphicsState.Vertex.Alive == GraphicsHandle.Null)
			{
				GraphicsState.Vertex = default(GraphicsHandle.Buffer);
				Dropped |= GraphicsState.ElementMask & ElementMaskUtility.Vertex;
				GraphicsState.ElementMask &= ElementMaskUtility.NonVertex;
			}

			if (0 == (GraphicsState.ElementMask & ElementMask.Index) ||
				GraphicsState.Index.Alive == GraphicsHandle.Null)
			{
				GraphicsState.Index = default(GraphicsHandle.Buffer);
				Dropped |= GraphicsState.ElementMask & ElementMask.Index;
				GraphicsState.ElementMask &= ElementMaskUtility.NonIndex;
			}
			if (0 == (GraphicsState.ElementMask & ElementMask.Texture) ||
				GraphicsState.Texture.Alive == GraphicsHandle.Null)
			{
				GraphicsState.Texture = default(GraphicsHandle.Texture);
				Dropped |= GraphicsState.ElementMask & ElementMask.Texture;
				GraphicsState.ElementMask &= ElementMaskUtility.NonTexture;
			}

			return Dropped;
		}

		/// <summary>
		/// when vertex colors are present, Color is set to white.
		/// when lighting is disabled, LightColor and DarkColor are set to defaults.
		/// </summary>
		public static void SanitizeColors(
			ref GraphicsState GraphicsState
			)
		{
			if (0 != (GraphicsState.ElementMask & ElementMask.Color))
			{
				GraphicsState.Color.Value = uint.MaxValue;
			}

			if(0 == (GraphicsState.FeatureMask & FeatureMask.Lit))
			{
				GraphicsState.Diffuse = Color4b.Default_Diffuse;
				GraphicsState.Ambient = Color4b.Default_Ambient;
			}
		}
		/// <summary>
		/// Ensures that all enum fields in the Graphics state are set to the values they represent.
		/// an example of a value being set to something that does not represent what it is would be when a texture preset specifies it is a Repeat on S value by declaring both MirrorS and ClampS flags (where this removes both flags, and represents the same value).
		/// </summary>
		public static void SanitizeEnums(ref GraphicsState GraphicsState)
		{
			IndexIntegerUtility.Sanitize(ref GraphicsState.IndexInteger);
			IndexPrimitiveUtility.Sanitize(ref GraphicsState.IndexPrimitive);
			LightModeUtility.Sanitize(ref GraphicsState.LightMode);
			FeatureMaskUtility.Sanitize(ref GraphicsState.FeatureMask);
			ElementMaskUtility.Sanitize(ref GraphicsState.ElementMask);
			CullingUtility.Sanitize(ref GraphicsState.Culling);
			VertexColorUtility.Sanitize(ref GraphicsState.VertexColor);
			TexturePresentationUtility.Sanitize(ref GraphicsState.TexturePresentation);
		}
		/// <summary>
		/// runs all sanitize functions. returns that of SanitizeHandles.
		/// </summary>
		public static ElementMask Sanitize(ref GraphicsState GraphicsState)
		{
			var Ret = SanitizeHandles(ref GraphicsState);
			SanitizeEnums(ref GraphicsState);
			SanitizeColors(ref GraphicsState);
			return Ret;
		}
		/// <summary>
		/// DOES NOT MODIFY THIS!
		/// Copies this to Target, then returns Sanitize(ref Target).
		/// </summary>
		public ElementMask CopyToSanitize(out GraphicsState Target)
		{
			Target = this;
			return Sanitize(ref Target);
		}
		public static void Copy([In] ref GraphicsState Source, out GraphicsState Target)
		{
			Target.Color = Source.Color;
			Target.Diffuse = Source.Diffuse;
			Target.Ambient = Source.Ambient;
			Target.ElementMask = Source.ElementMask;
			Target.Culling = Source.Culling;
			Target.FeatureMask = Source.FeatureMask;
			Target.Fog = Source.Fog;
			Target.FogMultiplier = Source.FogMultiplier;
			Target.FogOffset = Source.FogOffset;
			Target.Index = Source.Index;
			Target.IndexCount = Source.IndexCount;
			Target.IndexInteger = Source.IndexInteger;
			Target.IndexPrimitive = Source.IndexPrimitive;
			Target.LightMode = Source.LightMode;
			Target.Texture = Source.Texture;
			Target.TextureHeight = Source.TextureHeight;
			Target.TexturePresentation = Source.TexturePresentation;
			Target.TextureScaleX = Source.TextureScaleX;
			Target.TextureScaleY = Source.TextureScaleY;
			Target.TextureWidth = Source.TextureWidth;
			Target.Vertex = Source.Vertex;
			Target.VertexColor = Source.VertexColor;
			Target.Ambient = Source.Ambient;
			Target.Light1 = Source.Light1;
			Target.Light2 = Source.Light2;
			Target.Light3 = Source.Light3;
			Target.Light4 = Source.Light4;
			Target.Light5 = Source.Light5;
			Target.Light6 = Source.Light6;
			Target.Light7 = Source.Light7;
		}
		/// <summary>
		/// DOES NOT MODIFY Source!
		/// Copies Source to Target, then returns Sanitize(ref Target).
		/// </summary>
		public static ElementMask CopyToSanitize([In]ref GraphicsState Source, out GraphicsState Target)
		{
			Copy(ref Source, out Target);
			return Sanitize(ref Target);
		}
	}
	[StructLayout(LayoutKind.Explicit)]
	public abstract class GraphicsStateWrapper
	{
		[FieldOffset(0)]
		public readonly GraphicsState BoundState;

		[FieldOffset(0)]
		internal protected GraphicsState MutableBoundState;
	}
	public sealed class GraphicsInterface : GraphicsStateWrapper
	{
		public GraphicsState State;

		public Transform Camera = Transform.Identity;
		public Transform Model = Transform.Identity;

		public Matrix4 View = Matrix4.Identity;
		public Matrix4 Proj = Matrix4.Identity;
		public Matrix4 ViewProj = Matrix4.Identity;

		/// <summary>
		/// far and near planes (used for fog calculation only).
		/// </summary>
		public ushort FogFar=10, FogNear=10000;

		public readonly OpenTK.Graphics.IGraphicsContext GLContext;

		public GraphicsInterface(
			bool SkipGet = false
			)
		{
			GLContext = OpenTK.Graphics.GraphicsContext.CurrentContext;
			this.Refresh(SkipGet: SkipGet);
		}

	}

	public static class GraphicsInterfaceUtility
	{
		private enum Toggle : byte { }

		private const Toggle True = (Toggle)1;
		private const Toggle Unchanged = 0;
		private const Toggle Changed = (Toggle)2;
		private const Toggle False = 0;
		private const Toggle TurnedOn = True|Changed;
		private const Toggle TurnedOff = False|Changed;

		private const LightName LightName1 = LightName.Light0;
		private const LightName LightName2 = (LightName)((int)LightName1+1);
		private const LightName LightName3 = (LightName)((int)LightName1+2);
		private const LightName LightName4 = (LightName)((int)LightName1+3);
		private const LightName LightName5 = (LightName)((int)LightName1+4);
		private const LightName LightName6 = (LightName)((int)LightName1+5);
		private const LightName LightName7 = (LightName)((int)LightName1+6);

		private const EnableCap LightCap1 = EnableCap.Light0;
		private const EnableCap LightCap2 = (EnableCap)((int)LightCap1 + 1);
		private const EnableCap LightCap3 = (EnableCap)((int)LightCap1 + 2);
		private const EnableCap LightCap4 = (EnableCap)((int)LightCap1 + 3);
		private const EnableCap LightCap5 = (EnableCap)((int)LightCap1 + 4);
		private const EnableCap LightCap6 = (EnableCap)((int)LightCap1 + 5);
		private const EnableCap LightCap7 = (EnableCap)((int)LightCap1 + 6);

		private static Toggle ToggleBit(ref GraphicsState Target, ref GraphicsState Current,
			FeatureMask Bit)
		{
			Toggle Tog;
			if (0 == ((Target.FeatureMask ^ Current.FeatureMask) & Bit))
				Tog = 0 == (Target.FeatureMask & Bit) ? False : True;
			else
			{
				Current.FeatureMask ^= Bit;
				Tog = 0 == (Target.FeatureMask & Bit) ? TurnedOff : TurnedOn;
			}
			return Tog;
		}
		private static Toggle ToggleBit(
			ref GraphicsState Target,
			ref GraphicsState Current,
			ElementMask Bit)
		{
			Toggle Tog;
			if (0 == ((Target.ElementMask ^ Current.ElementMask) & Bit))
				Tog = 0 == (Target.ElementMask & Bit) ? False : True;
			else
			{
				Current.ElementMask ^= Bit;
				Tog = 0 == (Target.ElementMask & Bit) ? TurnedOff : TurnedOn;
			}
			return Tog;
		}
		private static void Cap(
			EnableCap Cap,
			Toggle Tog)
		{
			if (!(0 == (Tog & Changed)))
				if (0 == (Tog & True))
					GL.Disable(Cap);
				else
					GL.Enable(Cap);
		}
		private static void Cap(
			ArrayCap Cap,
			Toggle Tog)
		{
			if (!(0 == (Tog & Changed)))
				if (0 == (Tog & True))
					GL.DisableClientState(Cap);
				else
					GL.EnableClientState(Cap);
		}
		private static void Unbind(ElementMask Masks)
		{
			if (0 != (Masks & ElementMask.Color))
				GL.DisableClientState(ArrayCap.ColorArray);

			if (0 != (Masks & ElementMask.Index))
				GL.DisableClientState(ArrayCap.IndexArray);

			if (0 != (Masks & ElementMask.Texture))
				GL.Disable(EnableCap.Texture2D);

			if (0 != (Masks & ElementMask.Texcoord))
				GL.DisableClientState(ArrayCap.TextureCoordArray);

			if (0 != (Masks & ElementMask.Normal))
				GL.DisableClientState(ArrayCap.NormalArray);

			if (0 != (Masks & ElementMask.Position))
				GL.DisableClientState(ArrayCap.VertexArray);
		}
		private static Toggle ColorToggle(ref Color4b Target, ref Color4b Bound)
		{
			Toggle O = Target.A==0?False:True;

			if(Target.Value != Bound.Value)
			{
				Bound = Target;
				O |= Changed;
			}
			return O;
		}
		private static void LightToggle(
			ref GraphicsLight Target,
			ref GraphicsLight Bound,
			LightName LightName,
			Toggle ForceToggle)
		{
			if(0!=(Changed & (ColorToggle(ref Target.Color,ref Bound.Color) | ForceToggle)))
				Bound.Color.GL_LoadLight(LightColor, LightName);

			if (0 != (ForceToggle & Changed) ||
				(Target.Normal.X != Bound.Normal.X ||
				Target.Normal.Y != Bound.Normal.Y ||
				Target.Normal.Z != Bound.Normal.Z))
			{
				Bound.Normal = Target.Normal;

				var Restore = GL.GetInteger(GetPName.MatrixMode);
				GL.MatrixMode(MatrixMode.Modelview);
				GL.PushMatrix();
				GL.LoadIdentity();
				GL.Light(LightName, LightParameter.Position, new Vector4(Bound.Normal.X, Bound.Normal.Y, Bound.Normal.Z, 0f).Normalized());
				GL.PopMatrix();
				GL.MatrixMode((MatrixMode)Restore);
			}
		}

		private static readonly IntPtr
			PositionOffset = (IntPtr)0,
			TexcoordOffset = (IntPtr)8,
			ColorOffset = (IntPtr)12,
			NormalOffset = (IntPtr)12,
			AlphaOffset = (IntPtr)15;

		private static void Bind(
			GraphicsInterface gi,
			ref GraphicsState Target,
			ref GraphicsState Bound,
			Toggle ForceToggle)
		{
			int Restore;
			Toggle Tog;

			Cap(EnableCap.Blend,
				Tog = ToggleBit(ref Target, ref Bound, FeatureMask.AlphaBlend)
				| ForceToggle
				);

			Cap(EnableCap.PolygonOffsetFill,
				Tog = ToggleBit(ref Target, ref Bound, FeatureMask.PolygonOffset) 
				| ForceToggle
				);

			Cap(EnableCap.DepthTest,
				Tog = ToggleBit(ref Target, ref Bound, FeatureMask.ZTest)
				| ForceToggle);

			Cap(EnableCap.Fog,
				Tog = ToggleBit(ref Target, ref Bound, FeatureMask.Fog)
				| ForceToggle);
			if (0 != (Tog & True))
			{
				if (
					(0 != (ForceToggle & Changed) ||
					Bound.Fog.Value != Target.Fog.Value))
						(Bound.Fog = Target.Fog).GL_LoadFogColor();

				if ((0!= (ForceToggle & Changed) || 
					Bound.FogMultiplier != Target.FogMultiplier ||
					Bound.FogOffset != Target.FogOffset))
				{
					Bound.FogMultiplier = Target.FogMultiplier;
					Bound.FogOffset = Target.FogOffset;

					GL.Fog(FogParameter.FogStart,
						gi.FogNear +
						((gi.FogFar - gi.FogNear) * new Fixed16_16 { Value = Bound.FogOffset }.Single));

					GL.Fog(FogParameter.FogEnd,
						(gi.FogNear +
						((gi.FogFar - gi.FogNear) * new Fixed16_16 { Value = Bound.FogOffset }.Single))
						+(((gi.FogFar - gi.FogNear) * Bound.FogMultiplier) / 128000f));

					GL.Fog(FogParameter.FogMode, (int)FogMode.Linear);
				}
			}
			

			Cap(EnableCap.Lighting,
				Tog = ToggleBit(ref Target, ref Bound, FeatureMask.Lit) | ForceToggle);

			if (0 != (Tog & True))
			{
				if ((0 != (ForceToggle & Changed)) ||
					((LightMode.Smooth0 & Bound.LightMode) != (LightMode.Smooth0 & Target.LightMode))
					)
				{
					if (0 == (Target.LightMode & LightMode.Smooth0))
						Bound.LightMode &= ~LightMode.Smooth0;
					else
						Bound.LightMode |= LightMode.Smooth0;
					GL.ShadeModel(
						0 == (Bound.LightMode & LightMode.Smooth0) ?
						ShadingModel.Flat : ShadingModel.Smooth);
				}

				Tog = ColorToggle(ref Target.Ambient, ref Bound.Ambient)
					| ForceToggle;

				if (0 != (Tog & Changed))
					Bound.Ambient.GL_LoadMaterial(MaterialParameter: MaterialParameter.Ambient);

				Tog = ColorToggle(ref Target.Diffuse, ref Bound.Diffuse)
					| ForceToggle;

				if (0 != (Tog & Changed))
					Bound.Diffuse.GL_LoadMaterial(MaterialParameter: MaterialParameter.Diffuse);


				Restore = Target.LightMode.LightCount();

				if (0 != (ForceToggle & Changed))
				{
					Bound.LightMode = Target.LightMode;
					if(Restore == 0)
					{
						GL.Disable(LightCap1);
						GL.Disable(LightCap2);
						GL.Disable(LightCap3);
						GL.Disable(LightCap4);
						GL.Disable(LightCap5);
						GL.Disable(LightCap6);
						GL.Disable(LightCap7);
					}
					else
					{
						GL.Enable(LightCap1);
						if (Restore == 1)
						{
							GL.Disable(LightCap2);
							GL.Disable(LightCap3);
							GL.Disable(LightCap4);
							GL.Disable(LightCap5);
							GL.Disable(LightCap6);
							GL.Disable(LightCap7);
						}
						else
						{
							GL.Enable(LightCap2);
							if (Restore == 2)
							{
								GL.Disable(LightCap3);
								GL.Disable(LightCap4);
								GL.Disable(LightCap5);
								GL.Disable(LightCap6);
								GL.Disable(LightCap7);
							}
							else
							{
								GL.Enable(LightCap3);
								if (Restore == 3)
								{
									GL.Disable(LightCap4);
									GL.Disable(LightCap5);
									GL.Disable(LightCap6);
									GL.Disable(LightCap7);
								}
								else
								{
									GL.Enable(LightCap4);
									if (Restore == 4)
									{
										GL.Disable(LightCap5);
										GL.Disable(LightCap6);
										GL.Disable(LightCap7);
									}
									else
									{
										GL.Enable(LightCap5);
										if (Restore == 5)
										{
											GL.Disable(LightCap6);
											GL.Disable(LightCap7);
										}
										else
										{
											GL.Enable(LightCap6);
											if (Restore == 6)
												GL.Disable(LightCap7);
											else
												GL.Enable(LightCap7);
										}
									}
								}
							}
						}
					}
				}
				else if (Bound.LightMode < Target.LightMode)
				{
					do
					{
						GL.Enable((EnableCap)((int)LightCap1 + (((byte)Bound.LightMode) >> 1)));
						Bound.LightMode = (LightMode)((byte)Bound.LightMode + 2);
					} while (Bound.LightMode < Target.LightMode);
				}
				else if (Bound.LightMode > Target.LightMode)
				{
					do
					{
						GL.Disable((EnableCap)((int)LightCap1 + (((byte)Bound.LightMode) >> 1)));
						Bound.LightMode = (LightMode)((byte)Bound.LightMode - 2);
					} while (Bound.LightMode > Target.LightMode);
				}

				if (Restore > 0) LightToggle(ref Target.Light1, ref Bound.Light1, LightName1, ForceToggle);
				if (Restore > 1) LightToggle(ref Target.Light2, ref Bound.Light2, LightName2, ForceToggle);
				if (Restore > 2) LightToggle(ref Target.Light3, ref Bound.Light3, LightName3, ForceToggle);
				if (Restore > 3) LightToggle(ref Target.Light4, ref Bound.Light4, LightName4, ForceToggle);
				if (Restore > 4) LightToggle(ref Target.Light5, ref Bound.Light5, LightName5, ForceToggle);
				if (Restore > 5) LightToggle(ref Target.Light6, ref Bound.Light6, LightName6, ForceToggle);
				if (Restore > 6) LightToggle(ref Target.Light7, ref Bound.Light7, LightName7, ForceToggle);
			}

			Cap(EnableCap.CullFace, 
				Tog =
					(Target.Culling == Culling.Off ? False : True) |
					( (Target.Culling == Culling.Off ^
					  Bound.Culling == Culling.Off) ? Changed : Unchanged) 
					| ForceToggle);

			if (0 != (Tog & True) &&
				(0 != (ForceToggle & Changed) || 
				Bound.Culling != Target.Culling))
				GL.CullFace(
					Bound.Culling == Culling.Both ?
						CullFaceMode.FrontAndBack
					: Bound.Culling == Culling.Front ?
						CullFaceMode.Front
					: CullFaceMode.Back);

			Bound.Culling = Target.Culling;


			Tog = ToggleBit(
				ref Target, 
				ref Bound,
				FeatureMask.ZWrite) | ForceToggle;

			if (0 != (Tog & Changed))
				GL.DepthMask(0 != (Tog & True));


			Cap(EnableCap.Texture2D,
				Tog = ToggleBit(
					ref Target,
					ref Bound,
					ElementMask.Texture) | ForceToggle);

			if (0 != (Tog & True) &&
				(0!=(ForceToggle & Changed) ||
					(Bound.TexturePresentation!=Target.TexturePresentation || Bound.Texture != Target.Texture)))
			{
				Bound.Texture = Target.Texture;
				Bound.TexturePresentation = Target.TexturePresentation;
				GL.BindTexture(TextureTarget.Texture2D, Bound.Texture);

				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
					(int)Bound.TexturePresentation.GetWrapS().Select(TextureWrapMode.Repeat, TextureWrapMode.MirroredRepeat, TextureWrapMode.ClampToEdge));

				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
					(int)Bound.TexturePresentation.GetWrapT().Select(TextureWrapMode.Repeat, TextureWrapMode.MirroredRepeat, TextureWrapMode.ClampToEdge));
				
			} else
				Bound.Texture = Target.Texture;


			if (0 != (ForceToggle & Changed) || Target.Vertex != Bound.Vertex)
			{
				Bound.Vertex = Target.Vertex;

				if (Bound.Vertex.Alive != GraphicsHandle.Null)
				{
					GL.BindBuffer(BufferTarget.ArrayBuffer, Target.Vertex);


					// make every vertex element the target has treated as missing in the bound buffer for now.
					Bound.ElementMask ^=
						(Bound.ElementMask & Target.ElementMask)
						& ElementMaskUtility.Vertex;
				}

			}
			else
				Bound.Vertex = Target.Vertex;

			Cap(ArrayCap.VertexArray,
				Tog = ToggleBit(ref Target, ref Bound, ElementMask.Position)
				| ForceToggle);

			if (TurnedOn == Tog)
				GL.VertexPointer(3, VertexPointerType.Short, 16, PositionOffset);

			Cap(ArrayCap.TextureCoordArray,
				Tog = ToggleBit(ref Target, ref Bound, ElementMask.Texcoord) 
				| ForceToggle);

			if (TurnedOn == Tog)
				GL.TexCoordPointer(2, TexCoordPointerType.Short, 16,TexcoordOffset);

			if (0 != (Tog & True) &&
				(0 != (ForceToggle & Changed) ||
					(Bound.TextureWidth != Target.TextureWidth ||
					Bound.TextureHeight != Target.TextureHeight ||
					Bound.TextureScaleX != Target.TextureScaleX ||
					Bound.TextureScaleY != Target.TextureScaleY)))
			{
				Restore = GL.GetInteger(GetPName.MatrixMode);

				if (Restore != (int)MatrixMode.Texture)
					GL.MatrixMode(MatrixMode.Texture);

				GL.LoadIdentity();

				Bound.TextureWidth = Target.TextureWidth;
				Bound.TextureHeight = Target.TextureHeight;
				Bound.TextureScaleX = Target.TextureScaleX;
				Bound.TextureScaleY = Target.TextureScaleY;
				GL.Scale(
					(new Fixed16_16
					{
						Value = 1u + Bound.TextureScaleX,
					}.Double / ((int)Bound.TextureWidth << 5)),
					(new Fixed16_16
					{
						Value = 1u + Bound.TextureScaleY,
					}.Double / ((int)Bound.TextureHeight << 5)),
					1.0);

				if (Restore != (int)MatrixMode.Texture)
					GL.MatrixMode((MatrixMode)Restore);
			}

			Cap(ArrayCap.NormalArray,
				Tog = ToggleBit(ref Target, ref Bound, ElementMask.Normal) 
				| ForceToggle);

			if (TurnedOn == Tog)
			{
				GL.NormalPointer(NormalPointerType.Byte, 16, NormalOffset);
				GL.Disable(EnableCap.AutoNormal);
				GL.Enable(EnableCap.RescaleNormal);
			}
			else if (TurnedOff == Tog)
			{
				GL.Enable(EnableCap.AutoNormal);
				GL.Disable(EnableCap.RescaleNormal);
			}


			/*
			if (0 == (Tog & True))
				GL.Enable(EnableCap.AutoNormal);
			else
				GL.Disable(EnableCap.AutoNormal);*/
			
			Cap(ArrayCap.ColorArray,
				Tog = ToggleBit(ref Target, ref Bound, ElementMask.Color) 
				| ForceToggle);

			if (TurnedOn == Tog)
				GL.ColorPointer(3, ColorPointerType.UnsignedByte, 16, ColorOffset);
			
			Cap(ArrayCap.IndexArray,
				Tog = ToggleBit(ref Target, ref Bound, ElementMask.Index) 
				| ForceToggle);

			if (0 != (Tog & True) && 
				(0!=(ForceToggle & Changed)||
				Target.Index != Bound.Index))
			{
				Tog |= Changed;

				Bound.Index = Target.Index;
				if (GraphicsHandle.Null != Target.Index.Alive)
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, Bound.Index);
			}else // still need to do this.
				Bound.Index = Target.Index;
			

			Bound.IndexCount = Target.IndexCount;
			Bound.IndexInteger = Target.IndexInteger;
			Bound.IndexPrimitive = Target.IndexPrimitive;
			if (0 != (ForceToggle & Changed) ||
				Target.Color != Bound.Color)
			{
				Bound.Color = Target.Color;
				Bound.Color.GL_Load();
			}

		}
		private static Vector4 LightDir(ref Quaternion q, ref Vector4c n)
		{
			return new Vector4((q*new Vector3(n.X, n.Y, n.Z)).Normalized(), 0f);
		}
		public static void Redraw(
			this GraphicsInterface GraphicsInterface
			)
		{
			int LightNumber;

			using (new ContextLock(GraphicsInterface.GLContext))
			{
				
				if ((ElementMask.Index | ElementMask.Position) ==
				(GraphicsInterface.MutableBoundState.ElementMask & (ElementMask.Index | ElementMask.Position))
				&& GraphicsInterface.MutableBoundState.IndexCount != 0)
				{/*
					GL.GetFloat(GetPName.ModelviewMatrix, out Matrix4 mv);
					var Q = (GraphicsInterface.Camera.rotation * mv.ExtractRotation().Normalized()) * 
						GraphicsInterface.Camera.rotation;

					if (0 != (GraphicsInterface.MutableBoundState.FeatureMask & FeatureMask.Lit) &&
						0 != (LightNumber = ((byte)(GraphicsInterface.MutableBoundState.LightMode & LightModeUtility.Mask) >> 1)))
					{
						do
						{
							GL.Light(LightName1, LightParameter.Position, LightDir(ref Q,ref
									GraphicsInterface.MutableBoundState.Light1.Normal
									));
							if (0 == --LightNumber) break;
							GL.Light(LightName2, LightParameter.Position, LightDir(ref Q, ref
									GraphicsInterface.MutableBoundState.Light3.Normal
									));
							if (0 == --LightNumber) break;
							GL.Light(LightName3, LightParameter.Position, LightDir(ref Q, ref
									GraphicsInterface.MutableBoundState.Light3.Normal
									));
							if (0 == --LightNumber) break;
							GL.Light(LightName4, LightParameter.Position, LightDir(ref Q, ref
									GraphicsInterface.MutableBoundState.Light4.Normal
									));
							if (0 == --LightNumber) break;
							GL.Light(LightName5, LightParameter.Position, LightDir(ref Q, ref
									GraphicsInterface.MutableBoundState.Light5.Normal
									));
							if (0 == --LightNumber) break;
							GL.Light(LightName6, LightParameter.Position, LightDir(ref Q, ref
									GraphicsInterface.MutableBoundState.Light6.Normal
									));
							if (0 == --LightNumber) break;
							GL.Light(LightName7, LightParameter.Position, LightDir(ref Q, ref
									GraphicsInterface.MutableBoundState.Light7.Normal
									));
						} while (false);
					}
					*/
					GL.DrawElements(
						GraphicsInterface.MutableBoundState.IndexPrimitive.ToGL(),
						GraphicsInterface.MutableBoundState.IndexCount,
						GraphicsInterface.MutableBoundState.IndexInteger == IndexInteger.Byte ? DrawElementsType.UnsignedByte
						: GraphicsInterface.MutableBoundState.IndexInteger == IndexInteger.Short ? DrawElementsType.UnsignedShort
						: DrawElementsType.UnsignedInt, IntPtr.Zero);
				}
			}
		}
		public static void Bind(
			this GraphicsInterface GraphicsInterface,
			bool ForceInvalidate = false,
			bool Draw = false)
		{
			using (new ContextLock(GraphicsInterface.GLContext))
			{
				Unbind(
					GraphicsState.SanitizeHandles(ref GraphicsInterface.MutableBoundState)
					);

				GraphicsState.Sanitize(ref GraphicsInterface.State);

				if(ForceInvalidate || 
					!GraphicsState.Equals(
						ref GraphicsInterface.MutableBoundState,
						ref GraphicsInterface.State))
					Bind(
						GraphicsInterface,
						ref GraphicsInterface.State,
						ref GraphicsInterface.MutableBoundState,
						ForceInvalidate ? Changed : (Toggle)0);

				if (Draw)
					Redraw(GraphicsInterface);
			}
		}
		public static void GetFromGL(out ElementMask Mask)
		{
			Mask = 0;
			if (GL.GetBoolean(GetPName.ColorArray)) Mask |= ElementMask.Color;
			if (GL.GetBoolean(GetPName.NormalArray)) Mask |= ElementMask.Normal;
			if (GL.GetBoolean(GetPName.TextureCoordArray)) Mask |= ElementMask.Color;
			if (GL.GetBoolean(GetPName.VertexArray)) Mask |= ElementMask.Position;

			if (0!=GL.GetInteger(GetPName.ElementArrayBufferBinding)) Mask |= ElementMask.Index;
			if (GL.GetBoolean(GetPName.Texture2D)) Mask |= ElementMask.Texture;
		}
		public static void GetFromGL(out FeatureMask Mask)
		{
			Mask = 0;
			if (GL.GetBoolean(GetPName.DepthWritemask)) Mask |= FeatureMask.ZWrite;
			if (GL.GetBoolean(GetPName.DepthTest)) Mask |= FeatureMask.ZTest;
			if (GL.GetBoolean(GetPName.PolygonOffsetFill)) Mask |= FeatureMask.PolygonOffset;
			if (GL.GetBoolean(GetPName.Lighting)) Mask |= FeatureMask.Lit;
			if (GL.GetBoolean(GetPName.Fog)) Mask |= FeatureMask.Fog;
			if (GL.GetBoolean(GetPName.Blend)) Mask |= FeatureMask.AlphaBlend;
			//if (-1 != GL.GetInteger(GetPName.CurrentColor)) Mask |= FeatureMask.Tint;
		}
		public static void GetFromGL(out VertexColor Value)
		{
			//TODO
			Value = VertexColor.Smooth;
		}
		public static void GetFromGL(out Culling Value)
		{
			int V;
			Value = GL.GetBoolean(GetPName.CullFace) ?
				(V = GL.GetInteger(GetPName.CullFaceMode)) == (int)CullFaceMode.Front ? Culling.Front :
				V == (int)CullFaceMode.FrontAndBack ? Culling.Both : Culling.Back :
				Culling.Off;
		}
		public static void GetFromGL(out LightMode Value)
		{
			if (GL.GetBoolean(GetPName.Light0))
			{
				if (GL.GetBoolean(GetPName.Light1))
				{
					if (GL.GetBoolean(GetPName.Light2))
					{
						if (GL.GetBoolean(GetPName.Light3))
						{
							if (GL.GetBoolean(GetPName.Light4))
							{
								if (GL.GetBoolean(GetPName.Light5))
								{
									if (GL.GetBoolean(GetPName.Light6))
									{
										Value = LightMode.Hard7;
									}
									else
									{
										Value = LightMode.Hard6;
									}
								}
								else
								{
									Value = LightMode.Hard5;
								}
							}
							else
							{
								Value = LightMode.Hard4;
							}
						}
						else
						{
							Value = LightMode.Hard3;
						}
					}
					else
					{
						Value = LightMode.Hard2;
					}
				}
				else
				{
					Value = LightMode.Hard1;
				}
			}
			else
			{
				Value = LightMode.Hard0;
			}

			if (GL.GetInteger(GetPName.ShadeModel) == (int)ShadingModel.Smooth )
				Value |= LightMode.Smooth0;
		}
		public const LightParameter LightColor = LightParameter.Diffuse;
		public const LightParameter LightAmbientColor = LightParameter.Ambient;

		public unsafe static void GetFromGL(out GraphicsLight Light, int Index)
		{
			Light.Value = 0;

			float* f = stackalloc float[4];

			GL.GetLight((LightName)((int)LightName.Light0 + Index), LightColor, f);
			Light.Color = new Color4b { r = f[0], g = f[1], b = f[2], A = 255, };

			GL.GetLight((LightName)((int)LightName.Light0 + Index), LightParameter.Position, f);
			Light.Normal = new Vector4c
			{
				X = ByteUtility.FloatToNormal(f[0]),
				Y = ByteUtility.FloatToNormal(f[1]),
				Z = ByteUtility.FloatToNormal(f[2]),
				W = 0,
			};
		}
		public unsafe static void GetFromGL(out Color4b LightAmbient, int Index)
		{
			float* f = stackalloc float[4];
			GL.GetLight((LightName)((int)LightName.Light0 + Index), LightAmbientColor, f);
			LightAmbient = new Color4b { r = f[0], g = f[1], b = f[2], A = 255, };
		}
		public static void GetFromGL(out TexturePresentation Value)
		{
			Value = 0;

			int temp;
			GL.GetTexParameter(TextureTarget.Texture2D, GetTextureParameter.TextureWrapS, out temp);
			if ((int)TextureWrapMode.ClampToEdge == temp)
				Value |= TexturePresentation.ClampS;
			else if ((int)TextureWrapMode.MirroredRepeat == temp)
				Value |= TexturePresentation.MirrorS;

			GL.GetTexParameter(TextureTarget.Texture2D, GetTextureParameter.TextureWrapT, out temp);
			if ((int)TextureWrapMode.ClampToEdge == temp)
				Value |= TexturePresentation.ClampT;
			else if ((int)TextureWrapMode.MirroredRepeat == temp)
				Value |= TexturePresentation.MirrorT;
		}

		/// <summary>
		/// attempts to get state from GL. 
		/// if any buffer or texture changed, they will be set to zero (since we do not do a reverse lookup beyond the int), but we will retain links to existing ones if the id matches.
		/// 
		/// Does not touch IndexCount, IndexInteger or IndexPrimitive!
		/// </summary>
		public unsafe static void GetFromGL(ref GraphicsState State)
		{
			float* C = stackalloc float[4];
			int Temp;
			Temp = GL.GetInteger(GetPName.TextureBinding2D);
			if(State.Texture.Alive != Temp)
			{
				State.Texture = default(GraphicsHandle.Texture);
			}
			Temp = GL.GetInteger(GetPName.VertexArrayBinding);
			if (State.Vertex.Alive != Temp)
			{
				State.Vertex = default(GraphicsHandle.Buffer);
			}
			Temp = GL.GetInteger(GetPName.ElementArrayBufferBinding);
			if (State.Index.Alive != Temp)
			{
				State.Index = default(GraphicsHandle.Buffer);
			}
			GetFromGL(out State.ElementMask);
			GetFromGL(out State.FeatureMask);
			GetFromGL(out State.Culling);
			GetFromGL(out State.VertexColor);
			GetFromGL(out State.TexturePresentation);
			GetFromGL(out State.LightMode);
			GetFromGL(out State.Light1, 0);
			GetFromGL(out State.Light2, 1);
			GetFromGL(out State.Light3, 2);
			GetFromGL(out State.Light4, 3);
			GetFromGL(out State.Light5, 4);
			GetFromGL(out State.Light6, 5);
			GetFromGL(out State.Light7, 6);
			GetFromGL(out State.Ambient, 0);
			GraphicsState.Sanitize(ref State);

			{
				GL.GetFloat(GetPName.CurrentColor, out Vector4 c);
				State.Diffuse.r = c.X;
				State.Diffuse.g = c.Y;
				State.Diffuse.b = c.Z;
				State.Diffuse.a = c.W;
			}
			GL.GetMaterial(MaterialFace.Front, MaterialParameter.Ambient, C);
			State.Diffuse.r = C[0];
			State.Diffuse.g = C[1];
			State.Diffuse.b = C[2];
			State.Diffuse.a = C[3];

			GL.GetMaterial(MaterialFace.Front, MaterialParameter.Diffuse, C);
			State.Diffuse.r = C[0];
			State.Diffuse.g = C[1];
			State.Diffuse.b = C[2];
			State.Diffuse.a = C[3];

			{
				GL.GetFloat(GetPName.FogColor, out Vector4 c);
				State.Fog.r = c.X;
				State.Fog.g = c.Y;
				State.Fog.b = c.Z;
				State.Fog.a = c.W;
			}
		}
		private struct ContextLock : System.IDisposable
		{
			public OpenTK.Graphics.IGraphicsContext Restore;
			public void Dispose()
			{
				if (null != Restore) throw new System.InvalidOperationException("TODO");
			}
			public ContextLock( OpenTK.Graphics.IGraphicsContext Replace)
			{
				if (null == Replace)
					Restore = null;
				else
				{
					Restore = OpenTK.Graphics.GraphicsContext.CurrentContext;
					if (Restore != Replace && Restore != null)
						throw new System.InvalidOperationException("TODO");
					Restore = null;
				}
			}
		}
		public static void Refresh(
			this GraphicsInterface GraphicsInterface, 
			bool SkipGet = false, bool SkipSetState = false)
		{
			using (new ContextLock(GraphicsInterface.GLContext))
			{
				if (!SkipGet)
				{
					GetFromGL(ref GraphicsInterface.MutableBoundState);
				}
				else
				{
					Unbind(GraphicsState.SanitizeHandles(ref GraphicsInterface.MutableBoundState));
				}
				if (!SkipSetState)
				{
					GraphicsInterface.State = GraphicsInterface.MutableBoundState;
				}
				else
				{
					GraphicsState.Sanitize(ref GraphicsInterface.State);
				}
			}
		}
		public static void Draw(
			this GraphicsInterface GraphicsInterface,
			ref GraphicsState State,
			DrawOptions Options = 0)
		{
			if (0 == (Options & DrawOptions.NoGetState))
				GraphicsInterface.State = State;

			if (0 != (Options & DrawOptions.NoBuffers))
				GraphicsInterface.State.ElementMask = 0;

			if (0 != (Options & DrawOptions.NoCullingChanges))
				GraphicsInterface.State.Culling = GraphicsInterface.BoundState.Culling;


			if (0 != (Options & DrawOptions.NoMaterial))
			{
				GraphicsInterface.State.FeatureMask = 0;
				GraphicsInterface.State.TexturePresentation = GraphicsInterface.BoundState.TexturePresentation;
				GraphicsInterface.State.ElementMask &= ~ElementMask.Texture;
			}

			if (0 == (Options & DrawOptions.NoBind) ||
				0 != (Options & DrawOptions.ForceInvalidate))
			{
				Bind(GraphicsInterface: GraphicsInterface, 
					ForceInvalidate: 0 != (Options & DrawOptions.ForceInvalidate), 
					Draw: 0 == (Options & DrawOptions.NoTriangles));
			}
			else if (0 == (Options & DrawOptions.NoTriangles))
			{
				Redraw(GraphicsInterface: GraphicsInterface);
			}
		}
	}

}
