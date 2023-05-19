using System.Collections.Generic;
using UnityEngine;

public class Hallway : CollidorRoom
{
    CollidorCell cell;

    public void InitiateHallway(CollidorCell _cell)
    {
        cell = _cell;
    }

    public override void ApplyRules()
    {
        foreach (var rule in rules)
        {

            foreach (List<Vector3> directions in rule.directions)
            {
                if (cell.previousDirection == directions[0] && cell.nextDirection == directions[1])
                {
                    Quaternion rotation = Quaternion.Euler(rule.rotation);
                    GameObject gameObject = Instantiate(rule.prefab, cell.Position, rotation, this.gameObject.transform);
                    gameObject.name = rule.name;
                    continue;
                }
            }
        }

    }

    public CollidorCell Cell { get => cell; set => cell = value; }
}
