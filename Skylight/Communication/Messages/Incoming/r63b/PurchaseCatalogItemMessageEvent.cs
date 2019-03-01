using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class PurchaseCatalogItemMessageEvent : IncomingPacket
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
