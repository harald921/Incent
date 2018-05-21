using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture 
{
    public readonly Vector2DInt size;
    readonly Vector2DInt _position;


    public Furniture(Vector2DInt inSize, Vector2DInt inWorldPosition)
    {
        size = inSize;
        _position = inWorldPosition;
    }


    public int GetTextureIDFromWorldPosition(Vector2DInt inWorldPosition)
    {
        Vector2DInt requestedLocalPosition = new Vector2DInt() {
            x = inWorldPosition.x - _position.x,
            y = inWorldPosition.y - _position.y
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
}


