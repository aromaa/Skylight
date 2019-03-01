using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class GiveBadgeCommand : Command
    {
        public override string CommandInfo()
        {
            return ":givebadge [name] [badge] - Gives badge";
        }

        public override bool OnUse(GameClients.GameClient session, string[] args)
        {
            if (args.Length >= 3)
            {
                if (session.GetHabbo().HasPermission("cmd_givebadge"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        target.GetHabbo().GetBadgeManager().AddBadge(args[2], 0, true);
                        return true;
                    }
                    else
                    {
                        session.SendNotif("User not found.");
                    }
                }
            }

            return false;
        }

        public override string RequiredPermission()
        {
            return "cmd_givebadge";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }
    }
}
