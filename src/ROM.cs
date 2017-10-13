using LIBMIO0;
using Quad64.src;
using Quad64.src.JSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using BubblePony.Alloc;
using BubblePony.Integers;

namespace Quad64
{

    public enum ROM_Region : sbyte
    {
        JAPAN,
        JAPAN_SHINDOU,
        NORTH_AMERICA,
        EUROPE
    };

    public enum ROM_Type : sbyte
    {
        VANILLA, // 8MB Compressed ROM
        EXTENDED // Uncompressed ROM
    };

    public enum ROM_Endian : sbyte
    {
        BIG, // .z64
        LITTLE, // .n64
        MIXED // .v64
    };
	[StructLayout(LayoutKind.Explicit,Size =8)]
	public struct ROM_Segment : IComparable<ROM_Segment>, IEquatable<ROM_Segment>
	{
		[FieldOffset(0)]
		public uint Offset;

		[FieldOffset(4)]
		public uint Length;

		[FieldOffset(0)]
		public ulong Value;

		public bool Equals(ROM_Segment other)
		{
			return Value == other.Value;
		}
		public int CompareTo(ROM_Segment other)
		{
			return Offset == other.Offset ? Length.CompareTo(other.Length) : Offset.CompareTo(other.Offset);
		}
		public override bool Equals(object obj)
		{
			return obj is ROM_Segment && Value == ((ROM_Segment)obj).Value;
		}
		public override int GetHashCode()
		{
			unchecked {
				return (int)(Offset ^ (Length * 5u));
			};
		}
		public override string ToString()
		{
			return string.Format("0x{0:X8}+{1}", Offset, Length);
		}
		public static ROM_Segment operator &(ROM_Segment L, ROM_Segment R)
		{
			return L.Length == 0 ? new ROM_Segment { Length = 0, Offset = R.Offset, } : R;
		}
		public static ROM_Segment operator |(ROM_Segment L, ROM_Segment R)
		{
			return L.Length == 0 ? R : L;
		}
		public static ROM_Segment operator ^(ROM_Segment L, ROM_Segment R)
		{
			return R.Length == 0 ? L : 
				L.Length == 0 ? R :
				default(ROM_Segment);
		}
		public static bool operator true(ROM_Segment criteria) { return criteria.Length != 0; }
		public static bool operator false(ROM_Segment criteria) { return criteria.Length == 0; }
		public static bool operator ==(ROM_Segment L, ROM_Segment R) { return L.Value == R.Value; }
		public static bool operator !=(ROM_Segment L, ROM_Segment R) { return L.Value != R.Value; }
		public static bool operator <(ROM_Segment L, ROM_Segment R) { return L.Offset < R.Offset || (L.Offset == R.Offset && L.Length < R.Length); }
		public static bool operator >(ROM_Segment L, ROM_Segment R) { return L.Offset > R.Offset || (L.Offset == R.Offset && L.Length > R.Length); }
		public static bool operator <=(ROM_Segment L, ROM_Segment R) { return L.Offset < R.Offset || (L.Offset == R.Offset && L.Length <= R.Length); }
		public static bool operator >=(ROM_Segment L, ROM_Segment R) { return L.Offset > R.Offset || (L.Offset == R.Offset && L.Length >= R.Length); }
	}
	[StructLayout(LayoutKind.Explicit,Size =16)]
	public struct ROM_Address : IEquatable<ROM_Address>
	{
		/// <summary>
		/// the segment to the compressed or uncompressed data in rom.
		/// when compressed, the input must cover the entire compressed MIO0 area in rom.
		/// when uncompressed the length must be the entire size of the ROM.
		/// </summary>
		[FieldOffset(0)]
		public ROM_Segment Input;

		/// <summary>
		/// the offset to the address from input.
		/// if input represents a compressed MIO0 segment within the rom you must signify this by specifying the decompressed size in .Length of Data.
		/// otherwise .Length of data must always be zero.
		/// </summary>
		[FieldOffset(8)]
		public ROM_Segment Data;

		public static ROM_Address Null => default(ROM_Address);

		public unsafe static ROM_Address Parse(byte* str, int length)
		{
			ROM_Address o = default(ROM_Address);
			byte pos = 4;
			byte ch;sbyte sh;
			while (length-- > 0)
			{
				sh = 8;
				if (*str >= 'w')
				{
					if (*str <= 'z')
					{
						pos = (byte)((*str) - 'w');
					}
					str++;
					continue;
				}
				else if(*str >= 'W' && *str <= 'Z')
				{
					pos = (byte)((*str++) - 'W');
					continue;
				}
				else if(*str >= '0' && *str <= '9')
				{
					ch = (byte)(((*str++) - '0')<<4);
				}
				else if (*str >= 'A' && *str <= 'F')
				{
					ch = (byte)(((*str++) - ('A' - 10)) << 4);
				}
				else if (*str >= 'a' && *str <= 'f')
				{
					ch = (byte)(((*str++) - ('a' - 10)) << 4);
				}
				else
				{
					str++;
					continue;
				}
				if (pos == 4) continue;
				if (length-- == 0)
					ch >>= (sh = 4);
				else if (*str >= '0' && *str <= '9')
				{
					ch |= (byte)(((*str++) - '0'));
				}
				else if (*str >= 'A' && *str <= 'F')
				{
					ch |= (byte)(((*str++) - ('A' - 10)));
				}
				else if (*str >= 'a' && *str <= 'f')
				{
					ch |= (byte)(((*str++) - ('a' - 10)));
				}
				else
				{
					length++;
					ch >>= (sh = 4);
				}

				if (0 == (pos & 2))
					if (0 == (pos & 1))//w.
						o.Data.Length = (o.Data.Length << sh) | ch;
					else//x.
						o.Data.Offset = (o.Data.Offset << sh) | ch;
				else if (0 == (pos & 1))//y.
					o.Input.Offset = (o.Input.Offset << sh) | ch;
				else//z.
					o.Input.Length = (o.Input.Length << sh)|ch;
			}
			return o;

		}
		public unsafe static ROM_Address Parse(string value, int offset, int length)
		{
			byte* bufst = stackalloc byte[64];
			byte* buf = bufst;
			while (length-- != 0)
				*buf++ = (byte)value[offset++];
			return Parse(bufst, (int)(buf - bufst));
		}
		public unsafe static ROM_Address Parse(string value) { return Parse(value, 0, value.Length); }

		/// <summary>
		/// is input compressed?
		/// </summary>
		public bool IsCompressed => Data.Length != 0;

		public bool Equals(ROM_Address other)
		{
			return Data == other.Data && Input == other.Input;
		}
		public override bool Equals(object obj)
		{
			return obj is ROM_Address && Equals((ROM_Address)obj);
		}
		public override int GetHashCode()
		{
			return Data.Length == 0 ? Data.Offset.GetHashCode() : (Input.GetHashCode() ^ Data.GetHashCode());
		}

