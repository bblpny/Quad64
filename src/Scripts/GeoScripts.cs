using BubblePony.Alloc;
using BubblePony.Integers;

namespace Quad64.Scripts
{
	internal sealed class GeoScripts : Script
    {
		private partial struct Runtime
		{
			public readonly ROM rom;
			public readonly Level lvl;
			public readonly Model3D mdl;
			public readonly GeoRoot rootNode;
			public GeoParent currentParent => (GeoParent)nodeCurrent ?? rootNode;
			public GeoNode nodeCurrent;
			public ByteSegment data;
			private Persistant persistant;
			private struct Persistant
			{
				public short minDistance, maxDistance;

				public static Persistant reset => default(Persistant);

				public GeoNode Create(ref Runtime rt, GeoParent parent)
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

			public int off;
			private byte _seg;
			private bool _zbuf;
			public bool end;
			public byte code;
#if DEBUG
			public readonly ushort[] counter;
#endif
			public byte seg
			{
				get => _seg;
				set
				{
					_seg = value;
					if (_seg == 0)
					{
						data = default(ByteSegment);
						end = true;
					}
					else
					{
						data = rom.getSegment(seg);
					}
				}
			}
			static public bool GetCmd(ref Runtime rt, out ByteSegment cmd )
			{
				if (rt.end || rt.off < 0 || rt._seg == 0 || rt.off >= rt.data.Length)
				{
					cmd = default(ByteSegment);
					rt.end = true;
				}
				else
				{
					rt.data.Segment((uint)rt.off, (uint)getCmdLength(rt.data[rt.off], rt.data[rt.off+1]), out cmd);
					rt.end = 0==cmd.Length;
				}
				return !rt.end;
			}
			unsafe public Runtime(Level level, Model3D mdl, GeoRoot root
#if DEBUG
				, ushort[] counter=null
#endif
				)
			{
				this._zbuf = true;
				this.rom = level.rom;
				this.lvl = level;
				this.mdl = mdl;
				this.rootNode = root;
				this.nodeCurrent = null;
				this._seg = 0;
				this.code = 0;
				this.end = false;
				this.data = default(ByteSegment);
				this.off = -1;
#if DEBUG
				this.counter = counter ?? new ushort[259];
#endif
				this.persistant = Persistant.reset;
			}
			public Runtime(ref Runtime copy, byte seg, uint off)
				: this(copy.lvl, copy.mdl, copy.rootNode
#if DEBUG
					  , copy.counter
#endif
					  )
			{
				this.seg = seg;
				this.off = (int)off;
				this.nodeCurrent = copy.nodeCurrent;
				this._zbuf = copy._zbuf;
				this.persistant = copy.persistant;
			}
			public Runtime(ref Runtime copy, SegmentOffset segOff) : this(ref copy, segOff.Segment, segOff.Offset) { }
			

			public static void Execute(
				ref Runtime rt,
				ref Runtime originator)
			{
				Evaluate(ref rt);
				originator.nodeCurrent = rt.nodeCurrent;
				originator._zbuf = rt._zbuf;
				originator.persistant = rt.persistant;
			}

			public delegate void Command(ref Runtime rt, ref ByteSegment cmd);

			static class CommandList
			{
				static CommandList()
				{

				}
				static object thread_lock = new object();
				static BitList256 test_once;
				static BitList256 test_result;


