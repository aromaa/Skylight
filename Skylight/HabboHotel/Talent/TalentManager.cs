using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Users;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Talent
{
    public class TalentManager
    {
        public Dictionary<string, TalentTrack> TalentTracks;

        public TalentManager()
        {
            this.TalentTracks = new Dictionary<string, TalentTrack>();
        }

        public void LoadTalents(DatabaseClient dbClient)
        {
            Logging.Write("Loading talents... ");
            this.TalentTracks.Clear();

            DataTable talents = dbClient.ReadDataTable("SELECT * FROM talents; ");
            if (talents?.Rows.Count > 0)
            {
                foreach(DataRow dataRow in talents.Rows)
                {
                    string track = (string)dataRow["track"];

                    TalentTrack track_;
                    if (!this.TalentTracks.TryGetValue(track, out track_))
                    {
                        track_ = this.TalentTracks[track] = new TalentTrack(track);
                    }

                    track_.AddLevel(new TalentTrackLevel((int)dataRow["level"], ((string)dataRow["achievements_required"]).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToDictionary(k => k.Split(',')[0], v => int.Parse(v.Split(',')[1])), ((string)dataRow["prize_perks"]).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(), ((string)dataRow["prize_items"]).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToDictionary(k => uint.Parse(k.Split(',')[0]), v => int.Parse(v.Split(',')[1]))));
                }
            }

            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public TalentTrack GetTalentTrack(string name)
        {
            this.TalentTracks.TryGetValue(name, out TalentTrack track);
            return track;
        }

        public List<string> GetPerks(Habbo habbo)
        {
            List<string> perks = new List<string>();
            foreach(TalentTrack track in this.TalentTracks.Values)
            {
                perks.AddRange(track.GetPerks(habbo));
            }
            return perks;
        }
    }
}
