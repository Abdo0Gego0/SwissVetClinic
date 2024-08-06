using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataAccess.TimeZoneUtils
{
    public static class TimeZoneConverter
    {
        public static DateTime getLocalTime(DateTime? date1,string timeZone= "Arabian Standard Time")
        {
            date1 =date1!=null?date1: DateTime.UtcNow;
            
            timeZone = timeZone.IsNullOrEmpty() ? "Arabian Standard Time" : timeZone;

            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);

            DateTime date2 = TimeZoneInfo.ConvertTime((DateTime)date1, tz);
            return date2;
        }
    }
}
