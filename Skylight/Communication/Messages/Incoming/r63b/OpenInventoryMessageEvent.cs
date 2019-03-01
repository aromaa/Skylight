using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class OpenInventoryMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            session.GetHabbo().GetInventoryManager().SerializeFloorItemsSplitd();
            session.GetHabbo().GetInventoryManager().SerializeWallItemsSplitd();
        }
    }
}
