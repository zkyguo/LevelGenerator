using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class Collidor : MonoBehaviour
{
    Vector3 position;
}

public class Hallway : Collidor
{
    Cell cell;

}

public class Stair : Collidor
{
    List<Cell> cells;
}

public class CollidorGenerator : Singleton
{
    private Dictionary<GameObject, GameObject> connectedRoom = new Dictionary<GameObject, GameObject>();
    private List<List<Vector3>> PathList = new List<List<Vector3>>();
    private List<Collidor> allCollidor = new List<Collidor>();
    [SerializeField]
    MyGridSystem grid;

    [SerializeField]
    GameObject CollidorPrefab;
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
            List<Vector3> path = CollidorCalculator.FindPath(pair.Key.GetComponent<Room>(),
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
                for (int i = 0; i < road.Count; i++)
                {
                    if (grid.GetGridCells()[road[i]].CellType == CellType.Collidor)
                    {
                        allCollidors.Add(Instantiate(CollidorPrefab, road[i], Quaternion.identity));
                    }
                    else if (grid.GetGridCells()[road[i]].CellType == CellType.Stair)
                    {

                        Vector3 normal = Vector3.Cross(road[i + 1] - road[i], road[i + 2] - road[i]).normalized;
                        Quaternion rotation = Quaternion.LookRotation(normal);
                        allCollidors.Add(Instantiate(StairPrefab, road[i] + (road[i + 3] - road[i]) / 2, rotation));
                        i = i + 3;

                    }
                }
            }

        }
    }

    void DrawRoad()
    {
        foreach (var road in PathList)
        {

            for (int i = 0; i < road.Count - 1; i++)
            {
                Debug.DrawLine(road[i], road[i + 1], Color.white, 2f);
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
