using Shared.DataStructures.ConcatenatedList;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.DataStructures.Trie
{
	/// <summary>
	/// A thread-safe key-value data structure to store prefixes or suffixes associated data
	/// </summary>
	/// <typeparam name="IndexType"></typeparam>
	/// <typeparam name="DataType"></typeparam>
	public class Trie<IndexType, DataType>
	{
		public bool FromEnd { get; private set; } = false;
		private TrieNode<IndexType, DataType> _root = new(new List<IndexType>());
		public int LeafCount { get; private set; }
		public Trie(bool fromEnd)
		{
			FromEnd = fromEnd;
		}

		public void PreOrderDepthVisit(Action<TrieNode<IndexType, DataType>> callback)
		{
			RecPreOrderDepthVisit<int>(_root, (node, _) => { callback(node); return 0; }, 0);
		}
		public void PreOrderDepthVisit<T>(Func<TrieNode<IndexType, DataType>, T, T> callback, T seed) 
		{ 
			RecPreOrderDepthVisit(_root, callback, seed);
		}
		private void RecPreOrderDepthVisit<T>(TrieNode<IndexType,DataType> node, Func<TrieNode<IndexType, DataType>, T, T> callback, T v)
		{
			v = callback(node, v);
			foreach(TrieNode<IndexType, DataType> child in node.Childrens)
			{
				RecPreOrderDepthVisit(child, callback, v);
			}
		}

		public bool TryAdd(ITrieIndex<IndexType> index, DataType value)
		{
			var node = GetNodeOrCreateEmpty(index);
			if (node.GetData(out var _)) return false;
			lock (this)
			{
				LeafCount++;
				node.Data = value;
			}
			return true;
		}

		public TrieNode<IndexType, DataType> GetNode(IReadOnlyList<IndexType> index) 
		{ 
			if(index is null) throw new ArgumentNullException("index");
			ITrieIndex<IndexType> indexer;
			if (FromEnd) indexer = new ReverseTrieIndexer<IndexType>(index);
			else indexer = new TrieIndexer<IndexType>(index);
			var actualNode = _root;
			for(int i = 0; i < index.Count; i++)
			{
				var got = actualNode.TryGetChildred(indexer[i], out var children);
				if (got) actualNode = children;
				else return null;
			}
			return actualNode;
		}
		public TrieNode<IndexType, DataType> GetOrAdd(IReadOnlyList<IndexType> index, DataType addValue)
		{
			TrieNode<IndexType, DataType> node = GetNodeOrCreateEmpty(index);
			if(node.GetData(out var _)) return node;
			node.SetData(addValue);
			return node;
		}

		private TrieNode<IndexType, DataType> GetNodeOrCreateEmpty(IReadOnlyList<IndexType> index)
		{
			if (index is null) throw new ArgumentNullException("index");
			ITrieIndex<IndexType> indexer;
			if (FromEnd) indexer = new ReverseTrieIndexer<IndexType>(index);
			else indexer = new TrieIndexer<IndexType>(index);
			var actualNode = _root;
			bool added = false;
			for (int i = 0; i < index.Count; i++)
			{
				added = added || actualNode.GetOrAddChildren(indexer[i], new(index), out actualNode);
			}
			lock(this)
			{
				if(added) LeafCount++;
			}
			return actualNode;
		}

		public void CollectAllData(out ConcatenatedLinkedList<DataType> pippo)
		{
			pippo = new();
			_root.CollectAllData(ref pippo);
		}
		public void CollectAllLeafNodes(out ConcatenatedLinkedList<TrieNode<IndexType, DataType>> result)
		{
			result = new();
			_root.CollectAllLeafNode(ref result);
		}
	}
}
