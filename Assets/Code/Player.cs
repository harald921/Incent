using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player 
{
    List<Creature> _ownedCreatures = new List<Creature>();
    int _selectedCreatureID;
    

    public void AddCreature(Creature inCreatureToAdd)
    {
        Debug.Log("Adding creature to player");
        _ownedCreatures.Add(inCreatureToAdd);
    }


    public void ManualUpdate()
    {
        HandleInput();
    }

    public void HandleInput()
    {
        // This method is temporary. For debug purposes.

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2DInt targetWorldPosition = new Vector2DInt((int)mouseWorldPosition.x, (int)mouseWorldPosition.z);

            if (_ownedCreatures.Count > 0)
                _ownedCreatures[_selectedCreatureID].movementComponent.MoveTo(targetWorldPosition);
        }


        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_selectedCreatureID < _ownedCreatures.Count - 1)
                _selectedCreatureID++;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (_selectedCreatureID > 0)
                _selectedCreatureID--;
        }
    }
}

