using System;

namespace Shared
{
    public static class Constants
    {
        
        public static readonly TimeSpan UpdatesStablePeriod = TimeSpan.FromDays(7);
        public static readonly DateTime FixedDate = new(2023, 10, 16, 00, 00, 00);


        public const int FullIPv4TableSize = 500000;
        public const int FullIPv6TableSize = 100000;
        public const int LARGE_AS_NEIGHBORS = 1000;
    }
}
