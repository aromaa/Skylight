using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
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
    class RemoveFavouriteRoomMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            uint roomId = message.PopWiredUInt();
            if (session.GetHabbo().FavouriteRooms.Contains(roomId))
            {
                session.GetHabbo().FavouriteRooms.Remove(roomId);

                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_.Init(r63aOutgoing.FavouriteChange);
                message_.AppendUInt(roomId);
                message_.AppendBoolean(false);
                session.SendMessage(message_);

                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                    dbClient.AddParamWithValue("roomId", roomId);

                    dbClient.ExecuteQuery("DELETE FROM user_favorites WHERE user_id = @userId AND room_id = @roomId LIMIT 1");
                }
            }
        }
    }
}
