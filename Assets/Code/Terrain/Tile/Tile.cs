using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Tile
{
    public readonly Vector2DInt localPosition; // Chunk position of the tile
    public readonly Vector2DInt chunkPosition; // World position of the parent chunk
    public readonly Vector2DInt worldPosition; 

    readonly public Terrain terrain;

    public readonly Node node;

    public Tile(Vector2DInt inLocalPosition, Vector2DInt inChunkPosition, Terrain inTerrain)
    {
        localPosition = inLocalPosition;
        chunkPosition = inChunkPosition;
        worldPosition = localPosition + (chunkPosition * Constants.Terrain.CHUNK_SIZE); // TODO: Read from file ONCE

        terrain = inTerrain;
    }

    // public Tile GetNearbyTile(Vector2DInt inDirection) =>
    //     World.instance.chunkManager.GetTile(worldPosition + inDirection);

    public List<Tile> GetNeighbours()
    {
        Debug.LogError("Not implemented");
        return new List<Tile>();
    }
}

public class Node
{
    public readonly Tile owner;
    public Tile parent;

    public int distanceToStart;
    public int distanceToEnd;

    public int totalCost => distanceToStart + distanceToEnd;


    public Node(Tile inOwner)
    {
        owner = inOwner;
    }
}