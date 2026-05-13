using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Cells;

namespace TurnBasedStrategyFramework.Common.Pathfinding.Algorithms
{
    /// <summary>
    /// Represents the base structure for pathfinding algorithms, defining core methods to compute paths in a graph.
    /// </summary>
    public abstract class PathfindingAlgorithm
    {
        private static readonly Dictionary<ICell, float>.KeyCollection EmptyNeighbours = new Dictionary<ICell, float>().Keys;

        /// <summary>
        /// Finds a path between the origin and destination nodes in the graph.
        /// </summary>
        /// <param name="edges">
        /// The graph representation, where each key is a node, and the value is a dictionary of neighboring nodes with their respective edge weights.
        /// </param>
        /// <param name="originNode">The starting node of the pathfinding process.</param>
        /// <param name="destinationNode">The target node that the pathfinding process aims to reach.</param>
        /// <returns>
        /// A list of cells representing the computed path from the origin to the destination. If no path exists, returns an empty list.
        /// </returns>
        public abstract List<ICell> FindPath(Dictionary<ICell, Dictionary<ICell, float>> edges, ICell originNode, ICell destinationNode);

        /// <summary>
        /// Finds all possible paths from the origin node to all reachable nodes in the graph.
        /// </summary>
        /// <param name="edges">
        /// The graph representation, where each key is a node, and the value is a dictionary of neighboring nodes with their respective edge weights.
        /// </param>
        /// <param name="originNode">The starting node for finding all possible paths.</param>
        /// <returns>
        /// A dictionary where each key is a reachable destination node and the value is the preceding node on the shortest path from the origin.
        /// </returns>
        public abstract (Dictionary<ICell, ICell> cameFrom, Dictionary<ICell, float> costSoFar) FindAllPaths(Dictionary<ICell, Dictionary<ICell, float>> edges, ICell originNode);

        /// <summary>
        /// Retrieves the neighboring nodes for the specified node from the graph's edge structure.
        /// </summary>
        /// <param name="edges">The graph representation, where each key is a node, and the value is a dictionary of neighboring nodes.</param>
        /// <param name="node">The node whose neighbors are to be retrieved.</param>
        /// <returns>
        /// An enumerable of neighboring cells. If the node has no neighbors, returns an empty enumerable.
        /// </returns>
        protected Dictionary<ICell, float>.KeyCollection GetNeighbours(Dictionary<ICell, Dictionary<ICell, float>> edges, ICell node)
        {
            if (edges.TryGetValue(node, out var neighbours))
            {
                return neighbours.Keys;
            }
            return EmptyNeighbours;
        }

        /// <summary>
        /// Reconstructs a path from the source node to the destination node using the mapping of nodes visited during pathfinding.
        /// </summary>
        /// <param name="source">The starting node of the path.</param>
        /// <param name="destination">The target node of the path.</param>
        /// <param name="cameFrom">A dictionary mapping each visited node to its predecessor on the path from the source.</param>
        /// <param name="buffer">A list used to store the reconstructed path. The list will be cleared at the start of the method.</param>
        /// <returns>
        /// The same buffer list containing the reconstructed path from the source to the destination. If no path exists, the list will be empty.
        /// </returns>
        public List<ICell> ReconstructPath(ICell source, ICell destination, Dictionary<ICell, ICell> cameFrom, List<ICell> buffer)
        {
            buffer.Clear();

            if(!cameFrom.ContainsKey(destination))
            {
                return buffer;
            }

            var current = destination;
            while(current != null && !current.Equals(source)) 
            { 
                buffer.Add(current);
                current = cameFrom[current];
            }

            buffer.Reverse();
            return buffer;
        }
    }
}
