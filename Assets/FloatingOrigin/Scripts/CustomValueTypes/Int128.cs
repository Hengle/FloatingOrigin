// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace BigIntegers
{


/// <summary>
/// 128-bit unsigned integer. 
/// </summary>
/// <remarks>
/// Maximum size is 170141183460469231731687303715884105727 or around 170 undecillion.<br/>
/// Minimum size is -170141183460469231731687303715884105727 or around -170 undecillion.<br/>
/// </remarks> 
public struct Int128 : IFormattable, IComparable, IComparable<Int128>, IEquatable<Int128>
{
    const MethodImplOptions Inline = MethodImplOptions.AggressiveInlining;

    private UInt128 v;

    public static readonly Int128 MinValue = (Int128)((UInt128)1 << 127);
    public static readonly Int128 MaxValue = (Int128)(((UInt128)1 << 127) - 1);
    public static readonly Int128 Zero = (Int128)0;
    public static readonly Int128 One = (Int128)1;
    public static readonly Int128 MinusOne = (Int128)(-1);

    public readonly bool IsZero => v.IsZero;
    public readonly bool IsOne => v.IsOne; 
    public readonly bool IsPowerOfTwo => v.IsPowerOfTwo; 
    public readonly bool IsEven => v.IsEven; 
    public readonly bool IsNegative => v._upper > long.MaxValue; 
    public readonly int Sign => v._upper > long.MaxValue ? -1 : v.Sign; 


    // Constructors
    public Int128(long value) => v = new UInt128(value);
    public Int128(ulong value) => v = new UInt128(value);
    public Int128(ulong lo, ulong hi) => v = new UInt128(lo, hi);
    public Int128(double value) => v = new UInt128(value);
    public Int128(decimal value) => v = new UInt128(value);
    public Int128(BigInteger value) => v = new UInt128(value);
    public Int128(UInt128 value) => v = value;


    // Parsing
    public static Int128 Parse(string value)
    {
        if (!TryParse(value, out Int128 c))
            throw new FormatException();
        return c;
    }

    public static bool TryParse(string value, out Int128 result) 
    {
        return TryParse(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
    }

    public static bool TryParse(string value, NumberStyles style, IFormatProvider format, out Int128 result)
    {
        BigInteger a;
        if (!BigInteger.TryParse(value, style, format, out a))
        {
            result = Zero;
            return false;
        }
        result.v = new UInt128(a);
        return true;
    }

    // Formatting 
    public readonly override string ToString() => ((BigInteger)this).ToString();
    public readonly string ToString(string format) => ((BigInteger)this).ToString(format);
    public readonly string ToString(IFormatProvider provider) => ToString(null, provider);
    public readonly string ToString(string format, IFormatProvider provider) => ((BigInteger)this).ToString(format, provider);

    // Conversions

    // Unsigned to Int128
    public static implicit operator Int128(byte a) => new Int128((ulong)a);
    public static implicit operator Int128(ushort a) => new Int128((ulong)a);
    public static implicit operator Int128(uint a) => new Int128((ulong)a);
    public static implicit operator Int128(ulong a) => new Int128(a);
    public static explicit operator Int128(UInt128 a) => new Int128 { v = a };

    // Signed to Int128
    public static implicit operator Int128(sbyte a) => (a < 0) ? -new Int128(a) : new Int128(a);
    public static implicit operator Int128(short a) => (a < 0) ? -new Int128(a) : new Int128(a);
    public static implicit operator Int128(int a) => (a < 0) ? -new Int128(a) : new Int128(a);
    public static implicit operator Int128(long a) => (a < 0) ? -new Int128(a) : new Int128(a);
    public static explicit operator Int128(BigInteger a) => (a < 0) ? -new Int128(a) : new Int128(a);

    // Floating-Point to Int128
    public static explicit operator Int128(double a) => (a < 0) ? -new Int128(a) : new Int128(a); 
    public static explicit operator Int128(decimal a) => (a < 0) ? -new Int128(a) : new Int128(a);

    // Int128 to Unsigned
    public static explicit operator byte(Int128 a) => (byte)a.v._lower;
    public static explicit operator ushort(Int128 a) => (ushort)a.v._lower;
    public static explicit operator uint(Int128 a) => (uint)a.v._lower;
    public static explicit operator ulong(Int128 a) => a.v._lower;
    public static explicit operator UInt128(Int128 a) => a.v;

    // Int128 to Signed
    public static explicit operator sbyte(Int128 a) => (sbyte)a.v._lower;
    public static explicit operator short(Int128 a) => (short)a.v._lower;
    public static explicit operator int(Int128 a) => (int)a.v._lower;
    public static explicit operator long(Int128 a) => (long)a.v._lower;
    public static explicit operator BigInteger(Int128 a) => a.v._upper > long.MaxValue ? -(BigInteger)(-a.v) : (BigInteger)a.v;

    // Int128 to Floating-Point
    public static explicit operator float(Int128 a) => a.v._upper > long.MaxValue ? -(float)-a.v : (float)a.v;
    public static explicit operator double(Int128 a) => a.v._upper > long.MaxValue ? -(double)-a.v : (double)a.v;
    public static explicit operator decimal(Int128 a) => a.v._upper > long.MaxValue ? -(decimal)-a.v : (decimal)a.v;

    // Bitwise Operators- Inline to use direct function call
    [MethodImpl(Inline)] public static Int128 operator <<(Int128 a, int b) => new Int128 { v = a.v << b };
    [MethodImpl(Inline)] public static Int128 operator >>(Int128 a, int b) => new Int128 { v = a.v >> b };
    [MethodImpl(Inline)] public static Int128 operator &(Int128 a, Int128 b) => new Int128 { v = a.v & b.v };
    [MethodImpl(Inline)] public static Int128 operator |(Int128 a, Int128 b) => new Int128 { v = a.v | b.v };
    [MethodImpl(Inline)] public static Int128 operator ^(Int128 a, Int128 b) => new Int128 { v = a.v ^ b.v };
    [MethodImpl(Inline)] public static Int128 operator ~(Int128 a) => new Int128 { v = ~a.v };

    // Arithmetic Operators- Inline to use direct function call
    
    // Addition
    [MethodImpl(Inline)] public static Int128 operator +(Int128 a, long b) => Add(a, b);
    [MethodImpl(Inline)] public static Int128 operator +(Int128 a, ulong b) => Add(a, b);
    [MethodImpl(Inline)] public static Int128 operator +(long a, Int128 b) => Add(b, a);
    [MethodImpl(Inline)] public static Int128 operator +(ulong a, Int128 b) => Add(b, a);
    [MethodImpl(Inline)] public static Int128 operator +(Int128 a, Int128 b) => Add(a, b);
    [MethodImpl(Inline)] public static Int128 operator +(Int128 a) => a;
    [MethodImpl(Inline)] public static Int128 operator ++(Int128 a) => Add(a, 1);

    // Subtraction
    [MethodImpl(Inline)] public static Int128 operator -(Int128 a, long b) => Subtract(a, b);
    [MethodImpl(Inline)] public static Int128 operator -(Int128 a, ulong b) => Subtract(a, b);
    [MethodImpl(Inline)] public static Int128 operator -(Int128 a, Int128 b) => Subtract(a, b);
    [MethodImpl(Inline)] public static Int128 operator -(Int128 a) => new Int128 { v = -a.v };
    [MethodImpl(Inline)] public static Int128 operator --(Int128 a) => Subtract(a, 1);

    // Multiplication
    [MethodImpl(Inline)] public static Int128 operator *(Int128 a, long b) => Multiply(a, b);
    [MethodImpl(Inline)] public static Int128 operator *(Int128 a, ulong b) => Multiply(a, b);
    [MethodImpl(Inline)] public static Int128 operator *(long a, Int128 b) => Multiply(b, a);
    [MethodImpl(Inline)] public static Int128 operator *(ulong a, Int128 b) => Multiply(b, a);
    [MethodImpl(Inline)] public static Int128 operator *(Int128 a, Int128 b) => Multiply(a, b);

    // Division
    [MethodImpl(Inline)] public static Int128 operator /(Int128 a, long b) => Divide(a, b);
    [MethodImpl(Inline)] public static Int128 operator /(Int128 a, ulong b) => Divide(a, b);
    [MethodImpl(Inline)] public static Int128 operator /(Int128 a, Int128 b) => Divide(a, b);

    // Modulus
    [MethodImpl(Inline)] public static long operator %(Int128 a, long b) => Remainder(a, b);
    [MethodImpl(Inline)] public static long operator %(Int128 a, ulong b) => Remainder(a, b);
    [MethodImpl(Inline)] public static Int128 operator %(Int128 a, Int128 b) => Remainder(a, b);

    // Comparision Operators

    // Less
    public static bool operator <(Int128 a, UInt128 b) => a.CompareTo(b) < 0;
    public static bool operator <(UInt128 a, Int128 b) => b.CompareTo(a) > 0;
    public static bool operator <(Int128 a, Int128 b) => LessThan(a.v, b.v);
    public static bool operator <(Int128 a, long b) => LessThan(a.v, b);
    public static bool operator <(long a, Int128 b) => LessThan(a, b.v);
    public static bool operator <(Int128 a, ulong b) => LessThan(a.v, b);
    public static bool operator <(ulong a, Int128 b) => LessThan(a, b.v);

    // Less Equals
    public static bool operator <=(Int128 a, UInt128 b) => a.CompareTo(b) <= 0;
    public static bool operator <=(UInt128 a, Int128 b) => b.CompareTo(a) >= 0;
    public static bool operator <=(Int128 a, Int128 b) => !LessThan(b.v, a.v);
    public static bool operator <=(Int128 a, long b) => !LessThan(b, a.v);
    public static bool operator <=(long a, Int128 b) => !LessThan(b.v, a);
    public static bool operator <=(Int128 a, ulong b) => !LessThan(b, a.v);
    public static bool operator <=(ulong a, Int128 b) => !LessThan(b.v, a);

    // Greater
    public static bool operator >(Int128 a, UInt128 b) => a.CompareTo(b) > 0;
    public static bool operator >(UInt128 a, Int128 b) => b.CompareTo(a) < 0;
    public static bool operator >(Int128 a, Int128 b) => LessThan(b.v, a.v);
    public static bool operator >(Int128 a, long b) => LessThan(b, a.v);
    public static bool operator >(long a, Int128 b) => LessThan(b.v, a);
    public static bool operator >(Int128 a, ulong b) => LessThan(b, a.v);
    public static bool operator >(ulong a, Int128 b) => LessThan(b.v, a);

    // Greater Equals
    public static bool operator >=(Int128 a, UInt128 b) => a.CompareTo(b) >= 0;
    public static bool operator >=(UInt128 a, Int128 b) => b.CompareTo(a) <= 0;
    public static bool operator >=(Int128 a, Int128 b) => !LessThan(a.v, b.v);
    public static bool operator >=(Int128 a, long b) => !LessThan(a.v, b);
    public static bool operator >=(long a, Int128 b) => !LessThan(a, b.v);
    public static bool operator >=(Int128 a, ulong b) => !LessThan(a.v, b);
    public static bool operator >=(ulong a, Int128 b) => !LessThan(a, b.v);

    // Equals
    public static bool operator ==(UInt128 a, Int128 b) => b.Equals(a);
    public static bool operator ==(Int128 a, UInt128 b) => a.Equals(b);
    public static bool operator ==(Int128 a, Int128 b) => a.v.Equals(b.v);
    public static bool operator ==(Int128 a, long b) => a.Equals(b);
    public static bool operator ==(long a, Int128 b) => b.Equals(a);
    public static bool operator ==(Int128 a, ulong b) => a.Equals(b);
    public static bool operator ==(ulong a, Int128 b) => b.Equals(a);

    // Not Equals
    public static bool operator !=(UInt128 a, Int128 b) => !b.Equals(a);
    public static bool operator !=(Int128 a, UInt128 b) => !a.Equals(b);
    public static bool operator !=(Int128 a, Int128 b) => !a.v.Equals(b.v);
    public static bool operator !=(Int128 a, long b) => !a.Equals(b);
    public static bool operator !=(long a, Int128 b) => !b.Equals(a);
    public static bool operator !=(Int128 a, ulong b) => !a.Equals(b);
    public static bool operator !=(ulong a, Int128 b) => !b.Equals(a);

    public readonly int CompareTo(UInt128 other)
    {
        if (v._upper > long.MaxValue)
            return -1;
        return v.CompareTo(other);
    }

    public readonly int CompareTo(Int128 other) => SignedCompare(v, other.v._lower, other.v._upper);
    public readonly int CompareTo(int other) => SignedCompare(v, (ulong)other, (ulong)(other >> 31));
    public readonly int CompareTo(uint other) => SignedCompare(v, other, 0);
    public readonly int CompareTo(long other) => SignedCompare(v, (ulong)other, (ulong)(other >> 63));
    public readonly int CompareTo(ulong other) => SignedCompare(v, other, 0);

    public readonly int CompareTo(object obj)
    {
        if (obj == null)
            return 1;
            
        if (obj is not Int128)
            throw new ArgumentException();

        return CompareTo((Int128)obj);
    }


    private static bool LessThan(UInt128 a, UInt128 b) => a._upper != b._upper ? (long)a._upper < (long)b._upper : a._lower < b._lower;
    private static bool LessThan(UInt128 a, long b) => (long)a._upper != (b >> 63) ? (long)a._upper < (b >> 63) : a._lower < (ulong)b;
    private static bool LessThan(long a, UInt128 b) => (a >> 63) != (long)b._upper ? (a >> 63) < (long)b._upper : (ulong)a < b._lower;
    private static bool LessThan(UInt128 a, ulong b) => ((long)a._upper != 0) ? ((long)a._upper < 0) : (a._lower < b);
    private static bool LessThan(ulong a, UInt128 b) => (0 != (long)b._upper) ? (0 < (long)b._upper) : (a < b._lower);

    private static int SignedCompare(UInt128 a, ulong bs0, ulong bs1) => (a._upper != bs1) ? ((long)a._upper).CompareTo((long)bs1) : a._lower.CompareTo(bs0);

    public readonly bool Equals(UInt128 other) => !(v._upper > long.MaxValue) && v.Equals(other);
    public readonly bool Equals(ulong other) => v._upper == 0 && v._lower == other;
    public readonly bool Equals(Int128 other) => v.Equals(other.v);

    public readonly bool Equals(long other)
    {
        if (other < 0)
            return v._upper == ulong.MaxValue && v._lower == (ulong)other;
        return v._upper == 0 && v._lower == (ulong)other;
    }


    public readonly override bool Equals(object obj)
    {
        if (obj is not Int128)
            return false;

        return v.Equals(((Int128)obj).v);
    }

    public readonly override int GetHashCode() => v.GetHashCode();


    // Arithmetic Functions- A.K.A atrocious one-liner warcrimes.
    // These were readable at one point but I gave readability up to cram them together. 
    // Check out the un-fucked version here: https://github.com/ricksladkey/dirichlet-numerics/blob/master/Dirichlet.Numerics/Int128.cs

    // Inline all of these since it's faster than calling functions and copying around values.

    // This hopefully means that an addition compiles like so: (a + b) -> Int128.Add(a, b) -> new Int128 { v = a.v + b } -> new Int128 { v = UInt128.Add(a.v + b) }

    // Addition
    [MethodImpl(Inline)] 
    private static Int128 Add(Int128 a, ulong b) => new Int128 { v = a.v + b };

    [MethodImpl(Inline)] 
    private static Int128 Add(Int128 a, long b) => new Int128 { v = b < 0 ? a.v - (ulong)-b : a.v + (ulong)b };

    [MethodImpl(Inline)] 
    private static Int128 Add(Int128 a, Int128 b) => new Int128 { v = a.v + b.v };

    // Subtraction
    [MethodImpl(Inline)] 
    private static Int128 Subtract(Int128 a, ulong b) => new Int128 { v = a.v - b };

    [MethodImpl(Inline)] 
    public static Int128 Subtract(Int128 a, long b) => new Int128 { v = b < 0 ? a.v + (ulong)-b : a.v - (ulong)b };

    [MethodImpl(Inline)] 
    private static Int128 Subtract(Int128 a, Int128 b) => new Int128 { v = a.v - b.v };

    // Multiplication
    [MethodImpl(Inline)] 
    public static Int128 Multiply(Int128 a, long b) => new Int128 { v = a.v._upper > long.MaxValue ? (b < 0 ? ((-a.v) * (ulong)-b) : -((-a.v) * (ulong)b)) : (b < 0 ? -(a.v * (ulong)-b) : a.v * (ulong)b) };

    [MethodImpl(Inline)] 
    public static Int128 Multiply(Int128 a, ulong b) => new Int128 { v = a.v._upper > long.MaxValue ? -(-a.v * b) : (a.v * b) };

    [MethodImpl(Inline)] 
    public static Int128 Multiply(Int128 a, Int128 b) => new Int128 { v = a.v._upper > long.MaxValue ? (b.v._upper > long.MaxValue ? (-a.v * -b.v) : -(-a.v * b.v)) : (b.v._upper > long.MaxValue ? -(a.v * -b.v) : (a.v * b.v)) };

    // Division
    [MethodImpl(Inline)] 
    public static Int128 Divide(Int128 a, long b) => new Int128 { v = a.v._upper > long.MaxValue ? (b < 0 ? (-a.v / (ulong)-b) : -(-a.v / (ulong)b)) : (b < 0 ? -(a.v / (ulong)-b) : (a.v / (ulong)b)) };

    [MethodImpl(Inline)] 
    public static Int128 Divide(Int128 a, ulong b) => new Int128 { v = a.v._upper > long.MaxValue ? -(-a.v / b) : (a.v / b) };
    
    [MethodImpl(Inline)] 
    public static Int128 Divide(Int128 a, Int128 b) => new Int128 { v = a.v._upper > long.MaxValue ? (b.v._upper > long.MaxValue ? (-a.v / -b.v) : -(-a.v / b.v)) : (b.v._upper > long.MaxValue ? -(a.v / -b.v) : (a.v / b.v)) };


    // Modulus
    [MethodImpl(Inline)]
    public static long Remainder(Int128 a, long b) => a.v._upper > long.MaxValue ? (b < 0 ? (long)(-a.v % (ulong)-b) : -(long)(-a.v % (ulong)b)) : (b < 0 ? -(long)(a.v % (ulong)-b) : (long)(a.v % (ulong)b));
    
    [MethodImpl(Inline)] 
    public static long Remainder(Int128 a, ulong b) => a.v._upper > long.MaxValue ? -(long)(-a.v % b) : (long)(a.v % b);
    
    [MethodImpl(Inline)] 
    public static Int128 Remainder(Int128 a, Int128 b) => new Int128 { v = a.v._upper > long.MaxValue ? (b.v._upper > long.MaxValue ? (-a.v % -b.v) : -(-a.v % b.v)) : (b.v._upper > long.MaxValue ? -(a.v % -b.v) : (a.v % b.v)) };


    public static Int128 Pow(Int128 value, int exponent)
    {
        Int128 c;

        if (exponent < 0)
            throw new ArgumentException("exponent cannot be negative");

        if (value.v._upper > long.MaxValue)
        {
            if ((exponent & 1) == 0)
                c.v = UInt128.Pow(-value.v, (uint)exponent);
            else
                c.v = -(UInt128.Pow(-value.v, (uint)exponent));
        }
        else
            c.v = UInt128.Pow(value.v, (uint)exponent);
        
        return c;
    }
}

}