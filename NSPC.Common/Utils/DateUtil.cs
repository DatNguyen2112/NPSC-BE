using System;

namespace NSPC.Common
{
    public static class DateUtil
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTime(this DateTime datetime)
        {
            return (long)(datetime - epoch).TotalMilliseconds;
        }
    }
}