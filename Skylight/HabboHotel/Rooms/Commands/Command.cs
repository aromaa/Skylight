using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    public abstract class Command
    {
        public abstract string CommandInfo();
        public abstract string RequiredPermission();
        public abstract bool ShouldBeLogged();
        public abstract bool OnUse(GameClient session, string[] args);
    }
}
