using System.Runtime.InteropServices;
using System.Diagnostics;
namespace Quad64 {
	[StructLayout(LayoutKind.Explicit,Size=0x88)]
public partial struct LightArray : System.IEquatable<LightArray>{

	private static Light OutOfRangeLight(){throw new System.ArgumentOutOfRangeException("index");}
	private static uint OutOfRangeWord(){throw new System.ArgumentOutOfRangeException("index");}

	[FieldOffset(0x00)] public Light Light1;
	[FieldOffset(0x20)] public Light Light2;
	[FieldOffset(0x40)] public Light Light3;
	[FieldOffset(0x60)] public Light Light4;
	[FieldOffset(0x80)] public Light Light5;
	[FieldOffset(0xA0)] public Light Light6;
	[FieldOffset(0xC0)] public Light Light7;
	[FieldOffset(0xE0)] public Light Light8;
	[DebuggerBrowsable(DebuggerBrowsableState.Never),FieldOffset(0x00)] public Lights0 Lights0;
	[DebuggerBrowsable(DebuggerBrowsableState.Never),FieldOffset(0x00)] public Lights1 Lights1;
	[DebuggerBrowsable(DebuggerBrowsableState.Never),FieldOffset(0x00)] public Lights2 Lights2;
	[DebuggerBrowsable(DebuggerBrowsableState.Never),FieldOffset(0x00)] public Lights3 Lights3;
	[DebuggerBrowsable(DebuggerBrowsableState.Never),FieldOffset(0x00)] public Lights4 Lights4;
	[DebuggerBrowsable(DebuggerBrowsableState.Never),FieldOffset(0x00)] public Lights5 Lights5;
	[DebuggerBrowsable(DebuggerBrowsableState.Never),FieldOffset(0x00)] public Lights6 Lights6;
	[DebuggerBrowsable(DebuggerBrowsableState.Never),FieldOffset(0x00)] public Lights7 Lights7;
	public static explicit operator LightArray(Lights0 Value){ return new LightArray { Lights0=Value, }; }
	public static explicit operator Lights0(LightArray Value){ return Value.Lights0; }
	public void Get(out Lights0 Value){Value=Lights0;}
	public void Set(ref Lights0 Value){Lights0=Value;}
	public static explicit operator LightArray(Lights1 Value){ return new LightArray { Lights1=Value, }; }
	public static explicit operator Lights1(LightArray Value){ return Value.Lights1; }
	public void Get(out Lights1 Value){Value=Lights1;}
	public void Set(ref Lights1 Value){Lights1=Value;}
	public static explicit operator LightArray(Lights2 Value){ return new LightArray { Lights2=Value, }; }
	public static explicit operator Lights2(LightArray Value){ return Value.Lights2; }
	public void Get(out Lights2 Value){Value=Lights2;}
	public void Set(ref Lights2 Value){Lights2=Value;}
	public static explicit operator LightArray(Lights3 Value){ return new LightArray { Lights3=Value, }; }
	public static explicit operator Lights3(LightArray Value){ return Value.Lights3; }
	public void Get(out Lights3 Value){Value=Lights3;}
	public void Set(ref Lights3 Value){Lights3=Value;}
	public static explicit operator LightArray(Lights4 Value){ return new LightArray { Lights4=Value, }; }
	public static explicit operator Lights4(LightArray Value){ return Value.Lights4; }
	public void Get(out Lights4 Value){Value=Lights4;}
	public void Set(ref Lights4 Value){Lights4=Value;}
	public static explicit operator LightArray(Lights5 Value){ return new LightArray { Lights5=Value, }; }
	public static explicit operator Lights5(LightArray Value){ return Value.Lights5; }
	public void Get(out Lights5 Value){Value=Lights5;}
	public void Set(ref Lights5 Value){Lights5=Value;}
	public static explicit operator LightArray(Lights6 Value){ return new LightArray { Lights6=Value, }; }
	public static explicit operator Lights6(LightArray Value){ return Value.Lights6; }
	public void Get(out Lights6 Value){Value=Lights6;}
	public void Set(ref Lights6 Value){Lights6=Value;}
	public static explicit operator LightArray(Lights7 Value){ return new LightArray { Lights7=Value, }; }
	public static explicit operator Lights7(LightArray Value){ return Value.Lights7; }
	public void Get(out Lights7 Value){Value=Lights7;}
	public void Set(ref Lights7 Value){Lights7=Value;}
	public Light this[int index]{
		get => 0==index?Light1:1==index?Light2:2==index?Light3:3==index?Light4:4==index?Light5:5==index?Light6:6==index?Light7:7==index?Light8:OutOfRangeLight();
		set {if(0==index)Light1=value;else if(1==index)Light2=value;else if(2==index)Light3=value;else if(3==index)Light4=value;else if(4==index)Light5=value;else if(5==index)Light6=value;else if(6==index)Light7=value;else if(7==index)Light8=value;else throw new System.ArgumentOutOfRangeException("index"); }
	}
	public uint this[int index, int word]{
		get => 0==index?Light1[word]:1==index?Light2[word]:2==index?Light3[word]:3==index?Light4[word]:4==index?Light5[word]:5==index?Light6[word]:6==index?Light7[word]:7==index?Light8[word]:OutOfRangeWord();
		set {if(0==index)Light1[word]=value;else if(1==index)Light2[word]=value;else if(2==index)Light3[word]=value;else if(3==index)Light4[word]=value;else if(4==index)Light5[word]=value;else if(5==index)Light6[word]=value;else if(6==index)Light7[word]=value;else if(7==index)Light8[word]=value;else throw new System.ArgumentOutOfRangeException("index"); }
	}
	public static bool Equals(ref LightArray L, ref LightArray R){
		return Light.Equals(ref L.Light1,ref R.Light1) &&
			Light.Equals(ref L.Light2,ref R.Light2) &&
			Light.Equals(ref L.Light3,ref R.Light3) &&
			Light.Equals(ref L.Light4,ref R.Light4) &&
			Light.Equals(ref L.Light5,ref R.Light5) &&
			Light.Equals(ref L.Light6,ref R.Light6) &&
			Light.Equals(ref L.Light7,ref R.Light7) &&
			Light.Equals(ref L.Light8,ref R.Light8);
	}
	public static bool Inequals(ref LightArray L, ref LightArray R){
		return Light.Inequals(ref L.Light1,ref R.Light1) ||
			Light.Inequals(ref L.Light2,ref R.Light2) ||
			Light.Inequals(ref L.Light3,ref R.Light3) ||
			Light.Inequals(ref L.Light4,ref R.Light4) ||
			Light.Inequals(ref L.Light5,ref R.Light5) ||
			Light.Inequals(ref L.Light6,ref R.Light6) ||
			Light.Inequals(ref L.Light7,ref R.Light7) ||
			Light.Inequals(ref L.Light8,ref R.Light8);
	}
	public static bool operator == (LightArray L, LightArray R){
		return Light.Equals(ref L.Light1,ref R.Light1) &&
			Light.Equals(ref L.Light2,ref R.Light2) &&
			Light.Equals(ref L.Light3,ref R.Light3) &&
			Light.Equals(ref L.Light4,ref R.Light4) &&
			Light.Equals(ref L.Light5,ref R.Light5) &&
			Light.Equals(ref L.Light6,ref R.Light6) &&
			Light.Equals(ref L.Light7,ref R.Light7) &&
			Light.Equals(ref L.Light8,ref R.Light8);
	}
	public static bool operator != (LightArray L, LightArray R){
		return Light.Inequals(ref L.Light1,ref R.Light1) ||
			Light.Inequals(ref L.Light2,ref R.Light2) ||
			Light.Inequals(ref L.Light3,ref R.Light3) ||
			Light.Inequals(ref L.Light4,ref R.Light4) ||
			Light.Inequals(ref L.Light5,ref R.Light5) ||
			Light.Inequals(ref L.Light6,ref R.Light6) ||
			Light.Inequals(ref L.Light7,ref R.Light7) ||
			Light.Inequals(ref L.Light8,ref R.Light8);
	}
	public bool Equals(LightArray other){return Equals(ref this, ref other);}
	public bool Equals(ref LightArray other){return Equals(ref this, ref other);}
	public override bool Equals(object obj){ return obj is LightArray && ((LightArray)obj).Equals(ref this); }
	public static int GetHashCode(ref LightArray Value){
		uint Temp;
		unchecked {
		return
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light1)))<<0))|(Temp>>0)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light2)))<<5))|(Temp>>27)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light3)))<<10))|(Temp>>22)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light4)))<<15))|(Temp>>17)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light5)))<<20))|(Temp>>12)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light6)))<<25))|(Temp>>7)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light7)))<<30))|(Temp>>2)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light8)))<<3))|(Temp>>29));
		}
	}
	public override int GetHashCode(){return GetHashCode(ref this);}
	public override string ToString(){return "LightArray"; }
}
	[StructLayout(LayoutKind.Explicit,Size=0x18)]
