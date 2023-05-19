using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Collidor : SerializedMonoBehaviour
{
    public HashSet<CollidorRoom> Collidors = new HashSet<CollidorRoom>();

    public void ApplyRules()
    {
        foreach (var cell in Collidors)
        {
            cell.ApplyRules();
        }
    }

    public void AddCell(CollidorRoom cell)
    {
        Collidors.Add(cell);
    }

}