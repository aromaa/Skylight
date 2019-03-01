using SkylightEmulator.HabboHotel.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Trade
{
    public class TradeCancelComposerHandler : OutgoingHandler
    {
        public uint Canceled;
        public TradeCancelErrorCode ErrorCode;

        public TradeCancelComposerHandler(uint cancaled, TradeCancelErrorCode errorCode = TradeCancelErrorCode.None)
        {
            this.Canceled = cancaled;
            this.ErrorCode = errorCode;
        }
    }
}
