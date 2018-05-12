using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCreatureManager
{
    Dictionary<Creature, List<Vector2DInt>> _chunkPositionsVisibleToCreatures = new Dictionary<Creature, List<Vector2DInt>>();
    Creature[] _ownedCreatures => _chunkPositionsVisibleToCreatures.Keys.ToArray();
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

            if (_ownedCreatures.Length> 0)
                _ownedCreatures[_selectedCreatureID].movementComponent.MoveTo(targetWorldPosition);
        }


        if (Input.GetKeyDown(KeyCode.E))
            if (_selectedCreatureID < _ownedCreatures.Length - 1)
                _selectedCreatureID++;

        if (Input.GetKeyDown(KeyCode.Q))
            if (_selectedCreatureID > 0)
                _selectedCreatureID--;
    }


    public void AddCreature(Creature inCreatureToAdd)
    {
        Debug.Log("Adding creature to player");
        List<Vector2DInt> chunksVisibleToNewCreature = CalculateVisibleChunksPositions(inCreatureToAdd.movementComponent.currentTile.chunkPosition);

        _chunkPositionsVisibleToCreatures.Add(inCreatureToAdd, chunksVisibleToNewCreature);

        // Make the "_chunkPositionsVisibleToCreatures" get updated every time the creature enters a chunk
        inCreatureToAdd.movementComponent.OnChunkEnter += (Chunk inEnteredChunk) =>
        {
            List<Vector2DInt> newVisibleChunkPositions = CalculateVisibleChunksPositions(inEnteredChunk.data.position);
            List<Vector2DInt> oldVisibleChunkPositions = _chunkPositionsVisibleToCreatures[inCreatureToAdd];

            _chunkPositionsVisibleToCreatures[inCreatureToAdd] = newVisibleChunkPositions;


            // Calculate which chunks are newly discovered
            List<Vector2DInt> lostVisibleChunkPositions   = new List<Vector2DInt>();
            foreach (Vector2DInt oldVisibleChunkPosition in oldVisibleChunkPositions)
                if (!newVisibleChunkPositions.Contains(oldVisibleChunkPosition))
                    lostVisibleChunkPositions.Add(oldVisibleChunkPosition);

            if (lostVisibleChunkPositions.Count > 0)
                OnChunkPositionsVisibilityLost?.Invoke(lostVisibleChunkPositions);

            
            // Calculate which chunks were lost in the chunk change
            List<Vector2DInt> gainedVisibleChunkPositions = new List<Vector2DInt>();
            foreach (Vector2DInt newVisibleChunkPosition in newVisibleChunkPositions)
                if (!oldVisibleChunkPositions.Contains(newVisibleChunkPosition))
                    gainedVisibleChunkPositions.Add(newVisibleChunkPosition);

            if (gainedVisibleChunkPositions.Count > 0)
                OnChunkPositionsVisibilityGained?.Invoke(gainedVisibleChunkPositions);
        };
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