using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    public Texture2D levelMap;
    //   , assetMap;
    public PixelToObject[] pixelColorMappings;

    private Dictionary<Vector2Int, Tile> grid = new Dictionary<Vector2Int, Tile>();
    private Dictionary<Vector2Int, GameObject> tilePositions = new Dictionary<Vector2Int, GameObject>();

    private Color pixelColor;
    bool enemySpawn = false;
    bool playerSpawn = false;
    public GameObject player;
    public GameObject enemy;
    int count = 0;
    void Start()
    {
        
        GenerateLevel(levelMap);
        // GenerateLevel(assetMap);
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
                tilePositions[new Vector2Int(x, y)] = prefabClone;
                Tile tile = prefabClone.GetComponentInChildren<Tile>();
                tile.SetTile(gridPosition, isWalkable);
                grid[gridPosition] = tile;
                Vector3 entityPos = new Vector3(position.x,1,position.z);
                if (!playerSpawn)
                {

                    //spawn Player
                    Instantiate(player, entityPos, Quaternion.identity);
                    playerSpawn = true;
                }
                if(count>5 && !enemySpawn)
                {
                    Instantiate(enemy, entityPos, Quaternion.identity);
                    enemySpawn = true;
                }

            }
        }
    }
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

}

[System.Serializable]//View in the Inspector
public class PixelToObject
{
    public Color pixelColor;
    public GameObject prefab;
}