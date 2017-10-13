

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
namespace Quad64.src.Viewer
{
    static class BoundingBox
	{

		public static void draw_solid(Transform transform, Color4b color,
			Vector3 extents)
		{
			if (extents.X < 0f) extents.X = -extents.X;
			if (extents.Y < 0f) extents.Y = -extents.Y;
			if (extents.Z < 0f) extents.Y = -extents.Z;

			draw_solid(transform, color, extents, -extents);
		}
		public const float DefaultExtents = 150;

		public static void draw_solid(Transform transform, Color4b color,
			float extents = DefaultExtents)
		{
			if (extents < 0) extents = -extents;
			var nextents = -extents;
			draw(transform, color,
				new Vector3 { X = nextents, Y = nextents, Z = nextents, },
				new Vector3 { X = extents, Y = extents, Z = extents, });
		}
		private static readonly int XMIN = 0, XMAX = 1, YMIN = 0, YMAX = 2, ZMIN = 0, ZMAX = 4;

		[ThreadStatic]
		private static Vector3 VMIN;
		[ThreadStatic]
		private static Vector3 VMAX;

		private static void VERTEX(int CODE)
		{
			GL.Vertex3(
				(XMIN == (CODE & (XMIN | XMAX))) ? VMIN.X : VMAX.X,
				(YMIN == (CODE & (YMIN | YMAX))) ? VMIN.Y : VMAX.Y,
				(ZMIN == (CODE & (ZMIN | ZMAX))) ? VMIN.Z : VMAX.Z);
		}
		public static void draw_solid(Transform transform, Color4b color,
			Vector3 upper, Vector3 lower)
		{
			VMIN = lower;
			VMAX = upper;
			EnsureMinMax(ref VMIN, ref VMAX);

			GL.Disable(EnableCap.Blend);
			GL.Disable(EnableCap.Texture2D);
			GL.Disable(EnableCap.AlphaTest);
			GL.PushMatrix();
			transform.GL_Load();

			GL.Begin(PrimitiveType.Quads);
			color.GL_Load();

			VERTEX(XMAX | YMAX | ZMIN); // Top-right of top face
			VERTEX(XMIN | YMAX | ZMIN); // Top-left of top face
			VERTEX(XMIN | YMAX | ZMAX); // Bottom-left of top face
			VERTEX(XMAX | YMAX | ZMAX); // Bottom-right of top face

			VERTEX(XMAX | YMIN | ZMIN); // Top-right of bottom face
			VERTEX(XMIN | YMIN | ZMIN); // Top-left of bottom face
			VERTEX(XMIN | YMIN | ZMAX); // Bottom-left of bottom face
			VERTEX(XMAX | YMIN | ZMAX); // Bottom-right of bottom face

			VERTEX(XMAX | YMAX | ZMAX); // Top-Right of front face
			VERTEX(XMIN | YMAX | ZMAX); // Top-left of front face
			VERTEX(XMIN | YMIN | ZMAX); // Bottom-left of front face
			VERTEX(XMAX | YMIN | ZMAX); // Bottom-right of front face

			VERTEX(XMAX | YMIN | ZMIN); // Bottom-Left of back face
			VERTEX(XMIN | YMIN | ZMIN); // Bottom-Right of back face
			VERTEX(XMIN | YMAX | ZMIN); // Top-Right of back face
			VERTEX(XMAX | YMAX | ZMIN); // Top-Left of back face

			VERTEX(XMIN | YMAX | ZMAX); // Top-Right of left face
			VERTEX(XMIN | YMAX | ZMIN); // Top-Left of left face
			VERTEX(XMIN | YMIN | ZMIN); // Bottom-Left of left face
			VERTEX(XMIN | YMIN | ZMAX); // Bottom-Right of left face

			VERTEX(XMAX | YMAX | ZMAX); // Top-Right of left face
			VERTEX(XMAX | YMAX | ZMIN); // Top-Left of left face
			VERTEX(XMAX | YMIN | ZMIN); // Bottom-Left of left face
			VERTEX(XMAX | YMIN | ZMAX); // Bottom-Right of left face

			Color4b.White.GL_Load();
			GL.End();
			GL.PopMatrix();
			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.Texture2D);
			GL.Enable(EnableCap.AlphaTest);
		}

