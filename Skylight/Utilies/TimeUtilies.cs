using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class TimeUtilies
    {
        public static double GetUnixTimestamp()
        {
            return (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        public static DateTime UnixTimestampToDateTime(double unix)
        {
            DateTime result = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return result.AddSeconds(unix).ToLocalTime();
        }
    }
}