		static unsafe void WriteHex8(byte value, ref byte* buf)
		{
			buf[0] = (byte)(value >> 4);
			buf[1] = (byte)(value & 15);
			if (buf[0] < 10) buf[0] += (byte)'0';
			else buf[0] += (byte)('A' - 10);
			if (buf[1] < 10) buf[1] += (byte)'0';
			else buf[1] += (byte)('A' - 10);
			buf += 2;
		}
		static unsafe void WriteHex(uint value, ref byte* buf, char zero='0')
		{
			if (zero != '\0')
			{
				buf[0] = (byte)zero;
				buf[1] = (byte)'x';
				buf += 2;
			}
			if (0 != (value >> 24))
			{
				WriteHex8((byte)((value >> 24) & 255), ref buf);
				WriteHex8((byte)((value >> 16) & 255), ref buf);
				WriteHex8((byte)((value >> 8) & 255), ref buf);
				WriteHex8((byte)(value & 255), ref buf);
			}
			else if (0 != (value >> 16))
			{
				WriteHex8((byte)((value >> 16) & 255), ref buf);
				WriteHex8((byte)((value >> 8) & 255), ref buf);
				WriteHex8((byte)(value & 255), ref buf);
			}
			else if (0 != (value >> 8))
			{
				WriteHex8((byte)((value >> 8) & 255), ref buf);
				WriteHex8((byte)(value & 255), ref buf);
			}
			else if (0 != (value))
			{
				WriteHex8((byte)(value & 255), ref buf);
			}
			else
			{
				*buf++ = (byte)'#';
			}
		}
		public const string NullString = "#";
		/// <summary>
		/// buf must point to some memory that is recommended to be 64 bytes long (which is likely overkill).
		/// returns a pointer past last written char.
		/// </summary>
		public unsafe byte* EncodeString(byte* buf)
		{
			if (Input.Value == 0 && Data.Value == 0)
			{
				for (int i = 0, n = NullString.Length; i < n; i++)
					*buf++ = (byte)NullString[i];
			}
			else
			{
				if (Data.Offset != 0)
					WriteHex(Data.Offset, ref buf);

				if (Data.Length != 0)
				{
					*buf++ = (byte)'w';
					WriteHex(Data.Length, ref buf, '\0');
				}
				if (Input.Value != 0)
				{
					if (0 != Input.Offset)
					{
						*buf++ = (byte)'y';
						WriteHex(Input.Offset, ref buf, '\0');
					}
					if (0 != Input.Length)
					{
						*buf++ = (byte)'z';
						WriteHex(Input.Length, ref buf, '\0');
					}
				}
			}
			return buf;
		}
		public unsafe override string ToString()
		{
			if (Input.Value == 0 && Data.Value == 0)
				return NullString;
			else
			{
				byte* buf = stackalloc byte[64];
				return new string((sbyte*)buf, 0, (int)(EncodeString(buf) - buf));
			}
		}
		public static bool operator ==(ROM_Address L, ROM_Address R)
		{
			return L.Input.Value == R.Input.Value && L.Data.Value == R.Data.Value;
		}
		public static bool operator !=(ROM_Address L, ROM_Address R)
		{
			return L.Input.Value != R.Input.Value || L.Data.Value != R.Data.Value;
		}
		public static bool operator <(ROM_Address L, ROM_Address R)
		{
			return L.Input < R.Input || (L.Input == R.Input && L.Data < R.Data);
		}
		public static bool operator >(ROM_Address L, ROM_Address R)
		{
			return L.Input > R.Input || (L.Input == R.Input && L.Data > R.Data);
		}
		public static bool operator <=(ROM_Address L, ROM_Address R)
		{
			return L.Input < R.Input || (L.Input == R.Input && L.Data >= R.Data);
		}
		public static bool operator >=(ROM_Address L, ROM_Address R)
		{
			return L.Input > R.Input || (L.Input == R.Input && L.Data >= R.Data);
		}
		public int CompareTo(ROM_Address other)
		{
			return Input == other.Input ? Data.CompareTo(other.Data) : Input.CompareTo(other.Input);
		}
	}

	public enum ROM_AllocationUser : byte {
		Null,
		ROM,
		MIO0,
	};
	
	public sealed class ROM
	{
		public const byte NUM_SEGMENT = 0x20;//32
		public string filepath;
		private SegmentContainer nonMIO0ContainerComplete, nonMIO0ContainerEmpty;
		private readonly SegmentContainer[] segments = new SegmentContainer[NUM_SEGMENT];
		private readonly CacheDictionary<ulong> MIO0Cache = new CacheDictionary<ulong>();
		private readonly CacheDictionary<uint> colorTextures = new CacheDictionary<uint>();
		private ByteSegment bytes;
		private uint explicitSegDataIsMIO0;
		private ROM_Region region = ROM_Region.NORTH_AMERICA;
		private ROM_Endian endian = ROM_Endian.BIG;
		private ROM_Type type = ROM_Type.VANILLA;
		private delegate Value Loader<Owner, Key, Value>(Owner rom, Key key) where Owner : class where Key : struct, IEquatable<Key>, IComparable<Key> where Value : class;

		private sealed class SegmentContainer
		{
			public readonly ROM ROM;
			public readonly ByteSegment Input;
			public readonly ByteSegment Data;
			public readonly CacheDictionary<ulong> Textures;
			// if this is a non MIO0 container the value will be 2.
			// when this is a MIO0 container value will be 1 if it had been flagged with modifications, otherwise 0.
			private int Pinned;

			public bool IsUnpinned => 0 == Pinned;
			internal void Clean() { Textures.Clean(); }

			public uint DecodeInputOffset(uint offset)
			{
				return (uint)(Input.Offset - ROM.bytes.Offset) + offset;
			}
			public uint DecodeDataOffset(uint offset)
			{
				if (Input.Allocation != Data.Allocation)
					throw new System.InvalidOperationException("Cannot decode offset because the segment is MID0 compressed");
				return (uint)(Data.Offset - ROM.bytes.Offset) + offset;
			}
			public SegmentContainer(ROM ROM, ByteSegment Input, ByteSegment Output)
			{
				if (null == (object)ROM) throw new System.ArgumentNullException("ROM");

				this.ROM = ROM;
				this.Input = Input;
				this.Data = Output;
				this.Textures = new CacheDictionary<ulong>();
				this.Data.Allocation.Tag = this;
			}

			public SegmentContainer(ROM ROM) : this(ROM, ROM.bytes, ROM.bytes) {
				this.Pinned = 2;
			}

			public SegmentContainer SubContainer(ByteSegment Output)
			{
				return new SegmentContainer(this, Output);
			}
			private SegmentContainer(SegmentContainer Source, ByteSegment Output)
			{
				this.ROM = Source.ROM;
				this.Input = Source.Input;
				this.Data = Output;
				this.Textures = Source.Textures;
				this.Pinned = 1;
			}
			public bool Pin()
			{
				return (0 == Pinned && 0 == System.Threading.Interlocked.CompareExchange(ref Pinned, 1, 0));
			}
			public bool Unpin()
			{
				return (1 == Pinned && 1 == System.Threading.Interlocked.CompareExchange(ref Pinned, 0, 1));
			}
		}

		public static ROM GetUtilized(Allocation alloc)
		{
			SegmentContainer container;
			return null != (object)alloc ?
				null == (object)(container = (alloc.Tag as SegmentContainer)) ?
				alloc.Tag as ROM :
				container.ROM : null;
		}

		public static ROM_AllocationUser GetUser(Allocation alloc, out ROM rom)
		{
			SegmentContainer container;
			ROM_AllocationUser user;
			if (null == (object)alloc)
			{
				rom = null;
				user = ROM_AllocationUser.Null;
			}
			else if (null == (object)(container = alloc.Tag as SegmentContainer))
			{
				if (null == (object)(rom = alloc.Tag as ROM))
					user = ROM_AllocationUser.Null;
				else
					user = ROM_AllocationUser.ROM;
			}
			else
			{
				rom = container.ROM;
				user =
					((object)container.Textures == (object)container.ROM.nonMIO0ContainerComplete.Textures) ?
					ROM_AllocationUser.ROM :
					ROM_AllocationUser.MIO0;
			}
			return user;
		}
		private sealed class CacheDictionary<TKey> : Dictionary<TKey, WeakReference> 
			where TKey : struct, IEquatable<TKey>, IComparable<TKey>
		{
			public IntPtr Version;

			private Enumerator CleanEnumerator;
			private int CleanRemove;

			public CacheDictionary() : base() { }
			private bool Next()
			{
				CleanRemove++;
				if (CleanEnumerator.MoveNext())
				{
					KeyValuePair<TKey, WeakReference> pair;
					do
					{
						if (!(pair = CleanEnumerator.Current).Value.IsAlive)
							if (Next())
							{
								return true;
							}
							else
							{
								Remove(pair.Key);
								return false;
							}
					} while (CleanEnumerator.MoveNext());
				}
				CleanEnumerator.Dispose();
				Version = Version + 1;
				return CleanRemove == Count;
			}
			/// <summary>
			/// removes all dead weak references, increments version when there were any.
			/// </summary>
			public void Clean()
			{
				lock (this)
				{
					try
					{
						CleanEnumerator = GetEnumerator();
						if (CleanEnumerator.MoveNext())
						{
							KeyValuePair<TKey, WeakReference> Pair;
							do
							{
								if ((Pair = CleanEnumerator.Current).Value.IsAlive)
									continue;
								else if (Next())
									Clear();
								else
									Remove(Pair.Key);
								break;
							} while (CleanEnumerator.MoveNext());
						}
					}
					finally
					{
						CleanRemove = 0;
						CleanEnumerator = default(Enumerator); 
					}
				}
			}
		}
		private static class Cache
		{
			private static TextureFormats.Raw TextureByIdentifier(SegmentContainer rom, ulong key)
			{
				var identifier = new TextureIdentifier { Value = key, };
				rom.Data.Segment(identifier.SegmentOffset.Offset, identifier.ByteSize, out ByteSegment segment);
				return TextureFormats.decodeTexture(
					identifier.Format,
					segment,
					identifier.Width,
					identifier.Height);
			}

