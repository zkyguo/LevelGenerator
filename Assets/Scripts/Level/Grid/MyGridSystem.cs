using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MyGridSystem : MonoBehaviour
{
    [SerializeField]
    private int xSize;
    [SerializeField]
    private int ySize;
    [SerializeField]
    private int zSize;

    /// <summary>
    /// <position, is cell void>
    /// </summary>
    public Dictionary<Vector3, bool> gridCells = new Dictionary<Vector3, bool>();


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
        if(gridCells.Count == 0)
        {
            Initialize();
        }
        if (prefab)
        {
            ExtendGrid(worldPosition, size); //Check if need extend grid
            Vector3 offset = new Vector3(size.x % 2 == 0 ? 0 : 0.5f, size.y % 2 == 0 ? 0 : 0.5f, size.z % 2 == 0 ? 0 : 0.5f);
            GameObject gameObject = Instantiate(prefab, worldPosition + offset, Quaternion.identity);
            gameObject.transform.localScale = size;

            prefab.GetComponent<Room>().setRoom(size, PlaceObject(worldPosition, size));
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
        return gridCells[WorldToGrid(WorldPosition)];
    }
    #endregion

    [Button("Show Grid")]
    void ShowGrid()
    {
        Initialize();
    }

    void Initialize()
    {
        
        for (int x = -xSize; x < xSize; x++)
        {
            for (int y = -ySize; y < ySize; y++)
            {
                for (int z = -zSize; z < zSize; z++)
                {
                    gridCells.Add(new Vector3(x + 0.5f,y+0.5f,z+0.5f), false);
                }
            }
        }
    }

    /// <summary>
    /// Check if Cell is void
    /// </summary>
    /// <param name="GridPosition"></param>
    /// <returns></returns>
    bool IsCellEmpty(Vector3 GridPosition)
    {
        return gridCells[GridPosition];
    }

    /// <summary>
    /// Update object placed to grid dictionnary
    /// </summary>
    /// <param name="placePosition"></param>
    /// <param name="size"></param>
    List<Vector3> PlaceObject(Vector3Int placePosition, Vector3Int size)
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
                    cells.Add(WorldToGrid(cell));
                    gridCells[WorldToGrid(cell)] = true;
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
                    gridCells[WorldToGrid(new Vector3Int(x, y, z))] = false;
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
                    gridCells[new Vector3(xSize + 1 + 0.5f, y + 0.5f, z + 0.5f)] = false;
                    gridCells[new Vector3(-xSize - 1 + 0.5f, y + 0.5f, z + 0.5f)] = false;
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
                    gridCells[new Vector3(x + 0.5f, ySize + 1 + 0.5f, z + 0.5f)] = false;
                    gridCells[new Vector3(x + 0.5f, -ySize - 1 + 0.5f, z + 0.5f)] = false;
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
                    gridCells[new Vector3(x + 0.5f, y + 0.5f, zSize + 1 + 0.5f)] = false;
                    gridCells[new Vector3(x + 0.5f, y + 0.5f, -zSize - 1 + 0.5f)] = false;
                }
            }
            zSize++;
        }
    }

    /*void OnDrawGizmos()
    {
        foreach (KeyValuePair<Vector3, bool> item in grid)
        {
            Vector3 coord = item.Key;
            if (item.Value)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.blue;
            }
            Gizmos.DrawSphere(coord, 0.1f);
        }
    }*/

    void DrawGrid()
    {
        foreach (KeyValuePair<Vector3, bool> item in gridCells)
        {
            Vector3 coord = item.Key;
            Debug.DrawLine(new Vector3(coord.x, coord.y, coord.z), new Vector3(coord.x + 1, coord.y, coord.z), Color.white,1f);
            Debug.DrawLine(new Vector3(coord.x, coord.y, coord.z), new Vector3(coord.x, coord.y + 1, coord.z), Color.white,1f);
            Debug.DrawLine(new Vector3(coord.x, coord.y, coord.z), new Vector3(coord.x, coord.y, coord.z + 1), Color.white,1f);
        }
    }

    
}
