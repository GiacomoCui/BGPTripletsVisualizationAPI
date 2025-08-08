using MRTSharp.Model.IP;
using Shared.Model.BGP;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;

namespace Shared.Model
{

	public class TripletWithData : TripletWithCounts
	{
		public Dictionary<CollectorPeer, int> ObservedV4CollectorPeers { get; private set; }
		public Dictionary<CollectorPeer, int> ObservedV6CollectorPeers { get; private set; }

		public Dictionary<uint, int> ObserverdOrigins { get; private set; }

		public HashSet<IPPrefix> IncidencePrefix { get; private set; }


		public TripletWithData(uint prec, uint current, uint succ)
			: base(prec, current, succ)
		{
			ObservedV4CollectorPeers = new();
			ObservedV6CollectorPeers = new();
			IncidencePrefix = new();
			ObserverdOrigins = new();
		}


		public bool HasPeerOfAS(CollectorPeer peer, AddressFamily family = AddressFamily.Unknown)
		{
			bool HasV4 = ObservedV4CollectorPeers.Keys.Any(k => k.PeerAS == peer.PeerAS);
			if (family == AddressFamily.InterNetwork) return HasV4;
			bool HasV6 = ObservedV6CollectorPeers.Keys.Any(k => k.PeerAS == peer.PeerAS);
			if (family == AddressFamily.InterNetworkV6) return HasV6;
			return HasV4 || HasV6;
		}

		public bool HasPeerSeen(CollectorPeer peer, AddressFamily family = AddressFamily.Unknown)
		{
			bool HasV4 = ObservedV4CollectorPeers.ContainsKey(peer);
			if (family == AddressFamily.InterNetwork) return HasV4;
			bool HasV6 = ObservedV6CollectorPeers.ContainsKey(peer);
			if (family == AddressFamily.InterNetworkV6) return HasV6;
			return HasV4 || HasV6;
		}

		public IEnumerable<CollectorPeer> GetSeenPeers(AddressFamily family = AddressFamily.Unknown)
		{
			switch (family)
			{
				case AddressFamily.InterNetwork: return ObservedV4CollectorPeers.Keys;
				case AddressFamily.InterNetworkV6: return ObservedV6CollectorPeers.Keys;
				default: return ObservedV4CollectorPeers.Keys.Concat(ObservedV6CollectorPeers.Keys).ToHashSet();
			}
		}

		public void AddObservationPair(IPPrefix prefix, CollectorPeer cp, uint origin, int count = 1)
		{
			if (IncidencePrefix.Add(prefix)) PrefixCount++;
			ObserverdOrigins[origin] = ObserverdOrigins.GetValueOrDefault(origin, 0) + count;
			if (prefix.Family == AddressFamily.InterNetwork)
			{
				int v = ObservedV4CollectorPeers.GetValueOrDefault(cp, 0);
				ObservedV4CollectorPeers[cp] = v + count;
				TotalV4PathCount += count;
			}
			else
			{
				int v = ObservedV6CollectorPeers.GetValueOrDefault(cp, 0);
				ObservedV6CollectorPeers[cp] = v + count;
				TotalV6PathCount += count;
			}
		}

		public void Merge(TripletWithData tr2)
		{
			foreach (var peer2c in tr2.ObservedV4CollectorPeers)
			{
				int v = ObservedV4CollectorPeers.GetValueOrDefault(peer2c.Key, 0);
				ObservedV4CollectorPeers[peer2c.Key] = v + peer2c.Value;
				TotalV4PathCount += peer2c.Value;
			}
			foreach (var peer2c in tr2.ObservedV6CollectorPeers)
			{
				int v = ObservedV6CollectorPeers.GetValueOrDefault(peer2c.Key, 0);
				ObservedV6CollectorPeers[peer2c.Key] = v + peer2c.Value;
				TotalV6PathCount += peer2c.Value;
			}
			foreach (IPPrefix p in tr2.IncidencePrefix)
			{
				if (IncidencePrefix.Add(p)) PrefixCount++;
			}
		}

		internal int GetPathsNumberSeenBy(uint cpAS, AddressFamily family)
		{
			int v4 = ObservedV4CollectorPeers.Where(kv => kv.Key.PeerAS == cpAS).Sum(kv => kv.Value);//.GetValueOrDefault(singleCP, 0);
			int v6 = ObservedV6CollectorPeers.Where(kv => kv.Key.PeerAS == cpAS).Sum(kv => kv.Value);//.GetValueOrDefault(singleCP, 0);
			switch (family)
			{
				case AddressFamily.InterNetwork: return v4;
				case AddressFamily.InterNetworkV6: return v6;
				case AddressFamily.Unknown: return v4 + v6;
				default: throw new InvalidEnumArgumentException();
			}
		}

		internal int GetTotalPathCount(AddressFamily family)
		{
			switch (family)
			{
				case AddressFamily.InterNetwork: return TotalV4PathCount;
				case AddressFamily.InterNetworkV6: return TotalV6PathCount;
				case AddressFamily.Unknown: return TotalPathCount;
				default: throw new InvalidEnumArgumentException();
			}
		}


	}


}
