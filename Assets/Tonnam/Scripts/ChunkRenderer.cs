using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class ChunkRenderer : MonoBehaviour
{
    [SerializeField] private ChunkMesh chunkMeshPrefab;
    [SerializeField] private Transform chunkParent;

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
    }

    private void OnDisable()
    {
        MapGenerator.OnChunkGenerated -= HandleChunkGenerated;
    }

    private void HandleChunkGenerated(IEnumerable<MapChunk> chunks)
    {
        foreach (var chunk in chunks)
        {
            var chunkMesh = chunkMeshPool.Get();
            chunkMesh.Set(chunk);
        }
    }
}
