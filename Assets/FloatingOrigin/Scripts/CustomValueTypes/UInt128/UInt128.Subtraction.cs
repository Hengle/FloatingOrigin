

namespace BigIntegers
{

public partial struct UInt128
{
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