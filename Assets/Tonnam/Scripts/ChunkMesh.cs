using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class ChunkMesh : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshCollider meshCollider;

    public void Set(MapChunk chunk)
    {
        var (vertices, triangles, uv) = chunk.GetMeshData();

        meshFilter.sharedMesh.SetVertices(vertices.ToVector3List());
        meshFilter.sharedMesh.SetTriangles(triangles.ToIntList(), 0);
        meshFilter.sharedMesh.SetUVs(0, uv.ToVector2List());
        RecalculateMesh();
    }

    private void RecalculateMesh()
    {
        meshFilter.sharedMesh.RecalculateNormals();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }
}
