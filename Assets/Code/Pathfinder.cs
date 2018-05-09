using System.Collections;
using System.Collections.Generic;

public static class Pathfinder 
{
    public static List<Tile> FindPath(Tile inStart, Tile inDestination)
    {
        List<Tile> closedTiles = new List<Tile>();
        List<Tile> openTiles = new List<Tile>() {
            inStart
        };

        while (openTiles.Count > 0)
        {
            Tile currentTile = GetTileWithLowestCost(openTiles);

            // If the current tile is the target tile, the path is completed
            if (currentTile == inDestination)
                return RetracePath(inStart, inDestination);

            // Add all walkable neighbours to "openTiles"
            List<Tile> neighbours = currentTile.GetNeighbours();
            foreach (Tile neighbour in neighbours)
            {
                if (!neighbour.terrain.data.passable)
                    continue;

                if (closedTiles.Contains(neighbour))
                    continue;

                // Calculate the neighbours distance from start
                int newNeighbourDistanceToStart = currentTile.node.distanceToStart + currentTile.DistanceTo(neighbour);

                // If open tiles contains the neighbour and the new distance is longer than the existing, skip
                if (openTiles.Contains(neighbour))
                    if (newNeighbourDistanceToStart > neighbour.node.distanceToStart)
                        continue;

                // Since this is either a newly discovered tile or a tile with now better score, set all the node data and update the parent
                neighbour.node.distanceToStart = newNeighbourDistanceToStart;
                neighbour.node.distanceToEnd = neighbour.DistanceTo(inDestination);
                neighbour.node.parent = currentTile;

                // If it's newly discovered, add it as an open tile
                if (!openTiles.Contains(neighbour))
                    openTiles.Add(neighbour);
            }

            // This tile is now closed...
            closedTiles.Add(currentTile);
            openTiles.Remove(currentTile);
        }

        // If this is reached, no path was found. Return an empty list.
        return new List<Tile>();
    }


    static Tile GetTileWithLowestCost(List<Tile> inNodeList)
    {
        Tile currentTile = inNodeList[0];

        foreach (Tile openTile in inNodeList)
            if (openTile.node.totalCost <= currentTile.node.totalCost)
            {
                // If the open tile has the same total cost but is further away from the end, skip it
                if (openTile.node.totalCost == currentTile.node.totalCost)
                    if (openTile.node.distanceToEnd > currentTile.node.distanceToEnd)
                        continue;

                currentTile = openTile;
            }

        return currentTile;
    }


    static List<Tile> RetracePath(Tile inStartTile, Tile inTargetTile)
    {
        List<Tile> path = new List<Tile>();

        Tile currentTile = inTargetTile;
        while (currentTile != inStartTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.node.parent;
        }

        path.Reverse();

        return path;
    }
}