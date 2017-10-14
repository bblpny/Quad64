using OpenTK;
using System.Collections.Generic;

namespace Quad64.Scripts
{

    public class ModelBuilder : System.IDisposable
    {
		public struct TextureInfo
        {
            public int wrapS, wrapT;
        }
		public GeoModel geoModel;
        public bool processingTexture = false;

		public struct NodeBinder : System.IDisposable
		{
			private ModelBuilder builder;
			private GeoModel Restore;
			private GeoModel Replace;
			private volatile bool NeedDispose;
			public static NodeBinder Bind(ModelBuilder builder, GeoModel node)
			{
				if (null != (object)builder)
				{
					var nb = new NodeBinder
					{
						builder = builder,
						Restore = System.Threading.Interlocked.Exchange(ref builder.geoModel, node),
						Replace = node,
					};
					nb.NeedDispose = nb.Restore != nb.Replace;
					if (nb.NeedDispose)
						return nb;
				}
				return default(NodeBinder);
			}

			public void Dispose()
			{
				if (NeedDispose &&
					Replace == System.Threading.Interlocked.CompareExchange(ref builder.geoModel, Restore, Replace))
					NeedDispose = false;
			}
		}
		private TempMesh currentMaterial;

        public TextureFormats.Raw[] TextureImages {
			get {
				TextureFormats.Raw[] all = new TextureFormats.Raw[(int)TempMeshes.count];
				TempMesh iter;
				uint iter_pos;

				for (iter = TempMeshes.first, iter_pos = 0; iter_pos != TempMeshes.count; iter = iter.next, ++iter_pos)
					all[(int)iter_pos] = iter.references.bmp;

				return all;
			}
		}
        internal TempMesh.List TempMeshes;
		
		private Vector3 layout_offset = new Vector3(0, 0, 0);
		const byte ClampBit =2, MirrorBit = 1;
		const OpenTK.Graphics.OpenGL.TextureWrapMode
			ClampMirror = OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat,
			RepeatMirror = OpenTK.Graphics.OpenGL.TextureWrapMode.MirroredRepeat,
			Repeat = OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat,
			Clamp = OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge;

		static OpenTK.Graphics.OpenGL.TextureWrapMode WrapMode(byte b)
		{
			return (b & MirrorBit) == 0 ?
				(b & ClampBit) == 0 ? Repeat : Clamp
				: (b & ClampBit) == 0 ? RepeatMirror : ClampMirror;
		}

		public TextureInfo newTexInfo(ushort wrapModes)
		{
			TextureInfo info = new TextureInfo();

			info.wrapS = (int)WrapMode((byte)(wrapModes >> 8));
			info.wrapT = (int)WrapMode((byte)(wrapModes & 255));
			return info;
		}

