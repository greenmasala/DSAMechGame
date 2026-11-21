using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    [SerializeField] protected string populationName;
    [SerializeField] protected MapStructure prefab;
    [SerializeField] protected float densityPerChunk = 10;

    protected List<MapStructure> populations = new List<MapStructure>();

    private void OnEnable()
    {
        GameManager.OnGameOver += ResetPopulation;
        MapGenerator.OnChunkGenerated += Populate;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= ResetPopulation;
        MapGenerator.OnChunkGenerated -= Populate;
    }

    public virtual void Populate(IEnumerable<MapChunk> chunks)
    {
        foreach (var chunk in chunks)
        {
            Populate(chunk);
        }
    }

    public virtual void Populate(MapChunk chunk)
    {
        int toSpawn = GetNumberToSpawn();
        if (toSpawn <= 0) return;

        for (int i = 0; i < toSpawn; i++)
        {
            Vector3 spawnPosition = MapGenerator.Instance.GetRandomPositionInChunk(chunk.ChunkPosition);
            if (spawnPosition != Vector3.zero)
            {
                SpawnPopulation(chunk, spawnPosition);
            }
        }
    }

    protected virtual void SpawnPopulation(MapChunk chunk, Vector3 position)
    {
        MapStructure newPopulation = Instantiate(prefab, position, Quaternion.identity, transform);
        populations.Add(newPopulation);
    } 

    [ContextMenu("Reset Population")]
    public virtual void ResetPopulation()
    {
        foreach (var population in populations)
        {
            if (population == null) continue;
            Destroy(population.gameObject);
        }
        populations.Clear();
    }

    protected int GetNumberToSpawn()
    {
        int integerDensity = Mathf.FloorToInt(densityPerChunk);
        float fractionalDensity = densityPerChunk - integerDensity;

        int additionalSpawn = Random.value < fractionalDensity ? 1 : 0;

        return integerDensity + additionalSpawn;
    }
}
