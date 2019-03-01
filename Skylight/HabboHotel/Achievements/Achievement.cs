using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Achievements
{
    public class Achievement
    {
        public Dictionary<int, AchievementLevel> Levels;
        public readonly string Category;
        public readonly string GroupName;

        public int ID
        {
            get
            {
                return this.Levels[1].ID;
            }
        }

        public int LevelsCount
        {
            get
            {
                return this.Levels.Count;
            }
        }

        public Achievement(string category, string groupName)
        {
            this.Levels = new Dictionary<int, AchievementLevel>();
            this.Category = category;
            this.GroupName = groupName;
        }

        public void AddLevel(AchievementLevel level)
        {
            this.Levels.Add(level.Level, level);
        }

        public AchievementLevel GetLevel(int level)
        {
            this.Levels.TryGetValue(level, out AchievementLevel level_);
            return level_;
        }
    }
}
