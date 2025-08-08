using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataStructures.Trie
{
	internal class ReverseTrieIndexer<T> : ITrieIndex<T>
	{
		public T this[int index]
		{
			get { return _internal[^(index+1)]; }
		}

		public int Count => _internal.Count;

		public IReadOnlyList<T> IndexData => _internal;

		private IReadOnlyList<T> _internal;

		public ReverseTrieIndexer(IReadOnlyList<T> @internal)
		{
			_internal = @internal;
		}

		private class ReverseEnumerator : IEnumerator<T>
		{
			private ReverseTrieIndexer<T> _array;
			private int _index = 0;
			
			public T Current => _array[_index];

			object IEnumerator.Current => Current;

			public ReverseEnumerator(ReverseTrieIndexer<T> array)
			{
				_array = array;
			}

			public void Dispose()
			{
				throw new NotImplementedException();
			}

			public bool MoveNext()
			{
				_index++;
				if(_index >= _array.Count)
				{
					return false;
				}
				return true;
			}

			public void Reset()
			{
				_index = 0;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new ReverseEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
