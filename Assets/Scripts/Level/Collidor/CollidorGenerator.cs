using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollidorGenerator : Singleton
{
    private Dictionary<GameObject, GameObject> connectedRoom = new Dictionary<GameObject, GameObject>();
    [SerializeField]
    private List<List<Vector3>> Path = new List<List<Vector3>>();

    [Button("Generate")]
    void GenerateCollidors()
    {
        Clear();
        connectedRoom = SingletonManager.Instance.GetSingleton<PathGenerator>().getConnectRoom();

        foreach (var pair in connectedRoom)
        {
            Path.Add(CollidorCalculator.FindPath(pair.Key.GetComponent<Room>(),
                                           pair.Value.GetComponent<Room>(),
                                           SingletonManager.Instance.GetSingleton<MyGridSystem>()));
        }
       
    }

    [Button("Show")]
    void ShowRoad()
    {
        DrawRoad();
    }

    void DrawRoad()
    {
        foreach (var road in Path)
        {
            for (int i = 0; i < road.Count - 1; i++)
            {
                Debug.DrawLine(road[i], road[i + 1], Color.red, 2f);
            }
        }

    }

    void Clear()
    {
        Path.Clear();
    }
}
