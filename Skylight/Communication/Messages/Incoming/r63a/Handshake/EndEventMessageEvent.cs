using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class EndEventMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null && room.RoomEvent != null)
            {
                room.RoomEvent = null;

                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_.Init(r63aOutgoing.RoomEvent);
                message_.AppendString("-1");
                room.SendToAll(message_);
            }
        }
    }
}
