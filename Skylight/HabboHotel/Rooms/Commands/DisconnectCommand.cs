using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class DisconnectCommand : Command
    {
        public override string CommandInfo()
        {
            return ":disconnect [user] - Disconnects user";
        }

        public override string RequiredPermission()
        {
            return "cmd_disconnect";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_disconnect"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        if (target.GetHabbo().Rank < session.GetHabbo().Rank)
                        {
                            target.Stop("Disconnect command");
                        }
                        else
                        {
                            session.SendNotif("You are not allowed to disconnect this user!");
                        }

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
