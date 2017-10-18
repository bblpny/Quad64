using BubblePony.Alloc;
using BubblePony.Integers;
using System.Runtime.InteropServices;
namespace Quad64.Scripts
{
	public sealed class GeoScripts : Script
    {
		private GeoScripts() : base(
			RuntimeType:typeof(Runtime), 
			DelegateType:typeof(Runtime.Command),
			PreflightRunCommand:true) { }

		private static readonly GeoScripts Instance = new GeoScripts();
		
		protected sealed override byte EvaluateRuntime(Script.Runtime Source)
		{
			return Runtime.Evaluate((Runtime)Source);
		}
		protected sealed override Script.Runtime CloneRuntime(Script.Runtime Source)
		{
			return new Runtime((Runtime)Source);
		}
		new private sealed partial class Runtime : Script.Runtime.Impl<Runtime>,
			ILevelProperty
		{
			public readonly Level lvl;
			public readonly Model3D mdl;
			public readonly GeoRoot rootNode;

			Level ILevelProperty.Level => lvl;
			public GeoParent currentParent => (GeoParent)nodeCurrent ?? rootNode;
			public GeoNode nodeCurrent;

#if DEBUG
			public readonly ushort[] counter;
#endif
			public Persistant persistant=Persistant.reset;
			public bool _zbuf=true;


			public struct Persistant
			{
				public short minDistance, maxDistance;

				public static Persistant reset => default(Persistant);

				public GeoNode Create(Runtime rt, GeoParent parent)
				{
					var node = new GeoNode(parent)
					{
						MinDistance = minDistance,
						MaxDistance = maxDistance,
						Local = parent.Cursor,
						ZTest = rt._zbuf,
					};
					this = reset;
					return node;
				}
			}

			public Runtime(
				Level level,
				Model3D mdl,
				GeoRoot root
#if DEBUG
				, ushort[] counter=null
#endif
				) : base(Instance, level.rom)
			{
				if (null == (object)level || null == (object)mdl || null == (object)root)
					throw new System.ArgumentNullException();
				this._zbuf = true;
				this.lvl = level;
				this.mdl = mdl;
				this.rootNode = root;

				this.nodeCurrent = null;
#if DEBUG
				this.counter = counter ?? new ushort[259];
#endif
			}
			public Runtime(Runtime copy)
				: base(copy)
			{
				this.lvl = copy.lvl;
				this.mdl = copy.mdl;
				this.rootNode = copy.rootNode;
				this.nodeCurrent = copy.nodeCurrent;
				this._zbuf = copy._zbuf;
				this.persistant = copy.persistant;
#if DEBUG
				this.counter = copy.counter;
#endif
			}
			public Runtime(Runtime copy, SegmentOffset segOff) : this(copy)
			{
				SetSegment(this, segOff);
			}
			private static void EvaluateCopyState(
				Runtime rt,
				Runtime originator)
			{
				Evaluate(rt);

				originator.nodeCurrent = rt.nodeCurrent;
				originator._zbuf = rt._zbuf;
				originator.persistant = rt.persistant;
			}
			

		}
		public static GeoRoot Parse(
			Model3D mdl,
			Level lvl,
			SegmentOffset segOff)
        {
#if DEBUG
			System.Console.WriteLine("Start parse");
#endif
			var root = new GeoRoot(mdl);
			var rt = new Runtime(lvl, mdl, root);

			Runtime.SetSegment(rt, segOff);
			root.Code = Runtime.Evaluate(rt);
			var endingNode = rt.nodeCurrent;
#if DEBUG
			System.Console.WriteLine("End parse:"+(null==endingNode?"null":endingNode.Depth.ToString()));

			{
				for (int i = 0; i < 256; i++)
					if (rt.counter[i] != 0)
						System.Console.WriteLine(i.ToString("X2") + ":" + rt.counter[i]);
				System.Console.WriteLine(rt.counter[256]+ "("+rt.counter[257]+"+"+rt.counter[258]+")");
			}
#endif
			root.Bind();

			return mdl.root=root;
		}
		public static GeoRoot Parse(Area area,
			SegmentOffset segOff)
		{
			return Parse(area.AreaModel, area.level, segOff);
		}
		// always takes over.
		protected sealed override bool RunCommand(Script.Runtime _rt, CommandInfo info, ref ByteSegment cmd)
		{
			Runtime rt = (Runtime)_rt;
			GeoParent switcher;
#if DEBUG
			rt.counter[cmd[0]]++;
			int put_pos = 0;
			if (null != (object)rt.nodeCurrent)
				for (; put_pos <= rt.nodeCurrent.Depth; ++put_pos)
					System.Console.Write("| ");
			if (cmd[0] == 0x05)
				System.Console.Write("\'-");
			else if (cmd[0] == 0x04)
				System.Console.Write("+-");
			else
				System.Console.Write("|-");
			put_pos++;
			put_pos <<= 1;

			System.Console.Write(bytesToUInt16(cmd).ToString("X4"));
			//System.Console.Write('+');
			//System.Console.Write(cmd.Length.ToString("00"));
			put_pos += 4;
			while (put_pos++ < 40) System.Console.Write(' ');
			// Nintendo is very smart about selecting values with meaning.
			// this should help decipher whats going on.
			if (0 != (cmd[0] & 1)) { System.Console.Write('A'); put_pos++; }
			if (0 != (cmd[0] & 2)) { System.Console.Write('B'); put_pos++; }
			if (0 != (cmd[0] & 4)) { System.Console.Write('C'); put_pos++; }
			if (0 != (cmd[0] & 8)) { System.Console.Write('D'); put_pos++; }
			if (0 != (cmd[0] & 16)) { System.Console.Write('E'); put_pos++; }
			if (0 != (cmd[0] & 32)) { System.Console.Write('F'); put_pos++; }
			if (0 != (cmd[0] & 64)) { System.Console.Write('G'); put_pos++; }
			if (0 != (cmd[0] & 128)) { System.Console.Write('H'); put_pos++; }
			while (put_pos++ < 49) System.Console.Write(' ');

			for (int b = 2; b < cmd.Length; b++)
			{
				if (b != 2) System.Console.Write(' ');
				System.Console.Write(cmd[b].ToString("X2"));
			}
			System.Console.WriteLine();
#endif
			if (cmd[0] != 0x05 && 
				(switcher = rt.currentParent).isSwitch &&
				switcher.switchPos != 1)
			{
				if (switcher.switchFunc == 0x8029DB48)
				{
					//rom.printArray(cmd, cmdLen);
					//Console.WriteLine(nodeCurrent.switchPos);

					cmd = default(ByteSegment);
				}
			}else if (0 != (info.Traits & CommandInfoTraits.HasDelegate))
				((Runtime.Command)info.Delegate)(rt, ref cmd);

			if ((switcher = rt.currentParent).isSwitch)
				switcher.switchPos++;

			// we did (or decided not to do) the command so don't have the base do it.
			return false;
		}
		partial class Runtime
		{
			static public void CMD_00(Runtime rt, ref ByteSegment cmd)
			{
				/*
				var Address = bytesToSegmentOffset(cmd, 4);
				
				// layoutCommandPtr = Pointer to current Geo layout command
				// returnPtr = Pointer to array/table to where branch return pointers are stored.
				// branchDepth = Branch depth value.
				*(returnPtr + (branchDepth * 4)) = *layoutCommandPtr + 8; // Stores the pointer to the next geo command
				*branchDepth += 1; // Increment branch value

				*(returnPtr + (branchDepth * 4)) = (*0x8038BD78 << 16) + *0x8038BD7E; // Store another address
				*branchDepth += 1; // Increment branch value
				*0x8038BD7E = *branchDepth;

				layoutCommandPtr = SegmentedToVirtual(layoutCommandPtr + 4); // Go to the segmented address.
																 /*
																 end = true;
																 code = 0x00;*/
																 

			}
			static public void CMD_01(Runtime rt, ref ByteSegment cmd)
			{
				Exit(rt, 0x01);
			}
			static public void CMD_03(Runtime rt, ref ByteSegment cmd)
			{
				Exit(rt, 0x03);
			}

