using BubblePony.Alloc;
using BubblePony.Integers;

namespace Quad64.Scripts
{
	public abstract class Script
	{
		public abstract class Runtime : IROMProperty, System.ICloneable
		{
			public readonly Script script;
			public readonly ROM rom;
			ROM IROMProperty.ROM => rom;
			private ByteSegment data;
			protected SegmentOffset segmentOffset;
			protected bool ended;
			protected byte returnCode;

			private Runtime(Script script, ROM rom)
			{
				if (null == script) throw new System.ArgumentNullException("script");
				if (null == rom) throw new System.ArgumentNullException("rom");

				this.script = script;
				this.rom = rom;
			}

			private Runtime(Runtime Source) : this(Source.script, Source.rom)
			{
				this.ended = Source.ended;
				this.returnCode = Source.returnCode;
				this.segmentOffset = Source.segmentOffset;
				this.data = Source.data;
			}
			

			public static void GetSegmentOffset(
				Runtime rt,
				out SegmentOffset segmentOffset)
			{
				segmentOffset = rt.segmentOffset;
			}

			public static void SetSegment(
				Runtime rt, byte Segment)
			{
				rt.segmentOffset.Segment = Segment;
				if (!rt.rom.hasSegment(rt.segmentOffset.Segment))
				{
					rt.data = default(ByteSegment);
					rt.ended = true;
				}
				else
				{
					rt.data = rt.rom.getSegment(rt.segmentOffset.Segment);
				}
			}
			public static void SetSegment(Runtime rt, SegmentOffset Segment)
			{
				rt.segmentOffset.Offset = Segment.Offset;
				SetSegment(rt, Segment.Segment);
			}
			static public bool Advance(Runtime rt, uint offset)
			{
				if (offset + (uint)rt.segmentOffset.Offset >= rt.data.Length) {
					rt.segmentOffset.Offset = (UInt24)(uint)rt.data.Length;
					return false;
				}
				rt.segmentOffset.Offset += (UInt24)offset;
				return true;
			}
			static public bool GetCommand(
				Runtime rt,
				out ByteSegment cmd,
				out CommandInfo info)
			{
				byte Length;
				if (rt.ended || rt.segmentOffset.Offset >= rt.data.Length)
				{
					cmd = default(ByteSegment);
					rt.ended = true;
					info = null;
				}
				else
				{
					info = rt.script.Commands[rt.data[rt.segmentOffset.Offset]];
					if (null == info)
					{
						rt.ended = true;
						cmd = default(ByteSegment);
					}
					else
					{

						if (0 != (info.Traits & (CommandInfoTraits.HasNonZeroBB | CommandInfoTraits.SecondByteHoldsLength)) &&
							1 < (rt.data.Length - rt.segmentOffset.Offset) &&
							rt.data[rt.segmentOffset.Offset + 1] != 0 &&
							0 != (CommandInfoTraits.SecondByteHoldsLength & (info = info.Next).Traits))
							Length = rt.data[rt.segmentOffset.Offset + 1];
						else
							Length = info.Length;

						if (0== Length || (Length > (rt.data.Length - rt.segmentOffset.Offset)))
						{
							rt.ended = true;
							cmd = default(ByteSegment);
						}
						else
						{
							rt.data.Segment((uint)rt.segmentOffset.Offset, Length, out cmd);
							rt.ended = 0 == cmd.Length;
						}
					}
				}
				return !rt.ended;
			}
			public static bool Exit(Runtime rt, byte code, bool codeOnly = false)
			{
				if (!rt.ended)
				{
					rt.ended = !codeOnly;
					rt.returnCode = code;
					return true;
				}
				else
				{
					return false;
				}
			}

			object System.ICloneable.Clone() { return script.CloneRuntime(this); }

