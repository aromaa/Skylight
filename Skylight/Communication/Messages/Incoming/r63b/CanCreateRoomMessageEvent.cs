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
    class CanCreateRoomMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message_.Init(r63bOutgoing.CanCreateRoom);
            message_.AppendInt32(session.GetHabbo().UserRooms.Count > ServerConfiguration.MaxRoomsPerUser ? 1 : 0);
            message_.AppendInt32(ServerConfiguration.MaxRoomsPerUser);
            session.SendMessage(message_);
        }
    }
}
