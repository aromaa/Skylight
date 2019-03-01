using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class CoordsCommand : Command
    {
        public override string CommandInfo()
        {
            return ":coords - Returns the coordinates where you are standing";
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
            RoomUnit user = session.GetHabbo().GetRoomSession().GetRoomUser();
            session.SendNotif("X: " + user.X + "\nY: " + user.Y + "\nZ: " + user.Z + "\nRotation: " + user.HeadRotation);
            return true;
        }
    }
}
