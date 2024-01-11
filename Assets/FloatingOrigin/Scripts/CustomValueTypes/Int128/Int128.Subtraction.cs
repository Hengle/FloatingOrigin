// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

using System;
using System.Runtime.CompilerServices;

namespace BigIntegers
{
    public partial struct Int128
    {
        // Subtraction

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private static Int128 Subtract(Int128 a, ulong b) => new Int128 { v = a.v - b };    

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 Subtract(Int128 a, long b) => new Int128 { v = b < 0 ? a.v + (ulong)-b : a.v - (ulong)b }; 

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private static Int128 Subtract(Int128 a, Int128 b) => new Int128 { v = a.v - b.v }; 


        // Subtraction Operators    

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator -(Int128 a, long b) => Subtract(a, b);    

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator -(Int128 a, ulong b) => Subtract(a, b);   

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator -(Int128 a, Int128 b) => Subtract(a, b);  

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator -(Int128 a) => new Int128 { v = -a.v };   

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator --(Int128 a) => Subtract(a, 1);
    }
}