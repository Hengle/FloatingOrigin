// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

using System;

namespace BigIntegers
{
    /// <summary>
    /// 128-bit unsigned integer. 
    /// </summary>
    /// <remarks>
    /// Maximum size is 340282366920938463463374607431768211455, or around 340 undecillion.
    /// </remarks> 
    public partial struct UInt128
    {
        public ulong _lower;
        public ulong _upper;

        public readonly bool IsZero => (_lower | _upper) == 0;
        public readonly bool IsOne => _upper == 0 && _lower == 1;
        public readonly bool IsPowerOfTwo => (this & (this - 1)).IsZero;
        public readonly bool IsEven => (_lower & 1) == 0;
        public readonly int Sign => IsZero ? 0 : 1;

        public static readonly UInt128 MaxValue = ~(UInt128)0;
        public static readonly UInt128 MinValue = (UInt128)0;
        public static readonly UInt128 Zero = (UInt128)0;
        public static readonly UInt128 One = (UInt128)1;
    
        public override readonly int GetHashCode() => HashCode.Combine(_lower.GetHashCode(), _upper.GetHashCode());


        // String Formatting -> moved to UInt128.Formatting.cs

        // Comparison Operators -> moved to UInt128.Comparison.cs

        // Constructors -> moved to UInt128.Conversion.cs

        // Addition -> moved to UInt128.Addition.cs

        // Subtraction -> moved to UInt128.Subtraction.cs

        // Multiplication -> moved to UInt128.Multiplication.cs

        // Division -> moved to UInt128.Division.cs

        // Bitwise -> moved to UInt128.BitOperations.cs
    }
}