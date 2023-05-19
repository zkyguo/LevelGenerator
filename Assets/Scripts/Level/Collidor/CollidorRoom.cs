using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public abstract class CollidorRoom : SerializedMonoBehaviour
{

    public Vector3 position;
    public HashSet<BaseCollidorRule> rules = new HashSet<BaseCollidorRule>();
    public abstract void ApplyRules();

}