using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class ShutdownCommmand : Command
    {
        public override string CommandInfo()
        {
            return ":shutdown - Shutdowns the emulator";
        }

        public override string RequiredPermission()
        {
            return "cmd_shutdown";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().HasPermission("cmd_shutdown"))
            {
                Task task = new Task(new Action(() => Program.Destroy()));
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
