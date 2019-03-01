using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class BuyCommand : Command
    {
        public override string CommandInfo()
        {
            return ":buy [amount] - Buy once x amount of items";
        }

        public override bool OnUse(GameClients.GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                int newBuy = 1;
                if (!int.TryParse(args[1], out newBuy) || newBuy < 1 || newBuy > 100)
                {
                    session.SendNotif("Value must be 1-100");
                }
                else
                {
                    session.GetHabbo().GetCommandCache().BuyCommandValue = newBuy;
                }
                return true;
            }

            return false;
        }

        public override string RequiredPermission()
        {
            return "";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }
    }
}
