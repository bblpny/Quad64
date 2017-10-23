using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quad64
{
	public interface ICameraMode
	{
		// invoked prior to any other command once per usage.
		void Start(ref CameraData CameraData);
		// invoked when mouse is reset.
		void Reset(ref CameraData CameraData);
		// invoked when the mouse moves while holding the pan button.
		void Pan(ref CameraData CameraData);
		// invoked when the mouse moves while holding the rotate button.
		void Rotate(ref CameraData CameraData);
		// invoked when the scroll wheel changes.
		void Scroll(ref CameraData CameraData);
		// invoked when the selection changed.
		void Select(ref CameraData CameraData);
		// invoked when the camera data's transform mismatches that of previously calculated transform.
		// requests you to populate view with a transformation matrix.
		//
		// the view matrix should not be inversed. rather it should be that of an object the camera will view out of.
		void Update(ref CameraData CameraData, out Matrix4 View);
		// called prior to the camera mode being switched (if Start was invoked)
		void End(ref CameraData CameraData);
	}

	public struct CursorPoint : IEquatable<CursorPoint>
	{
		public int X, Y;
		public int this[int i] { get => (0 == (1 & i)) ? X : Y; set { if (0 == (1 & i)) X = value; else Y = value; } }
		public static bool operator == (CursorPoint L, CursorPoint R)
		{
			return L.X == R.X && L.Y == R.Y;
		}
		public static bool operator !=(CursorPoint L, CursorPoint R)
		{
			return L.X != R.X || L.Y != R.Y;
		}
		public static CursorPoint operator -(CursorPoint X)
		{
			return new CursorPoint { X = -X.X, Y = -X.Y, };
		}
		public static CursorPoint operator -(CursorPoint L, CursorPoint R)
		{
			return new CursorPoint { X = L.X - R.X, Y = L.Y - R.Y, };
		}
		public static CursorPoint operator +(CursorPoint L, CursorPoint R)
		{
			return new CursorPoint { X = L.X + R.X, Y = L.Y + R.Y, };
		}
		public static bool operator true(CursorPoint L) { return 0 != L.X || 0 !=L.Y; }
		public static bool operator false(CursorPoint L) { return 0 == L.X && 0 == L.Y; }
		public static CursorPoint operator &(CursorPoint L, CursorPoint R)
		{
			return (L.X == 0 && L.Y == 0) ? L : R;
		}
		public static CursorPoint operator |(CursorPoint L, CursorPoint R)
		{
			return (R.X == 0 && R.Y == 0) ? L : R;
		}
		public bool Equals(CursorPoint other) { return X == other.X && Y == other.Y; }
		public override int GetHashCode()
		{
			return X ^ ((Y << 16) | ((Y >> 16) & short.MaxValue));
		}
		public override bool Equals(object obj)
		{
			return obj is CursorPoint && Equals((CursorPoint)obj);
		}
		public override string ToString()
		{
			return string.Concat("[", X, ",", Y, "]");
		}
		public static Vector2 operator *(float Scale, CursorPoint Point)
		{
			return new Vector2 { X = Point.X * Scale, Y = Point.Y * Scale, };
		}
		public static Vector2d operator *(double Scale, CursorPoint Point)
		{
			return new Vector2d { X = Point.X * Scale, Y = Point.Y * Scale, };
		}
		public static Vector2 operator *(CursorPoint Point, float Scale)
		{
			return new Vector2 { X = Point.X * Scale, Y = Point.Y * Scale, };
		}
		public static Vector2d operator *(CursorPoint Point, double Scale)
		{
			return new Vector2d { X = Point.X * Scale, Y = Point.Y * Scale, };
		}
		public static Vector2 operator *(Vector2 Scale, CursorPoint Point)
		{
			return new Vector2 { X = Point.X * Scale.X, Y = Point.Y * Scale.Y, };
		}
		public static Vector2d operator *(Vector2d Scale, CursorPoint Point)
		{
			return new Vector2d { X = Point.X * Scale.X, Y = Point.Y * Scale.Y, };
		}
		public static Vector2 operator *(CursorPoint Point, Vector2 Scale)
		{
			return new Vector2 { X = Point.X * Scale.X, Y = Point.Y * Scale.Y, };
		}
		public static Vector2d operator *(CursorPoint Point, Vector2d Scale)
		{
			return new Vector2d { X = Point.X * Scale.X, Y = Point.Y * Scale.Y, };
		}
		public static Vector2 operator +(Vector2 Scale, CursorPoint Point)
		{
			return new Vector2 { X = Point.X + Scale.X, Y = Point.Y + Scale.Y, };
		}
		public static Vector2d operator +(Vector2d Scale, CursorPoint Point)
		{
			return new Vector2d { X = Point.X + Scale.X, Y = Point.Y + Scale.Y, };
		}
		public static Vector2 operator +(CursorPoint Point, Vector2 Scale)
		{
			return new Vector2 { X = Point.X + Scale.X, Y = Point.Y + Scale.Y, };
		}
		public static Vector2d operator +(CursorPoint Point, Vector2d Scale)
		{
			return new Vector2d { X = Point.X + Scale.X, Y = Point.Y + Scale.Y, };
		}
		public static Vector2 operator -(Vector2 Scale, CursorPoint Point)
		{
			return new Vector2 { X = Point.X - Scale.X, Y = Point.Y - Scale.Y, };
		}
		public static Vector2d operator -(Vector2d Scale, CursorPoint Point)
		{
			return new Vector2d { X = Point.X - Scale.X, Y = Point.Y - Scale.Y, };
		}
		public static Vector2 operator -(CursorPoint Point, Vector2 Scale)
		{
			return new Vector2 { X = Point.X - Scale.X, Y = Point.Y - Scale.Y, };
		}
		public static Vector2d operator-(CursorPoint Point, Vector2d Scale)
		{
			return new Vector2d { X = Point.X - Scale.X, Y = Point.Y - Scale.Y, };
		}
		public static Vector2 operator /(Vector2 Scale, CursorPoint Point)
		{
			return new Vector2 { X = Point.X / Scale.X, Y = Point.Y / Scale.Y, };
		}
		public static Vector2d operator /(Vector2d Scale, CursorPoint Point)
		{
			return new Vector2d { X = Point.X / Scale.X, Y = Point.Y / Scale.Y, };
		}
		public static Vector2 operator /(CursorPoint Point, Vector2 Scale)
		{
			return new Vector2 { X = Point.X / Scale.X, Y = Point.Y / Scale.Y, };
		}
		public static Vector2d operator /(CursorPoint Point, Vector2d Scale)
		{
			return new Vector2d { X = Point.X / Scale.X, Y = Point.Y / Scale.Y, };
		}
		public static Vector2 operator /(CursorPoint Point, float Scale)
		{
			return new Vector2 { X = Point.X / Scale, Y = Point.Y / Scale, };
		}
		public static Vector2d operator /(CursorPoint Point, double Scale)
		{
			return new Vector2d { X = Point.X / Scale, Y = Point.Y / Scale, };
		}
		public static explicit operator Vector2d(CursorPoint p)
		{
			return new Vector2d { X = p.X, Y = p.Y, };
		}
		public static explicit operator Vector2(CursorPoint p)
		{
			return new Vector2 { X = p.X, Y = p.Y, };
		}
	}
	public struct CameraData
	{
		public CursorPoint Cursor, CursorDelta;
		public double RadiansPitch, RadiansYaw, Scroll;
		public Transform Transform;
		public Object3D Selection;
		public Vector3 LookTarget;
		public bool Hint;
		public override int GetHashCode()
		{
			return Transform.InverseTransformPosition(LookTarget).GetHashCode() ^
				Transform.GetHashCode() ^
				(RadiansPitch.GetHashCode() ^ (RadiansYaw.GetHashCode()) * 5) ^
				(null == (object)Selection ? 0 : Selection.GetHashCode());
		}
		public static bool Equals(ref CameraData L, ref CameraData R)
		{
			return Transform.Equals(ref L.Transform, ref R.Transform) &&
				L.Selection == R.Selection &&
				L.RadiansPitch == R.RadiansPitch &&
				L.RadiansYaw == R.RadiansYaw &&
				L.LookTarget == R.LookTarget;
		}
		public static bool Inequals(ref CameraData L, ref CameraData R)
		{
			return Transform.Inequals(ref L.Transform, ref R.Transform) ||
				L.Selection != R.Selection ||
				L.RadiansPitch != R.RadiansPitch ||
				L.RadiansYaw != R.RadiansYaw ||
				L.LookTarget != R.LookTarget;
		}
		public static bool operator ==(CameraData L, CameraData R)
		{
			return Equals(ref L, ref R);
		}
		public static bool operator !=(CameraData L, CameraData R)
		{
			return Inequals(ref L, ref R);
		}
		public bool Equals(ref CameraData other) { return Equals(ref this, ref other); }
		public bool Equals(CameraData other) { return Equals(ref this, ref other); }
		public override bool Equals(object obj) { return obj is CameraData && ((CameraData)obj).Equals(ref this); }
		public override string ToString()
		{
			return "CameraData";
		}
		public static CameraData Init => new CameraData{
			Transform = { rotation = Quaternion.Identity, scale = 1, },
			LookTarget = { Z = -500, },
		};
		public static CameraData NaN => new CameraData
		{
			RadiansPitch = double.NaN,
		};
		private static void Sanitize(ref double value)
		{
			if (!(value <= double.MaxValue && value >= -double.MaxValue))
				value = 0;
		}
		public static void RadianAddClamp(ref double Pitch, double Value)
		{
			Sanitize(ref Value);

			Pitch += Value;
			if (Pitch <= (Math.PI / -2))
				Pitch = (Math.PI / -2) + double.Epsilon;
			else if (Pitch >= (Math.PI / 2))
				Pitch = (Math.PI / 2) - double.Epsilon;
			else if (!(Pitch <= double.MaxValue && Pitch >= -double.MaxValue))
				Pitch = 0;
		}
		public static void RadianAddLoop(ref double Yaw, double Value) {
			Sanitize(ref Value);

			Yaw += Value;

			if (Yaw <= double.MaxValue && Yaw >= -double.MaxValue)
			{
				if (Yaw <= (-Math.PI * 2) || Yaw >= (Math.PI * 2))
				{
					Yaw %= Math.PI * 2;
				}

				if (Yaw < -Math.PI)
				{
					Yaw += (2 * Math.PI);
					if (Yaw >= Math.PI) Yaw = -Math.PI;
				}
				else if (Yaw >= Math.PI)
				{
					Yaw -= (2 * Math.PI);
					if (Yaw < -Math.PI) Yaw = -Math.PI;
				}
			}
		}
		public static void OrientateCam(ref CameraData Camera)
		{
			var Local = Camera.Transform.InverseTransformPosition(Camera.LookTarget);

			{
				Camera.Transform.rotation = Quaternion.FromAxisAngle(Vector3.UnitY, (float)Camera.RadiansYaw)*
					Quaternion.FromAxisAngle(Vector3.UnitX, (float)Camera.RadiansPitch);
				Camera.Transform.rotation.Normalize();
			}

			Camera.LookTarget = Camera.Transform.TransformPosition(Local);
		}
	}
	
	public static class CameraMode
	{
		public static readonly ICameraMode FLY, ORBIT, DEFAULT;
		private static readonly ICameraMode[] ALL;

		public const int COUNT = 2;
		public static ICameraMode GET(int i) { return ALL[i]; }

		static CameraMode()
		{
			FLY = new Fly();
			ORBIT = new Orbit();
			ALL = new ICameraMode[] { FLY, ORBIT, };
			DEFAULT = FLY;
		}

		private static void Rotate(ref CameraData CameraData, double RadiansPitch, double RadiansYaw)
		{
			CameraData.RadianAddClamp(ref CameraData.RadiansPitch,RadiansPitch);
			CameraData.RadianAddLoop(ref CameraData.RadiansYaw,RadiansYaw);
			CameraData.OrientateCam(ref CameraData);
			
		}
		private static void SnapOrbit(ref CameraData CameraData)
		{
			if (null != CameraData.Selection)
			{
				CameraData.Selection.LoadTransform(out Transform obj_transform);
				CameraData.Transform.translation += (obj_transform.translation - CameraData.LookTarget);
				CameraData.LookTarget = obj_transform.translation;
			}
		}
		private static void ClampOrbit(ref CameraData CameraData, float MinDistance, float DeltaDistance=0f)
		{
			var Distance = -(CameraData.Transform.InverseTransformPosition(
				CameraData.LookTarget).Z);

			if(0.0!=DeltaDistance)
				Distance -= DeltaDistance;

			if (Distance < MinDistance) Distance = MinDistance;

			CameraData.Transform.translation = CameraData.LookTarget -
				CameraData.Transform.TransformVector(new Vector3 { Z = (float)(-Distance), });
		}
		private static void RotateOrbit(ref CameraData CameraData, double RadiansPitch, double RadiansYaw)
		{
			Rotate(ref CameraData, RadiansPitch, RadiansYaw);
			SnapOrbit(ref CameraData);
		}
		private static void TransformToMatrix(ref CameraData CameraData, out Matrix4 Matrix)
		{
			Matrix = CameraData.Transform.Matrix;
		}
		private struct Orbit : ICameraMode
		{
			public const double YawRate = 0.01;
			public const double PitchRate = 0.01;
			public const float MinDistance = 300;
			void ICameraMode.Reset(ref CameraData CameraData) { }
			void ICameraMode.Select(ref CameraData CameraData)
			{
				SnapOrbit(ref CameraData);
			}
			void ICameraMode.Update(ref CameraData CameraData, out Matrix4 View)
			{
				TransformToMatrix(ref CameraData, out View);
			}
			void ICameraMode.Start(ref CameraData CameraData)
			{
				ClampOrbit(ref CameraData, MinDistance);
			}

			void ICameraMode.End(ref CameraData CameraData) { }
			void ICameraMode.Rotate(ref CameraData CameraData) 
			{
				RotateOrbit(ref CameraData, CameraData.CursorDelta.Y * PitchRate, CameraData.CursorDelta.X * YawRate);
			}
			void ICameraMode.Pan(ref CameraData CameraData)
			{
				RotateOrbit(ref CameraData, CameraData.CursorDelta.Y * PitchRate, CameraData.CursorDelta.X * YawRate);
			}
			void ICameraMode.Scroll(ref CameraData CameraData)
			{
				ClampOrbit(ref CameraData, MinDistance, (float)CameraData.Scroll);
			}
		}
		private struct Fly : ICameraMode
		{
			public const int ScaleOffset = 6;
			public const double YawRate = -0.01;
			public const double PitchRate = -0.01;

			void ICameraMode.Reset(ref CameraData CameraData) { }
			void ICameraMode.Update(ref CameraData CameraData, out Matrix4 View)
			{
				TransformToMatrix(ref CameraData, out View);
			}
			void ICameraMode.Start(ref CameraData CameraData) { }
			void ICameraMode.End(ref CameraData CameraData) { }
			void ICameraMode.Select(ref CameraData CameraData) { }
			void ICameraMode.Rotate(ref CameraData CameraData)
			{
				Rotate(ref CameraData, 
					CameraData.CursorDelta.Y * PitchRate,
					CameraData.CursorDelta.X * YawRate);
			}
			void ICameraMode.Scroll(ref CameraData CameraData)
			{
				var Offset = CameraData.Transform.TransformVector(new Vector3 { Z = (float)CameraData.Scroll, });
				CameraData.Transform.translation -= Offset;
				CameraData.LookTarget -= Offset;
			}
			void ICameraMode.Pan(ref CameraData CameraData)
			{
				Vector3 Offset= CameraData.Transform.TransformVector(
					new Vector3
					{
						X = Globals.camSpeedMultiplier * ScaleOffset * CameraData.CursorDelta.X,
					});

				Offset.Y += Globals.camSpeedMultiplier * ScaleOffset * CameraData.CursorDelta.Y;
				CameraData.LookTarget += Offset;
				CameraData.Transform.translation += Offset;
			}
		}
	}

    public sealed class Camera
	{
		private Matrix4 matrix, inverseMatrix;
		private CameraData data = CameraData.Init, CameraDataValidated=CameraData.NaN;
		// Camera mode
		private ICameraMode mode = Quad64.CameraMode.DEFAULT;
        private Level level;
		private 
			bool camStarted,
				camInvalidated,
				resetMouse=true,
				glInvalidate,
				posManipulated,
				rotManipulated;

		private void EnsureCameraStarted()
		{
			if (!camStarted)
			{
				camStarted = true;
				data.Selection = getSelectedObject();
				mode.Start(ref data);
				camInvalidated = true;
			}
		}
		private void EnsureCameraValidated()
		{
			if (camInvalidated)
			{
				mode.Update(ref data, out matrix);
				CameraDataValidated = data;
				inverseMatrix = matrix;
				inverseMatrix.Invert();
				glInvalidate = true;
			}
		}
		private void EnsureCameraMatrix()
		{
			EnsureCameraStarted();
			EnsureCameraValidated();
		}
		private void CheckCameraChanges()
		{
			data.Hint = false;
			data.Scroll = 0;
			data.CursorDelta = default(CursorPoint);

			if (!camInvalidated &&
				!CameraData.Equals(ref data, ref CameraDataValidated))
				camInvalidated = true;
		}

		private void CameraEvent(
			Object3D Object=null,
			CursorPoint? NewPoint=null,
			double? Scroll=null,
			bool Pan=false,
			bool AutoObject=false,
			bool ForbidPending=false)
		{
			if (AutoObject)
				Object = getSelectedObject() ?? Object;

			bool IsCursor = NewPoint.HasValue, IsScroll = Scroll.HasValue, IsSelect = Object != data.Selection || (!camStarted && null != (object)Object);

			if (IsCursor)
			{
				data.CursorDelta = NewPoint.Value - data.Cursor;
				data.Cursor = NewPoint.Value;
			}

			if (IsScroll)
			{
				data.Scroll = Scroll.Value;
			}

			if (IsSelect)
			{
				data.Selection = Object;
			}

			if (resetMouse && (/*IsScroll ||*/ IsCursor) )
			{
				resetMouse = false;

				if (camStarted)
					mode.Reset(ref data);

				data.CursorDelta = default(CursorPoint);
				//data.Scroll = 0;
			}

			if (IsScroll || IsCursor || IsSelect || (data.Hint=(!ForbidPending && (posManipulated || rotManipulated))))
			{
				EnsureCameraStarted();

				if (IsSelect)
				{
					mode.Select(ref data);
				}

				if (IsCursor)
				{
					if (Pan)
					{
						posManipulated = false;
						mode.Pan(ref data);
					}
					else
					{
						rotManipulated = false;
						mode.Rotate(ref data);
					}

					data.CursorDelta = default(CursorPoint);
				}

				if (IsScroll)
				{
					mode.Scroll(ref data);
					data.Scroll = 0;
				}
				if (!ForbidPending)
				{
					if (Pan)
					{
						if (posManipulated)
						{
							posManipulated = false;
							mode.Pan(ref data);
						}

						if (rotManipulated)
						{
							rotManipulated = false;
							mode.Rotate(ref data);
						}
					}
					else
					{
						if (rotManipulated)
						{
							rotManipulated = false;
							mode.Rotate(ref data);
						}
						if (posManipulated)
						{
							posManipulated = false;
							mode.Pan(ref data);
						}

					}
				}
				data.Hint = false;
				CheckCameraChanges();
			}
		}

		public ICameraMode CameraMode
		{
			get => mode;
			set
			{
				if (null != value && mode != value)
				{
					if (camStarted)
						mode.End(ref data);
					mode = value;
					camStarted = false;
				}
			}
		}

		private static double Rad2Deg(double v)
		{
			return (v * 180) / Math.PI;
		}
        //private Matrix4 matrix = Matrix4.Identity;
		
        public float Yaw { get => (float)data.RadiansYaw; set { SetRotationFloat(ref data.RadiansYaw, value); } }
        public float Pitch { get=> (float)data.RadiansPitch; set { SetRotationFloat(ref data.RadiansPitch, value); } }
        public float Yaw_Degrees { get => (float)Rad2Deg(data.RadiansYaw); set { SetRotationFloat(ref data.RadiansYaw, (Math.PI*value)/180); } }
        public float Pitch_Degrees { get => (float)Rad2Deg(data.RadiansPitch); set { SetRotationFloat(ref data.RadiansPitch, (Math.PI*value)/180); } }
		public Transform Transform => data.Transform;
		private void SetPositionFloat(ref float CameraFloat, float Value)
		{
			if (Value != CameraFloat)
			{
				CameraFloat = Value;
				camInvalidated = true;
				posManipulated = true;
			}
		}
		private void SetRotationFloat(ref double CameraFloat, double Value)
		{
			if (Value != CameraFloat)
			{
				CameraFloat = Value;
				camInvalidated = true;
				rotManipulated = true;
			}
		}
		private void SetVector(ref Vector3 CameraVector, ref Vector3 Value)
		{
			if (!posManipulated)
			{
				SetPositionFloat(ref CameraVector.X, Value.X);
				SetPositionFloat(ref CameraVector.Y, Value.Y);
				SetPositionFloat(ref CameraVector.Z, Value.Z);
			}
			else
			{
				CameraVector = Value;
			}
		}
		public Vector3 Position {
			get => data.Transform.translation;
			set { SetVector(ref data.Transform.translation, ref value); }
		}
		public float X {get => data.Transform.translation.X;set{SetPositionFloat(ref data.Transform.translation.X,value);}}
		public float Y {get => data.Transform.translation.Y;set{SetPositionFloat(ref data.Transform.translation.Y,value);}}
		public float Z {get => data.Transform.translation.Z;set{SetPositionFloat(ref data.Transform.translation.Z,value);}}

		public Vector3 LookAt => data.LookTarget;
		public float LookAtX => data.LookTarget.X;
		public float LookAtY => data.LookTarget.Y;
		public float LookAtZ => data.LookTarget.Z;
		public Quaternion Rotation => data.Transform.rotation;
		public float Scale => data.Transform.scale;

        public Camera()
        {
        }

        public void SetLevel(Level level,
			ICameraMode cameraMode=null,
			bool keepCameraMode=false)
        {
            this.level = level;

			if (!keepCameraMode && null == cameraMode)
				cameraMode = Quad64.CameraMode.DEFAULT;

			this.CameraMode = cameraMode;
        }
		public Vector3 ZAxis => (data.Transform.TransformVectorNoScale(Vector3.UnitZ));
		public Vector3 YAxis => (data.Transform.TransformVectorNoScale(Vector3.UnitY));
		public Vector3 XAxis => (data.Transform.TransformVectorNoScale(Vector3.UnitX));

		public Vector3 InverseZAxis => (data.Transform.InverseTransformVectorNoScale(Vector3.UnitZ));
		public Vector3 InverseYAxis => (data.Transform.InverseTransformVectorNoScale(Vector3.UnitY));
		public Vector3 InverseXAxis => (data.Transform.InverseTransformVectorNoScale(Vector3.UnitX));
		

        private Object3D getSelectedObject()
        {
			return level.GetGlobalSelection();
		}
		

        public void Update(ref Matrix4 cameraMatrix, bool force=false)
		{
			CameraEvent(
				AutoObject: true
				);

			if (force) camInvalidated = true;

			EnsureCameraMatrix();

			cameraMatrix = inverseMatrix;

		}
		public void updateResetInvalidate(ref Matrix4 cameraMatrix, bool force=false)
		{
			Update(ref cameraMatrix, force: force);
			glInvalidate = false;
		}
		public bool updateCheckInvalidate(ref Matrix4 cameraMatrix, bool force=false)
		{
			Update(ref cameraMatrix, force: force);
			var ret = glInvalidate;
			glInvalidate = false;
			return ret;
		}
		public bool CheckInvalidate()
		{
			var ret = glInvalidate;
			glInvalidate = false;
			return ret;
		}
		public void ResetInvalidate()
		{
			glInvalidate = false;
		}

		public void SetCameraMode(ICameraMode mode, ref Matrix4 cameraMatrix)
        {
            CameraMode = mode;
			Update(ref cameraMatrix, force: true);
		}

		public void CursorRotate(int mouseX, int mouseY, ref Matrix4 cameraMatrix)
		{
			CameraEvent(
				AutoObject: true,
				NewPoint: new CursorPoint { X = mouseX, Y = mouseY, },
				Pan:false);
			EnsureCameraMatrix();

			cameraMatrix = inverseMatrix;
		}

        public void CursorPan(Vector3 orgPos, int mouseX, int mouseY, int w, int h, ref Matrix4 cameraMatrix)
		{
			CameraEvent(
				AutoObject: true,
				NewPoint: new CursorPoint { X = mouseX, Y = mouseY, },
				Pan: true);

			EnsureCameraMatrix();

			cameraMatrix = inverseMatrix;
		}

        public void Scroll(double amt, ref Matrix4 cameraMatrix)
		{
			CameraEvent(
				AutoObject: true,
				Scroll:amt);

			EnsureCameraMatrix();

			cameraMatrix = inverseMatrix;
		}
		public void SwallowNextCursor()
        {
            resetMouse = true;
        }
    }
}
