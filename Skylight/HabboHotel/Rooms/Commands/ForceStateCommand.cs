using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class ForceStateCommand : Command
    {
        public override string CommandInfo()
        {
            return ":fs [state] - Set your placement state, :fs -1 to reset";
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
                int state = 0;
                if (int.TryParse(args[1], out state))
                {
                    if (state < 0)
                    {
                        state = -1;
                    }

                    session.GetHabbo().GetRoomSession().GetRoomUser().ForceState = state;
                }
                else
                {
                    session.SendNotif("Not valid number!");
                }
            }
            else
            {
                session.GetHabbo().GetRoomSession().GetRoomUser().ForceState = -1;
            }

            return true;
        }
    }
}
