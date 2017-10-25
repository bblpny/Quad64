using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
namespace Quad64
{
	public sealed class GeoMesh
	{
		public readonly GeoModel Parent;
		public readonly byte[] VertexBuffer;
		public readonly Array IndexBuffer;
		public readonly Scripts.TempMeshData Options;
		public readonly TextureFormats.Raw texture;
		public readonly int IndexCount, IndicesByteSize;
		public readonly byte IndexElementSize, DrawLayerMask;
		public readonly Scripts.Material Material;
		public GraphicsState State = GraphicsState.Default;
		public GeoMesh Next;
		public int VertexCount => VertexBuffer.Length >> 4;
		private static void StoreVertex(byte[] Buffer, ref Vertex128 value, int Offset)
		{
			Block128 b = (Block128)value;
			Buffer[Offset + 0] = b.A;
			Buffer[Offset + 1] = b.B;
			Buffer[Offset + 2] = b.C;
			Buffer[Offset + 3] = b.D;
			Buffer[Offset + 4] = b.E;
			Buffer[Offset + 5] = b.F;
			Buffer[Offset + 6] = b.G;
			Buffer[Offset + 7] = b.H;
			Buffer[Offset + 8] = b.I;
			Buffer[Offset + 9] = b.J;
			Buffer[Offset + 10] = b.K;
			Buffer[Offset + 11] = b.L;
			Buffer[Offset + 12] = b.M;
			Buffer[Offset + 13] = b.N;
			Buffer[Offset + 14] = b.O;
			Buffer[Offset + 15] = b.P;
		}
		public GeoMesh(GeoModel Parent, Scripts.TempMesh128 Mesh, ref Dictionary<Scripts.TempVertex128, Scripts.TempVertex128> utilize)
		{
			Scripts.TempVertex128 viter;
			uint viterpos;
			this.Parent = Parent;
			this.Options = Mesh.value;
			this.Material = this.Options.getMaterial();
			this.texture = Mesh.references.bmp;
			utilize = Scripts.TempVertex128.Process(ref Mesh.references.list, utilize);
			var VertexCount = utilize.Count;
			this.VertexBuffer = new byte[VertexCount << 4];
			Array IndexBuffer;
			var IndexSize = VertexCount > 256 ? VertexCount > (ushort.MaxValue + 1u) ? 4 : 2 : 1;
			this.IndexCount = (int)Mesh.references.list.count;
			byte IndexElementSize;
			uint NextIndex = 0;
			if (IndexSize > 1)
			{
				if (IndexSize > 2)
				{
					var UIndexBuffer = new uint[IndexCount];
					for (viterpos = Mesh.references.list.count, viter = Mesh.references.list.first; viterpos != 0; viter = viter.next, --viterpos) unchecked
						{
							UIndexBuffer[(int)(Mesh.references.list.count - viterpos)] = viter.index;
							if (NextIndex == viter.index)
								StoreVertex(VertexBuffer, ref viter.value, (int)(NextIndex++) << 4);
						}
					IndexBuffer = UIndexBuffer;
					IndexElementSize = 4;
				}
				else
				{
					var UIndexBuffer = new ushort[IndexCount];
					for (viterpos = Mesh.references.list.count, viter = Mesh.references.list.first; viterpos != 0; viter = viter.next, --viterpos) unchecked
						{
							UIndexBuffer[(int)(Mesh.references.list.count - viterpos)] = (ushort)viter.index;
							if (NextIndex == viter.index)
								StoreVertex(VertexBuffer, ref viter.value, (int)(NextIndex++) << 4);
						}
					IndexBuffer = UIndexBuffer;
					IndexElementSize = 2;
				}
			}
			else
			{
				var UIndexBuffer = new byte[IndexCount];
				for (viterpos = Mesh.references.list.count, viter = Mesh.references.list.first; viterpos != 0; viter = viter.next, --viterpos) unchecked
					{
						UIndexBuffer[(int)(Mesh.references.list.count - viterpos)] = (byte)viter.index;
						if (NextIndex == viter.index)
							StoreVertex(VertexBuffer, ref viter.value, (int)(NextIndex++) << 4);
					}
				IndexBuffer = UIndexBuffer;
				IndexElementSize = 1;
			}
			this.IndexBuffer = IndexBuffer;
			this.IndexElementSize = IndexElementSize;
			this.IndicesByteSize = 2 == IndexElementSize ? IndexCount << 1 : 4 == IndexElementSize ? IndexCount << 2 : IndexCount;
			DrawLayerMask = (byte)(1 << (Material.drawLayerBillboard & 7));
		}
		static void BindLight(Light Light, ref GraphicsLight Graphics)
		{
			Graphics.Color = (Color4b)Light.Color;
			Graphics.Normal = Light.Normal;
			Graphics.Normal.W = 0;
			Graphics.Color.A = 255;
		}

