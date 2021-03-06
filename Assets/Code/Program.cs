﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Program : MonoBehaviour
{
    WorldChunkManager _worldChunkManager;

    void Awake()
    {
        Directory.CreateDirectory(Constants.Terrain.CHUNK_SAVE_FOLDER);
        _worldChunkManager = new WorldChunkManager();

        Player.creatureManager.SpawnCreatures();
    }

    void Update()
    {
        _worldChunkManager.Update();

        Player.ManualUpdate();
        MultiThreader.ManualUpdate();

        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector3     mouseWorldPosition  = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2DInt targetWorldPosition = new Vector2DInt((int)mouseWorldPosition.x, (int)mouseWorldPosition.z);

            WorldChunkManager.instance.PlaceFurniture(targetWorldPosition, new Furniture(new Vector2DInt(2, 2), targetWorldPosition)); 
        }
    }
}

