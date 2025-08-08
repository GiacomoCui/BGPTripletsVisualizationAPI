using Shared.DataStructures.ConcatenatedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace Shared.DataStructures.Trie
{
	public class TrieNode<IndexType, DataType>
	{
		public IReadOnlyList<IndexType> Index { get; }

		public DataType Data { get;	set; }
		public bool ValidData {	get; private set; }

		private Dictionary<IndexType, TrieNode<IndexType, DataType>> _childrens;

		public TrieNode(IReadOnlyList<IndexType> index, DataType data) : this(index)
		{
			Data = data;
			ValidData = true;

		}
		public TrieNode(IReadOnlyList<IndexType> index)
		{
			_childrens = new();
			Index = index;
		}

		public IEnumerable<TrieNode<IndexType, DataType>> Childrens => _childrens.Values;

		private bool AddChildren(IndexType index, TrieNode<IndexType, DataType> node)
		{	
			var v = _childrens.ContainsKey(index); 
			if(v) return false;
			_childrens[index] = node;
			return true;
		}
		public bool TryGetChildred(IndexType index, out TrieNode<IndexType, DataType> value)
		{
			var v  = _childrens.TryGetValue(index, out value);
			return v;
		}
		/// <summary>
		/// </summary>
		/// <param name="index"></param>
		/// <param name="newNode"></param>
		/// <param name="result"></param>
		/// <returns>True if the newNode has been added, false if there was a node to return</returns>
		internal bool GetOrAddChildren(IndexType index, TrieNode<IndexType, DataType> newNode, out TrieNode<IndexType, DataType> result)
		{
			bool added = false;
			bool v = _childrens.TryGetValue(index, out var res);
			if (!v)
			{
				AddChildren(index, newNode);
				added = true;
				res = newNode;
			}
			result = res;
			return added;
		}

		public void SetData(DataType value) 
		{
			Data = value;
			ValidData = true;
		}
		public bool GetData(out DataType value)
		{
			value = Data;
			var v = ValidData;
			return v;
		}

		
		internal void CollectAllData(ref ConcatenatedLinkedList<DataType> pippo)
		{
			if(ValidData) pippo.AddLast(Data);
			foreach(var child in _childrens.Values)
			{
				child.CollectAllData(ref pippo);
			}
		}

		internal void CollectAllLeafNode(ref ConcatenatedLinkedList<TrieNode<IndexType, DataType>> result)
		{
			if (ValidData) result.AddLast(this);
			foreach(var child in _childrens.Values)
			{
				child.CollectAllLeafNode(ref result);
			}
		}

		public override string ToString()
		{
			return $"{Index} -> [{string.Join(", ", _childrens.Keys.ToArray())}]";
		}
	}
}
