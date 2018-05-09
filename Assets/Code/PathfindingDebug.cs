using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingDebug : MonoBehaviour
{
    Tile tileA;
    Tile tileB;

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Vector3     mouseWorldPosition              = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2DInt mouseWorldPositionAsVector2DInt = new Vector2DInt(Mathf.FloorToInt(mouseWorldPosition.x), Mathf.FloorToInt(mouseWorldPosition.z));

            tileA = WorldChunkManager.instance.GetTile(mouseWorldPositionAsVector2DInt);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3     mouseWorldPosition              = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2DInt mouseWorldPositionAsVector2DInt = new Vector2DInt(Mathf.FloorToInt(mouseWorldPosition.x), Mathf.FloorToInt(mouseWorldPosition.z));

            tileB = WorldChunkManager.instance.GetTile(mouseWorldPositionAsVector2DInt);


            Debug.Log(Pathfinder.FindPath(tileA, tileB).Count);
        }
    }
}