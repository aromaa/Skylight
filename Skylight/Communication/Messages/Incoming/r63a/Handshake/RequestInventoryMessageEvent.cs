using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class RequestInventoryMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            //session.SendMessage(session.GetHabbo().GetInventoryManager().SerializeAllItems());

            //session.SendMessage(session.GetHabbo().GetInventoryManager().SerializeFloorItems());
            //session.SendMessage(session.GetHabbo().GetInventoryManager().SerializeWallItems());

            session.GetHabbo().GetInventoryManager().SerializeFloorItemsSplitd();
            session.GetHabbo().GetInventoryManager().SerializeWallItemsSplitd();
        }
    }
}
