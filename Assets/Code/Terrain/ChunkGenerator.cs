using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class ChunkGenerator
{
    static int _chunkSize;

    readonly DataGenerator _dataGenerator;
    readonly ViewGenerator _viewGenerator;


    public ChunkGenerator()
    {
        _chunkSize = Constants.Terrain.CHUNK_SIZE; // TODO: Read from disk

        _dataGenerator = new DataGenerator();
        _viewGenerator = new ViewGenerator();
    }


    public void GenerateWorld()
    {
        for (int y = 0; y < Constants.Terrain.WORLD_SIZE; y++)
            for (int x = 0; x < Constants.Terrain.WORLD_SIZE; x++)
            {
                Vector2DInt chunkPosition = new Vector2DInt(x, y);
                _dataGenerator.Generate(chunkPosition);
            }
    }

    public Chunk LoadChunk(Vector2DInt inPosition)
    {
        Debug.Log("load");
        // Create chunkdata and begin load from disk on a separate thread
        ChunkData  chunkData = new ChunkData(inPosition);
        new System.Threading.Thread(() => chunkData.BinaryLoad(inPosition)).Start();

        // Create blank chunk view
        GameObject chunkView = _viewGenerator.GenerateBlank(inPosition);

        // Make the viewGenerator update the UV2 of the view every time the chunk's data changes
        chunkData.OnDataDirtied += (ChunkData dirtiedData) =>
            _viewGenerator.UpdateUV2(chunkView, chunkData);

        return new Chunk(chunkData, chunkView);
    }

    public void UnloadChunk(Chunk inChunk)
    {
        Object.Destroy(inChunk.viewGO);
    }

    class DataGenerator
    {
        readonly Noise.Parameters[] _noiseParamters;


        // Constructor
        public DataGenerator()
        {
            _noiseParamters = LoadNoiseParameters();
        }


        public void Generate(Vector2DInt inPosition)
        {
            ChunkData newChunkData = new ChunkData(inPosition);

            newChunkData.SetTiles(GenerateTiles(inPosition, GenerateNoiseMap(inPosition)));

            newChunkData.BinarySave();
        }

        Noise.Parameters[] LoadNoiseParameters()
        {
            // TODO: Read form disk
            return new Noise.Parameters[]
            {
                new Noise.Parameters()
                {
                    scale       = 50,
                    octaves     = 7,
                    persistance = 1.01f,
                    lacunarity  = 1.01f,
                    seed        = 0
                }
            };
        }


        float[,] GenerateNoiseMap(Vector2DInt inOffset) => 
            Noise.Generate(_chunkSize, _noiseParamters[0], inOffset);

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
            _vertexSize  = _chunkSize  * 2;
            _vertexCount = _vertexSize * _vertexSize * 4;

            _vertices  = GenerateVertices();
            _triangles = GenerateTriangleIDs();
        }


        public GameObject GenerateBlank(Vector2DInt inPosition)
        {
            GameObject viewGO = CreateGO(inPosition);

            ApplyMesh(viewGO.GetComponent<MeshFilter>());

            return viewGO;
        }


        public GameObject CreateGO(Vector2DInt inPosition)
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

        public Vector2[] GenerateUV2(ChunkData inChunkData)
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


        public void ApplyMesh(MeshFilter inFilter)
        {
            inFilter.mesh.vertices  = _vertices;
            inFilter.mesh.triangles = _triangles;
        }

        public void UpdateUV2(GameObject inView, ChunkData inChunkData) =>
            inView.GetComponent<MeshFilter>().mesh.uv2 = GenerateUV2(inChunkData);
    }
}