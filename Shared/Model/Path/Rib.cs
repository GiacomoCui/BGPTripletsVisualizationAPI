using Shared.Model.BGP;
using Shared.Model.Caida;
using Shared.Model.Raw;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Model.Path
{
    public class Rib : IEnumerable<RIBAsPath>
	{
		private readonly List<RIBAsPath> asPaths;
		private HashSet<CollectorPeer> collectorPeers;
		private HashSet<AutonomousSystem> origins;
		public int Count => asPaths.Count;

		public Rib(HashSet<CollectorPeer> cps, List<RIBAsPath> paths)
		{
			asPaths = paths;
			collectorPeers = cps;
		}

		public IEnumerable<RIBAsPath> AsPaths => asPaths;

		public IEnumerable<CollectorPeer> CollectorPeers()
		{
			if (collectorPeers is null)
			{
				collectorPeers = new();
				asPaths.ForEach(path => collectorPeers.Add(path.CollectorPeer));
			}
			return collectorPeers;
		}

		public IEnumerable<AutonomousSystem> Origins()
		{
			if (origins is null)
			{
				origins = new();
				asPaths.ForEach(path => origins.Add(path.Origin));
			}
			return origins;
		}

		public IEnumerator<RIBAsPath> GetEnumerator()
		{
			return asPaths.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return asPaths.GetEnumerator();
		}
	}
}
