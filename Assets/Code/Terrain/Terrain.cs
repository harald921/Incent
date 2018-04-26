using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct Terrain
{
    static Dictionary<TerrainType, TerrainData> _staticTerrainData = new Dictionary<TerrainType, TerrainData>()
    {
        { TerrainType.Grass, new TerrainData(inTextureID: 2, inMoveSpeedModifier: 0.9f) },
        { TerrainType.Sand,  new TerrainData(inTextureID: 1, inMoveSpeedModifier: 0.8f) },
    };

    readonly TerrainType _type;
    public TerrainData data => _staticTerrainData[_type];


    public Terrain(TerrainType inType)
    {
        _type = inType;
    }
}

public class TerrainData
{
    public readonly int textureID;
    public readonly float moveSpeedModifier;

    public TerrainData(int inTextureID, float inMoveSpeedModifier)
    {
        textureID = inTextureID;
        moveSpeedModifier = inMoveSpeedModifier;
    }
}

public enum TerrainType
{
    Grass,
    Sand
}