// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

using System.Runtime.CompilerServices;

namespace BigIntegers
{
    public partial struct UInt128
    {
        private static UInt128 Add(UInt128 a, ulong b)
        {
            UInt128 c = new UInt128(a._upper, a._lower + b);
            if (c._lower < a._lower && c._lower < b)
                ++c._upper;

            return c;
        }

        private static UInt128 Add(UInt128 a, UInt128 b)
        {
            UInt128 c = new UInt128(a._upper + b._upper, a._lower + b._lower);
            if (c._lower < a._lower && c._lower < b._lower)
                ++c._upper;

            return c;
        }

        // Addition Operators- Inlined to use direct function call
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static UInt128 operator +(UInt128 a, UInt128 b) => Add(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static UInt128 operator +(UInt128 a, ulong b) => Add(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static UInt128 operator +(ulong a, UInt128 b) => Add(b, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static UInt128 operator +(UInt128 a) => a;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static UInt128 operator ++(UInt128 a) => Add(a, 1);
    }
}