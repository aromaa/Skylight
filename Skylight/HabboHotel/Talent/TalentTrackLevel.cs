using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Achievements;
using SkylightEmulator.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Talent
{
    public class TalentTrackLevel
    {
        public readonly int Level;
        public readonly Dictionary<string, int> AchievementsRequired; //id, level
        public readonly List<String> PrizePerks;
        public readonly Dictionary<uint, int> PrizeItems; //name, amount

        public TalentTrackLevel(int level, Dictionary<string, int> achievementsRequired, List<string> prizePerks, Dictionary<uint, int> prizeItems)
        {
            this.Level = level;
            this.AchievementsRequired = achievementsRequired;
            this.PrizePerks = prizePerks;
            this.PrizeItems = prizeItems;
        }

        public bool HasCompleted(Habbo habbo)
        {
            foreach (KeyValuePair<string, int> achievement in this.AchievementsRequired)
            {
                AchievementLevel achievementLevel = Skylight.GetGame().GetAchievementManager().GetAchievement(achievement.Key).GetLevel(achievement.Value);
                if (habbo.GetUserAchievements().GetAchievementLevel(achievement.Key) < achievementLevel.Level)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
