using OpenTK;
using System;
using BubblePony.Alloc;
using BubblePony.Integers;

namespace Quad64.Scripts
{
	internal sealed class Fast3DScripts : Script
	{
		private struct F3D_Vertex : IEquatable<F3D_Vertex>, IEquatable<Vertex128>
		{
			public bool Equals(F3D_Vertex other)
			{
				return p == other.p || (Vertex128)this == (Vertex128)other;
			}
			public bool Equals(Vertex128 other) { return other.Equals((Vertex128)this); }
			public static bool operator ==(F3D_Vertex a, F3D_Vertex b) { return a.Equals(b); }
			public static bool operator !=(F3D_Vertex a, F3D_Vertex b) { return a.Equals(b); }
			public static bool operator ==(F3D_Vertex a, Vertex128 b) { return a.Equals(b); }
			public static bool operator !=(F3D_Vertex a, Vertex128 b) { return a.Equals(b); }
			public static bool operator ==(Vertex128 b, F3D_Vertex a) { return a.Equals(b); }
			public static bool operator !=(Vertex128 b, F3D_Vertex a) { return a.Equals(b); }
			public override int GetHashCode()
			{
				return ((Vertex128)this).GetHashCode();
			}
			public override bool Equals(object obj)
			{
				return obj is F3D_Vertex ? Equals((F3D_Vertex)obj) :
					(obj is Vertex128 && ((Vertex128)this).Equals((Vertex128)obj));
			}
			public override string ToString()
			{
				return ((Vertex128)this).ToString();
			}
			public static implicit operator Vertex128 (F3D_Vertex v)
			{
				Vertex128 o;
				o.x = v.x;
				o.y = v.y;
				o.z = v.z;
				o.f = v.f;
				o.u = v.u;
				o.v = v.v;
				o.nx_r = v.nx_r;
				o.ny_g = v.ny_g;
				o.nz_b = v.nz_b;
				o.a = v.a;
				return o;
			}
			public ByteSegment p;
			public Vector3s Read(int position, Vector3s fallback)
			{
				Vector3s o;
				o.X = Read(position, fallback.X);
				o.Y = Read(position + 2, fallback.Y);
				o.Z = Read(position + 4, fallback.Z);
				return o;
			}
			public Vector3c Read(int position, Vector3c fallback)
			{
				Vector3c o;
				o.X = Read(position, fallback.X);
				o.Y = Read(position + 1, fallback.Y);
				o.Z = Read(position + 2, fallback.Z);
				return o;
			}
			public Vector4b Read(int position, Vector4b fallback)
			{
				Vector4b o;
				o.Packed = 0;
				o.X = Read(position, fallback.X);
				o.Y = Read(position + 1, fallback.Y);
				o.Z = Read(position + 2, fallback.Z);
				o.W = Read(position + 3, fallback.Z);
				return o;
			}
			public short Read(int position, short fallback) { return p.Length >= 0x10 ? (short)((((int)p[position] << 24) >> 16) | p[1 + position]) : fallback; }
			public ushort Read(int position, ushort fallback) { return p.Length >= 0x10 ? (ushort)(((ushort)p[position] << 8) | p[1 + position]) : fallback; }
			public byte Read(int position, byte fallback) { return p.Length >= 0x10 ? p[position] : fallback; }
			public sbyte Read(int position, sbyte fallback) { return p.Length >= 0x10 ? (sbyte)(((int)p[position] << 24) >> 24) : fallback; }

			public short x => Read(0, (short)0);
			public short y => Read(2, (short)0);
			public short z => Read(4, (short)0);
			public ushort f => Read(6, (ushort)0);
			public short u => Read(8, (short)0);
			public short v => Read(10, (short)0);
			public sbyte nx => Read(12, (sbyte)0);
			public sbyte ny => Read(13, (sbyte)0);
			public sbyte nz => Read(14, (sbyte)0);
			public byte r => Read(12, (byte)0);
			public byte g => Read(13, (byte)0);
			public byte b => Read(14, (byte)0);
			public byte a => Read(15, (byte)0);
			public byte nx_r => Read(12, (byte)0);
			public byte ny_g => Read(13, (byte)0);
			public byte nz_b => Read(14, (byte)0);

