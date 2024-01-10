

// Fixed-Point Math using only a Int128 numeric type
using System;
using BigIntegers;

public struct FBig
{
    public Int128 RawValue;
    const int ScaleFactor = 12; // 12 is 4096

    public static readonly Int128 One = 1 << ScaleFactor;

    
    public static FBig OneF = new FBig(1, true);
    public static FBig PI = Raw(12868); 


    public FBig(Int128 StartingRawValue, bool UseMultiple) => RawValue = UseMultiple ? StartingRawValue << ScaleFactor : StartingRawValue;

    public FBig(double DoubleValue) => RawValue = (long)Math.Round((double)One * DoubleValue);

    public static FBig Raw(Int128 rawValue) => new FBig { RawValue = rawValue };


    public readonly long LongValue => (long)(RawValue >> ScaleFactor);
    public readonly double DoubleValue => (long)RawValue / (double)One;


    public static FBig Abs(FBig F) => F < 0 ? -F : F;

    public static FBig FromParts(int preDecimal, int postDecimal)
    {
        FBig f = new(preDecimal, true);

        if (postDecimal != 0)
            f.RawValue += (new FBig(postDecimal) / 1000).RawValue;

        return f;
    }



    // Arithmetic Operators
    public static FBig operator +(FBig one, FBig other) => Raw(one.RawValue + other.RawValue);
    public static FBig operator +(FBig one, long other) => one + (FBig)other;
    public static FBig operator +(long other, FBig one) => one + (FBig)other;

    public static FBig operator -(FBig value) => Raw(-value.RawValue);
    public static FBig operator -(FBig minuend, FBig subtrahend) => Raw(minuend.RawValue - subtrahend.RawValue);
    public static FBig operator -(FBig one, long other) => one - (FBig)other;
    public static FBig operator -(long other, FBig one) => (FBig)other - one;

    public static FBig operator *(FBig multiplicand, FBig multiplier) => Raw((multiplicand.RawValue * multiplier.RawValue) >> ScaleFactor);
    public static FBig operator *(FBig multiplicand, long multiplier) => multiplicand * (FBig)multiplier;
    public static FBig operator *(long multiplicand, FBig multiplier) => (FBig)multiplicand * multiplier;

    public static FBig operator /(FBig one, FBig other) => Raw((one.RawValue << ScaleFactor) / other.RawValue);
    public static FBig operator /(FBig one, long divisor) => one / (FBig)divisor;
    public static FBig operator /(long divisor, FBig one) => (FBig)divisor / one;

    public static FBig operator %(FBig one, FBig other) => Raw(one.RawValue % other.RawValue);
    public static FBig operator %(FBig one, long divisor) => one % (FBig)divisor;
    public static FBig operator %(long divisor, FBig one) => (FBig)divisor % one;

    public static FBig operator <<(FBig one, int amount) => Raw(one.RawValue << amount);
    public static FBig operator >>(FBig one, int amount) => Raw(one.RawValue >> amount);


    // Comparison Operators
    public static bool operator ==(FBig one, FBig other) => one.RawValue == other.RawValue;
    public static bool operator ==(FBig one, long other) => one == (FBig)other;
    public static bool operator ==(long other, FBig one) => (FBig)other == one;

    public static bool operator !=(FBig one, FBig other) => one.RawValue != other.RawValue;
    public static bool operator !=(FBig one, long other) => one != (FBig)other;
    public static bool operator !=(long other, FBig one) => (FBig)other != one;

    public static bool operator >=( FBig one, FBig other ) => one.RawValue >= other.RawValue;
    public static bool operator >=(FBig one, long other) => one >= (FBig)other;
    public static bool operator >=(long other, FBig one) => (FBig)other >= one;
    
    public static bool operator <=(FBig one, FBig other) => one.RawValue <= other.RawValue;
    public static bool operator <=(FBig one, long other) => one <= (FBig)other;
    public static bool operator <=(long other, FBig one) => (FBig)other <= one;

    public static bool operator >(FBig one, FBig other) => one.RawValue > other.RawValue;
    public static bool operator >(FBig one, long other) => one > (FBig)other;
    public static bool operator >(long other, FBig one) => (FBig)other > one;

    public static bool operator <(FBig one, FBig other) => one.RawValue < other.RawValue;
    public static bool operator <(FBig one, long other) => one < (FBig)other;
    public static bool operator <(long other, FBig one) => (FBig)other < one;


    public static explicit operator long(FBig src) => (long)(src.RawValue >> ScaleFactor);
    public static explicit operator FBig(long src) => new FBig(src, true);

    public static explicit operator double(FBig src) => (double)src / (double)One;
    public static explicit operator FBig(double src) => new FBig(src);

    public static explicit operator FBig(Int128 src) => new FBig(src, true);
    public static explicit operator FBig(UInt128 src) => new FBig((Int128)src, true);


    public override bool Equals(object obj)
    {
        if (obj is FBig fint)
            return fint.RawValue == RawValue;
        else
            return false;
    }


    public override readonly int GetHashCode() => RawValue.GetHashCode();
    public override readonly string ToString() 
    {
        return ((decimal)RawValue / (decimal)One).ToString();
    }
}