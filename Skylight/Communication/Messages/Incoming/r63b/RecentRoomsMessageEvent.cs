using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.HabboHotel.Support;
using SkylightEmulator.Core;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class RecentRoomsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            message_.Init(r63bOutgoing.Navigator);
            message_.AppendInt32(7);
            message_.AppendString("");

            List<Roomvisit> rooms = Skylight.GetGame().GetRoomvisitManager().GetRoomvisits(session.GetHabbo().ID);
            message_.AppendInt32(rooms.Count);
            foreach (Roomvisit roomvisit in rooms)
            {
                Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(roomvisit.RoomID).Serialize(message_, false);
            }
            message_.AppendBoolean(false);

            session.SendMessage(message_);
        }
    }
}
