using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Storage;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class SetHomeRoomMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            uint roomId = message.PopWiredUInt();
            if (session.GetHabbo().HomeRoom != roomId)
            {
                session.GetHabbo().HomeRoom = roomId;

                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("roomId", roomId);
                    dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                    dbClient.ExecuteQuery("UPDATE users SET home_room = @roomId WHERE id = @userId LIMIT 1");
                }

                session.SendMessage(OutgoingPacketsEnum.HomeRoom, new ValueHolder("HomeRoom", session.GetHabbo().HomeRoom));
            }
        }
    }
}
