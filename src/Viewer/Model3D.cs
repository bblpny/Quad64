using OpenTK;
using OpenTK.Graphics.OpenGL;
using Quad64.Scripts;
using Quad64.src.Viewer;
using System;
using System.Collections.Generic;
using BubblePony.ExportUtility;
using Wavefront = BubblePony.Wavefront;
using Export = BubblePony.Export;
using System.Runtime.InteropServices;
using BubblePony.GLHandle;
namespace Quad64
{
	using Temporary = BubblePony.Temporary;

	[StructLayout(LayoutKind.Explicit, Size = 16, Pack = 1)]
	public struct Vertex128 : Temporary.IStruct<Vertex128>
	{
		private struct Handler : Temporary.IStructHandler<Vertex128>
		{
			Temporary.StructFlags Temporary.IStructHandler<Vertex128>.StructFlags { get => 0; }

			bool Temporary.IStructHandler<Vertex128>.Equals(ref Vertex128 Left, ref Vertex128 Right)
			{
				return (Block128)Left == (Block128)Right;
			}
			
			void Temporary.IStructHandler<Vertex128>.SetupHashCode(ref Vertex128 Value, out int HashCode)
			{
				HashCode = ((Block128)Value).GetHashCode();
			}
		}
		Temporary.IStructHandler<Vertex128> Temporary.IStruct<Vertex128>.Handler { get => default(Handler); }
		[FieldOffset(0)]
		public short x;
		[FieldOffset(2)]
		public short y;
		[FieldOffset(4)]
		public short z;
		[FieldOffset(6)]
		public ushort f;
		[FieldOffset(8)]
		public short u;
		[FieldOffset(10)]
		public short v; // f = flag (Not sure what it does)
		[FieldOffset(12)]
		public byte nx_r;
		[FieldOffset(13)]
		public byte ny_g;
		[FieldOffset(14)]
		public byte nz_b;
		[FieldOffset(15)]
		public byte a;

		private static int ROT(int v, byte f)
		{
			return (v << (sbyte)f) | ((v >> (32 - f)) & ((1 << (sbyte)f) - 1));
		}
		public override bool Equals(object obj)
		{
			return obj is Vertex128 && Equals((Vertex128)obj);
		}
		public bool Equals(Vertex128 other)
		{
			return x == other.x && y == other.y && z == other.z && f == other.f &&
				u == other.u && v == other.v && nx_r == other.nx_r && ny_g == other.ny_g &&
				nz_b == other.nz_b && a == other.a;
		}
		public static bool operator ==(Vertex128 L, Vertex128 R)
		{
			return L.x == R.x && L.y == R.y && L.z == R.z && L.f == R.f &&
				L.u == R.u && L.v == R.v && L.nx_r == R.nx_r && L.ny_g == R.ny_g &&
				L.nz_b == R.nz_b && L.a == R.a;
		}
		public static bool operator !=(Vertex128 L, Vertex128 R)
		{
			return L.x != R.x || L.y != R.y || L.z != R.z || L.f != R.f ||
				L.u != R.u || L.v != R.v || L.nx_r != R.nx_r || L.ny_g != R.ny_g ||
				L.nz_b != R.nz_b || L.a != R.a;
		}
		public static bool Equals(ref Vertex128 L, ref Vertex128 R)
		{
			return L.x == R.x && L.y == R.y && L.z == R.z && L.f == R.f &&
				L.u == R.u && L.v == R.v && L.nx_r == R.nx_r && L.ny_g == R.ny_g &&
				L.nz_b == R.nz_b && L.a == R.a;
		}
		public static bool Inequals(ref Vertex128 L, ref Vertex128 R)
		{
			return L.x != R.x || L.y != R.y || L.z != R.z || L.f != R.f ||
				L.u != R.u || L.v != R.v || L.nx_r != R.nx_r || L.ny_g != R.ny_g ||
				L.nz_b != R.nz_b || L.a != R.a;
		}
		public override int GetHashCode()
		{
			return
				((int)a << 24 | (int)nx_r << 16 | (int)ny_g << 8 | (int)nz_b) ^
				ROT(((int)x << 16) | ((int)y & ushort.MaxValue), 2) ^
				ROT(((int)z << 16) | f, 4) ^
				ROT(((int)u << 16) | ((int)v & ushort.MaxValue), 6);
		}
		public override string ToString()
		{
			return string.Format("{{\"pos\":{0},\"tex\":[{1},{2}],\"atr\":{3},\"flag\":{4}}}",
				new Vector3s { X = x, Y = y, Z = z, },
				u, v, new Vector4b { X = nx_r, Y = ny_g, Z = nz_b, W = a, },
				f);
		}
		[StructLayout(LayoutKind.Explicit, Size = 16, Pack = 1)]
		public struct Blocked
		{
			[FieldOffset(0)]
			public Vertex128 Vertex;