public partial struct Lights0 : System.IEquatable<Lights0>,
	LightContainer{

	private static Light OutOfRangeLight(){throw new System.ArgumentOutOfRangeException("index");}
	private static uint OutOfRangeWord(){throw new System.ArgumentOutOfRangeException("index");}

	[FieldOffset(0x00)] public Light Light1;
	[FieldOffset(0x20)] public Ambient Ambient;
	[FieldOffset(0x28)] public ulong AmbientPad;
	public const int Length = 0;
	public void Write(ref Light Value, byte Index){
		if(Index>=1)Ambient=Value.AsAmbient;
		else Light1=Value;
	}
	public void Get(out LightArray LightArray) {
		LightArray = new LightArray { Lights0 = this, };
	}
	public void Set(ref LightArray LightArray) {
		this = LightArray.Lights0;
	}
	Ambient LightContainer.Ambient {get => this.Ambient; set => this.Ambient = value; }
	int LightContainer.Length => Length;
	public Light this[int index]{
		get => OutOfRangeLight();
		set {throw new System.ArgumentOutOfRangeException("index"); }
	}
	public uint this[int index, int word]{
		get => 0==index?Ambient[word]:OutOfRangeWord();
		set {if(0==index)Ambient[word]=value;else throw new System.ArgumentOutOfRangeException("index"); }
	}
	public static bool Equals(ref Lights0 L, ref Lights0 R){
		return Ambient.Equals(ref L.Ambient,ref R.Ambient);
	}
	public static bool Inequals(ref Lights0 L, ref Lights0 R){
		return Ambient.Inequals(ref L.Ambient, ref R.Ambient);
	}
	public static bool operator == (Lights0 L, Lights0 R){
		return Ambient.Equals(ref L.Ambient,ref R.Ambient);
	}
	public static bool operator != (Lights0 L, Lights0 R){
		return Ambient.Inequals(ref L.Ambient, ref R.Ambient);
	}
	public bool Equals(Lights0 other){return Equals(ref this, ref other);}
	public bool Equals(ref Lights0 other){return Equals(ref this, ref other);}
	public override bool Equals(object obj){ return obj is Lights0 && ((Lights0)obj).Equals(ref this); }
	public static int GetHashCode(ref Lights0 Value){
		return
			Ambient.GetHashCode(ref Value.Ambient);
	}
	public override int GetHashCode(){return GetHashCode(ref this);}
	public override string ToString(){return "Lights0"; }
}
	[StructLayout(LayoutKind.Explicit,Size=0x18)]
