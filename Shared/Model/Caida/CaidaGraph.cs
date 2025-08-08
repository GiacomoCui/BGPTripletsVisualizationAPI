using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Model.BGP;
using static Shared.Enums;

namespace Shared.Model.Caida
{
    public class CaidaGraph
    {
		public Dictionary<uint, AutonomousSystem> asesSet { get; protected set; }

		public Dictionary<AutonomousSystem, uint> _AsHeigths { get; protected set; }
		public int Count => asesSet.Count;

        public HashSet<AutonomousSystem> tierOnes;
        public HashSet<uint> IXPs { get; protected set; }

        protected CaidaGraph() { }
        public CaidaGraph(Dictionary<(uint, uint), ASRelationship> relationships, uint[] iXPs, HashSet<uint> tierOnes)
        {
            Dictionary<uint, AutonomousSystem> asesSet = new();

            foreach (((uint uintAs1, uint uintAs2), ASRelationship rel) in relationships)
            {
                if (!asesSet.TryGetValue(uintAs1, out AutonomousSystem as1))
                {
                    as1 = new(uintAs1);
                    asesSet.Add(uintAs1, as1);
                }
                if (!asesSet.TryGetValue(uintAs2, out AutonomousSystem as2))
                {
                    as2 = new(uintAs2);
                    asesSet.Add(uintAs2, as2);
                }

                if (rel == ASRelationship.PEER)
                {
                    as1.AddPeer(as2);
                    as2.AddPeer(as1);
                }
                else if(rel == ASRelationship.PROVIDER_CUSTOMER)
                {
                    //AS1 è il PROVIDER
                    as1.AddCustomer(as2);
                    //AS2 è il CUSTOMER
                    as2.AddUpstream(as1);
                }else if(rel == ASRelationship.CUSTOMER_PROVIDER)
                {
                    as2.AddCustomer(as1);
                    as1.AddUpstream(as2);
                }
            }
            this.asesSet = asesSet;
            this.tierOnes = tierOnes?.Select(asn => asesSet[asn]).ToHashSet();
            if(this.tierOnes is null) ExtractTierOnes();
            ComputeASHeights();
            IXPs = iXPs.ToHashSet();
            TagASType();
        }
        
        private void ExtractTierOnes()
        {
            tierOnes = asesSet.Values.AsParallel().Where(x => x.UpstreamASes.Count == 0 && x.CustomerASes.Count != 0).ToHashSet();
        }
        
        private void ComputeASHeights()
        {
            ConcurrentDictionary<AutonomousSystem, uint> maxHeightsFromTiers1 = new();

            foreach (AutonomousSystem node in tierOnes)
            {
                maxHeightsFromTiers1.TryAdd(node, 1);
            }

            Parallel.ForEach(tierOnes, tier1 =>
            {
                DFScount(tier1, new HashSet<AutonomousSystem>(), maxHeightsFromTiers1, 1);
            });
            Parallel.ForEach(AllASes.Where(asn => !maxHeightsFromTiers1.ContainsKey(asn) && asn.UpstreamASes.Count == 0), asn =>
            {
                maxHeightsFromTiers1.TryAdd(asn, 1);
                DFScount(asn, new(), maxHeightsFromTiers1, 1);
            });
            _AsHeigths = new Dictionary<AutonomousSystem, uint>(maxHeightsFromTiers1);
        }
        
        private void DFScount(AutonomousSystem node, HashSet<AutonomousSystem> visited, ConcurrentDictionary<AutonomousSystem, uint> maxHeightsFromTiers1, uint height)
        {
            if (visited.Contains(node))
                return;

            visited.Add(node);

            foreach (AutonomousSystem customer in node.CustomerASes)
            {
                uint newHeight = height + 1;

                maxHeightsFromTiers1.AddOrUpdate(customer, newHeight, (key, oldHeight) =>
                {
                    return newHeight > oldHeight ? newHeight : oldHeight;
                });

                DFScount(customer, visited, maxHeightsFromTiers1, height + 1);
            }
        }

        public AutonomousSystem GetAsFromUInt(uint asn)
        {
            return asesSet[asn];
        }

        public IEnumerable<AutonomousSystem> AllASes
        {
            get { return asesSet.Values; }
        }

        public bool IsAncestorProviderOf(AutonomousSystem provider, AutonomousSystem customer)
        {
            return GetLowestCommonProvider(new() { provider, customer }) == provider;
        }

        public ASRelationship GetAncestorRelationship(AutonomousSystem as1, AutonomousSystem as2)
        {
            AutonomousSystem lowestCommonProvider = GetLowestCommonProvider(new() { as1, as2 });
            if (lowestCommonProvider == null) return ASRelationship.NONE;
            if (lowestCommonProvider == as1) return ASRelationship.PROVIDER_CUSTOMER;
            if (lowestCommonProvider == as2) return ASRelationship.CUSTOMER_PROVIDER;
            return ASRelationship.NONE;
        }

