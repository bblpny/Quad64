using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Windows.Forms;
using PropertyGridExtensionHacks;
using Quad64.Scripts;
using Quad64.src.JSON;
using Quad64.src;
using Quad64.src.Viewer;
using System.IO;
using Quad64.src.TestROM;
using Quad64.src.Forms;
using BubblePony.PixelFoundation;
using BubblePony.GLHandle;

namespace Quad64
{
    partial class MainForm : Form
    {
        Model3D model1 = new Model3D(), model2 = new Model3D();
        Color bgColor = Color.Black;
        Camera camera = new Camera();
        Vector3 savedCamPos = new Vector3();
        Matrix4 camMtx = Matrix4.Identity;
		readonly RenderList render_list = new RenderList();
        Matrix4 ProjMatrix;
		private object gl_lock = new object();
        bool isMouseDown = false, isShiftDown = false, moveState = false;
        static Level level;
        float FOV = 1.048f;

        public Level getLevelData { get { return level; } }

        public object SettingsForms { get; private set; }

        private short keepDegreesWithin360(short value)
        {
            if (value <0)
                return (short)TransformUtility.NormalizeDegrees180(value);
            else
                return (short)TransformUtility.NormalizeDegrees(value);
        }
		static string ExportAddress(BubblePony.Alloc.ByteSegment address)
		{
			var adr = address.ROM_Address();
			if (adr.Data.Length == 0)
				return adr.Data.Offset.ToString("X8");
			else
				return adr.Data.Offset.ToString("X8") + adr.Input.Value.ToString("X16");
		}
		const string notExportedString= "<NOT EXPORTED>";
		string ExportStringFilename(string directory, string filename)
		{
			if (0 == filename.Length)
				return notExportedString;

			if (!Path.IsPathRooted(directory))
				directory = Path.GetFullPath(directory);
	
			if (!Path.IsPathRooted(filename))
				filename = Path.GetFullPath(filename);

			if (directory == Path.GetDirectoryName(filename))
				return Path.GetFileName(filename);
			else
				return filename;
		}
		void PromptObjColors()
		{
			var res = MessageBox.Show(
string.Format(@"Some programs support vertex color through .OBJ but it is not standard.

Want to export vertex colors?
	(Yes to export vertex colors)
	(No to omit them)
	(Cancel to continue to {0} vertex colors)", Globals.wavefrontVertexColors ? "use" : "not use"),
"Vertex Color", MessageBoxButtons.YesNoCancel);

			if (res == DialogResult.Yes || res == DialogResult.No)
				Globals.wavefrontVertexColors = DialogResult.Yes == res;
		}
		static void HardGC()
		{
			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
		}
		const string exportImageText = @"imgWWWWHHHHmmmmmmmmmmmmmmmm{0}
	W=width(hex)
	H=height(hex)
	m=MD5 hash of ARGB values
(MTL references by this naming)
(Only images in area will be written)";
		bool Export(
			Area area = null,
			bool skip_prompt = false, bool? skip_vertex_color_prompt = null, bool? skip_results=null,
			string directory = null,
			string objFile = null,
			string objCollisionFile = null,
			string mtlFile = null,
			string objectName = null,
			string objectCollisionName = null,
			IImageFormatter<IntPtr, IntPtr> imageFormat = null,
			bool skip_images = false,
			bool skip_gc = false,
			bool area_only = false)
		{
			if (area == null)
			{
				var level = MainForm.level;
				if (null == level)
					return false;
				area = level.getCurrentArea();
				if (null == area)
					return false;
			}
			var levelIDHex = area.level.LevelID.ToString("X2");
			var areaIDHex = area.AreaID.ToString("X2");

			
			if(null==directory) directory =
				 System.IO.Path.Combine(
				 System.IO.Path.GetDirectoryName(ROM.Instance.filepath),
				 "Quad64Export_" + System.IO.Path.GetFileNameWithoutExtension(ROM.Instance.filepath));

			if (null == objectName)
			{
				objectName = string.Concat("L", levelIDHex, "X", areaIDHex);
			}

			if (null == objectCollisionName)
			{
				objectCollisionName = objectName + "c";
			}
			if (null == imageFormat)
				imageFormat = BubblePony.PixelFoundation.Drawing.Formatter.Png;

			if (null == objFile)
				objFile = System.IO.Path.Combine(directory, objectName + ".obj");

			if (null == objCollisionFile)
				objCollisionFile = System.IO.Path.Combine(directory, objectCollisionName + ".obj");

			if (null == mtlFile)
			{
				int objIndex = objFile.LastIndexOf(".obj", StringComparison.OrdinalIgnoreCase);

				if (objIndex + 4 != objFile.Length || objIndex <= objFile.LastIndexOf(Path.DirectorySeparatorChar) ||
					objIndex <= objFile.LastIndexOf(Path.AltDirectorySeparatorChar))
					mtlFile = System.IO.Path.Combine(directory, objectName + ".mtl");
				else
					mtlFile = objFile.Remove(objIndex) + ".mtl";
			}

			if (
				(objFile.Length == 0 && mtlFile.Length == 0 && objCollisionFile.Length == 0 && skip_images) ||
				(!skip_prompt && MessageBox.Show(
				string.Format(
@"DIR:	{0}
MTL:	{1}
OBJ:	{2}
COL:	{3}
TEX:	{4}

Still want to export?",
directory,
ExportStringFilename(directory, mtlFile),
ExportStringFilename(directory, objFile),
ExportStringFilename(directory, objCollisionFile),
skip_images ? notExportedString : string.Format(exportImageText, imageFormat.DefaultExtension ?? string.Empty)),
string.Format("Export of area 0x{1} in level 0x{0}", levelIDHex, areaIDHex),
MessageBoxButtons.YesNo) != DialogResult.Yes))
				return false;

			if (!(skip_vertex_color_prompt ?? skip_prompt)) PromptObjColors();
			if (!PreEmptiveDirectory(objFile) || !PreEmptiveDirectory(mtlFile) || !PreEmptiveDirectory(objCollisionFile) || !PreEmptiveDirectory(skip_images ? string.Empty : System.IO.Path.Combine(directory, "something.extsion")))
				return false;
			if (!skip_gc)
				HardGC();

			long ProcStart=0, ProcObjGen=0, ProcObjWrite=0, ProcMtlWrite=0, ProcTexWrite=0, ProcColGen=0,ProcColWrite=0;
			ProcStart = System.Diagnostics.Stopwatch.GetTimestamp();
			bool needsGeoObj = 0 != objFile.Length || 0 != mtlFile.Length || !skip_images;
			if (needsGeoObj)
			{
				var Obj = new BubblePony.Wavefront.Model(Globals.wavefrontVertexColors)
				{
					Object = objectName,
				};
				var baseGroupName = "g" + objectName;
				Obj.Group = baseGroupName;
				foreach (var mesh in area.AreaModel.meshes)
					Obj += mesh;

				foreach (var item_list in
					area_only ? new System.Collections.Generic.List<Object3D>[0] :
					new System.Collections.Generic.List<Object3D>[] { area.Objects, area.MacroObjects, area.SpecialObjects })
				{
					var baseListGroupName = baseGroupName +
							(item_list == area.Objects ? "o" :
							item_list == area.MacroObjects ? "m" :
							"s");
					foreach (var item in item_list)
					{
						var oid = item.ModelID;
						if (0 == oid || !area.level.ModelIDs.TryGetValue(oid, out Model3D m3d) ||
							m3d.meshes.Count == 0)
							continue;
						Obj.Group = string.Concat(baseListGroupName,
							oid.ToString("X2"),
							ExportAddress(item.memory));
						item.LoadTransform(out Transform objectTransform);

						if (!(objectTransform == Transform.Identity))
							foreach (var mesh in m3d.meshes)
								Obj += mesh.GetTransformed(objectTransform);
						else
							foreach (var mesh in m3d.meshes)
								Obj += mesh;
					}
				}
				ProcObjGen = System.Diagnostics.Stopwatch.GetTimestamp();
				if (0 != objFile.Length)
					using (
						var ObjStream =
							System.IO.File.CreateText(objFile)
							) if (0 == mtlFile.Length)
							Obj.SaveObj(objFile, ObjStream);
						else
							Obj.SaveObj(new string[] { Path.GetFileNameWithoutExtension(mtlFile), },
								ObjStream);
				ProcObjWrite = System.Diagnostics.Stopwatch.GetTimestamp();

				if (0 != mtlFile.Length)
					using (
						var MtlStream =
							System.IO.File.CreateText(mtlFile)
							) if (null == imageFormat)
							Obj.SaveMtl(MtlStream, ".png");
						else Obj.SaveMtl(MtlStream, imageFormat);
				ProcMtlWrite = System.Diagnostics.Stopwatch.GetTimestamp();

				if (!skip_images)
					Obj.SaveImages(
						directory, imageFormat);

				ProcTexWrite = System.Diagnostics.Stopwatch.GetTimestamp();

			}
			if (0 != objCollisionFile.Length)
			{
				var Obj = new BubblePony.Wavefront.Model(false)
				{
					Object = objectCollisionName,
				};
				var v = area.collision.GetVertices();
				var t_group_N = 0;
				Vector3 a, b, c;
				foreach (var tri_list in area.collision.GetTriangles())
				{
					Obj.Group = objectName + "C" + t_group_N.ToString("X4"); t_group_N++;

					for (int i = 0; i < tri_list.Length;)
					{
						a = v[tri_list[i++]];
						b = v[tri_list[i++]];
						c = v[tri_list[i++]];

						Obj.Publish(
							Obj.Facets[
								Obj.Positions[a.X, a.Y, a.Z],
								Obj.Positions[b.X, b.Y, b.Z],
								Obj.Positions[c.X, c.Y, c.Z]
								]);
					}
					ProcColGen = System.Diagnostics.Stopwatch.GetTimestamp();
					using (
						var ObjStream =
							System.IO.File.CreateText(objCollisionFile)
							) Obj.SaveObj((string)null, ObjStream);
				}
			}
			ProcColWrite = System.Diagnostics.Stopwatch.GetTimestamp();

			if (!(skip_results??skip_prompt))
			{
				if (!needsGeoObj)
				{
					ProcObjGen = ProcStart;
					ProcObjWrite = ProcStart;
					ProcMtlWrite = ProcStart;
					ProcTexWrite = ProcStart;
				}

				MessageBox.Show(
				string.Format(
@"PrepObj	{0}
WriteObj	{1}
WriteMtl	{2}
WriteTex	{3}
PrepCol	{4}
WriteCol	{5}
Total	{6}", TimeString(ProcObjGen - ProcStart), TimeString(ProcObjWrite - ProcObjGen), TimeString(ProcMtlWrite - ProcObjWrite), TimeString(ProcTexWrite - ProcMtlWrite), TimeString(ProcColGen - ProcMtlWrite), TimeString(ProcColWrite - ProcColGen), TimeString(ProcColWrite - ProcStart)),
				"Finished", MessageBoxButtons.OK);
			}
			if (!skip_gc)
				HardGC();
			return true;
		}
		static string TimeString(long elapsed)
		{
			if (elapsed <= 0) return "--";

			long seconds = elapsed / System.Diagnostics.Stopwatch.Frequency;

			elapsed -= seconds *System.Diagnostics.Stopwatch.Frequency;

			long milliseconds = (elapsed * 1000) / System.Diagnostics.Stopwatch.Frequency;

			elapsed -= (milliseconds * System.Diagnostics.Stopwatch.Frequency) / 1000;

			long nanoseconds =
				((elapsed * 1000) * 1000) / System.Diagnostics.Stopwatch.Frequency;

			if (seconds <= 0)
				if (milliseconds <= 0)
					if (nanoseconds <= 0)
						return "0ns";
					else
						return nanoseconds + "ns";
				else if (nanoseconds <= 0)
					return milliseconds + "ms";
				else
					return string.Concat(milliseconds.ToString(), "ms", nanoseconds, "ns");
			else if (milliseconds <= 0)
				if (nanoseconds <= 0)
					return seconds + "s";
				else
					return string.Concat(seconds.ToString(), "s", nanoseconds, "ns");
			else if (nanoseconds <= 0)
				return string.Concat(seconds.ToString(), "s", milliseconds, "ms");
			else
				return string.Concat(seconds.ToString(),"s", milliseconds.ToString(),"ms", nanoseconds.ToString(),"ns");
		}
		static bool PreEmptiveDirectory(string path)
		{
			if (path.Length!=0 && !Directory.Exists(path = Path.GetDirectoryName(path)))
				try { Directory.CreateDirectory(path); } catch (System.Exception e) { MessageBox.Show(e.ToString(), e.GetType().Name, MessageBoxButtons.OK); return false; }
			return true;
		}
		void ExportButton(object Sender, EventArgs Args)
		{
			Export();
		}
		void ExportEverythingButton(object Sender, EventArgs Args)
		{
			bool firstLevel = true;
			var rom = ROM.Instance;
			foreach (var entry in rom.getLevelEntriesCopy())
			{
				rom.getSegment(0x15);
				Level testLevel = new Level(rom, rom.getSegment(0x15), entry.ID, 1);

				LevelScripts.parse(testLevel, 0x15, 0);
				foreach(var area in testLevel.Areas)
				{
					testLevel.CurrentAreaID = area.AreaID;
					if (firstLevel)
					{
						if (!Export(area: testLevel.getCurrentArea(), skip_prompt: false, skip_results:true))
							return;
						firstLevel = false;
					}
					else Export(area: testLevel.getCurrentArea(), skip_prompt: true);
				}
			}
		}
		private Timer drainTimer;
        public MainForm(string rom_path=null)
        {
			InitializeComponent();
			drainTimer = new Timer()
			{
				Enabled = false,
				Interval = 100,
			};
			drainTimer.Tick += OnDrainTick;
            OpenTK.Toolkit.Init();
            glControl1.CreateControl();
            SettingsFile.LoadGlobalSettings("default");
			Globals.pathToAutoLoadROM = rom_path ?? Globals.pathToAutoLoadROM;
			glControl1.MouseWheel += new MouseEventHandler(glControl1_Wheel);
			UpdateProjection();
			glControl1.Enabled = false;
            KeyPreview = true;
            treeView1.HideSelection = false;
            camera.updateMatrix(ref camMtx);
			fileToolStripMenuItem.DropDownItems.Add("Export Area").Click += ExportButton;
			fileToolStripMenuItem.DropDownItems.Add("Export Everything").Click += ExportEverythingButton;
			//foreach(ObjectComboEntry entry in Globals.objectComboEntries) Console.WriteLine(entry.ToString());
			if (null != (object)rom_path && System.IO.File.Exists(rom_path))
				Globals.autoLoadROMOnStartup = true;

			drainTimer.Enabled = true;
		}
		const int DRAIN_INTERVAL_POST_GC_NOTHING = 20 << 5;//~20 seconds.
		const int DRAIN_INTERVAL_POST_GC_SOMETHING = 80;//~20 seconds.
		const int DRAIN_INTERVAL_POST_SOMETHING = 60;
		const int DRAIN_INTERVAL_INIT = 200;
		const int DRAIN_INTERVAL_PAINT = 500;
		const int DRAIN_INTERVAL_NOW = 0;
		const int DRAIN_INTERVAL_BLOCKED = 1;
		const int DRAIN_PER_PAINT = 8;
		volatile int paint_counter = 0;
		private void SetDrainInterval(int value)
		{
			if (value <= 0) OnDrainTick(null, null);

			if (drainTimer.Interval <= value)
				return;
			drainTimer.Interval = value;
		}
		private void OnDrainTick(object sender, EventArgs e)
		{
			int id, count;
			GraphicHandleKind kind;
			if (System.Threading.Monitor.TryEnter(gl_lock))
				try
				{
					if (!GraphicsHandle.Drain(out id, out kind))
					{
						GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
						if (!GraphicsHandle.Drain(out id, out kind))
						{
							drainTimer.Interval = DRAIN_INTERVAL_POST_GC_NOTHING;
							return;
						}
						else
							drainTimer.Interval = DRAIN_INTERVAL_POST_GC_SOMETHING;
					}
					else
						SetDrainInterval(DRAIN_INTERVAL_POST_SOMETHING);
					count = 0;
					do
					{
#if DEBUG
						Console.WriteLine(string.Format("Drained: GraphicsHandle.{0,-20}{{{1,-16}}}.", kind, id));
#endif
					} while (++count < 512 && GraphicsHandle.Drain(out id, out kind));
#if DEBUG
					Console.WriteLine(string.Format("Finished Drain Tick: Drained {0} objects (Max:512)", count));
#endif
				}
				finally
				{
					System.Threading.Monitor.Exit(gl_lock);
				}
			else
				SetDrainInterval(DRAIN_INTERVAL_BLOCKED);
		}

		private void loadROM(bool startingUp)
        {
            ROM rom = ROM.Instance;
            if (startingUp && !Globals.pathToAutoLoadROM.Equals(""))
            {
                rom.readFile(Globals.pathToAutoLoadROM);
            }
            else
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                openFileDialog1.Filter = "Z64 ROM|*.z64|V64 ROM|*.v64|N64 ROM|*.n64|All Files|*";
                DialogResult result = openFileDialog1.ShowDialog();
                if (result == DialogResult.OK) // Test result.
                {
                    ROM.Instance.readFile(openFileDialog1.FileName);
                }
                else
                {
                    return;
                }
            }
            Globals.objectComboEntries.Clear();
            ModelComboFile.parseObjectCombos(Globals.getDefaultObjectComboPath());

            rom.setSegment(0x15, Globals.seg15_location[0], Globals.seg15_location[1], false);
            rom.setSegment(0x02, Globals.seg02_location[0], Globals.seg02_location[1], rom.isSegmentMIO0(0x02));

            level = new Level(rom, rom.getSegment(0x15), 0x10, 1);
            LevelScripts.parse(level, 0x15, 0);
            level.sortAndAddNoModelEntries();
            level.CurrentAreaID = level.Areas[0].AreaID;
            refreshObjectsInList();
            glControl1.Enabled = true;
            bgColor = Color.CornflowerBlue;
            camera.setLevel(level);
            updateAreaButtons();
            glControl1.Invalidate();
        }

        private void refreshObjectsInList()
        {
            Globals.list_selected = -1;
            Globals.item_selected = -1;
            propertyGrid1.SelectedObject = null;
            TreeNode objects = treeView1.Nodes[0];
            objects.Nodes.Clear();
            foreach (Object3D obj in level.getCurrentArea().Objects)
            {
                obj.Title = obj.getObjectComboName();
                objects.Nodes.Add(obj.Title);
               // objects.Nodes.Add("0x" + obj.Behavior.ToString("X8"));
            }

            TreeNode macro_objects = treeView1.Nodes[1];
            macro_objects.Nodes.Clear();
            foreach (Object3D obj in level.getCurrentArea().MacroObjects)
            {
                obj.Title = obj.getObjectComboName();
                macro_objects.Nodes.Add(obj.Title);
                //macro_objects.Nodes.Add("0x" + obj.Behavior.ToString("X8"));
            }

            TreeNode special_objects = treeView1.Nodes[2];
            special_objects.Nodes.Clear();
            foreach (Object3D obj in level.getCurrentArea().SpecialObjects)
            {
                obj.Title = obj.getObjectComboName();
                special_objects.Nodes.Add(obj.Title);
                //special_objects.Nodes.Add("0x" + obj.Behavior.ToString("X8"));
            }

            TreeNode warps = treeView1.Nodes[3];
            warps.Nodes.Clear();
            foreach (Warp warp in level.getCurrentArea().Warps)
            {
                warps.Nodes.Add(warp.ToString());
            }
            foreach (Warp warp in level.getCurrentArea().PaintingWarps)
            {
                warps.Nodes.Add(warp.ToString());
            }
            foreach (WarpInstant warp in level.getCurrentArea().InstantWarps)
            {
                warps.Nodes.Add(warp.ToString());
            }
        }

		private void glControl1_Paint(object sender, PaintEventArgs e)
		{
			if (DRAIN_PER_PAINT == paint_counter++)
			{
				paint_counter -= DRAIN_PER_PAINT;
				SetDrainInterval(DRAIN_PER_PAINT);
			}

			lock (gl_lock)
			{
				GL_Paint(sender, e);
			}

		}
		private void GL_Paint(object sender, PaintEventArgs e) {

			GL.ClearColor(bgColor);
            if (level != null)
			{
				ResetGL();
				bool camera_change = render_list.SetupCamera(
					ref camMtx,
					ref ProjMatrix,
					camera.Position,
					camera.Pitch,
					camera.Yaw);
				if (camera_change)
				{
					UpdateProjection();
					render_list.SetupCamera(
						 ref camMtx,
						 ref ProjMatrix,
						 camera.Position,
						 camera.Pitch,
						 camera.Yaw);
				}
					GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.MatrixMode(MatrixMode.Projection);
				GL.LoadMatrix(ref render_list.Camera.Proj);
				//GL.LoadMatrix(ref render_list.Camera.ViewProj);
				GL.MatrixMode(MatrixMode.Modelview);
				GL.LoadMatrix(ref render_list.Camera.View);
				//GL.LoadIdentity();
				if (Globals.doWireframe)
					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
				else
					GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

	

				//level.getCurrentArea().drawPicking();
				//level.getCurrentArea().drawEverything();
				if (Globals.renderCollisionMap)
					level.getCurrentArea().collision.drawCollisionMap(false);

				render_list.Clear();

//				using (var layer = new RenderList())
				{
					level.getCurrentArea().renderEverything(render_list);
					render_list.UpdateLayers();
					ResetGL();
					GL.Disable(EnableCap.AlphaTest);
					GL.AlphaFunc(AlphaFunction.Always, 0.5f);
					GL.Disable(EnableCap.Blend);
					GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.Zero);
					GL.DepthFunc(DepthFunction.Less);
					render_list.Solid.Draw();
					// i haven't found any forum posts about draw layer 2.
					// from looking at what is on it, it seems that draw layer 2 is dedicated to
					// solid decals. what i mean by solid decals is polygons that are nearly directly on top of
					// other solid polygons behind it in layer 1.
					//
					// this leads me to believe that the 2nd bit, when present (such as layer 6 (shadows))
					// should have the depth offset active.. turns out this is probably the case, since shadows would clip otherwise.
					GL.DepthFunc(DepthFunction.Lequal);
					GL.PolygonOffset(-1, -1);
					GL.DepthMask(false);//<-- we could not use this by drawing this first, but it don't seem right.
					GL.Enable(EnableCap.PolygonOffsetFill);
					render_list.SolidDecal.Draw();
					render_list.Decal.Draw();
					ResetGL();
					GL.Enable(EnableCap.AlphaTest);
					GL.AlphaFunc(AlphaFunction.Gequal, 1.0f);
					render_list.SemiTransparent.Draw();
					ResetGL();
					GL.Enable(EnableCap.Blend);
					GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
					GL.DepthFunc(DepthFunction.Lequal);
					GL.PolygonOffset(-1, -1);
					GL.DepthMask(false);//<-- we could not use this by drawing this first, but it don't seem right.
					GL.Enable(EnableCap.PolygonOffsetFill);
					render_list.Shadow.Draw();//<-- shadows.
					ResetGL();
					GL.Enable(EnableCap.DepthTest);
					GL.AlphaFunc(AlphaFunction.Greater, 0f);
					GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
					GL.DepthMask(false);
					render_list.Transparent.Draw();//<-- transparents.
					ResetGL();
					//layer.DrawModels((byte)(255 & (~((1 << 4)|(1<<5)|(1<<6)|(1<<1)))), ref Camera);//<-- transparents.
					// bounds..
					render_list.DrawBounds();

					if (Globals.doWireframe)
						GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
				}


                glControl1.SwapBuffers();
            }
        }
		public static void ResetGL()
		{
			GL.Enable(EnableCap.Blend);
			GL.Disable(EnableCap.AlphaTest);
			GL.AlphaFunc(AlphaFunction.Always, 0f);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			GL.Enable(EnableCap.Texture2D);
			GL.Disable(EnableCap.Normalize);
			GL.Disable(EnableCap.AutoNormal);
			GL.Disable(EnableCap.PolygonSmooth);
			GL.DepthMask(true);
			GL.DisableClientState(ArrayCap.VertexArray);
			GL.DisableClientState(ArrayCap.NormalArray);
			GL.DisableClientState(ArrayCap.ColorArray);
			GL.DisableClientState(ArrayCap.TextureCoordArray);
			//GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			GL.Disable(EnableCap.Fog);
			GL.Disable(EnableCap.PolygonOffsetFill);
			GL.CullFace(0); GL.PolygonOffset(0, 0);


		}

