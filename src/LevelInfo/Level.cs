using OpenTK;
using Quad64.Scripts;
using Quad64.src.JSON;
using Quad64.src.Viewer;
using System;
using System.Collections.Generic;
using BubblePony.ExportUtility;
using Export = BubblePony.Export;
using BubblePony.Alloc;

namespace Quad64
{

	public sealed class Area : IROMProperty, ILevelProperty, IModel3DProperty, Export.Reference<Area>, Export.Reference
	{
		public readonly Level level;
		Level ILevelProperty.Level => level;
		Model3D IModel3DProperty.Model3D => AreaModel;
		public ROM ROM => level.rom;
		public TransformI[] startPoints = null;
		public readonly Model3D AreaModel = new Model3D();
		public readonly CollisionMap collision = new CollisionMap();

		public readonly List<Object3D> Objects = new List<Object3D>();
		public readonly List<Object3D> MacroObjects = new List<Object3D>();
		public readonly List<Object3D> SpecialObjects = new List<Object3D>();
		public readonly List<WaterBlock> WaterBlocks = new List<WaterBlock>();
		public readonly List<Warp> Warps = new List<Warp>();
		public readonly List<Warp> PaintingWarps = new List<Warp>();
		public readonly List<WarpInstant> InstantWarps = new List<WarpInstant>();
		

		public readonly uint GeometryLayoutPointer;
		public readonly ushort AreaID;

		public List<Object3D> this[int list_index] => 
			list_index == 0 ? Objects : list_index == 1 ? MacroObjects : list_index == 2 ? SpecialObjects : list_index<0?null:this[(list_index) % 3];

		public Object3D this[int list_index, int object_index] => 
			(object_index<0||list_index<0)?null:this[list_index][object_index];

        public Area(ushort areaID, uint geoLayoutPointer, Level parent)
        {
            this.AreaID = areaID;
            this.GeometryLayoutPointer = geoLayoutPointer;
            this.level = parent;
        }
		
		const int ObjectColorB = 1;
		const int MacroObjectColorB = 2;
		const int SpecialObjectColorB = 3;

        private bool isObjectSelected(int list, int obj)
        {
            if(list == Globals.list_selected && obj == Globals.item_selected)
                return true;
            return false;
		}
		public static Color4b IndexColor(int i, int b)
		{
			return new Color4b((byte)(i & 255), (byte)((i >> 8)&255), (byte)(b&255));
		}
		public static void drawPickingObjects<T>(Level parent, ref T Enumerator, int BColor)
			where T : IEnumerator<Object3D>
		{
			Object3D obj;
			Model3D model;
			int Index = -1;
			byte ModelID;
			bool hasParent = null != (object)parent;
			while (Enumerator.MoveNext())
				if (null != (object)(obj = Enumerator.Current))
					if ((ModelID = obj.ModelID) != 0)
						if (!hasParent || !parent.ModelIDs.TryGetValue(ModelID, out model))
							Index++;
						else
							BoundingBox.draw_solid(
								obj.transform,
								IndexColor(++Index, BColor),
								model);
					else
						BoundingBox.draw_solid(obj.transform, IndexColor(++Index, ObjectColorB));
				else
					Index++;
		}
		public static void drawPickingObjects(Level parent, List<Object3D> objects, int BColor)
		{
			if (objects != null)
			{
				var Enumerator = objects.GetEnumerator();
				drawPickingObjects(parent, ref Enumerator, BColor);
				Enumerator.Dispose();
			}
		}
		public static void drawPickingObjects(Level parent, IEnumerable<Object3D> objects, int BColor)
		{
			if (objects != null)
			{
				var Enumerator = objects.GetEnumerator();
				if (Enumerator != null)
				{
					drawPickingObjects(parent, ref Enumerator, BColor);
					Enumerator.Dispose();
				}
			}
		}

		public void drawPicking()
		{
			drawPickingObjects(level, Objects, ObjectColorB);
			drawPickingObjects(level, MacroObjects, MacroObjectColorB);
			drawPickingObjects(level, SpecialObjects, SpecialObjectColorB);
		}
		public void drawPicking(Transform transform)
		{
			if (transform != Transform.Identity)
			{
				OpenTK.Graphics.OpenGL.GL.PushMatrix();
				transform.GL_Load();
				drawPicking();
				OpenTK.Graphics.OpenGL.GL.PopMatrix();
			}
			else
			{
				drawPicking();
			}
		}

