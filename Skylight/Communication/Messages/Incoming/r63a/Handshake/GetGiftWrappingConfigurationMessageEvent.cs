using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class GetGiftWrappingConfigurationMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message_.Init(r63aOutgoing.GiftWrappingConfiguration);
            message_.AppendBoolean(true); //advanced
            message_.AppendInt32(1); //price

            List<Item> newBoxes = Skylight.GetGame().GetCatalogManager().GetNewGifts();
            message_.AppendInt32(newBoxes.Count); //box count
            foreach (Item item in newBoxes)
            {
                message_.AppendInt32(item.SpriteID);
            }

            message_.AppendInt32(7); //box count
            for (int i = 0; i < 7; i++)
            {
                message_.AppendInt32(i);
            }

            message_.AppendInt32(11); //ribbons count
            for (int i = 0; i < 11; i++)
            {
                message_.AppendInt32(i);
            } 
            session.SendMessage(message_);
        }
    }
}
