using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MapGenerator : MonoBehaviour
{
    public static event Action<MapData, MapChunk> OnChunkGenerated;
    public MapData Map => mapData;

    [SerializeField] private int chunkSize = 10;
    [SerializeField] private int subdivide = 1;
    [SerializeField] private int renderChunks = 5;
    [SerializeField] private float heightMult = 5;
    [SerializeField] private float noiseScale = 5;

    [Header("References")]
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshCollider meshCollider;

    private MapData mapData;
    
    [ContextMenu("Generate Map")]
    public void GenerateMap()
    {
        mapData = new MapData(chunkSize);
        meshFilter.sharedMesh = new Mesh();

        var vertices = new List<Vector3>();
        var triangles = new List<int>();

        for(int y = 0; y < renderChunks; y++)
        {
            for(int x = 0; x < renderChunks; x++)
            {
                GenerateChunk(new Vector2Int(x, y), ref vertices, ref triangles);
            }
        }

        meshFilter.sharedMesh.SetVertices(vertices);
        meshFilter.sharedMesh.SetTriangles(triangles, 0);
        RecalculateMesh();
    }

    [ContextMenu("Recalculate Mesh")]
    private void RecalculateMesh()
    {
        meshFilter.sharedMesh.RecalculateNormals();
        meshFilter.sharedMesh.RecalculateUVDistributionMetrics();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }

    private void SyncHeightMap()
    {
    }

    private void GenerateChunk(Vector2Int chunkPosition, ref List<Vector3> vertices, ref List<int> triangles)
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

                float h1 = Mathf.PerlinNoise(v1.x, v1.y) * heightMult;
                float h2 = Mathf.PerlinNoise(v2.x, v2.y) * heightMult;
                float h3 = Mathf.PerlinNoise(v3.x, v3.y) * heightMult;
                float h4 = Mathf.PerlinNoise(v4.x, v4.y) * heightMult;

                vertices.Add(new Vector3(v1.x,   h1,   v1.y));
                vertices.Add(new Vector3(v2.x,   h2,   v2.y));
                vertices.Add(new Vector3(v3.x,   h3,   v3.y));
                vertices.Add(new Vector3(v4.x,   h4,   v4.y));

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