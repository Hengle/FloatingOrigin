using UnityEngine;
using UnityEngine.Profiling;
using BigIntegers;

public class Test : MonoBehaviour
{
    public int iterations = 100;


    public long bigVal;
    public long otherBigVal;


    void Awake()
    {
        Int128 bigLad = bigVal;
        Debug.Log(AbbreviateChunk(bigLad));

    }


    private static readonly string[] abbreviations =  { "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc", "UnDc" };

    static string AbbreviateChunk(Int128 chunk)
    {
        if (chunk < 1000)
            return chunk.ToString();

        for (int i = 0; i < abbreviations.Length; i++)
        {
            Int128 valueLo = Int128.Pow(1000, i + 1);
            Int128 valueHi = Int128.Pow(1000, i + 2);

            if (chunk >= valueLo && chunk < valueHi)
                return (chunk / valueLo) + abbreviations[i];
        }
    
        return chunk.ToString();
    }
}