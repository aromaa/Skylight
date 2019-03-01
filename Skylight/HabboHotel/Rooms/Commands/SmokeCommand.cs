using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class SmokeCommand : Command
    {
        public override string CommandInfo()
        {
            return ":smoke - ";
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
            session.GetHabbo().GetRoomSession().GetRoomUser().Speak("*Pulls out my pack of cigarettes and lights one up*", false);

            return true;
        }
    }
}
