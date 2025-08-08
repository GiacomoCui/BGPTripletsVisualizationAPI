using System;
using System.Collections;
using System.Collections.Generic;

namespace Shared.DataStructures.ConcatenatedList
{
	public class ConcatenatedLinkedList<T> : IEnumerable<T>
	{
		private ConcatenatedLinkedListNode<T> _head;
		private int _size;
		public int Count => _size;

		/// <summary>
		/// Initialize an empty List
		/// </summary>
		public ConcatenatedLinkedList()
		{
			_size = 0;
		}
		/// <summary>
		/// Initialize a list with all the element in the given collection, respecting the order
		/// </summary>
		/// <param name="collection"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public ConcatenatedLinkedList(IEnumerable<T> collection)
			: base()
		{
			if (collection == null)
				throw new ArgumentNullException(nameof(collection));

			foreach (T elem in collection)
			{
				AddLast(elem);
			}
		}

		/// <summary>
		/// Initialize a list with a single element
		/// </summary>
		/// <param name="x"></param>
		public ConcatenatedLinkedList(T x)
			: base()
		{
			AddToEmpty(x);
		}

		public void AddFirst(T item)
		{
			if (_head == null)
			{
				AddToEmpty(item);
				return;
			}
			ConcatenatedLinkedListNode<T> node = new(item);
			AddFirst(node);
		}
		private void AddFirst(ConcatenatedLinkedListNode<T> node)
		{
			_size++;
			if (_head == null)
			{
				_head = node;
				return;
			}
			node.Prev = _head.Prev;
			_head.Prev.Next = node;
			node.Next = _head;
			_head.Prev = node;
			_head = node;
		}

		public void AddLast(T item)
		{
			if (_head == null)
			{
				AddToEmpty(item);
				return;
			}
			ConcatenatedLinkedListNode<T> node = new(item);
			AddLast(node);
		}
		private void AddLast(ConcatenatedLinkedListNode<T> node)
		{
			_size++;
			if (_head == null)
			{
				_head = node;
				return;
			}
			node.Prev = _head.Prev;
			_head.Prev.Next = node;
			node.Next = _head;
			_head.Prev = node;
		}

		public T RemoveLast()
		{
			if (_head == null)
			{
				return default;
			}
			ConcatenatedLinkedListNode<T> last = _head.Prev;
			last.Prev.Next = _head;
			_head.Prev = last.Prev;
			if (_size == 1)
			{
				_head = null;
			}
			_size--;
			return last.Value;
		}
		public T RemoveFirst()
		{
			if (_head == null)
			{
				throw new ListEmptyException();
			}
			ConcatenatedLinkedListNode<T> first = _head;
			first.Prev.Next = first.Next;
			first.Next.Prev = first.Prev;
			if (_size == 1)
			{
				_head = null;
			}
			else
			{
				_head = first.Next;
			}
			_size--;
			return first.Value;
		}

		private void AddToEmpty(T item)
		{
			ConcatenatedLinkedListNode<T> node = new(item);
			node.Prev = node;
			node.Next = node;
			_head = node;
			_size = 1;
		}
		public void Clear()
		{
			_size = 0;
			_head = null;
		}

		public void Concat(ConcatenatedLinkedList<T> other)
		{
			if (other == null)
			{
				return;
			}
			if (_size == 0)
			{
				_head = other._head;
				_size = other._size;

			}
			else if (_size == 1)
			{
				other.AddFirst(_head);
				_head = other._head;
				_size = other._size;
			}
			else if (other._size == 0)
			{
				return;
			}
			else if (other._size == 1)
			{
				AddLast(other._head);
			}
			else
			{
				ConcatenatedLinkedListNode<T> thisLast = _head.Prev;
				ConcatenatedLinkedListNode<T> otherFirst = other._head;

				ConcatenatedLinkedListNode<T> otherLast = otherFirst.Prev;

				thisLast.Next = otherFirst;
				otherFirst.Prev = thisLast;
				otherLast.Next = _head;
				_head.Prev = otherLast;
				_size += other._size;
			}
			other.Clear();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}


		public T GetLast()
		{
			if (_size == 0)
			{
				throw new ListEmptyException();
			}
			return _head.Prev.Value;
		}
		public T GetFirst()
		{
			if (_size == 0)
			{
				throw new ListEmptyException();
			}
			return _head.Value;
		}

		public class Enumerator : IEnumerator<T>, IEnumerator
		{
			public T Current => _current.Value;

			object IEnumerator.Current => Current;

			private ConcatenatedLinkedList<T> _list;
			private ConcatenatedLinkedListNode<T> _current;

			public Enumerator(ConcatenatedLinkedList<T> list)
			{
				_list = list;
				Reset();
			}

			public void Dispose()
			{

			}

			public bool MoveNext()
			{
				if (_current == null)
				{
					_current = _list._head;
					return _current is not null; // _current is not null;
				}
				if (_current != _list._head.Prev)
				{
					_current = _current.Next;
					return true;
				}
				return false;
			}

			public void Reset()
			{
				_current = null;
			}
		}
	}
}