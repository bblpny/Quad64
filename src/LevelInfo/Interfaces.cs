using System;
using BubblePony.Alloc;

namespace Quad64
{
	public interface IROMProperty
	{
		ROM ROM { get; }
	}

	public interface ILevelProperty
	{
		Level Level { get; }
	}

	public interface IModel3DProperty
	{
		Model3D Model3D { get; }
	}

	public interface IMemoryProperty
	{
		ByteSegment Memory { get; }

		void Address(
			out ByteSegment segment,
			out ROM_Address address, 
			out string address_string);
	}
	public static class IMemoryPropertyUtility
	{
		public static string GetAddressString(this IMemoryProperty prop) {
			string v;
			if (null == (object)prop)
				v = ROM_Address.NullString;
			else {
				prop.Address(out ByteSegment junk1, out ROM_Address junk2, out v);
				v = null == (object)v || 0 == v.Length ? "0x0y0z0" : v;
			}
			return v;
		}
		public static void Address(
			ByteSegment memory,
			ref string address_cache,
			out ByteSegment segment,
			out ROM_Address address,
			out string address_string)
		{

			if (null == (object)address_cache)
			{
				var address_for_string = memory.ROM_Address();
				address = address_for_string;
				System.Threading.Interlocked.CompareExchange(ref address_cache, address.ToString(), null);
#if DEBUG
				//why not make sure the parser works..
				if (address != ROM_Address.Parse(address_cache))
					throw new InvalidProgramException("critical failure");
#endif
			}
			else
			{
				address = memory.ROM_Address();
			}
			segment = memory;
			address_string = address_cache;
		}
	}
	
}