			[FieldOffset(0)]
			public Block128 Block;
		}
		public static explicit operator Block128(Vertex128 Vertex) { return new Blocked { Vertex = Vertex, }.Block; }
		public static explicit operator Vertex128(Block128 Block) { return new Blocked { Block = Block, }.Vertex; }
	}
	[StructLayout(LayoutKind.Explicit, Size = 16, Pack = 1)]
	public struct Block128 : IEquatable<Block128>
	{
		[FieldOffset(0)]
		public byte A;
		[FieldOffset(1)]
		public byte B;
		[FieldOffset(2)]
		public byte C;
		[FieldOffset(3)]
		public byte D;
		[FieldOffset(4)]
		public byte E;
		[FieldOffset(5)]
		public byte F;
		[FieldOffset(6)]
		public byte G;
		[FieldOffset(7)]
		public byte H;
		[FieldOffset(8)]
		public byte I;
		[FieldOffset(9)]
		public byte J;
		[FieldOffset(10)]
		public byte K;
		[FieldOffset(11)]
		public byte L;
		[FieldOffset(12)]
		public byte M;
		[FieldOffset(13)]
		public byte N;
		[FieldOffset(14)]
		public byte O;
		[FieldOffset(15)]
		public byte P;
		[FieldOffset(0)]
		public ushort AB;
		[FieldOffset(2)]
		public ushort CD;
		[FieldOffset(4)]
		public ushort EF;
		[FieldOffset(6)]
		public ushort GH;
		[FieldOffset(8)]
		public ushort IJ;
		[FieldOffset(10)]
		public ushort KL;
		[FieldOffset(12)]
		public ushort MN;
		[FieldOffset(14)]
		public ushort OP;
		[FieldOffset(0)]
		public uint ABCD;
		[FieldOffset(4)]
		public uint EFGH;
		[FieldOffset(8)]
		public uint IJKL;
		[FieldOffset(12)]
		public uint MNOP;
		[FieldOffset(0)]
		public ulong ABCDEFGH;
		[FieldOffset(8)]
		public ulong IJKLMNOP;
		private static int ROT(int v, byte f)
		{
			return (v << (sbyte)f) | ((v >> (32 - f)) & ((1 << (sbyte)f) - 1));
		}
		public override bool Equals(object obj)
		{
			return obj is Block128 && Equals((Block128)obj);
		}
		public bool Equals(Block128 other)
		{
			return ABCDEFGH==other.ABCDEFGH && IJKLMNOP == other.IJKLMNOP;
		}
		public static bool operator ==(Block128 L, Block128 R)
		{
			return L.ABCDEFGH == R.ABCDEFGH && L.IJKLMNOP == R.IJKLMNOP;
		}
		public static bool operator !=(Block128 L, Block128 R)
		{
			return L.ABCDEFGH != R.ABCDEFGH || L.IJKLMNOP != R.IJKLMNOP;
		}
		public static bool Equals([In]ref Block128 L, [In]ref Block128 R)
		{
			return L.ABCDEFGH == R.ABCDEFGH && L.IJKLMNOP == R.IJKLMNOP;
		}
		public static bool Inequals([In]ref Block128 L, [In]ref Block128 R)
		{
			return L.ABCDEFGH != R.ABCDEFGH || L.IJKLMNOP != R.IJKLMNOP;
		}
		public override int GetHashCode()
		{
			unchecked
			{
				return
					(int)MNOP ^
					ROT((int)ABCD, 2) ^
					ROT((int)EFGH, 4) ^
					ROT((int)IJKL, 6);
			}
		}
		public unsafe void StoTo(byte* buf)
		{
			buf[0] = A;
			buf[1] = B;
			buf[2] = C;
			buf[3] = D;
			buf[4] = E;
			buf[5] = F;
			buf[6] = G;
			buf[7] = H;
			buf[8] = I;
			buf[9] = J;
			buf[10] = K;
			buf[11] = L;
			buf[12] = M;
			buf[13] = N;
			buf[14] = O;
			buf[15] = P;

		}
		unsafe public override string ToString()
		{
			int i, j;
			byte* buf = stackalloc byte[35];
			buf[35] = 0;
			buf[0] = (byte)'0';
			buf[1] = (byte)'x';
			StoTo(buf + 2);
			for (i = 16; i != 0; --i)
			{
				buf[(j = (i << 1))] = (byte)(buf[32] >> 4);
				buf[j | 1] = (byte)(buf[32] & 15);
			}
			for (i = 34; i > 1; --i)
				buf[i] += buf[i] < 10 ? (byte)'0' : (byte)('A' - 10);
			return new string((sbyte*)buf, 0, 35);
		}
	}

