using SkylightEmulator.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Talent
{
    public class TalentTrack
    {
        private Dictionary<int, TalentTrackLevel> Levels;

        public readonly string Name;

        public TalentTrack(string name)
        {
            this.Levels = new Dictionary<int, TalentTrackLevel>();

            this.Name = name;
        }

        public List<string> GetPerks(Habbo habbo)
        {
            List<string> perks = new List<string>();
            foreach(TalentTrackLevel level in this.Levels.Values)
            {
                if (level.HasCompleted(habbo))
                {
                    perks.AddRange(level.PrizePerks);
                }
                else
                {
                    break;
                }
            }
            return perks;
        }

        public void AddLevel(TalentTrackLevel level)
        {
            this.Levels.Add(level.Level, level);
        }

        public ICollection<TalentTrackLevel> GetLevels()
        {
            return this.Levels.Values;
        }
    }
}