			private static TextureFormats.Raw TextureByColor(ROM rom, uint key)
			{
				return TextureFormats.ColorTexture(new Color4b { Value = key, });
			}

			private static SegmentContainer DecodeMIO0(ROM rom, ulong key)
			{
				uint segmentStart = (uint)(key >> 32);
				uint segmentSize = (uint)(key & uint.MaxValue);
				rom.bytes.Segment(segmentStart, segmentSize, out ByteSegment encoded);
				var alloc = LIBMIO0.Header.InflateToNewAllocation(encoded);
				if (null == alloc) throw new InvalidProgramException("expected alloc to be non null");
				return new SegmentContainer(rom, encoded, alloc.ByteSegment);
			}

			private static readonly Loader<SegmentContainer, ulong, TextureFormats.Raw> CB_TextureByIdentifier = TextureByIdentifier;
			private static readonly Loader<ROM, uint, TextureFormats.Raw> CB_TextureByColor = TextureByColor;
			private static readonly Loader<ROM, ulong, SegmentContainer> CB_DecodeMIO0 = DecodeMIO0;
			private static void Find<Key>(
				CacheDictionary<Key> cache,
				ref object found,
				out WeakReference weak,
				out bool gotWeak,
				ref IntPtr version,
				Key key) where Key : struct, IEquatable<Key>, IComparable<Key>
			{
				lock (cache)
				{
					if ((gotWeak = (cache.TryGetValue(key, out weak))))
					{
						found = weak.Target;
						version = cache.Version;
					}
				}
			}
			private static void Bind<Key>(
				CacheDictionary<Key> cache, 
				ref WeakReference weak,
				ref object prepared,
				IntPtr version,
				Key key) where Key : struct, IEquatable<Key>, IComparable<Key>
			{
				WeakReference weak2;
				object replacement;

				lock (cache)
				{
					// we check version changes.
					// if the version changed the weak is likely untied from the dictionary.
					// as version only changes when the cache is cleaned of dead references.
					if (version != cache.Version)
						if (cache.TryGetValue(key, out weak2))
							// may or may not be the same weak reference, but we assign anyways.
							weak = weak2;
						else
							// reinstate the weak reference.
							cache[key] = weak;

					if (null == (object)(replacement = (weak.Target)))
						// still dead, so assign target of the weak.
						weak.Target = prepared;
					else
						// if the weak is not dead. (could happen with locking)
						// then we want to use that instead of what we made in preparation.
						prepared = replacement;
				}
			}

			private static void Bind<Key>(
				CacheDictionary<Key> cache,
				out WeakReference weak,
				ref object prepared,
				Key key) where Key : struct, IEquatable<Key>, IComparable<Key>
			{
				object replacement;
				lock (cache)
					if (!cache.TryGetValue(key, out weak))
						// there was still no weak reference. make it.
						cache[key] = (weak = new WeakReference(prepared));
					else if (null == (replacement = weak.Target))
						// there was a weak reference, but it had a dead target.
						// assign it to what we prepared.
						weak.Target = prepared;
					else
						// there was a weak reference, but it had a live value.
						// use that instead of what we prepared since its likely being used elsewhere by now.
						prepared = replacement;
			}
			private static Value Acquire<Owner, Key, Value>(
				Owner outer,
				CacheDictionary<Key> cache,
				Loader<Owner, Key, Value> loader, Key key)
				where Owner : class
				where Value : class
				where Key : struct, IEquatable<Key>, IComparable<Key>
			{
				object instance = null;
				IntPtr version=default(IntPtr);

				Find(cache,
					ref instance,
					out WeakReference weak,
					out bool gotWeak,
					ref version, key);

				if ( (!gotWeak || null == (object)instance) &&
					null != (object)(instance=(object)loader(outer,key)))
					if (gotWeak)
						Bind(cache, ref weak, ref instance, version, key);
					else
						Bind(cache, out weak, ref instance, key);
				return (Value)instance;
			}
			public static TextureFormats.Raw Texture(SegmentContainer outer, TextureIdentifier identifier)
			{
				// convert the identifier to real memory offset from the allocation the outer owns.
				identifier.SegmentOffset.Value =
					(uint)outer.Data.Offset + (uint)identifier.SegmentOffset.Offset;

				return Acquire(outer, outer.Textures, CB_TextureByIdentifier, identifier.Value);
			}
			public static TextureFormats.Raw Texture(ROM rom, Color4b color)
			{
				return Acquire(rom, rom.colorTextures, CB_TextureByColor, color.Value);
			}
			public static SegmentContainer MIO0(ROM rom, uint segmentStart, uint segmentEnd)
			{
				return Acquire(
					rom,
					rom.MIO0Cache, 
					CB_DecodeMIO0,
					((ulong)segmentStart << 32) | (segmentEnd - segmentStart));
			}
		}

		public void Clean()
		{
			colorTextures.Clean();
			MIO0Cache.Clean();
			nonMIO0ContainerComplete.Clean();

			for (int i = NUM_SEGMENT - 1; i >= 0; --i)
				if (null != (object)segments[i] && segments[i].IsUnpinned)
					segments[i].Clean();

			lock (pinnedSegmentModifications)
				foreach (var item in pinnedSegmentModifications)
					item.Clean();

		}
		public ROM()
		{
			filepath = string.Empty;
		}

		


        private static ROM instance; // Singleton
        public static ROM Instance
		{
			get
			{
				return instance ?? 
					System.Threading.Interlocked.CompareExchange(ref instance, new ROM(), null) ??
					instance;
			}
		}

		public string Filepath { get { return filepath; } }

        public ROM_Region Region { get { return region; } }
        public ROM_Endian Endian { get { return endian; } }
        public ROM_Type Type { get { return type; } }
        public ByteSegment Bytes { get { return bytes; } }
		
		/// <summary>
		/// collects every texture on each loaded segment.
		/// </summary>
		/// <param name="deep">if true a test will be ran to find textures in segments that were previously loaded and have not yet been GC'd</param>
		/// <param name="list">you can opt to populate your own list, note that this still creates lists internally for each call to avoid thread locking abuse (since who knows what you set IList to do). setting this to null utilizes the underlying list that is created anyways</param>
		/// <returns>list of textures</returns>
		public IList<TextureFormats.Raw> collectTextures(bool deep=false, IList<TextureFormats.Raw> list=null)
		{
			HashSet<Dictionary<ulong, WeakReference>> dictionaries = new HashSet<Dictionary<ulong, WeakReference>>();

			List<TextureFormats.Raw> tempList = new List<TextureFormats.Raw>();
			list = list ?? tempList;
			TextureFormats.Raw tex;
			SegmentContainer container;
			for (int i = NUM_SEGMENT; i >= 0; --i)
				if ((container = (i == NUM_SEGMENT ? nonMIO0ContainerComplete : segments[i])) == null ||
					!dictionaries.Add(container.Textures)) continue;
				else
				{
					lock (container.Textures)
						foreach (var value in container.Textures.Values)
							if (null != (object)(tex = (TextureFormats.Raw)value.Target))
								tempList.Add(tex);

					if (tempList != list)
					{
						foreach (var item in tempList)
							list.Add(item);
						tempList.Clear();
					}
				}
			if (!deep)
				return list;
			foreach (var weak_cache in MIO0Cache.Values)
				if (null == (container = (SegmentContainer)weak_cache.Target) ||
					!dictionaries.Add(container.Textures)) continue;
				else
				{
					lock (container.Textures)
						foreach (var value in container.Textures.Values)
							if (null != (object)(tex = (TextureFormats.Raw)value.Target))
								tempList.Add(tex);

					if (tempList != list)
					{
						foreach (var item in tempList)
							list.Add(item);
						tempList.Clear();
					}
				}
			return list;
		}
		public TextureFormats.Raw getTexture(Color4b color)
		{
			return Cache.Texture(this, color);
		}

