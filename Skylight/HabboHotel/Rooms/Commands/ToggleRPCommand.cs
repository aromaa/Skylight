using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class ToggleRPCommand : Command
    {
        public override string CommandInfo()
        {
            return ":togglerp - Toggles RP commands on your room";
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
            if (session.GetHabbo().GetRoomSession().GetRoom().HaveOwnerRights(session))
            {
                session.GetHabbo().GetRoomSession().GetRoom().RoomData.ExtraData.RoleplayEnabled = !session.GetHabbo().GetRoomSession().GetRoom().RoomData.ExtraData.RoleplayEnabled;

                if (session.GetHabbo().GetRoomSession().GetRoom().RoomData.ExtraData.RoleplayEnabled)
                {
                    session.SendNotif("RP is now enabled!");
                }
                else
                {
                    session.SendNotif("RP is now disabled!");
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
