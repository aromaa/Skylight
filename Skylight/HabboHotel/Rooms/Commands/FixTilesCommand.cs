using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class FixTilesCommand : Command
    {
        public override string CommandInfo()
        {
            return ":fixtiles - If tiles are bugged this should fix it";
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
                session.GetHabbo().GetRoomSession().GetRoom().RoomGamemapManager.UpdateTiles();

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
