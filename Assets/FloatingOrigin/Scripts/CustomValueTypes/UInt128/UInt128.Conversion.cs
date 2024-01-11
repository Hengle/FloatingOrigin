// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace BigIntegers
{
    public partial struct UInt128
    {
        public UInt128(uint r0, uint r1, uint r2, uint r3)
        {
            _lower = (ulong)r1 << 32 | r0;
            _upper = (ulong)r3 << 32 | r2;
        }

        public UInt128(ulong upper, ulong lower)
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
            return new decimal((int)(uint)a._lower, (int)(uint)(a._lower >> 32), (int)(uint)a._upper, false, (byte)shift);
        }


        // Conversion operators- Inlined to use direct function call

        // Unsigned to UInt128
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator UInt128(byte a) => new UInt128(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator UInt128(ushort a) => new UInt128(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator UInt128(uint a) => new UInt128(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator UInt128(ulong a) => new UInt128(a);

        // Signed to UInt128
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator UInt128(sbyte a) => new UInt128(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator UInt128(short a) => new UInt128(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator UInt128(int a) => new UInt128(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator UInt128(long a) => new UInt128(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator UInt128(BigInteger a) => new UInt128(a);

        // Floating-Point to UInt128
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator UInt128(double a) => new UInt128(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator UInt128(decimal a) => new UInt128(a);

        // UInt128 to Unsigned
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator sbyte(UInt128 a) => (sbyte)a._lower;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator ushort(UInt128 a) => (ushort)a._lower;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator uint(UInt128 a) => (uint)a._lower;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator ulong(UInt128 a) => a._lower;

        // UInt128 to Signed
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator byte(UInt128 a) => (byte)a._lower;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator short(UInt128 a) => (short)a._lower;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator int(UInt128 a) => (int)a._lower;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator long(UInt128 a) => (long)a._lower;

        // No precision loss, allow implicit
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator BigInteger(UInt128 a) => ToBigInt(a);

        // UInt128 to Floating-Point
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float(UInt128 a) => ToFloat(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator double(UInt128 a) => ToDouble(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator decimal(UInt128 a) => ToDecimal(a);
    }
}