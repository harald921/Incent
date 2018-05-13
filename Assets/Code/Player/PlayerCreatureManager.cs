using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCreatureManager
{
    Dictionary<Creature, List<Vector2DInt>> _chunkPositionsVisibleToCreatures = new Dictionary<Creature, List<Vector2DInt>>();
    public Creature[] ownedCreatures => _chunkPositionsVisibleToCreatures.Keys.ToArray();
    HashSet<Vector2DInt> visibleChunkPositions
    {
        get
        {
            HashSet<Vector2DInt> visibleChunkPositions = new HashSet<Vector2DInt>();
            foreach (KeyValuePair<Creature, List<Vector2DInt>> item in _chunkPositionsVisibleToCreatures)
                visibleChunkPositions.UnionWith(item.Value);
            return visibleChunkPositions;
        }
    }

    int _selectedCreatureID;

    public event Action<List<Vector2DInt>> OnChunkPositionsVisibilityLost;
    public event Action<List<Vector2DInt>> OnChunkPositionsVisibilityGained;


    public void ManualUpdate()
    {
        HandleInput();
    }

    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2DInt targetWorldPosition = new Vector2DInt((int)mouseWorldPosition.x, (int)mouseWorldPosition.z);

            if (ownedCreatures.Length> 0)
                ownedCreatures[_selectedCreatureID].movementComponent.MoveTo(targetWorldPosition);
        }


        if (Input.GetKeyDown(KeyCode.E))
            if (_selectedCreatureID < ownedCreatures.Length - 1)
                _selectedCreatureID++;

        if (Input.GetKeyDown(KeyCode.Q))
            if (_selectedCreatureID > 0)
                _selectedCreatureID--;
    }


    public void AddCreature(Creature inCreatureToAdd)
    {
        Debug.Log("Adding creature to player");
        _chunkPositionsVisibleToCreatures.Add(inCreatureToAdd, new List<Vector2DInt>());

        // Make the "_chunkPositionsVisibleToCreatures" get updated every time the creature enters a chunk
        inCreatureToAdd.movementComponent.OnChunkEnter += (_) => 
            SetVisibleChunkPositions(inCreatureToAdd);

        SetVisibleChunkPositions(inCreatureToAdd);
    }

    public void SpawnCreatures()
    {
        Chunk spawnChunk = WorldChunkManager.instance.GetSpawnChunk();

        MultiThreader.DoThreaded(() => spawnChunk.data.TGetTile(new Vector2DInt(5, 5)), (Tile inTile) => Player.creatureManager.AddCreature(new Creature(inTile)));
        MultiThreader.DoThreaded(() => spawnChunk.data.TGetTile(new Vector2DInt(6, 5)), (Tile inTile) => Player.creatureManager.AddCreature(new Creature(inTile)));
    }

    void SetVisibleChunkPositions(Creature inCreature)
    {
        List<Vector2DInt> newVisibleChunkPositions = CalculateVisibleChunksPositions(inCreature.movementComponent.currentTile.chunk.data.position);
        List<Vector2DInt> oldVisibleChunkPositions = _chunkPositionsVisibleToCreatures[inCreature];

        _chunkPositionsVisibleToCreatures[inCreature] = newVisibleChunkPositions;
        OnChunkPositionsVisibilityGained?.Invoke(newVisibleChunkPositions);

        InvokeLostChunkVisionCallbacks(oldVisibleChunkPositions, newVisibleChunkPositions);
    }

    void InvokeLostChunkVisionCallbacks(List<Vector2DInt> inOldVisibleChunkPositions, List<Vector2DInt> inNewVisibleChunkPositions)
    {
        // Calculate which chunks were lost in the chunk change
        List<Vector2DInt> lostVisibleChunkPositions = new List<Vector2DInt>();
        foreach (Vector2DInt oldVisibleChunkPosition in inOldVisibleChunkPositions)
            if (!inNewVisibleChunkPositions.Contains(oldVisibleChunkPosition))
                if (!visibleChunkPositions.Contains(oldVisibleChunkPosition)) 
                    lostVisibleChunkPositions.Add(oldVisibleChunkPosition);

        if (lostVisibleChunkPositions.Count > 0)
            OnChunkPositionsVisibilityLost?.Invoke(lostVisibleChunkPositions);
    }

    List<Vector2DInt> CalculateVisibleChunksPositions(Vector2DInt inViewOrigin)
    {
        // Calculate the visible area
        int renderDistance = Constants.Terrain.RENDER_DISTANCE;
        int chunksToSide = (renderDistance - 1) / 2;

        List<Vector2DInt> visibleChunks = new List<Vector2DInt>();
        for (int y = 0; y < renderDistance; y++)
            for (int x = 0; x < renderDistance; x++)
            {
                Vector2DInt possibleChunksPositions = new Vector2DInt(x - chunksToSide + inViewOrigin.x,
                                                                      y - chunksToSide + inViewOrigin.y);

                if (WorldChunkManager.ChunkPositionIsWithinWorld(possibleChunksPositions))
                    visibleChunks.Add(possibleChunksPositions);
            }

        return visibleChunks;
    }
}

