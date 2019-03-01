using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class RequestInventoryItemsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            //session.SendMessage(session.GetHabbo().GetInventoryManager().SerializeAllItems());
            session.GetHabbo().GetInventoryManager().SerializeAllItemsSplit();
        }
    }
}
