using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public readonly Vector2DInt position;
    public readonly ChunkData data;
    public readonly GameObject viewGO;


    public Chunk(Vector2DInt inPosition, ChunkData inData, GameObject inViewGO)
    {
        position = inPosition;

        data = inData;
        viewGO = inViewGO;
    }
}

public struct ChunkData
{
    public Tile[,] tiles { get; private set; }
    public bool isDirty { get; private set; }

    public Tile GetTile(Vector2DInt inTileCoords) =>
        tiles[inTileCoords.x, inTileCoords.y];

    public void SetTile(Vector2DInt inTileCoords, Tile inTile)
    {
        tiles[inTileCoords.x, inTileCoords.y] = inTile;
        isDirty = true;
    }

    public void SetTiles(Tile[,] inTiles)
    {
        tiles = inTiles;
        isDirty = true;
    }
}