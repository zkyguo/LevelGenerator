using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrahedron
{
    public Vector3Int A { get; private set; }
    public Vector3Int B { get; private set; }
    public Vector3Int C { get; private set; }
    public Vector3Int D { get; private set; }

    public Tetrahedron(Vector3Int a, Vector3Int b, Vector3Int c, Vector3Int d)
    {
        A = a;
        B = b;
        C = c;
        D = d;
    }

    public Vector3Int[] Vertices
    {
        get { return new Vector3Int[] { A, B, C, D }; }
    }

    // TODO ? Additional methods to calculate properties like volume, centroid, etc. can be added here
}
