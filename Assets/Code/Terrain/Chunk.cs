using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;

public class Chunk
{
    public readonly ChunkData data;
    public readonly ChunkView view;

    public Chunk(ChunkData inData, ChunkView inView)
    {
        data = inData;
        view = inView;
    }


    public void Update() =>
        data.Update();
}

public class ChunkView
{
    readonly GameObject terrainViewGO;
    readonly GameObject furnitureViewGO;
    
    public ChunkView(GameObject inTerrainViewGO, GameObject inFurnitureViewGO)
    {
         terrainViewGO   = inTerrainViewGO;
         furnitureViewGO = inFurnitureViewGO;
    }

    public void Destroy()
    {
        UnityEngine.Object.Destroy(terrainViewGO);
        UnityEngine.Object.Destroy(furnitureViewGO);
    }
}

public class ChunkData 
{
    public readonly Vector2DInt position;

    ChunkDirtyFlags _dirtyFlags;

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
        if (_dirtyFlags.HasFlag(ChunkDirtyFlags.Terrain))
        {
            OnTerrainDataDirtied?.Invoke(this);
            _dirtyFlags.Remove(ChunkDirtyFlags.Terrain, ref _dirtyFlags);
        }

        if (_dirtyFlags.HasFlag(ChunkDirtyFlags.Furniture))
        {
            OnFurnitureDataDirtied?.Invoke(this);
            _dirtyFlags.Remove(ChunkDirtyFlags.Furniture, ref _dirtyFlags);
        }
    }


    public void SetTiles(Tile[,] inTiles)
    {
        _tiles = inTiles;

        _dirtyFlags = ChunkDirtyFlags.Terrain | ChunkDirtyFlags.Furniture;
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
        {
            tileToBeOccupied.SetFurniture(inFurniture);
            tileToBeOccupied.chunk.data._dirtyFlags.Add(ChunkDirtyFlags.Furniture, ref tileToBeOccupied.chunk.data._dirtyFlags);
        }
    }


    public void WriteToDisk()
    {
        FileStream stream = File.OpenWrite(Constants.Terrain.CHUNK_SAVE_FOLDER + "\\" + position.ToString() + ".chunk");
        BinaryWriter writer = new BinaryWriter(stream);

        HashSet<Furniture> furnituresToSave = new HashSet<Furniture>();

        foreach (Tile tile in _tiles)
        {
            writer.Write((UInt16)tile.terrain.type);                    // Write: Terrain type

            if (tile.furniture != null)
                furnituresToSave.Add(tile.furniture);
        }

        writer.Write(furnituresToSave.Count);                           // Write: Furniture count
        foreach (Furniture furnitureToSave in furnituresToSave)
            furnitureToSave.BinarySave(writer);                         // Write: Furniture
    }

    public void LoadFromDisk(Vector2DInt inPosition)
    {
        // Open chunk save file
        string chunkFilePath = Constants.Terrain.CHUNK_SAVE_FOLDER + "\\" + inPosition.ToString() + ".chunk";
        FileStream stream = FileStreamExtensions.LoadAndWaitUntilLoaded(chunkFilePath, FileMode.Open); 
        BinaryReader reader = new BinaryReader(stream);

        // Create tile array
        int chunkSize = Constants.Terrain.CHUNK_SIZE;
        SetTiles(new Tile[chunkSize, chunkSize]);
        
        // Load all tiles from disk
        for (int y = 0; y < chunkSize; y++)
            for (int x = 0; x < chunkSize; x++)
            {
                Vector2DInt tileLocalPosition = new Vector2DInt(x, y);
                Terrain tileTerrain = new Terrain((TerrainType)reader.ReadUInt16()); // Read: Terrain type
        
                _tiles[x, y] = new Tile(tileLocalPosition, inPosition, tileTerrain);
            }

        int numFurnitures = reader.ReadInt32();                                      // Read: Furniture count
        for (int i = 0; i < numFurnitures; i++)
        {
            Furniture loadedFurniture = new Furniture(reader);                       // Read: Furniture
            PlaceFurniture(loadedFurniture.position, loadedFurniture); // TODO: Make a check that it's actually possible to place a furniture here, just in case
        }
        
        MultiThreader.InvokeOnMain(() => OnTerrainDataDirtied?.Invoke(this));
    }
}

public enum ChunkDirtyFlags
{
    Terrain = 1,
    Furniture = 2,
}

