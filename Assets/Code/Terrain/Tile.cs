using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Tile
{
    public readonly Vector2Int localPosition; // The chunk position of the tile
    public readonly Vector2Int chunkPosition; // The world position of the chunk
    // public readonly Vector2Int worldPosition;

    Terrain _terrain;


    public Tile(Vector2Int inLocalPosition, Vector2Int inChunkPosition, Terrain inTerrain)
    {
        localPosition = inLocalPosition;
        chunkPosition = inChunkPosition;
       //  worldPosition = localPosition + (chunkPosition * Constants.TerrainGeneration.CHUNK_SIZE);

        _terrain = inTerrain;
    }

    // public Tile GetNearbyTile(Vector2Int inDirection) =>
    //     World.instance.chunkManager.GetTile(worldPosition + inDirection);
}