using System;
using System.Collections.Generic;

namespace Shared.Model.ASInternalGraph.GraphElements
{

	public class ASVertex : IEquatable<ASVertex>
	{
		public uint ASn { get; protected set; }
		public bool IsCP { get; set; }
		public ASVertex() { }
		public ASVertex(uint v)
		{
			ASn = v;
			IsCP = false;
		}
		public override string ToString()
		{
			return $"{ASn}";
		}

		public override bool Equals(object obj)
		{
			return obj is not null && obj is ASVertex vertex &&
				   Equals(vertex);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(ASn);
		}

		public bool Equals(ASVertex other)
		{
			return other is not null && ASn == other.ASn;
		}

		public virtual string DetailsString()
		{
			return $"{ASn}";
		}

		public static bool operator ==(ASVertex left, ASVertex right)
		{
			return EqualityComparer<ASVertex>.Default.Equals(left, right);
		}

		public static bool operator !=(ASVertex left, ASVertex right)
		{
			return !(left == right);
		}
	}

}