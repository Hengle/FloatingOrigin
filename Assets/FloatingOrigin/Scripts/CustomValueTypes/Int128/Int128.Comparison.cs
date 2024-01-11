// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

using System;
using System.Runtime.CompilerServices;

namespace BigIntegers
{
    public partial struct Int128 : IComparable, IComparable<Int128>, IEquatable<Int128>
    {
        // Less

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <(Int128 a, UInt128 b) => a.CompareTo(b) < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <(UInt128 a, Int128 b) => b.CompareTo(a) > 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <(Int128 a, Int128 b) => LessThan(a.v, b.v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <(Int128 a, long b) => LessThan(a.v, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <(long a, Int128 b) => LessThan(a, b.v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <(Int128 a, ulong b) => LessThan(a.v, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <(ulong a, Int128 b) => LessThan(a, b.v);

        // Less Equals

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <=(Int128 a, UInt128 b) => a.CompareTo(b) <= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <=(UInt128 a, Int128 b) => b.CompareTo(a) >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <=(Int128 a, Int128 b) => !LessThan(b.v, a.v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <=(Int128 a, long b) => !LessThan(b, a.v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <=(long a, Int128 b) => !LessThan(b.v, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <=(Int128 a, ulong b) => !LessThan(b, a.v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <=(ulong a, Int128 b) => !LessThan(b.v, a);

        // Greater

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >(Int128 a, UInt128 b) => a.CompareTo(b) > 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >(UInt128 a, Int128 b) => b.CompareTo(a) < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >(Int128 a, Int128 b) => LessThan(b.v, a.v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >(Int128 a, long b) => LessThan(b, a.v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >(long a, Int128 b) => LessThan(b.v, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >(Int128 a, ulong b) => LessThan(b, a.v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >(ulong a, Int128 b) => LessThan(b.v, a);

        // Greater Equals

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >=(Int128 a, UInt128 b) => a.CompareTo(b) >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >=(UInt128 a, Int128 b) => b.CompareTo(a) <= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >=(Int128 a, Int128 b) => !LessThan(a.v, b.v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >=(Int128 a, long b) => !LessThan(a.v, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >=(long a, Int128 b) => !LessThan(a, b.v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >=(Int128 a, ulong b) => !LessThan(a.v, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >=(ulong a, Int128 b) => !LessThan(a, b.v);

        // Equals

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator ==(UInt128 a, Int128 b) => b.Equals(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator ==(Int128 a, UInt128 b) => a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator ==(Int128 a, Int128 b) => a.v.Equals(b.v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator ==(Int128 a, long b) => a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator ==(long a, Int128 b) => b.Equals(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator ==(Int128 a, ulong b) => a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator ==(ulong a, Int128 b) => b.Equals(a);

        // Not Equals

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator !=(UInt128 a, Int128 b) => !b.Equals(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator !=(Int128 a, UInt128 b) => !a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator !=(Int128 a, Int128 b) => !a.v.Equals(b.v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator !=(Int128 a, long b) => !a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator !=(long a, Int128 b) => !b.Equals(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator !=(Int128 a, ulong b) => !a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator !=(ulong a, Int128 b) => !b.Equals(a);


        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public readonly int CompareTo(UInt128 other) => v._upper > long.MaxValue ? -1 : v.CompareTo(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public readonly int CompareTo(Int128 other) => SignedCompare(v, other.v._lower, other.v._upper);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public readonly int CompareTo(int other) => SignedCompare(v, (ulong)other, (ulong)(other >> 31));

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public readonly int CompareTo(uint other) => SignedCompare(v, other, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public readonly int CompareTo(long other) => SignedCompare(v, (ulong)other, (ulong)(other >> 63));

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public readonly int CompareTo(ulong other) => SignedCompare(v, other, 0);

        public readonly int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (obj is not Int128)
                throw new ArgumentException();

            return CompareTo((Int128)obj);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private static bool LessThan(UInt128 a, UInt128 b) => a._upper != b._upper ? (long)a._upper < (long)b._upper : a._lower < b._lower;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private static bool LessThan(UInt128 a, long b) => (long)a._upper != (b >> 63) ? (long)a._upper < (b >> 63) : a._lower < (ulong)b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private static bool LessThan(long a, UInt128 b) => (a >> 63) != (long)b._upper ? (a >> 63) < (long)b._upper : (ulong)a < b._lower;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private static bool LessThan(UInt128 a, ulong b) => ((long)a._upper != 0) ? ((long)a._upper < 0) : (a._lower < b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private static bool LessThan(ulong a, UInt128 b) => (0 != (long)b._upper) ? (0 < (long)b._upper) : (a < b._lower);


        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private static int SignedCompare(UInt128 a, ulong bs0, ulong bs1) => (a._upper != bs1) ? ((long)a._upper).CompareTo((long)bs1) : a._lower.CompareTo(bs0);


        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public readonly bool Equals(UInt128 other) => !(v._upper > long.MaxValue) && v.Equals(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public readonly bool Equals(ulong other) => v._upper == 0 && v._lower == other;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public readonly bool Equals(Int128 other) => v.Equals(other.v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public readonly bool Equals(long other) => other < 0 ? v._upper == ulong.MaxValue && v._lower == (ulong)other : v._upper == 0 && v._lower == (ulong)other;


        public readonly override bool Equals(object obj)
        {
            if (obj is not Int128)
                return false;

            return v.Equals(((Int128)obj).v);
        }
    }
}