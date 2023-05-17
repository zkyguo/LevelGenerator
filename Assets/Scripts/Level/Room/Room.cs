using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class Door
{
    Cell doorCell;

    public Door(Cell doorCell)
    {
        this.doorCell = doorCell;
    }   
}

public class Room : MonoBehaviour
{
    List<GameObject> assets = new List<GameObject>();
    Vector3 CentrePosition;
    List<Vector3> occupiedCells = new List<Vector3>();
    List<Vector3> boundaryCells = new List<Vector3>();
    Vector3Int Size = new Vector3Int();
    String Name;

    List<Cell> Cells = new List<Cell>();
    List<Door> doorCells = new List<Door>();

    MyGridSystem grid;

    public void setRoom(Vector3Int _size, List<Vector3> allNodeInside)
    {
        grid = SingletonManager.Instance.GetSingleton<MyGridSystem>();
        Size = _size;
        occupiedCells = allNodeInside;
        Name = transform.name;
    }

    public Vector3 GetRandomBoundaryCell()
    {

        // Randomly select one of the boundary cells
        if (boundaryCells.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, boundaryCells.Count);
            return boundaryCells[randomIndex];
        }

        // Find all boundary cells
        foreach (var cell in occupiedCells)
        {
            Cell roomCell = new Cell(cell, CellType.Room);
            Cells.Add(roomCell);
            // Check the six neighboring cells
            Vector3[] neighbors = new Vector3[]
            {
            new Vector3(cell.x + 1, cell.y, cell.z),
            new Vector3(cell.x - 1, cell.y, cell.z),
            new Vector3(cell.x, cell.y - 1, cell.z),
            new Vector3(cell.x, cell.y, cell.z + 1),
            new Vector3(cell.x, cell.y, cell.z - 1),
            };
            foreach (var neighbor in neighbors)
            {
                if (grid.GetGridCells().ContainsKey(neighbor) && !occupiedCells.Contains(neighbor))
                {
                    boundaryCells.Add(neighbor);          
                    doorCells.Add(new Door(roomCell));
                }
            }
        }
        // Randomly select one of the boundary cells
        if (boundaryCells.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, boundaryCells.Count);
            return boundaryCells[randomIndex];
        }

        return new Vector3();

    }

    public void SetCentrePosition(Vector3 position)
    {
        CentrePosition = position;
    }

    public Vector3 GetCentrePosition()
    {
        return CentrePosition;
    }

    /*private void OnDrawGizmos()
    {
        foreach (var cell in boundaryCells)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(cell, 0.2f);
        }
    }*/


    private void OnDestroy()
    {
        assets = null;
        occupiedCells = null;
        boundaryCells = null;
    }

}