			static public void CMD_02(Runtime rt, ref ByteSegment cmd)
			{
				if (!rt.ended)
				{
					var jump = new Runtime(rt, bytesToSegmentOffset(cmd, 4));
					EvaluateCopyState(jump, rt);

					if (jump.returnCode != 0x03)
						Exit(rt, rt.returnCode);
				}
			}
			static public void CMD_04(Runtime rt, ref ByteSegment cmd)
			{
				var parent = rt.currentParent;

				GeoNode newNode = rt.persistant.Create(rt, parent);
				if (parent.callSwitch)
				{
					newNode.switchPos = 0;
					newNode.switchCount = parent.switchCount;
					newNode.switchFunc = parent.switchFunc;
					newNode.isSwitch = true;
				}
				//if(nodeCurrent.Depth==0) System.Console.WriteLine("Hit Top");
				rt.nodeCurrent = newNode;
			}
			static public void CMD_05(Runtime rt,ref ByteSegment cmd)
			{
				if (null == (object)rt.nodeCurrent)
				{
#if DEBUG
					System.Console.WriteLine("Smashed Bottom");
					rt.counter[256]++;
#endif
				}
				else if (0 == rt.nodeCurrent.Depth)
				{
					rt.nodeCurrent = null;
				}
				else
				{
					rt.nodeCurrent = rt.nodeCurrent.Outer;
				}
			}

