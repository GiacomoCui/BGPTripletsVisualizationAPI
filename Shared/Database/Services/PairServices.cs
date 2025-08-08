using MongoDB.Driver;
using MRTSharp.Model.IP;
using Shared.Model;
using Shared.Model.BGP;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Shared.Database.Services
{
	public class PairServices
	{
		private MongoConnector _connector;

		public PairServices(MongoConnector connector)
		{
			_connector = connector;
		}

		public HashSet<IPPrefix> GetSeenIPsForPeer(Triplet triplet, CollectorPeer peer, AddressFamily family = AddressFamily.Unknown)
		{
			FilterDefinition<PairWithTriplets> filter = Builders<PairWithTriplets>.Filter.Where(pair => pair.CollectorPeer == peer);
			if(family != AddressFamily.Unknown)
			{
				filter &= Builders<PairWithTriplets>.Filter.Where(pair => pair.Prefix.Family == family);
			}

			return _connector.GetPairsCollection().Find(filter).ToEnumerable()
							.AsParallel()
							.Where(pair => pair.Triplets.Contains(triplet))
							.Select(pair => pair.Prefix).ToHashSet();
		}

	}
}
