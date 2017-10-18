using System;
using BubblePony.Alloc;
using System.Runtime.InteropServices;

namespace Quad64.Scripts
{
    public sealed class LevelScripts : Script
	{
		private CommandInfo UnknownCommandInfo;
		private LevelScripts() : base(
			RuntimeType:typeof(Runtime), 
			DelegateType:typeof(Runtime.Command),
			PreflightRunCommand:false) {
		}

		private static readonly LevelScripts Instance = new LevelScripts();

		protected sealed override byte EvaluateRuntime(Script.Runtime Source)
		{
			return Runtime.Evaluate((Runtime)Source);
		}
		protected sealed override Script.Runtime CloneRuntime(Script.Runtime Source)
		{
			return new Runtime((Runtime)Source);
		}
		//static byte static_most_length;
		new private sealed partial class Runtime : Script.Runtime.Impl<Runtime>
		{
			public readonly Level lvl;

			public Runtime(Level level) : base(Instance, level.rom)
			{
				this.lvl = level;
			}

			public Runtime(Runtime copy) : base(copy)
			{
				this.lvl = copy.lvl;
			}

			public Runtime(Runtime copy, SegmentOffset segOff) : this(copy)
			{
				SetSegment(this, segOff);
			}			
		}
		
		protected sealed override CommandInfo GetCommandInfo(byte AA)
		{
			Runtime.Command Command;
			switch (AA)
			{
				case 0x00: Command = Runtime.CMD_00; break;
				case 0x01: Command = Runtime.CMD_01; break;
				case 0x02: Command = Runtime.CMD_02; break;
				case 0x05: Command = Runtime.CMD_05; break;
				case 0x06: Command = Runtime.CMD_06; break;
				case 0x07: Command = Runtime.CMD_07; break;
				case 0x0C: Command = Runtime.CMD_0C; break;
				case 0x17: Command = Runtime.CMD_17; break;
				case 0x18: Command = Runtime.CMD_18; break;
				case 0x1A: Command = Runtime.CMD_1A; break;
				case 0x1F: Command = Runtime.CMD_1F; break;
				case 0x21: Command = Runtime.CMD_21; break;
				case 0x22: Command = Runtime.CMD_22; break;
				case 0x24: Command = Runtime.CMD_24; break;
				case 0x26: Command = Runtime.CMD_26; break;
				case 0x27: Command = Runtime.CMD_27; break;
				case 0x28: Command = Runtime.CMD_28; break;
				case 0x2E: Command = Runtime.CMD_2E; break;
				case 0x39: Command = Runtime.CMD_39; break;
				default:
					return UnknownCommandInfo ??
						System.Threading.Interlocked.CompareExchange(
							ref UnknownCommandInfo,
							new CommandInfo(this, null, 0), null)
						?? UnknownCommandInfo;
			}
			return new CommandInfo(this, Command, 0);
		}

		public static byte Parse(
			Level lvl,
			SegmentOffset segOff)
		{
			var rt = new Runtime(lvl);
			Runtime.SetSegment(rt, segOff);
			var ret = Runtime.Evaluate(rt);
			return ret;
		}

		partial class Runtime
		{
			static public void CMD_00(Runtime rt, ref ByteSegment cmd)
			{
				SegmentOffset segOff = new SegmentOffset
				{
					Segment = cmd[3],
					Offset = bytesToUInt24(cmd, 13),
				};
				uint start = bytesToUInt32(cmd, 4);
				uint end = bytesToUInt32(cmd, 8);
				rt.rom.setSegment(segOff.Segment, start, end, false);
				if (segOff.Segment == 0x14) return;
				Evaluate(new Runtime(rt, segOff));
			}
			static public void CMD_01(Runtime rt, ref ByteSegment cmd)
			{
				CMD_00(rt, ref cmd);
			}
			static public void CMD_02(Runtime rt, ref ByteSegment cmd)
			{
				Exit(rt, 0x02);
			}
			static public void CMD_05(Runtime rt, ref ByteSegment cmd)
			{
				var segOff = bytesToSegmentOffset(cmd, 4);
				if (segOff.Segment == rt.segmentOffset.Segment &&
					(((long)segOff.Offset - (long)rt.segmentOffset.Offset) == -4)) {
					Console.Error.WriteLine("Infinite loop detected!");
					Exit(rt, 0x02);
				}
				else
				{
					var rt2 = new Runtime(rt, segOff);
					Exit(rt, Evaluate(rt2));
				}
			}

