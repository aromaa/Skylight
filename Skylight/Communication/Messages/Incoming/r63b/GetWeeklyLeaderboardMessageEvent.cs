using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class GetWeeklyLeaderboardMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            int gameId = message.PopWiredInt32();
            int unknown = message.PopWiredInt32(); //int boolean
            int uknown2 = message.PopWiredInt32();
            int unknown3 = message.PopWiredInt32();
            int unknown4 = message.PopWiredInt32();
            int unknown5 = message.PopWiredInt32();
        }
    }
}
