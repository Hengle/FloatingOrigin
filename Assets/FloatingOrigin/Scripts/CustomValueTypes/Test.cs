using UnityEngine;
using UnityEngine.Profiling;
using BigIntegers;
using System.Numerics;
using System;

public class Test : MonoBehaviour
{
    public int iterations = 100;


    public double daVal;
    public double daVal2;
    public long prePart;
    public ushort postPart;


    void Awake()
    {
        Fixed128 ting = (Fixed128)daVal;

        Debug.Log(ting.ToString());
    }
}