	public sealed class Model3D : Export.Reference<Model3D>, Export.Reference
	{
        public sealed class MeshData : Export.Reference<MeshData>, Export.Reference
		{
            public Vector3[] vertices;
			public Vector3[] normals;
			public Vector2[] texCoord;
            public Vector4[] colors;
            public uint[] indices;
            public Texture2D texture;
			public Material material;
			public GraphicsHandle.Buffer vbo, ibo, texBuf, colorBuf, normalBuf;

			private Wavefront.Material GenerateMaterial(Wavefront.Model Model)
			{
				Wavefront.MaterialFilter Kd = default(Wavefront.MaterialFilter);
				Wavefront.MaterialMap map_d = default(Wavefront.MaterialMap);

				if (material.HasMaterialColor)
				{
					var color = (Color4b)material.color;

					Kd = new Wavefront.MaterialFilter(color.r, color.g, color.b);
				}
				Wavefront.Image image = null;
				if (material.HasTexture)
				{
					byte[] pixels = new byte[(texture.Width * texture.Height) << 2];
					GL.GetTextureImage(texture.ID, 0, PixelFormat.Rgba, PixelType.UnsignedInt8888, pixels.Length, pixels);
					for (int i = (texture.Width * texture.Height) - 1; i >= 0; i--)
					{
						byte r = pixels[(i << 2) | 1];
						byte g = pixels[(i << 2) | 2];
						byte b = pixels[(i << 2) | 3];
						byte a = pixels[(i << 2) | 0];

						pixels[(i << 2) | 0] = r;
						pixels[(i << 2) | 1] = g;
						pixels[(i << 2) | 2] = b;
						pixels[(i << 2) | 3] = a;
					}
					image = Model.Images[new Wavefront.Image(pixels, (ushort)texture.Width, (ushort)texture.Height)];

					map_d = new Wavefront.MaterialMap(
						image,
						Clamp: (material.HasClampUV) ? true : (bool?)null,
						AutoTexRes: true);
				}

				var map_Ka=map_d;

				if (!map_d.IsOmitted)
				{
					if (
						TextureFormats.GetFormat(material.format).AlphaDepth == 0 ||
						(!image.AlphaVariance && image.AlphaValue == 255))
						map_d = default(Wavefront.MaterialMap);
					else
						map_d = new Wavefront.MaterialMap(ref map_d,
							Channel: Wavefront.TextureChannel.Matte);
				}

				return Model.Materials[new Wavefront.Material(
					Kd: Kd,
					Ka: Kd,
					map_Ka: map_Ka,
					map_Kd: map_Ka,
					map_d: map_d,
					d: map_d.IsOmitted ? default(Wavefront.MaterialD) : (Wavefront.MaterialD)0.0,
					illum: material.CalcuateDefaultIlluminationMode(image == (object)null ? true : (!image.AlphaVariance && image.AlphaValue == 255))
					)];
			}
			static Wavefront.Vector2d WavefrontUV(ref Vector2 input)
			{
				return new Wavefront.Vector2d { X = input.X, Y = 1.0 - input.Y, };
			}
			static Wavefront.Vector2 ToWavefront(ref Vector2 input)
			{
				return new Wavefront.Vector2 { X = input.X, Y = input.Y, };
			}
			static Wavefront.Vector3 ToWavefront(ref Vector3 input)
			{
				return new Wavefront.Vector3 { X = input.X, Y = input.Y, Z = input.Z };
			}
			static Wavefront.Vector4 ToWavefront(ref Vector4 input)
			{
				return new Wavefront.Vector4 { X = input.X, Y = input.Y, Z = input.Z, W = input.W, };
			}
			static Wavefront.Vector2d ToWavefront(ref Vector2d input)
			{
				return new Wavefront.Vector2d { X = input.X, Y = input.Y, };
			}
			static Wavefront.Vector3d ToWavefront(ref Vector3d input)
			{
				return new Wavefront.Vector3d { X = input.X, Y = input.Y, Z = input.Z };
			}
			static Wavefront.Vector4d ToWavefront(ref Vector4d input)
			{
				return new Wavefront.Vector4d { X = input.X, Y = input.Y, Z = input.Z, W = input.W, };
			}
			public static Wavefront.Model operator +(Wavefront.Model Model, MeshData Data)
			{
				Model.Material = Data.GenerateMaterial(Model);
				Wavefront.Vertex A, B, C;
				int ivert;
				if (Data.material.IsLit)
				if (Data.material.HasTexture)
				{
					if (Data.colors != null && Data.colors.Length == Data.vertices.Length)
					{
						for (int i_index = 0, n_index = Data.indices.Length; i_index < n_index;)
						{
							ivert = (int)Data.indices[i_index++];
							A = Model.Vertices[
								Model.Positions[ToWavefront(ref Data.vertices[ivert]), ToWavefront(ref Data.colors[ivert])],
								Model.Texcoords[WavefrontUV(ref Data.texCoord[ivert])],
								Model.Normals[ToWavefront(ref Data.normals[ivert])]
								];
							ivert = (int)Data.indices[i_index++];
							B = Model.Vertices[
								Model.Positions[ToWavefront(ref Data.vertices[ivert]), ToWavefront(ref Data.colors[ivert])],
								Model.Texcoords[WavefrontUV(ref Data.texCoord[ivert])],
								Model.Normals[ToWavefront(ref Data.normals[ivert])]
								];
							ivert = (int)Data.indices[i_index++];
							C = Model.Vertices[
								Model.Positions[ToWavefront(ref Data.vertices[ivert]), ToWavefront(ref Data.colors[ivert])],
								Model.Texcoords[WavefrontUV(ref Data.texCoord[ivert])],
								Model.Normals[ToWavefront(ref Data.normals[ivert])]
								];
							Model.Publish(Model.Facets[A, B, C]);
						}
					}
					else
					{
						for (int i_index = 0, n_index = Data.indices.Length; i_index < n_index;)
						{
							ivert = (int)Data.indices[i_index++];
							A = Model.Vertices[
								Model.Positions[ToWavefront(ref Data.vertices[ivert])],
								Model.Texcoords[WavefrontUV(ref Data.texCoord[ivert])],
								Model.Normals[ToWavefront(ref Data.normals[ivert])]
								];
							ivert = (int)Data.indices[i_index++];
							B = Model.Vertices[
								Model.Positions[ToWavefront(ref Data.vertices[ivert])],
								Model.Texcoords[WavefrontUV(ref Data.texCoord[ivert])],
								Model.Normals[ToWavefront(ref Data.normals[ivert])]
								];
							ivert = (int)Data.indices[i_index++];
							C = Model.Vertices[
								Model.Positions[ToWavefront(ref Data.vertices[ivert])],
								Model.Texcoords[WavefrontUV(ref Data.texCoord[ivert])],
								Model.Normals[ToWavefront(ref Data.normals[ivert])]
								];
							Model.Publish(Model.Facets[A, B, C]);
						}
					}
				}
				else if (Data.colors != null && Data.colors.Length == Data.vertices.Length)
				{
					for (int i_index = 0, n_index = Data.indices.Length; i_index < n_index;)
					{
						ivert = (int)Data.indices[i_index++];
						A = Model.Vertices[
							Model.Positions[ToWavefront(ref Data.vertices[ivert]), ToWavefront(ref Data.colors[ivert])],
								Model.Normals[ToWavefront(ref Data.normals[ivert])]
							];
						ivert = (int)Data.indices[i_index++];
						B = Model.Vertices[
							Model.Positions[ToWavefront(ref Data.vertices[ivert]), ToWavefront(ref Data.colors[ivert])],
								Model.Normals[ToWavefront(ref Data.normals[ivert])]
							];
						ivert = (int)Data.indices[i_index++];
						C = Model.Vertices[
							Model.Positions[ToWavefront(ref Data.vertices[ivert]), ToWavefront(ref Data.colors[ivert])],
								Model.Normals[ToWavefront(ref Data.normals[ivert])]
							];
						Model.Publish(Model.Facets[A, B, C]);
					}
				}
					else
						for (int i_index = 0, n_index = Data.indices.Length; i_index < n_index;)
						{
							ivert = (int)Data.indices[i_index++];
							A = Model.Vertices[
								Model.Positions[ToWavefront(ref Data.vertices[ivert])],
								Model.Normals[ToWavefront(ref Data.normals[ivert])]
								];
							ivert = (int)Data.indices[i_index++];
							B = Model.Vertices[
								Model.Positions[ToWavefront(ref Data.vertices[ivert])],
								Model.Normals[ToWavefront(ref Data.normals[ivert])]
								];
							ivert = (int)Data.indices[i_index++];
							C = Model.Vertices[
								Model.Positions[ToWavefront(ref Data.vertices[ivert])],
								Model.Normals[ToWavefront(ref Data.normals[ivert])]
								];
							Model.Publish(Model.Facets[A, B, C]);
						}
				else
				if (Data.material.HasTexture)
				{
					if (Data.colors != null && Data.colors.Length == Data.vertices.Length)
					{
						for (int i_index = 0, n_index = Data.indices.Length; i_index < n_index;)
						{
							ivert = (int)Data.indices[i_index++];
							A = Model.Vertices[
								Model.Positions[ToWavefront(ref Data.vertices[ivert]), ToWavefront(ref Data.colors[ivert])],
								Model.Texcoords[WavefrontUV(ref Data.texCoord[ivert])]
								];
							ivert = (int)Data.indices[i_index++];
							B = Model.Vertices[
								Model.Positions[ToWavefront(ref Data.vertices[ivert]), ToWavefront(ref Data.colors[ivert])],
								Model.Texcoords[WavefrontUV(ref Data.texCoord[ivert])]
								];
							ivert = (int)Data.indices[i_index++];
							C = Model.Vertices[
								Model.Positions[ToWavefront(ref Data.vertices[ivert]), ToWavefront(ref Data.colors[ivert])],
								Model.Texcoords[WavefrontUV(ref Data.texCoord[ivert])]
								];
							Model.Publish(Model.Facets[A, B, C]);
						}
					}
					else
					{
						for (int i_index = 0, n_index = Data.indices.Length; i_index < n_index;)
						{
							ivert = (int)Data.indices[i_index++];
							A = Model.Vertices[
								Model.Positions[ToWavefront(ref Data.vertices[ivert])],
								Model.Texcoords[WavefrontUV(ref Data.texCoord[ivert])]
								];
							ivert = (int)Data.indices[i_index++];
							B = Model.Vertices[
								Model.Positions[ToWavefront(ref Data.vertices[ivert])],
								Model.Texcoords[WavefrontUV(ref Data.texCoord[ivert])]
								];
							ivert = (int)Data.indices[i_index++];
							C = Model.Vertices[
								Model.Positions[ToWavefront(ref Data.vertices[ivert])],
								Model.Texcoords[WavefrontUV(ref Data.texCoord[ivert])]
								];
							Model.Publish(Model.Facets[A, B, C]);
						}
					}
				}
				else if (Data.colors != null && Data.colors.Length == Data.vertices.Length)
				{
					for (int i_index = 0, n_index = Data.indices.Length; i_index < n_index;)
					{
						ivert = (int)Data.indices[i_index++];
						A = Model.Vertices[
							Model.Positions[ToWavefront(ref Data.vertices[ivert]), ToWavefront(ref Data.colors[ivert])]
							];
						ivert = (int)Data.indices[i_index++];
						B = Model.Vertices[
							Model.Positions[ToWavefront(ref Data.vertices[ivert]), ToWavefront(ref Data.colors[ivert])]
							];
						ivert = (int)Data.indices[i_index++];
						C = Model.Vertices[
							Model.Positions[ToWavefront(ref Data.vertices[ivert]), ToWavefront(ref Data.colors[ivert])]
							];
						Model.Publish(Model.Facets[A, B, C]);
					}
				}
				else for (int i_index = 0, n_index = Data.indices.Length; i_index < n_index;)
					{
						Model.Publish(Model.Facets[
							Model.Vertices[
							Model.Positions[ToWavefront(ref Data.vertices[(int)Data.indices[i_index++]])]
							],
							Model.Vertices[
							Model.Positions[ToWavefront(ref Data.vertices[(int)Data.indices[i_index++]])]
							],
							Model.Vertices[
							Model.Positions[ToWavefront(ref Data.vertices[(int)Data.indices[i_index++]])]
							]]);
					}
				return Model;
			}

