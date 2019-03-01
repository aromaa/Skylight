using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users.Badges
{
    public class BadgeManager
    {
        private readonly uint HabboID;
        private readonly Habbo Habbo;
        private readonly Dictionary<string, Badge> Badges;

        private readonly static Dictionary<string, int> BadgeIDs = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private static int BadgeIDCounter = 1;

        public static int GetBadgeID(string badge)
        {
            if (BadgeManager.BadgeIDs.ContainsKey(badge))
            {
                return BadgeManager.BadgeIDs[badge];
            }
            else
            {
                int nextId = BadgeManager.BadgeIDCounter++;
                BadgeManager.BadgeIDs.Add(badge, nextId);
                return nextId;
            }
        }

        public BadgeManager(uint habboId, Habbo habbo, UserDataFactory userDataFactory)
        {
            this.Badges = new Dictionary<string, Badge>(StringComparer.OrdinalIgnoreCase);
            this.HabboID = habboId;
            this.Habbo = habbo;
            
            foreach(DataRow dataRow in userDataFactory.GetBadges()?.Rows)
            {
                string badgeId = (string)dataRow["badge_id"];
                this.Badges.Add(badgeId, new Badge(badgeId, (int)dataRow["badge_slot"]));
            }
        }

        public Badge TryGetBadge(string badgeId)
        {
            Badge badge;
            this.Badges.TryGetValue(badgeId, out badge);
            return badge;
        }

        public bool HaveBadge(string badgeId)
        {
            return this.Badges.ContainsKey(badgeId);
        }

        public void AddBadge(string badgeId, int slotId, bool saveToDB)
        {
            if (!this.HaveBadge(badgeId))
            {
                if (saveToDB)
                {
                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("userId", this.HabboID);
                        dbClient.AddParamWithValue("badge", badgeId);
                        dbClient.AddParamWithValue("slotId", slotId);

                        dbClient.ExecuteQuery("INSERT INTO user_badges(user_id, badge_id, badge_slot) VALUES(@userId, @badge, @slotId)");
                    }
                }

                this.Badges.Add(badgeId, new Badge(badgeId, slotId));

                if (this.Habbo.GetSession().Revision > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
                {
                    this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.UnseenItem).Handle(new ValueHolder().AddValue("Type", 4).AddValue("ID", BadgeManager.GetBadgeID(badgeId))));
                }
                this.SendAllBadges();
            }
        }

        public void SendAllBadges()
        {
            this.Habbo.GetSession().SendMessage(BasicUtilies.GetRevisionPacketManager(this.Habbo.GetSession().Revision).GetOutgoing(OutgoingPacketsEnum.Badges).Handle(new ValueHolder().AddValue("Badges", this.Badges.Values.ToList()))); //we don't want order whole badge list, fixes issue badges not showing on correct slot id
        }

        public void ClearActiveBadges()
        {
            foreach(Badge badge in this.Badges.Values)
            {
                badge.SlotID = 0;
            }
        }

        public List<Badge> GetActiveBadges()
        {
            return this.Badges.Values.Where(b => b.SlotID > 0).OrderBy(b => b.SlotID).ToList(); //order by slot id to show them at right order!
        }

        public List<Badge> GetBadges()
        {
            return this.Badges.Values.ToList();
        }

        public void RemoveBadge(string badgeId)
        {
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userId", this.HabboID);
                dbClient.AddParamWithValue("badgeId", badgeId);
                dbClient.ExecuteQuery("DELETE FROM user_badges WHERE badge_id = @badgeId AND user_id = @userId LIMIT 1");
            }

            this.Badges.Remove(badgeId);
        }
    }
}
