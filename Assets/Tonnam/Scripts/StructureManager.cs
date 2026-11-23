using System.Collections.Generic;
using Unity.Mathematics;
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
        MapGenerator.OnChunksFirstGenerated += Populate;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= ResetPopulation;
        MapGenerator.OnChunksFirstGenerated -= Populate;
    }

    public virtual void Populate(HashSet<int2> chunks)
    {
        foreach (var chunkPos in chunks)
        {
            Populate(chunkPos);
        }
    }

    public virtual void Populate(int2 chunkPos)
    {
        int toSpawn = GetNumberToSpawn();
        if (toSpawn <= 0) return;

        for (int i = 0; i < toSpawn; i++)
        {
            Vector3 spawnPosition = MapGenerator.Instance.GetRandomPositionInChunk(chunkPos);
            if (spawnPosition != Vector3.zero)
            {
                SpawnPopulation(spawnPosition);
            }
        }
    }

    protected virtual void SpawnPopulation(Vector3 position)
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

        int additionalSpawn = UnityEngine.Random.value < fractionalDensity ? 1 : 0;

        return integerDensity + additionalSpawn;
    }
}
