using System.Linq;

namespace BigIntegers
{

public partial struct UInt128
{
    private static readonly byte[] bitLength = Enumerable.Range(0, 256).Select(value =>
        {
            int count = 0;
            for (; value != 0; count++)
                value >>= 1;
            return (byte)count;
        }).ToArray();


    private static int GetBitLength(uint value)
    {
        uint tt = value >> 16;
        uint t;

        if (tt != 0)
        {
            t = tt >> 8;
            if (t != 0)
                return bitLength[t] + 24;
            return bitLength[tt] + 16;
        }

        t = value >> 8;
        if (t != 0)
            return bitLength[t] + 8;
        return bitLength[value];
        
    }

    private static int GetBitLength(ulong value)
    {
        ulong r1 = value >> 32;
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
        
        if (b == 64)
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
        
        if (b == 64)
            return new UInt128(a._upper, 0);

        return new UInt128(a._upper >> (b - 64), 0);
    }

    private static UInt128 And(UInt128 a, UInt128 b) => new UInt128(a._lower & b._lower, a._upper & b._upper);
    private static UInt128 Or(UInt128 a, UInt128 b) => new UInt128(a._lower | b._lower, a._upper | b._upper);
    private static UInt128 ExclusiveOr(UInt128 a, UInt128 b) => new UInt128(a._lower ^ b._lower, a._upper ^ b._upper);
    private static UInt128 Not(UInt128 a) => new UInt128(~a._lower, ~a._upper);
}

}