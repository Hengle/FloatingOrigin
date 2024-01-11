// Original from https://github.com/ricksladkey/dirichlet-numerics
// Modified and reorganized

using System;
using System.Globalization;
using System.Numerics;

namespace BigIntegers
{
    public partial struct Int128 : IFormattable
    {
        // Parsing
        public static Int128 Parse(string value)
        {
            if (!TryParse(value, out Int128 c))
                throw new FormatException();
            return c;
        }

        public static bool TryParse(string value, out Int128 result) 
        {
            return TryParse(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
        }

        public static bool TryParse(string value, NumberStyles style, IFormatProvider format, out Int128 result)
        {
            BigInteger a;
            if (!BigInteger.TryParse(value, style, format, out a))
            {
                result = Zero;
                return false;
            }
            result.v = new UInt128(a);
            return true;
        }

        // Formatting 
        public readonly override string ToString() => ((BigInteger)this).ToString();
        public readonly string ToString(string format) => ((BigInteger)this).ToString(format);
        public readonly string ToString(IFormatProvider provider) => ToString(null, provider);
        public readonly string ToString(string format, IFormatProvider provider) => ((BigInteger)this).ToString(format, provider);

        private static readonly string[] abbreviations =  { "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc", "UnDc" };

        public readonly string Abbreviation(bool withDigits, string format)
        {
            Int128 abs = Abs(this);
            
            if (abs < 1000)
                return ToString(format);

            for (int i = 0; i < abbreviations.Length; i++)
            {
                var lo = Pow(1000, i + 1);
                var hi = Pow(1000, i + 2);

                if (abs >= lo && abs < hi)
                    return (withDigits ? ((decimal)this / (decimal)lo).ToString(format) : string.Empty) + abbreviations[i];
            }

            return ToString(format);
        }
    }
}