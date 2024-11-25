using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int actionPoints = 4;
    public int currentActionPoints;
    [SerializeField] private bool playerDetected = false;

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
    [SerializeField] private List<Tile> pathToPlayer = new List<Tile>();
    public bool enemyTurn = false;
    [SerializeField] private float turnTimer = 0f; // Timer to control turn rate
    [SerializeField] private float turnInterval = 1f; // Interval for each turn (in seconds)
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

        // Determine the current tile
        currentTile = FindCurrentTile();

        // Reset the color of the current tile at the start of the turn
        currentTile.UpdateTileMaterial(false, 0);

        // If there's an existing path, continue from the last position
        if (path.Count > 0)
        {
            currentTile = path[path.Count - 1];
        }
        else
        {
            currentTile = FindCurrentTile();
        }

        // Set or update the target tile
        if (path.Count < 4 && path.Count > 0)
        {
            Tile previousTarget = path[path.Count - 1];
            ExtendPath(previousTarget, path.Count + 4); // Extend the path 4 more tiles beyond the previous target
        }
        else if (path.Count == 0)
        {
            ExtendPath(currentTile, 4); // Start a new path from the current tile
        }

        UpdateTileMaterials();
        enemyTurn = true;
    }
    private void ExtendPath(Tile current, int targetLength)
    {
        // Ensure the path extends off the current tile
        while (path.Count < targetLength)
        {
            List<Tile> validNeighbors = new List<Tile>();

            // Collect all valid neighboring tiles
            foreach (var direction in directions)
            {
                Vector2Int neighborPos = current.Position + direction;
                if (levelGeneration.grid.ContainsKey(neighborPos))
                {
                    Tile neighborTile = levelGeneration.grid[neighborPos];
                    if (neighborTile.IsWalkable && !path.Contains(neighborTile))
                    {
                        validNeighbors.Add(neighborTile);
                    }
                }
            }

            // Add a valid neighboring tile or backtrack if no valid neighbors
            if (validNeighbors.Count > 0)
            {
                Tile nextTile = validNeighbors[Random.Range(0, validNeighbors.Count)];
                path.Add(nextTile);
                current = nextTile;
            }
            else
            {
                // Backtrack only if there are no valid neighbors and more than one tile in the path
                if (path.Count > 1)
                {
                    current = path[path.Count - 2]; // Move to the previous tile
                    path.RemoveAt(path.Count - 1); // Remove the current tile from the path
                }
                else
                {
                    break; // No more moves possible
                }
            }
        }
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
  
    private bool CheckPlayerDetection()
    {
        // Check if the player is within a detection range
        float detectionRange = 10f; // Adjust the range as needed
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        Debug.Log($"Distance to player: {distanceToPlayer}");

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

        Debug.Log($"Casting ray from {transform.position} to {playerTransform.position}");

        // Cast a ray from the enemy's position towards the player
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange, layerMask))
        {
            // If the ray hits something, check if it's the player
            Debug.Log($"Ray hit: {hit.transform.name}");
            if (hit.transform == playerTransform)
            {
                playerDetected = true;
                Debug.Log("Player detected");
                return true; // The player is detected
            }
            else
            {
                Debug.Log("Ray hit an obstacle, not the player");
            }
        }

        playerDetected = false;
        Debug.Log("Player not detected");
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

        // Only move if the timer exceeds the interval
        if (turnTimer >= turnInterval && currentActionPoints > 0 && currentPath.Count > 0)
        {
            Tile nextTile = currentPath[0];
            int movementCost = GetMovementCost(currentTile, nextTile);

            if (!IsFacingTile(nextTile))
            {
                TurnToFaceTile(nextTile);
                turnTimer = 0f; // Reset the timer after turning
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

                turnTimer = 0f; // Reset the timer after moving
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