public partial struct Lights1 : System.IEquatable<Lights1>,
	LightContainer{

	private static Light OutOfRangeLight(){throw new System.ArgumentOutOfRangeException("index");}
	private static uint OutOfRangeWord(){throw new System.ArgumentOutOfRangeException("index");}

	[FieldOffset(0x00)] public Light Light1;
	[FieldOffset(0x20)] public Ambient Ambient;
	[FieldOffset(0x28)] public ulong AmbientPad;
	public const int Length = 1;
	public void Write(ref Light Value, byte Index){
		if(Index>=1)Ambient=Value.AsAmbient;
		else Light1=Value;
	}
	public void Get(out LightArray LightArray) {
		LightArray = new LightArray { Lights1 = this, };
	}
	public void Set(ref LightArray LightArray) {
		this = LightArray.Lights1;
	}
	Ambient LightContainer.Ambient {get => this.Ambient; set => this.Ambient = value; }
	int LightContainer.Length => Length;
	public Light this[int index]{
		get => 0==index?Light1:OutOfRangeLight();
		set {if(0==index)Light1=value;else throw new System.ArgumentOutOfRangeException("index"); }
	}
	public uint this[int index, int word]{
		get => 0==index?Light1[word]:1==index?Ambient[word]:OutOfRangeWord();
		set {if(0==index)Light1[word]=value;else if(1==index)Ambient[word]=value;else throw new System.ArgumentOutOfRangeException("index"); }
	}
	public static bool Equals(ref Lights1 L, ref Lights1 R){
		return Ambient.Equals(ref L.Ambient,ref R.Ambient) &&
			Light.Equals(ref L.Light1,ref R.Light1);
	}
	public static bool Inequals(ref Lights1 L, ref Lights1 R){
		return Ambient.Inequals(ref L.Ambient, ref R.Ambient) ||
			Light.Inequals(ref L.Light1,ref R.Light1);
	}
	public static bool operator == (Lights1 L, Lights1 R){
		return Ambient.Equals(ref L.Ambient,ref R.Ambient) &&
			Light.Equals(ref L.Light1,ref R.Light1);
	}
	public static bool operator != (Lights1 L, Lights1 R){
		return Ambient.Inequals(ref L.Ambient, ref R.Ambient) ||
			Light.Inequals(ref L.Light1,ref R.Light1);
	}
	public bool Equals(Lights1 other){return Equals(ref this, ref other);}
	public bool Equals(ref Lights1 other){return Equals(ref this, ref other);}
	public override bool Equals(object obj){ return obj is Lights1 && ((Lights1)obj).Equals(ref this); }
	public static int GetHashCode(ref Lights1 Value){
		uint Temp;
		unchecked {
		return
			Ambient.GetHashCode(ref Value.Ambient) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light1)))<<0))|(Temp>>0));
		}
	}
	public override int GetHashCode(){return GetHashCode(ref this);}
	public override string ToString(){return "Lights1"; }
}
	[StructLayout(LayoutKind.Explicit,Size=0x28)]
public partial struct Lights2 : System.IEquatable<Lights2>,
	LightContainer{

	private static Light OutOfRangeLight(){throw new System.ArgumentOutOfRangeException("index");}
	private static uint OutOfRangeWord(){throw new System.ArgumentOutOfRangeException("index");}

	[FieldOffset(0x00)] public Light Light1;
	[FieldOffset(0x20)] public Light Light2;
	[FieldOffset(0x40)] public Ambient Ambient;
	[FieldOffset(0x48)] public ulong AmbientPad;
	public const int Length = 2;
	public void Write(ref Light Value, byte Index){
		if(Index>=2)Ambient=Value.AsAmbient;
		else if(1==Index) Light2=Value;
		else Light1=Value;
	}
	public void Get(out LightArray LightArray) {
		LightArray = new LightArray { Lights2 = this, };
	}
	public void Set(ref LightArray LightArray) {
		this = LightArray.Lights2;
	}
	Ambient LightContainer.Ambient {get => this.Ambient; set => this.Ambient = value; }
	int LightContainer.Length => Length;
	public Light this[int index]{
		get => 0==index?Light1:1==index?Light2:OutOfRangeLight();
		set {if(0==index)Light1=value;else if(1==index)Light2=value;else throw new System.ArgumentOutOfRangeException("index"); }
	}
	public uint this[int index, int word]{
		get => 0==index?Light1[word]:1==index?Light2[word]:2==index?Ambient[word]:OutOfRangeWord();
		set {if(0==index)Light1[word]=value;else if(1==index)Light2[word]=value;else if(2==index)Ambient[word]=value;else throw new System.ArgumentOutOfRangeException("index"); }
	}
	public static bool Equals(ref Lights2 L, ref Lights2 R){
		return Ambient.Equals(ref L.Ambient,ref R.Ambient) &&
			Light.Equals(ref L.Light1,ref R.Light1) &&
			Light.Equals(ref L.Light2,ref R.Light2);
	}
	public static bool Inequals(ref Lights2 L, ref Lights2 R){
		return Ambient.Inequals(ref L.Ambient, ref R.Ambient) ||
			Light.Inequals(ref L.Light1,ref R.Light1) ||
			Light.Inequals(ref L.Light2,ref R.Light2);
	}
	public static bool operator == (Lights2 L, Lights2 R){
		return Ambient.Equals(ref L.Ambient,ref R.Ambient) &&
			Light.Equals(ref L.Light1,ref R.Light1) &&
			Light.Equals(ref L.Light2,ref R.Light2);
	}
	public static bool operator != (Lights2 L, Lights2 R){
		return Ambient.Inequals(ref L.Ambient, ref R.Ambient) ||
			Light.Inequals(ref L.Light1,ref R.Light1) ||
			Light.Inequals(ref L.Light2,ref R.Light2);
	}
	public bool Equals(Lights2 other){return Equals(ref this, ref other);}
	public bool Equals(ref Lights2 other){return Equals(ref this, ref other);}
	public override bool Equals(object obj){ return obj is Lights2 && ((Lights2)obj).Equals(ref this); }
	public static int GetHashCode(ref Lights2 Value){
		uint Temp;
		unchecked {
		return
			Ambient.GetHashCode(ref Value.Ambient) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light1)))<<0))|(Temp>>0)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light2)))<<5))|(Temp>>27));
		}
	}
	public override int GetHashCode(){return GetHashCode(ref this);}
	public override string ToString(){return "Lights2"; }
}
	[StructLayout(LayoutKind.Explicit,Size=0x38)]
