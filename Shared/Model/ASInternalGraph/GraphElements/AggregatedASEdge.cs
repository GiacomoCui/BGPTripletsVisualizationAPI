using Shared.Model.BGP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.ASInternalGraph.GraphElements
{
	public class AggregatedASEdge : ASEdge
	{
		public struct AggregatePath
		{
			public uint AggASN { get; set; }
			public int Count { get; set; }
			public bool IsCP { get; set; }

			public string String => $"{AggASN} | {Count}{(IsCP ? " | IsCP" : "")}";
		}

		private string _details;
		public IEnumerable<AggregatePath> AggregatedEdges { get; set; }
		private IEnumerable<ASEdge> _originalEdges;

		public AggregatedASEdge(ASVertex source, ASVertex target, IEnumerable<ASEdge> aggregatedEdges, bool isSourceAggregated) 
			:base(source, target, null, 0)
		{
			if(!aggregatedEdges.Any())
			{
				throw new ArgumentException("Aggregated Edges is Empty");
			}
			Func<ASEdge, ASVertex> selctor = isSourceAggregated ? (e => e.Source) : e => e.Target;
			AggregatedEdges = aggregatedEdges.Select(e => {
				ASVertex n = selctor(e);
				return new AggregatePath() { AggASN = n.ASn, Count = e.Tag, IsCP = n.IsCP };
			}).ToList();

			PrecRelationship = aggregatedEdges.First().PrecRelationship;
			SuccRelationship = aggregatedEdges.First().SuccRelationship;

			StringBuilder b = new();
			b.AppendLine("ASn -- #paths");
			foreach(var aggEdge in AggregatedEdges.OrderBy(e => e.AggASN))
			{
				Tag += aggEdge.Count;
				b.AppendLine($"{aggEdge.AggASN}: #{aggEdge.Count}");
			}
			_details = b.ToString();
			_originalEdges = aggregatedEdges;
		}

		public override bool IsTraversedByPeer(CollectorPeer peer, AddressFamily family = AddressFamily.Unknown)
		{
			return _originalEdges.Any(e => e.IsTraversedByPeer(peer, family));
		}
		public override IEnumerable<CollectorPeer> GetSeenPeers(AddressFamily family)
		{
			return _originalEdges.SelectMany(e => e.GetSeenPeers(family));
		}

		public override string DetailString()
		{
			return _details;
		}
	}
}
