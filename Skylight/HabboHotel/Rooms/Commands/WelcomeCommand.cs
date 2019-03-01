using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class WelcomeCommand : Command
    {
        public override string CommandInfo()
        {
            return ":welcome [user] - Welcomes the user";
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
                GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                if (target != null)
                {
                    RoomUnit me = session.GetHabbo().GetRoomSession().GetRoomUser();
                    RoomUnit other = target.GetHabbo().GetRoomSession().GetRoomUser();
                    if (target.GetHabbo().GetRoomSession().IsInRoom && target.GetHabbo().GetRoomSession().CurrentRoomID == session.GetHabbo().GetRoomSession().CurrentRoomID)
                    {
                        me.Speak("Welcome to Cabbo Hotel " + target.GetHabbo().Username + ", we hope you enjoy your stay and make sure to stay safe.", false);

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
