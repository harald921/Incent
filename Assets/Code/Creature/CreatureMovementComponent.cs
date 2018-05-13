using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class CreatureMovementComponent 
{
    readonly Creature _creature;

    public Tile currentTile { get; private set; }

    CoroutineHandle _followHandle;

    public event Action<Tile> OnTileEnter;
    public event Action<Chunk> OnChunkEnter;


    public CreatureMovementComponent(Creature inCreature, Tile inSpawnTile)
    {
        _creature = inCreature;
        currentTile = inSpawnTile;
    }

    
    public void MoveTo(Vector2DInt inTargetPosition)
    {
        Tile targetTile = WorldChunkManager.instance.GetTile(inTargetPosition);

        List<Tile> path = Pathfinder.FindPath(currentTile, targetTile);

        if (path.Count > 0)
            _followHandle = Timing.RunCoroutineSingleton(FollowPath(path), _followHandle, SingletonBehavior.Overwrite);
        else
            Debug.LogError("Couldn't find path to target!");
    }

    IEnumerator<float> FollowPath(List<Tile> inPath)
    {
        foreach (Tile tile in inPath)
        {
            Tile previousTile = currentTile;
            currentTile = tile;


            if (previousTile.chunk != tile.chunk)
                OnChunkEnter?.Invoke(tile.chunk);

            OnTileEnter?.Invoke(tile);

            yield return Timing.WaitForSeconds(0.03f);
        }    
    }
}



