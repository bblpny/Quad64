using System;
using System.ComponentModel;
using BubblePony.ExportUtility;
using Export = BubblePony.Export;
using BubblePony.Alloc;
namespace Quad64
{
    public sealed class Warp : Export.Reference<Warp>, Export.Reference, IROMProperty,IAreaProperty, ILevelProperty,IMemoryProperty
	{
		public readonly Area area;
		private string mem_adr;
		public readonly ByteSegment memory;
		[Browsable(false)]
		public Area Area => area;

		[Browsable(false)]
		public Level Level => area.level;
		[Browsable(false)]
		public ROM ROM => area.level.rom;
		[Browsable(false)]
		public ByteSegment Memory => memory;
		void IMemoryProperty.Address(out ByteSegment segment, out ROM_Address address, out string address_string)
		{
			IMemoryPropertyUtility.Address(memory, ref mem_adr, out segment, out address, out address_string);
		}

		public Warp(Area area, ByteSegment memory,bool isPaintingWarp)
        {
			if (null == (object)area) throw new ArgumentNullException("area");
			if (0 == memory.Length) throw new ArgumentException("length is zero", "memory");
			this.area = area;
			this.memory = memory;
            this.isPaintingWarp = isPaintingWarp;
        }
        
        private const ushort NUM_OF_CATERGORIES = 2;
        private bool isPaintingWarp = false;

        private byte warpFrom_ID;

        [CustomSortedCategory("Connect Warps", 1, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("From ID")]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public byte WarpFrom_ID { get { return warpFrom_ID; } set { warpFrom_ID = value; } }

        private byte warpTo_LevelID;
        [CustomSortedCategory("Connect Warps", 1, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("To Level")]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public byte WarpTo_LevelID { get { return warpTo_LevelID; } set { warpTo_LevelID = value; } }

        private byte warpTo_AreaID;
        [CustomSortedCategory("Connect Warps", 1, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("To Area")]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public byte WarpTo_AreaID { get { return warpTo_AreaID; } set { warpTo_AreaID = value; } }

        private byte warpTo_WarpID;
        [CustomSortedCategory("Connect Warps", 1, NUM_OF_CATERGORIES)]
        [Browsable(true)]
        [DisplayName("To ID")]
        [TypeConverter(typeof(HexNumberTypeConverter))]
        public byte WarpTo_WarpID { get { return warpTo_WarpID; } set { warpTo_WarpID = value; } }

		[CustomSortedCategory("Info", 2, NUM_OF_CATERGORIES)]
		[Browsable(true)]
		[Description("Location inside the ROM file")]
		[DisplayName("Address")]
		[ReadOnly(true)]
		public string Address => this.GetAddressString();
        
        public void MakeReadOnly()
        {
            TypeDescriptor.AddAttributes(this, new Attribute[] { new ReadOnlyAttribute(true) });
        }

        public int getROMAddress()
        {
            return memory.ByteOffset-Level.rom.Bytes.ByteOffset;
        }

        public uint getROMUnsignedAddress()
        {
			return unchecked((uint)getROMAddress());
        }

        public void updateROMData()
        {
            //if (Address.Equals("N/A")) return;
            //ROM rom = ROM.Instance;
            //uint romAddr = getROMUnsignedAddress();
			var rom = ROM;
			bool m = false;
            rom.writeByte(ref m, memory,2, WarpFrom_ID);
            rom.writeByte(ref m, memory,3, WarpTo_LevelID);
            rom.writeByte(ref m, memory,4, WarpTo_AreaID);
            rom.writeByte(ref m, memory,5, WarpTo_WarpID);
			rom.wrote(m, memory);
        }

        private string getLevelName()
        {
			//ROM rom = ROM.Instance;
			if(-1 != ROM.getLevelEntry(id:warpTo_LevelID, entry:out LevelEntry entry))
				return string.Concat(entry.Title," (",warpTo_AreaID.ToString(),")");
			else
				return string.Concat("Unknown (", warpTo_AreaID.ToString(), ")");
        }

        private string getWarpName()
        {
            if (isPaintingWarp)
            {
                return string.Concat(" [to ", getLevelName(), "]");
            }
            else
            {
				string prefix;
                switch (WarpFrom_ID)
                {
                    case 0xF0:
						prefix = " (Success) [to ";break;
                    case 0xF1:
						prefix = " (Failure) [to ";break;
                    case 0xF2:
                    case 0xF3:
						prefix = " (Special) [to ";break;
                    default:
						prefix = " [to ";break;
                }
				return string.Concat(prefix, getLevelName(), "]");
            }
        }

        public override string ToString()
        {
            //isPaintingWarp
            string warpName = "Warp 0x";

            if (isPaintingWarp)
                warpName = "Painting 0x";
			return string.Concat(warpName,WarpFrom_ID.ToString("X2"), getWarpName());
        }
		const uint dat_size=sizeof(byte) * 4 + sizeof(uint);
		public static Export.ReferenceRegister<Warp> ExportRegister;
		Export.TypeReference Export.Reference.API() { return ExportRegister.Singleton; }
		unsafe void Export.Reference<Warp>.API(Export.Exporter ex)
		{
			byte* dat = stackalloc byte[(int)dat_size];
			byte* uchr = dat;

			*uchr ++ = this.warpFrom_ID;
			*uchr ++ = this.warpTo_AreaID;
			*uchr ++ = this.warpTo_LevelID;
			*uchr ++ = this.warpTo_WarpID;
			*((uint*)uchr)=this.getROMUnsignedAddress();
			ex.Value(dat, dat_size);
		}
	}
}
