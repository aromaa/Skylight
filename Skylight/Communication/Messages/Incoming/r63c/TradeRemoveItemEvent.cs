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
    class TradeRemoveItemEvent : TradeRemoveItemEventHandler
    {
        public override void Handle(GameClient session, ClientMessage message)
        {
            this.ItemID = message.PopWiredUInt();

            base.Handle(session, message);
        }
    }
}