			static public void CMD_06(Runtime rt, ref ByteSegment cmd)
			{
				var rt2 = new Runtime(rt, bytesToSegmentOffset(cmd, 4));
				if (0x02 == Evaluate(rt2))
					Exit(rt, 0x02);
			}
			static public void CMD_07(Runtime rt, ref ByteSegment cmd)
			{
				Exit(rt, 0x07);
			}

			static public void CMD_0C(Runtime rt, ref ByteSegment cmd)
			{
				Runtime rt2;
				if (null != rt.lvl && bytesToInt32(cmd, 4) == rt.lvl.LevelID)
				{
					rt2 = new Runtime(rt, bytesToSegmentOffset(cmd, 8));
					Evaluate(rt2);
				}
			}

			static public void CMD_17(Runtime rt, ref ByteSegment cmd)
			{
				rt.rom.setSegment(cmd[3], bytesToUInt32(cmd, 4), bytesToUInt32(cmd, 8), false);
			}

			static public void CMD_18(Runtime rt, ref ByteSegment cmd)
			{
				byte seg = cmd[3];
				uint start = bytesToUInt32(cmd, 4);
				var MIO0_header = ROM.getSubArray(rt.rom.Bytes, start, 0x10);

				if (bytesToInt32(MIO0_header, 0) == 0x4D494F30) // Check MIO0 signature
				{
					rt.rom.setSegment(seg, start, bytesToUInt32(cmd, 8), true);
				}//else?
			}
			static public void CMD_1A(Runtime rt, ref ByteSegment cmd) {
				CMD_18(rt, ref cmd);
			}

			static public void CMD_1F(Runtime rt, ref ByteSegment cmd)
			{
				byte areaID = cmd[2];
				var segOff = bytesToSegmentOffset(cmd, 4);

				Area newArea = new Area(areaID, segOff.Value, rt.lvl);
				newArea.AreaModel.GeoDataSegAddress = segOff.Value;
				GeoScripts.Parse(newArea, segOff);
				rt.lvl.Areas.Add(newArea);
				rt.lvl.CurrentAreaID = areaID;
				newArea.AreaModel.buildBuffers();
				// newArea.AreaModel.outputTextureAtlasToPng("Area_"+areaID+"_TexAtlus.png");
			}

			static public void CMD_21(Runtime rt, ref ByteSegment cmd)
			{
				byte modelID = cmd[3];
				var segOff = bytesToSegmentOffset(cmd, 4);

				Model3D newModel = new Model3D();
				newModel.GeoDataSegAddress = segOff.Value;
				rt.lvl.AddObjectCombos(modelID, newModel.GeoDataSegAddress);

				if (rt.rom.hasSegment(segOff.Segment))
					Fast3DScripts.Parse(
						newModel,
						rt.lvl, 
						segOff,
						(byte)(cmd[2]>>4)
						);

				//if (lvl.ModelIDs.ContainsKey(modelID))
				//	lvl.ModelIDs.Remove(modelID);
				rt.lvl.ModelIDs.Remove(modelID);
				newModel.buildBuffers();
				rt.lvl.ModelIDs.Add(modelID, newModel);
			}
			static public void CMD_22(Runtime rt, ref ByteSegment cmd)
			{
				byte modelID = cmd[3];
				var segOff = bytesToSegmentOffset(cmd, 4);
				//Console.WriteLine("Size of seg 0x"+seg.ToString("X2")+" = " + rom.getSegment(seg).Length);
				Model3D newModel = new Model3D();
				newModel.GeoDataSegAddress = segOff.Value;
				rt.lvl.AddObjectCombos(modelID, newModel.GeoDataSegAddress);
				if (rt.rom.hasSegment(segOff.Segment))
					GeoScripts.Parse(newModel, rt.lvl, segOff);
				//if (lvl.ModelIDs.ContainsKey(modelID))
				//	lvl.ModelIDs.Remove(modelID);
				rt.lvl.ModelIDs.Remove(modelID);
				newModel.buildBuffers();
				rt.lvl.ModelIDs.Add(modelID, newModel);
			}

			static public void CMD_24(Runtime rt, ref ByteSegment cmd)
			{
				Object3D newObj = new Object3D(rt.lvl, cmd, Object3DCategory.Object);
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
				rt.lvl.getCurrentArea().Objects.Add(newObj.PreAdd());
			}


			static public void CMD_26(Runtime rt, ref ByteSegment cmd)
			{
				Warp warp = new Warp(rt.lvl,cmd, false);
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
				rt.lvl.getCurrentArea().Warps.Add(warp);
			}

