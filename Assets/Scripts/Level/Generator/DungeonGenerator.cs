using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;


public enum GenerationType
{
    TwoDimension, ThreeDimension
}

public class DungeonGenerator : SerializedMonoBehaviour
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
    public int boundsRadius = 10;
    public int maxIteration = 10;

    private List<GameObject> generatedCubes = new List<GameObject>();
    [SerializeField]
    private MyGridSystem grid; //TODO : change to auto get Grid or Put grid to static

    [Button("Generate")]
    private void GenerateRandomCubes()
    {
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
                        Random.Range(-boundsRadius + newScale.x / 2, boundsRadius - newScale.x / 2),
                        Random.Range(-boundsRadius + newScale.y / 2, boundsRadius - newScale.y / 2),
                        Random.Range(-boundsRadius + newScale.z / 2, boundsRadius - newScale.z / 2)
                    );
                }
                else
                {
                    newPosition = new Vector3Int(
                        Random.Range(-boundsRadius + newScale.x / 2, boundsRadius - newScale.x / 2),
                        0,
                        Random.Range(-boundsRadius + newScale.z / 2, boundsRadius - newScale.z / 2)
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

            // Only instantiate new cube if a valid position is found
            if (validPosition)
            {             
                generatedCubes.Add(grid.SetObjectAt(cubePrefab, newScale, newPosition));
            }
        }
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

}

