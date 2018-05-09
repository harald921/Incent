using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingDebug : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3     mouseWorldPosition              = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2DInt mouseWorldPositionAsVector2DInt = new Vector2DInt(Mathf.FloorToInt(mouseWorldPosition.x), Mathf.FloorToInt(mouseWorldPosition.z));

            Tile destination = WorldChunkManager.instance.GetTile(mouseWorldPositionAsVector2DInt);

            WorldChunkManager.debugCreature.movementComponent.MoveTo(destination.worldPosition);
        }
    }
}