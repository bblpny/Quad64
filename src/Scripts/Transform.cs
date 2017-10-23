using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;
using BubblePony.Integers;

namespace Quad64
{
	using U = TransformUtility;

	partial struct Mtx
	{
		public static Mtx Identity => new Mtx { XX_I = 1, YY_I = 1, ZZ_I = 1, WW_I = 1, };

		public void LoadIdentity() { LoadIdentity(ref this); }

		public static void LoadIdentity(ref Mtx M)
		{
			M.XX_I = 1; M.XY_I = 0; M.XZ_I = 0; M.XW_I = 0;
			M.YX_I = 0; M.YY_I = 1; M.YZ_I = 0; M.YW_I = 0;
			M.ZX_I = 0; M.ZY_I = 0; M.ZZ_I = 1; M.ZW_I = 0;
			M.WX_I = 0; M.WY_I = 0; M.WZ_I = 0; M.WW_I = 1;

			M.XV_F = 0;
			M.YV_F = 0;
			M.ZV_F = 0;
			M.WV_F = 0;
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct Ambient : IEquatable<Ambient>
	{
		// alpha is ignored when testing for equality and generating hash code.
		[FieldOffset(0)]
		public Color4a Color;

		// alpha is ignored when testing for equality and generating hash code.
		[FieldOffset(4)]
		public Color4a ColorCopy;

		[FieldOffset(0)]
		public ulong Value;

		public uint this[int i]
		{
			get => 0 == (1 & i) ? Color.Value : ColorCopy.Value;
			set { if (0 == (1 & i)) Color.Value = value; else ColorCopy.Value = value; }
		}

		public static bool operator == (Ambient L, Ambient R)
		{
			return
				L.Color.R == R.Color.R &&
				L.Color.G == R.Color.G &&
				L.Color.B == R.Color.B &&
				L.ColorCopy.R == R.ColorCopy.R &&
				L.ColorCopy.G == R.ColorCopy.G &&
				L.ColorCopy.B == R.ColorCopy.B;
		}
		public static bool operator !=(Ambient L, Ambient R)
		{
			return
				L.Color.R != R.Color.R ||
				L.Color.G != R.Color.G ||
				L.Color.B != R.Color.B ||
				L.ColorCopy.R != R.ColorCopy.R ||
				L.ColorCopy.G != R.ColorCopy.G ||
				L.ColorCopy.B != R.ColorCopy.B;
		}
		public static bool Equals(ref Ambient L, ref Ambient R)
		{
			return
				L.Color.R == R.Color.R &&
				L.Color.G == R.Color.G &&
				L.Color.B == R.Color.B &&
				L.ColorCopy.R == R.ColorCopy.R &&
				L.ColorCopy.G == R.ColorCopy.G &&
				L.ColorCopy.B == R.ColorCopy.B;
		}
		public static bool Inequals(ref Ambient L, ref Ambient R)
		{
			return
				L.Color.R != R.Color.R ||
				L.Color.G != R.Color.G ||
				L.Color.B != R.Color.B ||
				L.ColorCopy.R != R.ColorCopy.R ||
				L.ColorCopy.G != R.ColorCopy.G ||
				L.ColorCopy.B != R.ColorCopy.B;
		}
		public static int GetHashCode(ref Ambient Ambient)
		{
			unchecked
			{
				return
					(((int)Ambient.Color.R << 16) | ((int)Ambient.Color.G << 8) | Ambient.Color.B) ^
					(((Ambient.ColorCopy.R - (int)Ambient.ColorCopy.R) * (3 << 24)) +
					((Ambient.ColorCopy.G - (int)Ambient.ColorCopy.G) * (3 << 25)) +
					((Ambient.ColorCopy.B - (int)Ambient.ColorCopy.B) * (3 << 26)));
			}
		}
		public override int GetHashCode()
		{
			return GetHashCode(ref this);
		}
		public bool Equals(Ambient other)
		{
			return Equals(ref this, ref other);
		}
		public bool Equals(ref Ambient other)
		{
			return Equals(ref this, ref other);
		}
		public override bool Equals(object obj)
		{
			return obj is Ambient && ((Ambient)obj).Equals(ref this);
		}
		public override string ToString()
		{
			// better than nothing.
			return "Ambient";
		}
	}
	public interface LightContainer
	{
		int Length { get; }
		Light this[int i] { get; set; }
		Ambient Ambient { get; set; }
		void Get(out LightArray LightArray);
		void Set(ref LightArray LightArray);
	}
	[StructLayout(LayoutKind.Explicit)]
	public struct Light : IEquatable<Light>
	{
		[FieldOffset(0)]
		public Ambient AsAmbient;

		// alpha is ignored when testing for equality and generating hash code.
		[FieldOffset(0)]
		public Color4a Color;

		// alpha is ignored when testing for equality and generating hash code.
		[FieldOffset(4)]
		public Color4a ColorCopy;

		[FieldOffset(0)]
		public ulong Pad0;

		// W is ignored when testing for equality and generating hash code.
		[FieldOffset(8)]
		public Vector4c Normal;

		// completely ignored from equality and hash code.
		[FieldOffset(12)]
		public uint WordPad;

		[FieldOffset(8)]
		public ulong Pad1;

		public static Light InitOn => new Light { Color = Color4a.U64_Ambient, ColorCopy = Color4a.U64_Ambient, Normal = { X=0,Y=1,Z=0, } };
		public static Light InitOff => new Light {};

		public uint this[int i]
		{
			get
			{
				return 0 == (2 & i) ?
					0 == (1 & i) ? Color.Value : ColorCopy.Value :
					0 == (1 & i) ? Normal.Packed : WordPad;
			}
			set
			{
				if (0 == (2 & i))
					if (0 == (1 & i))
						Color.Value = value;
					else
						ColorCopy.Value = value;
				else if (0 == (1 & i))
					Normal.Packed = value;
				else
					WordPad = value;
			}
		}

		public static int GetHashCode(ref Light Value)
		{
			return Ambient.GetHashCode(ref Value.AsAmbient) ^(
				(((Value.Normal.X & 127) << (32 - (3 + (7 * (0 + 1))))) | (Value.Normal.X < 0 ? (1 << 31) : 0)) |
				(((Value.Normal.Y & 127) << (32 - (3 + (7 * (1 + 1))))) | (Value.Normal.Y < 0 ? (1 << 30) : 0)) |
				(((Value.Normal.Z & 127) << (32 - (3 + (7 * (2 + 1))))) | (Value.Normal.Z < 0 ? (1 << 29) : 0)) );
		}
		public override int GetHashCode()
		{
			return GetHashCode(ref this);
		}
		public static bool operator ==(Light L, Light R)
		{
			return
				L.Normal.X == R.Normal.X &&
				L.Normal.Y == R.Normal.Y &&
				L.Normal.Z == R.Normal.Z &&
				Ambient.Equals(ref L.AsAmbient, ref R.AsAmbient);
		}
		public static bool operator !=(Light L, Light R)
		{
			return
				L.Normal.X != R.Normal.X ||
				L.Normal.Y != R.Normal.Y ||
				L.Normal.Z != R.Normal.Z ||
				Ambient.Inequals(ref L.AsAmbient, ref R.AsAmbient);
		}
		public static bool Equals(ref Light L,ref Light R)
		{
			return
				L.Normal.X == R.Normal.X &&
				L.Normal.Y == R.Normal.Y &&
				L.Normal.Z == R.Normal.Z &&
				Ambient.Equals(ref L.AsAmbient, ref R.AsAmbient);
		}
		public static bool Inequals(ref Light L,ref  Light R)
		{
			return
				L.Normal.X != R.Normal.X ||
				L.Normal.Y != R.Normal.Y ||
				L.Normal.Z != R.Normal.Z ||
				Ambient.Inequals(ref L.AsAmbient, ref R.AsAmbient);
		}
		public bool Equals(Light other)
		{
			return Equals(ref this, ref other);
		}
		public bool Equals(ref Light other)
		{
			return Equals(ref this, ref other);
		}
		public override bool Equals(object obj)
		{
			return obj is Light && ((Light)obj).Equals(ref this);
		}
		public override string ToString()
		{
			return "Light";
		}
	}
	partial struct LightArray
	{
		public static bool Equals(ref LightArray L, ref LightArray R, byte Count){
			return
				(Count < 4) ?
				(Count < 2) ?
				(Count < 1) ? Lights0.Equals(ref L.Lights0, ref R.Lights0)
				: Lights1.Equals(ref L.Lights1, ref R.Lights1)
				: (Count < 3) ? Lights2.Equals(ref L.Lights2, ref R.Lights2)
				: Lights3.Equals(ref L.Lights3, ref R.Lights3)
				: (Count < 6) ?
				(Count < 5) ? Lights4.Equals(ref L.Lights4, ref R.Lights4)
				: Lights5.Equals(ref L.Lights5, ref R.Lights5)
				: (Count < 7) ? Lights6.Equals(ref L.Lights6, ref R.Lights6)
				: Lights7.Equals(ref L.Lights7, ref R.Lights7);
		}
		public static int GetHashCode(
			ref LightArray L,
			byte Count)
		{
			return
				(Count < 4) ?
				(Count < 2) ?
				(Count < 1) ? Lights0.GetHashCode(ref L.Lights0)
				: Lights1.GetHashCode(ref L.Lights1)
				: (Count < 3) ? Lights2.GetHashCode(ref L.Lights2)
				: Lights3.GetHashCode(ref L.Lights3)
				: (Count < 6) ?
				(Count < 5) ? Lights4.GetHashCode(ref L.Lights4)
				: Lights5.GetHashCode(ref L.Lights5)
				: (Count < 7) ? Lights6.GetHashCode(ref L.Lights6)
				: Lights7.GetHashCode(ref L.Lights7);
		}
		public int GetHashCode(byte LightCount) { return GetHashCode(ref this, LightCount); }

		public static void GetAmbient(
			ref LightArray L,
			byte Count,
			out Ambient Ambient)
		{
			Ambient =
				(Count < 4) ?
				(Count < 2) ?
				(Count < 1) ? L.Lights0.Ambient
				: L.Lights1.Ambient
				: (Count < 3) ? L.Lights2.Ambient
				: L.Lights3.Ambient
				: (Count < 6) ?
				(Count < 5) ? L.Lights4.Ambient
				: L.Lights5.Ambient
				: (Count < 7) ? L.Lights6.Ambient
				: L.Lights7.Ambient;
		}
		public static Ambient GetAmbient(
			ref LightArray L,
			byte Count)
		{
			return
				(Count < 4) ?
				(Count < 2) ?
				(Count < 1) ? L.Lights0.Ambient
				: L.Lights1.Ambient
				: (Count < 3) ? L.Lights2.Ambient
				: L.Lights3.Ambient
				: (Count < 6) ?
				(Count < 5) ? L.Lights4.Ambient
				: L.Lights5.Ambient
				: (Count < 7) ? L.Lights6.Ambient
				: L.Lights7.Ambient;
		}
		public void GetAmbient(byte Count, out Ambient Ambient)
		{
			GetAmbient(ref this, Count, out Ambient);
		}
		public Ambient GetAmbient(byte Count)
		{
			return GetAmbient(ref this, Count);
		}
	}

	public struct Transform : IEquatable<Transform>
	{
		public Vector3 translation;
		public float scale;
		public Quaternion rotation;
		public void GL_Load()
		{
			GL.Translate(translation.X, translation.Y, translation.Z);
			U.GL_Rotate(ref rotation);
			GL.Scale(scale,scale,scale);
		}
		public Vector3 Degrees
		{
			get
			{
				Vector3 v;
				U.QDegrees(rotation.X, rotation.Y, rotation.Z, rotation.W, out v.X, out v.Y, out v.Z);
				return v;
			}

			set
			{
				rotation = new Quaternion(Degrees);
			}
		}
		public float Pitch
		{
			get { return U.QDegreesPitch(rotation.X, rotation.Y, rotation.Z, rotation.W); }
			set
			{
				var Degrees = this.Degrees;
				var Delta = U.NormalizeDegrees180(value - (double)Degrees.X);
				if (!(Delta != 0f))
					return;
				Degrees.X = (float)U.NormalizeDegrees180(Degrees.X + Delta);
				this.Degrees = Degrees;
			}
		}
		public float Yaw
		{
			get { return U.QDegreesYaw(rotation.X, rotation.Y, rotation.Z, rotation.W); }
			set
			{
				var Degrees = this.Degrees;
				var Delta = U.NormalizeDegrees180(value - (double)Degrees.Y);
				if (!(Delta != 0f))
					return;
				Degrees.Y = (float)U.NormalizeDegrees180(Degrees.Y + Delta);
				this.Degrees = Degrees;
			}
		}
		public float Roll
		{
			get { return U.QDegreesRoll(rotation.X, rotation.Y, rotation.Z, rotation.W); }
			set
			{
				var Degrees = this.Degrees;
				var Delta = U.NormalizeDegrees180(value - (double)Degrees.Z);
				if (!(Delta != 0f))
					return;
				Degrees.Z = (float)U.NormalizeDegrees180(Degrees.Z + Delta);
				this.Degrees = Degrees;
			}
		}
		public float Scale { get => scale; set { scale = value; } }
		public float X { get => translation.X; set { translation.X = value; } }
		public float Y { get => translation.Y; set { translation.Y = value; } }
		public float Z { get => translation.Z; set { translation.Z = value; } }
		public Vector3 Translation { get => translation; set { translation = value; } }
		public Quaternion Rotation { get => rotation; set { rotation = value; } }
		public Vector3 RotationAxis
		{
			get
			{
				Vector3 axis;
				float angle;
				rotation.ToAxisAngle(out axis, out angle);

				if (angle < 0f)
					axis = -axis;
				angle = axis.LengthSquared;
				if (angle != 1f)
					axis /= (float)System.Math.Sqrt(angle);
				return axis;
			}
			set
			{
				var sqrmag = value.LengthSquared;
				if (!(sqrmag >= float.Epsilon)) return;
				Vector3 axis; float angle;
				rotation.ToAxisAngle(out axis, out angle);
				if (angle < 0f) angle = -angle;

				if (sqrmag != 1f)
					rotation = Quaternion.FromAxisAngle(value / (float)System.Math.Sqrt(sqrmag), angle);
				else
					rotation = Quaternion.FromAxisAngle(value, angle);
			}
		}
		public float RotationAngle
		{
			get
			{
				Vector3 junk;
				float angle;
				rotation.ToAxisAngle(out junk, out angle);
				return angle < 0f ? -angle : angle;
			}
			set
			{
				Vector3 axis; float angle;
				rotation.ToAxisAngle(out axis, out angle);
				rotation = Quaternion.FromAxisAngle(((value < 0f) ^ (angle < 0f)) ? -axis : axis, value < 0f ? -value : value);
			}
		}
		public Vector4 AngleAxis { get { return rotation.ToAxisAngle(); } set { rotation = Quaternion.FromAxisAngle(value.Xyz, value.W); } }
		public Vector3 AngularVector
		{
			get
			{
				Vector3 axis;
				float angle;
				rotation.ToAxisAngle(out axis, out angle);
				return axis * angle;
			}
			set
			{
				float angle = value.Length;
				if (angle < float.Epsilon)
					rotation = Quaternion.Identity;
				else
					rotation = Quaternion.FromAxisAngle(value / angle, angle).Normalized();
			}
		}
		public Matrix4 Matrix
		{
			get
			{
				return Matrix4.CreateScale(scale) * 
					Matrix4.CreateFromQuaternion(rotation) *
					Matrix4.CreateTranslation(translation);
			}
			set
			{
				translation = value.ExtractTranslation();
				var scalev = value.ExtractScale();
				scale = scalev.Length / (
					(scalev.X < 0f || scalev.Y < 0f || scalev.Z < 0f) ?
					-1.732050807568877f : 1.732050807568877f
					);
				rotation = value.ExtractRotation();
			}
		}

		public static readonly Transform Identity = new Transform { scale = 1f, rotation = Quaternion.Identity, };

		public override int GetHashCode()
		{
			int hc = translation.GetHashCode();
			hc ^= (ushort.MaxValue * (1f - scale)).GetHashCode();
			return hc;
		}
		public bool Equals(Transform other)
		{
			return translation == other.translation &&
				U.QEquals(ref rotation, ref other.rotation) &&
				scale == other.scale;
		}
		public bool Equals(ref Transform other)
		{
			return translation == other.translation &&
				U.QEquals(ref rotation, ref other.rotation) &&
				scale == other.scale;
		}
		public static bool Equals(ref Transform L, ref Transform R)
		{
			return L.translation == R.translation &&
				U.QEquals(ref L.rotation, ref R.rotation) &&
				L.scale == R.scale;
		}
		public static bool Equals(Transform L, Transform R)
		{
			return L.translation == R.translation &&
				U.QEquals(ref L.rotation, ref R.rotation) &&
				L.scale == R.scale;
		}
		public bool Inequals(Transform other)
		{
			return translation != other.translation ||
				U.QInequals(ref rotation, ref other.rotation) ||
				scale != other.scale;
		}
		public bool Inequals(ref Transform other)
		{
			return translation != other.translation ||
				U.QInequals(ref rotation, ref other.rotation) ||
				scale != other.scale;
		}
		public static bool Inequals(ref Transform L, ref Transform R)
		{
			return L.translation != R.translation ||
				U.QInequals(ref L.rotation, ref R.rotation) ||
				L.scale != R.scale;
		}
		public static bool Inequals(Transform L, Transform R)
		{
			return L.translation != R.translation ||
				U.QInequals(ref L.rotation, ref R.rotation) ||
				L.scale != R.scale;
		}
		public override bool Equals(object obj)
		{
			return obj is Transform && Equals((Transform)obj);
		}
		public override string ToString()
		{
			return string.Format("{{\"t\":[{0},{1},{2}],\"r\":[{3},{4},{5},{6}],\"s\":{7}}}",
				translation.X, translation.Y, translation.Z,
				rotation.X, rotation.Y, rotation.Z, rotation.W,
				scale);
		}
		public static bool operator ==(Transform lhs, Transform rhs)
		{
			return Equals(ref lhs, ref rhs);
		}
		public static bool operator !=(Transform lhs, Transform rhs)
		{
			return Inequals(ref lhs, ref rhs);
		}

		private static Transform CTFS(Matrix4 LHS, Matrix4 RHS, float Scale)
		{
			Transform o;
			Matrix4 Prod = (LHS * RHS).ClearScale();
			o.rotation = Prod.ExtractRotation(false);
			var Sign = Scale < 0f ? -1f : Scale > 0f ? 1f : 0f;
			o.rotation = Quaternion.FromMatrix(
				new Matrix3(
					o.rotation * new Vector3(Sign, 0f, 0f),
					o.rotation * new Vector3(0f, Sign, 0f),
					o.rotation * new Vector3(0f, 0f, Sign))).Normalized();
			o.scale = Scale;
			o.translation = Prod.ExtractTranslation();
			return o;
		}
		public static Transform operator *(Transform lhs, Transform rhs)
		{
			return (lhs.scale < 0f || rhs.scale < 0f) ?
				CTFS(lhs.Matrix, rhs.Matrix, lhs.scale * rhs.scale) :
				new Transform
				{
					translation = rhs.translation + (rhs.rotation * (lhs.translation * rhs.scale)),
					rotation = (rhs.rotation * lhs.rotation).Normalized(),
					scale = rhs.scale * lhs.scale
				};
		}
		public Vector3 TransformVector(Vector3 value)
		{
			return rotation * (value * scale);
		}
		public Vector3 TransformVectorNoScale(Vector3 value)
		{
			return rotation * value;
		}
		public Vector3 TransformPosition(Vector3 value)
		{
			return (rotation * (value * scale)) + translation;
		}
		public Vector3 TransformPositionNoScale(Vector3 value)
		{
			return (rotation * value) + translation;
		}
		public Vector3 InverseTransformVectorNoScale(Vector3 value)
		{
			Vector3 Q = new Vector3(-rotation.X, -rotation.Y, -rotation.Z), S, T;
			Vector3.Cross(ref Q, ref value, out T);
			T *= 2;
			Vector3.Cross(ref Q, ref T, out S);
			return value + (rotation.W * T) + S;
		}
		public Vector3 InverseTransformPositionNoScale(Vector3 value)
		{
			return InverseTransformVectorNoScale(value - translation);
		}
		float ReciprocalSafe
		{
			get
			{
				return scale.SafeReciprocal();
			}
		}
		public Vector3 InverseTransformPosition(Vector3 value)
		{
			return InverseTransformPositionNoScale(value) * ReciprocalSafe;
		}
		public Vector3 InverseTransformVector(Vector3 value)
		{
			return InverseTransformVectorNoScale(value) * ReciprocalSafe;
		}
		public Transform Inversed
		{
			get
			{
				Transform o;
				o.rotation = new Quaternion(-rotation.X, -rotation.Y, -rotation.Z, rotation.W);
				o.scale = ReciprocalSafe;
				o.translation = o.rotation * ((-o.scale) * translation);
				return o;
			}
			set
			{
				this = value.Inversed;
			}
		}
	}

	public struct RenderCamera
	{
		/// <summary>
		/// the view matrix.
		/// </summary>
		public Matrix4 View;
		/// <summary>
		/// the projection matrix.
		/// </summary>
		public Matrix4 Proj;
		/// <summary>
		/// the view matrix * projection matrix.
		/// </summary>
		public Matrix4 ViewProj;
		/// <summary>
		/// the world space transform of the camera.
		/// </summary>
		public Transform Transform;
		/// <summary>
		/// the world space X normal/heading.
		/// </summary>
		public Vector3 X;
		/// <summary>
		/// the world space Y normal/heading.
		/// </summary>
		public Vector3 Y;
		/// <summary>
		/// the world space Z normal/heading.
		/// </summary>
		public Vector3 Z;
		/// <summary>
		/// makes other_transform face this camera.
		/// </summary>
		/// <param name="other_transform">the transform you want to face the camera</param>
		/// <param name="delta_rotation">set to the rotation applied to other_transform to have it face the camera</param>
		public void MakeBillboard(ref Transform other_transform, out Quaternion delta_rotation)
		{
			Vector3 look = other_transform.InverseTransformPosition(Transform.translation),
				up = other_transform.InverseTransformVectorNoScale(Y);

			look.Normalize();
			
			up.Normalize();
			{
				Vector3 right;
				Vector3.Cross(ref look, ref up, out right);
				right.Normalize();
				OpenTK.Vector3.Cross(ref up, ref right, out look);
			}
			look.X = -look.X;
			look.Y = -look.Y;
			look.Z = -look.Z;
			look.Normalize();
			{
				var rot_copy = other_transform.rotation;
				delta_rotation = OpenTK.Matrix4.LookAt(
						0, 0, 0, look.X, look.Y, look.Z, up.X, up.Y, up.Z).ExtractRotation();
				delta_rotation.Invert();
				delta_rotation.Normalize();

				Quaternion.Multiply(ref rot_copy, ref delta_rotation, out other_transform.rotation);
			}
		}
		/// <summary>
		/// makes other_transform face this camera.
		/// </summary>
		/// <param name="other_transform">the transform you want to face the camera</param>
		public void MakeBillboard(ref Transform other_transform)
		{
			Vector3 look = other_transform.InverseTransformPosition(Transform.translation),
				up = other_transform.InverseTransformVectorNoScale(Y);

			look.Normalize();
			up.Normalize();
			{
				Vector3 right;
				Vector3.Cross(ref look, ref up, out right);
				right.Normalize();
				OpenTK.Vector3.Cross(ref up, ref right, out look);
			}
			look.X = -look.X;
			look.Y = -look.Y;
			look.Z = -look.Z;
			look.Normalize();
			{
				var rot_copy = other_transform.rotation;
				var delta_rotation = OpenTK.Matrix4.LookAt(
						0, 0, 0, look.X, look.Y, look.Z, up.X, up.Y, up.Z).ExtractRotation();
				delta_rotation.Invert();
				delta_rotation.Normalize();

				Quaternion.Multiply(ref rot_copy, ref delta_rotation, out other_transform.rotation);
			}
		}
		public void MakeBillboardAndRotateGL(ref Transform other_transform)
		{
			MakeBillboard(ref other_transform, out Quaternion delta);
			TransformUtility.GL_Rotate(ref delta);
		}
	}

	public static class TransformUtility
	{

		static readonly Quaternion[] QXYZ = new Quaternion[360*3];
		static TransformUtility()
		{
			float rad;
			for (int x = 0; x < 360; x++)
			{
				rad = (float)(((x >= 180 ? (x - 360) : x) * System.Math.PI) / 180);
				QXYZ[x] = Quaternion.FromAxisAngle(Vector3.UnitX, rad);
				QXYZ[360+x] = Quaternion.FromAxisAngle(Vector3.UnitY, rad);
				QXYZ[720+x] = Quaternion.FromAxisAngle(Vector3.UnitZ, rad);
			}
		}
		public static ushort NormalizeDegrees(short Degrees)
		{
			if (Degrees >= 360 || Degrees <= -360)
				Degrees %= 360;

			return (Degrees < 0) ? (ushort)(Degrees + 360) : (ushort)Degrees;
		}
		/// <summary>
		/// returns -180 to +179
		/// </summary>
		public static short NormalizeDegrees180(short Degrees)
		{
			if (Degrees >= 360 || Degrees <= -360)
				Degrees %= 360;
			return (Degrees < -180) ? (short)(Degrees + 360) : (Degrees >= 180) ? (short)(Degrees - 360) : Degrees;
		}
		public static short NegateDegrees180(this short x)
		{
			x = NormalizeDegrees180(x);
			return x > -180 ? (short)-x : x;
		}
		public static ushort NegateDegrees360(this short x)
		{
			x = NormalizeDegrees180(x);
			return x >= 0 ? (ushort)x : (ushort)(360 + x);
		}

		public static double NormalizeDegrees(double Value)
		{
			if (Value >= 360d || Value <= -360d)
				Value %= 360d;

			if (Value < 0d) Value += 360d;

			return Value;
		}
		public static double NormalizeDegrees180(double Value)
		{
			if( Value >= 360d || Value <= -360d )
				Value %= 360d;

			if (Value < -180d) Value += 360d;
			else if (Value >= 180d) Value -= 360d;
			return Value;
		}
		public static void LoadQuaternionX(out Quaternion Result, short Degrees)
		{
			Result = QXYZ[NormalizeDegrees(Degrees)];
		}
		public static void LoadQuaternionY(out Quaternion Result, short Degrees)
		{
			Result = QXYZ[360 + NormalizeDegrees(Degrees)];
		}
		public static void LoadQuaternionZ(out Quaternion Result, short Degrees)
		{
			Result = QXYZ[720 + NormalizeDegrees(Degrees)];
		}
		public static void LoadQuaternion(out Quaternion Result, short Degrees, byte Axis)
		{
			if (Axis >= 3)
				Axis %= 3;

			Result = QXYZ[(360*(int)Axis)+NormalizeDegrees(Degrees)];
		}
		/// <summary>
		/// rotates Result on the right side where the left side is loaded via degree and axis argument.
		/// </summary>
		public static void LRotate(ref Quaternion Result, short Degrees, byte Axis)
		{
			if (Axis >= 3)
				Axis %= 3;
			Quaternion Copy = Result;
			Quaternion.Multiply(ref QXYZ[(360 * (int)Axis) + NormalizeDegrees(Degrees)], ref Copy, out Result);
		}
		/// <summary>
		/// rotates Result on the left side where the right side is loaded via degree and axis argument.
		/// </summary>
		public static void RRotate(ref Quaternion Result, short Degrees, byte Axis)
		{
			if (Axis >= 3)
				Axis %= 3;
			Quaternion Copy = Result;
			Quaternion.Multiply(ref Copy, ref QXYZ[(360 * (int)Axis) + NormalizeDegrees(Degrees)], out Result);
		}
		/// <summary>
		/// rotates Result on the right side where the left side is loaded via degree argument.
		/// </summary>
		public static void LRotateX(ref Quaternion Result, short Degrees)
		{
			Quaternion Copy = Result;
			Quaternion.Multiply(ref QXYZ[NormalizeDegrees(Degrees)], ref Copy, out Result);
		}
		/// <summary>
		/// rotates Result on the left side where the right side is loaded via degree argument.
		/// </summary>
		public static void RRotateX(ref Quaternion Result, short Degrees)
		{
			Quaternion Copy = Result;
			Quaternion.Multiply(ref Copy, ref QXYZ[NormalizeDegrees(Degrees)], out Result);
		}
		/// <summary>
		/// rotates Result on the right side where the left side is loaded via degree argument.
		/// </summary>
		public static void LRotateY(ref Quaternion Result, short Degrees)
		{
			Quaternion Copy = Result;
			Quaternion.Multiply(ref QXYZ[360+NormalizeDegrees(Degrees)], ref Copy, out Result);
		}
		/// <summary>
		/// rotates Result on the left side where the right side is loaded via degree argument.
		/// </summary>
		public static void RRotateY(ref Quaternion Result, short Degrees)
		{
			Quaternion Copy = Result;
			Quaternion.Multiply(ref Copy, ref QXYZ[360+NormalizeDegrees(Degrees)], out Result);
		}
		/// <summary>
		/// rotates Result on the right side where the left side is loaded via degree argument.
		/// </summary>
		public static void LRotateZ(ref Quaternion Result, short Degrees)
		{
			Quaternion Copy = Result;
			Quaternion.Multiply(ref QXYZ[720 + NormalizeDegrees(Degrees)], ref Copy, out Result);
		}
		/// <summary>
		/// rotates Result on the left side where the right side is loaded via degree argument.
		/// </summary>
		public static void RRotateZ(ref Quaternion Result, short Degrees)
		{
			Quaternion Copy = Result;
			Quaternion.Multiply(ref Copy, ref QXYZ[720 + NormalizeDegrees(Degrees)], out Result);
		}


		const float QDegThresh = 0.4999995f;
		public static float Asin(float Value)
		{
			// Clamp input to [-1,1].
			float x = (Value < 0f) ? -Value : Value;
			float omx = 1.0f - x;
			float result;
			if (omx <= 0.0f)
			{
				return ((Value >= 0.0f) ? (1.5707963050f) : (-(1.5707963050f)));
			}
			else
			{
				result = ((((((-0.0012624911f * x + 0.0066700901f) * x - 0.0170881256f) * x + 0.0308918810f) * x - 0.0501743046f) * x + 0.0889789874f) * x - 0.2145988016f) * x + (1.5707963050f);
				if (omx != 1f)
					result *= (float)System.Math.Sqrt(omx);
				return ((Value >= 0.0f) ? (1.5707963050f) - result : (result - (1.5707963050f)));
			}
		}
		public static void QDegrees(float X, float Y, float Z, float W, out float Pitch, out float Yaw, out float Roll)
		{
			float SingularityTest = Z * X - W * Y;
			double YawAngle = 2.0 * (W * (double)Z + X * (double)Y);
			YawAngle = (System.Math.Atan2(YawAngle, (1.0 - 2.0 * ((Y * (double)Y) + (Z * (double)Z)))) * 180) / Math.PI;
			if (SingularityTest < -QDegThresh)
			{
				Pitch = -90f;
				Yaw = (float)YawAngle;
				Roll = (float)NormalizeDegrees180(-YawAngle - (2 * System.Math.Atan2(X, W) * (180.0 / Math.PI)));
			}
			else if (SingularityTest > QDegThresh)
			{
				Pitch = 90f;
				Yaw = (float)YawAngle;
				Roll = (float)NormalizeDegrees180(YawAngle - (2 * System.Math.Atan2(X, W) * (180.0 / Math.PI)));
			}
			else
			{
				Pitch = Asin(2f * (SingularityTest)) * (float)(180.0 / Math.PI);
				Yaw = (float)YawAngle;
				Roll = (float)(System.Math.Atan2(-2 * (W * (double)X + Y * (double)Z), (1d - 2 * ((X * (double)X) + (Y * (double)Y)))) * (180.0 / Math.PI));
			}
		}
		public static float QDegreesPitch(float X, float Y, float Z, float W)
		{
			float SingularityTest = Z * X - W * Y;
			if (SingularityTest < -QDegThresh)
			{
				return -90f;
			}
			else if (SingularityTest > QDegThresh)
			{
				return 90f;
			}
			else
			{
				return Asin(2f * (SingularityTest)) * (float)(180.0 / Math.PI);
			}
		}
		public static float QDegreesRoll(
			float X, float Y, float Z, float W)
		{
			float SingularityTest = Z * X - W * Y;
			double YawAngle;
			if (SingularityTest < -QDegThresh)
			{
				YawAngle = -1.0;
			}
			else if (SingularityTest > QDegThresh)
			{
				YawAngle = 1.0;
			}
			else
			{
				return (float)(System.Math.Atan2(-2 * (W * (double)X + Y * (double)Z), (1d - 2 * ((X * (double)X) + (Y * (double)Y)))) * (180.0 / Math.PI));
			}
			return (float)NormalizeDegrees180(YawAngle * (System.Math.Atan2(2.0 * (W * (double)Z + X * (double)Y), (1.0 - 2.0 * ((Y * (double)Y) + (Z * (double)Z)))) * 180) / Math.PI);

		}
		public static float QDegreesYaw(float X, float Y, float Z, float W)
		{
			return
				(float)(
				(System.Math.Atan2(2.0 * (W * (double)Z + X * (double)Y), (1.0 - 2.0 * ((Y * (double)Y) + (Z * (double)Z)))) * 180) / Math.PI
				);
		}
		public static void QDegrees([In]ref Quaternion XYZW, out float Pitch, out float Yaw, out float Roll)
		{
			QDegrees(XYZW.X, XYZW.Y, XYZW.Z, XYZW.W, out Pitch, out Yaw, out Roll);
		}
		public static void QDegrees(this Quaternion XYZW, out float Pitch, out float Yaw, out float Roll)
		{
			QDegrees(XYZW.X, XYZW.Y, XYZW.Z, XYZW.W, out Pitch, out Yaw, out Roll);
		}
		public static bool QEquals(
			float LX, float LY, float LZ, float LW, float RX, float RY, float RZ, float RW)
		{
			return LX == RX ?
				(LX == 0f) ? (
					LY == RY ?
					(LY == 0f) ? (
						LZ == RZ ?
						(LZ == 0f) ? (
							LW == RW || -LW == RW
						) : LW == RW :
						(-LZ == RZ && -LW == RW)
					) : (LZ == RZ && LW == RW) :
					(-LY == RY && -LZ == RZ && -LW == RW)
				) : (LY == RY && LZ == RZ && LW == RW) :
				(-LX == RX && -LY == RY && -LZ == RZ && -LW == RW);
		}
		public static bool QInequals(
			float LX, float LY, float LZ, float LW, 
			float RX, float RY, float RZ, float RW)
		{
			return LX == RX ?
				(LX == 0f) ? (
					LY == RY ?
					(LY == 0f) ? (
						LZ == RZ ?
						(LZ == 0f) ? (
							LW != RW && -LW != RW
						) : LW == RW :
						(-LZ != RZ || -LW != RW)
					) : (LZ != RZ || LW != RW) :
					(-LY != RY || -LZ != RZ || -LW != RW)
				) : (LY != RY || LZ != RZ || LW != RW) :
				(-LX != RX || -LY != RY || -LZ != RZ || -LW != RW);
		}
		public static bool QEquals([In]ref Quaternion L, [In]ref Quaternion R)
		{
			return QEquals(L.X, L.Y, L.Z, L.W, R.X, R.Y, R.Z, R.W);
		}
		public static bool QInequals([In]ref Quaternion L, [In]ref Quaternion R)
		{
			return QInequals(L.X, L.Y, L.Z, L.W, R.X, R.Y, R.Z, R.W);
		}
		public static bool QEquals(Quaternion L, Quaternion R)
		{
			return QEquals(L.X, L.Y, L.Z, L.W, R.X, R.Y, R.Z, R.W);
		}
		public static bool QInequals(Quaternion L, Quaternion R)
		{
			return QInequals(L.X, L.Y, L.Z, L.W, R.X, R.Y, R.Z, R.W);
		}
		public static void QHash([In]ref Quaternion rotation, [In,Out]ref int hc)
		{
			float temp;
			if (!((temp = rotation.X) != 0f))
			{
				if (!((temp = rotation.Y) != 0f))
				{
					if (!((temp = rotation.Z) != 0f))
					{
						if (!((temp = rotation.W) != 0f))
						{
							hc ^= -1;
						}
						else if (temp < 0f)
						{
							hc ^= new Quaternion(0f, 0f, 0f, -temp).GetHashCode();
						}
						else
						{
							hc ^= new Quaternion(0f, 0f, 0f, temp).GetHashCode();
						}
					}
					else if (temp < 0f)
					{
						hc ^= new Quaternion(0f, 0f, -temp, Zero(-rotation.W)).GetHashCode();
					}
					else
					{
						hc ^= new Quaternion(0f, 0f, temp, Zero(rotation.W)).GetHashCode();
					}
				}
				else if (temp < 0f)
				{
					hc ^= new Quaternion(0f, -temp, Zero(-rotation.Z), Zero(-rotation.W)).GetHashCode();
				}
				else
				{
					hc ^= new Quaternion(0f, temp, Zero(rotation.Z), Zero(rotation.W)).GetHashCode();
				}
			}
			else if (temp < 0f)
			{
				hc ^= new Quaternion(-temp, Zero(-rotation.Y), Zero(-rotation.Z), Zero(-rotation.W)).GetHashCode();
			}
			else
			{
				hc ^= new Quaternion(temp,Zero(rotation.Y),Zero(rotation.Z),Zero(rotation.W)).GetHashCode();
			}
		}
		public static int QHash([In]ref Quaternion rotation)
		{
			int hc = 0;
			QHash(ref rotation, ref hc);
			return hc;
		}
		/// <summary>
		/// if value is not-not zero then zero is returned.
		/// (this returns zero when either zero or any number not equatable to any number (such as NaNs) is passed.)
		/// this also rules out negative zero to positive zero.
		/// </summary>
		public static float Zero(this float value) { return (!(value != 0f)) ? 0f : value; }
		/// <summary>
		/// see notes on Zero, this goes a step further in checking for infinity (where zero will be returned).
		/// </summary>
		public static float ZeroInf(this float value) { return ((!(value != 0f)) || !(value <= float.MaxValue && value >= -float.MaxValue)) ? 0f : value; }
		/// <summary>
		/// since all quats (that are not completely zero or contain NaN) have two identical versions
		/// this modifies the input to be deterministic by searching X then Y then Z then W.
		/// once a non zero is found (in the order above) if its value is negative it all elements there after are negated.
		/// 
		/// </summary>
		/// <param name="rotation"></param>
		public static void QDeterministic([In,Out]ref Quaternion rotation)
		{
			float temp;
			if (!((temp = rotation.X) != 0f))
			{
				if (!((temp = rotation.Y) != 0f))
				{
					if (!((temp = rotation.Z) != 0f))
					{
						if (!((temp = rotation.W) != 0f))
						{
							rotation = default(Quaternion);
						}
						else if (temp < 0f)
						{
							rotation = new Quaternion(0f, 0f, 0f, -temp);
						}
						else
						{
							rotation = new Quaternion(0f, 0f, 0f, temp);
						}
					}
					else if (temp < 0f)
					{
						rotation = new Quaternion(0f, 0f, -temp, Zero(-rotation.W));
					}
					else
					{
						rotation = new Quaternion(0f, 0f, temp, Zero(rotation.W));
					}
				}
				else if (temp < 0f)
				{
					rotation = new Quaternion(0f, -temp, Zero(-rotation.Z), Zero(-rotation.W));
				}
				else
				{
					rotation = new Quaternion(0f, temp, Zero(rotation.Z), Zero(rotation.W));
				}
			}
			else if (temp < 0f)
			{
				rotation = new Quaternion(-temp, Zero(-rotation.Y), Zero(-rotation.Z), Zero(-rotation.W));
			}
			else
			{
				rotation = new Quaternion(temp, Zero(rotation.Y), Zero(rotation.Z), Zero(rotation.W));
			}
		}
		public static void QDeterministic([In]ref Quaternion rotation, out Quaternion deterministic)
		{
			deterministic = rotation;
			QDeterministic(ref deterministic);
		}
		public static float SafeReciprocal(this float scale)
		{
			return (scale < (1e-8f) ? scale < -(1e-8f) : scale >= (1e-8f)) ? (1f / scale) : 0f;
		}
		public static void GL_Rotate16(ref Vector3s rotation)
		{
			if (rotation.X != 0 || rotation.Y != 0 || rotation.Z != 0)
			{
				Quat16(out Quaternion qrot, rotation.X, rotation.Y, rotation.Z);
				GL_Rotate(ref qrot);
			}
		}
		public static void GL_Rotate16(short X, short Y, short Z)
		{
			if (X != 0 || Y != 0 || Z != 0)
			{
				Quat16(out Quaternion qrot, X, Y, Z);
				GL_Rotate(ref qrot);
			}
		}
		public static void GL_Rotate(ref Quaternion rotation)
		{
			rotation.ToAxisAngle(out Vector3 Axis, out float Angle);
			if (Angle > 0f ? Angle <= float.MaxValue : (Angle < 0f && Angle <= float.MaxValue))
			{
				Angle = (float)((Angle * 180.0) / Math.PI);

				if (Angle > 0f ? Angle <= float.MaxValue : (Angle < 0f && Angle <= float.MaxValue))
					GL.Rotate(Angle, Axis.X, Axis.Y, Axis.Z);
			}
		}
		public static void Quat16(out Quaternion rotation, short x, short y, short z)
		{
			if (0 == x)
				if (0 == y)
					if (0 == z)
						rotation = Quaternion.Identity;
					else
						LoadQuaternionZ(out rotation, z);
				else
				{
					LoadQuaternionY(out rotation, y);
					if (0 != z)
						RRotateZ(ref rotation, z);
				}
			else
			{
				LoadQuaternionX(out rotation, x);

				if (0 != y)
					RRotateY(ref rotation, y);

				if (0 != z)
					RRotateZ(ref rotation, z);
			}
		}
	}

	[StructLayout(LayoutKind.Explicit, Size = 16)]
	public struct TransformI : IEquatable<TransformI>
	{
		[FieldOffset(6*sizeof(short))]
		public Fixed16_16 scale;

		[FieldOffset(3*sizeof(short))]
		public Vector3s translation;

		[FieldOffset(0)]
		public Vector3s rotation;

		[FieldOffset(0)]
		public ulong b;

		[FieldOffset(sizeof(ulong))]
		public ulong a;

		public static readonly TransformI Identity = new TransformI {
			scale = { Value = Fixed16_16.One, },
		};
		
		public static bool Equals(ref TransformI L, ref TransformI R)
		{
			return L.a == R.a && L.b == R.b;
		}
		public static bool Inequals(ref TransformI L, ref TransformI R)
		{
			return L.a != R.a || L.b != R.b;
		}
		public static bool operator == (TransformI L, TransformI R)
		{
			return L.a == R.a && L.b == R.b;
		}
		public static bool operator !=(TransformI L, TransformI R)
		{
			return L.a != R.a || L.b != R.b;
		}
		public bool Equals(ref TransformI other)
		{
			return a == other.a && b == other.b;
		}
		public bool Equals(TransformI other)
		{
			return a == other.a && b == other.b;
		}
		public override bool Equals(object obj)
		{
			return obj is TransformI && ((TransformI)obj).Equals(ref this);
		}
		public override int GetHashCode()
		{
			//lay-z
			return new Vector4s { Packed = (a ^ b),}.GetHashCode();
		}
		public override string ToString()
		{
			return string.Concat("[",
				translation.ToString(), ",",
				string.Concat(rotation.ToString(), ",", scale.PercentString, "]"));
		}
		public static implicit operator Transform(TransformI value)
		{
			value.LoadTransform(out Transform T);
			return T;
		}
		public void Normalize()
		{
			this.rotation.X = (short)U.NormalizeDegrees(this.rotation.X);
			this.rotation.Y = (short)U.NormalizeDegrees(this.rotation.Y);
			this.rotation.Z = (short)U.NormalizeDegrees(this.rotation.Z);
		}
		public TransformI Normalized()
		{
			var o = this; o.Normalize(); return o;
		}
		public void GL_Load()
		{
			GL.Translate(translation.X, translation.Y, translation.Z);
			U.GL_Rotate16(ref rotation);
			var temp = scale.Single;
			GL.Scale(temp, temp, temp);
		}
		public static Transform operator *(TransformI L, TransformI R)
		{
			L.LoadTransform(out Transform TL);
			R.LoadTransform(out Transform TR);
			TL *= TR;
			return TL;
		}
		public static Transform operator *(TransformI L, Transform R)
		{
			L.LoadTransform(out Transform TL);
			TL *= R;
			return TL;
		}
		public static Transform operator *(Transform L, TransformI R)
		{
			R.LoadTransform(out Transform TR);
			return L * TR;
		}
		/*  I am guessing that the angles we return will not be candidate for quat from euler.. so commenting these out for now 
		public Vector3 Degrees => new Vector3 {
			X = U.NormalizeDegrees180(rotation.X),
			Y = U.NormalizeDegrees180(rotation.Y),
			Z = U.NormalizeDegrees180(rotation.Z),
		};
		public float Pitch => U.NormalizeDegrees180(rotation.X);
		public float Yaw => U.NormalizeDegrees180(rotation.Y);
		public float Roll => U.NormalizeDegrees180(rotation.Z);*/
		public float Scale => scale.Single;
		public float X => translation.X;
		public float Y => translation.Y;
		public float Z => translation.Z;
		public Vector3 Translation => (Vector3)translation;
		public Quaternion Rotation { get { LoadRotation(out Quaternion q); return q; } }
		public Vector3 RotationAxis
		{
			get
			{
				Vector3 axis;
				float angle;
				Rotation.ToAxisAngle(out axis, out angle);

				if (angle < 0f)
					axis = -axis;
				angle = axis.LengthSquared;
				if (angle != 1f)
					axis /= (float)System.Math.Sqrt(angle);
				return axis;
			}
		}
		public float RotationAngle
		{
			get
			{
				Vector3 junk;
				float angle;
				Rotation.ToAxisAngle(out junk, out angle);
				return angle < 0f ? -angle : angle;
			}
		}
		public Vector4 AngleAxis { get { return Rotation.ToAxisAngle(); } }
		public Vector3 AngularVector
		{
			get
			{
				Vector3 axis;
				float angle;
				Rotation.ToAxisAngle(out axis, out angle);
				return axis * angle;
			}
		}
		public Matrix4 Matrix
		{
			get
			{
				return ((Transform)this).Matrix;
			}
		}
		public void LoadTranslation(out Vector3 V)
		{
			V.X = translation.X;
			V.Y = translation.Y;
			V.Z = translation.Z;
		}

		public void LoadScale(out float V)
		{
			V = scale.Single;
		}
		public void LoadRotation(out Quaternion R)
		{
			U.Quat16(out R, rotation.X, rotation.Y, rotation.Z);
		}

		public void LoadTransform(out Transform T)
		{
			LoadRotation(out T.rotation);
			LoadScale(out T.scale);
			LoadTranslation(out T.translation);
		}

	}
}

namespace Quad64 {
	using integer = System.Int16;
	using block = System.UInt64;
	using vec4 = Quad64.Vector4s;
	using vec3 = Quad64.Vector3s;

