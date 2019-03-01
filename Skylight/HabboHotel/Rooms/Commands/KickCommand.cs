using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class KickCommand : Command
    {
        public override string CommandInfo()
        {
            return ":kick [user] - Kicks the user";
        }

        public override string RequiredPermission()
        {
            return "cmd_kick";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_kick"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        if (target.GetHabbo().Rank < session.GetHabbo().Rank)
                        {
                            target.GetHabbo().GetRoomSession().GetRoom().RoomUserManager.KickUser(target, true);
                        }
                        else
                        {
                            session.SendNotif("You are not allowed to kick this user!");
                        }
                    }
                    else
                    {
                        session.SendNotif("Unable to find user!");
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
