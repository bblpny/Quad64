using System;
using BubblePony.Alloc;

namespace Quad64.Scripts
{
    internal sealed class LevelScripts : Script
    {
		//static byte static_most_length;
		private partial struct Runtime
		{
			public readonly ROM rom;
			public readonly Level lvl;
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
			unsafe public Runtime(Level level)
			{
				this.rom = level.rom;
				this.lvl = level;
				this._seg = 0;
				this.code = 0;
				this.end = false;
				this.data = default(ByteSegment);
				this.off = -1;
			}
			public Runtime(ref Runtime copy, byte seg, uint off) : this(copy.lvl)
			{
				this.seg = seg;
				this.off = (int)off;
			}
			public Runtime(ref Runtime copy, SegmentOffset off) : this(ref copy, off.Segment, off.Offset) { }

			public byte Execute() { Evaluate(); return code; }

			public bool GetCmd(out ByteSegment cmd)
			{
				if (!end && _seg != 0 && off >= 0 && off < data.Length)
				{
					data.Segment((uint)off, data[off + 1], out cmd);
					return cmd.Length!=0;
				}
				else
				{
					end = true;
					cmd = default(ByteSegment);
					return false;
				}
			}
			unsafe public void Evaluate()
			{
				ByteSegment cmd;
				while (GetCmd(out cmd))
				{
					// rom.printArray(cmd, cmdLen);
					switch (cmd[0])
					{
						case 0x00:
							CMD_00(cmd);
							break;
						case 0x01:
							CMD_01(cmd);
							break;
						case 0x02:
							CMD_02(cmd);
							break;
						case 0x05:
							CMD_05(cmd);
							break;
						case 0x06:
							CMD_06(cmd);
							break;
						case 0x07:
							CMD_07(cmd);
							break;
						case 0x0C:
							CMD_0C(cmd);
							break;
						case 0x17:
							CMD_17(cmd);
							break;
						case 0x18:
						case 0x1A:
							CMD_18(cmd);
							break;
						case 0x1F:
							//Globals.DEBUG_PLG = true;
							CMD_1F(cmd);
							break;
						case 0x21:
							CMD_21(cmd);
							break;
						case 0x22:
							//Globals.DEBUG_PLG = false;
							CMD_22(cmd);
							break;
						case 0x24:
							CMD_24(cmd);
							break;
						case 0x26:
							CMD_26(cmd);
							break;
						case 0x27:
							CMD_27(cmd);
							break;
						case 0x28:
							CMD_28(cmd);
							break;
						case 0x2E:
							CMD_2E(cmd);
							break;
						case 0x39:
							CMD_39(cmd);
							break;
					}
					off += cmd.Length;
				}
			}
		}
		public static int parse(Level lvl, byte seg, uint off)
		{
			return new Runtime(lvl)
			{
				seg = seg,
				off = (int)off,
			}.Execute();
		}
		partial struct Runtime
		{
			public void CMD_00(ByteSegment cmd)
			{
				SegmentOffset segOff = new SegmentOffset
				{
					Segment = cmd[3],
					Offset = bytesToUInt24(cmd, 13),
				};
				uint start = bytesToUInt32(cmd, 4);
				uint end = bytesToUInt32(cmd, 8);
				rom.setSegment(segOff.Segment, start, end, false);
				if (segOff.Segment == 0x14) return;

				new Runtime(ref this, segOff.Segment, segOff.Offset).Evaluate();
			}
			public void CMD_01(ByteSegment cmd)
			{
				CMD_00(cmd);
			}
			public void CMD_02(ByteSegment cmd)
			{
				end = true;
				code = 0x02;
			}
			public void CMD_05(ByteSegment cmd)
			{
				var segOff = bytesToSegmentOffset(cmd, 4);
				if (segOff.Segment == this._seg && 
					( ((long)segOff.Offset - (long)this.off) == -4)) {
					Console.Error.WriteLine("Infinite loop detected!");
					code = 0x02;
				}
				else
				{
					code = new Runtime(ref this, segOff.Segment, segOff.Offset).Execute();
				}
				end = true;
			}

