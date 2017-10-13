using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quad64.src
{
    class AssemblyReader
    {
        public void findJALsInFunction(uint RAMFunc, uint RAMtoROM, ref StructList<JAL_CALL> calls)
        {
			calls.Clear();
			StructList<Instruction> inst_list= new StructList<Instruction>();
			Instruction inst=default(Instruction);

			ReadFunction(RAMFunc, RAMtoROM, ref inst_list);
			JAL_CALL jal=default(JAL_CALL);

			bool addNextTime = false;

			while(inst_list.Dequeue(ref inst))
            {
                bool addJAL = addNextTime;
                switch (inst.opCode)
                {
                    case OPCODE.LUI:
                        if (inst.gp_dest == GP_REGISTER.A0)
							jal.a0 = (uint)(inst.immediate << 16);
                        else if (inst.gp_dest == GP_REGISTER.A1)
							jal.a1 = (uint)(inst.immediate << 16);
                        else if (inst.gp_dest == GP_REGISTER.A2)
							jal.a2 = (uint)(inst.immediate << 16);
                        else if (inst.gp_dest == GP_REGISTER.A3)
							jal.a3 = (uint)(inst.immediate << 16);
                        break;
                    case OPCODE.ADDIU:
                        if (inst.gp_dest == GP_REGISTER.A0 && inst.gp_1 == GP_REGISTER.A0)
							jal.a0 += (uint)inst.immediate;
                        else if (inst.gp_dest == GP_REGISTER.A1 && inst.gp_1 == GP_REGISTER.A1)
							jal.a1 += (uint)inst.immediate;
                        else if (inst.gp_dest == GP_REGISTER.A2 && inst.gp_1 == GP_REGISTER.A2)
							jal.a2 += (uint)inst.immediate;
                        else if (inst.gp_dest == GP_REGISTER.A3 && inst.gp_1 == GP_REGISTER.A3)
							jal.a3 += (uint)inst.immediate;
                        else if (inst.gp_dest == GP_REGISTER.A0 && inst.gp_1 == GP_REGISTER.R0)
							jal.a0 = (uint)inst.immediate;
                        else if (inst.gp_dest == GP_REGISTER.A1 && inst.gp_1 == GP_REGISTER.R0)
							jal.a1 = (uint)inst.immediate;
                        else if (inst.gp_dest == GP_REGISTER.A2 && inst.gp_1 == GP_REGISTER.R0)
							jal.a2 = (uint)inst.immediate;
                        else if (inst.gp_dest == GP_REGISTER.A3 && inst.gp_1 == GP_REGISTER.R0)
							jal.a3 = (uint)inst.immediate;
                        break;
                    case OPCODE.ORI:
                        if (inst.gp_dest == GP_REGISTER.A0 && inst.gp_1 == GP_REGISTER.A0)
							jal.a0 |= (ushort)inst.immediate;
                        else if (inst.gp_dest == GP_REGISTER.A1 && inst.gp_1 == GP_REGISTER.A1)
							jal.a1 |= (ushort)inst.immediate;
                        else if (inst.gp_dest == GP_REGISTER.A2 && inst.gp_1 == GP_REGISTER.A2)
							jal.a2 |= (ushort)inst.immediate;
                        else if (inst.gp_dest == GP_REGISTER.A3 && inst.gp_1 == GP_REGISTER.A3)
							jal.a3 |= (ushort)inst.immediate;
                        else if (inst.gp_dest == GP_REGISTER.A0 && inst.gp_1 == GP_REGISTER.R0)
							jal.a0 = (uint)inst.immediate;
                        else if (inst.gp_dest == GP_REGISTER.A1 && inst.gp_1 == GP_REGISTER.R0)
							jal.a1 = (uint)inst.immediate;
                        else if (inst.gp_dest == GP_REGISTER.A2 && inst.gp_1 == GP_REGISTER.R0)
							jal.a2 = (uint)inst.immediate;
                        else if (inst.gp_dest == GP_REGISTER.A3 && inst.gp_1 == GP_REGISTER.R0)
							jal.a3 = (uint)inst.immediate;
                        break;
                    case OPCODE.JAL:
						jal.JAL_ADDRESS = inst.jump_to_func;
                        addNextTime = true;
                        break;
                }

                if (addJAL)
                {
                    calls.Add(ref jal);
                    //Console.WriteLine(newCall.ToString());
                    addNextTime = false;
                }
            }
        }

        public void ReadFunction(uint RAMAddr, uint RAMtoROM, ref StructList<Instruction> instructions)
        {
			instructions.Clear();

			Instruction cur;

			ROM rom = ROM.Instance;
            uint ROM_OFFSET = RAMAddr - RAMtoROM;
            uint offset = ROM_OFFSET;
            int end = 0x8000;
            while (end > 0)
            {
                cur = parseInstruction(rom.readWordUnsigned(offset));
                offset += 4;
                end--;
                if (cur.opCode == OPCODE.JR && cur.gp_1 == GP_REGISTER.RA)
                    end = 1;
                instructions.Add(ref cur);
            }
        }

        private Instruction parseInstruction(uint data)
        {
            Instruction inst = new Instruction();
            if (data == 0)
            {
                inst.opCode = OPCODE.NOP;
                return inst;
            }

            uint opCode = data >> 26, func = 0;
            switch (opCode)
            {
                case 0x00:
                    func = data & 0x3F;
                    switch (func)
                    {
                        case 0x08:
                            inst.opCode = OPCODE.JR;
                            inst.gp_1 = (GP_REGISTER)((data >> 21) & 0x1F);
                            break;
                        default:
                            inst.opCode = OPCODE.DO_NOT_CARE;
                            break;
                    }
                    break;
                case 0x03:
                    inst.opCode = OPCODE.JAL;
                    inst.jump_to_func = 0x80000000 + ((data & 0x3FFFFFF) << 2);
                    break;
                case 0x09:
                    inst.opCode = OPCODE.ADDIU;
                    inst.gp_dest = (GP_REGISTER)((data >> 16) & 0x1F);
                    inst.gp_1 = (GP_REGISTER)((data >> 21) & 0x1F);
                    inst.immediate = (short)(data & 0xFFFF);
                    break;
                case 0x0D:
                    inst.opCode = OPCODE.ORI;
                    inst.gp_dest = (GP_REGISTER)((data >> 16) & 0x1F);
                    inst.gp_1 = (GP_REGISTER)((data >> 21) & 0x1F);
                    inst.immediate = (short)(data & 0xFFFF);
                    break;
                case 0x0F:
                    inst.opCode = OPCODE.LUI;
                    inst.gp_dest = (GP_REGISTER)((data >> 16) & 0x1F);
                    inst.immediate = (short)(data & 0xFFFF);
                    break;
                default:
                    inst.opCode = OPCODE.DO_NOT_CARE;
                    break;
            }

            return inst;
        }
        

        // General-Purpose Registers
        public enum GP_REGISTER
        {
            R0, // Constant 0
            AT, // Used for psuedo-instructions
            V0, V1, // Function returns
            A0, A1, A2, A3, // Function Arguments
            T0, T1, T2, T3, T4, T5, T6, T7, // Temporary
            S0, S1, S2, S3, S4, S5, S6, S7, // Saved
            T8, T9, // More temporary
            K0, K1, // Reserved for Kernal (do not use)
            GP, // Global area pointer
            SP, // Stack pointer
            FP, // Frame pointer
            RA // Return address
        }

        // Floating-Point Registers
        public enum FP_REGISTER
        {
            F0, F1, F2, F3, // Function returns
            F4, F5, F6, F7, F8, F9, F10, F11, // Temporary
            F12, F13, F14, F15, // Function arguments
            F16, F17, F18, F19, // More Temporary
            F20, F21, F22, F23, F24, F25, F26, F27, F28, F29, F30, F31 // Saved
        }

        public enum OPCODE
        {
            LUI,
            ADDIU,
            ORI,
            JAL,
            JR,
            NOP,
            DO_NOT_CARE
        }
		internal static class JAL_CALL_static { public static readonly StructDump<JAL_CALL> Dump = new StructDump<JAL_CALL>(); }
		public struct JAL_CALL : StructDumpable<JAL_CALL>, IEquatable<JAL_CALL>
		{
			public uint JAL_ADDRESS, a0, a1, a2, a3;
			public static bool Equals(ref JAL_CALL L, ref JAL_CALL R)
			{
				return L.JAL_ADDRESS == R.JAL_ADDRESS && L.a0 == R.a0 && L.a1 == R.a1 && L.a2 == R.a2 && L.a3 == R.a3;
			}
			public static bool Inequals(ref JAL_CALL L, ref JAL_CALL R)
			{
				return L.JAL_ADDRESS != R.JAL_ADDRESS || L.a0 != R.a0 || L.a1 != R.a1 || L.a2 != R.a2 || L.a3 != R.a3;
			}
			public static bool operator ==(JAL_CALL @this, JAL_CALL other) { return Equals(ref @this, ref other); }
			public static bool operator !=(JAL_CALL @this, JAL_CALL other) { return Inequals(ref @this, ref other); }
			public bool Equals(JAL_CALL other) { return Equals(ref this, ref other); }
			public override int GetHashCode() { return unchecked((int)(JAL_ADDRESS ^ a0 ^ ((a1 << 8) | (a1 >> 24)) ^ ((a2 << 16) | (a2 >> 16)) ^ ((a3 << 24) | (a3 >> 8)))); }
			public override bool Equals(object obj) { return obj is JAL_CALL && Equals((JAL_CALL)obj); }
			StructDump<JAL_CALL> StructDumpable<JAL_CALL>.GetDump() { return JAL_CALL_static.Dump; }
            public override string ToString()
            {
                return JAL_ADDRESS.ToString("X8") + "(" 
                    + a0.ToString("X8") + ", "
                    + a1.ToString("X8") + ", "
                    + a2.ToString("X8") + ", "
                    + a3.ToString("X8") + ")";
            }
        }

		internal static class Instruction_static
		{
			public static readonly StructDump<Instruction> Dump = new StructDump<Instruction>();
		}
        public struct Instruction : StructDumpable<Instruction>, IEquatable<Instruction>
        {
            public OPCODE opCode;
            public GP_REGISTER gp_1, /*gp_2,*/ gp_dest;
            /*public FP_REGISTER fp_1, fp_2, fP_dest;*/
            public short immediate;
            public uint jump_to_func;
			public static bool Equals(ref Instruction L, ref Instruction R)
			{
				return L.opCode == R.opCode &&
                    (L.opCode== OPCODE.JR?L.gp_1==R.gp_1
					:L.opCode== OPCODE.LUI?L.gp_dest==R.gp_dest&&L.immediate==R.immediate
					:L.opCode== OPCODE.ADDIU?L.gp_dest==R.gp_dest&&L.gp_1==R.gp_1&&L.immediate==R.immediate
					:L.opCode==OPCODE.JAL?L.jump_to_func==R.jump_to_func
					:(L.gp_1==R.gp_1&&L.gp_dest==R.gp_dest&&L.immediate==R.immediate&&L.jump_to_func==R.jump_to_func)
					);

			}
			public static bool Inequals(ref Instruction L, ref Instruction R)
			{
				return !Equals(ref L, ref R);
			}
			public static bool operator ==(Instruction @this, Instruction other) { return Equals(ref @this, ref other); }
			public static bool operator !=(Instruction @this, Instruction other) { return Inequals(ref @this, ref other); }
			public bool Equals(Instruction other) { return Equals(ref this, ref other); }
			public override int GetHashCode()
			{

				return opCode == OPCODE.JR ? 1|((int)gp_1<<2)
					: opCode == OPCODE.LUI ? 0|((int)gp_dest<<2)|(int)immediate<<10
					: opCode == OPCODE.ADDIU ? 3|(((int)gp_dest<<2)|((int)gp_1<<10))^((int)immediate<<16)
					: opCode == OPCODE.JAL ? 2|(((int)jump_to_func*5)<<2)
					: (((int)gp_1*3)<<2 | (((int)gp_dest*5)<<5) | ((int)jump_to_func<<16)|(int)(immediate&((1<<16)-1)));
			}

			public override bool Equals(object obj) { return obj is Instruction && Equals((Instruction)obj); }

			public override string ToString()
            {
                switch (opCode)
                {
                    case OPCODE.JR:
                        return "JR " + gp_1.ToString();
                    case OPCODE.LUI:
                        return "LUI " + gp_dest.ToString() + " 0x" + immediate.ToString("X4");
                    case OPCODE.ADDIU:
                        return "ADDIU " + gp_dest.ToString() + " " + gp_1.ToString() + " 0x" + immediate.ToString("X4");
                    case OPCODE.JAL:
                        return "JAL 0x" + jump_to_func.ToString("X8");
                }
                return opCode.ToString();
            }

			StructDump<Instruction> StructDumpable<Instruction>.GetDump() { return Instruction_static.Dump; }
		}
    }

	public interface StructDumpable<Struct>
		where Struct : struct, StructDumpable<Struct>, IEquatable<Struct>
	{
		StructDump<Struct> GetDump();
	}

	public sealed class StructDump<Struct>
		where Struct : struct, StructDumpable<Struct>, System.IEquatable<Struct>
	{
		public StructNode<Struct> Top;
		public int Count;
		public static StructDump<Struct> Singleton => default(Struct).GetDump();
	}

	public sealed class StructNode<Struct> : System.IEquatable<Struct>
		where Struct : struct, StructDumpable<Struct>, System.IEquatable<Struct>
	{
		public StructNode<Struct> Next;
		public Struct Value;
		public bool Equals(Struct value) { return value.Equals(Value); }
	}

	public struct StructList<Struct>
		where Struct : struct, StructDumpable<Struct>, System.IEquatable<Struct>
	{
		public StructNode<Struct> First, Last;
		public int Count;
		// whenever retain is not zero, clear will fail and decrement.
		// by default, most functions that take struct lists by reference.
		// this is because they immediately call clear (usually) on the list
		// most functions don't intend to concatenate these lists. but by incrementing this before passing by ref you force it to.
		public uint Retain;

		public void Add(ref Struct value)
		{
			var Node = new StructNode<Struct> { Value = value, };

			if (0 == Count++)
			{
				First = Node;
			}
			else
			{
				Last.Next = Node;
			}
			Node.Next = First;
			Last = Node;
		}

		public bool Dequeue(ref Struct read)
		{
			if (Count == 0) return false;
			var Node = First;
			if (0 == --Count)
			{
				this.First = null;
				this.Last = null;
			}
			else
			{
				First = Node.Next;
				Last.Next = First;
			}

			read = Node.Value;

			var Dump = read.GetDump();

			if (0 == Dump.Count++)
			{
				Node.Next = null;
			}
			else
			{
				Node.Next = Dump.Top;
			}
			Dump.Top = Node;
			return true;
		}
		public void Clear()
		{
			if (Retain != 0)
				--Retain;
			else if (Count != 0)
			{
				var NodeFirst = First;
				var NodeLast = Last;
				var NodeCount = Count;
				this = default(StructList<Struct>);

				var Dump = StructDump<Struct>.Singleton;

				if (0 == Dump.Count)
				{
					NodeLast.Next = null;
					Dump.Count = NodeCount;
				}
				else
				{
					NodeLast.Next = Dump.Top;
					Dump.Count += NodeCount;
				}
				Dump.Top = NodeFirst;
			}
		}
	}
}
