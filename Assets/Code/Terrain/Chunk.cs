using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Chunk
{
    public readonly ChunkData data;
    public readonly GameObject viewGO;

    public Chunk(ChunkData inData, GameObject inViewGO)
    {
        data = inData;

    }
}


public class ChunkData 
{
    public readonly Vector2DInt position;

    public Tile[,] tiles { get; private set; }

    public Tile GetTile(Vector2DInt inTileCoords) =>
        tiles[inTileCoords.x, inTileCoords.y];

    public event Action<ChunkData> OnDataDirtied;


    public ChunkData(Vector2DInt inPosition)
    {
        position = inPosition;
    }


    public void SetTile(Vector2DInt inTileCoords, Tile inTile)
    {
        tiles[inTileCoords.x, inTileCoords.y] = inTile;
        OnDataDirtied?.Invoke(this);
    }

    public void SetTiles(Tile[,] inTiles)
    {
        tiles = inTiles;
        OnDataDirtied?.Invoke(this);
    }


    public void BinarySave()
    {
        FileStream stream = File.OpenWrite(Constants.Terrain.CHUNK_SAVE_FOLDER + "\\" + position.ToString() + ".chunk");
        BinaryWriter writer = new BinaryWriter(stream);

        foreach (Tile tile in tiles)
            writer.Write((UInt16)tile.terrain.type);
    }

    public void BinaryLoad(Vector2DInt inPosition)
    {
        // Open chunk save file
        string chunkFilePath = Constants.Terrain.CHUNK_SAVE_FOLDER + "\\" + inPosition.ToString() + ".chunk";
        FileStream stream = FileStreamExtensions.LoadAndWaitUntilLoaded(chunkFilePath, FileMode.Open); 
        BinaryReader reader = new BinaryReader(stream);

        // Create tile array
        int chunkSize = Constants.Terrain.CHUNK_SIZE;
        tiles = new Tile[chunkSize, chunkSize];
        
        // Load all tiles from disk
        for (int y = 0; y < chunkSize; y++)
            for (int x = 0; x < chunkSize; x++)
            {
                Vector2DInt tileLocalPosition = new Vector2DInt(x, y);
                Terrain tileTerrain = new Terrain((TerrainType)reader.ReadUInt16());
        
                tiles[x, y] = new Tile(tileLocalPosition, inPosition, tileTerrain);
            }

        MultiThreader.InvokeOnMain(() => OnDataDirtied?.Invoke(this));
    }
}