	[StructLayout(LayoutKind.Explicit, Size = sizeof(block))]
	public struct Vector4s
	#region copy/paste
		: IEquatable<vec4>, IEquatable<vec3>
	{
		[FieldOffset(sizeof(integer) * 3)]
		public integer X;
		[FieldOffset(sizeof(integer) * 2)]
		public integer Y;
		[FieldOffset(sizeof(integer) * 1)]
		public integer Z;
		[FieldOffset(sizeof(integer) * 0)]
		public integer W;
		[FieldOffset(0)]
		public block Packed;
		public integer this[int axis]
		{
			get
			{
				return 0 == (axis & 2) ? 0 == (axis & 1) ? X : Y : 0 == (axis & 1) ? Z : W;
			}
			set
			{
				if (0 == (axis & 2))
					if (0 == (axis & 1))
						X = value;
					else
						Y = value;
				else
					if (0 == (axis & 1))
					Z = value;
				else
					W = value;
			}
		}
		public bool Equals(vec4 other)
		{
			return Packed == other.Packed;
		}
		public bool Equals(vec3 other)
		{
			return W == 0 && X == other.X && Y == other.Y && Z == other.Z;
		}
		public static bool operator ==(vec4 L, vec4 R) { return L.Packed == R.Packed; }
		public static bool operator !=(vec4 L, vec4 R) { return L.Packed != R.Packed; }
		public static bool operator ==(vec4 L, block R) { return L.Packed == R; }
		public static bool operator !=(vec4 L, block R) { return L.Packed != R; }
		public static bool operator ==(block L, vec4 R) { return L == R.Packed; }
		public static bool operator !=(block L, vec4 R) { return L != R.Packed; }

		public static vec4 operator &(vec4 L, vec4 R) { return new vec4 { Packed = L.Packed & R.Packed, }; }
		public static vec4 operator |(vec4 L, vec4 R) { return new vec4 { Packed = L.Packed | R.Packed, }; }
		public static vec4 operator ^(vec4 L, vec4 R) { return new vec4 { Packed = L.Packed ^ R.Packed, }; }
		public static bool operator true(vec4 criteria) { return criteria.Packed != 0; }
		public static bool operator false(vec4 criteria) { return 0 == criteria.Packed; }

		public static vec4 operator +(vec4 L, vec4 R)
		{
			unchecked
			{
				return new vec4
				{
					X = (integer)(L.X + R.X),
					Y = (integer)(L.Y + R.Y),
					Z = (integer)(L.Z + R.Z),
					W = (integer)(L.W + R.W),
				};
			}
		}
		public static vec4 operator -(vec4 L, vec4 R)
		{
			unchecked
			{
				return new vec4
				{
					X = (integer)(L.X - R.X),
					Y = (integer)(L.Y - R.Y),
					Z = (integer)(L.Z - R.Z),
					W = (integer)(L.W - R.W),
				};
			}
		}
		public static vec4 operator *(vec4 L, vec4 R)
		{
			unchecked
			{
				return new vec4
				{
					X = (integer)(L.X * R.X),
					Y = (integer)(L.Y * R.Y),
					Z = (integer)(L.Z * R.Z),
					W = (integer)(L.W * R.W),
				};
			}
		}
		public static vec4 operator /(vec4 L, vec4 R)
		{
			unchecked
			{
				return new vec4
				{
					X = (integer)(L.X / R.X),
					Y = (integer)(L.Y / R.Y),
					Z = (integer)(L.Z / R.Z),
					W = (integer)(L.W / R.W),
				};
			}
		}
		public static vec4 operator *(vec4 L, integer R)
		{
			unchecked
			{
				return new vec4
				{
					X = (integer)(L.X * R),
					Y = (integer)(L.Y * R),
					Z = (integer)(L.Z * R),
					W = (integer)(L.W * R),
				};
			}
		}
		public static vec4 operator /(vec4 L, integer R)
		{
			unchecked
			{
				return new vec4
				{
					X = (integer)(L.X / R),
					Y = (integer)(L.Y / R),
					Z = (integer)(L.Z / R),
					W = (integer)(L.W / R),
				};
			}
		}
		public static Vector4 operator *(vec4 L, float R)
		{
			unchecked
			{
				return new Vector4
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
					W = (L.W * R),
				};
			}
		}
		public static Vector4 operator /(vec4 L, float R)
		{
			unchecked
			{
				return new Vector4
				{
					X = (L.X / R),
					Y = (L.Y / R),
					Z = (L.Z / R),
					W = (L.W / R),
				};
			}
		}
		public static Vector4 operator *(float R, vec4 L)
		{
			unchecked
			{
				return new Vector4
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
					W = (L.W * R),
				};
			}
		}
		public static Vector4 operator /(float R, vec4 L)
		{
			unchecked
			{
				return new Vector4
				{
					X = (integer)(R / L.X),
					Y = (integer)(R / L.Y),
					Z = (integer)(R / L.Z),
					W = (integer)(R / L.W),
				};
			}
		}
		public static Vector4d operator *(vec4 L, double R)
		{
			unchecked
			{
				return new Vector4d
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
					W = (L.W * R),
				};
			}
		}
		public static Vector4d operator /(vec4 L, double R)
		{
			unchecked
			{
				return new Vector4d
				{
					X = (L.X / R),
					Y = (L.Y / R),
					Z = (L.Z / R),
					W = (L.W / R),
				};
			}
		}
		public static Vector4d operator *(double R, vec4 L)
		{
			unchecked
			{
				return new Vector4d
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
					W = (L.W * R),
				};
			}
		}
		public static Vector4d operator /(double R, vec4 L)
		{
			unchecked
			{
				return new Vector4d
				{
					X = (integer)(R / L.X),
					Y = (integer)(R / L.Y),
					Z = (integer)(R / L.Z),
					W = (integer)(R / L.W),
				};
			}
		}
		public static explicit operator Vector3(vec4 value)
		{
			return new Vector3 { X = value.X, Y = value.Y, Z = value.Z, };
		}
		public static explicit operator Vector2(vec4 value)
		{
			return new Vector2 { X = value.X, Y = value.Y, };
		}
		public static explicit operator Vector4(vec4 value)
		{
			return new Vector4 { X = value.X, Y = value.Y, Z = value.Z, W = value.W, };
		}
		public static explicit operator Vector3d(vec4 value)
		{
			return new Vector3d { X = value.X, Y = value.Y, Z = value.Z, };
		}
		public static explicit operator Vector2d(vec4 value)
		{
			return new Vector2d { X = value.X, Y = value.Y, };
		}
		public static explicit operator Vector4d(vec4 value)
		{
			return new Vector4d { X = value.X, Y = value.Y, Z = value.Z, W = value.W, };
		}
		public static explicit operator vec3(vec4 value)
		{
			return new vec3 { X = value.X, Y = value.Y, Z = value.Z, };
		}
		#endregion
		public override int GetHashCode()
		{
			unchecked
			{
				uint B = (uint)(Packed >> 32);
				return (int)((uint)(Packed & uint.MaxValue) ^ (uint)((B >> 10) | (B << (32 - 10))));
			}
		}
		public static vec4 operator -(vec4 v) { unchecked { return new vec4 { X = (integer)(-v.X), Y = (integer)(-v.Y), Z = (integer)(-v.Z), W = (integer)(-v.Z), }; } }
		#region copy/paste
		public override bool Equals(object obj)
		{
			return obj is vec4 && Packed == ((vec4)obj).Packed;
		}
		public override string ToString()
		{
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			var Zs = X == Z ? Xs : Y == Z ? Ys : Z == 0 ? "0" : Z.ToString();
			var Ws = X == W ? Xs : Y == W ? Ys : Z == W ? Zs : W == 0 ? "0" : W.ToString();

			return string.Concat("[",
				string.Concat(Xs, ",", Ys, ","),
				string.Concat(Zs, ",", Ws, "]"));
		}

		public static explicit operator vec4(block value) {
			return new vec4 { Packed = value, };
		}
	}
	#endregion copy/paste
	[StructLayout(LayoutKind.Sequential, Size = sizeof(integer) * 3, Pack = sizeof(integer))]
	public struct Vector3s
	#region copy/paste
		: IEquatable<vec4>, IEquatable<vec3>
	{
		public integer X;
		public integer Y;
		public integer Z;
		public block Packed
		{
			get { return new vec4 { X = X, Y = Y, Z = Z, }.Packed; }
			set
			{
				var V4 = new vec4 { Packed = value, };
				this.X = V4.X;
				this.Y = V4.Y;
				this.Z = V4.Z;
			}
		}
		#endregion
		public static vec3 operator -(vec3 v) { unchecked { return new vec3 { X = (integer)(-v.X), Y = (integer)(-v.Y), Z = (integer)(-v.Z), }; } }

		#region copy/paste
		public integer this[int axis]
		{
			get
			{
				return 0 == (axis & 2) ? 0 == (axis & 1) ? X : Y : 0 == (axis & 1) ? Z : (integer)0;
			}
			set
			{
				if (0 == (axis & 2))
					if (0 == (axis & 1))
						X = value;
					else
						Y = value;
				else
					if (0 == (axis & 1))
					Z = value;
			}
		}
		public bool Equals(vec3 other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}
		public bool Equals(vec4 other)
		{
			return 0 == other.W && X == other.X && Y == other.Y && Z == other.Z;
		}
		public static bool operator ==(vec3 L, vec3 R) { return L.X == R.X && L.Y == R.Y && L.Z == R.Z; }
		public static bool operator !=(vec3 L, vec3 R) { return L.X != R.X || L.Y != R.Y || L.Z != R.Z; }
		public static bool operator ==(vec3 L, block R) { return L.Packed == R; }
		public static bool operator !=(vec3 L, block R) { return L.Packed != R; }
		public static bool operator ==(block L, vec3 R) { return L == R.Packed; }
		public static bool operator !=(block L, vec3 R) { return L != R.Packed; }

		public static vec3 operator &(vec3 L, vec3 R) { return new vec3 { Packed = L.Packed & R.Packed, }; }
		public static vec3 operator |(vec3 L, vec3 R) { return new vec3 { Packed = L.Packed | R.Packed, }; }
		public static vec3 operator ^(vec3 L, vec3 R) { return new vec3 { Packed = L.Packed ^ R.Packed, }; }
		public static bool operator true(vec3 criteria) { return criteria.X != 0 || criteria.Y != 0 || criteria.Z != 0; }
		public static bool operator false(vec3 criteria) { return 0 == criteria.X && 0 == criteria.Y && 0 == criteria.Z; }

		public static vec3 operator +(vec3 L, vec3 R)
		{
			unchecked
			{
				return new vec3
				{
					X = (integer)(L.X + R.X),
					Y = (integer)(L.Y + R.Y),
					Z = (integer)(L.Z + R.Z),
				};
			}
		}
		public static vec3 operator -(vec3 L, vec3 R)
		{
			unchecked
			{
				return new vec3
				{
					X = (integer)(L.X - R.X),
					Y = (integer)(L.Y - R.Y),
					Z = (integer)(L.Z - R.Z),
				};
			}
		}
		public static vec3 operator *(vec3 L, vec3 R)
		{
			unchecked
			{
				return new vec3
				{
					X = (integer)(L.X * R.X),
					Y = (integer)(L.Y * R.Y),
					Z = (integer)(L.Z * R.Z),
				};
			}
		}
		public static vec3 operator /(vec3 L, vec3 R)
		{
			unchecked
			{
				return new vec3
				{
					X = (integer)(L.X / R.X),
					Y = (integer)(L.Y / R.Y),
					Z = (integer)(L.Z / R.Z),
				};
			}
		}
		public static vec3 operator *(vec3 L, integer R)
		{
			unchecked
			{
				return new vec3
				{
					X = (integer)(L.X * R),
					Y = (integer)(L.Y * R),
					Z = (integer)(L.Z * R),
				};
			}
		}
		public static vec3 operator /(vec3 L, integer R)
		{
			unchecked
			{
				return new vec3
				{
					X = (integer)(L.X / R),
					Y = (integer)(L.Y / R),
					Z = (integer)(L.Z / R),
				};
			}
		}
		public static Vector3 operator *(vec3 L, float R)
		{
			unchecked
			{
				return new Vector3
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
				};
			}
		}
		public static Vector3 operator /(vec3 L, float R)
		{
			unchecked
			{
				return new Vector3
				{
					X = (L.X / R),
					Y = (L.Y / R),
					Z = (L.Z / R),
				};
			}
		}
		public static Vector3 operator *(float R, vec3 L)
		{
			unchecked
			{
				return new Vector3
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
				};
			}
		}
		public static Vector3 operator /(float R, vec3 L)
		{
			unchecked
			{
				return new Vector3
				{
					X = (integer)(R / L.X),
					Y = (integer)(R / L.Y),
					Z = (integer)(R / L.Z),
				};
			}
		}
		public static Vector3d operator *(vec3 L, double R)
		{
			unchecked
			{
				return new Vector3d
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
				};
			}
		}
		public static Vector3d operator /(vec3 L, double R)
		{
			unchecked
			{
				return new Vector3d
				{
					X = (L.X / R),
					Y = (L.Y / R),
					Z = (L.Z / R),
				};
			}
		}
		public static Vector3d operator *(double R, vec3 L)
		{
			unchecked
			{
				return new Vector3d
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
				};
			}
		}
		public static Vector3d operator /(double R, vec3 L)
		{
			unchecked
			{
				return new Vector3d
				{
					X = (integer)(R / L.X),
					Y = (integer)(R / L.Y),
					Z = (integer)(R / L.Z),
				};
			}
		}
		public static explicit operator Vector3(vec3 value)
		{
			return new Vector3 { X = value.X, Y = value.Y, Z = value.Z, };
		}
		public static explicit operator Vector2(vec3 value)
		{
			return new Vector2 { X = value.X, Y = value.Y, };
		}
		public static explicit operator Vector3d(vec3 value)
		{
			return new Vector3d { X = value.X, Y = value.Y, Z = value.Z, };
		}
		public static explicit operator Vector2d(vec3 value)
		{
			return new Vector2d { X = value.X, Y = value.Y, };
		}
		public static explicit operator vec4(vec3 value)
		{
			return new vec4 { X = value.X, Y = value.Y, Z = value.Z, };
		}
		public static explicit operator vec3(block value) { return new vec3 { Packed = value, }; }

		public override int GetHashCode()
		{
			return new vec4 { X = X, Y = Y, Z = Z, }.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is vec3 ? Packed == ((vec3)obj).Packed :
				(obj is vec4 && Packed == ((vec4)obj).Packed);
		}
		public override string ToString()
		{
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			var Zs = X == Z ? Xs : Y == Z ? Ys : Z == 0 ? "0" : Z.ToString();

			return string.Concat("[",
				string.Concat(Xs, ",", Ys, ","),
				Zs, "]");
		}

	}
	#endregion
}

