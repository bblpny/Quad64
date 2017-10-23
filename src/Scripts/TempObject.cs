using System;
using System.Collections.Generic;
using OpenTK;
using Wavefront = BubblePony.Wavefront;

namespace Quad64.Scripts
{
	using BubblePony.Temporary;

	struct TempMaterialHandler : IStructHandler<Material>
	{
		StructFlags IStructHandler<Material>.StructFlags =>  StructFlags.AutoIndex;

		bool IStructHandler<Material>.Equals(ref Material Left, ref Material Right)
		{
			return Material.Equals(ref Left, ref Right);
		}

		void IStructHandler<Material>.SetupHashCode(ref Material Value, out int HashCode)
		{
			Material.GetHashCode(ref Value, out HashCode);
		}
	}

	public enum AlphaDither : byte
	{
		Pattern, NotPattern, Noise, Disabled,
	}
	public enum RGBDither : byte
	{
		MagicSQ, Bayer, Noise, Disabled,
	}
	public enum Pipeline : byte
	{
		NPrimitive=0,
		OnePrimitive=1,
	}
	public enum CombineKey : byte
	{
		None,Key
	}
	public enum TextureConvert : byte
	{
		Convert,
		FilterConvert = 5,
		Filter = 6,
	}
	public enum TextureLUT : byte
	{
		None, RGBA16=2, IA16,
	}
	public enum TextureLOD : byte
	{
		Tile, LOD
	}
	public enum TextureFilter : byte
	{
		Point, BiLerp = 2, Average
	}
	public enum TextureDetail : byte
	{
		Clamp, Sharpen, Detail,
	}
	public enum TexturePerspective : byte
	{
		None,Perspective
	}
	public enum CycleType : byte
	{
		One,Two,Copy,Fill
	}
	public enum ColorDither : byte
	{
		Disable,Enable
	}
	public enum AlphaCompare : byte
	{
		None,Threshold,Dither=3
	}
	public enum DepthSource : byte
	{
		Pixel, Prim
	}

	public enum DestinationCoverage : byte
	{
		Clamp,
		Wrap,
		Full,
		Save,
	}
	public enum ZMode : byte
	{
		OPA,
		INTER,
		XLU,
		DEC,
	}
	[Flags]
	public enum AntiAlias : byte {
		Enable =1,
	}
	[Flags]
	public enum ZFunction : byte
	{
		Compare = 1,
		Update = 2,
	}
	[Flags]
	public enum ImageRead : byte
	{
		Enable=1,
	}
	[Flags]
	public enum ClearOnCoverage : byte
	{
		Enable=1
	}
	[Flags]
	public enum RenderModeOptions : byte
	{
		CoverageTimesAlpha=1,
		AlphaCoverageSelect=2,
		ForceBL=4,
		TexEdge=8,
	}



	public struct Material : IStruct<Material>, IEquatable<Material>
	{
		public LightArray light;
		public Mtx Mtx;
		public Color4b pointRGBA;
		public ushort pointS, pointT;
		public Vector4s pointXYZScreen;
		public ushort w, h;
		public ushort texScaleX, texScaleY;
		public uint OtherModeL, OtherModeH;
		public uint CombinerL, CombinerH;
		public uint numLightWord,
			clipNX,
			clipNY,
			clipPX,
			clipPY;
		public uint fogWord;

		public ushort fogOffset
		{
			get => (ushort)(fogWord >> 16);
			set => fogWord = (fogWord & ushort.MaxValue) | ((uint)value << 16);
		}

		public ushort fogMultiplier
		{
			get => (ushort)(fogWord & ushort.MaxValue);
			set => fogWord = (fogWord & ((uint)ushort.MaxValue << 16)) | (uint)value;
		}

		public const uint LightCount0 = (((0u + 1u) * 32u) + 0x80000000);
		public const uint LightCount1 = (((1u + 1u) * 32u) + 0x80000000);
		public const uint LightCount2 = (((2u + 1u) * 32u) + 0x80000000);
		public const uint LightCount3 = (((3u + 1u) * 32u) + 0x80000000);
		public const uint LightCount4 = (((4u + 1u) * 32u) + 0x80000000);
		public const uint LightCount5 = (((5u + 1u) * 32u) + 0x80000000);
		public const uint LightCount6 = (((6u + 1u) * 32u) + 0x80000000);
		public const uint LightCount7 = (((7u + 1u) * 32u) + 0x80000000);

