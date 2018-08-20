using Onos.Net.Utils.Misc.OnLab.Graph;
using Onos.Net.Utils.Misc.OnLab.Test.Graph;
using System;
using System.Collections.Generic;
using System.Text;

namespace Onos.Net.Utils.Misc.OnLab.Test
{
    public class TestEdge : AbstractEdge<TestVertex>
    {
        private readonly IWeight weight;

        public TestEdge(TestVertex src, TestVertex dst, IWeight weight) : base(src, dst)
        {
            this.weight = weight;
        }

        public TestEdge(TestVertex src, TestVertex dst) : this(src, dst, )
    }
}
