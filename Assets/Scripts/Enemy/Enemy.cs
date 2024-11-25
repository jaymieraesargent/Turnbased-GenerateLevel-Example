using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int actionPoints = 4;
    public int currentActionPoints;
    private bool playerDetected = false;

    public int meleeRange = 1;
    public int rangeAttackRange = 3;
    public int moveCost = 1;
    public int turnCost = 1;
    public int turn180Cost = 2;
    public int meleeAttackCost = 1;
    public int rangeAttackCost = 1;

    public Tile currentTile;
    public Tile targetTile;
    public Transform playerTransform;
    public List<Tile> path = new List<Tile>();
    public LevelGeneration levelGeneration;
    private List<Tile> pathToPlayer = new List<Tile>();
    public bool enemyTurn = false;
    private float turnTimer = 0f; // Timer to control turn rate
    private float turnInterval = 1f; // Interval for each turn (in seconds)
    public LayerMask layerMask;

    // Define directions to check for neighbors
    Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(0, 1), // North
        new Vector2Int(1, 0), // East
        new Vector2Int(0, -1), // South
        new Vector2Int(-1, 0) // West
    };
    void Start()
    {
        if (this.transform.position.y != 1)
        {
            // Ensure the Y position is always set to 1 after movement/turn
            transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        }
        levelGeneration = GameObject.FindGameObjectWithTag("Level").GetComponent<LevelGeneration>();
        // Find the player transform in the scene
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        // Initialize the enemy's starting tile position
        currentTile = FindCurrentTile();
        currentActionPoints = actionPoints;
    }

    void Update()
    {
        if (this.transform.position.y != 1)
        {
            // Ensure the Y position is always set to 1 after movement/turn
            transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            BeginTurn(playerTransform);
        }
        if (enemyTurn)
        {
            if (currentActionPoints > 0)
            {
                turnTimer += Time.deltaTime;

                if (turnTimer >= turnInterval)
                {
                    TakeTurn();
                    turnTimer = 0f; // Reset the timer after taking the turn
                }
            }
            else
            {
                // End of turn, clear the path colors
                ClearPathColors();
                enemyTurn = false;
            }
        }
    }

    private Tile FindCurrentTile()
    {
        Ray ray = new Ray();
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 3,layerMask))
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null)
            {
                return tile;
            }
        }
        return null; // Return null if no tile is found
    }

    private void TakeTurn()
    {
        if (CheckPlayerDetection())
        {
            // If the player is detected, recalculates path to the player
            MoveTowardPlayer();
        }
        else
        {
            // If the player is not detected, continue following the pre-defined path
            FollowPath();
        }
    }
    private void UpdateTileMaterials()
    {
        // Iterate over the enemy's path
        for (int i = 0; i < path.Count; i++)
        {
            Tile pathTile = path[i];  // Get the tile in the current path

            // Calculate the position ratio of the tile within the path (from 0 to 1)
            float pathPositionRatio = (float)i / (float)(path.Count - 1);

            // Update the tile material based on whether it's part of the path
            pathTile.UpdateTileMaterial(true, pathPositionRatio);
        }

        // Optionally, reset tiles that aren't in the path
        for (int i = 0; i < path.Count; i++)
        {
            Tile tile = path[i];
            if (!path.Contains(tile))
            {
                tile.UpdateTileMaterial(false, 0);
            }
        }
    }
    private void ClearPathColors()
    {
        foreach (Tile tile in path)
        {
            tile.UpdateTileMaterial(false, 0); // Reset to default color
        }
    }

    public void BeginTurn(Transform player)
    {
        currentActionPoints = actionPoints;
        playerTransform = player;
        playerDetected = false;
        path.Clear();
        Vector3 currentPosition = transform.position;
        Tile startTile = currentTile;

        // Pick an initial point directly adjacent to the current tile
        Tile initialTile = GetAdjacentTile(startTile);

        if (initialTile != null)
        {
            targetTile = initialTile;
            path.Add(initialTile);
            ExtendPath(initialTile, 4);
        }
        else
        {
            targetTile = startTile;
            path.Clear();
        }

        UpdateTileMaterials();
        enemyTurn = true;
    }

    private Tile GetAdjacentTile(Tile startTile)
    {
        List<Tile> potentialTiles = new List<Tile>();

        foreach (var direction in directions)
        {
            Vector2Int neighborPos = startTile.Position + direction;
            if (levelGeneration.grid.ContainsKey(neighborPos) && levelGeneration.grid[neighborPos].IsWalkable)
            {
                potentialTiles.Add(levelGeneration.grid[neighborPos]);
            }
        }

        if (potentialTiles.Count > 0)
        {
            return potentialTiles[Random.Range(0, potentialTiles.Count)];
        }

        return null; // No valid adjacent tiles found
    }
    private void ExtendPath(Tile current, int targetLength)
    {
        while (path.Count < targetLength)
        {
            List<Tile> neighbors = new List<Tile>();
            foreach (var direction in directions)
            {
                Vector2Int neighborPos = current.Position + direction;
                if (levelGeneration.grid.ContainsKey(neighborPos) && levelGeneration.grid[neighborPos].IsWalkable && !path.Contains(levelGeneration.grid[neighborPos]))
                {
                    neighbors.Add(levelGeneration.grid[neighborPos]);
                }
            }

            if (neighbors.Count > 0)
            {
                Tile nextTile = neighbors[Random.Range(0, neighbors.Count)];
                path.Add(nextTile);
                current = nextTile;
            }
            else
            {
                if (path.Count > 1)
                {
                    path.Add(path[path.Count - 2]); // Backtrack by adding the previous tile
                }
                else
                {
                    break; // No more moves possible
                }
            }
        }
    }
    private bool CheckPlayerDetection()
    {
        // Check if the player is within a detection range (optional)
        float detectionRange = 10f; // Adjust the range as needed
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > detectionRange)
        {
            playerDetected = false;
            return false; // Player is out of range, can't be detected
        }

        // Calculate the direction from the enemy to the player
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Raycast from the enemy to the player to check for line of sight
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("Obstacles"); // Assuming you have an "Obstacles" layer

        // Cast a ray from the enemy's position towards the player
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, Mathf.Infinity, layerMask))
        {
            // If the ray hits something, check if it's the player
            if (hit.transform == playerTransform)
            {
                playerDetected = true;
                return true; // The player is detected
            }
        }
        playerDetected = false;
        return false; // No line of sight to the player, or the player is blocked by an obstacle
    }

    private void MoveTowardPlayer()
    {
        Tile playerTile = levelGeneration.FindTile(playerTransform.position);

        if (playerTile != null)
        {
            pathToPlayer = Pathfinding.FindPath(currentTile, playerTile);
            if (pathToPlayer != null && pathToPlayer.Count >0)
            {
                FollowPath(true); // Set to follow the player path

            }
        }
    }
    private void FollowPath(bool playerPath = false)
    {
        List<Tile> currentPath = playerPath ? pathToPlayer : path;

        if (currentPath.Count > 0)
        {
            Tile nextTile = currentPath[0];
            int movementCost = GetMovementCost(currentTile, nextTile);

            // Check if the enemy needs to rotate to face the next tile
            if (!IsFacingTile(nextTile))
            {
                TurnToFaceTile(nextTile);
            }
            else if (currentActionPoints >= movementCost)
            {
                // Proceed to move if facing the tile and have enough action points
                currentActionPoints -= movementCost;

                // Reset the material of the current tile to default color before moving
                currentTile.UpdateTileMaterial(false, 0);

                // Move to the next tile and update the current tile
                transform.position = nextTile.transform.position;
                currentTile = nextTile;
                currentPath.RemoveAt(0);

                // Ensure the remaining path is correctly visualized
                foreach (Tile tile in currentPath)
                {
                    float pathPositionRatio = (float)currentPath.IndexOf(tile) / Mathf.Max(1, (currentPath.Count - 1));
                    pathPositionRatio = Mathf.Clamp(pathPositionRatio, 0f, 1f);
                    tile.UpdateTileMaterial(true, pathPositionRatio);
                }
            }
        }
    }

    private void TurnToFaceTile(Tile tile)
    {
        // Calculate the direction vector from the enemy to the target tile
        Vector3 direction = (tile.transform.position - transform.position).normalized;

        // We only want to change the y-axis rotation
        direction.y = 0;

        // Ensure the direction vector is not zero
        if (direction == Vector3.zero)
        {
            Debug.Log("Look rotation viewing vector is zero, no need to turn.");
            return;
        }

        // Calculate the rotation needed to face the target tile
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // Ensure the enemy rotates only around the y-axis
        transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);

        // Determine the action cost based on the angle of rotation
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);

        int turnActionCost = Mathf.Abs(Mathf.RoundToInt(angle / 90)) == 2 ? turn180Cost : turnCost;

        if (currentActionPoints >= turnActionCost)
        {
            currentActionPoints -= turnActionCost;
            Debug.Log("Enemy turns to face tile.");
        }
    }

    private bool IsFacingTile(Tile tile)
    {
        Vector3 directionToTile = (tile.transform.position - transform.position).normalized;
        directionToTile.y = 0; // Ensure we only compare horizontal direction
        float angleToTile = Vector3.Angle(transform.forward, directionToTile);
        return angleToTile < 5f; // Allow a small margin of error
    }
    private void MoveToTile(Tile tile)
    {
        // Check if facing correct direction; if not, turn first
        if (!IsFacingTile(tile))
        {
            TurnToFaceTile(tile);
        }

        // Move forward if enough action points
        if (currentActionPoints >= moveCost)
        {
            transform.position = tile.transform.position;
            currentTile = FindCurrentTile();
            currentActionPoints -= moveCost;
            Debug.Log("Enemy moves to tile.");
        }
    }
    private int GetMovementCost(Tile startTile, Tile endTile)
    {
        // Assume 1 AP for moving 1 tile; adjust as needed
        return 1;
    }

    private void PerformMeleeAttack()
    {
        if (currentActionPoints >= meleeAttackCost)
        {
            // Attack logic here
            currentActionPoints -= meleeAttackCost;
            Debug.Log("Enemy performs melee attack.");
        }
    }
    private void PerformRangeAttack()
    {
        if (currentActionPoints >= rangeAttackCost)
        {
            // Range attack logic here
            currentActionPoints -= rangeAttackCost;
            Debug.Log("Enemy performs ranged attack.");
        }
    }
}