            public override string ToString() {
                return "Texture [ID/W/H]: [" + texture.ID + "/" + texture.Width + "/" + texture.Height + "]";
            }

			public static Export.ReferenceRegister<MeshData> ExportRegister;
			Export.TypeReference Export.Reference.API() { return ExportRegister.Singleton; }
			void Export.Reference<MeshData>.API(Export.Exporter ex)
			{
				ex.Ref(texture);
				ex.Array(vertices);
				ex.Array(texCoord);
				ex.Array(colors);
				ex.Array(indices);
			}
			static Vector3[] RotateCopy(Vector3[] i, Quaternion rot)
			{
				Vector3[] o = new
					Vector3[i.Length];
				for (int j = i.Length - 1; j >= 0; --j)
					o[j] = rot * i[j];
				return o;
			}
			static Vector3[] TransformCopy(Vector3[] i, Transform transform)
			{
				Vector3[] o = new
					Vector3[i.Length];

				for (int j = i.Length - 1; j >= 0; --j)
					o[j] = transform.TransformPosition(i[j]);

				return o;
			}
			public MeshData GetTransformed(Transform transform)
			{
				return new MeshData
				{
					ibo = ibo,
					colorBuf = colorBuf,
					colors = colors,
					indices = indices,
					material = material,
					normalBuf = normalBuf,
					normals = RotateCopy(normals, transform.rotation),
					texBuf = texBuf,
					texCoord = texCoord,
					texture = texture,
					vbo = vbo,
					vertices = TransformCopy(vertices, transform),
				};
			}
		}
        Vector3 center = new Vector3(0, 0, 0);
        Vector3 upper = new Vector3(0, 0, 0);
        Vector3 lower = new Vector3(0, 0, 0);
		internal GeoRoot root;
        public Vector3 UpperBoundary { get { return upper; } }
        public Vector3 LowerBoundary { get { return lower; } }
        public uint GeoDataSegAddress { get; set; }
        public ModelBuilder builder = new ModelBuilder();
        public readonly List<MeshData> meshes = new List<MeshData>();

