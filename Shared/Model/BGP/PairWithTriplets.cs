using MRTSharp.Model.IP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.BGP
{
	public class PairWithTriplets : IEquatable<PairWithTriplets>
	{
		public string Id { get; set; }
		public CollectorPeer CollectorPeer { get; set; }
		public IPPrefix Prefix { get; set; }

		public HashSet<uint> Origins { get; set; }
		public HashSet<Triplet> Triplets { get; set; }

		public PairWithTriplets(CollectorPeer collectorPeer, IPPrefix prefix)
		{
			CollectorPeer = collectorPeer;
			Prefix = prefix;
			Triplets = new();
			Origins = new();
		}

		public void AddTriplet(Triplet triplet, uint origin)
		{
			Triplets.Add(triplet);
			Origins.Add(origin);
		}

		public bool Contains(Triplet triplet)
		{
			return Triplets.Contains(triplet);
		}

		public bool Equals(PairWithTriplets other)
		{
			return other is not null &&
					Prefix.Equals(other.Prefix) &&
					CollectorPeer.Equals(other.CollectorPeer);
		}
		public override bool Equals(object obj)
		{
			return base.Equals(obj as PairWithTriplets);
		}
		public override int GetHashCode()
		{
			return HashCode.Combine(Prefix, CollectorPeer);
		}

		public void Merge(PairWithTriplets tr2)
		{
			foreach(Triplet t in tr2.Triplets)
			{
				Triplets.Add(t);
			}
			foreach(uint origin in tr2.Origins)
				Origins.Add(origin);
		}
	}
}
