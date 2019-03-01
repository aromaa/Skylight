using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class OverrideCommand : Command
    {
        public override string CommandInfo()
        {
            return ":override - Allows you walk to over any furnis with any height";
        }

        public override string RequiredPermission()
        {
            return "cmd_override";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().HasPermission("cmd_override"))
            {
                session.GetHabbo().GetRoomSession().GetRoomUser().Override = !session.GetHabbo().GetRoomSession().GetRoomUser().Override;
                if (session.GetHabbo().GetRoomSession().GetRoomUser().Override)
                {
                    session.SendNotif("Override is enabled!");
                }
                else
                {
                    session.SendNotif("Override is disabled!");
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
