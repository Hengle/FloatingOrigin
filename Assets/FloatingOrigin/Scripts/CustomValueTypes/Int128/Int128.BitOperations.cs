// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

using System.Runtime.CompilerServices;

namespace BigIntegers
{
    public partial struct Int128
    {
        // Bitwise Operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator <<(Int128 a, int b) => new Int128 { v = a.v << b };

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator >>(Int128 a, int b) => new Int128 { v = a.v >> b };

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator &(Int128 a, Int128 b) => new Int128 { v = a.v & b.v };

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator |(Int128 a, Int128 b) => new Int128 { v = a.v | b.v };

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator ^(Int128 a, Int128 b) => new Int128 { v = a.v ^ b.v };

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator ~(Int128 a) => new Int128 { v = ~a.v };
    }
}