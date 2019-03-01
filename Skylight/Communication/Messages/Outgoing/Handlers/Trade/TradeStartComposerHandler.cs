using SkylightEmulator.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Trade
{
    public class TradeStartComposerHandler : OutgoingHandler
    {
        public TradeUser[] Traders;

        public TradeStartComposerHandler(TradeUser[] traders)
        {
            this.Traders = traders;
        }
    }
}
