using System.Runtime.InteropServices;
using OpenTK;
namespace Quad64 {
[StructLayout(LayoutKind.Explicit)]
public partial struct Row4F : System.IEquatable<Row4F> {
	[FieldOffset(0x00)] public ushort X;
	[FieldOffset(0x02)] public ushort Y;
	[FieldOffset(0x04)] public ushort Z;
	[FieldOffset(0x06)] public ushort W;
	[FieldOffset(0x00)] public ulong Value;
	public static bool Equals(ref Row4F L, ref Row4F R){
		return L.Value==R.Value;
	}
	public static bool Inequals(ref Row4F L, ref Row4F R){
		return L.Value!=R.Value;
	}
	public static bool operator == (Row4F L, Row4F R){
		return L.Value==R.Value;
	}
	public static bool operator != (Row4F L, Row4F R){
		return L.Value!=R.Value;
	}
	public static int GetHashCode(ref Row4F Value){
		return
			Value.Value.GetHashCode();
	}
	public bool Equals(Row4F other){return Equals(ref this, ref other);}
	public bool Equals(ref Row4F other){return Equals(ref this, ref other);}
	public override bool Equals(object obj){return obj is Row4F && ((Row4F)obj).Equals(ref this);}
	public override int GetHashCode(){return GetHashCode(ref this);}
	public void Get(int Index, out ushort Value){
		Value=(0==(2&Index))?(0==(1&Index))?this.X:this.Y:(0==(1&Index))?this.Z:this.W;
	}
	public void Set(int Index, ushort Value){
		if(0==(2&Index))if(0==(1&Index))this.X=Value; else this.Y=Value; else if(0==(1&Index))this.Z=Value; else this.W=Value;;
	}
	public void Set(int Index, [In] ref ushort Value){
		if(0==(2&Index))if(0==(1&Index))this.X=Value; else this.Y=Value; else if(0==(1&Index))this.Z=Value; else this.W=Value;;
	}
}
[StructLayout(LayoutKind.Explicit)]
public partial struct Row4I : System.IEquatable<Row4I> {
	[FieldOffset(0x00)] public short X;
	[FieldOffset(0x02)] public short Y;
	[FieldOffset(0x04)] public short Z;
	[FieldOffset(0x06)] public short W;
	[FieldOffset(0x00)] public ulong Value;
	public static bool Equals(ref Row4I L, ref Row4I R){
		return L.Value==R.Value;
	}
	public static bool Inequals(ref Row4I L, ref Row4I R){
		return L.Value!=R.Value;
	}
	public static bool operator == (Row4I L, Row4I R){
		return L.Value==R.Value;
	}
	public static bool operator != (Row4I L, Row4I R){
		return L.Value!=R.Value;
	}
	public static int GetHashCode(ref Row4I Value){
		return
			Value.Value.GetHashCode();
	}
	public bool Equals(Row4I other){return Equals(ref this, ref other);}
	public bool Equals(ref Row4I other){return Equals(ref this, ref other);}
	public override bool Equals(object obj){return obj is Row4I && ((Row4I)obj).Equals(ref this);}
	public override int GetHashCode(){return GetHashCode(ref this);}
	public void Get(int Index, out short Value){
		Value=(0==(2&Index))?(0==(1&Index))?this.X:this.Y:(0==(1&Index))?this.Z:this.W;
	}
	public void Set(int Index, short Value){
		if(0==(2&Index))if(0==(1&Index))this.X=Value; else this.Y=Value; else if(0==(1&Index))this.Z=Value; else this.W=Value;;
	}
	public void Set(int Index, [In] ref short Value){
		if(0==(2&Index))if(0==(1&Index))this.X=Value; else this.Y=Value; else if(0==(1&Index))this.Z=Value; else this.W=Value;;
	}
}
[StructLayout(LayoutKind.Explicit)]
public partial struct MtxF : System.IEquatable<MtxF> {
	[FieldOffset(0x00)] public Row4F X;
	[FieldOffset(0x08)] public Row4F Y;
	[FieldOffset(0x10)] public Row4F Z;
	[FieldOffset(0x18)] public Row4F W;
	[FieldOffset(0x00)] public ushort XX;
	[FieldOffset(0x02)] public ushort XY;
	[FieldOffset(0x04)] public ushort XZ;
	[FieldOffset(0x06)] public ushort XW;
	[FieldOffset(0x00)] public ulong XV;
	[FieldOffset(0x08)] public ushort YX;
	[FieldOffset(0x0A)] public ushort YY;
	[FieldOffset(0x0C)] public ushort YZ;
	[FieldOffset(0x0E)] public ushort YW;
	[FieldOffset(0x08)] public ulong YV;
	[FieldOffset(0x10)] public ushort ZX;
	[FieldOffset(0x12)] public ushort ZY;
	[FieldOffset(0x14)] public ushort ZZ;
	[FieldOffset(0x16)] public ushort ZW;
	[FieldOffset(0x10)] public ulong ZV;
	[FieldOffset(0x18)] public ushort WX;
	[FieldOffset(0x1A)] public ushort WY;
	[FieldOffset(0x1C)] public ushort WZ;
	[FieldOffset(0x1E)] public ushort WW;
	[FieldOffset(0x18)] public ulong WV;
	[FieldOffset(0x00)] public uint XX_XY;
	[FieldOffset(0x04)] public uint XZ_XW;
	[FieldOffset(0x08)] public uint YX_YY;
	[FieldOffset(0x0C)] public uint YZ_YW;
	[FieldOffset(0x10)] public uint ZX_ZY;
	[FieldOffset(0x14)] public uint ZZ_ZW;
	[FieldOffset(0x18)] public uint WX_WY;
	[FieldOffset(0x1C)] public uint WZ_WW;
	public static bool Equals(ref MtxF L, ref MtxF R){
		return L.XV==R.XV &&
			L.YV==R.YV &&
			L.ZV==R.ZV &&
			L.WV==R.WV;
	}
	public static bool Inequals(ref MtxF L, ref MtxF R){
		return L.XV!=R.XV ||
			L.YV!=R.YV ||
			L.ZV!=R.ZV ||
			L.WV!=R.WV;
	}
	public static bool operator == (MtxF L, MtxF R){
		return L.XV==R.XV &&
			L.YV==R.YV &&
			L.ZV==R.ZV &&
			L.WV==R.WV;
	}
	public static bool operator != (MtxF L, MtxF R){
		return L.XV!=R.XV ||
			L.YV!=R.YV ||
			L.ZV!=R.ZV ||
			L.WV!=R.WV;
	}
	public static int GetHashCode(ref MtxF Value){
		uint Temp;
		unchecked {
		return
			Value.XV.GetHashCode() ^
			(int)(((((Temp=(uint)Value.YV.GetHashCode()))<<5))|(Temp>>27)) ^
			(int)(((((Temp=(uint)Value.ZV.GetHashCode()))<<10))|(Temp>>22)) ^
			(int)(((((Temp=(uint)Value.WV.GetHashCode()))<<15))|(Temp>>17));
		}
	}
	public bool Equals(MtxF other){return Equals(ref this, ref other);}
	public bool Equals(ref MtxF other){return Equals(ref this, ref other);}
	public override bool Equals(object obj){return obj is MtxF && ((MtxF)obj).Equals(ref this);}
	public override int GetHashCode(){return GetHashCode(ref this);}
	public void Get(int Index, out ushort Value){
		Value=(0==(8&Index))?(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.XX:this.XY:(0==(1&Index))?this.XZ:this.XW:(0==(2&Index))?(0==(1&Index))?this.YX:this.YY:(0==(1&Index))?this.YZ:this.YW:(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.ZX:this.ZY:(0==(1&Index))?this.ZZ:this.ZW:(0==(2&Index))?(0==(1&Index))?this.WX:this.WY:(0==(1&Index))?this.WZ:this.WW;
	}
	public void Set(int Index, ushort Value){
		if(0==(8&Index))if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.XX=Value; else this.XY=Value; else if(0==(1&Index))this.XZ=Value; else this.XW=Value; else if(0==(2&Index))if(0==(1&Index))this.YX=Value; else this.YY=Value; else if(0==(1&Index))this.YZ=Value; else this.YW=Value; else if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.ZX=Value; else this.ZY=Value; else if(0==(1&Index))this.ZZ=Value; else this.ZW=Value; else if(0==(2&Index))if(0==(1&Index))this.WX=Value; else this.WY=Value; else if(0==(1&Index))this.WZ=Value; else this.WW=Value;;
	}
	public void Set(int Index, [In] ref ushort Value){
		if(0==(8&Index))if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.XX=Value; else this.XY=Value; else if(0==(1&Index))this.XZ=Value; else this.XW=Value; else if(0==(2&Index))if(0==(1&Index))this.YX=Value; else this.YY=Value; else if(0==(1&Index))this.YZ=Value; else this.YW=Value; else if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.ZX=Value; else this.ZY=Value; else if(0==(1&Index))this.ZZ=Value; else this.ZW=Value; else if(0==(2&Index))if(0==(1&Index))this.WX=Value; else this.WY=Value; else if(0==(1&Index))this.WZ=Value; else this.WW=Value;;
	}
	public void Get(int Index, out uint Value){
		Value=(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.XX_XY:this.XZ_XW:(0==(1&Index))?this.YX_YY:this.YZ_YW:(0==(2&Index))?(0==(1&Index))?this.ZX_ZY:this.ZZ_ZW:(0==(1&Index))?this.WX_WY:this.WZ_WW;
	}
	public void Set(int Index, uint Value){
		if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.XX_XY=Value; else this.XZ_XW=Value; else if(0==(1&Index))this.YX_YY=Value; else this.YZ_YW=Value; else if(0==(2&Index))if(0==(1&Index))this.ZX_ZY=Value; else this.ZZ_ZW=Value; else if(0==(1&Index))this.WX_WY=Value; else this.WZ_WW=Value;;
	}
	public void Set(int Index, [In] ref uint Value){
		if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.XX_XY=Value; else this.XZ_XW=Value; else if(0==(1&Index))this.YX_YY=Value; else this.YZ_YW=Value; else if(0==(2&Index))if(0==(1&Index))this.ZX_ZY=Value; else this.ZZ_ZW=Value; else if(0==(1&Index))this.WX_WY=Value; else this.WZ_WW=Value;;
	}
	public void Get(int Index, out ulong Value){
		Value=(0==(2&Index))?(0==(1&Index))?this.XV:this.YV:(0==(1&Index))?this.ZV:this.WV;
	}
	public void Set(int Index, ulong Value){
		if(0==(2&Index))if(0==(1&Index))this.XV=Value; else this.YV=Value; else if(0==(1&Index))this.ZV=Value; else this.WV=Value;;
	}
	public void Set(int Index, [In] ref ulong Value){
		if(0==(2&Index))if(0==(1&Index))this.XV=Value; else this.YV=Value; else if(0==(1&Index))this.ZV=Value; else this.WV=Value;;
	}
	public void Get(int Index, out Row4F Value){
		Value=(0==(2&Index))?(0==(1&Index))?this.X:this.Y:(0==(1&Index))?this.Z:this.W;
	}
	public void Set(int Index, Row4F Value){
		if(0==(2&Index))if(0==(1&Index))this.X=Value; else this.Y=Value; else if(0==(1&Index))this.Z=Value; else this.W=Value;;
	}
	public void Set(int Index, [In] ref Row4F Value){
		if(0==(2&Index))if(0==(1&Index))this.X=Value; else this.Y=Value; else if(0==(1&Index))this.Z=Value; else this.W=Value;;
	}
}
[StructLayout(LayoutKind.Explicit)]
public partial struct MtxI : System.IEquatable<MtxI> {
	[FieldOffset(0x00)] public Row4I X;
	[FieldOffset(0x08)] public Row4I Y;
	[FieldOffset(0x10)] public Row4I Z;
	[FieldOffset(0x18)] public Row4I W;
	[FieldOffset(0x00)] public short XX;
	[FieldOffset(0x02)] public short XY;
	[FieldOffset(0x04)] public short XZ;
	[FieldOffset(0x06)] public short XW;
	[FieldOffset(0x00)] public ulong XV;
	[FieldOffset(0x08)] public short YX;
	[FieldOffset(0x0A)] public short YY;
	[FieldOffset(0x0C)] public short YZ;
	[FieldOffset(0x0E)] public short YW;
	[FieldOffset(0x08)] public ulong YV;
	[FieldOffset(0x10)] public short ZX;
	[FieldOffset(0x12)] public short ZY;
	[FieldOffset(0x14)] public short ZZ;
	[FieldOffset(0x16)] public short ZW;
	[FieldOffset(0x10)] public ulong ZV;
	[FieldOffset(0x18)] public short WX;
	[FieldOffset(0x1A)] public short WY;
	[FieldOffset(0x1C)] public short WZ;
	[FieldOffset(0x1E)] public short WW;
	[FieldOffset(0x18)] public ulong WV;
	[FieldOffset(0x00)] public uint XX_XY;
	[FieldOffset(0x04)] public uint XZ_XW;
	[FieldOffset(0x08)] public uint YX_YY;
	[FieldOffset(0x0C)] public uint YZ_YW;
	[FieldOffset(0x10)] public uint ZX_ZY;
	[FieldOffset(0x14)] public uint ZZ_ZW;
	[FieldOffset(0x18)] public uint WX_WY;
	[FieldOffset(0x1C)] public uint WZ_WW;
	public static bool Equals(ref MtxI L, ref MtxI R){
		return L.XV==R.XV &&
			L.YV==R.YV &&
			L.ZV==R.ZV &&
			L.WV==R.WV;
	}
	public static bool Inequals(ref MtxI L, ref MtxI R){
		return L.XV!=R.XV ||
			L.YV!=R.YV ||
			L.ZV!=R.ZV ||
			L.WV!=R.WV;
	}
	public static bool operator == (MtxI L, MtxI R){
		return L.XV==R.XV &&
			L.YV==R.YV &&
			L.ZV==R.ZV &&
			L.WV==R.WV;
	}
	public static bool operator != (MtxI L, MtxI R){
		return L.XV!=R.XV ||
			L.YV!=R.YV ||
			L.ZV!=R.ZV ||
			L.WV!=R.WV;
	}
	public static int GetHashCode(ref MtxI Value){
		uint Temp;
		unchecked {
		return
			Value.XV.GetHashCode() ^
			(int)(((((Temp=(uint)Value.YV.GetHashCode()))<<5))|(Temp>>27)) ^
			(int)(((((Temp=(uint)Value.ZV.GetHashCode()))<<10))|(Temp>>22)) ^
			(int)(((((Temp=(uint)Value.WV.GetHashCode()))<<15))|(Temp>>17));
		}
	}
	public bool Equals(MtxI other){return Equals(ref this, ref other);}
	public bool Equals(ref MtxI other){return Equals(ref this, ref other);}
	public override bool Equals(object obj){return obj is MtxI && ((MtxI)obj).Equals(ref this);}
	public override int GetHashCode(){return GetHashCode(ref this);}
	public void Get(int Index, out short Value){
		Value=(0==(8&Index))?(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.XX:this.XY:(0==(1&Index))?this.XZ:this.XW:(0==(2&Index))?(0==(1&Index))?this.YX:this.YY:(0==(1&Index))?this.YZ:this.YW:(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.ZX:this.ZY:(0==(1&Index))?this.ZZ:this.ZW:(0==(2&Index))?(0==(1&Index))?this.WX:this.WY:(0==(1&Index))?this.WZ:this.WW;
	}
	public void Set(int Index, short Value){
		if(0==(8&Index))if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.XX=Value; else this.XY=Value; else if(0==(1&Index))this.XZ=Value; else this.XW=Value; else if(0==(2&Index))if(0==(1&Index))this.YX=Value; else this.YY=Value; else if(0==(1&Index))this.YZ=Value; else this.YW=Value; else if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.ZX=Value; else this.ZY=Value; else if(0==(1&Index))this.ZZ=Value; else this.ZW=Value; else if(0==(2&Index))if(0==(1&Index))this.WX=Value; else this.WY=Value; else if(0==(1&Index))this.WZ=Value; else this.WW=Value;;
	}
	public void Set(int Index, [In] ref short Value){
		if(0==(8&Index))if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.XX=Value; else this.XY=Value; else if(0==(1&Index))this.XZ=Value; else this.XW=Value; else if(0==(2&Index))if(0==(1&Index))this.YX=Value; else this.YY=Value; else if(0==(1&Index))this.YZ=Value; else this.YW=Value; else if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.ZX=Value; else this.ZY=Value; else if(0==(1&Index))this.ZZ=Value; else this.ZW=Value; else if(0==(2&Index))if(0==(1&Index))this.WX=Value; else this.WY=Value; else if(0==(1&Index))this.WZ=Value; else this.WW=Value;;
	}
	public void Get(int Index, out uint Value){
		Value=(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.XX_XY:this.XZ_XW:(0==(1&Index))?this.YX_YY:this.YZ_YW:(0==(2&Index))?(0==(1&Index))?this.ZX_ZY:this.ZZ_ZW:(0==(1&Index))?this.WX_WY:this.WZ_WW;
	}
	public void Set(int Index, uint Value){
		if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.XX_XY=Value; else this.XZ_XW=Value; else if(0==(1&Index))this.YX_YY=Value; else this.YZ_YW=Value; else if(0==(2&Index))if(0==(1&Index))this.ZX_ZY=Value; else this.ZZ_ZW=Value; else if(0==(1&Index))this.WX_WY=Value; else this.WZ_WW=Value;;
	}
	public void Set(int Index, [In] ref uint Value){
		if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.XX_XY=Value; else this.XZ_XW=Value; else if(0==(1&Index))this.YX_YY=Value; else this.YZ_YW=Value; else if(0==(2&Index))if(0==(1&Index))this.ZX_ZY=Value; else this.ZZ_ZW=Value; else if(0==(1&Index))this.WX_WY=Value; else this.WZ_WW=Value;;
	}
	public void Get(int Index, out ulong Value){
		Value=(0==(2&Index))?(0==(1&Index))?this.XV:this.YV:(0==(1&Index))?this.ZV:this.WV;
	}
	public void Set(int Index, ulong Value){
		if(0==(2&Index))if(0==(1&Index))this.XV=Value; else this.YV=Value; else if(0==(1&Index))this.ZV=Value; else this.WV=Value;;
	}
	public void Set(int Index, [In] ref ulong Value){
		if(0==(2&Index))if(0==(1&Index))this.XV=Value; else this.YV=Value; else if(0==(1&Index))this.ZV=Value; else this.WV=Value;;
	}
	public void Get(int Index, out Row4I Value){
		Value=(0==(2&Index))?(0==(1&Index))?this.X:this.Y:(0==(1&Index))?this.Z:this.W;
	}
	public void Set(int Index, Row4I Value){
		if(0==(2&Index))if(0==(1&Index))this.X=Value; else this.Y=Value; else if(0==(1&Index))this.Z=Value; else this.W=Value;;
	}
	public void Set(int Index, [In] ref Row4I Value){
		if(0==(2&Index))if(0==(1&Index))this.X=Value; else this.Y=Value; else if(0==(1&Index))this.Z=Value; else this.W=Value;;
	}
}
[StructLayout(LayoutKind.Explicit)]
public partial struct Mtx : System.IEquatable<Mtx> {
	[FieldOffset(0x00)] public MtxI I;
	[FieldOffset(0x20)] public MtxF F;
	[FieldOffset(0x00)] public Row4I X_I;
	[FieldOffset(0x08)] public Row4I Y_I;
	[FieldOffset(0x10)] public Row4I Z_I;
	[FieldOffset(0x18)] public Row4I W_I;
	[FieldOffset(0x00)] public short XX_I;
	[FieldOffset(0x02)] public short XY_I;
	[FieldOffset(0x04)] public short XZ_I;
	[FieldOffset(0x06)] public short XW_I;
	[FieldOffset(0x00)] public ulong XV_I;
	[FieldOffset(0x08)] public short YX_I;
	[FieldOffset(0x0A)] public short YY_I;
	[FieldOffset(0x0C)] public short YZ_I;
	[FieldOffset(0x0E)] public short YW_I;
	[FieldOffset(0x08)] public ulong YV_I;
	[FieldOffset(0x10)] public short ZX_I;
	[FieldOffset(0x12)] public short ZY_I;
	[FieldOffset(0x14)] public short ZZ_I;
	[FieldOffset(0x16)] public short ZW_I;
	[FieldOffset(0x10)] public ulong ZV_I;
	[FieldOffset(0x18)] public short WX_I;
	[FieldOffset(0x1A)] public short WY_I;
	[FieldOffset(0x1C)] public short WZ_I;
	[FieldOffset(0x1E)] public short WW_I;
	[FieldOffset(0x18)] public ulong WV_I;
	[FieldOffset(0x00)] public uint XX_XY_I;
	[FieldOffset(0x04)] public uint XZ_XW_I;
	[FieldOffset(0x08)] public uint YX_YY_I;
	[FieldOffset(0x0C)] public uint YZ_YW_I;
	[FieldOffset(0x10)] public uint ZX_ZY_I;
	[FieldOffset(0x14)] public uint ZZ_ZW_I;
	[FieldOffset(0x18)] public uint WX_WY_I;
	[FieldOffset(0x1C)] public uint WZ_WW_I;
	[FieldOffset(0x20)] public Row4F X_F;
	[FieldOffset(0x28)] public Row4F Y_F;
	[FieldOffset(0x30)] public Row4F Z_F;
	[FieldOffset(0x38)] public Row4F W_F;
	[FieldOffset(0x20)] public ushort XX_F;
	[FieldOffset(0x22)] public ushort XY_F;
	[FieldOffset(0x24)] public ushort XZ_F;
	[FieldOffset(0x26)] public ushort XW_F;
	[FieldOffset(0x20)] public ulong XV_F;
	[FieldOffset(0x28)] public ushort YX_F;
	[FieldOffset(0x2A)] public ushort YY_F;
	[FieldOffset(0x2C)] public ushort YZ_F;
	[FieldOffset(0x2E)] public ushort YW_F;
	[FieldOffset(0x28)] public ulong YV_F;
	[FieldOffset(0x30)] public ushort ZX_F;
	[FieldOffset(0x32)] public ushort ZY_F;
	[FieldOffset(0x34)] public ushort ZZ_F;
	[FieldOffset(0x36)] public ushort ZW_F;
	[FieldOffset(0x30)] public ulong ZV_F;
	[FieldOffset(0x38)] public ushort WX_F;
	[FieldOffset(0x3A)] public ushort WY_F;
	[FieldOffset(0x3C)] public ushort WZ_F;
	[FieldOffset(0x3E)] public ushort WW_F;
	[FieldOffset(0x38)] public ulong WV_F;
	[FieldOffset(0x20)] public uint XX_XY_F;
	[FieldOffset(0x24)] public uint XZ_XW_F;
	[FieldOffset(0x28)] public uint YX_YY_F;
	[FieldOffset(0x2C)] public uint YZ_YW_F;
	[FieldOffset(0x30)] public uint ZX_ZY_F;
	[FieldOffset(0x34)] public uint ZZ_ZW_F;
	[FieldOffset(0x38)] public uint WX_WY_F;
	[FieldOffset(0x3C)] public uint WZ_WW_F;
	public static bool Equals(ref Mtx L, ref Mtx R){
		return L.XV_I==R.XV_I &&
			L.YV_I==R.YV_I &&
			L.ZV_I==R.ZV_I &&
			L.WV_I==R.WV_I &&
			L.XV_F==R.XV_F &&
			L.YV_F==R.YV_F &&
			L.ZV_F==R.ZV_F &&
			L.WV_F==R.WV_F;
	}
	public static bool Inequals(ref Mtx L, ref Mtx R){
		return L.XV_I!=R.XV_I ||
			L.YV_I!=R.YV_I ||
			L.ZV_I!=R.ZV_I ||
			L.WV_I!=R.WV_I ||
			L.XV_F!=R.XV_F ||
			L.YV_F!=R.YV_F ||
			L.ZV_F!=R.ZV_F ||
			L.WV_F!=R.WV_F;
	}
	public static bool operator == (Mtx L, Mtx R){
		return L.XV_I==R.XV_I &&
			L.YV_I==R.YV_I &&
			L.ZV_I==R.ZV_I &&
			L.WV_I==R.WV_I &&
			L.XV_F==R.XV_F &&
			L.YV_F==R.YV_F &&
			L.ZV_F==R.ZV_F &&
			L.WV_F==R.WV_F;
	}
	public static bool operator != (Mtx L, Mtx R){
		return L.XV_I!=R.XV_I ||
			L.YV_I!=R.YV_I ||
			L.ZV_I!=R.ZV_I ||
			L.WV_I!=R.WV_I ||
			L.XV_F!=R.XV_F ||
			L.YV_F!=R.YV_F ||
			L.ZV_F!=R.ZV_F ||
			L.WV_F!=R.WV_F;
	}
	public static int GetHashCode(ref Mtx Value){
		uint Temp;
		unchecked {
		return
			Value.XV_I.GetHashCode() ^
			(int)(((((Temp=(uint)Value.YV_I.GetHashCode()))<<5))|(Temp>>27)) ^
			(int)(((((Temp=(uint)Value.ZV_I.GetHashCode()))<<10))|(Temp>>22)) ^
			(int)(((((Temp=(uint)Value.WV_I.GetHashCode()))<<15))|(Temp>>17)) ^
			(int)(((((Temp=(uint)Value.XV_F.GetHashCode()))<<20))|(Temp>>12)) ^
			(int)(((((Temp=(uint)Value.YV_F.GetHashCode()))<<25))|(Temp>>7)) ^
			(int)(((((Temp=(uint)Value.ZV_F.GetHashCode()))<<30))|(Temp>>2)) ^
			(int)(((((Temp=(uint)Value.WV_F.GetHashCode()))<<3))|(Temp>>29));
		}
	}
	public bool Equals(Mtx other){return Equals(ref this, ref other);}
	public bool Equals(ref Mtx other){return Equals(ref this, ref other);}
	public override bool Equals(object obj){return obj is Mtx && ((Mtx)obj).Equals(ref this);}
	public override int GetHashCode(){return GetHashCode(ref this);}
	public void Get(int Index, out short Value){
		Value=(0==(8&Index))?(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.XX_I:this.XY_I:(0==(1&Index))?this.XZ_I:this.XW_I:(0==(2&Index))?(0==(1&Index))?this.YX_I:this.YY_I:(0==(1&Index))?this.YZ_I:this.YW_I:(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.ZX_I:this.ZY_I:(0==(1&Index))?this.ZZ_I:this.ZW_I:(0==(2&Index))?(0==(1&Index))?this.WX_I:this.WY_I:(0==(1&Index))?this.WZ_I:this.WW_I;
	}
	public void Set(int Index, short Value){
		if(0==(8&Index))if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.XX_I=Value; else this.XY_I=Value; else if(0==(1&Index))this.XZ_I=Value; else this.XW_I=Value; else if(0==(2&Index))if(0==(1&Index))this.YX_I=Value; else this.YY_I=Value; else if(0==(1&Index))this.YZ_I=Value; else this.YW_I=Value; else if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.ZX_I=Value; else this.ZY_I=Value; else if(0==(1&Index))this.ZZ_I=Value; else this.ZW_I=Value; else if(0==(2&Index))if(0==(1&Index))this.WX_I=Value; else this.WY_I=Value; else if(0==(1&Index))this.WZ_I=Value; else this.WW_I=Value;;
	}
	public void Set(int Index, [In] ref short Value){
		if(0==(8&Index))if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.XX_I=Value; else this.XY_I=Value; else if(0==(1&Index))this.XZ_I=Value; else this.XW_I=Value; else if(0==(2&Index))if(0==(1&Index))this.YX_I=Value; else this.YY_I=Value; else if(0==(1&Index))this.YZ_I=Value; else this.YW_I=Value; else if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.ZX_I=Value; else this.ZY_I=Value; else if(0==(1&Index))this.ZZ_I=Value; else this.ZW_I=Value; else if(0==(2&Index))if(0==(1&Index))this.WX_I=Value; else this.WY_I=Value; else if(0==(1&Index))this.WZ_I=Value; else this.WW_I=Value;;
	}
	public void Get(int Index, out ushort Value){
		Value=(0==(8&Index))?(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.XX_F:this.XY_F:(0==(1&Index))?this.XZ_F:this.XW_F:(0==(2&Index))?(0==(1&Index))?this.YX_F:this.YY_F:(0==(1&Index))?this.YZ_F:this.YW_F:(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.ZX_F:this.ZY_F:(0==(1&Index))?this.ZZ_F:this.ZW_F:(0==(2&Index))?(0==(1&Index))?this.WX_F:this.WY_F:(0==(1&Index))?this.WZ_F:this.WW_F;
	}
	public void Set(int Index, ushort Value){
		if(0==(8&Index))if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.XX_F=Value; else this.XY_F=Value; else if(0==(1&Index))this.XZ_F=Value; else this.XW_F=Value; else if(0==(2&Index))if(0==(1&Index))this.YX_F=Value; else this.YY_F=Value; else if(0==(1&Index))this.YZ_F=Value; else this.YW_F=Value; else if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.ZX_F=Value; else this.ZY_F=Value; else if(0==(1&Index))this.ZZ_F=Value; else this.ZW_F=Value; else if(0==(2&Index))if(0==(1&Index))this.WX_F=Value; else this.WY_F=Value; else if(0==(1&Index))this.WZ_F=Value; else this.WW_F=Value;;
	}
	public void Set(int Index, [In] ref ushort Value){
		if(0==(8&Index))if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.XX_F=Value; else this.XY_F=Value; else if(0==(1&Index))this.XZ_F=Value; else this.XW_F=Value; else if(0==(2&Index))if(0==(1&Index))this.YX_F=Value; else this.YY_F=Value; else if(0==(1&Index))this.YZ_F=Value; else this.YW_F=Value; else if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.ZX_F=Value; else this.ZY_F=Value; else if(0==(1&Index))this.ZZ_F=Value; else this.ZW_F=Value; else if(0==(2&Index))if(0==(1&Index))this.WX_F=Value; else this.WY_F=Value; else if(0==(1&Index))this.WZ_F=Value; else this.WW_F=Value;;
	}
	public void Get(int Index, out uint Value){
		Value=(0==(8&Index))?(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.XX_XY_I:this.XZ_XW_I:(0==(1&Index))?this.YX_YY_I:this.YZ_YW_I:(0==(2&Index))?(0==(1&Index))?this.ZX_ZY_I:this.ZZ_ZW_I:(0==(1&Index))?this.WX_WY_I:this.WZ_WW_I:(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.XX_XY_F:this.XZ_XW_F:(0==(1&Index))?this.YX_YY_F:this.YZ_YW_F:(0==(2&Index))?(0==(1&Index))?this.ZX_ZY_F:this.ZZ_ZW_F:(0==(1&Index))?this.WX_WY_F:this.WZ_WW_F;
	}
	public void Set(int Index, uint Value){
		if(0==(8&Index))if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.XX_XY_I=Value; else this.XZ_XW_I=Value; else if(0==(1&Index))this.YX_YY_I=Value; else this.YZ_YW_I=Value; else if(0==(2&Index))if(0==(1&Index))this.ZX_ZY_I=Value; else this.ZZ_ZW_I=Value; else if(0==(1&Index))this.WX_WY_I=Value; else this.WZ_WW_I=Value; else if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.XX_XY_F=Value; else this.XZ_XW_F=Value; else if(0==(1&Index))this.YX_YY_F=Value; else this.YZ_YW_F=Value; else if(0==(2&Index))if(0==(1&Index))this.ZX_ZY_F=Value; else this.ZZ_ZW_F=Value; else if(0==(1&Index))this.WX_WY_F=Value; else this.WZ_WW_F=Value;;
	}
	public void Set(int Index, [In] ref uint Value){
		if(0==(8&Index))if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.XX_XY_I=Value; else this.XZ_XW_I=Value; else if(0==(1&Index))this.YX_YY_I=Value; else this.YZ_YW_I=Value; else if(0==(2&Index))if(0==(1&Index))this.ZX_ZY_I=Value; else this.ZZ_ZW_I=Value; else if(0==(1&Index))this.WX_WY_I=Value; else this.WZ_WW_I=Value; else if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.XX_XY_F=Value; else this.XZ_XW_F=Value; else if(0==(1&Index))this.YX_YY_F=Value; else this.YZ_YW_F=Value; else if(0==(2&Index))if(0==(1&Index))this.ZX_ZY_F=Value; else this.ZZ_ZW_F=Value; else if(0==(1&Index))this.WX_WY_F=Value; else this.WZ_WW_F=Value;;
	}
	public void Get(int Index, out ulong Value){
		Value=(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.XV_I:this.YV_I:(0==(1&Index))?this.ZV_I:this.WV_I:(0==(2&Index))?(0==(1&Index))?this.XV_F:this.YV_F:(0==(1&Index))?this.ZV_F:this.WV_F;
	}
	public void Set(int Index, ulong Value){
		if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.XV_I=Value; else this.YV_I=Value; else if(0==(1&Index))this.ZV_I=Value; else this.WV_I=Value; else if(0==(2&Index))if(0==(1&Index))this.XV_F=Value; else this.YV_F=Value; else if(0==(1&Index))this.ZV_F=Value; else this.WV_F=Value;;
	}
	public void Set(int Index, [In] ref ulong Value){
		if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.XV_I=Value; else this.YV_I=Value; else if(0==(1&Index))this.ZV_I=Value; else this.WV_I=Value; else if(0==(2&Index))if(0==(1&Index))this.XV_F=Value; else this.YV_F=Value; else if(0==(1&Index))this.ZV_F=Value; else this.WV_F=Value;;
	}
	public void Get(int Index, out Row4F Value){
		Value=(0==(2&Index))?(0==(1&Index))?this.X_F:this.Y_F:(0==(1&Index))?this.Z_F:this.W_F;
	}
	public void Set(int Index, Row4F Value){
		if(0==(2&Index))if(0==(1&Index))this.X_F=Value; else this.Y_F=Value; else if(0==(1&Index))this.Z_F=Value; else this.W_F=Value;;
	}
	public void Set(int Index, [In] ref Row4F Value){
		if(0==(2&Index))if(0==(1&Index))this.X_F=Value; else this.Y_F=Value; else if(0==(1&Index))this.Z_F=Value; else this.W_F=Value;;
	}
	public void Get(int Index, out Row4I Value){
		Value=(0==(2&Index))?(0==(1&Index))?this.X_I:this.Y_I:(0==(1&Index))?this.Z_I:this.W_I;
	}
	public void Set(int Index, Row4I Value){
		if(0==(2&Index))if(0==(1&Index))this.X_I=Value; else this.Y_I=Value; else if(0==(1&Index))this.Z_I=Value; else this.W_I=Value;;
	}
	public void Set(int Index, [In] ref Row4I Value){
		if(0==(2&Index))if(0==(1&Index))this.X_I=Value; else this.Y_I=Value; else if(0==(1&Index))this.Z_I=Value; else this.W_I=Value;;
	}
	public int FixedXX { get => ((int)XX_I<<16)|XX_F;set { XX_I=(short)(value>>16);XX_F=(ushort)(value&ushort.MaxValue); } }
	public float SingleXX { get => (int)XX_I+(XX_F/(float)(1<<16));set { var floor = System.Math.Floor(value);XX_I=(short)floor;XX_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public double DoubleXX { get => (int)XX_I+(XX_F/(double)(1<<16));set { var floor = System.Math.Floor(value);XX_I=(short)floor;XX_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public int FixedXY { get => ((int)XY_I<<16)|XY_F;set { XY_I=(short)(value>>16);XY_F=(ushort)(value&ushort.MaxValue); } }
	public float SingleXY { get => (int)XY_I+(XY_F/(float)(1<<16));set { var floor = System.Math.Floor(value);XY_I=(short)floor;XY_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public double DoubleXY { get => (int)XY_I+(XY_F/(double)(1<<16));set { var floor = System.Math.Floor(value);XY_I=(short)floor;XY_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public int FixedXZ { get => ((int)XZ_I<<16)|XZ_F;set { XZ_I=(short)(value>>16);XZ_F=(ushort)(value&ushort.MaxValue); } }
	public float SingleXZ { get => (int)XZ_I+(XZ_F/(float)(1<<16));set { var floor = System.Math.Floor(value);XZ_I=(short)floor;XZ_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public double DoubleXZ { get => (int)XZ_I+(XZ_F/(double)(1<<16));set { var floor = System.Math.Floor(value);XZ_I=(short)floor;XZ_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public int FixedXW { get => ((int)XW_I<<16)|XW_F;set { XW_I=(short)(value>>16);XW_F=(ushort)(value&ushort.MaxValue); } }
	public float SingleXW { get => (int)XW_I+(XW_F/(float)(1<<16));set { var floor = System.Math.Floor(value);XW_I=(short)floor;XW_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public double DoubleXW { get => (int)XW_I+(XW_F/(double)(1<<16));set { var floor = System.Math.Floor(value);XW_I=(short)floor;XW_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public int FixedYX { get => ((int)YX_I<<16)|YX_F;set { YX_I=(short)(value>>16);YX_F=(ushort)(value&ushort.MaxValue); } }
	public float SingleYX { get => (int)YX_I+(YX_F/(float)(1<<16));set { var floor = System.Math.Floor(value);YX_I=(short)floor;YX_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public double DoubleYX { get => (int)YX_I+(YX_F/(double)(1<<16));set { var floor = System.Math.Floor(value);YX_I=(short)floor;YX_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public int FixedYY { get => ((int)YY_I<<16)|YY_F;set { YY_I=(short)(value>>16);YY_F=(ushort)(value&ushort.MaxValue); } }
	public float SingleYY { get => (int)YY_I+(YY_F/(float)(1<<16));set { var floor = System.Math.Floor(value);YY_I=(short)floor;YY_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public double DoubleYY { get => (int)YY_I+(YY_F/(double)(1<<16));set { var floor = System.Math.Floor(value);YY_I=(short)floor;YY_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public int FixedYZ { get => ((int)YZ_I<<16)|YZ_F;set { YZ_I=(short)(value>>16);YZ_F=(ushort)(value&ushort.MaxValue); } }
	public float SingleYZ { get => (int)YZ_I+(YZ_F/(float)(1<<16));set { var floor = System.Math.Floor(value);YZ_I=(short)floor;YZ_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public double DoubleYZ { get => (int)YZ_I+(YZ_F/(double)(1<<16));set { var floor = System.Math.Floor(value);YZ_I=(short)floor;YZ_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public int FixedYW { get => ((int)YW_I<<16)|YW_F;set { YW_I=(short)(value>>16);YW_F=(ushort)(value&ushort.MaxValue); } }
	public float SingleYW { get => (int)YW_I+(YW_F/(float)(1<<16));set { var floor = System.Math.Floor(value);YW_I=(short)floor;YW_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public double DoubleYW { get => (int)YW_I+(YW_F/(double)(1<<16));set { var floor = System.Math.Floor(value);YW_I=(short)floor;YW_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public int FixedZX { get => ((int)ZX_I<<16)|ZX_F;set { ZX_I=(short)(value>>16);ZX_F=(ushort)(value&ushort.MaxValue); } }
	public float SingleZX { get => (int)ZX_I+(ZX_F/(float)(1<<16));set { var floor = System.Math.Floor(value);ZX_I=(short)floor;ZX_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public double DoubleZX { get => (int)ZX_I+(ZX_F/(double)(1<<16));set { var floor = System.Math.Floor(value);ZX_I=(short)floor;ZX_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public int FixedZY { get => ((int)ZY_I<<16)|ZY_F;set { ZY_I=(short)(value>>16);ZY_F=(ushort)(value&ushort.MaxValue); } }
	public float SingleZY { get => (int)ZY_I+(ZY_F/(float)(1<<16));set { var floor = System.Math.Floor(value);ZY_I=(short)floor;ZY_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public double DoubleZY { get => (int)ZY_I+(ZY_F/(double)(1<<16));set { var floor = System.Math.Floor(value);ZY_I=(short)floor;ZY_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public int FixedZZ { get => ((int)ZZ_I<<16)|ZZ_F;set { ZZ_I=(short)(value>>16);ZZ_F=(ushort)(value&ushort.MaxValue); } }
	public float SingleZZ { get => (int)ZZ_I+(ZZ_F/(float)(1<<16));set { var floor = System.Math.Floor(value);ZZ_I=(short)floor;ZZ_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public double DoubleZZ { get => (int)ZZ_I+(ZZ_F/(double)(1<<16));set { var floor = System.Math.Floor(value);ZZ_I=(short)floor;ZZ_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public int FixedZW { get => ((int)ZW_I<<16)|ZW_F;set { ZW_I=(short)(value>>16);ZW_F=(ushort)(value&ushort.MaxValue); } }
	public float SingleZW { get => (int)ZW_I+(ZW_F/(float)(1<<16));set { var floor = System.Math.Floor(value);ZW_I=(short)floor;ZW_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public double DoubleZW { get => (int)ZW_I+(ZW_F/(double)(1<<16));set { var floor = System.Math.Floor(value);ZW_I=(short)floor;ZW_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public int FixedWX { get => ((int)WX_I<<16)|WX_F;set { WX_I=(short)(value>>16);WX_F=(ushort)(value&ushort.MaxValue); } }
	public float SingleWX { get => (int)WX_I+(WX_F/(float)(1<<16));set { var floor = System.Math.Floor(value);WX_I=(short)floor;WX_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public double DoubleWX { get => (int)WX_I+(WX_F/(double)(1<<16));set { var floor = System.Math.Floor(value);WX_I=(short)floor;WX_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public int FixedWY { get => ((int)WY_I<<16)|WY_F;set { WY_I=(short)(value>>16);WY_F=(ushort)(value&ushort.MaxValue); } }
	public float SingleWY { get => (int)WY_I+(WY_F/(float)(1<<16));set { var floor = System.Math.Floor(value);WY_I=(short)floor;WY_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public double DoubleWY { get => (int)WY_I+(WY_F/(double)(1<<16));set { var floor = System.Math.Floor(value);WY_I=(short)floor;WY_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public int FixedWZ { get => ((int)WZ_I<<16)|WZ_F;set { WZ_I=(short)(value>>16);WZ_F=(ushort)(value&ushort.MaxValue); } }
	public float SingleWZ { get => (int)WZ_I+(WZ_F/(float)(1<<16));set { var floor = System.Math.Floor(value);WZ_I=(short)floor;WZ_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public double DoubleWZ { get => (int)WZ_I+(WZ_F/(double)(1<<16));set { var floor = System.Math.Floor(value);WZ_I=(short)floor;WZ_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public int FixedWW { get => ((int)WW_I<<16)|WW_F;set { WW_I=(short)(value>>16);WW_F=(ushort)(value&ushort.MaxValue); } }
	public float SingleWW { get => (int)WW_I+(WW_F/(float)(1<<16));set { var floor = System.Math.Floor(value);WW_I=(short)floor;WW_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public double DoubleWW { get => (int)WW_I+(WW_F/(double)(1<<16));set { var floor = System.Math.Floor(value);WW_I=(short)floor;WW_F=(ushort)((int)(value-floor)&ushort.MaxValue); } }
	public int GetFixed(int Index){
		return (0==(8&Index))?(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.FixedXX:this.FixedXY:(0==(1&Index))?this.FixedXZ:this.FixedXW:(0==(2&Index))?(0==(1&Index))?this.FixedYX:this.FixedYY:(0==(1&Index))?this.FixedYZ:this.FixedYW:(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.FixedZX:this.FixedZY:(0==(1&Index))?this.FixedZZ:this.FixedZW:(0==(2&Index))?(0==(1&Index))?this.FixedWX:this.FixedWY:(0==(1&Index))?this.FixedWZ:this.FixedWW;
	}
	public void GetFixed(int Index, out int Value){
		Value=(0==(8&Index))?(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.FixedXX:this.FixedXY:(0==(1&Index))?this.FixedXZ:this.FixedXW:(0==(2&Index))?(0==(1&Index))?this.FixedYX:this.FixedYY:(0==(1&Index))?this.FixedYZ:this.FixedYW:(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.FixedZX:this.FixedZY:(0==(1&Index))?this.FixedZZ:this.FixedZW:(0==(2&Index))?(0==(1&Index))?this.FixedWX:this.FixedWY:(0==(1&Index))?this.FixedWZ:this.FixedWW;
	}
	public void SetFixed(int Index, int Value){
		if(0==(8&Index))if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.FixedXX=Value; else this.FixedXY=Value; else if(0==(1&Index))this.FixedXZ=Value; else this.FixedXW=Value; else if(0==(2&Index))if(0==(1&Index))this.FixedYX=Value; else this.FixedYY=Value; else if(0==(1&Index))this.FixedYZ=Value; else this.FixedYW=Value; else if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.FixedZX=Value; else this.FixedZY=Value; else if(0==(1&Index))this.FixedZZ=Value; else this.FixedZW=Value; else if(0==(2&Index))if(0==(1&Index))this.FixedWX=Value; else this.FixedWY=Value; else if(0==(1&Index))this.FixedWZ=Value; else this.FixedWW=Value;;
	}
	public void SetFixed(int Index, [In] ref int Value){
		if(0==(8&Index))if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.FixedXX=Value; else this.FixedXY=Value; else if(0==(1&Index))this.FixedXZ=Value; else this.FixedXW=Value; else if(0==(2&Index))if(0==(1&Index))this.FixedYX=Value; else this.FixedYY=Value; else if(0==(1&Index))this.FixedYZ=Value; else this.FixedYW=Value; else if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.FixedZX=Value; else this.FixedZY=Value; else if(0==(1&Index))this.FixedZZ=Value; else this.FixedZW=Value; else if(0==(2&Index))if(0==(1&Index))this.FixedWX=Value; else this.FixedWY=Value; else if(0==(1&Index))this.FixedWZ=Value; else this.FixedWW=Value;;
	}
	public float GetSingle(int Index){
		return (0==(8&Index))?(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.SingleXX:this.SingleXY:(0==(1&Index))?this.SingleXZ:this.SingleXW:(0==(2&Index))?(0==(1&Index))?this.SingleYX:this.SingleYY:(0==(1&Index))?this.SingleYZ:this.SingleYW:(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.SingleZX:this.SingleZY:(0==(1&Index))?this.SingleZZ:this.SingleZW:(0==(2&Index))?(0==(1&Index))?this.SingleWX:this.SingleWY:(0==(1&Index))?this.SingleWZ:this.SingleWW;
	}
	public void GetSingle(int Index, out float Value){
		Value=(0==(8&Index))?(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.SingleXX:this.SingleXY:(0==(1&Index))?this.SingleXZ:this.SingleXW:(0==(2&Index))?(0==(1&Index))?this.SingleYX:this.SingleYY:(0==(1&Index))?this.SingleYZ:this.SingleYW:(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.SingleZX:this.SingleZY:(0==(1&Index))?this.SingleZZ:this.SingleZW:(0==(2&Index))?(0==(1&Index))?this.SingleWX:this.SingleWY:(0==(1&Index))?this.SingleWZ:this.SingleWW;
	}
	public void SetSingle(int Index, float Value){
		if(0==(8&Index))if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.SingleXX=Value; else this.SingleXY=Value; else if(0==(1&Index))this.SingleXZ=Value; else this.SingleXW=Value; else if(0==(2&Index))if(0==(1&Index))this.SingleYX=Value; else this.SingleYY=Value; else if(0==(1&Index))this.SingleYZ=Value; else this.SingleYW=Value; else if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.SingleZX=Value; else this.SingleZY=Value; else if(0==(1&Index))this.SingleZZ=Value; else this.SingleZW=Value; else if(0==(2&Index))if(0==(1&Index))this.SingleWX=Value; else this.SingleWY=Value; else if(0==(1&Index))this.SingleWZ=Value; else this.SingleWW=Value;;
	}
	public void SetSingle(int Index, [In] ref float Value){
		if(0==(8&Index))if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.SingleXX=Value; else this.SingleXY=Value; else if(0==(1&Index))this.SingleXZ=Value; else this.SingleXW=Value; else if(0==(2&Index))if(0==(1&Index))this.SingleYX=Value; else this.SingleYY=Value; else if(0==(1&Index))this.SingleYZ=Value; else this.SingleYW=Value; else if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.SingleZX=Value; else this.SingleZY=Value; else if(0==(1&Index))this.SingleZZ=Value; else this.SingleZW=Value; else if(0==(2&Index))if(0==(1&Index))this.SingleWX=Value; else this.SingleWY=Value; else if(0==(1&Index))this.SingleWZ=Value; else this.SingleWW=Value;;
	}
	public double GetDouble(int Index){
		return (0==(8&Index))?(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.DoubleXX:this.DoubleXY:(0==(1&Index))?this.DoubleXZ:this.DoubleXW:(0==(2&Index))?(0==(1&Index))?this.DoubleYX:this.DoubleYY:(0==(1&Index))?this.DoubleYZ:this.DoubleYW:(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.DoubleZX:this.DoubleZY:(0==(1&Index))?this.DoubleZZ:this.DoubleZW:(0==(2&Index))?(0==(1&Index))?this.DoubleWX:this.DoubleWY:(0==(1&Index))?this.DoubleWZ:this.DoubleWW;
	}
	public void GetDouble(int Index, out double Value){
		Value=(0==(8&Index))?(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.DoubleXX:this.DoubleXY:(0==(1&Index))?this.DoubleXZ:this.DoubleXW:(0==(2&Index))?(0==(1&Index))?this.DoubleYX:this.DoubleYY:(0==(1&Index))?this.DoubleYZ:this.DoubleYW:(0==(4&Index))?(0==(2&Index))?(0==(1&Index))?this.DoubleZX:this.DoubleZY:(0==(1&Index))?this.DoubleZZ:this.DoubleZW:(0==(2&Index))?(0==(1&Index))?this.DoubleWX:this.DoubleWY:(0==(1&Index))?this.DoubleWZ:this.DoubleWW;
	}
	public void SetDouble(int Index, double Value){
		if(0==(8&Index))if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.DoubleXX=Value; else this.DoubleXY=Value; else if(0==(1&Index))this.DoubleXZ=Value; else this.DoubleXW=Value; else if(0==(2&Index))if(0==(1&Index))this.DoubleYX=Value; else this.DoubleYY=Value; else if(0==(1&Index))this.DoubleYZ=Value; else this.DoubleYW=Value; else if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.DoubleZX=Value; else this.DoubleZY=Value; else if(0==(1&Index))this.DoubleZZ=Value; else this.DoubleZW=Value; else if(0==(2&Index))if(0==(1&Index))this.DoubleWX=Value; else this.DoubleWY=Value; else if(0==(1&Index))this.DoubleWZ=Value; else this.DoubleWW=Value;;
	}
	public void SetDouble(int Index, [In] ref double Value){
		if(0==(8&Index))if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.DoubleXX=Value; else this.DoubleXY=Value; else if(0==(1&Index))this.DoubleXZ=Value; else this.DoubleXW=Value; else if(0==(2&Index))if(0==(1&Index))this.DoubleYX=Value; else this.DoubleYY=Value; else if(0==(1&Index))this.DoubleYZ=Value; else this.DoubleYW=Value; else if(0==(4&Index))if(0==(2&Index))if(0==(1&Index))this.DoubleZX=Value; else this.DoubleZY=Value; else if(0==(1&Index))this.DoubleZZ=Value; else this.DoubleZW=Value; else if(0==(2&Index))if(0==(1&Index))this.DoubleWX=Value; else this.DoubleWY=Value; else if(0==(1&Index))this.DoubleWZ=Value; else this.DoubleWW=Value;;
	}
	public static void Multiply(ref Mtx L, ref Mtx R, out Mtx Result){
		int
			LXX=((int)L.XX_I<<16)|L.XX_F,RXX=((int)R.XX_I<<16)|R.XX_F,OXX,
			LXY=((int)L.XY_I<<16)|L.XY_F,RXY=((int)R.XY_I<<16)|R.XY_F,OXY,
			LXZ=((int)L.XZ_I<<16)|L.XZ_F,RXZ=((int)R.XZ_I<<16)|R.XZ_F,OXZ,
			LXW=((int)L.XW_I<<16)|L.XW_F,RXW=((int)R.XW_I<<16)|R.XW_F,OXW,
			LYX=((int)L.YX_I<<16)|L.YX_F,RYX=((int)R.YX_I<<16)|R.YX_F,OYX,
			LYY=((int)L.YY_I<<16)|L.YY_F,RYY=((int)R.YY_I<<16)|R.YY_F,OYY,
			LYZ=((int)L.YZ_I<<16)|L.YZ_F,RYZ=((int)R.YZ_I<<16)|R.YZ_F,OYZ,
			LYW=((int)L.YW_I<<16)|L.YW_F,RYW=((int)R.YW_I<<16)|R.YW_F,OYW,
			LZX=((int)L.ZX_I<<16)|L.ZX_F,RZX=((int)R.ZX_I<<16)|R.ZX_F,OZX,
			LZY=((int)L.ZY_I<<16)|L.ZY_F,RZY=((int)R.ZY_I<<16)|R.ZY_F,OZY,
			LZZ=((int)L.ZZ_I<<16)|L.ZZ_F,RZZ=((int)R.ZZ_I<<16)|R.ZZ_F,OZZ,
			LZW=((int)L.ZW_I<<16)|L.ZW_F,RZW=((int)R.ZW_I<<16)|R.ZW_F,OZW,
			LWX=((int)L.WX_I<<16)|L.WX_F,RWX=((int)R.WX_I<<16)|R.WX_F,OWX,
			LWY=((int)L.WY_I<<16)|L.WY_F,RWY=((int)R.WY_I<<16)|R.WY_F,OWY,
			LWZ=((int)L.WZ_I<<16)|L.WZ_F,RWZ=((int)R.WZ_I<<16)|R.WZ_F,OWZ,
			LWW=((int)L.WW_I<<16)|L.WW_F,RWW=((int)R.WW_I<<16)|R.WW_F,OWW;
		OXX=(int)(((long)LXX * RXX +
			(long)LXY * RYX +
			(long)LXZ * RZX +
			(long)LXW * RWX)>>16);
		OXY=(int)(((long)LXX * RXY +
			(long)LXY * RYY +
			(long)LXZ * RZY +
			(long)LXW * RWY)>>16);
		OXZ=(int)(((long)LXX * RXZ +
			(long)LXY * RYZ +
			(long)LXZ * RZZ +
			(long)LXW * RWZ)>>16);
		OXW=(int)(((long)LXX * RXW +
			(long)LXY * RYW +
			(long)LXZ * RZW +
			(long)LXW * RWW)>>16);
		OYX=(int)(((long)LYX * RXX +
			(long)LYY * RYX +
			(long)LYZ * RZX +
			(long)LYW * RWX)>>16);
		OYY=(int)(((long)LYX * RXY +
			(long)LYY * RYY +
			(long)LYZ * RZY +
			(long)LYW * RWY)>>16);
		OYZ=(int)(((long)LYX * RXZ +
			(long)LYY * RYZ +
			(long)LYZ * RZZ +
			(long)LYW * RWZ)>>16);
		OYW=(int)(((long)LYX * RXW +
			(long)LYY * RYW +
			(long)LYZ * RZW +
			(long)LYW * RWW)>>16);
		OZX=(int)(((long)LZX * RXX +
			(long)LZY * RYX +
			(long)LZZ * RZX +
			(long)LZW * RWX)>>16);
		OZY=(int)(((long)LZX * RXY +
			(long)LZY * RYY +
			(long)LZZ * RZY +
			(long)LZW * RWY)>>16);
		OZZ=(int)(((long)LZX * RXZ +
			(long)LZY * RYZ +
			(long)LZZ * RZZ +
			(long)LZW * RWZ)>>16);
		OZW=(int)(((long)LZX * RXW +
			(long)LZY * RYW +
			(long)LZZ * RZW +
			(long)LZW * RWW)>>16);
		OWX=(int)(((long)LWX * RXX +
			(long)LWY * RYX +
			(long)LWZ * RZX +
			(long)LWW * RWX)>>16);
		OWY=(int)(((long)LWX * RXY +
			(long)LWY * RYY +
			(long)LWZ * RZY +
			(long)LWW * RWY)>>16);
		OWZ=(int)(((long)LWX * RXZ +
			(long)LWY * RYZ +
			(long)LWZ * RZZ +
			(long)LWW * RWZ)>>16);
		OWW=(int)(((long)LWX * RXW +
			(long)LWY * RYW +
			(long)LWZ * RZW +
			(long)LWW * RWW)>>16);
		Result = new Mtx{
		XX_F=(ushort)(OXX&ushort.MaxValue),
		XX_I=(short)(OXX>>16),
		XY_F=(ushort)(OXY&ushort.MaxValue),
		XY_I=(short)(OXY>>16),
		XZ_F=(ushort)(OXZ&ushort.MaxValue),
		XZ_I=(short)(OXZ>>16),
		XW_F=(ushort)(OXW&ushort.MaxValue),
		XW_I=(short)(OXW>>16),
		YX_F=(ushort)(OYX&ushort.MaxValue),
		YX_I=(short)(OYX>>16),
		YY_F=(ushort)(OYY&ushort.MaxValue),
		YY_I=(short)(OYY>>16),
		YZ_F=(ushort)(OYZ&ushort.MaxValue),
		YZ_I=(short)(OYZ>>16),
		YW_F=(ushort)(OYW&ushort.MaxValue),
		YW_I=(short)(OYW>>16),
		ZX_F=(ushort)(OZX&ushort.MaxValue),
		ZX_I=(short)(OZX>>16),
		ZY_F=(ushort)(OZY&ushort.MaxValue),
		ZY_I=(short)(OZY>>16),
		ZZ_F=(ushort)(OZZ&ushort.MaxValue),
		ZZ_I=(short)(OZZ>>16),
		ZW_F=(ushort)(OZW&ushort.MaxValue),
		ZW_I=(short)(OZW>>16),
		WX_F=(ushort)(OWX&ushort.MaxValue),
		WX_I=(short)(OWX>>16),
		WY_F=(ushort)(OWY&ushort.MaxValue),
		WY_I=(short)(OWY>>16),
		WZ_F=(ushort)(OWZ&ushort.MaxValue),
		WZ_I=(short)(OWZ>>16),
		WW_F=(ushort)(OWW&ushort.MaxValue),
		WW_I=(short)(OWW>>16),
		};
	}
	public static Mtx Multiply(ref Mtx L, ref Mtx R){
		int
			LXX=((int)L.XX_I<<16)|L.XX_F,RXX=((int)R.XX_I<<16)|R.XX_F,OXX,
			LXY=((int)L.XY_I<<16)|L.XY_F,RXY=((int)R.XY_I<<16)|R.XY_F,OXY,
			LXZ=((int)L.XZ_I<<16)|L.XZ_F,RXZ=((int)R.XZ_I<<16)|R.XZ_F,OXZ,
			LXW=((int)L.XW_I<<16)|L.XW_F,RXW=((int)R.XW_I<<16)|R.XW_F,OXW,
			LYX=((int)L.YX_I<<16)|L.YX_F,RYX=((int)R.YX_I<<16)|R.YX_F,OYX,
			LYY=((int)L.YY_I<<16)|L.YY_F,RYY=((int)R.YY_I<<16)|R.YY_F,OYY,
			LYZ=((int)L.YZ_I<<16)|L.YZ_F,RYZ=((int)R.YZ_I<<16)|R.YZ_F,OYZ,
			LYW=((int)L.YW_I<<16)|L.YW_F,RYW=((int)R.YW_I<<16)|R.YW_F,OYW,
			LZX=((int)L.ZX_I<<16)|L.ZX_F,RZX=((int)R.ZX_I<<16)|R.ZX_F,OZX,
			LZY=((int)L.ZY_I<<16)|L.ZY_F,RZY=((int)R.ZY_I<<16)|R.ZY_F,OZY,
			LZZ=((int)L.ZZ_I<<16)|L.ZZ_F,RZZ=((int)R.ZZ_I<<16)|R.ZZ_F,OZZ,
			LZW=((int)L.ZW_I<<16)|L.ZW_F,RZW=((int)R.ZW_I<<16)|R.ZW_F,OZW,
			LWX=((int)L.WX_I<<16)|L.WX_F,RWX=((int)R.WX_I<<16)|R.WX_F,OWX,
			LWY=((int)L.WY_I<<16)|L.WY_F,RWY=((int)R.WY_I<<16)|R.WY_F,OWY,
			LWZ=((int)L.WZ_I<<16)|L.WZ_F,RWZ=((int)R.WZ_I<<16)|R.WZ_F,OWZ,
			LWW=((int)L.WW_I<<16)|L.WW_F,RWW=((int)R.WW_I<<16)|R.WW_F,OWW;
		OXX=(int)(((long)LXX * RXX +
			(long)LXY * RYX +
			(long)LXZ * RZX +
			(long)LXW * RWX)>>16);
		OXY=(int)(((long)LXX * RXY +
			(long)LXY * RYY +
			(long)LXZ * RZY +
			(long)LXW * RWY)>>16);
		OXZ=(int)(((long)LXX * RXZ +
			(long)LXY * RYZ +
			(long)LXZ * RZZ +
			(long)LXW * RWZ)>>16);
		OXW=(int)(((long)LXX * RXW +
			(long)LXY * RYW +
			(long)LXZ * RZW +
			(long)LXW * RWW)>>16);
		OYX=(int)(((long)LYX * RXX +
			(long)LYY * RYX +
			(long)LYZ * RZX +
			(long)LYW * RWX)>>16);
		OYY=(int)(((long)LYX * RXY +
			(long)LYY * RYY +
			(long)LYZ * RZY +
			(long)LYW * RWY)>>16);
		OYZ=(int)(((long)LYX * RXZ +
			(long)LYY * RYZ +
			(long)LYZ * RZZ +
			(long)LYW * RWZ)>>16);
		OYW=(int)(((long)LYX * RXW +
			(long)LYY * RYW +
			(long)LYZ * RZW +
			(long)LYW * RWW)>>16);
		OZX=(int)(((long)LZX * RXX +
			(long)LZY * RYX +
			(long)LZZ * RZX +
			(long)LZW * RWX)>>16);
		OZY=(int)(((long)LZX * RXY +
			(long)LZY * RYY +
			(long)LZZ * RZY +
			(long)LZW * RWY)>>16);
		OZZ=(int)(((long)LZX * RXZ +
			(long)LZY * RYZ +
			(long)LZZ * RZZ +
			(long)LZW * RWZ)>>16);
		OZW=(int)(((long)LZX * RXW +
			(long)LZY * RYW +
			(long)LZZ * RZW +
			(long)LZW * RWW)>>16);
		OWX=(int)(((long)LWX * RXX +
			(long)LWY * RYX +
			(long)LWZ * RZX +
			(long)LWW * RWX)>>16);
		OWY=(int)(((long)LWX * RXY +
			(long)LWY * RYY +
			(long)LWZ * RZY +
			(long)LWW * RWY)>>16);
		OWZ=(int)(((long)LWX * RXZ +
			(long)LWY * RYZ +
			(long)LWZ * RZZ +
			(long)LWW * RWZ)>>16);
		OWW=(int)(((long)LWX * RXW +
			(long)LWY * RYW +
			(long)LWZ * RZW +
			(long)LWW * RWW)>>16);
		return new Mtx{
		XX_F=(ushort)(OXX&ushort.MaxValue),
		XX_I=(short)(OXX>>16),
		XY_F=(ushort)(OXY&ushort.MaxValue),
		XY_I=(short)(OXY>>16),
		XZ_F=(ushort)(OXZ&ushort.MaxValue),
		XZ_I=(short)(OXZ>>16),
		XW_F=(ushort)(OXW&ushort.MaxValue),
		XW_I=(short)(OXW>>16),
		YX_F=(ushort)(OYX&ushort.MaxValue),
		YX_I=(short)(OYX>>16),
		YY_F=(ushort)(OYY&ushort.MaxValue),
		YY_I=(short)(OYY>>16),
		YZ_F=(ushort)(OYZ&ushort.MaxValue),
		YZ_I=(short)(OYZ>>16),
		YW_F=(ushort)(OYW&ushort.MaxValue),
		YW_I=(short)(OYW>>16),
		ZX_F=(ushort)(OZX&ushort.MaxValue),
		ZX_I=(short)(OZX>>16),
		ZY_F=(ushort)(OZY&ushort.MaxValue),
		ZY_I=(short)(OZY>>16),
		ZZ_F=(ushort)(OZZ&ushort.MaxValue),
		ZZ_I=(short)(OZZ>>16),
		ZW_F=(ushort)(OZW&ushort.MaxValue),
		ZW_I=(short)(OZW>>16),
		WX_F=(ushort)(OWX&ushort.MaxValue),
		WX_I=(short)(OWX>>16),
		WY_F=(ushort)(OWY&ushort.MaxValue),
		WY_I=(short)(OWY>>16),
		WZ_F=(ushort)(OWZ&ushort.MaxValue),
		WZ_I=(short)(OWZ>>16),
		WW_F=(ushort)(OWW&ushort.MaxValue),
		WW_I=(short)(OWW>>16),
		};
	}
	public static void MultiplyUpdate(ref Mtx L, ref Mtx R, ref Mtx Result){
		int
			LXX=((int)L.XX_I<<16)|L.XX_F,RXX=((int)R.XX_I<<16)|R.XX_F,OXX,
			LXY=((int)L.XY_I<<16)|L.XY_F,RXY=((int)R.XY_I<<16)|R.XY_F,OXY,
			LXZ=((int)L.XZ_I<<16)|L.XZ_F,RXZ=((int)R.XZ_I<<16)|R.XZ_F,OXZ,
			LXW=((int)L.XW_I<<16)|L.XW_F,RXW=((int)R.XW_I<<16)|R.XW_F,OXW,
			LYX=((int)L.YX_I<<16)|L.YX_F,RYX=((int)R.YX_I<<16)|R.YX_F,OYX,
			LYY=((int)L.YY_I<<16)|L.YY_F,RYY=((int)R.YY_I<<16)|R.YY_F,OYY,
			LYZ=((int)L.YZ_I<<16)|L.YZ_F,RYZ=((int)R.YZ_I<<16)|R.YZ_F,OYZ,
			LYW=((int)L.YW_I<<16)|L.YW_F,RYW=((int)R.YW_I<<16)|R.YW_F,OYW,
			LZX=((int)L.ZX_I<<16)|L.ZX_F,RZX=((int)R.ZX_I<<16)|R.ZX_F,OZX,
			LZY=((int)L.ZY_I<<16)|L.ZY_F,RZY=((int)R.ZY_I<<16)|R.ZY_F,OZY,
			LZZ=((int)L.ZZ_I<<16)|L.ZZ_F,RZZ=((int)R.ZZ_I<<16)|R.ZZ_F,OZZ,
			LZW=((int)L.ZW_I<<16)|L.ZW_F,RZW=((int)R.ZW_I<<16)|R.ZW_F,OZW,
			LWX=((int)L.WX_I<<16)|L.WX_F,RWX=((int)R.WX_I<<16)|R.WX_F,OWX,
			LWY=((int)L.WY_I<<16)|L.WY_F,RWY=((int)R.WY_I<<16)|R.WY_F,OWY,
			LWZ=((int)L.WZ_I<<16)|L.WZ_F,RWZ=((int)R.WZ_I<<16)|R.WZ_F,OWZ,
			LWW=((int)L.WW_I<<16)|L.WW_F,RWW=((int)R.WW_I<<16)|R.WW_F,OWW;
		OXX=(int)(((long)LXX * RXX +
			(long)LXY * RYX +
			(long)LXZ * RZX +
			(long)LXW * RWX)>>16);
		OXY=(int)(((long)LXX * RXY +
			(long)LXY * RYY +
			(long)LXZ * RZY +
			(long)LXW * RWY)>>16);
		OXZ=(int)(((long)LXX * RXZ +
			(long)LXY * RYZ +
			(long)LXZ * RZZ +
			(long)LXW * RWZ)>>16);
		OXW=(int)(((long)LXX * RXW +
			(long)LXY * RYW +
			(long)LXZ * RZW +
			(long)LXW * RWW)>>16);
		OYX=(int)(((long)LYX * RXX +
			(long)LYY * RYX +
			(long)LYZ * RZX +
			(long)LYW * RWX)>>16);
		OYY=(int)(((long)LYX * RXY +
			(long)LYY * RYY +
			(long)LYZ * RZY +
			(long)LYW * RWY)>>16);
		OYZ=(int)(((long)LYX * RXZ +
			(long)LYY * RYZ +
			(long)LYZ * RZZ +
			(long)LYW * RWZ)>>16);
		OYW=(int)(((long)LYX * RXW +
			(long)LYY * RYW +
			(long)LYZ * RZW +
			(long)LYW * RWW)>>16);
		OZX=(int)(((long)LZX * RXX +
			(long)LZY * RYX +
			(long)LZZ * RZX +
			(long)LZW * RWX)>>16);
		OZY=(int)(((long)LZX * RXY +
			(long)LZY * RYY +
			(long)LZZ * RZY +
			(long)LZW * RWY)>>16);
		OZZ=(int)(((long)LZX * RXZ +
			(long)LZY * RYZ +
			(long)LZZ * RZZ +
			(long)LZW * RWZ)>>16);
		OZW=(int)(((long)LZX * RXW +
			(long)LZY * RYW +
			(long)LZZ * RZW +
			(long)LZW * RWW)>>16);
		OWX=(int)(((long)LWX * RXX +
			(long)LWY * RYX +
			(long)LWZ * RZX +
			(long)LWW * RWX)>>16);
		OWY=(int)(((long)LWX * RXY +
			(long)LWY * RYY +
			(long)LWZ * RZY +
			(long)LWW * RWY)>>16);
		OWZ=(int)(((long)LWX * RXZ +
			(long)LWY * RYZ +
			(long)LWZ * RZZ +
			(long)LWW * RWZ)>>16);
		OWW=(int)(((long)LWX * RXW +
			(long)LWY * RYW +
			(long)LWZ * RZW +
			(long)LWW * RWW)>>16);
		Result.XX_F=(ushort)(OXX&ushort.MaxValue);
		Result.XX_I=(short)(OXX>>16);
		Result.XY_F=(ushort)(OXY&ushort.MaxValue);
		Result.XY_I=(short)(OXY>>16);
		Result.XZ_F=(ushort)(OXZ&ushort.MaxValue);
		Result.XZ_I=(short)(OXZ>>16);
		Result.XW_F=(ushort)(OXW&ushort.MaxValue);
		Result.XW_I=(short)(OXW>>16);
		Result.YX_F=(ushort)(OYX&ushort.MaxValue);
		Result.YX_I=(short)(OYX>>16);
		Result.YY_F=(ushort)(OYY&ushort.MaxValue);
		Result.YY_I=(short)(OYY>>16);
		Result.YZ_F=(ushort)(OYZ&ushort.MaxValue);
		Result.YZ_I=(short)(OYZ>>16);
		Result.YW_F=(ushort)(OYW&ushort.MaxValue);
		Result.YW_I=(short)(OYW>>16);
		Result.ZX_F=(ushort)(OZX&ushort.MaxValue);
		Result.ZX_I=(short)(OZX>>16);
		Result.ZY_F=(ushort)(OZY&ushort.MaxValue);
		Result.ZY_I=(short)(OZY>>16);
		Result.ZZ_F=(ushort)(OZZ&ushort.MaxValue);
		Result.ZZ_I=(short)(OZZ>>16);
		Result.ZW_F=(ushort)(OZW&ushort.MaxValue);
		Result.ZW_I=(short)(OZW>>16);
		Result.WX_F=(ushort)(OWX&ushort.MaxValue);
		Result.WX_I=(short)(OWX>>16);
		Result.WY_F=(ushort)(OWY&ushort.MaxValue);
		Result.WY_I=(short)(OWY>>16);
		Result.WZ_F=(ushort)(OWZ&ushort.MaxValue);
		Result.WZ_I=(short)(OWZ>>16);
		Result.WW_F=(ushort)(OWW&ushort.MaxValue);
		Result.WW_I=(short)(OWW>>16);
	}
	public static void Convert([In]ref Mtx In, out Matrix4 Out){
		Out.Row0.X=(int)In.XX_I+(In.XX_F/(float)(1<<16));
		Out.Row0.Y=(int)In.XY_I+(In.XY_F/(float)(1<<16));
		Out.Row0.Z=(int)In.XZ_I+(In.XZ_F/(float)(1<<16));
		Out.Row0.W=(int)In.XW_I+(In.XW_F/(float)(1<<16));
		Out.Row1.X=(int)In.YX_I+(In.YX_F/(float)(1<<16));
		Out.Row1.Y=(int)In.YY_I+(In.YY_F/(float)(1<<16));
		Out.Row1.Z=(int)In.YZ_I+(In.YZ_F/(float)(1<<16));
		Out.Row1.W=(int)In.YW_I+(In.YW_F/(float)(1<<16));
		Out.Row2.X=(int)In.ZX_I+(In.ZX_F/(float)(1<<16));
		Out.Row2.Y=(int)In.ZY_I+(In.ZY_F/(float)(1<<16));
		Out.Row2.Z=(int)In.ZZ_I+(In.ZZ_F/(float)(1<<16));
		Out.Row2.W=(int)In.ZW_I+(In.ZW_F/(float)(1<<16));
		Out.Row3.X=(int)In.WX_I+(In.WX_F/(float)(1<<16));
		Out.Row3.Y=(int)In.WY_I+(In.WY_F/(float)(1<<16));
		Out.Row3.Z=(int)In.WZ_I+(In.WZ_F/(float)(1<<16));
		Out.Row3.W=(int)In.WW_I+(In.WW_F/(float)(1<<16));
	}
	public static void Convert([In]ref Matrix4 In, out Mtx Out){
		Out = new Mtx{
			SingleXX=In.Row0.X,
			SingleXY=In.Row0.Y,
			SingleXZ=In.Row0.Z,
			SingleXW=In.Row0.W,
			SingleYX=In.Row1.X,
			SingleYY=In.Row1.Y,
			SingleYZ=In.Row1.Z,
			SingleYW=In.Row1.W,
			SingleZX=In.Row2.X,
			SingleZY=In.Row2.Y,
			SingleZZ=In.Row2.Z,
			SingleZW=In.Row2.W,
			SingleWX=In.Row3.X,
			SingleWY=In.Row3.Y,
			SingleWZ=In.Row3.Z,
			SingleWW=In.Row3.W,
		};
	}
	public static void Convert([In]ref Mtx In, out Matrix4d Out){
		Out.Row0.X=(int)In.XX_I+(In.XX_F/(double)(1<<16));
		Out.Row0.Y=(int)In.XY_I+(In.XY_F/(double)(1<<16));
		Out.Row0.Z=(int)In.XZ_I+(In.XZ_F/(double)(1<<16));
		Out.Row0.W=(int)In.XW_I+(In.XW_F/(double)(1<<16));
		Out.Row1.X=(int)In.YX_I+(In.YX_F/(double)(1<<16));
		Out.Row1.Y=(int)In.YY_I+(In.YY_F/(double)(1<<16));
		Out.Row1.Z=(int)In.YZ_I+(In.YZ_F/(double)(1<<16));
		Out.Row1.W=(int)In.YW_I+(In.YW_F/(double)(1<<16));
		Out.Row2.X=(int)In.ZX_I+(In.ZX_F/(double)(1<<16));
		Out.Row2.Y=(int)In.ZY_I+(In.ZY_F/(double)(1<<16));
		Out.Row2.Z=(int)In.ZZ_I+(In.ZZ_F/(double)(1<<16));
		Out.Row2.W=(int)In.ZW_I+(In.ZW_F/(double)(1<<16));
		Out.Row3.X=(int)In.WX_I+(In.WX_F/(double)(1<<16));
		Out.Row3.Y=(int)In.WY_I+(In.WY_F/(double)(1<<16));
		Out.Row3.Z=(int)In.WZ_I+(In.WZ_F/(double)(1<<16));
		Out.Row3.W=(int)In.WW_I+(In.WW_F/(double)(1<<16));
	}
	public static void Convert([In]ref Matrix4d In, out Mtx Out){
		Out = new Mtx{
			DoubleXX=In.Row0.X,
			DoubleXY=In.Row0.Y,
			DoubleXZ=In.Row0.Z,
			DoubleXW=In.Row0.W,
			DoubleYX=In.Row1.X,
			DoubleYY=In.Row1.Y,
			DoubleYZ=In.Row1.Z,
			DoubleYW=In.Row1.W,
			DoubleZX=In.Row2.X,
			DoubleZY=In.Row2.Y,
			DoubleZZ=In.Row2.Z,
			DoubleZW=In.Row2.W,
			DoubleWX=In.Row3.X,
			DoubleWY=In.Row3.Y,
			DoubleWZ=In.Row3.Z,
			DoubleWW=In.Row3.W,
		};
	}
}
}