			static public void CMD_0E(Runtime rt, ref ByteSegment cmd)
			{
				var switcher = rt.currentParent;
				switcher.switchFunc = bytesToUInt32(cmd, 4);
				// Special Ignore cases
				//if (nodeCurrent.switchFunc == 0x8029DBD4) return;
				switcher.switchCount = cmd[3];
				//nodeCurrent.callSwitch = true;
			}

			static public void CMD_0A(Runtime rt, ref ByteSegment cmd)
			{
				if (cmd[1] != 0)
				{
					rt.rootNode.FrustumAsm = bytesToUInt32(cmd, 8);
				}
				rt.rootNode.FOVInt = bytesToUInt16(cmd, 2);
				rt.rootNode.Near = bytesToUInt16(cmd, 4);
				rt.rootNode.Far = bytesToUInt16(cmd, 6);

			}
			static public void CMD_0D(Runtime rt,ref ByteSegment cmd)
			{
				rt.persistant.minDistance = bytesToInt16(cmd, 4);
				rt.persistant.maxDistance = bytesToInt16(cmd, 6);
			}
			static public unsafe void CMD_10(Runtime rt, ref ByteSegment cmd)
			{
				var parent = rt.currentParent;
				parent.Cursor.translation = bytesToVector3s(cmd, 4);
				parent.Cursor.rotation = bytesToVector3s(cmd, 10);
			}
			static public unsafe void CMD_11(Runtime rt, ref ByteSegment cmd)
			{
				rt.rootNode.Pivot= bytesToVector3s(cmd, 2);
				//persistant.transform.translation =default(Vector3s)- bytesToVector3s(cmd, 2);
			}
			private static void F3D_Command(
				Runtime rt,
				GeoModel model,
				byte drawLayer,
				SegmentOffset segOff)
			{
				using (ModelBuilder.NodeBinder.Bind(
					rt.mdl.builder,
					model))
					Fast3DScripts.Parse(
						rt.mdl,
						rt.lvl,
						segOff,
						overrideDrawLayer:drawLayer);

				model.Build();
			}
			static public unsafe void CMD_13(Runtime rt, ref ByteSegment cmd)
			{
				var seg_offset = bytesToSegmentOffset(cmd, 8);
				rt.currentParent.Cursor.translation = bytesToVector3s(cmd, 2);
				if (0u!=seg_offset)
				{
					CMD_04(rt, ref cmd);
					var node = rt.nodeCurrent;
					// Don't bother processing duplicate display lists.
					//if (!mdl.hasGeoDisplayList(seg_offset.Offset))
					F3D_Command(rt, node.StartModel(), cmd[1], seg_offset);
					//else
					//{
					//	Console.WriteLine("dupe:"+seg_offset.ToString("X8")+" "+x+" "+y+" "+z);
					//}
					//mdl.builder.Offset = default(Vector3);
					CMD_05(rt, ref cmd);
				}
				else
				{
#if DEBUG
					//mdl.builder.Offset = new Vector3(-x, -y, -z);
					rt.counter[257]++;
#endif
				}

			}

			static public unsafe void CMD_14(Runtime rt, ref ByteSegment cmd)
			{
				rt.nodeCurrent.forceBillboard = true;
			}

