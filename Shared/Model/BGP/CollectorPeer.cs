using System;
using System.Net;

namespace Shared.Model.BGP
{
	public class CollectorPeer : IEquatable<CollectorPeer>
	{
		public IPAddress PeerIPAddress { get; set; } = new IPAddress(new byte[4] { 0, 0, 0, 0 });
		public uint PeerAS { get; set; }

		public CollectorPeer(uint peerAS)
		{
			PeerAS = peerAS;
		}
		public CollectorPeer(IPAddress peerIPAddress, uint peerAS)
		{
			PeerIPAddress = peerIPAddress;
			PeerAS = peerAS;
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (!(obj is CollectorPeer)) return false;
			return Equals(obj as CollectorPeer);
		}

		public bool Equals(CollectorPeer other)
		{
			if (other == null) return false;
			return PeerIPAddress.Equals(other.PeerIPAddress)
					&& PeerAS == other.PeerAS;
		}
		public override int GetHashCode()
		{
			return HashCode.Combine(PeerIPAddress, PeerAS);
		}

		public static bool operator ==(CollectorPeer peer1, CollectorPeer peer2)
		{
			if(peer1 is null) return peer2 is null;
			return peer1.Equals(peer2);
		}
		public static bool operator !=(CollectorPeer peer1, CollectorPeer peer2)
		{
			return !(peer1 == peer2);
		}

	}
}
