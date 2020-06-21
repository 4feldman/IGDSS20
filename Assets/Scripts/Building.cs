using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    #region Attributes
    public int upkeep;
    public float cost_money;
    public int cost_planks;
    public List<Tile.TileTypes> possibleTileTypes;
    
    private Tile _tile;
    #endregion

    #region Manager References
    internal ResourceManager _resourceManager;
    internal JobManager _jobManager;
    #endregion

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public virtual void Initialize(Tile tile, ResourceManager resourceManager, JobManager jobManager)
    {
        Debug.Log("Building");
        _tile = tile;
        _resourceManager = resourceManager;
        _jobManager = jobManager;
        
        _resourceManager.removeResource(ResourceManager.ResourceTypes.Planks, cost_planks);
        _resourceManager.removeMoney(cost_money);
    }

    public bool canBeBuilt(Tile t, ResourceManager resourceManager)
    {
        if (!possibleTileTypes.Contains(t._type))
        {
            Debug.Log("Wrong tile type");
            return false;
        }
        if (resourceManager.moneyCount() < cost_money)
        {
            Debug.Log("Not enough money");
            return false;
        }
        if (resourceManager.resourceCount(ResourceManager.ResourceTypes.Planks) < cost_planks)
        {
            Debug.Log("Not enough planks");
            return false;
        }
        return true;
    }
}
