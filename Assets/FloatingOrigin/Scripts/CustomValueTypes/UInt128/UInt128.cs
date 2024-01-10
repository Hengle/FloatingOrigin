// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace BigIntegers
{

/// <summary>
/// 128-bit unsigned integer. 
/// </summary>
/// <remarks>
/// Maximum size is 340282366920938463463374607431768211455, or around 340 undecillion.
/// </remarks> 
public partial struct UInt128 : IFormattable, IComparable, IComparable<UInt128>, IEquatable<UInt128>
{
    const MethodImplOptions Inline = MethodImplOptions.AggressiveInlining;

    public ulong _lower;
    public ulong _upper;


    public readonly uint r0 => (uint)_lower;
    public readonly uint r1 => (uint)(_lower >> 32);
    public readonly uint r2 => (uint)_upper;

    public readonly bool IsZero => (_lower | _upper) == 0;
    public readonly bool IsOne => _upper == 0 && _lower == 1;
    public readonly bool IsPowerOfTwo => (this & (this - 1)).IsZero;
    public readonly bool IsEven => (_lower & 1) == 0;
    public readonly int Sign => IsZero ? 0 : 1;


    public static readonly UInt128 MaxValue = ~(UInt128)0;
    public static readonly UInt128 MinValue = (UInt128)0;
    public static readonly UInt128 Zero = (UInt128)0;
    public static readonly UInt128 One = (UInt128)1;
    
    // Parsing
    public static UInt128 Parse(string value)
    {
        if (!TryParse(value, out UInt128 c))
            throw new FormatException();
        return c;
    }

    public static bool TryParse(string value, out UInt128 result)
    {
        return TryParse(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
    }

    public static bool TryParse(string value, NumberStyles style, IFormatProvider provider, out UInt128 result)
    {
        if (!BigInteger.TryParse(value, style, provider, out BigInteger a))
        {
            result = Zero;
            return false;
        }

        result = new UInt128(a);
        return true;
    }

    // Formatting
    public override readonly string ToString() => ((BigInteger)this).ToString();
    public readonly string ToString(string format) => ((BigInteger)this).ToString(format);
    public readonly string ToString(IFormatProvider provider) => ToString(null, provider);
    public readonly string ToString(string format, IFormatProvider provider) => ((BigInteger)this).ToString(format, provider);


    // Conversion operators
    
    // Unsigned to UInt128
    public static implicit operator UInt128(byte a) => new UInt128(a);
    public static implicit operator UInt128(ushort a) => new UInt128(a);
    public static implicit operator UInt128(uint a) => new UInt128(a);
    public static implicit operator UInt128(ulong a) => new UInt128(a);

    // Signed to UInt128
    public static explicit operator UInt128(sbyte a) => new UInt128(a);
    public static explicit operator UInt128(short a) => new UInt128(a);
    public static explicit operator UInt128(int a) => new UInt128(a);
    public static explicit operator UInt128(long a) => new UInt128(a);
    public static explicit operator UInt128(BigInteger a) => new UInt128(a);

    // Floating-Point to UInt128
    public static explicit operator UInt128(double a) => new UInt128(a);
    public static explicit operator UInt128(decimal a) => new UInt128(a);
    
    // UInt128 to Unsigned
    public static explicit operator sbyte(UInt128 a) => (sbyte)a._lower;
    public static explicit operator ushort(UInt128 a) => (ushort)a._lower;
    public static explicit operator uint(UInt128 a) => (uint)a._lower;
    public static explicit operator ulong(UInt128 a) => a._lower;
    
    // UInt128 to Signed
    public static explicit operator byte(UInt128 a) => (byte)a._lower;
    public static explicit operator short(UInt128 a) => (short)a._lower;
    public static explicit operator int(UInt128 a) => (int)a._lower;
    public static explicit operator long(UInt128 a) => (long)a._lower;

    // No precision loss, allow implicit
    public static implicit operator BigInteger(UInt128 a) => ToBigInt(a);

    // UInt128 to Floating-Point
    public static explicit operator float(UInt128 a) => ToFloat(a);
    public static explicit operator double(UInt128 a) => ToDouble(a);
    public static explicit operator decimal(UInt128 a) => ToDecimal(a);

    // Bitwise Operators- Inline to use direct function call
    [MethodImpl(Inline)] public static UInt128 operator <<(UInt128 a, int b) => LeftShift(a, b);
    [MethodImpl(Inline)] public static UInt128 operator >>(UInt128 a, int b) => RightShift(a, b);
    [MethodImpl(Inline)] public static UInt128 operator &(UInt128 a, UInt128 b) => And(a, b);
    [MethodImpl(Inline)] public static UInt128 operator |(UInt128 a, UInt128 b) => Or(a, b);
    [MethodImpl(Inline)] public static UInt128 operator ^(UInt128 a, UInt128 b) => ExclusiveOr(a, b);
    [MethodImpl(Inline)] public static UInt128 operator ~(UInt128 a) => Not(a);

    // Arithmetic Operators- Inline to use direct function call

    // Addition
    [MethodImpl(Inline)] public static UInt128 operator +(UInt128 a, UInt128 b) => Add(a, b);
    [MethodImpl(Inline)] public static UInt128 operator +(UInt128 a, ulong b) => Add(a, b);
    [MethodImpl(Inline)] public static UInt128 operator +(ulong a, UInt128 b) => Add(b, a);
    [MethodImpl(Inline)] public static UInt128 operator +(UInt128 a) => a;
    [MethodImpl(Inline)] public static UInt128 operator ++(UInt128 a) => Add(a, 1);

    // Subtraction
    [MethodImpl(Inline)] public static UInt128 operator -(UInt128 a, UInt128 b) => Subtract(a, b);
    [MethodImpl(Inline)] public static UInt128 operator -(UInt128 a, ulong b) => Subtract(a, b);
    [MethodImpl(Inline)] public static UInt128 operator -(ulong a, UInt128 b) => Subtract(a, b);
    [MethodImpl(Inline)] public static UInt128 operator -(UInt128 a) => Negate(a);
    [MethodImpl(Inline)] public static UInt128 operator --(UInt128 a) => Subtract(a, 1);

    // Multiplication
    [MethodImpl(Inline)] public static UInt128 operator *(UInt128 a, ulong b) => Multiply(a, b);
    [MethodImpl(Inline)] public static UInt128 operator *(ulong a, UInt128 b) => Multiply(b, a);
    [MethodImpl(Inline)] public static UInt128 operator *(UInt128 a, UInt128 b) => Multiply(a, b);

    // Division
    [MethodImpl(Inline)] public static UInt128 operator /(UInt128 a, ulong b) => Divide(a, b);
    [MethodImpl(Inline)] public static UInt128 operator /(UInt128 a, UInt128 b) => Divide(a, b);

    // Modulus
    [MethodImpl(Inline)] public static ulong operator %(UInt128 a, ulong b) => Remainder(a, b);
    [MethodImpl(Inline)] public static UInt128 operator %(UInt128 a, UInt128 b) => Remainder(a, b);

    // Comparison Operators

    // Less
    public static bool operator <(UInt128 a, UInt128 b) => LessThan(a, b); 
    public static bool operator <(UInt128 a, long b) => LessThan(a, b);
    public static bool operator <(long a, UInt128 b) => LessThan(a, b);
    public static bool operator <(UInt128 a, ulong b) => LessThan(a, b);
    public static bool operator <(ulong a, UInt128 b) => LessThan(a, b);

    // Less Equals
    public static bool operator <=(UInt128 a, UInt128 b) => !LessThan(b, a);
    public static bool operator <=(UInt128 a, long b) => !LessThan(b, a);
    public static bool operator <=(long a, UInt128 b) => !LessThan(b, a);
    public static bool operator <=(UInt128 a, ulong b) => !LessThan(b, a);
    public static bool operator <=(ulong a, UInt128 b) => !LessThan(b, a);

    // Greater
    public static bool operator >(UInt128 a, UInt128 b) => LessThan(b, a);
    public static bool operator >(UInt128 a, long b) => LessThan(b, a);
    public static bool operator >(long a, UInt128 b) => LessThan(b, a);
    public static bool operator >(UInt128 a, ulong b) => LessThan(b, a);
    public static bool operator >(ulong a, UInt128 b) => LessThan(b, a);

    // Greater Equals
    public static bool operator >=(UInt128 a, UInt128 b) => !LessThan(a, b);
    public static bool operator >=(UInt128 a, long b) => !LessThan(a, b);
    public static bool operator >=(long a, UInt128 b) => !LessThan(a, b);
    public static bool operator >=(UInt128 a, ulong b) => !LessThan(a, b);
    public static bool operator >=(ulong a, UInt128 b) => !LessThan(a, b);

    // Equals
    public static bool operator ==(UInt128 a, UInt128 b) => a.Equals(b);
    public static bool operator ==(UInt128 a, long b) => a.Equals(b);
    public static bool operator ==(long a, UInt128 b) => b.Equals(a);
    public static bool operator ==(UInt128 a, ulong b) => a.Equals(b);
    public static bool operator ==(ulong a, UInt128 b) => b.Equals(a);

    // Not Equals
    public static bool operator !=(UInt128 a, UInt128 b) => !a.Equals(b);
    public static bool operator !=(UInt128 a, long b) => !a.Equals(b);
    public static bool operator !=(long a, UInt128 b) => !b.Equals(a);
    public static bool operator !=(UInt128 a, ulong b) => !a.Equals(b);
    public static bool operator !=(ulong a, UInt128 b) => !b.Equals(a);


    public readonly int CompareTo(UInt128 other) => _upper != other._upper ? _upper.CompareTo(other._upper) : _lower.CompareTo(other._lower); 
    public readonly int CompareTo(long other) => _upper != 0 || other < 0 ? 1 : _lower.CompareTo((ulong)other);
    public readonly int CompareTo(ulong other) => _upper != 0 ? 1 : _lower.CompareTo(other);
    public readonly int CompareTo(object obj) => obj is UInt128 u ? CompareTo(u) : throw new InvalidCastException("invalid type"); 

    private static bool LessThan(UInt128 a, long b) => b >= 0 && a._upper == 0 && a._lower < (ulong)b;
    private static bool LessThan(long a, UInt128 b) => a < 0 || b._upper != 0 || (ulong)a < b._lower;
    private static bool LessThan(UInt128 a, UInt128 b) => a._upper != b._upper ? a._upper < b._upper : a._lower < b._lower;

    public readonly bool Equals(UInt128 other) => _lower == other._lower && _upper == other._upper;
    public readonly bool Equals(long other) => other >= 0 && _lower == (ulong)other && _upper == 0;
    public readonly bool Equals(ulong other) => _lower == other && _upper == 0;    
    public override readonly bool Equals(object obj) => obj is UInt128 _uint128 && Equals(_uint128); 

    public override readonly int GetHashCode() => HashCode.Combine(_lower.GetHashCode(), _upper.GetHashCode());


    // Arithmetic Functions

    // Addition -> moved to UInt128.Addition.cs

    // Subtraction -> moved to UInt128.Subtraction.cs

    // Multiplication -> moved to UInt128.Multiplication.cs

    // Division -> moved to UInt128.Division.cs

    // Bitwise -> moved to UInt128.BitOperations.cs
}

}