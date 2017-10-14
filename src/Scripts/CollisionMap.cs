using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using BubblePony.ExportUtility;
using BubblePony.GLHandle;
using Export = BubblePony.Export;

namespace Quad64
{

    internal sealed class CollisionTriangleList : Export.Reference<CollisionTriangleList>, Export.Reference
    {
        private int id = 0;
		public CollisionTriangleList next;
        public ushort[] indices;
		public GraphicsHandle.Buffer ibo;
        public readonly TextureFormats.Raw texture;

        public CollisionTriangleList(int ID)
        {
            id = ID;
			texture = TextureFormats.ColorTexture(
				Color4b.HSV(60+ID*39, (14 - (3+((ID >> 3) & 7))) /14.0,(4-((ID>>6)&3))/4.0)
				);
        }

		public static Export.ReferenceRegister<CollisionTriangleList> ExportRegister;
		Export.TypeReference Export.Reference.API() { return ExportRegister.Singleton; }
		void Export.Reference<CollisionTriangleList>.API(Export.Exporter ex)
		{
			ex.Value(id);
			ex.Array(indices);
		}
	}

    public sealed class CollisionMap : Export.Reference<CollisionMap>, Export.Reference
    {
        private GraphicsHandle.Buffer vbo;
        private List<Vector3> vertices = new List<Vector3>();
        private CollisionTriangleList tri_first, tri_last;
        private Vector3[] verts;
		private int tri_count;
		private struct TriangleInsert
		{
			public ushort A, B, C;
		}
		private List<TriangleInsert> indicesList = new List<TriangleInsert>();

		public Vector3[] GetVertices()
		{
			return vertices.ToArray();
		}
		public ushort[][] GetTriangles()
		{
			ushort[][] each = new ushort[tri_count][];
			int i;
			CollisionTriangleList col;
			for (col = tri_first, i = 0; i < tri_count; col = col.next, ++i)
				each[i] = col.indices;
			return each;
		}

        public void AddVertex(Vector3 newVert)
        {
            vertices.Add(newVert);
        }

        public void AddTriangle(ushort a, ushort b, ushort c)
        {
			if (tri_count == 0) throw new System.InvalidOperationException();
			indicesList.Add(new TriangleInsert { A = a, B = b, C = c, });
        }
		private ushort[] DumpIndices()
		{
			int i, j;
			var o = new ushort[(j = indicesList.Count) * 3];
			TriangleInsert insert;
			for (i = j - 1; i >= 0; --i)
			{
				insert = indicesList[i];
				o[(j = (i * 3))] = insert.A;
				o[1 + j] = insert.B;
				o[2 + j] = insert.C;
			}
			indicesList.Clear();
			return o;
		}
        public void NewTriangleList(int ID)
        {
			if (0 == tri_count++)
			{
				tri_first = new CollisionTriangleList(ID);
				tri_last = tri_first;
				tri_first.next = tri_first;
			}
			else
			{
				tri_last.indices = DumpIndices();
				tri_last.next = new CollisionTriangleList(ID);
				tri_last = tri_last.next;
				tri_last.next = tri_first;
			}
        }


        public static float barryCentric(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 pos)
        {
            float det = (p2.Z - p3.Z) * (p1.X - p3.X) + (p3.X - p2.X) * (p1.Z - p3.Z);
            float l1 = ((p2.Z - p3.Z) * (pos.X - p3.X) + (p3.X - p2.X) * (pos.Z - p3.Z)) / det;
            float l2 = ((p3.Z - p1.Z) * (pos.X - p3.X) + (p1.X - p3.X) * (pos.Z - p3.Z)) / det;
            float l3 = 1.0f - l1 - l2;
            return l1 * p1.Y + l2 * p2.Y + l3 * p3.Y;
        }

        public static bool PointInTriangle(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            var s = p0.Y * p2.X - p0.X * p2.Y + (p2.Y - p0.Y) * p.X + (p0.X - p2.X) * p.Y;
            var t = p0.X * p1.Y - p0.Y * p1.X + (p0.Y - p1.Y) * p.X + (p1.X - p0.X) * p.Y;

            if ((s < 0) != (t < 0))
                return false;

            var A = -p1.Y * p2.X + p0.Y * (p2.X - p1.X) + p0.X * (p1.Y - p2.Y) + p1.X * p2.Y;
            if (A < 0.0)
            {
                s = -s;
                t = -t;
                A = -A;
            }
            return s > 0 && t > 0 && (s + t) <= A;
        }

