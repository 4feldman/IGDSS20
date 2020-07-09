using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

public partial class GameManager : MonoBehaviour
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

    #region Managers
    public ResourceManager resourceManager;
    public JobManager jobManager;
    #endregion
    
    #region Enumerations

    //Enumeration of all available resourceManager types. Can be addressed from other scripts by calling GameManager.ResourceTypes
    #endregion

    #region MonoBehaviour
    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyboardInput();
    }
    #endregion

    #region Methods
    //Makes the resourceManager dictionary usable by populating the values and keys

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

        if (!buildingPrefab.canBeBuilt(t, resourceManager)) return;

        Building building = Instantiate(prefab, t.transform.position, Quaternion.identity).GetComponent<Building>();
        building.Initialize(t, resourceManager, jobManager);
        t._building = building;
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
            DisableTileEdges(tile, tile._neighborTiles);
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

        Tile t = tile.GetComponent<Tile>();
        t._coordinateHeight = i;
        t._coordinateWidth = j;
        _tileMap[i, j] = t;
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

    public void DisableTileEdges(Tile tile, List<Tile> neighbors)
    {
        foreach (Tile n in neighbors)
        {
            if (tile._type == n._type)
            {
                GameObject tileEdge;
                //Neighbor above tile
                if (n._coordinateHeight - tile._coordinateHeight < 0)
                {
                    //Neighbor definitely at top left
                    if (n._coordinateWidth < tile._coordinateWidth)
                    {
                        tileEdge = tile._tileEdges[3];
                    }
                    //Neighbor definitely at top right
                    else if (n._coordinateWidth > tile._coordinateWidth)
                    {
                        tileEdge = tile._tileEdges[4];
                    }
                    //Neighbor definitely at top left
                    else if (tile._coordinateHeight % 2 == 1)
                    {
                        tileEdge = tile._tileEdges[3];
                    }
                    //Neighbor definitely at top right
                    else
                    {
                        tileEdge = tile._tileEdges[4];
                    }
                }
                //Neighbor next to tile
                else if (n._coordinateHeight - tile._coordinateHeight == 0)
                {
                    //Neighbor left of tile
                    if (n._coordinateWidth < tile._coordinateWidth)
                    {
                        tileEdge = tile._tileEdges[2];
                    }
                    //Neighbor right of tile
                    else
                    {
                        tileEdge = tile._tileEdges[5];
                    }
                }
                //Neighbor below tile
                else
                {
                    //Neighbor definitely at bottom left
                    if (n._coordinateWidth < tile._coordinateWidth)
                    {
                        tileEdge = tile._tileEdges[1];
                    }
                    //Neighbor definitely at bottom right
                    else if (n._coordinateWidth > tile._coordinateWidth)
                    {
                        tileEdge = tile._tileEdges[0];
                    }
                    //Neighbor definitely at bottom left
                    else if (tile._coordinateHeight % 2 == 1)
                    {
                        tileEdge = tile._tileEdges[1];
                    }
                    //Neighbor definitely at bottom right
                    else
                    {
                        tileEdge = tile._tileEdges[0];
                    }
                }
                tileEdge.SetActive(false);
            }
        }
    }
    #endregion
}
