using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Program : MonoBehaviour
{
    void Awake()
    {
        Directory.CreateDirectory(Constants.Terrain.CHUNK_SAVE_FOLDER);
        new WorldChunkManager();

        Player.creatureManager.SpawnCreatures();
    }

    void Update()
    {
        Player.ManualUpdate();
        MultiThreader.ManualUpdate();

        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector3     mouseWorldPosition  = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2DInt targetWorldPosition = new Vector2DInt((int)mouseWorldPosition.x, (int)mouseWorldPosition.z);

            Chunk targetChunk = WorldChunkManager.instance.GetTile(targetWorldPosition).chunk;

            targetChunk.data.PlaceFurniture(WorldChunkManager.WorldPosToLocalTilePos(targetWorldPosition), new Furniture(new Vector2DInt(2, 2), targetWorldPosition)); // TODO: Bug: MEMORY LEAK IF FURNITURE CANNOT BE PLACED. 
        }                                                                                                                                                              // Check whether or not furniture can be placed beforehand          
    }
}