			public void CMD_06(ByteSegment cmd)
			{
				if(0x02 == new Runtime(ref this, bytesToSegmentOffset(cmd, 4)).Execute())
				{
					end = true;
					code = 0x02;
				}
			}
			public void CMD_07(ByteSegment cmd)
			{
				end = true;
				code = 0x07;
			}

			public void CMD_0C(ByteSegment cmd)
			{
				if (null != lvl && bytesToInt32(cmd, 4) == lvl.LevelID)
					new Runtime(ref this, bytesToSegmentOffset(cmd, 8)).Evaluate();
			}

			public void CMD_17(ByteSegment cmd)
			{
				rom.setSegment(cmd[3], bytesToUInt32(cmd, 4), bytesToUInt32(cmd, 8), false);
			}

			public void CMD_18(ByteSegment cmd)
			{
				byte seg = cmd[3];
				uint start = bytesToUInt32(cmd, 4);
				var MIO0_header = ROM.getSubArray(rom.Bytes, start, 0x10);

				if (bytesToInt32(MIO0_header, 0) == 0x4D494F30) // Check MIO0 signature
				{
					rom.setSegment(seg, start, bytesToUInt32(cmd, 8), true);
				}//else?
			}

			public void CMD_1F(ByteSegment cmd)
			{
				byte areaID = cmd[2];
				var segOff = bytesToSegmentOffset(cmd,4);

				Area newArea = new Area(areaID, segOff.Value, lvl);
				newArea.AreaModel.GeoDataSegAddress = segOff.Value;
				GeoScripts.parse(newArea, segOff.Segment, segOff.Offset);
				lvl.Areas.Add(newArea);
				lvl.CurrentAreaID = areaID;
				newArea.AreaModel.buildBuffers();
				// newArea.AreaModel.outputTextureAtlasToPng("Area_"+areaID+"_TexAtlus.png");
			}

			public void CMD_21(ByteSegment cmd)
			{
				byte modelID = cmd[3];
				var segOff = bytesToSegmentOffset(cmd, 4);

				Model3D newModel = new Model3D();
				newModel.GeoDataSegAddress = segOff.Value;
				lvl.AddObjectCombos(modelID, newModel.GeoDataSegAddress);

				if (rom.hasSegment(segOff.Segment))
					Fast3DScripts.parse(newModel, lvl, segOff.Segment, segOff.Offset);
				//if (lvl.ModelIDs.ContainsKey(modelID))
				//	lvl.ModelIDs.Remove(modelID);
				lvl.ModelIDs.Remove(modelID);
				newModel.buildBuffers();
				lvl.ModelIDs.Add(modelID, newModel);
			}
			public void CMD_22(ByteSegment cmd)
			{
				byte modelID = cmd[3];
				var segOff = bytesToSegmentOffset(cmd, 4);
				//Console.WriteLine("Size of seg 0x"+seg.ToString("X2")+" = " + rom.getSegment(seg).Length);
				Model3D newModel = new Model3D();
				newModel.GeoDataSegAddress = segOff.Value;
				lvl.AddObjectCombos(modelID, newModel.GeoDataSegAddress);
				if (rom.hasSegment(segOff.Segment))
					GeoScripts.parse(newModel, lvl, segOff.Segment, segOff.Offset);
				//if (lvl.ModelIDs.ContainsKey(modelID))
				//	lvl.ModelIDs.Remove(modelID);
				lvl.ModelIDs.Remove(modelID);
				newModel.buildBuffers();
				lvl.ModelIDs.Add(modelID, newModel);
			}

