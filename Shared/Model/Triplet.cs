using System;

namespace Shared.Model
{
	public class Triplet : IEquatable<Triplet>
	{
		public string Id { get; set; }

		public uint Current { get; set; }
		public uint Prec { get; set; }
		public uint Succ { get; set; }


		public Triplet(uint prec, uint current, uint succ)
		{
			Current = current;
			Prec = prec;
			Succ = succ;
		}

		public Triplet(Triplet t) : this(t.Prec, t.Current, t.Succ)
		{ }

		public override bool Equals(object obj)
		{
			return Equals(obj as Triplet);
		}

		public bool Equals(Triplet other)
		{
			return other is not null &&
					other.Current == Current &&
					other.Prec == Prec &&
					other.Succ == Succ;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Prec, Current, Succ);
		}
		public static bool operator ==(Triplet left, Triplet right)
		{
			if (left is null) return right is null;
			return left.Equals(right);
		}

		public static bool operator !=(Triplet left, Triplet right)
		{
			return !(left == right);
		}
	}
}