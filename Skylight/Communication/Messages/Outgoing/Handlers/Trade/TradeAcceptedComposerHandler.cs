using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Trade
{
    public class TradeAcceptedComposerHandler : OutgoingHandler
    {
        public uint UserID { get; }
        public bool Accepted { get; }

        public TradeAcceptedComposerHandler(uint userId, bool accepted)
        {
            this.UserID = userId;
            this.Accepted = accepted;
        }
    }
}
