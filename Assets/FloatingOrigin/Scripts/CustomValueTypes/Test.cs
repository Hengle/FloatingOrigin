using UnityEngine;
using UnityEngine.Profiling;
using BigIntegers;
using System.Numerics;

public class Test : MonoBehaviour
{
    public int iterations = 100;


    public long bigVal;
    public long otherBigVal;


    void Awake()
    {
        UInt128.ProfileDivision(iterations);
    }
}