			public void Load(ref ByteSegment vData, int offset)
			{
				vData.Segment((uint)offset, 0x10, out this.p);
			}
			public static void Load(out F3D_Vertex v, ref ByteSegment vData, int offset)
			{
				vData.Segment((uint)offset, 0x10, out v.p);
			}
		}
		private struct F3D_VertexBuffer
		{
			public F3D_Vertex
				j, k, l, m,
				n, o, p, q,
				r, s, t, u,
				w, x, y, z;

			public F3D_Vertex this[int i]
			{
				get => 0 == (i & 8) ?
						0 == (i & 4) ?
							0 == (i & 2) ?
								0 == (i & 1) ? j : k
							: 0 == (i & 1) ? l : m :
							0 == (i & 2) ?
								0 == (i & 1) ? n : o
							: 0 == (i & 1) ? p : q :
						0 == (i & 4) ?
							0 == (i & 2) ?
								0 == (i & 1) ? r : s
							: 0 == (i & 1) ? t : u :
							0 == (i & 2) ?
								0 == (i & 1) ? w : x
							: 0 == (i & 1) ? y : z;
				set
				{
					if (0 == (i & 8))
						if (0 == (i & 4))
							if (0 == (i & 2))
								if (0 == (i & 1)) j = value; else k = value;
							else if (0 == (i & 1)) l = value; else m = value;
						else if (0 == (i & 2))
							if (0 == (i & 1)) n = value; else o = value;
						else if (0 == (i & 1)) p = value; else q = value;
					else if (0 == (i & 4))
						if (0 == (i & 2))
							if (0 == (i & 1)) r = value; else s = value;
						else if (0 == (i & 1)) t = value; else u = value;
					else if (0 == (i & 2))
						if (0 == (i & 1)) w = value; else x = value;
					else if (0 == (i & 1)) y = value; else z = value;
				}
			}
			public static void Load(ref F3D_VertexBuffer v, ref ByteSegment vData, int i, int offset)
			{
				if (0 == (i & 8))
					if (0 == (i & 4))
						if (0 == (i & 2))
							if (0 == (i & 1)) F3D_Vertex.Load(out v.j, ref vData, offset); else F3D_Vertex.Load(out v.k, ref vData, offset);
						else if (0 == (i & 1)) F3D_Vertex.Load(out v.l, ref vData, offset); else F3D_Vertex.Load(out v.m, ref vData, offset);
					else if (0 == (i & 2))
						if (0 == (i & 1)) F3D_Vertex.Load(out v.n, ref vData, offset); else F3D_Vertex.Load(out v.o, ref vData, offset);
					else if (0 == (i & 1)) F3D_Vertex.Load(out v.p, ref vData, offset); else F3D_Vertex.Load(out v.q, ref vData, offset);
				else if (0 == (i & 4))
					if (0 == (i & 2))
						if (0 == (i & 1)) F3D_Vertex.Load(out v.r, ref vData, offset); else F3D_Vertex.Load(out v.s, ref vData, offset);
					else if (0 == (i & 1)) F3D_Vertex.Load(out v.t, ref vData, offset); else F3D_Vertex.Load(out v.u, ref vData, offset);
				else if (0 == (i & 2))
					if (0 == (i & 1)) F3D_Vertex.Load(out v.w, ref vData, offset); else F3D_Vertex.Load(out v.x, ref vData, offset);
				else if (0 == (i & 1)) F3D_Vertex.Load(out v.y, ref vData, offset); else F3D_Vertex.Load(out v.z, ref vData, offset);
			}
			public static void Load(ref F3D_VertexBuffer v, ref ByteSegment vData, int i)
			{
				Load(ref v, ref vData, i, i * 0x10);
			}

		}
		private static void LoadTriangle(
			ref Runtime rt,
			ref ByteSegment cmd,
			ref Material mat,
			ref F3D_VertexBuffer v,
			int i, int j, int k)
		{
			if (0 == (i & 8))
				if (0 == (i & 4))
					if (0 == (i & 2))
						if (0 == (i & 1)) LoadTriangle(ref rt, ref cmd, ref mat, ref v.j, ref v, j, k); else LoadTriangle(ref rt, ref cmd, ref mat, ref v.k, ref v, j, k);
					else if (0 == (i & 1)) LoadTriangle(ref rt, ref cmd, ref mat, ref v.l, ref v, j, k); else LoadTriangle(ref rt, ref cmd, ref mat, ref v.m, ref v, j, k);
				else if (0 == (i & 2))
					if (0 == (i & 1)) LoadTriangle(ref rt, ref cmd, ref mat, ref v.n, ref v, j, k); else LoadTriangle(ref rt, ref cmd, ref mat, ref v.o, ref v, j, k);
				else if (0 == (i & 1)) LoadTriangle(ref rt, ref cmd, ref mat, ref v.p, ref v, j, k); else LoadTriangle(ref rt, ref cmd, ref mat, ref v.q, ref v, j, k);
			else if (0 == (i & 4))
				if (0 == (i & 2))
					if (0 == (i & 1)) LoadTriangle(ref rt, ref cmd, ref mat, ref v.r, ref v, j, k); else LoadTriangle(ref rt, ref cmd, ref mat, ref v.s, ref v, j, k);
				else if (0 == (i & 1)) LoadTriangle(ref rt, ref cmd, ref mat, ref v.t, ref v, j, k); else LoadTriangle(ref rt, ref cmd, ref mat, ref v.u, ref v, j, k);
			else if (0 == (i & 2))
				if (0 == (i & 1)) LoadTriangle(ref rt, ref cmd, ref mat, ref v.w, ref v, j, k); else LoadTriangle(ref rt, ref cmd, ref mat, ref v.x, ref v, j, k);
			else if (0 == (i & 1)) LoadTriangle(ref rt, ref cmd, ref mat, ref v.y, ref v, j, k); else LoadTriangle(ref rt, ref cmd, ref mat, ref v.z, ref v, j, k);
		}
		private static void LoadTriangle(
				ref Runtime rt,
				ref ByteSegment cmd,
				ref Material mat,
				ref F3D_Vertex a,
				ref F3D_VertexBuffer v,
				int i, int k)
		{
			if (0 == (i & 8))
				if (0 == (i & 4))
					if (0 == (i & 2))
						if (0 == (i & 1)) LoadTriangle(ref rt, ref cmd, ref mat, ref a, ref v.j, ref v, k); else LoadTriangle(ref rt, ref cmd, ref mat, ref a, ref v.k, ref v, k);
					else if (0 == (i & 1)) LoadTriangle(ref rt, ref cmd, ref mat, ref a, ref v.l, ref v, k); else LoadTriangle(ref rt, ref cmd, ref mat, ref a, ref v.m, ref v, k);
				else if (0 == (i & 2))
					if (0 == (i & 1)) LoadTriangle(ref rt, ref cmd, ref mat, ref a, ref v.n, ref v, k); else LoadTriangle(ref rt, ref cmd, ref mat, ref a, ref v.o, ref v, k);
				else if (0 == (i & 1)) LoadTriangle(ref rt, ref cmd, ref mat, ref a, ref v.p, ref v, k); else LoadTriangle(ref rt, ref cmd, ref mat, ref a, ref v.q, ref v, k);
			else if (0 == (i & 4))
				if (0 == (i & 2))
					if (0 == (i & 1)) LoadTriangle(ref rt, ref cmd, ref mat, ref a, ref v.r, ref v, k); else LoadTriangle(ref rt, ref cmd, ref mat, ref a, ref v.s, ref v, k);
				else if (0 == (i & 1)) LoadTriangle(ref rt, ref cmd, ref mat, ref a, ref v.t, ref v, k); else LoadTriangle(ref rt, ref cmd, ref mat, ref a, ref v.u, ref v, k);
			else if (0 == (i & 2))
				if (0 == (i & 1)) LoadTriangle(ref rt, ref cmd, ref mat, ref a, ref v.w, ref v, k); else LoadTriangle(ref rt, ref cmd, ref mat, ref a, ref v.x, ref v, k);
			else if (0 == (i & 1)) LoadTriangle(ref rt, ref cmd, ref mat, ref a, ref v.y, ref v, k); else LoadTriangle(ref rt, ref cmd, ref mat, ref a, ref v.z, ref v, k);
		}
		private static void LoadTriangle(
				ref Runtime rt,
				ref ByteSegment cmd,
				ref Material mat,
				ref F3D_Vertex a,
				ref F3D_Vertex b,
				ref F3D_VertexBuffer v,
				int i)
		{
			if (0 == (i & 8))
				if (0 == (i & 4))
					if (0 == (i & 2))
						if (0 == (i & 1)) rt.F3D_TRI1(ref cmd, ref mat, ref a, ref b, ref v.j); else rt.F3D_TRI1(ref cmd, ref mat, ref a, ref b, ref v.k);
					else if (0 == (i & 1)) rt.F3D_TRI1(ref cmd, ref mat, ref a, ref b, ref v.l); else rt.F3D_TRI1(ref cmd, ref mat, ref a, ref b, ref v.m);
				else if (0 == (i & 2))
					if (0 == (i & 1)) rt.F3D_TRI1(ref cmd, ref mat, ref a, ref b, ref v.n); else rt.F3D_TRI1(ref cmd, ref mat, ref a, ref b, ref v.o);
				else if (0 == (i & 1)) rt.F3D_TRI1(ref cmd, ref mat, ref a, ref b, ref v.p); else rt.F3D_TRI1(ref cmd, ref mat, ref a, ref b, ref v.q);
			else if (0 == (i & 4))
				if (0 == (i & 2))
					if (0 == (i & 1)) rt.F3D_TRI1(ref cmd, ref mat, ref a, ref b, ref v.r); else rt.F3D_TRI1(ref cmd, ref mat, ref a, ref b, ref v.s);
				else if (0 == (i & 1)) rt.F3D_TRI1(ref cmd, ref mat, ref a, ref b, ref v.t); else rt.F3D_TRI1(ref cmd, ref mat, ref a, ref b, ref v.u);
			else if (0 == (i & 2))
				if (0 == (i & 1)) rt.F3D_TRI1(ref cmd, ref mat, ref a, ref b, ref v.w); else rt.F3D_TRI1(ref cmd, ref mat, ref a, ref b, ref v.x);
			else if (0 == (i & 1)) rt.F3D_TRI1(ref cmd, ref mat, ref a, ref b, ref v.y); else rt.F3D_TRI1(ref cmd, ref mat, ref a, ref b, ref v.z);
		}


