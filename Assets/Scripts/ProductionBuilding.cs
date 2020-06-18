using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductionBuilding : Building
{
    public List<GameManager.ResourceTypes> inputResources = new List<GameManager.ResourceTypes>();
    public GameManager.ResourceTypes outputResource;

    private float _efficiency = 1.0f;
    private Dictionary<GameManager.ResourceTypes, float> _warehouse;
    private float _progress = 0;
    private bool _production = false;

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
    
    #region Methods

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
    
    public void Initialize(Dictionary<GameManager.ResourceTypes, float> warehouse, Tile tile, ref int bank)
    {
        base.Initialize(tile);
        _warehouse = warehouse;
        _warehouse[GameManager.ResourceTypes.Planks] -= cost_planks;
        bank -= cost_money;
        GenerateEfficiency(tile._neighborTiles);
        InvokeRepeating(nameof(ProductionCycle), 0f, 1.0f);;
    }

    public void ProductionCycle()
    {
        if (!_production) StartProduction();

        if (!_production) return;
        _progress += _efficiency;
        if (_progress < resourceGenInterval) return;
        
        _warehouse[outputResource] += resourceGenAmount;
        StartProduction();
    }
    
    private void StartProduction()
    {
        if (!inputResources.TrueForAll(inputResource => _warehouse[inputResource] >= 1)) return;
        
        _production = true;
        inputResources.ForEach(inputResource => _warehouse[inputResource] -= 1);
        _progress = 0.0f;
    }
    
    private void GenerateEfficiency(IEnumerable<Tile> neighborTiles)
    {
        if (scalesWithNeighboringTiles == Tile.TileTypes.Empty) return;
        
        int fittingNeighbors = neighborTiles.Count(neighbor => neighbor._type == scalesWithNeighboringTiles);
        if (fittingNeighbors < minNeighbors)
        {
            _efficiency = 0.0f;
            return;
        }
        if (fittingNeighbors < maxNeighbors) {
            _efficiency = (float)fittingNeighbors / (float)maxNeighbors;
        }
    }
    #endregion
}
