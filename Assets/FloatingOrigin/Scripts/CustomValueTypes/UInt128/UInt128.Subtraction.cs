// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

using System.Runtime.CompilerServices;

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
            var s0 = value._lower;
            value._lower = 0 - s0;
            value._upper = 0 - value._upper;

            if (s0 > 0)
                --value._upper;

            return value;
        }

        // Subtraction Operators- Inlined to use direct function call

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static UInt128 operator -(UInt128 a, UInt128 b) => Subtract(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static UInt128 operator -(UInt128 a, ulong b) => Subtract(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static UInt128 operator -(ulong a, UInt128 b) => Subtract(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static UInt128 operator -(UInt128 a) => Negate(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static UInt128 operator --(UInt128 a) => Subtract(a, 1);
    }
}