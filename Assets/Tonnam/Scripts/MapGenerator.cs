using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static event Action<MapData, MapChunk> OnChunkGenerated;
    public MapData Map => mapData;

    [SerializeField] private int chunkSize = 10;
    [SerializeField] private int subdivide = 1;
    [SerializeField] private int renderChunks = 5;

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
        meshFilter.sharedMesh.RecalculateNormals();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
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
                vertices.Add(new Vector3(x + ix * quadSize,       0,   y + iy * quadSize));
                vertices.Add(new Vector3(x + ix * quadSize + quadSize,       0,   y + iy * quadSize));
                vertices.Add(new Vector3(x + ix * quadSize,       0,   y + iy * quadSize + quadSize));
                vertices.Add(new Vector3(x + ix * quadSize + quadSize,       0,   y + iy * quadSize + quadSize));

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