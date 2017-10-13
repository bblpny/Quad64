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
			public GeoNode nodeCurrent;
			public ByteSegment data;
			private byte _seg;
			public int off;
			public bool end;
			public byte code;

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
			public bool GetCmd( out ByteSegment cmd )
			{
				if (end || off < 0 || _seg == 0 || off >= data.Length)
				{
					cmd = default(ByteSegment);
					end = true;
				}
				else
				{
					data.Segment((uint)off, (uint)getCmdLength(data[off], data[off+1]), out cmd);
					end = 0==cmd.Length;
				}
				return !end;
			}
			unsafe public Runtime(Level level, Model3D mdl, GeoRoot root, GeoNode current)
			{
				this.rom = level.rom;
				this.lvl = level;
				this.mdl = mdl;
				this.rootNode = root;
				this.nodeCurrent = current;
				this._seg = 0;
				this.code = 0;
				this.end = false;
				this.data = default(ByteSegment);
				this.off = -1;
			}
			public Runtime(ref Runtime copy, byte seg, uint off) : this(copy.lvl, copy.mdl, copy.rootNode, copy.nodeCurrent)
			{
				this.seg = seg;
				this.off = (int)off;
			}
			public Runtime(ref Runtime copy, SegmentOffset segOff) : this(ref copy, segOff.Segment, segOff.Offset) { }

			public byte Execute() { Evaluate(); return code; }
			public byte Execute(out GeoNode nodeCurrent) { Evaluate(); nodeCurrent = this.nodeCurrent; return code; }

			unsafe public void Evaluate()
			{
				ByteSegment cmd;
				var enter = new SegmentOffset() { Segment = _seg, Offset = (UInt24)off };
				//Console.WriteLine("--->" + enter);
				while (GetCmd(out cmd))
				{
					if (cmd.Byte[0] != 0x05 && nodeCurrent.isSwitch && nodeCurrent.switchPos != 1)
					{
						if (nodeCurrent.switchFunc == 0x8029DB48)
						{
							//rom.printArray(cmd, cmdLen);
							//Console.WriteLine(nodeCurrent.switchPos);
						}
						nodeCurrent.switchPos++;
						off += cmd.Length;
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
						case 0x00:
							CMD_00(cmd);
							break;
						case 0x01:
							CMD_01(cmd);
							break;
						case 0x03:
							CMD_03(cmd);
							break;
						case 0x02:
							CMD_02(cmd);
							break;
						case 0x04:
							CMD_04(cmd);
							break;
						case 0x05:
							CMD_05(cmd);
							break;
						case 0x0E:
							//rom.printArray(cmd, cmdLen);
							CMD_0E(cmd);
							break;
						case 0x10:
							CMD_10(cmd);
							//Console.Write("CMD10");
							//for (int ic = 0; ic < cmd.Length; ic += 2)
							//	Console.Write("," + (0==ic?(short)cmd[1]:bytesToInt16(cmd, ic)));
							//Console.WriteLine();
							break;
						case 0x11:
							CMD_11(cmd);
							//rom.printArray(cmd, cmdLen);
							//Console.Write("CMD11");
							//for (int ic = 0; ic < cmd.Length; ic += 2)
							//	Console.Write("," + (0 == ic ? (short)cmd[1] : bytesToInt16(cmd, ic)));
							//Console.WriteLine();
							//CMD_11(ref mdl, ref lvl, cmd);
							break;
						case 0x13:
							//rom.printArray(cmd, cmdLen);
							CMD_13(cmd);
							break;
						case 0x14:
							CMD_14(cmd);
							break;
						case 0x15:
							CMD_15(cmd);
							// rom.printArray(cmd, cmdLen);
							break;
						case 0x1D:
							CMD_1D(cmd);
							break;
						default:
							break;
					}

					off += cmd.Length;

					if (nodeCurrent.isSwitch)
						nodeCurrent.switchPos++;
				}
				//Console.WriteLine("<----" + enter + "=" + code);
			}
		}
        public static GeoRoot parse(Model3D mdl, Level lvl, byte seg, uint off)
        {
			var root = new GeoRoot(mdl);
			var firstNode = new GeoNode(root);
			root.Code = new Runtime(lvl, mdl, root, firstNode) {
				off = (int)off,
				seg = seg,
			}.Execute();
			root.Bind();
			return mdl.root=root;
		}
		public static GeoRoot parse(Area area, byte seg, uint off)
		{
			return parse(area.AreaModel, area.level, seg, off);
		}
		partial struct Runtime
		{
			public void CMD_00(ByteSegment cmd)
			{
				end = true;
				code = 0x00;
			}
			public void CMD_01(ByteSegment cmd)
			{
				end = true;
				code = 0x03;
			}
			public void CMD_03(ByteSegment cmd)
			{
				end = true;
				code = 0x03;
			}

			public void CMD_02(ByteSegment cmd)
			{
				end =(!end) && 0x03!=(code = new Runtime(ref this,bytesToSegmentOffset(cmd,4)).Execute(out nodeCurrent));
			}
			public void CMD_04(ByteSegment cmd)
			{
				GeoNode newNode = new GeoNode(nodeCurrent);
				if (nodeCurrent.callSwitch)
				{
					newNode.switchPos = 0;
					newNode.switchCount = nodeCurrent.switchCount;
					newNode.switchFunc = nodeCurrent.switchFunc;
					newNode.isSwitch = true;
				}
				nodeCurrent = newNode;
			}
			public void CMD_05(ByteSegment cmd)
			{
				nodeCurrent = (GeoNode)nodeCurrent.Outer;
			}

			public void CMD_0E(ByteSegment cmd)
			{
				nodeCurrent.switchFunc = bytesToUInt32(cmd, 4);
				// Special Ignore cases
				//if (nodeCurrent.switchFunc == 0x8029DBD4) return;
				nodeCurrent.switchCount = cmd[3];
				//nodeCurrent.callSwitch = true;
			}

			public unsafe void CMD_10(ByteSegment cmd)
			{
				nodeCurrent.Local.translation = bytesToVector3s(cmd, 4);
				nodeCurrent.Local.rotation= bytesToVector3s(cmd,10);
			}
			public unsafe void CMD_11(ByteSegment cmd)
			{
				//nodeCurrent.Local.translation += bytesToVector3s(cmd, 2);
			}
			private void F3D_Command(GeoModel model, byte drawLayer, byte seg, uint off)
			{
				using (ModelBuilder.NodeBinder.Bind(this.mdl.builder, model))
					Fast3DScripts.parse(mdl, lvl, seg, off, drawLayer);
				model.Build();
			}
			public unsafe void CMD_13(ByteSegment cmd)
			{
				var node = nodeCurrent;
				node.Local.translation += bytesToVector3s(cmd, 2);
				var seg_offset = bytesToSegmentOffset(cmd, 8);
				if (seg_offset.Offset != 0 || seg_offset.Segment!=0)
				{
					// Don't bother processing duplicate display lists.
					//if (!mdl.hasGeoDisplayList(seg_offset.Offset))
						F3D_Command(node.StartModel(), cmd[1], seg_offset.Segment, seg_offset.Offset);
					//else
					//{
					//	Console.WriteLine("dupe:"+seg_offset.ToString("X8")+" "+x+" "+y+" "+z);
					//}
					//mdl.builder.Offset = default(Vector3);
				}
				else
				{
					//mdl.builder.Offset = new Vector3(-x, -y, -z);
				}
			}

			public unsafe void CMD_14(ByteSegment cmd)
			{
				nodeCurrent.forceBillboard = true;
			}

			public unsafe void CMD_15(ByteSegment cmd)
			{
				// if (bytesToInt(cmd, 4, 4) != 0x07006D70) return;
				var node = nodeCurrent;
					var segOff = bytesToSegmentOffset(cmd, 4);
					// Don't bother processing duplicate display lists.
					//if (!mdl.hasGeoDisplayList(segOff.Offset))
					{
						Globals.DEBUG_PDL = segOff;
						F3D_Command(node.StartModel(), cmd[1], segOff.Segment, segOff.Offset);
					}
					//else
					//{
					//	Console.WriteLine("dupe:" + bytesToInt(cmd, 4, 4).ToString("X8") + " ---");
					//}
			}

			public unsafe void CMD_1D(ByteSegment cmd)
			{
				var w = bytesToUInt16(cmd, 2);
				if (0x08 == cmd.Length)
				{
					nodeCurrent.Local.scale.Value = bytesToUInt32(cmd, 4);
				}
				else
				{
					nodeCurrent.Local.scale.Value = bytesToUInt32(cmd, 4);
				}
				//mdl.builder.applyScale(bytesToInt(cmd, 4, 4));
			}
		}

        private static byte getCmdLength(byte cmd, byte cmd2)
        {
            switch (cmd)
            {
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
