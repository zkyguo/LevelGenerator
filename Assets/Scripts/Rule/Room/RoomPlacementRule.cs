
using UnityEngine;

[CreateAssetMenu(fileName = "New Room Rule", menuName = "Room Rule/Room Place Rule")]
public class RoomPlacementRule : ScriptableObject
{
    public CellType topType;
    public CellType bottomType;
    public CellType leftType;
    public CellType rightType;
    public CellType frontType;
    public CellType backType;

    public Vector3 rotation;
    public GameObject prefab;
}


