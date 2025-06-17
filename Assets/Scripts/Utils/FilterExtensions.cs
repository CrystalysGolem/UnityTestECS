using Leopotam.EcsLite;

namespace Utils
{
    public static class FilterExtensions
    {
        public static int First(this EcsFilter filter)
        {
            foreach (var e in filter)
                return e;
            return -1;
        }
    }
}
