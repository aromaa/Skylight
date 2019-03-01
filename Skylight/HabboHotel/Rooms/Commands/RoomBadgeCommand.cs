using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class RoomBadgeCommand : Command
    {
        public override string CommandInfo()
        {
            return ":roombadge [badge] - Gives badge to everyone in room";
        }

        public override string RequiredPermission()
        {
            return "cmd_roombadge";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_roombadge"))
                {
                    string badge = args[1];

                    foreach(RoomUnitUser user in session.GetHabbo().GetRoomSession().GetRoom().RoomUserManager.GetRealUsers())
                    {
                        try
                        {
                            user.Session.GetHabbo().GetBadgeManager().AddBadge(badge, 0, true);
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
