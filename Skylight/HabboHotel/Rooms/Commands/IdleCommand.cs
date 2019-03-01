using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class IdleCommand : Command
    {
        public override string CommandInfo()
        {
            return ":idle - Goto idle";
        }

        public override string RequiredPermission()
        {
            return "";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            session.GetHabbo().GetRoomSession().GetRoomUser().Speak("Has gone idle", false);
            return true;
        }
    }
}
