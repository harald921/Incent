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
        new Player();


        Player.creatureManager.AddCreature(new Creature(WorldChunkManager.instance.GetTile(new Vector2DInt(5, 5))));
        Player.creatureManager.AddCreature(new Creature(WorldChunkManager.instance.GetTile(new Vector2DInt(6, 5))));
    }

    void Update()
    {
        Player.ManualUpdate();
        MultiThreader.ManualUpdate();
    }
}