		private enum CMD : byte
		{
			F3D_NOOP = 0x00,
			F3D_MTX = 0x01,
			F3D_MOVEMEM = 0x03,
			F3D_VTX = 0x04,
			F3D_DL = 0x06,
			F3D_CLEARGEOMETRYMODE = 0xB6,
			F3D_SETGEOMETRYMODE = 0xB7,
			F3D_ENDDL = 0xB8,
			F3D_SETOTHERMODE_L = 0xB9,
			F3D_SETOTHERMODE_H = 0xBA,
			F3D_TEXTURE = 0xBB,
			F3D_MOVEWORD = 0xBC,
			F3D_POPMTX = 0xBD,
			F3D_CULLDL = 0xBE,
			F3D_TRI1 = 0xBF,
			G_TEXRECT = 0xE4,
			G_TEXRECTFLIP = 0xE5,
			G_RDPLOADSYNC = 0xE6,
			G_RDPPIPESYNC = 0xE7,
			G_RDPTILESYNC = 0xE8,
			G_RDPFULLSYNC = 0xE9,
			G_SETKEYGB = 0xEA,
			G_SETKEYR = 0xEB,
			G_SETCONVERT = 0xEC,
			G_SETSCISSOR = 0xED,
			G_SETPRIMDEPTH = 0xEE,
			G_RDPSETOTHERMODE = 0xEF,
			G_LOADTLUT = 0xF0,
			G_SETTILESIZE = 0xF2,
			G_LOADBLOCK = 0xF3,
			G_SETTILE = 0xF5,
			G_FILLRECT = 0xF6,
			G_SETFILLCOLOR = 0xF7,
			G_SETFOGCOLOR = 0xF8,
			G_SETBLENDCOLOR = 0xF9,
			G_SETPRIMCOLOR = 0xFA,
			G_SETENVCOLOR = 0xFB,
			G_SETCOMBINE = 0xFC,
			G_SETTIMG = 0xFD,
			G_SETZIMG = 0xFE,
			G_SETCIMG = 0xFF
		}
		static readonly BitList256 CMDList = (BitList256)UInt256.GetEnumDefined(typeof(CMD));

