using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class RaveCommand : Command
    {
        public override string CommandInfo()
        {
            return ":rave";
        }

        public override string RequiredPermission()
        {
            return "cmd_rave";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().HasPermission("cmd_rave"))
            {
                foreach(RoomUnitUser user in session.GetHabbo().GetRoomSession().GetRoom().RoomUserManager.GetRealUsers())
                {
                    user.SetDance(1);
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
