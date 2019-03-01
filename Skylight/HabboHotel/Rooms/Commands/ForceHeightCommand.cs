using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class ForceHeightCommand : Command
    {
        public override string CommandInfo()
        {
            return ":fh [height] - Set your placement height, :fh -1 to reset";
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
            if (args.Length >= 2)
            {
                if (double.TryParse(args[1], out double height))
                {
                    if (height < 0.0)
                    {
                        height = -1.0;
                    }
                    else if (height >= 500.0)
                    {
                        height = 500.0;
                    }

                    session.GetHabbo().GetRoomSession().GetRoomUser().ForceHeight = height;
                }
                else
                {
                    session.SendNotif("Not valid decimal");
                }
            }
            else
            {
                session.GetHabbo().GetRoomSession().GetRoomUser().ForceHeight = -1.0;
            }

            return true;
        }
    }
}
