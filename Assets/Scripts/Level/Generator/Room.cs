using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Room : MonoBehaviour
{
    List<GameObject> assets = new List<GameObject>();
    Vector3 CentrePosition;

    List<Vector3> occupiedCells = new List<Vector3> ();
    [SerializeField]
    List<Vector3> boundaryCells = new List<Vector3>();

    Vector3Int Size = new Vector3Int();
    String Name;

    MyGridSystem grid;

    public void setRoom(Vector3Int _size, List<Vector3> allNodeInside)
    {
        Size = _size;
        CentrePosition = this.transform.localPosition;
        occupiedCells = allNodeInside;
        Name = transform.name;
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<MyGridSystem>();
        //GetRandomBoundaryCell();
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
                if (grid.gridCells.ContainsKey(neighbor) && !occupiedCells.Contains(neighbor))
                {
                    boundaryCells.Add(neighbor);
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

    private void OnDrawGizmos()
    {
        foreach (var cell in boundaryCells)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(cell, 0.1f);
        }
    }

    private void OnDestroy()
    {
        assets = null;
        occupiedCells = null;
        boundaryCells = null;
    }

}
