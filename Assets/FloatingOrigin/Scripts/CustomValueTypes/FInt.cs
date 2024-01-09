

// Fixed-Point Math using only a long numeric type
using System;

public struct FInt
{
    public long RawValue;
    const int ScaleFactor = 12; // 12 is 4096

    const long One = 1 << ScaleFactor;

    
    public static FInt OneF = new FInt(1, true);
    public static FInt PI = Raw(12868); 


    public FInt(long StartingRawValue, bool UseMultiple) => RawValue = UseMultiple ? StartingRawValue << ScaleFactor : StartingRawValue;

    public FInt(double DoubleValue) => RawValue = (int)Math.Round(DoubleValue * One);

    public static FInt Raw(long rawValue) => new FInt { RawValue = rawValue };


    public readonly int IntValue => (int)(RawValue >> ScaleFactor);
    public readonly double DoubleValue => RawValue / (double)One;


    public static FInt Abs(FInt F) => F < 0 ? -F : F;

    public static FInt FromParts(int preDecimal, int postDecimal)
    {
        FInt f = new(preDecimal, true);

        if (postDecimal != 0)
            f.RawValue += (new FInt(postDecimal) / 1000).RawValue;

        return f;
    }



    // Arithmetic Operators
    public static FInt operator +(FInt one, FInt other) => Raw(one.RawValue + other.RawValue);
    public static FInt operator +(FInt one, int other) => one + (FInt)other;
    public static FInt operator +(int other, FInt one) => one + (FInt)other;

    public static FInt operator -(FInt value) => Raw(-value.RawValue);
    public static FInt operator -(FInt minuend, FInt subtrahend) => Raw(minuend.RawValue - subtrahend.RawValue);
    public static FInt operator -(FInt one, int other) => one - (FInt)other;
    public static FInt operator -(int other, FInt one) => (FInt)other - one;

    public static FInt operator *(FInt multiplicand, FInt multiplier) => Raw((multiplicand.RawValue * multiplier.RawValue) >> ScaleFactor);
    public static FInt operator *(FInt multiplicand, int multiplier) => multiplicand * (FInt)multiplier;
    public static FInt operator *(int multiplicand, FInt multiplier) => (FInt)multiplicand * multiplier;

    public static FInt operator /(FInt one, FInt other) => Raw((one.RawValue << ScaleFactor) / other.RawValue);
    public static FInt operator /(FInt one, int divisor) => one / (FInt)divisor;
    public static FInt operator /(int divisor, FInt one) => (FInt)divisor / one;

    public static FInt operator %(FInt one, FInt other) => Raw(one.RawValue % other.RawValue);
    public static FInt operator %(FInt one, int divisor) => one % (FInt)divisor;
    public static FInt operator %(int divisor, FInt one) => (FInt)divisor % one;

    public static FInt operator <<(FInt one, int amount) => Raw(one.RawValue << amount);
    public static FInt operator >>(FInt one, int amount) => Raw(one.RawValue >> amount);


    // Comparison Operators
    public static bool operator ==(FInt one, FInt other) => one.RawValue == other.RawValue;
    public static bool operator ==(FInt one, int other) => one == (FInt)other;
    public static bool operator ==(int other, FInt one) => (FInt)other == one;

    public static bool operator !=(FInt one, FInt other) => one.RawValue != other.RawValue;
    public static bool operator !=(FInt one, int other) => one != (FInt)other;
    public static bool operator !=(int other, FInt one) => (FInt)other != one;

    public static bool operator >=( FInt one, FInt other ) => one.RawValue >= other.RawValue;
    public static bool operator >=(FInt one, int other) => one >= (FInt)other;
    public static bool operator >=(int other, FInt one) => (FInt)other >= one;
    
    public static bool operator <=(FInt one, FInt other) => one.RawValue <= other.RawValue;
    public static bool operator <=(FInt one, int other) => one <= (FInt)other;
    public static bool operator <=(int other, FInt one) => (FInt)other <= one;

    public static bool operator >(FInt one, FInt other) => one.RawValue > other.RawValue;
    public static bool operator >(FInt one, int other) => one > (FInt)other;
    public static bool operator >(int other, FInt one) => (FInt)other > one;

    public static bool operator <(FInt one, FInt other) => one.RawValue < other.RawValue;
    public static bool operator <(FInt one, int other) => one < (FInt)other;
    public static bool operator <(int other, FInt one) => (FInt)other < one;


    public static explicit operator int(FInt src) => (int)(src.RawValue >> ScaleFactor);
    public static explicit operator FInt(int src) => new FInt(src, true);
    public static explicit operator FInt(long src) => new FInt(src, true);
    public static explicit operator FInt(ulong src) => new FInt((long)src, true);


    public override bool Equals(object obj)
    {
        if (obj is FInt fint)
            return fint.RawValue == RawValue;
        else
            return false;
    }


    public override readonly int GetHashCode() => RawValue.GetHashCode();
    public override readonly string ToString() => RawValue.ToString();
}