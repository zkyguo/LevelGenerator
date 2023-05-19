
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Stair : CollidorRoom
{
    List<CollidorCell> cells;
    public HashSet<StairRule> Rules = new HashSet<StairRule>();
    public List<CollidorCell> Cells { get => cells; set => cells = value; }

    public override void ApplyRules()
    {
        foreach (var rule in Rules)
        {
            foreach (var dict in rule.Directions)
            {
                if (cells[0].previousDirection == dict.Key[0] && cells[3].nextDirection == dict.Key[1])
                {
                    Vector3 Direction = cells[3].Position - cells[0].Position;
                    Vector3 StairDirection = new Vector3(Direction.x, 0, Direction.z);
                    float EnterAngle = Vector3.SignedAngle(StairDirection, cells[0].previousDirection,Vector3.up);
                    float ExitAngle = Vector3.SignedAngle(StairDirection, cells[3].nextDirection, Vector3.up);
                    


                    if (((cells[3].Position.y > cells[0].Position.y) == dict.Value) && 
                        ((rule.Direction1[0] == EnterAngle && ExitAngle == rule.Direction1[1]) || (rule.Direction2[0] == EnterAngle && ExitAngle == rule.Direction2[1])) )
                    { 
                        Quaternion rotation = Quaternion.Euler(rule.rotation);
                        GameObject gameObject = Instantiate(rule.prefab, position, rotation, this.gameObject.transform);
                        gameObject.name = rule.name;
                    }   
                }
            }
        }
    }

    public void InitiateStair(List<CollidorCell> _cells)
    {
        cells = _cells;
        position = _cells[0].Position + (_cells[3].Position - _cells[0].Position) / 2;
    }
}
