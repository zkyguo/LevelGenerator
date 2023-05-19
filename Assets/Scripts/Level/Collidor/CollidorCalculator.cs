using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CollidorCalculator
{
    public static List<CollidorCell> FindPath(Room roomA, Room roomB, Dictionary<Vector3, Cell> gridCells)
    {
        int nbTry = 5;
        while (nbTry != 0)
        {
            var directionA = roomA.GetRandomBoundaryCell();
            var directionB = roomB.GetRandomBoundaryCell();

            Vector3 start = directionA.CollidorStart;
            Vector3 goal = directionB.CollidorStart;

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
                    roomA.UpdateDoorCells(directionA.CollidorStart - directionA.DoorDirection, directionA.DoorDirection);
                    roomB.UpdateDoorCells(directionB.CollidorStart - directionB.DoorDirection, directionB.DoorDirection);
                    // We found a path!
                    return ReconstructPath(cameFrom, start, goal, gridCells, directionA, directionB);
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

    private static List<CollidorCell> ReconstructPath(Dictionary<Vector3, Vector3> cameFrom, Vector3 start, Vector3 goal, Dictionary<Vector3, Cell> gridCells, CollidorDirection Startdirection, CollidorDirection GoalDirection)
    {
        HashSet<CollidorCell> path = new HashSet<CollidorCell>();
        Vector3 current = goal;
        Vector3 NextPosition = new Vector3();
        CollidorCell lastCell = new CollidorCell(current,CellType.Void);

        while (current != start)
        {
            NextPosition = cameFrom[current];
            if (current.y != NextPosition.y)
            {

                List<Vector3> stair = FindStair(NextPosition, current, gridCells);
                //Create collidor cell
                CollidorCell currentCell = new CollidorCell(current, CellType.Stair);   
                CollidorCell stair1 = new CollidorCell(stair[1], CellType.Stair);
                CollidorCell stair2 = new CollidorCell(stair[2], CellType.Stair);
                CollidorCell nextCell = new CollidorCell(NextPosition, CellType.Stair);
               

                //Update previous and next direction
                currentCell.previousDirection = lastCell.nextDirection; 
                if (current == goal)
                {
                    currentCell.previousDirection = GoalDirection.DoorDirection;
                }
                nextCell.nextDirection = cameFrom[NextPosition] - NextPosition;
                if(current == start)
                {
                    currentCell.nextDirection = -Startdirection.DoorDirection;
                }

                StairCell staircell = new StairCell((current + NextPosition) / 2, CellType.Stair, new List<CollidorCell> { currentCell, stair1, stair2, nextCell });
                //Add to path
                path.Add(staircell);
    
                //Update grid
                gridCells[current].CellType = CellType.Stair;
                gridCells[stair[1]].CellType = CellType.Stair;
                gridCells[stair[2]].CellType = CellType.Stair;          
                gridCells[NextPosition].CellType = CellType.Stair;

                current = cameFrom[NextPosition];
                lastCell = nextCell;
            }
            else
            {
                CollidorCell currentCell = new CollidorCell(current, CellType.Collidor);

                //Update previous and next direction
                currentCell.previousDirection = lastCell.nextDirection;
                if (current == goal)
                {
                    currentCell.previousDirection = GoalDirection.DoorDirection;
                }
                currentCell.nextDirection = NextPosition - current;
                if (current == start)
                {
                    currentCell.nextDirection = -Startdirection.DoorDirection;
                }

                path.Add(currentCell);
                gridCells[current].CellType = CellType.Collidor;
                current = cameFrom[current];
                lastCell = currentCell;
            }
            
            
        }
        if(path.FirstOrDefault(a => a.Position == start) == null)
        {
            CollidorCell currentCell = new CollidorCell(start, CellType.Collidor);
            currentCell.previousDirection = lastCell.nextDirection;
            currentCell.nextDirection = -Startdirection.DoorDirection;
            path.Add(currentCell); // optional
            gridCells[start].CellType = CellType.Collidor;
        }

        path.Reverse();
        return path.ToList();
    }

    private static float Heuristic(Vector3 a, Vector3 b)
    {
        // Use Manhattan distance for the heuristic
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }

    private static IEnumerable<Vector3> GetNeighbors(Vector3 current, Dictionary<Vector3, Cell> grid)
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
           
            if (grid.ContainsKey(next))
            {
                CellType type = grid[next].CellType;
                if (type == CellType.Void || type == CellType.Collidor) yield return next;
            }
        }
    }

    private static List<Vector3> FindStair(Vector3 current, Vector3 next, Dictionary<Vector3, Cell> gridCells)
    {
        HashSet<Vector3> stairCell = new HashSet<Vector3>();

        Vector3Int direction = Vector3Int.FloorToInt(next - current);

        if (gridCells[next].CellType != CellType.Void) return null;

        Vector3 directionX = new Vector3(current.x + direction.x, current.y, current.z);
        if (gridCells.ContainsKey(directionX) && gridCells[directionX].CellType != CellType.Void) return null;

        Vector3 directionY = new Vector3(current.x, current.y + direction.y, current.z);
        if (gridCells.ContainsKey(directionY) && gridCells[directionY].CellType != CellType.Void) return null;

        Vector3 directionZ = new Vector3(current.x, current.y, current.z + direction.z);
        if (gridCells.ContainsKey(directionZ) &&  gridCells[directionZ].CellType != CellType.Void) return null;

        stairCell.Add(current);
        stairCell.Add(directionX);
        stairCell.Add(directionY);
        stairCell.Add(directionZ);
        stairCell.Add(next);

        return stairCell.ToList();
    }

}
