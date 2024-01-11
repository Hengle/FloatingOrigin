// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

using System;
using System.Runtime.CompilerServices;


namespace BigIntegers
{
    public partial struct UInt128 : IComparable, IComparable<UInt128>, IEquatable<UInt128>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public readonly int CompareTo(UInt128 other) => _upper != other._upper ? _upper.CompareTo(other._upper) : _lower.CompareTo(other._lower); 

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public readonly int CompareTo(long other) => _upper != 0 || other < 0 ? 1 : _lower.CompareTo((ulong)other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public readonly int CompareTo(ulong other) => _upper != 0 ? 1 : _lower.CompareTo(other);

        public readonly int CompareTo(object obj) => obj is UInt128 u ? CompareTo(u) : throw new InvalidCastException("invalid type"); 


        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private static bool LessThan(UInt128 a, long b) => b >= 0 && a._upper == 0 && a._lower < (ulong)b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private static bool LessThan(long a, UInt128 b) => a < 0 || b._upper != 0 || (ulong)a < b._lower;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private static bool LessThan(UInt128 a, UInt128 b) => a._upper != b._upper ? a._upper < b._upper : a._lower < b._lower;


        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public readonly bool Equals(UInt128 other) => _lower == other._lower && _upper == other._upper;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public readonly bool Equals(long other) => other >= 0 && _lower == (ulong)other && _upper == 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public readonly bool Equals(ulong other) => _lower == other && _upper == 0; 

        public override readonly bool Equals(object obj) => obj is UInt128 _uint128 && Equals(_uint128); 


        // Less

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <(UInt128 a, UInt128 b) => LessThan(a, b); 

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <(UInt128 a, long b) => LessThan(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <(long a, UInt128 b) => LessThan(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <(UInt128 a, ulong b) => LessThan(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <(ulong a, UInt128 b) => LessThan(a, b);

        // Less Equals

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <=(UInt128 a, UInt128 b) => !LessThan(b, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <=(UInt128 a, long b) => !LessThan(b, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <=(long a, UInt128 b) => !LessThan(b, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <=(UInt128 a, ulong b) => !LessThan(b, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator <=(ulong a, UInt128 b) => !LessThan(b, a);

        // Greater

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >(UInt128 a, UInt128 b) => LessThan(b, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >(UInt128 a, long b) => LessThan(b, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >(long a, UInt128 b) => LessThan(b, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >(UInt128 a, ulong b) => LessThan(b, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >(ulong a, UInt128 b) => LessThan(b, a);

        // Greater Equals

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >=(UInt128 a, UInt128 b) => !LessThan(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >=(UInt128 a, long b) => !LessThan(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >=(long a, UInt128 b) => !LessThan(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >=(UInt128 a, ulong b) => !LessThan(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator >=(ulong a, UInt128 b) => !LessThan(a, b);

        // Equals

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator ==(UInt128 a, UInt128 b) => a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator ==(UInt128 a, long b) => a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator ==(long a, UInt128 b) => b.Equals(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator ==(UInt128 a, ulong b) => a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator ==(ulong a, UInt128 b) => b.Equals(a);

        // Not Equals

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator !=(UInt128 a, UInt128 b) => !a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator !=(UInt128 a, long b) => !a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator !=(long a, UInt128 b) => !b.Equals(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator !=(UInt128 a, ulong b) => !a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator !=(ulong a, UInt128 b) => !b.Equals(a);
    }
}