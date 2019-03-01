using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Achievements
{
    public class AchievementLevel
    {
        public readonly int ID;
        public readonly int Level;
        public readonly bool DynamicBadgeLevel;
        public readonly string Badge;
        public readonly int ActivityPointsType;
        public readonly int ActivityPoints;
        public readonly int Score;
        public readonly int ProgressNeeded;

        public AchievementLevel(int id, int level, bool dynamicBadgeLevel, string badge, int activityPointsType, int activityPoints, int score, int progressNeeded)
        {
            this.ID = id;
            this.Level = level;
            this.DynamicBadgeLevel = dynamicBadgeLevel;
            this.Badge = badge;
            this.ActivityPointsType = activityPointsType;
            this.ActivityPoints = activityPoints;
            this.Score = score;
            this.ProgressNeeded = progressNeeded;
        }

        public string LevelBadge
        {
            get
            {
                if (this.DynamicBadgeLevel)
                {
                    return this.Badge + this.Level;
                }
                else
                {
                    return this.Badge;
                }
            }
        }
    }
}
