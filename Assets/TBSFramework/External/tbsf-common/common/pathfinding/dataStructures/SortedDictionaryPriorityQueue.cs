using System.Collections.Generic;
using System.Linq;

namespace TurnBasedStrategyFramework.Common.Pathfinding.DataStructures
{
    /// <summary>
    /// Represents a soreted dictionary-based priority queue.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the priority queue.</typeparam>
    public class SortedDictionaryPriorityQueue<T> : IPriorityQueue<T>
    {
        private readonly SortedDictionary<float, Queue<T>> queue;
        public int Count => queue.Count;

        public SortedDictionaryPriorityQueue(int initialCapacity = 0)
        {
            queue = new SortedDictionary<float, Queue<T>>(new Dictionary<float, Queue<T>>(initialCapacity));
        }

        public void Enqueue(T item, float priority)
        {
            if (!queue.TryGetValue(priority, out Queue<T> items))
            {
                items = new Queue<T>();
                queue[priority] = items;
            }
            items.Enqueue(item);
        }

        public T Dequeue()
        {
            var pair = queue.First();
            var item = pair.Value.Dequeue();
            if (pair.Value.Count == 0)
            {
                queue.Remove(pair.Key);
            }
            return item;
        }
    }
}
