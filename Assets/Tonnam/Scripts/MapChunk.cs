using System;
using Unity.Collections;
using Unity.Mathematics;

public struct MapChunk : IDisposable
{
    public int2 ChunkPosition;
    public NativeParallelHashMap<int, MapObject> ObjectData;

    private NativeList<float3> vertices;
    private NativeList<int> triangles;
    private NativeList<float2> uv;

    public MapChunk(int2 chunkPosition)
    {
        ChunkPosition = chunkPosition;

        ObjectData = new NativeParallelHashMap<int, MapObject>(100, Allocator.Persistent); // 100 is arbitrary initial capacity
        vertices   = new NativeList<float3>(Allocator.Persistent);
        triangles  = new NativeList<int>(Allocator.Persistent);
        uv         = new NativeList<float2>(Allocator.Persistent);
    }

    public void SetVertices(NativeList<float3> newVertices)
    {
        if (vertices.IsCreated) vertices.Dispose();
        vertices = newVertices;
    }

    public void SetTriangles(NativeList<int> newTriangles)
    {
        if (triangles.IsCreated) triangles.Dispose();
        triangles = newTriangles;
    }

    public void SetUVs(NativeList<float2> newUVs)
    {
        if (uv.IsCreated) uv.Dispose();
        uv = newUVs;
    }

    public (NativeList<float3> Vertices, NativeList<int> Triangles, NativeList<float2> UV) GetMeshData()
    {
        return (vertices, triangles, uv);
    }

    public void Dispose()
    {
        if (ObjectData.IsCreated) ObjectData.Dispose();
        if (vertices.IsCreated)   vertices.Dispose();
        if (triangles.IsCreated)  triangles.Dispose();
    }
}
