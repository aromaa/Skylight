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
    class SetHomeRoomMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            uint roomId = message.PopWiredUInt();
            if (session.GetHabbo().HomeRoom != roomId)
            {
                RoomData roomData = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(roomId);
                if (roomData != null && roomData.OwnerID == session.GetHabbo().ID)
                {
                    session.GetHabbo().HomeRoom = roomId;

                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("roomId", roomId);
                        dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                        dbClient.ExecuteQuery("UPDATE users SET home_room = @roomId WHERE id = @userId LIMIT 1");
                    }

                    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    message_.Init(r63aOutgoing.HomeRoom);
                    message_.AppendUInt(roomId);
                    session.SendMessage(message_);
                }
            }
        }
    }
}
