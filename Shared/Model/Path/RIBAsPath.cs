using MRTSharp.Model.IP;
using Shared.Comparers;
using Shared.Model.BGP;
using Shared.Model.Caida;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Model.Path
{
    public class RIBAsPath : IEquatable<RIBAsPath>, IASPath
	{
		
		public readonly CollectorPeer CollectorPeer;
		public readonly AutonomousSystem Origin;
		public readonly IPPrefix Prefix;
		public readonly ASPath ASPath;
		public int Length => ASPath.Path.Count;
		public int Count => Length;

		private static IndexableEqualityComparer<AutonomousSystem> arrayEqualityComparer = new();

		public RIBAsPath(IPPrefix prefix, CollectorPeer cp, uint[] path, CaidaGraph caida)
		{
			Prefix = prefix;
			ASPath = new(path, caida);
			CollectorPeer = cp;
			Origin = ASPath.Path.Last();
		}

		public AutonomousSystem GetAsFromIndex(int index)
		{
			return ASPath.Path[index];
		}
		public int IndexOf(AutonomousSystem item)
		{
			for(int i = 0; i < Length; i++)
			{
				if (ASPath.Path[i] == item) return i;
			}
			return -1;
		}

		public AutonomousSystem this[int index]
		{
			get => ASPath[index];
		}
		AutonomousSystem IASPath.this[int i] 
		{
			get => ASPath[i]; 
			set => ASPath[i] = value; 
		}

		public bool Equals(RIBAsPath other)
		{
			return ASPath.Equals(other.ASPath);
		}
		public override int GetHashCode()
		{
			return ASPath.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			return obj is RIBAsPath other && Equals(other);
		}

		public override string ToString()
		{
			return $"{Prefix} {ASPath}";
		}

		public IEnumerator<AutonomousSystem> GetEnumerator()
		{
			return ASPath.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ASPath.GetEnumerator();
		}

		public bool Equals(IASPath other)
		{
			return arrayEqualityComparer.Equals(other?.ToArray(), this.ToArray());
		}
	}
}
