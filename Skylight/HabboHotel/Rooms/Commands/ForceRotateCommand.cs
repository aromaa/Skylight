using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class ForceRotateCommand : Command
    {
        public override string CommandInfo()
        {
            return ":fr <rotation (0-8)> - Set your placement rotation, :fr -1 to reset";
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
                int rotation = 0;
                int.TryParse(args[1], out rotation);
                if (rotation < 0)
                {
                    rotation = -1;
                }
                else if (rotation > 8)
                {
                    rotation = 8;
                }

                session.GetHabbo().GetRoomSession().GetRoomUser().ForceRotate = rotation;
            }
            else
            {
                session.GetHabbo().GetRoomSession().GetRoomUser().ForceRotate = -1;
            }

            return true;
        }
    }
}
