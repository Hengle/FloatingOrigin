// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

using System;
using System.Runtime.CompilerServices;

namespace BigIntegers
{
    public partial struct Int128
    {
        // Multiplication
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 Multiply(Int128 a, long b) => new Int128 { v = a.v._upper > long.MaxValue ? (b < 0 ? ((-a.v) * (ulong)-b) : -((-a.v) * (ulong)b)) : (b < 0 ? -(a.v * (ulong)-b) : a.v * (ulong)b) };
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 Multiply(Int128 a, ulong b) => new Int128 { v = a.v._upper > long.MaxValue ? -(-a.v * b) : (a.v * b) };
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 Multiply(Int128 a, Int128 b) => new Int128 { v = a.v._upper > long.MaxValue ? (b.v._upper > long.MaxValue ? (-a.v * -b.v) : -(-a.v * b.v)) : (b.v._upper > long.MaxValue ? -(a.v * -b.v) : (a.v * b.v)) };
    
        public static Int128 Pow(Int128 value, int exponent)
        {
            Int128 c;
    
            if (exponent < 0)
                throw new ArgumentException("exponent cannot be negative");
    
            if (value.v._upper > long.MaxValue)
            {
                if ((exponent & 1) == 0)
                    c.v = UInt128.Pow(-value.v, (uint)exponent);
                else
                    c.v = -UInt128.Pow(-value.v, (uint)exponent);
            }
            else
                c.v = UInt128.Pow(value.v, (uint)exponent);
            
            return c;
        }
    
    
        // Multiplication Operators
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator *(Int128 a, long b) => Multiply(a, b);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator *(Int128 a, ulong b) => Multiply(a, b);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator *(long a, Int128 b) => Multiply(b, a);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator *(ulong a, Int128 b) => Multiply(b, a);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Int128 operator *(Int128 a, Int128 b) => Multiply(a, b);
    }
}