using OpenTK;
using System;
using BubblePony.Alloc;
using BubblePony.Integers;
using System.Runtime.InteropServices;
namespace Quad64.Scripts
{
	public sealed class Fast3DScripts : Script
	{
		private Fast3DScripts() : base(
			RuntimeType: typeof(Runtime),
			DelegateType: typeof(Runtime.Command),
			PreflightRunCommand: false)
		{ }

		protected sealed override byte EvaluateRuntime(Script.Runtime Source)
		{
			return Runtime.Evaluate((Runtime)Source);
		}
		protected sealed override Script.Runtime CloneRuntime(Script.Runtime Source)
		{
			return new Runtime((Runtime)Source);
		}

		private static readonly Fast3DScripts Instance = new Fast3DScripts();

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
			Runtime rt,
			ref ByteSegment cmd,
			ref Material mat,
			ref F3D_VertexBuffer v,
			int i, int j, int k)
		{
			if (0 == (i & 8))
				if (0 == (i & 4))
					if (0 == (i & 2))
						if (0 == (i & 1)) LoadTriangle(rt, ref cmd, ref mat, ref v.j, ref v, j, k); else LoadTriangle(rt, ref cmd, ref mat, ref v.k, ref v, j, k);
					else if (0 == (i & 1)) LoadTriangle(rt, ref cmd, ref mat, ref v.l, ref v, j, k); else LoadTriangle(rt, ref cmd, ref mat, ref v.m, ref v, j, k);
				else if (0 == (i & 2))
					if (0 == (i & 1)) LoadTriangle(rt, ref cmd, ref mat, ref v.n, ref v, j, k); else LoadTriangle(rt, ref cmd, ref mat, ref v.o, ref v, j, k);
				else if (0 == (i & 1)) LoadTriangle(rt, ref cmd, ref mat, ref v.p, ref v, j, k); else LoadTriangle(rt, ref cmd, ref mat, ref v.q, ref v, j, k);
			else if (0 == (i & 4))
				if (0 == (i & 2))
					if (0 == (i & 1)) LoadTriangle(rt, ref cmd, ref mat, ref v.r, ref v, j, k); else LoadTriangle(rt, ref cmd, ref mat, ref v.s, ref v, j, k);
				else if (0 == (i & 1)) LoadTriangle(rt, ref cmd, ref mat, ref v.t, ref v, j, k); else LoadTriangle(rt, ref cmd, ref mat, ref v.u, ref v, j, k);
			else if (0 == (i & 2))
				if (0 == (i & 1)) LoadTriangle(rt, ref cmd, ref mat, ref v.w, ref v, j, k); else LoadTriangle(rt, ref cmd, ref mat, ref v.x, ref v, j, k);
			else if (0 == (i & 1)) LoadTriangle(rt, ref cmd, ref mat, ref v.y, ref v, j, k); else LoadTriangle(rt, ref cmd, ref mat, ref v.z, ref v, j, k);
		}
		private static void LoadTriangle(
				Runtime rt,
				ref ByteSegment cmd,
				ref Material mat,
				ref F3D_Vertex a,
				ref F3D_VertexBuffer v,
				int i, int k)
		{
			if (0 == (i & 8))
				if (0 == (i & 4))
					if (0 == (i & 2))
						if (0 == (i & 1)) LoadTriangle(rt, ref cmd, ref mat, ref a, ref v.j, ref v, k); else LoadTriangle(rt, ref cmd, ref mat, ref a, ref v.k, ref v, k);
					else if (0 == (i & 1)) LoadTriangle(rt, ref cmd, ref mat, ref a, ref v.l, ref v, k); else LoadTriangle(rt, ref cmd, ref mat, ref a, ref v.m, ref v, k);
				else if (0 == (i & 2))
					if (0 == (i & 1)) LoadTriangle(rt, ref cmd, ref mat, ref a, ref v.n, ref v, k); else LoadTriangle(rt, ref cmd, ref mat, ref a, ref v.o, ref v, k);
				else if (0 == (i & 1)) LoadTriangle(rt, ref cmd, ref mat, ref a, ref v.p, ref v, k); else LoadTriangle(rt, ref cmd, ref mat, ref a, ref v.q, ref v, k);
			else if (0 == (i & 4))
				if (0 == (i & 2))
					if (0 == (i & 1)) LoadTriangle(rt, ref cmd, ref mat, ref a, ref v.r, ref v, k); else LoadTriangle(rt, ref cmd, ref mat, ref a, ref v.s, ref v, k);
				else if (0 == (i & 1)) LoadTriangle(rt, ref cmd, ref mat, ref a, ref v.t, ref v, k); else LoadTriangle(rt, ref cmd, ref mat, ref a, ref v.u, ref v, k);
			else if (0 == (i & 2))
				if (0 == (i & 1)) LoadTriangle(rt, ref cmd, ref mat, ref a, ref v.w, ref v, k); else LoadTriangle(rt, ref cmd, ref mat, ref a, ref v.x, ref v, k);
			else if (0 == (i & 1)) LoadTriangle(rt, ref cmd, ref mat, ref a, ref v.y, ref v, k); else LoadTriangle(rt, ref cmd, ref mat, ref a, ref v.z, ref v, k);
		}
		private static void LoadTriangle(
				Runtime rt,
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

		//static Material buildingMaterial = Material.Default;

		new private sealed partial class Runtime : Script.Runtime.Impl<Runtime>, ILevelProperty
		{
			public readonly Level lvl;
			public readonly Model3D mdl;
			Level ILevelProperty.Level => lvl;
			public F3D_VertexBuffer vb;
			public Material mat;
			public Runtime(
				Level level,
				Model3D mdl) : base(Instance, level.rom)
			{
				if (null == mdl) throw new ArgumentNullException("mdl");
				this.lvl = level;
				this.mdl = mdl;
			}
			public Runtime(Runtime copy) : base(copy)
			{
				this.lvl = copy.lvl;
				this.mdl = copy.mdl;
				this.mat = copy.mat;
			}
			public Runtime(Runtime copy, SegmentOffset segOff) : this(copy)
			{
				SetSegment(this, segOff);
			}
		}
		static class Late
		{
			public static readonly BitList256 CMDList = (BitList256)UInt256.GetEnumDefined(typeof(CMD));
		}
		protected sealed override CommandInfo GetCommandInfo(byte AA)
		{
			Runtime.Command Command;
			if (!Late.CMDList.Contains(AA))
				return null;


			switch ((CMD)AA)
			{
				case CMD.F3D_NOOP:					Command = Runtime.CMD_00;break;
				case CMD.F3D_MOVEMEM:				Command = Runtime.CMD_03;break;
				case CMD.F3D_VTX:					Command = Runtime.CMD_04;break;
				case CMD.F3D_DL:					Command = Runtime.CMD_06;break;
				case CMD.F3D_CLEARGEOMETRYMODE:		Command = Runtime.CMD_B6;break;
				case CMD.F3D_SETGEOMETRYMODE:		Command = Runtime.CMD_B7;break;
				case CMD.F3D_ENDDL:					Command = Runtime.CMD_B8;break;
				case CMD.F3D_TEXTURE:				Command = Runtime.CMD_BB;break;
				case CMD.F3D_TRI1:					Command = Runtime.CMD_BF;break;
				case CMD.G_SETTILESIZE:				Command = Runtime.CMD_F2;break;
				case CMD.G_LOADBLOCK:				Command = Runtime.CMD_F3;break;
				case CMD.G_SETTILE:					Command = Runtime.CMD_F5;break;
				case CMD.G_SETCOMBINE:				Command = Runtime.CMD_FC;break;
				case CMD.G_SETTIMG:					Command = Runtime.CMD_FD;break;
				case CMD.G_SETENVCOLOR:				Command = Runtime.CMD_FB;break;
				case CMD.G_SETFOGCOLOR:				Command = Runtime.CMD_F8;break;
				case CMD.G_SETFILLCOLOR:			Command = Runtime.CMD_F7;break;
				case CMD.G_SETBLENDCOLOR:			Command = Runtime.CMD_F9;break;
				case CMD.G_SETPRIMCOLOR:			Command = Runtime.CMD_FA;break;
				case CMD.F3D_SETOTHERMODE_H:		Command = Runtime.CMD_BA;break;
				case CMD.F3D_SETOTHERMODE_L:		Command = Runtime.CMD_B9;break;
				case CMD.G_RDPSETOTHERMODE:			Command = Runtime.CMD_EF;break;
				case CMD.F3D_MOVEWORD:				Command = Runtime.CMD_BC;break;
				case CMD.G_RDPPIPESYNC:
				case CMD.G_RDPTILESYNC:
				case CMD.G_RDPLOADSYNC:
				case CMD.G_RDPFULLSYNC:				Command = null; break;
				default:
					System.Console.WriteLine("Unhandled:" + (CMD)AA);Command = null; break;
					//throw new System.InvalidOperationException("Unhandled:" + (CMD)AA);
			}
			return new CommandInfo(this, Command, 8);
		}
		public static byte Parse(
			Model3D mdl,
			Level lvl,
			ref Material material,
			SegmentOffset segOff,
			byte overrideDrawLayer = 0)
		{
			var rt = new Runtime(lvl, mdl);

			rt.mat = material;

			if (0 != overrideDrawLayer)
				rt.mat.drawLayerBillboard = overrideDrawLayer;

			Runtime.SetSegment(rt, segOff);

			var ret = Runtime.Evaluate(rt);

			return ret;
		}
		partial class Runtime
		{
			public static void CMD_00(Runtime rt, ref ByteSegment cmd)
			{
				var segoff = bytesToSegmentOffset(cmd, 0);

				if (segoff.Offset != 0)
				{
					Exit(rt, segoff.Segment);
				}
			}
			public static void CMD_03(Runtime rt,ref ByteSegment cmd)
			{
				switchTextureStatus(rt, ref rt.mat, true);
				F3D_MOVEMEM(rt, ref rt.mat, ref cmd);
			}
			public static void CMD_04(Runtime rt,ref ByteSegment cmd)
			{
				switchTextureStatus(rt, ref rt.mat, false);
				//if (tempMaterial.id != 0) return;
				if (!rt.F3D_VTX(ref cmd))
				{
					Exit(rt, 0x04);
				}
			}
			public static void CMD_06(Runtime rt,ref ByteSegment cmd)
			{
				var rt2 = new Runtime(rt, bytesToSegmentOffset(cmd, 4));

				Evaluate(rt2);

				//when we branch (1)
				// we inherit whatever was done.
				if (cmd[1] == 1)
				{
					rt.mat = rt2.mat;
					rt.vb = rt2.vb;
					// and terminate.
					Exit(rt, 0x06);
				}
				// otherwise, we do not.
				rt.mat = rt2.mat;
				rt.vb = rt2.vb;
			}
			public static void CMD_B6(Runtime rt,ref ByteSegment cmd)
			{
				rt.mat.geometryMode &= ~bytesToUInt32(cmd, 4);
			}
			public static void CMD_B7(Runtime rt,ref ByteSegment cmd)
			{
				rt.mat.geometryMode |= bytesToUInt32(cmd, 4);
			}
			public static void CMD_B8(Runtime rt,ref ByteSegment cmd)
			{
				Exit(rt, 0x0B8);
			}

			public static void CMD_BA(Runtime rt,ref ByteSegment cmd)
			{
				//F3D_SETOTHERMODE_H
				ExpandSetMode(cmd, ref rt.mat.OtherModeH);
			}
			public static void CMD_BC(Runtime rt,ref ByteSegment cmd)
			{//MOVEWORD
				MOVEWORD(ref rt.mat, cmd);
			}
			public static void CMD_B9(Runtime rt, ref ByteSegment cmd)
			{
				//F3D_SETOTHERMODE_L
				ExpandSetMode(cmd, ref rt.mat.OtherModeL);
			}
			public static void CMD_EF(Runtime rt, ref ByteSegment cmd)
			{
				//	G_RDPSETOTHERMODE
				rt.mat.OtherModeH = (rt.mat.OtherModeH & ((1u << 24) - 1)) | 
					(bytesToUInt32(cmd) & ((1u << 24) - 1));
				rt.mat.OtherModeL = bytesToUInt32(cmd, 4);
			}
			public static void CMD_BB(Runtime rt,ref ByteSegment cmd)
			{
				F3D_TEXTURE(ref rt.mat, ref cmd);
			}
			public static void CMD_BF(Runtime rt,ref ByteSegment cmd)
			{
				switchTextureStatus(rt, ref rt.mat, false);
				//if (tempMaterial.id != 0) return;
				rt.F3D_TRI1(ref rt.vb, ref rt.mat, ref cmd);
			}
			public static void CMD_F2(Runtime rt,ref ByteSegment cmd)
			{
				switchTextureStatus(rt, ref rt.mat, true);
				G_SETTILESIZE(ref cmd, ref rt.mat);
			}
			public static void CMD_F3(Runtime rt,ref ByteSegment cmd)
			{
			}
			public static void CMD_F5(Runtime rt,ref ByteSegment cmd)
			{
				G_SETTILE(ref rt.mat, ref cmd);
			}
			public static void CMD_F7(Runtime rt,ref ByteSegment cmd)
			{
				rt.mat.fillColor.Value = bytesToUInt32(cmd, 4);
			}
			public static void CMD_F8(Runtime rt,ref ByteSegment cmd)
			{
				rt.mat.fogColor.Value = bytesToUInt32(cmd, 4);
			}
			public static void CMD_FB(Runtime rt,ref ByteSegment cmd)
			{
				rt.mat.envColor.Value = bytesToUInt32(cmd, 4);
			}
			public static void CMD_F9(Runtime rt,ref ByteSegment cmd)
			{
				rt.mat.blendColor.Value = bytesToUInt32(cmd, 4);
			}
			public static void CMD_FA(Runtime rt,ref ByteSegment cmd)
			{
				rt.mat.primColor.Value = bytesToUInt32(cmd, 4);
				rt.mat.primColorMin = cmd[2];
				rt.mat.primColorFactor = cmd[3];
			}
			public static void CMD_FC(Runtime rt,ref ByteSegment cmd)
			{
				if (G_SETCOMBINE(ref rt.mat, ref cmd))
					switchTextureStatus(rt,ref rt.mat, true);
			}
			public static void CMD_FD(Runtime rt,ref ByteSegment cmd)
			{
				switchTextureStatus(rt, ref rt.mat, true);
				G_SETTIMG(ref rt.mat, ref cmd);
			}
		}
		partial class Runtime
		{
			private static unsafe void switchTextureStatus(Runtime rt, ref Material temp, bool status)
			{
				if (rt.mdl.builder.processingTexture != status)
				{
					if (status == false)
					{
						if (
							!rt.mdl.builder.hasTexture(temp.segOff, temp.Register()))
						{
							//System.Console.WriteLine("Adding new texture!");
							if (temp.segOff.Value != 0)
							{
								//System.Console.WriteLine("temp.segOff = " + temp.segOff.ToString("X8"));
								rt.mdl.builder.AddTexture(
									rt.rom.getTexture(ref temp),
									rt.mdl.builder.newTexInfo(temp.wrapModes),
									temp.segOff, temp.Register()
								);
							}
							else
							{
								rt.mdl.builder.AddTexture(
									TextureFormats.ColorTexture((Color4b)temp.light.Lights0.Light1.Color.InverseAlpha),
									rt.mdl.builder.newTexInfo(temp.wrapModes),
									temp.segOff, temp.Register()
								);
							}
						}
					}
					rt.mdl.builder.processingTexture = status;
				}
			}

			private static void MOVEWORD(ref Material mat, ByteSegment cmd)
			{

				var offset = bytesToUInt16(cmd, 1);
				var index = cmd[3];
				if (index == 0)
				{
					mat.Mtx.Set(index >> 2, (uint)bytesToUInt32(cmd, 4));
				}
				else if (index == 2)
				{
					mat.numLightWord = bytesToUInt32(cmd, 4);
					//var numlight = ((mat.numLightWord - 0x80000000u)/32)-1;
					//Console.WriteLine("NumLight=" + numlight);
				}
				else if (index == 4)
				{
					if (offset == 0x04)
						mat.clipNX = bytesToUInt32(cmd, 4);
					else if (offset == 0x0C)
						mat.clipNY = bytesToUInt32(cmd, 4);
					else if (offset == 0x14)
						mat.clipPX = bytesToUInt32(cmd, 4);
					else if (offset == 0x1C)
						mat.clipPY = bytesToUInt32(cmd, 4);
					else
						Console.WriteLine("Weird clip offset:" + offset.ToString("X2"));
				}
				else if (index == 8)
				{
					mat.fogWord = bytesToUInt32(cmd, 4);
				}
				else if (index == 10)
				{
					mat.light[offset >> 5, 3 & (offset >> 2)] = bytesToUInt32(cmd, 4);
				}
				// todo: 6 = segment, 12 = points, 14 = perspnorm.
			}
			private static void F3D_MOVEMEM(Runtime rt, ref Material temp, ref ByteSegment cmd)
			{
				var p = cmd[1];
				var size = bytesToUInt16(cmd, 2);
				var data = rt.rom.getDataFromSegmentAddress(bytesToSegmentOffset(cmd, 4), size);
				if (p >= 0x86 && p <= 0x94)
				{
					if (size != 16) throw new System.InvalidOperationException();

					temp.light[(byte)((p - 0x86) >> 1)] = new Light
					{
						Color = { Value = bytesToUInt32(data, 0), },
						ColorCopy = { Value = bytesToUInt32(data, 4), },
						Normal = { Packed = bytesToUInt32(data, 8), },
						WordPad = bytesToUInt32(data, 12),
					};
				}
				else
				{
					Console.WriteLine("Some other memory:" + p.ToString("X2"));
				}
			}
			static void ExpandSetMode(ByteSegment cmd, ref uint bits)
			{
				unchecked
				{
					bits = (bits & ~((uint)((1uL << (sbyte)cmd[3]) - 1uL) << (sbyte)cmd[2]))
						| bytesToUInt32(cmd, 4);
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
			private static unsafe void F3D_TEXTURE(ref Material temp, ref ByteSegment cmd)
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

			private static void LoadVertexShared(out Vector3 position, out Vector2 texCoord, ref Vertex128 v, ref Material mat)
			{
				position.X = v.x;
				position.Y = v.y;
				position.Z = v.z;

				texCoord.X = (float)(((int)v.u * mat.texScaleX) / (double)ushort.MaxValue);
				texCoord.Y = (float)(((int)v.v * mat.texScaleY) / (double)ushort.MaxValue);

			}
			private static void LoadVertexShared(
				out Vector3 position, out Vector2 texCoord, out Vector3 normal, ref Vertex128 v, ref Material mat)
			{
				LoadVertexShared(out position, out texCoord, ref v, ref mat);
				normal.X = ByteUtility.b2n[v.nx_r];
				normal.Y = ByteUtility.b2n[v.ny_g];
				normal.Z = ByteUtility.b2n[v.nz_b];
			}
			private static void LoadVertexShared(
				out Vector3 position, out Vector2 texCoord, out Vector4 color, ref Vertex128 v, ref Material mat)
			{
				LoadVertexShared(out position, out texCoord, ref v, ref mat);
				color.X = ByteUtility.b2f[v.nx_r];
				color.Y = ByteUtility.b2f[v.ny_g];
				color.Z = ByteUtility.b2f[v.nz_b];
				color.W = 1f;
			}
			private static void LoadFaceNormal(ref F3D_Vertex a, ref F3D_Vertex b, ref F3D_Vertex c, out Vector3 normal)
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
					temp_color.X = temp.light.Light1.Color.r;
					temp_color.Y = temp.light.Light1.Color.g;
					temp_color.Z = temp.light.Light1.Color.b;
					temp_color.W = 1f-temp.light.Light1.Color.a;

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

				Fast3DScripts.LoadTriangle(this, ref cmd, ref temp, ref vertices,
					cmd[5] / 0x0A,
					cmd[6] / 0x0A,
					cmd[7] / 0x0A);
			}

			private static unsafe void G_SETTILESIZE(ref ByteSegment cmd, ref Material temp)
			{
				temp.w = (ushort)((((cmd[5] << 8) | (cmd[6] & 0xF0)) >> 6) + 1);
				temp.h = (ushort)((((cmd[6] & 0x0F) << 8 | cmd[7]) >> 2) + 1);
			}

			private static unsafe void G_SETTILE(ref Material temp, ref ByteSegment cmd)
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

			static private unsafe bool G_SETCOMBINE(ref Material temp, ref ByteSegment cmd)
			{
				temp.CombinerH = bytesToUInt32(cmd, 0);
				temp.CombinerL = bytesToUInt32(cmd, 4);

				if ((~bytesToUInt24(cmd, 1)) == 0)
				{
					temp.segOff = 0;
					return true;
				}
				return false;
			}
			private static void G_SETTIMG(ref Material temp, ref ByteSegment cmd)
			{
				temp.segOff = bytesToSegmentOffset(cmd, 4);
			}
		}

	}
}
