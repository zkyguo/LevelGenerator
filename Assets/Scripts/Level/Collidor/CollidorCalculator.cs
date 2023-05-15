using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CollidorCalculator
{
    public static List<Vector3> FindPath(Room roomA, Room roomB, Dictionary<Vector3, CellType> gridCells)
    {
        int nbTry = 3;
        while (nbTry != 0)
        {
            Vector3 start = roomA.GetRandomBoundaryCell();
            Vector3 goal = roomB.GetRandomBoundaryCell();

            Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
            Dictionary<Vector3, float> costSoFar = new Dictionary<Vector3, float>();
            Dictionary<Vector3, HashSet<Vector3>> pathSoFar = new Dictionary<Vector3, HashSet<Vector3>>();

            PriorityQueue<Vector3> frontier = new PriorityQueue<Vector3>();
            frontier.Enqueue(start, 0);

            cameFrom[start] = start;
            costSoFar[start] = 0;
            pathSoFar[start] = new HashSet<Vector3> { start };

            while (!frontier.IsEmpty())
            {
                Vector3 current = frontier.Dequeue();

                if (current == goal)
                {
                    // We found a path!
                    return ReconstructPath(cameFrom, start, goal);
                }

                // Check all neighbors
                foreach (Vector3 next in GetNeighbors(current, gridCells))
                {
                    bool isStair = false;
                    List<Vector3> stairNodes = null;
                    if (next.y != current.y)
                    {
                        stairNodes = FindStair(current, next, gridCells);
                        if (stairNodes == null) continue;
                        isStair = true;
                    }

                    float newCost = 0;
                    if (stairNodes != null)
                    {
                        newCost = costSoFar[current] + 2;
                        foreach (var stairNode in stairNodes)
                        {
                            if (pathSoFar[current].Contains(stairNode))
                            {
                                isStair = false;
                                break;
                            }
                        }
                        if (isStair == false)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        newCost = costSoFar[current] + 1;
                    }

                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        if (!isStair && pathSoFar[current].Contains(next))
                        {
                            continue;
                        }

                        costSoFar[next] = newCost;
                        float priority = newCost + Heuristic(next, goal);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;
                        pathSoFar[next] = new HashSet<Vector3>(pathSoFar[current]);
                        pathSoFar[next].Add(current);
                        if (isStair)
                        {
                            foreach (var stairNode in stairNodes)
                            {
                                pathSoFar[next].Add(stairNode);
                            }
                        }
                    }
                }
            }
            nbTry--;
        }
        return null;
    }

    private static List<Vector3> ReconstructPath(Dictionary<Vector3, Vector3> cameFrom, Vector3 start, Vector3 goal)
    {
        HashSet<Vector3> path = new HashSet<Vector3>();
        Vector3 current = goal;
        
        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Add(start); // optional
        path.Reverse();
        return path.ToList();
    }

    private static float Heuristic(Vector3 a, Vector3 b)
    {
        // Use Manhattan distance for the heuristic
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }

    private static IEnumerable<Vector3> GetNeighbors(Vector3 current, Dictionary<Vector3, CellType> grid)
    {
        // Return the six neighboring cells
        Vector3[] neighbors = new Vector3[]
        {
        //Collidor
        new Vector3(current.x + 1, current.y, current.z),
        new Vector3(current.x - 1, current.y, current.z),
        new Vector3(current.x, current.y, current.z + 1),
        new Vector3(current.x, current.y, current.z - 1),
        //Stair
        new Vector3(current.x, current.y + 1, current.z+1),
        new Vector3(current.x, current.y - 1, current.z+1),
        new Vector3(current.x, current.y + 1, current.z-1),
        new Vector3(current.x, current.y - 1, current.z-1),
        new Vector3(current.x+1, current.y + 1, current.z),
        new Vector3(current.x+1, current.y - 1, current.z),
        new Vector3(current.x-1, current.y + 1, current.z),
        new Vector3(current.x-1, current.y - 1, current.z),
        };
        foreach (var next in neighbors)
        {
            if (grid.ContainsKey(next) && (grid[next] == CellType.Void))
            {
                yield return next;
            }
        }
    }

    private static List<Vector3> FindStair(Vector3 current, Vector3 next, Dictionary<Vector3, CellType> gridCells)
    {
        HashSet<Vector3> stairCell = new HashSet<Vector3>();

        Vector3Int direction = Vector3Int.FloorToInt(next - current);

        if (gridCells[next] != CellType.Void) return null;

        Vector3 directionX = new Vector3(current.x + direction.x, current.y, current.z);
        if (gridCells.ContainsKey(directionX) && gridCells[directionX] != CellType.Void) return null;

        Vector3 directionY = new Vector3(current.x, current.y + direction.y, current.z);
        if (gridCells.ContainsKey(directionY) && gridCells[directionY] != CellType.Void) return null;

        Vector3 directionZ = new Vector3(current.x, current.y, current.z + direction.z);
        if (gridCells[directionZ] != CellType.Void) return null;

        stairCell.Add(current);
        stairCell.Add(directionX);
        stairCell.Add(directionY);
        stairCell.Add(directionZ);
        stairCell.Add(next);

        return stairCell.ToList();
    }

}
