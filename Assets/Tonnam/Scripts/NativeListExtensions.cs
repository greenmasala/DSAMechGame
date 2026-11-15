using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public static class NativeListExtensions
{
    public static List<Vector3> ToVector3List(this NativeList<float3> nativeList)
    {
        var vectorList = new List<Vector3>();
        for (int i = 0; i < nativeList.Length; i++)
        {
            vectorList.Add(nativeList[i]);
        }

        return vectorList;
    }

    public static List<Vector2> ToVector2List(this NativeList<float2> nativeList)
    {
        var vectorList = new List<Vector2>();
        for (int i = 0; i < nativeList.Length; i++)
        {
            vectorList.Add(nativeList[i]);
        }

        return vectorList;
    }

    public static List<int> ToIntList(this NativeList<int> nativeList)
    {
        var intList = new List<int>();
        for (int i = 0; i < nativeList.Length; i++)
        {
            intList.Add(nativeList[i]);
        }

        return intList;
    }
}
