using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class StartTypingMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                RoomUnit user = session.GetHabbo().GetRoomSession().CurrentRoomRoomUser;
                if (user != null && user.Room != null)
                {
                    user.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.TypingStatus, new ValueHolder().AddValue("VirtualID", user.VirtualID).AddValue("Typing", true)));
                }
            }
        }
    }
}