			public void CMD_24(ByteSegment cmd)
			{
				Object3D newObj = new Object3D(lvl, cmd, Object3DCategory.Object);
				/*
				if (rom.isSegmentMIO0(seg))
				{
					newObj.MakeReadOnly();
					newObj.Address = "N/A";
				}
				else
				{
					newObj.Address = "0x" + rom.decodeSegmentAddress(seg, (uint)off).ToString("X");
				}*/
				byte actMask = cmd[2];
				newObj.AllActs = (actMask == 0x1F);
				newObj.Act1 = ((actMask & 0x1) == 0x1);
				newObj.Act2 = ((actMask & 0x2) == 0x2);
				newObj.Act3 = ((actMask & 0x4) == 0x4);
				newObj.Act4 = ((actMask & 0x8) == 0x8);
				newObj.Act5 = ((actMask & 0x10) == 0x10);
				newObj.Act6 = ((actMask & 0x20) == 0x20);
				newObj.ShowHideActs(newObj.AllActs);
				newObj.ModelID = cmd[3];
				newObj.pos = bytesToVector3s(cmd,0x4);
				newObj.rot = bytesToVector3s(cmd,0x0A);
				newObj.MakeBehaviorReadOnly(false);
				newObj.MakeModelIDReadOnly(false);
				newObj.setBehaviorFromAddress(bytesToUInt32(cmd, 0x14));
				newObj.BehaviorParameter1 = cmd[0x10];
				newObj.BehaviorParameter2 = cmd[0x11];
				newObj.BehaviorParameter3 = cmd[0x12];
				newObj.BehaviorParameter4 = cmd[0x13];
				//newObj.level = lvl;
				lvl.getCurrentArea().Objects.Add(newObj.PreAdd());
			}


			public void CMD_26(ByteSegment cmd)
			{
				Warp warp = new Warp(lvl,cmd, false);
				/*
				if (rom.isSegmentMIO0(seg))
				{
					warp.MakeReadOnly();
					warp.Address = "N/A";
				}
				else
				{
					warp.Address = "0x" + rom.decodeSegmentAddress(seg, (uint)off).ToString("X");
				}*/
				warp.WarpFrom_ID = cmd[2];
				warp.WarpTo_LevelID = cmd[3];
				warp.WarpTo_AreaID = cmd[4];
				warp.WarpTo_WarpID = cmd[5];
				lvl.getCurrentArea().Warps.Add(warp);
			}

			public void CMD_27(ByteSegment cmd)
			{
				Warp warp = new Warp(lvl,cmd, true);
				/*
				if (rom.isSegmentMIO0(seg))
				{
					warp.MakeReadOnly();
					warp.Address = "N/A";
				}
				else
				{
					warp.Address = "0x" + rom.decodeSegmentAddress(seg, (uint)off).ToString("X");
				}*/
				warp.WarpFrom_ID = cmd[2];
				warp.WarpTo_LevelID = cmd[3];
				warp.WarpTo_AreaID = cmd[4];
				warp.WarpTo_WarpID = cmd[5];
				lvl.getCurrentArea().PaintingWarps.Add(warp);
			}
			public void CMD_28(ByteSegment cmd)
			{
				WarpInstant warp = new WarpInstant(lvl,cmd);
				/*
				if (rom.isSegmentMIO0(seg))
				{
					warp.MakeReadOnly();
					warp.Address = "N/A";
				}
				else
				{
					warp.Address = "0x" + rom.decodeSegmentAddress(seg, (uint)off).ToString("X");
				}*/
				warp.TriggerID = cmd[2];
				warp.AreaID = cmd[3];
				warp.Tele = bytesToVector3s(cmd, 4);
				lvl.getCurrentArea().InstantWarps.Add(warp);
			}