public partial struct Lights3 : System.IEquatable<Lights3>,
	LightContainer{

	private static Light OutOfRangeLight(){throw new System.ArgumentOutOfRangeException("index");}
	private static uint OutOfRangeWord(){throw new System.ArgumentOutOfRangeException("index");}

	[FieldOffset(0x00)] public Light Light1;
	[FieldOffset(0x20)] public Light Light2;
	[FieldOffset(0x40)] public Light Light3;
	[FieldOffset(0x60)] public Ambient Ambient;
	[FieldOffset(0x68)] public ulong AmbientPad;
	public const int Length = 3;
	public void Write(ref Light Value, byte Index){
		if(Index>=3)Ambient=Value.AsAmbient;
		else if(2==Index) Light3=Value;
		else if(1==Index) Light2=Value;
		else Light1=Value;
	}
	public void Get(out LightArray LightArray) {
		LightArray = new LightArray { Lights3 = this, };
	}
	public void Set(ref LightArray LightArray) {
		this = LightArray.Lights3;
	}
	Ambient LightContainer.Ambient {get => this.Ambient; set => this.Ambient = value; }
	int LightContainer.Length => Length;
	public Light this[int index]{
		get => 0==index?Light1:1==index?Light2:2==index?Light3:OutOfRangeLight();
		set {if(0==index)Light1=value;else if(1==index)Light2=value;else if(2==index)Light3=value;else throw new System.ArgumentOutOfRangeException("index"); }
	}
	public uint this[int index, int word]{
		get => 0==index?Light1[word]:1==index?Light2[word]:2==index?Light3[word]:3==index?Ambient[word]:OutOfRangeWord();
		set {if(0==index)Light1[word]=value;else if(1==index)Light2[word]=value;else if(2==index)Light3[word]=value;else if(3==index)Ambient[word]=value;else throw new System.ArgumentOutOfRangeException("index"); }
	}
	public static bool Equals(ref Lights3 L, ref Lights3 R){
		return Ambient.Equals(ref L.Ambient,ref R.Ambient) &&
			Light.Equals(ref L.Light1,ref R.Light1) &&
			Light.Equals(ref L.Light2,ref R.Light2) &&
			Light.Equals(ref L.Light3,ref R.Light3);
	}
	public static bool Inequals(ref Lights3 L, ref Lights3 R){
		return Ambient.Inequals(ref L.Ambient, ref R.Ambient) ||
			Light.Inequals(ref L.Light1,ref R.Light1) ||
			Light.Inequals(ref L.Light2,ref R.Light2) ||
			Light.Inequals(ref L.Light3,ref R.Light3);
	}
	public static bool operator == (Lights3 L, Lights3 R){
		return Ambient.Equals(ref L.Ambient,ref R.Ambient) &&
			Light.Equals(ref L.Light1,ref R.Light1) &&
			Light.Equals(ref L.Light2,ref R.Light2) &&
			Light.Equals(ref L.Light3,ref R.Light3);
	}
	public static bool operator != (Lights3 L, Lights3 R){
		return Ambient.Inequals(ref L.Ambient, ref R.Ambient) ||
			Light.Inequals(ref L.Light1,ref R.Light1) ||
			Light.Inequals(ref L.Light2,ref R.Light2) ||
			Light.Inequals(ref L.Light3,ref R.Light3);
	}
	public bool Equals(Lights3 other){return Equals(ref this, ref other);}
	public bool Equals(ref Lights3 other){return Equals(ref this, ref other);}
	public override bool Equals(object obj){ return obj is Lights3 && ((Lights3)obj).Equals(ref this); }
	public static int GetHashCode(ref Lights3 Value){
		uint Temp;
		unchecked {
		return
			Ambient.GetHashCode(ref Value.Ambient) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light1)))<<0))|(Temp>>0)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light2)))<<5))|(Temp>>27)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light3)))<<10))|(Temp>>22));
		}
	}
	public override int GetHashCode(){return GetHashCode(ref this);}
	public override string ToString(){return "Lights3"; }
}
	[StructLayout(LayoutKind.Explicit,Size=0x48)]
