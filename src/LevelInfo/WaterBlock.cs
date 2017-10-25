using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using BubblePony.GLHandle;
using System.ComponentModel;
using BubblePony.Alloc;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Quad64
{
	public sealed class WaterBlock : ILevelProperty, IROMProperty, IMemoryProperty, IAreaProperty
	{
		const int VertexShorts = 8;
		const int VertexCount = 4;
		const int ShortCount = VertexShorts * VertexCount;
		const int ByteCount = ShortCount << 4;
		const int NUM_OF_CATERGORIES = 4;

		[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I2, SizeConst = ShortCount)]
		public readonly short[] vertices;
		private IndexBufferContainer ibc;
		public readonly Area area;
		private string mem_adr, name;
		private readonly ByteSegment memory;
		private GraphicsHandle.Buffer vb;
		private short x1, x2;
		private short z1, z2;
		private short unknown, y;
		private bool modified_check = true;
		static readonly byte[] indices = { 0, 2, 3, 1, };
		static readonly WeakReference ib_ref = new WeakReference(null);
		static object ib_lock = new object();

		public const int IndexCount = 4;
		public const Quad64.IndexPrimitive IndexPrimitive = Quad64.IndexPrimitive.Quads;
		public const int IndexPrimitiveCount = 1;
		public const Quad64.IndexInteger IndexInteger = Quad64.IndexInteger.Byte;

		[Browsable(false)]
		public Area Area => area;
		[Browsable(false)]
		public Level Level => Level;
		[Browsable(false)]
		public ROM ROM => Level.rom;
		[Browsable(false)]
		public ByteSegment Memory => memory;

		private void Set(ref short member, short value)
		{
			if (!modified_check)
			{
				if (member == value)
					return;
				modified_check = true;
			}
			member = value;
		}

		[CustomSortedCategory("Bounds", 2, NUM_OF_CATERGORIES)]
		[Browsable(true)]
		[DisplayName("X1")]
		[Description("First X corner (should be less than X2?)")]
		[TypeConverter(typeof(HexNumberTypeConverter))]
		public short X1 { get { return x1; } set { Set(ref x1, value); } }
		[CustomSortedCategory("Bounds", 2, NUM_OF_CATERGORIES)]
		[Browsable(true)]
		[DisplayName("X2")]
		[Description("Second X corner (should be greater than X1?)")]
		[TypeConverter(typeof(HexNumberTypeConverter))]
		public short X2 { get { return x2; } set { Set(ref x2, value); } }
		[CustomSortedCategory("Bounds", 2, NUM_OF_CATERGORIES)]
		[Browsable(true)]
		[DisplayName("Y")]
		[Description("The Y position of where the water is visibly shown")]
		[TypeConverter(typeof(HexNumberTypeConverter))]
		public short Y { get { return y; } set { Set(ref y, value); } }

		[CustomSortedCategory("Bounds", 2, NUM_OF_CATERGORIES)]
		[Browsable(true)]
		[DisplayName("Z1")]
		[Description("First Z corner (should be less than Z2?)")]
		[TypeConverter(typeof(HexNumberTypeConverter))]
		public short Z1 { get { return z1; } set { Set(ref z1, value); } }

		[CustomSortedCategory("Bounds", 2, NUM_OF_CATERGORIES)]
		[Browsable(true)]
		[DisplayName("Z2")]
		[Description("Second Z corner (should be greater than Z1?)")]
		[TypeConverter(typeof(HexNumberTypeConverter))]
		public short Z2 { get { return z2; } set { Set(ref z2, value); } }

		[Browsable(false)]
		public Vector3s V1 { get => new Vector3s { X = X1, Y = Y, Z = Z1, }; set { X1 = value.X; Y = value.Y; Z1 = value.Z; } }

		[Browsable(false)]
		public Vector3s V2 { get => new Vector3s { X = X2, Y = Y, Z = Z2, }; set { X2 = value.X; Y = value.Y; Z2 = value.Z; } }


		[CustomSortedCategory("Info", 1, NUM_OF_CATERGORIES)]
		[Browsable(true)]
		[Description("Location inside the ROM file")]
		[DisplayName("Address")]
		[ReadOnly(true)]
		public string Address => this.GetAddressString();

		[CustomSortedCategory("Unknown", 1, NUM_OF_CATERGORIES)]
		[Browsable(true)]
		[DisplayName("?")]
		[Description("Not yet sure.. first short.")]
		[TypeConverter(typeof(HexNumberTypeConverter))]
		public short Unknown { get { return unknown; } set { unknown = value; } }

		[Browsable(false), DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private GraphicsHandle.Buffer ib
		{
			get => null == ibc ? LoadIndexBuffer(ref ibc) : ibc;
		}


		public short CalculateX(byte v) => 0 == (v & 1) ? x1 : x2;
		public short CalculateZ(byte v) => 0 == (v & 2) ? z1 : z2;
		public short CalculateY(byte v) => y;

		public Vector3s CalculateXYZ(byte v)=> new Vector3s {
			X = CalculateX(v),
			Y = CalculateY(v),
			Z = CalculateZ(v),
		};

		public Vector3s GetVertexPosition(byte v)
		{
			v = (byte)((v & 3) << 3);
			return new Vector3s { X = vertices[v], Y = vertices[v | 1], Z = vertices[v | 2], };
		}
		public short GetU(byte v)
		{
			return vertices[((v & 3) << 3) | 4];
		}
		private bool SetU(short u, byte v)
		{
			v = (byte)(((v & 3) << 3) | 4);
			if (vertices[v] != u)
			{
				vertices[v] = u;
				v = 255;
			}
			return v == 255;
		}
		public short GetV(byte v)
		{
			return vertices[((v & 3) << 3) | 5];
		}
		private bool SetV(short u, byte v)
		{
			v = (byte)(((v & 3) << 3) | 5);
			if (vertices[v] != u)
			{
				vertices[v] = u;
				v = 255;
			}
			return v == 255;
		}
		private bool SetVertexPosition(Vector3s xyz, byte v)
		{
			v = (byte)((v & 3) << 3);
			if (vertices[v] != xyz.X || vertices[v | 1] != xyz.Y || vertices[v | 2] != xyz.Z)
			{
				vertices[v] = xyz.X;
				vertices[v | 1] = xyz.Y;
				vertices[v | 2] = xyz.Z;
				v = 255;
			}
			return v == 255;
		}



		void IMemoryProperty.Address(out ByteSegment segment, out ROM_Address address, out string address_string)
		{
			IMemoryPropertyUtility.Address(memory, ref mem_adr, out segment, out address, out address_string);
		}
		public WaterBlock(Area parent, ByteSegment memory, int index)
		{
			if (null == (object)parent) throw new System.ArgumentNullException("parent");
			if (memory.Length != 0xC) throw new System.ArgumentException("memory length was not 12","memory");
			this.vertices = (this.vertices ?? new short[ShortCount]);
			this.area = parent;
			this.memory = memory;
			unknown = Scripts.Script.bytesToInt16(memory, 0);
			x1 = Scripts.Script.bytesToInt16(memory, 2);
			z1 = Scripts.Script.bytesToInt16(memory, 4);
			x2 = Scripts.Script.bytesToInt16(memory, 6);
			z2 = Scripts.Script.bytesToInt16(memory, 8);
			y = Scripts.Script.bytesToInt16(memory, 10);
			this.name = GenerateName(index);
		}
		private void VertexBufferEvent(
			bool regen=false,
			bool populate=false,
			bool bind=false)
		{
			if (regen || populate || bind)
			{
				var restore = bind ? 0 : GL.GetInteger(GetPName.ArrayBufferBinding);

				if (regen || (regen = ((populate || bind) && (GraphicsHandle.Null == vb.Alive))))
				{
					vb.Gen();
					populate = true;
				}

				if (bind || populate)
					GL.BindBuffer(BufferTarget.ArrayBuffer, vb);

				if (populate)
					GL.BufferData(
						BufferTarget.ArrayBuffer,
						ByteCount,
						vertices,
						BufferUsageHint.StaticDraw
						);

				if (!bind)
					GL.BindBuffer(BufferTarget.ArrayBuffer, restore);
			}
		}
		public void LoadState(ref GraphicsState State, bool temp_is_selected=false)
		{
			if (modified_check)
			{
				modified_check = false;
				for(byte i = 0; i < VertexCount; ++i)
					modified_check |= SetVertexPosition(new Vector3s { X = CalculateX(i), Y = CalculateY(i), Z = CalculateZ(i), }, i);

				if (GraphicsHandle.Null == vb.Alive)
				{
					SetupVertexConstants();
				}
				else
				{
					UpdateVertices();
				}
				modified_check = false;
			}
			State.Index = ib;
			State.Vertex = vb;
			State.IndexPrimitive = IndexPrimitive;
			State.IndexCount = IndexCount;
			State.IndexInteger = IndexInteger;

			// until it is figured out how to get the f3d material, i'm just using the below.
			// once figured out, the material should be applied to the state before LoadState is even invoked.
			// (unless one level can have multiple materials)
			State.FeatureMask = FeatureMask.AlphaBlend | FeatureMask.ZTest;
			State.ElementMask = ElementMask.Texcoord | ElementMask.Position | ElementMask.Index;
			State.Color = new Color4b { R = 90, G = 128, B = 255, A = temp_is_selected ? (byte)200 : (byte)100, };
		}

		[Browsable(false)]
		public Vector3 TopCenter => new Vector3
		{
			X = 0 == ((x2 - x1) & 1) ? (float)(x1 + ((x2 - x1) / 2)) : (x1 + ((x2 - x1) / 2f)),
			Y = y,
			Z = 0 == ((z2 - z1) & 1) ? (float)(z1 + ((z2 - z1) / 2)) : (z1 + ((z2 - z1) / 2f)),
		};
		public void DrawSolidBoundingBox(Color4b? color=null)
		{
			Quad64.src.Viewer.BoundingBox.draw(
				new Transform { translation = TopCenter, scale = 1, rotation = Quaternion.Identity, },
				color??Color4b.White,
				new Vector3 { X = (x2 - x1)/2f, Y = 0.0001f, Z = (z2 - z1)/2f, });
		}
		public void DrawBoundingBox(Color4b? color=null)
		{
			Quad64.src.Viewer.BoundingBox.draw(
				new Transform { translation = TopCenter, scale = 1, rotation=Quaternion.Identity,},
				color??Color4b.White,
				new Vector3 { X = (x2 - x1) / 2f, Y = 0.0001f, Z = (z2 - z1) / 2f, });
		}
		private void SetupVertexConstants() {

			for (byte i = 0; i < 4; i++)
			{
				//white.
				vertices[(i << 3) | 6] = -1;
				vertices[(i << 3) | 7] = -1;
			}

			VertexBufferEvent(regen: true, populate: true);
		}
		private void UpdateVertices()
		{
			/* if modified_check is true, then positions changed */
			if (modified_check)
			{
				// we'll just have texcoords match position.. no idea if that is correct
				// but we're not even using textures yet.(2017/10/25)
				for (byte i = 0; i < 4; ++i)
				{
					SetU(CalculateX(i), i);
					SetV(CalculateZ((byte)((~i) & 3)), i);//<just a guess to flip the z to v/t.
				}
				VertexBufferEvent(populate: true);
			}
			// note that when it is false, texcoords or colors may need to change when we are not guessing.
		}
		private sealed class IndexBufferContainer
		{
			public GraphicsHandle.Buffer Value;
			public sealed override string ToString()
			{
				return "Water Block Index Buffer";
			}
			public sealed override bool Equals(object obj)
			{
				return obj is IndexBufferContainer;
			}
			public sealed override int GetHashCode()
			{
				return this == (object)null ? 0 : 1986 / 11 / 25;
			}
			public static implicit operator GraphicsHandle.Buffer(IndexBufferContainer container)
			{
				return null == (object)container ? default(GraphicsHandle.Buffer) : container.Value;
			}
		}


		private static IndexBufferContainer LoadIndexBuffer(ref IndexBufferContainer Container)
		{
			lock (ib_lock)
			{
				Container = (IndexBufferContainer)ib_ref.Target;
				if (!ib_ref.IsAlive)
				{
					Container = new IndexBufferContainer();
					var restore = GL.GetInteger(GetPName.ElementArrayBufferBinding);
					Container.Value.Gen();
					Container.Value.Tag = Container;
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, Container.Value);
					GL.BufferData(
						BufferTarget.ElementArrayBuffer,
						(IntPtr)(indices.Length),
						indices,
						BufferUsageHint.StaticDraw
						);
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, restore);
					ib_ref.Target = Container;
				}
			}
			return Container;
		}
		public override string ToString()
		{
			return name;
		}
		public void updateROMData()
		{
			var rom = ROM;
			bool m = false;
			rom.writeHalfword(ref m, memory, 0, unknown);
			rom.writeHalfword(ref m, memory, 2, x1);
			rom.writeHalfword(ref m, memory, 4, z1);
			rom.writeHalfword(ref m, memory, 6, x2);
			rom.writeHalfword(ref m, memory, 8, z2);
			rom.writeHalfword(ref m, memory, 10,y);
			rom.wrote(m, memory);
		}
		static string GenerateName(int index)
		{
			string name;
			if (index < 0) name = "-" + GenerateName(-index);
			else if (index == 0) name = "1st Water Box";
			else if (index == 1) name = "2nd Water Box";
			else if (index == 2) name = "3rd Water Box";
			else if (index == 3) name = "4th Water Box";
			else if (index == 4) name = "5th Water Box";
			else if (index == 5) name = "6th Water Box";
			else if (index == 6) name = "7th Water Box";
			else if (index == 7) name = "8th Water Box";
			else if (index == 8) name = "9th Water Box";
			else if (index == 9) name = "10th Water Box";
			else if (index == 10) name = "11th Water Box";
			else if (index == 11) name = "12th Water Box";
			else if (index == 12) name = "13th Water Box";
			else
			{
				int d = index / 10;
				int n = index - (d * 10);
				if (n == 0)
					name = index.ToString() + "th Water Box";
				else
					name = d.ToString() + GenerateName(n - 1);
			}
			return name;
		}
	}
}
