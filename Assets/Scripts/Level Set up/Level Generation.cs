using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    #region Variables
    // levelMap: A Texture2D object that represents the level layout where each pixel color corresponds to different game objects (e.g., tiles, enemies).
    public Texture2D levelMap;
    // pixelColorMappings: An array of PixelToObject that maps specific colors to prefabs (game objects) in the game world.
    public PixelToObject[] pixelColorMappings;
    // grid: A Dictionary<Vector2Int, Tile> storing the generated tiles by their grid position.
    private Dictionary<Vector2Int, Tile> grid = new Dictionary<Vector2Int, Tile>();
    // tilePositions: A Dictionary<Vector2Int, GameObject> for tracking the game objects (tiles) at specific grid positions.
    private Dictionary<Vector2Int, GameObject> tilePositions = new Dictionary<Vector2Int, GameObject>();
    // pixelColor: Stores the color of the current pixel being processed.
    private Color pixelColor;
    // enemySpawn: Flags to check if the enemy has already been spawned.
    bool enemySpawn = false;
    // playerSpawn: Flags to check if the player has already been spawned.
    bool playerSpawn = false;
    //player: Prefabs for the player character.
    public GameObject player;
    // enemy: Prefabs for the enemy character.
    public GameObject enemy;
    // count: A counter used to limit the number of times the enemy spawns.
    int count = 0;
    #endregion

    public Vector3 GetTileCenter(Vector2Int gridPosition)
    {
        if (tilePositions.TryGetValue(gridPosition, out GameObject tile))
        {
            return tile.transform.position;
        }
        return Vector3.zero;
    }
    public bool DetermineWalkability(GameObject prefab)
    {
        // Define walkability based on prefab properties
        return prefab.gameObject.GetComponentInChildren<Tile>().CompareTag("Walkable");
    }
    public bool IsWalkableTile(Vector2Int position)
    {
        return grid.ContainsKey(position) && grid[position].IsWalkable && !grid[position].IsOccupied;
    }

    public void UpdateTileOccupation(Vector2Int position, bool isOccupied)
    {
        if (grid.ContainsKey(position))
        {
            grid[position].IsOccupied = isOccupied;
        }
    }

    void Start()
    {
        GenerateLevel(levelMap);
        SetNeighbors();

    }
    public void GenerateLevel(Texture2D mapTexture)
    {
        //Scan whole Texture and get positions of objects
        for (int x = 0; x < mapTexture.width; x++)
        {
            for (int y = 0; y < mapTexture.height; y++)
            {
                GenerateObject(x, y, mapTexture);
            }
        }
    }
    public void GenerateObject(int x, int y, Texture2D map)
    {

        //Read Pixel Colour
        pixelColor = map.GetPixel(x, y);
        if (pixelColor.a == 0)
        {
            //do nothing
            Debug.Log("Skip");

            return;
        }

        foreach (PixelToObject pixelColorMapping in pixelColorMappings)
        {
            Debug.Log("Check");
            //Scan pixelColorMappings Array for Matching colour Mapping
            if (pixelColorMapping.pixelColor.Equals(pixelColor))
            {
                Debug.Log("Load");
                count++;

                Vector3 position = new Vector3(x + (4 * x), 0, y + (4 * y));
                GameObject prefabClone = Instantiate(pixelColorMapping.prefab, position, Quaternion.identity, transform);
                bool isWalkable = DetermineWalkability(prefabClone); // You can define based on prefab
                Vector2Int gridPosition = new Vector2Int(x, y);
                prefabClone.name = $"Tile_{x}_{y}";
                tilePositions[gridPosition] = prefabClone;
                Tile tile = prefabClone.GetComponentInChildren<Tile>();
                tile.SetTile(gridPosition, isWalkable);
                grid[gridPosition] = tile;

                Vector3 entityPos = new Vector3(position.x, 1, position.z);
                if (!playerSpawn)
                {

                    //spawn Player
                    Instantiate(player, entityPos, Quaternion.identity);
                    playerSpawn = true;
                }
                if (count > 5 && !enemySpawn)
                {
                    Instantiate(enemy, entityPos, Quaternion.identity);
                    enemySpawn = true;
                }

            }
        }
    }  

    public void SetNeighbors()
    {
        for (int x = 0; x < levelMap.width; x++)
        {
            for (int z = 0; z < levelMap.height; z++)
            {
                Vector2Int gridPosition = new Vector2Int(x, z);

                if (grid.ContainsKey(gridPosition))
                {
                    Tile currentTile = grid[gridPosition];                
                    // Up
                    Vector2Int neighborPosition = new Vector2Int(x, z + 1);
                    if (z + 1 < levelMap.height && grid.ContainsKey(neighborPosition))
                    {
                        currentTile.neighborNorth = grid[neighborPosition];
                    }
                    // Right
                    neighborPosition = new Vector2Int(x + 1, z);
                    if (x + 1 < levelMap.width && grid.ContainsKey(neighborPosition))
                    {
                        currentTile.neighborEast = grid[neighborPosition];
                    }
                    // Down
                    neighborPosition = new Vector2Int(x, z - 1);
                    if (z - 1 >= 0 && grid.ContainsKey(neighborPosition))
                    {
                        currentTile.neighborSouth = grid[neighborPosition];
                    }
                    // Left
                    neighborPosition = new Vector2Int(x - 1, z);
                    if (x - 1 >= 0 && grid.ContainsKey(neighborPosition))
                    {
                        currentTile.neighborWest = grid[neighborPosition];
                    }
                }
            }
        }
    }
}

[System.Serializable]//View in the Inspector
public class PixelToObject
{
    public Color pixelColor;
    public GameObject prefab;
}