public partial struct Lights4 : System.IEquatable<Lights4>,
	LightContainer{

	private static Light OutOfRangeLight(){throw new System.ArgumentOutOfRangeException("index");}
	private static uint OutOfRangeWord(){throw new System.ArgumentOutOfRangeException("index");}

	[FieldOffset(0x00)] public Light Light1;
	[FieldOffset(0x20)] public Light Light2;
	[FieldOffset(0x40)] public Light Light3;
	[FieldOffset(0x60)] public Light Light4;
	[FieldOffset(0x80)] public Ambient Ambient;
	[FieldOffset(0x88)] public ulong AmbientPad;
	public const int Length = 4;
	public void Write(ref Light Value, byte Index){
		if(Index>=4)Ambient=Value.AsAmbient;
		else if(3==Index) Light4=Value;
		else if(2==Index) Light3=Value;
		else if(1==Index) Light2=Value;
		else Light1=Value;
	}
	public void Get(out LightArray LightArray) {
		LightArray = new LightArray { Lights4 = this, };
	}
	public void Set(ref LightArray LightArray) {
		this = LightArray.Lights4;
	}
	Ambient LightContainer.Ambient {get => this.Ambient; set => this.Ambient = value; }
	int LightContainer.Length => Length;
	public Light this[int index]{
		get => 0==index?Light1:1==index?Light2:2==index?Light3:3==index?Light4:OutOfRangeLight();
		set {if(0==index)Light1=value;else if(1==index)Light2=value;else if(2==index)Light3=value;else if(3==index)Light4=value;else throw new System.ArgumentOutOfRangeException("index"); }
	}
	public uint this[int index, int word]{
		get => 0==index?Light1[word]:1==index?Light2[word]:2==index?Light3[word]:3==index?Light4[word]:4==index?Ambient[word]:OutOfRangeWord();
		set {if(0==index)Light1[word]=value;else if(1==index)Light2[word]=value;else if(2==index)Light3[word]=value;else if(3==index)Light4[word]=value;else if(4==index)Ambient[word]=value;else throw new System.ArgumentOutOfRangeException("index"); }
	}
	public static bool Equals(ref Lights4 L, ref Lights4 R){
		return Ambient.Equals(ref L.Ambient,ref R.Ambient) &&
			Light.Equals(ref L.Light1,ref R.Light1) &&
			Light.Equals(ref L.Light2,ref R.Light2) &&
			Light.Equals(ref L.Light3,ref R.Light3) &&
			Light.Equals(ref L.Light4,ref R.Light4);
	}
	public static bool Inequals(ref Lights4 L, ref Lights4 R){
		return Ambient.Inequals(ref L.Ambient, ref R.Ambient) ||
			Light.Inequals(ref L.Light1,ref R.Light1) ||
			Light.Inequals(ref L.Light2,ref R.Light2) ||
			Light.Inequals(ref L.Light3,ref R.Light3) ||
			Light.Inequals(ref L.Light4,ref R.Light4);
	}
	public static bool operator == (Lights4 L, Lights4 R){
		return Ambient.Equals(ref L.Ambient,ref R.Ambient) &&
			Light.Equals(ref L.Light1,ref R.Light1) &&
			Light.Equals(ref L.Light2,ref R.Light2) &&
			Light.Equals(ref L.Light3,ref R.Light3) &&
			Light.Equals(ref L.Light4,ref R.Light4);
	}
	public static bool operator != (Lights4 L, Lights4 R){
		return Ambient.Inequals(ref L.Ambient, ref R.Ambient) ||
			Light.Inequals(ref L.Light1,ref R.Light1) ||
			Light.Inequals(ref L.Light2,ref R.Light2) ||
			Light.Inequals(ref L.Light3,ref R.Light3) ||
			Light.Inequals(ref L.Light4,ref R.Light4);
	}
	public bool Equals(Lights4 other){return Equals(ref this, ref other);}
	public bool Equals(ref Lights4 other){return Equals(ref this, ref other);}
	public override bool Equals(object obj){ return obj is Lights4 && ((Lights4)obj).Equals(ref this); }
	public static int GetHashCode(ref Lights4 Value){
		uint Temp;
		unchecked {
		return
			Ambient.GetHashCode(ref Value.Ambient) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light1)))<<0))|(Temp>>0)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light2)))<<5))|(Temp>>27)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light3)))<<10))|(Temp>>22)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light4)))<<15))|(Temp>>17));
		}
	}
	public override int GetHashCode(){return GetHashCode(ref this);}
	public override string ToString(){return "Lights4"; }
}
	[StructLayout(LayoutKind.Explicit,Size=0x58)]
