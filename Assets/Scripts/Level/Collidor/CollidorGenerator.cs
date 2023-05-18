using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CollidorCell : Cell
{
    public Vector3 previousDirection;
    public Vector3 nextDirection;

    public CollidorCell(Vector3 _position, CellType _type) : base(_position, _type)
    {
    }

}

public class StairCell : CollidorCell
{
    public List<CollidorCell> cells;

    public StairCell(Vector3 _position, CellType _type, List<CollidorCell> _cells ) : base(_position, _type)
    {
        cells = _cells;
    }

    public Vector3 GetPreviousDirection()
    {
        return cells[0].previousDirection;
    }

    public Vector3 GetNextDirection()
    {
        return cells[3].nextDirection;
    }
}

public static class Directions
{
    public static List<Vector3Int> CubeDirections = new List<Vector3Int> {Vector3Int.down, Vector3Int.up, Vector3Int.back, Vector3Int.forward, Vector3Int.left, Vector3Int.right };
    public static List<Vector3Int> PlanDirections = new List<Vector3Int> { Vector3Int.back, Vector3Int.forward, Vector3Int.left, Vector3Int.right};

}


public class CollidorGenerator : Singleton
{
    private Dictionary<GameObject, GameObject> connectedRoom = new Dictionary<GameObject, GameObject>();
    private List<List<CollidorCell>> PathList = new List<List<CollidorCell>>();
    private List<Collidor> allCollidor = new List<Collidor>();
    [SerializeField]
    MyGridSystem grid;

    [SerializeField]
    GameObject CollidorPrefab;
    [SerializeField]
    GameObject HallwayPrefab;
    [SerializeField]
    GameObject StairPrefab;

    private List<GameObject> allCollidors = new List<GameObject>();

    [Button("Generate")]
    public void GenerateCollidors()
    {
        Clear();
        grid.ResetGrid();
        connectedRoom = SingletonManager.Instance.GetSingleton<PathGenerator>().getConnectRoom();
        foreach (var pair in connectedRoom)
        {
            List<CollidorCell> path = CollidorCalculator.FindPath(pair.Key.GetComponent<Room>(),
                                           pair.Value.GetComponent<Room>(), grid.GetGridCells()
                                           );
            PathList.Add(path);
        }
        GenerateCollidorAndStair();
    }

    [Button("Show")]
    void ShowRoad()
    {
        DrawRoad();
    }

    void GenerateCollidorAndStair()
    {
        foreach (var road in PathList)
        {
           
            if (road != null)
            {
                Collidor collidor = Instantiate(CollidorPrefab, road[0].Position, Quaternion.identity).GetComponent<Collidor>();
                for (int i = 0; i < road.Count; i++)
                {
                    CollidorCell cell = road[i];    
                    if (cell.CellType == CellType.Collidor)
                    {
                        GameObject obj = Instantiate(HallwayPrefab, road[i].Position, Quaternion.identity, collidor.gameObject.transform);
                        Hallway hallway = obj.GetComponent<Hallway>();   
                        hallway.InitiateHallway(cell);
                        collidor.AddCell(hallway);
                    }
                    else if (cell is StairCell)
                    {
                        StairCell stairCell = (StairCell)cell;

                        Vector3 normal = Vector3.Cross(stairCell.cells[1].Position - stairCell.cells[0].Position, stairCell.cells[2].Position - stairCell.cells[0].Position).normalized;
                        Quaternion rotation = Quaternion.LookRotation(normal);
                        Stair stair = Instantiate(StairPrefab, stairCell.cells[0].Position + (stairCell.cells[3].Position - stairCell.cells[0].Position) / 2, rotation, collidor.gameObject.transform).gameObject.GetComponent<Stair>();
                        stair.InitiateStair(stairCell.cells);
                        collidor.AddCell(stair);
 

                    }
                }
                collidor.ApplyRules();
                allCollidors.Add(collidor.gameObject);
            }
            

        }
    }

    void DrawRoad()
    {
        foreach (var road in PathList)
        {

            for (int i = 0; i < road.Count - 1; i++)
            {
                Debug.DrawLine(road[i].Position, road[i + 1].Position, Color.white, 2f);
            }

        }

    }

    void Clear()
    {

        PathList.Clear();
        foreach (var Collidor in allCollidors)
        {
            DestroyImmediate(Collidor);
        }
        allCollidors.Clear();
    }


}
