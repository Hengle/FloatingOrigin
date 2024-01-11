// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

using System;
using System.Runtime.CompilerServices;

namespace BigIntegers
{
    public partial struct Int128
    {
        // Division
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 Divide(Int128 a, long b) => new Int128 { v = a.v._upper > long.MaxValue ? (b < 0 ? (-a.v / (ulong)-b) : -(-a.v / (ulong)b)) : (b < 0 ? -(a.v / (ulong)-b) : (a.v / (ulong)b)) };
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 Divide(Int128 a, ulong b) => new Int128 { v = a.v._upper > long.MaxValue ? -(-a.v / b) : (a.v / b) };
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 Divide(Int128 a, Int128 b) => new Int128 { v = a.v._upper > long.MaxValue ? (b.v._upper > long.MaxValue ? (-a.v / -b.v) : -(-a.v / b.v)) : (b.v._upper > long.MaxValue ? -(a.v / -b.v) : (a.v / b.v)) };
    
    
        // Division Operators
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator /(Int128 a, long b) => Divide(a, b);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator /(Int128 a, ulong b) => Divide(a, b);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator /(Int128 a, Int128 b) => Divide(a, b);
    
    
        // Modulus
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Remainder(Int128 a, long b) => a.v._upper > long.MaxValue ? -(long)((-a.v) % (ulong)Math.Abs(b)) : (long)(a.v % (ulong)Math.Abs(b));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static long Remainder(Int128 a, ulong b) => a.v._upper > long.MaxValue ? -(long)(-a.v % b) : (long)(a.v % b);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 Remainder(Int128 a, Int128 b) => new Int128 { v = a.v._upper > long.MaxValue ? -((-a.v) % Abs(b).v) : (a.v % Abs(b).v) };
    
    
        // Modulus Operators
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static long operator %(Int128 a, long b) => Remainder(a, b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static long operator %(Int128 a, ulong b) => Remainder(a, b);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator %(Int128 a, Int128 b) => Remainder(a, b);
    }
}