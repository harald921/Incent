﻿using System.Collections;
using System.Collections.Generic;

public struct Vector2DInt
{
    public int x, y;

    public static Vector2DInt Zero => new Vector2DInt(0, 0);


    public Vector2DInt(int inXandY)
    {
        x = inXandY;
        y = inXandY;
    }

    public Vector2DInt(int inX, int inY)
    {
        x = inX;
        y = inY;
    }


    public override string ToString() =>
        "(" + x + ", " + y + ")";

    public override bool Equals(object obj)
    {
        if (obj is Vector2DInt)
        {
            Vector2DInt objAsVector2DInt = (Vector2DInt)obj;
            if (x == objAsVector2DInt.x &&
                y == objAsVector2DInt.y)
                return true;
        }

        return false;
    }

    public static Vector2DInt operator +(Vector2DInt inVector1, Vector2DInt inVector2) =>
        new Vector2DInt()
        {
            x = inVector1.x + inVector2.x,
            y = inVector1.y + inVector2.y
        };

    public static Vector2DInt operator -(Vector2DInt inVector1, Vector2DInt inVector2) =>
        new Vector2DInt()
        {
            x = inVector1.x - inVector2.x,
            y = inVector1.y - inVector2.y
        };

    public static Vector2DInt operator *(Vector2DInt inVector1, int inInt) =>
        new Vector2DInt()
        {
            x = inVector1.x * inInt,
            y = inVector1.y * inInt
        };

    public static Vector2DInt operator *(Vector2DInt inVector1, Vector2DInt inVector2) =>
        new Vector2DInt()
        {
            x = inVector1.x * inVector2.x,
            y = inVector1.y * inVector2.y
        };

    public static Vector2DInt operator /(Vector2DInt inVector1, Vector2DInt inVector2) =>
        new Vector2DInt()
        {
            x = inVector1.x / inVector2.x,
            y = inVector1.y / inVector2.y
        };

    public static Vector2DInt operator /(Vector2DInt inVector1, int inInt) =>
        new Vector2DInt()
        {
            x = inVector1.x / inInt,
            y = inVector1.y / inInt
        };

    public static Vector2DInt operator %(Vector2DInt inVector1, int inInt) =>
        new Vector2DInt()
        {
            x = inVector1.x % inInt,
            y = inVector1.y % inInt
        };

    public static bool operator ==(Vector2DInt inVector1, Vector2DInt inVector2) =>
        inVector1.x == inVector2.x &&
        inVector1.y == inVector2.y;

    public static bool operator !=(Vector2DInt inVector1, Vector2DInt inVector2) =>
        inVector1.x != inVector2.x ||
        inVector1.y != inVector2.y;
}