		//static Material buildingMaterial = Material.Default;

		private partial struct Runtime
		{
			public readonly ROM rom;
			public readonly Level lvl;
			public readonly Model3D mdl;
			public ByteSegment data;
			public F3D_VertexBuffer vb;
			public Material mat;
			private byte _seg;
			public int off;
			public bool end;
			public byte code;

			public byte seg
			{
				get => _seg;
				set
				{
					_seg = value;
					if (_seg == 0)
					{
						data = default(ByteSegment);
						end = true;
					}
					else
					{
						data = rom.getSegment(seg);
					}
				}
			}
			public bool GetCmd(out ByteSegment cmd)
			{
				if (end || off < 0 || _seg == 0 || off > data.Length - 8)
				{
					cmd = default(ByteSegment);
					end = true;
				}
				else
				{
					data.Segment((uint)off, 8u, out cmd);
					if (!CMDList.Contains(cmd[0]))
					{
						end = true;
						System.Console.Error.WriteLine("No such fast3d command! 0x" + (cmd[0]).ToString("X2"));
						//throw new Exception("UNDEFINED FAST3D COMMAND: 0x"+cmd[0].ToString("X2"));
					}
				}
				return !end;
			}
			public Runtime(Level level, Model3D mdl, byte drawLayerBillboard)
			{
				this.rom = level.rom;
				this.mat = Material.Default;
				this.mat.drawLayerBillboard = drawLayerBillboard;
				this.vb = default(F3D_VertexBuffer);
				this.lvl = level;
				this.mdl = mdl;
				this._seg = 0;
				this.code = 0;
				this.end = false;
				this.data = default(ByteSegment);
				this.off = -1;
			}
			public Runtime(ref Runtime copy, byte seg, uint off) : this(copy.lvl, copy.mdl, copy.mat.drawLayerBillboard)
			{
				this.mat = copy.mat;
				this.vb = copy.vb;
				this.seg = seg;
				this.off = (int)off;
			}
			public Runtime(ref Runtime copy, SegmentOffset segOff) : this(ref copy, segOff.Segment, segOff.Offset) { }

