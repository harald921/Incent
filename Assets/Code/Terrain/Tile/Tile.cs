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

        node = new Node(this);
    }


    public Tile GetNearbyTile(Vector2DInt inDirection) =>
        WorldChunkManager.instance.GetTile(worldPosition + inDirection);

    public int DistanceTo(Tile inTargetTile)
    {
        int distanceX = Mathf.Abs(worldPosition.x - inTargetTile.worldPosition.x);
        int distanceY = Mathf.Abs(worldPosition.y - inTargetTile.worldPosition.y);

        return (distanceX > distanceY) ? 14 * distanceY + 10 * (distanceX - distanceY) :
                                         14 * distanceX + 10 * (distanceY - distanceX);
    }

    public List<Tile> GetNeighbours()
    {
        List<Tile> neighbours = new List<Tile>();

        neighbours.AddIfNotNull(GetNearbyTile(new Vector2DInt(-1, 1)));  // LeftUp       
        neighbours.AddIfNotNull(GetNearbyTile(new Vector2DInt(0, 1)));   // Up           
        neighbours.AddIfNotNull(GetNearbyTile(new Vector2DInt(1, 1)));   // RightUp      
        neighbours.AddIfNotNull(GetNearbyTile(new Vector2DInt(-1, 0)));  // Left         
        neighbours.AddIfNotNull(GetNearbyTile(new Vector2DInt(1, 0)));   // Right        
        neighbours.AddIfNotNull(GetNearbyTile(new Vector2DInt(-1, -1))); // LeftDown 
        neighbours.AddIfNotNull(GetNearbyTile(new Vector2DInt(0, -1)));  // Down
        neighbours.AddIfNotNull(GetNearbyTile(new Vector2DInt(1, -1)));  // RightDown

        return neighbours;
    }
}