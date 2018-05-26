using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Furniture 
{
    Vector2DInt _size;
    public Vector2DInt size => _size;

    Vector2DInt _position;
    public Vector2DInt position => _position;


    public Furniture(Vector2DInt inSize, Vector2DInt inWorldPosition)
    {
        _size = inSize;
        _position = inWorldPosition;
    }

    public Furniture(BinaryReader inReader)
    {
        BinaryLoad(inReader);
    }


    public int GetTextureIDFromWorldPosition(Vector2DInt inWorldPosition)
    {
        Vector2DInt targetPartPosition = new Vector2DInt() {
            x = inWorldPosition.x - _position.x,
            y = inWorldPosition.y - _position.y
        };

        if (!targetPartPosition.IsWithinZeroAnd(_size))
        {
            Debug.LogError("Tried getting part of furniture that is outside of the furniture");
            return 0;
        }

        return GetTextureIDFromPart(targetPartPosition);
    }

    int GetTextureIDFromPart(Vector2DInt inFurniturePart)
    {
        if (!inFurniturePart.IsWithinZeroAnd(_size))
        {
            Debug.LogError("Tried getting part of furniture that is outside of the furniture");
            return 0;
        }

        return 1; // TODO: Change this for a lookup against a static collection of exmple furniture or smth
    }


    public void BinarySave(BinaryWriter inWriter)
    {
        _size.BinarySave(inWriter);
        _position.BinarySave(inWriter);
    }

    public void BinaryLoad(BinaryReader inReader)
    {
        _size.BinaryLoad(inReader);
        _position.BinaryLoad(inReader);
    }
}


