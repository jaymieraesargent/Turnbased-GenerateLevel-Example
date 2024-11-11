using UnityEngine;

/*
    public class Tile : MonoBehaviour: Tile inherits from MonoBehaviour, making it a Unity component. 
    This means it can be attached to GameObjects in Unity and leverage Unity’s lifecycle functions if needed.
*/
public class Tile : MonoBehaviour
{
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
}