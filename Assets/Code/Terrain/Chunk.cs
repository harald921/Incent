using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public readonly Vector2Int position;
    public readonly Data data;
    public readonly GameObject viewGO;



    public Chunk(Vector2Int inPosition, Data inData, GameObject inViewGO)
    {
        position = inPosition;

        data = inData;
        viewGO = inViewGO;
    }

    public class Data
    {
        public Tile[,] tiles { get; private set; }
        public bool isDirty { get; private set; }

        public event Action<Data> OnDataChanged;


        public Tile GetTile(Vector2Int inTileCoords) =>
            tiles[inTileCoords.x, inTileCoords.y];

        public void SetTile(Vector2Int inTileCoords, Tile inTile)
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
}