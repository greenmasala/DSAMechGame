using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct CreateChunkJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<int2> ChunkPositions;
    [ReadOnly] public MapConfig Config;
    [ReadOnly] public FastNoiseLite noise;

    [ReadOnly] public int VerticesPerChunk;
    [ReadOnly] public int TrianglesPerChunk;
    [ReadOnly] public int UVsPerChunk;

    // output
    [NativeDisableParallelForRestriction] public NativeArray<float3> Vertices;
    [NativeDisableParallelForRestriction] public NativeArray<int> Triangles;
    [NativeDisableParallelForRestriction] public NativeArray<float2> UVs;

    public void Execute(int index)
    {
        int offsetV = index * VerticesPerChunk;
        int offsetT = index * TrianglesPerChunk;
        int offsetU = index * UVsPerChunk;

        int2 chunkPosition = ChunkPositions[index];
        int chunkSize = Config.ChunkSize;
        int subdivide = Config.Subdivide;
        float heightMult = Config.HeightMult;

        int x = chunkPosition.x * chunkSize;
        int y = chunkPosition.y * chunkSize;

        float quadSize = (float)chunkSize / subdivide;

        for (int ix = 0; ix < subdivide; ix++)
        {
            for (int iy = 0; iy < subdivide; iy++)
            {
                Vector2 v1 = new Vector2(x + ix * quadSize,              y + iy * quadSize);
                Vector2 v2 = new Vector2(x + ix * quadSize + quadSize,   y + iy * quadSize);
                Vector2 v3 = new Vector2(x + ix * quadSize,              y + iy * quadSize + quadSize);
                Vector2 v4 = new Vector2(x + ix * quadSize + quadSize,   y + iy * quadSize + quadSize);

                float h1 = noise.GetNoise(v1.x, v1.y) * heightMult;
                float h2 = noise.GetNoise(v2.x, v2.y) * heightMult;
                float h3 = noise.GetNoise(v3.x, v3.y) * heightMult;
                float h4 = noise.GetNoise(v4.x, v4.y) * heightMult;

                int qIndex = (iy * subdivide + ix) * 4; // 4 vertices per quad
                int tIndex = (iy * subdivide + ix) * 6; // 6 indices per quad (2 triangles)

                Vertices[offsetV + qIndex]     = new float3(v1.x, h1, v1.y);
                Vertices[offsetV + qIndex + 1] = new float3(v2.x, h2, v2.y);
                Vertices[offsetV + qIndex + 2] = new float3(v3.x, h3, v3.y);
                Vertices[offsetV + qIndex + 3] = new float3(v4.x, h4, v4.y);

                UVs[offsetU + qIndex]     = new float2(ix / (float)subdivide, iy / (float)subdivide);
                UVs[offsetU + qIndex + 1] = new float2((ix + 1) / (float)subdivide, iy / (float)subdivide);
                UVs[offsetU + qIndex + 2] = new float2(ix / (float)subdivide, (iy + 1) / (float)subdivide);
                UVs[offsetU + qIndex + 3] = new float2((ix + 1) / (float)subdivide, (iy + 1) / (float)subdivide);

                Triangles[offsetT + tIndex]     = qIndex;
                Triangles[offsetT + tIndex + 1] = qIndex + 2;
                Triangles[offsetT + tIndex + 2] = qIndex + 1;
                Triangles[offsetT + tIndex + 3] = qIndex + 1;
                Triangles[offsetT + tIndex + 4] = qIndex + 2;
                Triangles[offsetT + tIndex + 5] = qIndex + 3;
            }
        }
    }
}
