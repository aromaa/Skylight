using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class MyRoomsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message_.Init(r63bOutgoing.Navigator);
            message_.AppendInt32(5);
            message_.AppendString("");
            message_.AppendInt32(session.GetHabbo().UserRooms.Count);
            foreach (uint roomId in session.GetHabbo().UserRooms)
            {
                Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(roomId).Serialize(message_, false);
            }
            message_.AppendBoolean(false); //show featured room

            session.SendMessage(message_);
        }
    }
}
