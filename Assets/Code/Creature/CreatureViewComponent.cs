using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureViewComponent 
{
    readonly Creature _creature;
    readonly GameObject _viewGO;


    public CreatureViewComponent(Creature inCreature, Vector2DInt inSpawnPosition)
    {
        _creature = inCreature;

        _viewGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _viewGO.transform.position = new Vector3(inSpawnPosition.x, 0, inSpawnPosition.y) + (Vector3.one * 0.5f);

        _creature.movementComponent.OnTileEnter += (Tile newTile) =>
            _viewGO.transform.position = new Vector3(newTile.worldPosition.x, 0, newTile.worldPosition.y) + (Vector3.one * 0.5f);
    }
}