using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Storage;
using SkylightEmulator.Core;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class MottoMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null)
            {
                RoomUnit roomUser = session.GetHabbo().GetRoomSession().GetRoomUser();
                if (roomUser != null)
                {
                    string motto = TextUtilies.FilterString(message.PopFixedString());
                    if (!TextUtilies.HaveBlacklistedWords(motto))
                    {
                        session.GetHabbo().Motto = motto;

                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                            dbClient.AddParamWithValue("motto", motto);

                            dbClient.ExecuteQuery("UPDATE users SET motto = @motto WHERE id = @userId LIMIT 1");
                            if (session.GetHabbo().GetUserSettings().FriendStream)
                            {
                                dbClient.ExecuteQuery("INSERT INTO user_friend_stream(type, user_id, timestamp, extra_data) VALUES('3', @userId, UNIX_TIMESTAMP(), @motto)");
                            }
                        }

                        session.SendMessage(OutgoingPacketsEnum.UpdateUser, new ValueHolder("VirtualID", -1, "Look", session.GetHabbo().Look, "Gender", session.GetHabbo().Gender, "Motto", session.GetHabbo().Motto, "AchievementPoints", session.GetHabbo().GetUserStats().AchievementPoints));
                        roomUser.Room.SendToAll(OutgoingPacketsEnum.UpdateUser, new ValueHolder("VirtualID", roomUser.VirtualID, "Look", session.GetHabbo().Look, "Gender", session.GetHabbo().Gender, "Motto", session.GetHabbo().Motto, "AchievementPoints", session.GetHabbo().GetUserStats().AchievementPoints));

                        Skylight.GetGame().GetAchievementManager().AddAchievement(session, "ChangeMotto", 1);
                    }
                }
            }
        }
    }
}
