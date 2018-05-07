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


            Debug.Log(FindPath(tileA, tileB).Count);
        }

        
    }

    List<Tile> FindPath(Tile inStartTile, Tile inTargetTile)
    {
        List<Tile> closedTiles = new List<Tile>();
        List<Tile> openTiles = new List<Tile>() {
            inStartTile
        };

        while (openTiles.Count > 0)
        {
            Tile currentTile = openTiles[0];

            // Find the tile that has the lowest total cost, and lowest distance to end
            foreach (Tile openTile in openTiles)
                if (openTile.node.totalCost <= currentTile.node.totalCost)
                {
                    // If the open tile has the same total cost but is further away from the end, skip it
                    if (openTile.node.totalCost == currentTile.node.totalCost)
                        if (openTile.node.distanceToEnd > currentTile.node.distanceToEnd)
                            continue;

                    currentTile = openTile;
                }

            // If the current tile is the target tile, the path is completed
            if (currentTile == inTargetTile)
                return RetracePath(inStartTile, inTargetTile);

            // Add all walkable neighbours to "openTiles"
            List<Tile> neighbours = currentTile.GetNeighbours();
            foreach (Tile neighbour in neighbours)
            {
                if (!neighbour.terrain.data.passable)
                    continue;

                if (closedTiles.Contains(neighbour))
                    continue;

                // Calculate the neighbours distance from start
                int newNeighbourDistanceToStart = currentTile.node.distanceToStart + GetDistance(currentTile, neighbour);

                // If open tiles contains the neighbour and the new distance is longer than the existing, skip
                if (openTiles.Contains(neighbour))
                    if (newNeighbourDistanceToStart > neighbour.node.distanceToStart)
                        continue;

                // Since this is either a newly discovered tile or a tile with now better score, set all the node data and update the parent
                neighbour.node.distanceToStart = newNeighbourDistanceToStart;
                neighbour.node.distanceToEnd = GetDistance(neighbour, inTargetTile);
                neighbour.node.parent = currentTile;

                // If it's newly discovered, add it as an open tile
                if (!openTiles.Contains(neighbour))
                {
                    openTiles.Add(neighbour);

                    GameObject openGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    openGO.GetComponent<MeshRenderer>().material.color = Color.green;
                    openGO.transform.position = new Vector3(neighbour.worldPosition.x, 0.2f, neighbour.worldPosition.y) + (Vector3.one * 0.5f);
                }
            }

            GameObject closedGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            closedGO.GetComponent<MeshRenderer>().material.color = Color.red;
            closedGO.transform.position = new Vector3(currentTile.worldPosition.x, 0.5f, currentTile.worldPosition.y) + (Vector3.one * 0.5f);

            // This tile is now closed...
            closedTiles.Add(currentTile);
            openTiles.Remove(currentTile);
        }

        // If this is reached, no path was found. Return an empty list.
        return new List<Tile>();
    }

    List<Tile> RetracePath(Tile inStartTile, Tile inTargetTile)
    {
        List<Tile> path = new List<Tile>();

        Tile currentTile = inTargetTile;
        while (currentTile != inStartTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.node.parent;

            GameObject openGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            openGO.GetComponent<MeshRenderer>().material.color = Color.yellow;
            openGO.transform.position = new Vector3(currentTile.worldPosition.x, 0.6f, currentTile.worldPosition.y) + (Vector3.one * 0.5f);
        }

        path.Reverse();

        return path;
    }

    int GetDistance(Tile inTileA, Tile inTileB)
    {
        int distanceX = Mathf.Abs(inTileA.worldPosition.x - inTileB.worldPosition.x);
        int distanceY = Mathf.Abs(inTileA.worldPosition.y - inTileB.worldPosition.y);

        return (distanceX > distanceY) ? 14 * distanceY + 10 * (distanceX - distanceY) :
                                         14 * distanceX + 10 * (distanceY - distanceX);
    }
}