		public byte numLight
		{
			get => unchecked((byte)(((numLightWord - 0x80000000u) / 32u) - 1u));
			set => numLightWord = (((uint)(value > 7 ? 7u : value < 0 ? 0u : value) + 1u) * 32u) + 0x80000000u;
		}
		public SegmentOffset segOff;
		public Color4b fillColor;
		public Color4a fogColor, envColor, blendColor, primColor;
		public uint geometryMode;
		public ushort wrapModes;
		public byte format, drawLayerBillboard,
			primColorMin, primColorFactor;

		private static uint SetBits(uint options, byte value, byte mask, sbyte shift)
		{
			return (options & (~((uint)mask << shift))) | (((uint)value & (uint)mask) << shift);
		}
		private static uint SetBit(uint options, bool value, sbyte shift)
		{
			return value ? (options | (1u << shift)) : (options & (~(1u << shift)));
		}
		private static bool GetBit(uint options, sbyte shift)
		{
			return 0 != (options & (1u << shift));
		}
		private static byte GetBit1(uint options, sbyte shift)
		{
			return (byte)((options>>shift)&1);
		}
		private static byte GetBits(uint options, sbyte shift, byte mask)
		{
			return (byte)((options>>shift)&mask);
		}
		public byte BlendMask
		{
			get => (byte)(OtherModeH & 15u);
			set => OtherModeH = (OtherModeH & ~15u) | (value & 15u);
		}
		public AlphaDither AlphaDither
		{
			get => (AlphaDither)GetBits(OtherModeH, 4, (byte)3);
			set => OtherModeH = SetBits(OtherModeH, (byte)value, (byte)3, 4);
		}
		public RGBDither RGBDither
		{
			get => (RGBDither)GetBits(OtherModeH, 6, (byte)3);
			set => OtherModeH = SetBits(OtherModeH, (byte)value, (byte)3, 6);
		}
		public CombineKey CombineKey
		{
			get => (CombineKey)GetBit1(OtherModeH, 8);
			set => OtherModeH = SetBit(OtherModeH, 0!=value, 8);
		}
		public TextureConvert TextureConvert
		{
			get => (TextureConvert)GetBits(OtherModeH, 9, (byte)7);
			set => OtherModeH = SetBits(OtherModeH, (byte)value, (byte)7, 9);
		}
		public TextureFilter TextureFilter
		{
			get => (TextureFilter)GetBits(OtherModeH, 12, (byte)3);
			set => OtherModeH = SetBits(OtherModeH, (byte)value, (byte)3, 12);
		}
		public TextureLUT TextureLUT
		{
			get => (TextureLUT)GetBits(OtherModeH, 14, (byte)3);
			set => OtherModeH = SetBits(OtherModeH, (byte)value, (byte)3, 14);
		}
		public TextureLOD TextureLOD
		{
			get => (TextureLOD)GetBit1(OtherModeH, 16);
			set => OtherModeH = SetBit(OtherModeH, 0!=value, 16);
		}
		public TextureDetail TextureDetail
		{
			get => (TextureDetail)GetBits(OtherModeH, 17, (byte)3);
			set => OtherModeH = SetBits(OtherModeH, (byte)value, (byte)3, 17);
		}
		public TexturePerspective TexturePerspective
		{
			get => (TexturePerspective)GetBit1(OtherModeH, 19);
			set => OtherModeH = SetBit(OtherModeH, 0!=value, 19);
		}
		public CycleType CycleType
		{
			get => (CycleType)GetBits(OtherModeH, 20, (byte)3);
			set => OtherModeH = SetBits(OtherModeH, (byte)value, (byte)3, 20);
		}
		public ColorDither ColorDither
		{
			get => (ColorDither)GetBit1(OtherModeH, 22);
			set => OtherModeH = SetBit(OtherModeH, 0!=value, 22);
		}
		public Pipeline Pipeline
		{
			get => (Pipeline)GetBit1(OtherModeH, 23);
			set => OtherModeH = SetBit(OtherModeH, 0!=value, 23);
		}
		public AlphaCompare AlphaCompare
		{
			get => (AlphaCompare)((byte)(OtherModeL & 3u));
			set => OtherModeL = (OtherModeL & (~(3u))) | ((byte)value & 3u);
		}
		public DepthSource DepthSource
		{
			get => (DepthSource)GetBit1(OtherModeL, 2);
			set => OtherModeL = SetBit(OtherModeL, 0 != value, 2);
		}
		public AntiAlias AntiAlias
		{
			get => (AntiAlias)GetBit1(OtherModeL, 3);
			set => OtherModeL = SetBit(OtherModeL, 0 != value, 3);
		}
		public ZFunction DepthFunction
		{
			get => (ZFunction)GetBits(OtherModeL, 4, (byte)3);
			set => OtherModeL = SetBits(OtherModeL, (byte)value, (byte)3, 4);
		}
		public ImageRead ImageRead
		{
			get => (ImageRead)GetBit1(OtherModeL, 6);
			set => OtherModeL = SetBit(OtherModeL, 0 != value, 6);
		}
		public ClearOnCoverage ClearOnCoverage
		{
			get => (ClearOnCoverage)GetBit1(OtherModeL, 7);
			set => OtherModeL = SetBit(OtherModeL, 0 != value, 7);
		}
		public DestinationCoverage DestinationCoverage
		{
			get => (DestinationCoverage)GetBits(OtherModeL, 8,(byte)3);
			set => OtherModeL = SetBits(OtherModeL, (byte)value, (byte)3, 8);
		}
		public ZMode DepthMode
		{
			get => (ZMode)GetBits(OtherModeL, 10, (byte)3);
			set => OtherModeL = SetBits(OtherModeL, (byte)value, (byte)3, 10);
		}
		public RenderModeOptions RenderModeOptions
		{
			get => (RenderModeOptions)GetBits(OtherModeL, 12, (byte)15);
			set => OtherModeL = SetBits(OtherModeL, (byte)value, (byte)15, 12);
		}
		public ushort Blender
		{
			get => (ushort)(OtherModeL >> 16);
			set => OtherModeL = (OtherModeL & ((1u << 16) - 1u)) | ((uint)value << 16);
		}

