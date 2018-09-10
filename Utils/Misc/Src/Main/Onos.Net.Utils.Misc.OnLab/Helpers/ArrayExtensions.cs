namespace Onos.Net.Utils.Misc.OnLab.Helpers
{
    public static class ArrayExtensions
    {
        public static int GetArrayHashCode<T>(this T[] array)
        {
            if (array is null)
            {
                return 0;
            }

            unchecked
            {
                int hash = 17;
                foreach (T item in array)
                {
                    hash = hash * 23 + (item?.GetHashCode() ?? 0);
                }
                return hash;
            }
        }
    }
}