using System;
using NaughtyAttributes;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static event Action<NativeArray<MapChunk>> OnChunkGenerated;
    public MapData Map => mapData;

    private NativeParallelHashMap<int2, MapChunk> loadedChunks;


    [Header("Map Settings")]

    /// <summary>
    /// The size of each chunk in world units. This determines how large each chunk will be in the game world. 
    /// This will affect the size of quads and not how many quads are generated. The number of quads is determined by the subdivide value.
    /// </summary>
    [SerializeField] [OnValueChanged(nameof(GenerateMap))] private int chunkSize = 10;

    /// <summary>
    /// The number of quads to generate per chunk. This determines how many quads will be generated within each chunk.
    /// There will always be subdivide * subdivide quads generated per chunk.
    /// </summary>
    [SerializeField] [OnValueChanged(nameof(GenerateMap))] private int subdivide = 1;

    /// <summary>
    /// The number of chunks to render in the game world.
    /// There will always be renderChunks * renderChunks chunks loaded in the game world.
    /// </summary>
    [SerializeField] [OnValueChanged(nameof(GenerateMap))] private int renderChunks = 5;


    [Header("Noise Settings")]
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

        var chunksToCreate = new NativeArray<MapChunk>(renderChunks * renderChunks, Allocator.Persistent);

        for(int y = 0; y < renderChunks; y++)
        {
            for(int x = 0; x < renderChunks; x++)
            {
                Vector2Int chunkPosition = new Vector2Int(x, y);
                int index = y * renderChunks + x;
                chunksToCreate[index] = new MapChunk(chunkPosition);
            }
        }

        var createChunkJob = new CreateChunkJob
        {
            Chunks = chunksToCreate,
            Config = new MapConfig(renderChunks, chunkSize, subdivide, heightMult),
            noise = noise
        }.Schedule(chunksToCreate.Length, 1);

        createChunkJob.Complete();
        OnChunkGenerated?.Invoke(chunksToCreate);

        chunksToCreate.Dispose();
    }
}