		public TextureFormats.Raw getTexture(TextureIdentifier identifier)
		{
			return Cache.Texture(segments[identifier.SegmentOffset.Segment], identifier);
		}
		
		public TextureFormats.Raw getTexture([In]ref Scripts.Material material)
		{
			return getTexture(material.TextureIdentifier);
		}

        private void checkROM()
        {
            if (bytes[0] == 0x80 && bytes[1] == 0x37)
                endian = ROM_Endian.BIG;
            else if (bytes[0] == 0x37 && bytes[1] == 0x80)
                endian = ROM_Endian.MIXED;
            else if (bytes[0] == 0x40 && bytes[1] == 0x12)
                endian = ROM_Endian.LITTLE;

            if (endian == ROM_Endian.MIXED)
                swapMixedBig();
            else if (endian == ROM_Endian.LITTLE)
                swapLittleBig();

            if (bytes[0x3E] == 0x45)
                region = ROM_Region.NORTH_AMERICA;
            else if (bytes[0x3E] == 0x50)
                region = ROM_Region.EUROPE;
            else if (bytes[0x3E] == 0x4A)
            {
                if (bytes[0x3F] < 3)
                    region = ROM_Region.JAPAN;
                else
                    region = ROM_Region.JAPAN_SHINDOU;
            }

            // Setup segment 0x02 & segment 0x15 addresses
            if (region == ROM_Region.NORTH_AMERICA)
            {
                Globals.macro_preset_table = 0xEC7E0;
                Globals.special_preset_table = 0xED350;
               // Globals.seg02_location = new[] { (uint)0x108A40, (uint)0x114750 };
                Globals.seg15_location = new[] { (uint)0x2ABCA0, (uint)0x2AC6B0 };
            }
            else if (region == ROM_Region.EUROPE)
            {
                Globals.macro_preset_table = 0xBD590;
                Globals.special_preset_table = 0xBE100;
               // Globals.seg02_location = new[] { (uint)0xDE190, (uint)0xE49F0 };
                Globals.seg15_location = new[] { (uint)0x28CEE0, (uint)0x28D8F0 };
            }
            else if (region == ROM_Region.JAPAN)
            {
                Globals.macro_preset_table = 0xEB6D0;
                Globals.special_preset_table = 0xEC240;
               // Globals.seg02_location = new[] { (uint)0x1076D0, (uint)0x112B50 };
                Globals.seg15_location = new[] { (uint)0x2AA240, (uint)0x2AAC50 };
            }
            else if (region == ROM_Region.JAPAN_SHINDOU)
            {
                Globals.macro_preset_table = 0xC8D60;
                Globals.special_preset_table = 0xC98D0;
                //Globals.seg02_location = new[] { (uint)0xE42F0, (uint)0xEF770 };
                Globals.seg15_location = new[] { (uint)0x286AC0, (uint)0x2874D0 };
            }

            findAndSetSegment02();
            Console.WriteLine("Segment2 location: 0x" + Globals.seg02_location[0].ToString("X8") +
                " to 0x" + Globals.seg02_location[1].ToString("X8"));

            if (bytes[(int)Globals.seg15_location[0]] == 0x17)
                type = ROM_Type.EXTENDED;
            else
                type = ROM_Type.VANILLA;
            

            Console.WriteLine("ROM = " + filepath);
            Console.WriteLine("ROM Endian = " + endian);
            Console.WriteLine("ROM Region = " + region);
            Console.WriteLine("ROM Type = " + type);
            Console.WriteLine("-----------------------");
        }

        private void swapMixedBig()
        {
            for (int i = 0; i < bytes.Length; i+=2)
            {
                byte temp = bytes[i];
                bytes[i] = bytes[i + 1];
                bytes[i + 1] = temp;
            }
        }

        private void swapLittleBig()
        {
            byte[] temp = new byte[4];
            for (int i = 0; i < bytes.Length; i += 4)
            {
                temp[0] = bytes[i + 0];
                temp[1] = bytes[i + 1];
                temp[2] = bytes[i + 2];
                temp[3] = bytes[i + 3];
                bytes[i + 0] = temp[3];
                bytes[i + 1] = temp[2];
                bytes[i + 2] = temp[1];
                bytes[i + 3] = temp[0];
            }
        }
        
        public string getROMFileName()
        {
            string name = filepath.Replace("\\", "/");
            if (name.Contains("/"))
                name = name.Substring(name.LastIndexOf("/") + 1);

            return name;
        }

        public string getRegionText()
        {
            switch (Region)
            {
                case ROM_Region.NORTH_AMERICA:
                    return "North America";
                case ROM_Region.EUROPE:
                    return "Europe";
                case ROM_Region.JAPAN:
                    return "Japan";
                case ROM_Region.JAPAN_SHINDOU:
                    return "Japan (Shindou edition)";
                default:
                    return "Unknown";
            }
        }

        public string getEndianText()
        {
            switch (Endian)
            {
                case ROM_Endian.BIG:
                    return "Big Endian";
                case ROM_Endian.MIXED:
                    return "Middle Endian";
                case ROM_Endian.LITTLE:
                    return "Little Endian";
                default:
                    return "Unknown";
            }
        }

        public string getInternalName()
        {
            return System.Text.Encoding.Default.GetString(Bytes.ToArray(0x20, 20));
        }

