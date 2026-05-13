namespace TurnBasedStrategyFramework.Common.Pathfinding.DataStructures
{
    /// <summary>
    /// Represents a prioritized queue.
    /// </summary>
    public interface IPriorityQueue<T>
    {
        /// <summary>
        /// Gets the number of items currently in the queue.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Adds an item to the priority queue with the specified priority.
        /// </summary>
        /// <param name="item">The item to be added to the queue.</param>
        /// <param name="priority">The priority of the item.</param>
        void Enqueue(T item, float priority);

        /// <summary>
        /// Removes and returns the item with the lowest priority value from the queue.
        /// </summary>
        /// <returns>The item with the lowest priority value.</returns>
        T Dequeue();
    }
}