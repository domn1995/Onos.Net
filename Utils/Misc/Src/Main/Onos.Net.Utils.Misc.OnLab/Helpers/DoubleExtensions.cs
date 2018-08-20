using System;

namespace Onos.Net.Utils.Misc.OnLab.Helpers
{
    public static class DoubleExtensions
    {
        public static bool FuzzyEquals(this double value, double other, double tolerance = 0) => Math.Abs(value - other) < tolerance;
    }
}