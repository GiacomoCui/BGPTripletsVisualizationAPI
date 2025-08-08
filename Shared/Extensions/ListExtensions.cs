using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Extensions
{
    public static class ListExtensions
    {
        public static List<List<T>> Split<T>(this List<T> collection, int size)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            var chunks = new List<List<T>>();
            var chunkCount = collection.Count / size;

            if (collection.Count % size > 0)
                chunkCount++;

            for (var i = 0; i < size; i++)
                chunks.Add(collection.Skip(i * chunkCount).Take(chunkCount).ToList());

            return chunks;
        }

        public static List<uint> Clone(this List<uint> listToClone)
        {
            return new List<uint>(listToClone);
        }

        public static List<List<uint>> Clone(this List<List<uint>> listToClone)
        {
            return listToClone.Select(item => item.Clone()).ToList();
        }
    }
}
