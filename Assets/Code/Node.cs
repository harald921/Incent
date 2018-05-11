public class Node
{
    public readonly Tile owner;
    public Tile parent;

    public int costToStart;
    public int distanceToEnd;

    public int totalCost => costToStart + distanceToEnd;


    public Node(Tile inOwner)
    {
        owner = inOwner;
    }
}