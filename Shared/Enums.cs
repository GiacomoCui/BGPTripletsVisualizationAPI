namespace Shared
{
	public class Enums
    {
        public enum RouteCollector
        {
            RRC00,
            RRC01,
            RRC02,
            RRC03,
            RRC04,
            RRC05,
            RRC06,
            RRC07,
            RRC08,
            RRC09,
            RRC10,
            RRC11,
            RRC12,
            RRC13,
            RRC14,
            RRC15,
            RRC16,
            RRC18,
            RRC19,
            RRC20,
            RRC21,
            RRC22,
            RRC23,
            RRC24,
            RRC25,
            RRC26,
            AMSIX,
            CHICAGO,
            CHILE,
            EQIX,
            FLIX,
            GOREX,
            ISC,
            KIXP,
            JINX,
            LINX,
            NAPAFRICA,
            NWAX,
            PHOIX,
            TELXATL,
            WIDE,
            SYDNEY,
            SAOPAULO,
            SG,
            PERTH,
            PERU,
            SFMIX,
            SIEX,
            SOXRS,
            MWIX,
            RIO,
            FORTALEZA,
            GIXA,
            BDIX,
            BKNIX,
            UAEIX,
            NY
        }
        
        public enum RRC
        {
            RRC00,
            RRC01,
            RRC02,
            RRC03,
            RRC04,
            RRC05,
            RRC06,
            RRC07,
            RRC08,
            RRC09,
            RRC10,
            RRC11,
            RRC12,
            RRC13,
            RRC14,
            RRC15,
            RRC16,
            RRC18,
            RRC19,
            RRC20,
            RRC21,
            RRC22,
            RRC23,
            RRC24,
            RRC25,
            RRC26
        }
        
        public enum RouteViewsCollector
        {
            AMSIX,
            CHICAGO,
            CHILE,
            EQIX,
            FLIX,
            GOREX,
            ISC,
            KIXP,
            JINX,
            LINX,
            NAPAFRICA,
            NWAX,
            PHOIX,
            TELXATL,
            WIDE,
            SYDNEY,
            SAOPAULO,
            //SG,
            PERTH,
            PERU,
            SFMIX,
            SIEX,
            SOXRS,
            MWIX,
            RIO,
            FORTALEZA,
            GIXA,
            BDIX,
            BKNIX,
            UAEIX,
            NY
        }

        public enum ASRelationship
        {
            NONE = 2,
            CUSTOMER_PROVIDER = 1,
            PROVIDER_CUSTOMER = -1,
            PEER = 0,
            ERROR = 3
        }

        public enum Preference
        {
            First,
            Second,
            None
        }
		public enum ASHierarchyType
		{
			NONE,
			TIER_1,
			LARGE_ISP,
			MEDIUM_ISP,
			SMALL_ISP,
			SINGLE_HOMED_STUB,
			MULTI_HOMED_STUB
		}

		public enum ASRelationshipRole
		{
            Violation = 4,
			Self = 3,
			Customer = 2,
			Provider = 0,
			Peer = 1
		}
		
        /// <summary>
		/// Restituisce true se a == Self e b != Self oppure se a == Down e b == Same/Up oppure se a == Same e b == Up, false altrimenti. 
		/// Il true corrisponde ad un valore di raggiungibilità di a migliore di quello di b
		/// </summary>
		public static bool CompareReachability(ASRelationshipRole a, ASRelationshipRole b)
        {
            if (a > b) return true;
            return false;
        }

    }
}
