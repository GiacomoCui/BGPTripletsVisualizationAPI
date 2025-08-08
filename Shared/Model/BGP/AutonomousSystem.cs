using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using static Shared.Enums;

namespace Shared.Model.BGP
{
    public class AutonomousSystem : IAutonomousSystem, IEquatable<AutonomousSystem>
    {
        public uint AsNumber { get; private set; }
        public ASHierarchyType HierarchyTag { get; set; } = ASHierarchyType.NONE;
		public HashSet<AutonomousSystem> PeerASes { get; private set; } = new();
        public HashSet<AutonomousSystem> CustomerASes { get; private set; } = new();
        public HashSet<AutonomousSystem> UpstreamASes { get; private set; } = new();

        public HashSet<AutonomousSystem> ViolatedPeers { get; private set; } = new();
        public HashSet<AutonomousSystem> ViolatedProviders { get; private set; } = new();

        
        public int NeighborCount => PeerASes.Count + CustomerASes.Count + UpstreamASes.Count;


		public AutonomousSystem(uint number)
        {
            AsNumber = number;
        }

        internal void AddCustomer(AutonomousSystem customer)
        {

            CustomerASes.Add(customer);
        }

        internal void AddPeer(AutonomousSystem peer)
        {
            PeerASes.Add(peer);
        }

        internal void AddUpstream(AutonomousSystem upstream)
        {
            UpstreamASes.Add(upstream);
        }

        public bool IsConnectedTo(AutonomousSystem autonomousSystem)
        {
            if (autonomousSystem is null) return false;
            return UpstreamASes.Contains(autonomousSystem) ||
                CustomerASes.Contains(autonomousSystem) ||
                PeerASes.Contains(autonomousSystem);
        }
 

        public bool IsPeerOf(AutonomousSystem peer)
        {
            return PeerASes.Contains(peer);
        }

        public bool Equals(AutonomousSystem other)
        {
            return other?.AsNumber == AsNumber;
        }
        public override int GetHashCode()
        {
            return AsNumber.GetHashCode();
        }
        public override string ToString()
        {
            return AsNumber.ToString();
        }
        public override bool Equals(object obj)
        {
            return obj is AutonomousSystem other && Equals(other);
        }

        public ASRelationship GetRelationshipWith(AutonomousSystem next)
        {
            if (PeerASes.Contains(next)) return ASRelationship.PEER;
            if (UpstreamASes.Contains(next)) return ASRelationship.CUSTOMER_PROVIDER;
            if (CustomerASes.Contains(next)) return ASRelationship.PROVIDER_CUSTOMER;
            return ASRelationship.ERROR;
        }

		public bool HasViolatedConnection(AutonomousSystem other)
		{
			return other is not null && (ViolatedPeers.Contains(other) || ViolatedProviders.Contains(other));
		}

		public ASRelationshipRole GetMyRoleWith(AutonomousSystem other)
		{
            if (HasViolatedConnection(other)) return ASRelationshipRole.Violation;
			if (PeerASes.Contains(other)) return ASRelationshipRole.Peer;
			if (UpstreamASes.Contains(other)) return ASRelationshipRole.Customer;
			if (CustomerASes.Contains(other)) return ASRelationshipRole.Provider;
			return ASRelationshipRole.Self;
		}

		public static bool operator ==(AutonomousSystem left, AutonomousSystem right)
        {
            return left is not null && left.Equals(right);
        }
        public static bool operator !=(AutonomousSystem left, AutonomousSystem right)
        {
            return !(left == right);
        }
    }
}