        public void readFile(string filename)
        {
            filepath = filename;
			{
				int byteSize;
				using (var Stream = System.IO.File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					byteSize = Globals.sixtyFourMBAllocation.ByteSegment.ReadFrom(Stream);

				Globals.sixtyFourMBAllocation.Segment(0, (uint)byteSize, out bytes);
				nonMIO0ContainerComplete = new SegmentContainer(this);
				{
					bytes.Segment(0, 0, out ByteSegment blank);
					nonMIO0ContainerEmpty = nonMIO0ContainerComplete.SubContainer(blank);
				}

				// we will initialize all the segments to empty.
				for (int i = 0; i < NUM_SEGMENT; ++i)
					segments[i] = nonMIO0ContainerEmpty;

				explicitSegDataIsMIO0 = 0;
			}
            checkROM();
            Globals.pathToAutoLoadROM = filepath;
            Globals.needToSave = false;
            SettingsFile.SaveGlobalSettings("default");
        }
		private static bool unwritable(string path, out string message)
		{
			message = null;
			var fi = new FileInfo(path);
			if (fi.Exists) if (fi.IsReadOnly) message = "it said it was read only"; else return false;
			else if (!Directory.Exists(fi.DirectoryName)) message = "no directory?"; else return false;
			return true;
		}
		/// <summary>
		/// if saving fails, a message is returned otherwise null.
		/// </summary>
        public string saveFile(bool ignore_message=false)
		{
			/*
			// even if we are ignoring it, we want to run the function.
			string message = prepareSave();
			if (!ignore_message && null != (object)message) return message; else message = null;

			if (unwritable(filepath, out message)) return message ?? "error";

			if (Endian == ROM_Endian.MIXED)
            {
                swapMixedBig();
				try
				{
					using (var fs = File.Create(filepath, 2048))
						bytes.WriteTo(fs);
				}
				catch (System.Exception e)
				{
					return e.ToString();
				}
				finally
				{
					swapMixedBig();
				}
            }
            else if (Endian == ROM_Endian.LITTLE)
            {
                swapLittleBig();
				try
				{
					using (var fs = File.Create(filepath, 2048))
						bytes.WriteTo(fs);
				}
				catch (System.Exception e)
				{
					return e.ToString();
				}
				finally
				{
					swapLittleBig();
				}
            }
            else // Save as big endian by default
			{
				try { 
				using (var fs = File.Create(filepath, 2048))
					bytes.WriteTo(fs);
				}
				catch (System.Exception e)
				{
					return e.ToString();
				}
			}
            Globals.pathToAutoLoadROM = filepath;
            Globals.needToSave = false;
            SettingsFile.SaveGlobalSettings("default");
			return message;*/
			return saveFileAs(filepath, ignore_message:ignore_message);
        }

		/// <summary>
		/// if saving fails, a message is returned otherwise null.
		/// </summary>
		public string saveFileAs(string filename, ROM_Endian? saveTypeOverride=null, bool ignore_message=false)
		{
			// even if we are ignoring it, we want to run the function.
			string message = prepareSave();
			if (!ignore_message && null != (object)message) return message; else message = null;
			if (unwritable(filename, out message)) return message ?? "error";
			var saveType = saveTypeOverride ?? Endian;
			if (saveType == ROM_Endian.MIXED)
            {
                swapMixedBig();
				try
				{
					using (var fs = File.Create(filename, 2048))
						bytes.WriteTo(fs);
				}
				catch (System.Exception e)
				{
					return e.ToString();
				}
				finally
				{
					swapMixedBig();
				}
                endian = ROM_Endian.MIXED;
            }
            else if (saveType == ROM_Endian.LITTLE)
            {
                swapLittleBig();
				try
				{
					using (var fs = File.Create(filename, 2048))
						bytes.WriteTo(fs);
				}
				catch (System.Exception e)
				{
					return e.ToString();
				}
				finally
				{
					swapLittleBig();
				}
                endian = ROM_Endian.LITTLE;
            }
            else // Save as big endian by default
			{
				try
				{
					using (var fs = File.Create(filename, 2048))
						bytes.WriteTo(fs);
				}
				catch (System.Exception e)
				{
					return e.ToString();
				}
				endian = ROM_Endian.BIG;
            }
            Globals.needToSave = false;
            filepath = filename;
            Globals.pathToAutoLoadROM = filepath;
            SettingsFile.SaveGlobalSettings("default");
			return message;
        }
		public string prepareSave()
		{
			lock (pinnedSegmentModifications)
			{
				// try again to re-encode stuff that needs it.
				for (int i = 0; i < pinnedSegmentModifications.Count; i++)
				{
					var item = pinnedSegmentModifications[i];
					if (!item.IsUnpinned && LIBMIO0.Header.Update(item.Input, item.Data) && item.Unpin())
						pinnedSegmentModifications.RemoveAt(i);
				}
				// TODO, not this:
				if (0 != pinnedSegmentModifications.Count)
				{
					// if possible, re-burning the rom to fit the mio0 data would be ideal.
					return
						"Unfortunately, theres some thing(s) that cannot be saved because they no longer fit in the rom when compressed due to modification of them.";
				}
			}
			return null;
		}
		public bool hasSegment(byte index)
		{
			return index < NUM_SEGMENT && null != segments[index] && 0 != segments[index].Data.Length;
		}
		public void setSegment(uint index, uint segmentStart, uint segmentEnd, bool isMIO0)
		{
			if (
				// check valid inputs.
				segmentStart > segmentEnd || index >= NUM_SEGMENT || 
				// check if already bound.
				( (segments[index].Input.Offset-bytes.Offset) == segmentStart &&
				( (segments[index].Input.Length == (segmentEnd-segmentStart) ) ) ) )
				return;

			if (!isMIO0)
			{

				//attempt to not abuse GC, if theres an identical segment elsewhere, use that instead of newing.
				{
					SegmentContainer current = null;
					bytes.Segment(segmentStart, segmentEnd - segmentStart, out ByteSegment find);
					sbyte i;
					for (i = (sbyte)NUM_SEGMENT;
						i >= 0 &&
						(0 != (explicitSegDataIsMIO0 & ((uint)1u << i)) ||
						(current = (i == NUM_SEGMENT ? nonMIO0ContainerComplete : segments[i])).Data != find ||
						current.Input != nonMIO0ContainerComplete.Input);
						--i) ;

					segments[(int)index] = i < 0 ? nonMIO0ContainerComplete.SubContainer(find) : current;
				}
				explicitSegDataIsMIO0 &= ~(1u << (int)index);
			}
			else
			{
				segments[(int)index] = Cache.MIO0(this, segmentStart, segmentEnd);
				explicitSegDataIsMIO0 |= (1u << (int)index);
			}
		}

		public ByteSegment getSegment(byte seg)
		{
			if(seg>=NUM_SEGMENT) throw new ArgumentOutOfRangeException("seg", "points past any segment");
			return segments[seg].Data;
		}
		public ByteSegment getSegmentInput(byte seg)
		{
			if (seg >= NUM_SEGMENT) throw new ArgumentOutOfRangeException("seg", "points past any segment");
			return segments[seg].Input;
		}


		public uint decodeSegmentAddress(SegmentOffset segOffset)
		{
			if (segOffset >= ((uint)NUM_SEGMENT << 24)) throw new ArgumentOutOfRangeException("segOffset", "points past any segment");
			return segments[(int)(segOffset >> 24)].DecodeDataOffset(segOffset&UInt24.MaxValue);
		}

		public uint decodeSegmentAddress(byte segment, uint offset)
		{
			if (segment >= NUM_SEGMENT) throw new ArgumentOutOfRangeException("segment", "points past any segment");
			if (offset > UInt24.MaxValue) throw new ArgumentOutOfRangeException("offset", "exceeds size of any possible segment");
			return segments[segment].DecodeDataOffset(offset);
		}

		private string ParameterOutOfRangeName_Data(SegmentOffset segOffset, string size)
		{
			return segments[segOffset.Segment].Data.Length <= segOffset.Offset ? "segOffset" : size;
		}
		private string ParameterOutOfRangeName_Input(SegmentOffset segOffset, string size)
		{
			return segments[segOffset.Segment].Input.Length <= segOffset.Offset ? "segOffset" : size;
		}
		public ByteSegment getInputFromSegmentAddress(SegmentOffset segOffset, uint size)
		{
			if (segOffset.Segment >= NUM_SEGMENT) throw new ArgumentOutOfRangeException("segOffset", "points past any segment");
			if (segments[segOffset.Segment].Input.Length < segOffset.Offset + size)
				throw new System.ArgumentOutOfRangeException(ParameterOutOfRangeName_Input(segOffset, "size"), "not less than the segment's length");

			return getSubArray(segments[segOffset.Segment].Input, segOffset.Offset, size);
		}
		public ByteSegment getDataFromSegmentAddress(SegmentOffset segOffset, uint size)
		{
			if (segOffset.Segment >= NUM_SEGMENT) throw new ArgumentOutOfRangeException("segOffset", "points past any segment");
			if (segments[segOffset.Segment].Data.Length <= segOffset.Offset + size)
				throw new System.ArgumentOutOfRangeException(ParameterOutOfRangeName_Data(segOffset,"size"),"not less than the segment's length");
            return getSubArray(segments[segOffset.Segment].Data, segOffset.Offset, size);
		}
		/*
		public unsafe byte* getDataFromSegmentAddress(SegmentOffset segOffset, uint size, byte* put)
		{

			if (segOffset.Segment >= NUM_SEGMENT) throw new ArgumentOutOfRangeException("segOffset", "points past any segment");
			byte seg = (byte)(segOffset >> 24);
			uint off = segOffset & 0x00FFFFFF;

			if (segments[seg].Data.Length < off + size)
				return null;

			return getSubArray(segments[seg].Data, off, size, put);
		}
		public unsafe byte* getInputFromSegmentAddress(SegmentOffset segOffset, uint size, byte* put)
		{
			if (segOffset.Segment >= NUM_SEGMENT) throw new ArgumentOutOfRangeException("segOffset", "points past any segment");


			if (segments[seg].Input.Length < off + size)
				return null;

			return getSubArray(segments[seg].Input, off, size, put);
		}
		*/
		public ByteSegment getDataFromSegmentAddress_safe(SegmentOffset segOffset, uint size)
		{
			if (segOffset.Segment >= NUM_SEGMENT) throw new ArgumentOutOfRangeException("segOffset", "points past any segment");
			if (segments[segOffset.Segment].Data.Length < segOffset.Offset)
				throw new System.ArgumentOutOfRangeException("segOffset", "starts past the length of the segment");
			return getSubArray_safe(segments[segOffset.Segment].Data, segOffset.Offset, size);
		}
		public ByteSegment getInputFromSegmentAddress_safe(SegmentOffset segOffset, uint size)
		{
			if (segOffset.Segment >= NUM_SEGMENT) throw new ArgumentOutOfRangeException("segOffset", "points past any segment");
			if (segments[segOffset.Segment].Input.Length < segOffset.Offset)
				throw new System.ArgumentOutOfRangeException("segOffset", "starts past the length of the segment");
			return getSubArray_safe(segments[segOffset.Segment].Input, segOffset.Offset, size);
		}
#if ANY_ARRAY_FUNCTIONS
		public byte[] getSubArray_safe(byte[] arr, uint offset, uint size)
		{
			if (arr == null)
				return new byte[size];

			byte[] newArr = new byte[size];
			for (uint i = 0; i < size; i++)
			{
				if (offset + i < arr.Length)
					newArr[i] = arr[offset + i];
				else
					newArr[i] = 0x00;
			}
			return newArr;
		}
#endif
		public static ByteSegment getSubArray_safe(ByteSegment arr, uint offset, uint size)
		{
			ByteSegment o;
			if (arr.Length < offset + size)
				arr.Segment(offset, (uint)arr.Length - offset, out o);
			else
				arr.Segment(offset,size, out o);
			return o;
		}
#if ANY_ARRAY_FUNCTIONS
		public unsafe byte* getSubArray_safe(byte[] arr, uint offset, uint size, byte*put)
		{
			if (arr == null)
				return zero_invalid(size,put);

			for (byte* newArr = put; size != 0 && offset < arr.Length; ++newArr, --size)
				*newArr = arr[offset ++];
			if (size != 0) return zero_invalid(size, put); else return put;
		}
		public byte[] getSubArray(byte[] arr, uint offset, uint size)
		{
			byte[] newArr = new byte[size];
			for (uint i = 0; i < size; i++)
				newArr[i] = arr[offset + i];
			return newArr;
		}
#endif
		public static ByteSegment getSubArray(ByteSegment arr, uint offset, uint size)
		{
			arr.Segment(offset, size, out ByteSegment o);
			return o;
		}
#if ANY_ARRAY_FUNCTIONS
		public unsafe byte* getSubArray(byte[] arr, uint offset, uint size, byte*put)
		{
			for(byte*loc = put;0!=size;loc++,offset++,--size)
				*loc = arr[offset];
			return put;
		}
#endif

#if ANY_ARRAY_FUNCTIONS
		public void printArray(byte[] arr, int size)
        {
            Console.WriteLine(BitConverter.ToString(arr.Take(size).ToArray()).Replace("-", " "));
        }

        public void printArraySection(byte[] arr, int offset, int size)
        {
            Console.WriteLine(BitConverter.ToString(arr.Skip(offset).Take(size).ToArray()).Replace("-", " "));
        }
#endif
        public void printROMSection(int start, int end)
        {
            Console.WriteLine(BitConverter.ToString(bytes.Skip(start).Take(end - start).ToArray()).Replace("-", " "));
        }
		

#if ANY_ARRAY_FUNCTIONS
        // From: https://stackoverflow.com/a/26880541
        private int SearchBytes(byte[] haystack, byte[] needle)
        {
            var len = needle.Length;
            var limit = haystack.Length - len;
            for (var i = 0; i <= limit; i++)
            {
                var k = 0;
                for (; k < len; k++)
                {
                    if (needle[k] != haystack[i + k]) break;
                }
                if (k == len) return i;
            }
            return -1;
        }

#endif
		private void setByteWrite(ref bool mod, ByteSegment segment, int offset, byte value)
		{
			if (mod)
				segment[offset] = value;
			else if (segment[offset] != value) {
				mod = true;
				segment[offset] = value;
			}
		}
        public void writeWord(ref bool mod, ByteSegment segment, uint offset, int word)
        {
            setByteWrite(ref mod, segment, (int)(offset + 0),(byte)((word >> 24)&255));
            setByteWrite(ref mod, segment, (int)(offset + 1),(byte)((word >> 16) & 255));
            setByteWrite(ref mod, segment, (int)(offset + 2),(byte)((word >> 8) & 255));
			setByteWrite(ref mod, segment, (int)(offset + 3),(byte)((word & 255)));
        }

        public void writeWord(ref bool mod, ByteSegment segment, uint offset, uint word)
        {
            writeWord(ref mod, segment, offset, unchecked((int)word));
        }

		public void writeHalfword(ref bool mod, ByteSegment segment, uint offset, short half)
		{
			setByteWrite(ref mod, segment, (int)offset + 0, (byte)(255&(half >> 8)));
			setByteWrite(ref mod, segment, (int)offset + 1, (byte)(255&(half)));
		}

        public void writeHalfword(ref bool mod, ByteSegment segment, uint offset, ushort word)
        {
            writeHalfword(ref mod, segment, offset, unchecked((short)word));
        }

        public void writeByte(ref bool mod, ByteSegment segment, uint offset, byte b)
        {
			setByteWrite(ref mod, segment, (int)offset, b);
        }

		private readonly List<SegmentContainer> pinnedSegmentModifications = new List<SegmentContainer>();

		public void wrote(bool mod,ByteSegment memory)
		{
			if (!mod) return;

			var user = GetUser(memory.Allocation, out ROM match);
			if (this != match) throw new ArgumentException("the segment of memory does not belong to this rom instance!","memory");
			if (user == ROM_AllocationUser.MIO0)
			{
				// need to encode the MIO0.
				SegmentContainer container = (SegmentContainer)memory.Allocation.Tag;

				// attempt to compress Data back into the Input
				lock (pinnedSegmentModifications)
				{
					if (!Globals.delayMID0Compression && Header.Update(container.Input, container.Data))
					{
						//when we fit the data back into the input we no longer need to pin the segment since it will be re-read when the level changes or whatever.
						if (container.Unpin())// <- had been added to the list previously.
							pinnedSegmentModifications.Remove(container);
					}
					else
					{
						// data did not fit, so this is a BIG TODO...
						//for now, we pin the segment to this object, making it not GC (so long as this ROM instance is alive).
						// the idea is it can be kept there until we need to re-write/burn the rom to disk, but none of that happens yet.
						if (container.Pin())//<- had not been added to the list previously.
							pinnedSegmentModifications.Add(container);
					}
				}
			}
		}

        public byte readByte(uint offset)
        {
            return bytes[(int)offset];
        }
        public short readHalfword(uint offset)
        {
            return (short)(bytes[(int)offset] << 8 | bytes[(int)offset + 1]);
        }
        public ushort readHalfwordUnsigned(uint offset)
        {
            return (ushort)readHalfword(offset);
        }
        public int readWord(uint offset)
        {
            return bytes[0 + (int)offset] << 24 | bytes[1 + (int)offset] << 16
                | bytes[2 + (int)offset] << 8 | bytes[3 + (int)offset];
        }
        public uint readWordUnsigned(uint offset)
        {
            return (uint)(bytes[0 + (int)offset] << 24 | bytes[1 + (int)offset] << 16
                | bytes[2 + (int)offset] << 8 | bytes[3 + (int)offset]);
        }


        public bool isSegmentMIO0(byte seg)
		{
			return 0!=(explicitSegDataIsMIO0 & (1u<<(int)seg));
		}

        public void findAndSetSegment02()
        {
            AssemblyReader ar = new AssemblyReader();
			var func_calls = new StructList<AssemblyReader.JAL_CALL>();
			AssemblyReader.JAL_CALL jal=default(AssemblyReader.JAL_CALL);

			switch (region)
            {
                default:
                case ROM_Region.NORTH_AMERICA:
					ar.findJALsInFunction(
						Globals.seg02_init_NA,
						Globals.RAMtoROM_NA,
						ref func_calls);
					while(func_calls.Dequeue(ref jal))
                        if(jal.JAL_ADDRESS == Globals.seg02_alloc_NA && jal.a0 == 0x2)
                        {
                            Globals.seg02_location = new[] { jal.a1, jal.a2 };
							if (readWordUnsigned(jal.a1) == 0x4D494F30)
								explicitSegDataIsMIO0 |= 1u << 0x02;
                        }
                    break;
                case ROM_Region.EUROPE:
                    ar.findJALsInFunction(
						Globals.seg02_init_EU,
						Globals.RAMtoROM_EU,
						ref func_calls);
					while (func_calls.Dequeue(ref jal))
						if (jal.JAL_ADDRESS == Globals.seg02_alloc_EU && jal.a0 == 0x2)
                        {
                            Globals.seg02_location = new[] { jal.a1, jal.a2 };
                            if (readWordUnsigned(jal.a1) == 0x4D494F30)
								explicitSegDataIsMIO0 |= 1u << 0x02;
						}
                    break;
                case ROM_Region.JAPAN:
                    ar.findJALsInFunction(
						Globals.seg02_init_JP,
						Globals.RAMtoROM_JP,
						ref func_calls);
					while (func_calls.Dequeue(ref jal))
						if (jal.JAL_ADDRESS == Globals.seg02_alloc_JP && jal.a0 == 0x2)
                        {
                            Globals.seg02_location = new[] { jal.a1, jal.a2 };
                            if (readWordUnsigned(jal.a1) == 0x4D494F30)
								explicitSegDataIsMIO0 |= 1u << 0x02;
						}
                    break;
                case ROM_Region.JAPAN_SHINDOU:
                    ar.findJALsInFunction(
						Globals.seg02_init_JS,
						Globals.RAMtoROM_JS,
						ref func_calls);
					while (func_calls.Dequeue(ref jal))
						if (jal.JAL_ADDRESS == Globals.seg02_alloc_JS && jal.a0 == 0x2)
                        {
                            Globals.seg02_location = new[] { jal.a1, jal.a2 };
                            if (readWordUnsigned(jal.a1) == 0x4D494F30)
								explicitSegDataIsMIO0 |= 1u << 0x02;
						}
                    break;
            }
		}
		private static class Defaults
		{
			public static readonly LevelEntry[] entries = 
			{
				new LevelEntry( 0x04, "Big Boo's Haunt"            ),
				new LevelEntry( 0x05, "Cool Cool Mountain"         ),
				new LevelEntry( 0x06, "Inside Castle"              ),
				new LevelEntry( 0x07, "Hazy Maze Cave"             ),
				new LevelEntry( 0x08, "Shifting Sand Land"         ),
				new LevelEntry( 0x09, "Bob-omb Battlefield"        ),
				new LevelEntry( 0x0A, "Snowman's Land"             ),
				new LevelEntry( 0x0B, "Wet Dry World"              ),
				new LevelEntry( 0x0C, "Jolly Roger Bay"            ),
				new LevelEntry( 0x0D, "Tiny Huge Island"           ),
				new LevelEntry( 0x0E, "Tick Tock Clock"            ),
				new LevelEntry( 0x0F, "Rainbow Ride"               ),
				new LevelEntry( 0x10, "Castle Grounds"             ),
				new LevelEntry( 0x11, "Bowser Course 1"            ),
				new LevelEntry( 0x12, "Vanish Cap"                 ),
				new LevelEntry( 0x13, "Bowser Course 2"            ),
				new LevelEntry( 0x14, "Secret Aquarium"            ),
				new LevelEntry( 0x15, "Bowser Course 3"            ),
				new LevelEntry( 0x16, "Lethal Lava Land"           ),
				new LevelEntry( 0x17, "Dire Dire Docks"            ),
				new LevelEntry( 0x18, "Whomp's Fortress"           ),
				new LevelEntry( 0x19, "End Cake Picture"           ),
				new LevelEntry( 0x1A, "Castle Courtyard"           ),
				new LevelEntry( 0x1B, "Peach's Secret Slide"       ),
				new LevelEntry( 0x1C, "Metal Cap"                  ),
				new LevelEntry( 0x1D, "Wing Cap"                   ),
				new LevelEntry( 0x1E, "Bowser Battle 1"            ),
				new LevelEntry( 0x1F, "Rainbow Clouds"             ),
				new LevelEntry( 0x21, "Bowser Battle 2"            ),
				new LevelEntry( 0x22, "Bowser Battle 3"            ),
				new LevelEntry( 0x24, "Tall Tall Mountain"         )
			};
			public static readonly Dictionary<string, int> titleToIndex;
			public static readonly Dictionary<ushort, int> idToIndex;
			static Defaults()
			{
				int n = entries.Length;
				var title = new Dictionary<string, int>(n);
				var id = new Dictionary<ushort, int>(n);
				for (int i = 0; i < n; i++)
				{
					title[entries[i].Title] = i;
					id[entries[i].ID] = i;
				}
				titleToIndex = new Dictionary<string, int>(title);
				idToIndex = new Dictionary<ushort, int>(id);
			}
		}
		private readonly List<LevelEntry> levelIDs = new List<LevelEntry>(Defaults.entries);
		private readonly Dictionary<ushort, int> levelIdToIndex = new Dictionary<ushort, int>(Defaults.idToIndex);
		private readonly Dictionary<string, int> levelTitleToIndex = new Dictionary<string, int>(Defaults.titleToIndex);

		public int getLevelCount() { lock (levelIDs) return levelIDs.Count; }
		public LevelEntry[] getLevelEntriesCopy() { lock (levelIDs) return levelIDs.ToArray(); }

		/// <summary>
		/// finds a level entry by it's title, returns the index the level entry is in the internal list when found, otherwise -1.
		/// </summary>
		public int getLevelEntry(string title, out LevelEntry entry)
		{
			int index;
			bool ret;
			if (null == (object)title)
			{
				ret = false;
				index = -1;
				entry = default(LevelEntry);
			}
			else
				lock (levelIDs)
					if ((ret = (levelTitleToIndex.TryGetValue(title, out index))))
						entry = levelIDs[index];
					else
						entry = default(LevelEntry);
			return ret ? index : -1;
		}

		/// <summary>
		/// gets a level entry by it's index in the rom's internal list returns true if the index was within the size of the internal list.
		/// </summary>
		public bool getLevelEntry(int index, out LevelEntry entry)
		{
			bool ret;
			if (index < 0)
			{
				ret = false;
				entry = default(LevelEntry);
			}
			else
				lock (levelIDs)
					if ((ret = (index < levelIDs.Count)))
						entry = levelIDs[index];
					else
						entry = default(LevelEntry);
			return ret;
		}

		/// <summary>
		/// finds a level entry by its id, returns the index the level entry is in the internal list when found, otherwise -1.
		/// </summary>
		public int getLevelEntry(ushort id, out LevelEntry entry)
		{
			int index;
			bool ret;
			lock (levelIDs)
				if ((ret = (levelIdToIndex.TryGetValue(id, out index))))
					entry = levelIDs[index];
				else
					entry = default(LevelEntry);
			return ret?index:-1;
		}

		public int getLevelIndex(LevelEntry entry)
		{

			int matchIndex;
			if (entry) {
				matchIndex = getLevelEntry(id: entry.ID, entry: out LevelEntry match);
				if (match.Title != entry.Title)
					matchIndex = -1;
			} else matchIndex = -1;
			return matchIndex;
		}

		public int getLevelIndex(ushort id)
		{
			int idx;
			lock (levelIDs)
				if (!levelIdToIndex.TryGetValue(id, out idx))
					idx = -1;
			return idx;
		}
		public int getLevelIndex(string title)
		{
			int idx;
			if (null == (object)title)
				idx = -1;
			else
				lock (levelIDs)
					if (!levelTitleToIndex.TryGetValue(title, out idx))
						idx = -1;
			return idx;
		}
		/// <summary>
		/// either updates or adds a level entry so long as the title on the entry is not null.
		/// updates occur when the id or name matches one existing entry (for an id match, title is changed and vicsa versa).
		/// adds occur when there is no entry with name or id.
		/// 
		/// an exception is thrown if the id matches that of one entry and the name (title) matches that of a different entry.
		/// </summary>
		/// <returns>internal index of the entry</returns>
		public int putLevelEntry(LevelEntry entry)
		{
			int ret, ret2;
			if (entry)
			{
				lock (levelIDs)
				{
					if (levelIdToIndex.TryGetValue(entry.ID, out ret))
					{
						if (levelTitleToIndex.TryGetValue(entry.Title, out ret2))
						{
							if (ret2 != ret)
								throw new ArgumentException("the entry specifies a known title and a known id, this makes it impossible to enter it without removing two entries", "entry");
							//else.. the entry is already in.
						}
						else
						{
							//change the name..
							levelIDs[ret] = entry;
						}
					}
					else if (!levelTitleToIndex.TryGetValue(entry.Title, out ret))
					{
						// add a new entry.
						levelTitleToIndex[entry.Title] = (ret = levelIDs.Count);
						levelIdToIndex[entry.ID] = levelIDs.Count;
						levelIDs.Add(entry);
					}
					else
					{
						//change the id.
						levelIDs[ret] = entry;
					}
				}
			}
			else ret = -1;
			return ret;
		}
		public bool removeLevelEntry(int internal_index)
		{
			LevelEntry entry;
			if (internal_index >= 0)
				lock (levelIDs)
					if (levelIDs.Count <= internal_index)
						internal_index = -1;
					else
					{
						entry = levelIDs[internal_index];
						levelIDs.RemoveAt(internal_index);
						levelIdToIndex.Remove(entry.ID);
						levelTitleToIndex.Remove(entry.Title);

						for (; internal_index < levelIDs.Count; ++internal_index)
						{
							entry = levelIDs[internal_index];
							levelIdToIndex[entry.ID] = internal_index;
							levelTitleToIndex[entry.Title] = internal_index;
						}
					}
			else internal_index = -1;//<-- just in case it was some other negative value.
			return internal_index != -1;
		}
		public bool removeLevelEntry(LevelEntry entry)
		{
			return (entry ? removeLevelEntry(getLevelIndex(entry)) : false);
		}
		public void clearLevelEntries()
		{
			lock (levelIDs)
			{
				levelIDs.Clear();
				levelIdToIndex.Clear();
				levelTitleToIndex.Clear();
			}
		}
		/// <summary>
		/// resets level entries to defaults.
		/// </summary>
		public void resetLevelEntries() {
			lock (levelIDs)
			{
				levelIDs.Clear();
				levelIdToIndex.Clear();
				levelTitleToIndex.Clear();
				levelIDs.AddRange(Defaults.entries);
				levelIdToIndex.Union(Defaults.idToIndex);
				levelTitleToIndex.Union(Defaults.titleToIndex);
			}
		}
		internal static void internal_getMIO0ROM_Address(
			Allocation allocation, out ROM_Address adr, uint offset)
		{
			SegmentContainer container = (SegmentContainer)allocation.Tag;
			adr.Input.Value = 0;
			adr.Data.Value = 0;
			adr.Input.Offset = (uint)(container.Input.Offset-container.ROM.bytes.Offset);
			adr.Input.Length = (uint)container.Input.Length;
			adr.Data.Offset = offset;
			adr.Data.Length = (uint)container.Data.Length;
		}
	}

