using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Collidor Rule", menuName = "Collidor Rule/Collidor Rule")]
public class BaseCollidorRule : SerializedScriptableObject
{
    public List<List<Vector3>> directions = new List<List<Vector3>>();
    public Vector3 rotation;
    public GameObject prefab;
}