		public override int GetHashCode()
		{
			GetHashCode(ref this, out int hashCode);
			return hashCode;
		}
		public bool Equals(ref Material other) { return Equals(ref this, ref other); }
		public bool Equals(Material other) { return Equals(ref this, ref other); }
		public override bool Equals(object obj)
		{
			return obj is Material && ((Material)obj).Equals(ref this);
		}
		public static bool Equals(ref Material Left, ref Material Right)
		{
			return
				Left.OtherModeL == Right.OtherModeL &&
				Left.OtherModeH == Right.OtherModeH &&
				Left.CombinerL == Right.CombinerL &&
				Left.CombinerH == Right.CombinerH &&
				Left.format == Right.format &&
				Left.numLightWord==Right.numLightWord &&
				//Left.lightColor == Right.lightColor &&
				//Left.darkColor == Right.darkColor &&
				Left.fogColor == Right.fogColor &&
				Left.envColor == Right.envColor &&
				Left.fillColor == Right.fillColor &&
				Left.blendColor == Right.blendColor &&
				Left.primColor == Right.primColor &&
				Left.geometryMode == Right.geometryMode &&
				Left.h == Right.h &&
				Left.w == Right.w &&
				Left.wrapModes == Right.wrapModes &&
				Left.segOff == Right.segOff &&
				Left.texScaleX == Right.texScaleX &&
				Left.texScaleY == Right.texScaleY &&
				Left.drawLayerBillboard == Right.drawLayerBillboard &&
				LightArray.Equals(ref Left.light, ref Right.light, Left.numLight);
		}
		public static void GetHashCode(ref Material Value, out int HashCode)
		{

			unchecked
			{
				HashCode =
					(int)(Value.OtherModeL ^ (Value.OtherModeH * 5)) ^
					(int)Value.segOff ^
					(((int)Value.w << 16) | Value.h) ^
					(((int)Value.wrapModes << 8)) ^
					((int)Value.fogColor) ^
					((int)Value.envColor) ^
					((int)Value.blendColor) ^
					((int)Value.fillColor) ^
					((int)Value.primColor) ^
					(int)(((Value.geometryMode << 8) ^ (Value.format) ^ (Value.geometryMode >> 24))) ^
					new Vector2 { X = 1 - Value.texScaleX, Y = 1 - Value.texScaleY }.GetHashCode() ^
					(Value.drawLayerBillboard << 18) ^
					LightArray.GetHashCode(ref Value.light, Value.numLight) ^
					Mtx.GetHashCode(ref Value.Mtx);
			};
		}