public partial struct Lights5 : System.IEquatable<Lights5>,
	LightContainer{

	private static Light OutOfRangeLight(){throw new System.ArgumentOutOfRangeException("index");}
	private static uint OutOfRangeWord(){throw new System.ArgumentOutOfRangeException("index");}

	[FieldOffset(0x00)] public Light Light1;
	[FieldOffset(0x20)] public Light Light2;
	[FieldOffset(0x40)] public Light Light3;
	[FieldOffset(0x60)] public Light Light4;
	[FieldOffset(0x80)] public Light Light5;
	[FieldOffset(0xA0)] public Ambient Ambient;
	[FieldOffset(0xA8)] public ulong AmbientPad;
	public const int Length = 5;
	public void Write(ref Light Value, byte Index){
		if(Index>=5)Ambient=Value.AsAmbient;
		else if(4==Index) Light5=Value;
		else if(3==Index) Light4=Value;
		else if(2==Index) Light3=Value;
		else if(1==Index) Light2=Value;
		else Light1=Value;
	}
	public void Get(out LightArray LightArray) {
		LightArray = new LightArray { Lights5 = this, };
	}
	public void Set(ref LightArray LightArray) {
		this = LightArray.Lights5;
	}
	Ambient LightContainer.Ambient {get => this.Ambient; set => this.Ambient = value; }
	int LightContainer.Length => Length;
	public Light this[int index]{
		get => 0==index?Light1:1==index?Light2:2==index?Light3:3==index?Light4:4==index?Light5:OutOfRangeLight();
		set {if(0==index)Light1=value;else if(1==index)Light2=value;else if(2==index)Light3=value;else if(3==index)Light4=value;else if(4==index)Light5=value;else throw new System.ArgumentOutOfRangeException("index"); }
	}
	public uint this[int index, int word]{
		get => 0==index?Light1[word]:1==index?Light2[word]:2==index?Light3[word]:3==index?Light4[word]:4==index?Light5[word]:5==index?Ambient[word]:OutOfRangeWord();
		set {if(0==index)Light1[word]=value;else if(1==index)Light2[word]=value;else if(2==index)Light3[word]=value;else if(3==index)Light4[word]=value;else if(4==index)Light5[word]=value;else if(5==index)Ambient[word]=value;else throw new System.ArgumentOutOfRangeException("index"); }
	}
	public static bool Equals(ref Lights5 L, ref Lights5 R){
		return Ambient.Equals(ref L.Ambient,ref R.Ambient) &&
			Light.Equals(ref L.Light1,ref R.Light1) &&
			Light.Equals(ref L.Light2,ref R.Light2) &&
			Light.Equals(ref L.Light3,ref R.Light3) &&
			Light.Equals(ref L.Light4,ref R.Light4) &&
			Light.Equals(ref L.Light5,ref R.Light5);
	}
	public static bool Inequals(ref Lights5 L, ref Lights5 R){
		return Ambient.Inequals(ref L.Ambient, ref R.Ambient) ||
			Light.Inequals(ref L.Light1,ref R.Light1) ||
			Light.Inequals(ref L.Light2,ref R.Light2) ||
			Light.Inequals(ref L.Light3,ref R.Light3) ||
			Light.Inequals(ref L.Light4,ref R.Light4) ||
			Light.Inequals(ref L.Light5,ref R.Light5);
	}
	public static bool operator == (Lights5 L, Lights5 R){
		return Ambient.Equals(ref L.Ambient,ref R.Ambient) &&
			Light.Equals(ref L.Light1,ref R.Light1) &&
			Light.Equals(ref L.Light2,ref R.Light2) &&
			Light.Equals(ref L.Light3,ref R.Light3) &&
			Light.Equals(ref L.Light4,ref R.Light4) &&
			Light.Equals(ref L.Light5,ref R.Light5);
	}
	public static bool operator != (Lights5 L, Lights5 R){
		return Ambient.Inequals(ref L.Ambient, ref R.Ambient) ||
			Light.Inequals(ref L.Light1,ref R.Light1) ||
			Light.Inequals(ref L.Light2,ref R.Light2) ||
			Light.Inequals(ref L.Light3,ref R.Light3) ||
			Light.Inequals(ref L.Light4,ref R.Light4) ||
			Light.Inequals(ref L.Light5,ref R.Light5);
	}
	public bool Equals(Lights5 other){return Equals(ref this, ref other);}
	public bool Equals(ref Lights5 other){return Equals(ref this, ref other);}
	public override bool Equals(object obj){ return obj is Lights5 && ((Lights5)obj).Equals(ref this); }
	public static int GetHashCode(ref Lights5 Value){
		uint Temp;
		unchecked {
		return
			Ambient.GetHashCode(ref Value.Ambient) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light1)))<<0))|(Temp>>0)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light2)))<<5))|(Temp>>27)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light3)))<<10))|(Temp>>22)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light4)))<<15))|(Temp>>17)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light5)))<<20))|(Temp>>12));
		}
	}
	public override int GetHashCode(){return GetHashCode(ref this);}
	public override string ToString(){return "Lights5"; }
}
	[StructLayout(LayoutKind.Explicit,Size=0x68)]
