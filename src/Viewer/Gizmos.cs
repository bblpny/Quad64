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
		const int arrow_sides = 8;
		const int arrow_shift = 9;
		public static void Arrow(
			float stem_radius,
			float stem_length,
			float head_radius,
			float head_length,
			Vector3 forward,
			Vector3 side0,
			Vector3 side1)
		{
			Vector3 next_a, next_b, nrm0, nrm1, nrm2, nrm3,nm;
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
					for (int i = 0; i != arrow_sides; i++)
					{
						GL.Vertex3(arrow_start + (nrm2 * IntCos(i << arrow_shift)) + (nrm3 * IntSin(i << arrow_shift)));
						GL.Normal3(-forward);
					}
					GL.End();
				}
			}
			else
			{
				sc.X = IntCos((arrow_sides-1) << arrow_shift);
				sc.Y = IntSin((arrow_sides-1) << arrow_shift);
				nm = (side0 * sc.X) + (side1 * sc.Y);
				next_a = (nrm0 * sc.X) + (nrm1 * sc.Y);
				next_b = next_a + arrow_start;
				GL.Begin(PrimitiveType.TriangleStrip);
				GL.Vertex3(next_b);
				GL.Normal3(nm);
				GL.Vertex3(next_a);
				GL.Normal3(nm);
				for (int i = 0; i != arrow_sides; ++i)
				{
					sc.X = IntCos(i << arrow_shift);
					sc.Y = IntSin(i << arrow_shift);
					nm = (side0 * sc.X) + (side1 * sc.Y);
					next_a = (nrm0 * sc.X) + (nrm1 * sc.Y);
					next_b = next_a + arrow_start;
					GL.Vertex3(next_b);
					GL.Normal3(nm);
					GL.Vertex3(next_a);
					GL.Normal3(nm);
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
							for (int i = 0; i <= arrow_sides; ++i)
							{
								sc.X = IntCos(i << arrow_shift);
								sc.Y = IntSin(i << arrow_shift);
								GL.Vertex3(arrow_start + (nrm0 * sc.X) + (nrm1 * sc.Y));
								GL.Normal3(forward);
								GL.Vertex3(arrow_start + (nrm2 * sc.X) + (nrm3 * sc.Y));
								GL.Normal3(forward);
							}
						}
						else
						{
							for (int i = 0; i <= arrow_sides; ++i)
							{
								sc.X = IntCos(i << arrow_shift);
								sc.Y = IntSin(i << arrow_shift);
								GL.Vertex3(arrow_start + (nrm2 * sc.X) + (nrm3 * sc.Y));
								GL.Normal3(-forward);
								GL.Vertex3(arrow_start + (nrm0 * sc.X) + (nrm1 * sc.Y));
								GL.Normal3(-forward);
							}

						}
						GL.End();
					}
					else
					{
						GL.Begin(PrimitiveType.TriangleFan);
						for (int i = 0; i!= arrow_sides;++i)
						{
							GL.Vertex3(arrow_start + (nrm0 * IntCos(i << arrow_shift)) + (nrm1 * IntSin(i << arrow_shift)));

							GL.Normal3(forward);
						}
						GL.End();
					}
				}

				// bottom cap.
				GL.Begin(PrimitiveType.TriangleFan);
				for (int i = arrow_sides; i != 0; --i)
				{
					GL.Vertex3((nrm0 * IntCos(i << arrow_shift)) + (nrm1 * IntSin(i << arrow_shift)));
					GL.Normal3(-forward);
				}
				GL.End();
			}

			if (head_radius != 0f)
			{
				GL.Begin(PrimitiveType.TriangleFan);
				GL.Vertex3(head_point);
				GL.Normal3(forward);
				for (int i = 0; i <= arrow_sides; i++)
				{
					GL.Vertex3(arrow_start + (nrm2 * IntCos(i << arrow_shift)) + (nrm3 * IntSin(i << arrow_shift)));
					GL.Normal3((side0 * IntCos(i << arrow_shift)) + (side1* IntSin(i << arrow_shift)));
				}
				GL.End();
			}
		}
	}
}
