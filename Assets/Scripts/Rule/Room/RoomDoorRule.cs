using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Door Rule", menuName = "Room Rule/Room Door Rule")]
public class RoomDoorRule : RoomPlacementRule
{
    public Vector3 direction;
}