			static public unsafe void CMD_15(Runtime rt, ref ByteSegment cmd)
			{
				// if (bytesToInt(cmd, 4, 4) != 0x07006D70) return;
				var node = rt.nodeCurrent;
					var segOff = bytesToSegmentOffset(cmd, 4);
#if DEBUG
				if (segOff == 0u) rt.counter[258]++;
#endif
				// Don't bother processing duplicate display lists.
				//if (!mdl.hasGeoDisplayList(segOff.Offset))
				{
					Globals.DEBUG_PDL = segOff;
					F3D_Command(rt, node.StartModel(), cmd[1], segOff);
				}
				//else
				//{
				//	Console.WriteLine("dupe:" + bytesToInt(cmd, 4, 4).ToString("X8") + " ---");
				//}
			}
			static public void CMD_16(Runtime rt, ref ByteSegment cmd)
			{

				// this seems to be what is expected.
				// when this bit is 1, 0x05 is called (1+ count of 0x04 commands)
				//if (0 != cmd[3])
				//{
				//	CMD_04(cmd);
				//}
				rt.rootNode.Shadow =rt.nodeCurrent;
				rt.rootNode.ShadowShape = cmd[4];
				rt.rootNode.ShadowTransparency = cmd[5];
				rt.rootNode.ShadowSize = bytesToUInt16(cmd, 6);
			}
			static public void CMD_19(Runtime rt, ref ByteSegment cmd)
			{
				rt.rootNode.Background = rt.currentParent;
				rt.rootNode.BackgroundRam = bytesToUInt32(cmd, 4);
				if (0 == rt.rootNode.BackgroundRam)
				{
					rt.rootNode.BackgroundColor = new Color4b(bytesToUInt16(cmd, 2));
					rt.rootNode.BackgroundImageIndex = ushort.MaxValue;
				}
				else
				{
					rt.rootNode.BackgroundColor = Color4b.White;
					rt.rootNode.BackgroundImageIndex = bytesToUInt16(cmd, 2);
				}
			}
			static public void CMD_1C(Runtime rt, ref ByteSegment cmd)
			{
				CMD_04(rt, ref cmd);
				rt.nodeCurrent.ramAddress = bytesToUInt32(cmd, 8);
			}

			static public unsafe void CMD_1D(Runtime rt, ref ByteSegment cmd)
			{
				var w = bytesToUInt16(cmd, 2);
				rt.currentParent.Cursor.scale.Value = bytesToUInt32(cmd, 4);
			}
			static public void CMD_20(Runtime rt, ref ByteSegment cmd)
			{
				rt.rootNode.DrawDistance = bytesToUInt16(cmd, 2);
			}
		}

		protected sealed override CommandInfo GetCommandInfo(byte AA)
		{
			Runtime.Command Command;
			switch (AA)
			{
				case 0x00: Command = Runtime.CMD_00; break;
				case 0x01: Command = Runtime.CMD_01; break;
				case 0x03: Command = Runtime.CMD_03; break;
				case 0x02: Command = Runtime.CMD_02; break;
				case 0x04: Command = Runtime.CMD_04; break;
				case 0x05: Command = Runtime.CMD_05; break;
				case 0x0A: Command = Runtime.CMD_0A; break;
				case 0x0D: Command = Runtime.CMD_0D; break;
				case 0x0E: Command = Runtime.CMD_0E; break;
				case 0x10: Command = Runtime.CMD_10; break;
				case 0x11: Command = Runtime.CMD_11; break;
				case 0x13: Command = Runtime.CMD_13; break;
				case 0x14: Command = Runtime.CMD_14; break;
				case 0x15: Command = Runtime.CMD_15; break;
				case 0x16: Command = Runtime.CMD_16; break;
				case 0x19: Command = Runtime.CMD_19; break;
				case 0x1C: Command = Runtime.CMD_1C; break;
				case 0x1D: Command = Runtime.CMD_1D; break;
				case 0x20: Command = Runtime.CMD_20; break;
				default: Command = null; break;
			}
			return new CommandInfo(this, Command,
				getCmdLength(AA, 0), getCmdLength(AA, 255));
		}

		private static byte getCmdLength(byte cmd, byte cmd2)
        {
            switch (cmd)
            {
				case 0x00:
                case 0x02:
                case 0x0D:
                case 0x0E:
                case 0x11:
                case 0x12:
                case 0x14:
				case 0x15:
				case 0x16:
                case 0x18:
                case 0x19:
				case 0x1A:
				case 0x1E:
					return 0x08;
				case 0x1D:
					return 0==(0x80 & cmd2) ? (byte)0x08 : (byte)0x0C;
				case 0x08:
                case 0x13:
                case 0x1C:
                    return 0x0C;
                case 0x1F:
                    return 0x10;
                case 0x0F:
                case 0x10:
                    return 0x14;
                default:
                    return 0x04;
				case 0x0A:
					return 0 != cmd2 ? (byte)0x0C : (byte)0x08;
			}
        }

    }
}
