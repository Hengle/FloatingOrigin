using System;
using System.Numerics;

namespace BigIntegers
{

public partial struct UInt128
{
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

    public UInt128(decimal value)
    {
        int[] bits = decimal.GetBits(decimal.Truncate(value));

        _lower = (ulong)(uint)bits[1] << 32 | (uint)bits[0];
        _upper = (ulong)0 << 32 | (uint)bits[2];

        if (value < 0)
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


    public static BigInteger ToBigInt(UInt128 a)
    {
        if (a._upper == 0)
            return a._lower;
        return (BigInteger)a._upper << 64 | a._lower;
    }

    public static float ToFloat(UInt128 a)
    {
        if (a._upper == 0)
            return a._lower;
        return a._upper * (float)ulong.MaxValue + a._lower;
    }

    public static double ToDouble(UInt128 a)
    {
        if (a._upper == 0)
            return a._lower;
        return a._upper * (double)ulong.MaxValue + a._lower;
    }

    public static decimal ToDecimal(UInt128 a)
    {
        if (a._upper == 0)
            return a._lower;
        var shift = Math.Max(0, 32 - GetBitLength(a._upper));
        return new decimal((int)a.r0, (int)a.r1, (int)a.r2, false, (byte)shift);
    }
}

}