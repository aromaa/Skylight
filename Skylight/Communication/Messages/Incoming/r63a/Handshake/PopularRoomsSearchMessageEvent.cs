using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class PopularRoomsSearchMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null)
            {
                session.SendMessage(Skylight.GetGame().GetNavigatorManager().GetPopularRooms(session, message.PopFixedInt32()));
            }
        }
    }
}
