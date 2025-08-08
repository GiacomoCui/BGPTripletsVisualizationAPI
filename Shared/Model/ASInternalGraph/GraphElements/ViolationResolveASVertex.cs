using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.ASInternalGraph.GraphElements
{
	public class ViolationResolveASVertex : ASVertex, IEquatable<ViolationResolveASVertex>
	{
		public bool IsIncomingVertex { get; private set; }

		public ViolationResolveASVertex(ASVertex generatingVertex, bool isIncoming) :
			base(generatingVertex.ASn)
		{
			IsIncomingVertex = isIncoming;
		}


		public override string ToString()
		{
			return $"{(IsIncomingVertex ? "In" : "Out")} {ASn}";
		}

		public override bool Equals(object obj)
		{
			return obj is ViolationResolveASVertex vertex &&
				  Equals(vertex);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(base.GetHashCode(), IsIncomingVertex);
		}

		public bool Equals(ViolationResolveASVertex other)
		{
			return other is not null && base.Equals(other) &&
				   IsIncomingVertex == other.IsIncomingVertex;
		}
		public static bool operator ==(ViolationResolveASVertex left, ViolationResolveASVertex right)
		{
			return EqualityComparer<ViolationResolveASVertex>.Default.Equals(left, right);
		}
		public static bool operator !=(ViolationResolveASVertex left, ViolationResolveASVertex right)
		{ return !(left == right); }
	}
}
