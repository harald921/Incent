using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Tile
{
    public readonly Vector2DInt localPosition; // Position of the tile in the chunk
    public readonly Vector2DInt chunkPosition; // Position of the chunk 
    public readonly Vector2DInt worldPosition; // Position of the tile in the world

    public readonly Node node;
    public readonly Terrain terrain;

    public Furniture furniture { get; private set; }

    public Chunk chunk =>_chunk ?? (_chunk = WorldChunkManager.instance.GetChunk(chunkPosition));
    Chunk _chunk;


    public void SetFurniture(Furniture inFurniture)
    {
        furniture = inFurniture;
    }


    public Tile(Vector2DInt inLocalPosition, Vector2DInt inChunkPosition, Terrain inTerrain)
    {
        localPosition = inLocalPosition;
        chunkPosition = inChunkPosition;
        worldPosition = localPosition + (chunkPosition * Constants.Terrain.CHUNK_SIZE); // TODO: Read from file ONCE

        terrain = inTerrain;

        node = new Node(this);
    }

    public List<Tile> GetNeighbours()
    {
        List<Tile> neighbours = new List<Tile>();

        neighbours.AddIfNotNull(GetRelativeTile(Vector2DInt.Up));   
        neighbours.AddIfNotNull(GetRelativeTile(Vector2DInt.Left));  
        neighbours.AddIfNotNull(GetRelativeTile(Vector2DInt.Right));   
        neighbours.AddIfNotNull(GetRelativeTile(Vector2DInt.Down));  
        neighbours.AddIfNotNull(GetRelativeTile(Vector2DInt.Up   + Vector2DInt.Left));  
        neighbours.AddIfNotNull(GetRelativeTile(Vector2DInt.Up   + Vector2DInt.Right)); 
        neighbours.AddIfNotNull(GetRelativeTile(Vector2DInt.Down + Vector2DInt.Left)); 
        neighbours.AddIfNotNull(GetRelativeTile(Vector2DInt.Down + Vector2DInt.Right));  

        return neighbours;
    }

    public Tile GetRelativeTile(Vector2DInt inOffset) =>
        WorldChunkManager.instance.GetTile(worldPosition + inOffset);
}