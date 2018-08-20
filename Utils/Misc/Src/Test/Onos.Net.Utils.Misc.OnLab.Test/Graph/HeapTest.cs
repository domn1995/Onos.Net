using Onos.Net.Utils.Misc.OnLab.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Onos.Net.Utils.Misc.OnLab.Test.Graph
{
    public class HeapTest
    {
        protected static IComparer<int> Min => Comparer<int>.Create((x, y) => y.CompareTo(x));
        protected static IComparer<int> Max => Comparer<int>.Default;

        protected List<int> Data => new List<int>() { 6, 4, 5, 9, 8, 3, 2, 1, 7, 0 };

        [Fact]
        public void Equality()
        {
            Assert.Equal(new Heap<int>(Data, Min), new Heap<int>(Data, Min));
            Assert.NotEqual(new Heap<int>(Data, Min), new Heap<int>(Data, Max));
        }

        [Fact]
        public void Empty()
        {
            Heap<int> h = new Heap<int>(new List<int>(), Min);
            Assert.True(h.IsEmpty, "Heap should be empty.");
            Assert.Equal(0, h.Count);
            Assert.Throws<InvalidOperationException>(() => h.Extreme);
            Assert.Throws<InvalidOperationException>(() => h.ExtractExtreme());
        }

        [Fact]
        public void Insert()
        {
            Heap<int> h = new Heap<int>(Data, Min);
            Assert.Equal(10, h.Count);
            h.Insert(3);
            Assert.Equal(11, h.Count);
        }

        [Fact]
        public void MinQueue()
        {
            Heap<int> h = new Heap<int>(Data, Min);
            Assert.False(h.IsEmpty, "Heap should not be empty.");
            Assert.Equal(10, h.Count);
            Assert.Equal(0, h.Extreme);
            for (int i = 0, n = h.Count; i < n; ++i)
            {
                Assert.Equal(i, h.ExtractExtreme());
            }
            Assert.True(h.IsEmpty, "Heap should be empty.");
        }

        [Fact]
        public void MaxQueue()
        {
            Heap<int> h = new Heap<int>(Data, Max);
            Assert.False(h.IsEmpty, "Heap should not be empty.");
            Assert.Equal(10, h.Count);
            Assert.Equal(9, h.Extreme);
            for (int i = h.Count; i > 0; --i)
            {
                Assert.Equal(i - 1, h.ExtractExtreme());
            }
            Assert.True(h.IsEmpty, "Heap should be empty.");
        }

        [Fact]
        public void Enumerator()
        {
            Heap<int> h = new Heap<int>(Data, Min);
            Assert.True(h.GetEnumerator().MoveNext(), "Heap enumerator should have next element.");
        }
    }
}