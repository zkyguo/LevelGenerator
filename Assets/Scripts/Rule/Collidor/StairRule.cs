using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stair Rule", menuName = "Collidor Rule/Stair Rule")]
public class StairRule : SerializedScriptableObject
{
    [DictionaryDrawerSettings(KeyLabel = "Directions", ValueLabel = "isNextHigh")]
    public Dictionary<List<Vector3>, bool> Directions = new Dictionary<List<Vector3>, bool>();
    public Vector3 rotation;
    public GameObject prefab;
    public List<float> Direction1 = new List<float>();
    public List<float> Direction2 = new List<float>();
}
