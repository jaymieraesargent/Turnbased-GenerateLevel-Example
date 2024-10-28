using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //public Vector2Int Position; 
    //public bool IsWalkable;
    //public bool IsOccupied;
    public Vector2Int Position { get; private set; }
    public bool IsWalkable { get; set; }
    public bool IsOccupied { get; set; }

    public void SetTile(Vector2Int position, bool isWalkable)
    {
        Position = position;
        IsWalkable = isWalkable;
        IsOccupied = false;
    }
}