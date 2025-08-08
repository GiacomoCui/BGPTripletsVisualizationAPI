using QuikGraph;
using Shared.Model;
using Shared.Model.BGP;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using static Shared.Enums;

namespace Shared.Model.ASInternalGraph.GraphElements
{

	public class ASEdge : TaggedEdge<ASVertex, int>, IEquatable<ASEdge>, IEdge<ASVertex>
	{
		private ASVertex source;
		private ASVertex target;
		public TripletWithData Triplet { get; set; }
		private int paths;

		//public int Weight { get; set; }
		public ASRelationship PrecRelationship { get; set; } = ASRelationship.NONE;
		public ASRelationship SuccRelationship { get; set; } = ASRelationship.NONE;

		public ASEdge(ASVertex source, ASVertex target, TripletWithData edge, int paths)
			: base(source, target, paths)
		{
			Triplet = edge;
		}


		public virtual bool IsTraversedByPeer(CollectorPeer peer, AddressFamily family = AddressFamily.Unknown)
		{
			return Triplet.HasPeerSeen(peer, family);
		}

		public TripletWithData ToTriplet(uint curr)
		{
			return new TripletWithData(Source.ASn, curr, Target.ASn);
		}

		public override string ToString()
		{
			return $"{Tag:N0}";
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ASEdge);
		}
		public override int GetHashCode()
		{
			return HashCode.Combine(Source, Target, Tag);
		}
		public bool Equals(ASEdge other)
		{
			if (other == null) return false;
			return Source == other.Source && Target == other.Target && Tag == other.Tag;
		}

		public virtual string DetailString()
		{
			switch (PrecRelationship)
			{
				case ASRelationship.PROVIDER_CUSTOMER:
					return "<-----";
				case ASRelationship.CUSTOMER_PROVIDER:
					return "----->";
				case ASRelationship.PEER:
					return "------";
				default:
					return " ?? ";
			}
		}

		public virtual IEnumerable<CollectorPeer> GetSeenPeers(AddressFamily family)
		{
			return Triplet.GetSeenPeers(family);
		}
	}

}