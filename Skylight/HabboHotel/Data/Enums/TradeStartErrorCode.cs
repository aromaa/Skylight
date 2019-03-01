using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Data.Enums
{
    public enum TradeStartErrorCode
    {
        TradingDisabled = 1,
        YourTradingDisabled = 2,
        TargetCannotTrade = 4,
        TradingNotAllowInRoom = 6,
        TradeOpen = 7,
        TargetTrading = 8,
    }
}
