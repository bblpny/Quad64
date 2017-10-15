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
			return Left.format == Right.format &&
				Left.color == Right.color &&
				Left.darkColor == Right.darkColor &&
				Left.fogColor == Right.fogColor &&
				Left.combineColor == Right.combineColor &&
				Left.geometryMode == Right.geometryMode &&
				Left.h == Right.h &&
				Left.w == Right.w &&
				Left.wrapModes == Right.wrapModes &&
				Left.segOff == Right.segOff &&
				Left.texScaleX == Right.texScaleX &&
				Left.texScaleY == Right.texScaleY &&
				Left.drawLayerBillboard==Right.drawLayerBillboard;
		}

		void IStructHandler<Material>.SetupHashCode(ref Material Value, out int HashCode)
		{
			unchecked {
				HashCode =
					(int)Value.segOff ^
					(((int)Value.w << 16) | Value.h) ^
					(((int)Value.wrapModes << 8) ) ^
					((int)Value.fogColor) ^
					((int)Value.combineColor) ^
					((int)Value.color) ^
					((int)Value.darkColor) ^
					(int)(((Value.geometryMode << 8) ^ (Value.format) ^ (Value.geometryMode >> 24)))^
					new Vector2{ X=1-Value.texScaleX,Y=1-Value.texScaleY}.GetHashCode() ^
					(Value.drawLayerBillboard << 18);
			};
		}
	}

	public struct Material : IStruct<Material>
	{
		public ushort w, h;
		public ushort texScaleX, texScaleY;
		public SegmentOffset segOff;
		public Color4b color, darkColor, fogColor, combineColor;
		public uint geometryMode;
		public ushort wrapModes;
		public byte format, drawLayerBillboard;
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
					w = 0,
					h = 0,
					segOff = 0,
					color = {Value=0xFFFFFFFFu},
					format = 0x10,
					geometryMode = 0x22205,
					texScaleX = ushort.MaxValue,
					texScaleY = ushort.MaxValue,
					wrapModes=0,
					drawLayerBillboard = 1,

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
