using MessagePack;
using MRTSharp.Model.IP;
using Shared.Converters;
using Shared.Model.BGP;
using System;
using static Shared.Enums;

namespace Shared.Model.Raw
{
	[MessagePackObject]
	public class RibEntry
	{
		public string Id { get; set; }
		[Key(0)]
		public RawAsPath Path;
		[Key(1)]
		[MessagePackFormatter(typeof(IPPrefixFormatter))]
		public IPPrefix Prefix;
		[Key(2)]
		public CollectorPeer Cp;
		public uint Origin;
		public RouteCollector rRC { get; set; }
		[Key(3)]
		public DateTime Timestamp;

		public RibEntry(RawAsPath path, IPPrefix prefix, CollectorPeer cp, DateTime timestamp)
		{
			Path = path;
			Prefix = prefix;
			Cp = cp;
			Timestamp = timestamp;
		}
	}
}
