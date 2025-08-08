using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataStructures.Trie
{
	public class TrieIndexer<T> : ITrieIndex<T>
	{
		public T this[int index] => _internal[index];

		public IReadOnlyList<T> IndexData => _internal;
		private IReadOnlyList<T> _internal;

		public int Count => _internal.Count;

		public TrieIndexer(IReadOnlyList<T> @internal)
		{
			_internal = @internal;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _internal.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
