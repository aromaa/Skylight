using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class MoonwalkCommand : Command
    {
        public override string CommandInfo()
        {
            return ":moonwalk - Enables/disables moonwalk";
        }

        public override string RequiredPermission()
        {
            return "cmd_moonwalk";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            session.GetHabbo().GetRoomSession().GetRoomUser().Moonwalk = !session.GetHabbo().GetRoomSession().GetRoomUser().Moonwalk;
            return true;
        }
    }
}
