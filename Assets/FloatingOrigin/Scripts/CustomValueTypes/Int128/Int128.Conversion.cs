// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

using System.Numerics;
using System.Runtime.CompilerServices;

namespace BigIntegers
{
    public partial struct Int128
    {
        // Constructors
        public Int128(long value) => v = new UInt128(value);
        public Int128(ulong value) => v = new UInt128(value);
        public Int128(ulong hi, ulong lo) => v = new UInt128(hi, lo);
        public Int128(double value) => v = new UInt128(value);
        public Int128(decimal value) => v = new UInt128(value);
        public Int128(BigInteger value) => v = new UInt128(value);
        public Int128(UInt128 value) => v = value;

        // Conversions

        // Unsigned to Int128

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static implicit operator Int128(byte a) => new Int128((ulong)a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static implicit operator Int128(ushort a) => new Int128((ulong)a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static implicit operator Int128(uint a) => new Int128((ulong)a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static implicit operator Int128(ulong a) => new Int128(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator Int128(UInt128 a) => new Int128 { v = a };

        // Signed to Int128

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static implicit operator Int128(sbyte a) => new Int128(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static implicit operator Int128(short a) => new Int128(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static implicit operator Int128(int a) => new Int128(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static implicit operator Int128(long a) => new Int128(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator Int128(BigInteger a) => new Int128(a);

        // Floating-Point to Int128

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator Int128(double a) => new Int128(a); 

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator Int128(decimal a) => new Int128(a);

        // Int128 to Unsigned

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator byte(Int128 a) => (byte)a.v._lower;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator ushort(Int128 a) => (ushort)a.v._lower;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator uint(Int128 a) => (uint)a.v._lower;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator ulong(Int128 a) => a.v._lower;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator UInt128(Int128 a) => a.v;

        // Int128 to Signed

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator sbyte(Int128 a) => (sbyte)a.v._lower;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator short(Int128 a) => (short)a.v._lower;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator int(Int128 a) => (int)a.v._lower;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator long(Int128 a) => (long)a.v._lower;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator BigInteger(Int128 a) => a.v._upper > long.MaxValue ? -(BigInteger)(-a.v) : (BigInteger)a.v;

        // Int128 to Floating-Point

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator float(Int128 a) => a.v._upper > long.MaxValue ? -(float)-a.v : (float)a.v;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator double(Int128 a) => a.v._upper > long.MaxValue ? -(double)-a.v : (double)a.v;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator decimal(Int128 a) => a.v._upper > long.MaxValue ? -(decimal)-a.v : (decimal)a.v;
    }
}