		public WrapModes WrapModes => (WrapModes)((byte)(((wrapModes>>6)&(3<<2))|(wrapModes &3)));
		public TextureIdentifier TextureIdentifier =>
			new TextureIdentifier
			{
				Format = format,
				Height = (byte)h,
				Width = (byte)w,
				SegmentOffset = segOff,
				Wrap = WrapModes,
			};
		/// <summary>
		/// returns true if U and/or V is clamped!
		/// </summary>
		public bool HasClampUV => ((wrapModes >> 2) & 3u) == 2 || (wrapModes&3u)==2;
		public Wavefront.Illumination CalcuateDefaultIlluminationMode(bool? solid_alpha)
		{
			var fmt = TextureFormats.GetFormat(format);
			if (fmt.ColorDepth == 0 || solid_alpha.HasValue && solid_alpha.Value)
			{
				return Wavefront.Illumination.Color;
			}
			else if (fmt.ColorChannels == 1)
			{
				// luminosity.
				return fmt.AlphaDepth == 0 ? Wavefront.Illumination.Highlight : Wavefront.Illumination.CastShadows;
			}
			else if (fmt.AlphaDepth == 1)
			{
				return Wavefront.Illumination.Color;
			}
			else
				return Wavefront.Illumination.Color;
		}
		public bool HasTexture
		{
			get
			{
				return (int)segOff != 0;
			}
		}
		public static Material Default
		{
			get
			{
				return new Material
				{
					Mtx = { XX_I = 1, YY_I = 1, ZZ_I = 1, WW_I = 1, },
					numLightWord = LightCount1,
					light =
					{
						Light1 = Light.InitOff,
						Light2 = Light.InitOn,
						Light3 = Light.InitOff,
						Light4 = Light.InitOff,
						Light5 = Light.InitOff,
						Light6 = Light.InitOff,
						Light7 = Light.InitOff,
						Light8 = Light.InitOff,
					},
					format = 0x10,
					geometryMode = 0x22205,
					texScaleX = ushort.MaxValue,
					texScaleY = ushort.MaxValue,
					fogColor = { Value = ((1u << 24) - 1u)<<8, },
					primColorFactor=204,
					primColorMin=204,
				};
			}
		}

		IStructHandler<Material> IStruct<Material>.Handler => default(TempMaterialHandler);

		public TempMaterial Register()
		{
			return TempMaterial.Register(ref this);
		}
		public bool IsLit { get { return (geometryMode & 0x20000) != 0; } }
		public bool CullFront => 0 != (0x1000 & geometryMode);
		public bool CullBack => 0 != (0x2000 & geometryMode);
		public bool Smooth => 0 != (0x0200 & geometryMode);
		public bool Shade => 0 != (0x04 & geometryMode);
		public bool ZTest => 0 != (0x01 & geometryMode);
		public bool Fog => 0 != (0x010000 & geometryMode);
		public bool TexGen => 0 != (0x040000 & geometryMode);
		public bool TexLin => 0 != (0x080000 & geometryMode);
		public bool Clip => 0 != (0x800000 & geometryMode);

		public bool HasVertexColors { get { return !IsLit; } }
		public bool HasMaterialColor { get { return true; } }// IsLit; } }// !Shade; } }
	}

	public sealed class TempMaterial 
		: Object<TempMaterial, Material>
		, IEquatable<TempMaterial> {

		static readonly TempMaterial tempMaterialKey = new TempMaterial();
		static readonly Dictionary<TempMaterial, TempMaterial> tempMaterials = new Dictionary<TempMaterial, TempMaterial>(TempMaterial.Equality.Instance);
		static TempMaterial.List tempMaterialList = new TempMaterial.List();
		static object tempMaterialLock = new object();
		
		// do not change the target of this weakref. it should always point to this.
		internal readonly WeakReference weakref;

		public TempMaterial() { weakref = new WeakReference(this); }
		public static TempMaterial Register(ref Material Data)
		{
			TempMaterial value;
			lock (tempMaterialLock)
			{
				tempMaterialKey.value = Data;
				TempMaterial.Equator.SetupHashCode(ref tempMaterialKey.value, out tempMaterialKey.hashCode);

				if (!tempMaterials.TryGetValue(tempMaterialKey, out value))
				{
					TempMaterial.Add(ref tempMaterialList, ref Data);
					tempMaterials.Add(tempMaterialList.last, tempMaterialList.last);
				}
			}
			return value;
		}
	}

