using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : SerializedMonoBehaviour
{
    public GameObject cubePrefab;
    private List<GameObject> generatedCubes = new List<GameObject>();
    public int numberOfCubes = 10;
    [SerializeField]
    private Vector3 minSize = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField]
    private Vector3 maxSize = new Vector3(2.0f, 2.0f, 2.0f);
    public float minDistance = 1.0f;
    public float maxDistance = 5.0f;
    public int boundsRadius = 10;
    public int maxIteration = 10;
    
    [Button("Generate")]
    private void GenerateRandomCubes()
    {
        Clear();

        for(int i = 0; i < numberOfCubes; i++)
        {
            bool validPosition = false;
            Vector3 newPosition = Vector3.zero;
            Vector3 newScale = new Vector3(
                Random.Range(minSize.x, maxSize.x),
                Random.Range(minSize.y, maxSize.y),
                Random.Range(minSize.z, maxSize.z)
            );
            int index = 0;
            while (!validPosition && index != maxIteration)
            {
                newPosition = new Vector3(
                    Random.Range(-boundsRadius, boundsRadius),
                    Random.Range(-boundsRadius, boundsRadius),
                    Random.Range(-boundsRadius, boundsRadius)
                );

                validPosition = true;

                Bounds newBounds = new Bounds(newPosition, newScale);

                foreach (GameObject cube in generatedCubes)
                {
                    Bounds cubeBounds = new Bounds(cube.transform.position, cube.transform.localScale);
                    cubeBounds.Expand(minDistance);

                    if (newBounds.Intersects(cubeBounds))
                    {
                        validPosition = false;
                        break;
                    }
                }
                index++;
            }

            GameObject newCube = Instantiate(cubePrefab, newPosition, Quaternion.identity, this.gameObject.transform.GetChild(0));
            newCube.transform.localScale = newScale;

            generatedCubes.Add(newCube);
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
}
