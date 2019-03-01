using SkylightEmulator.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Trade
{
    public class TradeUpdateComposerHandler : OutgoingHandler
    {
        public TradeUser[] Traders;

        public TradeUpdateComposerHandler(TradeUser[] traders)
        {
            this.Traders = traders;
        }
    }
}
