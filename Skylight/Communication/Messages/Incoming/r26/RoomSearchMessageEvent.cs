using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class RoomSearchMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            session.SendMessage(Skylight.GetGame().GetNavigatorManager().SearchRooms(message.PopStringUntilBreak(null), session.Revision));
        }
    }
}
