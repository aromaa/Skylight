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
    class RemoveRoomRightsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null && room.HaveOwnerRights(session))
            {
                int amount = message.PopWiredInt32();

                List<uint> users = new List<uint>();
                for (int i = 0; i < amount; i++)
                {
                    uint userId = message.PopWiredUInt();

                    room.UsersWithRights.Remove(userId);
                    users.Add(userId);

                    RoomUnitUser user = room.RoomUserManager.GetUserByID(userId);
                    if (user != null)
                    {
                        user.RemoveStatus("flatctrl");

                        ServerMessage roomRights = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        roomRights.Init(r63aOutgoing.RemoveRoomRights);
                        user.Session.SendMessage(roomRights);
                    }

                    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    message_.Init(r63aOutgoing.LostRights);
                    message_.AppendUInt(room.ID);
                    message_.AppendUInt(userId);
                    session.SendMessage(message_);
                }

                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("roomId", room.ID);
                    dbClient.ExecuteQuery("DELETE FROM room_rights WHERE room_id = @roomId AND user_id IN(" + string.Join(",", users) + ")");
                }
            }
        }
    }
}
