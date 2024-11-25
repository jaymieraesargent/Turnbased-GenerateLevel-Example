using UnityEngine;

/*
    public class Tile : MonoBehaviour: Tile inherits from MonoBehaviour, making it a Unity component. 
    This means it can be attached to GameObjects in Unity and leverage Unity’s lifecycle functions if needed.
*/
public class Tile : MonoBehaviour
{
    public MeshRenderer meshRenderer;  // The MeshRenderer component for this tile
    public Color defaultColor = Color.white; // Default color if not part of the path
    public Gradient pathColorGradient; // Gradient for coloring the path tiles
    private Material _instanceMaterial; // Instance of the material for this tile

    #region Private Fields
    private Vector2Int _position; // Vector2Int _position: Stores the tile's grid position as an x and y integer coordinate.
    private bool _isWalkable; // bool _isWalkable: Determines if the tile can be walked on (e.g., tiles like water or walls may be unwalkable).
    private bool _isOccupied; //bool _isOccupied: Indicates if the tile is currently occupied, such as by a character or object.

    #endregion
    #region Public Properties
    // Position: A public property for accessing and setting _position. This allows controlled access to the position variable.
    public Vector2Int Position
    {
        get
        {
            return _position;
        }
        set
        {
            _position = value;
        }
    }
    // IsWalkable: A public property for accessing and setting _isWalkable.
    public bool IsWalkable
    {
        get
        {
            return _isWalkable;
        }
        set
        {
            _isWalkable = value;
        }
    }
    // IsOccupied: A public property for accessing and setting _isOccupied.
    public bool IsOccupied
    {
        get
        {
            return _isOccupied;
        }
        set
        {
            _isOccupied = value;
        }
    }
    #endregion
    #region Neighbor References
    /*
       neighborNorth, neighborSouth, neighborEast, neighborWest: 
       These are Tile references representing the neighboring tiles in each direction. 
       They’re initialized as null but can be set later, 
       allowing the game to understand the tile’s surroundings and support pathfinding, adjacency checks, or grid navigation.
    */
    public Tile neighborNorth = null;
    public Tile neighborSouth = null;
    public Tile neighborEast = null;
    public Tile neighborWest = null;
    #endregion
    #region Costs for A* pathfinding
    public int CostFromStart { get; set; } // represents the movement cost accumulated from the start tile to the current tile
    public int EstimatedCostToTarget { get; set; } // is the heuristic estimation from the current tile to the target tile
    public int TotalCost => CostFromStart + EstimatedCostToTarget; // is the sum of CostFromStart and EstimatedCostToTarget, providing a way to prioritize tiles in the pathfinding algorithm
    #endregion
    #region Parent tile for path reconstruction
    public Tile ParentTile { get; set; } //is a reference to the tile from which the current tile was reached during the pathfinding process.
    #endregion
    #region Methods
    // SetTile(Vector2Int position, bool isWalkable): This method initializes the tile by setting its position, walkability, and marking it as unoccupied.
    public void SetTile(Vector2Int position, bool isWalkable)
    {
        // Position = position; sets the tile’s position.
        Position = position;
        // IsWalkable = isWalkable; sets whether the tile is walkable.
        IsWalkable = isWalkable;
        // IsOccupied = false; ensures that the tile starts unoccupied by default.
        IsOccupied = false;
    }
    #endregion
    // Update the material based on whether this tile is in the path
    public void UpdateTileMaterial(bool isPathTile, float pathPositionRatio)
    {
        if (isPathTile)
        { 
            // Clamp pathPositionRatio between 0 and 1 to avoid invalid values
            pathPositionRatio = Mathf.Clamp(pathPositionRatio, 0f, 1f);
            // Get the color based on the path's position within the gradient
            Color color = pathColorGradient.Evaluate(pathPositionRatio);
            meshRenderer.material.color = color;
        }
        else
        {
            // Set back to default color if not part of the path
            meshRenderer.material.color = defaultColor;
        }
    }
    private void Start()
    {
        // Instantiate a new material based on the original one so we can modify it
        _instanceMaterial = new Material(meshRenderer.material);
        meshRenderer.material = _instanceMaterial; // Apply the new material instance to the mesh
    }
}