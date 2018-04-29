using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChunkGenerator
{
    static int _chunkSize;

    readonly DataGenerator _dataGenerator;
    readonly ViewGenerator _viewGenerator;


    public ChunkGenerator(Noise.Parameters[] inNoiseParameters)
    {
        _chunkSize = 64; // TODO: Read from file

        _dataGenerator = new DataGenerator(inNoiseParameters);
        _viewGenerator = new ViewGenerator();
    }


    public Chunk GenerateChunk(Vector2DInt inPosition)
    {
        ChunkData  chunkData = _dataGenerator.Generate(inPosition);
        GameObject chunkView = _viewGenerator.Generate(inPosition, chunkData);

        Chunk newChunk = new Chunk(inPosition, chunkData, chunkView);

        return newChunk;
    }


    class DataGenerator
    {
        readonly Noise.Parameters[] _noiseParamters;


        // Constructor
        public DataGenerator(Noise.Parameters[] inNoiseParameters)
        {
            _noiseParamters = inNoiseParameters;
        }


        public ChunkData Generate(Vector2DInt inPosition)
        {
            ChunkData newChunkData = new ChunkData();

            newChunkData.SetTiles(GenerateTiles(inPosition, GenerateNoiseMap(inPosition)));

            return newChunkData;
        }

        float[,] GenerateNoiseMap(Vector2DInt inOffset) => 
            Noise.Generate((uint)_chunkSize, _noiseParamters[0], inOffset);

        Tile[,] GenerateTiles(Vector2DInt inChunkPos, float[,] inNoiseData)
        {
            Tile[,] newTiles = new Tile[_chunkSize, _chunkSize];

            for (int y = 0; y < _chunkSize; y++)
                for (int x = 0; x < _chunkSize; x++)
                    newTiles[x, y] = new Tile(new Vector2DInt(x, y), inChunkPos, TerrainGenerator.GetTerrain(inNoiseData[x, y]));

            return newTiles;
        }
    }


    class ViewGenerator
    {
        readonly Material _chunkMaterial;

        readonly int _vertexSize;
        readonly int _vertexCount;

        readonly int[] _triangles;
        readonly Vector3[] _vertices;


        public ViewGenerator()
        {
            _chunkMaterial = (Material)Resources.Load("Material_Chunk", typeof(Material));

            // Calculate sizes and counts
            _vertexSize = _chunkSize * 2;
            _vertexCount = _vertexSize * _vertexSize * 4;

            // Generate vertices
            _vertices = GenerateVertices();

            // Generate triangle ID's
            _triangles = GenerateTriangleIDs();
        }

        public GameObject Generate(Vector2DInt inPosition, ChunkData inChunkData)
        {
            GameObject viewGO = GenerateGO(inPosition);

            ApplyMesh(viewGO.GetComponent<MeshFilter>(), GenerateUV2(inChunkData));

            return viewGO;
        }

        GameObject GenerateGO(Vector2DInt inPosition)
        {
            GameObject newChunkGO = new GameObject("Chunk");

            newChunkGO.AddComponent<MeshFilter>();

            MeshRenderer meshRenderer = newChunkGO.AddComponent<MeshRenderer>();

            meshRenderer.material = _chunkMaterial;

            newChunkGO.transform.position = new Vector3(inPosition.x, 0, inPosition.y) * _chunkSize;

            return newChunkGO;
        }

        int[] GenerateTriangleIDs()
        {
            int[] triangles = new int[_chunkSize * _chunkSize * 6];
            int currentQuad = 0;
            for (int y = 0; y < _vertexSize; y += 2)
                for (int x = 0; x < _vertexSize; x += 2)
                {
                    int triangleOffset = currentQuad * 6;
                    int currentVertex = y * _vertexSize + x;

                    triangles[triangleOffset + 0] = currentVertex + 0;                 // Bottom - Left
                    triangles[triangleOffset + 1] = currentVertex + _vertexSize + 1;   // Top    - Right
                    triangles[triangleOffset + 2] = currentVertex + 1;                 // Bottom - Right

                    triangles[triangleOffset + 3] = currentVertex + 0;                 // Bottom - Left
                    triangles[triangleOffset + 4] = currentVertex + _vertexSize + 0;   // Top    - Left
                    triangles[triangleOffset + 5] = currentVertex + _vertexSize + 1;   // Top    - Right

                    currentQuad++;
                }

            return triangles;
        }

        Vector3[] GenerateVertices()
        {
            Vector3[] vertices = new Vector3[_vertexCount];
            int vertexID = 0;
            for (int y = 0; y < _chunkSize; y++)
            {
                for (int x = 0; x < _chunkSize; x++)
                {
                    // Generate a quad 
                    vertices[vertexID + 0].x = x;
                    vertices[vertexID + 0].z = y;

                    vertices[vertexID + 1].x = x + 1;
                    vertices[vertexID + 1].z = y;

                    vertices[vertexID + _vertexSize + 0].x = x;
                    vertices[vertexID + _vertexSize + 0].z = y + 1;

                    vertices[vertexID + _vertexSize + 1].x = x + 1;
                    vertices[vertexID + _vertexSize + 1].z = y + 1;

                    vertexID += 2;
                }
                vertexID += _vertexSize;
            }

            return vertices;
        }

        Vector2[] GenerateUV2(ChunkData inChunkData)
        {
            Vector2[] newUV2s = new Vector2[_vertexCount];
            int vertexID = 0;
            for (int y = 0; y < _chunkSize; y++)
            {
                for (int x = 0; x < _chunkSize; x++)
                {

                    int tileTextureID = inChunkData.GetTile(new Vector2DInt(x, y)).terrain.data.textureID;
                    
                    newUV2s[vertexID + 0] = new Vector2(tileTextureID, tileTextureID);
                    newUV2s[vertexID + 1] = new Vector2(tileTextureID, tileTextureID);
                    newUV2s[vertexID + _vertexSize + 0] = new Vector2(tileTextureID, tileTextureID);
                    newUV2s[vertexID + _vertexSize + 1] = new Vector2(tileTextureID, tileTextureID);
                    
                    vertexID += 2;
                }
                vertexID += _vertexSize;
            }

            return newUV2s;
        }


        void ApplyMesh(MeshFilter inFilter, Vector2[] inUV2)
        {
            inFilter.mesh.vertices  = _vertices;
            inFilter.mesh.triangles = _triangles;
            inFilter.mesh.uv2 = inUV2;
        }
    }
}