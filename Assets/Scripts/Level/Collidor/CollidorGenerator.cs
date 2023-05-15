using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;

public class CollidorGenerator : Singleton
{
    

    private Dictionary<GameObject, GameObject> connectedRoom = new Dictionary<GameObject, GameObject>();
    private List<List<Vector3>> PathList = new List<List<Vector3>>();
    [SerializeField]
    MyGridSystem grid;
    Dictionary<Vector3, CellType> initialGrid; 

    [Button("Generate")]
    void GenerateCollidors()
    {
        Clear();
        connectedRoom = SingletonManager.Instance.GetSingleton<PathGenerator>().getConnectRoom();
        foreach (var pair in connectedRoom)
        {
            List<Vector3> path = CollidorCalculator.FindPath(pair.Key.GetComponent<Room>(),
                                           pair.Value.GetComponent<Room>(), grid.GetGridCells()
                                           );
            PathList.Add(path);
        }
    }

    [Button("Show")]
    void ShowRoad()
    {
        DrawRoad();
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
    }

    void resetPath()
    {
        foreach (var path in PathList)
        {
            if(path?.Count > 0)
            {
                foreach (var pos in path)
                {
                    grid.SetCell(pos, CellType.Void);
                }
            }
        }
    }

    
}
