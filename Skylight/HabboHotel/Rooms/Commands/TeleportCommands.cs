using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class TeleportCommands : Command
    {
        public override string CommandInfo()
        {
            return ":teleport - Enable or disable teleportation";
        }

        public override string RequiredPermission()
        {
            return "cmd_teleport";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            session.GetHabbo().GetRoomSession().GetRoomUser().Teleport = !session.GetHabbo().GetRoomSession().GetRoomUser().Teleport;
            return true;
        }
    }
}
