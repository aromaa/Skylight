using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class RemoveBadgeCommand : Command
    {
        public override string CommandInfo()
        {
            return ":removebadge [user] [badge] - Removes the badge from user";
        }

        public override string RequiredPermission()
        {
            return "cmd_removebadge";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 3)
            {
                if (session.GetHabbo().HasPermission("cmd_removebadge"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        target.GetHabbo().GetBadgeManager().RemoveBadge(args[2]);
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
    }
}
