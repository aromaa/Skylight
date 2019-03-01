using SkylightEmulator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class UnloadCommand : Command
    {
        public override string CommandInfo()
        {
            return ":unload - Kicks everyone and saves furnitures";
        }

        public override bool OnUse(GameClients.GameClient session, string[] args)
        {
            if (session.GetHabbo().GetRoomSession().GetRoom() != null && session.GetHabbo().GetRoomSession().GetRoom().HaveOwnerRights(session))
            {
                Skylight.GetGame().GetRoomManager().UnloadRoom(session.GetHabbo().GetRoomSession().GetRoom());
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string RequiredPermission()
        {
            return "";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }
    }
}
