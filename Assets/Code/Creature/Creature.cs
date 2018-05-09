using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature 
{
    public readonly CreatureMovementComponent movementComponent;
    public readonly CreatureViewComponent viewComponent;


    public Creature(Tile inSpawnTile)
    {
        movementComponent = new CreatureMovementComponent(this, inSpawnTile);
        viewComponent     = new CreatureViewComponent(this, inSpawnTile.worldPosition);
    }
}