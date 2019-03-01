using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class BuyAsGiftMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            int pageId = message.PopWiredInt32();
            uint itemId = message.PopWiredUInt();
            string extraData = message.PopFixedString();
            string receiverUsername = TextUtilies.FilterString(message.PopFixedString());
            string giftMessage = TextUtilies.FilterString(message.PopFixedString());
            int giftSpriteId = message.PopWiredInt32(); //0 for no special gift
            int giftBoxId = message.PopWiredInt32();
            int giftRibbonId = message.PopWiredInt32();

            Skylight.GetGame().GetCatalogManager().BuyItem(session, pageId, itemId, extraData, 1, true, receiverUsername, giftMessage, giftSpriteId, giftBoxId, giftRibbonId);
        }
    }
}
