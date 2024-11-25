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
    public Dictionary<Vector2Int, Tile> grid = new Dictionary<Vector2Int, Tile>();
    // tilePositions: A Dictionary<Vector2Int, GameObject> for tracking the game objects (tiles) at specific grid positions.
    private Dictionary<Vector2Int, GameObject> tilePositions = new Dictionary<Vector2Int, GameObject>();
    // pixelColor: Stores the color of the current pixel being processed.
    private Color pixelColor;
    //player: Prefabs for the player character.
    public GameObject player;
    // enemy: Prefabs for the enemy character.
    public GameObject enemy;
    // count: A counter used to limit the number of times the enemy spawns.
    int count = 0;
    public int tileSize = 4;
    public List<Vector2Int> spawnPositions = new List<Vector2Int>();
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
    /// <summary>
    /// Called by Unity when the script starts. It invokes GenerateLevel(levelMap) to generate the level 
    /// and SetNeighbors() to set up the neighboring relationships between tiles.
    /// </summary>
    void Start()
    {
        GenerateLevel(levelMap);
        SetNeighbors();

    }
    public void GenerateLevel(Texture2D mapTexture)
    {
        // Scan whole Texture and get positions of objects
        // Loops through each pixel in the texture map (from x = 0 to width and y = 0 to height).
        for (int x = 0; x < mapTexture.width; x++)
        {
            for (int y = 0; y < mapTexture.height; y++)
            {
                // For each pixel, it calls GenerateObject(x, y, mapTexture) to create the corresponding object(tile) based on the pixel color.
                GenerateObject(x, y, mapTexture);
            }
        }
        // Randomly select spawn positions for player and enemy after generating the level
        if (spawnPositions.Count > 0)
        {
            Vector2Int playerSpawnPos = spawnPositions[Random.Range(0, spawnPositions.Count)];
            Vector3 playerPosition = GetTileCenter(playerSpawnPos);
            Instantiate(player, playerPosition, Quaternion.identity);

            // Ensure enemy spawns at a different position
            Vector2Int enemySpawnPos;
            do
            {
                enemySpawnPos = spawnPositions[Random.Range(0, spawnPositions.Count)];
            }
            while (enemySpawnPos == playerSpawnPos);
            Vector3 enemyPosition = GetTileCenter(enemySpawnPos);
            Instantiate(enemy, enemyPosition, Quaternion.identity);
        }
    }
    public void GenerateObject(int x, int y, Texture2D map)
    {

        // Reads the pixel color at(x, y) on the map using map.GetPixel(x, y).
        pixelColor = map.GetPixel(x, y);
        // If the pixel is fully transparent (alpha = 0)
        if (pixelColor.a == 0)
        {
            //  it skips processing.
            return;
        }
        // otherwise the pixel is non-transparent
        else
        {
           // iterates through the pixelColorMappings array
            foreach (PixelToObject pixelColorMapping in pixelColorMappings)
            {
                //Scan pixelColorMappings Array for Matching colour Mapping
                if (pixelColorMapping.pixelColor.Equals(pixelColor))
                {
                    count++;

                    Vector3 position = new Vector3(x + (tileSize * x), 0, y + (tileSize * y));
                    // instantiate the corresponding prefab at the correct position
                    GameObject prefabClone = Instantiate(pixelColorMapping.prefab, position, Quaternion.identity, transform);
                    // It checks if the tile is walkable using DetermineWalkability().
                    bool isWalkable = DetermineWalkability(prefabClone);
                    Vector2Int gridPosition = new Vector2Int(x, y);
                    prefabClone.name = $"Tile_{x}_{y}";
                    tilePositions[gridPosition] = prefabClone;
                    Tile tile = prefabClone.GetComponentInChildren<Tile>();
                    tile.SetTile(gridPosition, isWalkable);
                    // stores the Tile in the grid dictionary with the position as the key.
                    grid[gridPosition] = tile;

                    Vector3 entityPos = new Vector3(position.x, 1, position.z);
                    if (isWalkable)
                    {
                        // Add to possible spawn positions if walkable
                        spawnPositions.Add(gridPosition); 

                    }
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
    public Tile FindTile(Vector3 position)
    {
        // Convert world position to grid position based on tile size
        int x = Mathf.FloorToInt(position.x / tileSize);
        int y = Mathf.FloorToInt(position.z / tileSize); // Assuming the grid is flat on the XZ plane

        Vector2Int gridPosition = new Vector2Int(x, y);

        // Check if the tile exists in the grid dictionary
        if (tilePositions.ContainsKey(gridPosition))
        {
            return grid[gridPosition]; // Return the tile at the grid position
        }

        return null; // Return null if the tile does not exist
    }


}

[System.Serializable]//View in the Inspector
public class PixelToObject
{
    public Color pixelColor;
    public GameObject prefab;
}