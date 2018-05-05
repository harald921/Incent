using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// gCost = distanceFromStartNode
// hCost = distanceFromEndNode
// fCost = cost

public class CreatureMovementComponent 
{
    public Transform seeker, target;


    void FindPath(Tile inStartTile, Tile inTargetTile)
    {
        List<Node> checkedNodes   = new List<Node>();
        List<Node> uncheckedNodes = new List<Node>() {
            new Node(inStartTile)
        };


        while (uncheckedNodes.Count > 0)
        {
            // Find the unchecked node with the lowest totalCost and distanceFromEnd
            Node currentNode = uncheckedNodes[0];
            foreach (Node uncheckedNode in uncheckedNodes)                                 // Foreach unchecked node
                if (uncheckedNode.data.totalCost <= currentNode.data.totalCost)            // If its total cost is less or equal to the current tiles total cost
                    if (uncheckedNode.data.distanceToEnd < currentNode.data.distanceToEnd) // If its distance from the end is less than the current nodes distance from the end
                        currentNode = uncheckedNode;                                       // It's now the new current node


            // If the currentNode is the targetNode, the path has been found
            if (currentNode.tile == inTargetTile)
                return;

            // Foreach neighbour
            foreach (Tile neighbour in currentNode.tile.GetNeighbours())
            {
                // If the neighbour isn't passable or is already checked, skip it
                if (!neighbour.terrain.data.passable || checkedNodes.Contains(new Node(neighbour))) // TODO: No, doesn't even work 
                    continue;

                int distanceToNeighbour        = GetDistance(currentNode.tile, neighbour);               // Calculate if distance is 10 or 14
                int newNeighbourDistanceToStart = currentNode.data.distanceToStart + distanceToNeighbour; // Calculate how far away from the start node the neighbour is

                if (newNeighbourDistanceToStart < neighbour.distanceToStart)
                    if (!uncheckedNodes.Contains(neighbour))
                    {
                        neighbour.distanceToStart = newNeighbourDistanceToStart;
                        neighbour.distanceToEnd   = GetDistance(neighbour, inTargetTile);
                        neighbour.parent          = currentNode;

                        if (!uncheckedNodes.Contains(neighbour))
                            uncheckedNodes.Add(neighbour);
                    }
            }

            // Set current node as checked
            uncheckedNodes.Remove(currentNode); 
            checkedNodes.Add(currentNode);
        }
    }

    int GetDistance(Tile tileA, Tile tileB)
    {
        int distanceX = Mathf.Abs(tileA.worldPosition.x - tileB.worldPosition.x);
        int distanceY = Mathf.Abs(tileA.worldPosition.y - tileB.worldPosition.y);

        return (distanceX > distanceY) ? 14 * distanceY + 10 * (distanceX - distanceY) :
                                         14 * distanceX + 10 * (distanceY - distanceX);
    }


    Node FindNode(Vector2DInt inWorldPosition) => new Node(new Tile(Vector2DInt.Zero, Vector2DInt.Zero, new Terrain()));
}

class Node
{
    public Tile tile;
    public NodeData data;

    public Node(Tile inTile)
    {
        tile = inTile;
    }
}

struct NodeData
{
    public Node parent;
    public int totalCost;
    public int distanceToStart;
    public int distanceToEnd;
}


// Jag behöver göra så att varje tile har en "NodeData" på sig. Detta är för att jag måste kunna hämta "neighbouring nodes" så att nodes "distanceToStart" kan uppdateras.
// I need to add a "NodeData" to every Tile så that I can get "NeighbouringNodes" from a tile in order to be able to properly update a node's distanceToStart