public partial struct Lights6 : System.IEquatable<Lights6>,
	LightContainer{

	private static Light OutOfRangeLight(){throw new System.ArgumentOutOfRangeException("index");}
	private static uint OutOfRangeWord(){throw new System.ArgumentOutOfRangeException("index");}

	[FieldOffset(0x00)] public Light Light1;
	[FieldOffset(0x20)] public Light Light2;
	[FieldOffset(0x40)] public Light Light3;
	[FieldOffset(0x60)] public Light Light4;
	[FieldOffset(0x80)] public Light Light5;
	[FieldOffset(0xA0)] public Light Light6;
	[FieldOffset(0xC0)] public Ambient Ambient;
	[FieldOffset(0xC8)] public ulong AmbientPad;
	public const int Length = 6;
	public void Write(ref Light Value, byte Index){
		if(Index>=6)Ambient=Value.AsAmbient;
		else if(5==Index) Light6=Value;
		else if(4==Index) Light5=Value;
		else if(3==Index) Light4=Value;
		else if(2==Index) Light3=Value;
		else if(1==Index) Light2=Value;
		else Light1=Value;
	}
	public void Get(out LightArray LightArray) {
		LightArray = new LightArray { Lights6 = this, };
	}
	public void Set(ref LightArray LightArray) {
		this = LightArray.Lights6;
	}
	Ambient LightContainer.Ambient {get => this.Ambient; set => this.Ambient = value; }
	int LightContainer.Length => Length;
	public Light this[int index]{
		get => 0==index?Light1:1==index?Light2:2==index?Light3:3==index?Light4:4==index?Light5:5==index?Light6:OutOfRangeLight();
		set {if(0==index)Light1=value;else if(1==index)Light2=value;else if(2==index)Light3=value;else if(3==index)Light4=value;else if(4==index)Light5=value;else if(5==index)Light6=value;else throw new System.ArgumentOutOfRangeException("index"); }
	}
	public uint this[int index, int word]{
		get => 0==index?Light1[word]:1==index?Light2[word]:2==index?Light3[word]:3==index?Light4[word]:4==index?Light5[word]:5==index?Light6[word]:6==index?Ambient[word]:OutOfRangeWord();
		set {if(0==index)Light1[word]=value;else if(1==index)Light2[word]=value;else if(2==index)Light3[word]=value;else if(3==index)Light4[word]=value;else if(4==index)Light5[word]=value;else if(5==index)Light6[word]=value;else if(6==index)Ambient[word]=value;else throw new System.ArgumentOutOfRangeException("index"); }
	}
	public static bool Equals(ref Lights6 L, ref Lights6 R){
		return Ambient.Equals(ref L.Ambient,ref R.Ambient) &&
			Light.Equals(ref L.Light1,ref R.Light1) &&
			Light.Equals(ref L.Light2,ref R.Light2) &&
			Light.Equals(ref L.Light3,ref R.Light3) &&
			Light.Equals(ref L.Light4,ref R.Light4) &&
			Light.Equals(ref L.Light5,ref R.Light5) &&
			Light.Equals(ref L.Light6,ref R.Light6);
	}
	public static bool Inequals(ref Lights6 L, ref Lights6 R){
		return Ambient.Inequals(ref L.Ambient, ref R.Ambient) ||
			Light.Inequals(ref L.Light1,ref R.Light1) ||
			Light.Inequals(ref L.Light2,ref R.Light2) ||
			Light.Inequals(ref L.Light3,ref R.Light3) ||
			Light.Inequals(ref L.Light4,ref R.Light4) ||
			Light.Inequals(ref L.Light5,ref R.Light5) ||
			Light.Inequals(ref L.Light6,ref R.Light6);
	}
	public static bool operator == (Lights6 L, Lights6 R){
		return Ambient.Equals(ref L.Ambient,ref R.Ambient) &&
			Light.Equals(ref L.Light1,ref R.Light1) &&
			Light.Equals(ref L.Light2,ref R.Light2) &&
			Light.Equals(ref L.Light3,ref R.Light3) &&
			Light.Equals(ref L.Light4,ref R.Light4) &&
			Light.Equals(ref L.Light5,ref R.Light5) &&
			Light.Equals(ref L.Light6,ref R.Light6);
	}
	public static bool operator != (Lights6 L, Lights6 R){
		return Ambient.Inequals(ref L.Ambient, ref R.Ambient) ||
			Light.Inequals(ref L.Light1,ref R.Light1) ||
			Light.Inequals(ref L.Light2,ref R.Light2) ||
			Light.Inequals(ref L.Light3,ref R.Light3) ||
			Light.Inequals(ref L.Light4,ref R.Light4) ||
			Light.Inequals(ref L.Light5,ref R.Light5) ||
			Light.Inequals(ref L.Light6,ref R.Light6);
	}
	public bool Equals(Lights6 other){return Equals(ref this, ref other);}
	public bool Equals(ref Lights6 other){return Equals(ref this, ref other);}
	public override bool Equals(object obj){ return obj is Lights6 && ((Lights6)obj).Equals(ref this); }
	public static int GetHashCode(ref Lights6 Value){
		uint Temp;
		unchecked {
		return
			Ambient.GetHashCode(ref Value.Ambient) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light1)))<<0))|(Temp>>0)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light2)))<<5))|(Temp>>27)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light3)))<<10))|(Temp>>22)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light4)))<<15))|(Temp>>17)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light5)))<<20))|(Temp>>12)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light6)))<<25))|(Temp>>7));
		}
	}
	public override int GetHashCode(){return GetHashCode(ref this);}
	public override string ToString(){return "Lights6"; }
}
	[StructLayout(LayoutKind.Explicit,Size=0x78)]