		internal byte BindGL(GeoMesh prev, ref GeoMesh @this)
		{
			if (this.VertexCount != 0)
			{
				int vbo = this.State.Vertex.Gen();

				GL.BindBuffer(BufferTarget.ElementArrayBuffer, vbo);

				GL.BufferData(BufferTarget.ElementArrayBuffer,
					this.VertexBuffer.Length,
					VertexBuffer,
					BufferUsageHint.StaticDraw);

				int ibo = this.State.Index.Gen();

				GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);

				if (IndexElementSize == 4)
					GL.BufferData(
					BufferTarget.ElementArrayBuffer,
					this.IndexCount << 2,
					(uint[])this.IndexBuffer,
					BufferUsageHint.StaticDraw
					);
				else if (IndexElementSize == 2)
					GL.BufferData(
						BufferTarget.ElementArrayBuffer,
						this.IndexCount << 1,
						(ushort[])this.IndexBuffer,
						BufferUsageHint.StaticDraw);
				else
					GL.BufferData(
						BufferTarget.ElementArrayBuffer,
						this.IndexCount,
						(byte[])this.IndexBuffer,
						BufferUsageHint.StaticDraw);

				State.IndexCount = (ushort)IndexCount;
				State.IndexPrimitive = IndexPrimitive.Triangles;
				State.IndexInteger =
						IndexElementSize == 4 ? IndexInteger.Int :
						IndexElementSize == 2 ? IndexInteger.Short :
						IndexInteger.Byte;

				State.LightMode = Material.Smooth ? LightMode.Smooth0 : LightMode.Hard0;
				switch (Material.numLight)
				{
					case 0:
						State.Ambient = (Color4b)Material.light.Lights0.Ambient.Color;
						break;
					case 1:
						BindLight(Material.light.Lights1.Light1, ref State.Light1);
						State.Ambient = (Color4b)Material.light.Lights1.Ambient.Color;
						State.LightMode |= LightMode.Hard1;
						break;
					case 2:
						BindLight(Material.light.Lights2.Light1, ref State.Light1);
						BindLight(Material.light.Lights2.Light2, ref State.Light2);
						State.Ambient = (Color4b)Material.light.Lights2.Ambient.Color;
						State.LightMode |= LightMode.Hard2;
						break;
					case 3:
						BindLight(Material.light.Lights3.Light1, ref State.Light1);
						BindLight(Material.light.Lights3.Light2, ref State.Light2);
						BindLight(Material.light.Lights3.Light3, ref State.Light3);
						State.Ambient = (Color4b)Material.light.Lights3.Ambient.Color;
						State.LightMode |= LightMode.Hard3;
						break;
					case 4:
						BindLight(Material.light.Lights4.Light1, ref State.Light1);
						BindLight(Material.light.Lights4.Light2, ref State.Light2);
						BindLight(Material.light.Lights4.Light3, ref State.Light3);
						BindLight(Material.light.Lights4.Light4, ref State.Light4);
						State.Ambient = (Color4b)Material.light.Lights4.Ambient.Color;
						State.LightMode |= LightMode.Hard4;
						break;
					case 5:
						BindLight(Material.light.Lights5.Light1, ref State.Light1);
						BindLight(Material.light.Lights5.Light2, ref State.Light2);
						BindLight(Material.light.Lights5.Light3, ref State.Light3);
						BindLight(Material.light.Lights5.Light4, ref State.Light4);
						BindLight(Material.light.Lights5.Light5, ref State.Light5);
						State.Ambient = (Color4b)Material.light.Lights5.Ambient.Color;
						State.LightMode |= LightMode.Hard5;
						break;
					case 6:
						BindLight(Material.light.Lights6.Light1, ref State.Light1);
						BindLight(Material.light.Lights6.Light2, ref State.Light2);
						BindLight(Material.light.Lights6.Light3, ref State.Light3);
						BindLight(Material.light.Lights6.Light4, ref State.Light4);
						BindLight(Material.light.Lights6.Light5, ref State.Light5);
						BindLight(Material.light.Lights6.Light6, ref State.Light6);
						State.Ambient = (Color4b)Material.light.Lights6.Ambient.Color;
						State.LightMode |= LightMode.Hard6;
						break;
					default:
						BindLight(Material.light.Lights7.Light1, ref State.Light1);
						BindLight(Material.light.Lights7.Light2, ref State.Light2);
						BindLight(Material.light.Lights7.Light3, ref State.Light3);
						BindLight(Material.light.Lights7.Light4, ref State.Light4);
						BindLight(Material.light.Lights7.Light5, ref State.Light5);
						BindLight(Material.light.Lights7.Light6, ref State.Light6);
						BindLight(Material.light.Lights7.Light7, ref State.Light7);
						State.Ambient = (Color4b)Material.light.Lights7.Ambient.Color;
						State.LightMode |= LightMode.Hard7;
						break;
				};
				State.Ambient.A = 255;
				State.ElementMask = (Material.HasVertexColors ?
					ElementMask.Color : ElementMask.Normal) |
					(Material.HasTexture ?
					 (null != texture && !texture.IsColorTexture) ?
						(ElementMask.Texture | ElementMask.Texcoord) :
						ElementMask.Texcoord : (ElementMask)0) |
					(ElementMask.Index | ElementMask.Position);

				if (0 != (State.ElementMask & ElementMask.Texture))
					State.Texture = texture.GetLoadedHandle();

				State.TexturePresentation =
					(0 == ((Material.wrapModes >> 8) & 1) ?
						0 == ((Material.wrapModes >> 8) & 2) ?
							(TexturePresentation)0 :
							TexturePresentation.ClampS :
						0 == ((Material.wrapModes >> 8) & 2) ?
							TexturePresentation.MirrorS :
							(TexturePresentation)0) |
							(0 == ((Material.wrapModes) & 1) ?
						0 == ((Material.wrapModes) & 2) ?
							(TexturePresentation)0 :
							TexturePresentation.ClampT :
						0 == ((Material.wrapModes) & 2) ?
							TexturePresentation.MirrorT :
							(TexturePresentation)0);

				State.TextureScaleX = Material.texScaleX;
				State.TextureScaleY = Material.texScaleY;
				State.TextureWidth = Material.w;
				State.TextureHeight = Material.h;

				if (null != texture && 0 != (State.ElementMask & ElementMask.Texcoord))
				{
					State.Texture = texture.GetLoadedHandle();
					State.ElementMask |= ElementMask.Texture;
				}
				State.Fog = (Color4b)Material.fogColor;
				State.FogMultiplier = Material.fogMultiplier;
				State.FogOffset = Material.fogOffset;
				State.FeatureMask = (FeatureMask)(Material.drawLayerBillboard & 7) |
					(this.Parent.Parent.ZTest ? FeatureMask.ZTest : (FeatureMask)0) |
					(Material.IsLit ? FeatureMask.Lit : (FeatureMask)0) |
					(Material.Fog ? FeatureMask.Fog : (FeatureMask)0);
				State.Color.Value = uint.MaxValue;
				//State.Color.Diffuse
				//State.Color = Material.lightColor;
				//State.Diffuse = Material.lightColor;
				//State.Ambient = Material.darkColor;
				State.VertexColor = VertexColor.Smooth;
				State.Culling = Material.CullBack ? Material.CullFront ? Culling.Both : Culling.Back : Material.CullFront ? Culling.Front : Culling.Off;

				GraphicsState.Sanitize(ref State);
			}
			return DrawLayerMask;
		}
	}
}
