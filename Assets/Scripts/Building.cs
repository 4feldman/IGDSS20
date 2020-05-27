using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingType type;
    public int upkeep;
    public int cost_money;
    public int cost_planks;
    public Tile tile;
    public float efficiency = 1.0f;
    public int resourceGenInterval;
    public int resourceGenAmount;
    public List<Tile.TileTypes> possibleTileTypes;
    public Tile.TileTypes scalesWithNeighboringTiles = Tile.TileTypes.Empty;
    public int minNeighbors;
    public int maxNeighbors;
    public List<GameManager.ResourceTypes> inputResources = new List<GameManager.ResourceTypes>();
    public GameManager.ResourceTypes outputResource;

    public enum BuildingType {Null, Fishery, Lumberjack, Sawmill, SheepFarm, FrameworkKnitters, PotatoFarm, SchnappsDistillery};

    public void generateEfficiency(List<Tile> neighborTiles)
    {
        if(scalesWithNeighboringTiles != Tile.TileTypes.Empty)
        {
            int fittingNeighbors = 0;
            foreach(Tile neighbor in neighborTiles)
            {
                if (neighbor._type == scalesWithNeighboringTiles)
                {
                    fittingNeighbors++;
                }
            }
            if (fittingNeighbors < minNeighbors)
            {
                efficiency = 0.0f;
            }
            else if (fittingNeighbors >= maxNeighbors)
            {
                efficiency = 1.0f;
            }
            else {
                efficiency = (float)fittingNeighbors / (float)maxNeighbors;
            }
        }
    }
}
