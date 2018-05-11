using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Responsible for holding all the loaded chunks
public class WorldChunkManager 
{
    public static WorldChunkManager instance { get; private set; }

    public ChunkGenerator chunkGenerator { get; private set; }

    Dictionary<Vector2DInt, Chunk> _chunks = new Dictionary<Vector2DInt, Chunk>();


    public WorldChunkManager()
    {
        instance = this;

        chunkGenerator = new ChunkGenerator(); // TODO: Read from file

        chunkGenerator.GenerateWorld();

        _chunks.Add(Vector2DInt.Zero, chunkGenerator.LoadChunk(Vector2DInt.Zero));
    }


    public static Vector2DInt WorldPosToChunkPos(Vector2DInt inWorldPosition) =>
        inWorldPosition / Constants.Terrain.CHUNK_SIZE;

    public static Vector2DInt WorldPosToLocalTilePos(Vector2DInt inWorldPosition) =>
        inWorldPosition % Constants.Terrain.CHUNK_SIZE;


    public Chunk GetChunk(Vector2DInt inChunkPos) => _chunks[inChunkPos];

    public Tile GetTile(Vector2DInt inWorldPosition)
    {
        if (inWorldPosition.x < 0 || inWorldPosition.y < 0)
        {
            Debug.LogError("GetTile() tried to access a negative world position");
            return null;
        }

        Vector2DInt chunkPosition = WorldPosToChunkPos(inWorldPosition);
        Vector2DInt tilePosition = WorldPosToLocalTilePos(inWorldPosition);

        if (!_chunks.ContainsKey(chunkPosition))
        {
            Debug.LogError("GetTile() tried to access a chunk that doesn't exist");
            return null;
        }

        return _chunks[chunkPosition].data.GetTile(tilePosition);
    }
}
