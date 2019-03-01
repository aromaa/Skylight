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
    class UnignoreUserMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null)
            {
                string username = message.PopFixedString();
                RoomUnitUser user = room.RoomUserManager.GetUserByName(username);
                if (user != null)
                {
                    uint userId = user.Session.GetHabbo().ID;
                    if (session.GetHabbo().IgnoredUsers.Contains(userId))
                    {
                        session.GetHabbo().IgnoredUsers.Remove(userId);

                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                            dbClient.AddParamWithValue("targetId", userId);
                            dbClient.ExecuteQuery("DELETE FROM user_ignores WHERE user_id = @userId AND ignored_id = @targetId LIMIT 1");
                        }

                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.IgnoreStatus);
                        message_.AppendInt32(3);
                        session.SendMessage(message_);
                    }
                }
            }
        }
    }
}
