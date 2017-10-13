using Quad64.src.JSON;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;
using BubblePony.ExportUtility;
using Export = BubblePony.Export;
using OpenTK;
using BubblePony.Alloc;
namespace Quad64
{
	using Internal;
	public enum Object3DCategory : sbyte
	{
		Object,
		MacroObject,
		SpecialObject,
	}

	public sealed class Object3D : Export.Reference<Object3D>,Export.Reference,
		IROMProperty, ILevelProperty, IMemoryProperty
    {
		[Flags]
        public enum FLAGS : uint{
            POSITION_X = 0x1,
            POSITION_Y = 0x2,
            POSITION_Z = 0x4,
            ROTATION_X = 0x8,
            ROTATION_Y = 0x10,
            ROTATION_Z = 0x20,
            ACT1 = 0x40,
            ACT2 = 0x80,
            ACT3 = 0x100,
            ACT4 = 0x200,
            ACT5 = 0x400,
            ACT6 = 0x800,
            ALLACTS = 0x1000,
            BPARAM_1 = 0x2000,
            BPARAM_2 = 0x4000,
            BPARAM_3 = 0x8000,
            BPARAM_4 = 0x10000,
        }

		[Browsable(false)]
		public Level Level => level;
		[Browsable(false)]
		public ByteSegment Memory => memory;
		[Browsable(false)]
		public ROM ROM => level.rom;

		public readonly ByteSegment memory;
		private string mem_adr;
		private const ushort NUM_OF_CATERGORIES = 7;
		private ObjectComboEntry objectComboEntry;
		private ulong Flags = 0;
		private ushort presetID;
		public readonly Object3DCategory Kind;
		private TransformI trs;
		private byte modelID = 0;
		private bool isReadOnly = false;
		void IMemoryProperty.Address(out ByteSegment segment, out ROM_Address address, out string address_string)
		{
			IMemoryPropertyUtility.Address(memory, ref mem_adr, out segment, out address, out address_string);
		}
		bool isBehaviorReadOnly = false;
        bool isModelIDReadOnly = false;
        
        [CustomSortedCategory("Info", 1, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [Description("Name of the object combo")]
        [DisplayName("Combo Name")]
        [ReadOnly(true)]
        public string Title { get; set; }

		[CustomSortedCategory("Info", 1, NUM_OF_CATERGORIES)]
		[Browsable(true)]
		[Description("Location inside the ROM file")]
		[DisplayName("Address")]
		[ReadOnly(true)]
		public string Address => this.GetAddressString();
        
        [CustomSortedCategory("Model", 2, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [Description("Model identifer used by the object")]
        [DisplayName("Model ID")]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public byte ModelID { get { return modelID; } set { modelID = value; } }

        [CustomSortedCategory("Model", 2, NUM_OF_CATERGORIES)]
        [Browsable(false)]
        [Description("Model identifer used by the object")]
        [DisplayName("Model ID")]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        [ReadOnly(true)]
        public byte ModelID_ReadOnly { get { return modelID; } }

        [CustomSortedCategory("Position", 3, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("X")]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public short xPos { get { return trs.translation.X; } set { trs.translation.X = value; } }

        [CustomSortedCategory("Position", 3, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("Y")]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public short yPos { get { return trs.translation.Y; } set { trs.translation.Y = value; } }

        [CustomSortedCategory("Position", 3, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("Z")]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public short zPos { get { return trs.translation.Z; } set { trs.translation.Z = value; } }
        
        [CustomSortedCategory("Rotation", 4, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("RX")]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public short xRot { get { return trs.rotation.X; } set { trs.rotation.X = value; } }

        [CustomSortedCategory("Rotation", 4, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("RY")]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public short yRot { get { return trs.rotation.Y; } set { trs.rotation.Y = value; } }

        [CustomSortedCategory("Rotation", 4, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("RZ")]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public short zRot { get { return trs.rotation.Z; } set { trs.rotation.Z = value; } }

		[Browsable(false)]
		public short yRot_OddMultiply {
			set
			{
				yRot = (short)(value*1.40625);
			}
		}
		[Browsable(false)]
		public sbyte yRot_OddMultiply2
		{
			set
			{
				yRot = (short)(unchecked((byte)value&127) * 2.8125);
			}
		}

		[CustomSortedCategory("Behavior", 5, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("Behavior")]
        //[ReadOnly(true)]
        public string Behavior { get; set; }

        [CustomSortedCategory("Behavior", 5, NUM_OF_CATERGORIES)]
        [Browsable(false)]
        [DisplayName("Behavior")]
        [ReadOnly(true)]
        public string Behavior_ReadOnly { get; set; }
		
        // default names
        private const string BP1DNAME = "B.Param 1";
        private const string BP2DNAME = "B.Param 2";
        private const string BP3DNAME = "B.Param 3";
        private const string BP4DNAME = "B.Param 4";

        [CustomSortedCategory("Behavior", 5, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName(BP1DNAME)]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        [Description("")]
        public byte BehaviorParameter1 { get; set; }

        [CustomSortedCategory("Behavior", 5, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName(BP2DNAME)]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        [Description("")]
        public byte BehaviorParameter2 { get; set; }

        [CustomSortedCategory("Behavior", 5, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName(BP3DNAME)]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        [Description("")]
        public byte BehaviorParameter3 { get; set; }

        [CustomSortedCategory("Behavior", 5, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName(BP4DNAME)]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        [Description("")]
        public byte BehaviorParameter4 { get; set; }

        [CustomSortedCategory("Acts", 6, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("All Acts")]
        public bool AllActs { get; set; }
        [CustomSortedCategory("Acts", 6, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("Act 1")]
        public bool Act1 { get; set; }
        [CustomSortedCategory("Acts", 6, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("Act 2")]
        public bool Act2 { get; set; }
        [CustomSortedCategory("Acts", 6, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("Act 3")]
        public bool Act3 { get; set; }
        [CustomSortedCategory("Acts", 6, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("Act 4")]
        public bool Act4 { get; set; }
        [CustomSortedCategory("Acts", 6, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("Act 5")]
        public bool Act5 { get; set; }
        [CustomSortedCategory("Acts", 6, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("Act 6")]
        public bool Act6 { get; set; }

        [CustomSortedCategory("Misc", NUM_OF_CATERGORIES, NUM_OF_CATERGORIES)]
        [DisplayName("Read-Only")]
        [Browsable(false)]
        public bool IsReadOnly { get { return isReadOnly; } }

		/**************************************************************************************/
		
		public readonly Level level;

		public int getROMAddress()
        {
            return int.Parse(Address.Substring(2), NumberStyles.HexNumber);
        }

        public uint getROMUnsignedAddress()
        {
            return uint.Parse(Address.Substring(2), NumberStyles.HexNumber);
        }

        public void setPresetID(ushort ID)
        {
            presetID = ID;
        }

        public byte getActMask()
        {
            byte actMask = 0;
            if (Act1) actMask |= 0x1;
            if (Act2) actMask |= 0x2;
            if (Act3) actMask |= 0x4;
            if (Act4) actMask |= 0x8;
            if (Act5) actMask |= 0x10;
            if (Act6) actMask |= 0x20;
            return actMask;
        }

        public void setBehaviorFromAddress(uint address)
        {
            Behavior = "0x"+address.ToString("X8");
            Behavior_ReadOnly = Behavior;
        }

        public uint getBehaviorAddress()
        {
            return uint.Parse(Behavior.Substring(2), NumberStyles.HexNumber);
        }

        public void updateROMData()
        {
            //if (Address.Equals("N/A")) return;
			if (Globals.list_selected != (byte)Kind) throw new System.InvalidOperationException();
			var rom = ROM;
			bool m = false;
            //ROM rom = ROM.Instance;
            //uint romAddr = getROMUnsignedAddress();
            if (Globals.list_selected == 0) // Regular Object
            {
                rom.writeByte(ref m,memory, 2, getActMask());
                rom.writeByte(ref m,memory, 3, ModelID);
                rom.writeHalfword(ref m,memory, 4, xPos);
                rom.writeHalfword(ref m,memory, 6, yPos);
                rom.writeHalfword(ref m,memory, 8, zPos);
                rom.writeHalfword(ref m,memory, 10, xRot);
                rom.writeHalfword(ref m,memory, 12, yRot);
                rom.writeHalfword(ref m, memory, 14, zRot);
                rom.writeByte(ref m,memory, 16, BehaviorParameter1);
                rom.writeByte(ref m,memory, 17, BehaviorParameter2);
                rom.writeByte(ref m,memory, 18, BehaviorParameter3);
                rom.writeByte(ref m,memory, 19, BehaviorParameter4);
                rom.writeWord(ref m, memory, 20, getBehaviorAddress());

			}
			else if (Globals.list_selected == 1) // Macro Object
            {
                //Console.WriteLine("Preset ID = 0x" + presetID.ToString("X"));
                ushort first = (ushort)((((ushort)(yRot / 2.8125f) & 0x7F) << 9) | (presetID & 0x1FF));
                rom.writeHalfword(ref m,memory, 0, first);
                rom.writeHalfword(ref m,memory, 2, xPos);
                rom.writeHalfword(ref m,memory, 4, yPos);
                rom.writeHalfword(ref m, memory, 6, zPos);
                rom.writeByte(ref m, memory, 8, BehaviorParameter1);
				rom.writeByte(ref m, memory, 9, BehaviorParameter2);

			}
			else if (Globals.list_selected == 2) // Special Object
            {
                Console.WriteLine("Special Preset ID = 0x" + presetID.ToString("X"));
				rom.writeHalfword(ref m,memory, 0, presetID);
                rom.writeHalfword(ref m,memory, 2, xPos);
                rom.writeHalfword(ref m,memory, 4, yPos);
                rom.writeHalfword(ref m, memory, 6, zPos);
                if (!isHidden(FLAGS.ROTATION_Y))
                {
					rom.writeHalfword(ref m, memory, 8, (short)(yRot / 1.40625f));
					if (!isHidden(FLAGS.BPARAM_1))
					{
						rom.writeByte(ref m, memory, 10, BehaviorParameter1);
						rom.writeByte(ref m, memory, 11, BehaviorParameter2);
					}
				}
			}
			rom.wrote(m, memory);
		}

        public void MakeBehaviorReadOnly(bool isReadOnly)
        {
            isBehaviorReadOnly = isReadOnly;
        }

        public void MakeModelIDReadOnly(bool isReadOnly)
        {
            isModelIDReadOnly = isReadOnly;
        }

        public void MakeReadOnly()
        {
            TypeDescriptor.AddAttributes(this, new Attribute[] { new ReadOnlyAttribute(true) });
            isReadOnly = true;
        }

        
        private bool isHidden(FLAGS flag)
        {
            return (Flags & (ulong)flag) == (ulong)flag;
        }

        private void updateProperty(PropertyManipulator property, FLAGS flag)
        {
            if (isHidden(flag))
                Manipulator.HideShowProperty(property, false);
            else
				Manipulator.HideShowProperty(property, true);
        }
		private void updateProperty(string property, FLAGS flag) { updateProperty(Manipulator[property], flag); }
		private static class ReadonlySuffix
		{
			private static readonly System.Collections.Generic.Dictionary<string, string> values = new System.Collections.Generic.Dictionary<string, string>();
			private static object thread_lock = new object();
			public static string Get(string property)
			{
				string cached;
				lock (thread_lock)
					if (!values.TryGetValue(property, out cached))
					{
						cached = property + "_ReadOnly";
						values[property] = cached;
					}
				return cached;
			}
		}

        private void updateReadOnlyProperty(PropertyManipulator property, bool isReadOnly)
        {
            if (isReadOnly)
            {
                Manipulator.HideShowProperty(property, false);
                Manipulator.HideShowProperty(property.GetReadonlyPropLookup(), true);
            }
            else
            {
                Manipulator.HideShowProperty(property, true);
                Manipulator.HideShowProperty(property.GetReadonlyPropLookup(), false);
            }
		}
		private void updateReadOnlyProperty(string property, bool isReadOnly)
		{
			updateReadOnlyProperty(Manipulator[property], isReadOnly);
		}

		private static class ManipulatorInstance
		{
			public static readonly TypeManipulator Manipulator = new TypeManipulator(typeof(Object3D));
		}
		internal static TypeManipulator Manipulator => ManipulatorInstance.Manipulator;
		internal static class PROP
		{
			public static readonly PropertyManipulator
				BehaviorParameter1 = Manipulator["BehaviorParameter1"],
				BehaviorParameter2 = Manipulator["BehaviorParameter2"],
				BehaviorParameter3 = Manipulator["BehaviorParameter3"],
				BehaviorParameter4 = Manipulator["BehaviorParameter4"],
				xRot = Manipulator["xRot"],
				yRot = Manipulator["yRot"],
				zRot = Manipulator["zRot"],
				Act1 = Manipulator["Act1"],
				Act2 = Manipulator["Act2"],
				Act3 = Manipulator["Act3"],
				Act4 = Manipulator["Act4"],
				Act5 = Manipulator["Act5"],
				Act6 = Manipulator["Act6"],
				AllActs = Manipulator["AllActs"],
				Behavior = Manipulator["Behavior"],
				ModelID = Manipulator["ModelID"];
		}
#if NO_NEVERMIND
		private static class PLArray
		{
			public static readonly PropertyManipulator[] All;
			static PLArray()
			{
				var Fields = typeof(PROP).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				var Temp = new PropertyManipulator[Fields.Length];
				int i, n, P = 0;
				for (i = 0, n = Fields.Length, P = 0; i < n; ++i)
					if (Fields[i].FieldType == typeof(PropertyManipulator))
						Temp[P++] = (PropertyManipulator)Fields[i].GetValue(null);
				if (n != P) System.Array.Resize(ref Temp, P);

				All = Temp;
			}
		}
#endif
		private void UpdateObjectComboNames()
        {
            if (objectComboEntry != null)
            {
                Manipulator.UpdatePropertyName(PROP.BehaviorParameter1, objectComboEntry.BP1_NAME, BP1DNAME);
                Manipulator.UpdatePropertyName(PROP.BehaviorParameter2, objectComboEntry.BP2_NAME, BP2DNAME);
                Manipulator.UpdatePropertyName(PROP.BehaviorParameter3, objectComboEntry.BP3_NAME, BP3DNAME);
                Manipulator.UpdatePropertyName(PROP.BehaviorParameter4, objectComboEntry.BP4_NAME, BP4DNAME);
                Manipulator.UpdatePropertyDescription(PROP.BehaviorParameter1, objectComboEntry.BP1_DESCRIPTION);
                Manipulator.UpdatePropertyDescription(PROP.BehaviorParameter2, objectComboEntry.BP2_DESCRIPTION);
                Manipulator.UpdatePropertyDescription(PROP.BehaviorParameter3, objectComboEntry.BP3_DESCRIPTION);
                Manipulator.UpdatePropertyDescription(PROP.BehaviorParameter4, objectComboEntry.BP4_DESCRIPTION);
            }
            else
            {
                Manipulator.ChangePropertyName(PROP.BehaviorParameter1, BP1DNAME);
				Manipulator.ChangePropertyName(PROP.BehaviorParameter2, BP2DNAME);
				Manipulator.ChangePropertyName(PROP.BehaviorParameter3, BP3DNAME);
				Manipulator.ChangePropertyName(PROP.BehaviorParameter4, BP4DNAME);
                Manipulator.ChangePropertyDescription(PROP.BehaviorParameter1, string.Empty);
				Manipulator.ChangePropertyDescription(PROP.BehaviorParameter2, string.Empty);
				Manipulator.ChangePropertyDescription(PROP.BehaviorParameter3, string.Empty);
				Manipulator.ChangePropertyDescription(PROP.BehaviorParameter4, string.Empty);
            }
        }

        public void UpdateProperties()
        {
            updateProperty(PROP.xRot, FLAGS.ROTATION_X);
            updateProperty(PROP.yRot, FLAGS.ROTATION_Y);
            updateProperty(PROP.zRot, FLAGS.ROTATION_Z);
            updateProperty(PROP.Act1, FLAGS.ACT1);
            updateProperty(PROP.Act2, FLAGS.ACT2);
            updateProperty(PROP.Act3, FLAGS.ACT3);
            updateProperty(PROP.Act4, FLAGS.ACT4);
            updateProperty(PROP.Act5, FLAGS.ACT5);
            updateProperty(PROP.Act6, FLAGS.ACT6);
            updateProperty(PROP.AllActs, FLAGS.ALLACTS);
            updateProperty(PROP.BehaviorParameter1, FLAGS.BPARAM_1);
            updateProperty(PROP.BehaviorParameter2, FLAGS.BPARAM_2);
            updateProperty(PROP.BehaviorParameter3, FLAGS.BPARAM_3);
            updateProperty(PROP.BehaviorParameter4, FLAGS.BPARAM_4);
            updateReadOnlyProperty(PROP.Behavior, isBehaviorReadOnly);
            updateReadOnlyProperty(PROP.ModelID, isModelIDReadOnly);
            UpdateObjectComboNames();
        }

        public void SetBehaviorParametersToZero()
        {
            BehaviorParameter1 = 0;
            BehaviorParameter2 = 0;
            BehaviorParameter3 = 0;
            BehaviorParameter4 = 0;
        }

        public void DontShowActs()
        {
            Flags |= (ulong)(
                FLAGS.ACT1 | FLAGS.ACT2 | FLAGS.ACT3 | 
                FLAGS.ACT4 | FLAGS.ACT5 | FLAGS.ACT6 |
                FLAGS.ALLACTS);
        }

        public void ShowHideActs(bool hide)
        {
            if (hide)
                Flags |= (ulong)(FLAGS.ACT1 | FLAGS.ACT2 | 
                    FLAGS.ACT3 | FLAGS.ACT4 | FLAGS.ACT5 | FLAGS.ACT6);
            else
                Flags &= ~(ulong)(FLAGS.ACT1 | FLAGS.ACT2 |
                    FLAGS.ACT3 | FLAGS.ACT4 | FLAGS.ACT5 | FLAGS.ACT6);
            UpdateProperties();
        }

        public void HideProperty(FLAGS flag)
        {
            Flags |= (ulong)flag;
        }

        public string getObjectComboName()
        {
            uint behaviorAddr = getBehaviorAddress();
            for (int i = 0; i < Globals.objectComboEntries.Count; i++)
            {
                ObjectComboEntry entry = Globals.objectComboEntries[i];
                uint modelSegmentAddress = 0;
                if (level.ModelIDs.ContainsKey(ModelID))
                    modelSegmentAddress = level.ModelIDs[ModelID].GeoDataSegAddress;
                if (entry.ModelID == ModelID && entry.Behavior == behaviorAddr && entry.ModelSegmentAddress == modelSegmentAddress)
                {
                    objectComboEntry = entry;
                    return entry.Name;
                }
            }
            return "Unknown Combo";
        }

        public bool isPropertyShown(FLAGS flag)
        {
            return !isHidden(flag);
        }

		[Browsable(false)]
		public TransformI transform { get { return trs; } }

		public bool isBillboard;
		/// <summary>
		/// this should be used for picking..
		/// </summary>
		public void LoadTransform(out Transform transform)
		{
			trs.LoadTransform(out transform);
		}
		/// <summary>
		/// this should be used for rendering/drawing.
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="camera"></param>
		public void LoadTransform(out Transform transform, ref RenderCamera camera)
		{
			trs.LoadTransform(out transform);
			if (isBillboard) camera.MakeBillboard(ref transform);
		}
		[Browsable(false)]
		public Vector3s pos { get { return trs.translation; } set { trs.translation = value; } }
		[Browsable(false)]
		public Vector3s rot { get { return trs.rotation; } set { trs.rotation = value; } }
		[Browsable(false)]
		public Quaternion quat => trs.Rotation;
		const int data_size =
			sizeof(ulong) +
			sizeof(uint)+ 
			sizeof(short) * 6 +
			sizeof(ushort) * 1 + 
			sizeof(byte) * 6;
		public static Export.ReferenceRegister<Object3D> ExportRegister;
		Export.TypeReference Export.Reference.API() { return ExportRegister.Singleton; }
		unsafe void Export.Reference<Object3D>.API(Export.Exporter ex)
		{
			{
				byte* data = stackalloc byte[data_size];
				ulong* ulng = (ulong*)data;
				*ulng++ = Flags;
				uint* unt = (uint*)ulng;
				*unt++ = getBehaviorAddress();
				short* shrt = (short*)unt;
				*shrt++ = trs.translation.X;
				*shrt++ = trs.translation.Y;
				*shrt++ = trs.translation.Z;
				*shrt++ = trs.rotation.X;
				*shrt++ = trs.rotation.Y;
				*shrt++ = trs.rotation.Z;
				ushort* ushrt = (ushort*)shrt;
				*ushrt++ = presetID;
				byte* uchr = (byte*)ushrt;
				*uchr++ = modelID;//1
				*uchr++ = getActMask();//2
				*uchr++ = BehaviorParameter1;//3
				*uchr++ = BehaviorParameter2;//4
				*uchr++ = BehaviorParameter3;//5
				*uchr++ = BehaviorParameter4;//6
				if ((uint)(uchr - data) != data_size) throw new System.InvalidOperationException();

				ex.Value(data, (uint)data_size);
			}
			ex.Ref(level);

		}
		public Object3D(Level level, ByteSegment Memory, Object3DCategory Kind)
		{
			this.level = level;
			this.Kind = Kind;
			this.memory = Memory;
			this.trs.scale.Whole = 1;
		}
		/// <summary>
		/// called prior to adding to any object list. should return self.
		/// </summary>
		/// <returns></returns>
		internal Object3D PreAdd()
		{
			isBillboard |= "0x13002AA4" == Behavior;
			return this;
		}
		public sealed override string ToString()
		{
			return string.Format("{0}({1}:{2})", Title,
				0 == Kind ? "Object" : 1 == (sbyte)Kind ? "Macro" : 2 == (sbyte)Kind ? "Special" : "?",
				Address??string.Empty);
		}
	}
}
