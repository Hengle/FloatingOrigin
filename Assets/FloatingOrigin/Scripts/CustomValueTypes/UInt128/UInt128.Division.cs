// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

using System.Runtime.CompilerServices;


namespace BigIntegers
{
    public partial struct UInt128
    {
        // Divide UInt128-UInt128
        // |↳ Divide UInt128-ulong
        // |↳ DivRem96 UInt128-UInt128
        //      ↳ GetBitLength
        //      ↳ LeftShift64
        //      ↳ LeftShift64
        //      ↳ DivRem medium
        //          ↳ Qhat
        //      ↳ DivRem medium
        //          ↳ Qhat
        //      ↳ RightShift64
        // |↳ DivRem128 UInt128-UInt128 
        //      ↳ GetBitLength
        //      ↳ LeftShift64
        //      ↳ LeftShift64
        //      ↳ DivRem big
        //          ↳ Qhat
        //      ↳ RightShift64

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


        // Divide UInt128-ulong
        // |↳ Divide96 uint
        // |↳ Divide128 uint
        // |↳ Divide96 ulong
        //      ↳ GetBitLength
        //      ↳ DivRem small
        //          ↳ Qhat
        //      ↳ DivRem small
        //          ↳ Qhat
        // |↳ Divide128 ulong
        //      ↳ GetBitLength
        //      ↳ DivRem small
        //          ↳ Qhat
        //      ↳ DivRem small
        //          ↳ Qhat
        //      ↳ DivRem small
        //          ↳ Qhat

        private static UInt128 Divide(UInt128 u, ulong v)
        {
            if (u._upper == 0)
            {
                u._lower /= v;
                u._upper = 0;
            }
            else
            {
                uint v0 = (uint)v;
                if (v == v0)
                {
                    if (u._upper <= uint.MaxValue)
                        Divide96(ref u, v0);
                    else
                        Divide128(ref u, v0);
                }
                else
                {
                    if (u._upper <= uint.MaxValue)
                        Divide96(ref u, v);
                    else
                        Divide128(ref u, v);
                }
            }

            return u;
        }

        private static void Divide96(ref UInt128 u, uint v)
        {
            uint r2 = (uint)u._upper;
            uint w2 = r2 / v;
            ulong u0 = r2 - w2 * v;
            ulong u0u1 = u0 << 32 | (uint)(u._lower >> 32);
            uint w1 = (uint)(u0u1 / v);
            u0 = u0u1 - w1 * v;
            u0u1 = u0 << 32 | (uint)u._lower;
            uint w0 = (uint)(u0u1 / v);

            u._lower = w2;
            u._upper = (ulong)w1 << 32 | w0;
        }

        private static void Divide128(ref UInt128 u, uint v)
        {
            uint r3 = (uint)(u._upper >> 32);
            uint w3 = r3 / v;
            ulong u0 = r3 - w3 * v;
            ulong u0u1 = u0 << 32 | (uint)u._upper;
            uint w2 = (uint)(u0u1 / v);
            u0 = u0u1 - w2 * v;
            u0u1 = u0 << 32 | (uint)(u._lower >> 32);
            uint w1 = (uint)(u0u1 / v);
            u0 = u0u1 - w1 * v;
            u0u1 = u0 << 32 | (uint)u._lower;
            uint w0 = (uint)(u0u1 / v);

            u._lower = (ulong)w3 << 32 | w2;
            u._upper = (ulong)w1 << 32 | w0;
        }

        private static void Divide96(ref UInt128 u, ulong v)
        {
            int dneg = GetBitLength((uint)(v >> 32));
            int d = 32 - dneg;
            ulong vPrime = v << d;
            uint v1 = (uint)(vPrime >> 32);
            uint v2 = (uint)vPrime;
            uint r0 = (uint)u._lower;
            uint r1 = (uint)(u._lower >> 32);
            uint r2 = (uint)u._upper;
            uint r3 = 0;

            if (d != 0)
            {
                r3 = r2 >> dneg;
                r2 = r2 << d | r1 >> dneg;
                r1 = r1 << d | r0 >> dneg;
                r0 <<= d;
            }

            uint q1 = DivRem(r3, ref r2, ref r1, v1, v2);
            uint q0 = DivRem(r2, ref r1, ref r0, v1, v2);

            u._lower = (ulong)q1 << 32 | q0;
            u._upper = 0;
        }

        private static void Divide128(ref UInt128 u, ulong v)
        {
            int dneg = GetBitLength((uint)(v >> 32));
            int d = 32 - dneg;
            ulong vPrime = v << d;
            uint v1 = (uint)(vPrime >> 32);
            uint v2 = (uint)vPrime;
            uint r0 = (uint)u._lower;
            uint r1 = (uint)(u._lower >> 32);
            uint r2 = (uint)u._upper;
            uint r3 = (uint)(u._upper >> 32);
            uint r4 = 0;

            if (d != 0)
            {
                r4 = r3 >> dneg;
                r3 = r3 << d | r2 >> dneg;
                r2 = r2 << d | r1 >> dneg;
                r1 = r1 << d | r0 >> dneg;
                r0 <<= d;
            }

            u._upper = DivRem(r4, ref r3, ref r2, v1, v2);
            uint q1 = DivRem(r3, ref r2, ref r1, v1, v2);
            uint q0 = DivRem(r2, ref r1, ref r0, v1, v2);
            u._lower = (ulong)q1 << 32 | q0;
        }

