

namespace BigIntegers
{

public partial struct UInt128
{
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
}

}