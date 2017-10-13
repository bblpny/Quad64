using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quad64
{
    enum CameraMode
    {
        FLY = 0,
        ORBIT = 1
    }

    class Camera
    {
        private readonly float TAU = (float)(Math.PI * 2);

        // Camera mode
        private CameraMode camMode = CameraMode.FLY;
        private Level level;
        private Vector3d pos = new Vector3d(-5000, 3000, 4000);
        private Vector3d lookat = new Vector3d(0, 0, 0);
        private Vector3d farPoint = new Vector3d(0, 0, 0);
        private Vector3d nearPoint = new Vector3d(0, 0, 0);
        private int lastMouseX = -1, lastMouseY = -1;
        private double CamAngleX = 0, CamAngleY = (Math.PI / -2);
        private bool resetMouse = true;
        //private Matrix4 matrix = Matrix4.Identity;

        public CameraMode CamMode { get { return camMode; } }
        public float Yaw { get { return (float)CamAngleX; } }
        public float Pitch { get { return (float)CamAngleY; } }
        public float Yaw_Degrees { get { return (float)((CamAngleX * 180)/Math.PI); } }
        public float Pitch_Degrees { get { return (float)((CamAngleY * 180) / Math.PI); } }

        public Vector3 Position
		{
			get { return new Vector3((float)pos.X, (float)pos.Y, (float)pos.Z); }
			set { pos.X = value.X; pos.Y = value.Y; pos.Z = value.Z; }
		}

        public Vector3 LookAt
		{
			get { return new Vector3((float)lookat.X, (float)lookat.Y, (float)lookat.Z); }
			set { lookat.X = value.X; lookat.Y = value.Y; lookat.Z = value.Z; }
		}
		public Vector3 NearPoint
		{
			get { return new Vector3((float)nearPoint.X, (float)nearPoint.Y, (float)nearPoint.Z); }
			set { nearPoint.X = value.X; nearPoint.Y = value.Y; nearPoint.Z = value.Z; }
		}
        public Vector3 FarPoint
		{
			get { return new Vector3((float)farPoint.X, (float)farPoint.Y, (float)farPoint.Z); }
			set { farPoint.X = value.X; farPoint.Y = value.Y; farPoint.Z = value.Z; }
		}
		//public Matrix4 CameraMatrix { get { return matrix; } set { matrix = value; } }

		private double orbitDistance = 500;
        private double orbitTheta = 0, orbitPhi = 0;

        public Camera()
        {
            setRotationFromLookAt();
        }

        public void setLevel(Level level)
        {
            this.level = level;
            camMode = CameraMode.FLY;
        }

        static private double clampf(double value, double min, double max)
        {
            return (value > max ? max : (value < min ? min : value));
        }

        private void orientateCam(double ang, double ang2)
        {
            var CamLX = (double)Math.Sin(ang) * (double)Math.Sin(-ang2);
            var CamLY = (double)Math.Cos(ang2);
            var CamLZ = (double)-Math.Cos(ang) * (double)Math.Sin(-ang2);

            lookat.X = pos.X + (-CamLX) * 100f;
            lookat.Y = pos.Y + (-CamLY) * 100f;
            lookat.Z = pos.Z + (-CamLZ) * 100f;
        }
		public Vector3 forward
		{
			get
			{
				double pitch = CamAngleY - (Math.PI / 2);
				var cx = Math.Cos(-pitch);
				var CamLX = Math.Sin(CamAngleX) * cx;
				var CamLY = Math.Sin(pitch);
				var CamLZ = -Math.Cos(CamAngleX) * cx;
				cx = CamLX * CamLX + CamLY * CamLY + CamLZ * CamLZ;
				if (cx > float.Epsilon)
				{
					cx = Math.Sqrt(cx);
					return new Vector3((float)(CamLX / cx), (float)(CamLY / cx), (float)(CamLZ / cx));
				}
				return Vector3.Zero;
			}
		}
        private void offsetCam(double Amt)
        {
            double pitch = CamAngleY - (Math.PI/2);
			//Console.WriteLine("Math.Sin(pitch) = " + Math.Sin(pitch));
			var cx = Math.Cos(-pitch);
            var CamLX = Math.Sin(CamAngleX) * cx;
            var CamLY = Math.Sin(pitch);
            var CamLZ = -Math.Cos(CamAngleX) * cx;
			cx = CamLX * CamLX + CamLY * CamLY + CamLZ * CamLZ;
			if (cx > float.Epsilon)
			{
				cx = Math.Sqrt(cx);
				pos.X = pos.X + Amt * (CamLX/cx) * Globals.camSpeedMultiplier;
				pos.Y = pos.Y + Amt * (CamLY/cx) * Globals.camSpeedMultiplier;
				pos.Z = pos.Z + Amt * (CamLZ/cx) * Globals.camSpeedMultiplier;
			}
        }

        public void setRotationFromLookAt()
        {
            var x_diff = lookat.X - pos.X;
            var y_diff = lookat.Y - pos.Y;
            var z_diff = lookat.Z - pos.Z;
            var dist = Math.Sqrt(x_diff * x_diff + y_diff * y_diff + z_diff * z_diff);
            if (z_diff == 0) z_diff = 0.001;
            var nxz_ratio = -x_diff / z_diff;
            if (z_diff < 0)
                CamAngleX = (Math.Atan(nxz_ratio) + Math.PI);
            else
                CamAngleX = (Math.Atan(nxz_ratio));
            CamAngleY = -Math.PI - ((Math.Asin(y_diff/dist)) - (Math.PI/2));
        }

        private Object3D getSelectedObject()
        {
            if (Globals.list_selected == -1 || Globals.item_selected == -1)
                return null;
            switch (Globals.list_selected)
            {
                case 0:
                    return level.getCurrentArea().Objects[Globals.item_selected];
                case 1:
                    return level.getCurrentArea().MacroObjects[Globals.item_selected];
                case 2:
                    return level.getCurrentArea().SpecialObjects[Globals.item_selected];
                default:
                    return null;
            }
        }

        public void resetOrbitToSelectedObject()
        {
            Object3D obj = getSelectedObject();
            if (obj != null)
            {
                orbitTheta = (obj.yRot * Math.PI)/-180.0;
                orbitPhi = -0.3f;
                orbitDistance = 1200.0f;
            }
        }

        public void updateOrbitCamera(ref Matrix4 cameraMatrix)
        {
            if (camMode == CameraMode.ORBIT)
            {
                Object3D obj = getSelectedObject();
                if (obj == null)
                    return;
                pos.X = obj.xPos + (Math.Cos(orbitPhi) * -Math.Sin(orbitTheta) * orbitDistance);
                pos.Y = obj.yPos + (-Math.Sin(orbitPhi) * orbitDistance);
                pos.Z = obj.zPos + (Math.Cos(orbitPhi) * Math.Cos(orbitTheta) * orbitDistance);
                lookat.X = obj.xPos;
                lookat.Y = obj.yPos;
                lookat.Z = obj.zPos;
				calculateMatrix(out cameraMatrix);
                setRotationFromLookAt();
            }
        }

        public bool isOrbitCamera()
        {
            return camMode == CameraMode.ORBIT;
        }

        public void setCameraMode(CameraMode mode, ref Matrix4 cameraMatrix)
        {
            camMode = mode;
            if (isOrbitCamera())
            {
                resetOrbitToSelectedObject();
                updateOrbitCamera(ref cameraMatrix);
            }
        }

        public void updateCameraMatrixWithMouse(int mouseX, int mouseY, ref Matrix4 cameraMatrix)
        {
            if (camMode == CameraMode.ORBIT && Globals.item_selected > -1)
                updateCameraMatrixWithMouse_ORBIT(mouseX, mouseY, ref cameraMatrix);
            else
                updateCameraMatrixWithMouse_FLY(mouseX, mouseY, ref cameraMatrix);
        }

        public void updateCameraOffsetWithMouse(Vector3 orgPos, int mouseX, int mouseY, int w, int h, ref Matrix4 cameraMatrix)
        {
            if (camMode == CameraMode.ORBIT && Globals.item_selected > -1)
                updateCameraOffsetWithMouse_ORBIT(mouseX, mouseY, ref cameraMatrix);
            else
                updateCameraOffsetWithMouse_FLY(orgPos, mouseX, mouseY, w, h, ref cameraMatrix);
        }

        public void updateCameraMatrixWithScrollWheel(double amt, ref Matrix4 cameraMatrix)
        {
            if (camMode == CameraMode.ORBIT && Globals.item_selected > -1)
                updateCameraMatrixWithScrollWheel_ORBIT(amt, ref cameraMatrix);
            else
                updateCameraMatrixWithScrollWheel_FLY(amt, ref cameraMatrix);
        }

		private void calculateMatrix(out Matrix4 cameraMatrix)
		{
			var db=Matrix4d.LookAt(
				pos.X, pos.Y, pos.Z, 
				lookat.X, lookat.Y, lookat.Z,
				0, 1, 0);

			cameraMatrix = default(Matrix4);

			for (byte r = 0; r < 4; r++)
				for (byte c = 0; c < 4; c++)
					cameraMatrix[r, c] = (float)db[r, c];

		}
        private void updateCameraMatrixWithScrollWheel_FLY(double amt, ref Matrix4 cameraMatrix)
        {
            offsetCam(amt);
            orientateCam(CamAngleX, CamAngleY);
			calculateMatrix(out cameraMatrix);
		}

        public void updateMatrix(ref Matrix4 cameraMatrix)
		{
			calculateMatrix(out cameraMatrix);
		}

        private void updateCameraMatrixWithMouse_FLY(int mouseX, int mouseY, ref Matrix4 cameraMatrix)
        {
            if (resetMouse)
            {
                lastMouseX = mouseX;
                lastMouseY = mouseY;
                resetMouse = false;
            }
            int MousePosX = mouseX - lastMouseX;
            int MousePosY = mouseY - lastMouseY;
            CamAngleX = CamAngleX + (0.01f * MousePosX);
            // This next part isn't neccessary, but it keeps the Yaw rotation value within [0, 2*pi] which makes debugging simpler.
            if (CamAngleX > TAU) CamAngleX -= TAU;
            else if (CamAngleX < 0) CamAngleX += TAU;

            /* Lock pitch rotation within the bounds of [-3.1399.0, -0.0001], otherwise the LookAt function will snap to the 
               opposite side and reverse mouse looking controls.*/
            CamAngleY = clampf((CamAngleY + (0.01f * MousePosY)), -3.1399f, -0.0001f);
            
            lastMouseX = mouseX;
            lastMouseY = mouseY;
            orientateCam(CamAngleX, CamAngleY);
			calculateMatrix(out cameraMatrix);
            //Console.WriteLine("CamAngleX = " + CamAngleX + ", CamAngleY = " + CamAngleY);
            //setRotationFromLookAt();
        }

        private void updateCameraOffsetWithMouse_FLY(Vector3 orgPos, int mouseX, int mouseY, int w, int h, ref Matrix4 cameraMatrix)
        {
            if (resetMouse)
            {
                lastMouseX = mouseX;
                lastMouseY = mouseY;
                resetMouse = false;
            }
            int MousePosX = (-mouseX) + lastMouseX;
            int MousePosY = (-mouseY) + lastMouseY;
            //Console.WriteLine(MousePosX+","+ MousePosY);
            double pitch = CamAngleY - (Math.PI / 2);
            double yaw = CamAngleX - (Math.PI / 2);
            float CamLX = (float)Math.Sin(yaw);
           // float CamLY = (float)Math.Cos(pitch);
            float CamLZ = (float)-Math.Cos(yaw);
            pos.X = orgPos.X - ((MousePosX * Globals.camSpeedMultiplier) * (CamLX) * 6f);
            pos.Y = orgPos.Y - ((MousePosY * Globals.camSpeedMultiplier) * (-1f) * 6f);
            pos.Z = orgPos.Z - ((MousePosX * Globals.camSpeedMultiplier) * (CamLZ) * 6f);
            
            orientateCam(CamAngleX, CamAngleY);
			calculateMatrix(out cameraMatrix);
        }

        private void updateCameraMatrixWithMouse_ORBIT(int mouseX, int mouseY, ref Matrix4 cameraMatrix)
        {
            updateCameraOffsetWithMouse_ORBIT(mouseX, mouseY, ref cameraMatrix);
        }

        private void updateCameraOffsetWithMouse_ORBIT(int mouseX, int mouseY, ref Matrix4 cameraMatrix)
        {
            if (resetMouse)
            {
                lastMouseX = mouseX;
                lastMouseY = mouseY;
                resetMouse = false;
            }
            int MousePosX = (-mouseX) + lastMouseX;
            int MousePosY = (-mouseY) + lastMouseY;
            orbitTheta += MousePosX * 0.01f * Globals.camSpeedMultiplier;
            orbitPhi -= MousePosY * 0.01f * Globals.camSpeedMultiplier;
            orbitPhi = clampf(orbitPhi, -1.57f, 1.57f);
            updateOrbitCamera(ref cameraMatrix);
            lastMouseX = mouseX;
            lastMouseY = mouseY;
            //Console.WriteLine("ORBIT_THETA: " + orbitTheta + ", ORBIT_PHI: " + orbitPhi);
        }

        private void updateCameraMatrixWithScrollWheel_ORBIT(double amt, ref Matrix4 cameraMatrix)
        {
            orbitDistance -= amt;
            if (orbitDistance < 300.0f)
                orbitDistance = 300.0f;
            updateOrbitCamera(ref cameraMatrix);
        }

        public void resetMouseStuff()
        {
            resetMouse = true;
        }
    }
}
