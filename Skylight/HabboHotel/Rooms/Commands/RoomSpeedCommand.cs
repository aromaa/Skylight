using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class RoomSpeedCommand : Command
    {
        public override string CommandInfo()
        {
            return ":roomspeed <seconds> - How often room is cycled, default: 0.48";
        }

        public override string RequiredPermission()
        {
            return "cmd_roomspeed";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_roomspeed"))
                {
                    int speed = int.Parse(args[1]);
                    if (speed < 0)
                    {
                        speed = 0;
                    }

                    session.GetHabbo().GetRoomSession().GetRoom().RoomCycleInterval = speed;
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
