using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player 
{
    public static readonly PlayerCreatureManager creatureManager = new PlayerCreatureManager();

    public static void ManualUpdate()
    {
        creatureManager.ManualUpdate();
    }
}

