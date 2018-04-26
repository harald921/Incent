using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainGenerator
{
    public static Terrain GetTerrain(float inHeight)
    {
        TerrainType resultingType;

        // TODO: Implement this properly
        if (inHeight == 0) resultingType = TerrainType.Grass;
        else resultingType = TerrainType.Sand;

        return new Terrain(resultingType);
    }
}