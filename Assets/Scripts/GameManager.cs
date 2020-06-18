﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    #region Map generation
    private Tile[,] _tileMap;
    private Transform tiles_parent;

    public Texture2D heightmap;
    public GameObject waterPrefab;
    public GameObject sandPrefab;
    public GameObject grassPrefab;
    public GameObject forestPrefab;
    public GameObject stonePrefab;
    public GameObject mountainPrefab;
    #endregion

    #region Buildings
    public GameObject[] _buildingPrefabs; //References to the building prefabs
    public int _selectedBuildingPrefabIndex = 0; //The current index used for choosing a prefab to spawn from the _buildingPrefabs list
    private List<GameObject> _builtBuildings;
    #endregion

    #region Resources
    public int constantIncome = 100;
    public int bank = 1000;
    public int totalUpkeep = 0;
    private Dictionary<ResourceTypes, float> _resourcesInWarehouse = new Dictionary<ResourceTypes, float>(); //Holds a number of stored resources for every ResourceType

    //A representation of _resourcesInWarehouse, broken into individual floats. Only for display in inspector, will be removed and replaced with UI later
    [SerializeField]
    private float _ResourcesInWarehouse_Fish;
    [SerializeField]
    private float _ResourcesInWarehouse_Wood;
    [SerializeField]
    private float _ResourcesInWarehouse_Planks;
    [SerializeField]
    private float _ResourcesInWarehouse_Wool;
    [SerializeField]
    private float _ResourcesInWarehouse_Clothes;
    [SerializeField]
    private float _ResourcesInWarehouse_Potato;
    [SerializeField]
    private float _ResourcesInWarehouse_Schnapps;
    #endregion
    
    #region Enumerations
    public enum ResourceTypes { None, Fish, Wood, Planks, Wool, Clothes, Potato, Schnapps }; //Enumeration of all available resource types. Can be addressed from other scripts by calling GameManager.ResourceTypes
    #endregion

    #region MonoBehaviour
    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
        PopulateResourceDictionary();
        InvokeRepeating(nameof(EconomyCycle), 60.0f, 60.0f);
    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyboardInput();
        UpdateInspectorNumbersForResources();
    }
    #endregion

    #region Methods
    //Makes the resource dictionary usable by populating the values and keys
    void PopulateResourceDictionary()
    {
        _resourcesInWarehouse.Add(ResourceTypes.None, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Fish, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Wood, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Planks, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Wool, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Clothes, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Potato, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Schnapps, 0);
    }

    //Sets the index for the currently selected building prefab by checking key presses on the numbers 1 to 0
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _selectedBuildingPrefabIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _selectedBuildingPrefabIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _selectedBuildingPrefabIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _selectedBuildingPrefabIndex = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _selectedBuildingPrefabIndex = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _selectedBuildingPrefabIndex = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            _selectedBuildingPrefabIndex = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            _selectedBuildingPrefabIndex = 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            _selectedBuildingPrefabIndex = 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _selectedBuildingPrefabIndex = 9;
        }
        
    }

    void EconomyCycle()
    {
        bank += constantIncome - totalUpkeep;
    }

    //Updates the visual representation of the resource dictionary in the inspector. Only for debugging
    void UpdateInspectorNumbersForResources()
    {
        _ResourcesInWarehouse_Fish = _resourcesInWarehouse[ResourceTypes.Fish];
        _ResourcesInWarehouse_Wood = _resourcesInWarehouse[ResourceTypes.Wood];
        _ResourcesInWarehouse_Planks = _resourcesInWarehouse[ResourceTypes.Planks];
        _ResourcesInWarehouse_Wool = _resourcesInWarehouse[ResourceTypes.Wool];
        _ResourcesInWarehouse_Clothes = _resourcesInWarehouse[ResourceTypes.Clothes];
        _ResourcesInWarehouse_Potato = _resourcesInWarehouse[ResourceTypes.Potato];
        _ResourcesInWarehouse_Schnapps = _resourcesInWarehouse[ResourceTypes.Schnapps];
    }

    //Checks if there is at least one material for the queried resource type in the warehouse
    public bool HasResourceInWarehoues(ResourceTypes resource)
    {
        return _resourcesInWarehouse[resource] >= 1;
    }

    //Is called by MouseManager when a tile was clicked
    //Forwards the tile to the method for spawning buildings
    public void TileClicked(Tile t)
    {
        PlaceBuildingOnTile(t);
    }

    //Checks if the currently selected building type can be placed on the given tile and then instantiates an instance of the prefab
    private void PlaceBuildingOnTile(Tile t)
    {
        //if there is building prefab for the number input
        if (_selectedBuildingPrefabIndex >= _buildingPrefabs.Length) return;

        GameObject prefab = _buildingPrefabs[_selectedBuildingPrefabIndex];
        Building buildingPrefab = prefab.GetComponent<Building>();

        if (t._building != null)
        {
            Debug.Log("Tile already occupied");
            return;
        }

        if (!buildingPrefab.canBeBuilt(t, bank, _resourcesInWarehouse)) return;

        ProductionBuilding building = Instantiate(prefab, t.transform.position, Quaternion.identity).GetComponent<ProductionBuilding>();
        building.Initialize(_resourcesInWarehouse, t, ref bank);
        t._building = building;
        totalUpkeep += building.upkeep;
    }
    
    private void GenerateMap()
    {
        tiles_parent = GameObject.Find("Tiles").transform;
        DeactivateDefaultTiles();
        _tileMap = new Tile[heightmap.height, heightmap.width];
        for (int i = 0; i < heightmap.height; i++)
        {
            for (int j = 0; j < heightmap.width; j++)
            {
                float val = heightmap.GetPixel(i, j).b;
                Vector3 pos;
                if (i % 2 == 1)
                {
                    pos = new Vector3(i * 8.66f, val * 10, j * 10 + 5);
                }
                else
                {
                    pos = new Vector3(i * 8.66f, val * 10, j * 10);
                }


                if (val == 0.0f)
                {
                    InstantiateTile(waterPrefab, pos, i, j);
                }
                else if (val < 0.2f)
                {
                    InstantiateTile(sandPrefab, pos, i, j);
                }
                else if (val < 0.4f)
                {
                    InstantiateTile(grassPrefab, pos, i, j);
                }
                else if (val < 0.6f)
                {
                    InstantiateTile(forestPrefab, pos, i, j);
                }
                else if (val < 0.8f)
                {
                    InstantiateTile(stonePrefab, pos, i, j);
                }
                else
                {
                    InstantiateTile(mountainPrefab, pos, i, j);
                }
            }
        }
        foreach (Tile tile in _tileMap)
        {
            tile._neighborTiles = FindNeighborsOfTile(tile);
        }
    }

    private void DeactivateDefaultTiles()
    {
        GameObject obj = GameObject.Find("WaterTile");
        obj.SetActive(false);
        obj = GameObject.Find("SandTile");
        obj.SetActive(false);
        obj = GameObject.Find("GrassTile");
        obj.SetActive(false);
        obj = GameObject.Find("ForestTile");
        obj.SetActive(false);
        obj = GameObject.Find("StoneTile");
        obj.SetActive(false);
        obj = GameObject.Find("MountainTile");
        obj.SetActive(false);
    }

    private GameObject InstantiateTile(GameObject prefab, Vector3 pos, int i, int j)
    {
        GameObject tile = Instantiate(prefab, pos, Quaternion.identity);
        tile.layer = 8;
        tile.transform.parent = tiles_parent;
        _tileMap[i, j] = tile.GetComponent<Tile>();
        return tile;
    }

    //Returns a list of all neighbors of a given tile
    public List<Tile> FindNeighborsOfTile(Tile tile)
    {
        List<Tile> result = new List<Tile>();

        Collider[] hit = Physics.OverlapSphere(tile.transform.position, 10f);

        foreach (var t in hit)
        {
            if (t.gameObject.name.Contains("Tile"))
            {
                Tile neighbor = t.GetComponentInParent<Tile>();
                if (neighbor != tile)
                {
                    result.Add(neighbor);
                }
            }
        }
        return result;
    }
    #endregion
}
