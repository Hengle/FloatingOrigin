#include "absl/numeric/int128.h"

#include <random>


typedef unsigned tu_int __attribute__((mode(TI)));
typedef int ti_int __attribute__((mode(TI)));
typedef uint64_t du_int;
typedef uint32_t su_int;

typedef union {
  tu_int all;
  struct {
    du_int low;
    du_int high;
  } s;
} utwords;


inline du_int PrimitiveDiv(du_int u1, du_int u0, du_int v, du_int* r) 
{
    // Code taken from Hacker's Delight:
    // http://www.hackersdelight.org/HDcode/divlu.c.

    const uint64_t b = (1ULL << 32);  // Number base (32 bits)
    uint64_t un1, un0;                // Norm. dividend LSD's
    uint64_t vn1, vn0;                // Norm. divisor digits
    uint64_t q1, q0;                  // Quotient digits
    uint64_t un64, un21, un10;        // Dividend digit pairs
    uint64_t rhat;                    // A remainder
    int32_t s;                        // Shift amount for norm

    // If overflow, set rem. to an impossible value,
    // and return the largest possible quotient
    if (u1 >= v)
    {
        *r = (uint64_t)-1;
        return (uint64_t)-1;
    }

    // count leading zeros
    s = __builtin_clzll(v);
    if (s > 0) 
    {
        // Normalize divisor
        v = v << s;
        un64 = (u1 << s) | (u0 >> (64 - s));
        un10 = u0 << s;  // Shift dividend left
    } 
    else 
    {
        // Avoid undefined behavior of (u0 >> 64).
        // The behavior is undefined if the right operand is
        // negative, or greater than or equal to the length
        // in bits of the promoted left operand.
        un64 = u1;
        un10 = u0;
    }

    // Break divisor up into two 32-bit digits
    vn1 = v >> 32;
    vn0 = v & 0xFFFFFFFF;

    // Break right half of dividend into two digits
    un1 = un10 >> 32;
    un0 = un10 & 0xFFFFFFFF;

    // Compute the first quotient digit, q1
    q1 = un64 / vn1;
    rhat = un64 - q1 * vn1;

    while (q1 >= b || q1 * vn0 > b * rhat + un1) 
    {
        q1 = q1 - 1;
        rhat = rhat + vn1;
        if (rhat >= b) break;
    }

    // Multiply and subtract
    un21 = un64 * b + un1 - q1 * v;

    // Compute the second quotient digit
    q0 = un21 / vn1;
    rhat = un21 - q0 * vn1;

    while (q0 >= b || q0 * vn0 > b * rhat + un0) 
    {
        q0 = q0 - 1;
        rhat = rhat + vn1;
        if (rhat >= b) break;
    }

    *r = (un21 * b + un0 - q0 * v) >> s;
    return q1 * b + q0;
}


tu_int MyDivMod1(tu_int a, tu_int b, tu_int* rem) 
{
    const unsigned n_utword_bits = sizeof(tu_int) * CHAR_BIT;
    (void)n_utword_bits;
    utwords dividend;
    dividend.all = a;
    utwords divisor;
    divisor.all = b;
    utwords quotient;
    utwords remainder;

    if (divisor.s.high == 0) {
        remainder.s.high = 0;
        
        if (dividend.s.high < divisor.s.low)
        {
            quotient.s.low = PrimitiveDiv(dividend.s.high, dividend.s.low, divisor.s.low, &remainder.s.low);
            quotient.s.high = 0;
        } 
        else 
        {
            quotient.s.high = PrimitiveDiv(0, dividend.s.high, divisor.s.low, &dividend.s.high);
            quotient.s.low = PrimitiveDiv(dividend.s.high, dividend.s.low, divisor.s.low, &remainder.s.low);
        }
        
        if (rem) *rem = remainder.all;
        return quotient.all;
    }
    
    int shift = __builtin_clzll(divisor.s.high) - __builtin_clzll(dividend.s.high);
    divisor.all <<= shift;
    quotient.s.high = 0;
    quotient.s.low = 0;

    for (; shift >= 0; --shift) 
    {
        quotient.s.low <<= 1;
        // const ti_int s = (ti_int)(divisor.all - dividend.all - 1) >> (n_utword_bits - 1);
        // quotient.s.low |= s & 1;
        if (dividend.all >= divisor.all) {
          dividend.all -= divisor.all;
          quotient.s.low |= 1;
        }
        // dividend.all -= divisor.all & s;
        divisor.all >>= 1;
    }

    if (rem) *rem = dividend.all;
    return quotient.all;
}

struct MyDivision2 : public absl::uint128 {
  using Base = absl::uint128;
  using Base::Base;
  MyDivision2(absl::uint128 u) : Base(u) {}
};

absl::uint128 operator/(const MyDivision2& lhs, const MyDivision2& rhs) {
  utwords lhs_u;
  utwords rhs_u;
  lhs_u.s.low = absl::Uint128Low64(lhs);
  lhs_u.s.high = absl::Uint128High64(lhs);
  rhs_u.s.low = absl::Uint128Low64(rhs);
  rhs_u.s.high = absl::Uint128High64(rhs);
  utwords n;
  n.all = MyDivMod1<PrimitiveDiv>(lhs_u.all, rhs_u.all, nullptr);
  return absl::MakeUint128(n.s.high, n.s.low);
}

absl::uint128 operator%(const MyDivision2& lhs, const MyDivision2& rhs) {
  utwords lhs_u;
  utwords rhs_u;
  lhs_u.s.low = absl::Uint128Low64(lhs);
  lhs_u.s.high = absl::Uint128High64(lhs);
  rhs_u.s.low = absl::Uint128Low64(rhs);
  rhs_u.s.high = absl::Uint128High64(rhs);
  utwords rem;
  MyDivMod1<PrimitiveDiv>(lhs_u.all, rhs_u.all, &rem.all);
  return absl::MakeUint128(rem.s.high, rem.s.low);
}