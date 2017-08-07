using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arachnode.Utilities
{
    public static class DateTimes
    {
        public static DateTime FromUnixTime(this long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static int ToUnixTime(this DateTime date)
        {
            if(date == DateTime.MinValue)
            {
                return 0;
            }

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt32((date - epoch).TotalSeconds);
        }
    }
}
