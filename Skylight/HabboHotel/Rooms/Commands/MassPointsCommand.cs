using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class MassPointsCommand : Command
    {
        public override string CommandInfo()
        {
            return ":masspoints [type] [amount] - Gives x amount of points at selected type for evey users online";
        }

        public override string RequiredPermission()
        {
            return "cmd_masspoints";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 3)
            {
                if (session.GetHabbo().HasPermission("cmd_masspoints"))
                {
                    int type = 0;
                    int amount = 0;
                    if (int.TryParse(args[1], out type) && int.TryParse(args[2], out amount))
                    {
                        foreach (GameClient session_ in Skylight.GetGame().GetGameClientManager().GetClients())
                        {
                            try
                            {
                                session_.GetHabbo().AddActivityPoints(type, amount);
                                session_.GetHabbo().UpdateActivityPoints(type, true);
                                session_.SendNotif("You just received " + amount + " points from staff!");
                            }
                            catch
                            {

                            }
                        }

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