	internal struct TempVertexHandler : IStructHandler<TempVertexData>
	{
		StructFlags IStructHandler<TempVertexData>.StructFlags { get => 0; }

		bool IStructHandler<TempVertexData>.Equals(ref TempVertexData Left, ref TempVertexData Right)
		{
			return Left.position == Right.position &&
				Left.normal == Right.normal &&
				Left.texCoord == Right.texCoord &&
				Left.color == Right.color;
		}

		static int ROT(int v, sbyte L) { return v << L | (int)unchecked(((uint)v >> (32 - L))); }

		void IStructHandler<TempVertexData>.SetupHashCode(ref TempVertexData Value, out int HashCode)
		{
			HashCode = ROT(Value.position.GetHashCode(), 3) ^
				ROT(Value.normal.GetHashCode(), 5) ^
				ROT(Value.texCoord.GetHashCode(), 13) ^
				ROT(Value.color.GetHashCode(), 29);
		}
	}

	public struct TempVertexData : IStruct<TempVertexData>
	{
		public Vector3 position;
		public Vector3 normal;
		public Vector4 color;
		public Vector2 texCoord;
		IStructHandler<TempVertexData> IStruct<TempVertexData>.Handler { get => default(TempVertexHandler); }
	}

	public sealed class TempVertex : Object<TempVertex, TempVertexData>, IEquatable<TempVertex> { }
	
	public sealed class TempVertex128 : Object<TempVertex128, Vertex128>, IEquatable<TempVertex128> { }

	internal struct TempMeshHandler : IStructHandler<TempMeshData>
	{
		StructFlags IStructHandler<TempMeshData>.StructFlags { get => StructFlags.Dispose | StructFlags.AutoIndex; }

		bool IStructHandler<TempMeshData>.Equals(ref TempMeshData Left, ref TempMeshData Right)
		{
			return Left.segmentAddress == Right.segmentAddress && Left.wr_material==Right.wr_material;
		}
		void IStructHandler<TempMeshData>.SetupHashCode(ref TempMeshData Value, out int HashCode)
		{
			unchecked
			{
				if (null == Value.wr_material)
					HashCode = (int)Value.segmentAddress;
				else
				{
					var material = (TempMaterial)Value.wr_material.Target;
					if (null == (object)material) throw new System.InvalidOperationException("Material died on value, making it invalid");
					HashCode= material.hashCode;
				}
			}
		}
	}
	public struct TempMeshData : IStruct<TempMeshData>
	{
		/// <summary>
		/// material references (when obtained through Register() of TempMaterialData or static Register(ref data) of TempMaterial are pinned statically
		/// so the only time this reference would ever drop something would be when that would clear (which never happens right now)
		/// 
		/// so keeping in mind that this value struct will be kept in memory it seems responsible to use weak reference here.
		/// </summary>
		internal WeakReference wr_material;
		internal ModelBuilder.TextureInfo info;
		internal uint segmentAddress;
		internal TempMaterial tempMaterial => null == (object)wr_material ? null : (TempMaterial)wr_material.Target;
		public bool getMaterial(out Material material)
		{
			var tempMaterial = this.tempMaterial;
			bool ret;
			if ((ret = (null != (object)tempMaterial)))
				material = tempMaterial.value;
			else
				material = Material.Default;
			return ret;
		}
		public Material getMaterial() { getMaterial(out Material ret); return ret; }
		IStructHandler<TempMeshData> IStruct<TempMeshData>.Handler { get => default(TempMeshHandler); }
	}
	public struct TempMeshReferences : System.IDisposable
	{
		internal TextureFormats.Raw bmp;
		internal TempVertex.List list;
		void System.IDisposable.Dispose() {
			TempVertex.Free(ref list);
			bmp = null;
		}
	}
	public sealed class TempMesh: Object<TempMesh, TempMeshData, TempMeshReferences>, 
		IEquatable<TempMesh> { }
	
	public struct TempMesh128References : System.IDisposable
	{
		internal TextureFormats.Raw bmp;
		internal TempVertex128.List list;
		void System.IDisposable.Dispose()
		{
			TempVertex128.Free(ref list);
			bmp = null;
		}
	}
	public sealed class TempMesh128 : Object<TempMesh128, TempMeshData, TempMesh128References>, IEquatable<TempMesh128> { }

}
