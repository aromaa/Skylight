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
    class GiveRoomRightsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            uint userId = message.PopWiredUInt();

            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null && room.HaveOwnerRights(session))
            {
                RoomUnitUser user = room.RoomUserManager.GetUserByID(userId);
                if (user != null && !room.UsersWithRights.Contains(userId))
                {
                    room.UsersWithRights.Add(userId);

                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("roomId", room.ID);
                        dbClient.AddParamWithValue("userId", userId);

                        dbClient.ExecuteQuery("INSERT INTO room_rights(room_id, user_id) VALUES(@roomId, @userId)");
                    }

                    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    message_.Init(r63aOutgoing.GotRights);
                    message_.AppendUInt(room.ID);
                    message_.AppendUInt(user.UserID);
                    message_.AppendString(user.Session.GetHabbo().Username);
                    room.SendToAllWhoHaveOwnerRights(message_);

                    user.AddStatus("flatctrl", "");

                    ServerMessage roomRights = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    roomRights.Init(r63aOutgoing.GiveRoomRights);
                    user.Session.SendMessage(roomRights);
                }
            }
        }
    }
}
