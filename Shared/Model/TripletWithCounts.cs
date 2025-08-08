namespace Shared.Model
{
	public class TripletWithCounts : Triplet
	{
		public int PrefixCount { get; protected set; }
		public int TotalV4PathCount { get; protected set; }
		public int TotalV6PathCount { get; protected set; }
		public int TotalPathCount => TotalV4PathCount + TotalV6PathCount;
		public bool HasV4Events => TotalV4PathCount > 0;
		public bool HasV6Events => TotalV6PathCount > 0;

		protected TripletWithCounts(uint prec, uint current, uint succ) : base(prec, current, succ)
		{

		}

		public static TripletWithCounts FromFullTriplet(TripletWithData fullTriplet)
		{
			return new TripletWithCounts(fullTriplet.Prec, fullTriplet.Current, fullTriplet.Succ)
			{
				Id = fullTriplet.Id,
				PrefixCount = fullTriplet.PrefixCount,
				TotalV4PathCount = fullTriplet.TotalV4PathCount,
				TotalV6PathCount = fullTriplet.TotalV6PathCount
			};
		}
	}
}
