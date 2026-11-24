public struct MapConfig
{
    public int RenderDistance;
    public int ChunkSize;
    public int Subdivide;
    public float HeightMult;

    public MapConfig(int renderDistance, int chunkSize, int subdivide, float heightMult)
    {
        RenderDistance = renderDistance;
        ChunkSize = chunkSize;
        Subdivide = subdivide;
        HeightMult = heightMult;
    }
}
