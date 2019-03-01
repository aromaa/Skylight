using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class SetspeedCommand : Command
    {
        public override string CommandInfo()
        {
            return ":setspeed <speed> - Sets the speed of rollers";
        }

        public override string RequiredPermission()
        {
            return "cmd_setspeed";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_setspeed"))
                {
                    int speed = 0;
                    if (int.TryParse(args[1], out speed))
                    {
                        if (speed < 0)
                        {
                            speed = 0;
                        }

                        session.GetHabbo().GetRoomSession().GetRoom().RollerSpeed = speed;
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
            else
            {
                return false;
            }
        }
    }
}
