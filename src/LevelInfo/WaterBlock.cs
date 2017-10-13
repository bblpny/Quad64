using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BubblePony.Alloc;

namespace Quad64
{
	public sealed class WaterBlock : ILevelProperty, IROMProperty, IMemoryProperty
	{
		private readonly Level parent;
		private string mem_adr;
		private readonly ByteSegment memory;

		public Level Level => parent;
		public ROM ROM => parent.rom;
		public ByteSegment Memory => memory;

		public void Address(out ByteSegment segment, out ROM_Address address, out string address_string)
		{
			IMemoryPropertyUtility.Address(memory, ref mem_adr, out segment, out address, out address_string);
		}
		public WaterBlock(Level parent, ByteSegment memory)
		{
			if (null == (object)parent) throw new System.ArgumentNullException("parent");
			if (memory.Length != 0xC) throw new System.ArgumentException("memory length was not 12","memory");
			this.parent = parent;
			this.memory = memory;
		}
	}
}