        private readonly HashSet<uint> geoDisplayLists = new HashSet<uint>();

        public bool hasGeoDisplayList(uint value)
        {
			return !geoDisplayLists.Add(value);
        }

        private void calculateCenter()
        {
            float max_x = -1, min_x = -1, max_y = -1, min_y = -1, max_z = -1, min_z = -1;
            uint count = 0;
            foreach (MeshData md in meshes)
            {
                foreach (Vector3 vec in md.vertices)
                {
                    if (count == 0)
                    {
                        min_x = vec.X; max_x = vec.X;
                        min_y = vec.Y; max_y = vec.Y;
                        min_z = vec.Z; max_z = vec.Z;
                    }
                    else
                    {
                        if (vec.X < min_x)
                            min_x = vec.X;
                        if (vec.X > max_x)
                            max_x = vec.X;
                        if (vec.Y < min_y)
                            min_y = vec.Y;
                        if (vec.Y > max_y)
                            max_y = vec.Y;
                        if (vec.Z < min_z)
                            min_z = vec.Z;
                        if (vec.Z > max_z)
                            max_z = vec.Z;
                         /*Console.WriteLine("Values: [" + max_x + ", " +min_x + ", " +max_y + ", " +min_y + ", " +max_z + ", " + min_z +"]");*/
                    }
                    count++;
                }
            }
            center = new Vector3((max_x+min_x) / 2, (max_y + min_y) / 2, (max_z + min_z) / 2);
            upper = new Vector3(max_x, max_y, max_z);
            lower = new Vector3(min_x, min_y, min_z);
        }