		public void drawEverything(GraphicsInterface gi, ref RenderCamera camTrs)
		{
			Object3D obj;
			Model3D model;
			Transform transform;
			List<Object3D> list;
			int i,ilist;
			byte modelId;
			bool selected;
			if (Globals.renderCollisionMap)
                collision.drawCollisionMap(false);
            else
                AreaModel.drawModel(gi,ref camTrs);

            for (i = Objects.Count-1; i >= 0; --i)
            {
				obj = Objects[i];

				obj.LoadTransform(out transform, ref camTrs);

				// Need to slighting increase the model's size, just in-case of overlapping bounding boxes.
				if ((selected=isObjectSelected(0, i)))
                    transform.scale *=1.001f;

				if ((modelId = obj.ModelID) != 0)
				{
					if (!level.ModelIDs.TryGetValue(modelId, out model))
						continue;
					if (Globals.drawObjectModels)
						model.drawModel(gi,transform, ref camTrs);

					BoundingBox.draw(transform, selected ? Globals.SelectedObjectColor : Globals.ObjectColor,
						model);
				}
				else
				{
					BoundingBox.draw(transform,
						selected ? Globals.SelectedObjectColor : Globals.ObjectColor);
				}
            }
			// macro objects, then special objects.
			for (selected = false, ilist =1,list=MacroObjects;ilist!=3;list=SpecialObjects,++ilist)
			for (i = list.Count-1; i>=0; --i)
			{
				obj = list[i];
				if ((modelId = obj.ModelID) != 0)
				{
					if (!level.ModelIDs.TryGetValue(modelId, out model))
						continue;

					obj.LoadTransform(out transform, ref camTrs);

					if (Globals.drawObjectModels)
						model.drawModel(gi,transform, ref camTrs);

					BoundingBox.draw(transform,
						isObjectSelected(ilist, i) ? Globals.SelectedObjectColor : Globals.MacroObjectColor,
						model);
				}
				else
				{
					BoundingBox.draw(obj.transform,
						isObjectSelected(ilist, i) ? Globals.SelectedObjectColor : Globals.MacroObjectColor);
				}
            }
        }
		public void drawEverything(GraphicsInterface gi,Transform transform, ref RenderCamera camTrs)
		{
			if (transform != Transform.Identity)
			{
				OpenTK.Graphics.OpenGL.GL.PushMatrix();
				transform.GL_Load();
				drawEverything(gi,ref camTrs);
				OpenTK.Graphics.OpenGL.GL.PopMatrix();
			}
			else
			{
				drawEverything(gi,ref camTrs);
			}
		}

		public void renderEverything(RenderList layer)
		{
			Object3D obj;
			Model3D model;
			Transform transform;
			List<Object3D> list;
			int i, ilist;
			byte modelId;
			bool selected;
			if (!Globals.renderCollisionMap)
				layer.Render(Transform.Identity,this,AreaModel,System.Drawing.Color.Transparent);

			for (i = Objects.Count - 1; i >= 0; --i)
			{
				obj = Objects[i];
				
				obj.LoadTransform(out transform, ref layer.Camera);

				// Need to slighting increase the model's size, just in-case of overlapping bounding boxes.
				if ((selected = isObjectSelected(0, i)))
					transform.scale *= 1.001f;

				if ((modelId = obj.ModelID) != 0)
				{
					if (!level.ModelIDs.TryGetValue(modelId, out model))
						continue;

					layer.Render(
						ref transform,
						obj,
						model,
						selected ? Globals.SelectedObjectColor : Globals.ObjectColor);
				}
				else
				{
					layer.Render(
						ref transform,
						obj,
						null,
						selected ? Globals.SelectedObjectColor : Globals.ObjectColor);
				}
			}
			// macro objects, then special objects.
			for (selected = false, ilist = 1, list = MacroObjects; ilist != 3; list = SpecialObjects, ++ilist)
			for ( i = list.Count - 1; i >= 0; --i)
				{
					obj = list[i];
					if ((modelId = obj.ModelID) != 0)
					{
						if (!level.ModelIDs.TryGetValue(modelId, out model))
							continue;

						obj.LoadTransform(out transform, ref layer.Camera);

						layer.Render(ref transform, obj, model,
							isObjectSelected(ilist, i) ? Globals.SelectedObjectColor : Globals.MacroObjectColor);
					}
					else
					{
						obj.LoadTransform(out transform, ref layer.Camera);
						layer.Render(ref transform, obj, null,
							isObjectSelected(ilist, i) ? Globals.SelectedObjectColor : Globals.MacroObjectColor);
					}
				}
		}

