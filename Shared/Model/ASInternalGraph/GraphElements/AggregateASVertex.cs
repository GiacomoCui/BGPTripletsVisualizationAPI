using Shared.Comparers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared.Model.ASInternalGraph.GraphElements
{
	public class AggregateASVertex : ASVertex, IEquatable<AggregateASVertex>
	{
		public int ASCount { get { return _aggregatedASes.Count; } }

		private HashSet<ASVertex> _aggregatedASes;

		private int _hash;

		public AggregateASVertex()
		{
			_aggregatedASes = new HashSet<ASVertex>();
		}
		public AggregateASVertex(IEnumerable<ASVertex> vertices) : this(vertices.ToHashSet())
		{

		}
		public AggregateASVertex(HashSet<ASVertex> aggregatedASes)
		{
			ASn = 0;
			_aggregatedASes = aggregatedASes;
			ComputeHash();
		}

		public void Add(ASVertex vertex)
		{
			if (_aggregatedASes.Add(vertex))
			{
				ComputeHash();
			}

		}
		public bool Remove(ASVertex vertex)
		{
			var r = _aggregatedASes.Remove(vertex);
			if (r) ComputeHash();
			return r;
		}
		private void ComputeHash()
		{
			_hash = new HashsetEqualityComparer<ASVertex>().GetHashCode(_aggregatedASes);
		}

		public bool Equals(AggregateASVertex other)
		{
			if (other is null) return false;
			return new HashsetEqualityComparer<ASVertex>().Equals(_aggregatedASes, other._aggregatedASes);
		}
		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			return Equals(obj as AggregateASVertex);
		}
		public override int GetHashCode()
		{
			return _hash;
		}
		public override string ToString()
		{
			return $"Aggr {ASCount}";
		}
		public override string DetailsString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Aggregated ASes:");
			foreach (ASVertex ASn in _aggregatedASes.OrderBy(v => v.ASn))
			{
				sb.AppendLine(ASn.ToString());
			}
			return sb.ToString();
		}

		public static bool operator ==(AggregateASVertex left, AggregateASVertex right)
		{
			return EqualityComparer<AggregateASVertex>.Default.Equals(left, right);
		}
		public static bool operator !=(AggregateASVertex left, AggregateASVertex right)
		{ return !(left == right); }
	}
}