			public abstract class Impl<Implementation> : Runtime
				where Implementation : Impl<Implementation>
			{
				public delegate void Command(Implementation rt, ref ByteSegment cmd);
#if DEBUG
				private static void EnsureType(System.Type Type) { if(!Type.IsAssignableFrom(typeof(Implementation))) throw new System.InvalidOperationException("Class does illegal stuff."); }
#endif
				protected Impl(Script script, ROM rom) : base(script, rom) {
#if DEBUG
					EnsureType(GetType());
#endif
				}

				protected Impl(Implementation Source) : base(Source) {
#if DEBUG
					EnsureType(GetType());
#endif
				}

				public static byte Evaluate(Implementation rt)
				{
					CommandInfo info;
					ByteSegment cmd;

					while (!rt.ended && GetCommand(rt, out cmd, out info))
					{
						if (
							(!rt.script.PreflightCommand ||
							 rt.script.RunCommand(rt, info, ref cmd)) &&
							0 != (info.Traits & CommandInfoTraits.HasDelegate))
							((Command)info.Delegate)(rt, ref cmd);

						if (!rt.ended && 0 < cmd.Length && !Advance(rt, (uint)cmd.Length))
							rt.ended = true;
					}
					return rt.returnCode;
				}
			}
		}
		public BitList256 CommandMask;
		public readonly CommandInfo[] Commands = new CommandInfo[256];
		public readonly System.Type RuntimeType;
		public readonly System.Type DelegateType;

		/// <summary>
		/// if true then RunCommand before running the command.
		/// if RunCommand returns false then the command is not ran.
		/// 
		/// RunCommand can also alter the cmd input, which will alter advancement of reading.
		/// </summary>
		private readonly bool PreflightCommand;

		protected virtual bool RunCommand(
			Runtime rt,
			CommandInfo info,
			ref ByteSegment cmd)
		{ throw new System.InvalidOperationException("Either implement RunCommand or do not invoke base.RunCommand! Type:"+GetType().Name); }

		protected abstract CommandInfo GetCommandInfo(byte AA);
		protected abstract Runtime CloneRuntime(Runtime Source);
		protected abstract byte EvaluateRuntime(Runtime Source);

		[System.Flags]
		public enum CommandInfoTraits : byte
		{
			HasNonZeroBB = 1,
			IsNonZeroBB = 2,
			HasDelegate = 4,
			// use zero as length to append this when constructing command info.
			SecondByteHoldsLength = 8,
		}

		public sealed class CommandInfo
		{
			public readonly Script Script;
			public readonly System.Delegate Delegate;
			public readonly CommandInfo Next;
			public readonly byte Length;
			public readonly CommandInfoTraits Traits;
			private CommandInfo(
				Script Script,
				System.Delegate Delegate,
				CommandInfo Next,
				byte Length,
				byte NonZeroLength,
				CommandInfoTraits Traits)
			{
				if (null == Next)
				{
					if (0 != (Traits & CommandInfoTraits.HasNonZeroBB))
					{
						Next = new CommandInfo(
							Script,
							Delegate,
							this,
							NonZeroLength,
							Length,
							CommandInfoTraits.IsNonZeroBB |
							(Traits & ~CommandInfoTraits.HasNonZeroBB));
					}
					else
					{
						Next = this;
					}
				}
				this.Script = Script;
				this.Delegate = Delegate;
				this.Next = Next;
				this.Length = Length;
				this.Traits = Traits | 
					(null == Delegate ? 
						(CommandInfoTraits)0 : CommandInfoTraits.HasDelegate) |
					(0 == this.Length ? CommandInfoTraits.SecondByteHoldsLength : (CommandInfoTraits)0);
			}

			public CommandInfo(
				Script Script,
				System.Delegate Delegate,
				byte Length) : this(Script, Delegate, null, Length, 0,
					(null == (object)Delegate ?
					  (CommandInfoTraits)0 :
					  CommandInfoTraits.HasDelegate))
			{

			}

			public CommandInfo(
				Script Script,
				System.Delegate Delegate,
				byte BBZeroLength,
				byte BBNonZeroLength)
				: this(Script,
					  Delegate,
					  null,
					  BBZeroLength,
					  BBNonZeroLength,
					  (BBNonZeroLength != BBZeroLength ?
					  CommandInfoTraits.HasNonZeroBB :
					  (CommandInfoTraits)0) |
					  (null==(object)Delegate ? 
					  (CommandInfoTraits)0 :
					  CommandInfoTraits.HasDelegate))
			{ }
		}

