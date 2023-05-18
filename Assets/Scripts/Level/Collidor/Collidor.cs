using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collidor : MonoBehaviour
{
    List<ICollidorCell> cells = new List<ICollidorCell>();
}

interface ICollidorCell 
{
    List<GameObject> Asset { get; set; }
}

public class Hallway : MonoBehaviour, ICollidorCell
{
    Cell cell;

    public List<GameObject> Asset { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
}

public class Stair : MonoBehaviour, ICollidorCell
{
    List<Cell> cells;

    public List<GameObject> Asset { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
}
