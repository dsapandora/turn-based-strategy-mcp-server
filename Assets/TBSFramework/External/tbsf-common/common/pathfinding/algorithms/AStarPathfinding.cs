using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Pathfinding.DataStructures;

namespace TurnBasedStrategyFramework.Common.Pathfinding.Algorithms
{
    /// <summary>
    /// Implementation of A* pathfinding algorithm.
    /// </summary>
    public class AStarPathfinding : PathfindingAlgorithm
    {
        public override List<ICell> FindPath(Dictionary<ICell, Dictionary<ICell, float>> edges, ICell originNode, ICell destinationNode)
        {
            IPriorityQueue<ICell> frontier = new HeapPriorityQueue<ICell>();
            frontier.Enqueue(originNode, 0);

            Dictionary<ICell, ICell> cameFrom = new Dictionary<ICell, ICell>();
            cameFrom.Add(originNode, default);
            Dictionary<ICell, float> costSoFar = new Dictionary<ICell, float>();
            costSoFar.Add(originNode, 0);

            while (frontier.Count != 0)
            {
                var current = frontier.Dequeue();
                if (current.Equals(destinationNode)) break;

                var neighbours = GetNeighbours(edges, current);
                foreach (var neighbour in neighbours)
                {
                    var newCost = costSoFar[current] + edges[current][neighbour];
                    if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour])
                    {
                        costSoFar[neighbour] = newCost;
                        cameFrom[neighbour] = current;
                        var priority = newCost + Heuristic(destinationNode, neighbour);
                        frontier.Enqueue(neighbour, priority);
                    }
                }
            }

            return ReconstructPath(originNode, destinationNode, cameFrom, new List<ICell>());
        }
        public override (Dictionary<ICell, ICell> cameFrom, Dictionary<ICell, float> costSoFar) FindAllPaths(Dictionary<ICell, Dictionary<ICell, float>> edges, ICell originNode)
        {
            IPriorityQueue<ICell> frontier = new HeapPriorityQueue<ICell>(edges.Count);
            frontier.Enqueue(originNode, 0);

            Dictionary<ICell, ICell> cameFrom = new Dictionary<ICell, ICell>(edges.Count);
            cameFrom.Add(originNode, default);
            Dictionary<ICell, float> costSoFar = new Dictionary<ICell, float>(edges.Count);
            costSoFar.Add(originNode, 0);

            while (frontier.Count != 0)
            {
                var current = frontier.Dequeue();
                var neighbours = GetNeighbours(edges, current);
                var currentCost = costSoFar[current];
                var currentEdges = edges[current];

                foreach (var neighbour in neighbours)
                {
                    var newCost = currentCost + currentEdges[neighbour];
                    if (!costSoFar.TryGetValue(neighbour, out var neighbourCost) || newCost < neighbourCost)
                    {
                        costSoFar[neighbour] = newCost;
                        cameFrom[neighbour] = current;
                        var priority = newCost + Heuristic(originNode, neighbour);
                        frontier.Enqueue(neighbour, priority);
                    }
                }
            }

            return (cameFrom, costSoFar);
        }

        private int Heuristic(ICell a, ICell b)
        {
            return a.GetDistance(b);
        }
    }
}

