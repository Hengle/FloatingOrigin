// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

using System.Runtime.CompilerServices;

namespace BigIntegers
{
    public partial struct UInt128
    {
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

            return new UInt128((carry >> 32) + r2 + u1 * v1, carry << 32 | r0);
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

        private static UInt128 Square64(ulong u)
        {
            var u0 = (ulong)(uint)u;
            var u1 = u >> 32;
            var carry = u0 * u0;
            var r0 = (uint)carry;
            var u0u1 = u0 * u1;
            carry = (carry >> 32) + u0u1;
            var r2 = carry >> 32;
            carry = (uint)carry + u0u1;

            return new UInt128((carry >> 32) + r2 + u1 * u1, carry << 32 | r0);
        }

        public static UInt128 Square(UInt128 a)
        {
            if (a._upper == 0)
                return Square64(a._lower);

            return Multiply128(a, a);
        }

        public static UInt128 Pow(UInt128 value, uint exponent)
        {
            UInt128 result = One;
            while (exponent != 0)
            {
                if ((exponent & 1) != 0)
                {
                    var previous = result;
                    result = Multiply(previous, value);
                }
                if (exponent != 1)
                {
                    var previous = value;
                    value = Square(previous);
                }
                exponent >>= 1;
            }

            return result;
        }

        // Multiplication Operators- Inlined to use direct function call

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static UInt128 operator *(UInt128 a, ulong b) => Multiply(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static UInt128 operator *(ulong a, UInt128 b) => Multiply(b, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static UInt128 operator *(UInt128 a, UInt128 b) => Multiply(a, b);
    }
}