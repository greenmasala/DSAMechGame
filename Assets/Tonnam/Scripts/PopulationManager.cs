using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] protected string populationName;
    [SerializeField] protected GameObject prefab;
    [SerializeField] protected float densityPerChunk = 10;

    protected List<GameObject> populations = new List<GameObject>();

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

    public virtual void Populate(MapData mapData, MapChunk chunk)
    {
        int toSpawn = GetNumberToSpawn();
        if (toSpawn <= 0) return;

        for (int i = 0; i < toSpawn; i++)
        {
            Vector3 spawnPosition = mapData.GetRandomPositionInChunk(chunk.position);
            if (spawnPosition != Vector3.zero)
            {
                SpawnPopulation(mapData, chunk, spawnPosition);
            }
        }
    }

    protected virtual void SpawnPopulation(MapData mapData, MapChunk chunk, Vector3 position)
    {
        GameObject newPopulation = Instantiate(prefab, position, Quaternion.identity, transform);
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
