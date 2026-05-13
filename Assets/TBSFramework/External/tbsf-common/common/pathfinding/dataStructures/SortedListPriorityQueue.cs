using System.Collections.Generic;
using System.Linq;

namespace TurnBasedStrategyFramework.Common.Pathfinding.DataStructures
{
    /// <summary>
    /// Represents a sorted list-based priority queue.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the priority queue.</typeparam>
    public class SortedListPriorityQueue<T> : IPriorityQueue<T>
    {
        private readonly SortedList<float, Queue<T>> queue;

        public int Count => queue.Count;

        public SortedListPriorityQueue(int initialCapacity = 0)
        {
            queue = new SortedList<float, Queue<T>>(initialCapacity);
        }

        public void Enqueue(T item, float priority)
        {
            if (!queue.TryGetValue(priority, out Queue<T> items))
            {
                items = new Queue<T>();
                queue.Add(priority, items);
            }
            items.Enqueue(item);
        }

        public T Dequeue()
        {
            var pair = queue.First();
            var item = pair.Value.Dequeue();
            if (pair.Value.Count == 0)
            {
                queue.RemoveAt(0);
            }
            return item;
        }
    }
}
