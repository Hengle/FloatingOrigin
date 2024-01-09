// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace CustomTypes
{

public struct UInt128 : IFormattable, IComparable, IComparable<UInt128>, IEquatable<UInt128>
{
    const MethodImplOptions Inline = MethodImplOptions.AggressiveInlining;

    public ulong _lower;
    public ulong _upper;


    private readonly uint r0 => (uint)_lower;
    private readonly uint r1 => (uint)(_lower >> 32);
    private readonly uint r2 => (uint)_upper;
    private readonly uint r3 => (uint)(_upper >> 32);

    public readonly bool IsZero => (_lower | _upper) == 0;
    public readonly bool IsOne => _upper == 0 && _lower == 1;
    public readonly bool IsPowerOfTwo => (this & (this - 1)).IsZero;
    public readonly bool IsEven => (_lower & 1) == 0;
    public readonly int Sign => IsZero ? 0 : 1;


    public static readonly UInt128 MaxValue = ~(UInt128)0;
    public static readonly UInt128 MinValue = (UInt128)0;
    public static readonly UInt128 Zero = (UInt128)0;
    public static readonly UInt128 One = (UInt128)1;



    // Constructors
    public UInt128(uint r0, uint r1, uint r2, uint r3)
    {
        _lower = (ulong)r1 << 32 | r0;
        _upper = (ulong)r3 << 32 | r2;
    }

    public UInt128(ulong lower, ulong upper)
    {
        _lower = lower;
        _upper = upper;
    }

    public UInt128(long value) 
    {
        _lower = (ulong)value;
        _upper = value < 0 ? ulong.MaxValue : 0;
    }

    public UInt128(ulong value) 
    {
        _lower = value;
        _upper = 0;
    }

    public UInt128(decimal value)
    {
        var bits = decimal.GetBits(decimal.Truncate(value));

        _lower = (ulong)(uint)bits[1] << 32 | (uint)bits[0];
        _upper = (ulong)0 << 32 | (uint)bits[2];

        if (value < 0)
            this = Negate(this);
    }

    public UInt128(double value) 
    {
        var negate = false;
        if (value < 0)
        {
            negate = true;
            value = -value;
        }
        
        if (value <= ulong.MaxValue)
        {
            _lower = (ulong)value;
            _upper = 0;
        }
        else
        {
            var shift = Math.Max((int)Math.Ceiling(Math.Log(value, 2)) - 63, 0);
            _lower = (ulong)(value / Math.Pow(2, shift));
            _upper = 0;
            this <<= shift;
        }

        if (negate)
            this = Negate(this);
    }

    public UInt128(BigInteger value) 
    {
        var sign = value.Sign;
        if (sign == -1)
            value = -value;

        _lower = (ulong)(value & ulong.MaxValue);
        _upper = (ulong)(value >> 64);

        if (sign == -1)
            this = Negate(this);
    }

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

    public static implicit operator BigInteger(UInt128 a)
    {
        if (a._upper == 0)
            return a._lower;
        return (BigInteger)a._upper << 64 | a._lower;
    }

    // UInt128 to Floating-Point
    public static explicit operator float(UInt128 a)
    {
        if (a._upper == 0)
            return a._lower;
        return a._upper * (float)ulong.MaxValue + a._lower;
    }

    public static explicit operator double(UInt128 a)
    {
        if (a._upper == 0)
            return a._lower;
        return a._upper * (double)ulong.MaxValue + a._lower;
    }

    public static explicit operator decimal(UInt128 a)
    {
        if (a._upper == 0)
            return a._lower;
        var shift = Math.Max(0, 32 - GetBitLength(a._upper));
        return new decimal((int)a.r0, (int)a.r1, (int)a.r2, false, (byte)shift);
    }

    // Bitwise Operators
    [MethodImpl(Inline)] 
    public static UInt128 operator <<(UInt128 a, int b) => LeftShift(a, b);

    [MethodImpl(Inline)] 
    public static UInt128 operator >>(UInt128 a, int b) => RightShift(a, b);

    [MethodImpl(Inline)] 
    public static UInt128 operator &(UInt128 a, UInt128 b) => And(a, b);

    [MethodImpl(Inline)] 
    public static UInt128 operator |(UInt128 a, UInt128 b) => Or(a, b);

    [MethodImpl(Inline)] 
    public static UInt128 operator ^(UInt128 a, UInt128 b) => ExclusiveOr(a, b);

    [MethodImpl(Inline)] 
    public static UInt128 operator ~(UInt128 a) => Not(a);

    // Arithmetic Operators

    // Addition
    [MethodImpl(Inline)] 
    public static UInt128 operator +(UInt128 a, UInt128 b) => Add(a, b);

    [MethodImpl(Inline)] 
    public static UInt128 operator +(UInt128 a, ulong b) => Add(a, b);

    [MethodImpl(Inline)] 
    public static UInt128 operator +(ulong a, UInt128 b) => Add(b, a);
    
    [MethodImpl(Inline)] 
    public static UInt128 operator +(UInt128 a) => a;

    [MethodImpl(Inline)] 
    public static UInt128 operator ++(UInt128 a) => Add(a, 1);

    // Subtraction
    [MethodImpl(Inline)] 
    public static UInt128 operator -(UInt128 a, UInt128 b) => Subtract(a, b);

    [MethodImpl(Inline)] 
    public static UInt128 operator -(UInt128 a, ulong b) => Subtract(a, b);

    [MethodImpl(Inline)] 
    public static UInt128 operator -(ulong a, UInt128 b) => Subtract(a, b);

    [MethodImpl(Inline)] 
    public static UInt128 operator -(UInt128 a) => Negate(a);

    [MethodImpl(Inline)] 
    public static UInt128 operator --(UInt128 a) => Subtract(a, 1);

    // Multiplication
    [MethodImpl(Inline)] 
    public static UInt128 operator *(UInt128 a, ulong b) => Multiply(a, b);

    [MethodImpl(Inline)] 
    public static UInt128 operator *(ulong a, UInt128 b) => Multiply(b, a);

    [MethodImpl(Inline)] 
    public static UInt128 operator *(UInt128 a, UInt128 b) => Multiply(a, b);

    // Division
    [MethodImpl(Inline)] 
    public static UInt128 operator /(UInt128 a, ulong b) => Divide(a, b);

    [MethodImpl(Inline)] 
    public static UInt128 operator /(UInt128 a, UInt128 b) => Divide(a, b);

    // Modulus
    [MethodImpl(Inline)] 
    public static ulong operator %(UInt128 a, ulong b) => Remainder(a, b);

    [MethodImpl(Inline)] 
    public static UInt128 operator %(UInt128 a, UInt128 b) => Remainder(a, b);

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
    public readonly int CompareTo(object obj) => obj is UInt128 _uint128 ? CompareTo(_uint128) : throw new InvalidCastException("invalid type"); 

    private static bool LessThan(UInt128 a, long b) => b >= 0 && a._upper == 0 && a._lower < (ulong)b;
    private static bool LessThan(long a, UInt128 b) => a < 0 || b._upper != 0 || (ulong)a < b._lower;
    private static bool LessThan(UInt128 a, UInt128 b) => a._upper != b._upper ? a._upper < b._upper : a._lower < b._lower;

    public readonly bool Equals(UInt128 other) => _lower == other._lower && _upper == other._upper;
    public readonly bool Equals(long other) => other >= 0 && _lower == (ulong)other && _upper == 0;
    public readonly bool Equals(ulong other) => _lower == other && _upper == 0;
    
    public override readonly bool Equals(object obj) => obj is UInt128 _uint128 && Equals(_uint128); 

    public override readonly int GetHashCode() => _lower.GetHashCode() ^ _upper.GetHashCode();


    // Arithmetic Functions

    // Addition
    private static UInt128 Add(UInt128 a, ulong b)
    {
        UInt128 c = new UInt128(a._lower + b, a._upper);
        if (c._lower < a._lower && c._lower < b)
            ++c._upper;
            
        return c;
    }
    
    private static UInt128 Add(UInt128 a, UInt128 b)
    {
        UInt128 c = new UInt128(a._lower + b._lower, a._upper + b._upper);
        if (c._lower < a._lower && c._lower < b._lower)
            ++c._upper;

        return c;
    }

    // Subtraction
    private static UInt128 Subtract(UInt128 a, ulong b)
    {
        UInt128 c = new UInt128(a._lower - b, a._upper);
        if (a._lower < b)
            --c._upper;
        
        return c;
    }
    
    private static UInt128 Subtract(ulong a, UInt128 b)
    {
        UInt128 c = new UInt128(a - b._lower, a - b._upper);
        if (a < b._lower)
            --c._upper;
        
        return c;
    }

    private static UInt128 Subtract(UInt128 a, UInt128 b)
    {
        UInt128 c = new UInt128(a._lower - b._lower, a._upper - b._upper);
        if (a._lower < b._lower)
            --c._upper;
        
        return c;
    }

    // Multiplication
    private static UInt128 Multiply64(ulong u, ulong v)
    {
        ulong u0 = (uint)u;
        ulong u1 = u >> 32;
        ulong v0 = (uint)v;
        ulong v1 = v >> 32;

        ulong carry = u0 * v0;
        ulong r0 = (uint)carry;
        carry = (carry >> 32) + u0 * v1;
        ulong r2 = carry >> 32;
        carry = (uint)carry + u1 * v0;

        return new UInt128(carry << 32 | r0, (carry >> 32) + r2 + u1 * v1);
    }

    private static UInt128 Multiply128(UInt128 u, ulong v)
    {
        UInt128 w = Multiply64(u._lower, v);
        w._upper += u._upper * v;
        return w;
    }

    private static UInt128 Multiply128(UInt128 u, UInt128 v)
    {
        UInt128 w = Multiply64(u._lower, v._lower);
        w._upper += u._upper * v._lower + u._lower * v._upper;
        return w;
    }

    private static UInt128 Multiply(UInt128 a, ulong b)
    {
        if (a._upper == 0)
            return Multiply64(a._lower, b);
        else
            return Multiply128(a, b);
    }

    private static UInt128 Multiply(UInt128 a, UInt128 b)
    {
        if ((a._upper | b._upper) == 0)
            return Multiply64(a._lower, b._lower);
        else if (a._upper == 0)
            return Multiply128(b, a._lower);
        else if (b._upper == 0)
            return Multiply128(a, b._lower);
        else
            return Multiply128(a, b);
    }

    // Division
    private static void Divide64(out UInt128 c, ulong u, ulong v)
    {
        c._lower = u / v;
        c._upper = 0;
    }

    private static void Divide96(out UInt128 c, UInt128 u, uint v)
    {
        var r2 = u.r2;
        var w2 = r2 / v;
        var u0 = (ulong)(r2 - w2 * v);
        var u0u1 = u0 << 32 | u.r1;
        var w1 = (uint)(u0u1 / v);
        u0 = u0u1 - w1 * v;
        u0u1 = u0 << 32 | u.r0;
        var w0 = (uint)(u0u1 / v);

        c._lower = w2;
        c._upper = (ulong)w1 << 32 | w0;
    }

    private static void Divide128(out UInt128 c, UInt128 u, uint v)
    {
        var r3 = u.r3;
        var w3 = r3 / v;
        var u0 = (ulong)(r3 - w3 * v);
        var u0u1 = u0 << 32 | u.r2;
        var w2 = (uint)(u0u1 / v);
        u0 = u0u1 - w2 * v;
        u0u1 = u0 << 32 | u.r1;
        var w1 = (uint)(u0u1 / v);
        u0 = u0u1 - w1 * v;
        u0u1 = u0 << 32 | u.r0;
        var w0 = (uint)(u0u1 / v);

        c._lower = (ulong)w3 << 32 | w2;
        c._upper = (ulong)w1 << 32 | w0;
    }

    private static void Divide96(out UInt128 c, UInt128 u, ulong v)
    {
        c._lower = c._upper = 0;
        var dneg = GetBitLength((uint)(v >> 32));
        var d = 32 - dneg;
        var vPrime = v << d;
        var v1 = (uint)(vPrime >> 32);
        var v2 = (uint)vPrime;
        var r0 = u.r0;
        var r1 = u.r1;
        var r2 = u.r2;
        var r3 = (uint)0;

        if (d != 0)
        {
            r3 = r2 >> dneg;
            r2 = r2 << d | r1 >> dneg;
            r1 = r1 << d | r0 >> dneg;
            r0 <<= d;
        }

        var q1 = DivRem(r3, ref r2, ref r1, v1, v2);
        var q0 = DivRem(r2, ref r1, ref r0, v1, v2);

        c._lower = (ulong)q1 << 32 | q0;
        c._upper = 0;
    }

    private static void Divide128(out UInt128 c, UInt128 u, ulong v)
    {
        c._lower = c._upper = 0;
        var dneg = GetBitLength((uint)(v >> 32));
        var d = 32 - dneg;
        var vPrime = v << d;
        var v1 = (uint)(vPrime >> 32);
        var v2 = (uint)vPrime;
        var r0 = u.r0;
        var r1 = u.r1;
        var r2 = u.r2;
        var r3 = u.r3;
        var r4 = (uint)0;
    
        if (d != 0)
        {
            r4 = r3 >> dneg;
            r3 = r3 << d | r2 >> dneg;
            r2 = r2 << d | r1 >> dneg;
            r1 = r1 << d | r0 >> dneg;
            r0 <<= d;
        }

        c._upper = DivRem(r4, ref r3, ref r2, v1, v2);
        var q1 = DivRem(r3, ref r2, ref r1, v1, v2);
        var q0 = DivRem(r2, ref r1, ref r0, v1, v2);
        c._lower = (ulong)q1 << 32 | q0;
    }

    private static UInt128 Divide(UInt128 u, ulong v)
    {
        UInt128 c;

        if (u._upper == 0)
            Divide64(out c, u._lower, v);
        else
        {
            var v0 = (uint)v;
            if (v == v0)
            {
                if (u._upper <= uint.MaxValue)
                    Divide96(out c, u, v0);
                else
                    Divide128(out c, u, v0);
            }

            if (u._upper <= uint.MaxValue)
                Divide96(out c, u, v);
            else
                Divide128(out c, u, v);
        }

        return c;
    }

    private static UInt128 Divide(UInt128 a, UInt128 b)
    {
        if (LessThan(a, b))
            return Zero;
        
        if (b._upper == 0)
            return Divide(a, b._lower);
        
        if (b._upper <= uint.MaxValue)
            return new UInt128(DivRem96(out _, a, b));
        
        return new UInt128(DivRem128(out _, a, b));
    }

    // Modulus
    private static ulong Q(uint u0, uint u1, uint u2, uint v1, uint v2)
    {
        var u0u1 = (ulong)u0 << 32 | u1;
        var qhat = u0 == v1 ? uint.MaxValue : u0u1 / v1;
        var r = u0u1 - qhat * v1;

        if (r == (uint)r && v2 * qhat > (r << 32 | u2))
        {
            --qhat;
            r += v1;
            if (r == (uint)r && v2 * qhat > (r << 32 | u2))
                --qhat;
        }

        return qhat;
    }

    private static uint DivRem(uint u0, ref uint u1, ref uint u2, uint v1, uint v2)
    {
        var qhat = Q(u0, u1, u2, v1, v2);
        var carry = qhat * v2;
        var borrow = (long)u2 - (uint)carry;
        carry >>= 32;

        u2 = (uint)borrow;
        borrow >>= 32;
        carry += qhat * v1;
        borrow += (long)u1 - (uint)carry;
        carry >>= 32;

        u1 = (uint)borrow;
        borrow >>= 32;
        borrow += (long)u0 - (uint)carry;

        if (borrow != 0)
        {
            --qhat;
            carry = (ulong)u2 + v2;
            u2 = (uint)carry;
            carry >>= 32;
            carry += (ulong)u1 + v1;
            u1 = (uint)carry;
        }

        return (uint)qhat;
    }

    private static uint DivRem(uint u0, ref uint u1, ref uint u2, ref uint u3, uint v1, uint v2, uint v3)
    {
        var qhat = Q(u0, u1, u2, v1, v2);
        var carry = qhat * v3;
        var borrow = (long)u3 - (uint)carry;
        carry >>= 32;

        u3 = (uint)borrow;
        borrow >>= 32;
        carry += qhat * v2;
        borrow += (long)u2 - (uint)carry;
        carry >>= 32;

        u2 = (uint)borrow;
        borrow >>= 32;
        carry += qhat * v1;
        borrow += (long)u1 - (uint)carry;
        carry >>= 32;

        u1 = (uint)borrow;
        borrow >>= 32;
        borrow += (long)u0 - (uint)carry;

        if (borrow != 0)
        {
            --qhat;
            carry = (ulong)u3 + v3;
            u3 = (uint)carry;
            carry >>= 32;
            carry += (ulong)u2 + v2;
            u2 = (uint)carry;
            carry >>= 32;
            carry += (ulong)u1 + v1;
            u1 = (uint)carry;
        }

        return (uint)qhat;
    }

    private static uint DivRem(uint u0, ref uint u1, ref uint u2, ref uint u3, ref uint u4, uint v1, uint v2, uint v3, uint v4)
    {
        var qhat = Q(u0, u1, u2, v1, v2);
        var carry = qhat * v4;
        var borrow = (long)u4 - (uint)carry;
        carry >>= 32;

        u4 = (uint)borrow;
        borrow >>= 32;
        carry += qhat * v3;
        borrow += (long)u3 - (uint)carry;
        carry >>= 32;

        u3 = (uint)borrow;
        borrow >>= 32;
        carry += qhat * v2;
        borrow += (long)u2 - (uint)carry;
        carry >>= 32;

        u2 = (uint)borrow;
        borrow >>= 32;
        carry += qhat * v1;
        borrow += (long)u1 - (uint)carry;
        carry >>= 32;

        u1 = (uint)borrow;
        borrow >>= 32;
        borrow += (long)u0 - (uint)carry;

        if (borrow != 0)
        {
            --qhat;
            carry = (ulong)u4 + v4;
            u4 = (uint)carry;
            carry >>= 32;
            carry += (ulong)u3 + v3;
            u3 = (uint)carry;
            carry >>= 32;
            carry += (ulong)u2 + v2;
            u2 = (uint)carry;
            carry >>= 32;
            carry += (ulong)u1 + v1;
            u1 = (uint)carry;
        }

        return (uint)qhat;
    }
    
    private static ulong Remainder(UInt128 u, ulong v)
    {
        if (u._upper == 0)
            return u._lower % v;

        var v0 = (uint)v;
        if (v == v0)
        {
            if (u._upper <= uint.MaxValue)
                return Remainder96(u, v0);

            return Remainder128(u, v0);
        }
        if (u._upper <= uint.MaxValue)
            return Remainder96(u, v);

        return Remainder128(u, v);
    }

    private static UInt128 Remainder(UInt128 a, UInt128 b)
    {
        if (LessThan(a, b))
            return a;

        if (b._upper == 0)
            return new UInt128(Remainder(a, b._lower));

        if (b._upper <= uint.MaxValue)
        {
            DivRem96(out UInt128 rem, a, b);
            return rem;
        }
        else
        {
            DivRem128(out UInt128 rem, a, b);
            return rem;
        }
    }

    private static uint Remainder96(UInt128 u, uint v)
    {
        var u0 = (ulong)(u.r2 % v);
        var u0u1 = u0 << 32 | u.r1;
        u0 = u0u1 % v;
        u0u1 = u0 << 32 | u.r0;
        return (uint)(u0u1 % v);
    }

    private static uint Remainder128(UInt128 u, uint v)
    {
        var u0 = (ulong)(u.r3 % v);
        var u0u1 = u0 << 32 | u.r2;
        u0 = u0u1 % v;
        u0u1 = u0 << 32 | u.r1;
        u0 = u0u1 % v;
        u0u1 = u0 << 32 | u.r0;
        return (uint)(u0u1 % v);
    }

    private static ulong Remainder96(UInt128 u, ulong v)
    {
        var dneg = GetBitLength((uint)(v >> 32));
        var d = 32 - dneg;
        var vPrime = v << d;
        var v1 = (uint)(vPrime >> 32);
        var v2 = (uint)vPrime;
        var r0 = u.r0;
        var r1 = u.r1;
        var r2 = u.r2;
        var r3 = (uint)0;
        if (d != 0)
        {
            r3 = r2 >> dneg;
            r2 = r2 << d | r1 >> dneg;
            r1 = r1 << d | r0 >> dneg;
            r0 <<= d;
        }
        DivRem(r3, ref r2, ref r1, v1, v2);
        DivRem(r2, ref r1, ref r0, v1, v2);
        return ((ulong)r1 << 32 | r0) >> d;
    }

    private static ulong Remainder128(UInt128 u, ulong v)
    {
        var dneg = GetBitLength((uint)(v >> 32));
        var d = 32 - dneg;
        var vPrime = v << d;
        var v1 = (uint)(vPrime >> 32);
        var v2 = (uint)vPrime;
        var r0 = u.r0;
        var r1 = u.r1;
        var r2 = u.r2;
        var r3 = u.r3;
        var r4 = (uint)0;
        if (d != 0)
        {
            r4 = r3 >> dneg;
            r3 = r3 << d | r2 >> dneg;
            r2 = r2 << d | r1 >> dneg;
            r1 = r1 << d | r0 >> dneg;
            r0 <<= d;
        }
        DivRem(r4, ref r3, ref r2, v1, v2);
        DivRem(r3, ref r2, ref r1, v1, v2);
        DivRem(r2, ref r1, ref r0, v1, v2);
        return ((ulong)r1 << 32 | r0) >> d;
    }

    private static ulong DivRem96(out UInt128 rem, UInt128 a, UInt128 b)
    {
        var d = 32 - GetBitLength(b.r2);
        UInt128 v;
        LeftShift64(out v, b, d);
        var r4 = (uint)LeftShift64(out rem, a, d);
        var v1 = v.r2;
        var v2 = v.r1;
        var v3 = v.r0;
        var r3 = rem.r3;
        var r2 = rem.r2;
        var r1 = rem.r1;
        var r0 = rem.r0;
        var q1 = DivRem(r4, ref r3, ref r2, ref r1, v1, v2, v3);
        var q0 = DivRem(r3, ref r2, ref r1, ref r0, v1, v2, v3);
        rem = new UInt128(r0, r1, r2, 0);
        var div = (ulong)q1 << 32 | q0;
        RightShift64(rem, d);
        return div;
    }

    private static uint DivRem128(out UInt128 rem, UInt128 a, UInt128 b)
    {
        var d = 32 - GetBitLength(b.r3);
        UInt128 v;
        LeftShift64(out v, b, d);
        var r4 = (uint)LeftShift64(out rem, a, d);
        var r3 = rem.r3;
        var r2 = rem.r2;
        var r1 = rem.r1;
        var r0 = rem.r0;
        var div = DivRem(r4, ref r3, ref r2, ref r1, ref r0, v.r3, v.r2, v.r1, v.r0);
        rem = new UInt128(r0, r1, r2, r3);
        RightShift64(rem, d);
        return div;
    }

    // Bitwise 
    private static readonly byte[] bitLength = Enumerable.Range(0, byte.MaxValue + 1)
        .Select(value =>
        {
            int count;
            for (count = 0; value != 0; count++)
                value >>= 1;
            return (byte)count;
        }).ToArray();


    private static int GetBitLength(uint value)
    {
        var tt = value >> 16;
        if (tt != 0)
        {
            var t = tt >> 8;
            if (t != 0)
                return bitLength[t] + 24;
            return bitLength[tt] + 16;
        }
        else
        {
            var t = value >> 8;
            if (t != 0)
                return bitLength[t] + 8;
            return bitLength[value];
        }
    }

    private static int GetBitLength(ulong value)
    {
        var r1 = value >> 32;
        if (r1 != 0)
            return GetBitLength((uint)r1) + 32;
        return GetBitLength((uint)value);
    }

    private static ulong LeftShift64(out UInt128 c, UInt128 a, int d)
    {
        if (d == 0)
        {
            c = a;
            return 0;
        }
        var dneg = 64 - d;
        c._upper = a._upper << d | a._lower >> dneg;
        c._lower = a._lower << d;
        return a._upper >> dneg;
    }

    private static UInt128 LeftShift(UInt128 a, int b)
    {
        if (b < 64)
        {
            LeftShift64(out UInt128 c, a, b);
            return c;
        }
        else if (b == 64)
            return new UInt128(0, a._lower);
        return new UInt128(0, a._lower << (b - 64));
    }

    private static UInt128 RightShift64(UInt128 a, int b)
    {
        if (b == 0)
            return a;
        
        return new UInt128(a._lower >> b | a._upper << (64 - b), a._upper >> b);
    }

    private static UInt128 RightShift(UInt128 a, int b)
    {
        if (b < 64)
            return RightShift64(a, b);
        else if (b == 64)
            return new UInt128(a._upper, 0);

        return new UInt128(a._upper >> (b - 64), 0);
    }

    private static UInt128 And(UInt128 a, UInt128 b) => new UInt128(a._lower & b._lower, a._upper & b._upper);
    private static UInt128 Or(UInt128 a, UInt128 b) => new UInt128(a._lower | b._lower, a._upper | b._upper);
    private static UInt128 ExclusiveOr(UInt128 a, UInt128 b) => new UInt128(a._lower ^ b._lower, a._upper ^ b._upper);
    private static UInt128 Not(UInt128 a) => new UInt128(~a._lower, ~a._upper);


    private static UInt128 Negate(UInt128 value)
    {
        var a = value;
        var s0 = a._lower;
        a._lower = 0 - s0;
        a._upper = 0 - a._upper;
        if (s0 > 0)
            --a._upper;

        return a;
    }
}

}