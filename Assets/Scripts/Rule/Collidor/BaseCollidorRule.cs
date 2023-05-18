using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Collidor Rule", menuName = "Collidor Rule/Collidor Rule")]
public class BaseCollidorRule : ScriptableObject
{
    public List<Vector3> directions = new List<Vector3>();
    public Vector3 rotation;
    public GameObject prefab;
}
