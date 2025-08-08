using Shared.Model.ASInternalGraph;
using Shared.Model.BGP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static Shared.Enums;

namespace Shared.Model.Caida
{
    public class RelationshipCalculator
	{
		private CaidaGraph caida;
		//private Dictionary<uint, List<(AsPath path, int inx)>> prec2Paths;
		//private AutonomousSystem _middleAS;

		public RelationshipCalculator(CaidaGraph caida)
		{

			this.caida = caida;
			//Rib rib = new(TxTRIBParser.ParseRib(Locations.UpdatesFolder, new FileInfo(Locations.CaidaFile)), caida);
			//_middleAS = caida.GetAsFromUInt(middleAS);

			//prec2Paths = rib.AsPaths.AsParallel().Select(path => (path, path.IndexOf(_middleAS))).Where(p => p.Item2 > -1 && p.Item2 < p.path.Length - 1) //Prendo tutti i path con una tripletta con X
			//	.GroupBy(x => x.path.GetAsFromIndex(x.Item2 - 1).Number) //Raggruppo sul precedente
			//	.ToDictionary(g => g.Key, g => g.ToList());
		}

		public ASRelationship ComputeRelationship(uint origin, uint cp)
		{
			AutonomousSystem originAS, cpAS;
			try
			{
				originAS = caida.GetAsFromUInt(origin);
				cpAS = caida.GetAsFromUInt(cp);
			}
			catch (KeyNotFoundException)
			{
				return ASRelationship.ERROR;
			}

			return ComputeRelationship(originAS, cpAS);
		}

		public ASRelationship ComputeRelationship(AutonomousSystem originAS, AutonomousSystem cpAS)
		{
			if (originAS.IsPeerOf(cpAS)) return ASRelationship.PEER;
			return caida.GetAncestorRelationship(originAS, cpAS);
		}

		//public HashSet<AutonomousSystem> FindOriginsWithTriplet(AutonomousSystem prec, AutonomousSystem succ)
		//{
		//	try
		//	{
		//		var withTripletPats = prec2Paths[prec.Number].Where(p => p.path.GetAsFromIndex(p.inx).Equals(succ));

		//		return withTripletPats.Select(p => p.path.Origin).ToHashSet();
		//	}
		//	catch (KeyNotFoundException)
		//	{
		//		return new HashSet<AutonomousSystem>() { new AutonomousSystem(0) };
		//	}
		//}

		public AutonomousSystem GetAS(uint asN)
		{
			try
			{
				return caida.GetAsFromUInt(asN);
			}
			catch (KeyNotFoundException)
			{
				return null;
			}
		}



		public HashSet<CollectorPeer> GetCollectorsInCC(ASTripletsGraph graph, HashSet<CollectorPeer> peers)
		{
			
			(CollectorPeer, bool)[] res = new (CollectorPeer, bool)[peers.Count];
			Parallel.ForEach(peers.Select((cp, i) => (cp, i)), cp_i =>
			{
				var cp = cp_i.cp;
				var i = cp_i.i;
				res[i] = (cp, caida.GetAncestorRelationship(cp.PeerAS, graph.ASNumber) == ASRelationship.CUSTOMER_PROVIDER);
			});

			return res.Where(x => x.Item2).Select(x => x.Item1).ToHashSet();
		}
	}
}
