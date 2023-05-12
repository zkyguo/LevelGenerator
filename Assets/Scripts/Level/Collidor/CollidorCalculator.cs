using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathCalculator
{
    public static List<Vector3Int> FindPath(Room roomA, Room roomB)
    {
        Vector3 start = roomA.GetRandomBoundaryCell();
        Vector3 goal = roomB.GetRandomBoundaryCell();

        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, float> costSoFar = new Dictionary<Vector3Int, float>();

        PriorityQueue<Vector3Int> frontier = new PriorityQueue<Vector3Int>();
        frontier.Enqueue(start, 0);

        cameFrom[start] = start;
        costSoFar[start] = 0;

        while (!frontier.IsEmpty)
        {
            Vector3Int current = frontier.Dequeue();

            if (current == goal)
            {
                // We found a path!
                return ReconstructPath(cameFrom, start, goal);
            }

            // Check all neighbors
            foreach (Vector3Int next in GetNeighbors(current))
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
        }

        // No path was found
        return null;
    }

    private static List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int start, Vector3Int goal)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int current = goal;
        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Add(start); // optional
        path.Reverse();
        return path;
    }

    private static float Heuristic(Vector3Int a, Vector3Int b)
    {
        // Use Manhattan distance for the heuristic
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }

    private static IEnumerable<Vector3Int> GetNeighbors(Vector3Int cell)
    {
        // Return the six neighboring cells
        Vector3Int[] neighbors = new Vector3Int[]
        {
        new Vector3Int(cell.x + 1, cell.y, cell.z),
        new Vector3Int(cell.x - 1, cell.y, cell.z),
        new Vector3Int(cell.x, cell.y + 1, cell.z),
        new Vector3Int(cell.x, cell.y - 1, cell.z),
        new Vector3Int(cell.x, cell.y, cell.z + 1),
        new Vector3Int(cell.x, cell.y, cell.z - 1),
        };
        foreach (var neighbor in neighbors)
        {
            if (grid.ContainsKey(neighbor) && !grid[neighbor])
            {
                yield return neighbor;
            }
        }
    }
}
