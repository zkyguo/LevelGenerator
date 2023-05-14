using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum GenerationType
{
    TwoDimension, ThreeDimension
}

/// <summary>
/// Determinate placement of rooms
/// </summary>
public class DungeonGenerator : Singleton
{
    
    public GameObject cubePrefab;
    public GenerationType type;
    public int numberOfCubes = 10;
    [SerializeField]
    private Vector3Int minSize = new Vector3Int(2, 1, 2);
    [SerializeField]
    private Vector3Int maxSize = new Vector3Int(5, 2, 5);
    public float minDistance = 1.0f;
    public float maxDistance = 5.0f;
    [SerializeField]
    public Vector3Int boundsRadius;
    public int maxIteration = 10;

    private List<GameObject> generatedCubes = new List<GameObject>();
    [SerializeField]
    private MyGridSystem grid; //TODO : change to auto get Grid or Put grid to static

    [Button("Generate")]
    private void GenerateRandomCubes()
    {
        InitializeGrid();
        AddRoomsInScene();
        Clear();
        
        for (int i = 0; i < numberOfCubes; i++)
        {
            bool validPosition = false;
            Vector3Int newPosition = Vector3Int.zero;
            Vector3Int newScale = new Vector3Int(
                Random.Range(minSize.x, maxSize.x),
                Random.Range(minSize.y, maxSize.y),
                Random.Range(minSize.z, maxSize.z)
            );
            int index = 0;
            while (!validPosition && index < maxIteration)
            {
                if (type == GenerationType.ThreeDimension)
                {
                    newPosition = new Vector3Int(
                        Random.Range(-boundsRadius.x + newScale.x / 2, boundsRadius.x - newScale.x / 2),
                        Random.Range(-boundsRadius.y + newScale.y / 2, boundsRadius.y - newScale.y / 2),
                        Random.Range(-boundsRadius.z + newScale.z / 2, boundsRadius.z - newScale.z / 2)
                    );
                }
                else
                {
                    newPosition = new Vector3Int(
                        Random.Range(-boundsRadius.x + newScale.x / 2, boundsRadius.x - newScale.x / 2),
                        0,
                        Random.Range(-boundsRadius.z + newScale.z / 2, boundsRadius.z - newScale.z / 2)
                    );
                }

                validPosition = true;
                Bounds newBounds = new Bounds(newPosition, newScale);
                newBounds.Expand(minDistance);  // Expand the bounds of the new cube before checking intersection

                foreach (GameObject cube in generatedCubes)
                {
                    Bounds cubeBounds = new Bounds(cube.transform.position, cube.transform.localScale);
                    if (newBounds.Intersects(cubeBounds))
                    {
                        validPosition = false;
                        break;
                    }
                }
                index++;
            }

            // Only instantiate new room if a valid position is found
            if (validPosition)
            {
                GameObject room = grid.SetObjectAt(cubePrefab, newScale, newPosition);
                room.name = "Room" + generatedCubes.Count();
                generatedCubes.Add(room);
            }
        }

        grid.ExtendGrid();
    }



    private void Clear()
    {
        foreach (var room in generatedCubes)
        {
            DestroyImmediate(room);
        }
        
        generatedCubes.Clear();
    }

    private void AddRoomsInScene()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Room");

        foreach (GameObject obj in objects)
        {
            generatedCubes.Add(obj);
        }
    }

    private void InitializeGrid()
    {
        grid.Clear();
        grid.Initialize(boundsRadius);
    }
}

