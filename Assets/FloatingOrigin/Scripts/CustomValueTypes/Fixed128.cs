// Fixed-Point Math using only a Int128 numeric type
// Original code from here: https://stackoverflow.com/questions/605124/fixed-point-math-in-c-sharp

using System;
using UnityEngine;


namespace BigIntegers
{
    public struct Fixed128
    {
        public Int128 raw;

        const int ScaleFactor = 16; // 16-bit precision for the decimal part, or around one ushort's worth.

        public static readonly Int128 One = 1 << ScaleFactor;


        public Fixed128(Int128 StartingRawValue, bool UseMultiple) => raw = UseMultiple ? StartingRawValue << ScaleFactor : StartingRawValue;

        public Fixed128(double doubleValue)
        {
            raw = (Int128)Math.Round(doubleValue * (double)One);
        }

        public Fixed128(Int128 rawValue) => raw = rawValue << ScaleFactor;

        public static Fixed128 Abs(Fixed128 F) => F < 0 ? -F : F;

        public static Fixed128 FromParts(long preDecimal, ushort postDecimal)
        {
            Fixed128 f;
            f.raw = preDecimal << ScaleFactor;

            if (postDecimal != 0)
                f.raw.v._lower |= postDecimal;

            return f;
        }


        // Arithmetic Operators
        public static Fixed128 operator +(Fixed128 a, Fixed128 b) => new Fixed128 { raw = a.raw + b.raw };
        public static Fixed128 operator +(Fixed128 a, long b) => new Fixed128 { raw = a.raw + b };
        public static Fixed128 operator +(long a, Fixed128 b) => new Fixed128 { raw = a + b.raw };

        public static Fixed128 operator -(Fixed128 a) => new Fixed128 { raw = -a.raw };
        public static Fixed128 operator -(Fixed128 a, Fixed128 b) => new Fixed128 { raw = a.raw - b.raw };
        public static Fixed128 operator -(Fixed128 a, long b) => new Fixed128 { raw = a.raw - (b << ScaleFactor) };

        public static Fixed128 operator *(Fixed128 a, Fixed128 b) => new Fixed128 { raw = (a.raw * b.raw) >> ScaleFactor };
        public static Fixed128 operator *(Fixed128 a, long b) => new Fixed128 { raw = (a.raw * (b << ScaleFactor)) >> ScaleFactor };
        public static Fixed128 operator *(long a, Fixed128 b) => new Fixed128 { raw = ((a << ScaleFactor) * b.raw) >> ScaleFactor };

        public static Fixed128 operator /(Fixed128 a, Fixed128 b) => new Fixed128 { raw = (a.raw << ScaleFactor) / b.raw };
        public static Fixed128 operator /(Fixed128 a, long b) => new Fixed128 { raw = (a.raw << ScaleFactor) / (b << ScaleFactor) };
        public static Fixed128 operator /(long a, Fixed128 b) => new Fixed128 { raw = ((a << ScaleFactor) << ScaleFactor) / b.raw };

        public static Fixed128 operator %(Fixed128 a, Fixed128 b) => new Fixed128 { raw = a.raw % b.raw };
        public static Fixed128 operator %(Fixed128 a, long b) => new Fixed128 { raw = a.raw % (b << ScaleFactor) };
        public static Fixed128 operator %(long a, Fixed128 b) => new Fixed128 { raw = (Int128)a % b.raw };

        public static Fixed128 operator <<(Fixed128 a, int amount) => new Fixed128 { raw = a.raw << amount };
        public static Fixed128 operator >>(Fixed128 a, int amount) => new Fixed128 { raw = a.raw >> amount };


        // Comparison Operators
        public static bool operator ==(Fixed128 a, Fixed128 b) => a.raw == b.raw;
        public static bool operator ==(Fixed128 a, long b) => a == (b << ScaleFactor);
        public static bool operator ==(long b, Fixed128 a) => (b << ScaleFactor) == a;

        public static bool operator !=(Fixed128 a, Fixed128 b) => a.raw != b.raw;
        public static bool operator !=(Fixed128 a, long b) => a != (b << ScaleFactor);
        public static bool operator !=(long b, Fixed128 a) => (b << ScaleFactor) != a;

        public static bool operator >=( Fixed128 a, Fixed128 b ) => a.raw >= b.raw;
        public static bool operator >=(Fixed128 a, long b) => a >= (b << ScaleFactor);
        public static bool operator >=(long b, Fixed128 a) => (b << ScaleFactor) >= a;

        public static bool operator <=(Fixed128 a, Fixed128 b) => a.raw <= b.raw;
        public static bool operator <=(Fixed128 a, long b) => a <= (b << ScaleFactor);
        public static bool operator <=(long b, Fixed128 a) => (b << ScaleFactor) <= a;

        public static bool operator >(Fixed128 a, Fixed128 b) => a.raw > b.raw;
        public static bool operator >(Fixed128 a, long b) => a > (b << ScaleFactor);
        public static bool operator >(long b, Fixed128 a) => (b << ScaleFactor) > a;

        public static bool operator <(Fixed128 a, Fixed128 b) => a.raw < b.raw;
        public static bool operator <(Fixed128 a, long b) => a < (b << ScaleFactor);
        public static bool operator <(long b, Fixed128 a) => (b << ScaleFactor) < a;


        public static explicit operator long(Fixed128 src) => (long)(src.raw >> ScaleFactor);
        public static explicit operator double(Fixed128 src) => (double)src.raw / (double)One;

        public static explicit operator Fixed128(long src) => new Fixed128(src, true);
        public static explicit operator Fixed128(double src) => new Fixed128(src);
        public static explicit operator Fixed128(Int128 src) => new Fixed128(src, true);
        public static explicit operator Fixed128(UInt128 src) => new Fixed128((Int128)src, true);


        public override bool Equals(object obj)
        {
            if (obj is Fixed128 fint)
                return fint.raw == raw;
            else
                return false;
        }


        public override readonly int GetHashCode() => raw.GetHashCode();
        public override string ToString()
        {
            Int128 wholePart = raw / One;

            Debug.Log(wholePart);

            decimal fractionalPart = (decimal)(raw % One) / (decimal)One;

            Debug.Log(fractionalPart);

            return $"{wholePart}{fractionalPart}";
        }
    }
}