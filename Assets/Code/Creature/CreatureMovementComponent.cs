using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class CreatureMovementComponent 
{
    readonly Creature _creature;

    Tile _currentTile;

    public event Action<Tile> OnTileEnter;

    CoroutineHandle _followHandle;


    public CreatureMovementComponent(Creature inCreature, Tile inSpawnTile)
    {
        _creature = inCreature;
        _currentTile = inSpawnTile;
    }

    
    public void MoveTo(Vector2DInt inTargetPosition)
    {
        Tile targetTile = WorldChunkManager.instance.GetTile(inTargetPosition);

        List<Tile> path = Pathfinder.FindPath(_currentTile, targetTile);

        if (path.Count > 0)
            _followHandle = Timing.RunCoroutineSingleton(Follow(path), _followHandle, SingletonBehavior.Overwrite);
        else
            Debug.LogError("Couldn't find path to target!");
    }

    IEnumerator<float> Follow(List<Tile> inPath)
    {
        foreach (Tile tile in inPath)
        {
            _currentTile = tile;
            OnTileEnter?.Invoke(tile);
            yield return Timing.WaitForSeconds(0.1f);
        }    
    }
}



