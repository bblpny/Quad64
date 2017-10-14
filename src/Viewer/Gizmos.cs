using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Quad64
{
	public static class Gizmos
	{
		static readonly float[] sin400h = new float[1024];
		static Gizmos()
		{
			for (int i = 1023; i >= 0; --i)
				sin400h[i] = (float)System.Math.Sin(((i * Math.PI) / 2048));
		}
		/// <summary>
		/// pi == 2048.
		/// </summary>
		public static float IntSin(int value)
		{
			if (value >= 4096 || value <= -4096)
				value %= 4096;

			if (value < 0)
				value += 4096;

			return (0 == (value & 2048) ? -1f : 1f) *
				((0 == (value & 1024)) ? sin400h[value & 1023] : 1024 == (2047&value) ? 1f : sin400h[2048 - (2047&value)]);
		}

		/// <summary>
		/// pi == 2048.
		/// </summary>
		public static float IntCos(int value)
		{
			return IntSin(1024 - value);
		}

		public static void Arrow(
			float stem_radius,
			float stem_length,
			float head_radius,
			float head_length,
			Vector3 forward,
			Vector3 side0,
			Vector3 side1)
		{
			Vector3 next_a, next_b, nrm0, nrm1, nrm2, nrm3;
			Vector2 sc;
			Vector3.Multiply(ref forward, stem_length, out Vector3 arrow_start);
			Vector3.Multiply(ref forward, head_length + stem_length, out Vector3 head_point);

			Vector3.Multiply(ref side0, stem_radius, out nrm0);
			Vector3.Multiply(ref side1, stem_radius, out nrm1);
			Vector3.Multiply(ref side0, head_radius, out nrm2);
			Vector3.Multiply(ref side1, head_radius, out nrm3);

			if (!(stem_radius != 0f) || !(stem_length != 0f))
			{
				if (stem_length != 0f)
				{
					GL.Begin(PrimitiveType.Lines);
					GL.Vertex3(0, 0, 0);
					GL.Vertex3(arrow_start);
					GL.End();
				}

				if (head_radius != 0)
				{
					GL.Begin(PrimitiveType.TriangleFan);
					for (int i = 0; i != 16; i++)
						GL.Vertex3(arrow_start + (nrm2 * IntCos(i << 8)) + (nrm3 * IntSin(i << 8)));
					GL.End();
				}
			}
			else
			{
				sc.X = IntCos(15 << 8);
				sc.Y = IntSin(15 << 8);
				next_a = (nrm0 * sc.X) + (nrm1 * sc.Y);
				next_b = next_a + arrow_start;
				GL.Begin(PrimitiveType.TriangleStrip);
				GL.Vertex3(next_b);
				GL.Vertex3(next_a);
				for (int i = 0; i != 16; ++i)
				{
					sc.X = IntCos(i << 8);
					sc.Y = IntSin(i << 8);
					next_a = (nrm0 * sc.X) + (nrm1 * sc.Y);
					next_b = next_a + arrow_start;
					GL.Vertex3(next_b);
					GL.Vertex3(next_a);
				}
				GL.End();
				next_b = (nrm2 * sc.X) + (nrm3 * sc.Y);
				if (head_radius != stem_radius)
				{
					if (0 != head_radius)
					{
						GL.Begin(PrimitiveType.TriangleStrip);
						//GL.Vertex3(next_a);
						//GL.Vertex3(next_b);
						if (stem_radius > head_radius)
						{
							for (int i = 0; i <= 16; ++i)
							{
								sc.X = IntCos(i << 8);
								sc.Y = IntSin(i << 8);
								GL.Vertex3(arrow_start + (nrm0 * sc.X) + (nrm1 * sc.Y));
								GL.Vertex3(arrow_start + (nrm2 * sc.X) + (nrm3 * sc.Y));
							}
						}
						else
						{
							for (int i = 0; i <= 16; ++i)
							{
								sc.X = IntCos(i << 8);
								sc.Y = IntSin(i << 8);
								GL.Vertex3(arrow_start + (nrm2 * sc.X) + (nrm3 * sc.Y));
								GL.Vertex3(arrow_start + (nrm0 * sc.X) + (nrm1 * sc.Y));
							}

						}
						GL.End();
					}
					else
					{
						GL.Begin(PrimitiveType.TriangleFan);
						for (int i = 0; i!= 16;++i)
						{
							GL.Vertex3(arrow_start + (nrm0 * IntCos(i << 8)) + (nrm1 * IntSin(i << 8)));

						}
						GL.End();
					}
				}

				// bottom cap.
				GL.Begin(PrimitiveType.TriangleFan);
				for (int i = 16; i != 0; --i)
					GL.Vertex3((nrm0 * IntCos(i << 8)) + (nrm1 * IntSin(i << 8)));
				GL.End();
			}

			if (head_radius != 0f)
			{
				GL.Begin(PrimitiveType.TriangleFan);
				GL.Vertex3(head_point);
				for (int i = 0; i <= 16; i++)
					GL.Vertex3(arrow_start + (nrm2 * IntCos(i << 8)) + (nrm3 * IntSin(i << 8)));
				GL.End();
			}
		}
	}
}
