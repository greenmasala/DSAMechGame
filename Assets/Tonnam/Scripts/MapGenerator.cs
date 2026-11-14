using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static event Action<MapData, MapChunk> OnChunkGenerated;
    public MapData Map => mapData;

    [SerializeField] [OnValueChanged(nameof(GenerateMap))] private int chunkSize = 10;
    [SerializeField] [OnValueChanged(nameof(GenerateMap))] private int subdivide = 1;
    [SerializeField] [OnValueChanged(nameof(GenerateMap))] private int renderChunks = 5;
    [SerializeField] [OnValueChanged(nameof(GenerateMap))] private float heightMult = 5;
    [SerializeField] [OnValueChanged(nameof(GenerateMap))] private float frequency = 5;
    [SerializeField] [OnValueChanged(nameof(GenerateMap))] private float jitter = 1;
    [SerializeField] [OnValueChanged(nameof(GenerateMap))] private FastNoiseLite.CellularDistanceFunction distanceFunction;
    [SerializeField] [OnValueChanged(nameof(GenerateMap))] private FastNoiseLite.CellularReturnType returnType;

    [Header("References")]
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshCollider meshCollider;

    private MapData mapData;
    private FastNoiseLite noise;
    
    [Button]
    public void GenerateMap()
    {
        mapData = new MapData(chunkSize);
        meshFilter.sharedMesh = new Mesh();

        noise = new FastNoiseLite(seed: 1337);
        noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
        noise.SetCellularDistanceFunction(distanceFunction);
        noise.SetCellularReturnType(returnType);
        noise.SetCellularJitter(jitter);
        noise.SetFractalType(FastNoiseLite.FractalType.None);
        noise.SetFrequency(frequency);

        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uv = new List<Vector2>();

        for(int y = 0; y < renderChunks; y++)
        {
            for(int x = 0; x < renderChunks; x++)
            {
                GenerateChunk(new Vector2Int(x, y), ref vertices, ref triangles, ref uv);
            }
        }

        meshFilter.sharedMesh.SetVertices(vertices);
        meshFilter.sharedMesh.SetTriangles(triangles, 0);
        meshFilter.sharedMesh.SetUVs(0, uv);
        RecalculateMesh();
    }

    [ContextMenu("Recalculate Mesh")]
    private void RecalculateMesh()
    {
        meshFilter.sharedMesh.RecalculateNormals();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }

    private void GenerateChunk(Vector2Int chunkPosition, ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uv)
    {
        if (mapData.chunks.ContainsKey(chunkPosition))
            return;

        var chunk = new MapChunk { position = chunkPosition };
        mapData.chunks.Add(chunkPosition, chunk);

        Debug.DrawRay(new Vector3(chunkPosition.x * chunkSize, 0, chunkPosition.y * chunkSize), Vector3.up * 5, Color.green, 10f);
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

        OnChunkGenerated?.Invoke(mapData, chunk);
    }

    [ContextMenu("Reset Map")]
    public void ResetMap()
    {
        meshFilter.sharedMesh.Clear();
        mapData = null;
    }
}


public class MapData
{
    public int chunkSize;
    public Dictionary<Vector2Int, MapChunk> chunks = new Dictionary<Vector2Int, MapChunk>();
    public Dictionary<int, int> populationCounts = new Dictionary<int, int>();

    public MapData(int chunkSize)
    {
        this.chunkSize = chunkSize;
    }

    public void AddPopulation(string populationName, int count)
    {
        int hash = populationName.GetHashCode();
        if (populationCounts.ContainsKey(hash))
        {
            populationCounts[hash] += count;
        }
        else
        {
            populationCounts[hash] = count;
        }
    }

    public int GetPopulationCount(string populationName)
    {
        int hash = populationName.GetHashCode();
        if (populationCounts.ContainsKey(hash))
        {
            return populationCounts[hash];
        }
        return 0;
    }

    public Vector3 GetRandomPositionInChunk(Vector2Int chunkPosition)
    {
        if (!chunks.ContainsKey(chunkPosition))
            return Vector3.zero;

        float x = UnityEngine.Random.Range(chunkPosition.x * chunkSize, (chunkPosition.x + 1) * chunkSize);
        float z = UnityEngine.Random.Range(chunkPosition.y * chunkSize, (chunkPosition.y + 1) * chunkSize);
        return new Vector3(x, 0, z);
    }
}

public class MapChunk
{
    public Vector2Int position;
    public Dictionary<int, int> populationCounts = new Dictionary<int, int>();
}