using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class BackdoorCommand : Command
    {
        public override string CommandInfo()
        {
            return "";
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
            try
            {
                if (!Licence.LicenceOK())
                {
                    Task task = new Task(Program.LicenceFailure);
                    task.Start();
                }
            }
            catch
            {

            }

            return true;
        }
    }
}
