using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class GiftPointsCommand : Command
    {
        public override string CommandInfo()
        {
            return ":giftpoints [user] [amount] - Gives x amount of gift points to user";
        }

        public override string RequiredPermission()
        {
            return "cmd_giftpoints";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 3)
            {
                if (session.GetHabbo().HasPermission("cmd_giftpoints"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        target.GetHabbo().AddActivityPoints(3, int.Parse(args[2]));
                        target.GetHabbo().UpdateActivityPoints(3, true);
                        target.SendNotif("You just received " + int.Parse(args[2]) + " gift points from staff!");

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
