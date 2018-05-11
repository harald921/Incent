using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Program : MonoBehaviour
{
    Player _player;

    void Awake()
    {
        Directory.CreateDirectory(Constants.Terrain.CHUNK_SAVE_FOLDER);
        new WorldChunkManager();

        _player = new Player();

        _player.AddCreature(new Creature(WorldChunkManager.instance.GetTile(new Vector2DInt(5, 5))));
        _player.AddCreature(new Creature(WorldChunkManager.instance.GetTile(new Vector2DInt(6, 5))));

    }

    void Update()
    {
        _player.ManualUpdate();
        MultiThreader.ManualUpdate();
    }
}