	internal static class ROMUtility
	{
		public static ROM GetROM(this Allocation alloc)
		{
			return ROM.GetUtilized(alloc);
		}
		public static ROM GetROM<T>(this T segment)
			where T : struct, IAllocationSegment
		{
			return ROM.GetUtilized(segment.uAllocation);
		}
		public static ROM GetROM(this IAllocationSegment segment)
		{
			return null == (object)segment ? null : ROM.GetUtilized(segment.uAllocation);
		}
		public static ROM_AllocationUser GetROMUser(this Allocation alloc, out ROM rom)
		{
			return ROM.GetUser(alloc, out rom);
		}
		public static ROM_AllocationUser GetROMUser<T>(this T segment, out ROM rom)
			where T : struct, IAllocationSegment
		{
			return ROM.GetUser(segment.uAllocation, out rom);
		}
		public static ROM_AllocationUser GetROMUser(this IAllocationSegment segment, out ROM rom)
		{
			return ROM.GetUser(null == (object)segment ? null : segment.uAllocation, out rom);
		}
		public static ROM_AllocationUser GetROMUser(this Allocation alloc)
		{
			return ROM.GetUser(alloc, out ROM junk);
		}
		public static ROM_AllocationUser GetROMUser<T>(this T segment)
			where T : struct, IAllocationSegment
		{
			return ROM.GetUser(segment.uAllocation, out ROM junk);
		}
		public static ROM_AllocationUser GetROMUser(this IAllocationSegment segment)
		{
			return ROM.GetUser(null == (object)segment ? null : segment.uAllocation, out ROM junk);
		}