        private struct tempTriangle
        {
            public Vector3 a, b, c;
        }

        public short dropToGround(Vector3 pos)
		{

			float highestY = -0x2000;
			CollisionTriangleList list;
			int i, j;
			float d;
			bool any=false;
            for (list = tri_first, i = 0; i < tri_count; list=list.next, ++i)
            {
                for (j = 0; j < list.indices.Length; j += 3)
                {
                    tempTriangle temp;
                    temp.a = new Vector3(vertices[(int)list.indices[j + 0]]);
                    temp.b = new Vector3(vertices[(int)list.indices[j + 1]]);
                    temp.c = new Vector3(vertices[(int)list.indices[j + 2]]);
                    if (PointInTriangle(pos.Xz, temp.a.Xz, temp.b.Xz, temp.c.Xz))
                    {
						any = true;
                        d=barryCentric(temp.a, temp.b, temp.c, pos);
						if (d > highestY)
							highestY = d;
                    }
                }
            }

            if (!any)
                return (short)pos.Y;
            return (short)highestY;
        }

        public void buildCollisionMap()
        {
			CollisionTriangleList list;
			int i;
            verts = vertices.ToArray();

			vbo.Gen();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr)(Vector3.SizeInBytes * verts.Length),
                verts,
                BufferUsageHint.StaticDraw
                );

			if (tri_count!=0 && null != indicesList)
			{
				tri_last.indices = DumpIndices();
				indicesList = null;
			}

            for (list=tri_first, i = 0; i < tri_count; list=list.next, ++i)
            {
				list.ibo.Gen();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, list.ibo);
                GL.BufferData(
                    BufferTarget.ElementArrayBuffer,
                    (IntPtr)(list.indices.Length<<1),
                    list.indices,
                    BufferUsageHint.StaticDraw
                );
            }
        }

        public void drawCollisionMap(bool drawAsBlack)
        {
			CollisionTriangleList l;
			int i;
            GL.PushMatrix();
            GL.EnableClientState(ArrayCap.VertexArray);
            if (drawAsBlack) // Used as part of color picking
                GL.BlendFunc(BlendingFactorSrc.Zero, BlendingFactorDest.Zero);

			for (l = tri_first, i = 0; i < tri_count; l = l.next, ++i)
			{
				//if (m.vertices == null || m.indices == null) return;
				GL.BindTexture(TextureTarget.Texture2D, l.texture.GetLoadedHandle());
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.VertexPointer(3, VertexPointerType.Float, 0, IntPtr.Zero);
                if (Globals.doWireframe)
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                else
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, l.ibo);
                GL.DrawElements(PrimitiveType.Triangles, l.indices.Length,
                    DrawElementsType.UnsignedShort, IntPtr.Zero);

                if (Globals.doWireframe)
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
            if (drawAsBlack)
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.PopMatrix();

            if (!drawAsBlack && !Globals.doWireframe)
                drawCollisionMapOutline();
        }

        public void drawCollisionMapOutline()
		{
			CollisionTriangleList l;
			int i;
			GL.PushMatrix();
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BlendFunc(BlendingFactorSrc.Zero, BlendingFactorDest.Zero);
			for (l = tri_first, i = 0; i < tri_count; l = l.next, ++i)
			{
                //if (m.vertices == null || m.indices == null) return;
                GL.BindTexture(TextureTarget.Texture2D, l.texture.GetLoadedHandle());
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.VertexPointer(3, VertexPointerType.Float, 0, IntPtr.Zero);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, l.ibo);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, l.ibo);
                GL.DrawElements(PrimitiveType.Triangles, l.indices.Length,
                    DrawElementsType.UnsignedShort, IntPtr.Zero);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.PopMatrix();
        }

		public static Export.ReferenceRegister<CollisionMap> ExportRegister;
		Export.TypeReference Export.Reference.API() { return ExportRegister.Singleton; }
		void Export.Reference<CollisionMap>.API(Export.Exporter ex)
		{
			//ex.Array(vertices);
			//ex.RefArray(triangles);
		}
	}
}
