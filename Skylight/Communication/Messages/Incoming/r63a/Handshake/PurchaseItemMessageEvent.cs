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
    class PurchaseItemMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            int pageId = message.PopWiredInt32();
            uint itemId = message.PopWiredUInt();
            string extraData = message.PopFixedString();

            Skylight.GetGame().GetCatalogManager().BuyItem(session, pageId, itemId, extraData, session.GetHabbo().GetCommandCache().BuyCommandValue, false);
            session.GetHabbo().GetCommandCache().BuyCommandValue = 1;
        }
    }
}
