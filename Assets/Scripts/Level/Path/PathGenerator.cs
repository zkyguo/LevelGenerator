using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PathGenerator : MonoBehaviour
{
    public List<GameObject> cubes; // Your list of cubes
    public Vector2Int offset;

    private Dictionary<GameObject, float> minDistance = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, GameObject> connectedRoom = new Dictionary<GameObject, GameObject>();
    private HashSet<GameObject> visitedCubes = new HashSet<GameObject>();

    [Button("Generate Path")]
    void GeneratePath()
    {
        AddRoomsInScene();
        FindAllConnection();
        FindAllPointDepartAndEnd();
    }

    /// <summary>
    /// Use to determinate which room are connected together
    /// </summary>
    
    void FindAllConnection()
    {
        
        foreach (GameObject cube in cubes)
        {
            minDistance[cube] = float.MaxValue;
        }

        minDistance[cubes[0]] = 0;

        while (minDistance.Count > 0)
        {
            GameObject currentCube = GetCubeWithMinDistance();
            visitedCubes.Add(currentCube);

            foreach (GameObject neighborCube in cubes)
            {
                if (visitedCubes.Contains(neighborCube)) continue;

                float distance = Vector3.Distance(currentCube.transform.position, neighborCube.transform.position);
                distance += Random.Range(offset.x, offset.y); // Add a random offset to the distance
                if (distance < minDistance[neighborCube])
                {
                    minDistance[neighborCube] = distance;
                    connectedRoom[neighborCube] = currentCube;
                }
            }

            minDistance.Remove(currentCube);
        }

        DrawLines();
    }

    GameObject GetCubeWithMinDistance()
    {
        GameObject minDistanceCube = null;
        float minValue = float.MaxValue;

        foreach (KeyValuePair<GameObject, float> pair in minDistance)
        {
            if (pair.Value < minValue)
            {
                minValue = pair.Value;
                minDistanceCube = pair.Key;
            }
        }

        return minDistanceCube;
    }

    void FindAllPointDepartAndEnd()
    {

    }


    void DrawLines()
    {
        foreach (KeyValuePair<GameObject, GameObject> pair in connectedRoom)
        {
            Debug.DrawLine(pair.Key.transform.position, pair.Value.transform.position, Color.red, 0.5f);
        }
    }

    /// <summary>
    /// Add all room in scene and reset all data
    /// </summary>
    private void AddRoomsInScene()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Room");
        cubes.Clear();
        minDistance.Clear();
        connectedRoom.Clear();
        visitedCubes.Clear();
        foreach (GameObject obj in objects)
        {
            cubes.Add(obj);
        }
    }
}
