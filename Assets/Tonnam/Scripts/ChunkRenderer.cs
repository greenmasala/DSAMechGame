using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public class ChunkRenderer : MonoBehaviour
{
    [SerializeField] private ChunkMesh chunkMeshPrefab;
    [SerializeField] private Transform chunkParent;

    private List<ChunkMesh> activeChunkMeshes = new List<ChunkMesh>();
    private IObjectPool<ChunkMesh> chunkMeshPool;

    private void Awake()
    {
        chunkMeshPool = new ObjectPool<ChunkMesh>(
            createFunc: () => Instantiate(chunkMeshPrefab, chunkParent),
            actionOnGet: (chunkMesh) => chunkMesh.gameObject.SetActive(true),
            actionOnRelease: (chunkMesh) => chunkMesh.gameObject.SetActive(false),
            actionOnDestroy: (chunkMesh) => Destroy(chunkMesh.gameObject),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 20
        );
    }

    private void OnEnable()
    {
        MapGenerator.OnChunkGenerated += HandleChunkGenerated;
        MapGenerator.OnChunksUnloaded += HandleChunksUnloaded;
    }

    private void OnDisable()
    {
        MapGenerator.OnChunkGenerated -= HandleChunkGenerated;
        MapGenerator.OnChunksUnloaded -= HandleChunksUnloaded;
    }

    private void HandleChunkGenerated(IEnumerable<MapChunk> chunks)
    {
        foreach (var chunk in chunks)
        {
            var chunkMesh = chunkMeshPool.Get();
            chunkMesh.Set(chunk);
            activeChunkMeshes.Add(chunkMesh);
        }
    }

    private void HandleChunksUnloaded(List<int2> chunkPositions)
    {
        for (int i = activeChunkMeshes.Count - 1; i >= 0; i--)
        {
            var chunkMesh = activeChunkMeshes[i];
            if (chunkPositions.Contains(chunkMesh.GetChunkPosition()))
            {
                chunkMesh.Clear();
                chunkMeshPool.Release(chunkMesh);
                activeChunkMeshes.RemoveAt(i);
            }
        }
    }
}
