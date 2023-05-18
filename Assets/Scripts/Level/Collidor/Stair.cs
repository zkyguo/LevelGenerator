using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stair : CollidorRoom
{
    List<CollidorCell> cells;

    public List<CollidorCell> Cells { get => cells; set => cells = value; }

    public override void ApplyRules()
    {
        
    }

    public void InitiateStair(List<CollidorCell> _cells)
    {
        cells = _cells; 
    }


}