		public static Export.ReferenceRegister<Area> ExportRegister;
		Export.TypeReference Export.Reference.API() { return ExportRegister.Singleton; }
		void Export.Reference<Area>.API(Export.Exporter ex)
		{
			ex.Value(AreaID);
			ex.Value(GeometryLayoutPointer);
			ex.Ref(level);
			ex.Ref(AreaModel);
			ex.Ref(collision);
			ex.RefArray(Objects.ToArray());
			ex.RefArray(MacroObjects.ToArray());
			ex.RefArray(SpecialObjects.ToArray());
			ex.RefArray(Warps.ToArray());
			ex.RefArray(PaintingWarps.ToArray());
			ex.RefArray(InstantWarps.ToArray());
		}
	}

    public sealed class Level : Export.Reference<Level>, Export.Reference, IROMProperty, ILevelProperty, IMemoryProperty
    {
        private ushort levelID;
        public ushort LevelID { get { return levelID; } }
        private int _currentAreaID;
		private string mem_adr;
		public readonly ROM rom;
		public readonly ByteSegment memory;
		public ROM ROM => rom;
		Level ILevelProperty.Level => this;
		public ByteSegment Memory => memory;
		void IMemoryProperty.Address(out ByteSegment segment, out ROM_Address address, out string address_string)
		{
			IMemoryPropertyUtility.Address(memory, ref mem_adr, out segment, out address, out address_string);
		}

		private readonly WeakReference weakArea = new WeakReference(null);
        public ushort CurrentAreaID {
			get
			{
				return _currentAreaID < 0 ? (ushort)(-((1) + _currentAreaID)) : (ushort)_currentAreaID;
			}
			set
			{
				if (_currentAreaID != value)
				{
					foreach (var a in Areas)
						if (a.AreaID == value) {
							_currentAreaID = value;
							weakArea.Target = a;
						}

					weakArea.Target = null;
					_currentAreaID = (-1) - (int)value;
				}
			}
		}
        public readonly List<Area> Areas = new List<Area>();
        public readonly Dictionary<ushort, Model3D> ModelIDs = new Dictionary<ushort, Model3D>();

        internal readonly List<ObjectComboEntry> LevelObjectCombos = new List<ObjectComboEntry>();
		internal readonly List<PresetMacroEntry> MacroObjectPresets = new List<PresetMacroEntry>();
		internal readonly List<PresetMacroEntry> SpecialObjectPresets_8 = new List<PresetMacroEntry>();
		internal readonly List<PresetMacroEntry> SpecialObjectPresets_10 = new List<PresetMacroEntry>();
		internal readonly List<PresetMacroEntry> SpecialObjectPresets_12 = new List<PresetMacroEntry>();

        internal ObjectComboEntry getObjectComboFromData(byte modelID, uint modelAddress, uint behavior, out int index)
        {
            for (int i = 0; i < LevelObjectCombos.Count; i++)
            {
                ObjectComboEntry oce = LevelObjectCombos[i];
                if (oce.ModelID == modelID && oce.ModelSegmentAddress == modelAddress
                    && oce.Behavior == behavior)
                {
                    index = i;
                    return oce;
                }
            }
            index = -1;
            return null;
        }

