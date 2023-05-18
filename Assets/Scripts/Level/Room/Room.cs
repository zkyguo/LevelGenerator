using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door
{
    Cell doorCell;

    public Door(Cell doorCell)
    {
        this.doorCell = doorCell;
    }
}

public class Room : SerializedMonoBehaviour
{
    List<GameObject> assets = new List<GameObject>();
    Vector3 CentrePosition;
    List<Vector3> occupiedCells = new List<Vector3>();

    Vector3Int Size = new Vector3Int();
    String Name;

    Dictionary<Vector3, Cell> Cells = new Dictionary<Vector3, Cell>();
    Dictionary<Vector3, Vector3> doorCells = new Dictionary<Vector3, Vector3>();
    Dictionary<Vector3, Cell> allBoundary = new Dictionary<Vector3, Cell>();
    public HashSet<RoomPlacementRule> allRules = new HashSet<RoomPlacementRule>();

    MyGridSystem grid;

    public void setRoom(Vector3Int _size, List<Vector3> allNodeInside)
    {
        grid = SingletonManager.Instance.GetSingleton<MyGridSystem>();
        Size = _size;
        occupiedCells = allNodeInside;
        Name = transform.name;
        GetRandomBoundaryCell();
        ApplyRules();
    }

    public Vector3 GetRandomBoundaryCell()
    {

        // Randomly select one of the boundary cells
        if (allBoundary.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, allBoundary.Count);
            KeyValuePair<Vector3, Cell> keyValuePair = allBoundary.ElementAt(randomIndex);
            doorCells.Add(keyValuePair.Value.Position, keyValuePair.Key - keyValuePair.Value.Position);
            return keyValuePair.Key;
        }

        // Find all boundary cells
        foreach (var cell in occupiedCells)
        {
            Cell roomCell = new Cell(cell, CellType.Room);
            Cells.Add(cell, roomCell);
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
                    allBoundary.Add(neighbor, roomCell);
                }
            }
        }
        // Randomly select one of the boundary cells
        if (allBoundary.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, allBoundary.Count);
            KeyValuePair<Vector3, Cell> keyValuePair = allBoundary.ElementAt(randomIndex);
            doorCells.Add(keyValuePair.Value.Position, keyValuePair.Key - keyValuePair.Value.Position);
            return keyValuePair.Key;
        }

        return new Vector3();

    }

    private void ApplyRules()
    {
        foreach (var Cell in Cells)
        {
            CellType topType = CellType.Void;
            CellType bottomType = CellType.Void;
            CellType leftType = CellType.Void;
            CellType rightType = CellType.Void;
            CellType frontType = CellType.Void;
            CellType backType = CellType.Void;

            if (Cells.ContainsKey(Cell.Key + Vector3Int.up)) topType = CellType.Room;
            if (Cells.ContainsKey(Cell.Key + Vector3Int.down)) bottomType = CellType.Room;
            if (Cells.ContainsKey(Cell.Key + Vector3Int.left)) leftType = CellType.Room;
            if (Cells.ContainsKey(Cell.Key + Vector3Int.right)) rightType = CellType.Room;
            if (Cells.ContainsKey(Cell.Key + Vector3Int.forward)) frontType = CellType.Room;
            if (Cells.ContainsKey(Cell.Key + Vector3Int.back)) backType = CellType.Room;

            if (doorCells.ContainsKey(Cell.Key))
            {
                foreach (RoomPlacementRule rule in allRules)
                {
                    RoomDoorRule doorRule = rule as RoomDoorRule;
                    if (doorRule != null)
                    {
                        if (topType == rule.topType &&
                     bottomType == rule.bottomType &&
                     leftType == rule.leftType &&
                     rightType == rule.rightType &&
                     frontType == rule.frontType &&
                     backType == rule.backType &&
                     doorRule.direction == doorCells[Cell.Key])
                        {
                            Quaternion rotation = Quaternion.Euler(doorRule.rotation);
                            GameObject gameObject = Instantiate(doorRule.prefab, Cell.Key, rotation, this.gameObject.transform);
                            gameObject.name = doorRule.name;
                        }
                    }
                }
            }
            else
            {
                foreach (RoomPlacementRule rule in allRules)
                {
                    RoomDoorRule doorRule = rule as RoomDoorRule;
                    if (doorRule == null)
                    {
                        if (topType == rule.topType &&
                         bottomType == rule.bottomType &&
                         leftType == rule.leftType &&
                         rightType == rule.rightType &&
                         frontType == rule.frontType &&
                         backType == rule.backType)
                        {
                            Quaternion rotation = Quaternion.Euler(rule.rotation);
                            GameObject gameObject = Instantiate(rule.prefab, Cell.Key, rotation, this.gameObject.transform);
                            gameObject.name = rule.name;
                        }
                    }



                }
            }
        }

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
        allBoundary = null;
        doorCells = null;
        allRules = null;
    }

}
