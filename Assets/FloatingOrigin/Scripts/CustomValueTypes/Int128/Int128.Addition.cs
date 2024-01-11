// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

using System.Runtime.CompilerServices;

namespace BigIntegers
{
    public partial struct Int128
    {
        // Inline all of these since it's faster than calling functions and copying around values.
        // This hopefully means that an addition compiles like so: (a + b) -> Int128.Add(a, b) -> new Int128 { v = a + b } -> new Int128 { v = UInt128.Add(a, b) }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private static Int128 Add(Int128 a, ulong b) => new Int128 { v = a.v + b };

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private static Int128 Add(Int128 a, long b) => new Int128 { v = b < 0 ? a.v - (ulong)-b : a.v + (ulong)b };

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private static Int128 Add(Int128 a, Int128 b) => new Int128 { v = a.v + b.v };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int128 Abs(Int128 value) => value.v._upper > long.MaxValue ? new Int128 { v = -value.v } : new Int128 { v = value.v };


        // Addition Operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator +(Int128 a, long b) => Add(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator +(Int128 a, ulong b) => Add(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator +(long a, Int128 b) => Add(b, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator +(ulong a, Int128 b) => Add(b, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator +(Int128 a, Int128 b) => Add(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator +(Int128 a) => a;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator ++(Int128 a) => Add(a, 1);
    }
}