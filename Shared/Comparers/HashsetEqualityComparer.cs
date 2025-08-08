using System.Collections.Generic;

namespace Shared.Comparers
{
    public sealed class HashsetEqualityComparer<T> : IEqualityComparer<HashSet<T>>
    {
        // You could make this a per-instance field with a constructor parameter
        private static readonly EqualityComparer<T> elementComparer = EqualityComparer<T>.Default;

        public bool Equals(HashSet<T> first, HashSet<T> second)
        {
            if (first is null && second is null)
            {
                return true;
            }

            if (first == null || second == null)
            {
                return false;
            }

            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (first.GetType() != second.GetType())
            {
                return false;
            }

            if (first.Count != second.Count)
            {
                return false;
            }

            return first.SetEquals(second);
        }

        public int GetHashCode(HashSet<T> array)
        {
            unchecked
            {
                if (array == null)
                {
                    return 0;
                }
                int hash = 17;
                foreach (T element in array)
                {
                    hash = hash * 31 + elementComparer.GetHashCode(element);
                }
                return hash;
            }
        }
    }

}
