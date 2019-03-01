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
    class AddFavouriteRoomMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            uint roomId = message.PopWiredUInt();
            RoomData roomData = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(roomId);
            if (roomData != null && session.GetHabbo().FavouriteRooms.Count < 50 && !session.GetHabbo().FavouriteRooms.Contains(roomId))
            {
                session.GetHabbo().FavouriteRooms.Add(roomId);

                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_.Init(r63aOutgoing.FavouriteChange);
                message_.AppendUInt(roomId);
                message_.AppendBoolean(true);
                session.SendMessage(message_);

                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                    dbClient.AddParamWithValue("roomId", roomId);

                    dbClient.ExecuteQuery("INSERT INTO user_favorites(user_id, room_id) VALUES (@userId, @roomId)");
                }
            }
        }
    }
}
