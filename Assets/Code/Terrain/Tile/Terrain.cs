using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct Terrain
{
    static Dictionary<TerrainType, TerrainData> _staticTerrainData = new Dictionary<TerrainType, TerrainData>()
    {
        { TerrainType.Grass, new TerrainData(inTextureID: 2, inMoveSpeedModifier: 1.0f, inPassable: true)  },
        { TerrainType.Sand,  new TerrainData(inTextureID: 1, inMoveSpeedModifier: 0.5f, inPassable: true) },
    };

    public readonly TerrainType type;
    public TerrainData data => _staticTerrainData[type];


    public Terrain(TerrainType inType)
    {
        type = inType;
    }
}

public class TerrainData
{
    public readonly int   textureID;
    public readonly float moveSpeedModifier;
    public readonly bool  passable; // TODO: Replace this with some kinda bit flag

    public TerrainData(int inTextureID, float inMoveSpeedModifier, bool inPassable)
    {
        textureID         = inTextureID;
        moveSpeedModifier = inMoveSpeedModifier;
        passable          = inPassable;
    }
}

public enum TerrainType
{
    Grass,
    Sand
}