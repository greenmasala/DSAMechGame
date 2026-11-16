using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public static class NativeListExtensions
{
    public static List<Vector3> ToVector3List(this NativeList<float3> nativeList)
    {
        var vectorList = new List<Vector3>();
        vectorList.AddRange(nativeList.ToArray().Select(v => new Vector3(v.x, v.y, v.z)));
        return vectorList;
    }

    public static List<Vector2> ToVector2List(this NativeList<float2> nativeList)
    {
        var vectorList = new List<Vector2>();
        vectorList.AddRange(nativeList.ToArray().Select(v => new Vector2(v.x, v.y)));
        return vectorList;
    }

    public static List<int> ToIntList(this NativeList<int> nativeList)
    {
        var intList = new List<int>();
        intList.AddRange(nativeList.ToArray());
        return intList;
    }
}