			public byte Execute() { Evaluate(); return code; }
			unsafe public void Evaluate()
			{
				ByteSegment cmd;
				while (GetCmd(out cmd))
				{
					//rom.printArray(cmd, 8);
					switch ((CMD)cmd[0])
					{
						case CMD.F3D_NOOP:
							CMD_00(cmd);
							break;
						case CMD.F3D_MOVEMEM:
							CMD_03(cmd);
							break;
						case CMD.F3D_VTX:
							CMD_04(cmd);
							break;
						case CMD.F3D_DL:
							CMD_06(cmd);
							break;
						case CMD.F3D_CLEARGEOMETRYMODE:
							CMD_B6(cmd);
							break;
						case CMD.F3D_SETGEOMETRYMODE:
							CMD_B7(cmd);
							break;
						case CMD.F3D_ENDDL:
							CMD_B8(cmd);
							break;
						case CMD.F3D_TEXTURE:
							CMD_BB(cmd);
							break;
						case CMD.F3D_TRI1:
							CMD_BF(cmd);
							break;
						case CMD.G_SETTILESIZE:
							CMD_F2(cmd);
							break;
						case CMD.G_LOADBLOCK:
							CMD_F3(cmd);
							break;
						case CMD.G_SETTILE:
							CMD_F5(cmd);
							break;
						case CMD.G_SETCOMBINE:
							CMD_FC(cmd);
							break;
						case CMD.G_SETTIMG:
							CMD_FD(cmd);
							break;
					}
					off += 8;
				}
			}
		}
		public static byte parse(
			Model3D mdl, Level lvl, byte seg, uint off, byte drawLayer = 1)
		{
			return new Runtime(lvl, mdl, drawLayer)
			{
				seg = seg,
				off = (int)off,
			}.Execute();
		}
		partial struct Runtime
		{
			public void CMD_00(ByteSegment cmd)
			{
				if (bytesToUInt24(cmd, 1) != 0)
				{
					code = cmd[0];
					end = true;
				}
			}
			public void CMD_03(ByteSegment cmd)
			{
				switchTextureStatus(ref mat, true);
				F3D_MOVEMEM(ref mat, ref cmd);
			}
			public void CMD_04(ByteSegment cmd)
			{
				switchTextureStatus(ref mat, false);
				//if (tempMaterial.id != 0) return;
				if (!F3D_VTX(ref cmd))
				{
					code = 0x04;
					end = true;
				}
			}
			public void CMD_06(ByteSegment cmd)
			{
				F3D_DL(ref cmd);
				if (cmd[1] == 1)
				{
					code = 0x06;
					end = true;
				}
			}
			public void CMD_B6(ByteSegment cmd)
			{
				mat.geometryMode &= ~bytesToUInt32(cmd, 4);
			}
			public void CMD_B7(ByteSegment cmd)
			{
				mat.geometryMode |= bytesToUInt32(cmd, 4);
			}
			public void CMD_B8(ByteSegment cmd)
			{
				end = true;
				code = 0x0B8;
			}
			public void CMD_BB(ByteSegment cmd)
			{
				F3D_TEXTURE(ref mat, ref cmd);
			}
			public void CMD_BF(ByteSegment cmd)
			{
				switchTextureStatus(ref mat, false);
				//if (tempMaterial.id != 0) return;
				F3D_TRI1(ref vb, ref mat, ref cmd);
			}
			public void CMD_F2(ByteSegment cmd)
			{
				switchTextureStatus(ref mat, true);
				G_SETTILESIZE(ref cmd, ref mat);
			}
			public void CMD_F3(ByteSegment cmd)
			{
			}
			public void CMD_F5(ByteSegment cmd)
			{
				G_SETTILE(ref mat, ref cmd);
			}
			public void CMD_F8(ByteSegment cmd)
			{
				mat.fogColor = (Color4b)bytesToUInt32(cmd, 4);
			}
			public void CMD_FB(ByteSegment cmd)
			{
				mat.combineColor = (Color4b)bytesToUInt32(cmd, 4);
			}
			public void CMD_FC(ByteSegment cmd)
			{
				if (G_SETCOMBINE(ref mat, ref cmd))
					switchTextureStatus(ref mat, true);
			}
			public void CMD_FD(ByteSegment cmd)
			{
				switchTextureStatus(ref mat, true);
				G_SETTIMG(ref mat, ref cmd);
			}
		}
		partial struct Runtime
		{
			private unsafe void switchTextureStatus(ref Material temp, bool status)
			{
				if (mdl.builder.processingTexture != status)
				{
					if (status == false)
					{
						if (
							!mdl.builder.hasTexture(temp.segOff, temp.Register()))
						{
							//System.Console.WriteLine("Adding new texture!");
							if (temp.segOff.Value != 0)
							{
								//System.Console.WriteLine("temp.segOff = " + temp.segOff.ToString("X8"));
								mdl.builder.AddTexture(
									rom.getTexture(ref temp),
									mdl.builder.newTexInfo(temp.wrapModes),
									temp.segOff, temp.Register()
								);
							}
							else
							{
								mdl.builder.AddTexture(
									TextureFormats.ColorTexture(System.Drawing.Color.FromArgb((int)temp.color)),
									mdl.builder.newTexInfo(temp.wrapModes),
									temp.segOff, temp.Register()
								);
							}
						}
					}
					mdl.builder.processingTexture = status;
				}
			}

