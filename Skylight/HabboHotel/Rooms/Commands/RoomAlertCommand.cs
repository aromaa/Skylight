using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class RoomAlertCommand : Command
    {
        public override string CommandInfo()
        {
            return ":roomalert [message] - Sends alert to everyone in the room";
        }

        public override string RequiredPermission()
        {
            return "cmd_roomalert";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_roomalert"))
                {
                    string message = TextUtilies.MergeArrayToString(args, 1);

                    foreach (RoomUnitUser user in session.GetHabbo().GetRoomSession().GetRoom().RoomUserManager.GetRealUsers())
                    {
                        try
                        {
                            user.Session.SendNotif(message);
                        }
                        catch
                        {

                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
