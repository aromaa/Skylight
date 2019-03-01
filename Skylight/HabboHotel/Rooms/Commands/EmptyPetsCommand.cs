using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class EmptyPetsCommand : Command
    {
        public override string CommandInfo()
        {
            return ":emptypets - Clears your pets";
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            session.GetHabbo().GetInventoryManager().DeleteAllPets();
            return true;
        }

        public override string RequiredPermission()
        {
            return "";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }
    }
}
