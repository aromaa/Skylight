using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class MakesayCommand : Command
    {
        public override string CommandInfo()
        {
            return ":makesay [user] [message] - Forces user to say the message";
        }

        public override string RequiredPermission()
        {
            return "cmd_makesay";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 3)
            {
                if (session.GetHabbo().HasPermission("cmd_makesay"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        target.GetHabbo().GetRoomSession().GetRoomUser().Speak(TextUtilies.MergeArrayToString(args, 2), false);

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
