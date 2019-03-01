using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
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
    class UpdateMottoMessageEvent : IncomingPacket
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

                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.UpdateUser);
                        message_.AppendInt32(-1);
                        message_.AppendString(session.GetHabbo().Look);
                        message_.AppendString(session.GetHabbo().Gender.ToLower());
                        message_.AppendString(session.GetHabbo().Motto);
                        message_.AppendInt32(session.GetHabbo().GetUserStats().AchievementPoints);
                        session.SendMessage(message_);

                        ServerMessage message_2 = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_2.Init(r63aOutgoing.UpdateUser);
                        message_2.AppendInt32(roomUser.VirtualID);
                        message_2.AppendString(session.GetHabbo().Look);
                        message_2.AppendString(session.GetHabbo().Gender.ToLower());
                        message_2.AppendString(session.GetHabbo().Motto);
                        message_2.AppendInt32(session.GetHabbo().GetUserStats().AchievementPoints);
                        roomUser.Room.SendToAll(message_2);

                        Skylight.GetGame().GetAchievementManager().AddAchievement(session, "ChangeMotto", 1);
                    }
                }
            }
        }
    }
}
