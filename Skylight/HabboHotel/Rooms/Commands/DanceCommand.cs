using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class DanceCommand : Command
    {
        public override string CommandInfo()
        {
            return ":dance [user] <dance id> - Forces the user to dance";
        }

        public override string RequiredPermission()
        {
            return "cmd_dance";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_dance"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        target.GetHabbo().GetRoomSession().GetRoomUser().SetHanditem(0);

                        int dance = 1;
                        if (args.Length >= 3)
                        {
                            int.TryParse(args[2], out dance);
                        }

                        if (dance < 0 || dance > 4)
                        {
                            dance = 0;
                        }

                        target.GetHabbo().GetRoomSession().GetRoomUser().SetDance(dance);
                    }
                    return true;
                }
            }

            return false;
        }
    }
}
