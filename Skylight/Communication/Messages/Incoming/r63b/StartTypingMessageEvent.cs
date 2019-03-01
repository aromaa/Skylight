using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Utilies;

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
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
