using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MyGridSystem : MonoBehaviour
{
    public int xSize = 10;
    public int ySize = 10;
    public int zSize = 10;

    /// <summary>
    /// <position, is cell void>
    /// </summary>
    public Dictionary<Vector3Int, bool> grid = new Dictionary<Vector3Int, bool>();

    [Button("Show Grid")]
    void ShowGrid()
    {
        Initialize();
        DrawGrid();
    }

    void Initialize()
    {
        grid.Clear();
        for (int x = -xSize; x < xSize; x++)
        {
            for (int y = -ySize; y < ySize; y++)
            {
                for (int z = -zSize; z < zSize; z++)
                {
                    grid.Add(new Vector3Int(x,y,z), false);
                }
            }
        }
    }

    public bool IsAreaEmpty(Vector3Int start, Vector3Int size)
    {
        for (int x = start.x; x < start.x + size.x; x++)
        {
            for (int y = start.y; y < start.y + size.y; y++)
            {
                for (int z = start.z; z < start.z + size.z; z++)
                {
                    Vector3Int currentPosition = new Vector3Int(x, y, z);
                    if (!grid.ContainsKey(currentPosition) || grid[currentPosition])
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public Vector3Int? FindArea(Vector3Int size)
    {
        for (int x = -xSize; x < xSize; x++)
        {
            for (int y = -ySize; y < ySize; y++)
            {
                for (int z = -zSize; z < zSize; z++)
                {
                    Vector3Int start = new Vector3Int(x, y, z);
                    if (IsAreaEmpty(start, size))
                    {
                        return start;
                    }
                }
            }
        }
        return null;
    }

    public bool PlaceObject(Vector3Int size)
    {
        Vector3Int? startPosition = FindArea(size);
        if (startPosition.HasValue)
        {
            for (int x = startPosition.Value.x; x < startPosition.Value.x + size.x; x++)
            {
                for (int y = startPosition.Value.y; y < startPosition.Value.y + size.y; y++)
                {
                    for (int z = startPosition.Value.z; z < startPosition.Value.z + size.z; z++)
                    {
                        grid[new Vector3Int(x, y, z)] = true;
                    }
                }
            }
            return true;
        }
        return false;
    }

    void DrawGrid()
    {
        foreach (KeyValuePair<Vector3Int, bool> item in grid)
        {
            Vector3Int coord = item.Key;
            Debug.DrawLine(new Vector3(coord.x, coord.y, coord.z), new Vector3(coord.x + 1, coord.y, coord.z), Color.white,1f);
            Debug.DrawLine(new Vector3(coord.x, coord.y, coord.z), new Vector3(coord.x, coord.y + 1, coord.z), Color.white,1f);
            Debug.DrawLine(new Vector3(coord.x, coord.y, coord.z), new Vector3(coord.x, coord.y, coord.z + 1), Color.white,1f);
        }
    }
}
