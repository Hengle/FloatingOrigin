// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

namespace BigIntegers
{
    /// <summary>
    /// 128-bit unsigned integer. 
    /// </summary>
    /// <remarks>
    /// Maximum size is 170141183460469231731687303715884105727 or around 170 undecillion.<br/>
    /// Minimum size is -170141183460469231731687303715884105727 or around -170 undecillion.<br/>
    /// </remarks> 
    public partial struct Int128
    {
        public UInt128 v;

        public static readonly Int128 MinValue = (Int128)((UInt128)1 << 127);
        public static readonly Int128 MaxValue = (Int128)(((UInt128)1 << 127) - 1);
        public static readonly Int128 Zero = (Int128)0;
        public static readonly Int128 One = (Int128)1;
        public static readonly Int128 MinusOne = (Int128)(-1);

        public readonly bool IsZero => v.IsZero;
        public readonly bool IsOne => v.IsOne; 
        public readonly bool IsPowerOfTwo => v.IsPowerOfTwo; 
        public readonly bool IsEven => v.IsEven; 
        public readonly bool IsNegative => v._upper > long.MaxValue; 
        public readonly int Sign => v._upper > long.MaxValue ? -1 : v.Sign; 


        public readonly override int GetHashCode() => v.GetHashCode();

        // String Formatting -> moved to Int128.Formatting.cs

        // Comparison Operators -> moved to Int128.Comparison.cs

        // Constructors -> moved to Int128.Conversion.cs

        // Addition -> moved to Int128.Addition.cs

        // Subtraction -> moved to Int128.Subtraction.cs

        // Multiplication -> moved to Int128.Multiplication.cs

        // Division -> moved to Int128.Division.cs

        // Bitwise -> moved to Int128.BitOperations.cs
    }
}