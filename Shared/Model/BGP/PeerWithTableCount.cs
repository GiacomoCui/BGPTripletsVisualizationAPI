using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Shared.Enums;

namespace Shared.Model.BGP
{
	public class PeerWithTableCount : CollectorPeer, IEquatable<PeerWithTableCount>, IEquatable<CollectorPeer>
	{
		public string Id { get; set; }
		public int PrefixCount { get; set; }
		public RouteCollector rRC { get; set; }
		public PeerWithTableCount(IPAddress localIPAddress, uint localASn) : base(localIPAddress, localASn)
		{
		}

		public bool IsFullTable()
		{
			if (PeerIPAddress.AddressFamily == AddressFamily.InterNetwork) return PrefixCount >= Constants.FullIPv4TableSize;
			else return PrefixCount >= Constants.FullIPv6TableSize;
		}

		public bool Equals(PeerWithTableCount other)
		{
			return Equals(other as CollectorPeer);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as PeerWithTableCount);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
