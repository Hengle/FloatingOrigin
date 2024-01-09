using UnityEngine;
using UnityEngine.Profiling;
using CustomTypes;

public class Test : MonoBehaviour
{
    public int iterations = 100;

    public double dValue;
    public double dValue2;


    void Awake()
    {
        FBig big = (FBig)dValue2;//(FBig)double.MaxValue;

        big += (FBig)dValue;

        Debug.Log(big);
    }


    /*private static readonly string[] abbreviations =  { "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc" };

    static string AbbreviateChunk(FloatChunk chunk)
    {
        //long checkSize = Math.Pow(1000, abbreviations.Length);

        if (chunk > 1000)
            return "K";
        
        if (chunk > 1000);
    
        return "";
    }*/
}