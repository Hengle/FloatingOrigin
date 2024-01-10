using System;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using BigIntegers;

public static class Int128Tester
{
    static int test = 0;


    static void Validate(Int128 orig, BigInteger orig2, Int128 res1, BigInteger res2)
    {   
        test++;

        if ((BigInteger)res1 != res2)
            throw new InvalidOperationException($"Failed test {test}: values: {orig}, {orig2}. output: Int128: {res1}, default: {res2}");
    }


    private static void TestAdd(Int128 value, long otherValue)
    {
        Int128 result = value + otherValue;
        BigInteger oresult = (BigInteger)value + (BigInteger)otherValue;

        Validate(value, otherValue, result, oresult);
    }


    private static void TestSub(Int128 value, long otherValue)
    {
        Int128 result = value - otherValue;
        BigInteger oresult = (BigInteger)value - (BigInteger)otherValue;

        Validate(value, otherValue, result, oresult);
    }


    private static void TestMul(Int128 value, long otherValue)
    {
        Int128 result = value * otherValue;
        BigInteger oresult = (BigInteger)value * (BigInteger)otherValue;

        Validate(value, otherValue, result, oresult);
    }


    private static void TestDiv(Int128 value, long otherValue)
    {
        Int128 result = value / otherValue;
        BigInteger oresult = (BigInteger)value / (BigInteger)otherValue;

        Validate(value, (BigInteger)otherValue, result, oresult);
    }


    private static void TestMod(Int128 value, long otherValue)
    {
        Int128 result = value % otherValue;
        BigInteger oresult = (BigInteger)value % (BigInteger)otherValue;

        // Absolute value of oresult since Int128 modulus always returns positive
        Validate(value, otherValue, result, BigInteger.Abs(oresult));
    }


    private static readonly Random random = new();
    private static readonly byte[] buffer = new byte[8];

    private static ulong GetUNum()
    {
        random.NextBytes(buffer);
        return BitConverter.ToUInt64(buffer, 0) + 1;
    }

    private static long GetNum()
    {
        return (long)GetUNum();
    }

    private static long GetMiniNum()
    {
        return random.Next(1, 100000);
    }

    private static Int128 GetMegaNum()
    {
        return new Int128(GetUNum()) + long.MaxValue;
    }


    static void TestAddition()
    {
        TestAdd(GetNum(), GetNum());
        TestAdd(-GetNum(), GetNum());
        TestAdd(GetNum(), -GetNum());
        TestAdd(-GetNum(), -GetNum());

        // Above long range
        TestAdd(GetMegaNum(), GetNum());
        TestAdd(-GetMegaNum(), GetNum());
        TestAdd(GetMegaNum(), -GetNum());
        TestAdd(-GetMegaNum(), -GetNum());
    }

    static void TestSubtraction()
    {
        TestSub(GetNum(), GetNum());
        TestSub(-GetNum(), GetNum());
        TestSub(GetNum(), -GetNum());
        TestSub(-GetNum(), -GetNum());

        // Above long range
        TestSub(GetMegaNum(), GetNum());
        TestSub(-GetMegaNum(), GetNum());
        TestSub(GetMegaNum(), -GetNum());
        TestSub(-GetMegaNum(), -GetNum());
    }

    static void TestMultiplication()
    {
        TestMul(GetNum(), GetMiniNum());
        TestMul(-GetNum(), GetMiniNum());
        TestMul(GetNum(), -GetMiniNum());
        TestMul(-GetNum(), -GetMiniNum());

        // Above long range
        TestMul(GetMegaNum(), GetMiniNum());
        TestMul(-GetMegaNum(), GetMiniNum());
        TestMul(GetMegaNum(), -GetMiniNum());
        TestMul(-GetMegaNum(), -GetMiniNum());
    }

    static void TestDivision()
    {
        TestDiv(GetNum(), GetNum());
        TestDiv(-GetNum(), GetNum());
        TestDiv(GetNum(), -GetNum());
        TestDiv(-GetNum(), -GetNum());

        // Above long range
        TestDiv(GetMegaNum(), GetNum());
        TestDiv(-GetMegaNum(), GetNum());
        TestDiv(GetMegaNum(), -GetNum());
        TestDiv(-GetMegaNum(), -GetNum());
    }


    static void TestModulus()
    {
        TestMod(GetNum(), GetMiniNum());
        TestMod(-GetNum(), GetMiniNum());
        TestMod(GetNum(), -GetMiniNum());
        TestMod(-GetNum(), -GetMiniNum());

        // Above long range
        TestMod(GetMegaNum(), GetMiniNum());
        TestMod(-GetMegaNum(), GetMiniNum());
        TestMod(GetMegaNum(), -GetMiniNum());
        TestMod(-GetMegaNum(), -GetMiniNum());
    }



    static void RunTest(string name, Action test, int iterations)
    {
        UnityEngine.Debug.Log($"Testing {name}");
        Int128Tester.test = 0;

        for (int i = 0; i < iterations; i++)
            test.Invoke();
    }



    public static void Test(int iterations)
    {
        RunTest("Addition", TestAddition, iterations);
        RunTest("Subtraction", TestSubtraction, iterations);
        RunTest("Multiplication", TestMultiplication, iterations);
        RunTest("Division", TestDivision, iterations);
        RunTest("Modulus", TestModulus, iterations);
    }



    public static void TestSpeed(int iterations)
    {
        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            Int128 bigIntA = (ulong)i + ulong.MaxValue;
            Int128 bigIntB = (ulong)i / 2 + ulong.MaxValue;

            for (int j = 0; j < 3; j++)
            {
                Int128 bigIntC = bigIntA + (bigIntB + 1);
            }
        }

        stopwatch.Stop();

        UnityEngine.Debug.Log($"Int128 speed: {stopwatch.ElapsedMilliseconds}ms");

        stopwatch.Reset();
        stopwatch.Start();


        for (int i = 0; i < iterations; i++)
        {
            BigInteger bigIntA = (ulong)i + ulong.MaxValue;
            BigInteger bigIntB = (ulong)i / 2 + ulong.MaxValue;

            for (int j = 0; j < 3; j++)
            {
                BigInteger bigIntC = bigIntA + (bigIntB + 1);
            }
        }

        UnityEngine.Debug.Log($"BigInteger speed: {stopwatch.ElapsedMilliseconds}ms");

        stopwatch.Stop();
    }  


    public static void TestTrace()
    {
        UInt128.TraceDivision = true;
        Int128 res = (Int128)321846 / 866;
        UInt128.TraceDivision = false;
    }  
}