			private unsafe void F3D_MOVEMEM(ref Material temp, ref ByteSegment cmd)
			{
				if (cmd[1] == 0x86)
				{
					//ROM rom = ROM.Instance;
					temp.color = (Color4b)(bytesToUInt24(
						rom.getDataFromSegmentAddress(bytesToSegmentOffset(cmd, 4), 3),
						0) | 0xFF000000u);
					//rom.printArray(colData, 4);
				}
				else if(0x88 == cmd[1])
				{

					temp.darkColor = (Color4b)(bytesToUInt24(
						rom.getDataFromSegmentAddress(bytesToSegmentOffset(cmd, 4), 3),
						0) | 0xFF000000u);
				}
				else
				{
					Console.WriteLine("Got a color:" + cmd[1].ToString("X2"));
				}
			}
			private unsafe bool F3D_VTX(ref ByteSegment cmd)
			{
				//ROM rom = ROM.Instance;
				ushort size = (ushort)(((uint)cmd[2] << 8) | cmd[3]);
				var segOff = bytesToSegmentOffset(cmd, 4);
				byte seg = segOff.Segment;
				uint off = segOff.Offset;
				bool ret;
				// Console.WriteLine("04: Amt = " + amount + ", Seg = " + seg.ToString("X2")+", Off = "+off.ToString("X6"));
				if ((ret = rom.hasSegment(seg)))
				{
					var vData = ROM.getSubArray(rom.getSegment(seg), off, size);
					for (uint i = 0; i < size; i += 0x10)
						F3D_VertexBuffer.Load(ref vb, ref vData, (int)(i / 0x10), (int)i);
				}
				return ret;
			}

