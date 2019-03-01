using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class EmptyCommand : Command
    {
        public override string CommandInfo()
        {
            return ":empty [user] - Emptys users inventory";
        }

        public override string RequiredPermission()
        {
            return "cmd_empty";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_empty"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        target.GetHabbo().GetInventoryManager().DeleteAllItems();

                        session.SendNotif("Inventory cleaned!");

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
