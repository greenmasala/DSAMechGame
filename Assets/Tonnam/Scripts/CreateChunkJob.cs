using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct CreateChunkJob : IJobParallelFor
{
    public NativeArray<MapChunk> Chunks;
    public MapConfig Config;
    public FastNoiseLite noise;

    public void Execute(int index)
    {
        var vertices  = new NativeList<float3>(Allocator.Temp);
        var triangles = new NativeList<int>(Allocator.Temp);
        var uv        = new NativeList<float2>(Allocator.Temp);

        var chunk = Chunks[index];

        int2 chunkPosition = chunk.ChunkPosition;
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

                vertices.Add(new Vector3(v1.x,   h1,   v1.y));
                vertices.Add(new Vector3(v2.x,   h2,   v2.y));
                vertices.Add(new Vector3(v3.x,   h3,   v3.y));
                vertices.Add(new Vector3(v4.x,   h4,   v4.y));

                uv.Add(new Vector2(ix / (float)subdivide, iy / (float)subdivide));
                uv.Add(new Vector2((ix + 1) / (float)subdivide, iy / (float)subdivide));
                uv.Add(new Vector2(ix / (float)subdivide, (iy + 1) / (float)subdivide));
                uv.Add(new Vector2((ix + 1) / (float)subdivide, (iy + 1) / (float)subdivide));

                int vertIndex = vertices.Count - 4;
                triangles.Add(vertIndex);
                triangles.Add(vertIndex + 2);
                triangles.Add(vertIndex + 1);
                
                triangles.Add(vertIndex + 1);
                triangles.Add(vertIndex + 2);
                triangles.Add(vertIndex + 3);
            }
        }

        chunk.SetVertices(vertices);
        chunk.SetTriangles(triangles);
        chunk.SetUVs(uv);

        Chunks[index] = chunk;
    }
}