public partial struct Lights7 : System.IEquatable<Lights7>,
	LightContainer{

	private static Light OutOfRangeLight(){throw new System.ArgumentOutOfRangeException("index");}
	private static uint OutOfRangeWord(){throw new System.ArgumentOutOfRangeException("index");}

	[FieldOffset(0x00)] public Light Light1;
	[FieldOffset(0x20)] public Light Light2;
	[FieldOffset(0x40)] public Light Light3;
	[FieldOffset(0x60)] public Light Light4;
	[FieldOffset(0x80)] public Light Light5;
	[FieldOffset(0xA0)] public Light Light6;
	[FieldOffset(0xC0)] public Light Light7;
	[FieldOffset(0xE0)] public Ambient Ambient;
	[FieldOffset(0xE8)] public ulong AmbientPad;
	public const int Length = 7;
	public void Write(ref Light Value, byte Index){
		if(Index>=7)Ambient=Value.AsAmbient;
		else if(6==Index) Light7=Value;
		else if(5==Index) Light6=Value;
		else if(4==Index) Light5=Value;
		else if(3==Index) Light4=Value;
		else if(2==Index) Light3=Value;
		else if(1==Index) Light2=Value;
		else Light1=Value;
	}
	public void Get(out LightArray LightArray) {
		LightArray = new LightArray { Lights7 = this, };
	}
	public void Set(ref LightArray LightArray) {
		this = LightArray.Lights7;
	}
	Ambient LightContainer.Ambient {get => this.Ambient; set => this.Ambient = value; }
	int LightContainer.Length => Length;
	public Light this[int index]{
		get => 0==index?Light1:1==index?Light2:2==index?Light3:3==index?Light4:4==index?Light5:5==index?Light6:6==index?Light7:OutOfRangeLight();
		set {if(0==index)Light1=value;else if(1==index)Light2=value;else if(2==index)Light3=value;else if(3==index)Light4=value;else if(4==index)Light5=value;else if(5==index)Light6=value;else if(6==index)Light7=value;else throw new System.ArgumentOutOfRangeException("index"); }
	}
	public uint this[int index, int word]{
		get => 0==index?Light1[word]:1==index?Light2[word]:2==index?Light3[word]:3==index?Light4[word]:4==index?Light5[word]:5==index?Light6[word]:6==index?Light7[word]:7==index?Ambient[word]:OutOfRangeWord();
		set {if(0==index)Light1[word]=value;else if(1==index)Light2[word]=value;else if(2==index)Light3[word]=value;else if(3==index)Light4[word]=value;else if(4==index)Light5[word]=value;else if(5==index)Light6[word]=value;else if(6==index)Light7[word]=value;else if(7==index)Ambient[word]=value;else throw new System.ArgumentOutOfRangeException("index"); }
	}
	public static bool Equals(ref Lights7 L, ref Lights7 R){
		return Ambient.Equals(ref L.Ambient,ref R.Ambient) &&
			Light.Equals(ref L.Light1,ref R.Light1) &&
			Light.Equals(ref L.Light2,ref R.Light2) &&
			Light.Equals(ref L.Light3,ref R.Light3) &&
			Light.Equals(ref L.Light4,ref R.Light4) &&
			Light.Equals(ref L.Light5,ref R.Light5) &&
			Light.Equals(ref L.Light6,ref R.Light6) &&
			Light.Equals(ref L.Light7,ref R.Light7);
	}
	public static bool Inequals(ref Lights7 L, ref Lights7 R){
		return Ambient.Inequals(ref L.Ambient, ref R.Ambient) ||
			Light.Inequals(ref L.Light1,ref R.Light1) ||
			Light.Inequals(ref L.Light2,ref R.Light2) ||
			Light.Inequals(ref L.Light3,ref R.Light3) ||
			Light.Inequals(ref L.Light4,ref R.Light4) ||
			Light.Inequals(ref L.Light5,ref R.Light5) ||
			Light.Inequals(ref L.Light6,ref R.Light6) ||
			Light.Inequals(ref L.Light7,ref R.Light7);
	}
	public static bool operator == (Lights7 L, Lights7 R){
		return Ambient.Equals(ref L.Ambient,ref R.Ambient) &&
			Light.Equals(ref L.Light1,ref R.Light1) &&
			Light.Equals(ref L.Light2,ref R.Light2) &&
			Light.Equals(ref L.Light3,ref R.Light3) &&
			Light.Equals(ref L.Light4,ref R.Light4) &&
			Light.Equals(ref L.Light5,ref R.Light5) &&
			Light.Equals(ref L.Light6,ref R.Light6) &&
			Light.Equals(ref L.Light7,ref R.Light7);
	}
	public static bool operator != (Lights7 L, Lights7 R){
		return Ambient.Inequals(ref L.Ambient, ref R.Ambient) ||
			Light.Inequals(ref L.Light1,ref R.Light1) ||
			Light.Inequals(ref L.Light2,ref R.Light2) ||
			Light.Inequals(ref L.Light3,ref R.Light3) ||
			Light.Inequals(ref L.Light4,ref R.Light4) ||
			Light.Inequals(ref L.Light5,ref R.Light5) ||
			Light.Inequals(ref L.Light6,ref R.Light6) ||
			Light.Inequals(ref L.Light7,ref R.Light7);
	}
	public bool Equals(Lights7 other){return Equals(ref this, ref other);}
	public bool Equals(ref Lights7 other){return Equals(ref this, ref other);}
	public override bool Equals(object obj){ return obj is Lights7 && ((Lights7)obj).Equals(ref this); }
	public static int GetHashCode(ref Lights7 Value){
		uint Temp;
		unchecked {
		return
			Ambient.GetHashCode(ref Value.Ambient) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light1)))<<0))|(Temp>>0)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light2)))<<5))|(Temp>>27)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light3)))<<10))|(Temp>>22)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light4)))<<15))|(Temp>>17)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light5)))<<20))|(Temp>>12)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light6)))<<25))|(Temp>>7)) ^
			(int)(((((Temp=(uint)Light.GetHashCode(ref Value.Light7)))<<30))|(Temp>>2));
		}
	}
	public override int GetHashCode(){return GetHashCode(ref this);}
	public override string ToString(){return "Lights7"; }
}
}
