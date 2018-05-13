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
    }
}