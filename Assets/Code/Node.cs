public class Node
{
    public readonly Tile owner;
    public Tile parent;

    public int distanceToStart;
    public int distanceToEnd;

    public int totalCost => distanceToStart + distanceToEnd;


    public Node(Tile inOwner)
    {
        owner = inOwner;
    }
}