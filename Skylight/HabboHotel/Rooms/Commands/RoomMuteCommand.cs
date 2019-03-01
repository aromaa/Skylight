using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class RoomMuteCommand : Command
    {
        public override string CommandInfo()
        {
            return ":roommute - Mutes the whole room";
        }

        public override string RequiredPermission()
        {
            return "cmd_roommute";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().HasPermission("cmd_roommute"))
            {
                session.GetHabbo().GetRoomSession().GetRoom().RoomMute = !session.GetHabbo().GetRoomSession().GetRoom().RoomMute;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
