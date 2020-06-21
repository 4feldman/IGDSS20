using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductionBuilding : Building
{
    public List<ResourceManager.ResourceTypes> inputResources = new List<ResourceManager.ResourceTypes>();
    public ResourceManager.ResourceTypes outputResource;
    
    public ProductionBuildingType type;
    public Tile.TileTypes scalesWithNeighboringTiles = Tile.TileTypes.Empty;
    public int minNeighbors;
    public int maxNeighbors;
    public int resourceGenInterval;
    public int resourceGenAmount;
    
    #region Workers
    public List<Worker> _workers; //List of all workers associated with this building, either for work or living
    #endregion

    #region Jobs
    public List<Job> _jobs; // List of all available Jobs. Is populated in Start()
    public int _numJobs = 1;
    #endregion
    
    #region Production
    private float _efficiency = 1.0f;
    private float _neighborEfficiency = 1.0f;
    private float _progress = 0;
    private bool _production = false;
    private Dictionary<ResourceManager.ResourceTypes, float> inputResourceDict = new Dictionary<ResourceManager.ResourceTypes, float>();
    #endregion

    #region Enumerations
    public enum ProductionBuildingType {Null, Fishery, Lumberjack, Sawmill, SheepFarm, FrameworkKnitters, PotatoFarm, SchnappsDistillery};
    #endregion

    #region MonoBehaviour
    // Start is called before the first frame update, TODO remove if not needed
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame, TODO remove if not needed
    protected override void Update()
    {
        base.Update();
    }
    #endregion
    
    #region Initial
    public override void Initialize( Tile tile, ResourceManager resourceManager, JobManager jobManager)
    {
        base.Initialize(tile, resourceManager, jobManager);
        Debug.Log("Production");
        GenerateInputResources();
        GenerateNeighborEfficiency(tile._neighborTiles);
        GenerateJobs();
        InvokeRepeating(nameof(ProductionCycle), 0f, 1.0f);
    }

    private void GenerateInputResources()
    {
        foreach (ResourceManager.ResourceTypes resource in inputResources)
        {
            if (inputResourceDict.ContainsKey(resource))
            {
                inputResourceDict[resource] += 1;
            }
            else
            {
                inputResourceDict.Add(resource, 1);
            }
        }
    }
    
    private void GenerateNeighborEfficiency(IEnumerable<Tile> neighborTiles)
    {
        if (scalesWithNeighboringTiles == Tile.TileTypes.Empty) return;
        
        int fittingNeighbors = neighborTiles.Count(neighbor => neighbor._type == scalesWithNeighboringTiles);
        if (fittingNeighbors < minNeighbors)
        {
            _neighborEfficiency = 0.0f;
            return;
        }
        if (fittingNeighbors < maxNeighbors) {
            _neighborEfficiency = (float)fittingNeighbors / (float)maxNeighbors;
        }
        GenerateEfficiency();
    }

    private void GenerateJobs()
    {
        for(int i = 0; i < _numJobs; i++)
        {
            Job j = new Job(this);
            _jobs.Add(j);
            _jobManager.RegisterJob(j);
        }
    }
    #endregion

    #region Workers
    public void WorkerAssignedToBuilding(Worker w)
    {
        _workers.Add(w);
    }

    public void WorkerRemovedFromBuilding(Worker w)
    {
        _workers.Remove(w);
    }
    #endregion

    #region Production
   public void ProductionCycle()
    {
        if (!_production) StartProduction();

        if (!_production) return;
        GenerateEfficiency();
        _progress += _efficiency;
        if (_progress < resourceGenInterval) return;
        
        _resourceManager.addResource(outputResource, resourceGenAmount);
        StartProduction();
    }
    
    private void StartProduction()
    {
        _production = _resourceManager.removeResource(inputResourceDict);
        if (_production)
        {
            _progress = 0.0f;
        }
    }

    private void GenerateEfficiency()
    {
        _efficiency = _neighborEfficiency * (_workers.Sum(x => x._happiness) / _numJobs);
    }
    #endregion

    #region Money

    private void Costs()
    {
        _resourceManager.removeMoney(cost_money);   
    }
    #endregion
}