		public void AddTexture(TextureFormats.Raw bmp, TextureInfo info, uint segmentAddress, TempMaterial material)
		{
			TempMeshData data;
			data.info = info;
			data.segmentAddress = segmentAddress;
			if (null != (object)material)
			{
				data.wr_material = material.weakref;
			}
			else
			{
				data.wr_material = null;
			}
			TempMesh.Add(ref TempMeshes, ref data, new TempMeshReferences { bmp = bmp, });
			currentMaterial = TempMeshes.last;
        }
		public TempMesh MaterialFallback
		{
			get
			{
				if(null==(object)currentMaterial)
					AddTexture(
						TextureFormats.ColorTexture(System.Drawing.Color.White),
						newTexInfo(0),
						0x00000000,null
					);
				return currentMaterial;
			}
		}
		public void AddTempVertex(
			ref Vertex128 vertex128,
			ref Vector3 pos,
			ref Vector2 uv,
			ref Vector4 color,
			ref Vector3 normal)
        {
			TempVertexData vertex;
			Transform transform;

			if (null == geoModel)
			{
				transform = Transform.Identity;
			}
			else
			{
				geoModel.Parent.BuildTransform(out transform);
				geoModel.AddVertex(MaterialFallback,ref vertex128);
			}
			vertex.position = transform.TransformPosition(pos);
			vertex.texCoord = uv;
			vertex.color = color;
			vertex.normal = transform.TransformVectorNoScale(normal);

			TempVertex.Add(ref MaterialFallback.references.list, ref vertex);
        }
        public void BuildData(List<Model3D.MeshData> meshes) {
			if (meshes.Count != 0)
				System.Console.WriteLine("duplicate");

			{
				int target_c = (int)TempMeshes.count + meshes.Count;
				if (meshes.Capacity < target_c) meshes.Capacity = target_c;
			}

			Dictionary<TempVertex,TempVertex> vert_temp = null;
			Model3D.MeshData md;
			TempMesh mesh_iter;
			TempVertex vert_iter;
			uint mesh_iter_n,vert_iter_n,next_ind,u_pos;int v_count;
			float fw, fh;
			for (mesh_iter = TempMeshes.first,
				mesh_iter_n = TempMeshes.count;
				0 != mesh_iter_n; 
				mesh_iter = mesh_iter.next,
				--mesh_iter_n)
			{
				v_count = null == (object)(
					vert_temp = TempVertex.Process(ref mesh_iter.references.list, utilize: vert_temp)
					) ? 0 : vert_temp.Count;
				md = new Model3D.MeshData()
				{
					vertices = new Vector3[v_count],
					normals = new Vector3[v_count],
					colors = new Vector4[v_count],
					texCoord = new Vector2[v_count],
					indices = new uint[mesh_iter.references.list.count],
					texture = ContentPipe.LoadTexture(
						mesh_iter.references.bmp,
						mesh_iter.value.info.wrapS,
						mesh_iter.value.info.wrapT),
					material = mesh_iter.value.getMaterial(),
				};
				fw = ((uint)mesh_iter.references.bmp.Width << 5);
				fh = ((uint)mesh_iter.references.bmp.Height << 5);
				for (
					u_pos = 0, next_ind = 0,
					vert_iter = mesh_iter.references.list.first,
					vert_iter_n = mesh_iter.references.list.count;
					0 != vert_iter_n;
					vert_iter = vert_iter.next,
					u_pos++,
					--vert_iter_n)
				{
					md.indices[u_pos] = vert_iter.index;
					if (vert_iter.index != next_ind) continue;
					md.vertices[next_ind] = vert_iter.value.position;
					md.texCoord[next_ind].X = vert_iter.value.texCoord.X / fw;
					md.texCoord[next_ind].Y = vert_iter.value.texCoord.Y / fh;
					md.normals[next_ind] = vert_iter.value.normal;
					md.colors[next_ind] = vert_iter.value.color;
					next_ind++;
				}
				meshes.Add(md);
			}
        }

		/*
        public Vector3[] getVertices(int i) {
            return TempMeshes[i].final.vertices.ToArray();
        }

        public Vector2[] getTexCoords(int i)
        {
            return TempMeshes[i].final.texCoords.ToArray();
		}

		public Vector4[] getColors(int i)
		{
			return TempMeshes[i].final.colors.ToArray();
		}

		public Vector3[] getNormals(int i)
		{
			return TempMeshes[i].final.normals.ToArray();
		}

		public uint[] getIndices(int i)
        {
            return TempMeshes[i].final.indices.ToArray();
        }
        */
		public bool hasTexture(uint segmentAddress, TempMaterial register)
		{
			TempMesh mesh_iter;
			uint mesh_iter_pos;
			if (null == (object)register)
				for (mesh_iter = TempMeshes.first,
					mesh_iter_pos = TempMeshes.count; 0 != mesh_iter_pos;
					mesh_iter = mesh_iter.next, --mesh_iter_pos) {
					if (mesh_iter.value.segmentAddress == segmentAddress && mesh_iter.value.wr_material == null)
						break;
				}
			else
				for (mesh_iter = TempMeshes.first,
					mesh_iter_pos = TempMeshes.count; 0 != mesh_iter_pos;
					mesh_iter = mesh_iter.next, --mesh_iter_pos)
					if (mesh_iter.value.segmentAddress == segmentAddress && mesh_iter.value.wr_material == register.weakref)
						break;

			if (mesh_iter_pos != 0)
			{
				currentMaterial = mesh_iter;
				return true;
			}else
				return false;
        }
		public void Dispose()
		{
			TempMesh.Free(ref TempMeshes);
		}

	}
}