				public static void EnsureMissing(ByteSegment command)
				{
					bool result;
					byte name = command[0];
					lock (thread_lock)
					{
						if (test_once.Add(name))
						{
							result = null != typeof(Runtime)
								.GetMethod("CMD_" + name.ToString("X2"), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
							if (result) test_result.Add(name);
						}
						else
						{
							result = test_result.Contains(name);
						}
					}
					if (result) throw new System.InvalidOperationException("Code is wrong, should have added command to switch statement " + (name.ToString("X2")));
				}
			}
			static unsafe public void Evaluate(ref Runtime rt)
			{
				GeoParent switcher;
				ByteSegment cmd;
				var enter = new SegmentOffset() { Segment = rt._seg, Offset = (UInt24)rt.off };
				//Console.WriteLine("--->" + enter);
				while (GetCmd(ref rt, out cmd))
				{
#if DEBUG
					rt.counter[cmd[0]]++;
					int put_pos = 0;
					if(null!=(object)rt.nodeCurrent)
						for (; put_pos <= rt.nodeCurrent.Depth; ++put_pos)
							System.Console.Write("| ");
					if (cmd[0] == 0x05)
						System.Console.Write("\'-");
					else if (cmd[0] == 0x04)
						System.Console.Write("+-");
					else
						System.Console.Write("|-");
					put_pos++;
					put_pos <<=1;

					System.Console.Write(bytesToUInt16(cmd).ToString("X4"));
					//System.Console.Write('+');
					//System.Console.Write(cmd.Length.ToString("00"));
					put_pos +=4;
					while (put_pos++ < 40) System.Console.Write(' ');
					// nintendo is very smart about selecting values with meaning.
					// this should help deciphrer wahts going on.
					if (0 != (cmd[0] & 1)) { System.Console.Write('A'); put_pos++; }
					if (0 != (cmd[0] & 2)) { System.Console.Write('B'); put_pos++; }
					if (0 != (cmd[0] & 4)) { System.Console.Write('C'); put_pos++; }
					if (0 != (cmd[0] & 8)) { System.Console.Write('D'); put_pos++; }
					if (0 != (cmd[0] & 16)) { System.Console.Write('E'); put_pos++; }
					if (0 != (cmd[0] & 32)) { System.Console.Write('F'); put_pos++; }
					if (0 != (cmd[0] & 64)) {System.Console.Write('G'); put_pos++; }
					if (0 != (cmd[0] & 128)) { System.Console.Write('H'); put_pos++; }
					while (put_pos++ < 49) System.Console.Write(' ');

					for (int b = 2; b < cmd.Length; b++)
					{
						if (b != 2) System.Console.Write(' ');
						System.Console.Write(cmd[b].ToString("X2"));
					}
					System.Console.WriteLine();
#endif
					if (cmd.Byte[0] != 0x05 && (switcher=rt.currentParent).isSwitch && switcher.switchPos != 1)
					{
						if (switcher.switchFunc == 0x8029DB48)
						{
							//rom.printArray(cmd, cmdLen);
							//Console.WriteLine(nodeCurrent.switchPos);
						}
						switcher.switchPos++;
						rt.off += cmd.Length;
						continue;
					}
					/*
					Console.Write("?CMD" + cmd[0].ToString("X2"));
					for (int ic = 0; ic < cmd.Length; ic += 2)
						Console.Write((0==ic?"!":",") + (0 == ic ? (short)cmd[1] : bytesToInt16(cmd, ic)));
					if (1==(cmd.Length&1))
						Console.Write(",!" + cmd[cmd.Length-1]);
					Console.WriteLine();*/

					switch (cmd.Byte[0])
					{
						case 0x00:CMD_00(ref rt, ref cmd);break;
						case 0x01:CMD_01(ref rt, ref cmd);break;
						case 0x03:CMD_03(ref rt, ref cmd);break;
						case 0x02:CMD_02(ref rt, ref cmd);break;
						case 0x04:CMD_04(ref rt, ref cmd);break;
						case 0x05:CMD_05(ref rt, ref cmd);break;
						case 0x0A:CMD_0A(ref rt, ref cmd);break;
						case 0x0D:CMD_0D(ref rt, ref cmd);break;
						case 0x0E:CMD_0E(ref rt, ref cmd);break;
						case 0x10:CMD_10(ref rt, ref cmd);break;
						case 0x11:CMD_11(ref rt, ref cmd);break;
						case 0x13:CMD_13(ref rt, ref cmd);break;
						case 0x14:CMD_14(ref rt, ref cmd);break;
						case 0x15:CMD_15(ref rt, ref cmd);break;
						case 0x16:CMD_16(ref rt, ref cmd);break;
						case 0x19:CMD_19(ref rt, ref cmd);break;
						case 0x1C:CMD_1C(ref rt, ref cmd);break;
						case 0x1D:CMD_1D(ref rt, ref cmd);break;
						case 0x20:CMD_20(ref rt, ref cmd);break;
						default:/*MissingCheck.EnsureMissing(cmd);*/break;
					}

					rt.off += cmd.Length;

					if ((switcher=rt.currentParent).isSwitch)
						switcher.switchPos++;
				}
				//Console.WriteLine("<----" + enter + "=" + code);
			}
		}
        public static GeoRoot parse(Model3D mdl, Level lvl, byte seg, uint off)
        {
#if DEBUG
			System.Console.WriteLine("Start parse");
#endif
			var root = new GeoRoot(mdl);
			var rt = new Runtime(lvl, mdl, root)
			{
				off = (int)off,
				seg = seg,
			};
			Runtime.Evaluate(ref rt);
			root.Code = rt.code;
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
		public static GeoRoot parse(Area area, byte seg, uint off)
		{
			return parse(area.AreaModel, area.level, seg, off);
		}
		partial struct Runtime
		{
			static public void CMD_00(ref Runtime rt, ref ByteSegment cmd)
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
			static public void CMD_01(ref Runtime rt, ref ByteSegment cmd)
			{
				rt.end = true;
				rt.code = 0x01;
			}
			static public void CMD_03(ref Runtime rt, ref ByteSegment cmd)
			{
				rt.end = true;
				rt.code = 0x03;
			}

			static public void CMD_02(ref Runtime rt, ref ByteSegment cmd)
			{
				if (!rt.end)
				{
					var jump = new Runtime(ref rt, bytesToSegmentOffset(cmd, 4));
					Execute(ref jump, ref rt);
					if (jump.code != 0x03)
					{
						rt.code = jump.code;
						rt.end = true;
					}
				}
			}
			static public void CMD_04(ref Runtime rt, ref ByteSegment cmd)
			{
				var parent = rt.currentParent;

				GeoNode newNode = rt.persistant.Create(ref rt, parent);
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
			static public void CMD_05(ref Runtime rt,ref ByteSegment cmd)
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

			static public void CMD_0E(ref Runtime rt, ref ByteSegment cmd)
			{
				var switcher = rt.currentParent;
				switcher.switchFunc = bytesToUInt32(cmd, 4);
				// Special Ignore cases
				//if (nodeCurrent.switchFunc == 0x8029DBD4) return;
				switcher.switchCount = cmd[3];
				//nodeCurrent.callSwitch = true;
			}

			static public void CMD_0A(ref Runtime rt, ref ByteSegment cmd)
			{
				if (cmd[1] != 0)
				{
					rt.rootNode.FrustumAsm = bytesToUInt32(cmd, 8);
				}
				rt.rootNode.FOVInt = bytesToUInt16(cmd, 2);
				rt.rootNode.Near = bytesToUInt16(cmd, 4);
				rt.rootNode.Far = bytesToUInt16(cmd, 6);

			}
			static public void CMD_0D(ref Runtime rt,ref ByteSegment cmd)
			{
				rt.persistant.minDistance = bytesToInt16(cmd, 4);
				rt.persistant.maxDistance = bytesToInt16(cmd, 6);
			}
			static public unsafe void CMD_10(ref Runtime rt, ref ByteSegment cmd)
			{
				var parent = rt.currentParent;
				parent.Cursor.translation = bytesToVector3s(cmd, 4);
				parent.Cursor.rotation = bytesToVector3s(cmd, 10);
			}
			static public unsafe void CMD_11(ref Runtime rt, ref ByteSegment cmd)
			{
				rt.rootNode.Pivot= bytesToVector3s(cmd, 2);
				//persistant.transform.translation =default(Vector3s)- bytesToVector3s(cmd, 2);
			}
			private void F3D_Command(GeoModel model, byte drawLayer, byte seg, uint off)
			{
				using (ModelBuilder.NodeBinder.Bind(this.mdl.builder, model))
					Fast3DScripts.parse(mdl, lvl, seg, off, drawLayer);
				model.Build();
			}
			static public unsafe void CMD_13(ref Runtime rt, ref ByteSegment cmd)
			{
				var seg_offset = bytesToSegmentOffset(cmd, 8);
				rt.currentParent.Cursor.translation = bytesToVector3s(cmd, 2);
				if (0u!=seg_offset)
				{
					CMD_04(ref rt, ref cmd);
					var node = rt.nodeCurrent;
					// Don't bother processing duplicate display lists.
					//if (!mdl.hasGeoDisplayList(seg_offset.Offset))
					rt.F3D_Command(node.StartModel(), cmd[1], seg_offset.Segment, seg_offset.Offset);
					//else
					//{
					//	Console.WriteLine("dupe:"+seg_offset.ToString("X8")+" "+x+" "+y+" "+z);
					//}
					//mdl.builder.Offset = default(Vector3);
					CMD_05(ref rt, ref cmd);
				}
				else
				{
#if DEBUG
					//mdl.builder.Offset = new Vector3(-x, -y, -z);
					rt.counter[257]++;
#endif
				}

			}

			static public unsafe void CMD_14(ref Runtime rt, ref ByteSegment cmd)
			{
				rt.nodeCurrent.forceBillboard = true;
			}

			static public unsafe void CMD_15(ref Runtime rt, ref ByteSegment cmd)
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
						rt.F3D_Command(node.StartModel(), cmd[1], segOff.Segment, segOff.Offset);
					}
				//else
				//{
				//	Console.WriteLine("dupe:" + bytesToInt(cmd, 4, 4).ToString("X8") + " ---");
				//}
			}
			static public void CMD_16(ref Runtime rt, ref ByteSegment cmd)
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
			static public void CMD_19(ref Runtime rt, ref ByteSegment cmd)
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
			static public void CMD_1C(ref Runtime rt, ref ByteSegment cmd)
			{
				CMD_04(ref rt, ref cmd);
				rt.nodeCurrent.ramAddress = bytesToUInt32(cmd, 8);
			}

			static public unsafe void CMD_1D(ref Runtime rt, ref ByteSegment cmd)
			{
				// this seems to be what is expected.
				// when a list starts with this command it calls 0x05 (1+ count of 0x04 commands)
				//if (null == nodeCurrent)
				//	CMD_04(cmd);

				var w = bytesToUInt16(cmd, 2);
				/*
				if (0x08 == cmd.Length)
				{
					nodeCurrent.Local.scale.Value = bytesToUInt32(cmd, 4);
				}
				else
				{
					nodeCurrent.Local.scale.Value = bytesToUInt32(cmd, 4);
				}*/
				rt.currentParent.Cursor.scale.Value = bytesToUInt32(cmd, 4);
				//mdl.builder.applyScale(bytesToInt(cmd, 4, 4));
			}
			static public void CMD_20(ref Runtime rt, ref ByteSegment cmd)
			{
				rt.rootNode.DrawDistance = bytesToUInt16(cmd, 2);
			}
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
