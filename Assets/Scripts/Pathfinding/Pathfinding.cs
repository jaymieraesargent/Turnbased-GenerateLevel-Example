using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Tile startTile;
    public Tile targetTile;
    private Dictionary<Vector2Int, Tile> grid; // Reference to the level's tile grid

    void Start()
    {
        // Initialize or reference your grid here if needed
    }

    public static List<Tile> FindPath(Tile startTile, Tile targetTile)
    {
        // Open set of nodes to evaluate, closed set for evaluated nodes
        List<Tile> openSet = new List<Tile> { startTile };
        HashSet<Tile> closedSet = new HashSet<Tile>();

        while (openSet.Count > 0)
        {
            Tile currentTile = openSet[0];

            // Find the tile in openSet with the lowest TotalCost
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].TotalCost < currentTile.TotalCost || (openSet[i].TotalCost == currentTile.TotalCost && openSet[i].EstimatedCostToTarget < currentTile.EstimatedCostToTarget))
                {
                    currentTile = openSet[i];
                }
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            // If reached the target
            if (currentTile == targetTile)
            {
                return RetracePath(startTile, targetTile);
            }

            // Check each neighbor
            foreach (Tile neighbor in GetNeighbors(currentTile))
            {
                if (!neighbor.IsWalkable || closedSet.Contains(neighbor))
                    continue;

                int newMovementCostToNeighbor = currentTile.CostFromStart + GetDistance(currentTile, neighbor);
                if (newMovementCostToNeighbor < neighbor.CostFromStart || !openSet.Contains(neighbor))
                {
                    neighbor.CostFromStart = newMovementCostToNeighbor;
                    neighbor.EstimatedCostToTarget = GetDistance(neighbor, targetTile);
                    neighbor.ParentTile = currentTile;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null; // Return null if no path found
    }

    private static List<Tile> RetracePath(Tile startTile, Tile endTile)
    {
       
        Tile currentTile = endTile;
        List<Tile> path = new List<Tile>();
        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.ParentTile;
        }
        path.Reverse();
        return path;
    }

    private static int GetDistance(Tile tileA, Tile tileB)
    {
        int dstX = Mathf.Abs(tileA.Position.x - tileB.Position.x);
        int dstY = Mathf.Abs(tileA.Position.y - tileB.Position.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    private static List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        if (tile.neighborNorth != null) neighbors.Add(tile.neighborNorth);
        if (tile.neighborSouth != null) neighbors.Add(tile.neighborSouth);
        if (tile.neighborEast != null) neighbors.Add(tile.neighborEast);
        if (tile.neighborWest != null) neighbors.Add(tile.neighborWest);

        return neighbors;
    }
   
}