        public void outputTextureAtlasToPng(string filename)
        {
			List<System.Drawing.Bitmap> bitmaps;
			var raws = builder.TextureImages;
			bitmaps = new List<System.Drawing.Bitmap>(raws.Length);
			for(int i = raws.Length - 1; i >= 0; i--)
				bitmaps.Add(raws[i].ToBitmap());
			try
			{
				new TextureAtlasBuilder.TextureAtlas(bitmaps).outputToPNG(filename);
			}
			finally
			{
				foreach (var item in bitmaps)
					if (item != null)
						try { item.Dispose(); } catch (System.ObjectDisposedException){ }
			}

        }

        public void buildBuffers() {
            builder.BuildData(meshes);
            //Console.WriteLine("#meshes = " + meshes.Count);
            for (int i = 0; i < meshes.Count; i++)
            {
                MeshData m = meshes[i];

                //combined = ContentPipe.LoadTexture(builder.Atlas.Image);

                m.vbo.Gen();
                GL.BindBuffer(BufferTarget.ArrayBuffer, m.vbo);
                GL.BufferData(
                    BufferTarget.ArrayBuffer,
                    (IntPtr)(Vector3.SizeInBytes * m.vertices.Length),
                    m.vertices,
                    BufferUsageHint.StaticDraw
                    );

				m.texBuf.Gen();
                GL.BindBuffer(BufferTarget.ArrayBuffer, m.texBuf);
                GL.BufferData(
                    BufferTarget.ArrayBuffer,
                    (IntPtr)(Vector2.SizeInBytes * m.texCoord.Length),
                    m.texCoord,
                    BufferUsageHint.StaticDraw
                    );

				m.colorBuf.Gen();
                GL.BindBuffer(BufferTarget.ArrayBuffer, m.colorBuf);
                GL.BufferData(
                    BufferTarget.ArrayBuffer,
                    (IntPtr)(Vector4.SizeInBytes * m.colors.Length),
                    m.colors,
                    BufferUsageHint.StaticDraw
                    );
				m.normalBuf.Gen();
				GL.BindBuffer(BufferTarget.ArrayBuffer, m.normalBuf);
				GL.BufferData(
					BufferTarget.ArrayBuffer,
					(IntPtr)(Vector3.SizeInBytes * m.normals.Length),
					m.normals,
					BufferUsageHint.StaticDraw
					);

				m.ibo.Gen();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, m.ibo);
                GL.BufferData(
                    BufferTarget.ElementArrayBuffer,
                    (IntPtr)(sizeof(uint) * m.indices.Length),
                    m.indices,
                    BufferUsageHint.StaticDraw
                    );
            }
            calculateCenter();
        }

