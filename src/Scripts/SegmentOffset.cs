using System;
using System.Runtime.InteropServices;
using BubblePony.Integers;
namespace Quad64
{
	[StructLayout(LayoutKind.Explicit)]
	public struct SegmentOffset : 
		IEquatable<SegmentOffset>, 
		IEquatable<uint>,
		IComparable,
		IComparable<uint>,
		IComparable<SegmentOffset>,
		IConvertible,
		IFormattable
	{
		[FieldOffset(3)]
		public byte Segment;
		[FieldOffset(0)]
		public uint Value;
		[FieldOffset(0)]
		public UInt24 Offset;
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			return obj is SegmentOffset ? Value == ((SegmentOffset)obj).Value : Value.Equals(obj);
		}
		public bool Equals(uint other)
		{
			return Value.Equals(other);
		}
		public bool Equals(SegmentOffset other)
		{
			return Value.Equals(other.Value);
		}

		public int CompareTo(object obj)
		{
			return obj is SegmentOffset ? Value.CompareTo(((SegmentOffset)obj).Value) : Value.CompareTo(obj);
		}

		public int CompareTo(uint other)
		{
			return Value.CompareTo(other);
		}
		public int CompareTo(SegmentOffset other)
		{
			return Value.CompareTo(other.Value);
		}

		TypeCode IConvertible.GetTypeCode()
		{
			return Value.GetTypeCode();
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToBoolean(provider);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToChar(provider);
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToSByte(provider);
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToByte(provider);
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToInt16(provider);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToUInt16(provider);
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToInt32(provider);
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToUInt32(provider);
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToInt64(provider);
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToUInt64(provider);
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToSingle(provider);
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToDouble(provider);
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToDecimal(provider);
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToDateTime(provider);
		}

		public string ToString(IFormatProvider provider)
		{
			return Value.ToString(provider);
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			return conversionType == typeof(SegmentOffset) ? this : ((IConvertible)Value).ToType(conversionType, provider);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			return Value.ToString(format, formatProvider);
		}
		public override string ToString()
		{
			return Value.ToString();
		}

		public static bool operator <(SegmentOffset L, SegmentOffset R)
		{
			return L.Value < R.Value;
		}
		public static bool operator >(SegmentOffset L, SegmentOffset R)
		{
			return L.Value > R.Value;
		}
		public static bool operator <=(SegmentOffset L, SegmentOffset R)
		{
			return L.Value <= R.Value;
		}
		public static bool operator >=(SegmentOffset L, SegmentOffset R)
		{
			return L.Value >= R.Value;
		}
		public static bool operator <(SegmentOffset L, uint R)
		{
			return L.Value < R;
		}
		public static bool operator >(SegmentOffset L, uint R)
		{
			return L.Value > R;
		}
		public static bool operator <=(SegmentOffset L, uint R)
		{
			return L.Value <= R;
		}
		public static bool operator >=(SegmentOffset L, uint R)
		{
			return L.Value >= R;
		}
		public static bool operator <(uint L, SegmentOffset R)
		{
			return L < R.Value;
		}
		public static bool operator >(uint L, SegmentOffset R)
		{
			return L > R.Value;
		}
		public static bool operator <=(uint L, SegmentOffset R)
		{
			return L <= R.Value;
		}
		public static bool operator >=(uint L, SegmentOffset R)
		{
			return L >= R.Value;
		}
		public static bool operator ==(SegmentOffset L, SegmentOffset R)
		{
			return L.Value == R.Value;
		}
		public static bool operator !=(SegmentOffset L, SegmentOffset R)
		{
			return L.Value != R.Value;
		}
		public static bool operator ==(SegmentOffset L, uint R)
		{
			return L.Value == R;
		}
		public static bool operator !=(SegmentOffset L, uint R)
		{
			return L.Value != R;
		}
		public static bool operator ==(uint L, SegmentOffset R)
		{
			return L == R.Value;
		}
		public static bool operator !=(uint L, SegmentOffset R)
		{
			return L != R.Value;
		}
		public static implicit operator uint(SegmentOffset criteria)
		{
			return criteria.Value;
		}
		public static explicit operator int(SegmentOffset criteria)
		{
			unchecked
			{
				return (int)criteria.Value;
			}
		}
		public static implicit operator SegmentOffset(uint criteria)
		{
			return new SegmentOffset { Value = criteria, };
		}
		public static bool operator true(SegmentOffset criteria)
		{
			return criteria.Segment != 0;
		}
		public static bool operator false(SegmentOffset criteria)
		{
			return criteria.Segment == 0;
		}
		public static SegmentOffset operator &(SegmentOffset L, SegmentOffset R)
		{
			return L.Segment == 0 || R.Segment == 0 ? new SegmentOffset { Offset = R.Offset, } : R;
		}
		public static SegmentOffset operator |(SegmentOffset L, SegmentOffset R)
		{
			return L.Segment == 0 ? R : L;
		}
		public static SegmentOffset operator &(SegmentOffset L, bool R)
		{
			return R ?L : new SegmentOffset { Offset = L.Offset, };
		}
		public static SegmentOffset operator |(SegmentOffset L, bool R)
		{
			return L.Segment == 0 && R ? new SegmentOffset { Segment = 255, Offset = L.Offset, } : L;
		}
		public static SegmentOffset operator &(bool L, SegmentOffset R)
		{
			return L ? R : new SegmentOffset { Offset = R.Offset, };
		}
		public static SegmentOffset operator |(bool L, SegmentOffset R)
		{
			return (L && 0 == R.Segment) ? new SegmentOffset { Offset = R.Offset, Segment = 255, } : R;
		}
		public static SegmentOffset operator +(SegmentOffset L, uint value)
		{
			return new SegmentOffset { Segment = L.Segment, Offset = { Value = (L.Offset.Value + value) & UInt24.MaxValue, } };
		}
		public static SegmentOffset operator -(SegmentOffset L, uint value)
		{
			return new SegmentOffset { Segment = L.Segment, Offset = { Value = (L.Offset.Value - value) & UInt24.MaxValue, } };
		}
		public static SegmentOffset operator ++(SegmentOffset criteria)
		{
			return new SegmentOffset { Segment = criteria.Segment, Offset = { Value = (criteria.Offset.Value + 1u) & UInt24.MaxValue, } };
		}
		public static SegmentOffset operator --(SegmentOffset criteria)
		{
			return new SegmentOffset { Segment = criteria.Segment, Offset = { Value = (criteria.Offset.Value - 1u) & UInt24.MaxValue, } };
		}
	}
}
