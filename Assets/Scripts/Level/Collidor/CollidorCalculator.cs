using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollidorCalculator
{

    public static List<Vector3> FindPath(Room roomA, Room roomB, MyGridSystem grid)
    {
        Vector3 start = roomA.GetRandomBoundaryCell();
        Vector3 goal = roomB.GetRandomBoundaryCell();

        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
        Dictionary<Vector3, float> costSoFar = new Dictionary<Vector3, float>();

        PriorityQueue<Vector3> frontier = new PriorityQueue<Vector3>();
        frontier.Enqueue(start, 0);

        cameFrom[start] = start;
        costSoFar[start] = 0;

        while (!frontier.IsEmpty())
        {
            Vector3 current = frontier.Dequeue();

            if (current == goal)
            {
                // We found a path!
                if(current !=  goal)
                {
                    return ReconstructPath(cameFrom, start, current);
                }
                return ReconstructPath(cameFrom, start, goal);

            }

            bool canAdd = true;
            // Check all neighbors
            foreach (Vector3 next in GetNeighbors(current, grid.GetGridCells(), goal))
            {
                List<Vector3> stair = new List<Vector3>();
                if(next.y != current.y)
                {
                    stair = grid.IsStairClear(current, next);
                    canAdd = (stair != null);
                }
                if(canAdd)
                {
                    float newCost = costSoFar[current] + 1; // Assuming all moves have a cost of 1
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        float priority = newCost + Heuristic(next, goal);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;

                    }
                }
                canAdd = true;
            }
        }

        // No path was found
        return null;
    }

    private static List<Vector3> ReconstructPath(Dictionary<Vector3, Vector3> cameFrom, Vector3 start, Vector3 goal)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 current = goal;
        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Add(start); // optional
        path.Reverse();
        return path;
    }

    private static float Heuristic(Vector3 a, Vector3 b)
    {
        // Use Manhattan distance for the heuristic
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }

    private static IEnumerable<Vector3> GetNeighbors(Vector3 current, Dictionary<Vector3, CellType> grid, Vector3 goal)
    {
        // Return the six neighboring cells
        Vector3[] neighbors = new Vector3[]
        {
        new Vector3(current.x + 1, current.y, current.z),
        new Vector3(current.x - 1, current.y, current.z),

        new Vector3(current.x, current.y + 1, current.z+1),
        new Vector3(current.x, current.y - 1, current.z+1),
        new Vector3(current.x, current.y + 1, current.z-1),
        new Vector3(current.x, current.y - 1, current.z-1),

        new Vector3(current.x+1, current.y + 1, current.z),
        new Vector3(current.x+1, current.y - 1, current.z),
        new Vector3(current.x-1, current.y + 1, current.z),
        new Vector3(current.x-1, current.y - 1, current.z),

        new Vector3(current.x, current.y, current.z + 1),
        new Vector3(current.x, current.y, current.z - 1),
        };
        foreach (var next in neighbors)
        {
            if ( (grid.ContainsKey(next) && grid[next] == CellType.Void))
            {
                yield return next;
            }
        }
    }

}