		private void selectObject(int mx, int my)
        {
            int h = glControl1.Height;
            //Console.WriteLine("Picking... mx = "+mx+", my = "+my);
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f); // Set background to solid white
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref ProjMatrix);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref camMtx);
            level.getCurrentArea().collision.drawCollisionMap(true); // Draw collision map as solid black
            level.getCurrentArea().drawPicking(); // Draw solid color object bounding boxes
            byte[] pixel = new byte[4];
            GL.ReadPixels(mx, h - my, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, pixel);
            if (pixel[0] == pixel[1] && pixel[1] == pixel[2])
            {
                if(pixel[2] == 255 || pixel[2] == 0)
                    return; // If a pixel is fully white or fully black, then ignore picking.
            }
            if (pixel[2] > 0 && pixel[2] < 4)
            {
                Globals.list_selected = pixel[2] - 1;
                Globals.item_selected = (pixel[1] * 256) + pixel[0];
                treeView1.SelectedNode = 
                    treeView1.Nodes[Globals.list_selected].Nodes[Globals.item_selected];
                switch (Globals.list_selected)
                {
                    case 0:
                        propertyGrid1.SelectedObject = 
                            level.getCurrentArea().Objects[Globals.item_selected];
                        break;
                    case 1:
                        propertyGrid1.SelectedObject = 
                            level.getCurrentArea().MacroObjects[Globals.item_selected];
                        break;
                    case 2:
                        propertyGrid1.SelectedObject = 
                            level.getCurrentArea().SpecialObjects[Globals.item_selected];
                        break;
                }
                if (camera.isOrbitCamera())
                {
                    camera.updateOrbitCamera(ref camMtx);
                    glControl1.Invalidate();
                }
            }
            Color pickedColor = Color.FromArgb(pixel[0], pixel[1], pixel[2]);
            //Console.WriteLine(pickedColor.ToString());
            //Console.WriteLine("Picking Done");
        }

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            savedCamPos = camera.Position;
            if (e.Button == MouseButtons.Right)
            {
                selectObject(e.X, e.Y);
                glControl1.Invalidate();
            }
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            camera.resetMouseStuff();
            isMouseDown = false;
            if (!isShiftDown)
                moveState = false;
        }

        private void glControl1_MouseLeave(object sender, EventArgs e)
        {
            camera.resetMouseStuff();
            isMouseDown = false;
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown && e.Button == MouseButtons.Left)
            {
                if (!isShiftDown && !moveState)
                {
                    camera.updateCameraMatrixWithMouse(e.X, e.Y, ref camMtx);
                }
                else
                {
                    moveState = true;
                    camera.updateCameraOffsetWithMouse(savedCamPos, e.X, e.Y, glControl1.Width, glControl1.Height, ref camMtx);
                }
                glControl1.Invalidate();
            }
        }

        private void glControl1_Wheel(object sender, MouseEventArgs e)
        {
            camera.resetMouseStuff();
            camera.updateCameraMatrixWithScrollWheel(e.Delta * 1.5, ref camMtx);
            savedCamPos = camera.Position;
            glControl1.Invalidate();
        }

        private void glControl1_KeyUp(object sender, KeyEventArgs e)
        {
            isShiftDown = e.Shift;
            if (!isMouseDown)
                moveState = false;
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            isShiftDown = e.Shift;
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
			lock (gl_lock)
			{
				GL.Enable(EnableCap.Blend);
				GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

				GL.Enable(EnableCap.DepthTest);
				GL.DepthFunc(DepthFunction.Lequal);

				GL.Enable(EnableCap.Texture2D);
				GL.Enable(EnableCap.AlphaTest);
				GL.AlphaFunc(AlphaFunction.Gequal, 0.5f);

				if (Globals.doBackfaceCulling)
					GL.Enable(EnableCap.CullFace);
				else
					GL.Disable(EnableCap.CullFace);
			}
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            glControl1.Context.Update(glControl1.WindowInfo);
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
			UpdateProjection();
            glControl1.Invalidate();
        }
        
        private void loadROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult saveResult = Prompts.ShowShouldSaveDialog(this);
            if(saveResult != DialogResult.Cancel)
                loadROM(false);
        }

        private void saveROMAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Z64 ROM|*.z64|V64 ROM|*.v64|N64 ROM|*.n64|All Files|*";
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                if(saveFileDialog1.FilterIndex == 1)
					runSave(saveFileDialog1.FileName, ROM_Endian.BIG);
                else if (saveFileDialog1.FilterIndex == 2)
                    runSave(saveFileDialog1.FileName, ROM_Endian.MIXED);
                else if (saveFileDialog1.FilterIndex == 3)
                    runSave(saveFileDialog1.FileName, ROM_Endian.LITTLE);
            }
        }

        private void saveROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
			runSave();
        }

		public DialogResult runSave(string overridePath=null, ROM_Endian ? overrideEndian = null, bool ignore_first_attempt=false)
		{
			string message;
			while (null != (object)(message = ROM.Instance.saveFileAs(
				overridePath ?? ROM.Instance.filepath,
				overrideEndian,
				ignore_first_attempt)))
			{
				var res = MessageBox.Show(message, "While saving..", MessageBoxButtons.AbortRetryIgnore);
				if (res == DialogResult.Ignore)
					ignore_first_attempt = true;
				else
				{
					if (res != DialogResult.Retry)
						return res == DialogResult.Abort ? DialogResult.Cancel : res;

					ignore_first_attempt = false;
				}
			}
			return DialogResult.Yes;
		}
        
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settings = new SettingsForm();
            settings.ShowDialog();
            glControl1.Invalidate();
            propertyGrid1.Refresh();
            glControl1.Update(); // Needed after calling propertyGrid1.Refresh();
        }

        private void objectComboPresetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectComboPreset comboWindow;
            switch (Globals.list_selected)
            {
                case 0:
                    comboWindow =
                        new SelectComboPreset(level, 0, "Select Object Combos", Color.DarkRed);
                    comboWindow.ShowDialog();
                    if(comboWindow.ClickedSelect)
                    {
                        Area area = level.getCurrentArea();
                        area.Objects[Globals.item_selected].ModelID = comboWindow.ReturnObjectCombo.ModelID;
                        area.Objects[Globals.item_selected].setBehaviorFromAddress(comboWindow.ReturnObjectCombo.Behavior);
                        treeView1.Nodes[0].Nodes[Globals.item_selected].Text
                            = area.Objects[Globals.item_selected].getObjectComboName();
                        area.Objects[Globals.item_selected].SetBehaviorParametersToZero();
                        area.Objects[Globals.item_selected].UpdateProperties();
                    }
                    break;
                case 1:
                    comboWindow =
                        new SelectComboPreset(level, 1, "Select Macro Preset", Color.DarkBlue);
                    comboWindow.ShowDialog();
                    if (comboWindow.ClickedSelect)
                    {
                        Console.WriteLine(comboWindow.ReturnPresetMacro.PresetID);
                        Area area = level.getCurrentArea();
                        area.MacroObjects[Globals.item_selected].ModelID = comboWindow.ReturnPresetMacro.ModelID;
                        area.MacroObjects[Globals.item_selected].setPresetID(comboWindow.ReturnPresetMacro.PresetID);
                        area.MacroObjects[Globals.item_selected].setBehaviorFromAddress(comboWindow.ReturnPresetMacro.Behavior);
                        //area.MacroObjects[Globals.item_selected].SetBehaviorParametersToZero();
                        area.MacroObjects[Globals.item_selected].BehaviorParameter1
                            = comboWindow.ReturnPresetMacro.BehaviorParameter1;
                        area.MacroObjects[Globals.item_selected].BehaviorParameter2
                            = comboWindow.ReturnPresetMacro.BehaviorParameter2;
                        treeView1.Nodes[1].Nodes[Globals.item_selected].Text
                            = area.MacroObjects[Globals.item_selected].getObjectComboName();
                        area.MacroObjects[Globals.item_selected].UpdateProperties();
                    }
                    break;
                case 2:
                    {
                        Object3D obj = getSelectedObject();
                        int specialListType = 2;
                        if (obj.isPropertyShown(Object3D.FLAGS.BPARAM_1))
                            specialListType = 4;
                        else if (obj.isPropertyShown(Object3D.FLAGS.ROTATION_Y))
                            specialListType = 3;
                        comboWindow =
                            new SelectComboPreset(level, specialListType, "Select Special Preset", Color.DarkGreen);
                        comboWindow.ShowDialog();
                        if (comboWindow.ClickedSelect)
                        {
                            Console.WriteLine(comboWindow.ReturnPresetMacro.PresetID);
                            Area area = level.getCurrentArea();
                            area.SpecialObjects[Globals.item_selected].ModelID = comboWindow.ReturnPresetMacro.ModelID;
                            area.SpecialObjects[Globals.item_selected].setPresetID(comboWindow.ReturnPresetMacro.PresetID);
                            area.SpecialObjects[Globals.item_selected].setBehaviorFromAddress(comboWindow.ReturnPresetMacro.Behavior);
                            //area.SpecialObjects[Globals.item_selected].SetBehaviorParametersToZero();
                            area.SpecialObjects[Globals.item_selected].BehaviorParameter1
                                = comboWindow.ReturnPresetMacro.BehaviorParameter1;
                            area.SpecialObjects[Globals.item_selected].BehaviorParameter2
                                = comboWindow.ReturnPresetMacro.BehaviorParameter2;
                            treeView1.Nodes[2].Nodes[Globals.item_selected].Text
                                = area.SpecialObjects[Globals.item_selected].getObjectComboName();
                            area.SpecialObjects[Globals.item_selected].UpdateProperties();
                        }
                        break;
                    }
            }
            glControl1.Invalidate();
            propertyGrid1.Refresh();
            glControl1.Update(); // Needed after calling propertyGrid1.Refresh();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            string glString = GL.GetString(StringName.Version).Split(' ')[0];
            if (glString.StartsWith("1."))
            {
                MessageBox.Show(
                    "Error: You have an outdated version of OpenGL, which is not supported by this program."+
                    " The program will now exit.\n\n" +
                    "OpenGL version: [" + GL.GetString(StringName.Version) + "]\n",
                    "OpenGL version error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error
                );
                Close();
            }
            Text += " (OpenGL v" + glString + ")";
            loadROM(Globals.autoLoadROMOnStartup);
        }

        private void updateAreaButtons()
        {
            int areas = 0x00;
            foreach (Area area in level.Areas)
                areas |= (1 << area.AreaID);
            Area0Button.Enabled = ((areas & 0x1) == 0x1);
            Area1Button.Enabled = ((areas & 0x2) == 0x2);
            Area2Button.Enabled = ((areas & 0x4) == 0x4);
            Area3Button.Enabled = ((areas & 0x8) == 0x8);
            Area4Button.Enabled = ((areas & 0x10) == 0x10);
            Area5Button.Enabled = ((areas & 0x20) == 0x20);
            Area6Button.Enabled = ((areas & 0x40) == 0x40);
            Area7Button.Enabled = ((areas & 0x80) == 0x80);
        }

        private void trySwitchArea(ushort toArea)
        {
            if (level.getCurrentArea().AreaID == toArea)
                return;

            if (level.hasArea(toArea))
            {
                level.CurrentAreaID = toArea;
                refreshObjectsInList();
                glControl1.Invalidate();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.P:
                    if (Globals.list_selected != -1 && Globals.item_selected != -1)
                    {
                        int listSel = Globals.list_selected;
                        int objSel = Globals.item_selected;
                        Object3D obj = getSelectedObject();
                        if (obj == null) return;
                        string newName = Prompts.ShowInputDialog("Type the new combo name", "New combo name");
                        if (newName.Length > 0)
                        {
                            obj.Title = newName;
                            uint segmentAddress = 0;
                            if(level.ModelIDs.ContainsKey(obj.ModelID))
                                segmentAddress = level.ModelIDs[obj.ModelID].GeoDataSegAddress;
                            ObjectComboEntry oce = new ObjectComboEntry(newName, obj.ModelID,
                                segmentAddress, obj.getBehaviorAddress());
                            Globals.insertNewEntry(oce);
                            refreshObjectsInList();
                            treeView1.SelectedNode = treeView1.Nodes[listSel].Nodes[objSel];
                            Globals.list_selected = listSel;
                            Globals.item_selected = objSel;
                            propertyGrid1.Refresh();
                        }
                        ModelComboFile.writeObjectCombosFile(Globals.getDefaultObjectComboPath());
                        Console.WriteLine("Saved Object Combos!");
                    }
                    break;
                case Keys.D1:
                    trySwitchArea(1);
                    break;
                case Keys.D2:
                    trySwitchArea(2);
                    break;
                case Keys.D3:
                    trySwitchArea(3);
                    break;
                case Keys.D4:
                    trySwitchArea(4);
                    break;
                case Keys.D5:
                    trySwitchArea(5);
                    break;
                case Keys.D6:
                    trySwitchArea(6);
                    break;
                case Keys.D7:
                    trySwitchArea(7);
                    break;
                case Keys.D0:
                    trySwitchArea(0);
                    break;
            }
        }

        private void resetObjectVariables()
        {
            radioButton1.Checked = true;
            treeView1.SelectedNode = null;
            Globals.list_selected = -1;
            Globals.item_selected = -1;
        }

        private void selectLeveToolStripMenuItem_Click(object sender, EventArgs e)
        {
			//Console.WriteLine("Opening SelectLevelForm!");
			using (SelectLevelForm newLevel = new SelectLevelForm(level.LevelID))
			{
				newLevel.ShowDialog();
				if (newLevel.changeLevel)
				{
					var rom = ROM.Instance;
					//Console.WriteLine("Changing Level to " + newLevel.levelID);
					Level testLevel = new Level(rom, rom.getSegment(0x15), newLevel.levelID, 1);
					LevelScripts.parse(testLevel, 0x15, 0);
					if (testLevel.Areas.Count > 0)
					{
						level = testLevel;
						camera.setCameraMode(CameraMode.FLY, ref camMtx);
						camera.setLevel(level);
						level.sortAndAddNoModelEntries();
						level.CurrentAreaID = level.Areas[0].AreaID;
						resetObjectVariables();
						refreshObjectsInList();
						glControl1.Invalidate();
						updateAreaButtons();
					}
					else
					{
						ushort id = newLevel.levelID;
						MessageBox.Show("Error: No areas found in level ID: 0x" + id.ToString("X"));
					}
				}
			}
        }


        private void testROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LaunchROM.OpenEmulator(this);
            SettingsFile.SaveGlobalSettings("default");
        }

        private void rOMInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ROMInfoForm romInfo = new ROMInfoForm();
            romInfo.ShowDialog();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

            TreeNode node = e.Node;
            //Console.WriteLine("Selected: " + node.Text);
            if (node.Parent == null)
            {
                propertyGrid1.SelectedObject = null;
                Globals.list_selected = -1;
                Globals.item_selected = -1;
                objectComboPresetToolStripMenuItem.Enabled = false;
            }
            else
            {
                objectComboPresetToolStripMenuItem.Enabled = true;
                if (node.Parent.Text.Equals("3D Objects"))
                {
                    Globals.list_selected = 0;
                    Globals.item_selected = node.Index;
                    propertyGrid1.SelectedObject = level.getCurrentArea().Objects[node.Index];
                    if (camera.isOrbitCamera())
                    {
                        camera.updateOrbitCamera(ref camMtx);
                        glControl1.Invalidate();
                    }
                    //int addr = level.getCurrentArea().Objects[node.Index].getROMAddress();
                    //ROM.Instance.printROMSection(addr, addr + 0x18);
                }
                else if (node.Parent.Text.Equals("Macro 3D Objects"))
                {
                    Globals.list_selected = 1;
                    Globals.item_selected = node.Index;
                    propertyGrid1.SelectedObject = level.getCurrentArea().MacroObjects[node.Index];
                    if (camera.isOrbitCamera())
                    {
                        camera.updateOrbitCamera(ref camMtx);
                        glControl1.Invalidate();
                    }
                    //int addr = level.getCurrentArea().MacroObjects[node.Index].getROMAddress();
                    //ROM.Instance.printROMSection(addr, addr + 10);
                }
                else if (node.Parent.Text.Equals("Special 3D Objects"))
                {
                    Globals.list_selected = 2;
                    Globals.item_selected = node.Index;
                    propertyGrid1.SelectedObject = level.getCurrentArea().SpecialObjects[node.Index];
                    if (camera.isOrbitCamera())
                    {
                        camera.updateOrbitCamera(ref camMtx);
                        glControl1.Invalidate();
                    }
                    //int addr = level.getCurrentArea().SpecialObjects[node.Index].getROMAddress();
                    //ROM.Instance.printROMSection(addr, addr + 12);
                }
                else if (node.Parent.Text.Equals("Warps"))
                {
                    Globals.list_selected = 3;
                    Globals.item_selected = node.Index;
                    Area area = level.getCurrentArea();
                    if (node.Index < area.Warps.Count)
                        propertyGrid1.SelectedObject = area.Warps[node.Index];
                    else if (node.Index < area.Warps.Count + area.PaintingWarps.Count)
                        propertyGrid1.SelectedObject = area.PaintingWarps[node.Index - area.Warps.Count];
                    else
                        propertyGrid1.SelectedObject = area.InstantWarps[node.Index - area.Warps.Count - area.PaintingWarps.Count];
                }
            }
            
            Object3D obj = getSelectedObject();
            if (obj != null)
            {
                if(obj.IsReadOnly)
                    objectComboPresetToolStripMenuItem.Enabled = false;
                obj.UpdateProperties();
                propertyGrid1.Refresh();
            }
            
            glControl1.Invalidate();
            glControl1.Update();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            fovText.Text = "FOV: " + trackBar1.Value + "°";
            FOV = trackBar1.Value * ((float)Math.PI/180.0f);
            if (FOV < 0.1f)
                FOV = 0.1f;
			UpdateProjection();
            glControl1.Invalidate();
        }
		private void UpdateProjection()
		{
			/*
			Vector3 min, max;
			var area = level == null ? null : level.getCurrentArea();
			if (null == area)
			{
				min.Z = min.Y = min.X = short.MinValue;
				max.Z = max.Y = max.X = short.MaxValue;
			}
			else
			{
				min = area.AreaModel.LowerBoundary;
				max = area.AreaModel.UpperBoundary;
				if (min.X > max.X) { var swap = min.X; min.X = max.X; max.X = swap; }
				if (min.Y > max.Y) { var swap = min.Y; min.Y = max.Y; max.Y = swap; }
				if (min.Z > max.Z) { var swap = min.Z; min.Z = max.Z; max.Z = swap; }
				max.X += 200;
				max.Y += 200;
				max.Z += 200;
				min.X -= 200;
				min.Y -= 200;
				min.Z -= 200;
			}
			float maxDistance=float.NegativeInfinity, minDistance = float.PositiveInfinity, testDistance;
			Vector3 point,local_min,local_max;
			var pos = camera.Position;
			local_min.Z = local_min.Y = local_min.X = minDistance;
			local_max.Z = local_max.Y = local_max.X = maxDistance;

			var fwd = Quaternion.FromEulerAngles(camera.Pitch,camera.Yaw,0f).Normalized().Inverted();
			var Z = camera.forward;

			for (int i = 0; i < 8; i++)
			{
				point.X = ((i & 1) == 0 ? min.X : max.X)-pos.X;
				point.Y = ((i & 2) == 0 ? min.Y : max.Y)-pos.Y;
				point.Z = ((i & 4) == 0 ? min.Z : max.Z)-pos.Z;
				Vector3.Dot(ref point, ref Z, out testDistance);
				point = fwd * point;
				if (testDistance > maxDistance) maxDistance = testDistance;
				if (testDistance < minDistance) minDistance = testDistance;
				if (point.X > local_max.X) local_max.X = point.X;
				if (point.Y > local_max.Y) local_max.Y = point.Y;
				if (point.Z > local_max.Z) local_max.Z = point.Z;

				if (point.X < local_min.X) local_min.X = point.X;
				if (point.Y < local_min.Y) local_min.Y = point.Y;
				if (point.Z < local_min.Z) local_min.Z = point.Z;
			}
			//maxDistance = local_max.Length; minDistance = -local_min.Length;
			if (minDistance > maxDistance ) { var swap = minDistance; minDistance = maxDistance; maxDistance = swap; }
			//if (minDistance > maxDistance) {  }

			if (!(minDistance > 100)) minDistance = 100;
			if (!(maxDistance > 1000)) maxDistance = 1000;
			if (minDistance >= maxDistance - 100) maxDistance = minDistance+100;

			var ProjMatrixDouble = 
				Matrix4d.CreatePerspectiveFieldOfView(
					FOV, 
					(double)glControl1.Width / glControl1.Height,
					Math.Sqrt(((double)minDistance * minDistance) * 2),
					Math.Sqrt(((double)maxDistance * maxDistance )* 2));
			*/
			var ProjMatrixDouble = 
				Matrix4d.CreatePerspectiveFieldOfView(
					FOV, 
					(double)glControl1.Width / glControl1.Height,
					100,
					69999);

			for (byte r = 0; r < 4; r++)
				for (byte c = 0; c < 4; c++)
					ProjMatrix[r, c] = (float)ProjMatrixDouble[r, c];
		}

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            string label = e.ChangedItem.Label;
            if (Globals.list_selected > -1 && Globals.list_selected < 3)
            {
                Object3D obj = getSelectedObject();
                if (obj == null) return;
                if (label.Equals("All Acts"))
                {
                    obj.ShowHideActs((bool)e.ChangedItem.Value);
                    propertyGrid1.Refresh();
                }
                else if(label.Equals("Behavior") || label.Equals("Model ID"))
                {
                    if(Globals.item_selected > -1)
                        treeView1.Nodes[Globals.list_selected].Nodes[Globals.item_selected].Text
                            = obj.getObjectComboName();
                }
                obj.updateROMData();
                if (camera.isOrbitCamera())
                    camera.updateOrbitCamera(ref camMtx);
                glControl1.Invalidate();
            }
            else if (Globals.list_selected == 3)
            {
                object warp = getSelectedWarp();
                string warpTypeName = warp.GetType().Name;
                if (warpTypeName.Equals("Warp"))
                {
                    ((Warp)warp).updateROMData();
                }
                else if (warpTypeName.Equals("WarpInstant"))
                {
                    ((WarpInstant)warp).updateROMData();
                }
            }
            Globals.needToSave = true;
        }

        // I never want CategorizedAlphabetical, so I change it back to Categorized if detected.
        private void propertyGrid1_PropertySortChanged(object sender, EventArgs e)
        {
            if (propertyGrid1.PropertySort == PropertySort.CategorizedAlphabetical)
                propertyGrid1.PropertySort = PropertySort.Categorized;
        }

        /* 
        Taken from: https://stackoverflow.com/a/21199864. This basically makes it 
        so that the object name in the list will always stay highlighted.
        */
			private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node == null) return;

            // if treeview's HideSelection property is "True", 
            // this will always returns "False" on unfocused treeview
            var selected = (e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected;
            var unfocused = !e.Node.TreeView.Focused;

            // we need to do owner drawing only on a selected node
            // and when the treeview is unfocused, else let the OS do it for us
            if (selected && unfocused)
            {
                var font = e.Node.NodeFont ?? e.Node.TreeView.Font;
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
                TextRenderer.DrawText(e.Graphics, e.Node.Text, font, e.Bounds, SystemColors.HighlightText, TextFormatFlags.GlyphOverhangPadding);
            }
            else
            {
                e.DrawDefault = true;
            }
        }
        
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                camera.setCameraMode(CameraMode.ORBIT, ref camMtx);
                camera.updateMatrix(ref camMtx);
                glControl1.Invalidate();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                camera.setCameraMode(CameraMode.FLY, ref camMtx);
                camera.updateMatrix(ref camMtx);
                glControl1.Invalidate();
            }
        }


        private void starAct_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton box = (RadioButton)sender;
            if (box.Checked)
                box.BackgroundImage = Properties.Resources.icon_Star1;
            else
                box.BackgroundImage = Properties.Resources.icon_Star1_gray;
        }


        int moveCam_InOut_lastPosY = 0;
        bool moveCam_InOut_mouseDown = false;
        private void moveCam_InOut_MouseDown(object sender, MouseEventArgs e)
        {
            moveCam_InOut_mouseDown = true;
            moveCam_InOut_lastPosY = e.Y;
        }
        private void moveCam_InOut_MouseUp(object sender, MouseEventArgs e)
        {
            moveCam_InOut_mouseDown = false;
        }
        private void moveCam_InOut_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveCam_InOut_mouseDown)
            {
                camera.resetMouseStuff();
                camera.updateCameraMatrixWithScrollWheel((e.Y - moveCam_InOut_lastPosY) * -10, ref camMtx);
                savedCamPos = camera.Position;
                moveCam_InOut_lastPosY = e.Y;
                glControl1.Invalidate();
            }
        }
        
        bool moveCam_strafe_mouseDown = false;
        private void moveCam_strafe_MouseDown(object sender, MouseEventArgs e)
        {
            savedCamPos = camera.Position;
            moveCam_strafe_mouseDown = true;
        }
        private void moveCam_strafe_MouseUp(object sender, MouseEventArgs e)
        {
            camera.resetMouseStuff();
            moveCam_strafe_mouseDown = false;
        }

        private void moveCam_strafe_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveCam_strafe_mouseDown)
            {
                camera.updateCameraOffsetWithMouse(savedCamPos, e.X, e.Y, glControl1.Width, glControl1.Height, ref camMtx);
                glControl1.Invalidate();
            }
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

        private object getSelectedWarp()
        {
            if (Globals.list_selected == -1 || Globals.item_selected == -1)
                return null;
            switch (Globals.list_selected)
            {
                case 3:
                    {
                        Area area = level.getCurrentArea();
                        int index = Globals.item_selected;
                        if (index < area.Warps.Count)
                        {
                            propertyGrid1.SelectedObject = area.Warps[index];
                        }
                        else if (index < area.Warps.Count + area.PaintingWarps.Count)
                        {
                            propertyGrid1.SelectedObject = area.PaintingWarps[index - area.Warps.Count];
                        }
                        else
                        {
                            propertyGrid1.SelectedObject = area.InstantWarps[index - area.Warps.Count - area.PaintingWarps.Count];
                        }
                        return propertyGrid1.SelectedObject;
                    }
                default:
                    return null;
            }
        }

        bool moveObj_mouseDown = false;
        int moveObj_lastMouseX = 0;
        int moveObj_lastMouseY = 0;
        short moveObj_savedX=0, moveObj_savedY=0, moveObj_savedZ=0;
        private void moveObj_MouseDown(object sender, MouseEventArgs e)
        {
            if (Globals.item_selected > -1 && Globals.list_selected > -1)
            {
                Object3D obj = getSelectedObject();
                if (obj == null) return;
                if (obj.IsReadOnly) return;
                moveObj_mouseDown = true;
                moveObj_lastMouseX = e.X;
                moveObj_lastMouseY = e.Y;
                moveObj_savedX = obj.xPos;
                moveObj_savedY = obj.yPos;
                moveObj_savedZ = obj.zPos;
            }
        }
        private void moveObj_MouseUp(object sender, MouseEventArgs e)
        {
            moveObj_mouseDown = false;
            Object3D obj = getSelectedObject();
            if (obj != null)
                obj.updateROMData();
        }
        private void moveObj_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveObj_mouseDown)
            {
                if (Globals.item_selected > -1 && Globals.list_selected > -1)
                {
                    Object3D obj = getSelectedObject();
                    if (obj == null) return;
                    if (obj.IsReadOnly) return;
                    short speedMult = 30;

                    int mx = e.X - moveObj_lastMouseX;
                    int my = -(e.Y - moveObj_lastMouseY);

                    float CX = (float)Math.Sin(camera.Yaw);
                    float CZ = (float)-Math.Cos(camera.Yaw);
                    float CX_2 = (float)Math.Sin(camera.Yaw + (Math.PI / 2));
                    float CZ_2 = (float)-Math.Cos(camera.Yaw + (Math.PI / 2));

                    if (obj.isPropertyShown(Object3D.FLAGS.POSITION_X))
                        obj.xPos = (short)(moveObj_savedX - (short)(CX * my * speedMult * Globals.objSpeedMultiplier) - (short)(CX_2 * mx * speedMult * Globals.objSpeedMultiplier));
                    if (obj.isPropertyShown(Object3D.FLAGS.POSITION_Z))
                        obj.zPos = (short)(moveObj_savedZ - (short)(CZ * my * speedMult * Globals.objSpeedMultiplier) - (short)(CZ_2 * mx * speedMult * Globals.objSpeedMultiplier));
                    if (keepOnGround.Checked)
                        dropObjectToGround();
                    if (camera.isOrbitCamera())
                        camera.updateOrbitCamera(ref camMtx);
                    glControl1.Invalidate();
                    propertyGrid1.Refresh();
                    glControl1.Update(); // Needed after calling propertyGrid1.Refresh();
                    Globals.needToSave = true;
                }
            }
        }
        
        bool moveObj_UpDown_mouseDown = false;
        int moveObj_UpDown_lastMouseY = 0;
        private void movObj_UpDown_MouseDown(object sender, MouseEventArgs e)
        {
            if (Globals.item_selected > -1 && Globals.list_selected > -1)
            {
                moveObj_UpDown_lastMouseY = e.Y;
                moveObj_UpDown_mouseDown = true;
            }
        }
        private void movObj_UpDown_MouseUp(object sender, MouseEventArgs e)
        {
            moveObj_UpDown_mouseDown = false;
            Object3D obj = getSelectedObject();
            if (obj != null)
                obj.updateROMData();
        }
        private void movObj_UpDown_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveObj_UpDown_mouseDown)
            {
                if (Globals.item_selected > -1 && Globals.list_selected > -1)
                {
                    Object3D obj = getSelectedObject();
                    if (obj == null) return;
                    if (obj.IsReadOnly) return;
                    obj.yPos -= (short)(30 * (e.Y - moveObj_UpDown_lastMouseY) * Globals.objSpeedMultiplier);
                    if (camera.isOrbitCamera())
                        camera.updateOrbitCamera(ref camMtx);
                    glControl1.Invalidate();
                    propertyGrid1.Refresh();
                    glControl1.Update(); // Needed after calling propertyGrid1.Refresh();
                    moveObj_UpDown_lastMouseY = e.Y;
                    Globals.needToSave = true;
                }
            }
        }
        
        bool rotObj_mouseDown = false;
        int rotObj_lastMouseX = 0;
        int rotObj_lastMouseY = 0;
        short rotObj_savedX = 0, rotObj_savedY = 0, rotObj_savedZ = 0;

        private void rotObj_MouseDown(object sender, MouseEventArgs e)
        {
            if (Globals.item_selected > -1 && Globals.list_selected > -1)
            {
                Object3D obj = getSelectedObject();
                if (obj == null) return;
                if (obj.IsReadOnly) return;
                rotObj_mouseDown = true;
                rotObj_lastMouseX = e.X;
                rotObj_lastMouseY = e.Y;
                rotObj_savedX = obj.xRot;
                rotObj_savedY = obj.yRot;
                rotObj_savedZ = obj.zRot;
            }
        }

        private void rotObj_MouseUp(object sender, MouseEventArgs e)
        {
            rotObj_mouseDown = false;
            Object3D obj = getSelectedObject();
            if (obj != null)
                obj.updateROMData();
        }

        private void rotObj_MouseMove(object sender, MouseEventArgs e)
        {
            if (rotObj_mouseDown)
            {
                if (Globals.item_selected > -1 && Globals.list_selected > -1)
                {
                    Object3D obj = getSelectedObject();
                    if (obj == null) return;
                    if (obj.IsReadOnly) return;
                    float speedMult = 0.5f;

                    int mx = e.X - rotObj_lastMouseX;
                    int my = -(e.Y - rotObj_lastMouseY);

                    float CZ = (float)Math.Sin(camera.Yaw);
                    float CX = (float)-Math.Cos(camera.Yaw);
                    float CZ_2 = (float)Math.Sin(camera.Yaw + (Math.PI / 2));
                    float CX_2 = (float)-Math.Cos(camera.Yaw + (Math.PI / 2));
                    if (obj.isPropertyShown(Object3D.FLAGS.ROTATION_X))
                    {
                        obj.xRot = (short)(rotObj_savedX - (short)(CX * my * speedMult * Globals.objSpeedMultiplier) - (short)(CX_2 * mx * speedMult * Globals.objSpeedMultiplier));
                        obj.xRot = keepDegreesWithin360(obj.xRot);
                    }
                    if (obj.isPropertyShown(Object3D.FLAGS.ROTATION_Z))
                    {
                        obj.zRot = (short)(rotObj_savedZ - (short)(CZ * my * speedMult * Globals.objSpeedMultiplier) - (short)(CZ_2 * mx * speedMult * Globals.objSpeedMultiplier));
                        obj.zRot = keepDegreesWithin360(obj.zRot);
                    }

                    if (camera.isOrbitCamera())
                        camera.updateOrbitCamera(ref camMtx);
                    glControl1.Invalidate();
                    propertyGrid1.Refresh();
                    glControl1.Update(); // Needed after calling propertyGrid1.Refresh();
                    Globals.needToSave = true;
                }
            }
        }

        private void treeView1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsLetter(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void dropObjectToGround()
        {
            if (Globals.item_selected > -1 && Globals.list_selected > -1)
            {
                Object3D obj = getSelectedObject();
                if (obj == null) return;
                obj.yPos = level.getCurrentArea().collision.dropToGround(new Vector3(obj.xPos, obj.yPos, obj.zPos));
                if (camera.isOrbitCamera())
                    camera.updateOrbitCamera(ref camMtx);
                glControl1.Invalidate();
                propertyGrid1.Refresh();
                glControl1.Update(); // Needed after calling propertyGrid1.Refresh();
                Globals.needToSave = true;
            }
        }

        private void dropToGround_Click(object sender, EventArgs e)
        {
            dropObjectToGround();
        }
        
        private void AreaButton_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            ushort area = ushort.Parse(item.Text.Substring(item.Text.LastIndexOf(" ")+1));
            trySwitchArea(area);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Globals.needToSave)
            {
                DialogResult saveResult = Prompts.ShowShouldSaveDialog(this);
                e.Cancel = (saveResult == DialogResult.Cancel);
            }
        }

        bool rotObj_Yaw_mouseDown = false;
        int rotObj_Yaw_lastMouseY = 0;
        private void rotObj_Yaw_MouseDown(object sender, MouseEventArgs e)
        {
            if (Globals.item_selected > -1 && Globals.list_selected > -1)
            {
                rotObj_Yaw_lastMouseY = e.Y;
                rotObj_Yaw_mouseDown = true;
            }
        }

        private void rotObj_Yaw_MouseUp(object sender, MouseEventArgs e)
        {
            rotObj_Yaw_mouseDown = false;
            Object3D obj = getSelectedObject();
            if (obj != null)
                obj.updateROMData();
        }

        private void rotObj_Yaw_MouseMove(object sender, MouseEventArgs e)
        {
            if (rotObj_Yaw_mouseDown)
            {
                if (Globals.item_selected > -1 && Globals.list_selected > -1)
                {
                    Object3D obj = getSelectedObject();
                    if (obj == null) return;
                    if (obj.IsReadOnly) return;
                    if (obj.isPropertyShown(Object3D.FLAGS.ROTATION_Y))
                    {
                        obj.yRot -= (short)((e.Y - rotObj_Yaw_lastMouseY) * Globals.objSpeedMultiplier);
                        obj.yRot = keepDegreesWithin360(obj.yRot);
                    }
                    if (camera.isOrbitCamera())
                        camera.updateOrbitCamera(ref camMtx);
                    glControl1.Invalidate();
                    propertyGrid1.Refresh();
                    glControl1.Update(); // Needed after calling propertyGrid1.Refresh();
                    rotObj_Yaw_lastMouseY = e.Y;
                    Globals.needToSave = true;
                }
            }
        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            float newValue;
            if (trackBar3.Value > 50)
                newValue = 100.0f+((trackBar3.Value-50)*8f);
            else
                newValue = (trackBar3.Value/50.0f) * 100f;
            
            if (newValue < 1f)
                newValue = 1f;
            else if (newValue > 96f && newValue < 114f)
                newValue = 100f;

            camSpeedLabel.Text = (int)(newValue) + "%";
            Globals.camSpeedMultiplier = newValue/100.0f;
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {

            float newValue;
            if (trackBar2.Value > 50)
                newValue = 100.0f + ((trackBar2.Value - 50) * 8f);
            
            else
               newValue = (trackBar2.Value / 50.0f) * 100f;
            if (newValue < 1f)
                newValue = 1f;
            else if (newValue > 96f && newValue < 114f)
                newValue = 100f;

            objSpeedLabel.Text = (int)(newValue) + "%";
            Globals.objSpeedMultiplier = newValue / 100.0f;
        }
    }
}
