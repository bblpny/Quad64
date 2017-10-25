using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using BubblePony.ExportUtility;
using Export = BubblePony.Export;
using BubblePony.Alloc;

namespace Quad64
{
    public sealed class WarpInstant : Export.Reference<WarpInstant>, Export.Reference,
		IMemoryProperty, ILevelProperty, IROMProperty, IAreaProperty

	{
		public readonly Area area;

		public readonly ByteSegment memory;
		[Browsable(false)]
		public Level Level => area.level;
		[Browsable(false)]
		public ROM ROM => area.level.rom;
		[Browsable(false)]
		public Area Area => area;
		private string mem_adr;
		[Browsable(false)]
		public ByteSegment Memory => memory;

		private const ushort NUM_OF_CATERGORIES = 2;

        private byte triggerID;
        [CustomSortedCategory("Instant Warp", 1, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("Trigger ID")]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public byte TriggerID { get { return triggerID; } set { triggerID = value; } }

        private byte areaID;
        [CustomSortedCategory("Instant Warp", 1, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("To Area")]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public byte AreaID { get { return areaID; } set { areaID = value; } }

        private Vector3s tele;

        [CustomSortedCategory("Instant Warp", 1, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("Teleport X")]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public short TeleX { get { return tele.X; } set { tele.X = value; } }
		
        [CustomSortedCategory("Instant Warp", 1, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("Teleport Y")]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public short TeleY { get { return tele.Y; } set { tele.Y = value; } }
		
        [CustomSortedCategory("Instant Warp", 1, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("Teleport Z")]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public short TeleZ { get { return tele.Z; } set { tele.Z = value; } }

		[Browsable(false)]
		public Vector3s Tele { get => tele; set => tele = value; }

		[CustomSortedCategory("Info", 2, NUM_OF_CATERGORIES)]
		[Browsable(true)]
		[Description("Location inside the ROM file")]
		[DisplayName("Address")]
		[ReadOnly(true)]
		public string Address => this.GetAddressString();
		public WarpInstant(Area area, ByteSegment memory)
		{
			if (null == (object)area) throw new ArgumentNullException("area");
			if (0 == memory.Length) throw new ArgumentException("length is zero", "memory");

			this.area = area;
			this.memory = memory;
		}
        public void MakeReadOnly()
        {
            TypeDescriptor.AddAttributes(this, new Attribute[] { new ReadOnlyAttribute(true) });
        }
		
		void IMemoryProperty.Address(out ByteSegment segment, out ROM_Address address, out string address_string)
		{
			IMemoryPropertyUtility.Address(memory, ref mem_adr, out segment, out address, out address_string);
		}
		public int getROMAddress()
        {
            return int.Parse(Address.Substring(2), NumberStyles.HexNumber);
        }

        public uint getROMUnsignedAddress()
        {
            return uint.Parse(Address.Substring(2), NumberStyles.HexNumber);
        }
		

        public void updateROMData()
        {
            //if (Address.Equals("N/A")) return;
            //ROM rom = ROM.Instance;
            //uint romAddr = getROMUnsignedAddress();
			var rom = ROM;
			bool m = false;
            rom.writeByte(ref m,memory, 2, TriggerID);
            rom.writeByte(ref m, memory, 3, AreaID);
            rom.writeHalfword(ref m, memory, 4, TeleX);
            rom.writeHalfword(ref m, memory, 6, TeleY);
            rom.writeHalfword(ref m, memory, 8, TeleZ);
			rom.wrote(m, memory);

		}

		private string getWarpName()
        {
            return " [to Area "+AreaID+"]";
        }

        public override string ToString()
        {
            //isPaintingWarp
            string warpName = "Instant Warp 0x";

            warpName += TriggerID.ToString("X2") + getWarpName();
            
            return warpName;
		}
		const uint dat_size = sizeof(byte) * 2 + sizeof(short) * 3 + sizeof(uint);
		public static Export.ReferenceRegister<WarpInstant> ExportRegister;
		Export.TypeReference Export.Reference.API() { return ExportRegister.Singleton; }
		unsafe void Export.Reference<WarpInstant>.API(Export.Exporter ex)
		{
			byte* dat = stackalloc byte[(int)dat_size];
			byte* uchr = dat;

			*uchr++ = this.areaID;
			*uchr++ = this.triggerID;
			short* shrt = (short*)uchr;
			*shrt++ = this.tele.X;
			*shrt++ = this.tele.Y;
			*shrt++ = this.tele.Z;
			*((uint*)shrt) = this.getROMUnsignedAddress();
			ex.Value(dat, dat_size);
		}
	}
}