        public MeshData[] GetTransformed(Transform transform)
		{
			var o = meshes.ToArray();
			for (int i = o.Length - 1; i >= 0; i--) o[i] = o[i].GetTransformed(transform);
			return o;
		}
		public void drawModel([In]ref RenderCamera camTrs)
		{
			drawModel(Transform.Identity, ref camTrs);
		}
		public void drawModel(Transform transform, [In]ref RenderCamera camTrs)
		{
			drawModel(transform,255, ref camTrs);
		}
		//static bool old;
		public bool drawModel(Transform transform, byte drawLayers, ref RenderCamera camTrs)
		{
			if (/*!old &&*/ null != (object)root) return root.draw(transform, drawLayers, ref camTrs);
			bool started = false;

			for (int i = 0; i < meshes.Count; i++)
			{
				MeshData m = meshes[i];
				if (0== ((1<<(m.material.drawLayerBillboard &7))&drawLayers))
					continue;
				//if (m.vertices == null || m.indices == null) return;
				if (!started)
				{
					started = true;
					GL.PushMatrix();
					transform.GL_Load();

					GL.EnableClientState(ArrayCap.VertexArray);
					GL.EnableClientState(ArrayCap.TextureCoordArray);
					GL.EnableClientState(ArrayCap.ColorArray);
				}
				if (m.texture != null)
				{
					GL.BindTexture(TextureTarget.Texture2D, m.texture.ID);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, m.texture.TextureParamS);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, m.texture.TextureParamT);
				}
				GL.BindBuffer(BufferTarget.ArrayBuffer, m.vbo);
				GL.VertexPointer(3, VertexPointerType.Float, 0, IntPtr.Zero);
				GL.BindBuffer(BufferTarget.ArrayBuffer, m.texBuf);
				GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, IntPtr.Zero);
				GL.BindBuffer(BufferTarget.ArrayBuffer, m.colorBuf);
				GL.ColorPointer(4, ColorPointerType.Float, 0, IntPtr.Zero);
				GL.BindBuffer(BufferTarget.ArrayBuffer, m.normalBuf);
				GL.NormalPointer(NormalPointerType.Float, 0, IntPtr.Zero);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, m.ibo);

				if (Globals.doWireframe)
					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
				else
					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

				GL.DrawElements(PrimitiveType.Triangles, m.indices.Length,
					DrawElementsType.UnsignedInt, IntPtr.Zero);

				if (Globals.doWireframe)
					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			}
			if (started)
			{
				GL.DisableClientState(ArrayCap.VertexArray);
				GL.DisableClientState(ArrayCap.TextureCoordArray);
				GL.DisableClientState(ArrayCap.ColorArray);
				GL.PopMatrix();
			}
			return started;
		}

		public bool drawModel(byte drawLayers, ref RenderCamera camTrs)
		{
			return drawModel(Transform.Identity, drawLayers, ref camTrs);
		}

		public static Export.ReferenceRegister<Model3D> ExportRegister;
		Export.TypeReference Export.Reference.API() { return ExportRegister.Singleton; }
		void Export.Reference<Model3D>.API(Export.Exporter ex)
		{
			ex.Value(GeoDataSegAddress);
			//ex.Array(geoDisplayLists);
			ex.RefArray(meshes);
		}
	}
}
