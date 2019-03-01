using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class BotAction
    {
        public string Action;
        public string Value;
        public int Tick;

        public BotAction(string action, string value, int tick)
        {
            this.Action = action;
            this.Value = value;
            this.Tick = tick;
        }
    }
}