			/* Process collision map, Special Objects, and <S> water boxes </S>. */
			public void CMD_2E(ByteSegment cmd)
			{
				Object3D newObj;
				ByteSegment objDat;
				if (cmd.Length < 8)
					return;
				uint ent_behavior;
				ushort sub_cmd = 0x40;
				ushort obj_id;
				var segOff = bytesToSegmentOffset(cmd, 4);
				ByteSegment entry;
				int off = (int)segOff.Offset;
				var data = rom.getSegment(segOff.Segment);

				sub_cmd = bytesToUInt16(data, off);
				var num_verts = bytesToUInt16(data, off + 2);
				off += 4;
				for (var verts_end = (int)(off + ((int)num_verts * 6)); off != verts_end; off += 6)
					lvl.getCurrentArea().collision.AddVertex((OpenTK.Vector3)bytesToVector3s(data,off));

				if (sub_cmd != 0x0041) while ((sub_cmd = bytesToUInt16(data, (int)off)) != 0x0041)
				{
					lvl.getCurrentArea().collision.NewTriangleList((int)sub_cmd/*bytesToUInt16(data, (int)off)*/);
					off += 4;
					for (uint num_tri = bytesToUInt16(data, (int)off - 2), col_len = collisionLength(sub_cmd), vert_end = (uint)off + ((uint)num_tri * col_len); off < vert_end; off += (int)col_len)
						lvl.getCurrentArea().collision.AddTriangle(
							bytesToUInt16(data, off),
							bytesToUInt16(data, off + 2),
							bytesToUInt16(data, off + 4));
				}

				lvl.getCurrentArea().collision.buildCollisionMap();
				off += 2;//hm?
				for (ushort num_obj; ;)
				{
					sub_cmd = (ushort)bytesToUInt16(data, (int)off);
					switch (sub_cmd)
					{
						case 0x0042: break;
						case 0x0043:
							num_obj = (ushort)bytesToUInt16(data, (int)off + 2);
							off += 4;
							for (int i = 0; i < num_obj; i++)
							{
								obj_id = (ushort)bytesToUInt16(data, (int)off);
								entry = getSpecialObjectEntry(rom,(byte)obj_id);
								if (entry.Length == 0) throw new System.InvalidOperationException();
								data.Segment((uint)off,getSpecialObjectLength(obj_id), out objDat);
								newObj = new Object3D(lvl, objDat,Object3DCategory.SpecialObject);
								/*
								if (rom.isSegmentMIO0(segOff.Segment))
								{
									newObj.MakeReadOnly();
									newObj.Address = "N/A";
								}
								else
								{
									newObj.Address = "0x" + rom.decodeSegmentAddress(segOff.Segment, segOff.Offset).ToString("X");
								}*/
								newObj.setPresetID(obj_id);
								//newObj.level = lvl;
								newObj.HideProperty(Object3D.FLAGS.ROTATION_X);
								newObj.HideProperty(Object3D.FLAGS.ROTATION_Z);
								newObj.HideProperty(Object3D.FLAGS.BPARAM_3);
								newObj.HideProperty(Object3D.FLAGS.BPARAM_4);
								newObj.pos = bytesToVector3s(data, off + 2);
								newObj.BehaviorParameter1 = entry[1];
								newObj.BehaviorParameter2 = entry[2];
								newObj.MakeBehaviorReadOnly(true);
								newObj.MakeModelIDReadOnly(true);
								ent_behavior = bytesToUInt32(entry, 4);
								if (objDat.Length > 8)
								{
									newObj.yRot_OddMultiply = (short)bytesToInt16(data, (int)off + 8);
									if (objDat.Length > 10)
									{
										newObj.BehaviorParameter1 = data[(int)off + 10];
										newObj.BehaviorParameter2 = data[(int)off + 11];
										lvl.AddSpecialObjectPreset_12(obj_id, entry[3],
											ent_behavior, data[(int)off + 10], data[(int)off + 11]);
									}
									else
									{
										lvl.AddSpecialObjectPreset_10(obj_id, entry[3], ent_behavior);
										newObj.HideProperty(Object3D.FLAGS.BPARAM_1);
										newObj.HideProperty(Object3D.FLAGS.BPARAM_2);
									}
								}
								else
								{
									lvl.AddSpecialObjectPreset_8(obj_id, entry[3], ent_behavior);
									newObj.HideProperty(Object3D.FLAGS.BPARAM_1);
									newObj.HideProperty(Object3D.FLAGS.BPARAM_2);
									newObj.HideProperty(Object3D.FLAGS.ROTATION_Y);
								}
								newObj.ModelID = entry[3];
								newObj.setBehaviorFromAddress(ent_behavior);
								newObj.DontShowActs();
								if (ent_behavior != 0)
									lvl.getCurrentArea().SpecialObjects.Add(newObj.PreAdd());
								else if (entry[3] != 0)
									Console.WriteLine("Didn't draw:" + entry[3]);
								off += objDat.Length;
							}
							continue;
						case 0x0044:
							// Also skipping water boxes. Will come back to it later.
							{

								var num_boxes = bytesToUInt16(data, (int)off + 2);
								off += 4;
								for (int i = 0; i < num_boxes; i++)
								{
									var one = bytesToVector3s(data, off);
									var two = bytesToVector3s(data, off + 6);
									off += 0xC;
								}
							}
							continue;
						default:
							Console.WriteLine("Unchallenged obj:0x" + sub_cmd.ToString("X4"));
							continue;
					}
					break;
				}
			}
			public unsafe void CMD_39(ByteSegment cmd)
			{
				if (cmd.Length < 8)
					return;

				SegmentOffset pos = bytesToSegmentOffset(cmd, 4);

				var data = rom.getDataFromSegmentAddress_safe(pos, 10);

				lvl.getCurrentArea().MacroObjects.Clear();
				ushort iddat/*, bp*/;
				for (Object3D newObj; ;)
				{
					//rom.printArray(data, 10);
					iddat = bytesToUInt16(data);
					if ((iddat & 0x1FFu) == 0 || (iddat & 0x1FFu) == 0x1E) break;
					newObj = new Object3D(lvl, data, Object3DCategory.MacroObject);
#if NO
					// should this be data? not cmd?
					if (rom.isSegmentMIO0(pos.Segment/*cmd[4]*/))
					{
						newObj.MakeReadOnly();
						newObj.Address = "N/A";
					}
					else
					{
						newObj.Address = "0x" + rom.decodeSegmentAddress(pos).ToString("X");
					}
#endif
					uint table_off = ((iddat & 0x1FFu) - 0x1Fu)<<3;
					var entryData = ROM.getSubArray(rom.Bytes, Globals.macro_preset_table + table_off, 8);
					newObj.setBehaviorFromAddress(bytesToUInt32(entryData));
					newObj.HideProperty(Object3D.FLAGS.ROTATION_X);
					newObj.HideProperty(Object3D.FLAGS.ROTATION_Z);
					newObj.HideProperty(Object3D.FLAGS.BPARAM_3);
					newObj.HideProperty(Object3D.FLAGS.BPARAM_4);
					newObj.ModelID = entryData[5];
					newObj.setPresetID((ushort)(iddat & 0x1FF));
					newObj.yRot_OddMultiply2 = (sbyte)(((int)iddat<<16)>>(16+9));
					newObj.pos = bytesToVector3s(data, 2);
					newObj.DontShowActs();
					newObj.MakeBehaviorReadOnly(true);
					newObj.MakeModelIDReadOnly(true);
	//				bp = (ushort)bytesToUInt16(data, 8);
					if (data[8] != 0)
						newObj.BehaviorParameter1 = data[8];
					else
						newObj.BehaviorParameter1 = entryData[6];

					if (data[9] != 0)
						newObj.BehaviorParameter2 = data[9];
					else
						newObj.BehaviorParameter2 = entryData[7];

					lvl.getCurrentArea().MacroObjects.Add(newObj.PreAdd());
					pos += 10;
					data = rom.getDataFromSegmentAddress_safe(pos, 10);
				}
				//uint end = bytesToInt(cmd, 8, 4);
				//rom.setSegment(seg, start, end, false);
			}
		}
		unsafe private static ByteSegment getSpecialObjectEntry(ROM rom, byte presetID)
		{
			ByteSegment o = default(ByteSegment);
			//ROM rom = ROM.Instance;
            uint offset = Globals.special_preset_table;
            byte got = rom.Bytes[(int)offset];
            while(got != 0xFF)
            {
                if (got == presetID)
                {
					rom.Bytes.Segment(offset, 8u, out o);
					break;
                }
                offset += 8;
                got = rom.Bytes[(int)offset];
            }

			return o;
        }

        private static uint getSpecialObjectLength(int obj)
        {
            if (obj > 0x64 && obj < 0x79) return 10;
            else if (obj > 0x78 && obj < 0x7E) return 8;
            else if (obj > 0x7D && obj < 0x83) return 10;
            else if (obj > 0x88 && obj < 0x8E) return 10;
            else if (obj > 0x82 && obj < 0x8A) return 12;
            else if (obj == 0x40) return 10;
            else if (obj == 0x64) return 12;
            else if (obj == 0xC0) return 8;
            else if (obj == 0xE0) return 12;
            else if (obj == 0xCD) return 12;
            else if (obj == 0x00) return 10;
            return 8;
        }

        private static uint collisionLength(int type)
        {
            switch (type)
            {
                case 0x0E:
                case 0x24:
                case 0x25:
                case 0x27:
                case 0x2C:
                case 0x2D:
                case 0x40:
                    return 8;
                default:
                    return 6;
			}
		}

    }
}
