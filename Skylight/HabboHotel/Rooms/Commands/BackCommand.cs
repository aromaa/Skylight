using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class BackCommand : Command
    {
        public override string CommandInfo()
        {
            return ":back - Come back from idle";
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
            session.GetHabbo().GetRoomSession().GetRoomUser().Speak("Has come back", false);
            return true;
        }
    }
}
