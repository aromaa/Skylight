using SkylightEmulator.HabboHotel.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Trade
{
    public class TradeStartErrorComposerHandler : OutgoingHandler
    {
        public TradeStartErrorCode ErrorCode;
        public string Target;

        public TradeStartErrorComposerHandler(TradeStartErrorCode errorCode, string target = "")
        {
            this.ErrorCode = errorCode;
            this.Target = target;
        }
    }
}
