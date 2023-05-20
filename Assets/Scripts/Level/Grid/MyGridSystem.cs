using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public enum CellType
{
    Void, Room, Collidor, Stair
}

public class Cell
{
    Vector3 position;
    CellType cellType;

    public Cell(Vector3 _position, CellType _type)
    {
        position = _position;
        cellType = _type;
    }

    public Vector3 Position { get => position; set => position = value; }
    public CellType CellType { get => cellType; set => cellType = value; }
}

public class MyGridSystem : Singleton
{
    [SerializeField]
    private int xSize;
    [SerializeField]
    private int ySize;
    [SerializeField]
    private int zSize;

    public static int Unit = 1;

    Dictionary<Vector3, Cell> gridCells = new Dictionary<Vector3, Cell>();
    Dictionary<Vector3, Cell> initialCells = new Dictionary<Vector3, Cell>();

    List<Room> allRoom = new List<Room>();
    List<Collidor> allCollidor = new List<Collidor>();

    #region Public

    /// <summary>
    /// Set object on Grid
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="size"></param>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public GameObject SetObjectAt(GameObject prefab, Vector3Int size, Vector3Int worldPosition)
    {
        if (prefab)
        {
            ExtendGrid(worldPosition, size); //Check if need extend grid
            Vector3 offset = new Vector3(size.x % 2 == 0 ? 0 : 0.5f, size.y % 2 == 0 ? 0 : 0.5f, size.z % 2 == 0 ? 0 : 0.5f);
            GameObject gameObject = Instantiate(prefab, worldPosition + offset, Quaternion.identity);
            gameObject.transform.localScale = size;
            gameObject.GetComponent<Room>().setRoom(size, PlaceObject(worldPosition, size, gameObject.GetComponent<Room>()));
            return gameObject;
        }
        RemoveObject(size, worldPosition);
        return null;

    }

    /// <summary>
    /// Clear grid data
    /// </summary>
    public void Clear()
    {
        gridCells.Clear();
    }

    /// <summary>
    /// Check if position is void
    /// </summary>
    /// <param name="WorldPosition"></param>
    /// <returns></returns>
    public bool IsPositionEmpty(Vector3 WorldPosition)
    {
        return gridCells[WorldToGrid(WorldPosition)].CellType == CellType.Void;
    }
    #endregion

    /// <summary>
    /// Initialize Grid by size
    /// </summary>
    /// <param name="size"></param>
    public void Initialize(Vector3Int size)
    {
        xSize = size.x + 1;
        ySize = size.y + 1;
        zSize = size.z + 1;

        for (int x = -xSize; x < xSize; x++)
        {
            for (int y = -ySize; y < ySize; y++)
            {
                for (int z = -zSize; z < zSize; z++)
                {
                    Vector3 position = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
                    gridCells.Add(position, new Cell(position,CellType.Void));
                }
            }
        }
        initialCells = gridCells;
    }

    /// <summary>
    /// Check if Cell is void
    /// </summary>
    /// <param name="GridPosition"></param>
    /// <returns></returns>
    bool IsCellEmpty(Vector3 GridPosition)
    {
        return gridCells[GridPosition].CellType == CellType.Void;
    }

    /// <summary>
    /// Update object placed to grid dictionnary
    /// </summary>
    /// <param name="placePosition"></param>
    /// <param name="size"></param>
    List<Vector3> PlaceObject(Vector3Int placePosition, Vector3Int size, Room room)
    {
        Vector3Int halfSize = new Vector3Int(size.x / 2, size.y / 2, size.z / 2);
        Vector3Int start = placePosition - halfSize;
        Vector3Int end = start + size;
        List<Vector3> cells = new List<Vector3>();

        for (int x = start.x; x < end.x; x++)
        {
            for (int y = start.y; y < end.y; y++)
            {
                for (int z = start.z; z < end.z; z++)
                {
                    Vector3 cell = new Vector3Int(x,y,z);
                    if(cell == placePosition)
                    {
                        room.SetCentrePosition(WorldToGrid(cell));
                    }
                    cells.Add(WorldToGrid(cell));
                    gridCells[WorldToGrid(cell)].CellType = CellType.Room;
                    allRoom.Add(room);
                }
            }
        }

        return cells;
    }

