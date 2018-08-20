using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Onos.Net.Utils.Misc.OnLab.Graph
{
    /// <summary>
    /// Implementation of a heap structure whose sense of order is imposed by the provided comparator.
    /// This class is not thread-safe!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// TODO: Make thread-safe.
    /// TODO: Add where T : IComparable{T}?
    public class Heap<T> : IEnumerable<T>
    {
        private readonly IList<T> data;
        private readonly IComparer<T> comparer;
        
        /// <summary>
        /// Returns the most extreme item in the heap or null if empty.
        /// </summary>
        /// <exception cref="InvalidOperationException">Heap is empty.</exception>
        public T Extreme => data.Count == 0 ? throw new InvalidOperationException("Heap is empty.") : data[0];

        /// <summary>
        /// Returns true if there are no items in the heap.
        /// </summary>
        public bool IsEmpty => data.Count == 0;

        /// <summary>
        /// Gets the count of items in the heap.
        /// </summary>
        public int Count => data.Count;

        /// <summary>
        /// Initializes a new <see cref="Heap{T}"/> object with the given data and comparer.
        /// </summary>
        /// <param name="data">The data for the heap.</param>
        /// <param name="comparer">The comparer for organizing heap.</param>
        public Heap(IEnumerable<T> data, IComparer<T> comparer)
        {
            this.data = data?.ToList() ?? throw new ArgumentNullException(nameof(data));
            this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            Heapify();
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => data.GetEnumerator();

        /// <summary>
        /// Removes and returns the most extremem item from the heap.
        /// </summary>
        /// <returns>The heap extreme or default if empty.</returns>
        /// <exception cref="InvalidOperationException">Heap is empty.</exception>
        public T ExtractExtreme()
        {
            if (!IsEmpty)
            {
                T extreme = Extreme;
                data[0] = data.Last();
                data.RemoveAt(data.Count - 1);
                Heapify();
                return extreme;
            }
            throw new InvalidOperationException("Heap is empty.");
        }

        /// <summary>
        /// Restores the heap property following any heap modifications 
        /// by rearranging the elements in the backing data as necessary.
        /// </summary>
        public void Heapify()
        {
            for (int i = data.Count / 2; i >= 0; --i)
            {
                Heapify(i);
            }
        }

        /// <summary>
        /// Inserts the given item into the heap and returns the modified heap.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        /// <returns>The heap itself.</returns>
        public Heap<T> Insert(T item)
        {
            data.Add(item);
            BubbleUp();
            return this;
        }

        /// <summary>
        /// Bubbles up the last item in the heap to its proper position in order to restore the heap property.
        /// </summary>
        private void BubbleUp()
        {
            int child = data.Count - 1;
            while (child > 0)
            {
                int parent = child / 2;
                if (comparer.Compare(data[child], data[parent]) < 0)
                {
                    break;
                }
                Swap(child, parent);
                child = parent;
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => data.GetEnumerator();

        /// <summary>
        /// Restores the heap property of the specified heap layer.
        /// </summary>
        /// <param name="i">The heap layer to heapify.</param>
        private void Heapify(int i)
        {
            int left = 2 * i + 1;
            int right = 2 * i;
            int extreme = i;

            if (left < data.Count && comparer.Compare(data[extreme], data[left]) < 0)
            {
                extreme = left;
            }

            if (right < data.Count && comparer.Compare(data[extreme], data[right]) < 0)
            {
                extreme = right;
            }

            if (extreme != i)
            {
                Swap(i, extreme);
                Heapify(extreme);
            }
        }

        /// <summary>
        /// Swaps two heap items at the specified indices.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="k"></param>
        private void Swap(int i, int k)
        {
            T temp = data[i];
            data[i] = data[k];
            data[k] = temp;
        }
    }
}