			private unsafe void F3D_DL(ref ByteSegment cmd)
			{
				// i'm guessing we are to copy the material and vb back..
				var rt2 = new Runtime(ref this, bytesToSegmentOffset(cmd, 4));
				rt2.Evaluate();
				vb = rt2.vb;
				mat = rt2.mat;
			}
			private unsafe void F3D_TEXTURE(ref Material temp, ref ByteSegment cmd)
			{
				ushort tsX = (ushort)bytesToUInt16(cmd, 4);
				ushort tsY = (ushort)bytesToUInt16(cmd, 6);

				if ((temp.geometryMode & 0x40000) == 0x40000)
				{
					temp.w = (ushort)(tsX >> 6);
					temp.h = (ushort)(tsY >> 6);
				}
				else
				{
					temp.texScaleX = tsX;
					temp.texScaleY = tsY;
				}
			}

			private void LoadVertexShared(out Vector3 position, out Vector2 texCoord, ref Vertex128 v, ref Material mat)
			{
				position.X = v.x;
				position.Y = v.y;
				position.Z = v.z;

				texCoord.X = (float)(((int)v.u * mat.texScaleX) / (double)ushort.MaxValue);
				texCoord.Y = (float)(((int)v.v * mat.texScaleY) / (double)ushort.MaxValue);

			}
			private void LoadVertexShared(
				out Vector3 position, out Vector2 texCoord, out Vector3 normal, ref Vertex128 v, ref Material mat)
			{
				LoadVertexShared(out position, out texCoord, ref v, ref mat);
				normal.X = ByteUtility.b2n[v.nx_r];
				normal.Y = ByteUtility.b2n[v.ny_g];
				normal.Z = ByteUtility.b2n[v.nz_b];
			}
			private void LoadVertexShared(
				out Vector3 position, out Vector2 texCoord, out Vector4 color, ref Vertex128 v, ref Material mat)
			{
				LoadVertexShared(out position, out texCoord, ref v, ref mat);
				color.X = ByteUtility.b2f[v.nx_r];
				color.Y = ByteUtility.b2f[v.ny_g];
				color.Z = ByteUtility.b2f[v.nz_b];
				color.W = 1f;
			}
			private void LoadFaceNormal(ref F3D_Vertex a, ref F3D_Vertex b, ref F3D_Vertex c, out Vector3 normal)
			{
				Vector3d temp0, temp1, temp2;

				temp0.X = (int)b.x - a.x;
				temp0.Y = (int)b.y - a.y;
				temp0.Z = (int)b.z - a.z;
				temp2.X = temp0.LengthSquared;

				temp1.X = (int)c.x - a.x;
				temp1.Y = (int)c.y - a.y;
				temp1.Z = (int)c.z - a.z;
				temp2.Y = temp0.LengthSquared;

				if (temp2.X != 0.0 && temp2.Y != 0.0)
				{
					temp2.X = Math.Sqrt(temp2.X);
					temp2.Y = Math.Sqrt(temp2.Y);

					temp0.X /= temp2.X;
					temp0.Y /= temp2.X;
					temp0.Z /= temp2.X;

					temp1.X /= temp2.Y;
					temp1.Y /= temp2.Y;
					temp1.Z /= temp2.Y;

					Vector3d.Cross(ref temp0, ref temp1, out temp2);
					temp0.X = temp2.LengthSquared;
					if (temp0.X != 0.0)
					{
						temp0.X = Math.Sqrt(temp0.X);

						normal.X = (float)(temp2.X / temp0.X);
						normal.Y = (float)(temp2.Y / temp0.X);
						normal.Z = (float)(temp2.Z / temp0.X);
						return;
					}
				}
				normal.X = 0;
				normal.Y = 0;
				normal.Z = 0;
			}
			[ThreadStatic]
			static Vector3 temp_pos;

			[ThreadStatic]
			static Vector3 temp_normal;

			[ThreadStatic]
			static Vector4 temp_color;

			[ThreadStatic]
			static Vector2 temp_uv;

			[ThreadStatic]
			static Vertex128 temp_vertex;

