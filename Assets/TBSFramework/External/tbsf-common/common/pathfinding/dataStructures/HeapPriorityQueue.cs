using System.Collections.Generic;

namespace TurnBasedStrategyFramework.Common.Pathfinding.DataStructures
{
    class HeapPriorityQueue<T> : IPriorityQueue<T>
    {
        private List<PriorityQueueItem<T>> _queue;

        public HeapPriorityQueue(int initialCapacity = 0)
        {
            _queue = new List<PriorityQueueItem<T>>(initialCapacity);
        }

        public int Count
        {
            get { return _queue.Count; }
        }

        public void Enqueue(T item, float priority)
        {
            _queue.Add(new PriorityQueueItem<T>(item, priority));
            int ci = _queue.Count - 1;

            while (ci > 0)
            {
                int pi = (ci - 1) / 2;
                if (_queue[ci].Priority >= _queue[pi].Priority)
                    break;

                var tmp = _queue[pi];
                _queue[pi] = _queue[ci];
                _queue[ci] = tmp;

                ci = pi;
            }
        }

        public T Dequeue()
        {
            int li = _queue.Count - 1;
            var frontItem = _queue[0];
            _queue[0] = _queue[li];
            _queue.RemoveAt(li);

            --li;
            int pi = 0;

            while (true)
            {
                int ci = pi * 2 + 1;
                if (ci > li) break;

                int rc = ci + 1;
                if (rc <= li && _queue[rc].Priority < _queue[ci].Priority)
                    ci = rc;

                if (_queue[pi].Priority <= _queue[ci].Priority)
                    break;

                var tmp = _queue[pi];
                _queue[pi] = _queue[ci];
                _queue[ci] = tmp;

                pi = ci;
            }

            return frontItem.Item;
        }
    }
}
