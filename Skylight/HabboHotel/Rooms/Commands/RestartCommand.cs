using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class RestartCommand : Command
    {
        public override string CommandInfo()
        {
            return ":restart - Restarts the emulator";
        }

        public override string RequiredPermission()
        {
            return "cmd_restart";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().HasPermission("cmd_restart"))
            {
                Task task = new Task(new Action(() => Program.Destroy(true, false, false, true)));
                task.Start();

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
