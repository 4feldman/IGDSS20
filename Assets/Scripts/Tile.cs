using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileType type;
    public Building building;
    public List<Tile> neighborTiles;

    public enum TileType {Null, Water, Sand, Grass, Forest, Stone, Mountain};
}
