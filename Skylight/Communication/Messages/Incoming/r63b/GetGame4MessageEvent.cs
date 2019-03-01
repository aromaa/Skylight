using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class GetGame4MessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            int gameId = message.PopWiredInt32();
        }
    }
}
