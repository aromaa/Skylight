using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Users.Badges;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class SetActiveBadgesMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetBadgeManager() != null)
            {
                session.GetHabbo().GetBadgeManager().ClearActiveBadges();

                List<Badge> badges = new List<Badge>();
                while (message.GetRemainingLength() > 0) //dosen't send "amount", by default its allways 5
                {
                    int slot = message.PopWiredInt32();
                    string badgeId = message.PopFixedString();

                    if (slot > 0 && slot < 6) //slots 1-5
                    {
                        if (!string.IsNullOrEmpty(badgeId))
                        {
                            Badge badge = session.GetHabbo().GetBadgeManager().TryGetBadge(badgeId);
                            if (badge != null)
                            {
                                badge.SlotID = slot;

                                badges.Add(badge);
                            }
                        }
                    }
                }

                string query = "UPDATE user_badges SET badge_slot = 0 WHERE user_id = @userId; ";
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                    for (int i = 0; i < badges.Count; i++)
                    {
                        Badge badge = badges[i];

                        dbClient.AddParamWithValue("badgeSlot" + i, badge.SlotID);
                        dbClient.AddParamWithValue("badgeId" + i, badge.BadgeID);
                        query += "UPDATE user_badges SET badge_slot = @badgeSlot" + i + " WHERE badge_id = @badgeId" + i + " AND user_id = @userId LIMIT 1; ";
                    }

                    dbClient.ExecuteQuery(query);
                }

                if (session.GetHabbo().GetRoomSession() != null && session.GetHabbo().GetRoomSession().CurrentRoomRoomUser != null)
                {
                    session.GetHabbo().GetRoomSession().GetRoom().SendToAll(OutgoingPacketsEnum.ActiveBadges, new ValueHolder("UserID", session.GetHabbo().ID, "Badges", badges));
                }
                else
                {
                    session.SendMessage(OutgoingPacketsEnum.ActiveBadges, new ValueHolder("UserID", session.GetHabbo().ID, "Badges", badges));
                }
            }
        }
    }
}
