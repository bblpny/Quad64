using System;
using System.Runtime.InteropServices;
using OpenTK;
namespace Quad64{
	///<summary>A 4 dimension vector with elements of type sbyte.</summary>
	[StructLayout(LayoutKind.Explicit, Size = sizeof(uint))]
	public partial struct Vector4c
		: IEquatable<Vector4c>, IEquatable<Vector3c>
	{
		[FieldOffset(sizeof(sbyte) * 3)]
		public sbyte X;
		[FieldOffset(sizeof(sbyte) * 2)]
		public sbyte Y;
		[FieldOffset(sizeof(sbyte) * 1)]
		public sbyte Z;
		[FieldOffset(sizeof(sbyte) * 0)]
		public sbyte W;
		[FieldOffset(0)]
		public uint Packed;
		public sbyte this[int axis]{
			get	{
				return 0 == (axis & 2) ? 0 == (axis & 1) ? X : Y : 0 == (axis & 1) ? Z : W;
			}
			set	{
				if (0 == (axis & 2))
					if (0 == (axis & 1))
						X = value;
					else
						Y = value;
				else
					if (0 == (axis & 1))
					Z = value;
				else
					W = value;
			}
		}
		
		public bool Equals(Vector3c other) {
			return W == 0 && X == other.X && Y == other.Y && Z == other.Z;
		}
		public bool Equals(Vector4c other) {
			return Packed == other.Packed;
		}
		public override string ToString(){
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			var Zs = X == Z ? Xs : Y == Z ? Ys : Z == 0 ? "0" : Z.ToString();
			var Ws = X == W ? Xs : Y == W ? Ys : Z == W ? Zs : W == 0 ? "0" : W.ToString();

			return string.Concat("[",
				string.Concat(Xs, ",", Ys, ","),
				string.Concat(Zs, ",", Ws, "]"));
		}
		public static bool operator ==(Vector4c L, Vector4c R) { return L.Packed == R.Packed; }
		public static bool operator !=(Vector4c L, Vector4c R) { return L.Packed != R.Packed; }
		public static bool operator ==(Vector4c L, uint R) { return L.Packed == R; }
		public static bool operator !=(Vector4c L, uint R) { return L.Packed != R; }
		public static bool operator ==(uint L, Vector4c R) { return L == R.Packed; }
		public static bool operator !=(uint L, Vector4c R) { return L != R.Packed; }

		public static Vector4c operator &(Vector4c L, Vector4c R) { return new Vector4c { Packed = (uint)(L.Packed & R.Packed), }; }
		public static Vector4c operator |(Vector4c L, Vector4c R) { return new Vector4c { Packed = (uint)(L.Packed | R.Packed), }; }
		public static Vector4c operator ^(Vector4c L, Vector4c R) { return new Vector4c { Packed = (uint)(L.Packed ^ R.Packed), }; }
		public static bool operator true(Vector4c criteria) { return criteria.Packed != 0; }
		public static bool operator false(Vector4c criteria) { return 0 == criteria.Packed; }
		public static Vector4c operator +(Vector4c L, Vector4c R)
		{
			unchecked
			{
				return new Vector4c {
					X = (sbyte)(L.X + R.X),
					Y = (sbyte)(L.Y + R.Y),
					Z = (sbyte)(L.Z + R.Z),
					W = (sbyte)(L.W + R.W),
				};
			}
		}
		public static Vector4c operator +(Vector4c L, sbyte R)
		{
			unchecked
			{
				return new Vector4c {
					X = (sbyte)(L.X + R),
					Y = (sbyte)(L.Y + R),
					Z = (sbyte)(L.Z + R),
					W = (sbyte)(L.W + R),
				};
			}
		}
		public static Vector4c operator +(sbyte L, Vector4c R)
		{
			unchecked
			{
				return new Vector4c {
					X = (sbyte)(L + R.X),
					Y = (sbyte)(L + R.Y),
					Z = (sbyte)(L + R.Z),
					W = (sbyte)(L + R.W),
				};
			}
		}
		public static Vector4d operator +(Vector4c L, double R){
			return new Vector4d{
				X = (L.X + R),
				Y = (L.Y + R),
				Z = (L.Z + R),
				W = (L.W + R),
			};
		}
		public static Vector4 operator +(Vector4c L, float R){
			return new Vector4{
				X = (L.X + R),
				Y = (L.Y + R),
				Z = (L.Z + R),
				W = (L.W + R),
			};
		}
		public static Vector4d operator +(double L, Vector4c R){
			return new Vector4d{
				X = (L + R.X),
				Y = (L + R.Y),
				Z = (L + R.Z),
				W = (L + R.W),
			};
		}
		public static Vector4 operator +(float L, Vector4c R){
			return new Vector4{
				X = (L + R.X),
				Y = (L + R.Y),
				Z = (L + R.Z),
				W = (L + R.W),
			};
		}
		public static Vector4 operator +(Vector4 L, Vector4c R){
			return new Vector4{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
				W = (L.W + R.W),
			};
		}
		public static Vector4d operator +(Vector4d L, Vector4c R){
			return new Vector4d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
				W = (L.W + R.W),
			};
		}
		public static Vector4 operator +(Vector4c L, Vector4 R){
			return new Vector4{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
				W = (L.W + R.W),
			};
		}
		public static Vector4d operator +(Vector4c L, Vector4d R){
			return new Vector4d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
				W = (L.W + R.W),
			};
		}
		public static Vector4c operator -(Vector4c L, Vector4c R)
		{
			unchecked
			{
				return new Vector4c {
					X = (sbyte)(L.X - R.X),
					Y = (sbyte)(L.Y - R.Y),
					Z = (sbyte)(L.Z - R.Z),
					W = (sbyte)(L.W - R.W),
				};
			}
		}
		public static Vector4c operator -(Vector4c L, sbyte R)
		{
			unchecked
			{
				return new Vector4c {
					X = (sbyte)(L.X - R),
					Y = (sbyte)(L.Y - R),
					Z = (sbyte)(L.Z - R),
					W = (sbyte)(L.W - R),
				};
			}
		}
		public static Vector4c operator -(sbyte L, Vector4c R)
		{
			unchecked
			{
				return new Vector4c {
					X = (sbyte)(L - R.X),
					Y = (sbyte)(L - R.Y),
					Z = (sbyte)(L - R.Z),
					W = (sbyte)(L - R.W),
				};
			}
		}
		public static Vector4d operator -(Vector4c L, double R){
			return new Vector4d{
				X = (L.X - R),
				Y = (L.Y - R),
				Z = (L.Z - R),
				W = (L.W - R),
			};
		}
		public static Vector4 operator -(Vector4c L, float R){
			return new Vector4{
				X = (L.X - R),
				Y = (L.Y - R),
				Z = (L.Z - R),
				W = (L.W - R),
			};
		}
		public static Vector4d operator -(double L, Vector4c R){
			return new Vector4d{
				X = (L - R.X),
				Y = (L - R.Y),
				Z = (L - R.Z),
				W = (L - R.W),
			};
		}
		public static Vector4 operator -(float L, Vector4c R){
			return new Vector4{
				X = (L - R.X),
				Y = (L - R.Y),
				Z = (L - R.Z),
				W = (L - R.W),
			};
		}
		public static Vector4 operator -(Vector4 L, Vector4c R){
			return new Vector4{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
				W = (L.W - R.W),
			};
		}
		public static Vector4d operator -(Vector4d L, Vector4c R){
			return new Vector4d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
				W = (L.W - R.W),
			};
		}
		public static Vector4 operator -(Vector4c L, Vector4 R){
			return new Vector4{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
				W = (L.W - R.W),
			};
		}
		public static Vector4d operator -(Vector4c L, Vector4d R){
			return new Vector4d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
				W = (L.W - R.W),
			};
		}
		public static Vector4c operator *(Vector4c L, Vector4c R)
		{
			unchecked
			{
				return new Vector4c {
					X = (sbyte)(L.X * R.X),
					Y = (sbyte)(L.Y * R.Y),
					Z = (sbyte)(L.Z * R.Z),
					W = (sbyte)(L.W * R.W),
				};
			}
		}
		public static Vector4c operator *(Vector4c L, sbyte R)
		{
			unchecked
			{
				return new Vector4c {
					X = (sbyte)(L.X * R),
					Y = (sbyte)(L.Y * R),
					Z = (sbyte)(L.Z * R),
					W = (sbyte)(L.W * R),
				};
			}
		}
		public static Vector4c operator *(sbyte L, Vector4c R)
		{
			unchecked
			{
				return new Vector4c {
					X = (sbyte)(L * R.X),
					Y = (sbyte)(L * R.Y),
					Z = (sbyte)(L * R.Z),
					W = (sbyte)(L * R.W),
				};
			}
		}
		public static Vector4d operator *(Vector4c L, double R){
			return new Vector4d{
				X = (L.X * R),
				Y = (L.Y * R),
				Z = (L.Z * R),
				W = (L.W * R),
			};
		}
		public static Vector4 operator *(Vector4c L, float R){
			return new Vector4{
				X = (L.X * R),
				Y = (L.Y * R),
				Z = (L.Z * R),
				W = (L.W * R),
			};
		}
		public static Vector4d operator *(double L, Vector4c R){
			return new Vector4d{
				X = (L * R.X),
				Y = (L * R.Y),
				Z = (L * R.Z),
				W = (L * R.W),
			};
		}
		public static Vector4 operator *(float L, Vector4c R){
			return new Vector4{
				X = (L * R.X),
				Y = (L * R.Y),
				Z = (L * R.Z),
				W = (L * R.W),
			};
		}
		public static Vector4 operator *(Vector4 L, Vector4c R){
			return new Vector4{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
				W = (L.W * R.W),
			};
		}
		public static Vector4d operator *(Vector4d L, Vector4c R){
			return new Vector4d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
				W = (L.W * R.W),
			};
		}
		public static Vector4 operator *(Vector4c L, Vector4 R){
			return new Vector4{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
				W = (L.W * R.W),
			};
		}
		public static Vector4d operator *(Vector4c L, Vector4d R){
			return new Vector4d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
				W = (L.W * R.W),
			};
		}
		public static Vector4c operator /(Vector4c L, Vector4c R)
		{
			unchecked
			{
				return new Vector4c {
					X = (sbyte)(L.X / R.X),
					Y = (sbyte)(L.Y / R.Y),
					Z = (sbyte)(L.Z / R.Z),
					W = (sbyte)(L.W / R.W),
				};
			}
		}
		public static Vector4c operator /(Vector4c L, sbyte R)
		{
			unchecked
			{
				return new Vector4c {
					X = (sbyte)(L.X / R),
					Y = (sbyte)(L.Y / R),
					Z = (sbyte)(L.Z / R),
					W = (sbyte)(L.W / R),
				};
			}
		}
		public static Vector4c operator /(sbyte L, Vector4c R)
		{
			unchecked
			{
				return new Vector4c {
					X = (sbyte)(L / R.X),
					Y = (sbyte)(L / R.Y),
					Z = (sbyte)(L / R.Z),
					W = (sbyte)(L / R.W),
				};
			}
		}
		public static Vector4d operator /(Vector4c L, double R){
			return new Vector4d{
				X = (L.X / R),
				Y = (L.Y / R),
				Z = (L.Z / R),
				W = (L.W / R),
			};
		}
		public static Vector4 operator /(Vector4c L, float R){
			return new Vector4{
				X = (L.X / R),
				Y = (L.Y / R),
				Z = (L.Z / R),
				W = (L.W / R),
			};
		}
		public static Vector4d operator /(double L, Vector4c R){
			return new Vector4d{
				X = (L / R.X),
				Y = (L / R.Y),
				Z = (L / R.Z),
				W = (L / R.W),
			};
		}
		public static Vector4 operator /(float L, Vector4c R){
			return new Vector4{
				X = (L / R.X),
				Y = (L / R.Y),
				Z = (L / R.Z),
				W = (L / R.W),
			};
		}
		public static Vector4 operator /(Vector4 L, Vector4c R){
			return new Vector4{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
				W = (L.W / R.W),
			};
		}
		public static Vector4d operator /(Vector4d L, Vector4c R){
			return new Vector4d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
				W = (L.W / R.W),
			};
		}
		public static Vector4 operator /(Vector4c L, Vector4 R){
			return new Vector4{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
				W = (L.W / R.W),
			};
		}
		public static Vector4d operator /(Vector4c L, Vector4d R){
			return new Vector4d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
				W = (L.W / R.W),
			};
		}
		public static explicit operator Vector4(Vector4c value)
		{
			return new Vector4{X = value.X,Y = value.Y,Z = value.Z,W = value.W,};
		}
		public static explicit operator Vector4d(Vector4c value)
		{
			return new Vector4d{X = value.X,Y = value.Y,Z = value.Z,W = value.W,};
		}
		public static explicit operator Vector4c(Vector4 value)
		{
			return new Vector4c{X = (sbyte)value.X,Y = (sbyte)value.Y,Z = (sbyte)value.Z,W = (sbyte)value.W,};
		}
		public static explicit operator Vector4c(Vector4d value)
		{
			return new Vector4c{X = (sbyte)value.X,Y = (sbyte)value.Y,Z = (sbyte)value.Z,W = (sbyte)value.W,};
		}
		public override int GetHashCode(){return GetHashCode(ref this);}
		public static int GetHashCode(Vector4c Value){ 
			// IF YOU GET AN ERROR ON THE LINE BELOW, ITS BECAUSE YOU DID NOT IMPLEMENT
			// IT OUTSIDE OF THIS PARTIAL GENERATED CODE.
			//
			// NOTE THAT THE ARGUMENT MUST BE NAMED VALUE!
			return GetHashCode(Value:ref Value);
		}
		public static Vector4c operator -(Vector4c v) {
			unchecked { 
				return new Vector4c {
					X= (sbyte)(-v.X),
					Y= (sbyte)(-v.X),
					Z= (sbyte)(-v.X),
					W= (sbyte)(-v.X),
				};
			}
		}
		public override bool Equals(object obj)
		{
			return obj is Vector4c && Packed == ((Vector4c)obj).Packed;
		}
		public static explicit operator Vector4c(uint value) {
			return new Vector4c { Packed = value, };
		}
	}
	///<summary>A 3 dimension vector with elements of type sbyte.</summary>
	[StructLayout(LayoutKind.Sequential, Size = 3, Pack = 1)]
	public partial struct Vector3c
		: IEquatable<Vector3c>, IEquatable<Vector4c>
	{
		public sbyte X;
		public sbyte Y;
		public sbyte Z;
		public uint Packed {
			get { return new Vector4c { X = X, Y = Y, Z = Z, }.Packed; }
			set {
				var V4 = new Vector4c { Packed = value, };
				this.X = V4.X;
				this.Y = V4.Y;
				this.Z = V4.Z;
			}
		}
		public sbyte this[int axis] {
			get {
				return 0 == (axis & 2) ? 0 == (axis & 1) ? X : Y : 0 == (axis & 1) ? Z : (sbyte)0;
			}
			set	{
				if (0 == (axis & 2))
					if (0 == (axis & 1))
						X = value;
					else
						Y = value;
				else
					if (0 == (axis & 1))
					Z = value;
			}
		}
		public bool Equals(Vector3c other) {
			return X == other.X && Y == other.Y && Z == other.Z;
		}
		public bool Equals(Vector4c other) {
			return 0 == other.W && X == other.X && Y == other.Y && Z == other.Z;
		}
		public override string ToString() {
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			var Zs = X == Z ? Xs : Y == Z ? Ys : Z == 0 ? "0" : Z.ToString();

			return string.Concat("[",
				string.Concat(Xs, ",", Ys, ","),
				Zs, "]");
		}
		public static bool operator ==(Vector3c L, Vector3c R) { return L.X == R.X && L.Y == R.Y && L.Z == R.Z; }
		public static bool operator !=(Vector3c L, Vector3c R) { return L.X != R.X || L.Y != R.Y || L.Z != R.Z; }
		public static bool operator ==(Vector3c L, uint R) { return L.Packed == R; }
		public static bool operator !=(Vector3c L, uint R) { return L.Packed != R; }
		public static bool operator ==(uint L, Vector3c R) { return L == R.Packed; }
		public static bool operator !=(uint L, Vector3c R) { return L != R.Packed; }

		public static Vector3c operator &(Vector3c L, Vector3c R) { return new Vector3c { Packed = (uint)(L.Packed & R.Packed), }; }
		public static Vector3c operator |(Vector3c L, Vector3c R) { return new Vector3c { Packed = (uint)(L.Packed | R.Packed), }; }
		public static Vector3c operator ^(Vector3c L, Vector3c R) { return new Vector3c { Packed = (uint)(L.Packed ^ R.Packed), }; }
		public static bool operator true(Vector3c criteria) { return criteria.X != 0 || criteria.Y != 0 || criteria.Z != 0; }
		public static bool operator false(Vector3c criteria) { return 0 == criteria.X && 0 == criteria.Y && 0 == criteria.Z; }
		public static Vector3c operator +(Vector3c L, Vector3c R)
		{
			unchecked
			{
				return new Vector3c {
					X = (sbyte)(L.X + R.X),
					Y = (sbyte)(L.Y + R.Y),
					Z = (sbyte)(L.Z + R.Z),
				};
			}
		}
		public static Vector3c operator +(Vector3c L, sbyte R)
		{
			unchecked
			{
				return new Vector3c {
					X = (sbyte)(L.X + R),
					Y = (sbyte)(L.Y + R),
					Z = (sbyte)(L.Z + R),
				};
			}
		}
		public static Vector3c operator +(sbyte L, Vector3c R)
		{
			unchecked
			{
				return new Vector3c {
					X = (sbyte)(L + R.X),
					Y = (sbyte)(L + R.Y),
					Z = (sbyte)(L + R.Z),
				};
			}
		}
		public static Vector3d operator +(Vector3c L, double R){
			return new Vector3d{
				X = (L.X + R),
				Y = (L.Y + R),
				Z = (L.Z + R),
			};
		}
		public static Vector3 operator +(Vector3c L, float R){
			return new Vector3{
				X = (L.X + R),
				Y = (L.Y + R),
				Z = (L.Z + R),
			};
		}
		public static Vector3d operator +(double L, Vector3c R){
			return new Vector3d{
				X = (L + R.X),
				Y = (L + R.Y),
				Z = (L + R.Z),
			};
		}
		public static Vector3 operator +(float L, Vector3c R){
			return new Vector3{
				X = (L + R.X),
				Y = (L + R.Y),
				Z = (L + R.Z),
			};
		}
		public static Vector3 operator +(Vector3 L, Vector3c R){
			return new Vector3{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
			};
		}
		public static Vector3d operator +(Vector3d L, Vector3c R){
			return new Vector3d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
			};
		}
		public static Vector3 operator +(Vector3c L, Vector3 R){
			return new Vector3{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
			};
		}
		public static Vector3d operator +(Vector3c L, Vector3d R){
			return new Vector3d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
			};
		}
		public static Vector3c operator -(Vector3c L, Vector3c R)
		{
			unchecked
			{
				return new Vector3c {
					X = (sbyte)(L.X - R.X),
					Y = (sbyte)(L.Y - R.Y),
					Z = (sbyte)(L.Z - R.Z),
				};
			}
		}
		public static Vector3c operator -(Vector3c L, sbyte R)
		{
			unchecked
			{
				return new Vector3c {
					X = (sbyte)(L.X - R),
					Y = (sbyte)(L.Y - R),
					Z = (sbyte)(L.Z - R),
				};
			}
		}
		public static Vector3c operator -(sbyte L, Vector3c R)
		{
			unchecked
			{
				return new Vector3c {
					X = (sbyte)(L - R.X),
					Y = (sbyte)(L - R.Y),
					Z = (sbyte)(L - R.Z),
				};
			}
		}
		public static Vector3d operator -(Vector3c L, double R){
			return new Vector3d{
				X = (L.X - R),
				Y = (L.Y - R),
				Z = (L.Z - R),
			};
		}
		public static Vector3 operator -(Vector3c L, float R){
			return new Vector3{
				X = (L.X - R),
				Y = (L.Y - R),
				Z = (L.Z - R),
			};
		}
		public static Vector3d operator -(double L, Vector3c R){
			return new Vector3d{
				X = (L - R.X),
				Y = (L - R.Y),
				Z = (L - R.Z),
			};
		}
		public static Vector3 operator -(float L, Vector3c R){
			return new Vector3{
				X = (L - R.X),
				Y = (L - R.Y),
				Z = (L - R.Z),
			};
		}
		public static Vector3 operator -(Vector3 L, Vector3c R){
			return new Vector3{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
			};
		}
		public static Vector3d operator -(Vector3d L, Vector3c R){
			return new Vector3d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
			};
		}
		public static Vector3 operator -(Vector3c L, Vector3 R){
			return new Vector3{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
			};
		}
		public static Vector3d operator -(Vector3c L, Vector3d R){
			return new Vector3d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
			};
		}
		public static Vector3c operator *(Vector3c L, Vector3c R)
		{
			unchecked
			{
				return new Vector3c {
					X = (sbyte)(L.X * R.X),
					Y = (sbyte)(L.Y * R.Y),
					Z = (sbyte)(L.Z * R.Z),
				};
			}
		}
		public static Vector3c operator *(Vector3c L, sbyte R)
		{
			unchecked
			{
				return new Vector3c {
					X = (sbyte)(L.X * R),
					Y = (sbyte)(L.Y * R),
					Z = (sbyte)(L.Z * R),
				};
			}
		}
		public static Vector3c operator *(sbyte L, Vector3c R)
		{
			unchecked
			{
				return new Vector3c {
					X = (sbyte)(L * R.X),
					Y = (sbyte)(L * R.Y),
					Z = (sbyte)(L * R.Z),
				};
			}
		}
		public static Vector3d operator *(Vector3c L, double R){
			return new Vector3d{
				X = (L.X * R),
				Y = (L.Y * R),
				Z = (L.Z * R),
			};
		}
		public static Vector3 operator *(Vector3c L, float R){
			return new Vector3{
				X = (L.X * R),
				Y = (L.Y * R),
				Z = (L.Z * R),
			};
		}
		public static Vector3d operator *(double L, Vector3c R){
			return new Vector3d{
				X = (L * R.X),
				Y = (L * R.Y),
				Z = (L * R.Z),
			};
		}
		public static Vector3 operator *(float L, Vector3c R){
			return new Vector3{
				X = (L * R.X),
				Y = (L * R.Y),
				Z = (L * R.Z),
			};
		}
		public static Vector3 operator *(Vector3 L, Vector3c R){
			return new Vector3{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
			};
		}
		public static Vector3d operator *(Vector3d L, Vector3c R){
			return new Vector3d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
			};
		}
		public static Vector3 operator *(Vector3c L, Vector3 R){
			return new Vector3{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
			};
		}
		public static Vector3d operator *(Vector3c L, Vector3d R){
			return new Vector3d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
			};
		}
		public static Vector3c operator /(Vector3c L, Vector3c R)
		{
			unchecked
			{
				return new Vector3c {
					X = (sbyte)(L.X / R.X),
					Y = (sbyte)(L.Y / R.Y),
					Z = (sbyte)(L.Z / R.Z),
				};
			}
		}
		public static Vector3c operator /(Vector3c L, sbyte R)
		{
			unchecked
			{
				return new Vector3c {
					X = (sbyte)(L.X / R),
					Y = (sbyte)(L.Y / R),
					Z = (sbyte)(L.Z / R),
				};
			}
		}
		public static Vector3c operator /(sbyte L, Vector3c R)
		{
			unchecked
			{
				return new Vector3c {
					X = (sbyte)(L / R.X),
					Y = (sbyte)(L / R.Y),
					Z = (sbyte)(L / R.Z),
				};
			}
		}
		public static Vector3d operator /(Vector3c L, double R){
			return new Vector3d{
				X = (L.X / R),
				Y = (L.Y / R),
				Z = (L.Z / R),
			};
		}
		public static Vector3 operator /(Vector3c L, float R){
			return new Vector3{
				X = (L.X / R),
				Y = (L.Y / R),
				Z = (L.Z / R),
			};
		}
		public static Vector3d operator /(double L, Vector3c R){
			return new Vector3d{
				X = (L / R.X),
				Y = (L / R.Y),
				Z = (L / R.Z),
			};
		}
		public static Vector3 operator /(float L, Vector3c R){
			return new Vector3{
				X = (L / R.X),
				Y = (L / R.Y),
				Z = (L / R.Z),
			};
		}
		public static Vector3 operator /(Vector3 L, Vector3c R){
			return new Vector3{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
			};
		}
		public static Vector3d operator /(Vector3d L, Vector3c R){
			return new Vector3d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
			};
		}
		public static Vector3 operator /(Vector3c L, Vector3 R){
			return new Vector3{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
			};
		}
		public static Vector3d operator /(Vector3c L, Vector3d R){
			return new Vector3d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
			};
		}
		public static explicit operator Vector3(Vector3c value)
		{
			return new Vector3{X = value.X,Y = value.Y,Z = value.Z,};
		}
		public static explicit operator Vector3d(Vector3c value)
		{
			return new Vector3d{X = value.X,Y = value.Y,Z = value.Z,};
		}
		public static explicit operator Vector3c(Vector3 value)
		{
			return new Vector3c{X = (sbyte)value.X,Y = (sbyte)value.Y,Z = (sbyte)value.Z,};
		}
		public static explicit operator Vector3c(Vector3d value)
		{
			return new Vector3c{X = (sbyte)value.X,Y = (sbyte)value.Y,Z = (sbyte)value.Z,};
		}
		public override int GetHashCode() {
			return Vector4c.GetHashCode(new Vector4c { X=X,Y=Y,Z=Z,});
		}
		public static Vector3c operator -(Vector3c v) {
			unchecked { 
				return new Vector3c {
					X= (sbyte)(-v.X),
					Y= (sbyte)(-v.X),
					Z= (sbyte)(-v.X),
				};
			}
		}
		public override bool Equals(object obj)
		{
			return obj is Vector3c && Packed == ((Vector3c)obj).Packed;
		}
		public static explicit operator Vector3c(uint value) {
			return new Vector3c { Packed = value, };
		}
	}
	///<summary>A 2 dimension vector with elements of type sbyte.</summary>
	[StructLayout(LayoutKind.Explicit, Size = sizeof(ushort))]
	public partial struct Vector2c
		: IEquatable<Vector2c>
	{
		[FieldOffset(sizeof(sbyte) * 1)]
		public sbyte X;
		[FieldOffset(sizeof(sbyte) * 0)]
		public sbyte Y;
		[FieldOffset(0)]
		public ushort Packed;
		public sbyte this[int axis]{
			get	{
				return 0 == (axis & 1) ? X : Y;
			}
			set	{
				if (0 == (axis & 1))
					X = value;
				else
					Y = value;
			}
		}
		public bool Equals(Vector2c other){
			return Packed == other.Packed;
		}

		public override string ToString(){
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			return string.Concat("[",
				string.Concat(Xs, ",", Ys, "]"));
		}
		public static bool operator ==(Vector2c L, Vector2c R) { return L.Packed == R.Packed; }
		public static bool operator !=(Vector2c L, Vector2c R) { return L.Packed != R.Packed; }
		public static bool operator ==(Vector2c L, ushort R) { return L.Packed == R; }
		public static bool operator !=(Vector2c L, ushort R) { return L.Packed != R; }
		public static bool operator ==(ushort L, Vector2c R) { return L == R.Packed; }
		public static bool operator !=(ushort L, Vector2c R) { return L != R.Packed; }

		public static Vector2c operator &(Vector2c L, Vector2c R) { return new Vector2c { Packed = (ushort)(L.Packed & R.Packed), }; }
		public static Vector2c operator |(Vector2c L, Vector2c R) { return new Vector2c { Packed = (ushort)(L.Packed | R.Packed), }; }
		public static Vector2c operator ^(Vector2c L, Vector2c R) { return new Vector2c { Packed = (ushort)(L.Packed ^ R.Packed), }; }
		public static bool operator true(Vector2c criteria) { return criteria.Packed != 0; }
		public static bool operator false(Vector2c criteria) { return 0 == criteria.Packed; }
		public static Vector2c operator +(Vector2c L, Vector2c R)
		{
			unchecked
			{
				return new Vector2c {
					X = (sbyte)(L.X + R.X),
					Y = (sbyte)(L.Y + R.Y),
				};
			}
		}
		public static Vector2c operator +(Vector2c L, sbyte R)
		{
			unchecked
			{
				return new Vector2c {
					X = (sbyte)(L.X + R),
					Y = (sbyte)(L.Y + R),
				};
			}
		}
		public static Vector2c operator +(sbyte L, Vector2c R)
		{
			unchecked
			{
				return new Vector2c {
					X = (sbyte)(L + R.X),
					Y = (sbyte)(L + R.Y),
				};
			}
		}
		public static Vector2d operator +(Vector2c L, double R){
			return new Vector2d{
				X = (L.X + R),
				Y = (L.Y + R),
			};
		}
		public static Vector2 operator +(Vector2c L, float R){
			return new Vector2{
				X = (L.X + R),
				Y = (L.Y + R),
			};
		}
		public static Vector2d operator +(double L, Vector2c R){
			return new Vector2d{
				X = (L + R.X),
				Y = (L + R.Y),
			};
		}
		public static Vector2 operator +(float L, Vector2c R){
			return new Vector2{
				X = (L + R.X),
				Y = (L + R.Y),
			};
		}
		public static Vector2 operator +(Vector2 L, Vector2c R){
			return new Vector2{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2d operator +(Vector2d L, Vector2c R){
			return new Vector2d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2 operator +(Vector2c L, Vector2 R){
			return new Vector2{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2d operator +(Vector2c L, Vector2d R){
			return new Vector2d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2c operator -(Vector2c L, Vector2c R)
		{
			unchecked
			{
				return new Vector2c {
					X = (sbyte)(L.X - R.X),
					Y = (sbyte)(L.Y - R.Y),
				};
			}
		}
		public static Vector2c operator -(Vector2c L, sbyte R)
		{
			unchecked
			{
				return new Vector2c {
					X = (sbyte)(L.X - R),
					Y = (sbyte)(L.Y - R),
				};
			}
		}
		public static Vector2c operator -(sbyte L, Vector2c R)
		{
			unchecked
			{
				return new Vector2c {
					X = (sbyte)(L - R.X),
					Y = (sbyte)(L - R.Y),
				};
			}
		}
		public static Vector2d operator -(Vector2c L, double R){
			return new Vector2d{
				X = (L.X - R),
				Y = (L.Y - R),
			};
		}
		public static Vector2 operator -(Vector2c L, float R){
			return new Vector2{
				X = (L.X - R),
				Y = (L.Y - R),
			};
		}
		public static Vector2d operator -(double L, Vector2c R){
			return new Vector2d{
				X = (L - R.X),
				Y = (L - R.Y),
			};
		}
		public static Vector2 operator -(float L, Vector2c R){
			return new Vector2{
				X = (L - R.X),
				Y = (L - R.Y),
			};
		}
		public static Vector2 operator -(Vector2 L, Vector2c R){
			return new Vector2{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2d operator -(Vector2d L, Vector2c R){
			return new Vector2d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2 operator -(Vector2c L, Vector2 R){
			return new Vector2{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2d operator -(Vector2c L, Vector2d R){
			return new Vector2d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2c operator *(Vector2c L, Vector2c R)
		{
			unchecked
			{
				return new Vector2c {
					X = (sbyte)(L.X * R.X),
					Y = (sbyte)(L.Y * R.Y),
				};
			}
		}
		public static Vector2c operator *(Vector2c L, sbyte R)
		{
			unchecked
			{
				return new Vector2c {
					X = (sbyte)(L.X * R),
					Y = (sbyte)(L.Y * R),
				};
			}
		}
		public static Vector2c operator *(sbyte L, Vector2c R)
		{
			unchecked
			{
				return new Vector2c {
					X = (sbyte)(L * R.X),
					Y = (sbyte)(L * R.Y),
				};
			}
		}
		public static Vector2d operator *(Vector2c L, double R){
			return new Vector2d{
				X = (L.X * R),
				Y = (L.Y * R),
			};
		}
		public static Vector2 operator *(Vector2c L, float R){
			return new Vector2{
				X = (L.X * R),
				Y = (L.Y * R),
			};
		}
		public static Vector2d operator *(double L, Vector2c R){
			return new Vector2d{
				X = (L * R.X),
				Y = (L * R.Y),
			};
		}
		public static Vector2 operator *(float L, Vector2c R){
			return new Vector2{
				X = (L * R.X),
				Y = (L * R.Y),
			};
		}
		public static Vector2 operator *(Vector2 L, Vector2c R){
			return new Vector2{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2d operator *(Vector2d L, Vector2c R){
			return new Vector2d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2 operator *(Vector2c L, Vector2 R){
			return new Vector2{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2d operator *(Vector2c L, Vector2d R){
			return new Vector2d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2c operator /(Vector2c L, Vector2c R)
		{
			unchecked
			{
				return new Vector2c {
					X = (sbyte)(L.X / R.X),
					Y = (sbyte)(L.Y / R.Y),
				};
			}
		}
		public static Vector2c operator /(Vector2c L, sbyte R)
		{
			unchecked
			{
				return new Vector2c {
					X = (sbyte)(L.X / R),
					Y = (sbyte)(L.Y / R),
				};
			}
		}
		public static Vector2c operator /(sbyte L, Vector2c R)
		{
			unchecked
			{
				return new Vector2c {
					X = (sbyte)(L / R.X),
					Y = (sbyte)(L / R.Y),
				};
			}
		}
		public static Vector2d operator /(Vector2c L, double R){
			return new Vector2d{
				X = (L.X / R),
				Y = (L.Y / R),
			};
		}
		public static Vector2 operator /(Vector2c L, float R){
			return new Vector2{
				X = (L.X / R),
				Y = (L.Y / R),
			};
		}
		public static Vector2d operator /(double L, Vector2c R){
			return new Vector2d{
				X = (L / R.X),
				Y = (L / R.Y),
			};
		}
		public static Vector2 operator /(float L, Vector2c R){
			return new Vector2{
				X = (L / R.X),
				Y = (L / R.Y),
			};
		}
		public static Vector2 operator /(Vector2 L, Vector2c R){
			return new Vector2{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static Vector2d operator /(Vector2d L, Vector2c R){
			return new Vector2d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static Vector2 operator /(Vector2c L, Vector2 R){
			return new Vector2{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static Vector2d operator /(Vector2c L, Vector2d R){
			return new Vector2d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static explicit operator Vector2(Vector2c value)
		{
			return new Vector2{X = value.X,Y = value.Y,};
		}
		public static explicit operator Vector2d(Vector2c value)
		{
			return new Vector2d{X = value.X,Y = value.Y,};
		}
		public static explicit operator Vector2c(Vector2 value)
		{
			return new Vector2c{X = (sbyte)value.X,Y = (sbyte)value.Y,};
		}
		public static explicit operator Vector2c(Vector2d value)
		{
			return new Vector2c{X = (sbyte)value.X,Y = (sbyte)value.Y,};
		}
		public override int GetHashCode() {
			return Vector4c.GetHashCode(new Vector4c { X=X,Y=Y,});
		}
		public static Vector2c operator -(Vector2c v) {
			unchecked { 
				return new Vector2c {
					X= (sbyte)(-v.X),
					Y= (sbyte)(-v.X),
				};
			}
		}
		public override bool Equals(object obj)
		{
			return obj is Vector2c && Packed == ((Vector2c)obj).Packed;
		}
		public static explicit operator Vector2c(ushort value) {
			return new Vector2c { Packed = value, };
		}
	}
	///<summary>A 4 dimension vector with elements of type byte.</summary>
	[StructLayout(LayoutKind.Explicit, Size = sizeof(uint))]
	public partial struct Vector4b
		: IEquatable<Vector4b>, IEquatable<Vector3b>
	{
		[FieldOffset(sizeof(byte) * 3)]
		public byte X;
		[FieldOffset(sizeof(byte) * 2)]
		public byte Y;
		[FieldOffset(sizeof(byte) * 1)]
		public byte Z;
		[FieldOffset(sizeof(byte) * 0)]
		public byte W;
		[FieldOffset(0)]
		public uint Packed;
		public byte this[int axis]{
			get	{
				return 0 == (axis & 2) ? 0 == (axis & 1) ? X : Y : 0 == (axis & 1) ? Z : W;
			}
			set	{
				if (0 == (axis & 2))
					if (0 == (axis & 1))
						X = value;
					else
						Y = value;
				else
					if (0 == (axis & 1))
					Z = value;
				else
					W = value;
			}
		}
		
		public bool Equals(Vector3b other) {
			return W == 0 && X == other.X && Y == other.Y && Z == other.Z;
		}
		public bool Equals(Vector4b other) {
			return Packed == other.Packed;
		}
		public override string ToString(){
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			var Zs = X == Z ? Xs : Y == Z ? Ys : Z == 0 ? "0" : Z.ToString();
			var Ws = X == W ? Xs : Y == W ? Ys : Z == W ? Zs : W == 0 ? "0" : W.ToString();

			return string.Concat("[",
				string.Concat(Xs, ",", Ys, ","),
				string.Concat(Zs, ",", Ws, "]"));
		}
		public static bool operator ==(Vector4b L, Vector4b R) { return L.Packed == R.Packed; }
		public static bool operator !=(Vector4b L, Vector4b R) { return L.Packed != R.Packed; }
		public static bool operator ==(Vector4b L, uint R) { return L.Packed == R; }
		public static bool operator !=(Vector4b L, uint R) { return L.Packed != R; }
		public static bool operator ==(uint L, Vector4b R) { return L == R.Packed; }
		public static bool operator !=(uint L, Vector4b R) { return L != R.Packed; }

		public static Vector4b operator &(Vector4b L, Vector4b R) { return new Vector4b { Packed = (uint)(L.Packed & R.Packed), }; }
		public static Vector4b operator |(Vector4b L, Vector4b R) { return new Vector4b { Packed = (uint)(L.Packed | R.Packed), }; }
		public static Vector4b operator ^(Vector4b L, Vector4b R) { return new Vector4b { Packed = (uint)(L.Packed ^ R.Packed), }; }
		public static bool operator true(Vector4b criteria) { return criteria.Packed != 0; }
		public static bool operator false(Vector4b criteria) { return 0 == criteria.Packed; }
		public static Vector4b operator +(Vector4b L, Vector4b R)
		{
			unchecked
			{
				return new Vector4b {
					X = (byte)(L.X + R.X),
					Y = (byte)(L.Y + R.Y),
					Z = (byte)(L.Z + R.Z),
					W = (byte)(L.W + R.W),
				};
			}
		}
		public static Vector4b operator +(Vector4b L, byte R)
		{
			unchecked
			{
				return new Vector4b {
					X = (byte)(L.X + R),
					Y = (byte)(L.Y + R),
					Z = (byte)(L.Z + R),
					W = (byte)(L.W + R),
				};
			}
		}
		public static Vector4b operator +(byte L, Vector4b R)
		{
			unchecked
			{
				return new Vector4b {
					X = (byte)(L + R.X),
					Y = (byte)(L + R.Y),
					Z = (byte)(L + R.Z),
					W = (byte)(L + R.W),
				};
			}
		}
		public static Vector4d operator +(Vector4b L, double R){
			return new Vector4d{
				X = (L.X + R),
				Y = (L.Y + R),
				Z = (L.Z + R),
				W = (L.W + R),
			};
		}
		public static Vector4 operator +(Vector4b L, float R){
			return new Vector4{
				X = (L.X + R),
				Y = (L.Y + R),
				Z = (L.Z + R),
				W = (L.W + R),
			};
		}
		public static Vector4d operator +(double L, Vector4b R){
			return new Vector4d{
				X = (L + R.X),
				Y = (L + R.Y),
				Z = (L + R.Z),
				W = (L + R.W),
			};
		}
		public static Vector4 operator +(float L, Vector4b R){
			return new Vector4{
				X = (L + R.X),
				Y = (L + R.Y),
				Z = (L + R.Z),
				W = (L + R.W),
			};
		}
		public static Vector4 operator +(Vector4 L, Vector4b R){
			return new Vector4{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
				W = (L.W + R.W),
			};
		}
		public static Vector4d operator +(Vector4d L, Vector4b R){
			return new Vector4d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
				W = (L.W + R.W),
			};
		}
		public static Vector4 operator +(Vector4b L, Vector4 R){
			return new Vector4{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
				W = (L.W + R.W),
			};
		}
		public static Vector4d operator +(Vector4b L, Vector4d R){
			return new Vector4d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
				W = (L.W + R.W),
			};
		}
		public static Vector4b operator -(Vector4b L, Vector4b R)
		{
			unchecked
			{
				return new Vector4b {
					X = (byte)(L.X - R.X),
					Y = (byte)(L.Y - R.Y),
					Z = (byte)(L.Z - R.Z),
					W = (byte)(L.W - R.W),
				};
			}
		}
		public static Vector4b operator -(Vector4b L, byte R)
		{
			unchecked
			{
				return new Vector4b {
					X = (byte)(L.X - R),
					Y = (byte)(L.Y - R),
					Z = (byte)(L.Z - R),
					W = (byte)(L.W - R),
				};
			}
		}
		public static Vector4b operator -(byte L, Vector4b R)
		{
			unchecked
			{
				return new Vector4b {
					X = (byte)(L - R.X),
					Y = (byte)(L - R.Y),
					Z = (byte)(L - R.Z),
					W = (byte)(L - R.W),
				};
			}
		}
		public static Vector4d operator -(Vector4b L, double R){
			return new Vector4d{
				X = (L.X - R),
				Y = (L.Y - R),
				Z = (L.Z - R),
				W = (L.W - R),
			};
		}
		public static Vector4 operator -(Vector4b L, float R){
			return new Vector4{
				X = (L.X - R),
				Y = (L.Y - R),
				Z = (L.Z - R),
				W = (L.W - R),
			};
		}
		public static Vector4d operator -(double L, Vector4b R){
			return new Vector4d{
				X = (L - R.X),
				Y = (L - R.Y),
				Z = (L - R.Z),
				W = (L - R.W),
			};
		}
		public static Vector4 operator -(float L, Vector4b R){
			return new Vector4{
				X = (L - R.X),
				Y = (L - R.Y),
				Z = (L - R.Z),
				W = (L - R.W),
			};
		}
		public static Vector4 operator -(Vector4 L, Vector4b R){
			return new Vector4{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
				W = (L.W - R.W),
			};
		}
		public static Vector4d operator -(Vector4d L, Vector4b R){
			return new Vector4d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
				W = (L.W - R.W),
			};
		}
		public static Vector4 operator -(Vector4b L, Vector4 R){
			return new Vector4{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
				W = (L.W - R.W),
			};
		}
		public static Vector4d operator -(Vector4b L, Vector4d R){
			return new Vector4d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
				W = (L.W - R.W),
			};
		}
		public static Vector4b operator *(Vector4b L, Vector4b R)
		{
			unchecked
			{
				return new Vector4b {
					X = (byte)(L.X * R.X),
					Y = (byte)(L.Y * R.Y),
					Z = (byte)(L.Z * R.Z),
					W = (byte)(L.W * R.W),
				};
			}
		}
		public static Vector4b operator *(Vector4b L, byte R)
		{
			unchecked
			{
				return new Vector4b {
					X = (byte)(L.X * R),
					Y = (byte)(L.Y * R),
					Z = (byte)(L.Z * R),
					W = (byte)(L.W * R),
				};
			}
		}
		public static Vector4b operator *(byte L, Vector4b R)
		{
			unchecked
			{
				return new Vector4b {
					X = (byte)(L * R.X),
					Y = (byte)(L * R.Y),
					Z = (byte)(L * R.Z),
					W = (byte)(L * R.W),
				};
			}
		}
		public static Vector4d operator *(Vector4b L, double R){
			return new Vector4d{
				X = (L.X * R),
				Y = (L.Y * R),
				Z = (L.Z * R),
				W = (L.W * R),
			};
		}
		public static Vector4 operator *(Vector4b L, float R){
			return new Vector4{
				X = (L.X * R),
				Y = (L.Y * R),
				Z = (L.Z * R),
				W = (L.W * R),
			};
		}
		public static Vector4d operator *(double L, Vector4b R){
			return new Vector4d{
				X = (L * R.X),
				Y = (L * R.Y),
				Z = (L * R.Z),
				W = (L * R.W),
			};
		}
		public static Vector4 operator *(float L, Vector4b R){
			return new Vector4{
				X = (L * R.X),
				Y = (L * R.Y),
				Z = (L * R.Z),
				W = (L * R.W),
			};
		}
		public static Vector4 operator *(Vector4 L, Vector4b R){
			return new Vector4{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
				W = (L.W * R.W),
			};
		}
		public static Vector4d operator *(Vector4d L, Vector4b R){
			return new Vector4d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
				W = (L.W * R.W),
			};
		}
		public static Vector4 operator *(Vector4b L, Vector4 R){
			return new Vector4{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
				W = (L.W * R.W),
			};
		}
		public static Vector4d operator *(Vector4b L, Vector4d R){
			return new Vector4d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
				W = (L.W * R.W),
			};
		}
		public static Vector4b operator /(Vector4b L, Vector4b R)
		{
			unchecked
			{
				return new Vector4b {
					X = (byte)(L.X / R.X),
					Y = (byte)(L.Y / R.Y),
					Z = (byte)(L.Z / R.Z),
					W = (byte)(L.W / R.W),
				};
			}
		}
		public static Vector4b operator /(Vector4b L, byte R)
		{
			unchecked
			{
				return new Vector4b {
					X = (byte)(L.X / R),
					Y = (byte)(L.Y / R),
					Z = (byte)(L.Z / R),
					W = (byte)(L.W / R),
				};
			}
		}
		public static Vector4b operator /(byte L, Vector4b R)
		{
			unchecked
			{
				return new Vector4b {
					X = (byte)(L / R.X),
					Y = (byte)(L / R.Y),
					Z = (byte)(L / R.Z),
					W = (byte)(L / R.W),
				};
			}
		}
		public static Vector4d operator /(Vector4b L, double R){
			return new Vector4d{
				X = (L.X / R),
				Y = (L.Y / R),
				Z = (L.Z / R),
				W = (L.W / R),
			};
		}
		public static Vector4 operator /(Vector4b L, float R){
			return new Vector4{
				X = (L.X / R),
				Y = (L.Y / R),
				Z = (L.Z / R),
				W = (L.W / R),
			};
		}
		public static Vector4d operator /(double L, Vector4b R){
			return new Vector4d{
				X = (L / R.X),
				Y = (L / R.Y),
				Z = (L / R.Z),
				W = (L / R.W),
			};
		}
		public static Vector4 operator /(float L, Vector4b R){
			return new Vector4{
				X = (L / R.X),
				Y = (L / R.Y),
				Z = (L / R.Z),
				W = (L / R.W),
			};
		}
		public static Vector4 operator /(Vector4 L, Vector4b R){
			return new Vector4{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
				W = (L.W / R.W),
			};
		}
		public static Vector4d operator /(Vector4d L, Vector4b R){
			return new Vector4d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
				W = (L.W / R.W),
			};
		}
		public static Vector4 operator /(Vector4b L, Vector4 R){
			return new Vector4{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
				W = (L.W / R.W),
			};
		}
		public static Vector4d operator /(Vector4b L, Vector4d R){
			return new Vector4d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
				W = (L.W / R.W),
			};
		}
		public static explicit operator Vector4(Vector4b value)
		{
			return new Vector4{X = value.X,Y = value.Y,Z = value.Z,W = value.W,};
		}
		public static explicit operator Vector4d(Vector4b value)
		{
			return new Vector4d{X = value.X,Y = value.Y,Z = value.Z,W = value.W,};
		}
		public static explicit operator Vector4b(Vector4 value)
		{
			return new Vector4b{X = (byte)value.X,Y = (byte)value.Y,Z = (byte)value.Z,W = (byte)value.W,};
		}
		public static explicit operator Vector4b(Vector4d value)
		{
			return new Vector4b{X = (byte)value.X,Y = (byte)value.Y,Z = (byte)value.Z,W = (byte)value.W,};
		}
		public override int GetHashCode(){return GetHashCode(ref this);}
		public static int GetHashCode(Vector4b Value){ 
			// IF YOU GET AN ERROR ON THE LINE BELOW, ITS BECAUSE YOU DID NOT IMPLEMENT
			// IT OUTSIDE OF THIS PARTIAL GENERATED CODE.
			//
			// NOTE THAT THE ARGUMENT MUST BE NAMED VALUE!
			return GetHashCode(Value:ref Value);
		}
		public static Vector4b operator -(Vector4b v) {
			unchecked { 
				return new Vector4b {
					X= (byte)(-v.X),
					Y= (byte)(-v.X),
					Z= (byte)(-v.X),
					W= (byte)(-v.X),
				};
			}
		}
		public override bool Equals(object obj)
		{
			return obj is Vector4b && Packed == ((Vector4b)obj).Packed;
		}
		public static explicit operator Vector4b(uint value) {
			return new Vector4b { Packed = value, };
		}
	}
	///<summary>A 3 dimension vector with elements of type byte.</summary>
	[StructLayout(LayoutKind.Sequential, Size = 3, Pack = 1)]
	public partial struct Vector3b
		: IEquatable<Vector3b>, IEquatable<Vector4b>
	{
		public byte X;
		public byte Y;
		public byte Z;
		public uint Packed {
			get { return new Vector4b { X = X, Y = Y, Z = Z, }.Packed; }
			set {
				var V4 = new Vector4b { Packed = value, };
				this.X = V4.X;
				this.Y = V4.Y;
				this.Z = V4.Z;
			}
		}
		public byte this[int axis] {
			get {
				return 0 == (axis & 2) ? 0 == (axis & 1) ? X : Y : 0 == (axis & 1) ? Z : (byte)0;
			}
			set	{
				if (0 == (axis & 2))
					if (0 == (axis & 1))
						X = value;
					else
						Y = value;
				else
					if (0 == (axis & 1))
					Z = value;
			}
		}
		public bool Equals(Vector3b other) {
			return X == other.X && Y == other.Y && Z == other.Z;
		}
		public bool Equals(Vector4b other) {
			return 0 == other.W && X == other.X && Y == other.Y && Z == other.Z;
		}
		public override string ToString() {
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			var Zs = X == Z ? Xs : Y == Z ? Ys : Z == 0 ? "0" : Z.ToString();

			return string.Concat("[",
				string.Concat(Xs, ",", Ys, ","),
				Zs, "]");
		}
		public static bool operator ==(Vector3b L, Vector3b R) { return L.X == R.X && L.Y == R.Y && L.Z == R.Z; }
		public static bool operator !=(Vector3b L, Vector3b R) { return L.X != R.X || L.Y != R.Y || L.Z != R.Z; }
		public static bool operator ==(Vector3b L, uint R) { return L.Packed == R; }
		public static bool operator !=(Vector3b L, uint R) { return L.Packed != R; }
		public static bool operator ==(uint L, Vector3b R) { return L == R.Packed; }
		public static bool operator !=(uint L, Vector3b R) { return L != R.Packed; }

		public static Vector3b operator &(Vector3b L, Vector3b R) { return new Vector3b { Packed = (uint)(L.Packed & R.Packed), }; }
		public static Vector3b operator |(Vector3b L, Vector3b R) { return new Vector3b { Packed = (uint)(L.Packed | R.Packed), }; }
		public static Vector3b operator ^(Vector3b L, Vector3b R) { return new Vector3b { Packed = (uint)(L.Packed ^ R.Packed), }; }
		public static bool operator true(Vector3b criteria) { return criteria.X != 0 || criteria.Y != 0 || criteria.Z != 0; }
		public static bool operator false(Vector3b criteria) { return 0 == criteria.X && 0 == criteria.Y && 0 == criteria.Z; }
		public static Vector3b operator +(Vector3b L, Vector3b R)
		{
			unchecked
			{
				return new Vector3b {
					X = (byte)(L.X + R.X),
					Y = (byte)(L.Y + R.Y),
					Z = (byte)(L.Z + R.Z),
				};
			}
		}
		public static Vector3b operator +(Vector3b L, byte R)
		{
			unchecked
			{
				return new Vector3b {
					X = (byte)(L.X + R),
					Y = (byte)(L.Y + R),
					Z = (byte)(L.Z + R),
				};
			}
		}
		public static Vector3b operator +(byte L, Vector3b R)
		{
			unchecked
			{
				return new Vector3b {
					X = (byte)(L + R.X),
					Y = (byte)(L + R.Y),
					Z = (byte)(L + R.Z),
				};
			}
		}
		public static Vector3d operator +(Vector3b L, double R){
			return new Vector3d{
				X = (L.X + R),
				Y = (L.Y + R),
				Z = (L.Z + R),
			};
		}
		public static Vector3 operator +(Vector3b L, float R){
			return new Vector3{
				X = (L.X + R),
				Y = (L.Y + R),
				Z = (L.Z + R),
			};
		}
		public static Vector3d operator +(double L, Vector3b R){
			return new Vector3d{
				X = (L + R.X),
				Y = (L + R.Y),
				Z = (L + R.Z),
			};
		}
		public static Vector3 operator +(float L, Vector3b R){
			return new Vector3{
				X = (L + R.X),
				Y = (L + R.Y),
				Z = (L + R.Z),
			};
		}
		public static Vector3 operator +(Vector3 L, Vector3b R){
			return new Vector3{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
			};
		}
		public static Vector3d operator +(Vector3d L, Vector3b R){
			return new Vector3d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
			};
		}
		public static Vector3 operator +(Vector3b L, Vector3 R){
			return new Vector3{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
			};
		}
		public static Vector3d operator +(Vector3b L, Vector3d R){
			return new Vector3d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
			};
		}
		public static Vector3b operator -(Vector3b L, Vector3b R)
		{
			unchecked
			{
				return new Vector3b {
					X = (byte)(L.X - R.X),
					Y = (byte)(L.Y - R.Y),
					Z = (byte)(L.Z - R.Z),
				};
			}
		}
		public static Vector3b operator -(Vector3b L, byte R)
		{
			unchecked
			{
				return new Vector3b {
					X = (byte)(L.X - R),
					Y = (byte)(L.Y - R),
					Z = (byte)(L.Z - R),
				};
			}
		}
		public static Vector3b operator -(byte L, Vector3b R)
		{
			unchecked
			{
				return new Vector3b {
					X = (byte)(L - R.X),
					Y = (byte)(L - R.Y),
					Z = (byte)(L - R.Z),
				};
			}
		}
		public static Vector3d operator -(Vector3b L, double R){
			return new Vector3d{
				X = (L.X - R),
				Y = (L.Y - R),
				Z = (L.Z - R),
			};
		}
		public static Vector3 operator -(Vector3b L, float R){
			return new Vector3{
				X = (L.X - R),
				Y = (L.Y - R),
				Z = (L.Z - R),
			};
		}
		public static Vector3d operator -(double L, Vector3b R){
			return new Vector3d{
				X = (L - R.X),
				Y = (L - R.Y),
				Z = (L - R.Z),
			};
		}
		public static Vector3 operator -(float L, Vector3b R){
			return new Vector3{
				X = (L - R.X),
				Y = (L - R.Y),
				Z = (L - R.Z),
			};
		}
		public static Vector3 operator -(Vector3 L, Vector3b R){
			return new Vector3{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
			};
		}
		public static Vector3d operator -(Vector3d L, Vector3b R){
			return new Vector3d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
			};
		}
		public static Vector3 operator -(Vector3b L, Vector3 R){
			return new Vector3{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
			};
		}
		public static Vector3d operator -(Vector3b L, Vector3d R){
			return new Vector3d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
			};
		}
		public static Vector3b operator *(Vector3b L, Vector3b R)
		{
			unchecked
			{
				return new Vector3b {
					X = (byte)(L.X * R.X),
					Y = (byte)(L.Y * R.Y),
					Z = (byte)(L.Z * R.Z),
				};
			}
		}
		public static Vector3b operator *(Vector3b L, byte R)
		{
			unchecked
			{
				return new Vector3b {
					X = (byte)(L.X * R),
					Y = (byte)(L.Y * R),
					Z = (byte)(L.Z * R),
				};
			}
		}
		public static Vector3b operator *(byte L, Vector3b R)
		{
			unchecked
			{
				return new Vector3b {
					X = (byte)(L * R.X),
					Y = (byte)(L * R.Y),
					Z = (byte)(L * R.Z),
				};
			}
		}
		public static Vector3d operator *(Vector3b L, double R){
			return new Vector3d{
				X = (L.X * R),
				Y = (L.Y * R),
				Z = (L.Z * R),
			};
		}
		public static Vector3 operator *(Vector3b L, float R){
			return new Vector3{
				X = (L.X * R),
				Y = (L.Y * R),
				Z = (L.Z * R),
			};
		}
		public static Vector3d operator *(double L, Vector3b R){
			return new Vector3d{
				X = (L * R.X),
				Y = (L * R.Y),
				Z = (L * R.Z),
			};
		}
		public static Vector3 operator *(float L, Vector3b R){
			return new Vector3{
				X = (L * R.X),
				Y = (L * R.Y),
				Z = (L * R.Z),
			};
		}
		public static Vector3 operator *(Vector3 L, Vector3b R){
			return new Vector3{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
			};
		}
		public static Vector3d operator *(Vector3d L, Vector3b R){
			return new Vector3d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
			};
		}
		public static Vector3 operator *(Vector3b L, Vector3 R){
			return new Vector3{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
			};
		}
		public static Vector3d operator *(Vector3b L, Vector3d R){
			return new Vector3d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
			};
		}
		public static Vector3b operator /(Vector3b L, Vector3b R)
		{
			unchecked
			{
				return new Vector3b {
					X = (byte)(L.X / R.X),
					Y = (byte)(L.Y / R.Y),
					Z = (byte)(L.Z / R.Z),
				};
			}
		}
		public static Vector3b operator /(Vector3b L, byte R)
		{
			unchecked
			{
				return new Vector3b {
					X = (byte)(L.X / R),
					Y = (byte)(L.Y / R),
					Z = (byte)(L.Z / R),
				};
			}
		}
		public static Vector3b operator /(byte L, Vector3b R)
		{
			unchecked
			{
				return new Vector3b {
					X = (byte)(L / R.X),
					Y = (byte)(L / R.Y),
					Z = (byte)(L / R.Z),
				};
			}
		}
		public static Vector3d operator /(Vector3b L, double R){
			return new Vector3d{
				X = (L.X / R),
				Y = (L.Y / R),
				Z = (L.Z / R),
			};
		}
		public static Vector3 operator /(Vector3b L, float R){
			return new Vector3{
				X = (L.X / R),
				Y = (L.Y / R),
				Z = (L.Z / R),
			};
		}
		public static Vector3d operator /(double L, Vector3b R){
			return new Vector3d{
				X = (L / R.X),
				Y = (L / R.Y),
				Z = (L / R.Z),
			};
		}
		public static Vector3 operator /(float L, Vector3b R){
			return new Vector3{
				X = (L / R.X),
				Y = (L / R.Y),
				Z = (L / R.Z),
			};
		}
		public static Vector3 operator /(Vector3 L, Vector3b R){
			return new Vector3{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
			};
		}
		public static Vector3d operator /(Vector3d L, Vector3b R){
			return new Vector3d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
			};
		}
		public static Vector3 operator /(Vector3b L, Vector3 R){
			return new Vector3{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
			};
		}
		public static Vector3d operator /(Vector3b L, Vector3d R){
			return new Vector3d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
			};
		}
		public static explicit operator Vector3(Vector3b value)
		{
			return new Vector3{X = value.X,Y = value.Y,Z = value.Z,};
		}
		public static explicit operator Vector3d(Vector3b value)
		{
			return new Vector3d{X = value.X,Y = value.Y,Z = value.Z,};
		}
		public static explicit operator Vector3b(Vector3 value)
		{
			return new Vector3b{X = (byte)value.X,Y = (byte)value.Y,Z = (byte)value.Z,};
		}
		public static explicit operator Vector3b(Vector3d value)
		{
			return new Vector3b{X = (byte)value.X,Y = (byte)value.Y,Z = (byte)value.Z,};
		}
		public override int GetHashCode() {
			return Vector4b.GetHashCode(new Vector4b { X=X,Y=Y,Z=Z,});
		}
		public static Vector3b operator -(Vector3b v) {
			unchecked { 
				return new Vector3b {
					X= (byte)(-v.X),
					Y= (byte)(-v.X),
					Z= (byte)(-v.X),
				};
			}
		}
		public override bool Equals(object obj)
		{
			return obj is Vector3b && Packed == ((Vector3b)obj).Packed;
		}
		public static explicit operator Vector3b(uint value) {
			return new Vector3b { Packed = value, };
		}
	}
	///<summary>A 2 dimension vector with elements of type byte.</summary>
	[StructLayout(LayoutKind.Explicit, Size = sizeof(ushort))]
	public partial struct Vector2b
		: IEquatable<Vector2b>
	{
		[FieldOffset(sizeof(byte) * 1)]
		public byte X;
		[FieldOffset(sizeof(byte) * 0)]
		public byte Y;
		[FieldOffset(0)]
		public ushort Packed;
		public byte this[int axis]{
			get	{
				return 0 == (axis & 1) ? X : Y;
			}
			set	{
				if (0 == (axis & 1))
					X = value;
				else
					Y = value;
			}
		}
		public bool Equals(Vector2b other){
			return Packed == other.Packed;
		}

		public override string ToString(){
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			return string.Concat("[",
				string.Concat(Xs, ",", Ys, "]"));
		}
		public static bool operator ==(Vector2b L, Vector2b R) { return L.Packed == R.Packed; }
		public static bool operator !=(Vector2b L, Vector2b R) { return L.Packed != R.Packed; }
		public static bool operator ==(Vector2b L, ushort R) { return L.Packed == R; }
		public static bool operator !=(Vector2b L, ushort R) { return L.Packed != R; }
		public static bool operator ==(ushort L, Vector2b R) { return L == R.Packed; }
		public static bool operator !=(ushort L, Vector2b R) { return L != R.Packed; }

		public static Vector2b operator &(Vector2b L, Vector2b R) { return new Vector2b { Packed = (ushort)(L.Packed & R.Packed), }; }
		public static Vector2b operator |(Vector2b L, Vector2b R) { return new Vector2b { Packed = (ushort)(L.Packed | R.Packed), }; }
		public static Vector2b operator ^(Vector2b L, Vector2b R) { return new Vector2b { Packed = (ushort)(L.Packed ^ R.Packed), }; }
		public static bool operator true(Vector2b criteria) { return criteria.Packed != 0; }
		public static bool operator false(Vector2b criteria) { return 0 == criteria.Packed; }
		public static Vector2b operator +(Vector2b L, Vector2b R)
		{
			unchecked
			{
				return new Vector2b {
					X = (byte)(L.X + R.X),
					Y = (byte)(L.Y + R.Y),
				};
			}
		}
		public static Vector2b operator +(Vector2b L, byte R)
		{
			unchecked
			{
				return new Vector2b {
					X = (byte)(L.X + R),
					Y = (byte)(L.Y + R),
				};
			}
		}
		public static Vector2b operator +(byte L, Vector2b R)
		{
			unchecked
			{
				return new Vector2b {
					X = (byte)(L + R.X),
					Y = (byte)(L + R.Y),
				};
			}
		}
		public static Vector2d operator +(Vector2b L, double R){
			return new Vector2d{
				X = (L.X + R),
				Y = (L.Y + R),
			};
		}
		public static Vector2 operator +(Vector2b L, float R){
			return new Vector2{
				X = (L.X + R),
				Y = (L.Y + R),
			};
		}
		public static Vector2d operator +(double L, Vector2b R){
			return new Vector2d{
				X = (L + R.X),
				Y = (L + R.Y),
			};
		}
		public static Vector2 operator +(float L, Vector2b R){
			return new Vector2{
				X = (L + R.X),
				Y = (L + R.Y),
			};
		}
		public static Vector2 operator +(Vector2 L, Vector2b R){
			return new Vector2{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2d operator +(Vector2d L, Vector2b R){
			return new Vector2d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2 operator +(Vector2b L, Vector2 R){
			return new Vector2{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2d operator +(Vector2b L, Vector2d R){
			return new Vector2d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2b operator -(Vector2b L, Vector2b R)
		{
			unchecked
			{
				return new Vector2b {
					X = (byte)(L.X - R.X),
					Y = (byte)(L.Y - R.Y),
				};
			}
		}
		public static Vector2b operator -(Vector2b L, byte R)
		{
			unchecked
			{
				return new Vector2b {
					X = (byte)(L.X - R),
					Y = (byte)(L.Y - R),
				};
			}
		}
		public static Vector2b operator -(byte L, Vector2b R)
		{
			unchecked
			{
				return new Vector2b {
					X = (byte)(L - R.X),
					Y = (byte)(L - R.Y),
				};
			}
		}
		public static Vector2d operator -(Vector2b L, double R){
			return new Vector2d{
				X = (L.X - R),
				Y = (L.Y - R),
			};
		}
		public static Vector2 operator -(Vector2b L, float R){
			return new Vector2{
				X = (L.X - R),
				Y = (L.Y - R),
			};
		}
		public static Vector2d operator -(double L, Vector2b R){
			return new Vector2d{
				X = (L - R.X),
				Y = (L - R.Y),
			};
		}
		public static Vector2 operator -(float L, Vector2b R){
			return new Vector2{
				X = (L - R.X),
				Y = (L - R.Y),
			};
		}
		public static Vector2 operator -(Vector2 L, Vector2b R){
			return new Vector2{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2d operator -(Vector2d L, Vector2b R){
			return new Vector2d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2 operator -(Vector2b L, Vector2 R){
			return new Vector2{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2d operator -(Vector2b L, Vector2d R){
			return new Vector2d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2b operator *(Vector2b L, Vector2b R)
		{
			unchecked
			{
				return new Vector2b {
					X = (byte)(L.X * R.X),
					Y = (byte)(L.Y * R.Y),
				};
			}
		}
		public static Vector2b operator *(Vector2b L, byte R)
		{
			unchecked
			{
				return new Vector2b {
					X = (byte)(L.X * R),
					Y = (byte)(L.Y * R),
				};
			}
		}
		public static Vector2b operator *(byte L, Vector2b R)
		{
			unchecked
			{
				return new Vector2b {
					X = (byte)(L * R.X),
					Y = (byte)(L * R.Y),
				};
			}
		}
		public static Vector2d operator *(Vector2b L, double R){
			return new Vector2d{
				X = (L.X * R),
				Y = (L.Y * R),
			};
		}
		public static Vector2 operator *(Vector2b L, float R){
			return new Vector2{
				X = (L.X * R),
				Y = (L.Y * R),
			};
		}
		public static Vector2d operator *(double L, Vector2b R){
			return new Vector2d{
				X = (L * R.X),
				Y = (L * R.Y),
			};
		}
		public static Vector2 operator *(float L, Vector2b R){
			return new Vector2{
				X = (L * R.X),
				Y = (L * R.Y),
			};
		}
		public static Vector2 operator *(Vector2 L, Vector2b R){
			return new Vector2{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2d operator *(Vector2d L, Vector2b R){
			return new Vector2d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2 operator *(Vector2b L, Vector2 R){
			return new Vector2{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2d operator *(Vector2b L, Vector2d R){
			return new Vector2d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2b operator /(Vector2b L, Vector2b R)
		{
			unchecked
			{
				return new Vector2b {
					X = (byte)(L.X / R.X),
					Y = (byte)(L.Y / R.Y),
				};
			}
		}
		public static Vector2b operator /(Vector2b L, byte R)
		{
			unchecked
			{
				return new Vector2b {
					X = (byte)(L.X / R),
					Y = (byte)(L.Y / R),
				};
			}
		}
		public static Vector2b operator /(byte L, Vector2b R)
		{
			unchecked
			{
				return new Vector2b {
					X = (byte)(L / R.X),
					Y = (byte)(L / R.Y),
				};
			}
		}
		public static Vector2d operator /(Vector2b L, double R){
			return new Vector2d{
				X = (L.X / R),
				Y = (L.Y / R),
			};
		}
		public static Vector2 operator /(Vector2b L, float R){
			return new Vector2{
				X = (L.X / R),
				Y = (L.Y / R),
			};
		}
		public static Vector2d operator /(double L, Vector2b R){
			return new Vector2d{
				X = (L / R.X),
				Y = (L / R.Y),
			};
		}
		public static Vector2 operator /(float L, Vector2b R){
			return new Vector2{
				X = (L / R.X),
				Y = (L / R.Y),
			};
		}
		public static Vector2 operator /(Vector2 L, Vector2b R){
			return new Vector2{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static Vector2d operator /(Vector2d L, Vector2b R){
			return new Vector2d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static Vector2 operator /(Vector2b L, Vector2 R){
			return new Vector2{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static Vector2d operator /(Vector2b L, Vector2d R){
			return new Vector2d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static explicit operator Vector2(Vector2b value)
		{
			return new Vector2{X = value.X,Y = value.Y,};
		}
		public static explicit operator Vector2d(Vector2b value)
		{
			return new Vector2d{X = value.X,Y = value.Y,};
		}
		public static explicit operator Vector2b(Vector2 value)
		{
			return new Vector2b{X = (byte)value.X,Y = (byte)value.Y,};
		}
		public static explicit operator Vector2b(Vector2d value)
		{
			return new Vector2b{X = (byte)value.X,Y = (byte)value.Y,};
		}
		public override int GetHashCode() {
			return Vector4b.GetHashCode(new Vector4b { X=X,Y=Y,});
		}
		public static Vector2b operator -(Vector2b v) {
			unchecked { 
				return new Vector2b {
					X= (byte)(-v.X),
					Y= (byte)(-v.X),
				};
			}
		}
		public override bool Equals(object obj)
		{
			return obj is Vector2b && Packed == ((Vector2b)obj).Packed;
		}
		public static explicit operator Vector2b(ushort value) {
			return new Vector2b { Packed = value, };
		}
	}
	///<summary>A 4 dimension vector with elements of type ushort.</summary>
	[StructLayout(LayoutKind.Explicit, Size = sizeof(ulong))]
	public partial struct Vector4w
		: IEquatable<Vector4w>, IEquatable<Vector3w>
	{
		[FieldOffset(sizeof(ushort) * 3)]
		public ushort X;
		[FieldOffset(sizeof(ushort) * 2)]
		public ushort Y;
		[FieldOffset(sizeof(ushort) * 1)]
		public ushort Z;
		[FieldOffset(sizeof(ushort) * 0)]
		public ushort W;
		[FieldOffset(0)]
		public ulong Packed;
		public ushort this[int axis]{
			get	{
				return 0 == (axis & 2) ? 0 == (axis & 1) ? X : Y : 0 == (axis & 1) ? Z : W;
			}
			set	{
				if (0 == (axis & 2))
					if (0 == (axis & 1))
						X = value;
					else
						Y = value;
				else
					if (0 == (axis & 1))
					Z = value;
				else
					W = value;
			}
		}
		
		public bool Equals(Vector3w other) {
			return W == 0 && X == other.X && Y == other.Y && Z == other.Z;
		}
		public bool Equals(Vector4w other) {
			return Packed == other.Packed;
		}
		public override string ToString(){
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			var Zs = X == Z ? Xs : Y == Z ? Ys : Z == 0 ? "0" : Z.ToString();
			var Ws = X == W ? Xs : Y == W ? Ys : Z == W ? Zs : W == 0 ? "0" : W.ToString();

			return string.Concat("[",
				string.Concat(Xs, ",", Ys, ","),
				string.Concat(Zs, ",", Ws, "]"));
		}
		public static bool operator ==(Vector4w L, Vector4w R) { return L.Packed == R.Packed; }
		public static bool operator !=(Vector4w L, Vector4w R) { return L.Packed != R.Packed; }
		public static bool operator ==(Vector4w L, ulong R) { return L.Packed == R; }
		public static bool operator !=(Vector4w L, ulong R) { return L.Packed != R; }
		public static bool operator ==(ulong L, Vector4w R) { return L == R.Packed; }
		public static bool operator !=(ulong L, Vector4w R) { return L != R.Packed; }

		public static Vector4w operator &(Vector4w L, Vector4w R) { return new Vector4w { Packed = (ulong)(L.Packed & R.Packed), }; }
		public static Vector4w operator |(Vector4w L, Vector4w R) { return new Vector4w { Packed = (ulong)(L.Packed | R.Packed), }; }
		public static Vector4w operator ^(Vector4w L, Vector4w R) { return new Vector4w { Packed = (ulong)(L.Packed ^ R.Packed), }; }
		public static bool operator true(Vector4w criteria) { return criteria.Packed != 0; }
		public static bool operator false(Vector4w criteria) { return 0 == criteria.Packed; }
		public static Vector4w operator +(Vector4w L, Vector4w R)
		{
			unchecked
			{
				return new Vector4w {
					X = (ushort)(L.X + R.X),
					Y = (ushort)(L.Y + R.Y),
					Z = (ushort)(L.Z + R.Z),
					W = (ushort)(L.W + R.W),
				};
			}
		}
		public static Vector4w operator +(Vector4w L, ushort R)
		{
			unchecked
			{
				return new Vector4w {
					X = (ushort)(L.X + R),
					Y = (ushort)(L.Y + R),
					Z = (ushort)(L.Z + R),
					W = (ushort)(L.W + R),
				};
			}
		}
		public static Vector4w operator +(ushort L, Vector4w R)
		{
			unchecked
			{
				return new Vector4w {
					X = (ushort)(L + R.X),
					Y = (ushort)(L + R.Y),
					Z = (ushort)(L + R.Z),
					W = (ushort)(L + R.W),
				};
			}
		}
		public static Vector4d operator +(Vector4w L, double R){
			return new Vector4d{
				X = (L.X + R),
				Y = (L.Y + R),
				Z = (L.Z + R),
				W = (L.W + R),
			};
		}
		public static Vector4 operator +(Vector4w L, float R){
			return new Vector4{
				X = (L.X + R),
				Y = (L.Y + R),
				Z = (L.Z + R),
				W = (L.W + R),
			};
		}
		public static Vector4d operator +(double L, Vector4w R){
			return new Vector4d{
				X = (L + R.X),
				Y = (L + R.Y),
				Z = (L + R.Z),
				W = (L + R.W),
			};
		}
		public static Vector4 operator +(float L, Vector4w R){
			return new Vector4{
				X = (L + R.X),
				Y = (L + R.Y),
				Z = (L + R.Z),
				W = (L + R.W),
			};
		}
		public static Vector4 operator +(Vector4 L, Vector4w R){
			return new Vector4{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
				W = (L.W + R.W),
			};
		}
		public static Vector4d operator +(Vector4d L, Vector4w R){
			return new Vector4d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
				W = (L.W + R.W),
			};
		}
		public static Vector4 operator +(Vector4w L, Vector4 R){
			return new Vector4{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
				W = (L.W + R.W),
			};
		}
		public static Vector4d operator +(Vector4w L, Vector4d R){
			return new Vector4d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
				W = (L.W + R.W),
			};
		}
		public static Vector4w operator -(Vector4w L, Vector4w R)
		{
			unchecked
			{
				return new Vector4w {
					X = (ushort)(L.X - R.X),
					Y = (ushort)(L.Y - R.Y),
					Z = (ushort)(L.Z - R.Z),
					W = (ushort)(L.W - R.W),
				};
			}
		}
		public static Vector4w operator -(Vector4w L, ushort R)
		{
			unchecked
			{
				return new Vector4w {
					X = (ushort)(L.X - R),
					Y = (ushort)(L.Y - R),
					Z = (ushort)(L.Z - R),
					W = (ushort)(L.W - R),
				};
			}
		}
		public static Vector4w operator -(ushort L, Vector4w R)
		{
			unchecked
			{
				return new Vector4w {
					X = (ushort)(L - R.X),
					Y = (ushort)(L - R.Y),
					Z = (ushort)(L - R.Z),
					W = (ushort)(L - R.W),
				};
			}
		}
		public static Vector4d operator -(Vector4w L, double R){
			return new Vector4d{
				X = (L.X - R),
				Y = (L.Y - R),
				Z = (L.Z - R),
				W = (L.W - R),
			};
		}
		public static Vector4 operator -(Vector4w L, float R){
			return new Vector4{
				X = (L.X - R),
				Y = (L.Y - R),
				Z = (L.Z - R),
				W = (L.W - R),
			};
		}
		public static Vector4d operator -(double L, Vector4w R){
			return new Vector4d{
				X = (L - R.X),
				Y = (L - R.Y),
				Z = (L - R.Z),
				W = (L - R.W),
			};
		}
		public static Vector4 operator -(float L, Vector4w R){
			return new Vector4{
				X = (L - R.X),
				Y = (L - R.Y),
				Z = (L - R.Z),
				W = (L - R.W),
			};
		}
		public static Vector4 operator -(Vector4 L, Vector4w R){
			return new Vector4{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
				W = (L.W - R.W),
			};
		}
		public static Vector4d operator -(Vector4d L, Vector4w R){
			return new Vector4d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
				W = (L.W - R.W),
			};
		}
		public static Vector4 operator -(Vector4w L, Vector4 R){
			return new Vector4{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
				W = (L.W - R.W),
			};
		}
		public static Vector4d operator -(Vector4w L, Vector4d R){
			return new Vector4d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
				W = (L.W - R.W),
			};
		}
		public static Vector4w operator *(Vector4w L, Vector4w R)
		{
			unchecked
			{
				return new Vector4w {
					X = (ushort)(L.X * R.X),
					Y = (ushort)(L.Y * R.Y),
					Z = (ushort)(L.Z * R.Z),
					W = (ushort)(L.W * R.W),
				};
			}
		}
		public static Vector4w operator *(Vector4w L, ushort R)
		{
			unchecked
			{
				return new Vector4w {
					X = (ushort)(L.X * R),
					Y = (ushort)(L.Y * R),
					Z = (ushort)(L.Z * R),
					W = (ushort)(L.W * R),
				};
			}
		}
		public static Vector4w operator *(ushort L, Vector4w R)
		{
			unchecked
			{
				return new Vector4w {
					X = (ushort)(L * R.X),
					Y = (ushort)(L * R.Y),
					Z = (ushort)(L * R.Z),
					W = (ushort)(L * R.W),
				};
			}
		}
		public static Vector4d operator *(Vector4w L, double R){
			return new Vector4d{
				X = (L.X * R),
				Y = (L.Y * R),
				Z = (L.Z * R),
				W = (L.W * R),
			};
		}
		public static Vector4 operator *(Vector4w L, float R){
			return new Vector4{
				X = (L.X * R),
				Y = (L.Y * R),
				Z = (L.Z * R),
				W = (L.W * R),
			};
		}
		public static Vector4d operator *(double L, Vector4w R){
			return new Vector4d{
				X = (L * R.X),
				Y = (L * R.Y),
				Z = (L * R.Z),
				W = (L * R.W),
			};
		}
		public static Vector4 operator *(float L, Vector4w R){
			return new Vector4{
				X = (L * R.X),
				Y = (L * R.Y),
				Z = (L * R.Z),
				W = (L * R.W),
			};
		}
		public static Vector4 operator *(Vector4 L, Vector4w R){
			return new Vector4{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
				W = (L.W * R.W),
			};
		}
		public static Vector4d operator *(Vector4d L, Vector4w R){
			return new Vector4d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
				W = (L.W * R.W),
			};
		}
		public static Vector4 operator *(Vector4w L, Vector4 R){
			return new Vector4{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
				W = (L.W * R.W),
			};
		}
		public static Vector4d operator *(Vector4w L, Vector4d R){
			return new Vector4d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
				W = (L.W * R.W),
			};
		}
		public static Vector4w operator /(Vector4w L, Vector4w R)
		{
			unchecked
			{
				return new Vector4w {
					X = (ushort)(L.X / R.X),
					Y = (ushort)(L.Y / R.Y),
					Z = (ushort)(L.Z / R.Z),
					W = (ushort)(L.W / R.W),
				};
			}
		}
		public static Vector4w operator /(Vector4w L, ushort R)
		{
			unchecked
			{
				return new Vector4w {
					X = (ushort)(L.X / R),
					Y = (ushort)(L.Y / R),
					Z = (ushort)(L.Z / R),
					W = (ushort)(L.W / R),
				};
			}
		}
		public static Vector4w operator /(ushort L, Vector4w R)
		{
			unchecked
			{
				return new Vector4w {
					X = (ushort)(L / R.X),
					Y = (ushort)(L / R.Y),
					Z = (ushort)(L / R.Z),
					W = (ushort)(L / R.W),
				};
			}
		}
		public static Vector4d operator /(Vector4w L, double R){
			return new Vector4d{
				X = (L.X / R),
				Y = (L.Y / R),
				Z = (L.Z / R),
				W = (L.W / R),
			};
		}
		public static Vector4 operator /(Vector4w L, float R){
			return new Vector4{
				X = (L.X / R),
				Y = (L.Y / R),
				Z = (L.Z / R),
				W = (L.W / R),
			};
		}
		public static Vector4d operator /(double L, Vector4w R){
			return new Vector4d{
				X = (L / R.X),
				Y = (L / R.Y),
				Z = (L / R.Z),
				W = (L / R.W),
			};
		}
		public static Vector4 operator /(float L, Vector4w R){
			return new Vector4{
				X = (L / R.X),
				Y = (L / R.Y),
				Z = (L / R.Z),
				W = (L / R.W),
			};
		}
		public static Vector4 operator /(Vector4 L, Vector4w R){
			return new Vector4{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
				W = (L.W / R.W),
			};
		}
		public static Vector4d operator /(Vector4d L, Vector4w R){
			return new Vector4d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
				W = (L.W / R.W),
			};
		}
		public static Vector4 operator /(Vector4w L, Vector4 R){
			return new Vector4{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
				W = (L.W / R.W),
			};
		}
		public static Vector4d operator /(Vector4w L, Vector4d R){
			return new Vector4d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
				W = (L.W / R.W),
			};
		}
		public static explicit operator Vector4(Vector4w value)
		{
			return new Vector4{X = value.X,Y = value.Y,Z = value.Z,W = value.W,};
		}
		public static explicit operator Vector4d(Vector4w value)
		{
			return new Vector4d{X = value.X,Y = value.Y,Z = value.Z,W = value.W,};
		}
		public static explicit operator Vector4w(Vector4 value)
		{
			return new Vector4w{X = (ushort)value.X,Y = (ushort)value.Y,Z = (ushort)value.Z,W = (ushort)value.W,};
		}
		public static explicit operator Vector4w(Vector4d value)
		{
			return new Vector4w{X = (ushort)value.X,Y = (ushort)value.Y,Z = (ushort)value.Z,W = (ushort)value.W,};
		}
		public override int GetHashCode(){return GetHashCode(ref this);}
		public static int GetHashCode(Vector4w Value){ 
			// IF YOU GET AN ERROR ON THE LINE BELOW, ITS BECAUSE YOU DID NOT IMPLEMENT
			// IT OUTSIDE OF THIS PARTIAL GENERATED CODE.
			//
			// NOTE THAT THE ARGUMENT MUST BE NAMED VALUE!
			return GetHashCode(Value:ref Value);
		}
		public override bool Equals(object obj)
		{
			return obj is Vector4w && Packed == ((Vector4w)obj).Packed;
		}
		public static explicit operator Vector4w(ulong value) {
			return new Vector4w { Packed = value, };
		}
	}
	///<summary>A 3 dimension vector with elements of type ushort.</summary>
	[StructLayout(LayoutKind.Sequential, Size = 6, Pack = 2)]
	public partial struct Vector3w
		: IEquatable<Vector3w>, IEquatable<Vector4w>
	{
		public ushort X;
		public ushort Y;
		public ushort Z;
		public ulong Packed {
			get { return new Vector4w { X = X, Y = Y, Z = Z, }.Packed; }
			set {
				var V4 = new Vector4w { Packed = value, };
				this.X = V4.X;
				this.Y = V4.Y;
				this.Z = V4.Z;
			}
		}
		public ushort this[int axis] {
			get {
				return 0 == (axis & 2) ? 0 == (axis & 1) ? X : Y : 0 == (axis & 1) ? Z : (ushort)0;
			}
			set	{
				if (0 == (axis & 2))
					if (0 == (axis & 1))
						X = value;
					else
						Y = value;
				else
					if (0 == (axis & 1))
					Z = value;
			}
		}
		public bool Equals(Vector3w other) {
			return X == other.X && Y == other.Y && Z == other.Z;
		}
		public bool Equals(Vector4w other) {
			return 0 == other.W && X == other.X && Y == other.Y && Z == other.Z;
		}
		public override string ToString() {
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			var Zs = X == Z ? Xs : Y == Z ? Ys : Z == 0 ? "0" : Z.ToString();

			return string.Concat("[",
				string.Concat(Xs, ",", Ys, ","),
				Zs, "]");
		}
		public static bool operator ==(Vector3w L, Vector3w R) { return L.X == R.X && L.Y == R.Y && L.Z == R.Z; }
		public static bool operator !=(Vector3w L, Vector3w R) { return L.X != R.X || L.Y != R.Y || L.Z != R.Z; }
		public static bool operator ==(Vector3w L, ulong R) { return L.Packed == R; }
		public static bool operator !=(Vector3w L, ulong R) { return L.Packed != R; }
		public static bool operator ==(ulong L, Vector3w R) { return L == R.Packed; }
		public static bool operator !=(ulong L, Vector3w R) { return L != R.Packed; }

		public static Vector3w operator &(Vector3w L, Vector3w R) { return new Vector3w { Packed = (ulong)(L.Packed & R.Packed), }; }
		public static Vector3w operator |(Vector3w L, Vector3w R) { return new Vector3w { Packed = (ulong)(L.Packed | R.Packed), }; }
		public static Vector3w operator ^(Vector3w L, Vector3w R) { return new Vector3w { Packed = (ulong)(L.Packed ^ R.Packed), }; }
		public static bool operator true(Vector3w criteria) { return criteria.X != 0 || criteria.Y != 0 || criteria.Z != 0; }
		public static bool operator false(Vector3w criteria) { return 0 == criteria.X && 0 == criteria.Y && 0 == criteria.Z; }
		public static Vector3w operator +(Vector3w L, Vector3w R)
		{
			unchecked
			{
				return new Vector3w {
					X = (ushort)(L.X + R.X),
					Y = (ushort)(L.Y + R.Y),
					Z = (ushort)(L.Z + R.Z),
				};
			}
		}
		public static Vector3w operator +(Vector3w L, ushort R)
		{
			unchecked
			{
				return new Vector3w {
					X = (ushort)(L.X + R),
					Y = (ushort)(L.Y + R),
					Z = (ushort)(L.Z + R),
				};
			}
		}
		public static Vector3w operator +(ushort L, Vector3w R)
		{
			unchecked
			{
				return new Vector3w {
					X = (ushort)(L + R.X),
					Y = (ushort)(L + R.Y),
					Z = (ushort)(L + R.Z),
				};
			}
		}
		public static Vector3d operator +(Vector3w L, double R){
			return new Vector3d{
				X = (L.X + R),
				Y = (L.Y + R),
				Z = (L.Z + R),
			};
		}
		public static Vector3 operator +(Vector3w L, float R){
			return new Vector3{
				X = (L.X + R),
				Y = (L.Y + R),
				Z = (L.Z + R),
			};
		}
		public static Vector3d operator +(double L, Vector3w R){
			return new Vector3d{
				X = (L + R.X),
				Y = (L + R.Y),
				Z = (L + R.Z),
			};
		}
		public static Vector3 operator +(float L, Vector3w R){
			return new Vector3{
				X = (L + R.X),
				Y = (L + R.Y),
				Z = (L + R.Z),
			};
		}
		public static Vector3 operator +(Vector3 L, Vector3w R){
			return new Vector3{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
			};
		}
		public static Vector3d operator +(Vector3d L, Vector3w R){
			return new Vector3d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
			};
		}
		public static Vector3 operator +(Vector3w L, Vector3 R){
			return new Vector3{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
			};
		}
		public static Vector3d operator +(Vector3w L, Vector3d R){
			return new Vector3d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
			};
		}
		public static Vector3w operator -(Vector3w L, Vector3w R)
		{
			unchecked
			{
				return new Vector3w {
					X = (ushort)(L.X - R.X),
					Y = (ushort)(L.Y - R.Y),
					Z = (ushort)(L.Z - R.Z),
				};
			}
		}
		public static Vector3w operator -(Vector3w L, ushort R)
		{
			unchecked
			{
				return new Vector3w {
					X = (ushort)(L.X - R),
					Y = (ushort)(L.Y - R),
					Z = (ushort)(L.Z - R),
				};
			}
		}
		public static Vector3w operator -(ushort L, Vector3w R)
		{
			unchecked
			{
				return new Vector3w {
					X = (ushort)(L - R.X),
					Y = (ushort)(L - R.Y),
					Z = (ushort)(L - R.Z),
				};
			}
		}
		public static Vector3d operator -(Vector3w L, double R){
			return new Vector3d{
				X = (L.X - R),
				Y = (L.Y - R),
				Z = (L.Z - R),
			};
		}
		public static Vector3 operator -(Vector3w L, float R){
			return new Vector3{
				X = (L.X - R),
				Y = (L.Y - R),
				Z = (L.Z - R),
			};
		}
		public static Vector3d operator -(double L, Vector3w R){
			return new Vector3d{
				X = (L - R.X),
				Y = (L - R.Y),
				Z = (L - R.Z),
			};
		}
		public static Vector3 operator -(float L, Vector3w R){
			return new Vector3{
				X = (L - R.X),
				Y = (L - R.Y),
				Z = (L - R.Z),
			};
		}
		public static Vector3 operator -(Vector3 L, Vector3w R){
			return new Vector3{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
			};
		}
		public static Vector3d operator -(Vector3d L, Vector3w R){
			return new Vector3d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
			};
		}
		public static Vector3 operator -(Vector3w L, Vector3 R){
			return new Vector3{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
			};
		}
		public static Vector3d operator -(Vector3w L, Vector3d R){
			return new Vector3d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
			};
		}
		public static Vector3w operator *(Vector3w L, Vector3w R)
		{
			unchecked
			{
				return new Vector3w {
					X = (ushort)(L.X * R.X),
					Y = (ushort)(L.Y * R.Y),
					Z = (ushort)(L.Z * R.Z),
				};
			}
		}
		public static Vector3w operator *(Vector3w L, ushort R)
		{
			unchecked
			{
				return new Vector3w {
					X = (ushort)(L.X * R),
					Y = (ushort)(L.Y * R),
					Z = (ushort)(L.Z * R),
				};
			}
		}
		public static Vector3w operator *(ushort L, Vector3w R)
		{
			unchecked
			{
				return new Vector3w {
					X = (ushort)(L * R.X),
					Y = (ushort)(L * R.Y),
					Z = (ushort)(L * R.Z),
				};
			}
		}
		public static Vector3d operator *(Vector3w L, double R){
			return new Vector3d{
				X = (L.X * R),
				Y = (L.Y * R),
				Z = (L.Z * R),
			};
		}
		public static Vector3 operator *(Vector3w L, float R){
			return new Vector3{
				X = (L.X * R),
				Y = (L.Y * R),
				Z = (L.Z * R),
			};
		}
		public static Vector3d operator *(double L, Vector3w R){
			return new Vector3d{
				X = (L * R.X),
				Y = (L * R.Y),
				Z = (L * R.Z),
			};
		}
		public static Vector3 operator *(float L, Vector3w R){
			return new Vector3{
				X = (L * R.X),
				Y = (L * R.Y),
				Z = (L * R.Z),
			};
		}
		public static Vector3 operator *(Vector3 L, Vector3w R){
			return new Vector3{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
			};
		}
		public static Vector3d operator *(Vector3d L, Vector3w R){
			return new Vector3d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
			};
		}
		public static Vector3 operator *(Vector3w L, Vector3 R){
			return new Vector3{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
			};
		}
		public static Vector3d operator *(Vector3w L, Vector3d R){
			return new Vector3d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
			};
		}
		public static Vector3w operator /(Vector3w L, Vector3w R)
		{
			unchecked
			{
				return new Vector3w {
					X = (ushort)(L.X / R.X),
					Y = (ushort)(L.Y / R.Y),
					Z = (ushort)(L.Z / R.Z),
				};
			}
		}
		public static Vector3w operator /(Vector3w L, ushort R)
		{
			unchecked
			{
				return new Vector3w {
					X = (ushort)(L.X / R),
					Y = (ushort)(L.Y / R),
					Z = (ushort)(L.Z / R),
				};
			}
		}
		public static Vector3w operator /(ushort L, Vector3w R)
		{
			unchecked
			{
				return new Vector3w {
					X = (ushort)(L / R.X),
					Y = (ushort)(L / R.Y),
					Z = (ushort)(L / R.Z),
				};
			}
		}
		public static Vector3d operator /(Vector3w L, double R){
			return new Vector3d{
				X = (L.X / R),
				Y = (L.Y / R),
				Z = (L.Z / R),
			};
		}
		public static Vector3 operator /(Vector3w L, float R){
			return new Vector3{
				X = (L.X / R),
				Y = (L.Y / R),
				Z = (L.Z / R),
			};
		}
		public static Vector3d operator /(double L, Vector3w R){
			return new Vector3d{
				X = (L / R.X),
				Y = (L / R.Y),
				Z = (L / R.Z),
			};
		}
		public static Vector3 operator /(float L, Vector3w R){
			return new Vector3{
				X = (L / R.X),
				Y = (L / R.Y),
				Z = (L / R.Z),
			};
		}
		public static Vector3 operator /(Vector3 L, Vector3w R){
			return new Vector3{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
			};
		}
		public static Vector3d operator /(Vector3d L, Vector3w R){
			return new Vector3d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
			};
		}
		public static Vector3 operator /(Vector3w L, Vector3 R){
			return new Vector3{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
			};
		}
		public static Vector3d operator /(Vector3w L, Vector3d R){
			return new Vector3d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
			};
		}
		public static explicit operator Vector3(Vector3w value)
		{
			return new Vector3{X = value.X,Y = value.Y,Z = value.Z,};
		}
		public static explicit operator Vector3d(Vector3w value)
		{
			return new Vector3d{X = value.X,Y = value.Y,Z = value.Z,};
		}
		public static explicit operator Vector3w(Vector3 value)
		{
			return new Vector3w{X = (ushort)value.X,Y = (ushort)value.Y,Z = (ushort)value.Z,};
		}
		public static explicit operator Vector3w(Vector3d value)
		{
			return new Vector3w{X = (ushort)value.X,Y = (ushort)value.Y,Z = (ushort)value.Z,};
		}
		public override int GetHashCode() {
			return Vector4w.GetHashCode(new Vector4w { X=X,Y=Y,Z=Z,});
		}
		public override bool Equals(object obj)
		{
			return obj is Vector3w && Packed == ((Vector3w)obj).Packed;
		}
		public static explicit operator Vector3w(ulong value) {
			return new Vector3w { Packed = value, };
		}
	}
	///<summary>A 2 dimension vector with elements of type ushort.</summary>
	[StructLayout(LayoutKind.Explicit, Size = sizeof(uint))]
	public partial struct Vector2w
		: IEquatable<Vector2w>
	{
		[FieldOffset(sizeof(ushort) * 1)]
		public ushort X;
		[FieldOffset(sizeof(ushort) * 0)]
		public ushort Y;
		[FieldOffset(0)]
		public uint Packed;
		public ushort this[int axis]{
			get	{
				return 0 == (axis & 1) ? X : Y;
			}
			set	{
				if (0 == (axis & 1))
					X = value;
				else
					Y = value;
			}
		}
		public bool Equals(Vector2w other){
			return Packed == other.Packed;
		}

		public override string ToString(){
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			return string.Concat("[",
				string.Concat(Xs, ",", Ys, "]"));
		}
		public static bool operator ==(Vector2w L, Vector2w R) { return L.Packed == R.Packed; }
		public static bool operator !=(Vector2w L, Vector2w R) { return L.Packed != R.Packed; }
		public static bool operator ==(Vector2w L, uint R) { return L.Packed == R; }
		public static bool operator !=(Vector2w L, uint R) { return L.Packed != R; }
		public static bool operator ==(uint L, Vector2w R) { return L == R.Packed; }
		public static bool operator !=(uint L, Vector2w R) { return L != R.Packed; }

		public static Vector2w operator &(Vector2w L, Vector2w R) { return new Vector2w { Packed = (uint)(L.Packed & R.Packed), }; }
		public static Vector2w operator |(Vector2w L, Vector2w R) { return new Vector2w { Packed = (uint)(L.Packed | R.Packed), }; }
		public static Vector2w operator ^(Vector2w L, Vector2w R) { return new Vector2w { Packed = (uint)(L.Packed ^ R.Packed), }; }
		public static bool operator true(Vector2w criteria) { return criteria.Packed != 0; }
		public static bool operator false(Vector2w criteria) { return 0 == criteria.Packed; }
		public static Vector2w operator +(Vector2w L, Vector2w R)
		{
			unchecked
			{
				return new Vector2w {
					X = (ushort)(L.X + R.X),
					Y = (ushort)(L.Y + R.Y),
				};
			}
		}
		public static Vector2w operator +(Vector2w L, ushort R)
		{
			unchecked
			{
				return new Vector2w {
					X = (ushort)(L.X + R),
					Y = (ushort)(L.Y + R),
				};
			}
		}
		public static Vector2w operator +(ushort L, Vector2w R)
		{
			unchecked
			{
				return new Vector2w {
					X = (ushort)(L + R.X),
					Y = (ushort)(L + R.Y),
				};
			}
		}
		public static Vector2d operator +(Vector2w L, double R){
			return new Vector2d{
				X = (L.X + R),
				Y = (L.Y + R),
			};
		}
		public static Vector2 operator +(Vector2w L, float R){
			return new Vector2{
				X = (L.X + R),
				Y = (L.Y + R),
			};
		}
		public static Vector2d operator +(double L, Vector2w R){
			return new Vector2d{
				X = (L + R.X),
				Y = (L + R.Y),
			};
		}
		public static Vector2 operator +(float L, Vector2w R){
			return new Vector2{
				X = (L + R.X),
				Y = (L + R.Y),
			};
		}
		public static Vector2 operator +(Vector2 L, Vector2w R){
			return new Vector2{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2d operator +(Vector2d L, Vector2w R){
			return new Vector2d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2 operator +(Vector2w L, Vector2 R){
			return new Vector2{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2d operator +(Vector2w L, Vector2d R){
			return new Vector2d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2w operator -(Vector2w L, Vector2w R)
		{
			unchecked
			{
				return new Vector2w {
					X = (ushort)(L.X - R.X),
					Y = (ushort)(L.Y - R.Y),
				};
			}
		}
		public static Vector2w operator -(Vector2w L, ushort R)
		{
			unchecked
			{
				return new Vector2w {
					X = (ushort)(L.X - R),
					Y = (ushort)(L.Y - R),
				};
			}
		}
		public static Vector2w operator -(ushort L, Vector2w R)
		{
			unchecked
			{
				return new Vector2w {
					X = (ushort)(L - R.X),
					Y = (ushort)(L - R.Y),
				};
			}
		}
		public static Vector2d operator -(Vector2w L, double R){
			return new Vector2d{
				X = (L.X - R),
				Y = (L.Y - R),
			};
		}
		public static Vector2 operator -(Vector2w L, float R){
			return new Vector2{
				X = (L.X - R),
				Y = (L.Y - R),
			};
		}
		public static Vector2d operator -(double L, Vector2w R){
			return new Vector2d{
				X = (L - R.X),
				Y = (L - R.Y),
			};
		}
		public static Vector2 operator -(float L, Vector2w R){
			return new Vector2{
				X = (L - R.X),
				Y = (L - R.Y),
			};
		}
		public static Vector2 operator -(Vector2 L, Vector2w R){
			return new Vector2{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2d operator -(Vector2d L, Vector2w R){
			return new Vector2d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2 operator -(Vector2w L, Vector2 R){
			return new Vector2{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2d operator -(Vector2w L, Vector2d R){
			return new Vector2d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2w operator *(Vector2w L, Vector2w R)
		{
			unchecked
			{
				return new Vector2w {
					X = (ushort)(L.X * R.X),
					Y = (ushort)(L.Y * R.Y),
				};
			}
		}
		public static Vector2w operator *(Vector2w L, ushort R)
		{
			unchecked
			{
				return new Vector2w {
					X = (ushort)(L.X * R),
					Y = (ushort)(L.Y * R),
				};
			}
		}
		public static Vector2w operator *(ushort L, Vector2w R)
		{
			unchecked
			{
				return new Vector2w {
					X = (ushort)(L * R.X),
					Y = (ushort)(L * R.Y),
				};
			}
		}
		public static Vector2d operator *(Vector2w L, double R){
			return new Vector2d{
				X = (L.X * R),
				Y = (L.Y * R),
			};
		}
		public static Vector2 operator *(Vector2w L, float R){
			return new Vector2{
				X = (L.X * R),
				Y = (L.Y * R),
			};
		}
		public static Vector2d operator *(double L, Vector2w R){
			return new Vector2d{
				X = (L * R.X),
				Y = (L * R.Y),
			};
		}
		public static Vector2 operator *(float L, Vector2w R){
			return new Vector2{
				X = (L * R.X),
				Y = (L * R.Y),
			};
		}
		public static Vector2 operator *(Vector2 L, Vector2w R){
			return new Vector2{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2d operator *(Vector2d L, Vector2w R){
			return new Vector2d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2 operator *(Vector2w L, Vector2 R){
			return new Vector2{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2d operator *(Vector2w L, Vector2d R){
			return new Vector2d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2w operator /(Vector2w L, Vector2w R)
		{
			unchecked
			{
				return new Vector2w {
					X = (ushort)(L.X / R.X),
					Y = (ushort)(L.Y / R.Y),
				};
			}
		}
		public static Vector2w operator /(Vector2w L, ushort R)
		{
			unchecked
			{
				return new Vector2w {
					X = (ushort)(L.X / R),
					Y = (ushort)(L.Y / R),
				};
			}
		}
		public static Vector2w operator /(ushort L, Vector2w R)
		{
			unchecked
			{
				return new Vector2w {
					X = (ushort)(L / R.X),
					Y = (ushort)(L / R.Y),
				};
			}
		}
		public static Vector2d operator /(Vector2w L, double R){
			return new Vector2d{
				X = (L.X / R),
				Y = (L.Y / R),
			};
		}
		public static Vector2 operator /(Vector2w L, float R){
			return new Vector2{
				X = (L.X / R),
				Y = (L.Y / R),
			};
		}
		public static Vector2d operator /(double L, Vector2w R){
			return new Vector2d{
				X = (L / R.X),
				Y = (L / R.Y),
			};
		}
		public static Vector2 operator /(float L, Vector2w R){
			return new Vector2{
				X = (L / R.X),
				Y = (L / R.Y),
			};
		}
		public static Vector2 operator /(Vector2 L, Vector2w R){
			return new Vector2{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static Vector2d operator /(Vector2d L, Vector2w R){
			return new Vector2d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static Vector2 operator /(Vector2w L, Vector2 R){
			return new Vector2{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static Vector2d operator /(Vector2w L, Vector2d R){
			return new Vector2d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static explicit operator Vector2(Vector2w value)
		{
			return new Vector2{X = value.X,Y = value.Y,};
		}
		public static explicit operator Vector2d(Vector2w value)
		{
			return new Vector2d{X = value.X,Y = value.Y,};
		}
		public static explicit operator Vector2w(Vector2 value)
		{
			return new Vector2w{X = (ushort)value.X,Y = (ushort)value.Y,};
		}
		public static explicit operator Vector2w(Vector2d value)
		{
			return new Vector2w{X = (ushort)value.X,Y = (ushort)value.Y,};
		}
		public override int GetHashCode() {
			return Vector4w.GetHashCode(new Vector4w { X=X,Y=Y,});
		}
		public override bool Equals(object obj)
		{
			return obj is Vector2w && Packed == ((Vector2w)obj).Packed;
		}
		public static explicit operator Vector2w(uint value) {
			return new Vector2w { Packed = value, };
		}
	}
	///<summary>A 4 dimension vector with elements of type short.</summary>
	[StructLayout(LayoutKind.Explicit, Size = sizeof(ulong))]
	public partial struct Vector4s
		: IEquatable<Vector4s>, IEquatable<Vector3s>
	{
		[FieldOffset(sizeof(short) * 3)]
		public short X;
		[FieldOffset(sizeof(short) * 2)]
		public short Y;
		[FieldOffset(sizeof(short) * 1)]
		public short Z;
		[FieldOffset(sizeof(short) * 0)]
		public short W;
		[FieldOffset(0)]
		public ulong Packed;
		public short this[int axis]{
			get	{
				return 0 == (axis & 2) ? 0 == (axis & 1) ? X : Y : 0 == (axis & 1) ? Z : W;
			}
			set	{
				if (0 == (axis & 2))
					if (0 == (axis & 1))
						X = value;
					else
						Y = value;
				else
					if (0 == (axis & 1))
					Z = value;
				else
					W = value;
			}
		}
		
		public bool Equals(Vector3s other) {
			return W == 0 && X == other.X && Y == other.Y && Z == other.Z;
		}
		public bool Equals(Vector4s other) {
			return Packed == other.Packed;
		}
		public override string ToString(){
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			var Zs = X == Z ? Xs : Y == Z ? Ys : Z == 0 ? "0" : Z.ToString();
			var Ws = X == W ? Xs : Y == W ? Ys : Z == W ? Zs : W == 0 ? "0" : W.ToString();

			return string.Concat("[",
				string.Concat(Xs, ",", Ys, ","),
				string.Concat(Zs, ",", Ws, "]"));
		}
		public static bool operator ==(Vector4s L, Vector4s R) { return L.Packed == R.Packed; }
		public static bool operator !=(Vector4s L, Vector4s R) { return L.Packed != R.Packed; }
		public static bool operator ==(Vector4s L, ulong R) { return L.Packed == R; }
		public static bool operator !=(Vector4s L, ulong R) { return L.Packed != R; }
		public static bool operator ==(ulong L, Vector4s R) { return L == R.Packed; }
		public static bool operator !=(ulong L, Vector4s R) { return L != R.Packed; }

		public static Vector4s operator &(Vector4s L, Vector4s R) { return new Vector4s { Packed = (ulong)(L.Packed & R.Packed), }; }
		public static Vector4s operator |(Vector4s L, Vector4s R) { return new Vector4s { Packed = (ulong)(L.Packed | R.Packed), }; }
		public static Vector4s operator ^(Vector4s L, Vector4s R) { return new Vector4s { Packed = (ulong)(L.Packed ^ R.Packed), }; }
		public static bool operator true(Vector4s criteria) { return criteria.Packed != 0; }
		public static bool operator false(Vector4s criteria) { return 0 == criteria.Packed; }
		public static Vector4s operator +(Vector4s L, Vector4s R)
		{
			unchecked
			{
				return new Vector4s {
					X = (short)(L.X + R.X),
					Y = (short)(L.Y + R.Y),
					Z = (short)(L.Z + R.Z),
					W = (short)(L.W + R.W),
				};
			}
		}
		public static Vector4s operator +(Vector4s L, short R)
		{
			unchecked
			{
				return new Vector4s {
					X = (short)(L.X + R),
					Y = (short)(L.Y + R),
					Z = (short)(L.Z + R),
					W = (short)(L.W + R),
				};
			}
		}
		public static Vector4s operator +(short L, Vector4s R)
		{
			unchecked
			{
				return new Vector4s {
					X = (short)(L + R.X),
					Y = (short)(L + R.Y),
					Z = (short)(L + R.Z),
					W = (short)(L + R.W),
				};
			}
		}
		public static Vector4d operator +(Vector4s L, double R){
			return new Vector4d{
				X = (L.X + R),
				Y = (L.Y + R),
				Z = (L.Z + R),
				W = (L.W + R),
			};
		}
		public static Vector4 operator +(Vector4s L, float R){
			return new Vector4{
				X = (L.X + R),
				Y = (L.Y + R),
				Z = (L.Z + R),
				W = (L.W + R),
			};
		}
		public static Vector4d operator +(double L, Vector4s R){
			return new Vector4d{
				X = (L + R.X),
				Y = (L + R.Y),
				Z = (L + R.Z),
				W = (L + R.W),
			};
		}
		public static Vector4 operator +(float L, Vector4s R){
			return new Vector4{
				X = (L + R.X),
				Y = (L + R.Y),
				Z = (L + R.Z),
				W = (L + R.W),
			};
		}
		public static Vector4 operator +(Vector4 L, Vector4s R){
			return new Vector4{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
				W = (L.W + R.W),
			};
		}
		public static Vector4d operator +(Vector4d L, Vector4s R){
			return new Vector4d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
				W = (L.W + R.W),
			};
		}
		public static Vector4 operator +(Vector4s L, Vector4 R){
			return new Vector4{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
				W = (L.W + R.W),
			};
		}
		public static Vector4d operator +(Vector4s L, Vector4d R){
			return new Vector4d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
				W = (L.W + R.W),
			};
		}
		public static Vector4s operator -(Vector4s L, Vector4s R)
		{
			unchecked
			{
				return new Vector4s {
					X = (short)(L.X - R.X),
					Y = (short)(L.Y - R.Y),
					Z = (short)(L.Z - R.Z),
					W = (short)(L.W - R.W),
				};
			}
		}
		public static Vector4s operator -(Vector4s L, short R)
		{
			unchecked
			{
				return new Vector4s {
					X = (short)(L.X - R),
					Y = (short)(L.Y - R),
					Z = (short)(L.Z - R),
					W = (short)(L.W - R),
				};
			}
		}
		public static Vector4s operator -(short L, Vector4s R)
		{
			unchecked
			{
				return new Vector4s {
					X = (short)(L - R.X),
					Y = (short)(L - R.Y),
					Z = (short)(L - R.Z),
					W = (short)(L - R.W),
				};
			}
		}
		public static Vector4d operator -(Vector4s L, double R){
			return new Vector4d{
				X = (L.X - R),
				Y = (L.Y - R),
				Z = (L.Z - R),
				W = (L.W - R),
			};
		}
		public static Vector4 operator -(Vector4s L, float R){
			return new Vector4{
				X = (L.X - R),
				Y = (L.Y - R),
				Z = (L.Z - R),
				W = (L.W - R),
			};
		}
		public static Vector4d operator -(double L, Vector4s R){
			return new Vector4d{
				X = (L - R.X),
				Y = (L - R.Y),
				Z = (L - R.Z),
				W = (L - R.W),
			};
		}
		public static Vector4 operator -(float L, Vector4s R){
			return new Vector4{
				X = (L - R.X),
				Y = (L - R.Y),
				Z = (L - R.Z),
				W = (L - R.W),
			};
		}
		public static Vector4 operator -(Vector4 L, Vector4s R){
			return new Vector4{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
				W = (L.W - R.W),
			};
		}
		public static Vector4d operator -(Vector4d L, Vector4s R){
			return new Vector4d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
				W = (L.W - R.W),
			};
		}
		public static Vector4 operator -(Vector4s L, Vector4 R){
			return new Vector4{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
				W = (L.W - R.W),
			};
		}
		public static Vector4d operator -(Vector4s L, Vector4d R){
			return new Vector4d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
				W = (L.W - R.W),
			};
		}
		public static Vector4s operator *(Vector4s L, Vector4s R)
		{
			unchecked
			{
				return new Vector4s {
					X = (short)(L.X * R.X),
					Y = (short)(L.Y * R.Y),
					Z = (short)(L.Z * R.Z),
					W = (short)(L.W * R.W),
				};
			}
		}
		public static Vector4s operator *(Vector4s L, short R)
		{
			unchecked
			{
				return new Vector4s {
					X = (short)(L.X * R),
					Y = (short)(L.Y * R),
					Z = (short)(L.Z * R),
					W = (short)(L.W * R),
				};
			}
		}
		public static Vector4s operator *(short L, Vector4s R)
		{
			unchecked
			{
				return new Vector4s {
					X = (short)(L * R.X),
					Y = (short)(L * R.Y),
					Z = (short)(L * R.Z),
					W = (short)(L * R.W),
				};
			}
		}
		public static Vector4d operator *(Vector4s L, double R){
			return new Vector4d{
				X = (L.X * R),
				Y = (L.Y * R),
				Z = (L.Z * R),
				W = (L.W * R),
			};
		}
		public static Vector4 operator *(Vector4s L, float R){
			return new Vector4{
				X = (L.X * R),
				Y = (L.Y * R),
				Z = (L.Z * R),
				W = (L.W * R),
			};
		}
		public static Vector4d operator *(double L, Vector4s R){
			return new Vector4d{
				X = (L * R.X),
				Y = (L * R.Y),
				Z = (L * R.Z),
				W = (L * R.W),
			};
		}
		public static Vector4 operator *(float L, Vector4s R){
			return new Vector4{
				X = (L * R.X),
				Y = (L * R.Y),
				Z = (L * R.Z),
				W = (L * R.W),
			};
		}
		public static Vector4 operator *(Vector4 L, Vector4s R){
			return new Vector4{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
				W = (L.W * R.W),
			};
		}
		public static Vector4d operator *(Vector4d L, Vector4s R){
			return new Vector4d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
				W = (L.W * R.W),
			};
		}
		public static Vector4 operator *(Vector4s L, Vector4 R){
			return new Vector4{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
				W = (L.W * R.W),
			};
		}
		public static Vector4d operator *(Vector4s L, Vector4d R){
			return new Vector4d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
				W = (L.W * R.W),
			};
		}
		public static Vector4s operator /(Vector4s L, Vector4s R)
		{
			unchecked
			{
				return new Vector4s {
					X = (short)(L.X / R.X),
					Y = (short)(L.Y / R.Y),
					Z = (short)(L.Z / R.Z),
					W = (short)(L.W / R.W),
				};
			}
		}
		public static Vector4s operator /(Vector4s L, short R)
		{
			unchecked
			{
				return new Vector4s {
					X = (short)(L.X / R),
					Y = (short)(L.Y / R),
					Z = (short)(L.Z / R),
					W = (short)(L.W / R),
				};
			}
		}
		public static Vector4s operator /(short L, Vector4s R)
		{
			unchecked
			{
				return new Vector4s {
					X = (short)(L / R.X),
					Y = (short)(L / R.Y),
					Z = (short)(L / R.Z),
					W = (short)(L / R.W),
				};
			}
		}
		public static Vector4d operator /(Vector4s L, double R){
			return new Vector4d{
				X = (L.X / R),
				Y = (L.Y / R),
				Z = (L.Z / R),
				W = (L.W / R),
			};
		}
		public static Vector4 operator /(Vector4s L, float R){
			return new Vector4{
				X = (L.X / R),
				Y = (L.Y / R),
				Z = (L.Z / R),
				W = (L.W / R),
			};
		}
		public static Vector4d operator /(double L, Vector4s R){
			return new Vector4d{
				X = (L / R.X),
				Y = (L / R.Y),
				Z = (L / R.Z),
				W = (L / R.W),
			};
		}
		public static Vector4 operator /(float L, Vector4s R){
			return new Vector4{
				X = (L / R.X),
				Y = (L / R.Y),
				Z = (L / R.Z),
				W = (L / R.W),
			};
		}
		public static Vector4 operator /(Vector4 L, Vector4s R){
			return new Vector4{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
				W = (L.W / R.W),
			};
		}
		public static Vector4d operator /(Vector4d L, Vector4s R){
			return new Vector4d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
				W = (L.W / R.W),
			};
		}
		public static Vector4 operator /(Vector4s L, Vector4 R){
			return new Vector4{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
				W = (L.W / R.W),
			};
		}
		public static Vector4d operator /(Vector4s L, Vector4d R){
			return new Vector4d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
				W = (L.W / R.W),
			};
		}
		public static explicit operator Vector4(Vector4s value)
		{
			return new Vector4{X = value.X,Y = value.Y,Z = value.Z,W = value.W,};
		}
		public static explicit operator Vector4d(Vector4s value)
		{
			return new Vector4d{X = value.X,Y = value.Y,Z = value.Z,W = value.W,};
		}
		public static explicit operator Vector4s(Vector4 value)
		{
			return new Vector4s{X = (short)value.X,Y = (short)value.Y,Z = (short)value.Z,W = (short)value.W,};
		}
		public static explicit operator Vector4s(Vector4d value)
		{
			return new Vector4s{X = (short)value.X,Y = (short)value.Y,Z = (short)value.Z,W = (short)value.W,};
		}
		public override int GetHashCode(){return GetHashCode(ref this);}
		public static int GetHashCode(Vector4s Value){ 
			// IF YOU GET AN ERROR ON THE LINE BELOW, ITS BECAUSE YOU DID NOT IMPLEMENT
			// IT OUTSIDE OF THIS PARTIAL GENERATED CODE.
			//
			// NOTE THAT THE ARGUMENT MUST BE NAMED VALUE!
			return GetHashCode(Value:ref Value);
		}
		public static Vector4s operator -(Vector4s v) {
			unchecked { 
				return new Vector4s {
					X= (short)(-v.X),
					Y= (short)(-v.X),
					Z= (short)(-v.X),
					W= (short)(-v.X),
				};
			}
		}
		public override bool Equals(object obj)
		{
			return obj is Vector4s && Packed == ((Vector4s)obj).Packed;
		}
		public static explicit operator Vector4s(ulong value) {
			return new Vector4s { Packed = value, };
		}
	}
	///<summary>A 3 dimension vector with elements of type short.</summary>
	[StructLayout(LayoutKind.Sequential, Size = 6, Pack = 2)]
	public partial struct Vector3s
		: IEquatable<Vector3s>, IEquatable<Vector4s>
	{
		public short X;
		public short Y;
		public short Z;
		public ulong Packed {
			get { return new Vector4s { X = X, Y = Y, Z = Z, }.Packed; }
			set {
				var V4 = new Vector4s { Packed = value, };
				this.X = V4.X;
				this.Y = V4.Y;
				this.Z = V4.Z;
			}
		}
		public short this[int axis] {
			get {
				return 0 == (axis & 2) ? 0 == (axis & 1) ? X : Y : 0 == (axis & 1) ? Z : (short)0;
			}
			set	{
				if (0 == (axis & 2))
					if (0 == (axis & 1))
						X = value;
					else
						Y = value;
				else
					if (0 == (axis & 1))
					Z = value;
			}
		}
		public bool Equals(Vector3s other) {
			return X == other.X && Y == other.Y && Z == other.Z;
		}
		public bool Equals(Vector4s other) {
			return 0 == other.W && X == other.X && Y == other.Y && Z == other.Z;
		}
		public override string ToString() {
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			var Zs = X == Z ? Xs : Y == Z ? Ys : Z == 0 ? "0" : Z.ToString();

			return string.Concat("[",
				string.Concat(Xs, ",", Ys, ","),
				Zs, "]");
		}
		public static bool operator ==(Vector3s L, Vector3s R) { return L.X == R.X && L.Y == R.Y && L.Z == R.Z; }
		public static bool operator !=(Vector3s L, Vector3s R) { return L.X != R.X || L.Y != R.Y || L.Z != R.Z; }
		public static bool operator ==(Vector3s L, ulong R) { return L.Packed == R; }
		public static bool operator !=(Vector3s L, ulong R) { return L.Packed != R; }
		public static bool operator ==(ulong L, Vector3s R) { return L == R.Packed; }
		public static bool operator !=(ulong L, Vector3s R) { return L != R.Packed; }

		public static Vector3s operator &(Vector3s L, Vector3s R) { return new Vector3s { Packed = (ulong)(L.Packed & R.Packed), }; }
		public static Vector3s operator |(Vector3s L, Vector3s R) { return new Vector3s { Packed = (ulong)(L.Packed | R.Packed), }; }
		public static Vector3s operator ^(Vector3s L, Vector3s R) { return new Vector3s { Packed = (ulong)(L.Packed ^ R.Packed), }; }
		public static bool operator true(Vector3s criteria) { return criteria.X != 0 || criteria.Y != 0 || criteria.Z != 0; }
		public static bool operator false(Vector3s criteria) { return 0 == criteria.X && 0 == criteria.Y && 0 == criteria.Z; }
		public static Vector3s operator +(Vector3s L, Vector3s R)
		{
			unchecked
			{
				return new Vector3s {
					X = (short)(L.X + R.X),
					Y = (short)(L.Y + R.Y),
					Z = (short)(L.Z + R.Z),
				};
			}
		}
		public static Vector3s operator +(Vector3s L, short R)
		{
			unchecked
			{
				return new Vector3s {
					X = (short)(L.X + R),
					Y = (short)(L.Y + R),
					Z = (short)(L.Z + R),
				};
			}
		}
		public static Vector3s operator +(short L, Vector3s R)
		{
			unchecked
			{
				return new Vector3s {
					X = (short)(L + R.X),
					Y = (short)(L + R.Y),
					Z = (short)(L + R.Z),
				};
			}
		}
		public static Vector3d operator +(Vector3s L, double R){
			return new Vector3d{
				X = (L.X + R),
				Y = (L.Y + R),
				Z = (L.Z + R),
			};
		}
		public static Vector3 operator +(Vector3s L, float R){
			return new Vector3{
				X = (L.X + R),
				Y = (L.Y + R),
				Z = (L.Z + R),
			};
		}
		public static Vector3d operator +(double L, Vector3s R){
			return new Vector3d{
				X = (L + R.X),
				Y = (L + R.Y),
				Z = (L + R.Z),
			};
		}
		public static Vector3 operator +(float L, Vector3s R){
			return new Vector3{
				X = (L + R.X),
				Y = (L + R.Y),
				Z = (L + R.Z),
			};
		}
		public static Vector3 operator +(Vector3 L, Vector3s R){
			return new Vector3{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
			};
		}
		public static Vector3d operator +(Vector3d L, Vector3s R){
			return new Vector3d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
			};
		}
		public static Vector3 operator +(Vector3s L, Vector3 R){
			return new Vector3{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
			};
		}
		public static Vector3d operator +(Vector3s L, Vector3d R){
			return new Vector3d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
				Z = (L.Z + R.Z),
			};
		}
		public static Vector3s operator -(Vector3s L, Vector3s R)
		{
			unchecked
			{
				return new Vector3s {
					X = (short)(L.X - R.X),
					Y = (short)(L.Y - R.Y),
					Z = (short)(L.Z - R.Z),
				};
			}
		}
		public static Vector3s operator -(Vector3s L, short R)
		{
			unchecked
			{
				return new Vector3s {
					X = (short)(L.X - R),
					Y = (short)(L.Y - R),
					Z = (short)(L.Z - R),
				};
			}
		}
		public static Vector3s operator -(short L, Vector3s R)
		{
			unchecked
			{
				return new Vector3s {
					X = (short)(L - R.X),
					Y = (short)(L - R.Y),
					Z = (short)(L - R.Z),
				};
			}
		}
		public static Vector3d operator -(Vector3s L, double R){
			return new Vector3d{
				X = (L.X - R),
				Y = (L.Y - R),
				Z = (L.Z - R),
			};
		}
		public static Vector3 operator -(Vector3s L, float R){
			return new Vector3{
				X = (L.X - R),
				Y = (L.Y - R),
				Z = (L.Z - R),
			};
		}
		public static Vector3d operator -(double L, Vector3s R){
			return new Vector3d{
				X = (L - R.X),
				Y = (L - R.Y),
				Z = (L - R.Z),
			};
		}
		public static Vector3 operator -(float L, Vector3s R){
			return new Vector3{
				X = (L - R.X),
				Y = (L - R.Y),
				Z = (L - R.Z),
			};
		}
		public static Vector3 operator -(Vector3 L, Vector3s R){
			return new Vector3{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
			};
		}
		public static Vector3d operator -(Vector3d L, Vector3s R){
			return new Vector3d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
			};
		}
		public static Vector3 operator -(Vector3s L, Vector3 R){
			return new Vector3{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
			};
		}
		public static Vector3d operator -(Vector3s L, Vector3d R){
			return new Vector3d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
				Z = (L.Z - R.Z),
			};
		}
		public static Vector3s operator *(Vector3s L, Vector3s R)
		{
			unchecked
			{
				return new Vector3s {
					X = (short)(L.X * R.X),
					Y = (short)(L.Y * R.Y),
					Z = (short)(L.Z * R.Z),
				};
			}
		}
		public static Vector3s operator *(Vector3s L, short R)
		{
			unchecked
			{
				return new Vector3s {
					X = (short)(L.X * R),
					Y = (short)(L.Y * R),
					Z = (short)(L.Z * R),
				};
			}
		}
		public static Vector3s operator *(short L, Vector3s R)
		{
			unchecked
			{
				return new Vector3s {
					X = (short)(L * R.X),
					Y = (short)(L * R.Y),
					Z = (short)(L * R.Z),
				};
			}
		}
		public static Vector3d operator *(Vector3s L, double R){
			return new Vector3d{
				X = (L.X * R),
				Y = (L.Y * R),
				Z = (L.Z * R),
			};
		}
		public static Vector3 operator *(Vector3s L, float R){
			return new Vector3{
				X = (L.X * R),
				Y = (L.Y * R),
				Z = (L.Z * R),
			};
		}
		public static Vector3d operator *(double L, Vector3s R){
			return new Vector3d{
				X = (L * R.X),
				Y = (L * R.Y),
				Z = (L * R.Z),
			};
		}
		public static Vector3 operator *(float L, Vector3s R){
			return new Vector3{
				X = (L * R.X),
				Y = (L * R.Y),
				Z = (L * R.Z),
			};
		}
		public static Vector3 operator *(Vector3 L, Vector3s R){
			return new Vector3{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
			};
		}
		public static Vector3d operator *(Vector3d L, Vector3s R){
			return new Vector3d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
			};
		}
		public static Vector3 operator *(Vector3s L, Vector3 R){
			return new Vector3{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
			};
		}
		public static Vector3d operator *(Vector3s L, Vector3d R){
			return new Vector3d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
				Z = (L.Z * R.Z),
			};
		}
		public static Vector3s operator /(Vector3s L, Vector3s R)
		{
			unchecked
			{
				return new Vector3s {
					X = (short)(L.X / R.X),
					Y = (short)(L.Y / R.Y),
					Z = (short)(L.Z / R.Z),
				};
			}
		}
		public static Vector3s operator /(Vector3s L, short R)
		{
			unchecked
			{
				return new Vector3s {
					X = (short)(L.X / R),
					Y = (short)(L.Y / R),
					Z = (short)(L.Z / R),
				};
			}
		}
		public static Vector3s operator /(short L, Vector3s R)
		{
			unchecked
			{
				return new Vector3s {
					X = (short)(L / R.X),
					Y = (short)(L / R.Y),
					Z = (short)(L / R.Z),
				};
			}
		}
		public static Vector3d operator /(Vector3s L, double R){
			return new Vector3d{
				X = (L.X / R),
				Y = (L.Y / R),
				Z = (L.Z / R),
			};
		}
		public static Vector3 operator /(Vector3s L, float R){
			return new Vector3{
				X = (L.X / R),
				Y = (L.Y / R),
				Z = (L.Z / R),
			};
		}
		public static Vector3d operator /(double L, Vector3s R){
			return new Vector3d{
				X = (L / R.X),
				Y = (L / R.Y),
				Z = (L / R.Z),
			};
		}
		public static Vector3 operator /(float L, Vector3s R){
			return new Vector3{
				X = (L / R.X),
				Y = (L / R.Y),
				Z = (L / R.Z),
			};
		}
		public static Vector3 operator /(Vector3 L, Vector3s R){
			return new Vector3{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
			};
		}
		public static Vector3d operator /(Vector3d L, Vector3s R){
			return new Vector3d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
			};
		}
		public static Vector3 operator /(Vector3s L, Vector3 R){
			return new Vector3{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
			};
		}
		public static Vector3d operator /(Vector3s L, Vector3d R){
			return new Vector3d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
				Z = (L.Z / R.Z),
			};
		}
		public static explicit operator Vector3(Vector3s value)
		{
			return new Vector3{X = value.X,Y = value.Y,Z = value.Z,};
		}
		public static explicit operator Vector3d(Vector3s value)
		{
			return new Vector3d{X = value.X,Y = value.Y,Z = value.Z,};
		}
		public static explicit operator Vector3s(Vector3 value)
		{
			return new Vector3s{X = (short)value.X,Y = (short)value.Y,Z = (short)value.Z,};
		}
		public static explicit operator Vector3s(Vector3d value)
		{
			return new Vector3s{X = (short)value.X,Y = (short)value.Y,Z = (short)value.Z,};
		}
		public override int GetHashCode() {
			return Vector4s.GetHashCode(new Vector4s { X=X,Y=Y,Z=Z,});
		}
		public static Vector3s operator -(Vector3s v) {
			unchecked { 
				return new Vector3s {
					X= (short)(-v.X),
					Y= (short)(-v.X),
					Z= (short)(-v.X),
				};
			}
		}
		public override bool Equals(object obj)
		{
			return obj is Vector3s && Packed == ((Vector3s)obj).Packed;
		}
		public static explicit operator Vector3s(ulong value) {
			return new Vector3s { Packed = value, };
		}
	}
	///<summary>A 2 dimension vector with elements of type short.</summary>
	[StructLayout(LayoutKind.Explicit, Size = sizeof(uint))]
	public partial struct Vector2s
		: IEquatable<Vector2s>
	{
		[FieldOffset(sizeof(short) * 1)]
		public short X;
		[FieldOffset(sizeof(short) * 0)]
		public short Y;
		[FieldOffset(0)]
		public uint Packed;
		public short this[int axis]{
			get	{
				return 0 == (axis & 1) ? X : Y;
			}
			set	{
				if (0 == (axis & 1))
					X = value;
				else
					Y = value;
			}
		}
		public bool Equals(Vector2s other){
			return Packed == other.Packed;
		}

		public override string ToString(){
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			return string.Concat("[",
				string.Concat(Xs, ",", Ys, "]"));
		}
		public static bool operator ==(Vector2s L, Vector2s R) { return L.Packed == R.Packed; }
		public static bool operator !=(Vector2s L, Vector2s R) { return L.Packed != R.Packed; }
		public static bool operator ==(Vector2s L, uint R) { return L.Packed == R; }
		public static bool operator !=(Vector2s L, uint R) { return L.Packed != R; }
		public static bool operator ==(uint L, Vector2s R) { return L == R.Packed; }
		public static bool operator !=(uint L, Vector2s R) { return L != R.Packed; }

		public static Vector2s operator &(Vector2s L, Vector2s R) { return new Vector2s { Packed = (uint)(L.Packed & R.Packed), }; }
		public static Vector2s operator |(Vector2s L, Vector2s R) { return new Vector2s { Packed = (uint)(L.Packed | R.Packed), }; }
		public static Vector2s operator ^(Vector2s L, Vector2s R) { return new Vector2s { Packed = (uint)(L.Packed ^ R.Packed), }; }
		public static bool operator true(Vector2s criteria) { return criteria.Packed != 0; }
		public static bool operator false(Vector2s criteria) { return 0 == criteria.Packed; }
		public static Vector2s operator +(Vector2s L, Vector2s R)
		{
			unchecked
			{
				return new Vector2s {
					X = (short)(L.X + R.X),
					Y = (short)(L.Y + R.Y),
				};
			}
		}
		public static Vector2s operator +(Vector2s L, short R)
		{
			unchecked
			{
				return new Vector2s {
					X = (short)(L.X + R),
					Y = (short)(L.Y + R),
				};
			}
		}
		public static Vector2s operator +(short L, Vector2s R)
		{
			unchecked
			{
				return new Vector2s {
					X = (short)(L + R.X),
					Y = (short)(L + R.Y),
				};
			}
		}
		public static Vector2d operator +(Vector2s L, double R){
			return new Vector2d{
				X = (L.X + R),
				Y = (L.Y + R),
			};
		}
		public static Vector2 operator +(Vector2s L, float R){
			return new Vector2{
				X = (L.X + R),
				Y = (L.Y + R),
			};
		}
		public static Vector2d operator +(double L, Vector2s R){
			return new Vector2d{
				X = (L + R.X),
				Y = (L + R.Y),
			};
		}
		public static Vector2 operator +(float L, Vector2s R){
			return new Vector2{
				X = (L + R.X),
				Y = (L + R.Y),
			};
		}
		public static Vector2 operator +(Vector2 L, Vector2s R){
			return new Vector2{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2d operator +(Vector2d L, Vector2s R){
			return new Vector2d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2 operator +(Vector2s L, Vector2 R){
			return new Vector2{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2d operator +(Vector2s L, Vector2d R){
			return new Vector2d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2s operator -(Vector2s L, Vector2s R)
		{
			unchecked
			{
				return new Vector2s {
					X = (short)(L.X - R.X),
					Y = (short)(L.Y - R.Y),
				};
			}
		}
		public static Vector2s operator -(Vector2s L, short R)
		{
			unchecked
			{
				return new Vector2s {
					X = (short)(L.X - R),
					Y = (short)(L.Y - R),
				};
			}
		}
		public static Vector2s operator -(short L, Vector2s R)
		{
			unchecked
			{
				return new Vector2s {
					X = (short)(L - R.X),
					Y = (short)(L - R.Y),
				};
			}
		}
		public static Vector2d operator -(Vector2s L, double R){
			return new Vector2d{
				X = (L.X - R),
				Y = (L.Y - R),
			};
		}
		public static Vector2 operator -(Vector2s L, float R){
			return new Vector2{
				X = (L.X - R),
				Y = (L.Y - R),
			};
		}
		public static Vector2d operator -(double L, Vector2s R){
			return new Vector2d{
				X = (L - R.X),
				Y = (L - R.Y),
			};
		}
		public static Vector2 operator -(float L, Vector2s R){
			return new Vector2{
				X = (L - R.X),
				Y = (L - R.Y),
			};
		}
		public static Vector2 operator -(Vector2 L, Vector2s R){
			return new Vector2{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2d operator -(Vector2d L, Vector2s R){
			return new Vector2d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2 operator -(Vector2s L, Vector2 R){
			return new Vector2{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2d operator -(Vector2s L, Vector2d R){
			return new Vector2d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2s operator *(Vector2s L, Vector2s R)
		{
			unchecked
			{
				return new Vector2s {
					X = (short)(L.X * R.X),
					Y = (short)(L.Y * R.Y),
				};
			}
		}
		public static Vector2s operator *(Vector2s L, short R)
		{
			unchecked
			{
				return new Vector2s {
					X = (short)(L.X * R),
					Y = (short)(L.Y * R),
				};
			}
		}
		public static Vector2s operator *(short L, Vector2s R)
		{
			unchecked
			{
				return new Vector2s {
					X = (short)(L * R.X),
					Y = (short)(L * R.Y),
				};
			}
		}
		public static Vector2d operator *(Vector2s L, double R){
			return new Vector2d{
				X = (L.X * R),
				Y = (L.Y * R),
			};
		}
		public static Vector2 operator *(Vector2s L, float R){
			return new Vector2{
				X = (L.X * R),
				Y = (L.Y * R),
			};
		}
		public static Vector2d operator *(double L, Vector2s R){
			return new Vector2d{
				X = (L * R.X),
				Y = (L * R.Y),
			};
		}
		public static Vector2 operator *(float L, Vector2s R){
			return new Vector2{
				X = (L * R.X),
				Y = (L * R.Y),
			};
		}
		public static Vector2 operator *(Vector2 L, Vector2s R){
			return new Vector2{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2d operator *(Vector2d L, Vector2s R){
			return new Vector2d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2 operator *(Vector2s L, Vector2 R){
			return new Vector2{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2d operator *(Vector2s L, Vector2d R){
			return new Vector2d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2s operator /(Vector2s L, Vector2s R)
		{
			unchecked
			{
				return new Vector2s {
					X = (short)(L.X / R.X),
					Y = (short)(L.Y / R.Y),
				};
			}
		}
		public static Vector2s operator /(Vector2s L, short R)
		{
			unchecked
			{
				return new Vector2s {
					X = (short)(L.X / R),
					Y = (short)(L.Y / R),
				};
			}
		}
		public static Vector2s operator /(short L, Vector2s R)
		{
			unchecked
			{
				return new Vector2s {
					X = (short)(L / R.X),
					Y = (short)(L / R.Y),
				};
			}
		}
		public static Vector2d operator /(Vector2s L, double R){
			return new Vector2d{
				X = (L.X / R),
				Y = (L.Y / R),
			};
		}
		public static Vector2 operator /(Vector2s L, float R){
			return new Vector2{
				X = (L.X / R),
				Y = (L.Y / R),
			};
		}
		public static Vector2d operator /(double L, Vector2s R){
			return new Vector2d{
				X = (L / R.X),
				Y = (L / R.Y),
			};
		}
		public static Vector2 operator /(float L, Vector2s R){
			return new Vector2{
				X = (L / R.X),
				Y = (L / R.Y),
			};
		}
		public static Vector2 operator /(Vector2 L, Vector2s R){
			return new Vector2{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static Vector2d operator /(Vector2d L, Vector2s R){
			return new Vector2d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static Vector2 operator /(Vector2s L, Vector2 R){
			return new Vector2{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static Vector2d operator /(Vector2s L, Vector2d R){
			return new Vector2d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static explicit operator Vector2(Vector2s value)
		{
			return new Vector2{X = value.X,Y = value.Y,};
		}
		public static explicit operator Vector2d(Vector2s value)
		{
			return new Vector2d{X = value.X,Y = value.Y,};
		}
		public static explicit operator Vector2s(Vector2 value)
		{
			return new Vector2s{X = (short)value.X,Y = (short)value.Y,};
		}
		public static explicit operator Vector2s(Vector2d value)
		{
			return new Vector2s{X = (short)value.X,Y = (short)value.Y,};
		}
		public override int GetHashCode() {
			return Vector4s.GetHashCode(new Vector4s { X=X,Y=Y,});
		}
		public static Vector2s operator -(Vector2s v) {
			unchecked { 
				return new Vector2s {
					X= (short)(-v.X),
					Y= (short)(-v.X),
				};
			}
		}
		public override bool Equals(object obj)
		{
			return obj is Vector2s && Packed == ((Vector2s)obj).Packed;
		}
		public static explicit operator Vector2s(uint value) {
			return new Vector2s { Packed = value, };
		}
	}
	///<summary>A 2 dimension vector with elements of type uint.</summary>
	[StructLayout(LayoutKind.Explicit, Size = sizeof(ulong))]
	public partial struct Vector2u
		: IEquatable<Vector2u>
	{
		[FieldOffset(sizeof(uint) * 1)]
		public uint X;
		[FieldOffset(sizeof(uint) * 0)]
		public uint Y;
		[FieldOffset(0)]
		public ulong Packed;
		public uint this[int axis]{
			get	{
				return 0 == (axis & 1) ? X : Y;
			}
			set	{
				if (0 == (axis & 1))
					X = value;
				else
					Y = value;
			}
		}
		public bool Equals(Vector2u other){
			return Packed == other.Packed;
		}

		public override string ToString(){
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			return string.Concat("[",
				string.Concat(Xs, ",", Ys, "]"));
		}
		public static bool operator ==(Vector2u L, Vector2u R) { return L.Packed == R.Packed; }
		public static bool operator !=(Vector2u L, Vector2u R) { return L.Packed != R.Packed; }
		public static bool operator ==(Vector2u L, ulong R) { return L.Packed == R; }
		public static bool operator !=(Vector2u L, ulong R) { return L.Packed != R; }
		public static bool operator ==(ulong L, Vector2u R) { return L == R.Packed; }
		public static bool operator !=(ulong L, Vector2u R) { return L != R.Packed; }

		public static Vector2u operator &(Vector2u L, Vector2u R) { return new Vector2u { Packed = (ulong)(L.Packed & R.Packed), }; }
		public static Vector2u operator |(Vector2u L, Vector2u R) { return new Vector2u { Packed = (ulong)(L.Packed | R.Packed), }; }
		public static Vector2u operator ^(Vector2u L, Vector2u R) { return new Vector2u { Packed = (ulong)(L.Packed ^ R.Packed), }; }
		public static bool operator true(Vector2u criteria) { return criteria.Packed != 0; }
		public static bool operator false(Vector2u criteria) { return 0 == criteria.Packed; }
		public static Vector2u operator +(Vector2u L, Vector2u R)
		{
			unchecked
			{
				return new Vector2u {
					X = (uint)(L.X + R.X),
					Y = (uint)(L.Y + R.Y),
				};
			}
		}
		public static Vector2u operator +(Vector2u L, uint R)
		{
			unchecked
			{
				return new Vector2u {
					X = (uint)(L.X + R),
					Y = (uint)(L.Y + R),
				};
			}
		}
		public static Vector2u operator +(uint L, Vector2u R)
		{
			unchecked
			{
				return new Vector2u {
					X = (uint)(L + R.X),
					Y = (uint)(L + R.Y),
				};
			}
		}
		public static Vector2d operator +(Vector2u L, double R){
			return new Vector2d{
				X = (L.X + R),
				Y = (L.Y + R),
			};
		}
		public static Vector2 operator +(Vector2u L, float R){
			return new Vector2{
				X = (L.X + R),
				Y = (L.Y + R),
			};
		}
		public static Vector2d operator +(double L, Vector2u R){
			return new Vector2d{
				X = (L + R.X),
				Y = (L + R.Y),
			};
		}
		public static Vector2 operator +(float L, Vector2u R){
			return new Vector2{
				X = (L + R.X),
				Y = (L + R.Y),
			};
		}
		public static Vector2 operator +(Vector2 L, Vector2u R){
			return new Vector2{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2d operator +(Vector2d L, Vector2u R){
			return new Vector2d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2 operator +(Vector2u L, Vector2 R){
			return new Vector2{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2d operator +(Vector2u L, Vector2d R){
			return new Vector2d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2u operator -(Vector2u L, Vector2u R)
		{
			unchecked
			{
				return new Vector2u {
					X = (uint)(L.X - R.X),
					Y = (uint)(L.Y - R.Y),
				};
			}
		}
		public static Vector2u operator -(Vector2u L, uint R)
		{
			unchecked
			{
				return new Vector2u {
					X = (uint)(L.X - R),
					Y = (uint)(L.Y - R),
				};
			}
		}
		public static Vector2u operator -(uint L, Vector2u R)
		{
			unchecked
			{
				return new Vector2u {
					X = (uint)(L - R.X),
					Y = (uint)(L - R.Y),
				};
			}
		}
		public static Vector2d operator -(Vector2u L, double R){
			return new Vector2d{
				X = (L.X - R),
				Y = (L.Y - R),
			};
		}
		public static Vector2 operator -(Vector2u L, float R){
			return new Vector2{
				X = (L.X - R),
				Y = (L.Y - R),
			};
		}
		public static Vector2d operator -(double L, Vector2u R){
			return new Vector2d{
				X = (L - R.X),
				Y = (L - R.Y),
			};
		}
		public static Vector2 operator -(float L, Vector2u R){
			return new Vector2{
				X = (L - R.X),
				Y = (L - R.Y),
			};
		}
		public static Vector2 operator -(Vector2 L, Vector2u R){
			return new Vector2{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2d operator -(Vector2d L, Vector2u R){
			return new Vector2d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2 operator -(Vector2u L, Vector2 R){
			return new Vector2{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2d operator -(Vector2u L, Vector2d R){
			return new Vector2d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2u operator *(Vector2u L, Vector2u R)
		{
			unchecked
			{
				return new Vector2u {
					X = (uint)(L.X * R.X),
					Y = (uint)(L.Y * R.Y),
				};
			}
		}
		public static Vector2u operator *(Vector2u L, uint R)
		{
			unchecked
			{
				return new Vector2u {
					X = (uint)(L.X * R),
					Y = (uint)(L.Y * R),
				};
			}
		}
		public static Vector2u operator *(uint L, Vector2u R)
		{
			unchecked
			{
				return new Vector2u {
					X = (uint)(L * R.X),
					Y = (uint)(L * R.Y),
				};
			}
		}
		public static Vector2d operator *(Vector2u L, double R){
			return new Vector2d{
				X = (L.X * R),
				Y = (L.Y * R),
			};
		}
		public static Vector2 operator *(Vector2u L, float R){
			return new Vector2{
				X = (L.X * R),
				Y = (L.Y * R),
			};
		}
		public static Vector2d operator *(double L, Vector2u R){
			return new Vector2d{
				X = (L * R.X),
				Y = (L * R.Y),
			};
		}
		public static Vector2 operator *(float L, Vector2u R){
			return new Vector2{
				X = (L * R.X),
				Y = (L * R.Y),
			};
		}
		public static Vector2 operator *(Vector2 L, Vector2u R){
			return new Vector2{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2d operator *(Vector2d L, Vector2u R){
			return new Vector2d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2 operator *(Vector2u L, Vector2 R){
			return new Vector2{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2d operator *(Vector2u L, Vector2d R){
			return new Vector2d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2u operator /(Vector2u L, Vector2u R)
		{
			unchecked
			{
				return new Vector2u {
					X = (uint)(L.X / R.X),
					Y = (uint)(L.Y / R.Y),
				};
			}
		}
		public static Vector2u operator /(Vector2u L, uint R)
		{
			unchecked
			{
				return new Vector2u {
					X = (uint)(L.X / R),
					Y = (uint)(L.Y / R),
				};
			}
		}
		public static Vector2u operator /(uint L, Vector2u R)
		{
			unchecked
			{
				return new Vector2u {
					X = (uint)(L / R.X),
					Y = (uint)(L / R.Y),
				};
			}
		}
		public static Vector2d operator /(Vector2u L, double R){
			return new Vector2d{
				X = (L.X / R),
				Y = (L.Y / R),
			};
		}
		public static Vector2 operator /(Vector2u L, float R){
			return new Vector2{
				X = (L.X / R),
				Y = (L.Y / R),
			};
		}
		public static Vector2d operator /(double L, Vector2u R){
			return new Vector2d{
				X = (L / R.X),
				Y = (L / R.Y),
			};
		}
		public static Vector2 operator /(float L, Vector2u R){
			return new Vector2{
				X = (L / R.X),
				Y = (L / R.Y),
			};
		}
		public static Vector2 operator /(Vector2 L, Vector2u R){
			return new Vector2{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static Vector2d operator /(Vector2d L, Vector2u R){
			return new Vector2d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static Vector2 operator /(Vector2u L, Vector2 R){
			return new Vector2{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static Vector2d operator /(Vector2u L, Vector2d R){
			return new Vector2d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static explicit operator Vector2(Vector2u value)
		{
			return new Vector2{X = value.X,Y = value.Y,};
		}
		public static explicit operator Vector2d(Vector2u value)
		{
			return new Vector2d{X = value.X,Y = value.Y,};
		}
		public static explicit operator Vector2u(Vector2 value)
		{
			return new Vector2u{X = (uint)value.X,Y = (uint)value.Y,};
		}
		public static explicit operator Vector2u(Vector2d value)
		{
			return new Vector2u{X = (uint)value.X,Y = (uint)value.Y,};
		}
		public override int GetHashCode(){return GetHashCode(ref this);}
		public static int GetHashCode(Vector2u Value){ 
			// IF YOU GET AN ERROR ON THE LINE BELOW, ITS BECAUSE YOU DID NOT IMPLEMENT
			// IT OUTSIDE OF THIS PARTIAL GENERATED CODE.
			//
			// NOTE THAT THE ARGUMENT MUST BE NAMED VALUE!
			return GetHashCode(Value:ref Value);
		}
		public static Vector2u operator -(Vector2u v) {
			unchecked { 
				return new Vector2u {
					X= (uint)(-v.X),
					Y= (uint)(-v.X),
				};
			}
		}
		public override bool Equals(object obj)
		{
			return obj is Vector2u && Packed == ((Vector2u)obj).Packed;
		}
		public static explicit operator Vector2u(ulong value) {
			return new Vector2u { Packed = value, };
		}
	}
	///<summary>A 2 dimension vector with elements of type int.</summary>
	[StructLayout(LayoutKind.Explicit, Size = sizeof(ulong))]
	public partial struct Vector2i
		: IEquatable<Vector2i>
	{
		[FieldOffset(sizeof(int) * 1)]
		public int X;
		[FieldOffset(sizeof(int) * 0)]
		public int Y;
		[FieldOffset(0)]
		public ulong Packed;
		public int this[int axis]{
			get	{
				return 0 == (axis & 1) ? X : Y;
			}
			set	{
				if (0 == (axis & 1))
					X = value;
				else
					Y = value;
			}
		}
		public bool Equals(Vector2i other){
			return Packed == other.Packed;
		}

		public override string ToString(){
			var Xs = X == 0 ? "0" : X.ToString();
			var Ys = X == Y ? Xs : Y == 0 ? "0" : Y.ToString();
			return string.Concat("[",
				string.Concat(Xs, ",", Ys, "]"));
		}
		public static bool operator ==(Vector2i L, Vector2i R) { return L.Packed == R.Packed; }
		public static bool operator !=(Vector2i L, Vector2i R) { return L.Packed != R.Packed; }
		public static bool operator ==(Vector2i L, ulong R) { return L.Packed == R; }
		public static bool operator !=(Vector2i L, ulong R) { return L.Packed != R; }
		public static bool operator ==(ulong L, Vector2i R) { return L == R.Packed; }
		public static bool operator !=(ulong L, Vector2i R) { return L != R.Packed; }

		public static Vector2i operator &(Vector2i L, Vector2i R) { return new Vector2i { Packed = (ulong)(L.Packed & R.Packed), }; }
		public static Vector2i operator |(Vector2i L, Vector2i R) { return new Vector2i { Packed = (ulong)(L.Packed | R.Packed), }; }
		public static Vector2i operator ^(Vector2i L, Vector2i R) { return new Vector2i { Packed = (ulong)(L.Packed ^ R.Packed), }; }
		public static bool operator true(Vector2i criteria) { return criteria.Packed != 0; }
		public static bool operator false(Vector2i criteria) { return 0 == criteria.Packed; }
		public static Vector2i operator +(Vector2i L, Vector2i R)
		{
			unchecked
			{
				return new Vector2i {
					X = (int)(L.X + R.X),
					Y = (int)(L.Y + R.Y),
				};
			}
		}
		public static Vector2i operator +(Vector2i L, int R)
		{
			unchecked
			{
				return new Vector2i {
					X = (int)(L.X + R),
					Y = (int)(L.Y + R),
				};
			}
		}
		public static Vector2i operator +(int L, Vector2i R)
		{
			unchecked
			{
				return new Vector2i {
					X = (int)(L + R.X),
					Y = (int)(L + R.Y),
				};
			}
		}
		public static Vector2d operator +(Vector2i L, double R){
			return new Vector2d{
				X = (L.X + R),
				Y = (L.Y + R),
			};
		}
		public static Vector2 operator +(Vector2i L, float R){
			return new Vector2{
				X = (L.X + R),
				Y = (L.Y + R),
			};
		}
		public static Vector2d operator +(double L, Vector2i R){
			return new Vector2d{
				X = (L + R.X),
				Y = (L + R.Y),
			};
		}
		public static Vector2 operator +(float L, Vector2i R){
			return new Vector2{
				X = (L + R.X),
				Y = (L + R.Y),
			};
		}
		public static Vector2 operator +(Vector2 L, Vector2i R){
			return new Vector2{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2d operator +(Vector2d L, Vector2i R){
			return new Vector2d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2 operator +(Vector2i L, Vector2 R){
			return new Vector2{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2d operator +(Vector2i L, Vector2d R){
			return new Vector2d{
				X = (L.X + R.X),
				Y = (L.Y + R.Y),
			};
		}
		public static Vector2i operator -(Vector2i L, Vector2i R)
		{
			unchecked
			{
				return new Vector2i {
					X = (int)(L.X - R.X),
					Y = (int)(L.Y - R.Y),
				};
			}
		}
		public static Vector2i operator -(Vector2i L, int R)
		{
			unchecked
			{
				return new Vector2i {
					X = (int)(L.X - R),
					Y = (int)(L.Y - R),
				};
			}
		}
		public static Vector2i operator -(int L, Vector2i R)
		{
			unchecked
			{
				return new Vector2i {
					X = (int)(L - R.X),
					Y = (int)(L - R.Y),
				};
			}
		}
		public static Vector2d operator -(Vector2i L, double R){
			return new Vector2d{
				X = (L.X - R),
				Y = (L.Y - R),
			};
		}
		public static Vector2 operator -(Vector2i L, float R){
			return new Vector2{
				X = (L.X - R),
				Y = (L.Y - R),
			};
		}
		public static Vector2d operator -(double L, Vector2i R){
			return new Vector2d{
				X = (L - R.X),
				Y = (L - R.Y),
			};
		}
		public static Vector2 operator -(float L, Vector2i R){
			return new Vector2{
				X = (L - R.X),
				Y = (L - R.Y),
			};
		}
		public static Vector2 operator -(Vector2 L, Vector2i R){
			return new Vector2{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2d operator -(Vector2d L, Vector2i R){
			return new Vector2d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2 operator -(Vector2i L, Vector2 R){
			return new Vector2{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2d operator -(Vector2i L, Vector2d R){
			return new Vector2d{
				X = (L.X - R.X),
				Y = (L.Y - R.Y),
			};
		}
		public static Vector2i operator *(Vector2i L, Vector2i R)
		{
			unchecked
			{
				return new Vector2i {
					X = (int)(L.X * R.X),
					Y = (int)(L.Y * R.Y),
				};
			}
		}
		public static Vector2i operator *(Vector2i L, int R)
		{
			unchecked
			{
				return new Vector2i {
					X = (int)(L.X * R),
					Y = (int)(L.Y * R),
				};
			}
		}
		public static Vector2i operator *(int L, Vector2i R)
		{
			unchecked
			{
				return new Vector2i {
					X = (int)(L * R.X),
					Y = (int)(L * R.Y),
				};
			}
		}
		public static Vector2d operator *(Vector2i L, double R){
			return new Vector2d{
				X = (L.X * R),
				Y = (L.Y * R),
			};
		}
		public static Vector2 operator *(Vector2i L, float R){
			return new Vector2{
				X = (L.X * R),
				Y = (L.Y * R),
			};
		}
		public static Vector2d operator *(double L, Vector2i R){
			return new Vector2d{
				X = (L * R.X),
				Y = (L * R.Y),
			};
		}
		public static Vector2 operator *(float L, Vector2i R){
			return new Vector2{
				X = (L * R.X),
				Y = (L * R.Y),
			};
		}
		public static Vector2 operator *(Vector2 L, Vector2i R){
			return new Vector2{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2d operator *(Vector2d L, Vector2i R){
			return new Vector2d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2 operator *(Vector2i L, Vector2 R){
			return new Vector2{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2d operator *(Vector2i L, Vector2d R){
			return new Vector2d{
				X = (L.X * R.X),
				Y = (L.Y * R.Y),
			};
		}
		public static Vector2i operator /(Vector2i L, Vector2i R)
		{
			unchecked
			{
				return new Vector2i {
					X = (int)(L.X / R.X),
					Y = (int)(L.Y / R.Y),
				};
			}
		}
		public static Vector2i operator /(Vector2i L, int R)
		{
			unchecked
			{
				return new Vector2i {
					X = (int)(L.X / R),
					Y = (int)(L.Y / R),
				};
			}
		}
		public static Vector2i operator /(int L, Vector2i R)
		{
			unchecked
			{
				return new Vector2i {
					X = (int)(L / R.X),
					Y = (int)(L / R.Y),
				};
			}
		}
		public static Vector2d operator /(Vector2i L, double R){
			return new Vector2d{
				X = (L.X / R),
				Y = (L.Y / R),
			};
		}
		public static Vector2 operator /(Vector2i L, float R){
			return new Vector2{
				X = (L.X / R),
				Y = (L.Y / R),
			};
		}
		public static Vector2d operator /(double L, Vector2i R){
			return new Vector2d{
				X = (L / R.X),
				Y = (L / R.Y),
			};
		}
		public static Vector2 operator /(float L, Vector2i R){
			return new Vector2{
				X = (L / R.X),
				Y = (L / R.Y),
			};
		}
		public static Vector2 operator /(Vector2 L, Vector2i R){
			return new Vector2{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static Vector2d operator /(Vector2d L, Vector2i R){
			return new Vector2d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static Vector2 operator /(Vector2i L, Vector2 R){
			return new Vector2{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static Vector2d operator /(Vector2i L, Vector2d R){
			return new Vector2d{
				X = (L.X / R.X),
				Y = (L.Y / R.Y),
			};
		}
		public static explicit operator Vector2(Vector2i value)
		{
			return new Vector2{X = value.X,Y = value.Y,};
		}
		public static explicit operator Vector2d(Vector2i value)
		{
			return new Vector2d{X = value.X,Y = value.Y,};
		}
		public static explicit operator Vector2i(Vector2 value)
		{
			return new Vector2i{X = (int)value.X,Y = (int)value.Y,};
		}
		public static explicit operator Vector2i(Vector2d value)
		{
			return new Vector2i{X = (int)value.X,Y = (int)value.Y,};
		}
		public override int GetHashCode(){return GetHashCode(ref this);}
		public static int GetHashCode(Vector2i Value){ 
			// IF YOU GET AN ERROR ON THE LINE BELOW, ITS BECAUSE YOU DID NOT IMPLEMENT
			// IT OUTSIDE OF THIS PARTIAL GENERATED CODE.
			//
			// NOTE THAT THE ARGUMENT MUST BE NAMED VALUE!
			return GetHashCode(Value:ref Value);
		}
		public static Vector2i operator -(Vector2i v) {
			unchecked { 
				return new Vector2i {
					X= (int)(-v.X),
					Y= (int)(-v.X),
				};
			}
		}
		public override bool Equals(object obj)
		{
			return obj is Vector2i && Packed == ((Vector2i)obj).Packed;
		}
		public static explicit operator Vector2i(ulong value) {
			return new Vector2i { Packed = value, };
		}
	}
}