namespace Quad64
{
	using integer = System.SByte;
	using block = System.UInt32;
	using vec4 = Quad64.Vector4c;
	using vec3 = Quad64.Vector3c;

	[StructLayout(LayoutKind.Explicit, Size = sizeof(block))]
	public struct Vector4c
	#region copy/paste
		: IEquatable<vec4>, IEquatable<vec3>
	{
		[FieldOffset(sizeof(integer) * 3)]
		public integer X;
		[FieldOffset(sizeof(integer) * 2)]
		public integer Y;
		[FieldOffset(sizeof(integer) * 1)]
		public integer Z;
		[FieldOffset(sizeof(integer) * 0)]
		public integer W;
		[FieldOffset(0)]
		public block Packed;
		public integer this[int axis]
		{
			get
			{
				return 0 == (axis & 2) ? 0 == (axis & 1) ? X : Y : 0 == (axis & 1) ? Z : W;
			}
			set
			{
				if (0 == (axis & 2))
					if (0 == (axis & 1))
						X = value;
					else
						Y = value;
				else
					if (0 == (axis & 1))
					Z = value;
				else
					W = value;
			}
		}
		public bool Equals(vec4 other)
		{
			return Packed == other.Packed;
		}
		public bool Equals(vec3 other)
		{
			return W == 0 && X == other.X && Y == other.Y && Z == other.Z;
		}
		public static bool operator ==(vec4 L, vec4 R) { return L.Packed == R.Packed; }
		public static bool operator !=(vec4 L, vec4 R) { return L.Packed != R.Packed; }
		public static bool operator ==(vec4 L, block R) { return L.Packed == R; }
		public static bool operator !=(vec4 L, block R) { return L.Packed != R; }
		public static bool operator ==(block L, vec4 R) { return L == R.Packed; }
		public static bool operator !=(block L, vec4 R) { return L != R.Packed; }

		public static vec4 operator &(vec4 L, vec4 R) { return new vec4 { Packed = L.Packed & R.Packed, }; }
		public static vec4 operator |(vec4 L, vec4 R) { return new vec4 { Packed = L.Packed | R.Packed, }; }
		public static vec4 operator ^(vec4 L, vec4 R) { return new vec4 { Packed = L.Packed ^ R.Packed, }; }
		public static bool operator true(vec4 criteria) { return criteria.Packed != 0; }
		public static bool operator false(vec4 criteria) { return 0 == criteria.Packed; }

		public static vec4 operator +(vec4 L, vec4 R)
		{
			unchecked
			{
				return new vec4
				{
					X = (integer)(L.X + R.X),
					Y = (integer)(L.Y + R.Y),
					Z = (integer)(L.Z + R.Z),
					W = (integer)(L.W + R.W),
				};
			}
		}
		public static vec4 operator -(vec4 L, vec4 R)
		{
			unchecked
			{
				return new vec4
				{
					X = (integer)(L.X - R.X),
					Y = (integer)(L.Y - R.Y),
					Z = (integer)(L.Z - R.Z),
					W = (integer)(L.W - R.W),
				};
			}
		}
		public static vec4 operator *(vec4 L, vec4 R)
		{
			unchecked
			{
				return new vec4
				{
					X = (integer)(L.X * R.X),
					Y = (integer)(L.Y * R.Y),
					Z = (integer)(L.Z * R.Z),
					W = (integer)(L.W * R.W),
				};
			}
		}
		public static vec4 operator /(vec4 L, vec4 R)
		{
			unchecked
			{
				return new vec4
				{
					X = (integer)(L.X / R.X),
					Y = (integer)(L.Y / R.Y),
					Z = (integer)(L.Z / R.Z),
					W = (integer)(L.W / R.W),
				};
			}
		}
		public static vec4 operator *(vec4 L, integer R)
		{
			unchecked
			{
				return new vec4
				{
					X = (integer)(L.X * R),
					Y = (integer)(L.Y * R),
					Z = (integer)(L.Z * R),
					W = (integer)(L.W * R),
				};
			}
		}
		public static vec4 operator /(vec4 L, integer R)
		{
			unchecked
			{
				return new vec4
				{
					X = (integer)(L.X / R),
					Y = (integer)(L.Y / R),
					Z = (integer)(L.Z / R),
					W = (integer)(L.W / R),
				};
			}
		}
		public static Vector4 operator *(vec4 L, float R)
		{
			unchecked
			{
				return new Vector4
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
					W = (L.W * R),
				};
			}
		}
		public static Vector4 operator /(vec4 L, float R)
		{
			unchecked
			{
				return new Vector4
				{
					X = (L.X / R),
					Y = (L.Y / R),
					Z = (L.Z / R),
					W = (L.W / R),
				};
			}
		}
		public static Vector4 operator *(float R, vec4 L)
		{
			unchecked
			{
				return new Vector4
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
					W = (L.W * R),
				};
			}
		}
		public static Vector4 operator /(float R, vec4 L)
		{
			unchecked
			{
				return new Vector4
				{
					X = (integer)(R / L.X),
					Y = (integer)(R / L.Y),
					Z = (integer)(R / L.Z),
					W = (integer)(R / L.W),
				};
			}
		}
		public static Vector4d operator *(vec4 L, double R)
		{
			unchecked
			{
				return new Vector4d
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
					W = (L.W * R),
				};
			}
		}
		public static Vector4d operator /(vec4 L, double R)
		{
			unchecked
			{
				return new Vector4d
				{
					X = (L.X / R),
					Y = (L.Y / R),
					Z = (L.Z / R),
					W = (L.W / R),
				};
			}
		}
		public static Vector4d operator *(double R, vec4 L)
		{
			unchecked
			{
				return new Vector4d
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
					W = (L.W * R),
				};
			}
		}
		public static Vector4d operator /(double R, vec4 L)
		{
			unchecked
			{
				return new Vector4d
				{
					X = (integer)(R / L.X),
					Y = (integer)(R / L.Y),
					Z = (integer)(R / L.Z),
					W = (integer)(R / L.W),
				};
			}
		}
		public static explicit operator Vector3(vec4 value)
		{
			return new Vector3 { X = value.X, Y = value.Y, Z = value.Z, };
		}
		public static explicit operator Vector2(vec4 value)
		{
			return new Vector2 { X = value.X, Y = value.Y, };
		}
		public static explicit operator Vector4(vec4 value)
		{
			return new Vector4 { X = value.X, Y = value.Y, Z = value.Z, W = value.W, };
		}
		public static explicit operator Vector3d(vec4 value)
		{
			return new Vector3d { X = value.X, Y = value.Y, Z = value.Z, };
		}
		public static explicit operator Vector2d(vec4 value)
		{
			return new Vector2d { X = value.X, Y = value.Y, };
		}
		public static explicit operator Vector4d(vec4 value)
		{
			return new Vector4d { X = value.X, Y = value.Y, Z = value.Z, W = value.W, };
		}
		public static explicit operator vec3(vec4 value)
		{
			return new vec3 { X = value.X, Y = value.Y, Z = value.Z, };
		}
		#endregion
		public override int GetHashCode()
		{
			unchecked
			{
				return (int)Packed;
			}
		}
		public static vec4 operator -(vec4 v) { unchecked { return new vec4 { X = (integer)(-v.X), Y = (integer)(-v.Y), Z = (integer)(-v.Z), W = (integer)(-v.Z), }; } }
		#region copy/paste
		public override bool Equals(object obj)
		{
			return obj is vec4 && Packed == ((vec4)obj).Packed;
		}
		public override string ToString()
		{
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			var Zs = X == Z ? Xs : Y == Z ? Ys : Z == 0 ? "0" : Z.ToString();
			var Ws = X == W ? Xs : Y == W ? Ys : Z == W ? Zs : W == 0 ? "0" : W.ToString();

			return string.Concat("[",
				string.Concat(Xs, ",", Ys, ","),
				string.Concat(Zs, ",", Ws, "]"));
		}

		public static explicit operator vec4(block value)
		{
			return new vec4 { Packed = value, };
		}
	}
	#endregion copy/paste
	[StructLayout(LayoutKind.Sequential, Size = sizeof(integer) * 3, Pack = sizeof(integer))]
	public struct Vector3c
	#region copy/paste
		: IEquatable<vec4>, IEquatable<vec3>
	{
		public integer X;
		public integer Y;
		public integer Z;
		#endregion
		public static vec3 operator -(vec3 v) { unchecked { return new vec3 { X = (integer)(-v.X), Y = (integer)(-v.Y), Z = (integer)(-v.Z), }; } }
		#region copy/paste
		public block Packed
		{
			get { return new vec4 { X = X, Y = Y, Z = Z, }.Packed; }
			set
			{
				var V4 = new vec4 { Packed = value, };
				this.X = V4.X;
				this.Y = V4.Y;
				this.Z = V4.Z;
			}
		}
		public integer this[int axis]
		{
			get
			{
				return 0 == (axis & 2) ? 0 == (axis & 1) ? X : Y : 0 == (axis & 1) ? Z : (integer)0;
			}
			set
			{
				if (0 == (axis & 2))
					if (0 == (axis & 1))
						X = value;
					else
						Y = value;
				else
					if (0 == (axis & 1))
					Z = value;
			}
		}
		public bool Equals(vec3 other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}
		public bool Equals(vec4 other)
		{
			return 0 == other.W && X == other.X && Y == other.Y && Z == other.Z;
		}
		public static bool operator ==(vec3 L, vec3 R) { return L.X == R.X && L.Y == R.Y && L.Z == R.Z; }
		public static bool operator !=(vec3 L, vec3 R) { return L.X != R.X || L.Y != R.Y || L.Z != R.Z; }
		public static bool operator ==(vec3 L, block R) { return L.Packed == R; }
		public static bool operator !=(vec3 L, block R) { return L.Packed != R; }
		public static bool operator ==(block L, vec3 R) { return L == R.Packed; }
		public static bool operator !=(block L, vec3 R) { return L != R.Packed; }

		public static vec3 operator &(vec3 L, vec3 R) { return new vec3 { Packed = L.Packed & R.Packed, }; }
		public static vec3 operator |(vec3 L, vec3 R) { return new vec3 { Packed = L.Packed | R.Packed, }; }
		public static vec3 operator ^(vec3 L, vec3 R) { return new vec3 { Packed = L.Packed ^ R.Packed, }; }
		public static bool operator true(vec3 criteria) { return criteria.X != 0 || criteria.Y != 0 || criteria.Z != 0; }
		public static bool operator false(vec3 criteria) { return 0 == criteria.X && 0 == criteria.Y && 0 == criteria.Z; }

		public static vec3 operator +(vec3 L, vec3 R)
		{
			unchecked
			{
				return new vec3
				{
					X = (integer)(L.X + R.X),
					Y = (integer)(L.Y + R.Y),
					Z = (integer)(L.Z + R.Z),
				};
			}
		}
		public static vec3 operator -(vec3 L, vec3 R)
		{
			unchecked
			{
				return new vec3
				{
					X = (integer)(L.X - R.X),
					Y = (integer)(L.Y - R.Y),
					Z = (integer)(L.Z - R.Z),
				};
			}
		}
		public static vec3 operator *(vec3 L, vec3 R)
		{
			unchecked
			{
				return new vec3
				{
					X = (integer)(L.X * R.X),
					Y = (integer)(L.Y * R.Y),
					Z = (integer)(L.Z * R.Z),
				};
			}
		}
		public static vec3 operator /(vec3 L, vec3 R)
		{
			unchecked
			{
				return new vec3
				{
					X = (integer)(L.X / R.X),
					Y = (integer)(L.Y / R.Y),
					Z = (integer)(L.Z / R.Z),
				};
			}
		}
		public static vec3 operator *(vec3 L, integer R)
		{
			unchecked
			{
				return new vec3
				{
					X = (integer)(L.X * R),
					Y = (integer)(L.Y * R),
					Z = (integer)(L.Z * R),
				};
			}
		}
		public static vec3 operator /(vec3 L, integer R)
		{
			unchecked
			{
				return new vec3
				{
					X = (integer)(L.X / R),
					Y = (integer)(L.Y / R),
					Z = (integer)(L.Z / R),
				};
			}
		}
		public static Vector3 operator *(vec3 L, float R)
		{
			unchecked
			{
				return new Vector3
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
				};
			}
		}
		public static Vector3 operator /(vec3 L, float R)
		{
			unchecked
			{
				return new Vector3
				{
					X = (L.X / R),
					Y = (L.Y / R),
					Z = (L.Z / R),
				};
			}
		}
		public static Vector3 operator *(float R, vec3 L)
		{
			unchecked
			{
				return new Vector3
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
				};
			}
		}
		public static Vector3 operator /(float R, vec3 L)
		{
			unchecked
			{
				return new Vector3
				{
					X = (integer)(R / L.X),
					Y = (integer)(R / L.Y),
					Z = (integer)(R / L.Z),
				};
			}
		}
		public static Vector3d operator *(vec3 L, double R)
		{
			unchecked
			{
				return new Vector3d
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
				};
			}
		}
		public static Vector3d operator /(vec3 L, double R)
		{
			unchecked
			{
				return new Vector3d
				{
					X = (L.X / R),
					Y = (L.Y / R),
					Z = (L.Z / R),
				};
			}
		}
		public static Vector3d operator *(double R, vec3 L)
		{
			unchecked
			{
				return new Vector3d
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
				};
			}
		}
		public static Vector3d operator /(double R, vec3 L)
		{
			unchecked
			{
				return new Vector3d
				{
					X = (integer)(R / L.X),
					Y = (integer)(R / L.Y),
					Z = (integer)(R / L.Z),
				};
			}
		}
		public static explicit operator Vector3(vec3 value)
		{
			return new Vector3 { X = value.X, Y = value.Y, Z = value.Z, };
		}
		public static explicit operator Vector2(vec3 value)
		{
			return new Vector2 { X = value.X, Y = value.Y, };
		}
		public static explicit operator Vector3d(vec3 value)
		{
			return new Vector3d { X = value.X, Y = value.Y, Z = value.Z, };
		}
		public static explicit operator Vector2d(vec3 value)
		{
			return new Vector2d { X = value.X, Y = value.Y, };
		}
		public static explicit operator vec4(vec3 value)
		{
			return new vec4 { X = value.X, Y = value.Y, Z = value.Z, };
		}
		public static explicit operator vec3(block value) { return new vec3 { Packed = value, }; }

		public override int GetHashCode()
		{
			return new vec4 { X = X, Y = Y, Z = Z, }.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is vec3 ? Packed == ((vec3)obj).Packed :
				(obj is vec4 && Packed == ((vec4)obj).Packed);
		}
		public override string ToString()
		{
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			var Zs = X == Z ? Xs : Y == Z ? Ys : Z == 0 ? "0" : Z.ToString();

			return string.Concat("[",
				string.Concat(Xs, ",", Ys, ","),
				Zs, "]");
		}

	}
	#endregion
}
namespace Quad64
{
	using integer = System.Byte;
	using block = System.UInt32;
	using vec4 = Quad64.Vector4b;
	using vec3 = Quad64.Vector3b;

