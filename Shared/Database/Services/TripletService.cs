using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Shared.Model;
using Shared.Model.BGP;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Shared.Database.Services
{
	public class TripletService
	{
		private MongoConnector _connector;
		private IMongoCollection<TripletWithData> _triplets;

		public TripletService(MongoConnector connector)
		{
			_connector = connector;
			_triplets = connector.GetTripletCollection();
		}

		public TripletWithData GetByID(string id)
		{

			return _triplets.Find<TripletWithData>(triplet => triplet.Id == id)/*.Project<TripletWithData>(fields)*/.FirstOrDefault();
		}

		public HashSet<uint> GetAllMiddleASes()
		{
			return _triplets.AsQueryable().Select(t => t.Current).Distinct().ToHashSet();
		}

		public HashSet<TripletWithData> GetAllTripletsOfAs(uint asn, AddressFamily family = AddressFamily.Unknown)
		{
			FilterDefinition<TripletWithData> selector = Builders<TripletWithData>.Filter.Where(t => t.Current == asn);
			if (family == AddressFamily.InterNetwork)
				selector = Builders<TripletWithData>.Filter.Where(t => t.Current == asn && t.HasV4Events);
			if (family == AddressFamily.InterNetworkV6)
				selector = Builders<TripletWithData>.Filter.Where(t => t.Current == asn && t.HasV6Events);

			return _triplets.Find<TripletWithData>(selector)
				/*.Project<Triplet>(projectFiedls)*/.ToEnumerable().ToHashSet();
		}

		public HashSet<TripletWithData> GetAllTripletsOfASWithCP(uint asn, CollectorPeer collector, AddressFamily family = AddressFamily.Unknown)
		{
			return _triplets.Find<TripletWithData>(t => t.Current == asn)
					.ToEnumerable().AsParallel().Where(t => t.HasPeerSeen(collector, family)).ToHashSet();
		}

		public HashSet<TripletWithCounts> GetAllTripletsOfASWithCPSimpleData(uint asn, CollectorPeer collector, AddressFamily family = AddressFamily.Unknown)
		{
			return _triplets.Find(t => t.Current == asn)
					.ToEnumerable().AsParallel().Where(t => t.HasPeerSeen(collector, family)).Select(t =>
					{
						return TripletWithCounts.FromFullTriplet(t);
					}).ToHashSet();
		}



		public HashSet<CollectorPeer> GetAllTripletsCPs(uint asn, AddressFamily family = AddressFamily.Unknown)
		{
			return GetAllTripletsOfAs(asn).AsParallel()
					.SelectMany(t => t.GetSeenPeers(family)).ToHashSet();
		}

	}
}
