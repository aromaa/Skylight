using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class CurrenceUtilies
    {
        public static Dictionary<int, int> ActivityPointsToDictionary(string activityPoints) //Tries parse activity points data as best as it can, even bit corrupted/malformed
        {
            Dictionary<int, int> activityPointsData = new Dictionary<int, int>();

            int pixels;
            if (!int.TryParse(activityPoints, out pixels)) //if its only int we can assume its pixels only
            {
                foreach(string s in activityPoints.Split(';'))
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        string[] s_ = s.Split(',');

                        if (s_.Length == 2)
                        {
                            int activityPointsId;
                            int activityPointsAmount;
                            if (int.TryParse(s_[0], out activityPointsId) && int.TryParse(s_[1], out activityPointsAmount))
                            {
                                activityPointsData.Add(activityPointsId, activityPointsAmount);
                            }
                        }
                    }
                }
            }
            else
            {
                activityPointsData.Add(0, pixels);
            }
            return activityPointsData;
        }

        public static string ActivityPointsToString(Dictionary<int, int> activityPoints)
        {
            StringBuilder activityPointsData = new StringBuilder();
            foreach(KeyValuePair<int, int> data in activityPoints)
            {
                if (activityPointsData.Length > 0)
                {
                    activityPointsData.Append(";");
                }

                activityPointsData.Append(data.Key + "," + data.Value);
            }
            return activityPointsData.ToString();
        }
    }
}
