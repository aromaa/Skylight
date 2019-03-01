using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Users.Badges;
using SkylightEmulator.Core;
using SkylightEmulator.Storage;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc
{
    public class SetActiveBadgesEventHandler : IncomingPacket
    {
        protected string[] Badges;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo()?.GetBadgeManager() != null)
            {
                session.GetHabbo().GetBadgeManager().ClearActiveBadges();

                StringBuilder query = new StringBuilder();
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userId", session.GetHabbo().ID);

                    for (int i = 1; i <= this.Badges.Length; i++)
                    {
                        string code = this.Badges[i - 1];
                        if (!string.IsNullOrEmpty(code))
                        {
                            Badge badge = session.GetHabbo().GetBadgeManager().TryGetBadge(code);
                            if (badge != null)
                            {
                                badge.SlotID = i + 1;

                                dbClient.AddParamWithValue("badgeId" + i, badge.BadgeID);
                                dbClient.AddParamWithValue("slotId" + i, badge.SlotID);
                                query.Append("UPDATE user_badges SET badge_slot = @slotId" + i + " WHERE badge_id = @badgeId" + i + " AND user_id = @userId LIMIT 1; ");
                            }
                        }
                    }

                    if (query.Length > 0)
                    {
                        dbClient.ExecuteQuery(query.ToString());
                    }
                }

                if (session.GetHabbo().GetRoomSession().GetRoom() != null)
                {
                    session.GetHabbo().GetRoomSession().GetRoom().SendToAll(new SendUserActiveBadgesComposerHandler(Skylight.GetGame().GetUserProfileManager().GetProfile(session.GetHabbo().ID)));
                }
                else
                {
                    session.SendMessage(new SendUserActiveBadgesComposerHandler(Skylight.GetGame().GetUserProfileManager().GetProfile(session.GetHabbo().ID)));
                }
            }
        }
    }
}
