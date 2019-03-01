using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Core;
using SkylightEmulator.Utilies;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class PurchaseMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string[] data = message.PopStringUntilBreak(null).Split(Convert.ToChar(13));
            int pageId = int.Parse(data[1]);
            uint itemId = uint.Parse(data[3]);
            string extraData = data[4];

            Skylight.GetGame().GetCatalogManager().BuyItem(session, pageId, itemId, extraData, session.GetHabbo().GetCommandCache().BuyCommandValue, data[5] == "1", data[6], TextUtilies.FilterString(data[7]));
            session.GetHabbo().GetCommandCache().BuyCommandValue = 1;
        }
    }
}
