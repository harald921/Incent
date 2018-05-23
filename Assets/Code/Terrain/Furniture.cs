using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Furniture : IBinarySerializable
{
    public readonly Vector2DInt size;
    public readonly Vector2DInt position;


    public Furniture(Vector2DInt inSize, Vector2DInt inWorldPosition)
    {
        size = inSize;
        position = inWorldPosition;
    }

    public Furniture(BinaryReader inReader)
    {
        BinaryLoad(inReader);
    }

    public int GetTextureIDFromWorldPosition(Vector2DInt inWorldPosition)
    {
        Vector2DInt requestedLocalPosition = new Vector2DInt() {
            x = inWorldPosition.x - position.x,
            y = inWorldPosition.y - position.y
        };


        if (!requestedLocalPosition.IsWithinZeroAnd(size))
        {
            Debug.LogError("Tried getting part of furniture that is outside of the furniture");
            return 0;
        }

        return GetTextureIDFromPart(requestedLocalPosition);
    }

    int GetTextureIDFromPart(Vector2DInt inFurniturePart)
    {
        if (!inFurniturePart.IsWithinZeroAnd(size))
        {
            Debug.LogError("Tried getting part of furniture that is outside of the furniture");
            return 0;
        }

        return 1; // TODO: Change this for a lookup against a static collection of exmple furniture or smth
    }


    public void BinarySave(BinaryWriter inWriter)
    {
        size.BinarySave(inWriter);
        position.BinarySave(inWriter);
    }

    public void BinaryLoad(BinaryReader inReader)
    {
        size.BinaryLoad(inReader);
        position.BinaryLoad(inReader);
    }
}


public interface IBinarySerializable
{
    void BinarySave(BinaryWriter inWriter);

    void BinaryLoad(BinaryReader inReader);
}