        private static ulong Qhat(uint u0, uint u1, uint u2, uint v1, uint v2)
        {
            ulong u0u1 = (ulong)u0 << 32 | u1;
            ulong qhat = u0 == v1 ? uint.MaxValue : u0u1 / v1;
            ulong r = u0u1 - qhat * v1;

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
            ulong qhat = Qhat(u0, u1, u2, v1, v2);
            ulong carry = qhat * v2;
            long borrow = (long)u2 - (uint)carry;
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
            ulong qhat = Qhat(u0, u1, u2, v1, v2);
            ulong carry = qhat * v3;
            long borrow = (long)u3 - (uint)carry;
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
            ulong qhat = Qhat(u0, u1, u2, v1, v2);
            ulong carry = qhat * v4;
            long borrow = (long)u4 - (uint)carry;
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

            uint v0 = (uint)v;
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

            UInt128 rem;

            if (b._upper <= uint.MaxValue)
                DivRem96(out rem, a, b);
            else
                DivRem128(out rem, a, b);

            return rem;
        }

        private static uint Remainder96(UInt128 u, uint v)
        {
            ulong u0 = (uint)u._upper % v;
            ulong u0u1 = u0 << 32 | (uint)(u._lower >> 32);
            u0 = u0u1 % v;
            u0u1 = u0 << 32 | (uint)u._lower;
            return (uint)(u0u1 % v);
        }

        private static uint Remainder128(UInt128 u, uint v)
        {
            ulong u0 = (uint)(u._upper >> 32) % v;
            ulong u0u1 = u0 << 32 | (uint)u._upper;
            u0 = u0u1 % v;
            u0u1 = u0 << 32 | (uint)(u._lower >> 32);
            u0 = u0u1 % v;
            u0u1 = u0 << 32 | (uint)u._lower;
            return (uint)(u0u1 % v);
        }

        private static ulong Remainder96(UInt128 u, ulong v)
        {
            int dneg = GetBitLength((uint)(v >> 32));
            int d = 32 - dneg;
            ulong vPrime = v << d;
            uint v1 = (uint)(vPrime >> 32);
            uint v2 = (uint)vPrime;
            uint r0 = (uint)u._lower;
            uint r1 = (uint)(u._lower >> 32);
            uint r2 = (uint)u._upper;
            uint r3 = 0;

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
            int dneg = GetBitLength((uint)(v >> 32));
            int d = 32 - dneg;
            ulong vPrime = v << d;
            uint v1 = (uint)(vPrime >> 32);
            uint v2 = (uint)vPrime;
            uint r0 = (uint)u._lower;
            uint r1 = (uint)(u._lower >> 32);
            uint r2 = (uint)u._upper;
            uint r3 = (uint)(u._upper >> 32);
            uint r4 = 0;

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


        // Unintuitively, DivRem96 takes longer to execute than DivRem128.
        private static ulong DivRem96(out UInt128 rem, UInt128 a, UInt128 b)
        {
            int d = 32 - GetBitLength((uint)b._upper);
            LeftShift64(out UInt128 v, b, d);
            uint r4 = (uint)LeftShift64(out rem, a, d);
            uint v1 = (uint)v._upper;
            uint v2 = (uint)(v._lower >> 32);
            uint v3 = (uint)v._lower;
            uint r3 = (uint)(rem._upper >> 32);
            uint r2 = (uint)rem._upper;
            uint r1 = (uint)(rem._lower >> 32);
            uint r0 = (uint)rem._lower;
            uint q1 = DivRem(r4, ref r3, ref r2, ref r1, v1, v2, v3);
            uint q0 = DivRem(r3, ref r2, ref r1, ref r0, v1, v2, v3);
            rem = new UInt128(r0, r1, r2, 0);
            var div = (ulong)q1 << 32 | q0;
            rem = RightShift64(rem, d);
            return div;
        }

        private static uint DivRem128(out UInt128 rem, UInt128 a, UInt128 b)
        {
            var d = 32 - GetBitLength((uint)(b._upper >> 32));
            LeftShift64(out UInt128 v, b, d);
            uint r4 = (uint)LeftShift64(out rem, a, d);
            uint r3 = (uint)(rem._upper >> 32);
            uint r2 = (uint)rem._upper;
            uint r1 = (uint)(rem._lower >> 32);
            uint r0 = (uint)rem._lower;
            uint div = DivRem(r4, ref r3, ref r2, ref r1, ref r0, (uint)(v._upper >> 32), (uint)v._upper, (uint)(v._lower >> 32), (uint)v._lower);
            rem = new UInt128(r0, r1, r2, r3);
            rem = RightShift64(rem, d);
            return div;
        }

        // Division Operators- Inlined to use direct function call

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static UInt128 operator /(UInt128 a, ulong b) => Divide(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static UInt128 operator /(UInt128 a, UInt128 b) => Divide(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static ulong operator %(UInt128 a, ulong b) => Remainder(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static UInt128 operator %(UInt128 a, UInt128 b) => Remainder(a, b);
    }
}