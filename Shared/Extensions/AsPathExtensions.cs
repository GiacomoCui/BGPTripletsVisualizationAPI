using System.Collections.Generic;

namespace Shared.Extensions
{
    public static class AsPathExtensions
    {

        public static bool? IsUFirstOccurrenceInFlattenAsPath(this IEnumerable<uint> asPath, uint u, uint v)
        {
            foreach (uint asElement in asPath)
            {
                if (asElement == u)
                {
                    return true;
                }

                if (asElement == v)
                {
                    return false;
                }
            }
            return null;
        }
    }
}
