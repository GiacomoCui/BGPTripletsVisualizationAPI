using ScottPlot.Palettes;
using ScottPlot.TickGenerators.TimeUnits;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Shared.Comparers
{
    public sealed class IndexableEqualityComparer<T> : IEqualityComparer<T[]>, IEqualityComparer<List<T>>
    {
        // You could make this a per-instance field with a constructor parameter
        private static readonly EqualityComparer<T> elementComparer = EqualityComparer<T>.Default;

        public bool Equals(T[] first, T[] second)
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

            if (first.Length != second.Length)
            {
                return false;
            }

            for (int i = 0; i < first.Length; i++)
            {
                if (!elementComparer.Equals(first[i], second[i]))
                {
                    return false;
                }
            }

            return true;
        }
        public bool Equals(List<T> first, List<T> second)
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

			for (int i = 0; i < first.Count; i++)
			{
				if (!elementComparer.Equals(first[i], second[i]))
				{
					return false;
				}
			}

			return true;
		}

		public int GetHashCode(T[] array)
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

		public int GetHashCode([DisallowNull] List<T> list)
		{
			if (list == null)
			{
				return 0;
			}
			int hash = 17;
			foreach (T element in list)
			{
				hash = hash * 31 + elementComparer.GetHashCode(element);
			}
			return hash;
		}
	}

}
