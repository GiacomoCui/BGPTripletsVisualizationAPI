using MongoDB.Driver;
using Shared.DataStructures.ConcatenatedList;
using Shared.Model.Raw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Database.Services
{
	public class RIBEntryServices
	{
		private MongoConnector connector;

		public RIBEntryServices(MongoConnector connector) 
		{
			this.connector = connector;
		}

		public HashSet<uint> GetAllOriginOfFamily(DateTime timestamp, AddressFamily family)
		{
			var filter = Builders<RibEntry>.Filter.And(new FilterDefinition<RibEntry>[] {
					 Builders<RibEntry>.Filter.Eq(x => x.Timestamp, timestamp),
					 Builders<RibEntry>.Filter.Eq(x => x.Prefix.Family, family)
					 });
			return new(connector.GetRibEntryCollection().Distinct<uint>("Origin", filter).ToEnumerable());
		}

		public int GetPathCountForOrigin(DateTime fixedDate, uint asn)
		{
			return (int)connector.GetRibEntryCollection().CountDocuments(x => x.Timestamp == fixedDate && x.Origin == asn);
		}

		public ConcatenatedLinkedList<RibEntry> GetTimestampedPathsForOrigins(DateTime timestamp, HashSet<uint> origins)
		{
			ConcatenatedLinkedList<RibEntry> paths = new();
			Parallel.ForEach(origins, new ParallelOptions(), origin =>
			{
				var filter = Builders<RibEntry>.Filter.And(new FilterDefinition<RibEntry>[] {
					 Builders<RibEntry>.Filter.Eq(x => x.Timestamp, timestamp),
					 Builders<RibEntry>.Filter.Eq(x => x.Origin, origin)
					 });
				ConcatenatedLinkedList<RibEntry> p = new(connector.GetRibEntryCollection().Find(filter).ToEnumerable());
				lock (paths) paths.Concat(p);
			});
			return paths;
		}
	}
}
