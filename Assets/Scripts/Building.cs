using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    #region Attributes
    public BuildingType type;
    public int upkeep;
    public int cost_money;
    public int cost_planks;
    public int resourceGenInterval;
    public int resourceGenAmount;
    public List<Tile.TileTypes> possibleTileTypes;
    public Tile.TileTypes scalesWithNeighboringTiles = Tile.TileTypes.Empty;
    public int minNeighbors;
    public int maxNeighbors;
    
    private Tile _tile;
    #endregion

    #region Enumerations
    public enum BuildingType {Null, Fishery, Lumberjack, Sawmill, SheepFarm, FrameworkKnitters, PotatoFarm, SchnappsDistillery};
    #endregion

    #region Manager References
    JobManager _jobManager; //Reference to the JobManager
    #endregion
    
    #region Workers
    public List<Worker> _workers; //List of all workers associated with this building, either for work or living
    #endregion

    #region Jobs
    public List<Job> _jobs; // List of all available Jobs. Is populated in Start()
    #endregion

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected void Initialize(Tile tile)
    {
        _tile = tile;
    }

    public void WorkerAssignedToBuilding(Worker w)
    {
        _workers.Add(w);
    }

    public void WorkerRemovedFromBuilding(Worker w)
    {
        _workers.Remove(w);
    }

    public bool canBeBuilt(Tile t, int bank, Dictionary<GameManager.ResourceTypes, float> warehouse)
    {
        if (!possibleTileTypes.Contains(t._type))
        {
            Debug.Log("Wrong tile type");
            return false;
        }
        if (bank < cost_money)
        {
            Debug.Log("Not enough money");
            return false;
        }
        if (warehouse[GameManager.ResourceTypes.Planks] < cost_planks)
        {
            Debug.Log("Not enough planks");
            return false;
        }
        return true;
    }
}