			static public void CMD_27(Runtime rt, ref ByteSegment cmd)
			{
				Warp warp = new Warp(rt.lvl,cmd, true);
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
				rt.lvl.getCurrentArea().PaintingWarps.Add(warp);
			}
			static public void CMD_28(Runtime rt, ref ByteSegment cmd)
			{
				WarpInstant warp = new WarpInstant(rt.lvl,cmd);
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
				rt.lvl.getCurrentArea().InstantWarps.Add(warp);
			}

			/* Process collision map, Special Objects, and <S> water boxes </S>. */
			static public void CMD_2E(Runtime rt, ref ByteSegment cmd)
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
				var data = rt.rom.getSegment(segOff.Segment);

				sub_cmd = bytesToUInt16(data, off);
				var num_verts = bytesToUInt16(data, off + 2);
				off += 4;
				for (var verts_end = (int)(off + ((int)num_verts * 6)); off != verts_end; off += 6)
					rt.lvl.getCurrentArea().collision.AddVertex((OpenTK.Vector3)bytesToVector3s(data,off));

				if (sub_cmd != 0x0041) while ((sub_cmd = bytesToUInt16(data, (int)off)) != 0x0041)
				{
					rt.lvl.getCurrentArea().collision.NewTriangleList((int)sub_cmd/*bytesToUInt16(data, (int)off)*/);
					off += 4;
					for (uint num_tri = bytesToUInt16(data, (int)off - 2), col_len = collisionLength(sub_cmd), vert_end = (uint)off + ((uint)num_tri * col_len); off < vert_end; off += (int)col_len)
						rt.lvl.getCurrentArea().collision.AddTriangle(
							bytesToUInt16(data, off),
							bytesToUInt16(data, off + 2),
							bytesToUInt16(data, off + 4));
				}

				rt.lvl.getCurrentArea().collision.buildCollisionMap();
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
								entry = getSpecialObjectEntry(rt.rom,(byte)obj_id);
								if (entry.Length == 0) throw new System.InvalidOperationException();
								data.Segment((uint)off,getSpecialObjectLength(obj_id), out objDat);
								newObj = new Object3D(rt.lvl, objDat,Object3DCategory.SpecialObject);
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
										rt.lvl.AddSpecialObjectPreset_12(obj_id, entry[3],
											ent_behavior, data[(int)off + 10], data[(int)off + 11]);
									}
									else
									{
										rt.lvl.AddSpecialObjectPreset_10(obj_id, entry[3], ent_behavior);
										newObj.HideProperty(Object3D.FLAGS.BPARAM_1);
										newObj.HideProperty(Object3D.FLAGS.BPARAM_2);
									}
								}
								else
								{
									rt.lvl.AddSpecialObjectPreset_8(obj_id, entry[3], ent_behavior);
									newObj.HideProperty(Object3D.FLAGS.BPARAM_1);
									newObj.HideProperty(Object3D.FLAGS.BPARAM_2);
									newObj.HideProperty(Object3D.FLAGS.ROTATION_Y);
								}
								newObj.ModelID = entry[3];
								newObj.setBehaviorFromAddress(ent_behavior);
								newObj.DontShowActs();
								if (ent_behavior != 0)
									rt.lvl.getCurrentArea().SpecialObjects.Add(newObj.PreAdd());
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
			static public unsafe void CMD_39(Runtime rt, ref ByteSegment cmd)
			{
				if (cmd.Length < 8)
					return;

				SegmentOffset pos = bytesToSegmentOffset(cmd, 4);

				var data = rt.rom.getDataFromSegmentAddress_safe(pos, 10);

				rt.lvl.getCurrentArea().MacroObjects.Clear();
				ushort iddat/*, bp*/;
				for (Object3D newObj; ;)
				{
					//rom.printArray(data, 10);
					iddat = bytesToUInt16(data);
					if ((iddat & 0x1FFu) == 0 || (iddat & 0x1FFu) == 0x1E) break;
					newObj = new Object3D(rt.lvl, data, Object3DCategory.MacroObject);
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
					var entryData = ROM.getSubArray(rt.rom.Bytes, Globals.macro_preset_table + table_off, 8);
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

					rt.lvl.getCurrentArea().MacroObjects.Add(newObj.PreAdd());
					pos += 10;
					data = rt.rom.getDataFromSegmentAddress_safe(pos, 10);
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