			public void F3D_TRI1(
				ref ByteSegment cmd,
				ref Material temp,
				ref F3D_Vertex a,
				ref F3D_Vertex b,
				ref F3D_Vertex c)
			{
				if ((temp.geometryMode & 0x20000) != 0)
				{
					// face normal..
					temp_color.X = temp.color.r;
					temp_color.Y = temp.color.g;
					temp_color.Z = temp.color.b;
					temp_color.W = temp.color.a;

					temp_vertex = (Vertex128)a;
					LoadVertexShared(out temp_pos, out temp_uv, out temp_normal, ref temp_vertex, ref temp);
					mdl.builder.AddTempVertex(ref temp_vertex,ref temp_pos, ref temp_uv, ref temp_color, ref temp_normal);
					temp_vertex = (Vertex128)b;
					LoadVertexShared(out temp_pos, out temp_uv, out temp_normal, ref temp_vertex, ref temp);
					mdl.builder.AddTempVertex(ref temp_vertex, ref temp_pos, ref temp_uv, ref temp_color, ref temp_normal);

					temp_vertex = (Vertex128)c;
					LoadVertexShared(out temp_pos, out temp_uv, out temp_normal, ref temp_vertex, ref temp);
					mdl.builder.AddTempVertex(ref temp_vertex, ref temp_pos, ref temp_uv, ref temp_color, ref temp_normal);
				}
				else
				{
					LoadFaceNormal(
						ref a,
						ref b,
						ref c,
						out temp_normal);
					temp_vertex = (Vertex128)a;
					LoadVertexShared(out temp_pos, out temp_uv, out temp_color, ref temp_vertex, ref temp);
					mdl.builder.AddTempVertex(ref temp_vertex, ref temp_pos, ref temp_uv, ref temp_color, ref temp_normal);
					temp_vertex = (Vertex128)b;
					LoadVertexShared(out temp_pos, out temp_uv, out temp_color, ref temp_vertex, ref temp);
					mdl.builder.AddTempVertex(ref temp_vertex, ref temp_pos, ref temp_uv, ref temp_color, ref temp_normal);
					temp_vertex = (Vertex128)c;
					LoadVertexShared(out temp_pos, out temp_uv, out temp_color, ref temp_vertex, ref temp);
					mdl.builder.AddTempVertex(ref temp_vertex, ref temp_pos, ref temp_uv, ref temp_color, ref temp_normal);
				}

			}
			private unsafe void F3D_TRI1(
			ref F3D_VertexBuffer vertices,
			ref Material temp,
			ref ByteSegment cmd)
			{
				//System.Console.WriteLine("Adding new Triangle: " + a_pos + "," + b_pos + "," + c_pos);

				Fast3DScripts.LoadTriangle(ref this, ref cmd, ref temp, ref vertices,
					cmd[5] / 0x0A,
					cmd[6] / 0x0A,
					cmd[7] / 0x0A);
			}

			private unsafe void G_SETTILESIZE(ref ByteSegment cmd, ref Material temp)
			{
				temp.w = (ushort)((((cmd[5] << 8) | (cmd[6] & 0xF0)) >> 6) + 1);
				temp.h = (ushort)((((cmd[6] & 0x0F) << 8 | cmd[7]) >> 2) + 1);
			}

			private unsafe void G_SETTILE(ref Material temp, ref ByteSegment cmd)
			{
				if (cmd[4] == 0x00) // Make sure the tile is TX_RENDERTILE (0x0) and not TX_LOADTILE (0x7)
				{
					/* 
						The format for a texture should actually be used from SetTile (0xF5) command,
						and not the SetTextureImage (0xFD) command. If you used the format from 0xFD,
						then you will have issues with 4-bit textures. This is because the N64 4-bit 
						textures use 16-bit formats to load data.
					*/
					temp.format = cmd[1];
				}
				{
					var w1 = bytesToUInt32(cmd, 4);
					var t = (byte)((w1 >> 18) & 3);
					var s = (byte)((w1 >> 8) & 3);

					temp.wrapModes = (ushort)(((ushort)s << 8) | t);
				}
			}

			private unsafe bool G_SETCOMBINE(ref Material temp, ref ByteSegment cmd)
			{
				if ((~bytesToUInt24(cmd, 1)) == 0)
				{
					temp.segOff = 0;
					return true;
				}
				return false;
			}
			private unsafe void G_SETTIMG(ref Material temp, ref ByteSegment cmd)
			{
				temp.segOff = bytesToSegmentOffset(cmd, 4);
			}
		}

	}
}
