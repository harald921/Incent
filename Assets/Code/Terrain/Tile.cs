using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Tile
{
    public readonly Vector2DInt localPosition; // The chunk position of the tile
    public readonly Vector2DInt chunkPosition; // The world position of the chunk
    public readonly Vector2DInt worldPosition;

    readonly public Terrain terrain;

    public Tile(Vector2DInt inLocalPosition, Vector2DInt inChunkPosition, Terrain inTerrain)
    {
        localPosition = inLocalPosition;
        chunkPosition = inChunkPosition;
        worldPosition = localPosition + (chunkPosition * 64); // TODO: Read from file ONCE

        terrain = inTerrain;
    }

    // public Tile GetNearbyTile(Vector2DInt inDirection) =>
    //     World.instance.chunkManager.GetTile(worldPosition + inDirection);
}