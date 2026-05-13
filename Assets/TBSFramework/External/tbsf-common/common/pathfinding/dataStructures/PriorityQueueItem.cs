namespace TurnBasedStrategyFramework.Common.Pathfinding.DataStructures
{
    /// <summary>
    /// Represents an item in the priority queue, consisting of the item itself and its associated priority value.
    /// </summary>
    public readonly struct PriorityQueueItem<T>
    {
        /// <summary>
        /// The item stored in the priority queue.
        /// </summary>
        public readonly T Item;

        /// <summary>
        /// The priority value associated with the item.
        /// </summary>
        public readonly float Priority;

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueueItem{T}"/> struct.
        /// </summary>
        /// <param name="item">The item to be stored in the priority queue.</param>
        /// <param name="priority">The priority value of the item.</param>
        public PriorityQueueItem(T item, float priority)
        {
            Item = item;
            Priority = priority;
        }
    }
}