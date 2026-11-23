using Unity.Mathematics;
using UnityEngine;

public class ChunkMesh : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshCollider meshCollider;

    private MapChunk currentChunk;

    private float3 firstVertex;

    public void Set(MapChunk chunk)
    {
        var (vertices, triangles, uv) = chunk.GetMeshData();

        currentChunk = chunk;
        firstVertex = vertices[0];

        Mesh mesh = new Mesh();
        meshFilter.sharedMesh = mesh;
        meshFilter.sharedMesh.SetVertices(vertices.AsArray());
        meshFilter.sharedMesh.SetIndices(triangles.AsArray(), MeshTopology.Triangles, 0);
        meshFilter.sharedMesh.SetUVs(0, uv.AsArray());
        RecalculateMesh();
    }

    public void Clear()
    {
        meshFilter.sharedMesh.Clear();
        meshCollider.sharedMesh = null;
        currentChunk = default;
    }

    public int2 GetChunkPosition() => currentChunk.ChunkPosition;

    private void RecalculateMesh()
    {
        meshFilter.sharedMesh.RecalculateNormals();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }

    private void Update()
    {
        Debug.DrawRay(firstVertex, Vector3.up * 100f, Color.green, 0.1f);
    }
}