		public static ROM_Address ROM_Address(this ByteSegment segment)
		{
			ROM_Address o;
			ByteSegment rom_segment;
			var user = GetROMUser(segment, out ROM rom);
			if (user == ROM_AllocationUser.MIO0)
				ROM.internal_getMIO0ROM_Address(segment.Allocation, out o, (uint)segment.Offset);
			else
			{
				o = default(ROM_Address);
				if (user == ROM_AllocationUser.ROM &&
					((rom_segment = rom.Bytes).Allocation == segment.Allocation))
				{
					o.Input.Length = (uint)rom_segment.Length;
					o.Data.Offset = (uint)(segment.Offset - rom_segment.Offset);
				}
			}
			return o;
		}
	}
	public struct LevelEntry : IEquatable<LevelEntry>
	{
		public readonly string Title;
		public readonly ushort ID;
		public LevelEntry(ushort ID, string Title)
		{
			if (null == (object)Title) throw new ArgumentNullException("Title");
			this.ID = ID;
			this.Title = Title;
		}
		public override bool Equals(object obj)
		{
			return obj is LevelEntry ? Equals((LevelEntry)obj) :( null== obj && null == (object)Title);
		}
		public bool Equals(LevelEntry other)
		{
			return null == (object)Title ? null == (object)other.Title : (null != (object)other.Title && ID == other.ID && Title == other.Title);
		}
		public override int GetHashCode()
		{
			return null == (object)Title ? 0 : ((ID * ((int)(ushort.MaxValue / 3))) ^ Title.GetHashCode());
		}
		public static bool operator ==(LevelEntry L, LevelEntry R) { return L.Equals(R); }
		public static bool operator !=(LevelEntry L, LevelEntry R) { return !L.Equals(R); }
		public static bool operator true(LevelEntry criteria) { return null != (object)criteria.Title; }
		public static bool operator false(LevelEntry criteria) { return null == (object)criteria.Title; }
		public override string ToString()
		{
			return null == (object)Title ? "null" : string.Concat("[\"", Title, "\",", ID+"]");
		}
	}


}
