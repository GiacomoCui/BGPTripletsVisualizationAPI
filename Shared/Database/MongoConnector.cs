using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Model;
using MongoDB.Bson.Serialization.IdGenerators;
using Shared.Model.BGP;
using MRTSharp.Model.IP;
using Shared.Model.Raw;

namespace Shared.Database
{
    public sealed class MongoConnector
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        public MongoConnector()
        {
            var connectionString = "mongodb://localhost:5000";

            var clientSettings = MongoClientSettings.FromConnectionString(connectionString);

            //Decommenting this lines will force mongodb driver to write to console EACH query to the db
            //clientSettings.ClusterConfigurator = cb =>
            //{
            //	cb.Subscribe<CommandStartedEvent>(e =>
            //	{
            //		Console.ForegroundColor = ConsoleColor.Cyan;
            //		Console.WriteLine($"{e.CommandName} - {e.Command.ToJson()}");
            //		Console.ResetColor();
            //	});
            //};

			//Should transform all in FromMinutes
            clientSettings.ConnectTimeout = TimeSpan.FromHours(10);

            clientSettings.MaxConnectionPoolSize = 1000;

            clientSettings.SocketTimeout = TimeSpan.FromHours(20);

            clientSettings.WaitQueueTimeout = TimeSpan.FromHours(60);

            _client = new MongoClient(clientSettings);

            // Set up MongoDB conventions
            ConventionPack pack = new()
            {

                new EnumRepresentationConvention(BsonType.String),
                new IgnoreIfNullConvention(true),
                new MemberNameElementNameConvention(),
                new StringObjectIdIdGeneratorConventionThatWorks()
            };
            ConventionRegistry.Register("EnumStringConvention", pack, t => true);

            BsonSerializer.RegisterSerializer(typeof(uint), new UInt32Serializer(BsonType.Int32, new RepresentationConverter(true, false)));
            BsonSerializer.RegisterSerializer(typeof(ulong), new UInt64Serializer(BsonType.Int64, new RepresentationConverter(true, false)));
                                        

            #region SERIALIZATION-RULES

            BsonClassMap.RegisterClassMap<CollectorPeer>(cm =>
            {
                cm.AutoMap();
            });

            BsonClassMap.RegisterClassMap<Triplet>(cm =>
            {
				cm.MapIdMember(c => c.Id)
					.SetIdGenerator(new StringObjectIdGenerator())
					.SetSerializer(new StringSerializer(BsonType.ObjectId));
                cm.AutoMap();
			});

            BsonClassMap.RegisterClassMap<TripletWithData>(cm =>
            {
				
				cm.AutoMap();
                var serializer = new DictionaryInterfaceImplementerSerializer<Dictionary<CollectorPeer, int>>(DictionaryRepresentation.ArrayOfArrays);
                serializer = serializer.WithKeySerializer(new ObjectSerializer(type => type == typeof(CollectorPeer)));
				
                cm.MapMember(c => c.ObservedV4CollectorPeers)
                    .SetSerializer(serializer);
                cm.MapMember(c => c.ObservedV6CollectorPeers)
                    .SetSerializer(serializer);

                var serializer2 = new DictionaryInterfaceImplementerSerializer<Dictionary<CollectorPeer, HashSet<IPPrefix>>>(DictionaryRepresentation.ArrayOfArrays);
                serializer2 = serializer2.WithKeySerializer(new ObjectSerializer(type => type == typeof(CollectorPeer)));
                serializer2 = serializer2.WithValueSerializer(new ObjectSerializer(type => type == typeof(HashSet<IPPrefix>)));

				var serializer3 = new DictionaryInterfaceImplementerSerializer<Dictionary<uint, int>>(DictionaryRepresentation.ArrayOfArrays);
				cm.MapMember(c => c.ObserverdOrigins)
					.SetSerializer(serializer3);

				cm.UnmapMember(c => c.IncidencePrefix);
                cm.MapMember(c => c.TotalPathCount);
                cm.MapMember(c => c.HasV4Events);
                cm.MapMember(c => c.HasV6Events);
			});

			BsonClassMap.RegisterClassMap<PairWithTriplets>(cm =>
			{
				cm.MapIdMember(c => c.Id)
					.SetIdGenerator(new StringObjectIdGenerator())
					.SetSerializer(new StringSerializer(BsonType.ObjectId));
				cm.AutoMap();
			});

            BsonClassMap.RegisterClassMap<RibEntry>(cm =>
            {
				cm.MapIdMember(c => c.Id)
					.SetIdGenerator(new StringObjectIdGenerator())
					.SetSerializer(new StringSerializer(BsonType.ObjectId));
				cm.AutoMap();
                cm.MapMember(cm => cm.Origin).SetIsRequired(false);
				
			});

			BsonClassMap.RegisterClassMap<PeerWithTableCount>(cm =>
			{
				cm.MapIdMember(c => c.Id)
					.SetIdGenerator(new StringObjectIdGenerator())
					.SetSerializer(new StringSerializer(BsonType.ObjectId));
				cm.AutoMap();
			});

			#endregion


			_database = _client.GetDatabase("PolicyInference");

            #region INDEXES-CREATION

            CreateSingleIndex(Builders<TripletWithData>.IndexKeys.Ascending(x => x.Current), GetTripletCollection(), unique: false);
            CreateSingleIndex(Builders<TripletWithData>.IndexKeys.Ascending(x => x.Current).Ascending(x => x.Prec).Ascending(x => x.Succ), GetTripletCollection(), unique:false);   
            //CreateSingleIndex(Builders<Triplet>.IndexKeys.Ascending(x => x.ObservedCollectorPeers), GetTripletCollection(), unique:false);

            CreateSingleIndex(Builders<PairWithTriplets>.IndexKeys.Ascending(x => x.CollectorPeer), GetPairsCollection(), unique:false);
            CreateSingleIndex(Builders<PairWithTriplets>.IndexKeys.Ascending(x => x.CollectorPeer).Ascending(x => x.Prefix), GetPairsCollection(), unique: false);

			#region RIB-ENTRIES-INDEXES
			//All of a single timestamp
			CreateSingleIndex(Builders<RibEntry>.IndexKeys.Ascending(x => x.Timestamp), GetRibEntryCollection(), unique: false);


			CreateSingleIndex(Builders<RibEntry>.IndexKeys.Ascending(x => x.Timestamp).Ascending(x => x.Prefix.Family).Ascending(x => x.Origin), GetRibEntryCollection(), unique: false);

			//Single specific filters (per prefix, or Cp, or RRC)
			CreateSingleIndex(Builders<RibEntry>.IndexKeys.Ascending(x => x.Timestamp).Ascending(x => x.rRC), GetRibEntryCollection(), unique: false);
			CreateSingleIndex(Builders<RibEntry>.IndexKeys.Ascending(x => x.Timestamp).Ascending(x => x.Origin), GetRibEntryCollection(), unique: false);
			CreateSingleIndex(Builders<RibEntry>.IndexKeys.Ascending(x => x.Timestamp).Ascending(x => x.Cp), GetRibEntryCollection(), unique: false);
			CreateSingleIndex(Builders<RibEntry>.IndexKeys.Ascending(x => x.Timestamp).Ascending(x => x.Prefix), GetRibEntryCollection(), unique: false);
			
			//Pair with origin
			CreateSingleIndex(Builders<RibEntry>.IndexKeys.Ascending(x => x.Timestamp).Ascending(x => x.Cp).Ascending(x => x.Origin), GetRibEntryCollection(), unique: false);
			CreateSingleIndex(Builders<RibEntry>.IndexKeys.Ascending(x => x.Timestamp).Ascending(x => x.Prefix).Ascending(x => x.Origin), GetRibEntryCollection(), unique: false);

			//Per pair filters
			CreateSingleIndex(Builders<RibEntry>.IndexKeys.Ascending(x => x.Timestamp).Ascending(x => x.rRC).Ascending(x => x.Cp), GetRibEntryCollection(), unique: false);		
			CreateSingleIndex(Builders<RibEntry>.IndexKeys.Ascending(x => x.Timestamp).Ascending(x => x.rRC).Ascending(x => x.Prefix), GetRibEntryCollection(), unique: false);
			CreateSingleIndex(Builders<RibEntry>.IndexKeys.Ascending(x => x.Timestamp).Ascending(x => x.Prefix).Ascending(x => x.Cp), GetRibEntryCollection(), unique: false);

            //All filters togheter
			CreateSingleIndex(Builders<RibEntry>.IndexKeys.Ascending(x => x.Timestamp).Ascending(x => x.rRC).Ascending(x => x.Prefix).Ascending(x => x.Cp), GetRibEntryCollection(), unique: false);
			#endregion

			CreateSingleIndex(Builders<PeerWithTableCount>.IndexKeys.Ascending(x => x.rRC).Ascending(x => x.PrefixCount), GetFullTablePeersCollection(), unique: false);
			CreateSingleIndex(Builders<PeerWithTableCount>.IndexKeys.Ascending(x => x.PrefixCount), GetFullTablePeersCollection(), unique: false);
			CreateSingleIndex(Builders<PeerWithTableCount>.IndexKeys.Ascending(x => x.PeerAS).Ascending(x => x.PeerIPAddress), GetFullTablePeersCollection(), unique: true);
			CreateSingleIndex(Builders<PeerWithTableCount>.IndexKeys.Ascending(x => x.PeerAS).Ascending(x => x.PeerIPAddress).Ascending(x => x.PrefixCount), GetFullTablePeersCollection(), unique: true);
			#endregion
		}

		

		private static void CreateSingleIndex<T>(IndexKeysDefinition<T> indexKeys, IMongoCollection<T> collection, bool unique = false, String partialFilterExpression = null)
		{
			CreateIndexOptions<T> indexOptions = new()
			{
				Background = false,
				Unique = unique,
				PartialFilterExpression = partialFilterExpression
			};

			CreateIndexModel<T> indexModel = new(indexKeys, indexOptions);
			collection.Indexes.CreateOne(indexModel);
		}
        public IMongoCollection<TripletWithData> GetTripletCollection()
        {
            return _database.GetCollection<TripletWithData>("Triplets");
        }
        public IMongoCollection<PairWithTriplets> GetPairsCollection()
        {
            return _database.GetCollection<PairWithTriplets>("Pairs");
        }
		public IMongoCollection<RibEntry> GetRibEntryCollection()
		{
			return _database.GetCollection<RibEntry>("RIBEntries");
		}
		public IMongoCollection<PeerWithTableCount> GetFullTablePeersCollection()
		{
			return _database.GetCollection<PeerWithTableCount>("FullTablePeers");
		}
	}
}
