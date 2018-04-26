using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunkManager : MonoBehaviour
{
    public ChunkGenerator chunkGenerator { get; private set; }

    Dictionary<Vector2DInt, Chunk> _chunks = new Dictionary<Vector2DInt, Chunk>();


    void Awake()
    {
        chunkGenerator = new ChunkGenerator(LoadNoiseParameters()); // TODO: Read from file

        GenerateWorld();
    }


    public static Vector2DInt WorldPosToChunkPos(Vector2DInt inWorldPosition) =>
        inWorldPosition / 64;

    public static Vector2DInt WorldPosToLocalTilePos(Vector2DInt inWorldPosition) =>
        inWorldPosition % 64;


    public Chunk GetChunk(Vector2DInt inChunkPos) => _chunks[inChunkPos];

    public Tile GetTile(Vector2DInt inWorldPosition)
    {
        Vector2DInt chunkPosition = WorldPosToChunkPos(inWorldPosition);
        Vector2DInt tilePosition = WorldPosToLocalTilePos(inWorldPosition);

        return _chunks[chunkPosition].data.GetTile(tilePosition);
    }



    void GenerateWorld()
    {
        for (int y = 0; y < 16; y++)
            for (int x = 0; x < 16; x++)
                _chunks.Add(new Vector2DInt(x, y), chunkGenerator.GenerateChunk(new Vector2DInt(x, y)));
    }

    Noise.Parameters[] LoadNoiseParameters()
    {
        // TODO: Read form disk
        return new Noise.Parameters[]
        {
                new Noise.Parameters()
                {
                    scale       = 50,
                    octaves     = 7,
                    persistance = 1.01f,
                    lacunarity  = 1.01f,
                    seed        = 0
                }
        };
    }
}
