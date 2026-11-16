using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class MapGenerator : MonoBehaviour
{
    public static event Action<IEnumerable<MapChunk>> OnChunkGenerated;
    public static event Action<List<int2>> OnChunksUnloaded;
    public MapData Map => mapData;

    private Dictionary<int2, MapChunk> loadedChunks;


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

    /// <summary>
    /// The point where the chunks generated around.
    /// </summary>
    [SerializeField] private Transform referencePoint;
    [SerializeField] private bool useInspectorCameraAsReferencePoint;


    [Header("Noise Settings")]
    [SerializeField] [OnValueChanged(nameof(GenerateMap))] private float heightMult = 5;
    [SerializeField] [OnValueChanged(nameof(GenerateMap))] private float frequency = 5;
    [SerializeField] [OnValueChanged(nameof(GenerateMap))] private float jitter = 1;
    [SerializeField] [OnValueChanged(nameof(GenerateMap))] private FastNoiseLite.CellularDistanceFunction distanceFunction;
    [SerializeField] [OnValueChanged(nameof(GenerateMap))] private FastNoiseLite.CellularReturnType returnType;

    private MapData mapData;
    private FastNoiseLite noise;

    private Vector3 lastReferencePointPosition;
    
    [Button]
    public void GenerateMap()
    {
        // ----------- SETUP ---------------
        mapData = new MapData(chunkSize);

        noise = new FastNoiseLite(seed: 1337);
        noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
        noise.SetCellularDistanceFunction(distanceFunction);
        noise.SetCellularReturnType(returnType);
        noise.SetCellularJitter(jitter);
        noise.SetFractalType(FastNoiseLite.FractalType.None);
        noise.SetFrequency(frequency);

        var chunksToCreate = new NativeArray<int2>(GetChunkPositionsAroundReferencePoint(), Allocator.Persistent);

        int verticesPerChunk = subdivide * subdivide * 4; // 4 vertices per quad
        int trisPerChunk = subdivide * subdivide * 6; // 6 indices per quad (2 triangles)
        int uvsPerChunk = subdivide * subdivide * 4; // 4 UVs per quad
        int totalChunks = chunksToCreate.Length;

        var vertices = new NativeArray<float3>(verticesPerChunk * totalChunks, Allocator.Persistent);
        var triangles = new NativeArray<int>(trisPerChunk * totalChunks, Allocator.Persistent);
        var uv = new NativeArray<float2>(uvsPerChunk * totalChunks, Allocator.Persistent);


        // ----------- CREATE CHUNKS IN BATCH ---------------
        var createChunkJob = new CreateChunkJob
        {
            ChunkPositions = chunksToCreate,
            Config = new MapConfig(renderChunks, chunkSize, subdivide, heightMult),
            noise = noise,

            VerticesPerChunk = verticesPerChunk,
            TrianglesPerChunk = trisPerChunk,
            UVsPerChunk = uvsPerChunk,

            Vertices = vertices,
            Triangles = triangles,
            UVs = uv
        }.Schedule(chunksToCreate.Length, 1);
        createChunkJob.Complete();


        // ----------- SLICE DATA AND CREATE MAPCHUNKS ---------------
        List<MapChunk> newChunks = new List<MapChunk>();
        for(int i = 0; i < chunksToCreate.Length; i++)
        {
            MapChunk chunk = new MapChunk(chunksToCreate[i]);
            int startIndexV = i * verticesPerChunk;
            int endIndexV = startIndexV + verticesPerChunk;
            chunk.SetVertices(vertices.Slice(startIndexV, endIndexV - startIndexV));

            int startIndexT = i * trisPerChunk;
            int endIndexT = startIndexT + trisPerChunk;
            chunk.SetTriangles(triangles.Slice(startIndexT, endIndexT - startIndexT));

            int startIndexU = i * uvsPerChunk;
            int endIndexU = startIndexU + uvsPerChunk;
            chunk.SetUVs(uv.Slice(startIndexU, endIndexU - startIndexU));

            loadedChunks.TryAdd(chunksToCreate[i], chunk);
            newChunks.Add(chunk);
        }
        OnChunkGenerated?.Invoke(newChunks);

        // ----------- CLEANUP ---------------
        chunksToCreate.Dispose();
        vertices.Dispose();
        triangles.Dispose();
        uv.Dispose();

        UnloadChunksOutsideRenderDistance();
    }

    private void OnEnable()
    {
        loadedChunks = new Dictionary<int2, MapChunk>();
    }

    private void OnDisable()
    {
        if (loadedChunks != null)
        {
            foreach (var chunk in loadedChunks.Values)
            {
                chunk.Dispose();
            }
            OnChunksUnloaded?.Invoke(loadedChunks.Keys.ToList());
            loadedChunks.Clear();
        }
    }

    private void Update()
    {
        if (useInspectorCameraAsReferencePoint)
        {
            referencePoint = UnityEditor.SceneView.lastActiveSceneView.camera.transform;
        }

        if (referencePoint == null) return;

        if (Vector3.Distance(referencePoint.position, lastReferencePointPosition) > 0.1f)
        {
            GenerateMap();
            lastReferencePointPosition = referencePoint.position;
        }
    }



    private NativeArray<int2> GetChunkPositionsAroundReferencePoint()
    {
        void TryAddChunkPosition(int2 chunkPosition, ref NativeList<int2> chunkPositions)
        {
            if (!loadedChunks.ContainsKey(chunkPosition))
            {
                chunkPositions.Add(chunkPosition);
            }
        }

        Vector3 referencePosition = referencePoint.position;
        int2 referenceChunkPosition = MapHelper.WorldToChunkPosition(referencePosition, chunkSize);

        int halfRenderChunks = Mathf.FloorToInt(renderChunks / 2f);
        var chunkPositions = new NativeList<int2>(Allocator.Temp);

        for (int y = -halfRenderChunks; y <= halfRenderChunks; y++)
        {
            for (int x = -halfRenderChunks; x <= halfRenderChunks; x++)
            {
                int2 chunkPosition = new int2(referenceChunkPosition.x + x, referenceChunkPosition.y + y);
                TryAddChunkPosition(chunkPosition, ref chunkPositions);
            }
        }

        return chunkPositions.AsArray();
    }

    private void UnloadChunksOutsideRenderDistance()
    {
        Vector3 referencePosition = referencePoint.position;
        int2 referenceChunkPosition = new int2(
            Mathf.FloorToInt(referencePosition.x / chunkSize),
            Mathf.FloorToInt(referencePosition.z / chunkSize)
        );

        List<int2> chunksToUnload = new List<int2>();

        foreach (var kvp in loadedChunks)
        {
            int2 chunkPosition = kvp.Key;
            if (math.abs(chunkPosition.x - referenceChunkPosition.x) > renderChunks / 2 ||
                math.abs(chunkPosition.y - referenceChunkPosition.y) > renderChunks / 2)
            {
                chunksToUnload.Add(chunkPosition);
            }
        }

        foreach (var chunkPos in chunksToUnload)
        {
            if (loadedChunks.TryGetValue(chunkPos, out MapChunk chunk))
            {
                chunk.Dispose();
                loadedChunks.Remove(chunkPos);
            }
        }

        OnChunksUnloaded?.Invoke(chunksToUnload);
    }
}

public static class MapHelper
{
    public static int2 WorldToChunkPosition(float3 worldPosition, int chunkSize)
    {
        return new int2(
            Mathf.FloorToInt(worldPosition.x / chunkSize),
            Mathf.FloorToInt(worldPosition.z / chunkSize)
        );
    }
}