using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class PickallCommand : Command
    {
        public override string CommandInfo()
        {
            return "pickall - Picks up all items from room";
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().GetRoomSession().GetRoom() != null && session.GetHabbo().GetRoomSession().GetRoom().HaveOwnerRights(session))
            {
                session.GetHabbo().GetRoomSession().GetRoom().RoomItemManager.Pickall(session);
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
            return true;
        }
    }
}
