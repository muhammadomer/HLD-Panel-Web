using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.Helper
{
    public class DatetimeExtension
    {
        public static DateTime ConvertToEST(DateTime date)
        {
            DateTime universalDate = date.ToUniversalTime();
            var easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(universalDate, easternZone);
        }
    }
}
