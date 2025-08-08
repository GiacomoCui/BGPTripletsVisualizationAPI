using Shared.Model.BGP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.ASInternalGraph.GraphElements
{
	public class CPVertex : ASVertex, IEquatable<CPVertex>
	{
		public CollectorPeer Peer { get; set; }
		public CPVertex(CollectorPeer peer) 
			: base(peer.PeerAS)
		{
			Peer= peer;
		}

		public override string DetailsString()
		{
			return $"CP AS{Peer.PeerAS}\n{Peer.PeerIPAddress}";
		}
		public override string ToString()
		{
			return DetailsString();
		}

		public override bool Equals(object obj)
		{
			return obj is CPVertex && Equals((CPVertex)obj);
		}

		public override int GetHashCode()
		{
			return Peer.GetHashCode();
		}

		public bool Equals(CPVertex other)
		{
			return other is not null && Peer.Equals(other.Peer);
		}
		public static bool operator ==(CPVertex left, CPVertex right)
		{
			return EqualityComparer<CPVertex>.Default.Equals(left, right);
		}
		public static bool operator !=(CPVertex left, CPVertex right)
		{ return !(left == right); }
	}
}
