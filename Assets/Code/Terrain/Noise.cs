using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Noise
{
    public static float[,] Generate(uint inSize, Parameters inParameters, Vector2DInt inOffset)
    {
        // TODO: Find a perlin noise lib and use it

        System.Random rng = new System.Random((int)inParameters.seed);

        float[,] noiseMap = new float[inSize, inSize];

        for (int y = 0; y < inSize; y++)
            for (int x = 0; x < inSize; x++)
                noiseMap[x, y] = rng.Next(0, 2);

        return noiseMap;
    }

    public struct Parameters
    {
        public uint scale;
        public uint octaves;
        public float persistance;
        public float lacunarity;
        public uint seed;
    }
}