    /// <summary>
    /// Update object placed to grid dictionnary
    /// </summary>
    /// <param name="placePosition"></param>
    /// <param name="size"></param>
    void RemoveObject(Vector3Int placePosition, Vector3Int size)
    {
        Vector3Int halfSize = new Vector3Int(size.x / 2, size.y / 2, size.z / 2);
        Vector3Int start = placePosition - halfSize;
        Vector3Int end = start + size;

        for (int x = start.x; x < end.x; x++)
        {
            for (int y = start.y; y < end.y; y++)
            {
                for (int z = start.z; z < end.z; z++)
                {
                    gridCells[WorldToGrid(new Vector3Int(x, y, z))].CellType = CellType.Void;
                }
            }
        }
    }

    /// <summary>
    /// Convert WordPosition to Grid position
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Vector3 WorldToGrid(Vector3 worldPosition)
    {
        return (worldPosition) + new Vector3(0.5f, 0.5f, 0.5f);
    }

    /// <summary>
    /// Check if new added position has surpass grid. If yes, extend grid
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <param name="size"></param>
    void ExtendGrid(Vector3 worldPosition, Vector3Int size)
    {
        if(worldPosition.x + size.x / 2 > xSize || worldPosition.x - size.x / 2 < -xSize)
        {
            for (int y = -ySize; y <= ySize; y++)
            {
                for (int z = -zSize; z <= zSize; z++)
                {
                    gridCells[new Vector3(xSize + 1 + 0.5f, y + 0.5f, z + 0.5f)].CellType = CellType.Void;
                    gridCells[new Vector3(-xSize - 1 + 0.5f, y + 0.5f, z + 0.5f)].CellType = CellType.Void;
                }
            }
            xSize++;
        }
        if (worldPosition.y + size.y / 2 > ySize || worldPosition.y - size.y / 2 < -ySize)
        {
            for (int x = -xSize; x <= xSize; x++)
            {
                for (int z = -zSize; z <= zSize; z++)
                {
                    gridCells[new Vector3(x + 0.5f, ySize + 1 + 0.5f, z + 0.5f)].CellType = CellType.Void;
                    gridCells[new Vector3(x + 0.5f, -ySize - 1 + 0.5f, z + 0.5f)].CellType = CellType.Void;
                }
            }
            ySize++;
        }
        if (worldPosition.z + size.z / 2 > zSize || worldPosition.z - size.z / 2 < -zSize)
        {
            for (int x = -xSize; x <= xSize; x++)
            {
                for (int y = -ySize; y <= ySize; y++)
                {
                    gridCells[new Vector3(x + 0.5f, y + 0.5f, zSize + 1 + 0.5f)].CellType = CellType.Void;
                    gridCells[new Vector3(x + 0.5f, y + 0.5f, -zSize - 1 + 0.5f)].CellType = CellType.Void;
                }
            }
            zSize++;
        }
    }

    /// <summary>
    /// Extend Grid by 1 unit
    /// </summary>
    public void ExtendGrid()
    {
        xSize++;
        ySize++;
        zSize++;

        // Expand in X direction
        for (int y = -ySize; y <= ySize; y++)
        {
            for (int z = -zSize; z <= zSize; z++)
            {
                gridCells[new Vector3(xSize, y, z)].CellType = CellType.Void;
            }
        }
        for (int y = -ySize; y <= ySize; y++)
        {
            for (int z = -zSize; z <= zSize; z++)
            {
                gridCells[new Vector3(-xSize, y, z)].CellType = CellType.Void;
            }
        }

        // Expand in Y direction
        for (int x = -xSize; x <= xSize; x++)
        {
            for (int z = -zSize; z <= zSize; z++)
            {
                gridCells[new Vector3(x, ySize, z)].CellType = CellType.Void;
            }
        }
        for (int x = -xSize; x <= xSize; x++)
        {
            for (int z = -zSize; z <= zSize; z++)
            {
                gridCells[new Vector3(x, -ySize, z)].CellType = CellType.Void;
            }
        }
        

        // Expand in Z direction
        for (int x = -xSize; x <= xSize; x++)
        {
            for (int y = -ySize; y <= ySize; y++)
            {
                gridCells[new Vector3(x, y, zSize)].CellType = CellType.Void;
            }
        }
        for (int x = -xSize; x <= xSize; x++)
        {
            for (int y = -ySize; y <= ySize; y++)
            {
                gridCells[new Vector3(x, y, -zSize - 1)].CellType = CellType.Void;
            }
        }
        
    }

