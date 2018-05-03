using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Noise
{
    public static float[,] Generate(int inSize, Parameters inParameters, Vector2DInt inOffset)
    {
        // TODO: Find a perlin noise lib and use it

        System.Random rng = new System.Random(inParameters.seed);

        float[,] noiseMap = new float[inSize, inSize];

        for (int y = 0; y < inSize; y++)
            for (int x = 0; x < inSize; x++)
                noiseMap[x, y] = rng.Next(0, 2);

        return noiseMap;
    }

    public struct Parameters
    {
        public int scale;
        public int octaves;
        public float persistance;
        public float lacunarity;
        public int seed;
    }
}