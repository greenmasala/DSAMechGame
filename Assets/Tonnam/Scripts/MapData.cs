using System.Collections.Generic;
using UnityEngine;

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