    /// <summary>
    /// Draw grid on scene
    /// </summary>
    #region Draw Grid

    /*void OnDrawGizmos()
    {
        foreach (KeyValuePair<Vector3, CellType> item in gridCells)
        {

            if (item.Value == CellType.Collidor)
            {

                Vector3 coord = item.Key;
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(coord, 0.1f);
            }
            else if (item.Value == CellType.Void)
            {

                Vector3 coord = item.Key;
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(coord, 0.1f);
            }
            else if (item.Value == CellType.Stair)
            {

                Vector3 coord = item.Key;
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(coord, 0.1f);
            }
        }
    }*/

    [Button("Show Room")]
    void DrawRoom()
    {
        foreach (KeyValuePair<Vector3, Cell> item in gridCells)
        {
            if(item.Value.CellType == CellType.Room)
            {
                Vector3 coord = item.Key;
                Debug.DrawLine(new Vector3(coord.x, coord.y, coord.z), new Vector3(coord.x + 1, coord.y, coord.z), Color.white, 1f);
                Debug.DrawLine(new Vector3(coord.x, coord.y, coord.z), new Vector3(coord.x, coord.y + 1, coord.z), Color.white, 1f);
                Debug.DrawLine(new Vector3(coord.x, coord.y, coord.z), new Vector3(coord.x, coord.y, coord.z + 1), Color.white, 1f);
            }

        }
    }
    [Button("Show Stair")]
    void DrawStair()
    {
        foreach (KeyValuePair<Vector3, Cell> item in gridCells)
        {
            if (item.Value.CellType == CellType.Stair)
            {
                Vector3 coord = item.Key;
                Debug.DrawLine(new Vector3(coord.x, coord.y, coord.z), new Vector3(coord.x + 1, coord.y, coord.z), Color.red, 1f);
                Debug.DrawLine(new Vector3(coord.x, coord.y, coord.z), new Vector3(coord.x, coord.y + 1, coord.z), Color.red, 1f);
                Debug.DrawLine(new Vector3(coord.x, coord.y, coord.z), new Vector3(coord.x, coord.y, coord.z + 1), Color.red, 1f);
            }

        }
    }

    [Button("Show Collidor")]
    void DrawCollidor()
    {
        foreach (KeyValuePair<Vector3, Cell> item in gridCells)
        {
            if (item.Value.CellType == CellType.Collidor)
            {
                Vector3 coord = item.Key;
                Debug.DrawLine(new Vector3(coord.x, coord.y, coord.z), new Vector3(coord.x + 1, coord.y, coord.z), Color.red, 1f);
                Debug.DrawLine(new Vector3(coord.x, coord.y, coord.z), new Vector3(coord.x, coord.y + 1, coord.z), Color.red, 1f);
                Debug.DrawLine(new Vector3(coord.x, coord.y, coord.z), new Vector3(coord.x, coord.y, coord.z + 1), Color.red, 1f);
            }

        }
    }
    #endregion  


    public Dictionary<Vector3, Cell> GetGridCells()
    {
        return gridCells;
    }

    public void SetCellType(Vector3 pos, CellType type)
    {
        gridCells[pos].CellType = type;
    }
    
    /// <summary>
    /// Reset grid to initial status
    /// </summary>
    public void ResetGrid()
    {
        gridCells = new Dictionary<Vector3,Cell>(initialCells);
    }
}
