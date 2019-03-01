using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class BackupCommand : Command
    {
        public override string CommandInfo()
        {
            return ":backup <compress> <shutdown> - Shutdowns emu, backups database, restart emu, optionally can compressed & shutdown";
        }

        public override string RequiredPermission()
        {
            return "cmd_backup";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().HasPermission("cmd_backup"))
            {
                bool compress = false;
                if (args.Length >= 2)
                {
                    if (!bool.TryParse(args[1], out compress))
                    {
                        return false;
                    }
                }

                bool shutdown = false;
                if (args.Length >= 3)
                {
                    if (!bool.TryParse(args[2], out shutdown))
                    {
                        return false;
                    }
                }

                Task task = new Task(new Action(() => Program.Destroy(true, true, compress, !shutdown)));
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