	[StructLayout(LayoutKind.Explicit, Size = sizeof(block))]
	public struct Vector4b
	#region copy/paste
		: IEquatable<vec4>, IEquatable<vec3>
	{
		[FieldOffset(sizeof(integer) * 3)]
		public integer X;
		[FieldOffset(sizeof(integer) * 2)]
		public integer Y;
		[FieldOffset(sizeof(integer) * 1)]
		public integer Z;
		[FieldOffset(sizeof(integer) * 0)]
		public integer W;
		[FieldOffset(0)]
		public block Packed;
		public integer this[int axis]
		{
			get
			{
				return 0 == (axis & 2) ? 0 == (axis & 1) ? X : Y : 0 == (axis & 1) ? Z : W;
			}
			set
			{
				if (0 == (axis & 2))
					if (0 == (axis & 1))
						X = value;
					else
						Y = value;
				else
					if (0 == (axis & 1))
					Z = value;
				else
					W = value;
			}
		}
		public bool Equals(vec4 other)
		{
			return Packed == other.Packed;
		}
		public bool Equals(vec3 other)
		{
			return W == 0 && X == other.X && Y == other.Y && Z == other.Z;
		}
		public static bool operator ==(vec4 L, vec4 R) { return L.Packed == R.Packed; }
		public static bool operator !=(vec4 L, vec4 R) { return L.Packed != R.Packed; }
		public static bool operator ==(vec4 L, block R) { return L.Packed == R; }
		public static bool operator !=(vec4 L, block R) { return L.Packed != R; }
		public static bool operator ==(block L, vec4 R) { return L == R.Packed; }
		public static bool operator !=(block L, vec4 R) { return L != R.Packed; }

		public static vec4 operator &(vec4 L, vec4 R) { return new vec4 { Packed = L.Packed & R.Packed, }; }
		public static vec4 operator |(vec4 L, vec4 R) { return new vec4 { Packed = L.Packed | R.Packed, }; }
		public static vec4 operator ^(vec4 L, vec4 R) { return new vec4 { Packed = L.Packed ^ R.Packed, }; }
		public static bool operator true(vec4 criteria) { return criteria.Packed != 0; }
		public static bool operator false(vec4 criteria) { return 0 == criteria.Packed; }

		public static vec4 operator +(vec4 L, vec4 R)
		{
			unchecked
			{
				return new vec4
				{
					X = (integer)(L.X + R.X),
					Y = (integer)(L.Y + R.Y),
					Z = (integer)(L.Z + R.Z),
					W = (integer)(L.W + R.W),
				};
			}
		}
		public static vec4 operator -(vec4 L, vec4 R)
		{
			unchecked
			{
				return new vec4
				{
					X = (integer)(L.X - R.X),
					Y = (integer)(L.Y - R.Y),
					Z = (integer)(L.Z - R.Z),
					W = (integer)(L.W - R.W),
				};
			}
		}
		public static vec4 operator *(vec4 L, vec4 R)
		{
			unchecked
			{
				return new vec4
				{
					X = (integer)(L.X * R.X),
					Y = (integer)(L.Y * R.Y),
					Z = (integer)(L.Z * R.Z),
					W = (integer)(L.W * R.W),
				};
			}
		}
		public static vec4 operator /(vec4 L, vec4 R)
		{
			unchecked
			{
				return new vec4
				{
					X = (integer)(L.X / R.X),
					Y = (integer)(L.Y / R.Y),
					Z = (integer)(L.Z / R.Z),
					W = (integer)(L.W / R.W),
				};
			}
		}
		public static vec4 operator *(vec4 L, integer R)
		{
			unchecked
			{
				return new vec4
				{
					X = (integer)(L.X * R),
					Y = (integer)(L.Y * R),
					Z = (integer)(L.Z * R),
					W = (integer)(L.W * R),
				};
			}
		}
		public static vec4 operator /(vec4 L, integer R)
		{
			unchecked
			{
				return new vec4
				{
					X = (integer)(L.X / R),
					Y = (integer)(L.Y / R),
					Z = (integer)(L.Z / R),
					W = (integer)(L.W / R),
				};
			}
		}
		public static Vector4 operator *(vec4 L, float R)
		{
			unchecked
			{
				return new Vector4
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
					W = (L.W * R),
				};
			}
		}
		public static Vector4 operator /(vec4 L, float R)
		{
			unchecked
			{
				return new Vector4
				{
					X = (L.X / R),
					Y = (L.Y / R),
					Z = (L.Z / R),
					W = (L.W / R),
				};
			}
		}
		public static Vector4 operator *(float R, vec4 L)
		{
			unchecked
			{
				return new Vector4
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
					W = (L.W * R),
				};
			}
		}
		public static Vector4 operator /(float R, vec4 L)
		{
			unchecked
			{
				return new Vector4
				{
					X = (integer)(R / L.X),
					Y = (integer)(R / L.Y),
					Z = (integer)(R / L.Z),
					W = (integer)(R / L.W),
				};
			}
		}
		public static Vector4d operator *(vec4 L, double R)
		{
			unchecked
			{
				return new Vector4d
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
					W = (L.W * R),
				};
			}
		}
		public static Vector4d operator /(vec4 L, double R)
		{
			unchecked
			{
				return new Vector4d
				{
					X = (L.X / R),
					Y = (L.Y / R),
					Z = (L.Z / R),
					W = (L.W / R),
				};
			}
		}
		public static Vector4d operator *(double R, vec4 L)
		{
			unchecked
			{
				return new Vector4d
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
					W = (L.W * R),
				};
			}
		}
		public static Vector4d operator /(double R, vec4 L)
		{
			unchecked
			{
				return new Vector4d
				{
					X = (integer)(R / L.X),
					Y = (integer)(R / L.Y),
					Z = (integer)(R / L.Z),
					W = (integer)(R / L.W),
				};
			}
		}
		public static explicit operator Vector3(vec4 value)
		{
			return new Vector3 { X = value.X, Y = value.Y, Z = value.Z, };
		}
		public static explicit operator Vector2(vec4 value)
		{
			return new Vector2 { X = value.X, Y = value.Y, };
		}
		public static explicit operator Vector4(vec4 value)
		{
			return new Vector4 { X = value.X, Y = value.Y, Z = value.Z, W = value.W, };
		}
		public static explicit operator Vector3d(vec4 value)
		{
			return new Vector3d { X = value.X, Y = value.Y, Z = value.Z, };
		}
		public static explicit operator Vector2d(vec4 value)
		{
			return new Vector2d { X = value.X, Y = value.Y, };
		}
		public static explicit operator Vector4d(vec4 value)
		{
			return new Vector4d { X = value.X, Y = value.Y, Z = value.Z, W = value.W, };
		}
		public static explicit operator vec3(vec4 value)
		{
			return new vec3 { X = value.X, Y = value.Y, Z = value.Z, };
		}
		#endregion
		public override int GetHashCode()
		{
			unchecked
			{
				return (int)Packed;
			}
		}
		#region copy/paste
		public override bool Equals(object obj)
		{
			return obj is vec4 && Packed == ((vec4)obj).Packed;
		}
		public override string ToString()
		{
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			var Zs = X == Z ? Xs : Y == Z ? Ys : Z == 0 ? "0" : Z.ToString();
			var Ws = X == W ? Xs : Y == W ? Ys : Z == W ? Zs : W == 0 ? "0" : W.ToString();

			return string.Concat("[",
				string.Concat(Xs, ",", Ys, ","),
				string.Concat(Zs, ",", Ws, "]"));
		}

		public static explicit operator vec4(block value)
		{
			return new vec4 { Packed = value, };
		}
	}
	#endregion copy/paste
	[StructLayout(LayoutKind.Sequential, Size = sizeof(integer) * 3, Pack = sizeof(integer))]
	public struct Vector3b
	#region copy/paste
		: IEquatable<vec4>, IEquatable<vec3>
	{
		public integer X;
		public integer Y;
		public integer Z;
		public block Packed
		{
			get { return new vec4 { X = X, Y = Y, Z = Z, }.Packed; }
			set
			{
				var V4 = new vec4 { Packed = value, };
				this.X = V4.X;
				this.Y = V4.Y;
				this.Z = V4.Z;
			}
		}
		public integer this[int axis]
		{
			get
			{
				return 0 == (axis & 2) ? 0 == (axis & 1) ? X : Y : 0 == (axis & 1) ? Z : (integer)0;
			}
			set
			{
				if (0 == (axis & 2))
					if (0 == (axis & 1))
						X = value;
					else
						Y = value;
				else
					if (0 == (axis & 1))
					Z = value;
			}
		}
		public bool Equals(vec3 other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}
		public bool Equals(vec4 other)
		{
			return 0 == other.W && X == other.X && Y == other.Y && Z == other.Z;
		}
		public static bool operator ==(vec3 L, vec3 R) { return L.X == R.X && L.Y == R.Y && L.Z == R.Z; }
		public static bool operator !=(vec3 L, vec3 R) { return L.X != R.X || L.Y != R.Y || L.Z != R.Z; }
		public static bool operator ==(vec3 L, block R) { return L.Packed == R; }
		public static bool operator !=(vec3 L, block R) { return L.Packed != R; }
		public static bool operator ==(block L, vec3 R) { return L == R.Packed; }
		public static bool operator !=(block L, vec3 R) { return L != R.Packed; }

		public static vec3 operator &(vec3 L, vec3 R) { return new vec3 { Packed = L.Packed & R.Packed, }; }
		public static vec3 operator |(vec3 L, vec3 R) { return new vec3 { Packed = L.Packed | R.Packed, }; }
		public static vec3 operator ^(vec3 L, vec3 R) { return new vec3 { Packed = L.Packed ^ R.Packed, }; }
		public static bool operator true(vec3 criteria) { return criteria.X != 0 || criteria.Y != 0 || criteria.Z != 0; }
		public static bool operator false(vec3 criteria) { return 0 == criteria.X && 0 == criteria.Y && 0 == criteria.Z; }

		public static vec3 operator +(vec3 L, vec3 R)
		{
			unchecked
			{
				return new vec3
				{
					X = (integer)(L.X + R.X),
					Y = (integer)(L.Y + R.Y),
					Z = (integer)(L.Z + R.Z),
				};
			}
		}
		public static vec3 operator -(vec3 L, vec3 R)
		{
			unchecked
			{
				return new vec3
				{
					X = (integer)(L.X - R.X),
					Y = (integer)(L.Y - R.Y),
					Z = (integer)(L.Z - R.Z),
				};
			}
		}
		public static vec3 operator *(vec3 L, vec3 R)
		{
			unchecked
			{
				return new vec3
				{
					X = (integer)(L.X * R.X),
					Y = (integer)(L.Y * R.Y),
					Z = (integer)(L.Z * R.Z),
				};
			}
		}
		public static vec3 operator /(vec3 L, vec3 R)
		{
			unchecked
			{
				return new vec3
				{
					X = (integer)(L.X / R.X),
					Y = (integer)(L.Y / R.Y),
					Z = (integer)(L.Z / R.Z),
				};
			}
		}
		public static vec3 operator *(vec3 L, integer R)
		{
			unchecked
			{
				return new vec3
				{
					X = (integer)(L.X * R),
					Y = (integer)(L.Y * R),
					Z = (integer)(L.Z * R),
				};
			}
		}
		public static vec3 operator /(vec3 L, integer R)
		{
			unchecked
			{
				return new vec3
				{
					X = (integer)(L.X / R),
					Y = (integer)(L.Y / R),
					Z = (integer)(L.Z / R),
				};
			}
		}
		public static Vector3 operator *(vec3 L, float R)
		{
			unchecked
			{
				return new Vector3
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
				};
			}
		}
		public static Vector3 operator /(vec3 L, float R)
		{
			unchecked
			{
				return new Vector3
				{
					X = (L.X / R),
					Y = (L.Y / R),
					Z = (L.Z / R),
				};
			}
		}
		public static Vector3 operator *(float R, vec3 L)
		{
			unchecked
			{
				return new Vector3
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
				};
			}
		}
		public static Vector3 operator /(float R, vec3 L)
		{
			unchecked
			{
				return new Vector3
				{
					X = (integer)(R / L.X),
					Y = (integer)(R / L.Y),
					Z = (integer)(R / L.Z),
				};
			}
		}
		public static Vector3d operator *(vec3 L, double R)
		{
			unchecked
			{
				return new Vector3d
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
				};
			}
		}
		public static Vector3d operator /(vec3 L, double R)
		{
			unchecked
			{
				return new Vector3d
				{
					X = (L.X / R),
					Y = (L.Y / R),
					Z = (L.Z / R),
				};
			}
		}
		public static Vector3d operator *(double R, vec3 L)
		{
			unchecked
			{
				return new Vector3d
				{
					X = (L.X * R),
					Y = (L.Y * R),
					Z = (L.Z * R),
				};
			}
		}
		public static Vector3d operator /(double R, vec3 L)
		{
			unchecked
			{
				return new Vector3d
				{
					X = (integer)(R / L.X),
					Y = (integer)(R / L.Y),
					Z = (integer)(R / L.Z),
				};
			}
		}
		public static explicit operator Vector3(vec3 value)
		{
			return new Vector3 { X = value.X, Y = value.Y, Z = value.Z, };
		}
		public static explicit operator Vector2(vec3 value)
		{
			return new Vector2 { X = value.X, Y = value.Y, };
		}
		public static explicit operator Vector3d(vec3 value)
		{
			return new Vector3d { X = value.X, Y = value.Y, Z = value.Z, };
		}
		public static explicit operator Vector2d(vec3 value)
		{
			return new Vector2d { X = value.X, Y = value.Y, };
		}
		public static explicit operator vec4(vec3 value)
		{
			return new vec4 { X = value.X, Y = value.Y, Z = value.Z, };
		}
		public static explicit operator vec3(block value) { return new vec3 { Packed = value, }; }

		public override int GetHashCode()
		{
			return new vec4 { X = X, Y = Y, Z = Z, }.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is vec3 ? Packed == ((vec3)obj).Packed :
				(obj is vec4 && Packed == ((vec4)obj).Packed);
		}
		public override string ToString()
		{
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			var Zs = X == Z ? Xs : Y == Z ? Ys : Z == 0 ? "0" : Z.ToString();

			return string.Concat("[",
				string.Concat(Xs, ",", Ys, ","),
				Zs, "]");
		}

	}
	#endregion
}