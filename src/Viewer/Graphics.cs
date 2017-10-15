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
	public enum Culling : byte
	{
		Off,
		Front,
		Back,
		Both,
	}
	public enum LightMode : byte
	{
		Hard,
		Smooth,
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
	}
	public static class ElementMaskUtility
	{
		public const ElementMask Vertex = ElementMask.Color | ElementMask.Position | ElementMask.Texcoord | ElementMask.Normal;
		public const ElementMask NormalColor = ElementMask.Normal | ElementMask.Color;
		public const ElementMask All = Vertex | ElementMask.Index;

		public const ElementMask NonVertex = (ElementMask)((~((byte)Vertex)) & (byte)All);
		public const ElementMask NonIndex = (ElementMask)((~((byte)ElementMask.Index)) & (byte)All);
		public const ElementMask NonTexture = (ElementMask)((~((byte)ElementMask.Texture)) & (byte)All);

	}
	[Flags]
	public enum FeatureMask : byte
	{
		ZWrite = 1,
		PolygonOffset = 2,
		AlphaBlend = 4,
		ZTest = 8,
		Tint = 16,
		Lit = 32,
		Fog = 64,
	}
	public enum VertexColor : byte
	{
		Smooth,
		A,
		B,
		C = 4,
	}
	public enum TexturePresentation : byte
	{
		P0RR, L0RR, PGRR, LGRR,
		P0XR, L0XR, PMXR, LMXR,
		P0CR, L0CR, PGCR, LGCR,

		P0RX = 16, L0RX, PGRX, LGRX,
		P0XX, L0XX, PMXX, LMXX,
		P0CX, L0CX, PGCX, LGCX,

		P0RC = 32, L0RC, PGRC, LGRC,
		P0XC, L0XC, PMXC, LMXC,
		P0CC, L0CC, PGCC, LGCC,
	}
	public static class TexturePresentationUtility
	{
		public const TexturePresentation WrapS_Mirror = TexturePresentation.P0XR;
		public const TexturePresentation WrapS_Clamp = TexturePresentation.P0CR;

		public const TexturePresentation WrapT_Mirror = TexturePresentation.P0RX;
		public const TexturePresentation WrapT_Clamp = TexturePresentation.P0RC;

		public const TexturePresentation WrapMaskS = WrapS_Mirror | WrapS_Clamp;
		public const TexturePresentation WrapMaskT = WrapT_Mirror | WrapT_Clamp;
		public const TexturePresentation WrapMask = WrapMaskS | WrapMaskT;
		public const TexturePresentation TexGenLinearMask = TexturePresentation.L0RR;
		public const TexturePresentation TexGenLinear = TexGenLinearMask;


		public const TexturePresentation TexGenSphericalMask = TexturePresentation.PGRR;
		public const TexturePresentation TexGenSpherical = TexGenSphericalMask;



		public static TexturePresentation GetSanitized(this TexturePresentation TexturePresentation)
		{
			Sanitize(ref TexturePresentation);
			return TexturePresentation;
		}

		public static void Sanitize(ref TexturePresentation TexturePresentation)
		{
			if (WrapMaskS == (WrapMaskS & TexturePresentation))
				if (WrapMaskT == (WrapMaskT & TexturePresentation))
					TexturePresentation ^= WrapMask;
				else
					TexturePresentation ^= WrapMaskS;
			else if (WrapMaskT == (WrapMaskT & TexturePresentation))
				TexturePresentation ^= WrapMaskT;
		}
		public static bool IsTexGenLinear(this TexturePresentation TexturePresentation)
		{
			return TexGenLinear == (TexturePresentation & TexGenLinearMask);
		}
		public static bool IsTexGenSpherical(this TexturePresentation TexturePresentation)
		{
			return TexGenSpherical == (TexturePresentation & TexGenSphericalMask);
		}
		public static T GetWrapS<T>(this TexturePresentation TexturePresentation, T Repeat, T Mirror, T Clamp)
		{
			return 0 == (TexturePresentation & WrapS_Clamp) ?
				0 == (TexturePresentation & WrapS_Mirror) ? Repeat : Mirror :
				0 == (TexturePresentation & WrapS_Mirror) ? Clamp : Repeat;
		}
		public static T GetWrapT<T>(this TexturePresentation TexturePresentation, T Repeat, T Mirror, T Clamp)
		{
			return 0 == (TexturePresentation & WrapT_Clamp) ?
				0 == (TexturePresentation & WrapT_Mirror) ? Repeat : Mirror :
				0 == (TexturePresentation & WrapT_Mirror) ? Clamp : Repeat;
		}
	}
	// names must match opentk's PrimitiveType in order to convert
	// (index doesn't matter, but there should be no more than 16 entries)
	public enum IndexPrimitive : byte
	{
		Points,
		Lines,
		LineLoop,
		LineStrip,
		Triangles,
		TriangleStrip,
		TriangleFan,
		Quads,
		QuadsExt,
		QuadStrip,
		Polygon,
		LinesAdjacency,
		LineStripAdjacency,
		TrianglesAdjacency,
		TriangleStripAdjacency,
		Patches,
	}
	public static class IndexPrimitiveUtility
	{
		static readonly PrimitiveType[] GLEquiv = new PrimitiveType[16];

		static IndexPrimitiveUtility()
		{
			for (int i = GLEquiv.Length - 1; i >= 0; --i)
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
			return ((byte)Value >= GLEquiv.Length || GLEquiv[(byte)Value] == (PrimitiveType)254) ? Fallback : GLEquiv[(byte)Value];
		}
	}
	public enum IndexInteger : byte
	{
		Byte,
		Short,
		Int,
	}
	public interface GraphicsStateProvider
	{

		GraphicsHandle.Buffer Vertex { get; }
		GraphicsHandle.Buffer Index { get; }
		GraphicsHandle.Texture Texture { get; }
		Color4b Tint { get; }
		Color4b Dark { get; }
		Color4b Fog { get; }

		ushort IndexCount { get; }
		IndexPrimitive IndexPrimitive { get; }
		IndexInteger IndexInteger { get; }

		ushort TextureWidth { get; }
		ushort TextureHeight { get; }
		ushort TextureScaleX { get; }
		ushort TextureScaleY { get; }
		TexturePresentation TexturePresentation { get; }
		LightMode LightMode { get; }

		VertexColor VertexColor { get; }
		ElementMask ElementMask { get; }
		Culling Culling { get; }
		FeatureMask FeatureMask { get; }
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct GraphicsState : IEquatable<GraphicsState>
	{
		public GraphicsHandle.Buffer Vertex, Index;
		public GraphicsHandle.Texture Texture;
		public Color4b Tint, Dark, Fog;

		public ushort IndexCount;
		public IndexPrimitive IndexPrimitive;
		public IndexInteger IndexInteger;

		public ushort TextureWidth, TextureHeight, TextureScaleX, TextureScaleY;
		public TexturePresentation TexturePresentation;
		public LightMode LightMode;

		public VertexColor VertexColor;
		public ElementMask ElementMask;
		public Culling Culling;
		public FeatureMask FeatureMask;

		private const int FeatureMaskBits = 7;
		private const int LightModeBits = 1;

		private const int ElementMaskBits = 6;
		private const int CullingBits = 2;

		private const int TexturePresentationBits = 5;
		private const int VertexColorBits = 3;

		private const int IndexPrimitiveBits = 4;
		private const int IndexIntegerBits = 2;

		private const int SumBits = IndexIntegerBits +
			IndexPrimitiveBits + VertexColorBits + TexturePresentationBits +
			CullingBits + ElementMaskBits + LightModeBits + ElementMaskBits;

		private const uint CompileTestSumBitsGreaterEqual32 = 32 - SumBits;//<-- if this don't compile stuff don't fit.

		private const int FeatureMaskShift = (32 - FeatureMaskBits);
		private const int LightModeShift = FeatureMaskShift - 1;

		private const int ElementMaskShift = LightModeShift - ElementMaskBits;
		private const int CullingShift = ElementMaskShift - CullingBits;

		private const int TexturePresentationShift = CullingShift - TexturePresentationBits;
		private const int VertexColorShift = TexturePresentationShift - VertexColorBits;

		private const int IndexPrimitiveShift = VertexColorShift - IndexPrimitiveBits;
		private const int IndexIntegerShift = IndexPrimitiveShift - IndexIntegerBits;

		public int HashComponentRoot => unchecked( 5 * 
			(((int)((byte)this.ElementMask) << ElementMaskShift) |
			((int)((byte)this.FeatureMask) << FeatureMaskShift) |
			((int)((byte)this.Culling) << CullingShift) |
			(0 == (this.FeatureMask & FeatureMask.Lit) ? 0 
				: ((int)((byte)this.LightMode) << LightModeShift)) |
			(0 == (this.ElementMask & ElementMask.Color) ? 0 
				: ((int)((byte)this.VertexColor) << VertexColorShift)) |
			(0 == (this.ElementMask & ElementMask.Texture) ? 0 
				: ((int)((byte)this.TexturePresentation.GetSanitized()) << TexturePresentationShift)) |
			(0 == (this.ElementMask & ElementMask.Index)?0
				: (((int)((byte) IndexPrimitive)<<IndexPrimitiveShift)|
					((int)((byte)IndexInteger)<<IndexIntegerShift)))));

		private static int ROT3(int value) { return (value << 3) | ((value >> (32 - 3)) & ((1 << 3) - 1)); }
		private static int ROT6(int value) { return (value << 6) | ((value >> (32 - 6)) & ((1 << 6) - 1)); }
		private static int ROT9(int value) { return (value << 9) | ((value >> (32 - 9)) & ((1 << 9) - 1)); }
		private static int ROT15(int value) { return (value << 15) | ((value >> (32 - 15)) & ((1 << 15) - 1)); }
		private static int ROT24(int value) { return (value << 24) | ((value >> (32 - 24)) & ((1 << 24) - 1)); }


		public int HashComponentTexture => 0 == (this.ElementMask & ElementMask.Texture) ? 0 : (ROT3(
			((TextureWidth < TextureHeight) ? -1 : 0) ^
			(((int)TextureWidth << 16) | TextureHeight) ^
			ROT24(((int)TextureScaleX << 16)| TextureScaleY)) ^ this.Texture.GetHashCode());

		public int HashComponentVertex => 0 == (this.ElementMask & ElementMaskUtility.Vertex) ? 0 : 
			ROT6(this.Vertex.GetHashCode());

		public int HashComponentIndex => 0 == (this.ElementMask & ElementMask.Index) ? 0 :
			ROT9(this.Index.GetHashCode()) - this.IndexCount;

		public int HashComponentMaterial =>
			//TODO DARK COLOR
			0 == (this.FeatureMask & (FeatureMask.Fog | FeatureMask.Tint)) ? 0 :
			ROT15(
				0 == (this.FeatureMask & (FeatureMask.Tint)) ?
					unchecked((int)Fog.Value) :
				((0 == (this.FeatureMask & FeatureMask.AlphaBlend) ?
					((((int)(Tint.R * 7) << 21) | (((int)Tint.G * 7) << 11) | (((int)Tint.B * 7) << 1)) >> 1) :
				unchecked((int)Tint.Value)) | (0 == (this.FeatureMask & FeatureMask.Fog) ? 0 : ROT3(unchecked((int)Fog.Value))))
				);
		
		public static int GetHashCode(ref GraphicsState A)
		{
			return A.HashComponentRoot ^
				A.HashComponentTexture ^
				A.HashComponentVertex ^
				A.HashComponentIndex ^
				A.HashComponentMaterial;
		}
		public override int GetHashCode()
		{
			return HashComponentRoot ^
				HashComponentTexture ^
				HashComponentVertex ^
				HashComponentIndex ^
				HashComponentMaterial;
		}
		public static bool Equals(ref GraphicsState A, ref GraphicsState B)
		{//TODO DARK COLOR
			return (A.ElementMask == B.ElementMask && A.FeatureMask == B.FeatureMask && A.Culling == B.Culling) &&
				(0 == (A.ElementMask & ElementMask.Color) || (A.VertexColor == B.VertexColor)) &&
				(0 == (A.FeatureMask & FeatureMask.Tint) || (0 == (A.FeatureMask & FeatureMask.AlphaBlend) ?
					(A.Tint.R == B.Tint.R && A.Tint.G == B.Tint.G && A.Tint.B == B.Tint.B)
					: A.Tint.Value == B.Tint.Value)) &&
				(0 == (A.FeatureMask & FeatureMask.Fog) || (A.Fog.Value == B.Fog.Value)) &&
				(0 == (A.ElementMask & ElementMask.Texture) ||
				(A.TexturePresentation == B.TexturePresentation &&
				A.TextureWidth == B.TextureWidth &&
				A.TextureHeight == B.TextureHeight &&
				A.Texture == B.Texture)) &&
				(0 == (A.FeatureMask & FeatureMask.Lit) || (A.LightMode == B.LightMode)) &&
				(0 == (A.ElementMask & (ElementMask.Position | ElementMask.Normal | ElementMask.Color | ElementMask.Texcoord)) ||
					A.Vertex == B.Vertex) &&
				(0 == (A.ElementMask & ElementMask.Index) || (A.IndexInteger == B.IndexInteger && A.IndexCount == B.IndexCount && A.IndexPrimitive == B.IndexPrimitive && A.Index == B.Index));
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
		public static ElementMask Sanitize(ref GraphicsState GraphicsState)
		{
			ElementMask Dropped = 0;
			if (0 == (GraphicsState.ElementMask & ElementMaskUtility.Vertex) ||
				GraphicsState.Vertex.Alive == GraphicsHandle.Null) {
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
			TexturePresentationUtility.Sanitize(ref GraphicsState.TexturePresentation);
			return Dropped;
		}
		private enum Toggle : byte
		{
			False,
			True,
			ChangedFalse,
			ChangedTrue,
		}
		private static Toggle ToggleBit(ref GraphicsState Target, ref GraphicsState Current,
			FeatureMask Bit)
		{
			if (0 == ((Target.FeatureMask ^ Current.FeatureMask) & Bit))
				return 0 == (Target.FeatureMask & Bit) ? Toggle.False : Toggle.True;
			else
			{
				Current.FeatureMask ^= Bit;
				return 0 == (Target.FeatureMask & Bit) ? Toggle.ChangedFalse : Toggle.ChangedTrue;
			}
		}
		private static Toggle ToggleBit(ref GraphicsState Target, ref GraphicsState Current,
			ElementMask Bit)
		{
			if (0 == ((Target.ElementMask ^ Current.ElementMask) & Bit))
				return 0 == (Target.ElementMask & Bit) ? Toggle.False : Toggle.True;
			else
			{
				Current.ElementMask ^= Bit;
				return 0 == (Target.ElementMask & Bit) ? Toggle.ChangedFalse : Toggle.ChangedTrue;
			}
		}
		private static void Cap(
			EnableCap Cap,
			Toggle Toggle)
		{
			if (!(0 == (Toggle & Toggle.ChangedFalse)))
				if (0 == (Toggle & Toggle.True))
					GL.Disable(Cap);
				else
					GL.Enable(Cap);
		}
		private static void Cap(
			ArrayCap Cap,
			Toggle Toggle)
		{
			if (!(0 == (Toggle & Toggle.ChangedFalse)))
				if (0 == (Toggle & Toggle.True))
					GL.DisableClientState(Cap);
				else
					GL.EnableClientState(Cap);
		}
		private static void Bind(
			ref GraphicsState Target,
			ref GraphicsState Bound,
			Toggle ForceToggle)
		{
			int Restore;
			Toggle Tog;
			var DroppedStart = Sanitize(ref Bound);
			var DroppedNext = Sanitize(ref Target);

			if (0 != (ForceToggle&Toggle.ChangedFalse) ||
				!GraphicsState.Equals(ref Bound, ref Target))
			{
				Cap(EnableCap.Blend,
					Tog = ToggleBit(ref Target, ref Bound, FeatureMask.AlphaBlend) | ForceToggle
					);

				Cap(EnableCap.PolygonOffsetFill,
					Tog = ToggleBit(ref Target, ref Bound, FeatureMask.PolygonOffset) | ForceToggle
					);

				Cap(EnableCap.DepthTest,
					Tog = ToggleBit(ref Target, ref Bound, FeatureMask.ZTest) | ForceToggle);

				Cap(EnableCap.Fog,
					Tog = ToggleBit(ref Target, ref Bound, FeatureMask.Fog) | ForceToggle);

				if (Toggle.ChangedTrue == Tog || 
					(Toggle.True==Tog && Bound.Fog.Value != Target.Fog.Value))
				{
					(Bound.Fog = Target.Fog).GL_LoadFogColor();
				}



				Cap(EnableCap.Lighting,
					Tog = ToggleBit(ref Target, ref Bound, FeatureMask.Lit) | ForceToggle);
				if (0 != (Tog & Toggle.True) && (0 != (Tog & Toggle.ChangedFalse) || Bound.LightMode != Target.LightMode))
				{
					GL.ShadeModel(Target.LightMode == LightMode.Smooth ? ShadingModel.Smooth : ShadingModel.Flat);
					Bound.LightMode = Target.LightMode;
				}
				Cap(EnableCap.CullFace, 
					Tog =
						(Target.Culling == Culling.Off ? Toggle.False : Toggle.True) |
						(Target.Culling == Bound.Culling ? Toggle.False : Toggle.ChangedFalse) |
						ForceToggle);

				if (Tog == Toggle.ChangedTrue)
					GL.CullFace(
						Target.Culling == Culling.Both ? 
							CullFaceMode.FrontAndBack 
						: Target.Culling == Culling.Front ?
							CullFaceMode.Front 
						: CullFaceMode.Back);

				Bound.Culling = Target.Culling;

				Tog = ToggleBit(ref Target, ref Bound, FeatureMask.ZWrite) | ForceToggle;

				if (0 != (Tog & Toggle.ChangedFalse))
					GL.DepthMask(0 != (Tog & Toggle.True));


				Cap(EnableCap.Texture2D,
					Tog = ToggleBit(
						ref Target,
						ref Bound,
						ElementMask.Texture) | ForceToggle);

				if (0 != (Tog & Toggle.True))
				{
					Restore = GL.GetInteger(GetPName.MatrixMode);

					if (Restore != (int)MatrixMode.Texture)
					{
						GL.MatrixMode(MatrixMode.Texture);
					}

					GL.LoadIdentity();

					GL.Scale(
						(new Fixed16_16
						{
							Value = 1u +
							(Bound.TextureScaleX = Target.TextureScaleX)
						}.Double / ((int)(
							Bound.TextureWidth = Target.TextureWidth
							) << 5)),

						(new Fixed16_16
						{
							Value = 1u +
							(Bound.TextureScaleY = Target.TextureScaleY)
						}.Double / ((int)(
							Bound.TextureHeight = Target.TextureHeight
							) << 5)),
						1.0);

					if (Restore != (int)MatrixMode.Texture)
					{
						GL.MatrixMode((MatrixMode)Restore);
					}

					GL.BindTexture(TextureTarget.Texture2D,
						(Bound.Texture = Target.Texture));

					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
						(int)Target.TexturePresentation.GetWrapS(TextureWrapMode.Repeat, TextureWrapMode.MirroredRepeat, TextureWrapMode.ClampToEdge));

					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
						(int)Target.TexturePresentation.GetWrapT(TextureWrapMode.Repeat, TextureWrapMode.MirroredRepeat, TextureWrapMode.ClampToEdge));

					/*
					Restore = Target.TexturePresentation.IsTexGenLinear() ? 1 : 0;

					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
						(int)(Restore == 0 ? TextureMagFilter.Nearest : TextureMagFilter.Linear));
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
						(int)(Restore == 0 ? TextureMinFilter.Nearest : TextureMinFilter.Linear));

					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.,
						(int)(Target.TexturePresentation.IsTexGenSpherical() ? All.True : All.False));
						*/
					Bound.TexturePresentation
						= Target.TexturePresentation;
				}

				if (Target.Vertex != Bound.Vertex)
				{
				Bound.Vertex = Target.Vertex;
					GL.BindBuffer(BufferTarget.ArrayBuffer, Target.Vertex);
					// make every element the target has treated as missing in the bound buffer for now.
					Bound.ElementMask ^= (Bound.ElementMask & Target.ElementMask);
				}

				Cap(ArrayCap.VertexArray,
					Tog = ToggleBit(ref Target, ref Bound, ElementMask.Position) | ForceToggle);

				if (Toggle.ChangedTrue == (Tog & Toggle.ChangedTrue))
					GL.VertexPointer(3, VertexPointerType.Short, 16, IntPtr.Zero);

				Cap(ArrayCap.TextureCoordArray,
					Tog = ToggleBit(ref Target, ref Bound, ElementMask.Texcoord) | ForceToggle);

				if (Toggle.ChangedTrue == (Tog & Toggle.ChangedTrue))
					GL.TexCoordPointer(2, TexCoordPointerType.Short, 16, (IntPtr)(8));

				Cap(ArrayCap.NormalArray,
					Tog = ToggleBit(ref Target, ref Bound, ElementMask.Normal) | ForceToggle);

				if (Toggle.ChangedTrue == (Tog & Toggle.ChangedTrue))
					GL.NormalPointer(NormalPointerType.Byte, 16, 12);
				if (0 == (Tog & Toggle.True))
					GL.Enable(EnableCap.AutoNormal);
				else
					GL.Disable(EnableCap.AutoNormal);
			
				Cap(ArrayCap.ColorArray,
					Tog = ToggleBit(ref Target, ref Bound, ElementMask.Color) | ForceToggle);

				if (0 == (Tog & Toggle.True) || 0==(Target.FeatureMask & FeatureMask.Tint))
					Bound.Tint = Color4b.White;
				else
					Bound.Tint = Target.Tint;

				Bound.Tint.GL_Load();
				if (0!=((Bound.FeatureMask ^ Target.FeatureMask) & FeatureMask.Tint))
					Bound.FeatureMask ^= FeatureMask.Tint;

				if (Toggle.ChangedTrue == (Tog & Toggle.ChangedTrue))
					GL.ColorPointer(3, ColorPointerType.UnsignedByte, 16, 12);
			

				Cap(ArrayCap.IndexArray,
					Tog = ToggleBit(ref Target, ref Bound, ElementMask.Index) | ForceToggle);

				if (0 != (Tog & Toggle.True) &&
					Target.Index != Bound.Index)
				{
					Tog |= Toggle.ChangedFalse;
					Bound.Index = Target.Index;
					if (GraphicsHandle.Null != Target.Index.Alive)
					{
						GL.BindBuffer(BufferTarget.ElementArrayBuffer, Bound.Index);
					}
				}
				if (0 != (Tog & Toggle.ChangedFalse))
				{
					Bound.Index = Target.Index;
				}
				Bound.IndexCount = Target.IndexCount;
				Bound.IndexInteger = Target.IndexInteger;
				Bound.IndexPrimitive = Target.IndexPrimitive;
				GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient,
					new OpenTK.Graphics.Color4(Target.Dark.R, Target.Dark.G, Target.Dark.B, Target.Dark.A));
				GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse,
					new OpenTK.Graphics.Color4(Target.Tint.R, Target.Tint.G, Target.Tint.B, Target.Tint.A));
			}
		}

		public static void Redraw(this GraphicsInterface GraphicsInterface)
		{
			using (new ContextLock(GraphicsInterface.GLContext))
			{
				if ((ElementMask.Index | ElementMask.Position) ==
				(GraphicsInterface.MutableBoundState.ElementMask & (ElementMask.Index | ElementMask.Position))
				&& GraphicsInterface.MutableBoundState.IndexCount != 0)
					GL.DrawElements(
						GraphicsInterface.MutableBoundState.IndexPrimitive.ToGL(),
						GraphicsInterface.MutableBoundState.IndexCount,
						GraphicsInterface.MutableBoundState.IndexInteger == IndexInteger.Byte ? DrawElementsType.UnsignedByte
						: GraphicsInterface.MutableBoundState.IndexInteger == IndexInteger.Short ? DrawElementsType.UnsignedShort
						: DrawElementsType.UnsignedInt, IntPtr.Zero);
			}
		}
		public static void Bind(this GraphicsInterface GraphicsInterface, bool ForceInvalidate = false, bool Draw = false)
		{
			using (new ContextLock(GraphicsInterface.GLContext))
			{
				Bind(ref GraphicsInterface.State, ref GraphicsInterface.MutableBoundState, ForceInvalidate ? Toggle.ChangedFalse : (Toggle)0);
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
			if (-1 != GL.GetInteger(GetPName.CurrentColor)) Mask |= FeatureMask.Tint;
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
			Value = GL.GetBoolean(GetPName.PolygonSmooth) ? LightMode.Smooth : LightMode.Hard;
		}
		public static void GetFromGL(out TexturePresentation Value)
		{
			Value = 0;

			int temp;
			GL.GetTexParameter(TextureTarget.Texture2D, GetTextureParameter.TextureMinFilter, out temp);

			if ((int)TextureMinFilter.Nearest != temp) Value |= TexturePresentationUtility.TexGenLinear;
			GL.GetTexParameter(TextureTarget.Texture2D, GetTextureParameter.TextureWrapS, out temp);
			if ((int)TextureWrapMode.ClampToEdge == temp)
				Value |= TexturePresentationUtility.WrapS_Clamp;
			else if ((int)TextureWrapMode.MirroredRepeat == temp)
				Value |= TexturePresentationUtility.WrapS_Mirror;

			GL.GetTexParameter(TextureTarget.Texture2D, GetTextureParameter.TextureWrapT, out temp);
			if ((int)TextureWrapMode.ClampToEdge == temp)
				Value |= TexturePresentationUtility.WrapT_Clamp;
			else if ((int)TextureWrapMode.MirroredRepeat == temp)
				Value |= TexturePresentationUtility.WrapT_Mirror;

			GL.GetTexParameter(TextureTarget.Texture2D, GetTextureParameter.GenerateMipmap, out temp);
			if (temp != 0)
				Value |= TexturePresentationUtility.TexGenSpherical;
		}

		/// <summary>
		/// attempts to get state from GL. 
		/// if any buffer or texture changed, they will be set to zero (since we do not do a reverse lookup beyond the int), but we will retain links to existing ones if the id matches.
		/// 
		/// Does not touch IndexCount, IndexInteger or IndexPrimitive!
		/// </summary>
		public static void GetFromGL(ref GraphicsState State)
		{
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
			Sanitize(ref State);
			GL.GetFloat(GetPName.CurrentColor, out Vector4 c);
			State.Tint.r = c.X;
			State.Tint.g = c.Y;
			State.Tint.b = c.Z;
			State.Tint.a = c.W;
			if (State.Tint.Value != uint.MaxValue)
				State.FeatureMask |= FeatureMask.Tint;
			GL.GetFloat(GetPName.FogColor, out c);
			State.Fog.r = c.X;
			State.Fog.g = c.Y;
			State.Fog.b = c.Z;
			State.Fog.a = c.W;
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
					Sanitize(ref GraphicsInterface.MutableBoundState);
				}
				if (!SkipSetState)
				{
					GraphicsInterface.State = GraphicsInterface.MutableBoundState;
				}
				else
				{
					Sanitize(ref GraphicsInterface.State);
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
