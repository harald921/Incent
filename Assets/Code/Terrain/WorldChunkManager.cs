using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunkManager 
{
    public static WorldChunkManager instance { get; private set; }

    public ChunkGenerator chunkGenerator { get; private set; }

    Dictionary<Vector2DInt, Chunk> _loadedChunks = new Dictionary<Vector2DInt, Chunk>();


    public WorldChunkManager()
    {
        instance = this;

        chunkGenerator = new ChunkGenerator(); 

        chunkGenerator.GenerateWorld();

        Player.creatureManager.OnChunkPositionsVisibilityGained += (List<Vector2DInt> inSightedChunkPositions) => {
            foreach (Vector2DInt sightedChunkPosition in inSightedChunkPositions)
                if (!_loadedChunks.ContainsKey(sightedChunkPosition))
                    _loadedChunks.Add(sightedChunkPosition, chunkGenerator.LoadChunk(sightedChunkPosition));
        };

        Player.creatureManager.OnChunkPositionsVisibilityLost += (List<Vector2DInt> inLostVisibleChunks) => {
            foreach (Vector2DInt lostChunkPosition in inLostVisibleChunks)
                if (_loadedChunks.ContainsKey(lostChunkPosition))
                {
                    chunkGenerator.UnloadChunk(_loadedChunks[lostChunkPosition]);
                    _loadedChunks.Remove(lostChunkPosition);
                }
        };
    }


    public void Update()
    {
        foreach (KeyValuePair<Vector2DInt, Chunk> item in _loadedChunks)
            item.Value.Update();
    }

    public void PlaceFurniture(Vector2DInt inTargetTilePosition, Furniture inFurniture)
    {
        HashSet<Vector2DInt> chunkPositionsWithQueuedFurniturePlacements = new HashSet<Vector2DInt>();

        for (int y = 0; y < inFurniture.size.y; y++)
            for (int x = 0; x < inFurniture.size.x; x++)
            {
                Vector2DInt currentTilePosition = inTargetTilePosition + new Vector2DInt(x, y);
                Tile currentTile = GetTile(currentTilePosition);

                currentTile?.SetFurniture(inFurniture);

                if (currentTile == null)
                {
                    Vector2DInt currentTileChunkPos = WorldPosToChunkPos(currentTilePosition);

                    if (ChunkPositionIsWithinWorld(currentTileChunkPos))
                        chunkPositionsWithQueuedFurniturePlacements.Add(currentTileChunkPos);
                }
            }

        foreach (Vector2DInt chunkPosition in chunkPositionsWithQueuedFurniturePlacements)
        {
            // Write the queue to disk, for each chunkPosition. If one already exist, simply add to it. Actually, that could result in things being placed on top of eachother. That's no good. I probably need to re-think this. Wait, can't I just.. not allow furniture to be placed on borders?
        }
    }

    

    public static Vector2DInt WorldPosToChunkPos(Vector2DInt inWorldPosition) =>
        inWorldPosition / Constants.Terrain.CHUNK_SIZE;

    public static Vector2DInt WorldPosToLocalTilePos(Vector2DInt inWorldPosition) =>
        inWorldPosition % Constants.Terrain.CHUNK_SIZE;

    public static bool ChunkPositionIsWithinWorld(Vector2DInt inChunkPosition) =>
        inChunkPosition.x >= 0 && inChunkPosition.y >= 0 && inChunkPosition.x <= Constants.Terrain.WORLD_SIZE && inChunkPosition.y <= Constants.Terrain.WORLD_SIZE;


    public Chunk GetChunk(Vector2DInt inChunkPos) => _loadedChunks[inChunkPos];
    
    public Chunk GetSpawnChunk()
    {
        if (!_loadedChunks.ContainsKey(Vector2DInt.Zero))
            _loadedChunks.Add(Vector2DInt.Zero, chunkGenerator.LoadChunk(Vector2DInt.Zero));

        return _loadedChunks[Vector2DInt.Zero];
    }

    public Tile GetTile(Vector2DInt inWorldPosition)
    {
        if (inWorldPosition.x < 0 || inWorldPosition.y < 0)
        {
            // Debug.LogError("GetTile() tried to access a negative world position");
            return null;
        }

        Vector2DInt chunkPosition = WorldPosToChunkPos(inWorldPosition);
        Vector2DInt tilePosition = WorldPosToLocalTilePos(inWorldPosition);

        if (!_loadedChunks.ContainsKey(chunkPosition))
        {
            // Debug.LogError("GetTile() tried to access a chunk that doesn't exist");
            return null;
        }

        return _loadedChunks[chunkPosition].data.GetTile(tilePosition);
    }
}