		public static void draw(Transform transform, Color4b color,
			Vector3 upper, Vector3 lower)
		{
			VMIN = lower;
			VMAX = upper;
			//EnsureMinMax(ref VMIN, ref VMAX);

			GL.Disable(EnableCap.Blend);
			GL.Disable(EnableCap.Texture2D);
			GL.Disable(EnableCap.AlphaTest);
			GL.PushMatrix();
			transform.GL_Load();
			/*
			GL.Begin(PrimitiveType.LineLoop);
			GL.Color4(color);

			VERTEX(XMAX|YMAX|ZMIN); // 1
			VERTEX(XMIN|YMAX|ZMIN); // 2
			VERTEX(XMIN|YMAX|ZMAX); // 3
			VERTEX(XMAX|YMAX|ZMIN); // 1
			VERTEX(XMAX|YMAX|ZMAX); // 4
			VERTEX(XMIN|YMAX|ZMAX); // 3

			VERTEX(XMIN|YMIN|ZMAX); // 7
			VERTEX(XMIN|YMIN|ZMIN); // 6
			VERTEX(XMAX|YMIN|ZMIN); // 5
			VERTEX(XMIN|YMIN|ZMAX); // 7
			VERTEX(XMAX|YMIN|ZMAX); // 8
			VERTEX(XMAX|YMIN|ZMIN); // 5

			VERTEX(XMIN|YMAX|ZMIN); // 2
			VERTEX(XMIN|YMIN|ZMIN); // 6
			VERTEX(XMIN|YMAX|ZMAX); // 3
			VERTEX(XMAX|YMIN|ZMAX); // 8
			VERTEX(XMAX|YMAX|ZMAX); // 4
			VERTEX(XMAX|YMIN|ZMIN); // 5
			*/
			GL.Begin(PrimitiveType.Lines);
			color.GL_Load();

			VERTEX(XMAX | YMAX | ZMAX);
			VERTEX(XMAX | YMAX | ZMIN);
			VERTEX(XMAX | YMIN | ZMIN);
			VERTEX(XMAX | YMIN | ZMAX);
			VERTEX(XMIN | YMAX | ZMAX);
			VERTEX(XMIN | YMAX | ZMIN);
			VERTEX(XMIN | YMIN | ZMIN);
			VERTEX(XMIN | YMIN | ZMAX);

			VERTEX(YMAX | ZMAX | XMAX);
			VERTEX(YMAX | ZMAX | XMIN);
			VERTEX(YMAX | ZMIN | XMIN);
			VERTEX(YMAX | ZMIN | XMAX);
			VERTEX(YMIN | ZMAX | XMAX);
			VERTEX(YMIN | ZMAX | XMIN);
			VERTEX(YMIN | ZMIN | XMIN);
			VERTEX(YMIN | ZMIN | XMAX);

			VERTEX(ZMAX | XMAX | YMAX);
			VERTEX(ZMAX | XMAX | YMIN);
			VERTEX(ZMAX | XMIN | YMIN);
			VERTEX(ZMAX | XMIN | YMAX);
			VERTEX(ZMIN | XMAX | YMAX);
			VERTEX(ZMIN | XMAX | YMIN);
			VERTEX(ZMIN | XMIN | YMIN);
			VERTEX(ZMIN | XMIN | YMAX);

			Color4b.White.GL_Load();
			GL.End();
			GL.PopMatrix();
			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.Texture2D);
			GL.Enable(EnableCap.AlphaTest);
		}
		public static void draw(Transform transform, Color4b color,
			Vector3 extents)
		{
			if (extents.X < 0f) extents.X = -extents.X;
			if (extents.Y < 0f) extents.Y = -extents.Y;
			if (extents.Z < 0f) extents.Y = -extents.Z;

			draw(transform, color, extents, -extents);
		}

		public static void draw(Transform transform, Color4b color,
			float extents=DefaultExtents)
		{
			if (extents < 0) extents = -extents;
			var nextents = -extents;
			draw(transform, color, 
				new Vector3 { X = nextents, Y = nextents, Z = nextents, },
				new Vector3 { X = extents, Y = extents, Z = extents, });
		}

		public const float DefaultModelBorder = 25;

		[ThreadStatic] static float mmswap;
		public static void EnsureMinMax(ref float min, ref float max)
		{
			if (!(min <= max))
			{
				if (min > max)
				{
					mmswap = min;
					min = max;
					max = mmswap;
				}
				else
				{
					if (!float.IsNaN(min)) max = min;
					else if (!float.IsNaN(max)) min = max;
					else
					{
						min = 0;
						max = 0;
					}
				}
			}
		}
		public static void EnsureMinMax(ref Vector3 min, ref Vector3 max)
		{
			EnsureMinMax(ref min.X, ref max.X);
			EnsureMinMax(ref min.Y, ref max.Y);
			EnsureMinMax(ref min.Z, ref max.Z);
		}
		public static void ApplyMinMaxBorder(ref Vector3 min, ref Vector3 max, float border)
		{
			EnsureMinMax(ref min, ref max);
			if (!!(border != 0f))
			{
				if (border < 0f)
				{
					border = -border;
					if (border >= 2 * (max.X - min.X))
						min.X = max.X = (max.X + (min.X - max.X) / 2);
					else { min.X += border; max.X -= border; }
					if (border >= 2 * (max.Y - min.Y))
						min.Y = max.Y = (max.Y + (min.Y - max.Y) / 2);
					else { min.Y += border; max.Y -= border; }
					if (border >= 2 * (max.Z - min.Z))
						min.Z = max.Z = (max.Z + (min.Z - max.Z) / 2);
					else { min.Z += border; max.Z -= border; }
				}
				else
				{
					min.X -= border;
					min.Y -= border;
					min.Z -= border;
					max.X += border;
					max.Y += border;
					max.Z += border;
				}
			}
		}
		public static void draw(
			Transform transform,
			Color4b color, 
			Model3D model, 
			float border = DefaultModelBorder)
		{
			Vector3 upper = model.UpperBoundary;
			Vector3 lower = model.LowerBoundary;
			ApplyMinMaxBorder(ref lower, ref upper, border);
			draw(transform, color, upper, lower);
		}
		public static void draw_solid(
			Transform transform,
			Color4b color,
			Model3D model,
			float border = DefaultModelBorder)
		{
			Vector3 upper = model.UpperBoundary;
			Vector3 lower = model.LowerBoundary;
			ApplyMinMaxBorder(ref lower, ref upper, border);
			draw_solid(transform, color, upper, lower);
		}
	}
}
