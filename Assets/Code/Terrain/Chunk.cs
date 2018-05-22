using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;

public class Chunk
{
    public readonly ChunkData data;
    public readonly GameObject viewGO;

    public Chunk(ChunkData inData, GameObject inViewGO)
    {
        data = inData;
        viewGO = inViewGO;
    }


    public void Update() =>
        data.Update();
}

public class ChunkView
{
    // readonly GameObject terrainViewGO;
    // readonly GameObject furnitureViewGO;
    // 
    // public ChunkView(GameObject inTerrainViewGO, GameObject inFurnitureViewGO)
    // {
    //      terrainViewGO   = inTerrainViewGO;
    //      furnitureViewGO = inFurnitureViewGO;
    // }
}

enum DirtyFlags
{
    Terrain   = 1, 
    Furniture = 2,
    Test      = 4
}

public class ChunkData 
{
    public readonly Vector2DInt position;

    DirtyFlags _dirtyFlags;

    Tile[,] _tiles;


    public Tile GetTile(Vector2DInt inTileCoords) => _tiles[inTileCoords.x, inTileCoords.y];
    public Tile TGetTile(Vector2DInt inTileCoords)
    {
        while (_tiles == null)
            Thread.Sleep(5);

        return GetTile(inTileCoords);
    }


    public event Action<ChunkData> OnTerrainDataDirtied;
    public event Action<ChunkData> OnFurnitureDataDirtied;

    public ChunkData(Vector2DInt inPosition)
    {
        position = inPosition;  
    }

    public void Update()
    {
        if (_dirtyFlags.HasFlag(DirtyFlags.Terrain))
            OnTerrainDataDirtied?.Invoke(this);
        if (_dirtyFlags.HasFlag(DirtyFlags.Furniture))
            OnFurnitureDataDirtied?.Invoke(this);

        // TODO: Finish implementing this, and see if extension methods works
    }


    public void SetTiles(Tile[,] inTiles)
    {
        _tiles = inTiles;

        _dirtyFlags = DirtyFlags.Terrain | DirtyFlags.Furniture | DirtyFlags.Test;
    }

    public void PlaceFurniture(Vector2DInt inTileCoords, Furniture inFurniture)
    {
        Tile targetTile = _tiles[inTileCoords.x, inTileCoords.y];

        List<Tile> tilesToBeOccupied = new List<Tile>();
        for (int y = 0; y < inFurniture.size.y; y++)
            for (int x = 0; x < inFurniture.size.x; x++)
            {
                Tile nearbyTile = targetTile.GetRelativeTile(new Vector2DInt(x, y));

                if (nearbyTile.furniture != null) {
                    Debug.LogError("Cannot place furniture! Tile: " + nearbyTile.worldPosition + " already has a furniture.");
                    return;
                }

                tilesToBeOccupied.Add(nearbyTile);
            }

        foreach (Tile tileToBeOccupied in tilesToBeOccupied)
            tileToBeOccupied.SetFurniture(inFurniture);

        OnFurnitureDataDirtied?.Invoke(this);
    }


    public void BinarySave()
    {
        FileStream stream = File.OpenWrite(Constants.Terrain.CHUNK_SAVE_FOLDER + "\\" + position.ToString() + ".chunk");
        BinaryWriter writer = new BinaryWriter(stream);

        foreach (Tile tile in _tiles)
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
        _tiles = new Tile[chunkSize, chunkSize];
        
        // Load all tiles from disk
        for (int y = 0; y < chunkSize; y++)
            for (int x = 0; x < chunkSize; x++)
            {
                Vector2DInt tileLocalPosition = new Vector2DInt(x, y);
                Terrain tileTerrain = new Terrain((TerrainType)reader.ReadUInt16());
        
                _tiles[x, y] = new Tile(tileLocalPosition, inPosition, tileTerrain);
            }

        MultiThreader.InvokeOnMain(() => OnTerrainDataDirtied?.Invoke(this));
    }
}