        public AutonomousSystem GetLowestCommonProvider(HashSet<AutonomousSystem> startingASes)
        {
            Dictionary<AutonomousSystem, HashSet<uint>> as2containedOrigins = startingASes.ToDictionary(x => x, x => new HashSet<uint>() { x.AsNumber });
            Dictionary<AutonomousSystem, HashSet<AutonomousSystem>> as2ProvidersQueue = startingASes.ToDictionary(x => x, x => x.UpstreamASes.ToHashSet()); //Costruisco un nuovo hashSet uguale per poterlo modificare
            int ASoriginsSize = startingASes.Count;
            uint maxHeight = startingASes.Max(x => _AsHeigths[x]);

            while (maxHeight > 0)
            {
                List<AutonomousSystem> nodesAtHeight = as2containedOrigins.Where(node => _AsHeigths[node.Key] == maxHeight).Select(x => x.Key).ToList(); //trov	o tutti i nodi con la massima altezza e li controllo
                foreach (AutonomousSystem node in nodesAtHeight)
                {
                    if (as2containedOrigins[node].Count == ASoriginsSize)
                    {
                        return node; //May be more than one!
                    }
                }
                List<AutonomousSystem> asBelow = as2containedOrigins.Keys.Where(x => _AsHeigths[x] >= maxHeight).ToList(); //Seleziono gli AS ancora in coda sotto il layer corrente (=> di cui devo vedere altri providers)
                foreach (AutonomousSystem node in asBelow) //per ognuno di essi tolgo dalla sua coda providers tutti i suoi provider che stanno di 1 sopra di lui e li aggiungo alla mappa
                {
                    List<AutonomousSystem> providers = as2ProvidersQueue[node].Where(provider =>
                                                            _AsHeigths[provider] == maxHeight - 1).ToList();

                    foreach (AutonomousSystem provider in providers)
                    {
                        if (!as2containedOrigins.ContainsKey(provider))
                        {
                            as2containedOrigins.Add(provider, new(as2containedOrigins[node]));
                            as2ProvidersQueue[provider] = provider.UpstreamASes.Select(x => x).ToHashSet(); //Costruisco un nuovo hashSet per poterlo modificare
                        }
                        else
                        {
                            as2containedOrigins[provider].UnionWith(as2containedOrigins[node]);

                        }
                        as2ProvidersQueue[node].Remove(provider);
                    }

                    if (as2ProvidersQueue[node].Count == 0)
                        as2containedOrigins.Remove(node);
                }

                maxHeight--;
            }

            return null; // Per gli ASorigins i quali non è possibile trovare un customer cone che li contiene tutti. 
        }

        public bool IsIXP(uint asn)
        {
            if (IXPs != null && IXPs.Count > 0)
            {
                return IXPs.Contains(asn);
            }
            return false;
        }

        public bool ContainsEdge(uint asn1, uint asn2)
        {
            return asesSet.ContainsKey(asn1)
                && asesSet[asn1].IsConnectedTo(asesSet.GetValueOrDefault(asn2, null));
        }

        public ASRelationship GetAncestorRelationship(uint prec, uint current)
        {
            if (asesSet.ContainsKey(prec) && asesSet.ContainsKey(current))
            {
                return GetAncestorRelationship(asesSet[prec], asesSet[current]);
            }
            return ASRelationship.ERROR;
        }

		public bool IsLargeAS(uint key)
		{

            return asesSet.ContainsKey(key) && asesSet[key].NeighborCount >= Constants.LARGE_AS_NEIGHBORS;
		}
	
        public static bool IsViolation(ASRelationship fromRel, ASRelationship toRel)
        {
            return (fromRel == ASRelationship.PROVIDER_CUSTOMER || fromRel == ASRelationship.PEER) &&
                    (toRel == ASRelationship.CUSTOMER_PROVIDER || toRel == ASRelationship.PEER);
        }

		public void TagASType()
		{
			if (tierOnes.Count == 0) ExtractTierOnes();
			foreach (AutonomousSystem tier1 in tierOnes)
			{
				tier1.HierarchyTag = ASHierarchyType.TIER_1;
			}
			foreach (AutonomousSystem stub in asesSet.Values.AsParallel().Where(AS => AS.CustomerASes.Count == 0 && AS.UpstreamASes.Count != 0))
			{
				if (stub.UpstreamASes.Count == 1) stub.HierarchyTag = ASHierarchyType.SINGLE_HOMED_STUB;
				else stub.HierarchyTag = ASHierarchyType.MULTI_HOMED_STUB;
			}

			var onlyStubCustASes = asesSet.Values.AsParallel().Where(AS =>
			{
				if (AS.CustomerASes.Count == 0) return false;
				return AS.CustomerASes.All(customer => customer.HierarchyTag == ASHierarchyType.SINGLE_HOMED_STUB ||
													customer.HierarchyTag == ASHierarchyType.MULTI_HOMED_STUB);
			});
			foreach (AutonomousSystem onlyStubCust in onlyStubCustASes)
			{
				onlyStubCust.HierarchyTag = ASHierarchyType.SMALL_ISP;
			}

			var noStubCust = asesSet.Values.AsParallel().Where(AS =>
			{
				if (AS.CustomerASes.Count == 0 || AS.HierarchyTag != ASHierarchyType.NONE) return false;
				return AS.CustomerASes.Any(customer => customer.HierarchyTag != ASHierarchyType.SINGLE_HOMED_STUB &&
													customer.HierarchyTag != ASHierarchyType.MULTI_HOMED_STUB);
			});
			foreach (AutonomousSystem isp in noStubCust)
			{
				if (isp.UpstreamASes.Any(provider => provider.HierarchyTag == ASHierarchyType.TIER_1))
					isp.HierarchyTag = ASHierarchyType.LARGE_ISP;
				else isp.HierarchyTag = ASHierarchyType.MEDIUM_ISP;

			}
		}
	}
}
