using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class RoomKickCommand : Command
    {
        public override string CommandInfo()
        {
            return ":roomkick <reason> - Kicks everyone in the room";
        }

        public override string RequiredPermission()
        {
            return "cmd_roomkick";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().HasPermission("cmd_roomkick"))
            {
                string reason = TextUtilies.MergeArrayToString(args, 1);
                foreach(RoomUnitUser user in session.GetHabbo().GetRoomSession().GetRoom().RoomUserManager.GetRealUsers())
                {
                    if (user.Session.GetHabbo().Rank < session.GetHabbo().Rank)
                    {
                        if (!string.IsNullOrEmpty(reason))
                        {
                            user.Session.SendNotif(reason);
                        }

                        session.GetHabbo().GetRoomSession().GetRoom().RoomUserManager.KickUser(user.Session, false);
                    }
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
