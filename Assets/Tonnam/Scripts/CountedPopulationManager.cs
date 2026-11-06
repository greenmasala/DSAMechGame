using UnityEngine;

public class CountedPopulationManager : PopulationManager
{
    [SerializeField] private int maxPopulation = 100;

    public override void Populate(MapData mapData, MapChunk chunk)
    {
        if (mapData.GetPopulationCount(populationName) >= maxPopulation)
            return;

        base.Populate(mapData, chunk);
    }

    protected override void SpawnPopulation(MapData mapData, MapChunk chunk, Vector3 position)
    {
        if (mapData.GetPopulationCount(populationName) < maxPopulation)
        {
            base.SpawnPopulation(mapData, chunk, position);
            mapData.AddPopulation(populationName, 1);
        }
    }
}