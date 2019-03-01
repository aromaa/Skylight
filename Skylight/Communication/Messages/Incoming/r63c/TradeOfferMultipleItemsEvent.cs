using SkylightEmulator.Communication.Messages.Incoming.Handlers.Trade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class TradeOfferMultipleItemsEvent : TradeOfferMultipleItemsEventHandler
    {
        public override void Handle(GameClient session, ClientMessage message)
        {
            this.ItemIds = new uint[message.PopWiredInt32()];
            for(int i = 0; i < this.ItemIds.Length; i++)
            {
                this.ItemIds[i] = message.PopWiredUInt();
            }

            base.Handle(session, message);
        }
    }
}