		protected Script(
			System.Type RuntimeType,
			System.Type DelegateType,
			bool PreflightRunCommand = false)
		{
			this.RuntimeType = RuntimeType;
			this.DelegateType = DelegateType;
			this.PreflightCommand = PreflightRunCommand;

			for (int i = 255; i >= 0; --i) {
				if (null != (object)(this.Commands[i] = GetCommandInfo((byte)i)) &&
					0 != (this.Commands[i].Traits & CommandInfoTraits.HasDelegate))
					CommandMask.Add((byte)i);
				else if (null !=
					RuntimeType.GetMethod("CMD_" + i.ToString("X2"), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static)
					)
					throw new System.InvalidOperationException(
						string.Concat("The runtime type defines a method named CMD_",
						i.ToString("X2"),
						" but does not expose it. (",
						GetType().Name,
						")"));
			}
		}
		public static int bytesToInt32(ByteSegment b, int offset=0)
		{
			return
				((int)b[offset + 3]) |
				((int)b[offset + 2] << 8) |
				((int)b[offset + 1] << 16) | 
				((int)b[offset] << 24);
		}
		public static long bytesToInt64(ByteSegment b, int offset=0)
		{
			return
				((long)b[offset + 7]) |
				((long)b[offset] << 56) | ((long)b[offset + 1] << 48) | ((long)b[offset + 2] << 40) | ((long)b[offset + 3] << 32) |
				((long)b[offset + 4] << 24) | ((long)b[offset + 5] << 16) | ((long)b[offset + 6] << 8);
		}
		public static short bytesToInt16(ByteSegment b, int offset=0)
		{
			return (short)((((int)b[offset] << 24) | ((int)b[offset + 1]<<16))>>16);
		}
		public static sbyte bytesToSByte(ByteSegment b, int offset=0)
		{
			return unchecked((sbyte)b[offset]);
		}
		
		public static uint bytesToUInt32(ByteSegment b, int offset=0)
		{
			return
				((uint)b[offset + 3]) |
				((uint)b[offset + 2] << 8) |
				((uint)b[offset + 1] << 16) |
				((uint)b[offset] << 24);
		}
		public static ulong bytesToUInt64(ByteSegment b, int offset=0)
		{
			return
				((ulong)b[offset + 7]) |
				((ulong)b[offset] << 56) | ((ulong)b[offset + 1] << 48) | ((ulong)b[offset + 2] << 40) | ((ulong)b[offset + 3] << 32) |
				((ulong)b[offset + 4] << 24) | ((ulong)b[offset + 5] << 16) | ((ulong)b[offset + 6] << 8);
		}
		public static ushort bytesToUInt16(ByteSegment b, int offset = 0)
		{
			return (ushort)(((ushort)b[offset] << 8) | b[offset + 1]);
		}
		public static byte bytesToByte(ByteSegment b, int offset = 0)
		{
			return b[offset];
		}
		public static UInt24 bytesToUInt24(ByteSegment b, int offset = 0)
		{
			return new UInt24 { B0 = b[offset + 2], B1 = b[offset + 1], B2 = b[offset] };
		}
		public static Vector3s bytesToVector3s(ByteSegment b, int offset = 0)
		{
			unchecked
			{
				return new Vector3s
				{
					Z = bytesToInt16(b, offset + 4),
					Y = bytesToInt16(b, offset + 2),
					X = bytesToInt16(b, offset),
				};
			}
		}
		public static Vector3c bytesToVector3c(ByteSegment b, int offset = 0)
		{
			unchecked
			{
				return new Vector3c
				{
					Z = (sbyte)b[offset + 2],
					Y = (sbyte)b[offset + 1],
					X = (sbyte)b[offset],
				};
			}
		}
		public static SegmentOffset bytesToSegmentOffset(ByteSegment b, int offset = 0)
		{
			return new SegmentOffset { Value = bytesToUInt32(b, offset), };
		}
		public static short intToShort(uint value)
		{
			unchecked
			{
				return (short)(((int)value << 16) >> 16);
			}
		}
		public static short intToShort(int value)
		{
			return (short)(((int)value << 16) >> 16);
		}
		public static sbyte intToSByte(uint value)
		{
			unchecked
			{
				return (sbyte)(((int)value << 24) >> 24);
			}
		}
		public static sbyte intToSByte(int value)
		{
			return (sbyte)(((int)value << 24) >> 24);
		}
	}

}
