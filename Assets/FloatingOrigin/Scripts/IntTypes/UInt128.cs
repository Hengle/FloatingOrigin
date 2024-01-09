// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
/*
using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct UInt128
    {
        internal const int Size = 16;

#if BIGENDIAN
        private readonly ulong _upper;
        private readonly ulong _lower;
#else
        private readonly ulong _lower;
        private readonly ulong _upper;
#endif

        public UInt128(ulong upper, ulong lower)
        {
            _lower = lower;
            _upper = upper;
        }

        internal ulong Lower => _lower;

        internal ulong Upper => _upper;

        public int CompareTo(object value)
        {
            if (value is UInt128 other)
            {
                return CompareTo(other);
            }
            else if (value is null)
            {
                return 1;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public int CompareTo(UInt128 value)
        {
            if (this < value)
            {
                return -1;
            }
            else if (this > value)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is UInt128 other) && Equals(other);
        }

        public bool Equals(UInt128 other)
        {
            return this == other;
        }

        public override int GetHashCode() => HashCode.Combine(_lower, _upper);

        public static explicit operator byte(UInt128 value) => (byte)value._lower;
        public static explicit operator char(UInt128 value) => (char)value._lower;
        public static explicit operator decimal(UInt128 value)
        {
            ulong lo64 = value._lower;

            if (value._upper > uint.MaxValue)
            {
                // The default behavior of decimal conversions is to always throw on overflow
                throw new OverflowException();
            }

            uint hi32 = (uint)(value._upper);

            return new decimal((int)(lo64), (int)(lo64 >> 32), (int)(hi32), isNegative: false, scale: 0);
        }

        public static explicit operator double(UInt128 value)
        {
            // This code is based on `u128_to_f64_round` from m-ou-se/floatconv
            // Copyright (c) 2020 Mara Bos <m-ou.se@m-ou.se>. All rights reserved.
            //
            // Licensed under the BSD 2 - Clause "Simplified" License
            // See THIRD-PARTY-NOTICES.TXT for the full license text

            const double TwoPow52 = 4503599627370496.0;
            const double TwoPow76 = 75557863725914323419136.0;
            const double TwoPow104 = 20282409603651670423947251286016.0;
            const double TwoPow128 = 340282366920938463463374607431768211456.0;

            const ulong TwoPow52Bits = 0x4330000000000000;
            const ulong TwoPow76Bits = 0x44B0000000000000;
            const ulong TwoPow104Bits = 0x4670000000000000;
            const ulong TwoPow128Bits = 0x47F0000000000000;

            if (value._upper == 0)
            {
                // For values between 0 and ulong.MaxValue, we just use the existing conversion
                return (double)(value._lower);
            }
            else if ((value._upper >> 24) == 0) // value < (2^104)
            {
                // For values greater than ulong.MaxValue but less than 2^104 this takes advantage
                // that we can represent both "halves" of the uint128 within the 52-bit mantissa of
                // a pair of doubles.

                double lower = UInt64BitsToDouble(TwoPow52Bits | ((value._lower << 12) >> 12)) - TwoPow52;
                double upper = UInt64BitsToDouble(TwoPow104Bits | (ulong)(value >> 52)) - TwoPow104;

                return lower + upper;
            }
            else
            {
                // For values greater than 2^104 we basically do the same as before but we need to account
                // for the precision loss that double will have. As such, the lower value effectively drops the
                // lowest 24 bits and then or's them back to ensure rounding stays correct.

                double lower = UInt64BitsToDouble(TwoPow76Bits | ((ulong)(value >> 12) >> 12) | (value._lower & 0xFFFFFF)) - TwoPow76;
                double upper = UInt64BitsToDouble(TwoPow128Bits | (ulong)(value >> 76)) - TwoPow128;

                return lower + upper;
            }
        }


        static double UInt64BitsToDouble(ulong value)
        {
            return BitConverter.ToDouble(BitConverter.GetBytes(value));
        }


        static ulong DoubleBitsToUInt64(double value)
        {
            return BitConverter.ToUInt64(BitConverter.GetBytes(value));
        }



        public static explicit operator short(UInt128 value) => (short)value._lower;
        public static explicit operator int(UInt128 value) => (int)value._lower;
        public static explicit operator long(UInt128 value) => (long)value._lower;
        public static explicit operator Int128(UInt128 value) => new Int128(value._upper, value._lower);
        public static explicit operator nint(UInt128 value) => (nint)value._lower;
        public static explicit operator sbyte(UInt128 value) => (sbyte)value._lower;
        public static explicit operator float(UInt128 value) => (float)(double)(value);
        public static explicit operator ushort(UInt128 value) => (ushort)value._lower;
        public static explicit operator uint(UInt128 value) => (uint)value._lower;
        public static explicit operator ulong(UInt128 value) => value._lower;
        public static explicit operator nuint(UInt128 value) => (nuint)value._lower;
        public static explicit operator UInt128(decimal value)
        {
            value = decimal.Truncate(value);

            if (value < 0.0m)
            {
                throw new OverflowException();
            }

            var bits = decimal.GetBits(value);

            ulong lower = (ulong)(uint)bits[1] << 32 | (uint)bits[0];
            ulong upper = (ulong)0 << 32 | (uint)bits[2];

            return new UInt128(upper, lower);
        }

        /// <summary>Explicitly converts a <see cref="double" /> value to a 128-bit unsigned integer.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns><paramref name="value" /> converted to a 128-bit unsigned integer.</returns>
        public static explicit operator UInt128(double value)
        {
            const double TwoPow128 = 340282366920938463463374607431768211456.0;

            if (double.IsNegative(value) || double.IsNaN(value))
            {
                return MinValue;
            }
            else if (value >= TwoPow128)
            {
                return MaxValue;
            }

            return ToUInt128(value);
        }


        internal static UInt128 ToUInt128(double value)
        {
            const double TwoPow128 = 340282366920938463463374607431768211456.0;

            Debug.Assert(value >= 0);
            Debug.Assert(double.IsFinite(value));
            Debug.Assert(value < TwoPow128);

            // This code is based on `f64_to_u128` from m-ou-se/floatconv
            // Copyright (c) 2020 Mara Bos <m-ou.se@m-ou.se>. All rights reserved.
            //
            // Licensed under the BSD 2 - Clause "Simplified" License
            // See THIRD-PARTY-NOTICES.TXT for the full license text

            if (value >= 1.0)
            {
                // In order to convert from double to uint128 we first need to extract the signficand,
                // including the implicit leading bit, as a full 128-bit significand. We can then adjust
                // this down to the represented integer by right shifting by the unbiased exponent, taking
                // into account the significand is now represented as 128-bits.

                ulong bits = DoubleBitsToUInt64(value);
                UInt128 result = new UInt128((bits << 12) >> 1 | 0x8000_0000_0000_0000, 0x0000_0000_0000_0000);

                result >>= (1023 + 128 - 1 - (int)(bits >> 52));
                return result;
            }
            else
            {
                return MinValue;
            }
        }

        public static explicit operator UInt128(short value)
        {
            long lower = value;
            return new UInt128((ulong)(lower >> 63), (ulong)lower);
        }

        public static explicit operator UInt128(int value)
        {
            long lower = value;
            return new UInt128((ulong)(lower >> 63), (ulong)lower);
        }

        public static explicit operator UInt128(long value)
        {
            long lower = value;
            return new UInt128((ulong)(lower >> 63), (ulong)lower);
        }

        public static explicit operator UInt128(nint value)
        {
            long lower = value;
            return new UInt128((ulong)(lower >> 63), (ulong)lower);
        }

        public static explicit operator UInt128(sbyte value)
        {
            long lower = value;
            return new UInt128((ulong)(lower >> 63), (ulong)lower);
        }

        public static explicit operator UInt128(float value) => (UInt128)(double)(value);


        public static implicit operator UInt128(byte value) => new UInt128(0, value);
        public static implicit operator UInt128(char value) => new UInt128(0, value);
        public static implicit operator UInt128(ushort value) => new UInt128(0, value);
        public static implicit operator UInt128(uint value) => new UInt128(0, value);
        public static implicit operator UInt128(ulong value) => new UInt128(0, value);
        public static implicit operator UInt128(nuint value) => new UInt128(0, value);

        public static UInt128 operator +(UInt128 left, UInt128 right)
        {
            // For unsigned addition, we can detect overflow by checking `(x + y) < x`
            // This gives us the carry to add to upper to compute the correct result

            ulong lower = left._lower + right._lower;
            ulong carry = (lower < left._lower) ? 1UL : 0UL;

            ulong upper = left._upper + right._upper + carry;
            return new UInt128(upper, lower);
        }

        public static (UInt128 Quotient, UInt128 Remainder) DivRem(UInt128 left, UInt128 right)
        {
            UInt128 quotient = left / right;
            return (quotient, left - (quotient * right));
        }

        public static UInt128 LeadingZeroCount(UInt128 value)
            => (uint)LeadingZeroCountAsInt32(value);

        /// <summary>Computes the number of leading zero bits in this value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LeadingZeroCountAsInt32(UInt128 value)
        {
            if (value._upper == 0)
            {
                return 64 + BitOperations.LeadingZeroCount(value._lower);
            }
            return BitOperations.LeadingZeroCount(value._upper);
        }


        public static UInt128 operator &(UInt128 left, UInt128 right) => new UInt128(left._upper & right._upper, left._lower & right._lower);

        public static UInt128 operator |(UInt128 left, UInt128 right) => new UInt128(left._upper | right._upper, left._lower | right._lower);

        public static UInt128 operator ^(UInt128 left, UInt128 right) => new UInt128(left._upper ^ right._upper, left._lower ^ right._lower);

        public static UInt128 operator ~(UInt128 value) => new UInt128(~value._upper, ~value._lower);

        public static bool operator <(UInt128 left, UInt128 right)
        {
            return (left._upper < right._upper) || (left._upper == right._upper) && (left._lower < right._lower);
        }

        public static bool operator <=(UInt128 left, UInt128 right)
        {
            return (left._upper < right._upper) || (left._upper == right._upper) && (left._lower <= right._lower);
        }

        public static bool operator >(UInt128 left, UInt128 right)
        {
            return (left._upper > right._upper) || (left._upper == right._upper) && (left._lower > right._lower);
        }

        public static bool operator >=(UInt128 left, UInt128 right)
        {
            return (left._upper > right._upper) || (left._upper == right._upper) && (left._lower >= right._lower);
        }

        public static UInt128 operator --(UInt128 value) => value - One;

        public static UInt128 operator /(UInt128 left, UInt128 right)
        {
            if (right._upper == 0)
            {
                if (right._lower == 0)
                {
                    throw new DivideByZeroException();
                }

                if (left._upper == 0)
                {
                    // left and right are both uint64
                    return left._lower / right._lower;
                }
            }

            if (right >= left)
            {
                return (right == left) ? One : Zero;
            }

            return DivideSlow(left, right);

            static uint AddDivisor(Span<uint> left, ReadOnlySpan<uint> right)
            {
                Debug.Assert(left.Length >= right.Length);

                // Repairs the dividend, if the last subtract was too much

                ulong carry = 0UL;

                for (int i = 0; i < right.Length; i++)
                {
                    ref uint leftElement = ref left[i];
                    ulong digit = (leftElement + carry) + right[i];

                    leftElement = unchecked((uint)digit);
                    carry = digit >> 32;
                }

                return (uint)carry;
            }

            static bool DivideGuessTooBig(ulong q, ulong valHi, uint valLo, uint divHi, uint divLo)
            {
                Debug.Assert(q <= 0xFFFFFFFF);

                // We multiply the two most significant limbs of the divisor
                // with the current guess for the quotient. If those are bigger
                // than the three most significant limbs of the current dividend
                // we return true, which means the current guess is still too big.

                ulong chkHi = divHi * q;
                ulong chkLo = divLo * q;

                chkHi += (chkLo >> 32);
                chkLo = (uint)(chkLo);

                return (chkHi > valHi) || ((chkHi == valHi) && (chkLo > valLo));
            }

            unsafe static UInt128 DivideSlow(UInt128 quotient, UInt128 divisor)
            {
                // This is the same algorithm currently used by BigInteger so
                // we need to get a Span<uint> containing the value represented
                // in the least number of elements possible.

                // We need to ensure that we end up with 4x uints representing the bits from
                // least significant to most significant so the math will be correct on both
                // little and big endian systems. So we'll just allocate the relevant buffer
                // space and then write out the four parts using the native endianness of the
                // system.

                uint* pLeft = stackalloc uint[Size / sizeof(uint)];

                Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 0), (uint)(quotient._lower >> 00));
                Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 1), (uint)(quotient._lower >> 32));

                Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 2), (uint)(quotient._upper >> 00));
                Unsafe.WriteUnaligned(ref *(byte*)(pLeft + 3), (uint)(quotient._upper >> 32));

                Span<uint> left = new Span<uint>(pLeft, (Size / sizeof(uint)) - (LeadingZeroCountAsInt32(quotient) / 32));

                // Repeat the same operation with the divisor

                uint* pRight = stackalloc uint[Size / sizeof(uint)];

                Unsafe.WriteUnaligned(ref *(byte*)(pRight + 0), (uint)(divisor._lower >> 00));
                Unsafe.WriteUnaligned(ref *(byte*)(pRight + 1), (uint)(divisor._lower >> 32));

                Unsafe.WriteUnaligned(ref *(byte*)(pRight + 2), (uint)(divisor._upper >> 00));
                Unsafe.WriteUnaligned(ref *(byte*)(pRight + 3), (uint)(divisor._upper >> 32));

                Span<uint> right = new Span<uint>(pRight, (Size / sizeof(uint)) - (LeadingZeroCountAsInt32(divisor) / 32));

                Span<uint> rawBits = stackalloc uint[Size / sizeof(uint)];
                rawBits.Clear();
                Span<uint> bits = rawBits.Slice(0, left.Length - right.Length + 1);

                Debug.Assert(left.Length >= 1);
                Debug.Assert(right.Length >= 1);
                Debug.Assert(left.Length >= right.Length);

                // Executes the "grammar-school" algorithm for computing q = a / b.
                // Before calculating q_i, we get more bits into the highest bit
                // block of the divisor. Thus, guessing digits of the quotient
                // will be more precise. Additionally we'll get r = a % b.

                uint divHi = right[right.Length - 1];
                uint divLo = right.Length > 1 ? right[right.Length - 2] : 0;

                // We measure the leading zeros of the divisor
                int shift = BitOperations.LeadingZeroCount(divHi);
                int backShift = 32 - shift;

                // And, we make sure the most significant bit is set
                if (shift > 0)
                {
                    uint divNx = right.Length > 2 ? right[right.Length - 3] : 0;

                    divHi = (divHi << shift) | (divLo >> backShift);
                    divLo = (divLo << shift) | (divNx >> backShift);
                }

                // Then, we divide all of the bits as we would do it using
                // pen and paper: guessing the next digit, subtracting, ...
                for (int i = left.Length; i >= right.Length; i--)
                {
                    int n = i - right.Length;
                    uint t = ((uint)(i) < (uint)(left.Length)) ? left[i] : 0;

                    ulong valHi = ((ulong)(t) << 32) | left[i - 1];
                    uint valLo = (i > 1) ? left[i - 2] : 0;

                    // We shifted the divisor, we shift the dividend too
                    if (shift > 0)
                    {
                        uint valNx = i > 2 ? left[i - 3] : 0;

                        valHi = (valHi << shift) | (valLo >> backShift);
                        valLo = (valLo << shift) | (valNx >> backShift);
                    }

                    // First guess for the current digit of the quotient,
                    // which naturally must have only 32 bits...
                    ulong digit = valHi / divHi;

                    if (digit > 0xFFFFFFFF)
                    {
                        digit = 0xFFFFFFFF;
                    }

                    // Our first guess may be a little bit to big
                    while (DivideGuessTooBig(digit, valHi, valLo, divHi, divLo))
                    {
                        --digit;
                    }

                    if (digit > 0)
                    {
                        // Now it's time to subtract our current quotient
                        uint carry = SubtractDivisor(left.Slice(n), right, digit);

                        if (carry != t)
                        {
                            Debug.Assert(carry == (t + 1));

                            // Our guess was still exactly one too high
                            carry = AddDivisor(left.Slice(n), right);

                            --digit;
                            Debug.Assert(carry == 1);
                        }
                    }

                    // We have the digit!
                    if ((uint)(n) < (uint)(bits.Length))
                    {
                        bits[n] = (uint)(digit);
                    }

                    if ((uint)(i) < (uint)(left.Length))
                    {
                        left[i] = 0;
                    }
                }

                return new UInt128(
                    ((ulong)(rawBits[3]) << 32) | rawBits[2],
                    ((ulong)(rawBits[1]) << 32) | rawBits[0]
                );
            }

            static uint SubtractDivisor(Span<uint> left, ReadOnlySpan<uint> right, ulong q)
            {
                Debug.Assert(left.Length >= right.Length);
                Debug.Assert(q <= 0xFFFFFFFF);

                // Combines a subtract and a multiply operation, which is naturally
                // more efficient than multiplying and then subtracting...

                ulong carry = 0UL;

                for (int i = 0; i < right.Length; i++)
                {
                    carry += right[i] * q;

                    uint digit = (uint)(carry);
                    carry >>= 32;

                    ref uint leftElement = ref left[i];

                    if (leftElement < digit)
                    {
                        ++carry;
                    }
                    leftElement -= digit;
                }

                return (uint)(carry);
            }
        }

        public static bool operator ==(UInt128 left, UInt128 right) => (left._lower == right._lower) && (left._upper == right._upper);

        public static bool operator !=(UInt128 left, UInt128 right) => (left._lower != right._lower) || (left._upper != right._upper);


        public static UInt128 operator ++(UInt128 value) => value + One;

        public static UInt128 MinValue => new UInt128(0, 0);

        public static UInt128 MaxValue => new UInt128(0xFFFF_FFFF_FFFF_FFFF, 0xFFFF_FFFF_FFFF_FFFF);

        public static UInt128 operator %(UInt128 left, UInt128 right)
        {
            UInt128 quotient = left / right;
            return left - (quotient * right);
        }

        public static ulong BigMul(ulong a, ulong b, out ulong low)
        {
            // Adaptation of algorithm for multiplication
            // of 32-bit unsigned integers described
            // in Hacker's Delight by Henry S. Warren, Jr. (ISBN 0-201-91465-4), Chapter 8
            // Basically, it's an optimized version of FOIL method applied to
            // low and high dwords of each operand

            // Use 32-bit uints to optimize the fallback for 32-bit platforms.
            uint al = (uint)a;
            uint ah = (uint)(a >> 32);
            uint bl = (uint)b;
            uint bh = (uint)(b >> 32);

            ulong mull = ((ulong)al) * bl;
            ulong t = ((ulong)ah) * bl + (mull >> 32);
            ulong tl = ((ulong)al) * bh + (uint)t;

            low = tl << 32 | (uint)mull;

            return ((ulong)ah) * bh + (t >> 32) + (tl >> 32);
        }


        public static UInt128 operator *(UInt128 left, UInt128 right)
        {
            ulong upper = BigMul(left._lower, right._lower, out ulong lower);
            upper += (left._upper * right._lower) + (left._lower * right._upper);
            return new UInt128(upper, lower);
        }

        internal static UInt128 BigMul(UInt128 left, UInt128 right, out UInt128 lower)
        {
            // Adaptation of algorithm for multiplication
            // of 32-bit unsigned integers described
            // in Hacker's Delight by Henry S. Warren, Jr. (ISBN 0-201-91465-4), Chapter 8
            // Basically, it's an optimized version of FOIL method applied to
            // low and high qwords of each operand

            UInt128 al = left._lower;
            UInt128 ah = left._upper;

            UInt128 bl = right._lower;
            UInt128 bh = right._upper;

            UInt128 mull = al * bl;
            UInt128 t = ah * bl + mull._upper;
            UInt128 tl = al * bh + t._lower;

            lower = new UInt128(tl._lower, mull._lower);
            return ah * bh + t._upper + tl._upper;
        }

        public static UInt128 Clamp(UInt128 value, UInt128 min, UInt128 max)
        {
            if (min > max)
            {
                throw new ArgumentException("Min cannot be greater than Max");
            }

            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }

            return value;
        }

        public static UInt128 Max(UInt128 x, UInt128 y) => (x >= y) ? x : y;
        public static UInt128 Min(UInt128 x, UInt128 y) => (x <= y) ? x : y;
        public static int Sign(UInt128 value) => (value == 0U) ? 0 : 1;


        public static UInt128 One => new UInt128(0, 1);
        public static UInt128 Zero => default;


        public static UInt128 operator <<(UInt128 value, int shiftAmount)
        {
            // C# automatically masks the shift amount for UInt64 to be 0x3F. So we
            // need to specially handle things if the 7th bit is set.

            shiftAmount &= 0x7F;

            if ((shiftAmount & 0x40) != 0)
            {
                // In the case it is set, we know the entire lower bits must be zero
                // and so the upper bits are just the lower shifted by the remaining
                // masked amount

                ulong upper = value._lower << shiftAmount;
                return new UInt128(upper, 0);
            }
            else if (shiftAmount != 0)
            {
                // Otherwise we need to shift both upper and lower halves by the masked
                // amount and then or that with whatever bits were shifted "out" of lower

                ulong lower = value._lower << shiftAmount;
                ulong upper = (value._upper << shiftAmount) | (value._lower >> (64 - shiftAmount));

                return new UInt128(upper, lower);
            }
            else
            {
                return value;
            }
        }

        public static UInt128 operator >>(UInt128 value, int shiftAmount) => URightShift(value, shiftAmount);

        public static UInt128 URightShift(UInt128 value, int shiftAmount)
        {
            // C# automatically masks the shift amount for UInt64 to be 0x3F. So we
            // need to specially handle things if the 7th bit is set.

            shiftAmount &= 0x7F;

            if ((shiftAmount & 0x40) != 0)
            {
                // In the case it is set, we know the entire upper bits must be zero
                // and so the lower bits are just the upper shifted by the remaining
                // masked amount

                ulong lower = value._upper >> shiftAmount;
                return new UInt128(0, lower);
            }
            else if (shiftAmount != 0)
            {
                // Otherwise we need to shift both upper and lower halves by the masked
                // amount and then or that with whatever bits were shifted "out" of upper

                ulong lower = (value._lower >> shiftAmount) | (value._upper << (64 - shiftAmount));
                ulong upper = value._upper >> shiftAmount;

                return new UInt128(upper, lower);
            }
            else
            {
                return value;
            }
        }

        public static UInt128 operator -(UInt128 left, UInt128 right)
        {
            // For unsigned subtract, we can detect overflow by checking `(x - y) > x`
            // This gives us the borrow to subtract from upper to compute the correct result

            ulong lower = left._lower - right._lower;
            ulong borrow = (lower > left._lower) ? 1UL : 0UL;

            ulong upper = left._upper - right._upper - borrow;
            return new UInt128(upper, lower);
        }

        public static UInt128 operator -(UInt128 value) => Zero - value;
        public static UInt128 operator +(UInt128 value) => value;
    }
}*/