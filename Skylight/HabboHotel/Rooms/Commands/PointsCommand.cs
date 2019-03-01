using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class PointsCommand : Command
    {
        public override string CommandInfo()
        {
            return ":points [user] [type] [amount] - Gives user x amount of points of selected type";
        }

        public override string RequiredPermission()
        {
            return "cmd_points";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 4)
            {
                if (session.GetHabbo().HasPermission("cmd_points"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        int type = 0;
                        int amount = 0;

                        if (int.TryParse(args[2], out type) && int.TryParse(args[3], out amount))
                        {
                            target.GetHabbo().AddActivityPoints(type, amount);
                            target.GetHabbo().UpdateActivityPoints(type, true);
                            target.SendNotif("You just received " + amount + " points from staff!");

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
            else
            {
                return false;
            }
        }
    }
}
