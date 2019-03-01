using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users
{
    public class CommandCache
    {
        private readonly uint HabboID;
        private readonly Habbo Habbo;
        public int BuyCommandValue = 1;
        public int TradeXCommandValue = 1;

        public CommandCache(uint habboId, Habbo habbo)
        {
            this.HabboID = habboId;
            this.Habbo = habbo;
        }
    }
}
