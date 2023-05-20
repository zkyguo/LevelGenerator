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
    private int numberOfCubes;
    public Vector3Int minSize = new Vector3Int(2, 1, 2);
    public Vector3Int maxSize = new Vector3Int(5, 2, 5);
    public float minDistance = 1.0f;
    public float maxDistance = 5.0f;
    [SerializeField]
    public Vector3Int boundsRadius;
    public int maxIteration = 10;

    private List<GameObject> generatedRooms = new List<GameObject>();
    [SerializeField]
    private MyGridSystem grid; //TODO : change to auto get Grid or Put grid to static
    [SerializeField]
    private PathGenerator path; //TODO : change to auto get Grid or Put grid to static
    [SerializeField]
    private CollidorGenerator Collidor; //TODO : change to auto get Grid or Put grid to static

    

    public void Generate()
    {
        if(numberOfCubes != 0)
        {
            PlaceRandomRoom();
            if (path == null)
            {
                path = SingletonManager.Instance.GetSingleton<PathGenerator>();

            }
            path?.GeneratePath();
            if (Collidor == null)
            {
                Collidor = SingletonManager.Instance.GetSingleton<CollidorGenerator>();

            }
            Collidor?.GenerateCollidors();
            //ApplyRulesRoom();
        }

    }

    private void PlaceRandomRoom()
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

                foreach (GameObject cube in generatedRooms)
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
                room.name = "Room" + generatedRooms.Count();
                generatedRooms.Add(room);
            }
        }
    }



    private void Clear()
    {
        foreach (var room in generatedRooms)
        {
            DestroyImmediate(room);
        }

        generatedRooms.Clear();
    }

    private void AddRoomsInScene()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Room");

        foreach (GameObject obj in objects)
        {
            generatedRooms.Add(obj);
        }
    }

    private void InitializeGrid()
    {
        if(grid == null)
        {
            grid = SingletonManager.Instance.GetSingleton<MyGridSystem>();
        }
        grid.Clear();
        grid.Initialize(boundsRadius);
    }

    private void ApplyRulesRoom()
    {
        foreach (var room in generatedRooms)
        {
            room.GetComponent<Room>().ApplyRules();
        }
    }

    #region Getter Setter
    public void SetRoomNumber(int value)
    {
        numberOfCubes = value;
    }

    public void setMaxSize(Vector3Int value)
    {
        maxSize = value;
    }
    #endregion
}

