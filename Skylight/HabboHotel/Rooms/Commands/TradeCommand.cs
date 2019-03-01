using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class TradeCommand : Command
    {
        public override string CommandInfo()
        {
            return ":trade [amount] - Trade once x amount of items";
        }

        public override string RequiredPermission()
        {
            return "";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (!int.TryParse(args[1], out int newTradeX) || newTradeX <= 0)
                {
                    session.SendNotif("Value must be positive");
                }
                else
                {
                    session.GetHabbo().GetCommandCache().TradeXCommandValue = newTradeX;
                }
                return true;
            }

            return false;
        }
    }
}
