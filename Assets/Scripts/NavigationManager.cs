﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tile;

public class NavigationManager
{
    private Dictionary<Tile, int> _potentialMap;
    private Building _building;
    private const int _grassWeight = 1;
    private const int _stoneWeight = 1;
    private const int _sandWeight = 2;
    private const int _forestWeight = 2;
    private const int _mountainWeight = 3;
    private const int _waterWeight = 30;

    public NavigationManager(Building building)
    {
        _potentialMap = new Dictionary<Tile, int>();
        _potentialMap.Add(building._tile, 0);
        generateMapForNeighbors(building._tile, 0);
    }

    public List<Tile> generatePath(Tile origin)
    {
        List<Tile> path = new List<Tile>();
        path.Add(origin);
        int remainingWeight = getTileWeight(origin);

        Tile currentTile = origin;
        while (remainingWeight > 0)
        {
            Tile newTile = null;
            int newWeight = 0;
            foreach (Tile t in currentTile._neighborTiles)
            {
                if (newTile == null)
                {
                    newTile = t;
                    newWeight = getTileWeight(t);
                }
                else if (getTileWeight(t) < newWeight)
                {
                    newTile = t;
                    newWeight = getTileWeight(t);
                }
            }
            path.Add(newTile);
            currentTile = newTile;
            remainingWeight = newWeight;
        }
        return path;
    }

    private int getTileWeight(Tile t)
    {
        if (_potentialMap.ContainsKey(t))
        {
            return _potentialMap[t];
        }
        return 0;
    }

    private void generateMapForNeighbors(Tile tile, int tileWeight)
    {
        foreach (Tile t in tile._neighborTiles)
        {
            int newWeight = tileWeight + getWeight(t);

            if (!_potentialMap.ContainsKey(t))
            {
                _potentialMap.Add(t, newWeight);
                generateMapForNeighbors(t, newWeight);
            }
            else if (_potentialMap[t] > newWeight)
            {
                _potentialMap[t] = newWeight;
                generateMapForNeighbors(t, newWeight);
            }
        }
    }

    private int getWeight(Tile tile)
    {
        switch (tile._type)
        {
            case TileTypes.Grass:
                return _grassWeight;
            case TileTypes.Stone:
                return _stoneWeight;
            case TileTypes.Sand:
                return _sandWeight;
            case TileTypes.Forest:
                return _forestWeight;
            case TileTypes.Mountain:
                return _mountainWeight;
            case TileTypes.Water:
                return _waterWeight;
            default:
                return int.MaxValue;
        }
    }
}
