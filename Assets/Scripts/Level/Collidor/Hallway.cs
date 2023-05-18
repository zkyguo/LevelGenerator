using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hallway : CollidorRoom
{
    CollidorCell cell;

    public void InitiateHallway(CollidorCell _cell)
    {
         cell = _cell;
        position = _cell.Position;
    }

    public override void ApplyRules()
    {
        foreach (var rule in rule)
        {
            if(rule.directions.Contains(cell.nextDirection) && rule.directions.Contains(cell.previousDirection))
            {
                if(cell.nextDirection.x != 0)
                {
                    Quaternion rotation = Quaternion.Euler(rule.rotation);
                    GameObject gameObject = Instantiate(rule.prefab, cell.Position, rotation, this.gameObject.transform);
                    gameObject.name = rule.name;
                }
            }
        }
    }

    public CollidorCell Cell { get => cell; set => cell = value; }
}
