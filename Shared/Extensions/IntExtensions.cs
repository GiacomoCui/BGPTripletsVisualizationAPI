using System;

namespace Shared.Extensions
{
    public static class IntExtensions
    {
        public static int Round(this int number, int interval)
        {
            if (interval == 0)
            {
                return number;
            }

            if (number / interval > interval / 2)
            {
                return number / interval * interval;
            }
            else
            {
                return number / interval * interval;
            }
        }

        public static int TelescopicApprox(this int number)
        {
            int numbLength = (int)Math.Log10(number) + 1;

            if (numbLength == 1)
            {
                return number;
            }

            return number.Round((int)Math.Pow(10, numbLength - 1));
        }
    }
}
