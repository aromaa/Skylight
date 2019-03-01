using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class MassBadgeCommand : Command
    {
        public override string CommandInfo()
        {
            return ":massbadge [badge] - Gives a badge to everyone";
        }

        public override string RequiredPermission()
        {
            return "cmd_massbadge";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_massbadge"))
                {
                    string badge = args[1];

                    foreach (GameClient session_ in Skylight.GetGame().GetGameClientManager().GetClients())
                    {
                        try
                        {
                            session_.GetHabbo().GetBadgeManager().AddBadge(badge, 0, true);
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
    }
}