        private void AddMacroObjectEntries()
        {
            MacroObjectPresets.Clear();
            //ROM rom = ROM.Instance;
            ushort pID = 0x1F;
            for (int i = 0; i < 366; i++)
            {
                uint offset = (uint)(Globals.macro_preset_table + (i * 8));
                byte modelID = rom.readByte(offset + 5);
                uint behavior = rom.readWordUnsigned(offset);
                byte bp1 = rom.readByte(offset + 6);
                byte bp2 = rom.readByte(offset + 7);
                MacroObjectPresets.Add(new PresetMacroEntry(pID, modelID, behavior, bp1, bp2));
                pID++;
            }
        }

        public void AddSpecialObjectPreset_8(ushort presetID, byte modelId, uint behavior)
        {
            SpecialObjectPresets_8.Add(new PresetMacroEntry(presetID, modelId, behavior));
        }

        public void AddSpecialObjectPreset_10(ushort presetID, byte modelId, uint behavior)
        {
            SpecialObjectPresets_10.Add(new PresetMacroEntry(presetID, modelId, behavior));
        }

        public void AddSpecialObjectPreset_12(ushort presetID, byte modelId, uint behavior, byte bp1, byte bp2)
        {
            SpecialObjectPresets_12.Add(new PresetMacroEntry(presetID, modelId, behavior, bp1, bp2));
        }

        public void AddObjectCombos(byte modelId, uint modelSegAddress)
        {
            for (int i = 0; i < Globals.objectComboEntries.Count; i++)
            {
                ObjectComboEntry oce = Globals.objectComboEntries[i];
                if (oce.ModelID == modelId && oce.ModelSegmentAddress == modelSegAddress)
                    LevelObjectCombos.Add(oce);
            }
        }

        public void sortAndAddNoModelEntries()
        {
            for (int i = 0; i < Globals.objectComboEntries.Count; i++)
            {
                ObjectComboEntry oce = Globals.objectComboEntries[i];
                if (oce.ModelID == 0x00)
                    LevelObjectCombos.Add(oce);
            }
            LevelObjectCombos.Sort((x, y) => string.Compare(x.Name, y.Name));
        }

        public void printLevelObjectCombos()
        {
            for (int i = 0; i < LevelObjectCombos.Count; i++)
                Console.WriteLine(LevelObjectCombos[i].ToString());
        }

        public bool hasArea(ushort areaID)
        {
			Area af;

			if (areaID == _currentAreaID)
				if( null == (af = (Area)weakArea.Target) || !Areas.Contains(af))
				{
					_currentAreaID = (-1) - _currentAreaID;
				}
				else
					return true;

			foreach (Area a in Areas)
			{
				if (a.AreaID == areaID)
				{
					if (_currentAreaID < 0 && areaID == ((-1) - _currentAreaID))
					{
						weakArea.Target = a;
						_currentAreaID = areaID;
					}
					return true;
				}
			}

            return false;
        }

        public Area getCurrentArea()
        {
			Area af;
			ushort _areaMatch;
			if (_currentAreaID >= 0)
			{
				if (null == (af = ((Area)weakArea.Target)) || !Areas.Contains(af))
					_currentAreaID = (-1) - (_areaMatch=((ushort)_currentAreaID));
				else
					return af;
			}
			else
			{
				_areaMatch = (ushort)((-1) - _currentAreaID);
			}

			foreach (Area a in Areas)
				if (a.AreaID == _areaMatch)
				{
					weakArea.Target = a;
					_currentAreaID = _areaMatch;
					return a;
				}

            return Areas[0]; // return default area
        }

		public static Export.ReferenceRegister<Level> ExportRegister;
		Export.TypeReference Export.Reference.API() { return ExportRegister.Singleton; }
		void Export.Reference<Level>.API(Export.Exporter ex)
		{
			ex.Value(levelID);
			ex.RefArray(Areas);
		}

		public Level(ROM rom, ByteSegment memory, ushort levelID, ushort startArea) {
			if (null == (object)rom) throw new ArgumentNullException("rom");
			if (memory.Length == 0) throw new ArgumentException("zero in size", "memory");
			this.rom = rom;
			this.memory = memory;
            this.levelID = levelID;
            this._currentAreaID = (-1) - startArea;
            LevelObjectCombos.Clear();
            AddMacroObjectEntries();
        }
    }
}
